using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronEthernetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronEthernetIPDriverSettings : DriverSettings
    {
        #region Constructors

        public OmronEthernetIPDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OmronEthernetIPDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region IDataErrorInfo Members

        #endregion

    }
}
