////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Station.cs
//
// summary:	Implements the driver GESRTP2 station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace GESRTP2
{
    /// <summary>   Communication target device. </summary>
    public class GESRTP2Station : Station
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="GESRTP2StationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Station(CommunicationDriver commdriver, GESRTP2StationSettings settings)
            : base(commdriver, settings)
        {
            _PlcType = settings.PlcType;
            _Rack = settings.Rack;
            _Slot = (Byte)settings.Slot;

            _Dir = string.Empty;
            _LookUpTable = GESRTP2Protocol.LOOKUP_TABLE_NAME;
        }

        #endregion

        #region Abstract Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from CommJobSettings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="JobSettings">  . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as GESRTP2CommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new GESRTP2CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from Tag. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="defTag">   . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as GESRTP2Tag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new GESRTP2CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new variable instance from TagDefinition. </summary>
        ///
        /// <param name="td">   . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new GESRTP2Tag(td);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new settings instance for protocol's task. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as GESRTP2CommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new GESRTP2CommJobSettings(session, commJob);
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by Dynamic Settings. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    GESRTP2DynTagSettings dts = new GESRTP2DynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}AT{3}SA{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.AreaType,
                        dts.StartAddress.ToString("00000"),
                        dts.StringLength.ToString("00000")
                        );
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AT{3}SA{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.AreaType,
                        dts.StartAddress.ToString("00000"),
                        dts.StringLength.ToString("00000")
                        );
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Processed received data in ExecutedJobArgs. To call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            GESRTP2CommJob mJ = e.Job as GESRTP2CommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                List<object> ChangedTags = new List<object>();
                if (e.Job.IsCustomJob(CommJob.CustomJobCheckStates.Read))
                {
                    e.Job.GetCustomJobsChangedTags(ref ChangedTags);
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                }
                else
                {
                    if (e.Values != null)
                    {
                        byte[] Answer = (byte[])e.Values;
                        if (GESRTP2Protocol.ParseData(Answer, ref mJ, ref ChangedTags))
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);
                            }
                        else
                            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivedbuffer">   . </param>
        /// <param name="job">              . </param>
        /// <param name="arguments">        [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            GESRTP2CommJob mj = job as GESRTP2CommJob;

            GESRTP2Protocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        private bool IsTagStructType(Tag tag)
        {
            return (tag != null && tag.TagNode.DataType.IdType == IdType.Guid);
        }

        private void MergeStructMembersToTagsList(List<Tag> tList, int tagIndex)
        {
            List<Tag> innerList = new List<Tag>();
            List<Tag> lMethods = new List<Tag>();

            ParseDynamicTagsStructSplit(tList[tagIndex], innerList, lMethods);            
            
            tList.InsertRange(tagIndex + 1, innerList);
        }

        private bool IsTagAggregableToStructMixed(Tag tag)
        {
            // excluding Struct Atomic / Conditional variable / ExceptionOutput / UnconditionalOutput / Symbolic tag
            return !(IsStructAtomic(tag) 
                     || (!String.IsNullOrEmpty(tag.DynSettings.ConditionalVariableName) && !String.IsNullOrEmpty(tag.DynSettings.ConditionalVariableId))
                     || (((GESRTP2DynTagSettings)tag.DynSettings).TagLinkType == (int)LinkType.ExceptionOutput || ((GESRTP2DynTagSettings)tag.DynSettings).TagLinkType == (int)LinkType.UnconditionalOutput)                     
                    );
        }

        /// <summary>
        /// Calculate the portion (start and nr of elements) of the array used to read the StructMixed : result in alwayes in bytes 
        /// </summary>
        /// <param name="candTag"></param>
        /// <param name="byteStartAddress"></param>
        /// <param name="tagSize"></param>
        public void GetStructMixedTagByteStartAddressAndSize(Tag candTag, out uint byteStartAddress, out uint tagSize)
        {
            GESRTP2DynTagSettings dynSettings = ((GESRTP2DynTagSettings)(candTag).DynSettings);

            byteStartAddress = dynSettings.StartAddress;
                        
            tagSize = candTag.Size;

            if ((dynSettings.ElementNumber > 0 && !dynSettings.isProtocolBool()) || dynSettings.ProtocolDataSizeBig(candTag))
            {
                if (candTag.TagNode.ArrayDimension == 0)
                    tagSize = (uint)dynSettings.GetProtocolDataByteSize();
                else
                    tagSize = dynSettings.GetProtocolDataByteSize() * candTag.TagNode.ArrayDimension;
            }

            // specific case array of bool inside bit area --> the plc returns the results in bytes (compacted)
            switch (GESRTP2Protocol.DataType(((GESRTP2DynTagSettings)candTag.DynSettings).AreaType))
            {
                case BuiltInType.UInt16:
                    byteStartAddress *= 2;
                    break;
                case BuiltInType.Byte:
                    break;
                case BuiltInType.Boolean:
                    if (candTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        tagSize = 0;
                        if (candTag.TagNode.ArrayDimension == 0)
                        {
                            tagSize = 1;
                        }
                        else
                        {
                            if (byteStartAddress / 8 == (byteStartAddress + candTag.TagNode.ArrayDimension) / 8)
                            {
                                tagSize = 1;
                            }
                            else
                            {
                                // calculate the nr of bytes required to read bits (request of array of bit return in array of bytes)
                                int arrayRemain = (int)candTag.TagNode.ArrayDimension;
                                if (tagSize % 8 != 0)
                                {
                                    arrayRemain -= (8 - (int)byteStartAddress % 8);
                                    tagSize++;
                                }

                                if (arrayRemain > 0)
                                {
                                    if ((byteStartAddress + (int)candTag.TagNode.ArrayDimension) % 8 != 0)
                                    {
                                        arrayRemain -= ((int)byteStartAddress + (int)candTag.TagNode.ArrayDimension) % 8;
                                        tagSize++;
                                    }
                                }

                                if (arrayRemain > 0)
                                {
                                    tagSize += (uint)Math.DivRem(arrayRemain, 8, out int rest);
                                    if (rest != 0)
                                        tagSize++;
                                }
                            }
                        }
                    }
                    byteStartAddress /= 8;
                    break;            
            }
        }

        /// <summary>
        /// Calculate the distance (default in bytes, otherwise use area data size) between 2 tags
        /// </summary>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <param name="useAreaDataSize"></param>
        /// <returns></returns>
        private uint GetStructMixedTotalJobSize(Tag startTag, Tag endTag)
        {
            // adddress and size in always on byte
            GetStructMixedTagByteStartAddressAndSize(startTag, out uint startTagAddress, out uint startTagSize);
            GetStructMixedTagByteStartAddressAndSize(endTag, out uint endTagAddress, out uint endTagSize);
            
            return (uint)((endTagAddress - startTagAddress) + endTagSize);
        }

        /// <summary>
        /// Check if a tag can be aggregated with other tag considering the maximum frame size
        /// </summary>
        /// <param name="structMixedTag"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private JobAggregationType TestAggregateTagStructMixed(Tag structMixedTag, Tag tag)
        {           
            if (((GESRTP2DynTagSettings)structMixedTag.DynSettings).TagLinkType != ((GESRTP2DynTagSettings)tag.DynSettings).TagLinkType
                || ((GESRTP2DynTagSettings)structMixedTag.DynSettings).AreaType != ((GESRTP2DynTagSettings)tag.DynSettings).AreaType
                )
                return JobAggregationType.JobAggregImpossible;

            // create job to test if is valid 
            CommJob candJob = CreateJob(tag);            
            if (!candJob.IsValid)
            {
                tag.bIsValid = false;
                tag.InvalidReason = candJob.InvalidReason;

                // if tag is invalid, set JobAggregationType.JobAggregFits so it will be removed from list
                return JobAggregationType.JobAggregFits;
            }

            Tag firstTag = ((GESRTP2Driver)GetCommDriver()).GetInternalPrototypeMember(structMixedTag.TagNode.NodeId, 0);
            if (firstTag == null)
                firstTag = tag;
            
            // size is in bytes
            uint totalAggregationSize = GetStructMixedTotalJobSize(firstTag, tag);
            
            bool addTag = true;
            if (firstTag != tag)
            {                
                // size is in bytes
                GetStructMixedTagByteStartAddressAndSize(tag, out uint endTagAddress, out uint endTagSize);
                int holeAggregationSize = (int)((totalAggregationSize - structMixedTag.Size) - endTagSize);

                // size is in bytes
                addTag &= (holeAggregationSize <= GetCommDriver().AggregationThreshold);

                addTag &= (totalAggregationSize <= GetAggregateMaxJobSize());
            }

            if (addTag)
            {
                if (totalAggregationSize > structMixedTag.Size)
                    structMixedTag.Size = totalAggregationSize;
                ((GESRTP2Driver)GetCommDriver()).AddInternalPrototypeMember(structMixedTag.TagNode.NodeId, tag);
                return JobAggregationType.JobAggregFits;
            }
            else
            {
                // aggreagion impossible
                return JobAggregationType.JobAggregForward;
            }
        }

        private uint structMixedNr = 0;
        /// <summary>
        /// Create a new struct data type tag witch contain aggregated tags
        /// </summary>
        /// <param name="sourceTag"></param>
        /// <returns></returns>
        private Tag CreateStructMixedTag(Tag sourceTag)
        {
            structMixedNr++;

            string newDynamicSettings = GESRTP2DynTagSettings.AddStructMixedParameter(sourceTag.TagNode.DynamicSettings);

            TagDefinition t = new TagDefinition()
            {
                NodeId = new NodeId(Guid.NewGuid()),
                DynamicSettings = newDynamicSettings, 
                DataType = new NodeId(Guid.NewGuid()),
                SamplingInterval = 0,
                ArrayDimension = 0,
                InitialValue = null,
                MemberOrder = 0,
                Name = String.Format("{0}\\StructMixed{1}", this.Name, structMixedNr),
            };

            Tag tag = ((GESRTP2Driver)GetCommDriver()).CreateTag(t);

            tag.bIsValid = true;

            // create an internal prototype to containg all tag that will be aggregate --> emulate a movicon's prototype
            ((GESRTP2Driver)GetCommDriver()).AddInternalPrototype(tag.TagNode.NodeId);

            return tag;
        }

        public override bool ParseDynamicTags(IList<Tag> tags)
        {
            //start aggregation thread/procedure for this station.
            lock (lockListObject)
            {
                if (tags == null || tags.Count == 0)
                    return false;

                if (mapGroupedJobs == null)
                    mapGroupedJobs = new Dictionary<string, List<CommJob>>();

                List<Tag> tList = SortTags(tags);

                tags.Clear();

                while (tList.Count > 0)
                {
                    int tagIndex = 0;
                    Tag tagStructMixed = null;
                    JobAggregationType aggType = JobAggregationType.JobAggregImpossible;
                    while (tagIndex < tList.Count && aggType != JobAggregationType.JobAggregForward)
                    {
                        tList[tagIndex].ByteOffset = 0;
                        tList[tagIndex].BitOffset = 0;

                        if (Properties.Settings.Default.EnableDataAreaAggregations && !GESRTP2Protocol.IsSymbolic(tList[tagIndex]) && IsTagAggregableToStructMixed(tList[tagIndex]))
                        {
                            if (IsTagStructType(tList[tagIndex]))
                            {
                                // insert to tList the members of the struct
                                MergeStructMembersToTagsList(tList, tagIndex);
                                // remove struct's tag type
                                tList.RemoveAt(tagIndex);
                                tagIndex--;
                            }
                            else
                            {
                                if (tagStructMixed == null)
                                    tagStructMixed = CreateStructMixedTag(tList[tagIndex]);

                                aggType = TestAggregateTagStructMixed(tagStructMixed, tList[tagIndex]);
                                switch (aggType)
                                {
                                    case JobAggregationType.JobAggregImpossible:
                                        // tag is in the same area but has different link type  (and so on) : skip it
                                        break;
                                    case JobAggregationType.JobAggregFits:
                                        // tag was aggregated
                                        tList.RemoveAt(tagIndex);
                                        tagIndex--;
                                        break;
                                    case JobAggregationType.JobAggregForward:
                                        // tag cannot be aggregated because exceed the range
                                        break;
                                }
                            }
                        }
                        else
                        {
                            List<Tag> innerList = new List<Tag>();
                            List<Tag> lMethods = new List<Tag>();

                            // if tag is a struct split its members
                            ParseDynamicTagsStructSplit(tList[tagIndex], innerList, lMethods);

                            // try to aggregate result tags with of others jobs to speed-up communication
                            ParseDynamicTagsAggregate(tList[tagIndex], innerList, lMethods, tags);

                            tList.RemoveAt(tagIndex);
                            tagIndex--;
                        }

                        tagIndex++;
                    }

                    if (tagStructMixed != null)
                        MixedStructParseDynamicTagsAggregate(tagStructMixed, new List<Tag>(), new List<Tag>(), tags);
                }
            }
            return true;
        }

        #region Struct Atomic

        public override bool IsStructAtomic(Tag candTag)
        {
            bool isStruct = false;

            if (candTag != null && candTag.TagNode.DataType.IdType == IdType.Guid)
            {                
                if (/*Properties.Settings.Default.AtomicStructEnable || */
                    (!String.IsNullOrEmpty(candTag.DynSettings.ConditionalVariableName) && !String.IsNullOrEmpty(candTag.DynSettings.ConditionalVariableId))
                    || (((GESRTP2DynTagSettings)candTag.DynSettings).StructMixed))
                {
                    isStruct = true;
                }
            }

            return isStruct;
        }

        public override void SplitStructMembers(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {
            if (tag == null || tag.TagNode.DataType.IdType != IdType.Guid)
                return;

            // a mixed struct contains a list of that with own dynamic link (don't need to calculate current address starting from previous one)
            if (((GESRTP2DynTagSettings)tag.DynSettings).StructMixed)
            {
                List<TagDefinition> pList = new List<TagDefinition>();
                GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                for (int i = 0; i < pList.Count; i++)
                {                    
                    //get dynsettings for each tagdefinition.
                    Tag t = GetCommDriver().CreateTag(pList[i]);
                    if (t.DynSettings.MethodID > -1)
                    {
                        //ricordati del metodo...
                        lMethods.Add(t);
                        continue;
                    }
                    else
                    {
                        if (t.bIsValid)
                        {
                            innerList.Add(t);
                        }
                        else
                        {
                            CommDriver.OnTagChanged(t.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError), DisableQualityUpdate);
                            //CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }

                if (innerList.Count > 0)
                {
                    // recalculate the total tag size
                    tag.Size = 0;
                    foreach (Tag innerTag in innerList)
                    {
                        uint totalAggregationSize = GetStructMixedTotalJobSize(innerList[0], innerTag);
                        if (totalAggregationSize > tag.Size)
                            tag.Size = totalAggregationSize;
                    }
                }
            } 
            else
            {
                // standard prototype management
                base.SplitStructMembers(tag, innerList, lMethods);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return maximum memory size for aggregation of GESRTP2CommJob objects.
        /// </summary>
        ///
        /// <returns>   The aggregate maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = GetCommDriver().AggregationLimit;
            uint JobMaxSize = GESRTP2Protocol.GetMaxJobSize(_PlcType);
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        private void MixedStructParseDynamicTagsAggregate(Tag tag, List<Tag> innerList, List<Tag> lMethods, IList<Tag> tags)
        {
            // if struct contain only 1 tag, don't create a StructMixed
            int nrMembers = ((GESRTP2Driver)GetCommDriver()).GetInternalPrototypeNrMembers(tag.TagNode.NodeId);
            if (nrMembers == 0)
                return;

            if (nrMembers == 1)
            {
                tag = (((GESRTP2Driver)GetCommDriver()).GetInternalPrototypeMember(tag.TagNode.NodeId, 0));
                innerList.Add(tag);
            }

            ParseDynamicTagsAggregate(tag, innerList, lMethods, tags);            
        }

        public bool HasDir()
        {
            return !String.IsNullOrEmpty(_Dir);
        }

        public void ResetRunTimeSymbolicParameters()
        {
            List<CommJob> listJobs = new List<CommJob>();
            lock (lockListObject)
                listJobs = ListWholeJob.FindAll(j => GESRTP2Protocol.IsSymbolic(j));

            _Dir = string.Empty;
            _LookUpTable = GESRTP2Protocol.LOOKUP_TABLE_NAME;
            if (listJobs != null && listJobs.Count > 0)
            {                
                foreach (GESRTP2CommJob job in listJobs)
                    job.ResetRunTimeSymbolicParameters();
            }            
        }

        #endregion

        #region Properties

        private GESRTP2Protocol.PlcTypes _PlcType;
        public GESRTP2Protocol.PlcTypes PlcType
        {
            get { return _PlcType; }
            set { _PlcType = value; }
        }

        private Byte _Rack;
        public Byte Rack
        {
            get { return _Rack; }
            set { _Rack = value; }
        }

        private Byte _Slot;
        public Byte Slot
        {
            get { return _Slot; }
            set { _Slot = value; }
        }

        private String _Dir = string.Empty;
        public string Dir
        {
            get { return _Dir; }
            set { _Dir = value; }
        }

        private String _LookUpTable = string.Empty;
        public string LookUpTable
        {
            get { return _LookUpTable; }
            set { _LookUpTable = value; }
        }
        #endregion
    }
}
