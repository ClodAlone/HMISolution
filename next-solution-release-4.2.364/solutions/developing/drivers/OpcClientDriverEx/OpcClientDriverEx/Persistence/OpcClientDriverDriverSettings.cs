using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OpcClientDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OpcClientDriverDriverSettings : DriverSettings
    {
        #region Constructors

        public OpcClientDriverDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(OpcClientDriverDriverSettings));
        }
        protected OpcClientDriverDriverSettings()
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

        #region Properties

        private bool _DoNotUseServerRedundancy;

        public bool DoNotUseServerRedundancy
        {
            get { return _DoNotUseServerRedundancy; }

            set { SetPropertyValue("DoNotUseServerRedundancy", ref _DoNotUseServerRedundancy, value); }
        }
        #endregion
    }
}
