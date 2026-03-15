using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;

namespace PhoenixContactPLCI
{
    public sealed class PhoenixContactPLCIDynTagSettings : DynTagSettings
    {
        #region Constructors
        public PhoenixContactPLCIDynTagSettings()
            : base()
        {
            _Address = String.Empty;
            _DataFormat = PhoenixContactPLCIProtocol.VarType.UNKNOWN;
            _StringLength = 0;

            _ParseOk = false;
        }

        #endregion

        #region Static Members
                
        private static readonly String AddressParameter = "PLCIADDR";
        private static readonly String DataFormatParameter = "PLCITYPPH";
        private static readonly String StringLengthaFormatParameter = "PLCIDL";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            _Address = helper.GetPartByName(AddressParameter);
            _DataFormat = (PhoenixContactPLCIProtocol.VarType)(helper.GetPartByName(DataFormatParameter, (uint)PhoenixContactPLCIProtocol.VarType.UNKNOWN));
            _StringLength = (uint)helper.GetPartByName(StringLengthaFormatParameter, 0);   

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
            _DataFormat = (PhoenixContactPLCIProtocol.VarType)(helper.GetPartByName(DataFormatParameter, (uint)PhoenixContactPLCIProtocol.VarType.UNKNOWN));
            _StringLength = (uint)helper.GetPartByName(StringLengthaFormatParameter, 0);

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
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataFormatParameter, DynamicStringParser.CharAssign, (int)_DataFormat);
            if (_DataFormat == PhoenixContactPLCIProtocol.VarType.STRING)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StringLengthaFormatParameter, DynamicStringParser.CharAssign, (int)_StringLength);
            }
            
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
            PhoenixContactPLCIProtocol.VarType memVarType = _DataFormat;

            string Tree = PhoenixContactPLCIProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            Address += ("." + Tree.Replace('/', '.'));
            _DataFormat = PhoenixContactPLCIProtocol.GetDataFormatFromMoviconDataType((uint)thistagdefinition.DataType.Identifier);
            _StringLength = 0;
            string dynsettings = ToString();

            Address = memABAddress;
            _DataFormat = memVarType;

            return dynsettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (PhoenixContactPLCIProtocol.MAX_DATA_BYTES >= ByteSize);
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            return PhoenixContactPLCIProtocol.GetDataType(_DataFormat);
        }

        #endregion

        #region Functions

        private uint GetStringLengthFromAddress(PhoenixContactPLCIProtocol.VarType varType, ref string address)
        {
            return GetStringLengthFromAddress(varType, ref address, false);
        }

        private uint GetStringLengthFromAddress(PhoenixContactPLCIProtocol.VarType varType, ref string address, bool useDefaultIf0)
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
                    case PhoenixContactPLCIProtocol.VarType.STRING:
                        Length = 255;
                        break;
                    //case PhoenixContactPLCIProtocol.VarType.STRING:
                    //    Length = 128;
                    //    break;
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

            if (string.IsNullOrEmpty(_Address)) // || _PhoenixContactPLCIVarType == PhoenixContactPLCIProtocol.VarType.VAR_TYPE_E_UNKNOWN)
                _ParseOk = false;
            
            
            return _ParseOk;
        }

        #endregion

        #region Properties
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

        private PhoenixContactPLCIProtocol.VarType _DataFormat;
        [Category("Device Data Format")]
        [Description("PhoenixContactPLCI DataFormat")]
        public PhoenixContactPLCIProtocol.VarType DataFormat
        {
            get { return _DataFormat; }
            set
            {
                _DataFormat = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StringLength"));
            }
        }

        /// <summary>
        /// String length
        /// </summary>
        private uint _StringLength;       
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        private Dictionary<string, PhoenixContactPLCIStationSettings> _stationSettingList;
        public Dictionary<string, PhoenixContactPLCIStationSettings> stationSettingList
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
                        return Properties.Resources.ErrorAddressEmpty;

                    if (_Address.IndexOf("@GlobalVariables.") == -1 && _Address.IndexOf("@InstanceVariables.") == -1)
                        return Properties.Resources.ErrorAddressWrongFormat;


                    break;
                case "StringLength":
                    if (_DataFormat == PhoenixContactPLCIProtocol.VarType.STRING) {
                        if (!PhoenixContactPLCIProtocol.IsValidStringSize(_StringLength))
                            return Properties.Resources.ErrorInvalidStringLength;
                    }
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
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    break;
            }
        }
        #endregion
    }
}
