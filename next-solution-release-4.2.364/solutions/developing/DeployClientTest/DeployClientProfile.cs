using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeployClientTest
{
    [DataContract(Name = "DeployClientProfile")]
    public class DeployClientProfile : IDataErrorInfo, INotifyPropertyChanged
    {
        const String NameMatch = @"^[a-zA-Z][a-zA-Z0-9_]*$";

        #region IDataErrorInfo
        [Browsable(false)]
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "Name")
                    if (String.IsNullOrEmpty(Name) || !Regex.IsMatch(Name, NameMatch))
                        return Properties.Resources.InvalidProfileName;
                return null;
            }
        }

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
        #endregion

        #region Properties
        string name;
        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        string user;
        [DataMember]
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                if (user != value)
                {
                    user = value;
                    OnPropertyChanged("User");
                }
            }
        }

        string host;
        [DataMember]
        public string Host
        {
            get
            {
                return host;
            }
            set
            {
                if (host != value)
                {
                    host = value;
                    OnPropertyChanged("Host");
                }
            }
        }

        string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }
        }

        string encryptedPassword;
        [DataMember]
        public string EncryptedPassword
        {
            get
            {
                encryptedPassword = String.IsNullOrEmpty(Password) ? String.Empty : WPFUtilities.CryptString.CryptString.EncryptString(Password);
                return encryptedPassword;
            }
            set
            {
                if (value != encryptedPassword)
                {
                    encryptedPassword = value;
                    if (!String.IsNullOrEmpty(value))
                        Password = WPFUtilities.CryptString.CryptString.DecryptString(encryptedPassword);
                    OnPropertyChanged("EncryptedPassword");
                }
            }
        }
        #endregion

        #region Ctor
        public DeployClientProfile()
        {

        }

        public DeployClientProfile(string name, string host = "", string user = "", string password = "")
        {
            Name = name;
            Host = host;
            User = user;
            Password = password;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
