using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Utilities;
using System.Threading.Tasks;
using System.ComponentModel;
using ReportManager.ReportService;
using System.Threading;
using ReportSettings.Documents;
using ScreenSettings;
using UFInterfaces.PropertyControl;
using CommandManager.PropertyDataTemplate;
using UFUAEditor.ComponentService;
using log4net;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing.PreviewControl.Bars;
using DevExpress.Mvvm;
using UFInterfaces;
using System.Linq;
using OPCUAViewModel;
using DynamicTagAwareHelper;
using ReportParameters;
using DevExpress.Xpf.Core;
using System.Windows.Threading;
using UFProjectManager.ComponentService;

namespace ReportViewerControl
{
    /// <summary>
    /// Interaction logic for ReportViewer.xaml
    /// </summary>
    public partial class ReportViewer : UserControl, IContainPropertyEditors, IDisposable, IDynamicTagAware
    {
       
        #region Dependency Properties

        /// <summary>
        /// relative Uri for report document
        /// </summary>
        public static readonly DependencyProperty ReportNameProperty = DependencyProperty.Register("ReportName", typeof(Uri), typeof(ReportViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnReportNameChanged), new CoerceValueCallback(OnCoerceReportName)));

        private static object OnCoerceReportName(DependencyObject o, object value)
        {
            ReportViewer reportViewer = o as ReportViewer;
            if (reportViewer != null)
                return reportViewer.OnCoerceReportName((Uri)value);
            else
                return value;
        }

