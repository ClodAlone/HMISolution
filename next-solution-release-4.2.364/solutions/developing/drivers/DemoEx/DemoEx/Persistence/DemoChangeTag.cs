using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoChangeTag : ChangeTag
    {
        #region Constructors

        public DemoChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public DemoChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DemoChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
