using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using Opc.Ua;
using DriverBaseInterfaces;
using DevExpress.Xpo;

namespace MelsecQEth
{
    public sealed class MelsecQEthDynTagSettings : DynTagSettings
    {
        #region Constructors

        public MelsecQEthDynTagSettings()
            : base()
        {
            AddressType = MelsecQEthProtocol.AddressTypes.DataArea;
            Address = String.Empty;
            _CpuTarget = CpuTargets.ControlPLC;
            melsecQAddress = new MelsecQAddress();
            _StringLength = 0;
            _UnicodeString = false;
        }

        #endregion

        #region Members

        MelsecQAddress melsecQAddress;

        #endregion

        #region Static Members

        private static readonly String AddressTypeParameter = "AddrTy";
        private static readonly String AddressParameter = "Addr";
        private static readonly String CpuTargetParameter = "Cpu";
        private static readonly String SZTargetParameter = "DL";
        private static readonly String UNTargetParameter = "UN";

        #endregion

        #region Properties

        private MelsecQEthProtocol.AddressTypes _AddressType;
        [Category("Device Data")]
        [Description("AddressType")]
        public MelsecQEthProtocol.AddressTypes AddressType
        {
            get { return _AddressType; }
            set
            {
                _AddressType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Address"));                
            }
        }

        private string _Address;
        [Category("Device Data")]
        [Description("Address")]
        [Size(SizeAttribute.Unlimited)]
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }

        private CpuTargets _CpuTarget;
        [Category("Device Data")]
        [Description("Cpu Target")]
        public CpuTargets CpuTarget
        {
            get
            {
                return _CpuTarget;
            }

            set
            {
                _CpuTarget = value;
            }
        }

        private UInt16 _StringLength;
        [Category("Device Data")]
        [Description("String Length")]
        public UInt16 StringLength
        {
            get
            {
                return _StringLength;
            }

            set
            {
                _StringLength = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
            }
        }

        private bool _UnicodeString;
        [Category("Device Data")]
        [Description("String Length")]
        public bool UnicodeString
        {
            get
            {
                return _UnicodeString;
            }

            set
            {
                _UnicodeString = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StringLength"));
            }
        }

        /// <summary>
        /// Dictionary of station _stationSettingList<Neme, Ogbject Station> 
        /// </summary>
        private Dictionary<string, MelsecQEthStationSettings> _stationSettingList;
        public Dictionary<string, MelsecQEthStationSettings> stationSettingList
        {
            get { return _stationSettingList; }
            set
            {
                _stationSettingList = value;
            }
        }

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            AddressType = (MelsecQEthProtocol.AddressTypes)helper.GetPartByName(AddressTypeParameter, (byte)MelsecQEthProtocol.AddressTypes.DataArea);
            Address = helper.GetPartByName(AddressParameter);
            _CpuTarget = (CpuTargets)helper.GetPartByName(CpuTargetParameter, (UInt16)CpuTargets.ControlPLC);
            StringLength = helper.GetPartByName(SZTargetParameter,(UInt16)32);
            UnicodeString = helper.GetPartByName(UNTargetParameter, (bool)false);
            melsecQAddress.Set(AddressType, Address);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            AddressType = (MelsecQEthProtocol.AddressTypes)helper.GetPartByName(AddressTypeParameter, (byte)MelsecQEthProtocol.AddressTypes.DataArea);
            Address = helper.GetPartByName(AddressParameter);
            _CpuTarget = (CpuTargets)helper.GetPartByName(CpuTargetParameter, (UInt16)CpuTargets.ControlPLC);
            StringLength = helper.GetPartByName(SZTargetParameter, (UInt16)0);
            UnicodeString = helper.GetPartByName(UNTargetParameter, (bool)false);
            // Check the address
            melsecQAddress.Set(AddressType, Address);
            
