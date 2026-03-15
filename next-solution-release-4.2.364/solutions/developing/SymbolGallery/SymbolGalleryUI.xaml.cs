using System;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using System.IO;
using Utilities;
using System.Windows;
using System.Collections.Generic;
using UFInterfaces;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Utilities.WPF;
using VFS;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.Text;
using System.Runtime.Serialization;
using log4net;
using System.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.NavBar;
using IWorkspace = UFInterfaces.IWorkspace;
using UIMsgBoxAlertService.ComponentService;
using CommonControls;
using UFProjectManager.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SymbolGallery
{
    /// <summary>
    /// Interaction logic for SymbolGalleryControl.xaml
    /// </summary>
    public partial class SymbolGalleryUI : UserControl, IDisposable
    {

        #region IsEditable
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(SymbolGalleryUI), new UIPropertyMetadata(false));
        [Browsable(false)]
        public bool IsEditable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsEditableProperty);
            }
            internal set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        #endregion


        #region IsMergable
        public static readonly DependencyProperty IsMergableProperty = DependencyProperty.Register("IsMergable", typeof(bool), typeof(SymbolGalleryUI), new UIPropertyMetadata(false));
        [Browsable(false)]
        public bool IsMergable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsMergableProperty);
            }
            internal set
            {
                SetValue(IsMergableProperty, value);
            }
        }

        #endregion


        bool bLazyLoaded = false;

        readonly IWorkspace workspace;
        readonly Dictionary<String, NavBarGroup> mapFileToItem = new Dictionary<String, NavBarGroup>();
        readonly Dictionary<String, LibraryBrowser> mapLibraryBrowsers = new Dictionary<String, LibraryBrowser>();
        readonly Dictionary<LibraryBrowser, List<CarouselListControl>> mapOpenLibraries = new Dictionary<LibraryBrowser, List<CarouselListControl>>();
        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();
        readonly List<String> listEditables = new List<String>();

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.SymbolGallery);

        readonly String styleEditing;

        SymbolGalleryViewModel model;
        bool bLoaded;

        internal static String searchPatternXaml = $"*{Utilities.SymbolLibraryPath.PatternXaml}";
        internal static String patternXaml = Utilities.SymbolLibraryPath.PatternXaml;
        internal static String projectTag = Utilities.SymbolLibraryPath.ProjectSymbolTag;
        readonly IEnumerable<string> tables = new string[2] { $"{Utilities.SymbolLibraryPath.RootStyleFolder}", $"{Utilities.SymbolLibraryPath.RootSymbolFolder}" };
        public SymbolGalleryUI(IWorkspace w, String stlEditing = null)
        {
            workspace = w;
            styleEditing = stlEditing;
            InitializeComponent();

            if (!LoadSettings())
                DataContext = model = new SymbolGalleryViewModel(SymbolGalleryViewModel.VisualizationMode.Grid);

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        IsEditable = false;
                        string style = (string)ApplicationPropertiesHelper.GetProperty("CurrentSkin");

                        Brush brush = TryFindResource("HilightThemeButton") as Brush;
                        brush = WPFUtilities.ThemeHelper.GetHilightingThemeBrush(style);
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {

                                var grid = (navigationControl.Template.FindName("Root", navigationControl)) as Grid;
                                if (grid != null)
                                {
                                    var border = grid.FindParent<Border>();
                                    border.BorderBrush = Brushes.Transparent;
                                    border.CornerRadius = new CornerRadius(0);
                                    if (grid.RowDefinitions.Count > 0)
                                        grid.RowDefinitions[0].Height = new GridLength(0);
                                }

                                mapItems.Clear();

                                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                                string startingPath = String.Format("{0}.{1}\\Cultures\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion);
                                string fileToOpen = string.Empty;

                                foreach (string key in tables)
                                {
                                    fileToOpen = string.Format("{0}{1}\\StringTable_{2}.xml", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name, key);

                                    if (!File.Exists(fileToOpen))
                                        fileToOpen = string.Format("{0}StringTable_{1}.xml", startingPath, key);
                                    if (File.Exists(fileToOpen))
                                    {
                                        LoadFromXml(fileToOpen);
                                    }
                                }

                                if (!String.IsNullOrEmpty(styleEditing))
                                {
                                    toolbarTabLibrary.Visibility = Visibility.Collapsed;
                                    //var storyboard = TryFindResource("MouseOverOpacity") as Storyboard;
                                    //storyboard.Begin();
                                }
                            });
                    }
                };
            Unloaded += (o, e) =>
                {
                    if (bLoaded)
                    {
                        bLoaded = false;
                        SaveSettings();
                    }
                };
        }

        internal static String GetHiddenPluginFileName()
        {
            return String.Format("{0}.{1}\\ConfigFiles\\SGHiddens.xml", ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), Utilities.AssemblyInfo.FileFormatMainVersion);
        }

        static string GetAssemblyPath()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string s = a.Location.ToLower();
            string name = a.GetName().Name.ToLower() + ".dll";
            int idx = s.IndexOf(name);
            if (idx != -1)
                return a.Location.Substring(0, idx);
            return string.Empty;
        }

        internal void LoadSymbolGalleries()
        {
            if (bLazyLoaded)
                return;

            var fileSystemProvider = new DataSourceFileSystemProvider("")
            {
                ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zrcbzDCTAxNQQgiOfxR+Qrr+kaKsWBEuUjUZXAhwhL047wT/ZBFAeb9h/8jWCANOQcIUEFeLdYMAG0gWA3froFIq30bKpGNuZmkDWvshkbPvWxNIoSqTObryW5dRDGRokR6Gkg3nWKpA/1pzqo20qR0sGpHrbTkxM3r8pZJXdFkcArSrxxS6MQBeSuV5K1Kt5g=")
            };
            if (String.IsNullOrEmpty(styleEditing))
            {
                //AddTabBrowser(Properties.Resources.WebDynamicTab, GetStartingDynamicTypeFolder(fileSystemProvider), true, true, fileSystemProvider);
                //AddTabBrowser(Properties.Resources.DynamicTab, GetStartingDynamicTypeFolder(), false, true);

                //AddTabBrowser(Properties.Resources.CommonTab, Properties.Resources.CommonTabTooltip,
                //    GetStartingCommonFolder(), true, false);
                //AddTabBrowser(Properties.Resources.UserTab, Properties.Resources.UserTabTooltip,
                //    GetStartingUserFolder(), true, false);
                //AddTabBrowser(Properties.Resources.WebTab, Properties.Resources.WebTabTooltip,
                //    GetStartingFolder(fileSystemProvider), false, false, fileSystemProvider);
                var ret = AddTabBrowser(Properties.Resources.SystemTab, Properties.Resources.SystemTabTooltip,
                    GetStartingFolder(), false);
                bLazyLoaded = true;
                ret.IsSelected = true;
                SelectedTabLoad(Properties.Resources.SystemTab);
            }
            else
            {
                if (styleEditing == "Viewbox")
                {
                    //AddTabBrowser(Properties.Resources.WebTab, Properties.Resources.WebTabTooltip,
                    //    GetStartingFolder(fileSystemProvider), false, false, fileSystemProvider);
                    var ret = AddTabBrowser(Properties.Resources.SystemTab, Properties.Resources.SystemTabTooltip,
                        GetStartingFolder(), false);
                    bLazyLoaded = true;
                    ret.IsSelected = true;
                    SelectedTabLoad(Properties.Resources.SystemTab);
                }
                else
                {
                    //var folderDyn = String.Format("{0}\\{1}", GetStartingDynamicTypeFolder(fileSystemProvider), styleEditing);
                    //AddTabBrowser(Properties.Resources.WebDynamicTab, Properties.Resources.WebDynamicTabTooltip,
                    //    folderDyn, false, true, fileSystemProvider);
                    var folder = String.Format("{0}\\{1}", GetStartingDynamicTypeFolder(), styleEditing);
                    var ret = AddTabBrowser(Properties.Resources.DynamicTab, Properties.Resources.DynamicTabTooltip,
                        folder, false, true);
                    bLazyLoaded = true;
                    ret.IsSelected = true;
                    SelectedTabLoad(Properties.Resources.DynamicTab);
                }
            }
            list.SelectionChanged += (o, e) =>
                {
                    if (list.SelectedIndex >= 0)
                        navigationControl.SelectedGroup = navigationControl.Groups[list.SelectedIndex] as NavBarGroup;
                    txtNoLibrary.Visibility = list.SelectedIndex == -1 ? Visibility.Visible : Visibility.Collapsed;
                    // navigationControl.SelectedIndex = list.SelectedIndex;
                };
        }

        void RemoveTabBrowser(String name)
        {
            if (mapLibraryBrowsers.ContainsKey(name))
            {
                foreach (DXTabItem tab in tabControl.Items)
                {
                    if (tab.Header as String == name)
                    {
                        if (tab.Content is IDisposable)
                            (tab.Content as IDisposable).Dispose();
                        tabControl.Items.Remove(tab);
                        break;
                    }
                }

                if (mapOpenLibraries.ContainsKey(mapLibraryBrowsers[name]))
                    mapOpenLibraries[mapLibraryBrowsers[name]].ToList().ForEach(item =>
                    {
                        CloseLibrary(item.FilePath);
                    });

                if (mapOpenLibraries.ContainsKey(mapLibraryBrowsers[name]))
                    mapOpenLibraries[mapLibraryBrowsers[name]].Clear();

                mapLibraryBrowsers[name].Dispose();
                mapLibraryBrowsers.Remove(name);
            }
        }

        DXTabItem AddTabBrowser(String name, String tooltip, String startingFolder, bool isEditable, bool isDynamic = false, 
                                    FileSystemProviderBase fileSystemProvider = null, String relativeFolderTag = null)
        {
            if (mapLibraryBrowsers.ContainsKey(name))
            {
                foreach(DXTabItem tab in tabControl.Items)
                {
                    if (tab.Header as String == name)
                        return tab;
                }
                return null;
            }

            var tabItem = new DXTabItem() { Header = name, ToolTip = tooltip };
            tabItem.InitItemTemplate();
            var browser = new LibraryBrowser(workspace, fileSystemProvider, isDynamic, startingFolder, relativeFolderTag, 
                                                isEditable: isEditable);
            tabItem.Content = browser;
            mapLibraryBrowsers.Add(name, browser);
            tabControl.Items.Add(tabItem);
            if (isEditable)
                listEditables.Add(name);

            browser.SelectedChanged += (o, e) =>
                {
                    if (e.IsDynamic)
                        AddDynLibrary(browser, e.Name, e.fileSystemProvider);
                    else
                        AddLibrary(browser, e.Name, e.fileSystemProvider);
                };
            browser.DropChanged += (o, e) =>
                {
                    if (mapOpenLibraries.ContainsKey(e.Name))
                        mapOpenLibraries[e.Name].ForEach(control => control.CopyLink = e.bDrop == true);
                };

            return tabItem;
        }

        List<String> pending = new List<String>();
        void AddLibrary(LibraryBrowser browser, String filePath, FileSystemProviderBase fileSystemProvider)
        {
            if (mapFileToItem.ContainsKey(filePath))
            {
                navigationControl.SelectedGroup = mapFileToItem[filePath];
                list.SelectedIndex = navigationControl.Groups.IndexOf(navigationControl.SelectedGroup as NavBarGroup);
            }
            else
            {
                if (pending.Contains(filePath))
                    return;
                pending.Add(filePath);
                var task1 = Task.Factory.StartNew(() =>
                {
                    bool bAdd = false;
                    if (fileSystemProvider != null)
                    {
                        FileManagerFolder folder = new FileManagerFolder(fileSystemProvider, filePath);
                        if (fileSystemProvider.Exists(folder))
                        {
                            var listFiles = fileSystemProvider.GetFiles(folder);
                            var listXaml = (from c in listFiles/*.AsParallel()*/
                                            where System.IO.Path.GetExtension(c.FullName) == patternXaml
                                            select c).ToList();
                            bAdd = listXaml.Count > 0;
                        }
                    }
                    else
                    {   
                        if (Directory.Exists(filePath))
                        {
                            string[] directoryGetFiles = Directory.GetFiles(filePath, searchPatternXaml);
                            bAdd = directoryGetFiles.Length > 0;
                        }
                    }

                    return bAdd;
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    if (pending.Contains(filePath))
                        pending.Remove(filePath);

                    if (ret.Result)
                    {
                        var tabItemSelected = tabControl.SelectedItem as DXTabItem;
                        var selectedLibraryName = tabItemSelected.Header as String;

                        var grid = new Grid();
                        var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        if (XpoHelpers.XpoHelper.IsDataSource(filePath))
                            name = XpoHelpers.XpoHelper.GetDataSourceTitle(filePath);

                        var carousel = new CarouselListControl(workspace, filePath, false, DataContext as SymbolGalleryViewModel, mapItems, fileSystemProvider)
                        {
                            CanBeRemoved = listEditables.Contains(selectedLibraryName),
                            CanEditCode = listEditables.Contains(selectedLibraryName),
                            CanDrag = String.IsNullOrEmpty(styleEditing)
                        };
                        DesignerProperties.SetIsInDesignMode(carousel, true);
                        carousel.CopyLink = mapLibraryBrowsers[selectedLibraryName].CopyLink;
                        grid.Children.Add(carousel);
                        carousel.ClearValue(FrameworkElement.WidthProperty);
                        carousel.ClearValue(FrameworkElement.HeightProperty);

                        NavBarGroup group = new NavBarGroup()
                        {
                            Header = name,
                            DisplaySource = DisplaySource.Content,
                            GroupScrollMode = ScrollMode.None,
                            Content = grid
                        };

                        if (mapItems.ContainsKey(name))
                            group.Header = mapItems[name];

                        navigationControl.Groups.Add(group);
                        AddListBoxLibrary(name, filePath);
                        // newTabItem.IsSelected = true;
                        list.SelectedIndex = list.Items.Count - 1;
                        navigationControl.SelectedGroup = group;
                        mapFileToItem.Add(filePath, group);

                        if (!mapOpenLibraries.ContainsKey(browser))
                            mapOpenLibraries.Add(browser, new List<CarouselListControl>());
                        mapOpenLibraries[browser].Add(carousel);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                task1.ContinueWith((t) =>
                {
                    if (pending.Contains(filePath))
                        pending.Remove(filePath);

                    Debug.WriteLine("I have observed a {0}",
                        t.Exception.InnerException.GetType().Name);
                }, TaskContinuationOptions.OnlyOnFaulted);
                task2.ContinueWith((t) =>
                {
                    if (pending.Contains(filePath))
                        pending.Remove(filePath);

                    Debug.WriteLine("I have observed a {0}",
                        t.Exception.InnerException.GetType().Name);
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        void AddDynLibrary(LibraryBrowser browser, String filePath, FileSystemProviderBase fileSystemProvider)
        {
            if (mapFileToItem.ContainsKey(filePath))
            {
                navigationControl.SelectedGroup = mapFileToItem[filePath];
                list.SelectedIndex = navigationControl.Groups.IndexOf(navigationControl.SelectedGroup as NavBarGroup);
            }
            else
            {
                var tabItemSelected = tabControl.SelectedItem as DXTabItem;
                var selectedLibraryName = tabItemSelected.Header as String;

                var grid = new Grid();
                var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (XpoHelpers.XpoHelper.IsDataSource(filePath))
                    name = XpoHelpers.XpoHelper.GetDataSourceTitle(filePath);

                var newCarousel = new CarouselListControl(workspace, filePath, true, DataContext as SymbolGalleryViewModel, mapItems, fileSystemProvider) 
                { 
                    CanEditCode = listEditables.Contains(selectedLibraryName),
                    CanBeRemoved = listEditables.Contains(selectedLibraryName),
                    CanDrag = String.IsNullOrEmpty(styleEditing)
                };
                DesignerProperties.SetIsInDesignMode(newCarousel, true);
                newCarousel.CopyLink = mapLibraryBrowsers[selectedLibraryName].CopyLink;
                grid.Children.Add(newCarousel);
                newCarousel.ClearValue(FrameworkElement.WidthProperty);
                newCarousel.ClearValue(FrameworkElement.HeightProperty);

                NavBarGroup group = new NavBarGroup()
                {
                    Header = name,
                    DisplaySource = DisplaySource.Content,
                    GroupScrollMode = ScrollMode.None,
                    Content = grid
                };


                if (mapItems.ContainsKey(name))
                    group.Header = mapItems[name];

                navigationControl.Groups.Add(group);
                AddListBoxLibrary(name, filePath);
                list.SelectedIndex = list.Items.Count - 1;
                navigationControl.SelectedGroup = group;
                mapFileToItem.Add(filePath, group);


                if (!mapOpenLibraries.ContainsKey(browser))
                    mapOpenLibraries.Add(browser, new List<CarouselListControl>());
                mapOpenLibraries[browser].Add(newCarousel);
            }
        }

        private void tabControl_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            if (!bLazyLoaded)
                return;
            var item = e.NewSelectedItem as DXTabItem;
            if (item == null)
                return;
            var header = item.Header as String;
            SelectedTabLoad(header);
        }

        private void SelectedTabLoad(string header)
        {
            LibraryBrowser library;
            if (mapLibraryBrowsers.ContainsKey(header))
            {
                mapLibraryBrowsers[header].LoadSymbolGalleryTree();
                mapLibraryBrowsers[header].UpdateMergeCode();
                mapLibraryBrowsers[header].UpdateSelection();
                library = mapLibraryBrowsers[header];
            }
            else
            {
                var selected = tabControl.SelectedItem as DXTabItem;
                library = selected.Content as LibraryBrowser;
            }

            bool bEnable = listEditables.Contains(header);
            btnNewFolder.IsEnabled = bEnable;
            btnDeleteFolder.IsEnabled = bEnable;

            UpdateMergableState(library);
        }

        private void UpdateMergableState(LibraryBrowser library)
        {
            if (library == null)
            {
                IsEditable = false;
                IsMergable = false;
            }
            else
            {
                IsEditable = library.CheckDropLink();
                if (IsEditable)
                    IsMergable = !library.bCopyLink;
                else
                    IsMergable = false;
            }
        }

        private void navigationControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0 && UIGeneralCommands.ZoomIn.CanExecute(null, null))
                    UIGeneralCommands.ZoomIn.Execute(null, this);
                else if (e.Delta < 0 && UIGeneralCommands.ZoomOut.CanExecute(null, null))
                    UIGeneralCommands.ZoomOut.Execute(null, this);
                e.Handled = true;
            }
        }

        void CanZoomIn(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = DataContext as SymbolGalleryViewModel;
            e.CanExecute = viewModel != null && viewModel?.ZoomLevel < Properties.Settings.Default.MaxZoomGallery;
        }

        private void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as SymbolGalleryViewModel;
            viewModel.ZoomLevel += Properties.Settings.Default.ZoomingStep;
        }

        void CanZoomOut(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = DataContext as SymbolGalleryViewModel;
            e.CanExecute = viewModel != null && viewModel?.ZoomLevel > Properties.Settings.Default.MinZoomGallery;
        }

        private void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as SymbolGalleryViewModel;
            viewModel.ZoomLevel -= Properties.Settings.Default.ZoomingStep;
        }

        private void btnProtectKey_Click(object sender, RoutedEventArgs e)
        {
            //ProtectLibrary();
            // using (var cursor = new WaitCursor())
            {
                try
                {
                    var selected = tabControl.SelectedItem as DXTabItem;
                    var library = selected.Content as LibraryBrowser;
                    btnMergeCode.IsEnabled = library.ProtectLibrary();
                }
                catch (Exception ex)
                {
                    SymbolGallery.LibraryHelper.ShowMessage(String.Format(Properties.Resources.LibraryError, ex.Message), true);
                }
            }
        }

        internal static String GetStartingFolder(FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return "Symbols";
            else
            {
                String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                return String.Format("{0}\\Symbols4", startingPath);
            }
        }

        internal static String GetStartingFolder(DocumentManager.ComponentService.IDocument iDocument)
        {
            //if (iDocument.fileSystemProviderBase != null)
            //    return "Symbols";
            //else
            {
                String startingPath = iDocument.rootBase;
                return String.Format("{0}\\Symbols", startingPath);
            }
        }

        internal static String GetStartingUserFolder()
        {
            String startingPath = ApplicationPropertiesHelper.GetProperty<String>("UserFolder");
            return String.Format("{0}\\Symbols", startingPath);
        }

        internal static String GetStartingCommonFolder()
        {
            String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            return String.Format("{0}\\CommonSymbols", startingPath);
        }       

        internal static String GetStartingTypeFolder(FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return "TypeSymbols";
            else
            {
                String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                return String.Format("{0}\\TypeSymbols", startingPath);
            }
        }

        internal static String GetStartingDynamicTypeFolder(FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return STRL.STRL.fileSystemStyleFolder;
            else
            {
                String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                return String.Format("{0}\\{1}", startingPath, STRL.STRL.styleFolder);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var filePath = button.Tag as String;
            CloseLibrary(filePath);
        }

        void CloseLibrary(String filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !mapFileToItem.ContainsKey(filePath))
                return;

            var TabToRemove = mapFileToItem[filePath];
            var grid = TabToRemove.Content as Grid;
            int index = navigationControl.Groups.IndexOf(TabToRemove);
            if (index != 0)
                list.SelectedIndex = 0;
            else if (list.Items.Count > 1)
                list.SelectedIndex = list.Items.Count - 1;

            mapFileToItem[filePath].Content = null;
            mapFileToItem.Remove(filePath);
            if (navigationControl.Groups.Contains(TabToRemove))
                navigationControl.Groups.Remove(TabToRemove);
            var listItem = (from Border item in list.Items where (string)item.Tag == filePath select item).ToList();
            listItem.ForEach(item => list.Items.Remove(item));

            var listCarousel = (from CarouselListControl item in grid.Children select item).ToList();
            listCarousel.ForEach(carousel =>
            {
                var libraryBrowserList = (from c in mapOpenLibraries.Keys
                                          where mapOpenLibraries[c].Contains(carousel)
                                          select c).ToList();

                libraryBrowserList.ForEach(l =>
                {
                    mapOpenLibraries[l].Remove(carousel);
                    if (mapOpenLibraries[l].Count == 0)
                    {
                        mapOpenLibraries[l] = null;
                        mapOpenLibraries.Remove(l);
                    }
                });

                grid.Children.Remove(carousel);
                carousel.Dispose();
            });
        }

        internal void SelectFromTypeDefinition(String type)
        {
            if (String.IsNullOrEmpty(type))
                return;
            type = DependencyObjectExtensions.AdaptName(type);

            var startingFolder = GetStartingTypeFolder();
            var folderPath = String.Format("{0}\\{1}", startingFolder, DependencyObjectExtensions.AdaptName(type));

            var filePath = folderPath;

            if (mapFileToItem.ContainsKey(filePath))
                navigationControl.SelectedGroup = mapFileToItem[filePath];
            else
            {
                if (Directory.Exists(filePath))
                {
                    string[] directoryGetFiles = Directory.GetFiles(filePath, "*.xaml");
                    if (directoryGetFiles.Length > 0)
                    {
                        var tabItemSelected = tabControl.SelectedItem as DXTabItem;
                        var selectedLibraryName = tabItemSelected.Header as String;

                        var grid = new Grid();
                        var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        if (XpoHelpers.XpoHelper.IsDataSource(filePath))
                            name = XpoHelpers.XpoHelper.GetDataSourceTitle(filePath);

                        var carousel = new CarouselListControl(workspace, filePath, false, DataContext as SymbolGalleryViewModel, mapItems)
                                                {
                                                    CanBeRemoved = listEditables.Contains(selectedLibraryName),
                                                    CanEditCode = listEditables.Contains(selectedLibraryName),
                                                    CanDrag = String.IsNullOrEmpty(styleEditing)
                                                };
                        DesignerProperties.SetIsInDesignMode(carousel, true);
                        grid.Children.Add(carousel);
                        carousel.ClearValue(FrameworkElement.WidthProperty);
                        carousel.ClearValue(FrameworkElement.HeightProperty);
                        carousel.CopyLink = mapLibraryBrowsers[selectedLibraryName].CopyLink;

                        NavBarGroup group = new NavBarGroup()
                        {
                            Header = name,
                            DisplaySource = DisplaySource.Content,
                            GroupScrollMode = ScrollMode.None,
                            Content = grid
                        };

                        if (mapItems.ContainsKey(name))
                            group.Header = mapItems[name];

                        navigationControl.Groups.Add(group);
                        AddListBoxLibrary(name, filePath);
                        // newTabItem.IsSelected = true;
                        list.SelectedIndex = list.Items.Count - 1;
                        navigationControl.SelectedGroup = group;
                        mapFileToItem.Add(filePath, group);
                    }
                }
            }
        }
        void AddListBoxLibrary(String name, String filePath)
        {
            DataTemplate item = TryFindResource("ItemTemplate") as DataTemplate;
            var border = item.LoadContent() as Border;
            var stackPanel = border.Child as StackPanel;
            (stackPanel.Children[0] as TextBlock).Text = name;
            (stackPanel.Children[1] as Button).Tag = border.Tag = filePath;
            list.Items.Add(border);
        }

        internal String GetCurrentDropSettings()
        {
            var selectedItem = navigationControl.SelectedGroup as NavBarGroup;
            if (selectedItem == null)
                return String.Empty;

            var carousel = (selectedItem.Content as Grid).Children[0] as CarouselListControl;
            var ret = carousel.GetCurrentDropSettings();

            if (!String.IsNullOrEmpty(ret))
            {
                var foundlib = (from c in mapOpenLibraries.Keys
                                where mapOpenLibraries[c].Contains(carousel)
                                select c).ToList();
                if (foundlib.Count > 0 &&
                    !String.IsNullOrEmpty(foundlib[0].RelativeFolderTag))
                    ret = ret.Replace(foundlib[0].StartingFolder, foundlib[0].RelativeFolderTag);
            }

            return ret;
        }

        internal String GetCurrentSourceSymbolProvider()
        {
            var selectedItem = navigationControl.SelectedGroup as NavBarGroup;
            if (selectedItem == null)
                return String.Empty;

            return ((selectedItem.Content as Grid).Children[0] as CarouselListControl).GetCurrentSourceSymbolProvider();
        }

        internal String GetCurrentSourceSymbolPath(string currentPath)
        {
            var selectedItem = navigationControl.SelectedGroup as NavBarGroup;
            if (selectedItem == null)
                return String.Empty;

            var carousel = (selectedItem.Content as Grid).Children[0] as CarouselListControl;
            var ret = carousel.GetCurrentSourceSymbolPath();

            var foundlib = (from c in mapOpenLibraries.Keys
             where mapOpenLibraries[c].Contains(carousel)
             select c).ToList();
            if (foundlib.Count > 0 &&
                !String.IsNullOrEmpty(foundlib[0].RelativeFolderTag))
                ret = ret.Replace(foundlib[0].StartingFolder, foundlib[0].RelativeFolderTag);
            if (!string.IsNullOrEmpty(currentPath) && currentPath.Contains("@") && ret.Contains("@"))
                ret = $"{currentPath.Split('@').FirstOrDefault()}@{ret.Split('@').LastOrDefault()}";

            return WPFUtilities.CryptString.CryptString.EncryptString(ret);
        }

        internal String GetCurrentSourceSymbolCode()
        {
            var selectedItem = navigationControl.SelectedGroup as NavBarGroup;
            if (selectedItem == null)
                return String.Empty;

            var carousel = (selectedItem.Content as Grid).Children[0] as CarouselListControl;
            var ret = carousel.GetCurrentSourceSymbolCode();

            return ret;
        }

        bool SelectEditableFolder()
        {
            if (listEditables.Count == 0)
            {
                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.NoEditableLibrariesAvailable);
                return false;
            }

            var content = splitterPageLibraries.Content as FrameworkElement;
            if (content == null)
            {
                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.NoEditableLibrariesAvailable);
                return false;
            }

            splitterPageLibraries.Content = null;

            foreach (DXTabItem tab in tabControl.Items)
            {
                var header = tab.Header as String;
                if (!listEditables.Contains(header))
                    tab.Visibility = System.Windows.Visibility.Collapsed;
                else
                    tabControl.SelectedItem = tab;
            }

            bool bRet = false;
            while(true)
            {
                var wnd = new GeneralDialogContent(content)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectLibraryFolder,
                    HelpLink = "SelectLibraryFolder"
                };
                var ret = wnd.ShowDialog();
                bRet = ret != null && ret.Value == true;

                var selected = tabControl.SelectedItem as DXTabItem;
                var browser = selected.Content as LibraryBrowser;
                var filePath = browser.GetSelectedPath();
                if (!bRet || !String.IsNullOrEmpty(filePath))
                    break;
                else
                {
                    filePath = browser.GetDefaultFolder();
                    if(!String.IsNullOrEmpty(filePath))
                        break;
                    else
                        SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.NoLibraryOrFolderSelected);
                }
            }

            foreach (DXTabItem tab in tabControl.Items)
            {
                var header = tab.Header as String;
                if (!listEditables.Contains(header))
                    tab.Visibility = System.Windows.Visibility.Visible;
            }

            using (var cursor = new WaitCursor())
            {
                splitterPageLibraries.Content = content;
            }
            return bRet;
        }

        static void CleanTabControl(DXTabControl tabcontrol)
        {
            foreach (DXTabItem item in tabcontrol.Items)
            {
                if (item.Content is IDisposable)
                    (item.Content as IDisposable).Dispose();
            }
            tabcontrol.Items.Clear();
            tabcontrol.Dispose();
        }

        internal bool AddSymbolToLibrary(string folder, string nameSym, string xaml, string settings)
        {
            if (!SelectEditableFolder())
                return false;

            var selected = tabControl.SelectedItem as DXTabItem;
            /*
            if (!listEditables.Contains(selected.Header as String))
            {
                selected = null;
                foreach (DXTabItem tab in tabControl.Items)
                {
                    if (listEditables.Contains(tab.Header as String))
                    {
                        selected = tab;
                        break;
                    }
                }

                if (selected == null)
                    return false;
                tabControl.SelectedItem = selected;
            }
            */
            var browser = selected.Content as LibraryBrowser;
            var filePath = browser.GetSelectedPath();
            if (String.IsNullOrEmpty(filePath))
                return false;
            if (!mapFileToItem.ContainsKey(filePath))
            {
                var tabItemSelected = tabControl.SelectedItem as DXTabItem;
                var selectedLibraryName = tabItemSelected.Header as String;

                var grid = new Grid();
                var name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (XpoHelpers.XpoHelper.IsDataSource(filePath))
                    name = XpoHelpers.XpoHelper.GetDataSourceTitle(filePath);

                var carousel = new CarouselListControl(workspace, filePath, false,
                                DataContext as SymbolGalleryViewModel, mapItems, browser.FileSystemProvider)
                                {
                                    CanBeRemoved = listEditables.Contains(selectedLibraryName),
                                    CanEditCode = listEditables.Contains(selectedLibraryName),
                                    CanDrag = String.IsNullOrEmpty(styleEditing)
                                };
                DesignerProperties.SetIsInDesignMode(carousel, true);
                carousel.CopyLink = mapLibraryBrowsers[selectedLibraryName].CopyLink;
                grid.Children.Add(carousel);
                carousel.ClearValue(FrameworkElement.WidthProperty);
                carousel.ClearValue(FrameworkElement.HeightProperty);

                NavBarGroup group = new NavBarGroup()
                {
                    Header = name,
                    DisplaySource = DisplaySource.Content,
                    GroupScrollMode = ScrollMode.None,
                    Content = grid
                };

                if (mapItems.ContainsKey(name))
                    group.Header = mapItems[name];

                navigationControl.Groups.Add(group);
                AddListBoxLibrary(name, filePath);
                // newTabItem.IsSelected = true;
                list.SelectedIndex = list.Items.Count - 1;
                navigationControl.SelectedGroup = group;
                mapFileToItem.Add(filePath, group);

                if (!mapOpenLibraries.ContainsKey(browser))
                    mapOpenLibraries.Add(browser, new List<CarouselListControl>());
                mapOpenLibraries[browser].Add(carousel);
            }

            var gridFound = mapFileToItem[filePath].Content as Grid;
            var carouselFound = gridFound.Children[0] as CarouselListControl;
            return carouselFound.AddSymbolToLibrary(filePath, nameSym, xaml, settings);
        }

        public bool UpdateSymbolToLibrary(String xaml, String settings, String provider, String path, String relativePath)
        {
            try
            {
                var symbolPath = WPFUtilities.CryptString.CryptString.DecryptString(path);
                if (symbolPath.StartsWith(projectTag))
                    symbolPath = symbolPath.Replace(projectTag, String.Format("{0}\\Symbols", relativePath));
                var ret = CarouselListControl.UpdateSymbolToLibrary(xaml, settings, provider, symbolPath);

                if (ret)
                {
                    var filePath = System.IO.Path.GetDirectoryName(symbolPath);
                    if (String.IsNullOrEmpty(filePath) || !mapFileToItem.ContainsKey(filePath))
                        return ret;

                    var tab = mapFileToItem[filePath];
                    navigationControl.SelectedGroup = tab;
                    var grid = tab.Content as Grid;
                    var carousel = grid.Children[0] as CarouselListControl;
                    carousel.ReloadElement(symbolPath);
                }

                return ret;
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorUpdatingSymbol, ex);
                return false;
            }
        }

        public void Dispose()
        {
            SaveSettings();

            navigationControl.Groups.Clear();
            foreach (var key in mapFileToItem.Keys)
                mapFileToItem[key].Content = null;
            mapFileToItem.Clear();

            foreach (var key in mapLibraryBrowsers.Keys)
                mapLibraryBrowsers[key].Dispose();

            CleanTabControl(tabControl);
        }

        private void btnLibrary_Click(object sender, RoutedEventArgs e)
        {
            var connectionWizard = new ConnectionWizard(SymbolGalleryComponent.symbolGalleryComponent.UIMsgBoxAlertService, SymbolGalleryComponent.symbolGalleryComponent.HelpProvider);
            var wnd = new GeneralDialogContent(connectionWizard)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "OpenLibrary"
            };
            if (wnd.ShowDialog() == true)
            {
                using (var wait = new WaitCursor())
                {
                    var fileSystemProvider = new DataSourceFileSystemProvider("")
                    {
                        ConnectionString = connectionWizard.ConnectionString
                    };
                    var name = XpoHelpers.XpoHelper.GetDataSourceTitle(fileSystemProvider.ConnectionString);
                    if (String.IsNullOrEmpty(name))
                    {
                        SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.InvalidConnectionString);
                        return;
                    }

                    var ret = AddTabBrowser(name, 
                        Properties.Resources.UserLibraryTooltip,
                        GetStartingFolder(fileSystemProvider), 
                        true, false, fileSystemProvider);
                    ret.IsSelected = true;
                }
            }
        }

        private void btnMergeCode_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as DXTabItem;
            var library = selected.Content as LibraryBrowser;
            library.SetMergeCode();
            UpdateMergableState(library);
        }

        private void btnDropLink_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as DXTabItem;
            var library = selected.Content as LibraryBrowser;
            bool copy;
            library.SetDropLink();
            UpdateMergableState(library);
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as DXTabItem;
            var library = selected.Content as LibraryBrowser;
            library.AddNewFolder();
        }

        private void btnDeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            var selected = tabControl.SelectedItem as DXTabItem;
            var library = selected.Content as LibraryBrowser;
            library.DeleteSelectedFolder();
        }

        DocumentManager.ComponentService.IDocument lastDocument;


        bool lastIsWebHMIToolbox { get; set; }
        internal void AddCurrentProject(DocumentManager.ComponentService.IDocument iDocument)
        {
            if (iDocument != null)
            {
                var parent = iDocument.Parent;
                if (parent == null)
                    parent = iDocument;
                if (parent != lastDocument)
                {
                    var startingFolder = GetStartingFolder(parent);
                    if (iDocument.fileSystemProviderBase == null && !Directory.Exists(startingFolder))
                        Directory.CreateDirectory(startingFolder);
                    lastDocument = parent;
                    RemoveTabBrowser(Properties.Resources.ProjectTab);
                    AddTabBrowser(Properties.Resources.ProjectTab, Properties.Resources.ProjectTabTooltip,
                        startingFolder, true, false,
                        iDocument.fileSystemProviderBase, projectTag);
                }
            }
            else
            {
                RemoveTabBrowser(Properties.Resources.ProjectTab);
            }                       
        }

        #region Isolated Storage Settings

        readonly String StoreFileName = String.Format("{0}.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void SaveSettings()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
                return;

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    log.Warn("AbandonedMutexException", ex);
                }

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(SymbolGalleryViewModel));
                                serializer.WriteObject(writer, model);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                            }
                        }
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        bool LoadSettings()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                isoStorage.GetFileNames(StoreFileName).Length <= 0)
                return false;

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    log.Warn("AbandonedMutexException", ex);
                }

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(SymbolGalleryViewModel));
                                DataContext = model = serializer.ReadObject(reader) as SymbolGalleryViewModel;
                                txtSearch.EditValue = model.TextSearch;
                                return true;
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                            }
                        }
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }

            return false;
        }

        #endregion

        private void ClickSearch(object sender, RoutedEventArgs e)
        {
            model.TextSearch = txtSearch.EditValue as string;
            var selected = tabControl.SelectedItem as DXTabItem;
            var library = selected.Content as LibraryBrowser;
            library.SearchSelectedFolder(model.TextSearch);
        }

        public void LoadFromXml(string filepath)
        {
            //mapItems.Clear();

            try
            {
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(filepath), "resources",true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            mapItems.Add(r.Key.ToString(), r.Value.ToString());
                        });
                    });
                }
            }
            catch (Exception e)
            {

            }

        }
    }
}
