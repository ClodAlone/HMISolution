using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace CoDeSys
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class CoDeSysStationSettings : StationSettings
    {
        #region Constructors

        public CoDeSysStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(CoDeSysStationSettings));
        }
        protected CoDeSysStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void CopyProperties(CoDeSysStationSettings st)
        {
            base.CopyProperties(st);
            //PLCAddress = st.PLCAddress;
            //PlcVersion = st.PlcVersion;
            //PlcPort = st.PlcPort;
            //UpdateRate = st.UpdateRate;
            //Motorola = st.Motorola;
            //BufferSize = st.BufferSize;
            //LogIn = st.LogIn;
            //Protocol = st.Protocol;
            //UseDirectReading = st.UseDirectReading;
            //MaxVarPerCycle = st.MaxVarPerCycle;
            //UsePing = false;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_PLCAddress = string.Empty;
            //_PlcVersion = CoDeSysProtocol.PlcVersion.IDS_PLC_VER_30;
            //_PlcPort = CoDeSysProtocol.DEFAULT_PORT;
            //_UpdateRate = 100;
            //_Motorola = false;
            //_BufferSize = 0;
            //_LogIn = false;
            //_Protocol = CoDeSysProtocol.Protocol.IDS_PROTOCOL_L2_ROUTE;
            //_UseDirectReading = false;
            //_MaxVarPerCycle = 50;
            //_UsePing = false;

            //set as default value 0 so station will go on error immediatly; this driver recive event of change (state/value) from PVI Server (like OPC Client) and not polling directly device
            //base.MaxRetriesBeforeError = 0;
        }

        #region Properties

        ///// <summary>
        ///// PLC Address
        ///// </summary>
        //private string _PLCAddress;
        //public string PLCAddress
        //{
        //    get { return _PLCAddress; }
        //    set { SetPropertyValue("PLCAddress", ref _PLCAddress, value); }
        //}

        ///// <summary>
        ///// Enter the CoDeSys PLC Version
        ///// </summary>
        //private CoDeSysProtocol.PlcVersion _PlcVersion;
        //public CoDeSysProtocol.PlcVersion PlcVersion
        //{
        //    get { return _PlcVersion; }
        //    set {
        //        SetPropertyValue("PlcVersion", ref _PlcVersion, value);
        //        //RaisePropertyChangedEvent("Channel");
        //        //RaisePropertyChangedEvent("CPUSlot");
        //    }
        //}

        ///// <summary>
        ///// Port Number
        ///// </summary>
        //private uint _PlcPort;
        //public uint PlcPort
        //{
        //    get { return _PlcPort; }
        //    set {
        //        SetPropertyValue("PlcPort", ref _PlcPort, value);
        //        //RaisePropertyChangedEvent("Channel");
        //        //RaisePropertyChangedEvent("PlcType");
        //    }
        //}
               
        ///// <summary>
        ///// Cycling update rate
        ///// </summary>
        //private uint _UpdateRate;
        //public uint UpdateRate
        //{
        //    get { return _UpdateRate; }
        //    set { SetPropertyValue("UpdateRate", ref _UpdateRate, value); }
        //}

        ///// <summary>
        ///// Motorola byte order = default value = False
        ///// </summary>
        //private bool _Motorola;
        //public bool Motorola
        //{
        //    get { return _Motorola; }
        //    set { SetPropertyValue("Motorola", ref _Motorola, value); }
        //}

        ///// <summary>
        ///// Communication BufferSize
        ///// </summary>
        //private uint _BufferSize;
        //public uint BufferSize
        //{
        //    get { return _BufferSize; }
        //    set { SetPropertyValue("BufferSize", ref _BufferSize, value); }
        //}

        ///// <summary>
        ///// LogIn to PLC
        ///// </summary>
        //private bool _LogIn;
        //public bool LogIn
        //{
        //    get { return _LogIn; }
        //    set { SetPropertyValue("LogIn", ref _LogIn, value); }
        //}

        ///// <summary>
        ///// Protocol Type
        ///// </summary>
        //private CoDeSysProtocol.Protocol _Protocol;
        //public CoDeSysProtocol.Protocol Protocol
        //{
        //    get { return _Protocol; }
        //    set { SetPropertyValue("Protocol", ref _Protocol, value); }
        //}

        ///// <summary>
        ///// Enable to retrive data from PLC plling device
        ///// </summary>
        //private bool _UseDirectReading;
        //public bool UseDirectReading
        //{
        //    get { return _UseDirectReading; }
        //    set { SetPropertyValue("UseDirectReading", ref _UseDirectReading, value); }
        //}

        ///// <summary>
        ///// Task aggregation threshold limit
        ///// </summary>
        //private uint _MaxVarPerCycle;
        //public uint MaxVarPerCycle
        //{
        //    get { return _MaxVarPerCycle; }
        //    set { SetPropertyValue("MaxVarPerCycle", ref _MaxVarPerCycle, value); }
        //}

        ///// <summary>
        ///// Use ping to test presence of PLC 
        ///// </summary>
        //private bool _UsePing;
        //public bool UsePing
        //{
        //    get { return _UsePing; }
        //    set { SetPropertyValue("UsePing", ref _UsePing, value); }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Change the channel. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////      
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if ((!string.IsNullOrEmpty(propertyName)) && (propertyName == "Channel"))
            {
                //RaisePropertyChangedEvent("CPUSlot");
                //RaisePropertyChangedEvent("PlcType");
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

            //if (propertyName == "PLCStatusPollingTime")
            //{
            //    if (PLCStatusPollingTime > uint.MaxValue)
            //        return string.Format(Properties.Resources.PLCStatusPollingTimeOutOfRange, uint.MaxValue);
            //}
 
            //if (propertyName == "CPUSlot")
            //{
            //    if (CPUSlot > byte.MaxValue)
            //        return string.Format(Properties.Resources.CPUSlotOutOfRange, uint.MaxValue);
            //}

            //switch (propertyName)
            //{
            //    case "CPUSlot":
            //    case "PlcType":
            //    case "Channel":
            //        if (!string.IsNullOrWhiteSpace(Channel))
            //        {
            //            if ((DriverSettings != null) &&
            //               ((from c in DriverSettings.StationSettings
            //                 where ((c != this) &&
            //                        (c as EtherNetIPStationSettings).CPUSlot == CPUSlot &&
            //                        (c as EtherNetIPStationSettings).PlcType == PlcType &&
            //                        (c as EtherNetIPStationSettings).Channel == Channel)
            //                 select c).ToList().Count > 0))
            //            {
            //                return Properties.Resources.EtherNetIPStationErrorDuplicatedStation;
            //            }
            //        }
            //        break;
            //}
            return null;
        }

        #endregion

    }
}
