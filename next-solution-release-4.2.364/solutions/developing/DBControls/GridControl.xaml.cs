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
using UFInterfaces.PropertyControl;
using Converters;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using Ookii.Dialogs.Wpf;
using ScreenSettings;
using OPCUAViewModel;
using DataReader.Helpers;
using DevExpress.Data;
using Utilities.WPF;
using GridLayout;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using DevExpress.Data.Filtering;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using System.Windows.Threading;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using DevExpress.Export;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Grid.Themes;
using System.Xml.Serialization;
using System.Windows.Data;
using DBControls.Converters;
using WPFUtilities.Converters;
using DevExpress.Xpf.Bars.Themes;
using WPFUtilities;
using WPFUtilities.Extensions;
using DevExpress.Xpf.Grid.Themes;
using UFProjectManager.ComponentService;
using StorageHelper;

namespace DBControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class GridControl : UserControl, IContainPropertyEditors, IDisposable, ISettingsHelper, IConnectionAware
        , IGridLayoutUser
    {

        #region DP
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }

        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(GridControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(GridControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(GridControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(GridControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(GridControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(GridControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(GridControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(GridControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(GridControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(GridControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as GridControl;
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
            var control = sender as GridControl;
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
            var control = sender as GridControl;
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
            var control = sender as GridControl;
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
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as GridControl;
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
                Dispatcher.BeginInvokeIfRequired(() =>
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

                    (from c in (this as UIElement).GetVisualChildrenOfType<ComboBox>()
                     select c).ToList().ForEach(child =>
                     {
                         child.Foreground = ControlForeground;
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
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    bestFit.Foreground = ToolbarForeground;
            //}
        }
        #endregion


        #region DefaultDigitNumber
        public static readonly DependencyProperty DefaultDigitNumberProperty = DependencyProperty.Register("DefaultDigitNumber", typeof(int), typeof(GridControl), new UIPropertyMetadata(2));
        public int DefaultDigitNumber
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(DefaultDigitNumberProperty);
            }
            set
            {
                SetValue(DefaultDigitNumberProperty, value);
            }
        }
        #endregion


        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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

        #region RowMinHeight
        public static readonly DependencyProperty RowMinHeightProperty = DependencyProperty.Register("RowMinHeight", typeof(Double), typeof(GridControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnRowMinHeightChanged), new CoerceValueCallback(OnCoerceRowMinHeight)));

        private static object OnCoerceRowMinHeight(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceRowMinHeight((Double)value);
            else
                return value;
        }

        private static void OnRowMinHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnRowMinHeightChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRowMinHeight(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowMinHeightChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Double RowMinHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(RowMinHeightProperty);
            }
            set
            {
                SetValue(RowMinHeightProperty, value);
            }
        }
        #endregion

        #region RowSelectionMode
        public static readonly DependencyProperty RowSelectionModeProperty = DependencyProperty.Register("RowSelectionMode", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRowSelectionModeChanged), new CoerceValueCallback(OnCoerceRowSelectionMode)));

        private static object OnCoerceRowSelectionMode(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceRowSelectionMode((bool)value);
            else
                return value;
        }

        private static void OnRowSelectionModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnRowSelectionModeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceRowSelectionMode(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowSelectionModeChanged(bool oldValue, bool newValue)
        {
            if (bDispose || !bLoaded || bDesignmode)
                return;

            tableView.NavigationStyle = RowSelectionMode ? GridViewNavigationStyle.Row : GridViewNavigationStyle.Cell;
        }

        public bool RowSelectionMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(RowSelectionModeProperty);
            }
            set
            {
                SetValue(RowSelectionModeProperty, value);
            }
        }
        #endregion

        #endregion

        #region HeaderBackground
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHeaderBackgroundChanged), new CoerceValueCallback(OnCoerceHeaderBackground)));

        private static object OnCoerceHeaderBackground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceHeaderBackground((Brush)value);
            else
                return value;
        }

        private static void OnHeaderBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnHeaderBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceHeaderBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush HeaderBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }
            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }
        #endregion

        #region HeaderForeground
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHeaderForegroundChanged), new CoerceValueCallback(OnCoerceHeaderForeground)));

        private static object OnCoerceHeaderForeground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceHeaderForeground((Brush)value);
            else
                return value;
        }

        private static void OnHeaderForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnHeaderForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceHeaderForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bcInit)
            {
                UpdateControlLayout();
            }
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush HeaderForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }
            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }
        #endregion

        #region RowAreaForeground
        public static readonly DependencyProperty RowAreaForegroundProperty = DependencyProperty.Register("RowAreaForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRowAreaForegroundChanged), new CoerceValueCallback(OnCoerceRowAreaForeground)));

        private static object OnCoerceRowAreaForeground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceRowAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnRowAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnRowAreaForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRowAreaForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowAreaForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush RowAreaForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RowAreaForegroundProperty);
            }
            set
            {
                SetValue(RowAreaForegroundProperty, value);
            }
        }
        #endregion

        #region ShowCellsBorder
        public static readonly DependencyProperty ShowCellsBorderProperty = DependencyProperty.Register("ShowCellsBorder", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCellsBorderChanged), new CoerceValueCallback(OnCoerceShowCellsBorder)));

        private static object OnCoerceShowCellsBorder(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceShowCellsBorder((bool)value);
            else
                return value;
        }

        private static void OnShowCellsBorderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnShowCellsBorderChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowCellsBorder(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCellsBorderChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowCellsBorder
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowCellsBorderProperty);
            }
            set
            {
                SetValue(ShowCellsBorderProperty, value);
            }
        }
        #endregion

        #region FocusedRowBackground
        public static readonly DependencyProperty FocusedRowBackgroundProperty = DependencyProperty.Register("FocusedRowBackground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnFocusedRowBackgroundChanged), new CoerceValueCallback(OnCoerceFocusedRowBackground)));

        private static object OnCoerceFocusedRowBackground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceFocusedRowBackground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedRowBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnFocusedRowBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedRowBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedRowBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedRowBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedRowBackgroundProperty);
            }
            set
            {
                SetValue(FocusedRowBackgroundProperty, value);
            }
        }
        #endregion

        #region FocusedRowForeground
        public static readonly DependencyProperty FocusedRowForegroundProperty = DependencyProperty.Register("FocusedRowForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnFocusedRowForegroundChanged), new CoerceValueCallback(OnCoerceFocusedRowForeground)));

        private static object OnCoerceFocusedRowForeground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceFocusedRowForeground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedRowForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnFocusedRowForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedRowForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedRowForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Brush FocusedRowForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedRowForegroundProperty);
            }
            set
            {
                SetValue(FocusedRowForegroundProperty, value);
            }
        }
        #endregion

        #region CellBorderColor
        public static readonly DependencyProperty CellBorderColorProperty = DependencyProperty.Register("CellBorderColor", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCellBorderColorChanged), new CoerceValueCallback(OnCoerceCellBorderColor)));

        private static object OnCoerceCellBorderColor(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceCellBorderColor((Brush)value);
            else
                return value;
        }

        private static void OnCellBorderColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnCellBorderColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceCellBorderColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCellBorderColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush CellBorderColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(CellBorderColorProperty);
            }
            set
            {
                SetValue(CellBorderColorProperty, value);
            }
        }
        #endregion

        #region FocusedCellBackground
        public static readonly DependencyProperty FocusedCellBackgroundProperty = DependencyProperty.Register("FocusedCellBackground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFocusedCellBackgroundChanged), new CoerceValueCallback(OnCoerceFocusedCellBackground)));

        private static object OnCoerceFocusedCellBackground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceFocusedCellBackground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedCellBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnFocusedCellBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedCellBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedCellBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedCellBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedCellBackgroundProperty);
            }
            set
            {
                SetValue(FocusedCellBackgroundProperty, value);
            }
        }
        #endregion

        #region FocusedCellForeground
        public static readonly DependencyProperty FocusedCellForegroundProperty = DependencyProperty.Register("FocusedCellForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFocusedCellForegroundChanged), new CoerceValueCallback(OnCoerceFocusedCellForeground)));

        private static object OnCoerceFocusedCellForeground(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceFocusedCellForeground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedCellForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnFocusedCellForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedCellForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedCellForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedCellForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedCellForegroundProperty);
            }
            set
            {
                SetValue(FocusedCellForegroundProperty, value);
            }
        }
        #endregion

        #region  GridControlSettings Properties
        #region RowAreaFontSettings
        public static readonly DependencyProperty RowAreaFontSettingsProperty = DependencyProperty.Register("RowAreaFontSettings", typeof(FontSettings), typeof(GridControl), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnRowAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceRowAreaFontSettings)));

        private static object OnCoerceRowAreaFontSettings(DependencyObject o, object value)
        {
            GridControl historicalEvents = o as GridControl;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceRowAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnRowAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl historicalEvents = o as GridControl;
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
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(GridControl), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            GridControl historicalEvents = o as GridControl;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl historicalEvents = o as GridControl;
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

        #region ToolbarBackground
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [Category("GridControlSettings")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(GridControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("GridControlSettings")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
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

        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));

        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            GridControl gridAlarmWindow = o as GridControl;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridAlarmWindow = o as GridControl;
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

        [Category("GridControlSettings")]
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

        #region AllowEdit
        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowEditChanged), new CoerceValueCallback(OnCoerceAllowEdit)));

        private static object OnCoerceAllowEdit(DependencyObject o, object value)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                return gridControl.OnCoerceAllowEdit((bool)value);
            else
                return value;
        }

        private static void OnAllowEditChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                gridControl.OnAllowEditChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowEdit(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowEditChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("GridControlSettings")]
        public bool AllowEdit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowEditProperty);
            }
            set
            {
                SetValue(AllowEditProperty, value);
            }
        }
        #endregion

        #region AllowAddNew
        public static readonly DependencyProperty AllowAddNewProperty = DependencyProperty.Register("AllowAddNew", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowAddNewChanged), new CoerceValueCallback(OnCoerceAllowAddNew)));

        private static object OnCoerceAllowAddNew(DependencyObject o, object value)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                return gridControl.OnCoerceAllowAddNew((bool)value);
            else
                return value;
        }

        private static void OnAllowAddNewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                gridControl.OnAllowAddNewChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowAddNew(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowAddNewChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("GridControlSettings")]
        public bool AllowAddNew
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowAddNewProperty);
            }
            set
            {
                SetValue(AllowAddNewProperty, value);
            }
        }

        #endregion

        #region AllowRemove
        public static readonly DependencyProperty AllowRemoveProperty = DependencyProperty.Register("AllowRemove", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowRemoveChanged), new CoerceValueCallback(OnCoerceAllowRemove)));

        private static object OnCoerceAllowRemove(DependencyObject o, object value)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                return gridControl.OnCoerceAllowRemove((bool)value);
            else
                return value;
        }

        private static void OnAllowRemoveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                gridControl.OnAllowRemoveChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRemove(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRemoveChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("GridControlSettings")]
        public bool AllowRemove
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowRemoveProperty);
            }
            set
            {
                SetValue(AllowRemoveProperty, value);
            }
        }

        #endregion

        #region ControlDataSource
        public static readonly DependencyProperty ControlDataSourceProperty = DependencyProperty.Register("ControlDataSource", typeof(ControlDataSourceItem), typeof(GridControl), new UIPropertyMetadata(new ControlDataSourceItem(), new PropertyChangedCallback(OnControlDataSourceChanged), new CoerceValueCallback(OnCoerceControlDataSource)));

        private static object OnCoerceControlDataSource(DependencyObject o, object value)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                return gridControl.OnCoerceControlDataSource((ControlDataSourceItem)value);
            else
                return value;
        }

        private static void OnControlDataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridControl = o as GridControl;
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
            if (newValue == null || !bInit && !NeedsUpdate)
                return;

            if (colSettings == null)
                colSettings = new Dictionary<String, ColumnItem>();

            colSettings.Clear();

            if (newValue.ColumnListSettings != null)
                foreach (ColumnItem item in newValue.ColumnListSettings)
                {
                    item.isimage = ImageThresholds[item.ColName].Count() > 0;
                    colSettings[item.ColName] = item;
                }

            bConstraintsDisabled = false;
            bConstraintsUserWarned = false;
            gridDataSet.EnforceConstraints = true;

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

        /// <summary>
        /// The connection setting informations used for connecting to database.
        /// </summary>
        [Category("GridControlSettings")]
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

        #region MaxTransactionsBeforeCommit
        public static readonly DependencyProperty MaxTransactionsBeforeCommitProperty = DependencyProperty.Register("MaxTransactionsBeforeCommit", typeof(uint), typeof(GridControl), new UIPropertyMetadata((uint)10, new PropertyChangedCallback(OnMaxTransactionsBeforeCommitChanged), new CoerceValueCallback(OnCoerceMaxTransactionsBeforeCommit)));

        private static object OnCoerceMaxTransactionsBeforeCommit(DependencyObject o, object value)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                return gridControl.OnCoerceMaxTransactionsBeforeCommit((uint)value);
            else
                return value;
        }

        private static void OnMaxTransactionsBeforeCommitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl gridControl = o as GridControl;
            if (gridControl != null)
                gridControl.OnMaxTransactionsBeforeCommitChanged((uint)e.OldValue, (uint)e.NewValue);
        }

        protected virtual uint OnCoerceMaxTransactionsBeforeCommit(uint value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxTransactionsBeforeCommitChanged(uint oldValue, uint newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("GridControlSettings")]
        public uint MaxTransactionsBeforeCommit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (uint)GetValue(MaxTransactionsBeforeCommitProperty);
            }
            set
            {
                SetValue(MaxTransactionsBeforeCommitProperty, value);
            }
        }

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
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false));

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
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(GridControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            GridControl dridControl = o as GridControl;
            if (dridControl != null)
                return dridControl.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl dridControl = o as GridControl;
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
                gridDataControl1.SaveLayoutToStream(output);
                resetGridLayout = utf8noBOM.GetString(output.ToArray());
            }
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
                            gridDataControl1.RestoreLayoutFromStream(output);

                            if(ControlDataSource?.ColumnListSettings != null)
                            {
                                ColumnItemList columnssettings = ControlDataSource.ColumnListSettings;
                                foreach(ColumnItem colitem in columnssettings)
                                {
                                    colitem.IsVisible = gridDataControl1.Columns[colitem.ColName].Visible;
                                }
                                ControlDataSource.ColumnListSettings = columnssettings;
                            }
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
        [SvgValueConverter(typeof(StorageHelper.ConvertGridLayout))]
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


        #region AllowPrimaryKeyChanging
        public static readonly DependencyProperty AllowPrimaryKeyChangingProperty = DependencyProperty.Register("AllowPrimaryKeyChanging", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowPrimaryKeyChangingChanged), new CoerceValueCallback(OnCoerceAllowPrimaryKeyChanging)));

        private static object OnCoerceAllowPrimaryKeyChanging(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceAllowPrimaryKeyChanging((bool)value);
            else
                return value;
        }

        private static void OnAllowPrimaryKeyChangingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
            if (control != null)
                control.OnAllowPrimaryKeyChangingChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowPrimaryKeyChanging(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowPrimaryKeyChangingChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("GridControlSettings")]
        public bool AllowPrimaryKeyChanging
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowPrimaryKeyChangingProperty);
            }
            set
            {
                SetValue(AllowPrimaryKeyChangingProperty, value);
            }
        }

        #endregion

        #region Editable
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
            UpdateVisibility();
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
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(GridControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(GridControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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

        #region FilterCriteria
        public static readonly DependencyProperty FilterCriteriaProperty = DependencyProperty.Register("FilterCriteria", typeof(string), typeof(GridControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFilterCriteriaChanged), new CoerceValueCallback(OnCoerceFilterCriteria)));

        private static object OnCoerceFilterCriteria(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceFilterCriteria((string)value);
            else
                return value;
        }

        private static void OnFilterCriteriaChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
            if (control != null)
                control.OnFilterCriteriaChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFilterCriteria(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool bChangeCriteria;
        protected virtual void OnFilterCriteriaChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                bChangeCriteria = true;
                gridDataControl1.FilterCriteria = CriteriaOperator.Parse(newValue);
                bChangeCriteria = false;
            }
            catch (Exception ex)
            {
                bChangeCriteria = false;
            }
        }
        [Category("Advanced")]
        [Browsable(false)]
        [XmlIgnore]
        public string FilterCriteria
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                //return (string)GetValue(FilterCriteriaProperty);
                if (!ReferenceEquals(gridDataControl1.FilterCriteria, null))
                    return gridDataControl1.FilterCriteria.ToString();
                else
                    return string.Empty;
            }
            set
            {
                SetValue(FilterCriteriaProperty, value);
            }
        }

        #endregion

        #region ShowBestFitButton
        public static readonly DependencyProperty ShowBestFitButtonProperty = DependencyProperty.Register("ShowBestFitButton", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowBestFitButtonChanged), new CoerceValueCallback(OnCoerceShowBestFitButton)));

        private static object OnCoerceShowBestFitButton(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceShowBestFitButton((bool)value);
            else
                return value;
        }

        private static void OnShowBestFitButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
        [Category("AlarmWindowStyle")]
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
        public static readonly DependencyProperty ShowGroupPanelProperty = DependencyProperty.Register("ShowGroupPanel", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowGroupPanelChanged), new CoerceValueCallback(OnCoerceShowGroupPanel)));

        private static object OnCoerceShowGroupPanel(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceShowGroupPanel((bool)value);
            else
                return value;
        }

        private static void OnShowGroupPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnShowGroupPanelChanged((bool)e.OldValue, (bool)e.NewValue);
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

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(bool), "false")]
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
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
        [Category("AlarmWindowStyle")]
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
        public static readonly DependencyProperty ShowFilterPanelProperty = DependencyProperty.Register("ShowFilterPanel", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFilterPanelChanged), new CoerceValueCallback(OnCoerceShowFilterPanel)));

        private static object OnCoerceShowFilterPanel(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceShowFilterPanel((bool)value);
            else
                return value;
        }

        private static void OnShowFilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
        [Category("AlarmWindowStyle")]
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
        public static readonly DependencyProperty ShowCommandButtonsProperty = DependencyProperty.Register("ShowCommandButtons", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCommandButtonsChanged), new CoerceValueCallback(OnCoerceShowCommandButtons)));

        private static object OnCoerceShowCommandButtons(DependencyObject o, object value)
        {
            GridControl control = o as GridControl;
            if (control != null)
                return control.OnCoerceShowCommandButtons((bool)value);
            else
                return value;
        }

        private static void OnShowCommandButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl control = o as GridControl;
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
        [Category("AlarmWindowStyle")]
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

        #region ShowColumnHeaders
        public static readonly DependencyProperty ShowColumnHeadersProperty = DependencyProperty.Register("ShowColumnHeaders", typeof(bool), typeof(GridControl), new UIPropertyMetadata(true));

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

        #region CurrentCulture
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(GridControl));
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(GridControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        #region ImageThresholds
        public static readonly DependencyProperty ImageThresholdsProperty = DependencyProperty.Register("ImageThresholds", typeof(ImageThresholdCollection), typeof(GridControl), new UIPropertyMetadata(new ImageThresholdCollection(), new PropertyChangedCallback(OnImageThresholdsChanged), new CoerceValueCallback(OnCoerceImageThresholds)));

        private static object OnCoerceImageThresholds(DependencyObject o, object value)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                return GridControl.OnCoerceImageThresholds((ImageThresholdCollection)value);
            else
                return value;
        }

        private static void OnImageThresholdsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridControl GridControl = o as GridControl;
            if (GridControl != null)
                GridControl.OnImageThresholdsChanged((ImageThresholdCollection)e.OldValue, (ImageThresholdCollection)e.NewValue);
        }

        protected virtual ImageThresholdCollection OnCoerceImageThresholds(ImageThresholdCollection value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnImageThresholdsChanged(ImageThresholdCollection oldValue, ImageThresholdCollection newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [SvgValueConverter(typeof(ConvertImageThresholdCollection))]
        public ImageThresholdCollection ImageThresholds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ImageThresholdCollection)GetValue(ImageThresholdsProperty);
            }
            set
            {
                SetValue(ImageThresholdsProperty, value);
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
        RelayCommand _saveData;
        public ICommand SaveData
        {
            get
            {
                if (_saveData == null)
                {
                    _saveData = new RelayCommand(
                        param => CallSaveData(),
                        param => IsEnableCallSaveData
                        );
                }
                return _saveData;
            }
        }
        RelayCommand _insertRow;
        public ICommand InsertRow
        {
            get
            {
                if (_insertRow == null)
                {
                    _insertRow = new RelayCommand(
                        param => CallInsertRow(),
                        param => IsEnableCallInsertRow
                        );
                }
                return _insertRow;
            }
        }
        RelayCommand _deleteRow;
        public ICommand DeleteRow
        {
            get
            {
                if (_deleteRow == null)
                {
                    _deleteRow = new RelayCommand(
                        param => CallDeleteRow(),
                        param => IsEnableCallDeleteRow
                        );
                }
                return _deleteRow;
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
            return !RunningOnServer && gridDataControl1 != null && (gridDataControl1.ItemsSource as DataView) != null ? (gridDataControl1.ItemsSource as DataView).Count > 0 : false;
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

        void OnFocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e)
        {
            if (!bcInit || bDispose)
                return;

            //var focusedRow = gridDataControl1.View.GetRowElementByRowHandle(e.RowData.RowHandle.Value);
            //if (focusedRow as RowControl != null)
            //    (focusedRow as RowControl).Background = FocusedRowBackground;

            RowFocused?.Invoke(this, new RowFocusedEventArgs(focusedRowIndex, e.RowData.RowHandle.Value));
            focusedRowIndex = e.RowData.RowHandle.Value;
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
                if (bCallingReloadData || bCallingSaveData)
                    return false;
                else
                    return true;

            }
        }


        bool bCallingSaveData;
        internal void CallSaveData()
        {
            try
            {
                bCallingSaveData = true;
                if (gridDataControl1.View.CommitEditing())
                {
                    SaveTable();
                    TableChanged = false;
                    LoadData();
                }
            }
            catch
            {
                bCallingSaveData = true;
            }
            finally
            {
                //bCallingSaveData = false;
            }
        }

        private const string defaultSettings = "DefaultSettings";
   

        private void SaveTable()
        {
            DataView dataView = null;

            string _defaultDataProvider = DefaultDataProvider;
            string _defaultConnectionString = DefaultConnectionString;


            if (ControlDataSource.ControlDataSource != null &&
                !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.DataProvider) &&
                !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Connection))
            {
                _defaultDataProvider = ControlDataSource.ControlDataSource.DataProvider;
                _defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(ControlDataSource.ControlDataSource.Connection, Document?.rootBase);
            }

            defaultDataProvider = _defaultDataProvider;
            defaultConnectionString = _defaultConnectionString;

            if (string.IsNullOrEmpty(_defaultDataProvider) || string.IsNullOrEmpty(_defaultConnectionString))
            {
                    var error = string.Format("{0}: {1}", Name, Properties.Resources.ErrConnEmpty);
                log.Error(error);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, 
                    DateTime.UtcNow, error, System.Diagnostics.EventLogEntryType.Error);
                    return;
            }

            using (var writer = new DataWriter.DataSetWriter(_defaultDataProvider, _defaultConnectionString, MaxTransactionsBeforeCommit))
            {
                try
                {
                    writer.AddDbTypeToSqlDbTypeMapping(DbType.Time, SqlDbType.Time);
                    writer.AddDbTypeToSqlDbTypeMapping(DbType.DateTime, SqlDbType.DateTime2);

                    // deleted rows
                    dataView = DeletedRows;
                    if (dataView.Count > 0)
                    {
                        if (dataView.Table.PrimaryKey != null && dataView.Table.PrimaryKey.Count()>0)
                        {
                            dataView.Table.TableName = ControlDataSource.TableName;
                            writer.DeleteRows(dataView);
                            //foreach (DataRowView rowView in dataView)
                            //    rowView.Row.AcceptChanges();
                        }
                        else
                        {
                            try
                            {
                                dataView.Table.TableName = ControlDataSource.TableName;
                                writer.DeleteRows(dataView, false);
                            }
                            catch (Exception)
                            {
                                log.Info(Properties.Resources.NullPrimaryKey, null);
                                if (iUFProjectManager != null)
                                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, DateTime.UtcNow, 
                                    Properties.Resources.NullPrimaryKey, System.Diagnostics.EventLogEntryType.Information);

                            }
                        }
                    }

                    // added rows
                    dataView = AddedRows;
                    if (dataView.Count > 0)
                    {
                        dataView.Table.TableName = ControlDataSource.TableName;
                        var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                           where c.AutoIncrement == true
                                           select c.ColumnName).ToList();
                        if (columnsInfo == null)
                            InitColumnsInfo(dataView.Table.TableName);
                        writer.InsertRows(dataView, listColumns, true, columnsInfo);
                        //foreach (DataRowView rowView in dataView)
                        //    rowView.Row.AcceptChanges();
                    }

                    // update rows
                    dataView = UpdatedRows;
                    if (dataView.Count > 0)
                    {
                        if (dataView.Table.PrimaryKey != null && dataView.Table.PrimaryKey.Count() > 0)
                        {
                            dataView.Table.TableName = ControlDataSource.TableName;
                            var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                               where c.AutoIncrement == true 
                                               select c.ColumnName).ToList();
                            if (columnsInfo == null)
                                InitColumnsInfo(dataView.Table.TableName);
                            writer.UpdateRows(dataView, listColumns, true, !AllowPrimaryKeyChanging, columnsInfo);
                        }
                        else
                        {
                            dataView.Table.TableName = ControlDataSource.TableName;
                            var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                               where c.AutoIncrement == true
                                               select c.ColumnName).ToList();
                            if (columnsInfo == null)
                                InitColumnsInfo(dataView.Table.TableName);
                            writer.UpdateRows(dataView, listColumns, true, false, columnsInfo);
                        }
                    }

                    writer.Commit();
                }
                catch (Exception ex)
                {
                    writer.TryRollback();
                    ShowError(ex);
                }
            }
        }
        void InitColumnsInfo(string tablename)
        {
            if (!string.IsNullOrEmpty(tablename))
            {
                columnsInfo = new Dictionary<string, DataReader.SchemaInfo.ColumnInfo>();
                string[] restrictions = new string[4];
                restrictions[2] = tablename;
                var dynDataSourceInformation = DataReader.DataReader.GetSchemaInfo(defaultDataProvider, defaultConnectionString, "Columns", restrictions).ToList();
                if (dynDataSourceInformation.Count > 0)
                {
                    foreach (var dynColumn in dynDataSourceInformation)
                    {
                        var info = dynColumn as IDictionary<String, Object>;
                        if (info.ContainsKey("COLUMN_NAME") && !(info["COLUMN_NAME"] is System.DBNull))
                        {
                            DataReader.SchemaInfo.ColumnInfo cinfo = new DataReader.SchemaInfo.ColumnInfo();
                            string column = info["COLUMN_NAME"] as String;
                            if (info.ContainsKey("NUMERIC_PRECISION") && !(info["NUMERIC_PRECISION"] is System.DBNull))
                            {
                                short precision = short.Parse(info["NUMERIC_PRECISION"].ToString());
                                cinfo.Precision = precision;
                            }
                            if (info.ContainsKey("NUMERIC_SCALE") && !(info["NUMERIC_SCALE"] is System.DBNull))
                            {
                                short scale = short.Parse(info["NUMERIC_SCALE"].ToString());
                                cinfo.Scale = scale;
                            }
                            if (!columnsInfo.ContainsKey(column))
                                columnsInfo.Add(column, cinfo);
                        }
                    }
                }
            }
        }
        private void ShowError(Exception exception, bool bForceErrorInfo = false)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                var error = string.Format("{0}: {1}", Name, exception.Message);
                log.Error(error, exception);
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, DateTime.UtcNow,
                    $"{error}", 
                    System.Diagnostics.EventLogEntryType.Error);
                ShowError(error, bForceErrorInfo: bForceErrorInfo);
            });
        }
        private void ShowError(string error, bool bIsWarning = false, bool bForceErrorInfo = false)
        {
            //errorInfo.Text = string.Empty;   
            if (Document != null && !bForceErrorInfo)
            {
                if (UIMsgBoxAlertService != null)
                    if (bIsWarning)
                        UIMsgBoxAlertService.ShowWarning(error);
                    else
                        UIMsgBoxAlertService.ShowError(error);
                else
                    errorInfo.Text = error;
            }
            else
                errorInfo.Text = error;

            if (!string.IsNullOrEmpty(errorInfo.Text))
            {
                errorWindow.Visibility = Visibility.Visible;
            }
            else
                errorWindow.Visibility = Visibility.Collapsed;
        }

        internal bool IsEnableCallSaveData
        {
            get
            {
                if (bCallingSaveData || bCallingReloadData || !AllowEdit || !TableChanged || bConstraintsDisabled)
                    return false;
                else
                    return true;
            }
        }



        bool bCallingInsertRow;
        internal void CallInsertRow()
        {
            try
            {
                bCallingInsertRow = true;
                if (!tableView.IsEditing || tableView.CommitEditing())
                {
                    tableView.AddNewRow();
                    tableView.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;
                    tableView.MoveLastRow();
                    TableChanged = true;
                }
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                bCallingInsertRow = false;
            }
        }

        internal bool IsEnableCallInsertRow
        {
            get
            {
                if (bCallingInsertRow || !AllowAddNew || !AllowEdit || bConstraintsDisabled)
                    return false;
                else
                    return true;

            }
        }

        bool bCallingDeleteRow;
        internal void CallDeleteRow()
        {
            try
            {
                bCallingDeleteRow = true;
                DeleteSelectedRow();
                TableChanged = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                bCallingDeleteRow = false;
            }
        }
        private void DeleteSelectedRow()
        {
            //DataView dataView = new DataView(gridDataView.Table.Copy());
            //if (!tableView.IsEditing && dataView.Table.PrimaryKey != null)
            if (!tableView.IsEditing && gridDataView.Table!= null && gridDataView.Table.PrimaryKey != null)
            {
                tableView.BeginInit();
                try
                {
                    gridDataControl1.BeginDataUpdate();
                    tableView.DeleteRow(tableView.FocusedRowHandle);
                    gridDataControl1.EndDataUpdate();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
                finally
                {
                    tableView.EndInit();
                }
            }
        }
        internal bool IsEnableCallDeleteRow
        {
            get
            {
                try
                {
                  if(gridDataSet.Tables.Count == 0)
                      return false;

                  //DataView dataView = new DataView(gridDataView.Table.Copy());
                  //if (bCallingDeleteRow || !AllowRemove || !AllowEdit || dataView.Table.PrimaryKey == null)
                  if (bCallingDeleteRow || !AllowRemove || !AllowEdit || gridDataView.Table.PrimaryKey == null || bConstraintsDisabled)
                      return false;
                  else
                      return true;
                }
                catch(Exception ex)
                {
                    return true;
                }

            }
        }
        #endregion

        #region Declarations
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.DBControlLog);
        IUFProjectManager iUFProjectManager;
        internal bool bSmartSettingsEditing;
        internal bool NeedsUpdate { get; set; }
        bool bLoaded;
        bool bMustSetImageTemplates;
        DataView gridDataView = new DataView();
        DataSet gridDataSet = new DataSet();
        Object lockObject = new Object();
        private DataView AllRows
        {
            get
            {
                lock (lockObject)
                {
                    //DataView dataView = new DataView(gridDataView.Table.Copy()); 
                    //return CopyDataView(dataView,true);
                    using (DataView dataView = new DataView(gridDataView.Table.Copy()))
                    {
                        return CopyDataView(dataView, true);
                    }
                }
            }
        }

        private bool tableChanged;
        internal IDocument Document;
        IStringEditorManager stringManager;
        IUFUAEditorManager ufuaEditorService;
        IUIMsgBoxAlertService uiMsgBoxAlertService;
        IUIMsgBoxAlertService UIMsgBoxAlertService
        {
            get
            {
                if (uiMsgBoxAlertService == null)
                    uiMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiMsgBoxAlertService;
            }
        }
        Dictionary<String, String> stringlist;
        bool isLoadingValues;
        bool bConstraintsDisabled;
        bool bConstraintsUserWarned;
        public event EventHandler<DataLoadedEventArgs> DataLoaded;
        CancellationTokenSource tokenSource;
        CancellationToken ct;
        Task loadDataTask;
        DataLoadedEventArgs dataLoadedArgs = new DataLoadedEventArgs();
        UriToUriAbsoluteImageConverter uriToImageConverter;

        #region DefaultDataProvider & DefaultConnectionString
        internal string defaultDataProvider;
        internal string DefaultDataProvider { get { return defaultDataProvider; } }
        internal string defaultConnectionString;
        internal string DefaultConnectionString { get { return defaultConnectionString; } }
        #endregion

        public IValueConverter UriToImageConverter
        {
            get
            {
                if (uriToImageConverter == null)
                {
                    uriToImageConverter = new UriToUriAbsoluteImageConverter()
                    {
                        FileSystemProviderBase = Document.fileSystemProviderBase,
                        AbsolutePath = new Uri(Path.GetDirectoryName(Document.FilePath) + "\\", UriKind.RelativeOrAbsolute),
                        AbsolutePath2 = Document.GetSpecialFolder(SpecialFolders.Images)
                    };
                }
                return uriToImageConverter;
            }
        }

        public bool IsLoadingValues
        {
            get
            {
                return isLoadingValues;
            }
            set
            {
                if (isLoadingValues != value)
                {
                    isLoadingValues = value;
                    if (isLoadingValues)
                        dataLoadedArgs = new DataLoadedEventArgs();
                    else
                        DataLoaded?.Invoke(this, dataLoadedArgs);
                }
            }
        }

        private bool TableChanged
        {
            get
            {
                return tableChanged;
            }
            set
            {
                if (tableChanged == value)
                    return;
                tableChanged = value;
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        Dictionary<String, ColumnItem> colSettings;
        private Dictionary<String, ColumnItem> ColSettings
        {
            get
            {
                //if (colSettings == null)
                //{
                //    colSettings = new Dictionary<String, ColumnItem>();
                //    foreach (ColumnItem item in ControlDataSource.ColumnListSettings)
                //    {
                //        colSettings[item.ColName] = item;
                //    }
                //}

                return colSettings;
            }
        }
        
        private DataView AddedRows
        {
            get
            {
                lock (lockObject)
                {
                    using (DataView dataView = new DataView(gridDataView.Table.Copy()) { RowStateFilter = DataViewRowState.Added})
                    {
                        return CopyDataView(dataView);
                    }

                    //DataView dataView = new DataView(gridDataView.Table.Copy())
                    //{
                    //    RowStateFilter = DataViewRowState.Added
                    //};

                    //return CopyDataView(dataView);
                }
            }
        }
        private DataView DeletedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(gridDataView.Table.Copy())
                    {
                        RowStateFilter = DataViewRowState.Deleted
                    };

                    //return CopyDataView(dataView);
                    return dataView;
                }
            }
        }
        private DataView UpdatedRows
        {
            get
            {
                lock (lockObject)
                {
                    //DataView dataView = new DataView(gridDataView.Table.Copy())
                    //{
                    //    RowStateFilter = DataViewRowState.ModifiedCurrent
                    //};

                    //return CopyDataView(dataView);
                    using (DataView dataView = new DataView(gridDataView.Table.Copy()) { RowStateFilter = DataViewRowState.ModifiedCurrent })
                    {
                        return CopyDataView(dataView);
                    }
                }
            }
        }

        Setting defSetting;
        string resetGridLayout;
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

        DataReaderModel lastDataReaderModel;
        List<string> imageTemplatedColumns = new List<string>();
        #endregion

        #region Constructor
        public GridControl()
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

                    if(Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    if (Document != null)
                    {
                        iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        ufuaEditorService = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    }

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;

                    configMemory.EditValue = GridLayoutHelper.DesignSettingName;
                    UpdateControlLayout();
                    UpdateVisibility();

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
                        if (bSmartSettingsEditing)
                            LoadData(true);
                        if (!bSmartSettingsEditing)
                        {
                            view.IsHitTestVisible = false;
                        }
                    }
                    else
                    {
                        tableView.NavigationStyle = RowSelectionMode ? GridViewNavigationStyle.Row : GridViewNavigationStyle.Cell;
                        if (Document != null)
                        {
                            helper = new Helper(Document, this as ISettingsHelper);
                            helper.RefreshCurrentUser();
                        }
                        bDesignmode = false;
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

                        tableView.AllowEditing = AllowEdit;
                        OverrideBaseProperties();

                        var wnd = this.FindParent<Window>();
                        string currentStyle = null;
                        try
                        {
                            if (wnd != null)
                                currentStyle = WPFUtilities.ThemeHelper.GetTheme(wnd);
                        }
                        catch (Exception)
                        {
                        }

                        if (currentStyle != null)
                            ApplyStyleToGrid(currentStyle);

                        if (RunningOnServer)
                        {
                            tableView.ShowFilterPanelMode = DevExpress.Xpf.Grid.ShowFilterPanelMode.Never;
                            tableView.ShowGroupPanel = false;
                            tableView.AllowSorting = false;
                            Button9.Visibility = cmbExportType.Visibility = System.Windows.Visibility.Collapsed;
                            Button9.IsHitTestVisible = cmbExportType.IsHitTestVisible = false;
                            var filterpanel = (tableView as UIElement).GetVisualChildrenOfType<DevExpress.Xpf.Grid.FilterPanelContainer>().ToList();
                            filterpanel.ForEach(x => x.IsHitTestVisible = false);
                            var grouppanel = (tableView as UIElement).GetVisualChildrenOfType<DevExpress.Xpf.Grid.GroupPanelControl>().ToList();
                            grouppanel.ForEach(x => x.IsHitTestVisible = false);
                            var searchpanel = (from s in (tableView as UIElement).GetVisualChildrenOfType<ContentControl>() where s.Name == "part_SearchControlContainer" select s).ToList();
                            searchpanel.ForEach(x => x.IsHitTestVisible = false);
                            toolbarSettings.IsVisible = false;
                        }
                        else
                        {
                            MouseEnter += GridControl_MouseEnter;
                        }

                        if (ScrollChangedHandler == null)
                        {
                            ScrollChangedHandler = new ScrollChangedEventHandler(OnScrollChanged);
                            tableView.AddHandler(ScrollViewer.ScrollChangedEvent, ScrollChangedHandler);
                        }
                        LoadData(datasourcechanged:true, bFInit:true);
                    }
                    bcInit = true;
                }
            };
        }

        #region DevExpress T612441
        private Style CopyStyle(Style originalStyle)
        {
            Style copiedStyle = new Style();
            copiedStyle.TargetType = originalStyle.TargetType;

            foreach (var elem in originalStyle.Setters)
                copiedStyle.Setters.Add(elem);

            foreach (var elem in originalStyle.Triggers)
                copiedStyle.Triggers.Add(elem);
            return copiedStyle;
        }

        private void ApplyStyleToGrid(string themeName)
        {
            if (themeName == "Default")
                themeName = "DXStyle";
            if (themeName == "Blend")
                themeName = "MetropolisDark";
            GridRowThemeKeyExtension newKey = new GridRowThemeKeyExtension();
            newKey.ResourceKey = GridRowThemeKeys.LightweightCellStyle;
            newKey.ThemeName = themeName;
            Style currStyle = CopyStyle(gridDataControl1.View.CellStyle);
            try
            {
                currStyle.BasedOn = gridDataControl1.View.FindResource(newKey) as Style;
                gridDataControl1.View.CellStyle = currStyle;
            }
            catch (Exception) { }
        }
        #endregion
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

                if (bMustSetImageTemplates && imageTemplatedColumns.Count > 0)
                    ApplyImageCellTemplates(imageTemplatedColumns);
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
        public event EventHandler RowFocused;
        public event EventHandler<ScrollChangedEventArgs> GridScrollChanged;
        public ScrollChangedEventHandler ScrollChangedHandler;
        int focusedRowIndex = 0;
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

        public void AbortDataLoading()
        {
            if (IsLoadingValues && tokenSource != null)
                tokenSource.Cancel();
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
        void UpdateVisibility()
        {
            commands.Visibility = ShowCommandButtons ? Visibility.Visible : Visibility.Collapsed;
            bestFit.IsVisible = ShowBestFitButton;
            tableView.ShowFilterPanelMode = ShowFilterPanel && !RunningOnServer ? ShowFilterPanelMode.ShowAlways : ShowFilterPanelMode.Never;
            tableView.ShowSearchPanelMode = ShowSearchPanel && !RunningOnServer ? ShowSearchPanelMode.Always : ShowSearchPanelMode.Never;
            tableView.ShowGroupPanel = ShowGroupPanel && !RunningOnServer;
            toolbar.Visibility = !ShowBestFitButton && !Editable ? Visibility.Collapsed : Visibility.Visible;
        }
        string stringPlaceolder = "DBControl";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                try
                {
                    bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(Document, false) == String.Empty;
                    if (!bUntranslated)
                        stringlist = (Dictionary<String, String>)stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                    else
                        stringlist = null;

                    CultureInfo culture = CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                    CurrentCulture = culture;

                    if (bDesignmode || TranslationHelpers.TranslationHelper.CanTranslate(stringlist))
                    {
                        configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                        saveSmallBtn.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                        exportSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);
                        cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                        clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                        bestFit.ToolTip = bestFit.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);

                        Button9.ToolTip = txtExport.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportDataCommand", stringlist, Properties.Resources.ExportDataCommand);
                        configMemory.EditValue = ActualConfig;

                        ReloadButton.ToolTip = txtReload.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReloadData", stringlist, Properties.Resources.ReloadData);
                        InsertRowButton.ToolTip = txtInsertRow.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_InsertRow", stringlist, Properties.Resources.InsertRow);
                        DeleteRowButton.ToolTip = txtDeleteRow.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteRow", stringlist, Properties.Resources.DeleteRow);
                        SaveDataButton.ToolTip = txtSaveRow.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveData", stringlist, Properties.Resources.SaveData);

                        TranslationHelper.TranlslateColumns(gridDataControl1.Columns, stringlist, stringPlaceolder);
                    }
                }
                catch (Exception)
                {
                }
            });
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

        public LightweightCellEditor GetCellByCoords(int rowIndex, int columnIndex)
        {
            LightweightCellEditor cell = null;
            try
            {
                cell = gridDataControl1.View.GetCellElementByRowHandleAndColumn(rowIndex, gridDataControl1.Columns[columnIndex]) as LightweightCellEditor;
            }
            catch (Exception) { }
            return cell;
        }

        public string GetCellValueByCoords(int rowIndex, int columnIndex)
        {
            string cellValue = String.Empty;
            try
            {
                cellValue = gridDataControl1.GetCellDisplayText(rowIndex, gridDataControl1.Columns[columnIndex]);
            }
            catch (Exception) { }
            return cellValue;
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

        private void InitLocalization()
        {
            ReloadButton.ToolTip = Properties.Resources.ReloadData;
            txtReload.Text = Properties.Resources.ReloadData;
            InsertRowButton.ToolTip = Properties.Resources.InsertRow;
            txtInsertRow.Text = Properties.Resources.InsertRow;
            DeleteRowButton.ToolTip = Properties.Resources.DeleteRow;
            txtDeleteRow.Text = Properties.Resources.DeleteRow;
            busyContent.Text = Properties.Resources.WaitText;
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy && !RunningOnServer)
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
        DataView CopyDataView(DataView dataView, bool allRecords = false)
        {
            DataView copyDataView = null;
            try
            {
                uint maxTransactions = MaxTransactionsBeforeCommit;
                if (maxTransactions > 0)
                {
                    copyDataView = new DataView(dataView.Table.Clone());
                    while (dataView.Count > 0)
                    {
                        if (!allRecords && copyDataView.Count >= maxTransactions)
                            break;
                        copyDataView.Table.ImportRow(dataView[0].Row);
                        dataView.Delete(0);
                    }
                }
            }
            catch(Exception ex)
            {
            }
            return copyDataView;
        }

        private void InitDBControl()
        {
            gridDataControl1.ItemsSource = null;
        }


        internal event EventHandler Init;
        private void OnInit(EventArgs e)
        {
            EventHandler temp = Init;
            if (temp != null)
                temp(null, e);
        }
        Dictionary<String, DataReader.SchemaInfo.ColumnInfo> columnsInfo;
        internal void LoadData(bool schema = false, bool datasourcechanged = false, bool bFInit = false)
        {
            if (IsLoadingValues)
                return;
            IsLoadingValues = true;

            Task task2 = null;
            try
            {
                string _defaultDataProvider = DefaultDataProvider;
                string _defaultConnectionString = DefaultConnectionString;

                if (ControlDataSource.ControlDataSource != null &&
                    !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.DataProvider) &&
                    !string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Connection))
                {
                    _defaultDataProvider = ControlDataSource.ControlDataSource.DataProvider;
                    _defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(ControlDataSource.ControlDataSource.Connection, Document?.rootBase);
                }

                if (string.IsNullOrEmpty(_defaultDataProvider) || string.IsNullOrEmpty(_defaultConnectionString))
                {
                    var error = string.Format("{0}: {1}", Name, Properties.Resources.ErrConnEmpty);
                    log.Error(error);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, DateTime.UtcNow, error, 
                        System.Diagnostics.EventLogEntryType.Error);
                    return;
                }

                defaultDataProvider = _defaultDataProvider;
                defaultConnectionString = _defaultConnectionString;

                if (!bDesignmode)
                    ConnectionString = String.Format("DataProvider={0};{1}", defaultDataProvider, defaultConnectionString);

                if (ControlDataSource.ControlDataSource == null || string.IsNullOrEmpty(ControlDataSource.ControlDataSource.Select))
                {
                    log.Error(Properties.Resources.ErrDataSource);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, DateTime.UtcNow, 
                        Properties.Resources.ErrDataSource, System.Diagnostics.EventLogEntryType.Error);
                    return;
                }

                SetBusy(true);
                gridDataControl1.ItemsSource = null;

                bool isFirstLoad = !bcInit;
                if (lastDataReaderModel == null || !lastDataReaderModel.Equals(ControlDataSource.ControlDataSource))
                    gridDataControl1.Columns.Clear();

                var controlDataSource = ControlDataSource.ControlDataSource;
                var columnListSettings = ControlDataSource.ColumnListSettings;
                var tablename = ControlDataSource.TableName;

                if (tokenSource != null)
                {
                    try
                    {
                        if (loadDataTask != null)
                            loadDataTask.Wait();
                    }
                    catch { }
                    tokenSource.Dispose();
                }

                tokenSource = new CancellationTokenSource();
                ct = tokenSource.Token;

                loadDataTask = Task.Factory.StartNew(delegate
                {
                    using (var connection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                    {
                        try
                        {
                            connection.Open();

                            if (!ct.IsCancellationRequested)
                            {
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

	                            try
	                            {
                                    if(!schema)
	                                    dbdapater.Fill(gridDataSet);
	                            }
	                            catch
	                            {
	                                bConstraintsDisabled = true;
	                                gridDataSet.EnforceConstraints = false;
	
	                                if (gridDataSet.Tables.Count > 0)
	                                    gridDataSet.Tables.Clear();

                                    if (!schema)
                                        dbdapater.Fill(gridDataSet);
	                            }

                                if (columnListSettings.Count == 0 && !ct.IsCancellationRequested)
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
                                                IsVisible = true,
                                                isimage = false
                                            });
                                        }
                                    }

                                }

                                commantText = null;
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    return gridDataSet;
                }, tokenSource.Token);
                task2 = loadDataTask.ContinueWith(ret =>
                {
                    try
                    {
                        if (!ct.IsCancellationRequested)
                        {
                            if (ret.IsFaulted && ret.Exception != null)
                            {
                                var errMsg = ret.Exception.InnerException;
                                dataLoadedArgs.Result = DataLoadedEventArgs.LoadResult.Error;
                                dataLoadedArgs.ErrorMsg = errMsg.Message;
                                ShowError(errMsg, true);
                            }
                            else
                            {
                                //errorInfo.Text = string.Empty;
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

	                                try
	                                {
	                                    gridDataView = gridDataSet.Tables[0].Copy().AsDataView();
	                                }
	                                catch
	                                {   
	                                    gridDataSet.Tables[0].Constraints.Clear();
	                                    gridDataView = gridDataSet.Tables[0].Copy().AsDataView();
	                                }

                                    if (bDesignmode)
                                    {
                                        DataView _list = gridDataView;
                                        string filter = _list.Table.Columns[0].DataType == typeof(DateTime) ? "< #1/1/2001#" : _list.Table.Columns[0].DataType == typeof(string) ? "LIKE 'j*'" : "< 0";
	                                    _list.RowFilter = String.Format("{0} {1}", GetEscapedColName(_list.Table.Columns[0].ColumnName), filter);
                                    }
	                                else if (bConstraintsDisabled && !bConstraintsUserWarned && AllowEdit)
	                                {
	                                    bConstraintsUserWarned = true;
	                                    ShowError(Properties.Resources.WarningWriteDisabled, true);
	                                }
                                    bMustSetImageTemplates = false;
                                    gridDataControl1.ItemsSource = gridDataView;
                                }
                                catch (Exception ex)
                                {
                                    dataLoadedArgs.Result = DataLoadedEventArgs.LoadResult.Error;
                                    dataLoadedArgs.ErrorMsg = ex.Message;
                                    log.Error(Name, ex);
                                    if (iUFProjectManager != null)
                                        iUFProjectManager.AddLogEntity(Document, Properties.Resources.DBControlLog, DateTime.UtcNow,
                                        $"{Name}: {ex.Message}", System.Diagnostics.EventLogEntryType.Error);
                                }
                            }
                        }
                        else
                            dataLoadedArgs.Result = DataLoadedEventArgs.LoadResult.Abort;

                        if (isFirstLoad)
                        {
                            LoadDesignGridLayout();
                            if (stringManager != null)
                                StringManager_CultureChanged(null, null);
                        }

                        if (bFInit)
                        {
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
                        UpdateColumnSettings();
                        UpdateVisibility();
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
                        IsLoadingValues = false;
                        bCallingReloadData = false;
                        bCallingSaveData = false;
                    }
                }, tokenSource.Token, TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
            }
            finally
            {
                if (task2 == null)
                    IsLoadingValues = false;
            }
        }

        static string GetEscapedColName(string colName)
        {
            StringBuilder sBuilder = new StringBuilder(colName);

            string pattern = @"([-\]\[<>\?\*\\\""/\|\~\(\)\#/=><+\%&\^\'])";

            Regex expression = new Regex(pattern);

            if (expression.IsMatch(colName))
            {
                sBuilder.Replace(@"\", @"\\");
                sBuilder.Replace("]", @"\]");
                sBuilder.Insert(0, "[");
                sBuilder.Append("]");
            }
            return sBuilder.ToString();
        }

        void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //Detect direction by e.VerticalChange / e.HorizontalChange
            GridScrollChanged?.Invoke(this, e);
        }

        private void ApplyImageCellTemplates(List<string> colNames)
        {
            foreach(var colName in colNames)
            {
                if (String.IsNullOrEmpty(colName) || gridDataControl1.Columns.GetColumnByFieldName(colName) == null || gridDataControl1.Columns[colName].CellTemplate == (DataTemplate)Resources["ImageColumnTemplate"])
                    continue;

                gridDataControl1.Columns[colName].CellTemplate = (DataTemplate)Resources["ImageColumnTemplate"];
            }
        }

        private void ImageColumnEditClick_Handler(object sender, MouseButtonEventArgs e)
        {
            //var fieldName = ((sender as DockPanel).DataContext as EditGridCellData).Column.FieldName;
            if (e.ClickCount != 2 || !AllowEdit || !ControlDataSource.ColumnListSettings[gridDataControl1.CurrentColumn.FieldName].IsEditable)
                return;
            var items = gridDataControl1.SelectedItems;

            var bIsInEditMode = ((sender as DockPanel).TemplatedParent as FrameworkElement).Tag as bool? ?? false;
            if (!bIsInEditMode)
                ((sender as DockPanel).TemplatedParent as FrameworkElement).Tag = true;
        }

        private void gridDataControl1_ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e)
        {
            imageTemplatedColumns.Clear();
        }
        void UpdateColumnSettings()
        { 
            if (gridDataControl1.ItemsSource == null)
                return;

            if (lastDataReaderModel != null && lastDataReaderModel.Equals(ControlDataSource.ControlDataSource))
            {
                if (ControlDataSource.ColumnListSettings != null)
                    imageTemplatedColumns = (from item in ControlDataSource.ColumnListSettings where item.isimage == true select item.ColName).ToList();
                return;
            }

            lastDataReaderModel = new DataReaderModel(ControlDataSource.ControlDataSource);

            DataView view = gridDataView;
            if (!RunningOnServer)
            {
                view.BeginInit();
                gridDataControl1.BeginDataUpdate();
                gridDataControl1.Columns.BeginUpdate();
            }

            if (view.Table != null)
            {
                view.Table.TableName = ControlDataSource.TableName;
                for (int ii = 0; ii < view.Table.Columns.Count; ii++)
                {
                    bool isAutoIncrement = view.Table.Columns[ii].AutoIncrement;
                    var colName = view.Table.Columns[ii].ColumnName;
                    try
                    {
                        gridDataControl1.Columns[colName].Name = colName;
                        gridDataControl1.Columns[colName].Header = colName;
                        Type dbType = view.Table.Columns[ii].DataType;
                        
                        if (dbType == typeof(DateTime))
                        {
                            var settings = (TextEditSettings)Resources["dateSettings"];
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType == typeof(TimeSpan))
                        {
                            var settings = (TextEditSettings)Resources["timespanSettings"];
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType == typeof(UInt16) ||
                                dbType == typeof(UInt32) ||
                                dbType == typeof(UInt64) ||
                                dbType == typeof(Int16) ||
                                dbType == typeof(Int32) ||
                                dbType == typeof(Int64) ||
                                dbType == typeof(Byte) ||
                                dbType == typeof(SByte))
                        {
                            var settings = (TextEditSettingsEx)Resources["numericSettings"];
                            settings.EditValueType = dbType;
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType == typeof(UInt16) ||
                                dbType == typeof(UInt32) ||
                                dbType == typeof(UInt64) ||
                                dbType == typeof(Int16) ||
                                dbType == typeof(Int32) ||
                                dbType == typeof(Int64) ||
                                dbType == typeof(Byte) ||
                                dbType == typeof(SByte))
                        {
                            var settings = (TextEditSettingsEx)Resources["numericSettings"];
                            settings.EditValueType = dbType;
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType == typeof(Decimal))
                        {
                            var settings = (TextEditSettingsEx)Resources["decimalSettings"];
                            settings.EditValueType = dbType;
                            if (columnsInfo == null)
                                InitColumnsInfo(view.Table.TableName);
                            if (columnsInfo[colName].Precision <= 0)
                                settings.Mask = $"f{DefaultDigitNumber}";
                            else
                                settings.Mask = $"f{Math.Min(columnsInfo[colName].Precision, columnsInfo[colName].Scale)}";
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType == typeof(float) ||
                                dbType == typeof(Double) ||
                                dbType == typeof(Single))
                        {
                            var settings = (TextEditSettingsEx)Resources["decimalSettings"];
                            settings.EditValueType = dbType;
                            settings.Mask = $"f{DefaultDigitNumber}";
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }
                        else if (dbType != typeof(bool) && dbType != typeof(Boolean))
                        {
                            var settings = (TextEditSettingsEx)Resources["generalSettings"];
                            settings.EditValueType = dbType;
                            gridDataControl1.Columns[colName].EditSettings = settings;
                        }

                        if (!AllowEdit || isAutoIncrement || bConstraintsDisabled)
                        {
                            gridDataControl1.Columns[colName].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                            gridDataControl1.Columns[colName].ReadOnly = true;
                        }
                        else
                        {
                            gridDataControl1.Columns[colName].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                            gridDataControl1.Columns[colName].ReadOnly = false;
                        }
                        if (ControlDataSource.ColumnListSettings != null)
                        {
                            var colitem = ControlDataSource.ColumnListSettings[colName];

                            if (colitem != null)
                            { 
                                gridDataControl1.Columns[colName].Visible = colitem.IsVisible;
                                gridDataControl1.Columns[colName].Header = colitem.Caption;

                                if (!colitem.IsEditable)
                                {
                                    gridDataControl1.Columns[colName].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                                    gridDataControl1.Columns[colName].ReadOnly = true;
                                }

                                colitem.isimage = ImageThresholds[colName].Count() > 0;
                                if (colitem.isimage)
                                {
                                    //Applying image celltemplate even if the current value is not present in the thresholds map, because the cell value could be edited in runtime
                                    imageTemplatedColumns.Add(colName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    //rowView.Row[colName] = defValue;
                }
                if (isLoadingValues && bMustSetImageTemplates && imageTemplatedColumns.Count > 0)
                    ApplyImageCellTemplates(imageTemplatedColumns);
            }
            TranslationHelper.TranlslateColumns(gridDataControl1.Columns, stringlist, stringPlaceolder);
            //var state = rowView.Row.RowState;
            //rowView.EndEdit();
            if (!RunningOnServer)
            {
                gridDataControl1.Columns.EndUpdate();
                gridDataControl1.EndDataUpdate();
                view.EndInit();

                //tableView.BestFitColumns();
            }
        }

        void OnAutoGeneratedColumns(object sender, RoutedEventArgs e)
        {
            bMustSetImageTemplates = true;
            ApplyImageCellTemplates(imageTemplatedColumns);
        }

        private void tableView_InitNewRow(object sender, DevExpress.Xpf.Grid.InitNewRowEventArgs e)
        {
            DataView view = gridDataView;
            //DataRowView rowView = view.AddNew();
            //rowView.BeginEdit();
            if (!RunningOnServer)
            {
                view.BeginInit();
                gridDataControl1.BeginDataUpdate();
            }
            for (int ii = 0; ii < view.Table.Columns.Count; ii++)
            {
                bool isAutoIncrement = view.Table.Columns[ii].AutoIncrement;
                var autoIncrement = view.Table.Columns[ii].AutoIncrementSeed + view.Table.Columns[ii].AutoIncrementStep;
                Type dbType = view.Table.Columns[ii].DataType;
                var colName = view.Table.Columns[ii].ColumnName;
                var defValue = view.Table.Columns[ii].DefaultValue;
                if (dbType == typeof(System.DateTime))
                    defValue = DateTime.Today;
                else
                    if (dbType == typeof(System.TimeSpan))
                    defValue = new TimeSpan(0, 0, 0, 0);
                else
                    if (dbType == typeof(System.String))
                        defValue = string.Empty;
                    else
                        if (dbType == typeof(System.UInt16) ||
                            dbType == typeof(System.UInt32) ||
                            dbType == typeof(System.UInt64) ||
                            dbType == typeof(System.Double) ||
                            dbType == typeof(System.Single) ||
                            dbType == typeof(System.Int16) ||
                            dbType == typeof(System.Int32) ||
                            dbType == typeof(System.Int64) ||
                            dbType == typeof(System.Double) ||
                            dbType == typeof(System.Single) ||
                            dbType == typeof(System.Byte) ||
                            dbType == typeof(System.SByte) ||
                            dbType == typeof(System.Decimal))
                        {
                            if(isAutoIncrement)
                                defValue = System.DBNull.Value;
                            else
                                defValue = 0;
                        }
                        else
                            defValue = defValue is System.DBNull ? System.DBNull.Value : defValue;
                try
                {
                    gridDataControl1.SetCellValue(e.RowHandle, colName, defValue);
                }
                catch(Exception ex)
                {
                }
            }
            if (!RunningOnServer)
            {
                gridDataControl1.EndDataUpdate();
                view.EndInit();
            }
        }

        private void tableView_InvalidRowException(object sender, DevExpress.Xpf.Grid.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.Ignore;

            if (Document != null)
            {
                if (UIMsgBoxAlertService != null)
                {
                    var result = UIMsgBoxAlertService.ShowYesNo(
                        String.Format(Properties.Resources.InvalidRowException.Replace("'newline'", Environment.NewLine), e.ErrorText), CustomDialogIcons.Question);
                    if (result != CustomDialogResults.Yes)
                        e.ExceptionMode = DevExpress.Xpf.Grid.ExceptionMode.NoAction;
                }
                else
                    errorInfo.Text = e.ErrorText;
            }
            else
                errorInfo.Text = e.ErrorText;       
            
        }

        private void tableView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!bCallingInsertRow)
                TableChanged = true;

            if (imageTemplatedColumns.Contains(e.Cell.Property))
            {
                var cellElement = tableView.GetCellElementByRowHandleAndColumn(e.RowHandle, e.Column);
                var bIsInEditMode = cellElement?.Tag as bool? ?? false;
                if (bIsInEditMode)
                    cellElement.Tag = false;
            }
        }

        private void tableView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                var activeTextEditor = tableView.ActiveEditor as DevExpress.Xpf.Editors.TextEdit;
                if (activeTextEditor != null && activeTextEditor.SelectionLength == activeTextEditor.Text.Length)
                {
                    activeTextEditor.CaretIndex++;
                    e.Handled = true;
                }
            }
        }

        object editorTriggerObject;
        private void OnImageCell_EditorActivated(object sender, RoutedEventArgs e)
        {
            editorTriggerObject = ((sender as DevExpress.Xpf.Editors.TextEdit).Parent as DockPanel).TemplatedParent;
        }

        private void OnGridSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (editorTriggerObject as FrameworkElement != null)
            {
                (editorTriggerObject as FrameworkElement).Tag = false;
                editorTriggerObject = null;
            }
        }

        //private ColumnItemList GetDataSetSchema(DataReaderModel DataSource, out string tableName)
        //{
        //    ColumnItemList res = new ColumnItemList();

        //    if (DataSource != null && !string.IsNullOrEmpty(DataSource.Select))
        //    {
        //        var defaultDataProvider = historianDefaultProvider;
        //        var defaultConnectionString = historianDefaultConnection;

        //        if (string.IsNullOrEmpty(DataSource.DataProvider))
        //            DataSource.DataProvider = defaultDataProvider;
        //        if (string.IsNullOrEmpty(DataSource.Connection))
        //            DataSource.Connection = defaultConnectionString;
        //    }

        //    if (DataSource == null || string.IsNullOrEmpty(DataSource.Select)
        //         || string.IsNullOrEmpty(DataSource.DataProvider)
        //         || string.IsNullOrEmpty(DataSource.Connection))
        //    {
        //        log.Error(Properties.Resources.ErrDataSource);
        //        tableName = string.Empty;
        //        return res; 
        //    }

        //    DataTable dataTable = new DataTable();
        //    using (var connection = DataReader.DataReader.CreateDbConnection(DataSource.DataProvider, DataSource.Connection))
        //    {
        //        try
        //        {
        //            connection.Open();

        //            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(DataSource.DataProvider);
        //            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(DataSource.DataProvider);
        //            dbdapater.SelectCommand.Connection = connection;
        //            dbdapater.SelectCommand.CommandText = DataSource.Select;
        //            dbdapater.FillSchema(dataTable, SchemaType.Source);
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(Name, ex);
        //            tableName = string.Empty;
        //            return res;
        //        }
        //        finally
        //        {
        //            connection.Close();
        //        }
        //    }

        //    if (dataTable.Columns.Count > 0)
        //    {
        //        tableName = dataTable.TableName;

        //        foreach (DataColumn column in dataTable.Columns)
        //        {
        //            res.Add(new ColumnItem()
        //            {
        //                ColName = column.ColumnName,
        //                Caption = column.ColumnName,
        //                IsEditable = true,
        //                IsVisible = true
        //            });
        //        }
        //    }
        //    else
        //        tableName = string.Empty;

        //    return res;
        //}

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

        #region IGridLayoutUser
        public List<StorageColumn> GetColumns()
        {
            return (from column in gridDataControl1.Columns
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
            if (ScrollChangedHandler != null)
                tableView.RemoveHandler(ScrollViewer.ScrollChangedEvent, ScrollChangedHandler);

            if (oldMemoryList != null)
                oldMemoryList.Clear();
            oldMemoryList = null;
            if (MemorySettingList != null)
                MemorySettingList.Clear();
            MemorySettingList = null;

            if (helper is IDisposable)
                (helper as IDisposable).Dispose();
            helper = null;

            if (tokenSource != null)
                tokenSource.Cancel();

            try
            {
                if (loadDataTask != null)
                    loadDataTask.Wait();
            }
            catch { }

            if (tokenSource != null)
                tokenSource.Dispose();

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            DetachOverrideBaseProperties();

            if (columnsInfo != null)
                columnsInfo.Clear();
            columnsInfo = null;
            gridDataView = null;
            gridDataSet = null;
            lockObject = null;
        }
        #endregion

        private void errorInfo_ClearMessage(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            errorInfo.Text = string.Empty;
            errorWindow.Visibility = Visibility.Collapsed;
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            tableView.BestFitColumns();
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
            if(!string.IsNullOrEmpty(defaultDataProvider) && !string.IsNullOrEmpty(defaultConnectionString))
                ConnectionString = String.Format("DataProvider={0};{1}", defaultDataProvider, defaultConnectionString);
            return XpoHelpers.XpoHelper.NormalizeConnectionString(ConnectionString, Document?.rootBase);
        }
    }

    public class DataLoadedEventArgs : EventArgs
    {
        public string ErrorMsg = String.Empty;
        public LoadResult Result = LoadResult.Success;
        public enum LoadResult
        {
            Success,
            Error,
            Abort
        }
    }
    
    public class RowFocusedEventArgs : EventArgs
    {
        public int OldRowIndex;
        public int NewRowIndex;
        public RowFocusedEventArgs(int oldRowIndex, int newRowIndex)
        {
            OldRowIndex = oldRowIndex;
            NewRowIndex = newRowIndex;
        }
    }

    //public class CustomTemplateSelector : DataTemplateSelector
    //{
    //    List<ImageThreshold> columnThresholds;

    //    public CustomTemplateSelector(List<ImageThreshold> thresholds)
    //    {
    //        columnThresholds = thresholds;
    //    }
    //    //TODO , see: https://github.com/DevExpress-Examples/how-to-change-a-cell-template-based-on-some-condition-e2017/tree/11.1.4%2B/CS
    //    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    //    {
    //        GridCellData data = (GridCellData)item;
    //        var cellStringValue = data.GetValue() as string;
    //        if (cellStringValue != null)
    //        {
    //            var uri = (from ImageThreshold t in columnThresholds where t.ImageThresholdValue == cellStringValue select t.Value).FirstOrDefault();
    //            if (uri != null)
    //            {
    //                //TODO: set an image datatemplate with UriToUriAbsoluteImageConverter transforming uri to bitmap
    //                return null;
    //            }
    //        }
    //        //TODO: fallback to default cell datatemplate
    //        return null;
    //    }
    //}

    public class TextEditSettingsEx : TextEditSettings
    {
        static TextEditSettingsEx()
        {
            //var dataTypesInfo = typeof(UnboundColumnInfo).GetField("dataTypes", BindingFlags.NonPublic | BindingFlags.Static);  
            //var dataTypes = dataTypesInfo.GetValue(null) as Type[];  
            //dataTypes[1] = typeof(Int64);  

            DevExpress.Xpf.Editors.Helpers.EditorSettingsProvider.Default.RegisterUserEditor2(typeof(DevExpress.Xpf.Editors.TextEdit), typeof(TextEditSettingsEx),
                optimized => optimized ? new DevExpress.Xpf.Editors.InplaceBaseEdit() : (DevExpress.Xpf.Editors.IBaseEdit)new DevExpress.Xpf.Editors.TextEdit(), () => new TextEditSettingsEx());
        }

        public static readonly DependencyProperty EditValueTypeProperty = DependencyProperty.Register("EditValueType", typeof(Type), typeof(TextEditSettingsEx), new FrameworkPropertyMetadata(null));

        public Type EditValueType
        {
            get { return (Type)GetValue(EditValueTypeProperty); }
            set { SetValue(EditValueTypeProperty, value); }
        }

        protected override void AssignToEditCore(DevExpress.Xpf.Editors.IBaseEdit edit)
        {
            base.AssignToEditCore(edit);

            var textEdit = edit as DevExpress.Xpf.Editors.BaseEdit;

            if (textEdit != null)
                textEdit.EditValueType = EditValueType;
        }
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
            if (sender is GridControl)
            {
                GridControl control = sender as GridControl;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(GridControl.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);
                ret.Add("ControlForeground", ret["Foreground"]);

                if (control.ReadLocalValue(GridControl.CellBorderColorProperty) != DependencyProperty.UnsetValue)
                    ret.Add("CellBorderColor", control.CellBorderColor);
                else
                    ret.Add("CellBorderColor", ThemeHelper.GetLightWeightThemeBrush((document as ScreenDocument).Theme, true));

                if (control.ReadLocalValue(GridControl.BackgroundProperty) != DependencyProperty.UnsetValue)
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
            if (value is bool)
                return value;
            var cvalue = (Brush)value;
            string prop = ((DependencyProperty)property).Name;
            GridControl gridControl = sender as GridControl;
            Brush defColor = (sender as GridControl).Foreground;
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            bool useDefColor = true;
            try
            {
                if (prop.Equals(GridControl.RowAreaForegroundProperty.Name))
                    defColor = ThemeHelper.GetLightWeightThemeBrush((document as ScreenDocument).Theme);
                else if (prop.Equals(GridControl.FocusedRowBackgroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, false);
                else if (prop.Equals(GridControl.FocusedCellBackgroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, false);
                else if (prop.Equals(GridControl.FocusedCellForegroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, true);
                else if (prop.Equals(GridControl.HeaderForegroundProperty.Name))
                    defColor = foreground;
                else if (prop.Equals(GridControl.HeaderBackgroundProperty.Name))
                    defColor = Brushes.Transparent;
                else if (prop.Equals(GridControl.ToolbarBackgroundProperty.Name))
                {
                    if (background is SolidColorBrush)
                    {
                        SolidColorBrush solidColorBrush = (background as SolidColorBrush);

                        defColor = new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast(solidColorBrush.Color, (document as ScreenDocument).Theme));
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
                        defColor = linearGradientBrush;
                    }
                    else 
                        defColor = background;
                }
                else if (prop.Equals(GridControl.ToolbarForegroundProperty.Name))
                    defColor = foreground;
                else
                    useDefColor = false;
            }
            catch (Exception)
            {
                return value;
            }

            Brush res = (sender as GridControl).GetUnsetPropertyValue<Brush>((DependencyProperty)property, cvalue, useDefColor ? defColor : null, false);
            return res;
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
