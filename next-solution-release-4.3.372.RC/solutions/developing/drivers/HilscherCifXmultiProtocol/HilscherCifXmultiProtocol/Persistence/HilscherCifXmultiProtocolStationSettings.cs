using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
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
            session.UpdateSchema(typeof(HilscherCifXmultiProtocolStationSettings));
        }

        protected HilscherCifXmultiProtocolStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties (HilscherCifXmultiProtocolStationSettings st)
        {
            base.CopyProperties(st);
            //_BusAddress = st._BusAddress;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            //_BusAddress = 2;
        }


        #region Properties
 
        //private uint _BusAddress;
        //public uint BusAddress
        //{
        //    get { return _BusAddress; }
        //    set { SetPropertyValue("BusAddress", ref _BusAddress, value); ; }
        //}
        
        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
