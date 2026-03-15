using DataLoggerModel.Helpers;
using DataValidation.Converters;
using DevExpress.Export;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.PreviewControl.Bars;
using DevExpress.XtraPrinting.Drawing;
using DocumentManager.ComponentService;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using GridLayout;
using Ookii.Dialogs.Wpf;
using StorageHelper;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using TranslationHelpers;
using UFInterfaces.PropertyControl;
using UFUAEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using WPFUtilities;
using WPFUtilities.PropertyDataTemplate;
using DevExpress.Xpf.Bars.Themes;
using ScreenSettings;
using OPCUAViewModel;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Core.FilteringUI;
using WPFUtilities.Extensions;
namespace DataValidation
{
    /// <summary>
    /// Interaction logic for DataValidation.xaml
    /// </summary>
    public partial class DataValidation : UserControl, IContainPropertyEditors, IDisposable, ISettingsHelper
    {
        #region Dependency Properties

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(string), typeof(DataValidation), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceGridLayout((string)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnGridLayoutChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceGridLayout(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGridLayoutChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            LoadDesignGridLayout();
        }

        internal void ResetGridLayout()
        {
            if (!string.IsNullOrEmpty(resetGridLayout))
                GridLayout = resetGridLayout;
        }

        void SaveResetGridLayout()
        {
            if (bDesign || DesignerProperties.GetIsInDesignMode(this))
            {
                using (MemoryStream output = new MemoryStream())
                {


                    gridControl.SaveLayoutToStream(output);
                    var utf8NoBom = new UTF8Encoding(true);
                    resetGridLayout = utf8NoBom.GetString(output.ToArray());

                }
            }
            else
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

        void LoadDesignGridLayout(bool bDenyLoad = false)
        {
            if (bDisposed || string.IsNullOrEmpty(GridLayout))
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

            if (bDenyLoad)
            {
                GridLayout = string.Empty;
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
        [SvgValueConverter(false)]
        public string GridLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(GridLayoutProperty);
            }
            set
            {
                SetValue(GridLayoutProperty, value);
            }
        }

        #endregion

        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(DataValidation), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
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

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(DataValidation), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
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


        #region ReadDataOnLoading
        public static readonly DependencyProperty ReadDataOnLoadingProperty = DependencyProperty.Register("ReadDataOnLoading", typeof(bool), typeof(DataValidation), new UIPropertyMetadata(false));
        public bool ReadDataOnLoading
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ReadDataOnLoadingProperty);
            }
            set
            {
                SetValue(ReadDataOnLoadingProperty, value);
            }
        }

        #endregion


        #region LoadingFilterType
        public static readonly DependencyProperty LoadingFilterTypeProperty = DependencyProperty.Register("LoadingFilterType", typeof(DateSpan), typeof(DataValidation), new UIPropertyMetadata(DateSpan.Day));

