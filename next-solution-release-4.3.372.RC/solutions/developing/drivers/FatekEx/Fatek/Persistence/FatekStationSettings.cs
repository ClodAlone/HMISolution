using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace Fatek
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FatekStationSettings : StationSettings
    {
        #region Constructors

        public FatekStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FatekStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(FatekStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
        }

        #region Properties

        /// <summary>
        /// Enter the numeric station address (0..247)
        /// </summary>
        private uint _StationID;
        public uint StationID
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

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) {
                case "StationID":
                    if(StationID < 1 || StationID > 254)
                        return Properties.Resources.StationIDOutOfRange;
                    break;
            }

            return null;
        }

        #endregion

    }
}
