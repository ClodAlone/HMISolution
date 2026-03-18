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

        private readonly List<NodeId> _rootNodeIds = new();
        private FileSystemWatcher? _watcher;
        private System.Threading.Timer? _reloadTimer;

        private NodeModel? _lastModel;
        private readonly string _configPath;

        // Alarm tracking
        private readonly Dictionary<string, AlarmConditionInfo> _alarmConditions = new();

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

                    // Mark as processed so MasterNodeManager knows we handled it.
                    wv.Processed = true;

                    if (variable is ServerVariableState serverVar)
                    {
                        object v = wv.Value?.Value ?? wv.Value;
                        errors[i] = serverVar.InvokeWrite(SystemContext, variable, ref v) ?? ServiceResult.Good;
                        continue;
                    }

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
            // Cleanup scripts
            if (_scriptManager != null) { _scriptManager.Dispose(); _scriptManager = null; }
            if (_plcManager != null) { _plcManager.Dispose(); _plcManager = null; }
            if (_recipeManager != null) { _recipeManager.Dispose(); _recipeManager = null; }

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

                          // ─── Diagnostics OPC UA node (always created, license-exempt) ───
                          CreateDiagnosticsNode(references);
                      }
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

            // Monitor value changes to activate/deactivate the alarm
            variableState.OnStateChanged += (context, state, masks) =>
            {
                if ((masks & NodeStateChangeMasks.Value) != 0)
                    EvaluateAlarmCondition(info);
            };

            // Evaluate once with the initial value
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
            if (info.Config.TriggerType == AlarmTriggerType.Condition)
                EvaluateConditionAlarm(info);
            else
                EvaluateLimitAlarm(info);
        }

        private void EvaluateLimitAlarm(AlarmConditionInfo info)
        {
            double? numericValue = GetNumericValue(info.SourceVariable.Value);
            if (numericValue == null) return;

            double val = numericValue.Value;
            var cfg = info.Config;
            double highHigh = cfg.HighHighLimit ?? cfg.HighLimit;
            double lowLow = cfg.LowLowLimit ?? cfg.LowLimit;
            double hyst = cfg.Hysteresis;

            // Activation: value crosses the limit
            bool shouldActivate = val > cfg.HighLimit || val < cfg.LowLimit;
            // Deactivation: value must return past the limit by the hysteresis amount
            bool shouldDeactivate = val <= (cfg.HighLimit - hyst) && val >= (cfg.LowLimit + hyst);

            if (shouldActivate && !info.IsActive)
            {
                info.IsActive = true;
                var alarm = info.AlarmState;

                ushort severity;
                LimitAlarmStates limitState;
                string limitText;

                if (val >= highHigh)
                {
                    severity = 900;
                    limitState = LimitAlarmStates.HighHigh;
                    limitText = $"High-High limit ({highHigh}) exceeded";
                }
                else if (val > cfg.HighLimit)
                {
                    severity = 700;
                    limitState = LimitAlarmStates.High;
                    limitText = $"High limit ({cfg.HighLimit}) exceeded";
                }
                else if (val <= lowLow)
                {
                    severity = 900;
                    limitState = LimitAlarmStates.LowLow;
                    limitText = $"Low-Low limit ({lowLow}) violated";
                }
                else
                {
                    severity = 500;
                    limitState = LimitAlarmStates.Low;
                    limitText = $"Low limit ({cfg.LowLimit}) violated";
                }

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
            }
            else if (shouldDeactivate && info.IsActive)
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
            bool shouldActivate = EvaluateConditionExpression(cfg.Operator, cfg.CompareValue, value);

            // Determine deactivation with hysteresis for numeric operators
            bool shouldDeactivate;
            if (info.IsActive && cfg.Hysteresis > 0 && IsNumericOperator(cfg.Operator))
            {
                // Apply hysteresis: use the inverse condition shifted by hysteresis
                shouldDeactivate = EvaluateConditionDeactivation(cfg.Operator, cfg.CompareValue, cfg.Hysteresis, value);
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
            }
            else if (shouldDeactivate && info.IsActive)
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

        private static bool IsNumericOperator(string op) => op is ">" or ">=" or "<" or "<=" or "==" or "!=";

        /// <summary>
        /// Evaluates whether a condition alarm should deactivate, applying hysteresis.
        /// For ">" activation (value > X): deactivate when value &lt;= X − hysteresis.
        /// For "&lt;" activation (value &lt; X): deactivate when value >= X + hysteresis.
        /// For "==" activation: deactivate when |value − X| > hysteresis.
        /// For "!=" activation: deactivate when |value − X| &lt;= hysteresis (back within deadband of target).
        /// </summary>
        private static bool EvaluateConditionDeactivation(string op, string compareValue, double hysteresis, object? value)
        {
            double? numericValue = GetNumericValue(value);
            if (!numericValue.HasValue || !double.TryParse(compareValue, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var target))
                return !EvaluateConditionExpression(op, compareValue, value); // fallback: no hysteresis

            double v = numericValue.Value;
            return op switch
            {
                ">"  => v <= target - hysteresis,
                ">=" => v <  target - hysteresis,
                "<"  => v >= target + hysteresis,
                "<=" => v >  target + hysteresis,
                "==" => Math.Abs(v - target) > hysteresis,
                "!=" => Math.Abs(v - target) <= hysteresis,
                _ => !EvaluateConditionExpression(op, compareValue, value)
            };
        }

        /// <summary>
        /// Evaluates a condition expression against a variable value.
        /// Supports operators: ==, !=, &gt;, &gt;=, &lt;, &lt;=, True, False, Changed.
        /// </summary>
        private static bool EvaluateConditionExpression(string op, string compareValue, object? value)
        {
            if (string.Equals(op, "True", StringComparison.OrdinalIgnoreCase))
            {
                return value switch
                {
                    bool b => b,
                    int i => i != 0,
                    double d => d != 0,
                    string s => string.Equals(s, "true", StringComparison.OrdinalIgnoreCase) || s == "1",
                    _ => false
                };
            }

            if (string.Equals(op, "False", StringComparison.OrdinalIgnoreCase))
            {
                return value switch
                {
                    bool b => !b,
                    int i => i == 0,
                    double d => d == 0,
                    string s => string.Equals(s, "false", StringComparison.OrdinalIgnoreCase) || s == "0" || string.IsNullOrEmpty(s),
                    _ => true
                };
            }

            // For numeric comparisons, try to get numeric values for both sides
            double? numericValue = GetNumericValue(value);
            if (numericValue.HasValue && double.TryParse(compareValue, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var numericCompare))
            {
                double v = numericValue.Value;
                return op switch
                {
                    "==" => Math.Abs(v - numericCompare) < 1e-10,
                    "!=" => Math.Abs(v - numericCompare) >= 1e-10,
                    ">" => v > numericCompare,
                    ">=" => v >= numericCompare,
                    "<" => v < numericCompare,
                    "<=" => v <= numericCompare,
                    _ => false
                };
            }

            // Fall back to string comparison
            string strValue = value?.ToString() ?? "";
            return op switch
            {
                "==" => string.Equals(strValue, compareValue, StringComparison.OrdinalIgnoreCase),
                "!=" => !string.Equals(strValue, compareValue, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        private void ReportAlarmEvent(AlarmConditionState alarm)
        {
            try
            {
                var e = new InstanceStateSnapshot();
                e.Initialize(SystemContext, alarm);
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
                        item.QueueEvent(e);
                    }
                }
            }
            return ServiceResult.Good;
        }

        private static double? GetNumericValue(object? value)
        {
            return value switch
            {
                double d => d,
                float f => f,
                int i => i,
                short s => s,
                ushort us => us,
                uint ui => ui,
                long l => l,
                ulong ul => ul,
                decimal m => (double)m,
                byte b => b,
                sbyte sb => sb,
                string str when double.TryParse(str, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var parsed) => parsed,
                _ => null
            };
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
                _eventLogger?.Dispose();
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

            public ServerVariableState(NodeState parent, IVariableLogger? logger, DataLoggingConfig? config, SimpleFileServerNodeManager manager) : base(parent)
            {
                _logger = logger;
                _config = config;
                _manager = manager;

                OnSimpleWriteValue = HandleWriteValue;

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
                return HandleWriteValue(context, node, ref value);
            }

            private ServiceResult HandleWriteValue(ISystemContext context, NodeState node, ref object value)
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
