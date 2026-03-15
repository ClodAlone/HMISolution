using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
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
        }
        protected S7TCPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _DeviceID = ((S7TCPStationSettings)st)._DeviceID;
            _Rack = ((S7TCPStationSettings)st)._Rack;
            _Slot = ((S7TCPStationSettings)st)._Slot;
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
            set { SetPropertyValue("DeviceID", ref _DeviceID, value); 
                RaisePropertyChangedEvent("Rack");
                RaisePropertyChangedEvent("Slot");
            }
        }
        private byte _Rack;
        public byte Rack
        {
            get { return _Rack; }
            set { SetPropertyValue("Rack", ref _Rack, value);
                RaisePropertyChangedEvent("DeviceID");
                RaisePropertyChangedEvent("Slot");
            }
        }
        private byte _Slot;
        public byte Slot
        {
            get { return _Slot; }
            set { SetPropertyValue("Slot", ref _Slot, value);
                RaisePropertyChangedEvent("DeviceID");
                RaisePropertyChangedEvent("Rack");
            }
        }
        #endregion

        #region IDataErrorInfo Members

        private bool AnyStationsWithDeviceIDRackSlotDuplicated()
        {
            if (DriverSettings != null)
            {
                var stations = (from s in DriverSettings.StationSettings where s != this && s.Channel == Channel && ((S7TCPStationSettings)s).DeviceID == DeviceID && (((S7TCPStationSettings)s).Rack == Rack && ((S7TCPStationSettings)s).Slot == Slot) select s.Name).Count();
                return stations > 0;
            }
            else
            {
                return false;
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "DeviceID":
                    if (AnyStationsWithDeviceIDRackSlotDuplicated())
                        return Properties.Resources.ErrorDeviceIDRackSlotDuplicated;
                    break;
                case "Rack":
                    if (Rack > 15)
                        return Properties.Resources.RackOutOfRange;
                    if (AnyStationsWithDeviceIDRackSlotDuplicated())
                        return Properties.Resources.ErrorDeviceIDRackSlotDuplicated;
                    break;
                case "Slot":            
                    if (Slot < 0 || Slot > 31)
                        return Properties.Resources.SlotOutOfRange;
                    if (AnyStationsWithDeviceIDRackSlotDuplicated())
                        return Properties.Resources.ErrorDeviceIDRackSlotDuplicated;
                    break;
            }

            return null;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "DeviceID":
                    RaisePropertyChangedEvent("Rack");
                    RaisePropertyChangedEvent("Slot");
                    break;

                case "Rack":
                    RaisePropertyChangedEvent("DeviceID");
                    RaisePropertyChangedEvent("Slot");
                    break;

                case "Slot":
                    RaisePropertyChangedEvent("DeviceID");
                    RaisePropertyChangedEvent("Rack");
                    break;

            }
        }

        #endregion

    }
}
