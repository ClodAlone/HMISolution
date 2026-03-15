using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PubNub
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PubNubChannelSettings : ChannelSettings
    {
                #region Constructors

        public PubNubChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(PubNubChannelSettings));
        }

        private PubNubChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        //steve 080711
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
        }
        /////////////////////////////

        #region Properties

        #endregion


        #region IDataErrorInfo Members

        #endregion
    
    }
}
