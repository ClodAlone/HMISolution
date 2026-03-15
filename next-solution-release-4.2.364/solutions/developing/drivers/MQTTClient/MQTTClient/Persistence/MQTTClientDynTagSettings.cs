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
using System.Globalization;

namespace MQTTClient
{
    public sealed class MQTTClientDynTagSettings : DynTagSettings
    {
        #region Constructors

        public MQTTClientDynTagSettings()
            : base()
        {
            TagName = String.Empty;
            JsonMessageFormat = String.Empty;
            JsonMessageTimestampField = String.Empty;
            JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            JsonUseLocalTime = false;
            _HysteresisThreshold = 0.0;
        }

        #endregion

        #region Static Members

        private static readonly String TagNameParameter = "TN";
        private static readonly String retainedParameter = "RT";
        private static readonly String qosLevelParameter = "QOS";
        private static readonly String jsonMessageFormatParameter = "JMF";
        private static readonly String jsonMessageTimestampFieldParameter = "JMT";
        private static readonly String jsonTimestampFormatParameter = "JTF";
        private static readonly String jsonUseLocalTimeParameter = "JULT";
        private static readonly String hysteresisParameter = "HYS";

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
            Retained = helper.GetPartByName(retainedParameter, true);
            QualityOfServiceLevel = (QualityOfServiceLevels)helper.GetPartByName(qosLevelParameter, (UInt16)QualityOfServiceLevels.AtMostOnce_0);
            JsonMessageFormat = helper.GetPartByName(jsonMessageFormatParameter);
            JsonMessageTimestampField = helper.GetPartByName(jsonMessageTimestampFieldParameter);
            JsonTimestampFormat = (JSonTimestampFormats)helper.GetPartByName(jsonTimestampFormatParameter, (UInt16)JSonTimestampFormats.tf_ISO);
            JsonUseLocalTime = helper.GetPartByName(jsonUseLocalTimeParameter, false);
            // Hysteresis
            string hysteresisAuxString = helper.GetPartByName(hysteresisParameter);
            double hysteresisValue = 0.0;
            try
            {
                if (String.IsNullOrWhiteSpace(hysteresisAuxString) || !double.TryParse(hysteresisAuxString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out hysteresisValue))
                {
                    HysteresisThreshold = 0.0;
                }
                else
                {
                    HysteresisThreshold = hysteresisValue;
                }
            }
            catch (Exception e)
            {
                HysteresisThreshold = 0.0;
            }
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

