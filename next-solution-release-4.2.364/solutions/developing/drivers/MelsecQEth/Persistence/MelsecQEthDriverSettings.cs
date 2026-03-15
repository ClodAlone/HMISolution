using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecQEth
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecQEthDriverSettings : DriverSettings
    {
        #region Constructors

        public MelsecQEthDriverSettings(Session session)
            : base(session)
        {
        }

        protected MelsecQEthDriverSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members
        #endregion

    }
}
