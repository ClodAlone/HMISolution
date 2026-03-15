using Microsoft.AspNetCore.SignalR;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebNExTHMI.ImageDetection;
using WebNExTHMI.PlatformComponents;
using static WebNExTHMI.PlatformComponents.PlatformComponents;
using static Utilities.UriExtension;
using static WebNExTHMI.PlatformComponents.SchedulerData;
using UFUAModel.Extensions;

namespace WebNExTHMI.Hubs
{
    public class Data
    {
        public int status;
        public String data;
        public String controls;
        public String commands;
        public String animations;
        public String entities;
        public String style;
        public String id;
        public IDictionary<String, String> listStrings;
        public byte[] dataArray;
        public bool hashOk;
        public uint hash;
        public String role;
        public int level;
        public int mask;
        public int passwordDaysLeft;
        public String signature;
        public bool requireUserLogin;
        public long delayUnloadSecs;
        public bool keepAlwaysInMemory;
        public bool hideLayoutScreens;
        public String fullPath;
        public String title;
        public String windowState;
        public String windowStyle;
    }

    public class Configuration
    {
        public String projectTitle;
        public String projectTheme;
        public int startType;
        public String startupScreen;
        public bool requireLogin;
        public bool showDebugWindow;
        public bool forceMainPageAR;
        public bool ARScanQRCodeOnly;
        public bool ARScanQRCode;
        public int ARScanQRToleranceMS;
        public List<string> preloadedToolboxComponents;

        public bool enableUserManager;
        public int autoLogoutSeconds;
        public int minRequiredPasswordLength;
        public IEnumerable<String> availableLanguages;
        public String projectCulture;
        public int screenDelayUnloadMSecs;

        public String topScreen;
        public String bottomScreen;
        public String leftScreen;
        public String rightScreen;
        public String appbartopScreen;
        public String appbarbottomScreen;
        public String appbarleftScreen;
        public String appbarrightScreen;

        public double topScreenHeight;
        public double bottomScreenHeight;
        public double leftScreenWidth;
        public double rightScreenWidth;

        public bool restoreLastOpenScreenAndZoom;
        public bool inDemoMode;

        public String mapProvider;
        public String mapProviderKey;
        public String mapLayers;
        public int mapMaxZoomFactor;
        public int mapZoomFactor;
        public String mapBounds;
        public bool legacyMapView;
        public String mapViewTilesURL;
        public String mapViewTilesOptions;
        public int serverInvokeMinFreqOnDrag;
    }

    public class Rectangle
    {
        public double x;
        public double y;
        public double width;
        public double height;
    }

    public class CoreHub : Hub
    {
        static CoreHub()
        {
            EventEditorComponent.FireUIEvent += EventEditorComponent_FireUIEvent;
        }

        public override Task OnConnectedAsync()
        {
            Debug.WriteLine("New connection : " + Context.ConnectionId);
            var sessions = Session.Session.AddSession(Context.ConnectionId, Clients.Caller);

            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, sessions);

            return base.OnConnectedAsync();
        }

