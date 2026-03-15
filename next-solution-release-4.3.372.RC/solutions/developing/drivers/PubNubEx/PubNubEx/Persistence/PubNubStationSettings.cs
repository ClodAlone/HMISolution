using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PubNub
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PubNubStationSettings : StationSettings
    {
                #region Constructors

        public PubNubStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(PubNubStationSettings));
        }
        protected PubNubStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
        }
        //////////////////

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
