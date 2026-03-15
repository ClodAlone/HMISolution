using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Web;
using System.Security.Principal;
using System.Web.Security;
using Utilities;
using WPFUtilities;
using DocumentManager.ComponentService;
using WPFScreenSink;

namespace UFWebClient.HTML5
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ScreenSinkService
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        static Object sessionLock = new Object();

        ScreenSinkServiceSession GetSession(String clientId, bool createnew = false)
        {
            lock (sessionLock)
            {
                var ret = Global.GetSession(clientId);
                if (createnew && ret == null)
                {
                    ret = new ScreenSinkServiceSession(Global.Theme, clientId);
                    Global.AddSession(clientId, ret);
                    if (Global.projectDocument != null)
                    {
                        ret.currentPage = Global.projectDocument.MakeAbosoluteUri(Global.currentPage, Global.ScreenComponent);
                    }
                    else
                        ret.currentPage = Global.currentPage;

                    ret.ScreenSink.Recycling += (o, e) =>
                        {
                            RemoveSession(clientId);
                        };
                }

                return ret;
            }
        }

        void RemoveSession(String clientId)
        {
            lock (sessionLock)
            {
                var ret = Global.GetSession(clientId);
                if (ret != null)
                {
                    ret.Dispose();
                    Global.RemoveSession(clientId);
                }
            }
        }

        [OperationContract]
        public ElementData Register(int width, int height)
        {
            String clientId = Guid.NewGuid().ToString();
            var session = GetSession(clientId, true);

            if (Global.ClientSessionName != null)
                session.ScreenSink.ClientSessionName = Global.ClientSessionName;
            if (Global.RefreshPollingTime != 0)
                session.ScreenSink.RefreshPollingTime = Global.RefreshPollingTime;
            if (Global.SessionTimeout != 0)
                session.ScreenSink.SessionTimeout = Global.SessionTimeout;
            if (Global.LowResolution == true)
                session.ScreenSink.LowResolution = Global.LowResolution;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                try
                {
                    var mbsu = Membership.GetUser(HttpContext.Current.User.Identity.Name);
                    session.ScreenSink.Password = mbsu.GetPassword();
                    session.ScreenSink.User = HttpContext.Current.User.Identity.Name;

                    session.ScreenSink.Role = Global.UFUserEditorComponent.GetUserRole(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessMask = Global.UFUserEditorComponent.GetUserAccessMask(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.AccessLevel = Global.UFUserEditorComponent.GetUserAccessLevel(Global.projectDocument, session.ScreenSink.User);
                    session.ScreenSink.Culture = Global.UFUserEditorComponent.GetUserCultureName(Global.projectDocument, session.ScreenSink.User);

                    session.ScreenSink.ClientSessionName = String.Format("{0}-{1}",
                        session.ScreenSink.ClientSessionName, HttpContext.Current.User.Identity.Name);
                }
                catch
                { }
            }
            var size = new Size();
            Global.currentPage = Global.projectDocument.MakeAbosoluteUri(Global.currentPage, Global.ScreenComponent);
            var UIElements = session.ScreenSink.OpenUri(Global.currentPage, ref size, width, height,
                Global.projectDocument.fileSystemProviderBase, Global.projectDocument,
                Global.historiansettings,
                Global.eventsettings,
                Global.serverentitysettings,
                Global.schedulersettings);

            if (UIElements != null)
            {
                lock (session.lockObject)
                {
                    session.mapIdToUIElements.Add(clientId, UIElements);
                }
            }
            return new ElementData()
            {
                id = clientId,
                storageid = GetStorageName(Global.currentPage, session.ScreenSink.IsValid(), session.ScreenSink.User), 
                width = size.Width, height = size.Height 
            };
        }

        [OperationContract(IsOneWay=true)]
        public void Unregister(String clientId)
        {
            ScreenSink.DisposeClientId(clientId);
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
        }

        [OperationContract(IsOneWay = true)]
        public void CloseUri(String clientId, String url)
        {
            //CloseCurrentUri(clientId); Session is null on unregister
            //RemoveSession(clientId);
            var session = GetSession(clientId);
            if (session == null || Global.projectDocument == null)
                return;

            var uri = Global.projectDocument.MakeAbosoluteUri(new Uri(url, UriKind.RelativeOrAbsolute), Global.ScreenComponent);
            session.ScreenSink.CloseUri(uri);
        }

        private void CloseCurrentUri(String clientId)
        {
            var session = GetSession(clientId);
            if (session == null)
                return;
            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(clientId))
                    session.mapIdToUIElements.Remove(clientId);
            }

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

        [OperationContract]
        public ElementData OpenUri(String clientId, String uri, int width, int height)
        {
            CloseCurrentUri(clientId);
            var session = GetSession(clientId);
            if (session == null)
                return new ElementData();

            if (String.IsNullOrEmpty(uri))
                uri = Global.currentPage.GetPathString();

            if (Global.projectDocument != null)
            {
                session.currentPage = Global.projectDocument.MakeAbosoluteUri(new Uri(uri, UriKind.RelativeOrAbsolute), Global.ScreenComponent);
            }
            else
                session.currentPage = new Uri(uri, UriKind.RelativeOrAbsolute);

            var size = new Size();
            var UIElements = session.ScreenSink.OpenUri(session.currentPage, ref size, width, height,
                Global.projectDocument.fileSystemProviderBase, Global.projectDocument,
                Global.historiansettings,
                Global.eventsettings,
                Global.serverentitysettings,
                Global.schedulersettings);

            if (UIElements == null)
                return null;

            lock (session.lockObject)
            {
                session.mapIdToUIElements.Add(clientId, UIElements);
            }

            return new ElementData()
            {
                id = clientId,
                storageid = GetStorageName(session.currentPage, session.ScreenSink.IsValid(), session.ScreenSink.User), 
                width = size.Width, height = size.Height 
            };
        }

        [OperationContract]
        public IList<ElementData> GetElementData(String clientId)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;
            var listData = new List<ElementData>();
            lock (session.lockObject)
            {
                if (session.mapIdToUIElements.ContainsKey(clientId))
                {
                    var list = session.mapIdToUIElements[clientId];
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

            return listData;
        }

        [OperationContract]
        public IList<String> GetListChanges(String clientId)
        {
            var session = GetSession(clientId);
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

        [OperationContract]
        public IList<ElementData> GetListStatusChanges(String clientId)
        {
            var session = GetSession(clientId);
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

        [OperationContract]
        public IList<CommandData> GetListCommands(String clientId, String url)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;

            Uri currenturi = null;
            if (String.IsNullOrEmpty(url))
                currenturi = session.currentPage;
            else
            {
                if (Global.projectDocument != null)
                {
                    currenturi = Global.projectDocument.MakeAbosoluteUri(new Uri(url, UriKind.RelativeOrAbsolute), Global.ScreenComponent);
                }
                else
                    currenturi = new Uri(url, UriKind.RelativeOrAbsolute);
            }

            var commands = session.ScreenSink.ListPendingCommands(currenturi);
            var ret = new List<CommandData>();
            foreach (var command in commands)
            {                
                if (Global.projectDocument != null)
                {
                    currenturi = Global.projectDocument.MakeAbosoluteUri(new Uri(command.Key, UriKind.RelativeOrAbsolute), Global.ScreenComponent);
                }
                else
                    currenturi = new Uri(command.Key, UriKind.RelativeOrAbsolute);
                var size = ScreenSink.GetScreenSize(currenturi, Global.projectDocument.fileSystemProviderBase);
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
        
        [OperationContract]
        public ElementData GetImageBase64(String clientId, String imageId)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;
            var Guid = new Guid(imageId);
            var image = session.ScreenSink.GetImageBase64(Guid);
            var rect = session.ScreenSink.GetStartingPoint(Guid);
            if (rect.IsEmpty)
                return null;

            var elData = new ElementData()
            {
                id = imageId,
                top = rect.Y,
                left = rect.X,
                width = rect.Width,
                height = rect.Height,
                imageData = String.Format("data:image/png;base64,{0}", image)
            };
            return elData;
        }

        [OperationContract]
        public ElementData GetBackground(String clientId)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;
            var image = session.ScreenSink.GetBackground(session.currentPage);
            var ret = new ElementData()
            {
                imageData = String.Format("data:image/png;base64,{0}", image),
                id = Guid.NewGuid().ToString()
            };

            return ret;
        }

        [OperationContract]
        public void SendMouseDownEvent(String clientId, int x, int y)
        {
            var session = GetSession(clientId);
            if (session == null)
                return;
            session.ScreenSink.SimulateEvent(session.currentPage,
                            new Point(x, y), MouseButton.Left, true);
        }

        [OperationContract]
        public DataInfo SendMouseUpEvent(String clientId, int x, int y)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;

            var data = session.ScreenSink.SimulateEvent(session.currentPage,
                new Point(x, y), MouseButton.Left, false);
            if (data != null && data.hasValueProvider)
            {
                var ret = new DataInfo()
                {
                    value = data.value,
                    X = data.X,
                    Y = data.Y
                };
                
                session.ScreenSink.SimulateEvent(session.currentPage,
                    new Point(x, y), MouseButton.Left, false);
                return ret;
            }
            
            return null;
        }

        [OperationContract]
        DataInfo SendQueryDataInfo(String clientId, String dataId)
        {
            var session = GetSession(clientId);
            if (session == null)
                return null;
            var Guid = new Guid(dataId);
            var data = session.ScreenSink.GetDataValueAndRange(Guid);
            var ret = new DataInfo()
            {
                value = data.value,
                minValue = data.minValue,
                maxValue = data.maxValue
            };

            return ret;
        }

        [OperationContract]
        String SendDataValue(String clientId, String dataId, String value)
        {
            var session = GetSession(clientId);
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

        [OperationContract]
        String SendDataValueProvider(String clientId, int x, int y, String value)
        {
            var session = GetSession(clientId);
            if (session == null)
                return "No valid session available";
            try
            {
                return session.ScreenSink.SendDataValueProvider(session.currentPage,
                            new Point(x, y), value);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [OperationContract]
        public IDictionary<String, IList<TileInfo>> GetListTileInfo()
        {
            if (Global.projectDocument == null)
                return null;

            return Global.mapTiles;
        }
    }
}
