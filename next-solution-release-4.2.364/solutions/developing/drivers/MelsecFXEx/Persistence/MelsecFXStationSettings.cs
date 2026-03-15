using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFX
{
    public enum MelsecFXPLCType : int
    {
        FX,
        FX2N,
        FX3U
    }

    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXStationSettings : StationSettings
    {
        #region Constructors

        public MelsecFXStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MelsecFXStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(MelsecFXStationSettings st)
        {
            base.CopyProperties(st);
            PLCType = st.PLCType;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _PLCType = MelsecFXPLCType.FX3U;
        }

        #region Properties

        /// <summary>
        /// PLC Type
        /// </summary>
        private MelsecFXPLCType _PLCType;
        public MelsecFXPLCType PLCType
        {
            get
            {
                return _PLCType;
            }
            set
            {
                SetPropertyValue("PLCType", ref _PLCType, value);
            }
        }
 
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "PLCType")
            {
                if ((PLCType != MelsecFXPLCType.FX3U) &&
                    (PLCType != MelsecFXPLCType.FX2N) &&
                    (PLCType != MelsecFXPLCType.FX))
                {
                    return Properties.Resources.ErrorInvalidPlcType;
                }

            }

            return null;
        }

        #endregion
    }
}
