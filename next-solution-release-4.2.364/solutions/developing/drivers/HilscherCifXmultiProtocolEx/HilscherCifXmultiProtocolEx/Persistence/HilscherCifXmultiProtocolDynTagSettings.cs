using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;

namespace HilscherCifXmultiProtocol
{
    public sealed class HilscherCifXmultiProtocolDynTagSettings : DynTagSettings
    {
        #region Constructors

        public HilscherCifXmultiProtocolDynTagSettings()
            : base()
        {
            DataAddress = 0;
            Valid = false;
        }

        bool Valid = false;

        #endregion
         
        #region Static Members

        private static readonly String DataAddressParameter = "Addr";
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            DataAddress = helper.GetPartByName(DataAddressParameter, (UInt32)0);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
            {
                return false;
            }

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(DataAddressParameter)))
            {
                return false;
            }

            DataAddress = helper.GetPartByName(DataAddressParameter, (UInt32)0);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataAddressParameter, DynamicStringParser.CharAssign, DataAddress);
            return dynamicstring.ToString();
        }


        public bool ParseAddress(string address/*, string stationname, ref string dynamicaddress*/)
        {
            try
            {
                if (address.Length > 0)
                {
                    UInt32 sa = Convert.ToUInt32(address);
                    DataAddress = sa;
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (HilscherCifXmultiProtocolProtocol.MAX_DATA_BYTES >= ByteSize);
        }

        #endregion

        #region Properties

        private uint _DataAddress;
        [Category("Device Data")]
        [Description("Data Address")]
        public uint DataAddress
        {
            get { return _DataAddress; }
            set { _DataAddress = value; }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            // FOGBUGZ 12126
            if (propertyName == "DataAddress")
            {
                if((VarType == UFUAModel.DataType.Boolean) || (VarType == UFUAModel.DataType.String))
                {
                    return (Properties.Resources.ErrorBooleanString);
                }
            }

            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DataAddress")
            {
                if (DataAddress > 65535)
                {
                    return(Properties.Resources.ErrorInvalidAddress);
                }
            }

            return null;
        }

        #endregion
    }
}
