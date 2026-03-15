using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace RMS621
{   
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class RMS621StationSettings : StationSettings
    {
        #region Constructors

        public RMS621StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected RMS621StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(RMS621StationSettings st)
        {
            base.CopyProperties(st);
            _UnitNumber = st.UnitNumber;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _UnitNumber = 1;
        }

        #region Properties

        private byte _UnitNumber;
        /// <summary>
        /// Device unit number
        /// </summary>
        public byte UnitNumber
        {
            get
            {
                return _UnitNumber;
            }
            set
            {
                _UnitNumber = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "UnitNumber")
            {
                if (UnitNumber<0 || UnitNumber>99)
                    return Properties.Resources.ErrorInvalidUnitNumber;

                if (DriverSettings != null && ((from s in DriverSettings.StationSettings
                                                where s != this && s.Channel == this.Channel &&
                                                (s as RMS621StationSettings).UnitNumber == _UnitNumber
                                                select s).ToList().Count > 0))
                {
                    return Properties.Resources.ErrorStationWithSameUnitNumberExit;
                }
            }

            return null;
        }

        #endregion
    }
}
