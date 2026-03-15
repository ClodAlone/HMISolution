using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpo;
using System.ComponentModel;
using ViewModelLib;
using System.IO.IsolatedStorage;
using System.IO;
using System.Reflection;
using System.Data.SqlTypes;
using CommonControls.PropertyDataTemplate;
using UFInterfaces.PropertyControl;
using ScreenSettings;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using Utilities;
using DataloggerViewerControl.PropertyDataTemplate;
using System.Data;
using OPCUAViewModelService.ComponentService;
using log4net;
using DevExpress.Xpf.Grid;
using Converters;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using Ookii.Dialogs.Wpf;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using UIMsgBoxAlertService.ComponentService;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Media;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using DataLoggerModel.Helpers;
using System.Globalization;
using System.Windows.Data;
using System.Xml.Serialization;
using DevExpress.Export;
using DataloggerViewerControl.Converters;
using System.Threading;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Bars.Themes;
using HelpProvider.ComponentService;
using DevExpress.Mvvm.Native;
using UFProjectManager.ComponentService;
using StorageHelper;
using System.Windows.Threading;

namespace DataloggerViewerControl
{
    /// <summary>
    /// Interaction logic for DataloggerViewer.xaml
    /// </summary>
    public partial class DataloggerViewer : UserControl, IContainPropertyEditors, IDisposable, ISettingsHelper, IConnectionAware
        , IGridLayoutUser
    {
        #region DP

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        DevExpress.Xpf.Bars.BarEditItem DataloggerCombo
        {
            get
            {
                return RunningOnServer ? comboDataloggerList_web : comboDataloggerList;
            }
        }
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(DataloggerViewer));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(DataloggerViewer));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
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
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontFamily = FontFamily;
                measure.FontFamily = FontFamily;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
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
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontWeight = FontWeight;
                measure.FontWeight = FontWeight;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
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
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontStyle = FontStyle;
                measure.FontStyle = FontStyle;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
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
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontSize = (int)FontSize;
                measure.FontSize = (int)FontSize;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateControlLayout();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as DataloggerViewer;
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
            if (dpUpdateLayout == null ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Completed ||
                dpUpdateLayout.Status == DispatcherOperationStatus.Aborted)
            {
                dpUpdateLayout = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                { 
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
                });
            }
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
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(DataloggerViewer), new UIPropertyMetadata(Brushes.LightGray));

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

        #region RowAreaFontSettings
        public static readonly DependencyProperty RowAreaFontSettingsProperty = DependencyProperty.Register("RowAreaFontSettings", typeof(FontSettings), typeof(DataloggerViewer), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnRowAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceRowAreaFontSettings)));

        private static object OnCoerceRowAreaFontSettings(DependencyObject o, object value)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceRowAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnRowAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
            if (historicalEvents != null)
                historicalEvents.OnRowAreaFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceRowAreaFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowAreaFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);
            
            //{
            
            //}
        }

        [Category("Advanced")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings RowAreaFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(RowAreaFontSettingsProperty);
            }
            set
            {
                SetValue(RowAreaFontSettingsProperty, value);
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
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(DataloggerViewer), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
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

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(DataloggerViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("DataloggerViewerOptions")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(DataloggerViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("DataloggerViewerOptions")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
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

        #region ConnectionString
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(DataloggerViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceConnectionString((string)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                control.OnConnectionStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceConnectionString(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectionStringChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(!DesignerProperties.GetIsInDesignMode(this) && bLoaded == true && !bDesignmode && bDInit)
                CreateDataSource();
        }
        [Category("DataLoggerViewerOptions")]
        public string ConnectionString
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
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

        #region CommandTimeout
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(DataloggerViewer), new UIPropertyMetadata(30));
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

        #region DataLoggerName
        public static readonly DependencyProperty DataLoggerNameProperty = DependencyProperty.Register("DataLoggerName", typeof(String), typeof(DataloggerViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDataLoggerNameChanged), new CoerceValueCallback(OnCoerceDataLoggerName)));

        private static object OnCoerceDataLoggerName(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceDataLoggerName((String)value);
            else
                return value;
        }

        private static void OnDataLoggerNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                control.OnDataLoggerNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceDataLoggerName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDataLoggerNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (bCInit)
            {
                if (bDesignmode)
                {
                    UpdateCombo(newValue);
                    InitValues();
                    CreateColumns();
                    CreateDataSource(null, true);
                    SaveDesignGridLayout();
                }
                else
                {
                    UpdateCombo(newValue);
                    InitValues();
                    CreateColumns();
                    CreateDataSource(reloadLayout: bUserInteractionSettings);
                    SaveDesignGridLayout();
                }
            }
        }
        bool isEdinting;
        [Category("DataLoggerViewerOptions")]
        public String DataLoggerName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(DataLoggerNameProperty);
            }
            set
            {
                SetValue(DataLoggerNameProperty, value);
            }
        }

        #endregion


        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(DataloggerViewer), new UIPropertyMetadata(DateSpan.Day, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        [Category("DataLoggerViewerOptions")]
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
        public static readonly DependencyProperty DateTimeStartProperty = DependencyProperty.Register("DateTimeStart", typeof(DateTime), typeof(DataloggerViewer), new UIPropertyMetadata(new DateTime(2014, 1, 1), new PropertyChangedCallback(OnDateTimeStartChanged), new CoerceValueCallback(OnCoerceDateTimeStart)));

        private static object OnCoerceDateTimeStart(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceDateTimeStart((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeStartChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                control.OnDateTimeStartChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
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
        [Category("DataLoggerViewerOptions")]
        [Browsable(false)]
        [XmlIgnore]
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
        public static readonly DependencyProperty DateTimeEndProperty = DependencyProperty.Register("DateTimeEnd", typeof(DateTime), typeof(DataloggerViewer), new UIPropertyMetadata(new DateTime(2099, 12, 31), new PropertyChangedCallback(OnDateTimeEndChanged), new CoerceValueCallback(OnCoerceDateTimeEnd)));

        private static object OnCoerceDateTimeEnd(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceDateTimeEnd((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeEndChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                control.OnDateTimeEndChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
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
        [Category("DataLoggerViewerOptions")]
        [Browsable(false)]
        [XmlIgnore]
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


        #region MaxRows
        public static readonly DependencyProperty MaxRowsProperty = DependencyProperty.Register("MaxRows", typeof(Double), typeof(DataloggerViewer), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxRowsChanged), new CoerceValueCallback(OnCoerceMaxRows)));

        private static object OnCoerceMaxRows(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceMaxRows((Double)value);
            else
                return value;
        }

        private static void OnMaxRowsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                control.OnMaxRowsChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMaxRows(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxRowsChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("DataLoggerViewerOptions")]
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
        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(DataloggerViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer historicalEvents = o as DataloggerViewer;
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
            UpdateColumns();
            UpdateVisibility();
        }

        internal void ResetGridLayout()
        {
            if (!string.IsNullOrEmpty(resetGridLayout))
                GridLayout = resetGridLayout;
        }

        internal void SaveResetGridLayout()
        {
            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
            {
                using (MemoryStream output = new MemoryStream())
                {


                    gridControl.SaveLayoutToStream(output);
                    var utf8NoBom = new UTF8Encoding(true);
                    resetGridLayout = utf8NoBom.GetString(output.ToArray());

                }
            }
            else
                resetGridLayout = GridLayout;
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
                            if (!bCallingSaveCommand)
                                gridControl.RestoreLayoutFromStream(output);
                            else
                            {
                                gridControl.AutoGenerateColumns = DevExpress.Xpf.Grid.AutoGenerateColumnsMode.None;
                                gridControl.RestoreLayoutFromStream(output);
                                gridControl.AutoGenerateColumns = DevExpress.Xpf.Grid.AutoGenerateColumnsMode.AddNew;
                            }
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
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(DataloggerViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(DataloggerViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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


        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(DataloggerViewer));
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

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            DataloggerViewer DataloggerViewer = o as DataloggerViewer;
            if (DataloggerViewer != null)
                return DataloggerViewer.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer DataloggerViewer = o as DataloggerViewer;
            if (DataloggerViewer != null)
                DataloggerViewer.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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
        public static readonly DependencyProperty ShowBestFitButtonProperty = DependencyProperty.Register("ShowBestFitButton", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowBestFitButtonChanged), new CoerceValueCallback(OnCoerceShowBestFitButton)));

        private static object OnCoerceShowBestFitButton(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceShowBestFitButton((bool)value);
            else
                return value;
        }

        private static void OnShowBestFitButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        public static readonly DependencyProperty ShowGroupPanelProperty = DependencyProperty.Register("ShowGroupPanel", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowGroupPanelChanged), new CoerceValueCallback(OnCoerceShowGroupPanel)));

        private static object OnCoerceShowGroupPanel(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceShowGroupPanel((bool)value);
            else
                return value;
        }

        private static void OnShowGroupPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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

        #region FilterCriteria
        [Category("Advanced")]
        [Browsable(false)]
        [SvgValueConverter(typeof(FilterCriteriaConverter))]
        public string FilterCriteria
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                //return (string)GetValue(FilterCriteriaProperty);
                if (!ReferenceEquals(gridControl.FilterCriteria, null))
                    return gridControl.FilterCriteria.ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (value != FilterCriteria)
                    FilterCriteria = value;
            }
        }
        #endregion

        #region OriginalFilterCriteria
        [Browsable(false)]
        public string OriginalFilterCriteria
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                //return (string)GetValue(FilterCriteriaProperty);
                if (!ReferenceEquals(gridControl.FilterCriteria, null))
                    return gridControl.FilterCriteria.ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (value != OriginalFilterCriteria)
                    OriginalFilterCriteria = value;
            }
        }
        #endregion

        #region ShowFilterPanel
        public static readonly DependencyProperty ShowFilterPanelProperty = DependencyProperty.Register("ShowFilterPanel", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFilterPanelChanged), new CoerceValueCallback(OnCoerceShowFilterPanel)));

        private static object OnCoerceShowFilterPanel(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceShowFilterPanel((bool)value);
            else
                return value;
        }

        private static void OnShowFilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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
        public static readonly DependencyProperty ShowCommandButtonsProperty = DependencyProperty.Register("ShowCommandButtons", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCommandButtonsChanged), new CoerceValueCallback(OnCoerceShowCommandButtons)));

        private static object OnCoerceShowCommandButtons(DependencyObject o, object value)
        {
            DataloggerViewer control = o as DataloggerViewer;
            if (control != null)
                return control.OnCoerceShowCommandButtons((bool)value);
            else
                return value;
        }

        private static void OnShowCommandButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer control = o as DataloggerViewer;
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

        #region RefreshTimeout
        public static readonly DependencyProperty RefreshTimeoutProperty = DependencyProperty.Register("RefreshTimeout", typeof(int), typeof(DataloggerViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnRefreshTimeoutChanged), new CoerceValueCallback(OnCoerceRefreshTimeout)));

        private static object OnCoerceRefreshTimeout(DependencyObject o, object value)
        {
            DataloggerViewer DataloggerViewer = o as DataloggerViewer;
            if (DataloggerViewer != null)
                return DataloggerViewer.OnCoerceRefreshTimeout((int)value);
            else
                return value;
        }

        private static void OnRefreshTimeoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DataloggerViewer DataloggerViewer = o as DataloggerViewer;
            if (DataloggerViewer != null)
                DataloggerViewer.OnRefreshTimeoutChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRefreshTimeout(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRefreshTimeoutChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int RefreshTimeout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(RefreshTimeoutProperty);
            }
            set
            {
                SetValue(RefreshTimeoutProperty, value);
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

        #region ColumnTextWrap
        public static readonly DependencyProperty ColumnTextWrapProperty = DependencyProperty.Register("ColumnTextWrap", typeof(TextWrapping), typeof(DataloggerViewer), new UIPropertyMetadata(TextWrapping.NoWrap));

        [Browsable(false)]
        [XmlIgnore]
        public TextWrapping ColumnTextWrap
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextWrapping)GetValue(ColumnTextWrapProperty);
            }
            set
            {
                SetValue(ColumnTextWrapProperty, value);
            }
        }
        #endregion
        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(DataloggerViewer), new UIPropertyMetadata(false));

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

        private class ColumnData
        {
            public string name;
            public DevExpress.Utils.DefaultBoolean allowMoving = DevExpress.Utils.DefaultBoolean.True;
            public DevExpress.Utils.DefaultBoolean allowGrouping = DevExpress.Utils.DefaultBoolean.True;
            public DevExpress.Utils.DefaultBoolean allowColumnFiltering = DevExpress.Utils.DefaultBoolean.True;
            public DevExpress.Utils.DefaultBoolean allowSearchPanel = DevExpress.Utils.DefaultBoolean.True;
            public bool allowColumnFilteringAtTrue = true;
            public Type type = typeof(string);
        }
        #endregion
        #region Declarations
        CancellationTokenSource cts;
        IDataLayer dl;
        UnitOfWork ufw;
        internal bool bSmartSettingsEditing;
        //DateTime MaxDate = new DateTime(2099, 12, 31);
        //DateTime MinDate = new DateTime(2014, 1, 1);
        DateTime _maxDate;
        DateTime MaxDate
        {
            get
            {
                if(_maxDate == null)
                    _maxDate = new DateTime(2099, 12, 31);
                return _maxDate;
            }
        }
        DateTime _minDate;
        DateTime MinDate
        {
            get
            {
                if(_minDate == null)
                    _minDate = new DateTime(2014, 1, 1);
                return _minDate;
            }
        }

        String DatalogerConnection;
        internal IDocument Document;
        IStringEditorManager stringManager;
        internal IDictionary<String, String> stringlist;
        IUFUAEditorManager ufuaEditorService;
        IOPCUAViewModelService OPCUAViewModelService;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IUFProjectManager iUFProjectManager;
        InMemoryDataStore InMemory;
        bool bLoaded;
        bool bDesignmode;
        String defaultDataProvider;
        String defaultConnectionString;
        internal DataLoggerSettings currentSettings;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.DataLoggerControlLog);
        DataSet gridDataSet = new DataSet();
        Setting defaultsetting;
        string oldusername;
        string resetGridLayout;
        string designDataLoggerName;
        Helper helper;
        QualityIdentifierToStringConverter qualityToStringConverter;
        bool bTranslateColumnsOnInit;

        DispatcherOperation dpUpdateLayout;

        #region ActualConfig
        string actualConfig = GridLayoutHelper.DesignSettingName;
        List<string> qualityColumnNames = new List<string>();
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
        MemorySettings MemorySettingList;
        #endregion

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
                                    try
                                    {
                                        optionsCsv.CustomizeCell += options_CustomizeCell;
                                        tableView.ExportToCsv(sw, optionsCsv);
                                    }
                                    finally
                                    {
                                        optionsCsv.CustomizeCell -= options_CustomizeCell;
                                    }
                                    break;
                                case ExportFileType.Html:

                                    System.IO.MemoryStream l_HtmlLayoutMemoryStream = new MemoryStream();
                                    try
                                    {
                                        gridControl.SaveLayoutToStream(l_HtmlLayoutMemoryStream);
                                        l_HtmlLayoutMemoryStream.Seek(0, SeekOrigin.Begin);
                                        foreach (GridColumn col in gridControl.Columns)
                                        {
                                            if (col.PrintColumnHeaderStyle == null)
                                                col.PrintColumnHeaderStyle = this.FindResource("printColumnHeaderStyle") as Style;
                                        }

                                        if (ColumnTextWrap == TextWrapping.Wrap)
                                            tableView.PrintAutoWidth = true;
                                        else 
                                            tableView.PrintAutoWidth = false;

                                        tableView.BestFitColumns();
                                        tableView.ExportToHtml(sw);
                                        gridControl.RestoreLayoutFromStream(l_HtmlLayoutMemoryStream);
                                    }
                                    finally
                                    {
                                        l_HtmlLayoutMemoryStream?.Dispose();
                                    }
                                    break;
                                case ExportFileType.Xls:
                                    var optionsXls = new DevExpress.XtraPrinting.XlsExportOptionsEx();
                                    try
                                    {
                                        optionsXls.CustomizeCell += options_CustomizeCell;
                                        tableView.ExportToXls(sw, optionsXls);
                                    }
                                    finally
                                    {
                                        optionsXls.CustomizeCell -= options_CustomizeCell;
                                    }
                                    break;
                                case ExportFileType.Pdf:
                                    System.IO.MemoryStream l_MemoryStream = new MemoryStream();
                                    try
                                    {
                                        gridControl.SaveLayoutToStream(l_MemoryStream);
                                        l_MemoryStream.Seek(0, SeekOrigin.Begin);

                                        foreach (GridColumn col in gridControl.Columns)
                                        {
                                            if (col.PrintColumnHeaderStyle == null)
                                                col.PrintColumnHeaderStyle = this.FindResource("printColumnHeaderStyle") as Style;
                                        }

                                        tableView.PrintAutoWidth = true;
                                        tableView.BestFitColumns();
                                        tableView.ExportToPdf(sw);
                                        gridControl.RestoreLayoutFromStream(l_MemoryStream);
                                    }
                                    finally
                                    {
                                        l_MemoryStream?.Dispose();
                                    }
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
        void options_CustomizeCell(CustomizeCellEventArgs e)
        {
            var ex = e as CustomizeCellEventArgsExtended;
            if (ex != null && ex.Value is DateTime && ex.Column.FormatSettings.FormatString == "G")
            {
                var conv = new UTCToLocalTimeConverter();
                e.Value = conv.Convert(ex.Value, typeof(DateTime), true, CurrentCulture);
                e.Handled = true;
            }
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

        void InitConnection()
        {
            var settings = DatalogerConnection;
            if (String.IsNullOrEmpty(settings))
                settings = XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
            if (String.IsNullOrEmpty(settings))
                settings = ufuaEditorService.GetHistorianDefaultConnection(Document);

            if (!String.IsNullOrEmpty(settings))
            {
                if (Document is ScreenDocument)
                    settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(settings, (Document as ScreenDocument).SessionString);
                var helper = new ConnectionStringParser(settings);
                var providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType))
                {
                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                }
                else
                {
                    defaultDataProvider = helper.GetPartByName("DataProvider");
                    helper.RemovePartByName("DataProvider");
                    defaultConnectionString = helper.GetConnectionString();
                }
            }
        }
        private void errorInfo_ClearMessage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            errorInfo.Text = string.Empty;
        }
        void CreateDataSource(object param = null, bool bOnlySchema = false, bool reloadLayout = false)
        {
            if (!bDInit || bRefreshing)
                return;

            Task task2 = null;
            try
            {
                bRefreshing = true;
                InitConnection();

                if (currentSettings == null || string.IsNullOrEmpty(defaultConnectionString) || string.IsNullOrEmpty(defaultDataProvider))
                    return;
                SetBusy(true);

                var maxr = MaxRows;
                var _defaultDataProvider = defaultDataProvider;
                var _defaultConnectionString = defaultConnectionString;

                if (param != null)
                {
                    SetTimeSpan((DateSpan)Enum.Parse(typeof(DateSpan), param.ToString(), true));
                }
                //else
                //    SetAllTimeSpan();



                var utcStart = DateTimeStart.ToUniversalTime();
                var utcEnd = DateTimeEnd.ToUniversalTime();

                if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                    utcStart = (DateTime)SqlDateTime.MinValue;
                if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                    utcEnd = (DateTime)SqlDateTime.MaxValue;

                string _tablename = currentSettings.TableName;
                string _utccolumnname = currentSettings.UtcTimeColumnName;

                DataTable retTable = null;
                var task1 = Task.Factory.StartNew( (commandTimeout) =>
                {

                    using (var connection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);
                            DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                            dbdapater.SelectCommand.Connection = connection;
                            dbdapater.SelectCommand.CommandTimeout = (int)commandTimeout;

                            StringBuilder commantText = new StringBuilder("SELECT ");
                            if (maxr > 0 && dbSchemaInfo.IsSupportedTopKeyword)
                                commantText.AppendFormat(" TOP {0} ", maxr);

                            commantText.AppendFormat(" * FROM {0}", dbSchemaInfo.WrapObjectName(_tablename));

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

                            commantText.AppendFormat(" WHERE {0} >= {1} AND {2} <= {3}  ORDER BY {4} DESC",
                                dbSchemaInfo.WrapObjectName(_utccolumnname),
                                datestart.ParameterName,
                                dbSchemaInfo.WrapObjectName(_utccolumnname),
                                dateend.ParameterName,
                                dbSchemaInfo.WrapObjectName(_utccolumnname));

                            dbdapater.SelectCommand.CommandText = commantText.ToString();

                            gridDataSet = new DataSet();
                            dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                            if (dbSchemaInfo.IsSupportedTopKeyword)
                            {
                                if (!bOnlySchema)
                                    dbdapater.Fill(gridDataSet, _tablename);
                                retTable = gridDataSet.Tables[0];
                            }
                            else if (!bOnlySchema)
                                retTable = DataReader.DataReader.DataTableFromDataSet(gridDataSet, dbdapater, (int)maxr);

                            commantText = null;
                        }
                        finally
                        {
                            connection.Close();
                        }
                        return retTable;
                    }
                }, CommandTimeout);
                task2 = task1.ContinueWith(ret =>
                {
                    try
                    {
                        if (ret.IsFaulted && ret.Exception != null)
                            ShowError(ret.Exception.InnerException, true);
                        else
                        {
                            errorInfo.Text = string.Empty;

                            gridControl.ItemsSource = retTable.Copy().AsDataView();
                            gridControl.RefreshData();

                            if(reloadLayout)
                            {
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
                        }
                    }
                    finally
                    {
                        UpdateColumns();
                        SetBusy(false);
                        bRefreshing = false;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                if (task2 == null)
                    bRefreshing = false;
            }
        }
        private void ShowError(Exception exception, bool bForceErrorInfo = false)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.Message);
                log.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DataLoggerControlLog,
                DateTime.UtcNow, $"{error}: {exception.StackTrace}", System.Diagnostics.EventLogEntryType.Error);

                errorInfo.Text = string.Empty;

                if (Document != null && !bForceErrorInfo)
                {
                    if (UIMsgBoxAlertService == null)
                        UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (UIMsgBoxAlertService != null)
                        UIMsgBoxAlertService.ShowError(error);
                    else
                        errorInfo.Text = error;
                }
                else
                    errorInfo.Text = error;
            });
        }
        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.Ignore;

            if (Document != null)
            {

                if (UIMsgBoxAlertService == null)
                    UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                if (UIMsgBoxAlertService != null)
                {
                    var result = UIMsgBoxAlertService.ShowYesNo(
                        String.Format(Properties.Resources.InvalidRowException.Replace("'newline'", Environment.NewLine), e.ErrorText),
                        CustomDialogIcons.Question);
                    if (result != CustomDialogResults.Yes)
                        e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.NoAction;
                }
                else
                    errorInfo.Text = e.ErrorText;
            }
            else
                errorInfo.Text = e.ErrorText;
        }


        internal void  SetTimeSpan(DateSpan span)
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

        private void InitLocalization()
        {
            Button1.Content = Properties.Resources.HistoricalEvents_Minute;
            Button2.Content = Properties.Resources.HistoricalEvents_Hour;
            Button3.Content = Properties.Resources.HistoricalEvents_Day;
            Button4.Content = Properties.Resources.HistoricalEvents_Week;
            Button5.Content = Properties.Resources.HistoricalEvents_Month;
            Button6.Content = Properties.Resources.HistoricalEvents_Year;
            Button7.Content = Properties.Resources.HistoricalEvents_All;
            Button8.Content = Properties.Resources.HistoricalEvents_RefreshDataCommand;
            txtExport.Text = Properties.Resources.HistoricalEvents_ExportDataCommand;
            startTime.Content = Properties.Resources.HistoricalEvents_DateTimeStart;
            endTime.Content = Properties.Resources.HistoricalEvents_DateTimeEnd;
            comboDataloggerListTitle.Content = Properties.Resources.HistoricalEvents_DataloggerList;
        }

        public void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            if (RefreshDataCommand.CanExecute(null))
                CreateDataSource(7);
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
        #region Isolated Storage
        private const string defaultSettings = "defaultsettings";
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
            return gridControl != null && (gridControl.ItemsSource as DataView) != null ? (gridControl.ItemsSource as DataView).Count > 0 : false;
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

        bool CanRefreshData()
        {
            return !bRefreshing;
        }

        bool bRefreshing;
        private void OnRefreshData(object param = null)
        {
            if (param != null)
                try
                {
                    CreateDataSource((System.Convert.ToInt16(param)));
                }
                catch
                {
                }
            else
                CreateDataSource();
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

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                IHelpProvider helpProvider = null;
                if (Document != null)
                {
                    helpProvider = Document.GetService(typeof(IHelpProvider)) as IHelpProvider;
                    if (UIMsgBoxAlertService == null)
                        UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                }
                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ConnectionSourcePropertyEditor));
                factory.SetValue(ConnectionSourcePropertyEditor.UIMsgBoxAlertServiceProperty, UIMsgBoxAlertService);
                factory.SetValue(ConnectionSourcePropertyEditor.HelpProviderProperty, helpProvider);
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ConnectionStringProperty, dt);

                var dt1 = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DataloggerSettingsPropertyEditor));
                dt1.DataType = typeof(string);
                dt1.VisualTree = factory;
                mapDataTemplates.Add(DataLoggerNameProperty, dt1);

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

            if (cts != null)
                cts.Cancel();
            if (oldMemoryList != null)
                oldMemoryList.Clear();
            oldMemoryList = null;
            if (!bDesignmode)
            {
                if (bControlLoaded && GridLayoutHelper.DesignSettingName == ActualConfig && !Editable && UserBasedRuntimeSettings)
                    SaveGridConfiguration();
            }
            if (MemorySettingList != null)
                MemorySettingList.Clear();
            MemorySettingList = null;
            if (helper is IDisposable)
                (helper as IDisposable).Dispose();
            helper = null;

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            startTime.TouchDown -= OnTouchDown;
            startTime.LostFocus -= OnLostFocus;
            endTime.TouchDown -= OnTouchDown;
            endTime.LostFocus -= OnLostFocus;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            UnloadDataLayer();
            DetachOverrideBaseProperties();

            if (dpUpdateLayout != null &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateLayout.Status != DispatcherOperationStatus.Completed)
                dpUpdateLayout.Abort();

            gridDataSet = null;
        }
        #endregion
        #region Constructor
        bool bInit;
        bool bDInit;
        bool bCInit;
        bool bColumnsGenerated;
        public DataloggerViewer()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////
            // devexpress optimized mode implementation
            // (see https://www.devexpress.com/Support/Center/Question/Details/T147586)
            ///////////////////////////////////////////////////////////////////////////////////////////
            //tableView.UseLightweightTemplates = UseLightweightTemplates.None;

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
                        comboDataloggerList_web.IsVisible = true;
                        comboDataloggerList.IsVisible = false;
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    if (Document != null)
                    {
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        if (ufuaEditorService == null)
                            ufuaEditorService = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    }

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

                    chkMultiLine.IsChecked = false;

                    configMemory.EditValue = GridLayoutHelper.DesignSettingName;
                    configMemory.DataContext = MemorySettingList?.Names;
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
                        InitValues();
                        CreateColumns();
                        LoadDesignGridLayout();
                        OverrideBaseProperties();
                        if (bSmartSettingsEditing)
                            CreateDataSource();

                        toolbar.IsEnabled = false;
                        bCInit = true;

                        EventArgs m = new EventArgs();
                        OnInit(m);
                    }
                    else
                    {
                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();
                        oldusername = helper.Username;
                        qualityToStringConverter = new Converters.QualityIdentifierToStringConverter();

                        startTime.TouchDown += OnTouchDown;
                        startTime.LostFocus += OnLostFocus;
                        endTime.TouchDown += OnTouchDown;
                        endTime.LostFocus += OnLostFocus;
                        SetTimeSpan(FilterType);
                        InitValues();
                        CreateColumns();
                        LoadDesignGridLayout();
                        OverrideBaseProperties();

                        if(RunningOnServer)
                        {
                            Button9.Visibility = cmbExportType.Visibility = chkMultiLine.Visibility = System.Windows.Visibility.Collapsed;
                            tableView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                            toolbarSettings.IsVisible = false;
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }

                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();

                        InitDesign();
                        configMemory.DataContext = MemorySettingList?.Names;
                        configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;

                        bCInit = true;

                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            try
                            {
                                LoadRuntimeLayout(GetStorageName());
                                GetItem(ActualConfig, true);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                            CreateDataSource();
                    }

                    InitDLRItemsSource();

                    UpdateVisibility();

                    ColumnTextWrap = TextWrapping.NoWrap;
                }
            };
        }

        private void InitDLRItemsSource()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            var task1 = Task.Factory.StartNew(() =>
            {
                IList<string> dataloggerItems = null;
                if (ufuaEditorService != null)
                {
                    token.ThrowIfCancellationRequested();
                    dataloggerItems = ufuaEditorService.GetDataLoggerSettingsNameList(Document);
                }

                return dataloggerItems;
            }, token);
            task1.ContinueWith(ret =>
            {
                cts.Dispose();
                cts = null;

                if (token.IsCancellationRequested)
                    return;
                DataloggerCombo.DataContext = ret.Result;

                UpdateCombo(DataLoggerName);

                bInit = true;

                if (bTranslateColumnsOnInit)
                {
                    bTranslateColumnsOnInit = false;
                    UpdateColumns();
                }
                if (!bDesignmode && !bControlLoaded)
                {
                    bControlLoaded = true;
                    OnControlLoaded();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void InitDesign()
        {
            string actualgridlayout = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username)?.FirstOrDefault(s => s.Name == GridLayoutHelper.DesignSettingName)?.GridLayout;

            if (actualgridlayout != null)
                GridLayout = actualgridlayout;

            designDataLoggerName = DataLoggerName;
            string defaultTagSetting = GridLayoutHelper.DesignSettingName;
            try
            {
                MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper?.Username : null);
                defaultsetting = (from m in MemorySettingList where m.Name.Equals(defaultTagSetting) select m).FirstOrDefault();
                if (defaultsetting == null)
                    MemorySettingList.Add(new Setting() { Name = defaultTagSetting, DataLoggerName = designDataLoggerName, GridLayout = GridLayout, ReadOnly = true });
                else
                {
                    designDataLoggerName = DataLoggerName;
                    defaultsetting.ReadOnly = true;
                    defaultsetting.GridLayout = GridLayout;
                    defaultsetting.DataLoggerName = designDataLoggerName;
                }
            }
            catch (Exception ex)
            {
                MemorySettingList = new MemorySettings();
                defaultsetting = new Setting() { Name = defaultTagSetting, DataLoggerName = designDataLoggerName, GridLayout = GridLayout, ReadOnly = true };
                MemorySettingList.Add(defaultsetting);
            }
        }

        private void UpdateCombo(string dataLoggerName)
        {
            if (isEdinting || DataloggerCombo.DataContext == null)
                return;
            isEdinting = true;
            try
            {
                List<string> list = DataloggerCombo.DataContext as List<string>;
                if (list.Contains(dataLoggerName))
                    DataloggerCombo.EditValue = dataLoggerName;
                else
                    DataloggerCombo.EditValue = list.First();
            }
            finally
            {
                isEdinting = false;
            }
        }

        string stringPlaceolder = "DataloggerViewer";
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

                if (qualityToStringConverter != null && gridDataSet.Tables.Count > 0)
                {
                    qualityToStringConverter.CurrentStringList = stringlist;
                    DataView view = new DataView(gridDataSet.Tables[0]);
                    view.BeginInit();
                    gridControl.BeginDataUpdate();
                    (from DataColumn col in view.Table.Columns where qualityColumnNames.Contains(col.ColumnName) select col).ToList().ForEach((col) =>
                    {
                        SetQualityCellTemplate(col.ColumnName);
                    });
                    gridControl.EndDataUpdate();
                    view.EndInit();
                }

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                startTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeStart", stringlist, Properties.Resources.HistoricalEvents_DateTimeStart);
                endTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeEnd", stringlist, Properties.Resources.HistoricalEvents_DateTimeEnd);
                DataloggerCombo.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DataloggerList", stringlist, Properties.Resources.HistoricalEvents_DataloggerList);
                comboDataloggerListTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DataloggerList", stringlist, Properties.Resources.HistoricalEvents_DataloggerList);


                configMemory.EditValue = ActualConfig;
                Button1.Content = Button1.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Minute", stringlist, Properties.Resources.HistoricalEvents_Minute);
                Button2.Content = Button2.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Hour", stringlist, Properties.Resources.HistoricalEvents_Hour);
                Button3.Content = Button3.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Day", stringlist, Properties.Resources.HistoricalEvents_Day);
                Button4.Content = Button4.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Week", stringlist, Properties.Resources.HistoricalEvents_Week);
                Button5.Content = Button5.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Month", stringlist, Properties.Resources.HistoricalEvents_Month);
                Button6.Content = Button6.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Year", stringlist, Properties.Resources.HistoricalEvents_Year);
                Button7.Content = Button7.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_All", stringlist, Properties.Resources.HistoricalEvents_All);

                Button8.Content = Button8.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshDataCommand", stringlist, Properties.Resources.RefreshDataCommand);
                Button9.ToolTip = txtExport.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);

                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;

                if (bInit)
                    UpdateColumns();
                else
                    bTranslateColumnsOnInit = true;
            });
        }

        void UpdateColumns()
        {
            if (stringManager == null)
                return;
            
            TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);
        }
        internal event EventHandler Init;
        private void OnInit(EventArgs e)
        {
            EventHandler temp = Init;
            if (temp != null)
                temp(null, e);
        }
        private void AddColumns(GridControl gridControl, List<ColumnData> cdl)
        {
            gridControl.Columns.BeginUpdate();
            foreach (ColumnData cd in cdl)
            {
                GridColumn column = new GridColumn();
                column.Header = column.FieldName = cd.name;
                //column.Name = cd.name;
                //if (cd.name_delwhitespaces)
                //{
                    column.Name = new string(cd.name.Where(c => !Char.IsWhiteSpace(c)).ToArray());
                //}
                column.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                column.AllowResizing = DevExpress.Utils.DefaultBoolean.True;
                column.ReadOnly = true;
                column.BestFitMaxRowCount = 1;
                if (cd.allowColumnFilteringAtTrue)
                    column.AllowColumnFiltering = DevExpress.Utils.DefaultBoolean.True;
                column.PrintColumnHeaderStyle = this.FindResource("printColumnHeaderStyle") as Style;
                gridControl.Columns.Add(column);
            }
            gridControl.Columns.EndUpdate();
        }
        List<ColumnData> newColumnsList;
        private void CreateColumns()
        {
            gridControl.ItemsSource = null;
            gridControl.Columns.Clear();
            newColumnsList = new List<ColumnData>();
            DevExpress.Utils.DefaultBoolean bRunMode = bDesignmode ? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.True;
            Type defType = typeof(string);
            if (currentSettings?.Name.Length > 0 && dlt != null)
            {
                newColumnsList.AddRange(
                    new ColumnData[]{
                        new ColumnData()
                        {
                            name = dlt.UtcTimeColumnName,
                            type = Type.GetType(dlt.UtcTimeColumnType) ?? defType,
                            allowGrouping = bRunMode,
                            allowMoving = bRunMode,
                            allowSearchPanel = bRunMode
                        },
                        new ColumnData()
                        {
                            name = dlt.LocalTimeColumnName,
                            type = Type.GetType(dlt.LocalTimeColumnType) ?? defType,
                            allowGrouping = bRunMode,
                            allowMoving = bRunMode,
                            allowSearchPanel = bRunMode
                        },
                        new ColumnData()
                        {
                            name = dlt.MillisecondsColumnName,
                            type = Type.GetType(dlt.MillisecondsColumnType) ?? defType,
                            allowGrouping = bRunMode,
                            allowMoving = bRunMode,
                            allowSearchPanel = bRunMode
                        },
                        new ColumnData()
                        {
                            name = dlt.UserColumnName,
                            type = Type.GetType(dlt.UserColumnType) ?? defType,
                            allowColumnFilteringAtTrue=false
                        },
                        new ColumnData()
                        {
                            name = dlt.ReasonColumnName,
                            type = Type.GetType(dlt.ReasonColumnType) ?? defType,
                        }
                    }
                );

                if (currentSettings.Columns?.Count > 0)
                {
                    foreach (DataLoggerColumn _column in currentSettings.Columns)
                    {
                        Type t = (from DataColumn c in dlt.TableStructure.Columns where c.ColumnName == _column.Name select c.DataType).FirstOrDefault();
                        newColumnsList.Add(new ColumnData()
                        {
                            name = _column.Name,
                            type = t ?? defType
                        });
                    }
                }
                AddColumns(gridControl, newColumnsList);
            }

            if (bRunMode == DevExpress.Utils.DefaultBoolean.True)
                UpdateColumns();
        }


        IUFUAEditorManager UFUAEditor;
        DataLoggerTable dlt;
        private void InitValues()
        {
            if (Document == null)
                Document = ScreenDocument.GetScreenDocument(this);

            if (Document == null)
                return;

            if (UFUAEditor == null)
                UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor != null)
            {
                string lcurrentSettingsXml = UFUAEditor.GetDataLoggerDataTable(Document, DataLoggerName, inExecution: true);
                if (lcurrentSettingsXml != null)
                {
                    dlt = lcurrentSettingsXml.FromXml<DataLoggerTable>();
                    if (dlt != null)
                    {
                        qualityColumnNames = dlt.StatusCodeColumnNamesList;
                        currentSettings = new DataLoggerSettings()
                        {
                            Name = dlt.Name,
                            TableName = dlt.TableName,
                            UtcTimeColumnName = dlt.UtcTimeColumnName,
                            LocalTimeColumnName = dlt.LocalTimeColumnName,
                            MillisecondsColumnName = dlt.MillisecondsColumnName,
                            UserColumnName = dlt.UserColumnName,
                            ReasonColumnName = dlt.ReasonColumnName
                        };
                        currentSettings.Columns = new DataLoggerColumns();
                        var columns = UFUAEditor.GetDataLoggerColumnList(Document, DataLoggerName) as List<String>;
                        if (columns != null)
                            columns.ForEach(x => currentSettings.Columns.Add(new DataLoggerColumn() { Name = x }));
                    }
                }
                else if (currentSettings != null)
                {
                    currentSettings.Columns.Clear();
                    currentSettings = null;
                }

                DatalogerConnection = UFUAEditor.GetDataLoggerConnection(Document, DataLoggerName);
                bDInit = true;
            }
        }
        #endregion

        #region EditSettings
        bool bUserInteractionSettings;
        private void GetItem(string itemName, bool bForceCreateDataSource = false)
        {
            bUserInteractionSettings = true;
            try
            {

                var setting = (from m in MemorySettingList where m.Name.Equals(itemName) select m).FirstOrDefault();
                if (!string.IsNullOrEmpty(setting?.GridLayout))
                {
                    GridLayout = setting.GridLayout;
                    if (DataLoggerName == setting.DataLoggerName && bForceCreateDataSource && !bDesignmode)
                        CreateDataSource();
                    if (!string.IsNullOrEmpty(designDataLoggerName))
                        DataLoggerName = setting.DataLoggerName;
                    ActualConfig = setting.Name;
                }
                else
                {
                    ResetGridLayout();
                    if (DataLoggerName == designDataLoggerName && bForceCreateDataSource && !bDesignmode)
                        CreateDataSource();
                    if(!string.IsNullOrEmpty(designDataLoggerName))
                        DataLoggerName = designDataLoggerName;
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

        void SaveGridConfiguration()
        {
            var Selected = MemorySettingList?.FirstOrDefault(s => s.Name.Equals(ActualConfig));
            var Utf8NoBom = new UTF8Encoding(true);
            SaveDesignGridLayout();
            if (Selected != null)
                Selected.GridLayout = GridLayout;
            else
            {
                Selected = new Setting() { GridLayout = GridLayout, Name = GridLayoutHelper.DesignSettingName, ReadOnly = true };
                MemorySettingList.Add(Selected);
            }
            string currentUserName = helper.Username;
            string userName = oldusername != currentUserName ? oldusername : currentUserName;
            StorageHelper.StorageHelper.SaveMemoryMap(MemorySettingList, Document, Name, userName);
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
        bool bControlLoaded;
        void OnControlLoaded()
        {
            ControlLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void chkMultiLine_Checked(object sender, RoutedEventArgs e)
        {
            ColumnTextWrap = TextWrapping.Wrap;
        }

        private void chkMultiLine_Unchecked(object sender, RoutedEventArgs e)
        {
            ColumnTextWrap = TextWrapping.NoWrap;
        }

        private void cmbExportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exportType = e.Source as ComboBox;

            switch ((ExportFileType)exportType.SelectedIndex)
            {
                case ExportFileType.Csv:
                    chkMultiLine.IsEnabled = false;
                    break;
                case ExportFileType.Html:
                    chkMultiLine.IsEnabled = true;
                    break;
                case ExportFileType.Xls:
                    chkMultiLine.IsEnabled = false;
                    break;
                case ExportFileType.Pdf:
                    chkMultiLine.IsEnabled = true;
                    break;
                default:
                    break;
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
            {               
                ChangeSetting(editValue);
            }
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
                OnRefreshData();
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
            SaveDesignGridLayout();
            ResetGridLayout();
            configMemory.EditValue = ActualConfig;
            bCallingResetCommand = false;
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
                    return true;
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
            string configname = configMemory.EditValue as String;

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
                    DataLoggerName = DataLoggerName,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.DataLoggerName = DataLoggerName;
                selected.GridLayout = GridLayout;
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
            SaveGridConfiguration();
            SaveRuntimeLayout(GetStorageName());
            bCallingSaveCommand = false;
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
                        param => IsEnabledRemoveCommand
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
            configMemory.DataContext = MemorySettingList.Names;
            bCallingRemoveCommand = false;
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
                    return true;
            }
        }
        internal bool IsEnabledRemoveCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (string.IsNullOrEmpty(configMemory.EditValue as string) || bCallingResetCommand || bCallingSaveCommand || bCallingRemoveCommand || (configMemory.EditValue as string) == GridLayoutHelper.DesignSettingName)
                    return false;
                else
                    return true;
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
                if (oldusername != null && helper?.Username != oldusername && !Editable && UserBasedRuntimeSettings)
                    SaveGridConfiguration();

                if (!String.IsNullOrEmpty(helper.Username))
                    MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username);
                else
                    EnsureDefaultValue(false);
                if (!LoadRuntimeLayout(GetStorageName()))
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                GetItem(ActualConfig);
                oldusername = helper.Username;
            });
        }

        void EnsureDefaultValue(bool bSetCombo = true)
        {           
            MemorySettingList = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            var defaultsetting = (from m in MemorySettingList where m.Name.Equals(GridLayoutHelper.DesignSettingName) select m).FirstOrDefault();
            if (defaultsetting == null)
                MemorySettingList.Add(new Setting() { Name = GridLayoutHelper.DesignSettingName, DataLoggerName = designDataLoggerName, GridLayout = GridLayout, ReadOnly = true });
            if (bSetCombo)
            {
                configMemory.DataContext = MemorySettingList.Names;
                if (defaultsetting != null)
                {
                    bIsInEditMode = true;
                    configMemory.EditValue = defaultsetting.Name;
                    bIsInEditMode = false;
                }
            }
        }
        #endregion

        public void Initialize()
        {
        }
        #endregion
        private void OnColumnsGenerated(object sender, RoutedEventArgs e)
        {
            GridControl gridControl = sender as GridControl;
            if (gridControl.ItemsSource == null)
                return;

            DataView view = gridControl.ItemsSource as DataView;
            view.BeginInit();
            gridControl.BeginDataUpdate();

            GridColumnCollection cols = gridControl.Columns;
            cols.ForEach(col =>
            {
                col.AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                col.ReadOnly = true;
            });

            if (qualityToStringConverter != null)
            {
                qualityToStringConverter.CurrentStringList = stringlist;
            }

            for (int ii = 0; ii < view.Table.Columns.Count; ii++)
            {
                var col = view.Table.Columns[ii];
                var colName = col.ColumnName;
                try
                {
                    if (col.DataType == typeof(System.DateTime))
                    {
                        gridControl.Columns[colName].EditSettings = TryFindResource("dateSettings") as DevExpress.Xpf.Editors.Settings.TextEditSettings;

                        if (RunningOnServer && colName == currentSettings?.LocalTimeColumnName)
                        {
                            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
                            var binding = new Binding()
                            {
                                Path = new PropertyPath("Value"),
                                Mode = BindingMode.OneWay,
                                Converter = new UTCToLocalTimeConverter(),
                                ConverterParameter = ClientTimezoneOffset
                            };
                            factory.SetBinding(TextBlock.TextProperty, binding);
                            var dt = new DataTemplate { VisualTree = factory };
                            gridControl.Columns[colName].CellTemplate = dt;
                        }
                    }
                    else
                    {
                        gridControl.Columns[colName].EditSettings = TryFindResource("generalSettings") as DevExpress.Xpf.Editors.Settings.TextEditSettings;
                    }
                }
                catch (Exception ex)
                {
                }
                if (qualityColumnNames.Contains(colName))
                    SetQualityCellTemplate(colName);
            }
            gridControl.EndDataUpdate();
            view.EndInit();
            view = null;

            #region Runtime Settings Initialization
            if (!bDesignmode && !bColumnsGenerated)
            {
                if (string.IsNullOrEmpty(GridLayout) && configMemory.EditValue as string == (configMemory.DataContext as List<string>).First())
                    SaveDesignGridLayout();

                //designGridLayout = InitDesign();
                //configMemory.ItemsSource = MemorySettingList;
                //configMemory.SelectedIndex = 0;

                //if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                //{
                //    try
                //    {
                //        LoadRuntimeLayout(GetStorageName());
                //        GetItem(ActualConfig);
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}
                bColumnsGenerated = true;
            }
            #endregion
        }
        private void SetQualityCellTemplate(string colName)
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding("Value")
            {
                Converter = qualityToStringConverter,
                Mode = BindingMode.OneWay
            });
            var dt = new DataTemplate { VisualTree = factory };
            gridControl.Columns[colName].CellTemplate = dt;
        }

        private void tableView_ShowingEditor(object sender, DevExpress.Xpf.Grid.ShowingEditorEventArgs e)
        {
            e.Cancel = true;
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.AutoWidth = false;
            tableView.BestFitColumns();
        }

        private void comboDataloggerList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (isEdinting || bUserInteractionSettings)
                return;
            isEdinting = true;
            try
            {
                DataLoggerName = ((DevExpress.Xpf.Bars.BarEditItem)e.OriginalSource).EditValue as String;
            }
            finally
            {
                isEdinting = false;
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
                    select new StorageColumn()
                    {
                        FieldName = column.FieldName,
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
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            if (sender is DataloggerViewer)
            {
                DataloggerViewer control = sender as DataloggerViewer;
                if (control.ReadLocalValue(DataloggerViewer.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(DataloggerViewer.ControlForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlForeground", control.ControlForeground);
                else
                    ret.Add("ControlForeground", foreground);



                if (control.ReadLocalValue(DataloggerViewer.BackgroundProperty) != DependencyProperty.UnsetValue && control.Background != null)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;
            DataloggerViewer control = sender as DataloggerViewer;
            string prop = ((DependencyProperty)property)?.Name;
            if (prop.Equals(DataloggerViewer.ToolbarForegroundProperty.Name))
            {
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                return foreground;
            }
            else if (prop.Equals(DataloggerViewer.ToolbarBackgroundProperty.Name))
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
