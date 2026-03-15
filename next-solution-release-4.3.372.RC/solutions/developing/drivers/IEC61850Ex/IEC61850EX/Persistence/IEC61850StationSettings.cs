using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC61850
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC61850StationSettings : StationSettings
    {
        #region Constructors

        public IEC61850StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected IEC61850StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _TransportSelector = ((IEC61850StationSettings)st).TransportSelector;
            _SessionSelector = ((IEC61850StationSettings)st).SessionSelector;
            _PresentationSelector = ((IEC61850StationSettings)st).PresentationSelector;
            _ApplicationID = ((IEC61850StationSettings)st).ApplicationID;
            _AEQualifier = ((IEC61850StationSettings)st).AEQualifier;
            _AuthenticationEnabled = ((IEC61850StationSettings)st).AuthenticationEnabled;
            _AuthenticationPassword = ((IEC61850StationSettings)st).AuthenticationPassword;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TransportSelector = "00 01";
            _SessionSelector = "00 01";
            _PresentationSelector = "00 00 00 01";
            _ApplicationID = "1,1,1,999";
            _AEQualifier = 12;
            _AuthenticationEnabled = false;
            _AuthenticationPassword = String.Empty;
        }

        #region Properties

        /// <summary>
        /// Client Transport Selector
        /// </summary>
        private string _TransportSelector;
        public string TransportSelector
        {
            get
            {
                return _TransportSelector;
            }
            set
            {
                SetPropertyValue("TransportSelector", ref _TransportSelector, value);
            }
        }

        /// <summary>
        /// Client Session Selector
        /// </summary>
        private string _SessionSelector;
        public string SessionSelector
        {
            get
            {
                return _SessionSelector;
            }
            set
            {
                SetPropertyValue("SessionSelector", ref _SessionSelector, value);
            }
        }

        /// <summary>
        /// Presentation Selector 
        /// </summary>
        private string _PresentationSelector;
        public string PresentationSelector
        {
            get
            {
                return _PresentationSelector;
            }
            set
            {
                SetPropertyValue("PresentationSelector", ref _PresentationSelector, value);
            }
        }

        /// <summary>
        /// Application ID
        /// </summary>
        private string _ApplicationID;
        public string ApplicationID
        {
            get
            {
                return _ApplicationID;
            }
            set
            {
                SetPropertyValue("ApplicationID", ref _ApplicationID, value);
            }
        }

        /// <summary>
        /// AE Qualifier
        /// </summary>
        private uint _AEQualifier;
        public uint AEQualifier
        {
            get
            {
                return _AEQualifier;
            }
            set
            {
                SetPropertyValue("AEQualifier", ref _AEQualifier, value);
            }
        }

        /// <summary>
        /// Authentication Enabled
        /// </summary>
        private bool _AuthenticationEnabled;
        public bool AuthenticationEnabled
        {
            get
            {
                return _AuthenticationEnabled;
            }
            set
            {
                SetPropertyValue("AuthenticationEnabled", ref _AuthenticationEnabled, value);
            }
        }

        /// <summary>
        /// Authentication Password
        /// </summary>
        private string _AuthenticationPassword;
        public string AuthenticationPassword
        {
            get
            {
                return _AuthenticationPassword;
            }
            set
            {
                SetPropertyValue("AuthenticationPassword", ref _AuthenticationPassword, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TransportSelector")
            {
                string outSelector = String.Empty;
                if (IEC61850Protocol.CheckSelectorString(TransportSelector, ref outSelector, 8) == false)
                {
                    return (Properties.Resources.IEC61850InvalidTransportSelector);
                }
            }
            else if (propertyName == "SessionSelector")
            {
                string outSelector = String.Empty;
                if (IEC61850Protocol.CheckSelectorString(SessionSelector, ref outSelector, 16) == false)
                {
                    return (Properties.Resources.IEC61850InvalidSessionSelector);
                }
            }
            else if (propertyName == "PresentationSelector")
            {
                string outSelector = String.Empty;
                if (IEC61850Protocol.CheckSelectorString(PresentationSelector, ref outSelector, 16) == false)
                {
                    return (Properties.Resources.IEC61850InvalidPresentationSelector);
                }
            }
            else if (propertyName == "ApplicationID")
            {
                if (IEC61850Protocol.CheckAPTitleString(ApplicationID) == false)
                {
                    return (Properties.Resources.IEC61850InvalidApplicationID);
                }
            }
            else if (propertyName == "AEQualifier")
            {
                if (AEQualifier > 99999)
                {
                    return (Properties.Resources.IEC61850InvalidAEQualifier);
                }
            }

            return null;
        }

        #endregion

    }
}
