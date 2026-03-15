using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Converters;
using ViewModelLib;
using OPCUAViewModel;
using AlarmWindow.PopUp;
using Utilities;
using Utilities.WPF;
using System.Windows.Threading;
using UFInterfaces;
using ScreenSettings;
using DevExpress.Xpf.Grid;
using System.Text;
using DevExpress.Data.Filtering;
using System.Windows.Data;
using UFUAEditor.ComponentService;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using WPFUtilities;
using WPFUtilities.Extensions;
using VFS;
using GridLayout;
using System.Xml;
using System.Runtime.Serialization;
using WPFUtilities.PropertyDataTemplate;
using UFInterfaces.PropertyControl;
using UIMsgBoxAlertService.ComponentService;
using System.Globalization;
using System.Collections;
using System.Windows.Media.Animation;
using AlarmWindow.Commands;
using TranslationHelpers;
using System.Windows.Media.Imaging;
using UFInterfaces.Converters;
using AlarmWindow.Enums;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using DevExpress.Xpf.Editors.Settings;
using System.Xml.Serialization;
using DevExpress.Xpf.Bars.Themes;
using StorageHelper;


namespace AlarmWindow
{
    /// <summary>
    /// Interaction logic for GridAlarmWindow.xaml
    /// </summary>
    public partial class GridAlarmWindow : UserControl, IDisposable, IEntityReference, IContainPropertyEditors, ISettingsHelper, IDataErrorInfo
        , IGridLayoutUser
    {
        #region Dependency Properties

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }

