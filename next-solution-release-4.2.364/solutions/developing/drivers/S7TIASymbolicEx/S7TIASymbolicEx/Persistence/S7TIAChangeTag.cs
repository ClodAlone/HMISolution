using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace S7TIASymbolic
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class S7TIASymbolicChangeTag : ChangeTag
    {
        #region Constructors

        public S7TIASymbolicChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public S7TIASymbolicChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected S7TIASymbolicChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
