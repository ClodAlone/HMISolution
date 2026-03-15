using System;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC61850
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC61850ChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public IEC61850ChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private IEC61850ChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 102;
            _ClientSessionSelector = "00 01";
            _ClientPresentationSelector = "00 00 00 01";
            _ClientApplicationID = "1,1,1,999";
            _ClientAEQualifier = 12;
            _ReportAutomaticActivation = true;
            _TimeZone = string.Empty;
        }

        //steve 080711
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            _ClientSessionSelector = ((IEC61850ChannelSettings)ch).ClientSessionSelector;
            _ClientPresentationSelector = ((IEC61850ChannelSettings)ch).ClientPresentationSelector;
            _ClientApplicationID = ((IEC61850ChannelSettings)ch).ClientApplicationID;
            _ClientAEQualifier = ((IEC61850ChannelSettings)ch).ClientAEQualifier;
            _ReportAutomaticActivation = ((IEC61850ChannelSettings)ch).ReportAutomaticActivation;
            _TimeZone = ((IEC61850ChannelSettings)ch).TimeZone;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Client Session Selector
        /// </summary>
        private string _ClientSessionSelector;
        public string ClientSessionSelector
        {
            get
            {
                return _ClientSessionSelector;
            }
            set
            {
                SetPropertyValue("ClientSessionSelector", ref _ClientSessionSelector, value);
            }
        }

        /// <summary>
        /// Client Presentation Selector 
        /// </summary>
        private string _ClientPresentationSelector;
        public string ClientPresentationSelector
        {
            get
            {
                return _ClientPresentationSelector;
            }
            set
            {
                SetPropertyValue("ClientPresentationSelector", ref _ClientPresentationSelector, value);
            }
        }

        /// <summary>
        /// Client Application ID
        /// </summary>
        private string _ClientApplicationID;
        public string ClientApplicationID
        {
            get
            {
                return _ClientApplicationID;
            }
            set
            {
                SetPropertyValue("ClientApplicationID", ref _ClientApplicationID, value);
            }
        }

        /// <summary>
        /// Client AE Qualifier
        /// </summary>
        private uint _ClientAEQualifier;
        public uint ClientAEQualifier
        {
            get
            {
                return _ClientAEQualifier;
            }
            set
            {
                SetPropertyValue("ClientAEQualifier", ref _ClientAEQualifier, value);
            }
        }

        /// <summary>
        /// Automatic Activation Of Reports
        /// </summary>
        private bool _ReportAutomaticActivation;
        public bool ReportAutomaticActivation
        {
            get
            {
                return _ReportAutomaticActivation;
            }
            set
            {
                SetPropertyValue("ReportAutomaticActivation", ref _ReportAutomaticActivation, value);
            }
        }

        /// <summary>
        /// Time Zone for this channel 
        /// </summary>
        private string _TimeZone;
        [Category("Device Data")]
        [Description("TimeZone")]
        [Size(SizeAttribute.Unlimited)]
        public string TimeZone
        {
            get
            {
                return _TimeZone;
            }
            set
            {
                SetPropertyValue("TimeZone", ref _TimeZone, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "ClientSessionSelector")
            {
                string outSelector = String.Empty;
                if(IEC61850Protocol.CheckSelectorString(ClientSessionSelector, ref outSelector, 16) == false)
                {
                    return (Properties.Resources.IEC61850InvalidSessionSelector);
                }
            }
            else if(propertyName == "ClientPresentationSelector")
            {
                string outSelector = String.Empty;
                if (IEC61850Protocol.CheckSelectorString(ClientPresentationSelector, ref outSelector, 16) == false)
                {
                    return (Properties.Resources.IEC61850InvalidPresentationSelector);
                }
            }
            else if (propertyName == "ClientApplicationID")
            {
                if(IEC61850Protocol.CheckAPTitleString(ClientApplicationID) == false)
                {
                    return (Properties.Resources.IEC61850InvalidApplicationID);
                }
            }
            else if (propertyName == "ClientAEQualifier")
            {
                if(ClientAEQualifier > 99999)
                {
                    return (Properties.Resources.IEC61850InvalidAEQualifier);
                }
            }

            return null;
        }

        #endregion
    
    }
}
