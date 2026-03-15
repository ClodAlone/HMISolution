using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Windows;
using System.Windows.Input;
using UFWebClientResponsive_HTML5;
using Utilities;
using WPFScreenSink;
using DocumentManager.ComponentService;
using log4net;
using Opc.Ua;

namespace UFWebClient.HTML5.Hubs
{
    public class ScreenHub : Hub
    {
        private static readonly ILog log = LogManager.GetLogger(UFWebClient.HTML5.Properties.Resources.WebClientHTML5Log);

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
            var ret = Global.GetSession(clientId);
            if (ret == null)
            {
                string popup = Context.QueryString["sessionid"];
                ret = new ScreenSinkServiceSession(Global.Theme, clientId, Global.DisableStaticOptimization, !String.IsNullOrEmpty(popup));
                var sessions = Global.AddSession(clientId, ret);
                if (Global.projectDocument != null)
                {
                    ret.defaultPage = Global.projectDocument.MakeAbosoluteUri(Global.currentPage, Global.ScreenComponent);
                    ret.projectDocuments = new Dictionary<IDocument, UFProjectManager.UFProjectDocument>();

                    var projectDocument = UFProjectManager.UFProjectDocument.FromFile(Global.projectDocument.ProjectPath, Global.UFProjectManagerComponent);
                    ret.projectDocuments.Add(Global.projectDocument, projectDocument);

                    projectDocument.SetCurrentLogFileName();
                    projectDocument.UpdateSessionSettings();
                                        

                    OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
                    
                    if (Context.QueryString["id"] != null)
                    {
                        var parentSession = Global.GetSession(Context.QueryString["id"]);
                        if (parentSession != null)
                        {
                            if (parentSession.projectDocuments.ContainsKey(Global.projectDocument))
                            {
                                OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces(parentSession.projectDocuments[Global.projectDocument]);
                            }
                        }
                    }
                    else
                    {
                        OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
                    }

                    OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
                    
                    foreach (UFProjectManager.UFProjectDocument child in projectDocument.Childs)
                        SubscribeChildProjects(child, projectDocument, ret.projectDocuments);
                }
                else
                    ret.defaultPage = Global.currentPage;

                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, ScreenSinkServiceSession.GetLicenseInUseCounter());
                
                string url = Context.QueryString["url"];
                if (!String.IsNullOrEmpty(url))
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                    if (uri.IsAbsoluteUri)
                        uri = Global.projectDocument.MakeRelativeUri(uri, Global.ScreenComponent);
                    // if (Global.projectDocument.IsVisible(uri))
                    {
                        ret.defaultPage = uri;
                    }
                }

                var BroadcastInterval = TimeSpan.FromMilliseconds(Global.DelayBroadcaster);
                ret._broadcastLoop = new Timer(
                    BroadcastTimer,
                    clientId,
                    BroadcastInterval,
                    BroadcastInterval);

