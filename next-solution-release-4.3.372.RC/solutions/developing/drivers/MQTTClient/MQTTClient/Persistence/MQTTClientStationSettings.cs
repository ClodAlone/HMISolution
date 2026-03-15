using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

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
        }
        protected MQTTClientStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public void CopyProperties(MQTTClientStationSettings st)
        {
            base.CopyProperties(st);
            MQTTClientMessageFormat = st.MQTTClientMessageFormat;
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
