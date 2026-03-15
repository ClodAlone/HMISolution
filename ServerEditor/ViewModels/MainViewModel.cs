using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using SharedModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.VisualBasic;
using ServerEditor.Services;
using SimpleOpcFileServer;

namespace ServerEditor.ViewModels
{
    // Mock globals for syntax checking
    public class ScriptGlobals
    {
        public object? Read(string variableName) => null;
        public void Write(string variableName, object value) { }
        public void OnChanged(string variableName, Action<VariableChangedEventArgs> handler) { }
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
             => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public abstract class NodeItemViewModel : ObservableObject
    {
        private string _name = "";
        private bool _isSelected;
        private bool _isExpanded;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
        
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public abstract string TypeName { get; }
        
        public ObservableCollection<NodeItemViewModel> Children { get; } = new();

        public NodeItemViewModel? Parent { get; set; }
    }

    public class FolderViewModel : NodeItemViewModel
    {
        public override string TypeName => "Folder";
        public Folder Folder { get; }

        public FolderViewModel(Folder folder)
        {
            Folder = folder;
            Name = folder.Name;
            PropertyChanged += (s, e) => { if (e.PropertyName == nameof(Name)) folder.Name = Name; };
        }
    }

    public class VariableViewModel : NodeItemViewModel
    {
        public override string TypeName => "Variable";
        public Variable Variable { get; }
        
        public IEnumerable<string> AvailableTypes => new[] { "Double", "Int32", "Boolean", "String", "DateTime", "Float", "Int16", "UInt16", "UInt32" };

        private string _driverSettingsJsonString = "{}";

        public VariableViewModel(Variable variable)
        {
            Variable = variable;
            Name = variable.Name;
            
            // Initialize from model
            if (Variable.DriverConfigs != null && Variable.DriverConfigs.Count > 0)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                _driverSettingsJsonString = JsonSerializer.Serialize(Variable.DriverConfigs, options);
            }

            PropertyChanged += (s, e) => { if (e.PropertyName == nameof(Name)) variable.Name = Name; };
        }

        public string DriverSettingsJson
        {
            get => _driverSettingsJsonString;
            set
            {
                _driverSettingsJsonString = value;
                OnPropertyChanged();

                try
                {
                     if (string.IsNullOrWhiteSpace(value) || value.Trim() == "{}")
                     {
                         Variable.DriverConfigs = null; // Update model
                     }
                     else
                     {
                         // Update model if valid
                         Variable.DriverConfigs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(value);
                     }
                }
                catch
                {
                    // Invalid JSON, keep model as is, but UI shows edit (backing field updated)
                }
            }
        }
    }

    public class ScriptGroupViewModel : NodeItemViewModel
    {
         public override string TypeName => "ScriptGroup";
         public ScriptGroupViewModel()
         {
             Name = "Scripts";
         }
    }

    public class ScriptViewModel : NodeItemViewModel
    {
        public override string TypeName => "Script";
        public ScriptConfig Script { get; }

        public string Code
        {
            get => Script.Code;
            set { Script.Code = value; OnPropertyChanged(); }
        }

        public bool Enabled
        {
            get => Script.Enabled;
            set { Script.Enabled = value; OnPropertyChanged(); }
        }

        public int IntervalMs
        {
            get => Script.IntervalMs;
            set { Script.IntervalMs = value; OnPropertyChanged(); }
        }

        public string Language
        {
            get => Script.Language;
            set { Script.Language = value; OnPropertyChanged(); OnPropertyChanged(nameof(SyntaxHighlightingName)); }
        }

        public string SyntaxHighlightingName => IsVb ? "VB" : "C#";

        public bool IsVb => Language.Equals("VB", StringComparison.OrdinalIgnoreCase)
                         || Language.Equals("VB.NET", StringComparison.OrdinalIgnoreCase)
                         || Language.Equals("VisualBasic", StringComparison.OrdinalIgnoreCase);

        public IEnumerable<string> AvailableLanguages => ["CSharp", "VB"];

        public ScriptViewModel(ScriptConfig script)
        {
            Script = script;
            Name = script.Name;
        }
    }

    public class MainViewModel : ObservableObject
    {
        private string _nodesPath = @"..\Server\nodes.json"; // Relative path assumed
        private NodeModel? _rootModel;
        private bool _hasUnsavedChanges;
        private bool _isServerRunning;
        private Process? _serverProcess;

        public ObservableCollection<NodeItemViewModel> RootItems { get; } = new();

        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set { _hasUnsavedChanges = value; OnPropertyChanged(); }
        }

