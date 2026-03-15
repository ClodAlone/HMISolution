using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SQLDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SQLDriverChangeTag : ChangeTag
    {
        #region Constructors

        public SQLDriverChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public SQLDriverChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SQLDriverChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
