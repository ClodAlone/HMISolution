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

        public void CopyProperties(S7TIAStationSettings st)
        {
            base.CopyProperties(st);
            _S7TiaPortalSecurityHMIAccessLevel = st.S7TiaPortalSecurityHMIAccessLevel;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _S7TiaPortalSecurityHMIAccessLevel = false;
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
