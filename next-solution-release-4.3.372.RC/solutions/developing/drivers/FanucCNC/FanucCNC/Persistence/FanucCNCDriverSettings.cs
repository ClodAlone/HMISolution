using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace FanucCNC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FanucCNCDriverSettings : DriverSettings
    {
        #region Constructors

        public FanucCNCDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FanucCNCDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region IDataErrorInfo Members                
        #endregion

    }
}
