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
using UFUAModel;

namespace OmronFinsEthernet
{
    public sealed class OmronFinsEthernetDynTagSettings : DynTagSettings
    {
                #region Constructors

        public OmronFinsEthernetDynTagSettings()
            : base()
        {
            _Address = String.Empty;
            _DataConversionType = DataConversionTypes.None;
        }

        #endregion

        #region Static Members

        private static readonly String AddressParameter = "Addr";
        private static readonly String ConvParameter = "Conv";

        #endregion


        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _Address = helper.GetPartByName(AddressParameter);
            _DataConversionType = (DataConversionTypes)(helper.GetPartByName(ConvParameter, (UInt16)DataConversionTypes.None));
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            _Address = helper.GetPartByName(AddressParameter);
            // Check the address
            OmronAddress testAddress = new OmronAddress(_Address);
            if (!testAddress.IsValid)
            {
                return false;
            }
            UInt16 tmpDataConversionType = helper.GetPartByName(ConvParameter, (UInt16)DataConversionTypes.None);
            if (tmpDataConversionType > (UInt16)DataConversionTypes.BCD32Bits)
            {
                return false;
            }
            _DataConversionType = (DataConversionTypes)tmpDataConversionType;

            return true;
        }
        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (OmronFinsEthernetProtocol.GetMaxJobSize((LinkType)TagLinkType) >= ByteSize);
        }

        public override string ToString()
        {
            return ToStringOmronFinsEthernet(_Address);
        }

        public string ToStringOmronFinsEthernet(String inAddress)
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter, DynamicStringParser.CharAssign, inAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ConvParameter, DynamicStringParser.CharAssign, (int)_DataConversionType);

            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            OmronAddress nextAddress = new OmronAddress(_Address, prevtagdefinition);
            if (!nextAddress.IsValid)
            {
                return string.Empty;
            }
            String dynamicSettings = ToStringOmronFinsEthernet(nextAddress.Get());
            TryParse(dynamicSettings);
            return dynamicSettings;
        }

        #endregion

        #region Methods

        public bool ParseAddress(string inAddress)
        {
            try
            {
                OmronAddress addObj = new OmronAddress(inAddress);
                if (addObj.IsValid)
                {
                    _Address = inAddress;
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        public string OmronAddress()
        {
            OmronAddress addObj = new OmronAddress(_Address);
            return addObj.Get();
        }

        public string OmronFormattedAddress()
        {
            OmronAddress addObj = new OmronAddress(_Address);
            string stTmp = addObj.GetFormattedaddress();
            return addObj.GetFormattedaddress();
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            OmronAddress addObj = new OmronAddress(_Address);
            return (OmronFinsEthernetProtocol.DataType(addObj));
        }

        #endregion

        #region Properties

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}
        private String _Address;
        [Category("Device Data")]
        [Description("Address")]
        public string Address
        {
            get { return _Address; }
            set 
            { 
                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }

        private DataConversionTypes _DataConversionType;
        [Category("Device Data")]
        [Description("Data Conversion Type")]
        public DataConversionTypes DataConversionType
        {
            get { return _DataConversionType; }
            set { _DataConversionType = value; }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the address parameter
            if (propertyName == "Address")
            {
                OmronAddress fxAddress = new OmronAddress(Address);
                if (!fxAddress.IsValid)
                {
                    return Properties.Resources.ErrorInvalidAddress;
                }
                if ((VarType == UFUAModel.DataType.String) &&
                    ((fxAddress.StringLength < 1) || (fxAddress.StringLength > Properties.Settings.Default.MaxStringSize)))
                {
                    return string.Format(Properties.Resources.ErrorInvalidAddressString, Properties.Settings.Default.MaxStringSize);
                }

                if ((uint)VarType != unchecked((uint)(-1)))
                {
                    if ((uint)VarType != unchecked((uint)(-1)))
                        return ProtocolDataSizeValidation(VarType);

                }
            }
            // Check the Data Conversion parameter
            if (propertyName == "DataConversionType")
            {
                switch (VarType)
                {
                    case UFUAModel.DataType.Int16:
                    case UFUAModel.DataType.UInt16:
                        if (DataConversionType == DataConversionTypes.BCD32Bits)
                        {
                            return Properties.Resources.ErrorDataConvNotValid;
                        }
                        break;
                    case UFUAModel.DataType.Float:
                    case UFUAModel.DataType.Double:
                    case UFUAModel.DataType.Int32:
                    case UFUAModel.DataType.UInt32:
                    case UFUAModel.DataType.Int64:
                    case UFUAModel.DataType.UInt64:
                        break;
                    default:
                        if (DataConversionType != DataConversionTypes.None)
                        {
                            return Properties.Resources.ErrorDataConvNotValid;
                        }
                        break;

                }
            }

            return null;
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion
    }
}
