using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using DevExpress.Data.ODataLinq;
using DevExpress.Office;

namespace ROCDriver
{
    public class ROCDriverCommJob : CommJob
    {
        #region Constructors

        public ROCDriverCommJob(Station station, ROCDriverCommJobSettings settings)
            : base(station, settings)
        {
            _PointType = settings.PointType;
            _LogicalNumber = settings.LogicalNumber;
            _Parameter = settings.Parameter;
            _DataType = settings.DataType;
            _StringLength = settings.StringLength;

            CheckJobValid();
        }

        public ROCDriverCommJob(Station station, ROCDriverTag defTag)
            : base(station, defTag)
        {
            _PointType = defTag.ROCDynSettings.PointType;
            _LogicalNumber = defTag.ROCDynSettings.LogicalNumber;
            _Parameter = defTag.ROCDynSettings.Parameter;
            _DataType = defTag.ROCDynSettings.DataType;
            _StringLength = defTag.ROCDynSettings.StringLength;

            if (TotalJobSize == 0)
                TotalJobSize = (uint)((defTag.ROCDynSettings.StringLength + 1) / 2) * 2;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public ROCDriverCommJob(Station station)
            : base(station)
        {
            _PointType = 0;
            _LogicalNumber = 0;
            _Parameter = 0;
            _DataType = DataTypes.BIN;

            CheckJobValid();
        }

        protected ROCDriverCommJob()
        {
            _PointType = 0;
            _LogicalNumber = 0;
            _Parameter = 0;
            _DataType = DataTypes.BIN;

            CheckJobValid();
        }
        #endregion

        #region data Member

        #endregion

        #region Static methods
        public static bool IsTypeAdmitted(NodeId type)
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
                nType == (uint)BuiltInType.UInteger ||
                nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }
            return false;
        }

        #endregion
        
        #region Methods


        //public bool IsOnlyInput()
        //{
        //    return (AddressObj.MemoryArea == MemoryAreas.X);
        //}

        public bool IsReadCommand()
        {
            lock (lockListObject)
            {
                return (Type == DriverCodeBaseEx.Enumerators.LinkType.Input ||
                (Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
                TagsListOnWriting.Count == 0));
            }
        }

        public string GetCommandCode()
        {
            //if (!IsReadCommand() && (AddressObj.DataFormat == DataFormats.BOOL))
            //{
            //    switch (AddressObj.MemoryArea)
            //    {
            //        case MemoryAreas.X:
            //            return "";
            //        case MemoryAreas.Y:
            //        case MemoryAreas.R:
            //        case MemoryAreas.L:
            //            return String.Format("CP");
            //    }
            //}

            //switch (AddressObj.MemoryArea)
            //{
            //    case MemoryAreas.X:
            //        if(!IsReadCommand())
            //            return "";
            //        return "CCX";
            //    case MemoryAreas.Y:
            //        return "CCY";
            //    case MemoryAreas.R:
            //        return "CCR";
            //    case MemoryAreas.L:
            //        return "CCL";
            //    case MemoryAreas.DT:
            //        return "DD";
            //    case MemoryAreas.FL:
            //        return "DF";
            //    case MemoryAreas.LD:
            //        return "DL";
            //    case MemoryAreas.SV:
            //        return "S";
            //    case MemoryAreas.EV:
            //        return "K";
            //}

            return "";
        }


