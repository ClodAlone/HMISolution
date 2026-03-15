using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModBus
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModBusChangeTag : ChangeTag
    {
        #region Constructors

        public ModBusChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public ModBusChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ModBusChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
