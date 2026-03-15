using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverTcpExample
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverTcpExampleChangeTag : ChangeTag
    {
        #region Constructors

        public DriverTcpExampleChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public DriverTcpExampleChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DriverTcpExampleChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
