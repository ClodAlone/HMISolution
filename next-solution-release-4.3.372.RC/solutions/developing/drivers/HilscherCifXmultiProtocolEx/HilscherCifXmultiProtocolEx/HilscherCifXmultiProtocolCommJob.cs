using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.IO;

namespace HilscherCifXmultiProtocol
{
    public class HilscherCifXmultiProtocolCommJob : CommJob
    {        
        #region Constructors

        public HilscherCifXmultiProtocolCommJob(Station station, HilscherCifXmultiProtocolCommJobSettings settings)
            : base(station, settings)
        {
            _DataAddress = settings.DataAddress;
            _ExecutedInOutputMode = false;
            CheckJobValid();
        }

        public HilscherCifXmultiProtocolCommJob(Station station, HilscherCifXmultiProtocolTag defTag)
            : base(station, defTag)
        {

            _DataAddress = defTag.HilscherCifXmultiProtocolDynSettings.DataAddress;
            _ExecutedInOutputMode = false;
            if (ElementNumber > 0)
                TotalJobSize = GetProtocolDataByteSize();
            CheckJobValid();
        }

        public HilscherCifXmultiProtocolCommJob(Station station)
            : base(station)
        {
            _ExecutedInOutputMode = false;
            CheckJobValid();
        }

        protected HilscherCifXmultiProtocolCommJob()
        {
            _ExecutedInOutputMode = false;
            CheckJobValid();         
        }

        #endregion

        // FOGBUGZ 12127
        #region Data Members
        public bool addressChecked = false;
        #endregion

        #region Static methods

        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (
                nType == (uint)BuiltInType.Byte ||
                nType == (uint)BuiltInType.Double ||
                nType == (uint)BuiltInType.Float ||
                nType == (uint)BuiltInType.Int16 ||
                nType == (uint)BuiltInType.Int32 ||
                nType == (uint)BuiltInType.Int64 ||
                nType == (uint)BuiltInType.Integer ||
                nType == (uint)BuiltInType.SByte ||
                nType == (uint)BuiltInType.UInt16 ||
                nType == (uint)BuiltInType.UInt32 ||
                nType == (uint)BuiltInType.UInt64 ||
                nType == (uint)BuiltInType.UInteger
                )
                    return true;
                return false;
            }

            return false;
        }

        #endregion

        #region Methods

        // FOGBUGZ 12127
        public void SetIsValidState(bool newState)
        {
            IsValid = newState;
        }

        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        return 1;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        return 2;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        return 4;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        return 8;
                    default:
                        return 0;
                }
            }
            return 0;
        }

        private void CheckJobValid()
        {
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            IsValid = true;
            InvalidReason = string.Empty;
        }
 
        #endregion

        #region Abstract Methods
        public override void GetJobData(ref object jobData)
        {
            lock (lockListObject)
            {
                for (int i = 0; i < TagsList.Count; i++)
                {
                    uint dim = TagsList[i].Size;
                }
            }
        }

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public override uint GetMaxJobSize()
        {
            return HilscherCifXmultiProtocolProtocol.MAX_DATA_BYTES;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            HilscherCifXmultiProtocolCommJob testJob = candJob as HilscherCifXmultiProtocolCommJob;
            if (testJob == null)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = DataAddress;
            uint startTest = testJob.DataAddress;

            uint end = start + TotalJobSize - 1;
            uint endTest = startTest + testJob.TotalJobSize - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            if (((startTest >= start) && (startTest <= end)) ||
               ((endTest >= start) && (endTest <= end)))
            {
                foreach (var tag in TagsList)
                {
                    uint startTag = start + tag.ByteOffset;
                    int endTag =(int) (startTag + tag.Size - 1);
                    if ((startTest >= startTag && (int)startTest <= endTag)
                        || (endTest >= startTag && endTest <= endTag))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            else if (startTest < start && endTest > end)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // Check granularity
            else
            {
                if (endTest < start)
                {
                    if ((start - endTest) > (Granularity + 1))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else if ((startTest - end) > (Granularity + 1))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            // Calculate the new offset for the job tags
            uint newoffset = 0;
            if (startTest >= start)
            {
                newoffset = startTest - start;
            }

            if (startTest >= start && endTest <= end)
            {
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset += newoffset;
                }
                return JobAggregationType.JobAggregFits;
            }

            // Calculate and check the new job total size
            uint newStartAddress = start;
            if (newStartAddress > startTest)
            {
                newStartAddress = startTest;
            }
            uint newEndAddress = end;
            if (newEndAddress < endTest)
            {
                newEndAddress = endTest;
            }
            uint newJobSize = newEndAddress - newStartAddress + 1;
            if (newJobSize > GetAggregateMaxJobSize())
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // The job can aggregate the new data. Calculate the extension required
            ExtraBytes = newJobSize - TotalJobSize;

            // Aggregate forward
            if (startTest >= start)
            {
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset += newoffset;
                }

                return JobAggregationType.JobAggregForward;
            }

            // Aggregate backward
            else
            {
                return JobAggregationType.JobAggregBackward;
            }
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new HilscherCifXmultiProtocolTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new HilscherCifXmultiProtocolTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                    {
                        tag.ByteOffset += ExtraBytes;
                    }

                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new HilscherCifXmultiProtocolTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }

                    TotalJobSize += ExtraBytes;
                    DataAddress = ((HilscherCifXmultiProtocolCommJob)candJob).DataAddress;
                    break;
                default:
                    return false;
            }
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            return true;
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
            {
                return;
            }

            lock (lockListObject)
            {
                uint elemsize = 0;
                if (ElementNumber > 0)
                {
                    elemsize = GetProtocolDataByteSize();
                }
                for (int j = 0; j < TagsList.Count; j++)
                {
                    if (TagsList[j].DynSettings.MethodID != -1)
                    {
                        continue;
                    }

                    if (TagsList[j].SetTagValue(ref rec, (int)TagsList[j].ByteOffset,elemsize))
                    {
                        TagsList[j].Value.SourceTimestamp = DateTime.UtcNow;
                        changed.Add(TagsList[j]);
                    }
                }
            }
        }

        #endregion

        #region Properties

        private uint _DataAddress;
        public uint DataAddress
        {
            get { return _DataAddress; }
            set
            {
                _DataAddress = value;
            }
        }

        private bool _ExecutedInOutputMode;
        public bool ExecutedInOutputMode
        {
            get
            {
                return _ExecutedInOutputMode;
            }
            set
            {
                _ExecutedInOutputMode = value;
            }
        }
        #endregion
    }
}
