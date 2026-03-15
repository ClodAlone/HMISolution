using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverSerialExample
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverSerialExampleChannelSettings : SerialChannelSettings
    {
        #region Constructors

        public DriverSerialExampleChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private DriverSerialExampleChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            CommPortName = "Com1";
            CommPortBaudRate = 19200;
            CommPortDataBits = 8;
            CommPortParity = (int)System.IO.Ports.Parity.None;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;
            _FrameType = 0;
            _TurnaroundDelay = 2000;
        }

        //steve 080711
        public void CopyProperties(DriverSerialExampleChannelSettings ch)
        {
            base.CopyProperties(ch);
            FrameType = ch.FrameType;
            TurnaroundDelay = ch.TurnaroundDelay;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Enter the frame type of the data exchange telegrams
        /// </summary>
        private byte _FrameType;
        public byte FrameType
        {
            get
            {
                return _FrameType;
            }
            set
            {
                SetPropertyValue("FrameType", ref _FrameType, value);
            }
        }

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
            
            if (propertyName == "FrameType")
            {
                //FrameType
            }
            else if (propertyName == "TurnaroundDelay")
            {
                if(TurnaroundDelay > uint.MaxValue)
                    return string.Format(Properties.Resources.TurnaroundDelayOutOfRange, uint.MaxValue);
            }
        

            return null;
        }

        #endregion
    }
}
