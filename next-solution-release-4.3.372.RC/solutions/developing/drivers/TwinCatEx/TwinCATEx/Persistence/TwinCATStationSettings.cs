using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace TwinCAT
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class TwinCATStationSettings : StationSettings
    {
        #region Constructors

        public TwinCATStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected TwinCATStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
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
