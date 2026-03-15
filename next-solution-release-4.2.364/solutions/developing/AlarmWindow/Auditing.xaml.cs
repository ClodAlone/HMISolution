using System;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Utilities;
using Converters;
using OPCUAViewModel;
using ViewModelLib;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using ScreenSettings;
using Utilities.WPF;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using DevExpress.Xpf.Grid;
using System.Linq;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using System.Collections.Generic;
using UFInterfaces.PropertyControl;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using System.Globalization;
using System.Xml.Serialization;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Bars.Themes;

namespace AlarmWindow
{
    /// <summary>
    /// Interaction logic for Auditing.xaml
    /// </summary>
    public partial class Auditing : UserControl, IDisposable, IContainPropertyEditors, ISettingsHelper
    {
        #region DP
        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Auditing));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Auditing));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (bInit)
                ControlForeground = Foreground;
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
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
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
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
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    gridView.Columns.ToList().ForEach(x =>
                    {
                        x.HeaderStyle = gridView.TryFindResource("columnStyle") as Style;
                    });
                });

                Style style = view.TryFindResource("ButtonRowCommandStyle") as Style;
                SetNewStyle(ButtonRefresh, style);
            }
            else
            {
                ButtonRefresh.Style = null;
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    gridView.Columns.ToList().ForEach(x =>
                    {
                        x.HeaderStyle = null;
                    });
                });
            }

            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
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

            // Toolbar color settings aren't supported anymore after change from ToolBarTry to DevExpress's BarContainerControl.
            //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    toolbartray.Background = ToolbarBackground;
            //    toolbarSettings.Background = ToolbarBackground;
            //    commands.Background = ToolbarBackground;
            //}
            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    bestFit.Foreground = ToolbarForeground;
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
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
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
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
                var value = AlarmAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontFamily = FontFamily;
                measure.FontFamily = FontFamily;

                AlarmAreaFontSettings = value;
                HeaderFontSettings = measure;


            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
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
                var value = AlarmAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontWeight = FontWeight;
                measure.FontWeight = FontWeight;

                AlarmAreaFontSettings = value;
                HeaderFontSettings = measure;


            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
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
                var value = AlarmAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontStyle = FontStyle;
                measure.FontStyle = FontStyle;

                AlarmAreaFontSettings = value;
                HeaderFontSettings = measure;


            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as Auditing;
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
                var value = AlarmAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontSize = (int)FontSize;
                measure.FontSize = (int)FontSize;

                AlarmAreaFontSettings = value;
                HeaderFontSettings = measure;


            }
        }

        #endregion


        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(Auditing), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            Auditing control = o as Auditing;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing control = o as Auditing;
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

        #region ToolbarForeground
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(Auditing), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("Advanced")]
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


        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(Auditing), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("Advanced")]
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

        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(Auditing), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            Auditing control = o as Auditing;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing control = o as Auditing;
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(Auditing), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            Auditing control = o as Auditing;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing control = o as Auditing;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(Auditing), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            Auditing control = o as Auditing;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing control = o as Auditing;
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


        #region AlarmAreaFontSettings
        public static readonly DependencyProperty AlarmAreaFontSettingsProperty = DependencyProperty.Register("AlarmAreaFontSettings", typeof(FontSettings), typeof(Auditing), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnAlarmAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceAlarmAreaFontSettings)));

        private static object OnCoerceAlarmAreaFontSettings(DependencyObject o, object value)
        {
            Auditing gridAlarmWindow = o as Auditing;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAlarmAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnAlarmAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing gridAlarmWindow = o as Auditing;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAlarmAreaFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceAlarmAreaFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlarmAreaFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);

            //{

            //}
        }

        [Category("Advanced")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings AlarmAreaFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(AlarmAreaFontSettingsProperty);
            }
            set
            {
                SetValue(AlarmAreaFontSettingsProperty, value);
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


        #region AllowDropColumns
        public static readonly DependencyProperty AllowDropColumnsProperty = DependencyProperty.Register("AllowDropColumns", typeof(bool), typeof(Auditing), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowDropColumnsChanged), new CoerceValueCallback(OnCoerceAllowDropColumns)));

        private static object OnCoerceAllowDropColumns(DependencyObject o, object value)
        {
            Auditing control = o as Auditing;
            if (control != null)
                return control.OnCoerceAllowDropColumns((bool)value);
            else
                return value;
        }

        private static void OnAllowDropColumnsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing control = o as Auditing;
            if (control != null)
                control.OnAllowDropColumnsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowDropColumns(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowDropColumnsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public bool AllowDropColumns
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowDropColumnsProperty);
            }
            set
            {
                SetValue(AllowDropColumnsProperty, value);
            }
        }

        #endregion


        #region HeaderFontSettings
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(Auditing), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            Auditing gridAlarmWindow = o as Auditing;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing gridAlarmWindow = o as Auditing;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnHeaderFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
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
            //    if (bDispose)
            //        return;

            //    tableView.BestFitColumns();
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
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(Auditing), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));
        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            Auditing auditing = o as Auditing;
            if (auditing != null)
                return auditing.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing auditing = o as Auditing;
            if (auditing != null)
                auditing.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
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
                gridView.SaveLayoutToStream(output);
                resetGridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void SaveDesignGridLayout()
        {
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                gridView.SaveLayoutToStream(output);
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
                            gridView.RestoreLayoutFromStream(output);
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
        [SvgValueConverter(typeof(ConvertAuditingGridLayout))]
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
        #region ClientTimezoneOffset
        public static readonly DependencyProperty ClientTimezoneOffsetProperty = DependencyProperty.Register("ClientTimezoneOffset", typeof(double), typeof(Auditing), new UIPropertyMetadata(0.0));
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

        //[Browsable(false)]
        //public UserControl SmartControl
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return new Controls.SmartControl(this);
        //    }
        //}

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(Auditing), new UIPropertyMetadata(false));

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
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(Auditing));
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(Auditing), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            Auditing Auditing = o as Auditing;
            if (Auditing != null)
                return Auditing.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Auditing Auditing = o as Auditing;
            if (Auditing != null)
                Auditing.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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
        MonitoredItemViewModel monitoredItemViewModel;
        SafeObservableCollection<AuditEventStateViewModel> emptyList = new SafeObservableCollection<AuditEventStateViewModel>();
        bool bDesignmode;
        IDocument Document;
        IStringEditorManager stringManager;
        Setting defSetting;
        string resetGridLayout;
        internal bool bSmartSettingsEditing;
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
            var model = GetItemViewModel();
            if (model == null)
                return;

            if (model.AuditEventRefreshCommand.CanExecute(null) && model.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected == true)
            {
                bCallingRefreshCommand = true;
                monitoredItemViewModel.AuditEventRefreshCommand.Execute(null);
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
                    return GetItemViewModel() != null && monitoredItemViewModel.AuditEventRefreshCommand.CanExecute(null);
            }
        }

        #endregion


        MonitoredItemViewModel GetItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
            monitor = monitoredItemViewModel;
            if (monitor == null || !monitor.IsValid)
                return null;

            var list = monitor.AuditEventStateList; // initialize the alarm client subscription
            return monitor;
        }

        #region Constructor
        bool bInit;

        public Auditing()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////
            // devexpress optimized mode implementation
            // (see https://www.devexpress.com/Support/Center/Question/Details/T147586)
            ///////////////////////////////////////////////////////////////////////////////////////////
            //tableView.UseLightweightTemplates = UseLightweightTemplates.None;

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbartray.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (o, e) =>
            {
                if(!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    if (RunningOnServer)
                    {
                        toolbartray.Bars.Clear();
                        toolbartray.Bars.Add(toolBarControl);
                    }
#if !WINDOWS_UWP
                    this.AddToolBarStyleResource();
#endif

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                }

                configMemory.DataContext = MemorySettingList?.Names;
                configMemory.EditValue = GridLayoutHelper.DesignSettingName;
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
                    gridView.ItemsSource = new List<AuditEventStateViewModel>();
                    bLoaded = true;
                    OverrideBaseProperties();
                    toolbartray.IsEnabled = false;
                    if (!bSmartSettingsEditing)
                        view.IsHitTestVisible = false;
                    bInit = true;
                }
                else
                {
                    tableView.PreviewKeyDown += rowPresenterGrid_PreviewKeyDown;
                    tableView.PreviewMouseDown += rowPresenterGrid_PreviewMouseDown;

                    helper = new Helper(Document, this as ISettingsHelper);
                    helper.RefreshCurrentUser();

                    if (RunningOnServer)
                    {
                        toolbarSettings.IsVisible = false;
                        try
                        {
                            ClientTimezoneOffset = (double)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        MouseEnter += GridControl_MouseEnter;
                    }

                    OverrideBaseProperties();

                    if (string.IsNullOrEmpty(GridLayout))
                        SaveDesignGridLayout();

                    defSetting = GridLayoutHelper.InitDesign(new Setting() { GridLayout = GridLayout}, Document, Name, out MemorySettingList, UserBasedRuntimeSettings ? helper.Username : null);

                    configMemory.DataContext = MemorySettingList.Names;
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
                    bInit = true;
                    if (!bControlLoaded)
                    {
                        bControlLoaded = true;
                        OnControlLoaded();
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
                    //if (monitoredItemViewModel != null && monitoredItemViewModel.AuditEventStateList != null)
                    //    monitoredItemViewModel.AuditEventStateList.CollectionChanged -= AuditEventStateList_CollectionChanged;
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    collectionView = null;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;

                    statusText.Text = Properties.Resources.Connecting;
                    gridView.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

                    gridView.BeginDataUpdate();
                    gridView.ItemsSource = CollectionView;

                    var filter = gridView.FilterCriteria;
                    gridView.FilterCriteria = null;
                    gridView.FilterCriteria = filter;
                    gridView.EndDataUpdate();

                    if (monitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.Good)
                    {
                        statusText.Visibility = Visibility.Collapsed;
                        IsEnabled = true;
                    }

                }
            };
            ///////////////////////////////////////////////////////////////////////////////////////////       
        }

        private void rowPresenterGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source is FrameworkElement &&
                (e.Key == Key.Left || e.Key == Key.Right ||
                 e.Key == Key.Up || e.Key == Key.Down))
                UpdateFocus(e.OriginalSource as FrameworkElement);
        }

        private void UpdateFocus(FrameworkElement sender)
        {
            var isElementContained = (sender is TextBlock || sender is Grid) && (from c in tableView.GetVisualChildrenOfType<FrameworkElement>()
                                      where c == sender || sender == tableView
                                      select c).FirstOrDefault() != null;
            if (!isElementContained)
                return;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (!bDispose)
                    tableView.Focus();
            });
        }

        private void rowPresenterGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement)
                UpdateFocus(e.OriginalSource as FrameworkElement);
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

        void ChangeSetting(String settingName)
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
                configMemory.DataContext = MemorySettingList.Names;
                configMemory.EditValue = configname;
            }
            else
            {
                selected.GridLayout = GridLayout;
                selected.ReadOnly = false;
                configMemory.DataContext = MemorySettingList.Names;
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
        #region Methods
        CollectionViewSource collectionView;
        public ICollectionView CollectionView
        {
            get
            {
                if (monitoredItemViewModel == null || monitoredItemViewModel.AuditEventStateList == null)
                    return EmptyCollectionView;
                if (collectionView == null)
                    collectionView = new CollectionViewSource { Source = monitoredItemViewModel.AuditEventStateList };

                if (collectionView.Source == null)
                    return EmptyCollectionView;
                return CollectionViewSource.GetDefaultView(collectionView.Source);
            }
        }

        public ICollectionView EmptyCollectionView
        {
            get
            {
                return CollectionViewSource.GetDefaultView(emptyList);
            }
        }
        string stringPlaceolder = "Auditing";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                IDictionary<string, string> stringlist = null;
                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));

                TranslationHelper.TranlslateColumns(gridView.Columns, stringlist, stringPlaceolder);

                configMemory.EditValue = ActualConfig;
                ButtonRefresh.ToolTip = ButtonRefresh.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshCmd", stringlist, Properties.Resources.Refresh);
                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);

                if (!bDesignmode)
                    CallRefreshCommand();
                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;
            });
        }

        void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDispose)
                return;

            var monitoredItem = sender as MonitoredItemViewModel;
            if (e.PropertyName == "Quality")
            {
                if (monitoredItem.DataValue == null || monitoredItem.DataValue.StatusCode == Opc.Ua.StatusCodes.Good)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (bDispose)
                            return;

                        statusText.Text = Properties.Resources.Connecting;
                        statusText.Visibility = Visibility.Visible;
                        IsEnabled = true;

                        gridView.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

                        gridView.BeginDataUpdate();
                        gridView.ItemsSource = CollectionView;

                        var filter = gridView.FilterCriteria;
                        gridView.FilterCriteria = null;
                        gridView.FilterCriteria = filter;
                        gridView.EndDataUpdate();

                        statusText.Visibility = Visibility.Collapsed;
                    });
                }
                else
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (bDispose)
                            return;

                        IsEnabled = false;
                        gridView.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

                        gridView.BeginDataUpdate();
                        gridView.ItemsSource = EmptyCollectionView;
                        gridView.EndDataUpdate();

                        statusText.Text = Properties.Resources.NotConnected;
                        statusText.Visibility = Visibility.Visible;
                    });
                }
            }
        }
        private void InitLocalization()
        {
            Col1.Header = Properties.Resources.State;
            Col2.Header = Properties.Resources.Source;
            Col3.Header = Properties.Resources.Type;
            Col4.Header = Properties.Resources.Method;
            Col5.Header = Properties.Resources.Time;
            Col6.Header = Properties.Resources.Message;
        }
        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.BestFitColumns();
        }
        #endregion
        #region Isolated Storage

        internal String GetStorageName()
        {
            return StorageHelper.StorageHelper.GetStorageName(Document, this.Name, UserBasedRuntimeSettings ? helper.Username : null, true);
        }

        static String GetStoreFileNameDocking(String title, String username)
        {
            return String.Format("{0}.{1}Layout{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(username) ? String.Format("_{0}", username) : "");
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

        void SaveLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.Create, isoStorage))
                {
                    gridView.SaveLayoutToStream(stream);
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadLayout(String title)
        {
            try
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    var isoStorage = GetStorage();
                    if (null == isoStorage || string.IsNullOrEmpty(title))
                        return;

                    using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, UserBasedRuntimeSettings ? helper?.Username : null), FileMode.OpenOrCreate, isoStorage))
                    {
                        gridView.RestoreLayoutFromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {

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

                // Defines Data Template for 'ConnectionStringProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditingWriteAccessMaskProperty, dt);

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

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
            tableView.PreviewKeyDown -= rowPresenterGrid_PreviewKeyDown;
            tableView.PreviewMouseDown -= rowPresenterGrid_PreviewMouseDown;
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

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            DetachOverrideBaseProperties();

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            gridView.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;

            gridView.BeginDataUpdate();
            gridView.ItemsSource = EmptyCollectionView;
            gridView.EndDataUpdate();
        }
        #endregion
        internal List<string> defaultSvgColumns = new List<string>() { "EnabledState", "Source", "Type",
            "Method", "Time", "Message"};
    }
    public class ConvertAuditingGridLayout : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender = null)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            Auditing gridAlarmWindow = sender as Auditing;
            var gridLayout = value as string;
            List<object> res = new List<object>();
            if (string.IsNullOrEmpty(gridLayout))
            {
                gridAlarmWindow.defaultSvgColumns.ForEach(colName =>
                {
                    res.Add(new Dictionary<string, object>() {
                    { "FieldName", colName },
                    {"ActualWidth", double.NaN }});
                });
            }
            else
            {
                var columnlist = (from column in gridAlarmWindow.gridView.Columns where column.Visible == true orderby column.VisibleIndex select column);
                columnlist.ToList().ForEach(c =>
                {
                    res.Add(new Dictionary<string, object>() {
                    { "FieldName", c.FieldName },
                    {"ActualWidth", c.ActualWidth }});
                });
            }

            return res;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
}
