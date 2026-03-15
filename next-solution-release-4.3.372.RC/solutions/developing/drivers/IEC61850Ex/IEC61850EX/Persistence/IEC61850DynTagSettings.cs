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

namespace IEC61850
{
    public sealed class IEC61850DynTagSettings : DynTagSettings
    {
        #region Constructors

        public IEC61850DynTagSettings()
            : base()
        {
            _LogicalDeviceName = String.Empty;
            _LogicalNodeName = String.Empty;
            _FunctionalConstraint = FunctionalConstraints.None;
            _DataItemIdentifier = String.Empty;
            _MMSDataType = MMSDataTypes.Boolean;
            _DataMaximumLength = 0;
            _RetryOutputInCaseOfError = false;
            _ReportLogicalDeviceName = String.Empty;
            _ReportLogicalNodeName = String.Empty;
            _ReportName = String.Empty;
            _ReportType = ReportTypes.None;
            _InitializeData = true;
        }

        #endregion

        #region Static Members
        private static readonly String LogicalDeviceNameParameter = "DEV";
        private static readonly String LogicalNodeNameParameter = "NOD";
        private static readonly String FunctionalConstraintParameter = "FC";
        private static readonly String DataItemIdentifierParameter = "DAT";
        private static readonly String MMSDataTypeParameter = "MTY";
        private static readonly String DataMaximumLengthParameter = "LEN";
        private static readonly String RetryOutputInCaseOfErrorParameter = "RO";
        private static readonly String ReportLogicalDeviceNameParameter = "RPD";
        private static readonly String ReportLogicalNodeNameParameter = "RND";
        private static readonly String ReportNameParameter = "RPN";
        private static readonly String ReportTypeParameter = "RPT";
        private static readonly String InitializeDataParameter = "ID";