                //ret.ScreenSink.Recycling += (o, e) =>
                //{
                //    RemoveSession(clientId);
                //};
                ret.ScreenSink.PendingChanges += (o, e) =>
                {
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
                };
                ret.ScreenSink.PendingStatusChanges += (o, e) =>
                {
                    var list = GetListStatusChanges()?.ToList();
                    if (list == null)
                        return;

                    lock (ret.lockObject)
                    {
                        ret.listStatusChanges.AddRange(list);
                    }
                };
            }
        }

        public void BroadcastTimer(object state)
        {
            var clientId = state as String;
            var ret = Global.GetSession(clientId);
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
            var ret = Global.GetSession(clientId);
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
            var ret = Global.GetSession(clientId);
            if (ret != null)
            {
                ret.Dispose();
                var sessions = Global.RemoveSession(clientId);
                if (ret._broadcastLoop != null)
                {
                    ret._broadcastLoop.Dispose();
                    ret._broadcastLoop = null;
                }
                if (ret.projectDocuments != null)
                {
                    foreach (var doc in ret.projectDocuments.Values)
                        doc.Dispose();
                    ret.projectDocuments.Clear();
                }

                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, ScreenSinkServiceSession.GetLicenseInUseCounter());
            }
        }

        ScreenSinkServiceSession GetSession()
        {
            return Global.GetSession(Context.ConnectionId);
        }
        #endregion

        Uri ConvertToMobile(Uri uri, ScreenSinkServiceSession session)
        {
            if (!session.bIsMobile)
                return uri;

            var name = System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString);
            var newName = String.Format("{0}\\{1}{2}{3}", System.IO.Path.GetDirectoryName(uri.OriginalString), name, "Mobile", 
                System.IO.Path.GetExtension(uri.OriginalString));
            var uriNew = Global.projectDocument.MakeAbosoluteUri(new Uri(newName, UriKind.RelativeOrAbsolute), Global.ScreenComponent);
            if (System.IO.File.Exists(uriNew.OriginalString))
                return uriNew;
            return uri;
        }

        void SubscribeChildProjects(UFProjectManager.UFProjectDocument project, IDocument parent, IDictionary<IDocument, UFProjectManager.UFProjectDocument> map)
        {
            var projectDocument = UFProjectManager.UFProjectDocument.FromFile(project.ProjectPath, Global.UFProjectManagerComponent);
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
                        var BroadcastInterval = TimeSpan.FromMilliseconds(Global.DelayBroadcaster);
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

        public void Unregister(string clientId)
        {
            if (!Global.ForceSessionDecrementOnUnload)
                return;

            CloseCurrentUri();
            RemoveSession(clientId);
        }

        public ElementData Register(int width, int height, bool bMobile, int clientTimezoneOffset = 0)
        {
            var session = GetSession();
            if (session == null)
                return null;

            session.bIsMobile = bMobile;
            if (Global.ClientSessionName != null)
                session.ScreenSink.ClientSessionName = Global.ClientSessionName;
            if (Global.RefreshPollingTime != 0)
                session.ScreenSink.RefreshPollingTime = Global.RefreshPollingTime;
            if (Global.RefreshPollingTimeCount != 0)
                session.ScreenSink.RefreshPollingTimeCount = Global.RefreshPollingTimeCount;
            if (Global.SessionTimeout != 0)
                session.ScreenSink.SessionTimeout = Global.SessionTimeout;
            if (Global.LowResolution == true)
                session.ScreenSink.LowResolution = Global.LowResolution;
            session.ScreenSink.ConcurrentRenderingPipeline = Global.ConcurrentRenderingPipeline;
            session.ScreenSink.Culture = Global.projectDocument.GetProjectCulture();
            session.ScreenSink.Converter = Global.projectDocument.GetProjectConverter();

            int sessionTimeout = 0;
            if (HttpContext.Current.User != null && 
                HttpContext.Current.User.Identity.IsAuthenticated && 
                Global.UFUserEditorComponent.GetEnableUserManager(Global.projectDocument))
            {
                sessionTimeout = Global.UFUserEditorComponent.GetAutoLogoutSeconds(Global.projectDocument, Context.User.Identity.Name);

                try
                {
                    var mbsu = Membership.GetUser(Context.User.Identity.Name);
                    session.ScreenSink.User = Context.User.Identity.Name;
                    try
                    {
                        session.ScreenSink.Password = mbsu.GetPassword();
                        if (session.ScreenSink.User != null && session.ScreenSink.Password != null)
                            session.ScreenSink.UserIdentity = new UserIdentity(session.ScreenSink.User, session.ScreenSink.Password);
                    }
                    catch
                    { }

                    session.ScreenSink.Role = Global.UFUserEditorComponent.GetUserRole(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessMask = Global.UFUserEditorComponent.GetUserAccessMask(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessLevel = Global.UFUserEditorComponent.GetUserAccessLevel(Global.projectDocument, session.ScreenSink.User);
                    var userCulture = Global.UFUserEditorComponent.GetUserCultureName(Global.projectDocument, session.ScreenSink.User);
                    if (!String.IsNullOrEmpty(userCulture))
                        session.ScreenSink.Culture = userCulture;
                    var userConverter = Global.UFUserEditorComponent.GetUserConverterName(Global.projectDocument, session.ScreenSink.User);
                    if (!String.IsNullOrEmpty(userConverter))
                        session.ScreenSink.Converter = userConverter;
                    session.ScreenSink.ClientSessionName = String.Format("{0}-{1}",
                        Context.User.Identity.Name, session.ScreenSink.ClientSessionName);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Properties.Resources.UserErrorLogIn, Context.User.Identity.Name, ex.Message);
                }
            }

            string url = Context.QueryString["url"];
            if (!String.IsNullOrEmpty(url))
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                    uri = Global.projectDocument.MakeRelativeUri(uri, Global.ScreenComponent);
                // if (Global.projectDocument.IsVisible(uri))
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

            session.currentPage = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(session.defaultPage, session), Global.ScreenComponent);
            var parent = Global.projectDocument.UpdateParentFromUri(session.currentPage, Global.ScreenComponent);
            if (session.projectDocuments != null && session.projectDocuments.ContainsKey(parent))
            {
                if (session.ScreenSink.UserIdentity != null)
                    session.projectDocuments[parent].Alarms.SetUserIdentity(session.ScreenSink.UserIdentity);
                parent = session.projectDocuments[parent];
                
            }

            Uri relativeUri = null;
            if (session.currentPage.IsAbsoluteUri)
                relativeUri = Global.projectDocument.MakeRelativeUri(session.currentPage, Global.ScreenComponent);

            
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentConverter, session.ScreenSink.Converter ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentCulture, session.ScreenSink.Culture ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentUser, session.ScreenSink.User ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentRole, session.ScreenSink.Role ?? String.Empty);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, session.ScreenSink.AccessLevel);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, session.ScreenSink.AccessMask);
            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relativeUri ?? session.currentPage, parent?.fileSystemProviderBase != null));
            

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                parent.fileSystemProviderBase, parent,
                Global.historiansettings[parent.Title],
                Global.eventsettings[parent.Title],
                Global.serverentitysettings[parent.Title],
                Global.schedulersettings[parent.Title], paramterFile, clientTimezoneOffset);

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
                sessionTimeout = sessionTimeout,
                demoMode = session.InDemoMode
            };
        }

        public void OnResize(int width, int height)
        {
            var session = GetSession();
            if (session == null || Global.projectDocument == null || session.currentPage == null)
                return;

            session.ScreenSink.OnResize(session.currentPage, width, height);
        }

        public void CloseUri(String url)
        {
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
            var session = GetSession();
            if (session == null || Global.projectDocument == null)
                return;

            var parent = Global.projectDocument.UpdateParentFromUri(session.currentPage, Global.ScreenComponent);
            var uri = parent.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session));
            session.ScreenSink.CloseUri(uri);
        }

        public void ClosePopupUri()
        {
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
            var session = GetSession();
            if (session == null || Global.projectDocument == null)
                return;

            if (Context.QueryString["id"] != null)
            {
                var parentSession = Global.GetSession(Context.QueryString["id"]);
                if (parentSession != null)
                {
                    if (parentSession.projectDocuments.ContainsKey(Global.projectDocument))
                    {
                        if (parentSession.currentPage.IsAbsoluteUri)
                        {
                            var relativeUri = Global.projectDocument.MakeRelativeUri(parentSession.currentPage, Global.ScreenComponent);
                            SysVariables.SysVariables.GetSysVariables(parentSession.projectDocuments[Global.projectDocument]).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relativeUri ?? session.currentPage, parentSession.projectDocuments[Global.projectDocument]?.fileSystemProviderBase != null));
                        }
                    }
                }
            }
        }

        private void CloseCurrentUri()
        {
            var session = GetSession();
            if (session == null)
                return;

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
            if (Global.projectDocument.fileSystemProviderBase != null)
            {
                return String.Format("{0}-{1}-{2}-{3}", uri.OriginalString,
                    Global.projectDocument.fileSystemProviderBase.GetLastWriteTime(
                    new VFS.FileManagerFile(Global.projectDocument.fileSystemProviderBase, uri.OriginalString)), bIsValid, user);
            }
            else
            {
                return String.Format("{0}-{1}-{2}-{3}", uri.OriginalString,
                    System.IO.File.GetLastWriteTime(uri.OriginalString), bIsValid, user);
            }
        }

        public ElementData OpenUri(String uri, int width, int height, int clientTimezoneOffset, bool isFromTile = false)
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
            IDocument parent = Global.projectDocument;
            if (Global.projectDocument != null)
            {
                if (!isFromTile)
                    parent = Global.projectDocument.UpdateParentFromUri(session.currentPage, Global.ScreenComponent);
                session.currentPage = parent.MakeAbosoluteUri(ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session));
                parent = Global.projectDocument.UpdateParentFromUri(session.currentPage, Global.ScreenComponent);
                if (session.projectDocuments != null && session.projectDocuments.ContainsKey(parent))
                    parent = session.projectDocuments[parent];

                if (session.currentPage.IsAbsoluteUri)
                    relativeUri = Global.projectDocument.MakeRelativeUri(session.currentPage, Global.ScreenComponent);
            }
            else
                session.currentPage = ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session);

            SysVariables.SysVariables.GetSysVariables(parent).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, WPFUtilities.Converters.UriConverter.ToString(relativeUri ?? session.currentPage, parent?.fileSystemProviderBase != null));

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                parent?.fileSystemProviderBase, parent,
                Global.historiansettings[parent.Title],
                Global.eventsettings[parent.Title],
                Global.serverentitysettings[parent.Title],
                Global.schedulersettings[parent.Title], paramterFile, clientTimezoneOffset);

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
            //    if (Global.projectDocument != null)
            //    {
            //        currenturi = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session), Global.ScreenComponent);
            //    }
            //    else
            //        currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);
            //}

            Uri currenturi = null;
            if (Global.projectDocument != null)
            {
                var parent = Global.projectDocument.UpdateParentFromUri(session.currentPage, Global.ScreenComponent);
                currenturi = parent.MakeAbosoluteUri(ConvertToMobile(session.currentPage, session));
            }
            else
                currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);

            var currentParent = Global.projectDocument?.UpdateParentFromUri(currenturi, Global.ScreenComponent);
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
                        if (!Global.DisableAlertOnCommandExecution)
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

                IDocument parent = currentParent;
                if (Global.projectDocument != null)
                {
                    currenturi = currentParent.MakeAbosoluteUri(ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session));
                    parent = Global.projectDocument.UpdateParentFromUri(currenturi, Global.ScreenComponent);
                }
                else
                    currenturi = ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session);
                var size = ScreenSink.GetScreenSize(currenturi, parent, parent?.fileSystemProviderBase);
                ret.Add(new CommandData()
                {
                    url = command.Key,
                    isSynchro = command.Value && !Global.DisablePopupScreen,
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
                imageData = String.Format("data:image/png;base64,{0}", image)
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
                imageData = String.Format("data:image/png;base64,{0}", image),
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
            else if (++session.clickCounterInDemoMode >= Global.DemoModeClickCounter)
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
        #endregion
    }
}