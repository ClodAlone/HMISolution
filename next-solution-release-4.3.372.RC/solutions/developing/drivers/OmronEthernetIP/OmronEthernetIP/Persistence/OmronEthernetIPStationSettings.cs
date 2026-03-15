using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronEthernetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronEthernetIPStationSettings : StationSettings
    {
        #region Constructors

        public OmronEthernetIPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(OmronEthernetIPStationSettings));
        }
        protected OmronEthernetIPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void CopyProperties(OmronEthernetIPStationSettings st)
        {
            base.CopyProperties(st);
            CPUSlot = st.CPUSlot;
            PlcType = st.PlcType;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _CPUSlot = 0;
            _PlcType = PlcTypes.NJ;
        }

        #region Properties

        /// <summary>
        /// Enter the CPU slot number (used only for ControlLogix or CompactLogix PLCs)
        /// </summary>
        private byte _CPUSlot;
        public byte CPUSlot
        {
            get { return _CPUSlot; }
            set { 
                SetPropertyValue("CPUSlot", ref _CPUSlot, value);
                RaisePropertyChangedEvent("Channel");
                RaisePropertyChangedEvent("PlcType");
            }
        }

        /// <summary>
        /// Enter the PLC type
        /// </summary>
        private PlcTypes _PlcType;
        public PlcTypes PlcType
        {
            get { return _PlcType; }
            set { 
                SetPropertyValue("PlcType", ref _PlcType, value);
                RaisePropertyChangedEvent("Channel");
                RaisePropertyChangedEvent("CPUSlot");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Change the channel. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////      
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if ((!string.IsNullOrEmpty(propertyName)) && (propertyName == "Channel"))
            {
                RaisePropertyChangedEvent("CPUSlot");
                RaisePropertyChangedEvent("PlcType");
            }
            base.OnChanged(propertyName, oldValue, newValue);
        }
        #endregion



        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
 
            if (propertyName == "CPUSlot")
            {
                if (CPUSlot > byte.MaxValue)
                    return string.Format(Properties.Resources.CPUSlotOutOfRange, uint.MaxValue);
            }
            switch (propertyName)
            {
                case "CPUSlot":
                case "PlcType":
                case "Channel":
                    if (!string.IsNullOrWhiteSpace(Channel))
                    {
                        if ((DriverSettings != null) &&
                           ((from c in DriverSettings.StationSettings
                             where ((c != this) &&
                                    (c as OmronEthernetIPStationSettings).CPUSlot == CPUSlot &&
                                    (c as OmronEthernetIPStationSettings).PlcType == PlcType &&
                                    (c as OmronEthernetIPStationSettings).Channel == Channel)
                             select c).ToList().Count > 0))
                        {
                            return Properties.Resources.StationErrorDuplicatedStation;
                        }
                    }
                    break;
            }

            return null;
        }

        #endregion

    }
}
