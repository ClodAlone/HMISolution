using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Windows;
using System.Windows.Input;
using Utilities;
using WPFScreenSink;
using DocumentManager.ComponentService;
using log4net;
using ExternalAuthentication.Services;
using ExternalAuthentication.Model;
using System.Data;
using Opc.Ua;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace UFWebClient.Service
{
    public class ScreenHub : Hub
    {
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.Server);
        static int nCounterLoginFailed = 0;
        static DateTime lastCredentialsProviderMapping = DateTime.MinValue;
        static ExternalIdPUserSettings _externalIdpUserSettings;
        private ExternalIdPUserSettings ExternalIdPUserSettings { 
            get
            {
                if (_externalIdpUserSettings == null)
                    _externalIdpUserSettings = GetExternalAuthenticationConfiguration();
                return _externalIdpUserSettings;
            }
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            Debug.WriteLine("New connection : " + Context.ConnectionId);
            AddSession(Context.ConnectionId);
            return base.OnConnected();
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Disconnection : " + Context.ConnectionId);
            RemoveSession(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        public override System.Threading.Tasks.Task OnReconnected()
        {
            Debug.WriteLine("Reconnection : " + Context.ConnectionId);
            //RemoveSession(Context.ConnectionId);
            //AddSession(Context.ConnectionId);
            return base.OnReconnected();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    RemoveSession(Context.ConnectionId);
        //    // base.Dispose();
        //}

        #region Sessions
        void AddSession(String clientId)
        {
            var ret = UFWebClientService.GetSession(clientId);
            if (ret == null)
            {
                ret = new ScreenSinkServiceSession(UFWebClientService.Theme, clientId, UFWebClientService.DisableStaticOptimization);
                var sessions = UFWebClientService.AddSession(clientId, ret);
                if (UFWebClientService.projectDocument != null)
                {
                    ret.defaultPage = UFWebClientService.projectDocument.MakeAbosoluteUri(UFWebClientService.currentPage, UFWebClientService.ScreenComponent);
                    ret.projectDocuments = new Dictionary<IDocument, UFProjectManager.UFProjectDocument>();

                    var projectDocument = UFProjectManager.UFProjectDocument.FromFile(UFWebClientService.projectDocument.ProjectPath, UFWebClientService.UFProjectManagerComponent);
                    ret.projectDocuments.Add(UFWebClientService.projectDocument, projectDocument);

                    projectDocument.SetCurrentLogFileName();
                    projectDocument.UpdateSessionSettings();

                    OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
                    OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
                    OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();

                    foreach (UFProjectManager.UFProjectDocument child in projectDocument.Childs)
                        SubscribeChildProjects(child, projectDocument, ret.projectDocuments);
                }
                else
                    ret.defaultPage = UFWebClientService.currentPage;

                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, sessions);

                string url = Context.QueryString["url"];
                if (!String.IsNullOrEmpty(url))
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri)
                        uri = UFWebClientService.projectDocument.MakeRelativeUri(uri, UFWebClientService.ScreenComponent);
                    // if (UFWebClientService.projectDocument.IsVisible(uri))
                    {
                        ret.defaultPage = uri;
                    }
                }

                var BroadcastInterval = TimeSpan.FromMilliseconds(UFWebClientService.DelayBroadcaster);
                ret._broadcastLoop = new Timer(
                    BroadcastTimer,
                    clientId,
                    BroadcastInterval,
                    BroadcastInterval);

                if (ret.InDemoMode)
                {
                    var demoModeInterval = TimeSpan.FromSeconds(UFWebClientService.DemoModeInterval);
                    ret._demoModeLoop = new Timer(
                        ShowDemoMode,
                        clientId,
                        TimeSpan.Zero,
                        demoModeInterval);
                }

                //ret.ScreenSink.Recycling += (o, e) =>
                //{
                //    RemoveSession(clientId);
                //};

                ret.ScreenSink.PendingChanges += OnPendingChanges;
                ret.ScreenSink.PendingStatusChanges += OnPendingStatusChanges;
            }
        }

        void SubscribeChildProjects(UFProjectManager.UFProjectDocument project, IDocument parent, IDictionary<IDocument, UFProjectManager.UFProjectDocument> map)
        {
            var projectDocument = UFProjectManager.UFProjectDocument.FromFile(project.ProjectPath, UFWebClientService.UFProjectManagerComponent);
            projectDocument.Parent = parent;
            map.Add(project, projectDocument);

            projectDocument.SetCurrentLogFileName();
            projectDocument.UpdateSessionSettings();

            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();

            foreach (UFProjectManager.UFProjectDocument child in projectDocument.Childs)
                SubscribeChildProjects(child, projectDocument, map);
        }

        void OnPendingChanges(object sender, EventArgs e)
        {
            var ret = GetSession();
            if (ret == null)
                return;

            var list = ret.ScreenSink.ListChanges()?.ToList();
            if (list == null)
                return;
            lock (ret.lockObject)
            {
                foreach (var guid in list)
                {
                    if (!ret.listChanges.Contains(guid))
                        ret.listChanges.Add(guid);
                }
            }
        }

        void OnPendingStatusChanges(object sender, EventArgs e)
        {
            var ret = GetSession();
            if (ret == null)
                return;

            var list = GetListStatusChanges()?.ToList();
            if (list == null)
                return;
            lock (ret.lockObject)
            {
                ret.listStatusChanges.AddRange(list);
            }
        }

        public void BroadcastTimer(object state)
        {
            var clientId = state as String;
            var ret = UFWebClientService.GetSession(clientId);
            if (ret == null)
                return;

            lock (ret.lockObject)
            {
                if (ret.listChanges.Count > 0)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("SENDING PENDING CHANGES FOR CLIENT {0}, EVENT COUNT {1}, TIME {2}",
                        ret.sessionid, ret.listChanges.Count, DateTime.Now));
#endif
                    Clients.Caller.PendingChanges(ret.listChanges);
                    ret.listChanges.Clear();

#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("SENDING PENDING CHANGES FOR CLIENT {0}, SENT, TIME {2}",
                        ret.sessionid, ret.listChanges.Count, DateTime.Now));
