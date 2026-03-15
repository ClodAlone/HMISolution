using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Gauges;
using Utilities;
using Utilities.WPF;
using Converters;
using Meters.Enums;
using System.Windows.Shapes;
using OPCUAViewModel;
using Opc.Ua;
using System.Globalization;
using UFInterfaces;
using ViewModelLib;
using System.Windows.Media.Effects;
using ScreenSettings;
using System.Windows.Data;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using System.Windows.Input;
using System.IO;
using DynamicTagAwareHelper;
using Meters.Automations;
using System.Windows.Automation.Peers;
using System.Windows.Media.Imaging;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using System.Windows.Threading;
using System.Threading.Tasks;
using StringManager.ComponentService;
using System.Threading;
using System.Xml.Serialization;

namespace Meters
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [Obsolete("Use the Meters.LinearMeterControl.xaml insted of this.")]
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class LinearMeter : UserControl, IDisposable, IEntityReference, IDynamicTagAware, IContainPropertyEditors, IStringIDAware
    {
        #region Dependency Properties

        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(LinearMeter));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(LinearMeter));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(LinearMeter));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(LinearMeter));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(LinearMeter));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(LinearMeter));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(LinearMeter));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(LinearMeter));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(LinearMeter));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(LinearMeter));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as LinearMeter;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && DataContext != null)
            {
                var label = LabelFontSettings.Clone();
                var value = ValueFontSettings.Clone();
                var engeneering = EngeneeringUnitFontSettings.Clone();

                label.FontFamily = FontFamily;
                value.FontFamily = FontFamily;
                engeneering.FontFamily = FontFamily;

                LabelFontSettings = label;
                ValueFontSettings = value;
                EngeneeringUnitFontSettings = engeneering;
            }

        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as LinearMeter;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && DataContext != null)
            {
                var label = LabelFontSettings.Clone();
                var value = ValueFontSettings.Clone();
                var engeneering = EngeneeringUnitFontSettings.Clone();

                label.FontWeight = FontWeight;
                value.FontWeight = FontWeight;
                engeneering.FontWeight = FontWeight;

                LabelFontSettings = label;
                ValueFontSettings = value;
                EngeneeringUnitFontSettings = engeneering;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as LinearMeter;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && DataContext != null)
            {
                var label = LabelFontSettings.Clone();
                var value = ValueFontSettings.Clone();
                var engeneering = EngeneeringUnitFontSettings.Clone();

                label.FontStyle = FontStyle;
                value.FontStyle = FontStyle;
                engeneering.FontStyle = FontStyle;

                LabelFontSettings = label;
                ValueFontSettings = value;
                EngeneeringUnitFontSettings = engeneering;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as LinearMeter;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && DataContext != null)
            {
                var label = LabelFontSettings.Clone();
                var value = ValueFontSettings.Clone();
                var engeneering = EngeneeringUnitFontSettings.Clone();

                label.FontSize = (int)FontSize;
                value.FontSize = (int)FontSize;
                engeneering.FontSize = (int)FontSize;

                LabelFontSettings = label;
                ValueFontSettings = value;
                EngeneeringUnitFontSettings = engeneering;
            }
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as LinearMeter;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesign && DataContext != null)
            {
                LabelForeground = Foreground;
                ValueForeground = Foreground;
                EngeneeringUnitForeground = Foreground;
            }
        }
        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(LinearMeter), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(LinearMeter), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
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
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));

        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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

        #region LevelOptions
        public static readonly DependencyProperty LevelOptionsProperty = DependencyProperty.Register("LevelOptions", typeof(LevelOptions), typeof(LinearMeter), new UIPropertyMetadata(null));
        [SvgValueConverter(typeof(ConvertLevelOptions))]
        [Browsable(false)]
        [XmlIgnore]
        public LevelOptions LevelOptions
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (LevelOptions)GetValue(LevelOptionsProperty);
            }
            set
            {
                SetValue(LevelOptionsProperty, value);
            }
        }
        internal LevelOptions GetLevelOptions()
        {
            int offset = LevelOffset;
            int thickness = LevelThickness;

            if (RangeBarVisible)
            {
                offset = RangeBarOffset;
                thickness = RangeBarThickness;
            }

            return new LevelOptions()
            {
                Offset = offset,
                Thickness = thickness
            };
        }
        #endregion

        #region LevelBackgroundTemplate
        public static readonly DependencyProperty LevelBackgroundTemplateProperty = DependencyProperty.Register("LevelBackgroundTemplate", typeof(string), typeof(LinearMeter), new UIPropertyMetadata("Bar"));
        [SvgValueConverter(typeof(ConvertLevelBackgroundTemplate), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string LevelBackgroundTemplate
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(LevelBackgroundTemplateProperty);
            }
            set
            {
                SetValue(LevelBackgroundTemplateProperty, value);
            }
        }
        internal string LoadSVGBackLevelContent()
        {
            string template = GetTemplateName();
            switch (LayoutMode)
            {
                case LinearScaleLayoutMode.LeftToRight:
                case LinearScaleLayoutMode.RightToLeft:
                    return string.Format("{0}_H", template);
                case LinearScaleLayoutMode.BottomToTop:
                case LinearScaleLayoutMode.TopToBottom:
                    return string.Format("{0}_V", template);
                default:
                    return string.Format("{0}_H", template);
            }
        }
        internal string GetTemplateName()
        {
            return RangeBarVisible ? "Bar" : LinearBaseModel == PredefinedBaseElementKinds.Linear ||
                LinearBaseModel == PredefinedBaseElementKinds.Termometer ||
                LinearBaseModel == PredefinedBaseElementKinds.Termometer1 ? "BarRoundedNoBorder" : "BarRounded";
        }
        #endregion


        #region TagMinValue
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(LinearMeter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMinValueChanged), new CoerceValueCallback(OnCoerceTagMinValue)));

        private static object OnCoerceTagMinValue(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceTagMinValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(LinearMeter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMaxValueChanged), new CoerceValueCallback(OnCoerceTagMaxValue)));

        private static object OnCoerceTagMaxValue(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceTagMaxValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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


        #region ValueStyle

        #region ShowValue
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueChanged), new CoerceValueCallback(OnCoerceShowValue)));

        private static object OnCoerceShowValue(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowValue((bool)value);
            else
                return value;
        }

        private static void OnShowValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnShowValueChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowValue(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowValueChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }


        [Category("ValueStyle")]
        public bool ShowValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowValueProperty);
            }
            set
            {
                SetValue(ShowValueProperty, value);
            }
        }

        #endregion
        #region ValueForeground
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnValueForegroundChanged), new CoerceValueCallback(OnCoerceValueForeground)));

        private static object OnCoerceValueForeground(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueForeground((Brush)value);
            else
                return value;
        }

        private static void OnValueForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnValueForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceValueForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("ValueStyle")]
        public Brush ValueForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ValueForegroundProperty);
            }
            set
            {
                SetValue(ValueForegroundProperty, value);
            }
        }

        #endregion
        #region ValueFontSettings
        public static readonly DependencyProperty ValueFontSettingsProperty = DependencyProperty.Register("ValueFontSettings", typeof(FontSettings), typeof(LinearMeter), new UIPropertyMetadata(new FontSettings(FontWeights.DemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 48), new PropertyChangedCallback(OnValueFontSettingsChanged), new CoerceValueCallback(OnCoerceValueFontSettings)));

        private static object OnCoerceValueFontSettings(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnValueFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnValueFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceValueFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("ValueStyle")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings ValueFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(ValueFontSettingsProperty);
            }
            set
            {
                SetValue(ValueFontSettingsProperty, value);
            }
        }

        #endregion
        #region ValueOffset
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(LinearMeter), new UIPropertyMetadata(new Thickness(90, 0, 0, 0), new PropertyChangedCallback(OnValueOffsetChanged), new CoerceValueCallback(OnCoerceValueOffset)));

        private static object OnCoerceValueOffset(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueOffset((Thickness)value);
            else
                return value;
        }

        private static void OnValueOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnValueOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
        }

        protected virtual Thickness OnCoerceValueOffset(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueOffsetChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("ValueStyle")]
        public Thickness ValueOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(ValueOffsetProperty);
            }
            set
            {
                SetValue(ValueOffsetProperty, value);
            }
        }

        #endregion


        #region ValueStringFormat 
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(LinearMeter), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
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


        #endregion

        #region GaugeStyle
        #region EnableBackGroundLayer
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnEnableBackGroundLayerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceEnableBackGroundLayer(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableBackGroundLayerChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit) && !newValue)
                LinearBaseModel = PredefinedBaseElementKinds.None;
        }
        [Category("GaugeStyle")]
        [Browsable(false)]
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
        #region LinearBaseModel
        public static readonly DependencyProperty LinearBaseModelProperty = DependencyProperty.Register("LinearBaseModel", typeof(PredefinedBaseElementKinds), typeof(LinearMeter), new UIPropertyMetadata(PredefinedBaseElementKinds.Progressive, new PropertyChangedCallback(OnLinearBaseModelChanged), new CoerceValueCallback(OnCoerceLinearBaseModel)));

        private static object OnCoerceLinearBaseModel(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceLinearBaseModel((PredefinedBaseElementKinds)value);
            else
                return value;
        }

        private static void OnLinearBaseModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnLinearBaseModelChanged((PredefinedBaseElementKinds)e.OldValue, (PredefinedBaseElementKinds)e.NewValue);
        }

        protected virtual PredefinedBaseElementKinds OnCoerceLinearBaseModel(PredefinedBaseElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLinearBaseModelChanged(PredefinedBaseElementKinds oldValue, PredefinedBaseElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertMeterBaseModel), RequiredKey = true)]
        public PredefinedBaseElementKinds LinearBaseModel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedBaseElementKinds)GetValue(LinearBaseModelProperty);
            }
            set
            {
                SetValue(LinearBaseModelProperty, value);
            }
        }
        #endregion
        #region Background
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnBackgroundChanged), new CoerceValueCallback(OnCoerceBackground)));

        private static object OnCoerceBackground(DependencyObject o, object value)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                return linearMeter.OnCoerceBackground((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                linearMeter.OnBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
        public Brush Background
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }
        #endregion



        #region LayoutMode
        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(LinearScaleLayoutMode), typeof(LinearMeter), new UIPropertyMetadata(LinearScaleLayoutMode.BottomToTop, new PropertyChangedCallback(OnLayoutModeChanged), new CoerceValueCallback(OnCoerceLayoutMode)));

        private static object OnCoerceLayoutMode(DependencyObject o, object value)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                return linearMeter.OnCoerceLayoutMode((LinearScaleLayoutMode)value);
            else
                return value;
        }

        private static void OnLayoutModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                linearMeter.OnLayoutModeChanged((LinearScaleLayoutMode)e.OldValue, (LinearScaleLayoutMode)e.NewValue);
        }

        protected virtual LinearScaleLayoutMode OnCoerceLayoutMode(LinearScaleLayoutMode value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLayoutModeChanged(LinearScaleLayoutMode oldValue, LinearScaleLayoutMode newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
         [Category("GaugeStyle")]
        public LinearScaleLayoutMode LayoutMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (LinearScaleLayoutMode)GetValue(LayoutModeProperty);
            }
            set
            {
                SetValue(LayoutModeProperty, value);
            }
        }
        
        #endregion
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(LinearScaleLabelOrientation), typeof(LinearMeter), new UIPropertyMetadata(LinearScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLableOrientation((LinearScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLableOrientationChanged((LinearScaleLabelOrientation)e.OldValue, (LinearScaleLabelOrientation)e.NewValue);
        }

        protected virtual LinearScaleLabelOrientation OnCoerceLableOrientation(LinearScaleLabelOrientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLableOrientationChanged(LinearScaleLabelOrientation oldValue, LinearScaleLabelOrientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public LinearScaleLabelOrientation LabelOrientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (LinearScaleLabelOrientation)GetValue(LabelOrientationProperty);
            }
            set
            {
                SetValue(LabelOrientationProperty, value);
            }
        }

        #endregion
        #region LabelOffset
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-30, new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLabelOffset((int)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLabelOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLabelOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public int LabelOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(LabelOffsetProperty);
            }
            set
            {
                SetValue(LabelOffsetProperty, value);
            }
        }

        #endregion
        #region LabelForeground
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLabelForegroundChanged), new CoerceValueCallback(OnCoerceLabelForeground)));

        private static object OnCoerceLabelForeground(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLabelForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLabelForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty LabelZIndexProperty = DependencyProperty.Register("LabelZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnLabelZIndexChanged), new CoerceValueCallback(OnCoerceLabelZIndex)));

        private static object OnCoerceLabelZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLabelZIndex((int)value);
            else
                return value;
        }

        private static void OnLabelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLabelZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLabelZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty LabelFontSettingsProperty = DependencyProperty.Register("LabelFontSettings", typeof(FontSettings), typeof(LinearMeter), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnLabelFontSettingsChanged), new CoerceValueCallback(OnCoerceLabelFontSettings)));

        private static object OnCoerceLabelFontSettings(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLabelFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnLabelFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLabelFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
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
        [Category("GaugeStyle")]
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
        #region LabelStringFormat
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(LinearMeter), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceLabelStringFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelStringFormatChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowFirstLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFirstLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowLastLabel(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLastLabelChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
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


        #region ShowEngeneeringUnit
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceShowEngeneeringUnit)));

        private static object OnCoerceShowEngeneeringUnit(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowEngeneeringUnit((bool)value);
            else
                return value;
        }

        private static void OnShowEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnShowEngeneeringUnitChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowEngeneeringUnit(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowEngeneeringUnitChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("EngeneeringUnitStyle")]
        public bool ShowEngeneeringUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowEngeneeringUnitProperty);
            }
            set
            {
                SetValue(ShowEngeneeringUnitProperty, value);
            }
        }
        #endregion
        #region EngeneeringUnitForeground
        public static readonly DependencyProperty EngeneeringUnitForegroundProperty = DependencyProperty.Register("EngeneeringUnitForeground", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnEngeneeringUnitForegroundChanged), new CoerceValueCallback(OnCoerceEngeneeringUnitForeground)));

        private static object OnCoerceEngeneeringUnitForeground(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnitForeground((Brush)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnEngeneeringUnitForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceEngeneeringUnitForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEngeneeringUnitForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("EngeneeringUnitStyle")]
        public Brush EngeneeringUnitForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(EngeneeringUnitForegroundProperty);
            }
            set
            {
                SetValue(EngeneeringUnitForegroundProperty, value);
            }
        }

        #endregion
        #region EngeneeringOffset
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(LinearMeter), new UIPropertyMetadata(new Thickness(85, 110, 0, 0), new PropertyChangedCallback(OnEngeneeringOffsetChanged), new CoerceValueCallback(OnCoerceEngeneeringOffset)));

        private static object OnCoerceEngeneeringOffset(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringOffset((Thickness)value);
            else
                return value;
        }

        private static void OnEngeneeringOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnEngeneeringOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
        }

        protected virtual Thickness OnCoerceEngeneeringOffset(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEngeneeringOffsetChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("EngeneeringUnitStyle")]
        public Thickness EngeneeringOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(EngeneeringOffsetProperty);
            }
            set
            {
                SetValue(EngeneeringOffsetProperty, value);
            }
        }

        #endregion
        #region EngeneeringUnit
        public static readonly DependencyProperty EngeneeringUnitProperty = DependencyProperty.Register("EngeneeringUnit", typeof(String), typeof(LinearMeter), new UIPropertyMetadata("°C", new PropertyChangedCallback(OnEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceEngeneeringUnit)));

        private static object OnCoerceEngeneeringUnit(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnit((String)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnEngeneeringUnitChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceEngeneeringUnit(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEngeneeringUnitChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("EngeneeringUnitStyle")]
        public String EngeneeringUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(EngeneeringUnitProperty);
            }
            set
            {
                SetValue(EngeneeringUnitProperty, value);
            }
        }

        #endregion
        #region EngeneeringUnitFontSettings
        public static readonly DependencyProperty EngeneeringUnitFontSettingsProperty = DependencyProperty.Register("EngeneeringUnitFontSettings", typeof(FontSettings), typeof(LinearMeter), new UIPropertyMetadata(new FontSettings(FontWeights.SemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 36), new PropertyChangedCallback(OnEngeneeringUnitFontSettingsChanged), new CoerceValueCallback(OnCoerceEngeneeringUnitFontSettings)));

        private static object OnCoerceEngeneeringUnitFontSettings(DependencyObject o, object value)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnitFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter circularGauge = o as LinearMeter;
            if (circularGauge != null)
                circularGauge.OnEngeneeringUnitFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceEngeneeringUnitFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEngeneeringUnitFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("EngeneeringUnitStyle")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings EngeneeringUnitFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(EngeneeringUnitFontSettingsProperty);
            }
            set
            {
                SetValue(EngeneeringUnitFontSettingsProperty, value);
            }
        }

        #endregion


        #region FlowDirection
        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(LinearMeter), new UIPropertyMetadata(FlowDirection.LeftToRight, new PropertyChangedCallback(OnFlowDirectionChanged), new CoerceValueCallback(OnCoerceFlowDirection)));

        private static object OnCoerceFlowDirection(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceFlowDirection((FlowDirection)value);
            else
                return value;
        }

        private static void OnFlowDirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnFlowDirectionChanged((FlowDirection)e.OldValue, (FlowDirection)e.NewValue);
        }

        protected virtual FlowDirection OnCoerceFlowDirection(FlowDirection value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFlowDirectionChanged(FlowDirection oldValue, FlowDirection newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public FlowDirection FlowDirection
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

        #region LinePresentation
        public static readonly DependencyProperty LinePresentationProperty = DependencyProperty.Register("LinePresentation", typeof(PredefinedElementKinds), typeof(LinearMeter), new UIPropertyMetadata(PredefinedElementKinds.CleanWhite, new PropertyChangedCallback(OnLinePresentationChanged), new CoerceValueCallback(OnCoerceLinePresentation)));

        private static object OnCoerceLinePresentation(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLinePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnLinePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLinePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceLinePresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLinePresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
        [Browsable(false)]
        public PredefinedElementKinds LinePresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(LinePresentationProperty);
            }
            set
            {
                SetValue(LinePresentationProperty, value);
            }
        }

        #endregion
        #region ScaleLineTickmarkZIndex
        public static readonly DependencyProperty ScaleLineTickmarkZIndexProperty = DependencyProperty.Register("ScaleLineTickmarkZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)0, new PropertyChangedCallback(OnScaleLineTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkZIndex)));

        private static object OnCoerceScaleLineTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceScaleLineTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnScaleLineTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceScaleLineTickmarkZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnScaleLineTickmarkZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public int ScaleLineTickmarkZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ScaleLineTickmarkZIndexProperty);
            }
            set
            {
                SetValue(ScaleLineTickmarkZIndexProperty, value);
            }
        }
        #endregion
        #region ScaleLineTickmarkOffset
        public static readonly DependencyProperty ScaleLineTickmarkOffsetProperty = DependencyProperty.Register("ScaleLineTickmarkOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-27, new PropertyChangedCallback(OnScaleLineTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkOffset)));

        private static object OnCoerceScaleLineTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceScaleLineTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnScaleLineTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceScaleLineTickmarkOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnScaleLineTickmarkOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public int ScaleLineTickmarkOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ScaleLineTickmarkOffsetProperty);
            }
            set
            {
                SetValue(ScaleLineTickmarkOffsetProperty, value);
            }
        }
        #endregion
        #region ScaleLineTickmarkFactorThickness
        public static readonly DependencyProperty ScaleLineTickmarkFactorThicknessProperty = DependencyProperty.Register("ScaleLineTickmarkFactorThickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnScaleLineTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkFactorThickness)));

        private static object OnCoerceScaleLineTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceScaleLineTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnScaleLineTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceScaleLineTickmarkFactorThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnScaleLineTickmarkFactorThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public int ScaleLineTickmarkFactorThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ScaleLineTickmarkFactorThicknessProperty);
            }
            set
            {
                SetValue(ScaleLineTickmarkFactorThicknessProperty, value);
            }
        }

        #endregion
        #endregion
        #region GaugeTickmarkStyle


        #region TickmarkFill
        public static readonly DependencyProperty TickmarkFillProperty = DependencyProperty.Register("TickmarkFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnTickmarkFillChanged), new CoerceValueCallback(OnCoerceTickmarkFill)));

        private static object OnCoerceTickmarkFill(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                control.OnTickmarkFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTickmarkFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickmarkFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        private void SetTickmarkFill(Brush newValue)
        {
            (linearScale.TickmarksPresentation as PredefinedTickmarksPresentation).MinorTickBrush = (linearScale.TickmarksPresentation as PredefinedTickmarksPresentation).MajorTickBrush = newValue;
        }
        [Category("GaugeTicmarkStyle")]
        [Browsable(false)]
        public Brush TickmarkFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(TickmarkFillProperty);
            }
            set
            {
                SetValue(TickmarkFillProperty, value);
            }
        }

        #endregion


        #region TickmarksPresentation
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(LinearMeter), new UIPropertyMetadata(PredefinedElementKinds.Eco, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                return linearMeter.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                linearMeter.OnTickmarksPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceTickmarksPresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickmarksPresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkFactorLength((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkFactorLengthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkFactorLength(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFactorLengthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
        public int MajorTickmarkFactorLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MajorTickmarkFactorLengthProperty);
            }
            set
            {
                SetValue(MajorTickmarkFactorLengthProperty, value);
            }
        }

        #endregion
        #region MajorTickmarkZIndex
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkFactorThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFactorThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        [SvgValueConverter(typeof(ConvertMajorTickmarkOffset), RequiredKey = true)]
        #region MajorTickmarkOffset
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-5, new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorTickmarkOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkShowFirstChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMajorTickmarkShowFirst(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkShowFirstChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorTickmarkShowLastChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMajorTickmarkShowLast(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkShowLastChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        #region MajorTickmarkFill
        public static readonly DependencyProperty MajorTickmarkFillProperty = DependencyProperty.Register("MajorTickmarkFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMajorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFill)));

        private static object OnCoerceMajorTickmarkFill(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceMajorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                control.OnMajorTickmarkFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMajorTickmarkFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorTickmarkFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeTicmarkStyle")]
        public Brush MajorTickmarkFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MajorTickmarkFillProperty);
            }
            set
            {
                SetValue(MajorTickmarkFillProperty, value);
            }
        }

        #endregion



        #region MinorTickmarkFactorLength
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorTickmarkFactorLength((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorTickmarkFactorLengthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkFactorLength(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorLengthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-5, new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorTickmarkFactorThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFactorThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorTickmarkShowTicksForMajorChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMinorTickmarkShowTicksForMajor(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkShowTicksForMajorChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeTicmarkStyle")]
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
        #region MinorTickmarkFill
        public static readonly DependencyProperty MinorTickmarkFillProperty = DependencyProperty.Register("MinorTickmarkFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMinorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFill)));

        private static object OnCoerceMinorTickmarkFill(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceMinorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                control.OnMinorTickmarkFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMinorTickmarkFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorTickmarkFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeTicmarkStyle")]
        public Brush MinorTickmarkFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MinorTickmarkFillProperty);
            }
            set
            {
                SetValue(MinorTickmarkFillProperty, value);
            }
        }

        #endregion
        #endregion
        #region GaugeMarkerStyle

        #region MarkerVisible
        public static readonly DependencyProperty MarkerVisibleProperty = DependencyProperty.Register("MarkerVisible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerVisibleChanged), new CoerceValueCallback(OnCoerceMarkerVisible)));

        private static object OnCoerceMarkerVisible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerVisible((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public Boolean MarkerVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MarkerVisibleProperty);
            }
            set
            {
                SetValue(MarkerVisibleProperty, value);
            }
        }

        #endregion
        #region MarkerFactorHeight
        public static readonly DependencyProperty MarkerFactorHeightProperty = DependencyProperty.Register("MarkerFactorHeight", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnMarkerFactorHeightChanged), new CoerceValueCallback(OnCoerceMarkerFactorHeight)));

        private static object OnCoerceMarkerFactorHeight(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerFactorHeight((int)value);
            else
                return value;
        }

        private static void OnMarkerFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerFactorHeightChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerFactorHeight(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorHeightChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public int MarkerFactorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MarkerFactorHeightProperty);
            }
            set
            {
                SetValue(MarkerFactorHeightProperty, value);
            }
        }
        #endregion
        #region MarkerFactorWidth
        public static readonly DependencyProperty MarkerFactorWidthProperty = DependencyProperty.Register("MarkerFactorWidth", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnMarkerFactorWidthChanged), new CoerceValueCallback(OnCoerceMarkerFactorWidth)));

        private static object OnCoerceMarkerFactorWidth(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerFactorWidth((int)value);
            else
                return value;
        }

        private static void OnMarkerFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerFactorWidthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerFactorWidth(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorWidthChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public int MarkerFactorWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MarkerFactorWidthProperty);
            }
            set
            {
                SetValue(MarkerFactorWidthProperty, value);
            }
        }
        #endregion
        #region MarkerZIndex
        public static readonly DependencyProperty MarkerZIndexProperty = DependencyProperty.Register("MarkerZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)100, new PropertyChangedCallback(OnMarkerZIndexChanged), new CoerceValueCallback(OnCoerceMarkerZIndex)));

        private static object OnCoerceMarkerZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerZIndex((int)value);
            else
                return value;
        }

        private static void OnMarkerZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public int MarkerZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MarkerZIndexProperty);
            }
            set
            {
                SetValue(MarkerZIndexProperty, value);
            }
        }
        #endregion
        #region MarkerOffset
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)30, new PropertyChangedCallback(OnMarkerOffsetChanged), new CoerceValueCallback(OnCoerceMarkerOffset)));

        private static object OnCoerceMarkerOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerOffset((int)value);
            else
                return value;
        }

        private static void OnMarkerOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public int MarkerOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MarkerOffsetProperty);
            }
            set
            {
                SetValue(MarkerOffsetProperty, value);
            }
        }
        #endregion
        #region MarkerFill
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnMarkerFillChanged), new CoerceValueCallback(OnCoerceMarkerFill)));

        private static object OnCoerceMarkerFill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerFill((Brush)value);
            else
                return value;
        }

        private static void OnMarkerFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMarkerFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeMarkerStyle")]
        public Brush MarkerFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MarkerFillProperty);
            }
            set
            {
                SetValue(MarkerFillProperty, value);
            }
        }

        #endregion
        #region MarkerStroke
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkRed), new PropertyChangedCallback(OnMarkerStrokeChanged), new CoerceValueCallback(OnCoerceMarkerStroke)));

        private static object OnCoerceMarkerStroke(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceMarkerStroke((Brush)value);
            else
                return value;
        }

        private static void OnMarkerStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                control.OnMarkerStrokeChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMarkerStroke(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerStrokeChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("GaugeMarkerStyle")]
        public Brush MarkerStroke
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MarkerStrokeProperty);
            }
            set
            {
                SetValue(MarkerStrokeProperty, value);
            }
        }

        #endregion
        #region MarkerAnimationEnable
        public static readonly DependencyProperty MarkerAnimationEnableProperty = DependencyProperty.Register("MarkerAnimationEnable", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerAnimationEnableChanged), new CoerceValueCallback(OnCoerceMarkerAnimationEnable)));

        private static object OnCoerceMarkerAnimationEnable(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public Boolean MarkerAnimationEnable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MarkerAnimationEnableProperty);
            }
            set
            {
                SetValue(MarkerAnimationEnableProperty, value);
            }
        }

        #endregion
        #region MarkerIsInteractive
        public static readonly DependencyProperty MarkerIsInteractiveProperty = DependencyProperty.Register("MarkerIsInteractive", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnMarkerIsInteractiveChanged), new CoerceValueCallback(OnCoerceMarkerIsInteractive)));

        private static object OnCoerceMarkerIsInteractive(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMarkerIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMarkerIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public Boolean MarkerIsInteractive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(MarkerIsInteractiveProperty);
            }
            set
            {
                SetValue(MarkerIsInteractiveProperty, value);
            }
        }

        #endregion
        #region MarkerOrientation
        public static readonly DependencyProperty MarkerOrientationProperty = DependencyProperty.Register("MarkerOrientation", typeof(LinearScaleMarkerOrientation), typeof(LinearMeter), new UIPropertyMetadata(LinearScaleMarkerOrientation.Normal, new PropertyChangedCallback(OnMarkerOrientationChanged), new CoerceValueCallback(OnCoerceMarkerOrientation)));

        private static object OnCoerceMarkerOrientation(DependencyObject o, object value)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                return linearMeter.OnCoerceMarkerOrientation((LinearScaleMarkerOrientation)value);
            else
                return value;
        }

        private static void OnMarkerOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter linearMeter = o as LinearMeter;
            if (linearMeter != null)
                linearMeter.OnMarkerOrientationChanged((LinearScaleMarkerOrientation)e.OldValue, (LinearScaleMarkerOrientation)e.NewValue);
        }

        protected virtual LinearScaleMarkerOrientation OnCoerceMarkerOrientation(LinearScaleMarkerOrientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerOrientationChanged(LinearScaleMarkerOrientation oldValue, LinearScaleMarkerOrientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeMarkerStyle")]
        public LinearScaleMarkerOrientation MarkerOrientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (LinearScaleMarkerOrientation)GetValue(MarkerOrientationProperty);
            }
            set
            {
                SetValue(MarkerOrientationProperty, value);
            }
        }
        
        #endregion

        #endregion
        #region GaugeRangeBarStyle

        #region RangeBarVisible
        public static readonly DependencyProperty RangeBarVisibleProperty = DependencyProperty.Register("RangeBarVisible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarVisibleChanged), new CoerceValueCallback(OnCoerceRangeBarVisible)));

        private static object OnCoerceRangeBarVisible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarVisible((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public Boolean RangeBarVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(RangeBarVisibleProperty);
            }
            set
            {
                SetValue(RangeBarVisibleProperty, value);
            }
        }

        #endregion
        #region RangeBarZIndex
        public static readonly DependencyProperty RangeBarZIndexProperty = DependencyProperty.Register("RangeBarZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)51, new PropertyChangedCallback(OnRangeBarZIndexChanged), new CoerceValueCallback(OnCoerceRangeBarZIndex)));

        private static object OnCoerceRangeBarZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarZIndex((int)value);
            else
                return value;
        }

        private static void OnRangeBarZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRangeBarZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public int RangeBarZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(RangeBarZIndexProperty);
            }
            set
            {
                SetValue(RangeBarZIndexProperty, value);
            }
        }
        #endregion
        #region RangeBarOffset
        public static readonly DependencyProperty RangeBarOffsetProperty = DependencyProperty.Register("RangeBarOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnRangeBarOffsetChanged), new CoerceValueCallback(OnCoerceRangeBarOffset)));

        private static object OnCoerceRangeBarOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarOffset((int)value);
            else
                return value;
        }

        private static void OnRangeBarOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRangeBarOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public int RangeBarOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(RangeBarOffsetProperty);
            }
            set
            {
                SetValue(RangeBarOffsetProperty, value);
            }
        }
        #endregion
        #region RangeBarFill
        public static readonly DependencyProperty RangeBarFillProperty = DependencyProperty.Register("RangeBarFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRangeBarFillChanged), new CoerceValueCallback(OnCoerceRangeBarFill)));

        private static object OnCoerceRangeBarFill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarFill((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRangeBarFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeRangeBarStyle")]
        public Brush RangeBarFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RangeBarFillProperty);
            }
            set
            {
                SetValue(RangeBarFillProperty, value);
            }
        }

        #endregion
        #region RangeBarBackground
        public static readonly DependencyProperty RangeBarBackgroundProperty = DependencyProperty.Register("RangeBarBackground", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRangeBarBackgroundChanged), new CoerceValueCallback(OnCoerceRangeBarBackground)));

        private static object OnCoerceRangeBarBackground(DependencyObject o, object value)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                return control.OnCoerceRangeBarBackground((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter control = o as LinearMeter;
            if (control != null)
                control.OnRangeBarBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRangeBarBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
        public Brush RangeBarBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RangeBarBackgroundProperty);
            }
            set
            {
                SetValue(RangeBarBackgroundProperty, value);
            }
        }

        #endregion
        #region RangeBarAnimationEnable
        public static readonly DependencyProperty RangeBarAnimationEnableProperty = DependencyProperty.Register("RangeBarAnimationEnable", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarAnimationEnableChanged), new CoerceValueCallback(OnCoerceRangeBarAnimationEnable)));

        private static object OnCoerceRangeBarAnimationEnable(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public Boolean RangeBarAnimationEnable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(RangeBarAnimationEnableProperty);
            }
            set
            {
                SetValue(RangeBarAnimationEnableProperty, value);
            }
        }

        #endregion
        #region RangeBarIsInteractive
        public static readonly DependencyProperty RangeBarIsInteractiveProperty = DependencyProperty.Register("RangeBarIsInteractive", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarIsInteractiveChanged), new CoerceValueCallback(OnCoerceRangeBarIsInteractive)));

        private static object OnCoerceRangeBarIsInteractive(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public Boolean RangeBarIsInteractive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(RangeBarIsInteractiveProperty);
            }
            set
            {
                SetValue(RangeBarIsInteractiveProperty, value);
            }
        }

        #endregion
        #region RangeBarThickness
        public static readonly DependencyProperty RangeBarThicknessProperty = DependencyProperty.Register("RangeBarThickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)35, new PropertyChangedCallback(OnRangeBarThicknessChanged), new CoerceValueCallback(OnCoerceRangeBarThickness)));

        private static object OnCoerceRangeBarThickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRangeBarThickness((int)value);
            else
                return value;
        }

        private static void OnRangeBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRangeBarThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRangeBarThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeBarStyle")]
        public int RangeBarThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(RangeBarThicknessProperty);
            }
            set
            {
                SetValue(RangeBarThicknessProperty, value);
            }
        }

        #endregion
        #endregion
        #region GaugeLevelStyle

        #region LevelVisible
        public static readonly DependencyProperty LevelVisibleProperty = DependencyProperty.Register("LevelVisible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLevelVisibleChanged), new CoerceValueCallback(OnCoerceLevelVisible)));

        private static object OnCoerceLevelVisible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLevelVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLevelVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public Boolean LevelVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(LevelVisibleProperty);
            }
            set
            {
                SetValue(LevelVisibleProperty, value);
            }
        }

        #endregion
        #region LevelZIndex
        public static readonly DependencyProperty LevelZIndexProperty = DependencyProperty.Register("LevelZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)50, new PropertyChangedCallback(OnLevelZIndexChanged), new CoerceValueCallback(OnCoerceLevelZIndex)));

        private static object OnCoerceLevelZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelZIndex((int)value);
            else
                return value;
        }

        private static void OnLevelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLevelZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public int LevelZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(LevelZIndexProperty);
            }
            set
            {
                SetValue(LevelZIndexProperty, value);
            }
        }
        #endregion
        #region LevelOffset
        public static readonly DependencyProperty LevelOffsetProperty = DependencyProperty.Register("LevelOffset", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnLevelOffsetChanged), new CoerceValueCallback(OnCoerceLevelOffset)));

        private static object OnCoerceLevelOffset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelOffset((int)value);
            else
                return value;
        }

        private static void OnLevelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelOffsetChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLevelOffset(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelOffsetChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public int LevelOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(LevelOffsetProperty);
            }
            set
            {
                SetValue(LevelOffsetProperty, value);
            }
        }
        #endregion
        #region LevelFill
        public static readonly DependencyProperty LevelFillProperty = DependencyProperty.Register("LevelFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnLevelFillChanged), new CoerceValueCallback(OnCoerceLevelFill)));

        private static object OnCoerceLevelFill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelFill((Brush)value);
            else
                return value;
        }

        private static void OnLevelFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLevelFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeLevelStyle")]
        public Brush LevelFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LevelFillProperty);
            }
            set
            {
                SetValue(LevelFillProperty, value);
            }
        }

        #endregion
        #region LevelBackgroundFill
        public static readonly DependencyProperty LevelBackgroundFillProperty = DependencyProperty.Register("LevelBackgroundFill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLevelBackgroundFillChanged), new CoerceValueCallback(OnCoerceLevelBackgroundFill)));

        private static object OnCoerceLevelBackgroundFill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelBackgroundFill((Brush)value);
            else
                return value;
        }

        private static void OnLevelBackgroundFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelBackgroundFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLevelBackgroundFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelBackgroundFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeLevelStyle")]
        public Brush LevelBackgroundFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LevelBackgroundFillProperty);
            }
            set
            {
                SetValue(LevelBackgroundFillProperty, value);
            }
        }

        #endregion

        #region LevelAnimationEnable
        public static readonly DependencyProperty LevelAnimationEnableProperty = DependencyProperty.Register("LevelAnimationEnable", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLevelAnimationEnableChanged), new CoerceValueCallback(OnCoerceLevelAnimationEnable)));

        private static object OnCoerceLevelAnimationEnable(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnLevelAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLevelAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public Boolean LevelAnimationEnable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(LevelAnimationEnableProperty);
            }
            set
            {
                SetValue(LevelAnimationEnableProperty, value);
            }
        }

        #endregion
        #region LevelIsInteractive
        public static readonly DependencyProperty LevelIsInteractiveProperty = DependencyProperty.Register("LevelIsInteractive", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLevelIsInteractiveChanged), new CoerceValueCallback(OnCoerceLevelIsInteractive)));

        private static object OnCoerceLevelIsInteractive(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnLevelIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceLevelIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public Boolean LevelIsInteractive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(LevelIsInteractiveProperty);
            }
            set
            {
                SetValue(LevelIsInteractiveProperty, value);
            }
        }

        #endregion
        #region LevelThickness
        public static readonly DependencyProperty LevelThicknessProperty = DependencyProperty.Register("LevelThickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnLevelThicknessChanged), new CoerceValueCallback(OnCoerceLevelThickness)));

        private static object OnCoerceLevelThickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceLevelThickness((int)value);
            else
                return value;
        }

        private static void OnLevelThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnLevelThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceLevelThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLevelThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeLevelStyle")]
        public int LevelThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(LevelThicknessProperty);
            }
            set
            {
                SetValue(LevelThicknessProperty, value);
            }
        }

        #endregion
        
        #endregion

        #region GaugeScaleRangeOptions
        #region Range1Visible
        public static readonly DependencyProperty Range1VisibleProperty = DependencyProperty.Register("Range1Visible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange1VisibleChanged), new CoerceValueCallback(OnCoerceRange1Visible)));

        private static object OnCoerceRange1Visible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange1VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange1Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Boolean Range1Visible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(Range1VisibleProperty);
            }
            set
            {
                SetValue(Range1VisibleProperty, value);
            }
        }

        #endregion
        #region Range1StartValue
        public static readonly DependencyProperty Range1StartValueProperty = DependencyProperty.Register("Range1StartValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)0, new PropertyChangedCallback(OnRange1StartValueChanged), new CoerceValueCallback(OnCoerceRange1StartValue)));

        private static object OnCoerceRange1StartValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange1StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range1StartValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range1StartValueProperty);
            }
            set
            {
                SetValue(Range1StartValueProperty, value);
            }
        }

        #endregion
        #region Range1EndValue
        public static readonly DependencyProperty Range1EndValueProperty = DependencyProperty.Register("Range1EndValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)50, new PropertyChangedCallback(OnRange1EndValueChanged), new CoerceValueCallback(OnCoerceRange1EndValue)));

        private static object OnCoerceRange1EndValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange1EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range1EndValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range1EndValueProperty);
            }
            set
            {
                SetValue(Range1EndValueProperty, value);
            }
        }

        #endregion
        #region Range1Thickness
        public static readonly DependencyProperty Range1ThicknessProperty = DependencyProperty.Register("Range1Thickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange1ThicknessChanged), new CoerceValueCallback(OnCoerceRange1Thickness)));

        private static object OnCoerceRange1Thickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1Thickness((int)value);
            else
                return value;
        }

        private static void OnRange1ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange1Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range1Thickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range1ThicknessProperty);
            }
            set
            {
                SetValue(Range1ThicknessProperty, value);
            }
        }

        #endregion
        #region Range1Offset
        public static readonly DependencyProperty Range1OffsetProperty = DependencyProperty.Register("Range1Offset", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange1OffsetChanged), new CoerceValueCallback(OnCoerceRange1Offset)));

        private static object OnCoerceRange1Offset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1Offset((Double)value);
            else
                return value;
        }

        private static void OnRange1OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range1Offset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range1OffsetProperty);
            }
            set
            {
                SetValue(Range1OffsetProperty, value);
            }
        }

        #endregion
        #region Range1Fill
        public static readonly DependencyProperty Range1FillProperty = DependencyProperty.Register("Range1Fill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(115,170,19)), new PropertyChangedCallback(OnRange1FillChanged), new CoerceValueCallback(OnCoerceRange1Fill)));

        private static object OnCoerceRange1Fill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange1FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange1Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeScaleRangeOptions")]
        public Brush Range1Fill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(Range1FillProperty);
            }
            set
            {
                SetValue(Range1FillProperty, value);
            }
        }

        #endregion
        #region Range1ZIndex
        public static readonly DependencyProperty Range1ZIndexProperty = DependencyProperty.Register("Range1ZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange1ZIndexChanged), new CoerceValueCallback(OnCoerceRange1ZIndex)));

        private static object OnCoerceRange1ZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange1ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange1ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange1ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange1ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range1ZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range1ZIndexProperty);
            }
            set
            {
                SetValue(Range1ZIndexProperty, value);
            }
        }
        #endregion


        #region Range2Visible
        public static readonly DependencyProperty Range2VisibleProperty = DependencyProperty.Register("Range2Visible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange2VisibleChanged), new CoerceValueCallback(OnCoerceRange2Visible)));

        private static object OnCoerceRange2Visible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange2VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange2Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Boolean Range2Visible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(Range2VisibleProperty);
            }
            set
            {
                SetValue(Range2VisibleProperty, value);
            }
        }

        #endregion
        #region Range2StartValue
        public static readonly DependencyProperty Range2StartValueProperty = DependencyProperty.Register("Range2StartValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)50, new PropertyChangedCallback(OnRange2StartValueChanged), new CoerceValueCallback(OnCoerceRange2StartValue)));

        private static object OnCoerceRange2StartValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange2StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range2StartValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range2StartValueProperty);
            }
            set
            {
                SetValue(Range2StartValueProperty, value);
            }
        }

        #endregion
        #region Range2EndValue
        public static readonly DependencyProperty Range2EndValueProperty = DependencyProperty.Register("Range2EndValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)80, new PropertyChangedCallback(OnRange2EndValueChanged), new CoerceValueCallback(OnCoerceRange2EndValue)));

        private static object OnCoerceRange2EndValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange2EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range2EndValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range2EndValueProperty);
            }
            set
            {
                SetValue(Range2EndValueProperty, value);
            }
        }

        #endregion
        #region Range2Thickness
        public static readonly DependencyProperty Range2ThicknessProperty = DependencyProperty.Register("Range2Thickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange2ThicknessChanged), new CoerceValueCallback(OnCoerceRange2Thickness)));

        private static object OnCoerceRange2Thickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2Thickness((int)value);
            else
                return value;
        }

        private static void OnRange2ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange2Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range2Thickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range2ThicknessProperty);
            }
            set
            {
                SetValue(Range2ThicknessProperty, value);
            }
        }

        #endregion
        #region Range2Offset
        public static readonly DependencyProperty Range2OffsetProperty = DependencyProperty.Register("Range2Offset", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange2OffsetChanged), new CoerceValueCallback(OnCoerceRange2Offset)));

        private static object OnCoerceRange2Offset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2Offset((Double)value);
            else
                return value;
        }

        private static void OnRange2OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range2Offset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range2OffsetProperty);
            }
            set
            {
                SetValue(Range2OffsetProperty, value);
            }
        }

        #endregion
        #region Range2Fill
        public static readonly DependencyProperty Range2FillProperty = DependencyProperty.Register("Range2Fill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(OnRange2FillChanged), new CoerceValueCallback(OnCoerceRange2Fill)));

        private static object OnCoerceRange2Fill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange2FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange2Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeScaleRangeOptions")]
        public Brush Range2Fill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(Range2FillProperty);
            }
            set
            {
                SetValue(Range2FillProperty, value);
            }
        }

        #endregion
        #region Range2ZIndex
        public static readonly DependencyProperty Range2ZIndexProperty = DependencyProperty.Register("Range2ZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange2ZIndexChanged), new CoerceValueCallback(OnCoerceRange2ZIndex)));

        private static object OnCoerceRange2ZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange2ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange2ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange2ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange2ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range2ZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range2ZIndexProperty);
            }
            set
            {
                SetValue(Range2ZIndexProperty, value);
            }
        }
        #endregion

        #region Range3Visible
        public static readonly DependencyProperty Range3VisibleProperty = DependencyProperty.Register("Range3Visible", typeof(Boolean), typeof(LinearMeter), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange3VisibleChanged), new CoerceValueCallback(OnCoerceRange3Visible)));

        private static object OnCoerceRange3Visible(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange3VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange3Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Boolean Range3Visible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(Range3VisibleProperty);
            }
            set
            {
                SetValue(Range3VisibleProperty, value);
            }
        }

        #endregion
        #region Range3StartValue
        public static readonly DependencyProperty Range3StartValueProperty = DependencyProperty.Register("Range3StartValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)80, new PropertyChangedCallback(OnRange3StartValueChanged), new CoerceValueCallback(OnCoerceRange3StartValue)));

        private static object OnCoerceRange3StartValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange3StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range3StartValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range3StartValueProperty);
            }
            set
            {
                SetValue(Range3StartValueProperty, value);
            }
        }

        #endregion
        #region Range3EndValue
        public static readonly DependencyProperty Range3EndValueProperty = DependencyProperty.Register("Range3EndValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)100, new PropertyChangedCallback(OnRange3EndValueChanged), new CoerceValueCallback(OnCoerceRange3EndValue)));

        private static object OnCoerceRange3EndValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange3EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range3EndValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range3EndValueProperty);
            }
            set
            {
                SetValue(Range3EndValueProperty, value);
            }
        }

        #endregion
        #region Range3Thickness
        public static readonly DependencyProperty Range3ThicknessProperty = DependencyProperty.Register("Range3Thickness", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange3ThicknessChanged), new CoerceValueCallback(OnCoerceRange3Thickness)));

        private static object OnCoerceRange3Thickness(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3Thickness((int)value);
            else
                return value;
        }

        private static void OnRange3ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange3Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range3Thickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range3ThicknessProperty);
            }
            set
            {
                SetValue(Range3ThicknessProperty, value);
            }
        }

        #endregion
        #region Range3Offset
        public static readonly DependencyProperty Range3OffsetProperty = DependencyProperty.Register("Range3Offset", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange3OffsetChanged), new CoerceValueCallback(OnCoerceRange3Offset)));

        private static object OnCoerceRange3Offset(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3Offset((Double)value);
            else
                return value;
        }

        private static void OnRange3OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public Double Range3Offset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(Range3OffsetProperty);
            }
            set
            {
                SetValue(Range3OffsetProperty, value);
            }
        }

        #endregion
        #region Range3Fill
        public static readonly DependencyProperty Range3FillProperty = DependencyProperty.Register("Range3Fill", typeof(Brush), typeof(LinearMeter), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRange3FillChanged), new CoerceValueCallback(OnCoerceRange3Fill)));

        private static object OnCoerceRange3Fill(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange3FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange3Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }
        [Category("GaugeScaleRangeOptions")]
        public Brush Range3Fill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(Range3FillProperty);
            }
            set
            {
                SetValue(Range3FillProperty, value);
            }
        }

        #endregion
        #region Range3ZIndex
        public static readonly DependencyProperty Range3ZIndexProperty = DependencyProperty.Register("Range3ZIndex", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange3ZIndexChanged), new CoerceValueCallback(OnCoerceRange3ZIndex)));

        private static object OnCoerceRange3ZIndex(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceRange3ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange3ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnRange3ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange3ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeScaleRangeOptions")]
        public int Range3ZIndex
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(Range3ZIndexProperty);
            }
            set
            {
                SetValue(Range3ZIndexProperty, value);
            }
        }
        #endregion




        #endregion

        #region GaugeOptions


        #region StartValue
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)0, new PropertyChangedCallback(OnStartValueChanged), new CoerceValueCallback(OnCoerceStartValue)));

        private static object OnCoerceStartValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceStartValue((Double)value);
            else
                return value;
        }

        private static void OnStartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnStartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceStartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeOptions")]
        [Browsable(false)]
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
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register("EndValue", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)100, new PropertyChangedCallback(OnEndValueChanged), new CoerceValueCallback(OnCoerceEndValue)));

        private static object OnCoerceEndValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceEndValue((Double)value);
            else
                return value;
        }

        private static void OnEndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnEndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceEndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeOptions")]
        [Browsable(false)]
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

        #region MajorIntervalCount
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMajorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMajorIntervalCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMajorIntervalCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeOptions")]
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

        #region MinorIntervalCount
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(LinearMeter), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnMinorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMinorIntervalCount(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinorIntervalCountChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateGaugeLayout();
        }

        [Category("GaugeOptions")]
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

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(LinearMeter), new UIPropertyMetadata((Double)0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                return LinearMeter.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeter LinearMeter = o as LinearMeter;
            if (LinearMeter != null)
                LinearMeter.OnValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool isChangingValue;
        protected virtual void OnValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateValue();
        }

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






        #endregion
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool IsFastLinearMeterControl { get { return false; } }

        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int SvgFOEmptyMargin { get { return LinearBaseModel != PredefinedBaseElementKinds.EmptyBar ? 76 : 58; } }
        #endregion

        #region Declarations
        List<string> matchChangedMap = new List<string>();
        IStringEditorManager stringManager;
        bool bInit;
        bool bDesign;
        bool bDatacontextChanging;
        LinearScaleMarker selectedMarker;
        LinearScaleLevelBar selectedLevel;
        LinearScaleRangeBar selectedRangeBar;
        
        ScreenDocument Document;
        MonitoredItemViewModel monitoredItemViewModel;

        CancellationTokenSource cts;

        DataValue lastDataValue;
        DispatcherOperation dpUpdateWarning;
        object lockObject = new object();


        IDocument document;
        double _StartValue;
        double _EndValue;
        TypeHelper typeHelper = new TypeHelper();

        public IEnumerable<PredefinedElementKind> PredefinedLinearMeterModelKinds { get { return LinearGaugeControl.PredefinedModels; } }
        bool bLoaded = false;
        internal int CustomBaseIndex
        {
            get { return 12; }
        }
        string ConverterLabel;
        #endregion

        #region Constructors
        public LinearMeter()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            if (linearGauge != null)
                linearGauge.DataContext = this;

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
                    UpdateGaugeLayout();
                    if (!bDesign)
                        InitControl();

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
                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel != null)
                    {
                        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                    }

                    Action action = () =>
                    {
                        if (monitoredItemViewModel.HasRange)
                        {
                            StartValue = TagMinValue == null || TagMinValue.TagReference == null ? System.Convert.ToDouble(monitoredItemViewModel.Range.Low, CultureInfo.InvariantCulture) : StartValue;
                            EndValue = TagMaxValue == null || TagMaxValue.TagReference == null ? System.Convert.ToDouble(monitoredItemViewModel.Range.High, CultureInfo.InvariantCulture) : EndValue;
                        }
                        bDatacontextChanging = false;

                        if (bInit && linearScale == null)
                            UpdateGaugeLayout();
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

                            var eu = monitoredItemViewModel.HasRange;
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
            };
        }
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            if (bDesign || stringManager == null)
                return;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                UpdateCustomValue(Value);
                if (linearScale != null)
                {
                    linearScale.LabelOptions.FormatString = "";
                    linearScale.LabelOptions.FormatString = LabelStringFormat;
                }
            });
        }
        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                var m = (MonitoredItemViewModel)sender;

                if (m.IsReadOnly)
                    return;

                bool bForceDispatcherOperation = false;
                lock (lockObject)
                {
                    bForceDispatcherOperation = lastDataValue == null;
                    lastDataValue = m.DataValue;
                }

                if (dpUpdateWarning == null || bForceDispatcherOperation ||
                    dpUpdateWarning.Status == DispatcherOperationStatus.Completed ||
                    dpUpdateWarning.Status == DispatcherOperationStatus.Aborted)
                {
                    dpUpdateWarning = Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(this, () =>
                    {
                        if (bDispose)
                            return;

                        ManageWarning();
                    });
                }
            }
        }
        private void ManageWarning()
        {
            DataValue dataValue = null;
            lock (lockObject)
            {
                dataValue = lastDataValue;
                lastDataValue = null;
            }

            if (dataValue != null && Opc.Ua.StatusCode.IsBad(dataValue.StatusCode))
                ShowMarker(true);
            else
                ShowMarker(false);
        }
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }

        public void LinearMeterInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region Methods
        internal string LoadSVGBackContent()
        {
            switch (LayoutMode)
            {
                case LinearScaleLayoutMode.LeftToRight:
                case LinearScaleLayoutMode.RightToLeft:
                    return string.Format("{0}_H", LinearBaseModel.ToString());
                case LinearScaleLayoutMode.BottomToTop:
                case LinearScaleLayoutMode.TopToBottom:
                    return string.Format("{0}_V", LinearBaseModel.ToString());
                default:
                    return string.Format("{0}_H", LinearBaseModel.ToString());
            }
        }
        private void UpdateGaugeLayout()
        {
            if (bDispose || bDatacontextChanging)
                return;

            if (bDesign || !bInit)
            {
                _StartValue = UseEUnit ? 0.0 : MinValue;
                _EndValue = UseEUnit ? 100.0 : MaxValue;
            }
            else
            {
                _StartValue = StartValue;
                _EndValue = EndValue;
            }

            UpdateBackBorder();
            UpdateGuageControl();
            UpdateBaseLinearScale();
            UpdateLinearScale();
            UpdateLinearLayer();
            UpdateLevel();
            UpdateLinearScaleRange1();
            UpdateLinearScaleRange2();
            UpdateLinearScaleRange3();
            UpdateMarkers();
            UpdateRangeBars();
            UpdateCustomElements();
            UpdateValue();
        }

        private void UpdateCustomElements()
        {
            if (linearGauge == null || linearScale == null)
                return;
            try
            {
                string meas = ConverterLabel ?? EngeneeringUnit;
                if (!ShowEngeneeringUnit || string.IsNullOrEmpty(meas))
                {
                    if (gridUnitContainer != null)
                    {
                        if (linearScale.CustomElements.Contains(gridUnitContainer))
                            linearScale.CustomElements.Remove(gridUnitContainer);

                        gridUnitContainer.Content = null;
                        gridUnitContainer = null;
                    }
                }
                else
                {
                    if (gridUnitContainer == null)
                    {
                        gridUnitContainer = new ScaleCustomElement()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            ZIndex = 100,
                            Margin = new Thickness(-30)
                        };

                        TextBlock text = (TextBlock)LoadTemplate("Templates", "engeneeringUnit"); 
                        if(text != null)
                            gridUnitContainer.Content = text;

                        if (!linearScale.CustomElements.Contains(gridUnitContainer))
                            linearScale.CustomElements.Add(gridUnitContainer);
                    }

                    TextBlock _text = (TextBlock)gridUnitContainer.Content;
                    if (_text != null)
                        _text.Text = meas;
                }
            }
            catch (Exception)
            {
            }


            try
            {
                if (!ShowValue)
                {
                    if (gridValueContainer != null)
                    {
                        if (linearScale.CustomElements.Contains(gridValueContainer))
                            linearScale.CustomElements.Remove(gridValueContainer);

                        gridValueContainer.Content = null;
                        gridValueContainer = null;
                    }
                }
                else
                {
                    if (gridValueContainer == null)
                    {
                        gridValueContainer = new ScaleCustomElement()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            ZIndex = 100,
                            Margin = new Thickness(-30)
                        };

                        TextBlock text = (TextBlock)LoadTemplate("Templates", "value");
                        if(text != null)
                            gridValueContainer.Content = text;

                        if (!linearScale.CustomElements.Contains(gridValueContainer))
                            linearScale.CustomElements.Add(gridValueContainer);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        private void UpdateRangeBars()
        {
            if (linearGauge == null || linearBaseScale == null || linearScale == null)
                return;
            try
            {
                /* Background Arc */
                if (!RangeBarVisible || RangeBarBackground == null ||
                    RangeBarBackground == transparent)
                {
                    if (linearRangeBackground != null)
                    {
                        if (linearBaseScale.RangeBars.Contains(linearRangeBackground))
                            linearBaseScale.RangeBars.Remove(linearRangeBackground);

                        linearRangeBackground.Presentation = null;
                        linearRangeBackground.Options = null;
                        linearRangeBackground.Animation = null;
                        linearRangeBackground = null;
                    }
                }
                else
                {
                    if (linearRangeBackground == null)
                    {
                        linearRangeBackground = new LinearScaleRangeBar();
                        linearRangeBackground.Options = new LinearScaleRangeBarOptions();
                        linearRangeBackground.Presentation = new DefaultLinearScaleRangeBarPresentation();

                        if (!linearBaseScale.RangeBars.Contains(linearRangeBackground))
                            linearBaseScale.RangeBars.Add(linearRangeBackground);
                    }

                    (linearRangeBackground.Presentation as PredefinedLinearScaleRangeBarPresentation).Fill = RangeBarBackground;
                    linearRangeBackground.Options.Thickness = RangeBarThickness;
                    linearRangeBackground.Options.ZIndex = RangeBarZIndex - 1;
                    linearRangeBackground.Options.Offset = RangeBarOffset;
                    linearRangeBackground.IsInteractive = false;
                    linearRangeBackground.Animation = null;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                /* Animated Arc */
                if (!RangeBarVisible || RangeBarFill == null ||
                    RangeBarFill == transparent)
                {
                    if (linearRange != null)
                    {
                        if (linearScale.RangeBars.Contains(linearRange))
                            linearScale.RangeBars.Remove(linearRange);

                        linearRange.ValueChanged -= arcNeedle_ValueChanged;

                        linearRange.Presentation = null;
                        linearRange.Options = null;
                        linearRange.Animation = null;
                        linearRange = null;
                    }
                }
                else
                {
                    if (linearRange == null)
                    {
                        linearRange = new LinearScaleRangeBar();
                        linearRange.Options = new LinearScaleRangeBarOptions();
                        linearRange.Presentation = new DefaultLinearScaleRangeBarPresentation();

                        if (!linearScale.RangeBars.Contains(linearRange))
                            linearScale.RangeBars.Add(linearRange);

                        if (!bDesign)
                        {
                            linearRange.ValueChanged += arcNeedle_ValueChanged;
                        }
                    }

                    (linearRange.Presentation as PredefinedLinearScaleRangeBarPresentation).Fill = RangeBarFill;
                    linearRange.Options.Thickness = RangeBarThickness;
                    linearRange.Options.ZIndex = RangeBarZIndex;
                    linearRange.Options.Offset = RangeBarOffset;
                    linearRange.IsInteractive = RangeBarIsInteractive;
                    if (RangeBarAnimationEnable && !bDesign)
                    {
                        if (linearRange.Animation == null)
                            linearRange.Animation = new IndicatorAnimation() { Enable = true };
                    }
                    else
                        linearRange.Animation = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateMarkers()
        {
            if (linearGauge == null || linearScale == null)
                return;

            try
            {
                if (MarkerVisible)
                {
                    if (linearScaleMarker == null)
                    {
                        linearScaleMarker = new LinearScaleMarker();
                        linearScaleMarker.Options = new LinearScaleMarkerOptions();
                        CustomLinearScaleMarkerPresentation linearScaleMarkerpresentation = new CustomLinearScaleMarkerPresentation();
                        linearScaleMarkerpresentation.MarkerTemplate = (ControlTemplate)LoadTemplate("Templates", "markerTemplate");
                        linearScaleMarker.Presentation = linearScaleMarkerpresentation;


                        if (!linearScale.Markers.Contains(linearScaleMarker))
                            linearScale.Markers.Add(linearScaleMarker);

                        if (!bDesign)
                        {
                            linearScaleMarker.ValueChanged += arcNeedle_ValueChanged;
                        }
                    }

                    linearScaleMarker.Options.ZIndex = MarkerZIndex;
                    linearScaleMarker.Options.Offset = MarkerOffset;
                    linearScaleMarker.Options.FactorHeight = MarkerFactorHeight;
                    linearScaleMarker.Options.FactorWidth = MarkerFactorWidth;
                    linearScaleMarker.IsInteractive = MarkerIsInteractive;
                    if (MarkerAnimationEnable && !bDesign)
                    {
                        if (linearScaleMarker.Animation == null)
                            linearScaleMarker.Animation = new IndicatorAnimation() { Enable = true };
                    }
                    else
                        linearScaleMarker.Animation = null;
                }
                else
                {
                    if (linearScaleMarker != null)
                    {
                        linearScaleMarker.ValueChanged -= arcNeedle_ValueChanged;

                        linearScale.Markers.Clear();
                        linearScaleMarker.Presentation = null;
                        linearScaleMarker.Options = null;
                        linearScaleMarker.Animation = null;
                        linearScaleMarker = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateLinearScaleRange1()
        {
            if (linearGauge == null || linearScale == null)
                return;

            try
            {
                if (Range1Visible)
                {
                    if (LinearScaleRange1 == null)
                    {
                        LinearScaleRange1 = new LinearScaleRange();
                        LinearScaleRange1.Options = new RangeOptions();
                        LinearScaleRange1.Presentation = new DefaultLinearScaleRangePresentation();

                        if (!linearScale.Ranges.Contains(LinearScaleRange1))
                            linearScale.Ranges.Add(LinearScaleRange1);
                    }

                                (LinearScaleRange1.Presentation as PredefinedLinearScaleRangePresentation).Fill = Range1Fill;
                    LinearScaleRange1.StartValue = new RangeValue((_EndValue - _StartValue) * Range1StartValue / 100 + _StartValue);
                    LinearScaleRange1.EndValue = new RangeValue((_EndValue - _StartValue) * Range1EndValue / 100 + _StartValue);

                    LinearScaleRange1.Options.Thickness = Range1Thickness;
                    LinearScaleRange1.Options.Offset = Range1Offset;
                    LinearScaleRange1.Options.ZIndex = Range1ZIndex;
                }
                else
                {
                    if (LinearScaleRange1 != null)
                    {
                        if (linearScale.Ranges.Contains(LinearScaleRange1))
                            linearScale.Ranges.Remove(LinearScaleRange1);
                        LinearScaleRange1.Presentation = null;
                        LinearScaleRange1.Options = null;
                        LinearScaleRange1 = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateLinearScaleRange2()
        {
            if (linearGauge == null || linearScale == null)
                return;

            try
            {
                if (Range2Visible)
                {
                    if (LinearScaleRange2 == null)
                    {
                        LinearScaleRange2 = new LinearScaleRange();
                        LinearScaleRange2.Options = new RangeOptions();
                        LinearScaleRange2.Presentation = new DefaultLinearScaleRangePresentation();

                        if (!linearScale.Ranges.Contains(LinearScaleRange2))
                            linearScale.Ranges.Add(LinearScaleRange2);
                    }

                                (LinearScaleRange2.Presentation as PredefinedLinearScaleRangePresentation).Fill = Range2Fill;
                    LinearScaleRange2.StartValue = new RangeValue((_EndValue - _StartValue) * Range2StartValue / 100 + _StartValue);
                    LinearScaleRange2.EndValue = new RangeValue((_EndValue - _StartValue) * Range2EndValue / 100 + _StartValue);
                    LinearScaleRange2.Options.Thickness = Range2Thickness;
                    LinearScaleRange2.Options.Offset = Range2Offset;
                    LinearScaleRange2.Options.ZIndex = Range2ZIndex;
                }
                else
                {
                    if (LinearScaleRange2 != null)
                    {
                        if (linearScale.Ranges.Contains(LinearScaleRange2))
                            linearScale.Ranges.Remove(LinearScaleRange2);
                        LinearScaleRange2.Presentation = null;
                        LinearScaleRange2.Options = null;
                        LinearScaleRange2 = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateLinearScaleRange3()
        {
            if (linearGauge == null || linearScale == null)
                return;
            try
            {
                if (Range3Visible)
                {
                    if (LinearScaleRange3 == null)
                    {
                        LinearScaleRange3 = new LinearScaleRange();
                        LinearScaleRange3.Options = new RangeOptions();
                        LinearScaleRange3.Presentation = new DefaultLinearScaleRangePresentation();

                        if (!linearScale.Ranges.Contains(LinearScaleRange3))
                            linearScale.Ranges.Add(LinearScaleRange3);
                    }

                    (LinearScaleRange3.Presentation as PredefinedLinearScaleRangePresentation).Fill = Range3Fill;
                    LinearScaleRange3.StartValue = new RangeValue((_EndValue - _StartValue) * Range3StartValue / 100 + _StartValue);
                    LinearScaleRange3.EndValue = new RangeValue((_EndValue - _StartValue) * Range3EndValue / 100 + _StartValue);
                    LinearScaleRange3.Options.Thickness = Range3Thickness;
                    LinearScaleRange3.Options.Offset = Range3Offset;
                    LinearScaleRange3.Options.ZIndex = Range3ZIndex;
                }
                else
                {
                    if (LinearScaleRange3 != null)
                    {
                        if (linearScale.Ranges.Contains(LinearScaleRange3))
                            linearScale.Ranges.Remove(LinearScaleRange3);
                        LinearScaleRange3.Presentation = null;
                        LinearScaleRange3.Options = null;
                        LinearScaleRange3 = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateLevel()
        {
            if (linearGauge == null || linearScale == null)
                return;

            try
            {
                if (LevelVisible)
                {
                    if (linearLevelBar == null)
                    {
                        linearLevelBar = new LinearScaleLevelBar();
                        linearLevelBar.Options = new LinearScaleLevelBarOptions();

                        if (!linearScale.LevelBars.Contains(linearLevelBar))
                            linearScale.LevelBars.Add(linearLevelBar);

                        if (!bDesign)
                        {
                            linearLevelBar.ValueChanged += arcNeedle_ValueChanged;
                        }

                        CustomLinearScaleLevelBarPresentation scalelabelpresentation = new CustomLinearScaleLevelBarPresentation();
                        scalelabelpresentation.LevelBarBackgroundTemplate = (ControlTemplate)LoadTemplate("Templates", "levelBackgroundTemplate");
                        scalelabelpresentation.LevelBarForegroundTemplate = (ControlTemplate)LoadTemplate("Templates", "levelForegroundTemplate");
                        linearLevelBar.Presentation = scalelabelpresentation;
                    }

                    linearLevelBar.IsInteractive = LevelIsInteractive;
                    if (LevelAnimationEnable && !bDesign)
                    {
                        if (linearLevelBar.Animation == null)
                            linearLevelBar.Animation = new IndicatorAnimation() { Enable = true };
                    }
                    else
                        linearLevelBar.Animation = null;

                    linearLevelBar.Options.FactorThickness = LevelThickness;
                    linearLevelBar.Options.ZIndex = LevelZIndex;
                    linearLevelBar.Options.Offset = LevelOffset;

                }
                else
                {
                    if (linearLevelBar != null)
                    {
                        linearLevelBar.ValueChanged -= arcNeedle_ValueChanged;

                        linearScale.LevelBars.Clear();
                        linearLevelBar.Presentation = null;
                        linearLevelBar.Options = null;
                        linearLevelBar.Animation = null;
                        linearLevelBar = null;
                    }
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

            if (isChangingValue)
                Value = ev.NewValue;
        }

        private void UpdateLinearLayer()
        {
            if (linearGauge == null || linearScale == null)
                return;

            try
            {
                if ((int)LinearBaseModel >= CustomBaseIndex || LinearBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
                {
                    if (linearLayer != null)
                    {
                        linearScale.Layers.Clear();
                        linearLayer = null;
                    }
                }
                else
                {
                    if (linearLayer == null)
                    {
                        linearLayer = new LinearScaleLayer();

                        if (!linearScale.Layers.Contains(linearLayer))
                            linearScale.Layers.Add(linearLayer);
                    }
                    linearLayer.Presentation = GetLinearScaleBackground(LinearBaseModel);
                    UpdateDevBackColor(Background);
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateBaseLinearScale()
        {
            if (linearGauge == null)
                return;

            if (linearBaseScale == null)
            {
                linearBaseScale = new LinearScale();
                linearBaseScale.Name = "linearBaseScale";
                if (!linearGauge.Scales.Contains(linearBaseScale))
                    linearGauge.Scales.Add(linearBaseScale);
            }

            linearBaseScale.LayoutMode = LayoutMode;

            linearBaseScale.ShowLabels = DevExpress.Utils.DefaultBoolean.False;
            linearBaseScale.ShowMajorTickmarks = DevExpress.Utils.DefaultBoolean.False;
            linearBaseScale.ShowMinorTickmarks = DevExpress.Utils.DefaultBoolean.False;
            linearBaseScale.ShowLine = DevExpress.Utils.DefaultBoolean.False;
            linearBaseScale.StartValue = 0;
            linearBaseScale.EndValue = 100;
        }

        private void UpdateLinearScale()
        {
            if (linearGauge == null)
                return;

            try
            {
                if (linearScale == null)
                {
                    linearScale = new LinearScale();
                    linearScale.MajorTickmarkOptions = new MajorTickmarkOptions();
                    linearScale.MinorTickmarkOptions = new MinorTickmarkOptions();

                    CustomScaleLabelPresentation scalelabelpresentation = new CustomScaleLabelPresentation();
                    scalelabelpresentation.LabelTemplate = (ControlTemplate)LoadTemplate("Templates", "scaleLabelTemplate");
                    linearScale.LabelPresentation = scalelabelpresentation;

                    LinearScaleLinePresentation arcscalelinePresentationpresentation = GetLineScalePresentation(LinePresentation);
                    linearScale.LinePresentation = arcscalelinePresentationpresentation;

                    linearScale.LabelOptions = new LinearScaleLabelOptions();

                    if (!linearGauge.Scales.Contains(linearScale))
                        linearGauge.Scales.Add(linearScale);
                }

                linearScale.LayoutMode = LayoutMode;
                linearScale.MajorIntervalCount = MajorIntervalCount;
                linearScale.MinorIntervalCount = MinorIntervalCount;
                linearScale.StartValue = _StartValue;
                linearScale.EndValue = _EndValue;

                TickmarksPresentation arcscaletickmarkspresentation = GetTickmarkPresentation(TickmarksPresentation);
                linearScale.TickmarksPresentation = arcscaletickmarkspresentation;
                var _dtpres = linearScale.TickmarksPresentation as PredefinedTickmarksPresentation;
                _dtpres.MajorTickBrush = MajorTickmarkFill;
                _dtpres.MinorTickBrush = MinorTickmarkFill;

                var _lpres = linearScale.LinePresentation as PredefinedLinearScaleLinePresentation;
                _lpres.Fill = TickmarkFill;



                linearScale.MajorTickmarkOptions.FactorLength = MajorTickmarkFactorLength;
                linearScale.MajorTickmarkOptions.ZIndex = MajorTickmarkZIndex;
                linearScale.MajorTickmarkOptions.FactorThickness = MajorTickmarkFactorThickness;
                linearScale.MajorTickmarkOptions.Offset = MajorTickmarkOffset;
                linearScale.MajorTickmarkOptions.ShowFirst = MajorTickmarkShowFirst;
                linearScale.MajorTickmarkOptions.ShowLast = MajorTickmarkShowLast;

                linearScale.MinorTickmarkOptions.FactorLength = MinorTickmarkFactorLength;
                linearScale.MinorTickmarkOptions.ZIndex = MinorTickmarkZIndex;
                linearScale.MinorTickmarkOptions.FactorThickness = MinorTickmarkFactorThickness;
                linearScale.MinorTickmarkOptions.Offset = MinorTickmarkOffset;
                linearScale.MinorTickmarkOptions.ShowTicksForMajor = MinorTickmarkShowTicksForMajor;

                linearScale.LabelOptions.Orientation = LabelOrientation;
                linearScale.LabelOptions.ShowFirst = ShowFirstLabel;
                linearScale.LabelOptions.ShowLast = ShowLastLabel;
                linearScale.LabelOptions.Offset = LabelOffset;
                linearScale.LabelOptions.ZIndex = LabelZIndex;
                linearScale.LabelOptions.FormatString = LabelStringFormat;
            }
            catch (Exception)
            {
            }
        }
        const string horizontal = "Horizontal";
        const string vertical = "Vertical";
        private void UpdateGuageControl()
        {
            try
            {
                if (linearGauge == null)
                {
                    linearGauge = new LinearGaugeControl()
                    {
                        EnableAnimation = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontSize = 14,
                        FontStyle = FontStyles.Normal,
                        FontWeight = FontWeights.Normal,
                        FontFamily = new FontFamily("Segoe UI"),
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        ClipToBounds = true
                    };

                    linearGauge.Model = new LinearCleanWhiteModel();
                    var innerPadding = (linearGauge.Model as GaugeModelBase).InnerPadding;
                    if (LinearBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
                    {
                        if (GetOrientation() == horizontal)
                            (linearGauge.Model as GaugeModelBase).InnerPadding = new Thickness(innerPadding.Left, Properties.Settings.Default.MeterInnerPadding, innerPadding.Right, Properties.Settings.Default.MeterInnerPadding);
                        else
                            (linearGauge.Model as GaugeModelBase).InnerPadding = new Thickness(Properties.Settings.Default.MeterInnerPadding, innerPadding.Top, Properties.Settings.Default.MeterInnerPadding, innerPadding.Bottom);
                    }

                    Grid.SetZIndex(linearGauge, 1);

                    if (!container.Children.Contains(linearGauge))
                        container.Children.Add(linearGauge);
                }

                if(GetOrientation() == vertical)
                {
                    linearGauge.Width = 150;
                    linearGauge.Height = 300;
                }
                else
                {
                    linearGauge.Width = 300;
                    linearGauge.Height = 150;
                }

                linearGauge.PreviewMouseDown -= circularGaugeObject_PreviewMouseDown;
                linearGauge.PreviewTouchDown -= circularGaugeObject_PreviewTouchDown;

                if (MarkerIsInteractive || RangeBarIsInteractive || LevelIsInteractive)
                {
                    linearGauge.PreviewMouseDown += circularGaugeObject_PreviewMouseDown;
                    linearGauge.PreviewTouchDown += circularGaugeObject_PreviewTouchDown;
                }
                linearGauge.Foreground = LabelForeground;
            }
            catch (Exception)
            {
            }
        }

        private void circularGaugeObject_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            LinearGaugeControl meter = sender as LinearGaugeControl;
            if (meter != null)
            {
                LinearGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetTouchPoint(meter).Position);
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) && MarkerIsInteractive ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) && RangeBarIsInteractive ||
                (hitInfo.InLevelBar || hitInfo.InScale || hitInfo.InRange) && LevelIsInteractive)
                {
                    isChangingValue = true;
                }
            }
        }


        private void circularGaugeObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LinearGaugeControl meter = sender as LinearGaugeControl;
            if (meter != null)
            {
                LinearGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetPosition(meter));
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) && MarkerIsInteractive ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) && RangeBarIsInteractive ||
                (hitInfo.InLevelBar || hitInfo.InScale || hitInfo.InRange) && LevelIsInteractive)
                {
                    isChangingValue = true;
                }
            }
        }

        private void UpdateBackBorder()
        {

            if ((int)LinearBaseModel < CustomBaseIndex || LinearBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
            {
                if (BackContent != null)
                {
                    if (container.Children.Contains(BackContent))
                        container.Children.Remove(BackContent);

                    BackContent.Content = null;
                    BackContent = null;
                }
            }
            else
            {
                if (BackContent == null)
                {
                    BackContent = new ContentControl();
                    BackContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    BackContent.VerticalContentAlignment = VerticalAlignment.Stretch;

                    Grid.SetZIndex(BackContent, 0);

                    if (!container.Children.Contains(BackContent))
                        container.Children.Add(BackContent);
                }

                FrameworkElement content = LoadBackContent();
                BackContent.Content = content;
                if (content != null)
                {
                    (from c in (BackContent.Content as UIElement).GetVisualChildrenOfType<Shape>()
                     where (c.Tag as String) == Properties.Settings.Default.TagBackground
                     select c).ToList().ForEach(child =>
                     {
                         child.Fill = Background;
                     });

                    if (RangeBarVisible)
                        (from c in (BackContent.Content as UIElement).GetVisualChildrenOfType<Shape>()
                         where (c.Tag as String) == Properties.Settings.Default.TagFill
                         select c).ToList().ForEach(child =>
                         {
                             child.Fill = RangeBarFill;
                         });
                }
            }
        }
        private FrameworkElement LoadBackContent()
        {
            try
            {
                string basename = string.Format("meter{0}.xaml", (int)LinearBaseModel - CustomBaseIndex + 1);
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.MeterContainers.{1}", typeof(LinearMeter).Namespace, basename));
                if (stream == null)
                    return null;
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                FrameworkElement content = (FrameworkElement)canvas.TryFindResource(string.Format("{0}_{1}", LinearBaseModel.ToString(), GetOrientation()));

                if (LayoutMode == LinearScaleLayoutMode.LeftToRight ||
                                  LayoutMode == LinearScaleLayoutMode.RightToLeft)
                {
                    content.Width = 300;
                    content.Height = 150;
                }
                else
                {
                    content.Height = 300;
                    content.Width = 150;
                }
                return content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string GetOrientation()
        {
            if (LayoutMode == LinearScaleLayoutMode.LeftToRight ||
                LayoutMode == LinearScaleLayoutMode.RightToLeft)
                return horizontal;
            else
                return vertical;
        }

        private object LoadTemplate(string resources, string template)
        {
            try
            {
                string basename = string.Format("{0}.xaml", resources);
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.MeterContainers.{1}", typeof(LinearMeter).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                object content = (object)canvas.TryFindResource(template);
                return (object)content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private void UpdateValue()
        {
            double _value = Value;
            if (bDesign)
                _value = (EndValue - StartValue) / 2;

            if (linearLevelBar != null)
                linearLevelBar.Value = _value;
            if (linearRange != null)
                linearRange.Value = _value;
            if (linearScaleMarker != null)
                linearScaleMarker.Value = _value;
            UpdateCustomValue(_value);

            if (linearBaseScale != null)
            {
                linearBaseScale.StartValue = 0;
                linearBaseScale.EndValue = 100;
                if (linearRangeBackground != null)
                    linearRangeBackground.Value = 100;
            }
        }
        void UpdateCustomValue(double value)
        {
            if (gridValueContainer != null)
            {
                TextBlock text = gridValueContainer.GetChildrenOfType<TextBlock>().FirstOrDefault();
                if (text != null)
                    text.Text = ConvertValue(value);
            }
        }

        private void ShowMarker(bool showWarning)
        {
            if (bDesign || !showWarning)
            {
                container.ToolTip = null;
                if (linearLevelBar != null)
                    linearLevelBar.Visible = true;
                if (linearRange != null)
                    linearRange.Visible = true;
                if (linearScaleMarker != null)
                    linearScaleMarker.Visible = true;
                if (WarningMarker != null)
                {
                    if (container.Children.Contains(WarningMarker))
                        container.Children.Remove(WarningMarker);
                    WarningMarker = null;
                }
            }
            else if (showWarning)
            {
                container.ToolTip = Properties.Resources.NullValueWarning;
                if (linearLevelBar != null)
                    linearLevelBar.Visible = false;
                if (linearRange != null)
                    linearRange.Visible = false;
                if (linearScaleMarker != null)
                    linearScaleMarker.Visible = false;
                if (WarningMarker == null)
                {
                    WarningMarker = new Image()
                    {
                        Width = 16,
                        Height = 16,
                        Source = TryFindResource("Warning") as BitmapImage,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };

                    if (!container.Children.Contains(WarningMarker))
                        container.Children.Add(WarningMarker);

                    Grid.SetColumnSpan(WarningMarker, 20);
                    Grid.SetRowSpan(WarningMarker, 20);
                    Grid.SetZIndex(WarningMarker, 2000);
                }
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
        private LinearScaleLevelBarPresentation GetLevelBarPresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (LinearScaleLevelBar.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (LinearScaleLevelBarPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (LinearScaleLevelBarPresentation)Activator.CreateInstance(((LinearScaleLevelBar.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
            }
        }
        private TickmarksPresentation GetTickmarkPresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (LinearScale.PredefinedTickmarksPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (TickmarksPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (TickmarksPresentation)Activator.CreateInstance(((LinearScale.PredefinedTickmarksPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
            }
        }
        private LinearScaleLinePresentation GetLineScalePresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (LinearScale.PredefinedLinePresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (LinearScaleLinePresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (LinearScaleLinePresentation)Activator.CreateInstance(((LinearScale.PredefinedLinePresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)PredefinedElementKinds.Future) as PredefinedElementKind).Type);
            }

        }
        private LinearScaleLayerPresentation GetLinearScaleBackground(PredefinedBaseElementKinds value)
        {
                try
                {
                        PredefinedElementKind gaugeModelKind = (LinearScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                        return (LinearScaleLayerPresentation)Activator.CreateInstance(gaugeModelKind.Type);
                }
                catch
                {
                     return (LinearScaleLayerPresentation)Activator.CreateInstance(typeof(CleanWhiteLinearScaleBackgroundLayerPresentation));
                }
         }
        private void UpdateDevBackColor(Brush newValue)
        {
            if (bDispose)
                return;
            PredefinedLinearScaleLayerPresentation layer = (PredefinedLinearScaleLayerPresentation)linearLayer.Presentation;
            if (layer != null)
                layer.Fill = newValue;
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
        #endregion

        #region DynObjects
        LinearGaugeControl linearGauge;
        LinearScale linearBaseScale;
        LinearScale linearScale;
        LinearScaleRange LinearScaleRange1;
        LinearScaleRange LinearScaleRange2;
        LinearScaleRange LinearScaleRange3;
        LinearScaleMarker linearScaleMarker;
        LinearScaleLayer linearLayer;
        LinearScaleLevelBar linearLevelBar;
        LinearScaleRangeBar linearRangeBackground;
        LinearScaleRangeBar linearRange;
        ScaleCustomElement gridValueContainer;
        ScaleCustomElement gridUnitContainer;
        ContentControl BackContent;
        Image WarningMarker;
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
                                EndValue = val;
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
                                StartValue = val;
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
        bool bTerminateExecuted;
   
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

        #region IDisposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            DetachOverrideBaseProperties();

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (dpUpdateWarning != null &&
                dpUpdateWarning.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateWarning.Status != DispatcherOperationStatus.Completed)
                dpUpdateWarning.Abort();

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            container.Children.Clear();
            if (BackContent != null)
                BackContent.Content = null;
            BackContent = null;

            if (linearScale != null)
            {
                if (linearScale.LabelPresentation != null)
                {
                    CustomScaleLabelPresentation scalelabelpresentation = (CustomScaleLabelPresentation)linearScale.LabelPresentation;
                    ControlTemplate controltemplate = scalelabelpresentation.LabelTemplate;
                    scalelabelpresentation.LabelTemplate = null;
                }
                linearScale.LabelPresentation = null;
                linearScale.Layers.Clear();
                linearScale.LevelBars.Clear();
                linearScale.LabelOptions = null;
                linearScale.Ranges.Clear();
                linearScale.Markers.Clear();
                linearScale.CustomElements.Clear();
            }

            if (linearBaseScale != null)
            {
                if (linearBaseScale.LabelPresentation != null)
                {
                    CustomScaleLabelPresentation scalelabelpresentation = (CustomScaleLabelPresentation)linearBaseScale.LabelPresentation;
                    ControlTemplate controltemplate = scalelabelpresentation.LabelTemplate;
                    scalelabelpresentation.LabelTemplate = null;
                }
                linearBaseScale.LabelPresentation = null;
                linearBaseScale.Layers.Clear();
                linearBaseScale.LevelBars.Clear();
                linearBaseScale.LabelOptions = null;
                linearBaseScale.Ranges.Clear();
                linearBaseScale.Markers.Clear();
                linearBaseScale.CustomElements.Clear();
            }

            if (linearLevelBar != null)
            {
                linearLevelBar.ValueChanged -= arcNeedle_ValueChanged;
                linearLevelBar.Options = null;
                linearLevelBar.Presentation = null;
                linearLevelBar.Animation = null;
            }

            if (linearScaleMarker != null)
            {
                linearScaleMarker.ValueChanged -= arcNeedle_ValueChanged;
                linearScaleMarker.Presentation = null;
                linearScaleMarker.Options = null;
                linearScaleMarker.Animation = null;
                linearScaleMarker = null;
            }

            if (linearRange != null)
            {
                linearRange.ValueChanged -= arcNeedle_ValueChanged;
                linearRange.Presentation = null;
                linearRange.Options = null;
                linearRange.Animation = null;
                linearRange = null;
            }

            if (linearRangeBackground != null)
            {
                linearRangeBackground.Presentation = null;
                linearRangeBackground.Options = null;
                linearRangeBackground.Animation = null;
                linearRangeBackground = null;
            }

            if (gridUnitContainer != null)
                gridUnitContainer.Content = null;

            if (gridValueContainer != null)
                gridValueContainer.Content = null;

            if (LinearScaleRange1 != null)
            {
                LinearScaleRange1.Presentation = null;
                LinearScaleRange1.Options = null;
            }
            if (LinearScaleRange2 != null)
            {
                LinearScaleRange2.Presentation = null;
                LinearScaleRange2.Options = null;
            }
            if (LinearScaleRange3 != null)
            {
                LinearScaleRange3.Presentation = null;
                LinearScaleRange3.Options = null;
            }

            if (linearGauge != null)
            {
                linearGauge.PreviewTouchDown -= circularGaugeObject_PreviewTouchDown;
                linearGauge.PreviewMouseDown -= circularGaugeObject_PreviewMouseDown;
                linearGauge.Scales.Clear();
            }

            linearGauge = null;
            linearScale = null;
            linearLayer = null;
            linearLevelBar = null;

            LinearScaleRange1 = null;
            LinearScaleRange2 = null;
            LinearScaleRange3 = null;
            linearScaleMarker = null;
            linearRangeBackground = null;
            linearRange = null;
            gridValueContainer = null;
            gridUnitContainer = null;
            linearGauge = null;
            WarningMarker = null;

            if (!bDesign)
            {
                typeHelper.TerminateExecution(this, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, mintag, mintagMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, maxtag, maxtagMonitoredItemViewModel);
            }

            typeHelper.Dispose();
            typeHelper = null;
        }
        #endregion

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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0d);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, 100d);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
                dt.DataType = typeof(Double);
                dt.VisualTree = factory;
                mapDataTemplates.Add(Range1StartValueProperty, dt);
                mapDataTemplates.Add(Range1EndValueProperty, dt);
                mapDataTemplates.Add(Range2StartValueProperty, dt);
                mapDataTemplates.Add(Range2EndValueProperty, dt);
                mapDataTemplates.Add(Range3StartValueProperty, dt);
                mapDataTemplates.Add(Range3EndValueProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IStringIDAware

        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(EngeneeringUnit))
                list.Add(EngeneeringUnit);
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(EngeneeringUnit))
            {
                var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(LinearMeter), EngeneeringUnitProperty).DisplayName;
                map.Add(propertyName, EngeneeringUnit);
            }
            return map;
        }
        #endregion
    }
}
