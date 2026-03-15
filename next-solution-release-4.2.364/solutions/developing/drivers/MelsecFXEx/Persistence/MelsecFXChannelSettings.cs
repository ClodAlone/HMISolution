using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFX
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXChannelSettings : SerialChannelSettings
    {
        #region Constructors

        public MelsecFXChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private MelsecFXChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            CommPortName = "Com1";
            CommPortBaudRate = 9600;
            CommPortDataBits = 7;
            CommPortParity = (int)System.IO.Ports.Parity.Even;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;
        }

        public void CopyProperties(MelsecFXChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region IDataErrorInfo Members
        #endregion
    }
}
