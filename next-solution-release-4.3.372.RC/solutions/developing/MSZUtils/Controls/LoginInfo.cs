using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.ClientServices.Providers;
using Utilities;
using System.Security;
using MSZUtilsServiceHelper;

namespace MSZUtils.Controls
{
    class LoginInfo : Observable
    {
        private string userName;

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

        public ClientAutenticationCredentials ToLoginParameters()
        {
            return new ClientAutenticationCredentials(UserName, Password, -1, -1);
        }
    }
}
