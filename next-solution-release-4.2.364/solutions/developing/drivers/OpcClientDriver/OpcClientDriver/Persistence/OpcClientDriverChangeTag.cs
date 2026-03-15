using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OpcClientDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OpcClientDriverChangeTag : ChangeTag
    {
        #region Constructors

        public OpcClientDriverChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public OpcClientDriverChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OpcClientDriverChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
