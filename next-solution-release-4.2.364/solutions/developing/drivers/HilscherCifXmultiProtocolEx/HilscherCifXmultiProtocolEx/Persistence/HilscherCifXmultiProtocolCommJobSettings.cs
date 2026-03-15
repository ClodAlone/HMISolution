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
    public class HilscherCifXmultiProtocolCommJobSettings : CommJobSettings
    {
        #region Constructors

        public HilscherCifXmultiProtocolCommJobSettings(Session session, HilscherCifXmultiProtocolCommJob job)
            : base(session, job)
        {
            _DataAddress = job.DataAddress;
        }
        
        public HilscherCifXmultiProtocolCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected HilscherCifXmultiProtocolCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _DataAddress = 0;
        }

        #region Properties

        private uint _DataAddress;
        public uint DataAddress
        {
            get { return _DataAddress; }
            set { SetPropertyValue("DataAddress", ref _DataAddress, value); }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
