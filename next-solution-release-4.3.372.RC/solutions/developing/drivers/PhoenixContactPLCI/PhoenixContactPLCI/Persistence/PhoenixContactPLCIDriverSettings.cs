using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PhoenixContactPLCI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PhoenixContactPLCIDriverSettings : DriverSettings
    {
        #region Constructors

        public PhoenixContactPLCIDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PhoenixContactPLCIDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region IDataErrorInfo Members                
        #endregion

    }
}
