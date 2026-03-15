using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;

namespace ModbusTCP
{
    public sealed class ModbusTCPDynTagSettings : DynTagSettings
    {
        #region Constructors

        public ModbusTCPDynTagSettings()
            : base()
        {
            FunctionCode = FunctionCodes.MultipleRegisters;
            StartAddress = 0;
            FileNumber = 0;
            SwapDWords = false;
            _StringLength = 32;
            _BroadCast = false;
        }

        #endregion

        #region Static Members

        private static readonly String FunctionCodeParameter = "FC";
        private static readonly String StartAddressParameter = "SA";
        private static readonly String FileNumberParameter = "File";
        private static readonly String SwapDWordsParameter = "SwapDWords";
        private static readonly String StringLengthParameter = "DL";
        private static readonly String ModbusElementNumberParameter = "ElementNumber";
        private static readonly String BroadcastParameter = "BCast";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            FunctionCode = (FunctionCodes)(helper.GetPartByName(FunctionCodeParameter, (UInt16)FunctionCodes.MultipleRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            FileNumber = helper.GetPartByName(FileNumberParameter, (UInt16)0);
            SwapDWords = helper.GetPartByName(SwapDWordsParameter, false);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);
            BroadCast = helper.GetPartByName(BroadcastParameter, false);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(FunctionCodeParameter)))
                return false;

