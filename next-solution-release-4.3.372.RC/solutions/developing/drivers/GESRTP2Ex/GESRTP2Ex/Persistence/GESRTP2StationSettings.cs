////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\GESRTP2StationSettings.cs
//
// summary:	Implements the driver GESRTP2 station settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace GESRTP2
{
    /// <summary>   Settings for the drivers's station(GESRTP2Station). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2StationSettings : StationSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected GESRTP2StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from GESRTP2StationSettings "st". </summary>
        ///
        /// <param name="st">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _PlcType = ((GESRTP2StationSettings)st).PlcType;
            _Rack = ((GESRTP2StationSettings)st).Rack;
            _Slot = ((GESRTP2StationSettings)st).Slot;
        }

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _PlcType = GESRTP2Protocol.PlcTypes.PacSystem;
            _Rack = 0;
            _Slot = 1;
        }

        #region Properties
        private GESRTP2Protocol.PlcTypes _PlcType;
        public GESRTP2Protocol.PlcTypes PlcType
        {
            get { return _PlcType; }
            set {
                SetPropertyValue("PlcType", ref _PlcType, value); 
                this.RaisePropertyChangedEvent("Rack");
                this.RaisePropertyChangedEvent("Slot");
            }
        }

        private Byte _Rack;
        public Byte Rack
        {
            get { return _Rack; }
            set {
                SetPropertyValue("Rack", ref _Rack, value);
                this.RaisePropertyChangedEvent("Slot");
            }
        }

        private Byte? _Slot;
        public Byte? Slot
        {
            get { return _Slot; }
            set {
                SetPropertyValue("Slot", ref _Slot, value);
                this.RaisePropertyChangedEvent("Rack");
            }
        }
        #endregion


        #region IDataErrorInfo Members

        private bool AnyStationsWithRackSlotDuplicated()
        {
            if (DriverSettings != null)
            {
                var stations = (from s in DriverSettings.StationSettings where s != this && s.Channel == Channel && (((GESRTP2StationSettings)s).Rack == Rack && ((GESRTP2StationSettings)s).Slot == Slot) select s.Name).Count();
                return stations > 0;
            }
            else
            {
                return false;
            }
        }

        private bool AnyStationsWithDifferentPLCType()
        {
            if (DriverSettings != null)
            {
                var stations = (from s in DriverSettings.StationSettings where s != this && s.Channel == Channel && ((GESRTP2StationSettings)s).PlcType != PlcType select s.Name).Count();
                return stations > 0;
            }
            else
            {
                return false;
            }
        }

        private int NrStationsOfChannel()
        {
            if (DriverSettings != null)
            {
                var stations = (from s in DriverSettings.StationSettings where s.Channel == Channel select s.Name).Count();
                return stations;
            }
            else
            {
                return 0;
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "PlcType":
                    switch (_PlcType)
                    {
                        case GESRTP2Protocol.PlcTypes.Series90_30:
                            if (NrStationsOfChannel() > 1)
                                return Properties.Resources.ErrorMultipleStationSeries90_30NotAllowed;
                            break;
                        case GESRTP2Protocol.PlcTypes.PacSystem:
                            if (AnyStationsWithDifferentPLCType())
                                return Properties.Resources.ErrorStationPlcTypeDifferent;
                            break;
                    }                    
                    break;

                case "Rack":
                    if (_PlcType == GESRTP2Protocol.PlcTypes.PacSystem)
                    {
                        if (Rack < 0 || Rack > 9)
                            return Properties.Resources.RackOutOfRange;
                        if (AnyStationsWithRackSlotDuplicated())
                            return Properties.Resources.ErrorRackSlotDuplicated;
                    }
                    break;

                case "Slot":
                    if (_PlcType == GESRTP2Protocol.PlcTypes.PacSystem)
                    {
                        if (Slot < 0 || Slot > 15)
                            return Properties.Resources.SlotOutOfRange;
                        if (AnyStationsWithRackSlotDuplicated())
                            return Properties.Resources.ErrorRackSlotDuplicated;
                    }
                    break;
            }

            return null;
        }
        #endregion

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        protected override void EnsureDefaultValues()
        {
            base.EnsureDefaultValues();

            if (!_Slot.HasValue)
                _Slot = 1;
        }
        #endregion

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "PLCType":
                    RaisePropertyChangedEvent("Rack");
                    RaisePropertyChangedEvent("Slot");
                    break;

                case "Rack":
                    RaisePropertyChangedEvent("Slot");
                    break;

                case "Slot":
                    RaisePropertyChangedEvent("Rack");
                    break;

            }
        }
    }
}
