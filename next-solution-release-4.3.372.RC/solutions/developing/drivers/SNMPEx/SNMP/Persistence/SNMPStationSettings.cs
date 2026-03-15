using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.Linq;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPStationSettings : StationSettings
    {
        #region Constructors
        public SNMPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SNMPStationSettings));
        }
        protected SNMPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _snmpContextName = ((SNMPStationSettings)st).snmpContextName;
            _snmpUserName = ((SNMPStationSettings)st).snmpUserName;
            _snmpSecurityLevel = ((SNMPStationSettings)st).snmpSecurityLevel;
            _snmpAuthenticationProtocol = ((SNMPStationSettings)st).snmpAuthenticationProtocol;
            _snmpAuthenticationPassword = ((SNMPStationSettings)st).snmpAuthenticationPassword;
            _snmpPrivacyEncryptionProtocol = ((SNMPStationSettings)st).snmpPrivacyEncryptionProtocol;
            _snmpPrivacyEncryptionPassword = ((SNMPStationSettings)st).snmpPrivacyEncryptionPassword;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _snmpVersion = null;
            _snmpContextName = string.Empty;
            _snmpUserName = string.Empty;
            _snmpSecurityLevel = SNMPProtocol.SecurityLevel.NoAuthNoPriv;
            _snmpAuthenticationProtocol = SNMPProtocol.AuthenticationProtocol.None;
            _snmpAuthenticationPassword = string.Empty;
            _snmpPrivacyEncryptionProtocol = SNMPProtocol.PrivacyEncryptionProtocol.None;
            _snmpPrivacyEncryptionPassword = string.Empty;
        }

        public bool UserNameAlreadyUsed()
        {
            if (DriverSettings != null)
            {
                if (snmpVersion == SNMPVERSION.SNMPv3)
                {
                    var listStations = (from s in DriverSettings.StationSettings where s.Channel == this.Channel && snmpUserName == ((SNMPStationSettings)s).snmpUserName select s.Name).Count();
                    if (listStations > 1)
                        return true;
                }
            }

            return false;
        }

        #region IDataErrorInfo Members
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   SNMPStationSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "snmpContextName":
                    break;
                case "snmpUserName":
                    if (snmpVersion != null && snmpVersion == SNMPVERSION.SNMPv3)
                    {
                        if (string.IsNullOrEmpty(_snmpUserName))
                            return Properties.Resources.SNMPErrorUserNameInvalid;

                        if (UserNameAlreadyUsed())
                            return String.Format(Properties.Resources.SNMPErrorUserNameAlreadyUsed, snmpUserName);
                    }
                    break;
                
                case "snmpAuthenticationProtocol":
                    if (snmpVersion != null && snmpVersion == SNMPVERSION.SNMPv3)
                    {
                        if (_snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthNoPriv || _snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                        {
                            if (_snmpAuthenticationProtocol == SNMPProtocol.AuthenticationProtocol.None)
                                return Properties.Resources.SNMPErrorAuthenticationProtocolInvalid;
                        }
                    }
                    break;

                case "snmpAuthenticationPassword":
                    if (snmpVersion != null && snmpVersion == SNMPVERSION.SNMPv3)
                    {
                        if (_snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthNoPriv || _snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                        {
                            if (string.IsNullOrEmpty(_snmpAuthenticationPassword) || _snmpAuthenticationPassword.Length < 8)
                                return Properties.Resources.SNMPErrorAuthenticationPasswordInvalid;
                        }
                    }
                    break;

                case "snmpPrivacyEncryptionProtocol":
                    if (snmpVersion != null && snmpVersion == SNMPVERSION.SNMPv3)
                    {
                        if (_snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                        {
                            if (_snmpPrivacyEncryptionProtocol == SNMPProtocol.PrivacyEncryptionProtocol.None)
                                return Properties.Resources.SNMPErrorPrivacyEncryptionProtocol;
                        }
                    }
                    break;

                case "snmpPrivacyEncryptionPassword":
                    if (snmpVersion != null && snmpVersion == SNMPVERSION.SNMPv3)
                    {
                        if (_snmpSecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                        {                    
                            if (string.IsNullOrEmpty(_snmpPrivacyEncryptionPassword) || _snmpPrivacyEncryptionPassword.Length < 8)
                                return Properties.Resources.SNMPErrorPrivacyEncryptionPasswordInvalid;
                        }
                    }
                    break;
            }

            return null;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "snmpSecurityLevel":
                    RaisePropertyChangedEvent("snmpAuthenticationProtocol");
                    RaisePropertyChangedEvent("snmpAuthenticationPassword");
                    RaisePropertyChangedEvent("snmpPrivacyEncryptionProtocol");
                    RaisePropertyChangedEvent("snmpPrivacyEncryptionPassword");                    
                    break;
            }
        }
        #endregion

        #region Properties        
        private string _snmpContextName;
        public string snmpContextName
        {
            get { return (_snmpContextName == null ? string.Empty : _snmpContextName); }
            set
            {
                SetPropertyValue("snmpContextName", ref _snmpContextName, value);
            }
        }

        private string _snmpUserName;
        public string snmpUserName
        {
            get { return (_snmpUserName == null ? string.Empty : _snmpUserName); }
            set
            {
                SetPropertyValue("snmpUserName", ref _snmpUserName, value);
            }
        }

        private SNMPProtocol.SecurityLevel _snmpSecurityLevel;
        public SNMPProtocol.SecurityLevel snmpSecurityLevel
        {
            get { return _snmpSecurityLevel; }
            set
            {
                SetPropertyValue("snmpSecurityLevel", ref _snmpSecurityLevel, value);
            }
        }

        private SNMPProtocol.AuthenticationProtocol _snmpAuthenticationProtocol;
        public SNMPProtocol.AuthenticationProtocol snmpAuthenticationProtocol
        {
            get { return _snmpAuthenticationProtocol; }
            set
            {
                SetPropertyValue("snmpAuthenticationProtocol", ref _snmpAuthenticationProtocol, value);
            }
        }

        private string _snmpAuthenticationPassword;
        public string snmpAuthenticationPassword
        {
            get { return (_snmpAuthenticationPassword == null ? string.Empty : _snmpAuthenticationPassword); }
            set
            {
                SetPropertyValue("snmpAuthenticationPassword", ref _snmpAuthenticationPassword, value);
            }
        }

        private SNMPProtocol.PrivacyEncryptionProtocol _snmpPrivacyEncryptionProtocol;
        public SNMPProtocol.PrivacyEncryptionProtocol snmpPrivacyEncryptionProtocol
        {
            get { return _snmpPrivacyEncryptionProtocol; }
            set
            {
                SetPropertyValue("snmpPrivacyEncryptionProtocol", ref _snmpPrivacyEncryptionProtocol, value);
            }
        }

        private string _snmpPrivacyEncryptionPassword;
        public string snmpPrivacyEncryptionPassword
        {
            get { return (_snmpPrivacyEncryptionPassword == null ? string.Empty : _snmpPrivacyEncryptionPassword); }
            set
            {
                SetPropertyValue("snmpPrivacyEncryptionPassword", ref _snmpPrivacyEncryptionPassword, value);
            }
        }

        /// used only for properties validation !!! 
        private SNMPVERSION? _snmpVersion;
        public SNMPVERSION? snmpVersion
        {
            get { return _snmpVersion; }
            set { _snmpVersion = value; }
        }
        #endregion        
    }
}
