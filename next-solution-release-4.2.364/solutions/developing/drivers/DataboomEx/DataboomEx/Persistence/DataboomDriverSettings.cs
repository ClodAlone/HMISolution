using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Databoom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DataboomDriverSettings : DriverSettings
    {
        #region Constructors

        public DataboomDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DataboomDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _UrlToPostTo = "https://api.databoom.com/v1/signals/push";
            _UrlClockPost = "https://api.databoom.com/v1/auth/gmtclock/plain";
            _ApiKey = String.Empty;
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
                case "UrlToPostTo":
                    if (string.IsNullOrEmpty(UrlToPostTo))
                    {
                        return Properties.Resources.DataboomUrlToPostToNotNull;
                    }
                    break;
                case "UrlClockPost":
                    if (string.IsNullOrEmpty(UrlClockPost))
                    {
                        return Properties.Resources.DataboomUrlClockPostNotNull;
                    }
                    break;
                case "ApiKey":
                    if (string.IsNullOrEmpty(ApiKey))
                    {
                        return Properties.Resources.DataboomApiKeyNotNull;
                    }
                    break;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>   Url To Post To. </summary>
        private string _UrlToPostTo;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Url To Post To. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UrlToPostTo
        {
            get { return _UrlToPostTo; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Publish Key cannot be null");
                //}

                SetPropertyValue("UrlToPostTo", ref _UrlToPostTo, value);
            }
        }

        /// <summary>   Url Clock Post. </summary>
        private string _UrlClockPost;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Url Clock Post. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UrlClockPost
        {
            get { return _UrlClockPost; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Subscription Key cannot be null");
                //}

                SetPropertyValue("UrlClockPost", ref _UrlClockPost, value);
            }
        }

        /// <summary>   Api Key. </summary>
        private string _ApiKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Api Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ApiKey
        {
            get { return _ApiKey; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("The Subscription Key cannot be null");
                //}

                SetPropertyValue("ApiKey", ref _ApiKey, value);
            }
        }

        #endregion

    }
}
