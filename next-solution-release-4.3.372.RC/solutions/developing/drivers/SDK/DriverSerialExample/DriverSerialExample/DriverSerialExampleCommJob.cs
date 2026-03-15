////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverSerialExampleCommJob.cs
//
// summary:	Implements the driver serial example communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace DriverSerialExample
{
    /// <summary>   Protocol's task of the DriverSerialExample driver. </summary>
    public class DriverSerialExampleCommJob : CommJob
    {
        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DriverSerialExampleCommJob. </summary>
        ///
        /// <param name="station">  Assigned to an object of type Station. </param>
        /// <param name="settings"> Set with an object of type ModbusCommJobSettings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleCommJob(Station station, DriverSerialExampleCommJobSettings settings)
            : base(station, settings)
        {
            _FunctionCode = settings.FunctionCode;
            _StartAddress = settings.StartAddress;

            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DriverSerialExampleCommJob. </summary>
        ///
        /// <param name="station">  Assigned to an object of type Station. </param>
        /// <param name="defTag">   Set with an object of type ModbusTag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleCommJob(Station station, DriverSerialExampleTag defTag)
            : base(station, defTag)
        {
            _FunctionCode = defTag.DriverSerialExampleDynSettings.FunctionCode;
            _StartAddress = defTag.DriverSerialExampleDynSettings.StartAddress;

            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DriverSerialExampleCommJob. </summary>
        ///
        /// <param name="station">  Assigned to an object of type Station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleCommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        /// <summary>   Initializes the DriverSerialExampleCommJob. </summary>
        protected DriverSerialExampleCommJob()
        {
            CheckJobValid();
        }
        #endregion

        #region data Member
        #endregion

        #region Static methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if NodeId type is admitted. </summary>
        ///
        /// <param name="type"> . </param>
        ///
        /// <returns>   true if type admitted, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (nType == (uint)BuiltInType.Boolean ||
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return tag object memory size. </summary>
        ///
        /// <param name="t">    . </param>
        ///
        /// <returns>   The tag size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch((uint)t.DataType.Identifier)
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

        /// <summary>   Test if DriverSerialExampleCommJob object is valid. </summary>
        private void CheckJobValid()
        {
                
            bool bit = false;
            bool num = false;
            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
                size += GetTagSize(t);
            }

            if (FunctionCode == FunctionCodes.Coils ) 
            {
                //tag must be all bit or not
                if((bit && num))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tags type are inconsistent for the Function Code. (Tags: {0} F.Code: {1})", tagnamelist, FunctionCode);
                    return;
                }
            }
            else
            {
                //must be words or more, even dimension.
                if(bit || size%2 > 0)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Job size is invalid for the Function Code. (Tags: {0} F.Code: {1})", tagnamelist, FunctionCode);
                    return;
                }
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return maximum memory size for aggregation of DriverSerialExampleCommJob objects.
        /// </summary>
        ///
        /// <returns>   The aggregate maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of DriverSerialExampleCommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetMaxJobSize()
        {
            if (Type == LinkType.Input)
            {
                return 250;
            }
            else
            {
                switch (FunctionCode)
                {
                    case FunctionCodes.Coils:
                        return 100;
                    case FunctionCodes.MultipleRegisters:
                        return 200;
                }
            }

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if it can be aggregated candJob. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="ExtraBytes">   [out] Additional byte size for the aggregation. </param>
        ///
        /// <returns>   A JobAggregationType. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            DriverSerialExampleCommJob testJob = candJob as DriverSerialExampleCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode != testJob.FunctionCode)
                return JobAggregationType.JobAggregImpossible;

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states the presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType))) 
                return JobAggregationType.JobAggregImpossible;


            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == nBool
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool)
                return JobAggregationType.JobAggregImpossible;

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = StartAddress;
            uint startTest = testJob.StartAddress;
            uint elementsize = 2;

            if (FunctionCode == FunctionCodes.Coils )
            {
                if (TagsList[0].TagNode.DataType != nBool)
                {
                    if (System.Math.Abs(start - startTest) % 8 != 0)
                        return JobAggregationType.JobAggregImpossible;

                    start /= 8;
                    startTest /= 8;
                }
                else
                    Granularity *= 8;

                elementsize = 1;
            }

            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / elementsize;
                uint endTag = startTag + (tag.Size / elementsize) - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode != FunctionCodes.Coils )
                if (SwapWords)
                    if (start % 2 != startTest % 2)
                        return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) * elementsize;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset = newoffset;
                    newoffset += candJob.TagsList[i].Size;
                }

                return JobAggregationType.JobAggregFits;
            }

            if (startTest >= start && startTest <= end + Granularity)
            {
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = (startTest - start) * elementsize;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }
            }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
            {
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (start - startTest) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }
            }

            return JobAggregationType.JobAggregImpossible;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Aggregate candJob as specified by AggType and manage ExtraBytes. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="AggType">      . </param>
        /// <param name="ExtraBytes">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new DriverSerialExampleTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new DriverSerialExampleTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                        tag.ByteOffset += ExtraBytes;
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new DriverSerialExampleTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    StartAddress = ((DriverSerialExampleCommJob)candJob).StartAddress;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Get job data. Not used, but required because is defined as "abstract" in the base class.
        /// </summary>
        ///
        /// <param name="jobData">  [in,out]. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void GetJobData(ref object jobData)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Write the DriverSerialExampleCommJob's tags with JobData. </summary>
        ///
        /// <param name="jobData">  Data buffer to write. </param>
        /// <param name="changed">  [in,out] Out list of changed tags. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            if (FunctionCode == FunctionCodes.MultipleRegisters )
                _SwapBytes = true;

            if (FunctionCode == FunctionCodes.Coils)
                _SwapBytes = false;

            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            
            switch (FunctionCode)
            {
                case FunctionCodes.Coils:
                    lock (lockListObject)
                    {
                        if (TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric
                               && TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                        {
                            //bit tags
                            int bytecount = 0;
                            byte mask = 1;
                            object val = new object();
                            for (int i = 0; i < TagsList.Count; i++)
                            {
                                if (TagsList[i].DynSettings.MethodID != -1)
                                    continue;

                                if (TagsList[i].TagNode.ArrayDimension == 0)
                                {
                                    bytecount = (int)TagsList[i].ByteOffset / 8;
                                    mask = 1;
                                    byte sweep = ((byte)(TagsList[i].ByteOffset % 8));
                                    mask <<= (sweep > 0 ? sweep : 0);

                                    val = (((rec[bytecount] & mask) > 0) ? true : false);

                                    if (TagsList[i].GetReadValue() == null || (bool)TagsList[i].GetReadValue() != (bool)val || FirstTime)
                                    {
                                        TagsList[i].SetReadValue(val);
                                        TagsList[i].Value.Value = val;
                                        if (Type == LinkType.InputOutput)
                                        {
                                            TagsList[i].SetInternalValues(val);
                                            TagsList[i].LastValue = val;
                                        }

                                        changed.Add(TagsList[i]);
                                    }
                                }
                                else
                                {
                                    bool[] a = new bool[TagsList[i].TagNode.ArrayDimension];
                                    Array b = TagsList[i].GetReadValue() as Array;

                                    bool bCopy = false;
                                    bool check = (b != null && b.GetLength(0) == TagsList[i].TagNode.ArrayDimension);
                                    for (int j = 0; j < TagsList[i].TagNode.ArrayDimension; j++)
                                    {
                                        bytecount = (int)(TagsList[i].ByteOffset + j) / 8;
                                        mask = 1;
                                        byte sweep = ((byte)((TagsList[i].ByteOffset + j) % 8));
                                        mask <<= (sweep > 0 ? sweep : 0);

                                        a[j] = (((rec[bytecount] & mask) > 0) ? true : false);
                                        if (check && a[j] != (bool)b.GetValue(j))
                                            bCopy = true;

                                    }
                                    val = a;
                                    if (TagsList[i].GetReadValue() == null || bCopy || FirstTime)
                                   
                                    {
                                        TagsList[i].SetReadValue(val);
                                        if (Type == LinkType.InputOutput)
                                        {
                                            TagsList[i].SetInternalValues(val);
                                            TagsList[i].LastValue = val;
                                        }

                                        TagsList[i].Value.Value = val;
                                        changed.Add(TagsList[i]);
                                    }
                                }

                            }

                            FirstTime = false;

                        }
                        else
                        {
                            //copy following tag size
                            int idx = 0;
                            for (int i = 0; i < TagsList.Count; i++)
                            {
                                if (TagsList[i].SetTagValue(ref rec, idx))
                                    changed.Add(TagsList[i]);
                                idx += (int)TagsList[i].Size;
                            }
                        }
                    }
                        break;
                case FunctionCodes.MultipleRegisters:
                        lock (lockListObject)
                    {
                        for (int j = 0; j < TagsList.Count; j++)
                        {
                            if (TagsList[j].SetTagValue(ref rec, (int)TagsList[j].ByteOffset))
                                changed.Add(TagsList[j]);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Properties
        /// <summary>   The executioncode. </summary>
        private byte _Executioncode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Execution code of memory area Property. </summary>
        ///
        /// <value> The executioncode. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte Executioncode
        {
            get { return _Executioncode; }
            set
            {
                _Executioncode = value;
            }
        }
        
        /// <summary>   The function code. </summary>
        private FunctionCodes _FunctionCode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Memory area Property. </summary>
        ///
        /// <value> The function code. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public FunctionCodes FunctionCode
        {
            get { return _FunctionCode; }
            set
            {
                _FunctionCode = value;
            }
        }

        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start address of memory area Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String for grouping the DriverSerialExampleCommJobs Property. </summary>
        ///
        /// <value> The group string. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}FC{1:00}", ret, (uint)FunctionCode);
            }
        }

         #endregion
    }
}
