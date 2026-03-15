using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;
using System.Reflection;
using DevExpress.Xpo;

namespace TwinCAT
{
    public sealed class TwinCATDynTagSettings : DynTagSettings
    {
        #region Constructors

        public TwinCATDynTagSettings()
            : base()
        {
            Address = String.Empty;
            Length = 0;
            TwinCATVersion = (byte)TwinCATVersions.Version2x;
            tcAddress = new TwinCATAddress();
        }

        #endregion

        #region Members

        TwinCATAddress tcAddress;

        #endregion

        #region Static Members

        private static readonly String AddressParameter = "SA";
        private static readonly String LengthParameter = "DL";

        #endregion

        #region Static Methods
        public static string GetNodeTree(NodeId NodeId, string Name)
        {
            string Out = "";

            Regex NameParser = new Regex(@"[\d]+:(?<Name>[\w]+)$");
            Match NameMatch = NameParser.Match(Name);
            if (NameMatch.Success)
            {
                Out = NameMatch.Groups["Name"].Value;
                string nameNodeId = NodeId.Identifier.ToString();
                Regex NodeParser = new Regex(@"^[^?]+[?](?<Node>[\w/]+)/[\w-]+$");
                Match NodeMatch = NodeParser.Match(nameNodeId);
                if (NodeMatch.Success)
                {
                    Out = NodeMatch.Groups["Node"].Value + "." + Out;
                }
            }
            return Out;

        }
        #endregion

        #region Override Functions

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (TwinCATProtocol.MAX_DATA_BYTES >= ByteSize);
        }

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            Address = helper.GetPartByName(AddressParameter);
            Length = helper.GetPartByName(LengthParameter, (UInt32)0);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            Address = helper.GetPartByName(AddressParameter);

            // Optional parameter
            Length = helper.GetPartByName(LengthParameter, (UInt32)0);

            // Check the address
            tcAddress.Set(Address, TwinCATVersion);
            if (!tcAddress.IsValid)
            {
                return false;
            }

            return true;
        }

        public bool ParseAddress(string address)
        {
            try
            {
                tcAddress.Set(address, TwinCATVersion);
                if (tcAddress.IsValid)
                {
                    _Address = address;
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter,
                                       DynamicStringParser.CharAssign,
                                       Address);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", LengthParameter,
                                       DynamicStringParser.CharAssign,
                                       (UInt32)Length);

            return dynamicstring.ToString();
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            //if (!TryParse(tag.TagNode.DynamicSettings) || tcAddress.IsNumeric)
            //{
            //    return tag.TagNode.DynamicSettings;
            //}

            //return GetTagNextDynSetting(thistagdefinition);
            tcAddress.TwinCATVersion = TwinCATVersion;
            if (!TryParse(tag.TagNode.DynamicSettings))
            {
                return tag.TagNode.DynamicSettings;
            }
            else if (tcAddress.IsNumeric)
            {
                tcAddress.SetFirstDynSettings();
                _Address = tcAddress.Address;
                return (ToString());
            }
            else
            {
                return GetTagNextDynSetting(thistagdefinition);
            }
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            tcAddress.TwinCATVersion = TwinCATVersion;
            if (tcAddress.IsNumeric)
            {
                tcAddress.GetNextDynSetting(prevtagdefinition, thistagdefinition);
                _Address = tcAddress.Address;
                return ToString();
            }
            else
            {
                return GetTagNextDynSetting(thistagdefinition);
            }
        }

        public string GetTagNextDynSetting(TagDefinition thistagdefinition)
        {
            string prevAddress = _Address;
            string fieldName = GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            _Address += ("." + fieldName.Replace('/', '.'));
            string dynSettings = ToString();
            _Address = prevAddress;
            return dynSettings;
        }

        #endregion

        #region Properties

        private string _Address;
        [Category("Device Data")]
        [Description("Address")]
        [Size(SizeAttribute.Unlimited)]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private UInt32 _Length;
        [Category("Device Data")]
        [Description("Data Length")]
        public UInt32 Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private byte _TwinCATVersion;
        [Category("Device Data")]
        [Description("TwinCAT Version")]
        public byte TwinCATVersion
        {
            get { return _TwinCATVersion; }
            set { _TwinCATVersion = value; }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the address parameter
            switch (propertyName) {
                case "Address":
                    TwinCATAddress tcAddress = new TwinCATAddress(Address, TwinCATVersion);
                    if (!tcAddress.IsValid)
                    {
                        return Properties.Resources.ErrorInvalidAddress;
                    }
                    if (ArrayDimension > 0)
                    {
                        if (((tcAddress.DataFormat == TwinCATDataFormat.DataFormat_Bit) || (tcAddress.DataFormat == TwinCATDataFormat.DataFormat_Byte)) ^ VarType == UFUAModel.DataType.Boolean && tcAddress.IsNumeric)
                        {
                            return UFUAModel.Properties.Resources.DataTypeIncompatible;
                        }
                    }
                    else
                    {
                        if (tcAddress.DataFormat == TwinCATDataFormat.DataFormat_Bit ^ VarType == UFUAModel.DataType.Boolean && tcAddress.IsNumeric)
                        {
                            return UFUAModel.Properties.Resources.DataTypeIncompatible;
                        }
                    }
                    break;
                case "Length":
                    if (VarType == UFUAModel.DataType.String)
                    {
                        if (!TwinCATProtocol.IsValidStringSize(Length))
                            return string.Format(Properties.Resources.ErrorStringLengthOutOfRange, TwinCATProtocol.MAX_DATA_BYTES);
                    }
                    break;
            }
            return null;
        }

        #endregion
    }
}
