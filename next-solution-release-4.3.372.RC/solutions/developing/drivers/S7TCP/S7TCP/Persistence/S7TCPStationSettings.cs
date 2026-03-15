using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TCPStationSettings : StationSettings
    {
        #region Constructors

        public S7TCPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(S7TCPStationSettings));
        }
        protected S7TCPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(S7TCPStationSettings st)
        {
            base.CopyProperties(st);
            _DeviceID = st._DeviceID;
            _Rack = st._Rack;
            _Slot = st._Slot;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _DeviceID = 2;
            _Rack = 0;
            _Slot = 2;
        }

        #region Properties
        private byte _DeviceID;
        public byte DeviceID
        {
            get { return _DeviceID; }
            set { SetPropertyValue("DeviceID", ref _DeviceID, value); ; }
        }
        private byte _Rack;
        public byte Rack
        {
            get { return _Rack; }
            set { SetPropertyValue("Rack", ref _Rack, value); }
        }
        private byte _Slot;
        public byte Slot
        {
            get { return _Slot; }
            set { SetPropertyValue("Slot", ref _Slot, value); }
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
            else if (propertyName == "Rack")
            {
                if (Rack > 15)
                    return Properties.Resources.RackOutOfRange;
            }
            else if (propertyName == "Slot")
            {
                if (Slot < 0 || Slot > 31)
                    return Properties.Resources.SlotOutOfRange;
            }

            return null;
        }

        #endregion

    }
}
