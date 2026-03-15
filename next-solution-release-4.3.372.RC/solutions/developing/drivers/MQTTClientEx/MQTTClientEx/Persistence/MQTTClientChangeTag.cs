using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MQTTClient
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MQTTClientChangeTag : ChangeTag
    {
        #region Constructors

        public MQTTClientChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public MQTTClientChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected MQTTClientChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
