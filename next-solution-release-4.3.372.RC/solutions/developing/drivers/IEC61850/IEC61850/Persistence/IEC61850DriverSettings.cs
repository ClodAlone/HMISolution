using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC61850
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC61850DriverSettings : DriverSettings
    {
        #region Constructors

        public IEC61850DriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected IEC61850DriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
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
