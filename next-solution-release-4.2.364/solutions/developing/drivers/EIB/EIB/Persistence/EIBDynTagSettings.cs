using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;

namespace EIB
{
    public sealed class EIBDynTagSettings : DynTagSettings
    {
        #region Constructors

        public EIBDynTagSettings()
            : base()
        {
            EnablePolling = false;
            EnableOnlyInitialPolling = true;
            RetryInitialPolling = false;
            PollingOnlyOnRequest = false;
            OutputOnlyOnRequest = false;
            AutoResetNewDataTime = 0;
            RetryOutput = false;
            InputGroups = String.Empty;
            PollingGroup = String.Empty;
            OutputGroup = String.Empty;
            DataFormat = (int)EISDATAFORMAT.EISDFBit;
            PollingTime = 0;
        }
        #endregion

        #region Static Members
        private static readonly String EnablePollingParameter = "EP";
        private static readonly String EnableOnlyInitialPollingParameter = "EOIP";
        private static readonly String RetryInitialPollingParameter = "RSP";
        private static readonly String PollingOnlyOnRequestParameter = "POR";
        private static readonly String OutputOnlyOnRequestParameter = "OOR";
        private static readonly String AutoResetNewDataTimeParameter = "ART";
        private static readonly String RetryOutputParameter = "RO";
        private static readonly String InputGroupsParameter = "IG";
        private static readonly String PollingGroupParameter = "PG";
        private static readonly String OutputGroupParameter = "OG";
        private static readonly String DataFormatParameter = "DF";
        private static readonly String PollingTimeParameter = "PT";
        #endregion

        #region Override Functions
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            EnablePolling = helper.GetPartByName(EnablePollingParameter, false);
            EnableOnlyInitialPolling = helper.GetPartByName(EnableOnlyInitialPollingParameter, true);
            RetryInitialPolling = helper.GetPartByName(RetryInitialPollingParameter, false);
            PollingTime = helper.GetPartByName(PollingTimeParameter, (UInt32)0);
            PollingOnlyOnRequest = helper.GetPartByName(PollingOnlyOnRequestParameter, false);
            OutputOnlyOnRequest = helper.GetPartByName(OutputOnlyOnRequestParameter, false);
            AutoResetNewDataTime = helper.GetPartByName(AutoResetNewDataTimeParameter, (UInt32)0);
            RetryOutput = helper.GetPartByName(RetryOutputParameter, false);
            InputGroups = helper.GetPartByName(InputGroupsParameter);
            PollingGroup = helper.GetPartByName(PollingGroupParameter);
            OutputGroup = helper.GetPartByName(OutputGroupParameter);
            DataFormat = (int)(EISDATAFORMAT)(helper.GetPartByName(DataFormatParameter, (byte)EISDATAFORMAT.EISDFBit));
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameters
            if (String.IsNullOrEmpty(helper.GetPartByName(DataFormatParameter)))
            {
                return false;
            }