            return melsecQAddress.IsValid;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressTypeParameter,DynamicStringParser.CharAssign, (byte)AddressType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter,
                                       DynamicStringParser.CharAssign,
                                       Address);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CpuTargetParameter, DynamicStringParser.CharAssign, (uint)_CpuTarget);
            if (VarType == UFUAModel.DataType.String)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SZTargetParameter, DynamicStringParser.CharAssign,(uint)_StringLength);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", UNTargetParameter, DynamicStringParser.CharAssign, (bool)_UnicodeString);
            }

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (MelsecQEthProtocol.MAXBYTE_SIZE >= ByteSize);
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            return GetNodeDynSetting(thistagdefinition);            
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            if (AddressType == MelsecQEthProtocol.AddressTypes.Label)
            {
                return GetNodeDynSetting(thistagdefinition);
            }
            else
            {
                if (prevtagdefinition.DataType.IdType == IdType.Numeric)
                {
                    melsecQAddress.SetStructFieldAdd((uint)prevtagdefinition.DataType.Identifier, prevtagdefinition.ArrayDimension);
                    Address = melsecQAddress.Address;
                }
                return ToString();
            }
        }

        string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string memABAddress = Address;
            string Tree = MelsecQEthProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            Address += ("." + Tree.Replace('/', '.'));
            string dynsettings = ToString();
            Address = memABAddress;

            return dynsettings;
        }

        //Redefined the method because, using the basic method not associated different types of tags.
        public override UFUAModel.DataType getProtocolDataType()
        {
            MelsecQAddress AddressObj = new MelsecQAddress(_AddressType, _Address);
            return (MelsecQEthCommJob.DataType(AddressObj, VarType));
        }

        #endregion

        // TODO (used in import routine)
        public bool ParseAddress(MelsecQEthProtocol.AddressTypes addressType, string address)
        {
            try
            {
                melsecQAddress.Set(addressType, address);
                if (melsecQAddress.IsValid)
                {
                    _AddressType = addressType;
                    _Address = address;
                    if (melsecQAddress.DataArea == DataArea.X)
                    {
                        TagLinkType = (int)LinkType.Input;
                    }
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the address parameter
            switch (propertyName)
            {
                case "AddressType":
                    {
                        // check if selected station support label addressing
                        if (_AddressType == MelsecQEthProtocol.AddressTypes.Label)
                        {
                            if (stationSettingList != null)
                            {
                                if (!(stationSettingList.ContainsKey(StationName) && MelsecQEthProtocol.PlcSupportLabelAddress((MelsecQEthProtocol.PlcTypes)stationSettingList[StationName].PlcType)))
                                    return String.Format(Properties.Resources.ErrorPlcDontSupportLabelAddress, StationName);
                            }
                        }
                    }
                    break;
                case "Address":
                    {
                        if (_AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                        {
                            MelsecQAddress qAddressObj = new MelsecQAddress(AddressType, Address);
                            if (!qAddressObj.IsValid)
                            {
                                return Properties.Resources.ErrorInvalidAddress;
                            }
                            if (qAddressObj.DataArea == DataArea.X)
                            {
                                if (TagLinkType != (int)LinkType.Input)
                                {
                                    return Properties.Resources.ErrorReadOnly;
                                }
                            }
                            //if (VarType == UFUAModel.DataType.String)
                            //{
                            //    return string.Format(Properties.Resources.ErrorInvalidTagType,Address,VarType);
                            //}     
                            if (CheckVariablesSize(qAddressObj, VarType))
                            {
                                //return string.Format(Properties.Resources.ErrorInvalidAssignedAddress, Address);
                                return string.Format(Properties.Resources.ErrorInvalidSize);
                            }
                            else if (CheckArraySize(qAddressObj, VarType, ArrayDimension))
                            {
                                return string.Format(Properties.Resources.ErrorInvalidSize);
                            }
                            else if ((uint)VarType != unchecked((uint)(-1)))
                            {
                                return ProtocolDataSizeValidation(VarType);
                            }
                        }
                        else
                        {
                            MelsecQAddress qAddressSymObj = new MelsecQAddress(AddressType, Address);
                            if (!qAddressSymObj.IsValid)                            
                                return Properties.Resources.InvalidLabelAddress;
                        }
                    }
                    break;
                case "TagLinkType":
                    {
                        if (_AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                        {
                            MelsecQAddress qAddressObj = new MelsecQAddress(AddressType, Address);
                            if (qAddressObj.IsValid == true)
                            {
                                if (qAddressObj.DataArea == DataArea.X)
                                {
                                    if (TagLinkType != (int)LinkType.Input)
                                    {
                                        return Properties.Resources.ErrorReadOnly;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "StringLength":
                    {
                        if (VarType == UFUAModel.DataType.String)
                        {                            
                            if (_AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                            {
                                if (StringLength == 0 || MelsecQEthProtocol.GetStringJobSize(AddressType, UnicodeString, StringLength) > MelsecQEthProtocol.MAXBYTE_SIZE)
                                    return Properties.Resources.ErrorSizeString;
                                if (ArrayDimension != 0)
                                    return Properties.Resources.ErrorStringArraryNotSupported;
                            }
                            else
                            {
                                // StringLength --> driver request string size at runtime
                                if (MelsecQEthProtocol.GetStringJobSize(AddressType, UnicodeString, StringLength) > MelsecQEthProtocol.MAXBYTE_SIZE)
                                    return Properties.Resources.ErrorSizeString;
                            }
                        }
                    }
                    break;
            }

            return null;
        }

        public static bool CheckVariablesSize(MelsecQAddress qAddressObj, UFUAModel.DataType dataType)
        {
            bool bChecked = false;

            switch (qAddressObj.DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.S:
                case DataArea.M:
                case DataArea.SM:
                case DataArea.L:
                case DataArea.B:
                case DataArea.SB:
                case DataArea.F:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                    if (dataType == UFUAModel.DataType.Double || dataType == UFUAModel.DataType.Float) 
                    {
                        bChecked = true;
                    }
                    else if ((dataType != UFUAModel.DataType.Boolean) && ((qAddressObj.StartAddress % 16) != 0))
                    {
                        bChecked = true;
                    }
                    break;

                default:
                    break;
            }
            return bChecked;
        }

        public bool CheckArraySize(MelsecQAddress qAddressObj, UFUAModel.DataType dataType, uint ArrayDimension)
        {
            bool bChecked = false;
            uint protocolSize = MelsecQEthProtocol.MAXBYTE_SIZE;
            if ((stationSettingList != null) &&
                (stationSettingList.ContainsKey(StationName)))
            {
                protocolSize = MelsecQEthProtocol.GetMaxJobSize((MelsecQEthProtocol.PlcTypes)stationSettingList[StationName].PlcType);
            }

            uint StartAddress = (uint)qAddressObj.StartAddress;
            if (dataType == UFUAModel.DataType.Boolean)
            {
                switch (qAddressObj.DataArea)
                {
                    case DataArea.X:
                    case DataArea.Y:
                    case DataArea.S:
                    case DataArea.M:
                    case DataArea.SM:
                    case DataArea.L:
                    case DataArea.B:
                    case DataArea.SB:
                    case DataArea.F:
                    case DataArea.TS:
                    case DataArea.TC:
                    case DataArea.CS:
                    case DataArea.CC:
                        if (ArrayDimension > protocolSize)
                        {
                            bChecked = true;
                        }
                        break;
                }
            }
            else
            {
                if (ArrayDimension > protocolSize)
                {
                    bChecked = true;
                }
            }

            return bChecked;
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;                
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
                case "StationName":
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    break;
            }
        }
        #endregion
    }
}
