using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFXTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXTCPStationSettings : StationSettings
    {
        #region Constructors

        public MelsecFXTCPStationSettings(Session session)
            : base(session)
        {
            MaxRetriesBeforeError = 0;
        }

        protected MelsecFXTCPStationSettings()
        {
            MaxRetriesBeforeError = 0;
        }

        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            MaxRetriesBeforeError = 0;
        }

        #region IDataErrorInfo Members
        #endregion
    }
}
