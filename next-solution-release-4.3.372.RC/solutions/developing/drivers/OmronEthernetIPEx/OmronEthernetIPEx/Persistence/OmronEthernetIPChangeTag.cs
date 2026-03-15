using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronEthernetIP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronEthernetIPChangeTag : ChangeTag
    {
        #region Constructors

        public OmronEthernetIPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public OmronEthernetIPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OmronEthernetIPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
