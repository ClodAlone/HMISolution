using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace HilscherCifXmultiProtocol
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class HilscherCifXmultiProtocolStationSettings : StationSettings
    {
        #region Constructors

        public HilscherCifXmultiProtocolStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected HilscherCifXmultiProtocolStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties (StationSettings st)
        {
            base.CopyProperties(st);
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }


        #region Properties
        
        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
