using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SaiaDataMode
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SaiaDataModeStationSettings : StationSettings
    {
                #region Constructors

        public SaiaDataModeStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SaiaDataModeStationSettings));
        }
        protected SaiaDataModeStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(SaiaDataModeStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 0;
        }

        #region Properties

        /// <summary>
        /// Enter the numeric station address (0..247)
        /// </summary>
        private byte _StationID;
        public byte StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                SetPropertyValue("StationID", ref _StationID, value);
            }
        }
        
        #endregion



        #region IDataErrorInfo Members
        #endregion

    }
}
