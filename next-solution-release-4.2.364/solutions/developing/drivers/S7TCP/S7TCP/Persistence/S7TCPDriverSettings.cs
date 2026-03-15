using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TCPDriverSettings : DriverSettings
    {
        #region Constructors

        public S7TCPDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected S7TCPDriverSettings()
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
