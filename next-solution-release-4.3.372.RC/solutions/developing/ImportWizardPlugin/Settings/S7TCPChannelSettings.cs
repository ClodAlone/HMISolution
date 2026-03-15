using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using IpDriverCodeBase;

namespace ImportWizardPlugin.Settings
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TCPChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public S7TCPChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(TcpChannelSettings));
        }

        private S7TCPChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 102;
            _DeviceID = 1;
            _Rack = 0;
            _Slot = 0;
        }

        public void CopyProperties(S7TCPChannelSettings ch)
        {
            base.CopyProperties(ch);
            _DeviceID = ch.DeviceID;
            _Rack = ch.Rack;
            _Slot = ch.Slot;
        }

        #region Properties
        private byte _DeviceID;
        public byte DeviceID
        {
            get { return _DeviceID; }
            set { SetPropertyValue("DeviceID", ref _DeviceID, value); }
        }
        private byte _Rack;
        public byte Rack
        {
            get { return _Rack; }
            set { SetPropertyValue("Rack", ref _Rack, value); }
        }
        private byte _Slot;
        public byte Slot
        {
            get { return _Slot; }
            set { SetPropertyValue("Slot", ref _Slot, value); }
        }

        #endregion
    }
}
