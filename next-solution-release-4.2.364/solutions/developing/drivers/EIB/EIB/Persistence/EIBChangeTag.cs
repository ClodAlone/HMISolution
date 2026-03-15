using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EIB
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EIBChangeTag : ChangeTag
    {
        #region Constructors

        public EIBChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public EIBChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EIBChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
