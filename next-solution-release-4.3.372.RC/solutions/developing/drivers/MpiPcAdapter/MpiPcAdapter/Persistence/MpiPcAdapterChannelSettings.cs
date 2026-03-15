using System;
using System.Collections.Generic;
using System.Linq;
using SerialDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MpiPcAdapter
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MpiPcAdapterChannelSettings : SerialChannelSettings
    {
        #region Constructors

        public MpiPcAdapterChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private MpiPcAdapterChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        #region methods

        public void DefaultSettings()
        {
            base.DefaultSettings();

            CommPortName = "Com1";
            CommPortBaudRate = 38400;
            CommPortDataBits = 8;
            CommPortParity = (int)System.IO.Ports.Parity.Odd;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;

            _PcMpiID = 0;
            _MpiNetworkBitRate = MpiNetworkBitRates.BITRATE_187_5K;
            _PcOnlyMaster = true;
            _HighestStationAddress = 31;
        }

        public void CopyProperties(MpiPcAdapterChannelSettings ch)
        {
            base.CopyProperties(ch);
            _PcMpiID = ch.PcMpiID;
            _MpiNetworkBitRate = ch.MpiNetworkBitRate;
            _PcOnlyMaster = ch.PcOnlyMaster;
            _HighestStationAddress = ch.HighestStationAddress;
        }

        #endregion

        #region Properties

        private MpiNetworkBitRates _MpiNetworkBitRate;
        public MpiNetworkBitRates MpiNetworkBitRate
        {
            get { return _MpiNetworkBitRate; }
            set { SetPropertyValue("MpiNetworkBitrate", ref _MpiNetworkBitRate, value); }
        }

        private byte _PcMpiID;
        public byte PcMpiID
        {
            get { return _PcMpiID; }
            set { SetPropertyValue("PcMpiID", ref _PcMpiID, value); }
        }

        private bool _PcOnlyMaster;
        public bool PcOnlyMaster
        {
            get { return _PcOnlyMaster; }
            set { SetPropertyValue("PcOnlyMaster", ref _PcOnlyMaster, value); }
        }

        private byte _HighestStationAddress;
        public byte HighestStationAddress
        {
            get { return _HighestStationAddress; }
            set { SetPropertyValue("HighestStationAddress", ref _HighestStationAddress, value); }
        }

        #endregion


        #region IDataErrorInfo Members
       
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "PcMpiAddress")
            {
                //if(PcMpiAddress > 0xff)
            }

            return null;
        }

        #endregion
    }
}
