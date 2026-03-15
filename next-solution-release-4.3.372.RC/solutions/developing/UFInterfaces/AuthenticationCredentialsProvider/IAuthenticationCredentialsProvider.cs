using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using System.Web.ClientServices.Providers;

namespace UFInterfaces.AuthenticationCredentialsProvider
{
    public interface IAuthenticationCredentialsProvider : IUFInterfaceBase
    {
        void SetSharedApplicationName(String applicationName, String sharedApplicationName);

        void RemoveSharedApplicationName(String applicationName);

        void EnableValidationOnOS(String applicationName, bool bEnable);

        bool? ValidateUsingCredentialsProvider(IDocument document, String applicationName, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null);
        ClientFormsAuthenticationCredentials ShowLoginWindow(IDocument document, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null);

        void Logout(String applicationName, bool openBrowserOnExternalAuth = true);
        bool? Login(String applicationName, String user, String psw);
        bool? Validate(String applicationName, String user, String psw);
        bool LockUser(String applicationName, String userName);
        bool UnlockUser(String applicationName, String userName);
        bool IsUserLocked(String applicationName, String userName);
        bool CreateUser(String applicationName, String userName, String password, bool isApproved = true);
        bool DeleteUser(String applicationName, String userName);
        IEnumerable GetAllUsers(String applicationName);
        int GetNumberOfUsersOnline(String applicationName);

        bool IsCurrentUserInRole(String applicationName, String role);
        bool IsUserInRole(String applicationName, String username, String roleName);
	    bool AddUsersToRoles(String applicationName, String[] userNames, String[] roleNames);
	    bool RemoveUsersFromRoles(String applicationName, String[] userNames, String[] roleNames);
	    bool CreateRole(String applicationName, String role);
	    bool DeleteRole(String applicationName, String role);
        bool RoleExists(String applicationName, String role);

    	IEnumerable GetAllRoles(String applicationName);
    	String[] GetRolesForUser(String applicationName, String user);
	    String[] GetUsersInRole(String applicationName, String role);

        event EventHandler<LoginInfoEventArgs> UserOnline;
        event EventHandler<LoginInfoEventArgs> FailedLogin;
        event EventHandler<LoginInfoEventArgs> UserUnlocked;
        event EventHandler<LoginInfoEventArgs> FaultedProvider;

        event EventHandler<LoginInfoEventArgs> PromptUserLogin;

        bool IsFaulted(String applicationName);
        void ClearFaulted(String applicationName);

        string RefreshCurrentUser(String applicationName);
    }
}
