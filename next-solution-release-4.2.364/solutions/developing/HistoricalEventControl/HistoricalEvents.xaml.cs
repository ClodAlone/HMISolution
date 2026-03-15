using DevExpress.Xpo;
using System.IO.IsolatedStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Controls;
using ViewModelLib;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using Utilities;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Data.SqlTypes;
using StringManager.ComponentService;
using System.Globalization;
using DocumentManager.ComponentService;
using ScreenSettings;
using Converters;
using System.Windows.Data;
using UFUAHistorianModel;
using Ookii.Dialogs.Wpf;
using OPCUAViewModelService.ComponentService;
using log4net;
using PropertyControl.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using DevExpress.Xpf.Editors.Settings;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using StorageHelper;
using HelpProvider.ComponentService;
using DevExpress.Xpf.Grid;
using UFProjectManager.ComponentService;
using UFInterfaces;

namespace HistoricalEventControl
{
    /// <summary>
    /// Interaction logic for HistoricalEvents.xaml
    /// </summary>
    public partial class HistoricalEvents : UserControl, IContainPropertyEditors, IDisposable, ISettingsHelper, INotifyPropertyChanged, IConnectionAware
        , IGridLayoutUser
    {
        #region Dependency Properties
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }

        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(HistoricalEvents));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(HistoricalEvents));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
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
                var value = EventAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontFamily = FontFamily;
                measure.FontFamily = FontFamily;

                EventAreaFontSettings = value;
                HeaderFontSettings = measure; 
                
                
                
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
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
                var value = EventAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontWeight = FontWeight;
                measure.FontWeight = FontWeight;

                EventAreaFontSettings = value;
                HeaderFontSettings = measure;
                
                
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
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
                var value = EventAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontStyle = FontStyle;
                measure.FontStyle = FontStyle;

                EventAreaFontSettings = value;
                HeaderFontSettings = measure;
                
                
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
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
                var value = EventAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontSize = (int)FontSize;
                measure.FontSize = (int)FontSize;

                EventAreaFontSettings = value;
                HeaderFontSettings = measure;
                
                
            }
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                ControlForeground = Foreground;
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalEvents;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled)
                UpdateControlLayout();
        }
        private void UpdateControlLayout()
        {
            if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
            {
                (from c in (this as UIElement).GetVisualChildrenOfType<TextBox>()
                 select c).ToList().ForEach(child =>
                 {
                     child.Foreground = ControlForeground;
                 });
                (from c in (this as UIElement).GetVisualChildrenOfType<TextBlock>()
                 select c).ToList().ForEach(child =>
                 {
                     child.Foreground = ControlForeground;
                 });
                (from c in (this as UIElement).GetVisualChildrenOfType<Label>()
                 select c).ToList().ForEach(child =>
                 {
                     child.Foreground = ControlForeground;
                 });
                gridControl.Columns.ToList().ForEach(x =>
                {
                    x.HeaderStyle = gridControl.TryFindResource("columnStyle") as Style;
                });
                Style style = view.TryFindResource("ButtonRowCommandStyle") as Style;
                SetNewStyle(Button1, style);
                SetNewStyle(Button2, style);
                SetNewStyle(Button3, style);
                SetNewStyle(Button4, style);
                SetNewStyle(Button5, style);
                SetNewStyle(Button6, style);
                SetNewStyle(Button7, style);
                SetNewStyle(Button8, style);
                SetNewStyle(Button9, style);
            }
            else
            {
                Button1.Style = null;
                Button2.Style = null;
                Button3.Style = null;
                Button4.Style = null;
                Button5.Style = null;
                Button6.Style = null;
                Button7.Style = null;
                Button8.Style = null;
                Button9.Style = null;
                gridControl.Columns.ToList().ForEach(x =>
                {
                    x.HeaderStyle = null;
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

            //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    mainToolbar.Background = ToolbarBackground;
            //    toolbarSettings.Background = ToolbarBackground;
            //    bestFitbar.Background = ToolbarBackground;
            //}

            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    bestFit.Foreground = ToolbarForeground;
            //    Text1.Foreground = ToolbarForeground;
            //    Text2.Foreground = ToolbarForeground;
            //}
        }

        private void SetNewStyle(Button button, Style style)
        {
            button.Style = style;
            (from c in button.GetVisualChildrenOfType<TextBlock>()
            select c).ToList().ForEach(child =>
            {
                child.Background = Background;
            });
        }
        #endregion
        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(HistoricalEvents), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnControlForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceControlForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }
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

        #endregion

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(HistoricalEvents), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("HistoricalEventsOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(HistoricalEvents), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("HistoricalEventsOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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
        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(String), typeof(HistoricalEvents), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            HistoricalEvents dataAnalisys = o as HistoricalEvents;
            if (dataAnalisys != null)
                return dataAnalisys.OnCoerceConnectionString((String)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents dataAnalisys = o as HistoricalEvents;
            if (dataAnalisys != null)
                dataAnalisys.OnConnectionStringChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceConnectionString(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectionStringChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bInit)
                CreateDataSource();
        }
        [Category("HistoricalOptions")]
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
        #region FilterEventType
         public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("FilterEventType", typeof(Filters), typeof(HistoricalEvents), new UIPropertyMetadata(Filters.All, new PropertyChangedCallback(OnFilterChanged), new CoerceValueCallback(OnCoerceFilter)));

        private static object OnCoerceFilter(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceFilter((Filters)value);
            else
                return value;
        }

        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnFilterChanged((Filters)e.OldValue, (Filters)e.NewValue);
        }

        protected virtual Filters OnCoerceFilter(Filters value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilterChanged(Filters oldValue, Filters newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && bInit)
            {
                CreateDataSource();
                Combo.SelectedItem = (from ComboBoxItem c in Combo.Items where c.Tag.ToString() == newValue.ToString() select c).FirstOrDefault();
            }
        }

        [Category("HistoricalOptions")]
        [Browsable(true)]
        public Filters FilterEventType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Filters)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }
        #endregion
        #region MaxRows
        public static readonly DependencyProperty MaxRowsProperty = DependencyProperty.Register("MaxRows", typeof(Double), typeof(HistoricalEvents), new UIPropertyMetadata((Double)1000, new PropertyChangedCallback(OnMaxRowsChanged), new CoerceValueCallback(OnCoerceMaxRows)));

        private static object OnCoerceMaxRows(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceMaxRows((Double)value);
            else
                return value;
        }

        private static void OnMaxRowsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnMaxRowsChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMaxRows(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxRowsChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && bInit)
                CreateDataSource();
        }



        [Category("HistoricalOptions")]
        [Browsable(true)]
        public Double MaxRows
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MaxRowsProperty);
            }
            set
            {
                SetValue(MaxRowsProperty, value);
            }
        }
        #endregion
        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(HistoricalEvents), new UIPropertyMetadata(0));
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

        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(HistoricalEvents), new UIPropertyMetadata(DateSpan.Day, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
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
        }
        [Category("HistoricalEventsOptions")]
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
 
        #region DateTimeStart
        public static readonly DependencyProperty DateTimeStartProperty = DependencyProperty.Register("DateTimeStart", typeof(DateTime), typeof(HistoricalEvents), new UIPropertyMetadata(new DateTime(2014,1,1), new PropertyChangedCallback(OnDateTimeStartChanged), new CoerceValueCallback(OnCoerceDateTimeStart)));

        private static object OnCoerceDateTimeStart(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceDateTimeStart((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeStartChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnDateTimeStartChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual DateTime OnCoerceDateTimeStart(DateTime value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDateTimeStartChanged(DateTime oldValue, DateTime newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("HistoricalOptions")]
        [Browsable(true)]
        public DateTime DateTimeStart
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateTime)GetValue(DateTimeStartProperty);
            }
            set
            {
                SetValue(DateTimeStartProperty, value);
            }
        }
        #endregion

        #region DateTimeEnd
        public static readonly DependencyProperty DateTimeEndProperty = DependencyProperty.Register("DateTimeEnd", typeof(DateTime), typeof(HistoricalEvents), new UIPropertyMetadata(new DateTime(2099,12,31), new PropertyChangedCallback(OnDateTimeEndChanged), new CoerceValueCallback(OnCoerceDateTimeEnd)));

        private static object OnCoerceDateTimeEnd(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceDateTimeEnd((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeEndChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnDateTimeEndChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual DateTime OnCoerceDateTimeEnd(DateTime value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDateTimeEndChanged(DateTime oldValue, DateTime newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("HistoricalOptions")]
        [Browsable(true)]
        public DateTime DateTimeEnd
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateTime)GetValue(DateTimeEndProperty);
            }
            set
            {
                SetValue(DateTimeEndProperty, value);
            }
        }
        #endregion

        #region EventAreaFontSettings
        public static readonly DependencyProperty EventAreaFontSettingsProperty = DependencyProperty.Register("EventAreaFontSettings", typeof(FontSettings), typeof(HistoricalEvents), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnEventAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceEventAreaFontSettings)));

        private static object OnCoerceEventAreaFontSettings(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceEventAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnEventAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnEventAreaFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceEventAreaFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEventAreaFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);
            
            
        }

        [Category("Advanced")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings EventAreaFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(EventAreaFontSettingsProperty);
            }
            set
            {
                SetValue(EventAreaFontSettingsProperty, value);
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

        #endregion

        #region HeaderFontSettings
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(HistoricalEvents), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnHeaderFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceHeaderFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //{
            //    //tableView.BestFitColumns();
            //});
        }

        [Category("Advanced")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings HeaderFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(HeaderFontSettingsProperty);
            }
            set
            {
                SetValue(HeaderFontSettingsProperty, value);
            }
        }

        #endregion

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(HistoricalEvents), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents historicalEvents = o as HistoricalEvents;
            if (historicalEvents != null)
                historicalEvents.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
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

        void LoadDesignGridLayout(bool bDenyLoad = false)
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

        #endregion


        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(HistoricalEvents), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(HistoricalEvents), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
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

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            HistoricalEvents HistoricalEvents = o as HistoricalEvents;
            if (HistoricalEvents != null)
                return HistoricalEvents.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents HistoricalEvents = o as HistoricalEvents;
            if (HistoricalEvents != null)
                HistoricalEvents.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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


        #region ShowBestFitButton
        public static readonly DependencyProperty ShowBestFitButtonProperty = DependencyProperty.Register("ShowBestFitButton", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowBestFitButtonChanged), new CoerceValueCallback(OnCoerceShowBestFitButton)));

        private static object OnCoerceShowBestFitButton(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceShowBestFitButton((bool)value);
            else
                return value;
        }

        private static void OnShowBestFitButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnShowBestFitButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowBestFitButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowBestFitButtonChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateVisibility();
        }

        public bool ShowBestFitButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowBestFitButtonProperty);
            }
            set
            {
                SetValue(ShowBestFitButtonProperty, value);
            }
        }

        #endregion


        #region ShowGroupPanel
        public static readonly DependencyProperty ShowGroupPanelProperty = DependencyProperty.Register("ShowGroupPanel", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowGroupPanelChanged), new CoerceValueCallback(OnCoerceShowGroupPanel)));

        private static object OnCoerceShowGroupPanel(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceShowGroupPanel((bool)value);
            else
                return value;
        }

        private static void OnShowGroupPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnShowGroupPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowGroupPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowGroupPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateVisibility();
        }

        public bool ShowGroupPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowGroupPanelProperty);
            }
            set
            {
                SetValue(ShowGroupPanelProperty, value);
            }
        }

        #endregion

        #region ShowSearchPanel
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnShowSearchPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSearchPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSearchPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateVisibility();
        }
     
        public bool ShowSearchPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSearchPanelProperty);
            }
            set
            {
                SetValue(ShowSearchPanelProperty, value);
            }
        }

        #endregion

        #region ShowFilterPanel
        public static readonly DependencyProperty ShowFilterPanelProperty = DependencyProperty.Register("ShowFilterPanel", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFilterPanelChanged), new CoerceValueCallback(OnCoerceShowFilterPanel)));

        private static object OnCoerceShowFilterPanel(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceShowFilterPanel((bool)value);
            else
                return value;
        }

        private static void OnShowFilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnShowFilterPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowFilterPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFilterPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateVisibility();
        }
    
        public bool ShowFilterPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowFilterPanelProperty);
            }
            set
            {
                SetValue(ShowFilterPanelProperty, value);
            }
        }

        #endregion

        #region ShowCommandButtons
        public static readonly DependencyProperty ShowCommandButtonsProperty = DependencyProperty.Register("ShowCommandButtons", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCommandButtonsChanged), new CoerceValueCallback(OnCoerceShowCommandButtons)));

        private static object OnCoerceShowCommandButtons(DependencyObject o, object value)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                return control.OnCoerceShowCommandButtons((bool)value);
            else
                return value;
        }

        private static void OnShowCommandButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalEvents control = o as HistoricalEvents;
            if (control != null)
                control.OnShowCommandButtonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowCommandButtons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCommandButtonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateVisibility();
        }
  
        public bool ShowCommandButtons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowCommandButtonsProperty);
            }
            set
            {
                SetValue(ShowCommandButtonsProperty, value);
            }
        }

        #endregion

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        [Browsable(false)]
        double ClientTimezoneOffset
        {
            get
            {
                try
                {
                    if (!RunningOnServer)
                        return 0.0;
                    return (double)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
                }
                catch
                {
                    return 0.0;
                }
            }
        }
        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(HistoricalEvents));
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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(HistoricalEvents), new UIPropertyMetadata(false));

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
        IDataLayer dl;
        UnitOfWork ufw;
        bool bLoaded;
        bool bDesignmode;
        IDocument parent;
        internal bool bSmartSettingsEditing;
        IDocument Document;
        IStringEditorManager stringManager;
        IUFProjectManager iUFProjectManager;
        Setting defSetting;
        string resetGridLayout;
        IDictionary<String, String> stringlist;
        public IDictionary<string, string> StringList
        {
            get
            {
                return stringlist;
            }
            set
            {
                if (stringlist != value)
                {
                    stringlist = value;
                    OnPropertyChanged("StringList");
                }
            }
        }

        DateTime _maxDate;
        DateTime MaxDate
        {
            get
            {
                if (_maxDate == null)
                    _maxDate = new DateTime(2099, 12, 31);
                return _maxDate;
            }
        }
        DateTime _minDate;
        DateTime MinDate
        {
            get
            {
                if (_minDate == null)
                    _minDate = new DateTime(2014, 1, 1);
                return _minDate;
            }
        }

        SafeObservableCollection<UFUAAuditLogItem> emptyList = new SafeObservableCollection<UFUAAuditLogItem>();
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.HistoricalEventsControlLog);
        SafeObservableCollection<UFUAAuditLogItem> UFUAAuditLogItemList;
        CollectionViewSource collectionView;
        public ICollectionView CollectionView
        {
            get
            {
                if (UFUAAuditLogItemList == null)
                    return EmptyCollectionView;
                if (collectionView == null)
                    collectionView = new CollectionViewSource { Source = UFUAAuditLogItemList };

                if (collectionView.Source == null)
                    return EmptyCollectionView;
                return collectionView.View;
            }
        }

        public ICollectionView EmptyCollectionView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(emptyList);
            }
        }

        Helper helper;
        #region ActualConfig
        string actualConfig = GridLayoutHelper.DesignSettingName;
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
        MemorySettings MemorySettingList;
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
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
                                    optionsCsv.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                                    tableView.ExportToCsv(sw, optionsCsv);
                                    break;
                                case ExportFileType.Html:
                                    tableView.ExportToHtml(sw);
                                    break;
                                case ExportFileType.Xls:
                                    var optionsXls = new DevExpress.XtraPrinting.XlsExportOptionsEx();
                                    optionsXls.ExportType = DevExpress.Export.ExportType.WYSIWYG;
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
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            }//);
        }
        IUIMsgBoxAlertService IUIMsgBoxAlertService;
     
        private void ShowError(Exception exception)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.InnerException != null ? exception.InnerException.Message : exception.Message);
                log.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.HistoricalEventsControlLog,
                      DateTime.UtcNow, $"{error}",
                      System.Diagnostics.EventLogEntryType.Error);

                errorInfo.Text = string.Empty;

                if (Document != null)
                {
                    if (IUIMsgBoxAlertService == null)
                        IUIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (IUIMsgBoxAlertService != null)
                        IUIMsgBoxAlertService.ShowError(error);
                    else
                        errorInfo.Text = error;
                }
                else
                    errorInfo.Text = error;
            });
        }
        private void errorInfo_ClearMessage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            errorInfo.Text = string.Empty;
        }
 
        void UnloadDataLayer()
        {
            if (ufw != null)
            {
                ufw.Disconnect();
                ufw.Dispose();
                ufw = null;
            }
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
        }

        void CreateDataLayer(string connection, int commandTimeout)
        {
            if (ufw != null || String.IsNullOrEmpty(connection))
                return;

            try
            {
                
                dl = UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditDataLog>(connection, commandTimeout);
                ufw = new UnitOfWork(dl);
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }
                
        DateSpan lastFilter = DateSpan.Day;
        void CreateDataSource(object param = null)
        {
            if (String.IsNullOrEmpty(ConnectionString) || bRefreshing || bDispose)
                return;

            bRefreshing = true;

            var types = FilterTypes.FilterType[FilterEventType];
            var maxr = MaxRows;

            if (param != null)
            {
                switch ((DateSpan)param)
                {
                    case DateSpan.All:
                        SetAllTimeSpan();
                        break;
                    case DateSpan.Minute:
                        SetMinuteTimeSpan();
                        break;
                    case DateSpan.Hour:
                        SetHourTimeSpan();
                        break;
                    case DateSpan.Day:
                        SetDayTimeSpan();
                        break;
                    case DateSpan.Week:
                        SetWeekTimeSpan();
                        break;
                    case DateSpan.Month:
                        SetMonthTimeSpan();
                        break;
                    case DateSpan.Year:
                        SetYearTimeSpan();
                        break;
                    default:
                        break;
                };

                lastFilter = (DateSpan)param;
            }
            //else
            //    SetAllTimeSpan();



            var utcStart = DateTimeStart.ToUniversalTime();
            var utcEnd = DateTimeEnd.ToUniversalTime();
            bool runningOnServer = RunningOnServer;
            double clientTimezoneOffset = ClientTimezoneOffset;
            if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                utcStart = (DateTime)SqlDateTime.MinValue;
            if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                utcEnd = (DateTime)SqlDateTime.MaxValue;
            var connection = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
            SetBusy(true);
            var task1 = Task.Factory.StartNew((commandTimeout) =>
            {
                List<UFUAAuditLogItem> retlist = new List<UFUAAuditLogItem>();
                if (ufw == null)
                    CreateDataLayer(connection, (int)commandTimeout);

                if (ufw == null)
                    return retlist;

                if (utcStart == utcEnd)
                    retlist.AddRange((from entry in new XPQuery<UFUAAuditLogItem>(ufw)//.AsParallel()
                                      where types.Contains(entry.EventType)
                                      orderby entry.EventDateTimeUtc descending
                                      select entry).Take((int)maxr).ToList());
                else
                    retlist.AddRange((from entry in new XPQuery<UFUAAuditLogItem>(ufw)//.AsParallel()
                                      where types.Contains(entry.EventType) &&
                                           ((entry.EventDateTimeUtc >= utcStart) && (entry.EventDateTimeUtc <= utcEnd))
                                      orderby entry.EventDateTimeUtc descending
                                      select entry).Take((int)maxr).ToList());

                if (StringList != null || runningOnServer)
                    retlist.ForEach(entry =>
                    {
                        entry.EventDateTime = runningOnServer ? entry.EventDateTimeUtc.AddMinutes(clientTimezoneOffset) : entry.EventDateTime;
                    });

                if (maxr <= retlist.Count())
                    return retlist.Take((int)maxr).ToList();

                else
                    return retlist;
            }, CommandTimeout);
            var task2 = task1.ContinueWith(ret =>
            {
                SetBusy(false);
                if (ret.Exception != null)
                {
                    ShowError(ret.Exception);
                }
                else
                {
                    if (UFUAAuditLogItemList == null)
                        UFUAAuditLogItemList = new SafeObservableCollection<UFUAAuditLogItem>();
                    UFUAAuditLogItemList.Clear();
                    ret.Result.ForEach((item) => UFUAAuditLogItemList.Add(item));

                    gridControl.BeginDataUpdate();
                    gridControl.ItemsSource = CollectionView;

                    var filter = gridControl.FilterCriteria;
                    gridControl.FilterCriteria = null;
                    gridControl.FilterCriteria = filter;
                    gridControl.EndDataUpdate();

                    gridControl.View.MoveFirstRow();
                }
                bRefreshing = false;
                //gridControl.ItemsSource = null;
                //gridControl.ItemsSource = ret.Result;
                CommandManager.InvalidateRequerySuggested();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetAllTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.All);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetYearTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Year);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetMonthTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Month);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetWeekTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Week);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetDayTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Day);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetHourTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Hour);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetMinuteTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Minute);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }


        bool CanRefreshData()
        {
            return !bRefreshing;
        }

        #endregion

        #region Commands
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
            return dl != null;
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

        RelayCommand refreshDataCommand;
        public ICommand RefreshDataCommand
        {
            get
            {
                if (refreshDataCommand == null)
                {
                    refreshDataCommand = new RelayCommand(
                        param => OnRefreshData(param),
                        param => CanRefreshData()
                        );
                }
                return refreshDataCommand;
            }
        }
        bool bRefreshing;
        private void OnRefreshData(object param)
        {
            if (bDispose)
                return;
            if (param != null)
                try
                {
                    CreateDataSource((Filters)(System.Convert.ToInt16(param)));
                }
                catch
                {
                }
            else
                CreateDataSource();
        }
        #endregion

        #region Contructor
        bool bInit;
        public HistoricalEvents()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////
            // devexpress optimized mode implementation
            // (see https://www.devexpress.com/Support/Center/Question/Details/T147586)
            ///////////////////////////////////////////////////////////////////////////////////////////
           // tableView.UseLightweightTemplates = UseLightweightTemplates.None;

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = mainToolbar.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    if (RunningOnServer)
                    {
                        mainToolbar.Bars.Clear();
                        mainToolbar.Bars.Add(toolBarControl);
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif

                    gridControl.DataContext = this;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // temporary code for fixing the devexpress bug
                    // (see http://www.devexpress.com/Support/Center/Question/Details/Q445390)
                    //gridControl.ManipulationDelta += (obj, ev) =>
                    //{
                    //    ev.Handled = true;
                    //};
                    ///////////////////////////////////////////////////////////////////////////////////////////

                    cmbExportType.ItemsSource = Enum.GetValues(typeof(ExportFileType)).Cast<ExportFileType>();
                    cmbExportType.SelectedIndex = 0;
                    UpdateControlLayout(); 
                    if (parent == null)
                        parent = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    configMemory.EditValue = GridLayoutHelper.DesignSettingName;
                    configMemory.DataContext = MemorySettingList?.Names;

                    CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;

                    bDesignmode = bDesignmode || DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;
                    lastFilter = FilterType;
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
                        gridControl.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;
                        gridControl.BeginDataUpdate();
                        gridControl.ItemsSource = EmptyCollectionView;
                        gridControl.EndDataUpdate();

                        OverrideBaseProperties();
                        toolbarSettings.IsEnabled = false;

                        if (!bSmartSettingsEditing)
                            view.IsHitTestVisible = false;
                    }
                    else
                    {
                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();
                        startTime.TouchDown += OnTouchDown;
                        startTime.LostFocus += OnLostFocus;
                        endTime.TouchDown += OnTouchDown;
                        endTime.LostFocus += OnLostFocus;
                        SetTimeSpan(FilterType);

                        OverrideBaseProperties();

                        CreateDataSource(DateSpan.Day);
                        gridControl.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

                        if (RunningOnServer)
                        {
                            Button9.Visibility = cmbExportType.Visibility = System.Windows.Visibility.Collapsed;
                            toolbarSettings.IsVisible = false;
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }


                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();

                        defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
                        configMemory.DataContext = MemorySettingList?.Names;
                        configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            try
                            {
                                LoadRuntimeLayout(GetStorageName());
                                GetItem(ActualConfig);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    Combo.SelectedItem = (from ComboBoxItem c in Combo.Items where c.Tag.ToString() == FilterEventType.ToString() select c).FirstOrDefault();
                    bInit = true;
                    if (!bDesignmode && !bControlLoaded)
                    {
                        bControlLoaded = true;
                        OnControlLoaded();
                    }

                    UpdateVisibility();
                }
            };
        }

        internal void SetTimeSpan(DateSpan span)
        {
            switch (span)
            {
                case DateSpan.All:
                    SetAllTimeSpan();
                    break;
                case DateSpan.Minute:
                    SetMinuteTimeSpan();
                    break;
                case DateSpan.Hour:
                    SetHourTimeSpan();
                    break;
                case DateSpan.Day:
                    SetDayTimeSpan();
                    break;
                case DateSpan.Week:
                    SetWeekTimeSpan();
                    break;
                case DateSpan.Month:
                    SetMonthTimeSpan();
                    break;
                case DateSpan.Year:
                    SetYearTimeSpan();
                    break;
                default:
                    break;
            };
        }

        internal string stringPlaceolder = Properties.Settings.Default.HistoricalEventsPlaceHolder;
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    StringList = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                else
                    StringList = null;

                TranslationHelpers.TranslationHelper.TranlslateColumns(gridControl.Columns, StringList, stringPlaceolder);

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", StringList, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", StringList, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", StringList, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", StringList, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", StringList, Properties.Resources.BestFit);
                configMemory.EditValue = ActualConfig;

                startTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeStart", StringList, Properties.Resources.HistoricalEvents_DateTimeStart);
                endTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeEnd", StringList, Properties.Resources.HistoricalEvents_DateTimeEnd);

                Button1.Content = Button1.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", StringList, Properties.Resources.HistoricalEvents_Minute);
                Button2.Content = Button2.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", StringList, Properties.Resources.HistoricalEvents_Hour);
                Button3.Content = Button3.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", StringList, Properties.Resources.HistoricalEvents_Day);
                Button4.Content = Button4.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", StringList, Properties.Resources.HistoricalEvents_Week);
                Button5.Content = Button5.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", StringList, Properties.Resources.HistoricalEvents_Month);
                Button6.Content = Button6.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", StringList, Properties.Resources.HistoricalEvents_Year);
                Button7.Content = Button7.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_All", StringList, Properties.Resources.HistoricalEvents_All);

                Button8.Content = Button8.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshDataCommand", StringList, Properties.Resources.HistoricalEvents_RefreshDataCommand);
                Button9.ToolTip = txtExport.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", StringList, Properties.Resources.ExportDataCommand);

                Col7.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EventMessage", StringList, Properties.Resources.HistoricalEvents_EventMessageColumnName);
                Col9.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EventState", StringList, Properties.Resources.HistoricalEvents_EventStateColumnName);

                foreach (ComboBoxItem item in Combo.Items)
                {
                    switch (item.Tag.ToString())
                    {
                        case "All":
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ComboAll", StringList, Properties.Resources.ComboAll);
                            break;
                        case "Alarms":
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ComboAlarms", StringList, Properties.Resources.ComboAlarms);
                            break;
                        case "Drivers":
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ComboDrivers", StringList, Properties.Resources.ComboDrivers);
                            break;
                        case "System":
                            item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ComboSystem", StringList, Properties.Resources.ComboSystem);
                            break;
                    }
                }

                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;

                if (!bDesignmode && RefreshDataCommand.CanExecute(null))
                    OnRefreshData(lastFilter);
            });
        }
        #endregion

        #region Methods
        void UpdateVisibility()
        {
            commands.Visibility = ShowCommandButtons ? Visibility.Visible : Visibility.Collapsed;
            bestFit.IsVisible = ShowBestFitButton;
            tableView.ShowFilterPanelMode = ShowFilterPanel && !RunningOnServer ? ShowFilterPanelMode.ShowAlways : ShowFilterPanelMode.Never;
            tableView.ShowSearchPanelMode = ShowSearchPanel && !RunningOnServer ? ShowSearchPanelMode.Always : ShowSearchPanelMode.Never;
            tableView.ShowGroupPanel = ShowGroupPanel && !RunningOnServer;
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy)
            {
//                busyContent.Text = Properties.Resources.WaitText;
                busyContent.Text = Properties.Resources.HistoricalEvents_WaitText;
                busyControl.Visibility = Visibility.Visible;
                busyContent.Visibility = Visibility.Visible;
            }
            else
            {
                busyControl.Visibility = Visibility.Collapsed;
                busyContent.Visibility = Visibility.Collapsed;
            }
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Hide();
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Show();
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
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                IUIMsgBoxAlertService uiInterface = null;
                IHelpProvider helpProvider = null;
                IWorkspace workspace = null;
                if (Document != null)
                {
                    uiInterface = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
                }

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, uiInterface);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
                factory.SetValue(ConnectionSourcePropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

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

        #region IDisposable

        private bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            if (oldMemoryList != null)
                oldMemoryList.Clear();
            oldMemoryList = null;
            if (MemorySettingList != null)
                MemorySettingList.Clear();
            MemorySettingList = null;

            if (helper is IDisposable)
                (helper as IDisposable).Dispose();
            helper = null;

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            gridControl.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

            gridControl.BeginDataUpdate();
            gridControl.ItemsSource = EmptyCollectionView;
            gridControl.EndDataUpdate();

            if (UFUAAuditLogItemList != null)
            {
                UFUAAuditLogItemList.Clear();
                UFUAAuditLogItemList = null;
            }


            startTime.TouchDown -= OnTouchDown;
            startTime.LostFocus -= OnLostFocus;
            endTime.TouchDown -= OnTouchDown;
            endTime.LostFocus -= OnLostFocus;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            UnloadDataLayer();
            DetachOverrideBaseProperties();

            emptyList = null;
        }
        #endregion
        #region Isolated Storage

        internal String GetStorageName()
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
        #endregion

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.BestFitColumns();
        }

        #region EditSettings
        bool bUserInteractionSettings;
        private void GetItem(string itemName)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (!string.IsNullOrEmpty(setting?.GridLayout))
                {
                    GridLayout = setting.GridLayout;
                    ActualConfig = setting.Name;
                }
                else
                {
                    GridLayout = defSetting.GridLayout;
                    ActualConfig = GridLayoutHelper.DesignSettingName;
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
            MemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
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
        #region EventHandlers
        public event EventHandler ControlLoaded;
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
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
            }
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
            string configname = configMemory.EditValue as string;
            string defaultTagSetting = GridLayoutHelper.DesignSettingName;

            if (configname == defaultTagSetting)
                return;

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
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.GridLayout = GridLayout;
                selected.ReadOnly = false;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }

            if (StorageHelper.StorageHelper.SaveMemoryMap<MemorySettings>(MemorySettingList, Document, Name, UserBasedRuntimeSettings ? helper.Username : null))
            {
                if (oldMemoryList != null)
                    oldMemoryList.Clear();
                oldMemoryList = null;
                oldConfigName = null;
            }
            ActualConfig = configname;
            SaveRuntimeLayout(GetStorageName());
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
                if (string.IsNullOrEmpty(configMemory.EditValue as string) || bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand)
                    return false;
                else
                    return configMemory.EditValue as string != GridLayoutHelper.DesignSettingName;
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
                    EnsureDefaultValue(false);
                if (!LoadRuntimeLayout(GetStorageName()))
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                GetItem(ActualConfig);
            });
        }

        void EnsureDefaultValue(bool bSetCombo = true)
        {
            defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
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
        #endregion

        public void Initialize()
        {
        }
        #endregion
        bool bIsUpdating;
        private void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (bIsUpdating)
                return;

            try
            {
                FilterEventType = (Filters)Enum.Parse(typeof(Filters),(Combo.SelectedItem as ComboBoxItem).Tag.ToString());
            }
            catch (Exception)
            {
            }
        }
        #region IConnectionAware
        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
        }
        #endregion
        
        #region IGridLayoutUser
        public List<StorageColumn> GetColumns()
        {
            return (from column in gridControl.Columns
                    let fieldName = column.FieldName.StartsWith("Unbound") ? column.FieldName.Remove(0, "Unbound".Length) : column.FieldName
                    select new StorageColumn()
                    {
                        FieldName = fieldName,
                        ActualWidth = column.ActualWidth,
                        ColumnTag = column.Tag?.ToString(),
                        Visible = column.Visible,
                        VisibleIndex = column.VisibleIndex,
                        ColumnOrder = column.SortOrder,
                        SortIndex = column.SortIndex
                    }).ToList();
        }
        #endregion
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            return new Dictionary<string, Brush>();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is HistoricalEvents)
            {
                HistoricalEvents control = sender as HistoricalEvents;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(HistoricalEvents.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);
                if (control.ReadLocalValue(HistoricalEvents.ControlForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlForeground", control.ControlForeground);
                else
                    ret.Add("ControlForeground", foreground);

                if (control.ReadLocalValue(HistoricalEvents.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            HistoricalEvents control = sender as HistoricalEvents;
            string prop = ((DependencyProperty)property)?.Name;
            if (prop.Equals(HistoricalEvents.ToolbarForegroundProperty.Name))
            {
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                return foreground;
            }
            else if (prop.Equals(HistoricalEvents.ToolbarBackgroundProperty.Name))
            {
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
