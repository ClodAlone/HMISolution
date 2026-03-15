using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Timers;
#if !IO_Server
using System.Web.ClientServices;
using System.Web.ClientServices.Providers;
#endif
using System.Web.Security;
using UFInterfaces;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFInterfaces.CoreHostComponents;
using System.Security;
using System.DirectoryServices.AccountManagement;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using DocumentManager.ComponentService;
using Utilities;
using TranslationHelpers;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;

namespace AuthenticationCredentialsProvider.ComponentService
{
    public class AuthenticationCredentialsProviderComponent
#if !IO_Server
        : ComponentBase<IAuthenticationCredentialsProvider>, IAuthenticationCredentialsProvider, IDisposable
#endif
    {
        #region Declaration
        static Object lockObject = new Object();
#if !IO_Server
        IWorkspace workspace;
        static List<String> listFaulted = new List<String>();
        static Dictionary<String, String> currentMap = new Dictionary<String, String>();
        static Timer timerCleanFaulted;

        enum LoginAttemptResult
        {
            Invalid,
            Failed,
            Success
        };
        private bool firsttime = true;
        int failedAttemptsCounter = 0;
#endif
        #endregion


        #region IUFInterfaceBase Members
#if !IO_Server
        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
#endif
        #endregion
#if !IO_Server
        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
        }

        void AddToFaulted(String applicationName)
        {
            lock (listFaulted)
            {
                if (!listFaulted.Contains(applicationName))
                {
                    listFaulted.Add(applicationName);
                    OnFaultedProvider(applicationName);
                }
            }

            StartFualtedCleaner();
        }

        static bool IsInFault(String applicationName)
        {
            lock (listFaulted)
            {
                return listFaulted.Contains(applicationName);
            }
        }

