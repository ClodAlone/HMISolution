using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;
using System.Text.RegularExpressions;


namespace MTConnect
{
    public sealed class MTConnectDynTagSettings : DynTagSettings
    {
        private const uint DEFAULT_SAMPLING_RATE = 60;
        #region Constructors

        public MTConnectDynTagSettings()
            : base()
        {
            TagLinkType = (int)LinkType.Input;

            TagName = String.Empty;
            PathParameter = String.Empty;

        }

        #endregion

        #region Static Members

        private static readonly String TagNameParameter = "AD";
        private static readonly String TagPathParameter = "PH";


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
            TagName = helper.GetPartByName(TagName);
            PathParameter = helper.GetPartByName(PathParameter);
            //FrequencyOfSendingSignal = (uint )helper.GetPartByName(FrequencyOfSendingSignalParameter, (Int32)0);
            //Category = MTConnectProtocol.GetCategory(helper.GetPartByName(TagCategoryParameter));
            //SupervisonDataType = MTConnectProtocol.GetFormatMTConnect(helper.GetPartByName(TagDataTypeParameter));
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
            PathParameter = helper.GetPartByName(TagPathParameter);
            //FrequencyOfSendingSignal = (uint) helper.GetPartByName(FrequencyOfSendingSignalParameter, (UInt32)0);
            //Category = MTConnectProtocol.GetCategory(helper.GetPartByName(TagCategoryParameter));
            //SupervisonDataType = MTConnectProtocol.GetFormatMTConnect(helper.GetPartByName(TagDataTypeParameter));

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
            dynamicstring.AppendFormat("|{0}{1}{2}", TagPathParameter,
                                      DynamicStringParser.CharAssign,
                                      PathParameter);

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

        private string _PathParameter;
        [Category("Device Data")]
        [Description("Path Parametrer")]
        [Size(SizeAttribute.Unlimited)]
        public string PathParameter
        {
            get { return _PathParameter; }
            set { _PathParameter = value; }
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
                if (String.IsNullOrWhiteSpace(TagName))
                {
                    return (Properties.Resources.MTConnectErrorInvalidTagName);
                }
            }

            if (propertyName == "PathParameter")
            {
                if (String.IsNullOrWhiteSpace(PathParameter))
                {
                    return (Properties.Resources.MTConnectErrorInvalidPathParameter);
                }
            }

            if (propertyName == "TagLinkType")
            {
                if (base.TagLinkType != (int)LinkType.Input)
                {
                    return (Properties.Resources.MTConnectErrorLinkTypeAllowed);
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
