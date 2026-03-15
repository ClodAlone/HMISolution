using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;
using UFInterfaces.PropertyControl;
using System.Threading.Tasks;

namespace UFUserModel
{
    public class UFUserSettings : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged
    {
        public UFUserSettings(Session session)
            : base(session)
        { }

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const int defaultAutoLogoutSeconds = 60;
        const int defaultPortNumber = 25;
        const int defaultMaxRuntimeEditAccessLevel = 1000;
        const int defaultMinRequiredPasswordLength = 4;
        const int defaultMaxInvalidPasswordAttempts = 5;
        const UserLockType defaultUserLockMode = UserLockType.None;
        const int defaultSharedRepositoriesSynchIntervalInMinutes = 5;

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

            if (!_AutoLogoutSeconds.HasValue)
                _AutoLogoutSeconds = defaultAutoLogoutSeconds;
            if (!_PortNumber.HasValue)
                _PortNumber = defaultPortNumber;
            if (!_MaxRuntimeEditAccessLevel.HasValue)
                _MaxRuntimeEditAccessLevel = defaultMaxRuntimeEditAccessLevel;
            if (!_MinRequiredPasswordLength.HasValue)
                _MinRequiredPasswordLength = defaultMinRequiredPasswordLength;
            if (!_MaxInvalidPasswordAttempts.HasValue)
                _MaxInvalidPasswordAttempts = defaultMaxInvalidPasswordAttempts;
            if (!_UserLockMode.HasValue)
                _UserLockMode = defaultUserLockMode;
            if (_SharedRepositoriesSynchIntervalInMinutes < 1)
                _SharedRepositoriesSynchIntervalInMinutes = defaultSharedRepositoriesSynchIntervalInMinutes;
        }
        #endregion

        #region Properties

        private int? _AutoLogoutSeconds;
        public int? AutoLogoutSeconds
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

        private bool _EnableUserManager;
        public bool EnableUserManager
        {
            get
            {
                return _EnableUserManager;
            }
            set
            {
                SetPropertyValue("EnableUserManager", ref _EnableUserManager, value);
            }
        }

        private UserLockType? _UserLockMode;
        public UserLockType? UserLockMode
        {
            get
            {
                return _UserLockMode;
            }
            set
            {
                SetPropertyValue("UserLockMode", ref _UserLockMode, value);
            }
        }

        private bool _LoginControlVisible = true;
        public bool LoginControlVisible
        {
            get
            {
                return _LoginControlVisible;
            }
            set
            {
                SetPropertyValue("LoginControlVisible", ref _LoginControlVisible, value);
            }
        }
        

        private int? _MaxRuntimeEditAccessLevel;
        public int? MaxRuntimeEditAccessLevel
        {
            get
            {
                return _MaxRuntimeEditAccessLevel;
            }
            set
            {
                SetPropertyValue("MaxRuntimeEditAccessLevel", ref _MaxRuntimeEditAccessLevel, value);
            }
        }

        private int? _MinRequiredPasswordLength;
        public int? MinRequiredPasswordLength
        {
            get
            {
                return _MinRequiredPasswordLength;
            }
            set
            {
                SetPropertyValue("MinRequiredPasswordLength", ref _MinRequiredPasswordLength, value);
            }
        }

        private int? _MaxInvalidPasswordAttempts;
        public int? MaxInvalidPasswordAttempts
        {
            get
            {
                return _MaxInvalidPasswordAttempts;
            }
            set
            {
                SetPropertyValue("MaxInvalidPasswordAttempts", ref _MaxInvalidPasswordAttempts, value);
            }
        }

        private uint _EnforcePasswordHistory;
        public uint EnforcePasswordHistory
        {
            get
            {
                return _EnforcePasswordHistory;
            }
            set
            {
                SetPropertyValue("EnforcePasswordHistory", ref _EnforcePasswordHistory, value);
            }
        }

        private int _NotifyPasswordExpiresInDays;
        public int NotifyPasswordExpiresInDays
        {
            get
            {
                return _NotifyPasswordExpiresInDays;
            }
            set
            {
                SetPropertyValue("NotifyPasswordExpiresInDays", ref _NotifyPasswordExpiresInDays, value);
            }
        }

