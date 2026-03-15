using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;
using Utilities;

#if !NET_STANDARD
using System.Web.Security;
#endif


namespace UFUserModel
{
    public class UFUser : XPObject, IDataErrorInfo
#if !NET_STANDARD
        , XpoHelpers.UndoRedoIXPSimpleObjectHelper.IUniqueIdentifier
#endif
    {
        public UFUser(Session session)
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

        [Browsable(false)]
        [NonPersistent]
        public bool ValidatePasswordHistory { get; set; }
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

        private string _Password;
        [ValueConverter(typeof(EncryptedValueConverter))]
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
                this.RaisePropertyChangedEvent("PasswordConfirm");
            }
        }

        private string _PasswordConfirm;
        [NonPersistent]
        public string PasswordConfirm
        {
            get
            {
                return _PasswordConfirm;
            }
            set
            {
                SetPropertyValue("PasswordConfirm", ref _PasswordConfirm, value);
                this.RaisePropertyChangedEvent("Password");
            }
        }

        private string _ElectronicSignature;
        [ValueConverter(typeof(EncryptedValueConverter))]
        [Size(SizeAttribute.Unlimited)]
        [Custom("Generate", "Null")]
        public string ElectronicSignature
        {
            get
            {
                return _ElectronicSignature;
            }
            set
            {
                SetPropertyValue("ElectronicSignature", ref _ElectronicSignature, value);
            }
        }

        private int _PasswordExpiresInDays;
        public int PasswordExpiresInDays
        {
            get
            {
                return _PasswordExpiresInDays;
            }
            set
            {
                SetPropertyValue("PasswordExpiresInDays", ref _PasswordExpiresInDays, value);
            }
        }

        private bool _ForcePasswordChangeFirstLogin;
        public bool ForcePasswordChangeFirstLogin
        {
            get
            {
                return _ForcePasswordChangeFirstLogin;
            }
            set
            {
                SetPropertyValue("ForcePasswordChangeFirstLogin", ref _ForcePasswordChangeFirstLogin, value);
            }
        }

        private bool _Disabled;
        public bool Disabled
        {
            get
            {
                return _Disabled;
            }
            set
            {
                SetPropertyValue("Disabled", ref _Disabled, value);
            }
        }

        private DateTime _LastDateTimeChanged;
        [Custom("Generate", "DateTime")]
        [ReadOnly(true)]
        [Browsable(false)]
        public DateTime LastDateTimeChanged
        {
            get
            {
                return _LastDateTimeChanged;
            }
            set
            {
                if (value != DateTime.MinValue)
                    SetPropertyValue("LastDateTimeChanged", ref _LastDateTimeChanged, value);
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

        [Browsable(false)]
        public int GlobalAutoLogoutSeconds
        {
            get
            {
                if (UFRoleAss != null && _AutoLogoutSeconds > 0)
                    return UFRoleAss.AutoLogoutSeconds;
                return _AutoLogoutSeconds;
            }
        }

        private int _AccessLevel;
        public int AccessLevel
        {
            get 
            {
                return _AccessLevel; 
            }
            set
            {
                SetPropertyValue("AccessLevel", ref _AccessLevel, value);
            }
        }

        [Browsable(false)]
        public int GlobalAccessLevel
        {
            get
            {
                if (UFRoleAss != null && _AccessLevel == -1)
                    return UFRoleAss.AccessLevel;
                return _AccessLevel;
            }
        }

        private int? _AccessMask;
        public int? AccessMask
        {
            get 
            {
                return _AccessMask; 
            }
            set
            {
                SetPropertyValue("AccessMask", ref _AccessMask, value);
            }
        }

        [Browsable(false)]
        public int GlobalAccessMask
        {
            get
            {
                if (UFRoleAss != null && _AccessMask == -1)
                    return UFRoleAss.AccessMask.Value;

                return AccessMask.Value;
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

        [Browsable(false)]
        public string GlobalCultureName
        {
            get
            {
                if (UFRoleAss != null && String.IsNullOrEmpty(_CultureName))
                    return UFRoleAss.CultureName;

                return _CultureName;
            }
        }

        [Browsable(false)]
        public string GlobalConverterName
        {
            get
            {
                if (UFRoleAss != null && String.IsNullOrEmpty(_ConverterName))
                    return UFRoleAss.ConverterName;

                return _ConverterName;
            }
        }

        private string _Email;
        [Size(SizeAttribute.Unlimited)]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                SetPropertyValue("Email", ref _Email, value);
            }
        }

        private string _PhoneNumber;
        [Size(SizeAttribute.Unlimited)]
        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                SetPropertyValue("PhoneNumber", ref _PhoneNumber, value);
            }
        }

        private string _MobilePhoneNumber;
        [Size(SizeAttribute.Unlimited)]
        public string MobilePhoneNumber
        {
            get
            {
                return _MobilePhoneNumber;
            }
            set
            {
                SetPropertyValue("MobilePhoneNumber", ref _MobilePhoneNumber, value);
            }
        }

        private string _TelegramChatID;
        [Size(SizeAttribute.Unlimited)]
        public string TelegramChatID
        {
            get
            {
                return _TelegramChatID;
            }
            set
            {
                SetPropertyValue("TelegramChatID", ref _TelegramChatID, value);
            }
        }

        private string _FCMTokenPath;
        [Size(SizeAttribute.Unlimited)]
        public string FCMTokenPath
        {
            get
            {
                return _FCMTokenPath;
            }
            set
            {
                SetPropertyValue("FCMTokenPath", ref _FCMTokenPath, value);
            }
        }


        #region TagPhoneNumber
        private string _TagPhoneNumber;
        [Size(SizeAttribute.Unlimited)]