            // optional parameters
            FunctionCode = (FunctionCodes)(helper.GetPartByName(FunctionCodeParameter, (UInt16)FunctionCodes.MultipleRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            FileNumber = helper.GetPartByName(FileNumberParameter, (UInt16)0);
            SwapDWords = helper.GetPartByName(SwapDWordsParameter, false);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);
            BroadCast = helper.GetPartByName(BroadcastParameter, false);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            if (ElementNumber < 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ModbusElementNumberParameter, DynamicStringParser.CharAssign, ElementNumber);
            }
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", FunctionCodeParameter, DynamicStringParser.CharAssign, (int)FunctionCode);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", FileNumberParameter, DynamicStringParser.CharAssign, FileNumber);
            if (SwapDWords)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SwapDWordsParameter, DynamicStringParser.CharAssign, SwapDWords);
            }
            if (VarType == UFUAModel.DataType.String)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, (uint)StringLength);
            }
            if((TagLinkType != (int)LinkType.Input) && BroadCast)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", BroadcastParameter, DynamicStringParser.CharAssign, BroadCast);
            }

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (ModbusProtocol.GetMaxJobSize(FunctionCode, (LinkType)TagLinkType) >= ByteSize);
        }

        public bool ParseAddress(string address/*, string stationname, ref string dynamicaddress*/)
        {
            try
            {
                if (address.Length > 2)
                {
                    FunctionCodes fc = FunctionCodes.MultipleRegisters;
                    switch (address.Substring(0, 2))
                    {
                        case "CS":
                            fc = FunctionCodes.Coils;
                            break;
                        case "IS":
                            fc = FunctionCodes.DiscreteInputs;

                            // FOGUGZ 11686
                            TagLinkType = (int)LinkType.Input;

                            break;
                        case "HR":
                            fc = FunctionCodes.MultipleRegisters;
                            break;
                        case "IR":
                            fc = FunctionCodes.InputRegisters;

                            // FOGUGZ 11686
                            TagLinkType = (int)LinkType.Input;

                            break;
                        case "SC":
                            fc = FunctionCodes.SingleCoil;
                            break;
                        case "FR":
                            fc = FunctionCodes.FileRecord;
                            break;
                        default:
                            return false;
                    }
                    int nidx = address.IndexOf('.');
                    if (nidx == -1)
                        StartAddress = Convert.ToUInt16(address.Substring(2));
                    else
                    {
                        if (fc != FunctionCodes.MultipleRegisters)
                            return false;
                        byte tmpElementNumber = Convert.ToByte(address.Substring(nidx + 1));
                        if (tmpElementNumber > 15)
                            return false;
                        StartAddress = Convert.ToUInt16(address.Substring(2, nidx - 2));
                        ElementNumber = tmpElementNumber;
                        fc = FunctionCodes.MaskWriteRegister;
                    }
                    FunctionCode = fc;
                    
                    /*ModbusTCPDynTagSettings t = new ModbusTCPDynTagSettings();
                    t.StationName = stationname;
                    t.FunctionCode = fc;
                    t.StartAddress = sa;
                    dynamicaddress = t.ToString();*/
                    return true;
                }
            }
            catch (Exception e)
            { }
            //dynamicaddress = "";
            return false;
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            bool bits = (FunctionCode == FunctionCodes.Coils ||
                FunctionCode == FunctionCodes.SingleCoil ||
                FunctionCode == FunctionCodes.DiscreteInputs);

            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        if(bits)
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                        else
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1: 0)) : 1));
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 4) : 4));
                        break;
                    case (uint)BuiltInType.String:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 32 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 16) : 16));
                        break;
                }
            }

            return ToString();
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            switch (FunctionCode)
            {
                case FunctionCodes.Coils: // Coils
                case FunctionCodes.SingleCoil://Single coil
                case FunctionCodes.DiscreteInputs: // Input discretes
                    return UFUAModel.DataType.Boolean;
                case FunctionCodes.MultipleRegisters: // Multiple registers
                case FunctionCodes.InputRegisters: // Input registers
                case FunctionCodes.SingleRegister: // Single register
                case FunctionCodes.FileRecord: // File Record
                case FunctionCodes.MaskWriteRegister:
                    return UFUAModel.DataType.UInt16;
            }
            return 0;
        }
        #endregion

        #region Properties

        //private int/*LinkType*/ _TagLinkType;
        //[Category("General")]
        //[Description("Link Type")]
        //public override int/*LinkType*/ TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        //private UFUAModel.DataType _VarType;
        //[Category("General")]
        //[Description("Variable Type")]
        //public override UFUAModel.DataType VarType
        //{
        //    get
        //    {
        //        return _VarType;
        //    }
        //    set
        //    {
        //        _VarType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}
        //private uint _ArrayDimension;
        //[Category("General")]
        //[Description("Array Dimension")]
        //public override uint ArrayDimension
        //{
        //    get
        //    {
        //        return _ArrayDimension;
        //    }
        //    set
        //    {
        //        _ArrayDimension = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        /// <summary>   Number of element to exchange. </summary>
        //private int _ElementNumber;
        //[Category("General")]
        //[Description("Element Number")]
        //public override int ElementNumber
        //{
        //    get { return _ElementNumber; }
        //    set
        //    {
        //        _ElementNumber = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
        //    }
        //}

        private int _AddressType;
        [Browsable(false)]
        public int AddressType 
        {
            get { return _AddressType; }
            set 
            {
                _AddressType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
            }
        }

        private FunctionCodes _FunctionCode;
        [Category("Device Data")]
        [Description("Data Area")]
        public FunctionCodes FunctionCode
        {
            get { return _FunctionCode; }
            set
            {
                _FunctionCode = value;
                OnPropertyChanged("TagLinkType");
                OnPropertyChanged("ArrayDimension");
                OnPropertyChanged("ElementNumber");
                OnPropertyChanged("VarType");
            }
        }

        private Boolean _BroadCast;
        [Category("Device Data")]
        [Description("Broadcast")]
        public Boolean BroadCast
        {
            get { return _BroadCast; }
            set
            {
                _BroadCast = value;
            }
        }

        private UInt16 _StartAddress;
        [Category("Device Data")]
        [Description("Start Address")]
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        private UInt16 _FileNumber;
        [Category("Device Data")]
        [Description("File Number")]
        public UInt16 FileNumber
        {
            get { return _FileNumber; }
            set
            {
                _FileNumber = value;
            }
        }

        private bool _SwapDWords;
        [Category("Device Data")]
        [Description("Swap DWords")]
        public bool SwapDWords
        {
            get { return _SwapDWords; }
            set
            {
                _SwapDWords = value;
            }
        }

        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2 String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("String Length")]
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        #endregion

        #region IDataErrorInfo Members
        
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "FunctionCode")
            {
                if (FunctionCode != FunctionCodes.Coils && FunctionCode != FunctionCodes.DiscreteInputs &&
                    FunctionCode != FunctionCodes.ExceptionStatus && FunctionCode != FunctionCodes.FileRecord &&
                    FunctionCode != FunctionCodes.InputRegisters && FunctionCode != FunctionCodes.MultipleRegisters &&
                    FunctionCode != FunctionCodes.SingleCoil && FunctionCode != FunctionCodes.SingleRegister &&
                    FunctionCode != FunctionCodes.MaskWriteRegister)
                    return Properties.Resources.InvalidFunctionCode;

                if (InvalidFunctionCodeInutType())
                    return string.Format(Properties.Resources.FunctionCodeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);

                if (InvalidFunctionCodeOutputType())
                    return string.Format(Properties.Resources.FunctionCodeRequireOutput, DriverCodeBase.Properties.Resources.LinkType_Input);

                if (FunctionCode == FunctionCodes.MaskWriteRegister && GetDataTypeBitSize(VarType) != 1)
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;

                if (FunctionCode == FunctionCodes.MaskWriteRegister && ArrayDimension != 0)
                    return UFUAModel.Properties.Resources.ErroorProtocolSetIncompatibleWithArrays;

                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
            }
            else if (propertyName == "TagLinkType")
            {
                if (InvalidFunctionCodeInutType())
                    return string.Format(Properties.Resources.FunctionCodeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                if (InvalidFunctionCodeOutputType())
                    return string.Format(Properties.Resources.FunctionCodeRequireOutput, DriverCodeBase.Properties.Resources.LinkType_Input);
            }
            else if (propertyName == "SwapDWords")
            {
                if (SwapDWords == true)
                {
                    if ((VarType == UFUAModel.DataType.Boolean) ||
                        ((VarType == UFUAModel.DataType.Byte) && (ArrayDimension < 8)) ||
                        ((VarType == UFUAModel.DataType.SByte) && (ArrayDimension < 8)) ||
                        ((VarType == UFUAModel.DataType.Int16) && (ArrayDimension < 4)) ||
                        ((VarType == UFUAModel.DataType.UInt16) && (ArrayDimension < 4)) ||
                        ((VarType == UFUAModel.DataType.Int32) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.UInt32) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.Float) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.Float) && (ArrayDimension < 2)))
                    {
                        return Properties.Resources.SwapDWordsNotAdmitted;
                    }
                }
            }
            else if (propertyName == "StringLength")
            {
                if (InvalidStringSize())
                    return Properties.Resources.StrngSizeOutOfRange;
            }
            else if (propertyName == "StartAddress")
            {
                if (AddressType == (int)AddressTypes.OneBased && StartAddress <= 0)
                    return Properties.Resources.ErrorInvalidStartAddress;
            }

            return null;
        }
        private bool InvalidStringSize()
        {

            return ((VarType == UFUAModel.DataType.String) &&
                    ((StringLength > 240) ||
                    (StringLength < 1)));
        }


        public override string ProtocolDataSizeValidation(UFUAModel.DataType DataType)
        {
            if (ProtocolDataSizeBig(DataType))
            {
                if (ElementNumber > GetProtocolDataBitSize() / GetDataTypeBitSize(DataType) - 1)
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
                if (!isTagByteSizeOk((GetProtocolDataBitSize() * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
                if (FunctionCode != FunctionCodes.MaskWriteRegister &&
                    (TagLinkType == (int)LinkType.ExceptionOutput || TagLinkType == (int)LinkType.UnconditionalOutput))
                    return DriverCodeBase.Properties.Resources.JobTypeInvalid;
            }
            else
            {
                if (!isTagByteSizeOk(((ElementNumber == 0 ? GetDataTypeBitSize(DataType) : GetProtocolDataBitSize()) * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
            }

            return null;

        }

        private bool InvalidFunctionCodeInutType()
        {
            return ((FunctionCode == FunctionCodes.DiscreteInputs || FunctionCode == FunctionCodes.InputRegisters) && 
                TagLinkType != (int)LinkType.Input);
        }
        private bool InvalidFunctionCodeOutputType()
        {
            return FunctionCode == FunctionCodes.MaskWriteRegister && TagLinkType == (int)LinkType.Input;
        }
        #endregion

        #region INotifyPropertyChanged Members
        
        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                    break;
            }
        }
        #endregion
    }
}
