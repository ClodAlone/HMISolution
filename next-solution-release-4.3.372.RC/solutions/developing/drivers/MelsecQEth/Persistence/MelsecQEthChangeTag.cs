using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecQEth
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecQEthChangeTag : ChangeTag
    {
        #region Constructors

        public MelsecQEthChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public MelsecQEthChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MelsecQEthChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion
    }
}
