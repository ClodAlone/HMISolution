using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;
//using System.Security.Cryptography; 
using System.Text.RegularExpressions;
//using System.IO;

namespace ADSmtp
{
    [Persistent("ADSmtpPluginSettings")]
    public class ADSmtpPluginSettings : XPObject, IDataErrorInfo
    {
        #region  Constructors

        public ADSmtpPluginSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected ADSmtpPluginSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }   
   
        #endregion

        #region Methods
        public void DefaultSettings()
        {
            _ServerAddress = "";
            _ServerPort = 25;
            _User = "";
            _Password = "";
            _RasEnable = false;
            _DialupEntry = string.Empty;
            _PhoneNumber = string.Empty;
            _RASUser = string.Empty;
            _RASPassword = string.Empty;
            _RetryTime = 20;
            _DisconnectAfter = 20;
            _Retries = 3;
            _PhonebookPath = string.Empty;
            _From = string.Empty;
        }
        #endregion

        #region Properties
        private string _ServerAddress;
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
        private int _ServerPort;
        public int ServerPort
        {
            get
            {
                return _ServerPort;
            }
            set
            {
                SetPropertyValue("ServerPort", ref _ServerPort, value);
            }
        }

        private string _User;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }
        
        private string _Password;
        [ValueConverter(typeof(UFUserModel.EncryptedValueConverter))]
        [Size(200)]
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
        private bool _RasEnable;
        public bool RasEnable
        {
            get
            {
                return _RasEnable;
            }
            set
            {
              SetPropertyValue("RasEnable", ref _RasEnable, value);
              this.RaisePropertyChangedEvent("PhoneNumber");
              this.RaisePropertyChangedEvent("RASPassword");
              this.RaisePropertyChangedEvent("RASUser");
              this.RaisePropertyChangedEvent("DialupEntry");
            }
        }
        private string _DialupEntry;
        public string DialupEntry
        {
            get
            {
                return _DialupEntry;
            }
            set
            {
              SetPropertyValue("DialupEntry", ref _DialupEntry, value);
                this.OnChanged("RasEnable");
            }
        }
        private string _PhoneNumber;
        public string PhoneNumber
        {
           get
            {
                return _PhoneNumber;
            }
            set
            {
              SetPropertyValue("PhoneNumber", ref _PhoneNumber, value);
              this.OnChanged("RasEnable");
            }
        }
        private string _RASUser;
        public string RASUser
        {
            get
            {
                return _RASUser;
            }
            set
            {
              SetPropertyValue("RASUser", ref _RASUser, value);
              this.OnChanged("RasEnable");
            }
        }
        private string _RASPassword;
        [ValueConverter(typeof(UFUserModel.EncryptedValueConverter))]
        [Size(200)]
        public string RASPassword
        {
            get
            {
                return _RASPassword;
            }
            set
            {
              SetPropertyValue("RASPassword", ref _RASPassword, value);
              this.OnChanged("RasEnable");
            }
        }
        private int _RetryTime;
        public int RetryTime
        {
            get
            {
                return _RetryTime;
            }
            set
            {
              SetPropertyValue("RetryTime", ref _RetryTime, value);
            }
        }
        private int _DisconnectAfter;
        public int DisconnectAfter
        {
            get
            {
                return _DisconnectAfter;
            }
            set
            {
              SetPropertyValue("DisconnectAfter", ref _DisconnectAfter, value);
            }
        }
        private int _Retries;
        public int Retries
        {
            get
            {
                return _Retries;
            }
            set
            {
              SetPropertyValue("Retries", ref _Retries, value);
            }
        }
        private string _PhonebookPath;
        public string PhonebookPath
        {
            get
            {
                return _PhonebookPath;
            }
            set
            {
                SetPropertyValue("PhonebookPath", ref _PhonebookPath, value);
            }
        }
        private string _From;
        public string From
        {
            get
            {
                return _From;
            }
            set
            {
                SetPropertyValue("From", ref _From, value);
            }
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
        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "ServerAddress")
            {
                if (String.IsNullOrEmpty(ServerAddress))
                    return Properties.Resources.EnterServerAddress;

                if (!Regex.IsMatch(ServerAddress, ValidIpAddressRegex) && !Regex.IsMatch(ServerAddress, ValidHostnameRegex))
                    return Properties.Resources.InvalidServerAddress;

            }
            else if(propertyName == "From")
            {
                string expression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

                if (!Regex.IsMatch(From, expression))
                    return Properties.Resources.InvalidFromAddress;
            }
            else if (propertyName == "ServerPort")
            {
                if (ServerPort == 0)
                    return Properties.Resources.ServerPortInvalid; ;
            }
            else if (propertyName == "RasEnable")
            {
                if (RasEnable && ((PhoneNumber == null || PhoneNumber.Length == 0) || ((DialupEntry == null || DialupEntry.Length == 0) && 
                    ((RASUser == null || RASUser.Length == 0) || (RASPassword == null || RASPassword.Length == 0)))))
                {
                    return Properties.Resources.RASDatatInvalid;
                }
            }
            else if (propertyName == "DialupEntry")
            {
                //if (RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
                //    return Properties.Resources.RASDialupInvalid;
            }
            else if (propertyName == "RASUser")
            {
                //if (RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
                //    return Properties.Resources.RASUserInvalid;
            }
            else if (propertyName == "RASPassword")
            {
                //if (RasEnable && (DialupEntry == null || DialupEntry.Length == 0) && (RASUser == null || RASUser.Length == 0) && (RASPassword == null || RASPassword.Length == 0))
                //    return Properties.Resources.RASPasswordInvalid;
            }
            else if (propertyName == "PhoneNumber")
            {
                if (RasEnable && (PhoneNumber == null || PhoneNumber.Length == 0))
                    return Properties.Resources.RASPhoneInvalid;
            }


            return null;
        }


        #endregion
    }
}
