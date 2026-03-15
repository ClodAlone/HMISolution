using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using System.ComponentModel;
using DevExpress.Xpo;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Utilities.WPF;
using DevExpress.Xpf.Docking;
using System.Threading;
using Utilities;
using System.Windows.Media.Animation;
using WPFUtilities;
using DevExpress.Charts.Designer;
using DevExpress.Xpf.Printing;
using System.Text.RegularExpressions;
using log4net;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using ScreenSettings;
using System.Data;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using StringManager.ComponentService;
using System.Xml.Serialization;
using UFUAHistorianModel;
using OPCUAViewModel;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces;
using DynamicTagAwareHelper;
using TranslationHelpers;
using ViewModelLib;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using System.Windows.Threading;
using WPFPenHelpers;
using System.Globalization;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using Opc.Ua;
using System.Collections;
using System.Windows.Data;
using GlobalConverters = Converters;
using TrendRealTimeSlim.Converters;
using DevExpress.Mvvm.Native;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using StorageHelper;
using System.Windows.Automation.Peers;
using TrendRealTimeSlim.Automations;
using WPFUtilities.HistoricalHelpers;
using Utilities.Converters;
using UnitConverterManager.ComponentService;

namespace TrendRealTimeSlim
{
    /// <summary>
    /// Interaction logic for TrendRealTime.xaml
    /// </summary>
    public partial class RealTimeTrendControl : UserControl, IEntityReference, IContainPropertyEditors, IDisposable, IDynamicTagAware, ISettingsHelper, INotifyPropertyVisibilityChanged
        , IStringIDAware, IGridLayoutUser
    {
        #region Dependency Properties

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }


        #region retrocompatibility

