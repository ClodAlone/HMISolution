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
using DriverCodeBase.Enumerators;
using DriverBaseInterfaces;
using System.Threading;

namespace DriverCodeBase
{
    /// <summary>   protocol's task of the base communication driver. </summary>
    public abstract class CommJob
    {
        public enum RWStates
        {
            Standard,
            ReadForRW,
            WriteForRW,
        }

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
            if (!String.IsNullOrEmpty(_ConditionalVariableName) && !String.IsNullOrEmpty(_ConditionalVariableId))
            {
                jobConditionalVariable = new StateCommandVariable(_ConditionalVariableName, _ConditionalVariableId);
            }

            _OffsetVariableName = settings.OffsetVariableName;
            _OffsetVariableId = settings.OffsetVariableId;
            if (!String.IsNullOrEmpty(_OffsetVariableName) && !String.IsNullOrEmpty(_OffsetVariableId))
            {
                jobOffsetVariable = new StateCommandVariable(_OffsetVariableName, _OffsetVariableId);
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
                    ArrayDimension = tagsettings.ArrayDimension
                }, tagsettings.ByteOffset, tagsettings.BitOffset);
                TagsList.Add(t);

                station.AddToMapTagJob(t.TagNode.NodeId, this);
                station.GetCommDriver().AddToTagToStationMap(t.TagNode.NodeId, station);
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
                //System.Diagnostics.Trace.TraceInformation(string.Format("member: {2} type:{0} Settings:{1}", (tag.DataType != null ? tag.DataType.Identifier : "Method"), tag.DynamicSettings, tag.NodeId.ToString()));
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
                if (!String.IsNullOrEmpty(_ConditionalVariableName) && !String.IsNullOrEmpty(_ConditionalVariableId))
                {
                    jobConditionalVariable = new StateCommandVariable(_ConditionalVariableName, _ConditionalVariableId);
                }

