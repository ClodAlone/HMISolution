using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using Opc.Ua;
using DriverBaseInterfaces;

namespace MelsecQEth
{
    public sealed class MelsecQEthDynTagSettings : DynTagSettings
    {
       #region Constructors

        public MelsecQEthDynTagSettings()
            : base()
        {
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

        private static readonly String AddressParameter = "Addr";
        private static readonly String CpuTargetParameter = "Cpu";
        private static readonly String SZTargetParameter = "DL";
        private static readonly String UNTargetParameter = "UN";

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //    }
        //}

        private string _Address;
        [Category("Device Data")]
        [Description("Address")]
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

        #endregion
 
        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            Address = helper.GetPartByName(AddressParameter);
            _CpuTarget = (CpuTargets)helper.GetPartByName(CpuTargetParameter, (UInt16)CpuTargets.ControlPLC);
            StringLength = helper.GetPartByName(SZTargetParameter,(UInt16)32);
            UnicodeString = helper.GetPartByName(UNTargetParameter, (bool)false);
            melsecQAddress.Set(Address);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            Address = helper.GetPartByName(AddressParameter);
            _CpuTarget = (CpuTargets)helper.GetPartByName(CpuTargetParameter, (UInt16)CpuTargets.ControlPLC);
            StringLength = helper.GetPartByName(SZTargetParameter, (UInt16)0);
            UnicodeString = helper.GetPartByName(UNTargetParameter, (bool)false);
            // Check the address
            melsecQAddress.Set(Address);
            if (!melsecQAddress.IsValid)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
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
            return (MelsecQEthCommJob.MAXBYTE_SIZE >= ByteSize);
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            
            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                melsecQAddress.SetStructFieldAdd((uint)prevtagdefinition.DataType.Identifier, prevtagdefinition.ArrayDimension);
                Address = melsecQAddress.Address;
            }
            return ToString();
        }

         //Redefined the method because, using the basic method not associated different types of tags.
        public override UFUAModel.DataType getProtocolDataType()
        {
            MelsecQAddress AddressObj = new MelsecQAddress(_Address);
            return (MelsecQEthCommJob.DataType(AddressObj, VarType));
        }

        #endregion

        // TODO (used in import routine)
        public bool ParseAddress(string address)
        {
            try
            {
                melsecQAddress.Set(address);
                if (melsecQAddress.IsValid)
                {
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
            if (propertyName == "Address")
            {
                MelsecQAddress qAddressObj = new MelsecQAddress(Address);
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
            else if (propertyName == "TagLinkType")
            {
                MelsecQAddress qAddressObj = new MelsecQAddress(Address);
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
            else if (propertyName == "StringLength")
            {
                if (VarType == UFUAModel.DataType.String)
                {
                    if (StringLength ==0 || MelsecQEthCommJob.GetStringJobSize(UnicodeString, StringLength) > MelsecQEthCommJob.MAXBYTE_SIZE)
                    {
                        return Properties.Resources.ErrorSizeString;
                    }
                    if (ArrayDimension != 0)
                    {
                        return Properties.Resources.ErrorStringArraryNotSupported;
                    }
                }
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
        public static bool CheckArraySize(MelsecQAddress qAddressObj, UFUAModel.DataType dataType, uint ArrayDimension)
        {
            bool bChecked = false;
            uint StartAddress = (uint)qAddressObj.StartAddress;
            if(dataType == UFUAModel.DataType.Boolean)
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
                        if (ArrayDimension > 960)
                        {
                            bChecked = true;
                        }
                        break;
                }
            }
            else
            {
                if ( ArrayDimension > 240)
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
            }
        }
        #endregion
    }
}
