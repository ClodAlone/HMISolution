using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFXTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXTCPDriverSettings : DriverSettings
    {
        #region Constructors

        public MelsecFXTCPDriverSettings(Session session)
            : base(session)
        {
        }

        protected MelsecFXTCPDriverSettings()
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
