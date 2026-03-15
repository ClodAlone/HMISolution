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
using DevExpress.Xpf.Editors;
using DevExpress.Xpo;
using System.Windows.Automation;
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
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Charts.Designer;
using System.Windows.Media.Effects;
using DevExpress.Xpf.Printing;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using log4net;
using System.Windows.Markup;
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
using System.Runtime.Serialization.Formatters.Binary;
using UFUAHistorianModel;
using OPCUAViewModel;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using VFS;
using WPFUtilities.PropertyDataTemplate;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces;
using DynamicTagAwareHelper;
using DataAnalisysControl.Helpers;
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
using System.Windows.Data;
using DevExpress.Xpf.Editors.Settings;
using WPFUtilities.Extensions;
using HelpProvider.ComponentService;
using UFProjectManager.ComponentService;
using WPFUtilities.HistoricalHelpers;

namespace DataAnalisysControl
{
    /// <summary>
    /// Interaction logic for DataAnalisys.xaml
    /// </summary>
    [Obsolete("Use the DataAnalisysRTControl.DataAnalisys.xaml insted of this.")]
    public partial class DataAnalisys : UserControl, IEntityReference, IContainPropertyEditors, IDisposable, IDynamicTagAware, ISettingsHelper, INotifyPropertyVisibilityChanged, IConnectionAware
        , IStringIDAware
    {
        #region Dependency Properties
        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataAnalisys));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataAnalisys));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataAnalisys));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataAnalisys));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as DataAnalisys;
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
            var control = sender as DataAnalisys;
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
                ControlBackground = Background;
            }
        }
        #endregion


        #region DisableZoomBehaviour
        public static readonly DependencyProperty DisableZoomBehaviourProperty = DependencyProperty.Register("DisableZoomBehaviour", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false));
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
        public static readonly DependencyProperty TextPatternProperty = DependencyProperty.Register("TextPattern", typeof(string), typeof(DataAnalisys), new UIPropertyMetadata("{A}: {V:F2}"));
        [Browsable(false)]
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
        public static readonly DependencyProperty LegendAreaForegroundProperty = DependencyProperty.Register("LegendAreaForeground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnLegendAreaForegroundChanged), new CoerceValueCallback(OnCoerceLegendAreaForeground)));

        private static object OnCoerceLegendAreaForeground(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceLegendAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnLegendAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty PointPrecisionProperty = DependencyProperty.Register("PointPrecision", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(2, new PropertyChangedCallback(OnPointPrecisionChanged), new CoerceValueCallback(OnCoercePointPrecision)));

        private static object OnCoercePointPrecision(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoercePointPrecision((int)value);
            else
                return value;
        }

        private static void OnPointPrecisionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty AxisYTextPatternProperty = DependencyProperty.Register("AxisYTextPattern", typeof(string), typeof(DataAnalisys), new UIPropertyMetadata("{V:F2}"));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
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
        public static readonly DependencyProperty AxisLabelForegroundProperty = DependencyProperty.Register("AxisLabelForeground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxisLabelForegroundChanged), new CoerceValueCallback(OnCoerceAxisLabelForeground)));

        private static object OnCoerceAxisLabelForeground(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceAxisLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnAxisLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty DiagramBackgroundProperty = DependencyProperty.Register("DiagramBackground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnDiagramBackgroundChanged), new CoerceValueCallback(OnCoerceDiagramBackground)));

        private static object OnCoerceDiagramBackground(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceDiagramBackground((Brush)value);
            else
                return value;
        }

        private static void OnDiagramBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        Brush BlendForeground = new SolidColorBrush(Colors.White);
        private void UpdateControlLayout()
        {
            if (this.ReadLocalValue(AxisLabelForegroundProperty) != DependencyProperty.UnsetValue)
            {
                axisX.Label.Foreground = AxisLabelForeground;
                axisX.Label.FontSize = FontSize;
                axisY.Label.Foreground = AxisLabelForeground;
                axisY.Label.FontSize = FontSize;
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
                (from c in chart.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                    where c.Name == "PART_DomainBackground" || c.Name == "OutsideBorder"
                    select c).ToList().ForEach(x =>
                    {
                        x.Background = PlotBackground;
                    });
            }

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
                (axisX as AxisBase).MinorCount = XMinorCount;
            if (!YAutoGrid)
                (axisY as AxisBase).MinorCount = YMinorCount;
        }

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
        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34)), new PropertyChangedCallback(OnControlBackgroundChanged), new CoerceValueCallback(OnCoerceControlBackground)));

        private static object OnCoerceControlBackground(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceControlBackground((Brush)value);
            else
                return value;
        }

        private static void OnControlBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("DataAnalisysOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
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
        public static readonly DependencyProperty XAutoGridProperty = DependencyProperty.Register("XAutoGrid", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXAutoGridChanged), new CoerceValueCallback(OnCoerceXAutoGrid)));

        private static object OnCoerceXAutoGrid(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnXAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XGridLineVisibleProperty = DependencyProperty.Register("XGridLineVisible", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXGridLineVisible)));

        private static object OnCoerceXGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XGridLineColorProperty = DependencyProperty.Register("XGridLineColor", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXGridLineColorChanged), new CoerceValueCallback(OnCoerceXGridLineColor)));

        private static object OnCoerceXGridLineColor(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XMajorCountProperty = DependencyProperty.Register("XMajorCount", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(10, new PropertyChangedCallback(OnXMajorCountChanged), new CoerceValueCallback(OnCoerceXMajorCount)));

        private static object OnCoerceXMajorCount(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXMajorCount((int)value);
            else
                return value;
        }

        private static void OnXMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XMinorGridLineVisibleProperty = DependencyProperty.Register("XMinorGridLineVisible", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXMinorGridLineVisible)));

        private static object OnCoerceXMinorGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnXMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XMinorGridLineColorProperty = DependencyProperty.Register("XMinorGridLineColor", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnXMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceXMinorGridLineColor)));

        private static object OnCoerceXMinorGridLineColor(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnXMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty XMinorCountProperty = DependencyProperty.Register("XMinorCount", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(5, new PropertyChangedCallback(OnXMinorCountChanged), new CoerceValueCallback(OnCoerceXMinorCount)));

        private static object OnCoerceXMinorCount(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceXMinorCount((int)value);
            else
                return value;
        }

        private static void OnXMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YAutoGridProperty = DependencyProperty.Register("YAutoGrid", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYAutoGridChanged), new CoerceValueCallback(OnCoerceYAutoGrid)));

        private static object OnCoerceYAutoGrid(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYAutoGrid((bool)value);
            else
                return value;
        }

        private static void OnYAutoGridChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YGridLineVisibleProperty = DependencyProperty.Register("YGridLineVisible", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYGridLineVisible)));

        private static object OnCoerceYGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YGridLineColorProperty = DependencyProperty.Register("YGridLineColor", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYGridLineColorChanged), new CoerceValueCallback(OnCoerceYGridLineColor)));

        private static object OnCoerceYGridLineColor(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YMajorCountProperty = DependencyProperty.Register("YMajorCount", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMajorCountChanged), new CoerceValueCallback(OnCoerceYMajorCount)));

        private static object OnCoerceYMajorCount(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYMajorCount((int)value);
            else
                return value;
        }

        private static void OnYMajorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YMinorGridLineVisibleProperty = DependencyProperty.Register("YMinorGridLineVisible", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYMinorGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYMinorGridLineVisible)));

        private static object OnCoerceYMinorGridLineVisible(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYMinorGridLineVisible((bool)value);
            else
                return value;
        }

        private static void OnYMinorGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YMinorGridLineColorProperty = DependencyProperty.Register("YMinorGridLineColor", typeof(Brush), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnYMinorGridLineColorChanged), new CoerceValueCallback(OnCoerceYMinorGridLineColor)));

        private static object OnCoerceYMinorGridLineColor(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYMinorGridLineColor((Brush)value);
            else
                return value;
        }

        private static void OnYMinorGridLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty YMinorCountProperty = DependencyProperty.Register("YMinorCount", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(5, new PropertyChangedCallback(OnYMinorCountChanged), new CoerceValueCallback(OnCoerceYMinorCount)));

        private static object OnCoerceYMinorCount(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceYMinorCount((int)value);
            else
                return value;
        }

        private static void OnYMinorCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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

        public static readonly DependencyProperty ConnectionStringDesignModeProperty = DependencyProperty.Register("ConnectionStringDesignMode", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringDesignModeChanged), new CoerceValueCallback(OnCoerceConnectionStringDesignMode)));

        private static object OnCoerceConnectionStringDesignMode(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceConnectionStringDesignMode((String)value);
            else
                return value;
        }

        private static void OnConnectionStringDesignModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        [System.Xml.Serialization.XmlIgnore]
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
        public static readonly DependencyProperty MaxRecordsProperty = DependencyProperty.Register("MaxRecords", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxRecordsChanged), new CoerceValueCallback(OnCoerceMaxRecords)));

        private static object OnCoerceMaxRecords(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceMaxRecords((int)value);
            else
                return value;
        }

        private static void OnMaxRecordsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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

        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(30));
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

        #region UseAggregation
        public static readonly DependencyProperty UseAggregationProperty = DependencyProperty.Register("UseAggregation", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseAggregationChanged), new CoerceValueCallback(OnCoerceUseAggregation)));

        private static object OnCoerceUseAggregation(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceUseAggregation((bool)value);
            else
                return value;
        }

        private static void OnUseAggregationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
            if(bInit && !bDesignmode)
            {
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
        public static readonly DependencyProperty ShowPaletteProperty = DependencyProperty.Register("ShowPalette", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowPaletteChanged), new CoerceValueCallback(OnCoerceShowPalette)));

        private static object OnCoerceShowPalette(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceShowPalette((bool)value);
            else
                return value;
        }

        private static void OnShowPaletteChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty MaxAggregationFactorProperty = DependencyProperty.Register("MaxAggregationFactor", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(10, new PropertyChangedCallback(OnMaxAggregationFactorChanged), new CoerceValueCallback(OnCoerceMaxAggregationFactor)));

        private static object OnCoerceMaxAggregationFactor(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceMaxAggregationFactor((int)value);
            else
                return value;
        }

        private static void OnMaxAggregationFactorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty MaxResolveOverlappingPointsProperty = DependencyProperty.Register("MaxResolveOverlappingPoints", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(360, new PropertyChangedCallback(OnMaxResolveOverlappingPointsChanged), new CoerceValueCallback(OnCoerceMaxResolveOverlappingPoints)));

        private static object OnCoerceMaxResolveOverlappingPoints(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceMaxResolveOverlappingPoints((int)value);
            else
                return value;
        }

        private static void OnMaxResolveOverlappingPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty MaxLabelPointsProperty = DependencyProperty.Register("MaxLabelPoints", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxLabelPointsChanged), new CoerceValueCallback(OnCoerceMaxLabelPoints)));

        private static object OnCoerceMaxLabelPoints(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceMaxLabelPoints((int)value);
            else
                return value;
        }

        private static void OnMaxLabelPointsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoHideToolbar((bool)value);
            else
                return value;
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
            if(bDesignmode && bInit && newValue != oldValue)
            {
                if (newValue)
                {
                    var sbLeave = TryFindResource("MouseLeaveOpacity") as Storyboard;
                    sbLeave.Begin();
                }
                else
                {
                    Grid.SetRow(dockManager, 1);
                    Grid.SetRowSpan(dockManager, 1);

                    var sbOver = TryFindResource("MouseOverOpacity") as Storyboard;
                    sbOver.Begin();
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
        public static readonly DependencyProperty AutoCollapseHeightProperty = DependencyProperty.Register("AutoCollapseHeight", typeof(double), typeof(DataAnalisys), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseHeightChanged), new CoerceValueCallback(OnCoerceAutoCollapseHeight)));

        private static object OnCoerceAutoCollapseHeight(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseHeight((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty AutoCollapseWidthProperty = DependencyProperty.Register("AutoCollapseWidth", typeof(double), typeof(DataAnalisys), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseWidthChanged), new CoerceValueCallback(OnCoerceAutoCollapseWidth)));

        private static object OnCoerceAutoCollapseWidth(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAutoCollapseWidth((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty LogarithmicYScaleProperty = DependencyProperty.Register("LogarithmicYScale", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLogarithmicYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicYScale)));

        private static object OnCoerceLogarithmicYScale(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicYScale((bool)value);
            else
                return value;
        }

        private static void OnLogarithmicYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty LogarithmicBaseYScaleProperty = DependencyProperty.Register("LogarithmicBaseYScale", typeof(double), typeof(DataAnalisys), new UIPropertyMetadata(10.0, new PropertyChangedCallback(OnLogarithmicBaseYScaleChanged), new CoerceValueCallback(OnCoerceLogarithmicBaseYScale)));

        private static object OnCoerceLogarithmicBaseYScale(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceLogarithmicBaseYScale((double)value);
            else
                return value;
        }

        private static void OnLogarithmicBaseYScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty RotatedProperty = DependencyProperty.Register("Rotated", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRotatedChanged), new CoerceValueCallback(OnCoerceRotated)));

        private static object OnCoerceRotated(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceRotated((bool)value);
            else
                return value;
        }

        private static void OnRotatedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty AllowRuntimeChangesProperty = DependencyProperty.Register("AllowRuntimeChanges", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRuntimeChangesChanged), new CoerceValueCallback(OnCoerceAllowRuntimeChanges)));

        private static object OnCoerceAllowRuntimeChanges(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAllowRuntimeChanges((bool)value);
            else
                return value;
        }

        private static void OnAllowRuntimeChangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
                if (!bDesignmode && legend_GridControl != null) {
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
        #region AlwaysExpanded
        public static readonly DependencyProperty AlwaysExpandedProperty = DependencyProperty.Register("AlwaysExpanded", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAlwaysExpandedChanged), new CoerceValueCallback(OnCoerceAlwaysExpanded)));

        private static object OnCoerceAlwaysExpanded(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceAlwaysExpanded((bool)value);
            else
                return value;
        }

        private static void OnAlwaysExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty ShowStatisticLinesProperty = DependencyProperty.Register("ShowStatisticLines", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowStatisticLinesChanged), new CoerceValueCallback(OnCoerceShowStatisticLines)));

        private static object OnCoerceShowStatisticLines(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceShowStatisticLines((bool)value);
            else
                return value;
        }

        private static void OnShowStatisticLinesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty StaticSeriesSettingsProperty = DependencyProperty.Register("StaticSeriesSettings", typeof(SerieDataList), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnStaticSeriesSettingsChanged), new CoerceValueCallback(OnCoerceStaticSeriesSettings)));

        private static object OnCoerceStaticSeriesSettings(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceStaticSeriesSettings((SerieDataList)value);
            else
                return value;
        }

        private static void OnStaticSeriesSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
        public static readonly DependencyProperty DockLayoutProperty = DependencyProperty.Register("DockLayout", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDockLayoutChanged), new CoerceValueCallback(OnCoerceDockLayout)));

        private static object OnCoerceDockLayout(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDockLayout((String)value);
            else
                return value;
        }

        private static void OnDockLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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
            if (dockManager == null)
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
        public static readonly DependencyProperty ListViewLayoutProperty = DependencyProperty.Register("ListViewLayout", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnListViewLayoutChanged), new CoerceValueCallback(OnCoerceListViewLayout)));

        private static object OnCoerceListViewLayout(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceListViewLayout((String)value);
            else
                return value;
        }

        private static void OnListViewLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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

        #region AutomaticScale
        public static readonly DependencyProperty AutomaticScaleProperty = DependencyProperty.Register("AutomaticScale", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutomaticScaleChanged), new CoerceValueCallback(OnCoerceAutomaticScale)));

        private static object OnCoerceAutomaticScale(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceAutomaticScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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


        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(DataAnalisys), new UIPropertyMetadata(DateSpan.None, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
            if (bInit && !bDesignmode && !bUserInteractionSettings && oldValue != newValue)
            {
                SetTimeRange(newValue);
                SetDataSources();
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
        private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(DataAnalisys), new UIPropertyMetadata((double)0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        private static object OnCoerceMinimum(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceMinimum((double)value);
            else
                return value;
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                control.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
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
        [Category("Advanced")]
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

        #region Maximum
        private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(DataAnalisys), new UIPropertyMetadata((double)100, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        private static object OnCoerceMaximum(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceMaximum((double)value);
            else
                return value;
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                control.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
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
        [Category("Advanced")]
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

        #endregion


        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
        public static readonly DependencyProperty StartDateTitleProperty = DependencyProperty.Register("StartDateTitle", typeof(string), typeof(DataAnalisys), new UIPropertyMetadata(Properties.Resources.StartDate));
        [Browsable(false)]
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
        public static readonly DependencyProperty EndDateTitleProperty = DependencyProperty.Register("EndDateTitle", typeof(string), typeof(DataAnalisys), new UIPropertyMetadata(Properties.Resources.EndDate));
        [Browsable(false)]
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
        public static readonly DependencyProperty AllValuesXMarginProperty = DependencyProperty.Register("AllValuesXMargin", typeof(int), typeof(DataAnalisys), new UIPropertyMetadata(5, new PropertyChangedCallback(OnAllValuesXMarginChanged), new CoerceValueCallback(OnCoerceAllValuesXMargin)));

        private static object OnCoerceAllValuesXMargin(DependencyObject o, object value)
        {
            DataAnalisys DataAnalisys = o as DataAnalisys;
            if (DataAnalisys != null)
                return DataAnalisys.OnCoerceAllValuesXMargin((int)value);
            else
                return value;
        }

        private static void OnAllValuesXMarginChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys DataAnalisys = o as DataAnalisys;
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

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false));

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
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(DataAnalisys));
        [Browsable(false)]
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
        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register("DateTimeFormat", typeof(String), typeof(DataAnalisys), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDateTimeFormatChanged), new CoerceValueCallback(OnCoerceDateTimeFormat)));

        private static object OnCoerceDateTimeFormat(DependencyObject o, object value)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceDateTimeFormat((String)value);
            else
                return value;
        }

        private static void OnDateTimeFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys dataAnalisys = o as DataAnalisys;
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

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            DataAnalisys DataAnalisys = o as DataAnalisys;
            if (DataAnalisys != null)
                return DataAnalisys.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys DataAnalisys = o as DataAnalisys;
            if (DataAnalisys != null)
                DataAnalisys.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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
        List<MyDataValue> LoadData(int days)
        {
            List<MyDataValue> demoValues = new List<MyDataValue>();

            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < days; i++)
            {
                demoValues.Add(new MyDataValue
                {
                    SourceTimestamp = DateTime.Now.AddDays(i),
                    dValue = r.NextDouble() * Properties.Settings.Default.DemoMaxValue
                });
            }
            if (settingStorage == null)
                settingStorage = new SettingsStorage();
            settingStorage.StartTime = settingStorage.DateTimeStart = demoValues.First().SourceTimestamp;
            settingStorage.EndTime = settingStorage.DateTimeEnd = demoValues.Last().SourceTimestamp;

            return demoValues;
        }
        #endregion

        #region Declarations
        internal SerieDataList designPenList;
        IDataLayer dl;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        bool bLoaded;
        bool isTemplateApplied;
        bool bDesignmode;
        List<String> listSeries;// = new List<String>();
        SettingsStorage settingStorage;// = new SettingsStorage();
        TimeSpan timeSpanCompare;
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
        public readonly Dictionary<String, String> serieTypeKeys = new Dictionary<String, String>()
        {
            { Properties.Resources.LineSerie, "lineSeries" },
            { Properties.Resources.LineStepTitle, "lineStepSeries" },
            { Properties.Resources.LineStackedSerieTitle, "lineStackedSeries" },
            { Properties.Resources.LineFullStackedTitle, "lineFullStackedSeries" },
            { Properties.Resources.AreaSerieTitle, "areaSeries" },
            { Properties.Resources.AreaStepSerieTitle, "areaStepSeries"},
            { Properties.Resources.AreaStackedSerieTitle, "areaStackedSeries" },
            { Properties.Resources.AreaFullStackedSerieTitle, "areaFullStackedSeries" },
            { Properties.Resources.BarSideSerieTitle, "barSideSeries" }
        };

        const int secondsPerMinute = 60;
        const int minutesPerHour = 60;
        const int hoursPerDay = 24;
        const int daysPerWeek = 7;
        const int daysPerMonth = 30;
        const int daysPerYears = 365;

        Dictionary<string, SeriesPoint> nearestPoints = new Dictionary<string, SeriesPoint>();

        private Stream TemplateStream
        {
            get
            {
                return ExtractFileFromResource.Extract(Assembly.GetExecutingAssembly(), string.Format("{0}.Resources.{1}", typeof(DataAnalisys).Namespace, "DATemplates.xaml"));        
            }
        }

        internal Dictionary<string, OPCUAEntityReference> OpcuaEntityReference
        {
            get
            {
                if (opcuaEntityReference == null)
                {
                    opcuaEntityReference = new Dictionary<string, OPCUAEntityReference>();
                     if(StaticSeriesSettings != null)
                        for (int i = 0; i < StaticSeriesSettings.Count(); i++)
                        {
                            if (string.IsNullOrEmpty(StaticSeriesSettings[i].guiId))
                                StaticSeriesSettings[i].guiId = Guid.NewGuid().ToString();
                            opcuaEntityReference[StaticSeriesSettings[i].guiId] = StaticSeriesSettings[i].tagReference;
                        }
                }
                return opcuaEntityReference;
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
        List<string> matchChangedMap = new List<string>();
        #endregion

        #region Constructor
        private DelayedSingleActionInvoker SizeChangedInvoker;
        internal bool bSmartSettingsEditing;
        bool bInit;
        public DataAnalisys()
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
                        }

                        configMemory.EditValue = Properties.Settings.Default.DesignSettingName;
                        (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;

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
                        bDesignmode = bDesignmode || DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;

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
                            DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                            chart.CrosshairOptions.ShowArgumentLine = false;
                            chart.CrosshairOptions.ShowValueLine = false;
                            chart.CrosshairOptions.ShowArgumentLabels = false;
                            chart.CrosshairOptions.ShowValueLabels = false;
                            chart.CrosshairOptions.ShowCrosshairLabels = false;
                            diagram.EnableAxisXNavigation = false;
                            diagram.EnableAxisYNavigation = false;
                            var dataSource = LoadData(Properties.Settings.Default.DemoMaxDays);
                            diagram.Series.Clear();
                            ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Clear();
                            AddChartLine("Value", "Value", "areaSeries", 1, Color.FromArgb(255, 65, 90, 120), false, false, 0.0, dataSource);
                            UpdateAxisRange();
                            OverrideBaseProperties();
                            if (AutoHideToolbar)
                            {
                                var sbLeave = TryFindResource("MouseLeaveOpacity") as Storyboard;
                                sbLeave.Begin();
                            }
                            else
                            {
                                Grid.SetRow(dockManager, 1);
                                Grid.SetRowSpan(dockManager, 1);

                                var sbOver = TryFindResource("MouseOverOpacity") as Storyboard;
                                sbOver.Begin();
                            }

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

                            if (RunningOnServer)
                            {
                                toolbarSettings.IsVisible = false;
                                toolbarPrint.Visibility = Visibility.Collapsed;
                                btnPrint.IsVisible = false;
                                AutoHideToolbar = false;

                                HideAllHidden();
                                RestoreChartFromSettings();
                            }
                            else
                            {
                                RestoreChartFromSettings();
                            }

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

                            InitPanels();

                            palettePanel.Content = new PaletteChooser(chart);
                            startTime.TouchDown += OnTouchDown;
                            endTime.TouchDown += OnTouchDown;
                            startTime.LostFocus += OnLostFocus;
                            endTime.LostFocus += OnLostFocus;

                            OverrideBaseProperties();

                            if (AutoHideToolbar)
                            {
                                toolbar.MouseEnter += toolbar_MouseEnter;
                                toolbar.MouseLeave += toolbar_MouseLeave;
                            }
                            else
                            {
                                Grid.SetRow(dockManager, 1);
                                Grid.SetRowSpan(dockManager, 1);

                                toolbar_MouseEnter(null, null);
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

        void ApplyDateTimeFormat()
        {
            var format = GetDateTimeFormat();

            axisX.Label.TextPattern = String.Format("{{A{0}}}", String.Format(":{0}", format));

            if (!bDesignmode)
            {
                axisX.CrosshairAxisLabelOptions.Pattern = String.Format("{{A:{0}}}", format);
                var series = (from Series serie in diagram.Series where serie is XYSeries2D select serie as XYSeries2D).ToList();
                foreach (var serie in series)
                    serie.CrosshairLabelPattern = String.Format("{{S}}\n{{V:F2}}\n{{A:{0}}}", format);
            }
        }

        string GetDateTimeFormat()
        {
            var format = DateTimeFormat;
            if (String.IsNullOrEmpty(format?.Trim()))
                format = String.Format("{0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern);
            return format;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!isTemplateApplied && RunningOnServer)
                SetWebAsset();
            isTemplateApplied = true;
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
        public void SetTimeRange(DateSpan filterType)
        {
            DateTime date1;
            DateTime date2;

            CheckEnabledButtons();
            SelectTimeRangeCombo(filterType);

            switch (filterType)
            {
                case DateSpan.All:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.All, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Minute:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Minute, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Hour:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Hour, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Day:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Day, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Week:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Week, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Month:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Month, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                case DateSpan.Year:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Year, UseAbsoluteRanges);
                    settingStorage.DateTimeStart = date1;
                    settingStorage.DateTimeEnd = date2;
                    break;
                default:
                    break;
            }
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

            (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;
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

                configMemory.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist,Properties.Resources.ActualSettings);
                editGeneralSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EditSettings", stringlist,Properties.Resources.EditSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist,Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                legendPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendTitle", stringlist,Properties.Resources.LegendTitle);
                timeRangePanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TimeRange", stringlist,Properties.Resources.TimeRange);
                gridPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTitle", stringlist,Properties.Resources.GridTitle);

                Text1.Text = StartDateTitle = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist,Properties.Resources.StartDate);
                Text2.Text = EndDateTitle = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist,Properties.Resources.EndDate);

                configMemory.EditValue = ActualConfig;
                todayButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TodayBtnTooltip", stringlist,Properties.Resources.TodayBtnTooltip);
                tomorrowButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TomorrowBtnTooltip", stringlist,Properties.Resources.TomorrowBtnTooltip);
                btnFetchData.ToolTip = btnFetchData.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_FetchTitle", stringlist,Properties.Resources.FetchTitle);
                btnClearRecent.ToolTip = btnClearRecent.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ClearRecentTitle", stringlist,Properties.Resources.ClearRecent);
                    
                btnRefresh.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshTitle", stringlist,Properties.Resources.RefreshTitle);
                btnShowCrossHair.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CrossHairTitle", stringlist,Properties.Resources.CrossHairTitle);
                chkShowStatistics.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsTitle", stringlist,Properties.Resources.StatisticsTitle);
                chkUseAbsoluteRanges.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StatisticsTitle", stringlist, Properties.Resources.UseAbsoluteRangesTitle);
                chkUseAggregationTitle.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AggregationTitle", stringlist, Properties.Resources.UseAggregationTitle);
                btnShowLabels.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LabelTitle", stringlist,Properties.Resources.LabelTitle);
                btnExpand.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandTitle", stringlist,Properties.Resources.ExpandTitle);
                btnPrev.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevTitle", stringlist,Properties.Resources.PrevTitle);
                btnNext.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextTitle", stringlist,Properties.Resources.NextTitle);
                btnAll.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AllTitle", stringlist,Properties.Resources.AllTitle);
                btnMin.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MinTitle", stringlist,Properties.Resources.MinTitle);
                btnHour.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HourTitle", stringlist,Properties.Resources.HourTitle);
                btnDay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayTitle", stringlist,Properties.Resources.DayTitle);
                btnWeek.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeekTitle", stringlist,Properties.Resources.WeekTitle);
                btnMonth.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthTitle", stringlist,Properties.Resources.MonthTitle);
                btnYear.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_YearTitle", stringlist,Properties.Resources.YearTitle);
                btnPrintGrid.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintTitle", stringlist,Properties.Resources.PrintTitle);
                btnPrint.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrintTitle", stringlist,Properties.Resources.PrintTitle);

                maxRecord.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MaxRecordTitle", stringlist,Properties.Resources.MaxRecordTitle);
                cmbCompare.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CompareTitle", stringlist,Properties.Resources.CompareTitle);
                startTextTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist,Properties.Resources.StartDate);
                endTextTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndDate);

                txtGridDataTooltip.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridDataTooltip", stringlist, Properties.Resources.GridDataTooltip);
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

                gridTagName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_GridTagName", stringlist, Properties.Resources.GridTagName);

                foreach (ComboBoxEditItem item in (cmbTimeRange.EditSettings as ComboBoxEditSettings).Items)
                {
                    if (item.Tag is DateSpan)
                    {
                        var dateSpan = (DateSpan)item.Tag;
                        if (dateSpan == DateSpan.Minute)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.Minute);
                        else if (dateSpan == DateSpan.Hour)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.Hour);
                        else if (dateSpan == DateSpan.Day)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.Day);
                        else if (dateSpan == DateSpan.Week)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.Week);
                        else if (dateSpan == DateSpan.Month)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Month);
                        else if (dateSpan == DateSpan.Year)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", stringlist, Properties.Resources.Year);
                    }
                }

                foreach (ComboBoxEditItem item in (cmbCompare.EditSettings as ComboBoxEditSettings).Items)
                {
                    if (item.Tag is DateSpan)
                    {
                        var dateSpan = (DateSpan)item.Tag;
                        if (dateSpan == DateSpan.None)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_None", stringlist, Properties.Resources.None);
                        else if (dateSpan == DateSpan.Minute)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.Minute);
                        else if (dateSpan == DateSpan.Hour)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.Hour);
                        else if (dateSpan == DateSpan.Day)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.Day);
                        else if (dateSpan == DateSpan.Week)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.Week);
                        else if (dateSpan == DateSpan.Month)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.Month);
                        else if (dateSpan == DateSpan.Year)
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", stringlist, Properties.Resources.Year);
                    }
                }

                TranslationHelper.TranlslateColumns(legend_GridControl.Columns, stringlist, stringPlaceolder);
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
            legend_GridControl.ItemsSource = null;
            legend_GridControl.ItemsSource = settingStorage.mapSeries.Values.ToList();
        }
        private void InitPanels()
        {
            if (!RunningOnServer)
                timeRangePanel.DataContext = settingStorage;
            startTextTitle.DataContext = settingStorage;
            endTextTitle.DataContext = settingStorage;
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

                if(oldMemoryList != null)
                    oldMemoryList.Clear();
                oldMemoryList = null;

                if (MemorySettingList != null)
                    MemorySettingList.Clear();
                MemorySettingList = null;

                if (StaticSeriesSettings != null)
                    StaticSeriesSettings.Clear();

                (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = null;
                legend_GridControl.ItemsSource = null;
                listBoxRecent.ItemsSource = null;

                if (helper is IDisposable)
                    (helper as IDisposable).Dispose();
                helper = null;
                SizeChanged -= OnSizeChanged;
                SizeChangedInvoker = null;

                if (disposing)
                {
                    if (saveAutoHiddenStream != null)
                        saveAutoHiddenStream.Dispose();

                    if (palettePanel.Content is IDisposable)
                        (palettePanel.Content as IDisposable).Dispose();
                    
                    gridControl.ItemsSource = null;
                    if(gridControl is IDisposable)
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
                    mapHandlers.Clear();

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
                if(settingStorage != null)
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
                            if(settingStorage.mapSeries[key].listValues != null)
                                settingStorage.mapSeries[key].listValues.Clear();
                            settingStorage.mapSeries[key].listValues = null;
                        }
                        settingStorage.mapSeries.Clear();
                    }
                    settingStorage.mapSeries = null;
                    settingStorage.Dispose();
                    settingStorage = null;
                }
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
                    serie.Points.Clear();
                    serie.EndInit();
                }

                diagram.SecondaryAxesY.Clear();
                diagram.Series.Clear();
                ConstantLines.Clear();

                diagram.PreviewMouseWheel -= diagram_PreviewMouseWheel;
                diagram.MouseWheel -= diagram_MouseWheel;
                diagram.Zoom -= diagram_Zoom;

                chart.PreviewKeyDown -= chart_PreviewKeyDown;
                chart.CustomDrawSeries -= chart_CustomDrawSeries;
                chart.PreviewMouseDown -= chart_MouseDown;
                chart.BoundDataChanged -= chart_BoundDataChanged;
                chart.CustomDrawCrosshair -= chart_CustomDrawCrosshair;

                nearestPoints.Clear();

                if (listSeries != null)
                    listSeries.Clear();

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
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        bool bLayoutSaved;
        string DictionaryNodeName = "Columns";

        #endregion

        #region UseAbsoluteRanges
        public static readonly DependencyProperty UseAbsoluteRangesProperty = DependencyProperty.Register("UseAbsoluteRanges", typeof(bool), typeof(DataAnalisys), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseAbsoluteRangesChanged), new CoerceValueCallback(OnCoerceUseAbsoluteRanges)));

        private static object OnCoerceUseAbsoluteRanges(DependencyObject o, object value)
        {
            DataAnalisys control = o as DataAnalisys;
            if (control != null)
                return control.OnCoerceUseAbsoluteRanges((bool)value);
            else
                return value;
        }

        private static void OnUseAbsoluteRangesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataAnalisys control = o as DataAnalisys;
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
            cmbCompare.IsEnabled = false;
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
                ScrollViewer _legend = legendScrollViewer;
                _legend.Height = 130.0;
                _legend.FontSize = FontSize;

                //legend_GridControl.Columns.Clear();
                legendPanel.Content = null;

                if (!mainChartGrid.Children.Contains(_legend))
                    mainChartGrid.Children.Add(_legend);
                Grid.SetRow(_legend, 1);
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
                    if (saveAutoHiddenStream!= null)
                    {
                        try
                        {
                            saveAutoHiddenStream.Seek(0, SeekOrigin.Begin);
                            dockManager.RestoreLayoutFromStream(saveAutoHiddenStream);
                            saveAutoHiddenStream.Dispose();
                            saveAutoHiddenStream = null;
                            dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Auto;
                            if (RunningOnServer)
                                legendPanel.Visibility = Visibility.Collapsed;
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
                    CheckUfw(true);
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
                    CheckUfw();
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
            if(dl == null)
                return list;

            List<UFUAAuditDataLog> ret = null;
            try
            {
                using(UnitOfWork _ufw = new UnitOfWork(dl))
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

                if(MapToListVariablesNodeId == null)
                    MapToListVariablesNodeId = new Dictionary<string,string>();

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
            }
        }
        void CheckUfw(bool bReset = false)
        {
            if (bReset)
            {
                nullconnectionContent.Visibility = Visibility.Collapsed;
                return;
            }

            if (dl != null)
            {
                nullconnectionContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                var be = nullconnectionContent.GetBindingExpression(TextBlock.TextProperty);
                if (be != null)
                    be.UpdateTarget();
                nullconnectionContent.Visibility = Visibility.Visible;
            }
        }

        private void legendListBox_SelectionChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            if (settingStorage.mapSeries == null)
                settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

            foreach (Series serie in diagram.Series.OfType<Series>())
            {
                string tag = serie.Tag as String;
                var name = serie.DisplayName.Replace(String.Format(" - {0}", tag), "");
                if (settingStorage.mapSeries.ContainsKey(name))
                {
                    int thickness = settingStorage.mapSeries[name].thickness;
                    thickness = thickness > 0 ? thickness : 1;
                    if (serie is LineSeries2D)
                    {
                        (serie as LineSeries2D).LineStyle.Thickness = thickness;
                    }
                    else if (serie is AreaSeries2D)
                    {
                        (serie as AreaSeries2D).Border.LineStyle.Thickness = thickness;
                    }

                    string sanitized = GetSanitizedName(name);
                    var saFound = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                    if (saFound != null)
                        saFound.Visible = settingStorage.mapSeries[name].ShowAxis;
                }
            }

            var settings = e.NewRow as SerieSettings;
            if (settings == null)
            {
                txtGridDataTooltip.Visibility = Visibility.Visible;
                tagName.Text = string.Empty;
                gridControl.ItemsSource = null;
                return;
            }

            (from c in diagram.Series.OfType<Series>() where c.DisplayName == settings.Name 
                           || c.DisplayName == ($"{settings.Name} - {minTag}")
                           || c.DisplayName == ($"{settings.Name} - {maxTag}")
                           || c.DisplayName == ($"{settings.Name} - {avgTag}")
                           select c).ToList().ForEach(lsFound =>
                           {
                               string tag = lsFound.Tag as String;
                               var name = lsFound.DisplayName.Replace(String.Format(" - {0}", tag), "");
                               if (settingStorage.mapSeries.ContainsKey(name))
                               {
                                   string sanitized = GetSanitizedName(name);
                                   var saFound = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                                   if (saFound != null)
                                       saFound.Visible = true;
                               }

                               // lsFound.Animate();
                               if (lsFound is LineSeries2D)
                               {
                                   //lsFound.Tag = (lsFound as LineSeries2D).LineStyle;
                                   (lsFound as LineSeries2D).LineStyle.Thickness = 5;
                               }
                               else if (lsFound is AreaSeries2D)
                               {
                                   //lsFound.Tag = (lsFound as AreaSeries2D).Border;
                                   //(lsFound as AreaSeries2D).Border = new SeriesBorder();
                                   (lsFound as AreaSeries2D).Border.LineStyle.Thickness = 5;
                               }
                               if(lsFound.DisplayName == settings.Name || lsFound.Tag as String == avgTag)
                               {
                                   gridControl.ItemsSource = lsFound.DataSource;
                                   tagName.Text = lsFound.DisplayName;
                                   txtGridDataTooltip.Visibility = Visibility.Collapsed;
                               }
                           });
            UpdateAxisRange();
        }

        Dictionary<string, string> cmbTypes = new Dictionary<string, string>() {
            { "lineSeries", Properties.Resources.LineSerie},
            { "lineStepSeries", Properties.Resources.LineStepTitle},
            { "lineStackedSeries", Properties.Resources.LineStackedSerieTitle},
            { "lineFullStackedSeries", Properties.Resources.LineFullStackedTitle},
            { "areaSeries", Properties.Resources.AreaSerieTitle},
            { "areaStepSeries", Properties.Resources.AreaStepSerieTitle},
            { "areaStackedSeries", Properties.Resources.AreaStackedSerieTitle},
            { "areaFullStackedSeries", Properties.Resources.AreaFullStackedSerieTitle},
            { "barSideSeries", Properties.Resources.BarSideSerieTitle}
            };
        String AdaptSerieTypeName(SerieData data)
        {
            var found = (from t in cmbTypes where t.Value.Equals(data.serieTypeLine) select t.Key).FirstOrDefault();
            if (string.IsNullOrEmpty(found))
                return !String.IsNullOrEmpty(data.serieTypeLineKey) ? data.serieTypeLineKey : "lineSeries"; //return "lineSeries";
            else
                return found;
        }

        void RestoreChartFromSettings(bool bForceFetchData = false)
        {
            chart.BeginInit();
            try
            {
                InitServerDocument();

                InitSettingsForSeriesManagement();

                listSeries.Clear();

                if (StaticSeriesSettings != null && StaticSeriesSettings.Count > 0)
                {
                    foreach (var data in StaticSeriesSettings)
                        UpdateSettingSorage(data);
                }

                bool needToFetchData = false;
                ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Clear();
                bool bDiscreteTimeMode = (from string key in settingStorage.mapSeries.Keys where settingStorage.mapSeries[key].serieTypeLine == "barSideSeries" select key).FirstOrDefault() != null;
                if (bDiscreteTimeMode)
                {
                    axisX.DateTimeScaleOptions = new ManualDateTimeScaleOptions() { AggregateFunction = AggregateFunction.None };
                    axisX.SetBinding(ManualDateTimeScaleOptions.AutoGridProperty, new Binding("XAutoGrid") { Source = this });
                }
                foreach (var serie in settingStorage.mapSeries.Keys)
                {
                    listSeries.Add(serie);
                    string _LinkedPenName = serie;
                     _LinkedPenName = TranslationHelpers.TranslationHelper.TranslateComposedText(serie, stringlist, serie);
                    settingStorage.mapSeries[serie].Name = serie;
                    settingStorage.mapSeries[serie].DName = _LinkedPenName;
                    if(settingStorage.mapSeries[serie].UseTableAggregation)
                    {
                        needToFetchData = true;
                        settingStorage.mapSeries[serie].lineSerie = null;
                    }
                    else
                    {
                        var ls = AddChartLine(serie, serie, settingStorage.mapSeries[serie].serieTypeLine, settingStorage.mapSeries[serie].thickness, settingStorage.mapSeries[serie].Color, settingStorage.mapSeries[serie].ShowAxis, settingStorage.mapSeries[serie].LogarithmicYScale, settingStorage.mapSeries[serie].LogarithmicBaseYScale);
                        settingStorage.mapSeries[serie].lineSerie = ls;

                        if (settingStorage.mapSeries[serie].listValues == null || bForceFetchData)
                            needToFetchData = true;
                        else if(!AliasHelper.ContainsAlias(settingStorage.mapSeries[serie].TagName))
                            BuildLineFromValues(serie, settingStorage.mapSeries[serie].listValues);
                    }
                }
                legend_GridControl.ItemsSource = settingStorage.mapSeries.Values.ToList();
                if (needToFetchData)
                    SetDataSources();
                else
                {
                    UpdateAxisRange();
                    toolbarSettings.IsEnabled = true;
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
        }

        void UpdateSettingSorage(SerieData data)
        {
            var name = data.tagName;
            if (!String.IsNullOrEmpty(data.title))
                name = data.title;
            if (String.IsNullOrEmpty(name))
                return;

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
                    serieTypeLine = AdaptSerieTypeName(data),
                    AbsoluteMax = data.max,
                    AbsoluteMin = data.min
                });
            }
            else if (settingStorage.mapSeries.ContainsKey(name))
            {
                settingStorage.mapSeries[name].TagName = data.tagName;
                settingStorage.mapSeries[name].SGuid = data.guiId;
                settingStorage.mapSeries[name].HistoricalName = data.historicalName;
                settingStorage.mapSeries[name].TagreferenceXml = data.tagreferenceXml;
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
        }
        
        private void SetSerieDataSources(string serie, List<Exception> exceptions,string aggTablePostFiss)
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
            var connStr = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);

            if (!settingStorage.mapSeries.ContainsKey(serie))
                settingStorage.mapSeries.Add(serie, new SerieSettings());

            var bUseAggregatedTables = NeedToAggregate(settingStorage.mapSeries[serie]);
            AggregatedValues result = null;
            var minAgg = settingStorage.mapSeries[serie].MinAggregation;
            var maxAgg = settingStorage.mapSeries[serie].MaxAggregation;
            var avgAgg = settingStorage.mapSeries[serie].AvgAggregation;
            if (bUseAggregatedTables && !minAgg && !maxAgg && !avgAgg)
                return;
            var task1 = Task.Factory.StartNew((commandTimeout) =>
            {
                try
                {
                    if (!ct.IsCancellationRequested)
                    {
                        GetEuInformation(settingStorage.mapSeries[serie]);

                        var ret = CreateDataSource(ct, connStr, maxRecords, serie, new TimeSpan(), aggregate, maxaggregationfactor, bUseAggregatedTables, aggTablePostFiss, (int)commandTimeout);
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

            if (!bUseAggregatedTables && timeSpanCompare != null && timeSpanCompare.TotalSeconds > 0)
            {
                AggregatedValues cresult = null;
                var task3 = task1.ContinueWith((ret, commandTimeout) =>
                {
                    try
                    {
                        if (!ct.IsCancellationRequested)
                            cresult = CreateDataSource(ct, connStr, maxRecords, serie, timeSpanCompare, aggregate, maxaggregationfactor, bUseAggregatedTables, aggTablePostFiss, (int)commandTimeout);
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
                        TerminateSetDataSource(exceptions);
                    }
                }, sc);
            }

            var task2 = task1.ContinueWith(ret =>
            {
                if (pendingTask.ContainsKey(serie))
                {
                    if(pendingTask[serie].Contains(task1))
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
                    TerminateSetDataSource(exceptions);
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
                    BuildLineFromValues(serie, result.Values);
                }
                TerminateSetDataSource(exceptions);
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
                            min = (double)item.NodeIdViewModel?.Range?.Low;
                            max = (double)item.NodeIdViewModel?.Range?.High;
                            data.Min = min;
                            data.Max = max;
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
                    if (dlrSettings != null && dlrSettings.ContainsKey(data.HistoricalName) && !string.IsNullOrEmpty(data.TagName))
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
                            min = double.Parse(eu[1]);
                            max = double.Parse(eu[2]);
                            data.Min = min;
                            data.Max = max;
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

        Series AddChartLine(String Original, String Name, String type, int thickness, Color color, bool showaxis, bool logarithmicYScale, double logarithmicbaseYScale, object dataSource = null, bool bCompare = false, bool bAggregated = false, TableAggregation aggregation = TableAggregation.Min)
        {

            var lsFound = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).FirstOrDefault();
            if (lsFound != null)
                return lsFound;

            if (bCompare || bAggregated)
                type = "lineSeries";
            var serie = TryFindResource(type) as Series;
            if (serie == null)
                return null;
            serie.DisplayName = Name;

            if (bCompare)
                serie.Tag = compareTag;
            else if(bAggregated)
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
            serie.ArgumentDataMember = "SourceTimestamp";
            serie.ValueDataMember = "dValue";
            serie.ValueScaleType = ScaleType.Numerical;
            serie.Label.TextPattern = "{V:F3}";
            if (serie is XYSeries2D)
            {
                var pattern = String.Format("{{S}}\n{{V:F2}}\n{{A:{0}}}", GetDateTimeFormat());
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

            //if (showaxis)
            {
                try
                {
                    string sanitized = GetSanitizedName(Original);
                    var saFound = (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() where c.Name == sanitized select c).FirstOrDefault();
                    if (saFound == null)
                    {
                        saFound = TryFindResource("secondaryaxisY") as SecondaryAxisY2D;
                        saFound.Logarithmic = logarithmicYScale;
                        saFound.LogarithmicBase = logarithmicbaseYScale;
                        saFound.Name = sanitized;
                        saFound.Brush = new SolidColorBrush(color);
                        saFound.TickmarksCrossAxis = true;

                        ((XYDiagram2D)chart.Diagram).SecondaryAxesY.Add(saFound);
                    }

                    if (saFound != null)
                    {
                        saFound.Brush = new SolidColorBrush(color);
                        XYDiagram2D.SetSeriesAxisY((XYSeries)serie, saFound);
                        saFound.Visible = showaxis;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            diagram.Series.Add(serie);

            CheckAndAdaptAxisRange();

            if (thickness > 0)
            {
                if (serie is LineSeries2D)
                {
                    (serie as LineSeries2D).LineStyle = new LineStyle(thickness);
                }
                else if (serie is AreaSeries2D)
                {
                    (serie as AreaSeries2D).Border = new SeriesBorder();
                    (serie as AreaSeries2D).Border.Brush = new SolidColorBrush(color);
                    (serie as AreaSeries2D).Border.LineStyle = new LineStyle(thickness);
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

        readonly String compareTag = Properties.Settings.Default.CompareID;
        readonly String minTag = Properties.Settings.Default.MinID;
        readonly String maxTag = Properties.Settings.Default.MaxID;
        readonly String avgTag = Properties.Settings.Default.AvgID;

        void ClearAllCompareSeries()
        {
            var ls = (from c in diagram.Series.OfType<Series>() where c.Tag as String == compareTag select c).ToList();
            chart.BeginInit();
            ls.ForEach(serie => diagram.Series.Remove(serie));
            chart.EndInit();
        }

        void SetChartLineDataSource(String Name, object dataSource, bool bCompare = false, bool bAggregated = false, TableAggregation aggregation = TableAggregation.Min)
        {
            var original = Name;
            if (bCompare)
                Name = String.Format("{0} - {1}", Name, compareTag);
            else if(bAggregated)
            {
                var loriginal = (from c in diagram.Series.OfType<Series>() where c.DisplayName == Name select c).ToList();
                if (loriginal.Count > 0)
                    loriginal.ForEach(s => diagram.Series.Remove(s));
                Name = String.Format("{0} - {1}", Name, aggregation);
            }
            else
            {
                var laggregated = (from c in diagram.Series.OfType<Series>() where 
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
                var serie = AddChartLine(original, Name, settingStorage.mapSeries[original].serieTypeLine,
                    settingStorage.mapSeries[original].thickness, settingStorage.mapSeries[original].Color, settingStorage.mapSeries[original].ShowAxis, settingStorage.mapSeries[original].LogarithmicYScale, settingStorage.mapSeries[original].LogarithmicBaseYScale, dataSource, bCompare, bAggregated, aggregation);
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
                ls[0].DataSource = null;
                ls[0].DataSource = dataSource;
                ls[0].Visible = settingStorage.mapSeries[original].IsVisible;
                if (ls[0].Points != null)
                {
                    if (ls[0].Points.Count > MaxResolveOverlappingPoints)
                        (ls[0] as Series).Label.ResolveOverlappingMode = ResolveOverlappingMode.None;
                    (ls[0] as Series).LabelsVisibility = btnShowLabels.IsChecked == true && ls[0].Points.Count <= MaxLabelPoints;
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

        AggregatedValues CreateDataSource(CancellationToken ct, String connStr, int maxRecord, String serie, TimeSpan diff, bool aggregate, int maxaggregationfactor,bool useAggregatedTables,string aggTablePostFiss, int commandTimeout)
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

            if (settingStorage.DateTimeStart == settingStorage.DateTimeEnd)
            {
                settingStorage.DateTimeStart = minDateTimeValue;
                settingStorage.DateTimeEnd = maxDateTimeValue;
            }

            if (!dlrsource)
            {
                #region Historicals
                string hstname = historicalname;
                string realTagname = name.Replace("//", "/");
                realTagname = realTagname.Replace("/", ".");
                if (realTagname.StartsWith("."))
                    realTagname = realTagname.Substring(1);
                if(!realTagname.StartsWith("Tags."))
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
                                                               && !ct.IsCancellationRequested
                                                              select entry).FirstOrDefault();

                        if (_uFUAAuditDataLog == null)
                            return ret;

                        if (!string.IsNullOrEmpty(seriesettings.NodeID) && _uFUAAuditDataLog != null)
                            bUseOid = true;
                        bool useDateTime = !(settingStorage.DateTimeStart == minDateTimeValue && settingStorage.DateTimeEnd == maxDateTimeValue);
                        var utcStart = settingStorage.DateTimeStart;
                        var utcEnd = settingStorage.DateTimeEnd;
                        if (useDateTime)
                        {
                            if (settingStorage.DateTimeStart != DateTime.MinValue)
                                utcStart = settingStorage.DateTimeStart.ToUniversalTime() - diff;
                            if (settingStorage.DateTimeEnd != DateTime.MinValue)
                                utcEnd = settingStorage.DateTimeEnd.ToUniversalTime() - diff;
                        }
                        if (!usesourcetimestamp)
                        {
                            ret.Values = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                          where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                                (!useDateTime || (useDateTime && (entry.RecordDateTimeUtc >= utcStart) && (entry.RecordDateTimeUtc <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                && !ct.IsCancellationRequested
                                          orderby entry.RecordDateTimeUtc descending
                                          select new MyDataValue(
                                              entry.RecordDateTimeUtc.ToLocalTime(),
                                              entry.dValue
                                          )
                                        ).Take(maxrecord).ToList();
                        }
                        else
                        {
                            ret.Values = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                          where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                (!useDateTime || (useDateTime && (entry.SourceTimeStamp >= utcStart) && (entry.SourceTimeStamp <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                && !ct.IsCancellationRequested
                                          orderby entry.SourceTimeStamp descending
                                          select new MyDataValue(
                                              localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                              entry.dValue
                                            )
                                          ).Take(maxrecord).ToList();
                        }

                        if (diff.Ticks > 0)
                            Parallel.ForEach(ret.Values, (entry, loopState) =>
                            {
                                if (ct.IsCancellationRequested)
                                    loopState.Break();
                                entry.SourceTimestamp = entry.SourceTimestamp + diff;
                            });

                        if (ct.IsCancellationRequested)
                            return null;

                        settingStorage.mapSeries[serie].numPoints = ret.Values.Count;
                        settingStorage.mapSeries[serie].numCompressRation = 1;
                        if (ret.Values.Count > maxRecord)
                        {
                            int div = ret.Values.Count / maxRecord;
                            AggregatedValues retaggregated = new AggregatedValues();
                            retaggregated.Values = Aggregate(ret.Values, div, maxRecord, ct);
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
                    if(useAggregatedTables)
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

                        if (dbSchemaInfo.IsSupportedTopKeyword)
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
                        if(useAggregatedTables)
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


                        if (!(settingStorage.DateTimeStart == minDateTimeValue && settingStorage.DateTimeEnd == maxDateTimeValue))
                        {
                            var utcStart = settingStorage.DateTimeStart;
                            var utcEnd = settingStorage.DateTimeEnd;
                            if (settingStorage.DateTimeStart != DateTime.MinValue)
                                utcStart = settingStorage.DateTimeStart.ToUniversalTime() - diff;
                            if (settingStorage.DateTimeEnd != DateTime.MinValue)
                                utcEnd = settingStorage.DateTimeEnd.ToUniversalTime() - diff;

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
                        dbdapater.Fill(gridDataSet, _tablename);
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
                                var sourcetimestamp = (usesourcetimestamp && localize) || !usesourcetimestamp ? date.ToLocalTime() + diff : date + diff;
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
                            if (dataView.Count > maxRecord)
                            {
                                int div = dataView.Count / maxRecord;
                                AggregatedValues retaggregated = new AggregatedValues();

                                if (useAggregatedTables)
                                {
                                    if (ret.MinValues.Count > 0)
                                        retaggregated.MinValues = Aggregate(ret.MinValues, div, maxRecord, ct);
                                    if (ret.MaxValues.Count > 0)
                                        retaggregated.MaxValues = Aggregate(ret.MaxValues, div, maxRecord, ct);
                                    if (ret.AvgValues.Count > 0)
                                        retaggregated.AvgValues = Aggregate(ret.AvgValues, div, maxRecord, ct);
                                    settingStorage.mapSeries[serie].numCompressPoint = Math.Max(Math.Max(ret.MaxValues.Count, ret.MinValues.Count), ret.AvgValues.Count);
                                }
                                else
                                {
                                    if (ret.Values.Count > 0)
                                        retaggregated.Values = Aggregate(ret.Values, div, maxRecord, ct);
                                    settingStorage.mapSeries[serie].numCompressPoint = ret.Values.Count;
                                }

                                settingStorage.mapSeries[serie].numCompressRation = div;

                                return retaggregated;
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

        List<MyDataValue> Aggregate(List<MyDataValue> values, int div, int maxRecord,CancellationToken ct)
        {
            var aggregated = new List<MyDataValue>();
            for (int i = 0; i < maxRecord; ++i)
            {
                if (ct.IsCancellationRequested)
                    return null;

                var list = values.Skip(i * div).Take(div).ToList();
                var average = list.Aggregate((acc, cur) => acc + cur) / list.Count;
                var timespan = list[list.Count - 1].SourceTimestamp - list[0].SourceTimestamp;
                var timespanaverage = new TimeSpan(0, 0, (int)timespan.TotalSeconds / 2);
                var time = list[0].SourceTimestamp + timespanaverage;
                aggregated.Add(new MyDataValue() { SourceTimestamp = time, dValue = average.dValue, bCompressed = true });
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

                lock(dlrSettings)
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
            if(!isInZoomMode)
            {
                axisY.ActualWholeRange.SetAuto();
                axisY.ActualVisualRange.SetAuto();
                if (!AutomaticScale || bDesignmode)
                {
                    axisY.ActualWholeRange.MinValue = Minimum;
                    axisY.ActualWholeRange.MaxValue = Maximum;
                    axisY.ActualVisualRange.MinValue = Minimum;
                    axisY.ActualVisualRange.MaxValue = Maximum;
                }

                (from c in ((XYDiagram2D)chart.Diagram).SecondaryAxesY.OfType<SecondaryAxisY2D>() select c).ToList().ForEach(x =>
                {
                    x.ActualWholeRange.SetAuto();
                    x.ActualVisualRange.SetAuto();
                    if(!AutomaticScale)
                    {
                        var serie = (from s in settingStorage.mapSeries.Values where !s.AuthomaticScale && x.Name == GetSanitizedName(s.Name) select s).FirstOrDefault();
                        if (serie != null)
                        {
                            if(settingStorage.mapSeries.ContainsKey(serie.Name))
                            {
                                x.ActualWholeRange.MinValue = settingStorage.mapSeries[serie.Name].AbsoluteMin;
                                x.ActualWholeRange.MaxValue = settingStorage.mapSeries[serie.Name].AbsoluteMax;
                            }
                            else
                            {
                                x.ActualWholeRange.MinValue = serie.Min;
                                x.ActualWholeRange.MaxValue = serie.Max;
                            }
                        }
                    }
                });
            }
            isInZoomMode = false;

            SetTimeScaleAlignment(settingStorage.DateTimeStart, settingStorage.DateTimeEnd);

            UpdateXAxisSpacing();
            UpdateYAxisSpacing();
        }

        void UpdateXAxisSpacing()
        {
            double gridSpacing = 0.0;
            if (axisX.ActualWholeRange.ActualMaxValue is DateTime && axisX.ActualWholeRange.ActualMinValue is DateTime)
            {
                var delta = (DateTime)axisX.ActualWholeRange.ActualMaxValue - (DateTime)axisX.ActualWholeRange.ActualMinValue;
                double division = Math.Max(1.0, XMajorCount);

                DateTimeGridAlignment dtga;
                if (axisX.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                    dtga = (axisX.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridAlignment;
                else
                    dtga = (axisX.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridAlignment;

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

            if (axisX.DateTimeScaleOptions is ContinuousDateTimeScaleOptions)
                (axisX.DateTimeScaleOptions as ContinuousDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
            else
                (axisX.DateTimeScaleOptions as ManualDateTimeScaleOptions).GridSpacing = Math.Ceiling(gridSpacing);
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
            string pattern = "[ \\[\\]\\+°\\~#%&@$£'.!*{})(/:<>?|\"-]";
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
                if (FilterType == DateSpan.All || FilterType == DateSpan.None)
                {
                    if (list.Last().SourceTimestamp > settingStorage.DateTimeStart)
                    {
                        settingStorage.DateTimeStart = settingStorage.StartTime = list.Count == 1 ? list.Last().SourceTimestamp.AddMonths(-6) : list.Last().SourceTimestamp;
                    }
                    if (list.First().SourceTimestamp < settingStorage.DateTimeEnd)
                    {
                        settingStorage.DateTimeEnd = settingStorage.EndTime = list.Count == 1 ? list.First().SourceTimestamp.AddMonths(6) : list.First().SourceTimestamp;
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
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second, 0);
            }
            else if (datetimeDiff.TotalSeconds <= secondsPerMinute)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Second, DateTimeMeasureUnit.Millisecond);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second,0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second,0);
            }
            else if (datetimeDiff.TotalMinutes <= minutesPerHour)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Second, DateTimeMeasureUnit.Second);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, end.Second);
            }
            else if (datetimeDiff.TotalHours <= hoursPerDay)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Minute, DateTimeMeasureUnit.Minute);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, end.Minute, 59);
            }
            else if (datetimeDiff.TotalDays <= daysPerWeek)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, 59, 59);
            }
            else if (datetimeDiff.TotalDays <= daysPerMonth)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Hour, DateTimeMeasureUnit.Hour);
                minValue = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, end.Hour, 59, 59);
            }
            else if (datetimeDiff.TotalDays <= daysPerYears * 10)
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Day, DateTimeMeasureUnit.Day);
                minValue = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else
            {
                SetDateTimeScaleOptions(DateTimeGridAlignment.Year, DateTimeMeasureUnit.Week);
                minValue = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                maxValue = new DateTime(end.Year, end.Month, 1, 0, 0, 0);
            }

            if (FilterType == DateSpan.All)
            {
                try
                {
                    axisX.ActualWholeRange.MinValue = new DateTime(minValue.Ticks - dateMargin);
                    axisX.ActualWholeRange.MaxValue = new DateTime(maxValue.Ticks + dateMargin);
                }
                catch (ArgumentOutOfRangeException)
                {
                    axisX.ActualWholeRange.MinValue = minValue;
                    axisX.ActualWholeRange.MaxValue = maxValue;
                }
            }
            else
            {
                axisX.ActualWholeRange.MaxValue = minValue;
                axisX.ActualWholeRange.MaxValue = maxValue;
            }
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
                (dtso as ManualDateTimeScaleOptions).GridAlignment = ga;
                (dtso as ManualDateTimeScaleOptions).MeasureUnit = mu;
            }
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
        void SetDataSources()
        {
            if (string.IsNullOrEmpty(ConnectionString))
                return;

            ConstantLines.Clear();
            //diagram.Series.Clear();

            if (listSeries.Count == 0)
            {
                if(!bControlLoaded)
                {
                    bControlLoaded = true;
                    OnControlLoaded();
                }
                return;
            }
            CheckUfw(true);
            SetBusy(true);
            {
                if (settingStorage.mapSeries == null)
                    settingStorage.mapSeries = new Dictionary<String, SerieSettings>();

                settingStorage.StartTime = settingStorage.DateTimeStart;// DateTime.MinValue;
                settingStorage.EndTime = settingStorage.DateTimeEnd;// DateTime.MaxValue;
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
                    SetSerieDataSources(serie, exceptions, aggTablePostFiss);
                });
            }
        }
        void TerminateSetDataSource(List<Exception> exceptions)
        {
            if (pendingTask.Count == 0)
            {
                UpdateAxisRange();
                diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
                toolbarSettings.IsEnabled = true;
                legend_GridControl.ItemsSource = null;
                legend_GridControl.ItemsSource = settingStorage.mapSeries.Values.ToList();
                SetBusy(false);
                CheckUfw();

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
            }
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
        private void ShowError(Exception exception)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.InnerException != null ? exception.InnerException.Message : exception.Message);
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
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SetTimeRange();
            SetDataSources();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            var dates = GetSelectedTimeRangeCombo(true);
            if (dates.start == settingStorage.DateTimeStart && dates.end == settingStorage.DateTimeEnd)
                return;

            settingStorage.DateTimeEnd = dates.end;
            settingStorage.DateTimeStart = dates.start;
            //SetTimeScaleRange(settingStorage.DateTimeStart, settingStorage.DateTimeEnd);
            SetDataSources();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var dates = GetSelectedTimeRangeCombo(false);
            if (dates.start == settingStorage.DateTimeStart && dates.end == settingStorage.DateTimeEnd)
                return;

            settingStorage.DateTimeEnd = dates.end;
            settingStorage.DateTimeStart = dates.start;
            //SetTimeScaleRange(settingStorage.DateTimeStart, settingStorage.DateTimeEnd);
            SetDataSources();
        }

        void CheckEnabledButtons()
        {
            var bCanMove = FilterType != DateSpan.All;
            btnPrev.IsEnabled = bCanMove;
            btnRefresh.IsEnabled = bCanMove;
            btnNext.IsEnabled = bCanMove;
            cmbTimeRange.IsEnabled = bCanMove && !RunningOnServer;
            cmbCompare.IsEnabled = bCanMove && !RunningOnServer;
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
            var item = (from c in (cmbTimeRange.EditSettings as ComboBoxEditSettings).Items.OfType<ComboBoxEditItem>() where (DateSpan)c.Tag == tag select c).ToList();
            if (item.Count == 0)
                return;
            item[0].IsSelected = true;
        }

        TimeRangeDates GetSelectedTimeRangeCombo(bool prev)
        {
            TimeRangeDates dates;
            dates.start = settingStorage.DateTimeStart;
            dates.end = settingStorage.DateTimeEnd;

            var editsettings = cmbTimeRange.EditSettings as ComboBoxEditSettings;
            var selectedItem = (from ComboBoxEditItem item in editsettings.Items where item.Content == cmbTimeRange.EditValue select item).FirstOrDefault();

            if (selectedItem != null && selectedItem.Tag is DateSpan)
                TimeRangeHelper.GetSelectedTimeRangeCombo(settingStorage.DateTimeStart, settingStorage.DateTimeEnd, (DateSpan)selectedItem.Tag, UseAbsoluteRanges, prev, out dates);
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
            chart.CrosshairEnabled = !chart.CrosshairEnabled;
        }

        void chart_BoundDataChanged(object sender, RoutedEventArgs e)
        {
            if (!bInit || bDesignmode || bDispose)
                return;

            ConstantLines.Clear();
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
                    if (!settingStorage.mapSeries[serie.DisplayName].IsStatisticEnabled)
                        continue;

                    if (settingStorage.mapSeries[serie.DisplayName].listValues == null ||
                        settingStorage.mapSeries[serie.DisplayName].listValues.Count == 0)
                        continue;

                    minPrice = settingStorage.mapSeries[serie.DisplayName].minValue;
                    maxPrice = settingStorage.mapSeries[serie.DisplayName].maxValue;
                    averagePrice = settingStorage.mapSeries[serie.DisplayName].averageValue;
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
                ConstantLines.AddRange(new ConstantLine[] { minConstantLine, maxConstantLine, averageConstantLine });
                if (minPrice < minY)
                    minY = minPrice;
                if (maxPrice > maxY)
                    maxY = maxPrice;
            }

            foreach (ConstantLine constantLine in ConstantLines)
                constantLine.Title.Alignment = ConstantLineTitleAlignment.Far;

            //chart.Animate();
        }

        private void chart_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {
            foreach (var group in e.CrosshairElementGroups)
            {
                var serie = group.CrosshairElements.First().Series.DisplayName;
                var point = group.CrosshairElements.First().SeriesPoint;
                nearestPoints[serie] = point;
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

        private void chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement)
                UpdateFocus(e.Source as FrameworkElement);

            var hitInfo = chart.CalcHitInfo(e.GetPosition(chart));
            if (hitInfo != null && hitInfo.InDiagram)
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
                        s.LastClickedPoint.Date = nearestPoints[serie.DisplayName].Argument;
                        s.LastClickedPoint.Value = nearestPoints[serie.DisplayName].Value;
                    }
                }
            }
            if (hitInfo != null && hitInfo.Series != null)
            {
                string displayName = hitInfo.Series.DisplayName;
                if (settingStorage.mapSeries != null && settingStorage.mapSeries.ContainsKey(displayName))
                {
                    tagName.Text = hitInfo.Series.DisplayName;
                    SerieSettings selectedSerie = settingStorage.mapSeries[displayName];
                    legend_GridControl.SelectedItem = selectedSerie;
                }
                else
                {
                    string tag = hitInfo.Series.Tag as String;
                    var name = displayName.Replace(String.Format(" - {0}", tag), "");
                    if (settingStorage.mapSeries.ContainsKey(name))
                    {
                        gridControl.ItemsSource = hitInfo.Series.DataSource;
                        tagName.Text = hitInfo.Series.DisplayName;
                        txtGridDataTooltip.Visibility = Visibility.Collapsed;
                    }
                }
                e.Handled = true;
            }
        }

        private void Button_FetchData(object sender, RoutedEventArgs e)
        {
            settingStorage.ListRanges.Insert(0, new TimeRange()
            {
                DateTimeEnd = settingStorage.DateTimeEnd,
                DateTimeStart = settingStorage.DateTimeStart
            });
            while (settingStorage.ListRanges.Count > 10)
                settingStorage.ListRanges.Remove(settingStorage.ListRanges.Last());

            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settingStorage.ListRanges;

            if (FilterType != DateSpan.None)
                FilterType = DateSpan.None;
            else
                SetDataSources();
        }
        private void Button_ClearRecent(object sender, RoutedEventArgs e)
        {
            settingStorage.ListRanges.Clear();
            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settingStorage.ListRanges;
        }

        private void listBoxRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var timeRange = listBoxRecent.SelectedItem as TimeRange;
            if (timeRange == null)
                return;
            
            settingStorage.DateTimeEnd = timeRange.DateTimeEnd;
            settingStorage.DateTimeStart = timeRange.DateTimeStart;
            if (FilterType != DateSpan.None)
                FilterType = DateSpan.None;
            else
                SetDataSources();
        }

        private void FromToday_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            settingStorage.DateTimeStart = today;
        }
        private void TillTomorrow_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) + TimeSpan.FromDays(1);
            settingStorage.DateTimeEnd = today;
        }
        private void ChartDesigner_Click(object sender, RoutedEventArgs e)
        {
            var designer = new ChartDesigner(chart);
            designer.Show(this.FindParent<Window>());
        }

        private void cmbCompare_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!bLoaded)
                return;

            TimeSpan diff = TimeSpan.Zero;
            var editsettings = ((BarEditItem)e.OriginalSource).EditSettings as ComboBoxEditSettings;
            var selectedItem = (from ComboBoxEditItem item in editsettings.Items where item.Content == cmbCompare.EditValue select item).FirstOrDefault();

            if (selectedItem != null && selectedItem.Tag is DateSpan)
            {
                var dateSpan = (DateSpan)selectedItem.Tag;
                if (dateSpan == DateSpan.Minute)
                    diff = TimeSpan.FromMinutes(1);
                else if (dateSpan == DateSpan.Hour)
                    diff = TimeSpan.FromHours(1);
                else if (dateSpan == DateSpan.Day)
                    diff = TimeSpan.FromDays(1);
                else if (dateSpan == DateSpan.Week)
                    diff = TimeSpan.FromDays(daysPerWeek);
                else if (dateSpan == DateSpan.Month)
                    diff = TimeSpan.FromDays(daysPerMonth);
                else if (dateSpan == DateSpan.Year)
                    diff = TimeSpan.FromDays(daysPerYears);
            }

            timeSpanCompare = diff;
            if (timeSpanCompare.TotalSeconds > 0)
                SetDataSources();
            else
                ClearAllCompareSeries();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (RunningOnServer)
                return;

            UtilitiesPrintHelper.PrintElement(chart as FrameworkElement, this.FindParent<Window>(),true, true, System.Drawing.Printing.PaperKind.A4);
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
                    (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;
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

        private void Restart(SerieDataList penlist)
        {
            toolbarSettings.IsEnabled = false;
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
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            if (Document != null && Document.Parent != null)
            {

                if (UFUAEditor == null)
                    UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

            }

            if (StaticSeriesSettings == null)
                return ret;

            foreach (var pen in StaticSeriesSettings)
            {
                if (string.IsNullOrEmpty(pen.guiId))
                    pen.guiId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(pen.tagreferenceXml))
                    ret.Add(CreateUniqueName(pen.title, ret.Keys.ToList()), pen.tagreferenceXml);
            }
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
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute) || StaticSeriesSettings == null)
                return false;
            bool ret = false;

            OPCUAEntityReference _relative = relative.FromXml<OPCUAEntityReference>();
            OPCUAEntityReference _absolute = absolute.FromXml<OPCUAEntityReference>();
            SerieDataList penList = new SerieDataList();
            var opcInit = OpcuaEntityReference;
            penList.AddRange(StaticSeriesSettings.ToList());
            var penTagList = (from pen in penList where pen.tagReference != null select pen).ToList();

            foreach (var pen in penTagList)
            {
                //if (pen.tagReference != null /*&& pen.tagReference.IsValid*/)
                {
                    try
                    {
                        if (bDesignmode)
                            ret = relative == pen.tagreferenceXml;
                        else if (_absolute != null )
                        {
                            if (relative == pen.tagreferenceXml)
                            {
                                string key = pen.guiId;
                                matchChangedMap.Add(key);
                                TerminateExecution(key);
                                if(_absolute.MatchTypeDefintion(_relative) && _relative.IsRelative)
                                {
                                    _relative.Merge(_absolute);
                                    pen.tagReference = _relative;
                                    opcuaEntityReference[key] = _relative;
                                }
                                else
                                {
                                    pen.tagReference = _absolute;
                                    opcuaEntityReference[key] = _absolute;
                                }

                                if (pen.tagReference.HasValidValue)
                                {
                                    var tagPath = $"{pen.tagReference.StartingAddress}/{pen.tagReference.RelativePath}";
                                    if (tagPath.StartsWith("/") && tagPath.Length > 1)
                                        tagPath = tagPath.Substring(1);
                                    if (!pen.dlrsource)
                                        (pen.tagName) = GetRelativePath(tagPath);
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(pen.historicalName))
                                        {
                                            UpdateDlrSettings(pen.historicalName);
                                            if (!dlrSettings.ContainsKey(pen.historicalName))
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

                                    pen.nodeID = pen.tagReference.NodeIdViewModel?.nodeId.ToString();
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
                ret = matchChangedMap.Count == penTagList.Count;

            if (!bDesignmode && ret)
                StaticSeriesSettings = penList;
            return ret;
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
                        mapHandlers = new Dictionary<string, SeriesDataHelpers>();
                    if (mapHandlers.ContainsKey(key))
                    {
                        typeHelper.TerminateExecution(this, mapHandlers[key].opcuaEntityReference_PropertyChanged, mapHandlers[key].monitoredItemViewModel_PropertyChanged, OpcuaEntityReference[key], OpcuaEntityReference[key].MonitoredItemViewModel);
                        mapHandlers[key].Dispose();
                        mapHandlers.Remove(key);
                    }
                }
            }
        }
        Dictionary<string, SeriesDataHelpers> mapHandlers;
        private void PrepareExecution(string key)
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

                    mapHandlers[key] = new SeriesDataHelpers()
                    {
                        control = this,
                        Key = key
                    };

                    typeHelper.PrepareExecution(Properties.Resources.SessionName, Document as ScreenDocument, this, mapHandlers[key].opcuaEntityReference_PropertyChanged, OpcuaEntityReference[key]);
                }
                catch (Exception)
                {
                }
            }
        }
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            SerieDataList penList = new SerieDataList();
            if(StaticSeriesSettings != null)
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
                        (pen.tagName) = GetRelativePath($"{pen.tagReference.RelativePath}");
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
            }
            StaticSeriesSettings = penList;
        }
        internal void UpdateReferences(string key, string noideid)
        {
            var serie = (from s in StaticSeriesSettings where s.guiId == key select s).FirstOrDefault();
            if(serie != null)
            {
                serie.nodeID = noideid;
                InitSettingsForSeriesManagement();
                UpdateSettingSorage(serie);
                if(sc == null)
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
                string aggTablePostFiss = GetTablePostFiss();
                SetSerieDataSources(name, exceptions, aggTablePostFiss);
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
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
        List<DevExpress.Xpf.Charts.Range> zommingRangeList;
        private void diagram_Zoom(object sender, XYDiagram2DZoomEventArgs e)
        {
            if (!(e.OldXRange.MinValue is DateTime) || !(e.OldXRange.MaxValue is DateTime) ||
                !(e.NewXRange.MinValue is DateTime) || !(e.NewXRange.MaxValue is DateTime))
                return;

            if (zommingRangeList == null)
                zommingRangeList = new List<DevExpress.Xpf.Charts.Range>();

            if (isZoomingOnMouseWheel)
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
        private void ManageZoom(DateTime oldXRangeMinValue, DateTime oldXRangeMaxValue, DateTime newXRangeMinValue, DateTime newXRangeMaxValue,bool bForceUpdating = false)
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
                isZoomingOnMouseWheel = true;
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
                if(item != null)
                {
                    configMemory.EditValue = item.Name;
                    return true; 
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        private void configMemory_SelectionChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ChangeSetting(((DevExpress.Xpf.Bars.BarEditItem)e.OriginalSource).EditValue as String);
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
                Restart(StaticSeriesSettings);
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
                Restart(StaticSeriesSettings);
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
                (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;
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
                (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;
                configMemory.EditValue = selected.Name;
            }

            if (StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null))
            {
                if(oldMemoryList != null)
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
            (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;

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
            (configMemory.EditSettings as ComboBoxEditSettings).ItemsSource = MemorySettingList?.Names;
            bCallingRemoveCommand = false;
            configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;
        }

        internal bool IsEnableCommand
        {
            get
            {
                if(bDesignmode || DesignerProperties.GetIsInDesignMode(this))
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

            if (helper!= null && helper.ValidateAccessLevel())
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
            if(e.Source is FrameworkElement && (e.Key == Key.LeftShift || e.Key == Key.RightShift))
                UpdateFocus(e.Source as FrameworkElement);
        }
        private void UpdateFocus(FrameworkElement sender)
        {
            if (!IsElementContained(sender))
                return;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if(!bDispose)
                {
                    chart.Focus();
                    diagram.EnableAxisYNavigation = !DisableZoomBehaviour;
                }
            });
        }
        bool IsElementContained(object sender)
        {
            return (from c in chart.GetVisualChildrenOfType<FrameworkElement>()
                    where c == sender
                    select c).FirstOrDefault() != null;
        }
        private void diagram_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!IsElementContained(sender))
                return;
            diagram.EnableAxisYNavigation = false;
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
                map.Add(CreateUniqueName(x.title, map.Keys.ToList()), x.title);
                i++;
            });

            return map;
        }
        #endregion
    }
}
