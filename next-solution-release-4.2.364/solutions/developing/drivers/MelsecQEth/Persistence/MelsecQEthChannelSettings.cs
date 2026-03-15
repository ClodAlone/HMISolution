using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecQEth
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecQEthChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public MelsecQEthChannelSettings(Session session)
            : base(session)
        {
            session.UpdateSchema(typeof(MelsecQEthChannelSettings));
        }

        private MelsecQEthChannelSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 5000;
        }

        public void CopyProperties(MelsecQEthChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region IDataErrorInfo Members
        #endregion

    }
}
