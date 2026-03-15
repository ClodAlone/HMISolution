using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCPSlave
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPSlaveChangeTag : ChangeTag
    {
        #region Constructors

        public ModbusTCPSlaveChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public ModbusTCPSlaveChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModbusTCPSlaveChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
