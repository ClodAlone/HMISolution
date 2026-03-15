using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PPI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PPIChangeTag : ChangeTag
    {
        #region Constructors

        public PPIChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public PPIChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PPIChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
