using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFX
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXChangeTag : ChangeTag
    {
        #region Constructors

        public MelsecFXChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public MelsecFXChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MelsecFXChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
