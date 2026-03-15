using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EtherNetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EtherNetIPChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public EtherNetIPChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private EtherNetIPChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 0xAF12;
        }

        public void CopyProperties(EtherNetIPChannelSettings ch)
        {
            base.CopyProperties(ch);
        }
        /////////////////////////////

        #region Properties

 


        #endregion


        #region IDataErrorInfo Members
        #endregion

    }
}
