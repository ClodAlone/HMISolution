////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	CommJob.cs
//
// summary:	Implements the communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using DriverBaseInterfaces;
using System.Threading;

namespace DriverCodeBaseEx
{
    /// <summary>   protocol's task of the base communication driver. </summary>
    public abstract class CommJob
    {
        [Flags] public enum ScheduleProperties
        {
            None = 0,
            SetSyncroExec = 1,
            UnSetSyncroExec = 2,
            SetIsPending = 4,
            UnSetIsPending = 8,
            UnSetQueued = 16,
        }

        public enum RWStates
        {
            Standard,
            ReadForRW,
            WriteForRW,
        }

        public enum CustomJobCheckStates
        {
            None,
            Read,
            Write,
        }

        public const uint JOB_NOT_SCHEDULABLE = uint.MaxValue;

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="station">                          The station. </param>
        /// <param name="settings" type="CommJobSettings">  Options for controlling the operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommJob(Station station, CommJobSettings settings)
            : this(station)
        {
            _Type = settings.Type;
            _SamplingInterval = settings.SamplingInterval;
            _SwapBytes = settings.SwapBytes;
            _SwapWords = settings.SwapWords;
            _OutputAtStartup = settings.OutputAtStartup;
            _ElementNumber = settings.ElementNumber;

            TotalJobSize = settings.TotalJobSize;

            _ConditionalVariableName = settings.ConditionalVariableName;
            _ConditionalVariableId = settings.ConditionalVariableId;
            if (StateCommandVariable.IsTagsSet(_ConditionalVariableName, _ConditionalVariableId))
            {
                jobConditionalVariable = new StateCommandVariable(StateCommandVariable.ObjectTypes.Job, _ConditionalVariableName, _ConditionalVariableId);                
            }

            _OffsetVariableName = settings.OffsetVariableName;
            _OffsetVariableId = settings.OffsetVariableId;
            if (ObservedVariable.IsTagsSet(_OffsetVariableName, _OffsetVariableId))
            {
                jobOffsetVariable = new ObservedVariable(_OffsetVariableName, _OffsetVariableId);
            }

            InUse = false;

            LastExecutionTime = DateTime.UtcNow;
            StartExecutionTime = DateTime.UtcNow;

            RWState = RWStates.Standard;

            foreach (var tagsettings in settings.Tags)
            {
                Tag t = Station.GetCommDriver().CreateTag(new TagDefinition
                {
                    NodeId = tagsettings.NodeId,
                    DynamicSettings = tagsettings.DynamicSettings,
                    DataType = tagsettings.DataType,
                    SamplingInterval = tagsettings.SamplingInterval,
                    ArrayDimension = tagsettings.ArrayDimension,
                    InitialValue = tagsettings.InitialValue,
                    MemberOrder = tagsettings.MemberOrder,
                    Name = tagsettings.Name,
                }, tagsettings.ByteOffset, tagsettings.BitOffset);
                TagsList.Add(t);

                t.bIsValid = true;

                station.AddToMapTagJob(t.TagNode.NodeId, this);
                station.GetCommDriver().AddToTagToStationMap(t.TagNode.NodeId, station);
            }
            
            if (settings.StructTag != null)
            {
                StructTag = Station.GetCommDriver().CreateTag(new TagDefinition
                {
                    NodeId = settings.StructTag.NodeId,
                    DynamicSettings = settings.StructTag.DynamicSettings,
                    DataType = settings.StructTag.DataType,
                    SamplingInterval = settings.StructTag.SamplingInterval,
                    ArrayDimension = settings.StructTag.ArrayDimension,
                    InitialValue = settings.StructTag.InitialValue,
                    MemberOrder = settings.StructTag.MemberOrder,
                    Name = settings.StructTag.Name,
                }, settings.StructTag.ByteOffset, settings.StructTag.BitOffset);

                station.AddToMapTagJob(StructTag.TagNode.NodeId, this);
                station.GetCommDriver().AddToTagToStationMap(StructTag.TagNode.NodeId, station);

                if (IsCustomJob(StructTag))
                    // create internal jobs (read/write) to manage atomic structure
                    CreateCustomJob(StructTag);
            } else if (TagsList.Count == 1)
            {
                if (IsCustomJob(TagsList[0]))                    
                    CreateCustomJob(TagsList[0]);
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a prototype method to tag list to 'node'. </summary>
        ///
        /// <param name="station">              The station. </param>
        /// <param name="node" type="NodeId">   The node. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void AddPrototypeMethodToTagList(Station station, NodeId node)
        {
            List<TagDefinition> tList = new List<TagDefinition>();
            station.GetCommDriver().OnTagPrototypeQuery(node, ref tList);
            foreach (var tag in tList)
            {
                if (tag.DataType.IdType == IdType.Guid || (tag.DataType.IdType == IdType.Numeric && (uint)tag.DataType.Identifier == ObjectTypes.FolderType))
                    AddPrototypeMethodToTagList(station, tag.NodeId);
                else
                {
                    Tag t = station.GetCommDriver().CreateTag(tag);
                    if (t.DynSettings.MethodID > -1)
                    {
                        station.AddToMapTagJob(t.TagNode.NodeId, this);
                        station.GetCommDriver().AddToTagToStationMap(t.TagNode.NodeId, station);
                        TagsList.Add(t);
                    }
                    else
                    {
                        station.AddToMapTagJob(t.TagNode.NodeId, this);
                        station.GetCommDriver().AddToTagToStationMap(t.TagNode.NodeId, station);
                        t.ByteOffset = TotalJobSize;
                        TagsList.Add(t);
                        TotalJobSize += t.Size;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets simple tag list. </summary>
        ///
        /// <param name="t" type="TagDefinition">   The TagDefinition to process. </param>
        ///
        /// <returns>   The simple tag list. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected List<TagDefinition> GetSimpleTagList(TagDefinition t)
        {
            List<TagDefinition> l = new List<TagDefinition>();
            if (t.DataType.IdType == IdType.Guid)
            {
                //prototype?
                List<TagDefinition> tList = new List<TagDefinition>();
                Station.GetCommDriver().OnTagPrototypeQuery(t.NodeId, ref tList);
                foreach (var a in tList)
                {
                    l.AddRange(GetSimpleTagList(a));
                }
            }
            else
                l.Add(t);
            return l;
        }

        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="station">              The station. </param>
        /// <param name="defTag" type="Tag">    The definition tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommJob(Station station, Tag defTag)
            :this(station)
        {
            if (defTag.bIsValid)
            {
                _Type = (LinkType)defTag.DynSettings.TagLinkType;
                _SamplingInterval = (uint)defTag.TagNode.SamplingInterval;
                _TagsList.Add(defTag);
                TotalJobSize = defTag.Size;
                MethodID = defTag.DynSettings.MethodID;
                _SwapBytes = defTag.DynSettings.SwapBytes;
                _SwapWords = defTag.DynSettings.SwapWords;
                _OutputAtStartup = defTag.DynSettings.OutputAtStartup;
                _ElementNumber = defTag.DynSettings.ElementNumber;

                _ConditionalVariableName = defTag.DynSettings.ConditionalVariableName;
                _ConditionalVariableId = defTag.DynSettings.ConditionalVariableId;
                if (StateCommandVariable.IsTagsSet(_ConditionalVariableName, _ConditionalVariableId))
                {
                    jobConditionalVariable = new StateCommandVariable(StateCommandVariable.ObjectTypes.Job, _ConditionalVariableName, _ConditionalVariableId);
                }

                _OffsetVariableName = defTag.DynSettings.OffsetVariableName;
                _OffsetVariableId = defTag.DynSettings.OffsetVariableId;
                if (ObservedVariable.IsTagsSet(_OffsetVariableName, _OffsetVariableId))
                {
                    jobOffsetVariable = new ObservedVariable(_OffsetVariableName, _OffsetVariableId);
                }
            }
            else
            {
                IsValid = false;
            }

            if (defTag.TagNode.DataType.IdType == IdType.Guid)
            {
                StructTag = defTag;

                AddPrototypeMethodToTagList(station, defTag.TagNode.NodeId);
                var ProtoList = (from t in TagsList.AsParallel() where t.TagNode.DataType.IdType == IdType.Guid select t).ToList();
                foreach (var t in ProtoList)
                {
                    TagsList.Remove(t);
                }
                foreach (var t in TagsList)
                    t.bIsValid = true;
            }

            if (IsCustomJob(defTag))
                CreateCustomJob(defTag);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="station">  The station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommJob(Station station)
            : this()
        {
            Station = station;
            
            _ConditionalVariableName = String.Empty;
            _ConditionalVariableId = String.Empty;
            jobConditionalVariable = new StateCommandVariable();

            _OffsetVariableName = String.Empty;
            _OffsetVariableId = String.Empty;
            jobOffsetVariable = new ObservedVariable();

            if (Station != null && Station.GetChannel() != null && Station.GetChannel().PollingTimeInError > 0)
            {
                MaxErrorPollingTime = unchecked(Properties.Settings.Default.ErrorPollingTimeMultiplier * Station.GetChannel().PollingTimeInError);
            }
        }
        

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        protected CommJob()
        {
            FirstTime = true;
            IsValid = true;
            InvalidReason = string.Empty;
            ExecuteFirstTime = false;
            IsPending = false;
            ScheduleQueue = CommJobState.UnScheduled;

            _ConditionalVariableName = String.Empty;
            _ConditionalVariableId = String.Empty;
            jobConditionalVariable = new StateCommandVariable();

            _OffsetVariableName = String.Empty;
            _OffsetVariableId = String.Empty;
            jobOffsetVariable = new ObservedVariable();

            ErrorPollingTime = 0;
            MaxErrorPollingTime = Properties.Settings.Default.MaxErrorPollingTime;
        }

        #endregion

        #region Data Members


        public RWStates RWState;

        /// <summary>   The station. </summary>
        public readonly Station Station;

        /// <summary>   Message describing the last error. </summary>
        protected string LastErrorMessage;

        /// <summary>   The lock list object. </summary>
        protected Object lockListObject = new Object();

        /// <summary>  The Conditional variable associated to the job. </summary>
        protected StateCommandVariable jobConditionalVariable;

        /// <summary>  The Offset variable associated to the job. </summary>
        protected ObservedVariable jobOffsetVariable;

        private int MaxErrorPollingTime;
        #endregion

        #region Properties
#if DEBUG
        public string Name { get; set; }
#endif
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the first time. </summary>
        ///
        /// <value> true if first time, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FirstTime { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether this object is valid. </summary>
        ///
        /// <value> true if this object is valid, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsValid { get; protected set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the invalid reason. </summary>
        ///
        /// <value> The invalid reason. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string InvalidReason { get; protected set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the in error state. </summary>
        ///
        /// <value> true if in error state, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        bool _InErrorState = false;
        public bool InErrorState { 
            get { return _InErrorState; } 
            protected set {
                if (value == _InErrorState)
                    return;
                _InErrorState = value; 
                if(Station != null)
                {
                    if (_InErrorState)
                        Station.AddErrorJob(this);
                    else
                        Station.RemoveErrorJob(this);
                }
            } 
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the total number of job size. </summary>
        ///
        /// <value> The total number of job size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint TotalJobSize { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the in use. </summary>
        ///
        /// <value> true if in use, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InUse { get; protected set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the job is pending. </summary>
        ///
        /// <value> true if is pending, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsPending { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the job is in an execution queue. </summary>
        ///
        /// <value> true if is queued, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsQueued
        {
            get { return ScheduleQueue != CommJobState.UnScheduled; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the scheduling queue where job has been placed </summary>
        ///
        /// <value> The queed name </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommJobState ScheduleQueue { get; set; } = CommJobState.UnScheduled;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the execute first time. </summary>
        ///
        /// <value> true if execute first time, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ExecuteFirstTime { get; internal set; }//steve 240811
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the last execution time. </summary>
        ///
        /// <value> The last execution time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime LastExecutionTime { get; set; }
        /// <summary>   The last error time. </summary>
        public DateTime LastErrorTime { get; protected set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the start execution time. </summary>
        ///
        /// <value> The start execution time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime StartExecutionTime { get; set; }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the group string. </summary>
        ///
        /// <value> The group string. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;

                return string.Format("S{0}L{1}T{2:00000}", Station.Name, Type, SamplingInterval);
            }
        }

        /// <summary>
        /// Return if job is a struct
        /// </summary>
        public virtual bool IsStruct()
        { 
            return (StructTag != null);
        }

        public virtual bool IsStruct(Tag defTag)
        {
            return (defTag != null && defTag.TagNode.DataType.IdType == IdType.Guid);
        }

        /// <summary>
        /// Return if job is a struct (Atomic Struct of job of driver that don't split struct)
        /// </summary>
        public virtual bool IsStructAtomic()
        {
            return IsStructAtomic(StructTag);
        }

        public virtual bool IsStructAtomic(Tag defTag)
        {
            return Station.IsStructAtomic(defTag);
        }

        /// <summary>
        /// Return true if is a big job
        /// </summary>
        /// <returns></returns>
        protected bool _IsCustomJob = false;
        public virtual bool IsCustomJob(CustomJobCheckStates state = CustomJobCheckStates.None)
        {
            return _IsCustomJob;
        }

        /// <summary>
        /// Set job as a custom job
        /// </summary>
        public void SetAsACustomJob(bool enabled = true)
        {
            _IsCustomJob = enabled;
        }

        /// <summary>
        /// Return true if tag is a big job
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCustomJob(Tag defTag)
        {
            return _IsCustomJob;
        }        

        /// <summary>   true to syncro execute. </summary>
        private bool _SyncroExec = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the syncro execute. </summary>
        ///
        /// <value> true if syncro execute, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SyncroExec
        {
            get { return _SyncroExec; }
            set
            {
                _SyncroExec = value;
            }
        }
        /// <summary>   Identifier for the method. </summary>
        private int _MethodID = -1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier of the method. </summary>
        ///
        /// <value> The identifier of the method. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int MethodID
        {
            get { return _MethodID; }
            set
            {
                _MethodID = value;
            }
        }
        /// <summary>   true to local method. </summary>
        private bool _LocalMethod = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the local method. </summary>
        ///
        /// <value> true if local method, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool LocalMethod
        {
            get { return _LocalMethod; }
            set
            {
                _LocalMethod = value;
            }
        }
        /// <summary>   The synchro values. </summary>
        private object _SynchroValues;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the synchro values. </summary>
        ///
        /// <value> The synchro values. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public object SynchroValues
        {
            get { return _SynchroValues; }
            set
            {
                _SynchroValues = value;
            }
        }

        /// <summary>   The synchro error. </summary>
        private int _SynchroError = (int)DriverErrorCodes.ErrorNoError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the synchro error. </summary>
        ///
        /// <value> The synchro error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int SynchroError
        {
            get { return _SynchroError; }
            set { _SynchroError = value; }
        }

        /// <summary>   List of tags. </summary>
        private readonly List<Tag> _TagsList = new List<Tag>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a list of tags. </summary>
        ///
        /// <value> A List of tags. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Tag> TagsList
        {
            get {
                        return _TagsList;
                }

        }

        /// <summary>   The tags list to write. </summary>
        private readonly List<Tag> _TagsListToWrite = new List<Tag>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the tags list to write. </summary>
        ///
        /// <value> The tags list to write. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Tag> TagsListToWrite
        {
            get {
                        return _TagsListToWrite;
                }
        }

        public bool IsChildJob
        {
            get { return _ParentJob != null; }
            //set { _IsChildJob = value; }
        }

        public CommJob _ParentJob = null;
        public CommJob ParentJob
        {
            get { return _ParentJob; }
            set { _ParentJob = value; }
        }

        /// <summary>
        /// Error code of Struct Atomic child job
        /// </summary>
        private DriverErrorCodes _ChildJobErrorCode = DriverErrorCodes.ErrorNoError;
        public DriverErrorCodes ChildJobErrorCode
        {
            get { return _ChildJobErrorCode; }
            set { _ChildJobErrorCode = value; }
        }

        /// <summary>
        /// List of child use to read tags of Big job
        /// </summary>
        Dictionary<NodeId, CommJob> _ChildJobsRead = null;
        public Dictionary<NodeId, CommJob> ChildJobsRead
        {
            get { return _ChildJobsRead; }
            set { _ChildJobsRead = value; }
        }

        /// <summary>
        /// List of child use to write tags of Big job
        /// </summary>
        Dictionary<NodeId, CommJob> _ChildJobsWrite = null;
        public Dictionary<NodeId, CommJob> ChildJobsWrite
        {
            get { return _ChildJobsWrite; }
            set { _ChildJobsWrite = value; }
        }

        public void AddTagListToWrite(Tag addTag)
        {
            lock (lockListObject)
            {
                if (!TagsListToWrite.Contains(addTag))
                    TagsListToWrite.Add(addTag);
            }
        }

        public void RemoveFromTagsListToWrite(List<Tag> list)
        {
            lock (lockListObject)
            {
                foreach (var t in list)
                {
                    if (TagsListToWrite.Count == 0)
                        break;
                    TagsListToWrite.Remove(t);
                }
            }
        }
        /// <summary>   The tags list on writing. </summary>
        private readonly List<Tag> _TagsListOnWriting = new List<Tag>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the tags list on writing. </summary>
        ///
        /// <value> The tags list on writing. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Tag> TagsListOnWriting
        {
            get
            {
                    return _TagsListOnWriting;
            }
        }

        public void ClearTagListWrite()
        {
            lock (lockListObject)
            {
                TagsListOnWriting.Clear();
                TagsListToWrite.Clear();
            }
        }
        public void ClearTagListOnWriting()
        {
            lock(lockListObject)
            {
                TagsListOnWriting.Clear();
            }
        }
        public int GetTagListOnWritingCount()
        {
            lock(lockListObject)
            {
                return TagsListOnWriting.Count;
            }
        }
        public List<Tag> GetTagListOnWriting()
        {
            List<Tag> lTags = new List<Tag>();
            lock(lockListObject)
            {
                lTags.AddRange(TagsListOnWriting);
            }
            return lTags;
        }

        public void AddTagListOnWriting(Tag addTag)
        {
            lock (lockListObject)
            {
                if (!TagsListOnWriting.Contains(addTag))
                    TagsListOnWriting.Add(addTag);
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>  Remove a list of jobs from TagsListOnWriting. </summary> ///
        ///////////////////////////////////////////////////////////////////////////
        public void RemoveFromTagsListOnWriting(List<Tag> list)
        {
            lock (lockListObject)
            {
                foreach (var t in list)
                {
                    if (TagsListOnWriting.Count == 0)
                        break;
                    TagsListOnWriting.Remove(t);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Update the list TagsListOnWriting adding the tags of the list TagsListToWrite. </summary> ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void UpdateTagsListOnWriting(CommJob.ScheduleProperties schedulerProperties = CommJob.ScheduleProperties.None)
        {
            lock (lockListObject)
            {
                if (schedulerProperties != ScheduleProperties.None)
                    SetScheduleProperties(schedulerProperties);

                if (Type == LinkType.Input)
                {
                    return;
                }

                if (Type == LinkType.UnconditionalOutput)
                {                    
                    foreach (var tag in TagsList)
                    {
                        if (!TagsListOnWriting.Contains(tag))
                        {
                            // if no value assigned, use default initial value
                            if (tag.WriteVal == null)
                                tag.WriteVal = tag.GetValue();
                            TagsListOnWriting.Add(tag);
                        }
                    }
                    if (TagsListToWrite.Count > 0)
                    {
                        TagsListToWrite.Clear();
                    }
                    return;
                }

                if (TagsListToWrite.Count > 0)
                {
                    foreach (var t in TagsListToWrite)
                    {
                        if (!TagsListOnWriting.Contains(t))
                        {
                            TagsListOnWriting.Add(t);
                        }
                    }
                    TagsListToWrite.Clear();
                }
            }
        }

        public void FillWholeTagsListOnWriting()
        {
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    if (!TagsListOnWriting.Contains(tag))
                    {
                        // if no value assigned, use default initial value
                        if (tag.WriteVal == null)
                            tag.WriteVal = tag.GetValue();
                        TagsListOnWriting.Add(tag);
                    }
                }
            }
        }

        public void UpdateTagsListOnWritingAndTagsListToWrite(List<Tag> newTagListOnWriting)
        {
            lock (lockListObject)
            {
                // Remove from TagsListOnWriting the tags of newTagListOnWriting
                foreach (var t in newTagListOnWriting)
                {
                    if (TagsListOnWriting.Count == 0)
                        break;
                    TagsListOnWriting.Remove(t);
                }

                // Remove from TagsListToWrite the tags of newTagListOnWriting
                foreach (var t in newTagListOnWriting)
                {
                    if (TagsListToWrite.Count == 0)
                        break;
                    TagsListToWrite.Remove(t);
                }

                // Save the current contents of TagsListToWrite
                List<Tag> currentTagsListToWrite = new List<Tag>(TagsListToWrite);

                // Move the tags from TagsListOnWriting to TagsListToWrite
                TagsListToWrite.Clear();
                TagsListToWrite.AddRange(TagsListOnWriting);
                TagsListOnWriting.Clear();

                // Add the previous tags to the end of TagsListToWrite
                currentTagsListToWrite.ForEach((tag) =>
                {
                    if (!TagsListToWrite.Contains(tag))
                    {
                        TagsListToWrite.Add(tag);
                    }
                });

                // Fill TagsListOnWriting with the tags of newTagListOnWriting
                TagsListOnWriting.AddRange(newTagListOnWriting);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Update the list TagsListToWrite adding the tags of the list TagsListOnWriting. </summary> ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void UpdateTagsListToWrite(CommJob.ScheduleProperties schedulerProperties = CommJob.ScheduleProperties.None)
        {
            lock (lockListObject)
            {
                if (schedulerProperties != ScheduleProperties.None)
                    SetScheduleProperties(schedulerProperties);

                if (Type == LinkType.Input)
                {
                    return;
                }

                if (TagsListOnWriting.Count > 0)
                {
                    if (Type == LinkType.UnconditionalOutput)
                    {
                        TagsListToWrite.Clear();
                        TagsListToWrite.AddRange(TagsList);
                        foreach (var tag in TagsListToWrite)
                            tag.WriteVal = tag.GetValue();
                    }
                    else
                    {
                        foreach (var t in TagsListOnWriting)
                        {
                            if (!TagsListToWrite.Contains(t))
                            {
                                TagsListToWrite.Add(t);
                            }
                        }
                    }
                    TagsListOnWriting.Clear();
                }
            }
        }

        /// <summary>   The type. </summary>
        private LinkType _Type;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the type. </summary>
        ///
        /// <value> The type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public LinkType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        /// <summary>   The sampling interval. </summary>
        protected uint _SamplingInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the sampling interval. </summary>
        ///
        /// <value> The sampling interval. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint SamplingInterval
        {
            get { return _SamplingInterval; }
            set { _SamplingInterval = value; }
        }

        /// <summary>   true to swap bytes. </summary>
        protected bool _SwapBytes;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the swap bytes. </summary>
        ///
        /// <value> true if swap bytes, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SwapBytes
        {
            get { return _SwapBytes; }
            
        }

        /// <summary>   true to swap words. </summary>
        private readonly bool _SwapWords;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the swap words. </summary>
        ///
        /// <value> true if swap words, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SwapWords
        {
            get { return _SwapWords; }
        }

        /// <summary>   true to output at startup. </summary>
        private readonly bool _OutputAtStartup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the output at startup. </summary>
        ///
        /// <value> true if output at startup, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OutputAtStartup
        {
            get { return _OutputAtStartup; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether this object is read. </summary>
        ///
        /// <value> true if this object is read, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsRead
        {get; set;}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the exchanged tag. </summary>
        ///
        /// <value> The exchanged tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ExchangedTag
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the exchanged byte. </summary>
        ///
        /// <value> The exchanged byte. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ExchangedByte
        { get; set; }

        /// <summary>   Number of element to exchange. </summary>
        private int _ElementNumber;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the number of element to exchange. </summary>
        ///
        /// <value> Number of element to exchange. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ElementNumber
        { 
            get { return _ElementNumber; } 
            set { _ElementNumber = value; } 
        }

        /// <summary>  Name of the Conditional variable of the job. </summary>
        private string _ConditionalVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Conditional variable of the job. </summary>
        ///
        /// <value> The name of the Conditional variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ConditionalVariableName
        {
            get { return _ConditionalVariableName; }
            set
            {
                _ConditionalVariableName = value;
            }
        }

        /// <summary>  Name of the Conditional variable of the job. </summary>
        private string _ConditionalVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Node ID of the Conditional variable of the job. </summary>
        ///
        /// <value> The Node ID (as a string) of the Conditional variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ConditionalVariableId
        {
            get { return _ConditionalVariableId; }
            set
            {
                _ConditionalVariableId = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>True if a conditional Variable has been assigned to the job. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ConditionalVariableSet
        {
            get { return jobConditionalVariable.hasBeenSet; }
        }

        /// <summary>  Name of the Offset variable of the job. </summary>
        private string _OffsetVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Offset variable of the job. </summary>
        ///
        /// <value> The name of the Offset variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string OffsetVariableName
        {
            get { return _OffsetVariableName; }
            set
            {
                _OffsetVariableName = value;
            }
        }

        /// <summary>  Name of the Offset variable of the job. </summary>
        private string _OffsetVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Node ID of the Offset variable of the job. </summary>
        ///
        /// <value> The Node ID (as a string) of the Offset variable of the job. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string OffsetVariableId
        {
            get { return _OffsetVariableId; }
            set
            {
                _OffsetVariableId = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>True if a Offset Variable has been assigned to the job. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OffsetVariableSet
        {
            get { return jobOffsetVariable.hasBeenSet; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>Polling time in case of error. May vary in case of subsequent fail of the execution </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ErrorPollingTime { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>Return the tag (Struct data type) used the generate internal job's tags</summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Tag StructTag { get; set; } = null;


        /// <summary>
        /// Count the nr of consecutive write operation
        /// </summary>
        private uint _NrConsecutiveWrite;
        public uint NrConsecutiveWrite
        {
            get { return _NrConsecutiveWrite; }
            set { _NrConsecutiveWrite = value; }
        }
        #endregion

        #region Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the list of node ids to be observed for this job. </summary>
        ///
        /// <returns>   Gets the list of node ids to be observed for this job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<NodeId> GetObservingNodes()
        {
            List<NodeId> nodeIDs = new List<NodeId>();
            if (jobConditionalVariable.hasBeenSet == true)
            {
                nodeIDs.AddRange(jobConditionalVariable.GetNodesId());
            }
            if (jobOffsetVariable.hasBeenSet == true)
            {
                nodeIDs.Add(jobOffsetVariable.GetNodeId());
            }

            return nodeIDs;
        }

        /// <summary>
        /// Set (only) the value of conditional variable
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        public void SetConditionalVariableValue(NodeId node, DataValue value)
        {
            if (jobConditionalVariable.hasBeenSet)
            {
                jobConditionalVariable.SetValueDataType(node, value.WrappedValue.TypeInfo.BuiltInType);
                jobConditionalVariable.SetValue(node, value);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of the conditional variable associated to the job. </summary>
        /// 
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ManageUpdatedValueForTheConditionalVariable(NodeId node, DataValue value)
        {

            if(jobConditionalVariable.hasBeenSet)
            {
                SetConditionalVariableValue(node, value);
                
                if (Station == null)
                    return;
                var jobChannel = Station.GetChannel();
                if (jobChannel != null)
                    jobChannel.ForceExecution();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of the offset variable associated to the job. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ManageUpdatedValueForTheOffsetVariable(NodeId node, DataValue value)
        {
            jobOffsetVariable.SetValue(node, value);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get offset variable value </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetOffsetVariableIntNumericValue(out Int64 offsetValue)
        {
            offsetValue = 0;
            if (!OffsetVariableSet)
                return (false);

            object value;
            //Check up if Conditional variable was inizialized
            var varVal = jobOffsetVariable.GetValue();
            if (varVal  == null || varVal.Value == null)
                return (false);

            value = varVal.Value;
            
            if (value is Array)
                return (false);

            BuiltInType builtInType = Station.GetBuiltInType(value.GetType());
            switch (builtInType)
            {
                case BuiltInType.SByte:
                    offsetValue = (sbyte)value;
                    break;

                case BuiltInType.Byte:
                    offsetValue = (byte)value;
                    break;

                case BuiltInType.Int16:
                    offsetValue = (Int16)value;
                    break;

                case BuiltInType.UInt16:
                    offsetValue = (UInt16)value;
                    break;

                case BuiltInType.Int32:
                    offsetValue = (Int32)value;
                    break;

                case BuiltInType.UInt32:
                    offsetValue = (UInt32)value;
                    break;

                case BuiltInType.Int64:
                    offsetValue = (Int64)value;
                    break;

                case BuiltInType.UInt64:
                    offsetValue = (Int64)value;
                    break;

                default:
                    return (false);
            }

            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get offset array variable values </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetOffsetVariableArrayIntNumericValue(out Int64[] offsetArrayValues)
        {
            offsetArrayValues = new Int64[0];
            if (!OffsetVariableSet)
                return (false);

            object value;
            //Check up if Conditional variable was inizialized
            var varVal = jobOffsetVariable.GetValue();
            if ((varVal == null) || (varVal.Value == null))
                return (false);

            value = varVal.Value;
            
            if (!(value is Array))
                return (false);

            BuiltInType builtInType = Station.GetBuiltInArrayType(value.GetType());
            switch (builtInType)
            {
                case BuiltInType.Byte:
                    {
                        byte[] arr = (byte[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                case BuiltInType.SByte:
                    {
                        sbyte[] arr = (sbyte[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                case BuiltInType.Int16:
                    {
                        Int16[] arr = (Int16[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                case BuiltInType.Int32:
                    {
                        Int32[] arr = (Int32[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                case BuiltInType.UInt32:
                    {
                        UInt32[] arr = (UInt32[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                case BuiltInType.Int64:
                    {
                        offsetArrayValues = (Int64[])value;
                    }
                    break;
                case BuiltInType.UInt64:
                    {
                        UInt64[] arr = (UInt64[])value;
                        offsetArrayValues = Array.ConvertAll(arr, (v => (Int64)v));
                    }
                    break;
                default:
                    return (false);
                    break;
            }

            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get offset variable value </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetOffsetVariableStringValue(out string offsetValue)
        {
            offsetValue = string.Empty;
            if (!OffsetVariableSet)
                return (false);

            object value;
            //Check up if Conditional variable was inizialized
            var varVal = jobOffsetVariable.GetValue();
            if ((varVal == null) || (varVal.Value == null))
                    return (false);

            value = varVal.Value;
            
            if (value is Array)
                return (false);

            if (value.GetType() == typeof(string))
            {
                if (value != null)
                    offsetValue = value.ToString();
            }
            else
            {
                return (false);
            }

            return (true);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Checks if the value of the conditional variable is different from 0 (or false). </summary>
        ///
        /// <returns>   true if the value of the conditional variable is different from 0 or if the conditional variable has not been defined. </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool IsConditionalVariableOn()
        {
            bool returnValue = true;

            if (jobConditionalVariable.hasBeenSet == true)
            {
                uint uintValue = 0;
                //Check up if Conditional variable was inizialized
                if (jobConditionalVariable.GetStateCommandVariableValue(ref uintValue) == false)
                {
                    return (returnValue);
                }

                if (uintValue == 0)
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Resets the value of the conditional variable. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ResetConditionalVariable()
        {
            if (jobConditionalVariable.hasBeenSet)
            {
                foreach (NodeId nodeId in jobConditionalVariable.GetNodesId())
                {
                    //uint uintValue = 0;
                    var newVal = new DataValue(GetVariantForConditionalVariable(jobConditionalVariable.GetNodeVarDataType(nodeId)));
                    jobConditionalVariable.SetValue(nodeId, newVal);

                    Station.GetCommDriver().OnTagChanged(nodeId, newVal);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Return the datatype variant corretct 
        /// 
        /// </summary>
        /// <param name="typ"></param>
        /// <returns Variant="value" ></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Variant GetVariantForConditionalVariable(BuiltInType typ)
        {
            Variant value;
            switch (typ)
            {
                case BuiltInType.String:
                    {
                        string a = string.Empty;
                        value = new Variant(a);
                    }
                    break;
                default:
                    {
                        value = new Variant(Opc.Ua.TypeInfo.GetDefaultValue(typ));
                    }
                    break;

            }
            return (value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Return the datatype variant corretct 
        /// 
        /// </summary>
        /// <param name="typ"></param>
        /// <returns Variant="value" ></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Variant GetVarianForConditionalVariable(BuiltInType typ )
        {
            Variant value;
            switch (typ)
            {
                case BuiltInType.String:
                    {
                        string a = string.Empty;
                        value = new Variant(a);
                    }
                    break;
                default:
                    {
                        value = new Variant(Opc.Ua.TypeInfo.GetDefaultValue(typ));
                    }
                    break;

            }
            return(value);
        }


        /// <summary>
        /// set RW state of the job, with error code
        /// </summary>
        /// <param name="err">error code upon execution</param>
        /// <returns>true if job has to be executed promptly</returns>
        public bool SetRWState(DriverErrorCodes err)
        {
            lock (lockListObject)
            {
                if (RWState == CommJob.RWStates.ReadForRW &&
                        err == DriverErrorCodes.ErrorNoError)
                {
                    RWState = CommJob.RWStates.WriteForRW;
                    return true;
                }
                else
                    RWState = CommJob.RWStates.Standard;

                return false;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets maximum job size. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract uint GetMaxJobSize();
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets job data. </summary>
        ///
        /// <param name="jobData" type="ref object">    [in,out] Information describing the job. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract void GetJobData(ref object jobData);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets job data. </summary>
        ///
        /// <param name="jobData" type="object">        Information describing the job. </param>
        /// <param name="changed" type="ref List<Tag>"> [in,out] The changed. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            if (SwapBytes)
                SwapByteBuffer(ref rec);

            if (SwapWords)
                SwapWordBuffer(ref rec);
            
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Swap byte buffer. </summary>
        ///
        /// <param name="buf" type="ref byte []">   [in,out] The buffer. </param>
        /// <param name="init" type="int">          (Optional) the initialise. </param>
        /// <param name="len" type="int">           (Optional) the length. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SwapByteBuffer(ref byte [] buf, int init = 0, int len = 0)
        {
            byte bSaved;
            int max = (len == 0 ? buf.Length : (init + len));
            if (max > buf.Length)
            {
                max = buf.Length;
            }
            for (int i = init; i < (max - 1); i += 2)
            {
                bSaved = buf[i];
                buf[i] = buf[i + 1];
                buf[i + 1] = bSaved;
            }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Swap word buffer. </summary>
        ///
        /// <param name="buf" type="ref byte[]">    [in,out] The buffer. </param>
        /// <param name="init" type="int">          (Optional) the initialise. </param>
        /// <param name="len" type="int">           (Optional) the length. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SwapWordBuffer(ref byte[] buf, int init = 0, int len = 0)
        {
            byte[] bSaved = { 0, 0 };
            int max = (len == 0 ? buf.Length : (init + len));
            if (max > buf.Length)
            {
                max = buf.Length;
            }

            max = (max - init) - ((max - init) % 4);
            if (max < 4)
                return;

            for (int i = init; i < max; i += 4)
            {
                Buffer.BlockCopy(buf, i + 2, bSaved, 0, 2);

                Buffer.BlockCopy(buf, i, buf, i + 2, 2);

                Buffer.BlockCopy(bSaved, 0, buf, i, 2);
            }
        }

       
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the tag vaue. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="DataValue">   The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal uint UpdateTagVaue(NodeId tagnodeid, DataValue value, bool escapewrite = false)
        {   
            lock (lockListObject)
            {
                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == tagnodeid; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                tag.Value.Value = Utils.Clone(value.Value);
                tag.Value.StatusCode = value.StatusCode;
                tag.Value.ServerTimestamp = value.ServerTimestamp;
                tag.Value.SourceTimestamp = value.SourceTimestamp;

                if (!escapewrite)
                {
                    if (tag.DynSettings.OutputAtStartup &&
                        Type != LinkType.Input && !TagsListToWrite.Contains(tag))
                    {
                        tag.Value.StatusCode = StatusCodes.BadWaitingForInitialData;
                        TagsListToWrite.Add(tag);
                        ExecuteFirstTime = true;
                    }
                }
                
                return StatusCodes.Good;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// </summary>
        /// <param name="tagnodeid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual uint OnWriteTag(NodeId tagnodeid, ref object value)
        {
            return OnWriteTag(tagnodeid, ref value, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        /// <param name="forceValue" type="bool">  [inbt] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnWriteTag(NodeId tagnodeid, ref object value, bool forceValue)
        {
            lock (lockListObject)
            {
                if (!forceValue)
                {
                    if (DuplicatedTagValueRemoved(tagnodeid, ref value))
                        return StatusCodes.Good;
                }

                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == tagnodeid; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                tag.SetWriteVal(value);
                if (StatusCode.IsBad(tag.Value.StatusCode))
                    tag.Value.StatusCode = StatusCodes.Uncertain;
                if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput) && !TagsListToWrite.Contains(tag))
                {                    
                    TagsListToWrite.Add(tag);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("- {0}.{1} CommJob.OnWriteTag '{4}' tag:{2} value {3}", DateTime.Now.ToString(),
                                                                                                            DateTime.Now.Millisecond.ToString(),
                                                                                                            tagnodeid.ToString(),
                                                                                                            tag.WriteVal.ToString(), Name);
#endif      

                }
                return StatusCodes.Good;
            }
        }
        public virtual void PrepareSynchro()
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets in use. </summary>
        ///
        /// <param name="node" type="NodeId">               The node. </param>
        /// <param name="inUse" type="bool">                true to in use. </param>
        /// <param name="samplinginterval" type="double">   (Optional) the samplinginterval. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool SetInUse(NodeId node, bool inUse, double samplinginterval = -1)
        {
            bool ret = false;
            lock (lockListObject)
            {
                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == node; });
                if (tag == null)
                    return false;
                if (tag.InUse != inUse)
                {
                    tag.InUse = inUse;

                    ret = true;
                }

                List<Tag> listaordinata = (from taguse in TagsList.AsParallel()
                                 where taguse.InUse == true
                                 orderby taguse.SamplingInterval ascending
                                 select taguse).ToList();

                // decrease job's counter when job is in use and all job's tag are not in is use
                if (InUse && listaordinata.Count == 0)
                    Station.GetChannel().InUseJobsDecrement();
                // increase job's counter when job is not use and at least one tag is in use
                else if (!InUse && listaordinata.Count > 0)
                    Station.GetChannel().InUseJobsIncrement();

                InUse = (listaordinata.Count > 0);            

                if (samplinginterval >= 0 && (tag.SamplingInterval != samplinginterval || ret))
                {
    #if DEBUG
                    System.Diagnostics.Trace.TraceInformation(
                        string.Format("Tag({0}) inUse:{3} from SI={1} to SI={2}", 
                        node, tag.SamplingInterval, samplinginterval, inUse));
    #endif
                    tag.SamplingInterval = samplinginterval;

                    if (listaordinata.Count > 0)
                        SamplingInterval = Convert.ToUInt32(listaordinata[0].SamplingInterval);
    #if DEBUG
                System.Diagnostics.Trace.TraceInformation(
                        string.Format("Job({0}) InUse:{2} SI={1} cnt:{3}",
                        TagsList[0].TagNode.DynamicSettings, SamplingInterval, InUse, listaordinata.Count));
    #endif
                }
            }
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets a quality. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetQuality(uint quality)
        {
            if(Station.DisableQualityUpdate == true)
            {
                return;
            }
            List<NodeId> lista = new List<NodeId>();
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    // FOGBUGZ 11435
                    //if (tag.Value.StatusCode.Code != quality || StatusCode.IsNotGood(quality))
                    if (tag.Value.StatusCode.Code != quality)
                    {
                        tag.Value.StatusCode = quality;
                        lista.Add(tag.TagNode.NodeId);
                    }
                }
            }

            // child job don't publish data 
            if (IsChildJob)
                return;

            if (lista.Count > 0)
            {
                if (SyncroExec)
                    NotifyQuality(lista, (StatusCode)quality);
                else
                    Station.GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
                    {
                        NotifyQuality(lista, (StatusCode)quality);
                    });
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the quality of the tags of a Job. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetInternalQuality(uint quality)
        {
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    if (tag.Value.StatusCode.Code != quality)
                    {
                        tag.Value.StatusCode = quality;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets error state. </summary>
        ///
        /// <param name="errorcode" type="int"> The errorcode. </param>
        /// <param name="bIncrement">if True, increment the Error Polling Time</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetErrorState(int errorcode, bool bIncrement = true)
        {
            uint quality;
            string error;
            Station.GetCommDriver().GetDriverErrorInfo(errorcode, out quality, out error);

            SetErrorState(quality, error, bIncrement);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets error state. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        /// <param name="error" type="string">  The error. </param>
        /// <param name="bIncrement">if True, increment the Error Polling Time</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetErrorState(uint quality, string error, bool bIncrement = true)
        {
            if (InErrorState != StatusCode.IsBad(quality))
            {
                
                LastErrorMessage = error;
            }
            InErrorState = StatusCode.IsBad(quality);
            if (InErrorState)
            {
                LastErrorTime = DateTime.UtcNow;
                if (bIncrement && Station != null && Station.GetChannel() != null && 
                    Station.GetChannel().PollingTimeInError > 0 && ErrorPollingTime < MaxErrorPollingTime)
                {
                    ErrorPollingTime += Station.GetChannel().PollingTimeInError;
#if DEBUG
                    System.Diagnostics.Trace.TraceInformation(string.Format("{1} SetErrorState increment {0} err:{4} {2}.{3}", ErrorPollingTime, Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, error));
#endif
                }
            }
            else
            {
#if DEBUG
                //System.Diagnostics.Trace.TraceInformation(string.Format("{3} SetErrorState '{2}' NoError {0}.{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, Name, Thread.CurrentThread.ManagedThreadId));
#endif
                ErrorPollingTime = 0;
            }  
            SetQuality(quality);
#if DEBUG
            //System.Diagnostics.Trace.TraceInformation(string.Format("{6} CommJobEx '{5}' SetErrorState quality:{0} error:{1} InErrorState:{2} {3}.{4}",
            //    quality, error, InErrorState, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, Name, Thread.CurrentThread.ManagedThreadId));
#endif
        }

        public void UpdateErrorTime()
        {
            LastErrorTime = DateTime.UtcNow;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets InErrorState property value only </summary>
        ///
        /// <param name="newErrorState" type="bool">    The new error state. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetInErrorStateInternalValue(bool newErrorState)
        {
            InErrorState = newErrorState;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tag count. </summary>
        ///
        /// <returns>   The tag count. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetTagCount()
        {
            lock (lockListObject)
            {
                return TagsList.Count;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tag node identifier. </summary>
        ///
        /// <param name="Index" type="int"> Zero-based index of the. </param>
        ///
        /// <returns>   The tag node identifier. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public NodeId GetTagNodeId(int Index)
        {
            lock (lockListObject)
            {
                if (Index < TagsList.Count)
                    return TagsList.ElementAt(Index).TagNode.NodeId;
                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the tag described by oldTag. </summary>
        ///
        /// <param name="oldTag" type="Tag">    The old tag. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RemoveTag(Tag oldTag)
        {
            lock (lockListObject)
            { 
                for (int i = 0; i < TagsList.Count; i++)
                {
                    if (TagsList[i].TagNode.NodeId == oldTag.TagNode.NodeId)
                    {
                        TagsList.Remove(TagsList[i]);
                        return true;
                    }
                }
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes specific tag from TagListToWrite </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">    Node ID of tag to be removed. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RemoveTagFromTagListToWrite(NodeId tagnodeid)
        {
            lock (lockListObject)
            {
                var TagToRemove = (from tag in TagsListToWrite.AsParallel()
                                   where tag.TagNode.NodeId == tagnodeid
                                   select tag).FirstOrDefault();

                if (TagToRemove != null)
                {
                    TagsListToWrite.Remove(TagToRemove);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
                
        /// <summary>
        /// Create a child jobs list to read tags of Big Job
        /// </summary>
        /// <param name="membersList"></param>
        public virtual void CreateChildJobsRead(Tag defTag, List<Tag> membersList)
        {
            SetAsACustomJob();
            _ChildJobsRead = new Dictionary<NodeId, CommJob>();
            foreach (var tag in membersList)
            {                        
                CommJob candJob = Station.CreateJob(tag);
                if (candJob.IsValid)
                {                       
                    candJob.ParentJob = this;
                    _ChildJobsRead[candJob.TagsList[0].TagNode.NodeId] = candJob;
                }
            }
        }

        /// <summary>
        /// Create a child jobs list to write tags of Big jobs
        /// </summary>
        /// <param name="membersList"></param>
        public virtual void CreateChildJobsWrite(Tag defTag, List<Tag> membersList)
        {
            SetAsACustomJob();
            _ChildJobsWrite = new Dictionary<NodeId, CommJob>();
            foreach (var tag in membersList)
            {
                CommJob candJob = Station.CreateJob(tag);
                if (candJob.IsValid)
                {                        
                    candJob.ParentJob = this;
                    _ChildJobsWrite[candJob.TagsList[0].TagNode.NodeId] = candJob;
                }
            }            
        }

        /// <summary>
        /// Return the changes tags/value "generated" from child jobs
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual bool GetCustomJobsChangedTags(ref List<object> items)
        {
            foreach (Tag tag in _TagsList)
            {
                if (tag.IsValueChangedByChild)
                {
                    tag.IsValueChangedByChild = false;
                    items.Add(tag);
                }
            }

            return true;
        }

        /// <summary>
        /// Reset the internal state of the child process before running
        /// </summary>
        public virtual void ResetInitialValueChildJob()
        {
            IsPending = true;
            InErrorState = false;
            ChildJobErrorCode = DriverErrorCodes.ErrorNoError;
            foreach (Tag t in TagsList)
                t.SetInitialValueChild(null);
        }

        /// <summary>
        /// test if it can be aggregated candJob
        /// </summary>
        /// <param name="candJob"></param>
        /// <param name="ExtraBytes">additional byte size for the aggregation</param>
        /// <returns></returns>
        /*
         * <= candJob.Settings.TagSettins[0] JobAggregForward
         * _Settings.TagSettings[0]
         * _Settings.TagSettings[1]
         * .
         * .
         * <= candJob.Settings.TagSettins[0] JobAggregFits
         * .
         * _Settings.TagSettings[_Settings.TagSettings.Count-1]
         * <= candJob.Settings.TagSettins[0] JobAggregBackward
         */
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests aggregate job. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="ExtraBytes">   [out] additional byte size for the aggregation. </param>
        ///
        /// <returns>   A JobAggregationType. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;

            if (Station != candJob.Station)
                return JobAggregationType.JobAggregImpossible;
            if (Type != candJob.Type)
                return JobAggregationType.JobAggregImpossible;
            if (SwapBytes != candJob.SwapBytes)
                return JobAggregationType.JobAggregImpossible;
            if (SwapWords != candJob.SwapWords)
                return JobAggregationType.JobAggregImpossible;
            if (SwapBytes && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 16 || GetDataTypeBitSize((uint)candJob.TagsList[0].TagNode.DataType.Identifier) < 16))
                return JobAggregationType.JobAggregImpossible;
            if (SwapWords && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 32 || GetDataTypeBitSize((uint)candJob.TagsList[0].TagNode.DataType.Identifier) < 32))
                return JobAggregationType.JobAggregImpossible;
            if (SamplingInterval != candJob.SamplingInterval)
                return JobAggregationType.JobAggregImpossible;
            if (ProtocolDataSizeBig() ^ candJob.ProtocolDataSizeBig())
                return JobAggregationType.JobAggregImpossible;
            if (ProtocolDataSizeBig())
            {
                if (ElementNumber != candJob.ElementNumber)
                    return JobAggregationType.JobAggregImpossible;
            }
            else
            {
                if ((ElementNumber > 0) ^ (candJob.ElementNumber > 0))
                    return JobAggregationType.JobAggregImpossible;
            }

            if ((uint)TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean && 
                GetProtocolDataBitSize() < 8)
                return JobAggregationType.JobAggregImpossible;
            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ^
                (uint)candJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                return JobAggregationType.JobAggregImpossible;

            if(jobConditionalVariable.hasBeenSet != candJob.jobConditionalVariable.hasBeenSet)
            {
                return JobAggregationType.JobAggregImpossible;
            }
            if(jobConditionalVariable.hasBeenSet == true)
            {
                if((ConditionalVariableName != candJob.ConditionalVariableName) || (ConditionalVariableId != candJob.ConditionalVariableId))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            if (jobOffsetVariable.hasBeenSet != candJob.jobOffsetVariable.hasBeenSet)
            {
                return JobAggregationType.JobAggregImpossible;
            }
            if (jobOffsetVariable.hasBeenSet == true)
            {
                if ((OffsetVariableName != candJob.OffsetVariableName) || (OffsetVariableId != candJob.OffsetVariableId))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            return JobAggregationType.JobAggregFits;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Aggregate job. </summary>
        ///
        /// <param name="candJob">                              . </param>
        /// <param name="AggType" type="JobAggregationType">    Type of the aggregate. </param>
        /// <param name="ExtraBytes">                           additional byte size for the aggregation. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            
            return false;
        }

        public virtual bool ProtocolDataSizeIsValid(out string errorDesc)
        {
            errorDesc = null;
            if (ProtocolDataSizeBig())
            {
                if (ElementNumber > (GetProtocolDataBitSize() / GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) - 1))
                    errorDesc = "ElementNumber";
                if (Type == LinkType.ExceptionOutput || Type == LinkType.UnconditionalOutput)
                    errorDesc = "LinkType";
            }
            if (errorDesc == null)
                return true;
            else
                return false;
        }
        public virtual uint getProtocolDataType()
        {
            return (uint)TagsList[0].TagNode.DataType.Identifier;
        }
        public uint GetProtocolDataByteSize()
        {
            return (GetProtocolDataBitSize() + 7) / 8; 
        }
        public uint GetProtocolDataBitSize()
        {
            return GetDataTypeBitSize(getProtocolDataType());
        }
        public uint GetDataTypeByteSize(uint DataType)
        {
            return (GetDataTypeBitSize(DataType) + 7) / 8;
        }
        public static uint GetDataTypeBitSize(uint DataType)
        {
            switch (DataType)
            {
                case (uint)BuiltInType.Boolean:
                    return 1;
                case (uint)BuiltInType.SByte:
                case (uint)BuiltInType.Byte:
                    return 8;
                case (uint)BuiltInType.Int16:
                case (uint)BuiltInType.UInt16:
                    return 16;
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.UInt32:
                case (uint)BuiltInType.Int32:
                    return 32;
                case (uint)BuiltInType.UInt64:
                case (uint)BuiltInType.Int64:
                case (uint)BuiltInType.Double:
                    return 64;
                default:
                    return 8;
            }
        }

        public bool ProtocolDataSizeEqual()
        {
            return GetProtocolDataBitSize() == GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);
        }
        public bool ProtocolDataSizeBig()
        {
            switch ((uint)TagsList[0].TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.Double:
                case (uint)BuiltInType.String:
                    return false;
            }
            return GetProtocolDataBitSize() > GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);
        }
        public bool ProtocolDataSizeSmall()
        {
            return GetProtocolDataBitSize() < GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);
        }
        public bool isProtocolBool()
        {
            return ( GetProtocolDataBitSize() < 8);
        }

        public void BasicExecute()
        {
            GestRWState();
            StartExecutionTime = DateTime.UtcNow;
            if (ExecuteFirstTime)
                ExecuteFirstTime = false;

            IsRead = ReadRequest();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Calculate job's statistic values. </summary>
        ///
        /// <param name="job">  job to evaluate. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        long nExchByte = -1;
        long nExchTag = -1;
        public virtual void BasicCalculateStatistic()
        {
            ExchangedByte = 0;
            List<Tag> lTags = new List<Tag>();
            if (!IsRead)
            {
                lock (lockListObject)
                {
                    lTags.AddRange(TagsListOnWriting);
                }
                ExchangedTag = (uint)lTags.Count;
                ExchangedByte = (uint)lTags.Sum(tag => tag.Size);
            }
            else
            {
                if (nExchTag == -1 || nExchByte == -1)
                {
                    lock (lockListObject)
                    {
                        lTags.AddRange(TagsList);
                    }
                    nExchTag = lTags.Count;
                    nExchByte = lTags.Sum(tag => tag.Size);
                }
                ExchangedTag = (uint)nExchTag;
                ExchangedByte = (uint)nExchByte;
            }
        }
        
        public virtual bool IsReadRWReady()
        {
            lock (lockListObject)
            {
                return (RWState != RWStates.WriteForRW &&
                Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
                TagsListOnWriting.Count != 0 && ProtocolDataSizeBig());
            }
        }
        public void GestRWState()
        {
            lock (lockListObject)
            {
                if (IsReadRWReady())
                    RWState = RWStates.ReadForRW;
            }
        }

        public virtual bool ReadRequest()
        {
            lock (lockListObject)
            {
                bool bRead = (Type == DriverCodeBaseEx.Enumerators.LinkType.Input ||
                (Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
                (TagsListOnWriting.Count == 0 || RWState == RWStates.ReadForRW)));
                return bRead;
            }
        }

        public void FillWholeTagListToWrite()
        {
            lock (lockListObject)
            {
                if(TagsListToWrite.Count == 0)
                    TagsListToWrite.AddRange(TagsList);
            }
        }
        public void ReFillTagListToWrite()
        {
            lock (lockListObject)
            {
                foreach (var tag in TagsListOnWriting)
                {
                    if (!TagsListToWrite.Contains(tag))
                        TagsListToWrite.Add(tag);
                }
            }
            
        }
        
        public int GetTagListToWriteCount()
        {
            lock (lockListObject)
            {
                return TagsListToWrite.Count;
            }
        }

        public List<Tag> GetTagListToWrite()
        {
            List<Tag> lTags = new List<Tag>();
            lock (lockListObject)
            {
                lTags.AddRange(TagsListToWrite);
            }
            return lTags;
        }

        public void ClearTagListToWrite()
        {
            lock (lockListObject)
            {
                TagsListToWrite.Clear();
            }
        }

        public void SetUncertainQuality()
        {
            List<NodeId> lista = new List<NodeId>();
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    tag.Value.StatusCode = StatusCodes.Uncertain;
                    lista.Add(tag.TagNode.NodeId);
                }
            }
            if (SyncroExec)
                NotifyQuality(lista, StatusCodes.Uncertain);
            else
                Station.GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
                {
                    NotifyQuality(lista, StatusCodes.Uncertain);
                });
        }
        public void SetBadConnectionQuality()
        {
            List<NodeId> lista = new List<NodeId>();
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    tag.Value.StatusCode = StatusCodes.BadNotConnected;
                    lista.Add(tag.TagNode.NodeId);
                }
            }
            if (SyncroExec)
                NotifyQuality(lista, StatusCodes.BadConnectionRejected);
            else
                Station.GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
                {
                    NotifyQuality(lista, StatusCodes.BadConnectionRejected);
                });
        }

        public void SetUncertainLastUsableValueQuality()
        {
            List<NodeId> lista = new List<NodeId>();
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    tag.Value.StatusCode = StatusCodes.UncertainLastUsableValue;
                    lista.Add(tag.TagNode.NodeId);
                }
            }
            if (SyncroExec)
                NotifyQuality(lista, StatusCodes.UncertainLastUsableValue);
            else
                Station.GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
                {
                    NotifyQuality(lista, StatusCodes.UncertainLastUsableValue);
                });
        }

        void NotifyQuality(List<NodeId> list, StatusCode status)
        {
            foreach (var n in list)
            {
#if DEBUG
                System.Diagnostics.Trace.TraceInformation("NotifyQuality n:{0} s:{1}", n, status);
#endif
                Station.GetCommDriver().OnTagChanged(n, new DataValue(status));
            }
        }

        protected virtual void CreateCustomJob(Tag defTag)
        {
            List<Tag> innerList = new List<Tag>();
            List<Tag> lMethods = new List<Tag>();

            if (IsStruct())
                Station.SplitStructMembers(defTag, innerList, lMethods);
                            
            if (Type == LinkType.Input || Type == LinkType.InputOutput)
                CreateChildJobsRead(defTag, innerList);

            if (Type == LinkType.InputOutput || Type == LinkType.ExceptionOutput || Type == LinkType.UnconditionalOutput)
                CreateChildJobsWrite(defTag, innerList);            
        }

        public virtual List<CommJob> GetReadChildJobs()
        {
            List<CommJob> result = new List<CommJob>();

            if (_ChildJobsRead != null)
                result.AddRange(_ChildJobsRead.Values);

            return result;
        }

        public virtual List<CommJob> GetWriteChildJobs()
        {
            List<CommJob> result = new List<CommJob>();

            if (_ChildJobsRead != null)
                result.AddRange(_ChildJobsWrite.Values);

            return result;
        }

        public virtual bool IsJobAggregable()
        {
            return (!IsCustomJob() && !IsStruct());
        }


        /// <summary>
        /// Verifiy is a job inputoutput job should be forced to read
        /// </summary>
        /// <returns></returns>
        public virtual bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return false;
        }

        protected bool InputOutputRequiresReadAfterContinuosWriteBase()
        {
            return (Type == LinkType.InputOutput
                    //&& !SyncroExec --> for jobs created by recipe write operation is managed with UnconditionalOutput
                    && TagsList.Count > 1
                    && !conditionalVariableHasBeenSet
                    && !InErrorState
                    && NrConsecutiveWrite >= Properties.Settings.Default.MaxConsecutivePollingHiPriJobs)
                    && !IsChildJob;
        }
        #endregion

        /// <summary>
        /// Discard the data to be written as it is the same as the current one
        /// </summary>
        /// <param name="tagnodeid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool DuplicatedTagValueRemoved(NodeId tagnodeid, ref object value)
        {
            //check if the tag must be write 
            if ((Station.RewritingOfTheSameValue == false) && (Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
            {
                lock (lockListObject)
                {
                    Tag t = TagsList.Find(tag => tag.TagNode.NodeId == tagnodeid);
                    if (t != null)
                    {
                        // one data to write was put into one of the writing list (TagsListToWrite or TagsListOnWriting), cannot be removed --> data to write will be update into job.OnWriteTag
                        if (!TagsListToWrite.Contains(t) && !TagsListOnWriting.Contains(t))
                        {
                            DataValue objTmp = new DataValue();
                            objTmp.Value = Utils.Clone(value);
                            
                            if (StatusCode.IsGood(t.Value.StatusCode) && (t.GetValue() != null) && (t.GetValue().Equals(objTmp.Value)))
                            {
                                RemoveTagFromTagListToWrite(tagnodeid);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void SetScheduleProperties(ScheduleProperties sets)
        {
            lock (lockListObject)
            {
                if (sets != CommJob.ScheduleProperties.None)
                {
                    if (sets.HasFlag(ScheduleProperties.SetSyncroExec))
                        SyncroExec = true;
                    if (sets.HasFlag(ScheduleProperties.UnSetSyncroExec))
                        SyncroExec = false;
                    if (sets.HasFlag(ScheduleProperties.SetIsPending))
                        IsPending = true;
                    if (sets.HasFlag(ScheduleProperties.UnSetIsPending))
                        IsPending = false;
                    if (sets.HasFlag(ScheduleProperties.UnSetQueued))
                        ScheduleQueue = CommJobState.UnScheduled;
                }
            }
        }

        #region StateCommandVariable
        public bool conditionalVariableHasBeenSet
        {
            get {
                if (jobConditionalVariable == null || jobConditionalVariable.hasBeenSet == false)
                    return false;
                else
                    return jobConditionalVariable.hasBeenSet;
            }

            set
            {
                if (value == false)
                    jobConditionalVariable = new StateCommandVariable();
            }
        }
        #endregion
    }
}
