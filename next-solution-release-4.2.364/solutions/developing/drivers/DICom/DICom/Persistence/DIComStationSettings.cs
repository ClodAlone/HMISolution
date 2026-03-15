using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DICom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DIComStationSettings : StationSettings
    {
        #region Constructors

        public DIComStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(DIComStationSettings));
        }
        protected DIComStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(DIComStationSettings st)
        {
            base.CopyProperties(st);
            DeviceHostName = st.DeviceHostName;
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            DeviceHostName = "";
        }

        #region Properties        
        /// <summary>   Device Host name. </summary>
        private string _DeviceHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Device host. </summary>
        ///
        /// <value> The name of the Device host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set { SetPropertyValue("DeviceHostName", ref _DeviceHostName, value); }
        }

        /// <summary>  The Client-State-Command variable of the station. </summary>
        private UFUAModel.TagEntityReference _ClientStateCommandTag;
        /// <summary>
        /// Gets or sets the the Client-State-Command variable of the station.
        /// </summary>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference ClientStateCommandTag
        {
            get
            {
                return _ClientStateCommandTag;
            }
            set
            {
                SetPropertyValue("ClientStateCommandTag", ref _ClientStateCommandTag, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "DeviceHostName":
                    // if a station was already associated to the channel, all station's must set a not empty DeviceHostName
                    if (DriverSettings != null)
                    {
                        var stations = ((from s in DriverSettings.StationSettings
                                        where (s != this && s.Channel == this.Channel)
                                        select s).ToList());
                        // more stations were associated to channel
                        if (stations.Count > 0)
                        {
                            if (stations.Count(s=> string.IsNullOrWhiteSpace(((DIComStationSettings)s).DeviceHostName)) > 0)
                                return Properties.Resources.ErrorDeviceHostNameEmptyNotAllowed;

                            // check if DeviceHostName is unique when more stations were defined
                            if (!string.IsNullOrWhiteSpace(_DeviceHostName))
                            {
                                if (stations.Count(s => (((DIComStationSettings)s).DeviceHostName.ToLower() == _DeviceHostName.ToLower())) > 0)
                                    return Properties.Resources.ErrorDeviceHostNameEmptyNotAllowed;
                            }
                        }

                        if (stations.Count > 0 && string.IsNullOrWhiteSpace(_DeviceHostName))
                            return Properties.Resources.ErrorDeviceHostNameEmptyNotAllowed;
                    }
                    break;
            }

            return null;
        }

        #endregion

    }
}
