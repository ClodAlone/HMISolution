using System;
using System.ComponentModel.DataAnnotations;
using System.Web.ClientServices.Providers;
using Utilities;

namespace AuthenticationCredentialsProvider
{
    class LoginInfo : Observable
    {
        private string userName;
        private bool rememberMe;

        /// <summary>
        /// Gets and sets the user name.
        /// </summary>
        [Display(Name = "UserNameLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                Set(ref userName, value, "UserName");
            }
        }

        /// <summary>
        /// Gets and sets the password.
        /// </summary>
        [Display(Name = "PasswordLabel", ResourceType = typeof(Properties.Resources))]
        [Required]
        String password;
        public String Password
        {
            get
            {
                return password;
            }
            set
            {
                Set(ref password, value, "Password");
            }
        }

        /// <summary>
        /// Gets and sets the value indicating whether the user's authentication information
        /// should be recorded for future logins.
        /// </summary>
        [Display(Name = "RememberMeLabel", ResourceType = typeof(Properties.Resources))]
        public bool RememberMe
        {
            get
            {
                return rememberMe;
            }

            set
            {
                Set(ref rememberMe, value, "RememberMe");
            }
        }

        public ClientFormsAuthenticationCredentials ToLoginParameters()
        {
            return new ClientFormsAuthenticationCredentials(UserName, Password, RememberMe);
        }
    }
}
