using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BACnet
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BACnetChangeTag : ChangeTag
    {
        #region Constructors

        public BACnetChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public BACnetChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected BACnetChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
