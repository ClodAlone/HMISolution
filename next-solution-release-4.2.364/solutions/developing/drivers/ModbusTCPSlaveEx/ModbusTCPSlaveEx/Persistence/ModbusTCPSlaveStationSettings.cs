using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCPSlave
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPSlaveStationSettings : StationSettings
    {
                #region Constructors

        public ModbusTCPSlaveStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusTCPSlaveStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(ModbusTCPSlaveStationSettings st)
        {
            base.CopyProperties(st);
            StationID = st.StationID;
            DeviceHostName = st.DeviceHostName;
            DeviceBackupHostName = st.DeviceBackupHostName;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StationID = 1;
            DeviceHostName = "";
            DeviceBackupHostName = "";
        }

        #region Properties

        /// <summary>
        /// Enter the numeric station address (0..247)
        /// </summary>
        private uint _StationID;
        public uint StationID
        {
            get
            {
                return _StationID;
            }
            set
            {
                SetPropertyValue("StationID", ref _StationID, value);
                this.RaisePropertyChangedEvent("DeviceHostName");
            }
        }
        /// <summary>   Device Host name. </summary>
        private string _DeviceHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Device host. </summary>
        ///
        /// <value> The name of the Device host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set
            {
                SetPropertyValue("DeviceHostName", ref _DeviceHostName, value);
                this.RaisePropertyChangedEvent("StationID");
                this.RaisePropertyChangedEvent("DeviceBackupHostName");
            }
        }

        /// <summary>   Device Backup Host Name. </summary>
        private string _DeviceBackupHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Device Backup host. </summary>
        ///
        /// <value> The name of the Device Backup Host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceBackupHostName
        {
            get { return _DeviceBackupHostName; }
            set
            {
                SetPropertyValue("DeviceBackupHostName", ref _DeviceBackupHostName, value);
                this.RaisePropertyChangedEvent("DeviceHostName");
            }
        }

        #endregion



        #region IDataErrorInfo Members
        
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "StationID":
                    if (string.IsNullOrEmpty(_DeviceHostName))
                    {
                        if (StationID < 0 || StationID > 247)
                            return Properties.Resources.StationIDOutOfRange;

                        if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where s != this && s.Channel == this.Channel &&
                                                        (s as ModbusTCPSlaveStationSettings).StationID == _StationID &&
                                                        string.IsNullOrEmpty((s as ModbusTCPSlaveStationSettings).DeviceHostName)
                                                        select s).ToList().Count > 0))
                            return Properties.Resources.SelectExistingStationID;
                    }
                    break;
                case "DeviceHostName":
                    if (!string.IsNullOrEmpty(_DeviceHostName))
                    {
                        if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where (s != this && s.Channel == this.Channel &&
                                                        ((s as ModbusTCPSlaveStationSettings).DeviceHostName == _DeviceHostName ||
                                                        (s as ModbusTCPSlaveStationSettings).DeviceBackupHostName == _DeviceHostName) ||
                                                        _DeviceBackupHostName == _DeviceHostName)
                                                        select s).ToList().Count > 0))
                        {
                            return Properties.Resources.SelectExistingDeviceName;
                        }
                    }
                    else
                        _DeviceBackupHostName = "";
                    break;
                case "DeviceBackupHostName":
                    if (!string.IsNullOrEmpty(_DeviceBackupHostName))
                    {
                        if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where (s != this && s.Channel == this.Channel &&
                                                        ((s as ModbusTCPSlaveStationSettings).DeviceHostName == _DeviceBackupHostName ||
                                                        (s as ModbusTCPSlaveStationSettings).DeviceBackupHostName == _DeviceBackupHostName) ||
                                                        _DeviceBackupHostName == _DeviceHostName)
                                                        select s).ToList().Count > 0))
                        {
                            return Properties.Resources.SelectExistingDeviceName;
                        }
                    }
                    break;
            }

            return null;
        }

        #endregion

    }
}
