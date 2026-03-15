using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoDriverSettings : DriverSettings
    {
        #region Constructors

        public DemoDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DemoDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