        public static uint GetNodeSize(NodeId DataType)
        {
            if (DataType.IdType == IdType.Numeric)
            {
                switch ((uint)(DataType.Identifier))
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
                }
            }
            return 0;
        }

 
        public static int CompareTagByOffset(Tag x, Tag y)
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
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                    {
                        if (x.BitOffset > y.BitOffset)
                            return 1;
                        else if (x.BitOffset == y.BitOffset)
                            return 0;
                        else
                            return -1;// x < y
                    }
                    else
                        return -1;// x < y
                }
            }
        }

        private BuiltInType GetJobTagBuiltInType()
        {
            BuiltInType builtInType = BuiltInType.Null;
            if((TagsList[0].Value != null) && (TagsList[0].Value.Value != null))
            {
                Station.GetBuiltInType(TagsList[0].Value.Value.GetType());
            }
            if(builtInType == BuiltInType.Null)
            {
                builtInType = ROCDriverProtocol.GetProtocolBuiltInType(DataType);
            }
            return builtInType;
        }

        public bool GetWriteData(ref byte[] dataBuffer)
        {
            try
            {
                object objectData = null;
                byte[] jobDataBuffer = null;
                lock (lockListObject)
                {
                    GetJobData(ref objectData);
                    // discard inputoutput/exception output unchanged write value operation
                    if (TagsListOnWriting.Count == 0 || objectData == null)
                    {
                        return false;
                    }

                    jobDataBuffer = (byte[])objectData;
                }

                dataBuffer = new byte[GetWriteDataLength()];
                switch (DataType)
                {
                    case DataTypes.BIN:
                    case DataTypes.INT8:
                    case DataTypes.UINT8:
                        dataBuffer[0] = jobDataBuffer[0];
                        break;
                    case DataTypes.AC:
                        {
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            if (builtInType != BuiltInType.String)
                            {
                                return (false);
                            }
                            uint numOfBytesToBeCopied = (uint)jobDataBuffer.Count();
                            if (numOfBytesToBeCopied > StringLength)
                            {
                                numOfBytesToBeCopied = StringLength;
                            }
                            for (uint i = 0; i < numOfBytesToBeCopied; i++)
                            {
                                dataBuffer[i] = jobDataBuffer[i];
                            }
                            for (uint i = 0; i < StringLength; i++)
                            {
                                if (dataBuffer[i] == 0)
                                {
                                    dataBuffer[i] = 0x20; // Fill the string with spaces
                                }
                            }
                        }
                        break;
                    case DataTypes.INT16:
                        {
                            Int16 value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (Int16)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (Int16)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = (Int16)BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (Int16)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (Int16)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        float fValue = BitConverter.ToSingle(jobDataBuffer, 0);
                                        value = (Int16)BitConverter.DoubleToInt64Bits((double)fValue);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        double dValue = BitConverter.ToDouble(jobDataBuffer, 0);
                                        value = (Int16)BitConverter.DoubleToInt64Bits(dValue);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!Int16.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.UINT16:
                        {
                            UInt16 value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (UInt16)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (UInt16)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = (UInt16)BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (UInt16)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (UInt16)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        float fValue = BitConverter.ToSingle(jobDataBuffer, 0);
                                        value = (UInt16)BitConverter.DoubleToInt64Bits((double)fValue);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        double dValue = BitConverter.ToDouble(jobDataBuffer, 0);
                                        value = (UInt16)BitConverter.DoubleToInt64Bits(dValue);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!UInt16.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.INT32:
                        {
                            Int32 value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (Int32)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (Int32)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = (Int32)BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (Int32)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (Int32)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        float fValue = BitConverter.ToSingle(jobDataBuffer, 0);
                                        value = (Int32)BitConverter.DoubleToInt64Bits((double)fValue);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        double dValue = BitConverter.ToDouble(jobDataBuffer, 0);
                                        value = (Int32)BitConverter.DoubleToInt64Bits(dValue);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!Int32.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.UINT32:
                        {
                            UInt32 value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (UInt32)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (UInt32)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (UInt32)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (UInt32)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (UInt32)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        float fValue = BitConverter.ToSingle(jobDataBuffer, 0);
                                        value = (UInt32)BitConverter.DoubleToInt64Bits((double)fValue);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        double dValue = BitConverter.ToDouble(jobDataBuffer, 0);
                                        value = (UInt32)BitConverter.DoubleToInt64Bits(dValue);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!UInt32.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.FLOAT:
                        {
                            float value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (float)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (float)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (float)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = (float)BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (float)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (float)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        value = BitConverter.ToSingle(jobDataBuffer, 0);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        value = (float)BitConverter.ToDouble(jobDataBuffer, 0);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!float.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.DBL:
                        {
                            double value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (double)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (double)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (double)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = (double)BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (double)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (double)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        value = (double)BitConverter.ToSingle(jobDataBuffer, 0);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        value = BitConverter.ToDouble(jobDataBuffer, 0);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!double.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            dataBuffer = BitConverter.GetBytes(value);
                        }
                        break;
                    case DataTypes.TLP:
                        {
                            UInt32 value = 0;
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            switch (builtInType)
                            {
                                case BuiltInType.Boolean:
                                case BuiltInType.SByte:
                                case BuiltInType.Byte:
                                    value = jobDataBuffer[0];
                                    break;

                                case BuiltInType.Int16:
                                    value = (UInt32)BitConverter.ToInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt16:
                                    value = (UInt32)BitConverter.ToUInt16(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int32:
                                    value = (UInt32)BitConverter.ToInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt32:
                                    value = BitConverter.ToUInt32(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Int64:
                                    value = (UInt32)BitConverter.ToInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.UInt64:
                                    value = (UInt32)BitConverter.ToUInt64(jobDataBuffer, 0);
                                    break;

                                case BuiltInType.Float:
                                    {
                                        float fValue = BitConverter.ToSingle(jobDataBuffer, 0);
                                        value = (UInt32)BitConverter.DoubleToInt64Bits((double)fValue);
                                    }
                                    break;

                                case BuiltInType.Double:
                                    {
                                        double dValue = BitConverter.ToDouble(jobDataBuffer, 0);
                                        value = (UInt32)BitConverter.DoubleToInt64Bits(dValue);
                                    }
                                    break;

                                case BuiltInType.String:
                                    {
                                        string stringBuffer = String.Empty;
                                        for (int i = 0; i < jobDataBuffer.Length; i++)
                                        {
                                            stringBuffer.Append((char)jobDataBuffer[i]);
                                        }
                                        if (!UInt32.TryParse(stringBuffer, out value))
                                        {
                                            return (false);
                                        }
                                    }
                                    break;
                            }
                            uintUnion tlpValueUnion = new uintUnion(value);
                            dataBuffer[0] = tlpValueUnion.LOUSHORT.LOBYTE;
                            dataBuffer[1] = tlpValueUnion.LOUSHORT.HIBYTE;
                            dataBuffer[2] = tlpValueUnion.HIUSHORT.LOBYTE;
                        }
                        break;
                    case DataTypes.TIME:
                        {
                            BuiltInType builtInType = GetJobTagBuiltInType();
                            if (builtInType != BuiltInType.String)
                            {
                                return (false);
                            }
                            string stringBuffer = String.Empty;
                            for (int i = 0; i < jobDataBuffer.Length; i++)
                            {
                                stringBuffer.Append((char)jobDataBuffer[i]);
                            }

                            // The string format should be: "YYYY-MM-DDTHH:MM:SS"
                            string[] timestampElements = stringBuffer.Split(new char[] { '-', 'T', ':' });
                            if (timestampElements.GetLength(0) < 6)
                            {
                                return (false);
                            }

                            // Parse the year
                            int year = 0;
                            if (!int.TryParse(timestampElements[0], out year))
                            {
                                return (false);
                            }

                            // Parse the month
                            int month = 0;
                            if (!int.TryParse(timestampElements[1], out month))
                            {
                                return (false);
                            }

                            // Parse the day
                            int day = 0;
                            if (!int.TryParse(timestampElements[2], out day))
                            {
                                return (false);
                            }

                            // Parse the hours
                            int hour = 0;
                            if (!int.TryParse(timestampElements[3], out hour))
                            {
                                return (false);
                            }

                            // Parse the minutes
                            int minute = 0;
                            if (!int.TryParse(timestampElements[4], out minute))
                            {
                                return (false);
                            }

                            // Parse the seconds
                            int second = 0;
                            if (!int.TryParse(timestampElements[5], out second))
                            {
                                return (false);
                            }

                            DateTime writeTimestamp = new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Utc);
                            TimeSpan fromEpochStart = writeTimestamp - new DateTime(1970, 1, 1);
                            UInt32 numberOfSeconds = (UInt32)fromEpochStart.TotalSeconds;
                            dataBuffer = BitConverter.GetBytes(numberOfSeconds);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                return (false);
            }

            return true;
        }

        public uint GetWriteDataLength()
        {
            if(GetTagListOnWritingCount() < 1)
            {
                // Nothing to write???
                return (0);
            }

            return (ROCDriverProtocol.GetJobDataSize(this));
        }
       
        List<DriverBaseInterfaces.TagDefinition> GetSimpleTagList(DriverBaseInterfaces.TagDefinition t)
        {
            List<DriverBaseInterfaces.TagDefinition> l = new List<DriverBaseInterfaces.TagDefinition>();
            if (t.DataType.IdType == IdType.Guid)
            {
                //prototype?
                List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
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

        private void CheckJobValid()
        {
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList =
                                new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) &&
                    t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                                    Properties.Resources.ErrorInvalidTagType,
                                    t.TagNode.NodeId.ToString(),
                                    t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ?
                            ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            bool bit = false;
            bool num = false;
            uint size = 0;
            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                {
                    bit = true;
                }
                else
                {
                    num = true;
                }
                size += GetNodeSize(t.DataType);

            }

            // Do not mix bit and non bit variables
            if (bit && num)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorValidateJob,
                    tagnamelist,
                    Properties.Resources.ErrorMixBitsAndOthers);
                return;
            }

            ushort NumOfByte = 0;

            NumOfByte = (ushort)TotalJobSize;

            if (NumOfByte > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            
            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                //InvalidReason = string.Format("The {0} is invalid for the Address. (Tags: {1} Address: {2})", errorDesc, tagnamelist, Address);
                return;
            }

            // Check the data type
            if(TagsList.Count > 0)
            {
                ROCDriverTag rocTag = (ROCDriverTag)TagsList[0];
                if (rocTag != null)
                {
                    if ((StringLength > ROCDriverProtocol.GetMaxJobSize()) || (StringLength < 1))
                    {
                        IsValid = false;
                        InvalidReason = Properties.Resources.ErrorInvalidStringSize;
                        return;
                    }
                    if (((DataType == DataTypes.AC) || (DataType == DataTypes.TIME)) && ((uint)rocTag.TagNode.DataType.Identifier != (uint)BuiltInType.String))
                    {
                        IsValid = false;
                        InvalidReason = Properties.Resources.ErrorDataTypeRequiresStringVar;
                        return;
                    }
                }
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }
 
        public bool IsWriteRequest()
        {
            lock (lockListObject)
            {
                return ((TagsListOnWriting.Count > 0 &&
                      ((Type == LinkType.ExceptionOutput) || (Type == LinkType.InputOutput))) ||
                       (Type == LinkType.UnconditionalOutput));
            }
        }

        public override uint getProtocolDataType()
        {
            switch (DataType)
            {
                case DataTypes.BIN:
                case DataTypes.UINT8:
                    return (uint)BuiltInType.Byte;
                case DataTypes.AC:
                    return (uint)BuiltInType.String;
                case DataTypes.INT8:
                    return (uint)BuiltInType.SByte;
                case DataTypes.INT16:
                    return (uint)BuiltInType.Int16;
                case DataTypes.INT32:
                    return (uint)BuiltInType.Int32;
                case DataTypes.UINT16:
                    return (uint)BuiltInType.UInt16;
                case DataTypes.UINT32:
                    return (uint)BuiltInType.UInt32;
                case DataTypes.FLOAT:
                    return (uint)BuiltInType.Float;
                case DataTypes.DBL:
                    return (uint)BuiltInType.Double;
                case DataTypes.TLP:
                    return (uint)BuiltInType.UInt32;
                case DataTypes.TIME:
                    return (uint)BuiltInType.String;
            }
            return 0;
        }

        public override uint GetMaxJobSize()
        {
            return ROCDriverProtocol.GetMaxJobSize();
        }

        #region Aggregation
        // The ROC driver does not aggregates tags (run-time aggregation is implemented)
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            return JobAggregationType.JobAggregImpossible;
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }
        #endregion

        public override void GetJobData(ref object jobData)
        {
            if(GetTagListOnWritingCount() < 1)
            {
                // No data to write
                jobData = null;
                return;
            }

            var listOnWriting = new List<Tag>();
            Tag tagToBeWritten = null;
            uint nData = 0;
            byte[] dataBuffer;
            List<byte> outData = new List<byte>();
            lock (lockListObject)
            {
                listOnWriting.AddRange(TagsListOnWriting);
                tagToBeWritten = listOnWriting[0];
                if ((uint)tagToBeWritten.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    nData = (UInt16)((tagToBeWritten.Size + 7) / 8);
                else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                {
                    if (tagToBeWritten.TagNode.ArrayDimension == 0)
                        nData = (ushort)(GetProtocolDataByteSize());
                    else
                        nData = (ushort)(GetProtocolDataByteSize() * tagToBeWritten.TagNode.ArrayDimension);
                }
                else
                    nData = (UInt16)tagToBeWritten.Size;

                dataBuffer = new byte[nData];
                tagToBeWritten.GetTagBuffer(ref dataBuffer, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));

                uint ArraySize = tagToBeWritten.TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (ProtocolDataSizeBig())
                {
                    List<byte> correctData = new List<byte>();
                    UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)tagToBeWritten.TagNode.DataType.Identifier);
                    UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        byte[] tmpdata = new byte[sizeDataType];
                        if ((uint)tagToBeWritten.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                            Array.Copy(dataBuffer, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                        else
                        {
                            if ((1 << (ArrayIndex % 8) & dataBuffer[ArrayIndex / 8]) == 0)
                                tmpdata[0] = 0;
                            else
                                tmpdata[0] = 1;
                        }
                        tagToBeWritten.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                        correctData.AddRange(tmpdata);
                    }
                    dataBuffer = correctData.ToArray();
                }
                else if (isProtocolBool())
                {
                    if (ElementNumber == 0 || (uint)tagToBeWritten.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)tagToBeWritten.TagNode.DataType.Identifier)];
                        for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
                        {
                            if ((dataBuffer[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
                                tmpData[ArrayIndex] = 1;
                        }
                        dataBuffer = tmpData;
                    }
                }

                if (!isProtocolBool())
                {
                    if (SwapBytes)
                    {
                        SwapByteBuffer(ref dataBuffer);
                    }

                    if (SwapWords)
                    {
                        SwapWordBuffer(ref dataBuffer);
                    }
                }

                outData.AddRange(dataBuffer);

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();

                // no data to write --> return null
                if (outData.Count == 0)
                {
                    jobData = null;
                    return;
                }

                jobData = outData.ToArray();
            }

            //var listToWrite = new List<Tag>();
            //var listOnWriting = new List<Tag>();
            //Tag startTag = null;
            //List<byte> outData = new List<byte>();

            //lock (lockListObject)
            //{
            //    listToWrite.AddRange(TagsListOnWriting);
            //    startTag = listToWrite[0];

            //    //prepare a write request
            //    listToWrite.Sort(CompareTagByOffset);

            //    Tag cand = null;
            //    byte[] jobdata;
            //    UInt16 nData = 0;
            //    bool bLookForFirstTag = true;
            //    do
            //    {
            //        if (cand != null)
            //        {
            //            if (!isProtocolBool())
            //            {
            //                if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
            //                    break;
            //            }
            //        }
            //        cand = listToWrite[0];
            //        listToWrite.Remove(cand);
            //        if (bLookForFirstTag && cand != startTag)
            //        {
            //            cand = null;
            //            continue;
            //        }
            //        bLookForFirstTag = false;

            //        if (!listOnWriting.Contains(cand))
            //            listOnWriting.Add(cand);
            //        if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            //            nData = (UInt16)((cand.Size + 7) / 8);
            //        else if (ElementNumber > 0 && !ProtocolDataSizeBig())
            //        {
            //            if (cand.TagNode.ArrayDimension == 0)
            //                nData = (ushort)(GetProtocolDataByteSize());
            //            else
            //                nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            //        }
            //        else
            //            nData = (UInt16)cand.Size;

            //        cand.LastValue = cand.Value.Value;
            //        jobdata = new byte[nData];
            //        cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));

            //        uint ArraySize = cand.TagNode.ArrayDimension;
            //        if (ArraySize == 0)
            //            ArraySize = 1;
            //        if (ProtocolDataSizeBig())
            //        {
            //            List<byte> correctData = new List<byte>();
            //            UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
            //            UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
            //            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
            //            {
            //                byte[] tmpdata = new byte[sizeDataType];
            //                if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
            //                    Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
            //                else
            //                {
            //                    if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
            //                        tmpdata[0] = 0;
            //                    else
            //                        tmpdata[0] = 1;
            //                }
            //                cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
            //                correctData.AddRange(tmpdata);
            //            }
            //            jobdata = correctData.ToArray();
            //        }
            //        else if (isProtocolBool())
            //        {
            //            if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            //            {
            //                byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
            //                for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
            //                {
            //                    if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
            //                        tmpData[ArrayIndex] = 1;
            //                }
            //                jobdata = tmpData;
            //            }
            //        }

            //        if (!isProtocolBool())
            //        {
            //            if (SwapBytes)
            //            {
            //                SwapByteBuffer(ref jobdata);
            //            }

            //            if (SwapWords)
            //            {
            //                SwapWordBuffer(ref jobdata);
            //            }
            //        }

            //        outData.AddRange(jobdata);

            //    } while (listToWrite.Count > 0);

            //    UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
            //    listOnWriting.Clear();
            //    listToWrite.Clear();

            //    // no data to write --> return null
            //    if (outData.Count == 0)
            //    {
            //        jobData = null;
            //        return;
            //    }
            //}

            //jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            if (!ProtocolDataSizeBig())
                            {
                                uint elemsize = 0;
                                if (ElementNumber > 0)
                                {
                                    elemsize = GetProtocolDataByteSize();
                                }
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    changed.Add(TagsList[TagIndex]);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }
        #endregion

        #region Properties

        private Byte _PointType;
        public Byte PointType
        {
            get { return _PointType; }
            set{ _PointType = value; }
        }

        private Byte _LogicalNumber;
        public Byte LogicalNumber
        {
            get { return _LogicalNumber; }
            set { _LogicalNumber = value; }
        }

        private Byte _Parameter;
        public Byte Parameter
        {
            get { return _Parameter; }
            set { _Parameter = value; }
        }

        private DataTypes _DataType;
        public DataTypes DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
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
