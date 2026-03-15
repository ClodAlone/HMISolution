using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PubNub
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PubNubChangeTag : ChangeTag
    {
        #region Constructors

        public PubNubChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public PubNubChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PubNubChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
