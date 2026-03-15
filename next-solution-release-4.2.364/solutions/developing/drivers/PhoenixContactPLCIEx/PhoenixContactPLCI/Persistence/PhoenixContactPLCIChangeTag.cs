using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PhoenixContactPLCI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PhoenixContactPLCIChangeTag : ChangeTag
    {
        #region Constructors

        public PhoenixContactPLCIChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public PhoenixContactPLCIChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PhoenixContactPLCIChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
