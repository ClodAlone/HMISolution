using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoChannelSettings : ChannelSettings
    {
        #region Constructors

        public DemoChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private DemoChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion
         //steve 080711
        public void CopyProperties(DemoChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }
        #region Properties

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
