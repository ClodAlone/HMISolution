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
using Utilities;
using Utilities.WPF;
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
using System.IO;
using DevExpress.Xpf.Editors;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DynamicTagAwareHelper;
using TranslationHelpers;
using UIMsgBoxAlertService.ComponentService;
using WPFPenHelpers;
using log4net;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using WPFUtilities;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;


namespace Trends
{
    /// <summary>
    /// Interaction logic for ChartXY.xaml
    /// </summary>
    public partial class ChartXY : UserControl, IContainPropertyEditors, IDisposable, IEntityReference, IDynamicTagAware, IConnectionAware
        , IStringIDAware
    {
        #region DP

        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ChartXY));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
            //OnForegroundChanged();
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ChartXY));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as ChartXY;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            {
                var label = AxsisFontSettings.Clone();
                var title = TitleFonstSettings.Clone();

                label.FontFamily = FontFamily;
                title.FontFamily = FontFamily;

                AxsisFontSettings = label;
                TitleFonstSettings = title;
            }
        }
        
        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as ChartXY;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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
            var control = sender as ChartXY;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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
            var control = sender as ChartXY;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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
            var control = sender as ChartXY;
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
            var control = sender as ChartXY;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !IsManipulationEnabled)
            {
                TrendBackground = Background;
                PlotBackground = Background;
            }
        }
        #endregion


        #region ShowMarkers
        public static readonly DependencyProperty ShowMarkersProperty = DependencyProperty.Register("ShowMarkers", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(true));
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


        #region Title

        public static readonly DependencyProperty XTitleProperty = DependencyProperty.Register("XTitle", typeof(string), typeof(ChartXY), new UIPropertyMetadata(Properties.Resources.ArgumentTitle, new PropertyChangedCallback(OnXTitleChanged), new CoerceValueCallback(OnCoerceXTitle)));

        private static object OnCoerceXTitle(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceXTitle((string)value);
            else
                return value;
        }

        private static void OnXTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnXTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceXTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
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
        public static readonly DependencyProperty YTitleProperty = DependencyProperty.Register("YTitle", typeof(string), typeof(ChartXY), new UIPropertyMetadata(Properties.Resources.ValueTitle, new PropertyChangedCallback(OnYTitleChanged), new CoerceValueCallback(OnCoerceYTitle)));

        private static object OnCoerceYTitle(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceYTitle((string)value);
            else
                return value;
        }

        private static void OnYTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnYTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceYTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
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
        public static readonly DependencyProperty EnableAnimationProperty = DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ChartXY),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableAnimationChanged), new CoerceValueCallback(OnCoerceEnableAnimation)));

        private static object OnCoerceEnableAnimation(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (bcInit && chart != null)
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
        public static readonly DependencyProperty ChartAnimationModeProperty = DependencyProperty.Register("ChartAnimationMode", typeof(ChartAnimationMode), typeof(ChartXY), new UIPropertyMetadata(ChartAnimationMode.OnLoad, new PropertyChangedCallback(OnChartAnimationModeChanged), new CoerceValueCallback(OnCoerceChartAnimationMode)));

        private static object OnCoerceChartAnimationMode(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceChartAnimationMode((ChartAnimationMode)value);
            else
                return value;
        }

        private static void OnChartAnimationModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
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

        #region PenList
        public static readonly DependencyProperty PenListProperty = DependencyProperty.Register("PenList", typeof(XYPenItemList), typeof(ChartXY), new UIPropertyMetadata(new XYPenItemList(), new PropertyChangedCallback(OnPenListChanged), new CoerceValueCallback(OnCoercePenList)));

        private static object OnCoercePenList(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoercePenList((XYPenItemList)value);
            else
                return value;
        }

        private static void OnPenListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnPenListChanged((XYPenItemList)e.OldValue, (XYPenItemList)e.NewValue);
        }

        protected virtual XYPenItemList OnCoercePenList(XYPenItemList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPenListChanged(XYPenItemList oldValue, XYPenItemList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                //if (newValue != null)
                //{
                //    penReferenceList = new XYPenItemList();
                //    (from c in PenList where c.XTagReference != null && c.XTagName != null && c.YTagReference != null && c.YTagName != null select c).ToList().ForEach(c => penReferenceList.Add(c));
                //}

                if (bLoaded && bcInit)
                {
                    //bUpdatePrepare = true;
                    UpdateChartLayout();
                }
            }
        }
        [Category("ChartOptions")]
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertXYPenItemList))]
        public XYPenItemList PenList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (XYPenItemList)GetValue(PenListProperty);
            }
            set
            {
                SetValue(PenListProperty, value);
            }
        }

        #endregion

        #region SeriesLabelVisible

        public static readonly DependencyProperty SeriesLabelVisibleProperty = DependencyProperty.Register("SeriesLabelVisible", typeof (bool), typeof (ChartXY),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnSeriesLabelVisibleChanged), new CoerceValueCallback(OnCoerceSeriesLabelVisible)));

        private static object OnCoerceSeriesLabelVisible(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            {
                UpdateLabels();
                if (!bSeriesLabelsClicking)
                    checkbox.IsChecked = newValue;
            }
        }
        [Category("ChartOptions")]
        public bool SeriesLabelVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(SeriesLabelVisibleProperty); }
            set { SetValue(SeriesLabelVisibleProperty, value); }
        }

        #endregion

        #region SeriesLabelTextPattern
        public static readonly DependencyProperty SeriesLabelTextPatternProperty = DependencyProperty.Register("SeriesLabelTextPattern", typeof(string), typeof(ChartXY), new UIPropertyMetadata("{A:F2} : {V:F2}", new PropertyChangedCallback(OnSeriesLabelTextPatternChanged), new CoerceValueCallback(OnCoerceSeriesLabelTextPattern)));

        private static object OnCoerceSeriesLabelTextPattern(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceSeriesLabelTextPattern((string)value);
            else
                return value;
        }

        private static void OnSeriesLabelTextPatternChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnSeriesLabelTextPatternChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceSeriesLabelTextPattern(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeriesLabelTextPatternChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLabels();
        }
        [Category("ChartOptions")]
        public string SeriesLabelTextPattern
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SeriesLabelTextPatternProperty);
            }
            set
            {
                SetValue(SeriesLabelTextPatternProperty, value);
            }
        }

        #endregion

        #region LinkedPenName
        public static readonly DependencyProperty LinkedPenNameProperty = DependencyProperty.Register("LinkedPenName", typeof(string), typeof(ChartXY), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnLinkedPenNameChanged), new CoerceValueCallback(OnCoerceLinkedPenName)));

        private static object OnCoerceLinkedPenName(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceLinkedPenName((string)value);
            else
                return value;
        }

        private static void OnLinkedPenNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
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

        public static readonly DependencyProperty ResolveLabelOverlappingModeProperty = DependencyProperty.Register("ResolveLabelOverlappingMode", typeof(ResolveOverlappingMode), typeof(ChartXY),
            new UIPropertyMetadata(ResolveOverlappingMode.None, new PropertyChangedCallback(OnResolveLabelOverlappingModeChanged), new CoerceValueCallback(OnCoerceResolveLabelOverlappingMode)));

        private static object OnCoerceResolveLabelOverlappingMode(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnTitleForegroundChanged), new CoerceValueCallback(OnCoerceTitleForeground)));

        private static object OnCoerceTitleForeground(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceTitleForeground((Brush)value);
            else
                return value;
        }

        private static void OnTitleForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnTitleForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTitleForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit) && bCLoaded)
                    UpdateProperties();
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
        public static readonly DependencyProperty AxisLabelForegroundProperty = DependencyProperty.Register("AxisLabelForeground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnAxisLabelForegroundChanged), new CoerceValueCallback(OnCoerceAxisLabelForeground)));

        private static object OnCoerceAxisLabelForeground(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceAxisLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnAxisLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnAxisLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceAxisLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAxisLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit) && bCLoaded)
                UpdateProperties();
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
        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit) && bCLoaded)
                UpdateProperties();
        }
        [Category("ChartOptions")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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
        public static readonly DependencyProperty TrendBackgroundProperty = DependencyProperty.Register("TrendBackground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnTrendBackgroundChanged), new CoerceValueCallback(OnCoerceTrendBackground)));

        private static object OnCoerceTrendBackground(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceTrendBackground((Brush)value);
            else
                return value;
        }

        private static void OnTrendBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit) && bCLoaded)
                UpdateProperties();
        }
        [Category("ChartOptions")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("ChartOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(ChartXY), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("ChartOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
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
        public static readonly DependencyProperty TitleFonstSettingsProperty = DependencyProperty.Register("TitleFonstSettings", typeof(FontSettings), typeof(ChartXY), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnTitleFonstSettingsChanged), new CoerceValueCallback(OnCoerceTitleFonstSettings)));

        private static object OnCoerceTitleFonstSettings(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceTitleFonstSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnTitleFonstSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnTitleFonstSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceTitleFonstSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleFonstSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateTitleFonstSettings(newValue);
        }

        void UpdateTitleFonstSettings(FontSettings newValue)
        {
            AxisTitle titlex = (chart.Diagram as XYDiagram2D).AxisX.Title;
            AxisTitle titley = (chart.Diagram as XYDiagram2D).AxisY.Title;
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
        public static readonly DependencyProperty AxsisFontSettingsProperty = DependencyProperty.Register("AxsisFontSettings", typeof(FontSettings), typeof(ChartXY), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceAxsisFontSettings)));

        private static object OnCoerceAxsisFontSettings(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceAxsisFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateAxsisFontSettings(newValue);
        }
        void UpdateAxsisFontSettings(FontSettings newValue)
        {
            foreach (var c in chart.Diagram.Series)
            {
                InitSeriesFont(c, newValue);
            }
            UpdateLegend();
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
        public static readonly DependencyProperty RotatedProperty = DependencyProperty.Register("Rotated", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRotatedChanged), new CoerceValueCallback(OnCoerceRotated)));

        private static object OnCoerceRotated(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceRotated((bool)value);
            else
                return value;
        }

        private static void OnRotatedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
        public static readonly DependencyProperty LogarithmicYScaleProperty = DependencyProperty.Register("LogarithmicYScale", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLogarithmicYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicYScale)));

        private static object OnCoerceLogarithmicYScale(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceLogarithmicYScale((bool)value);
            else
                return value;
        }

        private static void OnLogarithmicYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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

        public static readonly DependencyProperty LogarithmicBaseYScaleProperty = DependencyProperty.Register("LogarithmicBaseYScale", typeof(double), typeof(ChartXY), new UIPropertyMetadata(10.0, new PropertyChangedCallback(OnLogarithmicBaseYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicBaseYScale)));

        private static object OnCoerceLogarithmicBaseYScale(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceLogarithmicBaseYScale((double)value);
            else
                return value;
        }

        private static void OnLogarithmicBaseYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
        public static readonly DependencyProperty LegendAreaVisibleProperty = DependencyProperty.Register("LegendAreaVisible", typeof(Boolean), typeof(ChartXY), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLegendAreaVisibleChanged), new CoerceValueCallback(OnCoerceLegendAreaVisible)));

        private static object OnCoerceLegendAreaVisible(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceLegendAreaVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLegendAreaVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
            {
                UpdateLegend();
                if (!bLegendAreaClicking)
                    checkbox1.IsChecked = newValue;
            }
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

        public static readonly DependencyProperty LegendMarginProperty = DependencyProperty.Register("LegendMargin", typeof(Thickness), typeof(ChartXY),
            new UIPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnLegendMarginChanged), new CoerceValueCallback(OnCoerceLegendMargin)));

        private static object OnCoerceLegendMargin(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLegend();
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

        public static readonly DependencyProperty LegendHorizontalPositionProperty = DependencyProperty.Register("LegendHorizontalPosition", typeof (HorizontalPosition), typeof (ChartXY),
            new UIPropertyMetadata(HorizontalPosition.Left, new PropertyChangedCallback(OnLegendHorizontalPositionChanged), new CoerceValueCallback(OnCoerceLegendHorizontalPosition)));

        private static object OnCoerceLegendHorizontalPosition(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLegend();
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

        public static readonly DependencyProperty LegendVerticalPositionProperty = DependencyProperty.Register("LegendVerticalPosition", typeof (VerticalPosition), typeof (ChartXY),
            new UIPropertyMetadata(VerticalPosition.Top, new PropertyChangedCallback(OnLegendVerticalPositionChanged), new CoerceValueCallback(OnCoerceLegendVerticalPosition)));

        private static object OnCoerceLegendVerticalPosition(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateLegend();
        }
        [Category("ChartOptions")]
        public VerticalPosition LegendVerticalPosition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (VerticalPosition) GetValue(LegendVerticalPositionProperty); }
            set { SetValue(LegendVerticalPositionProperty, value); }
        }


        #endregion
        #region SampleNumber
        public static readonly DependencyProperty SampleNumberProperty = DependencyProperty.Register("SampleNumber", typeof(int), typeof(ChartXY), new UIPropertyMetadata(10, new PropertyChangedCallback(OnSampleNumberChanged), new CoerceValueCallback(OnCoerceSampleNumber)));

        private static object OnCoerceSampleNumber(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceSampleNumber((int)value);
            else
                return value;
        }

        private static void OnSampleNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
            {
                sampleNumber = SampleNumber;
                UpdateChartLayout(true);
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


        #region AutomaticScale
        public static readonly DependencyProperty AutomaticScaleProperty = DependencyProperty.Register("AutomaticScale", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutomaticScaleChanged), new CoerceValueCallback(OnCoerceAutomaticScale)));

        private static object OnCoerceAutomaticScale(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceAutomaticScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
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
            if (bLoaded && !bDisposed)
                DispatchScalePaddingSet((chart.Diagram as XYDiagram2D).AxisY);
        }

        [Category("ChartOptions")]
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

        #region MinorXCount
        public static readonly DependencyProperty MinorXCountProperty = DependencyProperty.Register("MinorXCount", typeof(double), typeof(ChartXY), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnMinorXCountChanged), new CoerceValueCallback(OnCoerceMinorXCount)));

        private static object OnCoerceMinorXCount(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceMinorXCount((double)value);
            else
                return value;
        }

        private static void OnMinorXCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnMinorXCountChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinorXCount(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorXCountChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double MinorXCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinorXCountProperty);
            }
            set
            {
                SetValue(MinorXCountProperty, value);
            }
        }

        #endregion
        #region MinorYCount
        public static readonly DependencyProperty MinorYCountProperty = DependencyProperty.Register("MinorYCount", typeof(double), typeof(ChartXY), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnMinorYCountChanged), new CoerceValueCallback(OnCoerceMinorYCount)));

        private static object OnCoerceMinorYCount(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceMinorYCount((double)value);
            else
                return value;
        }

        private static void OnMinorYCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnMinorYCountChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinorYCount(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorYCountChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double MinorYCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinorYCountProperty);
            }
            set
            {
                SetValue(MinorYCountProperty, value);
            }
        }

        #endregion


        #region MinXIndent
        public static readonly DependencyProperty MinXIndentProperty = DependencyProperty.Register("MinXIndent", typeof(double), typeof(ChartXY), new UIPropertyMetadata(50.0, new PropertyChangedCallback(OnMinXIndentChanged), new CoerceValueCallback(OnCoerceMinXIndent)));

        private static object OnCoerceMinXIndent(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceMinXIndent((double)value);
            else
                return value;
        }

        private static void OnMinXIndentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnMinXIndentChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinXIndent(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinXIndentChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double MinXIndent
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinXIndentProperty);
            }
            set
            {
                SetValue(MinXIndentProperty, value);
            }
        }

        #endregion


        #region MinYIndent
        public static readonly DependencyProperty MinYIndentProperty = DependencyProperty.Register("MinYIndent", typeof(double), typeof(ChartXY), new UIPropertyMetadata(50.0, new PropertyChangedCallback(OnMinYIndentChanged), new CoerceValueCallback(OnCoerceMinYIndent)));

        private static object OnCoerceMinYIndent(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceMinYIndent((double)value);
            else
                return value;
        }

        private static void OnMinYIndentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnMinYIndentChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinYIndent(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinYIndentChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartOptions")]
        public double MinYIndent
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinYIndentProperty);
            }
            set
            {
                SetValue(MinYIndentProperty, value);
            }
        }

        #endregion


        #region Minimum and Maximum
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ChartXY), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        private static object OnCoerceMaximum(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceMaximum((double)value);
            else
                return value;
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
        [Category("ChartXYOptions")]
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


        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ChartXY), new UIPropertyMetadata(-1.0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        private static object OnCoerceMinimum(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceMinimum((double)value);
            else
                return value;
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
        [Category("ChartXYOptions")]
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
        public static readonly DependencyProperty UseEUMinMaxValuesProperty = DependencyProperty.Register("UseEUMinMaxValues", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseEUMinMaxValuesChanged), new CoerceValueCallback(OnCoerceUseEUMinMaxValues)));

        private static object OnCoerceUseEUMinMaxValues(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceUseEUMinMaxValues((bool)value);
            else
                return value;
        }

        private static void OnUseEUMinMaxValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
        [Category("ChartXYOptions")]
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
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(ChartXY), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            ChartXY chart = o as ChartXY;
            if (chart != null)
                return chart.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY chart = o as ChartXY;
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
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bcInit && !string.IsNullOrEmpty(newValue))
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
      
        #region SimpleDiagram

        public static readonly DependencyProperty SimpleDiagramProperty = DependencyProperty.Register("SimpleDiagram", typeof (bool), typeof (ChartXY),
            new UIPropertyMetadata(false));

        [Category("ChartOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public bool SimpleDiagram
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(SimpleDiagramProperty); }
            set { SetValue(SimpleDiagramProperty, value); }
        }

        #endregion

        #region Advanced
        #region Push

        public static readonly DependencyProperty PushProperty = DependencyProperty.Register("Push", typeof(OPCUAXMLEntityReference), typeof(ChartXY),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnPushChanged), new CoerceValueCallback(OnCoercePush)));

        private static object OnCoercePush(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
        public OPCUAXMLEntityReference Push
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (OPCUAXMLEntityReference)GetValue(PushProperty); }
            set { SetValue(PushProperty, value); }
        }

        #endregion
        #region Reset

        public static readonly DependencyProperty ResetProperty = DependencyProperty.Register("Reset", typeof(OPCUAXMLEntityReference), typeof(ChartXY),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnResetChanged), new CoerceValueCallback(OnCoerceReset)));

        private static object OnCoerceReset(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
        public OPCUAXMLEntityReference Reset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (OPCUAXMLEntityReference)GetValue(ResetProperty); }
            set { SetValue(ResetProperty, value); }
        }

        #endregion
       
        #region AllowRuntimeChanges

        public static readonly DependencyProperty AllowRuntimeChangesProperty = DependencyProperty.Register("AllowRuntimeChanges", typeof (bool), typeof (ChartXY),
            new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRuntimeChangesChanged), new CoerceValueCallback(OnCoerceAllowRuntimeChanges)));

        private static object OnCoerceAllowRuntimeChanges(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit))
                UpdateToolbar();
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

        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof (bool), typeof (ChartXY),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            var control = o as ChartXY;
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
            var control = o as ChartXY;
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
            if (oldValue != newValue && (bLoaded && bcInit && bTLoaded))
                UpdateToolbar();
        }
        [Category("ChartAdvancedOptions")]
        public bool AutoHideToolbar
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (bool) GetValue(AutoHideToolbarProperty); }
            set { SetValue(AutoHideToolbarProperty, value); }
        }

        #endregion
        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(ChartXY), new UIPropertyMetadata(0));
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

        #region ShowIndex
        public static readonly DependencyProperty ShowIndexProperty = DependencyProperty.Register("ShowIndex", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowIndexChanged), new CoerceValueCallback(OnCoerceShowIndex)));

        private static object OnCoerceShowIndex(DependencyObject o, object value)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                return control.OnCoerceShowIndex((bool)value);
            else
                return value;
        }

        private static void OnShowIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY control = o as ChartXY;
            if (control != null)
                control.OnShowIndexChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowIndex(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowIndexChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ChartAdvancedOptions")]
        public bool ShowIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowIndexProperty);
            }
            set
            {
                SetValue(ShowIndexProperty, value);
            }
        }

        #endregion

        #endregion

        #region ScalePaddingFactor
        public static readonly DependencyProperty ScalePaddingFactorProperty = DependencyProperty.Register("ScalePaddingFactor", typeof(int), typeof(ChartXY), new UIPropertyMetadata(10, new PropertyChangedCallback(OnScalePaddingFactorChanged), new CoerceValueCallback(OnCoerceScalePaddingFactor)));

        private static object OnCoerceScalePaddingFactor(DependencyObject o, object value)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                return ChartXY.OnCoerceScalePaddingFactor((int)value);
            else
                return value;
        }

        private static void OnScalePaddingFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ChartXY ChartXY = o as ChartXY;
            if (ChartXY != null)
                ChartXY.OnScalePaddingFactorChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceScalePaddingFactor(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnScalePaddingFactorChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int ScalePaddingFactor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ScalePaddingFactorProperty);
            }
            set
            {
                SetValue(ScalePaddingFactorProperty, value);
            }
        }
        #endregion

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.XYSmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(ChartXY), new UIPropertyMetadata(false));

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
        int sampleNumber;
        bool bAutoHideToolbar;
        double oldOpacity;
        object oldTooltip;
        bool isBusy;
        bool bDisposed;
        bool bLoaded;
        bool bDesignmode;
        //bool bCTRL;
        string monitoredKey;
        ScreenDocument Document;
        TypeHelper typeHelper = new TypeHelper();
        IUFProjectManager iUFProjectManager;
        int iCount;
        IWorkspace workspace;
        IStringEditorManager StringManager;
        IDictionary<String, String> stringlist;
        bool bSeriesLabelsClicking;
        bool bLegendAreaClicking;
        //bool bUpdatePrepare;

        ChartXYDataGenerator viewDataContext;
        MonitoredItemViewModel monitoredItemViewModel;
        Dictionary<string, Series> mapKeySeries = new Dictionary<string, Series>();
        Series dataContextSerie;
        Dictionary<string, ChartXYDataGenerator> viewList = new Dictionary<string, ChartXYDataGenerator>();
        Dictionary<string, Color> mapKeySeriesLabelForeground = new Dictionary<string, Color>();

        Dictionary<string, PenItemHelper> mapHandlers;

        private OPCUAEntityReference push;
        private OPCUAEntityReference reset;
        //private OPCUAEntityReference rotation;

        internal readonly static string ArgumentPlaceHolder = "_1";
        internal readonly static string ValuePlaceHolder = "_2";

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.ChartXYControlLog);

        DispatcherOperation dpUpdateValue;
        CancellationTokenSource cts;

        XYPenItemList penReferenceList;
        private double ToolTipOffset = 2.0;
        DispatcherOperation dpAxisScaleChanged;
        DispatcherOperation dpAutoHideToolbar;
        AxisBase lastAxisScaleChangedAxis;

        XYPenItemList PenReferenceList
        {
            get
            {
                if (penReferenceList == null)
                {
                    penReferenceList = new XYPenItemList();
                    (from c in PenList where c.XTagReference != null && c.XTagName != null && c.YTagReference != null && c.YTagName != null select c).ToList().ForEach(c => penReferenceList.Add(c));
                }

                return penReferenceList;
            }
        }
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
                        opcuaEntityReference[$"{PenReferenceList[i].NodeId}{ArgumentPlaceHolder}"] = PenReferenceList[i].XTagReference;
                        opcuaEntityReference[$"{PenReferenceList[i].NodeId}{ValuePlaceHolder}"] = PenReferenceList[i].YTagReference;
                    }
                }

                return opcuaEntityReference;
            }
        }
        #endregion

        #region DynObjects
        ChartControl chart;
        Popup pointTooltip;
        TextEdit ttContent;
        DevExpress.Xpf.Bars.BarContainerControl toolbartray;
        DevExpress.Xpf.Bars.ToolBarControl toolbar;
        Storyboard sbLeave;
        Storyboard sbOver;
        DevExpress.Xpf.Bars.BarButtonItem resetbutton;
        DevExpress.Xpf.Bars.BarButtonItem pushbutton;
        DevExpress.Xpf.Bars.BarCheckItem checkbox;
        DevExpress.Xpf.Bars.BarCheckItem checkbox1;
        #endregion

        #region Methods
        private void diagram_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            XYDiagram2D diagram = sender as XYDiagram2D;
            Pane pane = diagram.DefaultPane;
            if (e.Delta > 0)
            {
                ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                scrollBarOptions.Visible = true;
                scrollBarOptions = pane.AxisYScrollBarOptions;
                scrollBarOptions.Visible = true;
            }
            else if (e.Delta < 0)
            {
                if (diagram.CanZoomOut())
                {
                    ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                    scrollBarOptions.Visible = true;
                    scrollBarOptions = pane.AxisYScrollBarOptions;
                    scrollBarOptions.Visible = true;
                }
                else
                {
                    ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                    scrollBarOptions.Visible = false;
                    scrollBarOptions = pane.AxisYScrollBarOptions;
                    scrollBarOptions.Visible = false;
                }
            }
        }

        private void diagram_Zoom(object sender, XYDiagram2DZoomEventArgs e)
        {
            XYDiagram2D diagram = sender as XYDiagram2D;
            Pane pane = diagram.DefaultPane;
            if (e.Type == XYDiagram2DZoomEventType.ZoomIn)
            {
                ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                scrollBarOptions.Visible = true;
                scrollBarOptions = pane.AxisYScrollBarOptions;
                scrollBarOptions.Visible = true;
            }
            else if (e.Type == XYDiagram2DZoomEventType.ZoomOut)
            {
                if (diagram.CanZoomOut())
                {
                    ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                    scrollBarOptions.Visible = true;
                    scrollBarOptions = pane.AxisYScrollBarOptions;
                    scrollBarOptions.Visible = true;
                }
                else
                {
                    ScrollBarOptions scrollBarOptions = pane.AxisXScrollBarOptions;
                    scrollBarOptions.Visible = false;
                    scrollBarOptions = pane.AxisYScrollBarOptions;
                    scrollBarOptions.Visible = false;
                }
            }
        }
        private void InitSeriesFont(Series c, FontSettings newValue)
        {
            c.FontSize = newValue.FontSize;
            c.FontFamily = newValue.FontFamily;
            c.FontWeight = newValue.FontWeight;
            c.FontStyle = newValue.FontStyle;
        }

        private void InitChartLayout()
        {

            if (bDisposed)
                return;

            InitChartControl();
            UpdateToolbar();

            if (chart == null)
                return;

            if (bDesignmode)
            {
                InitChart();
            }
            else
            {
                InitMinMaxValues();
                UpdateAxisRange();
                UpdateChart();
            }

            UpdateLabels();
            UpdateLegend();
        }
        private void UpdateChartLayout(bool hardReload = false)
        {
            if (bDisposed || chart == null)
                return;

            UpdateToolbar();

            if (bDesignmode)
            {
                InitChart();
            }
            else
            {
                UpdateChart(hardReload);
            }
        }

        private void InitControl()
        {
            //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            //{
            //    bCTRL = true;
            //}
            if (mapHandlers == null)
                mapHandlers = new Dictionary<string, PenItemHelper>();

            InitTag(ref push, Push, push_PropertyChanged, PushProperty.Name);
            InitTag(ref reset, Reset, reset_PropertyChanged, ResetProperty.Name);

            //if (viewList != null)
            //    InitValues(true);

            try
            {
                if (OpcuaEntityReference.Count > 0)
                    foreach (var key in OpcuaEntityReference.Keys)
                    {
                        if (!OpcuaEntityReference[key].IsRelative && !matchChangedMap.Contains(key))
                            PrepareExecution(key);
                    }
            }
            catch (Exception)
            {
            }
        }

        private void InitTag(ref OPCUAEntityReference preparedtag, OPCUAXMLEntityReference xmltag, PropertyChangedEventHandler propertyChangedEventHandler, string property)
        {
            if (preparedtag == null && xmltag != null && xmltag.TagReference != null)
                preparedtag = xmltag.TagReference;
            if (preparedtag != null && !preparedtag.IsRelative && !matchChangedMap.Contains(property))
                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document, this, propertyChangedEventHandler, preparedtag);
        }

        private void UpdateLabels()
        {
            if (chart == null || chart.Diagram == null)
                return;
            foreach (var c in chart.Diagram.Series)
            {
                c.Label.TextPattern = SeriesLabelTextPattern;
                InitLabels(c, SeriesLabelVisible, ResolveLabelOverlappingMode);
                if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue && !bcInit)
                    InitSeriesFont(c, AxsisFontSettings);
            }

        }

        private void UpdateLegend()
        {
            if (chart == null)
                return;
            if (!LegendAreaVisible)
            {
                if (chart.Legend != null)
                    chart.Legend.Visible = false;
            }
            else
            {
                if (chart.Legend == null)
                    chart.Legend = new Legend();    
                chart.Legend.Margin = LegendMargin;
                chart.Legend.VerticalPosition = LegendVerticalPosition;
                chart.Legend.HorizontalPosition = LegendHorizontalPosition;
                chart.Legend.FontSize = AxsisFontSettings.FontSize;
                chart.Legend.FontFamily = AxsisFontSettings.FontFamily;
                chart.Legend.FontWeight = AxsisFontSettings.FontWeight;
                chart.Legend.FontStyle = AxsisFontSettings.FontStyle;

                chart.Legend.Visible = true;
            }
        }

        bool bTLoaded;
        double DefToolbarHeight;
        private void UpdateToolbar()
        {
            if (AllowRuntimeChanges)
            {
                if(toolbartray == null)
                {
                    toolbartray = new DevExpress.Xpf.Bars.BarContainerControl()
                    {
                        ContainerType = DevExpress.Xpf.Bars.BarContainerType.Top,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    if (toolbar == null)
                    {
                        toolbar = new DevExpress.Xpf.Bars.ToolBarControl()
                        {
                            UseWholeRow = true,
                            BarItemDisplayMode = DevExpress.Xpf.Bars.BarItemDisplayMode.ContentAndGlyph,
                            IsMultiLine = false,
                            ShowDragWidget = false,
                            AllowCustomizationMenu = false,
                            AllowQuickCustomization = false,
                            AllowDrop = false
                        };
                        toolbar.Loaded += (o, e) =>
                        {
                            if (bTLoaded)
                                return;
                            bTLoaded = true;
                        };
                    }
                    /*  reset button  */
                    if (resetbutton == null)
                    {
                        resetbutton = new DevExpress.Xpf.Bars.BarButtonItem()
                        {
                            ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetTitle", stringlist, Properties.Resources.ResetTitle),
                            GlyphSize = DevExpress.Xpf.Bars.GlyphSize.Custom,
                            CustomGlyphSize = new Size(16, 16)
                        };
                        resetbutton.SetResourceReference(DevExpress.Xpf.Bars.BarButtonItem.GlyphProperty, "CancelSmall");
                        resetbutton.ItemClick += ResetButton_Click;
                    }
                    toolbar.Items.Add(resetbutton);

                    /*  pushbutton  */
                    if (pushbutton == null)
                    {
                        pushbutton = new DevExpress.Xpf.Bars.BarButtonItem()
                        {
                            ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PushTitle", stringlist, Properties.Resources.PushTitle),
                            GlyphSize = DevExpress.Xpf.Bars.GlyphSize.Custom,
                            CustomGlyphSize = new Size(16, 16)
                        };
                        pushbutton.SetResourceReference(DevExpress.Xpf.Bars.BarButtonItem.GlyphProperty, "OkSmall");
                        pushbutton.ItemClick += PushButton_Click;
                    }
                    toolbar.Items.Add(pushbutton);

                    /*  separator  */
                    toolbar.Items.Add(new DevExpress.Xpf.Bars.BarItemSeparator());

                    /*  SeriesLabelVisible  */
                    if (checkbox == null)
                    {
                        checkbox = new DevExpress.Xpf.Bars.BarCheckItem()
                        {
                            Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SeriesLabelVisibleTitle", stringlist, Properties.Resources.SeriesLabelVisibleTitle),
                            IsChecked = SeriesLabelVisible
                        };
                        checkbox.ItemClick += OnSeriesLabelVisibleClicked;
                    }
                    toolbar.Items.Add(checkbox);

                    /*  LegendAreaVisible  */
                    if (checkbox1 == null)
                    {
                        checkbox1 = new DevExpress.Xpf.Bars.BarCheckItem()
                        {
                            Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendAreaVisible", stringlist, Properties.Resources.LegendAreaVisible),
                            IsChecked = LegendAreaVisible
                        };
                        checkbox1.ItemClick += OnLegendAreaVisibleClicked;
                    }
                    toolbar.Items.Add(checkbox1);

                    if (!toolbartray.Bars.Contains(toolbar))
                        toolbartray.Bars.Add(toolbar);
                    if (!grid.Children.Contains(toolbartray))
                        grid.Children.Add(toolbartray);
                }

                if (toolbartray != null)
                {
                    toolbartray.MouseEnter -= ToolbarMouseEnter;
                    toolbartray.MouseLeave -= ToolbarMouseLeave;

                    resetbutton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetTitle", stringlist, Properties.Resources.ResetTitle);
                    pushbutton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PushTitle", stringlist, Properties.Resources.PushTitle);
                    checkbox.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SeriesLabelVisibleTitle", stringlist, Properties.Resources.SeriesLabelVisibleTitle);
                    checkbox1.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendAreaVisible", stringlist, Properties.Resources.LegendAreaVisible);

                    if (chart != null)
                    {
                        Grid.SetRowSpan(chart, 1);
                        Grid.SetRow(chart, 1);
                    }

                    if (AutoHideToolbar)
                    {
                        if (dpAutoHideToolbar == null ||
                            dpAutoHideToolbar.Status == DispatcherOperationStatus.Completed ||
                            dpAutoHideToolbar.Status == DispatcherOperationStatus.Aborted)
                        {
                            dpAutoHideToolbar = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                            {
                                DefToolbarHeight = toolbar.ActualHeight;
                                ToolbarMouseLeave(null, null);
                                if (chart != null)
                                {
                                    Grid.SetRow(chart, 0);
                                    Grid.SetRowSpan(chart, 2);
                                }

                                if (!bDesignmode && !RunningOnSlowPC)
                                {
                                    toolbartray.MouseEnter += ToolbarMouseEnter;
                                    toolbartray.MouseLeave += ToolbarMouseLeave;
                                }
                            });
                        }
                    }
                    else
                    {
                        if(DefToolbarHeight != 0)
                            ToolbarMouseEnter(null, null);
                    }
                    Grid.SetZIndex(toolbartray, 1);
                }
            }
            else
            {
                if (toolbartray != null)
                {
                    if (grid.Children.Contains(toolbartray))
                        grid.Children.Remove(toolbartray);

                    toolbartray.MouseEnter -= ToolbarMouseEnter;
                    toolbartray.MouseLeave -= ToolbarMouseLeave;
                    toolbartray.Bars.Clear();
                    toolbartray = null;
                }
                if (toolbar != null)
                {
                    toolbar.Items.Clear();
                }
                if (resetbutton != null)
                    resetbutton.ItemClick -= ResetButton_Click;
                if (pushbutton != null)
                    pushbutton.ItemClick -= PushButton_Click;

                resetbutton = null;
                pushbutton = null;
            }
        }

        void OnSeriesLabelVisibleClicked(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            bSeriesLabelsClicking = true;
            SeriesLabelVisible = (sender as DevExpress.Xpf.Bars.BarCheckItem).IsChecked ?? false;
            bSeriesLabelsClicking = false;
        }

        void OnLegendAreaVisibleClicked(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            bLegendAreaClicking = true;
            LegendAreaVisible = (sender as DevExpress.Xpf.Bars.BarCheckItem).IsChecked ?? false;
            bLegendAreaClicking = false;
        }

        private void ToolbarMouseLeave(object sender, MouseEventArgs e)
        {
            if(sbLeave == null)
            {
                sbLeave = new Storyboard();

                var myDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                myDoubleAnimationUsingKeyFrames.BeginTime = new TimeSpan(0);

                var splineDoubleKeyFrame = new SplineDoubleKeyFrame();
                splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(00.5));
                splineDoubleKeyFrame.Value = 0.1;
                splineDoubleKeyFrame.KeySpline = new KeySpline(0.0, 0.0, 0.5, 1.0);

                myDoubleAnimationUsingKeyFrames.KeyFrames.Add(splineDoubleKeyFrame);

                Storyboard.SetTargetProperty(myDoubleAnimationUsingKeyFrames, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(myDoubleAnimationUsingKeyFrames, toolbartray);

                sbLeave.Children.Add(myDoubleAnimationUsingKeyFrames);

                var myDoubleAnimationUsingKeyFrames1 = new DoubleAnimationUsingKeyFrames();
                myDoubleAnimationUsingKeyFrames1.BeginTime = new TimeSpan(0);

                var splineDoubleKeyFrame1 = new SplineDoubleKeyFrame();
                splineDoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(00.5));
                splineDoubleKeyFrame1.Value = 5;
                splineDoubleKeyFrame1.KeySpline = new KeySpline(0.0, 0.0, 0.5, 1.0);

                myDoubleAnimationUsingKeyFrames1.KeyFrames.Add(splineDoubleKeyFrame1);

                Storyboard.SetTargetProperty(myDoubleAnimationUsingKeyFrames1, new PropertyPath(FrameworkElement.HeightProperty));
                Storyboard.SetTarget(myDoubleAnimationUsingKeyFrames1, toolbartray);
                sbLeave.Children.Add(myDoubleAnimationUsingKeyFrames1);
            }
            if (sbLeave != null)
                sbLeave.Begin();
        }

        private void ToolbarMouseEnter(object sender, MouseEventArgs e)
        {
            if (sbOver == null)
            {
                sbOver = new Storyboard();

                var myDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
                myDoubleAnimationUsingKeyFrames.BeginTime = new TimeSpan(0);

                var splineDoubleKeyFrame = new SplineDoubleKeyFrame();
                splineDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(00.1000000));
                splineDoubleKeyFrame.Value = 1;
                splineDoubleKeyFrame.KeySpline = new KeySpline(0.0, 0.0, 0.5, 1.0);

                myDoubleAnimationUsingKeyFrames.KeyFrames.Add(splineDoubleKeyFrame);

                Storyboard.SetTargetProperty(myDoubleAnimationUsingKeyFrames, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(myDoubleAnimationUsingKeyFrames, toolbartray);

                sbOver.Children.Add(myDoubleAnimationUsingKeyFrames);

                var myDoubleAnimationUsingKeyFrames1 = new DoubleAnimationUsingKeyFrames();
                myDoubleAnimationUsingKeyFrames1.BeginTime = new TimeSpan(0);

                var splineDoubleKeyFrame1 = new SplineDoubleKeyFrame();
                splineDoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(00.5));
                splineDoubleKeyFrame1.Value = DefToolbarHeight;
                splineDoubleKeyFrame1.KeySpline = new KeySpline(0.0, 0.0, 0.5, 1.0);

                myDoubleAnimationUsingKeyFrames1.KeyFrames.Add(splineDoubleKeyFrame1);

                Storyboard.SetTargetProperty(myDoubleAnimationUsingKeyFrames1, new PropertyPath(FrameworkElement.HeightProperty));
                Storyboard.SetTarget(myDoubleAnimationUsingKeyFrames1, toolbartray);
                sbOver.Children.Add(myDoubleAnimationUsingKeyFrames1);
            }
            if (sbOver != null)
                sbOver.Begin();
        }
        bool bCLoaded;
        private void InitChartControl()
        {
            if (chart == null)
            {
                chart = new ChartControl()
                {
                    MinHeight = 1,
                    MinWidth = 1,
                    BorderBrush = null,
                    BorderThickness = new Thickness(0),
                    IsManipulationEnabled = true
                };

                chart.Loaded += (o, e) =>
                {
                    if (bCLoaded)
                        return;
                    bCLoaded = true;
                    UpdateProperties();
                };

                chart.CustomDrawSeries += chart_CustomDrawSeries;
                chart.MouseLeave += chart_MouseLeave;
                chart.MouseMove += chart_MouseMove;
                chart.AxisScaleChanged += chart_AxisScaleChanged;
                chart.CrosshairOptions = new CrosshairOptions()
                {
                    ShowArgumentLine = true,
                    ShowValueLine = true,
                    ShowArgumentLabels = true,
                    ShowValueLabels = true,
                    ShowCrosshairLabels = true,
                    CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries
                };
                if (!grid.Children.Contains(chart))
                    grid.Children.Add(chart);
            }
                                
            Grid.SetRowSpan(chart, 10);
            Grid.SetZIndex(chart, 1);
            chart.IsEnabled = !bDesignmode;
            chart.AnimationMode = EnableAnimation && !RunningOnSlowPC ? ChartAnimationMode : ChartAnimationMode.Disabled;

            try
            {
                if(chart.Diagram == null)
                {
                    chart.Diagram = LoadTemplate("diagram", ChartDiagram) as Diagram;
                    (chart.Diagram as XYDiagram2D).Zoom += diagram_Zoom;
                    (chart.Diagram as XYDiagram2D).MouseWheel += diagram_MouseWheel;
                }
            }
            catch
            {
            }
                
            if (pointTooltip == null)
                pointTooltip = new Popup();

            if (ttContent == null)
                ttContent = new TextEdit() { IsReadOnly = true };

            pointTooltip.Child = ttContent;

            if (!grid.Children.Contains(pointTooltip))
                grid.Children.Add(pointTooltip);
            Grid.SetZIndex(pointTooltip, 2);
        }

        private void UpdateProperties()
        {
            if (chart == null || chart.Diagram == null)
                return;
            try
            {
                if (this.ReadLocalValue(PlotBackgroundProperty) != DependencyProperty.UnsetValue)
                {
                    var dxborder = (from c in chart.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                                    where c.Name == "PART_DomainBackground"
                                    select c).FirstOrDefault();
                    if (dxborder != null)
                        dxborder.Background = PlotBackground;
                }

                if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                {
                    chart.Background = TrendBackground;
                    grid.Background = TrendBackground;
                }

                if (this.ReadLocalValue(TitleForegroundProperty) != DependencyProperty.UnsetValue)
                {
                    (chart.Diagram as XYDiagram2D).AxisX.Title.Foreground = TitleForeground;
                    (chart.Diagram as XYDiagram2D).AxisY.Title.Foreground = TitleForeground;
                }

                if (this.ReadLocalValue(TitleFonstSettingsProperty) != DependencyProperty.UnsetValue)
                    UpdateTitleFonstSettings(TitleFonstSettings);

                if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                {
                    (chart.Diagram as XYDiagram2D).AxisX.Label.Foreground = AxisLabelForeground;
                    (chart.Diagram as XYDiagram2D).AxisY.Label.Foreground = AxisLabelForeground;
                }

                if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue)
                    UpdateAxsisFontSettings(AxsisFontSettings);
            }
            catch (Exception)
            {
            }
            if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                chart.Diagram.Background = TrendBackground;
        }

        private object LoadTemplate(string template, string dictionary)
        {
            try
            {
                string basename = $"{dictionary}.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.Resources.{1}", typeof(ChartXY).Namespace, basename));
                if (stream == null)
                    return null;

                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                object content = (object)canvas.TryFindResource(template);
                return content;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private bool bResetting;
        public bool IsResetting {
            get
            {
                return bResetting;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewList != null)
            {
                viewList.Keys.ToList().ForEach(key =>
                {
                    var xKey = $"{key}{ArgumentPlaceHolder}";
                    var yKey = $"{key}{ArgumentPlaceHolder}";
                    bool arrayX = IsArrayType(xKey);
                    bool arrayY = IsArrayType(yKey);

                    if (arrayX && arrayY)
                        return;

                    ChartXYDataGenerator.ResetType resetType = !(arrayX && arrayY) ? ChartXYDataGenerator.ResetType.XY : !arrayX ? ChartXYDataGenerator.ResetType.X : ChartXYDataGenerator.ResetType.Y;
                    viewList[key].ResetData(resetType);
                    if (mapKeySeries.ContainsKey(key))
                        DrawPoints(mapKeySeries[key], viewList[key].Values);
                });
            }

            if (!(monitoredItemViewModel != null && monitoredItemViewModel.DataValue != null && monitoredItemViewModel.DataValue.Value is Array))
            {
                if (viewDataContext != null)
                {
                    viewDataContext.ResetData(ChartXYDataGenerator.ResetType.XY);
                    DrawPoints(dataContextSerie, viewDataContext.Values);
                }
            }

            iCount = 0;
        }

        private bool IsArrayType(string key)
        {
            return (OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key].MonitoredItemViewModel != null &&
                        OpcuaEntityReference[key].MonitoredItemViewModel.DataValue != null &&
                        OpcuaEntityReference[key].MonitoredItemViewModel.DataValue.Value is Array);
        }

        private void PushButton_Click(object sender, RoutedEventArgs e)
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
            if (!string.IsNullOrEmpty(monitoredKey) && mDataValue != null && !(mDataValue.Value is Array))
            {
                if (Opc.Ua.StatusCode.IsGood(mDataValue.StatusCode) ||
                    mDataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                {
                    PrintMonitored(monitoredKey, mDataValue);
                }
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
                        ttContent.Text = $"{hitInfo.SeriesPoint.Series.DisplayName}:\n{XTitle} = {hitInfo.SeriesPoint.Argument}\n{YTitle} = {Math.Round(hitInfo.SeriesPoint.NonAnimatedValue, 3)}";

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
        public XYDataValue[] LoadData(int samples, double _minimum, double _maximum)
        {
            XYDataValue[] ret = new XYDataValue[samples];
            Random r = new Random(DateTime.Now.Millisecond);
            var offset = (_maximum - _minimum) / 2;
            var step = samples > 0 ? (_maximum - _minimum) / samples : (_maximum - _minimum);
            var a = r.NextDouble();
            var b = r.NextDouble() * 10;
            a = a * offset;
            try
            {
                int j = 0;
                for (double i = _minimum; i < _maximum; i += step)
                {
                    double t = Math.Tan((double)i / 180 * Math.PI);
                    double x = 3 * (double)a * t / (t * t * t + 1);
                    double y = b + x * t;
                    x = x + b;
                    XYDataValue xydata = new XYDataValue();
                    xydata.Value1 = new DataValue() { Value = x, SourceTimestamp = DateTime.MaxValue };
                    xydata.Value2 = new DataValue() { Value = y, SourceTimestamp = DateTime.MaxValue };
                    ret[j] = (xydata);
                    j++;
                }
            }
            catch (Exception)
            {
            }            
            return ret;
        }
        
        string ChartDiagram = "ChartDiagram";
        private void InitChart()
        {
            if (chart == null || chart.Diagram == null)
                return;
            try
            {
                InitMinMaxValues();

                chart.BeginInit();
                chart.Diagram.Series.Clear();
                chart.EndInit();

                UpdateAxisRange();

                mapKeySeries.Clear();
                mapKeySeriesLabelForeground.Clear();


                if (PenList.Count == 0)
                {
                    Series addedSeries;
                    addedSeries = LoadSeries();
                    if (addedSeries != null)
                    {
                        mapKeySeries.Add(new Guid().ToString(), addedSeries);
                        string _LinkedPenName = "DemoSerie";
                        addedSeries.Name = "DemoSerie";
                        addedSeries.DisplayName = _LinkedPenName;
                        InitSeries(addedSeries);
                        XYDataValue[] values = LoadData(SampleNumber, Minimum, Maximum);
                        chart.BeginInit();
                        chart.Diagram.Series.Add(addedSeries);
                        chart.EndInit();
                        DrawPoints(addedSeries, values);
                    }
                }
                else
                {
                    for (int key = 0; key < PenList.Count; key++)
                    {
                        Series addedSeries;
                        addedSeries = LoadSeries();
                        mapKeySeries.Add(PenList[key].NodeId.ToString(), addedSeries);
                        mapKeySeriesLabelForeground.Add(PenList[key].NodeId.ToString(), PenList[key].LFColor);
                        if (PenList[key].StrokeThickness > 0)
                            (addedSeries as LineScatterSeries2D).LineStyle = new LineStyle((int)PenList[key].StrokeThickness);
                        (addedSeries as LineScatterSeries2D).Visible = PenList[key].Visible;
                        addedSeries.Name = string.Format("Series{0}", key);
                        addedSeries.DisplayName = TranslationHelpers.TranslationHelper.TranslateComposedText(PenList[key].Name, stringlist, PenList[key].Name); 
                        InitSeries(addedSeries);
                        XYDataValue[] values = LoadData(SampleNumber, Minimum, Maximum);
                        chart.BeginInit();
                        chart.Diagram.Series.Add(addedSeries);
                        chart.EndInit();
                        DrawPoints(addedSeries, values);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void InitMinMaxValues()
        {
            double _Maximum = Maximum;
            double _Minimum = Minimum;

            try
            {
                XYPenItem maxValue = null;
                XYPenItem minValue = null;

                if (PenList != null && PenList.Count > 0)
                {
                    maxValue = (from c in PenList orderby c.Range.High descending select c).ToList().FirstOrDefault();
                    minValue = (from c in PenList orderby c.Range.Low ascending select c).ToList().FirstOrDefault();
                    _Maximum = UseEUMinMaxValues ? 100 : maxValue.Range.High;
                    _Minimum = UseEUMinMaxValues ? 0 : minValue.Range.Low;
                }
                else
                {
                    _Maximum = Maximum;
                    _Minimum = Minimum;
                }
            }
            catch
            {
                _Maximum = Maximum;
                _Minimum = Minimum;
            }

            Maximum = _Maximum;
            Minimum = _Minimum;
        }

        private void DrawPoints(Series addedSeries, IList<XYDataValue> values)
        {
            if (addedSeries == null)
                return;
            chart.BeginInit();
            addedSeries.BeginInit();
            for (int i = 0; i < addedSeries.Points.Count; i++)
            {
                if (i < values.Count)
                    addedSeries.Points[i] = GetSeriesPointFromValues(values[i].Value1.Value, values[i].Value2.Value);
                else
                    break;
            }
            for (int i = addedSeries.Points.Count; i < values.Count; i++)
            {
                addedSeries.Points.Add(GetSeriesPointFromValues(values[i].Value1.Value, values[i].Value2.Value));
            }
            addedSeries.EndInit();
            chart.EndInit();
        }

        SeriesPoint GetSeriesPointFromValues(object argument, object value)
        {
            var dArgument = argument as double?;
            var dValue = value as double?;

            if (dValue == null || dArgument == null || Double.IsInfinity((double)dValue) || Double.IsNaN((double)dValue) || Double.IsInfinity((double)dArgument) || Double.IsNaN((double)dArgument))
                return new SeriesPoint(null);
            return new SeriesPoint((double)dArgument, (double)dValue);
        }

        private Series LoadSeries()
        {
            Binding markers = new Binding()
            {
                Path = new PropertyPath("ShowMarkers"),
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
            };
            LineScatterSeries2D serie = new LineScatterSeries2D()
            {
                ArgumentScaleType = ScaleType.Numerical,
                MarkerSize = 5,
                MarkerVisible = ShowMarkers,
                MarkerModel = new CircleMarker2DModel(),
                AnimationAutoStartMode = AnimationAutoStartMode.SetStartState,
                Label = new SeriesLabel()
                {
                    ResolveOverlappingMode = ResolveOverlappingMode.JustifyAllAroundPoint,
                    ConnectorThickness = 1,
                    TextPattern = SeriesLabelTextPattern
                },
                LabelsVisibility = false
            };
            serie.SetBinding(LineScatterSeries2D.MarkerVisibleProperty, markers);
            return serie;
        }
        
        private void InitSeries(Series addedSeries)
        {
            if (this.ReadLocalValue(AxsisFontSettingsProperty) != DependencyProperty.UnsetValue)
                InitSeriesFont(addedSeries, AxsisFontSettings);
            InitLabels(addedSeries,SeriesLabelVisible, ResolveLabelOverlappingMode);
        }

        private void InitLabels(Series addedSeries, bool SeriesLabelVisible, ResolveOverlappingMode ResolveLabelOverlappingMode)
        {
            addedSeries.LabelsVisibility = SeriesLabelVisible;
            if (SeriesLabelVisible)
            {
                SeriesLabel labels = addedSeries.Label;
                labels.ResolveOverlappingMode = ResolveLabelOverlappingMode;
                var key = (from k in mapKeySeries.Keys where mapKeySeries[k] == addedSeries select k).FirstOrDefault();
                if (key != null && mapKeySeriesLabelForeground.ContainsKey(key))
                {
                    labels.Foreground = new SolidColorBrush(mapKeySeriesLabelForeground[key]);
                }
            }
        }

        private void chart_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {
            if(bDesignmode)
            {
                if (PenList.Count == 0)
                {
                    e.DrawOptions.Color = Colors.LightBlue;
                    e.Handled = true;
                }
                else if (PenList != null && mapKeySeries != null && mapKeySeries.ContainsValue(e.Series))
                {
                    string key = (from k in mapKeySeries.Keys where mapKeySeries[k] == e.Series select k).FirstOrDefault();
                    XYPenItem pen = (from p in PenList where p.NodeId == key select p).FirstOrDefault();
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
                XYPenItem pen = (from p in PenReferenceList where p.NodeId == key select p).FirstOrDefault();
                if (pen != null)
                {
                    e.DrawOptions.Color = pen.LColor;
                    e.Handled = true;
                }
            }
        }

        public bool Refresh()
        {
            if (bDesignmode || bDisposed || !bcInit || !bLoaded || !bcInit || !bCLoaded)
                return false;

            UpdateChart(true);
            return true;
        }

        private void UpdateChart(bool hardReload = false)
        {
            if (hardReload)
            {
                TerminateExecution();
                if (mapHandlers != null)
                    mapHandlers.Clear();
                opcuaEntityReference = null;
                try
                {
                    if (OpcuaEntityReference.Count > 0)
                        foreach (var key in OpcuaEntityReference.Keys)
                        {
                            if (!OpcuaEntityReference[key].IsRelative && !matchChangedMap.Contains(key))
                                PrepareExecution(key);
                        }
                }
                catch (Exception)
                {
                }
            }
            RestoreChart();
            AddSeries(hardReload);
            UpdateAxisTitles();
            bLoaded = true;

            bcInit = true;
        }

      
        MonitoredItemViewModel pushMonitoredItemViewModel;
        MonitoredItemViewModel resetMonitoredItemViewModel;
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

        bool bPushing;
        private void pushMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "Value")
            {
                if (!string.IsNullOrEmpty(m.Value))
                {
                    int _value;
                    bool _bvalue;
                    if (int.TryParse(m.Value, out _value))
                    {
                        if (_value != 0 && !bPushing)
                        {
                            bPushing = true;
                            PushButton_Click(null, null);
                            m.Value = "0";
                        }
                        else if (_value == 0)
                        {
                            bPushing = false;
                        }
                    }
                    else if (bool.TryParse(m.Value, out _bvalue))
                    {
                        if (_bvalue && !bPushing)
                        {
                            bPushing = true;
                            PushButton_Click(null, null);
                            m.Value = "False";
                        }
                        else if (!_bvalue)
                        {
                            bPushing = false;
                        }
                    }
                    SetEntityError(null);
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
        private void resetMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "Value")
            {
                if (!string.IsNullOrEmpty(m.Value))
                {
                    int _value;
                    bool _bvalue;
                    if (int.TryParse(m.Value, out _value))
                    {
                        if (_value != 0 && !bPushing)
                        {
                            bResetting = true;
                            ResetButton_Click(null, null);
                            m.Value = "0";
                        }
                        else if (_value == 0)
                        {
                            bResetting = false;
                        }
                    }
                    else if (bool.TryParse(m.Value, out _bvalue))
                    {
                        if (_bvalue && !bPushing)
                        {
                            bResetting = true;
                            ResetButton_Click(null, null);
                            m.Value = "False";
                        }
                        else if (!_bvalue)
                        {
                            bResetting = false;
                        }
                    }
                    SetEntityError(null);
                }
            }
        }

        //private void InitValues(bool forceHistoryLoading = false)
        //{
        //    if (PenReferenceList == null || PenReferenceList.Count == 0)
        //        return;

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

        //        if (forceHistoryLoading)
        //        {
        //            string keyname = key.Split('_').First();
        //            int index;
        //            int.TryParse(key.Split('_').Last(), out index);
        //            if (!OpcuaEntityReference[key].IsRelative && viewList != null && viewList.ContainsKey(keyname))
        //                viewList[keyname].UpdateReferences(monitoredItem.NodeIdModel, (PlotTypeEnum)index);
        //        }
        //    }
        //}

        //private void InitValue(string key,MonitoredItemViewModel monitoreditem)
        //{
        //    UpdateRange(monitoreditem);
        //    UpdateMonitoredValue(key, monitoreditem);
        //}

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
            if (chart == null || chart.Diagram == null)
                return;
            try
            {
                (from s in chart.Diagram.Series where s != dataContextSerie select s).ToList().ForEach(s => chart.Diagram.Series.Remove(s));
                mapKeySeries.Clear();
            }
            catch
            {
            }
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

        private void AddSeries(bool hardReload = false)
        {
            if (chart.Diagram == null)
                return;

            var range = GetMinMaxRange();
            Maximum = range.High;
            Minimum = range.Low;

            var penStyles = Enum.GetValues(typeof(PredefinedPenKinds)).Cast<PredefinedPenKinds>();

            if(!bcInit || hardReload)
            {
                mapKeySeries.Clear();
                mapKeySeriesLabelForeground.Clear();

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
            }
            var _data = new DataValue() { Value = 0};
            for (int key = 0; key < PenReferenceList.Count; key++)
            {
                string nodeID1 = $"{PenReferenceList[key].NodeId}{ArgumentPlaceHolder}";// PenReferenceList[key].XTagReference.ResolvedNodeId != null ? PenReferenceList[key].XTagReference.ResolvedNodeId.ToString() : PenReferenceList[key].XTagReference.RelativePath;
                string nodeID2 = $"{PenReferenceList[key].NodeId}{ValuePlaceHolder}";// PenReferenceList[key].YTagReference.ResolvedNodeId != null ? PenReferenceList[key].YTagReference.ResolvedNodeId.ToString() : PenReferenceList[key].YTagReference.RelativePath;
                string skey = PenReferenceList[key].NodeId;
                Series addedSeries = LoadSeries();

                if (addedSeries != null)
                {
                    addedSeries.AnimationAutoStartMode = AnimationAutoStartMode.PlayOnce;

                    if (PenReferenceList[key].StrokeThickness > 0)
                        (addedSeries as LineScatterSeries2D).LineStyle = new LineStyle((int)PenReferenceList[key].StrokeThickness);
                    (addedSeries as LineScatterSeries2D).Visible = PenReferenceList[key].Visible;
                    if (mapKeySeries.ContainsKey(skey))
                        mapKeySeries[skey] = addedSeries;
                    else
                        mapKeySeries.Add(skey, addedSeries);

                    if(mapKeySeriesLabelForeground.ContainsKey(skey))
                        mapKeySeriesLabelForeground[skey] = PenReferenceList[key].LFColor;
                    else
                        mapKeySeriesLabelForeground.Add(skey, PenReferenceList[key].LFColor);

                    ChartXYDataGenerator view;

                    if (!viewList.ContainsKey(skey))
                    {
                        /*
                         * viewList[skey] = new ChartXYDataGenerator(
                         * skey, 
                         * SampleNumber, 
                         * SampleNumber, 
                         * ConnectionString
                         * , ConnectionString, 
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
                        viewList[skey] = new ChartXYDataGenerator(skey, settings, CommandTimeout);
                        viewList[skey].Error += viewList_OnError;
                        viewList[skey].HistoryLoaded += SeriesHistoryLoaded;
                    }

                    view = viewList[skey];
                    viewList[PenReferenceList[key].NodeId] = view;
                    mapKeySeries[PenReferenceList[key].NodeId] = addedSeries;
                    addedSeries.Name = string.Format("Series{0}", key);

                    string _LinkedPenName = PenReferenceList[key].Name;
                    addedSeries.DisplayName = TranslationHelper.TranlslateText(_LinkedPenName, stringlist, _LinkedPenName);

                    InitSeries(addedSeries);
                    chart.Diagram.Series.Add(addedSeries);
                    //chart.Diagram.Series.Insert(key, addedSeries);
                }
            }

            UpdateAxisRange();
        }

        private void viewList_OnError(object sender, WPFPenHelpers.ErrorEventArgs e)
        {
            log.Error(string.Format(Properties.Resources.ErrorLoadingValues, e.ErrorMessage));
            if (iUFProjectManager != null)
                iUFProjectManager.AddLogEntity(Document, Properties.Resources.ChartXYControlLog,
              DateTime.UtcNow, string.Format(Properties.Resources.ErrorLoadingValues, e.ErrorMessage),
              System.Diagnostics.EventLogEntryType.Error);
        }

        private void SeriesHistoryLoaded(object sender, EventArgs e)
        {
            ChartXYDataGenerator _data = sender as ChartXYDataGenerator;
            string _key = _data.PenId;
            var _viewlist = viewList.ContainsKey(_key) ? viewList[_key] : null;
            if (_viewlist != null)
            {
                if (mapKeySeries.ContainsKey(_key))
                    DrawPoints(mapKeySeries[_key], viewList[_key].Values);
            }
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
                    UpdateModel(monitoredKey, m.NodeIdModel, PenTypeEnum.XY);
            }
        }

        //private void InitMonitoredValue()
        //{
        //    if (string.IsNullOrEmpty(monitoredKey))
        //        return;
        //    DataValue mDataValue = null;
        //    if(monitoredItemViewModel != null)
        //        mDataValue = monitoredItemViewModel.DataValue;
        //    if (mDataValue != null)
        //    {
        //        if (Opc.Ua.StatusCode.IsGood(mDataValue.StatusCode) ||
        //            mDataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
        //        {
        //            InitValue(monitoredKey, monitoredItemViewModel);
        //        }
        //    }
        //}

        private void AddMonitoredSeries()
        {
            if (monitoredItemViewModel == null)
                return;

            InitChartControl();

            var range = GetMinMaxRange();
            Maximum = range.High;
            Minimum = range.Low;

            if (string.IsNullOrEmpty(monitoredKey))
                monitoredKey = Guid.NewGuid().ToString();

            if (dataContextSerie != null && chart.Diagram.Series.Contains(dataContextSerie))
            {
                chart.BeginInit();
                chart.Diagram.Series.Remove(dataContextSerie);
                chart.EndInit();
            }

            dataContextSerie = LoadSeries();

            if (dataContextSerie != null)
            {
                if (viewDataContext == null)
                {
                    /*
                     * viewDataContext = new ChartXYDataGenerator(
                     * monitoredKey, 
                     * SampleNumber, 
                     * SampleNumber, 
                     * ConnectionString, 
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
                    viewDataContext = new ChartXYDataGenerator(monitoredKey, settings, CommandTimeout);
                    viewDataContext.Error += viewList_OnError;
                    viewDataContext.HistoryLoaded += MonitoredHistoryLoaded;
                }

                dataContextSerie.DataSource = null;
                dataContextSerie.AnimationAutoStartMode = AnimationAutoStartMode.PlayOnce;

                (dataContextSerie as LineScatterSeries2D).LineStyle = new LineStyle((int)1);

                dataContextSerie.Name = "dataContextSerie";
                dataContextSerie.DisplayName = TranslationHelper.TranlslateText(LinkedPenName, stringlist, LinkedPenName);
                InitSeries(dataContextSerie);

                chart.BeginInit();
                chart.Diagram.Series.Add(dataContextSerie);
                chart.EndInit();
            }

            if (cts == null)
                cts = new CancellationTokenSource();
            var token = cts.Token;
            var task1 = Task.Factory.StartNew(() =>
            {
                if (token.IsCancellationRequested)
                    return;

                var b = monitoredItemViewModel.HasRange;
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            var task2 = task1.ContinueWith(ret =>
            {
                if (token.IsCancellationRequested)
                    return;

                UpdateRange(monitoredItemViewModel);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void MonitoredHistoryLoaded(object sender, EventArgs e)
        {
            if (dataContextSerie != null && viewDataContext != null)
                DrawPoints(dataContextSerie, viewDataContext.Values);
        }

        private void UpdateAxisTitles()
        {
            try
            {
                (chart.Diagram as XYDiagram2D).AxisX.Title.Content = TranslationHelper.TranslateComposedText(XTitle, stringlist, XTitle);
                (chart.Diagram as XYDiagram2D).AxisY.Title.Content = TranslationHelper.TranslateComposedText(YTitle, stringlist, YTitle);
            }
            catch (Exception)
            {
            }
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
                    }
                }
            }
            else
            {
                if (dataContextSerie == null)
                    dataContextSerie = (from c in chart.Diagram.Series
                                        where c.Name == "dataContextSerie"
                                        select c).FirstOrDefault();
                if (dataContextSerie != null)
                {
                    dataContextSerie.DisplayName = TranslationHelper.TranlslateText(LinkedPenName, stringlist, LinkedPenName);
                }

                for (int key = 0; key < PenReferenceList.Count; key++)
                {
                    Series addedSeries = (from s in chart.Diagram.Series where s.Name == string.Format("Series{0}", key) select s).FirstOrDefault();
                    if (addedSeries != null)
                    {
                        string _LinkedPenName = PenReferenceList[key].Name;
                        addedSeries.DisplayName = TranslationHelper.TranlslateText(_LinkedPenName, stringlist, _LinkedPenName);
                    }
                }
            }
        }
        string stringPlaceolder = "ChartXY";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesignmode && StringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = StringManager.GetListStringForCulture(Document, StringManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                UpdateToolbar();
                UpdateSeries();
                UpdateAxisTitles();
            });
        }

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

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                IUIMsgBoxAlertService UIInterface = null;
                IHelpProvider helpProvider = null;
                if (Document != null)
                {
                    UIInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                }
                if (workspace == null && Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

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

                    mapDataTemplates.Add(XTitleProperty, dt);
                    mapDataTemplates.Add(YTitleProperty, dt);
                }

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.SmartPropertiesEditor));
                factory.SetValue(Controls.SmartPropertiesEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

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

            DetachOverrideBaseProperties();

            if (checkbox != null)
                checkbox.ItemClick -= OnSeriesLabelVisibleClicked;
            if (checkbox1 != null)
                checkbox1.ItemClick -= OnLegendAreaVisibleClicked;

            lock (queuedValues)
            {
                if (dpUpdateValue != null &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                    dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                    dpUpdateValue.Abort();
                queuedValues.Clear();
            }

            if (StringManager != null)
                StringManager.CultureChanged -= StringManager_CultureChanged;
                       
            SetBusyEffect(false);

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            if (!bDesignmode)
            {
                typeHelper.TerminateExecution(this, reset_PropertyChanged, resetMonitoredItemViewModel_PropertyChanged, reset, resetMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, push_PropertyChanged, pushMonitoredItemViewModel_PropertyChanged, push, pushMonitoredItemViewModel);
            }

            TerminateExecution();

            if (dpAxisScaleChanged != null && dpAxisScaleChanged.Status != DispatcherOperationStatus.Aborted &&
                dpAxisScaleChanged.Status != DispatcherOperationStatus.Completed)
                dpAxisScaleChanged.Abort();

            if (dpAutoHideToolbar != null && dpAutoHideToolbar.Status != DispatcherOperationStatus.Aborted &&
                dpAutoHideToolbar.Status != DispatcherOperationStatus.Completed)
                dpAutoHideToolbar.Abort();

            if (viewList != null)
            {
                foreach (var view in viewList.Values)
                    view.Dispose();
                viewList.Clear();
            }

            if (viewDataContext != null)
                viewDataContext.Dispose();

            if (penReferenceList != null)
                penReferenceList.Clear();

            if (chart != null)
            {
                chart.CustomDrawSeries -= chart_CustomDrawSeries;
                chart.MouseLeave -= chart_MouseLeave;
                chart.MouseMove -= chart_MouseMove;
                chart.AxisScaleChanged -= chart_AxisScaleChanged;

                if (chart.Diagram != null)
                {
                    chart.Diagram = LoadTemplate("diagram", ChartDiagram) as Diagram;
                    (chart.Diagram as XYDiagram2D).Zoom -= diagram_Zoom;
                    (chart.Diagram as XYDiagram2D).MouseWheel -= diagram_MouseWheel;
                }


                if (chart is IDisposable)
                    (chart as IDisposable).Dispose();
            }
            
            if (toolbartray != null)
            {
                toolbartray.MouseEnter -= ToolbarMouseEnter;
                toolbartray.MouseLeave -= ToolbarMouseLeave;
            }

            if (resetbutton != null)
            {
                resetbutton.ItemClick -= ResetButton_Click;
            }

            if (pushbutton != null)
            {
                pushbutton.ItemClick -= PushButton_Click;
            }

            if (sbLeave != null)
                sbLeave.Stop();

            if (sbOver != null)
                sbOver.Stop();

            if (opcuaEntityReference != null)
                opcuaEntityReference.Clear();

            mapKeySeries.Clear();
            mapKeySeriesLabelForeground.Clear();            

            if (mapHandlers != null)
                mapHandlers.Clear();

            typeHelper.Dispose();
        }


        Object lockObj = new object();
        #endregion

        #region Constructor
        //DispatcherTimer timer = new DispatcherTimer();
        public ChartXY()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            OverrideBaseProperties();

#if !WINDOWS_UWP
            this.AddToolBarStyleResource();
#endif
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (DesignerProperties.GetIsInDesignMode(this))
                        bDesignmode = true;

                    bAutoHideToolbar = !AutoHideToolbar;
                    //bUpdatePrepare = true;

                    InitChartLayout();

                    if (Document != null)
                    {
                        if (StringManager == null)
                            StringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (StringManager != null)
                        {
                            StringManager_CultureChanged(null, null);
                            StringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (!bDesignmode)
                    {
                        sampleNumber = SampleNumber;
                        InitControl();
                        IsHitTestVisible = true;
                    }
                    else
                    {
                        grid.IsHitTestVisible = false;
                        UpdateSeries();
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
                }
            };
        }
        private void TerminateExecution(string key)
        {
            lock (lockObj)
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
        private void PrepareExecution(string key)
        {
            lock (lockObj)
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
        }

        private bool bcInit;
        #endregion

        #region PenItemHelper Event Handlers
        private void PenItem_OnError(object sender, WPFPenHelpers.ErrorEventArgs e)
        {
            SetEntityError(e.ErrorMessage);
        }

        private void PenItem_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;
            var keyName = helper.Key;
            var penType = helper.PenType;

            if (!e.Model.IsUserReadable || !e.Model.IsReadable)
                return;

            UpdateModel(keyName, e.Model, penType);
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

        #region IDynamicTagAware
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (PenList != null)
                foreach (var pen in PenList)
                {
                    if (pen.XTagReference != null /*&& pen.XTagReference.IsValid*/)
                        ret.Add(pen.CreateUniqueName($"{pen.Name}{ArgumentPlaceHolder}", ret.Keys.ToList()), pen.XTagReferenceXml);
                    if (pen.YTagReference != null /*&& pen.YTagReference.IsValid*/)
                        ret.Add(pen.CreateUniqueName($"{pen.Name}{ValuePlaceHolder}", ret.Keys.ToList()), pen.YTagReferenceXml);
                }

            if (Push != null && Push.TagReference != null /*&& Push.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(PushProperty.Name, ret.Keys.ToList()), Push.TagReferenceXml);

            if (Reset != null && Reset.TagReference != null /*&& Reset.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(ResetProperty.Name, ret.Keys.ToList()), Reset.TagReferenceXml);

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

            XYPenItemList penList = new XYPenItemList(PenReferenceList);
            var penTagXList = (from pen in penList where pen.XTagReference != null select pen).ToList();
            var penTagYList = (from pen in penList where pen.YTagReference != null select pen).ToList();
            int mapCount = penTagXList.Count + penTagYList.Count + (Push != null && Push.TagReference != null ? 1 : 0) + (Reset != null && Reset.TagReference != null ? 1 : 0);
            foreach (var pen in penList)
            {
                if (pen.XTagReference != null /*&& pen.XTagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                            ret = relative == pen.XTagReferenceXml;
                        else if (_absolute != null)
                        {
                            if (relative == pen.XTagReferenceXml)
                            {
                                string key = $"{ pen.NodeId}{ArgumentPlaceHolder}";
                                matchChangedMap.Add(key);
                                TerminateExecution(key);
                                if(_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.XTagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                }
                                else
                                {
                                    pen.XTagReference = _absolute;
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
                if (pen.YTagReference != null /*&& pen.YTagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                            ret = relative == pen.YTagReferenceXml;
                        else if (_absolute != null)
                        {
                            if (relative == pen.YTagReferenceXml)
                            {
                                string key = $"{ pen.NodeId}{ValuePlaceHolder}";
                                matchChangedMap.Add(key);
                                TerminateExecution(key);
                                if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.YTagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                }
                                else
                                {
                                    pen.YTagReference = _absolute;
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
            {
                penReferenceList = penList;
            }

            return ret || cret;
        }
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            XYPenItemList penList = new XYPenItemList(PenList);
            foreach (var pen in penList)
            {
                if (pen.XTagReference != null /*&& pen.XTagReference.IsValid*/)
                {
                    if (map.ContainsKey($"{pen.NodeId}{ArgumentPlaceHolder}"))
                        pen.XTagReferenceXml = map[$"{pen.NodeId}{ArgumentPlaceHolder}"];
                    else
                        pen.XTagReferenceXml = typeHelper.UpdateTag(pen.XTagReferenceXml, map);
                }
                if (pen.YTagReference != null /*&& pen.YTagReference.IsValid*/)
                {
                    if (map.ContainsKey($"{pen.NodeId}{ValuePlaceHolder}"))
                        pen.YTagReferenceXml = map[$"{pen.NodeId}{ValuePlaceHolder}"];
                    else
                        pen.YTagReferenceXml = typeHelper.UpdateTag(pen.YTagReferenceXml, map);
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

        void UpdateRange(MonitoredItemViewModel monitoreditem)
        {
            if (monitoreditem == null)
                return;

            if (monitoreditem.HasRange && monitoreditem.Range != null)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (bDisposed)
                        return;

                    UpdateRange(monitoreditem.Range.Low, monitoreditem.Range.High);
                });
            }
        }

        void UpdateRange(double minValue, double maxValue)
        {
            if (UseEUMinMaxValues)
            {
                Minimum = Math.Max(Minimum, minValue);
                Maximum = Math.Max(Maximum, maxValue);
                UpdateAxisRange();
            }
        }

        void chart_AxisScaleChanged(object sender, AxisScaleChangedEventArgs arg)
        {
            DispatchScalePaddingSet(arg.Axis);
        }

        void DispatchScalePaddingSet(AxisBase axis)
        {
            lastAxisScaleChangedAxis = axis;
            if (dpAxisScaleChanged == null ||
                dpAxisScaleChanged.Status == DispatcherOperationStatus.Completed ||
                dpAxisScaleChanged.Status == DispatcherOperationStatus.Aborted)
            {
                dpAxisScaleChanged = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                {
                    if (bDisposed)
                        return;

                    var yAxis = (chart.Diagram as XYDiagram2D).AxisY;
                    if (lastAxisScaleChangedAxis != yAxis)
                        return;

                    if (!AutomaticScale)
                    {
                        yAxis.ActualWholeRange.SideMarginsValue = 0;
                        return;
                    }

                    double? actualMaxValue = null;
                    double? actualMinValue = null;

                    try
                    {
                        actualMaxValue = Convert.ToDouble(yAxis.ActualWholeRange.ActualMaxValue);
                        actualMinValue = Convert.ToDouble(yAxis.ActualWholeRange.ActualMinValue);
                    }
                    catch
                    {
                    }

                    if (actualMaxValue.HasValue && actualMinValue.HasValue)
                    {
                        var oldMargin = yAxis.ActualWholeRange.SideMarginsValue;
                        yAxis.ActualWholeRange.SideMarginsValue = ((yAxis.ActualWholeRange.ActualMaxValueInternal - yAxis.ActualWholeRange.SideMarginsValue) - (yAxis.ActualWholeRange.ActualMinValueInternal + yAxis.ActualWholeRange.SideMarginsValue)) * ScalePaddingFactor / 100;
                        if (double.IsInfinity(yAxis.ActualWholeRange.ActualMaxValueInternal) || double.IsInfinity(yAxis.ActualWholeRange.ActualMinValueInternal))
                            yAxis.ActualWholeRange.SideMarginsValue = oldMargin;
                    }
                });
            }
        }
                
        private void UpdateAxisRange()
        {
            if(AutomaticScale)
            {
                (chart.Diagram as XYDiagram2D).AxisX.ActualWholeRange.SetAuto();
                (chart.Diagram as XYDiagram2D).AxisY.ActualVisualRange.SetAuto();
            }
            else if (!AutomaticScale || bDesignmode)
            {
                (chart.Diagram as XYDiagram2D).AxisX.ActualWholeRange.MinValue = Minimum;
                (chart.Diagram as XYDiagram2D).AxisX.ActualWholeRange.MaxValue = Maximum;
                (chart.Diagram as XYDiagram2D).AxisY.ActualWholeRange.MinValue = Minimum;
                (chart.Diagram as XYDiagram2D).AxisY.ActualWholeRange.MaxValue = Maximum;
            }

        }

        void UpdateModel(string keyName, NodeIdViewModel model, PenTypeEnum penType)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDisposed)
                    return;

                if (keyName == monitoredKey)
                {
                    if (viewDataContext != null)
                        viewDataContext.UpdateReferences(model, penType);
                }
                else
                {
                    string keyPen = penType == PenTypeEnum.X ? keyName.Substring(0, keyName.LastIndexOf(ArgumentPlaceHolder)) : keyName.Substring(0, keyName.LastIndexOf(ValuePlaceHolder));
                    if (viewList != null && viewList.ContainsKey(keyPen))
                        viewList[keyPen].UpdateReferences(model, penType);
                }
            });
        }

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
            var _key = CheckPlaceHolder(k);
            bool isArray = value is System.Collections.IList;
            int count = 0;
            if (isArray)
            {
                var dataCollection = value as List<OPCUAViewModel.MonitoredItemViewModel.DataObject>;
                if (dataCollection != null && dataCollection.Count > 0)
                {
                    count = Math.Min(sampleNumber, dataCollection.Count);
                    if (viewDataContext != null && _key == monitoredKey)
                    {
                        for (ushort ii = 0; ii < count; ii++)
                        {
                            viewDataContext.AddData(dataCollection[ii].Value, dataCollection[ii].Value, ii);
                        }
                    }
                    else if (viewList != null && viewList.ContainsKey(_key))
                    {
                        for (ushort ii = 0; ii < count; ii++)
                        {
                            if (k.EndsWith(ArgumentPlaceHolder))
                                viewList[_key].AddData(dataCollection[ii].Value, null, ii);
                            else if (k.EndsWith(ValuePlaceHolder))
                                viewList[_key].AddData(null, dataCollection[ii].Value, ii);
                            else
                                viewList[_key].AddData(dataCollection[ii].Value, dataCollection[ii].Value, ii);
                        }
                    }
                }
            }
            else if (viewDataContext != null && _key == monitoredKey)
            {
                var newValue = value as DataValue;
                viewDataContext.AddData(newValue.Value, newValue.Value, iCount);
            }
            else if (viewList != null && viewList.ContainsKey(_key))
            {
                var newValue = value as DataValue;
                if (k.EndsWith(ArgumentPlaceHolder))
                    viewList[_key].AddData(newValue.Value, null, iCount);
                else if (k.EndsWith(ValuePlaceHolder))
                    viewList[_key].AddData(null, newValue.Value, iCount);
                else
                    viewList[_key].AddData(newValue.Value, newValue.Value, iCount);
            }

            UpdateView(_key);
        }

        void UpdateView(string key)
        {
            if (viewDataContext != null && monitoredKey == key && dataContextSerie != null)
                DrawPoints(dataContextSerie, viewDataContext.Values);

            if (viewList != null && viewList.ContainsKey(key) && mapKeySeries.ContainsKey(key))
                DrawPoints(mapKeySeries[key], viewList[key].Values);
        }

        static string CheckPlaceHolder(string key)
        {
            if (key.EndsWith(ArgumentPlaceHolder))
                return key.Substring(0, key.Length - ArgumentPlaceHolder.Length);
            else if (key.EndsWith(ValuePlaceHolder))
                return key.Substring(0, key.Length - ValuePlaceHolder.Length);
            else
                return key;
        }

        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
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
                    (document, typeof(ChartXY), LinkedPenNameProperty).DisplayName;
                map.Add(propertyName, LinkedPenName);
            }
            if (!string.IsNullOrEmpty(XTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(ChartXY), XTitleProperty).DisplayName;
                map.Add(propertyName, XTitle);
            }
            if (!string.IsNullOrEmpty(YTitle))
            {
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(ChartXY), YTitleProperty).DisplayName;
                map.Add(propertyName, YTitle);
            }
            int i = 1;
            PenList?.Where(x => !string.IsNullOrEmpty(x.Name)).ToList().ForEach(x =>
            {
                map.Add(CreateUniqueName(x.Name, map.Keys.ToList()), x.Name);
                i++;
            });
            return map;
        }
        #endregion

    }
}
