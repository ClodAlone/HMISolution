using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecFXTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecFXTCPChangeTag : ChangeTag
    {
        #region Constructors

        public MelsecFXTCPChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public MelsecFXTCPChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MelsecFXTCPChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
