using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TCPChangeTag : ChangeTag
    {
        #region Constructors

        public S7TCPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public S7TCPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected S7TCPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
