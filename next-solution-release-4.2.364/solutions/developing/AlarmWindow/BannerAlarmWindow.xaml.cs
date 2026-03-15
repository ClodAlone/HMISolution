using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Converters;
using OPCUAViewModel;
using System.Windows.Threading;
using System.Windows.Data;
using AlarmWindow.Enums;
using UFInterfaces.PropertyControl;
using ScreenSettings;
using StringManager.ComponentService;
using log4net;
using Utilities;
using Utilities.WPF;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using ViewModelLib;
using System.Windows.Input;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UFInterfaces;
using GridLayout;
using TranslationHelpers;
using System.Globalization;
using UFUAEditor.ComponentService;
using System.Xml.Serialization;
using UFProjectManager.ComponentService;
using System.Collections.Specialized;

namespace AlarmWindow
{
    /// <summary>
    /// Interaction logic for BannerAlarmWindow.xaml
    /// </summary>
    public partial class BannerAlarmWindow : UserControl, IEntityReference, IDisposable, IContainPropertyEditors, IStringIDAware
    {
        #region OverrideBaseProperties
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(BannerAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(BannerAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(BannerAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(BannerAlarmWindow));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(BannerAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(BannerAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(BannerAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(BannerAlarmWindow));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as BannerAlarmWindow;
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
                value.FontFamily = FontFamily;
                AlarmAreaFontSettings = value;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as BannerAlarmWindow;
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
                value.FontWeight = FontWeight;
                AlarmAreaFontSettings = value;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as BannerAlarmWindow;
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
                value.FontStyle = FontStyle;
                AlarmAreaFontSettings = value;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as BannerAlarmWindow;
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
                value.FontSize = (int)FontSize;
                AlarmAreaFontSettings = value;
            }
        }

        #endregion

        #region Dependency Properties
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }


        #region ShowDateTime
        public static readonly DependencyProperty ShowDateTimeProperty = DependencyProperty.Register("ShowDateTime", typeof(bool), typeof(BannerAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowDateTimeChanged), new CoerceValueCallback(OnCoerceShowDateTime)));

        private static object OnCoerceShowDateTime(DependencyObject o, object value)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                return control.OnCoerceShowDateTime((bool)value);
            else
                return value;
        }

