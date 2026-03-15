using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Converters;
using DevExpress.Xpf.Charts;
using Opc.Ua;
using OPCUAViewModel;
using ScreenSettings;
using ViewModelLib;
using Utilities;
using UFInterfaces;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using StringManager.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using Utilities.WPF;
using DynamicTagAwareHelper;
using TranslationHelpers;
using UIMsgBoxAlertService.ComponentService;
using log4net;
using WPFPenHelpers;
using System.Collections;
using System.Xml.Serialization;
using WPFUtilities;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using WPFUtilities.HistoricalHelpers;

namespace Trends
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl, IContainPropertyEditors, IDisposable, IEntityReference, IDynamicTagAware, IConnectionAware
        , IStringIDAware
    {

        #region DP

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Chart));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
            //OnForegroundChanged();
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Chart));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (/*bcInit &&*/ !bOverride)
            {
                var label = AxsisFontSettings.Clone();
                var title = TitleFonstSettings.Clone();

                label.FontFamily = FontFamily;
                title.FontFamily = FontFamily;

                AxsisFontSettings = label;
                TitleFonstSettings = title;
            }
        }
        bool bcInit;
        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (/*bcInit &&*/ !bOverride)
            {
                var label = AxsisFontSettings.Clone();
                var title = TitleFonstSettings.Clone();

                label.FontWeight = FontWeight;
                title.FontWeight = FontWeight;

                AxsisFontSettings = label;
                TitleFonstSettings = title;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (/*bcInit &&*/ !bOverride)
            {
                var label = AxsisFontSettings.Clone();
                var title = TitleFonstSettings.Clone();

                label.FontStyle = FontStyle;
                title.FontStyle = FontStyle;

                AxsisFontSettings = label;
                TitleFonstSettings = title;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (/*bcInit &&*/ !bOverride)
            {
                var label = AxsisFontSettings.Clone();
                var title = TitleFonstSettings.Clone();

                label.FontSize = (int)FontSize;
                title.FontSize = (int)FontSize;

                AxsisFontSettings = label;
                TitleFonstSettings = title;
            }
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                AxisLabelForeground = Foreground;
                TitleForeground = Foreground;
            }
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as Chart;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !IsManipulationEnabled && !bOverride)
            {
                TrendBackground = Background;
                PlotBackground = Background;
            }
        }
        #endregion
        #region DefToolbarHeight
        public static readonly DependencyProperty DefToolbarHeightProperty = DependencyProperty.Register("DefToolbarHeight", typeof(double), typeof(Chart), new UIPropertyMetadata(25d));
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

        #region Title

        public static readonly DependencyProperty XTitleProperty = DependencyProperty.Register("XTitle", typeof(string), typeof(Chart), new UIPropertyMetadata(Properties.Resources.ArgumentTitle, new PropertyChangedCallback(OnXTitleChanged), new CoerceValueCallback(OnCoerceXTitle)));

        private static object OnCoerceXTitle(DependencyObject o, object value)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
            {
                return Chart.OnCoerceXTitle((string)value);
            }
            else
                return value;
        }

        private static void OnXTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                Chart.OnXTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceXTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.          
            UpdateAxisTitles();
        }

        [Category("ChartOptions")]
        public string XTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(XTitleProperty);
            }
            set
            {
                SetValue(XTitleProperty, value);
            }
        }



        public static readonly DependencyProperty YTitleProperty = DependencyProperty.Register("YTitle", typeof(string), typeof(Chart), new UIPropertyMetadata(Properties.Resources.ValueTitle, new PropertyChangedCallback(OnYTitleChanged), new CoerceValueCallback(OnCoerceYTitle)));
        
        private static object OnCoerceYTitle(DependencyObject o, object value)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                return Chart.OnCoerceYTitle((string)value);
            else
                return value;
        }

        private static void OnYTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                Chart.OnYTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceYTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateAxisTitles();
        }

        [Category("ChartOptions")]
        public string YTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(YTitleProperty);
            }
            set
            {
                SetValue(YTitleProperty, value);
            }
        }
        
        #endregion
        #region EnableAnimation
        public static readonly DependencyProperty EnableAnimationProperty = DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(Chart),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableAnimationChanged), new CoerceValueCallback(OnCoerceEnableAnimation)));

        private static object OnCoerceEnableAnimation(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceEnableAnimation((bool)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnEnableAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnEnableAnimationChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        protected virtual bool OnCoerceEnableAnimation(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableAnimationChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                chart.AnimationMode = newValue && !RunningOnSlowPC ? ChartAnimationMode : ChartAnimationMode.Disabled;
            }
        }
        [Category("ChartOptions")]
        public bool EnableAnimation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        #endregion


        #region ChartAnimationMode
        public static readonly DependencyProperty ChartAnimationModeProperty = DependencyProperty.Register("ChartAnimationMode", typeof(ChartAnimationMode), typeof(Chart), new UIPropertyMetadata(ChartAnimationMode.OnLoad, new PropertyChangedCallback(OnChartAnimationModeChanged), new CoerceValueCallback(OnCoerceChartAnimationMode)));

        private static object OnCoerceChartAnimationMode(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceChartAnimationMode((ChartAnimationMode)value);
            else
                return value;
        }

        private static void OnChartAnimationModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
            if (control != null)
                control.OnChartAnimationModeChanged((ChartAnimationMode)e.OldValue, (ChartAnimationMode)e.NewValue);
        }

        protected virtual ChartAnimationMode OnCoerceChartAnimationMode(ChartAnimationMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnChartAnimationModeChanged(ChartAnimationMode oldValue, ChartAnimationMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public ChartAnimationMode ChartAnimationMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ChartAnimationMode)GetValue(ChartAnimationModeProperty);
            }
            set
            {
                SetValue(ChartAnimationModeProperty, value);
            }
        }

        #endregion


        #region MirrorHeight
        public static readonly DependencyProperty MirrorHeightProperty = DependencyProperty.Register("MirrorHeight", typeof(double), typeof(Chart), new UIPropertyMetadata(25.0, new PropertyChangedCallback(OnMirrorHeightChanged), new CoerceValueCallback(OnCoerceMirrorHeight)));

        private static object OnCoerceMirrorHeight(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceMirrorHeight((double)value);
            else
                return value;
        }

        private static void OnMirrorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
            if (control != null)
                control.OnMirrorHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMirrorHeight(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMirrorHeightChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && chart.Diagram is XYDiagram2D)
            {
                if (RunningOnSlowPC)
                    (chart.Diagram as XYDiagram2D).DefaultPane.MirrorHeight = 0;
                else
                    (chart.Diagram as XYDiagram2D).DefaultPane.MirrorHeight = newValue;
            }
        }
        [Category("ChartOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public double MirrorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MirrorHeightProperty);
            }
            set
            {
                SetValue(MirrorHeightProperty, value);
            }
        }

        #endregion


        #region PenList
        public static readonly DependencyProperty PenListProperty = DependencyProperty.Register("PenList", typeof(PenItemList), typeof(Chart), new UIPropertyMetadata(new PenItemList(), new PropertyChangedCallback(OnPenListChanged), new CoerceValueCallback(OnCoercePenList)));

        private static object OnCoercePenList(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoercePenList((PenItemList)value);
            else
                return value;

           
        }

        private static void OnPenListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnPenListChanged((PenItemList)e.OldValue, (PenItemList)e.NewValue);
        }

        protected virtual PenItemList OnCoercePenList(PenItemList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPenListChanged(PenItemList oldValue, PenItemList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(newValue != null)
            {
                //penReferenceList = new PenItemList();
                //(from c in PenList where c.TagReference != null select c).ToList().ForEach(c => penReferenceList.Add(c));

                if (bLoaded && bDesignmode)
                {
                    InitChart();
                }
            }
        }
        [Category("ChartOptions")]
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertPenItemList))]
        public PenItemList PenList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PenItemList)GetValue(PenListProperty);
            }
            set
            {
                SetValue(PenListProperty, value);
            }
        }
        #endregion

        #region SeriesLabelVisible

        public static readonly DependencyProperty SeriesLabelVisibleProperty = DependencyProperty.Register("SeriesLabelVisible", typeof (bool), typeof (Chart),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnSeriesLabelVisibleChanged), new CoerceValueCallback(OnCoerceSeriesLabelVisible)));

        private static object OnCoerceSeriesLabelVisible(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceSeriesLabelVisible((bool) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnSeriesLabelVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnSeriesLabelVisibleChanged((bool) e.OldValue, (bool) e.NewValue);
            }
        }

        protected virtual bool OnCoerceSeriesLabelVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeriesLabelVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLabels();
        }
        [Category("ChartOptions")]
        public bool SeriesLabelVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(SeriesLabelVisibleProperty); }
            set { SetValue(SeriesLabelVisibleProperty, value); }
        }

        #endregion


        #region LinkedPenName
        public static readonly DependencyProperty LinkedPenNameProperty = DependencyProperty.Register("LinkedPenName", typeof(string), typeof(Chart), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnLinkedPenNameChanged), new CoerceValueCallback(OnCoerceLinkedPenName)));

        private static object OnCoerceLinkedPenName(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceLinkedPenName((string)value);
            else
                return value;
        }

        private static void OnLinkedPenNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
            if (control != null)
                control.OnLinkedPenNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceLinkedPenName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLinkedPenNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public string LinkedPenName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(LinkedPenNameProperty);
            }
            set
            {
                SetValue(LinkedPenNameProperty, value);
            }
        }

        #endregion
        

        #region ResolveOverlappingMode

        public static readonly DependencyProperty ResolveLabelOverlappingModeProperty = DependencyProperty.Register("ResolveLabelOverlappingMode", typeof(ResolveOverlappingMode), typeof(Chart),
            new UIPropertyMetadata(ResolveOverlappingMode.None, new PropertyChangedCallback(OnResolveLabelOverlappingModeChanged), new CoerceValueCallback(OnCoerceResolveLabelOverlappingMode)));

        private static object OnCoerceResolveLabelOverlappingMode(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceResolveLabelOverlappingMode((ResolveOverlappingMode)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnResolveLabelOverlappingModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnResolveLabelOverlappingModeChanged((ResolveOverlappingMode)e.OldValue, (ResolveOverlappingMode)e.NewValue);
            }
        }

        protected virtual ResolveOverlappingMode OnCoerceResolveLabelOverlappingMode(ResolveOverlappingMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnResolveLabelOverlappingModeChanged(ResolveOverlappingMode oldValue, ResolveOverlappingMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLabels();
        }
        [Category("ChartOptions")]
        public ResolveOverlappingMode ResolveLabelOverlappingMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (ResolveOverlappingMode)GetValue(ResolveLabelOverlappingModeProperty); }
            set { SetValue(ResolveLabelOverlappingModeProperty, value); }
        }



        #endregion

        #region TitleForeground
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnTitleForegroundChanged), new CoerceValueCallback(OnCoerceTitleForeground)));

        private static object OnCoerceTitleForeground(DependencyObject o, object value)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                return Chart.OnCoerceTitleForeground((Brush)value);
            else
                return value;
        }

        private static void OnTitleForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                Chart.OnTitleForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTitleForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateControlLayout();
            }
        }

        [Category("ChartOptions")]
        public Brush TitleForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(TitleForegroundProperty);
            }
            set
            {
                SetValue(TitleForegroundProperty, value);
            }
        }
        #endregion
        #region AxisLabelForeground
        public static readonly DependencyProperty AxisLabelForegroundProperty = DependencyProperty.Register("AxisLabelForeground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnAxisLabelForegroundChanged), new CoerceValueCallback(OnCoerceAxisLabelForeground)));

        private static object OnCoerceAxisLabelForeground(DependencyObject o, object value)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                return Chart.OnCoerceAxisLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnAxisLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                Chart.OnAxisLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceAxisLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAxisLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateControlLayout();
            }
        }
        [Category("ChartOptions")]
        public Brush AxisLabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(AxisLabelForegroundProperty);
            }
            set
            {
                SetValue(AxisLabelForegroundProperty, value);
            }
        }

        #endregion
       
        #region PlotBackground
        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnPlotBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoercePlotBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPlotBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateControlLayout();
            }
        }
        [Category("ChartOptions")]
         public Brush PlotBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(PlotBackgroundProperty);
            }
            set
            {
                SetValue(PlotBackgroundProperty, value);
            }
        }
        #endregion
        #region TrendBackground
        public static readonly DependencyProperty TrendBackgroundProperty = DependencyProperty.Register("TrendBackground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnTrendBackgroundChanged), new CoerceValueCallback(OnCoerceTrendBackground)));

        private static object OnCoerceTrendBackground(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceTrendBackground((Brush)value);
            else
                return value;
        }

        private static void OnTrendBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnTrendBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTrendBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTrendBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateControlLayout();
            }
        }

        [Category("ChartOptions")]
        public Brush TrendBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(TrendBackgroundProperty);
            }
            set
            {
                SetValue(TrendBackgroundProperty, value);
            }
        }
        #endregion

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("ChartOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(Chart), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("ChartOptions")]
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

        #region TitleFonstSettings
        public static readonly DependencyProperty TitleFonstSettingsProperty = DependencyProperty.Register("TitleFonstSettings", typeof(FontSettings), typeof(Chart), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnTitleFonstSettingsChanged), new CoerceValueCallback(OnCoerceTitleFonstSettings)));

        private static object OnCoerceTitleFonstSettings(DependencyObject o, object value)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                return Chart.OnCoerceTitleFonstSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnTitleFonstSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart Chart = o as Chart;
            if (Chart != null)
                Chart.OnTitleFonstSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceTitleFonstSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleFonstSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);
            if (bInit)
                UpdateTitleFonstSettings(newValue);
        }

        void UpdateTitleFonstSettings(FontSettings newValue)
        {
            AxisTitle titlex = null;
            AxisTitle titley = null;
            if (chart.Diagram is XYDiagram3D)
            {
                titlex = (chart.Diagram as XYDiagram3D).AxisX.Title;
                titley = (chart.Diagram as XYDiagram3D).AxisY.Title;
            }
            else if (chart.Diagram is XYDiagram2D)
            {
                titlex = (chart.Diagram as XYDiagram2D).AxisX.Title;
                titley = (chart.Diagram as XYDiagram2D).AxisY.Title;
            }
            if (titlex != null)
            {
                titlex.FontSize = newValue.FontSize;
                titlex.FontFamily = newValue.FontFamily;
                titlex.FontWeight = newValue.FontWeight;
                titlex.FontStyle = newValue.FontStyle;
            }
            if (titley != null)
            {
                titley.FontSize = newValue.FontSize;
                titley.FontFamily = newValue.FontFamily;
                titley.FontWeight = newValue.FontWeight;
                titley.FontStyle = newValue.FontStyle;
            }
        }

        [Category("ChartOptions")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings TitleFonstSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(TitleFonstSettingsProperty);
            }
            set
            {
                SetValue(TitleFonstSettingsProperty, value);
            }
        }
        #endregion
        #region AxisFontSettings
        public static readonly DependencyProperty AxsisFontSettingsProperty = DependencyProperty.Register("AxsisFontSettings", typeof(FontSettings), typeof(Chart), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceAxsisFontSettings)));

        private static object OnCoerceAxsisFontSettings(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceAxsisFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnAxsisFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceAxsisFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAxsisFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateAxsisFontSettings(newValue);
            }
        }
        void UpdateAxsisFontSettings(FontSettings newValue)
        {
            foreach (var c in chart.Diagram.Series)
            {
                InitSeriesFont(c, newValue);
            }
            AxisLabel labelx = null;
            AxisLabel labely = null;
            if (chart.Diagram is XYDiagram3D)
            {
                labelx = (chart.Diagram as XYDiagram3D).AxisX.Label;
                labely = (chart.Diagram as XYDiagram3D).AxisY.Label;
            }
            else if (chart.Diagram is XYDiagram2D)
            {
                labelx = (chart.Diagram as XYDiagram2D).AxisX.Label;
                labely = (chart.Diagram as XYDiagram2D).AxisY.Label;
            }
            if (labelx != null)
            {
                labelx.FontSize = newValue.FontSize;
                labelx.FontFamily = newValue.FontFamily;
                labelx.FontWeight = newValue.FontWeight;
                labelx.FontStyle = newValue.FontStyle;
            }
            if (labely != null)
            {
                labely.FontSize = newValue.FontSize;
                labely.FontFamily = newValue.FontFamily;
                labely.FontWeight = newValue.FontWeight;
                labely.FontStyle = newValue.FontStyle;
            }
        }
        bool bOverride;
        void UpdateValueFont(FontSettings newValue)
        {
            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;
        }

        private void InitSeriesFont(Series c, FontSettings newValue)
        {
            c.FontSize = newValue.FontSize;
            c.FontFamily = newValue.FontFamily;
            c.FontWeight = newValue.FontWeight;
            c.FontStyle = newValue.FontStyle;
        }
        [Category("ChartOptions")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings AxsisFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(AxsisFontSettingsProperty);
            }
            set
            {
                SetValue(AxsisFontSettingsProperty, value);
            }
        }
        #endregion

        #region Rotated
        public static readonly DependencyProperty RotatedProperty = DependencyProperty.Register("Rotated", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRotatedChanged), new CoerceValueCallback(OnCoerceRotated)));

        private static object OnCoerceRotated(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceRotated((bool)value);
            else
                return value;
        }

        private static void OnRotatedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnRotatedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceRotated(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRotatedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public bool Rotated
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(RotatedProperty);
            }
            set
            {
                SetValue(RotatedProperty, value);
            }
        }
        #endregion
        #region LogarithmicYScale
        public static readonly DependencyProperty LogarithmicYScaleProperty = DependencyProperty.Register("LogarithmicYScale", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLogarithmicYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicYScale)));

        private static object OnCoerceLogarithmicYScale(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceLogarithmicYScale((bool)value);
            else
                return value;
        }

        private static void OnLogarithmicYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnLogarithmicYScaleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceLogarithmicYScale(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLogarithmicYScaleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public bool LogarithmicYScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(LogarithmicYScaleProperty);
            }
            set
            {
                SetValue(LogarithmicYScaleProperty, value);
            }
        }

        public static readonly DependencyProperty LogarithmicBaseYScaleProperty = DependencyProperty.Register("LogarithmicBaseYScale", typeof(double), typeof(Chart), new UIPropertyMetadata(10.0, new PropertyChangedCallback(OnLogarithmicBaseYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicBaseYScale)));

        private static object OnCoerceLogarithmicBaseYScale(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceLogarithmicBaseYScale((double)value);
            else
                return value;
        }

        private static void OnLogarithmicBaseYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnLogarithmicBaseYScaleChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceLogarithmicBaseYScale(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLogarithmicBaseYScaleChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double LogarithmicBaseYScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(LogarithmicBaseYScaleProperty);
            }
            set
            {
                SetValue(LogarithmicBaseYScaleProperty, value);
            }
        }
        #endregion
        #region LegendAreaVisible
        public static readonly DependencyProperty LegendAreaVisibleProperty = DependencyProperty.Register("LegendAreaVisible", typeof(Boolean), typeof(Chart), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLegendAreaVisibleChanged), new CoerceValueCallback(OnCoerceLegendAreaVisible)));

        private static object OnCoerceLegendAreaVisible(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceLegendAreaVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLegendAreaVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnLegendAreaVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLegendAreaVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendAreaVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
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
        #region LegendMargin

        public static readonly DependencyProperty LegendMarginProperty = DependencyProperty.Register("LegendMargin", typeof(Thickness), typeof(Chart),
            new UIPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnLegendMarginChanged), new CoerceValueCallback(OnCoerceLegendMargin)));

        private static object OnCoerceLegendMargin(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceLegendMargin((Thickness)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnLegendMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnLegendMarginChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
            }
        }

        protected virtual Thickness OnCoerceLegendMargin(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendMarginChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public Thickness LegendMargin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (Thickness)GetValue(LegendMarginProperty); }
            set { SetValue(LegendMarginProperty, value); }
        }

        #endregion
        #region LegendHorizontalPosition

        public static readonly DependencyProperty LegendHorizontalPositionProperty = DependencyProperty.Register("LegendHorizontalPosition", typeof (HorizontalPosition), typeof (Chart),
            new UIPropertyMetadata(HorizontalPosition.Left, new PropertyChangedCallback(OnLegendHorizontalPositionChanged), new CoerceValueCallback(OnCoerceLegendHorizontalPosition)));

        private static object OnCoerceLegendHorizontalPosition(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceLegendHorizontalPosition((HorizontalPosition) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnLegendHorizontalPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnLegendHorizontalPositionChanged((HorizontalPosition) e.OldValue, (HorizontalPosition) e.NewValue);
            }
        }

        protected virtual HorizontalPosition OnCoerceLegendHorizontalPosition(HorizontalPosition value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendHorizontalPositionChanged(HorizontalPosition oldValue, HorizontalPosition newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public HorizontalPosition LegendHorizontalPosition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (HorizontalPosition) GetValue(LegendHorizontalPositionProperty); }
            set { SetValue(LegendHorizontalPositionProperty, value); }
        }

        #endregion
        #region LegendVerticalPosition

        public static readonly DependencyProperty LegendVerticalPositionProperty = DependencyProperty.Register("LegendVerticalPosition", typeof (VerticalPosition), typeof (Chart),
            new UIPropertyMetadata(VerticalPosition.Top, new PropertyChangedCallback(OnLegendVerticalPositionChanged), new CoerceValueCallback(OnCoerceLegendVerticalPosition)));

        private static object OnCoerceLegendVerticalPosition(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceLegendVerticalPosition((VerticalPosition) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnLegendVerticalPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnLegendVerticalPositionChanged((VerticalPosition) e.OldValue, (VerticalPosition) e.NewValue);
            }
        }

        protected virtual VerticalPosition OnCoerceLegendVerticalPosition(VerticalPosition value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendVerticalPositionChanged(VerticalPosition oldValue, VerticalPosition newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public VerticalPosition LegendVerticalPosition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (VerticalPosition) GetValue(LegendVerticalPositionProperty); }
            set { SetValue(LegendVerticalPositionProperty, value); }
        }


        #endregion


        #region PointPrecision
        public static readonly DependencyProperty PointPrecisionProperty = DependencyProperty.Register("PointPrecision", typeof(int), typeof(Chart), new UIPropertyMetadata(2));
        public int PointPrecision
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(PointPrecisionProperty);
            }
            set
            {
                SetValue(PointPrecisionProperty, value);
            }
        }
        #endregion


        #region LegendMaxHeight
        public static readonly DependencyProperty LegendMaxHeightProperty = DependencyProperty.Register("LegendMaxHeight", typeof(double), typeof(Chart), new UIPropertyMetadata(200.0, new PropertyChangedCallback(OnLegendMaxHeightChanged), new CoerceValueCallback(OnCoerceLegendMaxHeight)));

        private static object OnCoerceLegendMaxHeight(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceLegendMaxHeight((double)value);
            else
                return value;
        }

        private static void OnLegendMaxHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
            if (control != null)
                control.OnLegendMaxHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceLegendMaxHeight(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendMaxHeightChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double LegendMaxHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(LegendMaxHeightProperty);
            }
            set
            {
                SetValue(LegendMaxHeightProperty, value);
            }
        }

        #endregion
        #region SampleNumber
        public static readonly DependencyProperty SampleNumberProperty = DependencyProperty.Register("SampleNumber", typeof(int), typeof(Chart), new UIPropertyMetadata(10, new PropertyChangedCallback(OnSampleNumberChanged), new CoerceValueCallback(OnCoerceSampleNumber)));

        private static object OnCoerceSampleNumber(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceSampleNumber((int)value);
            else
                return value;
        }

        private static void OnSampleNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnSampleNumberChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSampleNumber(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSampleNumberChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bDesignmode)
            {
                InitChart();
            }
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
        #region Minimum and Maximum
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(Chart), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        private static object OnCoerceMaximum(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceMaximum((double)value);
            else
                return value;
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMaximum(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaximumChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double Maximum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }


        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(Chart), new UIPropertyMetadata(-1.0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        private static object OnCoerceMinimum(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceMinimum((double)value);
            else
                return value;
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinimum(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinimumChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double Minimum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }
        #endregion
        #region UseEUMinMaxValues
        public static readonly DependencyProperty UseEUMinMaxValuesProperty = DependencyProperty.Register("UseEUMinMaxValues", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseEUMinMaxValuesChanged), new CoerceValueCallback(OnCoerceUseEUMinMaxValues)));

        private static object OnCoerceUseEUMinMaxValues(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceUseEUMinMaxValues((bool)value);
            else
                return value;
        }

        private static void OnUseEUMinMaxValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnUseEUMinMaxValuesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseEUMinMaxValues(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseEUMinMaxValuesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public bool UseEUMinMaxValues
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseEUMinMaxValuesProperty);
            }
            set
            {
                SetValue(UseEUMinMaxValuesProperty, value);
            }
        }

        #endregion
        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(Chart), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnConnectionStringChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionString(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectionStringChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bInit && !string.IsNullOrEmpty(newValue))
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
        [Browsable(false)]
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
        #region Enable3D
        public static readonly DependencyProperty Enable3DProperty = DependencyProperty.Register("Enable3D", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnable3DChanged), new CoerceValueCallback(OnCoerceEnable3D)));

        private static object OnCoerceEnable3D(DependencyObject o, object value)
        {
            Chart chart = o as Chart;
            if (chart != null)
                return chart.OnCoerceEnable3D((bool)value);
            else
                return value;
        }

        private static void OnEnable3DChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart chart = o as Chart;
            if (chart != null)
                chart.OnEnable3DChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceEnable3D(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnable3DChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded)
            {
                if (bDesignmode)
                {
                    InitChart();
                }
                else
                {
                    RefreshSettings();
                    if (chart != null && chart.Diagram is Diagram3D && !bWheeling)
                        (chart.Diagram as Diagram3D).ZoomPercent = Zoom3DPercent;
                }
            }
        }
        [Category("ChartOptions")]
        public bool Enable3D
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(Enable3DProperty);
            }
            set
            {
                SetValue(Enable3DProperty, value);
            }
        }
        #endregion
        #region SimpleDiagram

        public static readonly DependencyProperty SimpleDiagramProperty = DependencyProperty.Register("SimpleDiagram", typeof (bool), typeof (Chart),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnSimpleDiagramChanged), new CoerceValueCallback(OnCoerceSimpleDiagram)));

        private static object OnCoerceSimpleDiagram(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceSimpleDiagram((bool) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnSimpleDiagramChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnSimpleDiagramChanged((bool) e.OldValue, (bool) e.NewValue);
            }
        }

        protected virtual bool OnCoerceSimpleDiagram(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSimpleDiagramChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded)
            {
                if (bDesignmode)
                {
                    InitChart();
                }
                else
                {
                    RefreshSettings();
                }
            }
        }
        [Category("ChartOptions")]
        public bool SimpleDiagram
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(SimpleDiagramProperty); }
            set { SetValue(SimpleDiagramProperty, value); }
        }

        public bool Refresh()
        {
            if (bDisposed || bDesignmode || !bLoaded || !bInit || !bcInit)
                return false;

            RefreshSettings();
            return true;
        }

        void RefreshSettings()
        {
            if (bInit)
            {
                UpdateChart();
            }
            if (bMonitoredInit)
            {
                AddMonitoredSeries();
            }
            RefreshChart();
        }
        #endregion

        #region RotationAngle

        //public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof (double), typeof (Chart),
        //    new UIPropertyMetadata(-30.0, new PropertyChangedCallback(OnRotationAngleChanged), new CoerceValueCallback(OnCoerceRotationAngle)));

        //private static object OnCoerceRotationAngle(DependencyObject o, object value)
        //{
        //    var control = o as Chart;
        //    if (control != null)
        //    {
        //        return control.OnCoerceRotationAngle((double) value);
        //    }
        //    else
        //    {
        //        return value;
        //    }
        //}

        //private static void OnRotationAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = o as Chart;
        //    if (control != null)
        //    {
        //        control.OnRotationAngleChanged((double) e.OldValue, (double) e.NewValue);
        //    }
        //}

        //protected virtual double OnCoerceRotationAngle(double value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnRotationAngleChanged(double oldValue, double newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("Chart3DOptions")]
        //public double RotationAngle
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get { return (double) GetValue(RotationAngleProperty); }
        //    set { SetValue(RotationAngleProperty, value); }
        //}

        #endregion
        #region Zoom3DPercent

        public static readonly DependencyProperty Zoom3DPercentProperty = DependencyProperty.Register("Zoom3DPercent", typeof (double), typeof (Chart),
            new UIPropertyMetadata(130.0, new PropertyChangedCallback(OnZoom3DPercentChanged), new CoerceValueCallback(OnCoerceZoom3DPercent)));

        private static object OnCoerceZoom3DPercent(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceZoom3DPercent((double) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnZoom3DPercentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnZoom3DPercentChanged((double) e.OldValue, (double) e.NewValue);
            }
        }

        protected virtual double OnCoerceZoom3DPercent(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnZoom3DPercentChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bDesignmode)
            {
                if (bInit && !bDesignmode)
                    WriteValue(zoom, newValue);
                if (chart != null && chart.Diagram is Diagram3D && !bWheeling)
                    (chart.Diagram as Diagram3D).ZoomPercent = newValue;
            }
            //bUpdating = false;
            bWheeling = false;
        }
        [Category("Chart3DOptions")]
        public double Zoom3DPercent
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (double) GetValue(Zoom3DPercentProperty); }
            set { SetValue(Zoom3DPercentProperty, value); }
        }
        #endregion
        #region Advanced
        #region Push

        public static readonly DependencyProperty PushProperty = DependencyProperty.Register("Push", typeof(OPCUAXMLEntityReference), typeof(Chart),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnPushChanged), new CoerceValueCallback(OnCoercePush)));

        private static object OnCoercePush(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoercePush((OPCUAXMLEntityReference)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnPushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnPushChanged((OPCUAXMLEntityReference)e.OldValue, (OPCUAXMLEntityReference)e.NewValue);
            }
        }

        protected virtual OPCUAXMLEntityReference OnCoercePush(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPushChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartAdvancedOptions")]
        [DisplayNameExtension]
        public OPCUAXMLEntityReference Push
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (OPCUAXMLEntityReference)GetValue(PushProperty); }
            set { SetValue(PushProperty, value); }
        }

        #endregion
        #region Reset

        public static readonly DependencyProperty ResetProperty = DependencyProperty.Register("Reset", typeof(OPCUAXMLEntityReference), typeof(Chart),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnResetChanged), new CoerceValueCallback(OnCoerceReset)));

        private static object OnCoerceReset(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceReset((OPCUAXMLEntityReference)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnResetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnResetChanged((OPCUAXMLEntityReference)e.OldValue, (OPCUAXMLEntityReference)e.NewValue);
            }
        }

        protected virtual OPCUAXMLEntityReference OnCoerceReset(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnResetChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartAdvancedOptions")]
        [DisplayNameExtension]
        public OPCUAXMLEntityReference Reset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (OPCUAXMLEntityReference)GetValue(ResetProperty); }
            set { SetValue(ResetProperty, value); }
        }

        #endregion
        #region Zoom

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof (OPCUAXMLEntityReference), typeof (Chart),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnZoomChanged), new CoerceValueCallback(OnCoerceZoom)));

        private static object OnCoerceZoom(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceZoom((OPCUAXMLEntityReference) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnZoomChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnZoomChanged((OPCUAXMLEntityReference) e.OldValue, (OPCUAXMLEntityReference) e.NewValue);
            }
        }

        protected virtual OPCUAXMLEntityReference OnCoerceZoom(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnZoomChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartAdvancedOptions")]
        [DisplayNameExtension]
        public OPCUAXMLEntityReference Zoom
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (OPCUAXMLEntityReference) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        #endregion
        #region Rotation

        //public static readonly DependencyProperty RotationProperty = DependencyProperty.Register("Rotation", typeof (OPCUAXMLEntityReference), typeof (Chart),
        //    new UIPropertyMetadata(null, new PropertyChangedCallback(OnRotationChanged), new CoerceValueCallback(OnCoerceRotation)));

        //private static object OnCoerceRotation(DependencyObject o, object value)
        //{
        //    var control = o as Chart;
        //    if (control != null)
        //    {
        //        return control.OnCoerceRotation((OPCUAXMLEntityReference) value);
        //    }
        //    else
        //    {
        //        return value;
        //    }
        //}

        //private static void OnRotationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    var control = o as Chart;
        //    if (control != null)
        //    {
        //        control.OnRotationChanged((OPCUAXMLEntityReference) e.OldValue, (OPCUAXMLEntityReference) e.NewValue);
        //    }
        //}

        //protected virtual OPCUAXMLEntityReference OnCoerceRotation(OPCUAXMLEntityReference value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnRotationChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("ChartAdvancedOptions")]
        //public OPCUAXMLEntityReference Rotation
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get { return (OPCUAXMLEntityReference) GetValue(RotationProperty); }
        //    set { SetValue(RotationProperty, value); }
        //}

        #endregion
        #region AllowRuntimeChanges

        public static readonly DependencyProperty AllowRuntimeChangesProperty = DependencyProperty.Register("AllowRuntimeChanges", typeof (bool), typeof (Chart),
            new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRuntimeChangesChanged), new CoerceValueCallback(OnCoerceAllowRuntimeChanges)));

        private static object OnCoerceAllowRuntimeChanges(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceAllowRuntimeChanges((bool) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnAllowRuntimeChangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnAllowRuntimeChangesChanged((bool) e.OldValue, (bool) e.NewValue);
            }
        }

        protected virtual bool OnCoerceAllowRuntimeChanges(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRuntimeChangesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bLoaded)
                toolbar.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }
        [Category("ChartAdvancedOptions")]
        public bool AllowRuntimeChanges
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(AllowRuntimeChangesProperty); }
            set { SetValue(AllowRuntimeChangesProperty, value); }
        }

        #endregion
        #region AutoHideToolbar

        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof (bool), typeof (Chart),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            var control = o as Chart;
            if (control != null)
            {
                return control.OnCoerceAutoHideToolbar((bool) value);
            }
            else
            {
                return value;
            }
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Chart;
            if (control != null)
            {
                control.OnAutoHideToolbarChanged((bool) e.OldValue, (bool) e.NewValue);
            }
        }

        protected virtual bool OnCoerceAutoHideToolbar(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoHideToolbarChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit && newValue != oldValue)
            {
                if (bDisposed)
                    return;

                if (newValue)
                {
                    toolbar_MouseLeave(null, null);
                    Grid.SetRow(chart, 0);
                    Grid.SetRowSpan(chart, 2);
                }
                else
                {
                    toolbar_MouseEnter(null, null);
                    Grid.SetRow(chart, 1);
                    Grid.SetRowSpan(chart, 1);
                }
            }
        }
        [Category("ChartAdvancedOptions")]
        public bool AutoHideToolbar
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(AutoHideToolbarProperty); }
            set { SetValue(AutoHideToolbarProperty, value); }
        }

        #endregion

        #region AutomaticScale
        public static readonly DependencyProperty AutomaticScaleProperty = DependencyProperty.Register("AutomaticScale", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutomaticScaleChanged), new CoerceValueCallback(OnCoerceAutomaticScale)));

        private static object OnCoerceAutomaticScale(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceAutomaticScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
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
            if (bLoaded)
            {
                if (bDesignmode)
                {
                    InitChart();
                }
                else
                {
                    if (bInit)
                    {
                        UpdateChart();
                        //InitValues();
                    }
                    if (bMonitoredInit)
                    {
                        AddMonitoredSeries();
                    }
                }
            }
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
        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(Chart), new UIPropertyMetadata(0));
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


        #region ShowPercentValuesOnPieSerie
        public static readonly DependencyProperty ShowPercentValuesOnPieSerieProperty = DependencyProperty.Register("ShowPercentValuesOnPieSerie", typeof(bool), typeof(Chart), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowPercentValuesOnPieSerieChanged), new CoerceValueCallback(OnCoerceShowPercentValuesOnPieSerie)));

        private static object OnCoerceShowPercentValuesOnPieSerie(DependencyObject o, object value)
        {
            Chart control = o as Chart;
            if (control != null)
                return control.OnCoerceShowPercentValuesOnPieSerie((bool)value);
            else
                return value;
        }

        private static void OnShowPercentValuesOnPieSerieChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Chart control = o as Chart;
            if (control != null)
                control.OnShowPercentValuesOnPieSerieChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowPercentValuesOnPieSerie(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowPercentValuesOnPieSerieChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded) 
                UpdatePieSeries(newValue);
        }

        private void UpdatePieSeries(bool showPercentValuesOnPieSerie)
        {
            if (dataContextSerie != null)
                UpdatePieSerie(dataContextSerie, showPercentValuesOnPieSerie);
            mapKeySeries.Values.ToList().ForEach(serie => UpdatePieSerie(serie, showPercentValuesOnPieSerie));
        }

        void UpdatePieSerie(Series serie, bool showPercentValuesOnPieSerie)
        {
            string pointOption = "pointOption";
            if (serie != null)
            {
                if (SimpleDiagram)
                    pointOption = showPercentValuesOnPieSerie ? "percentPointOption" : "pointOption";
                else
                    pointOption = "pointOption";

                serie.PointOptions = TryFindResource(pointOption) as PointOptions;
            }
        }

        public bool ShowPercentValuesOnPieSerie
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowPercentValuesOnPieSerieProperty);
            }
            set
            {
                SetValue(ShowPercentValuesOnPieSerieProperty, value);
            }
        }

        #endregion


        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.ChartSmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(Chart), new UIPropertyMetadata(false));

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
        public bool RunningOnSlowPC
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnSlowPC(this);
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
        List<string> matchChangedMap = new List<string>();
        IDictionary<String, String> stringlist;
        bool bLoaded;
        bool bDesignmode;
        IUFProjectManager iUFProjectManager;
        int iCount;
        string monitoredKey;
        int sampleNumber;
        IWorkspace workspace;
        IStringEditorManager stringeditorManager;
        ScreenDocument Document;
        TypeHelper typeHelper = new TypeHelper();
        //bool bUpdating;
        bool bWheeling;

        ChartDataGenerator viewDataContext;
        MonitoredItemViewModel monitoredItemViewModel;
        Dictionary<string, Series> mapKeySeries = new Dictionary<string, Series>();
        Dictionary<string, ChartDataGenerator> viewList = new Dictionary<string, ChartDataGenerator>();
        private OPCUAEntityReference push;
        private OPCUAEntityReference reset;
        private OPCUAEntityReference zoom;

        Dictionary<string, PenItemHelper> mapHandlers;

        DispatcherOperation dpUpdateValue;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.ChartControlLog);

        PenItemList penReferenceList;
        private double ToolTipOffset = 2.0;
        PenItemList PenReferenceList
        {
            get
            {
                if (penReferenceList == null)
                {
                    penReferenceList = new PenItemList();
                    (from c in PenList where c.TagReference != null select c).ToList().ForEach(c => penReferenceList.Add(c));
                }

                return penReferenceList;
            }
        }
        Dictionary<string, OPCUAEntityReference> opcuaEntityReference;
        private Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                //if (opcuaEntityReference == null)
                //{
                //    opcuaEntityReference = new Dictionary<int, OPCUAEntityReference>();
                //    foreach (PenItem item in PenReferenceList)
                //    {
                //        opcuaEntityReference[PenReferenceList.IndexOf(item)] = item.TagReference;
                //    }
                //}
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    for (int i = 0; i < PenReferenceList.Count(); i++)
                    {
                        opcuaEntityReference[PenReferenceList[i].NodeId] = PenReferenceList[i].TagReference;
                    }
                }

                return opcuaEntityReference;
            }
        }
        #endregion
        #region methods
        private void diagram_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            XYDiagram2D diagram = sender as XYDiagram2D;
            Pane pane = diagram.DefaultPane;
            if (e.Delta > 0)
            {
                pane.AxisXScrollBarOptions.Visible = true;
                pane.AxisYScrollBarOptions.Visible = true;
            }
            else if (e.Delta < 0)
            {
                if (diagram.CanZoomOut())
                {
                    pane.AxisXScrollBarOptions.Visible = true;
                    pane.AxisYScrollBarOptions.Visible = true;
                }
                else
                {
                    pane.AxisXScrollBarOptions.Visible = false;
                    pane.AxisYScrollBarOptions.Visible = false;
                }
            }
        }

        private void diagram_Zoom(object sender, XYDiagram2DZoomEventArgs e)
        {
            XYDiagram2D diagram = sender as XYDiagram2D;
            Pane pane = diagram.DefaultPane;
            if (e.Type == XYDiagram2DZoomEventType.ZoomIn)
            {
                pane.AxisXScrollBarOptions.Visible = true;
                pane.AxisYScrollBarOptions.Visible = true;
            }
            else if (e.Type == XYDiagram2DZoomEventType.ZoomOut)
            {
                if (diagram.CanZoomOut())
                {
                    pane.AxisXScrollBarOptions.Visible = true;
                    pane.AxisYScrollBarOptions.Visible = true;
                }
                else
                {
                    pane.AxisXScrollBarOptions.Visible = false;
                    pane.AxisYScrollBarOptions.Visible = false;
                }
            }
        }

        private void chart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (chart!= null && chart.Diagram is Diagram3D)
            {
                bWheeling = true;
                Zoom3DPercent = (chart.Diagram as Diagram3D).ZoomPercent;
            }
        }
        private void WriteValue(OPCUAEntityReference tag, double value)
        {
            if(tag != null && tag.MonitoredItemViewModel != null && !bDesignmode)
            {
                try
                {
                    tag.MonitoredItemViewModel.WriteValue(value);
                }
                catch (Exception ex)
                {
                    log.Error($"{Properties.Resources.ErrorOnZooming} {ex.Message}");
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
                        DateTime.UtcNow, $"{Properties.Resources.ErrorOnZooming} {ex.Message}",
                        System.Diagnostics.EventLogEntryType.Error);
                }
            }
        }
        Brush BlendForeground = new SolidColorBrush(Colors.White);
        private void UpdateControlLayout()
        {
            if (bDisposed)
                return;

            bOverride = true;
            Background = TrendBackground;
            bOverride = false;

            if (chart.Diagram != null)
            {
                if (chart.Diagram is XYDiagram2D)
                {
                    (chart.Diagram as XYDiagram2D).AxisX.Title.Content = XTitle;
                    (chart.Diagram as XYDiagram2D).AxisY.Title.Content = YTitle;

                    if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                        (chart.Diagram as XYDiagram2D).DefaultPane.Background = TrendBackground;

                    if (this.ReadLocalValue(TitleForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (chart.Diagram as XYDiagram2D).AxisX.Title.Foreground = TitleForeground;
                        (chart.Diagram as XYDiagram2D).AxisY.Title.Foreground = TitleForeground;
                    }

                    if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (chart.Diagram as XYDiagram2D).AxisX.Label.Foreground = AxisLabelForeground;
                        (chart.Diagram as XYDiagram2D).AxisY.Label.Foreground = AxisLabelForeground;
                    }
                }
                if (chart.Diagram is XYDiagram3D)
                {
                    (chart.Diagram as XYDiagram3D).AxisX.Title.Content = XTitle;
                    (chart.Diagram as XYDiagram3D).AxisY.Title.Content = YTitle;

                    if (this.ReadLocalValue(TitleForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (chart.Diagram as XYDiagram3D).AxisX.Title.Foreground = TitleForeground;
                        (chart.Diagram as XYDiagram3D).AxisY.Title.Foreground = TitleForeground;
                    }

                    if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        (chart.Diagram as XYDiagram3D).AxisX.Label.Foreground = AxisLabelForeground;
                        (chart.Diagram as XYDiagram3D).AxisY.Label.Foreground = AxisLabelForeground;
                    }
                }
                
                if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue)
                    UpdateAxsisFontSettings(AxsisFontSettings);

                if (this.ReadLocalValue(TitleFonstSettingsProperty) != DependencyProperty.UnsetValue)
                    UpdateTitleFonstSettings(TitleFonstSettings);

                if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                    chart.Diagram.Background = TrendBackground;
            }
            if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                chart.Background = TrendBackground;
                grid.Background = TrendBackground;
            }
            if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                toolbar.Background = ToolbarBackground;
                toolbar1.Background = ToolbarBackground;
            }

            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    seriesLabelVisible.Foreground = ToolbarForeground;
            //    legendAreaVisible.Foreground = ToolbarForeground;
            //    checkBox3D.Foreground = ToolbarForeground;
            //    simplepie.Foreground = ToolbarForeground;
            //    zoom3D.Foreground = ToolbarForeground;
            //}
            //else
            //{
            //    var theme =ThemeImageHelper.GetTheme(Document);
            //    if(theme == "Blend")
            //    {
            //        seriesLabelVisible.Foreground = BlendForeground;
            //        legendAreaVisible.Foreground = BlendForeground;
            //        simplepie.Foreground = BlendForeground;
            //        checkBox3D.Foreground = BlendForeground;
            //    }
            //}

            if (this.ReadLocalValue(PlotBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                var dxborder = (from c in chart.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                                where c.Name == "PART_DomainBackground"
                                select c).FirstOrDefault();
                if (dxborder != null)
                    dxborder.Background = PlotBackground;
            }

        }


        void chart_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point position = e.GetPosition(chart);
                ChartHitInfo hitInfo = chart.CalcHitInfo(position);
                if (hitInfo != null && hitInfo.SeriesPoint != null)
                {
                    if (ttContent != null)
                    {
                        var pen = PenList.FirstOrDefault(x => x.Name == hitInfo.SeriesPoint.Series.DisplayName);
                        if (pen != null)
                        {
                            int precision;
                            if (!int.TryParse(pen.PointPrecision.ToString(), out precision) || precision == -1)
                                precision = PointPrecision;
                            ttContent.Text = $"{hitInfo.SeriesPoint.Series.DisplayName}:\n{Properties.Resources.ChartArgPointTooltip} = {hitInfo.SeriesPoint.Argument}\n{Properties.Resources.ChartValuePointTooltip} = {Math.Round(hitInfo.SeriesPoint.NonAnimatedValue, precision)}";
                        }
                    }

                    pointTooltip.Placement = PlacementMode.RelativePoint;
                    pointTooltip.PlacementTarget = chart;
                    pointTooltip.HorizontalOffset = position.X + ToolTipOffset;
                    pointTooltip.VerticalOffset = position.Y + ToolTipOffset;
                    pointTooltip.IsOpen = true;
                }
                else
                    pointTooltip.IsOpen = false;
            }
            catch (Exception)
            {
                pointTooltip.IsOpen = false;
            }
        }

        void chart_MouseLeave(object sender, MouseEventArgs e)
        {
            pointTooltip.IsOpen = false;
        }

        DateTime startDate = DateTime.MinValue;
        public List<ChartSampleData> LoadData(int days)
        {
            var ret = new List<ChartSampleData>();
            Random r = new Random(DateTime.Now.Millisecond);

            for (int i = 1; i <= days; i++)
            {
                ret.Add(new ChartSampleData
                {
                    Argument = (ushort)i,
                    Value = r.NextDouble() * 100
                });
            }

            return ret;
        }
        
        void InitDiagram()
        {
            chart.Diagram = TryFindResource(string.Format("{1}diagram{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "simple" : string.Empty)) as Diagram;
            if (chart.Diagram is XYDiagram2D)
            {
                //if (RunningOnSlowPC)
                    (chart.Diagram as XYDiagram2D).DefaultPane.MirrorHeight = 0;
                //else
                //    (chart.Diagram as XYDiagram2D).DefaultPane.MirrorHeight = MirrorHeight;
            }
            else if (chart.Diagram is Diagram3D && !bWheeling)
                (chart.Diagram as Diagram3D).ZoomPercent = Zoom3DPercent;

            UpdateAxisRange();
        }
        void UpdateAxisRange()
        {
            if (chart.Diagram == null)
                return;

            if (chart.Diagram is XYDiagram2D)
            {
                if (AutomaticScale)
                    (chart.Diagram as XYDiagram2D).AxisY.ActualWholeRange.SetAuto();
                else
                    (chart.Diagram as XYDiagram2D).AxisY.WholeRange = TryFindResource("rangeY2D") as DevExpress.Xpf.Charts.Range;

            }
            else if (chart.Diagram is XYDiagram3D)
            {
                if (AutomaticScale)
                    (chart.Diagram as XYDiagram3D).AxisY.ActualWholeRange.SetAuto();
                else
                    (chart.Diagram as XYDiagram3D).AxisY.WholeRange = TryFindResource("rangeY3D") as DevExpress.Xpf.Charts.Range;
            }
        }

        private void InitChart()
        {
            try
            {
                chart.BeginInit();
                InitDiagram();
                if (chart.Diagram == null)
                    return;
                var _series = (from Series s in chart.Diagram.Series select s).ToList();
                foreach (Series s in _series)
                    chart.Diagram.Series.Remove(s);

                chart.Diagram.Series.Clear();
                mapKeySeries.Clear();

                if (PenList.Count == 0)
                {
                    Series addedSeries;
                    addedSeries = TryFindResource(string.Format("{1}{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "pieSeries" : "seriesAreaStyle")) as Series;
                    if (addedSeries != null)
                    {
                        mapKeySeries.Add(new Guid().ToString(), addedSeries);
                        chart.Diagram.Series.Add(addedSeries);
                        string _LinkedPenName = "DemoSerie";
                        addedSeries.Name = "DemoSerie";
                        addedSeries.DisplayName = _LinkedPenName;
                        InitSeries(addedSeries);
                        var dataValues = LoadData(SampleNumber);
                        addedSeries.BeginInit();
                        addedSeries.DataSource = dataValues;
                        addedSeries.EndInit();
                    }
                }
                else
                {
                    var penStyles = Enum.GetValues(typeof(PredefinedPenKinds)).Cast<PredefinedPenKinds>();

                    for (int key = 0; key < PenList.Count; key++)
                    {
                        string _keyname = PenList[key].NodeId.ToString();
                        Series addedSeries;
                        var penStyle = PenList[key].PenStyle;
                        var penKind = ((Trends.PredefinedPenKinds[])(penStyles))[(int)penStyle].ToString();
                        addedSeries = TryFindResource(string.Format("{1}{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "pieSeries" : penKind)) as Series;
                        if (PenList[key].StrokeThickness > 0)
                        {
                            if (addedSeries is LineSeries2D)
                            {
                                (addedSeries as LineSeries2D).LineStyle = new LineStyle((int)PenList[key].StrokeThickness);
                            }
                            else if (addedSeries is AreaSeries2D)
                            {
                                (addedSeries as AreaSeries2D).Border = new SeriesBorder();
                                (addedSeries as AreaSeries2D).Border.Brush = new SolidColorBrush(PenList[key].LColor);
                                (addedSeries as AreaSeries2D).Border.LineStyle = new LineStyle((int)PenList[key].StrokeThickness);
                            }
                        }
                        (addedSeries as Series).Visible = PenList[key].Visible;
                        mapKeySeries.Add(_keyname, addedSeries);
                        chart.Diagram.Series.Add(addedSeries);
                        addedSeries.Name = string.Format("Series{0}", key);
                        addedSeries.DisplayName = TranslationHelpers.TranslationHelper.TranslateComposedText(PenList[key].Name, stringlist, PenList[key].Name);
                        InitSeries(addedSeries);
                        var dataValues = LoadData(SampleNumber);
                        addedSeries.BeginInit();
                        addedSeries.DataSource = dataValues;
                        addedSeries.EndInit();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                chart.EndInit();
            }
        }

        private void InitSeries(Series addedSeries)
        {
            if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue)
                InitSeriesFont(addedSeries, AxsisFontSettings);
            InitLabels(addedSeries,SeriesLabelVisible, ResolveLabelOverlappingMode);
            addedSeries.ArgumentScaleType = ScaleType.Numerical;
            addedSeries.ArgumentDataMember = "Argument";
            addedSeries.ValueDataMember = "Value";
            addedSeries.ValueScaleType = ScaleType.Numerical;
            addedSeries.ToolTipEnabled = false;
            if (SimpleDiagram)
                (addedSeries as PieSeries).Titles[0].Content = addedSeries.DisplayName;
        }

        private void InitLabels(Series addedSeries, bool SeriesLabelVisible, ResolveOverlappingMode ResolveLabelOverlappingMode)
        {
            SeriesLabel labels = addedSeries.Label;
            addedSeries.LabelsVisibility = SeriesLabelVisible;
            labels.ResolveOverlappingMode = ResolveLabelOverlappingMode;
        }
        private void UpdateLabels()
        {
            if (chart == null || chart.Diagram == null)
                return;
            foreach (var c in chart.Diagram.Series)
            {
                InitLabels(c, SeriesLabelVisible, ResolveLabelOverlappingMode);
                if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue && !bcInit)
                    InitSeriesFont(c, AxsisFontSettings);
            }

        }
        private void chart_CustomDrawSeriesPoint(object sender, CustomDrawSeriesPointEventArgs e)
        {
            var currentPen = PenList.FirstOrDefault(x => x.Name == e.SeriesPoint.Series.DisplayName);
            int precision;            
            precision = PointPrecision;
            
            if (currentPen != null && currentPen.PointPrecision != null && currentPen.PointPrecision != -1)
                int.TryParse(currentPen.PointPrecision.ToString(), out precision);

            if (SimpleDiagram)
            {                
                if (ShowPercentValuesOnPieSerie)
                    e.LabelText = $"{e.SeriesPoint.Argument}: {Math.Round(e.PercentValue * 100, precision)}%";
                else
                    e.LabelText = $"{e.SeriesPoint.Argument}: {Math.Round(e.SeriesPoint.Value, precision)}";
            }
            else
            {
                e.LabelText = $"{Math.Round(e.SeriesPoint.Value, precision)}";
            }

            if (!SimpleDiagram)
                return;
            try
            {
                //chart.BeginInit();
                var drawOptions = e.DrawOptions;
                if (drawOptions == null)
                    return;

                int point = 1;
                int.TryParse(e.SeriesPoint.Argument, out point);
                if (bDesignmode)
                {
                    if (PenList.Count == 0)
                    {
                        e.LegendText = $"{Properties.Resources.PointItems} {point}";
                        e.Handled = true;
                    }
                    else if (PenList != null && mapKeySeries != null && mapKeySeries.ContainsValue(e.Series))
                    {
                        string key = (from k in mapKeySeries.Keys where mapKeySeries[k] == e.Series select k).FirstOrDefault();
                        string guid = key;
                        PenItem pen = (from p in PenList where p.NodeId == guid select p).FirstOrDefault();
                        if (pen != null && pen.PointSettings.Count >= point)
                        {
                            e.DrawOptions.Color = pen.PointSettings[point - 1].PColor;
                            string _label = pen.PointSettings[point - 1].PLabel;
                            _label = TranslationHelpers.TranslationHelper.TranslateComposedText(_label, stringlist, _label);
                            e.LegendText = _label;
                            e.Handled = true;
                        }
                        else
                        {
                            e.LegendText = $"{Properties.Resources.PointItem} {point}";
                            e.Handled = true;
                        }
                    }
                }
                else if (PenReferenceList != null && mapKeySeries != null && mapKeySeries.ContainsValue(e.Series))
                {
                    string key = (from k in mapKeySeries.Keys where mapKeySeries[k] == e.Series select k).FirstOrDefault();
                    string guid = key;
                    PenItem pen = (from p in PenReferenceList where p.NodeId == guid select p).FirstOrDefault();
                    if (pen != null && pen.PointSettings.Count >= point)
                    {
                        e.DrawOptions.Color = pen.PointSettings[point - 1].PColor;
                        string _label = pen.PointSettings[point - 1].PLabel;
                        _label = TranslationHelpers.TranslationHelper.TranslateComposedText(_label, stringlist, _label);
                        e.LegendText = _label;
                        e.Handled = true;
                    }
                    else
                    {
                        e.LegendText = $"{Properties.Resources.PointItem} {point}";
                        e.Handled = true;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                //chart.EndInit();
            }
        }
        private void chart_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {
            if (SimpleDiagram)
                return;
            if (bDesignmode)
            {
                if (PenList.Count == 0)
                {
                    e.DrawOptions.Color = Colors.LightBlue;
                    e.Handled = true;
                }
                else if (PenList != null && mapKeySeries != null && mapKeySeries.ContainsValue(e.Series))
                {
                    string key = (from k in mapKeySeries.Keys where mapKeySeries[k] == e.Series select k).FirstOrDefault();
                    string guid = key;
                    PenItem pen = (from p in PenList where p.NodeId == guid select p).FirstOrDefault();
                    if (pen != null)
                    {
                        e.DrawOptions.Color = pen.LColor;
                        e.Handled = true;
                    }
                }
            }
            else if (PenReferenceList != null && mapKeySeries != null && mapKeySeries.ContainsValue(e.Series))
            {
                string key = (from k in mapKeySeries.Keys where mapKeySeries[k] == e.Series select k).FirstOrDefault();
                string guid = key;
                PenItem pen = (from p in PenReferenceList where p.NodeId == guid select p).FirstOrDefault();
                if (pen != null)
                {
                    e.DrawOptions.Color = pen.LColor;
                    e.Handled = true;
                }
            }
        }
        private void UpdateChart()
        {
            RestoreChart();
            UpdateAxisTitles();
            AddSeries();
            bLoaded = true;
            bInit = true;
        }
   
        MonitoredItemViewModel zoomMonitoredItemViewModel;
        MonitoredItemViewModel pushMonitoredItemViewModel;
        MonitoredItemViewModel resetMonitoredItemViewModel;

        private void zoom_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (zoomMonitoredItemViewModel != null)
                    zoomMonitoredItemViewModel.PropertyChanged -= zoomMonitoredItemViewModel_PropertyChanged;
                zoomMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (zoomMonitoredItemViewModel != null)
                {
                    zoomMonitoredItemViewModel.PropertyChanged += zoomMonitoredItemViewModel_PropertyChanged;
                    zoomMonitoredItemViewModel_PropertyChanged(zoomMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }

        private void zoomMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null)
                {
                    if (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                        m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                    {
                        try
                        {
                            double _zoom;
                            if (Double.TryParse(m.DataValue.Value.ToString(), out _zoom))
                            {
                                //bUpdating = true;
                                Zoom3DPercent = _zoom;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        SetEntityError(null);
                    }
                    else
                        SetEntityError(m.DataValue.StatusCode.ToString());
                }
            }
        }

        private void push_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (pushMonitoredItemViewModel != null)
                    pushMonitoredItemViewModel.PropertyChanged -= pushMonitoredItemViewModel_PropertyChanged;
                pushMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (pushMonitoredItemViewModel != null)
                {
                    pushMonitoredItemViewModel.PropertyChanged += pushMonitoredItemViewModel_PropertyChanged;
                    pushMonitoredItemViewModel_PropertyChanged(pushMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }
        string oldPushValue;
        private void pushMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "Value" && IsEnableCallPushCommand)
            {
                bCallingPushCommand = true;
                lock (lockObject)
                {
                    string value = m.Value;
                    bool _bvalue = false;
                    bool breset = false;
                    bool bpush = false;
                    int _ivalue = 0;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (int.TryParse(value, out _ivalue))
                            breset = _ivalue != 0;
                        else if (bool.TryParse(value, out _bvalue))
                            breset = _bvalue;
                    }
                    bpush = breset && oldPushValue != value;
                    if (bpush)
                    {
                        ExecutePush();
                    }

                    bCallingPushCommand = false;
                    oldPushValue = value;
                    if (breset)
                        try
                        {
                            m.WriteValue(0);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"{Properties.Resources.ErrorOnPushing} {ex.Message}");
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
                               DateTime.UtcNow, $"{Properties.Resources.ErrorOnPushing} {ex.Message}",
                               System.Diagnostics.EventLogEntryType.Error);
                        }
                }
            }
        }

        private void reset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (resetMonitoredItemViewModel != null)
                    resetMonitoredItemViewModel.PropertyChanged -= resetMonitoredItemViewModel_PropertyChanged;
                resetMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (resetMonitoredItemViewModel != null)
                {
                    resetMonitoredItemViewModel.PropertyChanged += resetMonitoredItemViewModel_PropertyChanged;
                    resetMonitoredItemViewModel_PropertyChanged(resetMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }

        string oldResetValue;
        object lockObject = new object();
        private void resetMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "Value" && IsEnableCallResetCommand)
            {
                bCallingResetCommand = true;
                lock (lockObject)
                {
                    string value = m.Value;
                    bool _bvalue = false;
                    bool breset = false;
                    bool bexecutereset = false;
                    int _ivalue = 0;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (int.TryParse(value, out _ivalue))
                            breset = _ivalue != 0;
                        else if (bool.TryParse(value, out _bvalue))
                            breset = _bvalue;
                    }
                    bexecutereset = breset && oldResetValue != value;
                    if (bexecutereset)
                    {
                        ExecuteReset();
                    }
                    bCallingResetCommand = false;
                    oldResetValue = value;
                    if (breset)
                        try
                        {
                            m.WriteValue(0);
                        }
                        catch (Exception ex)
                        {
                            log.Error($"{Properties.Resources.ErrorOnReset} {ex.Message}");
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
                               DateTime.UtcNow, $"{Properties.Resources.ErrorOnReset} {ex.Message}",
                               System.Diagnostics.EventLogEntryType.Error);
                        }
                }
            }
        }

        //void InitValues()
        //{
        //    if (PenReferenceList == null || PenReferenceList.Count == 0)
        //        return;

        //    var fe = this as FrameworkElement;
        //    foreach (var key in OpcuaEntityReference.Keys)
        //    {
        //        if (!OpcuaEntityReference[key].IsValid)
        //            continue;

        //        MonitoredItemViewModel monitoredItem = OpcuaEntityReference[key].MonitoredItemViewModel;

        //        DataValue dataValue = null;
        //        if (monitoredItem != null)
        //            dataValue = monitoredItem.DataValue;

        //        if (dataValue == null)
        //            continue;

        //        if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) || dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
        //            InitValue(key, monitoredItem);
        //    }
        //}

        //private void InitValue(string key, MonitoredItemViewModel monitoreditem)
        //{
        //    UpdateRange(monitoreditem);
        //    UpdateMonitoredValue(key, monitoreditem);
        //}

        void UpdateMonitoredValue(string key, MonitoredItemViewModel monitoreditem)
        {
            DataValue dataValue = null;
            if (monitoreditem != null)
                dataValue = monitoreditem.DataValue;
            if (bDisposed || dataValue == null)
                return;

            object newValue = null;
            if (dataValue.Value is Array)
                newValue = monitoreditem.DataValueCollectionDouble;
            else
                newValue = dataValue;

            if (newValue != null)
                UpdateMonitoredValue(key, newValue);
        }

        Dictionary<string, object> queuedValues = new Dictionary<string, object>();
        void UpdateMonitoredValue(string key, object newValue)
        {
            bool bExecute = false;
            lock (queuedValues)
            {
                bExecute = queuedValues.Count() == 0;
                queuedValues[key] = newValue;

                if (bExecute)
                {
                    dpUpdateValue = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (bDisposed)
                            return;

                        Dictionary<string, object> tmpqueued;
                        lock (queuedValues)
                        {
                            tmpqueued = new Dictionary<string, object>(queuedValues);
                            queuedValues.Clear();
                        }

                        foreach (var k in tmpqueued.Keys)
                        {
                            if (tmpqueued[k] != null)
                                PrintMonitored(k, tmpqueued[k]);
                        }
                    });
                }
            }
        }

        void PrintMonitored(string k, object value)
        {
            bool onError = false;
            bool isArray = value is IList;
            int count = 0;
            if (isArray)
            {
                var dataCollection = value as List<OPCUAViewModel.MonitoredItemViewModel.DataObject>;
                if (dataCollection != null && dataCollection.Count > 0)
                {
                    count = Math.Min(sampleNumber, dataCollection.Count);
                    if (viewDataContext != null && k == monitoredKey)
                    {
                        for (ushort ii = 0; ii < count; ii++)
                        {
                            viewDataContext.AddData(dataCollection[ii].Value, ii);
                        }
                    }
                    else if (viewList != null && viewList.ContainsKey(k))
                    {
                        for (ushort ii = 0; ii < count; ii++)
                        {
                            viewList[k].AddData(dataCollection[ii].Value, ii);
                        }
                    }
                    else
                        onError = true;
                }
            }
            else if (viewDataContext != null && k == monitoredKey)
            {
                var dataValue = value as DataValue;
                viewDataContext.AddData(dataValue.Value, iCount);
            }
            else if (viewList != null && viewList.ContainsKey(k))
            {
                var dataValue = value as DataValue;
                viewList[k].AddData(dataValue.Value, iCount);
            }
            else 
				onError = true;

            if (!onError)
                UpdateView(k);
        }

        void UpdateView(string key)
        {
            if (key == monitoredKey)
            {
                if (viewDataContext != null && dataContextSerie != null)
                    DrawPoints(dataContextSerie, viewDataContext.Values);
            }
            else if (viewList != null && viewList.ContainsKey(key) && mapKeySeries.ContainsKey(key))
                DrawPoints(mapKeySeries[key], viewList[key].Values);
        }

        private void DrawPoints(Series addedSeries, IList<MyDataValue> values)
        {
            if (addedSeries == null || addedSeries.Points == null || values == null)
                return;
            chart.BeginInit();
            addedSeries.BeginInit();
            for (int ii = 0; ii < addedSeries.Points.Count; ii++)
            {
                try
                {
                    if (ii < values.Count)
                        addedSeries.Points[ii] = GetSeriesPointFromValues(ii + 1, values[ii].Value);
                    else
                        break;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format(Properties.Resources.ErrorDrawingValues, ex.Message));
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
                       DateTime.UtcNow, $"{Properties.Resources.ErrorDrawingValues} {ex.Message}",
                       System.Diagnostics.EventLogEntryType.Error);
                }
            }
            for (int ii = addedSeries.Points.Count; ii < values.Count; ii++)
            {
                try
                {
                    addedSeries.Points.Add(GetSeriesPointFromValues(ii + 1, values[ii].Value));
                }
                catch (Exception ex)
                {
                    log.Error(string.Format(Properties.Resources.ErrorDrawingValues, ex.Message));
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
                       DateTime.UtcNow, $"{Properties.Resources.ErrorDrawingValues} {ex.Message}",
                       System.Diagnostics.EventLogEntryType.Error);
                }
            }
            addedSeries.EndInit();
            chart.EndInit();
        }
        SeriesPoint GetSeriesPointFromValues(object argument, object value)
        {
            var dArgument = Convert.ToDouble(argument);
            var dValue = Convert.ToDouble(value);

            if (dValue == null || Double.IsInfinity((double)dValue) || Double.IsNaN((double)dValue))
                return new SeriesPoint((double)dArgument);
            return new SeriesPoint((double)dArgument, (double)dValue);
        }
        void UpdateRange(MonitoredItemViewModel monitoreditem)
        {
            if (monitoreditem == null)
                return;

            if (monitoreditem.HasRange && monitoreditem.Range != null)
            {
                UpdateRange(monitoreditem.Range.Low, monitoreditem.Range.High);
            }
        }

        void UpdateRange(double minValue, double maxValue)
        {
            if (UseEUMinMaxValues)
            {
                Minimum = Math.Min(Minimum, minValue);
                Maximum = Math.Max(Maximum, maxValue);
            }
        }

        void UpdateModel(string keyName, NodeIdViewModel model)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                if (keyName == monitoredKey)
                {
                    if (viewDataContext != null)
                        viewDataContext.UpdateReferences(model);
                }
                else
                {
                    if (viewList != null && viewList.ContainsKey(keyName))
                        viewList[keyName].UpdateReferences(model);
                }
            });
        }

        private void PrepareExecution(string key)
        {
            if (bDisposed || !OpcuaEntityReference.ContainsKey(key))
                return;
            try
            {
                if (mapHandlers == null)
                    mapHandlers = new Dictionary<string, PenItemHelper>();

                if (!OpcuaEntityReference[key].IsValid)
                    return;

                if (mapHandlers.ContainsKey(key))
                    TerminateExecution(key);

                mapHandlers[key] = new PenItemHelper(key);
                mapHandlers[key].Error += PenItem_OnError;
                mapHandlers[key].ModelChanged += PenItem_ModelChanged;
                mapHandlers[key].ValueChanged += PenItem_ValueChanged;
                mapHandlers[key].MinMaxRangeChanged += PenItem_MinMaxRangeChanged;
                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
            }
            catch (Exception)
            {
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

            if (!e.Model.IsUserReadable || !e.Model.IsReadable)
                return;

            UpdateModel(keyName, e.Model);
        }

        private void PenItem_MinMaxRangeChanged(object sender, MinMaxRangeChangedEventArgs e)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                UpdateRange(e.MinValue, e.MaxValue);
            });
        }

        private void PenItem_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;
            UpdateMonitoredValue(helper.Key, e.NewValue);
        }
        #endregion

        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
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
                        (grid as UIElement).Effect = previousEffect;
                        (grid as UIElement).ClipToBounds = previousClipToBounds;
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
                        previousEffect = (grid as UIElement).Effect;
                        previousClipToBounds = (grid as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (grid as UIElement).Effect = effect;
                        (grid as UIElement).ClipToBounds = false;
                    }
                }
            });
        }
        private void RestoreChart()
        {
            //LoadLayout(GetStorageName());

            try
            {
                chart.BeginInit();
                InitDiagram();
                if (chart.Diagram == null)
                    return;

                if (chart.Diagram.Series.Count > 0)
                {
                    //var _series = (from Series s in chart.Diagram.Series
                    //               where s.DisplayName != "dataContextSerie" && s.DisplayName != "dataContextSerie3D"
                    //               && s.DisplayName != "simpledataContextSerie" && s.DisplayName != "simpledataContextSerie3D"
                    //               select s).ToList();
                    var _series = (from Series s in chart.Diagram.Series
                                   where s.Name != "dataContextSerie" && s.Name != "dataContextSerie3D"
                                   && s.Name != "simpledataContextSerie" && s.Name != "simpledataContextSerie3D"
                                   select s).ToList();
                    foreach (Series s in _series)
                        chart.Diagram.Series.Remove(s);

                    //chart.Diagram.Series.Clear();
                }

                mapKeySeries.Clear();
            }
            catch
            {
            }
            finally
            {
                chart.EndInit();
            }

            //chart.IsEnabled = false;

        }
        private Opc.Ua.Range GetMinMaxRange()
        {
            try
            {
                var maxValue = PenReferenceList.OrderByDescending(m => m.Range.High).FirstOrDefault();
                var minValue = PenReferenceList.OrderByDescending(m => m.Range.Low).LastOrDefault();
                return new Opc.Ua.Range()
                {
                    High = maxValue.Range.High,
                    Low = minValue.Range.Low
                };
            }
            catch
            {
                return new Opc.Ua.Range()
                {
                    High = 100,
                    Low = 0
                };
            }
        }

        private void AddSeries()
        {
            if (chart.Diagram == null)
                return;

            var range = GetMinMaxRange();
            Maximum = range.High;
            Minimum = range.Low;

            var penStyles = Enum.GetValues(typeof(PredefinedPenKinds)).Cast<PredefinedPenKinds>();

            if(!bInit)
            {
                mapKeySeries.Clear();

                if (viewList != null)
                {
                    foreach (var view in viewList.Values)
                        view.Dispose();
                    viewList.Clear();
                }
            }

            for (int key = 0; key < PenReferenceList.Count; key++)
            {

                var penStyle = PenReferenceList[key].PenStyle;
                var penKind = ((Trends.PredefinedPenKinds[])(penStyles))[(int)penStyle].ToString();

                Series addedSeries = TryFindResource(string.Format("{1}{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "pieSeries" : penKind)) as Series; 
                //TryFindResource(string.Format("{1}{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "pieSeries" : penKind)) as Series;

                if (addedSeries != null)
                {

                    addedSeries.AnimationAutoStartMode = AnimationAutoStartMode.PlayOnce;

                    string _keyname = PenReferenceList[key].NodeId;// PenReferenceList[key].TagReference.ResolvedNodeId != null ? PenReferenceList[key].TagReference.ResolvedNodeId.ToString() : PenReferenceList[key].TagReference.RelativePath;

                    if (PenReferenceList[key].StrokeThickness > 0)
                    {
                        if (addedSeries is LineSeries2D)
                        {
                            (addedSeries as LineSeries2D).LineStyle = new LineStyle((int)PenReferenceList[key].StrokeThickness);
                        }
                        else if (addedSeries is AreaSeries2D)
                        {
                            (addedSeries as AreaSeries2D).Border = new SeriesBorder();
                            (addedSeries as AreaSeries2D).Border.Brush = new SolidColorBrush(PenReferenceList[key].LColor);
                            (addedSeries as AreaSeries2D).Border.LineStyle = new LineStyle((int)PenReferenceList[key].StrokeThickness);
                        }
                    }
                    (addedSeries as Series).Visible = PenReferenceList[key].Visible;

                    ChartDataGenerator view;
                    if (!viewList.ContainsKey(_keyname))
                    {
                        /*
                         * viewList[_keyname] = new ChartDataGenerator(
                         * _keyname, 
                         * SampleNumber, 
                         * SampleNumber, 
                         * ConnectionString, 
                         * new TimeSpan(0, 0, 0, 0, 250), 
                         * new TimeSpan(0, 0, 1, 0));
                         */
                        var settings = new DataGeneratorSettings()
                        {
                            HDataCount = SampleNumber,
                            ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase),
                            DeadBandInterval = new TimeSpan(0, 0, 0, 0, 250),
                            DeadBandTimeFrame = new TimeSpan(0, 0, 1, 0)
                        };
                        viewList[_keyname] = new ChartDataGenerator(_keyname, settings, CommandTimeout);
                        viewList[_keyname].Error += viewList_OnError;
                        viewList[_keyname].HistoryLoaded += SeriesHistoryLoaded;
                    }

                    view = viewList[_keyname];
                    mapKeySeries[_keyname] = addedSeries;

                    string _LinkedPenName = PenReferenceList[key].Name;
                    _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(_LinkedPenName, stringlist, _LinkedPenName);

                    addedSeries.Name = string.Format("Series{0}", key);
                    addedSeries.DisplayName = _LinkedPenName;
                    InitSeries(addedSeries);
                    UpdatePieSerie(addedSeries, ShowPercentValuesOnPieSerie);
                    //addedSeries.DataSource = view.Values;
                    chart.Diagram.Series.Add(addedSeries);
                    //chart.Diagram.Series.Insert(key, addedSeries);
                }
            }
        }

        private void viewList_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            log.Error(string.Format(Properties.Resources.ErrorLoadingValues, e.ErrorMessage));
            if (iUFProjectManager != null)
                iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartControlLog,
               DateTime.UtcNow, $"{Properties.Resources.ErrorLoadingValues} {e.ErrorMessage}",
               System.Diagnostics.EventLogEntryType.Error);
        }

        private void SeriesHistoryLoaded(object sender, EventArgs e)
        {
            ChartDataGenerator _data = sender as ChartDataGenerator;
            string key = _data.PenId;

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                UpdateView(key);
            });
        }

        void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;

            if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null)
                    UpdateMonitoredValue(monitoredKey, m);
            }
            else if (e.PropertyName == "NodeIdViewModel")
            {
                if (m.NodeIdModel != null && m.NodeIdModel.IsUserReadable && m.NodeIdModel.IsReadable)
                    UpdateModel(monitoredKey, m.NodeIdModel);
            }
        }
        Series dataContextSerie;
        private void AddMonitoredSeries()
        {
            if (monitoredItemViewModel == null)
                return;

            var range = GetMinMaxRange();
            Maximum = range.High;
            Minimum = range.Low;

            if (string.IsNullOrEmpty(monitoredKey))
                monitoredKey = Guid.NewGuid().ToString();

            dataContextSerie = (from c in chart.Diagram.Series
                                where c.Name == string.Format("{1}dataContextSerie{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "simple" : string.Empty)
                                select c).FirstOrDefault();

            if (dataContextSerie != null)
            {
                if (viewDataContext == null)
                {
                    /*
                     * viewDataContext = new ChartDataGenerator(
                     * monitoredKey, 
                     * SampleNumber, 
                     * SampleNumber, 
                     * ConnectionString, 
                     * new TimeSpan(0, 0, 0, 0, 250), 
                     * new TimeSpan(0, 0, 1, 0));
                     */
                    var settings = new DataGeneratorSettings()
                    {
                        HDataCount = SampleNumber,
                        ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase),
                        DeadBandInterval = new TimeSpan(0, 0, 0, 0, 250),
                        DeadBandTimeFrame = new TimeSpan(0, 0, 1, 0)
                    };
                    viewDataContext = new ChartDataGenerator(monitoredKey, settings, CommandTimeout);
                    viewDataContext.Error += viewList_OnError;
                    viewDataContext.HistoryLoaded += SeriesHistoryLoaded;
                }

                dataContextSerie.AnimationAutoStartMode = AnimationAutoStartMode.PlayOnce;

                string _LinkedPenName = LinkedPenName;
                _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(_LinkedPenName, stringlist, _LinkedPenName);

                if (SimpleDiagram)
                    (dataContextSerie as PieSeries).Titles[0].Content = _LinkedPenName;

                dataContextSerie.DisplayName = _LinkedPenName;
                dataContextSerie.Visible = true;
                dataContextSerie.ToolTipEnabled = false;
                UpdatePieSerie(dataContextSerie, ShowPercentValuesOnPieSerie);
            }

            UpdateRange(monitoredItemViewModel);
        }

        private void UpdateAxisTitles()
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                XYDiagram2D diagram2D = TryFindResource("diagram") as XYDiagram2D;
                XYDiagram3D diagram3D = TryFindResource("diagram3D") as XYDiagram3D;

                string _XTitle = TranslationHelper.TranslateComposedText(XTitle, stringlist, XTitle);
                string _YTitle = TranslationHelper.TranslateComposedText(YTitle, stringlist, YTitle);
                diagram2D.AxisX.Title.Content = _XTitle;
                diagram2D.AxisY.Title.Content = _YTitle;
                diagram3D.AxisX.Title.Content = _XTitle;
                diagram3D.AxisY.Title.Content = _YTitle;
            });
        }

        private void UpdateSeries()
        {
            if (bDesignmode)
            {
                for (int key = 0; key < PenList.Count; key++)
                {
                    Series addedSeries = (from s in chart.Diagram.Series where s.Name == string.Format("Series{0}", key) select s).FirstOrDefault();
                    if (addedSeries != null)
                    {
                        string _LinkedPenName = TranslationHelper.TranlslateText(PenList[key].Name, stringlist, PenList[key].Name);
                        addedSeries.DisplayName = _LinkedPenName;
                        if (SimpleDiagram)
                            (addedSeries as PieSeries).Titles[0].Content = _LinkedPenName;
                    }
                }
            }
            else
            {
                var dataContextSerie = (from c in chart.Diagram.Series
                                        where c.Name == string.Format("{1}dataContextSerie{0}", Enable3D ? "3D" : string.Empty, SimpleDiagram ? "simple" : string.Empty)
                                        select c).FirstOrDefault();
                if (dataContextSerie != null)
                {
                    string _LinkedPenName = TranslationHelper.TranlslateText(LinkedPenName, stringlist, LinkedPenName);
                    if (SimpleDiagram)
                        (dataContextSerie as PieSeries).Titles[0].Content = _LinkedPenName;
                    dataContextSerie.DisplayName = _LinkedPenName;

                }

                for (int key = 0; key < PenReferenceList.Count; key++)
                {
                    Series addedSeries = (from s in chart.Diagram.Series where s.Name == string.Format("Series{0}", key) select s).FirstOrDefault();
                    if (addedSeries != null)
                    {
                        string _LinkedPenName = TranslationHelper.TranlslateText(PenReferenceList[key].Name, stringlist, PenReferenceList[key].Name);
                        addedSeries.DisplayName = _LinkedPenName;
                        if (SimpleDiagram)
                            (addedSeries as PieSeries).Titles[0].Content = _LinkedPenName;
                    }
                }
            }
        }
        string stringPlaceolder = "Chart";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesignmode && stringeditorManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringeditorManager.GetListStringForCulture(Document, stringeditorManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                resetbutton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetTitle", stringlist, Properties.Resources.ResetTitle);
                pushbutton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PushTitle", stringlist, Properties.Resources.PushTitle);
                seriesLabelVisible.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SeriesLabelVisibleTitle", stringlist, Properties.Resources.SeriesLabelVisibleTitle);
                legendAreaVisible.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendAreaVisible", stringlist, Properties.Resources.LegendAreaVisible);
                simplepie.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SimplePieTitle", stringlist, Properties.Resources.SimplePie);
                checkBox3D.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Style3DTitle", stringlist, Properties.Resources.Style3D);
                zoom3D.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Zoom3DTitle", stringlist, Properties.Resources.Zoom3DTitle);

                UpdateAxisTitles();
                UpdateSeries();
            });
        }

        //private void InitMonitoredValue()
        //{
        //    DataValue dataValue = null;
        //    if (monitoredItemViewModel != null)
        //        dataValue = monitoredItemViewModel.DataValue;
        //    if (monitoredItemViewModel != null && dataValue != null)
        //    {
        //        if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) ||
        //            dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
        //        {
        //            InitValue(monitoredKey, monitoredItemViewModel);
        //        }
        //    }
        //}
        #endregion
        #region IDisposable
        double oldOpacity;
        object oldTooltip;
        bool isBusy;
        public void SetBusyEffect(bool bSet)
        {
            if (bSet)
            {
                if (!isBusy)
                {
                    isBusy = true;
                    oldOpacity = Opacity;
                    oldTooltip = (grid as FrameworkElement).ToolTip;
                    Opacity = 0.2;
                    (grid as FrameworkElement).ToolTip = Properties.Resources.Connecting;
                }
            }
            else
            {
                if (isBusy)
                {
                    isBusy = false;
                    grid.Opacity = oldOpacity;
                    (grid as FrameworkElement).ToolTip = oldTooltip;
                }
            }
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                if (Document != null && Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
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
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);
                
                if (workspace != null)
                {
                    dt = new DataTemplate();
                    factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                    factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                    dt.DataType = typeof(String);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(LinkedPenNameProperty, dt);
                   
                    mapDataTemplates.Add(XTitleProperty , dt);
                    mapDataTemplates.Add(YTitleProperty , dt);
                }

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.SmartPropertiesEditor));
                factory.SetValue(Controls.SmartPropertiesEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 1d);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.DisplayFormatStringProperty, "d");
                dt.DataType = typeof(double);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SampleNumberProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            lock (queuedValues)
            {
                if (dpUpdateValue != null &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                    dpUpdateValue.Abort();
                queuedValues.Clear();
            }

            if (stringeditorManager != null)
                stringeditorManager.CultureChanged -= StringManager_CultureChanged;

            DetachOverrideBaseProperties();

            toolbar.MouseEnter -= toolbar_MouseEnter;
            toolbar.MouseLeave -= toolbar_MouseLeave;

            SetBusyEffect(false);

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (!bDesignmode)
            {
                typeHelper.TerminateExecution(this, zoom_PropertyChanged, zoomMonitoredItemViewModel_PropertyChanged, zoom, zoomMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, reset_PropertyChanged, resetMonitoredItemViewModel_PropertyChanged, reset, resetMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, push_PropertyChanged, pushMonitoredItemViewModel_PropertyChanged, push, pushMonitoredItemViewModel);
            }

            TerminateExecution();

            //**********************
            //clear penReferenceList
            //**********************
            if (penReferenceList != null)
                penReferenceList.Clear();

            if (opcuaEntityReference != null)
                opcuaEntityReference.Clear();

            if (viewList != null)
            {
                foreach (var view in viewList.Values)
                    view.Dispose();
                viewList.Clear();
            }

            if (viewDataContext != null)
                viewDataContext.Dispose();

            chart.CustomDrawSeries -= chart_CustomDrawSeries;
            chart.CustomDrawSeriesPoint -= chart_CustomDrawSeriesPoint;
            chart.MouseLeave -= chart_MouseLeave;
            chart.MouseMove -= chart_MouseMove;

            foreach (var serie in chart.Diagram.Series)
                serie.DataSource = null;
            chart.Diagram.Series.Clear();

            mapKeySeries.Clear();

            if (mapHandlers != null)
                mapHandlers.Clear();

            typeHelper.Dispose();
        }
        private void TerminateExecution(string key)
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
                    typeHelper.TerminateExecution(this, mapHandlers[key].opcuaEntityReference_PropertyChanged, mapHandlers[key].monitoredItemViewModel_PropertyChanged, OpcuaEntityReference[key], OpcuaEntityReference[key].MonitoredItemViewModel);
                    mapHandlers[key].Error -= PenItem_OnError;
                    mapHandlers[key].ModelChanged -= PenItem_ModelChanged;
                    mapHandlers[key].ValueChanged -= PenItem_ValueChanged;
                    mapHandlers[key].MinMaxRangeChanged -= PenItem_MinMaxRangeChanged;
                    mapHandlers[key].Dispose();
                    mapHandlers.Remove(key);
                }
            }
        }
        private void TerminateExecution()
        {
            if (PenReferenceList.Count == 0)
                return;

            //*************
            //set not in use
            //*************
            foreach (var key in OpcuaEntityReference.Keys)
            {
                TerminateExecution(key);
            }
        }
        #endregion

        #region constructor
        //DispatcherTimer timer = new DispatcherTimer();
        bool bMonitoredInit;
        public Chart()
        {
            InitializeComponent();
            
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

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
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    UpdateControlLayout();
                    OverrideBaseProperties();
                    toolbar.Visibility = AllowRuntimeChanges ? Visibility.Visible : Visibility.Collapsed;

                    if (Document != null)
                    {
                        if (stringeditorManager == null)
                            stringeditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringeditorManager != null)
                        {
                            StringManager_CultureChanged(Document, null);
                            stringeditorManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        chart.IsEnabled = false;
                        chart.AnimationMode = EnableAnimation && !RunningOnSlowPC ? ChartAnimationMode : ChartAnimationMode.Disabled;
                        InitChart();
                        toolbar.IsEnabled = false;
                        if (AutoHideToolbar)
                        {
                            toolbar_MouseLeave(null, null);
                            Grid.SetRow(chart, 0);
                            Grid.SetRowSpan(chart, 2);
                        }

                        grid.IsHitTestVisible = false;

                        bDesignmode = true;
                        UpdateSeries();
                        bInit = true;
                    }
                    else
                    {
                        bDesignmode = false;
                        sampleNumber = SampleNumber;
                        
                        if (RunningOnSlowPC)
                        {
                            AutoHideToolbar = false;
                            EnableAnimation = false;
                            Enable3D = false;
                            checkBox3D.IsEnabled = false;
                        }

                        chart.AnimationMode = EnableAnimation ? ChartAnimationMode : ChartAnimationMode.Disabled;

                        bDesignmode = false;
                        UpdateChart();
                        InitControl();
                        bInit = true;
                    }

                    if (AutoHideToolbar)
                    {
                        toolbar_MouseLeave(null, null);
                        Grid.SetRow(chart, 0);
                        Grid.SetRowSpan(chart, 2);
                        toolbar.MouseEnter += toolbar_MouseEnter;
                        toolbar.MouseLeave += toolbar_MouseLeave;
                    }
                    bcInit = true;
                }
            };
            DataContextChanged += (o, e) =>
            {
                if (bDisposed)
                    return;

                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                monitoredItemViewModel = DataContext as MonitoredItemViewModel;

                if (monitoredItemViewModel != null)
                {
                    AddMonitoredSeries();
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                    if (monitoredItemViewModel.DataValue != null)
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                    if (monitoredItemViewModel.NodeIdModel != null)
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("NodeIdViewModel"));
                    bMonitoredInit = true;
                }
            };
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

        private void InitControl()
        {
            if (mapHandlers == null)
                mapHandlers = new Dictionary<string, PenItemHelper>();

            InitTag(ref push, Push, push_PropertyChanged, PushProperty.Name);
            InitTag(ref reset, Reset, reset_PropertyChanged, ResetProperty.Name);
            InitTag(ref zoom, Zoom, zoom_PropertyChanged, ZoomProperty.Name);

            try
            {
                if (OpcuaEntityReference.Count > 0)
                    foreach (var key in OpcuaEntityReference.Keys)
                    {
                        if(!OpcuaEntityReference[key].IsRelative && !matchChangedMap.Contains(key))
                            PrepareExecution(key);
                    }
            }
            catch (Exception)
            {
            }

            //if (viewList != null)
            //    InitValues();
        }

        private void InitTag(ref OPCUAEntityReference preparedtag, OPCUAXMLEntityReference xmltag, PropertyChangedEventHandler propertyChangedEventHandler, string property)
        {
            if (preparedtag == null && xmltag != null && xmltag.TagReference != null)
                preparedtag = xmltag.TagReference;
            if (preparedtag != null && !preparedtag.IsRelative && !matchChangedMap.Contains(property))
                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document, this, propertyChangedEventHandler, preparedtag);
        }

        private bool bInit;
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

        #region PushCommand
        RelayCommand _PushCommand;
        public ICommand PushCommand
        {
            get
            {
                if (_PushCommand == null)
                {
                    _PushCommand = new RelayCommand(
                        param => CallPushCommand(),
                        param => IsEnableCallPushCommand
                        );
                }
                return _PushCommand;
            }
        }

        bool bCallingPushCommand;
        void CallPushCommand()
        {
            if (bCallingPushCommand || bCallingResetCommand)
                return;
            bCallingPushCommand = true;
            ExecutePush();
            bCallingPushCommand = false;
        }
        private void ExecutePush()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                DataValue mDataValue = null;
                if (monitoredItemViewModel != null)
                    mDataValue = monitoredItemViewModel.DataValue;
                iCount = iCount + 1 > SampleNumber ? 0 : iCount + 1;
                foreach (var key in OpcuaEntityReference.Keys)
                {
                    if (!OpcuaEntityReference[key].IsValid)
                        continue;

                    MonitoredItemViewModel monitoredItem = OpcuaEntityReference[key].MonitoredItemViewModel;
                    DataValue dataValue = null;
                    if (monitoredItem != null)
                        dataValue = monitoredItem.DataValue;

                    if (dataValue == null || dataValue.Value is Array)
                        continue;

                    if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) || dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                        PrintMonitored(key, dataValue);
                }
                if (!string.IsNullOrEmpty(monitoredKey) && mDataValue != null && !(mDataValue is Array))
                {
                    if (Opc.Ua.StatusCode.IsGood(mDataValue.StatusCode) ||
                        mDataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                    {
                        PrintMonitored(monitoredKey, mDataValue);
                    }
                }
            });
        }
        bool IsEnableCallPushCommand
        {
            get
            {
                if (bCallingPushCommand || bCallingResetCommand)
                    return false;
                else
                    return true;
            }
        }

        #endregion
        #region ResetCommand
        RelayCommand _ResetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_ResetCommand == null)
                {
                    _ResetCommand = new RelayCommand(
                        param => CallResetCommand(),
                        param => IsEnableCallResetCommand
                        );
                }
                return _ResetCommand;
            }
        }

        bool bCallingResetCommand;
        void CallResetCommand()
        {
            if (bCallingPushCommand || bCallingResetCommand)
                return;
            bCallingResetCommand = true;
            ExecuteReset();
            bCallingResetCommand = false;
            if (IsEnableCallPushCommand)
                CallPushCommand();
        }

        private void ExecuteReset()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                foreach (var k in viewList.Keys)
                {
                    if (OpcuaEntityReference.ContainsKey(k) && OpcuaEntityReference[k].MonitoredItemViewModel != null &&
                        OpcuaEntityReference[k].MonitoredItemViewModel.DataValue != null &&
                        OpcuaEntityReference[k].MonitoredItemViewModel.DataValue.Value is Array)
                        continue;

                    if (mapKeySeries.ContainsKey(k))
                    {
                        viewList[k].ResetData();
                        UpdateView(k);
                    }
                }
                if (!(monitoredItemViewModel != null && monitoredItemViewModel.DataValue != null && monitoredItemViewModel.DataValue.Value is Array))
                {
                    if (viewDataContext != null && dataContextSerie != null && !string.IsNullOrEmpty(monitoredKey))
                    {
                        viewDataContext.ResetData();
                        UpdateView(monitoredKey);
                    }
                }

                iCount = SampleNumber;
            });
        }

        private void RefreshChart()
        {
            if (viewList != null)
                foreach (var k in viewList.Keys)
                {
                    if (mapKeySeries.ContainsKey(k))
                        DrawPoints(mapKeySeries[k], viewList[k].Values);
                }
            if (viewDataContext != null && dataContextSerie != null && !string.IsNullOrEmpty(monitoredKey))
                DrawPoints(dataContextSerie, viewDataContext.Values);
        }
        bool IsEnableCallResetCommand
        {
            get
            {
                if (bCallingPushCommand || bCallingResetCommand)
                    return false;
                else
                    return true;
            }
        }

        #endregion
        #region IDynamicTagAware
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if(PenList != null)
                foreach (var pen in PenList)
                {
                    if (pen.TagReference != null /*&& pen.XTagReference.IsValid*/)
                        ret.Add(pen.CreateUniqueName(pen.Name, ret.Keys.ToList()), pen.TagReferenceXml);
                }

            if (Push != null && Push.TagReference != null /*&& Push.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(PushProperty.Name, ret.Keys.ToList()), Push.TagReferenceXml);

            if (Reset != null && Reset.TagReference != null /*&& Reset.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(ResetProperty.Name, ret.Keys.ToList()), Reset.TagReferenceXml);

            if (Zoom != null && Zoom.TagReference != null /*&& Zoom.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(ZoomProperty.Name, ret.Keys.ToList()), Zoom.TagReferenceXml);

            return ret;
        }
        String CreateUniqueName(String name, List<String> list)
        {
            if (string.IsNullOrEmpty(name))
                name = Properties.Resources.DefaultPenName;
            if (!list.Contains(name))
                return name;
            var newname = name;
            int i = 0;
            while (list.Contains(newname))
                newname = String.Format("{0} ({1})", name, ++i);

            return newname;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;
            bool ret = false;
            bool cret = false;
           
            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();

            if (Push != null && relative == Push.TagReferenceXml)
            {
                matchChangedMap.Add(PushProperty.Name);
                cret = typeHelper.ChecktypeDefinition(Properties.Resources.SessionName, Document, this, bDesignmode || DesignerProperties.GetIsInDesignMode(this), Push, relative, absolute, push_PropertyChanged, pushMonitoredItemViewModel_PropertyChanged, ref push, pushMonitoredItemViewModel);
            }
            else if (Reset != null && relative == Reset.TagReferenceXml)
            {
                matchChangedMap.Add(ResetProperty.Name);
                cret = typeHelper.ChecktypeDefinition(Properties.Resources.SessionName, Document, this, bDesignmode || DesignerProperties.GetIsInDesignMode(this), Reset, relative, absolute, reset_PropertyChanged, resetMonitoredItemViewModel_PropertyChanged, ref reset, resetMonitoredItemViewModel);
            }
            else if (Zoom != null && relative == Zoom.TagReferenceXml)
            {
                matchChangedMap.Add(ZoomProperty.Name);
                cret = typeHelper.ChecktypeDefinition(Properties.Resources.SessionName, Document, this, bDesignmode || DesignerProperties.GetIsInDesignMode(this), Zoom, relative, absolute, zoom_PropertyChanged, zoomMonitoredItemViewModel_PropertyChanged, ref zoom, zoomMonitoredItemViewModel);
            }

            //32926
            PenItemList penList = new PenItemList(PenList);
            var penTagList = (from pen in penList where pen.TagReference != null select pen).ToList();
            int mapCount = penTagList.Count + (Push != null && Push.TagReference != null ? 1 : 0) + (Reset != null && Reset.TagReference != null ? 1 : 0) + (Zoom != null && Zoom.TagReference != null ? 1 : 0);

            foreach (var pen in penTagList)
            {
                //if (pen.TagReference != null /*&& pen.TagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                            ret = relative == pen.TagReferenceXml;
                        else if (_absolute != null)
                        {
                            if (relative == pen.TagReferenceXml)
                            {
                                matchChangedMap.Add(pen.NodeId);
                                string key = pen.NodeId;
                                TerminateExecution(key);
                                if(_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.TagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                }
                                else
                                {
                                    pen.TagReference = _absolute;
                                    opcuaEntityReference[key] = _absolute;
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

            if (!bDesignmode)
                ret = matchChangedMap.Count == mapCount;

            if (ret)
                penReferenceList = penList;

            return ret || cret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            PenItemList penList = new PenItemList(PenList);
            foreach (var pen in penList)
            {
                if (pen.TagReference != null /*&& pen.TagReference.IsValid*/)
                {
                    if (map.ContainsKey(pen.NodeId))
                        pen.TagReferenceXml = map[pen.NodeId];
                    else
                        pen.TagReferenceXml = typeHelper.UpdateTag(pen.TagReferenceXml, map);
                }
            }
            PenList = penList;

            if (Push != null && Push.TagReference != null /*&& Push.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(PushProperty.Name))
                    newValue.TagReferenceXml = map[PushProperty.Name];
                else
                    newValue.TagReference = typeHelper.UpdateTag(Push.TagReferenceXml, map).FromXml<OPCUAEntityReference>();
                Push = newValue;
            }
            if (Reset != null && Reset.TagReference != null /*&& Reset.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(ResetProperty.Name))
                    newValue.TagReferenceXml = map[ResetProperty.Name];
                else
                    newValue.TagReference = typeHelper.UpdateTag(Reset.TagReferenceXml, map).FromXml<OPCUAEntityReference>();
                Reset = newValue;
            }
            if (Zoom != null && Zoom.TagReference != null /*&& Zoom.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(ZoomProperty.Name))
                    newValue.TagReferenceXml = map[ZoomProperty.Name];
                else
                    newValue.TagReference = typeHelper.UpdateTag(Zoom.TagReferenceXml, map).FromXml<OPCUAEntityReference>();
                Zoom = newValue;
            }
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
        #endregion

        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
        {
        }
        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(LinkedPenName))
                list.Add(LinkedPenName);
            if (!string.IsNullOrEmpty(XTitle))
                list.Add(XTitle);
            if (!string.IsNullOrEmpty(YTitle))
                list.Add(YTitle);

            if (PenList != null)
            {
                var _list = PenList.Where(x => !string.IsNullOrEmpty(x.Name)).Select(x => x.Name);
                if (_list != null && _list.Count() > 0)
                    list.AddRange(_list);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (!string.IsNullOrEmpty(LinkedPenName))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(Chart), LinkedPenNameProperty).DisplayName;
                map.Add(propertyName, LinkedPenName);
            }
            if (!string.IsNullOrEmpty(XTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(Chart), XTitleProperty).DisplayName;
                map.Add(propertyName, XTitle);
            }
            if (!string.IsNullOrEmpty(YTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(Chart), YTitleProperty).DisplayName;
                map.Add(propertyName, YTitle);
            }
            int i = 1;
            PenList?.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().ForEach(x =>
            {
                map.Add(x.CreateUniqueName(x.Name, map.Keys.ToList()), x.Name);
                i++;
            });
            return map;
        }
        #endregion
    }
    public class ChartSampleData
    {
        public double Argument { get; set; }
        public double Value { get; set; }
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
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            if (sender is Chart)
            {
                if (sender is Chart)
                {
                    Chart control = sender as Chart;
                    if (control.ReadLocalValue(Chart.ForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("Foreground", control.Foreground);
                    else
                        ret.Add("Foreground", foreground);

                    if (control.ReadLocalValue(Chart.TitleForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("TitleForeground", control.TitleForeground);
                    else
                        ret.Add("TitleForeground", foreground);

                    if (control.ReadLocalValue(Chart.AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("AxisLabelForeground", control.AxisLabelForeground);
                    else
                        ret.Add("AxisLabelForeground", foreground);

                    if (control.ReadLocalValue(Chart.ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("ToolbarForeground", control.ToolbarForeground);
                    else
                        ret.Add("ToolbarForeground", foreground);


                    if (control.ReadLocalValue(Chart.PlotBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("PlotBackground", control.PlotBackground);
                    else
                        ret.Add("PlotBackground", background);

                    if (control.ReadLocalValue(Chart.TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("TrendBackground", control.TrendBackground);
                    else
                        ret.Add("TrendBackground", background);

                    if (control.ReadLocalValue(Chart.ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("ToolbarBackground", control.ToolbarBackground);
                    else
                        ret.Add("ToolbarBackground", background);
                }
                if (sender is ChartXY)
                {
                    ChartXY control = sender as ChartXY;
                    if (control.ReadLocalValue(ChartXY.ForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("Foreground", control.Foreground);
                    else
                        ret.Add("Foreground", foreground);

                    if (control.ReadLocalValue(ChartXY.TitleForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("TitleForeground", control.TitleForeground);
                    else
                        ret.Add("TitleForeground", foreground);

                    if (control.ReadLocalValue(ChartXY.AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("AxisLabelForeground", control.AxisLabelForeground);
                    else
                        ret.Add("AxisLabelForeground", foreground);

                    if (control.ReadLocalValue(ChartXY.ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("ToolbarForeground", control.ToolbarForeground);
                    else
                        ret.Add("ToolbarForeground", foreground);


                    if (control.ReadLocalValue(ChartXY.PlotBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("PlotBackground", control.PlotBackground);
                    else
                        ret.Add("PlotBackground", background);

                    if (control.ReadLocalValue(ChartXY.TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("TrendBackground", control.TrendBackground);
                    else
                        ret.Add("TrendBackground", background);

                    if (control.ReadLocalValue(ChartXY.ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
                        ret.Add("ToolbarBackground", control.ToolbarBackground);
                    else
                        ret.Add("ToolbarBackground", background);
                }
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
}
