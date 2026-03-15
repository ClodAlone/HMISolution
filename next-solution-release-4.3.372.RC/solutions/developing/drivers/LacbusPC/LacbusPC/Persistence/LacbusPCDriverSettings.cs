using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace LacbusPC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class LacbusPCDriverSettings : DriverSettings
    {
        #region Constructors

        public LacbusPCDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected LacbusPCDriverSettings()
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
