using System;
using DevExpress.Xpo;

namespace UFUAModel
{
    [DeferredDeletion(false)]
    public class UFUAEnumString : XPObject
    {
        #region Ctor
        public UFUAEnumString(Session session)
            : base(session)
        { }

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
            //if (!_PropertyName.HasValue)
            //    _PropertyName = defaultPropertyName;
            //if (_TimeSpanPropertyName == TimeSpan.Zero)
            //    _TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (_DateTimePropertyName == DateTime.MinValue)
            //    _DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Properties

        private string _Data;
        [Size(SizeAttribute.Unlimited)]
        public string Data
        {
            get
            {
                return _Data;
            }
            set
            {
                SetPropertyValue("Data", ref _Data, value);
            }
        }

        private UFUATag _UFUATag;
        [Association("UFUATag-EnumStrings")]
        public UFUATag UFUATag
        {
            get
            {
                return _UFUATag;
            }
            set
            {
                SetPropertyValue("UFUATag", ref _UFUATag, value);
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

        public override bool Equals(object obj)
        {
            if (obj is UFUAEnumString)
            {
                var enumString = obj as UFUAEnumString;
                return enumString.Data == Data;
            }
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
