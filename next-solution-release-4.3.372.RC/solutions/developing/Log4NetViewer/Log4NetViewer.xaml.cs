using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DocumentManager.ComponentService;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using ScreenSettings;
using StringManager.ComponentService;
using Utilities;
using System.Runtime.Serialization;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using GridLayout;
using VFS;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using UIMsgBoxAlertService.ComponentService;
using TranslationHelpers;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using ViewModelLib;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Threading;
using DevExpress.Xpf.Bars;

namespace Log4NetViewer
{
    /// <summary>
    /// Interaction logic for Log4NetViewer.xaml
    /// </summary>
    public partial class Log4NetViewer : UserControl, IDisposable, IContainPropertyEditors, ISettingsHelper
    {
        #region DP
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        BarEditItem FilterCombo
        {
            get
            {
                return RunningOnServer ? cmbLoggerFilter_web : cmbLoggerFilter;
            }
        }

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Log4NetViewer));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Log4NetViewer));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Log4NetViewer));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Log4NetViewer));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as Log4NetViewer;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit || bInit)
                UpdateControlLayout();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as Log4NetViewer;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !IsManipulationEnabled)
                UpdateControlLayout();
        }
        private void UpdateControlLayout()
        {
            if (dpUpdateLayout == null ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Completed ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Aborted)
            {
                dpUpdateLayout = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                {
                    if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (from c in (this as UIElement).GetVisualChildrenOfType<TextBox>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = Foreground;
                         });
                        (from c in (this as UIElement).GetVisualChildrenOfType<TextBlock>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = Foreground;
                         });
                        (from c in (this as UIElement).GetVisualChildrenOfType<Label>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = Foreground;
                         });
                        (from c in (this as UIElement).GetVisualChildrenOfType<Button>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = Foreground;
                         });
                        (from c in (this as UIElement).GetVisualChildrenOfType<ComboBox>()
                         select c).ToList().ForEach(child =>
                         {
                             child.Foreground = Foreground;
                         });
                    }

                    gridControl.Background = Background;
                    if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (from c in tableView.GetVisualChildrenOfType<Grid>()
                         where c.Name == "rowPresenterGrid"
                         select c).ToList().ForEach(o =>
                         {
                             (from d in o.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                              select d).ToList().ForEach(x =>
                              {
                                  x.Background = Background;
                              });
                         });
                    }
                });
            }
        }

        private void UpdateForeground()
        {
            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                gridControl.Columns.ToList().ForEach(x =>
                {
                    x.HeaderStyle = gridControl.TryFindResource("columnStyle") as Style;
                });
                DataTemplate dataTemplate = TryFindResource("cellTemplate") as DataTemplate;
                gridControl.Columns.ToList().ForEach(c =>c.CellTemplate = dataTemplate);
            }
            else
            {
                gridControl.Columns.ToList().ForEach(x =>
                {
                    x.HeaderStyle = null;
                });
                DataTemplate dataTemplate = TryFindResource("cellDefaultTemplate") as DataTemplate;
                gridControl.Columns.ToList().ForEach(c => c.CellTemplate = dataTemplate);
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
        double ClientTimezoneOffset
        {
            get
            {
                try
                {
                    if (!RunningOnServer)
                        return 0.0;
                    return (double)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
                }
                catch
                {
                    return 0.0;
                }
            }
        }

        public static readonly DependencyProperty LogFileNameProperty = DependencyProperty.Register("LogFileName", typeof(String), typeof(Log4NetViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnLogFileNameChanged), new CoerceValueCallback(OnCoerceLogFileName)));

        private static object OnCoerceLogFileName(DependencyObject o, object value)
        {
            Log4NetViewer logViewer = o as Log4NetViewer;
            if (logViewer != null)
                return logViewer.OnCoerceLogFileName((String)value);
            else
                return value;
        }

        private static void OnLogFileNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer logViewer = o as Log4NetViewer;
            if (logViewer != null)
                logViewer.OnLogFileNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceLogFileName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLogFileNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit || bcInit)
                // UpdateLogEvents(newValue);
                Refresh();
        }

        [Browsable(false)]
        [XmlIgnore]
        public String LogFileName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(LogFileNameProperty);
            }
            set
            {
                SetValue(LogFileNameProperty, value);
            }
        }

        #region IsExternalWpfApp
        public static readonly DependencyProperty IsExternalWpfAppProperty = DependencyProperty.Register("IsExternalWpfApp", typeof(bool), typeof(Log4NetViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsExternalWpfAppChanged), new CoerceValueCallback(OnCoerceIsExternalWpfApp)));

        private static object OnCoerceIsExternalWpfApp(DependencyObject o, object value)
        {
            Log4NetViewer LogViewer = o as Log4NetViewer;
            if (LogViewer != null)
                return LogViewer.OnCoerceIsExternalWpfApp((bool)value);
            else
                return value;
        }

        private static void OnIsExternalWpfAppChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer LogViewer = o as Log4NetViewer;
            if (LogViewer != null)
                LogViewer.OnIsExternalWpfAppChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsExternalWpfApp(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsExternalWpfAppChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Browsable(false)]
        public bool IsExternalWpfApp
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsExternalWpfAppProperty);
            }
            set
            {
                SetValue(IsExternalWpfAppProperty, value);
            }
        }
        #endregion

        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(Log4NetViewer), new UIPropertyMetadata(Brushes.LightGray));

        [Browsable(false)]
        [XmlIgnore]
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

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(Log4NetViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            Log4NetViewer logViewer = o as Log4NetViewer;
            if (logViewer != null)
                return logViewer.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer logViewer = o as Log4NetViewer;
            if (logViewer != null)
                logViewer.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceGridLayout(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGridLayoutChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                LoadDesignGridLayout();
            });
        }

        internal void ResetGridLayout()
        {
            if (!string.IsNullOrEmpty(resetGridLayout))
                GridLayout = resetGridLayout;
        }

        void SaveResetGridLayout()
        {
            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    gridControl.SaveLayoutToStream(output);
                    resetGridLayout = utf8noBOM.GetString(output.ToArray());
                }
            }
            resetGridLayout = GridLayout;
        }

        internal void SaveDesignGridLayout()
        {
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                gridControl.SaveLayoutToStream(output);
                GridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void LoadDesignGridLayout()
        {
            if (string.IsNullOrEmpty(GridLayout))
                return;

            if (string.IsNullOrEmpty(resetGridLayout))
                SaveResetGridLayout();

            var dim = GridLayout.Length;
            string _mid = string.Empty;

            if (GridLayout.IndexOf('?') == 0)
            {
                _mid = GridLayout.Substring(1, dim - 1);
                SetValue(GridLayoutProperty, _mid);
                return;
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                if (!string.IsNullOrEmpty(GridLayout))
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(GridLayout)))
                    {
                        try
                        {
                            gridControl.RestoreLayoutFromStream(output);
                        }
                        catch
                        {
                            GridLayout = string.Empty;
                        }
                    }
                }
                else
                {
                    GridLayout = string.Empty;
                }
            }
            else
            {
                GridLayout = string.Empty;
            }
        }

        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertLogViewerGridLayout))]
        public String GridLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(GridLayoutProperty);
            }
            set
            {
                SetValue(GridLayoutProperty, value);
            }
        }

        private string localGridLayout;

        private void SaveLocalGridLayout()
        {
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                gridControl.SaveLayoutToStream(output);
                localGridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        private void LoadLocalGridLayout()
        {
            if (string.IsNullOrEmpty(localGridLayout))
                return;

            var dim = localGridLayout.Length;
            string _mid = string.Empty;

            if (!string.IsNullOrEmpty(localGridLayout))
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(localGridLayout)))
                {
                    try
                    {
                        gridControl.RestoreLayoutFromStream(output);
                    }
                    catch
                    {
                        localGridLayout = string.Empty;
                    }
                }
            }
            else
            {
                localGridLayout = string.Empty;
            }
        }

        #endregion

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(Log4NetViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("LogViewerOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ToolbarBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ToolbarBackgroundProperty);
            }
            set
            {
                SetValue(ToolbarBackgroundProperty, value);
            }
        }
        #endregion

        #region ToolbarForeground
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(Log4NetViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("LogViewerOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ToolbarForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ToolbarForegroundProperty);
            }
            set
            {
                SetValue(ToolbarForegroundProperty, value);
            }
        }
        #endregion


        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(Log4NetViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                control.OnEditableChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceEditable(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEditableChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public bool Editable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(EditableProperty);
            }
            set
            {
                SetValue(EditableProperty, value);
            }
        }

        #endregion


        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(Log4NetViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                control.OnEditingWriteAccessLevelChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceEditingWriteAccessLevel(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEditingWriteAccessLevelChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public int EditingWriteAccessLevel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(EditingWriteAccessLevelProperty);
            }
            set
            {
                SetValue(EditingWriteAccessLevelProperty, value);
            }
        }

        #endregion


        #region EditingWriteAccessMask
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(Log4NetViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer control = o as Log4NetViewer;
            if (control != null)
                control.OnEditingWriteAccessMaskChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceEditingWriteAccessMask(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEditingWriteAccessMaskChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public int EditingWriteAccessMask
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(EditingWriteAccessMaskProperty);
            }
            set
            {
                SetValue(EditingWriteAccessMaskProperty, value);
            }
        }

        #endregion

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(Log4NetViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            Log4NetViewer LogViewer = o as Log4NetViewer;
            if (LogViewer != null)
                return LogViewer.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Log4NetViewer LogViewer = o as Log4NetViewer;
            if (LogViewer != null)
                LogViewer.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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


        //[Browsable(false)]
        //public UserControl SmartControl
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return new Controls.SmartControl(this);
        //    }
        //}

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(Log4NetViewer), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditLayout
        {
            get
            {
                return (bool)GetValue(EditLayoutProperty);
            }
        }

        [MergablePropertyAttribute(false)]
        public bool SmartProperties
        {
            get { return false; }
        }
        #endregion

        #region Declarations
        readonly List<LogEntry> entries = new List<LogEntry>();
        List<LogEntry> filtered;
        List<String> listLoggers;
        readonly List<String> mergedFiles = new List<String>();
        IStringEditorManager stringManager;
        public IDocument Document;
        IDictionary<String, String> stringlist;
        public bool bSmartSettingsEditing;
        VirtualList vList;
        bool bDesignmode;
        bool bInit;
        bool bcInit;
        Setting defSetting;
        string resetGridLayout;
        Helper helper;
        string oldusername;
        DispatcherOperation dpUpdateLayout;
        #region ActualConfig
        string actualConfig = GridLayoutHelper.DesignSettingName;
        [Browsable(false)]
        internal string ActualConfig
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return actualConfig;
            }
            set
            {
                if (actualConfig.Equals(value))
                    return;
                actualConfig = value;
            }
        }
        #endregion
        MemorySettings MemorySettingList;
        #endregion

        #region ctor
        public Log4NetViewer()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            
            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbar.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
           Loaded += (o, e) =>
            {
                if (IsExternalWpfApp)
                {
                    LogFileName = null;
                    bInit = true;
                }
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    if (RunningOnServer)
                    {
                        toolbar.Bars.Clear();
                        toolbar.Bars.Add(toolBarControl);
                        cmbLoggerFilter_web.IsVisible = true;
                        cmbLoggerFilter.IsVisible = false;
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif

                    configMemory.EditValue = GridLayoutHelper.DesignSettingName;
                    configMemory.DataContext = MemorySettingList?.Names;
                    UpdateControlLayout();
                    OverrideBaseProperties();

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    bDesignmode = bDesignmode || DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;
                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document, true);

                    if (Document != null)
                    {
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(Document, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (bDesignmode)
                    {
                        toolbar.IsEnabled = false;
                        //gridControl.ItemsSource = null;
                        AssignDataSource();
                        UpdateForeground();
                        //DesignerProperties.SetIsInDesignMode(this, true);
                        if (!bSmartSettingsEditing)
                            view.IsHitTestVisible = false;
                    }
                    else
                    {
                        if (Document != null)
                        {
                            helper = new Helper(Document, this as ISettingsHelper);
                            helper.RefreshCurrentUser();
                            oldusername = helper.Username;
                        }

                        if (RunningOnServer)
                        {
                            SetWebAsset();
                            toolbarSettings.IsVisible = false;
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }

                        UpdateLogEvents(LogFileName);
                        //bInit = true;

                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();

                        if (helper != null)
                        {
                            string actualgridlayout = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username)?.FirstOrDefault(s => s.Name == GridLayoutHelper.DesignSettingName)?.GridLayout;

                            if (actualgridlayout != null)
                                GridLayout = actualgridlayout;
                        }
                        defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
                        configMemory.DataContext = MemorySettingList?.Names;
                        configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            try
                            {
                                LoadRuntimeLayout(GetStorageName());
                                GetItem(ActualConfig);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    bcInit = true;
                    if (String.IsNullOrEmpty(LogFileName))
                        LogFileName = GetLogFileName();

                    if (!bDesignmode && !bControlLoaded)
                    {
                        bControlLoaded = true;
                        OnControlLoaded();
                    }
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };

        }

        #endregion

        #region Methods
        internal string stringPlaceolder = "LogViewer";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (bUntranslated)
                    stringlist = null;
                else
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));

                configMemory.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                fileLog.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LogFile", stringlist, Properties.Resources.Path);
                refreshButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshData", stringlist, Properties.Resources.RefreshData);
                forceNewLog.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ForceNewLog", stringlist, Properties.Resources.ForceNewLog);
                FilterCombo.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Loggers", stringlist, Properties.Resources.Loggers);
                selectLast.Content = selectLast.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SelectLast", stringlist, Properties.Resources.SelectLast);
                selectFirst.Content = selectFirst.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SelectFirst", stringlist, Properties.Resources.SelectFirst);
                selectAll.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SelectAll", stringlist, Properties.Resources.SelectAll);
                configMemory.EditValue = ActualConfig;

                if (bcInit && !bDesignmode)
                    Refresh();
            });
        }
        private void SetWebAsset()
        {
            tableView.ShowFilterPanelMode = DevExpress.Xpf.Grid.ShowFilterPanelMode.Never;
            fileLog.IsEnabled = true;
        }

        static String GetLogFileName()
        {
            var rootAppender = ((Hierarchy)LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            var filename = rootAppender != null ? rootAppender.File : string.Empty;
            return filename;
        }

        #region Isolated Storage

        internal String GetStorageName(string defaultSettings = "")
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (Document != null)
            {
                var parent = Document.Parent;
                var parentName = String.Empty;
                if (parent != null)
                    parentName = parent.Title;
                return String.Format("{0}_{1}_{2}{3}", parentName, Document.Title, Name, defaultSettings);
            }
            return Name;
        }

        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
        }

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
        #endregion

        public void Refresh()
        {
            if (String.IsNullOrEmpty(LogFileName))
                return;
            SaveLocalGridLayout();

            UpdateLogEvents(LogFileName);
            tableView.FocusedRowHandle = gridControl.View.TopRowIndex;
            if (gridControl.View.TopRowIndex > 4)
            {
                tableView.FocusedRowHandle -= 3;
            }
            gridControl.View.MoveLastRow();
            LoadLocalGridLayout();
        }

        bool isLoadingValues;
        void UpdateLogEvents(String logFileName, bool withMerge = false, bool bDefEncoding = false)
        {
            if (String.IsNullOrEmpty(logFileName) || isLoadingValues)
                return;
            SaveLocalGridLayout();

            Task task2 = null;
            try
            {
                isLoadingValues = true;
                if (!withMerge)
                {
                    entries.Clear();
                    gridControl.ItemsSource = null;
                }
                else
                {
                    if (mergedFiles.Count == 0) mergedFiles.Add(logFileName);
                    if (mergedFiles.Contains(logFileName)) return;
                    mergedFiles.Add(logFileName);
                }

                AssignDataSource();
                UpdateForeground();
                //SetBusy(true);
                var runningOnServer = RunningOnServer;
                var clientTimezoneOffset = ClientTimezoneOffset;

                using (new WaitCursor())
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var list = new List<LogEntry>();
                        try
                        {
                            var sBuffer = "<root></root>";
                            if (!File.Exists(logFileName))
                            {
                                File.Create(logFileName);
                            }
                            else
                            {
                                try
                                {
                                    using (var fileStream = new FileStream(logFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    using (var textReader = new StreamReader(fileStream, bDefEncoding ? Encoding.UTF8 : Encoding.Unicode))
                                    {
                                        sBuffer = string.Format("<root>{0}</root>", textReader.ReadToEnd());
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                            var stringReader = new StringReader(sBuffer);
                            var xmlTextReader = new XmlTextReader(stringReader) { Namespaces = false };
                            while (xmlTextReader.Read())
                            {
                                var logentry = LogEntry.Parse(xmlTextReader);
                                if (logentry == null)
                                    continue;
                                entries.Add(logentry);
                                logentry.Item = entries.Count;
                                logentry.TimeStamp = runningOnServer ? logentry.TimeStamp.AddMinutes(clientTimezoneOffset) : logentry.TimeStamp.ToLocalTime();
                                logentry.Logger = TranslationHelpers.TranslationHelper.TranslateComposedText(logentry.Logger, stringlist, logentry.Logger); 
                                vList.RecordCount = entries.Count;
                            }
                        }
                        catch (Exception ex)
                        {
                            return null;
                        }
                        return entries;
                    });
                    task2 = task1.ContinueWith(ret =>
                    {
                        try
                        {
                            gridControl.BeginDataUpdate();
                            var filter = gridControl.FilterCriteria;
                            gridControl.FilterCriteria = null;
                            gridControl.FilterCriteria = filter;
                            gridControl.EndDataUpdate();

                            if (ret != null && ret.Result != null)
                            {
                                if (IsExternalWpfApp && entries.Count == 0 && bDefEncoding)
                                    MessageBox.Show(Properties.Resources.LogFileNoContent, logFileName,
                                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

                                if (TranslationHelpers.TranslationHelper.CanTranslate(stringlist))
                                    listLoggers = (from entry in entries.AsParallel()
                                                    where !String.IsNullOrEmpty(entry.Logger)
                                                    orderby entry.Logger ascending
                                                    select TranslationHelpers.TranslationHelper.TranslateComposedText(entry.Logger, stringlist, entry.Logger)).Distinct().ToList();
                                else
                                    listLoggers = (from entry in entries.AsParallel()
                                                    where !String.IsNullOrEmpty(entry.Logger)
                                                    orderby entry.Logger ascending
                                                    select entry.Logger).Distinct().ToList();

                                listLoggers.Add(Properties.Resources.AllLoggers);
                                FilterCombo.DataContext = listLoggers;
                                FilterCombo.EditValue = Properties.Resources.AllLoggers;
                                //gridControl.ItemsSource = list;
                                //gridControl.View.MoveLastRow();
                                var logFile = string.Empty;
                                var defFile = GetLogFileName();
                                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                                    logFile = defFile;
                                else
                                    logFile = logFileName;

                                var extension = System.IO.Path.GetExtension(defFile);
                                var directory = System.IO.Path.GetDirectoryName(logFile);

                                fileLog.DataContext = (from c in (Directory.GetFiles(directory, $"*{extension}*")).ToList() select System.IO.Path.GetFileName(c)).ToList();
                                fileLog.EditValue = System.IO.Path.GetFileName(logFileName);
                            }
                            else
                            {
                                SetBusy(false);
                                if (!RunningOnServer)
                                    MessageBox.Show(IsExternalWpfApp ? Properties.Resources.LogFileCorruptedDesktop : Properties.Resources.LogFileCorrupted, logFileName,
                                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                                if (!IsExternalWpfApp)
                                    File.Move(logFileName, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(logFileName), string.Format("{0}{2:yyyyMMddHHmmss}{1}", Properties.Settings.Default.BackupFilePrefix, System.IO.Path.GetFileName(logFileName), DateTime.Now)));
                            }
                        }
                        catch
                        {

                        }
                        finally
                        {
                            
                            bInit = true;
                            isLoadingValues = false;
                            gridControl.View.MoveLastRow();
                            SetBusy(false);
                            if (entries.Count == 0 && !bDefEncoding)
                                UpdateLogEvents(logFileName, withMerge, true); //Keeping the old behaviour with non-unicode files (opening as UTF-8)
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                
            }
            catch (Exception)
            {
                SetBusy(false);
            }
            finally
            {
                LoadLocalGridLayout();

                if (task2 == null)
                    isLoadingValues = false;
            }
        }

        public event EventHandler Init;
        private void OnInit(EventArgs e)
        {
            EventHandler temp = Init;
            if (temp != null)
                temp(null, e);
        }

        internal void InitColumns()
        {
            VirtualList vList = new VirtualList() { bList = entries };
            vList.RecordCount = entries.Count;
            vList.ColumnCount = 12;
            gridControl.ItemsSource = null;
            gridControl.Columns.Clear();
            gridControl.Columns.BeginUpdate();
            PropertyDescriptorCollection properties = ((ITypedList)vList).GetItemProperties(null);
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                GridColumn column = new GridColumn();
                column.Name = propertyDescriptor.Name;
                column.FieldName = propertyDescriptor.Name;
                if (propertyDescriptor.PropertyType == typeof(DateTime))
                    column.EditSettings = (TextEditSettings)Resources["dateSettings"];
                column.BestFitMaxRowCount = 15;
                column.AllowEditing = DefaultBoolean.False;
                column.ReadOnly = true;
                column.AllowSearchPanel = DefaultBoolean.True;
                column.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_{propertyDescriptor.Name}", stringlist, propertyDescriptor.Name);
                column.AllowColumnFiltering = DefaultBoolean.True;
                gridControl.Columns.Add(column);
            }

            gridControl.Columns.EndUpdate();
            gridControl.ItemsSource = vList;
        }

        private void AssignDataSource()
        {
            vList = new VirtualList() {bList = entries};
 
            vList.RecordCount = entries.Count;
            vList.ColumnCount = 12;
            gridControl.ItemsSource = null;
            gridControl.Columns.Clear();
            gridControl.Columns.BeginUpdate();
            PropertyDescriptorCollection properties = ((ITypedList)vList).GetItemProperties(null);
            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                GridColumn column = new GridColumn();
                column.Name = propertyDescriptor.Name;
                column.FieldName = propertyDescriptor.Name;
                if(propertyDescriptor.PropertyType == typeof(DateTime)) 
                    column.EditSettings = (TextEditSettings)Resources["dateSettings"];
                column.BestFitMaxRowCount = 15;
                column.AllowEditing = DefaultBoolean.False;
                column.ReadOnly = true;
                column.AllowSearchPanel = DefaultBoolean.True;
                column.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_{propertyDescriptor.Name}", stringlist, propertyDescriptor.Name);
                column.AllowColumnFiltering = DefaultBoolean.True;
                column.CellTemplate = TryFindResource("cellDefaultTemplate") as DataTemplate;
                gridControl.Columns.Add(column);
            }

            gridControl.Columns.EndUpdate();
            gridControl.ItemsSource = vList;
            LoadDesignGridLayout();
            EventArgs m = new EventArgs();
            OnInit(m);
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.AutoWidth = false;
            tableView.BestFitColumns();
        }

        public event EventHandler FileOpened;
        private void Click_OpenLogFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = Properties.Settings.Default.LogFileDefaultExt;
            dlg.Filter = Properties.Settings.Default.LogFileFilter;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                LogFileName = dlg.FileName;
                FileOpened?.Invoke(this, null);
                tableView.Focus();
            }
        }

        private void Click_forceNewLog(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(LogFileName) || !System.IO.File.Exists(LogFileName) || new FileInfo(LogFileName).Length == 0)
                    return;

                var folder = System.IO.Path.GetDirectoryName(LogFileName);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(LogFileName);
                var extension = System.IO.Path.GetExtension(LogFileName);
                System.IO.File.Move(LogFileName, System.IO.Path.Combine(folder, string.Format("{0}{2}{1:yyyyMMddHHmmss}", fileName, DateTime.Now, extension)));
                File.WriteAllText(LogFileName, String.Empty);
            }
            catch (Exception ex)
            {
                if (!RunningOnServer && Document != null)
                {
                    var msgBoxService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (msgBoxService != null)
                        msgBoxService.ShowError(ex.Message);
                    else
                        MessageBox.Show(ex.Message, Document.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            Refresh();
        }
        
        private void Control_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Refresh();
                e.Handled = true;
            }
        }

        private void fileLog_SelectionChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string defaultLog;
            SaveLocalGridLayout();

            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                defaultLog = GetLogFileName();
            else
                defaultLog = LogFileName;
            if (string.IsNullOrEmpty(defaultLog) || string.IsNullOrEmpty((string)fileLog.EditValue))
                return;
            LogFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(defaultLog), (string)fileLog.EditValue);
            LoadLocalGridLayout();

            //Refresh();
        }

        private void cmbLoggerFilter_SelectionChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            gridControl.ItemsSource = null;

            var filter = FilterCombo.EditValue as String;
            if (filter == null || filter == Properties.Resources.AllLoggers || FilterCombo.EditValue as string == (FilterCombo.DataContext as List<string>).Last())
            {
                var list = (from c in entries.AsParallel() orderby c.TimeStamp select c).ToList();
                gridControl.ItemsSource = list;
            }
            else
            {
                //if (stringlist != null && stringlist.ContainsValue(filter))
                //    filter = (from o in stringlist where o.Value == filter select o.Key).FirstOrDefault();

                var list = (from c in entries.AsParallel() where c.Logger == filter
                            orderby c.TimeStamp select c).ToList();
                gridControl.ItemsSource = list;
                gridControl.View.MoveLastRow();
            }
            
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy)
            {
                busyContent.Text = Properties.Resources.WaitText;
                busyControl.Visibility = Visibility.Visible;
                busyContent.Visibility = Visibility.Visible;
            }
            else
            {
                busyControl.Visibility = Visibility.Collapsed;
                busyContent.Visibility = Visibility.Collapsed;
            }
        }

        private void GridControl_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter -= GridControl_MouseEnter;
            MouseDown += GridControl_MouseDown;
        }

        void GridControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = e.LeftButton == MouseButtonState.Pressed;
        }

        public bool bLoaded { get; set; }

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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            if (oldMemoryList != null)
                oldMemoryList.Clear();
            oldMemoryList = null;
            if (!bDesignmode)
            {
                if (bControlLoaded && GridLayoutHelper.DesignSettingName == ActualConfig && !Editable && UserBasedRuntimeSettings)
                    SaveGridConfiguration();
            }
            if (MemorySettingList != null)
                MemorySettingList.Clear();
            MemorySettingList = null;

            if (helper is IDisposable)
                (helper as IDisposable).Dispose();
            helper = null;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            DetachOverrideBaseProperties();

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            if (dpUpdateLayout != null &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Completed)
                dpUpdateLayout.Abort();

            entries.Clear();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Click_SelectLast(object sender, RoutedEventArgs e)
        {
            if (gridControl.View.DataControl == null || gridControl.View.DataControl.VisibleRowCount <= 0)
                return;
            gridControl.View.Focus();
            gridControl.View.MoveLastRow();
            tableView.DataControl.UnselectAll();
            tableView.SelectCells(gridControl.View.DataControl.VisibleRowCount - 1, gridControl.Columns.First(), gridControl.View.DataControl.VisibleRowCount - 1, gridControl.Columns.Last());
        }

        private void Click_SelectFirst(object sender, RoutedEventArgs e)
        {
            if (gridControl.View.DataControl == null || gridControl.View.DataControl.VisibleRowCount <= 0)
                return;
            gridControl.View.Focus();
            gridControl.View.MoveFirstRow();
            tableView.DataControl.UnselectAll();
            tableView.SelectCells(0, gridControl.Columns.First(), 0, gridControl.Columns.Last());
        }

        private void Click_SelectAll(object sender, RoutedEventArgs e)
        {
            if (gridControl.View.DataControl == null || gridControl.View.DataControl.VisibleRowCount <= 0)
                return; 
            gridControl.View.Focus();
            gridControl.View.MoveLastRow();
            tableView.DataControl.SelectAll();
        }

        private void Click_SelectAllLoggers(object sender, RoutedEventArgs e)
        {
            FilterCombo.EditValue = Properties.Resources.AllLoggers;
        }
        #endregion

        #region EditSettings
        bool bUserInteractionSettings;
        private void GetItem(string itemName)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (!string.IsNullOrEmpty(setting?.GridLayout))
                {
                    GridLayout = setting.GridLayout;
                    ActualConfig = setting.Name;
                }
                else
                {
                    GridLayout = defSetting.GridLayout;
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                }
                bIsInEditMode = true;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = ActualConfig;
                bIsInEditMode = false;
            }
            catch (Exception)
            {
            }
            finally
            {
                bUserInteractionSettings = false;
            }
        }

        /// <summary>
        /// Use this method to get the control runtime MemorySettings list
        /// </summary>
        /// <returns></returns>
        public List<String> GetMemoryMap()
        {
            MemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            return (from n in list select n.Name).ToList();
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
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        Indent = true,
                        CloseOutput = true
                    };

                    using (XmlWriter writer = XmlDictionaryWriter.Create(stream, settings))
                    {
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(string));
                            serializer.WriteObject(writer, ActualConfig);
                            bRet = true;
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

        void SaveGridConfiguration()
        {
                var Selected = MemorySettingList?.FirstOrDefault(s => s.Name.Equals(ActualConfig));
                var Utf8NoBom = new UTF8Encoding(true);
                SaveDesignGridLayout();
                if (Selected != null)
                    Selected.GridLayout = GridLayout;
                else
                {
                    Selected = new Setting() { GridLayout = GridLayout, Name = GridLayoutHelper.DesignSettingName, ReadOnly = true };
                    MemorySettingList.Add(Selected);
                }
                string currentUserName = helper.Username;
                string userName = oldusername != currentUserName ? oldusername : currentUserName;
                StorageHelper.StorageHelper.SaveMemoryMap(MemorySettingList, Document, Name, userName);
        }
        bool LoadRuntimeLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return false;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlDictionaryReader.Create(stream, settings))
                    {
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer formatter = new DataContractSerializer(typeof(string));
                            ActualConfig = formatter.ReadObject(reader) as string;
                            bRet = true;
                        }
                        finally
                        {
                            reader.Close();
                        }
                        return bRet;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region EventHandlers
        public event EventHandler ControlLoaded;

        bool bControlLoaded;
        void OnControlLoaded()
        {
            ControlLoaded?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Setting Management

        /// <summary>
        /// Use this method to load runtime settings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool LoadSettings(string name)
        {
            try
            {
                if (!bControlLoaded)
                    return false;

                var item = (from m in MemorySettingList where m.Name.Equals(name) select m).FirstOrDefault();
                if (item != null)
                {
                    configMemory.EditValue = item.Name;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void configMemory_SelectionChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string editValue = ((DevExpress.Xpf.Bars.BarEditItem)e.OriginalSource).EditValue as String;
            if (MemorySettingList != null && MemorySettingList.Names.Contains(editValue))
            {
                
                ChangeSetting(editValue);
            }
        }

        void ChangeSetting(string settingName)
        {
            if (String.IsNullOrEmpty(settingName) || ActualConfig == settingName || bCallingRemoveCommand || bCallingSaveCommand || !bInit || bIsInEditMode)
                return;

            using (var cursor = new WaitCursor())
            {
                ActualConfig = settingName;
                SaveRuntimeLayout(GetStorageName());
                GetItem(ActualConfig);
            }
            return;
        }

        RelayCommand _resetCommand;
        [Browsable(false)]
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(
                        param => CallResetCommand(),
                        param => IsEnableResetCommand
                        );
                }
                return _resetCommand;
            }
        }

        MemorySettings oldMemoryList;
        string oldConfigName;
        bool bCallingResetCommand;
        void CallResetCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingResetCommand = true;
            SaveDesignGridLayout();
            ResetGridLayout();
            configMemory.EditValue = ActualConfig;
            bCallingResetCommand = false;
        }

        bool IsEnableResetCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand)
                    return false;
                else
                    return true;
            }
        }

        RelayCommand _saveCommand;
        [Browsable(false)]
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => CallSaveCommand(),
                        param => IsEnableCommand
                        );
                }
                return _saveCommand;
            }
        }

        bool bCallingSaveCommand;
        internal void CallSaveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingSaveCommand = true;
            string configname = configMemory.EditValue as String;

            SaveDesignGridLayout();

            var selected = (from m in MemorySettingList
                            where m.Name == configname
                            select m).FirstOrDefault();
            if (selected == null)
            {
                MemorySettingList.Add(new Setting()
                {
                    Name = configname,
                    GridLayout = GridLayout,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.GridLayout = GridLayout;
                selected.ReadOnly = false;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = selected.Name;
            }

            if (StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null))
            {
                if (oldMemoryList != null)
                    oldMemoryList.Clear();
                oldMemoryList = null;
                oldConfigName = null;
            }
            ActualConfig = configname;
            SaveGridConfiguration();
            SaveRuntimeLayout(GetStorageName());
            bCallingSaveCommand = false;
        }
        RelayCommand _removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => CallRemoveCommand(),
                        param => IsEnabledRemoveCommand
                        );
                }
                return _removeCommand;
            }
        }

        bool bCallingRemoveCommand;
        internal void CallRemoveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingRemoveCommand = true;

            var selected = (from m in MemorySettingList
                            where m.Name == configMemory.EditValue as string
                            select m).FirstOrDefault();

            if (oldMemoryList == null)
            {
                oldMemoryList = new MemorySettings(MemorySettingList);
                oldConfigName = configMemory.EditValue as string;
            }
            if (selected != null)
                MemorySettingList.Remove(selected);

            StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            configMemory.DataContext = MemorySettingList.Names;
            bCallingRemoveCommand = false;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }
        internal bool IsEnableCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (string.IsNullOrEmpty(configMemory.EditValue as String) || bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand)
                    return false;
                else
                    return true;
            }
        }
        internal bool IsEnabledRemoveCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (string.IsNullOrEmpty(configMemory.EditValue as string) || bCallingResetCommand || bCallingSaveCommand || bCallingRemoveCommand || (configMemory.EditValue as string) == GridLayoutHelper.DesignSettingName)
                    return false;
                else
                    return true;
            }
        }
        bool bIsInEditMode;
        private void configMemory_LostFocus(object sender, RoutedEventArgs e)
        {
            bIsInEditMode = false;
        }

        private void configMemory_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (bIsInEditMode)
                return;

            if (helper != null && helper.ValidateAccessLevel())
                return;
            
            bIsInEditMode = true;
        }

        #region ISettingsHelper
        public void UpdateWriteAccessCommands()
        {
            
        }

        public void ReloadRuntimeSettings()
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (!UserBasedRuntimeSettings)
                    return;

                if (oldusername != null && helper?.Username != oldusername && !Editable && UserBasedRuntimeSettings)
                    SaveGridConfiguration();

                if (!String.IsNullOrEmpty(helper.Username))
                    MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username);
                else
                    EnsureDefaultValues(false);
                if (!LoadRuntimeLayout(GetStorageName()))
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                GetItem(ActualConfig);
                oldusername = helper.Username;
            });
        }

        void EnsureDefaultValues(bool bSetCombo = true)
        {            
            defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
            if (bSetCombo)
            {
                configMemory.DataContext = MemorySettingList.Names;
                if (defSetting != null)
                {
                    bIsInEditMode = true;
                    configMemory.EditValue = defSetting.Name;
                    bIsInEditMode = false;
                }
            }
        }
        #endregion

        public void Initialize()
        {
        }
        #endregion

        internal List<string> defaultSvgColumns = new List<string>() { "Level", "Thread", "Message",
            "MachineName", "UserName", "Identity",
            "NDC", "HostName", "App",
            "Throwable", "Class", "Method"};

        private void Click_CopyRow(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            tableView.CopySelectedCellsToClipboard();
        }


    }

    public class CountInfo
    {
        public int Value { get; set; }
        public string Description { get; set; }
    }
    public class VirtualPropertyDescriptor : PropertyDescriptor
    {
        string propertyName;
        Type propertyType;
        bool isReadOnly;
        VirtualList list;
        int index;
        public VirtualPropertyDescriptor(VirtualList list, int index, string propertyName, Type propertyType, bool isReadOnly)
            : base(propertyName, null)
        {
            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.isReadOnly = isReadOnly;
            this.list = list;
            this.index = index;
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override object GetValue(object component)
        {
            return list.GetPropertyValue((int)component, index);
        }
        public override void SetValue(object component, object val)
        {
            list.SetPropertyValue((int)component, index, val);
        }
        public override bool IsReadOnly { get { return isReadOnly; } }
        public override string Name { get { return propertyName; } }
        public override Type ComponentType { get { return typeof(VirtualList); } }
        public override Type PropertyType { get { return propertyType; } }
        public override void ResetValue(object component)
        {
        }
        public override bool ShouldSerializeValue(object component) { return true; }
    }
    public struct Location
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public override bool Equals(object obj)
        {
            Location l = (Location)obj;
            return l.Row == Row && l.Column == Column;
        }
        public override int GetHashCode()
        {
            return Row ^ Column;
        }
    }
    public class VirtualList : IList, ITypedList
    {
        const int BaseColumnCount = 12;
        int recordCount;
        int columnCount;
        public List<LogEntry> bList = new List<LogEntry>();
        Dictionary<Location, object> fValues = new Dictionary<Location, object>();
        PropertyDescriptorCollection columnCollection;
        public VirtualList()
        {
            recordCount = 0;
            columnCount = 12;
            CreateColumnCollection();
        }
        public void SetPropertyValue(int rowIndex, int columnIndex, object value)
        {
            fValues[new Location() { Column = columnIndex, Row = rowIndex }] = value;
        }
        public object GetPropertyValue(int rowIndex, int columnIndex)
        {
            object value = null;
            if (fValues.TryGetValue(new Location() { Column = columnIndex, Row = rowIndex }, out value))
            {
                return value;
            }
            //if (columnIndex == 0)
            //    return rowIndex + 1;
            switch ((columnIndex) % BaseColumnCount)
            {
                case 0:
                    return bList[rowIndex].Item;
                case 1:
                    return bList[rowIndex].Logger;
                case 2:
                    return bList[rowIndex].TimeStamp;
                case 3:
                    return bList[rowIndex].Level;
                case 4: 
                    return bList[rowIndex].NDC;
                case 5: 
                    return bList[rowIndex].Identity;
                case 6: 
                    return bList[rowIndex].Message;
                case 7: 
                    return bList[rowIndex].Details;
                case 8: 
                    return bList[rowIndex].MachineName;
                case 9: 
                    return bList[rowIndex].HostName;
                case 10:
                    return bList[rowIndex].UserName;
                case 11: 
                    return bList[rowIndex].App;
            }
            throw new NotImplementedException();
        }
        string[] propertyName = new string[12] { "Item", "Logger", "TimeStamp", "Level", "NDC", "Identity", "Message", "Details", "MachineName", "HostName", "UserName", "App" };
        public string GetPropertyName(int columnIndex)
        {
            //if (columnIndex == 0)
            //    return "ID";
            try 
	        {	        
                return propertyName[(columnIndex) % BaseColumnCount];
	        }
	        catch (Exception)
	        {
                return null;
	        }
        }
        Type[] propertyType = new Type[12] { typeof(int), typeof(string), typeof(DateTime), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        public Type GetPropertyType(int columnIndex)
        {
            //if (columnIndex == 0)
            //    return typeof(int);
            try
            {
                return propertyType[(columnIndex) % BaseColumnCount];
            }
            catch (Exception)
            {
                return null;
            }
        }
        int GetPseudoRandomValue(int rowIndex, int columnIndex, int maxValue)
        {
            return (rowIndex + columnIndex) % maxValue;
        }
        public int RecordCount
        {
            get { return recordCount; }
            set
            {
                if (value < 1) value = 0;
                if (RecordCount == value) return;
                recordCount = value;
            }
        }
        public int ColumnCount
        {
            get { return columnCount; }
            set
            {
                if (value < 1) value = 0;
                if (ColumnCount == value) return;
                columnCount = value;
                CreateColumnCollection();
            }
        }
        protected virtual void CreateColumnCollection()
        {
            VirtualPropertyDescriptor[] pds = new VirtualPropertyDescriptor[ColumnCount];
            for (int n = 0; n < ColumnCount; n++)
            {
                pds[n] = new VirtualPropertyDescriptor(this, n, GetPropertyName(n), GetPropertyType(n), false);
            }
            columnCollection = new PropertyDescriptorCollection(pds);
        }

        #region ITypedList Interface
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] descs) { return columnCollection; }
        string ITypedList.GetListName(PropertyDescriptor[] descs) { return ""; }
        #endregion
        #region IList Interface
        public virtual int Count
        {
            get { return RecordCount; }
        }
        public virtual bool IsSynchronized
        {
            get { return true; }
        }
        public virtual object SyncRoot
        {
            get { return true; }
        }
        public virtual bool IsReadOnly
        {
            get { return false; }
        }
        public virtual bool IsFixedSize
        {
            get { return true; }
        }
        public virtual IEnumerator GetEnumerator()
        {
            return null;
        }
        public virtual void CopyTo(System.Array array, int fIndex)
        {
        }
        public virtual int Add(object val)
        {
            throw new NotImplementedException();
        }
        public virtual void Clear()
        {
            throw new NotImplementedException();
        }
        public virtual bool Contains(object val)
        {
            throw new NotImplementedException();
        }
        public virtual int IndexOf(object val)
        {
            throw new NotImplementedException();
        }
        public virtual void Insert(int fIndex, object val)
        {
            throw new NotImplementedException();
        }
        public virtual void Remove(object val)
        {
            throw new NotImplementedException();
        }
        public virtual void RemoveAt(int fIndex)
        {
            throw new NotImplementedException();
        }
        object IList.this[int fIndex]
        {
            get { return fIndex; }
            set { }
        }
        #endregion
    }
    public class ConvertLogViewerGridLayout : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender = null)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            Log4NetViewer logViewer = sender as Log4NetViewer;
            var gridLayout = value as string;
            List<object> res = new List<object>();

            logViewer.InitColumns();
            if (!string.IsNullOrEmpty(gridLayout))
                logViewer.LoadDesignGridLayout();
            var columnlist = (from column in logViewer.gridControl.Columns where column.Visible == true orderby column.VisibleIndex select column);
            columnlist.ToList().ForEach(c =>
            {
                res.Add(new Dictionary<string, object>() {
                    {"FieldName", c.FieldName },
                    {"ActualWidth", c.ActualWidth },
                    {"ColumnTag", c.Tag?.ToString() },
                    {"Visible", c.Visible },
                    {"VisibleIndex", c.VisibleIndex },
                    {"ColumnOrder", c.SortOrder },
                    {"SortIndex", c.SortIndex }
                });
            });

            return res;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            return new Dictionary<string, Brush>();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is Log4NetViewer)
            {
                Log4NetViewer control = sender as Log4NetViewer;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(Log4NetViewer.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);
                if (control.ReadLocalValue(Log4NetViewer.ControlForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlForeground", control.ControlForeground);
                else
                    ret.Add("ControlForeground", foreground);

                if (control.ReadLocalValue(Log4NetViewer.BackgroundProperty) != DependencyProperty.UnsetValue && control.Background != null)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;
            Log4NetViewer control = sender as Log4NetViewer;
            string prop = ((DependencyProperty)property)?.Name;
            if (prop.Equals(Log4NetViewer.ToolbarForegroundProperty.Name))
            {
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                return foreground;
            }
            else if (prop.Equals(Log4NetViewer.ToolbarBackgroundProperty.Name))
            {
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
}