        private static void OnShowDateTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                control.OnShowDateTimeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowDateTime(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowDateTimeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit && bDesignmode)
            {
                if (newValue)
                    alarmText.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} - {1:G}", Properties.Resources.AlarmBannerDefaultMessage, DateTime.Now);
                else
                    alarmText.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}", Properties.Resources.AlarmBannerDefaultMessage);
            }
        }

        public bool ShowDateTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowDateTimeProperty);
            }
            set
            {
                SetValue(ShowDateTimeProperty, value);
            }
        }

        #endregion

        #region Style

        #region EnableSpin
        public static readonly DependencyProperty EnableSpinProperty = DependencyProperty.Register("EnableSpin", typeof(bool), typeof(BannerAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableSpinChanged), new CoerceValueCallback(OnCoerceEnableSpin)));

            private static object OnCoerceEnableSpin(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceEnableSpin((bool)value);
                else
                    return value;
            }

            private static void OnEnableSpinChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnEnableSpinChanged((bool)e.OldValue, (bool)e.NewValue);
            }

            protected virtual bool OnCoerceEnableSpin(bool value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnEnableSpinChanged(bool oldValue, bool newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
                if (newValue != oldValue)
                    viewboxSpin.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }

            [Category("Style")]
            public bool EnableSpin
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (bool)GetValue(EnableSpinProperty);
                }
                set
                {
                    SetValue(EnableSpinProperty, value);
                }
            }
            
            #endregion
            #region SpinPosition
            public static readonly DependencyProperty SpinRelativePositionProperty = DependencyProperty.Register("SpinPosition", typeof(SpinRelativePosition), typeof(BannerAlarmWindow), new UIPropertyMetadata(SpinRelativePosition.Right, new PropertyChangedCallback(OnSpecificCommandPositionChanged), new CoerceValueCallback(OnCoerceSpecificCommandPosition)));

            private static object OnCoerceSpecificCommandPosition(DependencyObject o, object value)
            {
                BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
                if (BannerAlarmWindow != null)
                    return BannerAlarmWindow.OnCoerceSpecificCommandPosition((SpinRelativePosition)value);
                else
                    return value;
            }

            private static void OnSpecificCommandPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
                if (BannerAlarmWindow != null)
                    BannerAlarmWindow.OnSpecificCommandPositionChanged((SpinRelativePosition)e.OldValue, (SpinRelativePosition)e.NewValue);
            }

            protected virtual SpinRelativePosition OnCoerceSpecificCommandPosition(SpinRelativePosition value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnSpecificCommandPositionChanged(SpinRelativePosition oldValue, SpinRelativePosition newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
                if (newValue != oldValue)
                    switch (newValue)
                    {
                        case SpinRelativePosition.Left:
                            DockPanel.SetDock(viewboxSpin, Dock.Left);
                            break;
                        case SpinRelativePosition.Right:
                            DockPanel.SetDock(viewboxSpin, Dock.Right);
                            break;
                        default:
                            break;
                    }  
            }


            [Category("Style")]
            [DefaultValue(typeof(SpinRelativePosition), "Right")]
            public SpinRelativePosition SpinPosition
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (SpinRelativePosition)GetValue(SpinRelativePositionProperty);
                }
                set
                {
                    SetValue(SpinRelativePositionProperty, value);
                }
            }
            #endregion

            #region SpinDimension
            public static readonly DependencyProperty SpinDimensionProperty = DependencyProperty.Register("SpinDimension", typeof(SpinRelativeDimension), typeof(BannerAlarmWindow), new UIPropertyMetadata(SpinRelativeDimension.Large, new PropertyChangedCallback(OnSpinDimensionChanged), new CoerceValueCallback(OnCoerceSpinDimension)));

            private static object OnCoerceSpinDimension(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceSpinDimension((SpinRelativeDimension)value);
                else
                    return value;
            }

            private static void OnSpinDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnSpinDimensionChanged((SpinRelativeDimension)e.OldValue, (SpinRelativeDimension)e.NewValue);
            }

            protected virtual SpinRelativeDimension OnCoerceSpinDimension(SpinRelativeDimension value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnSpinDimensionChanged(SpinRelativeDimension oldValue, SpinRelativeDimension newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
                if(newValue != oldValue)
                    switch (newValue)
                    {
                        case SpinRelativeDimension.Small:
                            {
                               viewboxSpin.Width = (int)15;
                               break;
                            }
                        case SpinRelativeDimension.Medium:
                            {
                                viewboxSpin.Width = (int)20;
                                break;
                            }
                        case SpinRelativeDimension.Large:
                            {
                                viewboxSpin.Width = (int)25;
                                break;
                            }
                        default:
                            {
                                viewboxSpin.Width = (int)15;
                                break;
                            }
                    }

            }

            [Category("Style")]
            public SpinRelativeDimension SpinDimension
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (SpinRelativeDimension)GetValue(SpinDimensionProperty);
                }
                set
                {
                    SetValue(SpinDimensionProperty, value);
                }
            }
            #endregion

            #region EnableRefreshButtonProperty
            public static readonly DependencyProperty EnableRefreshButtonProperty = DependencyProperty.Register("EnableRefreshButton", typeof(Boolean), typeof(BannerAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableRefreshButtonChanged), new CoerceValueCallback(OnCoerceEnableRefreshButton)));

            private static object OnCoerceEnableRefreshButton(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceEnableRefreshButton((Boolean)value);
                else
                    return value;
            }

            private static void OnEnableRefreshButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnEnableRefreshButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
            }

            protected virtual Boolean OnCoerceEnableRefreshButton(Boolean value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnEnableRefreshButtonChanged(Boolean oldValue, Boolean newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
                if (newValue != oldValue)
                    refreshButton.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }

            [Category("Style")]
            public Boolean EnableRefreshButton
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (Boolean)GetValue(EnableRefreshButtonProperty);
                }
                set
                {
                    SetValue(EnableRefreshButtonProperty, value);
                }
            }
            #endregion

            #region CyclingTime
            public static readonly DependencyProperty CyclingTimeProperty = DependencyProperty.Register("CyclingTime", typeof(double), typeof(BannerAlarmWindow), new UIPropertyMetadata((double)3, new PropertyChangedCallback(OnCyclingTimeChanged), new CoerceValueCallback(OnCoerceCyclingTime)));

            private static object OnCoerceCyclingTime(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceCyclingTime((double)value);
                else
                    return value;
            }

            private static void OnCyclingTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnCyclingTimeChanged((double)e.OldValue, (double)e.NewValue);
            }

            protected virtual Double OnCoerceCyclingTime(double value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnCyclingTimeChanged(double oldValue, double newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
            }

            [Category("Style")]
            public double CyclingTime
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (double)GetValue(CyclingTimeProperty);
                }
                set
                {
                    SetValue(CyclingTimeProperty, value);
                }
            }
            #endregion

            #region AthomaticScroll
            public static readonly DependencyProperty AthomaticScrollProperty = DependencyProperty.Register("AthomaticScroll", typeof(bool), typeof(BannerAlarmWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAthomaticScrollChanged), new CoerceValueCallback(OnCoerceAthomaticScroll)));

            private static object OnCoerceAthomaticScroll(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceAthomaticScroll((bool)value);
                else
                    return value;
            }

            private static void OnAthomaticScrollChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnAthomaticScrollChanged((bool)e.OldValue, (bool)e.NewValue);
            }

            protected virtual bool OnCoerceAthomaticScroll(bool value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnAthomaticScrollChanged(bool oldValue, bool newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
            }

            [Category("Style")]
            public bool AthomaticScroll
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (bool)GetValue(AthomaticScrollProperty);
                }
                set
                {
                    SetValue(AthomaticScrollProperty, value);
                }
            }
            
            #endregion

            #region EmptyAlarmListDefaultText
            public static readonly DependencyProperty EmptyAlarmListDefaultTextProperty = DependencyProperty.Register("EmptyAlarmListDefaultText", typeof(string), typeof(BannerAlarmWindow), new UIPropertyMetadata(Properties.Resources.NoActiveAlarms, new PropertyChangedCallback(OnEmptyAlarmListDefaultTextChanged), new CoerceValueCallback(OnCoerceEmptyAlarmListDefaultText)));

            private static object OnCoerceEmptyAlarmListDefaultText(DependencyObject o, object value)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    return bannerAlarmWindow.OnCoerceEmptyAlarmListDefaultText((string)value);
                else
                    return value;
            }

            private static void OnEmptyAlarmListDefaultTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
                if (bannerAlarmWindow != null)
                    bannerAlarmWindow.OnEmptyAlarmListDefaultTextChanged((string)e.OldValue, (string)e.NewValue);
            }

            protected virtual string OnCoerceEmptyAlarmListDefaultText(string value)
            {
                // TODO: Keep the proposed value within the desired range.
                return value;
            }

            protected virtual void OnEmptyAlarmListDefaultTextChanged(string oldValue, string newValue)
            {
                // TODO: Add your property changed side-effects. Descendants can override as well.
            }
            [Category("Style")]
            public string EmptyAlarmListDefaultText
            {
                // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
                get
                {
                    return (string)GetValue(EmptyAlarmListDefaultTextProperty);
                }
                set
                {
                    SetValue(EmptyAlarmListDefaultTextProperty, value);
                }
            }
        #endregion


        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(BannerAlarmWindow), new UIPropertyMetadata(TextWrapping.Wrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
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

        [Category("Style")]
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
        #endregion

        #region Advanced


        #region DisableBlinkAnimation
        public static readonly DependencyProperty DisableBlinkAnimationProperty = DependencyProperty.Register("DisableBlinkAnimation", typeof(bool), typeof(BannerAlarmWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnDisableBlinkAnimationChanged), new CoerceValueCallback(OnCoerceDisableBlinkAnimation)));

            private static object OnCoerceDisableBlinkAnimation(DependencyObject o, object value)
            {
                BannerAlarmWindow control = o as BannerAlarmWindow;
                if (control != null)
                    return control.OnCoerceDisableBlinkAnimation((bool)value);
                else
                    return value;
            }

            private static void OnDisableBlinkAnimationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
                BannerAlarmWindow control = o as BannerAlarmWindow;
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
                alarmBlink.Visibility = newValue ? Visibility.Collapsed : Visibility.Visible;
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
            

        //#region AlarmAreaBackground
        //public static readonly DependencyProperty AlarmAreaBackgroundProperty = DependencyProperty.Register("AlarmAreaBackground", typeof(Brush), typeof(BannerAlarmWindow), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x38, 0x37, 0x37)), new PropertyChangedCallback(OnAlarmAreaBackgroundChanged), new CoerceValueCallback(OnCoerceAlarmAreaBackground)));

        //private static object OnCoerceAlarmAreaBackground(DependencyObject o, object value)
        //{
        //    BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
        //    if (BannerAlarmWindow != null)
        //        return BannerAlarmWindow.OnCoerceAlarmAreaBackground((Brush)value);
        //    else
        //        return value;
        //}

        //private static void OnAlarmAreaBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
        //    if (BannerAlarmWindow != null)
        //        BannerAlarmWindow.OnAlarmAreaBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        //}

        //protected virtual Brush OnCoerceAlarmAreaBackground(Brush value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnAlarmAreaBackgroundChanged(Brush oldValue, Brush newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    //if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
        //    //    AlarmBackgroundBorder.Background = newValue;
        //}

        //[Category("Advanced")]
        //public Brush AlarmAreaBackground
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (Brush)GetValue(AlarmAreaBackgroundProperty);
        //    }
        //    set
        //    {
        //        SetValue(AlarmAreaBackgroundProperty, value);
        //    }
        //}
        //#endregion

        #region SpinBackground
        public static readonly DependencyProperty SpinBackgroundProperty = DependencyProperty.Register("SpinBackground", typeof(Brush), typeof(BannerAlarmWindow), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x38, 0x37, 0x37)), new PropertyChangedCallback(OnSpinBackgroundChanged), new CoerceValueCallback(OnCoerceSpinBackground)));

        private static object OnCoerceSpinBackground(DependencyObject o, object value)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                return control.OnCoerceSpinBackground((Brush)value);
            else
                return value;
        }

        private static void OnSpinBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                control.OnSpinBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSpinBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpinBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateColor(viewboxSpin,Properties.Settings.Default.TagSpinBackground, newValue);
        }

        [Category("Advanced")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpinBackground), RequiredKey = true)]
        public Brush SpinBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SpinBackgroundProperty);
            }
            set
            {
                SetValue(SpinBackgroundProperty, value);
            }
        }

        #endregion


        #region RefreshButtonBackground
        public static readonly DependencyProperty RefreshButtonBackgroundProperty = DependencyProperty.Register("RefreshButtonBackground", typeof(Brush), typeof(BannerAlarmWindow), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x38, 0x37, 0x37)), new PropertyChangedCallback(OnRefreshButtonBackgroundChanged), new CoerceValueCallback(OnCoerceRefreshButtonBackground)));

        private static object OnCoerceRefreshButtonBackground(DependencyObject o, object value)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                return control.OnCoerceRefreshButtonBackground((Brush)value);
            else
                return value;
        }

        private static void OnRefreshButtonBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow control = o as BannerAlarmWindow;
            if (control != null)
                control.OnRefreshButtonBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRefreshButtonBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRefreshButtonBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateColor(viewboxRefresh, Properties.Settings.Default.TagRefreshBackground, newValue);
        }

        [Category("Advanced")]
        [SvgValueConverter(ConverterType = typeof(ConvertRefreshButtonBackground), RequiredKey = true)]
        public Brush RefreshButtonBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RefreshButtonBackgroundProperty);
            }
            set
            {
                SetValue(RefreshButtonBackgroundProperty, value);
            }
        }

        #endregion


        #region AlarmAreaFontSettings
        public static readonly DependencyProperty AlarmAreaFontSettingsProperty = DependencyProperty.Register("AlarmAreaFontSettings", typeof(FontSettings), typeof(BannerAlarmWindow), new UIPropertyMetadata(new FontSettings(FontWeights.Light, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnAlarmAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceAlarmAreaFontSettings)));

        private static object OnCoerceAlarmAreaFontSettings(DependencyObject o, object value)
        {
            BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
            if (BannerAlarmWindow != null)
                return BannerAlarmWindow.OnCoerceAlarmAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnAlarmAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
            if (BannerAlarmWindow != null)
                BannerAlarmWindow.OnAlarmAreaFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceAlarmAreaFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlarmAreaFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if(newValue != oldValue)
            {
                UpdateValueFont(newValue);
                
                //{
                
                //}
            }
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
            alarmText.FontFamily = newValue.FontFamily;
            alarmText.FontSize = newValue.FontSize;
            alarmText.FontWeight = newValue.FontWeight;
            alarmText.FontStyle = newValue.FontStyle;

            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;
        }
        #endregion

        #region Thresholds

        public static readonly DependencyProperty ThresholdsProperty = DependencyProperty.Register("Thresholds", typeof(ThresholdList), typeof(BannerAlarmWindow), new UIPropertyMetadata(new ThresholdList(), new PropertyChangedCallback(OnThresholdsChanged), new CoerceValueCallback(OnCoerceThresholds)));

        private static object OnCoerceThresholds(DependencyObject o, object value)
        {
            BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
            if (BannerAlarmWindow != null)
                return BannerAlarmWindow.OnCoerceThresholds((ThresholdList)value);
            else
                return value;
        }

        private static void OnThresholdsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow BannerAlarmWindow = o as BannerAlarmWindow;
            if (BannerAlarmWindow != null)
                BannerAlarmWindow.OnThresholdsChanged((ThresholdList)e.OldValue, (ThresholdList)e.NewValue);
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

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(BannerAlarmWindow), new UIPropertyMetadata(false));

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

        #endregion

        #region AlarmsSource
        public static readonly DependencyProperty AlarmsSourceProperty = DependencyProperty.Register("AlarmsSource", typeof(string), typeof(BannerAlarmWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAlarmsSourceChanged), new CoerceValueCallback(OnCoerceAlarmsSource)));

        private static object OnCoerceAlarmsSource(DependencyObject o, object value)
        {
            BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
            if (bannerAlarmWindow != null)
                return bannerAlarmWindow.OnCoerceAlarmsSource((string)value);
            else
                return value;
        }

        private static void OnAlarmsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BannerAlarmWindow bannerAlarmWindow = o as BannerAlarmWindow;
            if (bannerAlarmWindow != null)
                bannerAlarmWindow.OnAlarmsSourceChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceAlarmsSource(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlarmsSourceChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && !DesignerProperties.GetIsInDesignMode(this))
            {
                UnsubscribeAlarmsSource();
                if (document == null)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (document != null)
                {
                    var editor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (editor != null)
                    {
                        string reference = null;
                        if (!String.IsNullOrWhiteSpace(newValue))
                            reference = editor.GetAlarmsSourceEntityReference(document, newValue, true);
                        //else
                        //    reference = editor.GetServerEntityReference(document);
                        if (reference != null)
                        {
                            sourceEntityReference = reference.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                            SubscribeAlarmsSource();
                        }
                    }
                }
            }
        }
        
        public string AlarmsSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(AlarmsSourceProperty);
            }
            set
            {
                SetValue(AlarmsSourceProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        bool bDesignmode;
        bool bLoaded;
        bool bSubscribed;
        bool bInit;
        int selectedIndex;
        private DispatcherTimer _displayNextAlarm;
        IDocument document;
        IStringEditorManager stringManager;
        IWorkspace workspace;
        IDictionary<String, String> stringlist;
        IUFProjectManager iUFProjectManager;

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.BannerControlLog);

        OPCUAEntityReference sourceEntityReference;
        MonitoredItemViewModel monitoredItemViewModel;

        DispatcherOperation dp1;
        DispatcherOperation dp2;
        bool bBeginInit;
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

            bCallingRefreshCommand = true;
            monitoredItemViewModel.ConditionRefreshCommand.Execute(null);
            bCallingRefreshCommand = false;
        }

        internal bool IsEnableCallRefreshCommand
        {
            get
            {
                if (bCallingRefreshCommand)
                    return false;
                else
                    return GetItemViewModel() != null && monitoredItemViewModel.ConditionRefreshCommand.CanExecute(null);
            }
        }

        MonitoredItemViewModel GetItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
            monitor = monitoredItemViewModel;
            if (monitor == null || !monitor.IsValid)
                return null;

            var list = monitor.ConditionStateList; // initialize the alarm client subscription
            return monitor;
        }

        #endregion

        #region Constructor
        bool bForceRenew;
        public BannerAlarmWindow()
        {
            InitializeComponent();
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    if (Thresholds.Count == 0)
                        Thresholds.InitThreshold();

                    bDesignmode = DesignerProperties.GetIsInDesignMode(this) || bDesignmode;

                    if (document != null)
                    {
                        iUFProjectManager = document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                        if (stringManager == null)
                            stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            StringManager_CultureChanged(document, null);
                            stringManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (bDesignmode)
                    {
                        DesignerProperties.SetIsInDesignMode(this, true);
                        AlarmBackgroundBorder.Background = new SolidColorBrush(Colors.Transparent);

                        if(ShowDateTime)
                            alarmText.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} - {1:G}", Properties.Resources.AlarmBannerDefaultMessage, DateTime.Now);
                        else
                            alarmText.Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}", Properties.Resources.AlarmBannerDefaultMessage);

                        alarmBlink.IsEnabled = alarmText.IsEnabled = false;
                    }
                    else if (monitoredItemViewModel == null)
                    {
                        statusText.Text = Properties.Resources.Connecting;
                        statusText.Visibility = Visibility.Visible;
                        IsEnabled = false;

                        AlarmBackgroundBorder.Background = new SolidColorBrush(Colors.Transparent);
                        alarmText.Text = String.Empty;
                    }

                    if (this.ReadLocalValue(SpinBackgroundProperty) != DependencyProperty.UnsetValue)
                        UpdateColor(viewboxSpin, Properties.Settings.Default.TagSpinBackground, SpinBackground);

                    if (this.ReadLocalValue(RefreshButtonBackgroundProperty) != DependencyProperty.UnsetValue)
                        UpdateColor(viewboxRefresh, Properties.Settings.Default.TagRefreshBackground, RefreshButtonBackground);

                    alarmText.Foreground = this.Background;
                    OverrideBaseProperties();
                    DisplayNextAlarm();
                    bInit = true;
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDispose)
                    return;

                if (DataContext is MonitoredItemViewModel && sourceEntityReference == null)
                {
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    ConnectAtServer();
                }
            };
        }

        #endregion

        #region Methods

        string stringPlaceolder = "BannerAlarmWindow";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose)
                    return;

                bool bUntranslated = bDesignmode && stringManager.GetActiveCulture(document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
                else
                    stringlist = null;

                nextalarmButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NextAlarmCmd", stringlist, Properties.Resources.NextAlarmCmd);
                prevalarmButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_PrevAlarmCmd", stringlist, Properties.Resources.PrevAlarmCmd);
                refreshButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RefreshCmd", stringlist, Properties.Resources.RefreshCmd);

                if (bDesignmode)
                    return;

                if (bInit)
                    CallRefreshCommand();
                else
                    bForceRenew = true;
            });
        }

        void SubscribeAlarmsSource()
        {
            if (bSubscribed)
                return;
            bSubscribed = true;

            if (sourceEntityReference != null && sourceEntityReference.IsValid)
            {
                sourceEntityReference.PropertyChanged += sourceEntityReference_PropertyChanged;
                if (sourceEntityReference.MonitoredItemViewModel != null)
                    sourceEntityReference_PropertyChanged(sourceEntityReference, new PropertyChangedEventArgs("MonitoredItemViewModel"));

                var sessionName = Properties.Resources.SessionName;
                var doc = document as ScreenDocument;
                if (doc != null && !String.IsNullOrEmpty(doc.SessionString))
                    sessionName = doc.SessionString;

                sourceEntityReference.Resolve(sessionName);
                sourceEntityReference.SetInUse(this, true);
            }
        }

        void UnsubscribeAlarmsSource()
        {
            if (!bSubscribed)
                return;
            bSubscribed = false;

            if (sourceEntityReference != null)
                sourceEntityReference.PropertyChanged -= sourceEntityReference_PropertyChanged;

            if (monitoredItemViewModel != null)
            {
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                monitoredItemViewModel.ConditionStateList.CollectionChanged -= ConditionStateList_CollectionChanged;
            }

            if (sourceEntityReference != null && sourceEntityReference.IsValid)
            {
                sourceEntityReference.SetInUse(this, false);

                IsEnabled = false;
                statusText.Text = Properties.Resources.NotConnected;
                statusText.Visibility = Visibility.Visible;
            }

            sourceEntityReference = null;
        }

        void ConnectAtServer()
        {
            if (monitoredItemViewModel == null || bDispose)
                return;

            try
            {
                monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                monitoredItemViewModel.ConditionStateList.CollectionChanged += ConditionStateList_CollectionChanged;

                statusText.Text = Properties.Resources.Connecting;
                statusText.Visibility = Visibility.Visible;
                IsEnabled = false;

                if (monitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.Good)
                {
                    statusText.Visibility = Visibility.Collapsed;
                    IsEnabled = true;
                }

                InitTimer();
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.PropertyChangedSubscription, ex);
                if(iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(document, Properties.Resources.BannerControlLog, DateTime.UtcNow, $"{Properties.Resources.PropertyChangedSubscription}: {ex.Message}", System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void ConditionStateList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(monitoredItemViewModel != null)
                monitoredItemViewModel.ConditionStateList.CollectionChanged -= ConditionStateList_CollectionChanged;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() => { if (bInit && !bDispose) DisplayNextAlarm(); });
        }

        private void sourceEntityReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (monitoredItemViewModel != null)
                {
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                    monitoredItemViewModel.ConditionStateList.CollectionChanged -= ConditionStateList_CollectionChanged;
                }
                monitoredItemViewModel = n.MonitoredItemViewModel;
                ConnectAtServer();
            }
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
                    if (dp1 == null || dp1.Status == DispatcherOperationStatus.Completed || 
                        dp1.Status == DispatcherOperationStatus.Aborted)
                    {
                        dp1 = Dispatcher.BeginInvokeAsynchronouslyInBackground(this, () =>
                        {
                            if (bDispose || bBeginInit)
                                return;

                            bBeginInit = true;
                            try
                            {
                                statusText.Text = Properties.Resources.Connecting;
                                statusText.Visibility = Visibility.Visible;
                                IsEnabled = true;

                                statusText.Visibility = Visibility.Collapsed;
                            }
                            finally
                            {
                                bBeginInit = false;
                            }
                        });

                        /*
                        if (dp1 != null)
                        {
                            if (dp1.Status != DispatcherOperationStatus.Completed)
                                dp1.Completed += (o, ev) => { dp1 = null; };
                            else
                                dp1 = null;
                        }
                        */
                    }
                }
                else
                {
                    if (dp2 == null || dp2.Status == DispatcherOperationStatus.Completed || 
                        dp2.Status == DispatcherOperationStatus.Aborted)
                    {
                        dp2 = Dispatcher.BeginInvokeAsynchronouslyInBackground(this, () =>
                        {
                            if (bDispose || bBeginInit)
                                return;

                            bBeginInit = true;
                            try
                            {
                                IsEnabled = false;

                                statusText.Text = Properties.Resources.NotConnected;
                                statusText.Visibility = Visibility.Visible;
                            }
                            finally
                            {
                                bBeginInit = false;
                            }
                        });
                        /*
                        if (dp2 != null)
                        {
                            if (dp2.Status != DispatcherOperationStatus.Completed)
                                dp2.Completed += (o, ev) => { dp2 = null; };
                            else
                                dp2 = null;
                        }
                        */
                    }
                }
            }
        }

        private void UpdateColor(Viewbox uie, string tag, Brush newValue)
        {
            (from c in uie.GetVisualChildrenOfType<Shape>()
             where (c.Tag as String) == tag
             select c).ToList().ForEach(child =>
             {
                 child.Fill = newValue;
             });
        }

        private void UpdateForeColor(Brush newValue)
        {
            UpdateColor(viewboxSpin, Properties.Settings.Default.TagSpinForeground, newValue);
            UpdateColor(viewboxRefresh, Properties.Settings.Default.TagRefreshForeground, newValue);
        }

        private void GetNextAlarm()
        {
            try
            {
                List<ConditionStateViewModel> alarms = null;
                if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionStateList != null)
                    alarms = monitoredItemViewModel.ConditionStateList.OrderByDescending(x =>x.ActiveTransitionTime).ToList();
                if (alarms != null && alarms.Count > 0)
                {
                    if (AthomaticScroll)
                        selectedIndex += 1;
                    if (selectedIndex >= alarms.Count)
                        selectedIndex = 0;

                    UpdateAlarmText(alarms[selectedIndex]);
                }
                else
                {
                    SetNoAlarmState();
                }
            }
            catch
            {
                SetNoAlarmState();
            }
        }

        void SetNoAlarmState()
        {
            alarmText.Text = GetAlarmText(EmptyAlarmListDefaultText);
            alarmText.DataContext = null;
            alarmText.Background = new SolidColorBrush(Colors.Transparent);
            alarmBlink.DataContext = null;
            alarmBlink.Background = new SolidColorBrush(Colors.Transparent);
            AlarmBackgroundBorder.Background = new SolidColorBrush(Colors.Transparent);
            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
                alarmText.Foreground = Foreground;
            else
                alarmText.Foreground = Brushes.White;
        }

        private void GetPrevAlarm()
        {
            try
            {
                List<ConditionStateViewModel> alarms = null;
                if (monitoredItemViewModel != null && monitoredItemViewModel.ConditionStateList != null)
                    alarms = monitoredItemViewModel.ConditionStateList.OrderByDescending(x => x.ActiveTransitionTime).ToList();
                if (alarms != null && alarms.Count > 0)
                {
                    selectedIndex -= 1;
                    if (selectedIndex < 0)
                        selectedIndex = alarms.Count - 1;

                    UpdateAlarmText(alarms[selectedIndex]);
                }
                else
                {
                    SetNoAlarmState();
                }
            }
            catch
            {
                SetNoAlarmState();
            }
        }

        private void UpdateAlarmText(ConditionStateViewModel currentAlarm)
        {
            if (bDispose)
                return;
            
            if (currentAlarm == null)
            {
                SetNoAlarmState();
                return;
            }

            alarmText.DataContext = currentAlarm;
            alarmBlink.DataContext = currentAlarm;
            var threshold = Thresholds.Where(x => x.ThresholdValue <= currentAlarm.Severity).Select(x => x).LastOrDefault(); 
            if(threshold == null)
                threshold = Thresholds.Where(x => x.ThresholdValue > currentAlarm.Severity).Select(x => x).FirstOrDefault();
            if(threshold != null)
            {
                Color tc = currentAlarm.NeedsAcknoledge ? currentAlarm.EnabledState.Contains("Inactive") ? threshold.ThresholdOffColor :
                    threshold.ThresholdColor : currentAlarm.EnabledState.Contains("Inactive") ? threshold.ThresholdOffAckColor : threshold.ThresholdAckColor;
                Color tfc = currentAlarm.NeedsAcknoledge ? currentAlarm.EnabledState.Contains("Inactive") ? threshold.ThresholdOffForeColor :
                    threshold.ThresholdForeColor : currentAlarm.EnabledState.Contains("Inactive") ? threshold.ThresholdOffAckForeColor : threshold.ThresholdAckForeColor;
                AlarmBackgroundBorder.Background = new SolidColorBrush(tc);
                alarmText.Foreground = new SolidColorBrush(tfc);
            }

            try
            {
                if(ShowDateTime)
                {
                    var localTime = currentAlarm.Time.Value;
                    if (RunningOnServer)
                        localTime = localTime.AddMinutes(ClientTimezoneOffset);
                    else
                        localTime = localTime.ToLocalTime();
                    alarmText.Text = string.Format("{0} - {1}",
                        GetAlarmText(currentAlarm.Message),
                        String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:G}", localTime));
                }
                else
                    alarmText.Text = string.Format("{0}",
                        GetAlarmText(currentAlarm.Message));
            }
            catch (Exception)
            {
                alarmText.Text = string.Format("{0}",
                    GetAlarmText(currentAlarm.Message));
            }

        }

        private void InitTimer()
        {
            double cyclingTime = Math.Max(minCyclingTime, CyclingTime);
            if (_displayNextAlarm != null)
            {
                _displayNextAlarm.Stop();
                _displayNextAlarm.Interval = TimeSpan.FromSeconds(cyclingTime);
            }
            else
            {
                _displayNextAlarm = new DispatcherTimer(DispatcherPriority.Background)
                {
                    Interval = TimeSpan.FromSeconds(cyclingTime)
                };
                _displayNextAlarm.Tick += DisplayNextAlarm;

                System.Diagnostics.Debug.Write(String.Format("\n stepB: DisplayNextAlarm ADDED"));
            }
            _displayNextAlarm.Start();

            if (stringManager != null && IsEnableCallRefreshCommand && bForceRenew)
                CallRefreshCommand();
        }

        private void DisplayNextAlarm(object sender, EventArgs e)
        {
            DisplayNextAlarm();
        }
        private void DisplayNextAlarm()
        {
            if (bDispose)
                return;

            try
            {
                if (monitoredItemViewModel != null && !bInit)
                {
                    statusText.Visibility = Visibility.Collapsed;
                    IsEnabled = true;
                    bInit = true;
                }

                GetNextAlarm();
            }
            catch
            {
                SetNoAlarmState();
            }
        }

        string GetAlarmText(string alarmtext)
        {
            return TranslationHelper.TranslateComposedText(alarmtext,stringlist,alarmtext);
        }

        private void nextalarmButton_Click_1(object sender, RoutedEventArgs e)
        {
            GetNextAlarm();
        }

        private void prevalarmButton_Click_1(object sender, RoutedEventArgs e)
        {
            GetPrevAlarm();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        
        [Browsable(false)]
        int ClientTimezoneOffset
        {
            get
            {
                if (!RunningOnServer)
                    return 0;
                return (int) ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
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
        const double minCyclingTime = 0.01d;
        [Browsable(false)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                if (workspace == null)
                {
                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (document != null)
                        workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
                }
                if (workspace != null)
                {
                    // Defines Data Template for 'RecipeNameProperty' dependency property.
                    var dt = new DataTemplate();
                    var factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                    factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                    dt.DataType = typeof(string);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(EmptyAlarmListDefaultTextProperty, dt);

                    dt = new DataTemplate();
                    factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                    factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, minCyclingTime);
                    factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int16.MaxValue);
                    dt.DataType = typeof(double);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(CyclingTimeProperty, dt);

                    dt = new DataTemplate();
                    factory = new FrameworkElementFactory(typeof(OPCUAViewModel.PropertyDataTemplate.AlarmsSourcePropertyEditor));
                    dt.DataType = typeof(string);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(AlarmsSourceProperty, dt);
                }

                var dt1 = new DataTemplate();
                var factory1 = new FrameworkElementFactory(typeof(AlarmWindow.Controls.SmartPropertiesEditor));
                factory1.SetValue(AlarmWindow.Controls.SmartPropertiesEditor.DocumentProperty, document);
                dt1.DataType = typeof(bool);
                dt1.VisualTree = factory1;
                mapDataTemplates.Add(SmartPropertiesProperty, dt1);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(EmptyAlarmListDefaultText))
                list.Add(EmptyAlarmListDefaultText);
            return list;
        }
        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(EmptyAlarmListDefaultText))
            {
                var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(BannerAlarmWindow), EmptyAlarmListDefaultTextProperty).DisplayName;
                map.Add(propertyName, EmptyAlarmListDefaultText);
            }
            return map;
        }
        #endregion

        #region IDispose

        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            if (dp1 != null && dp1.Status != DispatcherOperationStatus.Aborted &&
                dp1.Status != DispatcherOperationStatus.Completed)
                dp1.Abort();
            if (dp2 != null && dp2.Status != DispatcherOperationStatus.Aborted &&
                dp2.Status != DispatcherOperationStatus.Completed)
                dp2.Abort();

            UnsubscribeAlarmsSource();
            
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            DetachOverrideBaseProperties();
            if (_displayNextAlarm != null)
            {
                _displayNextAlarm.Stop();
                _displayNextAlarm.Tick -= DisplayNextAlarm;
                _displayNextAlarm = null;
            }

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
        }
        #endregion
    }
    public class ConvertSpinBackground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            BannerAlarmWindow bannerAlarm = sender as BannerAlarmWindow;
            if (bannerAlarm == null)
                return value;
            if (bannerAlarm.ReadLocalValue(BannerAlarmWindow.SpinBackgroundProperty) != DependencyProperty.UnsetValue)
                return bannerAlarm.SpinBackground;
            return bannerAlarm.nextalarmButton.Background;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
    public class ConvertRefreshButtonBackground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            BannerAlarmWindow bannerAlarm = sender as BannerAlarmWindow;
            if (bannerAlarm == null)
                return value;
            if (bannerAlarm.ReadLocalValue(BannerAlarmWindow.RefreshButtonBackgroundProperty) != DependencyProperty.UnsetValue)
                return bannerAlarm.RefreshButtonBackground;
            return bannerAlarm.refreshButton.Background;
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
