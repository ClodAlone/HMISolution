using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler;
using DevExpress.Xpo;
using MSServerCMS;
using System.ServiceModel;
using MSModel;
using MSSchedulerSettings.Document;
using MSServerInfo;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using OPCUAViewModel;
using ScreenSettings;
using MSSchedulerSettings.Controls;
using Opc.Ua;
using ViewModelLib;
using System.Windows.Threading;
using OPCUAViewModelService.ComponentService;
using System.ComponentModel;
using UFInterfaces;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using System.IO;
using System.Xml;
using UFUserEditor.ComponentService;
using System.Web.Security;
using UFInterfaces.AuthenticationCredentialsProvider;
using DevExpress.Xpf.Editors;
using log4net;
using System.Windows.Media;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Windows.Automation.Peers;
using System.Windows.Media.Effects;
using DocumentManager.ComponentService;
using SchedulerRTControl.PropertyDataTemplate;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using StorageHelper;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;


namespace SchedulerRTControl
{

    /// <summary>
    /// Interaction logic for SchedulerControl.xaml
    /// </summary>
    public partial class SchedulerControl : UserControl, IContainPropertyEditors, IEntityReference, IDisposable, ISettingsHelper, IConnectionAware, IDataErrorInfo
    {
        #region Dependency Properties
        #region DP Foreground
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SchedulerControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            OnForegroundChanged();
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SchedulerControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            OnBackgroundChanged();
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SchedulerControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SchedulerControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as SchedulerControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateForeColor(); 
        }