        public bool IsServerRunning
        {
            get => _isServerRunning;
            set { _isServerRunning = value; OnPropertyChanged(); }
        }

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand AddVariableCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddScriptCommand { get; }
        public ICommand CheckSyntaxCommand { get; }
        public ICommand StartServerCommand { get; }
        public ICommand StopServerCommand { get; }
        public ICommand RefreshJsonCommand { get; }
        public ICommand ApplyJsonCommand { get; }
        
        // Service Management
        public ICommand InstallServiceCommand { get; }
        public ICommand UninstallServiceCommand { get; }
        public ICommand StartServiceCmd { get; } // Differs from StartServerCommand (Process)
        public ICommand StopServiceCmd { get; }  // Differs from StopServerCommand (Process)

        private ServiceControllerStatus _serviceStatus;
        public ServiceControllerStatus ServiceStatus
        {
            get => _serviceStatus;
            set { _serviceStatus = value; OnPropertyChanged(); }
        }

        private bool _isServiceInstalled;
        public bool IsServiceInstalled
        {
            get => _isServiceInstalled;
            set { _isServiceInstalled = value; OnPropertyChanged(); }
        }

        // Copilot
        private string _copilotPrompt = "";
        public string CopilotPrompt
        {
            get => _copilotPrompt;
            set { _copilotPrompt = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> AiEngines { get; } = new() { "OpenAI", "Gemini" };
        private string _selectedAiEngine = "OpenAI";
        public string SelectedAiEngine
        {
            get => _selectedAiEngine;
            set { _selectedAiEngine = value; OnPropertyChanged(); }
        }
        public ICommand AskCopilotCommand { get; }

        public LogViewerViewModel LogViewer { get; } = new();
        public LogViewerViewModel AppLog { get; } = new();

        public event Action? ServerStarted;

        private NodeItemViewModel? _selectedItem;
        public NodeItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        private bool _isAiProcessing;
        public bool IsAiProcessing
        {
            get => _isAiProcessing;
            set 
            {
                 if (_isAiProcessing != value)
                 {
                     _isAiProcessing = value;
                     OnPropertyChanged();
                     CommandManager.InvalidateRequerySuggested();
                 }
            }
        }

        private string _jsonEditorText = "";
        public string JsonEditorText
        {
            get => _jsonEditorText;
            set { _jsonEditorText = value; OnPropertyChanged(); }
        }

        private string _diffText = "";
        public string DiffText
        {
            get => _diffText;
            set { _diffText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> RecentFiles { get; } = new();
        public ICommand OpenRecentCommand { get; }

        private bool _isInternalChange;

        private string? _selectedRecentFile;
        public string? SelectedRecentFile
        {
            get => _selectedRecentFile;
            set
            {
                if (_selectedRecentFile != value)
                {
                    _selectedRecentFile = value;
                    OnPropertyChanged();
                    if (!_isInternalChange && !string.IsNullOrEmpty(value))
                    {
                        // Use dispatcher to allow UI to update and avoid re-entrancy issues with collection changes
                       Application.Current.Dispatcher.InvokeAsync(() => 
                       {
                           LoadFromFile(value);
                       });
                    }
                }
            }
        }

        // Git / GitHub Config
        private string _gitUserName = "";
        public string GitUserName
        {
            get => _gitUserName;
            set { _gitUserName = value; OnPropertyChanged(); }
        }

        private string _gitUserEmail = "";
        public string GitUserEmail
        {
            get => _gitUserEmail;
            set { _gitUserEmail = value; OnPropertyChanged(); }
        }

        private string _commitMessage = "Update nodes.json";
        public string CommitMessage
        {
            get => _commitMessage;
            set { _commitMessage = value; OnPropertyChanged(); }
        }

        private string _gitRemoteUrl = "";
        public string GitRemoteUrl
        {
            get => _gitRemoteUrl;
            set { _gitRemoteUrl = value; OnPropertyChanged(); }
        }
        
        public ICommand ConfigureGitUserCommand { get; }
        public ICommand InitGitRepoCommand { get; }
        public ICommand SetRemoteCommand { get; }

        private GitService? _gitService;

        private readonly WindowsServiceManager _serviceManager = new();
        private System.Threading.Timer? _statusTimer;

        private string _serverEndpointUrl = "opc.tcp://localhost:14840/SimpleOpcFileServer";
        public string ServerEndpointUrl
        {
            get => _serverEndpointUrl;
            set
            {
                if (_serverEndpointUrl != value)
                {
                    _serverEndpointUrl = value;
                    OnPropertyChanged();
                    if (_rootModel != null)
                    {
                         if (_rootModel.Server == null) _rootModel.Server = new ServerSettings();
                         _rootModel.Server.EndpointUrl = value;
                         HasUnsavedChanges = true;
                    }
                }
            }
        }

        public MainViewModel()
        {
            LoadSettings();

            // Initialize GitService with the directory containing nodes.json
            string serverDir = Path.GetDirectoryName(Path.GetFullPath(_nodesPath)) ?? ".";
            _gitService = new GitService(serverDir);

            LoadCommand = new RelayCommand(_ => LoadWithDialog());
            OpenRecentCommand = new RelayCommand(p => 
            {
                if (p is string path && !string.IsNullOrWhiteSpace(path)) 
                    LoadFromFile(path);
            });
            SaveCommand = new RelayCommand(_ => Save());
            SaveAsCommand = new RelayCommand(_ => SaveAs());
            AddFolderCommand = new RelayCommand(AddFolder, CanAddFolder);
            AddVariableCommand = new RelayCommand(AddVariable, CanAddVariable);
            AddScriptCommand = new RelayCommand(AddScript, CanAddScript);
            CheckSyntaxCommand = new RelayCommand(CheckSyntax, CanCheckSyntax);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            StartServerCommand = new RelayCommand(StartServer, CanStartServer);
            StopServerCommand = new RelayCommand(StopServer, CanStopServer);
            RefreshJsonCommand = new RelayCommand(_ => RefreshJsonFromTree());
            ApplyJsonCommand = new RelayCommand(_ => ApplyJsonToTree());
            InstallServiceCommand = new RelayCommand(InstallService, CanInstallService);
            UninstallServiceCommand = new RelayCommand(UninstallService, CanUninstallService);
            StartServiceCmd = new RelayCommand(StartService, CanStartService);
            StopServiceCmd = new RelayCommand(StopService, CanStopService);
            AskCopilotCommand = new RelayCommand(AskCopilot, _ => !string.IsNullOrWhiteSpace(CopilotPrompt) && !IsAiProcessing);
            
            // Git
            ConfigureGitUserCommand = new RelayCommand(async _ => await ConfigureGitUser(), _ => !string.IsNullOrWhiteSpace(GitUserName) && !string.IsNullOrWhiteSpace(GitUserEmail));
            InitGitRepoCommand = new RelayCommand(async _ => await InitGitRepo(), _ => _gitService != null && !_gitService.IsGitRepository());
            SetRemoteCommand = new RelayCommand(async _ => await SetRemote(), _ => !string.IsNullOrWhiteSpace(GitRemoteUrl));
            
            // Auto-load if file exists
            if (File.Exists(_nodesPath))
            {
                LoadFromFile(_nodesPath);
                HasUnsavedChanges = false;
            }

            // Initialize service status timer
            _statusTimer = new System.Threading.Timer(UpdateServiceStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        private async Task ConfigureGitUser()
        {
            if (_gitService != null)
            {
                var result = await _gitService.SetUserIdentityAsync(GitUserName, GitUserEmail);
                if (result.Contains("Error"))
                {
                    MessageBox.Show(result, "Git Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Git user configured successfully.", "Git", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Force refresh of commands
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private async Task InitGitRepo()
        {
            if (_gitService != null)
            {
                var result = await _gitService.InitRepoAsync();
                if (!result.StartsWith("Error"))
                {
                    MessageBox.Show("Git repository initialized.", "Git", MessageBoxButton.OK, MessageBoxImage.Information);
                    CommandManager.InvalidateRequerySuggested();
                }
                else
                {
                    MessageBox.Show($"Error initializing repository: {result}", "Git Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SetRemote()
        {
            if (_gitService != null)
            {
                var result = await _gitService.SetRemoteOriginAsync(GitRemoteUrl);
                if (result.Contains("Error"))
                {
                    MessageBox.Show(result, "Git Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Git remote origin set.", "Git", MessageBoxButton.OK, MessageBoxImage.Information);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private void RefreshJsonFromTree()
        {
            if (_rootModel == null) return;
            RebuildModelStructure();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                JsonEditorText = JsonSerializer.Serialize(_rootModel, options);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating JSON: {ex.Message}");
            }
        }

        private void ApplyJsonToTree()
        {
            try
            {
                var newModel = JsonSerializer.Deserialize<NodeModel>(JsonEditorText);
                if (newModel != null)
                {
                    _rootModel = newModel;
                    ReloadViewModels();
                    HasUnsavedChanges = true;
                    MessageBox.Show("JSON applied successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying JSON: {ex.Message}");
            }
        }

        private void ReloadViewModels()
        {
             RootItems.Clear();
             if (_rootModel?.Folder != null)
             {
                 var rootVm = CreateFolderVm(_rootModel.Folder);
                 RootItems.Add(rootVm);
                 rootVm.IsExpanded = true;
                 WatchChanges(rootVm);
             }

             var scriptGroupVm = new ScriptGroupViewModel();
             if (_rootModel?.Scripts != null)
             {
                 foreach (var script in _rootModel.Scripts)
                 {
                     var sVm = new ScriptViewModel(script) { Parent = scriptGroupVm };
                     scriptGroupVm.Children.Add(sVm);
                     WatchChanges(sVm);
                 }
             }
             RootItems.Add(scriptGroupVm);
             WatchChanges(scriptGroupVm);
        }

        private void LoadWithDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "nodes.json",
                DefaultExt = ".json",
                Filter = "JSON documents (.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                LoadFromFile(dialog.FileName);
            }
        }

        private void LoadFromFile(string path)
        {
            _isInternalChange = true;
            try
            {
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    _rootModel = JsonSerializer.Deserialize<NodeModel>(json);

                    // Load external resource files (scripts/, screens/, plcprograms/)
                    if (_rootModel != null)
                        ResourceFileManager.LoadExternalResources(_rootModel, path);

                    if (_rootModel.Server == null) _rootModel.Server = new ServerSettings();
                    ServerEndpointUrl = _rootModel.Server.EndpointUrl;

                    _nodesPath = path; // Update current path

                    // Add to Recent Files
                    string fullPath = Path.GetFullPath(path);
                    if (RecentFiles.Contains(fullPath)) RecentFiles.Remove(fullPath);
                    RecentFiles.Insert(0, fullPath);
                    while (RecentFiles.Count > 10) RecentFiles.RemoveAt(RecentFiles.Count - 1);
                    
                    SelectedRecentFile = fullPath; // Ensure selection
                    SaveSettings();

                    // Update service name based on config file
                    var configName = Path.GetFileNameWithoutExtension(path);
                    _serviceManager.ServiceName = $"SimpleOpcFileServer_{configName}";
                    UpdateServiceStatus(null);
                    
                    // Update log directory with dynamic pattern
                    var dir = Path.GetDirectoryName(Path.GetFullPath(_nodesPath));
                    if (dir != null)
                    {
                        LogViewer.SetLogDirectory(Path.Combine(dir, "Logs"), $"log-{configName}-*.txt");
                         _gitService = new GitService(dir);
                    }

                    ReloadViewModels();

                    HasUnsavedChanges = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading {path}: {ex.Message}");
            }
            finally
            {
                _isInternalChange = false;
            }
        }

        private async void Save()
        {
            try
            {
                if (_rootModel != null)
                {
                    // ViewModels update Models via PropertyChanged, so model is current except structure.
                    // Structure is maintained by Children collection?
                    // Wait, Adding/Removing children from VM must reflect in Model.
                    // I need to rebuild Model structure from VM before save or sync.
                    
                    RebuildModelStructure();

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
                    
                    if (_gitService != null && _gitService.IsGitRepository())
                    {
                        var msg = string.IsNullOrWhiteSpace(CommitMessage) ? "Update nodes.json" : CommitMessage;
                        var commitRes = await _gitService.CommitChangesAsync(Path.GetFileName(_nodesPath), msg);
                        if (!commitRes.StartsWith("Error"))
                        {
                            await _gitService.PushAsync();
                        }
                    }

                    MessageBox.Show($"Saved successfully to {_nodesPath}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving nodes.json: {ex.Message}");
            }
        }

        private void SaveAs()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "nodes.json",
                DefaultExt = ".json",
                Filter = "JSON documents (.json)|*.json"
            };

            if (dialog.ShowDialog() == true)
            {
                _nodesPath = dialog.FileName;
                var dir = Path.GetDirectoryName(_nodesPath);
                if (dir != null)
                {
                    _gitService = new GitService(dir);
                }
                Save();
            }
        }

        private void RebuildModelStructure()
        {
             // Recursively update model children based on VM children
             foreach(var rootVm in RootItems)
             {
                 if (rootVm is FolderViewModel fvm && _rootModel != null)
                 {
                     _rootModel.Folder = fvm.Folder; // This might be redundant if objects are reference-linked, but structure (Folders/Variables list) needs update
                     UpdateFolderModel(fvm);
                 }
                 else if (rootVm is ScriptGroupViewModel sgvm && _rootModel != null)
                 {
                     _rootModel.Scripts.Clear();
                     foreach(var child in sgvm.Children)
                     {
                         if (child is ScriptViewModel svm)
                         {
                             _rootModel.Scripts.Add(svm.Script);
                         }
                     }
                 }
             }
        }

        private void UpdateFolderModel(FolderViewModel fvm)
        {
            fvm.Folder.Folders.Clear();
            fvm.Folder.Variables.Clear();

            foreach(var child in fvm.Children)
            {
                if (child is FolderViewModel childFvm)
                {
                    fvm.Folder.Folders.Add(childFvm.Folder);
                    UpdateFolderModel(childFvm);
                }
                else if (child is VariableViewModel childVvm)
                {
                    fvm.Folder.Variables.Add(childVvm.Variable);
                }
            }
        }

        private FolderViewModel CreateFolderVm(Folder folder)
        {
            var vm = new FolderViewModel(folder);
            foreach(var subFolder in folder.Folders)
            {
                var childVm = CreateFolderVm(subFolder);
                childVm.Parent = vm;
                vm.Children.Add(childVm);
            }
            foreach(var variable in folder.Variables)
            {
                var childVm = new VariableViewModel(variable) { Parent = vm };
                vm.Children.Add(childVm);
            }
            return vm;
        }

        private void WatchChanges(NodeItemViewModel item)
        {
            item.PropertyChanged += (s, e) => HasUnsavedChanges = true;
            item.Children.CollectionChanged += (s, e) => HasUnsavedChanges = true;
            foreach (var child in item.Children)
            {
               WatchChanges(child);
            }
        }

        private bool CanAddFolder(object? parameter) => SelectedItem is FolderViewModel; // Only folders can contain folders
        private void AddFolder(object? parameter)
        {
            if (SelectedItem is FolderViewModel parent)
            {
                var newFolder = new Folder { Name = "New Folder" };
                var newVm = new FolderViewModel(newFolder) { Parent = parent };
                parent.Children.Add(newVm);
                WatchChanges(newVm);
                parent.IsExpanded = true;
                newVm.IsSelected = true;
            }
        }

        private bool CanAddVariable(object? parameter) => SelectedItem is FolderViewModel; // Only folders can contain variables
        private void AddVariable(object? parameter)
        {
            if (SelectedItem is FolderViewModel parent)
            {
                var newVar = new Variable { Name = "New Variable", Type = "Double", Value = 0.0, Access = "ReadWrite" };
                var newVm = new VariableViewModel(newVar) { Parent = parent };
                parent.Children.Add(newVm);
                WatchChanges(newVm);
                parent.IsExpanded = true;
                newVm.IsSelected = true;
            }
        }
        
        private bool CanDelete(object? parameter) => SelectedItem != null && SelectedItem.Parent != null; // Cannot delete root
        private void Delete(object? parameter)
        {
             if (SelectedItem != null && SelectedItem.Parent != null)
             {
                 var parent = SelectedItem.Parent;
                 parent.Children.Remove(SelectedItem);
                 SelectedItem = null;
                 HasUnsavedChanges = true;
             }
        }

        private bool CanAddScript(object? parameter) => SelectedItem is ScriptGroupViewModel; // Only script groups can contain scripts
        private void AddScript(object? parameter)
        {
            if (SelectedItem is ScriptGroupViewModel parent)
            {
                var newScript = new ScriptConfig { Name = "New Script", Code = "var val = Read(\"MyVar\");\nWrite(\"MyVar\", 123);", IntervalMs = 1000, Enabled = true };
                var newVm = new ScriptViewModel(newScript) { Parent = parent };
                parent.Children.Add(newVm);
                WatchChanges(newVm);
                parent.IsExpanded = true;
                newVm.IsSelected = true;
            }
        }

        private bool CanCheckSyntax(object? parameter) => SelectedItem is ScriptViewModel && !_isCheckingSyntax;
        private bool _isCheckingSyntax;

        private async void CheckSyntax(object? parameter)
        {
            if (SelectedItem is ScriptViewModel scriptVm)
            {
                _isCheckingSyntax = true;
                Mouse.OverrideCursor = Cursors.Wait;
                CommandManager.InvalidateRequerySuggested();

                try
                {
                    var code = scriptVm.Code;
                    var name = scriptVm.Name;
                    var isVb = scriptVm.IsVb;

                    var (success, message) = await Task.Run(() =>
                    {
                        if (isVb)
                            return CompileVbCheck(code);
                        else
                            return CompileCSharpCheck(code);
                    });

                    if (success)
                        MessageBox.Show($"Script '{name}' has no syntax errors.", "Syntax Check", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show(message, "Syntax Check", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error validating script: {ex.Message}", "Syntax Check", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                    _isCheckingSyntax = false;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private static (bool success, string message) CompileCSharpCheck(string code)
        {
            var options = ScriptOptions.Default
                    .AddReferences(typeof(SimpleFileServerNodeManager).Assembly)
                    .AddImports("System", "System.Collections.Generic", "System.Linq");

            var script = CSharpScript.Create(code, options, typeof(ScriptGlobals));
            var diagnostics = script.Compile();

            var errors = diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToList();

            if (errors.Count > 0)
            {
                var errorText = string.Join("\n", errors
                    .Select(d => $"Line {d.Location.GetLineSpan().StartLinePosition.Line + 1}: {d.GetMessage()}"));
                return (false, $"Syntax errors found:\n{errorText}");
            }

            return (true, "");
        }

        private static (bool success, string message) CompileVbCheck(string code)
        {
            try
            {
                ScriptRunner.CompileVbScript(code);
                return (true, "");
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message);
            }
        }

        private bool CanStartServer(object? parameter) => !IsServerRunning;
        private async void StartServer(object? parameter)
        {
            if (HasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before starting the server?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    Save();
                }
            }

            string? serverExe = null;
            string editorDir = AppContext.BaseDirectory;

            // 1. Look relative to the Editor executable (Production/Published - adjacent)
            string adjacentServer = Path.Combine(editorDir, "Server.exe");
            if (File.Exists(adjacentServer)) 
            {
                serverExe = adjacentServer;
            }

            // 2. Look relative to the Editor project structure (Development)
            // Navigate up from bin/Debug/netX.0 to find solution root
            if (serverExe == null)
            {
                 var dir = new DirectoryInfo(editorDir);
                 // Traverse up to find directory containing "Server" folder
                 // Limit steps to avoid infinite loop
                 for (int i = 0; i < 6; i++)
                 {
                     if (dir == null) break;
                     if (Directory.Exists(Path.Combine(dir.FullName, "Server")))
                     {
                         var serverProjectDir = Path.Combine(dir.FullName, "Server");
                         var debugPath = Path.Combine(serverProjectDir, "bin", "Debug", "net10.0", "Server.exe");
                         var releasePath = Path.Combine(serverProjectDir, "bin", "Release", "net10.0", "Server.exe");
                         
                         if (File.Exists(debugPath)) { serverExe = debugPath; break; }
                         if (File.Exists(releasePath)) { serverExe = releasePath; break; }
                     }
                     dir = dir.Parent;
                 }
            }

            // 3. Fallback: Look relative to the config file
            if (serverExe == null)
            {
                string configDir = Path.GetDirectoryName(Path.GetFullPath(_nodesPath)) ?? ".";
                string[] configRelativeAttempts = new[] {
                    Path.Combine(configDir, "bin", "Debug", "net10.0", "Server.exe"),
                    Path.Combine(configDir, "bin", "Release", "net10.0", "Server.exe"),
                    Path.Combine(configDir, "Server.exe")
                };
                
                foreach(var path in configRelativeAttempts)
                {
                    if (File.Exists(path))
                    {
                        serverExe = path;
                        break;
                    }
                }
            }
            
            if (serverExe == null)
            {
                MessageBox.Show("Could not find Server.exe. Please ensure the Server project is built.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _serverProcess = new Process();
                _serverProcess.StartInfo.FileName = serverExe;
                
                // Set working directory to where nodes.json is, so the server watches the edited file
                string fullNodesPath = Path.GetFullPath(_nodesPath);
                string nodesDir = Path.GetDirectoryName(fullNodesPath) ?? ".";
                _serverProcess.StartInfo.WorkingDirectory = nodesDir;
                
                // Pass the configuration file path as an argument. Use quotes to handle spaces.
                _serverProcess.StartInfo.Arguments = $"\"{fullNodesPath}\"";

                _serverProcess.StartInfo.UseShellExecute = true; // Use shell so it opens in new window
                _serverProcess.EnableRaisingEvents = true;
                _serverProcess.Exited += (s, e) => 
                {
                    Application.Current.Dispatcher.Invoke(() => IsServerRunning = false);
                };
                
                _serverProcess.Start();
                IsServerRunning = true;

                // Wait for server to initialize
                await Task.Delay(3000);
                ServerStarted?.Invoke();
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Failed to start server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanStopServer(object? parameter) => IsServerRunning;
        public void StopServer(object? parameter = null)
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                try
                {
                    _serverProcess.Kill();
                    _serverProcess.Dispose();
                    _serverProcess = null;
                    IsServerRunning = false;
                }
                catch (Exception ex)
                {
                     MessageBox.Show($"Failed to stop server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanInstallService(object? parameter) => !IsServiceInstalled && _rootModel != null;
        private async void InstallService(object? parameter)
        {
            var serverDir = Path.GetDirectoryName(Path.GetFullPath(_nodesPath)) ?? ".";
            string executablePath = Path.Combine(serverDir, "Server.exe");
            
            // Check for Server.exe differently if not found
            if (!File.Exists(executablePath))
            {
                 // Try well known locations if published
                 string[] attempts = new[] {
                    Path.Combine(serverDir, "bin", "Debug", "net10.0", "Server.exe"),
                    Path.Combine(serverDir, "bin", "Release", "net10.0", "Server.exe"),
                    Path.Combine(serverDir, "Server.exe")
                };
                foreach(var p in attempts)
                {
                    if (File.Exists(p))
                    {
                        executablePath = p;
                        break;
                    }
                }
            }
            
            if (!File.Exists(executablePath))
            {
                MessageBox.Show("Server.exe not found. Please build the server first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // We need full path for the config argument
            string configPath = Path.GetFullPath(_nodesPath);
            
            var result = await _serviceManager.InstallServiceAsync(executablePath, configPath);
            if (result.Contains("Success") || !result.StartsWith("Error"))
            {
                MessageBox.Show("Service installed successfully.", "Service", MessageBoxButton.OK, MessageBoxImage.Information);
                // Force status update
                UpdateServiceStatus(null);
            }
            else
            {
                MessageBox.Show($"Failed to install service: {result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUninstallService(object? parameter) => IsServiceInstalled;
        private async void UninstallService(object? parameter)
        {
            if (MessageBox.Show("Are you sure you want to uninstall the service?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var result = await _serviceManager.UninstallServiceAsync();
                if (result.Contains("Success") || !result.StartsWith("Error"))
                {
                    MessageBox.Show("Service uninstalled successfully.", "Service", MessageBoxButton.OK, MessageBoxImage.Information);
                    UpdateServiceStatus(null);
                }
                else
                {
                    MessageBox.Show($"Failed to uninstall service: {result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanStartService(object? parameter) => IsServiceInstalled && ServiceStatus == ServiceControllerStatus.Stopped;
        private async void StartService(object? parameter)
        {
             var result = await _serviceManager.StartServiceAsync();
             if (result.Contains("Success") || !result.StartsWith("Error"))
             {
                 // Wait a bit
                 await Task.Delay(1000);
                 UpdateServiceStatus(null);
             }
             else
             {
                 MessageBox.Show($"Failed to start service: {result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
             }
        }

        private bool CanStopService(object? parameter) => IsServiceInstalled && ServiceStatus == ServiceControllerStatus.Running;
        private async void StopService(object? parameter)
        {
             var result = await _serviceManager.StopServiceAsync();
             if (result.Contains("Success") || !result.StartsWith("Error"))
             {
                 // Wait a bit
                 await Task.Delay(1000);
                 UpdateServiceStatus(null);
             }
             else
             {
                 MessageBox.Show($"Failed to stop service: {result}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
             }
        }

        private void UpdateServiceStatus(object? state)
        {
            // Always check if installed
            IsServiceInstalled = _serviceManager.IsServiceInstalled();

            if (IsServiceInstalled)
            {
                try
                {
                    var status = _serviceManager.GetServiceStatus();
                    ServiceStatus = status;
                }
                catch
                {
                    ServiceStatus = (ServiceControllerStatus)0;
                }
            }
            else
            {
                ServiceStatus = (ServiceControllerStatus)0;
            }
        }

        private async void AskCopilot(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(CopilotPrompt)) return;
            
            AppLog.AddLog("INF", $"AI Request started with {SelectedAiEngine}: {CopilotPrompt}");
            var oldJson = JsonEditorText;

            try
            {
                 Mouse.OverrideCursor = Cursors.Wait;
                 IsAiProcessing = true;
                 
                 (string newJson, string comments) result = ("", "");

                 switch (SelectedAiEngine)
                 {
                     case "OpenAI":
                         result = await CallOpenAiAsync(oldJson);
                         break;
                     case "Gemini":
                         result = await CallGeminiAsync(oldJson);
                         break;
                     default:
                         MessageBox.Show("Selected AI Engine not supported yet.", "AI Engine", MessageBoxButton.OK, MessageBoxImage.Warning);
                         break;
                 }

                 if (!string.IsNullOrEmpty(result.newJson))
                 {
                     JsonEditorText = result.newJson;
                     AppLog.AddLog("INF", "AI Request completed.");
                     
                     if (!string.IsNullOrWhiteSpace(result.comments))
                     {
                        AppLog.AddLog("INF", $"AI Comments: {result.comments}");
                     }

                     DiffText = GenerateDiff(oldJson, result.newJson);
                     // LogDiff(oldJson, newJson); // Keeping existing call if it existed, but looking closely at previous file content, it was there.
                     // IMPORTANT: I am replacing the method body, so I need to check if LogDiff was there.
                     // Yes, line 999 in previous context.
                     // But I don't see LogDiff definition in the file snippets I've read.
                     // It might be below line 1100.
                     // So I should NOT include LogDiff definition in this edit block unless I'm sure I'm overwriting it or I need to add it.
                     // Since I am only replacing specific methods, I will just call it.
                     LogDiff(oldJson, result.newJson);
                 }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AI Error: {ex.Message}");
                AppLog.AddLog("ERR", $"AI Error: {ex.Message}");
            }
            finally
            {
                Mouse.OverrideCursor = null;
                IsAiProcessing = false;
            }
        }

        private async Task<(string, string)> CallOpenAiAsync(string oldJson)
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "";
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("To use OpenAI, set OPENAI_API_KEY environment variable.", "AI Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                return ("", "");
            }

            string endpoint = "https://api.openai.com/v1/chat/completions";
            string model = "gpt-4o";
            string newJson = "";
            string comments = "";

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                
                var prompt = $"You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n{oldJson}\n\nUser Instruction: {CopilotPrompt}\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.";
                
                var requestBody = new
                {
                    model = model,
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature = 0.1
                };

                var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                
                var responseString = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(responseString))
                {
                    var contentText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    if (contentText != null)
                    {
                        var parts = contentText.Split(new[] { "///COMMENTS///" }, StringSplitOptions.None);
                        
                        newJson = parts[0].Replace("```json", "").Replace("```", "").Trim();
                        if (parts.Length > 1)
                        {
                            comments = parts[1].Trim();
                        }
                    }
                }
            }
            return (newJson, comments);
        }

        private async Task<(string, string)> CallGeminiAsync(string oldJson)
        {
            string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("To use Gemini, set GEMINI_API_KEY environment variable.", "AI Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                return ("", "");
            }

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";
            string newJson = "";
            string comments = "";

            using (var client = new System.Net.Http.HttpClient())
            {
                var prompt = $"You are a JSON editor helper for an OPC UA Server configuration. The user wants to modify the following JSON:\n\n{oldJson}\n\nUser Instruction: {CopilotPrompt}\n\nReturn the FULL valid JSON content as the result, incorporating the requested changes. Do not return just the difference or a snippet. Do not use markdown blocks around the JSON.\n\nAt the end of the JSON, print the delimiter '///COMMENTS///', followed by a summary of what has been changed.";
                
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                
                var responseString = await response.Content.ReadAsStringAsync();
                using (var doc = JsonDocument.Parse(responseString))
                {
                    var contentText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                    if (contentText != null)
                    {
                        var parts = contentText.Split(new[] { "///COMMENTS///" }, StringSplitOptions.None);
                        
                        newJson = parts[0].Replace("```json", "").Replace("```", "").Trim();
                        if (parts.Length > 1)
                        {
                            comments = parts[1].Trim();
                        }
                    }
                }
            }
            return (newJson, comments);
        }

        private string GenerateDiff(string oldText, string newText)
        {
            // Simple placeholder for diff text. A real diff lib would be better.
            return newText; 
        }

        private void LogDiff(string oldJson, string newJson)
        {
            AppLog.AddLog("DBG", $"JSON Length change: {newJson.Length - oldJson.Length}");
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
                            {
                                RecentFiles.Add(file);
                            }
                        }
                    }
                }
            }
            catch { /* Ignore errors loading settings */ }
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
             catch { /* Ignore errors saving settings */ }
        }
    }

    public class EditorSettings
    {
        public List<string> RecentFiles { get; set; } = new();
    }
}
