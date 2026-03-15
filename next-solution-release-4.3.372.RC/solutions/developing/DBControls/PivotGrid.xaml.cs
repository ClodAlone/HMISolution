using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataReader;
using log4net;
using ViewModelLib;
using Utilities;
using System.Threading.Tasks;
using DevExpress.Xpf.Editors.Settings;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Reflection;
using Converters;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using Ookii.Dialogs.Wpf;
using ScreenSettings;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using DataReader.Helpers;
using DevExpress.Data;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using DevExpress.Data.Filtering;
using UIMsgBoxAlertService.ComponentService;
using DevExpress.Xpf.PivotGrid;
using System.Windows.Media;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using UFInterfaces.PropertyControl;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using UFProjectManager.ComponentService;
using System.Windows.Navigation;

namespace DBControls
{
    /// <summary>
    /// Interaction logic for PivotGrid.xaml
    /// </summary>
    public partial class PivotGrid : UserControl, IDisposable, IContainPropertyEditors, ISettingsHelper, IConnectionAware
    {
        #region DP

        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(PivotGrid));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(PivotGrid));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
                UpdateForeground();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
    
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !IsManipulationEnabled)
                UpdateBackground();
        }
        private void UpdateBackground()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                //Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    gridDataControl1.Background = Background;
                    gridDataControl1.CellBackground = Background;
                    gridDataControl1.CellTotalBackground = Background;
                    gridDataControl1.ValueBackground = Background;
                    gridDataControl1.ValueTotalBackground = Background;


                    //(from c in gridDataControl1.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                    // where c.Name == "DragBorder" || c.Name == "Border" || c.Name == "Separator" || c.Name == "columnBorder"
                    //  || c.Name == "border0"
                    // select c).ToList().ForEach(o =>
                    // {
                    //     o.Background = Background;
                    // });
                    //(from c in gridDataControl1.GetVisualChildrenOfType<DevExpress.Xpf.Core.XPFContentControl>()
                    // where c.Name == "rowValues"
                    // select c).ToList().ForEach(o =>
                    // {
                    //     o.Background = Background;
                    // });
                }//);
            }
        }
        private void UpdateForeground()
        {
            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                txtReload.Foreground = Foreground;
                txtExport.Foreground = Foreground;
                gridDataControl1.CellForeground = Foreground;
                gridDataControl1.CellTotalForeground = Foreground;
                gridDataControl1.ValueForeground = Foreground;
                gridDataControl1.ValueTotalForeground = Foreground;
            }
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !bOverride)
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
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !bOverride)
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
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !bOverride)
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
            var control = sender as PivotGrid;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit && !bOverride)
            {
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontSize = (int)FontSize;
                measure.FontSize = (int)FontSize;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
                
                
            }
        }

        private void UpdateControlLayout()
        {
            UpdateBackground();
            UpdateForeground();
            //UpdateToolbarBackground();
            //UpdateToolbarForeground();
        }

        //private void UpdateToolbarForeground()
        //{
        //    if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
        //    {
        //        actualSettings.Foreground = ToolbarForeground;
        //        configMemory.Foreground = ToolbarForeground;
        //        bestFit.Foreground = ToolbarForeground;
        //        expandAll.Foreground = ToolbarForeground;
        //        collapseAll.Foreground = ToolbarForeground;
        //    }
        //}

        //private void UpdateToolbarBackground()
        //{
        //    if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
        //    {
        //        toolbar.Background = ToolbarBackground;
        //        toolbarSettings.Background = ToolbarBackground;
        //        toolbarSettings1.Background = ToolbarBackground;
        //    }
        //}


        #endregion

        #region  PivotGridSettings Properties

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(PivotGrid), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("Advanced")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(PivotGrid), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("Advanced")]
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

        #region RowAreaFontSettings
        public static readonly DependencyProperty RowAreaFontSettingsProperty = DependencyProperty.Register("RowAreaFontSettings", typeof(FontSettings), typeof(PivotGrid), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnRowAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceRowAreaFontSettings)));

        private static object OnCoerceRowAreaFontSettings(DependencyObject o, object value)
        {
            PivotGrid historicalEvents = o as PivotGrid;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceRowAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnRowAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid historicalEvents = o as PivotGrid;
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
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(PivotGrid), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            PivotGrid historicalEvents = o as PivotGrid;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid historicalEvents = o as PivotGrid;
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

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                UpdateColumns();
                //Click_BestFit(null, null);
            });
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


        #region ShowRowGrandTotals
        public static readonly DependencyProperty ShowRowGrandTotalsProperty = DependencyProperty.Register("ShowRowGrandTotals", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowRowGrandTotalsChanged), new CoerceValueCallback(OnCoerceShowRowGrandTotals)));

        private static object OnCoerceShowRowGrandTotals(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowRowGrandTotals((bool)value);
            else
                return value;
        }

        private static void OnShowRowGrandTotalsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowRowGrandTotalsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowRowGrandTotals(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowRowGrandTotalsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowRowGrandTotals
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowRowGrandTotalsProperty);
            }
            set
            {
                SetValue(ShowRowGrandTotalsProperty, value);
            }
        }

        #endregion

        #region ShowRowTotals
        public static readonly DependencyProperty ShowRowTotalsProperty = DependencyProperty.Register("ShowRowTotals", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowRowTotalsChanged), new CoerceValueCallback(OnCoerceShowRowTotals)));

        private static object OnCoerceShowRowTotals(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowRowTotals((bool)value);
            else
                return value;
        }

        private static void OnShowRowTotalsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowRowTotalsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowRowTotals(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowRowTotalsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowRowTotals
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowRowTotalsProperty);
            }
            set
            {
                SetValue(ShowRowTotalsProperty, value);
            }
        }

        #endregion

        #region ShowRowHeaders
        public static readonly DependencyProperty ShowRowHeadersProperty = DependencyProperty.Register("ShowRowHeaders", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowRowHeadersChanged), new CoerceValueCallback(OnCoerceShowRowHeaders)));

        private static object OnCoerceShowRowHeaders(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowRowHeaders((bool)value);
            else
                return value;
        }

        private static void OnShowRowHeadersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowRowHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowRowHeaders(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowRowHeadersChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowRowHeaders
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowRowHeadersProperty);
            }
            set
            {
                SetValue(ShowRowHeadersProperty, value);
            }
        }

        #endregion

        #region ShowColumnGrandTotals
        public static readonly DependencyProperty ShowColumnGrandTotalsProperty = DependencyProperty.Register("ShowColumnGrandTotals", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowColumnGrandTotalsChanged), new CoerceValueCallback(OnCoerceShowColumnGrandTotals)));

        private static object OnCoerceShowColumnGrandTotals(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowColumnGrandTotals((bool)value);
            else
                return value;
        }

        private static void OnShowColumnGrandTotalsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowColumnGrandTotalsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowColumnGrandTotals(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowColumnGrandTotalsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowColumnGrandTotals
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowColumnGrandTotalsProperty);
            }
            set
            {
                SetValue(ShowColumnGrandTotalsProperty, value);
            }
        }

        #endregion


        #region ShowColumnGrandTotalHeader
        public static readonly DependencyProperty ShowColumnGrandTotalHeaderProperty = DependencyProperty.Register("ShowColumnGrandTotalHeader", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowColumnGrandTotalHeaderChanged), new CoerceValueCallback(OnCoerceShowColumnGrandTotalHeader)));

        private static object OnCoerceShowColumnGrandTotalHeader(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowColumnGrandTotalHeader((bool)value);
            else
                return value;
        }

        private static void OnShowColumnGrandTotalHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowColumnGrandTotalHeaderChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowColumnGrandTotalHeader(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowColumnGrandTotalHeaderChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowColumnGrandTotalHeader
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowColumnGrandTotalHeaderProperty);
            }
            set
            {
                SetValue(ShowColumnGrandTotalHeaderProperty, value);
            }
        }

        #endregion



        #region ShowColumnTotals
        public static readonly DependencyProperty ShowColumnTotalsProperty = DependencyProperty.Register("ShowColumnTotals", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowColumnTotalsChanged), new CoerceValueCallback(OnCoerceShowColumnTotals)));

        private static object OnCoerceShowColumnTotals(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowColumnTotals((bool)value);
            else
                return value;
        }

        private static void OnShowColumnTotalsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowColumnTotalsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowColumnTotals(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowColumnTotalsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowColumnTotals
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowColumnTotalsProperty);
            }
            set
            {
                SetValue(ShowColumnTotalsProperty, value);
            }
        }

        #endregion

        #region ShowColumnHeaders
        public static readonly DependencyProperty ShowColumnHeadersProperty = DependencyProperty.Register("ShowColumnHeaders", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowColumnHeadersChanged), new CoerceValueCallback(OnCoerceShowColumnHeaders)));

        private static object OnCoerceShowColumnHeaders(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowColumnHeaders((bool)value);
            else
                return value;
        }

        private static void OnShowColumnHeadersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowColumnHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowColumnHeaders(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowColumnHeadersChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowColumnHeaders
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowColumnHeadersProperty);
            }
            set
            {
                SetValue(ShowColumnHeadersProperty, value);
            }
        }

        #endregion


        #region ShowDataHeaders
        public static readonly DependencyProperty ShowDataHeadersProperty = DependencyProperty.Register("ShowDataHeaders", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowDataHeadersChanged), new CoerceValueCallback(OnCoerceShowDataHeaders)));

        private static object OnCoerceShowDataHeaders(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowDataHeaders((bool)value);
            else
                return value;
        }

        private static void OnShowDataHeadersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowDataHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowDataHeaders(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowDataHeadersChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowDataHeaders
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowDataHeadersProperty);
            }
            set
            {
                SetValue(ShowDataHeadersProperty, value);
            }
        }

        #endregion


        #region ShowFilterHeaders
        public static readonly DependencyProperty ShowFilterHeadersProperty = DependencyProperty.Register("ShowFilterHeaders", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowFilterHeadersChanged), new CoerceValueCallback(OnCoerceShowFilterHeaders)));

        private static object OnCoerceShowFilterHeaders(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowFilterHeaders((bool)value);
            else
                return value;
        }

        private static void OnShowFilterHeadersChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowFilterHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowFilterHeaders(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFilterHeadersChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowFilterHeaders
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowFilterHeadersProperty);
            }
            set
            {
                SetValue(ShowFilterHeadersProperty, value);
            }
        }

        #endregion


        #region ShowTotalsForSingleValues
        public static readonly DependencyProperty ShowTotalsForSingleValuesProperty = DependencyProperty.Register("ShowTotalsForSingleValues", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowTotalsForSingleValuesChanged), new CoerceValueCallback(OnCoerceShowTotalsForSingleValues)));

        private static object OnCoerceShowTotalsForSingleValues(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowTotalsForSingleValues((bool)value);
            else
                return value;
        }

        private static void OnShowTotalsForSingleValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowTotalsForSingleValuesChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowTotalsForSingleValues(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowTotalsForSingleValuesChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowTotalsForSingleValues
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowTotalsForSingleValuesProperty);
            }
            set
            {
                SetValue(ShowTotalsForSingleValuesProperty, value);
            }
        }

        #endregion


        #region ShowPrefilterPanel
        public static readonly DependencyProperty ShowPrefilterPanelProperty = DependencyProperty.Register("ShowPrefilterPanel", typeof(ShowPrefilterPanelMode), typeof(PivotGrid), new UIPropertyMetadata(ShowPrefilterPanelMode.Never, new PropertyChangedCallback(OnShowPrefilterPanelChanged), new CoerceValueCallback(OnCoerceShowPrefilterPanel)));

        private static object OnCoerceShowPrefilterPanel(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceShowPrefilterPanel((ShowPrefilterPanelMode)value);
            else
                return value;
        }

        private static void OnShowPrefilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                control.OnShowPrefilterPanelChanged((ShowPrefilterPanelMode)e.OldValue, (ShowPrefilterPanelMode)e.NewValue);
        }

        protected virtual ShowPrefilterPanelMode OnCoerceShowPrefilterPanel(ShowPrefilterPanelMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowPrefilterPanelChanged(ShowPrefilterPanelMode oldValue, ShowPrefilterPanelMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public ShowPrefilterPanelMode ShowPrefilterPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ShowPrefilterPanelMode)GetValue(ShowPrefilterPanelProperty);
            }
            set
            {
                SetValue(ShowPrefilterPanelProperty, value);
            }
        }

        #endregion


        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(PivotGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));

        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            PivotGrid gridAlarmWindow = o as PivotGrid;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid gridAlarmWindow = o as PivotGrid;
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

        [Category("PivotGridSettings")]
        [DefaultValue(typeof(Boolean), "true")]
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

        #region ControlDataSource
        public static readonly DependencyProperty ControlDataSourceProperty = DependencyProperty.Register("ControlDataSource", typeof(ControlDataSourceItem), typeof(PivotGrid), new UIPropertyMetadata(new ControlDataSourceItem(), new PropertyChangedCallback(OnControlDataSourceChanged), new CoerceValueCallback(OnCoerceControlDataSource)));

        private static object OnCoerceControlDataSource(DependencyObject o, object value)
        {
            PivotGrid gridControl = o as PivotGrid;
            if (gridControl != null)
                return gridControl.OnCoerceControlDataSource((ControlDataSourceItem)value);
            else
                return value;
        }

        private static void OnControlDataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid gridControl = o as PivotGrid;
            if (gridControl != null)
                gridControl.OnControlDataSourceChanged((ControlDataSourceItem)e.OldValue, (ControlDataSourceItem)e.NewValue);
        }

        protected virtual ControlDataSourceItem OnCoerceControlDataSource(ControlDataSourceItem value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlDataSourceChanged(ControlDataSourceItem oldValue, ControlDataSourceItem newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue == null)
                return;

            if (!bInit && !NeedsUpdate)
                return;

            if (NeedsUpdate)
                InitColumns(newValue.ColumnListSettings);

            NeedsUpdate = false;
            SaveDesignGridLayout();
            
            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
            {
                //InitDBControl();
                LoadData(true, true);
            }
            else
            {
                LoadData();
            }

        }

        private void InitColumns(ColumnItemList columnListSettings)
        {
            gridDataControl1.BeginInit();
            gridDataControl1.Fields.Clear();
            DataTemplate datatemplate = TryFindResource("ColumnHeaderTemplate") as DataTemplate;
            DataTemplate celltemplate = TryFindResource("CellTemplate") as DataTemplate;
            DataTemplate cellvaluetemplate = TryFindResource("CellValueTemplate") as DataTemplate;
            if (columnListSettings != null)
                foreach (ColumnItem item in columnListSettings)
                {
                    PivotGridField field = new PivotGridField(item.ColName, FieldArea.ColumnArea)
                    {
                        Caption = item.Caption,
                        HeaderListTemplate = datatemplate,
                        HeaderTemplate = datatemplate,
                        CellTemplate = celltemplate,
                        ValueTemplate = cellvaluetemplate
                    };
                    field.Visible = item.IsVisible;
                    gridDataControl1.Fields.Add(field);
                }

            gridDataControl1.EndInit();
        }
        private void UpdateColumns(ColumnItemList columnListSettings)
        {
            gridDataControl1.BeginInit();
            gridDataControl1.Fields.ToList().ForEach(f =>
            {
                string _column = TranslationHelper.TranlslateText($"_{stringPlaceolder}_{f.FieldName}", stringlist, f.Caption);
                f.Caption = _column;
            });
            gridDataControl1.EndInit();
        }
        private void UpdateColumns()
        {
            DataTemplate datatemplate = TryFindResource("ColumnHeaderTemplate") as DataTemplate;
            DataTemplate celltemplate = TryFindResource("CellTemplate") as DataTemplate;
            DataTemplate cellvaluetemplate = TryFindResource("CellValueTemplate") as DataTemplate;
            gridDataControl1.BeginInit();
            gridDataControl1.Fields.ToList().ForEach(x =>
            {
                x.HeaderTemplate = datatemplate;
                x.HeaderListTemplate = datatemplate;
                x.CellTemplate = celltemplate;
                x.ValueTemplate = cellvaluetemplate;
            });
            gridDataControl1.EndInit();
        }
        /// <summary>
        /// The connection setting informations used for connecting to database.
        /// </summary>
        [Category("PivotGridSettings")]
        [Browsable(false)]
        public ControlDataSourceItem ControlDataSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ControlDataSourceItem)GetValue(ControlDataSourceProperty);
            }
            set
            {
                SetValue(ControlDataSourceProperty, value);
            }
        }

        internal bool bDesignmode;

        #endregion

        #region ConnectionString
        [Browsable(false)]
        public string ConnectionString { get; set; }
        #endregion


        #region smartcontrol

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.PivotSmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false));

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

        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion
        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(PivotGrid), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            PivotGrid dridControl = o as PivotGrid;
            if (dridControl != null)
                return dridControl.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid dridControl = o as PivotGrid;
            if (dridControl != null)
                dridControl.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
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
            if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
            {
                using (MemoryStream output = new MemoryStream())
                {


                    gridDataControl1.SaveLayoutToStream(output);
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
                gridDataControl1.SaveLayoutToStream(output);
                GridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        void LoadDesignGridLayout()
        {
            string settings = string.Empty;
            settings = GridLayout;

            if (string.IsNullOrEmpty(settings))
                return;

            if (string.IsNullOrEmpty(resetGridLayout))
                SaveResetGridLayout();

            var dim = settings.Length;
            string _mid = string.Empty;

            if (settings.IndexOf('?') == 0)
            {
                _mid = settings.Substring(1, dim - 1);
                SetValue(GridLayoutProperty, _mid);
                return;
            }

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                if (!string.IsNullOrEmpty(settings))
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(settings)))
                    {
                        try
                        {
                            gridDataControl1.RestoreLayoutFromStream(output);
                            UpdateColumns();
                        }
                        catch (Exception ex)
                        {
                            settings = string.Empty;
                        }
                    }
                }
                else
                {
                    settings = string.Empty;
                }
            }
            else
            {
                settings = string.Empty;
            }

            GridLayout = settings;
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

        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(PivotGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(PivotGrid), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            PivotGrid control = o as PivotGrid;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid control = o as PivotGrid;
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(PivotGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            PivotGrid PivotGrid = o as PivotGrid;
            if (PivotGrid != null)
                return PivotGrid.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PivotGrid PivotGrid = o as PivotGrid;
            if (PivotGrid != null)
                PivotGrid.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region Commands
        RelayCommand _reloadData;
        public ICommand ReloadData
        {
            get
            {
                if (_reloadData == null)
                {
                    _reloadData = new RelayCommand(
                        param => CallReloadData(),
                        param => IsEnableCallReloadData
                        );
                }
                return _reloadData;
            }
        }

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
            return gridDataControl1 != null && (gridDataControl1.DataSource as DataView) != null ? (gridDataControl1.DataSource as DataView).Count > 0 : false;
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

        bool bCallingReloadData;
        internal void CallReloadData()
        {
            bCallingReloadData = true;
            LoadData();
            //bCallingReloadData = false;
        }
        internal bool IsEnableCallReloadData
        {
            get
            {
                if (bCallingReloadData)
                    return false;
                else
                    return true;

            }
        }

        private const string defaultSettings = "DefaultSettings";
        private void ShowError(Exception exception)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.Message);
                log.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog,
                    DateTime.UtcNow, $"{error}",
                    System.Diagnostics.EventLogEntryType.Error);

                errorInfo.Text = string.Empty;

                if (Document != null)
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
        #endregion

        #region Declarations
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.DBControlLog);
        internal bool bSmartSettingsEditing;

        bool bLoaded;
        DataView gridDataView = new DataView();
        DataSet gridDataSet = new DataSet();
        Object lockObject = new Object();
        private bool tableChanged;
        internal IDocument Document;
        IStringEditorManager stringManager;
        IDictionary<String, String> stringlist;
        IUFUAEditorManager ufuaEditorService;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IUFProjectManager iUFProjectManager;
        internal bool NeedsUpdate { get; set; }
        #region DefaultDataProvider & DefaultConnectionString
        private string defaultDataProvider;
        internal string DefaultDataProvider { get { return defaultDataProvider; } }
        private string defaultConnectionString;
        internal string DefaultConnectionString { get { return defaultConnectionString; } }
        #endregion 

        private bool TableChanged
        {
            get
            {
                return tableChanged;
            }
            set
            {
                if (value == tableChanged)
                    return;
                tableChanged = value;
                if (RunningOnServer)
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        Setting defSetting;
        string resetGridLayout;
        string oldusername;
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

        #region Constructor
        public PivotGrid()
        {
            InitializeComponent();
            CommonConstructor();
        }
        internal bool bInit = false;
        internal bool bcInit = false;
        void CommonConstructor()
        {
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

                    cmbExportType.ItemsSource = Enum.GetValues(typeof(ExportFileType)).Cast<ExportFileType>();
                    cmbExportType.SelectedIndex = 0;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    iUFProjectManager = Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (Document != null)
                        ufuaEditorService = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                    configMemory.EditValue = GridLayoutHelper.DesignSettingName;
                    UpdateControlLayout();

                    bDesignmode = bDesignmode || DesignerProperties.GetIsInDesignMode(this) || bSmartSettingsEditing;

                    if (Document != null)
                    {
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(null, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (bDesignmode)
                    {
                        //InitDBControl();

                        if (ufuaEditorService != null)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(ConnectionString))
                                {
                                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);
                                }
                                else
                                {
                                    string settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaEditorService.GetHistorianDefaultConnection(Document), (Document as ScreenDocument).SessionString);
                                    if (!string.IsNullOrEmpty(settings))
                                    {
                                        defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                                        defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        OverrideBaseProperties();
                        toolbar.IsEnabled = false;
                        LoadData(true);
                        if (!bSmartSettingsEditing)
                        {
                            view.IsHitTestVisible = false;
                        }
                    }
                    else
                    {
                        bDesignmode = false;
                        if (Document != null)
                        {
                            helper = new Helper(Document, this as ISettingsHelper);
                            helper.RefreshCurrentUser();
                            oldusername = helper.Username;
                            this.Init += Pivot_OnInit;
                        }

                        if (ufuaEditorService != null)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(ConnectionString))
                                {
                                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);
                                }
                                else
                                {
                                    string settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaEditorService.GetHistorianDefaultConnection(Document), (Document as ScreenDocument).SessionString);
                                    if (!string.IsNullOrEmpty(settings))
                                    {
                                        defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                                        defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        OverrideBaseProperties();
                        if (RunningOnServer)
                        {
                            gridDataControl1.ShowPrefilterPanelMode = DevExpress.Xpf.PivotGrid.ShowPrefilterPanelMode.Never;
                            exportDataButton.Visibility = cmbExportType.Visibility = System.Windows.Visibility.Collapsed;
                            toolbarSettings.IsVisible = false;

                            gridDataControl1.AllowDrop = false;
                            var elements = (gridDataControl1 as UIElement).GetVisualChildrenOfType<FrameworkElement>().ToList();
                            elements.ForEach(x => x.IsHitTestVisible = false);
                            IsHitTestVisible = IsHitTestVisible && IsEnabled;
                        }

                        LoadData(datasourcechanged: true, bFInit: true);
                    }
                    bcInit = true;
                }
            };
        }
        #endregion
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
        #endregion
        #region Setting Management

        /// <summary>
        /// Use this method to get the control runtime MemorySettings list
        /// </summary>
        /// <returns></returns>
        public List<String> GetMemoryMap()
        {
            MemorySettings list = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
            return (from n in list select n.Name).ToList();
        }
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
                if (bCallingResetCommand || bCallingSaveCommand)
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
                if (string.IsNullOrEmpty(configMemory.EditValue as string) || bCallingResetCommand || bCallingSaveCommand || bCallingRemoveCommand)
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


        #region Methods
        string stringPlaceolder = "PivotGrid";
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
                exportSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.ToolTip = bestFit.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                expandAll.ToolTip = expandAll.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandAll", stringlist, Properties.Resources.ExpandAll);
                collapseAll.ToolTip = collapseAll.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CollapseAll", stringlist, Properties.Resources.CollapseAll);
                configMemory.EditValue = ActualConfig;
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                exportDataButton.ToolTip = txtExport.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);
                ReloadButton.ToolTip = txtReload.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReloadData", stringlist, Properties.Resources.ReloadData);

                Pivot_OnInit(null, null);
            });
        }

        private void Pivot_OnInit(object sender, EventArgs e)
        {
            UpdateColumns(ControlDataSource.ColumnListSettings);
        }

        private void errorInfo_ClearMessage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            errorInfo.Text = string.Empty;
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < gridDataControl1.ColumnCount; i++)
            {
                gridDataControl1.BestFitColumn(i);
            } 
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
                                    gridDataControl1.ExportToCsv(sw);
                                    break;
                                case ExportFileType.Html:
                                    gridDataControl1.ExportToHtml(sw);
                                    break;
                                case ExportFileType.Xls:
                                    gridDataControl1.ExportToXls(sw);
                                    break;
                                case ExportFileType.Pdf:
                                    gridDataControl1.ExportToPdf(sw);
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
        private void InitDBControl()
        {
            //gridDataControl1.ItemsSource = null;
            gridDataControl1.RefreshData();
        }
        internal event EventHandler Init;
        private void OnInit(EventArgs e)
        {
            EventHandler temp = Init;
            if (temp != null)
                temp(null, e);
        }
        internal void LoadData(bool schema = false, bool datasourcechanged = false, bool bFInit = false)
        {
            if (isLoadingValues)
                return;

            Task task2 = null;
            try
            {
                isLoadingValues = true;
                string _defaultDataProvider = DefaultDataProvider;
                string _defaultConnectionString = DefaultConnectionString;

                if (ControlDataSource.ControlDataSource != null &&
                    !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.DataProvider) &&
                    !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Connection))
                {
                    _defaultDataProvider = ControlDataSource.ControlDataSource.DataProvider;
                    _defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ControlDataSource.ControlDataSource.Connection, (Document as ScreenDocument)?.SessionString), Document?.rootBase);
                }

                defaultDataProvider = _defaultDataProvider;
                defaultConnectionString = _defaultConnectionString;

                if (!bDesignmode)
                    ConnectionString = String.Format("DataProvider={0};{1}", defaultDataProvider, defaultConnectionString);

                if (string.IsNullOrEmpty(_defaultDataProvider) || string.IsNullOrEmpty(_defaultConnectionString))
                {
                    var error = string.Format("{0}: {1}", Name, Properties.Resources.ErrConnEmpty);
                    log.Error(error);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog,
                        DateTime.UtcNow, $"{error}",
                        System.Diagnostics.EventLogEntryType.Error);
                    return;
                }

                if (ControlDataSource.ControlDataSource == null || string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Select))
                {
                    log.Error(Properties.Resources.ErrDataSource);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog,
                        DateTime.UtcNow, $"{Properties.Resources.ErrDataSource}",
                        System.Diagnostics.EventLogEntryType.Error);
                    return;
                }

                SetBusy(true);
                //gridDataControl1.ItemsSource = null;
                gridDataControl1.RefreshData();
                //gridDataControl1.Columns.Clear();
                //gridDataControl1.Columns.BeginUpdate();

                var controlDataSource = ControlDataSource.ControlDataSource;
                var columnListSettings = ControlDataSource.ColumnListSettings;
                var tablename = ControlDataSource.TableName;
                
                var task1 = Task.Factory.StartNew(delegate
                {
                    using (var connection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                    {
                        try
                        {
                            connection.Open();

                            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);
                            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                            dbdapater.SelectCommand.Connection = connection;
                            StringBuilder commantText;

                            commantText = new StringBuilder(controlDataSource.Select);
                            if (!string.IsNullOrEmpty(controlDataSource.Where))
                                commantText.AppendFormat(" Where {0}", controlDataSource.Where);
                            if (!String.IsNullOrEmpty(controlDataSource.GroupBy))
                                commantText.AppendFormat(" Group By {0}", controlDataSource.GroupBy);
                            if (!string.IsNullOrEmpty(controlDataSource.Sort))
                                commantText.AppendFormat(" Order By {0}", controlDataSource.Sort);
                            dbdapater.SelectCommand.CommandText = commantText.ToString();

                            if (gridDataSet.Tables.Count > 0)
                                gridDataSet.Tables.Clear();

                        //if (string.IsNullOrEmpty(tablename))
                        {
                                DataTable dataTable = new DataTable();
                                dbdapater.FillSchema(dataTable, SchemaType.Source);
                                tablename = dataTable.TableName;
                            }

                            dbdapater.FillSchema(gridDataSet, SchemaType.Source);
                            dbdapater.Fill(gridDataSet);

                            if (columnListSettings.Count == 0)
                            {
                                var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(gridDataSet.Tables[0]);
                                if (columns != null)
                                {
                                    foreach (DataColumnInfo column in columns)
                                    {
                                        columnListSettings.Add(new ColumnItem()
                                        {
                                            ColName = column.Name,
                                            Caption = column.Name,
                                            IsEditable = true,
                                            IsVisible = true
                                        });
                                    }
                                }

                            }

                            commantText = null;
                        }
                        finally
                        {
                            connection.Close();
                        }
                        return gridDataSet;
                    }
                });
                task2 = task1.ContinueWith(ret =>
                {
                    try
                    {
                        if (ret.IsFaulted && ret.Exception != null)
                        {
                            ShowError(ret.Exception.InnerException);
                        }
                        else
                        {
                            errorInfo.Text = string.Empty;
                            try
                            {
                                if (!string.IsNullOrEmpty(tablename))
                                {
                                    ControlDataSource.TableName = tablename;
                                }
                                if (ControlDataSource.ColumnListSettings.Count == 0)
                                {
                                    ControlDataSource.ColumnListSettings = columnListSettings;
                                }
                                gridDataView = gridDataSet.Tables[0].Copy().AsDataView();
                                gridDataControl1.DataSource = gridDataView;
                                gridDataControl1.RefreshData();
                                tableChanged = false;
                            }
                            catch (Exception ex)
                            {
                                log.Error(Name, ex);
                                if (iUFProjectManager != null)
                                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog,
                                    DateTime.UtcNow, $"{Name}: {ex.Message}",
                                    System.Diagnostics.EventLogEntryType.Error);
                            }
                        }

                        //gridDataControl1.Columns.EndUpdate();

                        if (!datasourcechanged && bDesignmode)
                            LoadDesignGridLayout();

                        if (bFInit)
                        {
                            if (string.IsNullOrEmpty(GridLayout))
                                SaveDesignGridLayout();

                            string actualgridlayout = StorageHelper.StorageHelper.LoadMemoryMap<MemorySettings>(Document, Name, helper.Username)?.FirstOrDefault(s => s.Name == GridLayoutHelper.DesignSettingName)?.GridLayout;

                            if (!string.IsNullOrEmpty(actualgridlayout))
                                GridLayout = actualgridlayout;

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
                        bInit = true;
                        if (!bDesignmode && !bControlLoaded && bFInit)
                        {
                            bControlLoaded = true;
                            OnControlLoaded();
                        }
                        EventArgs m = new EventArgs();
                        OnInit(m);
                    }
                    finally
                    {
                        SetBusy(false);
                        isLoadingValues = false;
                        bCallingReloadData = false;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                if (task2 == null)
                    isLoadingValues = false;
            }
        }
        bool isLoadingValues;

        private void Click_ExpandAll(object sender, RoutedEventArgs e)
        {
            gridDataControl1.ExpandAll();
        }

        private void Click_CollapseAll(object sender, RoutedEventArgs e)
        {
            gridDataControl1.CollapseAll();
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

                // Defines Data Template for 'ControlDataSourceProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

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

        #region IDisposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
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
            this.Init -= Pivot_OnInit;

            DetachOverrideBaseProperties();

            gridDataView = null;
            gridDataSet = null;
            lockObject = null;
        }
        #endregion

        private void gridDataControl1_CustomCellAppearance(object sender, PivotCustomCellAppearanceEventArgs e)
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
                e.Background = Background;
        }

        private void gridDataControl1_CustomValueAppearance(object sender, PivotCustomValueAppearanceEventArgs e)
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
                e.Background = Background;
        }

        public string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);
            }

            if (ControlDataSource.ControlDataSource != null &&
                !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.DataProvider) &&
                !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Connection))
            {
                defaultDataProvider = ControlDataSource.ControlDataSource.DataProvider;
                defaultConnectionString = ControlDataSource.ControlDataSource.Connection;
            }
            if (!string.IsNullOrEmpty(defaultDataProvider) && !string.IsNullOrEmpty(defaultConnectionString))
                ConnectionString = String.Format("DataProvider={0};{1}", defaultDataProvider, defaultConnectionString);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
        }
    }
}
