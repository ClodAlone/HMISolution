using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPChangeTag : ChangeTag
    {
        #region Constructors

        public ModbusTCPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public ModbusTCPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusTCPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
