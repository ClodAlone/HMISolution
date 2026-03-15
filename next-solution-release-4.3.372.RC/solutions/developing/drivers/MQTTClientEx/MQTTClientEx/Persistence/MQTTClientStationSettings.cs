using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

namespace MQTTClient
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MQTTClientStationSettings : StationSettings
    {
                #region Constructors

        public MQTTClientStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(MQTTClientStationSettings));
        }
        protected MQTTClientStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            MQTTClientMessageFormat = ((MQTTClientStationSettings)st).MQTTClientMessageFormat;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _MQTTClientMessageFormat = MQTTClientMessageFormats.XML;
        }

        #region Properties

        /// <summary>
        /// Enter the message type
        /// </summary>
        private MQTTClientMessageFormats _MQTTClientMessageFormat;
        public MQTTClientMessageFormats MQTTClientMessageFormat
        {
            get { return _MQTTClientMessageFormat; }
            set { SetPropertyValue("MQTTClientMessageFormat", ref _MQTTClientMessageFormat, value); }
        }

        #endregion



        #region IDataErrorInfo Members

        #endregion

    }
}
