using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DICom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DIComChangeTag : ChangeTag
    {
        #region Constructors

        public DIComChangeTag(Session session, Tag tag)
            : base(session,tag)
        {            
        }

        public DIComChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DIComChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
