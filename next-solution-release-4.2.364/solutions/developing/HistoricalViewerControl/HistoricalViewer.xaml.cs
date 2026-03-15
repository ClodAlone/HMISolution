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
using DevExpress.Xpo.DB;
using Utilities;
using log4net;
using DevExpress.Xpf.Grid;
using System.Data;
using Converters;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using ScreenSettings;
using Ookii.Dialogs.Wpf;
using UFUAHistorianModel;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Media;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using System.Globalization;
using DevExpress.Export;
using DevExpress.Xpf.Editors.Settings;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using StorageHelper;
using HelpProvider.ComponentService;
using DevExpress.Xpf.Grid;
using UFProjectManager.ComponentService;
using UFInterfaces;

namespace HistoricalViewerControl
{
    
    /// <summary>
    /// Interaction logic for HistoricalViewer.xaml
    /// </summary>
    public partial class HistoricalViewer : UserControl, IContainPropertyEditors, INotifyPropertyVisibilityChanged, IDisposable, ISettingsHelper, IConnectionAware
        , IGridLayoutUser
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

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(HistoricalViewer));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(HistoricalViewer));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalViewer;
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
            var control = sender as HistoricalViewer;
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
            var control = sender as HistoricalViewer;
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
            var control = sender as HistoricalViewer;
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
            var control = sender as HistoricalViewer;
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
            //    toolbar.Background = ToolbarBackground;
            //    toolbarSettings.Background = ToolbarBackground;
            //    bestFitbar.Background = ToolbarBackground;
            //}

            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    Text1.Foreground = Text2.Foreground = ToolbarForeground;
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    bestFit.Foreground = ToolbarForeground;
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
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as HistoricalViewer;
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
        #endregion

        #region RowAreaFontSettings
        public static readonly DependencyProperty RowAreaFontSettingsProperty = DependencyProperty.Register("RowAreaFontSettings", typeof(FontSettings), typeof(HistoricalViewer), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnRowAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceRowAreaFontSettings)));

        private static object OnCoerceRowAreaFontSettings(DependencyObject o, object value)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceRowAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnRowAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
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
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(HistoricalViewer), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
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

        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(HistoricalViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(HistoricalViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(HistoricalViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty ConnectionStringProperty = DependencyProperty.Register("ConnectionString", typeof(string), typeof(HistoricalViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConnectionStringChanged), new CoerceValueCallback(OnCoerceConnectionString)));

        private static object OnCoerceConnectionString(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceConnectionString((string)value);
            else
                return value;
        }

        private static void OnConnectionStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
                CreateDataSource(DateSpan.None, true);
        }
        [Category("HistoricalViewerOptions")]
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

        #region ShowAuditTrace
        public static readonly DependencyProperty ShowAuditTraceProperty = DependencyProperty.Register("ShowAuditTrace", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowAuditTraceChanged), new CoerceValueCallback(OnCoerceShowAuditTrace)));

        private static object OnCoerceShowAuditTrace(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowAuditTrace((bool)value);
            else
                return value;
        }

        private static void OnShowAuditTraceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                control.OnShowAuditTraceChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowAuditTrace(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowAuditTraceChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (DesignerProperties.GetIsInDesignMode(this))
                OnPropertyVisiblityChanged("ShowAuditTrace");
            else if (bInit)
            {
                TerminatePendingTasks();
                UnloadDataLayer();
                InitValues();
                CreateDataSource(DateSpan.None, true);
            }
        }
        [Category("HistoricalViewerOptions")]
        public bool ShowAuditTrace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowAuditTraceProperty);
            }
            set
            {
                SetValue(ShowAuditTraceProperty, value);
            }
        }
        #endregion

        #region AuditTraceName
        public static readonly DependencyProperty AuditTraceNameProperty = DependencyProperty.Register("AuditTraceName", typeof(String), typeof(HistoricalViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnAuditTraceNameChanged), new CoerceValueCallback(OnCoerceAuditTraceName)));

        private static object OnCoerceAuditTraceName(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceAuditTraceName((String)value);
            else
                return value;
        }

        private static void OnAuditTraceNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                control.OnAuditTraceNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceAuditTraceName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAuditTraceNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("HistoricalViewerOptions")]
        public String AuditTraceName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(AuditTraceNameProperty);
            }
            set
            {
                SetValue(AuditTraceNameProperty, value);
            }
        }
        #endregion

        #region HistoricalName
        public static readonly DependencyProperty HistoricalNameProperty = DependencyProperty.Register("HistoricalName", typeof(String), typeof(HistoricalViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnHistoricalNameChanged), new CoerceValueCallback(OnCoerceHistoricalName)));

        private static object OnCoerceHistoricalName(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceHistoricalName((String)value);
            else
                return value;
        }

        private static void OnHistoricalNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                control.OnHistoricalNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceHistoricalName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHistoricalNameChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("HistoricalViewerOptions")]
        public String HistoricalName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(HistoricalNameProperty);
            }
            set
            {
                SetValue(HistoricalNameProperty, value);
            }
        }
        #endregion

        #region FilterType
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(DateSpan), typeof(HistoricalViewer), new UIPropertyMetadata(DateSpan.Day, new PropertyChangedCallback(OnFilterTypeChanged), new CoerceValueCallback(OnCoerceFilterType)));

        private static object OnCoerceFilterType(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceFilterType((DateSpan)value);
            else
                return value;
        }

        private static void OnFilterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty DateTimeStartProperty = DependencyProperty.Register("DateTimeStart", typeof(DateTime), typeof(HistoricalViewer), new UIPropertyMetadata(new DateTime(2014, 1, 1), new PropertyChangedCallback(OnDateTimeStartChanged), new CoerceValueCallback(OnCoerceDateTimeStart)));

        private static object OnCoerceDateTimeStart(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceDateTimeStart((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeStartChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty DateTimeEndProperty = DependencyProperty.Register("DateTimeEnd", typeof(DateTime), typeof(HistoricalViewer), new UIPropertyMetadata(new DateTime(2099, 12, 31), new PropertyChangedCallback(OnDateTimeEndChanged), new CoerceValueCallback(OnCoerceDateTimeEnd)));

        private static object OnCoerceDateTimeEnd(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceDateTimeEnd((DateTime)value);
            else
                return value;
        }

        private static void OnDateTimeEndChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty MaxRowsProperty = DependencyProperty.Register("MaxRows", typeof(Double), typeof(HistoricalViewer), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxRowsChanged), new CoerceValueCallback(OnCoerceMaxRows)));

        private static object OnCoerceMaxRows(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceMaxRows((Double)value);
            else
                return value;
        }

        private static void OnMaxRowsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        [Category("HistoricalViewerOptions")]
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
        public static readonly DependencyProperty CommandTimeoutProperty = DependencyProperty.Register("CommandTimeout", typeof(int), typeof(HistoricalViewer), new UIPropertyMetadata(0));
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

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(HistoricalViewer), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer historicalEvents = o as HistoricalViewer;
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
            UpdateVisibility();
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
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(HistoricalViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(HistoricalViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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

        #region ShowBestFitButton
        public static readonly DependencyProperty ShowBestFitButtonProperty = DependencyProperty.Register("ShowBestFitButton", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowBestFitButtonChanged), new CoerceValueCallback(OnCoerceShowBestFitButton)));

        private static object OnCoerceShowBestFitButton(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowBestFitButton((bool)value);
            else
                return value;
        }

        private static void OnShowBestFitButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty ShowGroupPanelProperty = DependencyProperty.Register("ShowGroupPanel", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowGroupPanelChanged), new CoerceValueCallback(OnCoerceShowGroupPanel)));

        private static object OnCoerceShowGroupPanel(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowGroupPanel((bool)value);
            else
                return value;
        }

        private static void OnShowGroupPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty ShowFilterPanelProperty = DependencyProperty.Register("ShowFilterPanel", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFilterPanelChanged), new CoerceValueCallback(OnCoerceShowFilterPanel)));

        private static object OnCoerceShowFilterPanel(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowFilterPanel((bool)value);
            else
                return value;
        }

        private static void OnShowFilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty ShowCommandButtonsProperty = DependencyProperty.Register("ShowCommandButtons", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCommandButtonsChanged), new CoerceValueCallback(OnCoerceShowCommandButtons)));

        private static object OnCoerceShowCommandButtons(DependencyObject o, object value)
        {
            HistoricalViewer control = o as HistoricalViewer;
            if (control != null)
                return control.OnCoerceShowCommandButtons((bool)value);
            else
                return value;
        }

        private static void OnShowCommandButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer control = o as HistoricalViewer;
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
        public static readonly DependencyProperty RefreshTimeoutProperty = DependencyProperty.Register("RefreshTimeout", typeof(int), typeof(HistoricalViewer), new UIPropertyMetadata(0, new PropertyChangedCallback(OnRefreshTimeoutChanged), new CoerceValueCallback(OnCoerceRefreshTimeout)));

        private static object OnCoerceRefreshTimeout(DependencyObject o, object value)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                return HistoricalViewer.OnCoerceRefreshTimeout((int)value);
            else
                return value;
        }

        private static void OnRefreshTimeoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                HistoricalViewer.OnRefreshTimeoutChanged((int)e.OldValue, (int)e.NewValue);
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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(false));

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

        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(HistoricalViewer));
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

        [Browsable(false)]
        [XmlIgnore]
        public string StringPlaceolder 
        { 
            get
            {
                return stringPlaceolder;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string StringToRemove
        {
            get
            {
                return $"{UFUAServerInfo.UFUAServerInfo.GetTagRootName()}.";
            }
        }

        #endregion

        #region ClientTimezoneOffset
        public static readonly DependencyProperty ClientTimezoneOffsetProperty = DependencyProperty.Register("ClientTimezoneOffset", typeof(double), typeof(HistoricalViewer), new UIPropertyMetadata(0.0));
        [Browsable(false)]
        [XmlIgnore]
        public double ClientTimezoneOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ClientTimezoneOffsetProperty);
            }
            set
            {
                SetValue(ClientTimezoneOffsetProperty, value);
            }
        }

        #endregion

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                return HistoricalViewer.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                HistoricalViewer.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region UTCSourceTimeStamp
        public static readonly DependencyProperty UTCSourceTimeStampProperty = DependencyProperty.Register("UTCSourceTimeStamp", typeof(bool), typeof(HistoricalViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUTCSourceTimeStampChanged), new CoerceValueCallback(OnCoerceUTCSourceTimeStamp)));

        private static object OnCoerceUTCSourceTimeStamp(DependencyObject o, object value)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                return HistoricalViewer.OnCoerceUTCSourceTimeStamp((bool)value);
            else
                return value;
        }

        private static void OnUTCSourceTimeStampChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HistoricalViewer HistoricalViewer = o as HistoricalViewer;
            if (HistoricalViewer != null)
                HistoricalViewer.OnUTCSourceTimeStampChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUTCSourceTimeStamp(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUTCSourceTimeStampChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool UTCSourceTimeStamp
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UTCSourceTimeStampProperty);
            }
            set
            {
                SetValue(UTCSourceTimeStampProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        IDataLayer dl;
        string connectionStringAuditTrace;
        DateTime MaxDate = new DateTime(2099, 12, 31);
        DateTime MinDate = new DateTime(2014, 1, 1);
        IDocument Document;
        IUIMsgBoxAlertService IUIMsgBoxAlertService;
        IUFUAEditorManager UFUAEditor;
        IUFProjectManager iUFProjectManager;
        IDictionary<String, String> MapToHistoricalConnectsions = new Dictionary<String, String>();
        IDictionary<String, IDataLayer> MapToHistoricalDataLayer = new Dictionary<String, IDataLayer>();
        List<Task> pendingTask = new List<Task>();
        IStringEditorManager stringManager;
        internal bool bSmartSettingsEditing;
        string resetGridLayout;

        InMemoryDataStore InMemory;
        bool bLoaded;
        bool bDesignmode;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.HistoricalViewerControlLog);
        Setting defSetting;
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
            //var task1 = Task.Factory.StartNew(delegate
            using(new WaitCursor())
            {
                try
                {
                    using (FileStream sw = new FileStream(file,FileMode.OpenOrCreate))
                    {
                        //Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            switch ((ExportFileType)index)
                            {
                                case ExportFileType.Csv:
                                    var optionsCsv = new DevExpress.XtraPrinting.CsvExportOptionsEx();
                                    optionsCsv.CustomizeCell += options_CustomizeCell;
                                    tableView.ExportToCsv(sw, optionsCsv);
                                    optionsCsv.CustomizeCell -= options_CustomizeCell;
                                    break;
                                case ExportFileType.Html:
                                    tableView.ExportToHtml(sw);
                                    break;
                                case ExportFileType.Xls:
                                    var optionsXls = new DevExpress.XtraPrinting.XlsExportOptionsEx();
                                    optionsXls.CustomizeCell += options_CustomizeCell;
                                    tableView.ExportToXls(sw, optionsXls);
                                    optionsXls.CustomizeCell -= options_CustomizeCell;
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

        void options_CustomizeCell(CustomizeCellEventArgs e)
        {
            var ex = e as CustomizeCellEventArgsExtended;
            if (ex != null && ex.Value is DateTime && ex.Column.FormatSettings.FormatString == "G")
            {
                var conv = new UTCToLocalConverterWithClientOffset();
                e.Value = conv.Convert(new object[] { ex.Value, ClientTimezoneOffset, RunningOnServer, CurrentCulture}, typeof(DateTime), null, CurrentCulture);
                e.Handled = true;
            }
        }

        private void errorInfo_ClearMessage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            errorInfo.Text = string.Empty;
        }

        void TerminatePendingTasks()
        {
            if (pendingTask.Count > 0)
                Task.WaitAll(pendingTask.ToArray());
        }

        void UnloadDataLayer()
        {
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
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
            }
            MapToHistoricalDataLayer.Clear();
        }

        IDataLayer CreateConnectionStringDataLayer(String connStr, int commandTimeout)
        {
            //if (ufw != null || String.IsNullOrEmpty(ConnectionString))
            if (String.IsNullOrEmpty(connStr))
                return null;
            if (dl != null)
                return dl;
            try
            {
                dl = CreateDataLayer(connStr, commandTimeout);
                return dl;
            }
            catch
            {
                return null;
            }
        }
        void CreateCustomDataLayer(String hstname, int commandTimeout)
        {
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
                    if (!MapToHistoricalDataLayer.ContainsKey(hstname))
                        MapToHistoricalDataLayer.Add(hstname, dl);
                    else if (MapToHistoricalDataLayer[hstname] != dl)
                    {
                        MapToHistoricalDataLayer[hstname].Dispose();
                        MapToHistoricalDataLayer[hstname] = dl;
                    }
                }
            }
            else
            {
                if (!MapToHistoricalDataLayer.ContainsKey(hstname))
                    MapToHistoricalDataLayer.Add(hstname, dl);
                else if (MapToHistoricalDataLayer[hstname] != dl)
                {
                    MapToHistoricalDataLayer[hstname].Dispose();
                    MapToHistoricalDataLayer[hstname] = dl;
                }
            }
        }

        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
 
        IDataLayer CreateDataLayer(String settings, int commandTimeout)
        {
            IDataLayer safedl = null;

            try
            {
                safedl = UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(settings, commandTimeout);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

            return safedl;
        }
        void CreateDataSource(DateSpan rangeType, bool dInit)
        {
            var selecteditem = SelectedItem;
            var showAuditTrace = ShowAuditTrace;
            var connectionString = showAuditTrace ? connectionStringAuditTrace : XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
            if (String.IsNullOrEmpty(connectionString) || !dInit || bRefreshing) // || ufw == null)
                return;

            Task task2 = null;
            try
            {
                bRefreshing = true;
                SetBusy(true);

                var maxr = MaxRows;
                SetTimeSpan(rangeType);

                var utcStart = DateTimeStart.ToUniversalTime();
                var utcEnd = DateTimeEnd.ToUniversalTime();

                if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                    utcStart = (DateTime)SqlDateTime.MinValue;
                if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                    utcEnd = (DateTime)SqlDateTime.MaxValue;
                var runningOnServer = RunningOnServer;
                try
                {
                    var task1 = Task.Factory.StartNew((commandTimeout) =>
                    {
                        List<AuditDataItemModel> list = new List<AuditDataItemModel>();
                        List<UFUAAuditDataItem> _list = new List<UFUAAuditDataItem>();

                        if (dl == null)
                            dl = CreateConnectionStringDataLayer(connectionString, (int)commandTimeout);

                        if (dl == null)
                            return list;

                        IDataLayer _dl = dl;
                        if (!showAuditTrace && !string.IsNullOrEmpty(selecteditem))
                        {
                            if (MapToHistoricalConnectsions != null && MapToHistoricalConnectsions.ContainsKey(selecteditem) && !string.IsNullOrEmpty(MapToHistoricalConnectsions[selecteditem]))
                            {
                                try
                                {
                                    CreateCustomDataLayer(selecteditem, (int)commandTimeout);
                                    if (MapToHistoricalDataLayer != null)
                                    {
                                        if (MapToHistoricalDataLayer.ContainsKey(selecteditem) && MapToHistoricalDataLayer[selecteditem] != null)
                                            _dl = MapToHistoricalDataLayer[selecteditem];

                                        if (!bDesignmode)
                                            connectionString = MapToHistoricalConnectsions[selecteditem];
                                    }
                                }
                                catch (Exception)
                                {
                                    _dl = dl;
                                }
                            }
                        }

                        Dictionary<int, string> mapDescriptions = null;
                        using (UnitOfWork _ufw = new UnitOfWork(_dl))
                        {
                            if (string.IsNullOrEmpty(selecteditem))
                            {
                                if (showAuditTrace)
                                {
                                    var auditTraceSuffix = String.Format("#{0}", UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix());
                                    mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                                       where entry.HistoricalName.EndsWith(auditTraceSuffix)
                                                       select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                                }
                                else
                                {
                                    mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                                       select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                                }

                                if (utcStart == utcEnd)
                                    _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                             where !showAuditTrace || mapDescriptions.Keys.Contains(entry.DataLogRef)
                                             orderby entry.RecordDateTimeUtc descending
                                             select entry).Take((int)maxr).ToList();
                                else
                                    _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                             where (!showAuditTrace || mapDescriptions.Keys.Contains(entry.DataLogRef)) &&
                                             (entry.RecordDateTimeUtc >= utcStart) && (entry.RecordDateTimeUtc <= utcEnd)
                                             orderby entry.RecordDateTimeUtc descending
                                             select entry).Take((int)maxr).ToList();
                            }
                            else
                            {
                                if (showAuditTrace)
                                {
                                    var auditTraceSuffix = String.Format("#{0}", UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix());
                                    var auditNameSuffix = String.Format(".{0}", selecteditem.Replace('/', '.'));
                                    mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                               			where entry.HistoricalName.EndsWith(auditTraceSuffix) && 
                                               			entry.Name.EndsWith(auditNameSuffix)
                                               			select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                                }
                                else
                                {
                                    mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                               			where entry.HistoricalName == selecteditem
                                               			select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                                }

                                if (utcStart == utcEnd)
                                    _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                             where mapDescriptions.Keys.Contains(entry.DataLogRef)
                                             orderby entry.RecordDateTimeUtc descending
                                             select entry).Take((int)maxr).ToList();
                                else
                                {
                                    _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                             where mapDescriptions.Keys.Contains(entry.DataLogRef) &&
                                             entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                                             orderby entry.RecordDateTimeUtc descending
                                             select entry).Take((int)maxr).ToList();
                                }
                            }
                        }
                        _list.ForEach(entry =>
                        {
                            var _entry = new AuditDataItemModel()
                            {
                                Name = entry.Name,
                                Description = mapDescriptions.ContainsKey(entry.DataLogRef) ? mapDescriptions[entry.DataLogRef] : String.Empty,
                                Value = entry.Value,
                                dValue = entry.dValue.HasValue ? entry.dValue.Value : double.NaN,
                                ValueBefore = entry.ValueBefore,
                                dValueBefore = entry.dValueBefore.HasValue ? entry.dValueBefore.Value : double.NaN,
                                RecordDateTime = entry.RecordDateTimeUtc,
                                SourceTimeStamp = entry.SourceTimeStamp,
                                SourcePicoseconds = entry.SourcePicoseconds,
                                ServerTimeStamp = entry.ServerTimeStamp,
                                ServerPicoseconds = entry.ServerPicoseconds,
                                Status = entry.Status,
                                UserName = entry.UserName,
                                Reason = entry.Reason
                            };
                            list.Add(_entry);
                        });
                        return list;
                    }, CommandTimeout);
                    task2 = task1.ContinueWith(ret =>
                    {
                        try
                        {
                            if (ret.IsFaulted && ret.Exception != null)
                            {
                                ShowError(ret.Exception.InnerException);
                            }
                            else
                                gridControl.ItemsSource = ret.Result;
                        }
                        finally
                        {
                            pendingTask.Remove(task1);
                            SetBusy(false);
                            bRefreshing = false;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    pendingTask.Add(task1);
                }
                catch (Exception ex)
                {
                    SetBusy(false);
                    ShowError(ex);
                }
            }
            finally
            {
                if (task2 == null)
                    bRefreshing = false;
            }
        }
        private void ShowError(Exception exception)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.InnerException != null ? exception.InnerException.Message : exception.Message);
                log.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.HistoricalViewerControlLog,
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
        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.Ignore;

            if (Document != null)
            {
                if (IUIMsgBoxAlertService == null)
                    IUIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                if (IUIMsgBoxAlertService != null)
                {
                    try
                    {
                        var result = IUIMsgBoxAlertService.ShowYesNo(
                            String.Format(Properties.Resources.InvalidRowException.Replace("'newline'", Environment.NewLine), e.ErrorText),
                            UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                        if (result != UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
                            e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.NoAction;
                    }
                    catch (Exception)
                    {
                        errorInfo.Text = e.ErrorText;
                    }

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
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.All);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetYearTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Year);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetMonthTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Month);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetWeekTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Week);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetDayTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Day);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetHourTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Hour);
            DateTimeStart = date1;
            DateTimeEnd = date2;
        }

        private void SetMinuteTimeSpan()
        {
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Minute);
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
        
        private string SelectedItem = string.Empty;

        public void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs args)
        {
            SelectedItem = Combo.SelectedItem.ToString();

            if (RefreshDataCommand.CanExecute(null))
            {
                CreateDataSource(DateSpan.None, bDInit);
            }
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
            return gridControl!= null && (gridControl.ItemsSource as List<AuditDataItemModel>) != null ? (gridControl.ItemsSource as List<AuditDataItemModel>).Count > 0 : false;
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
                        param => OnRefreshData((DateSpan)param),
                        param => CanRefreshData()
                        );
                }
                return refreshDataCommand;
            }
        }
        bool CanRefreshData()
        {
            return true;
        }

        bool bRefreshing;
        private void OnRefreshData(DateSpan rangeType)
        {
            CreateDataSource(rangeType, bDInit);
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
                factory = new FrameworkElementFactory(typeof(HistorianSettingsPropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(HistoricalNameProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(AuditTraceNamePropertyEditor));
                dt.DataType = typeof(string);
                dt.VisualTree = factory;
                mapDataTemplates.Add(AuditTraceNameProperty, dt);

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
                if (propertyName == "HistoricalName")
                {
                    return !ShowAuditTrace;
                }
                if (propertyName == "AuditTraceName")
                {
                    return ShowAuditTrace;
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

            startTime.TouchDown -= OnTouchDown;
            startTime.LostFocus -= OnLostFocus;
            endTime.TouchDown -= OnTouchDown;
            endTime.LostFocus -= OnLostFocus;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            TerminatePendingTasks();
            UnloadDataLayer();
            DetachOverrideBaseProperties();
        }
        #endregion

        #region Constructor
        bool bInit;
        bool bDInit;
        public HistoricalViewer()
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
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // temporary code for fixing the devexpress bug
                    // (see http://www.devexpress.com/Support/Center/Question/Details/Q445390)
                    //gridControl.ManipulationDelta += (obj, ev) =>
                    //{
                    //    ev.Handled = true;
                    //};
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    cmbExportType.ItemsSource = Enum.GetValues(typeof(ExportFileType)).Cast<ExportFileType>();
                    cmbExportType.SelectedIndex = 0;
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
                        gridControl.ItemsSource = new List<AuditDataItemModel>();
                        OverrideBaseProperties();
                        toolbar.IsEnabled = false;

                        if (!bSmartSettingsEditing)
                            view.IsHitTestVisible = false;
                    }
                    else
                    {
                        helper = new Helper(Document, this);
                        helper.RefreshCurrentUser();
                        startTime.TouchDown += OnTouchDown;
                        startTime.LostFocus += OnLostFocus;
                        endTime.TouchDown += OnTouchDown;
                        endTime.LostFocus += OnLostFocus;
                        SetTimeSpan(FilterType);
                        InitValues();

                        OverrideBaseProperties();
                        if (RunningOnServer)
                        {
                            Button9.Visibility = cmbExportType.Visibility = System.Windows.Visibility.Collapsed;
                            tableView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                            toolbarSettings.IsVisible = false;
                            ClientTimezoneOffset = (double)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }

                        if (string.IsNullOrEmpty(GridLayout))
                            SaveDesignGridLayout();

                        defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout, Option1 = HistoricalName }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
                        configMemory.DataContext = MemorySettingList?.Names;
                        configMemory.EditValue = MemorySettingList.Count > 0 ? MemorySettingList[0].Name : null;

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
                            CreateDataSource(DateSpan.None, true);
                    }

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

        internal string stringPlaceolder = "HistoricalViewer";
        internal IDictionary<String, String> stringlist;
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

                if (!bDesignmode)
                    Utilities.Converters.ResourceEnumConverter.StringTable = stringlist;

                TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);

                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);

                startTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeStart", stringlist, Properties.Resources.HistoricalEvents_DateTimeStart);
                endTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateTimeEnd", stringlist, Properties.Resources.HistoricalEvents_DateTimeEnd);

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
            });
        }

        private void InitValues()
        {
            if (Document == null)
                Document = ScreenDocument.GetScreenDocument(this);

            if (Document == null)
                return;

            var sdoc = Document;

            var list = new List<string>() { String.Empty };
            if (UFUAEditor == null)
                UFUAEditor = sdoc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor != null)
            {
                if (ShowAuditTrace)
                {
                    var list2 = UFUAEditor.GetAuditTraceTagNameList(sdoc, bReloadDocument: false, inExecution: true);
                    if (list2 != null)
                        list.AddRange(list2.OrderBy(x => x));
                }
                else
                {
                    var list2 = UFUAEditor.GetHistoricalSettingsNameList(sdoc, bReloadDocument: false, inExecution: true);
                    if (list2 != null)
                        list.AddRange(list2.OrderBy(x => x));
                }
            }

            string sessionString = (sdoc as ScreenDocument).SessionString;

            list.ForEach(x =>
            {
                if (!String.IsNullOrEmpty(x) && !MapToHistoricalConnectsions.ContainsKey(x))
                {
                    if (ShowAuditTrace)
                    {
                        connectionStringAuditTrace = UFUAEditor.GetAuditTraceDefaultConnection(sdoc);
                        connectionStringAuditTrace = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connectionStringAuditTrace, sessionString);
                    }
                    else
                    {
                        var conn = UFUAEditor.GetHistorianConnection(sdoc, x);
                        MapToHistoricalConnectsions.Add(x, string.IsNullOrEmpty(conn) ? string.Empty : RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, sessionString));
                    }
                }
            });
            Combo.ItemsSource = list;

            try
            {
                if (ShowAuditTrace && list.Contains(AuditTraceName))
                    Combo.SelectedIndex = list.IndexOf(AuditTraceName);
                else if (list.Contains(HistoricalName))
                    Combo.SelectedIndex = list.IndexOf(HistoricalName);
                else
                    Combo.SelectedIndex = 0;
            }
            catch
            {

            }
            bDInit = true;
        }
        #endregion

        private void tableView_ShowingEditor(object sender, DevExpress.Xpf.Grid.ShowingEditorEventArgs e)
        {
            e.Cancel = true;
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.BestFitColumns();
        }

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
                    HistoricalName = setting.Option1;
                    ActualConfig = setting.Name;
                }
                else
                {
                    GridLayout = defSetting.GridLayout;
                    ActualConfig = GridLayoutHelper.DesignSettingName;
                    HistoricalName = defSetting.Option1;
                }
                bIsInEditMode = true;
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = ActualConfig;
                bIsInEditMode = false;
                if (bForceCreateDataSource && !bDesignmode)
                    CreateDataSource(DateSpan.None, true);
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
            string configname = configMemory.EditValue as String;
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
                    Option1 = HistoricalName,
                    ReadOnly = false
                });
                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.GridLayout = GridLayout;
                selected.Option1 = Combo.SelectedItem.ToString();
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

        internal bool IsEnableCommand
        {
            get
            {
                if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    return false;
                if (string.IsNullOrEmpty(configMemory.EditValue as String) || bCallingResetCommand || bCallingRemoveCommand || bCallingSaveCommand)
                    return false;
                else
                    return configMemory.EditValue as String != GridLayoutHelper.DesignSettingName;
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
        #endregion

        #region IConnectionAware
        public string GetConnectionString()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, doc?.rootBase);
        }
        #endregion

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
            defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout, Option1 = HistoricalName }, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);
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

        public void Initialize()
        {
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
            return new Dictionary<string, Brush>();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is HistoricalViewer)
            {
                HistoricalViewer control = sender as HistoricalViewer;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(HistoricalViewer.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);
                if (control.ReadLocalValue(HistoricalViewer.ControlForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlForeground", control.ControlForeground);
                else
                    ret.Add("ControlForeground", foreground);

                if (control.ReadLocalValue(HistoricalViewer.BackgroundProperty) != DependencyProperty.UnsetValue && control.Background != null)
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
            HistoricalViewer control = sender as HistoricalViewer;
            string prop = ((DependencyProperty)property)?.Name;
            if (prop.Equals(HistoricalViewer.ToolbarForegroundProperty.Name))
            {
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                return foreground;
            }
            else if (prop.Equals(HistoricalViewer.ToolbarBackgroundProperty.Name))
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