        #region UTCSourceTimeStamp
        public static readonly DependencyProperty UTCSourceTimeStampProperty = DependencyProperty.Register("UTCSourceTimeStamp", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false));
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


        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(GridAlarmWindow), new UIPropertyMetadata(TextWrapping.NoWrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnTextWrappingChanged((TextWrapping)e.OldValue, (TextWrapping)e.NewValue);
        }

        protected virtual TextWrapping OnCoerceTextWrapping(TextWrapping value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextWrappingChanged(TextWrapping oldValue, TextWrapping newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TextWrapping TextWrapping
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextWrapping)GetValue(TextWrappingProperty);
            }
            set
            {
                SetValue(TextWrappingProperty, value);
            }
        }

        #endregion


        #region Background
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(GridAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(GridAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as GridAlarmWindow;
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
            var control = sender as GridAlarmWindow;
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

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as GridAlarmWindow;
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
            var control = sender as GridAlarmWindow;
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
            var control = sender as GridAlarmWindow;
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
            var control = sender as GridAlarmWindow;
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

        #region Style

        #region ControlForeground
        public static readonly DependencyProperty ControlForegroundProperty = DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(GridAlarmWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(OnControlForegroundChanged), new CoerceValueCallback(OnCoerceControlForeground)));

        private static object OnCoerceControlForeground(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceControlForeground((Brush)value);
            else
                return value;
        }

        private static void OnControlForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
        public static readonly DependencyProperty ToolbarBackgroundProperty = DependencyProperty.Register("ToolbarBackground", typeof(Brush), typeof(GridAlarmWindow), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x36, 0x36, 0x36))));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("AlarmWindowStyle")]
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
        public static readonly DependencyProperty ToolbarForegroundProperty = DependencyProperty.Register("ToolbarForeground", typeof(Brush), typeof(GridAlarmWindow), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("AlarmWindowStyle")]
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

        private void UpdateControlLayout()
        {
            if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    gridControl.Columns.ToList().ForEach(x =>
                    {
                        x.HeaderStyle = gridControl.TryFindResource("columnStyle") as Style;
                    });
                });
            }
            else if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    gridControl.Columns.ToList().ForEach(x =>
                    {
                        x.HeaderStyle = null;
                    });
                });
            }

            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                LoadBackContent();
            }

            //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    toolbartray.Background = ToolbarBackground;
            //    toolbarSettings.Background = ToolbarBackground;
            //    commands.Background = ToolbarBackground;
            //    server.Background = ToolbarBackground;
            //    status.Background = ToolbarBackground;
            //}
            //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
            //{
            //    actualSettings.Foreground = ToolbarForeground;
            //    configMemory.Foreground = ToolbarForeground;
            //    servertitle.Foreground = ToolbarForeground;
            //    qualitytitle.Foreground = ToolbarForeground;
            //    title.Foreground = ToolbarForeground;
            //    quality.Foreground = ToolbarForeground;
            //    ButtonUnsvelved.Foreground = ButtonExpandAll.Foreground = ButtonCollapseAll.Foreground = bestFit.Foreground = ToolbarForeground;
            //    bestFit.Foreground = ToolbarForeground;
            //}
            //gridControl.ApplyTemplate();
        }
        bool bCTLoaded;
        private void LoadBackContent()
        {
            if (!bCTLoaded)
            {
                try
                {
                    bCTLoaded = true;
                    Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                              String.Format("{0}.ControlTemplate.{1}", typeof(GridAlarmWindow).Namespace, "ControlTemplate.xaml"));
                    ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                    if (obj == null)
                        return;
                    gridControl.Resources.MergedDictionaries.Add(obj);
                }
                catch (Exception)
                {
                    bCTLoaded = false;
                }
            }

            //(from c in view.GetVisualChildrenOfType<ScrollContentPresenter>()
            //     where c.Name == "PART_ScrollContentPresenter"
            //     select c).ToList().ForEach(child =>
            //     {
            //         (from c in child.GetVisualChildrenOfType<DataPresenter>()
            //          where c.Name == "dataPresenter"
            //          select c).ToList().ForEach(d =>
            //          {
            //              (from c in d.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
            //               select c).ToList().ForEach(border =>
            //               {
            //                   border.Background = Background;
            //               });
            //          });
            //     });

        }

        #endregion
        #region CommandDimension
        public static readonly DependencyProperty CommandDimensionProperty = DependencyProperty.Register("CommandDimension", typeof(Dimensions), typeof(GridAlarmWindow), new UIPropertyMetadata(Dimensions.Small, new PropertyChangedCallback(OnCommandDimensionChanged), new CoerceValueCallback(OnCoerceCommandDimension)));

        private static object OnCoerceCommandDimension(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceCommandDimension((Dimensions)value);
            else
                return value;
        }

        private static void OnCommandDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnCommandDimensionChanged((Dimensions)e.OldValue, (Dimensions)e.NewValue);
        }

        protected virtual Dimensions OnCoerceCommandDimension(Dimensions value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCommandDimensionChanged(Dimensions oldValue, Dimensions newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("AlarmWindowStyle")]
        public Dimensions CommandDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Dimensions)GetValue(CommandDimensionProperty);
            }
            set
            {
                SetValue(CommandDimensionProperty, value);
            }
        }
            
        
        #endregion
        #region CommandPosition
        public static readonly DependencyProperty SpecificCommandPositionProperty = DependencyProperty.Register("SpecificCommandPosition", typeof(Dock), typeof(GridAlarmWindow), new UIPropertyMetadata(Dock.Bottom, new PropertyChangedCallback(OnSpecificCommandPositionChanged), new CoerceValueCallback(OnCoerceSpecificCommandPosition)));

            private static object OnCoerceSpecificCommandPosition(DependencyObject o, object value)
            {
                GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
                if (gridAlarmWindow != null)
                    return gridAlarmWindow.OnCoerceSpecificCommandPosition((Dock)value);
                else
                    return value;
            }

            private static void OnSpecificCommandPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
                if (gridAlarmWindow != null)
                    gridAlarmWindow.OnSpecificCommandPositionChanged((Dock)e.OldValue, (Dock)e.NewValue);
            }

            protected virtual Dock OnCoerceSpecificCommandPosition(Dock value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnSpecificCommandPositionChanged(Dock oldValue, Dock newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
                SetGridDimension(newValue);
                InitCommandPanel(newValue);
            }

            private void InitCommandPanel(Dock newValue)
            {
                switch (newValue)
                {
                    case Dock.Left:
                        horizontalPanel.Visibility = Visibility.Collapsed;
                        verticalPanel.Visibility = Visibility.Visible;
                        break;
                    case Dock.Right:
                        horizontalPanel.Visibility = Visibility.Collapsed;
                        verticalPanel.Visibility = Visibility.Visible;
                        break;
                    case Dock.Top:
                        horizontalPanel.Visibility = Visibility.Visible;
                        verticalPanel.Visibility = Visibility.Collapsed;
                        break;
                    case Dock.Bottom:
                        horizontalPanel.Visibility = Visibility.Visible;
                        verticalPanel.Visibility = Visibility.Collapsed;
                        break;
                }
            }

            private void SetGridDimension(Dock newValue)
            {
                gridControl.ClearValue(FrameworkElement.WidthProperty);
                gridControl.ClearValue(FrameworkElement.HeightProperty);
                gridControl.ClearValue(FrameworkElement.MinWidthProperty);
                gridControl.ClearValue(FrameworkElement.MinHeightProperty);
                gridControl.ClearValue(FrameworkElement.MaxWidthProperty);
                gridControl.ClearValue(FrameworkElement.MaxHeightProperty);

                //var headerarea = (Color)TryFindResource("HeaderBackgroundAreaColor");
                //if (headerarea != null)
                //    headerarea = HeaderBackground;

                return;
            }


            [Category("AlarmWindowStyle")]
            [DefaultValue(typeof(Dock), "Bottom")]
            public Dock SpecificCommandPosition
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (Dock)GetValue(SpecificCommandPositionProperty);
                }
                set
                {
                    SetValue(SpecificCommandPositionProperty, value);
                }
            }
            #endregion

        #region AuthomaticColumnLayout
        public static readonly DependencyProperty AuthomaticColumnLayoutProperty = DependencyProperty.Register("AuthomaticColumnLayout", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAuthomaticColumnLayoutChanged), new CoerceValueCallback(OnCoerceAuthomaticColumnLayout)));

        private static object OnCoerceAuthomaticColumnLayout(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAuthomaticColumnLayout((Boolean)value);
            else
                return value;
        }

        private static void OnAuthomaticColumnLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAuthomaticColumnLayoutChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAuthomaticColumnLayout(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAuthomaticColumnLayoutChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            bestFit.IsEnabled = newValue;
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        public Boolean AuthomaticColumnLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AuthomaticColumnLayoutProperty);
            }
            set
            {
                SetValue(AuthomaticColumnLayoutProperty, value);
            }
        }
        #endregion

        #region AllowResizing
        public static readonly DependencyProperty AllowResizingProperty = DependencyProperty.Register("AllowResizing", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowResizingChanged), new CoerceValueCallback(OnCoerceAllowResizing)));

        private static object OnCoerceAllowResizing(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAllowResizing((Boolean)value);
            else
                return value;
        }

        private static void OnAllowResizingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAllowResizingChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAllowResizing(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowResizingChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        public Boolean AllowResizing
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AllowResizingProperty);
            }
            set
            {
                SetValue(AllowResizingProperty, value);
            }
        }
        #endregion


        #region ShowServerTitle
        public static readonly DependencyProperty ShowServerTitleProperty = DependencyProperty.Register("ShowServerTitle", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowServerTitleChanged), new CoerceValueCallback(OnCoerceShowServerTitle)));

        private static object OnCoerceShowServerTitle(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowServerTitle((bool)value);
            else
                return value;
        }

        private static void OnShowServerTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnShowServerTitleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowServerTitle(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowServerTitleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateCommandVisibility();
        }
        [Category("AlarmWindowStyle")]
        public bool ShowServerTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowServerTitleProperty);
            }
            set
            {
                SetValue(ShowServerTitleProperty, value);
            }
        }

        #endregion

        #region ShowQualityStatus
        public static readonly DependencyProperty ShowQualityStatusProperty = DependencyProperty.Register("ShowQualityStatus", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowQualityStatusChanged), new CoerceValueCallback(OnCoerceShowQualityStatus)));

        private static object OnCoerceShowQualityStatus(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowQualityStatus((bool)value);
            else
                return value;
        }

        private static void OnShowQualityStatusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnShowQualityStatusChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowQualityStatus(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowQualityStatusChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateCommandVisibility();
        }
        [Category("AlarmWindowStyle")]
        public bool ShowQualityStatus
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowQualityStatusProperty);
            }
            set
            {
                SetValue(ShowQualityStatusProperty, value);
            }
        }

        #endregion

        #region ShowBestFitButton
        public static readonly DependencyProperty ShowBestFitButtonProperty = DependencyProperty.Register("ShowBestFitButton", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowBestFitButtonChanged), new CoerceValueCallback(OnCoerceShowBestFitButton)));

        private static object OnCoerceShowBestFitButton(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowBestFitButton((bool)value);
            else
                return value;
        }

        private static void OnShowBestFitButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
            UpdateCommandVisibility();
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


        #region ShowSettingButtons
        //keep for retrocompatibility
        [Browsable(false)]
        public bool ShowSettingButtons { get; set; }
        #endregion

        #region ShowUshelvedAlaramsButton
        public static readonly DependencyProperty ShowUshelvedAlaramsButtonProperty = DependencyProperty.Register("ShowUshelvedAlaramsButton", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowUshelvedAlaramsButtonChanged), new CoerceValueCallback(OnCoerceShowUshelvedAlaramsButton)));

        private static object OnCoerceShowUshelvedAlaramsButton(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowUshelvedAlaramsButton((bool)value);
            else
                return value;
        }

        private static void OnShowUshelvedAlaramsButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnShowUshelvedAlaramsButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowUshelvedAlaramsButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowUshelvedAlaramsButtonChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateCommandVisibility();
        }
        [Category("AlarmWindowStyle")]
        public bool ShowUshelvedAlaramsButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowUshelvedAlaramsButtonProperty);
            }
            set
            {
                SetValue(ShowUshelvedAlaramsButtonProperty, value);
            }
        }

        #endregion

        #region ShowExpandCollapseButtons
        public static readonly DependencyProperty ShowExpandCollapseButtonsProperty = DependencyProperty.Register("ShowExpandCollapseButtons", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowExpandCollapseButtonsChanged), new CoerceValueCallback(OnCoerceShowExpandCollapseButtons)));

        private static object OnCoerceShowExpandCollapseButtons(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowExpandCollapseButtons((bool)value);
            else
                return value;
        }

        private static void OnShowExpandCollapseButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnShowExpandCollapseButtonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowExpandCollapseButtons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowExpandCollapseButtonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateCommandVisibility();
        }
        [Category("AlarmWindowStyle")]
        public bool ShowExpandCollapseButtons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowExpandCollapseButtonsProperty);
            }
            set
            {
                SetValue(ShowExpandCollapseButtonsProperty, value);
            }
        }

        #endregion

        #region ShowGroupPanel
        public static readonly DependencyProperty ShowGroupPanelProperty = DependencyProperty.Register("ShowGroupPanel", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowGroupPanelChanged), new CoerceValueCallback(OnCoerceShowGroupPanel)));

        private static object OnCoerceShowGroupPanel(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceShowGroupPanel((bool)value);
            else
                return value;
        }

        private static void OnShowGroupPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnShowGroupPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowGroupPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowGroupPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            view.ShowGroupPanel = newValue;
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
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
            view.ShowSearchPanelMode = newValue ? ShowSearchPanelMode.Always : ShowSearchPanelMode.Never;
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
        public static readonly DependencyProperty ShowFilterPanelProperty = DependencyProperty.Register("ShowFilterPanel", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowFilterPanelChanged), new CoerceValueCallback(OnCoerceShowFilterPanel)));

        private static object OnCoerceShowFilterPanel(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceShowFilterPanel((bool)value);
            else
                return value;
        }

        private static void OnShowFilterPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
            view.ShowFilterPanelMode = newValue ? ShowFilterPanelMode.ShowAlways : ShowFilterPanelMode.Never;
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
            

        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));

        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
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

        [Category("AlarmWindowStyle")]
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
        #region AckCmdButton
        public static readonly DependencyProperty AckCmdButtonProperty = DependencyProperty.Register("AckCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAckCmdButtonChanged), new CoerceValueCallback(OnCoerceAckCmdButton)));

        private static object OnCoerceAckCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAckCmdButton((Boolean)value);
            else
                return value;
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        private static void OnAckCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAckCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAckCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAckCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean AckCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AckCmdButtonProperty);
            }
            set
            {
                SetValue(AckCmdButtonProperty, value);
            }
        }
        #endregion
        #region AckAllCmdButton
        public static readonly DependencyProperty AckAllCmdButtonProperty = DependencyProperty.Register("AckAllCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAckAllCmdButtonChanged), new CoerceValueCallback(OnCoerceAckAllCmdButton)));

        private static object OnCoerceAckAllCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAckAllCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnAckAllCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAckAllCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAckAllCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAckAllCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean AckAllCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AckAllCmdButtonProperty);
            }
            set
            {
                SetValue(AckAllCmdButtonProperty, value);
            }
        }
        #endregion

        #region ConfirmCmdButton
        public static readonly DependencyProperty ConfirmCmdButtonProperty = DependencyProperty.Register("ConfirmCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnConfirmCmdButtonChanged), new CoerceValueCallback(OnCoerceConfirmCmdButton)));

        private static object OnCoerceConfirmCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceConfirmCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnConfirmCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnConfirmCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceConfirmCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConfirmCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean ConfirmCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ConfirmCmdButtonProperty);
            }
            set
            {
                SetValue(ConfirmCmdButtonProperty, value);
            }
        }
        #endregion
        #region ConfirmAllCmdButton
        public static readonly DependencyProperty ConfirmAllCmdButtonProperty = DependencyProperty.Register("ConfirmAllCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnConfirmAllCmdButtonChanged), new CoerceValueCallback(OnCoerceConfirmAllCmdButton)));

        private static object OnCoerceConfirmAllCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceConfirmAllCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnConfirmAllCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnConfirmAllCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceConfirmAllCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConfirmAllCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean ConfirmAllCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ConfirmAllCmdButtonProperty);
            }
            set
            {
                SetValue(ConfirmAllCmdButtonProperty, value);
            }
        }
        #endregion

        #region RefreshCmdButton
        public static readonly DependencyProperty RefreshCmdButtonProperty = DependencyProperty.Register("RefreshCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRefreshCmdButtonChanged), new CoerceValueCallback(OnCoerceRefreshCmdButton)));

        private static object OnCoerceRefreshCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceRefreshCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnRefreshCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnRefreshCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRefreshCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRefreshCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean RefreshCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(RefreshCmdButtonProperty);
            }
            set
            {
                SetValue(RefreshCmdButtonProperty, value);
            }
        }
        #endregion

        #region ShelveCmdButton
        public static readonly DependencyProperty ShelveCmdButtonProperty = DependencyProperty.Register("ShelveCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShelveCmdButtonChanged), new CoerceValueCallback(OnCoerceShelveCmdButton)));

        private static object OnCoerceShelveCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceShelveCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnShelveCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnShelveCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShelveCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShelveCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean ShelveCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShelveCmdButtonProperty);
            }
            set
            {
                SetValue(ShelveCmdButtonProperty, value);
            }
        }
        #endregion
        #region EnableCmdButton
        //public static readonly DependencyProperty EnableCmdButtonProperty = DependencyProperty.Register("EnableCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableCmdButtonChanged), new CoerceValueCallback(OnCoerceEnableCmdButton)));

        //private static object OnCoerceEnableCmdButton(DependencyObject o, object value)
        //{
        //    GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
        //    if (gridAlarmWindow != null)
        //        return gridAlarmWindow.OnCoerceEnableCmdButton((Boolean)value);
        //    else
        //        return value;
        //}

        //private static void OnEnableCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
        //    if (gridAlarmWindow != null)
        //        gridAlarmWindow.OnEnableCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        //}

        //protected virtual Boolean OnCoerceEnableCmdButton(Boolean value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnEnableCmdButtonChanged(Boolean oldValue, Boolean newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("AlarmWindowStyle")]
        //[DefaultValue(typeof(Boolean), "true")]
        //[Browsable(true)]
        //public Boolean EnableCmdButton
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (Boolean)GetValue(EnableCmdButtonProperty);
        //    }
        //    set
        //    {
        //        SetValue(EnableCmdButtonProperty, value);
        //    }
        //}
        #endregion

        #region DisableCmdButton
        //public static readonly DependencyProperty DisableCmdButtonProperty = DependencyProperty.Register("DisableCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnDisableCmdButtonChanged), new CoerceValueCallback(OnCoerceDisableCmdButton)));

        //private static object OnCoerceDisableCmdButton(DependencyObject o, object value)
        //{
        //    GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
        //    if (gridAlarmWindow != null)
        //        return gridAlarmWindow.OnCoerceDisableCmdButton((Boolean)value);
        //    else
        //        return value;
        //}

        //private static void OnDisableCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
        //    if (gridAlarmWindow != null)
        //        gridAlarmWindow.OnDisableCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        //}

        //protected virtual Boolean OnCoerceDisableCmdButton(Boolean value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnDisableCmdButtonChanged(Boolean oldValue, Boolean newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //}
        //[Category("AlarmWindowStyle")]
        //[DefaultValue(typeof(Boolean), "true")]
        //[Browsable(true)]
        //public Boolean DisableCmdButton
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (Boolean)GetValue(DisableCmdButtonProperty);
        //    }
        //    set
        //    {
        //        SetValue(DisableCmdButtonProperty, value);
        //    }
        //}
        #endregion

        #region AddCommentCmdButton
        public static readonly DependencyProperty AddCommentCmdButtonProperty = DependencyProperty.Register("AddCommentCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAddCommentCmdButtonChanged), new CoerceValueCallback(OnCoerceAddCommentCmdButton)));

        private static object OnCoerceAddCommentCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAddCommentCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnAddCommentCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAddCommentCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceAddCommentCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAddCommentCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean AddCommentCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(AddCommentCmdButtonProperty);
            }
            set
            {
                SetValue(AddCommentCmdButtonProperty, value);
            }
        }
            
        #endregion

        #region SoundCmdButton
        public static readonly DependencyProperty SoundCmdButtonProperty = DependencyProperty.Register("SoundCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnSoundCmdButtonChanged), new CoerceValueCallback(OnCoerceSoundCmdButton)));

        private static object OnCoerceSoundCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceSoundCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnSoundCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnSoundCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceSoundCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSoundCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean SoundCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(SoundCmdButtonProperty);
            }
            set
            {
                SetValue(SoundCmdButtonProperty, value);
            }
        }
        #endregion

        #region ExecuteCmdButton
        public static readonly DependencyProperty ExecuteCmdButtonProperty = DependencyProperty.Register("ExecuteCmdButton", typeof(Boolean), typeof(GridAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnExecuteCmdButtonChanged), new CoerceValueCallback(OnCoerceExecuteCmdButton)));

        private static object OnCoerceExecuteCmdButton(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceExecuteCmdButton((Boolean)value);
            else
                return value;
        }

        private static void OnExecuteCmdButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnExecuteCmdButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceExecuteCmdButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnExecuteCmdButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(Boolean), "true")]
        [Browsable(true)]
        public Boolean ExecuteCmdButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ExecuteCmdButtonProperty);
            }
            set
            {
                SetValue(ExecuteCmdButtonProperty, value);
            }
        }
        #endregion

        #region AllowDropColumns
        public static readonly DependencyProperty AllowDropColumnsProperty = DependencyProperty.Register("AllowDropColumns", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowDropColumnsChanged), new CoerceValueCallback(OnCoerceAllowDropColumns)));

        private static object OnCoerceAllowDropColumns(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAllowDropColumns((bool)value);
            else
                return value;
        }

        private static void OnAllowDropColumnsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnAllowDropColumnsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        [Category("AlarmWindowStyle")]
        [DefaultValue(typeof(bool), "false")]
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

       #endregion

        #region Advanced
		#region FilterCriteria
        public static readonly DependencyProperty FilterCriteriaProperty = DependencyProperty.Register("FilterCriteria", typeof(string), typeof(GridAlarmWindow), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFilterCriteriaChanged), new CoerceValueCallback(OnCoerceFilterCriteria)));

        private static object OnCoerceFilterCriteria(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceFilterCriteria((string)value);
            else
                return value;
        }

        private static void OnFilterCriteriaChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnFilterCriteriaChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFilterCriteria(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilterCriteriaChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                bChangeCriteria = true;
                gridControl.FilterCriteria = CriteriaOperator.Parse(newValue);
                //UpdateUnshelved();
                UpdateUnshelveButton();
                bChangeCriteria = false;
            }
            catch (Exception ex)
            {
                bChangeCriteria = false;
            }
        }
        [Category("Advanced")]
        [Browsable(false)]
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
                SetValue(FilterCriteriaProperty, value);
            }
        }

        #endregion

        #region DisableBlinkAnimation
        public static readonly DependencyProperty DisableBlinkAnimationProperty = DependencyProperty.Register("DisableBlinkAnimation", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnDisableBlinkAnimationChanged), new CoerceValueCallback(OnCoerceDisableBlinkAnimation)));

        private static object OnCoerceDisableBlinkAnimation(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceDisableBlinkAnimation((bool)value);
            else
                return value;
        }

        private static void OnDisableBlinkAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnDisableBlinkAnimationChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceDisableBlinkAnimation(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisableBlinkAnimationChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public bool DisableBlinkAnimation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(DisableBlinkAnimationProperty);
            }
            set
            {
                SetValue(DisableBlinkAnimationProperty, value);
            }
        }

        #endregion


        #region CommentRequiredForSeverity
        public static readonly DependencyProperty CommentRequiredForSeverityProperty = DependencyProperty.Register("CommentRequiredForSeverity", typeof(double), typeof(GridAlarmWindow), new UIPropertyMetadata(0.0));
        [Category("Advanced")]
        [Browsable(false)]
        public double CommentRequiredForSeverity{ get; set; }
        #endregion

        #region SeverityRequiredForComment
        public static readonly DependencyProperty SeverityRequiredForCommentProperty = DependencyProperty.Register("SeverityRequiredForComment", typeof(int), typeof(GridAlarmWindow), new UIPropertyMetadata(0));
        [Category("Advanced")]
        public int SeverityRequiredForComment
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SeverityRequiredForCommentProperty);
            }
            set
            {
                SetValue(SeverityRequiredForCommentProperty, value);
            }
        }
        #endregion


        #region AlarmAreaSelectedBackground
        //keep for retrocompatibility
        [Browsable(false)]
        public Color AlarmAreaSelectedBackground { get; set; }
        #endregion

        #region AlarmAreaSelectedForeground
        //keep for retrocompatibility
        [Browsable(false)]
        public Color AlarmAreaSelectedForeground { get; set; }
        #endregion

        #region AlarmAreaFontSettings
        public static readonly DependencyProperty AlarmAreaFontSettingsProperty = DependencyProperty.Register("AlarmAreaFontSettings", typeof(FontSettings), typeof(GridAlarmWindow), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnAlarmAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceAlarmAreaFontSettings)));

        private static object OnCoerceAlarmAreaFontSettings(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceAlarmAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnAlarmAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
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
            //UpdateValueFont(newValue);
            
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

        #region HeaderFontSettings
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(GridAlarmWindow), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
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
            UpdateValueFont(newValue);
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

        #region Thresholds

        public static readonly DependencyProperty ThresholdsProperty = DependencyProperty.Register("Thresholds", typeof(ThresholdList), typeof(GridAlarmWindow), new UIPropertyMetadata(new ThresholdList(), new PropertyChangedCallback(OnThresholdsChanged), new CoerceValueCallback(OnCoerceThresholds)));

        private static object OnCoerceThresholds(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceThresholds((ThresholdList)value);
            else
                return value;
        }

        private static void OnThresholdsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnThresholdsChanged((ThresholdList)e.OldValue, (ThresholdList)e.NewValue);
        }

        protected virtual ThresholdList OnCoerceThresholds(ThresholdList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnThresholdsChanged(ThresholdList oldValue, ThresholdList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //newValue.OrderBy(t => t.ThresholdValue);
        }

        [Category("Advanced")]
        [SvgValueConverter(typeof(ConvertThresholdList))]
        public ThresholdList Thresholds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ThresholdList)GetValue(ThresholdsProperty);
            }
            set
            {
                SetValue(ThresholdsProperty, value);
            }
        }

        #endregion


        #region BranchColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String BranchColumnName { get; set; }
        #endregion

        #region ActionsColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public string ActionsColumnName { get; set; }
        #endregion

        #region CommentColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String CommentColumnName { get; set; }
        #endregion

        #region UserTempColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public string UserTempColumnName { get; set; }
        #endregion
        #region StateColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String StateColumnName { get; set; }
        #endregion


        #region ReasonColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String ReasonColumnName { get; set; }
        #endregion


        #region SourceColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String SourceColumnName { get; set; }
        #endregion

        #region ConditionColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String ConditionColumnName { get; set; }
        #endregion

        #region SeverityColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String SeverityColumnName { get; set; }
        #endregion

        #region TimeColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String TimeColumnName { get; set; }
        #endregion

        #region ActiveTransitionTimeColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String ActiveTransitionTimeColumnName { get; set; }
        #endregion


        #region AckedTransitionTimeColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String AckedTransitionTimeColumnName { get; set; }
        #endregion


        #region ConfirmedTransitionTimeColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String ConfirmedTransitionTimeColumnName { get; set; }
        #endregion



        #region MessageColumnName
        //keep for retrocompatibility
        [Browsable(false)]
        public String MessageColumnName { get; set; }
        #endregion


        #region HorizontalColumnAlignment
        public static readonly DependencyProperty HorizontalColumnAlignmentProperty = DependencyProperty.Register("HorizontalColumnAlignment", typeof(HorizontalAlignment), typeof(GridAlarmWindow), new UIPropertyMetadata(HorizontalAlignment.Left, new PropertyChangedCallback(OnHorizontalColumnAlignmentChanged), new CoerceValueCallback(OnCoerceHorizontalColumnAlignment)));

        private static object OnCoerceHorizontalColumnAlignment(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceHorizontalColumnAlignment((HorizontalAlignment)value);
            else
                return value;
        }

        private static void OnHorizontalColumnAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnHorizontalColumnAlignmentChanged((HorizontalAlignment)e.OldValue, (HorizontalAlignment)e.NewValue);
        }

        protected virtual HorizontalAlignment OnCoerceHorizontalColumnAlignment(HorizontalAlignment value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHorizontalColumnAlignmentChanged(HorizontalAlignment oldValue, HorizontalAlignment newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public HorizontalAlignment HorizontalColumnAlignment
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (HorizontalAlignment)GetValue(HorizontalColumnAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalColumnAlignmentProperty, value);
            }
        }

        #endregion


        #endregion
        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(GridAlarmWindow), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                return gridAlarmWindow.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow gridAlarmWindow = o as GridAlarmWindow;
            if (gridAlarmWindow != null)
                gridAlarmWindow.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
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
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    gridControl.SaveLayoutToStream(output);
                    GridLayout = utf8noBOM.GetString(output.ToArray());
                }
            }
            catch (Exception)
            {
            }
        }

        void LoadDesignGridLayout()
        {
            try
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
            catch
            {
                GridLayout = string.Empty;
            }
        }

        [Browsable(false)]
        [Category("AlarmWindowStyle")]
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
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEditableChanged), new CoerceValueCallback(OnCoerceEditable)));

        private static object OnCoerceEditable(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceEditable((bool)value);
            else
                return value;
        }

        private static void OnEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
            UpdateCommandVisibility();
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

        #region ChildAlarmsConnected
        public static readonly DependencyProperty ConnectChildAlarmsProperty = DependencyProperty.Register("ConnectChildAlarms", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnConnectChildAlarmsChanged), new CoerceValueCallback(OnCoerceConnectChildAlarms)));

        private static object OnCoerceConnectChildAlarms(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceConnectChildAlarms((bool)value);
            else
                return value;
        }


        private static void OnConnectChildAlarmsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                control.OnConnectChildAlarmsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceConnectChildAlarms(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConnectChildAlarmsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Advanced")]
        public bool ConnectChildAlarms
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ConnectChildAlarmsProperty);
            }
            set
            {
                SetValue(ConnectChildAlarmsProperty, value);
            }
        }

        #endregion


        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(GridAlarmWindow), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessLevelChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessLevel)));

        private static object OnCoerceEditingWriteAccessLevel(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceEditingWriteAccessLevel((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(GridAlarmWindow), new UIPropertyMetadata(0, new PropertyChangedCallback(OnEditingWriteAccessMaskChanged), new CoerceValueCallback(OnCoerceEditingWriteAccessMask)));

        private static object OnCoerceEditingWriteAccessMask(DependencyObject o, object value)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
            if (control != null)
                return control.OnCoerceEditingWriteAccessMask((int)value);
            else
                return value;
        }

        private static void OnEditingWriteAccessMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow control = o as GridAlarmWindow;
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


        #region ClientTimezoneOffset
        public static readonly DependencyProperty ClientTimezoneOffsetProperty = DependencyProperty.Register("ClientTimezoneOffset", typeof(double), typeof(GridAlarmWindow), new UIPropertyMetadata(0.0));
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUserBasedRuntimeSettingsChanged), new CoerceValueCallback(OnCoerceUserBasedRuntimeSettings)));

        private static object OnCoerceUserBasedRuntimeSettings(DependencyObject o, object value)
        {
            GridAlarmWindow GridAlarmWindow = o as GridAlarmWindow;
            if (GridAlarmWindow != null)
                return GridAlarmWindow.OnCoerceUserBasedRuntimeSettings((bool)value);
            else
                return value;
        }

        private static void OnUserBasedRuntimeSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow GridAlarmWindow = o as GridAlarmWindow;
            if (GridAlarmWindow != null)
                GridAlarmWindow.OnUserBasedRuntimeSettingsChanged((bool)e.OldValue, (bool)e.NewValue);
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

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false));

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

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(GridAlarmWindow), new UIPropertyMetadata(false));

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
        public static readonly DependencyProperty CurrentCultureProperty = DependencyProperty.Register("CurrentCulture", typeof(CultureInfo), typeof(GridAlarmWindow));
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
        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(GridAlarmWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDateTimeFormatChanged), new CoerceValueCallback(OnCoerceDateTimeFormat)));

        private static object OnCoerceDateTimeFormat(DependencyObject o, object value)
        {
            GridAlarmWindow GridAlarmWindow = o as GridAlarmWindow;
            if (GridAlarmWindow != null)
                return GridAlarmWindow.OnCoerceDateTimeFormat((string)value);
            else
                return value;
        }

        private static void OnDateTimeFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GridAlarmWindow GridAlarmWindow = o as GridAlarmWindow;
            if (GridAlarmWindow != null)
                GridAlarmWindow.OnDateTimeFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDateTimeFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDateTimeFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string DateTimeFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DateTimeFormatProperty);
            }
            set
            {
                SetValue(DateTimeFormatProperty, value);
            }
        }
        #endregion

        [Browsable(false)]
        public bool IsNotInDesignMode
        {
            get
            {
                return (!bDesignmode && DesignerProperties.GetIsInDesignMode(this));
            }
        }
        #endregion

        #region Declarations
        bool bDesignmode;
        bool bInit;
        bool bLoaded;
        bool bExecuted;
        internal bool bSmartSettingsEditing;
        internal IDocument Document;
        
        IStringEditorManager stringManager;
        IUFUAEditorManager ufuaEditorManager;

        MonitoredItemViewModel monitoredItemViewModel;
        OPCUAEntityReference EReload;

        readonly Dictionary<OPCUAEntityReference, MonitoredItemViewModel> childsMonitoredItemViewModel = new Dictionary<OPCUAEntityReference, MonitoredItemViewModel>();
        readonly Dictionary<String, CommandsExecuterHelper> commandsExecuter = new Dictionary<String, CommandsExecuterHelper>();

        ObservableCollection<ConditionStateViewModel> conditionStateList;
        List<NotifyCollectionChangedEventArgs> notifyCollectionChangesList = new List<NotifyCollectionChangedEventArgs>();

        Setting defSetting;
        string resetGridLayout;
        IQualityToStringConverter qualityToStringConverter;

        ConnectionState connectionState = ConnectionState.Undefined;

        DispatcherOperation dpUpdateGridStatus;
        DispatcherOperation dpUpdateGridAlarm;
        DispatcherOperation dpCollectionChanged;

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

        [Browsable(false)]
        public string Title
        {
            get
            {
                if (monitoredItemViewModel != null &&
                    monitoredItemViewModel.GetSubscriptionViewModelParent() != null &&
                    monitoredItemViewModel.GetSubscriptionViewModelParent().GetSessionViewModelParent() != null)
                    return monitoredItemViewModel.GetSubscriptionViewModelParent().GetSessionViewModelParent().Title;
                else if (monitoredItemViewModel != null)
                    return monitoredItemViewModel.Title;
                else
                    return String.Empty;
            }
        }

        #region Commands
        RelayCommand _unShelveCommand;
        public ICommand UnShelveCommand
        {
            get
            {
                if (_unShelveCommand == null)
                {
                    _unShelveCommand = new RelayCommand(
                        param => CallUnShelveCommand(),
                        param => IsEnableCallUnShelveCommand
                        );
                }
                return _unShelveCommand;
            }
        }

        bool bCallingUnShelveCommand;
        internal void CallUnShelveCommand()
        {
            try
            {
                bCallingUnShelveCommand = true;

                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null && SelectedRows.Count > 0)
                {
                    foreach (var focusedRow in SelectedRows)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        if (item != null)
                            item.UnshelveCommand.Execute(null);
                    }
                }
            }
            finally
            {
                bCallingUnShelveCommand = false;
            }
        }

        internal bool IsEnableCallUnShelveCommand
        {
            get
            {
                if (bCallingUnShelveCommand)
                    return false;
                else
                {
                    var SelectedRows = gridControl.SelectedItems;
                    if (SelectedRows != null && SelectedRows.Count > 0)
                    {
                        foreach (var focusedRow in SelectedRows)
                        {
                            var item = focusedRow as ConditionStateViewModel;
                            if (item == null || !item.UnshelveCommand.CanExecute(null))
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }

            }
        }


        RelayCommand _shelveCommand;
        public ICommand ShelveCommand
        {
            get
            {
                if (_shelveCommand == null)
                {
                    _shelveCommand = new RelayCommand(
                        param => CallShelveCommand(),
                        param => IsEnableCallShelveCommand
                        );
                }
                return _shelveCommand;
            }
        }

        bool bCallingShelveCommand;
        internal void CallShelveCommand()
        {
            try
            {
                bCallingShelveCommand = true;

                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null && gridControl.CurrentItem != null)
                {
                    foreach (var focusedRow in SelectedRows)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        if (item != null)
                        {
                            item.ShelvingTime = (gridControl.CurrentItem as ConditionStateViewModel).ShelvingTime;
                            item.ShelveCommand.Execute(null);
                        }
                    }
                }
            }
            finally
            {
                bCallingShelveCommand = false;
            }
        }

        internal bool IsEnableCallShelveCommand
        {
            get
            {
                if (bCallingShelveCommand)
                    return false;
                else
                {
                    var SelectedRows = gridControl.SelectedItems;
                    if (SelectedRows != null && SelectedRows.Count > 0)
                    {
                        foreach (var focusedRow in SelectedRows)
                        {
                            var item = focusedRow as ConditionStateViewModel;
                            if (item == null || !item.ShelveCommand.CanExecute(null))
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }

            }
        }


        RelayCommand _acknowledgeCommand;
        public ICommand AcknowledgeCommand
        {
            get
            {
                if (_acknowledgeCommand == null)
                {
                    _acknowledgeCommand = new RelayCommand(
                        param => CallAcknowledgeCommand(),
                        param => IsEnableCallAcknowledgeCommand
                        );
                }
                return _acknowledgeCommand;
            }
        }

        bool bCallingAcknowledgeCommand;
        internal void CallAcknowledgeCommand()
        {
            bCallingAcknowledgeCommand = true;

            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            int severity = CommentRequiredForSeverity >= 1 && SeverityRequiredForComment == 0 ? (int)CommentRequiredForSeverity : SeverityRequiredForComment;
            using (var cursor = new WaitCursor())
            {
                var list = new List<ConditionStateViewModel>();
                var acklist = new List<ConditionStateViewModel>();
                                        
                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null)
                {
                    foreach (var focusedRow in SelectedRows)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        if (item != null)
                            list.Add(item);
                    }
                }
                string _comment = string.Empty;
                if (severity > 0 && !RunningOnServer && (from a in list where a.Severity >= severity && string.IsNullOrEmpty(a.UserTempComment.ToString()) && a.IsEnableCallAcknowledge select a).ToList().Count > 0)
                {
                    var insertComment = new AddComment(true) { UserTempComment = string.Empty };
                    var dialog = new GeneralDialog(insertComment) { Title = Properties.Resources.AddCommentCmd, bShowHelp = false };
                    if (dialog.ShowDialog() == true)
                    {
                        _comment = insertComment.UserTempComment;
                    }
                }
                list.ForEach(v =>
                {
                    try
                    {
                        var ret = OnAckingEvent(v);
                        if (ret)
                            acklist.Add(v);
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
                {
                    try
                    {
                        acklist.ForEach(v =>
                        {
                            try
                            {
                                if (severity > 0 && v.Severity >= severity && string.IsNullOrEmpty(v.UserTempComment.ToString()))
                                {
                                    if (!string.IsNullOrEmpty(_comment) && v.IsEnableCallAcknowledge)
                                    {
                                        v.UserTempComment = _comment;
                                        v.CallAcknowledge(true);
                                    }
                                }
                                else
                                {
                                    if (v.IsEnableCallAcknowledge)
                                        v.CallAcknowledge(true);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    acklist.ForEach(m => OnAckEvent(m));
                    bCallingAcknowledgeCommand = false;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        internal bool IsEnableCallAcknowledgeCommand
        {
            get
            {
                if (bCallingAcknowledgeCommand)
                    return false;
                else
                {
                    var SelectedRows = gridControl.SelectedItems;
                    if (SelectedRows != null && SelectedRows.Count > 0)
                    {
                        foreach (var focusedRow in SelectedRows)
                        {
                            var item = focusedRow as ConditionStateViewModel;
                            if (item == null || !item.AcknowledgeCommand.CanExecute(null))
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }

            }
        }

        RelayCommand _confirmCommand;
        public ICommand ConfirmCommand
        {
            get
            {
                if (_confirmCommand == null)
                {
                    _confirmCommand = new RelayCommand(
                        param => CallConfirmCommand(),
                        param => IsEnableCallConfirmCommand
                        );
                }
                return _confirmCommand;
            }
        }

        bool bCallingConfirmCommand;
        internal void CallConfirmCommand()
        {
            bCallingConfirmCommand = true;

            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            using (var cursor = new WaitCursor())
            {
                var list = new List<ConditionStateViewModel>();
                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null)
                {
                    foreach (var focusedRow in SelectedRows)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        if (item != null)
                            list.Add(item);
                    }
                }
                var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
                {
                    try
                    {
                        list.ForEach(v =>
                        {
                            try
                            {
                                if (v.IsEnableCallConfirm)
                                    v.CallConfirm(true);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    list.ForEach(m => OnConfirmEvent(m));
                    bCallingConfirmCommand = false;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        internal bool IsEnableCallConfirmCommand
        {
            get
            {
                if (bCallingConfirmCommand)
                    return false;
                else
                {
                    var SelectedRows = gridControl.SelectedItems;
                    if (SelectedRows != null && SelectedRows.Count > 0)
                    {
                        foreach (var focusedRow in SelectedRows)
                        {
                            var item = focusedRow as ConditionStateViewModel;
                            if (item == null || !item.ConfirmCommand.CanExecute(null))
                                return false;
                        }
                        return true;
                    }
                    else
                        return false;
                }

            }
        }

        RelayCommand _addAlarmComment;
        public ICommand AddAlarmComment
        {
            get
            {
                if (_addAlarmComment == null)
                {
                    _addAlarmComment = new RelayCommand(
                        param => CallAddAlarmComment(),
                        param => IsEnableCallAddAlarmComment
                        );
                }
                return _addAlarmComment;
            }
        }

        bool bCallingAddComment;
        internal void CallAddAlarmComment()
        {
            try
            {
                bCallingAddComment = true;
                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null && SelectedRows.Count > 0 && gridControl.CurrentItem != null)
                {
                    var insertComment = new AddComment(false) { UserTempComment = (gridControl.CurrentItem as ConditionStateViewModel).Comment };
                    var dialog = new GeneralDialog(insertComment) { Title = Properties.Resources.AddCommentCmd, bShowHelp = false };
                    if (dialog.ShowDialog() == true)
                    {
                        foreach (var focusedRow in SelectedRows)
                        {
                            var item = focusedRow as ConditionStateViewModel;
                            if (item != null)
                            {
                                item.UserTempComment = insertComment.UserTempComment;
                                item.AddCommentCommand.Execute(null);
                            }
                        }
                    }

                }
            }
            finally
            {
                bCallingAddComment = false;
            }
        }

        internal bool IsEnableCallAddAlarmComment
        {
            get
            {
                if (bCallingAddComment)
                    return false;
                else
                {
                    var focusedRow = gridControl.CurrentItem;
                    if(focusedRow != null)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        return item != null && item.AddCommentCommand.CanExecute(null);
                    }
                    else
                        return false;
                }

            }
        }

        RelayCommand executeAlarmCommands;
        public ICommand ExecuteAlarmCommands
        {
            get
            {
                if (executeAlarmCommands == null)
                {
                    executeAlarmCommands = new RelayCommand(
                        param => CallExecuteAlarmCommands(),
                        param => IsEnableCallExecuteAlarmCommands
                        );
                }
                return executeAlarmCommands;
            }
        }

        bool bCallingExecuteAlarmCommands;
        void CallExecuteAlarmCommands()
        {
            try
            {
                bCallingExecuteAlarmCommands = true;
                var SelectedRows = gridControl.SelectedItems;
                if (SelectedRows != null && SelectedRows.Count > 0 && gridControl.CurrentItem != null)
                {
                    foreach (var focusedRow in SelectedRows)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        if (item != null)
                            CallExecuteAlarmCommands(item);
                    }
                }
            }
            finally
            {
                bCallingExecuteAlarmCommands = false;
            }
        }

        void CallExecuteAlarmCommands(ConditionStateViewModel conditionState)
        {
            var monitoredItem = conditionState.Parent as MonitoredItemViewModel;
            var appName = monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().AppName;
            if (commandsExecuter.ContainsKey(appName) && commandsExecuter[appName].CanExecute(conditionState))
            {
                if (RunningOnServer)
                    commandsExecuter[appName].RemoteExecute(conditionState);
                else
                    commandsExecuter[appName].Execute(conditionState);
            }

        }

        bool IsEnableCallExecuteAlarmCommands
        {
            get
            {
                if (bCallingExecuteAlarmCommands)
                    return false;
                else
                {
                    var focusedRow = gridControl.CurrentItem;
                    if (focusedRow != null)
                    {
                        var item = focusedRow as ConditionStateViewModel;
                        return CanExecuteAlarmCommands(item);
                    }
                    else
                        return false;
                }
            }
        }

        bool CanExecuteAlarmCommands(ConditionStateViewModel conditionState)
        {
            var monitoredItem = conditionState.Parent as MonitoredItemViewModel;
            var appName = monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().AppName;
            if (commandsExecuter.ContainsKey(appName))
                return commandsExecuter[appName].CanExecute(conditionState);

            return false;
        }

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
            try
            {
                bCallingRefreshCommand = true;

                gridControl.SelectedItems.Clear();

                if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionStateList != null)
                    monitoredItemViewModel.ConditionRefreshCommand.Execute(null);

                foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                {
                    if (monitoredItem != null && monitoredItem.ConditionStateList != null)
                        monitoredItem.ConditionRefreshCommand.Execute(null);
                }
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
                {
                    if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionRefreshCommand.CanExecute(null))
                        return true;

                    foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                    {
                        if (monitoredItem != null && monitoredItem.ConditionRefreshCommand.CanExecute(null))
                            return true;
                    }

                    return false;
                }
            }
        }


        RelayCommand _disableSoundCommand;
        public ICommand DisableSoundCommand
        {
            get
            {
                if (_disableSoundCommand == null)
                    _disableSoundCommand = new RelayCommand(param => CallDisableSoundCommand(), param => IsEnableCallDisableSoundCommand);

                return _disableSoundCommand;
            }
        }

        bool bCallingDisableSoundCommand;
        internal void CallDisableSoundCommand()
        {
            try
            {
                bCallingDisableSoundCommand = true;
                if (silentSound.HasValue && eReloadMonitoredItemViewModel != null)
                {
                    if (eReloadMonitoredItemViewModel.NodeIdModel != null)
                        eReloadMonitoredItemViewModel.WriteValue(silentSound.Value);
                    else
                        SysVariables.SysVariables.GetSysVariables(Document).UpdateSysVariable(SysVariables.SysNames.AlarmSoundActiveOnClient, silentSound.Value);
                }
            }
            catch
            { }
            finally
            {
                bCallingDisableSoundCommand = false;
            }
        }

        internal bool IsEnableCallDisableSoundCommand
        {
            get
            {
                if (bCallingDisableSoundCommand)
                    return false;
                else
                    return eReloadMonitoredItemViewModel != null && eReloadMonitoredItemViewModel.DataValue != null;
            }
        }

        RelayCommand _ackAllCommand;
        public ICommand AckAllCommand
        {
            
            get
            {
                if (_ackAllCommand == null)
                    _ackAllCommand = new RelayCommand(param => CallAckAllCommand(), param => IsEnableCallAckAllCommand);                   
              
                return _ackAllCommand;
            }
        }

        bool bCallingAckAllCommand;

        internal void CallAckAllCommand()
        {
            bCallingAckAllCommand = true;
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            int severity = CommentRequiredForSeverity >= 1 && SeverityRequiredForComment == 0 ? (int)CommentRequiredForSeverity : SeverityRequiredForComment;

            using (var cursor = new WaitCursor())
            {
                var acklist = new List<ConditionStateViewModel>();

                List<ConditionStateViewModel> list = GetListConditionStateViewModel();
                if (list == null || list.Count == 0)
                {
                    bCallingAckAllCommand = false;
                    return;
                }

                string _comment = string.Empty;
                if (severity > 0 && !RunningOnServer && (from a in list where a.Severity >= severity && string.IsNullOrEmpty(a.UserTempComment.ToString()) && a.IsEnableCallAcknowledge select a).ToList().Count > 0)
                {
                    var insertComment = new AddComment(true) { UserTempComment = string.Empty };
                    var dialog = new GeneralDialog(insertComment) { Title = Properties.Resources.AddCommentCmd, bShowHelp = false };
                    if (dialog.ShowDialog() == true)
                    {
                        _comment = insertComment.UserTempComment;
                    }
                }
                list.ForEach(v =>
                {
                    try
                    {
                        var ret = OnAckingEvent(v);
                        if (ret)
                            acklist.Add(v);
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
                {
                    try
                    {
                        acklist.ForEach(v =>
                        {
                            try
                            {
                                if (severity > 0 && v.Severity >= severity && string.IsNullOrEmpty(v.UserTempComment.ToString()))
                                {
                                    if (!string.IsNullOrEmpty(_comment) && v.IsEnableCallAcknowledge)
                                    {
                                        v.UserTempComment = _comment;
                                        v.CallAcknowledge(true);
                                    }
                                }
                                else
                                {
                                    if (v.IsEnableCallAcknowledge)
                                        v.CallAcknowledge(true);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    acklist.ForEach(m => OnAckEvent(m));
                    bCallingAckAllCommand = false;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        internal bool IsEnableCallAckAllCommand
        {
            get
            {
                if (bCallingAckAllCommand)
                    return false;
                else
                {
                    if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionAcknowledgeAllCommand.CanExecute(null))
                        return true;

                    foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                    {
                        if (monitoredItem != null && monitoredItem.ConditionAcknowledgeAllCommand.CanExecute(null))
                            return true;
                    }

                    return false;
                }
            }
        }

        RelayCommand _conditionConfirmAllCommand;
        public ICommand ConditionConfirmAllCommand
        {
            get
            {
                if (_conditionConfirmAllCommand == null)
                    _conditionConfirmAllCommand = new RelayCommand(param => CallConditionConfirmAllCommand(), param => IsEnableCallConditionConfirmAllCommand);

                return _conditionConfirmAllCommand;
            }
        }

        bool bCallingConditionConfirmAllCommand;
        internal void CallConditionConfirmAllCommand()
        {
            bCallingConditionConfirmAllCommand = true;
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            using (var cursor = new WaitCursor())
            {
                List<ConditionStateViewModel> list = GetListConditionStateViewModel();
                if (list == null || list.Count == 0)
                {
                    bCallingConditionConfirmAllCommand = false;
                    return;
                }

                var task1 = System.Threading.Tasks.Task.Factory.StartNew(delegate
                {
                    try
                    {
                        list.ForEach(v =>
                        {
                            try
                            {
                                if (v.IsEnableCallConfirm)
                                    v.CallConfirm(true);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                    catch (Exception ex)
                    {

                    }
                });
                var task2 = task1.ContinueWith(ret =>
                {
                    list.ForEach(m => OnConfirmEvent(m));
                    bCallingConditionConfirmAllCommand = false;
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        internal bool IsEnableCallConditionConfirmAllCommand
        {
            get
            {
                if (bCallingConditionConfirmAllCommand)
                    return false;
                else
                {
                    if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionConfirmAllCommand.CanExecute(null))
                        return true;

                    foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                    {
                        if (monitoredItem != null && monitoredItem.ConditionConfirmAllCommand.CanExecute(null))
                            return true;
                    }

                    return false;
                }
            }
                     
        }


        private bool? GetEReloadValue()
        {
            try
            {
                if (eReloadMonitoredItemViewModel != null &&
                    eReloadMonitoredItemViewModel.DataValue != null && 
                    eReloadMonitoredItemViewModel.DataValue.Value != null)
                {
                    return Convert.ToBoolean(eReloadMonitoredItemViewModel.DataValue.Value);
                }
            }
            catch { }

            return null;
        }
        #endregion

        #region Constructors
        bool bForceRenew;
        public GridAlarmWindow()
        {
            InitializeComponent();
            ///////////////////////////////////////////////////////////////////////////////////////////
            // devexpress optimized mode implementation
            // (see https://www.devexpress.com/Support/Center/Question/Details/T147586)
            ///////////////////////////////////////////////////////////////////////////////////////////
            //view.UseLightweightTemplates = UseLightweightTemplates.None;

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            ///////////////////////////////////////////////////////////////////////////////////////////
            // temporary code for fixing the devexpress bug
            // (see http://www.devexpress.com/Support/Center/Question/Details/Q445390)
            gridControl.ManipulationDelta += (o, e) =>
            {
                e.Handled = true;
            };
            ///////////////////////////////////////////////////////////////////////////////////////////

            //IsEnabled = false;
            DevExpress.Xpf.Bars.ToolBarControl toolBarControl = toolbartray.Bars[0] as DevExpress.Xpf.Bars.ToolBarControl;
            Loaded += (o, e) =>
            {
                if (Thresholds.Count == 0)
                    Thresholds.InitThreshold();

                if (!bLoaded && !bDispose)
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

                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (Document != null)
                    {
                        if (ufuaEditorManager == null)
                        {
                            ufuaEditorManager = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                        }
                    }

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
                            StringManager_CultureChanged(null, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (bDesignmode)
                    {
                        SetGridDimension(SpecificCommandPosition);
                        InitCommandPanel(SpecificCommandPosition);
                        gridControl.IsEnabled = true;
                        OverrideBaseProperties();
                        toolbartray.IsEnabled = false;
                        InitButtons();
                        if (!bSmartSettingsEditing)
                            container.IsHitTestVisible = false;
                        bInit = true;
                    }
                    else
                    {
                        view.PreviewKeyDown += rowPresenterGrid_PreviewKeyDown;
                        view.PreviewMouseDown += rowPresenterGrid_PreviewMouseDown;
                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();
                        qualityToStringConverter = Resources["qualityToStringConverter"] as IQualityToStringConverter;

                        if (RunningOnServer)
                        {
                            AckCmdButton =
                            ConfirmCmdButton =
                            AddCommentCmdButton =
                            ShelveCmdButton = false;
                            toolbarSettings.IsVisible = false;
                            try
                            {

                                ClientTimezoneOffset = (double)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
                            }
                            catch
                            {
                            }
                        }

                        SetGridDimension(SpecificCommandPosition);
                        InitCommandPanel(SpecificCommandPosition);

                        OverrideBaseProperties();

                        InitButtons();
                        InitSoundButton();

                        gridControl.FilterChanged += FilterChanged;
                        view.ShowGridMenu += onColumnContextMenuShow;

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

                        Dispatcher.BeginInvokeAsynchronouslyInRender(() => 
                        {
                            if (bDispose)
                                return;

                            gridControl.SelectedItems.Clear();
                            PrepareExecution();
                            UpdateGridStatus();
                        });

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
                    if (monitoredItemViewModel != null)
                    {
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                        if (monitoredItemViewModel.ConditionStateList != null)
                            monitoredItemViewModel.ConditionStateList.CollectionChanged -= CollectionView_CollectionChanged;
                    }

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                    if (monitoredItemViewModel.ConditionStateList != null)
                        monitoredItemViewModel.ConditionStateList.CollectionChanged += CollectionView_CollectionChanged;

                    UpdateGridAlarm();
                    UpdateGridStatus();
                    title.Content = Title;
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
            var isElementContained = (sender is TextBlock || sender is Grid) && (from c in view.GetVisualChildrenOfType<FrameworkElement>()
                                      where c == sender || sender == view
                                      select c).FirstOrDefault() != null;
            if (!isElementContained)
                return;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                if (!bDispose)
                    view.Focus();
            });
        }

        private void rowPresenterGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement)
                UpdateFocus(e.OriginalSource as FrameworkElement);
        }

        void Subscribe(OPCUAEntityReference eChild, string sessionName)
        {
            if (eChild == null || !eChild.IsValid || childsMonitoredItemViewModel.ContainsKey(eChild))
                return;

            childsMonitoredItemViewModel[eChild] = null;
            eChild.PropertyChanged += eChild_PropertyChanged;
            if (eChild.MonitoredItemViewModel != null)
                eChild_PropertyChanged(eChild, new PropertyChangedEventArgs("MonitoredItemViewModel"));

            eChild.Resolve(sessionName);
            eChild.SetInUse(this, true);
        }

        private void eChild_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (childsMonitoredItemViewModel[n] != null && childsMonitoredItemViewModel[n] == n.MonitoredItemViewModel)
                    return;

                if (childsMonitoredItemViewModel[n] != null)
                    childsMonitoredItemViewModel[n].PropertyChanged -= eChildMonitoredItemViewModel_PropertyChanged;
                childsMonitoredItemViewModel[n] = n.MonitoredItemViewModel;

                if (n.MonitoredItemViewModel != null)
                {
                    n.MonitoredItemViewModel.PropertyChanged += eChildMonitoredItemViewModel_PropertyChanged;
                    eChildMonitoredItemViewModel_PropertyChanged(n.MonitoredItemViewModel, new PropertyChangedEventArgs("Quality"));
                }

                UpdateGridAlarm();
                UpdateGridStatus();
                UpdateContextMenu();
            }
        }

        void eChildMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
                 UpdateGridStatus();
            }
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            if (!bDispose && !bChangeCriteria)
                UpdateUnshelveButton();
        }

        private void onColumnContextMenuShow(object sender, GridMenuEventArgs g)
        {
            if (!bDispose && g.MenuType == GridMenuType.Column && !AllowDropColumns)
            {
                g.Customizations.Add(new DevExpress.Xpf.Bars.RemoveBarItemAndLinkAction(){
                    ItemName = DefaultColumnMenuItemNames.ColumnChooser
                });
            }
        }

        string stringPlaceolder = "AlarmWindow";
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

                if (qualityToStringConverter != null)
                    qualityToStringConverter.CurrentStringList = stringlist;

                Col1.Header = Properties.Resources.BranchColumnName;
                Col2.Header = Properties.Resources.MessageColumnName;
                Col9.Header = Properties.Resources.QualityColumnName;
                Col3.Header = Properties.Resources.CommentColumnName;
                Col4.Header = Properties.Resources.SourceColumnName;
                Col5.Header = Properties.Resources.ReasonColumnName;
                Col113.Header = Properties.Resources.StateColumnName;
                Col6.Header = Properties.Resources.ConditionColumnName;
                Col7.Header = Properties.Resources.SeverityColumnName;
                Col8.Header = Properties.Resources.TimeColumnName;
                Col110.Header = Properties.Resources.ActiveTransitionTimeColumnName;
                Col111.Header = Properties.Resources.AckedTransitionTimeColumnName;
                Col112.Header = Properties.Resources.ConfirmedTransitionTimeColumnName;
                Col114.Header = Properties.Resources.ShelvingTransitionTimeColumnName;
                Col115.Header = Properties.Resources.ChildProjectColumnName;

                TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);

                ackAllBtn.ToolTip = ackAllBtn1.ToolTip = ackAllBtnText.Text = ackAllBtnText1.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AckAllCmd", stringlist, Properties.Resources.AckAllCmd);
                confirmAllBtn.ToolTip = confirmAllBtn1.ToolTip = confirmAllBtnText.Text = confirmAllBtnText1.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ConfirmAllCmd", stringlist, Properties.Resources.ConfirmAllCmd);
                soundButton.ToolTip = soundButton1.ToolTip = soundButtonText.Text = soundButtonText1.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DisableSoundCmd", stringlist, Properties.Resources.DisableSoundCmd);
                refreshBtn.ToolTip = refreshBtn1.ToolTip = refreshBtnText.Text = refreshBtnText1.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshCmd", stringlist, Properties.Resources.RefreshCmd);
                configMemoryTitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualSettings", stringlist, Properties.Resources.ActualSettings);
                saveSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveConfiguration", stringlist, Properties.Resources.SaveConfiguration);
                cancelSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteSettings", stringlist, Properties.Resources.DeleteSettings);
                clearSmall.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ResetSettings", stringlist, Properties.Resources.ResetSettingsTooltip);
                bestFit.Content = bestFit.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_BestFit", stringlist, Properties.Resources.BestFit);
                ButtonExpandAll.Content = ButtonExpandAll.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExpandAll", stringlist, Properties.Resources.ExpandAll);
                ButtonCollapseAll.Content = ButtonCollapseAll.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CollapseAll", stringlist, Properties.Resources.CollapseAll);
                ButtonUnsvelved.Content = ButtonUnsvelved.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Unshelved", stringlist, Properties.Resources.Unshelved);
                servertitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ServerTitle", stringlist, Properties.Resources.ServerTitle);
                qualitytitle.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Quality", stringlist, Properties.Resources.Quality);
                configMemory.EditValue = ActualConfig;

                string _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AckCmd", stringlist, Properties.Resources.AckCmd);
                ack.ToolTip = ack_Text.Text = _text;
                ack1.ToolTip = ack1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ConfirmCmd", stringlist, Properties.Resources.ConfirmCmd);
                reset.ToolTip = reset_Text.Text = _text;
                reset1.ToolTip = reset1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddCommentCmd", stringlist, Properties.Resources.AddCommentCmd);
                comment.ToolTip = comment_Text.Text = _text;
                comment1.ToolTip = comment1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExecuteComandsCmd", stringlist, Properties.Resources.ExecuteComandsCmd);
                command.ToolTip = command_Text.Text = _text;
                command1.ToolTip = command1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ShelveCmd", stringlist, Properties.Resources.ShelveCmd);
                shelve.ToolTip = shelve_Text.Text = _text;
                shelve1.ToolTip = shelve1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_UnshelveCmd", stringlist, Properties.Resources.UnshelveCmd);
                unshelve.ToolTip = unshelve_Text.Text = _text;
                unshelve1.ToolTip = unshelve1_Text.Text = _text;

                _text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ShelvingTransitionTime", stringlist, Properties.Resources.ShelveTime);
                shelve_Time.ToolTip = _text;

                if (!bDesignmode)
                {
                    if (bInit)
                        CallRefreshCommand();
                    else
                        bForceRenew = true;
                }

                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo(stringManager.GetActiveCulture(Document));
                CurrentCulture = culture;
            });
        }
        #endregion

        #region Methods

        void UpdateCommandVisibility()
        {
            toolBarControl.Visibility = Editable || ShowBestFitButton || ShowExpandCollapseButtons || ShowUshelvedAlaramsButton || ShowServerTitle || ShowQualityStatus ? Visibility.Visible : Visibility.Collapsed;
        }

        bool bChangeCriteria;
        private void UpdateUnshelveButton()
        {
            string filterCriteria = string.Empty;
            if (!ReferenceEquals(gridControl.FilterCriteria, null))
                filterCriteria = gridControl.FilterCriteria.ToString();
            if (filterCriteria.Contains(UnshelvedCriteria) || filterCriteria.Contains(AndUnshelvedCriteria))
                ButtonUnsvelved.IsChecked = true;
            else
                ButtonUnsvelved.IsChecked = false;
        }

        private void InitButtons()
        {
            OnShowServerTitleChanged(false, ShowServerTitle);
            OnShowQualityStatusChanged(false, ShowQualityStatus);
            OnShowBestFitButtonChanged(false, ShowBestFitButton);
            OnShowUshelvedAlaramsButtonChanged(false, ShowUshelvedAlaramsButton);
            OnShowExpandCollapseButtonsChanged(false, ShowExpandCollapseButtons);
            OnShowGroupPanelChanged(false, ShowGroupPanel);
            OnShowSearchPanelChanged(false, ShowSearchPanel);
            OnShowFilterPanelChanged(false, ShowFilterPanel);
        }

        List<ConditionStateViewModel> GetListConditionStateViewModel()
        {
            var items = gridControl.DataController.GetAllFilteredAndSortedRows();
            if (items != null)
                return GetListConditionStateViewModel(items);

            return null;
        }

        List<ConditionStateViewModel> GetListConditionStateViewModel(IEnumerable SelectedRows)
        {
            var list = new List<ConditionStateViewModel>();

            foreach (var focusedRow in SelectedRows)
            {
                if (focusedRow is ConditionStateViewModel)
                    list.Add(focusedRow as ConditionStateViewModel);
                else if (focusedRow is CollectionContainer)
                    list.AddRange(GetListConditionStateViewModel((focusedRow as CollectionContainer).Collection));
            }

            return list;
        }

        ConnectionState GetCurrentState()
        {
            var state = GetCurrentState(monitoredItemViewModel);
            if (state == ConnectionState.Connected)
            {
                foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                {
                    state = GetCurrentState(monitoredItem);
                    if (state != ConnectionState.Connected)
                        break;
                }
            }

            return state;
        }

        ConnectionState GetCurrentState(MonitoredItemViewModel monitoredItem)
        {
            if (monitoredItem == null)
                return ConnectionState.NotConnected;
            else if (monitoredItem.DataValue == null || Opc.Ua.StatusCode.IsBad(monitoredItem.DataValue.StatusCode))
                return ConnectionState.InError;
            else if (Opc.Ua.StatusCode.IsUncertain(monitoredItem.DataValue.StatusCode))
                return ConnectionState.Connecting;
            else if (Opc.Ua.StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                return ConnectionState.Connected;
            else
                return ConnectionState.Undefined;
        }

        void UpdateGridAlarm()
        {
            if (dpUpdateGridAlarm == null ||
                dpUpdateGridAlarm.Status == DispatcherOperationStatus.Completed ||
                dpUpdateGridAlarm.Status == DispatcherOperationStatus.Aborted)
            {
                dpUpdateGridAlarm = Dispatcher.BeginInvokeAsynchronously(this, () =>
                {
                    gridControl.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;
                    gridControl.BeginDataUpdate();
                    if (ConnectChildAlarms)
                        gridControl.ItemsSource = CollectionSource;
                    else
                    {
                        lock (notifyCollectionChangesList)
                        {
                            notifyCollectionChangesList.Clear();
                        }

                        conditionStateList = new ObservableCollection<ConditionStateViewModel>(monitoredItemViewModel.GetCurrentConditionStateList());
                        gridControl.ItemsSource = conditionStateList;
                    }

                    var filter = gridControl.FilterCriteria;
                    if (filter is FunctionOperator)
                    {
                        var function = filter as FunctionOperator;
                        foreach (CriteriaOperator o in function.Operands)
                        {
                            if (o is OperandProperty l_OperandProperty && l_OperandProperty.PropertyName.StartsWith("Unbound"))
                            {
                                l_OperandProperty.PropertyName = l_OperandProperty.PropertyName.Remove(0, "Unbound".Length);
                            }
                        }
                    }
                    gridControl.FilterCriteria = null;
                    gridControl.FilterCriteria = filter;
                    gridControl.EndDataUpdate();

                    if (stringManager != null && IsEnableCallRefreshCommand && bForceRenew)
                        CallRefreshCommand();
                });
            }
        }

        void UpdateGridStatus()
        {
            var newState = GetCurrentState();
            if (connectionState != newState)
            {
                connectionState = newState;

                if (dpUpdateGridStatus == null ||
                    dpUpdateGridStatus.Status == DispatcherOperationStatus.Completed ||
                    dpUpdateGridStatus.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdateGridStatus = Dispatcher.BeginInvokeAsynchronously(this, () =>
                    {
                        if (newState == ConnectionState.InError)
                        {
                            var effect = new System.Windows.Media.Effects.DropShadowEffect
                            {
                                ShadowDepth = 0,
                                BlurRadius = 10,
                                Color = Colors.Red
                            };
                            gridControl.Effect = effect;

                            statusText.Text = Properties.Resources.NotConnected;
                            statusText.Visibility = Visibility.Visible;
                        }
                        else if (newState == ConnectionState.Connecting)
                        {
                            var effect = new System.Windows.Media.Effects.DropShadowEffect
                            {
                                ShadowDepth = 0,
                                BlurRadius = 10,
                                Color = Colors.Yellow
                            };
                            gridControl.Effect = effect;

                            statusText.Text = Properties.Resources.Connecting;
                            statusText.Visibility = Visibility.Visible;
                        }
                        else if (newState == ConnectionState.Connected)
                        {
                            gridControl.Effect = null;
                            statusText.Visibility = Visibility.Collapsed;
                        }
                        else if (newState == ConnectionState.NotConnected)
                        {
                            gridControl.Effect = null;
                            statusText.Text = Properties.Resources.NotConnected;
                            statusText.Visibility = Visibility.Visible;
                        }
                    });
                }
            }
        }

        void UpdateContextMenu()
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                childsStatus.Items.Clear();
                foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                {
                    if (monitoredItem != null)
                    {
                        var statusUserControl = new OPCUAViewModel.UserControls.MonitoredItemStatusViewModel();
                        statusUserControl.DataContext = monitoredItem;
                        var menuItem = new MenuItem()
                        {
                            Header = monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().Title
                        };
                        menuItem.Items.Add(new MenuItem()
                        {
                            Header = statusUserControl
                        });
                        childsStatus.Items.Add(menuItem);
                    }
                };

                childsStatus.Visibility = childsStatus.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        CompositeCollection compositeCollection;
        ICollection CollectionSource
        {
            get
            {
                if (compositeCollection == null)
                    compositeCollection = new CompositeCollection();
                compositeCollection.Clear();

                if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionStateList != null)
                {
                    var container = new CollectionContainer() { Collection = monitoredItemViewModel.ConditionStateList };
                    compositeCollection.Add(container);
                }

                foreach (var monitoredItem in childsMonitoredItemViewModel.Values)
                {
                    if (monitoredItem != null && monitoredItem.ConditionStateList != null)
                    {
                        var container = new CollectionContainer() { Collection = monitoredItem.ConditionStateList };
                        compositeCollection.Add(container);
                    }
                };

                if (compositeCollection.Count == 0)
                {
                    var container = new CollectionContainer() { Collection = null };
                    compositeCollection.Add(container);
                }

                return compositeCollection;
            }
        }

        private void ButtonExpand_Click(object sender, RoutedEventArgs e)
        {
            if (!bDesignmode)
                gridControl.ExpandAllGroups();
        }

        private void ButtonCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (!bDesignmode)
                gridControl.CollapseAllGroups();
        }

        void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {  
            if (bDispose)
                return;

            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
               UpdateGridStatus();
            }
        }

        private void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool bNewDispatcherOperation = false;
            lock (notifyCollectionChangesList)
            {
                bNewDispatcherOperation = notifyCollectionChangesList.Count == 0;
                notifyCollectionChangesList.Add(e);
            }

            if (bNewDispatcherOperation || dpCollectionChanged == null ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Completed ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Aborted)
            {
                dpCollectionChanged = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    var notifyList = new List<NotifyCollectionChangedEventArgs>();
                    lock (notifyCollectionChangesList)
                    {
                        notifyList.AddRange(notifyCollectionChangesList);
                        notifyCollectionChangesList.Clear();
                    }

                    if (conditionStateList != null)
                    {
                        foreach (var item in notifyList)
                        {
                            if (item.Action == NotifyCollectionChangedAction.Reset)
                                conditionStateList.Clear();

                            if (item.NewItems != null && item.NewItems.Count != 0)
                            {
                                foreach (ConditionStateViewModel conditionState in item.NewItems)
                                {
                                    if (!conditionStateList.Contains(conditionState))
                                        conditionStateList.Add(conditionState);
                                }
                            }

                            if (item.OldItems != null && item.OldItems.Count != 0)
                            {
                                foreach (ConditionStateViewModel conditionState in item.OldItems)
                                {
                                    if (conditionStateList.Contains(conditionState))
                                        conditionStateList.Remove(conditionState);
                                }
                            }
                        }
                    }
                });
            }
        }

        const string UnshelvedCriteria = "Not Contains([EnabledState], 'TimedShelved')";
        const string AndUnshelvedCriteria = " And Not Contains([EnabledState], 'TimedShelved')";
        const string AndOP = " And ";
        const string OrOP = " Or ";
        const string NotAndOP = " NotAnd ";
        const string NotOrOP = " NotOr ";
        private void UpdateUnshelved()
        {
            gridControl.BeginDataUpdate();
            var filterCriteria = string.Empty;
            if (!ReferenceEquals(gridControl.FilterCriteria, null))
                filterCriteria = gridControl.FilterCriteria.ToString();

            try 
            {
                if ((bool)ButtonUnsvelved.IsChecked)
                    gridControl.FilterCriteria = CriteriaOperator.Parse(string.Format("{0}", !string.IsNullOrEmpty(filterCriteria) ? string.Format("{0}{1}",filterCriteria,AndUnshelvedCriteria) : UnshelvedCriteria));
                else
                {
                    if (string.IsNullOrEmpty(filterCriteria))
                        return;
                    var shelvedCriteria = filterCriteria;
                    var index = filterCriteria.IndexOf(AndUnshelvedCriteria);
                    if (index != -1)
                    {
                        shelvedCriteria = filterCriteria.Remove(index, AndUnshelvedCriteria.Length);
                    }
                    else
                    {
                        index = filterCriteria.IndexOf(UnshelvedCriteria);
                        if (index != -1)
                            shelvedCriteria = filterCriteria.Remove(index, UnshelvedCriteria.Length);
                    }

                    index = shelvedCriteria.IndexOf(AndOP);
                    if (index == 0)
                    {
                        shelvedCriteria = shelvedCriteria.Remove(0, AndOP.Length);
                    }

                    index = shelvedCriteria.IndexOf(OrOP);
                    if (index == 0)
                    {
                        shelvedCriteria = shelvedCriteria.Remove(0, OrOP.Length);
                    }

                    index = shelvedCriteria.IndexOf(NotAndOP);
                    if (index == 0)
                    {
                        shelvedCriteria = shelvedCriteria.Remove(0, NotAndOP.Length);
                    }

                    index = shelvedCriteria.IndexOf(NotOrOP);
                    if (index == 0)
                    {
                        shelvedCriteria = shelvedCriteria.Remove(0, NotOrOP.Length);
                    }

                    gridControl.FilterCriteria = CriteriaOperator.Parse(shelvedCriteria);
                }
            }
            catch
            {

            }

            gridControl.EndDataUpdate();
        }

        private void Click_BestFit(object sender, RoutedEventArgs e)
        {
            view.BestFitColumns();
        }

        private void InitSoundButton()
        {
            if (EReload == null)
            {
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (ufuaEditorManager != null && Document != null)
                {
                    var sessionName = GetSessionName();
                    var applicationName = ufuaEditorManager.GetDefApplicationName(Document);
                    if (RealTimeConnectionManagerViewModel.IsSessionServerRemote(sessionName, applicationName))
                    {
                        eReloadMonitoredItemViewModel = SysVariables.SysVariables.GetSysVariables(Document).GetVariable(SysVariables.SysNames.AlarmSoundActiveOnClient);
                        if (eReloadMonitoredItemViewModel != null)
                        {
                            eReloadMonitoredItemViewModel.PropertyChanged += eReloadMonitoredItemViewModel_PropertyChanged;
                            if (eReloadMonitoredItemViewModel.DataValue != null)
                                eReloadMonitoredItemViewModel_PropertyChanged(eReloadMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                        }
                    }
                    else
                    {
                        try
                        {
                            var opcString = ufuaEditorManager.GetNodeIdEntityReference(Document,
                                String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName,
                                UFUAServerInfo.BrowserNames.AlarmsSoundStateName),
                                UFUAServerInfo.Guids.SystemTagsGuid.ToString());
                            //"Alarms/SoundState", "60d54358-8bf6-485e-aaa4-c5c4b105b7b7");
                            //"AlarmSoundState", "{ns=2;s=60d54358-8bf6-485e-aaa4-c5c4b105b7b7?AlarmsSoundState}", null);
                            EReload = opcString.FromXml<OPCUAEntityReference>();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        String GetSessionName()
        {
            var sessionName = Properties.Resources.SessionName;
            var doc = Document as ScreenDocument;
            if (doc != null && !String.IsNullOrEmpty(doc.SessionString))
                sessionName = doc.SessionString;
            return sessionName;
        }

        void PrepareExecution()
        {
            if (bExecuted)
                return;
            bExecuted = true;

            var sessionName = GetSessionName();
            if (EReload != null && EReload.IsValid)
            {
                EReload.PropertyChanged += EReload_PropertyChanged;
                if (EReload.MonitoredItemViewModel != null)
                    EReload_PropertyChanged(EReload, new PropertyChangedEventArgs("MonitoredItemViewModel"));

                EReload.Resolve(sessionName);
                EReload.SetInUse(this, true);
            }

            if (ufuaEditorManager != null && Document != null)
            {
                var appName = ufuaEditorManager.GetDefApplicationName(Document);
                if (!String.IsNullOrEmpty(appName))
                    commandsExecuter.Add(appName, new CommandsExecuterHelper(Dispatcher, ufuaEditorManager, Document, this));
            }

            if (ConnectChildAlarms && Document.Parent.Childs.Count > 0)
            {
                if (ufuaEditorManager != null && Document != null)
                {
                    foreach (var project in Document.Parent.Childs)
                    {
                        var server = ufuaEditorManager.GetServerEntityReference(project);
                        if (server == null)
                            continue;
                        var eChild = server.FromXml<OPCUAEntityReference>();
                        Subscribe(eChild, sessionName);

                        if (!commandsExecuter.ContainsKey(eChild.AppName))
                            commandsExecuter.Add(eChild.AppName, new CommandsExecuterHelper(Dispatcher, ufuaEditorManager, project, this));
                    }
                }
            }

            foreach (var executer in commandsExecuter.Values)
                executer.Resolve(sessionName);
        }

        MonitoredItemViewModel eReloadMonitoredItemViewModel;
        private void EReload_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (eReloadMonitoredItemViewModel != null)
                    eReloadMonitoredItemViewModel.PropertyChanged -= eReloadMonitoredItemViewModel_PropertyChanged;
                eReloadMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (eReloadMonitoredItemViewModel != null)
                {
                    eReloadMonitoredItemViewModel.PropertyChanged += eReloadMonitoredItemViewModel_PropertyChanged;
                    if (eReloadMonitoredItemViewModel.DataValue != null)
                        eReloadMonitoredItemViewModel_PropertyChanged(eReloadMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }

        private void eReloadMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    if (bDispose)
                        return;

                    UpdateSound();
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });
            }
        }

        bool? silentSound;
        private void UpdateSound()
        {
            silentSound = GetEReloadValue();
            if (silentSound.HasValue)
                silentSound = !silentSound;
            soundButton.IsChecked = soundButton1.IsChecked = silentSound;
        }

        void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            if (EReload != null)
                EReload.PropertyChanged -= EReload_PropertyChanged;

            if (eReloadMonitoredItemViewModel != null)
                eReloadMonitoredItemViewModel.PropertyChanged -= eReloadMonitoredItemViewModel_PropertyChanged;

            if (EReload != null && EReload.IsValid)
                EReload.SetInUse(this, false);

            if (childsMonitoredItemViewModel.Keys.Count > 0)
            {
                foreach (var eChild in childsMonitoredItemViewModel.Keys)
                {
                    if (childsMonitoredItemViewModel[eChild] != null)
                        childsMonitoredItemViewModel[eChild].PropertyChanged -= eChildMonitoredItemViewModel_PropertyChanged;

                    eChild.PropertyChanged -= eChild_PropertyChanged;
                    eChild.SetInUse(this, false);
                }
                childsMonitoredItemViewModel.Clear();
            }

            foreach (var executer in commandsExecuter.Values)
                executer.Dispose();
            commandsExecuter.Clear();
        }

        void AbortDispatcherOperations()
        {

            if (dpUpdateGridAlarm != null &&
                dpUpdateGridAlarm.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateGridAlarm.Status != DispatcherOperationStatus.Completed)
                dpUpdateGridAlarm.Abort();

            if (dpUpdateGridStatus != null &&
                dpUpdateGridStatus.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateGridStatus.Status != DispatcherOperationStatus.Completed)
                dpUpdateGridStatus.Abort();

            if (dpCollectionChanged != null &&
                dpCollectionChanged.Status != DispatcherOperationStatus.Aborted &&
                dpCollectionChanged.Status != DispatcherOperationStatus.Completed)
                dpCollectionChanged.Abort();
        }

        private void TableView_ValidateRow(object sender, GridRowValidationEventArgs e)
        {
            e.IsValid = true;
        }

        private void TableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void TableView_InvalidRowException(object sender, InvalidRowExceptionEventArgs e) {
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        #region Isolated Storage

        String GetProjectPath()
        {
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(this);

            if (doc != null)
            {
                return String.Format("{0}", System.IO.Path.GetDirectoryName(doc.FilePath));
            }
            return string.Empty;
        }

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

        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            view.PreviewKeyDown -= rowPresenterGrid_PreviewKeyDown;
            view.PreviewMouseDown -= rowPresenterGrid_PreviewMouseDown;
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

            DetachOverrideBaseProperties();

            TerminateExecution();

            if (monitoredItemViewModel != null)
            {
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                if (monitoredItemViewModel.ConditionStateList != null)
                    monitoredItemViewModel.ConditionStateList.CollectionChanged -= CollectionView_CollectionChanged;
            }

            AbortDispatcherOperations();

            gridControl.View.FocusedRowHandle = DevExpress.Data.DataController.InvalidRow;
            gridControl.BeginDataUpdate();
            gridControl.ItemsSource = null;
            gridControl.FilterChanged -= FilterChanged;
            view.ShowGridMenu -= onColumnContextMenuShow;
            gridControl.EndDataUpdate();
        }

        private void ButtonUnsvelved_OnClick(object sender, RoutedEventArgs e)
        {
            bChangeCriteria = true;
            UpdateUnshelved();
            bChangeCriteria = false;
        }
        private void gridControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as GridControl).SelectedItems == null || (sender as GridControl).SelectedItems.Count == 0)
            {
                gridControl.View.FocusedRowHandle = GridControl.InvalidRowHandle;
            }
            OnSelectedEvent();
        }

        private void gridControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int rowHandle = view.GetRowHandleByMouseEventArgs(e);
            if (rowHandle == GridControl.InvalidRowHandle)
                return;

            if (!gridControl.IsGroupRowHandle(rowHandle))
            {
                var conditionState = gridControl.GetRow(rowHandle) as ConditionStateViewModel;
                if (conditionState != null)
                    CallExecuteAlarmCommands(conditionState);
            }
        }
        #endregion

        #region Selected event (for Script)
        ConditionStateViewModel lastSelectedEvent = null;
        void OnSelectedEvent()
        {
            var t = OnSelected;
            if (t != null)
            {
                var selectedevent = gridControl.SelectedItems != null && gridControl.SelectedItems.Count > 0 ? 
                    gridControl.SelectedItems[0] as ConditionStateViewModel : null;
                if (selectedevent != lastSelectedEvent)
                {
                    lastSelectedEvent = selectedevent;
                    t(this, new SelectedArgs(selectedevent));
                }
            }

        }
        public event EventHandler<SelectedArgs> OnSelected;

        void OnAckEvent(ConditionStateViewModel model)
        {
            var t = OnAck;
            if (t != null)
            {
                t(this, new SelectedArgs(model));
            }

        }
        public event EventHandler<SelectedArgs> OnAck;

        bool OnAckingEvent(ConditionStateViewModel model)
        {
            var t = OnAcking;
            if (t != null)
            {
                SelectedArgs arg = new SelectedArgs(model);
                t(this, arg);
                return !arg.Cancel;
            }
            return true;
        }
        public event EventHandler<SelectedArgs> OnAcking;

        void OnConfirmEvent(ConditionStateViewModel model)
        {
            var t = OnConfirm;
            if (t != null)
            {
                t(this, new SelectedArgs(model));
            }

        }
        public event EventHandler<SelectedArgs> OnConfirm;
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

        #region IDataErrorInfo
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                string error = null;

                try
                {
                    error = !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }
                catch (ArgumentException) { }

                return error;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "DateTimeFormat")
            {
                try
                {
                    var test = DateTime.Now.ToString(DateTimeFormat, CurrentCulture);
                }
                catch (FormatException e)
                {
                    return Properties.Resources.DateTimeFormatError;
                }
            }
            return null;
        }
        #endregion

        private void TableView_ShowFilterPopup(object sender, FilterPopupEventArgs e)
        {
            if (e.Column.FieldName != "EnabledState") return;
            List<object> filterItems = new List<object>();


            filterItems.Add(new CustomComboBoxItem()
            {
                DisplayValue = Properties.Resources.StateAll,
                EditValue = new CustomComboBoxItem()
            });

            if (e.Column.Tag.ToString() == "State")
            {
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateUnacknowledged,
                    EditValue = CriteriaOperator.Parse("Contains([EnabledState], 'Unacknowledged')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateUnconfirmed,
                    EditValue = CriteriaOperator.Parse("Contains([EnabledState], 'Unconfirmed')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateTimedShelved,
                    EditValue = CriteriaOperator.Parse("Contains([EnabledState], 'TimedShelved')")
                });
            }
            else if (e.Column.Tag.ToString() == "Reason")
            {
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateON,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'Active')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONHighHigh,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'HighHigh ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONHigh,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'High ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONLow,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'Low ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONLowLow,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'LowLow ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONHighHighActive,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'HighHighActive ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONHighActive,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'HighActive ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONLowActive,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'LowActive ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateONLowLowActive,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'LowLowActive ')")
                });
                filterItems.Add(new CustomComboBoxItem()
                {
                    DisplayValue = Properties.Resources.StateOFF,
                    EditValue = CriteriaOperator.Parse("StartsWith([EnabledState], 'Inactive')")
                });
            }

            e.ComboBoxEdit.ItemsSource = filterItems;
        }

        #region IGridLayoutUser
        public List<StorageColumn> GetColumns()
        {
            return (from column in gridControl.Columns
                    where 
                    column.FieldName != "ChildProjectName" || 
                    (column.FieldName == "ChildProjectName" && ConnectChildAlarms)
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

    public class SelectedArgs : EventArgs
    {
        ConditionStateViewModel model;

        public SelectedArgs(ConditionStateViewModel m)
        {
            model = m;
        }

        public Boolean Cancel
        {
            get;
            set;
        }

        public String ConditionName 
        { 
            get 
            {
                if (model == null)
                    return null;
                return model.Condition; 
            } 
        }

        public String ConditionMessage 
        { 
            get 
            {
                if (model == null)
                    return null;
                return model.Message; 
            } 
        }

        public DateTime? ConditionDateTime
        {
            get
            {
                if (model == null || !model.Time.HasValue)
                    return null;
                return model.Time.Value.ToLocalTime();
            }
        }

        public String ConditionSource
        {
            get
            {
                if (model == null)
                    return null;
                return model.Source;
            }
        }

        public String ConditionSourceNode
        {
            get
            {
                if (model == null)
                    return null;
                return model.SourceNode;
            }
        }

        public String ConditionQuality
        {
            get
            {
                if (model == null)
                    return null;
                return model.Quality;
            }
        }

        public ushort? ConditionSeverity
        {
            get
            {
                if (model == null)
                    return null;
                return model.Severity;
            }
        }
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
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            if (sender is GridAlarmWindow)
            {
                GridAlarmWindow control = sender as GridAlarmWindow;
                if (control.ReadLocalValue(GridAlarmWindow.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(GridAlarmWindow.ControlForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ControlForeground", control.ControlForeground);
                else
                    ret.Add("ControlForeground", foreground);

                if (control.ReadLocalValue(GridAlarmWindow.ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ToolbarForeground", control.ToolbarForeground);
                else
                    ret.Add("ToolbarForeground", foreground);

                if (control.ReadLocalValue(GridAlarmWindow.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);

                if (control.ReadLocalValue(GridAlarmWindow.ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ToolbarBackground", control.ToolbarBackground);
                else
                {
                    if (background is SolidColorBrush)
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast((background as SolidColorBrush).Color, (document as ScreenDocument).Theme));
                        ret.Add("ToolbarBackground", solidColorBrush);
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
                    else 
                        ret.Add("ToolbarBackground", background);
                }
            }
            if (sender is BannerAlarmWindow)
            {
                BannerAlarmWindow control = sender as BannerAlarmWindow;
                if (control.ReadLocalValue(BannerAlarmWindow.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(BannerAlarmWindow.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            if (sender is Auditing)
            {
                Auditing control = sender as Auditing;
                if (control.ReadLocalValue(Auditing.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(Auditing.BackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);

                if (control.ReadLocalValue(Auditing.ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ToolbarForeground", control.ToolbarForeground);
                else
                    ret.Add("ToolbarForeground", foreground);

                if (control.ReadLocalValue(Auditing.ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("ToolbarBackground", control.ToolbarBackground);
                else
                {
                    if (background is SolidColorBrush)
                    {
                        SolidColorBrush solidColorBrush = new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast((background as SolidColorBrush).Color, (document as ScreenDocument).Theme));
                        ret.Add("ToolbarBackground", solidColorBrush);
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
                    else
                        ret.Add("ToolbarBackground", background);
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
