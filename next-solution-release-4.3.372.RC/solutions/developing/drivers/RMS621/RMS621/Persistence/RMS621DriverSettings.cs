using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace RMS621
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class RMS621DriverSettings : DriverSettings
    {
        #region Constructors

        public RMS621DriverSettings(Session session)
            : base(session)
        {
        }

        protected RMS621DriverSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            /*if (propertyName == "Name")
            {
            }*/

            return null;
        }

        #endregion
    }
}
