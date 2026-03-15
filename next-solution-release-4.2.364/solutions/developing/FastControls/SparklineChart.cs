using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Editors;
using DataReader.Helpers;
using OPCUAViewModel;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using ScreenSettings;
using DataReader.SchemaInfo;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using ViewModelLib;
using System.Windows.Input;
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using CommonControls.PropertyDataTemplate;
using UFInterfaces;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using StringManager.ComponentService;
using WPFUtilities;
using WPFUtilities.Extensions;
using log4net;
using Utilities.WPF;
using DataReader;
using DataReaderEditor.PropertyDataTemplate;
using System.Xml.Serialization;
using WPFUtilities.Extensions;
using System.Windows.Media.Effects;
using UFProjectManager.ComponentService;

namespace FastControls
{
    public class SparklineChart : UserControl, IDisposable, IContainPropertyEditors, IDataErrorInfo, IConnectionAware
        , IStringIDAware
    {

        #region Declarations
        bool bLoaded;
        bool bInit;
        Grid mainGrid;
        Grid plotArea;
        Grid gridArea;
        TextBlock argumentLabel;
        TextBlock valueLabel;
        TextBlock statusText;
        Border mborder;
        bool templateApplied;
        bool bDesignmode;
        bool bLoadingData;
        List<Task> listToWait;
        CancellationTokenSource tokenSource;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.SparklineChart);
        CancellationToken ct;
        IDocument Document;
        IUFUAEditorManager ufuaEditorService;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IUFProjectManager iUFProjectManager;
        IStringEditorManager stringManager;
        IDictionary<String, String> stringlist;
        Border mainBorder;
        SparklineEdit sparklineEdit;
        DevExpress.Xpf.Editors.Range sparkRange;
        IList SourceCollection { get; set; }
        DelayedSingleActionInvoker SizeChangedInvoker;
        //int maxValue = 100;
        //int minValue = 0;
        DispatcherTimer timer;
        static double HourPerDateRange = 24; //24h SparklineChart
        double SecondPerDateRange = HourPerDateRange * 3600;
        DropShadowEffect errorEffect = new DropShadowEffect
        {
            ShadowDepth = 0,
            BlurRadius = 10,
            Color = Colors.Red
        };

