using Opc.Ua;
using Opc.Ua.Server;
using Serilog;
using SharedModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleOpcFileServer
{
    public class SimpleFileServerNodeManager : CustomNodeManager2
    {
        private const string NodeNamespaceUri = "http://simpleopcfileserver.org/UA";
        private ushort _namespaceIndex;
        // Test edit
        private readonly List<IDriver> _drivers = new();
        private IVariableLogger? _logger;

        // Security
        private readonly Dictionary<string, UserConfig> _users = new();
        private readonly Dictionary<string, UserGroupConfig> _userGroups = new();

        // Variables for scripts
        internal readonly System.Collections.Concurrent.ConcurrentDictionary<string, BaseDataVariableState> _variables = new();
        private ScriptManager? _scriptManager;
        private PlcManager? _plcManager;
        private RecipeManager? _recipeManager;
        private SchedulerManager? _schedulerManager;
        private ReportManager? _reportManager;
        private AssetManager? _assetManager;
        private BatchSequenceManager? _batchManager;
        private EventManager? _eventManager;

        // Cached configs for deferred start on redundancy failover
        private List<ScriptConfig>? _cachedScripts;
        private List<PlcProgramConfig>? _cachedPlcPrograms;

        // Redundancy system variable (_System.Redundancy.IsActive)
        private BaseDataVariableState<bool>? _redundancyIsActiveVar;

        private readonly List<NodeId> _rootNodeIds = new();
        private int _nodeCreationCount;
        private int _nodeCreationTotal;

        // Deferred loading support
        private bool _deferLoading;

        // Suppress alarm evaluation and data logging during bulk node creation
        private volatile bool _loading;

        // Pre-built alarm templates — created once, then cloned for each variable.
        // Avoids 100K × base64-decode + binary-deserialize + 7 recursive walks per alarm.
        [ThreadStatic] private static ExclusiveLimitAlarmState? t_limitAlarmTemplate;
        [ThreadStatic] private static OffNormalAlarmState? t_conditionAlarmTemplate;
        private IDictionary<NodeId, IList<IReference>>? _deferredExternalRefs;
        private FileSystemWatcher? _watcher;
        private System.Threading.Timer? _reloadTimer;

        private NodeModel? _lastModel;
        private readonly string _configPath;

        /// <summary>
        /// Resolves a relative database file path to the Data subfolder next to the config file.
        /// Creates the Data directory if it doesn't exist.
        /// </summary>
        private string ResolveDataPath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath) || relativePath.Contains(":memory:"))
                return relativePath;
            var configDir = Path.GetDirectoryName(Path.GetFullPath(_configPath)) ?? ".";
            var dataDir = Path.Combine(configDir, "Data");
            Directory.CreateDirectory(dataDir);
            return Path.GetFullPath(Path.Combine(dataDir, relativePath));
        }

        // Alarm tracking
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, AlarmConditionInfo> _alarmConditions = new();

        // Alarm analytics — in-memory ring buffer for activation / acknowledgement events
        private readonly List<AlarmAnalyticsEvent> _alarmAnalyticsLog = new();
        private readonly object _analyticsLock = new();
        private const int AlarmAnalyticsMaxEvents = 10_000;
        private const int AlarmAnalyticsWindowMinutes = 60;
        private const int AlarmFloodWindowSeconds = 60;
        private const int AlarmFloodThreshold = 10;

        // Event journal
        private EventLogger? _eventLogger;

        // Alarm notification service (Email, Telegram, WhatsApp)
        private NotificationService? _notificationService;

        // Anomaly detection service
        private AnomalyDetectionService? _anomalyDetectionService;

        // Redundancy / HA service
        internal RedundancyService? _redundancy;

        // Sparkplug B edge-node publisher
        private SparkplugPublisher? _sparkplugPublisher;

        // Retentive variable storage
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _retentiveValues = new();
        private string? _retentivePath;
        private System.Threading.Timer? _retentiveSaveTimer;

        // Variable statistics tracking
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, VariableStatisticsTracker> _statsTrackers = new();

        // Diagnostics OPC UA node
        private BaseDataVariableState<string>? _diagVariable;
        private System.Threading.Timer? _diagTimer;
        private System.Threading.Timer? _demoCheckTimer;

        public SimpleFileServerNodeManager(IServerInternal server, ApplicationConfiguration configuration, string configPath)
        : base(server, configuration, NodeNamespaceUri)
        {
            _configPath = configPath;
            _deferLoading = true;
            SystemContext.NodeIdFactory = this;

            // Hook up user validation
            if (server.SessionManager != null)
            {
                server.SessionManager.ImpersonateUser += (s, e) => SessionManager_ImpersonateUser(s, e);
            }

            InitializeDrivers();
            SetupWatcher();
            LoadRetentiveValues();
        }

        public override void Write(
            OperationContext context,
            IList<WriteValue> nodesToWrite,
            IList<ServiceResult> errors)
        {
            if (nodesToWrite == null) throw new ArgumentNullException(nameof(nodesToWrite));
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            for (int i = 0; i < nodesToWrite.Count; i++)
            {
                var wv = nodesToWrite[i];

                // Skip items already handled by another node manager.
                if (wv.Processed)
                {
                    continue;
                }

                try
                {
                    if (wv.AttributeId != Attributes.Value)
                    {
                        // Not our concern — leave Processed=false so the
                        // base node manager or others can handle it.
                        continue;
                    }

                    var variable = FindPredefinedNode(wv.NodeId, typeof(BaseDataVariableState)) as BaseDataVariableState;
                    if (variable == null)
                    {
                        // Node doesn't belong to this manager — leave
                        // Processed=false so MasterNodeManager tries others.
                        continue;
                    }

                    if (variable is ServerVariableState serverVar)
                    {
                        // Mark as processed so MasterNodeManager knows we handled it.
                        wv.Processed = true;
                        object v = wv.Value?.Value ?? wv.Value;
                        Utils.Trace("WRITE-OVERRIDE: nodeId={0} value={1} (type={2})",
                            wv.NodeId, v, v?.GetType().Name ?? "null");
                        errors[i] = serverVar.InvokeWrite(SystemContext, variable, ref v) ?? ServiceResult.Good;
                        continue;
                    }

                    // For non-ServerVariableState nodes (recipes, etc.), delegate to the base
                    // Write which handles OnWriteValue/WriteAttribute pipeline.
                    // Do NOT set Processed=true before calling base — base checks that flag.
                    var singleErrors = new ServiceResult[1];
                    base.Write(context, new[] { wv }, singleErrors);
                    errors[i] = singleErrors[0] ?? StatusCodes.BadInternalError;
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Write failed for " + wv.NodeId);
                    wv.Processed = true;
                    errors[i] = StatusCodes.BadUnexpectedError;
                }
            }
        }

        private void InitializeDrivers()
        {
            // Redundancy: drivers only run on the active server
            if (_redundancy != null && !_redundancy.IsActive)
            {
                Utils.Trace("[Redundancy] Standby mode — skipping driver initialization.");
                _eventLogger?.LogSystem("Info", "Redundancy", "Standby mode — drivers not started");
                return;
            }

            var loaded = DriverLoader.LoadDrivers(SystemContext);
            _drivers.AddRange(loaded);
            foreach (var d in loaded)
            {
                _eventLogger?.LogDriver("Info", d.Key, $"Driver '{d.Key}' loaded");
                DiagnosticsCollector.Instance.Register("Driver", d.Key, status: "Idle");

                // Subscribe to driver error events
                var driverKey = d.Key;
                d.OnError += (source, message) =>
                {
                    _eventLogger?.LogDriver("Error", $"{driverKey}:{source}", message);
                    DiagnosticsCollector.Instance.SetStatus("Driver", driverKey, "Error", message);
                };

                // Subscribe to driver cycle metrics
                d.OnCycleCompleted += elapsedMs =>
                {
                    DiagnosticsCollector.Instance.RecordCycle("Driver", driverKey, elapsedMs);
                };
            }
        }

        private readonly List<FileSystemWatcher> _resourceWatchers = new();

        private void SetupWatcher()
        {
            try
            {
                var path = Path.GetFullPath(_configPath);
                var dir = Path.GetDirectoryName(path);
                var fileName = Path.GetFileName(path);
                if (dir != null)
                {
                    _watcher = new FileSystemWatcher(dir, fileName);
                    _watcher.Changed += OnFileChanged;
                    _watcher.Created += OnFileChanged;
                    _watcher.Renamed += OnFileChanged;
                    _watcher.EnableRaisingEvents = true;

                    // Watch resource subfolders for external resource file changes
                    foreach (var subfolder in new[] { "scripts", "screens", "plcprograms", "recipes" })
                    {
                        var subDir = Path.Combine(dir, subfolder);
                        if (Directory.Exists(subDir))
                        {
                            var rw = new FileSystemWatcher(subDir, "*.json");
                            rw.Changed += OnFileChanged;
                            rw.Created += OnFileChanged;
                            rw.Deleted += OnFileChanged;
                            rw.Renamed += OnFileChanged;
                            rw.EnableRaisingEvents = true;
                            _resourceWatchers.Add(rw);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Failed to setup file watcher");
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            _reloadTimer?.Dispose();
            _reloadTimer = new System.Threading.Timer(OnReloadTimer, null, 1000, System.Threading.Timeout.Infinite);
        }

        private void OnReloadTimer(object? state)
        {
            _reloadTimer?.Dispose();
            _reloadTimer = null;
            ReloadConfiguration();
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                if (_namespaceIndex == 0)
                {
                    _namespaceIndex = (ushort)SystemContext.NamespaceUris.GetIndexOrAppend(NodeNamespaceUri);
                }

                if (_deferLoading)
                {
                    _deferredExternalRefs = externalReferences;
                    Log.Information("Address space creation deferred until server transport is ready.");
                    return;
                }

                IList<IReference> references = null;
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                try
                {
                    if (File.Exists(_configPath))
                    {
                        NodeModel? nodeModel;
                        using (var fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            nodeModel = JsonSerializer.Deserialize(fs, ServerJsonContext.Default.NodeModel);
                        }

                        // Load external resource files (scripts/, screens/, plcprograms/)
                        if (nodeModel != null)
                            ResourceFileManager.LoadExternalResources(nodeModel, _configPath);

                        LoadModel(nodeModel, externalReferences);
                        if (nodeModel != null) StripModelForCache(nodeModel);
                        _lastModel = nodeModel;
                    }

                    AddReverseReferences(externalReferences);
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Error loading initial configuration");
                }
            }

        }

        public void CompleteDeferredLoad()
        {
            var externalReferences = _deferredExternalRefs;
            _deferredExternalRefs = null;
            if (externalReferences == null) return;

            // Phase 1: Parse and prepare (inside lock)
            NodeModel? nodeModel = null;
            IList<IReference>? references = null;

            lock (Lock)
            {
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                try
                {
                    if (File.Exists(_configPath))
                    {
                        var fileInfo = new FileInfo(_configPath);
                        Log.Information("Parsing configuration file ({SizeMB:F1} MB)...", fileInfo.Length / (1024.0 * 1024.0));
                        var sw = System.Diagnostics.Stopwatch.StartNew();

                        using (var fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            nodeModel = JsonSerializer.Deserialize(fs, ServerJsonContext.Default.NodeModel);
                        }

                        Log.Information("Configuration parsed in {Elapsed:F1}s.", sw.Elapsed.TotalSeconds);

                        if (nodeModel != null)
                        {
                            var varCount = CountVariablesInFolder(nodeModel.Folder);
                            var alarmCount = nodeModel.Folder != null ? CountAlarmsInFolder(nodeModel.Folder) : 0;
                            Log.Information("Project summary: {Variables} variables, {Alarms} alarms, {Scripts} scripts, {PlcPrograms} PLC programs.",
                                varCount, alarmCount, nodeModel.Scripts?.Count ?? 0, nodeModel.PlcPrograms?.Count ?? 0);

                            ResourceFileManager.LoadExternalResources(nodeModel, _configPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Error during deferred configuration parsing");
                    Log.Error(ex, "Error during deferred configuration parsing.");
                    return;
                }
            }

            // Phase 2: LoadModel — runs mostly outside lock, acquires lock per-node via RegisterNode
            if (nodeModel != null)
            {
                try
                {
                    Log.Information("Creating address space...");
                    var swLoad = System.Diagnostics.Stopwatch.StartNew();
                    LoadModel(nodeModel, externalReferences, holdingLock: false);
                    Log.Information("Address space created in {Elapsed:F1}s.", swLoad.Elapsed.TotalSeconds);

                    lock (Lock)
                    {
                        if (nodeModel != null) StripModelForCache(nodeModel);
                        _lastModel = nodeModel;
                        AddReverseReferences(externalReferences);
                    }
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Error during deferred node loading");
                    Log.Error(ex, "Error during deferred node loading.");
                }
            }
            else
            {
                lock (Lock)
                {
                    AddReverseReferences(externalReferences);
                }
            }

            // Push external references to other node managers (e.g. ObjectsFolder).
            // During normal (non-deferred) startup MasterNodeManager calls
            // AddReferences on every node manager after CreateAddressSpace completes.
            // Because we deferred, the dict was empty at that point — replay it now.
            foreach (var kvp in externalReferences)
            {
                Server.NodeManager.AddReferences(kvp.Key, kvp.Value);
            }

            // Start demo license expiry check timer (fires every 30s)
            if (SharedModels.LicenseManager.DemoStartedUtc.HasValue)
            {
                _demoCheckTimer = new System.Threading.Timer(OnDemoCheckTimer, null, 30_000, 30_000);
                Log.Information("Demo mode active — full features for {Minutes} minutes.",
                    SharedModels.LicenseManager.Current.DemoGraceMinutes);
            }
        }

        private static int CountAlarmsInFolder(Folder folder)
        {
            int count = folder.Variables.Count(v => v.Alarm != null);
            foreach (var sub in folder.Folders)
                count += CountAlarmsInFolder(sub);
            return count;
        }

        private void ReloadConfiguration()
        {
            lock (Lock)
            {
                Utils.Trace("Reloading configuration..."); 
                try
                {
                    NodeModel? newModel;
                    using (var fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        newModel = JsonSerializer.Deserialize(fs, ServerJsonContext.Default.NodeModel);
                    }

                    // Load external resource files (scripts/, screens/, plcprograms/)
                    if (newModel != null)
                        ResourceFileManager.LoadExternalResources(newModel, _configPath);

                    if (newModel != null)
                    {
                        List<(Variable, string)> newVariables = new();
                        if (_lastModel != null && IsIncrementalChange(_lastModel, newModel, out newVariables)) 
                        {
                            Utils.Trace($"Incremental update detected. Adding {newVariables.Count} new variables.");
                            _eventLogger?.LogSystem("Info", "Config", $"Incremental config update — {newVariables.Count} new variable(s)");
                            foreach (var (variable, parentPath) in newVariables)
                            {
                                var parentNodeId = new NodeId(parentPath, _namespaceIndex);
                                var parentNode = FindPredefinedNode(parentNodeId, typeof(FolderState)) as BaseObjectState; 

                                if (parentNode != null)
                                {
                                    CreateVariable(variable, parentNode, parentPath);
                                }
                                else
                                {
                                    Utils.Trace($"Warning: Parent node {parentPath} not found for incremental update. Falling back to full matching.");
                                    // Could fallback to full reload, but let's continue best effort or fail.
                                }
                            }
                            StripModelForCache(newModel);
                            _lastModel = newModel;
                            Utils.Trace("Configuration updated incrementally.");
                        }
                        else
                        {
                           _eventLogger?.LogSystem("Info", "Config", "Full configuration reload triggered");
                           FullReload(newModel);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Error reloading configuration");
                    _eventLogger?.LogSystem("Error", "Config", $"Configuration reload failed: {ex.Message}");
                }
            }
        }

        private void FullReload(NodeModel nodeModel)
        {
            // Cleanup scripts
            if (_scriptManager != null) { _scriptManager.Dispose(); _scriptManager = null; }
            if (_plcManager != null) { _plcManager.Dispose(); _plcManager = null; }
            if (_recipeManager != null) { _recipeManager.Dispose(); _recipeManager = null; }

            if (_assetManager != null) { _assetManager.Dispose(); _assetManager = null; }
            _variables.Clear();
            if (_batchManager != null) { _batchManager.Dispose(); _batchManager = null; }
            _users.Clear();
            _userGroups.Clear();
            _alarmConditions.Clear();
            _statsTrackers.Clear();

            // Cleanup drivers
            foreach (var d in _drivers) d.Dispose();
            _drivers.Clear();
            InitializeDrivers();

            // Cleanup previous data logger (new one created in LoadModel)
            _logger?.Dispose();
            _logger = null;

            // Cleanup previous event logger (new one created in LoadModel)
            _eventLogger?.Dispose();
            _eventLogger = null;

            // Cleanup previous notification service (new one created in LoadModel)
            _notificationService?.Dispose();
            _notificationService = null;

            // Delete old root nodes
            foreach (var nodeId in _rootNodeIds)
            {
                DeleteNode(SystemContext, nodeId);
            }
            _rootNodeIds.Clear();

            // Re-create — build an externalReferences dictionary so that
            // AddReverseReferences can wire up ObjectsFolder → root folder references
            // making the address space browseable after reload.
            var externalReferences = new Dictionary<NodeId, IList<IReference>>();
            externalReferences[ObjectIds.ObjectsFolder] = new List<IReference>();
            LoadModel(nodeModel, externalReferences);
            AddReverseReferences(externalReferences);

            // Push external references to other node managers (e.g. ObjectsFolder)
            // so that browsing ObjectsFolder shows the new root folder.
            foreach (var kvp in externalReferences)
            {
                Server.NodeManager.AddReferences(kvp.Key, kvp.Value);
            }

            StripModelForCache(nodeModel);
            _lastModel = nodeModel;
            Utils.Trace("Configuration reloaded fully.");
        }

        private void SessionManager_ImpersonateUser(ISession session, ImpersonateEventArgs args)
        {
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                var username = userNameToken.UserName;

                if (_users.TryGetValue(username, out var userConfig))
                {
                    string password = null;
                    if (userNameToken.DecryptedPassword != null)
                    {
                         password = System.Text.Encoding.UTF8.GetString(userNameToken.DecryptedPassword);
                    }

                    // Try hashed password first, then fall back to legacy plain-text
                    bool valid = false;
                    if (!string.IsNullOrEmpty(userConfig.PasswordHash))
                    {
                        valid = PasswordHasher.Verify(password ?? "", userConfig.PasswordHash);
                    }
                    else if (userConfig.Password == password)
                    {
                        valid = true;
                    }

                    if (valid)
                    {
                        args.Identity = new UserIdentity(userNameToken);
                        Utils.Trace("User {0} logged in.", username);
                        _eventLogger?.LogAuth("Info", username, $"User '{username}' logged in");
                        return;
                    }
                }

                _eventLogger?.LogAuth("Warning", username, $"Login failed for user '{username}' — invalid credentials");
                throw new ServiceResultException(StatusCodes.BadUserAccessDenied, "Invalid username or password.");
            }
            else if (args.NewIdentity is AnonymousIdentityToken)
            {
                // Allow anonymous if the server is configured to accept it.
                // Anonymous users will still be constrained by variable AccessLevel/UserAccessLevel.
                args.Identity = new UserIdentity(new AnonymousIdentityToken());
                _eventLogger?.LogAuth("Info", "anonymous", "Anonymous session established");
                return;
            }
        }

        /// <summary>
        /// Strip heavyweight data (Screens, Images, etc.) from the model before caching
        /// as _lastModel. Only Users, UserGroups, Database, Scripts, PlcPrograms, Recipes,
        /// and Folder are needed for incremental change detection.
        /// </summary>
        private static void StripModelForCache(NodeModel model)
        {
            model.Screens = new();
            model.Strings = new();
            model.Images = new();
            model.Cameras = new();
            model.Schedulers = new();
            model.Reports = new();
            model.CalculatedVariables = new();
            model.Assets = new();
            model.BatchSequences = new();
        }

        private bool IsIncrementalChange(NodeModel oldModel, NodeModel newModel, out List<(Variable, string)> newVariables)
        {
            newVariables = new();

            // Check global configs equality
            if (!AreListsEqual(oldModel.Users, newModel.Users)) return false;
            if (!AreListsEqual(oldModel.UserGroups, newModel.UserGroups)) return false;
            if (!AreDatabaseConfigsEqual(oldModel.Database, newModel.Database)) return false;
            if (!AreListsEqual(oldModel.Scripts, newModel.Scripts)) return false;
            if (!AreListsEqual(oldModel.PlcPrograms, newModel.PlcPrograms)) return false;
            if (!AreListsEqual(oldModel.Recipes, newModel.Recipes)) return false;

            // Check Folder Structure
            return CheckFolderStructure(oldModel.Folder, newModel.Folder, "", newVariables);
        }

        private bool CheckFolderStructure(Folder oldFolder, Folder newFolder, string parentPath, List<(Variable, string)> newVariables)
        {
            if (oldFolder.Name != newFolder.Name) return false;

            string currentPath = string.IsNullOrEmpty(parentPath) ? oldFolder.Name : $"{parentPath}.{oldFolder.Name}";
            bool isRoot = string.IsNullOrEmpty(parentPath);
            string childPrefix = isRoot ? "" : currentPath;

            // Check Subfolders
            if (oldFolder.Folders.Count != newFolder.Folders.Count) return false; // No new/deleted folders allowed incrementally

            // Map old folders for comparison
            var oldFoldersMap = oldFolder.Folders.ToDictionary(f => f.Name);
            foreach (var newSubFolder in newFolder.Folders)
            {
                if (!oldFoldersMap.TryGetValue(newSubFolder.Name, out var oldSubFolder)) return false; // Name/Structure mismatch
                if (!CheckFolderStructure(oldSubFolder, newSubFolder, childPrefix, newVariables)) return false; 
            }

            // Check Variables
            // We allow ONLY additions.
            var oldVarsMap = oldFolder.Variables.ToDictionary(v => v.Name);
            
            foreach (var newVar in newFolder.Variables)
            {
                if (oldVarsMap.TryGetValue(newVar.Name, out var oldVar))
                {
                    // Exists in old, must be identical
                    if (!AreVariablesEqual(oldVar, newVar)) return false; // Modification detected
                    oldVarsMap.Remove(newVar.Name); // Track processed
                }
                else
                {
                    // New variable!
                    newVariables.Add((newVar, childPrefix));
                }
            }

            // If oldVarsMap is not empty, it means deletion detected
            if (oldVarsMap.Count > 0) return false;

            return true;
        }

        private bool AreVariablesEqual(Variable v1, Variable v2)
        {
            string s1 = JsonSerializer.Serialize(v1, typeof(Variable), ServerJsonContext.Default);
            string s2 = JsonSerializer.Serialize(v2, typeof(Variable), ServerJsonContext.Default);
            return s1 == s2;
        }

        private bool AreListsEqual<T>(List<T> l1, List<T> l2)
        {
             // Simplest is generic serialization compare for exact config match
             string s1 = JsonSerializer.Serialize(l1, typeof(List<T>), ServerJsonContext.Default);
             string s2 = JsonSerializer.Serialize(l2, typeof(List<T>), ServerJsonContext.Default);
             return s1 == s2;
        }

        private bool AreDatabaseConfigsEqual(DatabaseConfig? d1, DatabaseConfig? d2)
        {
             if (d1 == null && d2 == null) return true;
             if (d1 == null || d2 == null) return false;
             return JsonSerializer.Serialize(d1, typeof(DatabaseConfig), ServerJsonContext.Default) == JsonSerializer.Serialize(d2, typeof(DatabaseConfig), ServerJsonContext.Default);
        }

        private void LoadModel(NodeModel nodeModel, IDictionary<NodeId, IList<IReference>>? externalReferences, bool holdingLock = true)
        {
           IList<IReference>? references = null;
             if (externalReferences != null)
             {
                 externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references);
             }
             if (nodeModel != null)
             {
                 // ─── License enforcement (skipped in DEBUG builds) ────
                 // Note: only user-defined variables (from the Folder tree) are
                 // counted towards MaxVariables. Infrastructure nodes such as
                 // _Diagnostics and Recipe are created separately and are never
                 // restricted by the license.
#if !DEBUG
                 var lic = SharedModels.LicenseManager.Current.License;
                 if (lic != null)
                 {
                     EnforceLicenseLimits(nodeModel, lic);
                 }
#endif
                 // Load Users and Groups
                 if (nodeModel.Users != null)
                 {
                     foreach (var u in nodeModel.Users) _users[u.Username] = u;
                 }
                 if (nodeModel.UserGroups != null)
                 {
                     foreach (var g in nodeModel.UserGroups) _userGroups[g.Name] = g;
                 }

                 // Init Database Logger
                 if (nodeModel.Database != null && !string.IsNullOrEmpty(nodeModel.Database.ConnectionString))
                 {
                     var db = nodeModel.Database;
                     if (db.Provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase)
                         || db.Provider.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
                     {
                         _logger = new SqliteLogger(ResolveDataPath(db.ConnectionString), db.TableName);
                         }
                         else
                         {
                             _logger = new TimescaleLogger(db.ConnectionString, db.TableName);
                         }
                         _logger.Initialize();
                         DiagnosticsCollector.Instance.Register("DataLogger", _logger.GetType().Name);
                 }

                 // Init Event Logger
                 var evtCfg = nodeModel.Server?.EventLog;
                 if (evtCfg == null || evtCfg.Enabled)
                 {
                     var dbPath = ResolveDataPath(evtCfg?.DbPath ?? "events.db");
                     var maxAge = evtCfg?.MaxAgeDays ?? 90;
                     _eventLogger = new EventLogger(dbPath, maxAge);

                     // Query last known active time before logging startup
                     var lastActive = _eventLogger.GetLastEventTime();
                     var lastActiveText = lastActive.HasValue
                         ? $"Last known active: {lastActive.Value:yyyy-MM-dd HH:mm:ss} UTC ({FormatTimeAgo(DateTime.UtcNow - lastActive.Value)} ago)"
                         : "No previous activity recorded (first start)";

                     _eventLogger.LogSystem("Info", "Server", $"Server started — configuration loaded. {lastActiveText}");
                     DiagnosticsCollector.Instance.Register("EventLogger", "EventLog");
                 }

                 // Register alarm analytics provider for diagnostics snapshots
                 DiagnosticsCollector.Instance.AlarmAnalyticsProvider = BuildAlarmAnalytics;

                 // Register system stats provider for diagnostics snapshots
                 DiagnosticsCollector.Instance.SystemStatsProvider = BuildSystemStats;

                 // Alarm notifications (Email, Telegram, WhatsApp, Web Push)
                 var notifCfg = nodeModel.Server?.AlarmNotification;
                 if (notifCfg is { Enabled: true })
                 {
                     var projectDir = Path.GetDirectoryName(Path.GetFullPath(_configPath)) ?? AppContext.BaseDirectory;
                     _notificationService = new NotificationService(notifCfg, projectDir);
                     _eventLogger?.LogSystem("Info", "Notification", "Alarm notification service started");
                 }

                 // Anomaly detection service
                 var anomalyCfg = nodeModel.Server?.AnomalyDetection;
                 if (anomalyCfg is { Enabled: true })
                 {
                     _anomalyDetectionService = new AnomalyDetectionService(this, anomalyCfg, _eventLogger, _notificationService);
                     _eventLogger?.LogSystem("Info", "AnomalyDetection", "Anomaly detection service created");
                 }

                 // Redundancy / High Availability
                 var redCfg = nodeModel.Server?.Redundancy;
                 if (redCfg is { Enabled: true } && !string.IsNullOrEmpty(redCfg.PartnerEndpoint))
                 {
                     _redundancy?.Dispose();
                     _redundancy = new RedundancyService(redCfg, _variables);
                     _redundancy.SetLogger(_logger);
                     _redundancy.SetEventLogger(_eventLogger);
                     DiagnosticsCollector.Instance.SetRedundancyService(_redundancy);

                     // Hook event logger to replicate events to standby
                     if (_eventLogger != null)
                     {
                         var red = _redundancy;
                         _eventLogger.OnEventLogged = (cat, sev, src, msg, det) =>
                             red.EnqueueEventReplication(cat, sev, src, msg, det);
                     }

                     // When role changes, start/stop drivers, scripts, PLC accordingly
                     _redundancy.RoleChanged += role =>
                     {
                         // Update system variables
                         var now = DateTime.UtcNow;
                         if (_redundancyIsActiveVar != null)
                         {
                             _redundancyIsActiveVar.Value = role == RedundancyRole.Active;
                             _redundancyIsActiveVar.Timestamp = now;
                             _redundancyIsActiveVar.ClearChangeMasks(SystemContext, false);
                         }
                         if (_variables.TryGetValue("_System.Redundancy.ActiveRole", out var roleVar))
                         {
                             roleVar.Value = role.ToString();
                             roleVar.Timestamp = now;
                             roleVar.ClearChangeMasks(SystemContext, false);
                         }
                         if (_variables.TryGetValue("_System.Redundancy.PartnerAlive", out var partnerVar))
                         {
                             partnerVar.Value = _redundancy.PartnerAlive;
                             partnerVar.Timestamp = now;
                             partnerVar.ClearChangeMasks(SystemContext, false);
                         }

                         if (role == RedundancyRole.Active)
                         {
                             _eventLogger?.LogSystem("Info", "Redundancy", "Now ACTIVE — starting drivers");
                             InitializeDrivers();

                             // Start scripts if not already running and not configured to run on standby
                             if (_scriptManager == null && _cachedScripts != null && !redCfg.ScriptsRunOnStandby)
                             {
                                 _scriptManager = new ScriptManager(this);
                                 _scriptManager.Initialize(_cachedScripts);
                                 _eventLogger?.LogSystem("Info", "Redundancy", "Scripts started after promotion");
                             }

                             // Start PLC if not already running and not configured to run on standby
                             if (_plcManager == null && _cachedPlcPrograms is { Count: > 0 } && !redCfg.PlcRunOnStandby)
                             {
                                 _plcManager = new PlcManager(this);
                                 _plcManager.Initialize(_cachedPlcPrograms);
                                 _eventLogger?.LogSystem("Info", "Redundancy", "PLC programs started after promotion");
                             }

                             // Start Sparkplug B publisher on promotion
                             if (_sparkplugPublisher == null && nodeModel.Server?.Sparkplug is { Enabled: true } spCfgOnPromotion)
                             {
                                 _sparkplugPublisher = new SparkplugPublisher(spCfgOnPromotion, _variables, _redundancy, _eventLogger);
                                 _ = _sparkplugPublisher.StartAsync();
                                 _eventLogger?.LogSystem("Info", "Redundancy", "SparkplugB publisher started after promotion");
                             }
                         }
                         else
                         {
                             _eventLogger?.LogSystem("Info", "Redundancy", "Now STANDBY — stopping drivers");
                             foreach (var d in _drivers) d.Dispose();
                             _drivers.Clear();

                             // Stop scripts unless configured to run on standby
                             if (!redCfg.ScriptsRunOnStandby && _scriptManager != null)
                             {
                                 _scriptManager.Dispose();
                                 _scriptManager = null;
                                 _eventLogger?.LogSystem("Info", "Redundancy", "Scripts stopped after demotion");
                             }

                             // Stop PLC unless configured to run on standby
                             if (!redCfg.PlcRunOnStandby && _plcManager != null)
                             {
                                 _plcManager.Dispose();
                                 _plcManager = null;
                                 _eventLogger?.LogSystem("Info", "Redundancy", "PLC programs stopped after demotion");
                             }

                             // Stop Sparkplug B publisher on demotion
                             if (_sparkplugPublisher != null)
                             {
                                 _sparkplugPublisher.Dispose();
                                 _sparkplugPublisher = null;
                                 _eventLogger?.LogSystem("Info", "Redundancy", "SparkplugB publisher stopped after demotion");
                             }
                         }
                     };

                     _redundancy.Start();
                     DiagnosticsCollector.Instance.Register("Redundancy", redCfg.Role, status: _redundancy.ActiveRole.ToString());
                     _eventLogger?.LogSystem("Info", "Redundancy", $"Service initialized — role: {_redundancy.ActiveRole}");
                 }

                 Log.Information("Initializing subsystems...");


                 if (nodeModel.Folder != null)
                 {
                     _loading = true;
                     Log.Information("Creating OPC UA nodes...");
                     CreateAddressSpaceParallel(nodeModel.Folder, references, holdingLock);
                     _loading = false;
                     Log.Information("OPC UA nodes created. Total tracked variables: {Count}.", _variables.Count);

                     // Deferred alarm evaluation — now that all nodes are created,
                     // evaluate initial alarm states in a single pass.
                     if (_alarmConditions.Count > 0)
                     {
                         Log.Information("Evaluating initial alarm states for {Count} alarms...", _alarmConditions.Count);
                         foreach (var info in _alarmConditions.Values)
                             EvaluateAlarmCondition(info);
                         Log.Information("Initial alarm evaluation complete.");
                     }
                 }

           lock (Lock)
           { // re-acquire Lock for post-parallel setup

                 // Start all drivers now that loading is complete — timers were deferred
                 // to avoid driver polling competing with parallel node creation.
                 foreach (var d in _drivers) d.Start();

                 // Start anomaly detection after variables are created
                 _anomalyDetectionService?.Start();

                 // Sparkplug B publisher (edge node)
                 var spCfg = nodeModel.Server?.Sparkplug;
                 if (spCfg is { Enabled: true })
                 {
                     _sparkplugPublisher?.Dispose();
                     _sparkplugPublisher = new SparkplugPublisher(spCfg, _variables, _redundancy, _eventLogger);
                     _ = _sparkplugPublisher.StartAsync();
                     _eventLogger?.LogSystem("Info", "SparkplugB", "Publisher initialized");
                 }

                 // scripts — skip on standby unless ScriptsRunOnStandby is set
                 if (nodeModel.Scripts != null && (_redundancy == null || _redundancy.ShouldRunScripts))
                 {
                     _scriptManager = new ScriptManager(this);
                     _scriptManager.Initialize(nodeModel.Scripts);
                 }
                 // Cache script configs for deferred start on failover
                 _cachedScripts = nodeModel.Scripts;

                 // PLC programs — skip on standby unless PlcRunOnStandby is set
                 if (nodeModel.PlcPrograms != null && nodeModel.PlcPrograms.Count > 0
                     && (_redundancy == null || _redundancy.ShouldRunPlc))
                 {
                     _plcManager = new PlcManager(this);
                     _plcManager.Initialize(nodeModel.PlcPrograms);
                 }
                 // Cache PLC configs for deferred start on failover
                 _cachedPlcPrograms = nodeModel.PlcPrograms;

                           // Recipes
                          if (nodeModel.Recipes != null && nodeModel.Recipes.Count > 0)
                          {
                              _recipeManager = new RecipeManager(this, _configPath);
                              _recipeManager.Initialize(nodeModel.Recipes);

                                  // Create OPC variables for each recipe under a "Recipe" folder
                                  CreateRecipeVariables(nodeModel.Recipes, references);
                              }

                              // Schedulers
                              if (nodeModel.Schedulers != null && nodeModel.Schedulers.Count > 0)
                              {
                                  _schedulerManager = new SchedulerManager(this);
                                  _schedulerManager.Initialize(nodeModel.Schedulers);
                              }

// Reports
if (nodeModel.Reports != null && nodeModel.Reports.Count > 0)
{
    _reportManager = new ReportManager(this);
    _reportManager.Initialize(nodeModel.Reports);
                              }


                  // ─── Assets / Maintenance ───
                  if (nodeModel.Assets != null && nodeModel.Assets.Count > 0)
                  {
                      CreateAssetVariables(nodeModel.Assets, references);
                      _assetManager = new AssetManager(this);
                      _assetManager.Initialize(nodeModel.Assets);
                  }

                  // ─── Batch / Sequence Manager ───
                  if (nodeModel.BatchSequences != null && nodeModel.BatchSequences.Count > 0)
                  {
                      CreateBatchVariables(nodeModel.BatchSequences, references);
                      _batchManager = new BatchSequenceManager(this);
                      _batchManager.Initialize(nodeModel.BatchSequences);
                  }

                  // ─── Events (condition → command) ───
                  if (nodeModel.Events != null && nodeModel.Events.Count > 0)
                  {
                      CreateEventVariables(nodeModel.Events, references);
                      _eventManager = new EventManager(this);
                      _eventManager.Initialize(nodeModel.Events);
                  }
                              // ─── Diagnostics OPC UA node (always created, license-exempt) ───
                          CreateDiagnosticsNode(references);

                          // ─── Redundancy system variable ───
                          if (_redundancy != null)
                              CreateRedundancySystemVariable(references);
                      }
           } // end re-acquired lock(Lock)
                 }
        
        private void CreateRecipeVariables(List<RecipeConfig> recipes, IList<IReference>? references)
        {
            // Create a "Recipe" root folder
            var recipeFolder = new FolderState(null);
            var recipeFolderPath = "Recipe";
            recipeFolder.NodeId = new NodeId(recipeFolderPath, _namespaceIndex);
            recipeFolder.BrowseName = new QualifiedName("Recipe", _namespaceIndex);
            recipeFolder.DisplayName = new LocalizedText("Recipe");
            recipeFolder.TypeDefinitionId = ObjectTypeIds.FolderType;
            recipeFolder.ReferenceTypeId = ReferenceTypes.Organizes;

            _rootNodeIds.Add(recipeFolder.NodeId);
            AddPredefinedNode(SystemContext, recipeFolder);
            AddRootNotifier(recipeFolder);
            recipeFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            if (references != null)
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, recipeFolder.NodeId));

            foreach (var recipe in recipes)
            {
                if (!recipe.Enabled) continue;

                var prefix = $"{recipeFolderPath}.{recipe.Name}";

                // Sub-folder per recipe
                var subFolder = new FolderState(recipeFolder);
                subFolder.NodeId = new NodeId(prefix, _namespaceIndex);
                subFolder.BrowseName = new QualifiedName(recipe.Name, _namespaceIndex);
                subFolder.DisplayName = new LocalizedText(recipe.Name);
                subFolder.TypeDefinitionId = ObjectTypeIds.FolderType;
                subFolder.ReferenceTypeId = ReferenceTypes.Organizes;
                recipeFolder.AddChild(subFolder);
                AddPredefinedNode(SystemContext, subFolder);

                // Command variables — write a recipe name to trigger the action
                CreateRecipeCommandVariable(subFolder, prefix, "Load", recipe);
                CreateRecipeCommandVariable(subFolder, prefix, "Save", recipe);
                CreateRecipeCommandVariable(subFolder, prefix, "Activate", recipe);
                CreateRecipeCommandVariable(subFolder, prefix, "Delete", recipe);

                // Status variables — read-only
                CreateRecipeStatusVariable(subFolder, prefix, "ActiveName", "");
                CreateRecipeStatusVariable(subFolder, prefix, "RecipeList", "");
                CreateRecipeStatusVariable(subFolder, prefix, "LastStatus", "");
            }
        }

        private void CreateRecipeCommandVariable(FolderState parent, string prefix, string action, RecipeConfig recipe)
        {
            var path = $"{prefix}.{action}";
            var variable = new BaseDataVariableState<string>(parent);
            variable.NodeId = new NodeId(path, _namespaceIndex);
            variable.BrowseName = new QualifiedName(action, _namespaceIndex);
            variable.DisplayName = new LocalizedText(action);
            variable.DataType = DataTypeIds.String;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Value = "";
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Timestamp = DateTime.UtcNow;
            variable.StatusCode = StatusCodes.Good;

            // Hook the write to trigger the recipe action
            variable.OnWriteValue = (ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp) =>
            {
                var recipeName = value?.ToString() ?? "";
                if (!string.IsNullOrWhiteSpace(recipeName))
                {
                    ExecuteRecipeAction(recipe.Name, action, recipeName);
                }
                return Opc.Ua.ServiceResult.Good;
            };

            parent.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _variables[path] = variable;
        }

        private void CreateRecipeStatusVariable(FolderState parent, string prefix, string name, string defaultValue)
        {
            var path = $"{prefix}.{name}";
            var variable = new BaseDataVariableState<string>(parent);
            variable.NodeId = new NodeId(path, _namespaceIndex);
            variable.BrowseName = new QualifiedName(name, _namespaceIndex);
            variable.DisplayName = new LocalizedText(name);
            variable.DataType = DataTypeIds.String;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Value = defaultValue;
            variable.AccessLevel = AccessLevels.CurrentRead;
            variable.UserAccessLevel = AccessLevels.CurrentRead;
            variable.Timestamp = DateTime.UtcNow;
            variable.StatusCode = StatusCodes.Good;

            parent.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _variables[path] = variable;
        }

        private void ExecuteRecipeAction(string recipeName, string action, string targetRecipeName)
        {
            // Access the RecipeManager's runtime via reflection-free approach:
            // The RecipeManager delegates to RecipeRuntime internally.
            // We expose a simple method dispatch here.
            if (_recipeManager == null) return;

            // We need a way to call the specific recipe runtime.
            // Add a dispatch method to RecipeManager.
            _recipeManager.Execute(recipeName, action, targetRecipeName);
        }

        /// <summary>
        /// Creates a _Diagnostics folder in the OPC UA address space with a DiagnosticsJson
        /// variable that is updated every 2 seconds from DiagnosticsCollector.
        /// The editor reads this variable instead of using a separate HTTP endpoint.
        /// Safe to call on reload — skips if already created.
        /// </summary>

        private void CreateAssetVariables(List<AssetConfig> assets, IList<IReference>? references)
        {
            var rootFolder = new FolderState(null)
            {
                NodeId = new NodeId("_Assets", _namespaceIndex),
                BrowseName = new QualifiedName("_Assets", _namespaceIndex),
                DisplayName = new LocalizedText("Assets"),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };
            if (references != null)
                rootFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            AddPredefinedNode(SystemContext, rootFolder);
            foreach (var asset in assets)
            {
                if (!asset.Enabled) continue;
                var folder = new FolderState(rootFolder)
                {
                    NodeId = new NodeId($"_Assets.{asset.Name}", _namespaceIndex),
                    BrowseName = new QualifiedName(asset.Name, _namespaceIndex),
                    DisplayName = new LocalizedText(asset.Name),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };
                rootFolder.AddChild(folder);
                AddPredefinedNode(SystemContext, folder);
                var prefix = $"_Assets.{asset.Name}";
                CreateAssetVar<bool>(folder, prefix, "Running", false);
                CreateAssetVar<double>(folder, prefix, "RuntimeHours", asset.InitialRuntimeHours);
                CreateAssetVar<bool>(folder, prefix, "Faulted", false);
                CreateAssetVar<bool>(folder, prefix, "ServiceDue", false);
                CreateAssetVar<double>(folder, prefix, "NextServiceIn", 0.0);
                CreateAssetVar<string>(folder, prefix, "NextServiceName", "");
                var resetPath = $"{prefix}.ResetSchedule";
                var resetVar = new BaseDataVariableState<string>(folder)
                {
                    NodeId = new NodeId(resetPath, _namespaceIndex),
                    BrowseName = new QualifiedName("ResetSchedule", _namespaceIndex),
                    DisplayName = new LocalizedText("ResetSchedule"),
                    DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar,
                    Value = "", AccessLevel = AccessLevels.CurrentReadOrWrite,
                    UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                    Timestamp = DateTime.UtcNow, StatusCode = StatusCodes.Good
                };
                resetVar.OnWriteValue = (ISystemContext ctx, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp) =>
                {
                    var schedId = value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(schedId))
                        _assetManager?.ResetSchedule(asset.Name, schedId);
                    return Opc.Ua.ServiceResult.Good;
                };
                folder.AddChild(resetVar);
                AddPredefinedNode(SystemContext, resetVar);
                _variables[resetPath] = resetVar;
            }
        }

        private void CreateAssetVar<T>(FolderState parent, string prefix, string name, T defaultValue)
        {
            var path = $"{prefix}.{name}";
            var variable = new BaseDataVariableState<T>(parent)
            {
                NodeId = new NodeId(path, _namespaceIndex),
                BrowseName = new QualifiedName(name, _namespaceIndex),
                DisplayName = new LocalizedText(name),
                DataType = Opc.Ua.TypeInfo.GetDataTypeId(typeof(T)),
                ValueRank = ValueRanks.Scalar, Value = defaultValue,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                Timestamp = DateTime.UtcNow, StatusCode = StatusCodes.Good
            };
            parent.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _variables[path] = variable;
        }

        private void CreateEventVariables(List<SharedModels.EventConfig> events, IList<IReference>? references)
        {
            var rootFolder = new FolderState(null)
            {
                NodeId = new NodeId("_Events", _namespaceIndex),
                BrowseName = new QualifiedName("_Events", _namespaceIndex),
                DisplayName = new LocalizedText("Events"),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };
            if (references != null)
                rootFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            AddPredefinedNode(SystemContext, rootFolder);

            foreach (var evt in events)
            {
                if (!evt.Enabled) continue;

                var folder = new FolderState(rootFolder)
                {
                    NodeId = new NodeId($"_Events.{evt.Name}", _namespaceIndex),
                    BrowseName = new QualifiedName(evt.Name, _namespaceIndex),
                    DisplayName = new LocalizedText(evt.Name),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };
                rootFolder.AddChild(folder);
                AddPredefinedNode(SystemContext, folder);

                var prefix = $"_Events.{evt.Name}";
                CreateEventVar<bool>(folder, prefix, "Active", false);
                CreateEventVar<DateTime>(folder, prefix, "LastFired", DateTime.MinValue);
            }
        }

        private void CreateEventVar<T>(FolderState parent, string prefix, string name, T defaultValue)
        {
            var path = $"{prefix}.{name}";
            var variable = new BaseDataVariableState<T>(parent)
            {
                NodeId = new NodeId(path, _namespaceIndex),
                BrowseName = new QualifiedName(name, _namespaceIndex),
                DisplayName = new LocalizedText(name),
                DataType = Opc.Ua.TypeInfo.GetDataTypeId(typeof(T)),
                ValueRank = ValueRanks.Scalar, Value = defaultValue,
                AccessLevel = AccessLevels.CurrentRead,
                UserAccessLevel = AccessLevels.CurrentRead,
                Timestamp = DateTime.UtcNow, StatusCode = StatusCodes.Good
            };
            parent.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _variables[path] = variable;
        }

        internal void RaiseAssetAlarm(string assetName, string scheduleName, string message, ushort severity, bool isWarning)
        {
            _eventLogger?.LogAlarm(isWarning ? "Warning" : "Alarm", $"_Assets.{assetName}.{scheduleName}", message);
            Log.Information("Asset alarm [{Cat}] {Asset}/{Schedule}: {Msg}", isWarning ? "Warning" : "Alarm", assetName, scheduleName, message);
        }

        internal void ClearAssetAlarm(string assetName, string scheduleName)
        {
            _eventLogger?.LogAlarm("Info", $"_Assets.{assetName}.{scheduleName}", $"Service alarm cleared for {scheduleName} on {assetName}");
        }

        private void CreateBatchVariables(List<BatchSequenceConfig> sequences, IList<IReference>? references)
        {
            var rootFolder = new FolderState(null)
            {
                NodeId = new NodeId("_Batch", _namespaceIndex),
                BrowseName = new QualifiedName("_Batch", _namespaceIndex),
                DisplayName = new LocalizedText("Batch Sequences"),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };
            if (references != null)
                rootFolder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            AddPredefinedNode(SystemContext, rootFolder);

            foreach (var seq in sequences)
            {
                if (!seq.Enabled) continue;
                var folder = new FolderState(rootFolder)
                {
                    NodeId = new NodeId($"_Batch.{seq.Name}", _namespaceIndex),
                    BrowseName = new QualifiedName(seq.Name, _namespaceIndex),
                    DisplayName = new LocalizedText(seq.Name),
                    TypeDefinitionId = ObjectTypeIds.FolderType
                };
                rootFolder.AddChild(folder);
                AddPredefinedNode(SystemContext, folder);
                var p = $"_Batch.{seq.Name}";
                CreateAssetVar<string>(folder, p, "State", "Idle");
                CreateAssetVar<string>(folder, p, "CurrentStep", "");
                CreateAssetVar<string>(folder, p, "CurrentStepId", "");
                CreateAssetVar<double>(folder, p, "StepElapsed", 0.0);
                CreateAssetVar<double>(folder, p, "TotalElapsed", 0.0);
                CreateAssetVar<int>(folder, p, "StepIndex", -1);
                CreateAssetVar<string>(folder, p, "Message", "Idle");
            }
        }
        private void CreateDiagnosticsNode(IList<IReference>? references)
        {
            // Only create once — the diagnostics node survives reloads
            if (_diagVariable != null)
                return;

            var folder = new FolderState(null);
            folder.NodeId = new NodeId("_Diagnostics", _namespaceIndex);
            folder.BrowseName = new QualifiedName("_Diagnostics", _namespaceIndex);
            folder.DisplayName = new LocalizedText("_Diagnostics");
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.EventNotifier = EventNotifiers.None;

            folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            references?.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));

            _rootNodeIds.Add(folder.NodeId);
            AddPredefinedNode(SystemContext, folder);

            var variable = new BaseDataVariableState<string>(folder);
            variable.NodeId = new NodeId("_Diagnostics.Json", _namespaceIndex);
            variable.BrowseName = new QualifiedName("DiagnosticsJson", _namespaceIndex);
            variable.DisplayName = new LocalizedText("DiagnosticsJson");
            variable.DataType = DataTypeIds.String;
            variable.ValueRank = ValueRanks.Scalar;
            variable.Value = "{}";
            variable.AccessLevel = AccessLevels.CurrentRead;
            variable.UserAccessLevel = AccessLevels.CurrentRead;
            variable.Timestamp = DateTime.UtcNow;
            variable.StatusCode = StatusCodes.Good;

            folder.AddChild(variable);
            AddPredefinedNode(SystemContext, variable);
            _diagVariable = variable;

            // Start a timer that updates the diagnostics variable from DiagnosticsCollector
            _diagTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    var snapshot = DiagnosticsCollector.Instance.BuildSnapshot();
                    var json = JsonSerializer.Serialize(snapshot, DiagnosticsJsonContext.Default.ServerDiagnostics);
                    lock (Lock)
                    {
                        if (_diagVariable != null)
                        {
                            _diagVariable.Value = json;
                            _diagVariable.Timestamp = DateTime.UtcNow;
                            _diagVariable.ClearChangeMasks(SystemContext, false);
                        }
                    }
                }
                catch { }
            }, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Creates _System.Redundancy.IsActive (Boolean, read-only) in the OPC address space.
        /// Updated automatically when the redundancy role changes.
        /// </summary>
        private void CreateRedundancySystemVariable(IList<IReference>? references)
        {
            if (_redundancyIsActiveVar != null) return;

            var folder = new FolderState(null);
            folder.NodeId = new NodeId("_System.Redundancy", _namespaceIndex);
            folder.BrowseName = new QualifiedName("Redundancy", _namespaceIndex);
            folder.DisplayName = new LocalizedText("Redundancy");
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.EventNotifier = EventNotifiers.None;

            // Try to attach under an existing _System folder, otherwise create at root
            var systemFolderId = new NodeId("_System", _namespaceIndex);
            var systemFolder = FindPredefinedNode(systemFolderId, typeof(FolderState)) as FolderState;
            if (systemFolder != null)
            {
                systemFolder.AddChild(folder);
            }
            else
            {
                // Create _System root folder
                var sysRoot = new FolderState(null);
                sysRoot.NodeId = systemFolderId;
                sysRoot.BrowseName = new QualifiedName("_System", _namespaceIndex);
                sysRoot.DisplayName = new LocalizedText("_System");
                sysRoot.TypeDefinitionId = ObjectTypeIds.FolderType;
                sysRoot.EventNotifier = EventNotifiers.None;
                sysRoot.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                references?.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, sysRoot.NodeId));
                _rootNodeIds.Add(sysRoot.NodeId);
                AddPredefinedNode(SystemContext, sysRoot);
                sysRoot.AddChild(folder);
            }
            AddPredefinedNode(SystemContext, folder);

            // IsActive variable
            var isActive = new BaseDataVariableState<bool>(folder);
            isActive.NodeId = new NodeId("_System.Redundancy.IsActive", _namespaceIndex);
            isActive.BrowseName = new QualifiedName("IsActive", _namespaceIndex);
            isActive.DisplayName = new LocalizedText("IsActive");
            isActive.DataType = DataTypeIds.Boolean;
            isActive.ValueRank = ValueRanks.Scalar;
            isActive.Value = _redundancy?.IsActive ?? true;
            isActive.AccessLevel = AccessLevels.CurrentRead;
            isActive.UserAccessLevel = AccessLevels.CurrentRead;
            isActive.Timestamp = DateTime.UtcNow;
            isActive.StatusCode = StatusCodes.Good;
            folder.AddChild(isActive);
            AddPredefinedNode(SystemContext, isActive);
            _redundancyIsActiveVar = isActive;

            // ConfiguredRole variable
            var configuredRole = new BaseDataVariableState<string>(folder);
            configuredRole.NodeId = new NodeId("_System.Redundancy.ConfiguredRole", _namespaceIndex);
            configuredRole.BrowseName = new QualifiedName("ConfiguredRole", _namespaceIndex);
            configuredRole.DisplayName = new LocalizedText("ConfiguredRole");
            configuredRole.DataType = DataTypeIds.String;
            configuredRole.ValueRank = ValueRanks.Scalar;
            configuredRole.Value = _redundancy?.GetDiagnostics().ConfiguredRole ?? "";
            configuredRole.AccessLevel = AccessLevels.CurrentRead;
            configuredRole.UserAccessLevel = AccessLevels.CurrentRead;
            configuredRole.Timestamp = DateTime.UtcNow;
            configuredRole.StatusCode = StatusCodes.Good;
            folder.AddChild(configuredRole);
            AddPredefinedNode(SystemContext, configuredRole);

            // PartnerAlive variable
            var partnerAlive = new BaseDataVariableState<bool>(folder);
            partnerAlive.NodeId = new NodeId("_System.Redundancy.PartnerAlive", _namespaceIndex);
            partnerAlive.BrowseName = new QualifiedName("PartnerAlive", _namespaceIndex);
            partnerAlive.DisplayName = new LocalizedText("PartnerAlive");
            partnerAlive.DataType = DataTypeIds.Boolean;
            partnerAlive.ValueRank = ValueRanks.Scalar;
            partnerAlive.Value = _redundancy?.PartnerAlive ?? false;
            partnerAlive.AccessLevel = AccessLevels.CurrentRead;
            partnerAlive.UserAccessLevel = AccessLevels.CurrentRead;
            partnerAlive.Timestamp = DateTime.UtcNow;
            partnerAlive.StatusCode = StatusCodes.Good;
            folder.AddChild(partnerAlive);
            AddPredefinedNode(SystemContext, partnerAlive);

            // ActiveRole variable
            var activeRole = new BaseDataVariableState<string>(folder);
            activeRole.NodeId = new NodeId("_System.Redundancy.ActiveRole", _namespaceIndex);
            activeRole.BrowseName = new QualifiedName("ActiveRole", _namespaceIndex);
            activeRole.DisplayName = new LocalizedText("ActiveRole");
            activeRole.DataType = DataTypeIds.String;
            activeRole.ValueRank = ValueRanks.Scalar;
            activeRole.Value = _redundancy?.ActiveRole.ToString() ?? "Active";
            activeRole.AccessLevel = AccessLevels.CurrentRead;
            activeRole.UserAccessLevel = AccessLevels.CurrentRead;
            activeRole.Timestamp = DateTime.UtcNow;
            activeRole.StatusCode = StatusCodes.Good;
            folder.AddChild(activeRole);
            AddPredefinedNode(SystemContext, activeRole);

            // Register these in _variables so scripts can Read() them
            _variables["_System.Redundancy.IsActive"] = isActive;
            _variables["_System.Redundancy.ConfiguredRole"] = configuredRole;
            _variables["_System.Redundancy.PartnerAlive"] = partnerAlive;
            _variables["_System.Redundancy.ActiveRole"] = activeRole;
        }

        private void CreateAddressSpaceParallel(Folder rootFolder, IList<IReference> references, bool holdingLock = true)
        {
            // Create root folder node (serial, fast)
            var rootFolderState = new FolderState(null);
            string rootPath = rootFolder.Name;
            rootFolderState.NodeId = new NodeId(rootPath, _namespaceIndex);
            rootFolderState.BrowseName = new QualifiedName(rootFolder.Name, _namespaceIndex);
            rootFolderState.DisplayName = new LocalizedText(rootFolder.Name);
            rootFolderState.TypeDefinitionId = ObjectTypeIds.FolderType;
            rootFolderState.ReferenceTypeId = ReferenceTypes.Organizes;
            rootFolderState.EventNotifier = EventNotifiers.SubscribeToEvents;

            lock (Lock)
            {
                _rootNodeIds.Add(rootFolderState.NodeId);
                AddPredefinedNode(SystemContext, rootFolderState);
                AddRootNotifier(rootFolderState);
            }
            rootFolderState.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            references?.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, rootFolderState.NodeId));

            // Root folder is transparent: children use empty prefix
            var childPrefix = "";

            // Count total variables for progress reporting
            _nodeCreationTotal = CountVariablesInFolder(rootFolder);
            _nodeCreationCount = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            long lastLogTime = 0;

            if (!holdingLock && rootFolder.Folders.Count > 1)
            {
                Log.Information("Parallel node creation: {SubfolderCount} top-level groups, {TotalVars} variables. Using up to {Cores} cores.",
                    rootFolder.Folders.Count, _nodeCreationTotal, Environment.ProcessorCount);

                // Collect subtree roots during the fully lock-free parallel phase.
                var subtreeRoots = new System.Collections.Concurrent.ConcurrentBag<FolderState>();

                // Process subfolders in parallel -- no lock contention at all during this phase.
                Parallel.ForEach(rootFolder.Folders, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, subFolder =>
                {
                    // Pass a non-null batch sentinel to activate batch mode (children build the
                    // tree via AddChild but aren't individually registered -- the bulk registration
                    // pass after all parallel work completes handles dictionary insertion).
                    var batchSentinel = new List<NodeState>(0);
                    FolderState? subFolderState = null;
                    try
                    {
                        subFolderState = CreateFolder(subFolder, rootFolderState, null, childPrefix, batchSentinel);
                    }
                    catch (Exception ex)
                    {
                        Utils.Trace(ex, $"Error creating subfolder '{subFolder.Name}' under root");
                        Log.Error(ex, "Error creating subfolder '{SubFolder}' under root.", subFolder.Name);
                    }
                    if (subFolderState != null)
                        subtreeRoots.Add(subFolderState);

                    // Periodic progress reporting (every 10s)
                    var current = Volatile.Read(ref _nodeCreationCount);
                    var elapsed = sw.ElapsedMilliseconds;
                    var lastLog = Interlocked.Read(ref lastLogTime);
                    if (elapsed - lastLog >= 10_000)
                    {
                        if (Interlocked.CompareExchange(ref lastLogTime, elapsed, lastLog) == lastLog)
                        {
                            var pct = _nodeCreationTotal > 0 ? (int)(100.0 * current / _nodeCreationTotal) : 100;
                            Log.Information("Creating nodes... {Current:N0}/{Total:N0} ({Pct}%) -- {Elapsed:F1}s elapsed.",
                                current, _nodeCreationTotal, pct, sw.Elapsed.TotalSeconds);
                        }
                    }
                });

                // Bulk registration -- single-threaded, no lock needed.
                // All node trees were built in parallel via AddChild; now register them
                // into PredefinedNodes in one fast sequential pass.
                Log.Information("Parallel creation done in {Elapsed:F1}s. Registering {Count} subtrees into address space...",
                    sw.Elapsed.TotalSeconds, subtreeRoots.Count);

                var dict = PredefinedNodes;
                foreach (var root in subtreeRoots)
                {
                    var allNodes = new List<NodeState>(4096);
                    CollectSubtreeNodes(SystemContext, root, allNodes);
                    for (int i = 0; i < allNodes.Count; i++)
                        dict[allNodes[i].NodeId] = allNodes[i];
                }

                Log.Information("Address space registration complete in {Elapsed:F1}s.", sw.Elapsed.TotalSeconds);
            }
            else
            {
                // Single or no subfolders — run sequentially
                foreach (var subFolder in rootFolder.Folders)
                    CreateFolder(subFolder, rootFolderState, null, childPrefix);
            }

            // Process root-level variables (usually few or none)
            foreach (var variable in rootFolder.Variables)
            {
                try { CreateVariable(variable, rootFolderState, childPrefix); }
                catch (Exception ex) { Utils.Trace(ex, $"Error creating root variable '{variable.Name}'"); }
            }

            var finalCount = Volatile.Read(ref _nodeCreationCount);
            Log.Information("Node creation complete: {Count:N0} variables processed in {Elapsed:F1}s.", finalCount, sw.Elapsed.TotalSeconds);
        }

        /// <summary>Thread-safe wrapper for AddPredefinedNode. In batch mode the node is already
        /// attached via AddChild; AddPredefinedNode on the subtree root discovers it recursively.</summary>
        private void RegisterNode(ISystemContext context, NodeState node, List<NodeState>? batch = null)
        {
            if (batch == null) { lock (Lock) { AddPredefinedNode(context, node); } }
            // batch != null: no-op - node is already a child; subtree root flush handles it.
        }

        /// <summary>Thread-safe wrapper for parent.AddChild + AddPredefinedNode. In batch mode
        /// only AddChild is needed - AddPredefinedNode on the subtree root handles registration recursively.</summary>
        private void RegisterChildNode(ISystemContext context, BaseInstanceState child, NodeState parent, List<NodeState>? batch = null)
        {
            if (batch != null)
            {
                // In parallel mode, synchronize AddChild on the parent in case multiple
                // threads share the same parent node (e.g. rootFolderState).
                lock (parent)
                {
                    parent.AddChild(child);
                }
                // Don't add to batch - AddPredefinedNode is recursive and will discover
                // this child when the subtree root is flushed.
            }
            else
            {
                parent.AddChild(child);
                lock (Lock) { AddPredefinedNode(context, child); }
            }
        }

        /// <summary>Collects all nodes from a subtree into a flat list by walking children recursively.</summary>
        private static void CollectSubtreeNodes(ISystemContext context, NodeState root, List<NodeState> result)
        {
            result.Add(root);
            var children = new List<BaseInstanceState>();
            root.GetChildren(context, children);
            for (int i = 0; i < children.Count; i++)
                CollectSubtreeNodes(context, children[i], result);
        }

        /// <summary>Registers a subtree into the address space. Collects all nodes outside the lock,
        /// then does a fast flat dictionary insertion under the lock.</summary>
        private void FlushNodeBatch(ISystemContext context, NodeState subtreeRoot)
        {
            // Phase 1: Collect all nodes from the subtree (outside the lock — fully parallel)
            var allNodes = new List<NodeState>(4096);
            CollectSubtreeNodes(context, subtreeRoot, allNodes);

            // Phase 2: Register all nodes in the predefined dictionary (under lock — fast flat loop)
            var dict = PredefinedNodes;
            lock (Lock)
            {
                for (int i = 0; i < allNodes.Count; i++)
                    dict[allNodes[i].NodeId] = allNodes[i];
            }
        }

        private FolderState CreateFolder(Folder folder, BaseObjectState? parent, IList<IReference>? references, string pathPrefix, List<NodeState>? batch = null)
        {
            string currentPath = string.IsNullOrEmpty(pathPrefix) ? folder.Name : $"{pathPrefix}.{folder.Name}";

            var folderState = new FolderState(parent);
            folderState.NodeId = new NodeId(currentPath, _namespaceIndex);
            folderState.BrowseName = new QualifiedName(folder.Name, _namespaceIndex);
            folderState.DisplayName = new LocalizedText(folder.Name);
            folderState.TypeDefinitionId = ObjectTypeIds.FolderType;
            folderState.ReferenceTypeId = ReferenceTypes.Organizes;
            folderState.EventNotifier = EventNotifiers.SubscribeToEvents;

            if (parent != null)
            {
                RegisterChildNode(SystemContext, folderState, parent, batch);
            }
            else
            {
                _rootNodeIds.Add(folderState.NodeId); // Track root
                RegisterNode(SystemContext, folderState, batch);
                AddRootNotifier(folderState);

                // Ensure inverse reference exists
                folderState.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

                if (references != null)
                {
                    references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folderState.NodeId));
                }
            }

            // Root folder (parent == null) is transparent: children use empty prefix
            // so variable paths match user-facing convention (e.g. Process.Temperature)
            var childPrefix = parent != null ? currentPath : "";

            foreach(var subFolder in folder.Folders)
            {
                try
                {
                    CreateFolder(subFolder, folderState, references, childPrefix, batch);
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, $"Error creating subfolder '{subFolder.Name}' under '{currentPath}'");
                }
            }

            foreach(var variable in folder.Variables)
            {
                try
                {
                    CreateVariable(variable, folderState, childPrefix, batch);
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, $"Error creating variable '{variable.Name}' under '{currentPath}'");
                }
            }

            return folderState;
        }

        private void CreateVariable(Variable variable, BaseObjectState parent, string pathPrefix, List<NodeState>? batch = null)
        {
            string currentPath = string.IsNullOrEmpty(pathPrefix) ? variable.Name : $"{pathPrefix}.{variable.Name}";
            
            // Use ServerVariableState which handles logging + security
            // Pass this NodeManager to handle user lookups
            DataLoggingConfig? loggingConfig = variable.DataLogging != null && variable.DataLogging.Enabled ? variable.DataLogging : null;
            var variableState = new ServerVariableState(parent, _logger, loggingConfig, this);

            variableState.NodeId = new NodeId(currentPath, _namespaceIndex);
            variableState.BrowseName = new QualifiedName(variable.Name, _namespaceIndex);
            variableState.DisplayName = new LocalizedText(variable.Name);
            variableState.Value = ConvertValue(variable.Value, variable.Type); // Convert JsonElement
            // Apply initial value or retentive value
            if (variable.Retentive && _retentiveValues.TryGetValue(currentPath, out var retVal))
            {
                variableState.Value = ConvertValue(retVal, variable.Type);
            }
            else if (!string.IsNullOrEmpty(variable.InitialValue))
            {
                variableState.Value = ConvertValue(variable.InitialValue, variable.Type);
            }

            variableState.DataType = GetDataTypeId(variable.Type);
            variableState.ValueRank = ValueRanks.Scalar;
            variableState.Timestamp = DateTime.UtcNow;
            variableState.StatusCode = StatusCodes.Good;
            variableState.TypeDefinitionId = VariableTypeIds.BaseDataVariableType; // Explicitly set TypeDefinition

            // Add LastError property
            if (variable.DriverConfigs != null && variable.DriverConfigs.Count > 0)
            {
                var lastError = new PropertyState<string>(variableState);
                lastError.NodeId = new NodeId(currentPath + ".LastError", _namespaceIndex);
                lastError.BrowseName = new QualifiedName("LastError", _namespaceIndex);
                lastError.DisplayName = new LocalizedText("LastError");
                lastError.DataType = DataTypeIds.String;
                lastError.ValueRank = ValueRanks.Scalar;
                lastError.AccessLevel = AccessLevels.CurrentRead;
                lastError.UserAccessLevel = AccessLevels.CurrentRead;
                lastError.ReferenceTypeId = ReferenceTypeIds.HasProperty; // Ensure reference type is set
                
                variableState.AddChild(lastError);
                RegisterNode(SystemContext, lastError, batch);
            }

            byte accessLevel = AccessLevels.CurrentReadOrWrite;
            if (string.Equals(variable.Access, "Read", StringComparison.OrdinalIgnoreCase)) accessLevel = AccessLevels.CurrentRead;
            else if (string.Equals(variable.Access, "Write", StringComparison.OrdinalIgnoreCase)) accessLevel = AccessLevels.CurrentWrite;

            variableState.AccessLevel = accessLevel;
            // UserAccessLevel is determined dynamically in GetUserAccessLevel
            variableState.UserAccessLevel = accessLevel; 
            
            if (loggingConfig != null)
            {
                variableState.AccessLevel |= AccessLevels.HistoryRead;
                variableState.UserAccessLevel |= AccessLevels.HistoryRead; // Dynamic check allows this too if read allowed
                variableState.Historizing = true;
            }

            RegisterChildNode(SystemContext, variableState, parent, batch);
            Interlocked.Increment(ref _nodeCreationCount);

            // Add to dictionary for scripts
            // Use currentPath (NodeId identifier) as key
            _variables[currentPath] = variableState;

            // Notify scripts when the variable value changes (from drivers, OPC writes, etc.)
            var varPath = currentPath;
            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0)
                    _scriptManager?.NotifyVariableChanged(varPath, null, variableState.Value, DateTime.UtcNow);
            };

            // Configure drivers
            bool connectedToDriver = false;
            if (variable.DriverConfigs != null && variable.DriverConfigs.Count > 0)
            {
                foreach(var driver in _drivers)
                {
                    if (variable.DriverConfigs.TryGetValue(driver.Key, out var configJson))
                    {
                        if (!connectedToDriver)
                        {
                            variableState.StatusCode = StatusCodes.BadWaitingForInitialData;
                            connectedToDriver = true;
                        }
                        driver.AddItem(variableState, configJson.GetRawText());
                    }
                }
            }

            // Create alarm condition if AlarmConfig is specified
            if (variable.Alarm != null)
            {
                CreateAlarmCondition(variableState, variable.Alarm, currentPath, parent, batch);
            }

            // Register variable for anomaly detection
            if (variable.AnomalyDetection is { Enabled: true } && _anomalyDetectionService != null && _logger != null)
            {
                _anomalyDetectionService.Register(currentPath, variable.AnomalyDetection, variable.Alarm, _logger);
            }

            // Create statistics sub-variables if Statistics.Enabled
            if (variable.Statistics?.Enabled == true)
            {
                var statsFolder = new FolderState(variableState);
                statsFolder.NodeId = new NodeId(currentPath + ".Statistics", _namespaceIndex);
                statsFolder.BrowseName = new QualifiedName("Statistics", _namespaceIndex);
                statsFolder.DisplayName = new LocalizedText("Statistics");
                statsFolder.ReferenceTypeId = ReferenceTypeIds.HasComponent;

                var statsMin = new BaseDataVariableState<double>(statsFolder);
                statsMin.NodeId = new NodeId(currentPath + ".Statistics.Min", _namespaceIndex);
                statsMin.BrowseName = new QualifiedName("Min", _namespaceIndex);
                statsMin.DisplayName = new LocalizedText("Min");
                statsMin.DataType = DataTypeIds.Double;
                statsMin.ValueRank = ValueRanks.Scalar;
                statsMin.Value = 0.0;
                statsMin.AccessLevel = AccessLevels.CurrentRead;
                statsMin.UserAccessLevel = AccessLevels.CurrentRead;

                var statsMax = new BaseDataVariableState<double>(statsFolder);
                statsMax.NodeId = new NodeId(currentPath + ".Statistics.Max", _namespaceIndex);
                statsMax.BrowseName = new QualifiedName("Max", _namespaceIndex);
                statsMax.DisplayName = new LocalizedText("Max");
                statsMax.DataType = DataTypeIds.Double;
                statsMax.ValueRank = ValueRanks.Scalar;
                statsMax.Value = 0.0;
                statsMax.AccessLevel = AccessLevels.CurrentRead;
                statsMax.UserAccessLevel = AccessLevels.CurrentRead;

                var statsAvg = new BaseDataVariableState<double>(statsFolder);
                statsAvg.NodeId = new NodeId(currentPath + ".Statistics.Average", _namespaceIndex);
                statsAvg.BrowseName = new QualifiedName("Average", _namespaceIndex);
                statsAvg.DisplayName = new LocalizedText("Average");
                statsAvg.DataType = DataTypeIds.Double;
                statsAvg.ValueRank = ValueRanks.Scalar;
                statsAvg.Value = 0.0;
                statsAvg.AccessLevel = AccessLevels.CurrentRead;
                statsAvg.UserAccessLevel = AccessLevels.CurrentRead;

                var statsCount = new BaseDataVariableState<long>(statsFolder);
                statsCount.NodeId = new NodeId(currentPath + ".Statistics.Count", _namespaceIndex);
                statsCount.BrowseName = new QualifiedName("Count", _namespaceIndex);
                statsCount.DisplayName = new LocalizedText("Count");
                statsCount.DataType = DataTypeIds.Int64;
                statsCount.ValueRank = ValueRanks.Scalar;
                statsCount.Value = 0L;
                statsCount.AccessLevel = AccessLevels.CurrentRead;
                statsCount.UserAccessLevel = AccessLevels.CurrentRead;

                var statsReset = new ServerVariableState(statsFolder, null, null, this);
                statsReset.NodeId = new NodeId(currentPath + ".Statistics.Reset", _namespaceIndex);
                statsReset.BrowseName = new QualifiedName("Reset", _namespaceIndex);
                statsReset.DisplayName = new LocalizedText("Reset");
                statsReset.DataType = DataTypeIds.Boolean;
                statsReset.ValueRank = ValueRanks.Scalar;
                statsReset.Value = false;
                statsReset.AccessLevel = AccessLevels.CurrentReadOrWrite;
                statsReset.UserAccessLevel = AccessLevels.CurrentReadOrWrite;

                statsFolder.AddChild(statsMin);
                statsFolder.AddChild(statsMax);
                statsFolder.AddChild(statsAvg);
                statsFolder.AddChild(statsCount);
                statsFolder.AddChild(statsReset);
                variableState.AddChild(statsFolder);

                RegisterNode(SystemContext, statsFolder, batch);
                RegisterNode(SystemContext, statsMin, batch);
                RegisterNode(SystemContext, statsMax, batch);
                RegisterNode(SystemContext, statsAvg, batch);
                RegisterNode(SystemContext, statsCount, batch);
                RegisterNode(SystemContext, statsReset, batch);

                var tracker = new VariableStatisticsTracker(statsMin, statsMax, statsAvg, statsCount, statsReset, SystemContext);
                _statsTrackers[currentPath] = tracker;

                // Feed initial value to statistics
                if (variableState.Value != null && double.TryParse(
                    Convert.ToString(variableState.Value, System.Globalization.CultureInfo.InvariantCulture),
                    System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var initVal))
                {
                    tracker.Record(initVal);
                }

                // Track value changes for statistics
                var statsVarPath = currentPath;
                variableState.OnStateChanged += (ctx, state, masks) =>
                {
                    if ((masks & NodeStateChangeMasks.Value) != 0 && _statsTrackers.TryGetValue(statsVarPath, out var t))
                    {
                        if (state is BaseDataVariableState vs && vs.Value != null &&
                            double.TryParse(Convert.ToString(vs.Value, System.Globalization.CultureInfo.InvariantCulture),
                                System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dv))
                        {
                            t.Record(dv);
                        }
                    }
                };
            }

            // Track retentive variable value changes
            if (variable.Retentive)
            {
                var retPath = currentPath;
                variableState.OnStateChanged += (ctx, state, masks) =>
                {
                    if ((masks & NodeStateChangeMasks.Value) != 0 && state is BaseDataVariableState vs)
                    {
                        _retentiveValues[retPath] = Convert.ToString(vs.Value, System.Globalization.CultureInfo.InvariantCulture) ?? "";
                        ScheduleRetentiveSave();
                    }
                };
            }
        }

        private void CreateEventVariable(Variable variable, BaseObjectState parent, string pathPrefix, List<NodeState>? batch = null)
        {
            string currentPath = string.IsNullOrEmpty(pathPrefix) ? variable.Name : $"{pathPrefix}.{variable.Name}";
            
            // Use ServerVariableState which handles logging + security
            // Pass this NodeManager to handle user lookups
            DataLoggingConfig? loggingConfig = variable.DataLogging != null && variable.DataLogging.Enabled ? variable.DataLogging : null;
            var variableState = new ServerVariableState(parent, _logger, loggingConfig, this);

            variableState.NodeId = new NodeId(currentPath, _namespaceIndex);
            variableState.BrowseName = new QualifiedName(variable.Name, _namespaceIndex);
            variableState.DisplayName = new LocalizedText(variable.Name);
            variableState.Value = variable.Value;
            variableState.DataType = GetDataTypeId(variable.Type);
            variableState.ValueRank = ValueRanks.Scalar;
            
            byte accessLevel = AccessLevels.CurrentReadOrWrite;
            if (string.Equals(variable.Access, "Read", StringComparison.OrdinalIgnoreCase)) accessLevel = AccessLevels.CurrentRead;
            else if (string.Equals(variable.Access, "Write", StringComparison.OrdinalIgnoreCase)) accessLevel = AccessLevels.CurrentWrite;

            variableState.AccessLevel = accessLevel;
            // UserAccessLevel is determined dynamically in GetUserAccessLevel
            variableState.UserAccessLevel = accessLevel; 
            
            if (loggingConfig != null)
            {
                variableState.AccessLevel |= AccessLevels.HistoryRead;
                variableState.UserAccessLevel |= AccessLevels.HistoryRead; // Dynamic check allows this too if read allowed
                variableState.Historizing = true;
            }

            RegisterChildNode(SystemContext, variableState, parent, batch);

            // Add to dictionary for scripts
            _variables[currentPath] = variableState;

            // Notify scripts when the variable value changes (from drivers, OPC writes, etc.)
            var varPath = currentPath;
            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0)
                    _scriptManager?.NotifyVariableChanged(varPath, null, variableState.Value, DateTime.UtcNow);
            };

            // Configure drivers
            bool connectedToDriver = false;
            if (variable.DriverConfigs != null)
            {
                foreach(var driver in _drivers)
                {
                    if (variable.DriverConfigs.TryGetValue(driver.Key, out var configJson))
                    {
                        if (!connectedToDriver)
                        {
                            variableState.StatusCode = StatusCodes.BadWaitingForInitialData;
                            connectedToDriver = true;
                        }
                        driver.AddItem(variableState, configJson.GetRawText());
                    }
                }
            }
        }

        // removed GetGroupAccessLevel
        private byte GetGroupAccessLevel(string groupName) => 0; // Removed implementation to fix parser errors


        // Script Accessors
        /// <summary>
        /// Resolves a variable by name, trying the exact key first then prepending the root folder name.
        /// </summary>
        private bool TryResolveVariable(string variableName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out BaseDataVariableState? variable)
        {
            if (_variables.TryGetValue(variableName, out variable!))
                return true;
            // Fallback: user-facing paths omit the root folder name
            var rootName = _lastModel?.Folder?.Name;
            if (!string.IsNullOrEmpty(rootName) && _variables.TryGetValue(rootName + "." + variableName, out variable!))
                return true;
            variable = null;
            return false;
        }

        public object? ReadVariable(string variableName)
        {
            if (TryResolveVariable(variableName, out var variable))
            {
                return variable.Value;
            }
            throw new InvalidOperationException("Variable not found: " + variableName);
        }

        public async Task<string?> GenerateReportAsync(string reportName)
        {
            return _reportManager != null ? await _reportManager.GenerateReportAsync(reportName) : null;
        }

        public List<(DateTime Timestamp, double Value)>? ReadHistoricalValues(string variablePath, DateTime startTime, DateTime endTime, int maxPoints)
        {
            if (_logger == null) return null;
            try
            {
                var nodeId = variablePath;
                if (TryResolveVariable(variablePath, out var vs))
                    nodeId = vs.NodeId?.ToString() ?? variablePath;
                var dataValues = _logger.ReadHistory(nodeId, startTime, endTime);
                var result = new List<(DateTime, double)>();
                foreach (var dv in dataValues)
                {
                    if (dv?.Value != null && double.TryParse(dv.Value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
                        result.Add((dv.SourceTimestamp, d));
                    if (result.Count >= maxPoints) break;
                }
                return result.Count > 0 ? result : null;
            }
            catch { return null; }
        }

        public void WriteVariable(string variableName, object value)
        {
            if (TryResolveVariable(variableName, out var variable))
            {
                var oldValue = variable.Value;
                variable.Value = value;
                variable.Timestamp = DateTime.UtcNow;
                variable.ClearChangeMasks(SystemContext, false);
                _scriptManager?.NotifyVariableChanged(variableName, oldValue, value, DateTime.UtcNow);
            }
            else
                throw new InvalidOperationException("Variable not found: " + variableName);
        }

        private object? ConvertValue(object? value, string type)
        {
            if (value == null) return null;

            if (value is JsonElement element)
            {
                try
                {
                    // Handle numbers specifically
                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        return type switch
                        {
                            "Double" => element.GetDouble(),
                            "Int32" => element.GetInt32(),
                            "Float" => (float)element.GetDouble(),
                            "Int16" => element.GetInt16(),
                            "UInt16" => element.GetUInt16(),
                            "UInt32" => element.GetUInt32(),
                            _ => element.GetDouble() // Default for numbers
                        };
                    }
                    else if (element.ValueKind == JsonValueKind.String)
                    {
                        if (type == "DateTime") return element.GetDateTime();
                        return element.GetString();
                    }
                    else if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
                    {
                        return element.GetBoolean();
                    }
                }
                catch
                {
                    return null;
                }
            }
            // Handle plain string values (e.g. from InitialValue or retentive store)
            if (value is string str && !string.IsNullOrEmpty(type) && type != "String")
            {
                try
                {
                    return type switch
                    {
                        "Double" when double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) => d,
                        "Float" when float.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var f) => f,
                        "Int32" when int.TryParse(str, out var i32) => i32,
                        "Int16" when short.TryParse(str, out var i16) => i16,
                        "UInt16" when ushort.TryParse(str, out var u16) => u16,
                        "UInt32" when uint.TryParse(str, out var u32) => u32,
                        "Boolean" when bool.TryParse(str, out var b) => b,
                        "DateTime" when DateTime.TryParse(str, out var dt) => dt,
                        _ => value
                    };
                }
                catch { return value; }
            }

            return value;
        }

        private NodeId GetDataTypeId(string type)
        {
            return type switch
            {
                "Double" => DataTypeIds.Double,
                "Int32" => DataTypeIds.Int32,
                "String" => DataTypeIds.String,
                "Boolean" => DataTypeIds.Boolean,
                "DateTime" => DataTypeIds.DateTime,
                "Float" => DataTypeIds.Float,
                "Int16" => DataTypeIds.Int16,
                "UInt16" => DataTypeIds.UInt16,
                "UInt32" => DataTypeIds.UInt32,
                _ => DataTypeIds.BaseDataType
            };
        }

        // ─── Alarm & Condition support ──────────────────────────────────────

        /// <summary>Tracks state for each alarm condition instance.</summary>
        private class AlarmConditionInfo
        {
            public required AlarmConditionState AlarmState { get; init; }
            public required AlarmConfig Config { get; init; }
            public required BaseDataVariableState SourceVariable { get; init; }
            public required string VariablePath { get; init; }
            public bool IsActive { get; set; }
        }

        // ─── Alarm analytics helpers ─────────────────────────────────────

        private enum AlarmAnalyticsEventKind { Activated, Acknowledged }

        private sealed class AlarmAnalyticsEvent
        {
            public DateTime TimeUtc { get; init; }
            public AlarmAnalyticsEventKind Kind { get; init; }
            public string VariablePath { get; init; } = "";
            public string Message { get; init; } = "";
        }

        private void RecordAlarmAnalyticsEvent(AlarmAnalyticsEventKind kind, string variablePath, string message)
        {
            lock (_analyticsLock)
            {
                _alarmAnalyticsLog.Add(new AlarmAnalyticsEvent
                {
                    TimeUtc = DateTime.UtcNow,
                    Kind = kind,
                    VariablePath = variablePath,
                    Message = message
                });

                // Trim oldest events when buffer exceeds max
                if (_alarmAnalyticsLog.Count > AlarmAnalyticsMaxEvents)
                    _alarmAnalyticsLog.RemoveRange(0, _alarmAnalyticsLog.Count - AlarmAnalyticsMaxEvents);
            }
        }

        /// <summary>Build the alarm analytics snapshot from the in-memory ring buffer.</summary>
        internal AlarmAnalyticsSnapshot BuildAlarmAnalytics()
        {
            lock (_analyticsLock)
            {
                var cutoff = DateTime.UtcNow.AddMinutes(-AlarmAnalyticsWindowMinutes);
                var windowEvents = _alarmAnalyticsLog.Where(e => e.TimeUtc >= cutoff).ToList();

                var activations = windowEvents.Where(e => e.Kind == AlarmAnalyticsEventKind.Activated).ToList();
                var acks = windowEvents.Where(e => e.Kind == AlarmAnalyticsEventKind.Acknowledged).ToList();

                // Top-10 most frequent alarm sources
                var topAlarms = activations
                    .GroupBy(e => e.VariablePath)
                    .Select(g =>
                    {
                        // Compute per-source MTTA: for each ack, find the closest preceding activation
                        var sourceActs = activations.Where(a => a.VariablePath == g.Key).OrderBy(a => a.TimeUtc).ToList();
                        var sourceAcks = acks.Where(a => a.VariablePath == g.Key).OrderBy(a => a.TimeUtc).ToList();
                        double? avgAckSec = null;
                        if (sourceAcks.Count > 0 && sourceActs.Count > 0)
                        {
                            var deltas = new List<double>();
                            int actIdx = 0;
                            foreach (var ack in sourceAcks)
                            {
                                // Find latest activation before this ack
                                while (actIdx + 1 < sourceActs.Count && sourceActs[actIdx + 1].TimeUtc <= ack.TimeUtc)
                                    actIdx++;
                                if (sourceActs[actIdx].TimeUtc <= ack.TimeUtc)
                                    deltas.Add((ack.TimeUtc - sourceActs[actIdx].TimeUtc).TotalSeconds);
                            }
                            if (deltas.Count > 0)
                                avgAckSec = Math.Round(deltas.Average(), 1);
                        }

                        return new AlarmFrequencyEntry
                        {
                            VariablePath = g.Key,
                            Message = g.First().Message,
                            ActivationCount = g.Count(),
                            AvgAcknowledgeSeconds = avgAckSec
                        };
                    })
                    .OrderByDescending(e => e.ActivationCount)
                    .Take(10)
                    .ToList();

                // Global MTTA
                double? globalMtta = null;
                if (acks.Count > 0 && activations.Count > 0)
                {
                    var allActs = activations.OrderBy(a => a.TimeUtc).ToList();
                    var deltas = new List<double>();
                    foreach (var group in acks.GroupBy(a => a.VariablePath))
                    {
                        var srcActs = allActs.Where(a => a.VariablePath == group.Key).ToList();
                        if (srcActs.Count == 0) continue;
                        int ai = 0;
                        foreach (var ack in group.OrderBy(a => a.TimeUtc))
                        {
                            while (ai + 1 < srcActs.Count && srcActs[ai + 1].TimeUtc <= ack.TimeUtc)
                                ai++;
                            if (srcActs[ai].TimeUtc <= ack.TimeUtc)
                                deltas.Add((ack.TimeUtc - srcActs[ai].TimeUtc).TotalSeconds);
                        }
                    }
                    if (deltas.Count > 0)
                        globalMtta = Math.Round(deltas.Average(), 1);
                }

                // Flood detection — count activations in the last N seconds
                var floodCutoff = DateTime.UtcNow.AddSeconds(-AlarmFloodWindowSeconds);
                int floodCount = activations.Count(e => e.TimeUtc >= floodCutoff);

                return new AlarmAnalyticsSnapshot
                {
                    Timestamp = DateTime.UtcNow,
                    TotalActivations = activations.Count,
                    TotalAcknowledgements = acks.Count,
                    TopAlarms = topAlarms,
                    MeanTimeToAcknowledgeSeconds = globalMtta,
                    IsFloodDetected = floodCount >= AlarmFloodThreshold,
                    FloodWindowActivations = floodCount,
                    FloodThreshold = AlarmFloodThreshold,
                    FloodWindowSeconds = AlarmFloodWindowSeconds,
                    WindowMinutes = AlarmAnalyticsWindowMinutes
                };
            }
        }

        /// <summary>Build system-level statistics for the diagnostics snapshot.</summary>
        private SystemStats BuildSystemStats()
        {
            var stats = new SystemStats
            {
                ActiveAlarmCount = _alarmConditions.Values.Count(a => a.IsActive),
                TotalAlarmActivations = _alarmAnalyticsLog.Count(e => e.Kind == AlarmAnalyticsEventKind.Activated),
                TotalAlarmAcknowledgements = _alarmAnalyticsLog.Count(e => e.Kind == AlarmAnalyticsEventKind.Acknowledged)
            };

            // Driver stats
            foreach (var d in _drivers)
            {
                stats.Drivers.Add(new DriverSystemStats
                {
                    Name = d.Key,
                    Status = "Running"
                });
            }

            return stats;
        }

        /// <summary>Assigns unique NodeIds to all children of a cloned alarm template.
        /// Children get NodeIds like "prefix.ChildBrowseName" in our namespace.</summary>
        private void AssignAlarmChildNodeIds(NodeState alarm, string prefix)
        {
            var children = new List<BaseInstanceState>();
            alarm.GetChildren(SystemContext, children);
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.NodeId = new NodeId(prefix + "." + child.SymbolicName, _namespaceIndex);
                // Recurse into grandchildren (e.g. LimitState.CurrentState, EnabledState.Id, etc.)
                AssignAlarmChildNodeIds(child, prefix + "." + child.SymbolicName);
            }
        }

        private void CreateAlarmCondition(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent, List<NodeState>? batch = null)
        {
            if (alarmConfig.TriggerType == AlarmTriggerType.Condition)
                CreateConditionAlarm(variableState, alarmConfig, variablePath, parent, batch);
            else
                CreateLimitAlarm(variableState, alarmConfig, variablePath, parent, batch);
        }

        private void CreateLimitAlarm(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent, List<NodeState>? batch = null)
        {
            var alarmNodeId = new NodeId(variablePath + ".Alarm", _namespaceIndex);

            // Clone from a pre-built template to avoid the very expensive
            // base64-decode → binary-deserialize → 7-recursive-walk path in Create().
            // The template is [ThreadStatic] so there is zero lock contention.
            if (t_limitAlarmTemplate == null)
            {
                t_limitAlarmTemplate = new ExclusiveLimitAlarmState(null);
                t_limitAlarmTemplate.Create(SystemContext, null, null, null, false);
            }

            var alarm = (ExclusiveLimitAlarmState)t_limitAlarmTemplate.Clone();

            // Assign unique NodeIds to the alarm and all its children.
            alarm.NodeId = alarmNodeId;
            var prefix = variablePath + ".Alarm";
            AssignAlarmChildNodeIds(alarm, prefix);

            // Set identity
            alarm.BrowseName = new QualifiedName(variableState.BrowseName.Name + "Alarm", _namespaceIndex);
            alarm.DisplayName = new LocalizedText(variableState.DisplayName.Text + " Alarm");
            alarm.TypeDefinitionId = ObjectTypeIds.ExclusiveLimitAlarmType;
            alarm.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // Ensure BranchId is initialized — required by ConditionState.IsBranch().
            // Some SDK versions do not auto-create this property in Create().
            if (alarm.BranchId == null)
            {
                alarm.BranchId = new PropertyState<NodeId>(alarm)
                {
                    NodeId = new NodeId(variablePath + ".Alarm.BranchId", _namespaceIndex),
                    BrowseName = BrowseNames.BranchId,
                    DisplayName = BrowseNames.BranchId,
                    DataType = DataTypeIds.NodeId,
                    ValueRank = ValueRanks.Scalar,
                    Value = NodeId.Null,
                    AccessLevel = AccessLevels.CurrentRead,
                    UserAccessLevel = AccessLevels.CurrentRead,
                    ReferenceTypeId = ReferenceTypeIds.HasProperty,
                    TypeDefinitionId = VariableTypeIds.PropertyType
                };
                alarm.AddChild(alarm.BranchId);
            }
            else if (alarm.BranchId.Value == null)
            {
                alarm.BranchId.Value = NodeId.Null;
            }

            // Source & condition references
            alarm.SourceNode.Value = variableState.NodeId;
            alarm.SourceName.Value = variablePath;
            alarm.ConditionName.Value = alarmConfig.Message;

            // Set limit values on the properties already created by Create().
            // Do NOT replace the PropertyState objects — they have NodeIds wired up.
            if (alarm.HighHighLimit != null)
                alarm.HighHighLimit.Value = alarmConfig.HighHighLimit ?? alarmConfig.HighLimit;
            if (alarm.HighLimit != null)
                alarm.HighLimit.Value = alarmConfig.HighLimit;
            if (alarm.LowLimit != null)
                alarm.LowLimit.Value = alarmConfig.LowLimit;
            if (alarm.LowLowLimit != null)
                alarm.LowLowLimit.Value = alarmConfig.LowLowLimit ?? alarmConfig.LowLimit;

            // Initial state — set directly on the property values to avoid
            // the SetEnableState/SetActiveState call chain that can throw in some SDK versions.
            alarm.EnabledState.Value = new LocalizedText("en", "Enabled");
            alarm.EnabledState.Id.Value = true;
            alarm.EnabledState.TransitionTime.Value = DateTime.UtcNow;

            alarm.ActiveState.Value = new LocalizedText("en", "Inactive");
            alarm.ActiveState.Id.Value = false;
            alarm.ActiveState.TransitionTime.Value = DateTime.UtcNow;

            if (alarm.AckedState != null)
            {
                alarm.AckedState.Value = new LocalizedText("en", "Acknowledged");
                alarm.AckedState.Id.Value = true;
            }

            if (alarm.ConfirmedState != null)
            {
                alarm.ConfirmedState.Value = new LocalizedText("en", "Confirmed");
                alarm.ConfirmedState.Id.Value = true;
            }

            if (alarm.SuppressedState != null)
            {
                alarm.SuppressedState.Value = new LocalizedText("en", "Unsuppressed");
                alarm.SuppressedState.Id.Value = false;
            }

            alarm.Retain.Value = false;
            alarm.AutoReportStateChanges = true;

            // Severity: 500 = medium by default
            alarm.Severity.Value = 500;

            alarm.Message.Value = new LocalizedText(alarmConfig.Message);
            alarm.Time.Value = DateTime.UtcNow;

            // Wire up Acknowledge handler
            alarm.OnAcknowledge = OnAlarmAcknowledge;
            alarm.OnConfirm = OnAlarmConfirm;

            // Add to parent and address space
            RegisterChildNode(SystemContext, alarm, parent, batch);

            // Also add a HasCondition reference from the source variable to the alarm
            variableState.AddReference(ReferenceTypeIds.HasCondition, false, alarm.NodeId);
            alarm.AddReference(ReferenceTypeIds.HasCondition, true, variableState.NodeId);

            var info = new AlarmConditionInfo
            {
                AlarmState = alarm,
                Config = alarmConfig,
                SourceVariable = variableState,
                VariablePath = variablePath,
                IsActive = false
            };
            _alarmConditions[variablePath] = info;

            // Monitor value changes to activate/deactivate the alarm
            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0 && !_loading)
                    EvaluateAlarmCondition(info);
            };

            // Initial evaluation is deferred — performed in bulk after all nodes are created.
        }

        private void CreateConditionAlarm(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent, List<NodeState>? batch = null)
        {
            var alarmNodeId = new NodeId(variablePath + ".Alarm", _namespaceIndex);

            // Clone from a pre-built template (same pattern as CreateLimitAlarm).
            if (t_conditionAlarmTemplate == null)
            {
                t_conditionAlarmTemplate = new OffNormalAlarmState(null);
                t_conditionAlarmTemplate.Create(SystemContext, null, null, null, false);
            }

            var alarm = (OffNormalAlarmState)t_conditionAlarmTemplate.Clone();

            alarm.NodeId = alarmNodeId;
            var prefix = variablePath + ".Alarm";
            AssignAlarmChildNodeIds(alarm, prefix);

            alarm.BrowseName = new QualifiedName(variableState.BrowseName.Name + "Alarm", _namespaceIndex);
            alarm.DisplayName = new LocalizedText(variableState.DisplayName.Text + " Alarm");
            alarm.TypeDefinitionId = ObjectTypeIds.OffNormalAlarmType;
            alarm.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // Ensure BranchId is initialized — required by ConditionState.IsBranch()
            if (alarm.BranchId == null)
            {
                alarm.BranchId = new PropertyState<NodeId>(alarm)
                {
                    NodeId = new NodeId(variablePath + ".Alarm.BranchId", _namespaceIndex),
                    BrowseName = BrowseNames.BranchId,
                    DisplayName = BrowseNames.BranchId,
                    DataType = DataTypeIds.NodeId,
                    ValueRank = ValueRanks.Scalar,
                    Value = NodeId.Null,
                    AccessLevel = AccessLevels.CurrentRead,
                    UserAccessLevel = AccessLevels.CurrentRead,
                    ReferenceTypeId = ReferenceTypeIds.HasProperty,
                    TypeDefinitionId = VariableTypeIds.PropertyType
                };
                alarm.AddChild(alarm.BranchId);
            }
            else if (alarm.BranchId.Value == null)
            {
                alarm.BranchId.Value = NodeId.Null;
            }

            alarm.SourceNode.Value = variableState.NodeId;
            alarm.SourceName.Value = variablePath;
            alarm.ConditionName.Value = alarmConfig.Message;

            // Initial state — set directly on property values to avoid SDK internal NRE
            alarm.EnabledState.Value = new LocalizedText("en", "Enabled");
            alarm.EnabledState.Id.Value = true;
            alarm.EnabledState.TransitionTime.Value = DateTime.UtcNow;

            alarm.ActiveState.Value = new LocalizedText("en", "Inactive");
            alarm.ActiveState.Id.Value = false;
            alarm.ActiveState.TransitionTime.Value = DateTime.UtcNow;

            if (alarm.AckedState != null)
            {
                alarm.AckedState.Value = new LocalizedText("en", "Acknowledged");
                alarm.AckedState.Id.Value = true;
            }

            if (alarm.ConfirmedState != null)
            {
                alarm.ConfirmedState.Value = new LocalizedText("en", "Confirmed");
                alarm.ConfirmedState.Id.Value = true;
            }

            if (alarm.SuppressedState != null)
            {
                alarm.SuppressedState.Value = new LocalizedText("en", "Unsuppressed");
                alarm.SuppressedState.Id.Value = false;
            }

            alarm.Retain.Value = false;
            alarm.AutoReportStateChanges = true;

            alarm.Severity.Value = alarmConfig.ConditionSeverity;
            alarm.Message.Value = new LocalizedText(alarmConfig.Message);
            alarm.Time.Value = DateTime.UtcNow;

            alarm.OnAcknowledge = OnAlarmAcknowledge;
            alarm.OnConfirm = OnAlarmConfirm;

            RegisterChildNode(SystemContext, alarm, parent, batch);

            variableState.AddReference(ReferenceTypeIds.HasCondition, false, alarm.NodeId);
            alarm.AddReference(ReferenceTypeIds.HasCondition, true, variableState.NodeId);

            var info = new AlarmConditionInfo
            {
                AlarmState = alarm,
                Config = alarmConfig,
                SourceVariable = variableState,
                VariablePath = variablePath,
                IsActive = false
            };
            _alarmConditions[variablePath] = info;

            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0 && !_loading)
                    EvaluateAlarmCondition(info);
            };

            // Initial evaluation is deferred — performed in bulk after all nodes are created.
        }

        private void EvaluateAlarmCondition(AlarmConditionInfo info)
        {
            try
            {
                if (info.Config.TriggerType == AlarmTriggerType.Condition)
                    EvaluateConditionAlarm(info);
                else
                    EvaluateLimitAlarm(info);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Alarm evaluation error for {Path}", info.VariablePath);
            }
        }

        private void EvaluateLimitAlarm(AlarmConditionInfo info)
        {
            var result = AlarmEvaluator.EvaluateLimitAlarm(info.SourceVariable.Value, info.Config, info.IsActive);
            if (result == null) return;

            double val = AlarmEvaluator.GetNumericValue(info.SourceVariable.Value)!.Value;
            var cfg = info.Config;
            double highHigh = cfg.HighHighLimit ?? cfg.HighLimit;
            double lowLow = cfg.LowLowLimit ?? cfg.LowLimit;
            double hyst = cfg.Hysteresis;

            bool shouldActivate = result.ShouldActivate;
            bool shouldDeactivate = result.ShouldDeactivate;

            if (shouldActivate)
            {
                info.IsActive = true;
                var alarm = info.AlarmState;

                ushort severity = result.Severity;
                LimitAlarmStates limitState = result.LimitState switch
                {
                    "HighHigh" => LimitAlarmStates.HighHigh,
                    "High" => LimitAlarmStates.High,
                    "LowLow" => LimitAlarmStates.LowLow,
                    _ => LimitAlarmStates.Low
                };
                string limitText = result.LimitText;

                string message = string.IsNullOrEmpty(cfg.Message)
                    ? $"{info.VariablePath}: {limitText} — value={val:G6}"
                    : $"{cfg.Message} — {limitText} — value={val:G6}";

                alarm.SetActiveState(SystemContext, true);
                alarm.SetAcknowledgedState(SystemContext, false);
                alarm.SetConfirmedState(SystemContext, false);

                alarm.Severity.Value = severity;
                alarm.Message.Value = new LocalizedText(message);
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = DateTime.UtcNow;

                if (alarm is ExclusiveLimitAlarmState limitAlarm)
                    limitAlarm.SetLimitState(SystemContext, limitState);

                alarm.Retain.Value = true;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.ExclusiveLimitAlarmType;

                ReportAlarmEvent(alarm);
                _eventLogger?.LogAlarm(severity >= 800 ? "Critical" : "Warning", info.VariablePath, message,
                    $"Value={val:G6} HH={highHigh} H={cfg.HighLimit} L={cfg.LowLimit} LL={lowLow} Hyst={hyst}");
                RecordAlarmAnalyticsEvent(AlarmAnalyticsEventKind.Activated, info.VariablePath, message);

                if (cfg.NotifyOnActivation)
                    _notificationService?.NotifyAlarmActivated(info.VariablePath, message, severity);
            }
            else if (shouldDeactivate)
            {
                info.IsActive = false;
                var alarm = info.AlarmState;

                alarm.SetActiveState(SystemContext, false);

                alarm.Severity.Value = 1;
                alarm.Message.Value = new LocalizedText($"{info.VariablePath}: Returned to normal — value={val:G6}");
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = DateTime.UtcNow;

                bool fullyCleared = alarm.AckedState.Id.Value && alarm.ConfirmedState.Id.Value;
                alarm.Retain.Value = !fullyCleared;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.ExclusiveLimitAlarmType;

                ReportAlarmEvent(alarm);
                _eventLogger?.LogAlarm("Info", info.VariablePath, $"Returned to normal — value={val:G6}");
            }
        }

        private void EvaluateConditionAlarm(AlarmConditionInfo info)
        {
            var value = info.SourceVariable.Value;
            var cfg = info.Config;
            bool shouldActivate = AlarmEvaluator.EvaluateConditionActivation(cfg.Operator, cfg.CompareValue, value);

            // Determine deactivation with hysteresis for numeric operators
            bool shouldDeactivate;
            if (info.IsActive && cfg.Hysteresis > 0 && (cfg.Operator is ">" or ">=" or "<" or "<=" or "==" or "!="))
            {
                // Apply hysteresis: use the inverse condition shifted by hysteresis
                shouldDeactivate = AlarmEvaluator.EvaluateConditionDeactivation(cfg.Operator, cfg.CompareValue, cfg.Hysteresis, value);
            }
            else
            {
                shouldDeactivate = !shouldActivate;
            }

            if (shouldActivate && !info.IsActive)
            {
                info.IsActive = true;
                var alarm = info.AlarmState;

                string valueStr = value?.ToString() ?? "null";
                string conditionText = $"{cfg.Operator} {cfg.CompareValue}".Trim();
                string message = string.IsNullOrEmpty(cfg.Message)
                    ? $"{info.VariablePath}: Condition met ({conditionText}) — value={valueStr}"
                    : $"{cfg.Message} — Condition ({conditionText}) — value={valueStr}";

                alarm.SetActiveState(SystemContext, true);
                alarm.SetAcknowledgedState(SystemContext, false);
                alarm.SetConfirmedState(SystemContext, false);

                alarm.Severity.Value = cfg.ConditionSeverity;
                alarm.Message.Value = new LocalizedText(message);
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = DateTime.UtcNow;
                alarm.Retain.Value = true;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.OffNormalAlarmType;

                ReportAlarmEvent(alarm);
                _eventLogger?.LogAlarm(cfg.ConditionSeverity >= 800 ? "Critical" : "Warning",
                    info.VariablePath, message, $"Operator={cfg.Operator} Compare={cfg.CompareValue} Value={valueStr} Hyst={cfg.Hysteresis}");
                RecordAlarmAnalyticsEvent(AlarmAnalyticsEventKind.Activated, info.VariablePath, message);

                if (cfg.NotifyOnActivation)
                    _notificationService?.NotifyAlarmActivated(info.VariablePath, message, cfg.ConditionSeverity);
            }
            else if (shouldDeactivate)
            {
                info.IsActive = false;
                var alarm = info.AlarmState;

                string valueStr = value?.ToString() ?? "null";
                alarm.SetActiveState(SystemContext, false);

                alarm.Severity.Value = 1;
                alarm.Message.Value = new LocalizedText($"{info.VariablePath}: Condition cleared — value={valueStr}");
                alarm.Time.Value = DateTime.UtcNow;
                alarm.ReceiveTime.Value = DateTime.UtcNow;

                bool fullyCleared = alarm.AckedState.Id.Value && alarm.ConfirmedState.Id.Value;
                alarm.Retain.Value = !fullyCleared;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.OffNormalAlarmType;

                ReportAlarmEvent(alarm);
                _eventLogger?.LogAlarm("Info", info.VariablePath, $"Condition cleared — value={valueStr}");
            }
        }

        private void ReportAlarmEvent(AlarmConditionState alarm)
        {
            try
            {
                var e = new InstanceStateSnapshot();
                e.Initialize(SystemContext, alarm);
                // Ensure ConditionType.NodeId is included in the event so clients
                // can identify which condition instance raised the event.
                e.SetChildValue(BrowseNames.NodeId, NodeClass.Variable, alarm.NodeId);
                alarm.ReportEvent(SystemContext, e);

                // Also report on the Server object so subscribers to Server events see it
                Server?.ReportEvent(e);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Error reporting alarm event for {0}", alarm.NodeId);
            }
        }

        private ServiceResult OnAlarmAcknowledge(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            if (condition is AcknowledgeableConditionState ackCondition)
            {
                ackCondition.SetAcknowledgedState(context, true);
                if (comment != null && !string.IsNullOrEmpty(comment.Text))
                    ackCondition.Comment.Value = comment;

                ackCondition.Message.Value = new LocalizedText($"Acknowledged: {ackCondition.ConditionName.Value}");
                ackCondition.Time.Value = DateTime.UtcNow;

                // Clear retain if both acked and confirmed AND inactive
                bool isActive = (ackCondition as AlarmConditionState)?.ActiveState?.Id?.Value == true;
                bool isConfirmed = ackCondition.ConfirmedState?.Id?.Value == true;
                if (!isActive && isConfirmed)
                    ackCondition.Retain.Value = false;

                // Report the state change
                ackCondition.EventId.Value = Guid.NewGuid().ToByteArray();
                if (ackCondition is AlarmConditionState alarmState)
                    ReportAlarmEvent(alarmState);

                _eventLogger?.LogAlarm("Info", ackCondition.ConditionName.Value ?? "Alarm",
                    $"Alarm acknowledged: {ackCondition.ConditionName.Value}",
                    comment?.Text);

                var ackPath = _alarmConditions.FirstOrDefault(kv => kv.Value.AlarmState == ackCondition as AlarmConditionState).Key
                    ?? ackCondition.ConditionName.Value ?? "Alarm";
                RecordAlarmAnalyticsEvent(AlarmAnalyticsEventKind.Acknowledged, ackPath, $"Acknowledged: {ackCondition.ConditionName.Value}");
            }

            return ServiceResult.Good;
        }

        private ServiceResult OnAlarmConfirm(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            if (condition is AcknowledgeableConditionState ackCondition)
            {
                ackCondition.SetConfirmedState(context, true);
                if (comment != null && !string.IsNullOrEmpty(comment.Text))
                    ackCondition.Comment.Value = comment;

                ackCondition.Message.Value = new LocalizedText($"Confirmed: {ackCondition.ConditionName.Value}");
                ackCondition.Time.Value = DateTime.UtcNow;

                // Clear retain if both acked and confirmed AND inactive
                bool isActive = (ackCondition as AlarmConditionState)?.ActiveState?.Id?.Value == true;
                bool isAcked = ackCondition.AckedState?.Id?.Value == true;
                if (!isActive && isAcked)
                    ackCondition.Retain.Value = false;

                ackCondition.EventId.Value = Guid.NewGuid().ToByteArray();
                if (ackCondition is AlarmConditionState alarmState)
                    ReportAlarmEvent(alarmState);

                _eventLogger?.LogAlarm("Info", ackCondition.ConditionName.Value ?? "Alarm",
                    $"Alarm confirmed: {ackCondition.ConditionName.Value}",
                    comment?.Text);
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Override ConditionRefresh to report retained alarm conditions to the subscriber.
        /// </summary>
        public override ServiceResult ConditionRefresh(
            OperationContext context,
            IList<IEventMonitoredItem> monitoredItems)
        {
            foreach (var item in monitoredItems)
            {
                foreach (var info in _alarmConditions.Values)
                {
                    if (info.AlarmState.Retain.Value)
                    {
                        var e = new InstanceStateSnapshot();
                        e.Initialize(SystemContext, info.AlarmState);
                        e.SetChildValue(BrowseNames.NodeId, NodeClass.Variable, info.AlarmState.NodeId);
                        item.QueueEvent(e);
                    }
                }
            }
            return ServiceResult.Good;
        }

        private void OnDemoCheckTimer(object? state)
        {
            if (SharedModels.LicenseManager.CheckDemoExpiry())
            {
                Log.Warning("Demo period expired — degrading to Trial mode with limited features.");
                _eventLogger?.LogSystem("Warning", "License",
                    "Demo period expired. Running in Trial mode. Add a license file to restore full functionality.");

                // Re-enforce limits on the loaded model
                var lic = SharedModels.LicenseManager.Current.License;
                if (lic != null && _lastModel != null)
                {
                    lock (Lock)
                    {
                        EnforceLicenseLimits(_lastModel, lic);
                    }
                }

                // Stop checking — degradation is permanent until restart with a license file
                _demoCheckTimer?.Dispose();
                _demoCheckTimer = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _eventLogger?.LogSystem("Info", "Server", $"Server shutting down at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                foreach(var d in _drivers) d.Dispose();
                _logger?.Dispose();
                _scriptManager?.Dispose();
                _plcManager?.Dispose();
                _recipeManager?.Dispose();
                _schedulerManager?.Dispose();
                _reportManager?.Dispose();
                _assetManager?.Dispose();
                _notificationService?.Dispose();
                _batchManager?.Dispose();
                _eventManager?.Dispose();
                _anomalyDetectionService?.Dispose();
                _sparkplugPublisher?.Dispose();
                _redundancy?.Dispose();
                _eventLogger?.Dispose();
                foreach (var rw in _resourceWatchers) rw.Dispose();
                _resourceWatchers.Clear();
                _demoCheckTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private static string FormatTimeAgo(TimeSpan ts)
        {
            if (ts.TotalDays >= 1)
                return $"{(int)ts.TotalDays}d {ts.Hours}h {ts.Minutes}m";
            if (ts.TotalHours >= 1)
                return $"{ts.Hours}h {ts.Minutes}m";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}m {ts.Seconds}s";
            return $"{ts.Seconds}s";
        }

        private void EnforceLicenseLimits(NodeModel model, SharedModels.License lic)
        {
            var varCount = CountVariablesInFolder(model.Folder);
            if (lic.MaxVariables > 0 && varCount > lic.MaxVariables)
            {
                Serilog.Log.Warning("License limit: {Count} variables exceeds limit of {Max} ({Tier}). Extra variables will not be created.",
                    varCount, lic.MaxVariables, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Variable limit exceeded: {varCount}/{lic.MaxVariables} ({lic.Tier})");
                TrimVariables(model.Folder, lic.MaxVariables, 0);
            }

            if (lic.MaxDrivers > 0 && _drivers.Count > lic.MaxDrivers)
            {
                Serilog.Log.Warning("License limit: {Count} drivers exceeds limit of {Max} ({Tier}). Extra drivers unloaded.",
                    _drivers.Count, lic.MaxDrivers, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Driver limit exceeded: {_drivers.Count}/{lic.MaxDrivers} ({lic.Tier})");
                while (_drivers.Count > lic.MaxDrivers)
                {
                    var d = _drivers[^1];
                    d.Dispose();
                    _drivers.RemoveAt(_drivers.Count - 1);
                }
            }

            if (lic.MaxScripts > 0 && model.Scripts.Count > lic.MaxScripts)
            {
                Serilog.Log.Warning("License limit: {Count} scripts exceeds limit of {Max} ({Tier}). Extra scripts disabled.",
                    model.Scripts.Count, lic.MaxScripts, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Script limit exceeded: {model.Scripts.Count}/{lic.MaxScripts} ({lic.Tier})");
                for (int i = lic.MaxScripts; i < model.Scripts.Count; i++)
                    model.Scripts[i].Enabled = false;
            }

            if (lic.MaxPlcPrograms > 0 && model.PlcPrograms.Count > lic.MaxPlcPrograms)
            {
                Serilog.Log.Warning("License limit: {Count} PLC programs exceeds limit of {Max} ({Tier}). Extra programs disabled.",
                    model.PlcPrograms.Count, lic.MaxPlcPrograms, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"PLC program limit exceeded: {model.PlcPrograms.Count}/{lic.MaxPlcPrograms} ({lic.Tier})");
                for (int i = lic.MaxPlcPrograms; i < model.PlcPrograms.Count; i++)
                    model.PlcPrograms[i].Enabled = false;
            }

            if (lic.MaxScreens > 0 && model.Screens.Count > lic.MaxScreens)
            {
                Serilog.Log.Warning("License limit: {Count} screens exceeds limit of {Max} ({Tier}).",
                    model.Screens.Count, lic.MaxScreens, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Screen limit exceeded: {model.Screens.Count}/{lic.MaxScreens} ({lic.Tier})");
            }

            if (lic.MaxRecipes > 0 && model.Recipes.Count > lic.MaxRecipes)
            {
                Serilog.Log.Warning("License limit: {Count} recipes exceeds limit of {Max} ({Tier}). Extra recipes disabled.",
                    model.Recipes.Count, lic.MaxRecipes, lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Recipe limit exceeded: {model.Recipes.Count}/{lic.MaxRecipes} ({lic.Tier})");
                for (int i = lic.MaxRecipes; i < model.Recipes.Count; i++)
                    model.Recipes[i].Enabled = false;
            }

            if (!lic.AllowDataLogging && model.Database != null)
            {
                Serilog.Log.Warning("License limit: Data logging not allowed in {Tier} tier. Database disabled.", lic.Tier);
                _eventLogger?.LogSystem("Warning", "License", $"Data logging not allowed in {lic.Tier} tier — disabled");
                model.Database = null;
            }
        }

        private static int CountVariablesInFolder(Folder folder)
        {
            int count = folder.Variables.Count;
            foreach (var sub in folder.Folders)
                count += CountVariablesInFolder(sub);
            return count;
        }

        private static int TrimVariables(Folder folder, int max, int currentCount)
        {
            var toRemove = new List<Variable>();
            foreach (var v in folder.Variables)
            {
                if (currentCount >= max)
                    toRemove.Add(v);
                else
                    currentCount++;
            }
            foreach (var v in toRemove)
                folder.Variables.Remove(v);

            foreach (var sub in folder.Folders)
                currentCount = TrimVariables(sub, max, currentCount);
            return currentCount;
        }

        public override void HistoryRead(
            OperationContext context,
            HistoryReadDetails details,
            TimestampsToReturn timestampsToReturn,
            bool releaseContinuationPoints,
            IList<HistoryReadValueId> nodesToRead,
            IList<HistoryReadResult> results,
            IList<ServiceResult> errors)
        {
            if (details is ReadRawModifiedDetails readRaw && !readRaw.IsReadModified)
            {
                for (int i = 0; i < nodesToRead.Count; i++)
                {
                    var nodeToRead = nodesToRead[i];
                    var result = new HistoryReadResult();
                    results[i] = result;
                    errors[i] = StatusCodes.Good;

                    try
                    {
                        string id = nodeToRead.NodeId.Identifier?.ToString() ?? "";
                        if (string.IsNullOrEmpty(id)) 
                        {
                             result.StatusCode = StatusCodes.BadNodeIdUnknown;
                             continue;
                        }

                        // Look up the variable's own logger
                        IVariableLogger? logger = null;
                        if (_variables.TryGetValue(id, out var varState) && varState is ServerVariableState svs)
                        {
                            logger = svs.Logger;
                        }

                        // Fall back to the global logger if the variable doesn't have its own
                        logger ??= _logger;

                        if (logger == null)
                        {
                            result.StatusCode = StatusCodes.BadHistoryOperationUnsupported;
                            continue;
                        }

                        DateTime startTime = readRaw.StartTime;
                        DateTime endTime = readRaw.EndTime;

                        if (startTime == DateTime.MinValue) startTime = DateTime.UtcNow.AddYears(-1);
                        if (endTime == DateTime.MinValue) endTime = DateTime.UtcNow;

                        var values = logger.ReadHistory(id, startTime, endTime);

                        if (readRaw.NumValuesPerNode > 0 && values.Count > readRaw.NumValuesPerNode)
                        {
                            values = values.GetRange(0, (int)readRaw.NumValuesPerNode);
                        }

                        result.HistoryData = new ExtensionObject(new HistoryData { DataValues = new DataValueCollection(values) });
                        result.StatusCode = StatusCodes.Good;
                    }
                    catch (Exception ex)
                    {
                        Utils.Trace(ex, "Error reading history for " + nodeToRead.NodeId);
                        result.StatusCode = StatusCodes.BadUnexpectedError;
                    }
                }
            }
            else
            {
                base.HistoryRead(context, details, timestampsToReturn, releaseContinuationPoints, nodesToRead, results, errors);
            }
        }
        
        private class ServerVariableState : BaseDataVariableState
        {
            private readonly IVariableLogger? _logger;
            private readonly DataLoggingConfig? _config;
            private readonly SimpleFileServerNodeManager _manager;

            internal IVariableLogger? Logger => _logger;

            public ServerVariableState(NodeState parent, IVariableLogger? logger, DataLoggingConfig? config, SimpleFileServerNodeManager manager) : base(parent)
            {
                _logger = logger;
                _config = config;
                _manager = manager;

                // Use OnWriteValue (not OnSimpleWriteValue) so the handler runs BEFORE
                // the SDK's type validation. This allows string-to-typed conversions
                // from editor/runtime inputs to succeed.
                OnWriteValue = HandleWriteValue;

                // Ensure default values for monitoring
                MinimumSamplingInterval = 1000;
                
                if (_logger != null && _config != null && _config.Enabled)
                {
                    this.OnStateChanged += (context, state, masks) =>
                    {
                        if ((masks & NodeStateChangeMasks.Value) != 0 && !_manager._loading)
                        {
                            try
                            {
                                if (state is BaseDataVariableState varState)
                                {
                                    _logger.Log(varState, _config);

                                    // Redundancy: enqueue for replication to standby
                                    if (_manager._redundancy is { IsActive: true })
                                    {
                                        var val = varState.Value;
                                        double? numVal = val switch
                                        {
                                            double d => d, float f => f, int i => i, long l => l,
                                            short s => s, ushort us => us, uint ui => ui, byte b => b,
                                            decimal dc => (double)dc, _ => null
                                        };
                                        _manager._redundancy.EnqueueLogReplication(
                                            varState.NodeId.ToString(),
                                            varState.Timestamp,
                                            numVal,
                                            numVal == null ? val?.ToString() : null,
                                            varState.StatusCode.Code);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utils.Trace(ex, "Error logging variable " + state.NodeId);
                            }
                        }
                    };
                }
            }

            internal ServiceResult InvokeWrite(ISystemContext context, NodeState node, ref object value)
            {
                StatusCode sc = StatusCodes.Good;
                var ts = DateTime.UtcNow;
                return HandleWriteValue(context, node, NumericRange.Empty, null, ref value, ref sc, ref ts);
            }

            private ServiceResult HandleWriteValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
            {
                // Enforce access.
                var access = (byte)(AccessLevel | UserAccessLevel);
                if ((access & AccessLevels.CurrentWrite) != AccessLevels.CurrentWrite)
                {
                    return StatusCodes.BadUserAccessDenied;
                }

                try
                {
                    object? incoming = value;
                    if (incoming is DataValue dv)
                    {
                        incoming = dv.Value;
                    }
                    if (incoming is Variant v)
                    {
                        incoming = v.Value;
                    }

                    // Best-effort conversions from editor input (usually string) to the UA variable type.
                    if (incoming is string s)
                    {
                        if (DataType == DataTypeIds.Boolean && bool.TryParse(s, out var b)) incoming = b;
                        else if (DataType == DataTypeIds.SByte && sbyte.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var sb)) incoming = sb;
                        else if (DataType == DataTypeIds.Byte && byte.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var by)) incoming = by;
                        else if (DataType == DataTypeIds.Int16 && short.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i16)) incoming = i16;
                        else if (DataType == DataTypeIds.UInt16 && ushort.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var u16)) incoming = u16;
                        else if (DataType == DataTypeIds.Int32 && int.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i32)) incoming = i32;
                        else if (DataType == DataTypeIds.UInt32 && uint.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var u32)) incoming = u32;
                        else if (DataType == DataTypeIds.Int64 && long.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i64)) incoming = i64;
                        else if (DataType == DataTypeIds.UInt64 && ulong.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var u64)) incoming = u64;
                        else if (DataType == DataTypeIds.Float && float.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var f)) incoming = f;
                        else if (DataType == DataTypeIds.Double && double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d)) incoming = d;
                        else if (DataType == DataTypeIds.DateTime && DateTime.TryParse(s, out var dt)) incoming = dt;
                    }

                    Utils.Trace("WRITE-HANDLER: nodeId={0} incoming={1} (type={2}) DataType={3}",
                        NodeId, incoming, incoming?.GetType().Name ?? "null", DataType);
                    Value = incoming;
                    StatusCode = StatusCodes.Good;
                    Timestamp = DateTime.UtcNow;
                    ClearChangeMasks(context, false);

                    value = incoming!;
                    return ServiceResult.Good;
                }
                catch (Exception)
                {
                    return StatusCodes.BadTypeMismatch;
                }
            }
        }

        // ─── Retentive variable persistence ─────────────────────────
        private void LoadRetentiveValues()
        {
            try
            {
                var dir = Path.GetDirectoryName(Path.GetFullPath(_configPath));
                _retentivePath = dir != null ? Path.Combine(dir, "retentive.json") : null;
                if (_retentivePath != null && File.Exists(_retentivePath))
                {
                    var json = File.ReadAllText(_retentivePath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (dict != null)
                    {
                        foreach (var kv in dict)
                            _retentiveValues[kv.Key] = kv.Value;
                    }
                    Utils.Trace($"Loaded {_retentiveValues.Count} retentive values from {_retentivePath}");
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Failed to load retentive values");
            }
        }

        private void ScheduleRetentiveSave()
        {
            _retentiveSaveTimer?.Dispose();
            _retentiveSaveTimer = new System.Threading.Timer(_ => SaveRetentiveValues(), null, 2000, System.Threading.Timeout.Infinite);
        }

        private void SaveRetentiveValues()
        {
            try
            {
                if (_retentivePath == null) return;
                var snapshot = new Dictionary<string, string>(_retentiveValues);
                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_retentivePath, json);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Failed to save retentive values");
            }
        }

        // ─── Variable statistics tracker ─────────────────────────
        private class VariableStatisticsTracker
        {
            private readonly BaseDataVariableState<double> _min;
            private readonly BaseDataVariableState<double> _max;
            private readonly BaseDataVariableState<double> _avg;
            private readonly BaseDataVariableState<long> _count;
            private readonly ServerVariableState _reset;
            private readonly ISystemContext _context;

            private double _sum;
            private double _minVal = double.MaxValue;
            private double _maxVal = double.MinValue;
            private long _sampleCount;
            private readonly object _lock = new();

            public VariableStatisticsTracker(
                BaseDataVariableState<double> min,
                BaseDataVariableState<double> max,
                BaseDataVariableState<double> avg,
                BaseDataVariableState<long> count,
                ServerVariableState reset,
                ISystemContext context)
            {
                _min = min;
                _max = max;
                _avg = avg;
                _count = count;
                _reset = reset;
                _context = context;

                // Handle reset writes via OnStateChanged
                _reset.OnStateChanged += (ctx, state, masks) =>
                {
                    if ((masks & NodeStateChangeMasks.Value) != 0 && state is BaseDataVariableState vs)
                    {
                        var rv = vs.Value;
                        bool isReset = rv is true;
                        if (!isReset && rv is string sv)
                            isReset = sv.Equals("true", StringComparison.OrdinalIgnoreCase);
                        if (isReset)
                        {
                            Reset();
                            vs.Value = false;
                            vs.ClearChangeMasks(ctx, false);
                        }
                    }
                };
            }

            public void Record(double value)
            {
                lock (_lock)
                {
                    _sampleCount++;
                    _sum += value;
                    if (value < _minVal) _minVal = value;
                    if (value > _maxVal) _maxVal = value;

                    _min.Value = Math.Round(_minVal, 6);
                    _max.Value = Math.Round(_maxVal, 6);
                    _avg.Value = Math.Round(_sum / _sampleCount, 6);
                    _count.Value = _sampleCount;

                    _min.Timestamp = DateTime.UtcNow;
                    _max.Timestamp = DateTime.UtcNow;
                    _avg.Timestamp = DateTime.UtcNow;
                    _count.Timestamp = DateTime.UtcNow;

                    _min.ClearChangeMasks(_context, false);
                    _max.ClearChangeMasks(_context, false);
                    _avg.ClearChangeMasks(_context, false);
                    _count.ClearChangeMasks(_context, false);
                }
            }

            public void Reset()
            {
                lock (_lock)
                {
                    _sum = 0;
                    _minVal = double.MaxValue;
                    _maxVal = double.MinValue;
                    _sampleCount = 0;

                    _min.Value = 0.0;
                    _max.Value = 0.0;
                    _avg.Value = 0.0;
                    _count.Value = 0L;

                    _min.Timestamp = DateTime.UtcNow;
                    _max.Timestamp = DateTime.UtcNow;
                    _avg.Timestamp = DateTime.UtcNow;
                    _count.Timestamp = DateTime.UtcNow;

                    _min.ClearChangeMasks(_context, false);
                    _max.ClearChangeMasks(_context, false);
                    _avg.ClearChangeMasks(_context, false);
                    _count.ClearChangeMasks(_context, false);
                }
            }
        }

    }
}
