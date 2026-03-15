using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SNMP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SNMPChangeTag : ChangeTag
    {
        #region Constructors

        public SNMPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public SNMPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected SNMPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
