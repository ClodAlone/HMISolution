using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPDriverSettings : DriverSettings
    {
        #region Constructors

        public SNMPDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SNMPDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion
        public void DefaultSettings()
        {
            base.DefaultSettings();
        }
    }
}