#endif
                }

                if (ret.listStatusChanges.Count > 0)
                {
                    var distinct = ret.listStatusChanges
                      .GroupBy(p => new { p.id })
                      .Select(g => g.Last())
                      .ToList();

#if DEBUG
                    System.Diagnostics.Debug.WriteLine(String.Format("SENDING PENDING CHANGES STATUS FOR CLIENT {0}, EVENT COUNT {1}, TIME {2}",
                        ret.sessionid, distinct.Count, DateTime.Now));
#endif
                    Clients.Caller.PendingStatusChanges(distinct);
                    ret.listStatusChanges.Clear();
                }
            }
        }

        void ShowDemoMode(object state)
        {
            var clientId = state as String;
            var ret = UFWebClientService.GetSession(clientId);
            if (ret == null)
                return;

            if (ret.InDemoMode)
            {
                ret.lastDemoMode = DateTime.UtcNow;
                ret.clickCounterInDemoMode = 0;
                Clients.Caller.ShowDemoMode();
            }
        }

        void RemoveSession(String clientId)
        {
            var ret = UFWebClientService.GetSession(clientId);
            if (ret != null)
            {
                ret.ScreenSink.PendingChanges -= OnPendingChanges;
                ret.ScreenSink.PendingStatusChanges -= OnPendingStatusChanges;

                ret.Dispose();
                var sessions = UFWebClientService.RemoveSession(clientId);
                if (ret._broadcastLoop != null)
                {
                    ret._broadcastLoop.Dispose();
                    ret._broadcastLoop = null;
                }
                if (ret._demoModeLoop != null)
                {
                    ret._demoModeLoop.Dispose();
                    ret._demoModeLoop = null;
                }
                if (ret.projectDocuments != null)
                {
                    foreach (var doc in ret.projectDocuments.Values)
                        doc.Dispose();
                    ret.projectDocuments.Clear();
                }

                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, sessions);
            }
        }

        ScreenSinkServiceSession GetSession()
        {
            return UFWebClientService.GetSession(Context.ConnectionId);
        }
        #endregion

        Uri ConvertToMobile(Uri uri, ScreenSinkServiceSession session)
        {
            if (!session.bIsMobile)
                return uri;

            var name = System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString);
            var newName = String.Format("{0}\\{1}{2}{3}", System.IO.Path.GetDirectoryName(uri.OriginalString), name, "Mobile", 
                System.IO.Path.GetExtension(uri.OriginalString));
            var uriNew = UFWebClientService.projectDocument.MakeAbosoluteUri(new Uri(newName, UriKind.RelativeOrAbsolute), UFWebClientService.ScreenComponent);
            if (System.IO.File.Exists(uriNew.OriginalString))
                return uriNew;
            return uri;
        }

        void CheckSettingsAndUnlockUser(string username)
        {
            var userLockType = UFWebClientService.UFUserEditorComponent.GetUserLockMode(UFWebClientService.projectDocument);
            if (userLockType == UFUserModel.UserLockType.AllUsers)
                return;

            var unlock = false;
            if (userLockType == UFUserModel.UserLockType.None)
                unlock = true;
            else if (userLockType == UFUserModel.UserLockType.OnlyEditableUsers)
            {
                var accessLevel = UFWebClientService.UFUserEditorComponent.GetMaxRuntimeEditAccessLevel(UFWebClientService.projectDocument);
                var userLevel = UFWebClientService.UFUserEditorComponent.GetUserAccessLevel(UFWebClientService.projectDocument, username);
                if (userLevel > accessLevel)
                    unlock = true;
            }

            if (unlock)
            {
                try
                {
                    var mbsu = Membership.GetUser(username);
                    if (mbsu != null)
                        mbsu.UnlockUser();
                }
                catch
                { }
            }
        }

        #region Operations
        public void EnableEvents(bool bEnable)
        {
            var session = GetSession();
            if (session == null)
                return;

            lock (session.lockObject)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("Enabling-Disabling Events FOR CLIENT {0}, Enable {1}",
                    Context.ConnectionId, bEnable));
