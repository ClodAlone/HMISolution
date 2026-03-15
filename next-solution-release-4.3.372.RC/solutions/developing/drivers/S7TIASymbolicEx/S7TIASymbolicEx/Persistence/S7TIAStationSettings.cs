using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TIASymbolic
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TIAStationSettings : StationSettings
    {
        #region Constructors

        public S7TIAStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected S7TIAStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _S7TiaPortalSecurityHMIAccessLevel = ((S7TIAStationSettings)st).S7TiaPortalSecurityHMIAccessLevel;
            _S7TiaPortalSecurityHMIPassword = ((S7TIAStationSettings)st).S7TiaPortalSecurityHMIPassword;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _S7TiaPortalSecurityHMIAccessLevel = false;
            _S7TiaPortalSecurityHMIPassword = string.Empty;
        }

        #region Properties
        private bool _S7TiaPortalSecurityHMIAccessLevel;
        public bool S7TiaPortalSecurityHMIAccessLevel
        {
            get
            {
                return _S7TiaPortalSecurityHMIAccessLevel;
            }
            set
            {
                SetPropertyValue("S7TiaPortalSecurityHMIAccessLevel", ref _S7TiaPortalSecurityHMIAccessLevel, value);
            }
        }

        private string _S7TiaPortalSecurityHMIPassword;
        [ValueConverter(typeof(DriverCodeBaseEx.EncryptString.EncryptedValueConverter))]
        public string S7TiaPortalSecurityHMIPassword
        {
            get
            {
                return _S7TiaPortalSecurityHMIPassword;
            }
            set
            {
                SetPropertyValue("S7TiaPortalSecurityHMIPassword", ref _S7TiaPortalSecurityHMIPassword, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DeviceID")
            {
                //if(DeviceID > 0xff)
            }

            if (propertyName == "Name")
            {
                //Check if the station name is larger than 100 characters, as it may cause an exception when it is saved on the DB. So to avoid.
                if (Name.Length > 99)
                {
                    return Properties.Resources.ErrorInvalidStationName;
                }
            }
            return null;
        }

        #endregion

      

    }
}
