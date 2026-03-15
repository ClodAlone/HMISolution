using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SaiaDataMode
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SaiaDataModeChangeTag : ChangeTag
    {
        #region Constructors

        public SaiaDataModeChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public SaiaDataModeChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SaiaDataModeChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
