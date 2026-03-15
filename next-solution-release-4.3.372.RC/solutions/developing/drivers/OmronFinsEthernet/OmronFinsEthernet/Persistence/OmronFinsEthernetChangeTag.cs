using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace OmronFinsEthernet
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OmronFinsEthernetChangeTag : ChangeTag
    {
        #region Constructors

        public OmronFinsEthernetChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public OmronFinsEthernetChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected OmronFinsEthernetChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
