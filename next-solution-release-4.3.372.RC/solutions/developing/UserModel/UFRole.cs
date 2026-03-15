using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using UFInterfaces.PropertyControl;
using System.ComponentModel;

namespace UFUserModel
{
    public class UFRole : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged
#if !NET_STANDARD
        , XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
#endif
    {
        public UFRole(Session session)
            : base(session)
        { }

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultAccessMask = 0x7FFFFFFF;

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

            if (!_AccessMask.HasValue)
                _AccessMask = defaultAccessMask;
        }
        #endregion

        #region Not Persistence Properties
        [Browsable(false)]
        [NonPersistent]
        public int? MaxAccessLevel { get; set; }
        #endregion

        #region Properties

        private Guid _NodeId;
        [Custom("Generate", "Guid")]
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }

        private string _Name;
        [MergablePropertyAttribute(false)]
        [Size(SizeAttribute.Unlimited)]
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

        private int _AutoLogoutSeconds = 0;
        public int AutoLogoutSeconds
        {
            get
            {
                return _AutoLogoutSeconds;
            }
            set
            {
                SetPropertyValue("AutoLogoutSeconds", ref _AutoLogoutSeconds, value);
            }
        }

        private int _AccessLevel;
        public int AccessLevel
        {
            get { return _AccessLevel; }
            set
            {
                SetPropertyValue("AccessLevel", ref _AccessLevel, value);
            }
        }

        private int? _AccessMask;
        public int? AccessMask
        {
            get { return _AccessMask; }
            set
            {
                SetPropertyValue("AccessMask", ref _AccessMask, value);
            }
        }

        private string _CultureName;
        public string CultureName
        {
            get
            {
                return _CultureName;
            }
            set
            {
                SetPropertyValue("CultureName", ref _CultureName, value);
            }
        }

        private string _ConverterName;
        public string ConverterName
        {
            get
            {
                return _ConverterName;
            }
            set
            {
                SetPropertyValue("ConverterName", ref _ConverterName, value);
            }
        }

        private string _TelegramGroupChatID;
        [Size(SizeAttribute.Unlimited)]

        public string TelegramGroupChatID
        {
            get
            {
                return _TelegramGroupChatID;
            }
            set
            {
                SetPropertyValue("TelegramChatID", ref _TelegramGroupChatID, value);
            }
        }

        [Association("UFRole-UFUsers"), Aggregated]
        public XPCollection<UFUser> UFUsers
        {
            get
            {
                return GetCollection<UFUser>("UFUsers");
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

        #region IDataErrorInfo
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
        #endregion

#if !NET_STANDARD
        #region IUniqueIdentifier
        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return NodeId.ToString();
            }
        }
        #endregion
#endif

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "TelegramGroupChatID")
                    return !string.IsNullOrEmpty(TelegramGroupChatID);
                return true;
            }
        }
        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;
        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                if ((from c in new XPQuery<UFUserModel.UFRole>(Session, true).AsParallel()
                     where c != this && c.Name == Name
                     select c).ToList().Count > 0)
                {
                    return Properties.Resources.RoleNameAlreadyExists;
                }
            }
            else if (propertyName == "AccessLevel")
            {
                if (AccessLevel < 0)
                    return Properties.Resources.AccessLevelRoleInvalid;
                else if (MaxAccessLevel.HasValue && AccessLevel > MaxAccessLevel)
                    return String.Format(Properties.Resources.AccessLevelNotAllowed, MaxAccessLevel);
            }

            return null;
        }
    }
}
