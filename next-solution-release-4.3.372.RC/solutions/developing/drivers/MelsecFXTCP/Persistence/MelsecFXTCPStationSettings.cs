using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
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
            session.UpdateSchema(typeof(MelsecFXTCPStationSettings));
            MaxRetriesBeforeError = 0;
        }

        protected MelsecFXTCPStationSettings()
        {
            MaxRetriesBeforeError = 0;
        }

        #endregion

        public void CopyProperties(MelsecFXTCPStationSettings st)
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
