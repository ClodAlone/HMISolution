using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Converters;
using DevExpress.Xpf.Gauges;
using WPFUtilities.Extensions;
using Utilities;
using Gauges.Enums;
using UFInterfaces.PropertyControl;
using Gauges.PropertyDataTemplate;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using ScreenSettings;

namespace Gauges
{
    /// <summary>
    /// Interaction logic for Clock.xaml
    /// </summary>
    public partial class Clock : UserControl, IDisposable, IContainPropertyEditors
    {
        #region Dependency Properties
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Clock));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Clock));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Clock));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Clock));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Clock));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(Clock));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(Clock));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(Clock));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(Clock));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(Clock));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as Clock;
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
                var label = ClockFontSettings.Clone();
                var clock = LabelFontSettings.Clone();

                label.FontFamily = FontFamily;
                clock.FontFamily = FontFamily;

                LabelFontSettings = label;
                ClockFontSettings = clock;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as Clock;
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
                var label = ClockFontSettings.Clone();
                var clock = LabelFontSettings.Clone();

                label.FontWeight = FontWeight;
                clock.FontWeight = FontWeight;

                LabelFontSettings = label;
                ClockFontSettings = clock;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as Clock;
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
                var label = ClockFontSettings.Clone();
                var clock = LabelFontSettings.Clone();

                label.FontStyle = FontStyle;
                clock.FontStyle = FontStyle;

                LabelFontSettings = label;
                ClockFontSettings = clock;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as Clock;
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
                var label = ClockFontSettings.Clone();
                var clock = LabelFontSettings.Clone();

                label.FontSize = (int)FontSize;
                clock.FontSize = (int)FontSize; 

                LabelFontSettings = label;
                ClockFontSettings = clock;
            }
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as Clock;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            ClockTextForeground = Foreground;
            LabelForeground = Foreground;
        }
        #endregion
        #region GaugeStyle
        #region EnableBackGroundLayer
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnEnableBackGroundLayerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceEnableBackGroundLayer(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableBackGroundLayerChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    //if (bLoaded)
                    {
                        circularGauge.Model = GetDevBackground(ArcScalePresentation);
                        EnableBackLayer(ArcScalePresentation);
                        UpdateDevBackColor(ArcScaleFill);
                    }
                });
            }
        }

        [Category("ClockStyle")]
        public Boolean EnableBackGroundLayer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(EnableBackGroundLayerProperty);
            }
            set
            {
                SetValue(EnableBackGroundLayerProperty, value);
            }
        }
        
        #endregion


        #region UseSystemTimeZone
        public static readonly DependencyProperty UseSystemTimeZoneProperty = DependencyProperty.Register("UseSystemTimeZone", typeof(bool), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseSystemTimeZoneChanged), new CoerceValueCallback(OnCoerceUseSystemTimeZone)));

        private static object OnCoerceUseSystemTimeZone(DependencyObject o, object value)
        {
            Clock control = o as Clock;
            if (control != null)
                return control.OnCoerceUseSystemTimeZone((bool)value);
            else
                return value;
        }

        private static void OnUseSystemTimeZoneChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock control = o as Clock;
            if (control != null)
                control.OnUseSystemTimeZoneChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseSystemTimeZone(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseSystemTimeZoneChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesign)
            {
                DisplayAfterTimerElapsed(null, null);
            }
        }
        [Category("ClockStyle")]
        public bool UseSystemTimeZone
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseSystemTimeZoneProperty);
            }
            set
            {
                SetValue(UseSystemTimeZoneProperty, value);
            }
        }

        #endregion
        
        #region ClockTimeZone
        public static readonly DependencyProperty ClockTimeZoneProperty = DependencyProperty.Register("ClockTimeZone", typeof(ClockTimeZone), typeof(Clock), new UIPropertyMetadata(null, new PropertyChangedCallback(OnClockTimeZoneChanged), new CoerceValueCallback(OnCoerceClockTimeZone)));

        private static object OnCoerceClockTimeZone(DependencyObject o, object value)
        {
            Clock control = o as Clock;
            if (control != null)
                return control.OnCoerceClockTimeZone((ClockTimeZone)value);
            else
                return value;
        }

        private static void OnClockTimeZoneChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock control = o as Clock;
            if (control != null)
                control.OnClockTimeZoneChanged((ClockTimeZone)e.OldValue, (ClockTimeZone)e.NewValue);
        }

        protected virtual ClockTimeZone OnCoerceClockTimeZone(ClockTimeZone value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnClockTimeZoneChanged(ClockTimeZone oldValue, ClockTimeZone newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null && bDesign)
            {
                DisplayAfterTimerElapsed(null, null);
            }
        }
        [Category("ClockStyle")]

        public ClockTimeZone ClockTimeZone
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ClockTimeZone)GetValue(ClockTimeZoneProperty);
            }
            set
            {
                SetValue(ClockTimeZoneProperty, value);
            }
        }

        #endregion
        

        #region ClockTextVisible
        public static readonly DependencyProperty ClockTextVisibleProperty = DependencyProperty.Register("ClockTextVisible", typeof(bool), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnClockTextVisibleChanged), new CoerceValueCallback(OnCoerceClockTextVisible)));

        private static object OnCoerceClockTextVisible(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceClockTextVisible((bool)value);
            else
                return value;
        }

        private static void OnClockTextVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnClockTextVisibleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceClockTextVisible(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnClockTextVisibleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
         [Category("ClockStyle")]
        public bool ClockTextVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ClockTextVisibleProperty);
            }
            set
            {
                SetValue(ClockTextVisibleProperty, value);
            }
        }
        
        #endregion
        #region ClockTextForeground
         public static readonly DependencyProperty ClockTextForegroundProperty = DependencyProperty.Register("ClockTextForeground", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnClockTextForegroundChanged), new CoerceValueCallback(OnCoerceClockTextForeground)));

         private static object OnCoerceClockTextForeground(DependencyObject o, object value)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 return circularGauge.OnCoerceClockTextForeground((Brush)value);
             else
                 return value;
         }

         private static void OnClockTextForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 circularGauge.OnClockTextForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
         }

         protected virtual Brush OnCoerceClockTextForeground(Brush value)
         {
             // TODO: Keep the proposed value within the desired range.
             return value;
         }

         protected virtual void OnClockTextForegroundChanged(Brush oldValue, Brush newValue)
         {
             // TODO: Add your property changed side-effects. Descendants can override as well.
         }
         [Category("ClockStyle")]
         public Brush ClockTextForeground
         {
             // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
             get
             {
                 return (Brush)GetValue(ClockTextForegroundProperty);
             }
             set
             {
                 SetValue(ClockTextForegroundProperty, value);
             }
         }

         #endregion
        #region ClockTextBackground
         public static readonly DependencyProperty ClockTextBackgroundProperty = DependencyProperty.Register("ClockTextBackground", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnClockTextBackgroundChanged), new CoerceValueCallback(OnCoerceClockTextBackground)));

         private static object OnCoerceClockTextBackground(DependencyObject o, object value)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 return circularGauge.OnCoerceClockTextBackground((Brush)value);
             else
                 return value;
         }

         private static void OnClockTextBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 circularGauge.OnClockTextBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
         }

         protected virtual Brush OnCoerceClockTextBackground(Brush value)
         {
             // TODO: Keep the proposed value within the desired range.
             return value;
         }

         protected virtual void OnClockTextBackgroundChanged(Brush oldValue, Brush newValue)
         {
             // TODO: Add your property changed side-effects. Descendants can override as well.
         }
         [Category("ClockStyle")]
         public Brush ClockTextBackground
         {
             // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
             get
             {
                 return (Brush)GetValue(ClockTextBackgroundProperty);
             }
             set
             {
                 SetValue(ClockTextBackgroundProperty, value);
             }
         }

         #endregion

         #region ClockFontSettings
         public static readonly DependencyProperty ClockFontSettingsProperty = DependencyProperty.Register("ClockFontSettings", typeof(FontSettings), typeof(Clock), new UIPropertyMetadata(new FontSettings(FontWeights.DemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnClockFontSettingsChanged), new CoerceValueCallback(OnCoerceClockFontSettings)));

         private static object OnCoerceClockFontSettings(DependencyObject o, object value)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 return circularGauge.OnCoerceClockFontSettings((FontSettings)value);
             else
                 return value;
         }

         private static void OnClockFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
         {
             Clock circularGauge = o as Clock;
             if (circularGauge != null)
                 circularGauge.OnClockFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
         }

         protected virtual FontSettings OnCoerceClockFontSettings(FontSettings value)
         {
             // TODO: Keep the proposed value within the desired range.
             return value;
         }

         protected virtual void OnClockFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
         {
             // TODO: Add your property changed side-effects. Descendants can override as well.
         }
         [Category("ClockStyle")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings ClockFontSettings
         {
             // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
             get
             {
                 return (FontSettings)GetValue(ClockFontSettingsProperty);
             }
             set
             {
                 SetValue(ClockFontSettingsProperty, value);
             }
         }

         #endregion


        #region ArcScalePresentation
        public static readonly DependencyProperty ArcScalePresentationProperty = DependencyProperty.Register("ArcScalePresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.Classic, new PropertyChangedCallback(OnArcScalePresentationChanged), new CoerceValueCallback(OnCoerceArcScalePresentation)));

        private static object OnCoerceArcScalePresentation(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceArcScalePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnArcScalePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnArcScalePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceArcScalePresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnArcScalePresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    //if (bLoaded)
                    {
                        circularGauge.Model = GetDevBackground(newValue);
                        EnableBackLayer(newValue);
                        UpdateDevBackColor(ArcScaleFill);
                    }
                });
            }
        }
        private CircularGaugeModel GetDevBackground(PredefinedElementKinds value)
        {
            if (!EnableBackGroundLayer)
            {
                return GetGaugeModel(PredefinedElementKinds.CleanWhite);
            }
            else
            {
                return GetGaugeModel(value);
            }
        }
        private CircularGaugeModel GetGaugeModel(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (CircularGaugeControl.PredefinedModels as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (CircularGaugeModel)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (CircularGaugeModel)Activator.CreateInstance(((CircularGaugeControl.PredefinedModels as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
            }
        }
        [Category("ClockStyle")]
        public PredefinedElementKinds ArcScalePresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(ArcScalePresentationProperty);
            }
            set
            {
                SetValue(ArcScalePresentationProperty, value);
            }
        }
        #endregion
        #region ArcScaleFill
        public static readonly DependencyProperty ArcScaleFillProperty = DependencyProperty.Register("ArcScaleFill", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x03, 0x3B, 0x62)), new PropertyChangedCallback(OnArcScaleFillChanged), new CoerceValueCallback(OnCoerceArcScaleFill)));

        private static object OnCoerceArcScaleFill(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceArcScaleFill((Brush)value);
            else
                return value;
        }

        private static void OnArcScaleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnArcScaleFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceArcScaleFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnArcScaleFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    //if (bLoaded)
                    {
                        UpdateDevBackColor(newValue);
                    }
                });
            }
        }

        [Category("ClockStyle")]
        public Brush ArcScaleFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ArcScaleFillProperty);
            }
            set
            {
                SetValue(ArcScaleFillProperty, value);
            }
        }
        
        #endregion
        #region StartAngle
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(Double), typeof(Clock), new UIPropertyMetadata((Double)(270), new PropertyChangedCallback(OnStartAngleChanged), new CoerceValueCallback(OnCoerceStartAngle)));

        private static object OnCoerceStartAngle(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceStartAngle((Double)value);
            else
                return value;
        }

        private static void OnStartAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnStartAngleChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceStartAngle(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartAngleChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public Double StartAngle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(StartAngleProperty);
            }
            set
            {
                SetValue(StartAngleProperty, value);
            }
        }
        #endregion
        #region EndAngle
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(Double), typeof(Clock), new UIPropertyMetadata((Double)630, new PropertyChangedCallback(OnEndAngleChanged), new CoerceValueCallback(OnCoerceEndAngle)));

        private static object OnCoerceEndAngle(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceEndAngle((Double)value);
            else
                return value;
        }

        private static void OnEndAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnEndAngleChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceEndAngle(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndAngleChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public Double EndAngle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(EndAngleProperty);
            }
            set
            {
                SetValue(EndAngleProperty, value);
            }
        }
        #endregion

        #region LabelVisible
        public static readonly DependencyProperty LabelVisibleProperty = DependencyProperty.Register("LabelVisible", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLabelVisibleChanged), new CoerceValueCallback(OnCoerceLabelVisible)));

        private static object OnCoerceLabelVisible(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceLabelVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLabelVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnLabelVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLabelVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public Boolean LabelVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(LabelVisibleProperty);
            }
            set
            {
                SetValue(LabelVisibleProperty, value);
            }
        }
        
        #endregion
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(ArcScaleLabelOrientation), typeof(Clock), new UIPropertyMetadata(ArcScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLableOrientation((ArcScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLableOrientationChanged((ArcScaleLabelOrientation)e.OldValue, (ArcScaleLabelOrientation)e.NewValue);
        }

        protected virtual ArcScaleLabelOrientation OnCoerceLableOrientation(ArcScaleLabelOrientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLableOrientationChanged(ArcScaleLabelOrientation oldValue, ArcScaleLabelOrientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public ArcScaleLabelOrientation LabelOrientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ArcScaleLabelOrientation)GetValue(LabelOrientationProperty);
            }
            set
            {
                SetValue(LabelOrientationProperty, value);
            }
        }
        
        #endregion
        #region LabelOffset
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Double), typeof(Clock), new UIPropertyMetadata((Double)(-50.0), new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelOffset((Double)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLabelOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceLabelOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public Double LabelOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(LabelOffsetProperty);
            }
            set
            {
                SetValue(LabelOffsetProperty, value);
            }
        }
        
        #endregion
        #region LabelForeground
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLabelForegroundChanged), new CoerceValueCallback(OnCoerceLabelForeground)));

        private static object OnCoerceLabelForeground(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bInit)
            {
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    circularGauge.Foreground = LabelForeground;
                });
            }
        }
        [Category("ClockStyle")]
        public Brush LabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }
            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }

        #endregion

        
        #region LabelZIndex
        public static readonly DependencyProperty LabelZIndexProperty = DependencyProperty.Register("LabelZIndex", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnLabelZIndexChanged), new CoerceValueCallback(OnCoerceLabelZIndex)));

        private static object OnCoerceLabelZIndex(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelZIndex((int)value);
            else
                return value;
        }

        private static void OnLabelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLabelZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLabelZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        public int LabelZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(LabelZIndexProperty);
            }
            set
            {
                SetValue(LabelZIndexProperty, value);
            }
        }
        #endregion  
        #region LabelFontSettings
        public static readonly DependencyProperty LabelFontSettingsProperty = DependencyProperty.Register("LabelFontSettings", typeof(FontSettings), typeof(Clock), new UIPropertyMetadata(new FontSettings(FontWeights.DemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnLabelFontSettingsChanged), new CoerceValueCallback(OnCoerceLabelFontSettings)));

        private static object OnCoerceLabelFontSettings(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnLabelFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLabelFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceLabelFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);
        }
        [Category("ClockStyle")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings LabelFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(LabelFontSettingsProperty);
            }
            set
            {
                SetValue(LabelFontSettingsProperty, value);
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
        #region LabelStringFormat
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(Clock), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceLabelStringFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelStringFormatChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ClockStyle")]
        public String LabelStringFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(LabelStringFormatProperty);
            }
            set
            {
                SetValue(LabelStringFormatProperty, value);
            }
        }
        #endregion
        #region ShowFirstLabel
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowFirstLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFirstLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ClockStyle")]
        public Boolean ShowFirstLabel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowFirstLabelProperty);
            }
            set
            {
                SetValue(ShowFirstLabelProperty, value);
            }
        }
        
        #endregion
        #region ShowLastLabel
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowLastLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLastLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("ClockStyle")]
        public Boolean ShowLastLabel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowLastLabelProperty);
            }
            set
            {
                SetValue(ShowLastLabelProperty, value);
            }
        }
        
        #endregion

        #region FlowDirection
        new public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(Clock), new UIPropertyMetadata(FlowDirection.LeftToRight, new PropertyChangedCallback(OnFlowDirectionChanged), new CoerceValueCallback(OnCoerceFlowDirection)));

        private static object OnCoerceFlowDirection(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceFlowDirection((FlowDirection)value);
            else
                return value;
        }

        private static void OnFlowDirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnFlowDirectionChanged((FlowDirection)e.OldValue, (FlowDirection)e.NewValue);
        }

        protected virtual FlowDirection OnCoerceFlowDirection(FlowDirection value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFlowDirectionChanged(FlowDirection oldValue, FlowDirection newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockStyle")]
        new public FlowDirection FlowDirection
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FlowDirection)GetValue(FlowDirectionProperty);
            }
            set
            {
                SetValue(FlowDirectionProperty, value);
            }
        }
        
        #endregion

   
     
        #endregion
        #region GaugeTickmarkStyle
        #region TickmarksPresentation
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.Progressive, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnTickmarksPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceTickmarksPresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickmarksPresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                //if (bLoaded)
                {
                    arcScale.TickmarksPresentation = GetTickmarkPresentation(TickmarksPresentation);
                }
                //});
            }
        }
        [Category("ClockTickmarkStyle")]
        public PredefinedElementKinds TickmarksPresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(TickmarksPresentationProperty);
            }
            set
            {
                SetValue(TickmarksPresentationProperty, value);
            }
        }
        #endregion

        #region MajorTickmarkFactorLength
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(Double), typeof(Clock), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkFactorLengthChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMajorTickmarkFactorLength(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFactorLengthChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public Double MajorTickmarkFactorLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MajorTickmarkFactorLengthProperty);
            }
            set
            {
                SetValue(MajorTickmarkFactorLengthProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkZIndex
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(Clock), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MajorTickmarkZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorTickmarkZIndexProperty);
            }
            set
            {
                SetValue(MajorTickmarkZIndexProperty, value);
            }
        }
        #endregion
        #region MajorTickmarkFactorThickness
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkFactorThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFactorThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MajorTickmarkFactorThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorTickmarkFactorThicknessProperty);
            }
            set
            {
                SetValue(MajorTickmarkFactorThicknessProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkOffset
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(int), typeof(Clock), new UIPropertyMetadata((int)-27, new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MajorTickmarkOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorTickmarkOffsetProperty);
            }
            set
            {
                SetValue(MajorTickmarkOffsetProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkShowFirst
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkShowFirstChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMajorTickmarkShowFirst(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkShowFirstChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public Boolean MajorTickmarkShowFirst
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MajorTickmarkShowFirstProperty);
            }
            set
            {
                SetValue(MajorTickmarkShowFirstProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkShowLast
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkShowLastChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMajorTickmarkShowLast(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkShowLastChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public Boolean MajorTickmarkShowLast
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MajorTickmarkShowLastProperty);
            }
            set
            {
                SetValue(MajorTickmarkShowLastProperty, value);
            }
        }

        #endregion



        #region MinorTickmarkFactorLength
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorLength((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkFactorLengthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkFactorLength(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorLengthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MinorTickmarkFactorLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorTickmarkFactorLengthProperty);
            }
            set
            {
                SetValue(MinorTickmarkFactorLengthProperty, value);
            }
        }

        #endregion
        #region MinorTickmarkZIndex
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(Clock), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MinorTickmarkZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorTickmarkZIndexProperty);
            }
            set
            {
                SetValue(MinorTickmarkZIndexProperty, value);
            }
        }
        #endregion
        #region MinorTickmarkOffset
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(int), typeof(Clock), new UIPropertyMetadata((int)-27, new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MinorTickmarkOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorTickmarkOffsetProperty);
            }
            set
            {
                SetValue(MinorTickmarkOffsetProperty, value);
            }
        }
        #endregion
        #region MinorTickmarkFactorThickness
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkFactorThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public int MinorTickmarkFactorThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorTickmarkFactorThicknessProperty);
            }
            set
            {
                SetValue(MinorTickmarkFactorThicknessProperty, value);
            }
        }

        #endregion
        #region MinorTickmarkShowTicksForMajor
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkShowTicksForMajorChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMinorTickmarkShowTicksForMajor(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkShowTicksForMajorChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockTickmarkStyle")]
        public Boolean MinorTickmarkShowTicksForMajor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MinorTickmarkShowTicksForMajorProperty);
            }
            set
            {
                SetValue(MinorTickmarkShowTicksForMajorProperty, value);
            }
        }

        #endregion
        #endregion
        #region SpindleStyle

        #region SpindleCapPresentation
        public static readonly DependencyProperty SpindleCapPresentationProperty = DependencyProperty.Register("SpindleCapPresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.Default, new PropertyChangedCallback(OnSpindleCapPresentationChanged), new CoerceValueCallback(OnCoerceSpindleCapPresentation)));

        private static object OnCoerceSpindleCapPresentation(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleCapPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnSpindleCapPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnSpindleCapPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceSpindleCapPresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleCapPresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (oldValue != newValue && bLoaded && bInit)
            {
                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                //if (bLoaded && bInit)
                {
                    arcScale.SpindleCapPresentation = GetSpindlePresentation(SpindleCapPresentation);
                }
                //});
            }
        }


        [Category("ClockSpindleStyle")]
        public PredefinedElementKinds SpindleCapPresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(SpindleCapPresentationProperty);
            }
            set
            {
                SetValue(SpindleCapPresentationProperty, value);
            }
        }

        #endregion
        #region SpindleFactorHeight
        public static readonly DependencyProperty SpindleFactorHeightProperty = DependencyProperty.Register("SpindleFactorHeight", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnSpindleFactorHeightChanged), new CoerceValueCallback(OnCoerceSpindleFactorHeight)));

        private static object OnCoerceSpindleFactorHeight(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleFactorHeight((int)value);
            else
                return value;
        }

        private static void OnSpindleFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnSpindleFactorHeightChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSpindleFactorHeight(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleFactorHeightChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockSpindleStyle")]
        public int SpindleFactorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SpindleFactorHeightProperty);
            }
            set
            {
                SetValue(SpindleFactorHeightProperty, value);
            }
        }
        #endregion
        #region SpindleFactorWidth
        public static readonly DependencyProperty SpindleFactorWidthProperty = DependencyProperty.Register("SpindleFactorWidth", typeof(int), typeof(Clock), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnSpindleFactorWidthChanged), new CoerceValueCallback(OnCoerceSpindleFactorWidth)));

        private static object OnCoerceSpindleFactorWidth(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleFactorWidth((int)value);
            else
                return value;
        }

        private static void OnSpindleFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnSpindleFactorWidthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSpindleFactorWidth(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleFactorWidthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockSpindleStyle")]
        public int SpindleFactorWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SpindleFactorWidthProperty);
            }
            set
            {
                SetValue(SpindleFactorWidthProperty, value);
            }
        }
        #endregion
        #region SpindleCapZIndex
        public static readonly DependencyProperty SpindleCapZIndexProperty = DependencyProperty.Register("SpindleCapZIndex", typeof(int), typeof(Clock), new UIPropertyMetadata((int)150, new PropertyChangedCallback(OnSpindleCapZIndexChanged), new CoerceValueCallback(OnCoerceSpindleCapZIndex)));

        private static object OnCoerceSpindleCapZIndex(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleCapZIndex((int)value);
            else
                return value;
        }

        private static void OnSpindleCapZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnSpindleCapZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSpindleCapZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleCapZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockSpindleStyle")]
        public int SpindleCapZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SpindleCapZIndexProperty);
            }
            set
            {
                SetValue(SpindleCapZIndexProperty, value);
            }
        }
        #endregion
        #endregion
        #region NeedleStyle

        #region NeedleHourPresentation
        public static readonly DependencyProperty NeedleHourPresentationProperty = DependencyProperty.Register("NeedleHourPresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.Cosmic, new PropertyChangedCallback(OnNeedleHourPresentationChanged), new CoerceValueCallback(OnCoerceNeedleHourPresentation)));

        private static object OnCoerceNeedleHourPresentation(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceNeedleHourPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnNeedleHourPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnNeedleHourPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceNeedleHourPresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleHourPresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                //if (bLoaded)
                {
                    hourIndicator.Presentation = GetNeedlePresentation(newValue, NeedleHourFill);
                }
                //});
            }
        }
        [Category("ClockNeedleStyle")]
        public PredefinedElementKinds NeedleHourPresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(NeedleHourPresentationProperty);
            }
            set
            {
                SetValue(NeedleHourPresentationProperty, value);
            }
        }
        
        #endregion
        #region NeedleHourFill
        public static readonly DependencyProperty NeedleHourFillProperty = DependencyProperty.Register("NeedleHourFill", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnNeedleHourFillChanged), new CoerceValueCallback(OnCoerceNeedleHourFill)));

        private static object OnCoerceNeedleHourFill(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleHourFill((Brush)value);
            else
                return value;
        }

        private static void OnNeedleHourFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleHourFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleHourFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleHourFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && oldValue != newValue && bInit)
            {
                PredefinedArcScaleNeedlePresentation layer = (PredefinedArcScaleNeedlePresentation)hourIndicator.Presentation;
                layer.Fill = newValue;

            }
        }
        [Category("ClockNeedleStyle")]
        public Brush NeedleHourFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleHourFillProperty);
            }
            set
            {
                SetValue(NeedleHourFillProperty, value);
            }
        }

        #endregion
        #region NeedleMinutePresentation
        public static readonly DependencyProperty NeedleMinutePresentationProperty = DependencyProperty.Register("NeedleMinutePresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.Cosmic, new PropertyChangedCallback(OnNeedleMinutePresentationChanged), new CoerceValueCallback(OnCoerceNeedleMinutePresentation)));

        private static object OnCoerceNeedleMinutePresentation(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceNeedleMinutePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnNeedleMinutePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnNeedleMinutePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceNeedleMinutePresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleMinutePresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                //if (bLoaded)
                {
                    minuteIndicator.Presentation = GetNeedlePresentation(newValue, NeedleMinuteFill);
                }
                //});
            }
        }
        [Category("ClockNeedleStyle")]
        public PredefinedElementKinds NeedleMinutePresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(NeedleMinutePresentationProperty);
            }
            set
            {
                SetValue(NeedleMinutePresentationProperty, value);
            }
        }

        #endregion
        #region NeedleMinuteFill
        public static readonly DependencyProperty NeedleMinuteFillProperty = DependencyProperty.Register("NeedleMinuteFill", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnNeedleMinuteFillChanged), new CoerceValueCallback(OnCoerceNeedleMinuteFill)));

        private static object OnCoerceNeedleMinuteFill(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleMinuteFill((Brush)value);
            else
                return value;
        }

        private static void OnNeedleMinuteFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleMinuteFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleMinuteFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleMinuteFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && oldValue != newValue && bInit)
            {
                PredefinedArcScaleNeedlePresentation layer = (PredefinedArcScaleNeedlePresentation)minuteIndicator.Presentation;
                layer.Fill = newValue;
            }
        }
        [Category("ClockNeedleStyle")]
        public Brush NeedleMinuteFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleMinuteFillProperty);
            }
            set
            {
                SetValue(NeedleMinuteFillProperty, value);
            }
        }

        #endregion
        #region NeedleSecondPresentation
        public static readonly DependencyProperty NeedleSecondPresentationProperty = DependencyProperty.Register("NeedleSecondPresentation", typeof(PredefinedElementKinds), typeof(Clock), new UIPropertyMetadata(PredefinedElementKinds.RedClock, new PropertyChangedCallback(OnNeedleSecondPresentationChanged), new CoerceValueCallback(OnCoerceNeedleSecondPresentation)));

        private static object OnCoerceNeedleSecondPresentation(DependencyObject o, object value)
        {
            Clock clock = o as Clock;
            if (clock != null)
                return clock.OnCoerceNeedleSecondPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnNeedleSecondPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock clock = o as Clock;
            if (clock != null)
                clock.OnNeedleSecondPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceNeedleSecondPresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleSecondPresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                //Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                //{
                //if (bLoaded)
                {
                    secondIndicator.Presentation = GetNeedlePresentation(newValue, NeedleSecondFill);
                }
                //});
            }
        }
        [Category("ClockNeedleStyle")]
        public PredefinedElementKinds NeedleSecondPresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(NeedleSecondPresentationProperty);
            }
            set
            {
                SetValue(NeedleSecondPresentationProperty, value);
            }
        }

        #endregion
        #region NeedleSecondFill
        public static readonly DependencyProperty NeedleSecondFillProperty = DependencyProperty.Register("NeedleSecondFill", typeof(Brush), typeof(Clock), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnNeedleSecondFillChanged), new CoerceValueCallback(OnCoerceNeedleSecondFill)));

        private static object OnCoerceNeedleSecondFill(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleSecondFill((Brush)value);
            else
                return value;
        }

        private static void OnNeedleSecondFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleSecondFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleSecondFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleSecondFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && oldValue != newValue && bInit)
            {
                PredefinedArcScaleNeedlePresentation layer = (PredefinedArcScaleNeedlePresentation)secondIndicator.Presentation;
                layer.Fill = newValue;
            }
        }
        [Category("ClockNeedleStyle")]
        public Brush NeedleSecondFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleSecondFillProperty);
            }
            set
            {
                SetValue(NeedleSecondFillProperty, value);
            }
        }

        #endregion

        #region NeedleHourVisible
        public static readonly DependencyProperty NeedleHourVisibleProperty = DependencyProperty.Register("NeedleHourVisible", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleHourVisibleChanged), new CoerceValueCallback(OnCoerceNeedleHourVisible)));

        private static object OnCoerceNeedleHourVisible(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleHourVisible((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleHourVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleHourVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleHourVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleHourVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockNeedleStyle")]
        public Boolean NeedleHourVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleHourVisibleProperty);
            }
            set
            {
                SetValue(NeedleHourVisibleProperty, value);
            }
        }

        #endregion
        #region NeedleMinuteVisible
        public static readonly DependencyProperty NeedleMinuteVisibleProperty = DependencyProperty.Register("NeedleMinuteVisible", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleMinuteVisibleChanged), new CoerceValueCallback(OnCoerceNeedleMinuteVisible)));

        private static object OnCoerceNeedleMinuteVisible(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleMinuteVisible((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleMinuteVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleMinuteVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleMinuteVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleMinuteVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockNeedleStyle")]
        public Boolean NeedleMinuteVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleMinuteVisibleProperty);
            }
            set
            {
                SetValue(NeedleMinuteVisibleProperty, value);
            }
        }

        #endregion
        #region NeedleSecondVisible
        public static readonly DependencyProperty NeedleSecondVisibleProperty = DependencyProperty.Register("NeedleSecondVisible", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleSecondVisibleChanged), new CoerceValueCallback(OnCoerceNeedleSecondVisible)));

        private static object OnCoerceNeedleSecondVisible(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleSecondVisible((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleSecondVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleSecondVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleSecondVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleSecondVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockNeedleStyle")]
        public Boolean NeedleSecondVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleSecondVisibleProperty);
            }
            set
            {
                SetValue(NeedleSecondVisibleProperty, value);
            }
        }

        #endregion
        #region NeedleAnimationEnable
        public static readonly DependencyProperty NeedleAnimationEnableProperty = DependencyProperty.Register("NeedleAnimationEnable", typeof(Boolean), typeof(Clock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleAnimationEnableChanged), new CoerceValueCallback(OnCoerceNeedleAnimationEnable)));

        private static object OnCoerceNeedleAnimationEnable(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockNeedleStyle")]
        public Boolean NeedleAnimationEnable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleAnimationEnableProperty);
            }
            set
            {
                SetValue(NeedleAnimationEnableProperty, value);
            }
        }

        #endregion
        #region NeedleZIndex
        public static readonly DependencyProperty NeedleZIndexProperty = DependencyProperty.Register("NeedleZIndex", typeof(int), typeof(Clock), new UIPropertyMetadata((int)150, new PropertyChangedCallback(OnNeedleZIndexChanged), new CoerceValueCallback(OnCoerceNeedleZIndex)));

        private static object OnCoerceNeedleZIndex(DependencyObject o, object value)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleZIndex((int)value);
            else
                return value;
        }

        private static void OnNeedleZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Clock circularGauge = o as Clock;
            if (circularGauge != null)
                circularGauge.OnNeedleZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceNeedleZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("ClockNeedleStyle")]
        public int NeedleZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(NeedleZIndexProperty);
            }
            set
            {
                SetValue(NeedleZIndexProperty, value);
            }
        }
        #endregion
       #endregion
        #endregion

        #region Declarations
        public IEnumerable<PredefinedElementKind> PredefinedCircularGaugeModelKinds { get { return CircularGaugeControl.PredefinedModels; } }
        bool bLoaded = false;
        private DispatcherTimer _updateClock;
        private DispatcherTimer updateLayer;

        bool bInit;
        bool bDesign;

        IDocument document;

        private ReadOnlyCollection<TimeZoneInfo> Currents = TimeZoneInfo.GetSystemTimeZones();
        #endregion

        #region Constructors
        public Clock()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            circularGauge.DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(this);


                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        bDesign = true;
                        UpdateFill();


                        if (ClockTimeZone == null)
                            ClockTimeZone = new Gauges.ClockTimeZone() { TimeZone = TimeZoneInfo.Local };

                        circularGauge.IsEnabled = false;
                        OverrideBaseProperties();
                        bInit = true;
                    }
                    else
                    {
                        circularGauge.IsEnabled = true;
                        bNeedleAnimationEnable = NeedleAnimationEnable;

                        UpdateFill();

                        if (_updateClock == null)
                        {
                            _updateClock = new DispatcherTimer(DispatcherPriority.Render);
                            _updateClock.Interval = TimeSpan.FromMilliseconds(958);
                            _updateClock.Tick += DisplayAfterTimerElapsed;
                        }

                        if (ClockTimeZone == null)
                            ClockTimeZone = new Gauges.ClockTimeZone() { TimeZone = TimeZoneInfo.Local };

                        _updateClock.Start();
                        OverrideBaseProperties();
                        bInit = true;
                    }
                }

            };
        }
        #endregion

        #region Methods

        PredefinedElementKinds defGaugeBaseModel = PredefinedElementKinds.Eco;
        PredefinedElementKinds defTickmarksPresentation = PredefinedElementKinds.Progressive;
        PredefinedElementKinds defSpindleCapPresentation = PredefinedElementKinds.Default;

        private void UpdateFill()
        {
            if (bLoaded)
            {
                if (defGaugeBaseModel != ArcScalePresentation)
                    circularGauge.Model = GetDevBackground(ArcScalePresentation);

                EnableBackLayer(ArcScalePresentation);

                if (TickmarksPresentation != null && defTickmarksPresentation != TickmarksPresentation)
                    arcScale.TickmarksPresentation = GetTickmarkPresentation(TickmarksPresentation);
                if (SpindleCapPresentation != null && defSpindleCapPresentation != SpindleCapPresentation)
                    arcScale.SpindleCapPresentation = GetSpindlePresentation(SpindleCapPresentation);

                circularGauge.Foreground = LabelForeground;

                hourIndicator.Presentation = GetNeedlePresentation(NeedleHourPresentation, NeedleHourFill);
                //PredefinedArcScaleNeedlePresentation hlayer = (PredefinedArcScaleNeedlePresentation)hourIndicator.Presentation;
                //if (hlayer != null)
                //    hlayer.Fill = NeedleHourFill;

                minuteIndicator.Presentation = GetNeedlePresentation(NeedleMinutePresentation, NeedleMinuteFill);
                //PredefinedArcScaleNeedlePresentation mlayer = (PredefinedArcScaleNeedlePresentation)minuteIndicator.Presentation;
                //if (mlayer != null)
                //    mlayer.Fill = NeedleMinuteFill;

                secondIndicator.Presentation = GetNeedlePresentation(NeedleSecondPresentation, NeedleSecondFill);
                //PredefinedArcScaleNeedlePresentation slayer = (PredefinedArcScaleNeedlePresentation)secondIndicator.Presentation;
                //if (slayer != null)
                //    slayer.Fill = NeedleSecondFill;

                UpdateDevBackColor(ArcScaleFill);
            }
        }

        private ArcScaleNeedlePresentation GetNeedlePresentation(PredefinedElementKinds value, Brush fill)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScaleNeedle.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                ArcScaleNeedlePresentation layer = (ArcScaleNeedlePresentation)Activator.CreateInstance(gaugeModelKind.Type);
                PredefinedArcScaleNeedlePresentation hlayer = (PredefinedArcScaleNeedlePresentation)layer;
                if (hlayer != null)
                    hlayer.Fill = fill;
                return layer;
            }
            catch
            {
                ArcScaleNeedlePresentation layer = (ArcScaleNeedlePresentation)Activator.CreateInstance(((ArcScaleNeedle.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
                PredefinedArcScaleNeedlePresentation hlayer = (PredefinedArcScaleNeedlePresentation)layer;
                if (hlayer != null)
                    hlayer.Fill = fill;
                return layer;
            }
        }
        private SpindleCapPresentation GetSpindlePresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScale.PredefinedSpindleCapPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (SpindleCapPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (SpindleCapPresentation)Activator.CreateInstance(((ArcScale.PredefinedSpindleCapPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
            }
        }
        private TickmarksPresentation GetTickmarkPresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScale.PredefinedTickmarksPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (TickmarksPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (TickmarksPresentation)Activator.CreateInstance(((ArcScale.PredefinedTickmarksPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)TickmarksPresentation) as PredefinedElementKind).Type);
            }
        }

        private void EnableBackLayer(PredefinedElementKinds value)
        {
            if (!EnableBackGroundLayer)
            {
                arcLayer.Visible = false;
                arcLayer.Presentation = null;
            }
            else
            {
                arcLayer.Presentation = GetArcScaleBackground(value);
                arcLayer.Visible = true;
            }
        }
        private ArcScaleLayerPresentation GetArcScaleBackground(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value * 5) as PredefinedElementKind;
                return (ArcScaleLayerPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (ArcScaleLayerPresentation)Activator.CreateInstance(((ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
            }
        }

        private void UpdateDevBackColor(Brush newValue)
        {
            PredefinedArcScaleLayerPresentation layer = (PredefinedArcScaleLayerPresentation)arcLayer.Presentation;
            if (layer != null)
                layer.Fill = newValue;
        }

        bool bNeedleAnimationEnable;

        private void DisplayAfterTimerElapsed(object sender, EventArgs e)
        {

            DateTime date;
            if (UseSystemTimeZone)
            {
                if (RunningOnServer)
                    date = DateTime.UtcNow.AddMinutes(ClientTimezoneOffset);
                else
                    date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            }
            else
            {
                if (ClockTimeZone == null)
                    ClockTimeZone = new Gauges.ClockTimeZone() { TimeZone = TimeZoneInfo.Local };
                date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ClockTimeZone.TimeZone);
            }

            double hValue = date.Hour % 12 + date.Minute / 60.0;
            double mValue = ((date.Minute + date.Second / 60.0) / 60.0) * 12;
            double sValue = (date.Second / 60.0) * 12;

            if (hValue < hourIndicator.Value)
            {
                bNeedleAnimationEnable = NeedleAnimationEnable;
                NeedleAnimationEnable = false;
            }
            else
            {
                NeedleAnimationEnable = bNeedleAnimationEnable;
            }

            hourIndicator.Value = hValue;
            minuteIndicator.Value = mValue;
            secondIndicator.Value = sValue;

            var hour = date.Hour % 24;
            var minute = date.Minute % 60;
            var second = date.Second;
            ClockText.Text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
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
                return (int)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
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
            if (_updateClock != null)
            {
                _updateClock.Stop();
                _updateClock.Tick -= DisplayAfterTimerElapsed;
                _updateClock = null;
            }
                
            DetachOverrideBaseProperties();
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

                // Defines Data Template for 'RecipeNameProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(TimeZonePropertyEditor));
                dt.DataType = typeof(ClockTimeZone);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ClockTimeZoneProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion
    }
}
