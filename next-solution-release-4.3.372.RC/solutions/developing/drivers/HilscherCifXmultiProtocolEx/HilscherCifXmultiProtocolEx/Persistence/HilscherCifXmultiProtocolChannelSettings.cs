using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace HilscherCifXmultiProtocol
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class HilscherCifXmultiProtocolChannelSettings : ChannelSettings
    {
        #region Constructors

        public HilscherCifXmultiProtocolChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private HilscherCifXmultiProtocolChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
         
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _BoardNumber = 0;
            _ChannelNumber = 0;
            _ForceOutputDataAtStartup = false; 
        }

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            _BoardNumber = ((HilscherCifXmultiProtocolChannelSettings)ch).BoardNumber;
            _ChannelNumber = ((HilscherCifXmultiProtocolChannelSettings)ch).ChannelNumber;
            _ForceOutputDataAtStartup = ((HilscherCifXmultiProtocolChannelSettings)ch).ForceOutputDataAtStartup;
        }

        #region Properties

        private uint _BoardNumber;
        public uint BoardNumber
        {
            get { return _BoardNumber; }
            set { SetPropertyValue("BoardNumber", ref _BoardNumber, value); }
        }

        private uint _ChannelNumber;
        public uint ChannelNumber
        {
            get { return _ChannelNumber; }
            set { SetPropertyValue("ChannelNumber", ref _ChannelNumber, value); }
        }

        private bool _ForceOutputDataAtStartup;
        public bool ForceOutputDataAtStartup
        {
            get { return _ForceOutputDataAtStartup; }
            set { SetPropertyValue("ForceOutputDataAtStartup", ref _ForceOutputDataAtStartup, value); }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
