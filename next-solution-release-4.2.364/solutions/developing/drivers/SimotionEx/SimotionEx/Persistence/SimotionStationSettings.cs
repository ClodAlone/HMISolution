using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Simotion
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SimotionStationSettings : StationSettings
    {
        #region Constructors

        public SimotionStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SimotionStationSettings));
        }
        protected SimotionStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(SimotionStationSettings st)
        {
            base.CopyProperties(st);
            _SymbolicFile = st.SymbolicFile;
            _LastUpdate = st.LastUpdate;
            _FileBody = st.FileBody;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _FileBody = null;
            _SymbolicFile = String.Empty;
            _LastUpdate = DateTime.UtcNow;
    }
        
        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "Name":
                    //Check if the station name is larger than 100 characters, as it may cause an exception when it is saved on the DB. So to avoid.
                    if (Name.Length > 99)
                    {
                        return Properties.Resources.ErrorInvalidStationName;
                    }
                    break;
                case "SymbolicFile":                    
                    if (string.IsNullOrEmpty(_SymbolicFile))
                        return Properties.Resources.ErrorSymbolicFileNotDefined;
                    break;
            }
            return null;
        }

        #endregion

        #region Properties       
        private string _SymbolicFile;
        [Size(SizeAttribute.Unlimited)]
        public string SymbolicFile
        {
            get { return _SymbolicFile; }
            set { SetPropertyValue("SymbolicFile", ref _SymbolicFile, value); }            
        }

        private byte[] _FileBody;
        public byte[] FileBody
        {
            get
            {
                return _FileBody;
            }
            set
            {
                SetPropertyValue("FileBody", ref _FileBody, value);
            }
        }

        private DateTime _LastUpdate;
        public DateTime LastUpdate
        {
            get
            {
                return _LastUpdate;
            }
            set
            {
                SetPropertyValue("LastUpdate", ref _LastUpdate, value);
            }
        }
        #endregion
    }
}
