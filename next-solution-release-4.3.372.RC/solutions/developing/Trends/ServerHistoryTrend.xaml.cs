using Converters;
using OPCUAViewModel;
using Opc.Ua;
using ViewModelLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.IO;
using System.Reflection;
using System.Windows.Media.Animation;
using System.Data;
using System.Xml;
using System.Runtime.Serialization;

using Utilities;
using WPFUtilities;
using WPFUtilities.Extensions;
using UFInterfaces.PropertyControl;
using ScreenSettings;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using UFInterfaces;
using TranslationHelpers;
using DevExpress.Xpf.Grid;
using log4net;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using System.Globalization;
using System.Windows.Threading;
using System.Xml.Serialization;
using UFProjectManager.ComponentService;
using static Trends.TimeFilterViewModel;

namespace Trends
{
    public enum AggregationTypeDefinition
    {
        Interpolative,
        TimeAverage,
        Average,
        Count,
        Maximum,
        Minimum,
    }

    public enum ReadTypeDefinition
    {
        Raw,
        Modified,
        AtTime,
        Processed
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public partial class ServerHistoryTrend : UserControl, IContainPropertyEditors, IDisposable, IEntityReference, ISettingsHelper, INotifyPropertyVisibilityChanged
        , IStringIDAware
    {
        #region DependencyProperties
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ServerHistoryTrend));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
            //OnForegroundChanged();
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ServerHistoryTrend));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bOverride)
            {
                var title = TitleFonstSettings.Clone();
                var yaxsis = YAxsisFontSettings.Clone();
                var xaxsis = XAxsisFontSettings.Clone();
                var control = ControlFontSettings.Clone();


                title.FontFamily = FontFamily;
                yaxsis.FontFamily = FontFamily;
                xaxsis.FontFamily = FontFamily;
                control.FontFamily = FontFamily;

                TitleFonstSettings = title;
                YAxsisFontSettings = yaxsis;
                XAxsisFontSettings = xaxsis;
                ControlFontSettings = control;

                title1.FontFamily = FontFamily;
                axisY.FontFamily = FontFamily;
                axisX.FontFamily = FontFamily;


            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bOverride)
            {
                var title = TitleFonstSettings.Clone();
                var yaxsis = YAxsisFontSettings.Clone();
                var xaxsis = XAxsisFontSettings.Clone();
                var control = ControlFontSettings.Clone();


                title.FontWeight = FontWeight;
                yaxsis.FontWeight = FontWeight;
                xaxsis.FontWeight = FontWeight;
                control.FontWeight = FontWeight;

                TitleFonstSettings = title;
                YAxsisFontSettings = yaxsis;
                XAxsisFontSettings = xaxsis;
                ControlFontSettings = control;

                title1.FontWeight = FontWeight;
                axisY.FontWeight = FontWeight;
                axisX.FontWeight = FontWeight;


            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bOverride)
            {
                var title = TitleFonstSettings.Clone();
                var yaxsis = YAxsisFontSettings.Clone();
                var xaxsis = XAxsisFontSettings.Clone();
                var control = ControlFontSettings.Clone();


                title.FontStyle = FontStyle;
                yaxsis.FontStyle = FontStyle;
                xaxsis.FontStyle = FontStyle;
                control.FontStyle = FontStyle;

                TitleFonstSettings = title;
                YAxsisFontSettings = yaxsis;
                XAxsisFontSettings = xaxsis;
                ControlFontSettings = control;

                title1.FontStyle = FontStyle;
                axisY.FontStyle = FontStyle;
                axisX.FontStyle = FontStyle;


            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bOverride)
            {
                var title = TitleFonstSettings.Clone();
                var yaxsis = YAxsisFontSettings.Clone();
                var xaxsis = XAxsisFontSettings.Clone();
                var control = ControlFontSettings.Clone();


                title.FontSize = (int)FontSize;
                yaxsis.FontSize = (int)FontSize;
                xaxsis.FontSize = (int)FontSize;
                control.FontSize = (int)FontSize;

                TitleFonstSettings = title;
                YAxsisFontSettings = yaxsis;
                XAxsisFontSettings = xaxsis;
                ControlFontSettings = control;

                title1.FontSize = FontSize;
                axisY.FontSize = FontSize;
                axisX.FontSize = FontSize;


            }
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
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
                //TitleForeground = Foreground;
                //ControlForeground = Foreground;
                TitleForeground = Foreground;
                UpdateControlLayout();
            }
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as ServerHistoryTrend;
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
                PlotBackground = Background;
            }
        }
        #endregion

        #region ViewMode
        public static readonly DependencyProperty ControlViewModeProperty = DependencyProperty.Register("ControlViewMode", typeof(ViewMode), typeof(ServerHistoryTrend), new UIPropertyMetadata(ViewMode.Chart, new PropertyChangedCallback(OnControlViewModeChanged), new CoerceValueCallback(OnCoerceControlViewMode)));

        private static object OnCoerceControlViewMode(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceControlViewMode((ViewMode)value);
            else
                return value;
        }

        private static void OnControlViewModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnControlViewModeChanged((ViewMode)e.OldValue, (ViewMode)e.NewValue);
        }

        protected virtual ViewMode OnCoerceControlViewMode(ViewMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlViewModeChanged(ViewMode oldValue, ViewMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            chart.Visibility = newValue == ViewMode.Chart ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            listView1.Visibility = newValue == ViewMode.Chart ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            bestFit.IsVisible = newValue != ViewMode.Chart;
        }
        [Category("TrendOptions")]
        public ViewMode ControlViewMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ViewMode)GetValue(ControlViewModeProperty);
            }
            set
            {
                SetValue(ControlViewModeProperty, value);
            }
        }

        #endregion

        #region UseTouchKeyboard
        public static readonly DependencyProperty UseTouchKeyboardProperty = DependencyProperty.Register("UseTouchKeyboard", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseTouchKeyboardChanged), new CoerceValueCallback(OnCoerceUseTouchKeyboard)));

        private static object OnCoerceUseTouchKeyboard(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceUseTouchKeyboard((bool)value);
            else
                return value;
        }

        private static void OnUseTouchKeyboardChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnUseTouchKeyboardChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseTouchKeyboard(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseTouchKeyboardChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("TrendOptions")]
        public bool UseTouchKeyboard
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseTouchKeyboardProperty);
            }
            set
            {
                SetValue(UseTouchKeyboardProperty, value);
            }
        }

        #endregion

        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));


        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            ServerHistoryTrend gridAlarmWindow = o as ServerHistoryTrend;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend gridAlarmWindow = o as ServerHistoryTrend;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnUseIconChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("TrendOptions")]
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

        #region ShowLabels
        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowLabelsChanged), new CoerceValueCallback(OnCoerceShowLabels)));

        private static object OnCoerceShowLabels(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceShowLabels((bool)value);
            else
                return value;
        }

        private static void OnShowLabelsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
            if (bDesignmode)
            {
                btnShowLabels.IsEnabled = false;
                ChartShowLabels = newValue;
            }
            else
            {
                ChartShowLabels = Values != null && Values.Count > MaxLabelNumber ? false : newValue;
                btnShowLabels.IsEnabled = Values.Count <= MaxLabelNumber;
            }
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

        #region MarkerVisible
        public static readonly DependencyProperty MarkerVisibleProperty = DependencyProperty.Register("MarkerVisible", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerVisibleChanged), new CoerceValueCallback(OnCoerceMarkerVisible)));

        private static object OnCoerceMarkerVisible(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMarkerVisible((bool)value);
            else
                return value;
        }

        private static void OnMarkerVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnMarkerVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceMarkerVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesignmode)
                ChartMarkersVisible = newValue;
        }

        public bool MarkerVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(MarkerVisibleProperty);
            }
            set
            {
                SetValue(MarkerVisibleProperty, value);
            }
        }

        #endregion

        #region MarkerSize
        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register("MarkerSize", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(10, new PropertyChangedCallback(OnMarkerSizeChanged), new CoerceValueCallback(OnCoerceMarkerSize)));

        private static object OnCoerceMarkerSize(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMarkerSize((int)value);
            else
                return value;
        }

        private static void OnMarkerSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnMarkerSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int MarkerSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MarkerSizeProperty);
            }
            set
            {
                SetValue(MarkerSizeProperty, value);
            }
        }

        #endregion

        #region Transparency
        public static readonly DependencyProperty TransparencyProperty = DependencyProperty.Register("Transparency", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata(0.4, new PropertyChangedCallback(OnTransparencyChanged), new CoerceValueCallback(OnCoerceTransparency)));

        private static object OnCoerceTransparency(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceTransparency((double)value);
            else
                return value;
        }

        private static void OnTransparencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnTransparencyChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTransparency(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTransparencyChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double Transparency
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TransparencyProperty);
            }
            set
            {
                SetValue(TransparencyProperty, value);
            }
        }

        #endregion


        #region Title
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ServerHistoryTrend), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTitleChanged), new CoerceValueCallback(OnCoerceTitle)));

        private static object OnCoerceTitle(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceTitle((string)value);
            else
                return value;
        }

        private static void OnTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
                UpdateAxisTitles();
        }

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


        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(ServerHistoryTrend), new UIPropertyMetadata(DateSpan.Day, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
            if (bInit && !bDesignmode && !bUserInteractionSettings && serverHistoryTrend != null)
                ManageTimeRange(newValue);
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


        #region UseMaxReturnValues
        public static readonly DependencyProperty UseMaxReturnValuesProperty = DependencyProperty.Register("UseMaxReturnValues", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseMaxReturnValuesChanged), new CoerceValueCallback(OnCoerceUseMaxReturnValues)));

        private static object OnCoerceUseMaxReturnValues(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceUseMaxReturnValues((bool)value);
            else
                return value;
        }

        private static void OnUseMaxReturnValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnUseMaxReturnValuesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseMaxReturnValues(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseMaxReturnValuesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }

        public bool UseMaxReturnValues
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseMaxReturnValuesProperty);
            }
            set
            {
                SetValue(UseMaxReturnValuesProperty, value);
            }
        }

        #endregion


        #region MaxReturnValues
        public static readonly DependencyProperty MaxReturnValuesProperty = DependencyProperty.Register("MaxReturnValues", typeof(uint), typeof(ServerHistoryTrend), new UIPropertyMetadata((uint)25, new PropertyChangedCallback(OnMaxReturnValuesChanged), new CoerceValueCallback(OnCoerceMaxReturnValues)));

        private static object OnCoerceMaxReturnValues(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMaxReturnValues((uint)value);
            else
                return value;
        }

        private static void OnMaxReturnValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnMaxReturnValuesChanged((uint)e.OldValue, (uint)e.NewValue);
        }

        protected virtual uint OnCoerceMaxReturnValues(uint value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxReturnValuesChanged(uint oldValue, uint newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }

        public uint MaxReturnValues
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (uint)GetValue(MaxReturnValuesProperty);
            }
            set
            {
                SetValue(MaxReturnValuesProperty, value);
            }
        }

        #endregion



        #region ShowCommandPanel
        public static readonly DependencyProperty ShowCommandPanelProperty = DependencyProperty.Register("ShowCommandPanel", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCommandPanelChanged), new CoerceValueCallback(OnCoerceShowCommandPanel)));

        private static object OnCoerceShowCommandPanel(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceShowCommandPanel((Boolean)value);
            else
                return value;
        }

        private static void OnShowCommandPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnShowCommandPanelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowCommandPanel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCommandPanelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean ShowCommandPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowCommandPanelProperty);
            }
            set
            {
                SetValue(ShowCommandPanelProperty, value);
            }
        }
        #endregion

        #region AutoHideToolbar
        public static readonly DependencyProperty AutoHideToolbarProperty = DependencyProperty.Register("AutoHideToolbar", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideToolbarChanged), new CoerceValueCallback(OnCoerceAutoHideToolbar)));

        private static object OnCoerceAutoHideToolbar(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceAutoHideToolbar((bool)value);
            else
                return value;
        }

        private static void OnAutoHideToolbarChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnAutoHideToolbarChanged((bool)e.OldValue, (bool)e.NewValue);
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
                if (bDispose)
                    return;

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
        [Category("TrendOptions")]
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

        #region DefToolbarHeight
        public static readonly DependencyProperty DefToolbarHeightProperty = DependencyProperty.Register("DefToolbarHeight", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata(25d));
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


        #region AlwaysExpanded
        public static readonly DependencyProperty AlwaysExpandedProperty = DependencyProperty.Register("AlwaysExpanded", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAlwaysExpandedChanged), new CoerceValueCallback(OnCoerceAlwaysExpanded)));

        private static object OnCoerceAlwaysExpanded(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceAlwaysExpanded((bool)value);
            else
                return value;
        }

        private static void OnAlwaysExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnAlwaysExpandedChanged((bool)e.OldValue, (bool)e.NewValue);
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
        [Category("TrendOptions")]
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


        #region AutoCollapseHeight
        public static readonly DependencyProperty AutoCollapseHeightProperty = DependencyProperty.Register("AutoCollapseHeight", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseHeightChanged), new CoerceValueCallback(OnCoerceAutoCollapseHeight)));

        private static object OnCoerceAutoCollapseHeight(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceAutoCollapseHeight((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnAutoCollapseHeightChanged((double)e.OldValue, (double)e.NewValue);
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
        [Category("TrendOptions")]
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
        public static readonly DependencyProperty AutoCollapseWidthProperty = DependencyProperty.Register("AutoCollapseWidth", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata(300.0, new PropertyChangedCallback(OnAutoCollapseWidthChanged), new CoerceValueCallback(OnCoerceAutoCollapseWidth)));

        private static object OnCoerceAutoCollapseWidth(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceAutoCollapseWidth((double)value);
            else
                return value;
        }

        private static void OnAutoCollapseWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnAutoCollapseWidthChanged((double)e.OldValue, (double)e.NewValue);
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
        [Category("TrendOptions")]
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


        #region AutomaticScale
        public static readonly DependencyProperty AutomaticScaleProperty = DependencyProperty.Register("AutomaticScale", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAutomaticScaleChanged), new CoerceValueCallback(OnCoerceAutomaticScale)));

        private static object OnCoerceAutomaticScale(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceAutomaticScale((bool)value);
            else
                return value;
        }

        private static void OnAutomaticScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
            if (bLoaded && !bDispose && bInit)
                UpdateAxisRange();
        }
        [Category("Advanced")]
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


        #region Minimum
        private static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata((double)0, new PropertyChangedCallback(OnMinimumChanged), new CoerceValueCallback(OnCoerceMinimum)));

        private static object OnCoerceMinimum(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMinimum((double)value);
            else
                return value;
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
        private static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ServerHistoryTrend), new UIPropertyMetadata((double)100, new PropertyChangedCallback(OnMaximumChanged), new CoerceValueCallback(OnCoerceMaximum)));

        private static object OnCoerceMaximum(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMaximum((double)value);
            else
                return value;
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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


        #region PointPrecision
        public static readonly DependencyProperty PointPrecisionProperty = DependencyProperty.Register("PointPrecision", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(2, new PropertyChangedCallback(OnPointPrecisionChanged), new CoerceValueCallback(OnCoercePointPrecision)));

        private static object OnCoercePointPrecision(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoercePointPrecision((int)value);
            else
                return value;
        }

        private static void OnPointPrecisionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
        #region TextPattern
        public static readonly DependencyProperty TextPatternProperty = DependencyProperty.Register("TextPattern", typeof(string), typeof(ServerHistoryTrend), new UIPropertyMetadata("{A}: {V:F2}"));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        #region AxisYTextPattern
        public static readonly DependencyProperty AxisYTextPatternProperty = DependencyProperty.Register("AxisYTextPattern", typeof(string), typeof(ServerHistoryTrend), new UIPropertyMetadata("{V:F2}"));
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

        public static readonly DependencyProperty FetchdataOnScreenLoadingProperty = DependencyProperty.Register("FetchdataOnScreenLoading", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnFetchdataOnScreenLoadingChanged), new CoerceValueCallback(OnCoerceFetchdataOnScreenLoading)));

        private static object OnCoerceFetchdataOnScreenLoading(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceFetchdataOnScreenLoading((Boolean)value);
            else
                return value;
        }

        private static void OnFetchdataOnScreenLoadingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnFetchdataOnScreenLoadingChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceFetchdataOnScreenLoading(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFetchdataOnScreenLoadingChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean FetchdataOnScreenLoading
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(FetchdataOnScreenLoadingProperty);
            }
            set
            {
                SetValue(FetchdataOnScreenLoadingProperty, value);
            }
        }


        #region UseStartTime
        public static readonly DependencyProperty UseStartTimeProperty = DependencyProperty.Register("UseStartTime", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseStartTimeChanged), new CoerceValueCallback(OnCoerceUseStartTime)));

        private static object OnCoerceUseStartTime(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceUseStartTime((bool)value);
            else
                return value;
        }

        private static void OnUseStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnUseStartTimeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseStartTime(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseStartTimeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }

        [Category("TrendOptions")]
        public bool UseStartTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseStartTimeProperty);
            }
            set
            {
                SetValue(UseStartTimeProperty, value);
            }
        }


        #endregion

        #region StartTime
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(ServerHistoryTrend), new UIPropertyMetadata(new DateTime(2014, 1, 1), new PropertyChangedCallback(OnStartTimeChanged), new CoerceValueCallback(OnCoerceStartTime)));

        private static object OnCoerceStartTime(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceStartTime((DateTime)value);
            else
                return value;
        }

        private static void OnStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnStartTimeChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual DateTime OnCoerceStartTime(DateTime value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartTimeChanged(DateTime oldValue, DateTime newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }
        [Category("TrendOptions")]
        public DateTime StartTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateTime)GetValue(StartTimeProperty);
            }
            set
            {
                SetValue(StartTimeProperty, value);
            }
        }

        #endregion

        #region UseEndTime
        public static readonly DependencyProperty UseEndTimeProperty = DependencyProperty.Register("UseEndTime", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEndTimeChanged), new CoerceValueCallback(OnCoerceUseEndTime)));

        private static object OnCoerceUseEndTime(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceUseEndTime((bool)value);
            else
                return value;
        }

        private static void OnUseEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnUseEndTimeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseEndTime(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseEndTimeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }

        [Category("TrendOptions")]
        public bool UseEndTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseEndTimeProperty);
            }
            set
            {
                SetValue(UseEndTimeProperty, value);
            }
        }


        #endregion

        #region EndTime
        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register("EndTime", typeof(DateTime), typeof(ServerHistoryTrend), new UIPropertyMetadata(new DateTime(2099, 12, 31), new PropertyChangedCallback(OnEndTimeChanged), new CoerceValueCallback(OnCoerceEndTime)));

        private static object OnCoerceEndTime(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceEndTime((DateTime)value);
            else
                return value;
        }

        private static void OnEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnEndTimeChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual DateTime OnCoerceEndTime(DateTime value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndTimeChanged(DateTime oldValue, DateTime newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bDesignmode && bInit)
            {
                EventArgs m = new EventArgs();
                OnUpdateOptions(m);
            }
        }
        [Category("TrendOptions")]
        public DateTime EndTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateTime)GetValue(EndTimeProperty);
            }
            set
            {
                SetValue(EndTimeProperty, value);
            }
        }

        #endregion


        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(ServerHistoryTrend));
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

        public static readonly DependencyProperty TitleFonstSettingsProperty = DependencyProperty.Register("TitleFonstSettings", typeof(FontSettings), typeof(ServerHistoryTrend), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnTitleFonstSettingsChanged), new CoerceValueCallback(OnCoerceTitleFonstSettings)));

        private static object OnCoerceTitleFonstSettings(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceTitleFonstSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnTitleFonstSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnTitleFonstSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceTitleFonstSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleFonstSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null && newValue != oldValue)
            {
                UpdateValueFont(newValue);

                //{

                //}
                title1.FontSize = newValue.FontSize;
                title1.FontFamily = newValue.FontFamily;
                title1.FontWeight = newValue.FontWeight;
                title1.FontStyle = newValue.FontStyle;
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
        [Category("TrendOptions")]
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

        public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(ServerHistoryTrend), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnTitleForegroundChanged), new CoerceValueCallback(OnCoerceTitleForeground)));

        private static object OnCoerceTitleForeground(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceTitleForeground((Brush)value);
            else
                return value;
        }

        private static void OnTitleForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnTitleForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTitleForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                UpdateControlLayout();
            }
        }

        [Category("TrendOptions")]
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
        public static readonly DependencyProperty ControlFontSettingsProperty = DependencyProperty.Register("ControlFontSettings", typeof(FontSettings), typeof(ServerHistoryTrend), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnControlFontSettingsChanged), new CoerceValueCallback(OnCoerceControlFontSettings)));

        private static object OnCoerceControlFontSettings(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceControlFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnControlFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnControlFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceControlFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null && newValue != oldValue)
            {
                enStartTime.FontSize = newValue.FontSize;
                enStartTime.FontFamily = newValue.FontFamily;
                enStartTime.FontWeight = newValue.FontWeight;
                enStartTime.FontStyle = newValue.FontStyle;

                startTime.FontSize = newValue.FontSize;
                startTime.FontFamily = newValue.FontFamily;
                startTime.FontWeight = newValue.FontWeight;
                startTime.FontStyle = newValue.FontStyle;

                enEndTime.FontSize = newValue.FontSize;
                enEndTime.FontFamily = newValue.FontFamily;
                enEndTime.FontWeight = newValue.FontWeight;
                enEndTime.FontStyle = newValue.FontStyle;

                endTime.FontSize = newValue.FontSize;
                endTime.FontFamily = newValue.FontFamily;
                endTime.FontWeight = newValue.FontWeight;
                endTime.FontStyle = newValue.FontStyle;

                _maxReturnValues.FontSize = newValue.FontSize;
                _maxReturnValues.FontFamily = newValue.FontFamily;
                _maxReturnValues.FontWeight = newValue.FontWeight;
                _maxReturnValues.FontStyle = newValue.FontStyle;

                maxReturnValues.FontSize = newValue.FontSize;
                maxReturnValues.FontFamily = newValue.FontFamily;
                maxReturnValues.FontWeight = newValue.FontWeight;
                maxReturnValues.FontStyle = newValue.FontStyle;

                readType.FontSize = newValue.FontSize;
                readType.FontFamily = newValue.FontFamily;
                readType.FontWeight = newValue.FontWeight;
                readType.FontStyle = newValue.FontStyle;

                _ReadType.FontSize = newValue.FontSize;
                _ReadType.FontFamily = newValue.FontFamily;
                _ReadType.FontWeight = newValue.FontWeight;
                _ReadType.FontStyle = newValue.FontStyle;

                aggregationType.FontSize = newValue.FontSize;
                aggregationType.FontFamily = newValue.FontFamily;
                aggregationType.FontWeight = newValue.FontWeight;
                aggregationType.FontStyle = newValue.FontStyle;

                _AggregationType.FontSize = newValue.FontSize;
                _AggregationType.FontFamily = newValue.FontFamily;
                _AggregationType.FontWeight = newValue.FontWeight;
                _AggregationType.FontStyle = newValue.FontStyle;

                controlViewMode.FontSize = newValue.FontSize;
                controlViewMode.FontFamily = newValue.FontFamily;
                controlViewMode.FontWeight = newValue.FontWeight;
                controlViewMode.FontStyle = newValue.FontStyle;

                _ControlViewModel.FontSize = newValue.FontSize;
                _ControlViewModel.FontFamily = newValue.FontFamily;
                _ControlViewModel.FontWeight = newValue.FontWeight;
                _ControlViewModel.FontStyle = newValue.FontStyle;

                Fetch.FontSize = newValue.FontSize;
                Fetch.FontFamily = newValue.FontFamily;
                Fetch.FontWeight = newValue.FontWeight;
                Fetch.FontStyle = newValue.FontStyle;

                Previous.FontSize = newValue.FontSize;
                Previous.FontFamily = newValue.FontFamily;
                Previous.FontWeight = newValue.FontWeight;
                Previous.FontStyle = newValue.FontStyle;

                Next.FontSize = newValue.FontSize;
                Next.FontFamily = newValue.FontFamily;
                Next.FontWeight = newValue.FontWeight;
                Next.FontStyle = newValue.FontStyle;

                Stop.FontSize = newValue.FontSize;
                Stop.FontFamily = newValue.FontFamily;
                Stop.FontWeight = newValue.FontWeight;
                Stop.FontStyle = newValue.FontStyle;

            }
        }

        [Category("TrendOptions")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings ControlFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(ControlFontSettingsProperty);
            }
            set
            {
                SetValue(ControlFontSettingsProperty, value);
            }
        }
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(ServerHistoryTrend), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        //private static object OnCoerceControlForeground(DependencyObject o, object value)
        //{
        //    ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
        //    if (ServerHistoryTrend != null)
        //        return ServerHistoryTrend.OnCoerceControlForeground((Brush)value);
        //    else
        //        return value;
        //}

        //private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
        //    if (ServerHistoryTrend != null)
        //        ServerHistoryTrend.OnControlForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        //}

        //protected virtual Brush OnCoerceControlForeground(Brush value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnControlForegroundChanged(Brush oldValue, Brush newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    if (bInit)
        //    {
        //        UpdateControlLayout();
        //    }
        //}

        //[Category("TrendOptions")]
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



        #region ToolbarForeground
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(ServerHistoryTrend), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("TrendOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
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


        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(ServerHistoryTrend), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("TrendOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
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



        public static readonly DependencyProperty YAxsisFontSettingsProperty = DependencyProperty.Register("YAxsisFontSettings", typeof(FontSettings), typeof(ServerHistoryTrend), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnYAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceYAxsisFontSettings)));

        private static object OnCoerceYAxsisFontSettings(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceYAxsisFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnYAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnYAxsisFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceYAxsisFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYAxsisFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TDO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                axisY.FontSize = newValue.FontSize;
                axisY.FontFamily = newValue.FontFamily;
                axisY.FontWeight = newValue.FontWeight;
                axisY.FontStyle = newValue.FontStyle;
            }
        }

        [Category("TrendOptions")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings YAxsisFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(YAxsisFontSettingsProperty);
            }
            set
            {
                SetValue(YAxsisFontSettingsProperty, value);
            }
        }


        public static readonly DependencyProperty XAxsisFontSettingsProperty = DependencyProperty.Register("XAxsisFontSettings", typeof(FontSettings), typeof(ServerHistoryTrend), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 10), new PropertyChangedCallback(OnXAxsisFontSettingsChanged), new CoerceValueCallback(OnCoerceXAxsisFontSettings)));

        private static object OnCoerceXAxsisFontSettings(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceXAxsisFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnXAxsisFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnXAxsisFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceXAxsisFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXAxsisFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue)
            {
                axisX.FontSize = newValue.FontSize;
                axisX.FontFamily = newValue.FontFamily;
                axisX.FontWeight = newValue.FontWeight;
                axisX.FontStyle = newValue.FontStyle;
            }
        }

        [Category("TrendOptions")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings XAxsisFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(XAxsisFontSettingsProperty);
            }
            set
            {
                SetValue(XAxsisFontSettingsProperty, value);
            }
        }


        public static readonly DependencyProperty PlotBackgroundProperty = DependencyProperty.Register("PlotBackground", typeof(Brush), typeof(ServerHistoryTrend), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36)), new PropertyChangedCallback(OnPlotBackgroundChanged), new CoerceValueCallback(OnCoercePlotBackground)));

        private static object OnCoercePlotBackground(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoercePlotBackground((Brush)value);
            else
                return value;
        }

        private static void OnPlotBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnPlotBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
            {
                UpdateControlLayout();
            }
        }

        private void UpdateControlLayout()
        {



            // Toolbar color settings doesn't support anymore after change from ToolBarTry to DevExpress's BarContainerControl.
            //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    toolbar.Background = ToolbarBackground;
            //    toolbarSettings.Background = ToolbarBackground;
            //    toolbarOptions.Background = ToolbarBackground;
            //}

            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    cmbTimeRange.Foreground = ToolbarForeground;
            //    startText.Foreground = ToolbarForeground;
            //    endText.Foreground = ToolbarForeground;
            //    bestFit.Foreground = ToolbarForeground;
            //}


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

                axisYLabel.Foreground = Foreground;
                axisXLabel.Foreground = Foreground;
                tableView.ColumnHeaderTemplate = LoadTemplate("ColumnHeaderTemplate") as DataTemplate;
            }
            if (this.ReadLocalValue(PlotBackgroundProperty) != DependencyProperty.UnsetValue)
            {
                groupGeneral.Background = PlotBackground;
                chart.Background = PlotBackground;
                view.Background = PlotBackground;
            }
            if (this.ReadLocalValue(TitleForegroundProperty) != DependencyProperty.UnsetValue)
                title1.Foreground = TitleForeground;
            listView1.Background = Background;
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

        }
        private object LoadTemplate(string template)
        {
            try
            {
                string basename = "HistoryTrendTemplates.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.Resources.{1}", typeof(ServerHistoryTrend).Namespace, basename));
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
            catch (Exception ex)
            {
                return null;
            }
        }
        [Category("TrendOptions")]
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

        public static readonly DependencyProperty ShowCrosshairLineProperty = DependencyProperty.Register("ShowCrosshairLine", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCrosshairLineChanged), new CoerceValueCallback(OnCoerceShowCrosshairLine)));

        private static object OnCoerceShowCrosshairLine(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceShowCrosshairLine((Boolean)value);
            else
                return value;
        }

        private static void OnShowCrosshairLineChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnShowCrosshairLineChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowCrosshairLine(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCrosshairLineChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean ShowCrosshairLine
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowCrosshairLineProperty);
            }
            set
            {
                SetValue(ShowCrosshairLineProperty, value);
            }
        }

        public static readonly DependencyProperty ShowArgumentLabelValueProperty = DependencyProperty.Register("ShowArgumentLabelValue", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowArgumentLabelValueChanged), new CoerceValueCallback(OnCoerceShowArgumentLabelValue)));

        private static object OnCoerceShowArgumentLabelValue(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceShowArgumentLabelValue((Boolean)value);
            else
                return value;
        }

        private static void OnShowArgumentLabelValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnShowArgumentLabelValueChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowArgumentLabelValue(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowArgumentLabelValueChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean ShowArgumentLabelValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowArgumentLabelValueProperty);
            }
            set
            {
                SetValue(ShowArgumentLabelValueProperty, value);
            }
        }

        public static readonly DependencyProperty ShowCrosshairLabelProperty = DependencyProperty.Register("ShowCrosshairLabel", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCrosshairLabelChanged), new CoerceValueCallback(OnCoerceShowCrosshairLabel)));

        private static object OnCoerceShowCrosshairLabel(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceShowCrosshairLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowCrosshairLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnShowCrosshairLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowCrosshairLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCrosshairLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean ShowCrosshairLabel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowCrosshairLabelProperty);
            }
            set
            {
                SetValue(ShowCrosshairLabelProperty, value);
            }
        }

        public static readonly DependencyProperty YAxisLabelPrefixProperty = DependencyProperty.Register("YAxisLabelPrefix", typeof(string), typeof(ServerHistoryTrend), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnYAxisLabelPrefixChanged), new CoerceValueCallback(OnCoerceYAxisLabelPrefix)));

        private static object OnCoerceYAxisLabelPrefix(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceYAxisLabelPrefix((string)value);
            else
                return value;
        }

        private static void OnYAxisLabelPrefixChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnYAxisLabelPrefixChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceYAxisLabelPrefix(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYAxisLabelPrefixChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public string YAxisLabelPrefix
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(YAxisLabelPrefixProperty);
            }
            set
            {
                SetValue(YAxisLabelPrefixProperty, value);
            }
        }

        public static readonly DependencyProperty YGridLineVisibleProperty = DependencyProperty.Register("YGridLineVisible", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYGridLineVisibleChanged), new CoerceValueCallback(OnCoerceYGridLineVisible)));

        private static object OnCoerceYGridLineVisible(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceYGridLineVisible((Boolean)value);
            else
                return value;
        }

        private static void OnYGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnYGridLineVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceYGridLineVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYGridLineVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean YGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(YGridLineVisibleProperty);
            }
            set
            {
                SetValue(YGridLineVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty YGridLineMinorVisibleProperty = DependencyProperty.Register("YGridLineMinorVisible", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnYGridLineMinorVisibleChanged), new CoerceValueCallback(OnCoerceYGridLineMinorVisible)));

        private static object OnCoerceYGridLineMinorVisible(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceYGridLineMinorVisible((Boolean)value);
            else
                return value;
        }

        private static void OnYGridLineMinorVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnYGridLineMinorVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceYGridLineMinorVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnYGridLineMinorVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean YGridLineMinorVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(YGridLineMinorVisibleProperty);
            }
            set
            {
                SetValue(YGridLineMinorVisibleProperty, value);
            }
        }
        public static readonly DependencyProperty XGridLineVisibleProperty = DependencyProperty.Register("XGridLineVisible", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXGridLineVisibleChanged), new CoerceValueCallback(OnCoerceXGridLineVisible)));

        private static object OnCoerceXGridLineVisible(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceXGridLineVisible((Boolean)value);
            else
                return value;
        }

        private static void OnXGridLineVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnXGridLineVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceXGridLineVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXGridLineVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean XGridLineVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(XGridLineVisibleProperty);
            }
            set
            {
                SetValue(XGridLineVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty XGridLineMinorVisibleProperty = DependencyProperty.Register("XGridLineMinorVisible", typeof(Boolean), typeof(ServerHistoryTrend), new UIPropertyMetadata(true, new PropertyChangedCallback(OnXGridLineMinorVisibleChanged), new CoerceValueCallback(OnCoerceXGridLineMinorVisible)));

        private static object OnCoerceXGridLineMinorVisible(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceXGridLineMinorVisible((Boolean)value);
            else
                return value;
        }

        private static void OnXGridLineMinorVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnXGridLineMinorVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceXGridLineMinorVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXGridLineMinorVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public Boolean XGridLineMinorVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(XGridLineMinorVisibleProperty);
            }
            set
            {
                SetValue(XGridLineMinorVisibleProperty, value);
            }
        }


        public static readonly DependencyProperty XLabelFontSizeProperty = DependencyProperty.Register("XLabelFontSize", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(8, new PropertyChangedCallback(OnXLabelFontSizeChanged), new CoerceValueCallback(OnCoerceXLabelFontSize)));

        private static object OnCoerceXLabelFontSize(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceXLabelFontSize((int)value);
            else
                return value;
        }

        private static void OnXLabelFontSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnXLabelFontSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceXLabelFontSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnXLabelFontSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("TrendOptions")]
        public int XLabelFontSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(XLabelFontSizeProperty);
            }
            set
            {
                SetValue(XLabelFontSizeProperty, value);
            }
        }
        #region ReadTypeDefinition
        public static readonly DependencyProperty ReadTypeDefinitionProperty = DependencyProperty.Register("ReadTypeDefinition", typeof(ReadTypeDefinition), typeof(ServerHistoryTrend), new UIPropertyMetadata(ReadTypeDefinition.Raw, new PropertyChangedCallback(OnReadTypeDefinitionChanged), new CoerceValueCallback(OnCoerceReadTypeDefinition)));

        private static object OnCoerceReadTypeDefinition(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceReadTypeDefinition((ReadTypeDefinition)value);
            else
                return value;
        }

        private static void OnReadTypeDefinitionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnReadTypeDefinitionChanged((ReadTypeDefinition)e.OldValue, (ReadTypeDefinition)e.NewValue);
        }

        protected virtual ReadTypeDefinition OnCoerceReadTypeDefinition(ReadTypeDefinition value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReadTypeDefinitionChanged(ReadTypeDefinition oldValue, ReadTypeDefinition newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                readType.SelectedIndex = (int)newValue;
                if (!bDesignmode || bSmartSettingsEditing)
                {
                    if (!bDesignmode && serverHistoryTrend != null)
                        serverHistoryTrend.ReadType = (HistoryReadViewModel.ReadTypeDefinition)newValue;
                    aggregationType.IsEnabled = newValue == ReadTypeDefinition.Processed;
                }
            }
            OnPropertyVisiblityChanged("Aggregate");
        }

        [Category("TrendOptions")]
        public ReadTypeDefinition ReadTypeDefinition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ReadTypeDefinition)GetValue(ReadTypeDefinitionProperty);
            }
            set
            {
                SetValue(ReadTypeDefinitionProperty, value);
            }
        }
        #endregion
        #region Aggregate
        public static readonly DependencyProperty AggregateProperty = DependencyProperty.Register("Aggregate", typeof(AggregationTypeDefinition), typeof(ServerHistoryTrend), new UIPropertyMetadata(AggregationTypeDefinition.Average, new PropertyChangedCallback(OnAggregateChanged), new CoerceValueCallback(OnCoerceAggregate)));

        private static object OnCoerceAggregate(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceAggregate((AggregationTypeDefinition)value);
            else
                return value;
        }

        private static void OnAggregateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnAggregateChanged((AggregationTypeDefinition)e.OldValue, (AggregationTypeDefinition)e.NewValue);
        }

        protected virtual AggregationTypeDefinition OnCoerceAggregate(AggregationTypeDefinition value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAggregateChanged(AggregationTypeDefinition oldValue, AggregationTypeDefinition newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
            {
                aggregationType.SelectedIndex = (int)newValue;
                if (!bDesignmode && serverHistoryTrend != null)
                    serverHistoryTrend.Aggregate = browsableNames[(int)newValue];
            }
        }
        [Category("TrendOptions")]
        public AggregationTypeDefinition Aggregate
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (AggregationTypeDefinition)GetValue(AggregateProperty);
            }
            set
            {
                SetValue(AggregateProperty, value);
            }
        }
        #endregion


        #region MaxHistoryCount
        public static readonly DependencyProperty MaxHistoryCountProperty = DependencyProperty.Register("MaxHistoryCount", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(3600, new PropertyChangedCallback(OnMaxHistoryCountChanged), new CoerceValueCallback(OnCoerceMaxHistoryCount)));

        private static object OnCoerceMaxHistoryCount(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMaxHistoryCount((int)value);
            else
                return value;
        }

        private static void OnMaxHistoryCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnMaxHistoryCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxHistoryCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxHistoryCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public int MaxHistoryCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxHistoryCountProperty);
            }
            set
            {
                SetValue(MaxHistoryCountProperty, value);
            }
        }

        #endregion



        #region DockLayout
        public static readonly DependencyProperty DockLayoutProperty = DependencyProperty.Register("DockLayout", typeof(String), typeof(ServerHistoryTrend), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDockLayoutChanged), new CoerceValueCallback(OnCoerceDockLayout)));

        private static object OnCoerceDockLayout(DependencyObject o, object value)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                return serverHistoryTrend.OnCoerceDockLayout((String)value);
            else
                return value;
        }

        private static void OnDockLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend serverHistoryTrend = o as ServerHistoryTrend;
            if (serverHistoryTrend != null)
                serverHistoryTrend.OnDockLayoutChanged((String)e.OldValue, (String)e.NewValue);
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
                dockManager.SaveLayoutToStream(output);
                resetDockLayout = Encoding.Default.GetString(output.ToArray());
            }
        }

        internal void SaveDesignDockLayout()
        {
            if (dockManager == null)
                return;

            using (MemoryStream output = new MemoryStream())
            {
                dockManager.SaveLayoutToStream(output);
                DockLayout = Encoding.Default.GetString(output.ToArray());
            }
        }

        void LoadDesignDockLayout()
        {
            if (dockManager == null || string.IsNullOrEmpty(DockLayout))
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
                            dockManager.RestoreLayoutFromStream(output);
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
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(string), typeof(ServerHistoryTrend), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceGridLayout((string)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
                listView1.SaveLayoutToStream(output);
                resetGridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void SaveDesignGridLayout()
        {
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                listView1.SaveLayoutToStream(output);
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
                            listView1.RestoreLayoutFromStream(output);
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
        #endregion

        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
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


        //[Browsable(false)]
        //public UserControl SmartControl
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return new Controls.SmartControl(this);
        //    }
        //}

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false));

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
        int ClientTimezoneOffset
        {
            get
            {
                return (int)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public TimeFilterViewModel TimeFilterVM { get; set; } = new TimeFilterViewModel();

        #region ChartShowLabels
        public static readonly DependencyProperty ChartShowLabelsProperty = DependencyProperty.Register("ChartShowLabels", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false));
        [Browsable(false)]
        [XmlIgnore]
        public bool ChartShowLabels
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ChartShowLabelsProperty);
            }
            set
            {
                SetValue(ChartShowLabelsProperty, value);
            }
        }

        #endregion

        #region ChartMarkersVisible
        public static readonly DependencyProperty ChartMarkersVisibleProperty = DependencyProperty.Register("ChartMarkersVisible", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(true));
        [Browsable(false)]
        [XmlIgnore]
        public bool ChartMarkersVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ChartMarkersVisibleProperty);
            }
            set
            {
                SetValue(ChartMarkersVisibleProperty, value);
            }
        }

        #endregion


        #region MaxLabelNumber
        public static readonly DependencyProperty MaxLabelNumberProperty = DependencyProperty.Register("MaxLabelNumber", typeof(int), typeof(ServerHistoryTrend), new UIPropertyMetadata(100, new PropertyChangedCallback(OnMaxLabelNumberChanged), new CoerceValueCallback(OnCoerceMaxLabelNumber)));

        private static object OnCoerceMaxLabelNumber(DependencyObject o, object value)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                return control.OnCoerceMaxLabelNumber((int)value);
            else
                return value;
        }

        private static void OnMaxLabelNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend control = o as ServerHistoryTrend;
            if (control != null)
                control.OnMaxLabelNumberChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxLabelNumber(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxLabelNumberChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bDesignmode && bInit)
            {
                ChartMarkersVisible = ChartShowLabels = Values.Count > newValue ? false : ShowLabels;
                btnShowLabels.IsEnabled = Values.Count <= newValue;
            }
        }

        public int MaxLabelNumber
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxLabelNumberProperty);
            }
            set
            {
                SetValue(MaxLabelNumberProperty, value);
            }
        }

        #endregion

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(ServerHistoryTrend), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                return ServerHistoryTrend.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ServerHistoryTrend ServerHistoryTrend = o as ServerHistoryTrend;
            if (ServerHistoryTrend != null)
                ServerHistoryTrend.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region Declarations
        bool bLoaded;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ServerHistoryTrendControl);
        internal bool bSmartSettingsEditing;
        private List<string> browsableNames = new List<string>();
        private IDictionary<String, String> stringlist;
        event EventHandler UpdateOptions;
        private void OnUpdateOptions(EventArgs e)
        {
            EventHandler temp = UpdateOptions;
            if (temp != null)
                temp(null, e);
        }

        bool bDesignmode;
        DateTime MinDate = new DateTime(1900, 1, 1);
        DateTime MaxDate = new DateTime(2099, 12, 31);
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        UFInterfaces.IWorkspace workspace;
        IUFProjectManager iUFProjectManager;
        IDocument Document;
        IStringEditorManager stringManager;

        private DelayedSingleActionInvoker SizeChangedInvoker;
        List<DataValueCollection> _historyCache;
        List<DataValueCollection> historyCache
        {
            get
            {
                if (_historyCache == null)
                    _historyCache = new List<DataValueCollection>();
                return _historyCache;
            }
            set
            {
                _historyCache = value;
            }
        }
        int currentIndex = 0;

        #region Values
        //public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register("Values", typeof(List<DataValue>), typeof(ServerHistoryTrend), new UIPropertyMetadata(new DataValueCollection(), new PropertyChangedCallback(OnValuesChanged), new CoerceValueCallback(OnCoerceValues)));

        //private static object OnCoerceValues(DependencyObject o, object value)
        //{
        //    ServerHistoryTrend control = o as ServerHistoryTrend;
        //    if (control != null)
        //        return control.OnCoerceValues((List<DataValue>)value);
        //    else
        //        return value;
        //}

        //private static void OnValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    ServerHistoryTrend control = o as ServerHistoryTrend;
        //    if (control != null)
        //        control.OnValuesChanged((List<DataValue>)e.OldValue, (List<DataValue>)e.NewValue);
        //}

        //protected virtual List<DataValue> OnCoerceValues(List<DataValue> value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnValuesChanged(List<DataValue> oldValue, List<DataValue> newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Browsable(false)]
        //public List<DataValue> Values
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (List<DataValue>)GetValue(ValuesProperty);
        //    }
        //    set
        //    {
        //        SetValue(ValuesProperty, value);
        //    }
        //}
        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register("Values", typeof(DataValueCollection), typeof(ServerHistoryTrend), new PropertyMetadata(new DataValueCollection()));
        [Browsable(false)]
        [XmlIgnore]
        public DataValueCollection Values
        {
            get { return (DataValueCollection)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        #endregion
        string designGridLayout;
        string designDockLayout;
        string resetGridLayout;
        string resetDockLayout;
        bool designUseStartTime;
        DateTime designStartTime;
        bool designUseEndTime;
        DateTime designEndTime;
        uint designMaxReturnValues;
        bool designUseMaxReturnValues;
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
        TraceMemorySettings MemorySettingList;
        #endregion

        #endregion

        #region ctor
        bool bInit;
        public ServerHistoryTrend()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////
            // devexpress optimized mode implementation
            // (see https://www.devexpress.com/Support/Center/Question/Details/T147586)
            ///////////////////////////////////////////////////////////////////////////////////////////
            tableView.UseLightweightTemplates = UseLightweightTemplates.None;

            InitBrowsableNames();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            if (RunningOnServer)
                if (DesignerProperties.GetIsInDesignMode(this))
                    bLoaded = false;
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
                    iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
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

                    SizeChanged -= OnSizeChanged;
                    SizeChanged += OnSizeChanged;

                    if (AlwaysExpanded)
                        HideAllHidden();
                    configMemory.DataContext = MemorySettingList?.Names;
                    configMemory.EditValue = Properties.Settings.Default.DesignSettingName;
                    UpdateControlLayout();

                    readType.ItemsSource = Enum.GetValues(typeof(ReadTypeDefinition)).Cast<ReadTypeDefinition>();
                    readType.SelectedIndex = (int)ReadTypeDefinition;
                    aggregationType.ItemsSource = Enum.GetValues(typeof(AggregationTypeDefinition)).Cast<AggregationTypeDefinition>();
                    aggregationType.SelectedIndex = (int)Aggregate;

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

                    DefToolbarHeight = toolbar.ActualHeight;
                    if (bDesignmode)
                    {
                        DesignerProperties.SetIsInDesignMode(this, false); // this line is needed otherwise disposing docking throws an exception
                        bDesignmode = true;
                        chart.CrosshairOptions.ShowArgumentLine = false;
                        chart.CrosshairOptions.ShowValueLine = false;
                        chart.CrosshairOptions.ShowArgumentLabels = false;
                        chart.CrosshairOptions.ShowValueLabels = false;
                        chart.CrosshairOptions.ShowCrosshairLabels = false;
                        diagram.EnableAxisXNavigation = false;
                        diagram.EnableAxisYNavigation = false;
                        InitDesignSeries();
                        OverrideBaseProperties();
                        if (AutoHideToolbar)
                        {
                            toolbar_MouseLeave(null, null);
                            Grid.SetRow(adorner, 0);
                            Grid.SetRowSpan(adorner, 2);
                        }

                        toolbar.IsEnabled = false;
                        if (!bSmartSettingsEditing)
                        {
                            paramGrid.IsEnabled = false;
                            chartGrid.IsEnabled = false;
                            view.IsHitTestVisible = false;
                        }
                        else
                        {
                            aggregationType.IsEnabled = ReadTypeDefinition == ReadTypeDefinition.Processed;
                        }
                        bInit = true;
                        return;
                    }
                    else
                    {
                        if (RunningOnServer)
                        {
                            AutoHideToolbar = false;
                            HideAllHidden();
                            startText.IsVisible = endText.IsVisible = false;
                            tableView.ShowFilterPanelMode = DevExpress.Xpf.Grid.ShowFilterPanelMode.Never;
                            toolbarSettings.IsVisible = false;
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }

                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        if (UseTouchKeyboard)
                        {
                            startTime.TouchDown -= OnTouchDown;
                            endTime.TouchDown -= OnTouchDown;
                            maxReturnValues.TouchDown -= OnTouchDown;
                            startTime.LostFocus -= OnLostFocus;
                            endTime.LostFocus -= OnLostFocus;
                            maxReturnValues.LostFocus -= OnLostFocus;

                            startTime.TouchDown += OnTouchDown;
                            endTime.TouchDown += OnTouchDown;
                            maxReturnValues.TouchDown += OnTouchDown;
                            startTime.LostFocus += OnLostFocus;
                            endTime.LostFocus += OnLostFocus;
                            maxReturnValues.LostFocus += OnLostFocus;
                        }

                        OverrideBaseProperties();

                        InitDesign();
                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();

                        if (AutoHideToolbar)
                        {
                            toolbar_MouseLeave(null, null);
                            Grid.SetRow(adorner, 0);
                            Grid.SetRowSpan(adorner, 2);
                            toolbar.MouseEnter += toolbar_MouseEnter;
                            toolbar.MouseLeave += toolbar_MouseLeave;
                        }

                        UpdateAxisTitles();

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            try
                            {
                                LoadRuntimeLayout(GetStorageName(true));
                                GetItem(ActualConfig);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        UpdateAxisRange();
                        SetTimeRange(FilterType);
                        if (monitoredItemViewModel != null && serverHistoryTrend == null)
                            InitServer();
                        bInit = true;
                        if (!bControlLoaded)
                        {
                            bControlLoaded = true;
                            OnControlLoaded();
                        }
                    }

                }
            };
            ///////////////////////////////////////////////////////////////////////////////////////////
            // temporary code for fixing the devexpress bug
            // (see http://www.devexpress.com/Support/Center/Question/Details/Q384446)
            DataContextChanged += (o, e) =>
            {
                if (bDispose)
                    return;

                if (DataContext is MonitoredItemViewModel)
                {
                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel != null)
                    {
                        if (bInit)
                            InitServer();
                    }
                }
            };
            ///////////////////////////////////////////////////////////////////////////////////////////
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!bDispose && SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
        }
        #endregion


        #region EditSettings
        private void InitDesign()
        {
            designGridLayout = GridLayout;
            designDockLayout = DockLayout;
            designUseStartTime = UseStartTime;
            designStartTime = StartTime;
            designUseEndTime = UseEndTime;
            designEndTime = EndTime;
            designMaxReturnValues = MaxReturnValues;
            designUseMaxReturnValues = UseMaxReturnValues;
            EnsureDefaultValues();
        }
        void EnsureDefaultValues(bool bSetCombo = true)
        {
            MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<TraceMemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            var defaultsetting = (from m in MemorySettingList where m.Name.Equals(Properties.Settings.Default.DesignSettingName) select m).FirstOrDefault();
            if (defaultsetting == null)
            {
                defaultsetting = new TSetting()
                {
                    Name = Properties.Settings.Default.DesignSettingName,
                    DockLayout = designDockLayout,
                    GridLayout = designGridLayout,
                    ReadOnly = true,
                    UseStartTime = designUseStartTime,
                    StartTime = designStartTime,
                    UseEndTime = designUseEndTime,
                    EndTime = designEndTime,
                    MaxReturnValues = designMaxReturnValues,
                    UseMaxReturnValues = designUseMaxReturnValues
                };
                MemorySettingList.Add(defaultsetting);
            }
            else
            {
                defaultsetting.ReadOnly = true;
            }
            if (bSetCombo)
            {
                configMemory.DataContext = MemorySettingList.Names;
                bIsInEditMode = true;
                configMemory.EditValue = defaultsetting.Name;
                bIsInEditMode = false;
            }
        }
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
        private void configMemory_EditValueChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            string editValue = ((DevExpress.Xpf.Bars.BarEditItem)e.OriginalSource).EditValue as String;
            if (MemorySettingList != null && MemorySettingList.Names.Contains(editValue))
                ChangeSetting(editValue);
        }
        void ChangeSetting(String settingName)
        {
            if (String.IsNullOrEmpty(settingName) || ActualConfig == settingName ||
                bCallingRemoveCommand || bCallingSaveCommand || !bInit || bIsInEditMode)
                return;
            using (var cursor = new WaitCursor())
            {
                ActualConfig = settingName;
                SaveRuntimeLayout(GetStorageName(true));
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
        TraceMemorySettings oldMemoryList;
        string oldConfigName;
        bool bCallingResetCommand;
        internal void CallResetCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingResetCommand = true;
            try
            {
                MemorySettingList = new TraceMemorySettings(oldMemoryList);
                StorageHelper.StorageHelper.SaveMemoryMap<TraceMemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
                configMemory.DataContext = MemorySettingList.Names;

            }
            finally
            {
                bCallingResetCommand = false;
            }

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
                if (bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand)
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
        internal void CallSaveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingSaveCommand = true;
            try
            {
                string configname = configMemory.EditValue as String;
                string defaultTagSetting = Properties.Settings.Default.DesignSettingName;

                if (configname == defaultTagSetting)
                    return;

                SaveDesignGridLayout();
                SaveDesignDockLayout();

                var selected = (from m in MemorySettingList
                                where m.Name == configname
                                select m).FirstOrDefault();
                if (selected == null)
                {
                    MemorySettingList.Add(new TSetting()
                    {
                        Name = configname,
                        UseStartTime = UseStartTime,
                        StartTime = StartTime,
                        UseEndTime = UseEndTime,
                        EndTime = EndTime,
                        MaxReturnValues = MaxReturnValues,
                        UseMaxReturnValues = UseMaxReturnValues,
                        GridLayout = GridLayout,
                        DockLayout = DockLayout,
                        ReadOnly = false
                    });
                    configMemory.DataContext = MemorySettingList.Names;
                    configMemory.EditValue = configname;
                }
                else
                {
                    selected.GridLayout = GridLayout;
                    selected.DockLayout = DockLayout;
                    selected.UseStartTime = UseStartTime;
                    selected.StartTime = StartTime;
                    selected.UseEndTime = UseEndTime;
                    selected.EndTime = EndTime;
                    selected.MaxReturnValues = MaxReturnValues;
                    selected.UseMaxReturnValues = UseMaxReturnValues;
                    selected.ReadOnly = false;
                    configMemory.DataContext = MemorySettingList.Names;
                    configMemory.EditValue = selected.Name;
                }

                if (StorageHelper.StorageHelper.SaveMemoryMap<TraceMemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null))
                {
                    if (oldMemoryList != null)
                        oldMemoryList.Clear();
                    oldMemoryList = null;
                    oldConfigName = null;
                }
                ActualConfig = configname;
                SaveRuntimeLayout(GetStorageName(true));
            }
            finally
            {
                bCallingSaveCommand = false;
            }
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
        internal void CallRemoveCommand()
        {
            if (helper != null && helper.ValidateAccessLevel())
            {
                return;
            }
            bCallingRemoveCommand = true;
            try
            {
                var selected = (from m in MemorySettingList
                                where m.Name == configMemory.EditValue as String
                                select m).FirstOrDefault();

                if (oldMemoryList == null)
                {
                    oldMemoryList = new TraceMemorySettings(MemorySettingList);
                    oldConfigName = configMemory.EditValue as String;
                }
                if (selected != null)
                    MemorySettingList.Remove(selected);
                StorageHelper.StorageHelper.SaveMemoryMap<TraceMemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
                configMemory.DataContext = MemorySettingList.Names;
            }
            finally
            {
                bCallingRemoveCommand = false;
            }
            
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
            if (!UserBasedRuntimeSettings)
                return;

            if (!String.IsNullOrEmpty(helper.Username))
                MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<TraceMemorySettings>(Document, Name, helper.Username);
            else
                EnsureDefaultValues(false);
            if (!LoadRuntimeLayout(GetStorageName(true)))
                ActualConfig = Properties.Settings.Default.DesignSettingName;
            GetItem(ActualConfig);
            
            if (FilterType != DateSpan.None)
            {
                UseEndTime = UseStartTime = true;
                SetTimeRange(FilterType);
            }
            if (serverHistoryTrend != null)
            {
                UpdateSettings();
                RefreshDatas();
            }
        }
        #endregion

        public void Initialize()
        {
        }
        #endregion
        private void GetItem(string itemName)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (!string.IsNullOrEmpty(setting?.GridLayout))
                {
                    GridLayout = setting.GridLayout;
                    DockLayout = setting.DockLayout;
                    ActualConfig = setting.Name;
                    UseStartTime = setting.UseStartTime;
                    StartTime = setting.StartTime;
                    UseEndTime = setting.UseEndTime;
                    EndTime = setting.EndTime;
                    MaxReturnValues = setting.MaxReturnValues;
                    UseMaxReturnValues = setting.UseMaxReturnValues;
                }
                else
                {
                    GridLayout = designGridLayout;
                    DockLayout = designDockLayout;
                    UseStartTime = designUseStartTime;
                    StartTime = designStartTime;
                    UseEndTime = designUseEndTime;
                    EndTime = designEndTime;
                    MaxReturnValues = designMaxReturnValues;
                    UseMaxReturnValues = designUseMaxReturnValues;
                    ActualConfig = Properties.Settings.Default.DesignSettingName;
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
            TraceMemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<TraceMemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
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



        #region Methods
        private void ManageTimeRange(DateSpan filterType)
        {
            UseEndTime = UseStartTime = true;
            SetTimeRange(filterType);
            UpdateSettings();
            RefreshDatas();
        }

        private void SetTimeRange(DateSpan filterType)
        {
            DateTime date1;
            DateTime date2;

            switch (filterType)
            {
                case DateSpan.All:
                    EnableNextPrev(false);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.All, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Minute:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Minute);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Minute, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Hour:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Hour);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Hour, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Day:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Day);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Day, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Week:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Week);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Week, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Month:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Month);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Month, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                case DateSpan.Year:
                    EnableNextPrev(true);
                    SelectTimeRangeCombo(DateSpan.Year);
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Year, true);
                    StartTime = date1;
                    EndTime = date2;
                    break;
                default:
                    break;
            }

            startText.GetBindingExpression(DevExpress.Xpf.Bars.BarEditItem.EditValueProperty).UpdateTarget();
            endText.GetBindingExpression(DevExpress.Xpf.Bars.BarEditItem.EditValueProperty).UpdateTarget();
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
        bool bHidden = false;
        MemoryStream saveAutoHiddenStream;
        void RestoreAllHidden()
        {
            if (bDispose)
                return;

            using (var cursor = new WaitCursor())
            {
                if (bHidden)
                {
                    try
                    {
                        bHidden = false;
                        saveAutoHiddenStream.Seek(0, SeekOrigin.Begin);
                        dockManager.RestoreLayoutFromStream(saveAutoHiddenStream);
                        saveAutoHiddenStream.Dispose();
                        saveAutoHiddenStream = null;

                        dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Auto;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
        private void ExpandCollapse_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (!bHidden)
                HideAllHidden();
            else
                RestoreAllHidden();

            (e.Item as DevExpress.Xpf.Bars.BarCheckItem).IsChecked = bHidden;
        }

        TimeSpan GetSelectedTimeRangeCombo()
        {
            var dateSpan = GelectTimeRangeCombo();
            switch (dateSpan)
            {
                case DateSpan.Minute: return new TimeSpan(0, 1, 0);
                case DateSpan.Hour: return new TimeSpan(1, 0, 0);
                case DateSpan.Day: return new TimeSpan(1, 0, 0, 0);
                case DateSpan.Week: return new TimeSpan(7, 0, 0, 0);
                case DateSpan.Month: return new TimeSpan(30, 0, 0, 0);
                case DateSpan.Year: return new TimeSpan(365, 0, 0, 0);
                default: return EndTime - StartTime;
            }
        }

        void EnableNextPrev(bool bEnable)
        {
            btnPrev.IsEnabled = bEnable;
            btnNext.IsEnabled = bEnable;
            cmbEditTimeRange.IsEnabled = bEnable;
        }

        DateSpan GelectTimeRangeCombo()
        {
            return TimeFilterVM.SelectedItem.Tag;
        }

        void SelectTimeRangeCombo(DateSpan tag)
        {
            var item = (from c in TimeFilterVM.TimeFilterItems where c.Tag == tag select c).FirstOrDefault();
            if (item == null)
                return;

            TimeFilterVM.SelectedItem = item;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            var diff = GetSelectedTimeRangeCombo();
            EndTime = EndTime - diff;
            StartTime = StartTime - diff;
            FetchDatas();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            var diff = GetSelectedTimeRangeCombo();
            EndTime = EndTime + diff;
            StartTime = StartTime + diff;
            FetchDatas();
        }
        bool bUserInteractionSettings;
        private void All_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.All;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }
        private void Min_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Minute;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }

        private void Hour_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Hour;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }

        private void Day_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Day;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }

        private void Week_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Week;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }

        private void Month_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Month;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }

        private void Year_Click(object sender, RoutedEventArgs e)
        {
            UseEndTime = UseStartTime = true;
            bUserInteractionSettings = true;
            FilterType = DateSpan.Year;
            bUserInteractionSettings = false;
            SetTimeRange(FilterType);
            FetchDatas();
        }
        void HideAllHidden()
        {
            if (bDispose)
                return;
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
        private void InitDesignSeries()
        {

            int days = 15;
            //if (ControlViewMode == ViewMode.Grid)
            //    days = 1; 

            DataValueCollection data = LoadData(days);
            xSeries.DataSource = data;
            listView1.ItemsSource = data;

            UpdateAxisRange();
        }
        public DataValueCollection LoadData(int days)
        {
            var ret = new DataValueCollection();
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < days; i++)
            {
                ret.Add(new DataValue
                {
                    SourceTimestamp = DateTime.Now.AddDays(i),
                    Value = r.NextDouble() * 100
                });
            }
            return ret;
        }
        //#region Custom automation peers

        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new InvokeAutomationPeer(this);
        //}

        //public void ServerHistoryTrendInvokeAction()
        //{
        //    //TODO: handle some operations over this object
        //}
        //#endregion
        void UpdateAxisRange()
        {
            axisX.ActualWholeRange.SetAuto();
            axisX.ActualVisualRange.SetAuto();
            axisY.ActualWholeRange.SetAuto();
            axisY.ActualVisualRange.SetAuto();

            if (!AutomaticScale)
            {
                axisY.ActualWholeRange.MinValue = Minimum;
                axisY.ActualWholeRange.MaxValue = Maximum;
                axisY.ActualVisualRange.MinValue = Minimum;
                axisY.ActualVisualRange.MaxValue = Maximum;
            }
        }

        private void UpdateAxisTitles()
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                string _Title = Title;
                _Title = TranslationHelpers.TranslationHelper.TranslateComposedText(_Title, stringlist, _Title);

                title1.Content = _Title;
            });
        }

        string stringPlaceolder = "ServerHistory";
        private void StringManager_CultureChanged(object sender, EventArgs e)
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
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                paramPanel.Caption = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LegendTitle", stringlist, Properties.Resources.Options);
                bestFit.ToolTip = bestFit.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                btnShowLabels.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LabelTitle", stringlist, Properties.Resources.LabelTitle);
                btnExpand.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandTitle", stringlist, Properties.Resources.ExpandTitle);
                btnPrev.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevTitle", stringlist, Properties.Resources.PrevBuffer);
                btnNext.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextTitle", stringlist, Properties.Resources.NextBuffer);
                btnAll.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AllTitle", stringlist, Properties.Resources.AllTitle);
                btnMin.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MinTitle", stringlist, Properties.Resources.MinTitle);
                btnHour.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_HourTitle", stringlist, Properties.Resources.HourTitle);
                btnDay.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DayTitle", stringlist, Properties.Resources.DayTitle);
                btnWeek.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeekTitle", stringlist, Properties.Resources.WeekTitle);
                btnMonth.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_MonthTitle", stringlist, Properties.Resources.MonthTitle);
                btnYear.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_YearTitle", stringlist, Properties.Resources.YearTitle);
                configMemory.EditValue = ActualConfig;

                TimeFilterVM.TranslateContents(stringPlaceolder, stringlist);

                startText.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartTime", stringlist, Properties.Resources.StartTimeTitle);
                endText.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndTime", stringlist, Properties.Resources.EndTimeTitle);

                _ReadType.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReadType", stringlist, Properties.Resources.ReadType);
                _AggregationType.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AggregationType", stringlist, Properties.Resources.AggregationType);
                _ControlViewModel.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ControlViewModel", stringlist, Properties.Resources.ControlViewModel);

                PreviousBtn.ToolTip = Previous.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevBtn", stringlist, Properties.Resources.Previous);
                NextBtn.ToolTip = Next.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextBtn", stringlist, Properties.Resources.Next);
                FetchBtn.ToolTip = Fetch.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_FetchBtn", stringlist, Properties.Resources.Fetch);
                StopBtn.ToolTip = Stop.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StopBtn", stringlist, Properties.Resources.Stop);

                var lastTextPattern = TextPattern;
                TextPattern = "";
                TextPattern = lastTextPattern;

                lastTextPattern = AxisYTextPattern;
                AxisYTextPattern = "";
                AxisYTextPattern = lastTextPattern;

                lastTextPattern = axisX.Label.TextPattern;
                axisX.Label.TextPattern = "";
                axisX.Label.TextPattern = lastTextPattern;

                enStartTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_StartDate", stringlist, Properties.Resources.StartTime);
                enEndTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndDate", stringlist, Properties.Resources.EndTime);
                _maxReturnValues.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EndMaxReturnValues", stringlist, Properties.Resources.MaxReturnValues);

                TranslationHelper.TranlslateColumns(listView1.Columns, stringlist, stringPlaceolder);
                UpdateAxisTitles();

                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;
            });
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Hide();
        }

        HistoryReadViewModel serverHistoryTrend;
        MonitoredItemViewModel monitoredItemViewModel;
        internal string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                string relative = string.Format("{0}", (value).Replace(oldChars, ""));
                int index = relative.Split('/').Count() - 1;
                string finalpath = relative.Substring(0, (relative.Length - relative.Split('/')[index].Length));
                return finalpath;
            }

            return String.Empty;
        }

        private void InitServer()
        {
            if (monitoredItemViewModel == null)
                return;
            
            if(serverHistoryTrend != null)
                serverHistoryTrend.PropertyChanged -= serverHistoryTrend_PropertyChanged;

            try
            {
                serverHistoryTrend = monitoredItemViewModel.HistoryReadModel;
            }
            catch (Exception)
            {
                return;
            }

            if (serverHistoryTrend == null)
                return;

            var name = monitoredItemViewModel.Title;
            if (name.LastIndexOf('(') != -1)
                name = name.Remove(name.LastIndexOf('('));
            name = string.Format("{0}{1}", name, !string.IsNullOrEmpty(monitoredItemViewModel.EndpointUrl) ? string.Format("({0})", System.IO.Path.GetFileNameWithoutExtension(monitoredItemViewModel.EndpointUrl)) : string.Empty);

            xSeries.DisplayName = string.Format("{0}{1}", GetRelativePath(monitoredItemViewModel.RelativePath), name);
            //title1.Content = string.Format("{0}{1}", GetRelativePath(monitoredItemViewModel.RelativePath), name);

            //readType.ItemsSource = Enum.GetValues(typeof(ReadTypeDefinition)).Cast<ReadTypeDefinition>();
            //readType.SelectedItem = ReadTypeDefinition;
            //aggregationType.ItemsSource = Enum.GetValues(typeof(AggregationTypeDefinition)).Cast<AggregationTypeDefinition>();
            //aggregationType.SelectedItem = Aggregate;

            controlViewMode.ItemsSource = Enum.GetValues(typeof(ViewMode)).Cast<ViewMode>();
            controlViewMode.SelectedItem = ControlViewMode;

            serverHistoryTrend.ReadType = (HistoryReadViewModel.ReadTypeDefinition)ReadTypeDefinition;
            serverHistoryTrend.Aggregate = browsableNames[(int)Aggregate];

            UpdateSettings();

            serverHistoryTrend.PropertyChanged += serverHistoryTrend_PropertyChanged;

            RefreshDatas();

            //UpdateAxisRange();

            tableView.BestFitColumns();
        }

        void serverHistoryTrend_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Values")
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (bDispose)
                        return;

                    UseStartTime = serverHistoryTrend.UseStartTime;
                    StartTime = serverHistoryTrend.StartTime;
                    UseEndTime = serverHistoryTrend.UseEndTime;
                    EndTime = serverHistoryTrend.EndTime;


                    if (bWaitingForValues)
                    {
                        UpdateCache(Values);
                        currentIndex = historyCache.Count;
                    }

                    bWaitingForValues = false;
                    if (RunningOnServer)
                        UpdateValues(serverHistoryTrend.GetHistoryValues(ClientTimezoneOffset));
                    else
                        UpdateValues(serverHistoryTrend.Values);

                    UseMaxReturnValues = serverHistoryTrend.UseMaxReturnValues;
                    if (!serverHistoryTrend.UseMaxReturnValues)
                        MaxReturnValues = Values != null ? (uint)Values.Count : 0;
                });
            }
            else if (e.PropertyName == "LastMessage")
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (bDispose)
                        return;
                    string message = serverHistoryTrend.LastMessage;
                    if (!string.IsNullOrEmpty(message))
                    {
                        UIMsgBoxAlertService?.ShowError($"{Properties.Resources.Error}: {Properties.Resources.BadUnexpectedError}");
                        log.Error(message);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(Document, Properties.Resources.ServerHistoryTrendControl,
                           DateTime.UtcNow, message,
                           System.Diagnostics.EventLogEntryType.Error);
                    }
                });
            }
        }

        private void UpdateCache(DataValueCollection Values)
        {
            if (Values == null)
                return;

            historyCache.Add(Values);
            if (historyCache.Count > 0 && historyCache.Count > MaxHistoryCount)
            {
                historyCache.RemoveAt(0);
            }
        }

        private void InitBrowsableNames()
        {
            browsableNames.Add(BrowseNames.AggregateFunction_Interpolative);
            browsableNames.Add(BrowseNames.AggregateFunction_TimeAverage);
            browsableNames.Add(BrowseNames.AggregateFunction_Average);
            browsableNames.Add(BrowseNames.AggregateFunction_Count);
            browsableNames.Add(BrowseNames.AggregateFunction_Maximum);
            browsableNames.Add(BrowseNames.AggregateFunction_Minimum);
            //browsableNames.Add(BrowseNames.AggregateFunction_Total);
        }
        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
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
        #region Events
        //void chart_BoundDataChanged(object sender, RoutedEventArgs e)
        //{
        //    //Dispatcher.CurrentDispatcher.BeginInvokeInBackgroundIfRequired(() =>
        //    //{
        //    //    chart.Animate();
        //    //});
        //}
        private void aggregationType_SelectedIndexChanged_1(object sender, RoutedEventArgs e)
        {
            if (bDesignmode && !bSmartSettingsEditing || !bInit)
                return;

            if (sender != null)
            {
                Aggregate = (AggregationTypeDefinition)(sender as ComboBoxEdit).SelectedIndex;
                if (!bDesignmode)
                    RefreshDatas();
            }
        }

        private void RefreshDatas()
        {
            if (FetchdataOnScreenLoading)
                FetchDatas();
        }
        void FetchDatas()
        {
            if (IsEnableCallStopResults)
                CallStopResults();

            if (IsEnableCallRefreshCommand)
                CallRefreshCommand();
        }
        private void controlViewMode_SelectedIndexChanged_1(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                ControlViewMode = (ViewMode)((controlViewMode as ComboBoxEdit).SelectedIndex);
            }
        }

        private void readType_SelectedIndexChanged_1(object sender, RoutedEventArgs e)
        {
            if ((bDesignmode && !bSmartSettingsEditing) || !bInit)
                return;

            if (sender != null)
            {
                ReadTypeDefinition = (ReadTypeDefinition)(sender as ComboBoxEdit).SelectedIndex;
                if (!bDesignmode)
                    RefreshDatas();
            }
        }
        #endregion

        #region Dispose
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            if (oldMemoryList != null)
                oldMemoryList.Clear();

            if (MemorySettingList != null)
                MemorySettingList.Clear();

            if (helper != null)
                helper.Dispose();

            SizeChanged -= OnSizeChanged;

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            DetachOverrideBaseProperties();

            toolbar.MouseEnter -= toolbar_MouseEnter;
            toolbar.MouseLeave -= toolbar_MouseLeave;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            if (UseTouchKeyboard)
            {
                startTime.TouchDown -= OnTouchDown;
                endTime.TouchDown -= OnTouchDown;
                maxReturnValues.TouchDown -= OnTouchDown;
                startTime.LostFocus -= OnLostFocus;
                endTime.LostFocus -= OnLostFocus;
                maxReturnValues.LostFocus -= OnLostFocus;
            }

            if (serverHistoryTrend != null)
                serverHistoryTrend.PropertyChanged -= serverHistoryTrend_PropertyChanged;

            if (saveAutoHiddenStream != null)
                saveAutoHiddenStream.Dispose();

            Values.Clear();
            historyCache.Clear();
        }
        #endregion

        #region Commands

        RelayCommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                    _refreshCommand = new RelayCommand(param => CallRefreshCommand(), param => IsEnableCallRefreshCommand);

                return _refreshCommand;
            }
        }

        bool bCallingRefreshCommand;
        internal void CallRefreshCommand()
        {
            bCallingRefreshCommand = true; 
            try
            {
                UpdateSettings();
                historyCache.Clear();
                currentIndex = 0;
                bStartPrevSequence = true;
                serverHistoryTrend.GetResults.Execute(null);
            }
            finally
            {
                bCallingRefreshCommand = false;
            }
        }

        internal bool IsEnableCallRefreshCommand
        {
            get
            {
                if (bCallingRefreshCommand)
                    return false;
                else
                    return serverHistoryTrend != null && serverHistoryTrend.GetResults.CanExecute(null);
            }
        }



        RelayCommand _getNextResults;
        [Browsable(false)]
        public ICommand GetNextResults
        {
            get
            {
                if (_getNextResults == null)
                    _getNextResults = new RelayCommand(param => CallGetNextResults(), param => IsEnableCallGetNextResults);

                return _getNextResults;
            }
        }

        bool bCallingGetNextResults;
        bool bWaitingForValues;
        void UpdateSettings()
        {
            if (serverHistoryTrend == null)
                return;
            serverHistoryTrend.UseStartTime = UseStartTime;
            serverHistoryTrend.StartTime = StartTime;
            serverHistoryTrend.UseEndTime = UseEndTime;
            serverHistoryTrend.EndTime = EndTime;
            serverHistoryTrend.UseMaxReturnValues = UseMaxReturnValues;
            if (serverHistoryTrend.UseMaxReturnValues)
                serverHistoryTrend.MaxReturnValues = MaxReturnValues;
        }
        internal void CallGetNextResults()
        {
            bCallingGetNextResults = true;
            try
            {

                UpdateSettings();
                currentIndex += 1;
                if (currentIndex < historyCache.Count)
                {
                    if (historyCache.Count > 0)
                        UpdateValues(historyCache[currentIndex]);
                    else
                    {
                        try
                        {
                            bWaitingForValues = true;
                            bStartPrevSequence = true;
                            serverHistoryTrend.GetNextResults.Execute(null);
                        }
                        catch (ServiceResultException ex)
                        {
                            bWaitingForValues = false;
                            bStartPrevSequence = false;
                            if (UIMsgBoxAlertService != null)
                                UIMsgBoxAlertService.ShowError(String.Format("{0}: {1}", Properties.Resources.Error, ex.Message));
                        }
                    }
                }
                else
                {
                    try
                    {
                        bWaitingForValues = true;
                        bStartPrevSequence = true;
                        serverHistoryTrend.GetNextResults.Execute(null);
                    }
                    catch (ServiceResultException ex)
                    {
                        bWaitingForValues = false;
                        bStartPrevSequence = false;
                        if (UIMsgBoxAlertService != null)
                            UIMsgBoxAlertService.ShowError(String.Format("{0}: {1}", Properties.Resources.Error, ex.Message));
                    }
                }
            }
            finally
            {
                bCallingGetNextResults = false;
            }
        }


        private void UpdateValues(DataValueCollection dataValueCollection)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                try
                {
                    ChartMarkersVisible = dataValueCollection.Count > MaxLabelNumber ? false : MarkerVisible;
                    ChartShowLabels = dataValueCollection.Count > MaxLabelNumber ? false : ShowLabels;
                    btnShowLabels.IsEnabled = dataValueCollection.Count <= MaxLabelNumber;

                    if (ControlViewMode == ViewMode.Chart)
                    {
                        chart.BeginInit();
                        xSeries.DataSource = null;
                        Values.Clear();
                        Values.AddRange((DataValueCollection)dataValueCollection.Clone());
                        //UpdateAxisRange();
                        xSeries.DataSource = Values;
                        chart.EndInit();
                    }
                    else
                    {
                        listView1.BeginDataUpdate();
                        listView1.ItemsSource = null;
                        Values.Clear();
                        Values.AddRange((DataValueCollection)dataValueCollection.Clone());
                        listView1.ItemsSource = Values;
                        listView1.EndDataUpdate();
                    }
                }
                catch
                { }
            });
        }
        internal bool IsEnableCallGetNextResults
        {
            get
            {
                if (bCallingGetNextResults)
                    return false;
                else
                    return serverHistoryTrend != null && (currentIndex >= 0 && currentIndex < historyCache.Count || serverHistoryTrend.GetNextResults.CanExecute(null));
            }
        }

        RelayCommand _getPrevResults;
        public ICommand GetPrevResults
        {
            get
            {
                if (_getPrevResults == null)
                    _getPrevResults = new RelayCommand(param => CallGetPrevResults(), param => IsEnableCallGetPrevResults);

                return _getPrevResults;
            }
        }

        bool bCallingGetPrevResults;
        bool bStartPrevSequence;
        internal void CallGetPrevResults()
        {
            bWaitingForValues = false;
            if (currentIndex == 0)
                return;

            bCallingGetPrevResults = true;
            try
            {
                currentIndex -= 1;
                if (currentIndex >= 0)
                {
                    if (currentIndex == historyCache.Count - 1 && bStartPrevSequence)
                    {
	                    if (RunningOnServer)
	                        UpdateValues(serverHistoryTrend.GetHistoryValues(ClientTimezoneOffset));
	                    else
	                        UpdateValues(serverHistoryTrend.Values);
                        bStartPrevSequence = false;
                    }

                    if (currentIndex < historyCache.Count)
                        UpdateValues(historyCache[currentIndex]);
                }
            }
            finally
            {
                bCallingGetPrevResults = false;
            }
        }

        internal bool IsEnableCallGetPrevResults
        {
            get
            {
                if (bCallingGetPrevResults)
                    return false;
                else
                {
                    return serverHistoryTrend != null && currentIndex > 0 && historyCache.Count > 0;
                }
            }
        }


        RelayCommand _stopResults;
        public ICommand StopResults
        {
            get
            {
                if (_stopResults == null)
                    _stopResults = new RelayCommand(param => CallStopResults(), param => IsEnableCallStopResults);

                return _stopResults;
            }
        }

        bool bCallingStopResults;

        internal void CallStopResults()
        {
            try
            {
                bCallingStopResults = true;
                bStartPrevSequence = true;
                serverHistoryTrend.StopResults.Execute(null);
                historyCache.Clear();
                UpdateSettings();
                currentIndex = 0;
            }
            catch (ServiceResultException ex)
            {
                bStartPrevSequence = false;
                if (UIMsgBoxAlertService != null)
                    UIMsgBoxAlertService.ShowError(String.Format("{0}: {1}", Properties.Resources.Error, ex.Message));
            }
            finally
            {
                bCallingStopResults = false;
            }
        }

        internal bool IsEnableCallStopResults
        {
            get
            {
                if (bCallingStopResults)
                    return false;
                else
                    return serverHistoryTrend != null && serverHistoryTrend.StopResults.CanExecute(null);
            }
        }

        #endregion


        #region Isolated Storage

        internal String GetStorageName(bool useParent = false)
        {
            return StorageHelper.StorageHelper.GetStorageName(Document, this.Name, UserBasedRuntimeSettings ? helper.Username : null, true);
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
        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
        }
        //static String GetStoreFileNameLayout(String title)
        //{
        //    return String.Format("{0}.{1}Layout.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        //}
        #endregion

        #region TouchKeyboard
        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Show();
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


        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (workspace == null && Document != null)
                    workspace = Document.GetService(typeof(UFInterfaces.IWorkspace)) as UFInterfaces.IWorkspace;

                if (workspace != null)
                {
                    var dt1 = new DataTemplate();
                    var factory1 = new FrameworkElementFactory(typeof(TextPropertyEditor));
                    factory1.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                    dt1.DataType = typeof(String);
                    dt1.VisualTree = factory1;
                    mapDataTemplates.Add(TitleProperty, dt1);
                }

                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, 1.0);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.1);
                dt.DataType = typeof(Double);
                dt.VisualTree = factory;
                mapDataTemplates.Add(TransparencyProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, Document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }


        //[Browsable(false)]
        //public IDictionary<string, DataTemplate> GetListDataTemplates
        //{
        //    get
        //    {
        //        var mapDataTemplates = new Dictionary<string, DataTemplate>();

        //        if (workspace == null)
        //        {
        //            if (parent == null)
        //                parent = ScreenSettings.ScreenDocument.GetScreenDocument(this);
        //            if (parent != null)
        //                workspace = parent.GetService(typeof(IWorkspace)) as IWorkspace;
        //        }
        //        if (workspace != null)
        //        {
        //            var dt = new DataTemplate();
        //            var factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
        //            factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
        //            dt.DataType = typeof(String);
        //            dt.VisualTree = factory;
        //            mapDataTemplates.Add(TitleProperty.Name, dt);
        //        }

        //        return mapDataTemplates;
        //    }
        //}

        #endregion

        #region INotifyPropertyVisibilityChanged
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        public virtual bool this[string propertyName]
        {
            get
            {
                if (propertyName == "Aggregate")
                {
                    return ReadTypeDefinition == ReadTypeDefinition.Processed;
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
        protected virtual void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
        #endregion
        #region IStringIDAware

        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(Title))
                list.Add(Title);
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Title))
            {
                var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(ServerHistoryTrend), TitleProperty).DisplayName;
                map.Add(propertyName, Title);
            }
            return map;
        }
        #endregion
    }
}
