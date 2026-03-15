using DocumentManager.ComponentService;
using OPCUAViewModel;
using ScreenSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;
using WPFUtilities;
using log4net;
using UFInterfaces;
using DynamicTagAwareHelper;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using UFUAEditor.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Globalization;
using DevExpress.Xpf.Docking;
using System.IO;
using DataLoggerColumnListControl;
using WPFPenHelpers;
using static StatesChartControl.Helpers.TimeSamplingParams;
using UIMsgBoxAlertService.ComponentService;
using StringManager.ComponentService;
using TranslationHelpers;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using StatesChartControl.Helpers;
using System.Windows.Shapes;
using System.Text;
using EditSettingsHelper.ComponentService;
using ViewModelLib;
using Utilities.WPF;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Runtime.Serialization;
using EditSettingsHelper;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using StorageHelper;
using DevExpress.Xpf.Bars;

namespace StatesChartControl
{
    /// <summary>
    /// Interaction logic for StatesChartControl.xaml
    /// </summary>
    /// 
    public partial class StatesChartControl : UserControl, IDynamicTagAware, IDisposable, IEntityReference, INotifyPropertyChanged, IContainPropertyEditors, ISettingsHelper, IConnectionAware
    , IStringIDAware, IGridLayoutUser
    {
        #region Declarations
        bool bLoaded, bDesign, bDisposed;
        internal bool bSmartSettingsEditing;
        bool templateApplied;
        bool isRealTime;
        ulong initTimeStamp;
        //protected double singleValueColumnWidth=40;
        int penLegendWidth = 60;
        int actualSamplingUnitWidth;
        double availablePlotGridWidth;
        double timeAxisLabelDistance = 65;
        double plotgridLastWidth;
        int timeLabelscolspan;
        DelayedSingleActionInvoker SizeChangedInvoker;
        DelayedSingleActionInvoker VisibilityChangedInvoker;
        IDocument Document;
        IUFUAEditorManager UFUAEditor;
        bool bInit;
        bool customPensHeight;
        bool bShowError;
        TypeHelper typeHelper = new TypeHelper();
        Dictionary<String, DataLoggerSettings> dlrSettings;
        IDictionary<String, String> MapToHistoricalConnectsions;
        IDictionary<String, String> MapToDatalogerConnectsions;
        IDictionary<String, String> stringlist;
        Double[] allTimestampValues = { };
        TagPenList actualPensList;
        TagPenList penReferenceList;
        Dictionary<string, SCDataGenerator> viewList = new Dictionary<string, SCDataGenerator>();
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IUFProjectManager iUFProjectManager;
        Dictionary<string, GenericSeries> mapKeySeries = new Dictionary<string, GenericSeries>();
        GenericSeries dataContextSerie;
        //DispatcherTimer timer;
        SettingsStorage settingStorage;
        bool bAddMonitored;
        string monitoredKey;
        SCDataGenerator viewDataContext;
        MonitoredItemViewModel monitoredItemViewModel;
        private static Random random;
        DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;
        double defaultPensHeight = 60;
        double defaultSeparatorHeight = 30;
        string monitored_nodeid;
        PenList monitoredPenList;
        object lockObject = new Object();
        List<Rectangle> zoomableCells;
        int maxChartDays = 190;
        Dictionary<string, PenDataStatus> SeriesHistoryLoaded_WaitingPens;
        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        int maxMemoryDataCount = 5000;
        TimeSpan viewTimeFrame = dateSpanViewTimeFrame[RDateSpan.Hour];
        TimeSpan recordEvery = samplingUnit[RDateSpan.Hour];
        TimeSpan minRecordEvery = samplingUnit[RDateSpan.Minute];
        TimeSpan refreshInterval = new TimeSpan(0, 0, 0, 0);
        TimeSpan minRefreshInterval = new TimeSpan(0, 0, 20);
        DispatcherTimer refreshTimer;
        IStringEditorManager stringManager;
        int penLabelRightMargin = 5;

        Dictionary<string, PenItemHelper> mapHandlers;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.SCControlLog);

        public enum PenDataStatus
        {
            DataWaiting = 1,
            DataLoaded = 2,
            EmptyDataLoaded = 3,
            PresentDataLoaded = 4
        }
        Helper helper;
        Setting defSetting;
        string resetDockLayout;
        string resetGridLayout;
        DateTime lastDrawnDateStart;
        DateTime lastDrawnDateEnd;
        #endregion

        #region DP

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        BarEditItem TimeFilterCombo
        {
            get
            {
                return RunningOnServer ? cmbTimeRange_web : cmbTimeRange;
            }
        }
        //#region DefaultArrayIndex
        //public static readonly DependencyProperty ArrayIndexProperty = DependencyProperty.Register("DefaultArrayIndex", typeof(int), typeof(StatesChartControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnArrayIndexChanged), new CoerceValueCallback(OnCoerceArrayIndex)));

        //private static object OnCoerceArrayIndex(DependencyObject o, object value)
        //{
        //    StatesChartControl control = o as StatesChartControl;
        //    if (control != null)
        //        return control.OnCoerceArrayIndex((int)value);
        //    else
        //        return value;
        //}

        //private static void OnArrayIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    StatesChartControl control = o as StatesChartControl;
        //    if (control != null)
        //        control.OnArrayIndexChanged((int)e.OldValue, (int)e.NewValue);
        //}

