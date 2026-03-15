using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;
using System.Text.RegularExpressions;


namespace Databoom
{
    public sealed class DataboomDynTagSettings : DynTagSettings
    {
        private const uint DEFAULT_SAMPLING_RATE = 60;
        #region Constructors

        public DataboomDynTagSettings()
            : base()
        {
            TagName = String.Empty;
            FrequencyOfSendingSignal = DEFAULT_SAMPLING_RATE;
            TagLinkType = (int)LinkType.UnconditionalOutput;
        }

        #endregion

        #region Static Members

        private static readonly String TagNameParameter = "TN";
        private static readonly String FrequencyOfSendingSignalParameter = "FZ";

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

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            TagName = helper.GetPartByName(TagNameParameter);
            FrequencyOfSendingSignal = helper.GetPartByName(FrequencyOfSendingSignalParameter, (UInt32)0);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(TagNameParameter)))
                return false;
            TagName = helper.GetPartByName(TagNameParameter);
            FrequencyOfSendingSignal = helper.GetPartByName(FrequencyOfSendingSignalParameter, (UInt32)0);

            // optional parameters

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", TagNameParameter,
                                       DynamicStringParser.CharAssign,
                                       TagName);
            dynamicstring.AppendFormat("|{0}{1}{2}", FrequencyOfSendingSignalParameter,
                                       DynamicStringParser.CharAssign,
                                       FrequencyOfSendingSignal);

            return dynamicstring.ToString();
        }

        public bool ParseAddress(string address/*, string stationname, ref string dynamicaddress*/)
        {
            try
            {
                if (address.Length > 0)
                {
                    _TagName = address;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            if (!TryParse(tag.TagNode.DynamicSettings))
            {
                return tag.TagNode.DynamicSettings;
            }
            else
            {
                return GetTagNextDynSetting(thistagdefinition);
            }
        }

        public string GetTagNextDynSetting(TagDefinition thistagdefinition)
        {
            string prevAddress = _TagName;
            string fieldName = GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            _TagName += ("." + fieldName.Replace('/', '.'));
            string dynSettings = ToString();
            _TagName = prevAddress;
            return dynSettings;
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetTagNextDynSetting(thistagdefinition);
        }

        #endregion

        #region Properties

        private string _TagName;
        [Category("Device Data")]
        [Description("Tag Name")]
        [Size(SizeAttribute.Unlimited)]
        public string TagName
        {
            get { return _TagName; }
            set { _TagName = value; }
        }

        private UInt32 _FrequencyOfSendingSignal;
        [Category("Device Data")]
        [Description("Tag Name")]
        [Size(SizeAttribute.Unlimited)]
        public UInt32 FrequencyOfSendingSignal
        {
            get { return _FrequencyOfSendingSignal; }
            set { _FrequencyOfSendingSignal = value; }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the "Tag Name" parameter
            if (propertyName == "TagName")
            {
                if(String.IsNullOrWhiteSpace(TagName))
                {
                    return (Properties.Resources.DataboomErrorInvalidTagName);
                }
            }

            if (propertyName == "FrequencyOfSendingSignal")
            {
                if (FrequencyOfSendingSignal <= 0)
                {
                    return (Properties.Resources.DataboomErrorInvalidFrequencyOfSendingSignal);
                }
            }

            if (propertyName == "FrequencyOfSendingSignal")
            {
                if (FrequencyOfSendingSignal <= 0)
                {
                    return (Properties.Resources.DataboomErrorInvalidFrequencyOfSendingSignal);
                }
            }

            if (propertyName == "TagLinkType")
            {
                if (base.TagLinkType != (int)LinkType.UnconditionalOutput)
                {
                    return (Properties.Resources.DataboomErrorLinkTypeAllowed);
                }
            }

            if (propertyName == "VarType")
            {
                if (VarType == UFUAModel.DataType.String)
                {
                    return (UFUAModel.Properties.Resources.DataTypeIncompatible);
                }
            }

            if (propertyName == "ArrayDimension")
            {
                if (ArrayDimension  != 0)
                {
                    return (UFUAModel.Properties.Resources.ErroorProtocolSetIncompatibleWithArrays);
                }
            }

            return null;
        }

        #endregion
        
    }
}