        public DateSpan LoadingFilterType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateSpan)GetValue(LoadingFilterTypeProperty);
            }
            set
            {
                SetValue(LoadingFilterTypeProperty, value);
            }
        }

        #endregion


        #region DateTimeTolerance
        public static readonly DependencyProperty DateTimeToleranceProperty = DependencyProperty.Register("DateTimeTolerance", typeof(TimeSpan), typeof(DataValidation), new UIPropertyMetadata(TimeSpan.MinValue));

        //private static object OnCoerceDateTimeTolerance(DependencyObject o, object value)
        //{
        //    DataValidation control = o as DataValidation;
        //    if (control != null)
        //        return control.OnCoerceDateTimeTolerance((TimeSpan)value);
        //    else
        //        return value;
        //}

        //private static void OnDateTimeToleranceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    DataValidation control = o as DataValidation;
        //    if (control != null)
        //        control.OnDateTimeToleranceChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        //}

        //protected virtual TimeSpan OnCoerceDateTimeTolerance(TimeSpan value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnDateTimeToleranceChanged(TimeSpan oldValue, TimeSpan newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    if (!DesignerProperties.GetIsInDesignMode(this) && oldValue != newValue)
        //    {
        //        if (view.DataContext is DataValidationViewModel)
        //        {
        //            var viewModel = view.DataContext as DataValidationViewModel;
        //            view.DataContext = new DataValidationViewModel(viewModel.ItemsSource, newValue);
        //        }
        //    }
        //}

        [Category("Execution")]
        [Browsable(false)]
        [XmlIgnore]
        [Obsolete("No longer used.")]
        public TimeSpan DateTimeTolerance
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(DateTimeToleranceProperty);
            }
            set
            {
                SetValue(DateTimeToleranceProperty, value);
            }
        }
        #endregion

        #region MaxErrorBeforeAbort
        public static readonly DependencyProperty MaxErrorBeforeAbortProperty = DependencyProperty.Register("MaxErrorBeforeAbort", typeof(int), typeof(DataValidation), new UIPropertyMetadata(Properties.Settings.Default.MaxUnauthorizedAccess, new PropertyChangedCallback(OnMaxErrorBeforeAbortChanged), new CoerceValueCallback(OnCoerceMaxErrorBeforeAbort)));

        private static object OnCoerceMaxErrorBeforeAbort(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceMaxErrorBeforeAbort((int)value);
            else
                return value;
        }

        private static void OnMaxErrorBeforeAbortChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnMaxErrorBeforeAbortChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxErrorBeforeAbort(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxErrorBeforeAbortChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && oldValue != newValue)
            {
                if (view.DataContext is DataValidationViewModel)
                {
                    var viewModel = view.DataContext as DataValidationViewModel;
                    viewModel.MaxUnauthorizedAccess = newValue;
                }
            }
        }

        [Category("Execution")]
        public int MaxErrorBeforeAbort
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxErrorBeforeAbortProperty);
            }
            set
            {
                SetValue(MaxErrorBeforeAbortProperty, value);
            }
        }
        #endregion

        #region ExecuteQueryTimeout
        public static readonly DependencyProperty ExecuteQueryTimeoutProperty = DependencyProperty.Register("ExecuteQueryTimeout", typeof(int), typeof(DataValidation), new UIPropertyMetadata(Properties.Settings.Default.DefaultQueryTimeout, new PropertyChangedCallback(OnExecuteQueryTimeoutChanged), new CoerceValueCallback(OnCoerceExecuteQueryTimeout)));

        private static object OnCoerceExecuteQueryTimeout(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceExecuteQueryTimeout((int)value);
            else
                return value;
        }

        private static void OnExecuteQueryTimeoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnExecuteQueryTimeoutChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceExecuteQueryTimeout(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnExecuteQueryTimeoutChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && oldValue != newValue)
            {
                if (view.DataContext is DataValidationViewModel)
                {
                    var viewModel = view.DataContext as DataValidationViewModel;
                    viewModel.QueryTimeout = newValue;
                }
            }
        }

        [Category("Execution")]
        public int ExecuteQueryTimeout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ExecuteQueryTimeoutProperty);
            }
            set
            {
                SetValue(ExecuteQueryTimeoutProperty, value);
            }
        }
        #endregion

        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(DataValidation), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));

        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnUseIconChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceUseIcon(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseIconChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        public Boolean UseIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(UseIconProperty);
            }
            set
            {
                SetValue(UseIconProperty, value);
            }
        }
        #endregion

        #region AllowSelectBackupFiles
        public static readonly DependencyProperty AllowSelectBackupFilesProperty = DependencyProperty.Register("AllowSelectBackupFiles", typeof(Boolean), typeof(DataValidation), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowSelectBackupFilesChanged), new CoerceValueCallback(OnCoerceAllowSelectBackupFiles)));

        private static object OnCoerceAllowSelectBackupFiles(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceAllowSelectBackupFiles((Boolean)value);
            else
                return value;
        }

        private static void OnAllowSelectBackupFilesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnAllowSelectBackupFilesChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAllowSelectBackupFiles(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowSelectBackupFilesChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Execution")]
        public Boolean AllowSelectBackupFiles
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AllowSelectBackupFilesProperty);
            }
            set
            {
                SetValue(AllowSelectBackupFilesProperty, value);
            }
        }
        #endregion

        #region RowsInErrorColor
        public static readonly DependencyProperty RowsInErrorColorProperty = DependencyProperty.Register("RowsInErrorColor", typeof(Color), typeof(DataValidation), new UIPropertyMetadata(Colors.Red, new PropertyChangedCallback(OnRowsInErrorColorChanged), new CoerceValueCallback(OnCoerceRowsInErrorColor)));

        private static object OnCoerceRowsInErrorColor(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceRowsInErrorColor((Color)value);
            else
                return value;
        }

        private static void OnRowsInErrorColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnRowsInErrorColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual Color OnCoerceRowsInErrorColor(Color value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowsInErrorColorChanged(Color oldValue, Color newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        public Color RowsInErrorColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(RowsInErrorColorProperty);
            }
            set
            {
                SetValue(RowsInErrorColorProperty, value);
            }
        }
        #endregion

        #region ValidationSource
        public static readonly DependencyProperty ValidationSourceProperty = DependencyProperty.Register("ValidationSource", typeof(string), typeof(DataValidation), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnValidationSourceChanged), new CoerceValueCallback(OnCoerceValidationSource)));

        private static object OnCoerceValidationSource(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceValidationSource((string)value);
            else
                return value;
        }

        private static void OnValidationSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnValidationSourceChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceValidationSource(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValidationSourceChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("General")]
        public string ValidationSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ValidationSourceProperty);
            }
            set
            {
                SetValue(ValidationSourceProperty, value);
            }
        }
        #endregion

        #region CustomValidationSID
        public static readonly DependencyProperty CustomValidationSIDProperty = DependencyProperty.Register("CustomValidationSID", typeof(string), typeof(DataValidation), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCustomValidationSIDChanged), new CoerceValueCallback(OnCoerceCustomValidationSID)));

        private static object OnCoerceCustomValidationSID(DependencyObject o, object value)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                return control.OnCoerceCustomValidationSID((string)value);
            else
                return value;
        }

        private static void OnCustomValidationSIDChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataValidation control = o as DataValidation;
            if (control != null)
                control.OnCustomValidationSIDChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceCustomValidationSID(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCustomValidationSIDChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (viewModel != null && oldValue != newValue)
                viewModel.ValidationSID = newValue;
        }

        /// <summary>
        /// SDDL string for the SID used to validate the data
        /// </summary>
        [Category("Execution")]
        public string CustomValidationSID
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(CustomValidationSIDProperty);
            }
            set
            {
                SetValue(CustomValidationSIDProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        //[Browsable(false)]
        //public UserControl SmartControl
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return new Controls.SmartControl(this);
        //    }
        //}

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(DataValidation), new UIPropertyMetadata(false));

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
        #endregion

        #region Declarations
        DispatcherOperation dpUpdateLayout;
        internal bool bSmartSettingsEditing;
        internal MemorySettings MemorySettingList;
        Setting defSetting;
        string resetGridLayout;
        #region ActualConfig
        string actualConfig = GridLayoutHelper.DesignSettingName;
        internal string actualValidationConfig = string.Empty;
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
        internal IDocument document;
        Helper helper;
        DataValidationViewModel viewModel;
        CancellationTokenSource cts;

        bool bLoaded;
        bool bInit;
        bool bTranslateColumnsOnInit;
        bool bDisposed;
        bool bDesign;
        IDictionary<String, String> stringlist;
        IStringEditorManager stringManager;
        bool hasStorageSourceValue;
        string oldusername;
        #endregion

        #region Constructors
        public DataValidation()
        {
            InitializeComponent();
            OVerrideBaseProperties();
            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbar.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (s, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                if (RunningOnServer)
                {
                    toolbar.Bars.Clear();
                    toolbar.Bars.Add(toolBarControl);
                    DataSourceLabel.IsVisible = false;
                    DataSourceLabel_web.IsVisible = true;
                    ((DateEditSettings)StartDateTimeLabel.EditSettings).AllowDefaultButton = false;
                    ((DateEditSettings)EndDateTimeLabel.EditSettings).AllowDefaultButton = false;
                }
#if !WINDOWS_UWP
                this.AddToolBarStyleResource();
#endif

                if (!bSmartSettingsEditing)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                //configMemory-.EditValue = GridLayoutHelper.DesignSettingName;
                configMemory.DataContext = MemorySettingList?.Names;

                cmbExportType.ItemsSource = Enum.GetValues(typeof(ExportFileType)).Cast<ExportFileType>();
                cmbExportType.SelectedIndex = 0;

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
                UpdateControlLayout();
                bDesign = bDesign || DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;
                if (bDesign)
                {
                    //view.IsHitTestVisible = false;
                    busyControl.Visibility = progressBar.Visibility = Visibility.Collapsed;

                    toolbarSettings.IsEnabled = false;
                    if (!bSmartSettingsEditing)
                        view.IsHitTestVisible = false;
                    else
                    {
                        options.IsHitTestVisible = false;
                        commands.IsHitTestVisible = false;
                        backupFiles.IsHitTestVisible = false;
                        //StartDateTimeLabel.IsHitTestVisible = false;
                        //EndDateTimeLabel.IsHitTestVisible = false;
                        //stackValidation.IsHitTestVisible = false;
                        //printBut.IsHitTestVisible = false;

                        hasStorageSourceValue = true;
                        var designSource = string.Empty;
                        if (this.ReadLocalValue(ValidationSourceProperty) != DependencyProperty.UnsetValue && !string.IsNullOrEmpty(ValidationSource))
                        {
                            designSource = ValidationSource;
                            hasStorageSourceValue = false;
                        }

                        LoadData(designSource, true);
                    }
                }
                else
                {
                    helper = new Helper(document, this);
                    helper.RefreshCurrentUser();
                    oldusername = helper.Username;
                    hasStorageSourceValue = true;
                    var designSource = string.Empty;
                    bool bLoadConfig = false;
                    if (this.ReadLocalValue(ValidationSourceProperty) != DependencyProperty.UnsetValue && !string.IsNullOrEmpty(ValidationSource))
                    {
                        designSource = ValidationSource;
                        hasStorageSourceValue = false;
                    }

                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        bLoadConfig = true;

                    if (RunningOnServer)
                        toolbarSettings.IsVisible = false;
                    else
                    {
                        MouseEnter += GridControl_MouseEnter;
                    }

                    LoadData(designSource, bLoadConfig);
                   
                }
            };
        }

        private void LoadData(string designSource, bool bLoadConfig)
        {
            BaseValidation currentItem = null;

            cts = new CancellationTokenSource();
            var token = cts.Token;
            var task1 = Task.Factory.StartNew(() =>
            {
                var dataValidationItems = new List<BaseValidation>();
                if (UFUAEditorService != null)
                {
                    token.ThrowIfCancellationRequested();

                    var defaultConnection = UFUAEditorService.GetHistorianDefaultConnection(document);
                    if (document is ScreenDocument)
                        defaultConnection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(defaultConnection, (document as ScreenDocument).SessionString);

                    if (UFUAEditorService.IsEventDataProtectionEnabled(document))
                    {
                        var eventDefaultConnection = UFUAEditorService.GetEventDefaultConnection(document);
                        if (document is ScreenDocument)
                            eventDefaultConnection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(eventDefaultConnection, (document as ScreenDocument).SessionString);
                        dataValidationItems.Add(new EventValidation(Properties.Resources.EventDataSourceName, Properties.Settings.Default.EventDataSourceName, eventDefaultConnection));
                        if (currentItem == null && !String.IsNullOrEmpty(designSource) && designSource == Properties.Settings.Default.EventDataSourceName)
                            currentItem = dataValidationItems.LastOrDefault();

                        if (UFUAEditorService.IsAtLeastOneAuditTraceEnabled(document))
                        {
                            var auditTraceDefaultConnection = UFUAEditorService.GetAuditTraceDefaultConnection(document);
                            if (document is ScreenDocument)
                                auditTraceDefaultConnection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(auditTraceDefaultConnection, (document as ScreenDocument).SessionString);
                            dataValidationItems.Add(new HistorianValidation(Properties.Resources.AuditTraceSourceName, Properties.Settings.Default.AuditTraceSourceName, auditTraceDefaultConnection));
                            if (currentItem == null && !String.IsNullOrEmpty(designSource) && designSource == Properties.Settings.Default.AuditTraceSourceName)
                                currentItem = dataValidationItems.LastOrDefault();
                        }
                    }

                    var historicalNames = UFUAEditorService.GetHistoricalSettingsNameList(document, bReloadDocument: false, inExecution: true);
                    if (historicalNames != null)
                    {
                        foreach (var historicalName in historicalNames)
                        {
                            token.ThrowIfCancellationRequested();

                            if (UFUAEditorService.IsHistorianDataProtectionEnabled(document, historicalName))
                            {
                                var settings = UFUAEditorService.GetHistorianConnection(document, historicalName);
                                if (string.IsNullOrEmpty(settings))
                                    settings = defaultConnection;
                                else if (document is ScreenDocument)
                                    settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(settings, (document as ScreenDocument).SessionString);
                                dataValidationItems.Add(new HistorianValidation(historicalName, settings));
                                if (currentItem == null && !String.IsNullOrEmpty(designSource) && designSource == historicalName)
                                    currentItem = dataValidationItems.LastOrDefault();
                            }
                        }
                    }

                    var dataloggerNames = UFUAEditorService.GetDataLoggerSettingsNameList(document, inExecution: true);
                    if (dataloggerNames != null)
                    {
                        foreach (var dataloggerName in dataloggerNames)
                        {
                            token.ThrowIfCancellationRequested();

                            var settings = UFUAEditorService.GetDataLoggerDataTable(document, dataloggerName, inExecution: true);
                            if (settings == null)
                                continue;
                            var dataLoggerTable = settings.FromXml<DataLoggerTable>();
                            if (dataLoggerTable.IsDataProtectionEnabled)
                            {
                                if (document is ScreenDocument && dataLoggerTable.ConnectionSettings != null && !String.IsNullOrEmpty(dataLoggerTable.ConnectionSettings.Connection))
                                    dataLoggerTable.ConnectionSettings.Connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(dataLoggerTable.ConnectionSettings.Connection, (document as ScreenDocument).SessionString);
                                dataValidationItems.Add(new DataLoggerValidation(dataLoggerTable, defaultConnection));
                                if (currentItem == null && !String.IsNullOrEmpty(designSource) && designSource == dataLoggerTable.Name)
                                    currentItem = dataValidationItems.LastOrDefault();
                            }
                        }
                    }
                }

                return dataValidationItems;
            }, token);
            task1.ContinueWith(ret =>
            {
                cts.Dispose();
                cts = null;

                if (token.IsCancellationRequested)
                    return;

                if (ret.Result != null)
                {
                    viewModel = new DataValidationViewModel(ret.Result, document);
                    viewModel.UIInterface = UIInterface;
                    viewModel.QueryTimeout = ExecuteQueryTimeout;
                    viewModel.MaxUnauthorizedAccess = MaxErrorBeforeAbort;
                    viewModel.ValidationSID = CustomValidationSID;

                    if (currentItem != null)
                    {
                        viewModel.CurrentItem = currentItem;
                        viewModel.AllowChangeItemSource = hasStorageSourceValue;
                    }
                    view.DataContext = viewModel;
                    if (ReadDataOnLoading && viewModel.LoadDataCommand.CanExecute(null))
                        viewModel.LoadDataCommand.Execute(LoadingFilterType);
                }

                if (string.IsNullOrEmpty(GridLayout))
                    SaveDesignGridLayout();
                if (!bDesign)
                {
                    SaveDesignGridLayout();
                    SaveResetGridLayout();

                    var actualgridlayout = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(document, Name, helper.Username)?.FirstOrDefault(s => s.Name == GridLayoutHelper.DesignSettingName)?.GridLayout;

                        if (actualgridlayout != null)
                          GridLayout = actualgridlayout;
                }
                
                

                defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout, Option1 = ValidationSource }, document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null, true);
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;

                if (bDesign && !string.IsNullOrEmpty(actualValidationConfig))
                {
                    currentItem = (from item in viewModel.ItemsSource where item.InvariantName == actualValidationConfig select item).FirstOrDefault();
                    if (currentItem != null)
                        viewModel.CurrentItem = currentItem;
                }

                if (!bDesign)
                {
                    if (bLoadConfig)
                    {
                        try
                        {
                            LoadRuntimeLayout(GetStorageName());
                            GetItem(ActualConfig, true);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    bInit = true;

                    if (bTranslateColumnsOnInit)
                    {
                        bTranslateColumnsOnInit = false;
                        UpdateColumns();
                    }
                    if (!bControlLoaded)
                    {
                        bControlLoaded = true;
                        OnControlLoaded();
                    }

                }
                else
                {
                    LoadDesignGridLayout();
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void OVerrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataValidation));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataValidation));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataValidation));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataValidation));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as DataValidation;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (bInit || (bDesign && bLoaded))
                UpdateControlLayout();
        }


        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as DataValidation;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bInit && !IsManipulationEnabled) || (bDesign && bLoaded))
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
        #endregion

        #region EventHandlers
        public event EventHandler ControlLoaded;
        bool bControlLoaded;
        void OnControlLoaded()
        {
            ControlLoaded?.Invoke(this, EventArgs.Empty);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is DataValidationViewModel))
                DataContext = e.OldValue;
        }
        #endregion

        #region Commands


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
                SaveRuntimeLayout();
                GetItem(ActualConfig, true);
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
                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
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
        internal void CallSaveCommand(bool bForce = false)
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            try
            {
                bCallingSaveCommand = true;
                string configname = configMemory.EditValue as String;

                if (!bForce)
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
                        Option1 = !bForce ? ValidationSource : actualValidationConfig,
                        ReadOnly = bForce
                    });
                    configMemory.DataContext = MemorySettingList?.Names;
                    configMemory.EditValue = configname;
                }
                else
                {
                    selected.GridLayout = GridLayout;
                    selected.Option1 = !bForce ? viewModel?.CurrentItem?.InvariantName : actualValidationConfig;
                    selected.ReadOnly = bForce;
                    configMemory.DataContext = MemorySettingList?.Names;
                    configMemory.EditValue = selected.Name;
                }

                if (StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, document, Name, UserBasedRuntimeSettings ? helper.Username : null))
                {
                    if (oldMemoryList != null)
                        oldMemoryList.Clear();
                    oldMemoryList = null;
                    oldConfigName = null;
                }
                ActualConfig = configname;
                SaveGridConfiguration();
                SaveRuntimeLayout();
            }
            finally
            {
                bCallingSaveCommand = false;
            }
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

            StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            configMemory.DataContext = MemorySettingList.Names;
            bCallingRemoveCommand = false;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }
        bool IsEnableCommand
        {
            get
            {
                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
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
                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
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
            {
                e.Handled = true;
                return;
            }
            bIsInEditMode = true;
        }

        RelayCommand exportDataCommand;
        public ICommand ExportDataCommand
        {
            get
            {
                if (exportDataCommand == null)
                {
                    exportDataCommand = new RelayCommand(
                        param => OnExportData(param),
                        param => CanExportData()
                        );
                }
                return exportDataCommand;
            }
        }
        bool CanExportData()
        {
            return view.DataContext != null && view.DataContext is DataValidationViewModel && gridControl != null && gridControl.ItemsSource != null;
        }

        private void OnExportData(object param)
        {
            if (param != null)
                try
                {
                    ExportData((System.Convert.ToInt16(param)));
                }
                catch
                {
                }
            else
                ExportData();
        }
        RelayCommand printCommand;
        public ICommand PrintCommand
        {
            get
            {
                if (printCommand == null)
                {
                    printCommand = new RelayCommand(
                        param => PrintGrid(),
                        param => viewModel != null && viewModel.CurrentItem != null && !viewModel.IsRunning
                        );
                }
                return printCommand;
            }
        }

        void ExportData(object param = null)
        {
            string file = String.Empty;

            if (Environment.UserInteractive)
            {
                VistaOpenFileDialog dialog = new VistaOpenFileDialog();
                dialog.CheckFileExists = false;
                dialog.ValidateNames = true;
                dialog.FileName = string.Format("{0}_{1}{2}{3}_{4}{5}{6}.{7}", Properties.Resources.ExportFileTitle, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, cmbExportType.SelectedValue);
                dialog.Filter = string.Format("{0} files|*.{0}", cmbExportType.SelectedValue);
                if (dialog.ShowDialog() == true)
                {
                    file = dialog.FileName;
                }
            }
            if (String.IsNullOrEmpty(file))
                return;

            int index = cmbExportType.SelectedIndex;
            //var task1 = Task.Factory.StartNew(delegate
            using (new WaitCursor())
            {
                try
                {
                    using (FileStream sw = new FileStream(file, FileMode.OpenOrCreate))
                    {
                        //Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            switch ((ExportFileType)index)
                            {
                                case ExportFileType.Csv:
                                    var optionsCsv = new DevExpress.XtraPrinting.CsvExportOptionsEx();
                                    tableView.ExportToCsv(sw, optionsCsv);
                                    break;
                                case ExportFileType.Html:
                                    tableView.ExportToHtml(sw);
                                    break;
                                case ExportFileType.Xls:
                                    var optionsXls = new DevExpress.XtraPrinting.XlsExportOptionsEx();
                                    tableView.ExportToXls(sw, optionsXls);
                                    break;
                                case ExportFileType.Pdf:
                                    tableView.ExportToPdf(sw);
                                    break;
                                default:
                                    break;
                            }
                            // flush from the buffers.
                            sw.Flush();
                            // closes the file
                            sw.Close();
                        }//);
                    }
                }
                catch
                {

                }
            }//);
        }

        string stringPlaceolder = "DataValidation";
        void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesign && stringManager.GetActiveCulture(document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
                else
                    stringlist = null;

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                DataSourceCombo.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DataSourceLabel", stringlist, Properties.Resources.DataSourceNameLabel);
                StartDateTimeLabel.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDateTimeLabel", stringlist, Properties.Resources.StartDateTimeLabel);
                EndDateTimeLabel.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDateTimeLabel", stringlist, Properties.Resources.EndDateTimeLabel);
                ValidationResultLabel.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ValidationResultLabel", stringlist, Properties.Resources.LastValidationResultLabel);

                configMemory.EditValue = ActualConfig;
                Button1.Content = Button1.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.Filter_Minute);
                Button2.Content = Button2.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.Filter_Hour);
                Button3.Content = Button3.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.Filter_Day);
                Button4.Content = Button4.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.Filter_Week);
                Button5.Content = Button5.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Filter_Month);
                Button6.Content = Button6.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", stringlist, Properties.Resources.Filter_Year);
                Button7.Content = Button7.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_All", stringlist, Properties.Resources.Filter_All);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                ReadDataBtn.ToolTip = ReadDataTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReadDataBtn", stringlist, Properties.Resources.ReadDataBtnTitle);
                StartValidateBtn.ToolTip = StartValidateTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartValidateBtn", stringlist, Properties.Resources.StartValidateBtnTitle);
                AbortValidateBtn.ToolTip = AbortValidateTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AbortValidateBtn", stringlist, Properties.Resources.AbortValidateBtnTitle);
                AbortOnFirstErrorBtn.ToolTip = AbortOnFirstErrorTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AbortOnFirstErrorBtn", stringlist, Properties.Resources.AbortOnFirstError);
                ShowBackupFileBtn.ToolTip = ShowBackupFileTxt.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ShowBackupFileBtn", stringlist, Properties.Resources.ShowBackupFileBtnTitle);
                printBut.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintButTooltip", stringlist, Properties.Resources.PrintTitle);
                exportButton.ToolTip = txtExport.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);
                totUnauthAccess.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalUnauthorizedAccessLabel", stringlist, Properties.Resources.TotalUnauthorizedAccessLabel);
                bestFit.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);

                if (bInit)
                    UpdateColumns();
                else
                    bTranslateColumnsOnInit = true;
            });
        }

        public void OnCurrentItemChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            actualValidationConfig = ((DevExpress.Xpf.Bars.BarEditItem)e.OriginalSource).EditValue as String;
            UpdateColumns();
        }

        void UpdateColumns()
        {
            if (stringManager == null)
                return;

            TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);
        }

        void PrintGrid()
        {
            if (/*watermarkImage.Source == null || */viewModel == null)
                return;

            CsvExportOptions.FollowReportLayout = false;

            //UtilitiesPrintHelper.PrintControl(controlWindow, gridView, null, $"{Properties.Resources.ControlTitle}", false, System.Drawing.Printing.PaperKind.A4);
            using (var print = new PrintableControlLink(gridControl.View))
            {
                DocumentPreviewWindow preview = new DocumentPreviewWindow() { Owner = this.FindParent<Window>() };
                //Watermark imgWmark = new Watermark();
                //imgWmark.Image = ImageHelper.BitmapImageToImage((BitmapImage)watermarkImage.Source, ImageHelper.BitmapEncoderFormat.Png);
                //if (imgWmark.Image == null)
                //    return;
                //imgWmark.ImageTiling = true;
                //imgWmark.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
                //imgWmark.ImageViewMode = ImageViewMode.Clip;
                //imgWmark.ShowBehind = true;
                //imgWmark.ImageTransparency = 204; //0 to 255
                //print.PrintingSystem.Watermark.CopyFrom(imgWmark);

                print.PrintingSystem.Watermark.Text = ValidationResultToStringConverter.Convert(viewModel.ValidationResult);
                print.PrintingSystem.Watermark.TextDirection = DirectionMode.ForwardDiagonal;
                var fontSize = print.PrintingSystem.PageSettings.UsablePageSize.Height / Math.Max(1, Properties.Settings.Default.WatermarkFontPageRatio);
                print.PrintingSystem.Watermark.Font = new System.Drawing.Font(print.PrintingSystem.Watermark.Font.FontFamily, fontSize);
                var fontColor = ValidationResultToColorConverter.Convert(viewModel.ValidationResult).Color;
                print.PrintingSystem.Watermark.ForeColor = System.Drawing.Color.FromArgb(fontColor.A, fontColor.R, fontColor.G, fontColor.B);
                print.PrintingSystem.Watermark.TextTransparency = Properties.Settings.Default.WatermarkPrintTransparency;
                print.PrintingSystem.Watermark.ShowBehind = false;

                print.PrintingSystem.PageSettingsChanged += (s, e) =>
                {
                    fontSize = fontSize = print.PrintingSystem.PageSettings.UsablePageSize.Height / Math.Max(1, Properties.Settings.Default.WatermarkFontPageRatio);
                    print.PrintingSystem.Watermark.Font = new System.Drawing.Font(print.PrintingSystem.Watermark.Font.FontFamily, fontSize);
                };

                preview.PreviewControl.DocumentSource = print;

                var provider = new DocumentCommandProvider();
                preview.PreviewControl.CommandProvider = provider;
                var action = new RemoveAction();
                action.ElementName = DefaultPreviewBarItemNames.Watermark;
                provider.RibbonActions.Add(action);
                action = new RemoveAction();
                action.ElementName = DefaultPreviewBarItemNames.Open;
                provider.RibbonActions.Add(action);

                ThemeHelper.SetTheme(preview);
                print.PaperKind = System.Drawing.Printing.PaperKind.A4;

                // Page Header
                var templateHeader = new DataTemplate();
                var controlHeader = new FrameworkElementFactory(typeof(StackPanel));
                controlHeader.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                templateHeader.VisualTree = controlHeader;

                var textEdit = new FrameworkElementFactory(typeof(TextEdit));
                textEdit.SetValue(TextEdit.WidthProperty, new Binding("UsablePageWidth") { Converter = new AdjustWidthConverter(), ConverterParameter = 1 });
                textEdit.SetValue(TextEdit.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
                textEdit.SetValue(TextEdit.EditValueProperty, String.Format("{0} {1} - {2} {3}", Properties.Resources.StartDateTimeLabel, viewModel.StartDateTime, Properties.Resources.EndDateTimeLabel, viewModel.EndDateTime));
                controlHeader.AppendChild(textEdit);

                var controlSubHeader = new FrameworkElementFactory(typeof(StackPanel));
                controlSubHeader.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                controlHeader.AppendChild(controlSubHeader);

                textEdit = new FrameworkElementFactory(typeof(TextEdit));
                textEdit.SetValue(TextEdit.WidthProperty, new Binding("UsablePageWidth") { Converter = new AdjustWidthConverter(), ConverterParameter = 2 });
                textEdit.SetValue(TextEdit.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
                textEdit.SetValue(TextEdit.EditValueProperty, String.Format("{0} {1}", Properties.Resources.DataSourceNameLabel, viewModel.CurrentItem.Name));
                controlSubHeader.AppendChild(textEdit);

                textEdit = new FrameworkElementFactory(typeof(TextEdit));
                textEdit.SetValue(StackPanel.WidthProperty, new Binding("UsablePageWidth") { Converter = new AdjustWidthConverter(), ConverterParameter = 2 });
                textEdit.SetValue(TextEdit.HorizontalContentAlignmentProperty, HorizontalAlignment.Right);
                textEdit.SetValue(TextEdit.EditValueProperty, String.Format("{0} {1}", Properties.Resources.LastValidationResultLabel, ValidationResultToStringConverter.Convert(viewModel.ValidationResult)));
                controlSubHeader.AppendChild(textEdit);

                print.PageHeaderTemplate = templateHeader;

                // Page Footer
                var templateFooter = new DataTemplate();
                var controlFooter = new FrameworkElementFactory(typeof(StackPanel));
                controlFooter.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                templateFooter.VisualTree = controlFooter;

                textEdit = new FrameworkElementFactory(typeof(TextEdit));
                textEdit.SetValue(StackPanel.WidthProperty, new Binding("UsablePageWidth") { Converter = new AdjustWidthConverter(), ConverterParameter = 2 });
                textEdit.SetValue(TextEdit.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
                textEdit.SetValue(TextEdit.EditValueProperty, DateTime.Now);
                controlFooter.AppendChild(textEdit);

                textEdit = new FrameworkElementFactory(typeof(TextEdit));
                textEdit.SetValue(StackPanel.WidthProperty, new Binding("UsablePageWidth") { Converter = new AdjustWidthConverter(), ConverterParameter = 2 });
                textEdit.SetValue(TextEdit.HorizontalContentAlignmentProperty, HorizontalAlignment.Right);
                textEdit.SetValue(DevExpress.Xpf.Printing.ExportSettings.TargetTypeProperty, TargetType.PageNumber);
                textEdit.SetValue(PageNumberExportSettings.FormatProperty, Properties.Resources.PrintPageNumber);
                textEdit.SetValue(PageNumberExportSettings.KindProperty, PageNumberKind.NumberOfTotal);
                controlFooter.AppendChild(textEdit);

                //print.PrintReportFooterAtBottom = true;
                print.PageFooterTemplate = templateFooter;

                //using (var stream = new System.IO.MemoryStream())
                //{
                //    gridControl.SaveLayoutToStream(stream);
                //    stream.Seek(0, System.IO.SeekOrigin.Begin);

                //    ((TableView)gridControl.View).BestFitColumns();

                print.CreateDocument(true);

                //    gridControl.RestoreLayoutFromStream(stream);
                //}
                preview.ShowDialog();
            }
        }

        RelayCommand addBackupFileCommand;
        public ICommand AddBackupFileCommand
        {
            get
            {
                if (addBackupFileCommand == null)
                {
                    addBackupFileCommand = new RelayCommand(
                        param => AddBackupFile(),
                        param => viewModel != null && !viewModel.IsRunning
                        );
                }
                return addBackupFileCommand;
            }
        }

        void AddBackupFile()
        {
            if (viewModel == null)
                return;

            if (RunningOnServer || UIInterface == null)
                viewModel.BackupFiles.Add(new BackupFileViewModel(Properties.Resources.NewBackupFilePlaceHolder));
            else
            {
                String[] filePaths = UIInterface.ShowSelectFileDialog(Properties.Resources.BackupDialogFilterString);

                if (filePaths != null && filePaths.Length > 0)
                {
                    viewModel.IsRunning = true;
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var existingFiles = new List<String>();
                        viewModel.BackupFiles.ToList().ForEach(backup => existingFiles.Add(backup.FilePath));
                        return existingFiles;
                    });
                    task1.ContinueWith(ret =>
                    {
                        foreach (var filePath in filePaths)
                        {
                            if (ret.Result.Contains(filePath))
                                continue;

                            viewModel.BackupFiles.Add(new BackupFileViewModel(filePath));
                        }

                        viewModel.IsRunning = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        RelayCommand removeBackupFileCommand;
        public ICommand RemoveBackupFileCommand
        {
            get
            {
                if (removeBackupFileCommand == null)
                {
                    removeBackupFileCommand = new RelayCommand(
                        param => RemoveBackupFile(),
                        param => viewModel != null && !viewModel.IsRunning && gridBackupFiles.SelectedItem != null
                        );
                }
                return removeBackupFileCommand;
            }
        }

        void RemoveBackupFile()
        {
            if (viewModel == null)
                return;

            var itemsToRemove = new List<BackupFileViewModel>();
            foreach (BackupFileViewModel selectedItem in gridBackupFiles.SelectedItems)
            {
                if (selectedItem.IsReadOnly)
                    continue;

                itemsToRemove.Add(selectedItem);
            }

            itemsToRemove.ForEach((item) => viewModel.BackupFiles.Remove(item));
        }

        RelayCommand clearBackupFilesCommand;
        public ICommand ClearBackupFilesCommand
        {
            get
            {
                if (clearBackupFilesCommand == null)
                {
                    clearBackupFilesCommand = new RelayCommand(
                        param => ClearBackupFiles(),
                        param => viewModel != null && !viewModel.IsRunning
                        );
                }
                return clearBackupFilesCommand;
            }
        }

        void ClearBackupFiles()
        {
            if (viewModel == null)
                return;

            viewModel.FillBackupFiles();
        }
        #endregion

        #region Methods

        #region EditSettings
        bool bUserInteractionSettings;
        private void GetItem(string itemName, bool bForceCreateDataSource = false)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (!string.IsNullOrEmpty(setting?.GridLayout))
                {
                    GridLayout = setting.GridLayout;
                    if (viewModel != null && viewModel.AllowChangeItemSource)
                        ValidationSource = setting.Option1;
                    ActualConfig = setting.Name;
                }
                else
                {
                    GridLayout = defSetting.GridLayout;
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                    if (viewModel != null && viewModel.AllowChangeItemSource)
                        ValidationSource = defSetting.Option1;
                }

                bIsInEditMode = true;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = ActualConfig;
                bIsInEditMode = false;

                if (bForceCreateDataSource && !bDesign)
                {
                    var currentitem = (from item in viewModel.ItemsSource where item.InvariantName == ValidationSource select item).FirstOrDefault();
                    viewModel.CurrentItem = currentitem;
                    Dispatcher.BeginInvokeInBackgroundIfRequired(() => LoadDesignGridLayout());
                }
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
            MemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            return (from n in list select n.Name).ToList();
        }
        internal void SaveRuntimeLayout()
        {
            try
            {
                string title = GetStorageName();
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
                SaveDesignGridLayout();
                if (Selected != null)
                    Selected.GridLayout = GridLayout;
                else
                {
                    Selected = new Setting() { GridLayout = GridLayout,  Name = GridLayoutHelper.DesignSettingName, ReadOnly = true };
                    MemorySettingList.Add(Selected);
                }
                string currentUserName = helper.Username;
                string userName = oldusername != currentUserName ? oldusername : currentUserName;
                StorageHelper.StorageHelper.SaveMemoryMap(MemorySettingList, document, Name, userName);
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

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.AutoWidth = false;
            tableView.BestFitColumns();
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
        #endregion

        #region Overrides
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
        }
        #endregion

        #region Properties
        IUFUAEditorManager ufuaEditorService;
        [Browsable(false)]
        IUFUAEditorManager UFUAEditorService
        {
            get
            {
                if (ufuaEditorService == null && document != null)
                    ufuaEditorService = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditorService;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null && document != null)
                    uiInterface = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        [Browsable(false)]
        bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        BarEditItem DataSourceCombo
        {
            get
            {
                return RunningOnServer ? DataSourceLabel_web : DataSourceLabel;
            }
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (document == null)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                //var dt = new DataTemplate();
                //var factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                //factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, "hh:mm:ss");
                //dt.DataType = typeof(TimeSpan);
                //dt.VisualTree = factory;
                //mapDataTemplates.Add(DateTimeToleranceProperty.Name, dt);

                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ValidationSourcePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ValidationSourceProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (cts != null)
                cts.Cancel();
            if (!bDesign)
            {                
                if (bControlLoaded && GridLayoutHelper.DesignSettingName == ActualConfig && !Editable && UserBasedRuntimeSettings)
                    SaveGridConfiguration();
            }
            if (helper != null)
                helper.Dispose();

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            if (viewModel != null)
            {
                viewModel.AbortValidateData();
            }
            if (dpUpdateLayout != null &&
               dpUpdateLayout.Status != DispatcherOperationStatus.Aborted &&
               dpUpdateLayout.Status != DispatcherOperationStatus.Completed)
                dpUpdateLayout.Abort();
            DetachOverrideBaseProperties();
        }
        #endregion

        #region Isolated Storage
        String GetStorageName(bool useParent = false)
        {
            return StorageHelper.StorageHelper.GetStorageName(document, this.Name, UserBasedRuntimeSettings ? helper.Username : null, true);
        }

        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
        }

        static IsolatedStorageFile GetStorage()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }

        //void SaveRuntimeLayout(String title)
        //{
        //    try
        //    {
        //        var isoStorage = GetStorage();
        //        if (null == isoStorage || string.IsNullOrEmpty(title))
        //            return;

        //        using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.Create, isoStorage))
        //        {
        //            using (StreamWriter writer = new StreamWriter(stream))
        //            {
        //                try
        //                {
        //                    writer.Write(viewModel.CurrentItem.Name);
        //                }
        //                finally
        //                {
        //                    writer.Close();
        //                }
        //                return;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //string LoadRuntimeLayout(String title)
        //{
        //    string ret = string.Empty;
        //    try
        //    {
        //        var isoStorage = GetStorage();
        //        if (null == isoStorage || string.IsNullOrEmpty(title))
        //            return ret;

        //        using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.OpenOrCreate, isoStorage))
        //        {
        //            using (StreamReader reader = new StreamReader(stream))
        //            {
        //                try
        //                {
        //                    ret = reader.ReadToEnd();
        //                }
        //                finally
        //                {
        //                    reader.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return ret;
        //}
        #endregion

        #region ISettingsHelper
        public int EditingWriteAccessLevel { get; }
        public int EditingWriteAccessMask { get; }
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
                    MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(document, Name, helper.Username);
                else
                    EnsureDefaultValue(false);
                if (!LoadRuntimeLayout(GetStorageName(true)))
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                GetItem(ActualConfig);
                oldusername = helper.Username;
            });
        }

        void EnsureDefaultValue(bool bSetCombo = true)
        {
            defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout, Option1 = ValidationSource }, document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null, true);
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

        public void Initialize()
        {
        }
        #endregion

        private void FilterEditorControl_QueryOperators(object sender, DevExpress.Xpf.Core.FilteringUI.FilterEditorQueryOperatorsEventArgs e)
        {
            IDictionary<string, object> value = viewModel.CurrentItem.Rows[0].DataRow as IDictionary<string, object>;
            Type valuetype = value[e.FieldName].GetType();

            gridControl.Columns[e.FieldName].ColumnFilterMode = valuetype == typeof(string) ? ColumnFilterMode.DisplayText : ColumnFilterMode.Value;
        }

        private void gridControl_FilterChanged(object sender, RoutedEventArgs e)
        {
            IDictionary<string, object> value = viewModel.CurrentItem.Rows[0].DataRow as IDictionary<string, object>;
            string filter = gridControl.FilterString;
            int startbreaketindex = filter.IndexOf('[');
            if (startbreaketindex >= 0)
            {
                int fieldnameindex = startbreaketindex + 1;
                int endbreaketindex = filter.IndexOf(']');
                Type valuetype = value[filter.Substring(fieldnameindex, endbreaketindex - fieldnameindex)].GetType();
                if(endbreaketindex >= 0)
                if (valuetype != typeof(string) && valuetype != typeof(DateTime))
                    gridControl.FilterString = gridControl.FilterString.Replace("'", string.Empty);
                else
                {
                    if(valuetype == typeof(DateTime))
                            gridControl.FilterString = gridControl.FilterString.Replace("'","#");
                }
            }
        }
    }
}
