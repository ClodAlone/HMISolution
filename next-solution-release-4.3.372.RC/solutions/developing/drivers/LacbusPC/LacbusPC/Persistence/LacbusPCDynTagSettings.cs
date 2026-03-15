using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;
using System.Reflection;
using DevExpress.Xpo;

enum ValidationErrorCodes
{
    validationNoError,
    validationOutputTaskRequired,
    validationInputTaskRequired
}

namespace LacbusPC
{
    public sealed class LacbusPCDynTagSettings : DynTagSettings
    {
        #region Constructors

        public LacbusPCDynTagSettings()
            : base()
        {
            LacbusPCDatumNumber = 0;
            LacbusPCDatumType = DatumTypes.DigitalInput;
            LacbusPCDatumFormat = DatumFormats.Logical;
            LacbusPCDatumCategory = DatumCategories.Instantaneous;
            LacbusPCCommunicationDuration = 0;
            LacbusPCConversionMinRawValue = 0.0;
            LacbusPCConversionMaxRawValue = 0.0;
            LacbusPCConversionMinValue = 0.0;
            LacbusPCConversionMaxValue = 0.0;
        }

        #endregion

        #region Members


        #endregion

        #region Static Members

        private static readonly String DatumNumberParameter = "DN";
        private static readonly String DatumTypeParameter = "DT";
        private static readonly String DatumCategoryParameter = "DC";
        private static readonly String CommunicationDurationParameter = "CD";
        private static readonly String ConvMinRawValueParameter = "MinRV";
        private static readonly String ConvMaxRawValueParameter = "MaxRV";
        private static readonly String ConvMinValueParameter = "MinV";
        private static readonly String ConvMaxValueParameter = "MaxV";

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

