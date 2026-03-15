using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCPSlave
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPSlaveChannelSettings : TcpChannelSettings
    {
                #region Constructors

        public ModbusTCPSlaveChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private ModbusTCPSlaveChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 502;
            _InactivityTimeout = 300; 
            //_FrameType = 0;
        }

        public void CopyProperties(ModbusTCPSlaveChannelSettings ch)
        {
            base.CopyProperties(ch);
            _InactivityTimeout = ch.InactivityTimeout;
        }

        #region Properties

        /// <summary>
        /// Enter the master no activity Timeout (sec)
        /// </summary>
        private uint _InactivityTimeout;
        public uint InactivityTimeout
        {
            get
            {
                return _InactivityTimeout;
            }
            set
            {
                SetPropertyValue("InactivityTimeoutXX", ref _InactivityTimeout, value);
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
                case "InactivityTimeout":
                    //InactivityTimeout
                    break;
                case "TcpChannelSettingsHostName":
                    if (!string.IsNullOrEmpty(TcpChannelSettingsHostName))
                    {
                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where TcpChannelSettingsHostName == TcpChannelSettingsBackupHostName || (c != this &&
                                                        ((c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsHostPort == TcpChannelSettingsHostPort &&
                                                        ((c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsHostName == TcpChannelSettingsHostName ||
                                                        (c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsBackupHostName == TcpChannelSettingsHostName)))
                                                        
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.SelectExistingDeviceName;
                        }
                    }
                    break;
                case "TcpChannelSettingsBackupHostName":
                    if (!string.IsNullOrEmpty(TcpChannelSettingsBackupHostName))
                    {
                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where TcpChannelSettingsHostName == TcpChannelSettingsBackupHostName || (c != this &&
                                                        ((c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsHostPort == TcpChannelSettingsHostPort &&
                                                        ((c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsHostName == TcpChannelSettingsBackupHostName ||
                                                        (c as ModbusTCPSlaveChannelSettings).TcpChannelSettingsBackupHostName == TcpChannelSettingsBackupHostName)))
                                                        select c).ToList().Count > 0))
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
