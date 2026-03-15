using DocumentManager.ComponentService;
using Microsoft.AspNetCore.SignalR;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using ViewModelLib;
using WebNExTHMI.PlatformComponents;
using Utilities;
using Opc.Ua;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using WPFPenHelpers;
using WPFUtilities;
using MSModel;
using static WebNExTHMI.PlatformComponents.SchedulerData;
using System.Threading;
using System.IO;
using MjpegProcessor;

namespace WebNExTHMI.Session
{
    public class Session : ViewModelBase, IEntityReference
    {
        static Dictionary<String, Session> mapSessions = new Dictionary<string, Session>();

        Dictionary<String, ScreenData> mapScreenData = new Dictionary<String, ScreenData>();
        Dictionary<String, MapData> mapMapData = new Dictionary<String, MapData>();
        Dictionary<String, uint> mapScreenHashes = new Dictionary<String, uint>();
        Dictionary<String, uint> mapLanguageHashes = new Dictionary<String, uint>();

        Dictionary<IDocument, UFProjectManager.UFProjectDocument> projectDocuments;

        OPCUAEntityReference toggleSoundVariable;
        OPCUAEntityReference alarmServerVariable;
        public OPCUAEntityReference schedulerServerVariable;
        OPCUAEntityReference alarmSoundBuzzinVariable;
        PropertyObserver<OPCUAEntityReference> alarmSoundBuzzinVariableReferenceObserver;
        PropertyObserver<MonitoredItemViewModel> alarmSoundBuzzinVariableValueObserver;
        public static ExpressionBucket GlobalExpressionBucket;

        Dictionary<String, RecipeData> mapRecipeData = new Dictionary<String, RecipeData>();
        Dictionary<String, int> activeRecipePaths = new Dictionary<string, int>();
        Object recipeLock = new Object();
        Dictionary<String, DataGridData> mapDataGridData = new Dictionary<String, DataGridData>();
        Dictionary<String, Dictionary<String, SCDataGenerator>> mapStatesChartData = new Dictionary<string, Dictionary<string, SCDataGenerator>>();
        Dictionary<String, Dictionary<String, ChartDataGenerator>> mapChartData = new Dictionary<string, Dictionary<string, ChartDataGenerator>>();
        Dictionary<String, Dictionary<String, ChartXYDataGenerator>> mapChartXYData = new Dictionary<string, Dictionary<string, ChartXYDataGenerator>>();
        Dictionary<String, SchedulerData> mapSchedulerData = new Dictionary<String, SchedulerData>();
        Dictionary<String, DecoderData> mapSessionDecoders = new Dictionary<string, DecoderData>();

        readonly IClientProxy caller;
        readonly static WebSessions.WebSessions activeSessions;
        readonly static int demoModeCountDown = 5;
        readonly internal static int demoModeClickCounter = 10;
        internal DateTime lastDemoMode;
        internal int clickCounterInDemoMode;
        bool lastValueToggleAlarmSound;
        bool lastpushedValueAlarmBuzzing;

        #region Properties
        internal bool InDemoMode
        {
            get
            {
                return !activeSessions.FoundLicense;
            }
        }

        internal bool InDemoModeCountDown
        {
            get
            {
                return InDemoMode && lastDemoMode.AddSeconds(demoModeCountDown) > DateTime.UtcNow;
            }
        }
        #endregion

        static Session()
        {
            GlobalExpressionBucket = new ExpressionBucket();
            activeSessions = new WebSessions.WebSessions();
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                ReleaseAllSession();
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            ReleaseAllSession();
        }

        static void ReleaseAllSession()
        {
            lock (mapSessions)
            {
                mapSessions.Values.ToList().ForEach((session) => activeSessions.Release());
                mapSessions.Clear();
            }
        }

        public Session(IClientProxy c, string clientId)
        {
            caller = c;
            SubscribeProject();
            SubscribeAlarmServer();
            SubscribeSchedulerServer();
            SubscribeToggleAlarmServer();
            SubscribeAlarmBuzzing();
        }

        internal void ShowDemoMode(object state)
        {
            if (InDemoMode && !InDemoModeCountDown)
            {
                lastDemoMode = DateTime.UtcNow;
                clickCounterInDemoMode = 0;
                caller.SendAsync("showDemoMode");
            }
        }

        public static int AddSession(String clientId, IClientProxy c)
        {
            lock (mapSessions)
            {
                if (!activeSessions.Acquire())
                    throw new Exception(String.Format(Properties.Resources.LicenseMaxConcurrentUsers, activeSessions.MaxSessions));

                var session = GetSession(clientId);
                if (session == null)
                {
                    session = new Session(c, clientId);
                    mapSessions.Add(clientId, session);
                }

                return mapSessions.Count;
            }
        }

        public static int RemoveSession(String clientId)
        {
            Session disposable = null;
            int counter = 0;
            lock (mapSessions)
            {
                if (mapSessions.ContainsKey(clientId))
                {
                    disposable = mapSessions[clientId];
                    mapSessions.Remove(clientId);
                    activeSessions.Release();
                }
                counter = mapSessions.Count;
            }
            if (disposable != null)
                disposable.Dispose();

            return counter;
        }

        public static Session GetSession(String clientId)
        {
            lock (mapSessions)
            {
                if (mapSessions.ContainsKey(clientId))
                    return mapSessions[clientId];
            }

            return null;
        }

        public static void SendAllExecuteUICommand(String JsonCommand)
        {
            lock (mapSessions)
            {
                foreach (var session in mapSessions.Values)
                    session.caller.SendAsync("executeCommands", JsonCommand);
            }
        }

