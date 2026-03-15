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
    public class ModbusTCPChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public ModbusTCPChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(TcpChannelSettings));
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
            //_FrameType = 0;
        }

        //steve 080711
        public void CopyProperties(ModbusTCPChannelSettings ch)
        {
            base.CopyProperties(ch);
            //FrameType = ch.FrameType;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// Enter the frame type of the data exchange telegrams
        /// </summary>
        /*private byte _FrameType;
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
        }*/


        #endregion

    }
}
