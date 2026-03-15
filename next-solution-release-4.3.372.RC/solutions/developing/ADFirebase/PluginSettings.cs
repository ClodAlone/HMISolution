using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ADFirebase
{
    [Persistent("ADFirebasePluginSettings")]
    public class PluginSettings : XPObject, IDataErrorInfo
    {
        #region Ctor
        public PluginSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected PluginSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Methods
        public void DefaultSettings()
        {
            _serveiceAccountKeyFile = string.Empty;
        }
        #endregion

        #region Properties

        private string _serveiceAccountKeyFile;
        [Size(SizeAttribute.Unlimited)]
        public string ServeiceAccountKeyFile
        {
            get { return _serveiceAccountKeyFile; }
            set
            {
                SetPropertyValue("ServeiceAccountKey", ref _serveiceAccountKeyFile, value);
            }
        }

        private string _realtimeDatabaseUrl;
        [Size(SizeAttribute.Unlimited)]
        public string RealtimeDatabaseUrl
        {
            get => _realtimeDatabaseUrl;
            set => SetPropertyValue("RealtimeDatabaseUrl", ref _realtimeDatabaseUrl, value);
        }

        private string _notificationsTitle;
        [Size(SizeAttribute.Unlimited)]
        public string NotificationsTitle
        {
            get => _notificationsTitle;
            set => SetPropertyValue("NotificationsTitle", ref _notificationsTitle, value);
        }

        private bool _useAccessToken;
        public bool UseAccessToken
        {
            get => _useAccessToken;
            set => SetPropertyValue("UseAccessToken", ref _useAccessToken, value);
        }
        #endregion

        #region IDataErrorInfo Members

        const string ValidPathRegex = @"^(.+)\/([^\/]+)$";



        #endregion
        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region IDataErrorInfo Members
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        protected String PerformValidation(String propertyName)
        {

            switch (propertyName)
            {
                case "ServeiceAccountKey":
                    if (string.IsNullOrEmpty(ServeiceAccountKeyFile))
                        return Properties.Resources.ErrServiceAccountKeyMustBePresent;
                    break;
            }
            return null;
        }

        #endregion
    }
}