        String defaultParameterFile = string.Empty;
        internal String DefaultParameterFile
        {
            get
            {
                return defaultParameterFile;
            }

            set
            {
                if (defaultParameterFile != value)
                    defaultParameterFile = value;
            }
        }

        String currentUser;
        internal String CurrentUser
        {
            get
            {
                return currentUser;
            }

            set
            {
                if (currentUser != value)
                {
                    InvalidateScreenReferences();
                    currentUser = value;
                }
            }
        }

        internal bool IsAuthenticated
        {
            get
            {
                return currentUser != null;
            }
        }

        internal bool IsMobile
        {
            get; set;
        }

        internal int ClientTimezoneOffset
        {
            get; set;
        }

        internal bool EnableUserManager
        {
            get; set;
        }

        ExpressionBucket localExpressionBucket;
        public ExpressionBucket LocalExpressionBucket {
            get
            {
                if (localExpressionBucket == null)
                    localExpressionBucket = new ExpressionBucket();
                return localExpressionBucket;
            }
        }

        public bool IsHashValid(String name, uint hash)
        {
            lock (mapScreenHashes)
            {
                if (mapScreenHashes.ContainsKey(name) &&
                    mapScreenHashes[name] == hash)
                    return true;
            }

            return false;
        }

        public ScreenDataInfo AddScreenData(String screenid, String name, String parameter, IClientProxy caller, IDocument parent, string clientId, string callerScreenId)
        {
            String fullPath = null;
            lock (mapScreenData)
            {
                if (!mapScreenData.ContainsKey(screenid))
                {
                    if (projectDocuments != null && projectDocuments.ContainsKey(parent))
                        parent = projectDocuments[parent];
                    mapScreenData.Add(screenid, new ScreenData(screenid, name, parameter, caller, parent, clientId, callerScreenId));
                    if (mapScreenData[screenid].parameterPath == "*")
                    {
                        if (mapScreenData.ContainsKey(callerScreenId) && !String.IsNullOrEmpty(mapScreenData[callerScreenId].parameterPath))
                            mapScreenData[screenid].parameterPath = mapScreenData[callerScreenId].parameterPath;
                        else if (!String.IsNullOrEmpty(defaultParameterFile))
                            mapScreenData[screenid].parameterPath = defaultParameterFile;
                    }
                    fullPath = mapScreenData[screenid].GetFullPath();
                }
            }

            PromoteIdleExecution(60000);

            return mapScreenData[screenid].screenDataInfo;
        }

        void InvalidateScreenReferences()
        {
            lock (mapScreenData)
            {
                foreach (var screenData in mapScreenData.Values)
                    screenData.InvalidateReferenceAttributes();
            }
        }

