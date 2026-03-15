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
using LinqStatistics;
using ChartsDemo;
using Utilities.WPF;
using DevExpress.Xpf.Docking;
using System.Diagnostics;
using System.Threading;
using Utilities;
using System.Windows.Media.Animation;
using WPFUtilities;
using DevExpress.Xpf.Core;
using DevExpress.Charts.Designer;
using DevExpress.Xpf.Printing;
using System.Text.RegularExpressions;
using log4net;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using ScreenSettings;
using System.Data.Common;
using System.Data;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using StringManager.ComponentService;
using System.Xml.Serialization;
using UFUAHistorianModel;
using OPCUAViewModel;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces;
using DynamicTagAwareHelper;
using DataAnalisysRTControl.Helpers;
using TranslationHelpers;
using ViewModelLib;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using System.Windows.Threading;
using DataLoggerColumnListControl;
using WPFPenHelpers;
using System.Globalization;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using ScreenSettings.Documents;
using Opc.Ua;
using System.Collections;
using System.Windows.Data;
using GlobalConverters = Converters;
using DataAnalisysRTControl.Converters;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Mvvm.Native;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using StorageHelper;

namespace DataAnalisysRTControl
{
    /// <summary>
    /// Interaction logic for DataAnalisysRT.xaml
    /// </summary>
    public partial class DataAnalisysRT : UserControl, IEntityReference, IContainPropertyEditors, IDisposable, IDynamicTagAware, ISettingsHelper, INotifyPropertyVisibilityChanged, IConnectionAware
        , IStringIDAware, IDropRecordAware, IGridLayoutUser
    {
        #region Dependency Properties

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(DataAnalisysRT));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(DataAnalisysRT));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as DataAnalisysRT;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode) && !bOverride)
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
            var control = sender as DataAnalisysRT;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode) && !bOverride)
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
            var control = sender as DataAnalisysRT;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode) && !bOverride)
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
            var control = sender as DataAnalisysRT;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if ((bDesignmode && bInit || !bDesignmode) && !bOverride)
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
            var control = sender as DataAnalisysRT;
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
            var control = sender as DataAnalisysRT;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        bool bOverride;
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled && !bOverride)
            {
                DiagramBackground = Background;
                PlotBackground = Background;
                ControlBackground = Background;
            }
        }
        #endregion

        #region AutoHidePanelsVisible
        public static readonly DependencyProperty AutoHidePanelsVisibleProperty = DependencyProperty.Register("AutoHidePanelsVisible", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutoHidePanelsVisibleChanged), new CoerceValueCallback(OnCoerceAutoHidePanelsVisible)));

        private static object OnCoerceAutoHidePanelsVisible(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceAutoHidePanelsVisible((bool)value);
            else
                return value;
        }

        private static void OnAutoHidePanelsVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnAutoHidePanelsVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
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
        public static readonly DependencyProperty YAxsisFontSettingsProperty = DependencyProperty.Register("YAxsisFontSettings", typeof(GlobalConverters.FontSettings), typeof(DataAnalisysRT), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 12), new PropertyChangedCallback(OnYAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceYAxsisFontSettings)));

        private static object OnCoerceYAxsisFontSettings(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceYAxsisFontSettings((GlobalConverters.FontSettings)value);
            else
                return value;
        }

        private static void OnYAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnYAxsisFontSettingsChanged((GlobalConverters.FontSettings)e.OldValue, (GlobalConverters.FontSettings)e.NewValue);
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
            (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select c).ToList().ForEach(ySecAxis => {
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


        public static readonly DependencyProperty XAxsisFontSettingsProperty = DependencyProperty.Register("XAxsisFontSettings", typeof(GlobalConverters.FontSettings), typeof(DataAnalisysRT), new UIPropertyMetadata(new GlobalConverters.FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 12), new PropertyChangedCallback(OnXAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceXAxsisFontSettings)));

        private static object OnCoerceXAxsisFontSettings(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceXAxsisFontSettings((GlobalConverters.FontSettings)value);
            else
                return value;
        }

        private static void OnXAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnXAxsisFontSettingsChanged((GlobalConverters.FontSettings)e.OldValue, (GlobalConverters.FontSettings)e.NewValue);
        }

        protected virtual GlobalConverters.FontSettings OnCoerceXAxsisFontSettings(GlobalConverters.FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXAxsisFontSettingsChanged(GlobalConverters.FontSettings oldValue, GlobalConverters.FontSettings newValue)
        {
            SetAxisFontSettings(newValue, axisX);
            if (secondaryAxisX2D != null)
                SetAxisFontSettings(newValue, secondaryAxisX2D);
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
        public static readonly DependencyProperty DefToolbarHeightProperty = DependencyProperty.Register("DefToolbarHeight", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata(25d));
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
        public static readonly DependencyProperty MeasureUnitMultiplierProperty = DependencyProperty.Register("MeasureUnitMultiplier", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(1, new PropertyChangedCallback(OnMeasureUnitMultiplierChanged), new CoerceValueCallback(OnCoerceMeasureUnitMultiplier)));

        private static object OnCoerceMeasureUnitMultiplier(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceMeasureUnitMultiplier((int)value);
            else
                return value;
        }

        private static void OnMeasureUnitMultiplierChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
            SetDateTimeScaleMeasureUnitMultiplier(axisX, newValue);
            if (secondaryAxisX2D != null)
                SetDateTimeScaleMeasureUnitMultiplier(secondaryAxisX2D, newValue);
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


        #region DisableZoomBehaviour
        public static readonly DependencyProperty DisableZoomBehaviourProperty = DependencyProperty.Register("DisableZoomBehaviour", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false));
        public bool DisableZoomBehaviour
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(DisableZoomBehaviourProperty);
            }
            set
            {
                SetValue(DisableZoomBehaviourProperty, value);
            }
        }

        #endregion


        #region TextPattern
        public static readonly DependencyProperty TextPatternProperty = DependencyProperty.Register("TextPattern", typeof(string), typeof(DataAnalisysRT), new UIPropertyMetadata("{A}: {V:F2}"));
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
        public static readonly DependencyProperty LegendAreaForegroundProperty = DependencyProperty.Register("LegendAreaForeground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnLegendAreaForegroundChanged), new CoerceValueCallback(OnCoerceLegendAreaForeground)));

        private static object OnCoerceLegendAreaForeground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceLegendAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnLegendAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty PointPrecisionProperty = DependencyProperty.Register("PointPrecision", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(2, new PropertyChangedCallback(OnPointPrecisionChanged), new CoerceValueCallback(OnCoercePointPrecision)));

        private static object OnCoercePointPrecision(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoercePointPrecision((int)value);
            else
                return value;
        }

        private static void OnPointPrecisionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty AxisYTextPatternProperty = DependencyProperty.Register("AxisYTextPattern", typeof(string), typeof(DataAnalisysRT), new UIPropertyMetadata("{V:F2}"));
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
        public static readonly DependencyProperty AxisLabelForegroundProperty = DependencyProperty.Register("AxisLabelForeground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxisLabelForegroundChanged), new CoerceValueCallback(OnCoerceAxisLabelForeground)));

        private static object OnCoerceAxisLabelForeground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceAxisLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnAxisLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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


        #region DiagramBackground
        public static readonly DependencyProperty DiagramBackgroundProperty = DependencyProperty.Register("DiagramBackground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnDiagramBackgroundChanged), new CoerceValueCallback(OnCoerceDiagramBackground)));

        private static object OnCoerceDiagramBackground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceDiagramBackground((Brush)value);
            else
                return value;
        }

        private static void OnDiagramBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnDiagramBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceDiagramBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDiagramBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }
        
        private void UpdateControlLayout()
        {
            bOverride = true;
            Background = ControlBackground;
            bOverride = false;
            if (dpUpdateLayout == null ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Completed ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Aborted)
            {
                dpUpdateLayout = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                {
                    if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    {
                        axisX.Label.Foreground = AxisLabelForeground;

                        if (secondaryAxisX2D != null)
                            secondaryAxisX2D.Label.Foreground = AxisLabelForeground;

                        //axisX.Label.FontSize = FontSize;
                        ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>().ForEach(c => c.Label.Foreground = AxisLabelForeground);
                        //axisY.Label.FontSize = FontSize;
                    }
                });
            }
            
            if (this.ReadLocalValue(DiagramBackgroundProperty) != DependencyProperty.UnsetValue)
                (chart.Diagram as XYDiagram2D).DefaultPane.Background = DiagramBackground;

            if (this.ReadLocalValue(ControlBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                dockManager.Background = ControlBackground;
                chart.Background = ControlBackground;
                timeRangeGrid.Background = ControlBackground;
                legend_GridControl.Background = ControlBackground;
                printGrid.Background = ControlBackground;
                //legendListBox.Background = ControlBackground;
                gridControl.Background = ControlBackground;
                palettePanel.Background = ControlBackground;
                gridPanel.Background = ControlBackground;
                chartPanel.Background = ControlBackground;
                timeRangePanel.Background = ControlBackground;
                legendPanel.Background = ControlBackground;
            }

            if (this.ReadLocalValue(PlotBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    (from c in chart.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                     where c.Name == "PART_DomainBackground" || c.Name == "OutsideBorder"
                     select c).ToList().ForEach(x =>
                     {
                         x.Background = PlotBackground;
                     });
                });
            }

            //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    toolbar.Background = ToolbarBackground;
            //    toolbarSettings.Background = toolbarShowHideButtons.Background = toolbarAdvSettings.Background = toolbarRefresh.Background = toolbarTimeControls.Background = toolbarMaxRecords.Background = toolbarCompare.Background = ToolbarBackground;
            //    toolbarPrint.Background = ToolbarBackground;
            //    toolbarPrintSettings.Background = ToolbarBackground;
            //}
            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    //chkUseAbsoluteRanges.Foreground = ToolbarForeground;
            //    btnPrintGrid.Foreground = ToolbarForeground;
            //    btnPrint.Foreground = ToolbarForeground;
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    cmbTimeRange.Foreground = ToolbarForeground;
            //    cmbCompare.Foreground = ToolbarForeground;
            //    maxRecord.Foreground = ToolbarForeground;
            //    compareTitle.Foreground = ToolbarForeground;
            //    startTextTitle.Foreground = ToolbarForeground;
            //    endTextTitle.Foreground = ToolbarForeground;
            //}
            //else
            //{
            //    var theme = ThemeImageHelper.GetTheme(Document);
            //    if (theme == "Blend")
            //    {
            //        chkUseAbsoluteRanges.Foreground = BlendForeground;
            //    }
            //}

            if (XGridLineColor != null)
                (axisX as AxisBase).GridLinesBrush = XGridLineColor;
            else
                (axisX as AxisBase).ClearValue(AxisBase.GridLinesBrushProperty);
            if (YGridLineColor != null)
                (axisY as AxisBase).GridLinesBrush = YGridLineColor;
            else
                (axisY as AxisBase).ClearValue(AxisBase.GridLinesBrushProperty);
            if (XMinorGridLineColor != null)
                (axisX as AxisBase).GridLinesMinorBrush = XMinorGridLineColor;
            else
                (axisX as AxisBase).ClearValue(AxisBase.GridLinesMinorBrushProperty);
            if (YMinorGridLineColor != null)
                (axisY as AxisBase).GridLinesMinorBrush = YMinorGridLineColor;
            else
                (axisY as AxisBase).ClearValue(AxisBase.GridLinesMinorBrushProperty);
            if (!XAutoGrid)
            {
                (axisX as AxisBase).MinorCount = XMinorCount;
                (secondaryAxisX2D as AxisBase).MinorCount = XMinorCount;
            }
            if (!YAutoGrid)
                (axisY as AxisBase).MinorCount = YMinorCount;
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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
        #region PlotBackground
        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        #region ControlBackground
        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnControlBackgroundChanged), new CoerceValueCallback(OnCoerceControlBackground)));

        private static object OnCoerceControlBackground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceControlBackground((Brush)value);
            else
                return value;
        }

        private static void OnControlBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnControlBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceControlBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush ControlBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ControlBackgroundProperty);
            }
            set
            {
                SetValue(ControlBackgroundProperty, value);
            }
        }

        #endregion

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("DataAnalisysOptions")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("DataAnalisysOptions")]
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

        #region XAutoGrid
        public static readonly DependencyProperty XAutoGridProperty = DependencyProperty.Register("XAutoGrid", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXAutoGridChanged), new CoerceValueCallback(OnCoerceXAutoGrid)));

        private static object OnCoerceXAutoGrid(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnXAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
                UpdateControlLayout();
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
        public static readonly DependencyProperty XGridLineVisibleProperty = DependencyProperty.Register("XGridLineVisible", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXGridLineVisible)));

        private static object OnCoerceXGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        #region XGridLineColor
        public static readonly DependencyProperty XGridLineColorProperty = DependencyProperty.Register("XGridLineColor", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXGridLineColorChanged), new CoerceValueCallback(OnCoerceXGridLineColor)));

        private static object OnCoerceXGridLineColor(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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

        public Brush XGridLineColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(XGridLineColorProperty);
            }
            set
            {
                SetValue(XGridLineColorProperty, value);
            }
        }

        #endregion
        #region XMajorCount
        public static readonly DependencyProperty XMajorCountProperty = DependencyProperty.Register("XMajorCount", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(10, new PropertyChangedCallback(OnXMajorCountChanged), new CoerceValueCallback(OnCoerceXMajorCount)));

        private static object OnCoerceXMajorCount(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXMajorCount((int)value);
            else
                return value;
        }

        private static void OnXMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
            if (oldValue != newValue)
                UpdateXAxisSpacing();
        }

        public int XMajorCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(XMajorCountProperty);
            }
            set
            {
                SetValue(XMajorCountProperty, value);
            }
        }

        #endregion
        #region XMinorGridLineVisible
        public static readonly DependencyProperty XMinorGridLineVisibleProperty = DependencyProperty.Register("XMinorGridLineVisible", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXMinorGridLineVisible)));

        private static object OnCoerceXMinorGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty XMinorGridLineColorProperty = DependencyProperty.Register("XMinorGridLineColor", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceXMinorGridLineColor)));

        private static object OnCoerceXMinorGridLineColor(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        #region XMinorCount
        public static readonly DependencyProperty XMinorCountProperty = DependencyProperty.Register("XMinorCount", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(5, new PropertyChangedCallback(OnXMinorCountChanged), new CoerceValueCallback(OnCoerceXMinorCount)));

        private static object OnCoerceXMinorCount(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceXMinorCount((int)value);
            else
                return value;
        }

        private static void OnXMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
                UpdateControlLayout();
            }
        }

        public int XMinorCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(XMinorCountProperty);
            }
            set
            {
                SetValue(XMinorCountProperty, value);
            }
        }

        #endregion

        #region YAutoGrid
        public static readonly DependencyProperty YAutoGridProperty = DependencyProperty.Register("YAutoGrid", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYAutoGridChanged), new CoerceValueCallback(OnCoerceYAutoGrid)));

        private static object OnCoerceYAutoGrid(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnYAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
                UpdateControlLayout();
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
        public static readonly DependencyProperty YGridLineVisibleProperty = DependencyProperty.Register("YGridLineVisible", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYGridLineVisible)));

        private static object OnCoerceYGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        #region YGridLineColor
        public static readonly DependencyProperty YGridLineColorProperty = DependencyProperty.Register("YGridLineColor", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYGridLineColorChanged), new CoerceValueCallback(OnCoerceYGridLineColor)));

        private static object OnCoerceYGridLineColor(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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

        public Brush YGridLineColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(YGridLineColorProperty);
            }
            set
            {
                SetValue(YGridLineColorProperty, value);
            }
        }

        #endregion
        #region YMajorCount
        public static readonly DependencyProperty YMajorCountProperty = DependencyProperty.Register("YMajorCount", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMajorCountChanged), new CoerceValueCallback(OnCoerceYMajorCount)));

        private static object OnCoerceYMajorCount(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYMajorCount((int)value);
            else
                return value;
        }

        private static void OnYMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
            if (oldValue != newValue)
                UpdateYAxisSpacing();
        }

        public int YMajorCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(YMajorCountProperty);
            }
            set
            {
                SetValue(YMajorCountProperty, value);
            }
        }

        #endregion
        #region YMinorGridLineVisible
        public static readonly DependencyProperty YMinorGridLineVisibleProperty = DependencyProperty.Register("YMinorGridLineVisible", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYMinorGridLineVisible)));

        private static object OnCoerceYMinorGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        #region YMinorGridLineColor
        public static readonly DependencyProperty YMinorGridLineColorProperty = DependencyProperty.Register("YMinorGridLineColor", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceYMinorGridLineColor)));

        private static object OnCoerceYMinorGridLineColor(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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

        public Brush YMinorGridLineColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(YMinorGridLineColorProperty);
            }
            set
            {
                SetValue(YMinorGridLineColorProperty, value);
            }
        }

        #endregion
        #region YMinorCount
        public static readonly DependencyProperty YMinorCountProperty = DependencyProperty.Register("YMinorCount", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMinorCountChanged), new CoerceValueCallback(OnCoerceYMinorCount)));

        private static object OnCoerceYMinorCount(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceYMinorCount((int)value);
            else
                return value;
        }

        private static void OnYMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
                UpdateControlLayout();
            }
        }

        public int YMinorCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(YMinorCountProperty);
            }
            set
            {
                SetValue(YMinorCountProperty, value);
            }
        }

        #endregion


        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnConnectionStringChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionString(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        // bool bConnectionStringChanged;
        protected virtual void OnConnectionStringChanged(String oldValue, String newValue)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bInit)
                RestoreChartFromSettings();
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

        public static readonly DependencyProperty ConnectionStringDesignModeProperty = DependencyProperty.Register("ConnectionStringDesignMode", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringDesignModeChanged), new CoerceValueCallback(OnCoerceConnectionStringDesignMode)));

        private static object OnCoerceConnectionStringDesignMode(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceConnectionStringDesignMode((String)value);
            else
                return value;
        }

        private static void OnConnectionStringDesignModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnConnectionStringDesignModeChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionStringDesignMode(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectionStringDesignModeChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public String ConnectionStringDesignMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ConnectionStringDesignModeProperty);
            }
            set
            {

                SetValue(ConnectionStringDesignModeProperty, value);
            }
        }
        #endregion
        #region MaxRecords
        public static readonly DependencyProperty MaxRecordsProperty = DependencyProperty.Register("MaxRecords", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxRecordsChanged), new CoerceValueCallback(OnCoerceMaxRecords)));

        private static object OnCoerceMaxRecords(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceMaxRecords((int)value);
            else
                return value;
        }

        private static void OnMaxRecordsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnMaxRecordsChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxRecords(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxRecordsChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int MaxRecords
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxRecordsProperty);
            }
            set
            {
                SetValue(MaxRecordsProperty, value);
            }
        }

        #endregion

        #region LoadMaxRecordFromDB
        public static readonly DependencyProperty LoadMaxRecordFromDBProperty = DependencyProperty.Register("LoadMaxRecordFromDB", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true));
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
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(30));
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
        public static readonly DependencyProperty MaxDeadLockRetryProperty = DependencyProperty.Register("MaxDeadLockRetry", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(1, new PropertyChangedCallback(OnMaxDeadLockRetryChanged), new CoerceValueCallback(OnCoerceMaxDeadLockRetry)));

        private static object OnCoerceMaxDeadLockRetry(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceMaxDeadLockRetry((int)value);
            else
                return value;
        }

        private static void OnMaxDeadLockRetryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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

        #region UseAggregation
        public static readonly DependencyProperty UseAggregationProperty = DependencyProperty.Register("UseAggregation", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseAggregationChanged), new CoerceValueCallback(OnCoerceUseAggregation)));

        private static object OnCoerceUseAggregation(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceUseAggregation((bool)value);
            else
                return value;
        }

        private static void OnUseAggregationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnUseAggregationChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseAggregation(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseAggregationChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bDesignmode)
            {
                visualRangeBeforeAggregationToggle = new Tuple<DateTime, DateTime>(
                    (DateTime)axisX.ActualVisualRange.ActualMinValue,
                    (DateTime)axisX.ActualVisualRange.ActualMaxValue);
                SetDataSources();
            }
        }

        public bool UseAggregation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseAggregationProperty);
            }
            set
            {
                SetValue(UseAggregationProperty, value);
            }
        }

        #endregion


        #region ShowPalette
        public static readonly DependencyProperty ShowPaletteProperty = DependencyProperty.Register("ShowPalette", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowPaletteChanged), new CoerceValueCallback(OnCoerceShowPalette)));

        private static object OnCoerceShowPalette(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceShowPalette((bool)value);
            else
                return value;
        }

        private static void OnShowPaletteChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnShowPaletteChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowPalette(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowPaletteChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                palettePanel.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public bool ShowPalette
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowPaletteProperty);
            }
            set
            {
                SetValue(ShowPaletteProperty, value);
            }
        }

        #endregion


        #region MaxAggregationFactor
        public static readonly DependencyProperty MaxAggregationFactorProperty = DependencyProperty.Register("MaxAggregationFactor", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(10, new PropertyChangedCallback(OnMaxAggregationFactorChanged), new CoerceValueCallback(OnCoerceMaxAggregationFactor)));

        private static object OnCoerceMaxAggregationFactor(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceMaxAggregationFactor((int)value);
            else
                return value;
        }

        private static void OnMaxAggregationFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnMaxAggregationFactorChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxAggregationFactor(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxAggregationFactorChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int MaxAggregationFactor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxAggregationFactorProperty);
            }
            set
            {
                SetValue(MaxAggregationFactorProperty, value);
            }
        }

        #endregion


        #region MaxResolveOverlappingPoints
        public static readonly DependencyProperty MaxResolveOverlappingPointsProperty = DependencyProperty.Register("MaxResolveOverlappingPoints", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(360, new PropertyChangedCallback(OnMaxResolveOverlappingPointsChanged), new CoerceValueCallback(OnCoerceMaxResolveOverlappingPoints)));

        private static object OnCoerceMaxResolveOverlappingPoints(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceMaxResolveOverlappingPoints((int)value);
            else
                return value;
        }

        private static void OnMaxResolveOverlappingPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty MaxLabelPointsProperty = DependencyProperty.Register("MaxLabelPoints", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxLabelPointsChanged), new CoerceValueCallback(OnCoerceMaxLabelPoints)));

        private static object OnCoerceMaxLabelPoints(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceMaxLabelPoints((int)value);
            else
                return value;
        }

        private static void OnMaxLabelPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoHideToolbar((bool)value);
            else
                return value;
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
                RereshAtuoHide(newValue, true);
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
        public static readonly DependencyProperty AutoCollapseHeightProperty = DependencyProperty.Register("AutoCollapseHeight", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseHeightChanged), new CoerceValueCallback(OnCoerceAutoCollapseHeight)));

        private static object OnCoerceAutoCollapseHeight(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseHeight((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty AutoCollapseWidthProperty = DependencyProperty.Register("AutoCollapseWidth", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseWidthChanged), new CoerceValueCallback(OnCoerceAutoCollapseWidth)));

        private static object OnCoerceAutoCollapseWidth(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseWidth((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty LogarithmicYScaleProperty = DependencyProperty.Register("LogarithmicYScale", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLogarithmicYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicYScale)));

        private static object OnCoerceLogarithmicYScale(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicYScale((bool)value);
            else
                return value;
        }

        private static void OnLogarithmicYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty LogarithmicBaseYScaleProperty = DependencyProperty.Register("LogarithmicBaseYScale", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata(10.0, new PropertyChangedCallback(OnLogarithmicBaseYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicBaseYScale)));

        private static object OnCoerceLogarithmicBaseYScale(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicBaseYScale((double)value);
            else
                return value;
        }

        private static void OnLogarithmicBaseYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty RotatedProperty = DependencyProperty.Register("Rotated", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRotatedChanged), new CoerceValueCallback(OnCoerceRotated)));

        private static object OnCoerceRotated(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceRotated((bool)value);
            else
                return value;
        }

        private static void OnRotatedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        #region AllowRuntimeChanges
        public static readonly DependencyProperty AllowRuntimeChangesProperty = DependencyProperty.Register("AllowRuntimeChanges", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRuntimeChangesChanged), new CoerceValueCallback(OnCoerceAllowRuntimeChanges)));

        private static object OnCoerceAllowRuntimeChanges(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAllowRuntimeChanges((bool)value);
            else
                return value;
        }

        private static void OnAllowRuntimeChangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnAllowRuntimeChangesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRuntimeChanges(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRuntimeChangesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                //btnChartDesigner.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                btnExpand.IsVisible = newValue;
                btnRefresh.IsVisible = newValue;

                if (bDesignmode && bInit)
                    RereshAtuoHide(AutoHideToolbar);

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

        public bool AllowRuntimeChanges
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowRuntimeChangesProperty);
            }
            set
            {
                SetValue(AllowRuntimeChangesProperty, value);
            }
        }
        #endregion
        #region ShowToolbarShowHideButtons
        public static readonly DependencyProperty ShowToolbarShowHideButtonsProperty = DependencyProperty.Register("ShowToolbarShowHideButtons", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarShowHideButtonsChanged), new CoerceValueCallback(OnCoerceShowToolbarShowHideButtons)));

        private static object OnCoerceShowToolbarShowHideButtons(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceShowToolbarShowHideButtons((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarShowHideButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnShowToolbarShowHideButtonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarShowHideButtons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarShowHideButtonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit && newValue != oldValue)
                RereshAtuoHide(AutoHideToolbar);
        }

        private bool RereshAtuoHide(bool autoHideValue, bool bForceUpdate = false)
        {
            bool collapseToolbar = !ShowToolbarAdvSettings &&
                                     !ShowToolbarCompare &&
                                     !ShowToolbarMaxRecords &&
                                     !ShowToolbarShowHideButtons &&
                                     !ShowToolbarTimeControls &&
                                     !ShowToolbarAdvSettings &&
                                     !AllowRuntimeChanges;

            toolbar.Visibility = collapseToolbar ? Visibility.Collapsed : Visibility.Visible;

            if (bForceUpdate)
            {
                if (autoHideValue)
                {
                    OnMouseLeave();
                    Grid.SetRow(adorner, 0);
                    Grid.SetRowSpan(adorner, 2);
                }
                else if(!collapseToolbar)
                {
                    OnMouseEnter();
                    Grid.SetRow(adorner, 1);
                    Grid.SetRowSpan(adorner, 1);
                }
            }
            return autoHideValue && !collapseToolbar;
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
        public static readonly DependencyProperty ShowToolbarAdvSettingsProperty = DependencyProperty.Register("ShowToolbarAdvSettings", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarAdvSettingsChanged), new CoerceValueCallback(OnCoerceShowToolbarAdvSettings)));

        private static object OnCoerceShowToolbarAdvSettings(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceShowToolbarAdvSettings((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarAdvSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnShowToolbarAdvSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarAdvSettings(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarAdvSettingsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit && newValue != oldValue)
                RereshAtuoHide(AutoHideToolbar);
        }

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
        public static readonly DependencyProperty ShowToolbarTimeControlsProperty = DependencyProperty.Register("ShowToolbarTimeControls", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarTimeControlsChanged), new CoerceValueCallback(OnCoerceShowToolbarTimeControls)));

        private static object OnCoerceShowToolbarTimeControls(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceShowToolbarTimeControls((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarTimeControlsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnShowToolbarTimeControlsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarTimeControls(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarTimeControlsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit && newValue != oldValue)
                RereshAtuoHide(AutoHideToolbar);
        }

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
        public static readonly DependencyProperty ShowToolbarMaxRecordsProperty = DependencyProperty.Register("ShowToolbarMaxRecords", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarMaxRecordsChanged), new CoerceValueCallback(OnCoerceShowToolbarMaxRecords)));

        private static object OnCoerceShowToolbarMaxRecords(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceShowToolbarMaxRecords((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarMaxRecordsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnShowToolbarMaxRecordsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowToolbarMaxRecords(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowToolbarMaxRecordsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit && newValue != oldValue)
                RereshAtuoHide(AutoHideToolbar);
        }

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
        public static readonly DependencyProperty ShowToolbarCompareProperty = DependencyProperty.Register("ShowToolbarCompare", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowToolbarCompareChanged), new CoerceValueCallback(OnCoerceShowToolbarCompare)));

        private static object OnCoerceShowToolbarCompare(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceShowToolbarCompare((bool)value);
            else
                return value;
        }

        private static void OnShowToolbarCompareChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnShowToolbarCompareChanged((bool)e.OldValue, (bool)e.NewValue);
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


        #region ShowControlButtons
        public static readonly DependencyProperty ShowControlButtonsProperty = DependencyProperty.Register("ShowControlButtons", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true));

        public bool ShowControlButtons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowControlButtonsProperty);
            }
            set
            {
                SetValue(ShowControlButtonsProperty, value);
            }
        }

        #endregion


        #region AlwaysExpanded
        public static readonly DependencyProperty AlwaysExpandedProperty = DependencyProperty.Register("AlwaysExpanded", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAlwaysExpandedChanged), new CoerceValueCallback(OnCoerceAlwaysExpanded)));

        private static object OnCoerceAlwaysExpanded(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAlwaysExpanded((bool)value);
            else
                return value;
        }

        private static void OnAlwaysExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        #region ShowStatisticLines
        public static readonly DependencyProperty ShowStatisticLinesProperty = DependencyProperty.Register("ShowStatisticLines", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowStatisticLinesChanged), new CoerceValueCallback(OnCoerceShowStatisticLines)));

        private static object OnCoerceShowStatisticLines(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceShowStatisticLines((bool)value);
            else
                return value;
        }

        private static void OnShowStatisticLinesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnShowStatisticLinesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowStatisticLines(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowStatisticLinesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bDesignmode)
                chart_BoundDataChanged(this, null);
        }

        public bool ShowStatisticLines
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowStatisticLinesProperty);
            }
            set
            {
                SetValue(ShowStatisticLinesProperty, value);
            }
        }
        #endregion
        #region StaticSeriesSettings
        public static readonly DependencyProperty StaticSeriesSettingsProperty = DependencyProperty.Register("StaticSeriesSettings", typeof(SerieDataList), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnStaticSeriesSettingsChanged), new CoerceValueCallback(OnCoerceStaticSeriesSettings)));

        private static object OnCoerceStaticSeriesSettings(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceStaticSeriesSettings((SerieDataList)value);
            else
                return value;
        }

        private static void OnStaticSeriesSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                dataAnalisys.OnStaticSeriesSettingsChanged((SerieDataList)e.OldValue, (SerieDataList)e.NewValue);
        }

        protected virtual SerieDataList OnCoerceStaticSeriesSettings(SerieDataList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStaticSeriesSettingsChanged(SerieDataList oldValue, SerieDataList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode && bInit)
            {
                InitChart();
            }
        }

        [SvgValueConverter(typeof(ConvertSerieDataList))]
        public SerieDataList StaticSeriesSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SerieDataList)GetValue(StaticSeriesSettingsProperty);
            }
            set
            {
                SetValue(StaticSeriesSettingsProperty, value);
            }
        }

        #endregion
        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty DockLayoutProperty = DependencyProperty.Register("DockLayout", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDockLayoutChanged), new CoerceValueCallback(OnCoerceDockLayout)));

        private static object OnCoerceDockLayout(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDockLayout((String)value);
            else
                return value;
        }

        private static void OnDockLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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

        #region ListViewLayout
        public static readonly DependencyProperty ListViewLayoutProperty = DependencyProperty.Register("ListViewLayout", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnListViewLayoutChanged), new CoerceValueCallback(OnCoerceListViewLayout)));

        private static object OnCoerceListViewLayout(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceListViewLayout((String)value);
            else
                return value;
        }

        private static void OnListViewLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
                        bPointDateTitleVisible = pointDateTitle.Visible;
                        bPointValueTitleVisible = pointValueTitle.Visible;
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

        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(DataAnalisysRT), new UIPropertyMetadata(DateSpan.None, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                control.OnFilterTypeChanged((DateSpan)e.OldValue, (DateSpan)e.NewValue);
        }

        protected virtual DateSpan OnCoerceFilterType(DateSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilterTypeChanged(DateSpan oldValue, DateSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bDesignmode && IsInStop && !bUserInteractionSettings && oldValue != newValue)
            {
                CallFilterTypeCommand(newValue);
            }
        }

        public DateSpan FilterType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateSpan)GetValue(FilterTypeProperty);
            }
            set
            {
                SetValue(FilterTypeProperty, value);
            }
        }

        #endregion

        #region Minimum
        //private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata((double)0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        //private static object OnCoerceMinimum(DependencyObject o, object value)
        //{
        //    DataAnalisysRT control = o as DataAnalisysRT;
        //    if (control != null)
        //        return control.OnCoerceMinimum((double)value);
        //    else
        //        return value;
        //}

        //private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    DataAnalisysRT control = o as DataAnalisysRT;
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
        //private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(DataAnalisysRT), new UIPropertyMetadata((double)100, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        //private static object OnCoerceMaximum(DependencyObject o, object value)
        //{
        //    DataAnalisysRT control = o as DataAnalisysRT;
        //    if (control != null)
        //        return control.OnCoerceMaximum((double)value);
        //    else
        //        return value;
        //}

        //private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty StartDateTitleProperty = DependencyProperty.Register("StartDateTitle", typeof(string), typeof(DataAnalisysRT), new UIPropertyMetadata(Properties.Resources.StartDate));
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
        public static readonly DependencyProperty EndDateTitleProperty = DependencyProperty.Register("EndDateTitle", typeof(string), typeof(DataAnalisysRT), new UIPropertyMetadata(Properties.Resources.EndDate));
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
        public static readonly DependencyProperty AllValuesXMarginProperty = DependencyProperty.Register("AllValuesXMargin", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(5, new PropertyChangedCallback(OnAllValuesXMarginChanged), new CoerceValueCallback(OnCoerceAllValuesXMargin)));

        private static object OnCoerceAllValuesXMargin(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisys = o as DataAnalisysRT;
            if (DataAnalisys != null)
                return DataAnalisys.OnCoerceAllValuesXMargin((int)value);
            else
                return value;
        }

        private static void OnAllValuesXMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty ScalePaddingFactorProperty = DependencyProperty.Register("ScalePaddingFactor", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(20, new PropertyChangedCallback(OnScalePaddingFactorChanged), new CoerceValueCallback(OnCoerceScalePaddingFactor)));

        private static object OnCoerceScalePaddingFactor(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceScalePaddingFactor((int)value);
            else
                return value;
        }

        private static void OnScalePaddingFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnScalePaddingFactorChanged((int)e.OldValue, (int)e.NewValue);
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

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false));

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
        public string ConnectionError
        {
            get
            {
                if (dlException != null)
                    return String.Format(Properties.Resources.ErrorConnectionText, dlException.Message);
                else
                    return Properties.Resources.NullConnectionText;
            }
        }


        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(DataAnalisysRT));
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
        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register("DateTimeFormat", typeof(String), typeof(DataAnalisysRT), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDateTimeFormatChanged), new CoerceValueCallback(OnCoerceDateTimeFormat)));

        private static object OnCoerceDateTimeFormat(DependencyObject o, object value)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDateTimeFormat((String)value);
            else
                return value;
        }

        private static void OnDateTimeFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT dataAnalisys = o as DataAnalisysRT;
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
        public static readonly DependencyProperty IsInStopProperty = DependencyProperty.Register("IsInStop", typeof(Boolean), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsInStopChanged), new CoerceValueCallback(OnCoerceIsInStop)));

        private static object OnCoerceIsInStop(DependencyObject o, object value)
        {
            DataAnalisysRT da = o as DataAnalisysRT;
            if (da != null)
                return da.OnCoerceIsInStop((Boolean)value);
            else
                return value;
        }

        private static void OnIsInStopChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT da = o as DataAnalisysRT;
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
            if (OperatingMode == OperatingMode.OnlyStop || timer == null ||  !bControlLoaded)
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
                if (bPointDateTitleVisible)
                    pointDateTitle.Visible = newValue;
                if (bPointValueTitleVisible)
                    pointValueTitle.Visible = newValue;
            }
        }
        
        void EnableToolbars(bool bEnable)
        {
            toolbarSettings.IsEnabled = toolbarShowHideButtons.IsEnabled = toolbarAdvSettings.IsEnabled = toolbarRefresh.IsEnabled = toolbarTimeControls.IsEnabled = toolbarMaxRecords.IsEnabled = toolbarCompare.IsEnabled = bEnable;
        }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsInStop
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(IsInStopProperty) || OperatingMode == OperatingMode.OnlyStop;
            }
            set
            {
                SetValue(IsInStopProperty, value);
            }
        }
        #endregion
        #region OperatingMode
        public static readonly DependencyProperty OperatingModeProperty = DependencyProperty.Register("OperatingMode", typeof(OperatingMode), typeof(DataAnalisysRT), new UIPropertyMetadata(OperatingMode.RunStop, new PropertyChangedCallback(OnOperatingModeChanged), new CoerceValueCallback(OnCoerceOperatingMode)));

        private static object OnCoerceOperatingMode(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceOperatingMode((OperatingMode)value);
            else
                return value;
        }

        private static void OnOperatingModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnOperatingModeChanged((OperatingMode)e.OldValue, (OperatingMode)e.NewValue);
        }

        protected virtual OperatingMode OnCoerceOperatingMode(OperatingMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOperatingModeChanged(OperatingMode oldValue, OperatingMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bInit)
                ReloadPens();
        }

        public OperatingMode OperatingMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (OperatingMode)GetValue(OperatingModeProperty);
            }
            set
            {
                SetValue(OperatingModeProperty, value);
            }
        }

        #region PlayOnStart
        public static readonly DependencyProperty PlayOnStartProperty = DependencyProperty.Register("PlayOnStart", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnPlayOnStartChanged), new CoerceValueCallback(OnCoercePlayOnStart)));

        private static object OnCoercePlayOnStart(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoercePlayOnStart((bool)value);
            else
                return value;
        }

        private static void OnPlayOnStartChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnPlayOnStartChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoercePlayOnStart(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPlayOnStartChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool PlayOnStart
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(PlayOnStartProperty);
            }
            set
            {
                SetValue(PlayOnStartProperty, value);
            }
        }
        #endregion
        #endregion
        #region LinkedPenName
        public static readonly DependencyProperty LinkedPenNameProperty = DependencyProperty.Register("LinkedPenName", typeof(string), typeof(DataAnalisysRT), new UIPropertyMetadata("Pen", new PropertyChangedCallback(OnLinkedPenNameChanged), new CoerceValueCallback(OnCoerceLinkedPenName)));

        private static object OnCoerceLinkedPenName(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceLinkedPenName((string)value);
            else
                return value;
        }

        private static void OnLinkedPenNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty RecordEveryProperty = DependencyProperty.Register("RecordEvery", typeof(TimeSpan), typeof(DataAnalisysRT), new UIPropertyMetadata(new TimeSpan(0, 0, 0, 0, 250), new PropertyChangedCallback(OnRecordEveryChanged), new CoerceValueCallback(OnCoerceRecordEvery)));

        private static object OnCoerceRecordEvery(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceRecordEvery((TimeSpan)value);
            else
                return value;
        }

        private static void OnRecordEveryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnRecordEveryChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
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
        public static readonly DependencyProperty ViewTimeFrameProperty = DependencyProperty.Register("ViewTimeFrame", typeof(TimeSpan), typeof(DataAnalisysRT), new UIPropertyMetadata(new TimeSpan(0, 0, 1, 0), new PropertyChangedCallback(OnViewTimeFrameChanged), new CoerceValueCallback(OnCoerceViewTimeFrame)));

        private static object OnCoerceViewTimeFrame(DependencyObject o, object value)
        {
            DataAnalisysRT RealTimeSDataValue = o as DataAnalisysRT;
            if (RealTimeSDataValue != null)
                return RealTimeSDataValue.OnCoerceViewTimeFrame((TimeSpan)value);
            else
                return value;
        }

        private static void OnViewTimeFrameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT RealTimeSDataValue = o as DataAnalisysRT;
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
        public static readonly DependencyProperty RecordOnlyOnQualityGoodProperty = DependencyProperty.Register("RecordOnlyOnQualityGood", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRecordOnlyOnQualityGoodChanged), new CoerceValueCallback(OnCoerceRecordOnlyOnQualityGood)));

        private static object OnCoerceRecordOnlyOnQualityGood(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceRecordOnlyOnQualityGood((bool)value);
            else
                return value;
        }

        private static void OnRecordOnlyOnQualityGoodChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty CurrentValueLabelForegroundProperty = DependencyProperty.Register("CurrentValueLabelForeground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(255, 255, 255)), new PropertyChangedCallback(OnCurrentValueLabelForegroundChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelForeground)));

        private static object OnCoerceCurrentValueLabelForeground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceCurrentValueLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty CurrentValueLabelBackgroundProperty = DependencyProperty.Register("CurrentValueLabelBackground", typeof(Brush), typeof(DataAnalisysRT), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(215, 106, 0)), new PropertyChangedCallback(OnCurrentValueLabelBackgroundChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelBackground)));

        private static object OnCoerceCurrentValueLabelBackground(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceCurrentValueLabelBackground((Brush)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
        public static readonly DependencyProperty AutomaticGeneralScaleProperty = DependencyProperty.Register("AutomaticGeneralScale", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutomaticGeneralScaleChanged), new CoerceValueCallback(OnCoerceAutomaticGeneralScale)));

        private static object OnCoerceAutomaticGeneralScale(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceAutomaticGeneralScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticGeneralScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnAutomaticGeneralScaleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutomaticGeneralScale(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutomaticGeneralScaleChanged(bool oldValue, bool newValue)
        {
            if (bDesignmode || bDispose || newValue == oldValue)
                return;

            axisY.Visible = newValue;

            if (newValue)
            {
                if (settingStorage != null && settingStorage.mapSeries != null)
                {
                    foreach (var serie in settingStorage.mapSeries.Keys)
                    {
                        var lsFound = (from c in diagram.Series.OfType<Series>() where c.DisplayName == serie select c).FirstOrDefault();
                        if (lsFound != null)
                        {
                            var saFound = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == GetSanitizedName(serie) select c).FirstOrDefault();
                            if (saFound != null)
                                saFound.Visible = false;
                            XYDiagram2D.SetSeriesAxisY((XYSeries)lsFound, null);
                        }
                    }
                }
            }
            else
            {
                if (settingStorage != null && settingStorage.mapSeries != null)
                {
                    foreach (var serie in settingStorage.mapSeries.Keys)
                    {
                        var lsFound = (from c in diagram.Series.OfType<Series>() where c.DisplayName == serie select c).FirstOrDefault();
                        if (lsFound != null)
                        {
                            var saFound = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == GetSanitizedName(serie) select c).FirstOrDefault();
                            if (saFound != null)
                            {
                                saFound.Visible = true;
                                XYDiagram2D.SetSeriesAxisY((XYSeries)lsFound, saFound);
                            }
                        }
                    }
                }

                var legendItems = legend_GridControl.ItemsSource as List<SerieSettings>;
                if (legendItems != null && legendItems.Count() > 0)
                {
                    legend_GridControl.SelectedItem = null;
                    legend_GridControl.SelectedItem = legendItems.First();
                }
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
        public static readonly DependencyProperty CurrentValueLabelWidthProperty = DependencyProperty.Register("CurrentValueLabelWidth", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(40, new PropertyChangedCallback(OnCurrentValueLabelWidthChanged), new CoerceValueCallback(OnCoerceCurrentValueLabelWidth)));

        private static object OnCoerceCurrentValueLabelWidth(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceCurrentValueLabelWidth((int)value);
            else
                return value;
        }

        private static void OnCurrentValueLabelWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnCurrentValueLabelWidthChanged((int)e.OldValue, (int)e.NewValue);
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
        public static readonly DependencyProperty SelectedPenThicknessProperty = DependencyProperty.Register("SelectedPenThickness", typeof(int), typeof(DataAnalisysRT), new UIPropertyMetadata(5, new PropertyChangedCallback(OnSelectedPenThicknessChanged), new CoerceValueCallback(OnCoerceSelectedPenThickness)));

        private static object OnCoerceSelectedPenThickness(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceSelectedPenThickness((int)value);
            else
                return value;
        }

        private static void OnSelectedPenThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnSelectedPenThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                return DataAnalisysRT.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT DataAnalisysRT = o as DataAnalisysRT;
            if (DataAnalisysRT != null)
                DataAnalisysRT.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region Demo Values
        List<MyDataValue> LoadData(Random r, int days)
        {
            List<MyDataValue> demoValues = new List<MyDataValue>();

            for (int i = 0; i < days; i++)
            {
                double? val = r != null ? r.NextDouble() * Properties.Settings.Default.DemoMaxValue : 0;
                demoValues.Add(new MyDataValue
                {
                    SourceTimeStamp = DateTime.Now.AddDays(i),
                    dValue = val
                });
            }
            if (settingStorage == null)
            {
                settingStorage = new SettingsStorage();
                settingStorage.StartTime = settingStorage.DateTimeStart = demoValues.First().SourceTimeStamp;
                settingStorage.EndTime = settingStorage.DateTimeEnd = demoValues.Last().SourceTimeStamp;
            }

            return demoValues;
        }
        #endregion

        #region Declarations
        bool forceUpdateOnMatchTypeDefinition;
        List<string> matchChangedMap = new List<string>();
        internal SerieDataList designPenList;
        IDataLayer dl;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        bool bLoaded;
        bool isTemplateApplied;
        bool bDesignmode;
        List<String> listSeries;// = new List<String>();
        SettingsStorage settingStorage;// = new SettingsStorage();
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Settings.Default.DataAnalysisControl);
        internal IDocument Document;// = null;
        IStringEditorManager stringManager;
        IUFUAEditorManager UFUAEditor;
        IUFProjectManager iUFProjectManager;
        IDictionary<String, String> stringlist;

        IDictionary<String, String> MapToListVariablesNodeId;// = new Dictionary<String, String>();
        IDictionary<String, String> MapToDatalogerConnectsions;// = new Dictionary<String, String>();
        IDictionary<String, String> MapToHistoricalConnectsions;// = new Dictionary<String, String>();
        IDictionary<String, UnitOfWork> MapToHistoricalUnitOfWork;// = new Dictionary<String, UnitOfWork>();
        IDictionary<String, IDataLayer> MapToHistoricalDataLayer;// = new Dictionary<String, IDataLayer>();
        DateTime maxDateTimeValue = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
        DateTime minDateTimeValue = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
        Dictionary<String, DataLoggerSettings> dlrSettings = new Dictionary<String, DataLoggerSettings>();
        string defaultDataProvider;
        string defaultConnectionString;
        ConstantLineCollection ConstantLines { get { return ((XYDiagram2D)chart.Diagram).AxisY.ConstantLinesBehind; } }
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
        bool bCallingFilterCommand = false;
        Tuple<DateTime, DateTime> visualRangeBeforeAggregationToggle;
        SerieSettings nearestSerie;
        Dictionary<Series, int> seriesHistoryDataCount = new Dictionary<Series, int>();
        bool bYScrolling = false;
        List<AxisY2D> axisRedrawDispatched = new List<AxisY2D>();
        Dictionary <string, SecondaryAxisY2D> MapSerieAxis = new Dictionary<string, SecondaryAxisY2D>();
        private Stream TemplateStream
        {
            get
            {
                return ExtractFileFromResource.Extract(Assembly.GetExecutingAssembly(), string.Format("{0}.Resources.{1}", typeof(DataAnalisysRT).Namespace, "DATemplates.xaml"));
            }
        }

        internal Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    if (StaticSeriesSettings != null)
                        for (int i = 0; i < StaticSeriesSettings.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(StaticSeriesSettings[i].guiId))
                                StaticSeriesSettings[i].guiId = Guid.NewGuid().ToString();
                            opcuaEntityReference[StaticSeriesSettings[i].guiId] = StaticSeriesSettings[i].tagReference;
                            if(StaticSeriesSettings[i].conditionalTag != null)
                                opcuaEntityReference[$"{condPrefix}{StaticSeriesSettings[i].guiId}"] = StaticSeriesSettings[i].conditionalTag;
                        }
                }
                return opcuaEntityReference;
            }
        }
        const string condPrefix = "cond";

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
        DispatcherOperation dpUpdateLayout;

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
        Dictionary<string, AnalysisDataGenerator> viewList;
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
        bool bPointDateTitleVisible = true;
        bool bPointValueTitleVisible = true;
        SecondaryAxisX2D secondaryAxisX2D;
        public DataAnalisysRT()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            secondaryAxisX2D = TryFindResource("secondaryaxisX") as SecondaryAxisX2D;
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
                        else
                            InitTimeRange();
                    }
                    DefToolbarHeight = toolbar.ActualHeight;
                    
                    SetAxisFontSettings(XAxsisFontSettings, axisX);
                    if (secondaryAxisX2D != null)
                        SetAxisFontSettings(XAxsisFontSettings, secondaryAxisX2D);
                    SetAxisFontSettings(YAxsisFontSettings, axisY);

                    if (bDesignmode)
                    {
                        DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                        chart.CrosshairOptions.ShowArgumentLine = false;
                        chart.CrosshairOptions.ShowValueLine = false;
                        chart.CrosshairOptions.ShowArgumentLabels = false;
                        chart.CrosshairOptions.ShowValueLabels = false;
                        chart.CrosshairOptions.ShowCrosshairLabels = false;
                        diagram.EnableAxisXNavigation = false;
                        diagram.EnableAxisYNavigation = false;
                        InitChart();
                        OverrideBaseProperties();

                        RereshAtuoHide(AutoHideToolbar, AutoHideToolbar);

                        toolbar.IsEnabled = false;

                        LoadDesignDockLayout();
                        LoadDesignGridLayout();
                        LoadDesignListViewLayout();

                        mainChartGrid.IsEnabled = false;
                        printGrid.IsEnabled = false;
                        timeRangeGrid.IsEnabled = false;
                        legend_GridControl.IsEnabled = false;
                        palettePanel.IsEnabled = false;

                        if (bSmartSettingsEditing)
                        {
                            printGrid.IsEnabled = true;
                            legend_GridControl.IsEnabled = true;
                        }
                        else
                            view.IsHitTestVisible = false;

                        bInit = true;
                    }
                    else
                    {
#if !DEBUG
                                var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w=="/* REP */);
                                txtMode.SetZIndex(0);
                                txtMode.Visibility = mode ? Visibility.Collapsed : Visibility.Visible;
                                if (mode == false)
                                {
                                    mainGrid.IsEnabled = false;
                                    //logLicense.Warn(Properties.Resources.NoReportLicense);
                                    iUFProjectManager?.AddLogEntity(Document, Properties.Settings.Default.DataAnalysisControl, 
                                    DateTime.UtcNow, Properties.Resources.NoReportLicense, System.Diagnostics.EventLogEntryType.Warning);
                                    return;
                                   }
#endif
                        diagram.EnableAxisXNavigation = !DisableZoomBehaviour;
                        diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
                        diagram.NavigationOptions = new NavigationOptions() { AxisXMaxZoomPercent = double.MaxValue };

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

                        SetTimeRange();

                        if (OperatingMode != OperatingMode.OnlyStop)
                            ((XYDiagram2D)chart.Diagram).Scroll += AutoStopPlayOnScroll;

                        if (AutomaticGeneralScale)
                            axisY.Visible = true;
                        
                        if (RunningOnServer)
                        {
                            toolbarSettings.IsVisible = false;
                            toolbarPrintSettings.Visibility = Visibility.Collapsed;
                            btnPrint.IsVisible = false;
                            AutoHideToolbar = false;

                            HideAllHidden();
                            RestoreChartFromSettings(false, true);
                        }
                        else
                        {
                            RestoreChartFromSettings(false, true);
                        }

                        InitPanels();

                        palettePanel.Content = new PaletteChooser(chart);
                        startTime.TouchDown += OnTouchDown;
                        endTime.TouchDown += OnTouchDown;
                        startTime.LostFocus += OnLostFocus;
                        endTime.LostFocus += OnLostFocus;

                        OverrideBaseProperties();

                        if (RereshAtuoHide(AutoHideToolbar, AutoHideToolbar))
                        {
                            toolbar.MouseEnter += toolbar_MouseEnter;
                            toolbar.MouseLeave += toolbar_MouseLeave;
                        }

                        bInit = true;
                        chart_BoundDataChanged(this, null);
                    }
                }
            };

            //Unloaded += (o, e) =>
            //{
            //    if (bLoaded)
            //    {
            //        bLoaded = false;

            //        DetachOverrideBaseProperties();

            //        if (!bDesignerMode)
            //        {
            //            RestoreAndSaveBeforeQuit();

            //            startTime.TouchDown -= OnTouchDown;
            //            endTime.TouchDown -= OnTouchDown;
            //            startTime.LostFocus -= OnLostFocus;
            //            endTime.LostFocus -= OnLostFocus;

            //            if (palettePanel.Content is IDisposable)
            //                (palettePanel.Content as IDisposable).Dispose();
            //            palettePanel.Content = null;
            //        }
            //    }
            //};
        }

        bool bOnInit;
        void InitTimeRange()
        {
            bOnInit = true;
            var editValue = cmbTimeRange.EditValue as LocalizedTimeRange;
            var timeRanges = GetItemSource();
            cmbTimeRange.DataContext = timeRanges;
            cmbTimeRange.EditValue = timeRanges[0];
            if (editValue != null)
            {
                var selectedValue = (from v in timeRanges where v.Value == editValue.Value select v).FirstOrDefault();
                cmbTimeRange.EditValue = selectedValue;
            }
            bOnInit = false;
        }

        List<LocalizedTimeRange> GetItemSource()
        {
            List<LocalizedTimeRange> timeRanges = new List<LocalizedTimeRange>();
            if (stringlist != null)
            {
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_None", stringlist, Properties.Resources.None), DateSpan.None)); 
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.Minute), DateSpan.Minute));
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.Hour), DateSpan.Hour));
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.Day), DateSpan.Day));
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.Week), DateSpan.Week));
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Month), DateSpan.Month));
                timeRanges.Add(new LocalizedTimeRange(TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", stringlist, Properties.Resources.Year), DateSpan.Year));
            }
            else
            {
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.None, DateSpan.None));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Minute, DateSpan.Minute));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Hour, DateSpan.Hour));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Day, DateSpan.Day));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Week, DateSpan.Week));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Month, DateSpan.Month));
                timeRanges.Add(new LocalizedTimeRange(Properties.Resources.Year, DateSpan.Year));
            }
            return timeRanges;
        }
                     
        void InitChart()
        {
            diagram.Series.Clear();
            ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Clear();
            Random r = new Random(DateTime.Now.Millisecond);
            var demoData = LoadData(r, Properties.Settings.Default.DemoMaxDays);
            if (StaticSeriesSettings == null || StaticSeriesSettings.Count == 0)
                AddChartLine("Value", "Value", "lineSeries", 1, Color.FromArgb(255, 65, 90, 120), false, false, 0.0, null, demoData, isvisible: false);
            else
                for (var i = 0; i < StaticSeriesSettings.Count; i++)
                {
                    var name = String.Format("Value{0}", i);
                    AddChartLine(name, name, StaticSeriesSettings[i].serieType.ToString(), StaticSeriesSettings[i].thickness, StaticSeriesSettings[i].color, StaticSeriesSettings[i].showaxis, StaticSeriesSettings[i].logarithmicYScale, StaticSeriesSettings[i].logarithmicbaseYScale, null, demoData, isvisible: StaticSeriesSettings[i].isVisible);
                    if (i < StaticSeriesSettings.Count - 1)
                        demoData = LoadData(r, Properties.Settings.Default.DemoMaxDays);
                }
            UpdateAxisRange();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!isTemplateApplied && RunningOnServer)
                SetWebAsset();
            isTemplateApplied = true;
        }

        #region RealTime Methods
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
                        serie.listValues.Clear();
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
                    if (mapKeySeries[key].listValues != null && mapKeySeries[key].listValues.Count > 0)
                        viewList[key].AddData(mapKeySeries[key].listValues.First().dValue); //updating realtime LastValue to last historical available value
                    viewList[key].collectionValues.PrependList(mapKeySeries[key].listValues, true);
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
            if (e.ScrollOrientation == XYDiagram2DScrollOrientation.AxisYScroll || StaticSeriesSettings == null || StaticSeriesSettings.Count == 0)
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

        string PrepareRealTimeSerie(string serie, SerieData data)
        {
            var id = data.guiId != null ? data.guiId : Guid.NewGuid().ToString();
            string keyname = string.Format("{0}_{1}", id, data.arrayindex);
            if (mapKeySeries == null)
                mapKeySeries = new Dictionary<string, SerieSettings>();
            if (!mapKeySeries.ContainsKey(keyname))
            {
                if (viewList == null)
                    viewList = new Dictionary<string, AnalysisDataGenerator>();
                var settings = new DataGeneratorSettings()
                {
                    ClientTimezoneOffset = ClientTimezoneOffset,
                    HDataCount = MaxRecords,
                    //ConnectionString = connectionString,
                    DeadBandInterval = RecordEvery,
                    DeadBandTimeFrame = ViewTimeFrame,
                    ArrayIndex = data.arrayindex,
                    //DlrSource = PenReferenceList[key].Dlrsource,
                    //DlrName = tablename,
                    //ColName = PenReferenceList[key].ColuName,
                    //UtcTimeColumnName = utccolumnname
                };
                var view = new AnalysisDataGenerator(keyname, settings, RecordEvery, UseAggregation);
                view.Error += ViewList_OnError;
                //view.HistoryLoaded += OnHistoryLoaded;
                viewList[keyname] = view;
                mapKeySeries[keyname] = settingStorage.mapSeries[serie];
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
                        foreach (var peniten in StaticSeriesSettings)
                        {
                            var key = peniten.guiId;
                            PrepareItem(key, peniten.arrayindex);
                            PrepareItem($"{condPrefix}{key}");
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
                if ((!matchChangedMap.Contains(key) || (mapHandlers != null && !mapHandlers.ContainsKey(key))) && (bInit || !OpcuaEntityReference[key].IsRelative))
                    PrepareExecution(key, arreayIndex);
                if (matchChangedMap.Contains(key) && !bInit)
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
            
            if (!IsInStop)
            {
                if (dpPlayValue == null ||
                    dpPlayValue.Status == DispatcherOperationStatus.Completed ||
                    dpPlayValue.Status == DispatcherOperationStatus.Aborted)
                {
                    dpPlayValue = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                    {
                        if (IsInStop)
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
                SetDateTimeScaleOptions(axisX, DateTimeGridAlignment.Second, DateTimeMeasureUnit.Millisecond);
                //SetTimeScaleAlignment(minDate, (DateTime)endDate, true);
        }

        void UpdateModel(string key, int arrayIndex, string humanReadable, NodeIdViewModel model)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose)
                    return;
                
                if (key.StartsWith(condPrefix))
                {
                    UpdateCondMonitoredValue(key);
                    return;
                }

                if (viewList != null && viewList.ContainsKey($"{key}_{arrayIndex}"))
                    UpdateReferences(key, model.nodeId.ToString());

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
            });
        }

        void UpdateCondMonitoredValue(string key)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose)
                    return;

                if (IsStatusGood(key))
                {
                    if (mapHandlers != null && mapHandlers.ContainsKey(key))
                        mapHandlers[key].ValueChanged -= PenItem_ValueChanged;

                    SetDataSources();
                }
            });
        }

        void UpdateMonitoredValue(string key, object newValue)
        {
            bool isArray = newValue is IList;
            if (isArray)
            {
                var dataCollection = newValue as List<MonitoredItemViewModel.DataObject>;
                if (dataCollection != null && dataCollection.Count > 0)
                {
                    if (viewList != null && viewList.ContainsKey(key))
                    {
                        for (ushort ii = 0; ii < dataCollection.Count; ii++)
                        {
                            viewList[key].AddData(dataCollection[ii].Value);
                        }
                    }
                }
            }
            else if (viewList != null && viewList.ContainsKey(key))
            {
                var newDValue = newValue as DataValue;
                viewList[key].AddData(newDValue.Value);
            }
        }

        void UpdateLegend(string k)
        {
            if (settingStorage != null && settingStorage.mapSeries != null && mapKeySeries.ContainsKey(k) && mapKeySeries[k] != null && viewList.ContainsKey(k))
            {
                mapKeySeries[k].DValue = viewList[k].LastValue;
                mapKeySeries[k].minValue = viewList[k].MinValue;
                mapKeySeries[k].maxValue = viewList[k].MaxValue;
                mapKeySeries[k].averageValue = viewList[k].AvgValue;
                try
                {
                    currentValues.Set(k, Convert.ToDouble(viewList[k].LastValue.Value, System.Globalization.CultureInfo.InvariantCulture));
                }
                catch { }
            }
            bSettingLegendSource = true;
            legend_GridControl.RefreshData();
            bSettingLegendSource = false;
        }

        void OnDiagramScroll(object sender, XYDiagram2DScrollEventArgs e)
        {
            if (e.ScrollOrientation == XYDiagram2DScrollOrientation.AxisYScroll)
                bYScrolling = true;
        }

        DispatcherOperation dpAxisScaleChanged;
        bool bLoopingAxes;
        void chart_AxisScaleChanged(object sender, AxisScaleChangedEventArgs e)
        {
            if (bLoopingAxes)
                return;

            if (bDesignmode || settingStorage == null || bYScrolling || bDispose)
            {
                bYScrolling = false;
                return;
            }

            AxisY2D ax = e.Axis as AxisY2D;
            bool bSkipAxis = !((XYDiagram2D)chart.Diagram).SecondaryAxesY.Contains(ax) && (!AutomaticGeneralScale || (AutomaticGeneralScale && ax != axisY));

            if (ax == null || bSkipAxis)
                return;

            if (!axisRedrawDispatched.Contains(ax))
            {
                axisRedrawDispatched.Add(ax);
                if (axisRedrawDispatched.Count == 1)
                {
                    dpAxisScaleChanged = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                    {
                        if (bDispose || bDesignmode || settingStorage == null || bYScrolling)
                        {
                            bYScrolling = false;
                            axisRedrawDispatched.Clear();
                            return;
                        }
                        bLoopingAxes = true;
                        try
                        {
                            foreach (var axis in axisRedrawDispatched)
                            {
                                double? actualMaxValue = null;
                                double? actualMinValue = null;

                                try
                                {
                                    actualMaxValue = Convert.ToDouble(axis.ActualWholeRange.ActualMaxValue);
                                    actualMinValue = Convert.ToDouble(axis.ActualWholeRange.ActualMinValue);
                                }
                                catch
                                {
                                }

                                if (actualMaxValue.HasValue && actualMinValue.HasValue)
                                {
                                    SerieSettings serie = null;
                                    if (!AutomaticGeneralScale)
                                        serie = (from s in settingStorage.mapSeries.Values where s.AuthomaticScale && axis.Name == GetSanitizedName(s.Name) select s).FirstOrDefault();
                                    if (serie != null && !axis.Logarithmic || AutomaticGeneralScale && !LogarithmicYScale)
                                        axis.ActualWholeRange.SideMarginsValue = ((axis.ActualWholeRange.ActualMaxValueInternal - axis.ActualWholeRange.SideMarginsValue) - (axis.ActualWholeRange.ActualMinValueInternal + axis.ActualWholeRange.SideMarginsValue)) * ScalePaddingFactor / 100;
                                }
                            }
                        }
                        finally
                        {
                            bLoopingAxes = false;
                        }
                        axisRedrawDispatched.Clear();
                    });
                }
            }
        }
        
        private void Play(object sender, RoutedEventArgs e)
        {
            bChangingPlayPause = true;
            try
            {
                if (!IsInStop || StaticSeriesSettings == null || StaticSeriesSettings.Count == 0)
                    return;
                IsInStop = false;
                PlayExecute();
            }
            finally
            {
                bChangingPlayPause = false;
            }
        }

        void ChangeDateTimeScale(AxisX2D axisX2D, Type newScale /*, bool bResetTimeScaleAlignment = false*/)
        {
            if (!bDiscreteTimeMode || axisX2D.DateTimeScaleOptions.GetType() == newScale)
                return;
            if (newScale == typeof(ContinuousDateTimeScaleOptions))
            {
                axisX2D.DateTimeScaleOptions = new ContinuousDateTimeScaleOptions();
                axisX2D.SetBinding(ContinuousDateTimeScaleOptions.AutoGridProperty, new Binding("XAutoGrid") { Source = this });
            }
            else if (newScale == typeof(ManualDateTimeScaleOptions))
            {
                var oldVRmin = axisX2D.ActualVisualRange?.ActualMinValue;
                var oldVRmax = axisX2D.ActualVisualRange?.ActualMaxValue;
                axisX2D.DateTimeScaleOptions = new ManualDateTimeScaleOptions() { AggregateFunction = AggregateFunction.None, MeasureUnitMultiplier = MeasureUnitMultiplier };
                axisX2D.SetBinding(ManualDateTimeScaleOptions.AutoGridProperty, new Binding("XAutoGrid") { Source = this });
                //if (bResetTimeScaleAlignment && oldVRmin is DateTime && oldVRmax is DateTime)
                //    SetTimeScaleAlignment((DateTime)oldVRmin, (DateTime)oldVRmax);
            }
        }

        void PlayExecute()
        {
            ClearAllCompareSeries();

            FilterType = DateSpan.None;
            axisX.WholeRange.SideMarginsValue = 0;

            //ChangeDateTimeScale(typeof(ContinuousDateTimeScaleOptions));


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
                                bFirstPlay = false;
                                viewList[key].collectionValues.PrependList(diagramSerie.DataSource as List<MyDataValue>, true);
                            }
                            diagramSerie.DataSource = viewList[key].collectionValues;
                        }
                    }

                    mapKeySeries[key].lineSerie.BeginInit();
                    mapKeySeries[key].lineSerie.DataSource = viewList[key].collectionValues;
                    mapKeySeries[key].lineSerie.EndInit();
                }
            }
            axisX.WholeRange.MinValue = minValueDate;
            if (secondaryAxisX2D != null)
                secondaryAxisX2D.WholeRange.MinValue = minValueDate;
            timer_Tick(this, null);
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            bChangingPlayPause = true;
            try
            {
                if (IsInStop || StaticSeriesSettings == null || StaticSeriesSettings.Count == 0)
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
            ClearAllCompareSeries();

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
            //ChangeDateTimeScale(typeof(ManualDateTimeScaleOptions), true);
        }
        #endregion

        void ApplyDateTimeFormat()
        {
            var format = GetDateTimeFormat();
            string textPattern = String.Format("{{A{0}}}", String.Format(":{0}", format)); 
            axisX.Label.TextPattern = textPattern;
            if (secondaryAxisX2D != null)
                secondaryAxisX2D.Label.TextPattern = textPattern;

            if (!bDesignmode)
            {
                string pattern = String.Format("{{A:{0}}}", format);
                axisX.CrosshairAxisLabelOptions.Pattern = pattern;
                if (secondaryAxisX2D != null)
                    secondaryAxisX2D.CrosshairAxisLabelOptions.Pattern = pattern;
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
            OnMouseLeave();
        }

        private void toolbar_MouseEnter(object sender, MouseEventArgs e)
        {
            OnMouseEnter();
        }

        void OnMouseEnter()
        {
            var sbOver = TryFindResource("MouseOverOpacity") as Storyboard;
            sbOver.Begin();
        }

        void OnMouseLeave()
        {
            var sbLeave = TryFindResource("MouseLeaveOpacity") as Storyboard;
            sbLeave.Begin();
        }
        /// <summary>
        /// Use this method to set custom time range for data extraction
        /// </summary>
        /// <param name="delta"></param>
        public void SetTimeRange(TimeSpan delta)
        {
            settingStorage.DateTimeEnd = DateTime.Now;
            settingStorage.DateTimeStart = settingStorage.DateTimeEnd.Subtract(delta);

            if (settingStorage.DateTimeStart < minDateTimeValue)
                settingStorage.DateTimeStart = minDateTimeValue;

            bUserInteractionSettings = true;
            FilterType = DateSpan.None;
            bUserInteractionSettings = false;
            CheckEnabledButtons();

            SelectTimeRangeCombo(delta);
        }

        /// <summary>
        /// Use this method to set custom time range for data extraction
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        public void SetTimeRange(DateTime dateStart, DateTime dateEnd)
        {
            if (dateStart < minDateTimeValue)
                dateStart = minDateTimeValue;

            if (dateEnd > maxDateTimeValue)
                dateEnd = maxDateTimeValue;

            settingStorage.DateTimeStart = dateStart;
            settingStorage.DateTimeEnd = dateEnd;

            bUserInteractionSettings = true;
            FilterType = DateSpan.None;
            bUserInteractionSettings = false;
            CheckEnabledButtons();

            SelectTimeRangeCombo(dateEnd - dateStart);
        }

        /// <summary>
        /// Use this method to set custom time range for data extraction
        /// </summary>
        /// <param name="filterType"></param>
        public bool SetTimeRange(DateSpan filterType)
        {
            if (settingStorage == null || bDispose)
                return false;
            
            DateTime date1;
            DateTime date2;

            CheckEnabledButtons();
            SelectTimeRangeCombo(filterType);

            DateTime endDate = DateTime.Now;
            if (axisX.ActualVisualRange != null && axisX.ActualVisualRange.ActualMaxValue is DateTime) {
                var visualRangeEnd = (DateTime)axisX.ActualVisualRange.ActualMaxValue;
                if (visualRangeEnd != DateTime.MaxValue)
                    endDate = visualRangeEnd;
            }
            bool bExactTimerange = false;

            switch (filterType)
            {
                case DateSpan.All:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.All, true/*UseAbsoluteRanges*/);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Minute:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Minute);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                case DateSpan.Hour:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Hour);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                case DateSpan.Day:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Day);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                case DateSpan.Week:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Week);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                case DateSpan.Month:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Month);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                case DateSpan.Year:
                    DateRangeHelpers.SetStartDate(out date1, endDate, DateSpan.Year);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = endDate;
                    bExactTimerange = true;
                    break;
                default:
                    break;
            }
            return bExactTimerange;
        }

        void SetTimeRange()
        {
            SetTimeRange(FilterType);
        }

        /// <summary>
        /// Use this method to reload datas
        /// </summary>
        public void ReloadData()
        {
            SetDataSources();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!bDispose && SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
        }

        private void InitRuntimeConfig()
        {
            designPenList = StaticSeriesSettings != null ? new SerieDataList(StaticSeriesSettings) : new SerieDataList();

            helper = new Helper(Document, this as ISettingsHelper);
            helper.RefreshCurrentUser();

            designDockLayout = DockLayout;
            designListLayout = ListViewLayout;
            designGridLayout = GridLayout;
            designFilterType = FilterType;
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
                    FilterType = designFilterType,
                    UseAbsoluteRanges = designUseAbsoluteRanges,
                    ReadOnly = true
                });
            else
            {
                defaultsetting.ReadOnly = true;
                defaultsetting.GridLayout = designGridLayout;
                defaultsetting.FilterType = designFilterType;
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
        internal string stringPlaceolder = "DataAnalysis";
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
                timeRangePanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TimeRange", stringlist, Properties.Resources.TimeRange);
                gridPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTitle", stringlist, Properties.Resources.GridTitle);

                Text1.Text = StartDateTitle = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist, Properties.Resources.StartDate);
                Text2.Text = EndDateTitle = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndDate);

                configMemory.EditValue = ActualConfig;
                todayButton1.ToolTip = todayButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TodayBtnTooltip", stringlist, Properties.Resources.TodayBtnTooltip);
                tomorrowButton1.ToolTip = tomorrowButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TomorrowBtnTooltip", stringlist, Properties.Resources.TomorrowBtnTooltip);
                btnFetchData.ToolTip = btnFetchData.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_FetchTitle", stringlist, Properties.Resources.FetchTitle);
                btnClearRecent.ToolTip = btnClearRecent.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ClearRecentTitle", stringlist, Properties.Resources.ClearRecent);

                btnRefresh.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshTitle", stringlist, Properties.Resources.RefreshTitle);
                btnShowCrossHair.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CrossHairTitle", stringlist, Properties.Resources.CrossHairTitle);
                chkShowStatistics.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsTitle", stringlist, Properties.Resources.StatisticsTitle);
                //chkUseAbsoluteRanges.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsTitle", stringlist, Properties.Resources.UseAbsoluteRangesTitle);
                chkUseAggregationTitle.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AggregationTitle", stringlist, Properties.Resources.UseAggregationTitle);
                btnShowLabels.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LabelTitle", stringlist, Properties.Resources.LabelTitle);
                btnExpand.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandTitle", stringlist, Properties.Resources.ExpandTitle);
                btnPrev.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevTitle", stringlist, Properties.Resources.PrevTitle);
                btnNext.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextTitle", stringlist, Properties.Resources.NextTitle);
                btnAll.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AllTitle", stringlist, Properties.Resources.AllTitle);
                btnMin.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MinTitle", stringlist, Properties.Resources.MinTitle);
                btnHour.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HourTitle", stringlist, Properties.Resources.HourTitle);
                btnDay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayTitle", stringlist, Properties.Resources.DayTitle);
                btnWeek.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeekTitle", stringlist, Properties.Resources.WeekTitle);
                btnMonth.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthTitle", stringlist, Properties.Resources.MonthTitle);
                btnYear.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_YearTitle", stringlist, Properties.Resources.YearTitle);
                btnPrintGrid.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintTitle", stringlist, Properties.Resources.PrintTitle);
                btnPrint.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintTitle", stringlist, Properties.Resources.PrintTitle);
                btnPause.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PauseBtnTitle", stringlist, Properties.Resources.Pause);
                btnPlay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PlayBtnTitle", stringlist, Properties.Resources.Play);

                maxRecord.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MaxRecordTitle", stringlist, Properties.Resources.MaxRecordTitle);
                btnNow.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NowTitle", stringlist, Properties.Resources.NowTitle);

                startTextTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist, Properties.Resources.StartDate);
                endTextTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndDate);
                recentTimeRanges.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecentTimeRanges", stringlist, Properties.Resources.RecentTimeRanges);

                var lastTextPattern = TextPattern;
                TextPattern = "";
                TextPattern = lastTextPattern;

                lastTextPattern = AxisYTextPattern;
                AxisYTextPattern = "";
                AxisYTextPattern = lastTextPattern;

                ApplyDateTimeFormat();

                visibleTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_VisibleColumn", stringlist, Properties.Resources.VisibleTitle);
                statisticsLineTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsLineColumn", stringlist, Properties.Resources.StatisticsLineTitle);
                colorTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ColorColumn", stringlist, Properties.Resources.ColorTitle);
                tagNameTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TagNameColumn", stringlist, Properties.Resources.TagName);
                historicalNameTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HistoricalNameColumn", stringlist, Properties.Resources.HistoricalName);
                nameTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NameTitleColumn", stringlist, Properties.Resources.NameTitle);
                eUnitTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EUnitColumn", stringlist, Properties.Resources.EUnit);
                pointDateTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PointDateColumn", stringlist, Properties.Resources.PointDate);
                pointValueTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PointValueColumn", stringlist, Properties.Resources.PointValue);
                totDataSourceTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalDataSourceColumn", stringlist, Properties.Resources.TotalDataSource);
                totCompressedPointTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TotalCompressedPointColumn", stringlist, Properties.Resources.TotalCompressedPoint);
                compressRatioTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CompressRatioColumn", stringlist, Properties.Resources.CompressRatio);
                minTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MinColumn", stringlist, Properties.Resources.Min);
                maxTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MaxColumn", stringlist, Properties.Resources.Max);
                statMinTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticMinColumn", stringlist, Properties.Resources.StatisticMin);
                statMaxTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticMaxColumn", stringlist, Properties.Resources.StatisticMax);
                avgTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AverageColumn", stringlist, Properties.Resources.Average);
                medianTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MedianColumn", stringlist, Properties.Resources.Median);
                varianceTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_VarianceColumn", stringlist, Properties.Resources.Variance);
                stdDevTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StandardDevColumn", stringlist, Properties.Resources.StandardDev);
                dValueTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LiveValueColumn", stringlist, Properties.Resources.DValue);
                showAxisTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ShowAxisTitle", stringlist, Properties.Resources.ShowAxisTitle);
                virtualPointsTitle.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddVirtualPointsTitle", stringlist, Properties.Resources.AddVirtualPointsTitle);

                gridTagName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTagName", stringlist, Properties.Resources.GridTagName);
                maxMeasUnitMultiplier.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MeasureUnitMultiplier", stringlist, Properties.Resources.MeasureUnitMultiplier);

                InitTimeRange();

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

            foreach (var serie in settingStorage.mapSeries.Keys)
            {
                string _LinkedPenName = serie;
                _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(serie, stringlist, serie);

                settingStorage.mapSeries[serie].DName = _LinkedPenName;
            }
            SetLegendSource();
        }
        private void InitPanels()
        {
            if (!RunningOnServer)
                timeRangePanel.DataContext = settingStorage;
            startTextTitle.DataContext = settingStorage;
            endTextTitle.DataContext = settingStorage;
        }
        #endregion

        #region IDropRecordAware
        public bool OnDropRecord(object dropObject)
        {
            bool ret = true;
            var series = StaticSeriesSettings != null ? StaticSeriesSettings.ToList() : new SerieDataList(); 
            if (dropObject is DataLoggerModel.DataLoggerColumn)
            {
                if(!AddDLRPen(series, dropObject as DataLoggerModel.DataLoggerColumn))
                    ret = false;
            }
            else if (dropObject is DataLoggerModel.DataLoggerSettings)
            {
                (dropObject as DataLoggerModel.DataLoggerSettings).Columns.ForEach(column =>
                {
                    if (!AddDLRPen(series, column))
                        ret = false;
                });
            }
            else
                ret = false;

            if(ret)
                StaticSeriesSettings = new SerieDataList(series);

            return ret;
        }

        private bool AddDLRPen(List<SerieData> series, DataLoggerModel.DataLoggerColumn column)
        {
            if (column == null)
                return false;

            var data = GetDefaultSerie(series);
            data.dlrsource = true;
            data.tagName = column.ColumnName;
            data.tagReference = SmartControlUtilities.Helpers.GetTagReference(data.tagName, Document);
            data.historicalName = column.DataLoggerReference.Name;
            data.sourcetimestampColumnName = $"{column.ColumnName}_";
            series.Add(data);

            return true;
        }
        internal SerieData GetDefaultSerie(List<SerieData> serieDatas)
        {
            int i = serieDatas.Count;
            var _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, i);

            var list = (from s in serieDatas select s.title).ToList();
            while (list.Contains(_name))
            {
                _name = string.Format("{0}{1}", Properties.Resources.PenBaseName, ++i);
            }

            if (i > 255)
                i = 0;
            var random = new Random(i);
            var c = Color.FromRgb((byte)random.Next(255),
                                            (byte)random.Next(255),
                                            (byte)random.Next(255));

            var newData = new SerieData
            {
                title = _name,
                color = c,
                serieType = LineType.lineSeries,
                thickness = 1, 
                logarithmicbaseYScale = 10.0, 
                showaxis = false, 
                logarithmicYScale = false, 
                authomaticscale = true, 
                min = 0.0, 
                max = 100.0, 
                isVisible = true, 
                isSet = true, 
                guiId = Guid.NewGuid().ToString(), 
                useeuminmax = false, 
                addVirtualPoints = false };
            return newData;
        }
        #endregion

        #region EventHandlers
        public event EventHandler ControlLoaded;
        bool bControlLoaded;
        void OnControlLoaded()
        {
            if (OperatingMode != OperatingMode.OnlyStop)
            {
                SetRealTimeSource();
                if (PlayOnStart)
                    IsInStop = false;
            }
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
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);

                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);

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
                mapDataTemplates.Add(XMinorCountProperty, dt);
                mapDataTemplates.Add(YMinorCountProperty, dt);
                mapDataTemplates.Add(XMajorCountProperty, dt);
                mapDataTemplates.Add(YMajorCountProperty, dt);

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
                //    return OperatingMode != OperatingMode.OnlyStop;
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
            MapSerieAxis.Clear();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!bDispose)
            {
                bDispose = true;

                if (tokenSource != null)
                    tokenSource.Cancel();

                if (pendingTask.Count > 0)
                {
                    pendingTask.Values.ToList().ForEach(l =>
                    {
                        Task.WaitAll(l.ToArray());
                    });
                }

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

                if (StaticSeriesSettings != null)
                    StaticSeriesSettings.Clear();

                configMemory.DataContext = null;
                legend_GridControl.ItemsSource = null;
                listBoxRecent.ItemsSource = null;

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

                ((XYDiagram2D)chart.Diagram).Scroll -= AutoStopPlayOnScroll;

                axisRedrawDispatched.Clear();

                if (disposing)
                {
                    if (dpPlayValue != null &&
                        dpPlayValue.Status != DispatcherOperationStatus.Aborted &&
                        dpPlayValue.Status != DispatcherOperationStatus.Completed)
                        dpPlayValue.Abort();

                    if (dpBoundDataChanged != null &&
                        dpBoundDataChanged.Status != DispatcherOperationStatus.Aborted &&
                        dpBoundDataChanged.Status != DispatcherOperationStatus.Completed)
                        dpBoundDataChanged.Abort();

                    if (dpAxisScaleChanged != null &&
                        dpAxisScaleChanged.Status != DispatcherOperationStatus.Aborted &&
                        dpAxisScaleChanged.Status != DispatcherOperationStatus.Completed)
                        dpAxisScaleChanged.Abort();

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

                    if (palettePanel.Content is IDisposable)
                        (palettePanel.Content as IDisposable).Dispose();

                    gridControl.MouseDown -= lengedGrid_MouseDown;
                    gridControl.ItemsSource = null;
                    if (gridControl is IDisposable)
                        (gridControl as IDisposable).Dispose();

                    if (dockManager.DockController != null && dockManager.DockController is IDisposable)
                        (dockManager.DockController as IDisposable).Dispose();
                    if (dockManager is IDisposable)
                        (dockManager as IDisposable).Dispose();

                    if (dl != null)
                    {
                        dl.Dispose();
                    }
                    if (MapToHistoricalUnitOfWork != null)
                    {
                        foreach (var _ufw in MapToHistoricalUnitOfWork.Keys)
                        {
                            if (MapToHistoricalUnitOfWork[_ufw] != null)
                            {
                                MapToHistoricalUnitOfWork[_ufw].Disconnect();
                                MapToHistoricalUnitOfWork[_ufw].Dispose();
                            }
                        }
                        MapToHistoricalUnitOfWork.Clear();
                    }
                    if (MapToHistoricalDataLayer != null)
                    {
                        foreach (var _dl in MapToHistoricalDataLayer.Keys)
                        {
                            if (MapToHistoricalDataLayer[_dl] != null)
                            {
                                MapToHistoricalDataLayer[_dl].Dispose();
                            }
                        }
                        MapToHistoricalDataLayer.Clear();
                    }
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

                dl = null;
                saveAutoHiddenStream = null;
                palettePanel.Content = null;

                DetachOverrideBaseProperties();

                toolbar.MouseEnter -= toolbar_MouseEnter;
                toolbar.MouseLeave -= toolbar_MouseLeave;

                if (!bDesignmode)
                    RestoreAndSaveBeforeQuit();

                startTime.TouchDown -= OnTouchDown;
                endTime.TouchDown -= OnTouchDown;
                startTime.LostFocus -= OnLostFocus;
                endTime.LostFocus -= OnLostFocus;
                gridControl.ItemsSource = null;

                timeRangePanel.DataContext = null;
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
                                settingStorage.mapSeries[key].listValues.Clear();
                            settingStorage.mapSeries[key].listValues = null;
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
                            serie.listValues.Clear();
                        serie.listValues = null;
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
                        _datasource.ForEach(x => x = null);
                        _datasource.Clear();
                    }
                    _datasource = null;
                    serie.Tag = null;
                    //serie.Points.Clear();
                    serie.EndInit();
                }

                diagram.SecondaryAxesY.Clear();
                diagram.SecondaryAxesX.Clear();
                diagram.Series.Clear();
                ClearConstantLines();

                chart.PreviewMouseWheel -= chart_PreviewMouseWheel;
                diagram.MouseWheel -= diagram_MouseWheel;
                diagram.Zoom -= diagram_Zoom;

                chart.PreviewKeyDown -= chart_PreviewKeyDown;
                chart.CustomDrawSeries -= chart_CustomDrawSeries;
                chart.PreviewMouseDown -= chart_MouseDown;
                chart.BoundDataChanged -= chart_BoundDataChanged;
                chart.AxisScaleChanged -= chart_AxisScaleChanged;

                nearestPoints.Clear();

                if (listSeries != null)
                    listSeries.Clear();

                if (lineSeries != null)
                    lineSeries.Clear();

                if (stringManager != null)
                    stringManager.CultureChanged -= StringManager_CultureChanged;

                lock (dlrSettings)
                {
                    dlrSettings.Clear();
                }

                MapToHistoricalDataLayer = null;
                MapToHistoricalUnitOfWork = null;
                if (MapToDatalogerConnectsions != null)
                {
                    MapToDatalogerConnectsions.Clear();
                    MapToDatalogerConnectsions = null;
                }
                if (MapToHistoricalConnectsions != null)
                {
                    MapToHistoricalConnectsions.Clear();
                    MapToHistoricalConnectsions = null;
                }
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
                
                if (dpUpdateLayout != null &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Completed)
                    dpUpdateLayout.Abort();
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
        public static readonly DependencyProperty UseAbsoluteRangesProperty = DependencyProperty.Register("UseAbsoluteRanges", typeof(bool), typeof(DataAnalisysRT), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseAbsoluteRangesChanged), new CoerceValueCallback(OnCoerceUseAbsoluteRanges)));

        private static object OnCoerceUseAbsoluteRanges(DependencyObject o, object value)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
            if (control != null)
                return control.OnCoerceUseAbsoluteRanges((bool)value);
            else
                return value;
        }

        private static void OnUseAbsoluteRangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisysRT control = o as DataAnalisysRT;
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
                SetTimeRange();
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
        void CreateCustomConnectionStringDataLayer(String hstname, int commandTimeout)
        {
            if (MapToHistoricalDataLayer != null && MapToHistoricalDataLayer.ContainsKey(hstname) && MapToHistoricalDataLayer[hstname] != null)
                return;

            if (MapToHistoricalDataLayer == null)
                MapToHistoricalDataLayer = new Dictionary<String, IDataLayer>();

            if (MapToHistoricalConnectsions != null && MapToHistoricalConnectsions.ContainsKey(hstname) && !String.IsNullOrEmpty(MapToHistoricalConnectsions[hstname]))
            {
                IDataLayer _dl = CreateDataLayer(MapToHistoricalConnectsions[hstname], commandTimeout);
                if (_dl != null)
                {
                    if (!MapToHistoricalDataLayer.ContainsKey(hstname))
                        MapToHistoricalDataLayer.Add(hstname, _dl);
                    else
                    {
                        MapToHistoricalDataLayer[hstname].Dispose();
                        MapToHistoricalDataLayer[hstname] = _dl;
                    }
                }
                else
                {
                    if (MapToHistoricalDataLayer == null)
                        MapToHistoricalDataLayer = new Dictionary<String, IDataLayer>();

                    if (!MapToHistoricalDataLayer.ContainsKey(hstname))
                        MapToHistoricalDataLayer.Add(hstname, dl);
                    else if (MapToHistoricalDataLayer[hstname] != dl)
                    {
                        MapToHistoricalDataLayer[hstname].Dispose();
                        MapToHistoricalDataLayer[hstname] = dl;
                    }
                }
            }
        }

        void CreateConnectionStringDataLayer(String connStr, int commandTimeout)
        {
            //if (ufw != null)
            //    return;

            if (dl != null)
                return;

            dl = CreateDataLayer(connStr, commandTimeout);
            //if (dl != null)
            //    ufw = new UnitOfWork(dl);

            var helper = new ConnectionStringParser(connStr);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (!String.IsNullOrEmpty(providerType))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connStr);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(connStr);
            }
            else
            {
                defaultDataProvider = helper.GetPartByName("DataProvider");
                helper.RemovePartByName("DataProvider");
                defaultConnectionString = helper.GetConnectionString();
            }
        }

        private void SetWebAsset()
        {
            cmbTimeRange.IsEnabled = false;
            //btnChartDesigner.IsEnabled = false;
            btnShowCrossHair.IsEnabled = false;
            btnShowLabels.IsEnabled = false;
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
                legend_GridControl.FontSize = aggregatedTableName.FontSize = aggregatedTableTitle.FontSize = FontSize;

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
            //startTime.TouchDown -= OnTouchDown;
            //endTime.TouchDown -= OnTouchDown;
            //startTime.LostFocus -= OnLostFocus;
            //endTime.LostFocus -= OnLostFocus;
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
            if (!bHidden)
                HideAllHidden();
            else
                RestoreAllHidden();

            var btn = sender as DevExpress.Xpf.Bars.BarCheckItem;
            btn.IsChecked = bHidden;
        }

        void HelpUserChoose()
        {
            dockManager.Activate(legendPanel);
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

        List<String> listVariables;
        public List<String> ListVariables
        {
            get
            {
                if (listVariables != null)
                    return listVariables;
                using (var cursor = new WaitCursor())
                {
                    //CheckUfw(true);
                    //SetBusy(true);
                    var connStr = ConnectionString;
                    if (String.IsNullOrEmpty(connStr))
                        connStr = ConnectionStringDesignMode;
                    //var task1 = Task.Factory.StartNew(() =>
                    //{
                    //    try
                    //    {
                    //        return GetListVariables(connStr);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        return null;
                    //    }
                    //});
                    //task1.Wait();
                    //SetBusy(false);
                    //CheckUfw();

                    //listVariables = task1.Result;
                    //return listVariables;

                    connStr = XpoHelpers.XpoHelper.NormalizeConnectionString(connStr, Document?.rootBase);
                    listVariables = GetListVariables(connStr);
                    //CheckUfw();
                    return listVariables;
                }
            }
        }

        List<String> GetListVariables(String connStr)
        {
            if (String.IsNullOrEmpty(connStr))
                return null;

            var list = new List<String>();
            CreateConnectionStringDataLayer(connStr, CommandTimeout);
            //if (ufw == null)
            //    return new List<String>();
            if (dl == null)
                return list;

            List<UFUAAuditDataLog> ret = null;
            try
            {
                using (UnitOfWork _ufw = new UnitOfWork(dl))
                {
                    if (_ufw == null)
                        return list;
                    ret = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw) //.AsParallel()
                           orderby entry.Name ascending, entry.Oid descending
                           select entry).ToList();
                }
            }
            catch (Exception ex)
            {
                return list;
            }

            ret.ForEach(entry =>
            {
                var cleanedName = entry.Name.Replace('.', '/'); //regex.Replace(value, "",1);

                if (MapToListVariablesNodeId == null)
                    MapToListVariablesNodeId = new Dictionary<string, string>();

                if (!MapToListVariablesNodeId.ContainsKey(cleanedName))
                {
                    MapToListVariablesNodeId.Add(cleanedName, entry.NodeId);
                    list.Add(string.Format("{1} ({0})", entry.HistoricalName, cleanedName));
                }
            });
            return list;
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
                visualRangeBeforeAggregationToggle = null;
                if (bIsFetching)
                    bIsFetching = false;
            }
        }
        //void CheckUfw(bool bReset = false)
        //{
        //    if (bReset)
        //    {
        //        nullconnectionContent.Visibility = Visibility.Collapsed;
        //        return;
        //    }

        //    if (dl != null)
        //    {
        //        nullconnectionContent.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        var be = nullconnectionContent.GetBindingExpression(TextBlock.TextProperty);
        //        if (be != null)
        //            be.UpdateTarget();
        //        nullconnectionContent.Visibility = Visibility.Visible;
        //    }
        //}

        SecondaryAxisY2D GetSerieYAxis(string serieName)
        {
           return MapSerieAxis?[GetSanitizedName(serieName)];
        }
        void lengedGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = gridControl.View.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle) return;
            if (rowHandle == legendSelectedRowHandle && (legend_GridControl.ItemsSource as List<SerieSettings>).Count > rowHandle)
            {
                var settings = (legend_GridControl.ItemsSource as List<SerieSettings>)[rowHandle];
                var serie = (from c in diagram.Series.OfType<Series>() where c.DisplayName == settings.Name select c).FirstOrDefault();
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
                    SetPrintGridDataSource(serie);
                else
                    SetPrintGridDataSource();
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
                if(serie.DataSource is IList<MyDataValue> && (serie.DataSource as IList<MyDataValue>) != null)
                    realPoints.AddRange(serie.DataSource as IList<MyDataValue>);
                if (serie.DataSource is ReadOnlyList<MyDataValue>)
                {
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
                realPoints.ForEach(point => { dataSource.Add(new GridDataValue(point.SourceTimeStamp, point.dValue, serie.DisplayName)); });
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
            if (!bInit || !bLoaded || bDispose)
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
             where c.DisplayName == settings.Name
					|| c.DisplayName == ($"{settings.Name} - {minTag}")
					|| c.DisplayName == ($"{settings.Name} - {maxTag}")
					|| c.DisplayName == ($"{settings.Name} - {avgTag}")
             select c).ToList().ForEach(lsFound =>
             {
                 string tag = lsFound.Tag as String;
                 var name = lsFound.DisplayName.Replace(String.Format(" - {0}", tag), "");

                 // lsFound.Animate();
                 if (lsFound is LineSeries2D)
                 {
                     //lsFound.Tag = (lsFound as LineSeries2D).LineStyle;
                     (lsFound as LineSeries2D).LineStyle.Thickness = Math.Max(1, SelectedPenThickness);
                     (lsFound as LineSeries2D).LineStyle.LineJoin = PenLineJoin.Bevel;
                 }
                 else if (lsFound is AreaSeries2D)
                 {
                     //lsFound.Tag = (lsFound as AreaSeries2D).Border;
                     //(lsFound as AreaSeries2D).Border = new SeriesBorder();
                     (lsFound as AreaSeries2D).Border.LineStyle.Thickness = Math.Max(1, SelectedPenThickness);
                     (lsFound as AreaSeries2D).Border.LineStyle.LineJoin = PenLineJoin.Bevel;
                 }
                 if (lsFound.DisplayName == settings.Name || lsFound.Tag as String == avgTag)
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
            chart.BeginInit();
            try
            {
                InitServerDocument();

                InitSettingsForSeriesManagement();

                if (OperatingMode != OperatingMode.OnlyStop)
                    RestartRealTime(bOnLoad);
                else
                {
                    try
                    {
                        if (OpcuaEntityReference.Count > 0)
                            foreach (var key in OpcuaEntityReference.Keys)
                            {
                                var pen = (from p in StaticSeriesSettings where p.guiId == key select p).FirstOrDefault();
                                if (!OpcuaEntityReference[key].IsRelative && !matchChangedMap.Contains(key))
                                    PrepareExecution(key, !key.StartsWith(condPrefix) ? pen?.arrayindex ?? -1 : -1);
                            }
                    }
                    catch (Exception)
                    {
                    }
                }

                var serie_keynames = new List<string>();
                if (StaticSeriesSettings != null && StaticSeriesSettings.Count > 0)
                {
                    foreach (var data in StaticSeriesSettings)
                    {
                        var serie = UpdateSettingSorage(data);
                        if (OperatingMode != OperatingMode.OnlyStop)
                            serie_keynames.Add(PrepareRealTimeSerie(serie, data));
                    }
                }
                bDiscreteTimeMode = (from string key in settingStorage.mapSeries.Keys where settingStorage.mapSeries[key].serieTypeLine == "barSideSeries" select key).FirstOrDefault() != null;
                listSeries.Clear();
                lineSeries.Clear();
                bool needToFetchData = false;
                ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Clear();

                ChangeDateTimeScale(axisX, typeof(ManualDateTimeScaleOptions));

                if (secondaryAxisX2D != null)
                    ChangeDateTimeScale(secondaryAxisX2D, typeof(ManualDateTimeScaleOptions));

                var i = 0;
                foreach (var serie in settingStorage.mapSeries.Keys)
                {
                    listSeries.Add(serie);

                    string _LinkedPenName = serie;
                    _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(serie, stringlist, serie);
                    settingStorage.mapSeries[serie].Name = serie;
                    settingStorage.mapSeries[serie].DName = _LinkedPenName;
                    var ls = AddChartLine(serie, serie, settingStorage.mapSeries[serie].serieTypeLine, settingStorage.mapSeries[serie].thickness, settingStorage.mapSeries[serie].Color, settingStorage.mapSeries[serie].ShowAxis, settingStorage.mapSeries[serie].LogarithmicYScale, settingStorage.mapSeries[serie].LogarithmicBaseYScale, String.Format("{0}_{1}", settingStorage.mapSeries[serie].SGuid, settingStorage.mapSeries[serie].arrayindex), isvisible: settingStorage.mapSeries[serie].IsVisible);
                    if (serie_keynames.Count >= i+1 && !String.IsNullOrEmpty(serie_keynames[i]))
                        lineSeries[serie_keynames[i]] = ls;
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
                if (needToFetchData)
                    SetDataSources(true);
                else
                {
                    UpdateAxisRange();
                    EnableToolbars(IsInStop);
                    if (bIsRestarting)
                        bIsRestarting = false;
                }
                //    if (settingStorage.mapSeries.Count == 0 && AllowRuntimeChanges)
                //        HelpUserChoose();
            }
            finally
            {
                chart.EndInit();
            }
        }

        void InitSettingsForSeriesManagement()
        {
            if (settingStorage == null)
                settingStorage = new SettingsStorage();

            settingStorage.StartTime = settingStorage.DateTimeStart;// DateTime.MinValue;
            settingStorage.EndTime = settingStorage.DateTimeEnd;// DateTime.MaxValue;

            if (listSeries == null)
                listSeries = new List<string>();

            if (settingStorage.mapSeries == null)
                settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

            if (mapHandlers == null)
                mapHandlers = new Dictionary<string, SeriesDataHelpers>();
        }

        string UpdateSettingSorage(SerieData data)
        {
            var name = data.tagName;
            if (!String.IsNullOrEmpty(data.title))
                name = data.title;
            if (String.IsNullOrEmpty(name))
                return null;

            if (string.IsNullOrEmpty(data.guiId))
                data.guiId = Guid.NewGuid().ToString();

            if (!settingStorage.mapSeries.ContainsKey(name))
            {
                settingStorage.mapSeries.Add(name, new SerieSettings()
                {
                    TagName = data.tagName,
                    UseSourceTimeStamp = data.usesourcetimestamp,
                    LocalizeSourceTimeStamp = data.localizeSourceTimeStamp,
                    SourcetimestampColumnName = data.sourcetimestampColumnName,
                    NodeID = data.nodeID,
                    TagreferenceXml = data.tagreferenceXml,
                    ConditionalTagXml = data.conditionalTagXml,
                    SGuid = data.guiId,
                    HistoricalName = data.historicalName,
                    DLRSorce = data.dlrsource,
                    UseTableAggregation = data.useTableAggregation,
                    MinAggregation = data.minAggregation,
                    MaxAggregation = data.maxAggregation,
                    AvgAggregation = data.avgAggregation,
                    ShowAxis = data.showaxis,
                    AuthomaticScale = data.authomaticscale,
                    IsVisible = data.isVisible,
                    IsSet = true,
                    EUnit = data.eUnit,
                    Min = data.min,
                    Max = data.max,
                    LogarithmicYScale = data.logarithmicYScale,
                    LogarithmicBaseYScale = data.logarithmicbaseYScale,
                    IsStatisticEnabled = data.isstatisticenabled,
                    Name = name,
                    Color = data.color,
                    thickness = data.thickness,
                    serieTypeLine = data.serieType.ToString(),
                    AbsoluteMax = data.max,
                    AbsoluteMin = data.min,
                    UseEUMinMax = data.useeuminmax,
                    arrayindex = data.arrayindex,
                    AddVirtualPoints = data.addVirtualPoints,
                    SelectedPenThickness = SelectedPenThickness
                });
            }
            else if (settingStorage.mapSeries.ContainsKey(name))
            {
                settingStorage.mapSeries[name].TagName = data.tagName;
                settingStorage.mapSeries[name].SGuid = data.guiId;
                settingStorage.mapSeries[name].HistoricalName = data.historicalName;
                settingStorage.mapSeries[name].TagreferenceXml = data.tagreferenceXml;
                settingStorage.mapSeries[name].ConditionalTagXml = data.conditionalTagXml; 
                settingStorage.mapSeries[name].NodeID = data.nodeID;
                settingStorage.mapSeries[name].SourcetimestampColumnName = data.sourcetimestampColumnName;
                settingStorage.mapSeries[name].UseSourceTimeStamp = data.usesourcetimestamp;
                settingStorage.mapSeries[name].LocalizeSourceTimeStamp = data.localizeSourceTimeStamp;
                settingStorage.mapSeries[name].EUnit = data.eUnit;
                settingStorage.mapSeries[name].Min = data.min;
                settingStorage.mapSeries[name].Max = data.max;
                settingStorage.mapSeries[name].AbsoluteMax = data.max;
                settingStorage.mapSeries[name].AbsoluteMin = data.min;
            }
            return name;
        }
        bool isComparing;
        private void SetSerieDataSources(string serie, List<Exception> exceptions, string aggTablePostFiss, bool bFromSettings = false)
        {
            if (AliasHelper.ContainsAlias(settingStorage.mapSeries[serie].TagName))
                return;
            if (pendingTask.ContainsKey(serie) && pendingTask[serie].Count > 0)
                return;
            List<Task> taskList = new List<Task>();
            pendingTask[serie] = taskList;
            if (string.IsNullOrEmpty(ConnectionString))
                return;
            if (settingStorage.mapSeries == null)
                return;
            bool aggregate = UseAggregation;
            int maxaggregationfactor = MaxAggregationFactor;
            int maxRecords = MaxRecords;
            int maxDeadLockRetry = MaxDeadLockRetry;
            var connStr = ConnectionString;

            if (!settingStorage.mapSeries.ContainsKey(serie))
                settingStorage.mapSeries.Add(serie, new SerieSettings() { SelectedPenThickness = SelectedPenThickness });

            var bUseAggregatedTables = NeedToAggregate(settingStorage.mapSeries[serie]);
            AggregatedValues result = null;
            var minAgg = settingStorage.mapSeries[serie].MinAggregation;
            var maxAgg = settingStorage.mapSeries[serie].MaxAggregation;
            var avgAgg = settingStorage.mapSeries[serie].AvgAggregation;
            var bAggregationChecked = UseAggregation;
            var viewTimeFrame = ViewTimeFrame;
            var addVirtualPoints = settingStorage.mapSeries[serie].AddVirtualPoints;
            var loadMaxRecordFromDB = LoadMaxRecordFromDB;
            runningOnServer = RunningOnServer;
            clientTimezoneOffset = ClientTimezoneOffset;
            if (bUseAggregatedTables && !minAgg && !maxAgg && !avgAgg)
                return;

            isComparing = settingStorage.Compare;

            if (settingStorage.DateTimeStart == settingStorage.DateTimeEnd)
            {
                settingStorage.DateTimeStart = minDateTimeValue;
                settingStorage.DateTimeEnd = maxDateTimeValue;
            }

            if (settingStorage.DateTimeStartCompare == settingStorage.DateTimeEndCompare)
            {
                settingStorage.DateTimeStartCompare = minDateTimeValue;
                settingStorage.DateTimeEndCompare = maxDateTimeValue;
            }

            var startTime = settingStorage.DateTimeStart;
            var endTime = settingStorage.DateTimeEnd;
            var startTimeCompare = settingStorage.DateTimeStartCompare;
            var endTimeCompare = settingStorage.DateTimeEndCompare;

            var task1 = Task.Factory.StartNew((commandTimeout) =>
            {
                try
                {
                    if (!ct.IsCancellationRequested)
                    {
                        GetEuInformation(settingStorage.mapSeries[serie]);

                        var ret = CreateDataSource(ct, loadMaxRecordFromDB, connStr, maxRecords, serie, false, aggregate, maxaggregationfactor, bUseAggregatedTables, aggTablePostFiss, (int)commandTimeout, maxDeadLockRetry, viewTimeFrame, startTime, endTime, bAggregationChecked, addVirtualPoints);
                        result = ret;

                        // statistics : http://www.codeproject.com/Articles/42492/Using-LINQ-to-Calculate-Basic-Statistics
                        settingStorage.mapSeries[serie].maxValue = Double.NaN;
                        settingStorage.mapSeries[serie].minValue = Double.NaN;
                        settingStorage.mapSeries[serie].averageValue = Double.NaN;
                        settingStorage.mapSeries[serie].medianValue = Double.NaN;
                        settingStorage.mapSeries[serie].varianceValue = Double.NaN;
                        settingStorage.mapSeries[serie].standardDeviationValue = Double.NaN;
                        settingStorage.mapSeries[serie].populationVarianceValue = Double.NaN;
                        settingStorage.mapSeries[serie].populationStandardDeviationValue = Double.NaN;
                        settingStorage.mapSeries[serie].rangeValue = Double.NaN;

                        if (!bUseAggregatedTables && result != null && result.Values != null && result.Values.Count > 0)
                        {
                            var statDouble = (from c in ret.Values
                                              where c.dValue != null
                                              select (double)c.dValue);
                            if (statDouble.Count() > 0)
                            {
                                settingStorage.mapSeries[serie].maxValue = statDouble.Max();
                                settingStorage.mapSeries[serie].minValue = statDouble.Min();
                                settingStorage.mapSeries[serie].averageValue = statDouble.Average();
                                settingStorage.mapSeries[serie].medianValue = statDouble.Median();
                                settingStorage.mapSeries[serie].varianceValue = statDouble.Variance();
                                settingStorage.mapSeries[serie].standardDeviationValue = statDouble.StandardDeviation();
                                settingStorage.mapSeries[serie].populationVarianceValue = statDouble.VarianceP();
                                settingStorage.mapSeries[serie].populationStandardDeviationValue = statDouble.StandardDeviationP();
                                settingStorage.mapSeries[serie].rangeValue = statDouble.Range();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("I have observed a {0}",
                        ex.GetType().Name);
                    exceptions.Add(ex);
                }

                return result;
            }, CommandTimeout, tokenSource.Token);

            taskList.Add(task1);

            if (!bUseAggregatedTables && settingStorage.Compare && DateTime.Compare(settingStorage.DateTimeStartCompare, settingStorage.DateTimeEndCompare) != 0)
            {
                AggregatedValues cresult = null;
                var task3 = task1.ContinueWith((ret, commandTimeout) =>
                {
                    try
                    {
                        if (!ct.IsCancellationRequested)
                            cresult = CreateDataSource(ct, loadMaxRecordFromDB, connStr, maxRecords, serie, true, aggregate, maxaggregationfactor, bUseAggregatedTables, aggTablePostFiss, (int)commandTimeout, maxDeadLockRetry, viewTimeFrame, startTimeCompare, endTimeCompare, false, addVirtualPoints);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("I have observed a {0}",
                            ex.GetType().Name);
                        exceptions.Add(ex);
                    }

                    return cresult;
                }, CommandTimeout, tokenSource.Token);

                taskList.Add(task3);

                var task4 = task3.ContinueWith(ret =>
                {
                    if (pendingTask.ContainsKey(serie))
                    {
                        if (pendingTask[serie].Contains(task3))
                            pendingTask[serie].Remove(task3);
                        if (pendingTask[serie].Count == 0)
                            pendingTask.Remove(serie);
                    }
                    if (ct.IsCancellationRequested)
                    {
                        if (pendingTask.Count == 0)
                            SetBusy(false);
                        return;
                    }
                    else if (cresult != null)
                    {
                        SetChartLineDataSource(serie, cresult.Values, true);
                        TerminateSetDataSource(exceptions, bFromSettings);
                    }
                }, sc);
            }

            var task2 = task1.ContinueWith(ret =>
            {
                if (pendingTask.ContainsKey(serie))
                {
                    if (pendingTask[serie].Contains(task1))
                        pendingTask[serie].Remove(task1);
                    if (pendingTask[serie].Count == 0)
                        pendingTask.Remove(serie);
                }

                if (ct.IsCancellationRequested)
                {
                    if (pendingTask.Count == 0)
                        SetBusy(false);
                    return;
                }
                else if (result == null)
                {
                    TerminateSetDataSource(exceptions, bFromSettings);
                    return;
                }

                dlException = null;

                if (bUseAggregatedTables)
                {
                    if (minAgg)
                        SetChartLineDataSource(serie, result.MinValues, false, true, TableAggregation.Min);
                    if (maxAgg)
                        SetChartLineDataSource(serie, result.MaxValues, false, true, TableAggregation.Max);
                    if (avgAgg)
                        SetChartLineDataSource(serie, result.AvgValues, false, true, TableAggregation.Avg);
                }
                else
                {
                    settingStorage.mapSeries[serie].listValues = result.Values;
                    if (string.IsNullOrEmpty(settingStorage.mapSeries[serie].Name))
                        settingStorage.mapSeries[serie].Name = serie;

                    string _LinkedPenName = settingStorage.mapSeries[serie].Name;
                    _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(_LinkedPenName, stringlist, _LinkedPenName);

                    settingStorage.mapSeries[serie].DName = _LinkedPenName;
                    BuildLineFromValues(serie, result.Values, false, TableAggregation.Min);
                }

                string guid = settingStorage.mapSeries[serie].SGuid;
                if (matchChangedMap.Contains(guid) && mapHandlers.ContainsKey(guid))
                {
                    mapHandlers[guid].RefreshValue();
                    string key = String.Format("{0}_{1}", settingStorage.mapSeries[serie].SGuid, settingStorage.mapSeries[serie].arrayindex);
                    if (OperatingMode != OperatingMode.OnlyStop && mapKeySeries.ContainsKey(key) && viewList.ContainsKey(key))
                    {
                        viewList[key].collectionValues.PrependList(mapKeySeries[key].listValues, true);
                        mapKeySeries[key].lineSerie.BeginInit();
                        seriesHistoryDataCount[mapKeySeries[key].lineSerie] = viewList[key].HistoryData.Count;
                        if (IsInStop)
                            mapKeySeries[key].lineSerie.DataSource = viewList[key].HistoryData;
                        else
                            mapKeySeries[key].lineSerie.DataSource = viewList[key].collectionValues;
                        mapKeySeries[key].lineSerie.EndInit();
                    }
                }

                TerminateSetDataSource(exceptions, bFromSettings);
            }, sc);
        }
        private void GetEuInformation(SerieSettings data)
        {
            string eunit = string.Empty;
            double min = data.Min;
            double max = data.Max;
            if (!data.DLRSorce)
            {
                if (!string.IsNullOrEmpty(data.TagreferenceXml))
                {
                    OPCUAEntityReference item = data.TagreferenceXml.FromXml<OPCUAEntityReference>();
                    if (item != null)
                    {
                        if (item.NodeIdViewModel == null && item.IsValid)
                            item.CreateNodeIdViewModel((Document as ScreenDocument).SessionString);
                        eunit = item.NodeIdViewModel?.EUInformation?.DisplayName?.ToString();
                        try
                        {
                            if (data.UseEUMinMax)
                            {
                                min = (double)item.NodeIdViewModel?.Range?.Low;
                                max = (double)item.NodeIdViewModel?.Range?.High;
                                data.Min = min;
                                data.Max = max;
                            }
                            data.EUnit = eunit;
                        }
                        catch
                        {
                        }

                        if (string.IsNullOrEmpty(data.HistoricalName) && UFUAEditor != null)
                            data.HistoricalName = UFUAEditor.GetHistorianName(Document, item.ResolvedNodeId);
                    }
                }
                else
                    return;
            }
            else
            {
                bool bExecute = false;
                string guid = string.Empty;
                lock (dlrSettings)
                {
                    if (dlrSettings != null && !String.IsNullOrEmpty(data.HistoricalName) && dlrSettings.ContainsKey(data.HistoricalName) && !string.IsNullOrEmpty(data.TagName))
                    {
                        string realTagname = data.TagName.Replace("/", ".");
                        var column = (from c in dlrSettings[data.HistoricalName].Columns where c.Name == realTagname select c).FirstOrDefault();
                        guid = column?.ColumnTagGuid;
                        if (!string.IsNullOrEmpty(guid))
                            bExecute = true;
                    }
                }
                if (bExecute)
                {
                    var eu = UFUAEditor.GetTagEngineeringUnit(Document, guid).Split(';');
                    if (eu.Count() >= 3)
                    {
                        eunit = eu[0];
                        try
                        {
                            if (data.UseEUMinMax)
                            {
                                min = double.Parse(eu[1]);
                                max = double.Parse(eu[2]);
                                data.Min = min;
                                data.Max = max;
                            }
                            data.EUnit = eunit;
                        }
                        catch
                        {
                        }
                    }
                }
                else
                    return;
            }
        }

        Series AddChartLine(String Original, String Name, String type, int thickness, Color color, bool showaxis, bool logarithmicYScale, double logarithmicbaseYScale, string key, object dataSource = null, bool bCompare = false, bool bAggregated = false, TableAggregation aggregation = TableAggregation.Min, bool isvisible = true)
        {

            var lsFound = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).FirstOrDefault();
            if (lsFound != null)
            {
                AddSerieYScale(Original, color, logarithmicYScale, logarithmicbaseYScale, key, lsFound, showaxis, isvisible);
                return lsFound;
            }

            if (bCompare || bAggregated)
                type = "lineSeries";
            var serie = TryFindResource(type) as Series;
            if (serie == null)
                return null;
            serie.DisplayName = Name;

            if (bCompare)
                serie.Tag = compareTag;
            else if (bAggregated)
            {
                switch (aggregation)
                {
                    case TableAggregation.Min:
                        serie.Tag = minTag;
                        break;
                    case TableAggregation.Max:
                        serie.Tag = maxTag;
                        break;
                    case TableAggregation.Avg:
                        serie.Tag = avgTag;
                        break;
                }
            }

            serie.BeginInit();
            serie.ArgumentScaleType = ScaleType.DateTime;
            serie.ArgumentDataMember = "SourceTimeStamp";
            serie.ValueDataMember = "dValue";
            serie.ValueScaleType = ScaleType.Numerical;
            serie.Label.TextPattern = "{V:F3}";
            if (serie is XYSeries2D)
            {
                var pattern = String.Format("{{S}}\n{{V:F{0}}}\n{{A:{1}}}", PointPrecision, GetDateTimeFormat()); 
                (serie as XYSeries2D).CrosshairLabelPattern = pattern;
            }
            /*
            serie.PointOptions = new PointOptions()
            {
                ValueNumericOptions = new NumericOptions()
                {
                    Format = NumericFormat.FixedPoint,
                    Precision = 3
                }
            };
            */

            serie.DataSource = null;
            serie.DataSource = dataSource;

            //if (!AutomaticGeneralScale)
            AddSerieYScale(Original, color, logarithmicYScale, logarithmicbaseYScale, key, serie, showaxis, isvisible);
            if(bCompare)
            {
                if (secondaryAxisX2D != null)
                {
                    if (!((XYDiagram2D)chart.Diagram).SecondaryAxesX.OfType<SecondaryAxisX2D>().Contains(secondaryAxisX2D))
                        ((XYDiagram2D)chart.Diagram).SecondaryAxesX.Add(secondaryAxisX2D);

                    XYDiagram2D.SetSeriesAxisX((XYSeries)serie, secondaryAxisX2D);
                    secondaryAxisX2D.Visible = true;
                    SetAxisFontSettings(XAxsisFontSettings, secondaryAxisX2D);
                }
            }

            diagram.Series.Add(serie);
            CheckAndAdaptAxisRange();

            var xySerie = serie as XYSeries;
            if (xySerie != null)
                xySerie.Brush = isvisible ? new SolidColorBrush(color) : Brushes.Transparent;

            if (thickness > 0)
            {
                if (serie is LineSeries2D)
                {
                    (serie as LineSeries2D).LineStyle = new LineStyle(thickness);
                    (serie as LineSeries2D).LineStyle.LineJoin = PenLineJoin.Bevel;
                }
                else if (serie is AreaSeries2D)
                {
                    (serie as AreaSeries2D).Border = new SeriesBorder();
                    (serie as AreaSeries2D).Border.Brush = new SolidColorBrush(color);
                    (serie as AreaSeries2D).Border.LineStyle = new LineStyle(thickness);
                    (serie as AreaSeries2D).Border.LineStyle.LineJoin = PenLineJoin.Bevel;
                }
            }

            if (serie is LineSeries2D)
            {
                var compareStyle = (serie as LineSeries2D).LineStyle;
                if (bCompare)
                {
                    compareStyle.DashStyle = DashStyles.Dot;
                    (serie as LineSeries2D).LineStyle = compareStyle;
                }
                else if (bAggregated)
                {
                    switch (aggregation)
                    {
                        case TableAggregation.Min:
                            compareStyle.DashStyle = DashStyles.DashDot;
                            (serie as LineSeries2D).LineStyle = compareStyle;
                            break;
                        case TableAggregation.Max:
                            compareStyle.DashStyle = DashStyles.Dot;
                            (serie as LineSeries2D).LineStyle = compareStyle;
                            break;
                    }
                }
            }

            serie.EndInit();
            return serie;
        }

        void AddSerieYScale(String Original, Color color, bool logarithmicYScale, double logarithmicbaseYScale, string key, Series serie, bool showaxis, bool isvisible)
        {
            try
            {
                string sanitized = GetSanitizedName(Original);
               
                if (MapSerieAxis.ContainsKey(sanitized)) return;

                var saFound = TryFindResource("secondaryaxisY") as SecondaryAxisY2D;
                saFound.Logarithmic = logarithmicYScale;
                saFound.LogarithmicBase = logarithmicbaseYScale;
                saFound.Brush = new SolidColorBrush(color);
                saFound.TickmarksCrossAxis = true;

                SetAxisFontSettings(YAxsisFontSettings, saFound);

                if (key != null && OperatingMode != OperatingMode.OnlyStop)
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

                ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Add(saFound);
                MapSerieAxis.Add(sanitized, saFound);
                

                if (saFound != null)
                {
                    saFound.Brush = new SolidColorBrush(color);
                    if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(Original))
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
            var title = new AxisTitle() { Alignment = TitleAlignment.Near, ContentTemplate = (DataTemplate)Resources["AxisTitleTemplate"] };
            title.SetBinding(AxisTitle.ContentProperty, bnd);
            saFound.Title = title;
        }

        readonly String compareTag = Properties.Settings.Default.CompareID;
        readonly String minTag = Properties.Settings.Default.MinID;
        readonly String maxTag = Properties.Settings.Default.MaxID;
        readonly String avgTag = Properties.Settings.Default.AvgID;

        void ClearAllCompareSeries()
        {
            isComparing = false;
            var ls = (from c in diagram.Series.OfType<Series>() where c.Tag as String == compareTag select c).ToList();
            chart.BeginInit();
            ls.ForEach(serie => diagram.Series.Remove(serie));
            if (((XYDiagram2D)chart.Diagram).SecondaryAxesX.OfType<SecondaryAxisX2D>().Contains(secondaryAxisX2D))
                ((XYDiagram2D)chart.Diagram).SecondaryAxesX.Remove(secondaryAxisX2D);
            chart.EndInit();
        }

        void SetChartLineDataSource(String Name, object dataSource, bool bCompare = false, bool bAggregated = false, TableAggregation aggregation = TableAggregation.Min)
        {
            var original = Name;
            if (bCompare)
                Name = String.Format("{0} - {1}", Name, compareTag);
            else if (bAggregated)
            {
                var loriginal = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).ToList();
                if (loriginal.Count > 0)
                    loriginal.ForEach(s => diagram.Series.Remove(s));
                Name = String.Format("{0} - {1}", Name, aggregation);
            }
            else
            {
                var laggregated = (from c in diagram.Series.OfType<Series>()
                                   where
c.DisplayName == String.Format("{0} - {1}", Name, TableAggregation.Min) ||
c.DisplayName == String.Format("{0} - {1}", Name, TableAggregation.Max) ||
c.DisplayName == String.Format("{0} - {1}", Name, TableAggregation.Avg)
                                   select c).ToList();
                if (laggregated.Count > 0)
                    laggregated.ForEach(s => diagram.Series.Remove(s));
            }

            var ls = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).ToList();
            if (ls.Count == 0)
            {
                var serie = AddChartLine(original, Name, settingStorage.mapSeries[original].serieTypeLine, settingStorage.mapSeries[original].thickness, settingStorage.mapSeries[original].Color, settingStorage.mapSeries[original].ShowAxis, settingStorage.mapSeries[original].LogarithmicYScale, settingStorage.mapSeries[original].LogarithmicBaseYScale, String.Format("{0}_{1}", settingStorage.mapSeries[original].SGuid, settingStorage.mapSeries[original].arrayindex), dataSource, bCompare, bAggregated, aggregation, isvisible: settingStorage.mapSeries[original].IsVisible);
                if (bCompare)
                {
                    settingStorage.mapSeries[original].lineSerieCompare = serie;
                    settingStorage.mapSeries[original].lineSerieMin = null;
                    settingStorage.mapSeries[original].lineSerieMax = null;
                    settingStorage.mapSeries[original].lineSerieAvg = null;
                }
                else if (bAggregated)
                {
                    switch (aggregation)
                    {
                        case TableAggregation.Min:
                            settingStorage.mapSeries[original].lineSerieMin = serie;
                            break;
                        case TableAggregation.Max:
                            settingStorage.mapSeries[original].lineSerieMax = serie;
                            break;
                        case TableAggregation.Avg:
                            settingStorage.mapSeries[original].lineSerieAvg = serie;
                            break;
                    }
                    settingStorage.mapSeries[original].lineSerie = null;
                }
                else
                {
                    settingStorage.mapSeries[original].lineSerie = serie;
                    settingStorage.mapSeries[original].lineSerieMin = null;
                    settingStorage.mapSeries[original].lineSerieMax = null;
                    settingStorage.mapSeries[original].lineSerieAvg = null;
                }

                serie.Visible = settingStorage.mapSeries[original].IsVisible;
                if (serie.Points != null && serie.Points.Count > MaxResolveOverlappingPoints)
                    (serie as Series).Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
            }
            else
            {
                ls[0].BeginInit();
                if (OperatingMode == OperatingMode.OnlyStop || IsInStop)
                {
                    ls[0].DataSource = null;
                    ls[0].DataSource = dataSource;
                }
                ls[0].Visible = settingStorage.mapSeries[original].IsVisible;
                if (ls[0].Points != null)
                {
                    if (ls[0].Points.Count > MaxResolveOverlappingPoints)
                        (ls[0] as Series).Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
                    (ls[0] as Series).LabelsVisibility = !IsInStop ? false : btnShowLabels.IsChecked == true && ls[0].Points.Count <= MaxLabelPoints;
                }
                ls[0].EndInit();

            }

            CheckAndAdaptAxisRange();

        }

        /*
        List<UFUAAuditDataItem> CreateGridDataSource(List<String> listSelected, String connStr, int maxRecord)
        {
            if (String.IsNullOrEmpty(connStr))
                return null;
            CreateConnectionStringDataLayer(connStr);

            var ret = (from entry in new XPQuery<UFUAAuditDataItem>(ufw)// .AsParallel()
                       where (listSelected.Contains(entry.Name) || listSelected.Contains(entry.NodeId)) &&
                             (settingStorage.DateTimeStart == DateTime.MinValue || entry.RecordDateTime.ToLocalTime() >= settingStorage.DateTimeStart) &&
                             (settingStorage.DateTimeEnd == DateTime.MinValue || entry.RecordDateTime.ToLocalTime() <= settingStorage.DateTimeEnd)
                       orderby entry.RecordDateTime ascending
                       select entry).Take(maxRecord).ToList();
            return ret;
        }
        */

        AggregatedValues CreateDataSource(CancellationToken ct, bool loadMaxRecordFromDB, String connStr, int maxRecord, String serie, bool compare, bool aggregate, int maxaggregationfactor, bool useAggregatedTables, string aggTablePostFiss, int commandTimeout, int maxDeadLockRetry, TimeSpan viewTimeFrame, DateTime startTime, DateTime endTime, bool bAggregationChecked = false, bool bAddVirtualPoints = false)
        {
            if (ct.IsCancellationRequested)
                return null;

            AggregatedValues ret = new AggregatedValues();
            if (!settingStorage.mapSeries.ContainsKey(serie))
                return ret;
            var seriesettings = settingStorage.mapSeries[serie];
            bool dlrsource = settingStorage.mapSeries[serie].DLRSorce;
            if (seriesettings == null || string.IsNullOrEmpty(seriesettings.HistoricalName))
                return ret;

            String name = serie;
            if (!String.IsNullOrEmpty(seriesettings.TagName))
                name = seriesettings.TagName;

            bool usesourcetimestamp = seriesettings.UseSourceTimeStamp;
            bool localize = seriesettings.LocalizeSourceTimeStamp;
            string historicalname = seriesettings.HistoricalName;

            if (String.IsNullOrEmpty(connStr))
                return null;
            CreateConnectionStringDataLayer(connStr, commandTimeout);

            if (dl == null)
                return new AggregatedValues();

            IDataLayer _dl = dl;

            int maxrecord = aggregate ? maxRecord * maxaggregationfactor : maxRecord;

            if (!bAggregationChecked && visualRangeBeforeAggregationToggle != null && !compare)
            {
                startTime = visualRangeBeforeAggregationToggle.Item1;
                endTime = visualRangeBeforeAggregationToggle.Item2;
            }

            if (!dlrsource)
            {
                #region Historicals
                string hstname = historicalname;
                string realTagname = name.Replace("//", "/");
                realTagname = realTagname.Replace("/", ".");
                if (realTagname.StartsWith("."))
                    realTagname = realTagname.Substring(1);
                if (!realTagname.StartsWith("Tags."))
                    realTagname = $"Tags.{realTagname}";

                if (MapToHistoricalConnectsions != null && MapToHistoricalConnectsions.ContainsKey(hstname) && !string.IsNullOrEmpty(MapToHistoricalConnectsions[hstname]))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(hstname))
                        {
                            CreateCustomConnectionStringDataLayer(hstname, commandTimeout);
                            if (MapToHistoricalDataLayer != null)
                            {
                                if (MapToHistoricalDataLayer.ContainsKey(hstname) && MapToHistoricalDataLayer[hstname] != null)
                                    _dl = MapToHistoricalDataLayer[hstname];
                            }
                        }
                    }
                    catch (Exception)
                    {
                        _dl = dl;
                    }
                }

                bool bUseOid = false;
                
                using (UnitOfWork _ufw = new UnitOfWork(_dl))
                {
                    try
                    {
                        UFUAAuditDataLog _uFUAAuditDataLog = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)// .AsParallel()
                                                              where entry.NodeId == seriesettings.NodeID
                                                              select entry).FirstOrDefault();

                        if (_uFUAAuditDataLog == null)
                            return ret;
                        
                        realTagname = _uFUAAuditDataLog.Name;

                        if (!string.IsNullOrEmpty(seriesettings.NodeID) && _uFUAAuditDataLog != null)
                            bUseOid = true;
                        bool useDateTime = !(startTime == minDateTimeValue && endTime == maxDateTimeValue);
                        var utcStart = startTime;
                        var utcEnd = endTime;

                        if(useDateTime)
                        {
                            utcStart = utcStart.ToUniversalTime();
                            utcEnd = utcEnd.ToUniversalTime();
                        }

                        if (utcStart < minDateTimeValue)
                            utcStart = minDateTimeValue;
                        if (utcEnd > maxDateTimeValue)
                            utcEnd = maxDateTimeValue;

                        if (!usesourcetimestamp)
                        {
                            int retry = 0;
                            while (true)
                            {
                                try
                                {
                                    var res = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                  where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                                  (!useDateTime || (useDateTime && (entry.RecordDateTimeUtc >= utcStart) && (entry.RecordDateTimeUtc <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                  orderby entry.RecordDateTimeUtc descending
		                                          select new MyDataValue(
		                                              runningOnServer ? entry.RecordDateTimeUtc.Add(clientTimezoneOffset) : entry.RecordDateTimeUtc.ToLocalTime(),
		                                              entry.dValue
		                                            )
		                                          ).ToList();
                                    ret.Values = loadMaxRecordFromDB ? res.Take(maxrecord).ToList() : res;
                                    if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null)) /*&& (useDateTime || ret.Values.Count == 1)*/
                                    {
                                        var pointBeforeVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                                 where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                                                 entry.RecordDateTimeUtc < utcStart
                                                                 orderby entry.RecordDateTimeUtc descending
                                                                 select new MyDataValue(
                                                                     runningOnServer ? entry.RecordDateTimeUtc.Add(clientTimezoneOffset) : entry.RecordDateTimeUtc.ToLocalTime(),
                                                                     entry.dValue
                                                                   )
                                                      ).Take(1).ToList();
                                        var pointAfterVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                                 where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                                                 entry.RecordDateTimeUtc > utcEnd
                                                                 orderby entry.RecordDateTimeUtc descending
                                                                     select new MyDataValue(
                                                                     runningOnServer ? entry.RecordDateTimeUtc.Add(clientTimezoneOffset) : entry.RecordDateTimeUtc.ToLocalTime(),
                                                                     entry.dValue
                                                                   )
                                                      ).Take(1).ToList();
                                        InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame, startTime, endTime);
                                    }
                                    break;
                                }
                                catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                {
                                    if (++retry >= maxDeadLockRetry)
                                        throw ex;
                                }
                            }
                        }
                        else
                        {
                            int retry = 0;
                            while (true)
                            {
                                try
                                {
                                    var res = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                  where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                  (!useDateTime || (useDateTime && (entry.SourceTimeStamp >= utcStart) && (entry.SourceTimeStamp <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                  orderby entry.SourceTimeStamp descending
		                                          select new MyDataValue(
		                                              localize ? runningOnServer ? entry.SourceTimeStamp.Add(clientTimezoneOffset) : entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
		                                              entry.dValue
		                                            )
		                                          ).ToList();
                                    ret.Values = loadMaxRecordFromDB ? res.Take(maxrecord).ToList() : res;
                                    if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null)) /*&& useDateTime*/
                                    {
                                        var pointBeforeVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                                      where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                                      entry.SourceTimeStamp < utcStart
                                                                      orderby entry.SourceTimeStamp descending
                                                                      select new MyDataValue(
                                                                          localize ? runningOnServer ? entry.SourceTimeStamp.Add(clientTimezoneOffset) : entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                                          entry.dValue
                                                                        )
                                                      ).Take(1).ToList();
                                        var pointAfterVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                                     where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                                     entry.SourceTimeStamp > utcEnd
                                                                     orderby entry.SourceTimeStamp descending
                                                                     select new MyDataValue(
                                                                         localize ? runningOnServer ? entry.SourceTimeStamp.Add(clientTimezoneOffset) : entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                                         entry.dValue
                                                                       )
                                                      ).Take(1).ToList();
                                        InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame, startTime, endTime);
                                    }
                                    break;
                                }
                                catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                {
                                    if (++retry >= maxDeadLockRetry)
                                        throw ex;
                                }
                            }
                        }


                        if (ct.IsCancellationRequested)
                            return null;

                        settingStorage.mapSeries[serie].numPoints = ret.Values.Count;
                        settingStorage.mapSeries[serie].numCompressRation = 1;
                        if (loadMaxRecordFromDB && ret.Values.Count > maxRecord)
                        {
                            int div = ret.Values.Count / maxRecord;

                            if (useDateTime && !bUseOid)
                            {
                                List<MyDataValue> missingBefore;
                                List<MyDataValue> missingAfter;
                                int retry = 0;
                                while (true)
                                {
                                    try
                                    {
                                        missingBefore = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                   where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                   entry.SourceTimeStamp < utcStart
                                                   orderby entry.SourceTimeStamp descending
                                                   select new MyDataValue(
                                                       localize ? runningOnServer ? entry.SourceTimeStamp.Add(clientTimezoneOffset) : entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                       entry.dValue
                                                     )
                                                   ).Take(div).ToList();
                                        missingAfter = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                         where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                         entry.SourceTimeStamp > utcEnd
                                                         orderby entry.SourceTimeStamp ascending
                                                         select new MyDataValue(
                                                             localize ? runningOnServer ? entry.SourceTimeStamp.Add(clientTimezoneOffset) : entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                             entry.dValue
                                                           )
                                                   ).Take(div).ToList();
                                        break;
                                    }
                                    catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                    {
                                        if (++retry >= maxDeadLockRetry)
                                            throw ex;
                                    }
                                }
                                if (missingBefore.Count > 0)
                                    ret.Values.InsertRange(0, missingBefore);
                                if (missingAfter.Count > 0)
                                    ret.Values.AddRange(missingAfter);
                            }

                            AggregatedValues retaggregated = new AggregatedValues();
                            retaggregated.Values = Aggregate(ret.Values, div, ct);
                            settingStorage.mapSeries[serie].numCompressRation = div;
                            settingStorage.mapSeries[serie].numCompressPoint = retaggregated.Values.Count;

                            return retaggregated;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
                #endregion
            }
            else
            {
                #region Datalogger
                var minAgg = settingStorage.mapSeries[serie].MinAggregation;
                var maxAgg = settingStorage.mapSeries[serie].MaxAggregation;
                var avgAgg = settingStorage.mapSeries[serie].AvgAggregation;

                if (useAggregatedTables && !minAgg && !maxAgg && !avgAgg)
                    return ret;
                string postfix = aggTablePostFiss;
                string dlrname = historicalname;
                string realTagname = name.Replace("/", ".");
                if (realTagname.StartsWith("Tags."))
                    realTagname = realTagname.Remove(0, "Tags.".Length);

                String _defaultDataProvider = defaultDataProvider;
                String _defaultConnectionString = defaultConnectionString;

                if (MapToDatalogerConnectsions != null && MapToDatalogerConnectsions.ContainsKey(dlrname) && !string.IsNullOrEmpty(MapToDatalogerConnectsions[dlrname]))
                {
                    try
                    {
                        var helper = new ConnectionStringParser(MapToDatalogerConnectsions[dlrname]);
                        string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                        if (!String.IsNullOrEmpty(providerType))
                        {
                            _defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(MapToDatalogerConnectsions[dlrname]);
                            _defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(MapToDatalogerConnectsions[dlrname]);
                        }
                        else
                        {
                            _defaultDataProvider = helper.GetPartByName("DataProvider");
                            helper.RemovePartByName("DataProvider");
                            _defaultConnectionString = helper.GetConnectionString();
                        }
                    }
                    catch (Exception)
                    {
                        _defaultDataProvider = defaultDataProvider;
                        _defaultConnectionString = defaultConnectionString;
                    }
                }

                if (string.IsNullOrEmpty(dlrname) || string.IsNullOrEmpty(realTagname) || string.IsNullOrEmpty(_defaultConnectionString) || string.IsNullOrEmpty(_defaultDataProvider))
                    return ret;
                DataLoggerColumns columns;
                string _utccolumnname;
                string _tablename;
                lock (dlrSettings)
                {
                    if (dlrSettings == null || !dlrSettings.ContainsKey(dlrname))
                        return ret;

                    _tablename = string.IsNullOrEmpty(dlrSettings[dlrname].TableName) ? dlrname : dlrSettings[dlrname].TableName;
                    if (useAggregatedTables)
                        _tablename = $"{ _tablename}{postfix}";
                    _utccolumnname = seriesettings.UseTableAggregation || string.IsNullOrEmpty(dlrSettings[dlrname].UtcTimeColumnName) ? "UtcTimeCol" : dlrSettings[dlrname].UtcTimeColumnName;
                    columns = dlrSettings[dlrname].Columns;
                }
                var column = (from c in columns where c.Name == realTagname select c).FirstOrDefault();
                string _timeColumnName = !seriesettings.UseTableAggregation && usesourcetimestamp && column != null && column.AddSourceTimeStampColumn ? !string.IsNullOrEmpty(column.SourceTimeStampColumnName) ? $"{realTagname}_{column.SourceTimeStampColumnName}" : $"{realTagname}_SourceTimeStamp" : _utccolumnname;
                var minrealTagname = $"{realTagname}{Properties.Settings.Default.MinPenPostFix}";
                var maxrealTagname = $"{realTagname}{Properties.Settings.Default.MaxPenPostFix}";
                var avgrealTagname = $"{realTagname}{Properties.Settings.Default.AvgPenPostFix}";

                using (var dbconnection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                {
                    try
                    {
                        dbconnection.Open();
                        var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);

                        DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                        dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                        dbdapater.SelectCommand.Connection = dbconnection;
                        dbdapater.SelectCommand.CommandTimeout = commandTimeout;

                        StringBuilder commantText = new StringBuilder("SELECT ");
                        DataSet gridDataSet = new DataSet();

                        if (loadMaxRecordFromDB && dbSchemaInfo.IsSupportedTopKeyword)
                            commantText.AppendFormat("TOP {0} ", maxrecord);
                        commantText.AppendFormat("{0}", dbSchemaInfo.WrapObjectName(_timeColumnName));

                        if (useAggregatedTables)
                        {
                            if (minAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(minrealTagname));
                            if (maxAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(maxrealTagname));
                            if (avgAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(avgrealTagname));
                        }
                        else
                            commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(realTagname));


                        commantText.AppendFormat(" FROM {0} WHERE {1} IS NOT NULL",
                            dbSchemaInfo.WrapObjectName(_tablename),
                            dbSchemaInfo.WrapObjectName(_timeColumnName));

                        string condGuid = $"{condPrefix}{seriesettings.SGuid}";
                        if (condValues.ContainsKey(condGuid))
                        {
                            string conditionalValue = condValues[condGuid].Value?.ToString();
                            if(!string.IsNullOrEmpty(conditionalValue))
                                commantText.AppendFormat(" AND {0}", conditionalValue);
                        }

                        if (useAggregatedTables)
                        {
                            if (minAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(minrealTagname));
                            if (maxAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(maxrealTagname));
                            if (avgAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(avgrealTagname));
                        }
                        //else
                        //    commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(realTagname));


                        if (!(startTime == minDateTimeValue && endTime == maxDateTimeValue))
                        {
                            var utcStart = startTime;
                            var utcEnd = endTime;
                            bool useDateTime = !(startTime == minDateTimeValue && endTime == maxDateTimeValue);
                            if (useDateTime)
                            {
                                utcStart = utcStart.ToUniversalTime();
                                utcEnd = utcEnd.ToUniversalTime();
                            }

                            var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                            datestart.DbType = System.Data.DbType.DateTime;
                            datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                            datestart.Value = utcStart;
                            dbdapater.SelectCommand.Parameters.Add(datestart);

                            var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                            dateend.DbType = System.Data.DbType.DateTime;
                            dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                            dateend.Value = utcEnd;
                            dbdapater.SelectCommand.Parameters.Add(dateend);

                            commantText.AppendFormat(" AND {0} >= {1} AND {0} <= {2} AND {0} IS NOT NULL",
                                dbSchemaInfo.WrapObjectName(_timeColumnName),
                                datestart.ParameterName,
                                dateend.ParameterName);

                        }

                        commantText.AppendFormat(" ORDER BY {0} DESC",
                            dbSchemaInfo.WrapObjectName(_timeColumnName));

                        dbdapater.SelectCommand.CommandText = commantText.ToString();
                        dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                        //dbdapater.Fill(gridDataSet, _tablename);
                        DataTable retTable = null;
                        if (dbSchemaInfo.IsSupportedTopKeyword)
                        {
                            dbdapater.Fill(gridDataSet, _tablename);
                            retTable = gridDataSet.Tables[0];
                        }
                        else
                            retTable = DataReader.DataReader.DataTableFromDataSet(gridDataSet, dbdapater, maxrecord);


                        using (DataView dataView = new DataView(retTable))
                        {
                            dataView.Sort = string.Format("{0} DESC", _timeColumnName);

                            foreach (DataRowView rowView in dataView)
                            {
                                if (ct.IsCancellationRequested)
                                    break;
                                DateTime date = (DateTime)rowView[_timeColumnName];
                                var sourcetimestamp = (usesourcetimestamp && localize) || !usesourcetimestamp ? runningOnServer ? date.Add(clientTimezoneOffset) : date.ToLocalTime() : date;
                                if (useAggregatedTables)
                                {
                                    if (minAgg && retTable.Columns.Contains(minrealTagname))
                                        ret.MinValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[minrealTagname])));
                                    if (maxAgg && retTable.Columns.Contains(maxrealTagname))
                                        ret.MaxValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[maxrealTagname])));
                                    if (avgAgg && retTable.Columns.Contains(avgrealTagname))
                                        ret.AvgValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[avgrealTagname])));
                                }
                                else
                                    ret.Values.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[realTagname])));
                            }

                            if (ct.IsCancellationRequested)
                                return null;

                            settingStorage.mapSeries[serie].numPoints = dataView.Count;
                            settingStorage.mapSeries[serie].numCompressRation = 1;
                            if (loadMaxRecordFromDB && dataView.Count > maxRecord)
                            {
                                int div = dataView.Count / maxRecord;
                                AggregatedValues retaggregated = new AggregatedValues();

                                if (useAggregatedTables)
                                {
                                    if (ret.MinValues.Count > 0)
                                        retaggregated.MinValues = Aggregate(ret.MinValues, div, ct);
                                    if (ret.MaxValues.Count > 0)
                                        retaggregated.MaxValues = Aggregate(ret.MaxValues, div, ct);
                                    if (ret.AvgValues.Count > 0)
                                        retaggregated.AvgValues = Aggregate(ret.AvgValues, div, ct);
                                    settingStorage.mapSeries[serie].numCompressPoint = Math.Max(Math.Max(ret.MaxValues.Count, ret.MinValues.Count), ret.AvgValues.Count);
                                }
                                else
                                {
                                    if (ret.Values.Count > 0)
                                    {
                                        List<MyDataValue> missingBefore;
                                        List<MyDataValue> missingAfter;
                                        missingBefore = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, div, _tablename, _timeColumnName, realTagname, usesourcetimestamp, localize, true, startTime, endTime);
                                        missingAfter = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, div, _tablename, _timeColumnName, realTagname, usesourcetimestamp, localize, false, startTime, endTime);
                                        if (missingBefore.Count > 0)
                                            ret.Values.AddRange(missingBefore);
                                        if (missingAfter.Count > 0)
                                            ret.Values.InsertRange(0, missingAfter);
                                        retaggregated.Values = Aggregate(ret.Values, div, ct);
                                    }
                                    settingStorage.mapSeries[serie].numCompressPoint = ret.Values.Count;
                                }

                                settingStorage.mapSeries[serie].numCompressRation = div;

                                return retaggregated;
                            }
                            else if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null))
                            {
                                var pointBeforeVisualRange = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, 1, _tablename, _timeColumnName, realTagname, usesourcetimestamp, localize, true, startTime, endTime);
                                var pointAfterVisualRange = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, 1, _tablename, _timeColumnName, realTagname, usesourcetimestamp, localize, false, startTime, endTime);
                                InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame, startTime, endTime);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
                #endregion
            }
            return ret;
        }

        void InsertOuterPoints(List<MyDataValue> ret, List<MyDataValue> beforePoints, List<MyDataValue> afterPoints, TimeSpan viewTimeFrame, DateTime startTime, DateTime endTime)
        {
            var bOnlyOnePoint = ret.Count == 1;
            TimeSpan timeSpanDiff = new TimeSpan(viewTimeFrame.Ticks);
            try
            {
                if (startTime != minDateTimeValue)
                    timeSpanDiff = endTime.Subtract(startTime);
            }
            catch { }

            if (beforePoints.Count > 0)
                ret.AddRange(beforePoints);
            else if (ret.Count > 0)
            {
                DateTime firstPointDate = minDateTimeValue;
                try
                {
                    if (bOnlyOnePoint)
                        firstPointDate = ret.Last().SourceTimeStamp - timeSpanDiff;
                }
                catch { }
                ret.Add(new MyDataValue(firstPointDate, ret.Last().dValue) { bIsFakePoint = true });
            }
            if (afterPoints.Count > 0)
                ret.InsertRange(0, afterPoints);
            else if (ret.Count > 0)
            {
                DateTime lastPointDate = DateTime.Now;
                try
                {
                    if (bOnlyOnePoint)
                    {
                        DateTime margin = ret.First().SourceTimeStamp + timeSpanDiff;
                        lastPointDate = margin <= DateTime.Now ? margin : DateTime.Now;
                    }
                }
                catch { }
                ret.Insert(0, new MyDataValue(lastPointDate, ret.First().dValue) { bIsFakePoint = true });
            }
        }

        List<MyDataValue> FillMissingValues(DbDataAdapter dbdapater, DbSchemaInfo dbSchemaInfo, string _defaultDataProvider, int div, string _tablename, string _timeColumnName, string realTagname, bool usesourcetimestamp, bool localize, bool bBefore, DateTime startTime, DateTime endTime)
        {
            if (startTime == minDateTimeValue && endTime == maxDateTimeValue)
                return new List<MyDataValue>();

            List<MyDataValue> ret = new List<MyDataValue>();
            DataTable retTable = null;

            StringBuilder commantText = new StringBuilder("SELECT ");
            DataSet gridDataSet = new DataSet();

            if (dbSchemaInfo.IsSupportedTopKeyword)
                commantText.AppendFormat("TOP {0} ", div);

            commantText.AppendFormat("{0}", dbSchemaInfo.WrapObjectName(_timeColumnName));

            commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(realTagname));

            commantText.AppendFormat(" FROM {0} WHERE {1} IS NOT NULL",
            dbSchemaInfo.WrapObjectName(_tablename),
            dbSchemaInfo.WrapObjectName(_timeColumnName));

            dbdapater.SelectCommand.Parameters.Clear();

            var utcStart = startTime;
            var utcEnd = endTime;
            if (startTime != DateTime.MinValue)
                utcStart = startTime.ToUniversalTime();
            if (endTime != DateTime.MinValue)
                utcEnd = endTime.ToUniversalTime();

            if (bBefore)
            {
                var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                datestart.DbType = System.Data.DbType.DateTime;
                datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                datestart.Value = utcStart;
                dbdapater.SelectCommand.Parameters.Add(datestart);
                commantText.AppendFormat(" AND {0} < {1} AND {0} IS NOT NULL",
                dbSchemaInfo.WrapObjectName(_timeColumnName),
                datestart.ParameterName);
            }
            else
            {
                var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                dateend.DbType = System.Data.DbType.DateTime;
                dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                dateend.Value = utcEnd;
                dbdapater.SelectCommand.Parameters.Add(dateend);

                commantText.AppendFormat(" AND {0} > {1} AND {0} IS NOT NULL",
                dbSchemaInfo.WrapObjectName(_timeColumnName),
                dateend.ParameterName);
            }

            commantText.AppendFormat(" ORDER BY {0} DESC",
                dbSchemaInfo.WrapObjectName(_timeColumnName));

            dbdapater.SelectCommand.CommandText = commantText.ToString();
            dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);

            if (dbSchemaInfo.IsSupportedTopKeyword)
            {
                dbdapater.Fill(gridDataSet, _tablename);
                retTable = gridDataSet.Tables[0];
            }
            else
                retTable = DataReader.DataReader.DataTableFromDataSet(gridDataSet, dbdapater, div);

            if (retTable != null)
            {
                using (DataView dataView = new DataView(retTable))
                {
                    dataView.Sort = string.Format("{0} DESC", _timeColumnName);

                    foreach (DataRowView rowView in dataView)
                    {
                        if (ct.IsCancellationRequested)
                            break;
                        DateTime date = (DateTime)rowView[_timeColumnName];
                        var sourcetimestamp = (usesourcetimestamp && localize) || !usesourcetimestamp ? runningOnServer ? date.Add(clientTimezoneOffset) : date.ToLocalTime() : date;
                        ret.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[realTagname])));
                    }
                }
            }
            return ret;
        }

        double? GetNullOrValidDouble(object item)
        {
            double? val = null;
            if (item is DBNull)
                return val;
            try
            {
                val = System.Convert.ToDouble(item, CultureInfo.InvariantCulture);
            }
            catch { }
            return val;
        }

        List<MyDataValue> Aggregate(List<MyDataValue> values, int div, CancellationToken ct)
        {
            var aggregated = new List<MyDataValue>();
            int i = 0;
            while (i * div < values.Count)
            {
                if (ct.IsCancellationRequested)
                    return null;

                var list = values.Skip(i * div).Take(div).ToList();
                var average = list.Aggregate((acc, cur) => acc + cur) / list.Count;
                var timespan = list[list.Count - 1].SourceTimeStamp - list[0].SourceTimeStamp;
                var timespanaverage = new TimeSpan(0, 0, (int)timespan.TotalSeconds / 2);
                var time = list[0].SourceTimeStamp + timespanaverage;
                aggregated.Add(new MyDataValue() { SourceTimeStamp = time, dValue = average.dValue, bCompressed = true });
                i++;
            }
            return aggregated;

        }

        void InitServerDocument()
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (Document != null && Document.Parent != null)
            {

                if (UFUAEditor == null)
                    UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                if (UFUAEditor == null)
                    return;

                lock (dlrSettings)
                {
                    dlrSettings.Clear();
                    if (StaticSeriesSettings != null)
                    {
                        var dlrlist = (from s in StaticSeriesSettings where s.dlrsource && !string.IsNullOrEmpty(s.historicalName) select s.historicalName).ToList();
                        var _list = UFUAEditor.GetDataLoggerSettings(Document).ToList();
                        if (_list != null)
                        {
                            (from d in _list where dlrlist.Contains(d[0]) select d).ToList()./*AsParallel().ForAll*/ForEach(d =>
                            {
                                if (!dlrSettings.ContainsKey(d[0]))
                                {
                                    DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                                    ds.Columns = new DataLoggerColumns();
                                    var columns = UFUAEditor.GetDataLoggerColumnSettingList(Document, ds.Name) as List<List<String>>;
                                    if (columns != null)
                                    {
                                        columns.ForEach(s =>
                                        {
                                            ds.Columns.Add(new DataLoggerColumn() { Name = s[0], SourceTimeStampColumnName = s[1], AddSourceTimeStampColumn = bool.Parse(s[2]), ColumnTagName = s[3], ColumnTagGuid = s[4] });
                                        });
                                        dlrSettings.Add(ds.Name, ds);
                                    }
                                }
                            });
                        }
                    }
                    if (MapToDatalogerConnectsions == null)
                        MapToDatalogerConnectsions = new Dictionary<String, String>();
                    MapToDatalogerConnectsions.Clear();

                    if (MapToHistoricalConnectsions == null)
                        MapToHistoricalConnectsions = new Dictionary<String, String>();
                    MapToHistoricalConnectsions.Clear();

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

                    List<string> list = new List<string>();
                    var list3 = UFUAEditor.GetHistoricalSettingsNameList(Document, bReloadDocument: false, inExecution: true);
                    if (list3 != null)
                        list.AddRange(list3);
                    list.ForEach(x =>
                    {
                        if (!MapToHistoricalConnectsions.ContainsKey(x))
                        {
                            var conn = UFUAEditor.GetHistorianConnection(Document, x);
                            MapToHistoricalConnectsions.Add(x, string.IsNullOrEmpty(conn) ? string.Empty : RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, sessionString));
                        }
                    });
                }
            }
        }

        void UpdateAxisRange()
        {
            //axisX.ActualWholeRange.SetAuto();
            //axisX.ActualVisualRange.SetAuto();
            if (!isInZoomMode)
            {
                axisY.ActualWholeRange.SetAuto();
                axisY.ActualVisualRange.SetAuto();
                //if (!AutomaticScale || bDesignmode)
                //{
                //    axisY.ActualWholeRange.MinValue = Minimum;
                //    axisY.ActualWholeRange.MaxValue = Maximum;
                //    axisY.ActualVisualRange.MinValue = Minimum;
                //    axisY.ActualVisualRange.MaxValue = Maximum;
                //}

                (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select c).ToList().ForEach(x =>
                {
                    x.ActualWholeRange.SetAuto();
                    x.ActualVisualRange.SetAuto();
                    if (!bDesignmode)
                    {
                        var serie = (from s in settingStorage.mapSeries.Values where !s.AuthomaticScale && x.Name == GetSanitizedName(s.Name) select s).FirstOrDefault();
                        if (serie != null)
                        {
                            //if (settingStorage.mapSeries.ContainsKey(serie.Name))
                            //{
                            //    x.ActualWholeRange.MinValue = settingStorage.mapSeries[serie.Name].AbsoluteMin;
                            //    x.ActualWholeRange.MaxValue = settingStorage.mapSeries[serie.Name].AbsoluteMax;
                            //}
                            //else
                            //{
                            Dispatcher.BeginInvokeAsynchronously(() =>
                            {
                                if (bDispose)
                                    return;

                                x.ActualWholeRange.MinValue = serie.Min;
                                x.ActualWholeRange.MaxValue = serie.Max;
                            });
                            //}
                        }
                    }
                });
            }
            isInZoomMode = false;

            if (settingStorage == null)
                return;

            DateTime dateTimeStart = settingStorage.DateTimeStart;
            DateTime dateTimeEnd = settingStorage.DateTimeEnd;
            DateTime dateTimeStartCompare = settingStorage.DateTimeStartCompare;
            DateTime dateTimeEndCompare = settingStorage.DateTimeEndCompare;

            if (settingStorage.Compare && settingStorage.ComparingAlignment != HorizontalComparisonAlignment.Stretch)
            {
                TimeSpan deltaTime = dateTimeEnd - dateTimeStart;
                TimeSpan deltaTimeCompare = dateTimeEndCompare - dateTimeStartCompare;
                TimeSpan span;
                if (deltaTime > deltaTimeCompare)
                {
                    span = deltaTime - deltaTimeCompare;
                    UpdateRange(span, settingStorage.DateTimeStartCompare, settingStorage.DateTimeEndCompare, out dateTimeStartCompare, out dateTimeEndCompare);
                }
                else if (deltaTime < deltaTimeCompare)
                {
                    span = deltaTimeCompare - deltaTime;
                    UpdateRange(span, settingStorage.DateTimeStart, settingStorage.DateTimeEnd, out dateTimeStart, out dateTimeEnd);
                }
            }

            SetTimeScaleAlignment(axisX, dateTimeStart, dateTimeEnd);
            if (secondaryAxisX2D != null)
                SetTimeScaleAlignment(secondaryAxisX2D, dateTimeStartCompare, dateTimeEndCompare);

            UpdateXAxisSpacing();
            UpdateYAxisSpacing();
        }

        private void UpdateRange(TimeSpan span, DateTime startDefTime, DateTime endDefTime, out DateTime startTime, out DateTime endTime)
        {
            startTime = startDefTime;
            endTime = endDefTime;

            if (settingStorage == null || !settingStorage.Compare)
            {
                endTime = endTime.Add(span);
            }
            else
            {
                switch (settingStorage.ComparingAlignment)
                {
                    case HorizontalComparisonAlignment.Left:
                        endTime = endTime.Add(span);
                        break;
                    case HorizontalComparisonAlignment.Center:
                        long tick = span.Ticks / 2;
                        startTime = startTime.AddTicks(-tick);
                        endTime = endTime.AddTicks(tick);
                        break;
                    case HorizontalComparisonAlignment.Right:
                        startTime = startTime.Add(-span);
                        break;
                    case HorizontalComparisonAlignment.Stretch:
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdateXAxisSpacing()
        {
            UpdateXAxisSpacing(axisX);
            if (secondaryAxisX2D != null)
                UpdateXAxisSpacing(secondaryAxisX2D);
        }

        void UpdateXAxisSpacing(AxisX2D axisX2D)
        {
            double gridSpacing = 0.0;
            if (axisX2D.ActualWholeRange.ActualMaxValue is DateTime && axisX2D.ActualWholeRange.ActualMinValue is DateTime)
            {
                var delta = (DateTime)axisX2D.ActualWholeRange.ActualMaxValue - (DateTime)axisX2D.ActualWholeRange.ActualMinValue;
                double division = Math.Max(1.0, XMajorCount);

                DateTimeGridAlignment dtga;
                if (axisX2D.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                    dtga = (axisX2D.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridAlignment;
                else
                    dtga = (axisX2D.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridAlignment;

                if (dtga == DateTimeGridAlignment.Millisecond)
                    gridSpacing = delta.TotalMilliseconds / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Second)
                    gridSpacing = delta.TotalSeconds / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Minute)
                    gridSpacing = delta.TotalMinutes / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Hour)
                    gridSpacing = delta.TotalHours / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Day)
                    gridSpacing = delta.TotalDays / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Week)
                    gridSpacing = (delta.TotalDays / daysPerWeek) / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Month)
                    gridSpacing = (delta.TotalDays / daysPerMonth) / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Quarter)
                    gridSpacing = (delta.TotalDays / (daysPerYears / 4.0)) / XMajorCount;
                else if (dtga == DateTimeGridAlignment.Year)
                    gridSpacing = (delta.TotalDays / daysPerYears) / XMajorCount;
            }

            if (axisX2D.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                (axisX2D.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
            else
                (axisX2D.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
        }

        void UpdateYAxisSpacing()
        {
            double gridSpacing = 0.0;
            double? actualMaxValue = null;
            double? actualMinValue = null;

            try
            {
                actualMaxValue = Convert.ToDouble(axisY.ActualWholeRange.ActualMaxValue);
                actualMinValue = Convert.ToDouble(axisY.ActualWholeRange.ActualMinValue);
            }
            catch
            { }

            if (actualMaxValue.HasValue && actualMinValue.HasValue)
            {
                var delta = actualMaxValue.Value - actualMinValue.Value;
                double division = Math.Max(1.0, YMajorCount);
                gridSpacing = delta / division;
            }

            numericalScaleOptions.GridSpacing = Math.Max(1.0, gridSpacing);
        }

        string GetSanitizedName(string name)
        {
            string axisname = string.Format("{0}{1}", name, Properties.Settings.Default.SecondaryYAxis);
            string pattern = "[ \\[\\]\\+°\\~#%&@$£'.!*{})(/:<>?|\"-,]";
            string replacement = "_";

            Regex regEx = new Regex(pattern);
            return Regex.Replace(regEx.Replace(axisname, replacement), @"\s+", "_");
        }
        void BuildLineFromValues(String serie, List<MyDataValue> list, bool bAggregated = false, TableAggregation aggregation = TableAggregation.Min)
        {
            if (list == null)
                return;

            //chart.BeginInit();
            SetChartLineDataSource(serie, list, bAggregated: bAggregated, aggregation: aggregation);

            if (list.Count > 0)
            {
                if (!bIsFetching && (FilterType == DateSpan.All || FilterType == DateSpan.None))
                {
                    var firstTimestamp = list.First().SourceTimeStamp != maxDateTimeValue ? list.First().SourceTimeStamp : list[1].SourceTimeStamp;
                    var lastTimestamp = list.Last().SourceTimeStamp != minDateTimeValue ? list.Last().SourceTimeStamp : list[list.Count - 2].SourceTimeStamp;

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
                //UpdateAxisRange();
            }
            else
                System.Diagnostics.Debug.WriteLine(String.Format("No data for serie {0}", serie));

            //chart.EndInit();
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

        void SetTimeScaleAlignment(AxisX2D axisX2D, DateTime start, DateTime end)
        {
            if (axisX2D == null)
                return;

            //axisX2D.ActualWholeRange.SetAuto();
            //axisX2D.ActualVisualRange.SetAuto();

            var datetimeDiff = end - start;
            var dateMargin = datetimeDiff.Ticks * AllValuesXMargin / 100;

            DateTime minValue;
            DateTime maxValue;

            if (datetimeDiff.TotalSeconds <= 1)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Millisecond, DateTimeMeasureUnit.Millisecond);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalSeconds <= secondsPerMinute)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Second, DateTimeMeasureUnit.Millisecond);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalMinutes <= minutesPerHour)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Second, DateTimeMeasureUnit.Second);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalHours <= hoursPerDay)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Minute, DateTimeMeasureUnit.Minute);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerWeek)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerMonth)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else if (datetimeDiff.TotalDays <= daysPerYears * 10)
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Day, DateTimeMeasureUnit.Day);
                minValue = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 999);
            }
            else
            {
                SetDateTimeScaleOptions(axisX2D, DateTimeGridAlignment.Year, DateTimeMeasureUnit.Week);
                minValue = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, 1, 0, 0, 0);
            }

            if (FilterType == DateSpan.All)
            {
                try
                {
                    axisX2D.ActualWholeRange.MinValue = new DateTime(minValue.Ticks - dateMargin);
                    axisX2D.ActualWholeRange.MaxValue = new DateTime(maxValue.Ticks + dateMargin);
                }
                catch (ArgumentOutOfRangeException)
                {
                    axisX2D.ActualWholeRange.MinValue = minValue;
                    axisX2D.ActualWholeRange.MaxValue = maxValue;
                }
            }
            else
            {
                axisX2D.ActualWholeRange.MinValue = minValue;
                axisX2D.ActualWholeRange.MaxValue = maxValue;
            }

        }

        void SetDateTimeScaleOptions(AxisX2D axisX2D, DateTimeGridAlignment ga, DateTimeMeasureUnit mu)
        {
            if (axisX2D == null)
                return;
            DateTimeScaleOptionsBase dtso = axisX2D.DateTimeScaleOptions;
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
                    SetDateTimeScaleMeasureUnitMultiplier(axisX2D, MeasureUnitMultiplier);
                }
            }
        }

        void SetDateTimeScaleMeasureUnitMultiplier(AxisX2D axisX2D, int measureUnitMultiplier)
        {
            if (axisX2D == null)
                return;
            DateTimeScaleOptionsBase dtso = axisX2D.DateTimeScaleOptions;
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
        Dictionary<string, List<Task>> pendingTask = new Dictionary<string, List<Task>>();
        object seriesObject = new object();
        TaskScheduler sc;
        bool SetDataSources(bool bFromSettings = false)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                visualRangeBeforeAggregationToggle = null;
                return false;
            }

            ClearConstantLines();
            //diagram.Series.Clear();

            if (listSeries.Count == 0)
            {
                if (!bControlLoaded)
                {
                    bControlLoaded = true;
                    OnControlLoaded();
                }
                visualRangeBeforeAggregationToggle = null;
                return false;
            }
            //CheckUfw(true);
            SetBusy(true);
            {
                InitSettingsForSeriesManagement();

                var exceptions = new List<Exception>();
                if (sc == null)
                    sc = TaskScheduler.FromCurrentSynchronizationContext();
                if (tokenSource == null)
                {
                    tokenSource = new CancellationTokenSource();
                    ct = tokenSource.Token;
                }
                string aggTablePostFiss = GetTablePostFiss();
                var toBeUpdated = (from s in settingStorage.mapSeries.Values
                                   where s.UseTableAggregation &&
                                    (s.MinAggregation || s.MaxAggregation || s.AvgAggregation) &&
                                    s.IsVisible
                                   select s).FirstOrDefault();
                aggregated.Visibility = toBeUpdated != null ? Visibility.Visible : Visibility.Collapsed;

                listSeries.ForEach(serie =>
                {
                   UpdateDataSource(serie, exceptions, bFromSettings);
                });
            }
            return true;
        }
        Dictionary<string, DataValue> condValues = new Dictionary<string, DataValue>();
        void UpdateDataSource(string serie, List<Exception> exceptions, bool bFromSettings = false)
        {
            InitSettingsForSeriesManagement();
            
            if (!settingStorage.mapSeries.ContainsKey(serie))
                return;

            string aggTablePostFiss = GetTablePostFiss();
            bool dlrsource = settingStorage.mapSeries[serie].DLRSorce;
            string condGuid = $"{condPrefix}{settingStorage.mapSeries[serie].SGuid}";
            if (mapHandlers.ContainsKey(condGuid))
                mapHandlers[condGuid].ValueChanged -= PenItem_ValueChanged;
            bool serieHasCondTag = OpcuaEntityReference.ContainsKey(condGuid);
            bool waitForGoodValue = serieHasCondTag && (bFromSettings || !IsStatusGood(condGuid));

            if (dlrsource)
            {
                if (waitForGoodValue)
                {
                    if (mapHandlers.ContainsKey(condGuid))
                        mapHandlers[condGuid].ValueChanged += PenItem_ValueChanged;
                }
                else
                {
                    if(serieHasCondTag)
                    {
                        if (condValues.ContainsKey(condGuid))
                            condValues[condGuid] = OpcuaEntityReference[condGuid].MonitoredItemViewModel.DataValue;
                        else
                            condValues.Add(condGuid, OpcuaEntityReference[condGuid].MonitoredItemViewModel.DataValue);
                    }

                    SetSerieDataSources(serie, exceptions, aggTablePostFiss, bFromSettings);
                }
            }
            else
                SetSerieDataSources(serie, exceptions, aggTablePostFiss, bFromSettings);
        }

        bool IsStatusGood(string key)
        {
            return OpcuaEntityReference.ContainsKey(key) && OpcuaEntityReference[key] != null && 
                    OpcuaEntityReference[key].MonitoredItemViewModel != null &&
                    OpcuaEntityReference[key].MonitoredItemViewModel.DataValue != null &&
                    Opc.Ua.StatusCode.IsGood(OpcuaEntityReference[key].MonitoredItemViewModel.DataValue.StatusCode);
        }
        void TerminateSetDataSource(List<Exception> exceptions, bool bFromSettings)
        {
            if (pendingTask.Count == 0)
            {
                if (visualRangeBeforeAggregationToggle == null)
                    UpdateAxisRange();
                diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
                EnableToolbars(IsInStop);
                if (bIsRestarting)
                    bIsRestarting = false;
                if (IsInStop)
                    SetLegendSource();
                SetBusy(false);
                //CheckUfw();

                if (exceptions.Count > 0)
                    while (exceptions.Count > 0)
                    {
                        var item = exceptions[0];
                        var aggregated = (from c in exceptions.AsParallel() where c.GetType() == item.GetType() && c.Message == item.Message select c).ToList();
                        if (aggregated.Count > 0)
                        {
                            exceptions.RemoveAll((c) => aggregated.Contains(c));
                            ShowError(aggregated[0]);
                            logLicense.Error(Properties.Resources.Error, aggregated[0]);
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(Document, Properties.Settings.Default.DataAnalysisControl,
                           DateTime.UtcNow, $"{Properties.Resources.Error}: {aggregated[0].StackTrace}", System.Diagnostics.EventLogEntryType.Error);
                        }
                    }

                if (!bControlLoaded)
                {
                    bControlLoaded = true;
                    OnControlLoaded();
                }
                else if (bFromSettings && OperatingMode != OperatingMode.OnlyStop)
                    SetRealTimeSource(true);
                SetPrintGridDataSource();
            }
        }

        bool bSettingLegendSource;
        void SetLegendSource()
        {
            var source = settingStorage.mapSeries.Values.ToList();
            bSettingLegendSource = true;
            legend_GridControl.ItemsSource = null;
            legend_GridControl.ItemsSource = source;
            bSettingLegendSource = false;
            //legend_GridControl.SelectedItem = null;
        }

        private string GetTablePostFiss()
        {
            return GetTablePostFiss(settingStorage.DateTimeStart, settingStorage.DateTimeEnd);
        }

        private bool NeedToAggregate(SerieSettings serieSettings)
        {
            var utcStart = settingStorage.DateTimeStart;
            var utcEnd = settingStorage.DateTimeEnd;
            return CanUseAggregatedTables(serieSettings) &&
                (utcStart == utcEnd || (utcEnd - utcStart).TotalMinutes > Properties.Settings.Default.MinuteTableDateDiff) && (serieSettings.MinAggregation || serieSettings.MaxAggregation || serieSettings.AvgAggregation);
        }

        bool CanUseAggregatedTables(SerieSettings serieSettings)
        {
            return !string.IsNullOrEmpty(serieSettings.HistoricalName) && serieSettings.UseTableAggregation
                && serieSettings.DLRSorce && UFUAEditor.UsesAggreagatedTables(Document, serieSettings.HistoricalName);
        }
        CancellationTokenSource tokenSource;
        CancellationToken ct;
        private void ShowError(Exception exception, string errorMsg = null)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var msg = errorMsg != null ? errorMsg : (exception.InnerException != null ? exception.InnerException.Message : exception.Message);
                var error = string.Format("{0}: {1}", Name, msg);
                logLicense.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Settings.Default.DataAnalysisControl,
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
            });
        }

        void CallFilterTypeCommand(DateSpan? newValue)
        {
            if (newValue == null || bDispose || bCallingFilterCommand)
                return;

            bCallingFilterCommand = true;
            if (newValue != DateSpan.All && btnAll.IsChecked == true)
                btnAll.IsChecked = false;
            if (newValue == DateSpan.All)
                ClearAllCompareSeries(); 

            var newDate = (DateSpan)newValue;
            if (SetTimeRange(newDate))
                bIsFetching = true;
            if (!SetDataSources(false))
                bIsFetching = false;
            bCallingFilterCommand = false;
        }

        void SetDataSources_KeepingTimeRange()
        {
            bIsFetching = true;
            if (!SetDataSources())
                bIsFetching = false;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SetTimeRange();
            SetDataSources();
        }

        void MovePrevNext(bool bPrev)
        {
            if (bDispose || !bLoaded)
                return;

            if (axisX.ActualVisualRange != null && axisX.ActualVisualRange.ActualMinValue is DateTime && axisX.ActualVisualRange.ActualMaxValue is DateTime)
            {
                var visualRangeMin = (DateTime)axisX.ActualVisualRange.ActualMinValue;
                var visualRangeMax = (DateTime)axisX.ActualVisualRange.ActualMaxValue;
                var dates = GetSelectedTimeRangeCombo(visualRangeMin, visualRangeMax, bPrev);
                if (dates.start == (DateTime)axisX.ActualVisualRange.ActualMinValue && dates.end == (DateTime)axisX.ActualVisualRange.ActualMaxValue)
                    return;
                settingStorage.DateTimeEnd = dates.end;
                settingStorage.DateTimeStart = dates.start;
                SetDataSources_KeepingTimeRange();
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            MovePrev();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MoveNext();
        }

        private void GoToNow(object sender, RoutedEventArgs e)
        {
            GoToNow();
        }

        public void MovePrev()
        {
            MovePrevNext(true);
        }

        public void MoveNext()
        {
            MovePrevNext(false);
        }

        public void GoToNow()
        {
            if (bDispose || !bLoaded)
                return;

            if (axisX.ActualVisualRange != null && axisX.ActualVisualRange.ActualMaxValue is DateTime && axisX.ActualVisualRange.ActualMinValue is DateTime)
            {
                TimeSpan timeFrame = ViewTimeFrame;
                try
                {
                    timeFrame = (DateTime)axisX.ActualVisualRange.ActualMaxValue - (DateTime)axisX.ActualVisualRange.ActualMinValue;
                    settingStorage.DateTimeStart = DateTime.Now - timeFrame;
                    settingStorage.DateTimeEnd = DateTime.Now;
                }
                catch { }
                SetDataSources_KeepingTimeRange();
            }
        }

        void CheckEnabledButtons()
        {
            var bCanMove = FilterType != DateSpan.All;
            btnPrev.IsEnabled = bCanMove;
            btnRefresh.IsEnabled = bCanMove;
            btnNext.IsEnabled = bCanMove;
            cmbTimeRange.IsEnabled = bCanMove && !RunningOnServer;
            compareCheckbox.IsEnabled = bCanMove && !RunningOnServer;
            if (!compareCheckbox.IsEnabled && settingStorage != null)
            {
                settingStorage.Compare = false;
            }
        }

        TimeSpan GetCurrentTimeRange()
        {
            switch (FilterType)
            {
                case DateSpan.Minute:
                    return TimeSpan.FromMinutes(1);
                case DateSpan.Hour:
                    return TimeSpan.FromHours(1);
                case DateSpan.Day:
                    return TimeSpan.FromDays(1);
                case DateSpan.Week:
                    return TimeSpan.FromDays(daysPerWeek);
                case DateSpan.Month:
                    return TimeSpan.FromDays(daysPerMonth);
                case DateSpan.Year:
                    return TimeSpan.FromDays(daysPerYears);
                default:
                    return TimeSpan.Zero;
            }
        }

        void SelectTimeRangeCombo(TimeSpan delta)
        {
            if (delta.TotalDays >= daysPerYears)
                SelectTimeRangeCombo(DateSpan.Year);
            else if (delta.TotalDays >= daysPerMonth)
                SelectTimeRangeCombo(DateSpan.Month);
            else if (delta.TotalDays >= daysPerWeek)
                SelectTimeRangeCombo(DateSpan.Week);
            else if (delta.TotalHours >= hoursPerDay * 2)
                SelectTimeRangeCombo(DateSpan.Day);
            else if (delta.TotalHours >= hoursPerDay)
                SelectTimeRangeCombo(DateSpan.Hour);
            else
                SelectTimeRangeCombo(DateSpan.Minute);
        }

        void SelectTimeRangeCombo(DateSpan tag)
        {
            if (!bInit || bDispose || cmbTimeRange.DataContext == null)
                return;

            var item = (from c in cmbTimeRange.DataContext as List<LocalizedTimeRange> where c.Value == tag select c).FirstOrDefault();
            cmbTimeRange.EditValue = item;
        }

        TimeRangeDates GetSelectedTimeRangeCombo(DateTime start, DateTime end, bool prev)
        {
            TimeRangeDates dates;
            dates.start = start;
            dates.end = end;

            var selectedItem = cmbTimeRange.EditValue as LocalizedTimeRange;

            if (selectedItem != null)
                TimeRangeHelper.GetSelectedTimeRangeCombo(start, end, selectedItem.Value, true/*UseAbsoluteRanges*/, prev, out dates, true);
            return dates;
        }

        private void ShowLabels_Click(object sender, RoutedEventArgs e)
        {
            bool bShow = btnShowLabels.IsChecked == true;
            var list = (from c in diagram.Series.OfType<Series>() select c).ToList();
            list.ForEach(serie =>
            {
                serie.LabelsVisibility = !RunningOnServer && bShow && serie.Points.Count <= MaxLabelPoints;
            });
        }
        private void ShowCrossHair_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInStop)
                return;
            chart.CrosshairEnabled = !chart.CrosshairEnabled;
        }

        void ClearConstantLines()
        {
            ConstantLines.Clear();
            (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select c).ToList().ForEach((yaxis) => {
                yaxis.ConstantLinesBehind.Clear();
            });
        }

        DispatcherOperation dpBoundDataChanged;
        void chart_BoundDataChanged(object sender, RoutedEventArgs arg)
        {
            if (dpBoundDataChanged == null ||
                dpBoundDataChanged.Status == DispatcherOperationStatus.Completed ||
                dpBoundDataChanged.Status == DispatcherOperationStatus.Aborted)
            {
                dpBoundDataChanged = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                {
                if (!bInit || bDesignmode || bDispose)
                    return;

                ClearConstantLines();

                if (!ShowStatisticLines)
                    return;

                double maxY = 1;
                double minY = 0;

                var diagram = (XYDiagram2D)chart.Diagram;
                foreach (var serie in diagram.Series)
                {
                    if (serie.Tag as String == compareTag || serie.Tag as String == minTag || serie.Tag as String == maxTag || serie.Tag as String == avgTag)
                        continue;

                    double minPrice = Double.MaxValue;
                    double maxPrice = 0;
                    double averagePrice = 0;

                    if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(serie.DisplayName))
                    {
                        var serieSettings = settingStorage.mapSeries[serie.DisplayName];
                        if (!serieSettings.IsStatisticEnabled)
                            continue;
                        if (serieSettings.listValues == null || serieSettings.listValues.Count == 0)
                            continue;

                        minPrice = serieSettings.minValue;
                        maxPrice = serieSettings.maxValue;
                        averagePrice = serieSettings.averageValue;
                    }
                    else if (serie.Points.Count == 0)
                        continue;
                    else
                    {
                        //foreach (SeriesPoint point in diagram.Series[0].Points)
                        foreach (SeriesPoint point in serie.Points)
                        {
                            double price = point.Value;
                            if (price < minPrice)
                                minPrice = price;
                            if (price > maxPrice)
                                maxPrice = price;
                            averagePrice += price;
                        }
                        //averagePrice /= diagram.Series[0].Points.Count;
                        averagePrice /= serie.Points.Count;
                    }

                    var minConstantLine = new ConstantLine(minPrice, String.Format(Properties.Resources.MinLine, serie.DisplayName));
                    minConstantLine.Brush = new SolidColorBrush(Colors.Green);
                    minConstantLine.Title.Foreground = new SolidColorBrush(Colors.Green);
                    var maxConstantLine = new ConstantLine(maxPrice, String.Format(Properties.Resources.MaxLine, serie.DisplayName));
                    maxConstantLine.Brush = new SolidColorBrush(Colors.Red);
                    maxConstantLine.Title.Foreground = new SolidColorBrush(Colors.Red);
                    var averageConstantLine = new ConstantLine(averagePrice, String.Format(Properties.Resources.AverageLine, serie.DisplayName));
                    averageConstantLine.Brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32));
                    averageConstantLine.Title.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32));

                    var lines = new ConstantLine[] { minConstantLine, maxConstantLine, averageConstantLine };
                    if (AutomaticGeneralScale)
                        ConstantLines.AddRange(lines);
                    else
                    {
                        var serieYAxis = GetSerieYAxis(settingStorage.mapSeries[serie.DisplayName].Name);
                        serieYAxis?.ConstantLinesBehind.AddRange(lines);
                    }

                    if (minPrice < minY)
                        minY = minPrice;
                    if (maxPrice > maxY)
                        maxY = maxPrice;
                }

                foreach (ConstantLine constantLine in ConstantLines)
                    constantLine.Title.Alignment = ConstantLineTitleAlignment.Far;

                    //chart.Animate();
                });
            }
        }

        private void chart_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {
            if (bDispose)
                return;
                    
            if (e.Series.Points != null)
            {
                e.Series.LabelsVisibility = !RunningOnServer && btnShowLabels.IsChecked == true && e.Series.Points.Count <= MaxLabelPoints;
                if (e.Series.Points.Count > MaxResolveOverlappingPoints)
                    e.Series.Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
            }

            if (e.Series.Tag as String == compareTag || e.Series.Tag as String == minTag || e.Series.Tag as String == maxTag || e.Series.Tag as String == avgTag)
            {
                string tag = e.Series.Tag as String;
                var name = e.Series.DisplayName.Replace(String.Format(" - {0}", tag), "");
                if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(name) &&
                    settingStorage.mapSeries[name].Color != Colors.Transparent)
                {
                    e.DrawOptions.Color = settingStorage.mapSeries[name].Color;
                    e.Handled = true;
                }
                else
                {
                    var ls = (from c in diagram.Series.OfType<Series>() where c.DisplayName == name select c).ToList();
                    if (ls.Count > 0)
                    {
                        e.DrawOptions.Color = chart.Palette[diagram.Series.IndexOf(ls[0])];
                        e.Handled = true;
                    }
                }
            }
            else if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(e.Series.DisplayName) &&
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

        private void UpdateLegendNearestPoints(DateTime dateTimeArgument)
        {
            foreach (var serie in diagram.Series)
            {
                SerieSettings s = null;
                if (settingStorage != null && settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(serie.DisplayName))
                    s = settingStorage.mapSeries[serie.DisplayName];
                if (s == null)
                    continue;
                // Get a value of series point that is nearest to the current cursor position.
                SeriesPoint point = GetSeriesPoint(serie, dateTimeArgument);
                if (point == null)
                    point = new SeriesPoint(dateTimeArgument, double.NaN);

                nearestPoints[serie.DisplayName] = point;
                s.LastClickedPoint.Date = SerieArgumentDateTimeFormat(point.Argument);
                s.LastClickedPoint.Value = point.Value;
            }
        }

        // Find a series point that is closest to an argument from chart coordinates.
        SeriesPoint GetSeriesPoint(Series series, DateTime argument, int index = 0, int max = 0)
        {
            using (var v = new WaitCursor())
            {
                if (series == null)
                    return null;

                if (series.Points.Count == 0)
                    return null;

                if (series.Points.Count == 1 ||
                    argument <= series.Points[0].DateTimeArgument)
                    return series.Points[0];

                if (argument >= series.Points[series.Points.Count - 1].DateTimeArgument)
                    return series.Points.Last();

                int midIndex = index == 0 ? series.Points.Count / 2 : (max + index) / 2;
                int maxIndex = index == 0 ? series.Points.Count : max;
                var midPoint = series.Points[midIndex];
                SeriesPoint found = null;
                var midPointNext = series.Points[midIndex + 1];

                if (midPoint.DateTimeArgument < argument && midPointNext.DateTimeArgument > argument)
                {
                    TimeSpan interval1 = argument - midPoint.DateTimeArgument;
                    TimeSpan interval2 = midPointNext.DateTimeArgument - argument;
                    found = interval1 <= interval2 ? midPoint : midPointNext;
                }
                else
                {
                    if (midPoint != null && argument > midPoint.DateTimeArgument)
                    {
                        midIndex = (maxIndex + midIndex) / 2;
                        found = GetSeriesPoint(series, argument, midIndex, maxIndex);
                    }
                    else
                    {
                        maxIndex = midIndex;
                        midIndex = index == 0 ? series.Points.Count / 2 : index / 2;

                        found = GetSeriesPoint(series, argument, midIndex, maxIndex);
                    }
                }
                return found;
            }
        }

        private void chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement)
                UpdateFocus(e.Source as FrameworkElement);

            Point position = e.GetPosition(chart);
            DiagramCoordinates diagramCoordinates = diagram.PointToDiagram(position);
            var hitInfo = chart.CalcHitInfo(position);
            var bInDiagram = hitInfo != null && hitInfo.InDiagram;
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
                    nearestSerie = selectedSerie;
                    bSerieSelected = true;
                    tagName.Text = hitInfo.Series.DisplayName;
                    legend_GridControl.SelectedItem = legend_GridControl.SelectedItem == selectedSerie ? null : selectedSerie;
                }
                e.Handled = true;
            }

            if (bInDiagram)
            {
                if (!diagramCoordinates.IsEmpty)
                    UpdateLegendNearestPoints(diagramCoordinates.DateTimeArgument);
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
                    chart.SelectedItem = nearestPoints[currentSerie.Name];
            }
        }

        private void Button_FetchData(object sender, RoutedEventArgs e)
        {
            ClearAllCompareSeries();

            settingStorage.ListRanges.Insert(0, new TimeRange()
            {
                DateTimeEnd = settingStorage.DateTimeEnd,
                DateTimeStart = settingStorage.DateTimeStart,
                DateTimeEndCompare = settingStorage.DateTimeEndCompare,
                DateTimeStartCompare = settingStorage.DateTimeStartCompare,
                Compare = settingStorage.Compare,
                ComparingAlignment = settingStorage.ComparingAlignment
            });
            while (settingStorage.ListRanges.Count > 10)
                settingStorage.ListRanges.Remove(settingStorage.ListRanges.Last());

            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settingStorage.ListRanges;

            bIsFetching = true;
            if (FilterType != DateSpan.None)
                FilterType = DateSpan.None;
            else if (!SetDataSources(false))
                bIsFetching = false;
        }
        private void Button_ClearRecent(object sender, RoutedEventArgs e)
        {
            settingStorage.ListRanges.Clear();
            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settingStorage.ListRanges;
        }

        private void listBoxRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsInStop)
                return;
            var timeRange = listBoxRecent.SelectedItem as TimeRange;
            if (timeRange == null)
                return;

            ClearAllCompareSeries();

            settingStorage.DateTimeEnd = timeRange.DateTimeEnd;
            settingStorage.DateTimeStart = timeRange.DateTimeStart;
            settingStorage.DateTimeEndCompare = timeRange.DateTimeEndCompare;
            settingStorage.DateTimeStartCompare = timeRange.DateTimeStartCompare;
            settingStorage.Compare = timeRange.Compare;
            bIsFetching = true;
            if (FilterType != DateSpan.None)
                FilterType = DateSpan.None;
            else if (!SetDataSources())
                    bIsFetching = false;
        }

        private void FromToday_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            if (((sender as FrameworkElement).Tag as string) == "compare")
                settingStorage.DateTimeStartCompare = today;
            else
                settingStorage.DateTimeStart = today;
        }
        private void TillTomorrow_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) + TimeSpan.FromDays(1);
            if (((sender as FrameworkElement).Tag as string) == "compare")
                settingStorage.DateTimeEndCompare = today;
            else
                settingStorage.DateTimeEnd = today;
        }
        private void ChartDesigner_Click(object sender, RoutedEventArgs e)
        {
            var designer = new ChartDesigner(chart);
            designer.Show(this.FindParent<Window>());
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (RunningOnServer)
                return;

            UtilitiesPrintHelper.PrintElement(chart as FrameworkElement, this.FindParent<Window>(), true, true, System.Drawing.Printing.PaperKind.A4);
        }

        private void PrintGrid_Click(object sender, RoutedEventArgs e)
        {
            if (RunningOnServer)
                return;

            UtilitiesPrintHelper.PrintControl(this.FindParent<Window>(), (IPrintableControl)gridControl.View, GetStorageName(), $"{Properties.Resources.GridTagName} {tagName.Text}", false, System.Drawing.Printing.PaperKind.A4);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            chart_BoundDataChanged(this, null);
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
                    StaticSeriesSettings = new SerieDataList(setting?.PenList);
                    ActualConfig = setting.Name;
                    DockLayout = setting.DockLayout;
                    ListViewLayout = setting.ListViewLayout;
                    GridLayout = setting.GridLayout;
                    FilterType = setting.FilterType;
                    UseAbsoluteRanges = setting.UseAbsoluteRanges;
                    configMemory.DataContext = MemorySettingList?.Names;
                    configMemory.EditValue = setting.Name;
                }
                else
                {
                    StaticSeriesSettings = designPenList != null ? new SerieDataList(designPenList) : new SerieDataList();
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
                    DockLayout = designDockLayout;
                    ListViewLayout = designListLayout;
                    GridLayout = designGridLayout;
                    FilterType = designFilterType;
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
        public SerieData GetPen(string title)
        {
            return StaticSeriesSettings == null ? null : (from SerieData pen in StaticSeriesSettings where pen.title == title select pen).FirstOrDefault();
        }
        public SerieData GetPen(int index)
        {
            if (StaticSeriesSettings == null || StaticSeriesSettings.Count <= index)
                return null;
            return StaticSeriesSettings[index];
        }
        public bool RemovePen(string title)
        {
            var foundPen = GetPen(title);
            if (foundPen != null)
            {
                StaticSeriesSettings.Remove(foundPen);
                return true;
            }
            return false;
        }
        public bool RemovePen(int index)
        {
            var foundPen = GetPen(index);
            if (foundPen != null)
            {
                StaticSeriesSettings.Remove(foundPen);
                return true;
            }
            return false;
        }
        public int GetPensNumber()
        {
            return StaticSeriesSettings == null ? 0 : StaticSeriesSettings.Count;
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
        public SerieData AddPen(string penName, string variableName, bool useDatalogger, Color penColor, string historicalName = "", double min = 0, double max = 100, bool automaticScale = true, int plotType = 0)
        {
            if (!Enum.IsDefined(typeof(LineType), plotType))
                plotType = 0;

            var newData = new SerieData
            {
                title = penName,
                color = penColor,                
                authomaticscale = automaticScale,
                min = min,
                max = max,
                serieType = (LineType)plotType,
                historicalName = historicalName,
                dlrsource = useDatalogger,
                logarithmicbaseYScale = 10.0,
                showaxis = false,
                logarithmicYScale = false,
                thickness = 1,
                isVisible = true,
                isSet = true,
                guiId = Guid.NewGuid().ToString(),
                useeuminmax = false,
                addVirtualPoints = false
            };

            if (GetPen(penName) != null || !newData.SetTag(Document, variableName))
                return null;

            if (!String.IsNullOrEmpty(historicalName))
                newData.historicalName = historicalName;

            StaticSeriesSettings.Add(newData);
            return newData;
        }

        public void ReloadPens(SerieDataList penlist = null)
        {
            if (!bLoaded || bDispose)
                return;

            if (penlist == null)
                penlist = StaticSeriesSettings;

            bIsRestarting = true;
            EnableToolbars(false);
            Clear();
            StaticSeriesSettings = penlist != null ? new SerieDataList(penlist) : new SerieDataList();
            SetTimeRange();
            RestoreChartFromSettings();
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
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (StaticSeriesSettings == null)
                return ret;
            foreach (var pen in StaticSeriesSettings)
            {
                if (string.IsNullOrEmpty(pen.guiId))
                    pen.guiId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(pen.tagreferenceXml))
                    ret.Add(pen.CreateUniqueName(pen.title, ret.Keys.ToList()), pen.tagreferenceXml);
                if (!string.IsNullOrEmpty(pen.conditionalTagXml))
                    ret.Add(pen.CreateUniqueName($"{pen.title}_Condition", ret.Keys.ToList()), pen.conditionalTagXml);
            }
            return ret;
        }

        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute) || StaticSeriesSettings == null)
                return false;
            bool ret = false;

            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
            SerieDataList penList = new SerieDataList();
            var opcInit = OpcuaEntityReference;
            penList.AddRange(StaticSeriesSettings.ToList());
            var penTagList = (from pen in penList where pen.tagReference != null select pen).ToList();
            var condPenTagList = (from pen in penList where pen.conditionalTag != null select pen).ToList();
            int matchCount = penTagList.Count + condPenTagList.Count;
            penTagList.AddRange((from p in condPenTagList where !penTagList.Contains(p) select p).ToList());
            foreach (var pen in penTagList)
            {
                MatchTag(pen, relative, _relative, _absolute);
            }

            if (!bDesignmode)
                ret = matchChangedMap.Count == matchCount;

            if (!bDesignmode && ret)
                StaticSeriesSettings = penList;
            return ret;
        }

        void MatchTag(SerieData pen, String relative, OPCUAEntityReference _relative, OPCUAEntityReference _absolute)
        {
            try
            {
                string key = $"{pen.guiId}";
                string condKey = $"{condPrefix}{pen.guiId}";
                bool updateTag = false;
                bool updateCondTag = false;
                if (pen.tagreferenceXml != null && relative == pen.tagreferenceXml)
                {
                    matchChangedMap.Add(key);
                    TerminateExecution(key);
                    updateTag = true;
                }
                if (pen.conditionalTagXml != null && relative == pen.conditionalTagXml)
                {
                    matchChangedMap.Add(condKey);
                    TerminateExecution(condKey);
                    updateCondTag = true;
                }

                if (_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                {
                    _relative.Merge(_absolute);
                    if (updateTag)
                    {
                        pen.tagReference = _relative;
                        opcuaEntityReference[key] = _relative;
                    }
                    if (updateCondTag)
                    {
                        pen.conditionalTag = _relative;
                        opcuaEntityReference[condKey] = _relative;
                    }
                }
                else
                {
                    if (updateTag)
                    {
                        pen.tagReference = _absolute;
                        opcuaEntityReference[key] = _absolute;
                    }
                    if (updateCondTag)
                    {
                        pen.conditionalTag = _absolute;
                        opcuaEntityReference[condKey] = _absolute;
                    }
                }

                if (updateTag)
                {
                    if(pen.tagReference.HasValidValue)
                    {
                        var tagPath = $"{pen.tagReference.StartingAddress}/{pen.tagReference.RelativePath}";
                        if (tagPath.StartsWith("/") && tagPath.Length > 1)
                            tagPath = tagPath.Substring(1);
                        if (!pen.dlrsource)
                            pen.tagName = TagPathHelper.GetTagPath(null, tagPath, false);
                        else 
                        {
                            if (!string.IsNullOrEmpty(pen.historicalName))
                            {
                                UpdateDlrSettings(pen.historicalName);
                                if(!dlrSettings.ContainsKey(pen.historicalName))
                                    pen.historicalName = null;
                                else
                                {
                                    var columns = dlrSettings[pen.historicalName].Columns;
                                    pen.tagName = (from c in columns where c.ColumnTagName == tagPath select c.Name).FirstOrDefault();
                                    if (string.IsNullOrEmpty(pen.tagName))
                                        pen.historicalName = null;
                                }
                            }
                            else
                                pen.historicalName = null;
                        }

                        pen.nodeID = pen.tagReference.NodeIdViewModel?.nodeId.ToString() ?? pen.tagReference.ResolvedNodeId?.ToString();
                    }
                    PrepareExecution(key, pen.arrayindex);
                }
                if (updateCondTag)
                    PrepareExecution(key, pen.arrayindex);

            }
            catch (Exception)
            {
            }
        }

        void UpdateDlrSettings(string historicalName)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (Document == null || Document.Parent == null)
                return;

            if (UFUAEditor == null)
                UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

            if (UFUAEditor == null)
                return;

            lock (dlrSettings)
            {
                if (!dlrSettings.ContainsKey(historicalName))
                {
                    var _list = UFUAEditor.GetDataLoggerSettings(Document).ToList();
                    if (_list != null)
                    {
                        (from d in _list where historicalName == (d[0]) select d).ToList()./*AsParallel().ForAll*/ForEach(d =>
                        {
                            DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                            ds.Columns = new DataLoggerColumns();
                            var columns = UFUAEditor.GetDataLoggerColumnSettingList(Document, ds.Name) as List<List<String>>;
                            if (columns != null)
                            {
                                columns.ForEach(s =>
                                {
                                    ds.Columns.Add(new DataLoggerColumn() { Name = s[0], SourceTimeStampColumnName = s[1], AddSourceTimeStampColumn = bool.Parse(s[2]), ColumnTagName = s[3], ColumnTagGuid = s[4] });
                                });
                                dlrSettings.Add(ds.Name, ds);
                            }
                        });
                    }
                }
            }
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
                        mapHandlers = new Dictionary<string, SeriesDataHelpers>();

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
        Dictionary<string, SeriesDataHelpers> mapHandlers;
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
                        mapHandlers = new Dictionary<string, SeriesDataHelpers>();

                    if (OpcuaEntityReference[key] == null || !OpcuaEntityReference[key].IsValid || (OpcuaEntityReference[key].ResolvedNodeId == null && string.IsNullOrEmpty(OpcuaEntityReference[key].RelativePath)))
                        return;

                    if (mapHandlers.ContainsKey(key))
                        TerminateExecution(key);
                    mapHandlers[key] = new SeriesDataHelpers(arrayindex, OperatingMode != OperatingMode.OnlyStop || key.StartsWith(condPrefix))
                    {
                        control = this,
                        Key = key
                    };

                    if (key.StartsWith(condPrefix))
                    {
                        mapHandlers[key].Error += PenItem_OnError;
                        mapHandlers[key].ModelChanged += PenItem_ModelChanged;
                        mapHandlers[key].ValueChanged += PenItem_ValueChanged;
                    }
                    else if (OperatingMode != OperatingMode.OnlyStop)
                    {
                        mapHandlers[key].Error += PenItem_OnError;
                        mapHandlers[key].ModelChanged += PenItem_ModelChanged;
                        mapHandlers[key].ValueChanged += PenItem_ValueChanged;
                        mapHandlers[key].MinMaxRangeChanged += PenItem_MinMaxRangeChanged;
                    }
                    typeHelper.PrepareExecution(Properties.Resources.SessionName, Document as ScreenDocument, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
                }
                catch (Exception)
                {
                }
            }
        }

        #region SeriesDataHelpers Event Handlers
        private void PenItem_OnError(object sender, WPFPenHelpers.ErrorEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.ErrorMessage))
                ShowError(null, e.ErrorMessage);
        }

        private void ViewList_OnError(object sender, WPFPenHelpers.ErrorEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.ErrorMessage))
                ShowError(null, e.ErrorMessage);
        }

        private void PenItem_ModelChanged(object sender, ModelChangedEventArgs e)
        {
            var helper = (SeriesDataHelpers)sender;

            if (!e.Model.IsUserReadable || !e.Model.IsReadable)
                return;

            //var key = string.Format("{0}_{1}", helper.Key, helper.arrayIndex);
            UpdateModel(helper.Key, helper.arrayIndex, e.HumanReadable, e.Model);
        }

        private void PenItem_MinMaxRangeChanged(object sender, MinMaxRangeChangedEventArgs e)
        {
            var helper = (SeriesDataHelpers)sender;
            var key = string.Format("{0}_{1}", helper.Key, helper.arrayIndex);
            if (!mapKeySeries.ContainsKey(key) || !mapKeySeries[key].UseEUMinMax)
                return;

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (bDispose || !mapKeySeries.ContainsKey(key))
                    return;

                mapKeySeries[key].AbsoluteMin = e.MinValue;
                mapKeySeries[key].AbsoluteMax = e.MaxValue;
                UpdateAxisRange();
            });
        }

        private void PenItem_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var helper = (SeriesDataHelpers)sender;

            var key = string.Format("{0}_{1}", helper.Key, helper.arrayIndex);
            if (helper.Key.StartsWith(condPrefix))
            {
                UpdateCondMonitoredValue(helper.Key);
            }
            else
                UpdateMonitoredValue(key, e.NewValue);
        }
        #endregion

        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            SerieDataList penList = new SerieDataList();
            if (StaticSeriesSettings != null)
                penList.AddRange(StaticSeriesSettings.ToList());
            foreach (var pen in penList)
            {
                if (string.IsNullOrEmpty(pen.guiId))
                    pen.guiId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(pen.tagreferenceXml))
                {
                    if (map.ContainsKey(pen.guiId))
                        pen.tagreferenceXml = map[pen.guiId];
                    else
                        pen.tagreferenceXml = typeHelper.UpdateTag(pen.tagreferenceXml, map);
                    if (pen.tagReference.HasValidValue)
                    {
                        pen.tagName = TagPathHelper.GetTagPath(null, $"{pen.tagReference.RelativePath}", false);
                        try
                        {
                            if (!pen.tagReference.IsRelative)
                            {
                                (pen.historicalName) = UFUAEditor.GetHistorianName(Document, pen.tagReference.ResolvedNodeId);
                                (pen.nodeID) = pen.tagReference.ResolvedNodeId.ToString();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                if (!string.IsNullOrEmpty(pen.conditionalTagXml))
                {
                    if (map.ContainsKey($"{condPrefix}{pen.guiId}"))
                        pen.conditionalTagXml = map[$"{condPrefix}{pen.guiId}"];
                    else
                        pen.conditionalTagXml = typeHelper.UpdateTag(pen.conditionalTagXml, map);
                }
            }
            StaticSeriesSettings = penList;
        }
        internal void UpdateReferences(string key, string noideid)
        {
            var serie = (from s in StaticSeriesSettings where s.guiId == key select s).FirstOrDefault();
            if (serie != null)
            {
                serie.nodeID = noideid;
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
                var name = serie.tagName;
                if (!String.IsNullOrEmpty(serie.title))
                    name = serie.title;
                if (String.IsNullOrEmpty(name))
                    return;

                UpdateDataSource(name, exceptions);
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

            if (isComparing)
                return;

            if (zommingRangeList == null)
                zommingRangeList = new List<DevExpress.Xpf.Charts.Range>();

            if (isZoomingOnMouseWheel && e.AxisX == axisX)
                zommingRangeList.Add(new DevExpress.Xpf.Charts.Range() { MaxValue = e.OldXRange.MaxValue, MinValue = e.OldXRange.MinValue });

            isZoomingOnMouseWheel = false;
            var toBeUpdated = (from s in settingStorage.mapSeries.Values
                               where s.UseTableAggregation &&
                                (s.MinAggregation || s.MaxAggregation || s.AvgAggregation) &&
                                s.IsVisible
                               select s).FirstOrDefault();
            if (toBeUpdated == null)
                return;

            ManageZoom((DateTime)e.OldXRange.MinValue, (DateTime)e.OldXRange.MaxValue, (DateTime)e.NewXRange.MinValue, (DateTime)e.NewXRange.MaxValue);
        }
        bool isInZoomMode;
        private void ManageZoom(DateTime oldXRangeMinValue, DateTime oldXRangeMaxValue, DateTime newXRangeMinValue, DateTime newXRangeMaxValue, bool bForceUpdating = false)
        {
            if (listSeries == null || listSeries.Count == 0)
                return;

            if (settingStorage.mapSeries == null)
                settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

            string oldPostFiss = GetTablePostFiss(oldXRangeMinValue, oldXRangeMaxValue);
            string newPostFiss = GetTablePostFiss(newXRangeMinValue, newXRangeMaxValue);
#if DEBUG
            Debug.WriteLine($"{oldPostFiss} - {newPostFiss}");
#endif
            if (!oldPostFiss.Equals(newPostFiss) || bForceUpdating && (newXRangeMinValue != oldXRangeMinValue || newXRangeMaxValue != oldXRangeMaxValue))
            {
                settingStorage.DateTimeStart = newXRangeMinValue;
                settingStorage.DateTimeEnd = newXRangeMaxValue;
                isInZoomMode = true;
                SetDataSources();
            }
            else
                diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
        }

        private string GetTablePostFiss(DateTime startValue, DateTime endValue)
        {
            var utcStart = startValue;
            var utcEnd = endValue;

            if (utcStart == utcEnd)
                return Properties.Settings.Default.DayTablePostFix;
            else
            {
                var totalminute = (utcEnd - utcStart).TotalMinutes;
                if (totalminute > Properties.Settings.Default.DayTableDateDiff)
                {
                    aggregatedTableName.Content = Properties.Resources.DayAggregatedTableName;
                    return Properties.Settings.Default.DayTablePostFix;
                }
                else if (totalminute > Properties.Settings.Default.HourTableDateDiff)
                {
                    aggregatedTableName.Content = Properties.Resources.HourAggregatedTableName;
                    return Properties.Settings.Default.HourTablePostFix;
                }
                else if (totalminute > Properties.Settings.Default.MinuteTableDateDiff)
                {
                    aggregatedTableName.Content = Properties.Resources.MinAggregatedTableName;
                    return Properties.Settings.Default.MinTablePostFix;
                }
                else
                {
                    aggregatedTableName.Content = Properties.Resources.NormalAggregatedTableName;
                    return string.Empty;
                }
            }

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
                if (isComparing)
                {
                    e.Handled = true;
                    return;
                }

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
                            var toBeUpdated = (from s in settingStorage.mapSeries.Values
                                               where s.UseTableAggregation &&
                                                (s.MinAggregation || s.MaxAggregation || s.AvgAggregation) &&
                                                s.IsVisible
                                               select s).FirstOrDefault();
                            if (toBeUpdated == null)
                                return;

                            DateTime newXRangeMinValue = (DateTime)rangeInfo.MinValue;
                            DateTime newXRangeMaxValue = (DateTime)rangeInfo.MaxValue;
                            newXRangeMinValue = new[] { minDateTimeValue, newXRangeMinValue }.Max();
                            newXRangeMaxValue = new[] { maxDateTimeValue, newXRangeMaxValue }.Min();
                            ManageZoom(minDateTimeValue, maxDateTimeValue, newXRangeMinValue, newXRangeMaxValue, true);

                            e.Handled = true;
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

                if (pendingTask.Count != 0)
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
                SetTimeRange();
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
                SetTimeRange();
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
                    PenList = StaticSeriesSettings != null ? new SerieDataList(StaticSeriesSettings) : new SerieDataList(),
                    GridLayout = GridLayout,
                    DockLayout = DockLayout,
                    FilterType = FilterType,
                    UseAbsoluteRanges = UseAbsoluteRanges,
                    ListViewLayout = ListViewLayout,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.PenList = StaticSeriesSettings != null ? new SerieDataList(StaticSeriesSettings) : new SerieDataList();
                selected.GridLayout = GridLayout;
                selected.DockLayout = DockLayout;
                selected.ListViewLayout = ListViewLayout;
                selected.FilterType = FilterType;
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
        RelayCommand _filterTypeCommand;
        public ICommand FilterTypeCommand
        {
            get
            {
                if (_filterTypeCommand == null)
                {
                    _filterTypeCommand = new RelayCommand(
                        param => CallFilterTypeCommand(param as DateSpan?),
                        param => IsEnableFilterTypeCommand
                        );
                }
                return _filterTypeCommand;
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
        internal bool IsEnableFilterTypeCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                return true;
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
                if (!UserBasedRuntimeSettings)
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
            if (IsInStop && chart.SelectedItem as SeriesPoint != null && (chart.SelectedItem as SeriesPoint).Series != null && (e.Key == Key.Right || e.Key == Key.Left)) //Arrow key navigation through series points
            {
                var selPoint = chart.SelectedItem as SeriesPoint;

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
                
                chart.SelectedItem = nextPoint;
                //Updating legend with next point data
                AxisY2D yAxis = null;
                if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(selPoint.Series.DisplayName))
                {
                    string sanitized = GetSanitizedName(selPoint.Series.DisplayName);
                    yAxis = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                    settingStorage.mapSeries[selPoint.Series.DisplayName].LastClickedPoint.Date = nextPoint.Argument;
                    settingStorage.mapSeries[selPoint.Series.DisplayName].LastClickedPoint.Value = nextPoint.Value;
                }

                //Autoscroll if the nextPoint is out of the current X axis'VisualRange
                AutoScroll(nextPoint);

                ControlCoordinates pointCoords;
                if (yAxis != null)
                    pointCoords = diagram.DiagramToPoint(nextPoint.DateTimeArgument, nextPoint.Value, axisX, yAxis);
                else
                    pointCoords = diagram.DiagramToPoint(nextPoint.DateTimeArgument, nextPoint.Value);

                UpdateLegendNearestPoints(nextPoint.DateTimeArgument);

                crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForEachSeries;
                diagram.ShowCrosshair(new Point(pointCoords.Point.X, pointCoords.Point.Y));
                crosshair.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
            }
        }

        void AutoScroll(SeriesPoint nextPoint)
        {
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

            if (secondaryAxisX2D != null && ((XYDiagram2D)chart.Diagram).SecondaryAxesX.OfType<SecondaryAxisX2D>().Contains(secondaryAxisX2D) &&
                secondaryAxisX2D.ActualVisualRange.ActualMinValue is DateTime && secondaryAxisX2D.ActualVisualRange.ActualMaxValue is DateTime)
            {
                bool bNextPointOverMax = nextPoint.DateTimeArgument >= (DateTime)secondaryAxisX2D.ActualVisualRange.ActualMaxValue;
                bool bNextPointBeforeMin = nextPoint.DateTimeArgument <= (DateTime)secondaryAxisX2D.ActualVisualRange.ActualMinValue;
                if (bNextPointBeforeMin || bNextPointOverMax)
                {
                    TimeSpan timeFrame = (DateTime)secondaryAxisX2D.ActualVisualRange.ActualMaxValue - (DateTime)secondaryAxisX2D.ActualVisualRange.ActualMinValue;
                    var timeFramePadding = new TimeSpan(timeFrame.Ticks / Properties.Settings.Default.KeyboardAutoScrollTimeFraction);
                    if (bNextPointOverMax)
                        secondaryAxisX2D.ActualVisualRange.SetMinMaxValues(nextPoint.DateTimeArgument - timeFrame + timeFramePadding, nextPoint.DateTimeArgument + timeFramePadding);
                    else
                        secondaryAxisX2D.ActualVisualRange.SetMinMaxValues(nextPoint.DateTimeArgument - timeFramePadding, nextPoint.DateTimeArgument + timeFrame - timeFramePadding);
                }
            }
        }
        private void UpdateFocus(FrameworkElement sender)
        {
            if (!IsElementContained(sender))
                return;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (!bDispose)
                {
                    chart.Focus();
                    diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
                }
            });
        }
        bool IsElementContained(object sender)
        {
            return sender == chart || (from c in chart.GetVisualChildrenOfType<FrameworkElement>()
                    where c == sender
                    select c).FirstOrDefault() != null;
        }
        private void chart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsElementContained(sender))
                return;
            if (!IsInStop)
                Pause(this, null);
            //if (OperatingMode != OperatingMode.OnlyStop)
            //    axisX.WholeRange.MinValue = e.Delta > 0 ? axisX.ActualVisualRange.ActualMinValue : minValueDate;
            if (isComparing)
            {
                e.Handled = true;
                return;
            }

            diagram.EnableAxisYNavigation = false;
        }

        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
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
            if (StaticSeriesSettings != null)
            {
                var _list = StaticSeriesSettings.Where(x => !string.IsNullOrEmpty(x.title)).Select(x => x.title);
                if (_list != null && _list.Count() > 0)
                    list.AddRange(_list);
            }
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            int i = 1;
            StaticSeriesSettings?.Where(x => !string.IsNullOrEmpty(x.title)).ToList().ForEach(x =>
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

        private void compareCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (settingStorage !=null && settingStorage.Compare)
            {
                settingStorage.DateTimeStartCompare = settingStorage.DateTimeStart.Add(-ViewTimeFrame);
                settingStorage.DateTimeEndCompare = settingStorage.DateTimeEnd.Add(-ViewTimeFrame);
            }
        }
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
            if (sender is DataAnalisysRT)
            {
                DataAnalisysRT control = sender as DataAnalisysRT;
                if (control.ReadLocalValue(DataAnalisysRT.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
                if (control.ReadLocalValue(DataAnalisysRT.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(DataAnalisysRT.AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("AxisLabelForeground", control.AxisLabelForeground);
                else
                    ret.Add("AxisLabelForeground", foreground);

                if (control.ReadLocalValue(DataAnalisysRT.LegendAreaForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("LegendAreaForeground", control.LegendAreaForeground);
                else
                    ret.Add("LegendAreaForeground", foreground);

                if (control.ReadLocalValue(DataAnalisysRT.PlotBackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("PlotBackground", control.PlotBackground);
                else
                    ret.Add("PlotBackground", background);

                if (control.ReadLocalValue(DataAnalisysRT.ControlBackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlBackground", control.ControlBackground);
                else
                    ret.Add("ControlBackground", background);

                if (control.ReadLocalValue(DataAnalisysRT.DiagramBackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("DiagramBackground", control.DiagramBackground);
                else
                    ret.Add("DiagramBackground", background);


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
}
