using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SaiaDataMode
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SaiaDataModeDriverSettings : DriverSettings
    {
                #region Constructors

        public SaiaDataModeDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SaiaDataModeDriverSettings()
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
        #endregion
    
    }
}
