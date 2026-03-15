using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace CoDeSys
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class CoDeSysChangeTag : ChangeTag
    {
        #region Constructors

        public CoDeSysChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public CoDeSysChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected CoDeSysChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
