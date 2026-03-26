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
        private AuditTrailLogger? _auditTrailLogger;
        private NotificationManager? _notificationManager;
        private RedundancyManager? _redundancyManager;
        private CalculatedVariableManager? _calculatedManager;
        private RestApiService? _restApi;

        private readonly List<NodeId> _rootNodeIds = new();
        private FileSystemWatcher? _watcher;
        private System.Threading.Timer? _reloadTimer;

        private NodeModel? _lastModel;
        private readonly string _configPath;

        // Alarm tracking
        private readonly Dictionary<string, AlarmConditionInfo> _alarmConditions = new();
        private readonly object _shelvingLock = new();

        private bool _shelvingMethodsCreated;
        // Event journal
        private EventLogger? _eventLogger;

        // Retentive variable storage
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _retentiveValues = new();
        private string? _retentivePath;
        private System.Threading.Timer? _retentiveSaveTimer;

        // Variable statistics tracking
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, VariableStatisticsTracker> _statsTrackers = new();

        // Diagnostics OPC UA node
        private BaseDataVariableState<string>? _diagVariable;
        private System.Threading.Timer? _diagTimer;

        // Rate limiters for endpoint protection
        private RateLimiter? _writeRateLimiter;
        private RateLimiter? _loginRateLimiter;

        public SimpleFileServerNodeManager(IServerInternal server, ApplicationConfiguration configuration, string configPath)
        : base(server, configuration, NodeNamespaceUri)
        {
            _configPath = configPath;
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

            // Rate limiting: throttle excessive writes per session
            if (_writeRateLimiter != null && context?.SessionId != null)
            {
                var sessionKey = context.SessionId.ToString();
                if (!_writeRateLimiter.IsAllowed(sessionKey))
                {
                    _eventLogger?.LogSystem("Warning", $"OPC UA write rate limit exceeded for session {sessionKey}");
                    for (int j = 0; j < nodesToWrite.Count; j++)
                    {
                        nodesToWrite[j].Processed = true;
                        errors[j] = StatusCodes.BadTooManyOperations;
                    }
                    return;
                }
            }

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
                        // Reject writes when this server is in standby mode (redundancy)
                        if (_redundancyManager != null && !_redundancyManager.IsActiveServer)
                        {
                            wv.Processed = true;
                            errors[i] = StatusCodes.BadNotWritable;
                            continue;
                        }

                        // Mark as processed so MasterNodeManager knows we handled it.
                        wv.Processed = true;
                        object v = wv.Value?.Value ?? wv.Value;
                        var oldValue = serverVar.Value;
                        Utils.Trace("WRITE-OVERRIDE: nodeId={0} value={1} (type={2})",
                            wv.NodeId, v, v?.GetType().Name ?? "null");
                        errors[i] = serverVar.InvokeWrite(SystemContext, variable, ref v) ?? ServiceResult.Good;

                        // Audit trail: log operator write
                        if (StatusCode.IsGood(errors[i].StatusCode) && _auditTrailLogger != null)
                        {
                            var username = "unknown";
                            try
                            {
                                username = context?.Session?.Identity?.DisplayName ?? "unknown";
                            }
                            catch { }
                            var varPath = wv.NodeId?.ToString() ?? "";
                            _auditTrailLogger.LogVariableWrite(username, varPath,
                                oldValue?.ToString(), v?.ToString(),
                                context?.Session?.Id?.ToString());
                        }
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

                IList<IReference> references = null;
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                try
                {
                    if (File.Exists(_configPath))
                    {
                        string json;
                        using (var fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs))
                        {
                            json = sr.ReadToEnd();
                        }
                        var nodeModel = JsonSerializer.Deserialize(json, ServerJsonContext.Default.NodeModel);

                        // Load external resource files (scripts/, screens/, plcprograms/)
                        if (nodeModel != null)
                            ResourceFileManager.LoadExternalResources(nodeModel, _configPath);

                        LoadModel(nodeModel, externalReferences);
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

        private void ReloadConfiguration()
        {
            lock (Lock)
            {
                Utils.Trace("Reloading configuration..."); 
                try
                {
                    string json;
                    using (var fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        json = sr.ReadToEnd();
                    }
                    var newModel = JsonSerializer.Deserialize(json, ServerJsonContext.Default.NodeModel);

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
            // Cleanup managers
            if (_scriptManager != null) { _scriptManager.Dispose(); _scriptManager = null; }
            if (_plcManager != null) { _plcManager.Dispose(); _plcManager = null; }
            if (_recipeManager != null) { _recipeManager.Dispose(); _recipeManager = null; }
            if (_calculatedManager != null) { _calculatedManager.Dispose(); _calculatedManager = null; }

            // Cleanup audit trail (new one created in LoadModel)
            if (_auditTrailLogger != null) { _auditTrailLogger.Dispose(); _auditTrailLogger = null; }

            // Cleanup notification manager (new one created in LoadModel)
            if (_notificationManager != null) { _notificationManager.Dispose(); _notificationManager = null; }

            // Cleanup redundancy manager (new one created in LoadModel)
            if (_redundancyManager != null) { _redundancyManager.Dispose(); _redundancyManager = null; }

            // Cleanup REST API (new one created in LoadModel)
            if (_restApi != null) { _restApi.Dispose(); _restApi = null; }

            // Cleanup rate limiters (new ones created in LoadModel)
            _writeRateLimiter?.Dispose(); _writeRateLimiter = null;
            _loginRateLimiter?.Dispose(); _loginRateLimiter = null;

            // Cleanup shelving timers
            lock (_shelvingLock)
            {
                foreach (var t in _shelvingTimers.Values) t.Dispose();
                _shelvingTimers.Clear();
            }

            _variables.Clear();
            _users.Clear();
            _userGroups.Clear();
            _alarmConditions.Clear();
            _statsTrackers.Clear();

            // Cleanup drivers
            foreach (var d in _drivers) d.Dispose();
            _drivers.Clear();
            InitializeDrivers();

            // Cleanup previous event logger (new one created in LoadModel)
            _eventLogger?.Dispose();
            _eventLogger = null;

            // Delete old root nodes
            foreach (var nodeId in _rootNodeIds)
            {
                DeleteNode(SystemContext, nodeId);
            }
            _rootNodeIds.Clear();

            // Re-create
            LoadModel(nodeModel, null);
            _lastModel = nodeModel;
            Utils.Trace("Configuration reloaded fully.");
        }

        private void SessionManager_ImpersonateUser(ISession session, ImpersonateEventArgs args)
        {
            if (args.NewIdentity is UserNameIdentityToken userNameToken)
            {
                var username = userNameToken.UserName;

                // Rate limiting: throttle excessive login attempts per username
                if (_loginRateLimiter != null && !_loginRateLimiter.IsAllowed(username ?? "unknown"))
                {
                    _eventLogger?.LogAuth("Warning", username ?? "unknown",
                        $"Login rate limit exceeded for user '{username}' — too many attempts");
                    throw new ServiceResultException(StatusCodes.BadTooManyOperations,
                        "Too many login attempts. Please try again later.");
                }

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
                        _auditTrailLogger?.LogLogin(username, session?.Id?.ToString());
                        return;
                    }
                }

                _eventLogger?.LogAuth("Warning", username, $"Login failed for user '{username}' — invalid credentials");
                _auditTrailLogger?.LogLogin(username, $"FAILED — {session?.Id}");
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

            // Check Subfolders
            if (oldFolder.Folders.Count != newFolder.Folders.Count) return false; // No new/deleted folders allowed incrementally

            // Map old folders for comparison
            var oldFoldersMap = oldFolder.Folders.ToDictionary(f => f.Name);
            foreach (var newSubFolder in newFolder.Folders)
            {
                if (!oldFoldersMap.TryGetValue(newSubFolder.Name, out var oldSubFolder)) return false; // Name/Structure mismatch
                if (!CheckFolderStructure(oldSubFolder, newSubFolder, currentPath, newVariables)) return false; 
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
                    newVariables.Add((newVar, currentPath));
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

        private void LoadModel(NodeModel nodeModel, IDictionary<NodeId, IList<IReference>>? externalReferences)
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
                         _logger = new SqliteLogger(db.ConnectionString, db.TableName);
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
                     var dbPath = evtCfg?.DbPath ?? "events.db";
                     if (!Path.IsPathRooted(dbPath))
                     {
                         var dir = Path.GetDirectoryName(Path.GetFullPath(_configPath));
                         if (!string.IsNullOrEmpty(dir))
                             dbPath = Path.Combine(dir, dbPath);
                     }
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

                 // Init Audit Trail Logger
                 var auditCfg = nodeModel.Server?.AuditTrail;
                 if (auditCfg == null || auditCfg.Enabled)
                 {
                     var auditDbPath = auditCfg?.DbPath ?? "audit.db";
                     if (!Path.IsPathRooted(auditDbPath))
                     {
                         var dir = Path.GetDirectoryName(Path.GetFullPath(_configPath));
                         if (!string.IsNullOrEmpty(dir))
                             auditDbPath = Path.Combine(dir, auditDbPath);
                     }
                     var auditMaxAge = auditCfg?.MaxAgeDays ?? 365;
                         _auditTrailLogger = new AuditTrailLogger(auditDbPath, auditMaxAge);
                         DiagnosticsCollector.Instance.Register("AuditTrail", "AuditLog");
                     }

                     // Init Notification Manager
                     _notificationManager = new NotificationManager(nodeModel.Server?.Notifications);
                      DiagnosticsCollector.Instance.Register("Notifications", "AlarmNotifications");

                  // Init Redundancy Manager
                  var redundancyCfg = nodeModel.Server?.Redundancy;
                  if (redundancyCfg != null && redundancyCfg.Enabled)
                  {
                      _redundancyManager = new RedundancyManager(redundancyCfg, _eventLogger);

                      // State sync: provide variable snapshot from primary, apply on standby
                      _redundancyManager.GetStateSnapshot = () =>
                      {
                          var snapshot = new Dictionary<string, string>();
                          foreach (var kv in _variables)
                          {
                              var val = kv.Value.Value;
                              if (val != null)
                                  snapshot[kv.Key] = val.ToString() ?? "";
                          }
                          return snapshot;
                      };
                      _redundancyManager.ApplyStateSnapshot = (snapshot) =>
                      {
                          foreach (var kv in snapshot)
                          {
                              if (_variables.TryGetValue(kv.Key, out var variable) && variable is ServerVariableState svs)
                              {
                                  try
                                  {
                                      var converted = ServerVariableState.ConvertValueToType(kv.Value, variable.DataType, SystemContext);
                                      if (converted != null)
                                      {
                                          variable.Value = converted;
                                          variable.Timestamp = DateTime.UtcNow;
                                          variable.ClearChangeMasks(SystemContext, false);
                                      }
                                  }
                                  catch { }
                              }
                          }
                      };

                      // Failover events: log and audit
                      _redundancyManager.OnFailover += (reason) =>
                      {
                          _eventLogger?.LogSystem("Critical", "Redundancy", reason);
                          _auditTrailLogger?.LogConfigChange("system", "Redundancy", "Standby", "Active (Failover)");
                      };
                      _redundancyManager.OnSwitchback += (reason) =>
                      {
                          _eventLogger?.LogSystem("Info", "Redundancy", reason);
                          _auditTrailLogger?.LogConfigChange("system", "Redundancy", "Active (Failover)", "Standby");
                      };
                      _redundancyManager.OnReplicationStatusChanged += (status, healthy) =>
                      {
                          if (!healthy)
                              _eventLogger?.LogSystem("Warning", "Redundancy", $"Database replication: {status}");
                      };

                      _redundancyManager.Start();
                  }

                  if (nodeModel.Folder != null)
                 {
                     CreateFolder(nodeModel.Folder, null, references, "");
                 }

                 // scripts
                 if (nodeModel.Scripts != null)
                 {
                     _scriptManager = new ScriptManager(this);
                     _scriptManager.Initialize(nodeModel.Scripts);
                 }

                 // PLC programs (IEC 61131-3 Structured Text)
                 if (nodeModel.PlcPrograms != null && nodeModel.PlcPrograms.Count > 0)
                 {
                     _plcManager = new PlcManager(this);
                     _plcManager.Initialize(nodeModel.PlcPrograms);
                 }

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

                  // Calculated / Virtual Tags
                  if (nodeModel.CalculatedVariables != null && nodeModel.CalculatedVariables.Count > 0)
                  {
                      CreateCalculatedVariables(nodeModel.CalculatedVariables, references);
                      _calculatedManager = new CalculatedVariableManager(this);
                      _calculatedManager.Initialize(nodeModel.CalculatedVariables);
                  }

                               // ─── Diagnostics OPC UA node (always created, license-exempt) ───
                          CreateDiagnosticsNode(references);

                                        // ─── REST API for external integration ───
                                        if (nodeModel.Server?.Api != null && nodeModel.Server.Api.Enabled)
                                        {
                                            _restApi = new RestApiService(this, nodeModel.Server.Api, _eventLogger, _auditTrailLogger, nodeModel.Server?.RateLimit);
                                            _restApi.Start();
                                        }

                                        // ─── Rate limiters for OPC UA endpoints ───
                                        if (nodeModel.Server?.RateLimit is { Enabled: true } rl)
                                        {
                                            var window = TimeSpan.FromSeconds(rl.WindowSeconds);
                                            if (rl.OpcUaWriteMaxPerWindow > 0)
                                                _writeRateLimiter = new RateLimiter(rl.OpcUaWriteMaxPerWindow, window);
                                            if (rl.LoginMaxAttemptsPerWindow > 0)
                                                _loginRateLimiter = new RateLimiter(rl.LoginMaxAttemptsPerWindow, window);
                                        }
                               }

                               // ─── Alarm shelving OPC UA methods ───
                          CreateAlarmShelvingMethods(references);
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
        /// Creates OPC UA variables for each calculated/virtual tag under their configured folder paths.
        /// These variables are written to cyclically by the CalculatedVariableManager.
        /// </summary>
        private void CreateCalculatedVariables(List<CalculatedVariableConfig> configs, IList<IReference>? references)
        {
            // Group by folder path to create folders as needed
            var folders = new Dictionary<string, FolderState>();

            foreach (var config in configs)
            {
                if (!config.Enabled) continue;

                var folderPath = config.FolderPath;
                if (!folders.TryGetValue(folderPath, out var folder))
                {
                    folder = CreateOrGetFolder(folderPath, references);
                    folders[folderPath] = folder;
                }

                var varPath = $"{folderPath}.{config.Name}";
                var dataTypeId = GetDataTypeId(config.Type);

                var variable = new BaseDataVariableState(folder);
                variable.NodeId = new NodeId(varPath, _namespaceIndex);
                variable.BrowseName = new QualifiedName(config.Name, _namespaceIndex);
                variable.DisplayName = new LocalizedText(config.Name);
                variable.DataType = dataTypeId;
                variable.ValueRank = ValueRanks.Scalar;
                variable.Value = config.Type switch
                {
                    "Int32" => 0,
                    "Boolean" => false,
                    "String" => "",
                    _ => 0.0
                };
                variable.AccessLevel = AccessLevels.CurrentRead;
                variable.UserAccessLevel = AccessLevels.CurrentRead;
                variable.Timestamp = DateTime.UtcNow;
                variable.StatusCode = StatusCodes.Good;

                // Add engineering unit property if configured
                if (!string.IsNullOrEmpty(config.EngineeringUnit))
                {
                    var euProp = new PropertyState<EUInformation>(variable);
                    euProp.NodeId = new NodeId(varPath + ".EngineeringUnits", _namespaceIndex);
                    euProp.BrowseName = BrowseNames.EngineeringUnits;
                    euProp.DisplayName = new LocalizedText("EngineeringUnits");
                    euProp.DataType = DataTypeIds.EUInformation;
                    euProp.ValueRank = ValueRanks.Scalar;
                    euProp.Value = new EUInformation(config.EngineeringUnit, config.EngineeringUnit, "http://www.opcfoundation.org/UA/units/un/cefact");
                    euProp.AccessLevel = AccessLevels.CurrentRead;
                    euProp.UserAccessLevel = AccessLevels.CurrentRead;
                    variable.AddChild(euProp);
                    AddPredefinedNode(SystemContext, euProp);
                }

                // Add description based on config
                var description = !string.IsNullOrEmpty(config.AggregateFunction)
                    ? $"Aggregate: {config.AggregateFunction}({config.AggregateSourcePath})"
                    : $"Expression: {config.Expression}";
                variable.Description = new LocalizedText(description);

                folder.AddChild(variable);
                AddPredefinedNode(SystemContext, variable);
                _variables[varPath] = variable;
            }
        }

        /// <summary>
        /// Creates or retrieves a folder in the OPC UA address space by dot-separated path.
        /// Nested paths like "Plant.Calculated" create the hierarchy automatically.
        /// </summary>
        private FolderState CreateOrGetFolder(string folderPath, IList<IReference>? references)
        {
            // Check if any existing root node has this path
            var existingNode = FindPredefinedNode(new NodeId(folderPath, _namespaceIndex), typeof(FolderState));
            if (existingNode is FolderState existing) return existing;

            // Build hierarchically
            var parts = folderPath.Split('.');
            FolderState? parent = null;
            string currentPath = "";

            for (int i = 0; i < parts.Length; i++)
            {
                currentPath = i == 0 ? parts[i] : $"{currentPath}.{parts[i]}";
                var nodeId = new NodeId(currentPath, _namespaceIndex);

                var existingFolder = FindPredefinedNode(nodeId, typeof(FolderState)) as FolderState;
                if (existingFolder != null)
                {
                    parent = existingFolder;
                    continue;
                }

                var folder = new FolderState(parent);
                folder.NodeId = nodeId;
                folder.BrowseName = new QualifiedName(parts[i], _namespaceIndex);
                folder.DisplayName = new LocalizedText(parts[i]);
                folder.TypeDefinitionId = ObjectTypeIds.FolderType;
                folder.ReferenceTypeId = ReferenceTypes.Organizes;

                if (parent != null)
                {
                    parent.AddChild(folder);
                    AddPredefinedNode(SystemContext, folder);
                }
                else
                {
                    _rootNodeIds.Add(folder.NodeId);
                    AddPredefinedNode(SystemContext, folder);
                    AddRootNotifier(folder);
                    folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                    references?.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
                }
                parent = folder;
            }

            return parent!;
        }

        /// <summary>
        /// Creates a _Diagnostics folder in the OPC UA address space with a DiagnosticsJson
        /// variable that is updated every 2 seconds from DiagnosticsCollector.
        /// The editor reads this variable instead of using a separate HTTP endpoint.
        /// Safe to call on reload — skips if already created.
        /// </summary>
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
        /// Creates OPC UA method nodes for shelving/unshelving alarms.
        /// Clients can call these methods to shelve or unshelve alarms by variable path.
        /// </summary>
        private void CreateAlarmShelvingMethods(IList<IReference>? references)
        {
            if (_shelvingMethodsCreated) return;
            _shelvingMethodsCreated = true;

            var folder = new FolderState(null);
            folder.NodeId = new NodeId("_AlarmManagement", _namespaceIndex);
            folder.BrowseName = new QualifiedName("_AlarmManagement", _namespaceIndex);
            folder.DisplayName = new LocalizedText("_AlarmManagement");
            folder.TypeDefinitionId = ObjectTypeIds.FolderType;
            folder.EventNotifier = EventNotifiers.None;

            folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
            references?.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folder.NodeId));
            _rootNodeIds.Add(folder.NodeId);
            AddPredefinedNode(SystemContext, folder);

            // ShelveAlarm(string variablePath, int durationMinutes) → bool
            var shelveMethod = new MethodState(folder);
            shelveMethod.NodeId = new NodeId("_AlarmManagement.ShelveAlarm", _namespaceIndex);
            shelveMethod.BrowseName = new QualifiedName("ShelveAlarm", _namespaceIndex);
            shelveMethod.DisplayName = new LocalizedText("ShelveAlarm");
            shelveMethod.Executable = true;
            shelveMethod.UserExecutable = true;
            shelveMethod.InputArguments = new PropertyState<Argument[]>(shelveMethod)
            {
                NodeId = new NodeId("_AlarmManagement.ShelveAlarm.InputArguments", _namespaceIndex),
                BrowseName = BrowseNames.InputArguments,
                DisplayName = new LocalizedText("InputArguments"),
                TypeDefinitionId = VariableTypeIds.PropertyType,
                DataType = DataTypeIds.Argument,
                ValueRank = ValueRanks.OneDimension,
                Value = new Argument[]
                {
                    new Argument { Name = "VariablePath", DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar, Description = "The alarm variable path" },
                    new Argument { Name = "DurationMinutes", DataType = DataTypeIds.Int32, ValueRank = ValueRanks.Scalar, Description = "Shelving duration in minutes (0 = default)" },
                    new Argument { Name = "Username", DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar, Description = "Operator username" }
                }
            };
            shelveMethod.OutputArguments = new PropertyState<Argument[]>(shelveMethod)
            {
                NodeId = new NodeId("_AlarmManagement.ShelveAlarm.OutputArguments", _namespaceIndex),
                BrowseName = BrowseNames.OutputArguments,
                DisplayName = new LocalizedText("OutputArguments"),
                TypeDefinitionId = VariableTypeIds.PropertyType,
                DataType = DataTypeIds.Argument,
                ValueRank = ValueRanks.OneDimension,
                Value = new Argument[]
                {
                    new Argument { Name = "Success", DataType = DataTypeIds.Boolean, ValueRank = ValueRanks.Scalar, Description = "True if shelved successfully" }
                }
            };
            shelveMethod.OnCallMethod = OnShelveAlarmCall;
            folder.AddChild(shelveMethod);
            AddPredefinedNode(SystemContext, shelveMethod);

            // UnshelveAlarm(string variablePath) → bool
            var unshelveMethod = new MethodState(folder);
            unshelveMethod.NodeId = new NodeId("_AlarmManagement.UnshelveAlarm", _namespaceIndex);
            unshelveMethod.BrowseName = new QualifiedName("UnshelveAlarm", _namespaceIndex);
            unshelveMethod.DisplayName = new LocalizedText("UnshelveAlarm");
            unshelveMethod.Executable = true;
            unshelveMethod.UserExecutable = true;
            unshelveMethod.InputArguments = new PropertyState<Argument[]>(unshelveMethod)
            {
                NodeId = new NodeId("_AlarmManagement.UnshelveAlarm.InputArguments", _namespaceIndex),
                BrowseName = BrowseNames.InputArguments,
                DisplayName = new LocalizedText("InputArguments"),
                TypeDefinitionId = VariableTypeIds.PropertyType,
                DataType = DataTypeIds.Argument,
                ValueRank = ValueRanks.OneDimension,
                Value = new Argument[]
                {
                    new Argument { Name = "VariablePath", DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar, Description = "The alarm variable path" },
                    new Argument { Name = "Username", DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar, Description = "Operator username" }
                }
            };
            unshelveMethod.OutputArguments = new PropertyState<Argument[]>(unshelveMethod)
            {
                NodeId = new NodeId("_AlarmManagement.UnshelveAlarm.OutputArguments", _namespaceIndex),
                BrowseName = BrowseNames.OutputArguments,
                DisplayName = new LocalizedText("OutputArguments"),
                TypeDefinitionId = VariableTypeIds.PropertyType,
                DataType = DataTypeIds.Argument,
                ValueRank = ValueRanks.OneDimension,
                Value = new Argument[]
                {
                    new Argument { Name = "Success", DataType = DataTypeIds.Boolean, ValueRank = ValueRanks.Scalar, Description = "True if unshelved successfully" }
                }
            };
            unshelveMethod.OnCallMethod = OnUnshelveAlarmCall;
            folder.AddChild(unshelveMethod);
            AddPredefinedNode(SystemContext, unshelveMethod);
        }

        private ServiceResult OnShelveAlarmCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            var variablePath = inputArguments[0]?.ToString() ?? "";
            var durationMinutes = Convert.ToInt32(inputArguments[1]);
            var username = inputArguments[2]?.ToString() ?? "anonymous";
            var success = ShelveAlarm(variablePath, durationMinutes, username);
            outputArguments[0] = success;
            return success ? ServiceResult.Good : new ServiceResult(StatusCodes.BadInvalidArgument);
        }

        private ServiceResult OnUnshelveAlarmCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            var variablePath = inputArguments[0]?.ToString() ?? "";
            var username = inputArguments[1]?.ToString() ?? "anonymous";
            var success = UnshelveAlarm(variablePath, username);
            outputArguments[0] = success;
            return success ? ServiceResult.Good : new ServiceResult(StatusCodes.BadInvalidArgument);
        }
        private void CreateFolder(Folder folder, BaseObjectState? parent, IList<IReference>? references, string pathPrefix)
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
                parent.AddChild(folderState);
                AddPredefinedNode(SystemContext, folderState);
            }
            else
            {
                _rootNodeIds.Add(folderState.NodeId); // Track root
                AddPredefinedNode(SystemContext, folderState);
                AddRootNotifier(folderState);

                // Ensure inverse reference exists
                folderState.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

                if (references != null)
                {
                    references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, folderState.NodeId));
                }
            }

            foreach(var subFolder in folder.Folders)
            {
                try
                {
                    CreateFolder(subFolder, folderState, references, currentPath);
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
                    CreateVariable(variable, folderState, currentPath);
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, $"Error creating variable '{variable.Name}' under '{currentPath}'");
                }
            }
        }

        private void CreateVariable(Variable variable, BaseObjectState parent, string pathPrefix)
        {
            string currentPath = $"{pathPrefix}.{variable.Name}";
            
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

            // ─── Scaling configuration ──────────────────────────────────
            if (variable.Scaling != null)
            {
                variableState.Scaling = variable.Scaling;

                // Apply forward scaling (raw → eng) to the initial value
                if (variableState.Value != null &&
                    double.TryParse(Convert.ToString(variableState.Value, System.Globalization.CultureInfo.InvariantCulture),
                        System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var rawInit))
                {
                    double engInit = variable.Scaling.RawToEng(rawInit);
                    variableState.Value = ConvertValueToType(engInit, variable.Type);
                    Utils.Trace("SCALING-INIT: {0} raw={1:G6} → eng={2:G6}", currentPath, rawInit, engInit);
                }
            }

            // ─── Engineering Units OPC UA property ──────────────────────
            if (!string.IsNullOrEmpty(variable.EngineeringUnit))
            {
                var euProp = new PropertyState<EUInformation>(variableState);
                euProp.NodeId = new NodeId(currentPath + ".EngineeringUnits", _namespaceIndex);
                euProp.BrowseName = BrowseNames.EngineeringUnits;
                euProp.DisplayName = new LocalizedText("EngineeringUnits");
                euProp.DataType = DataTypeIds.EUInformation;
                euProp.ValueRank = ValueRanks.Scalar;
                euProp.AccessLevel = AccessLevels.CurrentRead;
                euProp.UserAccessLevel = AccessLevels.CurrentRead;
                euProp.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                euProp.Value = new EUInformation(variable.EngineeringUnit, variable.EngineeringUnit, "http://www.opcfoundation.org/UA/units/un/cefact");
                variableState.AddChild(euProp);
                AddPredefinedNode(SystemContext, euProp);
            }

            // ─── EU Range OPC UA property (shows min/max engineering range) ─
            if (variable.Scaling != null)
            {
                var rangeProp = new PropertyState<Opc.Ua.Range>(variableState);
                rangeProp.NodeId = new NodeId(currentPath + ".EURange", _namespaceIndex);
                rangeProp.BrowseName = BrowseNames.EURange;
                rangeProp.DisplayName = new LocalizedText("EURange");
                rangeProp.DataType = DataTypeIds.Range;
                rangeProp.ValueRank = ValueRanks.Scalar;
                rangeProp.AccessLevel = AccessLevels.CurrentRead;
                rangeProp.UserAccessLevel = AccessLevels.CurrentRead;
                rangeProp.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                rangeProp.Value = new Opc.Ua.Range(
                    Math.Max(variable.Scaling.EngMin, variable.Scaling.EngMax),
                    Math.Min(variable.Scaling.EngMin, variable.Scaling.EngMax));
                variableState.AddChild(rangeProp);
                AddPredefinedNode(SystemContext, rangeProp);
            }

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
                AddPredefinedNode(SystemContext, lastError);
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

            parent.AddChild(variableState);
            AddPredefinedNode(SystemContext, variableState);

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

            // Apply forward scaling (raw → eng) when the driver pushes a new raw value
            if (variable.Scaling != null && connectedToDriver)
            {
                var scaling = variable.Scaling;
                var varType = variable.Type;
                variableState.OnStateChanged += (ctx, state, masks) =>
                {
                    if ((masks & NodeStateChangeMasks.Value) != 0 && state is ServerVariableState svs && !svs.ScalingInProgress)
                    {
                        if (svs.Value != null &&
                            double.TryParse(Convert.ToString(svs.Value, System.Globalization.CultureInfo.InvariantCulture),
                                System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var rawVal))
                        {
                            double engVal = scaling.RawToEng(rawVal);
                            svs.ScalingInProgress = true;
                            try
                            {
                                svs.Value = ConvertValueToType(engVal, varType);
                                svs.Timestamp = DateTime.UtcNow;
                                svs.ClearChangeMasks(ctx, false);
                            }
                            finally
                            {
                                svs.ScalingInProgress = false;
                            }
                        }
                    }
                };
            }

            // Create alarm condition if AlarmConfig is specified
            if (variable.Alarm != null)
            {
                CreateAlarmCondition(variableState, variable.Alarm, currentPath, parent);
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

                AddPredefinedNode(SystemContext, statsFolder);
                AddPredefinedNode(SystemContext, statsMin);
                AddPredefinedNode(SystemContext, statsMax);
                AddPredefinedNode(SystemContext, statsAvg);
                AddPredefinedNode(SystemContext, statsCount);
                AddPredefinedNode(SystemContext, statsReset);

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

        private void CreateEventVariable(Variable variable, BaseObjectState parent, string pathPrefix)
        {
            string currentPath = $"{pathPrefix}.{variable.Name}";
            
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

            parent.AddChild(variableState);
            AddPredefinedNode(SystemContext, variableState);

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
        public object? ReadVariable(string variableName)
        {
            if (_variables.TryGetValue(variableName, out var variable))
            {
                return variable.Value;
            }
            // Try by full path/NodeId if needed? 
            // variableName matches variable.Name property from json.
            throw new InvalidOperationException("Variable not found: " + variableName);
            //return null;
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
                if (_variables.TryGetValue(variablePath, out var vs))
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
            if (_variables.TryGetValue(variableName, out var variable))
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

        /// <summary>
        /// Converts a double value to the specified variable type string ("Double", "Int32", "Float", etc.).
        /// Used for scaling transformations where the result is always a double that needs to match the variable's declared type.
        /// </summary>
        private static object ConvertValueToType(double val, string type)
        {
            return type switch
            {
                "SByte" => (sbyte)Math.Clamp(val, sbyte.MinValue, sbyte.MaxValue),
                "Byte" => (byte)Math.Clamp(val, byte.MinValue, byte.MaxValue),
                "Int16" => (short)Math.Clamp(val, short.MinValue, short.MaxValue),
                "UInt16" => (ushort)Math.Clamp(val, ushort.MinValue, ushort.MaxValue),
                "Int32" => (int)Math.Clamp(val, int.MinValue, int.MaxValue),
                "UInt32" => (uint)Math.Clamp(val, uint.MinValue, uint.MaxValue),
                "Int64" => (long)Math.Clamp(val, long.MinValue, long.MaxValue),
                "UInt64" => (ulong)Math.Max(val, 0),
                "Float" => (float)val,
                _ => val // Double or fallback
            };
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
            public bool IsShelved { get; set; }
            public DateTime? ShelvedUntil { get; set; }
            public string? ShelvedBy { get; set; }
        }

        // Shelving auto-unshelve timers
        private readonly Dictionary<string, Timer> _shelvingTimers = new();

        private void CreateAlarmCondition(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent)
        {
            if (alarmConfig.TriggerType == AlarmTriggerType.Condition)
                CreateConditionAlarm(variableState, alarmConfig, variablePath, parent);
            else
                CreateLimitAlarm(variableState, alarmConfig, variablePath, parent);
        }

        private void CreateLimitAlarm(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent)
        {
            var alarmNodeId = new NodeId(variablePath + ".Alarm", _namespaceIndex);

            var alarm = new ExclusiveLimitAlarmState(parent);

            alarm.Create(
                SystemContext,
                alarmNodeId,
                new QualifiedName(variableState.BrowseName.Name + "Alarm", _namespaceIndex),
                new LocalizedText(variableState.DisplayName.Text + " Alarm"),
                true);

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
            parent.AddChild(alarm);
            AddPredefinedNode(SystemContext, alarm);

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
            Utils.Trace("ALARM-CREATE: {0} HighLimit={1} LowLimit={2} HH={3} LL={4} Hyst={5} InitialValue={6}",
                variablePath, alarmConfig.HighLimit, alarmConfig.LowLimit,
                alarmConfig.HighHighLimit, alarmConfig.LowLowLimit, alarmConfig.Hysteresis,
                variableState.Value);

            // Monitor value changes to activate/deactivate the alarm
            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0)
                {
                    try
                    {
                        Utils.Trace("ALARM-STATECHANGED: {0} masks={1} value={2}",
                            info.VariablePath, masks, info.SourceVariable.Value);
                        EvaluateAlarmCondition(info);
                    }
                    catch (Exception ex)
                    {
                        Utils.Trace(ex, "ALARM-STATECHANGED-ERROR: {0}", info.VariablePath);
                    }
                }
            };

            // Evaluate once with the initial value
            Utils.Trace("ALARM-INIT-EVAL: {0} value={1}", variablePath, variableState.Value);
            EvaluateAlarmCondition(info);
        }

        private void CreateConditionAlarm(BaseDataVariableState variableState, AlarmConfig alarmConfig, string variablePath, BaseObjectState parent)
        {
            var alarmNodeId = new NodeId(variablePath + ".Alarm", _namespaceIndex);

            var alarm = new OffNormalAlarmState(parent);

            alarm.Create(
                SystemContext,
                alarmNodeId,
                new QualifiedName(variableState.BrowseName.Name + "Alarm", _namespaceIndex),
                new LocalizedText(variableState.DisplayName.Text + " Alarm"),
                true);

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

            parent.AddChild(alarm);
            AddPredefinedNode(SystemContext, alarm);

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
                if ((masks & NodeStateChangeMasks.Value) != 0)
                    EvaluateAlarmCondition(info);
            };

            EvaluateAlarmCondition(info);
        }

        private void EvaluateAlarmCondition(AlarmConditionInfo info)
        {
            try
            {
                // Skip evaluation for shelved alarms — suppress notifications
                if (info.IsShelved)
                {
                    Utils.Trace("ALARM-EVAL-SHELVED: {0} shelved until {1}, skipping", info.VariablePath, info.ShelvedUntil);
                    return;
                }

                Utils.Trace("ALARM-EVAL: {0} type={1} value={2} (type={3}) isActive={4}",
                    info.VariablePath, info.Config.TriggerType,
                    info.SourceVariable.Value, info.SourceVariable.Value?.GetType().Name ?? "null", info.IsActive);
                if (info.Config.TriggerType == AlarmTriggerType.Condition)
                    EvaluateConditionAlarm(info);
                else
                    EvaluateLimitAlarm(info);
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "ALARM-EVAL-ERROR: {0}", info.VariablePath);
            }
        }

        private void EvaluateLimitAlarm(AlarmConditionInfo info)
        {
            var result = AlarmEvaluator.EvaluateLimitAlarm(info.SourceVariable.Value, info.Config, info.IsActive);
            Utils.Trace("ALARM-LIMIT: {0} result={1} activate={2} deactivate={3}",
                info.VariablePath, result != null ? "ok" : "null",
                result?.ShouldActivate, result?.ShouldDeactivate);
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
                Utils.Trace("ALARM-ACTIVATE: {0} severity={1} limitState={2}", info.VariablePath, result.Severity, result.LimitState);

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
                Utils.Trace("ALARM-REPORTED: {0} message={1}", info.VariablePath, message);
                _eventLogger?.LogAlarm(severity >= 800 ? "Critical" : "Warning", info.VariablePath, message,
                    $"Transition=Inactive->Active Value={val:G6} HH={highHigh} H={cfg.HighLimit} L={cfg.LowLimit} LL={lowLow} Hyst={hyst}");

                // Notification: email, webhook, escalation
                _notificationManager?.NotifyAlarmActivated(info.VariablePath, message, severity, cfg.Notification);
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

                bool isAcked = alarm.AckedState.Id.Value;
                bool isConfirmed = alarm.ConfirmedState.Id.Value;
                bool fullyCleared = isAcked && isConfirmed;
                alarm.Retain.Value = !fullyCleared;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.ExclusiveLimitAlarmType;

                ReportAlarmEvent(alarm);
                var stateDesc = fullyCleared ? "Cleared" : !isAcked ? "Inactive(Unacked)" : "Inactive(Unconfirmed)";
                _eventLogger?.LogAlarm("Info", info.VariablePath,
                    $"Returned to normal — value={val:G6}",
                    $"Transition=Active->{stateDesc} Acked={isAcked} Confirmed={isConfirmed}");

                // Notification: cancel escalation on deactivation
                _notificationManager?.NotifyAlarmCleared(info.VariablePath);
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
                    info.VariablePath, message,
                    $"Transition=Inactive->Active Operator={cfg.Operator} Compare={cfg.CompareValue} Value={valueStr} Hyst={cfg.Hysteresis}");

                // Notification: email, webhook, escalation
                _notificationManager?.NotifyAlarmActivated(info.VariablePath, message, cfg.ConditionSeverity, cfg.Notification);
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

                bool isAcked = alarm.AckedState.Id.Value;
                bool isConfirmed = alarm.ConfirmedState.Id.Value;
                bool fullyCleared = isAcked && isConfirmed;
                alarm.Retain.Value = !fullyCleared;

                alarm.EventId.Value = Guid.NewGuid().ToByteArray();
                alarm.EventType.Value = ObjectTypeIds.OffNormalAlarmType;

                ReportAlarmEvent(alarm);
                var stateDesc = fullyCleared ? "Cleared" : !isAcked ? "Inactive(Unacked)" : "Inactive(Unconfirmed)";
                _eventLogger?.LogAlarm("Info", info.VariablePath,
                    $"Condition cleared — value={valueStr}",
                    $"Transition=Active->{stateDesc} Acked={isAcked} Confirmed={isConfirmed}");

                // Notification: cancel escalation on deactivation
                _notificationManager?.NotifyAlarmCleared(info.VariablePath);
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
                    $"Transition=Unacked->Acknowledged Active={isActive} Confirmed={isConfirmed} Comment={comment?.Text}");

                // Audit trail: log alarm acknowledgment
                if (_auditTrailLogger != null)
                {
                    var username = "unknown";
                    try
                    {
                        if (context is ServerSystemContext ssc)
                            username = ssc.OperationContext?.Session?.Identity?.DisplayName ?? "unknown";
                    }
                    catch { }
                    _auditTrailLogger.LogAlarmAcknowledge(username,
                        ackCondition.ConditionName.Value ?? "Alarm", comment?.Text);
                }

                // Notification: cancel escalation when alarm is acknowledged
                var alarmPath = _alarmConditions.FirstOrDefault(kv => kv.Value.AlarmState == condition).Key;
                if (!string.IsNullOrEmpty(alarmPath))
                    _notificationManager?.CancelEscalation(alarmPath);
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
                    $"Transition=Unconfirmed->Confirmed Active={isActive} Acked={isAcked} Comment={comment?.Text}");

                // Audit trail: log alarm confirmation
                if (_auditTrailLogger != null)
                {
                    var username = "unknown";
                    try
                    {
                        if (context is ServerSystemContext ssc)
                            username = ssc.OperationContext?.Session?.Identity?.DisplayName ?? "unknown";
                    }
                    catch { }
                    _auditTrailLogger.LogAlarmConfirm(username,
                        ackCondition.ConditionName.Value ?? "Alarm", comment?.Text);
                }
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

        // ─── Alarm Shelving ─────────────────────────────────────────────────

        /// <summary>
        /// Shelve (temporarily suppress) an alarm for the specified duration.
        /// The alarm will not fire while shelved. Auto-unshelves when the duration expires.
        /// </summary>
        /// <param name="variablePath">The variable path identifying the alarm.</param>
        /// <param name="durationMinutes">Duration in minutes. 0 = use config default.</param>
        /// <param name="username">Operator who shelved the alarm (for audit trail).</param>
        /// <returns>True if the alarm was successfully shelved.</returns>
        public bool ShelveAlarm(string variablePath, int durationMinutes, string username)
        {
            if (!_alarmConditions.TryGetValue(variablePath, out var info))
                return false;

            if (!info.Config.AllowShelving)
            {
                Utils.Trace("ALARM-SHELVE-DENIED: {0} — shelving not allowed by configuration", variablePath);
                return false;
            }

            // Clamp duration to configured maximum
            var maxMinutes = info.Config.MaxShelvingMinutes;
            if (durationMinutes <= 0)
                durationMinutes = maxMinutes > 0 ? maxMinutes : 60;
            if (maxMinutes > 0 && durationMinutes > maxMinutes)
                durationMinutes = maxMinutes;

            lock (_shelvingLock)
            {
                info.IsShelved = true;
                info.ShelvedUntil = DateTime.UtcNow.AddMinutes(durationMinutes);
                info.ShelvedBy = username;

                // Set OPC UA SuppressedState
                if (info.AlarmState.SuppressedState != null)
                {
                    info.AlarmState.SuppressedState.Value = new LocalizedText("en", "Shelved");
                    info.AlarmState.SuppressedState.Id.Value = true;
                    info.AlarmState.SuppressedState.TransitionTime.Value = DateTime.UtcNow;
                }
                info.AlarmState.Message.Value = new LocalizedText(
                    $"Shelved by {username} until {info.ShelvedUntil.Value:HH:mm:ss} UTC");
                info.AlarmState.Time.Value = DateTime.UtcNow;
                info.AlarmState.EventId.Value = Guid.NewGuid().ToByteArray();
                ReportAlarmEvent(info.AlarmState);

                // Cancel any existing timer for this alarm
                if (_shelvingTimers.TryGetValue(variablePath, out var existingTimer))
                {
                    existingTimer.Dispose();
                    _shelvingTimers.Remove(variablePath);
                }

                // Schedule auto-unshelve
                var path = variablePath; // capture for lambda
                var timer = new Timer(_ => UnshelveAlarm(path, "System (auto-unshelve)"),
                    null, TimeSpan.FromMinutes(durationMinutes), Timeout.InfiniteTimeSpan);
                _shelvingTimers[variablePath] = timer;
            }

            _eventLogger?.LogAlarm("Info", variablePath,
                $"Alarm shelved by {username} for {durationMinutes} minutes",
                $"ShelvedUntil={info.ShelvedUntil:o}");
            _auditTrailLogger?.LogAlarmShelve(username, variablePath,
                $"Duration={durationMinutes}min Until={info.ShelvedUntil:o}");

            Utils.Trace("ALARM-SHELVED: {0} by {1} for {2}min", variablePath, username, durationMinutes);
            return true;
        }

        /// <summary>
        /// Unshelve (re-enable) a previously shelved alarm. Triggers immediate re-evaluation.
        /// </summary>
        /// <param name="variablePath">The variable path identifying the alarm.</param>
        /// <param name="username">Operator who unshelved the alarm (for audit trail).</param>
        /// <returns>True if the alarm was successfully unshelved.</returns>
        public bool UnshelveAlarm(string variablePath, string username)
        {
            if (!_alarmConditions.TryGetValue(variablePath, out var info))
                return false;

            lock (_shelvingLock)
            {
                if (!info.IsShelved)
                    return false;

                info.IsShelved = false;
                info.ShelvedUntil = null;
                info.ShelvedBy = null;

                // Clear OPC UA SuppressedState
                if (info.AlarmState.SuppressedState != null)
                {
                    info.AlarmState.SuppressedState.Value = new LocalizedText("en", "Unsuppressed");
                    info.AlarmState.SuppressedState.Id.Value = false;
                    info.AlarmState.SuppressedState.TransitionTime.Value = DateTime.UtcNow;
                }
                info.AlarmState.Message.Value = new LocalizedText(
                    $"Unshelved by {username}");
                info.AlarmState.Time.Value = DateTime.UtcNow;
                info.AlarmState.EventId.Value = Guid.NewGuid().ToByteArray();
                ReportAlarmEvent(info.AlarmState);

                // Cancel auto-unshelve timer
                if (_shelvingTimers.TryGetValue(variablePath, out var timer))
                {
                    timer.Dispose();
                    _shelvingTimers.Remove(variablePath);
                }
            }

            _eventLogger?.LogAlarm("Info", variablePath,
                $"Alarm unshelved by {username}");
            _auditTrailLogger?.LogAlarmUnshelve(username, variablePath);

            Utils.Trace("ALARM-UNSHELVED: {0} by {1}", variablePath, username);

            // Re-evaluate immediately — alarm may need to fire now
            EvaluateAlarmCondition(info);
            return true;
        }

        /// <summary>
        /// Returns the current state of all alarms for external consumers (REST API, UI).
        /// </summary>
        public List<AlarmStateSnapshot> GetAlarmStates()
        {
            var result = new List<AlarmStateSnapshot>();
            foreach (var kv in _alarmConditions)
            {
                var info = kv.Value;
                var alarm = info.AlarmState;
                result.Add(new AlarmStateSnapshot
                {
                    VariablePath = info.VariablePath,
                    Message = alarm.Message.Value?.Text ?? info.Config.Message,
                    IsActive = info.IsActive,
                    IsAcknowledged = alarm.AckedState?.Id?.Value == true,
                    IsConfirmed = alarm.ConfirmedState?.Id?.Value == true,
                    IsShelved = info.IsShelved,
                    ShelvedUntil = info.ShelvedUntil,
                    ShelvedBy = info.ShelvedBy,
                    AllowShelving = info.Config.AllowShelving,
                    Severity = alarm.Severity.Value,
                    LastTransitionTime = alarm.Time.Value,
                    Retain = alarm.Retain.Value
                });
            }
            return result;
        }

        /// <summary>DTO representing a point-in-time snapshot of one alarm's state.</summary>
        public class AlarmStateSnapshot
        {
            public string VariablePath { get; set; } = "";
            public string Message { get; set; } = "";
            public bool IsActive { get; set; }
            public bool IsAcknowledged { get; set; }
            public bool IsConfirmed { get; set; }
            public bool IsShelved { get; set; }
            public DateTime? ShelvedUntil { get; set; }
            public string? ShelvedBy { get; set; }
            public bool AllowShelving { get; set; }
            public ushort Severity { get; set; }
            public DateTime LastTransitionTime { get; set; }
            public bool Retain { get; set; }
        }

        // ─── Public accessors for REST API integration ───────────────────

        /// <summary>Acknowledge an alarm from the REST API (bypasses OPC UA session context).</summary>
        public bool AcknowledgeAlarm(string variablePath, string? comment = null)
        {
            if (!_alarmConditions.TryGetValue(variablePath, out var info))
                return false;

            var alarm = info.AlarmState;
            if (alarm.AckedState?.Id?.Value == true)
                return true; // already acknowledged

            alarm.SetAcknowledgedState(SystemContext, true);
            if (!string.IsNullOrEmpty(comment))
                alarm.Comment.Value = new Opc.Ua.LocalizedText(comment);

            alarm.Message.Value = new Opc.Ua.LocalizedText($"Acknowledged: {alarm.ConditionName.Value}");
            alarm.Time.Value = DateTime.UtcNow;

            bool isActive = alarm.ActiveState?.Id?.Value == true;
            bool isConfirmed = alarm.ConfirmedState?.Id?.Value == true;
            if (!isActive && isConfirmed)
                alarm.Retain.Value = false;

            alarm.EventId.Value = Guid.NewGuid().ToByteArray();
            ReportAlarmEvent(alarm);

            _eventLogger?.LogAlarm("Info", alarm.ConditionName.Value ?? "Alarm",
                $"Alarm acknowledged via REST API: {alarm.ConditionName.Value}",
                $"Comment={comment}");

            _auditTrailLogger?.LogAlarmAcknowledge("REST-API",
                alarm.ConditionName.Value ?? "Alarm", comment);

            _notificationManager?.CancelEscalation(variablePath);
            return true;
        }

        /// <summary>Execute a recipe action from the REST API.</summary>
        public void ExecuteRecipe(string recipeName, string action, string? targetRecipeName = null)
        {
            ExecuteRecipeAction(recipeName, action, targetRecipeName ?? "");
        }

        /// <summary>Get the list of configured recipe names.</summary>
        public List<string> GetRecipeNames()
        {
            return _recipeManager?.GetRecipeNames() ?? new List<string>();
        }

        /// <summary>Get the current redundancy status, or null if redundancy is not configured.</summary>
        public RedundancyStatus? GetRedundancyStatus()
        {
            return _redundancyManager?.GetStatus();
        }

        /// <summary>Force this server to become the active node.</summary>
        public bool ForceRedundancyActive(string reason)
        {
            return _redundancyManager?.ForceActive(reason) ?? false;
        }

        /// <summary>Force this server into standby mode.</summary>
        public bool ForceRedundancyStandby(string reason)
        {
            return _redundancyManager?.ForceStandby(reason) ?? false;
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
                _calculatedManager?.Dispose();
                _auditTrailLogger?.Dispose();
                _notificationManager?.Dispose();
                _redundancyManager?.Dispose();
                _restApi?.Dispose();
                _writeRateLimiter?.Dispose();
                _loginRateLimiter?.Dispose();
                _eventLogger?.Dispose();
                lock (_shelvingLock)
                {
                    foreach (var t in _shelvingTimers.Values) t.Dispose();
                    _shelvingTimers.Clear();
                }
                foreach (var rw in _resourceWatchers) rw.Dispose();
                _resourceWatchers.Clear();
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

            /// <summary>Optional linear scaling configuration (raw ↔ engineering).</summary>
            internal ScalingConfig? Scaling { get; set; }

            /// <summary>
            /// When true, a scaling transformation is in progress and re-entrant
            /// OnStateChanged callbacks should be ignored to prevent infinite loops.
            /// </summary>
            internal bool ScalingInProgress { get; set; }

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
                        if ((masks & NodeStateChangeMasks.Value) != 0)
                        {
                            try
                            {
                                if (state is BaseDataVariableState varState)
                                {
                                    var sw = System.Diagnostics.Stopwatch.StartNew();
                                    _logger.Log(varState, _config);
                                    sw.Stop();
                                    DiagnosticsCollector.Instance.RecordCycle("DataLogger", _logger.GetType().Name, sw.Elapsed.TotalMilliseconds);
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

                    // Apply clamping when scaling is configured
                    if (Scaling != null && Scaling.ClampEnabled && incoming != null)
                    {
                        if (double.TryParse(Convert.ToString(incoming, System.Globalization.CultureInfo.InvariantCulture),
                            System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var numVal))
                        {
                            double lo = Math.Min(Scaling.EngMin, Scaling.EngMax);
                            double hi = Math.Max(Scaling.EngMin, Scaling.EngMax);
                            double clamped = Math.Clamp(numVal, lo, hi);
                            if (Math.Abs(clamped - numVal) > 1e-15)
                            {
                                Utils.Trace("WRITE-CLAMPED: {0} from {1} to {2} (range [{3}, {4}])",
                                    NodeId, numVal, clamped, lo, hi);
                                incoming = ConvertToNodeType(clamped);
                            }
                        }
                    }

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

            /// <summary>
            /// Converts a double value back to the OPC UA data type of this variable.
            /// Used after scaling/clamping to preserve the original type (Int32, Float, etc.).
            /// </summary>
            private object ConvertToNodeType(double val)
            {
                if (DataType == DataTypeIds.SByte) return (sbyte)Math.Clamp(val, sbyte.MinValue, sbyte.MaxValue);
                if (DataType == DataTypeIds.Byte) return (byte)Math.Clamp(val, byte.MinValue, byte.MaxValue);
                if (DataType == DataTypeIds.Int16) return (short)Math.Clamp(val, short.MinValue, short.MaxValue);
                if (DataType == DataTypeIds.UInt16) return (ushort)Math.Clamp(val, ushort.MinValue, ushort.MaxValue);
                if (DataType == DataTypeIds.Int32) return (int)Math.Clamp(val, int.MinValue, int.MaxValue);
                if (DataType == DataTypeIds.UInt32) return (uint)Math.Clamp(val, uint.MinValue, uint.MaxValue);
                if (DataType == DataTypeIds.Int64) return (long)Math.Clamp(val, long.MinValue, long.MaxValue);
                if (DataType == DataTypeIds.UInt64) return (ulong)Math.Max(val, 0);
                if (DataType == DataTypeIds.Float) return (float)val;
                return val; // Double or fallback
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
