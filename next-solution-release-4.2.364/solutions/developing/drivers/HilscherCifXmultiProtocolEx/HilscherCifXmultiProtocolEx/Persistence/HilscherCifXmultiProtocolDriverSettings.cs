using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace HilscherCifXmultiProtocol
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class HilscherCifXmultiProtocolDriverSettings : DriverSettings
    {
        #region Constructors

        public HilscherCifXmultiProtocolDriverSettings(Session session)
                : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected HilscherCifXmultiProtocolDriverSettings()
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
