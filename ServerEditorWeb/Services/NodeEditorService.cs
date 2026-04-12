using System.Text.Json;
using SharedModels;
using ServerEditorWeb.Models;

namespace ServerEditorWeb.Services;

public class NodeEditorService
{
    private UndoRedoService? _undoRedo;

    // --- Multi-project state ------------------------------------
    private readonly List<ProjectNode> _openProjects = new();

    /// <summary>The currently active project node (bold in tree).</summary>
    public ProjectNode? ActiveProject => _openProjects.FirstOrDefault(p => p.IsActive);

    /// <summary>All currently open projects.</summary>
    public IReadOnlyList<ProjectNode> OpenProjects => _openProjects;

    // Private aliases so existing code keeps compiling against the active project
    private NodeModel? _rootModel => ActiveProject?.Model;
    private string _nodesPath
    {
        get => ActiveProject?.FilePath ?? "";
        set { if (ActiveProject != null) ActiveProject.FilePath = value; }
    }

    public List<TreeNode> RootItems { get; } = new();
    public TreeNode? SelectedItem { get; set; }
    public List<TreeNode> SelectedItems { get; } = new();
    public bool HasUnsavedChanges
    {
        get => ActiveProject?.HasUnsavedChanges ?? false;
        set { if (ActiveProject != null) ActiveProject.HasUnsavedChanges = value; }
    }
    /// <summary>True if ANY open project has unsaved changes.</summary>
    public bool AnyUnsavedChanges => _openProjects.Any(p => p.HasUnsavedChanges);
    public string JsonEditorText { get; set; } = "";
    public string DiffText { get; set; } = "";
    public string ServerEndpointUrl { get; set; } = "opc.tcp://localhost:14840/SimpleOpcFileServer";
    public List<string> RecentFiles { get; } = new();
    public string CurrentFilePath => ActiveProject?.FilePath ?? "";
    public NodeModel? RootModel => ActiveProject?.Model;

    /// <summary>Inject the UndoRedoService so structural changes can be undone.</summary>
    public void SetUndoService(UndoRedoService undoRedo) => _undoRedo = undoRedo;

    public event Action? StateChanged;
    public event Action? UserSymbolGroupsChanged;
    public void NotifyUserSymbolGroupsChanged() => UserSymbolGroupsChanged?.Invoke();

    // Screen symbol selection (shared between ScreenEditorPanel and ScreenSymbolProperties)
    public ScreenSymbol? SelectedScreenSymbol { get; private set; }
    public IReadOnlyList<ScreenSymbol> SelectedScreenSymbols { get; private set; } = [];
    public event Action? ScreenSelectionChanged;
    public event Action? AnimationPreviewTick;
    public void NotifyAnimationPreviewTick() => AnimationPreviewTick?.Invoke();

    public void SetScreenSelection(ScreenSymbol? primary, IEnumerable<ScreenSymbol> all)
    {
        SelectedScreenSymbol = primary;
        SelectedScreenSymbols = all.ToList();
        ScreenSelectionChanged?.Invoke();
    }

    // Report section selection (shared between ReportEditorPanel and NodeProperties)
    public ReportSection? SelectedReportSection { get; private set; }
    public event Action? ReportSectionSelectionChanged;

    public void SetReportSectionSelection(ReportSection? section)
    {
        SelectedReportSection = section;
        ReportSectionSelectionChanged?.Invoke();
    }

    private void NotifyStateChanged() => StateChanged?.Invoke();

    public NodeEditorService()
    {
        LoadSettings();
    }

    /// <summary>
    /// True once the previous session has been fully restored.
    /// </summary>
    public bool IsSessionRestored { get; private set; }

    /// <summary>
    /// Restores previously open projects asynchronously so the UI can render first.
    /// Call this once from OnAfterRenderAsync(firstRender).
    /// </summary>
    public async Task RestorePreviousSessionAsync(Action<string>? onProgress = null)
    {
        if (IsSessionRestored) return;
        IsSessionRestored = true;

        if (_pendingOpenPaths.Count > 0)
        {
            for (int i = 0; i < _pendingOpenPaths.Count; i++)
            {
                var path = _pendingOpenPaths[i];
                if (!File.Exists(path)) continue;
                onProgress?.Invoke($"Restoring project {i + 1}/{_pendingOpenPaths.Count}: {Path.GetFileName(path)}\u2026");
                await OpenProjectAsync(path, onProgress);
            }
            if (!string.IsNullOrEmpty(_pendingActivePath))
            {
                var active = _openProjects.FirstOrDefault(p =>
                    string.Equals(p.FilePath, _pendingActivePath, StringComparison.OrdinalIgnoreCase));
                if (active != null)
                    SetActiveProject(active);
            }
        }
        else if (RecentFiles.Count > 0)
        {
            var first = RecentFiles[0];
            if (File.Exists(first))
            {
                onProgress?.Invoke($"Restoring {Path.GetFileName(first)}\u2026");
                await OpenProjectAsync(first, onProgress);
            }
        }

        NotifyStateChanged();
    }

    // Populated by LoadSettings(), consumed by RestorePreviousSessionAsync
    private List<string> _pendingOpenPaths = new();
    private string? _pendingActivePath;

    public void NewFile()
    {
        var model = new NodeModel
        {
            Folder = new Folder { Name = "Root" },
            Server = new ServerSettings(),
            Screens = new List<ScreenConfig>()
        };
        var proj = new ProjectNode(model, "") { Name = "New Project" };
        AddProjectNode(proj);
        ServerEndpointUrl = model.Server.EndpointUrl;
        HasUnsavedChanges = true;
        NotifyStateChanged();
    }

    public void NewFileFromModel(NodeModel model)
    {
        model.Folder ??= new Folder { Name = "Root" };
        model.Server ??= new ServerSettings();
        model.Screens ??= new List<ScreenConfig>();
        var proj = new ProjectNode(model, "") { Name = "New Project" };
        AddProjectNode(proj);
        ServerEndpointUrl = model.Server.EndpointUrl;
        HasUnsavedChanges = true;
        NotifyStateChanged();
    }

    public string[] GetAvailableFiles()
    {
        // Look for nodes.json files in common locations
        var files = new List<string>();
        var baseDir = AppContext.BaseDirectory;

        // Traverse up to find solution root
        var dir = new DirectoryInfo(baseDir);
        for (int i = 0; i < 6; i++)
        {
            if (dir == null) break;
            if (Directory.Exists(Path.Combine(dir.FullName, "Server")))
            {
                var serverDir = Path.Combine(dir.FullName, "Server");
                foreach (var f in Directory.GetFiles(serverDir, "*.json", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetFileName(f).Contains("nodes", StringComparison.OrdinalIgnoreCase))
                        files.Add(f);
                }
                break;
            }
            dir = dir.Parent;
        }

        foreach (var recent in RecentFiles)
        {
            if (File.Exists(recent) && !files.Contains(recent))
                files.Add(recent);
        }

        return files.ToArray();
    }