            LacbusPCDatumNumber = helper.GetPartByName(DatumNumberParameter, (UInt16)0);
            LacbusPCDatumType = (DatumTypes)helper.GetPartByName(DatumTypeParameter, (UInt16)DatumTypes.DigitalInput);
            LacbusPCDatumCategory = (DatumCategories)helper.GetPartByName(DatumCategoryParameter, (UInt16)DatumCategories.Instantaneous);
            LacbusPCCommunicationDuration = helper.GetPartByName(CommunicationDurationParameter, (UInt16)0);
            LacbusPCConversionMinRawValue = 0.0;
            string valueString = helper.GetPartByName(ConvMinRawValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMinRawValue = valueDouble;
                }
            }
            LacbusPCConversionMaxRawValue = 0.0;
            valueString = helper.GetPartByName(ConvMaxRawValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMaxRawValue = valueDouble;
                }
            }
            LacbusPCConversionMinValue = 0.0;
            valueString = helper.GetPartByName(ConvMinValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMinValue = valueDouble;
                }
            }
            LacbusPCConversionMaxValue = 0.0;
            valueString = helper.GetPartByName(ConvMaxValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMaxValue = valueDouble;
                }
            }
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameter
            LacbusPCDatumType = (DatumTypes)helper.GetPartByName(DatumTypeParameter, (UInt16)DatumTypes.DigitalInput);

            // Optional parameter
            LacbusPCDatumCategory = (DatumCategories)helper.GetPartByName(DatumCategoryParameter, (UInt16)DatumCategories.Instantaneous);

            // Optional parameter
            LacbusPCDatumNumber = helper.GetPartByName(DatumNumberParameter, (UInt16)0);

            // Optional parameter
            LacbusPCCommunicationDuration = helper.GetPartByName(CommunicationDurationParameter, (UInt16)0);

            // Optional parameter
            LacbusPCConversionMinRawValue = 0.0;
            string valueString = helper.GetPartByName(ConvMinRawValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMinRawValue = valueDouble;
                }
            }

            // Optional parameter
            LacbusPCConversionMaxRawValue = 0.0;
            valueString = helper.GetPartByName(ConvMaxRawValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMaxRawValue = valueDouble;
                }
            }

            // Optional parameter
            LacbusPCConversionMinValue = 0.0;
            valueString = helper.GetPartByName(ConvMinValueParameter);
            if(!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if(double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMinValue = valueDouble;
                }
            }

            // Optional parameter
            LacbusPCConversionMaxValue = 0.0;
            valueString = helper.GetPartByName(ConvMaxValueParameter);
            if (!String.IsNullOrWhiteSpace(valueString))
            {
                double valueDouble = 0.0;
                if (double.TryParse(valueString, out valueDouble))
                {
                    LacbusPCConversionMaxValue = valueDouble;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DatumTypeParameter,
                                       DynamicStringParser.CharAssign,
                                       (int)LacbusPCDatumType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DatumNumberParameter,
                                       DynamicStringParser.CharAssign,
                                       (int)LacbusPCDatumNumber);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CommunicationDurationParameter,
                                       DynamicStringParser.CharAssign,
                                       (int)LacbusPCCommunicationDuration);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DatumCategoryParameter,
                                       DynamicStringParser.CharAssign,
                                       (int)LacbusPCDatumCategory);
            if((LacbusPCConversionMinRawValue != 0.0) ||
               (LacbusPCConversionMaxRawValue != 0.0) ||
               (LacbusPCConversionMinValue != 0.0) ||
               (LacbusPCConversionMaxValue != 0.0))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConvMinRawValueParameter,
                                           DynamicStringParser.CharAssign,
                                           LacbusPCConversionMinRawValue);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConvMaxRawValueParameter,
                                           DynamicStringParser.CharAssign,
                                           LacbusPCConversionMaxRawValue);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConvMinValueParameter,
                                           DynamicStringParser.CharAssign,
                                           LacbusPCConversionMinValue);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConvMaxValueParameter,
                                           DynamicStringParser.CharAssign,
                                           LacbusPCConversionMaxValue);
            }

            return dynamicstring.ToString();
        }

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCDatumType"));
        //    }
        //}

        private UInt16 _LacbusPCDatumNumber;
        [Category("Device Data")]
        [Description("Datum Number")]
        [Size(SizeAttribute.Unlimited)]
        public UInt16 LacbusPCDatumNumber
        {
            get { return _LacbusPCDatumNumber; }
            set { _LacbusPCDatumNumber = value; }
        }

        private DatumTypes _LacbusPCDatumType;
        [Category("Device Data")]
        [Description("Datum Type")]
        public DatumTypes LacbusPCDatumType
        {
            get { return _LacbusPCDatumType; }
            set
            {
                _LacbusPCDatumType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
            }
        }

        private UInt16 _LacbusPCCommunicationDuration;
        [Category("Device Data")]
        [Description("Communication Duration")]
        [Size(SizeAttribute.Unlimited)]
        public UInt16 LacbusPCCommunicationDuration
        {
            get { return _LacbusPCCommunicationDuration; }
            set { _LacbusPCCommunicationDuration = value; }
        }

        private DatumFormats _LacbusPCDatumFormat;
        [Category("Device Data")]
        [Description("Datum Format")]
        public DatumFormats LacbusPCDatumFormat
        {
            get { return _LacbusPCDatumFormat; }
            set { _LacbusPCDatumFormat = value; }
        }

        private DatumCategories _LacbusPCDatumCategory;
        [Category("Device Data")]
        [Description("Datum Category")]
        public DatumCategories LacbusPCDatumCategory
        {
            get { return _LacbusPCDatumCategory; }
            set { _LacbusPCDatumCategory = value; }
        }

        private double _LacbusPCConversionMinRawValue;
        [Category("Device Data")]
        [Description("Minimum Raw Value")]
        [Size(SizeAttribute.Unlimited)]
        public double LacbusPCConversionMinRawValue
        {
            get { return _LacbusPCConversionMinRawValue; }
            set
            {
                _LacbusPCConversionMinRawValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxValue"));
            }
        }

        private double _LacbusPCConversionMaxRawValue;
        [Category("Device Data")]
        [Description("Maximum Raw Value")]
        [Size(SizeAttribute.Unlimited)]
        public double LacbusPCConversionMaxRawValue
        {
            get { return _LacbusPCConversionMaxRawValue; }
            set
            {
                _LacbusPCConversionMaxRawValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxValue"));
            }
        }

        private double _LacbusPCConversionMinValue;
        [Category("Device Data")]
        [Description("Minimum Converted Value")]
        [Size(SizeAttribute.Unlimited)]
        public double LacbusPCConversionMinValue
        {
            get { return _LacbusPCConversionMinValue; }
            set
            {
                _LacbusPCConversionMinValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxValue"));
            }
        }

        private double _LacbusPCConversionMaxValue;
        [Category("Device Data")]
        [Description("Maximum Converted Value")]
        [Size(SizeAttribute.Unlimited)]
        public double LacbusPCConversionMaxValue
        {
            get { return _LacbusPCConversionMaxValue; }
            set
            {
                _LacbusPCConversionMaxValue = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMaxRawValue"));
                OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCConversionMinValue"));
            }
        }

        #endregion

        #region IDataErrorInfo Members        

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "LacbusPCDatumType")
            {
                ValidationErrorCodes errorCode = CheckTaskType();
                if (errorCode == ValidationErrorCodes.validationInputTaskRequired)
                {
                    return string.Format(Properties.Resources.ErrorInputTaskRequired, DriverCodeBase.Properties.Resources.LinkType_Input);
                }
                if (VarType == UFUAModel.DataType.String)
                {
                    return string.Format(Properties.Resources.ErrorInvalidTagType, LacbusPCDatumType, VarType);
                }
                else if (errorCode == ValidationErrorCodes.validationOutputTaskRequired)
                {
                    return string.Format(Properties.Resources.ErrorOutputTaskRequired, DriverCodeBase.Properties.Resources.LinkType_ExceptionOutput, DriverCodeBase.Properties.Resources.LinkType_UnconditionalOutput);
                }
            }
            else if (propertyName == "TagLinkType")
            {
                ValidationErrorCodes errorCode = CheckTaskType();
                if (errorCode == ValidationErrorCodes.validationInputTaskRequired)
                {
                    return string.Format(Properties.Resources.ErrorInputTaskRequired, DriverCodeBase.Properties.Resources.LinkType_Input);
                }
                else if (errorCode == ValidationErrorCodes.validationOutputTaskRequired)
                {
                    return string.Format(Properties.Resources.ErrorOutputTaskRequired, DriverCodeBase.Properties.Resources.LinkType_ExceptionOutput, DriverCodeBase.Properties.Resources.LinkType_UnconditionalOutput);
                }
            }
            else if((propertyName == "LacbusPCConversionMinRawValue") ||
                    (propertyName == "LacbusPCConversionMaxRawValue") ||
                    (propertyName == "LacbusPCConversionMinValue") ||
                    (propertyName == "LacbusPCConversionMaxValue"))
            {
                if((LacbusPCConversionMinRawValue > LacbusPCConversionMaxRawValue) ||
                   (LacbusPCConversionMinValue > LacbusPCConversionMaxValue))
                {
                    return Properties.Resources.ErrorInvalidConversionData;
                }
            }

            return null;
        }

        private ValidationErrorCodes CheckTaskType()
        {
            ValidationErrorCodes returnValue = ValidationErrorCodes.validationNoError;
            switch(LacbusPCDatumType)
            {
                case DatumTypes.DigitalInput:
                case DatumTypes.AnalogInput:
                case DatumTypes.Alarm:
                    if (TagLinkType != (int)LinkType.Input)
                    {
                        returnValue = ValidationErrorCodes.validationInputTaskRequired;
                    }
                    break;

                case DatumTypes.SetDateTime:
                case DatumTypes.ModbusCoilSetpoints:
                case DatumTypes.ModbusRegisterSetpoints:
                case DatumTypes.RTUPollRequest:
                case DatumTypes.ShutdownFR1000FrontEnd:
                    if ((TagLinkType != (int)LinkType.ExceptionOutput) &&
                        (TagLinkType != (int)LinkType.UnconditionalOutput))
                    {
                        returnValue = ValidationErrorCodes.validationOutputTaskRequired;
                    }
                    break;
            }
            return (returnValue);
        }
        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("LacbusPCDatumType"));
                    break;
        }
    }
    #endregion
}
}
