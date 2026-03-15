using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPStationSettings : StationSettings
    {
                #region Constructors

        public SNMPStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SNMPStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(SNMPStationSettings st)
        {
            base.CopyProperties(st);
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members
        #endregion

    }
}
