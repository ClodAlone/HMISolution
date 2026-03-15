using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using AvalonDock.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using WpfApp3.Contracts;
using WpfApp3.Plugins;
using WpfApp3.Services;
using WpfApp3.ViewModels;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = true;
        private readonly string _layoutPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "layout.config");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            // Load Settings
            var settings = ConfigurationService.Load();
            this.Top = settings.Top;
            this.Left = settings.Left;
            this.Width = settings.Width;
            this.Height = settings.Height;
            this.WindowState = settings.WindowState;
            ApplyTheme(settings.IsDarkTheme);

            LoadLayout();

            PluginManager.Instance.PluginLoaded += OnPluginLoaded;

            // Load plugins from current assembly
            PluginManager.Instance.LoadPluginsFromAssembly(Assembly.GetExecutingAssembly());

            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            Directory.CreateDirectory(pluginsPath);
            PluginManager.Instance.LoadPluginsFromFolder(pluginsPath);
        }

        private void LoadLayout()
        {
            if (!File.Exists(_layoutPath)) return;

            try
            {
                // Capture existing content
                var contentMap = new Dictionary<string, object>();
                var documents = dockManager.Layout.Descendents().OfType<LayoutDocument>().ToArray();
                var anchorables = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray();

                foreach (var doc in documents)
                {
                    if (doc.ContentId != null && doc.Content != null)
                        contentMap[doc.ContentId] = doc.Content;
                }
                foreach (var anc in anchorables)
                {
                    if (anc.ContentId != null && anc.Content != null)
                        contentMap[anc.ContentId] = anc.Content;
                }

                var serializer = new XmlLayoutSerializer(dockManager);
                serializer.LayoutSerializationCallback += (s, args) =>
                {
                    if (args.Model.ContentId != null && contentMap.ContainsKey(args.Model.ContentId))
                        args.Content = contentMap[args.Model.ContentId];
                };
                
                using var reader = new StreamReader(_layoutPath);
                serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load layout: {ex.Message}");
            }
        }

        private void SaveLayout()
        {
            try
            {
                using var writer = new StreamWriter(_layoutPath);
                var serializer = new XmlLayoutSerializer(dockManager);
                serializer.Serialize(writer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save layout: {ex.Message}");
            }
        }

        private void SwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme(!_isDarkTheme);
        }

        private void ApplyTheme(bool isDark)
        {
            _isDarkTheme = isDark;
            var appResources = Application.Current.Resources.MergedDictionaries;
            appResources.Clear();

            // Always add Styles
            appResources.Add(new ResourceDictionary { Source = new Uri("Themes/Styles.xaml", UriKind.Relative) });

            if (_isDarkTheme)
            {
                appResources.Add(new ResourceDictionary { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) });
                dockManager.Theme = new Vs2013DarkTheme();
            }
            else
            {
                appResources.Add(new ResourceDictionary { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) });
                dockManager.Theme = new Vs2013LightTheme();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var settings = ConfigurationService.Load();

            if (WindowState == WindowState.Normal)
            {
                settings.Top = this.Top;
                settings.Left = this.Left;
                settings.Width = this.Width;
                settings.Height = this.Height;
            }
            else
            {
                settings.Top = this.RestoreBounds.Top;
                settings.Left = this.RestoreBounds.Left;
                settings.Width = this.RestoreBounds.Width;
                settings.Height = this.RestoreBounds.Height;
            }

            settings.WindowState = this.WindowState == WindowState.Minimized ? WindowState.Normal : this.WindowState;
            settings.IsDarkTheme = _isDarkTheme;

            ConfigurationService.Save(settings);
            SaveLayout();
            base.OnClosing(e);
        }

        private void OnPluginLoaded(IPlugin plugin)
        {
            if (plugin.Location == PreferredLocation.Document)
            {
                var existingDocument = dockManager.Layout.Descendents()
                    .OfType<LayoutDocument>()
                    .FirstOrDefault(d => d.ContentId == plugin.Name);

                if (existingDocument != null)
                {
                    if (existingDocument.Content == null)
                        existingDocument.Content = plugin.CreateView();
                    existingDocument.Title = plugin.Header;
                    existingDocument.IsActive = true;
                }
                else
                {
                    var document = new LayoutDocument
                    {
                        Title = plugin.Header,
                        ContentId = plugin.Name,
                        Content = plugin.CreateView()
                    };

                    var documentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                    if (documentPane != null)
                    {
                        documentPane.Children.Add(document);
                        document.IsActive = true;
                    }
                }
                return;
            }

            // Check if this plugin is already in the layout (restored from file)
            var existingAnchorable = dockManager.Layout.Descendents()
                .OfType<LayoutAnchorable>()
                .FirstOrDefault(a => a.ContentId == plugin.Name);

            if (existingAnchorable != null)
            {
                if (existingAnchorable.Content == null)
                    existingAnchorable.Content = plugin.CreateView();

                existingAnchorable.Title = plugin.Header;
            }
            else
            {
                var anchorable = new LayoutAnchorable
                {
                    Title = plugin.Header,
                    ContentId = plugin.Name,
                    Content = plugin.CreateView()
                };

                // Find destination pane
                LayoutAnchorablePane? destinationPane = null;

                if (plugin.Location == PreferredLocation.Right)
                {
                    // Try to finding 'SolutionExplorer' and use its parent
                    var anchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(a => a.ContentId == "SolutionExplorer");
                    if (anchor?.Parent is LayoutAnchorablePane pane)
                        destinationPane = pane;
                }
                else if (plugin.Location == PreferredLocation.Bottom)
                {
                     var anchor = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(a => a.ContentId == "Output");
                    if (anchor?.Parent is LayoutAnchorablePane pane)
                        destinationPane = pane;
                }

                // Fallback to searching by orientation if specific anchor not found
                if (destinationPane == null && plugin.Location == PreferredLocation.Right)
                {
                    // Find a vertical pane or use default if initial layout
                    if (RightPane != null && RightPane.Root != null) destinationPane = RightPane;
                }
                 if (destinationPane == null && plugin.Location == PreferredLocation.Bottom)
                {
                    if (BottomPane != null && BottomPane.Root != null) destinationPane = BottomPane;
                }

                if (destinationPane != null)
                {
                    destinationPane.Children.Add(anchorable);
                    anchorable.IsActive = true;
                }
                else
                {
                    // Just float it if we can't find a home
                    anchorable.AddToLayout(dockManager, AnchorableShowStrategy.Most);
                }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}