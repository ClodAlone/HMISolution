using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;

namespace CoDeSys
{
    public sealed class CoDeSysDynTagSettings : DynTagSettings
    {
        #region Constructors
        public CoDeSysDynTagSettings()
            : base()
        {
            _Address = String.Empty;
            _ShortAddress = string.Empty;
            _CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
            _StringLength = 0;

            _ParseOk = false;
            
            //Symbolic Address
            //_TagName = string.Empty;
        }

        #endregion

        #region Static Members
                
        private static readonly String CoDeSysVarTypeParameter = "CDSTYP";
        private static readonly String AddressParameter = "CDSADR";
        
        #endregion

        #region Override Functions
        
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            _Address = helper.GetPartByName(AddressParameter);
            // CoDeSys in not case sensitive --> force ToLower avoid later duplicate address in map
            _ShortAddress = _Address.ToLower();
            //_CoDeSysVarType = (CoDeSysProtocol.VarType)(helper.GetPartByName(CoDeSysVarTypeParameter, (uint)CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN));            
            //if (CoDeSysProtocol.IsStringType(_CoDeSysVarType))
            //    _StringLength = GetStringLengthFromAddress(_CoDeSysVarType, ref _ShortAddress, true);                
            //else
            //    _StringLength = 0;
            _CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
            _StringLength = 0;

            ParseAddress();
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
            // CoDeSys in not case sensitive --> force ToLower avoid later duplicate address in maps
            _ShortAddress = _Address.ToLower();
            _CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
            _StringLength = 0;
            //_CoDeSysVarType = (CoDeSysProtocol.VarType)(helper.GetPartByName(CoDeSysVarTypeParameter, (uint)CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN));
            //if (CoDeSysProtocol.IsStringType(_CoDeSysVarType))
            //    _StringLength = GetStringLengthFromAddress(_CoDeSysVarType , ref _ShortAddress, true);
            //else
            //    _StringLength = 0;

            return ParseAddress();
        }

        public void AddStringLengthToAddress(uint length)
        {
            _Address += ":" + length.ToString();
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());

            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter, DynamicStringParser.CharAssign, _Address);
            //dynamicstring.Append(DynamicStringParser.CharSep);
            //dynamicstring.AppendFormat("{0}{1}{2}", CoDeSysVarTypeParameter, DynamicStringParser.CharAssign, (int)_CoDeSysVarType);
            
            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetNodeDynSetting(thistagdefinition);
        }
        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            return GetNodeDynSetting(thistagdefinition);
        }

        string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string memABAddress = Address;
            CoDeSysProtocol.VarType memVarType = _CoDeSysVarType;

            string Tree = CoDeSysProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            Address += ("." + Tree.Replace('/', '.'));
            _ShortAddress = _Address.ToLower();
            _CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN; // CoDeSysProtocol.GetDataTypeFromUAMode(thistagdefinition.DataType.Identifier.ToString());
            _StringLength = 0;
            string dynsettings = ToString();

            Address = memABAddress;
            _CoDeSysVarType = memVarType;

            return dynsettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (CoDeSysProtocol.MAX_DATA_BYTES >= ByteSize);
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            return CoDeSysProtocol.GetDataType(_CoDeSysVarType);
        }

        #endregion

        #region Functions

        private uint GetStringLengthFromAddress(CoDeSysProtocol.VarType varType, ref string address)
        {
            return GetStringLengthFromAddress(varType, ref address, false);
        }

        private uint GetStringLengthFromAddress(CoDeSysProtocol.VarType varType, ref string address, bool useDefaultIf0)
        {
            uint Length = 0;

            var Splitted = address.Split(':');
            if (Splitted.Length>1)
            {
                uint Dummy = 0;
                if (uint.TryParse(Splitted[1], out Dummy))
                {
                    Length = Dummy;
                    address = Splitted[0];
                }
            }

            if (Length ==0 && useDefaultIf0)
            {
                switch (varType)
                {
                    case CoDeSysProtocol.VarType.VAR_TYPE_STRING:
                        Length = 255;
                        break;
                    case CoDeSysProtocol.VarType.VAR_TYPE_WSTRING:
                        Length = 128;
                        break;
                }
            }

            return Length;
        }

        public bool ParseAddress(string InAddress)
        {
            return ParseAddress();
        }

        public bool ParseAddress()
        {
            _ParseOk = true;

            if (string.IsNullOrEmpty(_Address)) // || _CoDeSysVarType == CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN)
                _ParseOk = false;
            
            
            return _ParseOk;
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Station Name. </summary>
        ///
        /// <value> The name of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Station Name")]
        public string StationName
        {
            get
            {
                return base.StationName;
            }
            set
            {
                base.StationName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
            }
        }

        /// <summary>
        /// Variable address
        /// </summary>
        private string _Address;
        [Category("Device Data")]
        [Description("Address")]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private CoDeSysProtocol.VarType _CoDeSysVarType;
        [Category("Device Data")]
        [Description("CoDeSys VarType")]
        public CoDeSysProtocol.VarType CoDeSysVarType
        {
            get { return _CoDeSysVarType; }
            set { _CoDeSysVarType = value; }
        }

        private uint _StringLength;       
        public uint StringLength
        {
            get { return _StringLength; }
        }

        /// <summary>
        /// Variable address without string length
        /// </summary>
        private string _ShortAddress;
        public string ShortAddress
        {
            get { return _ShortAddress; }
            set { _ShortAddress = value; }
        }

        private Dictionary<string, CoDeSysStationSettings> _stationSettingList;
        public Dictionary<string, CoDeSysStationSettings> stationSettingList
        {
            get { return _stationSettingList; }
            set
            {
                _stationSettingList = value;
            }
        }
                
        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) {
                case "Address":
                    if (string.IsNullOrWhiteSpace(_Address))
                        return Properties.Resources.ErrorAddressInvalid;
                    break;
                case "ElementNumber":
                    if (ElementNumber<0 || ElementNumber>63)
                        return Properties.Resources.ErrorElementNumberInvalid;
                    break;
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
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
                case "ElementNumber":
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
            }
        }
        #endregion
    }
}
