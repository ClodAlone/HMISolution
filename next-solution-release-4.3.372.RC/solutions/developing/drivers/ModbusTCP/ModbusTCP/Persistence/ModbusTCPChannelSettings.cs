using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

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
            session.UpdateSchema(typeof(ModbusTCPChannelSettings));
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

        //steve 080711
        public void CopyProperties(ModbusTCPChannelSettings ch)
        {
            base.CopyProperties(ch);
            TurnaroundDelay = ch.TurnaroundDelay;
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
