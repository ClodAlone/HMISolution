using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EtherNetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EtherNetIPStationSettings : StationSettings
    {
        #region Constructors

        public EtherNetIPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EtherNetIPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            PlcType = ((EtherNetIPStationSettings)st).PlcType;
            CPUSlot = ((EtherNetIPStationSettings)st).CPUSlot;
            Logix5550NonBlockPhAdd = ((EtherNetIPStationSettings)st).Logix5550NonBlockPhAdd;
            ReadStructuresMode = ((EtherNetIPStationSettings)st).ReadStructuresMode;
            PLCStatusPollingTime = ((EtherNetIPStationSettings)st).PLCStatusPollingTime;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _PlcType = PlcTypes.SLC500_MicroLogix;
            _CPUSlot = 0;
            _Logix5550NonBlockPhAdd = PhisicalAddressesOptimizzations.False;
            _ReadStructuresMode = ReadStructuresModes.ReadFieldByField;
            _PLCStatusPollingTime = 0;
        }

        #region Properties

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
        ///  Enter the Addresses Mode
        /// </summary>
        private PhisicalAddressesOptimizzations _Logix5550NonBlockPhAdd;
        public PhisicalAddressesOptimizzations Logix5550NonBlockPhAdd
        {
            get { return _Logix5550NonBlockPhAdd; }
            set { SetPropertyValue("Logix5550NonBlockPhAdd", ref _Logix5550NonBlockPhAdd, value); }
        }

        /// <summary>
        ///  Enter the Read Structures Mode 
        /// </summary>
        private ReadStructuresModes _ReadStructuresMode;
        public ReadStructuresModes ReadStructuresMode
        {
            get { return _ReadStructuresMode; }
            set { SetPropertyValue("ReadStructuresMode", ref _ReadStructuresMode, value); }
        }

        /// <summary>
        ///  Enter PLC Status Polling Time\nFrequency in ms. of PLC status requests (0 = no requests)
        /// </summary>
        private uint _PLCStatusPollingTime;
        public uint PLCStatusPollingTime
        {
            get { return _PLCStatusPollingTime; }
            set { SetPropertyValue("PLCStatusPollingTime", ref _PLCStatusPollingTime, value); }
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

            if (propertyName == "PLCStatusPollingTime")
            {
                if (PLCStatusPollingTime > uint.MaxValue)
                    return string.Format(Properties.Resources.PLCStatusPollingTimeOutOfRange, uint.MaxValue);
            }
 
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
                                    (c as EtherNetIPStationSettings).CPUSlot == CPUSlot &&
                                    (c as EtherNetIPStationSettings).PlcType == PlcType &&
                                    (c as EtherNetIPStationSettings).Channel == Channel)
                             select c).ToList().Count > 0))
                        {
                            return Properties.Resources.EtherNetIPStationErrorDuplicatedStation;
                        }
                    }
                    break;
            }
            return null;
        }

        #endregion

    }
}