            EnablePolling = helper.GetPartByName(EnablePollingParameter, false);
            EnableOnlyInitialPolling = helper.GetPartByName(EnableOnlyInitialPollingParameter, true);
            RetryInitialPolling = helper.GetPartByName(RetryInitialPollingParameter, false);
            PollingTime = helper.GetPartByName(PollingTimeParameter, (UInt32)0);
            PollingOnlyOnRequest = helper.GetPartByName(PollingOnlyOnRequestParameter, false);
            OutputOnlyOnRequest = helper.GetPartByName(OutputOnlyOnRequestParameter, false);
            AutoResetNewDataTime = helper.GetPartByName(AutoResetNewDataTimeParameter, (UInt32)0);
            RetryOutput = helper.GetPartByName(RetryOutputParameter, false);
            InputGroups = helper.GetPartByName(InputGroupsParameter);
            PollingGroup = helper.GetPartByName(PollingGroupParameter);
            OutputGroup = helper.GetPartByName(OutputGroupParameter);
            DataFormat = (int)(EISDATAFORMAT)(helper.GetPartByName(DataFormatParameter, (byte)EISDATAFORMAT.EISDFBit));

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", EnablePollingParameter, DynamicStringParser.CharAssign, EnablePolling);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", EnableOnlyInitialPollingParameter, DynamicStringParser.CharAssign, EnableOnlyInitialPolling);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PollingTimeParameter, DynamicStringParser.CharAssign, PollingTime);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", RetryInitialPollingParameter, DynamicStringParser.CharAssign, RetryInitialPolling);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PollingOnlyOnRequestParameter, DynamicStringParser.CharAssign, PollingOnlyOnRequest);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", OutputOnlyOnRequestParameter, DynamicStringParser.CharAssign, OutputOnlyOnRequest);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AutoResetNewDataTimeParameter, DynamicStringParser.CharAssign, AutoResetNewDataTime);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", RetryOutputParameter, DynamicStringParser.CharAssign, RetryOutput);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", InputGroupsParameter, DynamicStringParser.CharAssign, InputGroups);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PollingGroupParameter, DynamicStringParser.CharAssign, PollingGroup);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", OutputGroupParameter, DynamicStringParser.CharAssign, OutputGroup);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataFormatParameter, DynamicStringParser.CharAssign, (byte)DataFormat);
            return dynamicstring.ToString();
        }
        #endregion

        #region Properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   modes to access the tags property. </summary>
        ///
        /// <value> The type of the tag link. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("General")]
        //[Description("Link Type")]
        //public override int/*LinkType*/ TagLinkType
        //{
        //    get
        //    {
        //        return base.TagLinkType;
        //    }
        //    set
        //    {
        //        base.TagLinkType = value;
        //        OnPropertyChanged("PollingGroup");
        //        OnPropertyChanged("InputGroups");
        //        OnPropertyChanged("OutputGroup");
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Conditional Variable Name. </summary>
        ///
        /// <value> The name of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Conditional Variable Name")]
        public override string ConditionalVariableName
        {
            get { return base.ConditionalVariableName; }
            set
            {
                base.ConditionalVariableName = value;

                OnPropertyChanged("PollingOnlyOnRequest");
                OnPropertyChanged("OutputOnlyOnRequest");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Conditional Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Conditional Variable Node Id")]
        public override string ConditionalVariableId
        {
            get { return base.ConditionalVariableId; }
            set
            {
                base.ConditionalVariableId = value;

                OnPropertyChanged("PollingOnlyOnRequest");
                OnPropertyChanged("OutputOnlyOnRequest");
            }
        }

        private bool _EnablePolling;
         [Category("Device Data")]
         [Description("Enable Polling")]
         public bool EnablePolling
         {
             get
             {
                 return _EnablePolling;
             }

             set
             {
                 _EnablePolling = value;
                OnPropertyChanged("PollingGroup");
            }
        }

         private bool _EnableOnlyInitialPolling;
         [Category("Device Data")]
         [Description("Enable Only Initial Polling")]
         public bool EnableOnlyInitialPolling
         {
             get
             {
                 return _EnableOnlyInitialPolling;
             }

             set
             {
                 _EnableOnlyInitialPolling = value;
             }
         }

         private bool _RetryInitialPolling;
         [Category("Device Data")]
         [Description("Retry Initial Polling in case of error")]
         public bool RetryInitialPolling
         {
             get
             {
                 return _RetryInitialPolling;
             }

             set
             {
                 _RetryInitialPolling = value;

                OnPropertyChanged("PollingGroup");
            }
        }

         private bool _PollingOnlyOnRequest;
         [Category("Device Data")]
         [Description("Output Only On Request")]
         public bool PollingOnlyOnRequest
         {
             get
             {
                 return _PollingOnlyOnRequest;
             }

             set
             {
                 _PollingOnlyOnRequest = value;

                OnPropertyChanged("ConditionalVariableId");
                OnPropertyChanged("ConditionalVariableName");
            }
        }

         private bool _OutputOnlyOnRequest;
         [Category("Device Data")]
         [Description("Output Only On Request")]
         public bool OutputOnlyOnRequest
         {
             get
             {
                 return _OutputOnlyOnRequest;
             }

             set
             {
                 _OutputOnlyOnRequest = value;

                OnPropertyChanged("ConditionalVariableId");
                OnPropertyChanged("ConditionalVariableName");
            }
        }

         private uint _AutoResetNewDataTime;
         [Category("Device Data")]
         [Description("Auto Reset New Data Time")]
         public uint AutoResetNewDataTime
         {
             get
             {
                 return _AutoResetNewDataTime;
             }

             set
             {
                 _AutoResetNewDataTime = value;
             }
         }

         private bool _RetryOutput;
         [Category("Device Data")]
         [Description("Retry Output in case of error")]
         public bool RetryOutput
         {
             get
             {
                 return _RetryOutput;
             }

             set
             {
                 _RetryOutput = value;
                OnPropertyChanged("OutputGroup");
            }
        }

         private string _InputGroups;
         [Category("Device Data")]
         [Description("Input group list")]
         public string InputGroups
         {
             get
             {
                 return _InputGroups;
             }

             set
             {
                 _InputGroups = value;
                OnPropertyChanged("TagLinkType");
            }
        }

         private string _PollingGroup;
         [Category("Device Data")]
         [Description("Polling Group")]
         public string PollingGroup
         {
             get
             {
                 return _PollingGroup;
             }

             set
             {
                 _PollingGroup = value;
                OnPropertyChanged("EnablePolling");
                OnPropertyChanged("RetryInitialPolling");
                OnPropertyChanged("TagLinkType");
            }
        }

         private string _OutputGroup;
         [Category("Device Data")]
         [Description("Output Group")]
         public string OutputGroup
         {
             get
             {
                 return _OutputGroup;
             }

             set
             {
                 _OutputGroup = value;
                OnPropertyChanged("RetryOutput");
                OnPropertyChanged("TagLinkType");
            }
        }

        private EISDATAFORMAT _NewDataFormat = EISDATAFORMAT.EISDFDWord;
        [Category("Device Data")]
        [Description("New Data Format")]
        public EISDATAFORMAT NewDataFormat
        {
            get
            {
                return _NewDataFormat;
            }

            set
            {
                _NewDataFormat = value;
            }
        }

        private /*EISDATAFORMAT*/int _DataFormat;
         [Category("Device Data")]
         [Description("Data Format")]
         public /*EISDATAFORMAT*/int DataFormat
         {
             get
             {
                 return _DataFormat;
             }

             set
             {
                 _DataFormat = value;
             }
         }

         private uint _PollingTime;
         [Category("Device Data")]
         [Description("Polling Time")]
         public uint PollingTime
         {
             get
             {
                 return _PollingTime;
             }

             set
             {
                 _PollingTime = value;
             }
         }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "PollingOnlyOnRequest")
            {
                if (PollingOnlyOnRequest && string.IsNullOrEmpty(ConditionalVariableName))
                    return Properties.Resources.ErrorNeedCondionalVar;
            }

            if (propertyName == "OutputOnlyOnRequest")
            {
                if(OutputOnlyOnRequest && string.IsNullOrEmpty(ConditionalVariableName))
                    return Properties.Resources.ErrorNeedCondionalVar;
            }

            if (propertyName == "RetryInitialPolling")
            {
                if (RetryInitialPolling && (string.IsNullOrEmpty(PollingGroup) || !EIBCommJob.IsValidAddress(PollingGroup)))
                    return Properties.Resources.ErrorNeedPollingGroup;
            }
            if (propertyName == "EnablePolling")
            {
                if (EnablePolling && (string.IsNullOrEmpty(PollingGroup) || !EIBCommJob.IsValidAddress(PollingGroup)))
                    return Properties.Resources.ErrorNeedPollingGroup;
            }
            if (propertyName == "RetryOutput")
            {
                if (RetryOutput && (string.IsNullOrEmpty(OutputGroup) || !EIBCommJob.IsValidAddress(OutputGroup)))
                    return Properties.Resources.ErrorNeedOutputGroup;
            }
            if (propertyName == "TagLinkType")
            {
                if(TagLinkType == (int)LinkType.Input)
                {
                    if ((string.IsNullOrEmpty(InputGroups) || !EIBCommJob.IsValidAddress(InputGroups)) && (string.IsNullOrEmpty(PollingGroup) || !EIBCommJob.IsValidAddress(PollingGroup)))
                        return Properties.Resources.ErrorNeedInputOrPollingGroup;
                }
                else if (TagLinkType == (int)LinkType.InputOutput)
                {
                    if ((string.IsNullOrEmpty(OutputGroup) || !EIBCommJob.IsValidAddress(OutputGroup)) && (string.IsNullOrEmpty(PollingGroup) || !EIBCommJob.IsValidAddress(PollingGroup)))
                        return Properties.Resources.ErrorNeedPollingAndOutputGroup;
                }
                else
                {
                    if ((string.IsNullOrEmpty(OutputGroup) || !EIBCommJob.IsValidAddress(OutputGroup)))
                        return Properties.Resources.ErrorNeedOutputGroup;
                }
            }
            // Check Data Format
            if (propertyName == "DataFormat")
            {
                switch (DataFormat)
                {
                    case (int)EISDATAFORMAT.EISDFBit:
                    case (int)EISDATAFORMAT.EISDFByte:
                    case (int)EISDATAFORMAT.EISDFWord:
                    case (int)EISDATAFORMAT.EISDFDWord:
                    case (int)EISDATAFORMAT.EISDFFloat:
                    case (int)EISDATAFORMAT.EISDFEIS3:
                    case (int)EISDATAFORMAT.EISDFEIS4:
                    case (int)EISDATAFORMAT.EISDFEIS5:
                    case (int)EISDATAFORMAT.EISDFEIS6:
                    case (int)EISDATAFORMAT.EISDFAccessPWD6Bytes:
                    case (int)EISDATAFORMAT.EISDFAccessPWD10Bytes:
                    case (int)EISDATAFORMAT.EISDFInt64:
                        break;
                    default:
                        return Properties.Resources.ErrorInvalidDataFormat;
                }
            }

            // Check input group addresses
            else if (propertyName == "InputGroups")
            {
                if (!String.IsNullOrWhiteSpace(InputGroups))
                {
                    string[] ListAddressSplit = InputGroups.Split(new Char[] { ';' });
                    foreach (var InAdd in ListAddressSplit)
                    {
                        if (!EIBCommJob.IsValidAddress(InAdd))
                        {
                            return Properties.Resources.ErrorInvalidInputAddress;
                        }
                    }
                }
            }

            // Check the polling group address
            else if (propertyName == "PollingGroup")
            {
                if (!String.IsNullOrWhiteSpace(PollingGroup))
                {
                    if (!EIBCommJob.IsValidAddress(PollingGroup))
                    {
                            return Properties.Resources.ErrorInvalidPollingAddress;
                    }
                }
            }

            // Check the output group address
            else if (propertyName == "OutputGroup")
            {
                if (!String.IsNullOrWhiteSpace(OutputGroup))
                {
                    if (!EIBCommJob.IsValidAddress(OutputGroup))
                    {
                        return Properties.Resources.ErrorInvalidOutputAddress;
                    }
                }
            }

            return null;
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName) {
                case "TagLinkType":
                    OnPropertyChanged("PollingGroup");
                    OnPropertyChanged("InputGroups");
                    OnPropertyChanged("OutputGroup");
                    break;
            }
        }
        #endregion
    }
}
