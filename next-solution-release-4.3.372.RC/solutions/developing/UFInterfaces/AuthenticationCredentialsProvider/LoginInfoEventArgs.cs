using Opc.Ua;
using System;

namespace UFInterfaces.AuthenticationCredentialsProvider
{
    public class LoginInfoEventArgs : EventArgs
    {
        public String ApplicationName;
        public String User;
        public String Password;
        public string Role;
        public UserIdentity UserIdentity;
        public bool IsRefreshing;
        public bool IsLocked;
        public bool? IsPasswordExpired;
        public string ErrorInfo;
        public bool IsExternalAuthenticationActive;

        public String requestedRole;
        public String requestedUser;
        public int requestedLevel;
        public bool requestingUserAuthentication;
        public bool dialogResult;
    }
}
