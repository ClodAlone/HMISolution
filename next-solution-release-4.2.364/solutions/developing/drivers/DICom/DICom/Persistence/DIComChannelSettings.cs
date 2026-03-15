using System;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.Linq;

namespace DICom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DIComChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public DIComChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(DIComChannelSettings));
        }

        private DIComChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = DIComProtocol.DEFAULT_TCP_PORT;
            _DataCollectingTimeout = DIComProtocol.DEFAULT_DATA_COLLECTING_TIMEOUT;
        }

        public void CopyProperties(DIComChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region Properties
        /// <summary>
        /// Set data collection timout. 
        ///     If elaps, driver force data publis.
        ///     If set = 0, publish data when arrive
        /// </summary>
        private uint? _DataCollectingTimeout;
        public uint? DataCollectingTimeout
        {
            get
            {
                return _DataCollectingTimeout;
            }
            set
            {
                SetPropertyValue("DataCollectingTimeout", ref _DataCollectingTimeout, value);
                RaisePropertyChangedEvent("DataCollectingTimeout");
            }
        }
        #endregion

        private bool DoubleHostNameAndPortInstance()
        {
            // check if other channel use the same parameters
            return (DriverSettings != null &&
                    ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                      where (c != this && ((c as DIComChannelSettings).TcpChannelSettingsHostPort == TcpChannelSettingsHostPort && (c as DIComChannelSettings).TcpChannelSettingsHostName.ToLower() == TcpChannelSettingsHostName.ToLower()))
                      select c).ToList().Count > 0)
                );
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "TcpChannelSettingsHostName":
                    if (DoubleHostNameAndPortInstance())
                        return Properties.Resources.ErrorConfigurationAlreadyExist;

                    // if left BLANK (allowed), default local network card will be used
                    if (string.IsNullOrEmpty(TcpChannelSettingsHostName))
                        return null;
                    break;
            }

            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;


            switch (propertyName)
            {
                case "TcpChannelSettingsHostPort":
                    if (DoubleHostNameAndPortInstance())                    
                        return Properties.Resources.ErrorConfigurationAlreadyExist;                    
                    break;
            }

            return null;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "TcpChannelSettingsHostName":
                    RaisePropertyChangedEvent("TcpChannelSettingsHostPort");
                    break;

                case "TcpChannelSettingsHostPort":
                    RaisePropertyChangedEvent("TcpChannelSettingsHostName");
                    break;
            }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            if (!_DataCollectingTimeout.HasValue)
                _DataCollectingTimeout = DIComProtocol.DEFAULT_DATA_COLLECTING_TIMEOUT;
        }
        #endregion
    }
}