        public bool RequireUserLogin(String screenid)
        {
            if (!EnableUserManager)
                return false;

            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenid))
                    return mapScreenData[screenid].RequireUserLogin();
            }

            return false;
        }

        public long DelayUnloadSecs(String screenid)
        {
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenid))
                    return mapScreenData[screenid].DelayUnloadSecs();
            }

            return -1;
        }

        public bool KeepAlwaysInMemory(String screenid)
        {
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenid))
                    return mapScreenData[screenid].KeepAlwaysInMemory();
            }

            return false;
        }

        public bool HideLayoutScreens(String screenid)
        {
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenid))
                    return mapScreenData[screenid].HideLayoutScreens();
            }

            return false;
        }

        protected override void IdleExecution()
        {
            base.IdleExecution();

            UpdateKeepAliveScreen();
            KeepAliveLiveMapPins();

            PromoteIdleExecution(60000);
        }

        public void SetValue(String screenId, int id, String value)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);
            screenData.SetValue(id, value);
        }

        public void ResetStatistics(String screenId, int id)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);
            screenData.ResetStatistics(id);
        }

        public void ToggleAlarmSound()
        {
            if (toggleSoundVariable == null || toggleSoundVariable.MonitoredItemViewModel == null)
                return;
            var value = false;
            if (toggleSoundVariable.MonitoredItemViewModel.DataValue != null)
            {
                var isGood = Opc.Ua.StatusCode.IsGood(toggleSoundVariable.MonitoredItemViewModel.DataValue.StatusCode) ||
                    toggleSoundVariable.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue;
                value = Convert.ToBoolean(toggleSoundVariable.MonitoredItemViewModel.DataValue.Value);
            }
            try
            {
                toggleSoundVariable.MonitoredItemViewModel.WriteValue(!value);
                lastValueToggleAlarmSound = !lastValueToggleAlarmSound;
                caller.SendAsync("AlarmBuzzing", !value && lastpushedValueAlarmBuzzing);
            }
            catch (Exception ex)
            {
                caller.SendAsync("WriteValueError", toggleSoundVariable.HumanReadable, !value, ex.Message);
            }
        }

        public IDictionary<int, String> GetReferenceNameList(String screenId)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);
            return screenData.GetReferenceNameList();
        }

        public string GetParameterPath(String screenId)
        {
            string parameterPath = defaultParameterFile;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    parameterPath = mapScreenData[screenId].parameterPath;
                else
                    parameterPath = defaultParameterFile;
            }
            return parameterPath;
        }

        public Dictionary<string, Tuple<string, string>> GetParametersMap(string screenId)
        {
            var ret = new Dictionary<string, Tuple<string, string>>();
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    foreach (var k in mapScreenData[screenId].parametersMap.Keys)
                        ret.Add(k, new Tuple<string, string>(mapScreenData[screenId].parametersMap[k].RelativePath, mapScreenData[screenId].parametersMap[k].ResolvedNodeId));
            }
            return ret;
        }

        public string GetScreenPath(String screenId)
        {
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    return mapScreenData[screenId].GetFullPath();
            }
            return null;
        }

        #region Scheduler
        public void Scheduler_InitServerConnection(IClientProxy caller, String screenId, int idreference, String schedulerId, string connectionString, CancellationToken ct)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }
            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);

            OPCUAEntityReference reference = screenData.GetReference(idreference);
            
            if (reference == null)
                throw new ArgumentException(Properties.Resources.CannotFindVariableToSetValue);

            SchedulerData schedulerData;
            var key = screenId + "_" + schedulerId;
            lock (mapSchedulerData)
            {
                if (!mapSchedulerData.ContainsKey(key))
                    mapSchedulerData.Add(key, new SchedulerData(EnableUserManager));
                schedulerData = mapSchedulerData[key];
            }

            if (ct.IsCancellationRequested)
                return;

            schedulerData.Scheduler_InitServerConnection(caller, idreference, reference, connectionString, screenData.GetSessionSettings(), ct);
        }

        public List<SimpleScheduledEvent_Web> InitSchedulerOnRuntime(String screenId, String schedulerId, string schedulerName, CancellationToken ct)
        {
            SchedulerData schedulerData = null;
            var key = screenId + "_" + schedulerId;
            lock (mapSchedulerData)
            {
                if (mapSchedulerData.ContainsKey(key))
                    schedulerData = mapSchedulerData[key];
            }

            if (schedulerData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScheduler);

            if (ct.IsCancellationRequested)
                return null;

            return schedulerData.InitSchedulerOnRuntime(schedulerName, CurrentUser, ct);
        }

        public bool SaveCurrentScheduler(String screenId, String schedulerId, MSScheduledAction_Web action)
        {
            SchedulerData schedulerData = null;
            var key = screenId + "_" + schedulerId;
            lock (mapSchedulerData)
            {
                if (mapSchedulerData.ContainsKey(key))
                    schedulerData = mapSchedulerData[key];
            }

            if (schedulerData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScheduler);
            return schedulerData.SaveCurrentScheduler(action);
        }

        public MSScheduledAction_Web AddMissingYearHolidays(string screenId, String schedulerId, string ISORegion, MSScheduledAction_Web clientAction, CancellationToken ct)
        {
            SchedulerData schedulerData = null;
            var key = screenId + "_" + schedulerId;
            lock (mapSchedulerData)
            {
                if (mapSchedulerData.ContainsKey(key))
                    schedulerData = mapSchedulerData[key];
            }

            if (schedulerData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScheduler);

            if (ct.IsCancellationRequested)
                return null;

            return schedulerData.AddMissingYearHolidays(ISORegion, clientAction, ct);
        }

        public Tuple<MSScheduledAction, ScheduledEventWebData> UpdateScheduler(String screenId, String schedulerId, string scheduler, CancellationToken ct)
        {
            SchedulerData schedulerData = null;
            var key = screenId + "_" + schedulerId;
            lock (mapSchedulerData)
            {
                if (mapSchedulerData.ContainsKey(key))
                    schedulerData = mapSchedulerData[key];
            }

            if (schedulerData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScheduler);

            if (ct.IsCancellationRequested)
                return null;

            return schedulerData.UpdateScheduler(scheduler);
        }

        public void SchedulerDispose(String screenId, String schedulerId)
        {
            var key = screenId + "_" + schedulerId;
            SchedulerData toDispose = null;
            lock (mapSchedulerData)
            {
                if (mapSchedulerData.ContainsKey(key))
                {
                    toDispose = mapSchedulerData[key];
                    mapSchedulerData.Remove(key);
                }
            }
            toDispose?.Dispose();
        }
        #endregion

        public List<ConditionData> GetConditionStateList(String screenId, int id, CancellationToken ct, bool bSortByTimeDescending = false, bool bNeedsRefresh = false)
        {
            if (ct.IsCancellationRequested)
                return new List<ConditionData>();
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);
            return screenData.GetConditionStateList(id, ct, bSortByTimeDescending, bNeedsRefresh);
        }

        public void AckReset(String screenId, int id, string[] list, bool isReset)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData == null)
                throw new ArgumentException(Properties.Resources.CannotFindScreenToSetValue);
            screenData.AckReset(id, list, isReset);
        }

        public string InitializeRecipeDocument(string recipePath, int readWriteTimeOut)
        {
            var recipeNames = InitializeRecipeDataSet(recipePath, readWriteTimeOut);
            return JsonConvert.SerializeObject(recipeNames);
        }

        public Dictionary<string, List<JToken>> InitializeRecipeDataSet(string recipePath, int readWriteTimeOut, bool bExecutingCommand = false)
        {
            RecipeData recipeData;
            bool bRefill;
            Dictionary<string, List<JToken>> ret = new Dictionary<string, List<JToken>>();
            lock (recipeLock)
            {
                bRefill = mapRecipeData.ContainsKey(recipePath);
                if (!bRefill)
                    mapRecipeData[recipePath] = new RecipeData(recipePath, readWriteTimeOut, bExecutingCommand);
                recipeData = mapRecipeData[recipePath];
                if (!activeRecipePaths.ContainsKey(recipePath))
                    activeRecipePaths[recipePath] = 0;
                activeRecipePaths[recipePath]++;
                if (bRefill)
                    recipeData.Refill();
                try
                {
                    ret = recipeData.GetRecipeNames();
                }
                catch (Exception)
                {
                    if (!bExecutingCommand)
                        throw;
                }
            }
            ret.Add("recipeReadWrite", new List<JToken>() { mapRecipeData[recipePath].HasReferences, mapRecipeData[recipePath].HasReferences });
            if (recipeData.LastFillError != null)
                ret.Add("lastFillError", new List<JToken>() { recipeData.LastFillError });
            return ret;
        }

        public string LoadDataGridData(string thisGridID, string connectionString, string sessionString, string dataProvider, string connection, string select, string where, string groupBy, string sort, string tableName, uint maxTransactionsBeforeCommit, bool allowPrimaryKeyChanging, CancellationToken ct)
        {
            DataGridData gridData;
            if (!mapDataGridData.ContainsKey(thisGridID))
                mapDataGridData[thisGridID] = new DataGridData();
            gridData = mapDataGridData[thisGridID];
            return gridData.LoadDataGridData(connectionString, sessionString, dataProvider, connection, select, where, groupBy, sort, tableName, maxTransactionsBeforeCommit, allowPrimaryKeyChanging, ct);
        }

        public ChartData LoadChartHistoricalData(int commandTimeout, int maxRecord, string nodeID, string sessionString, string controlID, string historicalName, CancellationToken ct)
        {
            ChartData ret = new ChartData();
            if (ct.IsCancellationRequested)
                return ret;

            lock (mapChartData)
            {
                if (!mapChartData.ContainsKey(controlID))
                    mapChartData[controlID] = new Dictionary<string, ChartDataGenerator>();
            }

            ChartDataGenerator generator;

            var connString = PlatformComponents.PlatformComponents.InitChartHistoricalData(historicalName, sessionString);

            lock (mapChartData)
            {
                if (!mapChartData[controlID].ContainsKey(nodeID))
                {
                    var settings = new DataGeneratorSettings()
                    {
                        HDataCount = maxRecord,
                        //ConnectionString = connString,
                        ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connString, GetProjectDocument().rootBase),
                        DeadBandInterval = new TimeSpan(0, 0, 0, 0, 250),
                        DeadBandTimeFrame = new TimeSpan(0, 0, 1, 0)
                    };
                    mapChartData[controlID][nodeID] = new ChartDataGenerator(nodeID, settings, commandTimeout);
                    generator = mapChartData[controlID][nodeID];
                }
                else
                    generator = mapChartData[controlID][nodeID];
            }

            generator.CallSetDataSources(ct);
            ret.Values = generator.Values.ToList();
            return ret;
        }

        public void LoadMjpegStream(string uriId, Uri uri, string user, string password, int closeTimeout)
        {
            lock (mapSessionDecoders) {
                if (!mapSessionDecoders.ContainsKey(uriId))
                {
                    mapSessionDecoders[uriId] = PlatformComponents.PlatformComponents.GetOrCreateMjpegDecoder(uriId, uri, user, password, closeTimeout);
                    mapSessionDecoders[uriId].Decoder.WebHMIFrameReady += OnWebHMIFrameReady;
                    mapSessionDecoders[uriId].Decoder.Error += OnMjpegStreamError;
                }
            }
        }
        public Tuple<IPCameraFrame, string> GetLastFrameData(string sourceId, string lastFrameHash)
        {
            IPCameraFrame frame = null;
            string latestError = String.Empty;
            lock (mapSessionDecoders)
            {
                if (mapSessionDecoders.ContainsKey(sourceId))
                {
                    if (mapSessionDecoders[sourceId].LatestFrame?.HasDifferentHash(lastFrameHash) == true)
                        frame = mapSessionDecoders[sourceId].LatestFrame;
                    latestError = mapSessionDecoders[sourceId].LatestError;
                }
            }
            return new Tuple<IPCameraFrame, string>(frame, latestError);
        }
        public void OnWebHMIFrameReady(object sender, WebHMIFrameReadyEventArgs e)
        {
            lock (mapSessionDecoders)
            {
                if (!mapSessionDecoders.ContainsKey(e.sourceId))
                    return;

                mapSessionDecoders[e.sourceId].UpdateLatestRequestTimestamp();
                mapSessionDecoders[e.sourceId].LatestFrame = new IPCameraFrame(e.FrameBase64, String.Format("{0}", Guid.NewGuid()));
            }
        }
        public void OnMjpegStreamError(object sender, WebHMIErrorEventArgs e)
        {
            Uri uri;
            string user, pwd;
            int closeTimeout;
            lock (mapSessionDecoders)
            {
                if (!mapSessionDecoders.ContainsKey(e.sourceId))
                    return;

                mapSessionDecoders[e.sourceId].UpdateLatestRequestTimestamp();
                mapSessionDecoders[e.sourceId].LatestError = e.GetException().Message;
                
                uri = mapSessionDecoders[e.sourceId].Uri;
                user = mapSessionDecoders[e.sourceId].User;
                pwd = mapSessionDecoders[e.sourceId].Password;
                closeTimeout = mapSessionDecoders[e.sourceId].CloseTimeout;
            }
            DisposeMjpegStream(e.sourceId, true);
            LoadMjpegStream(e.sourceId, uri, user, pwd, closeTimeout);
        }

        public void DisposeMjpegStream(string uriId, bool bForcedStreamDispose = false)
        {
            lock (mapSessionDecoders) {
                if (mapSessionDecoders.ContainsKey(uriId)) {
                    mapSessionDecoders[uriId].Decoder.WebHMIFrameReady -= OnWebHMIFrameReady;
                    mapSessionDecoders[uriId].Decoder.Error -= OnMjpegStreamError;
                    mapSessionDecoders.Remove(uriId);
                    PlatformComponents.PlatformComponents.CheckDisposeMjpegDecoder(uriId, bForcedStreamDispose);
                }
            }
        }

        public XYChartData LoadChartXYHistoricalData(int commandTimeout, int maxRecord, string XNodeID, string YNodeID, string sessionString, string controlID, string XHistoricalName, string YHistoricalName, CancellationToken ct)
        {
            XYChartData ret = new XYChartData();
            if (ct.IsCancellationRequested)
                return ret;
            
            var connString = PlatformComponents.PlatformComponents.InitChartHistoricalData(XHistoricalName ?? YHistoricalName, sessionString);
            var settings = new DataGeneratorSettings()
            {
                HDataCount = maxRecord,
                //ConnectionString = connString,
                ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connString, GetProjectDocument().rootBase),
                DeadBandInterval = new TimeSpan(0, 0, 0, 0, 250),
                DeadBandTimeFrame = new TimeSpan(0, 0, 1, 0)
            };
            var generator = new ChartXYDataGenerator(XNodeID, YNodeID, settings, commandTimeout);
            generator.CallSetDataSources(ct);
            ret.Values = generator.Values.ToList();
            generator.Dispose();
            return ret;
        }

        #region DataReader
        public List<string> GetItemSources(string xmlUri, string xmlItems, int maxTake, string dataProvider, string connection, string select, string where, string groupBy, string sort, CancellationToken ct)
        {
            var ret = new List<string>();
            connection = XpoHelpers.XpoHelper.NormalizeConnectionString(connection, GetProjectDocument()?.rootBase);
            if (!String.IsNullOrEmpty(xmlUri) &&
                !String.IsNullOrEmpty(xmlItems)) {
                var data = Utilities.XmlHelper.GetExpandoCTSFromXml(xmlUri,
                    xmlItems, null, maxTake).ToList();
                if (ct.IsCancellationRequested)
                    return ret;
                foreach (var element in data)
                {
                    if (ct.IsCancellationRequested)
                        return ret;
                    ret.Add(new Dictionary<string, object>(element).Values.First().ToString());
                }
                return ret;
            }

            if (!String.IsNullOrEmpty(dataProvider) &&
                !String.IsNullOrEmpty(connection) &&
                !String.IsNullOrEmpty(select))
            {
                var data = DataReader.DataReader.GetDynamicSqlData(dataProvider, connection,
                                            select, where,
                                            groupBy, sort,
                                            null, maxTake).ToList();
                foreach (var element in data)
                {
                    if (ct.IsCancellationRequested)
                        return ret;
                    ret.Add(new Dictionary<string, object>(element).Values.First().ToString());
                }
                return ret;
            }
            return ret;
        }
        #endregion

        public StatesChartData LoadStatesChartHistoricalData(string connString, int commandTimeout, int maxRecord, TimeSpan viewTimeFrame, TimeSpan recordEvery, string nodeID, string historicalname, bool bDlrSource, string sessionString, string coluName, string controlID, int clientTimezoneOffset, long? startTimestamp, long? endTimestamp, DateSpan rangeType, CancellationToken ct)
        {
            StatesChartData ret = new StatesChartData();
            if (ct.IsCancellationRequested)
                return ret;

            bool bContainsNodeId = false;
            lock (mapStatesChartData)
            {
                if (!mapStatesChartData.ContainsKey(controlID))
                    mapStatesChartData[controlID] = new Dictionary<string, SCDataGenerator>();
                else
                    bContainsNodeId = mapStatesChartData[controlID].ContainsKey(nodeID);
            }

            DateTime DateTimeStart;
            DateTime DateTimeEnd;

            if (startTimestamp != null && endTimestamp != null)
            {
                DateTimeStart = DateTimeOffset.FromUnixTimeSeconds((long)startTimestamp).DateTime.ToLocalTime();
                DateTimeEnd = DateTimeOffset.FromUnixTimeSeconds((long)endTimestamp).DateTime.ToLocalTime();
            }
            else
            {
                var dt = PlatformComponents.PlatformComponents.SetTimeSpan(rangeType);
                DateTimeStart = dt.Item1;
                DateTimeEnd = dt.Item2;
            }

            var timezoneDiff = DateTimeOffset.Now.Offset.Ticks + clientTimezoneOffset * TimeSpan.TicksPerMinute;

            ret.TimezoneDiff = timezoneDiff / TimeSpan.TicksPerSecond * 1000;
            ret.StartDateTimeTicks = DateTimeStart.Ticks - timezoneDiff;
            ret.EndDateTimeTicks = DateTimeEnd.Ticks - timezoneDiff;

            var settingStorage = new SettingsStorage()
            {
                DateTimeStart = DateTimeStart,
                DateTimeEnd = DateTimeEnd
            };

            //Max number of columns in the current viewframe
            var maxDataCount = viewTimeFrame.TotalMilliseconds / recordEvery.TotalMilliseconds;
            SCDataGenerator generator;

            if (!bContainsNodeId) {
                var scData = PlatformComponents.PlatformComponents.InitStatesChartHistoricalData(connString, historicalname, bDlrSource, sessionString);
                var settings = new SCDataGeneratorSettings()
                {
                    ConnectionString = scData.connectionString,
                    ClientTimezoneOffset = TimeSpan.FromMinutes(clientTimezoneOffset),
                    DataCount = (int)Math.Ceiling(maxDataCount), 
                    HDataCount = maxRecord,
                    MDataCount = 5000,
                    DeadBandInterval = recordEvery,
                    DeadBandTimeFrame = viewTimeFrame,
                    DlrSource = bDlrSource,
                    DlrName = scData.tablename,
                    ColName = coluName,
                    UtcTimeColumnName = scData.utccolumnname,
                    Storage = settingStorage
                };

                lock (mapStatesChartData)
                {
                    mapStatesChartData[controlID][nodeID] = new SCDataGenerator(nodeID, settings, commandTimeout);
                    generator = mapStatesChartData[controlID][nodeID];
                }
            }
            else
            {
                lock (mapStatesChartData)
                {
                    generator = mapStatesChartData[controlID][nodeID];
                    generator.Settings.Storage = settingStorage;
                    generator.Settings.DeadBandInterval = recordEvery;
                    generator.Settings.DeadBandTimeFrame = viewTimeFrame;
                    generator.Settings.DataCount = (int)Math.Ceiling(maxDataCount);
                }
            }
            if (ct.IsCancellationRequested)
                return ret;
            generator.CallSetDataSources(ct);
            ret.Values = generator.Values.ToList();
            ret.SecondValues = generator.SecondValues.ToList();
            return ret;
        }

        public void GridControlDBSave(string thisGridID, string sessionString, object pendingChanges)
        {
            DataGridData gridData = mapDataGridData[thisGridID];
            gridData.GridControlDBSave(sessionString, pendingChanges);
        }

        public RecipeItemsData FilterSingleRecipeData(string recipePath, string selectedSubrecipeName, CancellationToken ct)
        {
            if (!mapRecipeData.ContainsKey(recipePath))
                throw new ArgumentException("RecipeDataSetNotInitialized");

            ct.ThrowIfCancellationRequested();

            var ret = new RecipeItemsData() { items = new List<RecipeItem>() };
            lock (recipeLock)
            {
                ret = mapRecipeData[recipePath].GetRecipeItems(selectedSubrecipeName, ct);
            }
            return ret;
        }

        public List<UFRecipeSettings.UFRecipeModel.UFDataValueEntity> GetFlatDataValues(string recipePath, CancellationToken token)
        {
            if (!mapRecipeData.ContainsKey(recipePath))
                throw new ArgumentException("RecipeDataSetNotInitialized");

            token.ThrowIfCancellationRequested();

            return mapRecipeData[recipePath].RecipeDocument.RecipeEntity.GetFlatDataValuesCollection();
        }

        public List<string> RecipeDBLoadNames(String screenId, int id)
        {
            var ret = new List<string>() { "FirstRecipe", "SecondRecipe" };
            return ret;
        }

        public void RecipeDBSave(String screenId, int id, string recipePath, object pendingChanges, string stringRecipeGuid, string newRecipePrefix, bool bDeleting)
        {
            mapRecipeData[recipePath].RecipeDBSave(pendingChanges, stringRecipeGuid, newRecipePrefix, bDeleting);
        }

        public void RecipeDeviceSave(String screenId, int id, string recipePath, object pendingChanges, string stringRecipeGuid, string newRecipePrefix)
        {
            mapRecipeData[recipePath].RecipeDeviceSave(pendingChanges, stringRecipeGuid, newRecipePrefix);
        }

        public RecipeItemsData RecipeDeviceLoad(String screenId, int id, string recipePath, string selectedSubrecipeName, string recipeGuid, CancellationToken ct)
        {
            return mapRecipeData[recipePath].RecipeDeviceLoad(selectedSubrecipeName, recipeGuid, ct);
        }

        public void RecipeCommandExecute(String recipeStringPath, int commandType, int syncTimeout, bool bSynchronous)
        {
            lock (recipeLock)
            {
                if (!mapRecipeData.ContainsKey(recipeStringPath))
                    InitializeRecipeDataSet(recipeStringPath, 0, true);
            }
            mapRecipeData[recipeStringPath].RecipeCommandExecute(new Uri(recipeStringPath, UriKind.RelativeOrAbsolute), commandType, syncTimeout, bSynchronous);
        }

        public void RecipeDispose(string recipePath)
        {
            RecipeData recipeData = null;
            lock (recipeLock)
            {
                if (mapRecipeData.ContainsKey(recipePath) && activeRecipePaths.ContainsKey(recipePath))
                {
                    activeRecipePaths[recipePath]--;
                    if (activeRecipePaths[recipePath] == 0)
                    {
                        activeRecipePaths.Remove(recipePath);
                        recipeData = mapRecipeData[recipePath];
                        mapRecipeData.Remove(recipePath);
                    }
                }
            }
            if (recipeData != null)
                recipeData.Dispose();
        }

        public void AckResetAll(bool isReset)
        {
            if (alarmServerVariable != null)
            {
                if (isReset)
                    alarmServerVariable.MonitoredItemViewModel.ConditionConfirmAllCommand.Execute(null);
                else
                    alarmServerVariable.MonitoredItemViewModel.ConditionAcknowledgeAllCommand.Execute(null);
            }
        }

        public void AddScreenHash(String name, uint hash)
        {
            lock (mapScreenHashes)
            {
                if (mapScreenHashes.ContainsKey(name))
                    mapScreenHashes.Remove(name);

                mapScreenHashes.Add(name, hash);
            }
        }

        public bool IsLanguageHashValid(String culture, uint hash)
        {
            lock (mapLanguageHashes)
            {
                if (mapLanguageHashes.ContainsKey(culture) &&
                    mapLanguageHashes[culture] == hash)
                    return true;
            }

            return false;
        }

        public void AddLanguageHash(String culture, uint hash)
        {
            lock (mapLanguageHashes)
            {
                if (mapLanguageHashes.ContainsKey(culture))
                    mapLanguageHashes.Remove(culture);

                mapLanguageHashes.Add(culture, hash);
            }
        }

        public void UpdateKeepAliveScreen(String id = null, bool bForce = false)
        {
            var listDead = new List<ScreenData>();
            lock (mapScreenData)
            {
                if (!String.IsNullOrEmpty(id) && mapScreenData.ContainsKey(id))
                {
                    mapScreenData[id].KeepAlive();
                }

                var deadScreens = (from pair in mapScreenData.AsParallel() where bForce || !pair.Value.IsAlive() select pair.Key).ToList();
                foreach (var key in deadScreens)
                {
                    listDead.Add(mapScreenData[key]);
                    mapScreenData.Remove(key);
                }
            }

            listDead.ForEach(screenData => screenData.Dispose());
        }

        public void RemoveScreenData(String id)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(id))
                {
                    screenData = mapScreenData[id];
                    mapScreenData.Remove(id);
                }
            }

            if (screenData != null)
                screenData.Dispose();
        }

        internal IDocument GetProjectDocument()
        {
            var projectDocument = PlatformComponents.PlatformComponents.GetProjectDocument();
            if (projectDocuments != null && projectDocuments.ContainsKey(projectDocument))
                return projectDocuments[projectDocument];
            else
                return projectDocument;
        }

        internal string GetSessionSettings(string id)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(id))
                {
                    screenData = mapScreenData[id];
                }
            }

            if (screenData != null)
                return screenData.GetSessionSettings();
            else
                return PlatformComponents.PlatformComponents.GetSessionString();
        }

        internal void UnregisterLiveMapPins(string id)
        {
            MapData screenMap = null;
            lock (mapMapData)
            {
                if (mapMapData.ContainsKey(id))
                {
                    screenMap = mapMapData[id];
                    mapMapData.Remove(id);
                }
            }

            if (screenMap != null)
                screenMap.Dispose();
        }

        internal void KeepAliveLiveMapPins(string id = null, bool bForce = false)
        {
            var listDead = new List<MapData>();
            lock (mapMapData)
            {
                if (!String.IsNullOrEmpty(id) && mapMapData.ContainsKey(id))
                {
                    mapMapData[id].KeepAlive();
                }

                var deadMaps = (from pair in mapMapData.AsParallel() where bForce || !pair.Value.IsAlive() select pair.Key).ToList();
                foreach (var key in deadMaps)
                {
                    listDead.Add(mapMapData[key]);
                    mapMapData.Remove(key);
                }
            }

            listDead.ForEach(mapData => mapData.Dispose());
        }

        internal bool AddLiveMapPins(string id, IClientProxy caller)
        {
            var listgeovar = (from c in PlatformComponents.PlatformComponents.listMapPinsFlat.AsParallel()
                              where !String.IsNullOrEmpty(c.LonTag) && !String.IsNullOrEmpty(c.LatTag)
                              select c).ToList();
            if (listgeovar.Count == 0)
                return false;

            lock (mapMapData)
            {
                if (!mapMapData.ContainsKey(id))
                    mapMapData.Add(id, new MapData(id, listgeovar, caller, PlatformComponents.PlatformComponents.GetSessionString()));
            }

            PromoteIdleExecution(60000);

            return true;
        }

        void SubscribeProject()
        {
            var platformProject = PlatformComponents.PlatformComponents.GetProjectDocument() as UFProjectManager.UFProjectDocument;
            if (platformProject != null)
            {
                projectDocuments = new Dictionary<IDocument, UFProjectManager.UFProjectDocument>();
                var projectDocument = UFProjectManager.UFProjectDocument.FromFile(platformProject.ProjectPath, PlatformComponents.PlatformComponents.UFProjectManagerComponent);

                projectDocuments.Add(platformProject, projectDocument);

                projectDocument.UpdateSessionSettings();

                OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
                OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
                OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();

                foreach(UFProjectManager.UFProjectDocument child in platformProject.Childs)
                    SubscribeChildProjects(child, projectDocument);
            }
        }

        void SubscribeChildProjects(UFProjectManager.UFProjectDocument platformProject, IDocument parent)
        {
            var projectDocument = UFProjectManager.UFProjectDocument.FromFile(platformProject.ProjectPath, PlatformComponents.PlatformComponents.UFProjectManagerComponent);
            projectDocument.Parent = parent;
            projectDocuments.Add(platformProject, projectDocument);

            projectDocument.UpdateSessionSettings();

            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();

            foreach (UFProjectManager.UFProjectDocument child in platformProject.Childs)
                SubscribeChildProjects(child, projectDocument);
        }

        void UnsubscribeProject()
        {
            if (projectDocuments != null)
            {
                foreach (var doc in projectDocuments.Values)
                    doc.Dispose();
                projectDocuments.Clear();
            }
        }

        void SubscribeAlarmBuzzing()
        {
            var serverXML = PlatformComponents.PlatformComponents.UFUAEditorComponent.GetServerEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
            if (serverXML == null)
                return;

            alarmSoundBuzzinVariable = PlatformComponents.PlatformComponents.GetAlarmsSoundBuzzingNameNodeId().FromXml<OPCUAEntityReference>();

            alarmSoundBuzzinVariableReferenceObserver = new PropertyObserver<OPCUAEntityReference>(alarmSoundBuzzinVariable)
                .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    if (n.MonitoredItemViewModel != null)
                    {
                        alarmSoundBuzzinVariableValueObserver?.Dispose();
                        alarmSoundBuzzinVariableValueObserver = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);

                        try
                        {
                            if (n.MonitoredItemViewModel.NodeIdModel != null &&
                                !n.MonitoredItemViewModel.NodeIdModel.IsEventNotifier)
                            {
                                if (n.MonitoredItemViewModel.DataValue.Value != null)
                                    AddPendingValueAlarmSoundBuzzin(n.MonitoredItemViewModel.DataValue);
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        alarmSoundBuzzinVariableValueObserver.RegisterHandler(m => m.DataValue, m =>
                        {
                            if (m.DataValue != null)
                                AddPendingValueAlarmSoundBuzzin(m.DataValue);
                        });

                        if (/*(n.MonitoredItemViewModel.NodeIdModel == null ||
                                n.MonitoredItemViewModel.NodeIdModel.IsVariable) &&*/
                            n.MonitoredItemViewModel.DataValue != null &&
                            (Opc.Ua.StatusCode.IsGood(n.MonitoredItemViewModel.DataValue.StatusCode) ||
                            n.MonitoredItemViewModel.DataValue != null &&
                            n.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue))
                            AddPendingValueAlarmSoundBuzzin(n.MonitoredItemViewModel.DataValue);
                    }
                });

            alarmSoundBuzzinVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                PlatformComponents.PlatformComponents.GetProjectDocument());
            alarmSoundBuzzinVariable.SetInUse(this, true);
        }

        void SubscribeAlarmServer()
        {
            var serverXML = PlatformComponents.PlatformComponents.UFUAEditorComponent.GetServerEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
            if (serverXML != null)
            {
                alarmServerVariable = serverXML.FromXml<OPCUAEntityReference>();
                alarmServerVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                    PlatformComponents.PlatformComponents.GetProjectDocument());
                alarmServerVariable.SetInUse(this, true);
            }
        }

        void SubscribeSchedulerServer()
        {
            var serverXML = PlatformComponents.PlatformComponents.SchedulerComponent.GetServerEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
            if (serverXML != null)
            {
                schedulerServerVariable = serverXML.FromXml<OPCUAEntityReference>();
                schedulerServerVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                    PlatformComponents.PlatformComponents.GetProjectDocument());
                schedulerServerVariable.SetInUse(this, true);
            }
        }

        void SubscribeToggleAlarmServer()
        {
            var serverXML = PlatformComponents.PlatformComponents.UFUAEditorComponent.GetServerEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
            if (serverXML != null)
            {
                toggleSoundVariable = PlatformComponents.PlatformComponents.GetToggleAlarmSoundNodeId().FromXml<OPCUAEntityReference>();
                toggleSoundVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                    PlatformComponents.PlatformComponents.GetProjectDocument());
                toggleSoundVariable.SetInUse(this, true);
            }
        }

        void AddPendingValueAlarmSoundBuzzin(DataValue dataValue)
        {
            var pushvalue = false;
            var value = dataValue;
            if (value != null)
            {
                var isGood = Opc.Ua.StatusCode.IsGood(value.StatusCode) || value.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue;
                if (isGood)
                {
                    try
                    {
                        pushvalue = Convert.ToBoolean(value.Value);
                    }
                    catch
                    {

                    }
                }
            }

            if (pushvalue != lastpushedValueAlarmBuzzing)
            {
                lastpushedValueAlarmBuzzing = pushvalue;
                caller.SendAsync("AlarmBuzzing", pushvalue && !lastValueToggleAlarmSound);
            }
        }

        internal CancellationToken GetCancellationToken(string screenId)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData != null)
                return screenData.GetCancellationToken();
            else
                return CancellationToken.None;
        }

        internal void CancelPendingExecution(string screenId)
        {
            ScreenData screenData = null;
            lock (mapScreenData)
            {
                if (mapScreenData.ContainsKey(screenId))
                    screenData = mapScreenData[screenId];
            }

            if (screenData != null)
                screenData.CancelPendingExecution();
        }

        void UnsubscribeAlarmBuzzing()
        {
            alarmSoundBuzzinVariableValueObserver?.Dispose();
            alarmSoundBuzzinVariableReferenceObserver?.Dispose();
            alarmSoundBuzzinVariable?.SetInUse(this, false);
        }

        void UnsubscribeAlarmServer()
        {
            alarmServerVariable?.SetInUse(this, false);
        }

        void UnsubscribeSchedulerServer()
        {
            schedulerServerVariable?.SetInUse(this, false);
        }

        void UnsubscribeToggleAlarmSound()
        {
            toggleSoundVariable?.SetInUse(this, false);
        }
        

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Dispose
        protected override void OnDispose()
        {
            base.OnDispose();

            UnsubscribeAlarmServer();
            UnsubscribeSchedulerServer();
            UnsubscribeToggleAlarmSound();
            UnsubscribeAlarmBuzzing();
            UpdateKeepAliveScreen(bForce: true);
            KeepAliveLiveMapPins(bForce: true);
            UnsubscribeProject();
            if (localExpressionBucket != null)
                localExpressionBucket.Dispose();

            lock (recipeLock)
            {
                foreach (var recipeData in mapRecipeData.Values)
                    recipeData.Dispose();
            }

            lock (mapSchedulerData)
            {
                foreach (var schedulerData in mapSchedulerData.Values)
                    schedulerData.Dispose();
            }

            lock (mapStatesChartData)
            {
                foreach (var scData in mapStatesChartData.Values)
                    foreach (var sc in scData.Values)
                    {
                        sc.Settings.Storage.Dispose();
                        sc.Dispose();
                    }
            }

            lock (mapChartData)
            {
                foreach (var cData in mapChartData.Values)
                    foreach (var sc in cData.Values)
                        sc.Dispose();
            }

            lock (mapSessionDecoders)
            {
                foreach (var uriId in mapSessionDecoders.Keys)
                    PlatformComponents.PlatformComponents.CheckDisposeMjpegDecoder(uriId);
            }
        }
        #endregion
    }
}
