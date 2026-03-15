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

namespace SQLDriver
{
    public sealed class SQLDriverDynTagSettings : DynTagSettings
    {

        #region Constructors

        public SQLDriverDynTagSettings()
            : base()
        {
            TagLinkType = (int)LinkType.InputOutput;
            TagName = string.Empty;
            SQLDriverColumn = string.Empty;
        }

        #endregion

        #region Static Members

        private static readonly String TagNameParameter = "TN";
        private static readonly String SQLDriverColumnParameter = "CL";

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
            if(dynamicSettings.Contains(SQLDriverColumnParameter))
            SQLDriverColumn = helper.GetPartByName(SQLDriverColumnParameter);
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
            if (dynamicSettings.Contains(SQLDriverColumnParameter))
                SQLDriverColumn = helper.GetPartByName(SQLDriverColumnParameter);
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
            if(!String.IsNullOrEmpty(SQLDriverColumn))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SQLDriverColumnParameter,
                               DynamicStringParser.CharAssign,
                               SQLDriverColumn);
            }
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

        private string _SQLDriverColumn;
        [Category("Device Data")]
        [Description("Tag Name")]
        [Size(SizeAttribute.Unlimited)]
        public string SQLDriverColumn
        {
            get { return _SQLDriverColumn; }
            set { _SQLDriverColumn = value; }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            //Check the "ColumnNameTagName" parameter
            if (propertyName == "TagName")
            {
                if (String.IsNullOrWhiteSpace(TagName))
                {
                    return (Properties.Resources.SQLDriverErrorInvalidTagName);
                }
            }
            if (propertyName == "VarType")
            {
                if (VarType != UFUAModel.DataType.Boolean && VarType != UFUAModel.DataType.SByte && VarType != UFUAModel.DataType.Byte && VarType != UFUAModel.DataType.Int16 && VarType != UFUAModel.DataType.UInt16 && VarType != UFUAModel.DataType.Int32 && VarType != UFUAModel.DataType.UInt32 && VarType != UFUAModel.DataType.Int64 && VarType != UFUAModel.DataType.UInt64 && VarType != UFUAModel.DataType.Float && VarType != UFUAModel.DataType.Double && VarType != UFUAModel.DataType.String)
                    return (Properties.Resources.PrototypeNotAllowed);
            }

            if (propertyName == "ArrayDimension")
            {
                if (ArrayDimension  < 0)
                {
                    return (UFUAModel.Properties.Resources.ErroorProtocolSetIncompatibleWithArrays);
                }
            }

            return null;
        }

        #endregion        
    }
}
