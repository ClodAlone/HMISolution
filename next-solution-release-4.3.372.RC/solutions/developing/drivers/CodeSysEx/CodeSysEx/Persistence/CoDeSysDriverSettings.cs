using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace CoDeSys
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class CoDeSysDriverSettings : DriverSettings
    {
        #region Constructors

        public CoDeSysDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected CoDeSysDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region IDataErrorInfo Members                
        #endregion

    }
}