                _OffsetVariableName = defTag.DynSettings.OffsetVariableName;
                _OffsetVariableId = defTag.DynSettings.OffsetVariableId;
                if (!String.IsNullOrEmpty(_OffsetVariableName) && !String.IsNullOrEmpty(_OffsetVariableId))
                {
                    jobOffsetVariable = new StateCommandVariable(_OffsetVariableName, _OffsetVariableId);
                }
            }
            else
            {
                IsValid = false;                                
            }
            if (defTag.TagNode.DataType.IdType == IdType.Guid)
            {
                AddPrototypeMethodToTagList(station, defTag.TagNode.NodeId);
                var ProtoList = (from t in TagsList/*.AsParallel()*/ where t.TagNode.DataType.IdType == IdType.Guid select t).ToList();
                foreach (var t in ProtoList)
                {
                    TagsList.Remove(t);
                }
            }
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
            jobOffsetVariable = new StateCommandVariable();
        }
        

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        protected CommJob()
        {
            FirstTime = true;
            IsValid = true;
            InvalidReason = string.Empty;
            ExecuteFirstTime = false;
            IsPending = false;

            _ConditionalVariableName = String.Empty;
            _ConditionalVariableId = String.Empty;
            jobConditionalVariable = new StateCommandVariable();

            _OffsetVariableName = String.Empty;
            _OffsetVariableId = String.Empty;
            jobOffsetVariable = new StateCommandVariable();
        }

        #endregion

        #region Data Members


        public RWStates RWState;

        /// <summary>   The station. </summary>
        public readonly Station Station;

        /// <summary>   true to in error state. </summary>
        private bool _InErrorState;
        /// <summary>   The last error time. </summary>
        protected DateTime LastErrorTime;
        /// <summary>   Message describing the last error. </summary>
        protected string LastErrorMessage;

        /// <summary>   The lock list object. </summary>
        protected Object lockListObject = new Object();

        /// <summary>  The Conditional variable associated to the job. </summary>
        protected StateCommandVariable jobConditionalVariable;

        /// <summary>  The Offset variable associated to the job. </summary>
        protected StateCommandVariable jobOffsetVariable;
        #endregion

        #region Properties
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
        public bool InErrorState { get; protected set; }
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
        /// <summary>   Gets or sets a value indicating whether the execute first time. </summary>
        ///
        /// <value> true if execute first time, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal bool ExecuteFirstTime { get; set; }//steve 240811
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the last execution time. </summary>
        ///
        /// <value> The last execution time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime LastExecutionTime { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the start execution time. </summary>
        ///
        /// <value> The start execution time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime StartExecutionTime { get; set; }//steve 230811

        /// <summary>   The reset synchro. </summary>
        private ManualResetEvent _ResetSynchro = new ManualResetEvent(false);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the reset synchro. </summary>
        ///
        /// <value> The reset synchro. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ManualResetEvent ResetSynchro
        {
            get { return _ResetSynchro; }
            set
            {
                _ResetSynchro = value;
            }
        }

        /// <summary>   The end synchro execute. </summary>
        private ManualResetEvent _EndSynchroExec = new ManualResetEvent(false);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the end synchro execute. </summary>
        ///
        /// <value> The end synchro execute. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ManualResetEvent EndSynchroExec
        {
            get { return _EndSynchroExec; }
            set
            {
                _EndSynchroExec = value;
            }
        }
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
        public uint SamplingInterval
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
                nodeIDs.Add(jobConditionalVariable.varNodeId);
            }
            if (jobOffsetVariable.hasBeenSet == true)
            {
                nodeIDs.Add(jobOffsetVariable.varNodeId);
            }

            return nodeIDs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of the conditional variable associated to the job. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ManageUpdatedValueForTheConditionalVariable(DataValue value)
        {
            if(jobConditionalVariable.hasBeenSet)
            {
                jobConditionalVariable.varValue = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the value of the offset variable associated to the job. </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ManageUpdatedValueForTheOffsetVariable(DataValue value)
        {
            if (jobOffsetVariable.hasBeenSet)
            {
                jobOffsetVariable.varValue = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get offset variable value </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetOffsetVariableIntNumericValue(out Int64 offsetValue)
        {
            offsetValue = 0;
            if (!OffsetVariableSet)
                return (false);

            //Check up if Conditional variable was inizialized
            if ((jobOffsetVariable.varValue == null) || (jobOffsetVariable.varValue.Value == null))
                return (false);

            object value = jobOffsetVariable.varValue.Value;
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

            //Check up if Conditional variable was inizialized
            if ((jobOffsetVariable.varValue == null) || (jobOffsetVariable.varValue.Value == null))
                return (false);

            object value = jobOffsetVariable.varValue.Value;
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

            //Check up if Conditional variable was inizialized
            if ((jobOffsetVariable.varValue == null) || (jobOffsetVariable.varValue.Value == null))
                return (false);

            object value = jobOffsetVariable.varValue.Value;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Converts an object value to an unsigned integer (32 bit). </summary>
        ///
        /// <returns>   The converted unsigned integer value (32 bit). </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected uint ConvertValueToUint(object value)
        {
            Type systemType = value.GetType();
            BuiltInType builtInType = Station.GetBuiltInType(systemType);
            uint uintValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        bool boolValue = (bool)value;
                        if (boolValue == true)
                        {
                            uintValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        sbyte sbyteValue = (sbyte)value;
                        uintValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        byte byteValue = (byte)value;
                        uintValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        short shortValue = (short)value;
                        uintValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        ushort ushortValue = (ushort)value;
                        uintValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        int intValue = (int)value;
                        uintValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.Float:
                    {
                        float FloatValue = (float)value;
                        uintValue = (uint)FloatValue;
                    }
                    break;

                case BuiltInType.Int64:
                    {
                        Int64 Int64Value = (Int64)value;
                        uintValue = (uint)Int64Value;
                    }
                    break;

                case BuiltInType.UInt64:
                    {
                        UInt64 UInt64Value = (UInt64)value;
                        uintValue = (uint)UInt64Value;
                    }
                    break;

                case BuiltInType.Double:
                    {
                        Double DoubleValue = (Double)value;
                        uintValue = (uint)DoubleValue;
                    }
                    break;

                case BuiltInType.String:
                    {
                        string st = value.ToString();
                        if (!string.IsNullOrWhiteSpace(st))
                        {
                            if (!uint.TryParse(st, out uintValue))
                            {
                                return (uintValue);
                            }
                        }
                    }
                    break;

                case BuiltInType.UInt32:
                    uintValue = (uint)value;
                    break;
            }

            return (uintValue);
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
                //Check up if Conditional variable was inizialized
                if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
                {
                    return (returnValue);
                }
                object value = jobConditionalVariable.varValue.Value;
                if ((value != null) && !(value is Array))
                {
                    uint uintValue = ConvertValueToUint(value);
                    if(uintValue == 0)
                    {
                        returnValue = false;
                    }
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
            if(jobConditionalVariable.hasBeenSet)
            {
                uint uintValue = 0;
                jobConditionalVariable.varValue = new DataValue(new Variant(uintValue));
                Station.GetCommDriver().OnTagChanged(jobConditionalVariable.varNodeId, jobConditionalVariable.varValue);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Ret lock list. </summary>
        ///
        /// <returns>   An object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public object retLockList()
        {
            return lockListObject;
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
            if(max > buf.Length)
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

        //private static byte[] _temp;
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
                tag.SetInternalValues(Utils.Clone(value.Value));

                if (escapewrite)
                {
                    tag.SetReadValue(Utils.Clone(value.Value));
                }
                else
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
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnWriteTag(NodeId tagnodeid, ref object value)
        {
            lock (lockListObject)
            {
                
                Tag tag = TagsList.Find(o => { return o.TagNode.NodeId == tagnodeid; });
                if (tag == null)
                    return StatusCodes.BadNodeIdInvalid;

                tag.Value.Value = Utils.Clone(value);
                if (StatusCode.IsBad(tag.Value.StatusCode))
                    tag.Value.StatusCode = StatusCodes.Uncertain;
                if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput) && !TagsListToWrite.Contains(tag))
                {                    
                    TagsListToWrite.Add(tag);
                }
               
                System.Diagnostics.Debug.WriteLine("- {0}.{1} CommJob.OnWriteTag tag:{2} value {3}", DateTime.Now.ToString(),
                                                                                                        DateTime.Now.Millisecond.ToString(), 
                                                                                                        tagnodeid.ToString(), 
                                                                                                        tag.Value.Value.ToString());
               
                return StatusCodes.Good;
            }
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
        public bool SetInUse(NodeId node, bool inUse, double samplinginterval = -1)
        {
            bool ret = false;
            Tag tag = null;
            List<Tag> listaordinata = new List<Tag>();
            lock (lockListObject)
            {
                tag = TagsList.Find(o => { return o.TagNode.NodeId == node; });
                if (tag == null)
                    return false;
                if (tag.InUse != inUse)
                {
                    tag.InUse = inUse;

                    //InUse = ((from taguse in TagsList/*.AsParallel()*/
                    //          where taguse.InUse == true
                    //          select taguse).ToList().Count > 0);
                    ret = true;
                }

                listaordinata = (from taguse in TagsList
                                 where taguse.InUse == true
                                 orderby taguse.SamplingInterval ascending
                                 select taguse).ToList();

                InUse = (listaordinata.Count > 0);
            }

            if (samplinginterval >= 0 && (tag.SamplingInterval != samplinginterval || ret))
            {
                System.Diagnostics.Trace.TraceInformation(
                    string.Format("Tag({0}) from SI={1} to SI={2}", 
                    node, tag.SamplingInterval, samplinginterval));
                tag.SamplingInterval = samplinginterval;

                //List<Tag> listaordinata = new List<Tag>();
                //lock (lockListObject)
                //{
                //    listaordinata = (from taguse in TagsList
                //                            where taguse.InUse == true
                //                        orderby taguse.SamplingInterval ascending
                //                        select taguse).ToList();
                //}
                if (listaordinata.Count > 0)
                    SamplingInterval = Convert.ToUInt32(listaordinata[0].SamplingInterval);
#if DEBUG
                System.Diagnostics.Trace.TraceInformation(
                    string.Format("Job({0})  SI={1}",
                    TagsList[0].TagNode.DynamicSettings, SamplingInterval));
#endif
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
            List<NodeId> lista = new List<NodeId>();
            lock (lockListObject)
            {
                foreach (var tag in TagsList)
                {
                    // FOGBUGZ 11435
                    //if (tag.Value.StatusCode.Code != quality || StatusCode.IsNotGood(quality))
                    if (tag.Value.StatusCode.Code != quality)
                    {
                        //System.Diagnostics.Trace.TraceInformation(string.Format("Set quality: {0}", quality));
                        tag.Value.StatusCode = quality;
                        lista.Add(tag.TagNode.NodeId);
                    }
                }
            }
            foreach(var n in lista)
                Station.GetCommDriver().OnTagChanged(n, new DataValue(quality));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets the quality of the tags of a Job. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void SetInternalQuality(uint quality)
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetErrorState(int errorcode)
        {
            uint quality;
            string error;
            Station.GetCommDriver().GetDriverErrorInfo(errorcode, out quality, out error);

            SetErrorState(quality, error);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets error state. </summary>
        ///
        /// <param name="quality" type="uint">  The quality. </param>
        /// <param name="error" type="string">  The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetErrorState(uint quality, string error)
        {
            if (InErrorState != StatusCode.IsBad(quality))
            {
                InErrorState = StatusCode.IsBad(quality);
                LastErrorMessage = error;
                LastErrorTime = DateTime.UtcNow;
                //Station.GetCommDriver().OnSystemEvent(ObjectIds.Server, error, EventSeverity.High);
            }

            SetQuality(quality);

            //Station.GetCommDriver().OnSystemEvent(ObjectIds.Server, error, EventSeverity.High);
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

            //List<Tag> tl = (from t in TagsList.AsParallel()
            //                where (t.DynSettings.MethodID == (int)DriverMethods.ReadSynchro
            //                || t.DynSettings.MethodID == (int)DriverMethods.WriteSynchro)
            //                select t).ToList();
            //if (tl.Count > 0)
            //    return JobAggregationType.JobAggregImpossible;

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

        public void GestRWState()
        {
            if (RWState != RWStates.WriteForRW &&
                Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                TagsListToWrite.Count != 0 && ProtocolDataSizeBig())
                RWState = RWStates.ReadForRW;

        }
        
        public bool ReadRequest()
        {
            return (Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                (TagsListToWrite.Count == 0 || RWState == RWStates.ReadForRW)));
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
            foreach (var n in lista)
            {
                Station.GetCommDriver().OnTagChanged(n, new DataValue(StatusCodes.Uncertain));
            }
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
            foreach (var n in lista)
            {
                Station.GetCommDriver().OnTagChanged(n, new DataValue(StatusCodes.BadConnectionRejected));
            }
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
            foreach (var n in lista)
            {
                Station.GetCommDriver().OnTagChanged(n, new DataValue(StatusCodes.UncertainLastUsableValue));
            }
        }

        #endregion

        #region StateCommandVariable
        public bool conditionalVariableHasBeenSet
        {
            get
            {
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
