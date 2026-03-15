using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace EtherNetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EtherNetIPChangeTag : ChangeTag
    {
        #region Constructors

        public EtherNetIPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public EtherNetIPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EtherNetIPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
