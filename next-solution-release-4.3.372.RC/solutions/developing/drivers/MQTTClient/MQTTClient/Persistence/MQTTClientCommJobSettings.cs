using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MQTTClient
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MQTTClientCommJobSettings : CommJobSettings
    {
                #region Constructors

        public MQTTClientCommJobSettings(Session session, MQTTClientCommJob job)
            : base(session, job)
        {
            _TagName = job.TagName;
            _Retained = job.Retained;
            _QualityOfServiceLevel = job.QualityOfServiceLevel;
            _JsonMessageFormat = job.JsonMessageFormat;
            _JsonMessageTimestampField = job.JsonMessageTimestampField;
            _JsonTimestampFormat = job.JsonTimestampFormat;
            _JsonUseLocalTime = job.JsonUseLocalTime;
            _HysteresisThreshold = job.HysteresisThreshold;
        }

        public MQTTClientCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected MQTTClientCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TagName = String.Empty;
            _Retained = true;
            _QualityOfServiceLevel = QualityOfServiceLevels.AtMostOnce_0;
            _JsonMessageFormat = String.Empty;
            _JsonMessageTimestampField = String.Empty;
            _JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            _JsonUseLocalTime = false;
            _HysteresisThreshold = 0.0;
        }

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _TagName;
        [Size(SizeAttribute.Unlimited)]
        public string TagName
        {
            get
            {
                return _TagName;
            }

            set
            {
                SetPropertyValue("TagName", ref _TagName, value);
            }
        }

        /// <summary>
        /// Retain flag for PUBLISH message
        /// </summary>
        private bool _Retained;
        public bool Retained
        {
            get { return _Retained; }
            set
            {
                SetPropertyValue("Retained", ref _Retained, value);
            }
        }

        /// <summary>
        /// Quality Of Service level for PUBLISH and SUBSCRIBE messages
        /// </summary>
        private QualityOfServiceLevels _QualityOfServiceLevel;
        public QualityOfServiceLevels QualityOfServiceLevel
        {
            get { return _QualityOfServiceLevel; }
            set
            {
                SetPropertyValue("QualityOfServiceLevel", ref _QualityOfServiceLevel, value);
            }
        }

        /// <summary>
        /// Format of Json messages
        /// </summary>
        private string _JsonMessageFormat;
        public string JsonMessageFormat
        {
            get { return _JsonMessageFormat; }
            set { SetPropertyValue("JsonMessageFormat", ref _JsonMessageFormat, value); }
        }

        /// <summary>
        /// Timestamp of Json messages
        /// </summary>
        private string _JsonMessageTimestampField;
        public string JsonMessageTimestampField
        {
            get { return _JsonMessageTimestampField; }
            set { SetPropertyValue("JsonMessageTimestampField", ref _JsonMessageTimestampField, value); }
        }

        /// <summary>
        /// Time format for the timestamp of the JSon message
        /// </summary>
        private JSonTimestampFormats _JsonTimestampFormat;
        public JSonTimestampFormats JsonTimestampFormat
        {
            get { return _JsonTimestampFormat; }
            set
            {
                SetPropertyValue("JsonTimestampFormat", ref _JsonTimestampFormat, value);
            }
        }

        /// <summary>
        /// "Use Local Time" flag for the timestamp info of a JSon message
        /// </summary>
        private bool _JsonUseLocalTime;
        public bool JsonUseLocalTime
        {
            get { return _JsonUseLocalTime; }
            set
            {
                SetPropertyValue("JsonUseLocalTime", ref _JsonUseLocalTime, value);
            }
        }

        /// <summary>
        /// Hysteresis Threshold
        /// </summary>
        private double _HysteresisThreshold;
        public double HysteresisThreshold
        {
            get { return _HysteresisThreshold; }
            set {
                SetPropertyValue("HysteresisThreshold", ref _HysteresisThreshold, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members
        #endregion

    }
}