    /// <summary>
    /// Returns the logical drive letters available on the server (e.g. C:\, D:\).
    /// </summary>
    public string[] GetDrives()
    {
        try
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => d.RootDirectory.FullName)
                .ToArray();
        }
        catch { return []; }
    }

    /// <summary>
    /// Lists directories and JSON files in the given path.
    /// Returns (directories, files) as full paths.
    /// </summary>
    public (string[] directories, string[] files) ListDirectory(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return ([], []);

            var dirs = Directory.GetDirectories(path)
                .Where(d =>
                {
                    try { var info = new DirectoryInfo(d); return !info.Attributes.HasFlag(FileAttributes.Hidden) && !info.Attributes.HasFlag(FileAttributes.System); }
                    catch { return false; }
                })
                .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var files = Directory.GetFiles(path, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return (dirs, files);
        }
        catch
        {
            return ([], []);
        }
    }

    /// <summary>
    /// Returns a sensible starting directory for the file browser.
    /// </summary>
    public string GetBrowseStartPath()
    {
        if (!string.IsNullOrEmpty(_nodesPath))
            return Path.GetDirectoryName(_nodesPath) ?? "";

        if (RecentFiles.Count > 0)
        {
            var dir = Path.GetDirectoryName(RecentFiles[0]);
            if (dir != null && Directory.Exists(dir)) return dir;
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    public (bool success, string message) LoadFromFile(string path) => OpenProject(path);

    /// <summary>
    /// Asynchronously opens a project file, reporting progress at each stage.
    /// </summary>
    public async Task<(bool success, string message)> OpenProjectAsync(string path, Action<string>? onProgress = null)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
                return (false, "No path specified.");

            string fullPath = Path.GetFullPath(path);

            var existing = _openProjects.FirstOrDefault(p =>
                string.Equals(p.FilePath, fullPath, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                SetActiveProject(existing);
                return (true, $"{existing.Name} is already open.");
            }

            if (!File.Exists(path))
                return (false, $"File not found: {path}");

            var fileInfo = new FileInfo(path);
            double sizeMb = fileInfo.Length / (1024.0 * 1024.0);
            onProgress?.Invoke($"Reading file ({sizeMb:F1} MB)\u2026");

            var json = await Task.Run(() => File.ReadAllText(path));

            onProgress?.Invoke("Parsing project model\u2026");
            var model = await Task.Run(() => JsonSerializer.Deserialize<NodeModel>(json));
            if (model == null)
                return (false, "Failed to parse JSON.");

            int varCount = CountVariables(model.Folder);
            onProgress?.Invoke($"Loading resources ({varCount:N0} variables)\u2026");

            await Task.Run(() => ResourceFileManager.LoadExternalResources(model, path));
            model.Server ??= new ServerSettings();

            CrashReporter.ConfigureEmail(model.Server.CrashEmail);

            var licFile = LicenseManager.FindLicenseFile(path);
            LicenseManager.Validate(licFile);

            onProgress?.Invoke("Building project tree\u2026");

            var proj = new ProjectNode(model, fullPath);
            AddProjectNode(proj);
            ServerEndpointUrl = model.Server.EndpointUrl;

            RecentFiles.Remove(fullPath);
            RecentFiles.Insert(0, fullPath);
            while (RecentFiles.Count > 10) RecentFiles.RemoveAt(RecentFiles.Count - 1);
            SaveSettings();

            return (true, $"Loaded {Path.GetFileName(path)}");
        }
        catch (Exception ex)
        {
            return (false, $"Error loading {path}: {ex.Message}");
        }
    }

    private static int CountVariables(Folder? folder)
    {
        if (folder == null) return 0;
        int count = folder.Variables?.Count ?? 0;
        if (folder.Folders != null)
            foreach (var sub in folder.Folders)
                count += CountVariables(sub);
        return count;
    }

    /// <summary>
    /// Opens a project file and adds it to the tree. If already open, just activates it.
    /// </summary>
    public (bool success, string message) OpenProject(string path)
    {
        try
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fullPath = Path.GetFullPath(path);

                // If already open, just activate it
                var existing = _openProjects.FirstOrDefault(p =>
                    string.Equals(p.FilePath, fullPath, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    SetActiveProject(existing);
                    return (true, $"{existing.Name} is already open.");
                }

                if (!File.Exists(path))
                    return (false, $"File not found: {path}");

                var json = File.ReadAllText(path);
                var model = JsonSerializer.Deserialize<NodeModel>(json);
                if (model == null)
                    return (false, "Failed to parse JSON.");

                // Load external resource files (scripts/, screens/, plcprograms/)
                ResourceFileManager.LoadExternalResources(model, path);
                model.Server ??= new ServerSettings();

                // Configure crash email from project settings
                CrashReporter.ConfigureEmail(model.Server.CrashEmail);

                // Validate license
                var licFile = LicenseManager.FindLicenseFile(path);
                LicenseManager.Validate(licFile);

                var proj = new ProjectNode(model, fullPath);
                AddProjectNode(proj);
                ServerEndpointUrl = model.Server.EndpointUrl;

                // Recent files
                RecentFiles.Remove(fullPath);
                RecentFiles.Insert(0, fullPath);
                while (RecentFiles.Count > 10) RecentFiles.RemoveAt(RecentFiles.Count - 1);
                SaveSettings();

                return (true, $"Loaded {Path.GetFileName(path)}");
            }
            return (false, "No path specified.");
        }
        catch (Exception ex)
        {
            return (false, $"Error loading {path}: {ex.Message}");
        }
    }

    /// <summary>
    /// Closes a project and removes it from the tree.
    /// If it was active, activates another project or clears the tree.
    /// </summary>
    public void CloseProject(ProjectNode project)
    {
        if (project.HasUnsavedChanges)
        {
            // Let the caller handle save prompts; for now just close
        }
        _openProjects.Remove(project);
        RootItems.Remove(project);

        if (project.IsActive && _openProjects.Count > 0)
        {
            SetActiveProject(_openProjects[0]);
        }
        else if (_openProjects.Count == 0)
        {
            SelectedItem = null;
            SelectedItems.Clear();
        }
        SaveSettings();
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets the given project as the active one (bold in tree).
    /// </summary>
    public void SetActiveProject(ProjectNode project)
    {
        foreach (var p in _openProjects)
            p.IsActive = false;
        project.IsActive = true;
        ServerEndpointUrl = project.Model.Server?.EndpointUrl ?? "opc.tcp://localhost:14840/SimpleOpcFileServer";
        NotifyStateChanged();
    }

    /// <summary>
    /// Internal helper: adds a ProjectNode, builds its children, makes it active, and rebuilds the tree.
    /// </summary>
    private void AddProjectNode(ProjectNode proj)
    {
        foreach (var p in _openProjects)
            p.IsActive = false;
        proj.IsActive = true;
        _openProjects.Add(proj);
        BuildProjectChildren(proj);
        RebuildFullTree();
    }

    /// <summary>
    /// Rebuilds the children of a single ProjectNode from its Model.
    /// </summary>
    private void BuildProjectChildren(ProjectNode proj)
    {
        proj.Children.Clear();

        // Server settings node
        var serverSettingsNode = new ServerSettingsNode(proj.Model.Server) { Parent = proj };
        proj.Children.Add(serverSettingsNode);

        var variableGroup = new VariableGroupNode() { Parent = proj };
        if (proj.Model.Folder != null)
        {
            var rootVm = CreateFolderNode(proj.Model.Folder);
            rootVm.IsExpanded = true;
            rootVm.Parent = variableGroup;
            variableGroup.Children.Add(rootVm);
        }
        variableGroup.IsExpanded = true;
        proj.Children.Add(variableGroup);

        var scriptGroup = new ScriptGroupNode() { Parent = proj };
        if (proj.Model.Scripts != null)
        {
            BuildResourceTree(scriptGroup, proj.Model.Scripts, "Script",
                s => s.Group, s => new ScriptNode(s) { });
        }
        proj.Children.Add(scriptGroup);

        var plcGroup = new PlcGroupNode() { Parent = proj };
        if (proj.Model.PlcPrograms != null)
        {
            BuildResourceTree(plcGroup, proj.Model.PlcPrograms, "PlcProgram",
                p => p.Group, p => new PlcProgramNode(p) { });
        }
        proj.Children.Add(plcGroup);

        var recipeGroup = new RecipeGroupNode() { Parent = proj };
        if (proj.Model.Recipes != null)
        {
            foreach (var recipe in proj.Model.Recipes)
            {
                var rNode = new RecipeNode(recipe) { Parent = recipeGroup };
                recipeGroup.Children.Add(rNode);
            }
        }
        proj.Children.Add(recipeGroup);

        var schedulerGroup = new SchedulerGroupNode() { Parent = proj };
        if (proj.Model.Schedulers != null)
        {
            foreach (var scheduler in proj.Model.Schedulers)
            {
                var sNode = new SchedulerNode(scheduler) { Parent = schedulerGroup };
                schedulerGroup.Children.Add(sNode);
            }
        }
        proj.Children.Add(schedulerGroup);

        var reportGroup = new ReportGroupNode() { Parent = proj };
        if (proj.Model.Reports != null)
        {
            foreach (var report in proj.Model.Reports)
            {
                var rptNode = new ReportNode(report) { Parent = reportGroup };
                reportGroup.Children.Add(rptNode);
            }
        }
        proj.Children.Add(reportGroup);

        var calcGroup = new CalculatedGroupNode() { Parent = proj };
        if (proj.Model.CalculatedVariables != null)
        {
            foreach (var calc in proj.Model.CalculatedVariables)
            {
                var cNode = new CalculatedNode(calc) { Parent = calcGroup };
                calcGroup.Children.Add(cNode);
            }
        }
        proj.Children.Add(calcGroup);

        var assetGroup = new AssetGroupNode() { Parent = proj };
        if (proj.Model.Assets != null)
        {
            foreach (var asset in proj.Model.Assets)
            {
                var aNode = new AssetNode(asset) { Parent = assetGroup };
                assetGroup.Children.Add(aNode);
            }
        }
        proj.Children.Add(assetGroup);

        var batchGroup = new BatchGroupNode() { Parent = proj };
        if (proj.Model.BatchSequences != null)
        {
            foreach (var batch in proj.Model.BatchSequences)
            {
                var bNode = new BatchNode(batch) { Parent = batchGroup };
                batchGroup.Children.Add(bNode);
            }
        }
        proj.Children.Add(batchGroup);

        var eventGroup = new EventGroupNode() { Parent = proj };
        if (proj.Model.Events != null)
        {
            foreach (var evt in proj.Model.Events)
            {
                var eNode = new EventNode(evt) { Parent = eventGroup };
                eventGroup.Children.Add(eNode);
            }
        }
        proj.Children.Add(eventGroup);

        var aliasMapGroup = new AliasMapGroupNode() { Parent = proj };
        if (proj.Model.AliasMaps != null)
        {
            foreach (var map in proj.Model.AliasMaps)
            {
                var mNode = new AliasMapNode(map) { Parent = aliasMapGroup };
                aliasMapGroup.Children.Add(mNode);
            }
        }
        proj.Children.Add(aliasMapGroup);

        var screenGroup = new ScreenGroupNode() { Parent = proj };
        if (proj.Model.Screens != null)
        {
            BuildResourceTree(screenGroup, proj.Model.Screens, "Screen",
                s => s.Group, s => new ScreenNode(s) { });
        }
        proj.Children.Add(screenGroup);

        var userGroupList = new UserGroupListNode() { Parent = proj };
        if (proj.Model.UserGroups != null)
        {
            foreach (var group in proj.Model.UserGroups)
            {
                var gNode = new UserGroupNode(group) { Parent = userGroupList };
                if (proj.Model.Users != null)
                {
                    foreach (var user in proj.Model.Users.Where(u => u.Group == group.Name))
                    {
                        var uNode = new UserNode(user) { Parent = gNode };
                        gNode.Children.Add(uNode);
                    }
                }
                userGroupList.Children.Add(gNode);
            }
            if (proj.Model.Users != null)
            {
                var assignedGroups = proj.Model.UserGroups.Select(g => g.Name).ToHashSet();
                foreach (var user in proj.Model.Users.Where(u => !assignedGroups.Contains(u.Group)))
                {
                    var uNode = new UserNode(user) { Parent = userGroupList };
                    userGroupList.Children.Add(uNode);
                }
            }
        }
        else if (proj.Model.Users is { Count: > 0 })
        {
            foreach (var user in proj.Model.Users)
            {
                var uNode = new UserNode(user) { Parent = userGroupList };
                userGroupList.Children.Add(uNode);
            }
        }
        proj.Children.Add(userGroupList);
    }

    /// <summary>
    /// Rebuilds RootItems from all open ProjectNodes.
    /// </summary>
    private void RebuildFullTree()
    {
        RootItems.Clear();
        SelectedItems.Clear();
        SelectedItem = null;
        foreach (var proj in _openProjects)
            RootItems.Add(proj);
        NotifyStateChanged();
    }


    public (bool success, string message) Save()
    {
        if (_rootModel == null || string.IsNullOrEmpty(_nodesPath))
            return (false, "No file loaded.");

        try
        {
            RebuildModelStructure();
            MigratePasswords();

            // Save resources to external files (scripts/, screens/, plcprograms/)
            ResourceFileManager.SaveExternalResources(_rootModel, _nodesPath);

            // Temporarily clear inline lists so the main JSON stays clean
            var snapshot = ResourceFileManager.DetachResources(_rootModel);
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_rootModel, options);
                File.WriteAllText(_nodesPath, json);
            }
            finally
            {
                // Restore in-memory lists so the editor keeps working
                ResourceFileManager.ReattachResources(_rootModel, snapshot);
            }

            HasUnsavedChanges = false;
            NotifyStateChanged();
            return (true, $"Saved to {_nodesPath}");
        }
        catch (Exception ex)
        {
            return (false, $"Error saving: {ex.Message}");
        }
    }

    public (bool success, string message) SaveAs(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
        {
            var existingJsonFiles = Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly);
            bool hasOtherProject = existingJsonFiles.Any(f =>
                !string.Equals(Path.GetFullPath(f), fullPath, StringComparison.OrdinalIgnoreCase));

            if (hasOtherProject)
                return (false, $"The folder already contains a project. Choose a different folder.");
        }

        _nodesPath = path;
        if (ActiveProject != null) ActiveProject.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        return Save();
    }

    public void SetServerEndpointUrl(string url)
    {
        ServerEndpointUrl = url;
        if (_rootModel != null)
        {
            _rootModel.Server ??= new ServerSettings();
            _rootModel.Server.EndpointUrl = url;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    // --- Project Lock / Unlock ----------------------------------

    /// <summary>Whether the active project is currently locked.</summary>
    public bool IsActiveProjectLocked => ActiveProject?.IsLocked ?? false;

    /// <summary>Whether the active project has a password set (even if currently unlocked).</summary>
    public bool ActiveProjectHasPassword => !string.IsNullOrEmpty(_rootModel?.ProjectPasswordHash);

    /// <summary>
    /// Sets (or changes) the project protection password.
    /// If <paramref name="password"/> is empty, the protection is removed.
    /// </summary>
    public (bool success, string message) SetProjectPassword(string password, string? currentPassword = null)
    {
        if (_rootModel == null || ActiveProject == null)
            return (false, "No project loaded.");

        // If a password is already set, verify the current password first
        if (!string.IsNullOrEmpty(_rootModel.ProjectPasswordHash))
        {
            if (string.IsNullOrEmpty(currentPassword) || !PasswordHasher.Verify(currentPassword, _rootModel.ProjectPasswordHash))
                return (false, "Current password is incorrect.");
        }

        if (string.IsNullOrEmpty(password))
        {
            // Remove protection
            _rootModel.ProjectPasswordHash = "";
            ActiveProject.IsLocked = false;
            HasUnsavedChanges = true;
            NotifyStateChanged();
            return (true, "Project protection removed.");
        }

        _rootModel.ProjectPasswordHash = PasswordHasher.Hash(password);
        ActiveProject.IsLocked = false; // Keep unlocked after setting
        HasUnsavedChanges = true;
        NotifyStateChanged();
        return (true, "Project password set. The project will be locked when reopened.");
    }

    /// <summary>
    /// Attempts to unlock the active project with the given password.
    /// </summary>
    public (bool success, string message) UnlockProject(string password)
    {
        if (_rootModel == null || ActiveProject == null)
            return (false, "No project loaded.");

        if (!ActiveProject.IsLocked)
            return (true, "Project is already unlocked.");

        if (string.IsNullOrEmpty(_rootModel.ProjectPasswordHash))
        {
            ActiveProject.IsLocked = false;
            NotifyStateChanged();
            return (true, "Project unlocked.");
        }

        if (PasswordHasher.Verify(password, _rootModel.ProjectPasswordHash))
        {
            ActiveProject.IsLocked = false;
            NotifyStateChanged();
            return (true, "Project unlocked.");
        }

        return (false, "Incorrect password.");
    }

    /// <summary>
    /// Re-locks the active project (requires it to have a password set).
    /// </summary>
    public void LockProject()
    {
        if (ActiveProject != null && !string.IsNullOrEmpty(_rootModel?.ProjectPasswordHash))
        {
            ActiveProject.IsLocked = true;
            NotifyStateChanged();
        }
    }

    public void AddFolder()
    {
        var parent = SelectedItem as FolderNode
            ?? (SelectedItem is VariableGroupNode vg ? vg.Children.OfType<FolderNode>().FirstOrDefault() : null);
        if (parent != null)
        {
            var newFolder = new Folder { Name = "New Folder" };
            var newNode = new FolderNode(newFolder) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddVariable()
    {
        var parent = SelectedItem as FolderNode
            ?? (SelectedItem is VariableGroupNode vg ? vg.Children.OfType<FolderNode>().FirstOrDefault() : null);
        if (parent != null)
        {
            var newVar = new Variable { Name = "New Variable", Type = "Double", Value = 0.0, Access = "ReadWrite" };
            var newNode = new VariableNode(newVar) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddScript()
    {
        var parent = SelectedItem is ResourceFolderNode rf && rf.ResourceKind == "Script"
            ? (TreeNode)rf
            : SelectedItem is ScriptGroupNode ? SelectedItem : null;
        if (parent != null)
        {
            var newScript = new ScriptConfig { Name = "New Script", Code = "var val = Read(\"MyVar\");\nWrite(\"MyVar\", 123);", IntervalMs = 1000, Enabled = true };
            var newNode = new ScriptNode(newScript) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddPlcProgram()
    {
        var parent = SelectedItem is ResourceFolderNode rf && rf.ResourceKind == "PlcProgram"
            ? (TreeNode)rf
            : SelectedItem is PlcGroupNode ? SelectedItem : null;
        if (parent != null)
        {
            var newPlc = new PlcProgramConfig
            {
                Name = "New Program",
                Enabled = true,
                IntervalMs = 100,
                Code = "VAR\n    counter : INT := 0;\nEND_VAR\n\ncounter := counter + 1;\n"
            };
            var newNode = new PlcProgramNode(newPlc) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddScreen()
    {
        var parent = SelectedItem is ResourceFolderNode rf && rf.ResourceKind == "Screen"
            ? (TreeNode)rf
            : SelectedItem is ScreenGroupNode ? SelectedItem : null;
        if (parent != null)
        {
            var newScreen = new ScreenConfig { Name = "New Screen", Width = 800, Height = 600 };
            var newNode = new ScreenNode(newScreen) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddRecipe()
    {
        if (SelectedItem is RecipeGroupNode parent)
        {
            var newRecipe = new RecipeConfig
            {
                Name = "New Recipe",
                Enabled = true,
                Variables = new List<RecipeVariable>()
            };
            var newNode = new RecipeNode(newRecipe) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddScheduler()
    {
        if (SelectedItem is SchedulerGroupNode parent)
        {
            var newScheduler = new SchedulerConfig
            {
                Name = "New Scheduler",
                Enabled = true,
                SlotMinutes = 60,
                WeekendMode = "Same",
                HolidayMode = "Same"
            };
            var newNode = new SchedulerNode(newScheduler) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddReport()
    {
        if (SelectedItem is ReportGroupNode parent)
        {
            var newReport = new ReportConfig
            {
                Name = "New Report",
                Enabled = true,
                Format = "HTML",
                Title = "New Report"
            };
            var newNode = new ReportNode(newReport) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddCalculated()
    {
        if (SelectedItem is CalculatedGroupNode parent)
        {
            var newCalc = new CalculatedVariableConfig
            {
                Name = "NewCalculated",
                Expression = "",
                Type = "Double",
                IntervalMs = 1000,
                Enabled = true,
                FolderPath = "_Calculated"
            };
            var newNode = new CalculatedNode(newCalc) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddAsset()
    {
        if (SelectedItem is AssetGroupNode parent)
        {
            var newAsset = new AssetConfig
            {
                Name = "New Asset",
                Enabled = true
            };
            var newNode = new AssetNode(newAsset) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddBatch()
    {
        if (SelectedItem is BatchGroupNode parent)
        {
            var newBatch = new BatchSequenceConfig
            {
                Name = "New Sequence",
                Enabled = true
            };
            var newNode = new BatchNode(newBatch) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }


    public void AddEvent()
    {
        if (SelectedItem is EventGroupNode parent)
        {
            var newEvent = new EventConfig
            {
                Name = "New Event",
                Enabled = true
            };
            var newNode = new EventNode(newEvent) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddAliasMap()
    {
        if (SelectedItem is AliasMapGroupNode parent)
        {
            var newMap = new VariableAliasMap
            {
                Name = "New Alias Map"
            };
            var newNode = new AliasMapNode(newMap) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            _rootModel?.AliasMaps.Add(newMap);
            SetSingleSelection(newNode);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Adds a resource folder
    /// </summary>
    public void AddResourceFolder()
    {
        string? kind = null;
        TreeNode? parent = null;

        switch (SelectedItem)
        {
            case ScriptGroupNode:
                kind = "Script"; parent = SelectedItem; break;
            case PlcGroupNode:
                kind = "PlcProgram"; parent = SelectedItem; break;
            case ScreenGroupNode:
                kind = "Screen"; parent = SelectedItem; break;
            case ResourceFolderNode rf:
                kind = rf.ResourceKind; parent = rf; break;
        }

        if (kind != null && parent != null)
        {
            var folder = new ResourceFolderNode("New Folder", kind) { Parent = parent };
            // Insert folders before leaf items
            var insertIdx = parent.Children.Count(c => c is ResourceFolderNode);
            parent.Children.Insert(insertIdx, folder);
            parent.IsExpanded = true;
            SetSingleSelection(folder);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddUserGroup()
    {
        if (SelectedItem is UserGroupListNode parent)
        {
            var newGroup = new UserGroupConfig { Name = "New Group", AccessLevel = "Read" };
            var newNode = new UserGroupNode(newGroup) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SetSingleSelection(newNode);
            _rootModel?.UserGroups.Add(newGroup);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddUser()
    {
        // Add a user to the currently selected group
        if (SelectedItem is UserGroupNode groupNode)
        {
            var newUser = new UserConfig { Username = "newuser", Group = groupNode.UserGroup.Name };
            var newNode = new UserNode(newUser) { Parent = groupNode };
            groupNode.Children.Add(newNode);
            groupNode.IsExpanded = true;
            SetSingleSelection(newNode);
            _rootModel?.Users.Add(newUser);
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    /// <summary>Migrate any legacy plain-text passwords to hashed passwords before save.</summary>
    public void MigratePasswords()
    {
        if (_rootModel?.Users == null) return;
        foreach (var user in _rootModel.Users)
        {
            if (!string.IsNullOrEmpty(user.Password) && string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.Hash(user.Password);
                user.Password = "";
            }
        }
    }

    public void DeleteSelected()
    {
        var toDelete = SelectedItems.Where(n => n.Parent != null).ToList();
        if (toDelete.Count == 0) return;

        // Capture state for undo: each deleted item's parent, index, and any model objects
        var deletedInfo = new List<(TreeNode Item, TreeNode Parent, int Index, UserConfig? User, UserGroupConfig? Group, List<UserConfig>? GroupUsers)>();

        foreach (var item in toDelete)
        {
            var parent = item.Parent!;
            var index = parent.Children.IndexOf(item);

            UserConfig? user = null;
            UserGroupConfig? group = null;
            List<UserConfig>? groupUsers = null;

            if (item is UserNode un)
            {
                user = un.User;
                _rootModel?.Users.Remove(un.User);
            }
            else if (item is UserGroupNode ugn)
            {
                group = ugn.UserGroup;
                groupUsers = ugn.Children.OfType<UserNode>().Select(u => u.User).ToList();
                _rootModel?.UserGroups.Remove(ugn.UserGroup);
                foreach (var child in ugn.Children.OfType<UserNode>())
                    _rootModel?.Users.Remove(child.User);
            }
            else if (item is AliasMapNode amn)
            {
                _rootModel?.AliasMaps.Remove(amn.AliasMap);
            }

            deletedInfo.Add((item, parent, index, user, group, groupUsers));
            parent.Children.Remove(item);
            item.IsSelected = false;
        }

        var description = toDelete.Count == 1
            ? $"Delete {toDelete[0].Name}"
            : $"Delete {toDelete.Count} items";

        _undoRedo?.RecordAction(new CollectionAction
        {
            Description = description,
            UndoCallback = () =>
            {
                // Re-insert in reverse order so indices stay valid
                for (int i = deletedInfo.Count - 1; i >= 0; i--)
                {
                    var (item, parent, index, user, group, groupUsers) = deletedInfo[i];
                    var insertAt = Math.Min(index, parent.Children.Count);
                    parent.Children.Insert(insertAt, item);
                    item.Parent = parent;

                    if (group != null)
                    {
                        _rootModel?.UserGroups.Add(group);
                        if (groupUsers != null)
                            foreach (var u in groupUsers)
                                _rootModel?.Users.Add(u);
                    }
                    else if (user != null)
                    {
                        _rootModel?.Users.Add(user);
                    }
                    if (item is AliasMapNode amnUndo)
                        _rootModel?.AliasMaps.Add(amnUndo.AliasMap);
                }
                NotifyStateChanged();
            },
            RedoCallback = () =>
            {
                foreach (var (item, parent, _, user, group, groupUsers) in deletedInfo)
                {
                    if (user != null)
                        _rootModel?.Users.Remove(user);
                    if (group != null)
                    {
                        _rootModel?.UserGroups.Remove(group);
                        if (groupUsers != null)
                            foreach (var u in groupUsers)
                                _rootModel?.Users.Remove(u);
                    }
                    if (item is AliasMapNode amnRedo)
                        _rootModel?.AliasMaps.Remove(amnRedo.AliasMap);
                    parent.Children.Remove(item);
                    item.IsSelected = false;
                }
                SelectedItems.Clear();
                SelectedItem = null;
                NotifyStateChanged();
            }
        });

        SelectedItems.Clear();
        SelectedItem = null;
        HasUnsavedChanges = true;
        NotifyStateChanged();
    }

    public void CopySelected(ClipboardService clipboard)
    {
        // Multi-copy: if multiple homogeneous nodes are selected, copy them all
        if (SelectedItems.Count > 1)
        {
            CopyMultiple(clipboard);
            return;
        }

        switch (SelectedItem)
        {
            case VariableNode vn:
                clipboard.CopyVariable(vn.Variable);
                break;
            case FolderNode fn:
                UpdateFolderModel(fn);
                clipboard.CopyFolder(fn.Folder);
                break;
            case ScriptNode sn:
                sn.SyncName();
                clipboard.CopyScript(sn.Script);
                break;
            case PlcProgramNode pn:
                pn.SyncName();
                clipboard.CopyPlcProgram(pn.PlcProgram);
                break;
            case ScreenNode scn:
                scn.SyncName();
                clipboard.CopyScreen(scn.Screen);
                break;
            case RecipeNode rn:
                rn.SyncName();
                clipboard.CopyRecipe(rn.Recipe);
                break;
            case ResourceFolderNode rf:
                CopyResourceFolder(rf, clipboard);
                break;
        }
    }

    private void CopyResourceFolder(ResourceFolderNode folder, ClipboardService clipboard)
    {
        switch (folder.ResourceKind)
        {
            case "Script":
            {
                var items = new List<ScriptConfig>();
                CollectResourceItems<ScriptNode, ScriptConfig>(folder, "", n => { n.SyncName(); return n.Script; },
                    (item, group) => { }, items);
                if (items.Count > 0) clipboard.CopyScripts(items);
                break;
            }
            case "PlcProgram":
            {
                var items = new List<PlcProgramConfig>();
                CollectResourceItems<PlcProgramNode, PlcProgramConfig>(folder, "", n => { n.SyncName(); return n.PlcProgram; },
                    (item, group) => { }, items);
                if (items.Count > 0) clipboard.CopyPlcPrograms(items);
                break;
            }
            case "Screen":
            {
                var items = new List<ScreenConfig>();
                CollectResourceItems<ScreenNode, ScreenConfig>(folder, "", n => { n.SyncName(); return n.Screen; },
                    (item, group) => { }, items);
                if (items.Count > 0) clipboard.CopyScreens(items);
                break;
            }
        }
    }

    private void CopyMultiple(ClipboardService clipboard)
    {
        // Only copy if all selected items are of the same type
        var first = SelectedItems[0];
        if (!SelectedItems.All(n => n.GetType() == first.GetType())) return;

        switch (first)
        {
            case VariableNode:
                clipboard.CopyVariables(SelectedItems.Cast<VariableNode>().Select(n => n.Variable).ToList());
                break;
            case FolderNode:
                foreach (var fn in SelectedItems.Cast<FolderNode>()) UpdateFolderModel(fn);
                clipboard.CopyFolders(SelectedItems.Cast<FolderNode>().Select(n => n.Folder).ToList());
                break;
            case ScriptNode:
                foreach (var sn in SelectedItems.Cast<ScriptNode>()) sn.SyncName();
                clipboard.CopyScripts(SelectedItems.Cast<ScriptNode>().Select(n => n.Script).ToList());
                break;
            case PlcProgramNode:
                foreach (var pn in SelectedItems.Cast<PlcProgramNode>()) pn.SyncName();
                clipboard.CopyPlcPrograms(SelectedItems.Cast<PlcProgramNode>().Select(n => n.PlcProgram).ToList());
                break;
            case ScreenNode:
                foreach (var scn in SelectedItems.Cast<ScreenNode>()) scn.SyncName();
                clipboard.CopyScreens(SelectedItems.Cast<ScreenNode>().Select(n => n.Screen).ToList());
                break;
            case RecipeNode:
                foreach (var rn in SelectedItems.Cast<RecipeNode>()) rn.SyncName();
                clipboard.CopyRecipes(SelectedItems.Cast<RecipeNode>().Select(n => n.Recipe).ToList());
                break;
        }
    }

    public void PasteFromClipboard(ClipboardService clipboard)
    {
        // Resolve VariableGroupNode to its first FolderNode child for paste targets
        var target = SelectedItem;
        if (target is VariableGroupNode vg)
            target = vg.Children.OfType<FolderNode>().FirstOrDefault();

        switch (clipboard.ContentType)
        {
            case "Variable" when target is FolderNode parentFolder:
            {
                var newVar = clipboard.PasteVariable();
                if (newVar == null) return;
                var newNode = new VariableNode(newVar) { Parent = parentFolder };
                parentFolder.Children.Add(newNode);
                parentFolder.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Folder" when target is FolderNode parentFolder2:
            {
                var newFolder = clipboard.PasteFolder();
                if (newFolder == null) return;
                var newNode = CreateFolderNode(newFolder);
                newNode.Parent = parentFolder2;
                parentFolder2.Children.Add(newNode);
                parentFolder2.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Script" when SelectedItem is ScriptGroupNode or ResourceFolderNode { ResourceKind: "Script" }:
            {
                var pasteTarget = SelectedItem!;
                var newScript = clipboard.PasteScript();
                if (newScript == null) return;
                var newNode = new ScriptNode(newScript) { Parent = pasteTarget };
                pasteTarget.Children.Add(newNode);
                pasteTarget.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "PlcProgram" when SelectedItem is PlcGroupNode or ResourceFolderNode { ResourceKind: "PlcProgram" }:
            {
                var pasteTarget = SelectedItem!;
                var newPlc = clipboard.PastePlcProgram();
                if (newPlc == null) return;
                var newNode = new PlcProgramNode(newPlc) { Parent = pasteTarget };
                pasteTarget.Children.Add(newNode);
                pasteTarget.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Screen" when SelectedItem is ScreenGroupNode or ResourceFolderNode { ResourceKind: "Screen" }:
            {
                var pasteTarget = SelectedItem!;
                var newScreen = clipboard.PasteScreen();
                if (newScreen == null) return;
                var newNode = new ScreenNode(newScreen) { Parent = pasteTarget };
                pasteTarget.Children.Add(newNode);
                pasteTarget.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Recipe" when SelectedItem is RecipeGroupNode recipeGroup:
            {
                var newRecipe = clipboard.PasteRecipe();
                if (newRecipe == null) return;
                var newNode = new RecipeNode(newRecipe) { Parent = recipeGroup };
                recipeGroup.Children.Add(newNode);
                recipeGroup.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }

            case "Scheduler" when SelectedItem is SchedulerGroupNode schedulerGroup:
            {
                var newSch = clipboard.PasteScheduler();
                if (newSch == null) return;
                var newSchNode = new SchedulerNode(newSch) { Parent = schedulerGroup };
                schedulerGroup.Children.Add(newSchNode);
                schedulerGroup.IsExpanded = true;
                SelectedItem = newSchNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Report" when SelectedItem is ReportGroupNode reportGroup:
            {
                var newRpt = clipboard.PasteReport();
                if (newRpt == null) return;
                var newRptNode = new ReportNode(newRpt) { Parent = reportGroup };
                reportGroup.Children.Add(newRptNode);
                reportGroup.IsExpanded = true;
                SelectedItem = newRptNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Event" when SelectedItem is EventGroupNode eventGroup:
            {
                var newEvt = clipboard.PasteEvent();
                if (newEvt == null) return;
                var newEvtNode = new EventNode(newEvt) { Parent = eventGroup };
                eventGroup.Children.Add(newEvtNode);
                eventGroup.IsExpanded = true;
                SelectedItem = newEvtNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "AliasMap" when SelectedItem is AliasMapGroupNode aliasGroup:
            {
                var newMap = clipboard.PasteAliasMap();
                if (newMap == null) return;
                var newMapNode = new AliasMapNode(newMap) { Parent = aliasGroup };
                aliasGroup.Children.Add(newMapNode);
                aliasGroup.IsExpanded = true;
                _rootModel?.AliasMaps.Add(newMap);
                SelectedItem = newMapNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Calculated" when SelectedItem is CalculatedGroupNode calcGroup:
            {
                var newCalc = clipboard.PasteCalculated();
                if (newCalc == null) return;
                var newCalcNode = new CalculatedNode(newCalc) { Parent = calcGroup };
                calcGroup.Children.Add(newCalcNode);
                calcGroup.IsExpanded = true;
                SelectedItem = newCalcNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Asset" when SelectedItem is AssetGroupNode assetGroup:
            {
                var newAsset = clipboard.PasteAsset();
                if (newAsset == null) return;
                var newAssetNode = new AssetNode(newAsset) { Parent = assetGroup };
                assetGroup.Children.Add(newAssetNode);
                assetGroup.IsExpanded = true;
                SelectedItem = newAssetNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Batch" when SelectedItem is BatchGroupNode batchGroup:
            {
                var newBatch = clipboard.PasteBatch();
                if (newBatch == null) return;
                var newBatchNode = new BatchNode(newBatch) { Parent = batchGroup };
                batchGroup.Children.Add(newBatchNode);
                batchGroup.IsExpanded = true;
                SelectedItem = newBatchNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }

            // Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡ Multi-paste cases Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡Ã”Ã¶Ã‡
            case "Variables" when target is FolderNode pf:
            {
                var items = clipboard.PasteVariables();
                if (items == null) return;
                foreach (var v in items)
                {
                    var n = new VariableNode(v) { Parent = pf };
                    pf.Children.Add(n);
                }
                pf.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Folders" when target is FolderNode pf2:
            {
                var items = clipboard.PasteFolders();
                if (items == null) return;
                foreach (var f in items)
                {
                    var n = CreateFolderNode(f);
                    n.Parent = pf2;
                    pf2.Children.Add(n);
                }
                pf2.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Scripts" when SelectedItem is ScriptGroupNode or ResourceFolderNode { ResourceKind: "Script" }:
            {
                var pt = SelectedItem!;
                var items = clipboard.PasteScripts();
                if (items == null) return;
                foreach (var s in items)
                    pt.Children.Add(new ScriptNode(s) { Parent = pt });
                pt.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "PlcPrograms" when SelectedItem is PlcGroupNode or ResourceFolderNode { ResourceKind: "PlcProgram" }:
            {
                var pt = SelectedItem!;
                var items = clipboard.PastePlcPrograms();
                if (items == null) return;
                foreach (var p in items)
                    pt.Children.Add(new PlcProgramNode(p) { Parent = pt });
                pt.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Screens" when SelectedItem is ScreenGroupNode or ResourceFolderNode { ResourceKind: "Screen" }:
            {
                var pt = SelectedItem!;
                var items = clipboard.PasteScreens();
                if (items == null) return;
                foreach (var s in items)
                    pt.Children.Add(new ScreenNode(s) { Parent = pt });
                pt.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Recipes" when SelectedItem is RecipeGroupNode rg:
            {
                var items = clipboard.PasteRecipes();
                if (items == null) return;
                foreach (var r in items)
                    rg.Children.Add(new RecipeNode(r) { Parent = rg });
                rg.IsExpanded = true;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
        }
    }

    public void SelectNode(TreeNode? node, bool ctrl = false, bool shift = false)
    {
        if (node == null)
        {
            ClearSelection();
            return;
        }

        if (ctrl)
        {
            // Toggle the node in the multi-selection
            if (SelectedItems.Contains(node))
            {
                node.IsSelected = false;
                SelectedItems.Remove(node);
                SelectedItem = SelectedItems.LastOrDefault();
            }
            else
            {
                node.IsSelected = true;
                SelectedItems.Add(node);
                SelectedItem = node;
            }
        }
        else if (shift && SelectedItem != null)
        {
            // Range select: find siblings between the anchor and the target
            var siblings = GetSiblings(SelectedItem, node);
            if (siblings != null)
            {
                // Clear previous selection
                foreach (var n in SelectedItems)
                    n.IsSelected = false;
                SelectedItems.Clear();

                foreach (var n in siblings)
                {
                    n.IsSelected = true;
                    SelectedItems.Add(n);
                }
                SelectedItem = node;
            }
            else
            {
                // Not siblings â€” just select the new node
                SetSingleSelection(node);
            }
        }
        else
        {
            SetSingleSelection(node);
        }

        NotifyStateChanged();
    }

    public void ClearSelection()
    {
        foreach (var n in SelectedItems)
            n.IsSelected = false;
        SelectedItems.Clear();
        if (SelectedItem != null)
            SelectedItem.IsSelected = false;
        SelectedItem = null;
        NotifyStateChanged();
    }

    private void SetSingleSelection(TreeNode node)
    {
        foreach (var n in SelectedItems)
            n.IsSelected = false;
        SelectedItems.Clear();

        node.IsSelected = true;
        SelectedItems.Add(node);
        SelectedItem = node;

        // Clear report section selection when switching nodes
        if (SelectedReportSection != null && node is not ReportNode)
            SetReportSectionSelection(null);
    }

    /// <summary>Returns the range of sibling nodes between a and b (inclusive), or null if not siblings.</summary>
    private static List<TreeNode>? GetSiblings(TreeNode a, TreeNode b)
    {
        if (a.Parent == null || b.Parent == null) return null;
        if (a.Parent != b.Parent) return null;

        var children = a.Parent.Children;
        int idxA = children.IndexOf(a);
        int idxB = children.IndexOf(b);
        if (idxA < 0 || idxB < 0) return null;

        int start = Math.Min(idxA, idxB);
        int end = Math.Max(idxA, idxB);
        return children.GetRange(start, end - start + 1);
    }

    public void RefreshJsonFromTree()
    {
        if (_rootModel == null) return;
        RebuildModelStructure();
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonEditorText = JsonSerializer.Serialize(_rootModel, options);
            NotifyStateChanged();
        }
        catch { }
    }

    public (bool success, string message) ApplyJsonToTree()
    {
        try
        {
            var newModel = JsonSerializer.Deserialize<NodeModel>(JsonEditorText);
            if (newModel != null)
            {
                if (ActiveProject != null) ActiveProject.Model = newModel;
                ReloadViewModels();
                HasUnsavedChanges = true;
                NotifyStateChanged();
                return (true, "JSON applied successfully.");
            }
            return (false, "Failed to parse JSON.");
        }
        catch (Exception ex)
        {
            return (false, $"Error applying JSON: {ex.Message}");
        }
    }

    public void MarkChanged()
    {
        HasUnsavedChanges = true;
        NotifyStateChanged();
    }

    /// <summary>
    /// Renames a variable or folder and propagates the change across all references
    /// in screens, scripts, PLC programs, recipes, reports, schedulers, etc.
    /// Returns the number of references that were updated.
    /// </summary>
    /// <summary>
    /// Renames a variable or folder (from the rename dialog) and propagates all references.
    /// </summary>
    public int RenameVariableOrFolder(TreeNode node, string newName)
    {
        if (_rootModel == null || string.IsNullOrWhiteSpace(newName))
            return 0;

        string oldPath = GetFullPath(node);

        node.Name = newName;
        if (node is VariableNode vn) { vn.SyncName(); vn.AcceptName(); }
        else if (node is FolderNode fn) { fn.SyncName(); fn.AcceptName(); }

        string newPath = GetFullPath(node);
        int refCount = 0;
        if (!oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
            refCount = VariableRenameService.RenameAll(_rootModel, oldPath, newPath);

        HasUnsavedChanges = true;
        NotifyStateChanged();
        return refCount;
    }

    /// <summary>
    /// Checks all selected tree nodes for name changes (e.g. from PropertyGrid edits)
    /// and propagates renames automatically. Returns a message if any renames were propagated.
    /// </summary>
    public string? DetectAndPropagateRenames()
    {
        if (_rootModel == null) return null;

        int totalRefs = 0;
        int renamedItems = 0;

        foreach (var node in SelectedItems)
        {
            if (node is VariableNode vn && vn.Name != vn.PreviousName)
            {
                var oldPath = GetFullPathWithName(vn, vn.PreviousName);
                vn.SyncName();
                var newPath = GetFullPath(vn);
                vn.AcceptName();
                if (!oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                { totalRefs += VariableRenameService.RenameAll(_rootModel, oldPath, newPath); renamedItems++; }
            }
            else if (node is FolderNode fn && fn.Name != fn.PreviousName)
            {
                var oldPath = GetFullPathWithName(fn, fn.PreviousName);
                fn.SyncName();
                var newPath = GetFullPath(fn);
                fn.AcceptName();
                if (!oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                { totalRefs += VariableRenameService.RenameAll(_rootModel, oldPath, newPath); renamedItems++; }
            }
        }

        if (renamedItems == 0) return null;
        return totalRefs > 0
            ? $"Updated {totalRefs} reference(s) for {renamedItems} renamed item(s)."
            : $"Renamed {renamedItems} item(s). No references to update.";
    }

    /// <summary>Computes the full path but substitutes the node's name with a specific value.</summary>
    private static string GetFullPathWithName(TreeNode node, string nameOverride)
    {
        var parts = new List<string>();
        var current = node.Parent;
        while (current != null)
        {
            if (current is FolderNode or VariableNode)
                parts.Add(current.Name);
            current = current.Parent;
        }
        parts.Reverse();
        parts.Add(nameOverride);
        return string.Join(".", parts);
    }

    public static string GetFullPath(TreeNode node)
    {
        var parts = new List<string>();
        var current = node;
        while (current != null)
        {
            if (current is FolderNode or VariableNode)
                parts.Add(current.Name);
            current = current.Parent;
        }
        parts.Reverse();
        return string.Join(".", parts);
    }

    private void ReloadViewModels()
    {
        if (ActiveProject != null)
        {
            BuildProjectChildren(ActiveProject);
            RebuildFullTree();
        }
    }

    private FolderNode CreateFolderNode(Folder folder)
    {
        var node = new FolderNode(folder);
        foreach (var subFolder in folder.Folders)
        {
            var child = CreateFolderNode(subFolder);
            child.Parent = node;
            node.Children.Add(child);
        }
        foreach (var variable in folder.Variables)
        {
            var child = new VariableNode(variable) { Parent = node };
            node.Children.Add(child);
        }
        return node;
    }

    /// <summary>
    /// Builds a folder tree from a flat list of resource items using their Group path.
    /// Items with an empty Group go directly under the parent; others are nested in ResourceFolderNodes.
    /// </summary>
    private static void BuildResourceTree<T>(TreeNode parent, List<T> items, string resourceKind,
        Func<T, string> getGroup, Func<T, TreeNode> createNode)
    {
        // Cache of group-path â†’ folder node
        var folderCache = new Dictionary<string, ResourceFolderNode>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            var group = getGroup(item)?.Trim() ?? "";
            TreeNode target;

            if (string.IsNullOrEmpty(group))
            {
                target = parent;
            }
            else
            {
                target = EnsureResourceFolder(parent, group, resourceKind, folderCache);
            }

            var node = createNode(item);
            node.Parent = target;
            target.Children.Add(node);
        }
    }

    /// <summary>
    /// Ensures the folder hierarchy exists for a given group path (e.g. "Alarms/Temperature").
    /// Creates intermediate ResourceFolderNodes as needed.
    /// </summary>
    private static ResourceFolderNode EnsureResourceFolder(TreeNode root, string groupPath,
        string resourceKind, Dictionary<string, ResourceFolderNode> cache)
    {
        if (cache.TryGetValue(groupPath, out var existing))
            return existing;

        var parts = groupPath.Split('/');
        TreeNode current = root;
        var pathSoFar = "";

        foreach (var part in parts)
        {
            pathSoFar = string.IsNullOrEmpty(pathSoFar) ? part : $"{pathSoFar}/{part}";

            if (!cache.TryGetValue(pathSoFar, out var folder))
            {
                // Look for an existing child folder with this name
                folder = current.Children.OfType<ResourceFolderNode>()
                    .FirstOrDefault(f => f.Name.Equals(part, StringComparison.OrdinalIgnoreCase));

                if (folder == null)
                {
                    folder = new ResourceFolderNode(part, resourceKind) { Parent = current };
                    // Insert folders before leaf items
                    var insertIdx = current.Children.Count(c => c is ResourceFolderNode);
                    current.Children.Insert(insertIdx, folder);
                }

                cache[pathSoFar] = folder;
            }

            current = folder;
        }

        return cache[groupPath];
    }

    private void RebuildModelStructure()
    {
        var children = ActiveProject?.Children ?? RootItems;
        foreach (var root in children)
        {
            if (root is VariableGroupNode vgNode && _rootModel != null)
            {
                // The actual folder tree is the first child of the VariableGroupNode
                var fNode = vgNode.Children.OfType<FolderNode>().FirstOrDefault();
                if (fNode != null)
                {
                    _rootModel.Folder = fNode.Folder;
                    UpdateFolderModel(fNode);
                }
            }
            else if (root is FolderNode fNode2 && _rootModel != null)
            {
                // Legacy: direct FolderNode as root (backward compat)
                _rootModel.Folder = fNode2.Folder;
                UpdateFolderModel(fNode2);
            }
            else if (root is ScriptGroupNode sgNode && _rootModel != null)
            {
                _rootModel.Scripts.Clear();
                CollectResourceItems<ScriptNode, ScriptConfig>(sgNode, "", n =>
                {
                    n.SyncName();
                    return n.Script;
                }, (item, group) => item.Group = group, _rootModel.Scripts);
            }
            else if (root is PlcGroupNode pgNode && _rootModel != null)
            {
                _rootModel.PlcPrograms.Clear();
                CollectResourceItems<PlcProgramNode, PlcProgramConfig>(pgNode, "", n =>
                {
                    n.SyncName();
                    return n.PlcProgram;
                }, (item, group) => item.Group = group, _rootModel.PlcPrograms);
            }
            else if (root is RecipeGroupNode rgNode && _rootModel != null)
            {
                _rootModel.Recipes.Clear();
                foreach (var child in rgNode.Children)
                {
                    if (child is RecipeNode rNode)
                    {
                        rNode.SyncName();
                        _rootModel.Recipes.Add(rNode.Recipe);
                    }
                }
            }
            else if (root is SchedulerGroupNode schGrpNode && _rootModel != null)
            {
                _rootModel.Schedulers.Clear();
                foreach (var child in schGrpNode.Children)
                {
                    if (child is SchedulerNode schNode)
                    {
                        schNode.SyncName();
                        _rootModel.Schedulers.Add(schNode.Scheduler);
                    }
                }
            }
            else if (root is ReportGroupNode rptGrpNode && _rootModel != null)
            {
                _rootModel.Reports.Clear();
                foreach (var child in rptGrpNode.Children)
                {
                    if (child is ReportNode rptNode)
                    {
                        rptNode.SyncName();
                        _rootModel.Reports.Add(rptNode.Report);
                    }
                }
            }
            else if (root is CalculatedGroupNode calcGrpNode && _rootModel != null)
            {
                _rootModel.CalculatedVariables.Clear();
                foreach (var child in calcGrpNode.Children)
                {
                    if (child is CalculatedNode cNode)
                    {
                        cNode.SyncName();
                        _rootModel.CalculatedVariables.Add(cNode.Config);
                    }
                }
            }
            else if (root is AssetGroupNode assetGrpNode && _rootModel != null)
            {
                _rootModel.Assets.Clear();
                foreach (var child in assetGrpNode.Children)
                {
                    if (child is AssetNode aNode)
                    {
                        aNode.SyncName();
                        _rootModel.Assets.Add(aNode.Asset);
                    }
                }
            }
            else if (root is BatchGroupNode batchGrpNode && _rootModel != null)
            {
                _rootModel.BatchSequences.Clear();
                foreach (var child in batchGrpNode.Children)
                {
                    if (child is BatchNode bNode)
                    {
                        bNode.SyncName();
                        _rootModel.BatchSequences.Add(bNode.Batch);
                    }
                }
            }
            else if (root is EventGroupNode eventGrpNode && _rootModel != null)
            {
                _rootModel.Events.Clear();
                foreach (var child in eventGrpNode.Children)
                {
                    if (child is EventNode eNode)
                    {
                        eNode.SyncName();
                        _rootModel.Events.Add(eNode.Event);
                    }
                }
            }
            else if (root is AliasMapGroupNode aliasGrpNode && _rootModel != null)
            {
                _rootModel.AliasMaps.Clear();
                foreach (var child in aliasGrpNode.Children)
                {
                    if (child is AliasMapNode mNode)
                    {
                        mNode.SyncName();
                        _rootModel.AliasMaps.Add(mNode.AliasMap);
                    }
                }
            }
            else if (root is ScreenGroupNode scrNode && _rootModel != null)
            {
                _rootModel.Screens.Clear();
                CollectResourceItems<ScreenNode, ScreenConfig>(scrNode, "", n =>
                {
                    n.SyncName();
                    return n.Screen;
                }, (item, group) => item.Group = group, _rootModel.Screens);

                // Sync camera configs from ipcamera symbols to top-level Cameras list
                _rootModel.Cameras.Clear();
                foreach (var screen in _rootModel.Screens)
                {
                    foreach (var sym in screen.Symbols)
                    {
                        if (sym.Type == "ipcamera" && sym.Camera != null && !string.IsNullOrEmpty(sym.Camera.CameraId))
                        {
                            if (!_rootModel.Cameras.Any(c => c.CameraId == sym.Camera.CameraId))
                                _rootModel.Cameras.Add(sym.Camera);
                        }
                    }
                }
            }
            else if (root is UserGroupListNode ugListNode && _rootModel != null)
            {
                _rootModel.UserGroups.Clear();
                _rootModel.Users.Clear();

                foreach (var child in ugListNode.Children)
                {
                    if (child is UserGroupNode gNode)
                    {
                        gNode.SyncName();
                        _rootModel.UserGroups.Add(gNode.UserGroup);

                        foreach (var uChild in gNode.Children)
                        {
                            if (uChild is UserNode uNode)
                            {
                                uNode.SyncName();
                                _rootModel.Users.Add(uNode.User);
                            }
                        }
                    }
                    else if (child is UserNode orphanUser)
                    {
                        orphanUser.SyncName();
                        _rootModel.Users.Add(orphanUser.User);
                    }
                }
            }
        }
    }

    private void UpdateFolderModel(FolderNode fNode)
    {
        fNode.SyncName();
        fNode.Folder.Folders.Clear();
        fNode.Folder.Variables.Clear();

        foreach (var child in fNode.Children)
        {
            if (child is FolderNode childFolder)
            {
                fNode.Folder.Folders.Add(childFolder.Folder);
                UpdateFolderModel(childFolder);
            }
            else if (child is VariableNode childVar)
            {
                childVar.SyncName();
                fNode.Folder.Variables.Add(childVar.Variable);
            }
        }
    }

    /// <summary>
    /// Recursively collects resource items from a tree that may contain ResourceFolderNodes,
    /// flattening them into a list and setting the Group path on each item.
    /// </summary>
    private static void CollectResourceItems<TNode, TConfig>(TreeNode parent, string groupPath,
        Func<TNode, TConfig> getConfig, Action<TConfig, string> setGroup, List<TConfig> result)
        where TNode : TreeNode
    {
        foreach (var child in parent.Children)
        {
            if (child is ResourceFolderNode folder)
            {
                var childPath = string.IsNullOrEmpty(groupPath)
                    ? folder.Name
                    : $"{groupPath}/{folder.Name}";
                CollectResourceItems<TNode, TConfig>(folder, childPath, getConfig, setGroup, result);
            }
            else if (child is TNode itemNode)
            {
                var config = getConfig(itemNode);
                setGroup(config, groupPath);
                result.Add(config);
            }
        }
    }

    private void LoadSettings()
    {
        var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleOpcFileServer", "editor_settings.json");
        try
        {
            if (File.Exists(settingsPath))
            {
                var settings = JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(settingsPath));
                if (settings?.RecentFiles != null)
                {
                    foreach (var file in settings.RecentFiles)
                    {
                        if (File.Exists(file) && !RecentFiles.Contains(file))
                            RecentFiles.Add(file);
                    }
                }
                if (settings?.OpenProjectPaths != null)
                {
                    _pendingOpenPaths = settings.OpenProjectPaths;
                    _pendingActivePath = settings.ActiveProjectPath;
                }
            }
        }
        catch { }
    }

    private void SaveSettings()
    {
        var settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SimpleOpcFileServer", "editor_settings.json");
        try
        {
            var dir = Path.GetDirectoryName(settingsPath);
            if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var settings = new EditorSettings
            {
                RecentFiles = new List<string>(RecentFiles),
                OpenProjectPaths = _openProjects.Where(p => !string.IsNullOrEmpty(p.FilePath)).Select(p => p.FilePath).ToList(),
                ActiveProjectPath = ActiveProject?.FilePath
            };
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(settings));
        }
        catch { }
    }

    private class EditorSettings
    {
        public List<string> RecentFiles { get; set; } = new();
        public List<string> OpenProjectPaths { get; set; } = new();
        public string? ActiveProjectPath { get; set; }
    }
}