        private bool _EnableValidationOnOS;
        public bool EnableValidationOnOS
        {
            get
            {
                return _EnableValidationOnOS;
            }
            set
            {
                SetPropertyValue("EnableValidationOnOS", ref _EnableValidationOnOS, value);
            }
        }

        private bool _UseSharedRepositoryTriggers;
        public bool UseSharedRepositoryTriggers
        {
            get
            {
                return _UseSharedRepositoryTriggers;
            }
            set
            {
                SetPropertyValue("UseSharedRepositoryTriggers", ref _UseSharedRepositoryTriggers, value);
            }
        }

        private String _SharedConnectionRepository;
        [Size(SizeAttribute.Unlimited)]
        public String SharedConnectionRepository
        {
            get
            {
                return _SharedConnectionRepository;
            }
            set
            {
                if (SetPropertyValue("SharedConnectionRepository", ref _SharedConnectionRepository, value))
                {
                    RaisePropertyChangedEvent("SharedConnectionRepositoryBackup");
                    OnPropertyVisiblityChanged("SharedConnectionRepository");
                }
            }
        }

        private String _SharedConnectionRepositoryBackup;
        [Size(SizeAttribute.Unlimited)]
        public String SharedConnectionRepositoryBackup
        {
            get
            {
                return _SharedConnectionRepositoryBackup;
            }
            set
            {
                if (SetPropertyValue("SharedConnectionRepositoryBackup", ref _SharedConnectionRepositoryBackup, value))
                    RaisePropertyChangedEvent("SharedConnectionRepository");
            }
        }

        private int _SharedRepositoriesSynchIntervalInMinutes;
        public int SharedRepositoriesSynchIntervalInMinutes
        {
            get
            {
                return _SharedRepositoriesSynchIntervalInMinutes;
            }
            set
            {
                SetPropertyValue("SharedRepositoriesSynchIntervalInMinutes", ref _SharedRepositoriesSynchIntervalInMinutes, value);
            }
        }

        private DateTime _LastChangedTime;
        [Browsable(false)]
        public DateTime LastChangedTime
        {
            get
            {
                return _LastChangedTime;
            }
            set
            {
                SetPropertyValue("LastChangedTime", ref _LastChangedTime, value);
            }
        }

        private String _DesktopSystemRole;
        public String DesktopSystemRole
        {
            get
            {
                return _DesktopSystemRole;
            }
            set
            {
                SetPropertyValue("DesktopSystemRole", ref _DesktopSystemRole, value);
            }
        }

        private string _ServerAddress;
        [Category("SMTPSettings")]
        [Size(SizeAttribute.Unlimited)]
        public string ServerAddress
        {
            get
            {
                return _ServerAddress;
            }
            set
            {
                SetPropertyValue("ServerAddress", ref _ServerAddress, value);
            }
        }

        private string _StaticFromAddress;
        [Category("SMTPSettings")]
        [Size(SizeAttribute.Unlimited)]
        public string StaticFromAddress
        {
            get
            {
                return _StaticFromAddress;
            }
            set
            {
                SetPropertyValue("StaticFromAddress", ref _StaticFromAddress, value);
            }
        }

        private int? _PortNumber;
        [Category("SMTPSettings")]
        public int? PortNumber
        {
            get
            {
                return _PortNumber;
            }
            set
            {
                SetPropertyValue("PortNumber", ref _PortNumber, value);
            }
        }

        private bool _EnAutentication;
        [Category("SMTPSettings")]
        public bool EnAutentication
        {
            get
            {
                return _EnAutentication;
            }
            set
            {
                SetPropertyValue("EnAutentication", ref _EnAutentication, value);
                OnPropertyVisiblityChanged("EnAutentication");
            }
        }

        private string _UserName;
        [Category("SMTPSettings")]
        [Size(SizeAttribute.Unlimited)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }

