using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Databoom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DataboomChannelSettings : ChannelSettings
    {
                #region Constructors

        public DataboomChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(DataboomChannelSettings));
        }

        private DataboomChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.Timeout = 30000;
            _DeviceToken = String.Empty;
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

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            switch (propertyName)
            {
                case "DeviceToken":
                    if (string.IsNullOrEmpty(DeviceToken))
                    {
                        return Properties.Resources.DataboomDeviceTokenNotNull;
                    }
                    break;
            }

            return null;
        }

        /// <summary>   Device Token. </summary>
        private string _DeviceToken;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Token. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceToken
        {
            get { return _DeviceToken; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Subscription Key cannot be null");
                //}

                SetPropertyValue("DeviceToken", ref _DeviceToken, value);
            }
        }
        #endregion
    }
}
