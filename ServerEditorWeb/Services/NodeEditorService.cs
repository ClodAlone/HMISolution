using System.Text.Json;
using SharedModels;
using ServerEditorWeb.Models;

namespace ServerEditorWeb.Services;

public class NodeEditorService
{
    private string _nodesPath = "";
    private NodeModel? _rootModel;

    public List<TreeNode> RootItems { get; } = new();
    public TreeNode? SelectedItem { get; set; }
    public bool HasUnsavedChanges { get; set; }
    public string JsonEditorText { get; set; } = "";
    public string DiffText { get; set; } = "";
    public string ServerEndpointUrl { get; set; } = "opc.tcp://localhost:14840/SimpleOpcFileServer";
    public List<string> RecentFiles { get; } = new();
    public string CurrentFilePath => _nodesPath;
    public NodeModel? RootModel => _rootModel;

    public event Action? StateChanged;

    // Screen symbol selection (shared between ScreenEditorPanel and ScreenSymbolProperties)
    public ScreenSymbol? SelectedScreenSymbol { get; private set; }
    public IReadOnlyList<ScreenSymbol> SelectedScreenSymbols { get; private set; } = [];
    public event Action? ScreenSelectionChanged;

    public void SetScreenSelection(ScreenSymbol? primary, IEnumerable<ScreenSymbol> all)
    {
        SelectedScreenSymbol = primary;
        SelectedScreenSymbols = all.ToList();
        ScreenSelectionChanged?.Invoke();
    }

    private void NotifyStateChanged() => StateChanged?.Invoke();

    public NodeEditorService()
    {
        LoadSettings();
        if (!string.IsNullOrEmpty(_nodesPath) && File.Exists(_nodesPath))
        {
            LoadFromFile(_nodesPath);
            HasUnsavedChanges = false;
        }
    }

