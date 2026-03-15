using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFX
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXDriverSettings : DriverSettings
    {
        #region Constructors

        public MelsecFXDriverSettings(Session session)
            : base(session)
        {
        }

        protected MelsecFXDriverSettings()
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