        private void UpdateForeColor()
        {
            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                if(currentEvent != null)
                    currentEvent.Foreground = Foreground;
            }
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as SchedulerControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled)
                UpdateBackColor();
        }

        private void UpdateBackColor()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                view.Background = Background;
                if (currentEvent != null)
                    currentEvent.Background = Background;
            }
        }
        #endregion        
        
        #region MultiSelectionColor
        public static readonly DependencyProperty MultiSelectionColorProperty = DependencyProperty.Register("MultiSelectionColor", typeof(Brush), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnMultiSelectionColorChanged), new CoerceValueCallback(OnCoerceMultiSelectionColor)));

        private static object OnCoerceMultiSelectionColor(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceMultiSelectionColor((Brush)value);
            else
                return value;
        }

        private static void OnMultiSelectionColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnMultiSelectionColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMultiSelectionColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMultiSelectionColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateSelectedBrush(SelectionColor, newValue);
        }

        [SvgValueConverter(ConverterType = typeof(ConvertSelectionColor), RequiredKey = true)]
        public Brush MultiSelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MultiSelectionColorProperty);
            }
            set
            {
                SetValue(MultiSelectionColorProperty, value);
            }
        }
        #endregion

        #region SelectionColor
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register("SelectionColor", typeof(Brush), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectionColorChanged), new CoerceValueCallback(OnCoerceSelectionColor)));

        private static object OnCoerceSelectionColor(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceSelectionColor((Brush)value);
            else
                return value;
        }

        private static void OnSelectionColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnSelectionColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSelectionColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectionColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateSelectedBrush(newValue, MultiSelectionColor);
        }

        [SvgValueConverter(ConverterType = typeof(ConvertSelectionColor), RequiredKey = true)]
        public Brush SelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SelectionColorProperty);
            }
            set
            {
                SetValue(SelectionColorProperty, value);
            }
        }
        #endregion

        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnControlForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceControlForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Browsable(false)]
        [XmlIgnore]
        //[SvgValueConverter(ConverterType = typeof(ConvertControlForeground), RequiredKey = true)]
        public Brush ControlForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ControlForegroundProperty);
            }
            set
            {
                SetValue(ControlForegroundProperty, value);
            }
        }

        #endregion


        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));
        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            SchedulerControl scheduler = o as SchedulerControl;
            if (scheduler != null)
                return scheduler.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl scheduler = o as SchedulerControl;
            if (scheduler != null)
                scheduler.OnConnectionStringChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionString(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectionStringChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public String ConnectionString
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ConnectionStringProperty);
            }
            set
            {
                SetValue(ConnectionStringProperty, value);
            }
        }

        #endregion
        #region SelectedNodeId
        public static readonly DependencyProperty SelectedNodeIdProperty = DependencyProperty.Register("SelectedNodeId", typeof(String), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedNodeIdChanged), new CoerceValueCallback(OnCoerceSelectedNodeId)));

        private static object OnCoerceSelectedNodeId(DependencyObject o, object value)
        {
            SchedulerControl scheduler = o as SchedulerControl;
            if (scheduler != null)
                return scheduler.OnCoerceSelectedNodeId((String)value);
            else
                return value;
        }

        private static void OnSelectedNodeIdChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl scheduler = o as SchedulerControl;
            if (scheduler != null)
                scheduler.OnSelectedNodeIdChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceSelectedNodeId(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectedNodeIdChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public String SelectedNodeId
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(SelectedNodeIdProperty);
            }
            set
            {
                SetValue(SelectedNodeIdProperty, value);
            }
        }
        #endregion


        #region StartUpScheduler
        public static readonly DependencyProperty StartUpSchedulerProperty = DependencyProperty.Register("StartUpScheduler", typeof(String), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnStartUpSchedulerChanged), new CoerceValueCallback(OnCoerceStartUpScheduler)));

        private static object OnCoerceStartUpScheduler(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceStartUpScheduler((String)value);
            else
                return value;
        }

        private static void OnStartUpSchedulerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnStartUpSchedulerChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceStartUpScheduler(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartUpSchedulerChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && DesignerProperties.GetIsInDesignMode(this))
                InitSchedulerOnDesign();
        }

        public String StartUpScheduler
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(StartUpSchedulerProperty);
            }
            set
            {
                SetValue(StartUpSchedulerProperty, value);
            }
        }

        #endregion

        #region WeeklyEventBrush
        public static readonly DependencyProperty WeeklyEventBrushProperty = DependencyProperty.Register("WeeklyEventBrush", typeof(Brush), typeof(SchedulerControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWeeklyEventBrushChanged), new CoerceValueCallback(OnCoerceWeeklyEventBrush)));

        private static object OnCoerceWeeklyEventBrush(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceWeeklyEventBrush((Brush)value);
            else
                return value;
        }

        private static void OnWeeklyEventBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnWeeklyEventBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceWeeklyEventBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWeeklyEventBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Browsable(false)]
        [XmlIgnore]
        public Brush WeeklyEventBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(WeeklyEventBrushProperty);
            }
            set
            {
                SetValue(WeeklyEventBrushProperty, value);
            }
        }

        #endregion


        #region TimeScale
        public static readonly DependencyProperty TimeScaleProperty = DependencyProperty.Register("TimeScale", typeof(TimeSpan), typeof(SchedulerControl), new UIPropertyMetadata(new TimeSpan(0,1,0,0)));
        [Browsable(false)]
        [XmlIgnore]
        public TimeSpan TimeScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(TimeScaleProperty);
            }
            set
            {
                SetValue(TimeScaleProperty, value);
            }
        }

        #endregion


        #region WorkViewStartTime
        public static readonly DependencyProperty WorkViewStartTimeProperty = DependencyProperty.Register("WorkViewStartTime", typeof(TimeSpan), typeof(SchedulerControl), new UIPropertyMetadata(new TimeSpan(0,8, 0, 0), new PropertyChangedCallback(OnWorkViewStartTimeChanged), new CoerceValueCallback(OnCoerceWorkViewStartTime)));

        private static object OnCoerceWorkViewStartTime(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceWorkViewStartTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnWorkViewStartTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewStartTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewStartTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TimeSpan WorkViewStartTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewStartTimeProperty);
            }
            set
            {
                SetValue(WorkViewStartTimeProperty, value);
            }
        }

        #endregion

        #region WorkViewEndTime
        public static readonly DependencyProperty WorkViewEndTimeProperty = DependencyProperty.Register("WorkViewEndTime", typeof(TimeSpan), typeof(SchedulerControl), new UIPropertyMetadata(new TimeSpan(0, 18, 0, 0), new PropertyChangedCallback(OnWorkViewEndTimeChanged), new CoerceValueCallback(OnCoerceWorkViewEndTime)));

        private static object OnCoerceWorkViewEndTime(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceWorkViewEndTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnWorkViewEndTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewEndTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewEndTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TimeSpan WorkViewEndTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewEndTimeProperty);
            }
            set
            {
                SetValue(WorkViewEndTimeProperty, value);
            }
        }

        #endregion

        #region ShowWorkTimeOnly
        public static readonly DependencyProperty ShowWorkTimeOnlyProperty = DependencyProperty.Register("ShowWorkTimeOnly", typeof(bool), typeof(SchedulerControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowWorkTimeOnlyChanged), new CoerceValueCallback(OnCoerceShowWorkTimeOnly)));

        private static object OnCoerceShowWorkTimeOnly(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceShowWorkTimeOnly((bool)value);
            else
                return value;
        }

        private static void OnShowWorkTimeOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnShowWorkTimeOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowWorkTimeOnly(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowWorkTimeOnlyChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowWorkTimeOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowWorkTimeOnlyProperty);
            }
            set
            {
                SetValue(ShowWorkTimeOnlyProperty, value);
            }
        }

        #endregion


        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(SchedulerControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                return control.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerControl control = o as SchedulerControl;
            if (control != null)
                control.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUserBasedRuntimeSettings(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUserBasedRuntimeSettingsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool UserBasedRuntimeSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UserBasedRuntimeSettingsProperty);
            }
            set
            {
                SetValue(UserBasedRuntimeSettingsProperty, value);
            }
        }

        #endregion

        #endregion

        #region Declarations
        object lockObject = new object();


        Helper helper;
        UnitOfWork schedulerUow;

        MSSchedulerSettings.Controls.NewEventControl currentEvent;
        SchedulerEditorDocument schedulerDocument;
        SimpleScheduledEvent currSimpleScheduler;

        MonitoredItemViewModel monitoredItemViewModel;
        OPCUAEntityReference GetSettingsListMethod;
        OPCUAEntityReference SetSettingsMethod;

        System.Windows.Media.Brush oldBrush;
        internal IDocument document;
        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
        IStringEditorManager stringManager;
        IDictionary<String, String> stringlist;
        IUFUserEditorManager userEditor;
        IUFProjectManager iUFProjectManager;
        bool alreadyLoaded;
        bool enableUserManager;
        string containerName = Properties.Resources.SchedulerName;

        DataValue lastDataValue;
        DispatcherOperation dpUpdateConnStatus;
        
        VariantCollection outputParameters;

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        #endregion

        #region Ctor/Dtor
        //private DelayedSingleActionInvoker SizeChangedInvoker;
        bool bDesignmode;
        bool bInit;
        bool bDataInit;
        bool bTranslateEventOnInit;

        public SchedulerControl()
        {
            InitializeComponent();

            System.Diagnostics.Trace.TraceInformation("SchedulerControl Costruttore {0}.{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond.ToString());

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            previousEffect = (this as UIElement).Effect;
            previousClipToBounds = (this as UIElement).ClipToBounds;

            Loaded += (o, e) =>
            {
                if (alreadyLoaded || bDispose)
                    return;
                alreadyLoaded = true;
               
                document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document);
                InitSchedulerDocument();

                bool runningOnServer = ScreenSettings.ScreenDocument.GetRunningOnServer(this);
                if (runningOnServer)
                    flv.Visibility = Visibility.Visible;

                bDesignmode = DesignerProperties.GetIsInDesignMode(this) || bDesignmode;

                if (document != null)
                {
                    if (stringManager == null)
                        stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                    if (stringManager != null)
                    {
                        StringManager_CultureChanged(document, null);
                        stringManager.CultureChanged += StringManager_CultureChanged;
                    }
                }

                if (bDesignmode)
                {
                    currentEvent = new MSSchedulerSettings.Controls.NewEventControl(schedulerDocument, runningOnServer,styleName:ThemeImageHelper.GetTheme(document));
                    currentEvent.ClearValue(FrameworkElement.WidthProperty);
                    currentEvent.ClearValue(FrameworkElement.HeightProperty);
                    UpdateSelectedBrush(SelectionColor, MultiSelectionColor);
                    //currentEvent.IsHitTestVisible = false;
                    //gridEvent.IsHitTestVisible = false;
                    //GridContainer.IsHitTestVisible = false;

                    InitSchedulerOnDesign();
                }
                else
                {
                    currentEvent = new MSSchedulerSettings.Controls.NewEventControl(schedulerDocument, runningOnServer, bRuntime: true, styleName: ThemeImageHelper.GetTheme(document)) { CurrentCulture = RunningOnServer ? System.Threading.Thread.CurrentThread.CurrentCulture : System.Globalization.CultureInfo.CurrentUICulture };
                    if (runningOnServer)
                        currentEvent.WebDialogUC = WebDialogUC_Event;

                    if (this.ReadLocalValue(WeeklyEventBrushProperty) != DependencyProperty.UnsetValue)
                        currentEvent.WeeklyEventBrush = WeeklyEventBrush;
                    if (this.ReadLocalValue(TimeScaleProperty) != DependencyProperty.UnsetValue)
                        currentEvent.TimeScale = TimeScale;
                    if (this.ReadLocalValue(ShowWorkTimeOnlyProperty) != DependencyProperty.UnsetValue)
                        currentEvent.ShowWorkTimeOnly = ShowWorkTimeOnly;
                    if (this.ReadLocalValue(WorkViewStartTimeProperty) != DependencyProperty.UnsetValue)
                        currentEvent.WorkViewStartTime = TimeSpan.FromMinutes(WorkViewStartTime.TotalMinutes);
                    if (this.ReadLocalValue(WorkViewEndTimeProperty) != DependencyProperty.UnsetValue)
                        currentEvent.WorkViewEndTime = TimeSpan.FromMinutes(WorkViewEndTime.TotalMinutes);

                    currentEvent.ClearValue(FrameworkElement.WidthProperty);
                    currentEvent.ClearValue(FrameworkElement.HeightProperty);
                    UpdateSelectedBrush(SelectionColor, MultiSelectionColor);

                    currentEvent.IsHitTestVisible = false;
                    gridEvent.IsHitTestVisible = false;
                    GridContainer.IsHitTestVisible = false;

                    currentEvent.DisableCoreSettings();

                    btnRefresh.IsEnabled = btnRefreshWeb.IsEnabled = false;
                    oldBrush = btnRefresh.BorderBrush;

                    btnSave.IsEnabled = btnSaveWeb.IsEnabled = false;

                    if (document != null)
                    {
                        userEditor = document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;

                        if (userEditor != null)
                        {
                            try
                            {
                                enableUserManager = userEditor.GetEnableUserManager(document);
                                if (enableUserManager)
                                {
                                    containerName = document.Title;
                                    if (authenticationCredentialsProvider == null)
                                        authenticationCredentialsProvider = document.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                                    if (authenticationCredentialsProvider != null && document.Parent != null)
                                    {
                                        authenticationCredentialsProvider.UserOnline += authenticationCredentialsProvider_UserOnline;
                                        helper = new Helper(document, this);
                                        helper.RefreshCurrentUser();
                                        // authenticationCredentialsProvider.RefreshCurrentUser(document.Parent.Title);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    if (runningOnServer)
                    {
                        WebDialogUC.WebDialogYesClicked += WebDialogYesClick;
                        WebDialogUC.WebDialogNoClicked += WebDialogNoClick;
                    }

                    SetBusy(true);
                }

                gridEvent.Content = currentEvent;
                UpdateBackColor();
                if (runningOnServer && this.ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                    Foreground = flv.Foreground;
                UpdateForeColor();
                OverrideBaseProperties();

                bInit = true;
                if (bTranslateEventOnInit && currentEvent != null)
                {
                    bTranslateEventOnInit = false;
                    MSModel.LocalizedEnumConverter.StringTable = stringlist;
                    currentEvent.TranlslateText(stringlist, stringPlaceolder);
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDispose)
                    return;

                if (DataContext is MonitoredItemViewModel)
                {
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                    monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }

                InitServerConnection();
            };
        }

        private void UpdateSelectedBrush(Brush selectionValue, Brush multiSelectionValue)
        {
            if (currentEvent != null)
            {
                currentEvent.SelectionColor = GetSelectionColor(selectionValue, SelectionColorProperty);
                currentEvent.MultiSelectionColor = GetSelectionColor(multiSelectionValue, MultiSelectionColorProperty);
            }
        }

        internal Brush GetSelectionColor(Brush value, DependencyProperty dependencyProperty)
        {
            if (document == null)
                return value;
            Brush brush = WPFUtilities.ThemeHelper.GetHilightingThemeBrush(ThemeImageHelper.GetTheme(document), dependencyProperty.Name == MultiSelectionColorProperty.Name);
            return this.GetUnsetPropertyValue<Brush>(dependencyProperty, value, brush, false);
        }

        private void WebDialogYesClick(object sender, EventArgs e)
        {
            var currSched = currentEvent != null ? currentEvent.DataContext as MSScheduledAction : null;
            if (currSched != null)
                SaveCurrentScheduler(currSched, nScheduler, scheduler);
        }

        private void WebDialogNoClick(object sender, EventArgs e)
        {
            UpdateCurrentSchedulerData();
        }

        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
                MonitoredItemViewModel m = (MonitoredItemViewModel)sender;

                if (m.IsReadOnly)
                    return;

                bool bForceDispatcherOperation = false;
                lock (lockObject)
                {
                    bForceDispatcherOperation = lastDataValue == null;
                    lastDataValue = m.DataValue;
                }

                if (dpUpdateConnStatus == null || bForceDispatcherOperation ||
                    dpUpdateConnStatus.Status == DispatcherOperationStatus.Completed ||
                    dpUpdateConnStatus.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdateConnStatus = Dispatcher.BeginInvokeAsynchronously(this, () => 
                    {
                        if (bDispose)
                            return;

                        ManageWarning();
                    });
                }
            }
        }

        private void ManageWarning()
        {
            DataValue dataValue = null;
            lock (lockObject)
            {
                dataValue = lastDataValue;
                lastDataValue = null;
            }

            if (dataValue != null && Opc.Ua.StatusCode.IsBad(dataValue.StatusCode))
                SetEntityError(Properties.Resources.ErrorNotConnected);
            else
                SetEntityError(null);
        }

        void InitSchedulerOnDesign()
        {
            var list = GetListSchedulers();
            if (list != null && !string.IsNullOrEmpty(StartUpScheduler))
            {
                var selectedItem = (from s in list where s.FullName == StartUpScheduler select s).FirstOrDefault();
                if (selectedItem != null)
                {
                    SimpleScheduledEvent se = new SimpleScheduledEvent();
                    se.UpdateAction(selectedItem, RunningOnServer);
                    UpdateScheduler(se);

                    cmbVariables.ItemsSource = list;
                    cmbVariables.SelectedItem = selectedItem;
                }
                else
                {
                    UpdateScheduler(null);
                    cmbVariables.ItemsSource = null;
                    cmbVariables.SelectedItem = null;
                }
            }
            else
            {
                UpdateScheduler(null);
                cmbVariables.ItemsSource = null;
                cmbVariables.SelectedItem = null;
            }
        }

        void InitSchedulerOnRuntime(SimpleScheduledEvent schedEvent = null)
        {
            if (bDispose)
                return;
            
            string schedulerName = string.Empty;
            if (this.ReadLocalValue(StartUpSchedulerProperty) != DependencyProperty.UnsetValue && !string.IsNullOrEmpty(StartUpScheduler))
            {
                schedulerName = StartUpScheduler;
            }
            SimpleScheduledEvent selectedItem = null;

            List<SimpleScheduledEvent> list = null;
            SetBusy(true);
            if (RunningOnServer)
            {
                currAccessLevel = ScreenDocument.GetAccessLevel(this);
                currAccessMask = ScreenDocument.GetAccessMask(this);
                currAccessRole = ScreenDocument.GetAccessRole(this);
            }

            var task = Task.Factory.StartNew(delegate
            {
                list = GetListSchedulersFromServer(schedulerName);
            });
            var task1 = task.ContinueWith(ret =>
            {
                try
                {
                    if (list == null)
                    {
                        SetEntityError(Properties.Resources.GetSettingsListMethodException);
                        bRefreshing = false;
                        SetBusy(false);
                        return;
                    }


                    if (this.ReadLocalValue(StartUpSchedulerProperty) != DependencyProperty.UnsetValue && !string.IsNullOrEmpty(StartUpScheduler))
                    {
                        selectedItem = (from s in list where s.FullName == StartUpScheduler select s).FirstOrDefault();
                        cmbVariables.IsEnabled = selectedItem == null;
                    }
                    else if (schedEvent == null)
                    {
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            if (this.ReadLocalValue(StartUpSchedulerProperty) == DependencyProperty.UnsetValue)
                            {
                                var selected = LoadRuntimeLayout(GetStorageName());
                                selectedItem = selected != null ? (from s in list where s.FullName == selected select s).FirstOrDefault() : cmbVariables.SelectedItem as SimpleScheduledEvent;
                            }
                            else
                                selectedItem = cmbVariables.SelectedItem as SimpleScheduledEvent;
                        }
                        else
                            selectedItem = cmbVariables.SelectedItem as SimpleScheduledEvent;
                    }
                    else
                    {
                        selectedItem = (from s in list where s.FullName == schedEvent.FullName select s).FirstOrDefault();
                    }

                    if (!RunningOnServer)
                    {
                        cmbVariables.ItemsSource = null;
                        cmbVariables.ItemsSource = list;
                        if (cmbVariables.ItemsSource != null)
                        {
                            if (selectedItem != null)
                            {
                                cmbVariables.SelectedItem = selectedItem;
                                currSimpleScheduler = selectedItem;
                            }
                            else
                                cmbVariables.SelectedIndex = 0;
                        }
                        btnSave.Visibility = Visibility.Visible;
                        btnRefresh.IsEnabled = btnSave.IsEnabled = true;
                    }
                    else
                    {
                        listVariables.ItemsSource = null;
                        listVariables.ItemsSource = list;
                        if (listVariables.ItemsSource != null)
                            if (selectedItem != null)
                                listVariables.SelectedItem = selectedItem;
                            else
                                listVariables.SelectedIndex = 0;

                        //gridEvent.GetVisualChildrenOfType<FrameworkElement>().ToList().ForEach(x => x.IsHitTestVisible = false);
                        schedSelectionWeb.Visibility = Visibility.Visible;
                        schedSelectionDesign.Visibility = Visibility.Collapsed;
                        cmbVariables.IsEnabled = false;
                        btnSaveWeb.Visibility = Visibility.Visible;
                        btnSave.IsEnabled = btnRefresh.IsEnabled = false;
                        btnSaveWeb.IsEnabled = btnRefreshWeb.IsEnabled = true;
                    }

                    btnRefresh.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReloadData", stringlist, Properties.Resources.ReloadData);
                    btnRefresh.BorderBrush = oldBrush;

                    btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerSave", stringlist, Properties.Resources.SchedulerSave);
                    btnSave.BorderBrush = btnSaveWeb.BorderBrush = oldBrush;

                    currentEvent.IsHitTestVisible = true;
                    gridEvent.IsHitTestVisible = true;
                    GridContainer.IsHitTestVisible = true;
                }
                finally
                {
                    bRefreshing = false;
                    bDataInit = true;
                    SetBusy(false);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        int currAccessLevel = -1;
        int currAccessMask = defaultAccessMask;
        string currAccessRole;
        String lastUser;
        void authenticationCredentialsProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            if (lastUser == e.User)
                return;
            lastUser = e.User;

            if (userEditor == null || document == null || !enableUserManager)
                return;

            if (String.IsNullOrEmpty(e.User))
            {
                //logout
                currAccessRole = null;
                currAccessLevel = -1;
                currAccessMask = defaultAccessMask;
            }
            else
            {
                //login
                currAccessRole = userEditor.GetUserRole(document, e.User);
                currAccessMask = userEditor.GetUserAccessMask(document, e.User);
                currAccessLevel = userEditor.GetUserAccessLevel(document, e.User);
            }
            List<SimpleScheduledEvent> list = null;
            string schedulerName = string.Empty;
            if (this.ReadLocalValue(StartUpSchedulerProperty) != DependencyProperty.UnsetValue && !string.IsNullOrEmpty(StartUpScheduler))
            {
                schedulerName = StartUpScheduler;
            }
            SetBusy(true);
            var task = Task.Factory.StartNew(delegate
            {
                list = GetListSchedulersFromServer(schedulerName);
            });
            var task1 = task.ContinueWith(ret =>
            {
                if(list == null)
                    SetEntityError(Properties.Resources.GetSettingsListMethodException);

                var newusername = e.User;
                //update events list
                var oldevent = cmbVariables.SelectedItem as SimpleScheduledEvent;

                cmbVariables.ItemsSource = null;
                cmbVariables.ItemsSource = list;
                if (cmbVariables.Items.Contains(oldevent))
                    cmbVariables.SelectedItem = oldevent;
                else if (cmbVariables.Items.Count > 0)
                    cmbVariables.SelectedIndex = 0;
                else
                    UpdateScheduler(null);

                SetBusy(false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void CleanAll()
        {
            DetachOverrideBaseProperties();

            if (observerMonitoredModel != null)
            {
                try
                {
                    observerMonitoredModel.UnregisterHandler(p => p.LastMessage);
                    observerMonitoredModel.UnregisterHandler(p => p.DataValue);
                }
                catch
                { }
                observerMonitoredModel.Dispose();
            }

            if (observer != null)
            {
                try
                {
                    observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                }
                catch
                { }
                observer.Dispose();
            }

            if (schedulerUow != null)
            {
                schedulerUow.Disconnect();
                schedulerUow.Dispose();
                schedulerUow = null;
            }

            if (schedulerDocument != null)
                schedulerDocument.Dispose();
            schedulerDocument = null;

            if (currentEvent != null && currentEvent is IDisposable)
                (currentEvent as IDisposable).Dispose();
            currentEvent = null;

            var currList = cmbVariables.ItemsSource as List<MSScheduledAction>;
            if (currList != null)
            {
                currList.Clear();
            }
            
            if (authenticationCredentialsProvider != null)
                authenticationCredentialsProvider.UserOnline -= authenticationCredentialsProvider_UserOnline;

            if (GetSettingsListMethod != null && GetSettingsListMethod.IsValid)
                GetSettingsListMethod.SetInUse(this, false);
            
            if (SetSettingsMethod != null && SetSettingsMethod.IsValid)
                SetSettingsMethod.SetInUse(this, false);

            gridEvent.Content = null;
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

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                if (document == null)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                IUIMsgBoxAlertService UIInterface = null;
                IHelpProvider helpProvider = null;
                if (document != null)
                {
                    UIInterface = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    helpProvider = document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                }
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(SchedulerSettingsPropertyEditor));
                factory.SetValue(SchedulerSettingsPropertyEditor.DocumentProperty, document);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                mapDataTemplates.Add(StartUpSchedulerProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TimeSpanPropertyEditor));
                factory.SetValue(WPFUtilities.PropertyDataTemplate.TimeSpanPropertyEditor.TimeSpanFormatProperty, "hh:mm");
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                mapDataTemplates.Add(WorkViewStartTimeProperty, dt);
                mapDataTemplates.Add(WorkViewEndTimeProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion
        #region IDisposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            WebDialogUC.WebDialogYesClicked -= WebDialogYesClick;
            WebDialogUC.WebDialogNoClicked -= WebDialogNoClick;
            WebDialogUC.Dispose();

            if (!bDesignmode && this.ReadLocalValue(StartUpSchedulerProperty) == DependencyProperty.UnsetValue)
                SaveRuntimeLayout(GetStorageName());

            if (helper != null)
                helper.Dispose();

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (dpUpdateConnStatus != null &&
                dpUpdateConnStatus.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateConnStatus.Status != DispatcherOperationStatus.Completed)
                dpUpdateConnStatus.Abort();

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            CleanAll();
            //GC.SuppressFinalize(this);
        }
        #endregion

        #region Custom automation peers
        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new ValueAutomationPeer(this);
        //}
        #endregion


        #region Methods
        string stringPlaceolder = "SchedulerControl";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
                else
                    stringlist = null;

                MSModel.LocalizedEnumConverter.StringTable = stringlist;

                schedName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerName", stringlist, Properties.Resources.SchedulerName);
                schedName1.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerName", stringlist, Properties.Resources.SchedulerName);
                btnRefresh.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReloadData", stringlist, Properties.Resources.ReloadData);
                btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerSave", stringlist, Properties.Resources.SchedulerSave);

                if (currentEvent != null)
                {
                    currentEvent.CurrentCulture = RunningOnServer ? System.Threading.Thread.CurrentThread.CurrentCulture : System.Globalization.CultureInfo.CurrentUICulture;
                    currentEvent.TranlslateText(stringlist, stringPlaceolder);
                }
                else if (!bInit)
                    bTranslateEventOnInit = true;
            });
        }

        bool bRefreshing;
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshSchedulerData();
        }
        private void RefreshSchedulerData()
        {
            if (bDispose || !bInit || !bDataInit || bRefreshing)
                return;

            bRefreshing = true;
            InitSchedulerOnRuntime(currSimpleScheduler);
        }

        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> observerMonitoredModel;

        PropertyObserver<OPCUAEntityReference> observerGetList;
        public void PrepareExecution(String sessionname)
        {

            if (GetSettingsListMethod != null && GetSettingsListMethod.IsValid)
            {
                if (observerGetList != null)
                {
                    observerGetList.UnregisterHandler(p => p.NodeIdViewModel);
                    observerGetList.Dispose();
                }

                observerGetList = new PropertyObserver<OPCUAEntityReference>(GetSettingsListMethod);
                observerGetList.RegisterHandler(n => n.NodeIdViewModel, n => 
                {
                    observerGetList.UnregisterHandler(p => p.NodeIdViewModel);
                    Dispatcher.BeginInvokeIfRequired(() =>
                    {
                        SetEntityError(null);
                        InitSchedulerOnRuntime();
                    });
                });

                GetSettingsListMethod.Resolve(sessionname, document);
                GetSettingsListMethod.SetInUse(this, true);
            }

            if (SetSettingsMethod != null && SetSettingsMethod.IsValid)
            {
                SetSettingsMethod.Resolve(sessionname, document);
                SetSettingsMethod.SetInUse(this, true);
            }

        }

        Effect previousEffect = null;
        bool previousClipToBounds = true;
        private void SetEntityError(String error)
        {
            if (String.IsNullOrEmpty(error))
            {
                Effect = previousEffect;
                ClipToBounds = previousClipToBounds;
                ToolTip = string.Empty;
            }
            else
            {
                var effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 10,
                    Color = Colors.Red
                };
                Effect = effect;
                ClipToBounds = false;
                ToolTip = error;
            }
        }
        private void cmbVariables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bDesignmode)
                return;
            if (sender is ComboBox && (sender as ComboBox).SelectedItem != null)
            {
                UpdateScheduler((sender as ComboBox).SelectedItem as SimpleScheduledEvent);
                RefreshSchedulerData();
            }
            else if (sender is ListBox && (sender as ListBox).SelectedItem != null)
            {
                UpdateScheduler((sender as ListBox).SelectedItem as SimpleScheduledEvent);
                RefreshSchedulerData();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (currentEvent == null || currSimpleScheduler == null)
                return;

            var currSched = currentEvent != null ? currentEvent.DataContext as MSScheduledAction : null;
            if (currSched != null && SetSettingsMethod != null && SetSettingsMethod.NodeIdViewModel != null)
            {
                SimpleScheduledEvent se = new SimpleScheduledEvent();
                se.UpdateAction(currSched, RunningOnServer);
                se.NodeId = currSimpleScheduler.NodeId;
                if(currSimpleScheduler.Compare(se))
                {
                    //runtime properties not changed...
                    return;
                }


                Exception exCaptured = null;
                SetBusy(true);
                var task1 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        outputParameters = SetSettingsMethod.NodeIdViewModel.CallMethod(se.ToXml());
                    }
                    catch (Exception ex)
                    {
                        exCaptured = ex;
                    }
                });
                task1.ContinueWith(ret =>
                {
                    if (exCaptured != null)
                    {
                        SetEntityError(Properties.Resources.SetSettingsMethodException);
                        var syslog = LogManager.GetLogger(containerName);
                        syslog.Error(String.Format(Properties.Resources.SetSettingsMethodException), exCaptured);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(document, containerName,
                            DateTime.UtcNow, $"{Properties.Resources.SetSettingsMethodException}: {exCaptured.Message}",
                            System.Diagnostics.EventLogEntryType.Error);
                        btnSave.BorderBrush = btnSaveWeb.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SetSettingsMethodException", stringlist, Properties.Resources.SetSettingsMethodException);
                    }
                    else if (outputParameters != null && outputParameters.Count > 0 && Convert.ToBoolean(outputParameters[0].Value))
                    {
                        btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerSave", stringlist, Properties.Resources.SchedulerSave);
                        btnSave.BorderBrush = btnSaveWeb.BorderBrush = oldBrush;

                        var list = cmbVariables.ItemsSource as List<SimpleScheduledEvent>;
                        if (list != null)
                        {
                            var seMod = list.Find((o => { return o.NodeId == se.NodeId; }));
                            if (seMod != null)
                                seMod.Update(se, RunningOnServer);
                        }
                        currSimpleScheduler.Update(se, RunningOnServer);
                    }
                    else
                    {
                        btnSave.BorderBrush = btnSaveWeb.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SetSettingsMethodError", stringlist, Properties.Resources.SetSettingsMethodError);
                    }
                    SetBusy(false);
                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
        }

        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        private String GetConnectionString(String activeconnection, String xmlExt)
        {
            if (String.IsNullOrEmpty(activeconnection) || String.IsNullOrEmpty(xmlExt))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = ds.Replace('/', '\\');
                ds = String.Format("{0}.{1}", ds, xmlExt);

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = String.Format("{0}_{1}.mdb", ds, xmlExt);

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(CatalogSourceHeader);
                if (!String.IsNullOrEmpty(ds))
                    ds = String.Format("{0}_{1}", ds, xmlExt);

                helper.UpdatePartByName(CatalogSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else
                return helper.GetConnectionString();
        }

        bool prepared = false;
        private void InitServerConnection()
        {
            if (!prepared)
            {
                var server = monitoredItemViewModel;
                string sessionname = string.Empty;
                var doc = document as ScreenDocument;
                if (doc != null && !string.IsNullOrEmpty(doc.SessionString))
                    sessionname = doc.SessionString;

                if (server != null)
                {
                    string appName = string.Empty;
                    var subscription = server.Parent as SubscriptionViewModel;
                    if(subscription != null && subscription.Parent != null && subscription.Parent is SessionViewModel)
                        appName = (subscription.Parent as SessionViewModel).AppName;

                    if (string.IsNullOrEmpty(appName))
                        return;
                        
                    if (GetSettingsListMethod == null)
                        GetSettingsListMethod = SchedulerEditorDocument.GetGeneralOPCUAEntityReference(appName, server.EndpointUrl,
                        string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSchedulersListMethodName()),
                        MSServerInfo.MSServerInfo.GetSchedulersListMethodGuid(),
                        MSServerInfo.MSServerInfo.GetSchedulersListMethodName());

                    if (SetSettingsMethod == null)
                        SetSettingsMethod = SchedulerEditorDocument.GetGeneralOPCUAEntityReference(appName, server.EndpointUrl,
                        string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName()),
                        MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodGuid(),
                        MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName());

                    if(string.IsNullOrEmpty(sessionname))
                        sessionname = GetSettingsListMethod.AppName;

                    PrepareExecution(sessionname);
                }
                else if (schedulerDocument != null)
                {
                    GetSettingsListMethod = schedulerDocument.GetNodeIdOPCUAEntityReference(
                        string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSchedulersListMethodName()),
                        MSServerInfo.MSServerInfo.GetSchedulersListMethodGuid());
                    SetSettingsMethod = schedulerDocument.GetNodeIdOPCUAEntityReference(
                        string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName()),
                    MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodGuid());

                    if (string.IsNullOrEmpty(sessionname))
                        sessionname = GetSettingsListMethod.AppName;
                    PrepareExecution(sessionname);
                }
                prepared = true;
            }
        }

        void InitSchedulerDocument()
        {
            if (document == null || schedulerDocument != null)
                return;

            if (!String.IsNullOrEmpty(ConnectionString))
            {
                schedulerDocument = MSSchedulerSettings.Document.SchedulerEditorDocument.FromConnectionString(XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, document.rootBase), document);
            }
            else
            {
                var uri = new Uri(document.Parent.rootBase, UriKind.RelativeOrAbsolute);
                schedulerDocument = MSSchedulerSettings.Document.SchedulerEditorDocument.FromFile(uri.GetPathString(), null, document, bCreateNew: true, bCheckEmpty: true);
            }

            if (schedulerDocument != null)
                schedulerUow = schedulerDocument.BeginNestedUnitOfWork();
        }

        const int defaultAccessMask = 0;

        public List<SimpleScheduledEvent> GetListSchedulersFromServer(string SchedulerName = null)
        {
            List<SimpleScheduledEvent> lista = new List<SimpleScheduledEvent>();
            if (GetSettingsListMethod != null && GetSettingsListMethod.NodeIdViewModel != null)
            {
                try
                {
                    outputParameters = GetSettingsListMethod.NodeIdViewModel.CallMethod(SchedulerName == null ? string.Empty : SchedulerName);
                }
                catch (Exception ex)
                {
                    var syslog = LogManager.GetLogger(containerName);
                    syslog.Error(String.Format(Properties.Resources.GetSettingsListMethodException), ex);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(document, containerName,
                        DateTime.UtcNow, $"{Properties.Resources.GetSettingsListMethodException}: {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                    return null;
                }
                if (outputParameters.Count > 0)
                {
                    System.Diagnostics.Trace.TraceInformation("GetServerListSchedulers lista {0}.{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond.ToString());
                    foreach (var s in outputParameters)
                    {
                        string scheddef = s.ToString();
                        var sched = scheddef.FromXml<SimpleScheduledEvent>();
                        if (sched != null)
                            lista.Add(sched);
                    }
                }
            }

            if (enableUserManager)
            {
                var filteredlist = (from tt in lista
                                    where (
                                    (tt.AccessRole == null || tt.AccessRole.Length == 0 || tt.AccessRole == currAccessRole) &&
                                        (tt.AccessLevels == 0 || tt.AccessLevels <= currAccessLevel) &&
                                        (tt.AccessMasks == 0 || (tt.AccessMasks & currAccessMask) != 0)
                                        ) orderby tt.FullName ascending
                                    select tt).ToList();
                return filteredlist;
            }
            else
                return lista.OrderBy(s => s.FullName).ToList();
                
        }
        
        List<MSScheduledAction> GetListSchedulers()
        {
            if (schedulerDocument == null)
                return null;
            
            var currlist = schedulerDocument.GetCompleteEventsListOrdered(true).ToList<MSModel.MSScheduledAction>();
            if (currlist == null)
                return null;

            if (enableUserManager)
            {
                var filteredlist = (from tt in currlist
                                    where (tt.AccessRole == null || (tt.AccessRole.Length == 0 || (tt.AccessRole == currAccessRole)) &&
                                        (tt.AccessLevels == 0 || (tt.AccessLevels <= currAccessLevel)) &&
                                        (tt.AccessMasks == 0 || (tt.AccessMasks & currAccessMask) != 0)
                                        )
                                    select tt).ToList();

                return filteredlist;
            }
            else
                return currlist;
        }

        MSScheduledAction nScheduler;
        SimpleScheduledEvent scheduler;
        void UpdateScheduler(SimpleScheduledEvent scheduler)
        {
            if (scheduler == null)
            {
                if (currentEvent != null)
                {
                    currentEvent.DataContext = null;
                    currentEvent.UpdateData();
                }
                return;
            }

            this.scheduler = scheduler;

            var p = (from tag in new XPQuery<MSModel.MSScheduledAction>(schedulerUow, true)
                     where tag.MSFolderAss == null && tag.Name == scheduler.FullName
                     select tag).ToList();
            if (p.Count > 0)
                nScheduler = p[0];
            else
                nScheduler = new MSScheduledAction(schedulerUow);

            nScheduler.Fill(scheduler);

            var currSched = currentEvent != null ? currentEvent.DataContext as MSScheduledAction : null;
            bool bConfirmed = false;
            bool bShowDialog = !bRefreshing && !bDesignmode && currSimpleScheduler != null && currSched != null && currSched.NodeId == currSimpleScheduler.Guid && currSimpleScheduler.Modified(currSched, RunningOnServer);
            if (bShowDialog)
            {
                if (!RunningOnServer)
                {
                    bConfirmed = System.Windows.Forms.MessageBox.Show(Properties.Resources.SaveQuestion, Properties.Resources.SchedulerSave, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
                    if (bConfirmed)
                        SaveCurrentScheduler(currSched, nScheduler, scheduler);
                }
                else
                    WebDialogUC.Show(Properties.Resources.SaveQuestion);
            }
            if (!RunningOnServer && !bConfirmed && currentEvent != null)
                UpdateCurrentSchedulerData();
            else if (RunningOnServer && !bShowDialog)
                UpdateCurrentSchedulerData();
        }

        void UpdateCurrentSchedulerData()
        {
            if (scheduler == null || nScheduler == null)
                return;
            currentEvent.DataContext = nScheduler;
            currentEvent.UpdateData();
            currSimpleScheduler = scheduler;
        }

        void SaveCurrentScheduler(MSScheduledAction currSched, MSScheduledAction nScheduler, SimpleScheduledEvent scheduler)
        {
            //save currSched, before passing to the new one...
            SimpleScheduledEvent se = new SimpleScheduledEvent(); ;
            se.UpdateAction(currSched, RunningOnServer);
            if (currSimpleScheduler != null)
                se.NodeId = currSimpleScheduler.NodeId;

            Exception exCaptured = null;
            //save se
            SetBusy(true);
            var task1 = Task.Factory.StartNew(() =>
            {
                try
                {
                    outputParameters = SetSettingsMethod.NodeIdViewModel.CallMethod(se.ToXml());
                }
                catch (Exception e)
                {
                    exCaptured = e;
                }
            });
            task1.ContinueWith(ret =>
            {
                if (exCaptured != null)
                {
                    SetEntityError(Properties.Resources.SetSettingsMethodException);
                    var syslog = LogManager.GetLogger(containerName);
                    syslog.Error(String.Format(Properties.Resources.SetSettingsMethodException), exCaptured);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(document, containerName,
                        DateTime.UtcNow, $"{Properties.Resources.SetSettingsMethodException}: {exCaptured.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                }
                else
                {
                    if (outputParameters != null && outputParameters.Count > 0 && Convert.ToBoolean(outputParameters[0].Value))
                    {
                        var list = cmbVariables.ItemsSource as List<SimpleScheduledEvent>;
                        if (list != null)
                        {
                            var seMod = list.Find((o => { return o.NodeId == se.NodeId; }));
                            if (seMod != null)
                                seMod.Update(se, RunningOnServer);
                        }
                        currSimpleScheduler.Update(se, RunningOnServer);
                        btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SchedulerSave", stringlist, Properties.Resources.SchedulerSave);
                        btnSave.BorderBrush = btnSaveWeb.BorderBrush = oldBrush;
                    }
                    else
                    {
                        btnSave.BorderBrush = btnSaveWeb.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        btnSave.ToolTip = btnSaveWeb.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SetSettingsMethodError", stringlist, Properties.Resources.SetSettingsMethodError);
                    }

                    if (currentEvent != null)
                    {
                        currentEvent.DataContext = nScheduler;
                        currentEvent.UpdateData();
                        currSimpleScheduler = scheduler;
                    }
                }
                SetBusy(false);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy)
            {
                busyControl.Visibility = Visibility.Visible;
            }
            else
            {
                busyControl.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Isolated Storage
        String GetStorageName(bool useParent = false)
        {
            return StorageHelper.StorageHelper.GetStorageName(document, this.Name, UserBasedRuntimeSettings ? helper?.Username : null, true);
        }

        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
        }

        static IsolatedStorageFile GetStorage()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }

        void SaveRuntimeLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.Create, isoStorage))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        try
                        {
                            writer.Write((cmbVariables.SelectedItem as SimpleScheduledEvent).FullName);
                        }
                        finally
                        {
                            writer.Close();
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        string LoadRuntimeLayout(String title)
        {
            string ret = string.Empty;
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return ret;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.OpenOrCreate, isoStorage))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            ret = reader.ReadToEnd();
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return ret;
        }
        #endregion

        #region ISettingsHelper
        public int EditingWriteAccessLevel { get; }
        public int EditingWriteAccessMask { get; }
        public void UpdateWriteAccessCommands()
        {

        }

        public void ReloadRuntimeSettings()
        {

        }

        public void Initialize()
        {
        }
        #endregion

        #region IConnectionAware
        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
        }
        #endregion
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarBackground), RequiredKey = true)]
        public Brush ToolbarBackground { get { return Background; } }
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarForeground), RequiredKey = true)]
        public Brush ToolbarForeground { get { return Foreground; } }


        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "WorkViewStartTime" || propertyName == "WorkViewEndTime")
            {
                if (WorkViewEndTime.CompareTo(WorkViewStartTime) <= 0)
                    return Properties.Resources.WorkViewTimeError;
            }
            return null;
        }
        #endregion
    }

    internal class ConvertSelectionColor : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null || !(sender is SchedulerControl))
                return null;
            (sender as SchedulerControl).document = document as IDocument;
            return (sender as SchedulerControl).GetSelectionColor(value as Brush, property as DependencyProperty);
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertControlForeground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null || !(sender is SchedulerControl))
                return null;
            return (sender as SchedulerControl).Foreground;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is SchedulerControl)
            {
                SchedulerControl control = sender as SchedulerControl;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(SchedulerControl.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                ret.Add("ControlForeground", ret["Foreground"]);

                if (control.ReadLocalValue(SchedulerControl.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            return value;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertDefaultToolbarBackground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            SchedulerControl control = sender as SchedulerControl;
            Brush defColor = control.Foreground;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;

            if (background is SolidColorBrush)
            {
                SolidColorBrush solidColorBrush = (background as SolidColorBrush);

                return new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast(solidColorBrush.Color, (document as ScreenDocument).Theme));
            }
            else if (background is LinearGradientBrush)
            {
                LinearGradientBrush linearGradientBrush = background.Clone() as LinearGradientBrush;
                if (linearGradientBrush.GradientStops.Count > 0)
                {
                    linearGradientBrush.GradientStops.ToList().ForEach(gradient =>
                    {
                        gradient.Color = WPFUtilities.DeployHelper.GetColorInContrast(gradient.Color, (document as ScreenDocument).Theme);
                    });
                }
                return linearGradientBrush;
            }

            return background;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertDefaultToolbarForeground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            SchedulerControl control = sender as SchedulerControl;
            Brush defColor = control.Foreground;
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;

            return foreground;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
