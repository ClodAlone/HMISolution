using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;

namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    internal sealed class DemoCommJobSettings : CommJobSettings
    {
        #region Constructors

        public DemoCommJobSettings(Session session, DemoCommJob job)
            : base(session, job)
        {
            _DemoType = job.DemoType;
        }
        
        protected DemoCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected DemoCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties
        private DemoTypes _DemoType;
        public DemoTypes DemoType
        {
            get
            {
                return _DemoType;
            }
            set
            {
                SetPropertyValue("DemoCode", ref _DemoType, value);
            }
        }
        #endregion

    }
}
