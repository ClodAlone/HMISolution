using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace FanucCNC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FanucCNCChangeTag : ChangeTag
    {
        #region Constructors

        public FanucCNCChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public FanucCNCChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FanucCNCChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
