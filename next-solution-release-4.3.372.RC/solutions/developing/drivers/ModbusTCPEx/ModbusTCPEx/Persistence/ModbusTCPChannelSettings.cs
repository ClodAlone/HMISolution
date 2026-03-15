using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

namespace ModbusTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPChannelSettings : TcpChannelSettings
    {
                #region Constructors

        public ModbusTCPChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private ModbusTCPChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 502;
            _TurnaroundDelay = 0;
        }

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            TurnaroundDelay = ((ModbusTCPChannelSettings)ch).TurnaroundDelay;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Enter the time, in millisecond, to wait after broadcast tasks have been excetuted
        /// </summary>
        private uint _TurnaroundDelay;
        public uint TurnaroundDelay
        {
            get
            {
                return _TurnaroundDelay;
            }
            set
            {
                SetPropertyValue("TurnaroundDelay", ref _TurnaroundDelay, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TurnaroundDelay")
            {
                if (TurnaroundDelay > uint.MaxValue)
                {
                    return string.Format(Properties.Resources.TurnaroundDelayOutOfRange, uint.MaxValue);
                }
            }

            return null;
        }

        #endregion
    
    }
}
