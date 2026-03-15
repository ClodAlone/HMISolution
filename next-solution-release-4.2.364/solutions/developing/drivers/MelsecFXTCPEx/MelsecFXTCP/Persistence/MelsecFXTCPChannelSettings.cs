using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFXTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXTCPChannelSettings : TcpChannelSettings
    {
        #region Constructors

        public MelsecFXTCPChannelSettings(Session session)
            : base(session)
        {
        }

        private MelsecFXTCPChannelSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.TcpChannelSettingsHostPort = 1025;
        }

        public void CopyProperties(MelsecFXTCPChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region IDataErrorInfo Members
        #endregion

    }
}
