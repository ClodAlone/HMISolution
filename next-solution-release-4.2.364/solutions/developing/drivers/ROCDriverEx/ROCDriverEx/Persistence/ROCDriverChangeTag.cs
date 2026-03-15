using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ROCDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ROCDriverChangeTag : ChangeTag
    {
        #region Constructors

        public ROCDriverChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public ROCDriverChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ROCDriverChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
