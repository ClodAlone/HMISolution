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
using System.Reflection;
using DevExpress.Xpo;

namespace BrPvi
{
    public sealed class BrPviDynTagSettings : DynTagSettings
    {
        #region Constructors

        public BrPviDynTagSettings()
            : base()
        {
            BrPviVariableName = String.Empty;
            BrPviTaskName = String.Empty;
            BrPviRefreshRate = 500;
            BrPviArrayLength = 0;
            BrPviStringType = (uint)BrPviDynTagSettings.EnStringType.String;
        }

        #endregion

        #region Static Members

        private static readonly String PviVariableNameParameter = "PviVar";
        private static readonly String PviTaskNameParameter = "PviTask";
        private static readonly String RefreshRateParameter = "RR";
        private static readonly String ArrayLengthParameter = "AL";
        private static readonly String StringTyteParameter = "STRTY";

        /// <summary>   modes to access the tags. </summary>
        public enum EnStringType
        {
            /// <summary>   String utf8. </summary>
            String = 0,
            /// <summary>   String utf16. </summary>
            WString,
        }


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
            BrPviVariableName = helper.GetPartByName(PviVariableNameParameter);
            BrPviTaskName = helper.GetPartByName(PviTaskNameParameter);
            BrPviRefreshRate = helper.GetPartByName(RefreshRateParameter, (int)500);
            BrPviArrayLength = helper.GetPartByName(ArrayLengthParameter, (uint)0);
            BrPviStringType = helper.GetPartByName(StringTyteParameter, (uint)BrPviDynTagSettings.EnStringType.String);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            if (String.IsNullOrWhiteSpace(helper.GetPartByName(PviVariableNameParameter)))
                return false;
            BrPviVariableName = helper.GetPartByName(PviVariableNameParameter);

            // Optional parameters
            BrPviTaskName = helper.GetPartByName(PviTaskNameParameter);
            BrPviRefreshRate = helper.GetPartByName(RefreshRateParameter, (int)500);
            BrPviArrayLength = helper.GetPartByName(ArrayLengthParameter, (uint)0);
            BrPviStringType = helper.GetPartByName(StringTyteParameter, (uint)BrPviDynTagSettings.EnStringType.String);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PviVariableNameParameter,
                                       DynamicStringParser.CharAssign,
                                       BrPviVariableName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PviTaskNameParameter,
                                       DynamicStringParser.CharAssign,
                                       BrPviTaskName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", RefreshRateParameter,
                                       DynamicStringParser.CharAssign,
                                       BrPviRefreshRate);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ArrayLengthParameter,
                                       DynamicStringParser.CharAssign,
                                       BrPviArrayLength);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StringTyteParameter,
                                       DynamicStringParser.CharAssign,
                                       BrPviStringType);

            return dynamicstring.ToString();
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

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetTagNextDynSetting(thistagdefinition);
        }

        public string GetTagNextDynSetting(TagDefinition thistagdefinition)
        {
            string prevAddress = _BrPviVariableName;
            string fieldName = GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            _BrPviVariableName += ("." + fieldName.Replace('/', '.'));
            string dynSettings = ToString();
            _BrPviVariableName = prevAddress;
            return dynSettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (Properties.Settings.Default.MaxJobSize > ByteSize);
        }

        #endregion

        #region Properties

        private string _BrPviTaskName;
        [Category("Device Data")]
        [Description("PVI Task Name")]
        [Size(SizeAttribute.Unlimited)]
        public string BrPviTaskName
        {
            get { return _BrPviTaskName; }
            set { _BrPviTaskName = value; }
        }

        private string _BrPviVariableName;
        [Category("Device Data")]
        [Description("PVI Variable Name")]
        [Size(SizeAttribute.Unlimited)]
        public string BrPviVariableName
        {
            get { return _BrPviVariableName; }
            set { _BrPviVariableName = value; }
        }

        private int _BrPviRefreshRate;
        [Category("Device Data")]
        [Description("Refresh Rate")]
        [Size(SizeAttribute.Unlimited)]
        public int BrPviRefreshRate
        {
            get { return _BrPviRefreshRate; }
            set { _BrPviRefreshRate = value; }
        }

        private uint _BrPviArrayLength;
        [Category("Device Data")]
        [Description("Array Length")]
        [Size(SizeAttribute.Unlimited)]
        public uint BrPviArrayLength
        {
            get { return _BrPviArrayLength; }
            set { _BrPviArrayLength = value; }
        }

        private uint _BrPviStringType;
        [Category("Device Data")]
        [Description("String Type")]
        [Size(SizeAttribute.Unlimited)]
        public uint BrPviStringType
        {
            get { return _BrPviStringType; }
            set { _BrPviStringType = value; }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the VI Variable Name parameter
            if (propertyName == "BrPviVariableName")
            {
                if (String.IsNullOrWhiteSpace(BrPviVariableName))
                {
                    return Properties.Resources.ErrorInvalidPviVariableName;
                }
            }
            if (propertyName == "BrPviArrayLength")
            {
                if ((VarType == UFUAModel.DataType.String) && (BrPviArrayLength <= 0 || BrPviArrayLength> Properties.Settings.Default.MaxJobSize))
                {
                    return string.Format(Properties.Resources.ErrorInvalidLengthString, Properties.Settings.Default.MaxJobSize);
                }
            }
            return null;
        }

        #endregion
    }
}
