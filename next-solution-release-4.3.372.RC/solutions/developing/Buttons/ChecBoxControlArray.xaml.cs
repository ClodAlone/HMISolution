using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using Utilities;
using OPCUAViewModel;
using System.Windows.Threading;
using log4net;
using ScreenSettings;
using ScreenManager.ComponentService;
using AuditTrace;
using UFProjectManager.ComponentService;
using System.Xml.Serialization;

namespace Buttons
{
    /// <summary>
    /// Interaction logic for ChecBoxControlArray.xaml
    /// </summary>
    public partial class CheckBoxControlArray : UserControl, IContainPropertyEditors, IDisposable
        , IStringIDAware
    {
        #region DP
        #region BulletDimension
        public static readonly DependencyProperty BulletDimensionProperty = DependencyProperty.Register("BulletDimension", typeof(double), typeof(CheckBoxControlArray), new UIPropertyMetadata(25.0, new PropertyChangedCallback(OnBulletDimensionChanged), new CoerceValueCallback(OnCoerceBulletDimension)));

        private static object OnCoerceBulletDimension(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceBulletDimension((double)value);
            else
                return value;
        }

        private static void OnBulletDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnBulletDimensionChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceBulletDimension(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBulletDimensionChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("CheckBoxControlArrayOptions")]
        public double BulletDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(BulletDimensionProperty);
            }
            set
            {
                SetValue(BulletDimensionProperty, value);
            }
        }

        #endregion

        #region ControlArray
        public static readonly DependencyProperty ControlArrayProperty = DependencyProperty.Register("ControlArray", typeof(int), typeof(CheckBoxControlArray), new UIPropertyMetadata(3, new PropertyChangedCallback(OnControlArrayChanged), new CoerceValueCallback(OnCoerceControlArray)));

        private static object OnCoerceControlArray(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceControlArray((int)value);
            else
                return value;
        }

        private static void OnControlArrayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnControlArrayChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceControlArray(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlArrayChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateControlLayout();
        }

        public int ControlArray
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ControlArrayProperty);
            }
            set
            {
                SetValue(ControlArrayProperty, value);
            }
        }

        #endregion

        #region ArrayOrientation
        public static readonly DependencyProperty ArrayOrientationProperty = DependencyProperty.Register("ArrayOrientation", typeof(Orientation), typeof(CheckBoxControlArray), new UIPropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnArrayOrientationChanged), new CoerceValueCallback(OnCoerceArrayOrientation)));

        private static object OnCoerceArrayOrientation(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceArrayOrientation((Orientation)value);
            else
                return value;
        }

        private static void OnArrayOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnArrayOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
        }

        protected virtual Orientation OnCoerceArrayOrientation(Orientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnArrayOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (stackPanel != null)
                stackPanel.Orientation = ArrayOrientation;
            if (wrapPanel != null)
                wrapPanel.Orientation = ArrayOrientation;
        }

        public Orientation ArrayOrientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Orientation)GetValue(ArrayOrientationProperty);
            }
            set
            {
                SetValue(ArrayOrientationProperty, value);
            }
        }

        #endregion

        #region Content
        public static readonly DependencyProperty ContentStringsProperty = DependencyProperty.Register("ContentStrings", typeof(string), typeof(CheckBoxControlArray), new UIPropertyMetadata("CheckBox", new PropertyChangedCallback(OnContentChanged), new CoerceValueCallback(OnCoerceContent)));

        private static object OnCoerceContent(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceContent((string)value);
            else
                return value;
        }

        private static void OnContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnContentChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceContent(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnContentChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateContent(true);
        }

        public string ContentStrings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ContentStringsProperty);
            }
            set
            {
                SetValue(ContentStringsProperty, value);
            }
        }

        #endregion

        #region PostFixFormat
        public static readonly DependencyProperty PostFixFormatProperty = DependencyProperty.Register("PostFixFormat", typeof(string), typeof(CheckBoxControlArray), new UIPropertyMetadata("{0:#}", new PropertyChangedCallback(OnPostFixFormatChanged), new CoerceValueCallback(OnCoercePostFixFormat)));

        private static object OnCoercePostFixFormat(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoercePostFixFormat((string)value);
            else
                return value;
        }

        private static void OnPostFixFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnPostFixFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoercePostFixFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPostFixFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateContent();
        }

        public string PostFixFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(PostFixFormatProperty);
            }
            set
            {
                SetValue(PostFixFormatProperty, value);
            }
        }

        #endregion
        
        #region CheckBoxMargin
        public static readonly DependencyProperty CheckBoxMarginProperty = DependencyProperty.Register("CheckBoxMargin", typeof(Thickness), typeof(CheckBoxControlArray), new UIPropertyMetadata(new Thickness(2), new PropertyChangedCallback(OnCheckBoxMarginChanged), new CoerceValueCallback(OnCoerceCheckBoxMargin)));

        private static object OnCoerceCheckBoxMargin(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceCheckBoxMargin((Thickness)value);
            else
                return value;
        }

        private static void OnCheckBoxMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnCheckBoxMarginChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
        }

        protected virtual Thickness OnCoerceCheckBoxMargin(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCheckBoxMarginChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateMargin();
        }

        public Thickness CheckBoxMargin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(CheckBoxMarginProperty);
            }
            set
            {
                SetValue(CheckBoxMarginProperty, value);
            }
        }

        #endregion


        #region UseWrapPanel
        public static readonly DependencyProperty UseWrapPanelProperty = DependencyProperty.Register("UseWrapPanel", typeof(bool), typeof(CheckBoxControlArray), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseWrapPanelChanged), new CoerceValueCallback(OnCoerceUseWrapPanel)));

        private static object OnCoerceUseWrapPanel(DependencyObject o, object value)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                return control.OnCoerceUseWrapPanel((bool)value);
            else
                return value;
        }

        private static void OnUseWrapPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CheckBoxControlArray control = o as CheckBoxControlArray;
            if (control != null)
                control.OnUseWrapPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseWrapPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseWrapPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                ChangeContentLayout();
        }

        public bool UseWrapPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseWrapPanelProperty);
            }
            set
            {
                SetValue(UseWrapPanelProperty, value);
            }
        }

        #endregion


        #region OptionStyle
        public static readonly DependencyProperty OptionStyleProperty = DependencyProperty.Register("CheckBoxStyle", typeof(string), typeof(CheckBoxControlArray), new UIPropertyMetadata("ThreeStateCheckBoxControl"));

        [Browsable(false)]
        [XmlIgnore]
        public string OptionStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(OptionStyleProperty);
            }
            set
            {
                SetValue(OptionStyleProperty, value);
            }
        }
        #endregion


        #region OptionBorderBrush
        public static readonly DependencyProperty OptionBorderBrushProperty = DependencyProperty.Register("OptionBorderBrush", typeof(Brush), typeof(CheckBoxControlArray), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 201, 255))));
        public Brush OptionBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(OptionBorderBrushProperty);
            }
            set
            {
                SetValue(OptionBorderBrushProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declaration
        bool bInit;
        bool bLoaded;
        bool isTemplateApplied;
        ScreenDocument Document;
        IStringEditorManager stringManager;
        IScreenManager screenManager;
        IUFProjectManager iUFProjectManager;
        IDictionary<String, String> stringlist = new Dictionary<String, String>();
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.CheckBoxControlArray);
        WrapPanel wrapPanel;
        ScrollViewer scrollViewer;
        StackPanel stackPanel;
        MonitoredItemViewModel monitoredItemViewModel;
        bool bDesign;
        bool bAlreadyDispatched;
        bool bUpdateValueAlreadyDispatched;
        DispatcherOperation dpUpdateValue;
        List<Action> ExecuteOnInit = new List<Action>();
        Grid mainGrid;

        AuditTraceViewModel auditTraceViewModel;

        object lockObject = new object();
        #endregion

        #region ctor
        public CheckBoxControlArray()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if(!bLoaded && !bDisposed)
                {
                    bLoaded = true;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

                    bInit = true;

                    foreach (Action a in ExecuteOnInit) { a.Invoke(); }
                    ExecuteOnInit.Clear();

                    if (Document != null)
                    {
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                    }

                    if (stringManager != null)
                    {
                        StringManager_CultureChanged(Document, null);
                        stringManager.CultureChanged += StringManager_CultureChanged;
                    }
                }
            };
            DataContextChanged += (o, e) =>
            {
                if (bDisposed)
                    return;

                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this))
                {
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel.ReferenceViewModel != null)
                        monitoredItemViewModel = monitoredItemViewModel.ReferenceViewModel;
                    if (monitoredItemViewModel != null)
                    {
                        if (Document == null)
                            Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                        var auditTraceMonitoredItemViewModel = DataContext as MonitoredItemViewModel;
                        if (!RunningOnServer && Document != null && auditTraceMonitoredItemViewModel.monitoredItem != null)
                        {
                            if (auditTraceViewModel != null)
                            {
                                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                                auditTraceViewModel.Dispose();
                                auditTraceViewModel = null;
                            }

                            var name = Document.GetEntityName(this, bAdd: false);
                            if (!String.IsNullOrEmpty(name) && Document.MapScreenEntities.ContainsKey(name))
                            {
                                var entity = Document.MapScreenEntities[name];
                                auditTraceViewModel = new AuditTraceViewModel(auditTraceMonitoredItemViewModel, entity, Document, Document.SessionString)
                                {
                                    Control = entity.Element
                                };
                                auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                                ChangeIsEnabledProperty(false);
                            }
                        }

                        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                        var updateValueAction = new Action(() =>
                        {
                            UpdateValue();
                            bUpdateValueAlreadyDispatched = false;
                        });
                        InvokeWhenLoaded(updateValueAction, ref bUpdateValueAlreadyDispatched);
                    }
                }
            };
        }

        void InvokeWhenLoaded(Action action, ref bool bIsDispatched)
        {
            if (bLoaded)
                action.Invoke();
            else if (!bIsDispatched)
            {
                bIsDispatched = true;
                ExecuteOnInit.Add(action);
            }
        }

        void OnAuditFetched(object s, EventArgs ev)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                ChangeIsEnabledProperty(true);
                var entity = auditTraceViewModel?.Entity as ScreenSettings.Entities.ScreenEntity;
                if (entity != null)
                    entity.ReexecuteEnable();
            });
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainGrid = VisualTreeHelper.GetChild(this, 0) as Grid;

            if (mainGrid != null && !isTemplateApplied)
            {
                InitPanelLayout();
                UpdateControlLayout();
                isTemplateApplied = true;
            }
        }

        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                UpdateValue();
            }
        }

        bool bChangingValue;
        UInt64? lastValue;
        private void UpdateValue()
        {
            if (monitoredItemViewModel == null)
                return;

            UInt64 value;
            if (!UInt64.TryParse(monitoredItemViewModel.Value, out value))
            {
                Int64 intValue;
                if (Int64.TryParse(monitoredItemViewModel.Value, out intValue))
                    value = unchecked((uint)intValue);
            }

            bool bForceDispatcherOperation = false;
            lock (lockObject)
            {
                if (lastValue == value)
                    return;

                bForceDispatcherOperation = !lastValue.HasValue;
                lastValue = value;
            }

            if (dpUpdateValue == null || bForceDispatcherOperation ||
                dpUpdateValue.Status == DispatcherOperationStatus.Completed ||
                dpUpdateValue.Status == DispatcherOperationStatus.Aborted)
            {
                dpUpdateValue = Dispatcher.BeginInvokeAsynchronously(this, () =>
                {
                    if (bDisposed)
                        return;

                    UInt64 newValue;
                    lock (lockObject)
                    {
                        newValue = lastValue.Value;
                        lastValue = null;
                    }

                    Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
                    try
                    {
                        bChangingValue = true;
                        foreach (var c in panel.Children.Cast<CheckBoxControl>())
                        {
                            int index = (int)c.Tag;
                            var checkBit = (UInt64)Math.Pow(2, (double)index);
                            c.IsChecked = (newValue & checkBit) > 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(Properties.Resources.RuntimeError,ex);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, Properties.Resources.CheckBoxControlArray, DateTime.UtcNow, $"{Properties.Resources.RuntimeError}: {ex.Message}", System.Diagnostics.EventLogEntryType.Error);
                    }
                    finally
                    {
                        bChangingValue = false;
                    }
                });
            }
        }
        #endregion

        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            list.AddRange(GetContentList());
            return list;
        }
        private List<string> GetContentList()
        {
            var list = new List<string>();
            Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
            if (panel == null)
                return list;
            foreach (var c in panel.Children.Cast<CheckBoxControl>())
            {
                if (c.Content is string)
                    list.Add(c.Content as string);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            for (int i = 0; i < ControlArray; i++)
            {
                var _content = $"{ContentStrings} {string.Format(PostFixFormat, i)}";
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(CheckBoxControlArray), ContentStringsProperty).DisplayName;
                var _property = $"{propertyName} {i}";
                map.Add(_property, _content);
            }
            return map;
        }
        #endregion

        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesign && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                else
                    stringlist = null;
                if(bInit)
                    UpdateContent();
            });
        }
        void InitPanelLayout()
        {
            wrapPanel = new WrapPanel();
            stackPanel = new StackPanel();
            wrapPanel.Orientation = ArrayOrientation;
            stackPanel.Orientation = ArrayOrientation;
            scrollViewer = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            scrollViewer.Content = stackPanel;

            if (UseWrapPanel)
                mainGrid.Children.Add(wrapPanel);
            else
                mainGrid.Children.Add(scrollViewer);
        }
        void UpdatePanelLayout()
        {
            if (mainGrid == null)
                return;
            mainGrid.Children.Clear();
            if (UseWrapPanel)
            {
                RemoveHandles(stackPanel);
                stackPanel.Children.Clear();
                mainGrid.Children.Add(wrapPanel);
            }
            else
            {
                RemoveHandles(wrapPanel);
                wrapPanel.Children.Clear();
                mainGrid.Children.Add(scrollViewer);
            }
        }
        void ChangeContentLayout()
        {
            UpdatePanelLayout();
            UpdateControlLayout();
        }
        private void UpdateControlLayout()
        {
            if (ControlArray <= 0)
                ControlArray = 1;

            Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
            if (panel == null)
                return;
            RemoveHandles(panel);
            panel.Children.Clear();
            var delta = ControlArray - panel.Children.Count;
            for (int i = 0; i < ControlArray; i++)
            {
                var _content = $"{ContentStrings} {string.Format(PostFixFormat, i)}";
                _content = TranslationHelpers.TranslationHelper.TranslateComposedText(_content, stringlist, _content); 
                CheckBoxControl checkbox = new CheckBoxControl()
                {
                    Tag = i,
                    Margin = CheckBoxMargin,
                    Content = _content,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Style = FindResource(OptionStyle) as Style
                };
                checkbox.ClearValue(CheckBoxControl.WidthProperty);
                checkbox.ClearValue(CheckBoxControl.HeightProperty);
                var OptionBorderBrushBinding = new Binding()
                {
                    Path = new PropertyPath("OptionBorderBrush"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                };
                var heightBinding = new Binding()
                {
                    Path = new PropertyPath("BulletDimension"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                };
                var borderBrushBinding = new Binding()
                {
                    Path = new PropertyPath("BorderBrush"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                };
                var backgroundBinding = new Binding()
                {
                    Path = new PropertyPath("Background"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                };
                var foregroundBinding = new Binding()
                {
                    Path = new PropertyPath("Foreground"),
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                };

                checkbox.SetBinding(CheckBoxControl.OptionBorderBrushProperty, OptionBorderBrushBinding);
                checkbox.SetBinding(CheckBox.ForegroundProperty, foregroundBinding);
                checkbox.SetBinding(CheckBox.BackgroundProperty, backgroundBinding);
                checkbox.SetBinding(CheckBox.BorderBrushProperty, borderBrushBinding);
                checkbox.SetBinding(CheckBox.HeightProperty, heightBinding);
                checkbox.Checked += checkbox_statusChanged;
                checkbox.Unchecked += checkbox_statusChanged;
                panel.Children.Add(checkbox);
            }
        }
        private void checkbox_statusChanged(object sender, RoutedEventArgs e)
        {
            
            if (monitoredItemViewModel == null || bChangingValue)
                return;

            UInt64 value;
            if (!UInt64.TryParse(monitoredItemViewModel.Value, out value))
            {
                Int64 intValue;
                if (Int64.TryParse(monitoredItemViewModel.Value, out intValue))
                    value = unchecked((uint)intValue);
            }

            var checkBox = sender as CheckBoxControl;
            try
            {
                int index = (int)checkBox.Tag;
                UInt64 checkBit = (UInt64)Math.Pow(2, (double)index);
                value = (bool)checkBox.IsChecked ? value | checkBit : value & (UInt64.MaxValue - checkBit);
                lock (lockObject)
                {
                    lastValue = value;
                }
                if (auditTraceViewModel != null && auditTraceViewModel.IsAuditTraceEnabled)
                {
                    if (!auditTraceViewModel.SetValue(value.ToString()))
                    {
                        try
                        {
                            bChangingValue = true;
                            checkBox.IsChecked = !checkBox.IsChecked;
                        }
                        finally
                        {
                            bChangingValue = false;
                        }
                    }
                }
                else
                    monitoredItemViewModel.Value = value.ToString();
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.RuntimeError, ex);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.CheckBoxControlArray, DateTime.UtcNow, $"{Properties.Resources.RuntimeError}: {ex.Message}", System.Diagnostics.EventLogEntryType.Error);
            }

        }

        private void UpdateContent(bool bNewStringID = false)
        {
            Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
            if (panel == null)
                return;
            foreach (var c in panel.Children.Cast<CheckBoxControl>())
            {
                var postfix = string.Format(PostFixFormat, c.Tag);
                var _content = $"{ContentStrings}";
                if (!String.IsNullOrEmpty(postfix))
                    _content = String.Format("{0} {1}", _content, postfix);

                if (bNewStringID && Document != null)
                {
                    c.Content = _content;
                    if (screenManager == null)
                        screenManager = Document.GetService(typeof(IScreenManager)) as IScreenManager;
                    screenManager.UpdateStringId(c, true, true, false);
                }
                else
                    c.Content = TranslationHelpers.TranslationHelper.TranslateComposedText(_content, stringlist, _content); 
            }
        }

        private void UpdateMargin()
        {
            Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
            if (panel == null)
                return; 
            foreach (var c in panel.Children.Cast<CheckBoxControl>())
            {
                c.Margin = CheckBoxMargin;
            }
        }

        private void ChangeIsEnabledProperty(bool newValue)
        {
            Action editPanelAction = new Action(() => {
                Panel panel = UseWrapPanel ? wrapPanel as Panel : stackPanel as Panel;
                if (panel == null)
                    return;
                foreach (var c in panel.Children.Cast<CheckBoxControl>())
                    c.IsEnabled = newValue;
                bAlreadyDispatched = false;
            });
            InvokeWhenLoaded(editPanelAction, ref bAlreadyDispatched);
        }

        void RemoveHandles(Panel panel)
        {
            panel.Children.Cast<CheckBoxControl>().ToList().ForEach(c =>
            {
                c.Checked -= checkbox_statusChanged;
                c.Unchecked -= checkbox_statusChanged;
            });
        }
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (auditTraceViewModel != null)
            {
                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                auditTraceViewModel.Dispose();
                auditTraceViewModel = null;
            }

            if (dpUpdateValue != null &&
               dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
               dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                dpUpdateValue.Abort();
            if(scrollViewer != null)
                scrollViewer.Content = null;
            if (stackPanel != null)
            {
                RemoveHandles(stackPanel);
                stackPanel.Children.Clear();
            }
            if (wrapPanel != null)
            {
                RemoveHandles(wrapPanel);
                wrapPanel.Children.Clear();
            }
                
            if (mainGrid != null)
                mainGrid.Children.Clear();
            stackPanel = null;
            wrapPanel = null;
            scrollViewer = null;
            mainGrid = null;
        }

        #region Properties
        [Browsable(false)]
        public bool RunningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
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

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 1.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ControlArrayProperty, dt);
                return mapDataTemplates;
            }
        }

        #endregion        
    }
}