        private string _Password;
        [Category("SMTPSettings")]
        [Size(SizeAttribute.Unlimited)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
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

        #region Public Methods
        /// <summary>
        /// Add a new password in the history of the user given with the argument.
        /// </summary>
        /// <param name="user">
        /// The user where the add operation has to be performed.
        /// </param>
        /// <param name="oldPassword">
        /// The string password to add.
        /// </param>
        /// <param name="newPassword">
        /// The string password to check.
        /// </param>
        /// <returns>
        /// false if the password cannot be added because already exists in the history.
        /// </returns>
        public bool AddUserPasswordInHistory(UFUser user, string oldPassword, string newPassword)
        {
            if (EnforcePasswordHistory == 0)
                return true;

            if (EnforcePasswordHistory > 0)
            {
                if (!CheckUserPasswordInHistory(user, newPassword))
                {
                    user.PasswordHistory.Add(new UFUserModel.UFPasswordHistory(Session) { Password = oldPassword, LastChangedTime = DateTime.UtcNow });
                    if (user.PasswordHistory.Count > EnforcePasswordHistory)
                    {
                        var entry = (from c in user.PasswordHistory.AsParallel() orderby c.LastChangedTime select c).FirstOrDefault();
                        if (entry != null)
                            entry.Delete();
                    }
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the password exist in the history of the user given with the argument.
        /// </summary>
        /// <param name="user">
        /// The user where the check operation has to be performed.
        /// </param>
        /// <param name="password">
        /// The string password to check.
        /// </param>
        /// <returns>
        /// true if the password already exists in the history.
        /// </returns>
        public bool CheckUserPasswordInHistory(UFUser user, string password)
        {
            if (EnforcePasswordHistory > 0)
            {
                CleanUserPasswordHistory(user);
                var passwordHistory = (from c in user.PasswordHistory.AsParallel()
                                       where c.Password == password
                                       select c).FirstOrDefault();
                if (passwordHistory != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Cleans the password history of the user given with the argument.
        /// </summary>
        /// <param name="user">
        /// The user where the clean operation has to be performed.
        /// </param>
        public void CleanUserPasswordHistory(UFUser user)
        {
            if (user.PasswordHistory.Count > EnforcePasswordHistory)
            {
                var orderedList = (from c in user.PasswordHistory.AsParallel() orderby c.LastChangedTime select c).ToList();
                while (orderedList.Count > EnforcePasswordHistory)
                {
                    var entry = orderedList[0];
                    orderedList.RemoveAt(0);
                    entry.Delete();
                }
            }
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

        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ServerAddress")
            {
                if (!String.IsNullOrEmpty(ServerAddress) && !Regex.IsMatch(ServerAddress, ValidIpAddressRegex) && !Regex.IsMatch(ServerAddress, ValidHostnameRegex))
                    return Properties.Resources.InvalidServerAddress;

            }
            else if (propertyName == "PortNumber")
            {
                if (PortNumber == 0)
                    return Properties.Resources.ServerPortInvalid; ;
            }
            else if (propertyName == "SharedConnectionRepository" || propertyName == "SharedConnectionRepositoryBackup")
            {
                if (!String.IsNullOrEmpty(SharedConnectionRepository) && String.Compare(SharedConnectionRepository, SharedConnectionRepositoryBackup, true) == 0)
                    return Properties.Resources.SharedRepositoriesCannotBeEquals;
            }
            else if (propertyName == "UseSharedRepositoryTriggers")
            {
                if (UseSharedRepositoryTriggers && 
                    (!String.IsNullOrEmpty(SharedConnectionRepository) && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(SharedConnectionRepository) ||
                    !String.IsNullOrEmpty(SharedConnectionRepositoryBackup) && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(SharedConnectionRepositoryBackup)))
                    return Properties.Resources.SharedRepositortyTriggersNotSupported;
            }
            else if (propertyName == "SharedRepositoriesSynchIntervalInMinutes")
            {
                if (!String.IsNullOrEmpty(SharedConnectionRepositoryBackup) && SharedRepositoriesSynchIntervalInMinutes < 1)
                    return String.Format(Properties.Resources.SharedRepositoriesSynchIntervalError, 1);
            }

            return null;
        }

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
                if (propertyName == "UserName" || propertyName == "Password")
                {
                    return EnAutentication;
                }
                else if (propertyName == "SharedConnectionRepositoryBackup" || propertyName == "UseSharedRepositoryTriggers")
                {
                    return !String.IsNullOrEmpty(SharedConnectionRepository);
                }

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
    }
}
