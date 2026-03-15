using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ADTelegram
{
    [Persistent("ADTelegramPluginSettings")]
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
            _ChatID = string.Empty;
            _Token = string.Empty;
            _PhoneNumber = string.Empty;
            _Code = string.Empty;
        }
        #endregion

        #region Properties

        private string _Token;
        public string Token
        {
            get { return _Token; }
            set
            {
                SetPropertyValue("Api_Hash", ref _Token, value);
            }
        }

        private string _ChatID;
        public string ChatID
        {
            get { return _ChatID; }
            set
            {
                SetPropertyValue("Api_Key", ref _ChatID, value);
            }
        }

        private string _PhoneNumber;
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set
            {
                SetPropertyValue("PhoneNumber", ref _PhoneNumber, value);
            }
        }

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        #endregion
        #region IDataErrorInfo Members

        const string ValidPhoneNumber = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";



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

                case "Api_Key":
                    break;
                case "Api_Hash":
                    break;
                case "PhoneNumber":
                    if (!Regex.IsMatch(PhoneNumber, ValidPhoneNumber))
                        return Properties.Resources.ADTelegramErrorWrongNumber;
                    break;
                case "Code":
                    break;
            }
            return null;
        }

        #endregion
    }
}
