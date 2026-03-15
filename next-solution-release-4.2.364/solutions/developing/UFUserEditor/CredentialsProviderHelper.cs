using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using log4net;
#if !IO_Server
using UFInterfaces.AuthenticationCredentialsProvider;
#else
using AuthenticationCredentialsProvider.ComponentService;
#endif

#if !IO_Server
namespace UFUserEditor
#else
namespace UFUAServerBase
#endif
{
    internal class ErrorMappingEventArgs : EventArgs
    {
        public ErrorMappingEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public String Message { get; private set; }
        public Exception Exception { get; private set; }
    }

    internal class CredentialsProviderHelper
    {
#region Declarations
        readonly String applicationName;
#if !IO_Server
        readonly IAuthenticationCredentialsProvider credentialsProvider;
#else
        readonly AuthenticationCredentialsProviderComponent credentialsProvider;
#endif
        readonly IList<UFUserModel.UFUser> users;
        readonly IList<UFUserModel.UFRole> roles;
#endregion

#region Constructors
        public CredentialsProviderHelper(String applicationName,
#if !IO_Server
            IAuthenticationCredentialsProvider credentialsProvider, 
#else
            AuthenticationCredentialsProviderComponent credentialsProvider, 
#endif
            IList<UFUserModel.UFUser> users, IList<UFUserModel.UFRole> roles)
        {
            this.applicationName = applicationName;
            this.credentialsProvider = credentialsProvider;
            this.users = users;
            this.roles = roles;
        }
#endregion

#region Public Events
        public event EventHandler<ErrorMappingEventArgs> ErrorMapping;
        void OnError(string message, Exception ex)
        {
            var t = ErrorMapping;
            if (t != null)
                t(this, new ErrorMappingEventArgs(message, ex));
        }
#endregion

#region Public Methods
        public void MapToCredentialProvider()
        {
            var lockedUsers = new List<String>();
            try
            {
                var oldUsers = credentialsProvider.GetAllUsers(applicationName);
                foreach (MembershipUser olduser in oldUsers)
                {
                    try
                    {
                        if (credentialsProvider.IsUserLocked(applicationName, olduser.UserName))
                            lockedUsers.Add(olduser.UserName);
                        credentialsProvider.DeleteUser(applicationName, olduser.UserName);
                    }
                    catch (Exception ex)
                    {
                        OnError(String.Format(Properties.Resources.ErrorDeletingUser, olduser.UserName), ex);
                    }
                }

                var oldRoles = credentialsProvider.GetAllRoles(applicationName);
                foreach (String oldRole in oldRoles)
                {
                    var usersinrole = credentialsProvider.GetUsersInRole(applicationName, oldRole);
                    if (usersinrole.Length > 0)
                        try
                        {
                            credentialsProvider.RemoveUsersFromRoles(applicationName, usersinrole, new String[] { oldRole });
                        }
                        catch (Exception ex)
                        {
                            OnError(String.Format(Properties.Resources.ErrorRemovingUsersFromRole, oldRole), ex);
                        }
                    try
                    {
                        credentialsProvider.DeleteRole(applicationName, oldRole);
                    }
                    catch (Exception ex)
                    {
                        OnError(String.Format(Properties.Resources.ErrorDeletingRole, oldRole), ex);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            foreach (var user in users)
            {
                try
                {
                    credentialsProvider.CreateUser(applicationName, user.Name, user.Password, !user.Disabled);
                    if (lockedUsers.Contains(user.Name))
                        credentialsProvider.LockUser(applicationName, user.Name);
                }
                catch (Exception ex)
                {
                    OnError(String.Format(Properties.Resources.ErrorCreatingUser, user.Name), ex);
                }
            }

            foreach (var role in roles)
            {
                try
                {
                    credentialsProvider.CreateRole(applicationName, role.Name);
                }
                catch (Exception ex)
                {
                    OnError(String.Format(Properties.Resources.ErrorCreatingRole, role.Name), ex);
                    continue;
                }

                var usersname = (from c in role.UFUsers/*.AsParallel()*/ select c.Name).ToList();
                if (usersname.Count > 0)
                {
                    try
                    {
                        credentialsProvider.AddUsersToRoles(applicationName, usersname.ToArray(), new String[] { role.Name });
                    }
                    catch (Exception ex)
                    {
                        OnError(String.Format(Properties.Resources.ErrorAssigningUserToRole, role.Name), ex);
                    }
                }
            }
        }
    }
#endregion
}