        private const char MOVICON_RESERVED_CHARS = '$';

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _LogicalDeviceName = helper.GetPartByName(LogicalDeviceNameParameter);
            _LogicalNodeName = helper.GetPartByName(LogicalNodeNameParameter);
            _FunctionalConstraint = (FunctionalConstraints)(helper.GetPartByName(FunctionalConstraintParameter, (UInt16)FunctionalConstraints.None));
            _DataItemIdentifier = helper.GetPartByName(DataItemIdentifierParameter);
            _MMSDataType = (MMSDataTypes)(helper.GetPartByName(MMSDataTypeParameter, (UInt16)MMSDataTypes.Boolean));
            _DataMaximumLength = helper.GetPartByName(DataMaximumLengthParameter, (UInt16)0);
            _RetryOutputInCaseOfError = helper.GetPartByName(RetryOutputInCaseOfErrorParameter, false);
            _ReportLogicalDeviceName = helper.GetPartByName(ReportLogicalDeviceNameParameter);
            _ReportLogicalNodeName = helper.GetPartByName(ReportLogicalNodeNameParameter);
            _ReportName = helper.GetPartByName(ReportNameParameter);
            _ReportType = (ReportTypes)(helper.GetPartByName(ReportTypeParameter, (UInt16)ReportTypes.None));
            _InitializeData = helper.GetPartByName(InitializeDataParameter, true);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameters
            if (String.IsNullOrWhiteSpace(helper.GetPartByName(LogicalDeviceNameParameter)))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(helper.GetPartByName(LogicalNodeNameParameter)))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(helper.GetPartByName(DataItemIdentifierParameter)))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(helper.GetPartByName(MMSDataTypeParameter)))
            {
                return false;
            }
            if ((String.IsNullOrWhiteSpace(helper.GetPartByName(ReportLogicalDeviceNameParameter)) != String.IsNullOrWhiteSpace(helper.GetPartByName(ReportLogicalNodeNameParameter))) ||
                (String.IsNullOrWhiteSpace(helper.GetPartByName(ReportLogicalDeviceNameParameter)) != String.IsNullOrWhiteSpace(helper.GetPartByName(ReportNameParameter))))
            {
                return false;
            }
            _LogicalDeviceName = helper.GetPartByName(LogicalDeviceNameParameter);
            _LogicalNodeName = helper.GetPartByName(LogicalNodeNameParameter);
            _DataItemIdentifier = helper.GetPartByName(DataItemIdentifierParameter);
            _MMSDataType = (MMSDataTypes)(helper.GetPartByName(MMSDataTypeParameter, (UInt16)MMSDataTypes.Boolean));

            // optional parameters
            _FunctionalConstraint = (FunctionalConstraints)(helper.GetPartByName(FunctionalConstraintParameter, (UInt16)FunctionalConstraints.None));
            _DataMaximumLength = helper.GetPartByName(DataMaximumLengthParameter, (UInt16)0);
            _RetryOutputInCaseOfError = helper.GetPartByName(RetryOutputInCaseOfErrorParameter, false);
            _ReportLogicalDeviceName = helper.GetPartByName(ReportLogicalDeviceNameParameter);
            _ReportLogicalNodeName = helper.GetPartByName(ReportLogicalNodeNameParameter);
            _ReportName = helper.GetPartByName(ReportNameParameter);
            _ReportType = (ReportTypes)(helper.GetPartByName(ReportTypeParameter, (UInt16)ReportTypes.None));
            _InitializeData = helper.GetPartByName(InitializeDataParameter, true);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", LogicalDeviceNameParameter, DynamicStringParser.CharAssign, _LogicalDeviceName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", LogicalNodeNameParameter, DynamicStringParser.CharAssign, _LogicalNodeName);
            if (_FunctionalConstraint != FunctionalConstraints.None)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", FunctionalConstraintParameter, DynamicStringParser.CharAssign, (uint)_FunctionalConstraint);
            }
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataItemIdentifierParameter, DynamicStringParser.CharAssign, _DataItemIdentifier);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", MMSDataTypeParameter, DynamicStringParser.CharAssign, (uint)_MMSDataType);
            if (_DataMaximumLength > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", DataMaximumLengthParameter, DynamicStringParser.CharAssign, _DataMaximumLength);
            }
            if (_RetryOutputInCaseOfError == true)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", RetryOutputInCaseOfErrorParameter, DynamicStringParser.CharAssign, _RetryOutputInCaseOfError);
            }
            if (_ReportType != ReportTypes.None)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ReportTypeParameter, DynamicStringParser.CharAssign, (uint)_ReportType);
            }
            if (!String.IsNullOrWhiteSpace(_ReportLogicalDeviceName) && !String.IsNullOrWhiteSpace(_ReportLogicalNodeName) && !String.IsNullOrWhiteSpace(_ReportName))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ReportLogicalDeviceNameParameter, DynamicStringParser.CharAssign, _ReportLogicalDeviceName);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ReportLogicalNodeNameParameter, DynamicStringParser.CharAssign, _ReportLogicalNodeName);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ReportNameParameter, DynamicStringParser.CharAssign, _ReportName);
            }            
            if (_InitializeData == false)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", InitializeDataParameter, DynamicStringParser.CharAssign, _InitializeData);
            }

            return dynamicstring.ToString();
        }

        // Check the dynamic address of tags to be imported
        public bool ParseAddress(string address)
        {
            if (String.IsNullOrWhiteSpace(address))
            {
                return (false);
            }

            // Count and check the number of fields of the address
            string[] addressInfo = address.Split(new char[] { '/', '$' });
            if (addressInfo.Count() < 3)
            {
                return (false);
            }

            // Parse the name of the logical device (mandatory)
            int index = address.IndexOf('/');
            if ((index <= 0) || (index >= (address.Length - 1)))
            {
                return (false);
            }
            _LogicalDeviceName = address.Substring(0, index);
            if (String.IsNullOrWhiteSpace(_LogicalDeviceName))
            {
                return (false);
            }

            // Parse the Logical Node Name (mandatory)
            string auxString = address.Substring(index + 1);
            if (String.IsNullOrWhiteSpace(auxString))
            {
                return (false);
            }
            index = auxString.IndexOf('$');
            if ((index <= 0) || (index >= (auxString.Length - 1)))
            {
                return (false);
            }
            _LogicalNodeName = auxString.Substring(0, index);

            // Parse the Functional Constraint (optional)
            _FunctionalConstraint = FunctionalConstraints.None;
            string auxString2 = auxString.Substring(index + 1);
            if (String.IsNullOrWhiteSpace(auxString2))
            {
                return (false);
            }
            index = auxString2.IndexOf('$');
            if ((index > 0) && (index < (auxString2.Length - 1)))
            {
                string fcString = auxString2.Substring(0, index);
                _FunctionalConstraint = GetFunctionalConstraintFromString(fcString);
                auxString = auxString2.Substring(index + 1);
                if (String.IsNullOrWhiteSpace(auxString))
                {
                    return (false);
                }
            }
            else
            {
                auxString = auxString2;
            }

            //  Set the data attribute string
            _DataItemIdentifier = auxString.Replace('$', '.');

            return true;
        }

        FunctionalConstraints GetFunctionalConstraintFromString(string fcString)
        {
            if (string.Compare(fcString, "ST", true) == 0)
            {
                return (FunctionalConstraints.ST);
            }
            else if (string.Compare(fcString, "MX", true) == 0)
            {
                return (FunctionalConstraints.MX);
            }
            else if (string.Compare(fcString, "SG", true) == 0)
            {
                return (FunctionalConstraints.SG);
            }
            else if (string.Compare(fcString, "CO", true) == 0)
            {
                return (FunctionalConstraints.CO);
            }
            else if (string.Compare(fcString, "SP", true) == 0)
            {
                return (FunctionalConstraints.SP);
            }
            else if (string.Compare(fcString, "SV", true) == 0)
            {
                return (FunctionalConstraints.SV);
            }
            else if (string.Compare(fcString, "CF", true) == 0)
            {
                return (FunctionalConstraints.CF);
            }
            else if (string.Compare(fcString, "DC", true) == 0)
            {
                return (FunctionalConstraints.DC);
            }
            else if (string.Compare(fcString, "RP", true) == 0)
            {
                return (FunctionalConstraints.RP);
            }
            else if (string.Compare(fcString, "BR", true) == 0)
            {
                return (FunctionalConstraints.BR);
            }

            return (FunctionalConstraints.None);
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            switch (_MMSDataType)
            {
                case MMSDataTypes.Boolean:
                    return (UFUAModel.DataType.Boolean);

                case MMSDataTypes.Integer8Bits:
                    return (UFUAModel.DataType.SByte);

                case MMSDataTypes.UnsignedInteger8Bits:
                    return (UFUAModel.DataType.Byte);

                case MMSDataTypes.Integer16Bits:
                    return (UFUAModel.DataType.Int16);

                case MMSDataTypes.UnsignedInteger16Bits:
                    return (UFUAModel.DataType.UInt16);

                case MMSDataTypes.Integer32Bits:
                    return (UFUAModel.DataType.Int32);

                case MMSDataTypes.UnsignedInteger32Bits:
                    return (UFUAModel.DataType.UInt32);

                case MMSDataTypes.FloatingPoint32Bits:
                    return (UFUAModel.DataType.Float);

                case MMSDataTypes.FloatingPoint64Bits:
                    return (UFUAModel.DataType.Double);

                case MMSDataTypes.BitString:
                    if (_DataMaximumLength <= 8)
                    {
                        return (UFUAModel.DataType.Byte);
                    }
                    else if (_DataMaximumLength <= 16)
                    {
                        return (UFUAModel.DataType.UInt16);
                    }
                    else if (_DataMaximumLength <= 32)
                    {
                        return (UFUAModel.DataType.UInt32);
                    }
                    else
                    {
                        return (UFUAModel.DataType.String);
                    }

                case MMSDataTypes.OctetString:
                case MMSDataTypes.VisibleString:
                case MMSDataTypes.MMSString:
                case MMSDataTypes.BinaryTime:
                case MMSDataTypes.UTCTime:
                    return (UFUAModel.DataType.String);

                    //case MMSDataTypes.Structure:
                    //    return (UFUAModel.DataType.Boolean);
            }

            return (UFUAModel.DataType.Boolean);
        }
        #endregion

        #region Methods

        public static bool IsMMSDataStringType(MMSDataTypes mmsDataType) 
        {
            return (mmsDataType == MMSDataTypes.VisibleString || mmsDataType == MMSDataTypes.OctetString || mmsDataType == MMSDataTypes.MMSString || mmsDataType == MMSDataTypes.BitString);
        }

        public bool IsMMSDataStringType()
        {
            return (IsMMSDataStringType(_MMSDataType));
        }

        private bool TextContainMoviconReservedChars(string text) {
            return (text.IndexOf(MOVICON_RESERVED_CHARS) > 0);
        }

        private string GetListInvalidChacarters()
        {
            return MOVICON_RESERVED_CHARS.ToString();
        }
        #endregion

        #region Properties

        /// <summary> Name of the logical device. </summary>
        private string _LogicalDeviceName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Logical Device Name" Property. </summary>
        ///
        /// <value> The name of the logical device. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Logical Device Name")]
        public string LogicalDeviceName
        {
            get { return _LogicalDeviceName; }
            set { _LogicalDeviceName = value; }
        }

        /// <summary> Name of the logical node. </summary>
        private string _LogicalNodeName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Logical Node Name" Property. </summary>
        ///
        /// <value> The name of the logical node. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Logical Node Name")]
        public string LogicalNodeName
        {
            get { return _LogicalNodeName; }
            set { _LogicalNodeName = value; }
        }

        /// <summary> Functional constraint of the data item. </summary>
        private FunctionalConstraints _FunctionalConstraint;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Functional Constraint" Property. </summary>
        ///
        /// <value> The functional constraint of the data item. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Functional Constraint")]
        public FunctionalConstraints FunctionalConstraint
        {
            get { return _FunctionalConstraint; }
            set
            {
                _FunctionalConstraint = value;
                OnPropertyChanged("ReportType");
            }
        }

        /// <summary> Identifier of the data item. </summary>
        private string _DataItemIdentifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "Data Item Identifier" Property. </summary>
        ///
        /// <value> The identifier of the data item. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Item Identifier")]
        public string DataItemIdentifier
        {
            get { return _DataItemIdentifier; }
            set { _DataItemIdentifier = value; }
        }

        /// <summary> MMS data type. </summary>
        private MMSDataTypes _MMSDataType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "MM Data Type" Property. </summary>
        ///
        /// <value> The MMS Data Type of the data item. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("MMS Data Type")]
        public MMSDataTypes MMSDataType
        {
            get { return _MMSDataType; }
            set
            {
                _MMSDataType = value;
                OnPropertyChanged("DataMaximumLength");
            }
        }

        /// <summary> The data maximum length. </summary>
        private uint _DataMaximumLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "Data Maximum Length" Property. </summary>
        ///
        /// <value> The data maximum length. Used for data of type BitString, OctetString, VisibleString, MMSString </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Maximum Length")]
        public uint DataMaximumLength
        {
            get { return _DataMaximumLength; }
            set { _DataMaximumLength = value; }
        }

        /// <summary> Retry output in case of error. </summary>
        private bool _RetryOutputInCaseOfError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "Retry Output In Case Of Error" Property. </summary>
        ///
        /// <value> Retry Output In Case Of Error </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Retry Output In Case Of Error")]
        public bool RetryOutputInCaseOfError
        {
            get { return _RetryOutputInCaseOfError; }
            set { _RetryOutputInCaseOfError = value; }
        }

        /// <summary> Name of the logical device of the report. </summary>
        private string _ReportLogicalDeviceName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Report Logical Device Name" Property. </summary>
        ///
        /// <value> The name of the logical device of the report. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Report Logical Device Name")]
        public string ReportLogicalDeviceName
        {
            get { return _ReportLogicalDeviceName; }
            set {
                _ReportLogicalDeviceName = value;
                OnPropertyChanged("ReportLogicalNodeName");
                OnPropertyChanged("ReportName");
            }
        }

        /// <summary> Name of the logical node of the report. </summary>
        private string _ReportLogicalNodeName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Report Logical Node Name" Property. </summary>
        ///
        /// <value> The name of the logical node of the report. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Report Logical Node Name")]
        public string ReportLogicalNodeName
        {
            get { return _ReportLogicalNodeName; }
            set {
                _ReportLogicalNodeName = value;                
                OnPropertyChanged("ReportLogicalDeviceName");
                OnPropertyChanged("ReportName");
            }
        }

        /// <summary> Name of the logical node of the report. </summary>
        private string _ReportName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> IEC 61850 "Report Name" Property. </summary>
        ///
        /// <value> The name of the report. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Report Name")]
        public string ReportName
        {
            get { return _ReportName; }
            set {
                _ReportName = value;
                OnPropertyChanged("ReportLogicalNodeName");
                OnPropertyChanged("ReportLogicalDeviceName");
            }
        }

        /// <summary> Report type. </summary>
        private ReportTypes _ReportType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "Report Type" Property. </summary>
        ///
        /// <value> The Type of the Report. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Report Type")]
        public ReportTypes ReportType
        {
            get { return _ReportType; }
            set
            {
                _ReportType = value;
                OnPropertyChanged("FunctionalConstraint");
            }
        }

        /// <summary> Initialize Data. </summary>
        private bool _InitializeData;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> "Initialize Data" Property. </summary>
        ///
        /// <value> Initialize Data </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Initialize Data")]
        public bool InitializeData
        {
            get { return _InitializeData; }
            set { _InitializeData = value; }
        }

        /// <summary>   Type of the tag link. </summary>
        private int _TagLinkType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Modes to access the tags. Override base property for simultaneous validation of
        /// "BacnetObjectType" property.
        /// </summary>
        ///
        /// <value> The type of the tag link. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("General")]
        //[Description("Link Type")]
        //public override int TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged("MMSDataType");
        //        OnPropertyChanged("FunctionalConstraint");
        //    }
        //}

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
            {
                return sBase;
            }

            switch (propertyName) {
                case "MMSDataType":
                    break;
                case "LogicalDeviceName":
                    if (TextContainMoviconReservedChars(_LogicalDeviceName))
                        return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                    if (String.IsNullOrWhiteSpace(_LogicalDeviceName))
                        return Properties.Resources.IEC61850InvalidLogicalDeviceName;
                    break;
                case "LogicalNodeName":
                    if (TextContainMoviconReservedChars(_LogicalNodeName))
                        return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                    if (String.IsNullOrWhiteSpace(_LogicalDeviceName))
                        return Properties.Resources.IEC61850InvalidLogicalNodeName;
                    break;
                case "DataItemIdentifier":
                    if (TextContainMoviconReservedChars(_DataItemIdentifier))
                        return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                    if (String.IsNullOrWhiteSpace(_DataItemIdentifier))
                        return Properties.Resources.IEC61850InvalidLogicalNodeName;
                    break;
                case "FunctionalConstraint":
                    switch (_FunctionalConstraint)
                    {
                        case FunctionalConstraints.None:
                            break;
                        case FunctionalConstraints.ST:
                        case FunctionalConstraints.MX:
                        case FunctionalConstraints.SG:
                            if ((LinkType)TagLinkType != LinkType.Input)
                                return string.Format(Properties.Resources.IEC61850FCInconsistentWithLinkType, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                            break;
                        case FunctionalConstraints.CO:
                        case FunctionalConstraints.SP:
                        case FunctionalConstraints.SV:
                        case FunctionalConstraints.CF:
                        case FunctionalConstraints.DC:
                            //break;
                        case FunctionalConstraints.RP:
                            //// if report was setted
                            //if (!String.IsNullOrWhiteSpace(_ReportLogicalDeviceName) && !String.IsNullOrWhiteSpace(_ReportLogicalNodeName) && !String.IsNullOrWhiteSpace(_ReportName)) {
                            //    if (_ReportType != ReportTypes.Unbuffered)
                            //        return Properties.Resources.IEC61850InvalidFunctionalConstraintAssociatedToReportType;
                            //}
                            //break;
                        case FunctionalConstraints.BR:
                            // if report was setted
                            //if (!String.IsNullOrWhiteSpace(_ReportLogicalDeviceName) && !String.IsNullOrWhiteSpace(_ReportLogicalNodeName) && !String.IsNullOrWhiteSpace(_ReportName)) {
                            //    if (_ReportType != ReportTypes.Buffered)
                            //        return Properties.Resources.IEC61850InvalidFunctionalConstraintAssociatedToReportType;
                            //}
                            break;
                        default:
                            return Properties.Resources.IEC61850InvalidFunctionalConstraint;
                    }
                    break;                
                case "ReportLogicalDeviceName":
                case "ReportLogicalNodeName":
                case "ReportName":
                    if (_ReportType != ReportTypes.None)
                    {
                        if (TextContainMoviconReservedChars(_ReportLogicalDeviceName))
                            return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                        if (TextContainMoviconReservedChars(_ReportLogicalNodeName))
                            return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                        if (TextContainMoviconReservedChars(_ReportName))
                            return string.Format(Properties.Resources.IEC61850ParamaterContainInvalidCharacter, GetListInvalidChacarters());
                        if ((String.IsNullOrWhiteSpace(_ReportLogicalDeviceName) || String.IsNullOrWhiteSpace(_ReportLogicalNodeName)) || String.IsNullOrWhiteSpace(_ReportName))
                            return Properties.Resources.IEC61850InconsistentReportSettings;
                    }
                    break;
                case "DataMaximumLength":
                    if (IsMMSDataStringType() || _MMSDataType == MMSDataTypes.UTCTime || _MMSDataType == MMSDataTypes.BinaryTime)
                    {
                        if (DataMaximumLength <=0 || DataMaximumLength > IEC61850Protocol.MAX_STRING_LENGTH)
                            return string.Format(Properties.Resources.IEC61850InvalidDataMaximumLength, IEC61850Protocol.MAX_STRING_LENGTH);
                    }
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
                case "TagLinkType":
                    OnPropertyChanged("MMSDataType");
                    OnPropertyChanged("FunctionalConstraint");
                    break;
            }
        }
        #endregion
    }
}