#if !NETSTANDARD
        [DisplayNameExtension]
#endif
        public string TagPhoneNumber
        {
            get { return _TagPhoneNumber; }
            set
            {
                SetPropertyValue("TagPhoneNumber", ref _TagPhoneNumber, value);
            }
        }

        #endregion

        private UFRole _UFRoleAss;
        [Association("UFRole-UFUsers")]
        public UFRole UFRoleAss
        {
            get
            {
                return _UFRoleAss;
            }
            set
            {
                SetPropertyValue("UFRoleAss", ref _UFRoleAss, value);
            }
        }

        [Association("UFUser-UFPasswordHistory"), Aggregated]
        public XPCollection<UFPasswordHistory> PasswordHistory
        {
            get
            {
                return GetCollection<UFPasswordHistory>("PasswordHistory");
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
            _PasswordConfirm = Password;
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

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (String.IsNullOrWhiteSpace(Name))
                {
                    return Properties.Resources.ObjectNameMissing;
                }
                else if ((from c in new XPQuery<UFUserModel.UFUser>(Session, true).AsParallel()
                          where c != this && (String.Compare(c.Name, Name, true) == 0 || String.Compare(c.Name, Name.Trim(), true) == 0)
                          select c).ToList().Count > 0)
                {
                    return Properties.Resources.UserNameAlreadyExists;
                }
            }
            else if (propertyName == "ElectronicSignature")
            {
                if (!String.IsNullOrEmpty(ElectronicSignature) && 
                    (from c in new XPQuery<UFUserModel.UFUser>(Session, true)
                     where c != this && c.ElectronicSignature == ElectronicSignature 
                     select c).ToList().Count > 0)
                {
                    return Properties.Resources.UserElectronicSignatureAlreadyExists;
                }
            }
            else if (propertyName == "AccessLevel")
            {
                if (AccessLevel < -1)
                    return Properties.Resources.AccessLevelUserInvalid;
                else if (MaxAccessLevel.HasValue && GlobalAccessLevel > MaxAccessLevel)
                    return String.Format(Properties.Resources.AccessLevelNotAllowed, MaxAccessLevel);
            }
            else if (propertyName == "Password")
            {
                var list = (from tag in new XPQuery<UFUserModel.UFUserSettings>(Session, true) select tag).ToList();
                if (list.Count > 0 && list[0].MinRequiredPasswordLength > 0)
                {
                    if (Password == null || Password.Length < list[0].MinRequiredPasswordLength)
                        return String.Format(Properties.Resources.PasswordRequiredLengthError,
                            list[0].MinRequiredPasswordLength);
                }
                if (ValidatePasswordHistory && list.Count > 0 && list[0].EnforcePasswordHistory > 0)
                {
                    if (list[0].CheckUserPasswordInHistory(this, Password))
                        return Properties.Resources.PasswordCannotBeReused;
                }
#if !NET_STANDARD
                if (Membership.Provider != null)
                {
                    if (!String.IsNullOrEmpty(Membership.Provider.PasswordStrengthRegularExpression))
                    {
                        var rg = new System.Text.RegularExpressions.Regex(Membership.Provider.PasswordStrengthRegularExpression);
                        if (Password == null || !rg.IsMatch(Password))
                            return String.Format(Properties.Resources.PasswordStrengthRegularExpressionError, 
                                Membership.Provider.PasswordStrengthRegularExpression);
                    }
                    if (Membership.Provider.MinRequiredPasswordLength > 0)
                    {
                        if (Password == null || Password.Length < Membership.Provider.MinRequiredPasswordLength)
                            return String.Format(Properties.Resources.PasswordRequiredLengthError,
                                Membership.Provider.MinRequiredPasswordLength);
                    }
                    if (Membership.Provider.MinRequiredNonAlphanumericCharacters > 0)
                    {
                        if (Password == null || (Password.Length - Password.Count(char.IsLetterOrDigit)) < Membership.Provider.MinRequiredNonAlphanumericCharacters)
                            return String.Format(Properties.Resources.PasswordRequiredNonAlphanumericCharactersError,
                                Membership.Provider.MinRequiredNonAlphanumericCharacters);
                    }
                }
#endif
            }
            else if (propertyName == "PasswordConfirm")
            {
                if (Password != PasswordConfirm)
                    return Properties.Resources.PasswordMismatch;
            }
            else if(propertyName == "Email")
            {
                if (!String.IsNullOrEmpty(Email) && !IsValidEmail(Email))
                    return Properties.Resources.EmailError;
            }

            return null;
        }
        bool invalid = false;
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

    }
}
