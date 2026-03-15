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
    public List<TreeNode> SelectedItems { get; } = new();
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
        var parent = SelectedItem is ResourceFolderNode rf && rf.ResourceKind == "Script"
            ? (TreeNode)rf
            : SelectedItem is ScriptGroupNode ? SelectedItem : null;
        if (parent != null)
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
            SelectedItem = newNode;
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

    /// <summary>
    /// Adds a resource folder under the current selection (Script/PLC/Screen group or folder).
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
            SelectedItem = folder;
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
        var toDelete = SelectedItems.Where(n => n.Parent != null).ToList();
        if (toDelete.Count == 0) return;

        foreach (var item in toDelete)
        {
            if (item is UserNode un)
                _rootModel?.Users.Remove(un.User);
            else if (item is UserGroupNode ugn)
            {
                _rootModel?.UserGroups.Remove(ugn.UserGroup);
                foreach (var child in ugn.Children.OfType<UserNode>())
                    _rootModel?.Users.Remove(child.User);
            }

            item.Parent!.Children.Remove(item);
            item.IsSelected = false;
        }

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

            // ─── Multi-paste cases ──────────────────────────────
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
                // Not siblings — just select the new node
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
        SelectedItems.Clear();
        SelectedItem = null;

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
            BuildResourceTree(scriptGroup, _rootModel.Scripts, "Script",
                s => s.Group, s => new ScriptNode(s) { });
        }
        RootItems.Add(scriptGroup);

        var plcGroup = new PlcGroupNode();
        if (_rootModel?.PlcPrograms != null)
        {
            BuildResourceTree(plcGroup, _rootModel.PlcPrograms, "PlcProgram",
                p => p.Group, p => new PlcProgramNode(p) { });
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
            BuildResourceTree(screenGroup, _rootModel.Screens, "Screen",
                s => s.Group, s => new ScreenNode(s) { });
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

    /// <summary>
    /// Builds a folder tree from a flat list of resource items using their Group path.
    /// Items with an empty Group go directly under the parent; others are nested in ResourceFolderNodes.
    /// </summary>
    private static void BuildResourceTree<T>(TreeNode parent, List<T> items, string resourceKind,
        Func<T, string> getGroup, Func<T, TreeNode> createNode)
    {
        // Cache of group-path → folder node
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
