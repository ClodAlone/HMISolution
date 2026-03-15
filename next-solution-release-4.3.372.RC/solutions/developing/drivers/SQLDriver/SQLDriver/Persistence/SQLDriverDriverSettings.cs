using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SQLDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SQLDriverDriverSettings : DriverSettings
    {
        #region Constructors

        public SQLDriverDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SQLDriverDriverSettings()
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

        #region Properties

        #endregion

    }
}
