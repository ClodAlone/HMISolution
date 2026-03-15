using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EtherNetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EtherNetIPDriverSettings : DriverSettings
    {
        #region Constructors

        public EtherNetIPDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EtherNetIPDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
