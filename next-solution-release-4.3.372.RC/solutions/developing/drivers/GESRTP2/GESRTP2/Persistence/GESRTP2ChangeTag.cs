using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace GESRTP2
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class GESRTP2ChangeTag : ChangeTag
    {
        #region Constructors

        public GESRTP2ChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public GESRTP2ChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected GESRTP2ChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