        //protected virtual int OnCoerceArrayIndex(int value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnArrayIndexChanged(int oldValue, int newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //int defaultArrayIndex;
        //[Category("ChartOptions")]
        //public int DefaultArrayIndex
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (int)GetValue(ArrayIndexProperty);
        //    }
        //    set
        //    {
        //        SetValue(ArrayIndexProperty, value);
        //        defaultArrayIndex = value;
        //    }
        //}
        //#endregion
        #region DefToolbarHeight
        public static readonly DependencyProperty DefToolbarHeightProperty = DependencyProperty.Register("DefToolbarHeight", typeof(double), typeof(StatesChartControl), new UIPropertyMetadata(25d));
        [Browsable(false)]
        [XmlIgnore]
        public double DefToolbarHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(DefToolbarHeightProperty);
            }
            set
            {
                SetValue(DefToolbarHeightProperty, value);
            }
        }
        #endregion
        #region AutoRefresh
        public static readonly DependencyProperty AutoRefreshProperty = DependencyProperty.Register("AutoRefresh", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoRefreshChanged), new CoerceValueCallback(OnCoerceAutoRefresh)));

        private static object OnCoerceAutoRefresh(DependencyObject o, object value)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                return StatesChartControl.OnCoerceAutoRefresh((bool)value);
            else
                return value;
        }

        private static void OnAutoRefreshChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                StatesChartControl.OnAutoRefreshChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutoRefresh(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoRefreshChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateRefreshInterval();
        }

        [Category("ChartOptions")]
        public bool AutoRefresh
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutoRefreshProperty);
            }
            set
            {
                SetValue(AutoRefreshProperty, value);
            }
        }
        #endregion
        #region PensHeight
        //IF at least one between PensHeight and SeparatorHeight is set, the other picks the default value
        public static readonly DependencyProperty PensHeightProperty = DependencyProperty.Register("PensHeight", typeof(double), typeof(StatesChartControl), new UIPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnPensHeightChanged), new CoerceValueCallback(OnCoercePensHeight)));

        private static object OnCoercePensHeight(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoercePensHeight((double)value);
            else
                return value;
        }

        private static void OnPensHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                control.OnPensHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoercePensHeight(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            pensHeight = value < 0 ? 0 : value;
            return pensHeight;
        }

        protected virtual void OnPensHeightChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                pensHeight = newValue;
                if (Double.IsNaN(newValue))
                {
                    customPensHeight = false;
                    plotVerticalScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    PensHeight = Double.NaN;
                }
                else if (Double.IsNaN(oldValue))
                {
                    customPensHeight = true;
                    plotVerticalScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    if (Double.IsNaN(SeparatorHeight))
                        SeparatorHeight = defaultSeparatorHeight;
                }
                RedrawPlotGrid(false, true);
            }
        }
        double pensHeight;
        [Category("ChartOptions")]
        public double PensHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(PensHeightProperty);
            }
            set
            {
                SetValue(PensHeightProperty, value);
                pensHeight = value;
            }
        }

        #endregion
        #region SeparatorHeight
        //IF at least one between PensHeight and SeparatorHeight is set, the other picks the default value
        public static readonly DependencyProperty SeparatorHeightProperty = DependencyProperty.Register("SeparatorHeight", typeof(double), typeof(StatesChartControl), new UIPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnSeparatorHeightChanged), new CoerceValueCallback(OnCoerceSeparatorHeight)));

        private static object OnCoerceSeparatorHeight(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceSeparatorHeight((double)value);
            else
                return value;
        }

        private static void OnSeparatorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                control.OnSeparatorHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceSeparatorHeight(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            separatorHeight = value < 0 ? 0 : value;
            return separatorHeight;
        }

        protected virtual void OnSeparatorHeightChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                separatorHeight = newValue;
                if (Double.IsNaN(newValue))
                {
                    customPensHeight = false;
                    plotVerticalScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    SeparatorHeight = Double.NaN;
                }
                else if (Double.IsNaN(oldValue))
                {
                    customPensHeight = true;
                    plotVerticalScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    if (Double.IsNaN(PensHeight))
                        PensHeight = defaultPensHeight;
                }
                RedrawPlotGrid(false, true);
            }
        }
        double separatorHeight;
        [Category("ChartOptions")]
        public double SeparatorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(SeparatorHeightProperty);
            }
            set
            {
                SetValue(SeparatorHeightProperty, value);
                separatorHeight = value;
            }
        }

        #endregion
        #region PenListProperty
        public static readonly DependencyProperty PenListProperty = DependencyProperty.Register("TPenList", typeof(TagPenList), typeof(StatesChartControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTPenListChanged), new CoerceValueCallback(OnCoerceTPenList)));

        private static object OnCoerceTPenList(DependencyObject o, object value)
        {
            StatesChartControl statesChartControl = o as StatesChartControl;
            if (statesChartControl != null)
                return statesChartControl.OnCoerceTPenList((TagPenList)value);
            else
                return value;
        }

        private static void OnTPenListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl statesChartControl = o as StatesChartControl;
            if (statesChartControl != null)
                statesChartControl.OnTPenListChanged((TagPenList)e.OldValue, (TagPenList)e.NewValue);
        }

        protected virtual TagPenList OnCoerceTPenList(TagPenList value)
        {
            return value;
        }

        protected virtual void OnTPenListChanged(TagPenList oldValue, TagPenList newValue)
        {
            penReferenceList = actualPensList = null;
            ddm?.SetLegendSource(PenReferenceList);
        }
        #endregion
        #region AutoHideToolbar
        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            StatesChartControl statesChartControl = o as StatesChartControl;
            if (statesChartControl != null)
                return statesChartControl.OnCoerceAutoHideToolbar((bool)value);
            else
                return value;
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl statesChartControl = o as StatesChartControl;
            if (statesChartControl != null)
                statesChartControl.OnAutoHideToolbarChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutoHideToolbar(bool value)
        {
            return value;
        }

        protected virtual void OnAutoHideToolbarChanged(bool oldValue, bool newValue)
        {
            if (bDesign && bInit && newValue != oldValue)
            {
                if (newValue)
                {
                    toolbar_MouseLeave(null, null);
                    Grid.SetRow(maybeLayoutManagerGrid, 0);
                    Grid.SetRowSpan(maybeLayoutManagerGrid, 2);
                }
                else
                {
                    toolbar_MouseEnter(null, null);
                    Grid.SetRow(maybeLayoutManagerGrid, 1);
                    Grid.SetRowSpan(maybeLayoutManagerGrid, 1);
                }
            }
        }

        public bool AutoHideToolbar
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutoHideToolbarProperty);
            }
            set
            {
                SetValue(AutoHideToolbarProperty, value);
            }
        }
        #endregion
        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(RDateSpan), typeof(StatesChartControl), new UIPropertyMetadata(RDateSpan.Hour, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceFilterType((RDateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                control.OnFilterTypeChanged((RDateSpan)e.OldValue, (RDateSpan)e.NewValue);
        }

        protected virtual RDateSpan OnCoerceFilterType(RDateSpan value)
        {
            return value;
        }

        protected virtual void OnFilterTypeChanged(RDateSpan oldValue, RDateSpan newValue)
        {
            if (bInit && !avoidFilterTypeChange)
                ManageTimeRange(newValue);
        }

        [Category("ChartOptions")]
        public RDateSpan FilterType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (RDateSpan)GetValue(FilterTypeProperty);
            }
            set
            {
                SetValue(FilterTypeProperty, value);
            }
        }

        #endregion
        #region UseAbsoluteRanges
        public static readonly DependencyProperty UseAbsoluteRangesProperty = DependencyProperty.Register("UseAbsoluteRanges", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseAbsoluteRangesChanged), new CoerceValueCallback(OnCoerceUseAbsoluteRanges)));

        private static object OnCoerceUseAbsoluteRanges(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceUseAbsoluteRanges((bool)value);
            else
                return value;
        }

        private static void OnUseAbsoluteRangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                control.OnUseAbsoluteRangesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseAbsoluteRanges(bool value)
        {
            return value;
        }

        protected virtual void OnUseAbsoluteRangesChanged(bool oldValue, bool newValue)
        {
            if (UseAbsoluteRangesEnabled && bInit)
                ManageTimeRange(FilterType);
        }
        [Category("ChartOptions")]
        public bool UseAbsoluteRanges
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseAbsoluteRangesProperty);
            }
            set
            {
                SetValue(UseAbsoluteRangesProperty, value);
            }
        }
        #endregion
        #region SampleNumber
        public static readonly DependencyProperty SampleNumberProperty = DependencyProperty.Register("SampleNumber", typeof(int), typeof(StatesChartControl), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnSampleNumberChanged), new CoerceValueCallback(OnCoerceSampleNumber)));

        private static object OnCoerceSampleNumber(DependencyObject o, object value)
        {
            StatesChartControl scControl = o as StatesChartControl;
            if (scControl != null)
                return scControl.OnCoerceSampleNumber((int)value);
            else
                return value;
        }

        private static void OnSampleNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl scControl = o as StatesChartControl;
            if (scControl != null)
                scControl.OnSampleNumberChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSampleNumber(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSampleNumberChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public int SampleNumber
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SampleNumberProperty);
            }
            set
            {
                SetValue(SampleNumberProperty, value);
            }
        }
        #endregion
        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(StatesChartControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            StatesChartControl chart = o as StatesChartControl;
            if (chart != null)
                return chart.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl chart = o as StatesChartControl;
            if (chart != null)
                chart.OnConnectionStringChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionString(String value)
        {
            return value;
        }

        protected virtual void OnConnectionStringChanged(String oldValue, String newValue)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesign && bInit && !string.IsNullOrEmpty(newValue))
            {
                try
                {
                    if (viewList != null)
                        (from v in viewList.Values select v).ToList().ForEach(x => x.UpdateConnection(newValue));
                    if (viewDataContext != null)
                        viewDataContext.UpdateConnection(newValue);
                }
                catch (Exception)
                {
                }
            }
        }
        [Category("ChartOptions")]
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
        #region ShowRowLabels
        public static readonly DependencyProperty ShowRowLabelsProperty = DependencyProperty.Register("ShowRowLabels", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(true, OnShowRowLabelsChanged));

        private static void OnShowRowLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = d as StatesChartControl;
            if (control != null)
                control.OnShowRowLabelsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnShowRowLabelsChanged(bool oldValue, bool newValue)
        {
            labelsGrid.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            plotGrid.Margin = newValue ? new Thickness(0) : new Thickness(5, 0, 0, 0);
            RedrawPlotGrid(false, newValue);
        }
        [Category("ChartOptions")]
        public bool ShowRowLabels
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowRowLabelsProperty);
            }
            set
            {
                SetValue(ShowRowLabelsProperty, value);
            }
        }
        #endregion
        #region ShowTooltips
        public static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.Register("ShowTooltips", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(true, OnShowTooltipsChanged));

        private static void OnShowTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = d as StatesChartControl;
            if (control != null)
                control.OnShowTooltipsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnShowTooltipsChanged(bool oldValue, bool newValue)
        {
            RedrawPlotGrid();
        }
        [Category("ChartOptions")]
        public bool ShowTooltips
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowTooltipsProperty);
            }
            set
            {
                SetValue(ShowTooltipsProperty, value);
            }
        }
        #endregion
        #region LabelForegroundBrush
        public static readonly DependencyProperty LabelForegroundBrushProperty = DependencyProperty.Register("LabelForegroundBrush", typeof(Brush), typeof(StatesChartControl), new UIPropertyMetadata(Brushes.LightGray, OnLabelForegroundBrushChanged));

        private static void OnLabelForegroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = d as StatesChartControl;
            if (control != null)
                control.OnControlLabelForegroundBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        private void OnControlLabelForegroundBrushChanged(Brush oldValue, Brush newValue)
        {
            RedrawPlotGrid(false, true);
        }

        [Category("Style")]
        public Brush LabelForegroundBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelForegroundBrushProperty);
            }
            set
            {
                SetValue(LabelForegroundBrushProperty, value);
            }
        }
        #endregion
        #region LegendAreaVisible
        public static readonly DependencyProperty LegendAreaVisibleProperty = DependencyProperty.Register("LegendAreaVisible", typeof(Boolean), typeof(StatesChartControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLegendAreaVisibleChanged), new CoerceValueCallback(OnCoerceLegendAreaVisible)));

        private static object OnCoerceLegendAreaVisible(DependencyObject o, object value)
        {
            StatesChartControl scControl = o as StatesChartControl;
            if (scControl != null)
                return scControl.OnCoerceLegendAreaVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLegendAreaVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl scControl = o as StatesChartControl;
            if (scControl != null)
                scControl.OnLegendAreaVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLegendAreaVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendAreaVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateControlLayout();
            OnPropertyChanged("DockManagerAllowed");
        }
        [Category("ChartOptions")]
        [Browsable(true)]
        public Boolean LegendAreaVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(LegendAreaVisibleProperty);
            }
            set
            {
                SetValue(LegendAreaVisibleProperty, value);
            }
        }
        #endregion
        #region LegendAreaForeground
        public static readonly DependencyProperty LegendAreaForegroundProperty = DependencyProperty.Register("LegendAreaForeground", typeof(Brush), typeof(StatesChartControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnLegendAreaForegroundChanged), new CoerceValueCallback(OnCoerceLegendAreaForeground)));

        private static object OnCoerceLegendAreaForeground(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceLegendAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnLegendAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                control.OnLegendAreaForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLegendAreaForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendAreaForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            UpdateLegendForeground();
        }
        [Category("Style")]
        public Brush LegendAreaForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LegendAreaForegroundProperty);
            }
            set
            {
                SetValue(LegendAreaForegroundProperty, value);
            }
        }

        #endregion

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(StatesChartControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("ChartOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarBackground), RequiredKey = true)]
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

        #region DockLayout
        public static readonly DependencyProperty DockLayoutProperty = DependencyProperty.Register("DockLayout", typeof(String), typeof(StatesChartControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDockLayoutChanged), new CoerceValueCallback(OnCoerceDockLayout)));

        private static object OnCoerceDockLayout(DependencyObject o, object value)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                return StatesChartControl.OnCoerceDockLayout((String)value);
            else
                return value;
        }

        private static void OnDockLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                StatesChartControl.OnDockLayoutChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceDockLayout(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDockLayoutChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            LoadDesignDockLayout();
        }

        internal void ResetDockLayout()
        {
            if (!string.IsNullOrEmpty(resetDockLayout))
                DockLayout = resetDockLayout;
        }

        void SaveResetDockLayout()
        {
            if (ddm == null)
                return;

            using (MemoryStream output = new MemoryStream())
            {
                ddm.dockManager.SaveLayoutToStream(output);
                resetDockLayout = Encoding.Default.GetString(output.ToArray());
            }
        }

        internal void SaveDesignDockLayout()
        {
            if (NotNestedInDockManager || ddm == null)
                return;
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    ddm.dockManager.SaveLayoutToStream(output);
                    DockLayout = Encoding.Default.GetString(output.ToArray());
                }
            }
            catch (Exception)
            {
            }
        }

        void LoadDesignDockLayout()
        {
            if (NotNestedInDockManager || ddm == null || string.IsNullOrEmpty(DockLayout))
                return;

            if (string.IsNullOrEmpty(resetDockLayout))
                SaveResetDockLayout();

            var dim = DockLayout.Length;
            string _mid = string.Empty;
            if (DockLayout.IndexOf('?') == 0)
            {
                _mid = DockLayout.Substring(1, dim - 1);
                SetValue(DockLayoutProperty, _mid);
                return;
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                if (!string.IsNullOrEmpty(DockLayout))
                    using (MemoryStream output = new MemoryStream(Encoding.Default.GetBytes(DockLayout)))
                    {
                        try
                        {
                            ddm.dockManager.RestoreLayoutFromStream(output);
                            List<LayoutPanel> autoHiddenPanels = new List<LayoutPanel>();
                            foreach (var group in ddm.dockManager.AutoHideGroups)
                            {
                                foreach (LayoutPanel panel in GetChildPanels(group))
                                    if (panel.AutoHidden)
                                        autoHiddenPanels.Add(panel);
                            }
                            foreach (var panel in autoHiddenPanels)
                            {
                                ddm.dockManager.BeginUpdate();
                                panel.AutoHidden = false;
                                panel.AutoHidden = true;
                                ddm.dockManager.EndUpdate();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
            }
        }

        [Browsable(false)]
        public String DockLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DockLayoutProperty);
            }
            set
            {
                SetValue(DockLayoutProperty, value);
            }
        }
        #endregion

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(StatesChartControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            StatesChartControl dataAnalisys = o as StatesChartControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl dataAnalisys = o as StatesChartControl;
            if (dataAnalisys != null)
                dataAnalisys.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceGridLayout(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGridLayoutChanged(String oldValue, String newValue)
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
            if (NotNestedInDockManager || ddm == null)
                return;
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    ddm.legendListBox.SaveLayoutToStream(output);
                    resetGridLayout = utf8noBOM.GetString(output.ToArray());
                }
            }
            catch { }
        }
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertGridLayout))]
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

        internal void SaveDesignGridLayout()
        {
            if (NotNestedInDockManager || ddm == null)
                return;

            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    ddm.legendListBox.SaveLayoutToStream(output);
                    GridLayout = utf8noBOM.GetString(output.ToArray());
                }
            }
            catch (Exception)
            {
            }
        }

        void LoadDesignGridLayout()
        {
            if (string.IsNullOrEmpty(GridLayout) || NotNestedInDockManager || ddm == null)
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
                            ddm.legendListBox.RestoreLayoutFromStream(output);
                        }
                        catch (Exception ex)
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

        #endregion
        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
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
            if (newValue != oldValue)
                btnExpand.IsVisible = newValue;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(StatesChartControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(StatesChartControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            StatesChartControl control = o as StatesChartControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl control = o as StatesChartControl;
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
        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(StatesChartControl), new UIPropertyMetadata(30));
        public int CommandTimeout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(CommandTimeoutProperty);
            }
            set
            {
                SetValue(CommandTimeoutProperty, value);
            }
        }
        #endregion
        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                return StatesChartControl.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StatesChartControl StatesChartControl = o as StatesChartControl;
            if (StatesChartControl != null)
                StatesChartControl.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region Properties
        #region SmartControl
        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(StatesChartControl), new UIPropertyMetadata(false));

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
        #region MaxDataCount
        private double MaxDataCount //Max number of columns in the current viewframe
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return viewTimeFrame.TotalMilliseconds / recordEvery.TotalMilliseconds;
            }
        }
        #endregion
        #region RunningOnServer
        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion
        #region ClientTimezoneOffset
        [Browsable(false)]
        TimeSpan ClientTimezoneOffset
        {
            get
            {
                try
                {
                    if (RunningOnServer)
                        return TimeSpan.FromMinutes(ScreenDocument.GetClientTimezoneOffset(this));
                    else
                        return TimeSpan.Zero;
                }
                catch
                {
                    return TimeSpan.Zero;
                }
            }
        }
        #endregion
        #region PenReferenceList
        private TagPenList PenReferenceList
        {
            get
            {
                if (penReferenceList == null)
                {
                    penReferenceList = new TagPenList();
                    if (TPenList != null)
                    {
                        (from c in TPenList where c.tagReference != null && c.IsVisible select c).ToList().ForEach(c => penReferenceList.Add(c));
                    }
                }

                return penReferenceList;
            }
        }
        #endregion
        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion
        #region OpcuaEntityReference
        Dictionary<string, OPCUAEntityReference> opcuaEntityReference;
        private Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    for (int i = 0; i < PenReferenceList.Count(); i++)
                    {
                        opcuaEntityReference[PenReferenceList[i].NodeId] = PenReferenceList[i].tagReference;
                    }
                }
                return opcuaEntityReference;
            }
        }
        #endregion
        #region UseAbsoluteRangesEnabled
        bool useAbsoluteRangesEnabled = true;
        [Browsable(false)]
        public bool UseAbsoluteRangesEnabled
        {
            get
            {
                return useAbsoluteRangesEnabled;
            }
            set
            {
                if (useAbsoluteRangesEnabled != value)
                {
                    useAbsoluteRangesEnabled = value;
                    OnPropertyChanged("UseAbsoluteRangesEnabled");
                }
            }
        }
        #endregion
        #region TPenList
        private void OnPenPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            var pen = (PenList)sender;
            if (e.PropertyName == "IsVisible" && !bDisposed && !bDesign && VisibilityChangedInvoker != null)
                VisibilityChangedInvoker.BeginInvoke();
        }

        [Category("Advanced")]
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertPenItemList))]
        public TagPenList TPenList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TagPenList)GetValue(PenListProperty);
            }
            set
            {
                if (!bDisposed && !bDesign)
                {
                    if (TPenList != null)
                        foreach (var pen in TPenList)
                            pen.PropertyChanged -= OnPenPropertyChange;
                    if (value != null)    
                        foreach (var pen in value)
                            pen.PropertyChanged += OnPenPropertyChange;
                }
                SetValue(PenListProperty, value);
            }
        }
        #endregion
        #region IsDisposed
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return bDisposed;
            }
        }
        #endregion
        #region ZoomBackEnabled
        bool zoomBackEnabled = false;
        [Browsable(false)]
        public bool ZoomBackEnabled
        {
            get
            {
                return zoomBackEnabled;
            }
            set
            {
                if (zoomBackEnabled != value)
                {
                    zoomBackEnabled = value;
                    OnPropertyChanged("ZoomBackEnabled");
                }
            }
        }
        #endregion
        #region NestedInDockManager
        bool notNestedInDockManager = true;
        [Browsable(false)]
        public bool NotNestedInDockManager
        {
            get
            {
                return notNestedInDockManager;
            }
            set
            {
                if (notNestedInDockManager != value)
                {
                    notNestedInDockManager = value;
                    OnPropertyChanged("NotNestedInDockManager");
                }
            }
        }
        #endregion
        #region GridContainer
        [Browsable(false)]
        public UIElement GridContainer
        {
            get
            {
                return !NotNestedInDockManager && ddm != null ? ddm.dockManager as UIElement : mainGrid as UIElement;
            }
        }
        #endregion
        #region DockManagerAllowed
        [Browsable(false)]
        public Boolean DockManagerAllowed
        {
            get
            {
                return !LegendAreaVisible && !RunningOnServer;
            }
        }
        #endregion
        #region IContainPropertyEditors
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenDocument.GetScreenDocument(this);
                IUIMsgBoxAlertService UIInterface = null;
                IHelpProvider helpProvider = null;
                if (Document != null)
                {
                    UIInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                }

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.SmartPropertiesEditor));
                factory.SetValue(Controls.SmartPropertiesEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }
        #endregion
        #region ActualConfig
        string actualConfig = Properties.Settings.Default.DesignSettingName;
        [Browsable(false)]
        string ActualConfig
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return actualConfig;
            }
            set
            {
                if (actualConfig != value)
                {
                    actualConfig = value;
                    matchChangedMap.Clear();
                }
            }
        }
        MemorySettings MemorySettingList;
        #endregion
        #endregion

        #region ctor
        static StatesChartControl()
        {
            random = new Random();
        }

        public StatesChartControl()
        {
            InitializeComponent();

            ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbar.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;
                    if (RunningOnServer)
                    {
                        toolbar.Bars.Clear();
                        toolbar.Bars.Add(toolBarControl);
                        cmbTimeRange_web.IsVisible = true;
                        cmbTimeRange.IsVisible = false;
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif

                    if (SizeChangedInvoker == null)
                        SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                        {
                            if (!bDisposed)
                                RedrawPlotGrid();
                        });
                    SizeChanged += StatesChartControl_SizeChanged;

                    if (VisibilityChangedInvoker == null)
                        VisibilityChangedInvoker = new DelayedSingleActionInvoker(() =>
                        {
                            if (!bDisposed)
                            {
                                penReferenceList = actualPensList = null;
                                RedrawPlotGrid(true, true, true);
                            }
                        });

                    if (settingStorage == null)
                        settingStorage = new SettingsStorage();
                    if (settingStorage.ListRanges == null)
                        settingStorage.ListRanges = new List<TimeRange>();

                    bDesign = DesignerProperties.GetIsInDesignMode(this) || bDesign || bSmartSettingsEditing;

                    if (Document == null)
                        Document = ScreenDocument.GetScreenDocument(this);

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (Document != null)
                    {
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(Document, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                        else
                            InitTimeRange();
                    }

                    DefToolbarHeight = toolbar.ActualHeight;
                    if (bDesign)
                    {
                        DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                        SetTimeRange(FilterType);
                        InitChart();
                        if (!bSmartSettingsEditing)
                            mainBorder.IsHitTestVisible = false;
                        bInit = true;
                        UpdateControlLayout();
                    }
                    else
                    {
                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();

                        if (string.IsNullOrEmpty(DockLayout))
                            SaveDesignDockLayout();
                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();

                        EnsureDefaultValues();

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            LoadRuntimeLayout(GetStorageName(true));
                            GetItem(ActualConfig);
                        }

                        initTimeStamp = ulong.Parse(DateTime.Now.ToString("yyyyMMddHHmmssffff"));
                        InitServerDocument();
                        EnableNextPrev(true);
                        //if (bAddMonitored)
                        //{
                        //    bAddMonitored = false;
                        //    if (monitoredItemViewModel != null)
                        //        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                        //    InitMonitored();
                        //}
                        InitPanels();
                        if (RunningOnServer)
                        {
                            AutoHideToolbar = false;
                            configMemory_Toolbar.IsVisible = false;
                        }
                        UpdateControlLayout(true);
                        if (RunningOnServer)
                        {
                            if (ddm != null && !NotNestedInDockManager)
                            {
                                //ddm.toolbarPrint.Visibility = Visibility.Collapsed;
                                ddm.HideAllHidden();
                            }
                            SetWebAsset();
                        }
                        SetTimeRange(FilterType);
                        InitChart();
                        LoadRealData(true);
                        bInit = true;
                    }
                }
            };
            //DataContextChanged += (s, ea) =>
            //{
            //    if (bDisposed)
            //        return;

            //    if (DataContext is MonitoredItemViewModel)
            //    {
            //        bAddMonitored = false;

            //        if (monitoredItemViewModel != null)
            //            monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;


            //        monitoredItemViewModel = DataContext as MonitoredItemViewModel;

            //        if (bInit)
            //        {
            //            InitMonitored();
            //            RedrawPlotGrid(!bDesign);
            //        }
            //        else
            //            bAddMonitored = true;
            //    }
            //};
        }
        #endregion

        #region  Methods
        private void SetWebAsset()
        {
            btnExpand.IsEnabled = false;
            if (ddm != null && !NotNestedInDockManager)
                MoveLegend();
        }
        private void MoveLegend()
        {
            if (ddm == null || NotNestedInDockManager)
                return;
            try
            {
                ScrollViewer _legend = ddm.legendScrollViewer;
                var legendHeightFactor = Math.Max(1.5, Properties.Settings.Default.WebLegendHeightFactor);
                _legend.Height = Math.Max(Properties.Settings.Default.WebLegendMinHeight, plotGridContainer.ActualHeight / legendHeightFactor);
                //_legend.Margin = new Thickness(0, _legend.Height / legendHeightFactor, 0, 0);
                _legend.FontSize = FontSize;
                _legend.VerticalAlignment = VerticalAlignment.Bottom;

                ddm.legendGrid.Children.Remove(ddm.legendScrollViewer);

                plotGridContainer.VerticalAlignment = VerticalAlignment.Top;
                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    plotGridContainer.Height = mainGrid.ActualHeight - _legend.Height;
                });
                if (!mainGrid.Children.Contains(_legend))
                    mainGrid.Children.Add(_legend);

                Grid.SetRow(_legend, 1);
            }
            catch (Exception)
            {
            }
        }

        //void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    MonitoredItemViewModel m = (MonitoredItemViewModel)sender;

        //    if (e.PropertyName == "DataValue")
        //    {
        //        if (m.DataValue != null)
        //            UpdateMonitoredValue(monitoredKey);
        //    }
        //}

        //private void InitMonitored()
        //{
        //    if (monitoredItemViewModel == null)
        //        return;

        //    List<OPCUAEntityReference> variables = null;
        //    try
        //    {
        //        variables = (from c in OpcuaEntityReference.Values
        //                     where (c.NodeIdViewModel.nodeId.ToString()) == monitoredItemViewModel.NodeIdModel.nodeId.ToString()
        //                     select c).ToList();
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    if (variables == null || variables.Count == 0)
        //    {
        //        AddMonitoredSeries();

        //        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;

        //        InitMonitoredValue();

        //        if (monitoredItemViewModel.NodeIdModel != null)
        //        {
        //            bool isArray = false;
        //            bool isBool = false;
        //            if (PenItemHelper.ParseNumericDataType(monitoredItemViewModel.NodeIdModel, out isArray, out isBool))
        //            {
        //                viewDataContext.HistoryLoaded += SeriesHistoryLoaded;
        //                viewDataContext.UpdateReferences(monitoredItemViewModel.NodeIdModel, monitoredItemViewModel.NodeIdModel.nodeId.ToString(), isArray, isBool);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        monitoredItemViewModel = null;
        //    }
        //}
        //private void AddMonitoredSeries()
        //{
        //    if (string.IsNullOrEmpty(monitoredKey))
        //        monitoredKey = Guid.NewGuid().ToString();

        //    //TODO:
        //    //1) Pen value-color associations must be defined in runtime depending on the data. For testing purposes only, we now define them statically;
        //    //2) The legend must be updated accordingly, in runtime.

        //    monitoredPenList = new PenList();
        //    monitoredPenList.FirstBackground = Brushes.Blue;
        //    ObservableCollection<ValueItem> coll = new ObservableCollection<ValueItem>(
        //        new List<ValueItem> {
        //            new ValueItem
        //            {
        //            Label = "Valore1",
        //            Value = "1",
        //            ControlBackground = Brushes.Bisque
        //            },
        //            new ValueItem
        //            {
        //            Label = "Valore2",
        //            Value = "2",
        //            ControlBackground = Brushes.Cyan
        //            }
        //        }
        //    );
        //    monitoredPenList.Valuelist = new ValueItemList(coll);
        //    foreach (ValueItem Vi in monitoredPenList.Valuelist)
        //    {
        //        if (Vi != null)
        //            monitoredPenList.DatavalueValueitemDictionary.Add(Vi);
        //    }
        //    dataContextSerie = new GenericSeries(monitoredPenList, 0); //it will be the first pen
        //    monitored_nodeid = monitoredItemViewModel.NodeIdModel.nodeId.ToString();
        //    if (viewDataContext != null)
        //    {
        //        viewDataContext.HistoryLoaded -= SeriesHistoryLoaded;
        //    }
        //    else
        //    {
        //        viewDataContext = new SCDataGenerator(this, monitored_nodeid, ClientTimezoneOffset, monitoredItemViewModel, (int)MaxDataCount, SampleNumber, maxMemoryDataCount, ConnectionString, recordEvery, viewTimeFrame, 0, useClientTimeZone: RunningOnServer);
        //    }
        //    TPenList.Add(monitoredPenList);
        //    PrepareExecution(monitored_nodeid);
        //    RedrawPlotGrid();
        //}
        //private void InitMonitoredValue()
        //{
        //    UpdateMonitoredValue(monitoredKey, monitoredItemViewModel);
        //}

        #region Isolated Storage
        String GetStorageName(bool useParent = false)
        {
            return StorageHelper.StorageHelper.GetStorageName(Document, this.Name, UserBasedRuntimeSettings ? helper.Username : null, true);
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
                        try
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(string));
                            serializer.WriteObject(writer, ActualConfig);
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

        #region IDynamicTagAware
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if(TPenList != null)
                foreach (var pen in TPenList)
                {
                    if (pen.tagReference != null /*&& pen.tagReference.IsValid*/)
                        ret.Add(pen.CreateUniqueName(pen.title, ret.Keys.ToList()), pen.tagreferenceXml);
                }
            return ret;
        }
        List<string> matchChangedMap = new List<string>();
        TagPenList matchingPenList = null;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;
            bool ret = false;

            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
            if (matchingPenList == null)
                matchingPenList = new TagPenList(TPenList);
            var penTagList = (from pen in matchingPenList where pen.tagReference != null select pen).ToList();

            foreach (var pen in penTagList)
            {
                //if (pen.tagReference != null /*&& pen.tagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesign)
                            ret = relative == pen.tagreferenceXml;
                        else if (_absolute != null)
                        {
                            if (relative == pen.tagreferenceXml)
                            {
                                string key = pen.NodeId;
                                matchChangedMap.Add(key);
                                TerminateExecution(key);
                                if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.tagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                    pen.ColuName = pen.GetRelativePath($"{_relative.RelativePath}");
                                }
                                else
                                {
                                    pen.tagReference = _absolute;
                                    opcuaEntityReference[key] = _absolute;
                                    pen.ColuName = pen.GetRelativePath($"{_absolute.RelativePath}");
                                }
                                
                                PrepareExecution(key);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (!bDesign)
                ret = matchChangedMap.Count == penTagList.Count;

            if (!bDesign && ret)
            {
                TPenList = matchingPenList;
                penReferenceList = matchingPenList;
            }
            return ret;
        }
        //Object lockObj = new object();
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            TagPenList penList = new TagPenList(TPenList);
            foreach (var pen in penList)
            {
                if (pen.tagReference != null)
                {
                    if (map.ContainsKey(pen.NodeId))
                        pen.tagreferenceXml = map[pen.NodeId];
                    else
                        pen.tagreferenceXml = typeHelper.UpdateTag(pen.tagreferenceXml, map);
                }
            }
            TPenList = penList;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreserveTagsFromMap(Dictionary<string, string> map)
        {
            var tobeupdated = typeHelper.PreserveTagsFromMap(GetMapDynamics(), map);
            UpdateMapDynamics(tobeupdated);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetConverterLabel(string label)
        {
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
        {
        }
        #endregion

        void UpdateRefreshInterval()
        {
            refreshInterval = !AutoRefresh ? new TimeSpan(0, 0, 0) : new TimeSpan(Math.Max(minRefreshInterval.Ticks, recordEvery.Ticks));
        }

        void StatesChartControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
        }

        private void InitPanels()
        {
            startText.DataContext = settingStorage;
            endText.DataContext = settingStorage;
        }

        private void DrawPenLabels(int penIndex)
        {
            TextBlock rowLegendText = new TextBlock()
            {
                Foreground = this.ReadLocalValue(LabelForegroundBrushProperty) != DependencyProperty.UnsetValue ? LabelForegroundBrush : Foreground,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, penLabelRightMargin, 0),
                Text = GetTranslation(actualPensList[penIndex].title),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = penLegendWidth - penLabelRightMargin
            };
            actualPensList[penIndex].TranslatedTitle = rowLegendText.Text;

            labelsGrid.Children.Add(rowLegendText);
            Grid.SetRow(rowLegendText, (penIndex + 1) * 2 - 1);
            Grid.SetColumn(rowLegendText, 0);
        }

        private void DrawPenValue(int penIndex, int colindex, double val, DateTime cellStartDate, TimeSpan timeToCellEnd, double secondval, Brush forceBrush = null)
        {
            Brush chosenBorderBackground = null;
            Brush secondBorderBackground = null;
            ValueItem dataAssociatedValueItem = new ValueItem();
            ValueItem secondAssociatedValueItem = new ValueItem();
            bool mainColorNotFound = false;
            bool bValIsNaN = double.IsNaN(val);
            bool bSecondvalIsNaN = double.IsNaN(secondval);
            bool multipleVals = (secondval.ToString() != val.ToString()) || (bValIsNaN != bSecondvalIsNaN);
            if (forceBrush != null)
                chosenBorderBackground = forceBrush;
            else
            {
                if (!bValIsNaN)
                    dataAssociatedValueItem = actualPensList[penIndex].Valuelist.Where(vi => vi.Value == val.ToString()).FirstOrDefault();
                if (bValIsNaN || dataAssociatedValueItem == null)
                {
                    chosenBorderBackground = actualPensList[penIndex].ValueNotFoundBackground;
                    mainColorNotFound = true;
                }
                else if (dataAssociatedValueItem != null)
                    chosenBorderBackground = dataAssociatedValueItem.ControlBackground;
            }

            if (!bSecondvalIsNaN)
                secondAssociatedValueItem = actualPensList[penIndex].Valuelist.Where(vi => vi.Value == secondval.ToString()).FirstOrDefault();
            if (bSecondvalIsNaN || secondAssociatedValueItem == null)
            {
                if (!mainColorNotFound || !multipleVals)
                    secondBorderBackground = actualPensList[penIndex].ValueNotFoundBackground;
                else // in order to highlight it, I have to choose a random color between the mapped ones (the first)
                    secondBorderBackground = actualPensList[penIndex].Valuelist[0].ControlBackground;
            }
            else if (secondAssociatedValueItem != null)
                secondBorderBackground = secondAssociatedValueItem.ControlBackground;

            Rectangle myb;
            var rowindex = (penIndex + 1) * 2 - 1;
            myb = (Rectangle)plotGrid.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == rowindex && Grid.GetColumn(e) == colindex);
            if (myb != null)
            {
                myb.MouseUp -= ZoomInCell;
                if (multipleVals && recordEvery > minRecordEvery)
                {
                    myb.Tag = new object[] { cellStartDate, timeToCellEnd };
                    zoomableCells.Add(myb);
                    myb.Cursor = Cursors.Hand;
                    myb.MouseUp += ZoomInCell;
                }
                if (!bDesign && secondBorderBackground != null && secondBorderBackground != chosenBorderBackground && secondBorderBackground as SolidColorBrush != null)
                {
                    myb.Stroke = secondBorderBackground;
                    myb.StrokeThickness = 2;
                }
                else
                    myb.Stroke = null;
                myb.Fill = chosenBorderBackground;
                if (ShowTooltips)
                {
                    string tooltipText = String.Format("{0}{1}{2}", GetTimeAxisLabelText(colindex, true), (dataAssociatedValueItem == null || String.IsNullOrEmpty(dataAssociatedValueItem.Label) ? "" : String.Format("\n{0}", GetTranslation(dataAssociatedValueItem.Label))), (bDesign ? "" : String.Format("\n{0}: {1}", Properties.Resources.ValueColumnHeader, bValIsNaN ? "NaN" : val.ToString())));
                    if (multipleVals)
                        tooltipText = String.Format("{0}\n{1}: {2}", tooltipText, Properties.Resources.SecondValueColumnHeader, bSecondvalIsNaN ? "NaN" : secondval.ToString());
                    myb.ToolTip = tooltipText;
                }
            }
        }

        void ZoomInCell(object s, EventArgs e)
        {
            Rectangle b = s as Rectangle;
            object[] dates = b.Tag as object[];
            DateTime cellStartDate = (DateTime)dates[0];
            settingStorage.OldZoomLevels.Store();
            settingStorage.DateTimeStart = cellStartDate;
            settingStorage.DateTimeEnd = cellStartDate + (TimeSpan)dates[1];
            ZoomBackEnabled = true;
            SelectRangeFromSettingsAndReload();
        }

        void ZoomOutCell(object sender, RoutedEventArgs e)
        {
            int remaining = settingStorage.OldZoomLevels.Restore();
            if (remaining == 0)
                ZoomBackEnabled = false;
            if (remaining != -1)
                SelectRangeFromSettingsAndReload();
        }

        private void DrawPenSeparator(int penIndex, bool isLastSeparator = false, bool firstTime = false)
        {
            RowDefinition gridRow;
            gridRow = new RowDefinition();
            if (isLastSeparator)
                gridRow.Height = new GridLength(3);
            else if (!customPensHeight)
                gridRow.Height = new GridLength(1, GridUnitType.Star);
            else
                gridRow.Height = new GridLength(separatorHeight);
            plotGrid.RowDefinitions.Add(gridRow);
            if (firstTime)
                labelsGrid.RowDefinitions.Add(new RowDefinition() { Height=gridRow.Height });
            
            Rectangle emptyCol = new Rectangle()
            {
                Fill = Brushes.Transparent
            };
            plotGrid.Children.Add(emptyCol);
            Grid.SetColumnSpan(emptyCol, (int)MaxDataCount);
            Grid.SetRow(emptyCol, penIndex * 2);
            Grid.SetColumn(emptyCol, 0);
        }

        private void DrawPen(int penIndex, bool firstTime = false)
        {
            RowDefinition gridRow = new RowDefinition();
            if (!customPensHeight)
                gridRow.Height = new GridLength(3, GridUnitType.Star);
            else
                gridRow.Height = new GridLength(pensHeight);
            plotGrid.RowDefinitions.Add(gridRow);
            if (firstTime)
                labelsGrid.RowDefinitions.Add(new RowDefinition() { Height = gridRow.Height });

            if (ShowRowLabels && firstTime)
            {
                DrawPenLabels(penIndex);
            }
            DrawEmptyColumns(penIndex);
            DrawPenSeparator(penIndex, false, firstTime);

            if (bDesign)
            {
                int v = penIndex % 3;
                int dataElems = 20 + (v == 0 ? 0 : v == 1 ? 5 : -5);
                Double[] actualPenData = new Double[dataElems];
                if (actualPensList[penIndex].Valuelist.Count() == 0) //create a new threshold with "valueNotFoundBackground" set to a random Brush
                {
                    if (actualPensList[penIndex].ValueNotFoundBackground == Brushes.Transparent)
                    {
                        Brush rndbrush = new SolidColorBrush(
                                Color.FromRgb((byte)random.Next(255),
                                (byte)random.Next(255),
                                (byte)random.Next(255))
                                );
                        actualPensList[penIndex].ValueNotFoundBackground = rndbrush;
                    }
                }
                for (int j = 0; j < actualPenData.Length; j++)
                {
                    DrawPenValue(penIndex, j, 0.0D, new DateTime(), new TimeSpan(), 0.0D, actualPensList[penIndex].ValueNotFoundBackground);
                }
            }
        }

        private void DrawEmptyColumns(int penIndex)
        {
            for (int i = 0; i < MaxDataCount; i++)
            {
                Rectangle cellInnerBackground = new Rectangle()
                {
                    Fill = Brushes.Transparent
                };
                plotGrid.Children.Add(cellInnerBackground);
                Grid.SetRow(cellInnerBackground, (penIndex + 1) * 2 - 1);
                Grid.SetColumn(cellInnerBackground, i);
            }
        }
        
        //private string GetRelativePath(string value)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
        //        UInt16 ns = (UInt16)(n.Count + 2 - 1);
        //        string oldChars = string.Format("{0}:", ns);
        //        string relative = string.Format("{0}", (value).Replace(oldChars, ""));
        //        return relative;
        //    }

        //    return String.Empty;
        //}

        private void InitServerDocument()
        {
            if (dlrSettings == null)
                dlrSettings = new Dictionary<string, DataLoggerSettings>();
            if (Document != null && Document.Parent != null)
            {
                if (UFUAEditor == null)
                    UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (UFUAEditor == null)
                    return;

                dlrSettings.Clear();
                var settings = UFUAEditor.GetDataLoggerSettings(Document);
                if (settings != null)
                {
                    foreach (List<String> d in settings)
                    {
                        if (!dlrSettings.ContainsKey(d[0]))
                        {
                            DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                            dlrSettings.Add(ds.Name, ds);
                        }
                    }
                }
                
                if (MapToDatalogerConnectsions == null)
                    MapToDatalogerConnectsions = new Dictionary<String, String>();
                if (MapToHistoricalConnectsions == null)
                    MapToHistoricalConnectsions = new Dictionary<String, String>();
                string sessionString = (Document as ScreenDocument).SessionString;

                List<string> dlist = new List<string>();
                var list2 = UFUAEditor.GetDataLoggerSettingsNameList(Document, bReloadDocument: false, inExecution: true);
                if (list2 != null)
                    dlist.AddRange(list2);
                dlist.ForEach(x =>
                {
                    if (!MapToDatalogerConnectsions.ContainsKey(x))
                    {
                        var conn = UFUAEditor.GetDataLoggerConnection(Document, x);
                        MapToDatalogerConnectsions.Add(x, string.IsNullOrEmpty(conn) ? string.Empty : RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, sessionString));
                    }
                });

                InitHistoricalConnections();
            }
        }

        private void InitHistoricalConnections()
        {
            foreach(var pen in PenReferenceList)
            {
                OPCUAEntityReference item = null;

                if (!string.IsNullOrEmpty(pen.tagreferenceXml))
                {
                    item = pen.tagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                if (item != null)
                {
                    var document = UFUAEditor.GetProjectDocument(Document, item.AppName);
                    if (document != null)
                    {
                        var connString = UFUAEditor.GetHistorianConnection(document, pen.HistoricalDlrName);
                        if (string.IsNullOrEmpty(connString))
                            connString = UFUAEditor.GetHistorianDefaultConnection(document);

                        MapToHistoricalConnectsions.Add(pen.NodeId, connString);
                    }
                }
            }
        }

        private void InitChart()
        {
            if (bDisposed)
                return;
            
            object bkp = this.ReadLocalValue(BackgroundProperty);
            Background = (bkp == DependencyProperty.UnsetValue || bkp == null) ? new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)) : Background;
            mainGrid.DataContext = this;
            if (zoomableCells != null)
                foreach (Rectangle b in zoomableCells)
                {
                    b.MouseUp -= ZoomInCell;
                }
            zoomableCells = new List<Rectangle>();
            SetToolbar();
            Draw(true);
        }

        DynamicDockManager ddm;
        void NestMainGridInsideDockManager(object s, EventArgs e)
        {
            //if (RunningOnServer)
            //    return;

            if ((LegendAreaVisible || bSmartSettingsEditing || (!bDesign && NotNestedInDockManager && s as Button != null)) && ddm == null)
            {
                NotNestedInDockManager = false;
                ddm = new DynamicDockManager(Document, settingStorage, stringPlaceolder, mainGrid);
                ddm.DataContext = this;
                mainGrid.Margin = new Thickness(0);
                //ddm.dockManager.Margin = new Thickness(0, !AutoHideToolbar ? 30 : 0, 0, 0);
                if (!bDesign)
                {
                    ddm.SetLegendSource(PenReferenceList);
                    ddm.DataFetchRequest += Button_FetchData;
                    ddm.LoadRangeRequest += ListBoxRecent_Selected;
                    ddm.dockManager.DockOperationCompleted += NeedsRedrawCheck;
                }
                maybeLayoutManagerGrid.Children.Clear();
                maybeLayoutManagerGrid.Children.Add(ddm);
            }
            else if (ddm != null)
            {
                NotNestedInDockManager = true;
                FrameworkElement dockedContent = ddm.GetContent() as FrameworkElement;
                //dockedContent.Margin = new Thickness(0, !AutoHideToolbar ? 30 : 0, 0, 0);
                maybeLayoutManagerGrid.Children.Clear();
                ddm.SetLegendSource(null);
                ddm.DataFetchRequest -= Button_FetchData;
                ddm.LoadRangeRequest -= ListBoxRecent_Selected;
                ddm.dockManager.DockOperationCompleted -= NeedsRedrawCheck;
                ddm.Dispose();
                ddm = null;
                maybeLayoutManagerGrid.Children.Add(dockedContent);
            }
            LoadDesignDockLayout();
            LoadDesignGridLayout();
            UpdateLegendForeground();
        }
        void UpdateLegendForeground()
        {
            if (bDisposed || ddm == null)
                return;
            ddm.LegendAreaForeground = this.ReadLocalValue(LegendAreaForegroundProperty) != DependencyProperty.UnsetValue ? LegendAreaForeground : Foreground;
        }
        DispatcherOperation dp;
        private void NeedsRedrawCheck(object sender, EventArgs e)
        {
            if (dp == null || dp.Status == DispatcherOperationStatus.Aborted || dp.Status == DispatcherOperationStatus.Completed)
                dp = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (plotGridContainer.ActualWidth != plotgridLastWidth)
                        RedrawPlotGrid(true);
                });
        }

        private void SetToolbar()
        {
            if (bDesign)
            {
                if (AutoHideToolbar)
                {
                    toolbar_MouseLeave(null, null);
                    Grid.SetRow(maybeLayoutManagerGrid, 0);
                    Grid.SetRowSpan(maybeLayoutManagerGrid, 2);
                }

                toolbar.IsEnabled = false;
                ddm?.DisableTimeGrid();
            }
            else
            {
                if (AutoHideToolbar)
                {
                    toolbar_MouseLeave(null, null);
                    Grid.SetRow(maybeLayoutManagerGrid, 0);
                    Grid.SetRowSpan(maybeLayoutManagerGrid, 2);
                    toolbar.MouseEnter += toolbar_MouseEnter;
                    toolbar.MouseLeave += toolbar_MouseLeave;
                }
            }
            //SetToolbarBckg();
        }

        private void toolbar_MouseLeave(object sender, MouseEventArgs e)
        {
            var sbLeave = TryFindResource("MouseLeaveOpacity") as Storyboard;
            sbLeave.Begin();
        }

        private void toolbar_MouseEnter(object sender, MouseEventArgs e)
        {
            var sbOver = TryFindResource("MouseOverOpacity") as Storyboard;
            sbOver.Begin();
        }

        //void SetToolbarBckg()
        //{
        //    toolbar.Background = ToolbarBackground;
        //    toolbarSettings.Background = ToolbarBackground;
        //    toolbarSettings1.Background = ToolbarBackground;
        //}

        void LoadRealData(bool forceHardReload = false)
        {
            lock (lockObject)
            {
                SeriesHistoryLoaded_WaitingPens = new Dictionary<string, PenDataStatus>();
                (from p in PenReferenceList select p.NodeId).ToList().ForEach(k => SeriesHistoryLoaded_WaitingPens.Add(k, PenDataStatus.DataWaiting));
            }
            if (!forceHardReload && viewList != null)
            {
                foreach (SCDataGenerator d in viewList.Values)
                {
                    d.Settings.DeadBandInterval = recordEvery;
                    d.Settings.DeadBandTimeFrame = viewTimeFrame;
                    d.Settings.DataCount = (int)Math.Ceiling(MaxDataCount);
                    d.CallSetDataSources(System.Threading.CancellationToken.None);
                }
            }
            else
            {
                if (forceHardReload)
                {
                    foreach (var serie in mapKeySeries.Values)
                        serie.Points.Clear();
                    mapKeySeries.Clear();

                    if (viewList != null)
                    {
                        foreach (var key in viewList.Keys.ToList())
                            if (!matchChangedMap.Contains(key))
                            {
                                viewList[key].Error -= viewList_OnError;
                                viewList[key].HistoryLoaded -= SeriesHistoryLoaded;
                                viewList[key].Dispose();
                                viewList.Remove(key);
                            }
                    }

                    settingStorage.PrevTimeFrame.Clear();

                    if (opcuaEntityReference != null)
                    {
                        foreach (var key in opcuaEntityReference.Keys.ToList())
                            if (!matchChangedMap.Contains(key))
                                TerminateExecution(key);
                        opcuaEntityReference = null;
                    }
                    if (mapHandlers != null)
                    {
                        foreach (var key in mapHandlers.Keys.ToList())
                            if (!matchChangedMap.Contains(key))
                            {
                                mapHandlers[key].Dispose();
                                mapHandlers.Remove(key);
                            }
                    }
                }

                for (int key = 0; key < PenReferenceList.Count; key++)
                {
                    string _keyname = PenReferenceList[key].NodeId;
                    mapKeySeries[_keyname] = new GenericSeries(PenReferenceList[key], dataContextSerie != null ? key + 1 : key);//the first pen is the contestual tag

                    string connectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
                    string tablename = PenReferenceList[key].HistoricalDlrName;
                    string utccolumnname = Properties.Settings.Default.UtcTimeColumnName;

                    if (PenReferenceList[key].Dlrsource)
                    {
                        var historical = PenReferenceList[key].HistoricalDlrName;
                        if (!string.IsNullOrEmpty(historical))
                        {
                            if (MapToDatalogerConnectsions != null && MapToDatalogerConnectsions.ContainsKey(historical) && !string.IsNullOrEmpty(MapToDatalogerConnectsions[historical]))
                            {
                                connectionString = MapToDatalogerConnectsions[PenReferenceList[key].HistoricalDlrName];
                            }
                            if (dlrSettings != null && dlrSettings.ContainsKey(historical))
                            {
                                tablename = string.IsNullOrEmpty(dlrSettings[historical].TableName) ? historical : dlrSettings[historical].TableName;
                                utccolumnname = string.IsNullOrEmpty(dlrSettings[historical].UtcTimeColumnName) ? Properties.Settings.Default.UtcTimeColumnName : dlrSettings[historical].UtcTimeColumnName;
                            }
                        }
                    }
                    else
                    {                        
                        connectionString = (MapToHistoricalConnectsions != null && !string.IsNullOrEmpty(PenReferenceList[key].HistoricalDlrName) && MapToHistoricalConnectsions.ContainsKey(PenReferenceList[key].NodeId) && !string.IsNullOrEmpty(MapToHistoricalConnectsions[PenReferenceList[key].NodeId])) ? MapToHistoricalConnectsions[PenReferenceList[key].NodeId] : ConnectionString;
                    }

                    if (!viewList.ContainsKey(_keyname))
                    {
                        var settings = new SCDataGeneratorSettings()
                        {
                            ConnectionString = connectionString,
                            ClientTimezoneOffset = ClientTimezoneOffset,
                            DataCount = (int)Math.Ceiling(MaxDataCount),
                            HDataCount = SampleNumber,
                            MDataCount = maxMemoryDataCount,
                            DeadBandInterval = recordEvery,
                            DeadBandTimeFrame = viewTimeFrame,
                            DlrSource = PenReferenceList[key].Dlrsource,
                            DlrName = tablename,
                            ColName = PenReferenceList[key].ColuName,
                            UtcTimeColumnName = utccolumnname,
                            Storage = settingStorage
                        };

                        viewList[_keyname] = new SCDataGenerator(_keyname, settings, CommandTimeout);
                        viewList[_keyname].Error += viewList_OnError;
                        viewList[_keyname].HistoryLoaded += SeriesHistoryLoaded;
                    }
                }

                if (OpcuaEntityReference.Count > 0)
                {
                    foreach (var key in OpcuaEntityReference.Keys)
                    {
                        if (matchChangedMap.Contains(key)) {
                            if (viewList.ContainsKey(key) && bInit)
                                viewList[key].CallSetDataSources(System.Threading.CancellationToken.None);
                        }
                        else if (!OpcuaEntityReference[key].IsRelative)
                            PrepareExecution(key);
                    }
                }
            }
            //RestartRecording();
            if (AutoRefresh)
                RestartAutoRefresh();
        }

        void RestartAutoRefresh()
        {
            if (bDisposed)
                return;
            UpdateRefreshInterval();
            StopAutoRefresh();
            if (refreshInterval.Ticks > 0)
            {
                if (refreshTimer == null)
                {
                    refreshTimer = new DispatcherTimer();
                    refreshTimer.Tick += RefreshTimer_Tick;
                    refreshTimer.Interval = refreshInterval;
                    refreshTimer.Start();
                }
                else
                {
                    if (refreshTimer.Interval != refreshInterval)
                        refreshTimer.Interval = refreshInterval;
                    refreshTimer.Start();
                }
            }
        }

        void StopAutoRefresh()
        {
            if (refreshTimer != null)
                refreshTimer.Stop();
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Ticks >= settingStorage.DateTimeEnd.Ticks) // Advancing automatically the current timeframe by 1 step
            {
                TimeSpan diff = settingStorage.DateTimeEnd - settingStorage.DateTimeStart;
                settingStorage.DateTimeEnd += diff;
                settingStorage.DateTimeStart += diff;
            }
            SetDataSources();
        }

        //private void StopRecording()
        //{
        //    if (timer != null)
        //        timer.Stop();
        //}
        //private void RestartRecording()
        //{
        //    StopRecording();
        //    if (recordEvery.Ticks > 0)
        //    {
        //        if (timer == null)
        //        {
        //            timer = new DispatcherTimer();
        //            timer.Tick += Timer_Tick;
        //            timer.Interval = recordEvery;
        //            timer.Start();
        //        }
        //        else
        //        {
        //            if (timer.Interval != recordEvery)
        //                timer.Interval = recordEvery;
        //            timer.Start();
        //        }
        //    }
        //}

        void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var key in mapHandlers.Keys)
                UpdateMonitoredValue(key);

            if (monitoredKey != null)
                UpdateMonitoredValue(monitoredKey);
        }

        private void PrepareExecution(string key)
        {
            //lock (lockObj)
            {
                if (bDisposed || !OpcuaEntityReference.ContainsKey(key))
                    return;

                if (mapHandlers == null)
                    mapHandlers = new Dictionary<string, PenItemHelper>();

                if (!OpcuaEntityReference[key].IsValid)
                    return;

                if (mapHandlers.ContainsKey(key))
                    return;

                mapHandlers[key] = new PenItemHelper(key, NotifyValueType.Never);
                mapHandlers[key].Error += PenItem_OnError;
                mapHandlers[key].ModelChanged += PenItem_ModelChanged;

                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document as ScreenDocument, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
            }
        }

        #region PenItemHelper Event Handlers
        private void PenItem_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            SetEntityError(e.ErrorMessage);
        }

        private void PenItem_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;
            var keyName = helper.Key;

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (!e.Model.IsUserReadable || !e.Model.IsReadable)
                {
                    mainBorder.IsEnabled = false;
                    mainBorder.ToolTip = string.Format("{0}: {1}", e.HumanReadable, Properties.Resources.ItemNotReadable);
                }
                else if (viewList != null && viewList.ContainsKey(keyName))
                    viewList[keyName].UpdateReferences(e.Model);
            });
        }
        #endregion
        private void TerminateExecution(string key)
        {
            //lock (lockObj)
            {
                //*************
                //set not in use
                //*************
                if (OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key] != null)
                {
                    if (mapHandlers == null)
                        mapHandlers = new Dictionary<string, PenItemHelper>();
                    if (mapHandlers.ContainsKey(key))
                    {
                        typeHelper.TerminateExecution(this, mapHandlers[key].opcuaEntityReference_PropertyChanged, null, OpcuaEntityReference[key], OpcuaEntityReference[key].MonitoredItemViewModel);

                        mapHandlers[key].Error -= PenItem_OnError;
                        mapHandlers[key].ModelChanged -= PenItem_ModelChanged;
                        mapHandlers[key].Dispose();
                        mapHandlers.Remove(key);
                    }
                }
            }
        }

        private void viewList_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            log.Error(string.Format(Properties.Resources.ErrorLoadingValues, e.ErrorMessage));
            if (iUFProjectManager != null)
                iUFProjectManager.AddLogEntity(Document, Properties.Resources.SCControlLog,
                  DateTime.UtcNow, string.Format(Properties.Resources.ErrorLoadingValues, e.ErrorMessage),
                  System.Diagnostics.EventLogEntryType.Error);
        }

        private void SeriesHistoryLoaded(object sender, EventArgs e) //Historical data has been fetched and now it must be drawn on the chart
        {
            SCDataGenerator _data = sender as SCDataGenerator;
            string key = _data.PenId;

            bool bRaiseOnLoaded = false;
            lock (lockObject)
            {
                if (SeriesHistoryLoaded_WaitingPens.ContainsKey(key))
                    SeriesHistoryLoaded_WaitingPens[key] = PenDataStatus.DataLoaded;
                if (AllPensDataLoaded(SeriesHistoryLoaded_WaitingPens) && !bControlLoaded)
                {
                    bControlLoaded = true;
                    bRaiseOnLoaded = true;
                }
            }
            if (bRaiseOnLoaded)
                OnControlLoaded();
            // //Timer_Tick(null, null);
            UpdateMonitoredValue(key/*, bRaiseOnLoaded*/);
        }

        bool AllPensDataLoaded(Dictionary<string, PenDataStatus> penStatusList)
        {
            return penStatusList != null && (from p in penStatusList where p.Value == PenDataStatus.DataWaiting select p).Count() == 0;
        }

        private void UpdateView(string k)
        {
            NeedsRedrawCheck(null, null);
            bool noPenDataFound = true;
            if (!mapKeySeries.ContainsKey(k) || !viewList.ContainsKey(k) ||
                mapKeySeries[k].Points == null || viewList[k].Values == null)
                return;

            for (int i = 0; i < mapKeySeries[k].Points.Count; i++)
            {
                if (i < viewList[k].Values.Count())
                {
                    try
                    {
                        var firstValue = Convert.ToDouble(viewList[k].Values[i].Value);
                        var secondValue = Convert.ToDouble(viewList[k].SecondValues[i].Value);
                        mapKeySeries[k].Points[i] = new GenericSeriesPoint(viewList[k].Values[i].SourceTimestamp, firstValue, secondValue);
                        if (IsInsideTimeFrame(mapKeySeries[k].Points[i].argument))
                        {
                            noPenDataFound = false;
                            DrawPenValue(mapKeySeries[k].penIndex, FindDataColIndex(mapKeySeries[k].Points[i].argument), mapKeySeries[k].Points[i].value, mapKeySeries[k].Points[i].argument, recordEvery, mapKeySeries[k].Points[i].secondvalue);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format(Properties.Resources.ErrorDrawingValues, ex.Message));
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, Properties.Resources.SCControlLog,
                              DateTime.UtcNow, string.Format(Properties.Resources.ErrorDrawingValues, ex.Message),
                              System.Diagnostics.EventLogEntryType.Error);
                    }
                }
            }
            for (int i = mapKeySeries[k].Points.Count; i < viewList[k].Values.Count(); i++)
            {
                try
                {
                    var firstValue = Convert.ToDouble(viewList[k].Values[i].Value);
                    var secondValue = Convert.ToDouble(viewList[k].SecondValues[i].Value);
                    mapKeySeries[k].Points.Add(new GenericSeriesPoint(viewList[k].Values[i].SourceTimestamp, firstValue, secondValue));
                    if (IsInsideTimeFrame(mapKeySeries[k].Points[i].argument))
                    {
                        noPenDataFound = false;
                        DrawPenValue(mapKeySeries[k].penIndex, FindDataColIndex(mapKeySeries[k].Points[i].argument), mapKeySeries[k].Points[i].value, mapKeySeries[k].Points[i].argument, recordEvery, mapKeySeries[k].Points[i].secondvalue);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format(Properties.Resources.ErrorDrawingValues, ex.Message));
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.SCControlLog,
                          DateTime.UtcNow, string.Format(Properties.Resources.ErrorDrawingValues, ex.Message),
                          System.Diagnostics.EventLogEntryType.Error);
                }
            }

            lock (lockObject)
            {
                if (SeriesHistoryLoaded_WaitingPens != null)
                {
                    SeriesHistoryLoaded_WaitingPens[k] = noPenDataFound ? PenDataStatus.EmptyDataLoaded : PenDataStatus.PresentDataLoaded;
                }
            }

            /*lock (lockObject) //if data is missing, notify user and take him back to the last period
            {
                if (SeriesHistoryLoaded_WaitingPens != null)
                {
                    SeriesHistoryLoaded_WaitingPens[k] = noPenDataFound ? PenDataStatus.EmptyDataLoaded : PenDataStatus.PresentDataLoaded;
                    bool bSomeDataFound = (from p in SeriesHistoryLoaded_WaitingPens where p.Value == PenDataStatus.PresentDataLoaded select p).Count() > 0;
                    bool bAllPensDataEmpty = (from p in SeriesHistoryLoaded_WaitingPens where p.Value != PenDataStatus.EmptyDataLoaded select p).Count() == 0;
                    if (bSomeDataFound)
                        bShowError = true;
                    if (bAllPensDataEmpty)
                    {
                        if (settingStorage.PrevTimeFrame.Count() == 0)
                            settingStorage.PrevTimeFrame.Store();
                        else if (bShowError)
                        {
                            ShowError(Properties.Resources.DataNotFound);
                            settingStorage.PrevTimeFrame.Restore(true);
                            SelectRangeFromSettingsAndReload();
                        }
                    }
                }
            }*/

            //BELOW: if data is not ordered, we must update the value of a certain source timestamp that could already be in the list
            //for (int i = 0; i < viewList[k].Values.Count; i++)
            //{
            //    try //viewList will be cleared if the user hits the prev/next button, causing an exception on FindIndex() method
            //    {
            //        var oldValueIndex = mapKeySeries[k].Points.FindIndex(x => x != null && viewList[k].Values[i] != null && x.argument == viewList[k].Values[i].SourceTimestamp);
            //        GenericSeriesPoint editedPoint;
            //        if (oldValueIndex == -1)
            //        {
            //            mapKeySeries[k].Points.Add(new GenericSeriesPoint(viewList[k].Values[i].SourceTimestamp, viewList[k].Values[i].Value, viewList[k].Secondvalues[i].Value));
            //            editedPoint = mapKeySeries[k].Points.Last();
            //        }
            //        else
            //        {
            //            mapKeySeries[k].Points[oldValueIndex].value = viewList[k].Values[i].Value;
            //            mapKeySeries[k].Points[oldValueIndex].secondvalue = viewList[k].Secondvalues[i].Value;
            //            editedPoint = mapKeySeries[k].Points[oldValueIndex];
            //        }
            //        if (IsInsideTimeFrame(editedPoint.argument))
            //        {
            //            int dataColIndex = FindDataColIndex(editedPoint.argument);
            //            DrawPenValue(mapKeySeries[k].penIndex, dataColIndex, editedPoint.value, editedPoint.secondvalue);
            //        }
            //    } catch (Exception ex) {
            //    }
            //}
        }

        private int FindDataColIndex(DateTime sourceTimestamp)
        {
            int c = 0;
            switch (FilterType)
            {
                case RDateSpan.Minute:
                    c = (int)(sourceTimestamp - settingStorage.DateTimeStart).TotalSeconds;
                    break;
                case RDateSpan.Hour:
                    c = (int)(sourceTimestamp - settingStorage.DateTimeStart).TotalMinutes;
                    break;
                case RDateSpan.Day: //every 15 min
                    c = (int)(sourceTimestamp - settingStorage.DateTimeStart).TotalMinutes / 15;
                    break;
                case RDateSpan.Week: //1 sample every 4 hours
                    int viewStartDay = DayOfWeekNormalized(settingStorage.DateTimeStart);
                    int dataDayIndex = DayOfWeekNormalized(sourceTimestamp);
                    int relday = dataDayIndex - viewStartDay;
                    if (relday < 0) relday += 7;
                    c = relday * 6 + sourceTimestamp.Hour / 4;
                    break;
                case RDateSpan.Month: //1 sample every 12 hours
                    int halfDayOffset = sourceTimestamp.Hour >= 12 ? 1 : 0;
                    int pastMonths_inHalfDays = 0;
                    int currYear = settingStorage.DateTimeStart.Year;
                    int currMonth = settingStorage.DateTimeStart.Month;
                    while (currMonth != sourceTimestamp.Month || currYear != sourceTimestamp.Year)
                    {
                        pastMonths_inHalfDays += DateTime.DaysInMonth(currYear, currMonth) * 2;
                        currMonth = currMonth == 12 ? 1 : currMonth + 1;
                        if (currMonth == 1)
                            currYear++;
                    }
                    c = (sourceTimestamp.Day - settingStorage.DateTimeStart.Day) * 2 + pastMonths_inHalfDays + halfDayOffset;
                    break;
                default:
                    break;
            }
            return c;
        }

        int DayOfWeekNormalized(DateTime dt) //0=Monday, 6=Sunday
        {
            int dow = (int)(dt.DayOfWeek) - 1;
            if (dow < 0) return 6;
            return dow;
        }

        public bool IsInsideTimeFrame(DateTime dt)
        {
            return (DateTime.Compare(settingStorage.DateTimeStart, dt) <= 0 && DateTime.Compare(settingStorage.DateTimeEnd, dt) >= 0);
        }

        public void UpdateElementsWidth() //To call on user control resize
        {
            plotgridLastWidth = plotGridContainer.ActualWidth;
            availablePlotGridWidth = plotgridLastWidth - (ShowRowLabels ? penLegendWidth : 0) - plotGrid.Margin.Left - plotGrid.Margin.Right;
            actualSamplingUnitWidth = Math.Max(1, (int)Math.Floor(availablePlotGridWidth / MaxDataCount));
        }
        internal string stringPlaceolder = "StatesChartControl";
        internal void StringManager_CultureChanged(object sender, EventArgs e)
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

                btnRefresh.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshTitle", stringlist, Properties.Resources.RefreshTitle);
                chkUseAbsoluteRanges.ToolTip = chkUseAbsoluteRanges.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_UseAbsoluteRangesTitle", stringlist, Properties.Resources.UseAbsoluteRangesTitle);
                btnExpand.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandTitle", stringlist, Properties.Resources.ExpandTitle);
                btnPrev.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevTitle", stringlist, Properties.Resources.PrevTitle);
                btnNext.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextTitle", stringlist, Properties.Resources.NextTitle);
                btnMin.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MinTitle", stringlist, Properties.Resources.MinTitle);
                btnHour.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HourTitle", stringlist, Properties.Resources.HourTitle);
                btnDay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayTitle", stringlist, Properties.Resources.DayTitle);
                btnWeek.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeekTitle", stringlist, Properties.Resources.WeekTitle);
                btnMonth.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthTitle", stringlist, Properties.Resources.MonthTitle);
                startText.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist, Properties.Resources.StartDate);
                endText.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndDate);
                btnZoomBack.ToolTip = btnZoomBack.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ZoomBackTitle", stringlist, Properties.Resources.ZoomBackTitle);
                btnNestInDockMgr.ToolTip = btnNestInDockMgr.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Advanced", stringlist, Properties.Resources.AddDockManager);

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                editGeneralSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EditSettings", stringlist, Properties.Resources.EditSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);


                InitTimeRange();
                RedrawPlotGrid(!bDesign, true);
            });
        }

        void InitTimeRange()
        {
            var editValue = TimeFilterCombo.EditValue as LocalizedTimeRange;
            UpdateMapToTimeRangeItem();
            TimeFilterCombo.DataContext = null;
            TimeFilterCombo.DataContext = mapToTimeRangeItem.Values;
            TimeFilterCombo.EditValue = mapToTimeRangeItem[DateSpan.Minute].Value;
            if (editValue != null)
            {
                var selectedValue = (from v in mapToTimeRangeItem.Values where v.Value == editValue.Value select v).FirstOrDefault();
                TimeFilterCombo.EditValue = selectedValue;
            }
        }
        Dictionary<DateSpan, LocalizedTimeRange> mapToTimeRangeItem = new Dictionary<DateSpan, LocalizedTimeRange>();
        void UpdateMapToTimeRangeItem()
        {
            if (mapToTimeRangeItem.Count == 0)
            {
                mapToTimeRangeItem.Add(DateSpan.Minute, new LocalizedTimeRange(Properties.Resources.Minute, DateSpan.Minute));
                mapToTimeRangeItem.Add(DateSpan.Hour, new LocalizedTimeRange(Properties.Resources.Hour, DateSpan.Hour));
                mapToTimeRangeItem.Add(DateSpan.Day, new LocalizedTimeRange(Properties.Resources.Day, DateSpan.Day));
                mapToTimeRangeItem.Add(DateSpan.Week, new LocalizedTimeRange(Properties.Resources.Week, DateSpan.Week));
                mapToTimeRangeItem.Add(DateSpan.Month, new LocalizedTimeRange(Properties.Resources.Month, DateSpan.Month));

            }
            else if (stringlist != null)
            {
                mapToTimeRangeItem[DateSpan.Minute].Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.Minute);
                mapToTimeRangeItem[DateSpan.Hour].Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.Hour);
                mapToTimeRangeItem[DateSpan.Day].Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.Day);
                mapToTimeRangeItem[DateSpan.Week].Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.Week);
                mapToTimeRangeItem[DateSpan.Month].Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Month);
            }
        }

        private string GetTranslation(string name)
        {
           return TranslationHelpers.TranslationHelper.TranslateComposedText(name, stringlist, name);
        }

        public void RedrawPlotGrid(bool reloadData = false, bool redrawLabels = false, bool hardReload = false)
        {
            if (!bInit || !IsValidTimerange())
                return;

            //if (!bDesign)
            //    StopRecording();
            if (!bDesign && AutoRefresh)
                StopAutoRefresh();

            if (redrawLabels)
            {
                labelsGrid.Children.Clear();
                labelsGrid.RowDefinitions.Clear();
                labelsGrid.ColumnDefinitions.Clear();
            }
            plotGrid.Children.Clear();
            plotGrid.RowDefinitions.Clear();
            plotGrid.ColumnDefinitions.Clear();
            timeAxisGrid.Children.Clear();
            timeAxisGrid.RowDefinitions.Clear();
            timeAxisGrid.ColumnDefinitions.Clear();
            Draw(redrawLabels);

            if (reloadData)
            {
                foreach (Rectangle b in zoomableCells)
                {
                    b.MouseUp -= ZoomInCell;
                }
                zoomableCells.Clear();
                ReloadData(hardReload);
            }
            else if (!bDesign && AutoRefresh)
                RestartAutoRefresh();
            //else if (!bDesign)
            //    RestartRecording();
        }

        void ReloadData(bool hardReload = false)
        {
            LoadRealData(hardReload);
        }

        bool IsValidTimerange()
        {
            if (settingStorage.DateTimeEnd.Subtract(settingStorage.DateTimeStart).Ticks <= 0)
            {
                ShowError(string.Format(Properties.Resources.NegativeTimeRange));
                return false;
            }
            return true;            
        }

        void Draw(bool firstTime = false)
        {
            if (bDesign)
            {
                if (TPenList == null)
                    TPenList = new TagPenList();

                actualPensList = new TagPenList(new ObservableCollection<PenList>(TPenList.Take(3).ToList()));
            }
            else if (actualPensList == null)    
            {
                actualPensList = new TagPenList(PenReferenceList);
                if (monitoredPenList != null)
                    actualPensList.Insert(0, monitoredPenList);
            }
            UpdateElementsWidth();
            ColumnDefinition gridCol;
            if (ShowRowLabels)
            {
                gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(penLegendWidth);
                if (firstTime)
                    labelsGrid.ColumnDefinitions.Add(gridCol);
                timeAxisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridCol.Width });
            }

            DrawPenSeparator(0, false, firstTime);

            for (int i = 0; i < MaxDataCount; i++)
            {
                gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(actualSamplingUnitWidth);
                plotGrid.ColumnDefinitions.Add(gridCol);
                timeAxisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridCol.Width });
            }

            for (int i = 0; i < actualPensList.Count; i++)
            {
                DrawPen(i, firstTime);
            }
            DrawPenSeparator(actualPensList.Count, true, firstTime);
            DrawTimeAxis();
        }

        void DrawTimeAxis()
        {
            lastDrawnDateStart = settingStorage.DateTimeStart;
            lastDrawnDateEnd = settingStorage.DateTimeEnd;
            RowDefinition gridRow = new RowDefinition();
            gridRow.Height = GridLength.Auto;
            timeAxisGrid.RowDefinitions.Add(gridRow);

            int lastLabelPos = 0;
            int timeLabels = 0;
            List<TextBlock> timeTexts = new List<TextBlock>();
            for (int i = 0; i < Math.Max(1, MaxDataCount - 1); i++)
            {
                if (i == 0 || /*i == MaxDataCount-1 ||*/ (i * actualSamplingUnitWidth - lastLabelPos >= timeAxisLabelDistance && i % GetTimeRangeMinColsMultiple(FilterType) == 0))
                {
                    timeLabels++;
                    lastLabelPos = i * actualSamplingUnitWidth;

                    TextBlock tTextBlock = new TextBlock()
                    {
                        Foreground = this.ReadLocalValue(LabelForegroundBrushProperty) != DependencyProperty.UnsetValue ? LabelForegroundBrush : Foreground,
                        TextAlignment = ShowRowLabels ? TextAlignment.Center : TextAlignment.Left,
                        Text = GetTimeAxisLabelText(i),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Line vLine = new Line()
                    {
                        Stroke = this.ReadLocalValue(LabelForegroundBrushProperty) != DependencyProperty.UnsetValue ? LabelForegroundBrush : Foreground,
                        Stretch = Stretch.Fill,
                        Y2 = 1,
                        StrokeThickness = 2,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    timeAxisGrid.Children.Add(tTextBlock);
                    Grid.SetRow(tTextBlock, 0);
                    Grid.SetColumn(tTextBlock, ShowRowLabels ? i + 1 : i);
                    timeTexts.Add(tTextBlock);

                    plotGrid.Children.Add(vLine);
                    Grid.SetRowSpan(vLine, plotGrid.RowDefinitions.Count);
                    Grid.SetRow(vLine, 0);
                    Grid.SetColumn(vLine, i);
                }
            }

            Line hLine = new Line()
            {
                Stroke = this.ReadLocalValue(LabelForegroundBrushProperty) != DependencyProperty.UnsetValue ? LabelForegroundBrush : Foreground,
                Stretch = Stretch.Fill,
                X2 = 1,
                StrokeThickness = 2,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            plotGrid.Children.Add(hLine);
            Grid.SetColumnSpan(hLine, plotGrid.ColumnDefinitions.Count);
            Grid.SetRow(hLine, plotGrid.RowDefinitions.Count);
            Grid.SetColumn(hLine, 0);

            timeLabelscolspan = (int)Math.Ceiling(MaxDataCount / timeLabels);
            for (int i = 0; i < timeLabelscolspan; i++) //Adding columns to the time axis in order to set colspan also for the last label, otherwise it won't be centered
            {
                ColumnDefinition gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(actualSamplingUnitWidth);
                plotGrid.ColumnDefinitions.Add(gridCol);
                timeAxisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridCol.Width });
            }
            timeTexts.ForEach(tblock => {
                Grid.SetColumnSpan(tblock, timeLabelscolspan);
                tblock.Margin = new Thickness(!ShowRowLabels /*|| tblock.Equals(timeTexts.Last())*/ ? 0 : -(timeLabelscolspan - 1) * actualSamplingUnitWidth, 0, 0, 0);
            });
        }

        string GetTimeAxisLabelText(int colindex, bool isColumnTooltip = false)
        {
            string t = "";
            switch (FilterType)
            {
                case RDateSpan.Minute:
                    t = GetColHMSLabel(colindex, settingStorage.DateTimeStart.ToString("HH"), settingStorage.DateTimeStart.ToString("mm"), settingStorage.DateTimeStart.ToString("ss"));
                    break;
                case RDateSpan.Hour:
                    t = GetColHMSLabel(colindex, settingStorage.DateTimeStart.ToString("HH"), settingStorage.DateTimeStart.ToString("mm"));
                    break;
                case RDateSpan.Day: //U.M.: 15min
                    int starting_quarters = (int)Math.Floor(double.Parse(settingStorage.DateTimeStart.ToString("mm")) / 15);
                    int total_quarters = starting_quarters + colindex;
                    int relh = (int)Math.Floor(double.Parse(settingStorage.DateTimeStart.ToString("HH")) + total_quarters / 4);
                    int h = relh % 24;
                    string d = settingStorage.DateTimeStart.AddDays(relh / 24).ToString("d", dtfi).Substring(0, 5);
                    int quarter = total_quarters % 4;
                    t = d + "\n(" + h.ToString().PadLeft(2, '0') + ":" + (quarter * 15).ToString().PadLeft(2, '0') + ")";
                    break;
                case RDateSpan.Week: //U.M.: 4h
                    DateTime dt = settingStorage.DateTimeStart.AddDays(colindex / 6);
                    if (colindex == 0 || colindex % 6 == 0 || isColumnTooltip)
                        t = dt.ToString("ddd", CultureInfo.CurrentCulture) + "\n" + dt.ToString("d", dtfi).Substring(0, 5);
                    else
                        t = "";
                    if (isColumnTooltip) //showing hour range
                        t += "\n" + (colindex % 6 * 4).ToString().PadLeft(2, '0') + ":00 - " + (colindex % 6 * 4 + 4).ToString().PadLeft(2, '0') + ":00";
                    break;
                case RDateSpan.Month: //U.M.: 12h
                    int starting_12h = (int) Math.Floor(double.Parse(settingStorage.DateTimeStart.ToString("HH")) / 12);
                    int total_12h = starting_12h + colindex;
                    int reld = settingStorage.DateTimeStart.Day + total_12h / 2; // Starts from current day and goes on, even over month's days
                    int currDay = reld;
                    int currMonth = settingStorage.DateTimeStart.Month;
                    int currYear = settingStorage.DateTimeStart.Year;
                    DateTime tlabel = new DateTime();
                    int currentMonthDays = DateTime.DaysInMonth(currYear, currMonth);
                    while (currDay > currentMonthDays)
                    {
                        currDay -= currentMonthDays;
                        currMonth++;
                        if (currMonth > 12)
                        {
                            currMonth = 1;
                            currYear++;
                        }
                        currentMonthDays = DateTime.DaysInMonth(currYear, currMonth);
                    }
                    tlabel = new DateTime(currYear, currMonth, currDay);
                    t = tlabel.ToString("d", dtfi); //t = CultureInfo.CurrentCulture.Name == "en-GB" || CultureInfo.CurrentCulture.Name == "en-US" ? tlabel.ToString("yy/MM/dd") : tlabel.ToString("dd/MM/yy");
                    break;
                default:
                    break;
            }
            return t;
        }

        string GetColHMSLabel(int colindex, string startH, string startM, string startS = null)
        {
            int relstart, m, h = int.Parse(startH);
            int? s = null;
            if (startS != null)
            {
                relstart = int.Parse(startS) + colindex;
                s = relstart % 60;
                m = int.Parse(startM) + relstart / 60;
                h += m / 60;
            }
            else
            {
                relstart = int.Parse(startM) + colindex;
                m = relstart % 60;
                h += relstart / 60;
            }
            return (h > 23 ? 0 : h).ToString().PadLeft(2, '0') + ":" + (m > 59 ? 0 : m).ToString().PadLeft(2, '0') + (startS != null ? ":" + s.ToString().PadLeft(2, '0') : "");
        }

        internal void UpdateControlLayout(bool bForceInit = false)
        {
            if ((bDisposed || !bInit) && !bForceInit)
                return;
            NestMainGridInsideDockManager(null, null);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.templateApplied = true;
        }

        List<string> queuedKeys = new List<string>();
        void UpdateMonitoredValue(string key/*, bool bFirstExecution = false*/)
        {
            bool bForceDispatcher = false;
            lock (lockObject)
            {
                if (bDisposed)
                    return;

                bForceDispatcher = queuedKeys.Count() == 0;
                if (!queuedKeys.Contains(key))
                    queuedKeys.Add(key);

                if (bForceDispatcher)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        List<string> tmpqueued;
                        lock (lockObject)
                        {
                            tmpqueued = new List<string>(queuedKeys);
                            queuedKeys.Clear();
                        }
                        foreach (var k in tmpqueued)
                        {
                            UpdateView(k);
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Use this method to set custom time range for data extraction
        /// </summary>
        /// <param name="filterType"></param>
        bool avoidFilterTypeChange = false;
        public void SetTimeRange(RDateSpan forcedFilterType, TimeSpan? customViewTimeFrame = null, bool fromCustomTimeRange = false)
        {
            DateTime date1;
            DateTime date2;
            if (fromCustomTimeRange)
            {
                avoidFilterTypeChange = true;
                FilterType = forcedFilterType;
                avoidFilterTypeChange = false;
                UseAbsoluteRangesEnabled = false;
                UseAbsoluteRanges = false;
            }else
                UseAbsoluteRangesEnabled = true;
            SelectTimeRangeCombo(forcedFilterType);
            if (mapToTimeRangeItem.Count == 0)
                InitTimeRange();
            switch (forcedFilterType)
            {
                case RDateSpan.Minute:
                    mapToTimeRangeItem[DateSpan.Minute].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Hour].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Day].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Week].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Month].IsEnabled = false;
                    break;
                case RDateSpan.Hour:
                    mapToTimeRangeItem[DateSpan.Minute].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Hour].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Day].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Week].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Month].IsEnabled = false;
                    break;
                case RDateSpan.Day:
                    mapToTimeRangeItem[DateSpan.Minute].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Hour].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Day].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Week].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Month].IsEnabled = true;
                    break;
                case RDateSpan.Week:
                    mapToTimeRangeItem[DateSpan.Minute].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Hour].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Day].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Week].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Month].IsEnabled = true;
                    break;
                case RDateSpan.Month:
                    mapToTimeRangeItem[DateSpan.Minute].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Hour].IsEnabled = false;
                    mapToTimeRangeItem[DateSpan.Day].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Week].IsEnabled = true;
                    mapToTimeRangeItem[DateSpan.Month].IsEnabled = true;
                    break;
                default:
                    break;
            }
            if (!fromCustomTimeRange)
            {
                DateSpan dateSpan = Enum.IsDefined(typeof(DateSpan), (int)forcedFilterType) ? (DateSpan)forcedFilterType : DateSpan.Month;
                DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, dateSpan, UseAbsoluteRanges);
                RoundDatesByTimeRange(forcedFilterType, date1, date2, customViewTimeFrame);
            }

            EnableNextPrev(true);
        }

        void ManageTimeRange(RDateSpan forcedFilterType)
        {
            SetTimeRange(forcedFilterType);
            SetDataSources();
        }

        void SelectTimeRangeCombo(RDateSpan tag)
        {
            var item = (from c in mapToTimeRangeItem.Values where (RDateSpan)c.Value == tag select c).FirstOrDefault();
            TimeFilterCombo.EditValue = item;
        }

        void SelectRangeFromSettingsAndReload(bool bSameTimeRange = false, bool bRefreshControl = true)
        {
            RDateSpan newTimeRange;
            if (!bSameTimeRange)
            {
                if (!IsValidTimerange())
                    return;
                TimeSpan datediffTspan = settingStorage.DateTimeEnd.Subtract(settingStorage.DateTimeStart);
                if (datediffTspan.TotalDays > maxChartDays)
                {
                    ShowError(string.Format(Properties.Resources.MaxDaysExceeded, maxChartDays));
                    return;
                }
                newTimeRange = GetRangeFromSeconds(datediffTspan.TotalSeconds);
                TimeSpan newDateDiff = RoundDatesByTimeRange(newTimeRange, null, null, null, true);
            }
            else
                newTimeRange = FilterType;
            SetTimeRange(newTimeRange, null, bSameTimeRange ? false : true);
            if (bRefreshControl)
                SetDataSources();
        }
        
        void ShowError(string errorMsg)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                errorMessageContent.Text = errorMsg;

                if (Document != null)
                {
                    if (UIMsgBoxAlertService == null)
                        UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (UIMsgBoxAlertService != null)
                        UIMsgBoxAlertService.ShowError(errorMsg);
                }
                if (UIMsgBoxAlertService == null)
                {
                    var errShowHide = TryFindResource("ShowHideErrorMessage") as Storyboard;
                    errShowHide.Begin();
                }
            });
        }

        RDateSpan GetRangeFromSeconds(double seconds)
        {
            if (seconds <= 120)
                return RDateSpan.Minute;
            else if (seconds <= 5400) //1.5 hours
                return RDateSpan.Hour;
            else if (seconds <= 172800)//2 days
                return RDateSpan.Day;
            else if (seconds <= 604800)//1 week
                return RDateSpan.Week;
            else //if (datediff <= 2592000)//30 days
                return RDateSpan.Month;
            //else if (datediff <= 31536000)
            //    return DateSpan.Year;
        }

        void Timebutton_Click(RDateSpan actrange)
        {
            bool bRangeChanged = true;
            if (FilterType == actrange)
                bRangeChanged = false;
            FilterType = actrange;
            SetTimeRange(FilterType);
            if (!bRangeChanged) //Restoring std timerange
                SelectRangeFromSettingsAndReload(true);
        }

        void Min_Click(object sender, RoutedEventArgs e)
        {
            Timebutton_Click(RDateSpan.Minute);
        }

        void Hour_Click(object sender, RoutedEventArgs e)
        {
            Timebutton_Click(RDateSpan.Hour);
        }

        void Day_Click(object sender, RoutedEventArgs e)
        {
            Timebutton_Click(RDateSpan.Day);
        }

        void Week_Click(object sender, RoutedEventArgs e)
        {
            Timebutton_Click(RDateSpan.Week);
        }

        void Month_Click(object sender, RoutedEventArgs e)
        {
            Timebutton_Click(RDateSpan.Month);
        }

        void Prev_Click(object sender, RoutedEventArgs e)
        {
            var dates = GetSelectedTimeRangeCombo(true);
            if (dates.start == settingStorage.DateTimeStart && dates.end == settingStorage.DateTimeEnd)
                return;

            settingStorage.DateTimeEnd = dates.end;
            settingStorage.DateTimeStart = dates.start;
            SetDataSources();
        }

        void Next_Click(object sender, RoutedEventArgs e)
        {
            var dates = GetSelectedTimeRangeCombo(false);
            if (dates.start == settingStorage.DateTimeStart && dates.end == settingStorage.DateTimeEnd)
                return;

            settingStorage.DateTimeEnd = dates.end;
            settingStorage.DateTimeStart = dates.start;
            SetDataSources();
        }

        void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (lastDrawnDateStart == settingStorage.DateTimeStart && lastDrawnDateEnd == settingStorage.DateTimeEnd)
                ReloadData(true);
            else
                RedrawPlotGrid(true, true, true);
        }

        void EnableNextPrev(bool bEnable)
        {
            btnPrev.IsEnabled = bEnable;
            btnNext.IsEnabled = bEnable;
            TimeFilterCombo.IsEnabled = bEnable;
        }

        public void SetDateRange(DateTime startDate, DateTime endDate, bool bRefreshControl = true)
        {
            if (!bInit || bDisposed)
                return;

            settingStorage.DateTimeStart = startDate;
            settingStorage.DateTimeEnd = endDate;
            SelectRangeFromSettingsAndReload(false, bRefreshControl);
        }

        TimeRangeDates GetSelectedTimeRangeCombo(bool prev)
        {
            TimeRangeDates dates;
            dates.start = settingStorage.DateTimeStart;
            dates.end = settingStorage.DateTimeEnd;

            var selectedItem = TimeFilterCombo.EditValue as LocalizedTimeRange;

            if (selectedItem != null)
                TimeRangeHelper.GetSelectedTimeRangeCombo(settingStorage.DateTimeStart, settingStorage.DateTimeEnd, selectedItem.Value, UseAbsoluteRanges, prev, out dates);
            return dates;
        }

        TimeSpan RoundDatesByTimeRange(RDateSpan timeRange, DateTime? date1 = null, DateTime? date2 = null, TimeSpan? customViewTimeFrame = null, bool viewTimeFrameFromRoundedSettings = false) //Rounding to the "roundingUnit" of the current RDateSpan timerange
        {
            TimeSpan roundingTimespan = dateRangeRoundingUnit[timeRange];
            if (date1 == null)
                date1 = settingStorage.DateTimeStart;
            if (date2 == null)
                date2 = settingStorage.DateTimeEnd;
            if (!UseAbsoluteRanges)
                settingStorage.DateTimeStart = DateRangeHelpers.RoundToCeilingTimeSpan((DateTime)date1, roundingTimespan);
            else
                settingStorage.DateTimeStart = DateRangeHelpers.RoundToFloorTimeSpan((DateTime)date1, roundingTimespan);
            settingStorage.DateTimeEnd = DateRangeHelpers.RoundToCeilingTimeSpan((DateTime)date2, roundingTimespan);
            recordEvery = samplingUnit[timeRange];
            if (customViewTimeFrame != null)
                viewTimeFrame = (TimeSpan)customViewTimeFrame;
            else if (viewTimeFrameFromRoundedSettings)
                viewTimeFrame = settingStorage.DateTimeEnd.Subtract(settingStorage.DateTimeStart);
            else
                ResetViewTimeFrame(timeRange);
            return settingStorage.DateTimeEnd.Subtract(settingStorage.DateTimeStart);
        }

        void ResetViewTimeFrame(RDateSpan timeRange)
        {
            if (timeRange == RDateSpan.Month)
                viewTimeFrame = settingStorage.DateTimeStart.Day != 1 ? dateSpanViewTimeFrame[timeRange] : new TimeSpan(DateTime.DaysInMonth(settingStorage.DateTimeStart.Year, settingStorage.DateTimeStart.Month), 0, 0, 0);
            else
                viewTimeFrame = dateSpanViewTimeFrame[timeRange];
        }

        void Button_FetchData(object sender, EventArgs e)
        {
            SelectRangeFromSettingsAndReload();
            settingStorage.ListRanges.Insert(0, new TimeRange()
            {
                DateTimeEnd = settingStorage.DateTimeEnd,
                DateTimeStart = settingStorage.DateTimeStart
            });
            while (settingStorage.ListRanges.Count > 10)
                settingStorage.ListRanges.Remove(settingStorage.ListRanges.Last());
        }

        void ListBoxRecent_Selected(object sender, EventArgs e)
        {
            EnableNextPrev(false);
            SelectRangeFromSettingsAndReload();
        }

        void ExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (ddm == null)
                return;
            bool bPanelsHidden = ddm.ExpandCollapseDocking();
            (sender as DevExpress.Xpf.Bars.BarCheckItem).IsChecked = bPanelsHidden;
        }

        private void SetDataSources() //Update the chart with the new timerange
        {
            RedrawPlotGrid(!bDesign);
        }

        //private void Print_Click(object sender, RoutedEventArgs e)
        //{
        //    if (RunningOnServer)
        //        return;

        //    UtilitiesPrintHelper.PrintElement(chart as FrameworkElement, this.FindParent<Window>(), true, true, System.Drawing.Printing.PaperKind.A4);
        //}

        void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        (mainBorder as UIElement).Effect = previousEffect;
                        (mainBorder as UIElement).ClipToBounds = previousClipToBounds;
                        //(this as UIElement).Opacity = 1;
                        previousEffect = null;
                        errorEffectOn = false;
                    }
                }
                else
                {
                    if (!errorEffectOn)
                    {
                        errorEffectOn = true;
                        previousEffect = (mainBorder as UIElement).Effect;
                        previousClipToBounds = (mainBorder as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (mainBorder as UIElement).Effect = effect;
                        (mainBorder as UIElement).ClipToBounds = false;
                    }
                }
            });
        }

        void EnsureDefaultValues(bool bSetCombo = true)
        {
            if (defSetting == null)
            {
                var designPenList = PenReferenceList != null ? new TagPenList(PenReferenceList) : new TagPenList();
                defSetting = new Setting()
                {
                    Name = Properties.Settings.Default.DesignSettingName,
                    PenList = designPenList.ToXml(),
                    DockLayout = DockLayout,
                    GridLayout = GridLayout,
                    FilterType = FilterType,
                    UseAbsoluteRanges = UseAbsoluteRanges,
                    ReadOnly = true
                };
            }
            MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            var defaultsetting = (from m in MemorySettingList where m.Name.Equals(Properties.Settings.Default.DesignSettingName) select m).FirstOrDefault();
            if (defaultsetting == null && defSetting != null)
                MemorySettingList.Add(defSetting);
            else
            {
                defaultsetting.ReadOnly = true;
                defaultsetting.DockLayout = defSetting.DockLayout;
                defaultsetting.GridLayout = defSetting.GridLayout;
                defaultsetting.PenList = defSetting.PenList;
                defaultsetting.FilterType = defSetting.FilterType;
                defaultsetting.UseAbsoluteRanges = defSetting.UseAbsoluteRanges;
            }
            if (bSetCombo)
            {
                configMemory.DataContext = MemorySettingList.Names;
                bIsInEditMode = true;
                configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
                bIsInEditMode = false;
            }
        }

        List<LayoutPanel> GetChildPanels(LayoutGroup root)
        {
            List<LayoutPanel> panels = new List<LayoutPanel>();
            foreach (BaseLayoutItem item in root.Items)
            {
                if (item is LayoutPanel)
                {
                    panels.Add((LayoutPanel)item);
                }

                if (item is LayoutGroup)
                {
                    panels.AddRange(GetChildPanels((LayoutGroup)item));
                }
            }
            return panels;
        }

        #region Setting Management

        /// <summary>
        /// Use this method to load runtime settings
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private void GetItem(string itemName)
        {
            //bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (setting?.PenList != null)
                {
                    TagPenList tpl = TagPenList.FromXml(setting.PenList);
                    TPenList = new TagPenList(tpl);
                    actualPensList = new TagPenList(TPenList);
                    if (monitoredPenList != null)
                        actualPensList.Insert(0, monitoredPenList);
                    ActualConfig = setting.Name;
                    DockLayout = setting.DockLayout;
                    GridLayout = setting.GridLayout;
                    FilterType = setting.FilterType;
                    UseAbsoluteRanges = setting.UseAbsoluteRanges;
                }
                else
                {
                    TPenList = !string.IsNullOrEmpty(defSetting.PenList) ? defSetting.PenList.FromXml<TagPenList>() : new TagPenList();
                    actualPensList = new TagPenList(TPenList);
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
                    DockLayout = defSetting.DockLayout;
                    GridLayout = defSetting.GridLayout;
                    FilterType = defSetting.FilterType;
                    UseAbsoluteRanges = defSetting.UseAbsoluteRanges;
                }
                bIsInEditMode = true;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = ActualConfig;
                bIsInEditMode = false;
            }
            catch (Exception)
            {
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
                ChangeSetting(editValue);
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
                RedrawPlotGrid(true, true, true);
            }
            return;
        }
        RelayCommand _editCommand;
        [Browsable(false)]
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        param => CallEditCommand(),
                        param => IsEnableCommand
                        );
                }
                return _editCommand;
            }
        }

        bool bCallingEditCommand;
        void CallEditCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingEditCommand = true;

            var layoutcontrol = new Controls.Settings(this) { Name = this.Name };
            var dialog = new GeneralDialog(layoutcontrol)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.EditSettings,
                bShowOk = true,
                bShowCancel = true,
                bShowClose = false,
                bShowHelp = true
            };

            dialog.Loaded += (o, ea) => { ThemeHelper.SetTheme(dialog); };

            if (dialog.ShowDialog() == true)
            {
                CallSaveCommand();
                GetItem(ActualConfig);
                RedrawPlotGrid(true, true, true);
            }

            bCallingEditCommand = false;
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

            MemorySettingList = new MemorySettings(oldMemoryList);
            StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            configMemory.DataContext = MemorySettingList?.Names;

            bCallingResetCommand = false;
            configMemory.EditValue = oldConfigName;
            oldMemoryList.Clear();
            oldMemoryList = null;
            oldConfigName = null;
        }

        bool IsEnableResetCommand
        {
            get
            {
                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand || bCallingEditCommand)
                    return false;
                else
                    return oldMemoryList != null;
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
        void CallSaveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingSaveCommand = true;
            string configname = configMemory.EditValue as String;
            string defaultTagSetting = Properties.Settings.Default.DesignSettingName;

            if (configname == defaultTagSetting)
                return;

            SaveDesignDockLayout();
            SaveDesignGridLayout();

            var selected = (from m in MemorySettingList
                            where m.Name == configname
                            select m).FirstOrDefault();
            if (selected == null)
            {
                MemorySettingList.Add(new Setting()
                {
                    Name = configname,
                    PenList = TPenList != null ? new TagPenList(TPenList).ToXml() : new TagPenList().ToXml(),
                    DockLayout = DockLayout,
                    GridLayout = GridLayout,
                    FilterType = FilterType,
                    UseAbsoluteRanges = UseAbsoluteRanges,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.PenList = TPenList != null ? new TagPenList(TPenList).ToXml() : new TagPenList().ToXml();
                selected.DockLayout = DockLayout;
                selected.GridLayout = GridLayout;
                selected.FilterType = FilterType;
                selected.UseAbsoluteRanges = UseAbsoluteRanges;
                selected.ReadOnly = false;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = selected.Name;
            }

            if (StorageHelper.StorageHelper.SaveMemoryMap(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null))
            {
                if (oldMemoryList != null)
                    oldMemoryList.Clear();
                oldMemoryList = null;
                oldConfigName = null;
            }
            ActualConfig = configname;
            SaveRuntimeLayout(GetStorageName(true));
            bCallingSaveCommand = false;
        }

        RelayCommand _removeCommand;
        [Browsable(false)]
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => CallRemoveCommand(),
                        param => IsEnableCommand
                        );
                }
                return _removeCommand;
            }
        }

        bool bCallingRemoveCommand;
        void CallRemoveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingRemoveCommand = true;

            var selected = (from m in MemorySettingList
                            where m.Name == configMemory.EditValue as String
                            select m).FirstOrDefault();

            if (oldMemoryList == null)
            {
                oldMemoryList = new MemorySettings(MemorySettingList);
                oldConfigName = configMemory.EditValue as string;
            }
            if (selected != null)
                MemorySettingList.Remove(selected);
            
            StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            configMemory.DataContext = MemorySettingList?.Names;
            bCallingRemoveCommand = false;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }

        bool IsEnableCommand
        {
            get
            {
                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (string.IsNullOrEmpty(configMemory.EditValue as String) || bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand || bCallingEditCommand)
                    return false;
                else
                    return configMemory.EditValue as String != Properties.Settings.Default.DesignSettingName;
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

                if (!String.IsNullOrEmpty(helper.Username))
                    MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username);
                else
                    EnsureDefaultValues(false);
                if (!LoadRuntimeLayout(GetStorageName(true)))
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
                GetItem(ActualConfig);
                Timebutton_Click(FilterType);
            });
        }
        #endregion

        public void Initialize()
        {
        }
        #endregion
        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        //bool INotifyPropertyVisibilityChanged.this[string propertyName]
        //{
        //    get
        //    {
        //        if (propertyName == "PensHeight" || propertyName == "SeparatorHeight")
        //        {
        //            return customPensHeight;
        //        }
        //        return true;
        //    }
        //}
        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        //public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        //protected void OnPropertyVisiblityChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyVisiblityChanged;
        //    if (handler != null)
        //    {
        //        var e = new PropertyChangedEventArgs(propertyName);
        //        handler(this, e);
        //    }
        //}
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
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

        #region IDIsposable
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (oldMemoryList != null)
                oldMemoryList.Clear();
            
            mapToTimeRangeItem.Clear();
            
            if (MemorySettingList != null)
                MemorySettingList.Clear();

            if (helper != null)
                helper.Dispose();

            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Tick -= RefreshTimer_Tick;
            }

            SizeChanged -= StatesChartControl_SizeChanged;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            
            if (penReferenceList != null)
                penReferenceList.Clear();

            if (actualPensList != null)
                actualPensList.Clear();

            if (TPenList != null)
            {
                foreach (var pen in TPenList)
                    pen.PropertyChanged -= OnPenPropertyChange;
                TPenList.Clear();
            }

            //*************
            //set not in use
            //*************
            foreach (var key in OpcuaEntityReference.Keys)
            {
                TerminateExecution(key);
            }

            if (mapHandlers != null)
            {
                foreach (var helper in mapHandlers.Values)
                    helper.Dispose();
                mapHandlers.Clear();
            }

            if (opcuaEntityReference != null)
                opcuaEntityReference.Clear();

            foreach (var serie in mapKeySeries.Values)
                serie.Points.Clear();
            mapKeySeries.Clear();

            if (dataContextSerie != null)
                dataContextSerie.Points.Clear();

            toolbar.MouseEnter -= toolbar_MouseEnter;
            toolbar.MouseLeave -= toolbar_MouseLeave;

            //if (monitoredItemViewModel != null)
            //    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            typeHelper.Dispose();

            if (viewList != null)
            {
                foreach (var view in viewList.Values)
                {
                    view.Error -= viewList_OnError;
                    view.HistoryLoaded -= SeriesHistoryLoaded;
                    view.Dispose();
                }
                viewList.Clear();
            }

            if (viewDataContext != null)
                viewDataContext.Dispose();

            if (MapToDatalogerConnectsions != null)
                MapToDatalogerConnectsions.Clear();

            if (MapToHistoricalConnectsions != null)
                MapToDatalogerConnectsions.Clear();

            if (dlrSettings != null)
            {
                foreach (var dlSetting in dlrSettings.Values)
                    dlSetting.Columns?.Clear();
                dlrSettings.Clear();
            }

            ddm?.Dispose();

            if (zoomableCells != null)
            {
                foreach (Rectangle b in zoomableCells)
                    b.MouseUp -= ZoomInCell;
                zoomableCells.Clear();
            }

            lock (lockObject)
            {
                queuedKeys.Clear();
            }
            settingStorage?.Dispose();
            plotGrid.Children.Clear();
            plotGrid.RowDefinitions.Clear();
            plotGrid.ColumnDefinitions.Clear();
            timeAxisGrid.Children.Clear();
            timeAxisGrid.RowDefinitions.Clear();
            timeAxisGrid.ColumnDefinitions.Clear();
            lock (lockObject)
            {
                SeriesHistoryLoaded_WaitingPens?.Clear();
            }
        }
        #endregion
        #region IConnectionAware
        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
        }
        #endregion


        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (TPenList != null)
            {
                var _list = TPenList.Where(x => !string.IsNullOrEmpty(x.title)).Select(x => x.title);
                if (_list != null && _list.Count() > 0)
                    list.AddRange(_list);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            int i = 1;
            TPenList?.Where(x => !string.IsNullOrEmpty(x.title)).ToList().ForEach(x =>
            {
                map.Add(x.CreateUniqueName(x.title, map.Keys.ToList()), x.title);
                i++;
            });
            return map;
        }
        #endregion

        #region IGridLayoutUser
        public List<StorageColumn> GetColumns()
        {
            UpdateControlLayout(true);
            if (ddm == null)
                return new List<StorageColumn>();

            return (from column in ddm.legendListBox.Columns
                    select new StorageColumn()
                    {
                        FieldName = column.Tag?.ToString(),
                        ActualWidth = column.ActualWidth,
                        ColumnTag = column.Tag?.ToString(),
                        Visible = column.Visible,
                        VisibleIndex = column.VisibleIndex,
                        ColumnOrder = column.SortOrder,
                        SortIndex = column.SortIndex
                    }).ToList();
        }
        #endregion
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarForeground), RequiredKey = true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush ToolbarForeground { get { return Foreground; } }
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
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            if (sender is StatesChartControl)
            {
                StatesChartControl control = sender as StatesChartControl;
                if (control.ReadLocalValue(StatesChartControl.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(StatesChartControl.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);

                if (control.ReadLocalValue(StatesChartControl.LegendAreaForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("LegendAreaForeground", control.LegendAreaForeground);
                else
                    ret.Add("LegendAreaForeground", foreground);

                if (control.ReadLocalValue(StatesChartControl.LabelForegroundBrushProperty) != DependencyProperty.UnsetValue)
                    ret.Add("LabelForegroundBrush", control.LabelForegroundBrush);
                else
                    ret.Add("LabelForegroundBrush", foreground);
                
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
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

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;

            StatesChartControl control = sender as StatesChartControl;
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

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;

            StatesChartControl control = sender as StatesChartControl;
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
