using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using DriverCodeBaseEx.Properties;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel;

namespace SNMP
{
    public sealed class SNMPDynTagSettings : DynTagSettings
    {
        #region Constructors

        public SNMPDynTagSettings()
            : base()
        {
            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpDataSize = 4;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = "public";
        }

        #endregion

        #region Static Members

        private static readonly String dataTypeParameter = "DA";
        private static readonly String dataSizeParameter = "DS";
        private static readonly String oidAddressParameter = "OID";
        private static readonly String communityParameter = "CMN";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _snmpDataType = (SNMPDATATYPE)(helper.GetPartByName(dataTypeParameter, (byte)SNMPDATATYPE.Integer));
            _snmpDataSize = helper.GetPartByName(dataSizeParameter, (UInt32)4);
            _snmpOid_Address = helper.GetPartByName(oidAddressParameter);
            _snmpCommunity = helper.GetPartByName(communityParameter);
            if(String.IsNullOrEmpty(_snmpCommunity))
            {
                _snmpCommunity = "public";
            }
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
            {
                return(false);
            }

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(oidAddressParameter)))
            {
                return(false);
            }

            // Data Type
            SNMPDATATYPE tempDataType = (SNMPDATATYPE)(helper.GetPartByName(dataTypeParameter, (byte)SNMPDATATYPE.Integer));
            if (tempDataType > SNMPDATATYPE.IpAddress)
            {
                return (false);
            }
            _snmpDataType = tempDataType;

            // Data Size
            _snmpDataSize = helper.GetPartByName(dataSizeParameter, (UInt32)4);

            // OID
            _snmpOid_Address = helper.GetPartByName(oidAddressParameter);
            SNMPOid testOid = new SNMPOid(_snmpOid_Address);
            if(!testOid.IsValid)
            {
                return (false);
            }

            // Community
            _snmpCommunity = helper.GetPartByName(communityParameter);
            if (String.IsNullOrEmpty(_snmpCommunity))
            {
                _snmpCommunity = "public";
            }

            return true;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (SNMPProtocol.GetMaxJobSize((LinkType)TagLinkType) >= ByteSize);
        }

        public override string ToString()
        {
            //return ToStringSNMP(_Address);
            return ToStringSNMP(_snmpOid_Address);
        }

        public string ToStringSNMP(String inAddress)
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", dataTypeParameter, DynamicStringParser.CharAssign, (byte)_snmpDataType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", dataSizeParameter, DynamicStringParser.CharAssign, (UInt32)_snmpDataSize);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", oidAddressParameter, DynamicStringParser.CharAssign, inAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", communityParameter, DynamicStringParser.CharAssign, _snmpCommunity);

            return dynamicstring.ToString();
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
        //        //OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        //OnPropertyChanged(new PropertyChangedEventArgs("Address"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}
 
        private SNMPDATATYPE _snmpDataType;
        [Category("Device Data")]
        [Description("Data Type")]
        public SNMPDATATYPE snmpDataType
        {
            get { return _snmpDataType; }
            set
            {
                _snmpDataType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("snmpDataSize"));
            }
        }

        private UInt32 _snmpDataSize;
        [Category("Device Data")]
        [Description("Data Size")]
        public UInt32 snmpDataSize
        {
            get { return _snmpDataSize; }
            set
            {
                _snmpDataSize = value;
                OnPropertyChanged(new PropertyChangedEventArgs("snmpDataType"));
            }
        }

        private string _snmpOid_Address;
        [Category("Device Data")]
        [Description("OID Address")]
        public string snmpOid_Address
        {
            get { return _snmpOid_Address; }
            set
            {
                _snmpOid_Address = value;
                //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }

        private string _snmpCommunity;
        [Category("Device Data")]
        [Description("Community")]
        public string snmpCommunity
        {
            get { return _snmpCommunity; }
            set
            {
                _snmpCommunity = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if(propertyName == "snmpOid_Address")
            {
                if(String.IsNullOrWhiteSpace(snmpOid_Address))
                {
                    return Properties.Resources.SNMPInvalidOID;
                }
                else
                {
                    SNMPOid oidObject = new SNMPOid(snmpOid_Address);
                    if(!oidObject.IsValid)
                    {
                        return Properties.Resources.SNMPInvalidOID;
                    }
                }
            }

            if ((propertyName == "snmpDataType") || (propertyName == "snmpDataSize"))
            {
                if ((snmpDataType == SNMPDATATYPE.IpAddress || snmpDataType == SNMPDATATYPE.OctetString) && snmpDataSize == 0)
                {
                    return Properties.Resources.SNMPErrorDataSizeInvalid;
                }

                if ((snmpDataType == SNMPDATATYPE.IpAddress) && (snmpDataSize < 15))
                {
                    return Properties.Resources.SNMPMinLengthOfIpAddress;
                }

                if (propertyName == "snmpDataType")
                {
                    switch (snmpDataType)
                    {
                        case SNMPDATATYPE.Integer:
                        case SNMPDATATYPE.Integer32:
                        case SNMPDATATYPE.Counter32:
                        case SNMPDATATYPE.Unsigned32:
                        case SNMPDATATYPE.Gauge32:
                        case SNMPDATATYPE.TimeTicks:
                            if ((VarType != UFUAModel.DataType.Int32) &&
                                (VarType != UFUAModel.DataType.UInt32))
                            {
                                return Properties.Resources.SNMPMismatchVarTypeInteger32;
                            }
                            break;

                        case SNMPDATATYPE.OctetString:
                            if ((VarType != UFUAModel.DataType.String) &&
                                (VarType != UFUAModel.DataType.Byte) &&
                                (VarType != UFUAModel.DataType.SByte))
                            {
                                return Properties.Resources.SNMPMismatchVarTypeOctetString;
                            }
                            else if ((VarType == UFUAModel.DataType.Byte) ||
                                    (VarType == UFUAModel.DataType.SByte))
                            {
                                if (ArrayDimension == 0)
                                {
                                    return Properties.Resources.SNMPMismatchVarTypeOctetString;
                                }
                            }
                            break;

                        case SNMPDATATYPE.IpAddress:
                            if (VarType != UFUAModel.DataType.String)
                            {
                                return Properties.Resources.SNMPMismatchVarTypeIpAddress;
                            }
                            break;
                    }
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
                    //OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    //OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion
    }
}
