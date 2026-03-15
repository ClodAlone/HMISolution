using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using DriverCodeBase.Properties;
using DriverBaseInterfaces;
using Opc.Ua;

namespace SaiaDataMode
{
    public sealed class SaiaDataModeDynTagSettings : DynTagSettings
    {
        #region Constructors

        public SaiaDataModeDynTagSettings()
            : base()
        {
            _StartAddress = 0;
            _DbNumber = 0;
            _AreaType = AreaTypes.Inputs;
            _DataConversionType = DataConversionTypes.None;
        }

        #endregion
        
        #region Static Members

        private static readonly String AreaTypeParameter = "DA";
        private static readonly String DbNumberParameter = "DB";
        private static readonly String StartAddressParameter = "SA";
        private static readonly String DataConversionTypeParameter = "Conv";
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.Inputs));
            _StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            _DbNumber = helper.GetPartByName(DbNumberParameter, (UInt16)0);
            _DataConversionType = (DataConversionTypes)(helper.GetPartByName(DataConversionTypeParameter, (UInt16)DataConversionTypes.None));
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(StartAddressParameter)))
                return false;

            // optional parameters
            _AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.Inputs));
            _StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            _DbNumber = helper.GetPartByName(DbNumberParameter, (UInt16)0);
            _DataConversionType = (DataConversionTypes)(helper.GetPartByName(DataConversionTypeParameter, (UInt16)DataConversionTypes.None));

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AreaTypeParameter, DynamicStringParser.CharAssign, (int)_AreaType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, _StartAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DbNumberParameter, DynamicStringParser.CharAssign, _DbNumber);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataConversionTypeParameter, DynamicStringParser.CharAssign, (int)_DataConversionType);

            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                ushort ArrayDimension = (ushort)prevtagdefinition.ArrayDimension;
                if (ArrayDimension == 0)
                    ArrayDimension = 1;
                if (_AreaType < AreaTypes.Registers)
                {
                    switch ((uint)prevtagdefinition.DataType.Identifier)
                    {
                        case (uint)BuiltInType.Boolean:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress += ArrayDimension;
                            else
                                _StartAddress = (ushort)(_StartAddress / 8 + ArrayDimension);
                            break;

                        case (uint)BuiltInType.SByte:
                        case (uint)BuiltInType.Byte:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress = (ushort)(_StartAddress * 8 + ArrayDimension);
                            else
                                _StartAddress += ArrayDimension;
                            break;

                        case (uint)BuiltInType.Int16:
                        case (uint)BuiltInType.UInt16:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress = (ushort)(_StartAddress * 8 + 2 * ArrayDimension);
                            else
                                _StartAddress += (ushort)(2 * ArrayDimension);
                            break;
                        case (uint)BuiltInType.Float:
                        case (uint)BuiltInType.UInt32:
                        case (uint)BuiltInType.Int32:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress = (ushort)(_StartAddress * 8 + 4 * ArrayDimension);
                            else
                                _StartAddress += (ushort)(4 * ArrayDimension);
                            break;
                        case (uint)BuiltInType.UInt64:
                        case (uint)BuiltInType.Int64:
                        case (uint)BuiltInType.Double:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress = (ushort)(_StartAddress * 8 + 8 * ArrayDimension);
                            else
                                _StartAddress += (ushort)(8 * ArrayDimension);
                            break;
                        case (uint)BuiltInType.String:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                                _StartAddress = (ushort)(_StartAddress * 8 + ArrayDimension);
                            else
                                _StartAddress += ArrayDimension;
                            break;
                    }
                }
                else
                {
                    switch ((uint)prevtagdefinition.DataType.Identifier)
                    {
                        case (uint)BuiltInType.Boolean:
                        case (uint)BuiltInType.SByte:
                        case (uint)BuiltInType.Byte:
                        case (uint)BuiltInType.Int16:
                        case (uint)BuiltInType.UInt16:
                        case (uint)BuiltInType.Float:
                        case (uint)BuiltInType.UInt32:
                        case (uint)BuiltInType.Int32:
                            _StartAddress += ArrayDimension;
                            break;
                        case (uint)BuiltInType.UInt64:
                        case (uint)BuiltInType.Int64:
                        case (uint)BuiltInType.Double:
                            _StartAddress += (ushort)(2 * ArrayDimension);
                            break;
                        case (uint)BuiltInType.String:
                            _StartAddress += (ushort)( (ArrayDimension + 3) / 4);
                            break;
                    }
                }
            }
            return ToString();
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            if (_AreaType < AreaTypes.Registers)
                return UFUAModel.DataType.Boolean;
            else
                return UFUAModel.DataType.UInt32;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (SaiaDataModeProtocol.GetMaxJobSize(AreaType) >= ByteSize);
        }

        public bool ParseAddress(string address/*, string stationname, ref string dynamicaddress*/)
        {
            try
            {
                UInt16 sa = Convert.ToUInt16(address);
                StartAddress = sa;
                return true;
            }
            catch (Exception e)
            { }
            return false;
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

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

        private AreaTypes _AreaType;
        [Category("Device Data")]
        [Description("Area Type")]
        public AreaTypes AreaType
        {
            get { return _AreaType; }
            set
            {
                _AreaType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

        private UInt16 _DbNumber;
        [Category("Device Data")]
        [Description("Db Number")]
        public UInt16 DbNumber
        {
            get { return _DbNumber; }
            set
            {
                _DbNumber = value;
            }
        }

        private DataConversionTypes _DataConversionType;
        [Category("Device Data")]
        [Description("Data Conversion Type")]
        public DataConversionTypes DataConversionType
        {
            get { return _DataConversionType; }
            set
            {
                _DataConversionType = value;
            }
        }

        #endregion
        #region IDataErrorInfo Members
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TagLinkType")
            {
                if (InvalidAreaTypeLinkType())
                    return DriverCodeBase.Properties.Resources.JobTypeInvalid;
                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
            }
            if (propertyName == "AreaType")
            {
              
                if (InvalidAreaTypeLinkType())
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                if (VarType == UFUAModel.DataType.String)
                {
                    return string.Format(Properties.Resources.ErrorInvalidTagType, AreaType, VarType);
                }
                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);

            }

            if (propertyName == "StartAddress")
            {
                //if (StartAddress > 100)
                //{
                //    return Properties.Resources.StartAddressOutOfRange;
                //}
            }

            if (propertyName == "DataConversionType")
            {
            }


            return null;
        }
        private bool InvalidAreaTypeLinkType()
        {
                LinkType lt = (LinkType)TagLinkType;
                return ((lt == LinkType.ExceptionOutput || lt == LinkType.InputOutput || lt == LinkType.UnconditionalOutput) &&
                    _AreaType == AreaTypes.Inputs);
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion
    }
}