#endif

                session.ScreenSink.EnableEvents(bEnable);
                session.ScreenSink.EnableUpdates(bEnable);
                if (bEnable)
                {
                    if (session._broadcastLoop == null)
                    {
                        var BroadcastInterval = TimeSpan.FromMilliseconds(UFWebClientService.DelayBroadcaster);
                        session._broadcastLoop = new Timer(
                            BroadcastTimer,
                            Context.ConnectionId,
                            TimeSpan.FromMilliseconds(0),
                            BroadcastInterval);
                    }
                }
                else
                {
                    if (session._broadcastLoop != null)
                    {
                        session._broadcastLoop.Dispose();
                        session._broadcastLoop = null;

                    }
                }
            }
        }

        public async Task<ElementData> Register(int width, int height, bool bMobile, String user, String password)
        {
            return await Register(width, height, bMobile, user, password, 0);
        }

        async public Task<ElementData> Register(int width, int height, bool bMobile, String user, String password, int clientTimezoneOffset)
        {
            return await Register(width, height, bMobile, user, password, clientTimezoneOffset, null);
        }

        async public Task<ElementData> Register(int width, int height, bool bMobile, String user, String password, int clientTimezoneOffset, Dictionary<string,string> externalAuthData = null)
        {
            var session = GetSession();
            if (session == null)
                return null;

            session.bIsMobile = bMobile;
            if (UFWebClientService.ClientSessionName != null)
                session.ScreenSink.ClientSessionName = UFWebClientService.ClientSessionName;
            if (UFWebClientService.RefreshPollingTime != 0)
                session.ScreenSink.RefreshPollingTime = UFWebClientService.RefreshPollingTime;
            if (UFWebClientService.RefreshPollingTimeCount != 0)
                session.ScreenSink.RefreshPollingTimeCount = UFWebClientService.RefreshPollingTimeCount;
            if (UFWebClientService.SessionTimeout != 0)
                session.ScreenSink.SessionTimeout = UFWebClientService.SessionTimeout;
            if (UFWebClientService.LowResolution == true)
                session.ScreenSink.LowResolution = UFWebClientService.LowResolution;
            session.ScreenSink.ConcurrentRenderingPipeline = UFWebClientService.ConcurrentRenderingPipeline;
            session.ScreenSink.Culture = UFWebClientService.projectDocument.GetProjectCulture();
            session.ScreenSink.Converter = UFWebClientService.projectDocument.GetProjectConverter();

            bool bValidated = true;
            bool bLocked = false;
            var invalidElementData = new ElementData() { id = Context.ConnectionId };
            if (UFWebClientService.UFUserEditorComponent.GetEnableUserManager(UFWebClientService.projectDocument))
            {
                var bExternalAuthActive = ExternalIdPUserSettings.ExternalIdentityProviderActive;
                try
                {
                    if (bExternalAuthActive)
                    {
                        var idToken = externalAuthData["idToken"];
                        var accessToken = externalAuthData["accessToken"];

                        var tokenValidationService = new TokenValidationService(
                            new ValidateTokenKeysService(new GetAppPublicKeysService(new System.Net.Http.HttpClient())),
                            new JwtValidationService());
                        var tokenValidation = await tokenValidationService.Execute(idToken, ExternalIdPUserSettings);
                        if (tokenValidation)
                        {
                            var setLoginResultFromTokensService = new SetLoginResultFromTokensService(new CreateUserTokenFromJwtService(), new GetRolesListService());
                            var externalLoginResult = setLoginResultFromTokensService.Execute(idToken, accessToken);
                            SetScreenSinkUserData(session, externalLoginResult.Username, password, externalLoginResult.Roles.FirstOrDefault(), true, externalLoginResult.UserIdentity);
                        }
                        else
                        {
                            bValidated = false;
                            invalidElementData.LastMessage = String.Format(Properties.Resources.UserErrorLogIn, user, ExternalAuthentication.Properties.Resources.TokenValidationFailed);
                            log.ErrorFormat(Properties.Resources.UserErrorLogIn, user, ExternalAuthentication.Properties.Resources.TokenValidationFailed);
                        }
                    }
                    else
                    {
                        var sharedApplicationName = UFWebClientService.UFUserEditorComponent.GetSharedApplicationName(UFWebClientService.projectDocument);
                        Membership.ApplicationName = sharedApplicationName ?? UFWebClientService.projectDocument.Title;
                        if (UFWebClientService.AllowCredentialsProviderMapping)
                        {
                            var lastChangedTime = UFWebClientService.UFUserEditorComponent.GetLastChangedTime(UFWebClientService.projectDocument);
                            if (lastCredentialsProviderMapping < lastChangedTime)
                            {
                                lastCredentialsProviderMapping = lastChangedTime;
                                UFWebClientService.UFUserEditorComponent.EnsureCredentialProvider(UFWebClientService.projectDocument);
                            }
                        }
                        CheckSettingsAndUnlockUser(user);
                        bValidated = Membership.ValidateUser(user, password);
                        if (bValidated)
                            SetScreenSinkUserData(session, user, password, UFWebClientService.UFUserEditorComponent.GetUserRole(UFWebClientService.projectDocument, user));
                        else
                        {
                            var mu = Membership.GetUser(user);
                            if (mu != null)
                                bLocked = mu.IsLockedOut;
                            if (bLocked)
                                log.Info(String.Format(Properties.Resources.UserLockedFailedLogIn, user));
                            else if (!String.IsNullOrEmpty(user))
                            {
                                var maxInvalidPasswordAttempts = UFWebClientService.UFUserEditorComponent.GetMaxInvalidPasswordAttempts(UFWebClientService.projectDocument);
                                if (++nCounterLoginFailed >= maxInvalidPasswordAttempts)
                                    log.Info(String.Format(Properties.Resources.UserFailedLogIn, user));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bValidated = false;
                    log.ErrorFormat(Properties.Resources.UserErrorLogIn, user, ex.Message);
                }
            }

            if (!bValidated)
            {
                RemoveSession(Context.ConnectionId);
                invalidElementData.validated = bValidated;
                invalidElementData.userLocked = bLocked;
                return invalidElementData;
            }
            else
                nCounterLoginFailed = 0;

            string url = Context.QueryString["url"];
            if (!String.IsNullOrEmpty(url))
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                    uri = UFWebClientService.projectDocument.MakeRelativeUri(uri, UFWebClientService.ScreenComponent);
                // if (UFWebClientService.projectDocument.IsVisible(uri))
                {
                    session.defaultPage = uri;
                }
            }

            String paramterFile = null;
            if (session.defaultPage.GetPathString().Contains('$'))
            {
                var split = session.defaultPage.GetPathString().Split('$');
                paramterFile = split[1];
                session.defaultPage = new Uri(split[0], UriKind.RelativeOrAbsolute);
            }

            session.currentPage = UFWebClientService.projectDocument.MakeAbosoluteUri(ConvertToMobile(session.defaultPage, session), UFWebClientService.ScreenComponent);
            var parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
            
            if (session.projectDocuments != null && session.projectDocuments.ContainsKey(parent))
            {
                if (session.ScreenSink.UserIdentity != null)
                    session.projectDocuments[parent].Alarms.SetUserIdentity(session.ScreenSink.UserIdentity);
                parent = session.projectDocuments[parent];
            }

            Uri relativeUri = null;
            if (session.currentPage.IsAbsoluteUri)
                relativeUri = UFWebClientService.projectDocument.MakeRelativeUri(session.currentPage, UFWebClientService.ScreenComponent);

            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentUser, session.ScreenSink.User ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentRole, session.ScreenSink.Role ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, session.ScreenSink.AccessLevel);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, session.ScreenSink.AccessMask);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relativeUri ?? session.currentPage, parent?.fileSystemProviderBase != null));

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                parent.fileSystemProviderBase, parent,
                UFWebClientService.historiansettings[parent.Title],
                UFWebClientService.eventsettings[parent.Title],
                UFWebClientService.serverentitysettings[parent.Title],
                UFWebClientService.schedulersettings[parent.Title], paramterFile, clientTimezoneOffset);

            if (UIElements != null)
            {
                lock (session.lockObject)
                {
                    session.mapIdToUIElements.Add(Context.ConnectionId, UIElements);
                }
            }
            return new ElementData()
            {
                id = Context.ConnectionId,
                storageid = GetStorageName(session.currentPage, session.ScreenSink.IsValid(), session.ScreenSink.User),
                width = size.Width,
                height = size.Height,
                resizable = session.ScreenSink.IsResizable(session.currentPage),
                validated = bValidated,
                userLocked = bLocked,
                demoMode = session.InDemoMode
            };
        }

        public ExternalIdPUserSettings GetExternalIdpUserSettings()
        {
            return ExternalIdPUserSettings.ExternalIdentityProviderActive ? ExternalIdPUserSettings : null;
        }

        public string GetExternalIdpLogoutURL(string postLogoutRedirectUri)
        {
            if (!ExternalIdPUserSettings.ExternalIdentityProviderActive)
                return null;

            var requestUrl = new RequestUrl(ExternalIdPUserSettings.LogoutEndpoint);

            var logoutUrl = ExternalAuthentication.Utilities.OidcExtensionMethods.CreateSignOutUrl(
               clientId: ExternalIdPUserSettings.LogoutEndpoint,
               requestUrl,
               postLogoutRedirectUri: postLogoutRedirectUri);

            return logoutUrl;
        }

        void SetScreenSinkUserData(ScreenSinkServiceSession session, string user, string password, string role, bool isExternalAuthenticationActive = false, UserIdentity userIdentity = null)
        {
            session.ScreenSink.User = user;
            session.ScreenSink.Password = password;
            session.ScreenSink.UserIdentity = userIdentity ?? new UserIdentity(user, password);

            session.ScreenSink.Role = isExternalAuthenticationActive ? UFWebClientService.UFUserEditorComponent.GetExternalAuthenticationUserRole(UFWebClientService.projectDocument, role) : role; ;
            session.ScreenSink.AccessMask = isExternalAuthenticationActive ? UFWebClientService.UFUserEditorComponent.GetRoleAccessMask(UFWebClientService.projectDocument, role) : UFWebClientService.UFUserEditorComponent.GetUserAccessMask(UFWebClientService.projectDocument, session.ScreenSink.User);
            session.ScreenSink.AccessLevel = isExternalAuthenticationActive ? UFWebClientService.UFUserEditorComponent.GetRoleAccessLevel(UFWebClientService.projectDocument, role) : UFWebClientService.UFUserEditorComponent.GetUserAccessLevel(UFWebClientService.projectDocument, session.ScreenSink.User);
            var userCulture = UFWebClientService.UFUserEditorComponent.GetUserCultureName(UFWebClientService.projectDocument, session.ScreenSink.User);
            if (!String.IsNullOrEmpty(userCulture))
                session.ScreenSink.Culture = userCulture;
            var userConverter = UFWebClientService.UFUserEditorComponent.GetUserConverterName(UFWebClientService.projectDocument, session.ScreenSink.User);
            if (!String.IsNullOrEmpty(userConverter))
                session.ScreenSink.Converter = userConverter;
            session.ScreenSink.ClientSessionName = String.Format("{0}-{1}",
                session.ScreenSink.User, session.ScreenSink.ClientSessionName);
        }

        public void OnResize(int width, int height)
        {
            var session = GetSession();
            if (session == null || UFWebClientService.projectDocument == null || session.currentPage == null)
                return;

            session.ScreenSink.OnResize(session.currentPage, width, height);
        }

        public void CloseUri(String url)
        {
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
            var session = GetSession();
            if (session == null || UFWebClientService.projectDocument == null)
                return;

            if (session.InDemoMode)
                ShowDemoMode(Context.ConnectionId);

            var parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
            var uri = parent.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session));
            session.ScreenSink.CloseUri(uri);
        }

        private void CloseCurrentUri()
        {
            var session = GetSession();
            if (session == null)
                return;

            if (session.InDemoMode)
                ShowDemoMode(Context.ConnectionId);

            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(Context.ConnectionId))
                    session.mapIdToUIElements.Remove(Context.ConnectionId);
            }

            session.ScreenSink.EnableEvents(false);
            session.ScreenSink.CloseUri(session.currentPage);
        }

        String GetStorageName(Uri uri, bool bIsValid, String user)
        {
            if (UFWebClientService.projectDocument.fileSystemProviderBase != null)
            {
                return String.Format("{0}-{1}-{2}-{3}", uri.OriginalString,
                    UFWebClientService.projectDocument.fileSystemProviderBase.GetLastWriteTime(
                    new VFS.FileManagerFile(UFWebClientService.projectDocument.fileSystemProviderBase, uri.OriginalString)), bIsValid, user);
            }
            else
            {
                return String.Format("{0}-{1}-{2}-{3}", uri.OriginalString,
                    System.IO.File.GetLastWriteTime(uri.OriginalString), bIsValid, user);
            }
        }

        public ElementData OpenUri(String uri, int width, int height)
        {
            return OpenUri(uri, width, height, 0);
        }
        public ElementData OpenUri(String uri, int width, int height, int clientTimezoneOffset)
        {
            CloseCurrentUri();
            var session = GetSession();
            if (session == null)
                return new ElementData();

            if (String.IsNullOrEmpty(uri))
                uri = session.defaultPage.GetPathString();

            String paramterFile = null;
            if (uri.Contains('$'))
            {
                var split = uri.Split('$');
                paramterFile = split[1];
                uri = split[0];
            }

            Uri relativeUri = null;
            IDocument parent = UFWebClientService.projectDocument;
            if (UFWebClientService.projectDocument != null)
            {
                parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
                session.currentPage = parent.MakeAbosoluteUri(ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session));
                parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
                if (session.projectDocuments != null && session.projectDocuments.ContainsKey(parent))
                    parent = session.projectDocuments[parent];

                if (session.currentPage.IsAbsoluteUri)
                    relativeUri = UFWebClientService.projectDocument.MakeRelativeUri(session.currentPage, UFWebClientService.ScreenComponent);
            }
            else
                session.currentPage = ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session);

            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relativeUri ?? session.currentPage, parent?.fileSystemProviderBase != null));

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                parent?.fileSystemProviderBase, parent,
                UFWebClientService.historiansettings[parent.Title],
                UFWebClientService.eventsettings[parent.Title],
                UFWebClientService.serverentitysettings[parent.Title],
                UFWebClientService.schedulersettings[parent.Title], paramterFile, clientTimezoneOffset);

            if (UIElements == null)
                return null;

            lock (session.lockObject)
            {
                session.mapIdToUIElements.Add(Context.ConnectionId, UIElements);
            }

            return new ElementData()
            {
                id = Context.ConnectionId,
                storageid = GetStorageName(session.currentPage, session.ScreenSink.IsValid(), session.ScreenSink.User),
                width = size.Width,
                height = size.Height,
                resizable = session.ScreenSink.IsResizable(session.currentPage)
            };
        }

        public IList<ElementData> GetElementData()
        {
            var session = GetSession();
            if (session == null)
                return null;
            var listData = new List<ElementData>();
            IList<Guid> list = null;
            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(Context.ConnectionId))
                {
                    list = session.mapIdToUIElements[Context.ConnectionId];
                    if (list == null)
                        return null;
                }
            }

            if (list != null)
            {
                foreach (var el in list)
                {
                    var rect = session.ScreenSink.GetStartingPoint(el);
                    if (rect.IsEmpty)
                        continue;

                    var status = session.ScreenSink.GetStatus(el);
                    var elData = new ElementData()
                    {
                        id = el.ToString(),
                        top = rect.Y,
                        left = rect.X,
                        width = rect.Width,
                        height = rect.Height,
                        connected = status.connected,
                        simulateEvent = status.simulateEvent,
                        writable = status.writable,
                        dataType = status.dataType,
                        LastMessage = status.LastMessage,
                        hasImage = status.hasImage
                    };

                    listData.Add(elData);
                }
            }

            session.ScreenSink.EnableEvents();
            return listData;
        }

        public IList<String> GetListChanges()
        {
            var session = GetSession();
            if (session == null)
                return null;

            var listData = new List<String>();
            var list = session.ScreenSink.ListChanges();
            if (list != null)
            {
                foreach (var el in list)
                    listData.Add(el.ToString());
            }

            return listData;
        }

        public IList<ElementData> GetListStatusChanges()
        {
            var session = GetSession();
            if (session == null)
                return null;

            var listData = new List<ElementData>();
            var list = session.ScreenSink.ListStatusChanges();
            if (list != null)
            {
                foreach (var el in list)
                {
                    var rect = session.ScreenSink.GetStartingPoint(el);
                    //if (rect.IsEmpty)
                    //    continue;

                    var status = session.ScreenSink.GetStatus(el);
                    var elData = new ElementData()
                    {
                        id = el.ToString(),
                        top = rect.Y,
                        left = rect.X,
                        width = rect.Width,
                        height = rect.Height,
                        connected = status.connected,
                        simulateEvent = status.simulateEvent,
                        writable = status.writable,
                        dataType = status.dataType,
                        LastMessage = status.LastMessage
                    };

                    listData.Add(elData);
                }
            }
            return listData;
        }

        public IDictionary<String, String>[] GetCurrentListVoiceCommands(String url, string locale)
        {
            var session = GetSession();
            if (session == null)
                return null;
            Uri currenturi = null;
            if (UFWebClientService.projectDocument != null)
            {
                var parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
                currenturi = parent.MakeAbosoluteUri(ConvertToMobile(session.currentPage, session));
            }
            else
                currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);

            return session.ScreenSink.GetCurrentListVoiceCommands(currenturi, locale);
        }

        public List<string> GetDefaultVoiceCommands()
        {
            return UFWebClientService.projectDocument != null ? UFWebClientService.projectDocument.GetDefaultSpeechCommands() : new List<string>();
        }

        public void ExecuteVoiceCommands(String url, String command)
        {
            var session = GetSession();
            if (session == null)
                return;
            Uri currenturi = null;
            if (UFWebClientService.projectDocument != null)
            {
                var parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
                currenturi = parent.MakeAbosoluteUri(ConvertToMobile(session.currentPage, session));
            }
            else
                currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);

            session.ScreenSink.ExecuteVoiceCommands(currenturi, command);
        }

        public IList<CommandData> GetListCommands(String url)
        {
            var session = GetSession();
            if (session == null)
                return null;

            if (session.InDemoModeCountDown)
                return null;

            //Uri currenturi = null;
            //if (String.IsNullOrEmpty(url))
            //    currenturi = session.currentPage;
            //else
            //{
            //    if (UFWebClientService.projectDocument != null)
            //    {
            //        currenturi = UFWebClientService.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session), UFWebClientService.ScreenComponent);
            //    }
            //    else
            //        currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);
            //}

            Uri currenturi = null;
            if (UFWebClientService.projectDocument != null)
            {
                var parent = UFWebClientService.projectDocument.UpdateParentFromUri(session.currentPage, UFWebClientService.ScreenComponent);
                currenturi = parent.MakeAbosoluteUri(ConvertToMobile(session.currentPage, session));
            }
            else
                currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);

            var currentParent = UFWebClientService.projectDocument?.UpdateParentFromUri(currenturi, UFWebClientService.ScreenComponent);
            var commands = session.ScreenSink.ListPendingCommands(currenturi);
            var ret = new List<CommandData>();
            foreach (var command in commands)
            {
                if (String.IsNullOrEmpty(command.Key))
                {
                    ret.Add(new CommandData()
                    {
                        closeCurrent = true
                    });
                    continue;
                }

                String realUriWithoutParameter = command.Key;
                if (command.Key.Contains('$'))
                {
                    var split = command.Key.Split('$');
                    realUriWithoutParameter = split[0];
                    if (String.IsNullOrEmpty(realUriWithoutParameter))
                    {
                        if (!UFWebClientService.DisableAlertOnCommandExecution)
                        {
                            ret.Add(new CommandData()
                            {
                                url = split[1],
                                isError = true
                            });
                        }
                        continue;
                    }
                }

                IDocument parent = UFWebClientService.projectDocument;
                if (UFWebClientService.projectDocument != null)
                {
                    currenturi = currentParent.MakeAbosoluteUri(ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session));
                    parent = UFWebClientService.projectDocument.UpdateParentFromUri(currenturi, UFWebClientService.ScreenComponent);
                }
                else
                    currenturi = ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session);
                var size = ScreenSink.GetScreenSize(currenturi, parent, parent?.fileSystemProviderBase);
                ret.Add(new CommandData()
                {
                    url = command.Key,
                    isSynchro = command.Value && !UFWebClientService.DisablePopupScreen,
                    width = size.Width,
                    height = size.Height
                });
            }
            return ret;
        }

        public ElementData GetImageBase64(String imageId)
        {
            var session = GetSession();
            if (session == null)
                return null;
            var Guid = new Guid(imageId);

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("GETTING IMAGE FOR CLIENT {0}, IMAGE {1}, TIME {2}",
                session.sessionid, Guid, DateTime.Now));