    public void NewFile()
    {
        _rootModel = new NodeModel
        {
            Folder = new Folder { Name = "Root" },
            Server = new ServerSettings(),
            Screens = new List<ScreenConfig>()
        };
        _nodesPath = "";
        ServerEndpointUrl = _rootModel.Server.EndpointUrl;
        ReloadViewModels();
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

    public (bool success, string message) LoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
                return (false, $"File not found: {path}");

            var json = File.ReadAllText(path);
            _rootModel = JsonSerializer.Deserialize<NodeModel>(json);

            if (_rootModel == null)
                return (false, "Failed to parse JSON.");

            // Load external resource files (scripts/, screens/, plcprograms/)
            ResourceFileManager.LoadExternalResources(_rootModel, path);

            _rootModel.Server ??= new ServerSettings();
            ServerEndpointUrl = _rootModel.Server.EndpointUrl;
            _nodesPath = path;

            // Configure crash email from project settings
            CrashReporter.ConfigureEmail(_rootModel.Server.CrashEmail);

            // Validate license
            var licFile = LicenseManager.FindLicenseFile(path);
            LicenseManager.Validate(licFile);

            // Recent files
            string fullPath = Path.GetFullPath(path);
            RecentFiles.Remove(fullPath);
            RecentFiles.Insert(0, fullPath);
            while (RecentFiles.Count > 10) RecentFiles.RemoveAt(RecentFiles.Count - 1);
            SaveSettings();

            ReloadViewModels();
            HasUnsavedChanges = false;
            NotifyStateChanged();
            return (true, $"Loaded {Path.GetFileName(path)}");
        }
        catch (Exception ex)
        {
            return (false, $"Error loading {path}: {ex.Message}");
        }
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
        _nodesPath = path;
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
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddScript()
    {
        if (SelectedItem is ScriptGroupNode parent)
        {
            var newScript = new ScriptConfig { Name = "New Script", Code = "var val = Read(\"MyVar\");\nWrite(\"MyVar\", 123);", IntervalMs = 1000, Enabled = true };
            var newNode = new ScriptNode(newScript) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddPlcProgram()
    {
        if (SelectedItem is PlcGroupNode parent)
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
            SelectedItem = newNode;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void AddScreen()
    {
        if (SelectedItem is ScreenGroupNode parent)
        {
            var newScreen = new ScreenConfig { Name = "New Screen", Width = 800, Height = 600 };
            var newNode = new ScreenNode(newScreen) { Parent = parent };
            parent.Children.Add(newNode);
            parent.IsExpanded = true;
            SelectedItem = newNode;
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
            SelectedItem = newNode;
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
            SelectedItem = newNode;
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
            SelectedItem = newNode;
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
        if (SelectedItem?.Parent != null)
        {
            // Keep RootModel lists in sync when deleting user/group nodes
            if (SelectedItem is UserNode un)
                _rootModel?.Users.Remove(un.User);
            else if (SelectedItem is UserGroupNode ugn)
            {
                _rootModel?.UserGroups.Remove(ugn.UserGroup);
                // Also remove users belonging to this group
                foreach (var child in ugn.Children.OfType<UserNode>())
                    _rootModel?.Users.Remove(child.User);
            }

            SelectedItem.Parent.Children.Remove(SelectedItem);
            SelectedItem = null;
            HasUnsavedChanges = true;
            NotifyStateChanged();
        }
    }

    public void CopySelected(ClipboardService clipboard)
    {
        switch (SelectedItem)
        {
            case VariableNode vn:
                clipboard.CopyVariable(vn.Variable);
                break;
            case FolderNode fn:
                // Sync the folder model before copying
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
            case "Script" when SelectedItem is ScriptGroupNode scriptGroup:
            {
                var newScript = clipboard.PasteScript();
                if (newScript == null) return;
                var newNode = new ScriptNode(newScript) { Parent = scriptGroup };
                scriptGroup.Children.Add(newNode);
                scriptGroup.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "PlcProgram" when SelectedItem is PlcGroupNode plcGroup:
            {
                var newPlc = clipboard.PastePlcProgram();
                if (newPlc == null) return;
                var newNode = new PlcProgramNode(newPlc) { Parent = plcGroup };
                plcGroup.Children.Add(newNode);
                plcGroup.IsExpanded = true;
                SelectedItem = newNode;
                HasUnsavedChanges = true;
                NotifyStateChanged();
                break;
            }
            case "Screen" when SelectedItem is ScreenGroupNode screenGroup:
            {
                var newScreen = clipboard.PasteScreen();
                if (newScreen == null) return;
                var newNode = new ScreenNode(newScreen) { Parent = screenGroup };
                screenGroup.Children.Add(newNode);
                screenGroup.IsExpanded = true;
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
        }
    }

    public void SelectNode(TreeNode? node)
    {
        if (SelectedItem != null)
            SelectedItem.IsSelected = false;

        SelectedItem = node;
        if (node != null)
            node.IsSelected = true;

        NotifyStateChanged();
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
                _rootModel = newModel;
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

    private void ReloadViewModels()
    {
        RootItems.Clear();

        var variableGroup = new VariableGroupNode();
        if (_rootModel?.Folder != null)
        {
            var rootVm = CreateFolderNode(_rootModel.Folder);
            rootVm.IsExpanded = true;
            rootVm.Parent = variableGroup;
            variableGroup.Children.Add(rootVm);
        }
        variableGroup.IsExpanded = true;
        RootItems.Add(variableGroup);

        var scriptGroup = new ScriptGroupNode();
        if (_rootModel?.Scripts != null)
        {
            foreach (var script in _rootModel.Scripts)
            {
                var sNode = new ScriptNode(script) { Parent = scriptGroup };
                scriptGroup.Children.Add(sNode);
            }
        }
        RootItems.Add(scriptGroup);

        var plcGroup = new PlcGroupNode();
        if (_rootModel?.PlcPrograms != null)
        {
            foreach (var plc in _rootModel.PlcPrograms)
            {
                var pNode = new PlcProgramNode(plc) { Parent = plcGroup };
                plcGroup.Children.Add(pNode);
            }
        }
        RootItems.Add(plcGroup);

        var recipeGroup = new RecipeGroupNode();
        if (_rootModel?.Recipes != null)
        {
            foreach (var recipe in _rootModel.Recipes)
            {
                var rNode = new RecipeNode(recipe) { Parent = recipeGroup };
                recipeGroup.Children.Add(rNode);
            }
        }
        RootItems.Add(recipeGroup);

        var screenGroup = new ScreenGroupNode();
        if (_rootModel?.Screens != null)
        {
            foreach (var screen in _rootModel.Screens)
            {
                var scNode = new ScreenNode(screen) { Parent = screenGroup };
                screenGroup.Children.Add(scNode);
            }
        }
        RootItems.Add(screenGroup);

        var userGroupList = new UserGroupListNode();
        if (_rootModel?.UserGroups != null)
        {
            foreach (var group in _rootModel.UserGroups)
            {
                var gNode = new UserGroupNode(group) { Parent = userGroupList };

                // Add users belonging to this group as children
                if (_rootModel.Users != null)
                {
                    foreach (var user in _rootModel.Users.Where(u => u.Group == group.Name))
                    {
                        var uNode = new UserNode(user) { Parent = gNode };
                        gNode.Children.Add(uNode);
                    }
                }

                userGroupList.Children.Add(gNode);
            }

            // Add users with no matching group (orphans)
            if (_rootModel.Users != null)
            {
                var assignedGroups = _rootModel.UserGroups.Select(g => g.Name).ToHashSet();
                foreach (var user in _rootModel.Users.Where(u => !assignedGroups.Contains(u.Group)))
                {
                    var uNode = new UserNode(user) { Parent = userGroupList };
                    userGroupList.Children.Add(uNode);
                }
            }
        }
        else if (_rootModel?.Users is { Count: > 0 })
        {
            foreach (var user in _rootModel.Users)
            {
                var uNode = new UserNode(user) { Parent = userGroupList };
                userGroupList.Children.Add(uNode);
            }
        }
        RootItems.Add(userGroupList);
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

    private void RebuildModelStructure()
    {
        foreach (var root in RootItems)
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
                foreach (var child in sgNode.Children)
                {
                    if (child is ScriptNode sNode)
                        _rootModel.Scripts.Add(sNode.Script);
                }
            }
            else if (root is PlcGroupNode pgNode && _rootModel != null)
            {
                _rootModel.PlcPrograms.Clear();
                foreach (var child in pgNode.Children)
                {
                    if (child is PlcProgramNode pNode)
                    {
                        pNode.SyncName();
                        _rootModel.PlcPrograms.Add(pNode.PlcProgram);
                    }
                }
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
            else if (root is ScreenGroupNode scrNode && _rootModel != null)
            {
                _rootModel.Screens.Clear();
                foreach (var child in scrNode.Children)
                {
                    if (child is ScreenNode scNode)
                    {
                        scNode.SyncName();
                        _rootModel.Screens.Add(scNode.Screen);
                    }
                }

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
                    if (RecentFiles.Count > 0)
                        _nodesPath = RecentFiles[0];
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
            var settings = new EditorSettings { RecentFiles = new List<string>(RecentFiles) };
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(settings));
        }
        catch { }
    }

    private class EditorSettings
    {
        public List<string> RecentFiles { get; set; } = new();
    }
}
