using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MTConnect
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MTConnectStationSettings : StationSettings
    {
                #region Constructors

        public MTConnectStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(MTConnectStationSettings));
        }
        protected MTConnectStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        //steve 080711
        public void CopyProperties(MTConnectStationSettings st)
        {
            base.CopyProperties(st);
        }
        //////////////////

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.MaxRetriesBeforeError = 0;
            _DeviceId = String.Empty;
            _MaxNumberOfItemsForRequest = 25;
        }

        #region Properties

        /// <summary>   Server Adress. </summary>
        private string _DeviceId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Adress. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceId
        {
            get { return _DeviceId; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("DeviceId;", ref _DeviceId, value);
            }
        }

        /// <summary>   Server Port. </summary>
        private uint _MaxNumberOfItemsForRequest;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Port. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxNumberOfItemsForRequest
        {
            get { return _MaxNumberOfItemsForRequest; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("MaxNumberOfItemsForRequest;", ref _MaxNumberOfItemsForRequest, value);
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
                case "DeviceId":
                    if (string.IsNullOrEmpty(DeviceId))
                    {
                        return Properties.Resources.MTConnectDeviceTokenNotNull;
                    }
                    break;
                case "MaxNumberOfItemsForRequest":
                    {
                        if (MaxNumberOfItemsForRequest > Properties.Settings.Default.MaxItemForMessage)
                        {
                            MaxNumberOfItemsForRequest = Properties.Settings.Default.DefaultItemForMessage;
                            return String.Format(Properties.Resources.MTCconnectLimitItemsForRequest, Properties.Settings.Default.MaxItemForMessage);
                        }
                    }
                    break;
            }
            return null;
        }
        #endregion

    }
}
