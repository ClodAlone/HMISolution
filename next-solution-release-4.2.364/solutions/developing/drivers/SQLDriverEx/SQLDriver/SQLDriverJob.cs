using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using Utilities;

namespace SQLDriver
{
    public class SQLDriverCommJob : CommJob
    {
        #region Constructors
        public SQLDriverCommJob(Station station, SQLDriverCommJobSettings settings)
            : base(station, settings)
        {
            _TagName = settings.TagName;
            _SQLDriverColumnName = settings.SQLDriverColumnName;
            _SQLDriverColumnValue = settings.SQLDriverColumnValue;
            _lastExecutionTimeSched = DateTime.MinValue;
            CheckJobValid();
        }

        public SQLDriverCommJob(SQLDriverStation station, SQLDriverTag defTag)
            : base(station, defTag)
        {
            _TagName = defTag.SQLDriverDynSettings.TagName;
            _SQLDriverColumnName = station.SQLDriverColumnNameTagName;
            if (station.SQLDriverMultiColumn)
            {
                _SQLDriverColumnValue = defTag.SQLDriverDynSettings.SQLDriverColumn;
            }
            else
            {
                _SQLDriverColumnValue = station.SQLDriverColumnValue;
            }
            _lastExecutionTimeSched = DateTime.MinValue;
            CheckJobValid();
        }

        public SQLDriverCommJob(Station station)
            : base(station)
        {
            _TagName = String.Empty;
            _SQLDriverColumnName = String.Empty;
            _SQLDriverColumnValue = String.Empty;
            _lastExecutionTimeSched = DateTime.MinValue;
            CheckJobValid();
        }

        protected SQLDriverCommJob()
        {
            _TagName = String.Empty;
            _SQLDriverColumnName = String.Empty;
            _SQLDriverColumnValue = String.Empty;
            _lastExecutionTimeSched = DateTime.MinValue;
            CheckJobValid();
        }
        #endregion
        #region data Member



        #endregion

        #region Static methods

        #endregion

        #region override Methods
        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Methods
        private void CheckJobValid()
        {
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
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override uint GetMaxJobSize()
        {
            return 2048;
        }

        public override void GetJobData(ref object jobData)
        {
            List<byte> outData = new List<byte>();
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    return;
                }
                cand = TagsListOnWriting[0];
            }
            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                nData = (UInt16)(cand.Size);
            }
            else
                if (ElementNumber > 0 && !ProtocolDataSizeBig())
            {
                if (cand.TagNode.ArrayDimension == 0)
                    nData = (ushort)(GetProtocolDataByteSize());
                else
                    nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            }
            else
            {
                nData = (UInt16)cand.Size;
            }
            lock (lockListObject)
            {
                if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                {
                    if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                    {
                        if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                        {
                            if (TagsListOnWriting.Contains(cand))
                            {
                                TagsListOnWriting.Remove(cand);
                            }
                            return;
                        }
                    }
                }
                cand.LastValue = cand.Value.Value;
                jobdata = new byte[nData];
                if (cand.Value.Value != null)
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
            }
            uint ArraySize = cand.TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            if (ProtocolDataSizeBig())
            {
                List<byte> correctData = new List<byte>();
                UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                {
                    byte[] tmpdata = new byte[sizeDataType];
                    if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                        Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                    else
                    {
                        if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                            tmpdata[0] = 0;
                        else
                            tmpdata[0] = 1;
                    }
                    cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                    correctData.AddRange(tmpdata);
                }
                jobdata = correctData.ToArray();
            }
            outData.AddRange(jobdata);
            jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            base.SetJobData(rec, ref changed);
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                if (TagsList[0].TagNode.DataType == DataTypes.Boolean)
                {
                    if (isProtocolBool())
                    {
                        if (((SQLDriverTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset))
                            changed.Add(TagsList[0]);
                    }
                    else
                    {
                        uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                        if (ArraySize == 0)
                            ArraySize = 1;
                        byte[] tmpData = new byte[ArraySize];
                        TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            bool valBool = TagsList[0].getBoolValueFromMemRW((int)TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                            tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                        }
                        if (((SQLDriverTag)(TagsList[0])).SetTagValue(ref tmpData, 0, UpdateTimeStamp))
                            changed.Add(TagsList[0]);
                    }
                }
                else
                {
                    if (isProtocolBool() && ((uint)TagsList[0].TagNode.DataType.Identifier == DataTypes.Structure))
                    {
                        if (((SQLDriverTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset))
                            changed.Add(TagsList[0]);
                    }
                    else if (((SQLDriverTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset))
                    {
                        changed.Add(TagsList[0]);
                    }
                    else
                    {
                        if (((SQLDriverTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset))
                            changed.Add(TagsList[0]);
                    }
                }
                FirstTime = false;
            }
        }
        #endregion

        #region Properties

        // <summary>
        // Tag Name
        // </summary>
        private string _SQLDriverColumnName;
        public string SQLDriverColumnName
        {
            get
            {
                return _SQLDriverColumnName;
            }

            set
            {
                _SQLDriverColumnName = value;
            }
        }

        // <summary>
        // Tag Name
        // </summary>
        private string _SQLDriverColumnValue;
        public string SQLDriverColumnValue
        {
            get
            {
                return _SQLDriverColumnValue;
            }

            set
            {
                _SQLDriverColumnValue = value;
            }
        }

        /// <summary>
        /// Tag Name
        /// </summary>
        private DateTime _lastExecutionTimeSched;
        public DateTime lastExecutionTimeSchede
        {
            get
            {
                return _lastExecutionTimeSched;
            }

            set
            {
                _lastExecutionTimeSched = value;
            }
        }

        private bool _UpdateTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select if the driver have to update timestamp at every read. </summary>
        ///
        /// <value> Always update TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTimeStamp
        {
            get
            {
                return _UpdateTimeStamp;
            }
            set
            {
                _UpdateTimeStamp = value;
            }
        }

        private string _TagName;


        public string TagName
        {
            get
            {
                return _TagName;
            }
            set
            {
                _TagName = value;
            }
        }
        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        #endregion
    }
}
