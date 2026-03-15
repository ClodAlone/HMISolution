using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC61850
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC61850ChangeTag : ChangeTag
    {
        #region Constructors

        public IEC61850ChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public IEC61850ChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected IEC61850ChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
