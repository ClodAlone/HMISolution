using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using ExternalAuthentication.Utilities;
using IdentityModel.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UFUAModel.Extensions;
using WebNExTHMI.ImageDetection;
using WebNExTHMI.PlatformComponents;
using WebNExTHMI.Services;
using WPFUtilities.HistoricalHelpers;
using static Utilities.UriExtension;
using static WebNExTHMI.PlatformComponents.PlatformComponents;
using static WebNExTHMI.PlatformComponents.SchedulerData;

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
        public String username;
        public String role;
        public int level;
        public int mask;
        public int passwordDaysLeft;
        public String signature;
        public bool requireUserLogin;
        public long delayUnloadSecs;
        public bool keepAlwaysInMemory;
        public bool keepAlwaysTagsAlive;
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
        public float ARQRCodeScaleFactor;
        public string ActiveSessionCountVariableName;
        public List<string> preloadedToolboxComponents;
        public bool isExternalAuthActive;

        public bool enableUserManager;
        public int autoLogoutSeconds;
        public int minRequiredPasswordLength;
        public IEnumerable<String> availableLanguages;
        public String projectCulture;
        public bool forceStartupCultureName;
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
        #region External Authentication
        private static ExternalIdPUserSettings _externalIdPUserSettings;
        private static string _codeVerifier;
        private static string _stateGuid;
        private static Random _random = new Random();
        #endregion

        static CoreHub()
        {
            _externalIdPUserSettings = GetExternalAuthenticationConfiguration();
            EventEditorComponent.FireUIEvent += EventEditorComponent_FireUIEvent;
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                Debug.WriteLine("New connection : " + Context.ConnectionId);
                var sessions = Session.Session.AddSession(Context.ConnectionId, Clients.Caller);

                SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.NumActiveWebClientUsers, sessions);
            }
            catch (Exception e)
            {
                Session.Session.RemoveSession(Context.ConnectionId);
                throw;
            }

            return base.OnConnectedAsync();
        }

        static private void EventEditorComponent_FireUIEvent(object sender, UFEventEditor.ComponentService.EventEventArgs e)
        {
            Session.Session.SendAllExecuteUICommand(e.JsonCommand);
            e.bExecuted = true;
        }

        static private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }
            return new string(result);
        }

        static private string GenerateCodeChallenge(string code_verifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(code_verifier));
            return WebEncoders.Base64UrlEncode(hash);
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
                ret.ARScanQRCode = ARScanQRCode;
                ret.ARScanQRToleranceMS = ARScanQRToleranceMS;
                ret.ARScanQRCodeOnly = ARScanQRCodeOnly;
                ret.ARQRCodeScaleFactor = ARQRCodeScaleFactor;
                ret.ActiveSessionCountVariableName = ActiveSessionCountVariableName;
                
                var externalIdpUserSettings = GetExternalAuthenticationConfiguration();
                ret.isExternalAuthActive = externalIdpUserSettings.ExternalIdentityProviderActive;

                ret.startType = GetConfigurationStartType();
                ret.startupScreen = GetConfigurationStartupScreen();
                ret.autoLogoutSeconds = GetConfigurationAutoLogoutSeconds();
                ret.enableUserManager = session.EnableUserManager;
                ret.minRequiredPasswordLength = GetConfigurationMinRequiredPasswordLength();
                ret.availableLanguages = GetAvailableLanguages();
                ret.projectCulture = GetProjectCulture();
                ret.forceStartupCultureName = GetForceStartupCultureName();
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

        public async Task<List<Tuple<string, string>>> GetListScreens()
        {
            return await Task.Run(() =>
            {
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
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
                if (!state)
                    throw new Exception(Properties.Resources.NoGeoLicense);
#endif
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
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
                if (!state)
                    throw new Exception(Properties.Resources.NoGeoLicense);
#endif

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
            try
            {
                await Task.Run(() =>
                {
                    ret = session.GetItemSources(xmlUri, xmlItems, maxTake, dataProvider, connection, select, where, groupBy, sort, token);
                }, token);
            }
            catch (TaskCanceledException) { }
            return ret;
        }

        public async Task<string> CreateUrlForExternalIdPLogin()
        {
            var loginUrl = string.Empty;

            await Task.Run(() =>
            {
                _codeVerifier = GenerateRandomString(43);
                _stateGuid = Guid.NewGuid().ToString();
                var codeChallengeForUrl = GenerateCodeChallenge(_codeVerifier);
                var requestUrl = new RequestUrl(_externalIdPUserSettings.AuthorizeEndpoint);
                var requestState = _stateGuid + Context.ConnectionId;

                loginUrl = requestUrl.CreateAuthorizeUrl(
                clientId: _externalIdPUserSettings.ClientId,
                responseType: "code",
                redirectUri: _externalIdPUserSettings.ReturnUrlAfterLogin,
                responseMode: "form_post",
                nonce: _externalIdPUserSettings.Nonce,
                scope: _externalIdPUserSettings.Scope,
                state: requestState,
                codeChallenge: codeChallengeForUrl,
                codeChallengeMethod: "S256");
                
                loginUrl = loginUrl.Replace("%2B", "+");
            });

            return loginUrl;
        }

        public async Task<Data> LoginUser(string name, string message, string locale)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                throw new Exception(Properties.Resources.DemoModeMessage);
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);

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
                await SendUserLoggedInNotifications(
                    session,
                    name,
                    role,
                    level,
                    mask,
                    autologoffTimeout,
                    culture,
                    locale,
                    new UserIdentity(name, message));
            }

            return new Data() { status = s, role = role, level = level, mask = mask, signature = signature, passwordDaysLeft = daysleft };
        }

        public async Task OnExternalIdpUserLoggedIn(string code, string responseState)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                throw new Exception(Properties.Resources.DemoModeMessage);
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);

            string username = null;
            string role = null;
            int level = 0;
            int mask = 0;
            UserIdentity userIdentity = null;
            string culture = null;
            int autologoffTimeout = 0;

            await Task.Run(async () =>
            {
                var exchangeAuthCodeWithTokensService = new ExchangeAuthCodeWithTokensService(new System.Net.Http.HttpClient());
                var validateResponseStateService = new ValidateResponseStateService();
                var originalState = _stateGuid + Context.ConnectionId;
                var stateValidation = validateResponseStateService.Execute(originalState, responseState);

                if (stateValidation)
                {
                    var idpResponse = await exchangeAuthCodeWithTokensService.Execute(
                    _externalIdPUserSettings.TokenEndpoint,
                    _externalIdPUserSettings.ClientId,
                    _externalIdPUserSettings.ReturnUrlAfterLogin,
                    code,
                    _codeVerifier,
                    _externalIdPUserSettings.ClientSecret);

                    var idToken = string.Empty;
                    var accessToken = string.Empty;
                    var responseValidation = await ValidateIdpResponse(idpResponse);

                    if (responseValidation.ValidResponse && string.IsNullOrEmpty(idpResponse.id_token) == false && string.IsNullOrEmpty(idpResponse.access_token) == false)
                    {
                        idToken = idpResponse.id_token;
                        accessToken = idpResponse.access_token;

                        var setLoginResultFromTokensService =
                            new SetLoginResultFromTokensService(new CreateUserTokenFromJwtService(), new GetRolesListService());

                        var externalLoginResult = setLoginResultFromTokensService.Execute(idToken, accessToken);
                        username = externalLoginResult.Username;
                        role = GetExternalAuthenticationUserRole(externalLoginResult.Roles?.FirstOrDefault()); 
                        userIdentity = externalLoginResult.UserIdentity;
                        level = GetRoleAccessLevel(role);
                        mask = GetRoleAccessMask(role);
                        culture = GetRoleCultureName(role);
                        autologoffTimeout = GetRoleAutoLogoutSeconds(role);

                        if (culture == null)
                        {
                            culture = "en-US";
                        }
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("externalLoginFailed", responseValidation.ErrorMessage);
                        throw new Exception(responseValidation.ErrorMessage);
                    }
                } 
                else
                {
                    await Clients.Caller.SendAsync("externalLoginFailed", ExternalAuthentication.Properties.Resources.AuthenticationFailed);
                    throw new Exception(ExternalAuthentication.Properties.Resources.AuthenticationFailed);
                }
            });

            await SendUserLoggedInNotifications(
                session,
                username,
                role,
                level,
                mask,
                autologoffTimeout,
                culture,
                culture,
                userIdentity);

            var externalLoginData = new Data() { status = 1, username = username, role = role, level = level, mask = mask, signature = string.Empty };
            await Clients.Caller.SendAsync("externalLoginSuccessful", externalLoginData);
        }

        private async Task SendUserLoggedInNotifications(
            Session.Session session,
            string userName,
            string userRole,
            int userLevel,
            int userMask,
            int autologoffTimeout,
            string culture,
            string locale,
            UserIdentity userIdentity)
        {
            session.CurrentUser = userName;
            session.CurrentUserAccessLevel = userLevel;
            session.CurrentUserReadAccessMask = userMask;

            await Clients.Caller.SendAsync("userLoggedIn", userName, Context.UserIdentifier, autologoffTimeout, culture);

            await Task.Run(() =>
            {
                RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(),
                userIdentity, new StringCollection() { locale });
                RealTimeConnectionManagerViewModel.SetUserIdentity(session.GetAlarmCommandsSessionName(),
                userIdentity, new StringCollection() { locale });

                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentUser, userName);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentRole, userRole);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, userLevel);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, userMask);
            });
        }

        private async Task<IdpResponseValidatonOnWebHMI> ValidateIdpResponse(IdPTokenResponse tokenResponse)
        {
            var username = string.Empty;
            var errorMessage = string.Empty;

            var tokenValidationService = new TokenValidationService(
                new ValidateTokenKeysService(new GetAppPublicKeysService(new System.Net.Http.HttpClient())),
                new JwtValidationService());

            var tokenValidation = await tokenValidationService.Execute(tokenResponse.id_token, _externalIdPUserSettings);
            
            if (tokenValidation)
            {
                var jwtIdToken = new JsonWebToken(tokenResponse.id_token);

                foreach (var claim in jwtIdToken.Claims)
                {
                    switch (claim.Type.ToLower())
                    {
                        case "email":
                            username = claim.Value;
                            break;
                    }
                }

                if (string.IsNullOrEmpty(username))
                {
                    tokenValidation = false;
                    errorMessage = ExternalAuthentication.Properties.Resources.IdPUsernameNotSet;
                }
            } 
            else
            {
                errorMessage = ExternalAuthentication.Properties.Resources.TokenValidationFailed;
            }
     
            return IdpResponseValidatonOnWebHMI.Create(tokenValidation, errorMessage);
        }

        public async Task SetCurrentLocale(String locale)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(),
                    null, new StringCollection() { locale });
                RealTimeConnectionManagerViewModel.SetUserIdentity(session.GetAlarmCommandsSessionName(),
                    null, new StringCollection() { locale });

                StringEditorComponent.SetActiveCulture(GetProjectDocument(), locale);
                var culture = StringEditorComponent.GetActiveCulture(GetProjectDocument());
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
            if (_externalIdPUserSettings.ExternalIdentityProviderActive)
            {
                await ExecuteExternalIdpLogout();
                await ExecuteStandardLogout(locale);
            }
            else
            {
                await ExecuteStandardLogout(locale);
            }
        }

        private async Task ExecuteStandardLogout(String locale)
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
            session.CurrentUserAccessLevel = 0;
            session.CurrentUserReadAccessMask = 0;

            await Task.Run(() =>
            {
                RealTimeConnectionManagerViewModel.SetUserIdentity(GetProjectTitle(), null, new StringCollection() { locale });
                RealTimeConnectionManagerViewModel.SetUserIdentity(session.GetAlarmCommandsSessionName(), null, new StringCollection() { locale });

                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentUser, String.Empty);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentRole, String.Empty);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessLevel, 0);
                SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.CurrentAccessMask, 0);
            });
        }

        private async Task ExecuteExternalIdpLogout()
        {
            var requestUrl = new RequestUrl(_externalIdPUserSettings.LogoutEndpoint);

            var logoutUrl = OidcExtensionMethods.CreateSignOutUrl(
               clientId: _externalIdPUserSettings.LogoutEndpoint,
               requestUrl,
               postLogoutRedirectUri: _externalIdPUserSettings.ReturnUrlAfterLogout);

            await Clients.Caller.SendAsync("navigateToExternalLogout", logoutUrl);
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

            bool bRet = true;
            string screenTitle = String.Empty;
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
                screenTitle = screenDataInfo.ScreenTitle;
                bool requireUserLogin = session.RequireUserLogin(screenid);
                long delayUnloadSecs = session.DelayUnloadSecs(screenid);
                bool keepAlwaysInMemory = session.KeepAlwaysInMemory(screenid);
                bool keepAlwaysTagsAlive = session.KeepAlwaysTagsAlive(screenid);
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
                            return new Data()
                            {
                                id = screenid,
                                hashOk = true,
                                requireUserLogin = requireUserLogin,
                                delayUnloadSecs = delayUnloadSecs,
                                keepAlwaysInMemory = keepAlwaysInMemory,
                                keepAlwaysTagsAlive = keepAlwaysTagsAlive,
                                hideLayoutScreens = bHideLayoutScreens,
                                fullPath = nameWithExtension,
                                title = screenTitle,
                                windowState = windowState,
                                windowStyle = windowStyle
                            };
                    }

                    return new Data()
                    {
                        data = svgData.Replace("{0}", screenid),
                        controls = svgControls,
                        animations = svgAnimations,
                        entities = svgEntities,
                        commands = svgCommands,
                        style = svgStyle,
                        id = screenid,
                        hash = h,
                        requireUserLogin = requireUserLogin,
                        delayUnloadSecs = delayUnloadSecs,
                        keepAlwaysInMemory = keepAlwaysInMemory,
                        keepAlwaysTagsAlive = keepAlwaysTagsAlive,
                        hideLayoutScreens = bHideLayoutScreens,
                        fullPath = nameWithExtension,
                        title = screenTitle,
                        windowState = windowState,
                        windowStyle = windowStyle
                    };
                }
                else
                {
                    return new Data()
                    {
                        id = screenid,
                        hashOk = true,
                        requireUserLogin = requireUserLogin,
                        delayUnloadSecs = delayUnloadSecs,
                        keepAlwaysInMemory = keepAlwaysInMemory,
                        keepAlwaysTagsAlive = keepAlwaysTagsAlive,
                        hideLayoutScreens = bHideLayoutScreens,
                        fullPath = nameWithExtension,
                        title = screenTitle,
                        windowState = windowState,
                        windowStyle = windowStyle
                    };
                }
            }
            catch (Exception ex)
            {
                bRet = false;
            }
            finally
            {
                if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                    session.ShowDemoMode(Context.ConnectionId);
                if (bRet)
                {
                    SysVariables.SysVariables.GetSysVariables(session.GetProjectDocument()).UpdateSysVariable(SysVariables.SysNames.ActiveScreen, screenTitle);
                }
            }

            return new Data();
        }

        public async Task SetValue(int id, string value, string screenId, bool bForceSync)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.SetValue(screenId, id, value, bForceSync);
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
            try
            {
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
            try
            {
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
            try
            {
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
            try
            {
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

        public async Task SubscribeChildrenAlarmServer(string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;

            await Task.Run(() =>
            {
                session.SubscribeChildrenAlarmServer(screenId);
            });
        }

        public async Task UnsubscribeChildrenAlarmServer(string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;

            await Task.Run(() =>
            {
                session.UnsubscribeChildrenAlarmServer(screenId);
            });
        }

        public async Task<List<ConditionData>> GetConditionStateList(int id, string screenId, bool bSortByTimeDescending = false, bool bNeedsRefresh = false, object filters = null, bool bConnectChildAlarms = false)
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
                    ret = session.GetConditionStateList(screenId, id, token, bSortByTimeDescending, bNeedsRefresh, filters as Dictionary<object, object>, bConnectChildAlarms);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task AckReset(int id, string screenId, string[] list, bool isReset, bool bConnectChildAlarms = false)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.AckReset(screenId, id, list, isReset, bConnectChildAlarms);
            });
        }

        public async Task AckResetAll(bool isReset, bool isCommand, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            if (session.InDemoModeCountDown)
                return;
            if (session.InDemoMode && ++session.clickCounterInDemoMode >= Session.Session.demoModeClickCounter)
                session.ShowDemoMode(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.AckResetAll(isReset, isCommand, screenId);
            });
        }

        public async Task SubscribeCanAckResetAll(string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            await Task.Run(() =>
            {
                session.SubscribeCanAckResetAll(screenId);
            });
        }

        public async Task<string> InitializeRecipeDocument(string recipeName, int readWriteTimeOut, bool bInit)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            string recipeNames = null;
            await Task.Run(() =>
            {
                recipeNames = session.InitializeRecipeDocument(recipeName, readWriteTimeOut, bInit);
            });
            return recipeNames;
        }

        public async Task<RecipeItemsData> FilterSingleRecipeData(string recipePath, string selectedSubrecipe, string screenId)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            RecipeItemsData RData = null;
            try
            {
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
            try
            {
                await Task.Run(() =>
                {
                    var r = session.GetFlatDataValues(recipePath, token);
                    foreach (var dv in r)
                    {
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
            try
            {
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

        public async Task SetScreenInIdle(string id, bool newValue)
        {
            var session = Session.Session.GetSession(Context.ConnectionId);

            await Task.Run(() =>
            {
                session.SetInIdle(id, newValue);
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
            try
            {
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
            try
            {
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
            try
            {
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

        public async Task<AggregatedValues> LoadHistoricalData(string screenId, string connString, int commandTimeout, long? startTimestamp, long? endTimestamp, bool usesourcetimestamp, int maxRecord, bool aggregate, int maxaggregationfactor, int maxDeadLockRetry, bool localize, string viewTimeFrame, bool minAggregation, bool maxAggregation, bool avgAggregation, string tagName, string columnTagName, string serieTitle, int SVGReferenceId, string nodeID, string historicalName, bool bDlrSource, string conditionalString, bool bNeedsMedian, bool bNeedsVariance, bool bNeedsStdDev)
        {
            AggregatedValues ret = new AggregatedValues();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var parametersMap = session.GetParametersMap(screenId);
            var token = session.GetCancellationToken(screenId);
            try
            {
                await Task.Run(() =>
                {
                    ret = session.LoadHistoricalData(screenId, connString, commandTimeout, startTimestamp, endTimestamp, usesourcetimestamp, maxRecord, aggregate, maxaggregationfactor, maxDeadLockRetry, localize, viewTimeFrame, minAggregation, maxAggregation, avgAggregation, tagName, columnTagName, parametersMap, serieTitle, SVGReferenceId, nodeID, historicalName, bDlrSource, conditionalString, bNeedsMedian, bNeedsVariance, bNeedsStdDev, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<AggregatedValues> LoadSparklineHistoricalData(string screenId, string connection, string dataprovider, int commandTimeout, double invalidPointsValue, string dataColumn, string groupBy, int maxTake, string select, string sort, string tableName, string timeColumn, string where, string connectionstring)
        {
            AggregatedValues ret = new AggregatedValues();
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try
            {
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
            try
            {
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
            try
            {
                await Task.Run(() =>
                {
                    ret = session.LoadChartXYHistoricalData(commandTimeout, maxRecord, XNodeID, YNodeID, session.GetSessionSettings(screenId), controlID, XHistoricalName, YHistoricalName, token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<StatesChartData> LoadStatesChartHistoricalData(string screenId, string connString, int commandTimeout, int maxRecord, string viewTimeFrame, string recordEvery, string nodeID, string historicalName, bool bDlrSource, string coluName, int SVGReferenceId, string controlID, int clientTimezoneOffset, long? startTimestamp, long? endTimestamp, string rangeType)
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
            try
            {
                await Task.Run(() =>
                {
                    ret = session.LoadStatesChartHistoricalData(screenId, connString, commandTimeout, maxRecord, System.Xml.XmlConvert.ToTimeSpan(viewTimeFrame), System.Xml.XmlConvert.ToTimeSpan(recordEvery), nodeID, historicalName, bDlrSource, session.GetSessionSettings(screenId), coluName, SVGReferenceId, controlID, clientTimezoneOffset, startTimestamp, endTimestamp, (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), rangeType), token);
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
            try
            {
                await Task.Run(() =>
                {
                    ret = PlatformComponents.PlatformComponents.LoadDataloggerViewerData(connectionString, historicalList, selectedHistorian, maxRows, commandTimeout, (WPFUtilities.DateSpan)Enum.Parse(typeof(WPFUtilities.DateSpan), rangeType), startTimestamp, endTimestamp, session.GetSessionSettings(screenId), token);
                }, token);
            }
            catch (TaskCanceledException) { }

            return ret;
        }

        public async Task<string> LoadDataGridData(string screenId, string thisGridID, int commandTimeout, string connectionString, string dataProvider, string connection, string select, string where, string groupBy, string sort, string tableName, uint maxTransactionsBeforeCommit, bool allowPrimaryKeyChanging)
        {
            string ret = String.Empty;
            var session = Session.Session.GetSession(Context.ConnectionId);
            var token = session.GetCancellationToken(screenId);
            try
            {
                await Task.Run(() =>
                {
                    ret = session.LoadDataGridData(thisGridID, connectionString, commandTimeout, session.GetSessionSettings(screenId), dataProvider, connection, select, where, groupBy, sort, tableName, maxTransactionsBeforeCommit, allowPrimaryKeyChanging, token);
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
            try
            {
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
            try
            {
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
            try
            {
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
            try
            {
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
            try
            {
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
