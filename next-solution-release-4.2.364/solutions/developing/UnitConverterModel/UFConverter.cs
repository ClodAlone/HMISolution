using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace UnitConverterModel
{
    [DeferredDeletion(false)]
    public class UFConverter : XPObject
    {
        public UFConverter(Session session)
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
        private string _Name;
        [Indexed(Unique = false)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
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

        [Association("UFConverter-UFConverterItems"), Aggregated]
        public XPCollection<UFConverterItem> UFConverterItems
        {
            get
            {
                return GetCollection<UFConverterItem>("UFConverterItems");
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