        private static void OnReportNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = o as ReportViewer;
            if (reportViewer != null)
                reportViewer.OnReportNameChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceReportName(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReportNameChanged(Uri oldValue, Uri newValue)
        {
            if (oldValue != newValue)
            {
                if (DesignerProperties.GetIsInDesignMode(this) && bLoaded)
                {
                    TerminateExecution(Parameters);
                    AbortLoading();
                    PrepareDocument();
                }
            }
        }

        public Uri ReportName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(ReportNameProperty);
            }
            set
            {
                SetValue(ReportNameProperty, value);
            }
        }

        /// <summary>
        /// Parameters dependecy property
        /// </summary>
        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ReportParameters.ParameterCollection), typeof(ReportViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnParametersChanged), new CoerceValueCallback(OnCoerceParameters)));

        private static object OnCoerceParameters(DependencyObject o, object value)
        {
            ReportViewer reportViewer = o as ReportViewer;
            if (reportViewer != null)
                return reportViewer.OnCoerceParameters((ReportParameters.ParameterCollection)value);
            else
                return value;
        }

        private static void OnParametersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer reportViewer = o as ReportViewer;
            if (reportViewer != null)
                reportViewer.OnParametersChanged((ReportParameters.ParameterCollection)e.OldValue, (ReportParameters.ParameterCollection)e.NewValue);
        }

        protected virtual ReportParameters.ParameterCollection OnCoerceParameters(ReportParameters.ParameterCollection value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnParametersChanged(ReportParameters.ParameterCollection oldValue, ReportParameters.ParameterCollection newValue)
        {
            if (oldValue != newValue)
            {
                if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded)
                {
                    TerminateExecution(oldValue);
                    AbortLoading();
                    PrepareDocument();
                }
            }
        }

        public ReportParameters.ParameterCollection Parameters
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ReportParameters.ParameterCollection)GetValue(ParametersProperty);
            }
            set
            {
                SetValue(ParametersProperty, value);
            }
        }

        #region WaitTimeout
        /// <summary>
        /// WaitTimeout dependecy property
        /// </summary>
        public static readonly DependencyProperty WaitTimeoutProperty = DependencyProperty.Register("WaitTimeout", typeof(int), typeof(ReportViewer), new UIPropertyMetadata(30, new PropertyChangedCallback(OnWaitTimeoutChanged), new CoerceValueCallback(OnCoerceWaitTimeout)));

        private static object OnCoerceWaitTimeout(DependencyObject o, object value)
        {
            ReportViewer control = o as ReportViewer;
            if (control != null)
                return control.OnCoerceWaitTimeout((int)value);
            else
                return value;
        }

        private static void OnWaitTimeoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ReportViewer control = o as ReportViewer;
            if (control != null)
                control.OnWaitTimeoutChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceWaitTimeout(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWaitTimeoutChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Execution")]
        public int WaitTimeout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(WaitTimeoutProperty);
            }
            set
            {
                SetValue(WaitTimeoutProperty, value);
            }
        }
        #endregion

        #endregion

        #region Declarations
        bool bLoaded;
        bool bUriLoaded;
        bool bUriLoading;
        string loadingError;
        ScreenDocument Document;
        IUFUAEditorManager ufuaEditorService;
        IUFProjectManager iUFProjectManager;
        TypeHelper typeHelper = new TypeHelper();

        DispatcherTimer dispatcherTimer;
        CancellationTokenSource ctsLoading;

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        #endregion

        #region Constructors

        public ReportViewer()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            //var notifier = new PropertyChangeNotifier(this, ScreenDocument.ScreenDocumentProperty);
            //notifier.ValueChanged += (o, e) =>
            //{
            //    PrepareDocument();
            //};

            Loaded += (s, e) =>
            {
                if (bLoaded || bDisposed)
                    return;
                bLoaded = true;

                if (Document == null)
                    Document = ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    preview.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Bars;
                    preview.Visibility = Visibility.Visible;
                    return;
                }

                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                Dispatcher.BeginInvokeAsynchronouslyInRender(() => 
                {
                    var refreshButton = new BarButtonItem()
                    {
                        Content = Properties.Resources.RefreshToolBarCommandText,
                    };
                    refreshButton.SetResourceReference(DevExpress.Xpf.Bars.BarButtonItem.GlyphProperty, "ReloadInverseSmall");
                    refreshButton.SetResourceReference(DevExpress.Xpf.Bars.BarButtonItem.LargeGlyphProperty, "ReloadInverse");

                    refreshButton.Command = new DelegateCommand(ExecuteCreateDocumentCommand, CanExecuteCreateDocumentCommand);

                    bool runningOnServer = ScreenSettings.ScreenDocument.GetRunningOnServer(this);
                    if (runningOnServer)
                    {
                        preview.AutoShowDocumentMap = false;
                        preview.AutoShowParametersPanel = false;

                        preview.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Bars;
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.ClockwiseRotate });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.CounterClockwiseRotate });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.OpenRemoteDocument });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Open });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Close });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Copy });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Save });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.FileGroup });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.NavigationGroup });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.DocumentGroup });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.PrintGroup });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.ToolsGroup });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.DefaultPageCategory });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.DocumentMap });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Parameters });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Watermark });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Thumbnails });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.HandTool });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.EditingFields });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Find });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Search });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Export });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Send });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Print });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.PrintDirect });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.PreviewPage });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Scale });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.ZoomFactor });

                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.PageSetup });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.Pagination });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.NextView });
                        preview.CommandProvider.Actions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.PreviousView });

                        preview.CommandProvider.Actions.Add(new InsertAction()
                        {
                            ContainerName = DefaultPreviewBarItemNames.DocumentGroup,
                            Element = refreshButton
                        });
                    }
                    else
                    {
                        preview.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                        preview.CommandProvider.RibbonActions.Add(new RemoveAction() { ElementName = DefaultPreviewBarItemNames.FileGroup });
                        preview.CommandProvider.RibbonActions.Add(new InsertAction()
                        {
                            ContainerName = DefaultPreviewBarItemNames.DocumentGroup,
                            Element = refreshButton
                        });
                    }


                });

                PrepareDocument();
            };
        }

        #endregion

        #region Implementation

        internal void PrepareDocument()
        {
            if (bUriLoading)
                return;

#if !DEBUG                            
            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w=="/* REP */);
            txtMode.SetZIndex(0);
            txtMode.Visibility = mode ? Visibility.Collapsed : Visibility.Visible;
            if (mode == false)
            {
                mainGrid.IsEnabled = false;
                //logLicense.Warn(Properties.Resources.NoReportLicense);
                iUFProjectManager?.AddLogEntity(Document, Properties.Resources.LicenseManager, 
                    DateTime.UtcNow, Properties.Resources.NoReportLicense, 
                    System.Diagnostics.EventLogEntryType.Warning);
               return;
            }
#endif
            if (Document == null)
                Document = ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (Document != null)
            {
                ufuaEditorService = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (ReportName != null)
                {
                    Uri uri = Document.MakeAbosoluteUri(ReportName);
                    var reportDocument = ReportDocument.FromFile(uri.GetPathString(), Document);
                    if (reportDocument != null && !reportDocument.IsEmpty)
                    {
                        reportDocument.Parent = Document;

                        if (!DesignerProperties.GetIsInDesignMode(this))
                        {
                            SetBusy(true);

                            bUriLoading = true;

                            if (Parameters != null && Parameters.ContainsRelativeEntityReferences())
                                return;
                            
                            if (ctsLoading == null)
                                ctsLoading = new CancellationTokenSource();

                            var parameters = Parameters;
                            var sc = TaskScheduler.FromCurrentSynchronizationContext();
                            var action = new Action(() =>
                            {
                                if (ctsLoading == null || ctsLoading.IsCancellationRequested)
                                    return;
                                var task = Task.Factory.StartNew(() =>
                                {
                                    var helper = new ReportServiceHelper(reportDocument, ufuaEditorService, throwOnException: true);
                                    var report = helper.PrepareXtraReportDocument(parameters);
                                    report.CreateDocument();
                                    return report;
                                }, ctsLoading.Token);
                                task.ContinueWith((ret) =>
                                {
                                    try
                                    {
                                        if (dispatcherTimer != null)
                                        {
                                            dispatcherTimer.Stop();
                                            dispatcherTimer.Tick -= DispatcherTimer_Tick;
                                            dispatcherTimer = null;
                                        }

                                        if (!ctsLoading.IsCancellationRequested)
                                        {
                                            if (ret.Exception == null)
                                            {
                                                try
                                                {
                                                    preview.DocumentSource = ret.Result;
                                                    bUriLoaded = true;
                                                }
                                                catch (Exception ex)
                                                {
                                                    loadingError = String.Format(Properties.Resources.ErrorLoadingReport,
                                                        ReportName.OriginalString, ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                loadingError = String.Format(Properties.Resources.ErrorLoadingReport,
                                                   ReportName.OriginalString, ret.Exception.InnerException.Message);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        bUriLoading = false;
                                        SetBusy(false);
                                        CheckIfLoaded();
                                    }
                                }, sc);
                            });

                            if (dispatcherTimer != null)
                            {
                                dispatcherTimer.Stop();
                                dispatcherTimer.Tick -= DispatcherTimer_Tick;
                                dispatcherTimer = null;
                            }

                            if (parameters != null && parameters.Count > 0)
                            {
                                if (WaitTimeout > 0)
                                {
                                    dispatcherTimer = new DispatcherTimer();
                                    dispatcherTimer.Interval = TimeSpan.FromSeconds(WaitTimeout);
                                    dispatcherTimer.Tick += DispatcherTimer_Tick;
                                    dispatcherTimer.Start();
                                }

                                parameters.Ready += (s, e) =>
                                {
                                    action();
                                };

                                var name = Document.GetEntityName(this, bAdd: false);
                                if (!String.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
                                    parameters.PrepareExecution(Document.SessionString, Document.MapScreenEntities[name], Document);
                            }
                            else
                                action();
                        }
                        else
                            bUriLoaded = true;
                    }
                }
            }

            CheckIfLoaded();
        }

        void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= DispatcherTimer_Tick;
                dispatcherTimer = null;
            }

            SetBusy(false);
            SetError(true, Properties.Resources.LoadingTimeout);
        }

        void AbortLoading()
        {
            if (ctsLoading != null)
            {
                ctsLoading.Cancel();
                ctsLoading.Dispose();
                ctsLoading = null;
            }

            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= DispatcherTimer_Tick;
                dispatcherTimer = null;
            }

            bUriLoaded = false;
            bUriLoading = false;
            loadingError = null;

            SetBusy(false);
            SetError(false);
        }

        void SetBusy(bool bSet)
        {
            if (bSet)
            {
                uriError.Visibility = Visibility.Collapsed;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                uriLoading.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void SetError(bool bSet, string error = null)
        {
            if (bSet)
            {
                preview.Visibility = Visibility.Collapsed;
                uriLoading.Visibility = Visibility.Collapsed;
            }

            if (error != null)
                txtUriError.Text = error;

            uriError.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
        }

        void CheckIfLoaded()
        {
            if (bUriLoaded)
            {
                SetBusy(false);
                SetError(false);

                preview.Visibility = Visibility.Visible;
            }
            else if (!bUriLoading)
            {
                if (ReportName == null)
                    txtUriError.Text = Properties.Resources.EmptyReportUri;
                else if (loadingError != null)
                    txtUriError.Text = loadingError;
                else
                    txtUriError.Text = String.Format(Properties.Resources.InvalidReportUri, ReportName.OriginalString);

                SetBusy(false);
                SetError(true);
            }
        }

        void ExecuteCreateDocumentCommand()
        {
            if (Parameters != null && Parameters.Count > 0)
            {
                TerminateExecution(Parameters);
            }
            AbortLoading();
            PrepareDocument();
        }

        bool CanExecuteCreateDocumentCommand()
        {
            return bUriLoaded;
        }

        #endregion

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

                // Defines Data Template for 'ReportNameProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ReportUriPropertyEditor));
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ReportNameProperty, dt);

                // Defines Data Template for 'ParametersProperty' dependency property.
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ReportParameters.PropertyDataTemplate.ReportParametersPropertyEditor));
                dt.DataType = typeof(ReportParameters.ParameterCollection);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ParametersProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region Methods
        void TerminateExecution(ParameterCollection parameters)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    if (Document == null)
                        Document = ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    if (Document != null)
                    {
                        var name = Document.GetEntityName(this, bAdd: false);
                        if (!String.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
                            parameters.TerminateExecution(Document.MapScreenEntities[name]);
                    }
                }
            }
        }
        public bool DropManager(Object o)
        {
            if (o is Uri)
            {
                var dataUri = o as Uri;

                if (dataUri.GetPathString().Contains(".report"))
                {
                    ReportName = dataUri;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region IDynamicTagAware
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (Parameters != null)
            {
                foreach (var par in Parameters)
                {
                    if (par.TagRef != null)
                        ret.Add(CreateUniqueName(par.NodeId, ret.Keys.ToList()), par.TagRefXml);
                }
            }

            return ret;
        }
        String CreateUniqueName(String name, List<String> list)
        {
            if (!list.Contains(name))
                return name;
            var newname = name;
            int i = 0;
            while (list.Contains(newname))
                newname = String.Format("{0} {1}", name, ++i);

            return newname;
        }

        List<string> matchChangedMap = new List<string>();
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;
            bool ret = false;

            if (Parameters != null && Parameters.Count > 0)
            {
                OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
                OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
                ParameterCollection parList = new ParameterCollection(Parameters);
                var penTagList = (from pen in parList where pen.TagRef != null select pen).ToList();
                foreach (var par in penTagList)
                {
                    //if (par.TagRef != null /*&& par.TagReference.IsValid*/)
                    {
                        try
                        {
                            if (DesignerProperties.GetIsInDesignMode(this))
                                ret = relative == par.TagRefXml;
                            else if (_absolute != null)
                            {
                                if (relative == par.TagRefXml)
                                {
                                    string key = par.NodeId;
                                    matchChangedMap.Add(key);
                                    //TerminateExecution(key);
                                    if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                    {
                                        _relative.Merge(_absolute);
                                        par.TagRef = _relative;
                                    }
                                    else
                                    {
                                        par.TagRef = _absolute;
                                    }
                                    //PrepareExecution(key);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    ret = matchChangedMap.Count == penTagList.Count;
                    if (ret)
                        Parameters = parList;
                }
            }
            return ret;
        }
        Object lockObj = new object();
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            if (Parameters != null && Parameters.Count > 0)
            {
                ParameterCollection parList = new ParameterCollection(Parameters);
                foreach (var par in parList)
                {
                    if (par.TagRef != null)
                    {
                        if (map.ContainsKey(par.NodeId))
                            par.TagRefXml = map[par.NodeId];
                        else
                            par.TagRefXml = typeHelper.UpdateTag(par.TagRefXml, map);
                    }
                }
                Parameters = parList;
            }
        }
        public void PreserveTagsFromMap(Dictionary<string, string> map)
        {
            var tobeupdated = typeHelper.PreserveTagsFromMap(GetMapDynamics(), map);
            UpdateMapDynamics(tobeupdated);
        }
        public void SetConverterLabel(string label)
        {
        }
        #endregion

        #region IDisposable Members

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            DataContext = null;

            AbortLoading();

            if (preview is IDisposable)
                (preview as IDisposable).Dispose();

            if(Parameters != null)
            {
                TerminateExecution(Parameters);
                Parameters.Dispose();
            }
        }

        #endregion
    }
}
