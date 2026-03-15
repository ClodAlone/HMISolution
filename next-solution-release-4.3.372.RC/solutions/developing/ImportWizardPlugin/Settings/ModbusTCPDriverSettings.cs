using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBase;

namespace ImportWizardPlugin.Settings
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPDriverSettings : DriverSettings
    {
        #region Constructors

        public ModbusTCPDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusTCPDriverSettings()
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
