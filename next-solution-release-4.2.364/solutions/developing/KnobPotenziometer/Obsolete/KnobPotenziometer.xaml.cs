using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Converters;
using DevExpress.Xpf.Gauges;
using KnobPotenziometer.Automations;
using Utilities;
using OPCUAViewModel;
using Opc.Ua;
using System.Globalization;
using ViewModelLib;
using System.Windows.Media.Effects;
using ScreenSettings;
using UFInterfaces;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using Utilities.WPF;
using System.IO;
using KnobPotenziometer.Enums;
using DynamicTagAwareHelper;
using System.Threading.Tasks;
using StringManager.ComponentService;
using System.Threading;
using System.Xml.Serialization;
using WPFUtilities.Extensions;

namespace KnobPotenziometer
{
    /// <summary>
    /// Interaction logic for KnobPotenziometer.xaml
    /// </summary>
    [Obsolete("Use the KnobPotenziometer.KnobExpress.xaml insted of this.")]
    public partial class KnobPotenziometer : UserControl, IDisposable, IEntityReference, IDynamicTagAware
    {
        #region Dependency Properties
        #region Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnIsEnabledChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(KnobPotenziometer));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnIsEnabledChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(KnobPotenziometer));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            {
                var value = LabelFontSettings.Clone();
                value.FontFamily = FontFamily;
                LabelFontSettings = value;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            {
                var value = LabelFontSettings.Clone();
                value.FontWeight = FontWeight;
                LabelFontSettings = value;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            {
                var value = LabelFontSettings.Clone();
                value.FontStyle = FontStyle;
                LabelFontSettings = value;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            {
                var value = LabelFontSettings.Clone();
                value.FontSize = (int)FontSize;
                LabelFontSettings = value;
            }
        }
        void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesign)
            {
                UpdateGuageControl();
                UpdateCustomElements();
            }
        }
        private void OnIsEnabledChanged(object sender, EventArgs e)
        {
            var control = sender as KnobPotenziometer;
            if (control != null)
            {
                control.OnIsEnabledChanged();
            }
        }
        protected virtual void OnIsEnabledChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            container.Opacity = IsEnabled ? 1.0 : 0.7;
        }
        #endregion

        #region ShowLabels
        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLabelsChanged), new CoerceValueCallback(OnCoerceShowLabels)));

        private static object OnCoerceShowLabels(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceShowLabels((bool)value);
            else
                return value;
        }

        private static void OnShowLabelsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
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
        }
        [Category("KnobStyle")]
        [Browsable(true)]
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

        #region ShowFirstLabel
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowFirstLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFirstLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowLastLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLastLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]
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
        #region MajorTickBrush
        public static readonly DependencyProperty MajorTickBrushProperty = DependencyProperty.Register("MajorTickBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.Gray), new PropertyChangedCallback(OnMajorTickBrushChanged), new CoerceValueCallback(OnCoerceMajorTickBrush)));

        private static object OnCoerceMajorTickBrush(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMajorTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnMajorTickBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMajorTickBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]
        public Brush MajorTickBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MajorTickBrushProperty);
            }
            set
            {
                SetValue(MajorTickBrushProperty, value);
            }
        }

        #endregion
        #region MinorTickBrush
        public static readonly DependencyProperty MinorTickBrushProperty = DependencyProperty.Register("MinorTickBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.Gray), new PropertyChangedCallback(OnMinorTickBrushChanged), new CoerceValueCallback(OnCoerceMinorTickBrush)));

        private static object OnCoerceMinorTickBrush(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMinorTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnMinorTickBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMinorTickBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]

        public Brush MinorTickBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MinorTickBrushProperty);
            }
            set
            {
                SetValue(MinorTickBrushProperty, value);
            }
        }

        #endregion
        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            KnobPotenziometer circularGaugeObject = o as KnobPotenziometer;
            if (circularGaugeObject != null)
                return circularGaugeObject.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGaugeObject = o as KnobPotenziometer;
            if (circularGaugeObject != null)
                circularGaugeObject.OnValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateValue();
        }

        [Category("KnobStyle")]
        [Browsable(false)]
        [XmlIgnore]
        public Double Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        #endregion

        #region ValueStringFormat 
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(KnobPotenziometer), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnValueStringFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceValueStringFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueStringFormatChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("ValueStyle")]
        public String ValueStringFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ValueStringFormatProperty);
            }
            set
            {
                SetValue(ValueStringFormatProperty, value);
            }
        }

        #endregion

        #region MinorIntervalCount
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(KnobPotenziometer), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnMinorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorIntervalCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorIntervalCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]

        public int MinorIntervalCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MinorIntervalCountProperty);
            }
            set
            {
                SetValue(MinorIntervalCountProperty, value);
            }
        }

        #endregion
        #region MajorIntervalCount
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(KnobPotenziometer), new UIPropertyMetadata((int)6, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnMajorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorIntervalCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorIntervalCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
        [Browsable(true)]

        public int MajorIntervalCount
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorIntervalCountProperty);
            }
            set
            {
                SetValue(MajorIntervalCountProperty, value);
            }
        }

        #endregion

        #region LabelStringFormat
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(KnobPotenziometer), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceLabelStringFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelStringFormatChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
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
        #region LabelFontSettings
        public static readonly DependencyProperty LabelFontSettingsProperty = DependencyProperty.Register("LabelFontSettings", typeof(FontSettings), typeof(KnobPotenziometer), new UIPropertyMetadata(new FontSettings(FontWeights.DemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 12), new PropertyChangedCallback(OnLabelFontSettingsChanged), new CoerceValueCallback(OnCoerceLabelFontSettings)));

        private static object OnCoerceLabelFontSettings(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceLabelFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnLabelFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnLabelFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceLabelFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("KnobStyle")]
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


        #endregion
        #region LabelForeground
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.DimGray)));
        [Browsable(false)]
        [XmlIgnore]
        [Category("KnobStyle")]
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
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(ArcScaleLabelOrientation), typeof(KnobPotenziometer), new UIPropertyMetadata(ArcScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceLableOrientation((ArcScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnLableOrientationChanged((ArcScaleLabelOrientation)e.OldValue, (ArcScaleLabelOrientation)e.NewValue);
        }

        protected virtual ArcScaleLabelOrientation OnCoerceLableOrientation(ArcScaleLabelOrientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLableOrientationChanged(ArcScaleLabelOrientation oldValue, ArcScaleLabelOrientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("KnobStyle")]
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
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)(-12), new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceLabelOffset((Double)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnLabelOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceLabelOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("KnobStyle")]
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

        #region NeedleBorderBrush
        public static readonly DependencyProperty NeedleBorderBrushProperty = DependencyProperty.Register("NeedleBorderBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.DimGray), new PropertyChangedCallback(OnNeedleBorderBrushChanged), new CoerceValueCallback(OnCoerceNeedleBorderBrush)));

        private static object OnCoerceNeedleBorderBrush(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceNeedleBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnNeedleBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnNeedleBorderBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleBorderBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleBorderBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

        }
        [Category("KnobStyle")]
        [Browsable(true)]
        public Brush NeedleBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleBorderBrushProperty);
            }
            set
            {
                SetValue(NeedleBorderBrushProperty, value);
            }
        }

        #endregion
        #region NeedleBackBrush
        public static readonly DependencyProperty NeedleBackBrushProperty = DependencyProperty.Register("NeedleBackBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkGray), new PropertyChangedCallback(OnNeedleBackBrushChanged), new CoerceValueCallback(OnCoerceNeedleBackBrush)));

        private static object OnCoerceNeedleBackBrush(DependencyObject o, object value)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceNeedleBackBrush((Brush)value);
            else
                return value;
        }

        private static void OnNeedleBackBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer knobPotenziometer = o as KnobPotenziometer;
            if (knobPotenziometer != null)
                knobPotenziometer.OnNeedleBackBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleBackBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleBackBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

        }
        [Category("KnobStyle")]
        [Browsable(true)]
        public Brush NeedleBackBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleBackBrushProperty);
            }
            set
            {
                SetValue(NeedleBackBrushProperty, value);
            }
        }

        #endregion
        #region BackgroundBrush
        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnBackgroundBrushChanged), new CoerceValueCallback(OnCoerceBackgroundBrush)));

        private static object OnCoerceBackgroundBrush(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceBackgroundBrush((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnBackgroundBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceBackgroundBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBackgroundBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

        }
        [Category("KnobStyle")]
        public Brush BackgroundBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BackgroundBrushProperty);
            }
            set
            {
                SetValue(BackgroundBrushProperty, value);
            }
        }


        #endregion
        #region BackgroundBorderBrush
        public static readonly DependencyProperty BackgroundBorderBrushProperty = DependencyProperty.Register("BackgroundBorderBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.Silver), new PropertyChangedCallback(OnBackgroundBorderBrushChanged), new CoerceValueCallback(OnCoerceBackgroundBorderBrush)));

        private static object OnCoerceBackgroundBorderBrush(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceBackgroundBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnBackgroundBorderBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceBackgroundBorderBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBackgroundBorderBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("KnobStyle")]
        public Brush BackgroundBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BackgroundBorderBrushProperty);
            }
            set
            {
                SetValue(BackgroundBorderBrushProperty, value);
            }
        }
        #endregion
        #region RingBackBrush
        public static readonly DependencyProperty RingBackBrushProperty = DependencyProperty.Register("RingBackBrush", typeof(Brush), typeof(KnobPotenziometer), new UIPropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnRingBackBrushChanged), new CoerceValueCallback(OnCoerceRingBackBrush)));

        private static object OnCoerceRingBackBrush(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceRingBackBrush((Brush)value);
            else
                return value;
        }

        private static void OnRingBackBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnRingBackBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRingBackBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRingBackBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("KnobStyle")]
        public Brush RingBackBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RingBackBrushProperty);
            }
            set
            {
                SetValue(RingBackBrushProperty, value);
            }
        }

        #endregion
        #region KnobRadius
        public static readonly DependencyProperty KnobRadiusProperty = DependencyProperty.Register("KnobRadius", typeof(double), typeof(KnobPotenziometer), new UIPropertyMetadata(76.0, new PropertyChangedCallback(OnKnobRadiusChanged), new CoerceValueCallback(OnCoerceKnobRadius)));

        private static object OnCoerceKnobRadius(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceKnobRadius((double)value);
            else
                return value;
        }

        private static void OnKnobRadiusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnKnobRadiusChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceKnobRadius(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnKnobRadiusChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("KnobStyle")]
        public double KnobRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(KnobRadiusProperty);
            }
            set
            {
                SetValue(KnobRadiusProperty, value);
            }
        }

        #endregion



        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(KnobPotenziometer), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnMinValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit && !bDatacontextChanging))
            {
                UpdateRanges();
            }
        }
        [Category("Advanced")]
        public double MinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        #endregion
        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(KnobPotenziometer), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnMaxValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMaxValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit && !bDatacontextChanging))
            {
                UpdateRanges();
            }
        }
        [Category("Advanced")]
        public double MaxValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion
        #region UseEUnit
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(KnobPotenziometer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));

        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnUseEUnitChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseEUnit(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseEUnitChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("Advanced")]
        public bool UseEUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseEUnitProperty);
            }
            set
            {
                SetValue(UseEUnitProperty, value);
            }
        }

        #endregion

        #region TagMinValue
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(KnobPotenziometer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMinValueChanged), new CoerceValueCallback(OnCoerceTagMinValue)));

        private static object OnCoerceTagMinValue(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceTagMinValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnTagMinValueChanged((OPCUAXMLEntityReference)e.OldValue, (OPCUAXMLEntityReference)e.NewValue);
        }

        protected virtual OPCUAXMLEntityReference OnCoerceTagMinValue(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagMinValueChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public OPCUAXMLEntityReference TagMinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (OPCUAXMLEntityReference)GetValue(TagMinValueProperty);
            }
            set
            {
                SetValue(TagMinValueProperty, value);
            }
        }

        #endregion
        #region TagMaxValue
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(KnobPotenziometer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMaxValueChanged), new CoerceValueCallback(OnCoerceTagMaxValue)));

        private static object OnCoerceTagMaxValue(DependencyObject o, object value)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                return control.OnCoerceTagMaxValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer control = o as KnobPotenziometer;
            if (control != null)
                control.OnTagMaxValueChanged((OPCUAXMLEntityReference)e.OldValue, (OPCUAXMLEntityReference)e.NewValue);
        }

        protected virtual OPCUAXMLEntityReference OnCoerceTagMaxValue(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagMaxValueChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public OPCUAXMLEntityReference TagMaxValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (OPCUAXMLEntityReference)GetValue(TagMaxValueProperty);
            }
            set
            {
                SetValue(TagMaxValueProperty, value);
            }
        }

        #endregion

        #region KnobOptions
        #region StartValue
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnStartValueChanged), new CoerceValueCallback(OnCoerceStartValue)));

        private static object OnCoerceStartValue(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceStartValue((Double)value);
            else
                return value;
        }

        private static void OnStartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnStartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceStartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("KnobOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public Double StartValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(StartValueProperty);
            }
            set
            {
                SetValue(StartValueProperty, value);
            }
        }

        #endregion

        #region EndValue
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register("EndValue", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)100.0, new PropertyChangedCallback(OnEndValueChanged), new CoerceValueCallback(OnCoerceEndValue)));

        private static object OnCoerceEndValue(DependencyObject o, object value)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                return KnobPotenziometer.OnCoerceEndValue((Double)value);
            else
                return value;
        }

        private static void OnEndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer KnobPotenziometer = o as KnobPotenziometer;
            if (KnobPotenziometer != null)
                KnobPotenziometer.OnEndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceEndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("KnobOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public Double EndValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(EndValueProperty);
            }
            set
            {
                SetValue(EndValueProperty, value);
            }
        }

        #endregion

        #endregion

        #region TickmarkOptions
        #region TickmarksPresentation
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(KnobPotenziometer), new UIPropertyMetadata(PredefinedElementKinds.Smart, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(KnobPotenziometer), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkFactorThicknessChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMajorTickmarkFactorThickness(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFactorThicknessChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
        public Double MajorTickmarkFactorThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MajorTickmarkFactorThicknessProperty);
            }
            set
            {
                SetValue(MajorTickmarkFactorThicknessProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkOffset
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)(-3.5), new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                circularGauge.OnMajorTickmarkOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMajorTickmarkOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
        public Double MajorTickmarkOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MajorTickmarkOffsetProperty);
            }
            set
            {
                SetValue(MajorTickmarkOffsetProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkShowFirst
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)1, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkFactorLengthChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMinorTickmarkFactorLength(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorLengthChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
        public Double MinorTickmarkFactorLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MinorTickmarkFactorLengthProperty);
            }
            set
            {
                SetValue(MinorTickmarkFactorLengthProperty, value);
            }
        }

        #endregion
        #region MinorTickmarkZIndex
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(KnobPotenziometer), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)(-3.5), new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMinorTickmarkOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
        public Double MinorTickmarkOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MinorTickmarkOffsetProperty);
            }
            set
            {
                SetValue(MinorTickmarkOffsetProperty, value);
            }
        }
        #endregion
        #region MinorTickmarkFactorThickness
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(Double), typeof(KnobPotenziometer), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                circularGauge.OnMinorTickmarkFactorThicknessChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMinorTickmarkFactorThickness(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorThicknessChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
        public Double MinorTickmarkFactorThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MinorTickmarkFactorThicknessProperty);
            }
            set
            {
                SetValue(MinorTickmarkFactorThicknessProperty, value);
            }
        }

        #endregion
        #region MinorTickmarkShowTicksForMajor
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(KnobPotenziometer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobPotenziometer circularGauge = o as KnobPotenziometer;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeTickmarkStyle")]
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

     
        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion

        #region Declarations
        List<string> matchChangedMap = new List<string>();
        IStringEditorManager stringManager;
        bool bLoaded;
        bool bDatacontextChanging;
        double _StartValue;
        double _EndValue;
        bool bDesign;

        bool bInit;
        TypeHelper typeHelper = new TypeHelper();
        CancellationTokenSource cts;
        SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        bool isChangingValue;
        bool isUserMouseDownAction;
        ArcScaleNeedle selectedNeedle = null;
        ScreenDocument Document;
        string ConverterLabel;
        MonitoredItemViewModel monitoredItemViewModel;
        bool bTagMinValueSet;
        bool bTagMaxValueSet;
        #endregion

        #region DynObjects
        CircularGaugeControl circularGaugeObject;
        ArcScale arcScale;
        ArcScaleLayer arcLayer;
        ArcScaleNeedle arcNeedle;
        ScaleCustomElement gridValueContainer;
        ContentControl BackContent;
        #endregion

        #region Constructor

        public KnobPotenziometer()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            if (circularGaugeObject != null)
                circularGaugeObject.DataContext = this;


            OverrideBaseProperties();

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

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

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;
                    if (this.ReadLocalValue(LabelForegroundProperty) != DependencyProperty.UnsetValue)
                        Foreground = LabelForeground;

                    if (!bDesign)
                        InitControl();

                    UpdateGaugeLayout();

                    bInit = true;
                }
            };
            DataContextChanged += (o, e) =>
            {
                if (bDispose)
                    return;
                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this) && !bDesign)
                {
                    bDatacontextChanging = true;
                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    UpdateRanges();
                }
            };
        }

        protected void UpdateRanges()
        {
            double startValue = MinValue;
            double endValue = MaxValue;
            Action action = () =>
            {
                if (UseEUnit && monitoredItemViewModel != null)
                    if (monitoredItemViewModel.HasRange)
                    {
                        if (TagMinValue == null || TagMinValue.TagReference == null)
                            startValue = monitoredItemViewModel.Range.Low;
                        if (TagMaxValue == null || TagMaxValue.TagReference == null)
                            endValue = monitoredItemViewModel.Range.High;
                    }

                if (TagMinValue == null || TagMinValue.TagReference == null || !bTagMinValueSet)
                    _StartValue = startValue;
                if (TagMaxValue == null || TagMaxValue.TagReference == null || !bTagMaxValueSet)
                    _EndValue = endValue;

                bDatacontextChanging = false;
                UpdateScales();
            };

            if (UseEUnit)
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                var token = cts.Token;
                var task1 = Task.Factory.StartNew(() =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    var eu = monitoredItemViewModel?.HasRange;
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                var task2 = task1.ContinueWith(ret =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    action();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                action();
        }
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            if (bDesign || stringManager == null)
                return;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                UpdateCustomValue(Value);
                if (arcScale != null)
                {
                    arcScale.LabelOptions.FormatString = "";
                    arcScale.LabelOptions.FormatString = LabelStringFormat;
                }
            });
        }
        Point oldposition;
        bool IsInEditMode;
        public bool HasTouchInput()
        {
            foreach (TabletDevice tabletDevice in Tablet.TabletDevices)
            {
                //Only detect if it is a touch Screen not how many touches (i.e. Single touch or Multi-touch)
                if (tabletDevice.Type == TabletDeviceType.Touch)
                    return true;
            }

            return false;
        }

        #endregion

        #region Methods
        private void UpdateGaugeLayout()
        {
            if (bDispose || bDatacontextChanging)
                return;

            UpdateStartEndValues();

            UpdateGuageControl();
            UpdateArcScale();
            UpdateArcLayer();
            UpdateNeedle();
            UpdateCustomElements();
            UpdateValue();

            //if (bDesign)
                //container.IsHitTestVisible = false;
        }
        private void UpdateCustomElements()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (gridValueContainer == null)
                {
                    gridValueContainer = new ScaleCustomElement()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        ZIndex = 500
                    };

                    Grid _grid = new Grid();
                    StackPanel _stack = new StackPanel() { Orientation = Orientation.Vertical };
                    _stack.VerticalAlignment = VerticalAlignment.Center;
                    TextBlock _unitLabel = new TextBlock()
                    {
                        Name = "tvalue",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        IsHitTestVisible = false,
                        TextAlignment = TextAlignment.Center,
                        Background = transparent,
                        Width = 40
                    };

                    TextBox _text = new TextBox()
                    {
                        Name = "tvalue",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        IsReadOnly = true,
                        IsHitTestVisible = false,
                        TextAlignment = TextAlignment.Center,
                        Background = transparent,
                        BorderThickness = new Thickness(0),
                        Width = 40
                    };


                    if (HasTouchInput() && !RunningOnServer)
                    {
                        _text.IsReadOnly = false;
                        _text.IsHitTestVisible = true;
                        _text.PreviewTouchDown += (sender, ev) =>
                        {
                            IsInEditMode = true;
                        };
                        _text.PreviewKeyDown += (sender, ev) =>
                        {
                            if (ev.Key != Key.Enter || ev.Key != Key.Return)
                            {
                                ev.Handled = false;
                                return;
                            }

                            ev.Handled = true;
                            IsInEditMode = false;
                            double _value = Value;
                            if (Double.TryParse((sender as TextBox).Text, out _value))
                            {
                                if (_value >= MinValue && _value <= MaxValue)
                                    Value = _value;
                            }
                            (sender as TextBox).SelectAll();
                        };
                        _text.LostFocus += (sender, ev) =>
                        {
                            IsInEditMode = false;
                            double _value = Value;
                            if (Double.TryParse((sender as TextBox).Text, out _value))
                            {
                                if (_value >= MinValue && _value <= MaxValue)
                                    Value = _value;
                            }

                        };
                        _text.LostKeyboardFocus += (sender, ev) =>
                        {
                            IsInEditMode = false;
                            double _value = Value;
                            if (Double.TryParse((sender as TextBox).Text, out _value))
                            {
                                if (_value >= MinValue && _value <= MaxValue)
                                    Value = _value;
                            }

                        };
                    }


                    _stack.Children.Add(_text);
                    _stack.Children.Add(_unitLabel);
                    _grid.Children.Add(_stack);
                    gridValueContainer.Content = _grid;

                    if (!arcScale.CustomElements.Contains(gridValueContainer))
                        arcScale.CustomElements.Add(gridValueContainer);
                }
                Grid grid = (Grid)gridValueContainer.Content;
                if (grid != null)
                {
                    TextBox text = grid.GetChildrenOfType<TextBox>().FirstOrDefault();
                    TextBlock label = grid.GetChildrenOfType<TextBlock>().FirstOrDefault();

                    label.FontSize = text.FontSize = LabelFontSettings.FontSize;
                    label.FontWeight = text.FontWeight = LabelFontSettings.FontWeight;
                    label.FontFamily = text.FontFamily = LabelFontSettings.FontFamily;
                    label.FontStyle = text.FontStyle = LabelFontSettings.FontStyle;
                    label.Foreground = text.Foreground = Foreground;
                    label.Text = ConverterLabel;
                    label.Visibility = string.IsNullOrEmpty(ConverterLabel) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateNeedle()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (arcNeedle == null)
                {
                    arcNeedle = new ArcScaleNeedle();
                    arcNeedle.Options = new ArcScaleNeedleOptions();
                    arcNeedle.IsInteractive = true;
                    arcNeedle.Options.EndOffset = 20;
                    arcNeedle.Options.ZIndex = 400;

                    CustomArcScaleNeedlePresentation scaleneedlepresentation = new CustomArcScaleNeedlePresentation();
                    scaleneedlepresentation.NeedleTemplate = LoadTemplate("OscilloscopeTopNeedleTemplate");
                    arcNeedle.Presentation = scaleneedlepresentation;


                    if (!arcScale.Needles.Contains(arcNeedle))
                        arcScale.Needles.Add(arcNeedle);

                    if (!bDesign)
                    {
                        arcNeedle.ValueChanged += arcNeedle_ValueChanged;
                    }
                    else
                        arcNeedle.Animation = null;
                }

            }
            catch (Exception)
            {
            }
        }
        private void arcNeedle_ValueChanged(object sender, ValueChangedEventArgs ev)
        {
            if (bDesign)
                return;

            if (isChangingValue || isUserMouseDownAction)
                Value = ev.NewValue;
            isUserMouseDownAction = false;
        }

        private void UpdateArcLayer()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (arcLayer == null)
                {
                    arcLayer = new ArcScaleLayer();
                    CustomArcScaleLayerPresentation scalelayerpresentation = new CustomArcScaleLayerPresentation();
                    scalelayerpresentation.ScaleLayerTemplate = LoadTemplate("OscilloscopeScaleLayerTemplate1");
                    arcLayer.Presentation = scalelayerpresentation;

                    arcLayer.Options = new LayerOptions();
                    arcLayer.Options.ZIndex = 300;

                    if (!arcScale.Layers.Contains(arcLayer))
                        arcScale.Layers.Add(arcLayer);
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateArcScale()
        {
            if (circularGaugeObject == null)
                return;

            try
            {
                if (arcScale == null)
                {
                    arcScale = new ArcScale()
                    {
                        ClipToBounds = false
                    };
                    arcScale.MajorTickmarkOptions = new MajorTickmarkOptions();
                    arcScale.MinorTickmarkOptions = new MinorTickmarkOptions();


                    CustomScaleLabelPresentation scalelabelpresentation = new CustomScaleLabelPresentation();
                    scalelabelpresentation.LabelTemplate = LoadTemplate("LabelTemplate");
                    arcScale.LabelPresentation = scalelabelpresentation;
                    arcScale.LabelOptions = new ArcScaleLabelOptions();
                    arcScale.LinePresentation = null;
                    arcScale.SpindleCapPresentation = null;

                    if (!circularGaugeObject.Scales.Contains(arcScale))
                        circularGaugeObject.Scales.Add(arcScale);
                }

                arcScale.StartAngle = 135;
                arcScale.EndAngle = 405;
                arcScale.MajorIntervalCount = MajorIntervalCount;
                arcScale.MinorIntervalCount = MinorIntervalCount;
                arcScale.StartValue = _StartValue;
                arcScale.EndValue = _EndValue;

                TickmarksPresentation arcscaletickmarkspresentation = GetTickmarkPresentation(TickmarksPresentation);
                arcScale.TickmarksPresentation = arcscaletickmarkspresentation;
                var _dtpres = arcScale.TickmarksPresentation as PredefinedTickmarksPresentation;
                _dtpres.MajorTickBrush = MajorTickBrush;
                _dtpres.MinorTickBrush = MinorTickBrush;


                arcScale.MajorTickmarkOptions.FactorLength = MajorTickmarkFactorLength;
                arcScale.MajorTickmarkOptions.ZIndex = MajorTickmarkZIndex;
                arcScale.MajorTickmarkOptions.FactorThickness = MajorTickmarkFactorThickness;
                arcScale.MajorTickmarkOptions.Offset = MajorTickmarkOffset;
                arcScale.MajorTickmarkOptions.ShowFirst = MajorTickmarkShowFirst;
                arcScale.MajorTickmarkOptions.ShowLast = MajorTickmarkShowLast;
                arcScale.MajorTickmarkOptions.ZIndex = 400;

                arcScale.MinorTickmarkOptions.FactorLength = MinorTickmarkFactorLength;
                arcScale.MinorTickmarkOptions.ZIndex = MinorTickmarkZIndex;
                arcScale.MinorTickmarkOptions.FactorThickness = MinorTickmarkFactorThickness;
                arcScale.MinorTickmarkOptions.Offset = MinorTickmarkOffset;
                arcScale.MinorTickmarkOptions.ShowTicksForMajor = MinorTickmarkShowTicksForMajor;
                arcScale.MinorTickmarkOptions.ZIndex = 400;

                arcScale.LabelOptions.Orientation = LabelOrientation;
                arcScale.LabelOptions.ShowFirst = ShowFirstLabel;
                arcScale.LabelOptions.ShowLast = ShowLastLabel;
                arcScale.LabelOptions.Offset = LabelOffset;
                arcScale.LabelOptions.FormatString = LabelStringFormat;
                arcScale.LabelOptions.ZIndex = 400;
            }
            catch (Exception)
            {
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
        private ControlTemplate LoadTemplate(string template)
        {
            try
            {
                string basename = "Templates.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(KnobPotenziometer).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                ControlTemplate content = (ControlTemplate)canvas.TryFindResource(template);
                return (ControlTemplate)content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void UpdateGuageControl()
        {
            try
            {
                if (circularGaugeObject == null)
                {
                    circularGaugeObject = new CircularGaugeControl()
                    {
                        Width = 100,
                        Height = 100,
                        EnableAnimation = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontSize = 7,
                        FontStyle = FontStyles.Normal,
                        FontWeight = FontWeights.SemiBold,
                        FontFamily = new FontFamily("Segoe UI"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        ClipToBounds = false,
                        Background = transparent
                    };

                    circularGaugeObject.Model = new CircularEcoModel();

                    Grid.SetZIndex(circularGaugeObject, 1);

                    if (!container.Children.Contains(circularGaugeObject))
                        container.Children.Add(circularGaugeObject);


                    circularGaugeObject.PreviewMouseDown += circularGaugeObject_PreviewMouseDown;
                    circularGaugeObject.PreviewTouchDown += circularGaugeObject_PreviewTouchDown;

                    circularGaugeObject.PreviewMouseUp += circularGaugeObject_PreviewMouseUp;
                    circularGaugeObject.PreviewTouchUp += circularGaugeObject_PreviewTouchUp;
                }

                circularGaugeObject.Foreground = Foreground;
            }
            catch (Exception)
            {
            }
        }

        private void circularGaugeObject_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isChangingValue = false;
        }

        private void circularGaugeObject_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            isChangingValue = false;
        }

        private void circularGaugeObject_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            CircularGaugeControl meter = sender as CircularGaugeControl;
            if (meter != null)
            {
                CircularGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetTouchPoint(meter).Position);
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) ||
                (hitInfo.InNeedle || hitInfo.InScale || hitInfo.InRange))
                {
                    isChangingValue = true;
                    isUserMouseDownAction = true;
                }
            }
        }


        private void circularGaugeObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CircularGaugeControl meter = sender as CircularGaugeControl;
            if (meter != null)
            {
                CircularGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetPosition(meter));
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) ||
                (hitInfo.InNeedle || hitInfo.InScale || hitInfo.InRange))
                {
                    isChangingValue = true;
                    isUserMouseDownAction = true;
                }
            }
        }

        private void UpdateValue()
        {
            if (IsInEditMode)
                return;

            double _value = Value;
            if (bDesign)
                _value = (EndValue - StartValue) / 2;

            if (arcNeedle != null && !isChangingValue && !isUserMouseDownAction)
                arcNeedle.Value = _value;
            UpdateCustomValue(_value);
        }
        void UpdateCustomValue(double value)
        {
            if (gridValueContainer != null)
            {
                TextBox text = gridValueContainer.GetChildrenOfType<TextBox>().FirstOrDefault();
                if (text != null)
                    text.Text = ConvertValue(value);
            }
        }
        private string ConvertValue(double value)
        {
            try
            {
                return (value as IFormattable).ToString(ValueStringFormat, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return (0.0).ToString(CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }

        public void KnobPotenziometerInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region minmaxtags
        private void InitControl()
        {
            StartValue = MinValue;
            EndValue = MaxValue;
            InitTag(ref mintag, TagMinValue, mintag_PropertyChanged, TagMinValueProperty.Name);
            InitTag(ref maxtag, TagMaxValue, maxtag_PropertyChanged, TagMaxValueProperty.Name);
        }

        private void InitTag(ref OPCUAEntityReference preparedtag, OPCUAXMLEntityReference xmltag, PropertyChangedEventHandler propertyChangedEventHandler, string property)
        {
            if (preparedtag == null && xmltag != null && xmltag.TagReference != null)
                preparedtag = xmltag.TagReference;
            if (preparedtag != null && !preparedtag.IsRelative && !matchChangedMap.Contains(property))
                typeHelper.PrepareExecution(Properties.Resources.SessionName, Document, this, propertyChangedEventHandler, preparedtag);
        }

        private OPCUAEntityReference mintag;
        private OPCUAEntityReference maxtag;

        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        private void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        (container as UIElement).Effect = previousEffect;
                        (container as UIElement).ClipToBounds = previousClipToBounds;
                        //(this as UIElement).Opacity = 1;
                        previousEffect = null;
                        errorEffectOn = false;
                    }
                }
                else
                {
                    if (!errorEffectOn)
                    {
                        errorEffectOn = true;
                        previousEffect = (container as UIElement).Effect;
                        previousClipToBounds = (container as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (container as UIElement).Effect = effect;
                        (container as UIElement).ClipToBounds = false;
                    }
                }
            });
        }

        MonitoredItemViewModel mintagMonitoredItemViewModel;
        MonitoredItemViewModel maxtagMonitoredItemViewModel;
        private void maxtag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (maxtagMonitoredItemViewModel != null)
                    maxtagMonitoredItemViewModel.PropertyChanged -= maxtagMonitoredItemViewModel_PropertyChanged;

                if (bDispose)
                    return;

                maxtagMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (maxtagMonitoredItemViewModel != null)
                {
                    maxtagMonitoredItemViewModel.PropertyChanged += maxtagMonitoredItemViewModel_PropertyChanged;

                    if (maxtagMonitoredItemViewModel.NodeIdModel != null &&
                        maxtagMonitoredItemViewModel.NodeIdModel.IsVariable)
                        maxtagMonitoredItemViewModel_PropertyChanged(maxtagMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }
        private void mintag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (mintagMonitoredItemViewModel != null)
                    mintagMonitoredItemViewModel.PropertyChanged -= mintagMonitoredItemViewModel_PropertyChanged;

                if (bDispose)
                    return;

                mintagMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (mintagMonitoredItemViewModel != null)
                {
                    mintagMonitoredItemViewModel.PropertyChanged += mintagMonitoredItemViewModel_PropertyChanged;

                    if (mintagMonitoredItemViewModel.NodeIdModel != null &&
                        mintagMonitoredItemViewModel.NodeIdModel.IsVariable)
                        mintagMonitoredItemViewModel_PropertyChanged(mintagMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }
        private void maxtagMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDispose)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (m == null)
                return;

            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null)
                {
                    if (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                        m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                    {
                        try
                        {
                            if (m.DataValue.Value != null)
                            {
                                double val;
                                System.Double.TryParse(m.DataValue.Value.ToString(), out val);
                                bTagMaxValueSet = true;
                                _EndValue = val;
                                UpdateScales();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        SetEntityError(null);
                    }
                    else
                        SetEntityError(m.DataValue.StatusCode.ToString());
                }
            }
        }

        private void mintagMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDispose)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (m == null)
                return;

            if (e.PropertyName == "LastMessage")
            {
                SetEntityError(m.LastMessage);
            }
            else if (e.PropertyName == "DataValue")
            {
                if (m.DataValue != null)
                {
                    if (Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode) ||
                        m.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                    {
                        try
                        {
                            if (m.DataValue.Value != null)
                            {
                                double val;
                                System.Double.TryParse(m.DataValue.Value.ToString(), out val);
                                bTagMinValueSet = true;
                                _StartValue = val;
                                UpdateScales();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        SetEntityError(null);
                    }
                    else
                        SetEntityError(m.DataValue.StatusCode.ToString());
                }
            }
        }
        private void UpdateScaleStartEndValues()
        {
            if (arcScale != null)
            {
                arcScale.StartValue = _StartValue;
                arcScale.EndValue = _EndValue;
            }
        }
        private void UpdateScales()
        {
            if (bInit)
            {
                UpdateStartEndValues();
                UpdateScaleStartEndValues();
            }
        }

        protected void UpdateStartEndValues()
        {
            if (bDesign)
            {
                _StartValue = UseEUnit ? 0.0 : MinValue;
                _EndValue = UseEUnit ? 100.0 : MaxValue;
            }
        }


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

        #endregion

        private bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            transparent = null;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            container.Children.Clear();
            if (BackContent != null)
                BackContent.Content = null;
            BackContent = null;

            if (arcScale != null)
            {
                if (arcScale.LabelPresentation != null)
                {
                    CustomScaleLabelPresentation scalelabelpresentation = (CustomScaleLabelPresentation)arcScale.LabelPresentation;
                    ControlTemplate controltemplate = scalelabelpresentation.LabelTemplate;
                    scalelabelpresentation.LabelTemplate = null;
                }
                arcScale.LabelPresentation = null;
                arcScale.Layers.Clear();
                arcScale.Needles.Clear();
                arcScale.LabelOptions = null;
                arcScale.CustomElements.Clear();
            }

            if (arcNeedle != null)
            {
                arcNeedle.ValueChanged -= arcNeedle_ValueChanged;
                arcNeedle.Options = null;
                arcNeedle.Presentation = null;
                arcNeedle.Animation = null;
            }

            if (gridValueContainer != null)
                gridValueContainer.Content = null;

            if (circularGaugeObject != null)
            {
                circularGaugeObject.PreviewTouchDown -= circularGaugeObject_PreviewTouchDown;
                circularGaugeObject.PreviewMouseDown -= circularGaugeObject_PreviewMouseDown;
                circularGaugeObject.PreviewMouseUp -= circularGaugeObject_PreviewMouseUp;
                circularGaugeObject.PreviewTouchUp -= circularGaugeObject_PreviewTouchUp;
                circularGaugeObject.Scales.Clear();
            }

            arcScale = null;
            arcLayer = null;
            arcNeedle = null;
            gridValueContainer = null;
            circularGaugeObject = null;

            if (!bDesign)
            {
                typeHelper.TerminateExecution(this, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, mintag, mintagMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, maxtag, maxtagMonitoredItemViewModel);
            }

            DetachOverrideBaseProperties();

            typeHelper.Dispose();
            typeHelper = null;
        }
        #region IDynamicTagAware
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (TagMinValue != null && TagMinValue.TagReference != null /*&& TagMinValue.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(TagMinValueProperty.Name, ret.Keys.ToList()), TagMinValue.TagReferenceXml);

            if (TagMaxValue != null && TagMaxValue.TagReference != null /*&& TagMaxValue.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(TagMaxValueProperty.Name, ret.Keys.ToList()), TagMaxValue.TagReferenceXml);
            return ret;
        }
        String CreateUniqueName(String name, List<String> list)
        {
            if (!list.Contains(name))
                return name;
            var newname = name;
            int i = 0;
            while (list.Contains(newname))
                newname = String.Format("{0} {1}", name, ++i);

            return newname;
        }
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;

            if (TagMinValue != null && relative == TagMinValue.TagReferenceXml)
            {
                if(typeHelper.ChecktypeDefinition(Properties.Resources.SessionName, Document, this, bDesign, TagMinValue, relative, absolute, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, ref mintag, mintagMonitoredItemViewModel))
                    matchChangedMap.Add(TagMinValueProperty.Name);
            }
            else if (TagMaxValue != null && relative == TagMaxValue.TagReferenceXml)
            {
                if(typeHelper.ChecktypeDefinition(Properties.Resources.SessionName, Document, this, bDesign, TagMaxValue, relative, absolute, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, ref maxtag, maxtagMonitoredItemViewModel))
                    matchChangedMap.Add(TagMaxValueProperty.Name);
            }

            return (TagMinValue == null || (TagMinValue != null && matchChangedMap.Contains(TagMinValueProperty.Name))) &&
                   (TagMaxValue == null || (TagMaxValue != null && matchChangedMap.Contains(TagMaxValueProperty.Name)));
        }
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            if (TagMinValue != null && TagMinValue.TagReference != null /*&& TagMinValue.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(TagMinValueProperty.Name))
                    newValue.TagReferenceXml = map[TagMinValueProperty.Name];
                else
                    newValue.TagReferenceXml = typeHelper.UpdateTag(TagMinValue.TagReferenceXml, map);
                TagMinValue = newValue;
            }

            if (TagMaxValue != null && TagMaxValue.TagReference != null /*&& TagMaxValue.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(TagMaxValueProperty.Name))
                    newValue.TagReferenceXml = map[TagMaxValueProperty.Name];
                else
                    newValue.TagReferenceXml = typeHelper.UpdateTag(TagMaxValue.TagReferenceXml, map);
                TagMaxValue = newValue;
            }
        }
        public void PreserveTagsFromMap(Dictionary<string, string> map)
        {
            var tobeupdated = typeHelper.PreserveTagsFromMap(GetMapDynamics(), map);
            UpdateMapDynamics(tobeupdated);
        }
        public void SetConverterLabel(string label)
        {
            ConverterLabel = label;
            if (bInit)
                UpdateCustomElements();
        }
        #endregion

    }
}