        static private void EventEditorComponent_FireUIEvent(object sender, UFEventEditor.ComponentService.EventEventArgs e)
        {
            Session.Session.SendAllExecuteUICommand(e.JsonCommand);
            e.bExecuted = true;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Debug.WriteLine("Disconnection : " + Context.ConnectionId);
            var sessions = Session.Session.RemoveSession(Context.ConnectionId);
            ImageDetection.DetectionProcessor.RemoveImageProcessing(Context.ConnectionId);

            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, sessions);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SetDefaultParameterFile(string parameterFile)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.DefaultParameterFile = parameterFile;
            });
        }

        public async Task<Configuration> GetConfiguration(bool isMobile, int clientTimezoneOffset = 0)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            session.IsMobile = isMobile;
            session.ClientTimezoneOffset = clientTimezoneOffset;

            var ret = new Configuration();
            await Task.Run(() =>
            {
                session.EnableUserManager = GetConfigurationEnableUserManager();

                ret.projectTitle = GetProjectTitle();
                ret.projectTheme = GetProjectTheme();

                ret.requireLogin = LogingRequired;
                ret.showDebugWindow = ShowDebugWindow;
                ret.forceMainPageAR = ForceMainPageAR;
                ret.preloadedToolboxComponents = PreloadedToolboxComponents;
                ret.ARScanQRCodeOnly = ARScanQRCodeOnly;
                ret.ARScanQRCode = ARScanQRCode;
                ret.ARScanQRToleranceMS = ARScanQRToleranceMS;

                ret.startType = GetConfigurationStartType();
                ret.startupScreen = GetConfigurationStartupScreen();
                ret.autoLogoutSeconds = GetConfigurationAutoLogoutSeconds();
                ret.enableUserManager = session.EnableUserManager;
                ret.minRequiredPasswordLength = GetConfigurationMinRequiredPasswordLength();
                ret.availableLanguages = GetAvailableLanguages();
                ret.projectCulture = GetProjectCulture();
                ret.screenDelayUnloadMSecs = ScreenDelayUnloadMSecs;

                ret.topScreen = GetConfigurationTopScreen();
                ret.bottomScreen = GetConfigurationBottomScreen();
                ret.leftScreen = GetConfigurationLeftScreen();
                ret.rightScreen = GetConfigurationRightScreen();
                ret.appbartopScreen = GetConfigurationAppBarTopScreen();
                ret.appbarbottomScreen = GetConfigurationAppBarBottomScreen();
                ret.appbarleftScreen = GetConfigurationAppBarLeftScreen();
                ret.appbarrightScreen = GetConfigurationAppBarRightScreen();

                ret.topScreenHeight = GetConfigurationTopScreenHeight();
                ret.bottomScreenHeight = GetConfigurationBottomScreenHeight();
                ret.leftScreenWidth = GetConfigurationLeftScreenWidth();
                ret.rightScreenWidth = GetConfigurationRightScreenWidth();
                ret.restoreLastOpenScreenAndZoom = GetRestoreLastOpenScreenAndZoom();

                ret.mapProvider = MapProvider;
                ret.mapProviderKey = MapProviderKey;
                ret.mapLayers = MapLayers;
                ret.mapZoomFactor = MapZoomFactor;
                ret.mapMaxZoomFactor = MapMaxZoomFactor;
                ret.mapBounds = MapBounds;
                ret.legacyMapView = LegacyMapView;
                ret.mapViewTilesURL = MapViewTilesURL;
                ret.mapViewTilesOptions = MapViewTilesOptions;
                ret.serverInvokeMinFreqOnDrag = ServerInvokeMinFreqOnDrag;
                ret.inDemoMode = session.InDemoMode;
    });

            return ret;
        }

        public async Task<List<string>> GetListScreens()
        {
            return await Task.Run(() => {
                return GetProjectScreensList();
            });
        }

        public async Task<Tuple<string, List<TileInfo>>> GetListTiles()
        {
            return await Task.Run(() => { return new Tuple<string, List<TileInfo>>(tileViewBackground, listTilesFlat); });
        }

        public async Task<List<TileInfo>> GetListMapPins()
        {
            return await Task.Run(() =>
            {
                return listMapPinsFlat;
            });
        }

        public async Task<Rectangle> GetReportSize(String reportUrl)
        {
            return await Task.Run(() =>
            {
                var tuple = MakeAbsolute(reportUrl);
                reportUrl = CompleteWithExtensionReport(tuple.Item1);

                var reportDoc = ReportSettings.Documents.ReportDocument.FromFile(reportUrl, GetProjectDocument());

                return new Rectangle()
                {
                    x = reportDoc.Left,
                    y = reportDoc.Top,
                    width = reportDoc.Width,
                    height = reportDoc.Height
                };
            });
        }

        public async Task<String> RegisterLiveMapPins()
        {
            return await Task.Run(() =>
            {
                var idnew = Guid.NewGuid();
                var id = String.Format("id{0}", idnew.ToString());

                var session = Session.Session.GetSession(Context.ConnectionId);
                if (session.AddLiveMapPins(id, Clients.Caller))
                    return id;
                else
                    return String.Empty;
            });
        }

        public async Task KeepAliveLiveMapPins(String id)
        {
            await Task.Run(() =>
            {
                var session = Session.Session.GetSession(Context.ConnectionId);
                session.KeepAliveLiveMapPins(id);
            });
        }

        public async Task UnregisterLiveMapPins(String id)
        {
            await Task.Run(() =>
            {
                var session = Session.Session.GetSession(Context.ConnectionId);
                session.UnregisterLiveMapPins(id);
            });
        }

        public async Task<Data> GetListStringForCulture(String culture, uint hash)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var bHashOk = false;
            if (hash != 0)
                bHashOk = session.IsLanguageHashValid(culture, hash);

            if (bHashOk)
                return new Data() { id = culture, hashOk = true };

            var data = new Data();
            await Task.Run(() =>
            {
                data.listStrings = PlatformComponents.PlatformComponents.GetListStringForCulture(culture);
                if (data.listStrings != null)
                {
                    var crcstring = String.Empty;
                    foreach (var s in data.listStrings.Values)
                        crcstring += s;
                    uint h = CRC32.CalculateCRC32(crcstring);
                    session.AddLanguageHash(culture, h);
                    if (hash != 0)
                    {
                        bHashOk = session.IsLanguageHashValid(culture, hash);
                        if (bHashOk)
                        {
                            data.listStrings = null;
                            data.hashOk = true;
                        }
                    }

                    data.id = culture;
                    data.hash = h;
                }
            });

            return data;
        }

        public async Task<IDictionary<int, String>> GetReferenceNameList(string screenid)
        {
            IDictionary<int, string> ret = new Dictionary<int, string>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                ret = session.GetReferenceNameList(screenid);
            });

            return ret;
        }

        public async Task<List<string>> GetEntityItemSources(string xmlUri, string xmlItems, int maxTake, string dataProvider, string connection, string select, string where, string groupBy, string sort, string screenId)
        {
            var ret = new List<string>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.GetItemSources(xmlUri, xmlItems, maxTake, dataProvider, connection, select, where, groupBy, sort, token);
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task<Data> LoginUser(string name, string message, string locale)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                throw new Exception(Properties.Resources.DemoModeMessage);
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);

            // await Clients.Caller.SendAsync("receive", Context.UserIdentifier, message);
            int s = 0;
            String role = null;
            int level = 0;
            int mask = 0;
            String signature = null;
            String culture = null;
            int autologoffTimeout = 0;
            int daysleft = 0;
            await Task.Run(() =>
            {
                s = ValidateUser(name, message);

                role = GetUserRole(name);
                level = GetUserAccessLevel(name);
                mask = GetUserAccessMask(name);
                culture = GetUserCultureName(name);
                if (String.IsNullOrEmpty(culture))
                    culture = null;
                else
                    locale = culture;
                signature = GetUserElectronicSignature(name);
                autologoffTimeout = GetConfigurationAutoLogoutSeconds(name);
                daysleft = GetDaysLeftPasswordExpires(name);
            });

            if (s > 0)
            {
                session.CurrentUser = name;
                await Clients.Caller.SendAsync("userLoggedIn", name, Context.UserIdentifier, autologoffTimeout, culture);

                await Task.Run(() =>
                {
                    RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(),
                        new UserIdentity(name, message), new StringCollection() { locale });

                    SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentUser, name);
                    SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentRole, role);
                    SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, level);
                    SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, mask);
                });
            }

            return new Data() { status = s, role = role, level = level, mask = mask, signature = signature, passwordDaysLeft = daysleft };
        }

        public async Task SetCurrentLocale(String locale)
        {
            await Task.Run(() =>
            {
                RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(),
                    null, new StringCollection() { locale });
                StringEditorComponent.SetActiveCulture(GetProjectDocument(), locale);
            });
        }

        public async Task<int> UpdateUser(string user, string oldpassword, string newpassword)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);

            // await Clients.Caller.SendAsync("receive", Context.UserIdentifier, message);
            int ret = 0;
            await Task.Run(() =>
            {
                if (PlatformComponents.PlatformComponents.UpdateUser(user, oldpassword, newpassword))
                    ret = 1;
                else if (PlatformComponents.PlatformComponents.IsOldPassword(user, newpassword))
                    ret = -1;
            });

            return ret;
        }

        public async Task LogoutUser(String locale)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (!session.IsAuthenticated)
                return;
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);

            await Clients.Caller.SendAsync("userLoggedOut", session.CurrentUser, Context.UserIdentifier);
            session.CurrentUser = null;

            await Task.Run(() =>
            {
                RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(), null, new StringCollection() { locale });

                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentUser, String.Empty);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentRole, String.Empty);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, 0);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, 0);
            });
        }

        static readonly String svgControlsFormat = "{0}.controls";
        static readonly String svgCommandsFormat = "{0}.commands";
        static readonly String svgAnimationsFormat = "{0}.animations";
        static readonly String svgEntitiesFormat = "{0}.entities";
        static readonly String svgStyleFormat = "{0}.styles";
        static readonly String svgDataFormat = "{0}.svg";
        static readonly String svgiOSDataFormat = "{0}.ios";

        public async Task<Data> OpenScreen(string screenid, string name, string parameter, string callerScreenId, uint hash = 0, bool biOS = false, bool bIsInFittedParent = false, bool bIsDocked = false)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);

            if (session.InDemoModeCountDown && !bIsDocked)
                throw new Exception(Properties.Resources.DemoModeMessage);
            //var page = Context.GetHttpContext()?.Request.Query["page"].ToString();
            //if (!String.IsNullOrEmpty(page))
            {
                String username = String.Empty;
                var user = Context.User;
                if (session.CurrentUser != null)
                    username = session.CurrentUser;

                {
                    var uri = new Uri(CompleteWithExtensionScreen(PrependScreenTypeLabel(name)), UriKind.RelativeOrAbsolute).GetPathString();
                    var found = (from tileinfo in listTilesFlat where tileinfo.Url == uri select tileinfo).ToList();
                    if (found.Count > 0)
                    {
                        var tileinfo = found[0];
                        name = tileinfo.Url;
                        if (!String.IsNullOrEmpty(tileinfo.UsersVisibility))
                        {
                            var users = tileinfo.UsersVisibility.Split(';');
                            if (!String.IsNullOrEmpty(tileinfo.UsersVisibility) && users.Length > 0 && !users.Contains(username))
                                throw new ArgumentException(Properties.Resources.CannotFindScreen);
                        }
                        if (!String.IsNullOrEmpty(tileinfo.RolesVisibility))
                        {
                            var userrole = GetUserRole(username);

                            var roles = tileinfo.RolesVisibility.Split(';');
                            if (!String.IsNullOrEmpty(tileinfo.RolesVisibility) && roles.Length > 0)
                            {
                                bool bFound = false;
                                foreach (var role in roles)
                                {
                                    if (String.Compare(role, userrole, true) == 0)
                                    {
                                        bFound = true;
                                        break;
                                    }
                                }
                                if (!bFound)
                                    throw new ArgumentException(Properties.Resources.CannotFindScreen);
                            }
                        }
                    }
                }
            }

            try
            {
                var tuple = MakeAbsolute(name, session.GetScreenPath(callerScreenId));
                name = tuple.Item1;

                var bHashOk = false;
                if (hash != 0)
                    bHashOk = session.IsHashValid(name, hash);

                //var idnew = Guid.NewGuid();
                //var screenid = String.Format("id{0}", idnew.ToString());
                //#if DEBUG
                //                System.IO.File.AppendAllText("d:\\webhmilog.txt", String.Format("Opening Screen {0} {1} {2} {3}\n", screenid, name, parameter, hash));
                //#endif
                var nameWithExtension = CompleteWithExtensionScreen(name);
                var screenDataInfo = session.AddScreenData(screenid, nameWithExtension, parameter, Clients.Caller, tuple.Item2, Context.ConnectionId, callerScreenId);
                var windowState = screenDataInfo.WindowState.ToString().ToLower();
                var windowStyle = screenDataInfo.WindowStyle.ToString().ToLower();
                var bFitInWindow = screenDataInfo.FitInWindow;
                var title = screenDataInfo.ScreenTitle;
                bool requireUserLogin = session.RequireUserLogin(screenid);
                long delayUnloadSecs = session.DelayUnloadSecs(screenid);
                bool keepAlwaysInMemory = session.KeepAlwaysInMemory(screenid);
                bool bHideLayoutScreens = session.HideLayoutScreens(screenid);

                if (!bHashOk)
                {
                    String svgControls = String.Empty, svgCommands = String.Empty,
                        svgAnimations = String.Empty, svgData = String.Empty, svgEntities = String.Empty, svgStyle = String.Empty;

                    try
                    {
                        svgControls = await File.ReadAllTextAsync(String.Format(svgControlsFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        svgCommands = await File.ReadAllTextAsync(String.Format(svgCommandsFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        svgAnimations = await File.ReadAllTextAsync(String.Format(svgAnimationsFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        svgEntities = await File.ReadAllTextAsync(String.Format(svgEntitiesFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        svgStyle = await File.ReadAllTextAsync(String.Format(svgStyleFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        if (biOS && (bFitInWindow || bIsInFittedParent))
                        {
                            try
                            {
                                svgData = await File.ReadAllTextAsync(String.Format(svgiOSDataFormat, name));
                            }
                            catch { }
                        }
                        if (String.IsNullOrEmpty(svgData))
                            svgData = await File.ReadAllTextAsync(String.Format(svgDataFormat, name));
                    }
                    catch (Exception ex)
                    {

                    }

                    var crcstring = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        svgControls, svgData, svgCommands, svgAnimations, svgEntities, svgStyle, windowState, windowStyle);
                    var h = CRC32.CalculateCRC32(crcstring);
                    session.AddScreenHash(name, h);
                    if (hash != 0)
                    {
                        bHashOk = session.IsHashValid(name, hash);
                        if (bHashOk)
                            return new Data() { id = screenid, hashOk = true,
                                requireUserLogin = requireUserLogin,
                                delayUnloadSecs = delayUnloadSecs,
                                keepAlwaysInMemory = keepAlwaysInMemory,
                                hideLayoutScreens = bHideLayoutScreens,
                                fullPath = nameWithExtension,
                                title = title,
                                windowState = windowState,
                                windowStyle = windowStyle
                            };
                    }

                    return new Data() { data = svgData.Replace("{0}", screenid),
                        controls = svgControls,
                        animations = svgAnimations,
                        entities = svgEntities,
                        commands = svgCommands,
                        style = svgStyle,
                        id = screenid, hash = h,
                        requireUserLogin = requireUserLogin,
                        delayUnloadSecs = delayUnloadSecs,
                        keepAlwaysInMemory = keepAlwaysInMemory,
                        hideLayoutScreens = bHideLayoutScreens,
                        fullPath = nameWithExtension,
                        title = title,
                        windowState = windowState,
                        windowStyle = windowStyle
                    };
                }
                else
                {
                    return new Data() { id = screenid, hashOk = true,
                        requireUserLogin = requireUserLogin,
                        delayUnloadSecs = delayUnloadSecs,
                        keepAlwaysInMemory = keepAlwaysInMemory,
                        hideLayoutScreens = bHideLayoutScreens,
                        fullPath = nameWithExtension,
                        title = title,
                        windowState = windowState,
                        windowStyle = windowStyle
                    };
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                    session.ShowDemoMode(Context.ConnectionId);
            }

            return new Data();
        }

        public async Task SetValue(int id, string value, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.SetValue(screenId, id, value);
            });
        }

        public async Task ResetStatistics(int id, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.ResetStatistics(screenId, id);
            });
        }

        public async Task ToggleAlarmSound()
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.ToggleAlarmSound();
            });
        }

        #region Scheduler
        public async Task Scheduler_InitServerConnection(String screenId, int idreference, String schedulerId, string connectionString)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    session.Scheduler_InitServerConnection(Clients.Caller, screenId, idreference, schedulerId, connectionString, token);
                }, token);
            }
            catch (TaskCanceledException) { }
        }

        MSScheduledAction_Web GetMSScheduledActionWeb_Holidays(string currentSchedulerFullName, string currentSchedulerNodeId, string currentSchedulerType, List<object> holidays)
        {
            var action = new MSScheduledAction_Web()
            {
                FullName = currentSchedulerFullName,
                ScheduledEventNodeId = currentSchedulerNodeId,
                Type = currentSchedulerType,
            };
            foreach (Dictionary<object, object> item in holidays)
            {
                var dates = ParseClientDateTime(item["StartDate"], item["EndDate"]);
                var startDate = dates.Item1;
                var endDate = dates.Item2;
                action.ExceptionCalendarItems.Add(new CalendarItem_Web()
                {
                    StartDate = startDate.Add(TimeSpan.FromMilliseconds((uint)item["StartDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                    EndDate = endDate.Add(TimeSpan.FromMilliseconds((uint)item["EndDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                    Month = (string)item["Month"],
                    WeekOfMonth = (string)item["WeekOfMonth"],
                    ItemType = (string)item["ItemType"],
                    DayOfWeek = (string)item["DayOfWeek"],
                    DayOfMonth = GetDayOfMonthIndex(item["DayOfMonth"].ToString())
                });
            }
            return action;
        }

        public async Task<bool> SaveCurrentScheduler(String screenId, String schedulerId, int clientTimezoneOffset, string currentSchedulerFullName, string currentSchedulerNodeId, string currentSchedulerType, long? time, long? timeOff, bool execOn, bool execOff, List<object> calendar, List<object> holidays, List<object> week)
        {
            bool ret = false;

            if (time != null)
                time *= 10000000;
            if (timeOff != null)
                timeOff *= 10000000;
            var action = GetMSScheduledActionWeb(currentSchedulerFullName, currentSchedulerNodeId, currentSchedulerType, time, timeOff, execOn, execOff, calendar, holidays, week);

            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return false;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                ret = session.SaveCurrentScheduler(screenId, schedulerId, action);
            });
            return ret;
        }

        public async Task<MSScheduledAction_Web> AddMissingYearHolidays(string screenId, String schedulerId, int clientTimezoneOffset, string currentSchedulerFullName, string currentSchedulerNodeId, string currentSchedulerType, string ISORegion, List<object> clientHolidays)
        {
            MSScheduledAction_Web ret = null;
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return ret;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);

            var holidays = GetMSScheduledActionWeb_Holidays(currentSchedulerFullName, currentSchedulerNodeId, currentSchedulerType, clientHolidays);

            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.AddMissingYearHolidays(screenId, schedulerId, ISORegion, holidays, token);
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task<MSScheduledAction_Web> UpdateScheduler(String screenId, String schedulerId, string scheduler)
        {
            MSScheduledAction_Web ret = null;

            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    var retms = session.UpdateScheduler(screenId, schedulerId, scheduler, token);
                    ret = new MSScheduledAction_Web(retms);
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task<List<SimpleScheduledEvent_Web>> InitSchedulerOnRuntime(String screenId, String schedulerId, string schedulerName)
        {
            List<SimpleScheduledEvent_Web> ret = null;
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.InitSchedulerOnRuntime(screenId, schedulerId, schedulerName, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task SchedulerDispose(String screenId, String schedulerId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.SchedulerDispose(screenId, schedulerId);
            });
        }
        #endregion

        public async Task<List<ConditionData>> GetConditionStateList(int id, string screenId, bool bSortByTimeDescending = false, bool bNeedsRefresh = false)
        {
            List<ConditionData> ret = null;
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return ret;
            var token = session.GetCancellationToken(screenId);
            try
            {
                await Task.Run(() =>
                {
                    ret = session.GetConditionStateList(screenId, id, token, bSortByTimeDescending, bNeedsRefresh);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task AckReset(int id, string screenId, string[] list, bool isReset)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.AckReset(screenId, id, list, isReset);
            });
        }

        public async Task AckResetAll(bool isReset)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.AckResetAll(isReset);
            });
        }

        public async Task<string> InitializeRecipeDocument(string recipeName, int readWriteTimeOut)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            string recipeNames = null;
            await Task.Run(() =>
            {
                recipeNames = session.InitializeRecipeDocument(recipeName, readWriteTimeOut);
            });
            return recipeNames;
        }

        public async Task<RecipeItemsData> FilterSingleRecipeData(string recipePath, string selectedSubrecipe, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            RecipeItemsData RData = null;
            try {
                await Task.Run(() =>
                {
                    RData = session.FilterSingleRecipeData(recipePath, selectedSubrecipe, token);
                }, token);
            }
            catch (TaskCanceledException) { }
            return RData;
        }

        public async Task<List<RecipeItem>> GetFlatDataValues(string recipePath, string screenId)
        {
            List<RecipeItem> ret = new List<RecipeItem>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    var r = session.GetFlatDataValues(recipePath, token);
                    foreach (var dv in r) {
                        token.ThrowIfCancellationRequested();
                        var defValue = DataReader.Extensions.TypeExtensions.ChangeType(dv.DefaultValue, dv.DataType.ToNetType(), force: true, dv.ArrayDimension);
                        ret.Add(new RecipeItem()
                        {
                            DataType = dv.DataType,
                            DecimalDigits = dv.DecimalDigits,
                            Name = dv.Name,
                            GroupName = dv.UFGroupAss != null ? dv.UFGroupAss.Name : String.Empty,
                            Description = dv.Description,
                            DefaultValue = defValue?.ToString()
                        });
                    }
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task<List<string>> RecipeDBLoadNames(int id, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            List<string> ret = null;
            await Task.Run(() =>
            {
                ret = session.RecipeDBLoadNames(screenId, id);
            });
            return ret;
        }

        public async Task RecipeDBSave(int id, string screenId, string recipePath, object pendingChanges, string recipeGuid, string newRecipePrefix, bool bDeleting)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.RecipeDBSave(screenId, id, recipePath, pendingChanges, recipeGuid, newRecipePrefix, bDeleting);
            });
        }

        public async Task RecipeDeviceSave(int id, string screenId, string recipePath, object pendingChanges, string recipeGuid, string newRecipePrefix)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.RecipeDeviceSave(screenId, id, recipePath, pendingChanges, recipeGuid, newRecipePrefix);
            });
        }

        public async Task<RecipeItemsData> RecipeDeviceLoad(int id, string screenId, string recipePath, string selectedRecipe, string recipeGuid)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return null;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            RecipeItemsData ret = null;
            try {
                await Task.Run(() =>
                {
                    ret = session.RecipeDeviceLoad(screenId, id, recipePath, selectedRecipe, recipeGuid, token);
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task RecipeCommandExecute(string recipeStringPath, int commandType, int syncTimeout, bool bSynchronous)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.RecipeCommandExecute(recipeStringPath, commandType, syncTimeout, bSynchronous);
            });
        }

        public async Task RecipeDispose(string recipePath)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.RecipeDispose(recipePath);
            });
        }

        public async Task KeepAliveScreen(string id)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);

            await Task.Run(() =>
            {
                session.UpdateKeepAliveScreen(id);
            });
        }

        public async Task CloseScreen(string id)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            session.CancelPendingExecution(id);
            if (session.InDemoModeCountDown)
                return;
            await Task.Run(() =>
            {
                session.RemoveScreenData(id);
            });
        }

        public async Task LoadMjpegStream(string screenId, string sourceId, string uri, string user, string password, int closeTimeout)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    session.LoadMjpegStream(sourceId, new Uri(uri), user, password, closeTimeout);
                }, token);
            }
            catch (TaskCanceledException) { }
        }
        public async Task<Tuple<IPCameraFrame, string>> GetLastFrameData(string screenId, string sourceId, string lastFrameHash)
        {
            Tuple<IPCameraFrame, string> frame = null;
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    frame = session.GetLastFrameData(sourceId, lastFrameHash);
                }, token);
            }
            catch (TaskCanceledException) { }
            return frame;
        }
        public async Task DisposeMjpegStream(string screenId, string sourceId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    session.DisposeMjpegStream(sourceId);
                }, token);
            }
            catch (TaskCanceledException) { }
        }

        public async Task<DetectionProcessStatus> SendImage(string dataURL, int index, string pageId)
        {
            var ctxHpp = Context.GetHttpContext();
            var connectionId = Context.ConnectionId;
            var caller = Clients.Caller;

#if !DEBUG
            var ar = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxyhpUsHtdpLhTXw+J7nyWtg=="/* ART */);
            if (!ar)
            {
                return new DetectionProcessStatus() { lastError = Properties.Resources.NoARLicense };
            }
#endif
            DetectionProcessStatus ret = null;
            await Task.Run(() =>
            {
                byte[] binData;
                if (ctxHpp.Request.Headers.ContainsKey("Hololens"))
                {
                    binData = Convert.FromBase64String(dataURL);
                }
                else
                {
                    var base64Data = Regex.Match(dataURL, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    binData = Convert.FromBase64String(base64Data);
                }

                ret = ImageDetection.DetectionProcessor.ProcessImage(binData, connectionId, caller, index, pageId);
            });

            return ret;
        }

        public async Task<WPFPenHelpers.AggregatedValues> LoadHistoricalData(string screenId, string connString, int commandTimeout, long? startTimestamp, long? endTimestamp, bool usesourcetimestamp, int maxRecord, bool aggregate, int maxaggregationfactor, int maxDeadLockRetry, bool localize, string viewTimeFrame, bool minAggregation, bool maxAggregation, bool avgAggregation, string tagName, string columnTagName, string serieTitle, string nodeID, string historicalName, bool bDlrSource, string conditionalString)
        {
            WPFPenHelpers.AggregatedValues ret = new WPFPenHelpers.AggregatedValues();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var parametersMap = session.GetParametersMap(screenId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = PlatformComponents.PlatformComponents.LoadHistoricalData(connString, commandTimeout, startTimestamp, endTimestamp, usesourcetimestamp, maxRecord, aggregate, maxaggregationfactor, maxDeadLockRetry, localize, System.Xml.XmlConvert.ToTimeSpan(viewTimeFrame), minAggregation, maxAggregation, avgAggregation, tagName, columnTagName, parametersMap, serieTitle, nodeID, historicalName, bDlrSource, session.GetSessionSettings(screenId), conditionalString, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<WPFPenHelpers.AggregatedValues> LoadSparklineHistoricalData(string screenId, string connection, string dataprovider, int commandTimeout, double invalidPointsValue, string dataColumn, string groupBy, int maxTake, string select, string sort, string tableName, string timeColumn, string where, string connectionstring)
        {
            WPFPenHelpers.AggregatedValues ret = new WPFPenHelpers.AggregatedValues();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = PlatformComponents.PlatformComponents.LoadSparklineHistoricalData(screenId, connection, dataprovider, commandTimeout, invalidPointsValue,
                                dataColumn, groupBy, maxTake, select, sort, tableName, timeColumn, where, connectionstring, session.GetSessionSettings(screenId), token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<ChartData> LoadChartHistoricalData(string screenId, int commandTimeout, int maxRecord, string nodeID, string controlID, string historicalName)
        {
            ChartData ret = new ChartData();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.LoadChartHistoricalData(commandTimeout, maxRecord, nodeID, session.GetSessionSettings(screenId), controlID, historicalName, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<XYChartData> LoadChartXYHistoricalData(string screenId, int commandTimeout, int maxRecord, string XNodeID, string YNodeID, string controlID, string XHistoricalName, string YHistoricalName)
        {
            XYChartData ret = new XYChartData();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.LoadChartXYHistoricalData(commandTimeout, maxRecord, XNodeID, YNodeID, session.GetSessionSettings(screenId), controlID, XHistoricalName, YHistoricalName, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<StatesChartData> LoadStatesChartHistoricalData(string screenId, string connString, int commandTimeout, int maxRecord, string viewTimeFrame, string recordEvery, string nodeID, string historicalName, bool bDlrSource, string coluName, string controlID, int clientTimezoneOffset, long? startTimestamp, long? endTimestamp, string rangeType)
        {
            StatesChartData ret = new StatesChartData();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var parametersMap = session.GetParametersMap(screenId);
            if (parametersMap.Count > 0)
            {
                var tagData = UpdateTagFromMap(coluName, parametersMap);
                if (tagData != null)
                {
                    coluName = tagData.Item1;
                    nodeID = tagData.Item2;
                }
            }
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.LoadStatesChartHistoricalData(connString, commandTimeout, maxRecord, System.Xml.XmlConvert.ToTimeSpan(viewTimeFrame), System.Xml.XmlConvert.ToTimeSpan(recordEvery), nodeID, historicalName, bDlrSource, session.GetSessionSettings(screenId), coluName, controlID, clientTimezoneOffset, startTimestamp, endTimestamp, (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), rangeType), token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<List<string>> InitDataloggerValues(string screenId)
        {
            List<string> ret = new List<string>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                ret = PlatformComponents.PlatformComponents.InitDataloggerValues(session.GetSessionSettings(screenId));
            });

            return ret;
        }

        public async Task<List<string>> InitHistoricalValues(string screenId, bool showAuditTrace)
        {
            List<string> ret = new List<string>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                ret = PlatformComponents.PlatformComponents.InitHistoricalValues(showAuditTrace, session.GetSessionSettings(screenId));
            });

            return ret;
        }

        public async Task<string> LoadDataloggerViewerData(string screenId, string connectionString, List<string> historicalList, string selectedHistorian, int maxRows, int commandTimeout, string rangeType, long? startTimestamp, long? endTimestamp)
        {
            string ret = String.Empty;
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = PlatformComponents.PlatformComponents.LoadDataloggerViewerData(connectionString, historicalList, selectedHistorian, maxRows, commandTimeout, (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), rangeType), startTimestamp, endTimestamp, session.GetSessionSettings(screenId), token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<string> LoadDataGridData(string screenId, string thisGridID, string connectionString, string dataProvider, string connection, string select, string where, string groupBy, string sort, string tableName, uint maxTransactionsBeforeCommit, bool allowPrimaryKeyChanging)
        {
            string ret = String.Empty;
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = session.LoadDataGridData(thisGridID, connectionString, session.GetSessionSettings(screenId), dataProvider, connection, select, where, groupBy, sort, tableName, maxTransactionsBeforeCommit, allowPrimaryKeyChanging, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task GridControlDBSave(string screenId, string thisGridID, object pendingChanges)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.GridControlDBSave(thisGridID, session.GetSessionSettings(screenId), pendingChanges);
            });
        }

        public async Task<ServerAuditDataItemModel> LoadHistoricalViewerData(string connectionString, string screenId, bool showAuditTrace, List<string> historicalList, string selectedHistorian, int maxRows, int commandTimeout, string rangeType, long? startTimestamp, long? endTimestamp)
        {
            ServerAuditDataItemModel ret = new ServerAuditDataItemModel();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    ret = PlatformComponents.PlatformComponents.LoadHistoricalViewerData(connectionString, showAuditTrace, historicalList, selectedHistorian, maxRows, commandTimeout, (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), rangeType), startTimestamp, endTimestamp, session.GetSessionSettings(screenId), token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<string> SavePrintSendReport(string jsonoptions)
        {
            string ret = null;
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return null;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                ret = PlatformComponents.PlatformComponents.SavePrintSendReport(jsonoptions);
            });

            return ret;
        }

        public async Task<List<string>> InitLogViewer(string screenId)
        {
            var ret = new List<string>();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    var logsDirInfo = new DirectoryInfo(String.Format("{0}{1}{2}", Path.GetDirectoryName(GetProjectDocument().FilePath), Path.DirectorySeparatorChar, logsDir));
                    ret = logsDirInfo.EnumerateFiles().Where(f => logSupportedExtensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase)).Select(fi => fi.Name).ToList();
                }, token);
            }
            catch (TaskCanceledException) { }
            if (ret.Count > 0)
                ret.Sort();
            return ret;
        }

        public async Task NewLogFile(string screenId, string logFileName)
        {
            var ret = new LogItems();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try { 
                await Task.Run(() =>
                {
                    var projectFolder = Path.GetDirectoryName(GetProjectDocument().FilePath);
                    var logFilePath = String.Format("{0}{1}{3}{1}{2}", projectFolder, Path.DirectorySeparatorChar, logFileName, logsDir);
                    //if (String.IsNullOrEmpty(logFileName) || !File.Exists(logFilePath) || new FileInfo(logFilePath).Length == 0)
                    //    return;
                    var folder = Path.GetDirectoryName(logFilePath);
                    var fileName = Path.GetFileNameWithoutExtension(logFilePath);
                    var extension = Path.GetExtension(logFilePath);
                    File.Move(logFilePath, Path.Combine(folder, string.Format("{0}{2}{1:yyyyMMddHHmmss}", fileName, DateTime.Now, extension)));
                    File.WriteAllText(logFilePath, String.Empty);
                });
            }
            catch (TaskCanceledException) { }
        }

        public async Task<LogItems> GetLogFile(string screenId, string logFileName)
        {
            //LogItems ret = new LogItems();
            var ret = new LogItems();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try {
                await Task.Run(() =>
                {
                    var projectFolder = Path.GetDirectoryName(GetProjectDocument().FilePath);
                    var logFilePath = String.Format("{0}{1}{3}{1}{2}", projectFolder, Path.DirectorySeparatorChar, logFileName, logsDir);
                    if (File.Exists(logFilePath))
                    {
                        if (Path.GetExtension(logFilePath).ToLower() == ".xml")
                            LoadXMLLogFile(ret, logFilePath);
                        else
                        {
                            var currentItemIndex = 1;
                            foreach (string line in File.ReadLines(logFilePath))
                            {
                                var logItem = new LogItem()
                                {
                                    Item = currentItemIndex,
                                    Message = line
                                };
                                ret.Items.Add(logItem);
                                currentItemIndex++;
                            }
                            ret.SingleLineStructure = true;
                        }
                    }
                }, token);
            }
            catch (TaskCanceledException) { }
            if (ret.Loggers.Count > 0)
                ret.Loggers.Sort();
            return ret;
        }

        public async Task<ServerAuditLogItem> GetHistoricalEventList(string connString, string screenId, int filterEventType,
                                    long? startTimestamp, long? endTimestamp, string dateSpan, int defaultDateSpan, int maxRows)
        {
            ServerAuditLogItem ret = new ServerAuditLogItem();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            WPFUtilities.DateSpan? rangeType = null;
            if (dateSpan != null)
                rangeType = (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), dateSpan);
            try {
                await Task.Run(() =>
                {
                    ret = LoadHistoricalEventData(connString, session.GetSessionSettings(screenId), filterEventType,
                                        startTimestamp, endTimestamp, rangeType, (WPFUtilities.DateSpan)defaultDateSpan, maxRows, session.ClientTimezoneOffset, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }
        
    }
}
