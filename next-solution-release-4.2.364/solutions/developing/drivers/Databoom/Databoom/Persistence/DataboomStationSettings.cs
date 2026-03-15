using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Databoom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DataboomStationSettings : StationSettings
    {
                #region Constructors

        public DataboomStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DataboomStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public void CopyProperties(DataboomStationSettings st)
        {
            base.CopyProperties(st);
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.MaxRetriesBeforeError = 0;
        }

        #region Properties

        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