        #region DateFormat
        public static readonly DependencyProperty DateFormatProperty = DependencyProperty.Register("DateFormat", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata(null));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public string DateFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DateFormatProperty);
            }
            set
            {
                SetValue(DateFormatProperty, value);
            }
        }
        #endregion
        #region TimeFormat
        public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register("TimeFormat", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata(null));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public string TimeFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TimeFormatProperty);
            }
            set
            {
                SetValue(TimeFormatProperty, value);
            }
        }
        #endregion

        #region PrimaryAxisLabelForeground
        public static readonly DependencyProperty PrimaryAxisLabelForegroundProperty = DependencyProperty.Register("PrimaryAxisLabelForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(Brushes.Transparent));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush PrimaryAxisLabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(PrimaryAxisLabelForegroundProperty);
            }
            set
            {
                SetValue(PrimaryAxisLabelForegroundProperty, value);
            }
        }
        #endregion
        #region SecAxisLabelForeground
        public static readonly DependencyProperty SecAxisLabelForegroundProperty = DependencyProperty.Register("SecAxisLabelForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(Brushes.Transparent));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush SecAxisLabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SecAxisLabelForegroundProperty);
            }
            set
            {
                SetValue(SecAxisLabelForegroundProperty, value);
            }
        }
        #endregion
        #region PenAreaBackground
        public static readonly DependencyProperty PenAreaBackgroundProperty = DependencyProperty.Register("PenAreaBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34))));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush PenAreaBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(PenAreaBackgroundProperty);
            }
            set
            {
                SetValue(PenAreaBackgroundProperty, value);
            }
        }
        #endregion
        #region AxisStroke
        public static readonly DependencyProperty AxisStrokeProperty = DependencyProperty.Register("AxisStroke", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnAxisStrokeChanged), new CoerceValueCallback(OnCoerceAxisStroke)));

        private static object OnCoerceAxisStroke(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceAxisStroke((Brush)value);
            else
                return value;
        }

        private static void OnAxisStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnAxisStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceAxisStroke(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAxisStrokeChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }
        public Brush AxisStroke
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(AxisStrokeProperty);
            }
            set
            {
                SetValue(AxisStrokeProperty, value);
            }
        }
        #endregion

        #region AxisStrokeThickness
        public static readonly DependencyProperty AxisStrokeThicknessProperty = DependencyProperty.Register("AxisStrokeThickness", typeof(double), typeof(RealTimeTrendControl), new UIPropertyMetadata(1.0));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public double AxisStrokeThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(AxisStrokeThicknessProperty);
            }
            set
            {
                SetValue(AxisStrokeThicknessProperty, value);
            }
        }
        #endregion
        #region PointFormat
        public static readonly DependencyProperty PointFormatProperty = DependencyProperty.Register("PointFormat", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata("{0:0.00}"));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public string PointFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(PointFormatProperty);
            }
            set
            {
                SetValue(PointFormatProperty, value);
            }
        }
        #endregion
        #region LabelFormat
        public static readonly DependencyProperty LabelFormatProperty = DependencyProperty.Register("LabelFormat", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata("0.00"));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
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
        #region ShowTagName
        public static readonly DependencyProperty ShowTagNameProperty = DependencyProperty.Register("ShowTagName", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public bool ShowTagName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowTagNameProperty);
            }
            set
            {
                SetValue(ShowTagNameProperty, value);
            }
        }
        #endregion
        #region PointFonstSettings
        public static readonly DependencyProperty PointFonstSettingsProperty = DependencyProperty.Register("PointFonstSettings", typeof(GlobalConverters.FontSettings), typeof(RealTimeTrendControl), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10)));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(typeof(GlobalConverters.ConvertFontSettings))]
        public GlobalConverters.FontSettings PointFonstSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (GlobalConverters.FontSettings)GetValue(PointFonstSettingsProperty);
            }
            set
            {
                SetValue(PointFonstSettingsProperty, value);
            }
        }
        #endregion
        #region TitleFonstSettings
        public static readonly DependencyProperty TitleFonstSettingsProperty = DependencyProperty.Register("TitleFonstSettings", typeof(GlobalConverters.FontSettings), typeof(RealTimeTrendControl), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10)));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(typeof(GlobalConverters.ConvertFontSettings))]
        public GlobalConverters.FontSettings TitleFonstSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (GlobalConverters.FontSettings)GetValue(TitleFonstSettingsProperty);
            }
            set
            {
                SetValue(TitleFonstSettingsProperty, value);
            }
        }
        #endregion
        #region Title
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata(string.Empty));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public string Title
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        #endregion
        #region TitleForeground
        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
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
        #region PlotBorderBrush
        public static readonly DependencyProperty PlotBorderBrushProperty = DependencyProperty.Register("PlotBorderBrush", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnPlotBorderBrushChanged), new CoerceValueCallback(OnCoercePlotBorderBrush)));

        private static object OnCoercePlotBorderBrush(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoercePlotBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnPlotBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnPlotBorderBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoercePlotBorderBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPlotBorderBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }
        public Brush PlotBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(PlotBorderBrushProperty);
            }
            set
            {
                SetValue(PlotBorderBrushProperty, value);
            }
        }
        #endregion
        #region PlotBorderThickness
        public static readonly DependencyProperty PlotBorderThicknessProperty = DependencyProperty.Register("PlotBorderThickness", typeof(Thickness), typeof(RealTimeTrendControl), new UIPropertyMetadata(new Thickness(1)));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Thickness PlotBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(PlotBorderThicknessProperty);
            }
            set
            {
                SetValue(PlotBorderThicknessProperty, value);
            }
        }

        #endregion
        #region PenAreaVisible
        public static readonly DependencyProperty PenAreaVisibleProperty = DependencyProperty.Register("PenAreaVisible", typeof(Boolean), typeof(RealTimeTrendControl), new UIPropertyMetadata(false));
        [Obsolete("Node more used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Boolean PenAreaVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(PenAreaVisibleProperty);
            }
            set
            {
                SetValue(PenAreaVisibleProperty, value);
            }
        }
        #endregion
        #endregion



        #region LegendAreaVisible
        public static readonly DependencyProperty LegendAreaVisibleProperty = DependencyProperty.Register("LegendAreaVisible", typeof(Boolean), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLegendAreaVisibleChanged), new CoerceValueCallback(OnCoerceLegendAreaVisible)));

        private static object OnCoerceLegendAreaVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceLegendAreaVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLegendAreaVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnLegendAreaVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLegendAreaVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLegendAreaVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDispose || !bInit)
                return;

            NestMainGridInsideDockManager();

            OnPropertyChanged("DockManagerAllowed");
        }

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

        #region NestedInDockManager
        bool notNestedInDockManager = true;
        [Browsable(false)]
        [SvgValueConverter(false)]
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

        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RealTimeTrendControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RealTimeTrendControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode))
            {
                var labelx = XAxsisFontSettings.Clone();
                var labely = YAxsisFontSettings.Clone();

                labelx.FontFamily = FontFamily;
                labely.FontFamily = FontFamily;

                XAxsisFontSettings = labelx;
                YAxsisFontSettings = labely;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode))
            {
                var labelx = XAxsisFontSettings.Clone();
                var labely = YAxsisFontSettings.Clone();

                labelx.FontWeight = FontWeight;
                labely.FontWeight = FontWeight;

                XAxsisFontSettings = labelx;
                YAxsisFontSettings = labely;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode))
            {
                var labelx = XAxsisFontSettings.Clone();
                var labely = YAxsisFontSettings.Clone();

                labelx.FontStyle = FontStyle;
                labely.FontStyle = FontStyle;

                XAxsisFontSettings = labelx;
                YAxsisFontSettings = labely;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode))
            {
                var labelx = XAxsisFontSettings.Clone();
                var labely = YAxsisFontSettings.Clone();

                labelx.FontSize = (int)FontSize;
                labely.FontSize = (int)FontSize;

                XAxsisFontSettings = labelx;
                YAxsisFontSettings = labely;
            }
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                AxisLabelForeground = Foreground;
                LegendAreaForeground = Foreground;
            }
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as RealTimeTrendControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled)
            {
                DiagramBackground = Background;
                PlotBackground = Background;
                TrendBackground = Background;
            }
        }
        #endregion

        #region AutoHidePanelsVisible
        public static readonly DependencyProperty AutoHidePanelsVisibleProperty = DependencyProperty.Register("AutoHidePanelsVisible", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutoHidePanelsVisibleChanged), new CoerceValueCallback(OnCoerceAutoHidePanelsVisible)));

        private static object OnCoerceAutoHidePanelsVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceAutoHidePanelsVisible((bool)value);
            else
                return value;
        }

        private static void OnAutoHidePanelsVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnAutoHidePanelsVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutoHidePanelsVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoHidePanelsVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("DataAnalisysOptions")]
        public bool AutoHidePanelsVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutoHidePanelsVisibleProperty);
            }
            set
            {
                SetValue(AutoHidePanelsVisibleProperty, value);
            }
        }
        #endregion

        #region XYFontsettings
        public static readonly DependencyProperty YAxsisFontSettingsProperty = DependencyProperty.Register("YAxsisFontSettings", typeof(GlobalConverters.FontSettings), typeof(RealTimeTrendControl), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 12), new PropertyChangedCallback(OnYAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceYAxsisFontSettings)));

        private static object OnCoerceYAxsisFontSettings(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceYAxsisFontSettings((GlobalConverters.FontSettings)value);
            else
                return value;
        }

        private static void OnYAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnYAxsisFontSettingsChanged((GlobalConverters.FontSettings)e.OldValue, (GlobalConverters.FontSettings)e.NewValue);
        }

        protected virtual GlobalConverters.FontSettings OnCoerceYAxsisFontSettings(GlobalConverters.FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYAxsisFontSettingsChanged(GlobalConverters.FontSettings oldValue, GlobalConverters.FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDispose)
                return;

            SetAxisFontSettings(newValue, axisY);
            (from c in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select c).ToList().ForEach(ySecAxis => {
                SetAxisFontSettings(newValue, ySecAxis);
            });
        }

        [Category("DataAnalisysOptions")]
        [SvgValueConverter(typeof(GlobalConverters.ConvertFontSettings))]
        public GlobalConverters.FontSettings YAxsisFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (GlobalConverters.FontSettings)GetValue(YAxsisFontSettingsProperty);
            }
            set
            {
                SetValue(YAxsisFontSettingsProperty, value);
            }
        }


        public static readonly DependencyProperty XAxsisFontSettingsProperty = DependencyProperty.Register("XAxsisFontSettings", typeof(GlobalConverters.FontSettings), typeof(RealTimeTrendControl), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 12), new PropertyChangedCallback(OnXAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceXAxsisFontSettings)));

        private static object OnCoerceXAxsisFontSettings(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceXAxsisFontSettings((GlobalConverters.FontSettings)value);
            else
                return value;
        }

        private static void OnXAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnXAxsisFontSettingsChanged((GlobalConverters.FontSettings)e.OldValue, (GlobalConverters.FontSettings)e.NewValue);
        }

        protected virtual GlobalConverters.FontSettings OnCoerceXAxsisFontSettings(GlobalConverters.FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXAxsisFontSettingsChanged(GlobalConverters.FontSettings oldValue, GlobalConverters.FontSettings newValue)
        {
            SetAxisFontSettings(newValue, axisX);
        }

        [Category("DataAnalisysOptions")]
        [SvgValueConverter(typeof(GlobalConverters.ConvertFontSettings))]
        public GlobalConverters.FontSettings XAxsisFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (GlobalConverters.FontSettings)GetValue(XAxsisFontSettingsProperty);
            }
            set
            {
                SetValue(XAxsisFontSettingsProperty, value);
            }
        }
        #endregion

        #region DefToolbarHeight
        public static readonly DependencyProperty DefToolbarHeightProperty = DependencyProperty.Register("DefToolbarHeight", typeof(double), typeof(RealTimeTrendControl), new UIPropertyMetadata(25d));
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

        #region MeasureUnitMultiplier
        public static readonly DependencyProperty MeasureUnitMultiplierProperty = DependencyProperty.Register("MeasureUnitMultiplier", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(1, new PropertyChangedCallback(OnMeasureUnitMultiplierChanged), new CoerceValueCallback(OnCoerceMeasureUnitMultiplier)));

        private static object OnCoerceMeasureUnitMultiplier(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceMeasureUnitMultiplier((int)value);
            else
                return value;
        }

        private static void OnMeasureUnitMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnMeasureUnitMultiplierChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMeasureUnitMultiplier(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMeasureUnitMultiplierChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            SetDateTimeScaleMeasureUnitMultiplier(newValue);
        }

        public int MeasureUnitMultiplier
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MeasureUnitMultiplierProperty);
            }
            set
            {
                SetValue(MeasureUnitMultiplierProperty, value);
            }
        }

        #endregion


        #region TextPattern
        public static readonly DependencyProperty TextPatternProperty = DependencyProperty.Register("TextPattern", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata("{A}: {V:F2}"));
        [Browsable(false)]
        [XmlIgnore]
        public string TextPattern
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TextPatternProperty);
            }
            set
            {
                SetValue(TextPatternProperty, value);
            }
        }

        #endregion

        #region LegendAreaForeground
        public static readonly DependencyProperty LegendAreaForegroundProperty = DependencyProperty.Register("LegendAreaForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnLegendAreaForegroundChanged), new CoerceValueCallback(OnCoerceLegendAreaForeground)));

        private static object OnCoerceLegendAreaForeground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceLegendAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnLegendAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
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
            if (bInit)
            {
                UpdateControlLayout();
            }
        }
        [Category("DataAnalisysOptions")]
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

        #region PointPrecision
        public static readonly DependencyProperty PointPrecisionProperty = DependencyProperty.Register("PointPrecision", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(2, new PropertyChangedCallback(OnPointPrecisionChanged), new CoerceValueCallback(OnCoercePointPrecision)));

        private static object OnCoercePointPrecision(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoercePointPrecision((int)value);
            else
                return value;
        }

        private static void OnPointPrecisionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnPointPrecisionChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoercePointPrecision(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPointPrecisionChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                StringBuilder stringformat = new StringBuilder("{V:0");
                if (newValue > 0)
                {
                    stringformat.Append(".");
                    for (int i = 0; i < newValue; i++)
                        stringformat.Append("0");
                }
                stringformat.Append("} ({A})");
                TextPattern = stringformat.ToString();
                AxisYTextPattern = $"{{V:F{newValue}}}";
            }
        }

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


        #region AxisYTextPattern
        public static readonly DependencyProperty AxisYTextPatternProperty = DependencyProperty.Register("AxisYTextPattern", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata("{V:F2}"));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public string AxisYTextPattern
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(AxisYTextPatternProperty);
            }
            set
            {
                SetValue(AxisYTextPatternProperty, value);
            }
        }
        #endregion

        #region AxisLabelForeground
        public static readonly DependencyProperty AxisLabelForegroundProperty = DependencyProperty.Register("AxisLabelForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxisLabelForegroundChanged), new CoerceValueCallback(OnCoerceAxisLabelForeground)));

        private static object OnCoerceAxisLabelForeground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceAxisLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnAxisLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnAxisLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceAxisLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAxisLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }

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
        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnPlotBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoercePlotBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPlotBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }

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

        #region DiagramBackground
        public static readonly DependencyProperty DiagramBackgroundProperty = DependencyProperty.Register("DiagramBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34))));

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush DiagramBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(DiagramBackgroundProperty);
            }
            set
            {
                SetValue(DiagramBackgroundProperty, value);
            }
        }

        #endregion
        #region TrendBackground
        public static readonly DependencyProperty TrendBackgroundProperty = DependencyProperty.Register("TrendBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnTrendBackgroundChanged), new CoerceValueCallback(OnCoerceTrendBackground)));

        private static object OnCoerceTrendBackground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceTrendBackground((Brush)value);
            else
                return value;
        }

        private static void OnTrendBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnTrendBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTrendBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTrendBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }

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

        private void UpdateControlLayout()
        {
            if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
            {
                axisX.Label.Foreground = AxisLabelForeground;
                //axisX.Label.FontSize = FontSize;
                axisY.Label.Foreground = AxisLabelForeground;
                //axisY.Label.FontSize = FontSize;
            }

            if (this.ReadLocalValue(TrendBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                dockManager.Background = TrendBackground;
                sfchart.Background = TrendBackground;
                legend_GridControl.Background = TrendBackground;
                printGrid.Background = TrendBackground;
                //legendListBox.Background = TrendBackground;
                gridControl.Background = TrendBackground;
                gridPanel.Background = TrendBackground;
                chartPanel.Background = TrendBackground;
                legendPanel.Background = TrendBackground;
            }

            if (this.ReadLocalValue(PlotBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                var childrenList = from c in sfchart.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                    where c.Name == "PART_DomainBackground" || c.Name == "OutsideBorder"
                    select c;

                foreach(var c in childrenList)
                {
                    c.Background = PlotBackground;
                }
            }

            if (this.ReadLocalValue(PlotBorderBrushProperty) != DependencyProperty.UnsetValue)
            {
                (sfchart.Diagram as XYDiagram2D).DefaultPane.DomainBorderBrush = PlotBorderBrush;
            }

            if (PrimaryAxisGridColor != null)
                (axisX as AxisBase).GridLinesBrush = PrimaryAxisGridColor;
            else
                (axisX as AxisBase).ClearValue(AxisBase.GridLinesBrushProperty);
            if (SecAxisGridColor != null)
                (axisY as AxisBase).GridLinesBrush = SecAxisGridColor;
            else
                (axisY as AxisBase).ClearValue(AxisBase.GridLinesBrushProperty);
            if (XMinorGridLineColor != null)
                (axisX as AxisBase).GridLinesMinorBrush = XMinorGridLineColor;
            else
                (axisX as AxisBase).ClearValue(AxisBase.GridLinesMinorBrushProperty);
            if (GridColor != null)
                (axisY as AxisBase).GridLinesMinorBrush = GridColor;
            else
                (axisY as AxisBase).ClearValue(AxisBase.GridLinesMinorBrushProperty);
        }

        void UpdateXMinorCount()
        {
            (axisX as AxisBase).MinorCount = XSmallTicksPerInterval;
        }

        void UpdateYMinorCount()
        {
            (axisY as AxisBase).MinorCount = YSmallTicksPerInterval;

            foreach (var axis in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY)
            {
                (axis as AxisBase).MinorCount = YSmallTicksPerInterval;
            }
        }

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("DataAnalisysOptions")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("DataAnalisysOptions")]
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

        #region XAutoGrid
        public static readonly DependencyProperty XAutoGridProperty = DependencyProperty.Register("XAutoGrid", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXAutoGridChanged), new CoerceValueCallback(OnCoerceXAutoGrid)));

        private static object OnCoerceXAutoGrid(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnXAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXAutoGridChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceXAutoGrid(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXAutoGridChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateXAxisRange();
            }
            OnPropertyVisiblityChanged("XAutoGrid");
        }

        public bool XAutoGrid
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(XAutoGridProperty);
            }
            set
            {
                SetValue(XAutoGridProperty, value);
            }
        }

        #endregion
        #region XGridLineVisible
        public static readonly DependencyProperty XGridLineVisibleProperty = DependencyProperty.Register("XGridLineVisible", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXGridLineVisible)));

        private static object OnCoerceXGridLineVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXGridLineVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceXGridLineVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXGridLineVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            (axisX as AxisBase).GridLinesVisible = newValue;
        }

        public bool XGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(XGridLineVisibleProperty);
            }
            set
            {
                SetValue(XGridLineVisibleProperty, value);
            }
        }

        #endregion
        #region PrimaryAxisGridColor
        public static readonly DependencyProperty PrimaryAxisGridColorProperty = DependencyProperty.Register("PrimaryAxisGridColor", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXGridLineColorChanged), new CoerceValueCallback(OnCoerceXGridLineColor)));

        private static object OnCoerceXGridLineColor(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXGridLineColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceXGridLineColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXGridLineColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateControlLayout();
            }
        }

        public Brush PrimaryAxisGridColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(PrimaryAxisGridColorProperty);
            }
            set
            {
                SetValue(PrimaryAxisGridColorProperty, value);
            }
        }

        #endregion
        #region MajorXTicks
        public static readonly DependencyProperty MajorXTicksProperty = DependencyProperty.Register("MajorXTicks", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(10, new PropertyChangedCallback(OnXMajorCountChanged), new CoerceValueCallback(OnCoerceXMajorCount)));

        private static object OnCoerceXMajorCount(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXMajorCount((int)value);
            else
                return value;
        }

        private static void OnXMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXMajorCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceXMajorCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXMajorCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateXAxisRange();
            }
        }

        public int MajorXTicks
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorXTicksProperty);
            }
            set
            {
                SetValue(MajorXTicksProperty, value);
            }
        }

        #endregion
        #region XMinorGridLineVisible
        public static readonly DependencyProperty XMinorGridLineVisibleProperty = DependencyProperty.Register("XMinorGridLineVisible", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXMinorGridLineVisible)));

        private static object OnCoerceXMinorGridLineVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXMinorGridLineVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceXMinorGridLineVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXMinorGridLineVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            (axisX as AxisBase).GridLinesMinorVisible = newValue;
        }

        public bool XMinorGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(XMinorGridLineVisibleProperty);
            }
            set
            {
                SetValue(XMinorGridLineVisibleProperty, value);
            }
        }

        #endregion
        #region XMinorGridLineColor
        public static readonly DependencyProperty XMinorGridLineColorProperty = DependencyProperty.Register("XMinorGridLineColor", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceXMinorGridLineColor)));

        private static object OnCoerceXMinorGridLineColor(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXMinorGridLineColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceXMinorGridLineColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXMinorGridLineColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateControlLayout();
            }
        }

        public Brush XMinorGridLineColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(XMinorGridLineColorProperty);
            }
            set
            {
                SetValue(XMinorGridLineColorProperty, value);
            }
        }

        #endregion
        #region XSmallTicksPerInterval
        public static readonly DependencyProperty XSmallTicksPerIntervalProperty = DependencyProperty.Register("XSmallTicksPerInterval", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(5, new PropertyChangedCallback(OnXMinorCountChanged), new CoerceValueCallback(OnCoerceXMinorCount)));

        private static object OnCoerceXMinorCount(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceXMinorCount((int)value);
            else
                return value;
        }

        private static void OnXMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnXMinorCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceXMinorCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXMinorCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateXAxisRange();
            }
        }

        public int XSmallTicksPerInterval
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(XSmallTicksPerIntervalProperty);
            }
            set
            {
                SetValue(XSmallTicksPerIntervalProperty, value);
            }
        }

        #endregion

        #region YAutoGrid
        public static readonly DependencyProperty YAutoGridProperty = DependencyProperty.Register("YAutoGrid", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYAutoGridChanged), new CoerceValueCallback(OnCoerceYAutoGrid)));

        private static object OnCoerceYAutoGrid(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnYAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYAutoGridChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceYAutoGrid(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYAutoGridChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateYAxisRange();
            }
            OnPropertyVisiblityChanged("YAutoGrid");
        }

        public bool YAutoGrid
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(YAutoGridProperty);
            }
            set
            {
                SetValue(YAutoGridProperty, value);
            }
        }

        #endregion
        #region YGridLineVisible
        public static readonly DependencyProperty YGridLineVisibleProperty = DependencyProperty.Register("YGridLineVisible", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYGridLineVisible)));

        private static object OnCoerceYGridLineVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYGridLineVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceYGridLineVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYGridLineVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            (axisY as AxisBase).GridLinesVisible = newValue;
        }

        public bool YGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(YGridLineVisibleProperty);
            }
            set
            {
                SetValue(YGridLineVisibleProperty, value);
            }
        }

        #endregion
        #region SecAxisGridColor
        public static readonly DependencyProperty SecAxisGridColorProperty = DependencyProperty.Register("SecAxisGridColor", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYGridLineColorChanged), new CoerceValueCallback(OnCoerceYGridLineColor)));

        private static object OnCoerceYGridLineColor(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYGridLineColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceYGridLineColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYGridLineColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateControlLayout();
            }
        }

        public Brush SecAxisGridColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SecAxisGridColorProperty);
            }
            set
            {
                SetValue(SecAxisGridColorProperty, value);
            }
        }

        #endregion
        #region MajorYTicks
        public static readonly DependencyProperty MajorYTicksProperty = DependencyProperty.Register("MajorYTicks", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMajorCountChanged), new CoerceValueCallback(OnCoerceYMajorCount)));

        private static object OnCoerceYMajorCount(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYMajorCount((int)value);
            else
                return value;
        }

        private static void OnYMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYMajorCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceYMajorCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYMajorCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateYAxisRange();
            }
        }

        public int MajorYTicks
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorYTicksProperty);
            }
            set
            {
                SetValue(MajorYTicksProperty, value);
            }
        }

        #endregion
        #region YMinorGridLineVisible
        public static readonly DependencyProperty YMinorGridLineVisibleProperty = DependencyProperty.Register("YMinorGridLineVisible", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYMinorGridLineVisible)));

        private static object OnCoerceYMinorGridLineVisible(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYMinorGridLineVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceYMinorGridLineVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYMinorGridLineVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            (axisY as AxisBase).GridLinesMinorVisible = newValue;
        }

        public bool YMinorGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(YMinorGridLineVisibleProperty);
            }
            set
            {
                SetValue(YMinorGridLineVisibleProperty, value);
            }
        }

        #endregion
        #region GridColor
        public static readonly DependencyProperty GridColorProperty = DependencyProperty.Register("GridColor", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceYMinorGridLineColor)));

        private static object OnCoerceYMinorGridLineColor(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYMinorGridLineColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceYMinorGridLineColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYMinorGridLineColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateControlLayout();
            }
        }

        public Brush GridColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(GridColorProperty);
            }
            set
            {
                SetValue(GridColorProperty, value);
            }
        }

        #endregion
        #region YSmallTicksPerInterval
        public static readonly DependencyProperty YSmallTicksPerIntervalProperty = DependencyProperty.Register("YSmallTicksPerInterval", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMinorCountChanged), new CoerceValueCallback(OnCoerceYMinorCount)));

        private static object OnCoerceYMinorCount(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceYMinorCount((int)value);
            else
                return value;
        }

        private static void OnYMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnYMinorCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceYMinorCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYMinorCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateYAxisRange();
            }
        }

        public int YSmallTicksPerInterval
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(YSmallTicksPerIntervalProperty);
            }
            set
            {
                SetValue(YSmallTicksPerIntervalProperty, value);
            }
        }

        #endregion

        #region SampleNumber
        public static readonly DependencyProperty SampleNumberProperty = DependencyProperty.Register("SampleNumber", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata((int)3600, new PropertyChangedCallback(OnSampleNumberChanged), new CoerceValueCallback(OnCoerceSampleNumber)));

        private static object OnCoerceSampleNumber(DependencyObject o, object value)
        {
            RealTimeTrendControl realTimeSDataValue = o as RealTimeTrendControl;
            if (realTimeSDataValue != null)
                return realTimeSDataValue.OnCoerceSampleNumber((int)value);
            else
                return value;
        }

        private static void OnSampleNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl realTimeSDataValue = o as RealTimeTrendControl;
            if (realTimeSDataValue != null)
                realTimeSDataValue.OnSampleNumberChanged((int)e.OldValue, (int)e.NewValue);
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
        [Category("TrendOptions")]
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

        #region LoadMaxRecordFromDB
        public static readonly DependencyProperty LoadMaxRecordFromDBProperty = DependencyProperty.Register("LoadMaxRecordFromDB", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true));
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool LoadMaxRecordFromDB
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(LoadMaxRecordFromDBProperty);
            }
            set
            {
                SetValue(LoadMaxRecordFromDBProperty, value);
            }
        }
        #endregion


        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(30));

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
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

        #region MaxDeadLockRetry
        public static readonly DependencyProperty MaxDeadLockRetryProperty = DependencyProperty.Register("MaxDeadLockRetry", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(1, new PropertyChangedCallback(OnMaxDeadLockRetryChanged), new CoerceValueCallback(OnCoerceMaxDeadLockRetry)));

        private static object OnCoerceMaxDeadLockRetry(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceMaxDeadLockRetry((int)value);
            else
                return value;
        }

        private static void OnMaxDeadLockRetryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnMaxDeadLockRetryChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxDeadLockRetry(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxDeadLockRetryChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public int MaxDeadLockRetry
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxDeadLockRetryProperty);
            }
            set
            {
                SetValue(MaxDeadLockRetryProperty, value);
            }
        }

        #endregion


        #region MaxResolveOverlappingPoints
        public static readonly DependencyProperty MaxResolveOverlappingPointsProperty = DependencyProperty.Register("MaxResolveOverlappingPoints", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(360, new PropertyChangedCallback(OnMaxResolveOverlappingPointsChanged), new CoerceValueCallback(OnCoerceMaxResolveOverlappingPoints)));

        private static object OnCoerceMaxResolveOverlappingPoints(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceMaxResolveOverlappingPoints((int)value);
            else
                return value;
        }

        private static void OnMaxResolveOverlappingPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnMaxResolveOverlappingPointsChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxResolveOverlappingPoints(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxResolveOverlappingPointsChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int MaxResolveOverlappingPoints
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxResolveOverlappingPointsProperty);
            }
            set
            {
                SetValue(MaxResolveOverlappingPointsProperty, value);
            }
        }

        #endregion


        #region MaxLabelPoints
        public static readonly DependencyProperty MaxLabelPointsProperty = DependencyProperty.Register("MaxLabelPoints", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxLabelPointsChanged), new CoerceValueCallback(OnCoerceMaxLabelPoints)));

        private static object OnCoerceMaxLabelPoints(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceMaxLabelPoints((int)value);
            else
                return value;
        }

        private static void OnMaxLabelPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnMaxLabelPointsChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxLabelPoints(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxLabelPointsChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int MaxLabelPoints
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxLabelPointsProperty);
            }
            set
            {
                SetValue(MaxLabelPointsProperty, value);
            }
        }

        #endregion



        #region AutoHideToolbar
        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoHideToolbar((bool)value);
            else
                return value;
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnAutoHideToolbarChanged((bool)e.OldValue, (bool)e.NewValue);
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
                if (newValue)
                {
                    toolbar_MouseLeave(null, null);
                    Grid.SetRow(adorner, 0);
                    Grid.SetRowSpan(adorner, 2);
                }
                else
                {
                    toolbar_MouseEnter(null, null);
                    Grid.SetRow(adorner, 1);
                    Grid.SetRowSpan(adorner, 1);
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
        #region AutoCollapseHeight
        public static readonly DependencyProperty AutoCollapseHeightProperty = DependencyProperty.Register("AutoCollapseHeight", typeof(double), typeof(RealTimeTrendControl), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseHeightChanged), new CoerceValueCallback(OnCoerceAutoCollapseHeight)));

        private static object OnCoerceAutoCollapseHeight(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseHeight((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnAutoCollapseHeightChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceAutoCollapseHeight(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoCollapseHeightChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double AutoCollapseHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(AutoCollapseHeightProperty);
            }
            set
            {
                SetValue(AutoCollapseHeightProperty, value);
            }
        }
        #endregion
        #region AutoCollapseWidth
        public static readonly DependencyProperty AutoCollapseWidthProperty = DependencyProperty.Register("AutoCollapseWidth", typeof(double), typeof(RealTimeTrendControl), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseWidthChanged), new CoerceValueCallback(OnCoerceAutoCollapseWidth)));

        private static object OnCoerceAutoCollapseWidth(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseWidth((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnAutoCollapseWidthChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceAutoCollapseWidth(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoCollapseWidthChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double AutoCollapseWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(AutoCollapseWidthProperty);
            }
            set
            {
                SetValue(AutoCollapseWidthProperty, value);
            }
        }
        #endregion
        #region LogarithmicYScale
        public static readonly DependencyProperty LogarithmicYScaleProperty = DependencyProperty.Register("LogarithmicYScale", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLogarithmicYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicYScale)));

        private static object OnCoerceLogarithmicYScale(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicYScale((bool)value);
            else
                return value;
        }

        private static void OnLogarithmicYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnLogarithmicYScaleChanged((bool)e.OldValue, (bool)e.NewValue);
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
        #endregion
        #region LogarithmicBaseYScale
        public static readonly DependencyProperty LogarithmicBaseYScaleProperty = DependencyProperty.Register("LogarithmicBaseYScale", typeof(double), typeof(RealTimeTrendControl), new UIPropertyMetadata(10.0, new PropertyChangedCallback(OnLogarithmicBaseYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicBaseYScale)));

        private static object OnCoerceLogarithmicBaseYScale(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicBaseYScale((double)value);
            else
                return value;
        }

        private static void OnLogarithmicBaseYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnLogarithmicBaseYScaleChanged((double)e.OldValue, (double)e.NewValue);
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
        #region Rotated
        public static readonly DependencyProperty RotatedProperty = DependencyProperty.Register("Rotated", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRotatedChanged), new CoerceValueCallback(OnCoerceRotated)));

        private static object OnCoerceRotated(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceRotated((bool)value);
            else
                return value;
        }

        private static void OnRotatedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnRotatedChanged((bool)e.OldValue, (bool)e.NewValue);
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
        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnEditableChanged((bool)e.OldValue, (bool)e.NewValue);
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
            {
                //btnChartDesigner.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                btnExpand.IsVisible = newValue;
                if (!bDesignmode && legend_GridControl != null)
                {
                    if (!newValue)
                        legend_GridControl.View?.ColumnMenuCustomizations.Add(new RemoveBarItemAndLinkAction()
                        {
                            ItemName = DefaultColumnMenuItemNames.ColumnChooser
                        });
                    else
                        legend_GridControl.View?.ColumnMenuCustomizations.Clear();
                }

            }
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
        #region ShowToolbarShowHideButtons
        public static readonly DependencyProperty ShowToolbarShowHideButtonsProperty = DependencyProperty.Register("ShowToolbarShowHideButtons", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarShowHideButtonsChanged), new CoerceValueCallback(OnCoerceShowToolbarShowHideButtons)));

        private static object OnCoerceShowToolbarShowHideButtons(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceShowToolbarShowHideButtons((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarShowHideButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnShowToolbarShowHideButtonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarShowHideButtons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarShowHideButtonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowToolbarShowHideButtons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowToolbarShowHideButtonsProperty);
            }
            set
            {
                SetValue(ShowToolbarShowHideButtonsProperty, value);
            }
        }
        #endregion
        #region ShowToolbarAdvSettings
        public static readonly DependencyProperty ShowToolbarAdvSettingsProperty = DependencyProperty.Register("ShowToolbarAdvSettings", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarAdvSettingsChanged), new CoerceValueCallback(OnCoerceShowToolbarAdvSettings)));

        private static object OnCoerceShowToolbarAdvSettings(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceShowToolbarAdvSettings((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarAdvSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnShowToolbarAdvSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarAdvSettings(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarAdvSettingsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool ShowToolbarAdvSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowToolbarAdvSettingsProperty);
            }
            set
            {
                SetValue(ShowToolbarAdvSettingsProperty, value);
            }
        }
        #endregion
        #region ShowToolbarTimeControls
        public static readonly DependencyProperty ShowToolbarTimeControlsProperty = DependencyProperty.Register("ShowToolbarTimeControls", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarTimeControlsChanged), new CoerceValueCallback(OnCoerceShowToolbarTimeControls)));

        private static object OnCoerceShowToolbarTimeControls(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceShowToolbarTimeControls((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarTimeControlsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnShowToolbarTimeControlsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarTimeControls(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarTimeControlsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool ShowToolbarTimeControls
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowToolbarTimeControlsProperty);
            }
            set
            {
                SetValue(ShowToolbarTimeControlsProperty, value);
            }
        }
        #endregion
        #region ShowToolbarMaxRecords
        public static readonly DependencyProperty ShowToolbarMaxRecordsProperty = DependencyProperty.Register("ShowToolbarMaxRecords", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarMaxRecordsChanged), new CoerceValueCallback(OnCoerceShowToolbarMaxRecords)));

        private static object OnCoerceShowToolbarMaxRecords(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceShowToolbarMaxRecords((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarMaxRecordsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnShowToolbarMaxRecordsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarMaxRecords(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarMaxRecordsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool ShowToolbarMaxRecords
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowToolbarMaxRecordsProperty);
            }
            set
            {
                SetValue(ShowToolbarMaxRecordsProperty, value);
            }
        }
        #endregion
        #region ShowToolbarCompare
        public static readonly DependencyProperty ShowToolbarCompareProperty = DependencyProperty.Register("ShowToolbarCompare", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarCompareChanged), new CoerceValueCallback(OnCoerceShowToolbarCompare)));

        private static object OnCoerceShowToolbarCompare(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceShowToolbarCompare((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarCompareChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnShowToolbarCompareChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarCompare(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarCompareChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        public bool ShowToolbarCompare
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowToolbarCompareProperty);
            }
            set
            {
                SetValue(ShowToolbarCompareProperty, value);
            }
        }
        #endregion

        #region AlwaysExpanded
        public static readonly DependencyProperty AlwaysExpandedProperty = DependencyProperty.Register("AlwaysExpanded", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAlwaysExpandedChanged), new CoerceValueCallback(OnCoerceAlwaysExpanded)));

        private static object OnCoerceAlwaysExpanded(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAlwaysExpanded((bool)value);
            else
                return value;
        }

        private static void OnAlwaysExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnAlwaysExpandedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAlwaysExpanded(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlwaysExpandedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue)
                HideAllHidden();
            else
                RestoreAllHidden();
        }

        public bool AlwaysExpanded
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AlwaysExpandedProperty);
            }
            set
            {
                SetValue(AlwaysExpandedProperty, value);
            }
        }
        #endregion

        #region StaticSeriesSettings
        public static readonly DependencyProperty PenListProperty = DependencyProperty.Register("PenList", typeof(PenItemList), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnStaticSeriesSettingsChanged), new CoerceValueCallback(OnCoerceStaticSeriesSettings)));

        private static object OnCoerceStaticSeriesSettings(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceStaticSeriesSettings((PenItemList)value);
            else
                return value;
        }

        private static void OnStaticSeriesSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnStaticSeriesSettingsChanged((PenItemList)e.OldValue, (PenItemList)e.NewValue);
        }

        protected virtual PenItemList OnCoerceStaticSeriesSettings(PenItemList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStaticSeriesSettingsChanged(PenItemList oldValue, PenItemList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit)
            {
                InitChart();
            }
        }

        [SvgValueConverter(typeof(ConvertSerieDataList))]
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
        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(RealTimeTrendControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
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
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                gridControl.SaveLayoutToStream(output);
                resetGridLayout = utf8noBOM.GetString(output.ToArray());
            }
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

        void LoadDesignGridLayout()
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


        [Browsable(false)]
        [SvgValueConverter(false)]
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

        #endregion
        #region DockLayout
        public static readonly DependencyProperty DockLayoutProperty = DependencyProperty.Register("DockLayout", typeof(String), typeof(RealTimeTrendControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDockLayoutChanged), new CoerceValueCallback(OnCoerceDockLayout)));

        private static object OnCoerceDockLayout(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDockLayout((String)value);
            else
                return value;
        }

        private static void OnDockLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnDockLayoutChanged((String)e.OldValue, (String)e.NewValue);
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
            if (dockManager == null)
                return;
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                dockManager.SaveLayoutToStream(output);
                resetDockLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void SaveDesignDockLayout()
        {
            if (dockManager == null)
                return;
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                dockManager.SaveLayoutToStream(output);
                DockLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        void LoadDesignDockLayout()
        {
            if (dockManager == null || ((bDesignmode || DesignerProperties.GetIsInDesignMode(this)) && !bLoaded))
                return;
            if (string.IsNullOrEmpty(DockLayout))
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
                Encoding utf8noBOM = new UTF8Encoding(true);
                if (!string.IsNullOrEmpty(DockLayout))
                    using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(DockLayout)))
                    {
                        try
                        {
                            dockManager.RestoreLayoutFromStream(output);
                            List<LayoutPanel> autoHiddenPanels = new List<LayoutPanel>();
                            foreach (var group in dockManager.AutoHideGroups)
                            {
                                foreach (LayoutPanel panel in GetChildPanels(group))
                                    if (panel.AutoHidden)
                                        autoHiddenPanels.Add(panel);
                            }
                            foreach (var panel in autoHiddenPanels)
                            {
                                dockManager.BeginUpdate();
                                panel.AutoHidden = false;
                                panel.AutoHidden = true;
                                dockManager.EndUpdate();
                            }
                        }
                        catch (Exception ex)
                        {
                            DockLayout = string.Empty;
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


        #region TrendOrientation
        public static readonly DependencyProperty TrendOrientationProperty = DependencyProperty.Register("TrendOrientation", typeof(TrendMode), typeof(RealTimeTrendControl), new UIPropertyMetadata(TrendMode.Horizontal));

        public TrendMode TrendOrientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TrendMode)GetValue(TrendOrientationProperty);
            }
            set
            {
                SetValue(TrendOrientationProperty, value);
            }
        }

        #endregion

        #region ListViewLayout
        public static readonly DependencyProperty ListViewLayoutProperty = DependencyProperty.Register("ListViewLayout", typeof(String), typeof(RealTimeTrendControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnListViewLayoutChanged), new CoerceValueCallback(OnCoerceListViewLayout)));

        private static object OnCoerceListViewLayout(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceListViewLayout((String)value);
            else
                return value;
        }

        private static void OnListViewLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnListViewLayoutChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceListViewLayout(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnListViewLayoutChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            LoadDesignListViewLayout();
        }

        internal void ResetLegendLayout()
        {
            if (!string.IsNullOrEmpty(resetListLayout))
                ListViewLayout = resetListLayout;
        }

        void SaveResetLegendLayout()
        {
            if (legend_GridControl == null)
                return;
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                legend_GridControl.SaveLayoutToStream(output);
                resetListLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void SaveDesignListViewLayout()
        {
            if (legend_GridControl == null)
                return;
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                legend_GridControl.SaveLayoutToStream(output);
                ListViewLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        void LoadDesignListViewLayout()
        {
            if (legend_GridControl == null || string.IsNullOrEmpty(ListViewLayout))
                return;

            if (string.IsNullOrEmpty(resetListLayout))
                SaveResetLegendLayout();

            var dim = ListViewLayout.Length;
            string _mid = string.Empty;

            if (ListViewLayout.IndexOf('?') == 0)
            {
                _mid = ListViewLayout.Substring(1, dim - 1);
                SetValue(ListViewLayoutProperty, _mid);
                return;
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(ListViewLayout)))
                {
                    try
                    {
                        legend_GridControl.RestoreLayoutFromStream(output);
                    }
                    catch (Exception ex)
                    {
                        ListViewLayout = string.Empty;
                    }
                }
            }
        }

        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertGridLayout))]
        public String ListViewLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ListViewLayoutProperty);
            }
            set
            {
                SetValue(ListViewLayoutProperty, value);
            }
        }

        #endregion

        #region Minimum
        //private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(TrendRealTime), new UIPropertyMetadata((double)0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        //private static object OnCoerceMinimum(DependencyObject o, object value)
        //{
        //    TrendRealTime control = o as TrendRealTime;
        //    if (control != null)
        //        return control.OnCoerceMinimum((double)value);
        //    else
        //        return value;
        //}

        //private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    TrendRealTime control = o as TrendRealTime;
        //    if (control != null)
        //        control.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        //}

        //protected virtual double OnCoerceMinimum(double value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnMinimumChanged(double oldValue, double newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("Advanced")]
        //public double Minimum
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (double)GetValue(MinimumProperty);
        //    }
        //    set
        //    {
        //        SetValue(MinimumProperty, value);
        //    }
        //}
        #endregion

        #region Maximum
        //private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(TrendRealTime), new UIPropertyMetadata((double)100, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        //private static object OnCoerceMaximum(DependencyObject o, object value)
        //{
        //    TrendRealTime control = o as TrendRealTime;
        //    if (control != null)
        //        return control.OnCoerceMaximum((double)value);
        //    else
        //        return value;
        //}

        //private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    TrendRealTime control = o as TrendRealTime;
        //    if (control != null)
        //        control.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        //}

        //protected virtual double OnCoerceMaximum(double value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnMaximumChanged(double oldValue, double newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("Advanced")]
        //public double Maximum
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (double)GetValue(MaximumProperty);
        //    }
        //    set
        //    {
        //        SetValue(MaximumProperty, value);
        //    }
        //}

        #endregion


        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
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
        #region StartDateTitle
        public static readonly DependencyProperty StartDateTitleProperty = DependencyProperty.Register("StartDateTitle", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata(Properties.Resources.StartDate));
        [Browsable(false)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StartDateTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(StartDateTitleProperty);
            }
            set
            {
                SetValue(StartDateTitleProperty, value);
            }
        }
        #endregion

        #region EndDateTitle
        public static readonly DependencyProperty EndDateTitleProperty = DependencyProperty.Register("EndDateTitle", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata(Properties.Resources.EndDate));
        [Browsable(false)]
        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string EndDateTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(EndDateTitleProperty);
            }
            set
            {
                SetValue(EndDateTitleProperty, value);
            }
        }
        #endregion

        #region AllValuesXMargin
        public static readonly DependencyProperty AllValuesXMarginProperty = DependencyProperty.Register("AllValuesXMargin", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(5, new PropertyChangedCallback(OnAllValuesXMarginChanged), new CoerceValueCallback(OnCoerceAllValuesXMargin)));

        private static object OnCoerceAllValuesXMargin(DependencyObject o, object value)
        {
            RealTimeTrendControl DataAnalisys = o as RealTimeTrendControl;
            if (DataAnalisys != null)
                return DataAnalisys.OnCoerceAllValuesXMargin((int)value);
            else
                return value;
        }

        private static void OnAllValuesXMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl DataAnalisys = o as RealTimeTrendControl;
            if (DataAnalisys != null)
                DataAnalisys.OnAllValuesXMarginChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceAllValuesXMargin(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllValuesXMarginChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int AllValuesXMargin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(AllValuesXMarginProperty);
            }
            set
            {
                SetValue(AllValuesXMarginProperty, value);
            }
        }
        #endregion

        #region ScalePaddingFactor
        public static readonly DependencyProperty ScalePaddingFactorProperty = DependencyProperty.Register("ScalePaddingFactor", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(20, new PropertyChangedCallback(OnScalePaddingFactorChanged), new CoerceValueCallback(OnCoerceScalePaddingFactor)));

        private static object OnCoerceScalePaddingFactor(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceScalePaddingFactor((int)value);
            else
                return value;
        }

        private static void OnScalePaddingFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnScalePaddingFactorChanged((int)e.OldValue, (int)e.NewValue);
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
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false));

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

        bool runningOnServer;
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

        [Browsable(false)]
        public string GeneralError
        {
            get
            {
                if (dlException != null)
                    return String.Format(Properties.Resources.ErrorDrawingValues, dlException.Message);
                else
                    return string.Empty;
            }
        }


        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(RealTimeTrendControl));
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(false)]
        public CultureInfo CurrentCulture
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (CultureInfo)GetValue(CurrentCultureProperty);
            }
            set
            {
                SetValue(CurrentCultureProperty, value);
            }
        }

        #endregion

        #region DateTimeFormat
        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register("DateTimeFormat", typeof(String), typeof(RealTimeTrendControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDateTimeFormatChanged), new CoerceValueCallback(OnCoerceDateTimeFormat)));

        private static object OnCoerceDateTimeFormat(DependencyObject o, object value)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDateTimeFormat((String)value);
            else
                return value;
        }

        private static void OnDateTimeFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl dataAnalisys = o as RealTimeTrendControl;
            if (dataAnalisys != null)
                dataAnalisys.OnDateTimeFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceDateTimeFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDateTimeFormatChanged(String oldValue, String newValue)
        {
            if (!bLoaded || bDispose)
                return;

            ApplyDateTimeFormat();
        }

        public String DateTimeFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DateTimeFormatProperty);
            }
            set
            {
                SetValue(DateTimeFormatProperty, value);
            }
        }
        #endregion

        #region IsInStop
        public static readonly DependencyProperty IsInStopProperty = DependencyProperty.Register("IsInStop", typeof(Boolean), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsInStopChanged), new CoerceValueCallback(OnCoerceIsInStop)));

        private static object OnCoerceIsInStop(DependencyObject o, object value)
        {
            RealTimeTrendControl da = o as RealTimeTrendControl;
            if (da != null)
                return da.OnCoerceIsInStop((Boolean)value);
            else
                return value;
        }

        private static void OnIsInStopChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl da = o as RealTimeTrendControl;
            if (da != null)
                da.OnIsInStopChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceIsInStop(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsInStopChanged(Boolean oldValue, Boolean newValue)
        {
            if (timer == null)
                return;
            if (oldValue != newValue)
            {
                if (zommingRangeList != null)
                    zommingRangeList.Clear();

                if (newValue)
                {
                    if (!bChangingPlayPause)
                        PauseExecute();
                    if (!bIsRestarting)
                        EnableToolbars(true);
                    foreach (var serie in diagram.Series.OfType<Series>())
                        serie.LabelsVisibility = true;
                    //((XYDiagram2D)chart.Diagram).EnableAxisXNavigation = true; //enables x-axis scroll
                }
                else
                {
                    if (!bChangingPlayPause)
                        PlayExecute();
                    EnableToolbars(false);
                    foreach (var serie in diagram.Series.OfType<Series>())
                        serie.LabelsVisibility = false;
                    //((XYDiagram2D)chart.Diagram).EnableAxisXNavigation = false; //disables x-axis scroll
                }
            }
        }
        
        void EnableToolbars(bool bEnable)
        {
            toolbarSettings.IsEnabled = toolbarShowHideButtons.IsEnabled = bEnable;
        }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsInStop
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(IsInStopProperty);
            }
            set
            {
                SetValue(IsInStopProperty, value);
            }
        }
        #endregion
        #region RuntimeMode
        public static readonly DependencyProperty RuntimeModeProperty = DependencyProperty.Register("RuntimeMode", typeof(RunMode), typeof(RealTimeTrendControl), new UIPropertyMetadata(RunMode.RunStop, new PropertyChangedCallback(OnRuntimeModeChanged), new CoerceValueCallback(OnCoerceRuntimeMode)));

        private static object OnCoerceRuntimeMode(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceRuntimeMode((RunMode)value);
            else
                return value;
        }

        private static void OnRuntimeModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnRuntimeModeChanged((RunMode)e.OldValue, (RunMode)e.NewValue);
        }

        protected virtual RunMode OnCoerceRuntimeMode(RunMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRuntimeModeChanged(RunMode oldValue, RunMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bInit)
                ReloadPens();
        }

        public RunMode RuntimeMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (RunMode)GetValue(RuntimeModeProperty);
            }
            set
            {
                SetValue(RuntimeModeProperty, value);
            }
        }
        #endregion
        #region LinkedPenName
        public static readonly DependencyProperty LinkedPenNameProperty = DependencyProperty.Register("LinkedPenName", typeof(string), typeof(RealTimeTrendControl), new UIPropertyMetadata("Pen", new PropertyChangedCallback(OnLinkedPenNameChanged), new CoerceValueCallback(OnCoerceLinkedPenName)));

        private static object OnCoerceLinkedPenName(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceLinkedPenName((string)value);
            else
                return value;
        }

        private static void OnLinkedPenNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
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
        #region RecordEvery
        public static readonly DependencyProperty RecordEveryProperty = DependencyProperty.Register("RecordEvery", typeof(TimeSpan), typeof(RealTimeTrendControl), new UIPropertyMetadata(new TimeSpan(0, 0, 0, 0, 250), new PropertyChangedCallback(OnRecordEveryChanged), new CoerceValueCallback(OnCoerceRecordEvery)));

        private static object OnCoerceRecordEvery(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceRecordEvery((TimeSpan)value);
            else
                return value;
        }

        private static void OnRecordEveryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnRecordEveryChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceRecordEvery(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRecordEveryChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public TimeSpan RecordEvery
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(RecordEveryProperty);
            }
            set
            {
                SetValue(RecordEveryProperty, value);
            }
        }
        #endregion
        #region ViewTimeFrame
        public static readonly DependencyProperty ViewTimeFrameProperty = DependencyProperty.Register("ViewTimeFrame", typeof(TimeSpan), typeof(RealTimeTrendControl), new UIPropertyMetadata(new TimeSpan(0, 0, 1, 0), new PropertyChangedCallback(OnViewTimeFrameChanged), new CoerceValueCallback(OnCoerceViewTimeFrame)));

        private static object OnCoerceViewTimeFrame(DependencyObject o, object value)
        {
            RealTimeTrendControl RealTimeSDataValue = o as RealTimeTrendControl;
            if (RealTimeSDataValue != null)
                return RealTimeSDataValue.OnCoerceViewTimeFrame((TimeSpan)value);
            else
                return value;
        }

        private static void OnViewTimeFrameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl RealTimeSDataValue = o as RealTimeTrendControl;
            if (RealTimeSDataValue != null)
                RealTimeSDataValue.OnViewTimeFrameChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceViewTimeFrame(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnViewTimeFrameChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Advanced")]
        public TimeSpan ViewTimeFrame
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(ViewTimeFrameProperty);
            }
            set
            {
                SetValue(ViewTimeFrameProperty, value);
            }
        }
        #endregion
        #region RecordOnlyOnQualityGood
        public static readonly DependencyProperty RecordOnlyOnQualityGoodProperty = DependencyProperty.Register("RecordOnlyOnQualityGood", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRecordOnlyOnQualityGoodChanged), new CoerceValueCallback(OnCoerceRecordOnlyOnQualityGood)));

        private static object OnCoerceRecordOnlyOnQualityGood(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceRecordOnlyOnQualityGood((bool)value);
            else
                return value;
        }

        private static void OnRecordOnlyOnQualityGoodChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnRecordOnlyOnQualityGoodChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceRecordOnlyOnQualityGood(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRecordOnlyOnQualityGoodChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            recordOnlyOnQualityGood = newValue;
        }
        [Category("Advanced")]
        public bool RecordOnlyOnQualityGood
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(RecordOnlyOnQualityGoodProperty);
            }
            set
            {
                SetValue(RecordOnlyOnQualityGoodProperty, value);
            }
        }

        #endregion
        #region CurrentValueLabelForeground
        public static readonly DependencyProperty CurrentValueLabelForegroundProperty = DependencyProperty.Register("CurrentValueLabelForeground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), new PropertyChangedCallback(OnCurrentValueLabelForegroundChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelForeground)));

        private static object OnCoerceCurrentValueLabelForeground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceCurrentValueLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnCurrentValueLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceCurrentValueLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCurrentValueLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("DataAnalisysOptions")]
        public Brush CurrentValueLabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(CurrentValueLabelForegroundProperty);
            }
            set
            {
                SetValue(CurrentValueLabelForegroundProperty, value);
            }
        }

        #endregion
        #region CurrentValueLabelBackground
        public static readonly DependencyProperty CurrentValueLabelBackgroundProperty = DependencyProperty.Register("CurrentValueLabelBackground", typeof(Brush), typeof(RealTimeTrendControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(215, 106, 0)), new PropertyChangedCallback(OnCurrentValueLabelBackgroundChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelBackground)));

        private static object OnCoerceCurrentValueLabelBackground(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceCurrentValueLabelBackground((Brush)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnCurrentValueLabelBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceCurrentValueLabelBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCurrentValueLabelBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("DataAnalisysOptions")]
        public Brush CurrentValueLabelBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(CurrentValueLabelBackgroundProperty);
            }
            set
            {
                SetValue(CurrentValueLabelBackgroundProperty, value);
            }
        }

        #endregion
        #region AutomaticGeneralScale
        public static readonly DependencyProperty AutomaticGeneralScaleProperty = DependencyProperty.Register("AutomaticGeneralScale", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutomaticGeneralScaleChanged), new CoerceValueCallback(OnCoerceAutomaticGeneralScale)));

        private static object OnCoerceAutomaticGeneralScale(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceAutomaticGeneralScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticGeneralScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnAutomaticGeneralScaleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutomaticGeneralScale(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutomaticGeneralScaleChanged(bool oldValue, bool newValue)
        {
            if (bInit)
            {
                UpdateYAxisRange();
            }
        }

        [Category("DataAnalisysOptions")]
        public bool AutomaticGeneralScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutomaticGeneralScaleProperty);
            }
            set
            {
                SetValue(AutomaticGeneralScaleProperty, value);
            }
        }
        #endregion
        
        #region CurrentValueLabelWidth
        public static readonly DependencyProperty CurrentValueLabelWidthProperty = DependencyProperty.Register("CurrentValueLabelWidth", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(40, new PropertyChangedCallback(OnCurrentValueLabelWidthChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelWidth)));

        private static object OnCoerceCurrentValueLabelWidth(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceCurrentValueLabelWidth((int)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnCurrentValueLabelWidthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceCurrentValueLabelWidth(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCurrentValueLabelWidthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("DataAnalisysOptions")]
        public int CurrentValueLabelWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(CurrentValueLabelWidthProperty);
            }
            set
            {
                SetValue(CurrentValueLabelWidthProperty, value);
            }
        }
        #endregion
        
        #region SelectedPenThickness
        public static readonly DependencyProperty SelectedPenThicknessProperty = DependencyProperty.Register("SelectedPenThickness", typeof(int), typeof(RealTimeTrendControl), new UIPropertyMetadata(5, new PropertyChangedCallback(OnSelectedPenThicknessChanged), new CoerceValueCallback(OnCoerceSelectedPenThickness)));

        private static object OnCoerceSelectedPenThickness(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceSelectedPenThickness((int)value);
            else
                return value;
        }

        private static void OnSelectedPenThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnSelectedPenThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSelectedPenThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectedPenThicknessChanged(int oldValue, int newValue)
        {
            if (bDesignmode || bDispose || newValue == oldValue)
                return;

            foreach(var settings in mapKeySeries.Values)
            {
                settings.SelectedPenThickness = newValue;
            }
        }

        public int SelectedPenThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SelectedPenThicknessProperty);
            }
            set
            {
                SetValue(SelectedPenThicknessProperty, value);
            }
        }
        #endregion
        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                return TrendRealTime.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl TrendRealTime = o as RealTimeTrendControl;
            if (TrendRealTime != null)
                TrendRealTime.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }

        public void RealTimeTrendControlInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region Demo Values
        List<MyDataValue> LoadData(Random r, int days)
        {
            List<MyDataValue> demoValues = new List<MyDataValue>();

            for (int i = 0; i < days; i++)
            {
                double? val = r != null ? r.NextDouble() * Properties.Settings.Default.DemoMaxValue : 0;
                demoValues.Add(new MyDataValue
                {
                    SourceTimestamp = DateTime.Now.AddDays(i),
                    dValue = val
                });
            }
            if (settingStorage == null)
            {
                settingStorage = new SettingsStorage();
                settingStorage.StartTime = settingStorage.DateTimeStart = demoValues.First().SourceTimestamp;
                settingStorage.EndTime = settingStorage.DateTimeEnd = demoValues.Last().SourceTimestamp;
            }

            return demoValues;
        }
        #endregion

        #region Declarations
        bool forceUpdateOnMatchTypeDefinition;
        List<string> matchChangedMap = new List<string>();
        internal PenItemList designPenList;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        bool bLoaded;
        bool isTemplateApplied;
        bool bDesignmode;
        List<String> listSeries;// = new List<String>();
        SettingsStorage settingStorage;// = new SettingsStorage();
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.TrendControlLog);
        internal IDocument Document;// = null;
        IStringEditorManager stringManager;
        IUFUAEditorManager UFUAEditor;
        IUFProjectManager iUFProjectManager;
        IUnitConverterEditorManager ConverterEditorManager;
        IDictionary<String, String> stringlist;

        IDictionary<String, String> MapToListVariablesNodeId;// = new Dictionary<String, String>();
        DateTime maxDateTimeValue = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
        DateTime minDateTimeValue = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        string designGridLayout;
        string designDockLayout;
        string designListLayout;
        string resetGridLayout;
        string resetDockLayout;
        string resetListLayout;
        TypeHelper typeHelper = new TypeHelper();
        Dictionary<string, OPCUAEntityReference> opcuaEntityReference;

        const int secondsPerMinute = 60;
        const int minutesPerHour = 60;
        const int hoursPerDay = 24;
        const int daysPerWeek = 7;
        const int daysPerMonth = 30;
        const int daysPerYears = 365;

        Dictionary<string, SeriesPoint> nearestPoints = new Dictionary<string, SeriesPoint>();
        CurrentValues currentValues = new CurrentValues();
        int? legendSelectedRowHandle = 0;
        bool bFirstPlay = true;
        bool bChangingPlayPause = false;
        bool bIsFetching = false;
        SerieSettings nearestSerie;
        Dictionary<Series, int> seriesHistoryDataCount = new Dictionary<Series, int>();
        List<AxisY2D> axisToUpdateGridSpacing = new List<AxisY2D>();
        List<AxisY2D> axisToUpdateSideMargins = new List<AxisY2D>();

        private Stream TemplateStream
        {
            get
            {
                return ExtractFileFromResource.Extract(Assembly.GetExecutingAssembly(), string.Format("{0}.Resources.{1}", typeof(RealTimeTrendControl).Namespace, "DATemplates.xaml"));
            }
        }

        internal Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    if (PenList != null)
                        for (int i = 0; i < PenList.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(PenList[i].guiId))
                                PenList[i].guiId = Guid.NewGuid().ToString();
                            opcuaEntityReference[PenList[i].guiId] = PenList[i].TagReference;
                        }
                }
                return opcuaEntityReference;
            }
        }

        TimeSpan clientTimezoneOffset;
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

        Helper helper;
        #region ActualConfig
        string actualConfig = Properties.Settings.Default.DesignSettingName;
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
                actualConfig = value;
            }
        }
        MemorySettings MemorySettingList;
        #endregion
        DateSpan designFilterType;
        bool designUseAbsoluteRanges;
        bool bIsRestarting;
        bool bDiscreteTimeMode;
        #region realTime Declarations
        Dictionary<string, TrendDataGenerator> viewList;
        TrendDataGenerator viewDataContext;
        Dictionary<string, SerieSettings> mapKeySeries;
        DispatcherTimer timer;
        //int pointsToAddOnTick = 1;
        //int timeFrameMinutes = 1;
        //double totalSeriesMin = double.MinValue;
        //double totalSeriesMax = double.MaxValue;
        DispatcherOperation dpUpdateValue;
        bool recordOnlyOnQualityGood;
        DateTime? lastPrintedLivePointDate = null;
        DateTime minValueDate;
        #endregion
        #endregion
        #region Constructor
        private DelayedSingleActionInvoker SizeChangedInvoker;
        internal bool bSmartSettingsEditing;
        bool bInit; 
        MonitoredItemViewModel monitoredItemViewModel;
        string monitoredKey;

        public RealTimeTrendControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbar.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
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
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                    {
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                        ConverterEditorManager = Document.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                    }

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);

                    configMemory.EditValue = Properties.Settings.Default.DesignSettingName;
                    configMemory.DataContext = MemorySettingList?.Names;

                    if (SizeChangedInvoker == null)
                        SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                        {
                            if (!bDispose && !AlwaysExpanded)
                            {
                                if (ActualHeight < AutoCollapseHeight || ActualWidth < AutoCollapseWidth)
                                    HideAllHidden();
                                else
                                    RestoreAllHidden();
                            }
                        });

                    SizeChanged += OnSizeChanged;

                    if (AlwaysExpanded)
                        HideAllHidden();
                    UpdateControlLayout();

                    CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;

                    bDesignmode = DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;


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
                    DefToolbarHeight = toolbar.ActualHeight;
                    
                    SetAxisFontSettings(XAxsisFontSettings, axisX);
                    SetAxisFontSettings(YAxsisFontSettings, axisY);

                    if (bDesignmode)
                    {
                        DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                        sfchart.CrosshairOptions.ShowArgumentLine = false;
                        sfchart.CrosshairOptions.ShowValueLine = false;
                        sfchart.CrosshairOptions.ShowArgumentLabels = false;
                        sfchart.CrosshairOptions.ShowValueLabels = false;
                        sfchart.CrosshairOptions.ShowCrosshairLabels = false;
                        diagram.EnableAxisXNavigation = false;
                        diagram.EnableAxisYNavigation = false;
                        InitChart();
                        OverrideBaseProperties();
                        if (AutoHideToolbar)
                        {
                            toolbar_MouseLeave(null, null);
                            Grid.SetRow(adorner, 0);
                            Grid.SetRowSpan(adorner, 2);
                        }

                        toolbar.IsEnabled = false;

                        NestMainGridInsideDockManager();
                        LoadDesignDockLayout();
                        LoadDesignGridLayout();
                        LoadDesignListViewLayout();

                        mainChartGrid.IsEnabled = false;
                        printGrid.IsEnabled = false;
                        legend_GridControl.IsEnabled = false;

                        if (bSmartSettingsEditing)
                        {
                            printGrid.IsEnabled = true;
                            legend_GridControl.IsEnabled = true;
                        }
                        else
                            grid.IsHitTestVisible = false;

                        bInit = true;
                    }
                    else
                    {
                        diagram.EnableAxisXNavigation = IsInStop;
                        diagram.EnableAxisYNavigation = false;
                        diagram.NavigationOptions = new NavigationOptions() { AxisXMaxZoomPercent = double.MaxValue };

                        ScreenDocument.SetDisableShowMenuOnLef(this, true);

                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();
                        if (string.IsNullOrEmpty(ListViewLayout))
                            SaveDesignListViewLayout();
                        if (string.IsNullOrEmpty(DockLayout))
                            SaveDesignDockLayout();

                        InitRuntimeConfig();

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            LoadRuntimeLayout(GetStorageName(true));
                            GetItem(ActualConfig);
                        }

                        if (settingStorage == null)
                            settingStorage = new SettingsStorage();
                        if (settingStorage.ListRanges == null)
                            settingStorage.ListRanges = new List<TimeRange>();

                        if (listSeries == null)
                            listSeries = new List<String>();

                        ((XYDiagram2D)sfchart.Diagram).Scroll += AutoStopPlayOnScroll;

                        if (AutomaticGeneralScale)
                            axisY.Visible = true;

                        SetTimeRange(ViewTimeFrame);

                        if (RunningOnServer)
                        {
                            toolbarSettings.IsVisible = false;
                            toolbarPrintSettings.Visibility = Visibility.Collapsed;
                            AutoHideToolbar = false;

                            HideAllHidden();
                            RestoreChartFromSettings(false, true);
                        }
                        else
                        {
                            NestMainGridInsideDockManager();
                            RestoreChartFromSettings(false, true);
                        }

                        OverrideBaseProperties();

                        if (AutoHideToolbar)
                        {
                            toolbar_MouseLeave(null, null);
                            Grid.SetRow(adorner, 0);
                            Grid.SetRowSpan(adorner, 2);
                            toolbar.MouseEnter += toolbar_MouseEnter;
                            toolbar.MouseLeave += toolbar_MouseLeave;
                        }

                        bInit = true;

                        if (!bControlLoaded)
                        {
                            bControlLoaded = true;
                            OnControlLoaded();
                        }
                    }
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDispose)
                    return;

                if (monitoredItemViewModel != null)
                {
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                }
                monitoredItemViewModel = DataContext as MonitoredItemViewModel;

                if (monitoredItemViewModel != null)
                {
                    AddMonitoredSerie();
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                    if (monitoredItemViewModel.DataValue != null)
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                    if (monitoredItemViewModel.NodeIdModel != null)
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("NodeIdViewModel"));
                }
            };
        }

        private void AddMonitoredSerie()
        {
            if (dataContextSerie == null)
                return;

            sfchart.BeginInit();

            if (string.IsNullOrEmpty(monitoredKey))
                monitoredKey = Guid.NewGuid().ToString();

            if (viewDataContext == null)
            {
                var settings = new DataGeneratorSettings()
                {
                    ClientTimezoneOffset = ClientTimezoneOffset,
                    HDataCount = SampleNumber,
                    DeadBandInterval = RecordEvery,
                    DeadBandTimeFrame = ViewTimeFrame,
                    ArrayIndex = -1
                };
                var penUnitConverter = linkedPenSettings != null ? linkedPenSettings.penUnitConverter : null;
                if (linkedPenSettings != null)
                {
                    linkedPenSettings.expConverter = linkedPenSettings.expConverter != null ?
                        linkedPenSettings.expConverter :
                        GetExpressionValueConverter(penUnitConverter, linkedPenSettings.DName);
                }
                viewDataContext = new TrendDataGenerator(monitoredKey, settings, RecordEvery, linkedPenSettings?.expConverter);
                viewDataContext.Error += ViewList_OnError;
            }

            dataContextSerie.BeginInit();
            dataContextSerie.DisplayName = "DataContextSerie";
            dataContextSerie.ArgumentScaleType = ScaleType.DateTime;
            dataContextSerie.ArgumentDataMember = "SourceTimestamp";
            dataContextSerie.ValueDataMember = "dValueConverted";
            dataContextSerie.ValueScaleType = ScaleType.Numerical;
            dataContextSerie.Label.TextPattern = "{V:F3}";
            if (dataContextSerie is XYSeries2D)
            {
                var precision = linkedPenSettings != null ? linkedPenSettings.pointPrecision : PointPrecision;
                var pattern = String.Format("{{S}}\n{{V:F{0}}}\n{{A:{1}}}", precision, GetDateTimeFormat());
                (dataContextSerie as XYSeries2D).CrosshairLabelPattern = pattern;
            }

            dataContextSerie.DataSource = null;
            if(!IsInStop)
                dataContextSerie.DataSource = viewDataContext.collectionValues;
            else
                dataContextSerie.DataSource = viewDataContext.HistoryData;

            if (linkedPenSettings == null)
                linkedPenSettings = new SerieSettings()
                {
                    TagName = "DataContextSerie",
                    DName = GetLinkedPenName(),
                    NodeID = monitoredKey,
                    SGuid = monitoredKey,
                    ShowAxis = true,
                    AuthomaticScale = false,
                    IsVisible = true,
                    Name = "DataContextSerie",
                    Color = dataContextSerie.Brush.Color,
                    thickness = 1,
                    serieTypeLine = "seriesAreaStyle",
                    AbsoluteMax = 100,
                    AbsoluteMin = 0,
                    UseEUMinMax = true,
                    arrayindex = 0,
                    AddVirtualPoints = false,
                    SelectedPenThickness = SelectedPenThickness,
                    lineSerie = dataContextSerie,
                    pointPrecision = -1
                };

            AddSerieYScale("DataContextSerie", dataContextSerie.Brush.Color, monitoredKey, dataContextSerie, true, true);
            dataContextSerie.Visible = true;
            dataContextSerie.EndInit();

            sfchart.EndInit();

            SetLegendSource();
            OnMinMaxRangeChanged();
        }
        CancellationTokenSource cts;
        void OnMinMaxRangeChanged()
        {
            Action action = () =>
            {
                if (monitoredItemViewModel.HasRange && monitoredItemViewModel.Range != null && linkedPenSettings != null)
                {
                    linkedPenSettings.AbsoluteMin = monitoredItemViewModel.Range.Low;
                    linkedPenSettings.AbsoluteMax = monitoredItemViewModel.Range.High;
                    UpdateYAxisRange();
                }
            };

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

                action();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void InitChart()
        {
            diagram.Series.Clear();
            ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.Clear();
            Random r = new Random(DateTime.Now.Millisecond);
            var demoData = LoadData(r, Properties.Settings.Default.DemoMaxDays);
            if (PenList == null || PenList.Count == 0)
                AddChartLine("Value", "Value", "lineSeries", 1, Color.FromArgb(255, 65, 90, 120), false, null, demoData, isvisible: false);
            else
                for (var i = 0; i < PenList.Count; i++)
                {
                    var name = PenList[i].Name;
                    if (!String.IsNullOrEmpty(PenList[i].title))
                        name = PenList[i].title;
                    if(string.IsNullOrEmpty(name))
                        name = String.Format("Value{0}", i);

                    AddChartLine(name, name, PenList[i].PenStyle.ToString(), PenList[i].StrokeThickness, PenList[i].LColor, PenList[i].ShowAxis, null, demoData, isvisible: PenList[i].Visible, pointPrecision: PenList[i].PointPrecision);
                    if (i < PenList.Count - 1)
                        demoData = LoadData(r, Properties.Settings.Default.DemoMaxDays);
                }
            UpdateAxisRange();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!isTemplateApplied && RunningOnServer)
                SetWebAsset();
            isTemplateApplied = true;
        }

        #region RealTime Methods


        void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;

            if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null && (!recordOnlyOnQualityGood || StatusCode.IsGood(m.DataValue.StatusCode)))
                    UpdateMonitoredValue(monitoredKey, m.DataValue);
            }
            else if (e.PropertyName == "NodeIdViewModel")
            {
                if (m.NodeIdModel != null && m.NodeIdModel.IsUserReadable && m.NodeIdModel.IsReadable)
                    UpdateModel(monitoredKey, -1, m.DisplayName, m.NodeIdModel);
            }
        }
        void RestartRealTime(bool bOnLoad = false)
        {
            if (viewList != null)
            {
                foreach (var view in viewList.Values)
                {
                    view.Error -= ViewList_OnError;
                    view.Dispose();
                }
                viewList.Clear();
            }

            if (mapKeySeries != null && mapKeySeries.Count > 0)
            {
                foreach (var serie in mapKeySeries.Values)
                {
                    serie.lineSerie = null;
                    serie.lineSerieCompare = null;
                    serie.lineSerieMin = null;
                    serie.lineSerieMax = null;
                    serie.lineSerieAvg = null;
                    if (serie.listValues != null)
                    {
                        serie.listValues.Clear();
                    }
                    serie.listValues = null;
                }
                mapKeySeries.Clear();
            }

            if (!bOnLoad)
            {
                foreach (var key in OpcuaEntityReference.Keys)
                {
                    TerminateExecution(key);
                }

                if (mapHandlers != null)
                    mapHandlers.Clear();

                if (opcuaEntityReference != null)
                    opcuaEntityReference.Clear();
                opcuaEntityReference = null;
            }

            StartRecording();
        }

        void SetRealTimeSource(bool bFromSettings = false)
        {
            if (mapKeySeries == null)
                return;

            if (!IsInStop)
                UpdateRealTimeView();

            seriesHistoryDataCount.Clear();

            foreach (var key in mapKeySeries.Keys)
            {
                if (viewList.ContainsKey(key))
                {
                    mapKeySeries[key].expConverter = mapKeySeries[key].expConverter != null ?
                        mapKeySeries[key].expConverter :
                        GetExpressionValueConverter(mapKeySeries[key].penUnitConverter, mapKeySeries[key].DName);
                    if (mapKeySeries[key].listValues != null && mapKeySeries[key].listValues.Count > 0)
                        viewList[key].AddData(mapKeySeries[key].listValues.First().dValueConverted); //updating realtime LastValue to last historical available value
                    viewList[key].collectionValues.PrependList(mapKeySeries[key].listValues, mapKeySeries[key].expConverter, true);
                    if (mapKeySeries[key].lineSerie == null && lineSeries.ContainsKey(key))
                        mapKeySeries[key].lineSerie = lineSeries[key];
                    mapKeySeries[key].lineSerie.BeginInit();
                    seriesHistoryDataCount[mapKeySeries[key].lineSerie] = viewList[key].HistoryData.Count;
                    if (IsInStop)
                        mapKeySeries[key].lineSerie.DataSource = viewList[key].HistoryData;
                    else
                        mapKeySeries[key].lineSerie.DataSource = viewList[key].collectionValues;
                    mapKeySeries[key].lineSerie.EndInit();
                }
            }
            if (axisX.ActualWholeRange.MinValue != null)
                minValueDate = (DateTime)axisX.ActualWholeRange.MinValue;
        }

        private void AutoStopPlayOnScroll(object sender, XYDiagram2DScrollEventArgs e)
        {
            if (e.ScrollOrientation == XYDiagram2DScrollOrientation.AxisYScroll || PenList == null || PenList.Count == 0)
                return;

            var newVisualMax = (DateTime)e.NewXRange.MaxValue;
            var actScrollMax = (DateTime)axisX.ActualWholeRange.MaxValue;
            if (!IsInStop && (lastPrintedLivePointDate == null || newVisualMax < lastPrintedLivePointDate) && newVisualMax < actScrollMax)
            {
                Pause(this, null);
            }
            else
            {
                try
                {
                    if (IsInStop && (lastPrintedLivePointDate == null || newVisualMax > lastPrintedLivePointDate) && newVisualMax > actScrollMax.Add(-new TimeSpan(ViewTimeFrame.Ticks / 100)))
                    {
                        Play(this, null);
                    }
                }
                catch { }
            }
        }

        string PrepareRealTimeSerie(string serie, PenItem data)
        {
            var id = data.guiId != null ? data.guiId : Guid.NewGuid().ToString();
            string keyname = string.Format("{0}_{1}", id, data.ArrayIndex);
            if (mapKeySeries == null)
                mapKeySeries = new Dictionary<string, SerieSettings>();
            if (!mapKeySeries.ContainsKey(keyname))
            {
                if (viewList == null)
                    viewList = new Dictionary<string, TrendDataGenerator>();
                var settings = new DataGeneratorSettings()
                {
                    ClientTimezoneOffset = ClientTimezoneOffset,
                    HDataCount = SampleNumber,
                    DeadBandInterval = RecordEvery,
                    DeadBandTimeFrame = ViewTimeFrame,
                    ArrayIndex = data.ArrayIndex
                };
                settingStorage.mapSeries[serie].expConverter = settingStorage.mapSeries[serie].expConverter != null ?
                    settingStorage.mapSeries[serie].expConverter :
                    GetExpressionValueConverter(settingStorage.mapSeries[serie].penUnitConverter, settingStorage.mapSeries[serie].DName);
                var view = new TrendDataGenerator(keyname, settings, RecordEvery, settingStorage.mapSeries[serie].expConverter);
                view.Error += ViewList_OnError;
                viewList[keyname] = view;
                if (settingStorage != null && settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(serie))
                    mapKeySeries[keyname] = settingStorage.mapSeries[serie];
                if (mapHandlers.ContainsKey(id))
                    mapHandlers[id].RefreshValue();
            }
            return keyname;
        }

        private void StartRecording()
        {
            if (timer != null)
                timer.Stop();

            if (OpcuaEntityReference.Count > 0)
            {
                try
                {
                    if (OpcuaEntityReference.Count > 0)
                        foreach (var peniten in PenList)
                        {
                            var key = peniten.guiId;
                            PrepareItem(key, peniten.ArrayIndex);
                        }
                }
                catch (Exception)
                {
                }
            }

            if (RecordEvery.Ticks > 0)
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    timer.Tick += timer_Tick;
                    timer.Interval = RecordEvery;
                    timer.Start();
                }
                else if (!timer.IsEnabled)
                {
                    timer.Start();
                }
            }
        }
        void PrepareItem(string key, int arreayIndex = -1)
        {
            if (OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key] != null)
            {
                //if ((!matchChangedMap.Contains(key) || (mapHandlers != null && !mapHandlers.ContainsKey(key))) && (bInit || !OpcuaEntityReference[key].IsRelative))
                //    PrepareExecution(key, arreayIndex);
                //if (matchChangedMap.Contains(key) && !bInit)
                //{
                //    if (mapHandlers != null && mapHandlers.ContainsKey(key) && OpcuaEntityReference[key].NodeIdViewModel != null)
                //        PenItem_ModelChanged(mapHandlers[key], new ModelChangedEventArgs(OpcuaEntityReference[key].HumanReadable, OpcuaEntityReference[key].NodeIdViewModel));
                //}
                if (!matchChangedMap.Contains(key) && (bInit || !OpcuaEntityReference[key].IsRelative))
                    PrepareExecution(key, arreayIndex);
                else if (matchChangedMap.Contains(key) && !bInit)
                {
                    if (mapHandlers != null && mapHandlers.ContainsKey(key) && OpcuaEntityReference[key].NodeIdViewModel != null)
                        PenItem_ModelChanged(mapHandlers[key], new ModelChangedEventArgs(OpcuaEntityReference[key].HumanReadable, OpcuaEntityReference[key].NodeIdViewModel));
                }
            }
        }
        DispatcherOperation dpPlayValue;
        void timer_Tick(object sender, EventArgs e)
        {
            if (viewList != null)
            {
                foreach (var vl in viewList)
                {
                    vl.Value.PlotData();
                    UpdateLegend(vl.Key);
                }
            }

            if (viewDataContext != null)
            {
                viewDataContext.PlotData();
                if(monitoredKey != null)
                    UpdateLegend(monitoredKey);
            }

            if (!IsInStop)
            {
                if (dpPlayValue == null ||
                    dpPlayValue.Status == DispatcherOperationStatus.Completed ||
                    dpPlayValue.Status == DispatcherOperationStatus.Aborted)
                {
                    dpPlayValue = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                    {
                        if (bDispose || IsInStop)
                            return;

		                var utcNow = DateTime.UtcNow;
		                if (RunningOnServer)
		                    utcNow = utcNow.Add(ClientTimezoneOffset);
		                else
		                    utcNow = utcNow.ToLocalTime();
		
		                UpdateRealTimeView(utcNow);
		                lastPrintedLivePointDate = utcNow;
                    });
                }
            }
        }

        private string GetLinkedPenName()
        {
            string _LinkedPenName = !string.IsNullOrEmpty(LinkedPenName) ? LinkedPenName : Properties.Resources.DroppedTag;
            return GetTranslation(_LinkedPenName);
        }

        private string GetTranslation(string name)
        {
           return TranslationHelpers.TranslationHelper.TranslateComposedText(name, stringlist, name);
        }

        void UpdateRealTimeView(DateTime? endDate = null)
        {
            if (endDate == null)
            {
                if (RunningOnServer)
                    endDate = DateTime.UtcNow.Add(ClientTimezoneOffset);
                else
                    endDate = DateTime.UtcNow.ToLocalTime();
            }
            var minDate = ((DateTime)endDate).AddMilliseconds(-ViewTimeFrame.TotalMilliseconds);
            axisX.WholeRange.MaxValue = endDate; //extending scrollbar
            axisX.VisualRange = new DevExpress.Xpf.Charts.Range() { MinValue = minDate, MaxValue = endDate }; //delimiting visible chart's view

            if (bDiscreteTimeMode)
                SetDateTimeScaleOptions(DateTimeGridAlignment.Second, DateTimeMeasureUnit.Millisecond);
                //SetTimeScaleAlignment(minDate, (DateTime)endDate, true);
        }

        void UpdateModel(string key, int arrayIndex, string humanReadable, NodeIdViewModel model)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose)
                    return;

                if (key == monitoredKey)
                {
                    if (linkedPenSettings != null)
                    {
                        if (!model.IsScalar)
                            linkedPenSettings.Name = string.Format("{0} i:{1}", humanReadable, arrayIndex);
                        linkedPenSettings.EUnit = model.EUInformation?.DisplayName?.ToString();
                    }
                }
                else
                {
                    if (settingStorage != null && settingStorage.mapSeries != null /*&& settingStorage.mapSeries.ContainsKey(key)*/)
                    {
                        string pen = (from k in settingStorage.mapSeries.Keys where settingStorage.mapSeries[k].SGuid == key select k).FirstOrDefault();
                        if (!string.IsNullOrEmpty(pen))
                        {
                            if (!model.IsScalar)
                                settingStorage.mapSeries[pen].Name = string.Format("{0} i:{1}", humanReadable, arrayIndex);
                            settingStorage.mapSeries[pen].EUnit = model.EUInformation?.DisplayName?.ToString();
                        }
                    }
                }                
            });
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
                        if (bDispose)
                            return;

                        Dictionary<string, object> tmpqueued;
                        lock (queuedValues)
                        {
                            tmpqueued = new Dictionary<string, object>(queuedValues);
                            queuedValues.Clear();
                        }

                        foreach (var k in tmpqueued.Keys)
                        {
                            PrintMonitored(k, tmpqueued[k]);
                        }
                    });
                }
            }
        }

        void PrintMonitored(string key, object newValue)
        {
            bool isArray = newValue is IList;
            if (isArray)
            {
                var dataCollection = newValue as List<MonitoredItemViewModel.DataObject>;
                if (dataCollection != null && dataCollection.Count > 0)
                {
                    if (viewDataContext != null && key == monitoredKey)
                    {
                        for (ushort ii = 0; ii < dataCollection.Count; ii++)
                        {
                            viewDataContext.AddData(dataCollection[ii].Value);
                        }
                    }
                    else if (viewList != null && viewList.ContainsKey(key))
                    {
                        for (ushort ii = 0; ii < dataCollection.Count; ii++)
                        {
                            viewList[key].AddData(dataCollection[ii].Value);
                        }
                    }
                }
            }
            else if (viewDataContext != null && key == monitoredKey && newValue != null)
            {
                var newDValue = newValue as DataValue;
                if(newDValue != null)
                    viewDataContext.AddData(newDValue.Value);
            }
            else if (viewList != null && viewList.ContainsKey(key) && newValue != null)
            {
                var newDValue = newValue as DataValue;
                if (newDValue != null)
                    viewList[key].AddData(newDValue.Value);
            }
        }

        void UpdateLegend(string k)
        {
            if(k == monitoredKey && linkedPenSettings != null)
            {
                linkedPenSettings.DValue = viewDataContext.LastValue?.ToDataValue();
                linkedPenSettings.minValue = viewDataContext.MinValue;
                linkedPenSettings.maxValue = viewDataContext.MaxValue;
                linkedPenSettings.averageValue = viewDataContext.AvgValue;
                try
                {
                    var val = Convert.ToDouble(linkedPenSettings.DValue?.Value, System.Globalization.CultureInfo.InvariantCulture);
                    currentValues.Set(k, val);
                }
                catch { }
            }
            else if (settingStorage != null && settingStorage.mapSeries != null && mapKeySeries.ContainsKey(k) && mapKeySeries[k] != null && viewList.ContainsKey(k))
            {
                mapKeySeries[k].DValue = viewList[k].LastValue?.ToDataValue();
                mapKeySeries[k].minValue = viewList[k].MinValue;
                mapKeySeries[k].maxValue = viewList[k].MaxValue;
                mapKeySeries[k].averageValue = viewList[k].AvgValue;
                try
                {
                    var val = Convert.ToDouble(mapKeySeries[k].DValue?.Value, System.Globalization.CultureInfo.InvariantCulture);
                    currentValues.Set(k, val);
                }
                catch { }
            }
            bSettingLegendSource = true;
            legend_GridControl.RefreshData();
            bSettingLegendSource = false;
        }

        void chart_BoundDataChanged(object sender, RoutedEventArgs e)
        {
            foreach (var axis in axisToUpdateSideMargins)
            {
                double? actualMaxValue = null;
                double? actualMinValue = null;

                try
                {
                    actualMaxValue = Convert.ToDouble(axis.ActualWholeRange.ActualMaxValue);
                    actualMinValue = Convert.ToDouble(axis.ActualWholeRange.ActualMinValue);
                }
                catch
                { }

                if (actualMaxValue.HasValue && actualMinValue.HasValue)
                {
                    var sideMargins = ((axis.ActualWholeRange.ActualMaxValueInternal - axis.ActualWholeRange.SideMarginsValue) - (axis.ActualWholeRange.ActualMinValueInternal + axis.ActualWholeRange.SideMarginsValue)) * ScalePaddingFactor / 100;
                    if (axis.ActualWholeRange.SideMarginsValue != sideMargins && sideMargins > 0)
                        axis.ActualWholeRange.SideMarginsValue = sideMargins;
                }
            }


            foreach (var axis in axisToUpdateGridSpacing)
            {
                double? actualMaxValue = null;
                double? actualMinValue = null;

                try
                {
                    actualMaxValue = Convert.ToDouble(axis.ActualWholeRange.ActualMaxValue);
                    actualMinValue = Convert.ToDouble(axis.ActualWholeRange.ActualMinValue);
                }
                catch
                { }

                if (actualMaxValue.HasValue && actualMinValue.HasValue)
                {
                    var delta = actualMaxValue.Value - actualMinValue.Value;
                    double division = Math.Max(1.0, MajorYTicks);
                    var gridSpacing = Math.Max(1.0, delta / division);
                    if (axis.NumericScaleOptions.GridSpacing != gridSpacing)
                        axis.NumericScaleOptions.GridSpacing = gridSpacing;
                }
            }
        }

        private void ScrollHorizontally(int factor, bool bForeward)
        {
            try
            {
                if (factor == 0)
                {
                    if (bForeward)
                    {
                        diagram.ScrollAxisXTo(1);
                    }
                    else
                    {
                        diagram.ScrollAxisXTo(0);
                    }
                    return;
                }

                DateTime maxValue;
                DateTime minValue;
                
                if (RunningOnServer)
                    maxValue = DateTime.UtcNow.Add(ClientTimezoneOffset);
                else
                    maxValue = DateTime.UtcNow.ToLocalTime();

                if (firstStartTime == DateTime.MinValue)
                    firstStartTime = maxValue.Subtract(ViewTimeFrame);

                minValue = firstStartTime;


                TimeSpan delta = maxValue.Subtract(minValue); 

                DateTime endDate;
                DateTime startDate;
                if (axisX.VisualRange != null)
                {
                    endDate = (DateTime)axisX.VisualRange.ActualMaxValue;
                    startDate = (DateTime)axisX.VisualRange.ActualMinValue;
                    delta = endDate.Subtract(startDate);
                }
                else
                {
                    endDate = maxValue;
                    startDate = minValue;
                }

                delta = new TimeSpan(delta.Ticks * Math.Abs(factor));

                int px = (int)(delta.Ticks * factor / RecordEvery.Ticks);
                if (bForeward)
                {
                    if (diagram.CanScrollHorizontally(-px))
                        diagram.ScrollHorizontally(-px);
                    else
                        diagram.ScrollAxisXTo(1);
                }
                else
                {
                    if (diagram.CanScrollHorizontally(px))
                        diagram.ScrollHorizontally(px);
                    else
                        diagram.ScrollAxisXTo(0);
                }
            }
            catch (Exception)
            {
            }
        }

        private void GoToNext(object sender, RoutedEventArgs e)
        {
            ScrollHorizontally(1, true);
        }
        private void GoToNextNext(object sender, RoutedEventArgs e)
        {
           ScrollHorizontally(2, true);
        }
        private void GoToEnd(object sender, RoutedEventArgs e)
        {
            ScrollHorizontally(0, true);
        }
        private void GoToPrev(object sender, RoutedEventArgs e)
        {
            ScrollHorizontally(1, false);
        }
        private void GoToPrevPrev(object sender, RoutedEventArgs e)
        {
            ScrollHorizontally(2, false);
        }
        private void GoToStart(object sender, RoutedEventArgs e)
        {
            ScrollHorizontally(0, false);
        }


        private void Play(object sender, RoutedEventArgs e)
        {
            bChangingPlayPause = true;
            try
            {
                if (!IsInStop)
                    return;
                IsInStop = false;
                PlayExecute();
            }
            finally
            {
                bChangingPlayPause = false;
            }
        }

        void ChangeDateTimeScale(Type newScale /*, bool bResetTimeScaleAlignment = false*/)
        {
            if (!bDiscreteTimeMode || axisX.DateTimeScaleOptions.GetType() == newScale)
                return;
            if (newScale == typeof(ContinuousDateTimeScaleOptions))
            {
                axisX.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions();
                axisX.SetBinding(ContinuousDateTimeScaleOptions.AutoGridProperty, new Binding("XAutoGrid") { Source = this });
            }
            else if (newScale == typeof(ManualDateTimeScaleOptions))
            {
                var oldVRmin = axisX.ActualVisualRange?.ActualMinValue;
                var oldVRmax = axisX.ActualVisualRange?.ActualMaxValue;
                axisX.DateTimeScaleOptions = new ManualDateTimeScaleOptions() { AggregateFunction = AggregateFunction.None, MeasureUnitMultiplier = MeasureUnitMultiplier };
                axisX.SetBinding(ManualDateTimeScaleOptions.AutoGridProperty, new Binding("XAutoGrid") { Source = this });
                //if (bResetTimeScaleAlignment && oldVRmin is DateTime && oldVRmax is DateTime)
                //    SetTimeScaleAlignment((DateTime)oldVRmin, (DateTime)oldVRmax);
            }
        }

        void PlayExecute()
        {
            axisX.WholeRange.SideMarginsValue = 0;

            //ChangeDateTimeScale(typeof(ContinuousDateTimeScaleOptions));
            if (mapKeySeries == null)
                mapKeySeries = new Dictionary<string, SerieSettings>();

            foreach (var key in mapKeySeries.Keys)
            {
                if (viewList.ContainsKey(key))
                {
                    if (mapKeySeries[key].lineSerie == null && lineSeries.ContainsKey(key))
                        mapKeySeries[key].lineSerie = lineSeries[key];

                    if (mapKeySeries[key].UseTableAggregation)
                    {
                        var diagramSerie = (from c in diagram.Series.OfType<Series>()
                                            where c.DisplayName == mapKeySeries[key].lineSerie.DisplayName || c.DisplayName == ($"{mapKeySeries[key].lineSerie.DisplayName} - {c.Tag as String}")
                                            select c).FirstOrDefault();
                        if (diagramSerie != null)
                        {
                            if (bFirstPlay)
                            {
                                mapKeySeries[key].expConverter = mapKeySeries[key].expConverter != null ?
                                    mapKeySeries[key].expConverter:
                                    GetExpressionValueConverter(mapKeySeries[key].penUnitConverter, mapKeySeries[key].DName);
                                bFirstPlay = false;
                                viewList[key].collectionValues.PrependList(diagramSerie.DataSource as List<MyDataValue>, mapKeySeries[key].expConverter, true);
                            }
                            diagramSerie.DataSource = viewList[key].collectionValues;
                        }
                    }

                    mapKeySeries[key].lineSerie.BeginInit();
                    mapKeySeries[key].lineSerie.DataSource = viewList[key].collectionValues;
                    mapKeySeries[key].lineSerie.EndInit();
                }
            }

            if (viewDataContext != null)
            {
                dataContextSerie.BeginInit();
                dataContextSerie.DataSource = viewDataContext.collectionValues;
                dataContextSerie.EndInit();
            }

            axisX.WholeRange.MinValue = minValueDate;
            timer_Tick(this, null);
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            bChangingPlayPause = true;
            try
            {
                if (IsInStop)
                    return;
                IsInStop = true;
                PauseExecute();
            }
            finally
            {
                bChangingPlayPause = false;
            }
        }

        void PauseExecute()
        {
            if (axisX.ActualVisualRange.MaxValue != null)
                lastPrintedLivePointDate = (DateTime)axisX.ActualVisualRange.MaxValue;
            //axisX.WholeRange.MaxValue = ((DateTime)axisX.ActualWholeRange.MaxValue).Add(new TimeSpan(ViewTimeFrame.Ticks / 50));
            axisX.WholeRange.SideMarginsValue = (axisX.VisualRange.ActualMaxValueInternal - axisX.VisualRange.ActualMinValueInternal) / 50;
            
            foreach (var key in mapKeySeries.Keys)
            {
                if (viewList.ContainsKey(key))
                {
                    if (mapKeySeries[key].lineSerie == null && lineSeries.ContainsKey(key))
                        mapKeySeries[key].lineSerie = lineSeries[key];
                    mapKeySeries[key].lineSerie.BeginInit();
                    mapKeySeries[key].lineSerie.DataSource = viewList[key].HistoryData;
                    mapKeySeries[key].lineSerie.EndInit();
                }
            }

            if (viewDataContext != null)
            {
                dataContextSerie.BeginInit();
                dataContextSerie.DataSource = viewDataContext.HistoryData;
                dataContextSerie.EndInit();
            }
        }
        #endregion

        void ApplyDateTimeFormat()
        {
            var format = GetDateTimeFormat();

            axisX.Label.TextPattern = String.Format("{{A{0}}}", String.Format(":{0}", format));

            if (!bDesignmode)
            {
                axisX.CrosshairAxisLabelOptions.Pattern = String.Format("{{A:{0}}}", format);
                var series = (from Series serie in diagram.Series where serie is XYSeries2D select serie as XYSeries2D).ToList();
                foreach (var serie in series)
                    serie.CrosshairLabelPattern = String.Format("{{S}}\n{{V:F{0}}}\n{{A:{1}}}", PointPrecision, format);
            }
        }

        string GetDateTimeFormat()
        {
            var format = DateTimeFormat;
            if (String.IsNullOrEmpty(format?.Trim()))
                format = String.Format("{0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
            return format;
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
        DateTime firstStartTime;
        /// <summary>
        /// Use this method to set custom time range for data extraction
        /// </summary>
        /// <param name="delta"></param>
        public void SetTimeRange(TimeSpan delta)
        {
            if (RunningOnServer)
                settingStorage.DateTimeEnd = DateTime.UtcNow.Add(ClientTimezoneOffset);
            else
                settingStorage.DateTimeEnd = DateTime.UtcNow.ToLocalTime();

            settingStorage.DateTimeStart = settingStorage.DateTimeEnd.Subtract(delta);
            firstStartTime = settingStorage.DateTimeEnd.Subtract(delta);

            if (settingStorage.DateTimeStart < minDateTimeValue)
                settingStorage.DateTimeStart = minDateTimeValue;
            axisX.ActualWholeRange.MinValue = settingStorage.DateTimeStart;
            axisX.ActualWholeRange.MaxValue = settingStorage.DateTimeEnd;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!bDispose && SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
        }

        private void InitRuntimeConfig()
        {
            designPenList = PenList != null ? new PenItemList(PenList) : new PenItemList();

            helper = new Helper(Document, this as ISettingsHelper);
            helper.RefreshCurrentUser();

            designDockLayout = DockLayout;
            designListLayout = ListViewLayout;
            designGridLayout = GridLayout;
            designUseAbsoluteRanges = UseAbsoluteRanges;

            InitMemorySettingList();

            configMemory.DataContext = MemorySettingList?.Names;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }

        void InitMemorySettingList()
        {
            string defaultTagSetting = Properties.Settings.Default.DesignSettingName;
            MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            var defaultsetting = (from m in MemorySettingList where m.Name.Equals(defaultTagSetting) select m).FirstOrDefault();
            if (defaultsetting == null)
                MemorySettingList.Add(new Setting()
                {
                    Name = defaultTagSetting,
                    PenList = designPenList,
                    GridLayout = designGridLayout,
                    DockLayout = designDockLayout,
                    ListViewLayout = designListLayout,
                    UseAbsoluteRanges = designUseAbsoluteRanges,
                    ReadOnly = true
                });
            else
            {
                defaultsetting.ReadOnly = true;
                defaultsetting.GridLayout = designGridLayout;
                defaultsetting.UseAbsoluteRanges = designUseAbsoluteRanges;
                defaultsetting.DockLayout = designDockLayout;
                defaultsetting.ListViewLayout = designListLayout;
                defaultsetting.PenList = designPenList;
            }
        }

        private bool WriteProjectDataStream(Stream ostrm, MemorySettings memories)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(MemorySettings));
                    serializer.WriteObject(writer, memories);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }
        internal string stringPlaceolder = "RealTimeTrend";
        internal void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                editGeneralSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EditSettings", stringlist, Properties.Resources.EditSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                legendPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendTitle", stringlist, Properties.Resources.LegendTitle);
                gridPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTitle", stringlist, Properties.Resources.GridTitle);

                configMemory.EditValue = ActualConfig;
 
                //chkUseAbsoluteRanges.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsTitle", stringlist, Properties.Resources.UseAbsoluteRangesTitle);
                btnPrintGrid.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintTitle", stringlist, Properties.Resources.PrintTitle);
                btnPause.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PauseBtnTitle", stringlist, Properties.Resources.Pause);
                btnPlay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PlayBtnTitle", stringlist, Properties.Resources.Play);


                var lastTextPattern = TextPattern;
                TextPattern = "";
                TextPattern = lastTextPattern;

                lastTextPattern = AxisYTextPattern;
                AxisYTextPattern = "";
                AxisYTextPattern = lastTextPattern;

                ApplyDateTimeFormat();

                visibleTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_VisibleColumn", stringlist, Properties.Resources.VisibleTitle);
                colorTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PenColorColumn", stringlist, Properties.Resources.ColorTitle);
                nameTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NameTitleColumn", stringlist, Properties.Resources.NameTitle);
                eUnitTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EUnitColumn", stringlist, Properties.Resources.EUnit);
                statMinTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticMinColumn", stringlist, Properties.Resources.StatisticMin);
                statMaxTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticMaxColumn", stringlist, Properties.Resources.StatisticMax);
                avgTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AvgColumn", stringlist, Properties.Resources.Average);
                dValueTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ValueColumn", stringlist, Properties.Resources.DValue);
                pointDateTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PointDateColumn", stringlist, Properties.Resources.PointDate);
                pointValueTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PointValueColumn", stringlist, Properties.Resources.PointValue);

                gridTagName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTagName", stringlist, Properties.Resources.GridTagName);

                TranslationHelper.TranlslateColumns(legend_GridControl.Columns, stringlist, stringPlaceolder, true);
                TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;
                UpdateSeries();
            });
        }

        private void UpdateSeries()
        {
            if (settingStorage == null || settingStorage.mapSeries == null)
                return;
            PenList.ForEach(data =>
            {
                var penName = data.Name;
                if (!String.IsNullOrEmpty(data.title))
                    penName = data.title;
                if (settingStorage.mapSeries.ContainsKey(penName))
                {
                    string _LinkedPenName = data.Name;
                    _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(_LinkedPenName, stringlist, _LinkedPenName);
                    settingStorage.mapSeries[penName].DName = _LinkedPenName;
                }
            });
            SetLegendSource();
        }
        #endregion

        #region EventHandlers
        public event EventHandler ControlLoaded;
        bool bControlLoaded;
        void OnControlLoaded()
        {
            SetRealTimeSource();
            IsInStop = false;
            ControlLoaded?.Invoke(this, EventArgs.Empty);
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
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                IUIMsgBoxAlertService UIInterface = null;
                IHelpProvider helpProvider = null;
                if (Document != null)
                {
                    UIInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                }

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(PointPrecisionProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 1.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(XSmallTicksPerIntervalProperty, dt);
                mapDataTemplates.Add(YSmallTicksPerIntervalProperty, dt);
                mapDataTemplates.Add(MajorXTicksProperty, dt);
                mapDataTemplates.Add(MajorYTicksProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
                factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss.fff", Properties.Resources.TimeSpanFormatDaysPart));
                dt.DataType = typeof(TimeSpan);
                dt.VisualTree = factory;
                mapDataTemplates.Add(RecordEveryProperty, dt);
                mapDataTemplates.Add(ViewTimeFrameProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, 99.0);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ScalePaddingFactorProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 1.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SelectedPenThicknessProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, 90.0);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(AllValuesXMarginProperty, dt);

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

        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "XMajorCount" || propertyName == "XMinorCount")
                {
                    return !XAutoGrid;
                }
                if (propertyName == "YMajorCount" || propertyName == "YMinorCount")
                {
                    return !YAutoGrid;
                }
                //if (propertyName == "RecordEvery" || propertyName == "StartStopped" || propertyName == "RecordOnlyOnQualityGood" || propertyName == "ViewTimeFrame" || propertyName == "LinkedPenName")
                //{
                //    return RuntimeMode != RuntimeMode.OnlyStop;
                //}

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region IDisposable
        bool bDispose;
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!bDispose)
            {
                bDispose = true;

                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                }

                if (tokenSource != null)
                    tokenSource.Cancel();

                if (tokenSource != null)
                    tokenSource.Dispose();

                if (zommingRangeList != null)
                    zommingRangeList.Clear();
                zommingRangeList = null;

                if (oldMemoryList != null)
                    oldMemoryList.Clear();
                oldMemoryList = null;

                if (MemorySettingList != null)
                {
                    foreach (var ms in MemorySettingList)
                        ms.PenList.Clear();
                    MemorySettingList.Clear();
                }
                MemorySettingList = null;

                if (PenList != null)
                    PenList.Clear();

                configMemory.DataContext = null;
                legend_GridControl.ItemsSource = null;

                if (linkedPenSettings != null)
                    linkedPenSettings = null;

                if (helper is IDisposable)
                    (helper as IDisposable).Dispose();
                helper = null;
                SizeChanged -= OnSizeChanged;
                SizeChangedInvoker = null;

                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= timer_Tick;
                }

                ((XYDiagram2D)sfchart.Diagram).Scroll -= AutoStopPlayOnScroll;

                axisToUpdateGridSpacing.Clear();
                axisToUpdateSideMargins.Clear();

                if (disposing)
                {
                    if (dpPlayValue != null &&
                        dpPlayValue.Status != DispatcherOperationStatus.Aborted &&
                        dpPlayValue.Status != DispatcherOperationStatus.Completed)
                        dpPlayValue.Abort();
           
                    lock (queuedValues)
                    {
                        if (dpUpdateValue != null &&
                            dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                            dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                            dpUpdateValue.Abort();
                        queuedValues.Clear();
                    }

                    if (viewList != null)
                    {
                        foreach (var view in viewList.Values)
                        {
                            view.Error -= ViewList_OnError;
                            view.Dispose();
                        }
                        viewList.Clear();
                    }

                    if (saveAutoHiddenStream != null)
                        saveAutoHiddenStream.Dispose();


                    gridControl.MouseDown -= lengedGrid_MouseDown;
                    gridControl.ItemsSource = null;
                    if (gridControl is IDisposable)
                        (gridControl as IDisposable).Dispose();

                    if (dockManager.DockController != null && dockManager.DockController is IDisposable)
                        (dockManager.DockController as IDisposable).Dispose();
                    if (dockManager is IDisposable)
                        (dockManager as IDisposable).Dispose();
                }
                //*************
                //set not in use
                //*************
                foreach (var key in OpcuaEntityReference.Keys)
                {
                    TerminateExecution(key);
                }

                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                monitoredItemViewModel = null;

                if (dataContextSerie != null)
                    dataContextSerie.DataSource = null;

                if (viewDataContext != null)
                {
                    viewDataContext.Error -= ViewList_OnError;
                    viewDataContext.Dispose();
                }

                if (mapHandlers != null)
                {
                    foreach (var helper in mapHandlers.Values)
                        helper.Dispose();
                    mapHandlers.Clear();
                }

                if (opcuaEntityReference != null)
                    opcuaEntityReference.Clear();

                saveAutoHiddenStream = null;

                DetachOverrideBaseProperties();

                toolbar.MouseEnter -= toolbar_MouseEnter;
                toolbar.MouseLeave -= toolbar_MouseLeave;

                if (!bDesignmode)
                    RestoreAndSaveBeforeQuit();

                gridControl.ItemsSource = null;

                if (settingStorage != null)
                {
                    if (settingStorage.ListRanges != null)
                        settingStorage.ListRanges.Clear();
                    settingStorage.ListRanges = null;

                    if (settingStorage.mapSeries != null && settingStorage.mapSeries.Count > 0)
                    {
                        foreach (var key in settingStorage.mapSeries.Keys)
                        {
                            settingStorage.mapSeries[key].lineSerie = null;
                            settingStorage.mapSeries[key].lineSerieCompare = null;
                            settingStorage.mapSeries[key].lineSerieMin = null;
                            settingStorage.mapSeries[key].lineSerieMax = null;
                            settingStorage.mapSeries[key].lineSerieAvg = null;
                            if (settingStorage.mapSeries[key].listValues != null)
                            {
                                settingStorage.mapSeries[key].listValues.Clear();
                            }
                            settingStorage.mapSeries[key].listValues = null;
                            if (settingStorage.mapSeries[key].expConverter != null)
                            {
                                settingStorage.mapSeries[key].expConverter.Dispose();
                                settingStorage.mapSeries[key].expConverter = null;
                            }
                        }
                        settingStorage.mapSeries.Clear();
                    }
                    settingStorage.mapSeries = null;
                    settingStorage.Dispose();
                    settingStorage = null;
                }
                if (mapKeySeries != null && mapKeySeries.Count > 0)
                {
                    foreach (var serie in mapKeySeries.Values)
                    {
                        serie.lineSerie = null;
                        serie.lineSerieCompare = null;
                        serie.lineSerieMin = null;
                        serie.lineSerieMax = null;
                        serie.lineSerieAvg = null;
                        if (serie.listValues != null)
                        {
                            serie.listValues.Clear();
                        }
                        serie.listValues = null;
                        serie.expConverter?.Dispose();
                        serie.expConverter = null;
                    }
                    mapKeySeries.Clear();
                }
                mapKeySeries = null;
                foreach (var serie in diagram.Series)
                {
                    List<MyDataValue> _datasource = (serie.DataSource as List<MyDataValue>);
                    serie.BeginInit();
                    serie.DataSource = null;
                    if (_datasource != null)
                    {
                        _datasource.Clear();
                    }
                    _datasource = null;
                    serie.Tag = null;
                    //serie.Points.Clear();
                    serie.EndInit();
                }

                diagram.SecondaryAxesY.Clear();
                diagram.Series.Clear();

                sfchart.PreviewMouseWheel -= chart_PreviewMouseWheel;
                diagram.MouseWheel -= diagram_MouseWheel;
                diagram.Zoom -= diagram_Zoom;

                sfchart.PreviewKeyDown -= chart_PreviewKeyDown;
                sfchart.CustomDrawSeries -= chart_CustomDrawSeries;
                sfchart.PreviewMouseDown -= chart_MouseDown;
                sfchart.CustomDrawCrosshair -= chart_CustomDrawCrosshair;
                sfchart.BoundDataChanged -= chart_BoundDataChanged;

                nearestPoints.Clear();

                if (listSeries != null)
                    listSeries.Clear();

                if (lineSeries != null)
                    lineSeries.Clear();

                if (stringManager != null)
                    stringManager.CultureChanged -= StringManager_CultureChanged;

                if (MapToListVariablesNodeId != null)
                {
                    MapToListVariablesNodeId.Clear();
                    MapToListVariablesNodeId = null;
                }

                if (designPenList != null)
                {
                    designPenList.Clear();
                    designPenList = null;
                }

                typeHelper.Dispose();
                typeHelper = null;

                condValues.Clear();
            }
        }

        #endregion

        #region Isolated Storage
        internal String GetStorageName(bool useParent = false)
        {
            return StorageHelper.StorageHelper.GetStorageName(Document, this.Name, UserBasedRuntimeSettings ? helper.Username : null, true);
        }

        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
        }

        //static String GetStoreFileNameGrid(String title)
        //{
        //    return String.Format("{0}.{1}Grid.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        //}

        //static String GetStoreFileNameChart(String title)
        //{
        //    return String.Format("{0}.{1}Chart.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        //}

        //static String GetStoreFileName(String title)
        //{
        //    return String.Format("{0}.{1}Data.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        //}

        //static String GetStoreListViewFileName(String title)
        //{
        //    return String.Format("{0}.{1}LW.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        //}


        static IsolatedStorageFile GetStorage()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }

        bool bLayoutSaved;
        string DictionaryNodeName = "Columns";

        #endregion

        #region UseAbsoluteRanges
        public static readonly DependencyProperty UseAbsoluteRangesProperty = DependencyProperty.Register("UseAbsoluteRanges", typeof(bool), typeof(RealTimeTrendControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseAbsoluteRangesChanged), new CoerceValueCallback(OnCoerceUseAbsoluteRanges)));

        private static object OnCoerceUseAbsoluteRanges(DependencyObject o, object value)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                return control.OnCoerceUseAbsoluteRanges((bool)value);
            else
                return value;
        }

        private static void OnUseAbsoluteRangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RealTimeTrendControl control = o as RealTimeTrendControl;
            if (control != null)
                control.OnUseAbsoluteRangesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseAbsoluteRanges(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseAbsoluteRangesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bDesignmode && oldValue != newValue)
            {
                SetDataSources();
            }
        }

        [Browsable(false)]
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

        #region TouchKeyboard

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Show();
        }

        #endregion

        #region Methods

        private void SetWebAsset()
        {
            //btnChartDesigner.IsEnabled = false;
            btnShowCrossHair.IsEnabled = false;
            btnExpand.IsEnabled = false;

            MoveLegend();
        }

        private void MoveLegend()
        {
            try
            {
                var _legend = legendInnerPanel;
                var legendHeightFactor = Math.Max(1.5, Properties.Settings.Default.WebLegendHeightFactor);
                _legend.Height = Math.Max(Properties.Settings.Default.WebLegendMinHeight, this.Height / legendHeightFactor);
                legend_GridControl.FontSize = FontSize;

                //legend_GridControl.Columns.Clear();
                legendPanel.Content = null;

                if (!mainChartGrid.Children.Contains(_legend))
                    mainChartGrid.Children.Add(_legend);
                Grid.SetRow(_legend, 2);
                (legend_GridControl.View as TableView).BestFitColumns();
            }
            catch (Exception)
            {
            }
        }

        bool bRestored;
        void RestoreAndSaveBeforeQuit()
        {
            if (bRestored)
                return;
            bRestored = true;
            RestoreAllHidden();
        }

        bool bHidden = false;

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

        MemoryStream saveAutoHiddenStream;
        void HideAllHidden()
        {
            using (var cursor = new WaitCursor())
            {
                if (!bHidden)
                {
                    bHidden = true;

                    if (saveAutoHiddenStream != null)
                        saveAutoHiddenStream.Dispose();
                    saveAutoHiddenStream = new MemoryStream();
                    dockManager.SaveLayoutToStream(saveAutoHiddenStream);

                    var listToHide = new List<BaseLayoutItem>();
                    dockManager.FloatGroups.ToList().ForEach(group =>
                    {
                        if (!listToHide.Contains(group))
                            listToHide.Add(group);
                    });
                    dockManager.AutoHideGroups.ToList().ForEach(group =>
                    {
                        dockManager.DockController.Dock(group);
                    });
                    GetChildPanels(groupGeneral).ForEach(group =>
                    {
                        if (!listToHide.Contains(group))
                            listToHide.Add(group);
                    });

                    if (listToHide.Contains(chartPanel))
                        listToHide.Remove(chartPanel);
                    listToHide.ForEach(group => dockManager.DockController.Close(group));

                    dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Never;

                    if (RunningOnServer && AlwaysExpanded)
                        legendInnerPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        void RestoreAllHidden()
        {
            using (var cursor = new WaitCursor())
            {
                if (bHidden)
                {
                    bHidden = false;
                    if (saveAutoHiddenStream != null)
                    {
                        try
                        {
                            saveAutoHiddenStream.Seek(0, SeekOrigin.Begin);
                            dockManager.RestoreLayoutFromStream(saveAutoHiddenStream);
                            saveAutoHiddenStream.Dispose();
                            saveAutoHiddenStream = null;
                            dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Auto;
                            if (RunningOnServer)
                            {
                                legendPanel.Visibility = Visibility.Collapsed;
                                legendInnerPanel.Visibility = Visibility.Visible;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        private void ExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            NestMainGridInsideDockManager();
        }

        void NestMainGridInsideDockManager()
        {
            if (LegendAreaVisible || bSmartSettingsEditing)
            {
                if (bHidden)
                    RestoreAllHidden();
            }
            else
            {
                if (!bHidden)
                    HideAllHidden();
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Hide();
        }

        Exception dlException;
        IDataLayer CreateDataLayer(String settings, int commandTimeout)
        {
            IDataLayer safedl = null;

            try
            {
                safedl = UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(settings, commandTimeout);
                dlException = null;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

            return safedl;
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
                if (bIsFetching)
                    bIsFetching = false;
            }
        }

        SecondaryAxisY2D GetSerieYAxis(string serieName)
        {
            string sanitized = GetSanitizedName(serieName);
            return (from c in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault(); 
        }

        void lengedGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridControl.View.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) return;
            if (rowHandle == legendSelectedRowHandle && (legend_GridControl.ItemsSource as List<SerieSettings>).Count > rowHandle)
            {
                var settings = (legend_GridControl.ItemsSource as List<SerieSettings>)[rowHandle];
                var serie = linkedPenSettings != null && settings == linkedPenSettings && dataContextSerie != null ? dataContextSerie :  
                    (from c in diagram.Series.OfType<Series>() where c.DisplayName == settings.Name select c).FirstOrDefault();
                if (serie == null)
                    return;

                settings.SelectUnselectPen();

                if (!settings.ShowAxis && settings.IsVisible)
                {
                    var saFound = GetSerieYAxis(settings.Name);
                    if (saFound != null)
                        saFound.Visible = settings.IsHighlighted;
                }
                if (settings.IsHighlighted)
                {
                    SetPrintGridDataSource(serie);
                    AxisYTextPattern = $"{{V:F{settings.pointPrecision}}}";
                }
                else
                {
                    SetPrintGridDataSource();
                    AxisYTextPattern = $"{{V:F{PointPrecision}}}";
                }
            }
        }

        class GridDataValue
        {
            public DateTime SourceTimeStamp { get; set; }
            public double? dValue { get; set; }
            public string PenName { get; set; }
            public GridDataValue(DateTime st, double? dVal, string pen)
            {
                SourceTimeStamp = st;
                dValue = dVal;
                PenName = pen;
            }
        }

        void SetPrintGridDataSource(Series s = null)
        {
            List<Series> series;
            if (s == null)
                series = (from c in diagram.Series.OfType<Series>() orderby c.DisplayName select c).ToList();
            else
                series = new List<Series> { s };

            if (series.Count == 0)
                return;

            var dataSource = new List<GridDataValue>();
            foreach (var serie in series)
            {
                var name = serie.DisplayName.Replace(String.Format(" - {0}", serie.Tag as String), "");
                SerieSettings settings = settingStorage.mapSeries.ContainsKey(name) ? settingStorage.mapSeries[name] : null;
                if (settings != null && !settings.IsVisible && !settings.ShowAxis)
                    continue;

                var realPoints = new List<MyDataValue>();
                if (serie.DataSource is ReadOnlyList<MyDataValue>)
                {
                    realPoints.AddRange(serie.DataSource as IList<MyDataValue>);
                    if (seriesHistoryDataCount.ContainsKey(serie) && seriesHistoryDataCount[serie] > 0 && realPoints.Count > seriesHistoryDataCount[serie])
                        realPoints.RemoveRange(seriesHistoryDataCount[serie], realPoints.Count - seriesHistoryDataCount[serie]);
                    realPoints.Reverse();
                }
                if (realPoints.Count > 2)
                {
                    if (realPoints[0].bIsFakePoint)
                        realPoints.RemoveAt(0);
                    if (realPoints[realPoints.Count - 1].bIsFakePoint)
                        realPoints.RemoveAt(realPoints.Count - 1);
                }
                realPoints.ForEach(point => { dataSource.Add(new GridDataValue(point.SourceTimestamp, point.dValueConverted, serie.DisplayName)); });
            }
            gridControl.ItemsSource = dataSource;
        }

        private void UnselectPen(Series serie, string name, SerieSettings settings)
        {
            settings.SelectUnselectPen(false);

            var saFound = GetSerieYAxis(name);
            if (saFound != null)
                saFound.Visible = settings.ShowAxis && settings.IsVisible;
        }

        private void legendListBox_SelectionChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            if (!bInit || !bLoaded || bDispose || bSettingLegendSource)
                return;

            if (settingStorage.mapSeries == null)
                settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

            foreach (Series serie in diagram.Series.OfType<Series>())
            {
                string tag = serie.Tag as String;
                var name = serie.DisplayName.Replace(String.Format(" - {0}", tag), "");
                SerieSettings sets = settingStorage.mapSeries.ContainsKey(name) ? settingStorage.mapSeries[name] : null;
                if (sets != null)
                    UnselectPen(serie, name, sets);
            }

            var settings = e.NewRow as SerieSettings;
            legendSelectedRowHandle = (legend_GridControl.ItemsSource as List<SerieSettings>)?.IndexOf(settings);
            if (settings == null)
            {
                tagName.Text = string.Empty;
                SetPrintGridDataSource();
                return;
            }

            (from c in diagram.Series.OfType<Series>()
             where c.DisplayName == settings.Name || c == dataContextSerie
             select c).ToList().ForEach(lsFound =>
             {
                 string tag = lsFound.Tag as String;
                 var name = lsFound.DisplayName.Replace(String.Format(" - {0}", tag), "");

                 // lsFound.Animate();
                 if (lsFound is LineSeries2D)
                 {
                     //lsFound.Tag = (lsFound as LineSeries2D).LineStyle;
                     (lsFound as LineSeries2D).LineStyle.Thickness = Math.Max(1, SelectedPenThickness);
                 }
                 else if (lsFound is AreaSeries2D)
                 {
                     //lsFound.Tag = (lsFound as AreaSeries2D).Border;
                     //(lsFound as AreaSeries2D).Border = new SeriesBorder();
                     (lsFound as AreaSeries2D).Border.LineStyle.Thickness = Math.Max(1, SelectedPenThickness);
                 }
                 else if (lsFound is PointSeries2D)
                 {
                     (lsFound as PointSeries2D).MarkerSize = Math.Max(1, SelectedPenThickness);
                 }
                 if (lsFound.DisplayName == settings.Name)
                 {
                     if (IsInStop)
                        SetPrintGridDataSource(lsFound);
                     tagName.Text = lsFound.DisplayName;
                 }
             });
            //if (IsInStop)
            //    UpdateAxisRange();
        }

        Dictionary<string, Series> lineSeries = new Dictionary<string, Series>();
        void RestoreChartFromSettings(bool bForceFetchData = false, bool bOnLoad = false)
        {
            bForceFetchData = bForceFetchData || forceUpdateOnMatchTypeDefinition;
            forceUpdateOnMatchTypeDefinition = false;
            sfchart.BeginInit();
            try
            {
                InitSettingsForSeriesManagement();

                RestartRealTime(bOnLoad);

                var serie_keynames = new List<string>();
                if (PenList != null && PenList.Count > 0)
                {
                    foreach (var data in PenList)
                    {
                        var serie = UpdateSettingSorage(data);
                        if(serie != null)
                            serie_keynames.Add(PrepareRealTimeSerie(serie, data));
                    }
                }
                bDiscreteTimeMode = (from string key in settingStorage.mapSeries.Keys where settingStorage.mapSeries[key].serieTypeLine == "barSideSeries" select key).FirstOrDefault() != null;
                listSeries.Clear();
                lineSeries.Clear();
                bool needToFetchData = false;
                ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.Clear();

                ChangeDateTimeScale(typeof(ManualDateTimeScaleOptions));

                var i = 0;
                foreach (var serie in settingStorage.mapSeries.Keys)
                {
                    listSeries.Add(serie);

                    GetEUInformation(settingStorage.mapSeries[serie]);
                    //string _LinkedPenName = serie;
                    //_LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(settingStorage.mapSeries[serie].DName, stringlist, settingStorage.mapSeries[serie].DName);
                    //settingStorage.mapSeries[serie].Name = serie;
                    //settingStorage.mapSeries[serie].DName = _LinkedPenName;
                    var ls = AddChartLine(serie, serie, settingStorage.mapSeries[serie].serieTypeLine, settingStorage.mapSeries[serie].thickness, settingStorage.mapSeries[serie].Color, settingStorage.mapSeries[serie].ShowAxis, String.Format("{0}_{1}", settingStorage.mapSeries[serie].SGuid, settingStorage.mapSeries[serie].arrayindex), isvisible: settingStorage.mapSeries[serie].IsVisible, pointPrecision: settingStorage.mapSeries[serie].pointPrecision);
                    if (serie_keynames.Count >= i+1 && !String.IsNullOrEmpty(serie_keynames[i]))
                        lineSeries[serie_keynames[i]] = ls;

                    settingStorage.mapSeries[serie].UnitConverterLabel = ConverterEditorManager?.GetUnitLabel(Document, settingStorage.mapSeries[serie].penUnitConverter, UnitConverterSystem);

                    if (settingStorage.mapSeries[serie].UseTableAggregation)
                    {
                        needToFetchData = true;
                        settingStorage.mapSeries[serie].lineSerie = null;
                    }
                    else
                    {
                        settingStorage.mapSeries[serie].lineSerie = ls;

                        if (settingStorage.mapSeries[serie].listValues == null || bForceFetchData)
                            needToFetchData = true;
                        else if (!AliasHelper.ContainsAlias(settingStorage.mapSeries[serie].TagName))
                            BuildLineFromValues(serie, settingStorage.mapSeries[serie].listValues);
                    }
                    i++;
                }
                SetLegendSource();
                UpdateAxisRange();
                EnableToolbars(IsInStop);
                if (bIsRestarting)
                    bIsRestarting = false;
            }
            finally
            {
                sfchart.EndInit();
            }
        }

        private void GetEUInformation(SerieSettings value)
        {
            OPCUAEntityReference item = null;
            Action action = () =>
            {
                if (!string.IsNullOrEmpty(value.TagreferenceXml))
                {
                    item = value.TagreferenceXml.FromXml<OPCUAEntityReference>();
                }

                if (item != null)
                {
                    if (item.NodeIdViewModel == null && item.IsValid)
                        item.CreateNodeIdViewModel((Document as ScreenDocument).SessionString);
                    value.EUnit = item.NodeIdViewModel?.EUInformation?.DisplayName?.ToString();
                }
            };

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

                action();
            }, TaskScheduler.FromCurrentSynchronizationContext());            
        }

        void InitSettingsForSeriesManagement()
        {
            if (settingStorage == null)
                settingStorage = new SettingsStorage();

            settingStorage.StartTime = settingStorage.DateTimeStart;
            settingStorage.EndTime = settingStorage.DateTimeEnd;

            if (listSeries == null)
                listSeries = new List<string>();

            if (settingStorage.mapSeries == null)
                settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

            if (mapHandlers == null)
                mapHandlers = new Dictionary<string, PenItemHelper>();
        }

        string UpdateSettingSorage(PenItem data)
        {
            var name = data.Name;
            if (!String.IsNullOrEmpty(data.title))
                name = data.title;
            if (String.IsNullOrEmpty(name))
                return null;

            if (string.IsNullOrEmpty(data.guiId))
                data.guiId = Guid.NewGuid().ToString();

            if (!settingStorage.mapSeries.ContainsKey(name))
            {
                string _LinkedPenName = data.Name;
                _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(_LinkedPenName, stringlist, _LinkedPenName);
                settingStorage.mapSeries.Add(name, new SerieSettings()
                {
                    TagName = data.TagName,
                    DName = _LinkedPenName,
                    NodeID = data.NodeId,
                    TagreferenceXml = data.TagReferenceXml,
                    SGuid = data.guiId,
                    DLRSorce = false,
                    UseTableAggregation = false,
                    MinAggregation = false,
                    MaxAggregation = false,
                    AvgAggregation = false,
                    ShowAxis = data.ShowAxis,
                    AuthomaticScale = data.AutoScale,
                    IsVisible = data.Visible,
                    IsSet = true,
                    Min = data.Range.Low,
                    Max = data.Range.High,
                    LogarithmicBaseYScale = 10.0,
                    IsStatisticEnabled = false,
                    Name = name,
                    Color = data.LColor,
                    thickness = data.StrokeThickness,
                    serieTypeLine = data.PenStyle.ToString(),
                    AbsoluteMax = data.Range.Low,
                    AbsoluteMin = data.Range.High,
                    UseEUMinMax = data.UseEURange,
                    arrayindex = data.ArrayIndex,
                    AddVirtualPoints = true,
                    SelectedPenThickness = SelectedPenThickness,
                    pointPrecision = data.PointPrecision,
                    penUnitConverter = data.PenUnitConverter
                    
                });
            }
            else if (settingStorage.mapSeries.ContainsKey(name))
            {
                settingStorage.mapSeries[name].TagName = data.TagName;
                settingStorage.mapSeries[name].SGuid = data.guiId;
                settingStorage.mapSeries[name].TagreferenceXml = data.TagReferenceXml;
                settingStorage.mapSeries[name].NodeID = data.NodeId;
                settingStorage.mapSeries[name].UseSourceTimeStamp = false;
                settingStorage.mapSeries[name].LocalizeSourceTimeStamp = false;
                settingStorage.mapSeries[name].Min = data.Range.Low;
                settingStorage.mapSeries[name].Max = data.Range.High;
                settingStorage.mapSeries[name].AbsoluteMax = data.Range.High;
                settingStorage.mapSeries[name].AbsoluteMin = data.Range.Low;
                settingStorage.mapSeries[name].penUnitConverter = data.PenUnitConverter;
            }
            return name;
        }

        private void SetSerieDataSources(string serie, List<Exception> exceptions, string aggTablePostFiss, bool bFromSettings = false)
        {
        }

        Series AddChartLine(String Original, String Name, String type, int thickness, Color color, bool showaxis, string key, object dataSource = null, bool isvisible = true, int? pointPrecision = -1)
        {

            var lsFound = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).FirstOrDefault();
            if (lsFound != null)
            {
                AddSerieYScale(Original, color, key, lsFound, showaxis, isvisible);
                return lsFound;
            }

            var serie = TryFindResource(type) as Series;
            if (serie == null)
                return null;
            serie.DisplayName = Name;

            serie.BeginInit();
            serie.ArgumentScaleType = ScaleType.DateTime;
            serie.ArgumentDataMember = "SourceTimestamp";
            serie.ValueDataMember = "dValueConverted";
            serie.ValueScaleType = ScaleType.Numerical;
            serie.Label.TextPattern = "{V:F3}";
            if (serie is XYSeries2D)
            {
                var precision = pointPrecision != null && pointPrecision != -1 ? pointPrecision : PointPrecision;
                var pattern = String.Format("{{S}}\n{{V:F{0}}}\n{{A:{1}}}", precision, GetDateTimeFormat()); 
                (serie as XYSeries2D).CrosshairLabelPattern = pattern;
            }

            serie.DataSource = null;
            serie.DataSource = dataSource;

            AddSerieYScale(Original, color, key, serie, showaxis, isvisible);

            diagram.Series.Add(serie);
            CheckAndAdaptAxisRange();

            var xySerie = serie as XYSeries;
            if (xySerie != null)
                xySerie.Brush = isvisible ? new SolidColorBrush(color) : Brushes.Transparent;

            if (thickness > 0)
            {
                if (serie is PointSeries2D)
                {
                    (serie as PointSeries2D).MarkerSize = thickness * 10;
                }
                else if(serie is LineSeries2D)
                {
                    (serie as LineSeries2D).LineStyle = new LineStyle(thickness);
                }
                else if (serie is AreaSeries2D)
                {
                    (serie as AreaSeries2D).Border = new SeriesBorder();
                    (serie as AreaSeries2D).Border.Brush = new SolidColorBrush(color);
                    (serie as AreaSeries2D).Border.LineStyle = new LineStyle(thickness);
                }
                else if (serie is BarSideBySideSeries2D)
                {
                    (serie as BarSideBySideSeries2D).BarWidth = thickness * 10;
                }
            }
            serie.Visible = isvisible;
            serie.EndInit();
            return serie;
        }

        void AddSerieYScale(String Original, Color color, string key, Series serie, bool showaxis, bool isvisible)
        {
            try
            {
                string sanitized = GetSanitizedName(Original);
                var saFound = (from c in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                if (saFound == null)
                {
                    saFound = TryFindResource("secondaryaxisY") as SecondaryAxisY2D;
                    saFound.Logarithmic = false;
                    saFound.Name = sanitized;
                    saFound.Brush = new SolidColorBrush(color);
                    saFound.TickmarksCrossAxis = true;

                    SetAxisFontSettings(YAxsisFontSettings, saFound);

                    if (key != null)
                    {
                        currentValues.Set(key, 0);
                        var bind = new Binding() { Source = currentValues, Path = new PropertyPath("MapValues"), Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                        var conv = new DictionaryValueConverter();
                        bind.Converter = conv;
                        bind.ConverterParameter = key;
                        var cal = new CustomAxisLabel(currentValues.Get(key), currentValues.Get(key).ToString());
                        cal.SetBinding(CustomAxisLabel.ValueProperty, bind);
                        var txt = new TextBlock() { Padding = new Thickness(2), TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap };
                        txt.SetBinding(ForegroundProperty, new Binding() { Source = CurrentValueLabelForeground, Mode = BindingMode.OneWay });
                        txt.SetBinding(TextBlock.WidthProperty, new Binding() { Source = CurrentValueLabelWidth, Mode = BindingMode.OneWay });
                        txt.SetBinding(TextBlock.TextProperty, bind);
                        var brd = new Border() { CornerRadius = new CornerRadius(5) };
                        brd.SetBinding(BackgroundProperty, new Binding() { Source = CurrentValueLabelBackground, Mode = BindingMode.OneWay });
                        brd.Child = txt;
                        cal.Content = brd;
                        saFound.CustomLabels.Add(cal);
                        saFound.LabelVisibilityMode = AxisLabelVisibilityMode.AutoGeneratedAndCustom;
                    }

                    ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.Add(saFound);
                }

                if (saFound != null)
                {
                    saFound.Brush = new SolidColorBrush(color);
                    if (key == monitoredKey && linkedPenSettings != null)
                        SetAxisTitle(linkedPenSettings, saFound);
                    else if (settingStorage != null && settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(Original))
                        SetAxisTitle(settingStorage.mapSeries[Original], saFound);
                    if (!AutomaticGeneralScale)
                        XYDiagram2D.SetSeriesAxisY((XYSeries)serie, saFound);
                    saFound.Visible = AutomaticGeneralScale ? false : isvisible && showaxis;
                }
            }
            catch (Exception ex)
            {
            }
        }

        void SetAxisTitle(SerieSettings serieSettings, SecondaryAxisY2D saFound)
        {
            var bnd = new Binding() { Source = serieSettings, Path = new PropertyPath("EUnit"), Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            var title = new AxisTitle() { Alignment = TitleAlignment.Center, ContentTemplate = (DataTemplate)Resources["AxisTitleTemplate"] };
            title.SetBinding(AxisTitle.ContentProperty, bnd);
            saFound.Title = title;
        }

        void SetChartLineDataSource(String Name, object dataSource, bool bCompare = false)
        {
            var original = Name;
            var ls = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).ToList();
            if (ls.Count == 0)
            {
                var serie = AddChartLine(original, Name, settingStorage.mapSeries[original].serieTypeLine, settingStorage.mapSeries[original].thickness, settingStorage.mapSeries[original].Color, settingStorage.mapSeries[original].ShowAxis, String.Format("{0}_{1}", settingStorage.mapSeries[original].SGuid, settingStorage.mapSeries[original].arrayindex), dataSource, isvisible: settingStorage.mapSeries[original].IsVisible);
                settingStorage.mapSeries[original].lineSerie = serie;
                settingStorage.mapSeries[original].lineSerieMin = null;
                settingStorage.mapSeries[original].lineSerieMax = null;
                settingStorage.mapSeries[original].lineSerieAvg = null;

                serie.Visible = settingStorage.mapSeries[original].IsVisible;
                if (serie.Points != null && serie.Points.Count > MaxResolveOverlappingPoints)
                    (serie as Series).Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
            }
            else
            {
                ls[0].BeginInit();
                if (IsInStop)
                {
                    ls[0].DataSource = null;
                    ls[0].DataSource = dataSource;
                }
                ls[0].Visible = settingStorage.mapSeries[original].IsVisible;
                if (ls[0].Points != null)
                {
                    if (ls[0].Points.Count > MaxResolveOverlappingPoints)
                        (ls[0] as Series).Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
                    (ls[0] as Series).LabelsVisibility = false;
                }
                ls[0].EndInit();

            }

            CheckAndAdaptAxisRange();

        }

        void UpdateAxisRange()
        {
            UpdateYAxisRange();
            UpdateXAxisRange();
        }

        void UpdateYAxisRange()
        {
            axisToUpdateGridSpacing.Clear();
            axisToUpdateSideMargins.Clear();            

            SerieSettings selectedItem = legend_GridControl.SelectedItem as SerieSettings;

            (from axis in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select axis).ToList().ForEach(x =>
            {                
                if (!bDesignmode)
                {
                    var serie = linkedPenSettings != null && x.Name == GetSanitizedName(linkedPenSettings.Name) /*&& !linkedPenSettings.AuthomaticScale*/ ? linkedPenSettings :
                    settingStorage.mapSeries != null ? (from s in settingStorage.mapSeries.Values where /*!s.AuthomaticScale &&*/ (x.Name == GetSanitizedName(s.Name)) select s).FirstOrDefault() : null;
                    if (serie != null)
                    {
                        if (AutomaticGeneralScale)
                        {
                            x.ActualWholeRange.SetAuto();
                            x.ActualVisualRange.SetAuto();

                            XYDiagram2D.SetSeriesAxisY((XYSeries)serie.lineSerie, null);
                            x.Visible = false;
                        }
                        else
                        {
                            if (!serie.AuthomaticScale)
                            {
                                if (serie.expConverter != null)
                                {
                                    var min = serie.expConverter.Convert(serie.Min, typeof(double), null, CultureInfo.InvariantCulture);
                                    var max = serie.expConverter.Convert(serie.Max, typeof(double), null, CultureInfo.InvariantCulture);
                                    x.ActualWholeRange.MinValue = min;
                                    x.ActualWholeRange.MaxValue = max;
                                }
                                else
                                {
                                    x.ActualWholeRange.MinValue = serie.Min;
                                    x.ActualWholeRange.MaxValue = serie.Max;
                                }
                            }
                            else if (!AutomaticGeneralScale && !x.Logarithmic)
                                axisToUpdateSideMargins.Add(x);

                            XYDiagram2D.SetSeriesAxisY((XYSeries)serie.lineSerie, x);
                            if (serie.ShowAxis && serie.IsVisible)
                                (x as SecondaryAxisY2D).Visible = true;
                            axisToUpdateGridSpacing.Add(x);
                        }
                    }
                }
                else
                {
                    var serie = (from s in PenList where /*!s.AutoScale &&*/ (x.Name == GetSanitizedName(s.title)) select s).FirstOrDefault();
                    if (serie != null)
                    {
                        var name = serie.Name;
                        if (!String.IsNullOrEmpty(serie.title))
                            name = serie.title;
                        if (string.IsNullOrEmpty(name))
                            name = String.Format("Value{0}", PenList.IndexOf(serie));
                        var lineSerie = (from c in diagram.Series.OfType<Series>() where c.DisplayName == name select c).FirstOrDefault();
                        if (AutomaticGeneralScale)
                        {
                            x.ActualWholeRange.SetAuto();
                            x.ActualVisualRange.SetAuto();

                            XYDiagram2D.SetSeriesAxisY((XYSeries)lineSerie, null);
                            x.Visible = false;
                        }
                        else
                        {
                            if (!serie.AutoScale)
                            {
                                x.ActualWholeRange.MinValue = serie.Range.Low;
                                x.ActualWholeRange.MaxValue = serie.Range.High;
                            }
                            else if (!AutomaticGeneralScale && !x.Logarithmic)
                                axisToUpdateSideMargins.Add(x);

                            XYDiagram2D.SetSeriesAxisY((XYSeries)lineSerie, x);
                            if (serie.ShowAxis && serie.Visible)
                                (x as SecondaryAxisY2D).Visible = true;
                            axisToUpdateGridSpacing.Add(x);
                        }
                    }
                }
            });

            if (AutomaticGeneralScale)
            {
                axisY.ActualWholeRange.SetAuto();
                axisY.ActualVisualRange.SetAuto();

                axisY.Visible = true;
                (axisY.NumericScaleOptions as ContinuousNumericScaleOptions).AutoGrid = YAutoGrid;
                if (!LogarithmicYScale)
                {
                    axisToUpdateGridSpacing.Add(axisY);
                    axisToUpdateSideMargins.Add(axisY);
                }
            }
            else
            {
                axisY.Visible = false;
                (axisY.NumericScaleOptions as ContinuousNumericScaleOptions).AutoGrid = true;
            }

            UpdateYMinorCount();
            sfchart.UpdateData();
        }

        void UpdateXAxisRange()
        {
            SetTimeScaleAlignment(settingStorage.DateTimeStart, settingStorage.DateTimeEnd);
            UpdateXAxisSpacing();
            UpdateXMinorCount();
        }

        void UpdateXAxisSpacing()
        {
            double gridSpacing = 0.0;
            if (axisX.ActualWholeRange.ActualMaxValue is DateTime && axisX.ActualWholeRange.ActualMinValue is DateTime)
            {
                var delta = (DateTime)axisX.ActualWholeRange.ActualMaxValue - (DateTime)axisX.ActualWholeRange.ActualMinValue;
                double division = Math.Max(1.0, MajorXTicks);

                DateTimeGridAlignment dtga;
                if (axisX.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                    dtga = (axisX.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridAlignment;
                else
                    dtga = (axisX.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridAlignment;

                if (dtga == DateTimeGridAlignment.Millisecond)
                    gridSpacing = delta.TotalMilliseconds / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Second)
                    gridSpacing = delta.TotalSeconds / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Minute)
                    gridSpacing = delta.TotalMinutes / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Hour)
                    gridSpacing = delta.TotalHours / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Day)
                    gridSpacing = delta.TotalDays / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Week)
                    gridSpacing = (delta.TotalDays / daysPerWeek) / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Month)
                    gridSpacing = (delta.TotalDays / daysPerMonth) / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Quarter)
                    gridSpacing = (delta.TotalDays / (daysPerYears / 4.0)) / MajorXTicks;
                else if (dtga == DateTimeGridAlignment.Year)
                    gridSpacing = (delta.TotalDays / daysPerYears) / MajorXTicks;
            }

            if (axisX.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                (axisX.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
            else
                (axisX.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
        }

        string GetSanitizedName(string name)
        {
            string axisname = string.Format("{0}{1}", name, Properties.Settings.Default.SecondaryYAxis);
            string pattern = "[ \\[\\]\\+°\\~#%&@$£'.!*{})(/:<>?|\"-,]";
            string replacement = "_";

            Regex regEx = new Regex(pattern);
            return Regex.Replace(regEx.Replace(axisname, replacement), @"\s+", "_");
        }
        void BuildLineFromValues(String serie, List<MyDataValue> list)
        {
            if (list == null)
                return;

            SetChartLineDataSource(serie, list);

            if (list.Count > 0)
            {
                if (!bIsFetching)
                {
                    var firstTimestamp = list.First().SourceTimestamp != maxDateTimeValue ? list.First().SourceTimestamp : list[1].SourceTimestamp;
                    var lastTimestamp = list.Last().SourceTimestamp != minDateTimeValue ? list.Last().SourceTimestamp : list[list.Count - 2].SourceTimestamp;

                    bool bOnlyOnePoint = list.Count == 1 || (list.Count == 3 && list.First().bIsFakePoint && list.Last().bIsFakePoint);
                    if (bOnlyOnePoint)
                    {
                        var vt = new TimeSpan(ViewTimeFrame.Ticks / 2);
                        firstTimestamp = firstTimestamp - vt;
                        lastTimestamp = lastTimestamp + vt;
                    }

                    if (firstTimestamp < settingStorage.DateTimeEnd)
                    {
                        settingStorage.DateTimeEnd = settingStorage.EndTime = firstTimestamp;
                    }
                    if (lastTimestamp > settingStorage.DateTimeStart)
                    {
                        settingStorage.DateTimeStart = settingStorage.StartTime = lastTimestamp;
                    }
                }
            }
            else
                System.Diagnostics.Debug.WriteLine(String.Format("No data for serie {0}", serie));
        }

        private DateTime Floor(DateTime dateTime, TimeSpan interval)
        {
            try
            {
                return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
            }
            catch (Exception)
            {
                return dateTime;
            }
        }

        private DateTime Ceiling(DateTime dateTime, TimeSpan interval)
        {
            var overflow = dateTime.Ticks % interval.Ticks;
            try
            {
                return overflow == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - overflow);
            }
            catch (Exception)
            {
                return Floor(dateTime, interval);
            }
        }

        void SetTimeScaleAlignment(DateTime start, DateTime end)
        {
            axisX.ActualWholeRange.SetAuto();
            axisX.ActualVisualRange.SetAuto();

            var datetimeDiff = end - start;
            var dateMargin = datetimeDiff.Ticks * AllValuesXMargin / 100;

            DateTime minValue;
            DateTime maxValue;

            if (datetimeDiff.TotalSeconds <= 1)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Millisecond, DateTimeMeasureUnit.Millisecond);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalSeconds <= secondsPerMinute)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Second, DateTimeMeasureUnit.Millisecond);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalMinutes <= minutesPerHour)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Second, DateTimeMeasureUnit.Second);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalHours <= hoursPerDay)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Minute, DateTimeMeasureUnit.Minute);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerWeek)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerMonth)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerYears * 10)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Day, DateTimeMeasureUnit.Day);
                minValue = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Year, DateTimeMeasureUnit.Week);
                minValue = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, 1, 0, 0, 0);
            }

            axisX.ActualWholeRange.MinValue = minValue;
            axisX.ActualWholeRange.MaxValue = maxValue;
        }

        void SetDateTimeScaleOptions(DateTimeGridAlignment ga, DateTimeMeasureUnit mu)
        {
            DateTimeScaleOptionsBase dtso = axisX.DateTimeScaleOptions;
            if (dtso is ContinuousDateTimeScaleOptions)
            {
                (dtso as ContinuousDateTimeScaleOptions).GridAlignment = ga;
            }
            else
            {
                if (bDiscreteTimeMode)
                {
                    (dtso as ManualDateTimeScaleOptions).GridAlignment = DateTimeGridAlignment.Second;
                    (dtso as ManualDateTimeScaleOptions).MeasureUnit = DateTimeMeasureUnit.Second;
                }
                else
                {
                    (dtso as ManualDateTimeScaleOptions).GridAlignment = ga;
                    (dtso as ManualDateTimeScaleOptions).MeasureUnit = mu;
                    SetDateTimeScaleMeasureUnitMultiplier(MeasureUnitMultiplier);
                }
            }
        }

        void SetDateTimeScaleMeasureUnitMultiplier(int measureUnitMultiplier)
        {
            DateTimeScaleOptionsBase dtso = axisX.DateTimeScaleOptions;
            if (dtso is ManualDateTimeScaleOptions)
                (dtso as ManualDateTimeScaleOptions).MeasureUnitMultiplier = measureUnitMultiplier;
        }

        void CheckAndAdaptAxisRange()
        {
            if (settingStorage.mapSeries == null)
                return;
            var list = (from c in settingStorage.mapSeries.Keys
                        where settingStorage.mapSeries[c].minValue == 0
                        select c).ToList();
            if (list.Count == 0 || !(axisY.WholeRange.MinValue is double))
                return;

            var min = (double)axisY.WholeRange.MinValue;
            if (min != 0)
                return;

            axisY.WholeRange.MinValue = -1;
        }
        object seriesObject = new object();
        TaskScheduler sc;
        bool SetDataSources(bool bFromSettings = false)
        {
            return true;
        }
        Dictionary<string, DataValue> condValues = new Dictionary<string, DataValue>();

        bool IsStatusGood(string key)
        {
            return OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key] != null && 
                    OpcuaEntityReference[key].MonitoredItemViewModel != null &&
                    OpcuaEntityReference[key].MonitoredItemViewModel.DataValue != null &&
                    Opc.Ua.StatusCode.IsGood(OpcuaEntityReference[key].MonitoredItemViewModel.DataValue.StatusCode);
        }

        bool bSettingLegendSource;
        void SetLegendSource()
        {
            if (settingStorage == null)
                return;

            var source = settingStorage.mapSeries.Values.ToList();
            if (linkedPenSettings != null)
                source.Add(linkedPenSettings);
            bSettingLegendSource = true;
            legend_GridControl.ItemsSource = null;
            legend_GridControl.ItemsSource = source;
            bSettingLegendSource = false;
            //legend_GridControl.SelectedItem = null;
        }

        CancellationTokenSource tokenSource;
        CancellationToken ct;

        Storyboard story;
        private void ShowError(Exception exception, string errorMsg = null)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose)
                    return;

                var msg = errorMsg != null ? errorMsg : (exception.InnerException != null ? exception.InnerException.Message : exception.Message);
                var error = string.Format("{0}: {1}", Name, msg);
                logLicense.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.TrendControlLog,
                DateTime.UtcNow, $"{error}: {exception.StackTrace}", System.Diagnostics.EventLogEntryType.Error);

                dlException = null;

                if (Document != null)
                {
                    if (UIMsgBoxAlertService == null)
                        UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (UIMsgBoxAlertService != null)
                        UIMsgBoxAlertService.ShowError(error);
                    else
                        dlException = exception;
                }
                else
                    dlException = exception;

                if(!string.IsNullOrEmpty(dlException?.Message))
                {
                    if (story != null)
                        return;
                    story = errorContent.FindResource("blink") as Storyboard;
                    if (story != null)
                    {
                        errorContent.Visibility = Visibility.Visible;
                        story.Completed += (o, ea) =>
                        {
                            if (bDispose)
                                return;
                            errorContent.Visibility = Visibility.Collapsed;
                            story = null;
                        };
                        story.Begin();
                    }
                }

            });
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SetDataSources();
        }

        private void ShowCrossHair_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInStop)
                return;
            sfchart.CrosshairEnabled = !sfchart.CrosshairEnabled;
        }

        SerieSettings linkedPenSettings;
        private void chart_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {
            if (!bLoaded || bDispose || bDesignmode)
                return;

            if (e.CrosshairElementGroups.Count > 0)
            {
                if (e.CrosshairElementGroups[0].CrosshairElements.First().Series == dataContextSerie)
                    nearestSerie = linkedPenSettings;
                else
                {
                    var serieName = e.CrosshairElementGroups[0].CrosshairElements.First().Series.DisplayName;
                    if (settingStorage.mapSeries.ContainsKey(serieName))
                        nearestSerie = settingStorage.mapSeries[serieName];
                }

            }

            foreach (var group in e.CrosshairElementGroups)
            {
                var elem = group.CrosshairElements.First();
                var serie = elem.Series.DisplayName;
                var point = elem.SeriesPoint;
                if (point.Tag is MyDataValue && (point.Tag as MyDataValue).bIsFakePoint)
                    elem.LabelElement.Visible = false;
                nearestPoints[serie] = point;
            }
        }

        private void chart_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {
            if (bDispose)
                return;
                    
            if (e.Series.Points != null)
            {
                e.Series.LabelsVisibility = false;
                if (e.Series.Points.Count > MaxResolveOverlappingPoints)
                    e.Series.Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
            }

            if(dataContextSerie != null && e.Series == dataContextSerie && linkedPenSettings != null)
            {
                e.DrawOptions.Color = linkedPenSettings.Color;
                e.Handled = true;
            }
            else if (settingStorage != null && settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(e.Series.DisplayName) &&
                settingStorage.mapSeries[e.Series.DisplayName].Color != Colors.Transparent)
            {
                e.DrawOptions.Color = settingStorage.mapSeries[e.Series.DisplayName].Color;
                e.Handled = true;
            }
        }

        string SerieArgumentDateTimeFormat(string argument)
        {
            DateTime argumentDt;
            try
            {
                argumentDt = DateTime.Parse(argument, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return argument;
            }
            return argumentDt.ToString(GetDateTimeFormat());
        }


        private void chart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var hitInfo = sfchart.CalcHitInfo(e.GetPosition(sfchart));
            var bInDiagram = hitInfo != null && hitInfo.InDiagram;
            if (bInDiagram)
            {
                foreach (var serie in diagram.Series)
                {
                    SerieSettings s = null;
                    if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(serie.DisplayName))
                        s = settingStorage.mapSeries[serie.DisplayName];
                    if (s == null)
                        continue;
                    if (nearestPoints.ContainsKey(serie.DisplayName))
                    {
                        s.LastClickedPoint.Date = SerieArgumentDateTimeFormat(nearestPoints[serie.DisplayName].Argument);
                        s.LastClickedPoint.Value = nearestPoints[serie.DisplayName].Value;
                    }
                }
            }
            crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
        }

        private void chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement)
                UpdateFocus(e.Source as FrameworkElement);

            var hitInfo = sfchart.CalcHitInfo(e.GetPosition(sfchart));
            var bInDiagram = hitInfo != null && hitInfo.InDiagram;
            if (bInDiagram)
            {
                crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForEachSeries;
            }

            bool bSerieSelected = false;
            if (hitInfo != null && hitInfo.Series != null)
            {
                string displayName = hitInfo.Series.DisplayName;
                SerieSettings selectedSerie = null;
                if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(displayName))
                    selectedSerie = settingStorage.mapSeries[displayName];
                if (selectedSerie == null)
                {
                    string tag = hitInfo.Series.Tag as String;
                    var name = displayName.Replace(String.Format(" - {0}", tag), "");
                    if (settingStorage.mapSeries.ContainsKey(name))
                    {
                        SetPrintGridDataSource(hitInfo.Series);
                        tagName.Text = hitInfo.Series.DisplayName;
                    }
                }
                else
                {
                    bSerieSelected = true;
                    tagName.Text = hitInfo.Series.DisplayName;
                    legend_GridControl.SelectedItem = legend_GridControl.SelectedItem == selectedSerie ? null : selectedSerie;
                }
                e.Handled = true;
            }

            if (bInDiagram)
            {
                if (!bSerieSelected && (legend_GridControl.ItemsSource as List<SerieSettings>).Contains(nearestSerie)
                && !Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt) && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    if (!nearestSerie.IsHighlighted && legend_GridControl.SelectedItem as SerieSettings != nearestSerie && (legend_GridControl.ItemsSource as List<SerieSettings>).Contains(nearestSerie))
                        legend_GridControl.SelectedItem = nearestSerie;
                    else if (legend_GridControl.SelectedItem == nearestSerie)
                        legend_GridControl.SelectedItem = null;
                }
                var currentSerie = legend_GridControl.SelectedItem as SerieSettings;
                if (currentSerie != null && nearestPoints.ContainsKey(currentSerie.Name))
                    sfchart.SelectedItem = nearestPoints[currentSerie.Name];
            }
        }

        private void ChartDesigner_Click(object sender, RoutedEventArgs e)
        {
            var designer = new ChartDesigner(sfchart);
            designer.Show(this.FindParent<Window>());
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (RunningOnServer)
                return;

            UtilitiesPrintHelper.PrintElement(sfchart as FrameworkElement, this.FindParent<Window>(), true, true, System.Drawing.Printing.PaperKind.A4);
        }

        private void PrintGrid_Click(object sender, RoutedEventArgs e)
        {
            if (RunningOnServer)
                return;

            UtilitiesPrintHelper.PrintControl(this.FindParent<Window>(), (IPrintableControl)gridControl.View, GetStorageName(), $"{Properties.Resources.GridTagName} {tagName.Text}", false, System.Drawing.Printing.PaperKind.A4);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }
        public void RefreshViewTimeFrame()
        {
            if (!bDesignmode && bInit)
            {
                UpdateXAxisRange();
                UpdateYAxisRange();
            }
        }
        #endregion

        #region Editing
        bool bUserInteractionSettings;
        private void GetItem(string itemName)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (setting?.PenList != null)
                {
                    //PenList = new PenItemList(setting.PenList);
                    PenList = new PenItemList(setting?.PenList);
                    ActualConfig = setting.Name;
                    DockLayout = setting.DockLayout;
                    ListViewLayout = setting.ListViewLayout;
                    GridLayout = setting.GridLayout;
                    UseAbsoluteRanges = setting.UseAbsoluteRanges;
                    configMemory.DataContext = MemorySettingList?.Names;
                    configMemory.EditValue = setting.Name;
                }
                else
                {
                    PenList = designPenList != null ? new PenItemList(designPenList) : new PenItemList();
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
                    DockLayout = designDockLayout;
                    ListViewLayout = designListLayout;
                    GridLayout = designGridLayout;
                    UseAbsoluteRanges = setting.UseAbsoluteRanges;
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
            MemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            return (from n in list select n.Name).ToList();
        }

        #region Script Utils Methods
        public PenItem GetPen(string title)
        {
            return PenList == null ? null : (from PenItem pen in PenList where pen.title == title select pen).FirstOrDefault();
        }
        public PenItem GetPen(int index)
        {
            if (PenList == null || PenList.Count <= index)
                return null;
            return PenList[index];
        }
        public bool RemovePen(string title)
        {
            var foundPen = GetPen(title);
            if (foundPen != null)
            {
                PenList.Remove(foundPen);
                return true;
            }
            return false;
        }
        public bool RemovePen(int index)
        {
            var foundPen = GetPen(index);
            if (foundPen != null)
            {
                PenList.Remove(foundPen);
                return true;
            }
            return false;
        }
        public int GetPensNumber()
        {
            return PenList == null ? 0 : PenList.Count;
        }
        #endregion

        /// <summary>
        /// Allow to define a new pen and add it the list of used pens. If pen name is already use or tag does not exist on server, return null.
        /// </summary>
        /// <param name="penName"></param>
        /// <param name="variableName"></param>
        /// <param name="useDatalogger"></param>
        /// <param name="penColor"></param>
        /// <param name="historicalName"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="automaticScale"></param>
        /// <param name="plotType"></param>
        /// <returns></returns>
        public PenItem AddPen(string penName, string variableName, Color penColor, double min = 0, double max = 100, bool automaticScale = true, int plotType = 0)
        {
            if (!Enum.IsDefined(typeof(PredefinedPenKinds), plotType))
                plotType = 0;

            var newData = new PenItem
            {
                Name = penName,
                LColor = penColor,
                PenStyle = (PredefinedPenKinds)plotType,
                AutoScale = automaticScale,
                Range = new Opc.Ua.Range(min, max),
                Visible = true,
                title = penName
            };

            if (GetPen(penName) != null || !newData.SetTag(Document, variableName))
                return null;

            PenList.Add(newData);
            return newData;
        }

        public void ReloadPens(PenItemList penlist = null)
        {
            if (!bLoaded || bDispose)
                return;

            if (penlist == null)
                penlist = PenList;

            bIsRestarting = true;
            EnableToolbars(false);
            Clear();
            PenList = penlist != null ? new PenItemList(penlist) : new PenItemList();
            RestoreChartFromSettings();

            if (!IsInStop)
                PlayExecute();
        }

        private void Clear()
        {
            diagram.BeginInit();
            if (settingStorage == null)
                settingStorage = new SettingsStorage();
            if (settingStorage.ListRanges == null)
                settingStorage.ListRanges = new List<TimeRange>();
            if (listSeries == null)
                listSeries = new List<String>();

            listSeries.Clear();
            settingStorage.ListRanges.Clear();
            settingStorage.mapSeries.Clear();

            diagram.Series.Clear();
            diagram.EndInit();
            diagram.Refresh();
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
            if (PenList == null)
                return ret;
            foreach (var pen in PenList)
            {
                if (string.IsNullOrEmpty(pen.guiId))
                    pen.guiId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(pen.TagReferenceXml))
                    ret.Add(pen.CreateUniqueName(pen.title, ret.Keys.ToList()), pen.TagReferenceXml);
            }
            return ret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute) || PenList == null)
                return false;
            bool ret = false;

            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
            PenItemList penList = new PenItemList();
            var opcInit = OpcuaEntityReference;
            penList.AddRange(PenList.ToList());
            var penTagList = (from pen in penList where pen.TagReference != null select pen).ToList();
            int matchCount = penTagList.Count;

            foreach (var pen in penTagList)
            {
                MatchTag(pen, relative, _relative, _absolute);
            }

            if (!bDesignmode)
                ret = matchChangedMap.Count == matchCount;

            if (!bDesignmode && ret)
                PenList = penList;
            return ret;
        }

        void MatchTag(PenItem pen, String relative, OPCUAEntityReference _relative, OPCUAEntityReference _absolute)
        {
            try
            {
                string key = $"{pen.guiId}";
                bool updateTag = false;
                if (pen.TagReferenceXml != null && relative == pen.TagReferenceXml)
                {
                    matchChangedMap.Add(key);
                    TerminateExecution(key);
                    updateTag = true;
                }

                if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                {
                    _relative.Merge(_absolute);
                    if (updateTag)
                    {
                        pen.TagReference = _relative;
                        opcuaEntityReference[key] = _relative;
                    }
                }
                else
                {
                    if (updateTag)
                    {
                        pen.TagReference = _absolute;
                        opcuaEntityReference[key] = _absolute;
                    }
                }

                if (updateTag)
                {
                    if(pen.TagReference.HasValidValue)
                    {
                        var tagPath = $"{pen.TagReference.StartingAddress}/{pen.TagReference.RelativePath}";
                        if (tagPath.StartsWith("/") && tagPath.Length > 1)
                            tagPath = tagPath.Substring(1);
                        pen.NodeId = pen.TagReference.NodeIdViewModel?.nodeId.ToString() ?? pen.TagReference.ResolvedNodeId?.ToString();
                    }
                    PrepareExecution(key, pen.ArrayIndex);
                }

            }
            catch (Exception)
            {
            }
        }

        internal string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                //string oldChars = string.Format("{0}:Tags", ns);
                //string relative = string.Format("{0}", (value).Replace(oldChars, ""));
                string oldChars = string.Format("{0}:", ns);
                string relative = string.Format("{0}", (value).Replace(oldChars, ""));
                return relative; // string.Format("Tags/{0}", relative);
            }

            return String.Empty;
        }

        //Object lockObj = new object();
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

                    if (condValues.ContainsKey(key))
                        condValues.Remove(key);

                    if (mapHandlers.ContainsKey(key))
                    {
                        typeHelper.TerminateExecution(this, mapHandlers[key].opcuaEntityReference_PropertyChanged, 
                            mapHandlers[key].monitoredItemViewModel_PropertyChanged, OpcuaEntityReference[key],
                            OpcuaEntityReference[key].MonitoredItemViewModel);
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
        Dictionary<string, PenItemHelper> mapHandlers;
        private void PrepareExecution(string key, int arrayindex = -1)
        {
            if (bDispose)
                return;

            //lock (lockObj)
            {
                if (OpcuaEntityReference == null || !OpcuaEntityReference.ContainsKey(key))
                    return;
                try
                {
                    if (mapHandlers == null)
                        mapHandlers = new Dictionary<string, PenItemHelper>();

                    if (OpcuaEntityReference[key] == null || !OpcuaEntityReference[key].IsValid)// || (OpcuaEntityReference[key].ResolvedNodeId == null && string.IsNullOrEmpty(OpcuaEntityReference[key].RelativePath)))
                        return;

                    if (mapHandlers.ContainsKey(key))
                        TerminateExecution(key);
                    mapHandlers[key] = new PenItemHelper(key, arrayindex); //new PenItemHelper(arrayindex, true)
                    //{
                    //    control = this,
                    //    Key = key
                    //};


                    mapHandlers[key].Error += PenItem_OnError;
                    mapHandlers[key].ModelChanged += PenItem_ModelChanged;
                    mapHandlers[key].ValueChanged += PenItem_ValueChanged;
                    mapHandlers[key].MinMaxRangeChanged += PenItem_MinMaxRangeChanged;

                    typeHelper.PrepareExecution(Properties.Resources.SessionName, Document as ScreenDocument, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
                }
                catch (Exception)
                {
                }
            }
        }

        #region PenItemHelper Event Handlers
        private void PenItem_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.ErrorMessage))
                ShowError(null, e.ErrorMessage);
        }

        private void ViewList_OnError(object sender, Utilities.ErrorEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.ErrorMessage))
                ShowError(null, e.ErrorMessage);
        }

        private void PenItem_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;

            if (!e.Model.IsUserReadable || !e.Model.IsReadable)
                return;

            //var key = string.Format("{0}_{1}", helper.Key, helper.arrayIndex);
            UpdateModel(helper.Key, helper.ArrayIndex, e.HumanReadable, e.Model);
        }

        private void PenItem_MinMaxRangeChanged(object sender, MinMaxRangeChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;
            var key = string.Format("{0}_{1}", helper.Key, helper.ArrayIndex);
            if (!mapKeySeries.ContainsKey(key) || !mapKeySeries[key].UseEUMinMax)
                return;

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose || !mapKeySeries.ContainsKey(key))
                    return;

                mapKeySeries[key].AbsoluteMin = e.MinValue;
                mapKeySeries[key].AbsoluteMax = e.MaxValue;
                UpdateYAxisRange();
            });
        }

        private void PenItem_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var helper = (PenItemHelper)sender;

            var key = string.Format("{0}_{1}", helper.Key, helper.ArrayIndex);
            UpdateMonitoredValue(key, e.NewValue);
        }
        #endregion
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            PenItemList penList = new PenItemList();
            if (PenList != null)
                penList.AddRange(PenList.ToList());
            foreach (var pen in penList)
            {
                if (string.IsNullOrEmpty(pen.guiId))
                    pen.guiId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(pen.TagReferenceXml))
                {
                    if (map.ContainsKey(pen.guiId))
                        pen.TagReferenceXml = map[pen.guiId];
                    else
                        pen.TagReferenceXml = typeHelper.UpdateTag(pen.TagReferenceXml, map);
                    if (pen.TagReference.HasValidValue)
                    {
                        pen.TagName = GetRelativePath($"{pen.TagReference.RelativePath}");
                        try
                        {
                            if (!pen.TagReference.IsRelative)
                                pen.NodeId = pen.TagReference.ResolvedNodeId.ToString();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            PenList = penList;
        }
        internal void UpdateReferences(string key, string noideid)
        {
            var serie = (from s in PenList where s.guiId == key select s).FirstOrDefault();
            if (serie != null)
            {
                serie.NodeId = noideid;
                InitSettingsForSeriesManagement();
                UpdateSettingSorage(serie);
                if (sc == null)
                    sc = TaskScheduler.FromCurrentSynchronizationContext();
                var exceptions = new List<Exception>();
                if (tokenSource == null)
                {
                    tokenSource = new CancellationTokenSource();
                    ct = tokenSource.Token;
                }
                var name = serie.TagName;
                if (!String.IsNullOrEmpty(serie.title))
                    name = serie.title;
                if (String.IsNullOrEmpty(name))
                    return;

                SetLegendSource();
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
        {
            UnitConverterSystem = converterSystem;

            if (bLoaded && mapKeySeries != null && mapKeySeries.Any(x => x.Value.penUnitConverter != null))
            {
                foreach (var setting in mapKeySeries)
                {
                    UpdateExpressionValueConverter(setting.Value, setting.Key);
                    var dataList = viewList.FirstOrDefault(x => x.Key == setting.Key);
                    dataList.Value?.UpdateUnitConverter(setting.Value.expConverter);
                    UpdateScaleForUnitConverter(GetSanitizedName(setting.Value.Name), setting.Value.expConverter);
                }
                SetRealTimeSource();
            }
        }

        private void UpdateScaleForUnitConverter(string mapAxisKey, IExpressionValueConverter expConverter)
        {
            if (!bDesignmode && !AutomaticGeneralScale)
            {
                var penAxesY = ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>().FirstOrDefault(ax => ax.Name == mapAxisKey);

                var serie = linkedPenSettings != null && mapAxisKey == GetSanitizedName(linkedPenSettings.Name) ? linkedPenSettings : settingStorage.mapSeries != null ?
                        (from s in settingStorage.mapSeries.Values where (mapAxisKey == GetSanitizedName(s.Name)) select s).FirstOrDefault() : null;

                if (serie != null)
                {
                    if (!serie.AuthomaticScale)
                    {
                        if (expConverter != null)
                        {
                            var min = expConverter.Convert(serie.Min, typeof(double), null, CultureInfo.InvariantCulture);
                            var max = expConverter.Convert(serie.Max, typeof(double), null, CultureInfo.InvariantCulture);
                            penAxesY.ActualWholeRange.MinValue = min;
                            penAxesY.ActualWholeRange.MaxValue = max;
                        }
                        else
                        {
                            penAxesY.ActualWholeRange.MinValue = serie.Min;
                            penAxesY.ActualWholeRange.MaxValue = serie.Max;
                        }
                    }
                }
            }
        }

        #endregion

        #region Pen Unit Converter

        public string UnitConverterSystem;

        private IExpressionValueConverter GetExpressionValueConverter(string penUnitConverter, string penName)
        {
            var inputExp = ConverterEditorManager?.GetInputExpression(Document, penUnitConverter, UnitConverterSystem);
            if (inputExp == null)
                return null;

            var converter = new ExpressionValueConverter(inputExp);
            converter.ThrowExceptions = true;
            converter.ParseFormula();
            var error = converter.GetParserError();
            if (!String.IsNullOrEmpty(error))
            {
                var msgError = String.Format(Properties.Resources.ErrorParseUnitConverter, error, penName);
                ShowError(new Exception(error), msgError);
                converter.Dispose();
                return null;
            }
            return converter;
        }

        private void UpdateExpressionValueConverter(SerieSettings serie, string penName)
        {
            if (serie == null)
                return;

            serie.UnitConverterLabel = ConverterEditorManager?.GetUnitLabel(Document, serie.penUnitConverter, UnitConverterSystem);

            var inputExp = ConverterEditorManager?.GetInputExpression(Document, serie.penUnitConverter, UnitConverterSystem);
            if (inputExp == null)
                return;

            if (serie.expConverter == null)
                return;

            if (serie.expConverter.Formula != inputExp)
            {
                serie.expConverter.Formula = inputExp;
                serie.expConverter.ThrowExceptions = true;
                serie.expConverter.ParseFormula();
                var error = serie.expConverter.GetParserError();
                if (!String.IsNullOrEmpty(error))
                {
                    var msgError = String.Format(Properties.Resources.ErrorParseUnitConverter, error, penName);
                    ShowError(new Exception(error), msgError);
                    serie.expConverter.Dispose();
                    return;
                }
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

        #region Zoom Management
        string oldPostfiss;
        bool isZoomingOnMouseWheel;
        bool bIsZooming;
        List<DevExpress.Xpf.Charts.Range> zommingRangeList;
        private void diagram_Zoom(object sender, XYDiagram2DZoomEventArgs e)
        {
            if (!(e.OldXRange.MinValue is DateTime) || !(e.OldXRange.MaxValue is DateTime) ||
                !(e.NewXRange.MinValue is DateTime) || !(e.NewXRange.MaxValue is DateTime))
                return;

            bIsZooming = true;


            if (!IsInStop)
                Pause(this, null);

            if (zommingRangeList == null)
                zommingRangeList = new List<DevExpress.Xpf.Charts.Range>();

            if (isZoomingOnMouseWheel)
                zommingRangeList.Add(new DevExpress.Xpf.Charts.Range() { MaxValue = e.OldXRange.MaxValue, MinValue = e.OldXRange.MinValue });

            isZoomingOnMouseWheel = false;
        }
       
        private void diagram_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                isZoomingOnMouseWheel = true;
                diagram.Series.ForEach(s => s.Label.ResolveOverlappingMode = ResolveOverlappingMode.None);
            }
            else if (e.Delta < 0)
            {
                if (diagram.CanZoomOut())
                {
                    if (zommingRangeList == null)
                        zommingRangeList = new List<DevExpress.Xpf.Charts.Range>();
                    if (zommingRangeList.Count > 0)
                    {
                        var rangeInfo = zommingRangeList.Last();
                        if (rangeInfo != null)
                        {
                            axisX.VisualRange = rangeInfo;
                            zommingRangeList.Remove(rangeInfo);
                            e.Handled = true;
                        }
                    }
                }
                else
                {
                    if (zommingRangeList != null && zommingRangeList.Count > 0)
                    {
                        var rangeInfo = zommingRangeList.Last();
                        if (rangeInfo != null)
                        {
                            zommingRangeList.Remove(rangeInfo);
                        }
                    }
                }
            }
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
                ChangeSetting(editValue);
        }
        void ChangeSetting(string settingName)
        {
            if (String.IsNullOrEmpty(settingName) || ActualConfig == settingName || bCallingRemoveCommand || bCallingSaveCommand || !bInit || bIsInEditMode)
                return;

            using (var cursor = new WaitCursor())
            {
                ActualConfig = settingName;
                SaveRuntimeLayout(GetStorageName(true));
                GetItem(ActualConfig);
                ReloadPens();
            }
            return;
        }
        RelayCommand _editCommand;
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
        internal void CallEditCommand()
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
                ReloadPens();
            }

            bCallingEditCommand = false;
        }

        RelayCommand _saveCommand;
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
            string defaultTagSetting = Properties.Settings.Default.DesignSettingName;

            if (configname == defaultTagSetting)
                return;

            SaveDesignGridLayout();
            SaveDesignListViewLayout();
            SaveDesignDockLayout();

            var selected = (from m in MemorySettingList
                            where m.Name == configname
                            select m).FirstOrDefault();
            if (selected == null)
            {
                MemorySettingList.Add(new Setting()
                {
                    Name = configname,
                    PenList = PenList != null ? new PenItemList(PenList) : new PenItemList(),
                    GridLayout = GridLayout,
                    DockLayout = DockLayout,
                    UseAbsoluteRanges = UseAbsoluteRanges,
                    ListViewLayout = ListViewLayout,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.PenList = PenList != null ? new PenItemList(PenList) : new PenItemList();
                selected.GridLayout = GridLayout;
                selected.DockLayout = DockLayout;
                selected.ListViewLayout = ListViewLayout;
                selected.UseAbsoluteRanges = UseAbsoluteRanges;
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
            SaveRuntimeLayout(GetStorageName(true));
            bCallingSaveCommand = false;
        }
        RelayCommand _resetCommand;
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
        internal void CallResetCommand()
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

        internal bool IsEnableResetCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand || bCallingEditCommand)
                    return false;
                else
                    return oldMemoryList != null;
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
                        param => IsEnableCommand
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
                            where m.Name == configMemory.EditValue as String
                            select m).FirstOrDefault();

            if (oldMemoryList == null)
            {
                oldMemoryList = new MemorySettings(MemorySettingList);
                oldConfigName = configMemory.EditValue as String;
            }

            if (selected != null)
                MemorySettingList.Remove(selected);
            
            StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            configMemory.DataContext = MemorySettingList?.Names;
            bCallingRemoveCommand = false;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }

        internal bool IsEnableCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
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
                if (bDispose || !UserBasedRuntimeSettings)
                    return;

                if (!String.IsNullOrEmpty(helper.Username))
                    MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username);
                else
                    InitMemorySettingList();
                if (!LoadRuntimeLayout(GetStorageName(true)))
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
                GetItem(ActualConfig);
            });
        }
        #endregion

        public void Initialize()
        {
        }
        #endregion
        private void OnDockItemHidden(object sender, DevExpress.Xpf.Docking.Base.ItemEventArgs e)
        {
            if (e.Item as LayoutPanel != null && e.Item.IsActive)
                dockManager.ActiveDockItem = chartPanel;
        }
        private void chart_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is FrameworkElement && (e.Key == Key.LeftShift || e.Key == Key.RightShift))
                UpdateFocus(e.Source as FrameworkElement);
            if (IsInStop && sfchart.SelectedItem as SeriesPoint != null && (e.Key == Key.Right || e.Key == Key.Left)) //Arrow key navigation through series points
            {
                var selPoint = sfchart.SelectedItem as SeriesPoint;

                SeriesPoint np = null;
                int bFw = 1;
                var curIndex = selPoint.Series.Points.IndexOf(selPoint);
                if (curIndex < selPoint.Series.Points.Count - 1)
                {
                    np = selPoint.Series.Points[curIndex + 1];
                    if (np.DateTimeArgument < selPoint.DateTimeArgument)
                        bFw = -1;
                }
                else if (curIndex > 0)
                {
                    np = selPoint.Series.Points[curIndex - 1];
                    if (np.DateTimeArgument > selPoint.DateTimeArgument)
                        bFw = -1;
                }

                if (np == null)
                    return;

                var npIndex = selPoint.Series.Points.IndexOf(selPoint) + (e.Key == Key.Right ? 1 : -1) * bFw;
                if (npIndex < 0 || npIndex > selPoint.Series.Points.Count - 1)
                    return;

                var nextPoint = selPoint.Series.Points[npIndex];
                
                sfchart.SelectedItem = nextPoint;
                //Updating legend with next point data
                AxisY2D yAxis = null;
                if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(selPoint.Series.DisplayName))
                {
                    string sanitized = GetSanitizedName(selPoint.Series.DisplayName);
                    yAxis = (from c in ((XYDiagram2D)sfchart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                    settingStorage.mapSeries[selPoint.Series.DisplayName].LastClickedPoint.Date = nextPoint.Argument;
                    settingStorage.mapSeries[selPoint.Series.DisplayName].LastClickedPoint.Value = nextPoint.Value;
                }

                //Autoscroll if the nextPoint is out of the current X axis'VisualRange
                if (axisX.ActualVisualRange.ActualMinValue is DateTime && axisX.ActualVisualRange.ActualMaxValue is DateTime)
                {
                    bool bNextPointOverMax = nextPoint.DateTimeArgument >= (DateTime)axisX.ActualVisualRange.ActualMaxValue;
                    bool bNextPointBeforeMin = nextPoint.DateTimeArgument <= (DateTime)axisX.ActualVisualRange.ActualMinValue;
                    if (bNextPointBeforeMin || bNextPointOverMax)
                    {
                        TimeSpan timeFrame = (DateTime)axisX.ActualVisualRange.ActualMaxValue - (DateTime)axisX.ActualVisualRange.ActualMinValue;
                        var timeFramePadding = new TimeSpan(timeFrame.Ticks / Properties.Settings.Default.KeyboardAutoScrollTimeFraction);
                        if (bNextPointOverMax)
                            axisX.ActualVisualRange.SetMinMaxValues(nextPoint.DateTimeArgument - timeFrame + timeFramePadding, nextPoint.DateTimeArgument + timeFramePadding);
                        else
                            axisX.ActualVisualRange.SetMinMaxValues(nextPoint.DateTimeArgument - timeFramePadding, nextPoint.DateTimeArgument + timeFrame - timeFramePadding);
                    }
                }

                //Moving crosshair to the next point
                ControlCoordinates pointCoords;
                if (yAxis != null)
                    pointCoords = diagram.DiagramToPoint(nextPoint.DateTimeArgument, nextPoint.Value, axisX, yAxis);
                else
                    pointCoords = diagram.DiagramToPoint(nextPoint.DateTimeArgument, nextPoint.Value);
                crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForEachSeries;
                diagram.ShowCrosshair(new Point(pointCoords.Point.X, pointCoords.Point.Y));
                crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;

                foreach (var serie in diagram.Series)
                {
                    if (serie == selPoint.Series)
                        continue;
                    SerieSettings s = null;
                    if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(serie.DisplayName))
                        s = settingStorage.mapSeries[serie.DisplayName];
                    if (s == null)
                        continue;
                    if (nearestPoints.ContainsKey(serie.DisplayName))
                    {
                        s.LastClickedPoint.Date = nearestPoints[serie.DisplayName].Argument;
                        s.LastClickedPoint.Value = nearestPoints[serie.DisplayName].Value;
                    }
                }
            }
        }
        private void UpdateFocus(FrameworkElement sender)
        {
            if (!IsElementContained(sender))
                return;

            sfchart.Focus();
            diagram.EnableAxisYNavigation = false;
        }
        bool IsElementContained(object sender)
        {
            return sender == sfchart || (from c in sfchart.GetVisualChildrenOfType<FrameworkElement>()
                    where c == sender
                    select c).FirstOrDefault() != null;
        }
        private void chart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsElementContained(sender))
                return;
            if (!IsInStop)
                Pause(this, null);
            diagram.EnableAxisYNavigation = false;
        }

        void SetAxisFontSettings(GlobalConverters.FontSettings newSettings, AxisBase axis)
        {
            if (bDispose)
                return;

            axis.Label.FontFamily = newSettings.FontFamily;
            axis.Label.FontSize = newSettings.FontSize;
            axis.Label.FontStyle = newSettings.FontStyle;
            axis.Label.FontWeight = newSettings.FontWeight;
        }
        #region IStringIDAware

        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (PenList != null)
            {
                var _list = PenList.Where(x => !string.IsNullOrEmpty(x.title)).Select(x => x.title);
                if (_list != null && _list.Count() > 0)
                    list.AddRange(_list);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            int i = 1;
            PenList?.Where(x => !string.IsNullOrEmpty(x.title)).ToList().ForEach(x =>
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
            return (from column in legend_GridControl.Columns
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
    }

    public class CurrentValues : INotifyPropertyChanged
    {
        public Dictionary<string, double> MapValues { get; set; } = new Dictionary<string, double>();

        bool Add(string key, double val = 0)
        {
            if (!MapValues.ContainsKey(key))
            {
                MapValues.Add(key, val);
                OnPropertyChanged("MapValues");
                return true;
            }
            return false;
        }
        public void Set(string key, double val = 0)
        {
            if (!Add(key, val))
            {
                MapValues[key] = val;
                OnPropertyChanged("MapValues");
            }
        }
        public double Get(string key)
        {
            if (MapValues.ContainsKey(key))
                return MapValues[key];
            return 0;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
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
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "plotArea", (document as ScreenDocument).Theme)?.BorderBrush;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "plotArea", (document as ScreenDocument).Theme)?.Background;
            if (sender is RealTimeTrendControl)
            {
                RealTimeTrendControl control = sender as RealTimeTrendControl;
                if (control.ReadLocalValue(RealTimeTrendControl.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
                if (control.ReadLocalValue(RealTimeTrendControl.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(RealTimeTrendControl.GridColorProperty) != DependencyProperty.UnsetValue)
                    ret.Add("GridColor", control.GridColor);
                else
                    ret.Add("GridColor", null);

                if (control.ReadLocalValue(RealTimeTrendControl.XMinorGridLineColorProperty) != DependencyProperty.UnsetValue)
                    ret.Add("XMinorGridLineColor", control.XMinorGridLineColor);
                else
                    ret.Add("XMinorGridLineColor", null);

                if (control.ReadLocalValue(RealTimeTrendControl.AxisStrokeProperty) != DependencyProperty.UnsetValue)
                    ret.Add("AxisStroke", control.AxisStroke);
                else
                    ret.Add("AxisStroke", null);

                if (control.ReadLocalValue(RealTimeTrendControl.PrimaryAxisGridColorProperty) != DependencyProperty.UnsetValue)
                    ret.Add("PrimaryAxisGridColor", control.PrimaryAxisGridColor);
                else
                    ret.Add("PrimaryAxisGridColor", null);

                if (control.ReadLocalValue(RealTimeTrendControl.SecAxisGridColorProperty) != DependencyProperty.UnsetValue)
                    ret.Add("SecAxisGridColor", control.SecAxisGridColor);
                else
                    ret.Add("SecAxisGridColor", null);

                if (control.ReadLocalValue(RealTimeTrendControl.AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("AxisLabelForeground", control.AxisLabelForeground);
                else
                    ret.Add("AxisLabelForeground", foreground);

                if (control.ReadLocalValue(RealTimeTrendControl.PrimaryAxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("PrimaryAxisLabelForeground", control.PrimaryAxisLabelForeground);
                else
                    ret.Add("PrimaryAxisLabelForeground", foreground);

                if (control.ReadLocalValue(RealTimeTrendControl.SecAxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("SecAxisLabelForeground", control.SecAxisLabelForeground);
                else
                    ret.Add("SecAxisLabelForeground", foreground);

                if (control.ReadLocalValue(RealTimeTrendControl.LegendAreaForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("LegendAreaForeground", control.LegendAreaForeground);
                else
                    ret.Add("LegendAreaForeground", foreground);

                if (control.ReadLocalValue(RealTimeTrendControl.PlotBackgroundProperty) != DependencyProperty.UnsetValue)
                {
                    ret.Add("PlotBackground", control.PlotBackground);
                    ret.Add("DiagramBackground", control.TrendBackground);
                }
                else
                {
                    ret.Add("PlotBackground", background);
                    ret.Add("DiagramBackground", background);
                }

                if (control.ReadLocalValue(RealTimeTrendControl.TrendBackgroundProperty) != DependencyProperty.UnsetValue)
                {
                    ret.Add("TrendBackground", control.TrendBackground);
                }
                else
                {
                    ret.Add("TrendBackground", background);
                }

                ret.Add("ToolbarForeground", foreground);
                if (background is SolidColorBrush)
                {
                    SolidColorBrush solidColorBrush = (background as SolidColorBrush);
                    ret.Add("ToolbarBackground", new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast(solidColorBrush.Color, (document as ScreenDocument).Theme)));
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
                    ret.Add("ToolbarBackground", linearGradientBrush);
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
