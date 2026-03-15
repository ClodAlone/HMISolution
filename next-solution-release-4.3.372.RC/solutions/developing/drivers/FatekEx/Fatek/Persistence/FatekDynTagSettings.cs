using System;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;

namespace Fatek
{
    public sealed class FatekDynTagSettings : DynTagSettings
    {
        #region Static Members

        private static readonly String FatekElementNumberParameter = "ElementNumber";

        #endregion

        #region Constructors

        public FatekDynTagSettings()
            : base()
        {            
            _StartAddress = string.Empty;
            _Area = FatekProtocol.DataArea.Invalid;
            _AreaAddress = 0;
            //SwapDWords = false;
            //_StringLength = 32;
        }

        #endregion

        #region Static Members

        private static readonly String StartAddressParameter = "SA";
        //private static readonly String StringLengthParameter = "DL";
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            StartAddress = helper.GetPartByName(StartAddressParameter);
            //StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);            
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(StartAddressParameter)))
                return false;

            StartAddress = helper.GetPartByName(StartAddressParameter);
            FatekProtocol.SplitStartAddress(StartAddress, out _Area, out _AreaAddress, out string errorCode);

            // optional parameters
            //StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);            

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            if (ElementNumber < 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", FatekElementNumberParameter, DynamicStringParser.CharAssign, ElementNumber);
            }
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            //dynamicstring.Append(DynamicStringParser.CharSep);
            //dynamicstring.AppendFormat("{0}{1}{2}", FileNumberParameter, DynamicStringParser.CharAssign, FileNumber);
            
            //if (VarType == UFUAModel.DataType.String)
            //{
            //    dynamicstring.Append(DynamicStringParser.CharSep);
            //    dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, (uint)StringLength);
            //}
            
            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint byteSize)
        {
            return (FatekProtocol.GetMaxJobSize(_Area) >= byteSize);
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            bool bitArea = FatekProtocol.IsBitDataArea(_Area);
            
            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                // not supported data movicon or invalid Area (used also to manage Invalid Data Type)
                if (!FatekProtocol.IsTypeAdmitted(_Area, thistagdefinition.DataType) || _Area == FatekProtocol.DataArea.Invalid)
                {                    
                    _Area = FatekProtocol.DataArea.Invalid;
                    _AreaAddress = 0;
                    return FatekProtocol.STRUCT_MEMBER_INVALID; // return not emtpy (and invalid) DynamiSetting so next cycle return in GetNextDynSetting
                }

                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        if (bitArea)
                            _AreaAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                        else
                            _AreaAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                        _StartAddress = FatekProtocol.GetStartAddressFormatted(_Area, _AreaAddress);
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (bitArea)
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                        else
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1 : 0)) : 1));
                        _StartAddress = FatekProtocol.GetStartAddressFormatted(_Area, _AreaAddress);
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (bitArea)
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                        else
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        _StartAddress = FatekProtocol.GetStartAddressFormatted(_Area, _AreaAddress);
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (bitArea)
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else
                            _AreaAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));

                        _StartAddress = FatekProtocol.GetStartAddressFormatted(_Area, _AreaAddress);
                        break;                    
                }
            }

            return ToString();
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            switch (_Area)
            {
                case FatekProtocol.DataArea.X:
                case FatekProtocol.DataArea.Y:
                case FatekProtocol.DataArea.M:
                case FatekProtocol.DataArea.S:
                case FatekProtocol.DataArea.T:
                case FatekProtocol.DataArea.C:
                    return UFUAModel.DataType.Boolean;
                case FatekProtocol.DataArea.TMR:
                case FatekProtocol.DataArea.CTR:
                case FatekProtocol.DataArea.HR:
                case FatekProtocol.DataArea.DR:
                case FatekProtocol.DataArea.FR:
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

        private string _StartAddress;
        [Category("Device Data")]
        [Description("Start Address")]
        public string StartAddress
        {
            get { return _StartAddress; }
            set { _StartAddress = value; }
        }
        
        private FatekProtocol.DataArea _Area;
        public FatekProtocol.DataArea Area
        {
            get { return _Area; }            
        }

        private ushort _AreaAddress;
        public ushort AreaAddress
        {
            get { return _AreaAddress; }            
        }

        ///// <summary>   The String Length. </summary>
        //private uint _StringLength;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary> String Length Property. </summary>
        /////
        ///// <value> The String Length. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("Device Data")]
        //[Description("String Length")]
        //public uint StringLength
        //{
        //    get { return _StringLength; }
        //    set { _StringLength = value; }
        //}



        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) {
                case "StartAddress":
                    if (string.IsNullOrWhiteSpace(_StartAddress))
                        return Properties.Resources.ErrorStartAddressEmpty;

                    if (!FatekProtocol.SplitStartAddress(_StartAddress, out _Area, out ushort address, out string errorCode))
                        return string.Format(errorCode);

                    if (!FatekProtocol.IsTypeAdmitted(_Area, VarType))
                        return Properties.Resources.ErrorTagDataTypeNoSupported;

                    if ((uint)VarType != unchecked((uint)(-1)))
                        return ProtocolDataSizeValidation(VarType);
                    break;
            }

            return null;
        }

        //private bool InvalidStringSize()
        //{
        //    return ((VarType == UFUAModel.DataType.String) &&
        //            ((StringLength > 240) ||
        //            (StringLength < 1)));
        //}

        public override string ProtocolDataSizeValidation(UFUAModel.DataType DataType)
        {
            if (ProtocolDataSizeBig(DataType))
            {
                if (ElementNumber > GetProtocolDataBitSize() / GetDataTypeBitSize(DataType) - 1)
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
                if (!isTagByteSizeOk((GetProtocolDataBitSize() * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
            }
            else
            {
                if (!isTagByteSizeOk(((ElementNumber == 0 ? GetDataTypeBitSize(DataType) : GetProtocolDataBitSize()) * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
                if ((DataType == UFUAModel.DataType.Float) && (ElementNumber > 2))
                    return UFUAModel.Properties.Resources.TagOverSize;
            }

            return null;

        }
        #endregion

        //#region INotifyPropertyChanged Members
        
        //protected override void OnPropertyChanged(string propertyName)
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        //    switch (propertyName)
        //    {
        //        //case "TagLinkType":
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        //    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //        //    OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        //    break;
        //        case "ArrayDimension":
        //            OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));                    
        //            break;                
        //    }
        //}
        //#endregion
    }
}
