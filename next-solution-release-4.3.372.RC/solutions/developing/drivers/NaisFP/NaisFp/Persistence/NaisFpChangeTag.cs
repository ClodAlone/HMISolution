using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace NaisFp
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class NaisFpChangeTag : ChangeTag
    {
        #region Constructors

        public NaisFpChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public NaisFpChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected NaisFpChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