#endif

            var image = session.ScreenSink.GetImageBase64(Guid);
            var rect = session.ScreenSink.GetStartingPoint(Guid);
            //if (rect.IsEmpty)
            //    return null;

            var elData = new ElementData()
            {
                id = imageId,
                top = rect.Y,
                left = rect.X,
                width = rect.Width,
                height = rect.Height,
                imageData = image
                // imageData = String.Format("data:image/png;base64,{0}", image)
            };
            if (String.IsNullOrEmpty(image))
                elData.imageData = null;

            return elData;
        }

        public ElementData GetBackground()
        {
            var session = GetSession();
            if (session == null)
                return null;
            var image = session.ScreenSink.GetBackground(session.currentPage);
            var ret = new ElementData()
            {
                // imageData = String.Format("data:image/png;base64,{0}", image),
                imageData = image,
                id = session.currentPage.ToString().GetHashCode().ToString()
            };

            return ret;
        }

        public void SetZoomVisibilityItems(double zoomLevel)
        {
            var session = GetSession();
            if (session == null)
                return;
            session.ScreenSink.SetZoomVisibilityItems(session.currentPage,
                            zoomLevel);
        }

        public void SendMouseDownEvent(int x, int y)
        {
            var session = GetSession();
            if (session == null)
                return;

            if (session.InDemoModeCountDown)
                return;

            session.ScreenSink.SimulateEvent(session.currentPage,
                            new Point(x, y), MouseButton.Left, true);
        }

        public DataInfo SendMouseUpEvent(int x, int y)
        {
            var session = GetSession();
            if (session == null)
                return null;

            if (session.InDemoModeCountDown)
                return null;
            else if (++session.clickCounterInDemoMode > UFWebClientService.DemoModeClickCounter)
                ShowDemoMode(Context.ConnectionId);

            var data = session.ScreenSink.SimulateEvent(session.currentPage,
                            new Point(x, y), MouseButton.Left, false);
            if (data != null && data.hasValueProvider)
            {
                var ret = new DataInfo()
                {
                    value = data.value,
                    selection = data.selection,
                    X = data.X,
                    Y = data.Y
                };

                return ret;
            }

            return null;
        }

        public String SendDataValueProvider(int x, int y, String value)
        {
            var session = GetSession();
            if (session == null)
                return null;

            if (session.InDemoMode)
                ShowDemoMode(Context.ConnectionId);

            return session.ScreenSink.SendDataValueProvider(session.currentPage,
                            new Point(x, y), value);
        }

        public DataInfo SendQueryDataInfo(String dataId)
        {
            var session = GetSession();
            if (session == null)
                return null;

            if (session.InDemoModeCountDown)
                return null;

            var Guid = new Guid(dataId);
            var data = session.ScreenSink.GetDataValueAndRange(Guid);
            if (data == null)
                return null;
            var ret = new DataInfo()
            {
                value = data.value,
                selection = data.selection,
                minValue = data.minValue,
                maxValue = data.maxValue,
                X = data.X,
                Y = data.Y
            };

            return ret;
        }

        public String SendDataValue(String dataId, String value)
        {
            var session = GetSession();
            if (session == null)
                return "No valid session available";

            if (session.InDemoMode)
                ShowDemoMode(Context.ConnectionId);

            var Guid = new Guid(dataId);
            try
            {
                if (session.ScreenSink.SetDataValue(Guid, value))
                    return "OK";

                return "Cannot find where to write the value";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        static ExternalIdPUserSettings GetExternalAuthenticationConfiguration()
        {
            var externalIdPSettingsDictionary = UFWebClientService.UFUserEditorComponent.GetExternalAuthenticationSettings(UFWebClientService.projectDocument);
            var externalIdPUserSettings = new ExternalIdPUserSettings();

            if (externalIdPSettingsDictionary != null && externalIdPSettingsDictionary.Count > 0)
            {
                var convertGeneralSettingsToExternalIdPUserSettings = new ConvertGeneralSettingsToExternalIdPUserSettings();
                externalIdPUserSettings =
                        convertGeneralSettingsToExternalIdPUserSettings.Execute(externalIdPSettingsDictionary);
            }

            return externalIdPUserSettings;
        }
        #endregion
    }
}