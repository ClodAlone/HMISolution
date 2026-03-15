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
using Utilities;
using WPFScreenSink;

namespace UFWebClient.HTML5.Hubs
{
    public class ScreenHub : Hub
    {
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
                ret = new ScreenSinkServiceSession(Global.Theme, clientId, Global.DisableStaticOptimization);
                Global.AddSession(clientId, ret);
                if (Global.projectDocument != null)
                {
                    ret.defaultPage = Global.projectDocument.MakeAbosoluteUri(Global.currentPage, Global.ScreenComponent);
                }
                else
                    ret.defaultPage = Global.currentPage;

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
                    lock (ret.lockObject)
                    {
                        foreach (var guid in ret.ScreenSink.ListChanges().ToList())
                        {
                            if (!ret.listChanges.Contains(guid))
                                ret.listChanges.Add(guid);
                        }
                    }
                };
                ret.ScreenSink.PendingStatusChanges += (o, e) =>
                {
                    lock (ret.lockObject)
                    {
                        ret.listStatusChanges.AddRange(GetListStatusChanges().ToList());
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

        void RemoveSession(String clientId)
        {
            var ret = Global.GetSession(clientId);
            if (ret != null)
            {
                ret.Dispose();
                Global.RemoveSession(clientId);
                if (ret._broadcastLoop != null)
                {
                    ret._broadcastLoop.Dispose();
                    ret._broadcastLoop = null;
                }
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

        #region Operations
        public void EnableEvents(bool bEnable)
        {
            var session = GetSession();
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

        public ElementData Register(int width, int height, bool bMobile, int clientTimezoneOffset = 0)
        {
            var session = GetSession();
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
            
            if (HttpContext.Current.User.Identity.IsAuthenticated && 
                Global.UFUserEditorComponent.GetEnableUserManager(Global.projectDocument))
            {
                try
                {
                    var mbsu = Membership.GetUser(Context.User.Identity.Name);
                    session.ScreenSink.User = Context.User.Identity.Name;

                    session.ScreenSink.Role = Global.UFUserEditorComponent.GetUserRole(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessMask = Global.UFUserEditorComponent.GetUserAccessMask(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessLevel = Global.UFUserEditorComponent.GetUserAccessLevel(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.Culture = Global.UFUserEditorComponent.GetUserCultureName(Global.projectDocument, session.ScreenSink.User);

                    session.ScreenSink.ClientSessionName = String.Format("{0}-{1}",
                        session.ScreenSink.ClientSessionName, Context.User.Identity.Name);
                }
                catch (Exception ex)
                {
                    
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

            var size = new Size();
            session.currentPage = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(session.defaultPage, session), Global.ScreenComponent);
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                Global.projectDocument.fileSystemProviderBase, Global.projectDocument,
                Global.historiansettings,
                Global.eventsettings,
                Global.serverentitysettings,
                Global.schedulersettings, paramterFile, clientTimezoneOffset);

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
                height = size.Height
            };
        }

        public void CloseUri(String url)
        {
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
            var session = GetSession();
            if (session == null || Global.projectDocument == null)
                return;

            var uri = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session), 
                                                                Global.ScreenComponent);
            session.ScreenSink.CloseUri(uri);
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

            if (Global.projectDocument != null)
            {
                session.currentPage = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session), 
                                                    Global.ScreenComponent);
            }
            else
                session.currentPage = ConvertToMobile(new Uri(uri, UriKind.RelativeOrAbsolute), session);

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                Global.projectDocument.fileSystemProviderBase, Global.projectDocument,
                Global.historiansettings,
                Global.eventsettings,
                Global.serverentitysettings,
                Global.schedulersettings, paramterFile, clientTimezoneOffset);

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
                height = size.Height
            };
        }

        public IList<ElementData> GetElementData()
        {
            var session = GetSession();
            if (session == null)
                return null;
            var listData = new List<ElementData>();
            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(Context.ConnectionId))
                {
                    var list = session.mapIdToUIElements[Context.ConnectionId];
                    if (list == null)
                        return null;

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
                            LastMessage = status.LastMessage
                        };

                        listData.Add(elData);
                    }
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

            Uri currenturi = null;
            if (String.IsNullOrEmpty(url))
                currenturi = session.currentPage;
            else
            {
                if (Global.projectDocument != null)
                {
                    currenturi = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session), Global.ScreenComponent);
                }
                else
                    currenturi = ConvertToMobile(new Uri(url, UriKind.RelativeOrAbsolute), session);
            }

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
                }

                if (Global.projectDocument != null)
                {
                    currenturi = Global.projectDocument.MakeAbosoluteUri(ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session), Global.ScreenComponent);
                }
                else
                    currenturi = ConvertToMobile(new Uri(realUriWithoutParameter, UriKind.RelativeOrAbsolute), session);
                var size = ScreenSink.GetScreenSize(currenturi, Global.projectDocument, Global.projectDocument.fileSystemProviderBase);
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
            session.ScreenSink.SimulateEvent(session.currentPage,
                            new Point(x, y), MouseButton.Left, true);
        }

        public DataInfo SendMouseUpEvent(int x, int y)
        {
            var session = GetSession();
            if (session == null)
                return null;
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