        static void StartFualtedCleaner()
        {
            lock (listFaulted)
            {
                if (timerCleanFaulted == null)
                {
                    timerCleanFaulted = new Timer(10000);
                    timerCleanFaulted.Interval = 2000;
                    timerCleanFaulted.Elapsed += (o, e) =>
                        {
                            timerCleanFaulted.Enabled = false;

                            try
                            {
                                var checkList = new List<String>();
                                lock (listFaulted)
                                {
                                    checkList.AddRange(listFaulted);
                                }

                                checkList.ForEach(applicationName =>
                                    {
                                        try
                                        {
                                            bool bOk = false;
                                            lock (lockObject)
                                            {
                                                Membership.ApplicationName = applicationName;
                                                var ret = Membership.GetUser(true);
                                                bOk = true;
                                            }

                                            if (bOk)
                                            {
                                                lock (listFaulted)
                                                {
                                                    if (listFaulted.Contains(applicationName))
                                                        listFaulted.Remove(applicationName);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    });
                            }
                            finally
                            {
                                timerCleanFaulted.Enabled = true;
                            }
                        };

                    timerCleanFaulted.Enabled = true;
                }
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lockObject = null;
        }

        #endregion
#endif

        #region IAuthenticationCredentialsProvider Members
#if !IO_Server
        public event EventHandler<LoginInfoEventArgs> UserOnline;
        #region OnUserOnline
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        void OnUserOnline(String applicationName, String user, String password, bool isRefreshing = false)
        {
            var e = UserOnline;
            if (e != null)
            {
                e(this, new LoginInfoEventArgs() { User = user, Password = password, ApplicationName = applicationName, IsRefreshing = isRefreshing });
            }
        }
        #endregion

        public event EventHandler<LoginInfoEventArgs> FailedLogin;
        #region OnFailedLogin
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        void OnFailedLogin(String applicationName, String user, bool isLocked = false)
        {
            var e = FailedLogin;
            if (e != null)
            {
                e(this, new LoginInfoEventArgs() { ApplicationName = applicationName, User = user, IsLocked = isLocked });
            }
        }
        #endregion

        public event EventHandler<LoginInfoEventArgs> UserUnlocked;
        #region OnUserUnlocked
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        void OnUserUnlocked(String applicationName, String user)
        {
            var e = UserUnlocked;
            if (e != null)
            {
                e(this, new LoginInfoEventArgs() { ApplicationName = applicationName, User = user });
            }
        }
        #endregion

        public event EventHandler<LoginInfoEventArgs> FaultedProvider;
        #region OnFaultedProvider
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        void OnFaultedProvider(String applicationName)
        {
            var e = FaultedProvider;
            if (e != null)
            {
                e(this, new LoginInfoEventArgs() { ApplicationName = applicationName });
            }
        }
        #endregion

        public event EventHandler<LoginInfoEventArgs> PromptUserLogin;
        #region OnPromptUserLogin
        /// <summary>
        /// Triggers the UserOnline event.
        /// </summary>
        void OnPromptUserLogin(LoginInfoEventArgs loginInfo)
        {
            var e = PromptUserLogin;
            if (e != null)
            {
                e(this, loginInfo);
            }
        }
        #endregion

        public bool IsFaulted(String applicationName)
        {
            applicationName = GetSharedApplicationName(applicationName);

            return IsInFault(applicationName);
        }

        public void ClearFaulted(String applicationName)
        {
            applicationName = GetSharedApplicationName(applicationName);

            lock (listFaulted)
            {
                if (listFaulted.Contains(applicationName))
                    listFaulted.Remove(applicationName);
            }
        }

        static Dictionary<String, bool> mapApplicationNameEnableOS = new Dictionary<String, bool>();
        public void EnableValidationOnOS(String applicationName, bool bEnable)
        {
            applicationName = GetSharedApplicationName(applicationName);

            lock (mapApplicationNameEnableOS)
            {
                if (mapApplicationNameEnableOS.ContainsKey(applicationName))
                    mapApplicationNameEnableOS.Remove(applicationName);
                mapApplicationNameEnableOS.Add(applicationName, bEnable);
            }
        }

        static Dictionary<String, String> mapApplicationNameSharedName = new Dictionary<String, String>();
        public void SetSharedApplicationName(String applicationName, String sharedApplicationName)
        {
            lock (mapApplicationNameSharedName)
            {
                if (!mapApplicationNameSharedName.ContainsKey(applicationName))
                    mapApplicationNameSharedName[applicationName] = sharedApplicationName;
            }
        }

        public void RemoveSharedApplicationName(String applicationName)
        {
            lock (mapApplicationNameSharedName)
            {
                mapApplicationNameSharedName.Remove(applicationName);
            }
        }

        static String GetSharedApplicationName(String applicationName)
        {
            lock (mapApplicationNameSharedName)
            {
                if (mapApplicationNameSharedName.ContainsKey(applicationName))
                    return mapApplicationNameSharedName[applicationName];
            }

            return applicationName;
        }

        static bool IsSharedApplicationName(String applicationName)
        {
            lock (mapApplicationNameSharedName)
            {
                return mapApplicationNameSharedName.ContainsKey(applicationName);
            }
        }

        public bool Login(String applicationName, String user, String psw)
        {
            return ValidateCredentials(false, applicationName, user: user, psw: psw);
        }

        public bool Validate(String applicationName, String user, String psw)
        {
            return ValidateCredentials(false, applicationName, user: user, psw: psw, bApplyUserIndentity: false);
        }

        public bool ValidateUsingCredentialsProvider(IDocument document, String applicationName, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null)
        {
            return ValidateCredentials(true, applicationName, document, requestedRoleOrUser, requestedLevel, owner);
        }

        bool ValidateCredentials(bool bWithUI, String applicationName, IDocument document = null, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null, String user = null, String psw = null, bool bApplyUserIndentity = true)
        {
            if (IsFaulted(applicationName))
                return false;

            bool bRetryOnFailed = IsSharedApplicationName(applicationName);
            applicationName = GetSharedApplicationName(applicationName);

            try
            {
                if (bWithUI)
                {
                    failedAttemptsCounter = 0;
                    while (true)
                    {
                        var ret = ProcessCredentials(true, applicationName, document, requestedRoleOrUser, requestedLevel, owner, bRetryOnFailed: bRetryOnFailed, bApplyUserIndentity: bApplyUserIndentity);
                        if (ret == LoginAttemptResult.Invalid || ret == LoginAttemptResult.Success)
                            return ret == LoginAttemptResult.Success;
                    }
                }
                else
                {
                    var ret = ProcessCredentials(false, applicationName, user: user, psw: psw, bRetryOnFailed: bRetryOnFailed, bApplyUserIndentity: bApplyUserIndentity);
                    bool bSuccess = ret == LoginAttemptResult.Success;
                    if (bSuccess)
                        failedAttemptsCounter = 0;
                    return bSuccess;
                }
            }
            catch (Exception ex)
            {
                AddToFaulted(applicationName);
                return false;
            }
        }

        LoginAttemptResult ProcessCredentials(bool bWithUI, String applicationName, IDocument document = null, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null, String user = null, String psw = null, bool bRetryOnFailed = false, bool bApplyUserIndentity = true)
        {
            ClientFormsAuthenticationCredentials ret;
            if (bWithUI)
            {
                if (owner == null)
                {
                    var ie = Keyboard.FocusedElement as DependencyObject;
                    if (ie != null)
                        owner = Window.GetWindow(ie);
                }
                ret = ShowLoginWindow(document, requestedRoleOrUser, requestedLevel, owner);
            }
            else
            {
                ret = new LoginInfo() { UserName = user, Password = psw }.ToLoginParameters();
            }
            if (ret == null)
                return LoginAttemptResult.Invalid;
            else if (String.IsNullOrEmpty(ret.UserName) || String.IsNullOrEmpty(ret.Password))
                return LoginAttemptResult.Failed;

            bool isAuthorized = false;
            lock (lockObject)
            {
                try
                {
                    Membership.ApplicationName = applicationName;
                    // Call ValidateUser with empty strings in order to display the 
                    // login dialog box configured as a credentials provider.
                    isAuthorized = Membership.ValidateUser(
                        ret.UserName, ret.Password);
                }
                catch (WebException)
                {
                    ConnectivityStatus.IsOffline = true;
                    isAuthorized = Membership.ValidateUser(
                        ret.UserName, ret.Password);
                }

                if (!isAuthorized)
                {
                    bool bCheckOnOS = false;
                    lock (mapApplicationNameEnableOS)
                    {
                        if (mapApplicationNameEnableOS.ContainsKey(applicationName) &&
                            mapApplicationNameEnableOS[applicationName])
                            bCheckOnOS = true;
                    }

                    if (bCheckOnOS)
                    {
                        try
                        {
                            var split = ret.UserName.Split(new char[] { '\\' });
                            if (split.Length > 1)
                            {
                                using (var pc = new PrincipalContext(ContextType.Domain, split[0]))
                                {
                                    // validate the credentials
                                    isAuthorized = pc.ValidateCredentials(split[1], ret.Password);
                                }
                            }
                            else
                            {
                                using (var pc = new PrincipalContext(ContextType.Machine))
                                {
                                    // validate the credentials
                                    isAuthorized = pc.ValidateCredentials(ret.UserName, ret.Password);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            isAuthorized = false;
                        }
                    }
                }

                if (isAuthorized && bApplyUserIndentity)
                {
                    Roles.ApplicationName = applicationName;

                    var roles = Roles.GetRolesForUser(ret.UserName);
                    if (roles.Length == 0)
                    {
                        //var groups = UserPrincipal.Current.GetGroups();
                        //var roles = groups.Select(x => x.SamAccountName).ToArray();
                        roles = Utilities.CurrentUser.GetGroups(ret.UserName);
                    }

                    var ngi = new GenericPrincipal(new GenericIdentity(ret.UserName), roles);
                    if (firsttime)
                    {
                        firsttime = false;
                        AppDomain.CurrentDomain.SetThreadPrincipal(ngi);
                    }
                    System.Threading.Thread.CurrentPrincipal = ngi;

                    OnUserOnline(applicationName, ret.UserName, ret.Password);

                    var key = String.Format("{0}@{1}", applicationName, ret.UserName);
                    lock (currentMap)
                    {
                        var list = (from c in currentMap where c.Key.StartsWith(applicationName) select c.Key).ToList();
                        list.ForEach(c => currentMap.Remove(c));
                        if (currentMap.ContainsKey(key))
                            currentMap.Remove(key);
                        currentMap.Add(key, ret.Password);
                    }
                }
            }

            if (isAuthorized)
            {
                return LoginAttemptResult.Success;
            }
            else if (bRetryOnFailed)
            {
                IUFUserEditorManager userManager = document?.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                if (userManager != null)
                    userManager.EnsureCredentialProvider(document);
                return ProcessCredentials(false, applicationName, document, user: ret.UserName, psw: ret.Password, bRetryOnFailed: false, bApplyUserIndentity: bApplyUserIndentity);
            }
            else
            {
                var usr = Membership.GetUser(ret.UserName);
                bool isLocked = usr != null && usr.IsLockedOut;
                if (bApplyUserIndentity)
                    OnFailedLogin(applicationName, ret.UserName, isLocked);
                if (!isLocked && ++failedAttemptsCounter > 3)
                    System.Threading.Thread.Sleep((failedAttemptsCounter - 3) * 2000);
                return LoginAttemptResult.Failed;
            }
        }

        string stringPlaceolder = "UserManagement";
        public ClientFormsAuthenticationCredentials ShowLoginWindow(IDocument document, String requestedRoleOrUser = null, int requestedLevel = 0, Window owner = null)
        {
            string requestedRole = requestedRoleOrUser;
            string requestedUser = null;
            if (!String.IsNullOrEmpty(requestedRole) && requestedRole.IndexOf('\\') != -1)
            {
                var index = requestedRoleOrUser.IndexOf('\\');
                requestedRole = requestedRoleOrUser.Substring(0, index);
                requestedUser = requestedRoleOrUser.Substring(index + 1);
            }

            if (owner == null)
            {
                var ie = Keyboard.FocusedElement as DependencyObject;
                if (ie != null)
                    owner = Window.GetWindow(ie);
            }

            var loginInfo = new LoginInfoEventArgs()
            {
                requestedRole = requestedRole,
                requestedUser = requestedUser,
                requestedLevel = requestedLevel
            };
            OnPromptUserLogin(loginInfo);
            if (loginInfo.requestingUserAuthentication)
            {
                if (!loginInfo.dialogResult)
                {
                    return null;
                }

                return new ClientFormsAuthenticationCredentials(loginInfo.User, loginInfo.Password, false);
            }

            var loginWindow = new LoginControl(document);
            if (String.IsNullOrEmpty(requestedRoleOrUser) && requestedLevel <= 0)
            {
                loginWindow.gridInfoRequested.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                if (!String.IsNullOrEmpty(requestedUser))
                {
                    loginWindow.txtUser.Text = requestedUser;
                    loginWindow.txtUser.IsEnabled = false;
                }

                if (!String.IsNullOrEmpty(requestedRole))
                {
                    loginWindow.txtRequestedRole.Text = requestedRole;
                    loginWindow.gridRole.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    loginWindow.gridRole.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (requestedLevel > 0)
                {
                    loginWindow.txtRequestedLevel.Text = String.Format("{0}", requestedLevel);
                    loginWindow.gridLevel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    loginWindow.gridLevel.Visibility = System.Windows.Visibility.Collapsed;
                }
                loginWindow.gridInfoRequested.Visibility = System.Windows.Visibility.Visible;
            }

            IStringEditorManager stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            var stringlist = stringManager?.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
            var title = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LoginWindowTitle", stringlist, Properties.Resources.LoginTitle);
            var captions = new Dictionary<GeneralDialogButtons, String>()
                            {
                                { GeneralDialogButtons.OkButton, TranslationHelper.TranlslateText("_UserDialog_OkLabel", stringlist, WPFUtilities.Properties.Resources.LabelOk) },
                                { GeneralDialogButtons.CancelButton, TranslationHelper.TranlslateText("_UserDialog_CancelLabel", stringlist, WPFUtilities.Properties.Resources.LabelCancel) }
                            };
            GeneralDialogContent Dialog = new GeneralDialogContent(loginWindow, WPFUtilities.Properties.Settings.Default.DialogFontSize, WPFUtilities.Properties.Settings.Default.ButtonsWidth, WPFUtilities.Properties.Settings.Default.ButtonsHeight,
            GeneralDialogButtons.OkCancelButtons, captions)
            {
                Owner = owner,
                Title = title,
                HelpLink = "UserLogin"
            };
            var ret = Dialog.ShowDialog();
            if (ret == false)
                return null;

            return loginWindow.GetCredentials();
        }

        public string RefreshCurrentUser(String applicationName)
        {
            String username = String.Empty;
            if (IsFaulted(applicationName))
                return username;

            applicationName = GetSharedApplicationName(applicationName);

            try
            {
                lock (lockObject)
                {
                    //Membership.ApplicationName = applicationName;
                    //var ret = Membership.GetUser(false);
                    //if (ret != null)
                    //{
                    //    bool online = false;
                    //    var psw = String.Empty;
                    //    var key = String.Format("{0}@{1}", applicationName, ret.UserName);
                    //    lock (currentMap)
                    //    {
                    //        if (currentMap.ContainsKey(key))
                    //        {
                    //            psw = currentMap[key];
                    //            online = true;
                    //        }
                    //    }

                    //    if (online)
                    //        OnUserOnline(applicationName, ret.UserName, psw, true);
                    //    else
                    //        OnUserOnline(applicationName, String.Empty, String.Empty, true);
                    //}
                    //else
                    {
                        String userpsw = String.Empty;
                        lock (currentMap)
                        {
                            var list = (from c in currentMap where c.Key.StartsWith(applicationName) select c).ToList();
                            if (list.Count > 0)
                            {
                                username = list[0].Key.Split('@')[1];
                                userpsw = list[0].Value;
                            }
                        }

                        if (!String.IsNullOrEmpty(username))
                            OnUserOnline(applicationName, username, userpsw, true);
                        else
                            OnUserOnline(applicationName, String.Empty, String.Empty, true);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToFaulted(applicationName);
            }
            return username;
        }
#endif

        public bool CreateUser(String applicationName, String userName, String password, bool isApproved = true)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                var user = Membership.CreateUser(userName, password);
                if (user.IsApproved != isApproved)
                {
                    user.IsApproved = isApproved;
                    Membership.UpdateUser(user);
                }
                return true;
            }
        }

        public bool DeleteUser(String applicationName, String userName)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Membership.DeleteUser(userName);
                return true;
            }
        }

        public IEnumerable GetAllUsers(String applicationName)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                return Membership.GetAllUsers();
            }
        }

#if !IO_Server
        public int GetNumberOfUsersOnline(String applicationName)
        {
            if (IsFaulted(applicationName))
                return -1;

            applicationName = GetSharedApplicationName(applicationName);

            try
            {
                lock (lockObject)
                {
                    //Membership.ApplicationName = applicationName;
                    //var ret = Membership.GetUser(true);
                    //if (ret != null)
                    //    return 1;

                    lock (currentMap)
                    {
                        var list = (from c in currentMap where c.Key.StartsWith(applicationName) select c).ToList();
                        if (list.Count > 0)
                            return list.Count;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                AddToFaulted(applicationName);
                return -1;
            }
        }

        public void Logout(String applicationName)
        {
            if (IsFaulted(applicationName))
                return;

            applicationName = GetSharedApplicationName(applicationName);

            try
            {
                lock (lockObject)
                {
                    ClientFormsAuthenticationMembershipProvider authProvider =
                            Membership.Provider as ClientFormsAuthenticationMembershipProvider;

                    try
                    {
                        Membership.ApplicationName = applicationName;
                        var ret = Membership.GetUser(true);
                        if (ret != null)
                        {
                            if (authProvider != null)
                                authProvider.Logout();

                            OnUserOnline(applicationName, String.Empty, String.Empty);

                            var key = String.Format("{0}@{1}", applicationName, ret.UserName);
                            lock (currentMap)
                            {
                                if (currentMap.ContainsKey(key))
                                    currentMap.Remove(key);
                            }
                        }
                        else
                        {
                            OnUserOnline(applicationName, String.Empty, String.Empty);

                            lock(currentMap)
                            {
                                var list = (from c in currentMap where c.Key.StartsWith(applicationName) select c.Key).ToList();
                                list.ForEach(app => currentMap.Remove(app));
                            }
                            //var key = String.Format("{0}{1}", applicationName, ret.UserName);
                            //lock (currentMap)
                            //{
                            //    if (currentMap.ContainsKey(key))
                            //        currentMap.Remove(key);
                            //}
                        }

                        Utilities.CurrentUser.CleanUserGroupsCache();

                        var ngi = new GenericPrincipal(new GenericIdentity(""), null);
                        System.Threading.Thread.CurrentPrincipal = ngi;
                    }
                    catch (WebException)
                    {
                        ConnectivityStatus.IsOffline = true;
                        if (authProvider != null)
                            authProvider.Logout();
                        ConnectivityStatus.IsOffline = false;
                        OnUserOnline(applicationName, String.Empty, String.Empty);
                        Utilities.CurrentUser.CleanUserGroupsCache();
                    }
                }
            }
            catch (Exception ex)
            {
                AddToFaulted(applicationName);                
            }
        }
#endif

        public bool IsUserLocked(String applicationName, String userName)
        {
#if !IO_Server
            if (IsFaulted(applicationName) || String.IsNullOrEmpty(userName))
                return false;

            applicationName = GetSharedApplicationName(applicationName);
#endif

            try
            {
                lock (lockObject)
                {
                    Membership.ApplicationName = applicationName;
                    MembershipUser me = Membership.GetUser(userName);
                    return me != null && me.IsLockedOut;
                }
            }
            catch (Exception ex)
            {
#if !IO_Server
                AddToFaulted(applicationName);
#endif
                return false;
            }
        }

        public bool LockUser(String applicationName, String userName)
        {
#if !IO_Server
            if (IsFaulted(applicationName) || String.IsNullOrEmpty(userName))
                return false;

            applicationName = GetSharedApplicationName(applicationName);
#endif

            try
            {
                lock (lockObject)
                {
                    Membership.ApplicationName = applicationName;
                    MembershipUser me = Membership.GetUser(userName);
                    if (me != null && !me.IsLockedOut && me.IsApproved)
                    {
                        var lockPassword = Guid.NewGuid().ToString();
                        while (Membership.ValidateUser(userName, lockPassword))
                            lockPassword = Guid.NewGuid().ToString();

                        while (true)
                        {
                            me = Membership.GetUser(userName);
                            if (me == null || me.IsLockedOut || !me.IsApproved)
                                break;

                            Membership.ValidateUser(userName, lockPassword);
                        }
                    }

                    return me != null && me.IsLockedOut;
                }
            }
            catch (Exception ex)
            {
#if !IO_Server
                AddToFaulted(applicationName);
#endif
                return false;
            }
        }

        public bool UnlockUser(String applicationName, String userName)
        {
#if !IO_Server
            if (IsFaulted(applicationName) || String.IsNullOrEmpty(userName))
                return false;

            applicationName = GetSharedApplicationName(applicationName);
#endif

            try
            {
                lock (lockObject)
                {
                    Membership.ApplicationName = applicationName;
                    MembershipUser me = Membership.GetUser(userName);
                    var bRet = me != null && me.IsLockedOut && me.UnlockUser();
#if !IO_Server
                    if (bRet)
                        OnUserUnlocked(applicationName, userName);
#endif
                    return bRet;
                }
            }
            catch (Exception ex)
            {
#if !IO_Server
                AddToFaulted(applicationName);
#endif
                return false;
            }
        }

        public bool IsCurrentUserInRole(String applicationName, String role)
        {
#if !IO_Server
            if (IsFaulted(applicationName))
                return false;

            applicationName = GetSharedApplicationName(applicationName);
#endif

            try
            {
                lock (lockObject)
                {
                    Membership.ApplicationName = applicationName;
                    Roles.ApplicationName = applicationName;
                    return System.Threading.Thread.CurrentPrincipal.IsInRole(role);
                }
            }
            catch (Exception ex)
            {
#if !IO_Server
                AddToFaulted(applicationName);
#endif
                return false;
            }
        }

        public bool IsUserInRole(String applicationName, String username, String roleName)
        {
#if !IO_Server
            if (IsFaulted(applicationName))
                return false;

            applicationName = GetSharedApplicationName(applicationName);
#endif

            try
            {
                lock (lockObject)
                {
                    Membership.ApplicationName = applicationName;
                    Roles.ApplicationName = applicationName;
                    return Roles.IsUserInRole(username, roleName);
                }
            }
            catch (Exception ex)
            {
#if !IO_Server
                AddToFaulted(applicationName);
#endif
                return false;
            }
        }

        public bool AddUsersToRoles(String applicationName, String[] userNames, String[] roleNames)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                Roles.AddUsersToRoles(userNames, roleNames);
                return true;
            }
        }

        public bool RemoveUsersFromRoles(String applicationName, String[] userNames, String[] roleNames)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                Roles.RemoveUsersFromRoles(userNames, roleNames);
                return true;
            }
        }

        public bool CreateRole(String applicationName, String role)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                Roles.CreateRole(role);
                return true;
            }
        }

        public bool DeleteRole(String applicationName, String role)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                Roles.DeleteRole(role);
                return true;
            }
        }

        public bool RoleExists(String applicationName, String role)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                return Roles.RoleExists(role);
            }
        }

        public IEnumerable GetAllRoles(String applicationName)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                return Roles.GetAllRoles();
            }
        }

        public String[] GetRolesForUser(String applicationName, String user)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                return Roles.GetRolesForUser(user);
            }
        }

        public String[] GetUsersInRole(String applicationName, String role)
        {
#if !IO_Server
            applicationName = GetSharedApplicationName(applicationName);
#endif

            lock (lockObject)
            {
                Membership.ApplicationName = applicationName;
                Roles.ApplicationName = applicationName;
                return Roles.GetUsersInRole(role);
            }
        }
        #endregion
    }
}
