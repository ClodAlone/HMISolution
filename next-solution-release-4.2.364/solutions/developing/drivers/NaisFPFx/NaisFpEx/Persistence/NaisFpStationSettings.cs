using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace NaisFp
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class NaisFpStationSettings : StationSettings
    {
                #region Constructors

        public NaisFpStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected NaisFpStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(NaisFpStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
            FrameFormat = st.FrameFormat;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
            _FrameFormat = FrameFormats.Normal;
        }

        #region Properties

 
        /// <summary>
        /// Enter the numeric station address (0..99)
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

        /// <summary>
        /// select the frame type (the long format is supported only by high level units, such as the ET-LAN unit or MEWNET-H link unit)
        /// </summary>
        private FrameFormats _FrameFormat;
        public FrameFormats FrameFormat
        {
            get
            {
                return _FrameFormat;
            }
            set
            {
                SetPropertyValue("FrameFormat", ref _FrameFormat, value);
            }
        }


        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StationID")
            {
                if(StationID < 1 || StationID > 99)
                    return Properties.Resources.StationIDOutOfRange;

            }

            return null;
        }

        #endregion

    }
}
