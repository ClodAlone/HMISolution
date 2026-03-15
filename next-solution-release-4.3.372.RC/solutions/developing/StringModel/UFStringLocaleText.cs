using System;
using DevExpress.Xpo;

namespace StringModel
{
    [DeferredDeletion(false)]
    public class UFStringLocaleText : XPObject
    {
        public UFStringLocaleText(Session session)
            : base(session)
        { }

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

        #region Properties

        private string _Text;
        [Size(SizeAttribute.Unlimited)]
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                SetPropertyValue("Text", ref _Text, value);
            }
        }

        private string _Locale;
        [Size(SizeAttribute.Unlimited)]
        public string Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                SetPropertyValue("Locale", ref _Locale, value);
            }
        }

        private string _Culture;
        [Size(SizeAttribute.Unlimited)]
        public string Culture
        {
            get
            {
                return _Culture;
            }
            set
            {
                SetPropertyValue("Culture", ref _Culture, value);
            }
        }

        private UFStringLocale _UFStringLocale;
        [Association("UFStringLocale-UFStringLocaleTexts")]
        public UFStringLocale UFStringLocale
        {
            get
            {
                return _UFStringLocale;
            }
            set
            {
                SetPropertyValue("UFStringLocale", ref _UFStringLocale, value);
            }
        }

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
    }
}
