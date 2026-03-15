using System;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;

namespace SNMP
{
    public sealed class SNMPDynTagSettings : DynTagSettings
    {
        #region Constructors

        public SNMPDynTagSettings()
            : base()
        {
            _snmpDataType = SNMPDATATYPE.Integer;
            _snmpOid_Address = String.Empty;
            _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;
            _snmpTrapOnly = false;
        }

        #endregion

        #region Static Members

        private static readonly String dataTypeParameter = "DA";
        private static readonly String oidAddressParameter = "OID";
        private static readonly String communityParameter = "CMN";
        private static readonly String trapOnlyParameter = "TO";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _snmpDataType = (SNMPDATATYPE)(helper.GetPartByName(dataTypeParameter, (byte)SNMPDATATYPE.Integer));
            _snmpOid_Address = helper.GetPartByName(oidAddressParameter);
            _snmpCommunity = helper.GetPartByName(communityParameter);
            if (String.IsNullOrEmpty(_snmpCommunity))
                _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;
            _snmpTrapOnly = helper.GetPartByName(trapOnlyParameter, false);
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

            // OID
            _snmpOid_Address = helper.GetPartByName(oidAddressParameter);
            SNMPOid testOid = new SNMPOid(_snmpOid_Address);
            if (!testOid.IsValid())
                return (false);

            // Community
            _snmpCommunity = helper.GetPartByName(communityParameter);
            if (String.IsNullOrEmpty(_snmpCommunity))
                _snmpCommunity = SNMPProtocol.SNMP_COMUNITY_PUBLIC;

            _snmpTrapOnly = helper.GetPartByName(trapOnlyParameter, false);

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
            dynamicstring.AppendFormat("{0}{1}{2}", oidAddressParameter, DynamicStringParser.CharAssign, inAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", communityParameter, DynamicStringParser.CharAssign, _snmpCommunity);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", trapOnlyParameter, DynamicStringParser.CharAssign, _snmpTrapOnly);

            return dynamicstring.ToString();
        }

        #endregion

        #region Properties       
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

        private bool _snmpTrapOnly;
        [Category("Device Data")]
        [Description("Trap Only")]
        public bool snmpTrapOnly
        {
            get { return _snmpTrapOnly; }
            set
            {
                _snmpTrapOnly = value;
            }
        }        
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "snmpOid_Address":
                    if (String.IsNullOrWhiteSpace(snmpOid_Address))
                    {
                        return Properties.Resources.SNMPInvalidOID;
                    }
                    else
                    {
                        SNMPOid oidObject = new SNMPOid(snmpOid_Address);
                        if (!oidObject.IsValid())
                        {
                            return Properties.Resources.SNMPInvalidOID;
                        }
                    }
                    break;

                case "snmpDataType":               
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
                    break;

                case "snmpTrapOnly":
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
