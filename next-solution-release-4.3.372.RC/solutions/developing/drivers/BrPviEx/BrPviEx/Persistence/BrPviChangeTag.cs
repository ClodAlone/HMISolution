using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BrPvi
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BrPviChangeTag : ChangeTag
    {
        #region Constructors

        public BrPviChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public BrPviChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected BrPviChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
