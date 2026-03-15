using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.WindowsUI;
using ScreenManager.ComponentService;
using ScreenSettings;
using Utilities;
using StringManager.ComponentService;
using CommandManager;
using UFInterfaces.RemotelyCommandable;
using System.Xml.Serialization;
using UFInterfaces.PropertyControl;

namespace ScreenManager.SpecialObjects
{

    /// <summary>
    /// Interaction logic for EmbeddedTabScreen.xaml
    /// </summary>
    public partial class EmbeddedTabScreen : UserControl, IRemotelyCommandable, IContainPropertyEditors, IDisposable, IStringIDAware
    {
        #region DP

        #region TabBackground
        public static readonly DependencyProperty TabBackgroundProperty = DependencyProperty.Register("TabBackground", typeof(SolidColorBrush), typeof(EmbeddedTabScreen), new UIPropertyMetadata(new SolidColorBrush(Colors.Orange)));
        public SolidColorBrush TabBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SolidColorBrush)GetValue(TabBackgroundProperty);
            }
            set
            {
                SetValue(TabBackgroundProperty, value);
            }
        }

        #endregion

        public static readonly DependencyProperty ScreensProperty = DependencyProperty.Register("Screens", typeof(ScreenList), typeof(EmbeddedTabScreen), new UIPropertyMetadata(null, new PropertyChangedCallback(OnScreensChanged), new CoerceValueCallback(OnCoerceScreens)));

        private static object OnCoerceScreens(DependencyObject o, object value)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                return embeddedTabScreen.OnCoerceScreens((ScreenList)value);
            else
                return value;
        }

        private static void OnScreensChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                embeddedTabScreen.OnScreensChanged((ScreenList)e.OldValue, (ScreenList)e.NewValue);
        }

        protected virtual ScreenList OnCoerceScreens(ScreenList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnScreensChanged(ScreenList oldValue, ScreenList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateHelpTextVisibility(newValue);
                UpdateTabs(newValue);
            }
        }

        public ScreenList Screens
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ScreenList)GetValue(ScreensProperty);
            }
            set
            {
                SetValue(ScreensProperty, value);
            }
        }

        public static readonly DependencyProperty PageHeadersAlignmentProperty = DependencyProperty.Register("PageHeadersAlignment", typeof(PageHeadersAlignment), typeof(EmbeddedTabScreen), new UIPropertyMetadata(PageHeadersAlignment.Top, new PropertyChangedCallback(OnPageHeadersAlignmentChanged), new CoerceValueCallback(OnCoercePageHeadersAlignment)));

        private static object OnCoercePageHeadersAlignment(DependencyObject o, object value)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                return embeddedTabScreen.OnCoercePageHeadersAlignment((PageHeadersAlignment)value);
            else
                return value;
        }

        private static void OnPageHeadersAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                embeddedTabScreen.OnPageHeadersAlignmentChanged((PageHeadersAlignment)e.OldValue, (PageHeadersAlignment)e.NewValue);
        }

        protected virtual PageHeadersAlignment OnCoercePageHeadersAlignment(PageHeadersAlignment value)
        {
            return value;
        }

        protected virtual void OnPageHeadersAlignmentChanged(PageHeadersAlignment oldValue, PageHeadersAlignment newValue)
        {
        }

        public PageHeadersAlignment PageHeadersAlignment
        {
            get
            {
                return (PageHeadersAlignment)base.GetValue(PageHeadersAlignmentProperty);
            }
            set
            {
                base.SetValue(PageHeadersAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty ChosedAnimationProperty = DependencyProperty.Register("ChosedAnimation", typeof(AnimationType), typeof(EmbeddedTabScreen), new UIPropertyMetadata(AnimationType.SlideHorizontal, new PropertyChangedCallback(OnChosedAnimationChanged), new CoerceValueCallback(OnCoerceChosedAnimation)));

        private static object OnCoerceChosedAnimation(DependencyObject o, object value)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                return embeddedTabScreen.OnCoerceChosedAnimation((AnimationType)value);
            else
                return value;
        }

        private static void OnChosedAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EmbeddedTabScreen embeddedTabScreen = o as EmbeddedTabScreen;
            if (embeddedTabScreen != null)
                embeddedTabScreen.OnChosedAnimationChanged((AnimationType)e.OldValue, (AnimationType)e.NewValue);
        }

        protected virtual AnimationType OnCoerceChosedAnimation(AnimationType value)
        {
            return value;
        }

        protected virtual void OnChosedAnimationChanged(AnimationType oldValue, AnimationType newValue)
        {
        }

        public AnimationType ChosedAnimation
        {
            get
            {
                return (AnimationType)GetValue(ChosedAnimationProperty);
            }
            set
            {
                SetValue(ChosedAnimationProperty, value);
            }
        }

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new ScreenManager.SpecialObjects.Controls.EmbeddedTabScreenSmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(EmbeddedTabScreen), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SmartProperties
        {
            get
            {
                return (bool)GetValue(SmartPropertiesProperty);
            }
        }

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        [Browsable(false)]
        public bool NotRunningOnServer
        {
            get
            {
                return !RunningOnServer;
            }
        }

        #endregion

        #region Declarations
        bool bDesignerMode;
        bool bLoaded;
        bool bInit;
        bool bDisposed;
        bool multipleScreens;

        ScreenDocument Document;
        ScreenDocument viewDocument;
        IStringEditorManager stringeditorManager;
        Dictionary<PageViewItem, EmbeddedScreenItem> mapItemTitles = new Dictionary<PageViewItem, EmbeddedScreenItem>();
        #endregion

        void UpdateTabs(List<String> screens)
        {
            ClearItems();
            if (screens == null || bDisposed || screens.Count == 0)
                return;

            Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

            multipleScreens = screens.Count > 1;

            if (Document != null /*&& !bDesignerMode*/)
            {
                stringeditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringeditorManager != null)
                {
                    var cultures = stringeditorManager.GetListAvailableCultures(Document);
                    if (cultures != null && cultures.Count() > 0 && multipleScreens)
                    {
                        stringeditorManager.CultureChanged += stringeditorManager_CultureChanged;
                    }
                }
            }

            if (!multipleScreens)
            {
                pageView.BeginInit();
                PopulatePageView(screens[0], "");
                pageView.EndInit();
            }
            else
            {
                pageView.BeginInit();
                screens.ForEach(screen =>
                {
                    PopulatePageView(screen, System.IO.Path.GetFileNameWithoutExtension(screen));
                });
                pageView.EndInit();
            }

            UpdateLanguage();
            UpdateHeaderVisibility();

            if (pageView.Items.Count > 0)
                pageView.SelectedIndex = 0;
        }
        private void PopulatePageView(String screen, string header)
        {
            var screenPage = new PageViewItem()
            {
                Header = header,
                Tag = screen,
                DataContext = null
            };
            mapItemTitles.Add(screenPage, new EmbeddedScreenItem() { Title = header, Path = screen } );
            pageView.Items.Add(screenPage);
        }
        TextBlock txtHelp;
        private void UpdateHelpTextVisibility(List<string> screens)
        {
            try
            {
                if (screens != null && screens.Count > 0)
                {
                    if (bDesignerMode && screens.Count == 1)
                    {
                        if (txtHelp == null)
                        {
                            txtHelp = new TextBlock()
                            {
                                FontSize = 22,
                                TextWrapping = TextWrapping.Wrap,
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center
                            };
                            if (!mainGrid.Children.Contains(txtHelp))
                                mainGrid.Children.Add(txtHelp);
                        }

                        txtHelp.Text = System.IO.Path.GetFileNameWithoutExtension(screens[0]);
                    }
                    else
                    {
                        if (txtHelp != null)
                        {
                            if (mainGrid.Children.Contains(txtHelp))
                                mainGrid.Children.Remove(txtHelp);
                            txtHelp = null;
                        }
                    }
                }
                else
                {
                    if (txtHelp == null)
                    {
                        txtHelp = new TextBlock()
                        {
                            FontSize = 22,
                            TextWrapping = TextWrapping.Wrap,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        if (!mainGrid.Children.Contains(txtHelp))
                            mainGrid.Children.Add(txtHelp);
                    }


                    if (bDesignerMode)
                        txtHelp.Text = Properties.Resources.EmbeddedTagHelp;
                    else
                        txtHelp.Text = Properties.Resources.EmbeddedTagHelpRuntime;
                }
            }
            catch (Exception)
            {
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<RemoteExecute> GetPendingCommands()
        {
            var ret = new List<RemoteExecute>();
            var doc = viewDocument;
            if (doc != null)
            {
                var l = doc.GetPendingCommands();
                if (l != null)
                    ret.AddRange(l);
            }
            return ret;
        }

        Dictionary<UIElement, String> mapElementCommandToName = new Dictionary<UIElement, String>();
        ScreenDocument preparedDocument;
        void PrepareRemoteCommand()
        {
            var doc = viewDocument;
            if (doc == null)
                return;
            if (preparedDocument == doc)
                return;
            preparedDocument = doc;

            var commandOnNotCommandable = (from c in doc.MapScreenEntities/*.AsParallel()*/
                                           where c.Value.HasCommands &&
                                                 c.Value.Element != null/* && 
                                                             !(c.Value.Element is ICommandSource)*/
                                           select c.Key).ToList();
            commandOnNotCommandable.ForEach(key =>
            {
                var uie = doc.MapScreenEntities[key].Element;
                if (mapElementCommandToName.ContainsKey(uie))
                    mapElementCommandToName.Remove(uie);
                mapElementCommandToName.Add(uie, key);
                if (uie is ContentControl && !(uie is UserControl))
                {
                    var cc = uie as ContentControl;
                    if (cc.Content != null && cc.Content is UIElement)
                    {
                        uie = cc.Content as UIElement;
                        if (mapElementCommandToName.ContainsKey(uie))
                            mapElementCommandToName.Remove(uie);
                        mapElementCommandToName.Add(uie, key);
                    }
                }
            });
        }

        #region IRemotelyCommandable
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckRemoteCommands(UIElement uie, bool bExecute = true)
        {
            var doc = viewDocument;
            if (doc != null)
            {
                PrepareRemoteCommand();
                if (mapElementCommandToName.ContainsKey(uie))
                {
                    if (bExecute)
                        doc.RemoteExecuteCommand(mapElementCommandToName[uie]);
                    return true;
                }
            }
            return false;
        }
        #endregion

        void UpdateLanguage()
        {
            if (stringeditorManager == null || Document == null || bDisposed || !multipleScreens)
                return;

            Dispatcher.BeginInvokeIfRequired((Action)(() =>
            {
                IDictionary<String, String> map = null;
                bool bUntranslated = bDesignerMode && stringeditorManager.GetActiveCulture(Document, false) == String.Empty;
                map = stringeditorManager.GetListStringForCulture(Document, stringeditorManager.GetActiveCulture(Document));

                var list = (from c in pageView.Items.OfType<PageViewItem>() select c).ToList();
                list.ForEach(screen =>
                {
                    if (mapItemTitles.ContainsKey(screen))
                    {
                        var header = mapItemTitles[screen].Title;
                        var path = mapItemTitles[screen].Path;
                        if (bUntranslated)
                        {
                            screen.Header = header;
                        }
                        else if (!String.IsNullOrEmpty(path))
                        {
                            var relative = path.Replace(String.Format("{0}/", Properties.Settings.Default.TypeLabel), "");
                            relative = relative.Replace(String.Format("{0}\\", Properties.Settings.Default.TypeLabel), "");
                            var screenHeader = System.IO.Path.GetFileNameWithoutExtension(relative);
                            if (map != null)
                            {
                                if (header != null && map.ContainsKey(header))
                                    screenHeader = map[header];
                                else if (relative != null) {
                                    if(map.ContainsKey(relative))
                                        screenHeader = map[relative];
                                    else
                                    {
                                        var relativeWithoutExtension = System.IO.Path.ChangeExtension(relative, null);
                                        if (map.ContainsKey(relativeWithoutExtension))
                                            screenHeader = map[relativeWithoutExtension];
                                    }
                                }
                            }
                            screen.Header = screenHeader;
                        }
                    }
                });
            }));
        }

        void stringeditorManager_CultureChanged(object sender, EventArgs e)
        {
            UpdateLanguage();
        }

        void UpdateHeaderVisibility()
        {
            var control = (pageView.Template.FindName("PART_NavigationHeader", pageView)) as FrameworkElement;
            if (control != null)
            {
                control.Visibility = multipleScreens ? Visibility.Visible : Visibility.Collapsed;
                control.Margin = new Thickness(0, 0, 0, 0);
            }

            control = (pageView.Template.FindName("PART_ItemsPresenter", pageView)) as FrameworkElement;
            if (control != null)
                control.Visibility = multipleScreens ? Visibility.Visible : Visibility.Collapsed;

            pageView.PageHeadersLayoutType = multipleScreens ? PageHeadersLayoutType.Scroll : PageHeadersLayoutType.Clip;

        }

        public EmbeddedTabScreen()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;

                        if (bDesignerMode || DesignerProperties.GetIsInDesignMode(this))
                        {
                            bDesignerMode = true;
                            DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                            pageView.BorderThickness = new Thickness(1);
                            pageView.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                            UpdateHelpTextVisibility(Screens);
                            UpdateTabs(Screens);
                        }
                        else
                        {
                            pageView.AnimationType = ChosedAnimation;
                            pageView.SelectionChanged += (ob, ev) =>
                                {
                                    bool bDocumentNotLoaded = false;
                                    var selected = pageView.SelectedItem as PageViewItem;
                                    if (selected != null && selected.Tag is String &&
                                        selected.Content == null)
                                    {
                                        bDocumentNotLoaded = true;
                                        using (var cursor = new WaitCursor())
                                        {
                                            var screen = selected.Tag as String;
                                            // selected.Tag = null;

                                            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                                            if (doc != null)
                                            {
                                                // var editor = ScreenViewer.GetScreenEditor(this) as ScreenManagerComponent;
                                                var editor = doc.GetService(typeof(IScreenManager)) as IScreenManager;
                                                var bTest = ScreenViewer.GetIsInTest(this);

                                                var uri = new Uri(screen, UriKind.RelativeOrAbsolute);
                                                uri = doc.MakeAbosoluteUri(uri);

                                                if (doc.FilePath == uri.GetPathString() || editor == null)
                                                {
                                                    var textError = new TextBlock()
                                                    {
                                                        Text = Properties.Resources.EmbeddedScreenRecursiveNotAllow
                                                    };
                                                    selected.Content = textError;
                                                    viewDocument = null;
                                                }
                                                else
                                                {
                                                    var view = new ScreenViewer(editor as ScreenManagerComponent, uri,
                                                                                    doc.Parent, doc.ActiveView, bTest, true, 
                                                                                    bIsLayout: true, bRunningOnServer:RunningOnServer,
                                                                                    parameterFile: (doc as ScreenDocument)?.ParameterFile);
                                                    selected.Content = view;
                                                    viewDocument = view.Document;
                                                }
                                            }
                                        }
                                    }
                                    if (selected != null && ev.OriginalSource == pageView)
                                    {
                                        var viewer = selected.Content as ScreenViewer;
                                        if (bDocumentNotLoaded && viewer != null)
                                            viewer.Loaded += ActivateDocumentOnViewerLoaded;
                                        else
                                            viewer?.Document?.OnActivated();
                                    }
                                };
                            if (pageView.bIsTemplateReady)
                                OnTemplateLoaded(null, null);
                            else
                                pageView.TemplateReady += OnTemplateLoaded;
                        }
                        bInit = true;
                    }
                };

            //Unloaded += (o, e) =>
            //{
            //    if (bLoaded)
            //    {
            //        bLoaded = false;
            //    }
            //};
        }

        void ActivateDocumentOnViewerLoaded(object sender, RoutedEventArgs e)
        {
            var viewer = (ScreenViewer)sender;
            viewer.Loaded -= ActivateDocumentOnViewerLoaded;
            viewer.Document?.OnActivated();
        }

        bool bTemplateLoaded = false;
        void OnTemplateLoaded(object sender, EventArgs e)
        {
            if (bDisposed || bTemplateLoaded)
                return;
            bTemplateLoaded = true;

            UpdateHelpTextVisibility(Screens);
            UpdateTabs(Screens);
        }

        public PageView PageViewSelector
        {
            get
            {
                return pageView;
            }
        }
        [Browsable(false)]
        public String SelectedItem
        {
            get
            {
                var selected = pageView.SelectedItem as PageViewItem;
                if (selected != null && selected.Tag is String)
                {
                    return selected.Tag as String;
                }

                return null;
            }

            set
            {
                var list = (from c in pageView.Items.OfType<PageViewItem>() where (c.Tag as String) == value select c).ToList();
                if (list.Count > 0)
                    pageView.SelectedItem = list[0];
            }
        }
        internal event EventHandler DragScreenOver;
        private void OnDragScreenOver(EventArgs e)
        {
            EventHandler temp = DragScreenOver;
            if (temp != null)
                temp(null, e);
        }
        public bool DropManager(Object o)
        {
            if (o is Uri)
            {
                var dataUri = o as Uri;

                if (dataUri.GetPathString().Contains(".xaml"))
                {
                    var screen = dataUri.GetPathString();
                    if (Screens == null || !Screens.Contains(screen))
                    {
                        var list = new ScreenManager.SpecialObjects.ScreenList();
                        if (Screens != null)
                            list.AddRange(Screens);
                        list.Add(screen);
                        Screens = list;
                        EventArgs d = new EventArgs();
                        OnDragScreenOver(d);
                        return true;
                    }
                }
            }

            return false;
        }

        void ClearItems()
        {
            if (stringeditorManager != null)
                stringeditorManager.CultureChanged -= stringeditorManager_CultureChanged;

            foreach (var disp in pageView.Items.OfType<PageViewItem>())
            {
                if (disp.Content is ScreenViewer)
                    (disp.Content as ScreenViewer).Dispose();
            }
            viewDocument = null;
            pageView.Items.Clear();
            pageView.TemplateReady -= OnTemplateLoaded;
            mapItemTitles.Clear();
        }

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [Browsable(false)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.SmartPropertiesEditor));
                factory.SetValue(PropertyDataTemplate.SmartPropertiesEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            ClearItems();
        }

        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            if (mapItemTitles == null || mapItemTitles.Count == 0)
                return new List<string>();
            return mapItemTitles.Values.Where(x => !string.IsNullOrEmpty(x.Path)).
                Select(x =>
                {
                    try
                    {
                        var ret = x.Path;
                        if (ret.EndsWith($"{Properties.Settings.Default.DefaultFileExt}", StringComparison.OrdinalIgnoreCase))
                            ret = ret.Remove(ret.Length - (Properties.Settings.Default.DefaultFileExt).Length);
                        if (ret.StartsWith($"{Properties.Settings.Default.TypeLabel}/", StringComparison.OrdinalIgnoreCase))
                            ret = ret.Substring(($"{Properties.Settings.Default.TypeLabel}/").Length);
                        return ret;
                    }
                    catch
                    {
                        return x.Path;
                    }
                }).ToList();
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            mapItemTitles.Where(x => !string.IsNullOrEmpty(x.Value.Path)).
                Select(x => x).ToList().ForEach(x =>
                {
                    try
                    {
                        var ret = x.Value.Path;
                        if (ret.EndsWith($"{Properties.Settings.Default.DefaultFileExt}", StringComparison.OrdinalIgnoreCase))
                            ret = ret.Remove(ret.Length - (Properties.Settings.Default.DefaultFileExt).Length);
                        if (ret.StartsWith($"{Properties.Settings.Default.TypeLabel}/", StringComparison.OrdinalIgnoreCase))
                            ret = ret.Substring(($"{Properties.Settings.Default.TypeLabel}/").Length);
                        map.Add(x.Value.Title, ret);
                    }
                    catch 
                    {
                        map.Add(x.Value.Title, x.Value.Path);
                    }
                });
            return map;
        }
        #endregion
    }

    public class ControlPageView : PageView
    {
        public event EventHandler TemplateReady;
        public bool bIsTemplateReady = false;
        protected override void OnApplyTemplateComplete()
        {
            base.OnApplyTemplateComplete();
            bIsTemplateReady = true;
            TemplateReady?.Invoke(this, null);
        }
    }

    public class EmbeddedScreenItem
    {
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