            // optional parameters
            Retained = helper.GetPartByName(retainedParameter, true);
            QualityOfServiceLevel = (QualityOfServiceLevels)helper.GetPartByName(qosLevelParameter, (UInt16)QualityOfServiceLevels.AtMostOnce_0);
            JsonMessageFormat = helper.GetPartByName(jsonMessageFormatParameter);
            JsonMessageTimestampField = helper.GetPartByName(jsonMessageTimestampFieldParameter);
            JsonTimestampFormat = (JSonTimestampFormats)helper.GetPartByName(jsonTimestampFormatParameter, (UInt16)JSonTimestampFormats.tf_ISO);
            JsonUseLocalTime = helper.GetPartByName(jsonUseLocalTimeParameter, false);
            // Hysteresis
            string hysteresisAuxString = helper.GetPartByName(hysteresisParameter);
            double hysteresisValue = 0.0;
            try
            {
                if (String.IsNullOrWhiteSpace(hysteresisAuxString) || !double.TryParse(hysteresisAuxString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out hysteresisValue))
                {
                    HysteresisThreshold = 0.0;
                }
                else
                {
                    HysteresisThreshold = hysteresisValue;
                }
            }
            catch (Exception e)
            {
                HysteresisThreshold = 0.0;
            }

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", TagNameParameter,
                                       DynamicStringParser.CharAssign,
                                       TagName);
            if(Retained == false)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", retainedParameter,
                                           DynamicStringParser.CharAssign,
                                           Retained);
            }

            if (QualityOfServiceLevel != QualityOfServiceLevels.AtMostOnce_0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", qosLevelParameter,
                                           DynamicStringParser.CharAssign,
                                           (byte)QualityOfServiceLevel);
            }

            if(!String.IsNullOrWhiteSpace(JsonMessageFormat))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", jsonMessageFormatParameter,
                                           DynamicStringParser.CharAssign,
                                           JsonMessageFormat);
            }

            if (!String.IsNullOrWhiteSpace(JsonMessageTimestampField))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", jsonMessageTimestampFieldParameter,
                                           DynamicStringParser.CharAssign,
                                           JsonMessageTimestampField);
            }

            if (JsonTimestampFormat != JSonTimestampFormats.tf_ISO)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", jsonTimestampFormatParameter,
                                           DynamicStringParser.CharAssign,
                                           (byte)JsonTimestampFormat);
            }

            if (JsonUseLocalTime != false)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", jsonUseLocalTimeParameter,
                                           DynamicStringParser.CharAssign,
                                           JsonUseLocalTime);
            }

            if (HysteresisThreshold > 0.0)
            {
                string auxString = HysteresisThreshold.ToString("F", CultureInfo.InvariantCulture);

                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", hysteresisParameter,
                                           DynamicStringParser.CharAssign,
                                           auxString);
            }

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

        public string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition, MQTTClientMessageFormats messageFormat)
        {
            if (!TryParse(tag.TagNode.DynamicSettings))
            {
                return tag.TagNode.DynamicSettings;
            }
            else
            {
                return GetTagNextDynSetting(thistagdefinition, messageFormat);
            }
        }

        public string GetTagNextDynSetting(TagDefinition thistagdefinition, MQTTClientMessageFormats messageFormat)
        {
            if(messageFormat == MQTTClientMessageFormats.XML)
            {
                string prevAddress = _TagName;
                string fieldName = GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
                _TagName += ("." + fieldName.Replace('/', '.'));
                string dynSettings = ToString();
                _TagName = prevAddress;
                return dynSettings;
            }
            else if (messageFormat == MQTTClientMessageFormats.JSON)
            {
                string prevFormat = _JsonMessageFormat;
                string fieldName = GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
                _JsonMessageFormat += ("." + fieldName.Replace('/', '.'));
                string dynSettings = ToString();
                _JsonMessageFormat = prevFormat;
                return dynSettings;
            }
            else
            {
                // Do nothing (RAW format cannot be applied to structures)
                string dynSettings = ToString();
                return dynSettings;
            }
        }

        public string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition, MQTTClientMessageFormats messageFormat)
        {
            return GetTagNextDynSetting(thistagdefinition, messageFormat);
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

        private bool _Retained;
        [Category("Device Data")]
        [Description("Retain flag")]
        [Size(SizeAttribute.Unlimited)]
        public bool Retained
        {
            get { return _Retained; }
            set { _Retained = value; }
        }

        private QualityOfServiceLevels _QualityOfServiceLevel;
        [Category("Device Data")]
        [Description("Quality Of Service level ")]
        [Size(SizeAttribute.Unlimited)]
        public QualityOfServiceLevels QualityOfServiceLevel
        {
            get { return _QualityOfServiceLevel; }
            set { _QualityOfServiceLevel = value; }
        }

        private string _JsonMessageFormat;
        [Category("Device Data")]
        [Description("Json Message Format")]
        [Size(SizeAttribute.Unlimited)]
        public string JsonMessageFormat
        {
            get { return _JsonMessageFormat; }
            set { _JsonMessageFormat = value; }
        }

        private string _JsonMessageTimestampField;
        [Category("Device Data")]
        [Description("Json Message Timestamp Field")]
        [Size(SizeAttribute.Unlimited)]
        public string JsonMessageTimestampField
        {
            get { return _JsonMessageTimestampField; }
            set { _JsonMessageTimestampField = value; }
        }

        private JSonTimestampFormats _JsonTimestampFormat;
        [Category("Device Data")]
        [Description("Timestamp Format")]
        [Size(SizeAttribute.Unlimited)]
        public JSonTimestampFormats JsonTimestampFormat
        {
            get { return _JsonTimestampFormat; }
            set { _JsonTimestampFormat = value; }
        }

        private bool _JsonUseLocalTime;
        [Category("Device Data")]
        [Description("Flag Use Local Time in Json messages")]
        [Size(SizeAttribute.Unlimited)]
        public bool JsonUseLocalTime
        {
            get { return _JsonUseLocalTime; }
            set { _JsonUseLocalTime = value; }
        }

        private double _HysteresisThreshold;
        [Category("Device Data")]
        [Description("Hysteresis Threshold")]
        [Size(SizeAttribute.Unlimited)]
        public double HysteresisThreshold
        {
            get { return _HysteresisThreshold; }
            set { _HysteresisThreshold = value; }
        }

        private Dictionary<string, MQTTClientStationSettings> _stationSettingList;
        public Dictionary<string, MQTTClientStationSettings> stationSettingList
        {
            get { return _stationSettingList; }
            set
            {
                _stationSettingList = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the "Tag Name" parameter
            if(propertyName == "TagName")
            {
                if(String.IsNullOrWhiteSpace(TagName))
                {
                    return (Properties.Resources.MQTTClientErrorInvalidTagName);
                }
            }
            // Check the "Hysteresis Threshold" parameter
            else if (propertyName == "HysteresisThreshold ")
            {
                if (HysteresisThreshold < 0.0)
                {
                    return (Properties.Resources.MQTTClientErrorInvalidHysteresis);
                }
            }
            else if (propertyName == "StationName")
            {
                if (InvalidVarType())
                {
                    return (Properties.Resources.MQTTClientInvalidVarTypeForMessageFormat);
                }
            }

            return null;
        }

        bool InvalidVarType()
        {
            if (stationSettingList == null || !stationSettingList.ContainsKey(StationName))
            {
                return false;
            }
            // If the message format selected in the station is "RAW", the variable must be a string
            return ((VarType != UFUAModel.DataType.String) && (stationSettingList[StationName].MQTTClientMessageFormat == MQTTClientMessageFormats.RAW));
        }

        #endregion

    }
}
