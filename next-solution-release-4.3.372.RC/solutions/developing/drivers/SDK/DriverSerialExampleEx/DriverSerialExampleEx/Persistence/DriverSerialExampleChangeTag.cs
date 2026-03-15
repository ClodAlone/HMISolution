using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverSerialExample
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverSerialExampleChangeTag : ChangeTag
    {
        #region Constructors

        public DriverSerialExampleChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public DriverSerialExampleChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DriverSerialExampleChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