        DateTime startDate
        {
            get
            {
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
        }
        DateTime endDate
        {
            get
            {
                return startDate.AddHours(HourPerDateRange);
            }
        }


        #endregion
        #region ICommand
        RelayCommand _refreshCommand;
        [Browsable(false)]
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        param => CallRefreshCommand(),
                        param => IsEnableCallRefreshCommand
                        );
                }
                return _refreshCommand;
            }
        }

        bool bCallingRefreshCommand;
        void CallRefreshCommand()
        {
            try
            {
                bCallingRefreshCommand = true;
                LoadData();
            }
            finally
            {
                bCallingRefreshCommand = false;
            }
        }

        bool IsEnableCallRefreshCommand
        {
            get
            {
                if (bDesignmode || !bInit || bCallingRefreshCommand)
                    return false;
                else
                    return true;

            }
        }
        #endregion
        #region DP
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SparklineChart));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(SparklineChart));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(SparklineChart));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(SparklineChart));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(SparklineChart));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SparklineChart));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(SparklineChart));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(SparklineChart));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(SparklineChart));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(SparklineChart));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as SparklineChart;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bInit && bDesignmode)
            ManageOverlappingGridLabels();
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as SparklineChart;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bInit && bDesignmode)
            ManageOverlappingGridLabels();
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as SparklineChart;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bInit && bDesignmode)
            ManageOverlappingGridLabels();
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as SparklineChart;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bInit && bDesignmode)
            ManageOverlappingGridLabels();
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as SparklineChart;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesignmode)
                LabelBrush = Foreground;
        }

        #endregion
        #region LabelBrush
        public static readonly DependencyProperty LabelBrushProperty = DependencyProperty.Register("LabelBrush", typeof(Brush), typeof(SparklineChart), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        public Brush LabelBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelBrushProperty);
            }
            set
            {
                SetValue(LabelBrushProperty, value);
            }
        }
        #endregion
        #region PenBrush
        public static readonly DependencyProperty PenBrushProperty = DependencyProperty.Register("PenBrush", typeof(Color), typeof(SparklineChart), new UIPropertyMetadata(Colors.Orange, OnPenBrushChanged));

        private static void OnPenBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlPenBrushChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        private void OnControlPenBrushChanged(Color oldValue, Color newValue)
        {
            if (bInit)
                UpdateStyle();
        }

        public Color PenBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(PenBrushProperty);
            }
            set
            {
                SetValue(PenBrushProperty, value);
            }
        }
        #endregion
        #region MarkerBrush
        public static readonly DependencyProperty MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Color), typeof(SparklineChart), new UIPropertyMetadata(Colors.Black, OnMarkerBrushChanged));

        private static void OnMarkerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlMarkerBrushChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        private void OnControlMarkerBrushChanged(Color oldValue, Color newValue)
        {
            if (bInit)
                UpdateStyle();
        }

        public Color MarkerBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(MarkerBrushProperty);
            }
            set
            {
                SetValue(MarkerBrushProperty, value);
            }
        }
        #endregion
        #region GridBrush
        public static readonly DependencyProperty GridBrushProperty = DependencyProperty.Register("GridBrush", typeof(Color), typeof(SparklineChart), new UIPropertyMetadata(Colors.LightGray, OnGridBrushChanged));

        private static void OnGridBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlGridBrushChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        private void OnControlGridBrushChanged(Color oldValue, Color newValue)
        {
            if (bInit)
                UpdateGridBrush();
        }

        public Color GridBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(GridBrushProperty);
            }
            set
            {
                SetValue(GridBrushProperty, value);
            }
        }
        #endregion
        #region PlotBorderBrush
        public static readonly DependencyProperty PlotBorderBrushProperty = DependencyProperty.Register("PlotBorderBrush", typeof(Color), typeof(SparklineChart), new UIPropertyMetadata(Colors.Black, OnPlotBorderBrushChanged));

        private static void OnPlotBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlPlotBorderBrushChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        private void OnControlPlotBorderBrushChanged(Color oldValue, Color newValue)
        {
            if (bInit)
                UpdateMainBorder();
        }

        public Color PlotBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(PlotBorderBrushProperty);
            }
            set
            {
                SetValue(PlotBorderBrushProperty, value);
            }
        }
        #endregion

        #region LabelFormat
        public static readonly DependencyProperty LabelFormatProperty = DependencyProperty.Register("LabelFormat", typeof(string), typeof(SparklineChart), new UIPropertyMetadata("{0:#0}", new PropertyChangedCallback(OnLabelFormatChanged), new CoerceValueCallback(OnCoerceLabelFormat)));

        private static object OnCoerceLabelFormat(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceLabelFormat((string)value);
            else
                return value;
        }

        private static void OnLabelFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnLabelFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceLabelFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string LabelFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(LabelFormatProperty);
            }
            set
            {
                SetValue(LabelFormatProperty, value);
            }
        }

        #endregion


        #region MinorXDivision
        public static readonly DependencyProperty MinorXDivisionProperty = DependencyProperty.Register("MinorXDivision", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(4, OnMinorXDivisionChanged));
        private static void OnMinorXDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlMinorXDivisionChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void OnControlMinorXDivisionChanged(int oldValue, int newValue)
        {
            if (bInit)
                ManageGridDivisions();
        }
        public int MinorXDivision
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorXDivisionProperty);
            }
            set
            {
                SetValue(MinorXDivisionProperty, value);
            }
        }
        #endregion
        #region MajorXDivision
        public static readonly DependencyProperty MajorXDivisionProperty = DependencyProperty.Register("MajorXDivision", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(12, OnMajorXDivisionChanged));
        private static void OnMajorXDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlMajorXDivisionChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void OnControlMajorXDivisionChanged(int oldValue, int newValue)
        {
            if (bInit)
                ManageGridDivisions();
        }
        public int MajorXDivision
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorXDivisionProperty);
            }
            set
            {
                SetValue(MajorXDivisionProperty, value);
            }
        }
        #endregion

        #region MinorYDivision
        public static readonly DependencyProperty MinorYDivisionProperty = DependencyProperty.Register("MinorYDivision", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(0, OnMinorYDivisionChanged));
        private static void OnMinorYDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlMinorYDivisionChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void OnControlMinorYDivisionChanged(int oldValue, int newValue)
        {
            if (bInit)
                ManageGridDivisions();
        }
        public int MinorYDivision
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorYDivisionProperty);
            }
            set
            {
                SetValue(MinorYDivisionProperty, value);
            }
        }
        #endregion
        #region MajorYDivision
        public static readonly DependencyProperty MajorYDivisionProperty = DependencyProperty.Register("MajorYDivision", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(2, OnMajorYDivisionChanged));
        private static void OnMajorYDivisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlMajorYDivisionChanged((int)e.OldValue, (int)e.NewValue);
        }

        private void OnControlMajorYDivisionChanged(int oldValue, int newValue)
        {
            if (bInit)
                ManageGridDivisions();
        }
        public int MajorYDivision
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorYDivisionProperty);
            }
            set
            {
                SetValue(MajorYDivisionProperty, value);
            }
        }
        #endregion
        #region ShowMarkers
        public static readonly DependencyProperty ShowMarkersProperty = DependencyProperty.Register("ShowMarkers", typeof(bool), typeof(SparklineChart), new UIPropertyMetadata(false, OnShowMarkersChanged));

        private static void OnShowMarkersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = d as SparklineChart;
            if (control != null)
                control.OnControlShowMarkersChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private void OnControlShowMarkersChanged(bool oldValue, bool newValue)
        {
            if (bInit)
                UpdateStyle();
        }

        public bool ShowMarkers
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowMarkersProperty);
            }
            set
            {
                SetValue(ShowMarkersProperty, value);
            }
        }
        #endregion

        #region InvalidPointsValue
        public static readonly DependencyProperty InvalidPointsValueProperty = DependencyProperty.Register("InvalidPointsValue", typeof(double), typeof(SparklineChart), new UIPropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalidPointsValueChanged), new CoerceValueCallback(OnCoerceInvalidPointsValue)));

        private static object OnCoerceInvalidPointsValue(DependencyObject o, object value)
        {
            SparklineChart SparklineChart = o as SparklineChart;
            if (SparklineChart != null)
                return SparklineChart.OnCoerceInvalidPointsValue((double)value);
            else
                return value;
        }

        private static void OnInvalidPointsValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart SparklineChart = o as SparklineChart;
            if (SparklineChart != null)
                SparklineChart.OnInvalidPointsValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceInvalidPointsValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnInvalidPointsValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        
        public double InvalidPointsValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(InvalidPointsValueProperty);
            }
            set
            {
                SetValue(InvalidPointsValueProperty, value);
            }
        }
        #endregion

        #region ShowLabels
        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(SparklineChart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowLabelsChanged), new CoerceValueCallback(OnCoerceShowLabels)));

        private static object OnCoerceShowLabels(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceShowLabels((bool)value);
            else
                return value;
        }

        private static void OnShowLabelsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnShowLabelsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowLabels(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLabelsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesignmode)
                ManageGridDivisions();
        }

        public bool ShowLabels
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowLabelsProperty);
            }
            set
            {
                SetValue(ShowLabelsProperty, value);
            }
        }

        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(SparklineChart), new UIPropertyMetadata(0D, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnMinValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesignmode)
                UpdateYGridLabelValues();
        }

        public double MinValue
        {
            // IMPORTANT: To madoubleain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        #endregion

        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SparklineChart), new UIPropertyMetadata(100D, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnMaxValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMaxValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesignmode)
                UpdateYGridLabelValues();
        }

        public double MaxValue
        {
            // IMPORTANT: To madoubleain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        #region AutomaticScale
        public static readonly DependencyProperty AutomaticScaleProperty = DependencyProperty.Register("AutomaticScale", typeof(bool), typeof(SparklineChart), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutomaticScaleChanged), new CoerceValueCallback(OnCoerceAutomaticScale)));

        private static object OnCoerceAutomaticScale(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceAutomaticScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnAutomaticScaleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutomaticScale(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutomaticScaleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bDesignmode && bInit)
                UpdateYGridLabelValues();
        }

        public bool AutomaticScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutomaticScaleProperty);
            }
            set
            {
                SetValue(AutomaticScaleProperty, value);
            }
        }

        #endregion

        #region LoadDataEvery
        public static readonly DependencyProperty LoadDataEveryProperty = DependencyProperty.Register("LoadDataEvery", typeof(TimeSpan), typeof(SparklineChart), new UIPropertyMetadata(new TimeSpan(0, 0, 1, 0), new PropertyChangedCallback(OnLoadDataEveryChanged), new CoerceValueCallback(OnCoerceLoadDataEvery)));

        private static object OnCoerceLoadDataEvery(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceLoadDataEvery((TimeSpan)value);
            else
                return value;
        }

        private static void OnLoadDataEveryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnLoadDataEveryChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceLoadDataEvery(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLoadDataEveryChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TimeSpan LoadDataEvery
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(LoadDataEveryProperty);
            }
            set
            {
                SetValue(LoadDataEveryProperty, value);
            }
        }

        #endregion

        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public string ConnectionString
        {
            get
            {
                return (string)GetValue(ConnectionStringProperty);
            }
            set
            {
                SetValue(ConnectionStringProperty, value);
            }
        }
        #endregion
        
        #region ValueTitle
        public static readonly DependencyProperty ValueTitleProperty = DependencyProperty.Register("ValueTitle", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValueTitleChanged), new CoerceValueCallback(OnCoerceValueTitle)));

        private static object OnCoerceValueTitle(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceValueTitle((string)value);
            else
                return value;
        }

        private static void OnValueTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnValueTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceValueTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (valueLabel != null)
            {
                valueLabel.Margin = string.IsNullOrEmpty(newValue) ? new Thickness(0) : new Thickness(5, 0, 5, 0);
                UpdateValueAxisTitle();
            }
        }

        public string ValueTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ValueTitleProperty);
            }
            set
            {
                SetValue(ValueTitleProperty, value);
            }
        }

        #endregion


        #region ArgumentTitle
        public static readonly DependencyProperty ArgumentTitleProperty = DependencyProperty.Register("ArgumentTitle", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null, new PropertyChangedCallback(OnArgumentTitleChanged), new CoerceValueCallback(OnCoerceArgumentTitle)));

        private static object OnCoerceArgumentTitle(DependencyObject o, object value)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                return control.OnCoerceArgumentTitle((string)value);
            else
                return value;
        }

        private static void OnArgumentTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart control = o as SparklineChart;
            if (control != null)
                control.OnArgumentTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceArgumentTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnArgumentTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (argumentLabel != null)
            {
                argumentLabel.Margin = string.IsNullOrEmpty(newValue) ? new Thickness(0) : new Thickness(0, 5, 0, 10);
                UpdateArgumentAxisTitle();
            }
        }

        public string ArgumentTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ArgumentTitleProperty);
            }
            set
            {
                SetValue(ArgumentTitleProperty, value);
            }
        }
        #endregion

        #region ControlDataSource
        public static readonly DependencyProperty ControlDataSourceProperty = DependencyProperty.Register("ControlDataSource", typeof(DataReaderModelXML), typeof(SparklineChart), new UIPropertyMetadata(new DataReaderModelXML(), new PropertyChangedCallback(OnControlDataSourceChanged), new CoerceValueCallback(OnCoerceControlDataSource)));

        private static object OnCoerceControlDataSource(DependencyObject o, object value)
        {
            SparklineChart SparklineChart = o as SparklineChart;
            if (SparklineChart != null)
                return SparklineChart.OnCoerceControlDataSource((DataReaderModelXML)value);
            else
                return value;
        }

        private static void OnControlDataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SparklineChart SparklineChart = o as SparklineChart;
            if (SparklineChart != null)
                SparklineChart.OnControlDataSourceChanged((DataReaderModelXML)e.OldValue, (DataReaderModelXML)e.NewValue);
        }

        protected virtual DataReaderModelXML OnCoerceControlDataSource(DataReaderModelXML value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlDataSourceChanged(DataReaderModelXML oldValue, DataReaderModelXML newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                if (!bDesignmode)
                    LoadData();
            }
        }

        /// <summary>
        /// The connection setting informations used for connecting to database.
        /// </summary>
        public DataReaderModelXML ControlDataSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DataReaderModelXML)GetValue(ControlDataSourceProperty);
            }
            set
            {
                SetValue(ControlDataSourceProperty, value);
            }
        }

        #endregion


        #region TableName
        public static readonly DependencyProperty TableNameProperty = DependencyProperty.Register("TableName", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public string TableName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TableNameProperty);
            }
            set
            {
                SetValue(TableNameProperty, value);
            }
        }
        #endregion
        #region ValueColumnName
        public static readonly DependencyProperty ValueColumnNameProperty = DependencyProperty.Register("ValueColumnName", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public string ValueColumnName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ValueColumnNameProperty);
            }
            set
            {
                SetValue(ValueColumnNameProperty, value);
            }
        }
        #endregion
        #region TimeColumnName
        public static readonly DependencyProperty TimeColumnNameProperty = DependencyProperty.Register("TimeColumnName", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public string TimeColumnName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TimeColumnNameProperty);
            }
            set
            {
                SetValue(TimeColumnNameProperty, value);
            }
        }
        #endregion
        #region MaxrecordNum
        public static readonly DependencyProperty MaxrecordNumProperty = DependencyProperty.Register("MaxrecordNum", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public int MaxrecordNum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxrecordNumProperty);
            }
            set
            {
                SetValue(MaxrecordNumProperty, value);
            }
        }
        #endregion
        #region WhereCondition
        public static readonly DependencyProperty WhereConditionProperty = DependencyProperty.Register("WhereCondition", typeof(string), typeof(SparklineChart), new UIPropertyMetadata(null));
        [Obsolete("Use ControlDataSource instead")]
        [Browsable(false)]
        [XmlIgnore]
        public string WhereCondition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(WhereConditionProperty);
            }
            set
            {
                SetValue(WhereConditionProperty, value);
            }
        }
        #endregion
        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(SparklineChart), new UIPropertyMetadata(30));
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

        #endregion

        #region ctor
        static SparklineChart()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SparklineChart), new FrameworkPropertyMetadata(typeof(SparklineChart)));
        }
        public SparklineChart()
        {
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            Action actionDelaySizeChange = () =>
            {
                if (this.templateApplied)
                    this.Draw();
            };
            SizeChangedInvoker = new DelayedSingleActionInvoker(actionDelaySizeChange);
            base.SizeChanged += SparklineChart_SizeChanged;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;
                    OverrideBaseProperties();
                    DataContext = this;
                    actionDelaySizeChange();
                    RenewControlDataSource();
                }
            };
        }

        private void RenewControlDataSource()
        {
            if(ControlDataSource.ReaderModel == null && (
                !string.IsNullOrEmpty(TableName) ||
                !string.IsNullOrEmpty(ValueColumnName) ||
                !string.IsNullOrEmpty(TimeColumnName) ||
                !string.IsNullOrEmpty(WhereCondition) ||
                !string.IsNullOrEmpty(ConnectionString)))
            {
                var dataReader = new DataReader.DataReaderModel()
                {
                    Connection = ConnectionString,
                    TableName = TableName,
                    DataColumn = ValueColumnName,
                    TimeColumn = TimeColumnName,
                    MaxTake = MaxrecordNum,
                    Where = WhereCondition
                };

                ControlDataSource = new DataReaderModelXML(dataReader);
            }
        }

        private void SparklineChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
        }

        private void Draw()
        {
            if (bDisposed)
                return;

            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                bDesignmode = true;

            //base.Background = this.ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue ? new SolidColorBrush(Color.FromArgb(255, 105, 105, 105)) : Background;
            if(templateApplied)
            {
                UpdateStyle();
                if (!bInit)
                {
                    argumentLabel.Margin = string.IsNullOrEmpty(ArgumentTitle) ? new Thickness(0) : new Thickness(0, 5, 0, 10);
                    valueLabel.Margin = string.IsNullOrEmpty(ValueTitle) ? new Thickness(0) : new Thickness(5, 0, 5, 0);
                    ManageGridDivisions();

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (sparklineEdit != null && Document != null)
                    {
                        if (stringManager == null)
                        {
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                            if (stringManager != null)
                            {
                                stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                                StringManager_CultureChanged(Document, null);
                                stringManager.CultureChanged += StringManager_CultureChanged;
                            }
                        }
                    }

                    if (bDesignmode)
                        GenerateData();
                    else if (sparklineEdit != null)
                    {
                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        LoadData();
                        if (timer == null && !bDisposed)
                        {
                            timer = new DispatcherTimer();
                            timer.Tick += timer_Tick;
                            timer.Interval = LoadDataEvery;
                            timer.Start();
                        }
                    }
                }
                else
                {
                    if (gridArea != null && gridArea.Children.Count > 0)
                    {
                        gridArea.InvalidateVisual();
                    }
                    else
                    {
                        ManageGridDivisions();
                    }
                    UpdateMainBorder();
                    UdateValues();
                }

                ManageOverlappingGridLabels();
                bInit = true;
            }
        }
        protected Size GetLayoutSize(FrameworkElement fe)
        {
            double width;
            double height;
            if (!double.IsNaN(fe.Width) && !double.IsInfinity(fe.Width))
            {
                width = fe.Width;
            }
            else if ((fe.ActualWidth == 0.0) && (fe.DesiredSize.Width > 0.0))
            {
                width = fe.DesiredSize.Width;
            }
            else
            {
                width = fe.ActualWidth;
            }
            if (!double.IsNaN(fe.Height) && !double.IsInfinity(fe.Height))
            {
                height = fe.Height;
            }
            else if ((fe.ActualHeight == 0.0) && (fe.DesiredSize.Height > 0.0))
            {
                height = fe.DesiredSize.Height;
            }
            else
            {
                height = fe.ActualHeight;
            }
            return new Size(width, height);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            LoadData();
        }

        private void InitRange()
        {
            if (sparklineEdit == null)
                return;

            Range range = new Range();
            range.Limit1 = startDate;
            range.Limit2 = endDate;
            range.Auto = false;
            sparklineEdit.BeginInit();
            sparklineEdit.PointArgumentRange = range;

            if (AutomaticScale)
            {
                Range valuerange = new Range();
                valuerange.Auto = true;
                sparklineEdit.PointValueRange = valuerange;
            }
            else
            {
                Range valuerange = new Range();
                valuerange.Limit1 = MaxValue;
                valuerange.Limit2 = MinValue;
                valuerange.Auto = false;
                sparklineEdit.PointValueRange = valuerange;
            }
            sparklineEdit.EndInit();
        }

