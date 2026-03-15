using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PubNub
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PubNubDriverSettings : DriverSettings
    {
        #region Constructors

        public PubNubDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PubNubDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _PublishKey = String.Empty;
            _SubscribeKey = String.Empty;
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            /*if (propertyName == "Name")
            {
            }*/
            switch(propertyName)
            {
                case "PublishKey":
                    if (string.IsNullOrEmpty(PublishKey))
                    {
                        return Properties.Resources.PubNubPublishKeyNotNull;
                    }
                    break;
                case "SubscribeKey":
                    if (string.IsNullOrEmpty(SubscribeKey))
                    {
                        return Properties.Resources.PubNubSubscribeKeyNotNull;
                    }
                    break;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>   Publish Key. </summary>
        private string _PublishKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Publish Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string PublishKey
        {
            get { return _PublishKey; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("PublishKey", ref _PublishKey, value);
            }
        }

        /// <summary>   Subscription Key. </summary>
        private string _SubscribeKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Subscription Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SubscribeKey
        {
            get { return _SubscribeKey; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Subscription Key cannot be null");
                //}

                SetPropertyValue("SubscribeKey", ref _SubscribeKey, value);
            }
        }
        #endregion

    }
}