#if !WINDOWS_UWP
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            this.mainGrid = base.GetTemplateChild("mainGrid") as Grid;
            this.mainBorder = base.GetTemplateChild("mainBorder") as Border;
            this.sparkRange = base.GetTemplateChild("sparkRange") as DevExpress.Xpf.Editors.Range;
            this.plotArea = base.GetTemplateChild("plotArea") as Grid;
            this.gridArea = base.GetTemplateChild("gridArea") as Grid;
            this.sparklineEdit = base.GetTemplateChild("sparklineEdit") as SparklineEdit;
            this.valueLabel = base.GetTemplateChild("valueLabel") as TextBlock;
            this.argumentLabel = base.GetTemplateChild("argumentLabel") as TextBlock;
            this.statusText = base.GetTemplateChild("statusText") as TextBlock;
            this.templateApplied = true;
        }
        #endregion
        #region Methods
        internal string stringPlaceolder = "SparklineChart";
        internal void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeIfRequired((Action)(() =>
            {
                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture((IDocument)this.Document, stringManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                UpdateValueAxisTitle();
                UpdateArgumentAxisTitle();
            }));
        }
        void UpdateValueAxisTitle()
        {
            if (!string.IsNullOrEmpty(ValueTitle))
                valueLabel.Text = TranslationHelpers.TranslationHelper.TranslateComposedText(ValueTitle, stringlist, ValueTitle);
        }
        void UpdateArgumentAxisTitle()
        {
            if (!string.IsNullOrEmpty(ArgumentTitle))
                argumentLabel.Text = stringlist != null && stringlist.ContainsKey(ArgumentTitle) ? stringlist[ArgumentTitle] : ArgumentTitle;
        }
        async void LoadData()
        {
            if (bLoadingData || bDisposed || sparklineEdit == null)
                return;

            string defaultDataProvider = null;
            string defaultConnectionString = null;
            var _valuecolumnname = !string.IsNullOrEmpty(ValueColumnName) ? ValueColumnName : Properties.Settings.Default.ValueColumnName;
            var _timecolumnname = !string.IsNullOrEmpty(TimeColumnName) ? TimeColumnName : Properties.Settings.Default.TimeColumnName;
            var _tablename = !string.IsNullOrEmpty(TableName) ? TableName : Properties.Settings.Default.TableName;
            var _maxrecord = MaxrecordNum;
            var _whereCondition = !string.IsNullOrEmpty(WhereCondition) ? WhereCondition : string.Empty;
            var _orderby = string.Empty;
            var _groupby = string.Empty;
            string _select = null;

            sparklineEdit.EditValue = null;

            if (!string.IsNullOrEmpty(ConnectionString))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);
            }
            else
            {
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (Document != null)
                    ufuaEditorService = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (ufuaEditorService != null)
                {
                    string settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaEditorService.GetHistorianDefaultConnection(Document), (Document as ScreenDocument).SessionString);
                    if (!string.IsNullOrEmpty(settings))
                    {
                        defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                        defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                    }
                }
            }

            if (ControlDataSource != null && ControlDataSource.ReaderModel != null)
            {
                var model = ControlDataSource.ReaderModel;
                if (!string.IsNullOrEmpty(model.DataProvider) &&
                    !string.IsNullOrEmpty(model.Connection))
                {
                    defaultDataProvider = model.DataProvider;
                    defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(model.Connection, Document?.rootBase);
                }

                if(!string.IsNullOrEmpty(model.DataColumn))
                    _valuecolumnname =  model.DataColumn;
                if (!string.IsNullOrEmpty(model.TimeColumn))
                    _timecolumnname = model.TimeColumn;
                if (!string.IsNullOrEmpty(model.TableName))
                    _tablename = model.TableName;

                _maxrecord = model.MaxTake;
                _whereCondition = model.Where;
                _select = model.Select;
                _orderby = model.Sort;
                _groupby = model.GroupBy;
            }
            if (string.IsNullOrEmpty(defaultDataProvider) || string.IsNullOrEmpty(defaultConnectionString))
                return;

            bLoadingData = true;

            TaskScheduler sc = TaskScheduler.FromCurrentSynchronizationContext();
            if (listToWait == null)
                listToWait = new List<Task>();
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
                ct = tokenSource.Token;
            }

            var _data = new Collection<CustomElement>();
           
            DateTime startdate = startDate;
            DateTime enddate = endDate;
            var invalidPointsValue = InvalidPointsValue;
            var task1 = Task.Factory.StartNew((commandTimeout) =>
            {
                using (var dbconnection = DataReader.DataReader.CreateDbConnection(defaultDataProvider, defaultConnectionString))
                {
                    if (ct.IsCancellationRequested)
                        return _data;

                    dbconnection.Open();
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(defaultDataProvider);

                    DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(defaultDataProvider, defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(defaultDataProvider);
                    dbdapater.SelectCommand.Connection = dbconnection;

                    StringBuilder commantText = new StringBuilder();
                    DataSet gridDataSet = new DataSet();

                    if(string.IsNullOrEmpty(_select))
                    {
                        commantText.AppendFormat("SELECT");
                        if (dbSchemaInfo.IsSupportedTopKeyword && _maxrecord > 0.0)
                            commantText.AppendFormat(" TOP {0} ", _maxrecord);
                        commantText.AppendFormat(" {0}", dbSchemaInfo.WrapObjectName(_timecolumnname));
                        commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(_valuecolumnname));

                        commantText.AppendFormat(" FROM {0}",
                            dbSchemaInfo.WrapObjectName(_tablename));


                    }
                    else
                        commantText.AppendFormat("{0}", _select);

                    //commantText.AppendFormat(" WHERE {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(_valuecolumnname));

                    var datestart = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                    datestart.DbType = System.Data.DbType.DateTime;
                    datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                    datestart.Value = startdate;
                    dbdapater.SelectCommand.Parameters.Add(datestart);

                    var dateend = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                    dateend.DbType = System.Data.DbType.DateTime;
                    dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                    dateend.Value = enddate;
                    dbdapater.SelectCommand.Parameters.Add(dateend);

                    commantText.AppendFormat(" WHERE {0} >= {1} AND {0} < {2} AND {0} IS NOT NULL",
                        dbSchemaInfo.WrapObjectName(_timecolumnname),
                        datestart.ParameterName,
                        dateend.ParameterName);

                    if (!string.IsNullOrEmpty(_whereCondition))
                        commantText.AppendFormat(" AND {0}", _whereCondition);

                    commantText.AppendFormat(" ORDER BY ");

                    if (!string.IsNullOrEmpty(_orderby))
                    {
                        if (!_orderby.Contains(_timecolumnname))
                            commantText.AppendFormat("{0} ASC, ", dbSchemaInfo.WrapObjectName(_timecolumnname));
                        commantText.AppendFormat("{0}", _orderby);
                    }
                    else
                        commantText.AppendFormat("{0} ASC", dbSchemaInfo.WrapObjectName(_timecolumnname));

                    if (!string.IsNullOrEmpty(_groupby))
                        commantText.AppendFormat(" GROUP BY {0}", _groupby);

                    dbdapater.SelectCommand.CommandText = commantText.ToString();
                    dbdapater.SelectCommand.CommandTimeout = (int)commandTimeout;

                    if (ct.IsCancellationRequested)
                        return _data;

                    dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                    dbdapater.Fill(gridDataSet, _tablename);

                    using (DataView dataView = new DataView(gridDataSet.Tables[0]))
                    {
                        foreach (DataRowView rowView in dataView)
                        {
                            if (ct.IsCancellationRequested)
                                break;
                            DateTime date = (DateTime)rowView[_timecolumnname];
                            _data.Add(new CustomElement()
                            {
                                ArgumentColumn = date,
                                ValueColumn = rowView[_valuecolumnname] is DBNull ? invalidPointsValue : System.Convert.ToDouble(rowView[_valuecolumnname]),
                                FilteringColumn = 1
                            });
                        }
                    }
                    return _data;
                }
            }, CommandTimeout, tokenSource.Token);
            listToWait.Add(task1);
            var task2 = task1.ContinueWith(ret =>
            {
                if (ct.IsCancellationRequested)
                    return;

                if (ret.IsFaulted && ret.Exception != null)
                {
                    ShowError(ret.Exception);
                    if (IsBlocking(ret.Exception))
                    {
                        log.Error(Name, ret.Exception);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, Properties.Resources.SparklineChart,
                             DateTime.UtcNow, $"{Name}: {ret.Exception.Message}",
                             System.Diagnostics.EventLogEntryType.Error);
                    }
                    else
                    {
                        var error = string.Format("{0}: {1}", Name, ret.Exception.InnerException != null ? ret.Exception.InnerException.Message : ret.Exception.Message);
                        log.Info(error);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, Properties.Resources.SparklineChart,
                         DateTime.UtcNow, $"{error}",
                         System.Diagnostics.EventLogEntryType.Information);
                    }
                }
                else
                {
                    HideError();
                    SourceCollection = new Collection<CustomElement>(_data);
                    InitRange();
                    sparklineEdit.EditValue = SourceCollection;
                    UpdateYGridLabelValues();
                }
            }, sc);

            if (listToWait.Count > 0)
            {
                try
                {
                    await Task.WhenAll(listToWait);
                }
                catch (Exception ex)
                {
                    if (IsBlocking(ex))
                        StopTimer();
                }
                listToWait.Clear();
            }
            bLoadingData = false;
        }
        bool IsBlocking(Exception ex)
        {
            if ((ex as System.Data.SqlClient.SqlException)?.Number == -2 || (ex as InvalidOperationException)?.HResult == -2146233079) //CommandTimeout exception - Connection timeout expired
                return false;
            return ex.InnerException == null || IsBlocking(ex.InnerException);
        }
        void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;
            }
        }
        private void ShowError(Exception exception)
        {
            if (bDisposed || mainGrid == null || statusText == null)
                return;

            var error = string.Format("{0}: {1}", Name, exception.InnerException != null ? exception.InnerException.Message : exception.Message);

            mainGrid.Effect = errorEffect;
            statusText.Text = error;
            statusText.Visibility = Visibility.Visible;
        }
        private void HideError()
        {
            if (bDisposed || mainGrid == null || statusText == null)
                return;

            mainGrid.Effect = null;
            statusText.Visibility = Visibility.Collapsed;
        }
        private void UpdateGridBrush()
        {
            if (gridArea == null)
                return;
            var toUpdateList = (from b in gridArea.Children.OfType<Border>() where b.Uid.StartsWith("major") || b.Uid.StartsWith("minor") select b).ToList();
            Brush brush = this.ReadLocalValue(GridBrushProperty) == DependencyProperty.UnsetValue ? Foreground : new SolidColorBrush(GridBrush);
            toUpdateList.ForEach(b => b.BorderBrush = brush);
        }

        private void InitMainBorder()
        {
            if (gridArea == null)
                return;
            Brush brush = this.ReadLocalValue(PlotBorderBrushProperty) == DependencyProperty.UnsetValue ? Background : new SolidColorBrush(PlotBorderBrush);
            mborder = new Border()
            {
                BorderBrush = brush,
                Opacity = 0.5,
                BorderThickness = new Thickness(1)
            };
            gridArea.Children.Add(mborder);
            Grid.SetColumnSpan(mborder, 2000);
            Grid.SetRowSpan(mborder, 2000);
        }
        private void UpdateMainBorder()
        {
            if (gridArea == null)
                return;
            if (mborder == null || !gridArea.Children.Contains(mborder))
                InitMainBorder();
            else
            {
                Brush brush = this.ReadLocalValue(PlotBorderBrushProperty) == DependencyProperty.UnsetValue ? Background : new SolidColorBrush(PlotBorderBrush);
                mborder.BorderBrush = brush;
            }
        }
        private void UpdateStyle()
        {
            if (sparklineEdit == null)
                return;
            LineSparklineStyleSettings sparkSettings = new LineSparklineStyleSettings()
            {
                LineWidth = 1,
                ShowMarkers = ShowMarkers,
                MarkerSize = 3,
                Brush = new SolidColorBrush(PenBrush),
                MarkerBrush = new SolidColorBrush(MarkerBrush),
                HighlightMaxPoint = false,
                HighlightMinPoint = false,
                HighlightStartPoint = false,
                HighlightEndPoint = false,
                HighlightNegativePoints = false
            };

            sparklineEdit.StyleSettings = sparkSettings;
        }
        private void GenerateData()
        {
            if (sparklineEdit == null)
                return;

            sparklineEdit.EditValue = null;
            if(SourceCollection == null)
            {
                SourceCollection = new Collection<CustomElement>();
                Random rnd = new Random();
                for (int i = 1; i <= 15; i++)
                {
                    SourceCollection.Add(new CustomElement()
                    {
                        ArgumentColumn = startDate.AddMinutes(15 * i),
                        ValueColumn = rnd.Next(20),
                        FilteringColumn = 1
                    });
                }
            }
            InitRange();
            sparklineEdit.EditValue = SourceCollection;
        }
        void UdateValues()
        {
            if (sparklineEdit == null)
                return;

            sparklineEdit.EditValue = null;
            sparklineEdit.EditValue = SourceCollection;
        }
        private void UpdateGrid()
        {
            if (plotArea == null || bDesignmode)
                return;
            var numPoints = System.Convert.ToInt32(Math.Abs(MaxValue - MinValue));
            if (numPoints == plotArea.RowDefinitions.Count)
                return;

            if (numPoints > plotArea.RowDefinitions.Count)
                for (int i = plotArea.RowDefinitions.Count + 1; i < numPoints; i++)
                    plotArea.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
            else
                for (int i = plotArea.RowDefinitions.Count; i > numPoints; i--)
                    plotArea.RowDefinitions.Remove(new System.Windows.Controls.RowDefinition());
        }
   
        void ManageGridDivisions()
        {
            if (gridArea == null)
                return;
            gridArea.ClipToBounds = false;
            gridArea.Children.Clear();
            gridArea.ColumnDefinitions.Clear();
            gridArea.RowDefinitions.Clear();
            double labelwidth = 0;
            double labelheight = 0;
            List<Label> xlabels = new List<Label>();
            
            var k = MajorXDivision > 0 ? SecondPerDateRange / MajorXDivision : SecondPerDateRange;
            for (int i = 0; i < MajorXDivision; i++)
            {
                gridArea.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
                Border border = new Border()
                {
                    Uid = $"majorXT{i}",
                    BorderBrush = new SolidColorBrush(GridBrush),
                    Opacity = 0.5,
                    BorderThickness = new Thickness(1, 0, 0, 0),
                };
                if(i > 0)
                {
                    TextBlock label = new TextBlock()
                    {
                        FontFamily = this.FontFamily,
                        FontSize = this.FontSize,
                        FontStyle = this.FontStyle,
                        FontWeight = this.FontWeight,
                        Uid = $"labelXT{i}",
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Visibility = ShowLabels ? Visibility.Visible : Visibility.Collapsed
                    };
                    label.Text = startDate.AddSeconds(k * i).ToString("HH:mm");
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size labelsize = GetLayoutSize(label);
                    label.Margin = new Thickness(-labelsize.Width / 2, 0, 0, -labelsize.Height);
                    gridArea.Children.Add(label);
                    Grid.SetColumn(label, i);
                    Grid.SetRowSpan(label, 2000);
                }

                gridArea.Children.Add(border);
                Grid.SetColumn(border, i);
                Grid.SetRowSpan(border, 2000);

                if (MinorXDivision > 0)
                {
                    Grid minorGrid = new Grid();
                    gridArea.Children.Add(minorGrid);
                    Grid.SetColumn(minorGrid, i);
                    Grid.SetRowSpan(minorGrid, 2000);
                    for (int j = 0; j < MinorXDivision; j++)
                    {
                        minorGrid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
                        Border maxborder = new Border()
                        {
                            Uid = $"minorXT{i}{j}",
                            BorderBrush = new SolidColorBrush(GridBrush),
                            Opacity = 0.2,
                            BorderThickness = new Thickness(1, 0, 0, 0),
                        };
                        minorGrid.Children.Add(maxborder);
                        Grid.SetColumn(maxborder, j);
                        Grid.SetRowSpan(maxborder, 2000);
                    }
                }
            }

            var maxValue = MaxValue;
            var minValue = MinValue;

            if (!bDesignmode)
            {
                if (AutomaticScale && SourceCollection != null && SourceCollection.Count > 0)
                {
                    var statDouble = (from c in SourceCollection.OfType<CustomElement>() select c.ValueColumn);
                    maxValue = statDouble.Max();
                    minValue = statDouble.Min();
                }
                MaxValue = maxValue;
                MinValue = minValue;
            }

            var kv = MajorYDivision > 0 ? (maxValue - minValue) / MajorYDivision : (maxValue - minValue);
            int ylenght = 0;
            for (int i = 0; i < MajorYDivision; i++)
            {
                gridArea.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
                Border border = new Border()
                {
                    Uid = $"majorYT{i}",
                    BorderBrush = new SolidColorBrush(GridBrush),
                    Opacity = 0.5,
                    BorderThickness = new Thickness(0, 1, 0, 0),
                };
                TextBlock label = new TextBlock()
                {
                    FontFamily = this.FontFamily,
                    FontSize = this.FontSize,
                    FontStyle = this.FontStyle,
                    FontWeight = this.FontWeight,
                    ClipToBounds = false,
                    Uid = $"labelYT{i}",
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Visibility = ShowLabels ? Visibility.Visible : Visibility.Collapsed
                };
                try
                {
                    label.Text = $"{string.Format(LabelFormat, (maxValue - kv * i))}";
                }
                catch (FormatException e) {
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.SparklineChart,
                             DateTime.UtcNow, $"{String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat)}",
                             System.Diagnostics.EventLogEntryType.Information);
                    log.Error(String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat));
                }
                if ((label.Text.Length) > ylenght)
                {
                    ylenght = label.ToString().Length;
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size labelsize = GetLayoutSize(label);
                    labelwidth = labelsize.Width;
                    labelheight = labelsize.Height;
                }
                label.Width = labelwidth;
                label.Height = labelheight;
                label.Margin = new Thickness(-labelwidth, -labelheight / 2, 0, 0);
                gridArea.Children.Add(label);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                gridArea.Children.Add(border);
                Grid.SetRow(border, i);
                Grid.SetColumnSpan(border, 2000);

                if (MinorYDivision > 0)
                {
                    Grid minorGrid = new Grid();
                    gridArea.Children.Add(minorGrid);
                    Grid.SetRow(minorGrid, i);
                    Grid.SetColumnSpan(minorGrid, 2000);
                    for (int j = 0; j < MinorYDivision; j++)
                    {
                        minorGrid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());
                        Border minborder = new Border()
                        {
                            Uid = $"minorYT{i}{j}",
                            BorderBrush = new SolidColorBrush(GridBrush),
                            Opacity = 0.2,
                            BorderThickness = new Thickness(0, 1, 0, 0),
                        };
                        minorGrid.Children.Add(minborder);
                        Grid.SetRow(minborder, j);
                        Grid.SetColumnSpan(minborder, 2000);
                    }
                }
            }
            TextBlock lylabel = new TextBlock()
            {
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                FontStyle = this.FontStyle,
                FontWeight = this.FontWeight,
                ClipToBounds = false,
                Uid = $"labelYT{MajorYDivision}",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Visibility = ShowLabels ? Visibility.Visible : Visibility.Collapsed
            };
            try
            {
                lylabel.Text = $"{string.Format(LabelFormat, minValue)}";
            }
            catch (FormatException e) {
                log.Error(String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat));
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.SparklineChart,
                         DateTime.UtcNow, $"{String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat)}",
                         System.Diagnostics.EventLogEntryType.Information);
            }
            if ((lylabel.Text.Length) > ylenght)
            {
                ylenght = lylabel.Text.Length;
                lylabel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size labelsize = GetLayoutSize(lylabel);
                labelwidth = labelsize.Width;
                labelheight = labelsize.Height;
            }
            lylabel.Width = labelwidth;
            lylabel.Height = labelheight;
            lylabel.Margin = new Thickness(-labelwidth, labelheight / 2, 0, 0);
            gridArea.Children.Add(lylabel);
            Grid.SetRow(lylabel, MajorYDivision);
            Grid.SetColumn(lylabel, 0);

            
            UpdateMainBorder();
            plotArea.Margin = ShowLabels ? new Thickness(labelwidth, 20, 20, labelheight) : new Thickness(0, 20, 20, 0);
        }

        void ManageOverlappingLabels(bool bwidth)
        {
            if (gridArea == null || mborder == null)
                return;
            string labelname = bwidth ? "labelXT" : "labelYT";

            Size mbordersize = GetLayoutSize(mborder);
            if ((mbordersize.Width == Double.NaN && bwidth) ||
                (mbordersize.Height == Double.NaN && !bwidth))
                return;

            var ToBeUpdated = (from c in gridArea.GetVisualChildrenOfType<TextBlock>() where c.Uid.StartsWith(labelname) select c).ToList();
            if (ToBeUpdated != null && ToBeUpdated.Count > 0)
            {
                var label = ToBeUpdated[0];
                label.FontFamily = this.FontFamily;
                label.FontSize = this.FontSize;
                label.FontStyle = this.FontStyle;
                label.FontWeight = this.FontWeight;
                label.Visibility = ShowLabels ? Visibility.Visible : Visibility.Collapsed;
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size labelsize = GetLayoutSize(label);
                //manage label overlapping
                var mainMeasure = bwidth ? mbordersize.Width : mbordersize.Height;
                var divMeasure = bwidth ? labelsize.Width : labelsize.Height;
                var div = bwidth ? (ToBeUpdated.Count * divMeasure * 2) : (ToBeUpdated.Count * divMeasure);
                bool bOverlapped = div == 0 ? false : mainMeasure <= div;
                int delta = 1;
                if (bOverlapped && bInit)
                {
                    var acc = ToBeUpdated.Count;
                    while (mainMeasure <= div)
                    {
                        acc = acc / 2;
                        delta++;
                        div = (acc * divMeasure * 2);
                    }
                }
                for (int i = 1; i < ToBeUpdated.Count; i++)
                {
                    ToBeUpdated[i].FontFamily = this.FontFamily;
                    ToBeUpdated[i].FontSize = this.FontSize;
                    ToBeUpdated[i].FontStyle = this.FontStyle;
                    ToBeUpdated[i].FontWeight = this.FontWeight;
                    ToBeUpdated[i].Visibility = ShowLabels && (!bOverlapped || bOverlapped && (i % delta) == 0) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        bool bRecalculating;
        void ManageOverlappingGridLabels()
        {
            if (bDisposed || gridArea == null || mborder == null || bRecalculating)
                return;
            bRecalculating = true;
            ManageOverlappingLabels(true);
            ManageOverlappingLabels(false);
            bRecalculating = false;
        }
        void UpdateYGridLabelValues()
        {
            if (gridArea == null)
                return;

            var _maxValue = MaxValue;
            var _minValue = MinValue;
            if(!bDesignmode)
            {
                if (AutomaticScale && SourceCollection != null && SourceCollection.Count > 0)
                {
                    var statDouble = (from c in SourceCollection.OfType<CustomElement>() select c.ValueColumn);
                    _maxValue = statDouble.Max();
                    _minValue = statDouble.Min();

                    MaxValue = _maxValue;
                    MinValue = _minValue;
                }
            }

            var kv = MajorYDivision > 0 ? (MaxValue - MinValue) / MajorYDivision : (MaxValue - MinValue);
            var yToBeUpdated = (from c in gridArea.GetVisualChildrenOfType<TextBlock>() where c.Uid.StartsWith("labelYT") select c).ToList();
            if (yToBeUpdated != null && yToBeUpdated.Count > 0)
            {
                try
                {
                    for (int i = 0; i < yToBeUpdated.Count - 1; i++)
                        yToBeUpdated[i].Text = $"{string.Format(LabelFormat, (MaxValue - kv * i))}";
                    yToBeUpdated[yToBeUpdated.Count - 1].Text = $"{string.Format(LabelFormat, MinValue)}";
                }
                catch (FormatException e) {
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.SparklineChart,
                         DateTime.UtcNow, $"{String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat)}",
                         System.Diagnostics.EventLogEntryType.Information);
                    log.Error(String.Format(Properties.Resources.LabelFormatError, Name, LabelFormat));
                }
            }
        }
        #endregion
        #region IDIsposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            DetachOverrideBaseProperties();
            base.SizeChanged -= SparklineChart_SizeChanged;
            SizeChangedInvoker = null;

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            StopTimer();

            if (tokenSource != null)
                tokenSource.Cancel();

            if (listToWait != null)
            {
                if (listToWait.Count > 0)
                {
                    try
                    {
                        var task = Task.WhenAll(listToWait);
                        task.Wait();
                    }
                    catch
                    { }
                    listToWait.Clear();
                }
            }

            if (tokenSource != null)
                tokenSource.Dispose();
            
            if (gridArea != null)
            {
                gridArea.Children.Clear();
                gridArea.ColumnDefinitions.Clear();
                gridArea.RowDefinitions.Clear();
            }

            if (plotArea != null)
            {
                plotArea.Children.Clear();
            }
            mborder = null;
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

                        IWorkspace workspace = null;
                        if (Document == null)
                            Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                        if (Document != null)
                            workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

                        // Defines Data Template for 'ConnectionStringProperty' dependency property.
                        var dt = new DataTemplate();
                        var factory = new FrameworkElementFactory(typeof(DataSourcePropertyEditor));
                        factory.SetValue(DataSourcePropertyEditor.UseXMLProperty, true);
                        factory.SetValue(DataSourcePropertyEditor.WorkspaceProperty, workspace);
                        dt.DataType = typeof(DataReaderModelXML);
                        dt.VisualTree = factory;
                        mapDataTemplates.Add(ControlDataSourceProperty, dt);

                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                        factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                        factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                        dt.DataType = typeof(int);
                        dt.VisualTree = factory;
                        mapDataTemplates.Add(MinorXDivisionProperty, dt);
                        mapDataTemplates.Add(MajorXDivisionProperty, dt);
                        mapDataTemplates.Add(MinorYDivisionProperty, dt);
                        mapDataTemplates.Add(MajorYDivisionProperty, dt);

                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(typeof(StringFormatPropertyEditor));
                        dt.DataType = typeof(String);
                        dt.VisualTree = factory;
                        mapDataTemplates.Add(LabelFormatProperty, dt);

                        if (workspace != null)
                        {
                            dt = new DataTemplate();
                            factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                            factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                            dt.DataType = typeof(String);
                            dt.VisualTree = factory;
                            mapDataTemplates.Add(ValueTitleProperty, dt);
                            mapDataTemplates.Add(ArgumentTitleProperty, dt);
                        }

                        return mapDataTemplates;
                    }
                }

        #endregion

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
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "LabelFormat")
            {
                try
                {
                    var test = $"{string.Format(LabelFormat, "test")}";
                }
                catch (FormatException e)
                {
                    return Properties.Resources.LabelFormatInlineError;
                }
            }
            return null;
        }
        #endregion
        #region IConnectionAware
        public string GetConnectionString()
        {
            DataReaderEditor.Converters.DataReaderModelConverter converter = new DataReaderEditor.Converters.DataReaderModelConverter();
            var connection = converter.Convert(ControlDataSource.ReaderModel, typeof(String), null, System.Globalization.CultureInfo.CurrentUICulture)?.ToString();
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(connection, doc?.rootBase);
        }
        #endregion
        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(ValueTitle))
                list.Add(ValueTitle);
            if (!string.IsNullOrEmpty(ArgumentTitle))
                list.Add(ArgumentTitle);
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (!string.IsNullOrEmpty(ValueTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(SparklineChart), ValueTitleProperty).DisplayName;
                map.Add(propertyName, ValueTitle);
            }
            if (!string.IsNullOrEmpty(ArgumentTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(SparklineChart), ArgumentTitleProperty).DisplayName;
                map.Add(propertyName, ArgumentTitle);
            }
            return map;
        }
        #endregion
    }
    public class CustomElement
    {
        public DateTime ArgumentColumn { get; set; }
        public double ValueColumn { get; set; }
        public int FilteringColumn { get; set; }
    }
}
