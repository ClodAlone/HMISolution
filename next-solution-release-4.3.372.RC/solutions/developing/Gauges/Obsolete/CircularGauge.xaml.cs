using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Converters;
using DevExpress.Xpf.Gauges;
using UFInterfaces;
using Utilities;
using Utilities.WPF;
using Gauges.Enums;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using Gauges.Automations;
using System.Globalization;
using OPCUAViewModel;
using Opc.Ua;
using ScreenSettings;
using WPFUtilities;
using WPFUtilities.Extensions;
using ViewModelLib;
using UFInterfaces.PropertyControl;
using System.Windows.Media.Effects;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;
using DynamicTagAwareHelper;
using System.Windows.Media.Imaging;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Threading;
using StringManager.ComponentService;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Gauges
{
    /// <summary>
    /// Interaction logic for CircularGauge.xaml
    /// </summary>
    /// 
    [Obsolete("Use the Gauges.GaugeControl.xaml insted of this.")]
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class CircularGauge : UserControl, IDisposable, IEntityReference, IDynamicTagAware, IContainPropertyEditors, IStringIDAware
    {
        #region Dependency Properties

        #region OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(CircularGauge));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(CircularGauge));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(CircularGauge));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(CircularGauge));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(CircularGauge));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(CircularGauge));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(CircularGauge));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(CircularGauge));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(CircularGauge));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(CircularGauge));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as CircularGauge;
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
            var control = sender as CircularGauge;
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
            var control = sender as CircularGauge;
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
            var control = sender as CircularGauge;
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
            var control = sender as CircularGauge;
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
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(CircularGauge), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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

        private void UpdateScaleStartEndValues()
        {
            if (arcScale != null)
            {
                arcScale.StartValue = _StartValue;
                arcScale.EndValue = _EndValue;
            }
        }

        private void UpdateScaleRanges()
        {

            if (ArcScaleRange1 != null)
            {
                ArcScaleRange1.StartValue = new RangeValue((_EndValue - _StartValue) * Range1StartValue / 100 + _StartValue);
                ArcScaleRange1.EndValue = new RangeValue((_EndValue - _StartValue) * Range1EndValue / 100 + _StartValue);
            }
            if (ArcScaleRange2 != null)
            {
                ArcScaleRange2.StartValue = new RangeValue((_EndValue - _StartValue) * Range2StartValue / 100 + _StartValue);
                ArcScaleRange2.EndValue = new RangeValue((_EndValue - _StartValue) * Range2EndValue / 100 + _StartValue);
            }
            if (ArcScaleRange3 != null)
            {
                ArcScaleRange3.StartValue = new RangeValue((_EndValue - _StartValue) * Range3StartValue / 100 + _StartValue);
                ArcScaleRange3.EndValue = new RangeValue((_EndValue - _StartValue) * Range3EndValue / 100 + _StartValue);
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(CircularGauge), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));

        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit && !bDatacontextChanging))
            {
                UpdateStartEndValues();
                UpdateScaleStartEndValues();
                UpdateScaleRanges();
            }
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
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(CircularGauge), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMinValueChanged), new CoerceValueCallback(OnCoerceTagMinValue)));

        private static object OnCoerceTagMinValue(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceTagMinValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(CircularGauge), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMaxValueChanged), new CoerceValueCallback(OnCoerceTagMaxValue)));

        private static object OnCoerceTagMaxValue(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceTagMaxValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueChanged), new CoerceValueCallback(OnCoerceShowValue)));

        private static object OnCoerceShowValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowValue((bool)value);
            else
                return value;
        }

        private static void OnShowValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnValueForegroundChanged), new CoerceValueCallback(OnCoerceValueForeground)));

        private static object OnCoerceValueForeground(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueForeground((Brush)value);
            else
                return value;
        }

        private static void OnValueForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty ValueFontSettingsProperty = DependencyProperty.Register("ValueFontSettings", typeof(FontSettings), typeof(CircularGauge), new UIPropertyMetadata(new FontSettings(FontWeights.DemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 48), new PropertyChangedCallback(OnValueFontSettingsChanged), new CoerceValueCallback(OnCoerceValueFontSettings)));

        private static object OnCoerceValueFontSettings(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnValueFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(CircularGauge), new UIPropertyMetadata(new Thickness(0,0,0,45), new PropertyChangedCallback(OnValueOffsetChanged), new CoerceValueCallback(OnCoerceValueOffset)));

        private static object OnCoerceValueOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueOffset((Thickness)value);
            else
                return value;
        }

        private static void OnValueOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(CircularGauge), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
                UpdateValue();
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
        #region EngeneeringUnitStyle
        #region ShowEngeneeringUnit
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceShowEngeneeringUnit)));

        private static object OnCoerceShowEngeneeringUnit(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowEngeneeringUnit((bool)value);
            else
                return value;
        }

        private static void OnShowEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty EngeneeringUnitForegroundProperty = DependencyProperty.Register("EngeneeringUnitForeground", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnEngeneeringUnitForegroundChanged), new CoerceValueCallback(OnCoerceEngeneeringUnitForeground)));

        private static object OnCoerceEngeneeringUnitForeground(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnitForeground((Brush)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(CircularGauge), new UIPropertyMetadata(new Thickness(0, 0, 0, 40), new PropertyChangedCallback(OnEngeneeringOffsetChanged), new CoerceValueCallback(OnCoerceEngeneeringOffset)));

        private static object OnCoerceEngeneeringOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringOffset((Thickness)value);
            else
                return value;
        }

        private static void OnEngeneeringOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty EngeneeringUnitProperty = DependencyProperty.Register("EngeneeringUnit", typeof(String), typeof(CircularGauge), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceEngeneeringUnit)));

        private static object OnCoerceEngeneeringUnit(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnit((String)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty EngeneeringUnitFontSettingsProperty = DependencyProperty.Register("EngeneeringUnitFontSettings", typeof(FontSettings), typeof(CircularGauge), new UIPropertyMetadata(new FontSettings(FontWeights.SemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 36), new PropertyChangedCallback(OnEngeneeringUnitFontSettingsChanged), new CoerceValueCallback(OnCoerceEngeneeringUnitFontSettings)));

        private static object OnCoerceEngeneeringUnitFontSettings(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringUnitFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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


        #endregion
        #region LabelStyle
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(ArcScaleLabelOrientation), typeof(CircularGauge), new UIPropertyMetadata(ArcScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLableOrientation((ArcScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("LabelStyle")]
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
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-56.0), new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelOffset((Double)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("LabelStyle")]
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
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLabelForegroundChanged), new CoerceValueCallback(OnCoerceLabelForeground)));

        private static object OnCoerceLabelForeground(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelForeground((Brush)value);
            else
                return value;
        }

        private static void OnLabelForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("LabelStyle")]
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
        public static readonly DependencyProperty LabelZIndexProperty = DependencyProperty.Register("LabelZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnLabelZIndexChanged), new CoerceValueCallback(OnCoerceLabelZIndex)));

        private static object OnCoerceLabelZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelZIndex((int)value);
            else
                return value;
        }

        private static void OnLabelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("LabelStyle")]
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
        public static readonly DependencyProperty LabelFontSettingsProperty = DependencyProperty.Register("LabelFontSettings", typeof(FontSettings), typeof(CircularGauge), new UIPropertyMetadata(new FontSettings(FontWeights.Medium, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnLabelFontSettingsChanged), new CoerceValueCallback(OnCoerceLabelFontSettings)));

        private static object OnCoerceLabelFontSettings(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnLabelFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("LabelStyle")]
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
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(CircularGauge), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("LabelStyle")]
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
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("LabelStyle")]
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("LabelStyle")]
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
        new public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("ScaleFlowDirection", typeof(FlowDirection), typeof(CircularGauge), new UIPropertyMetadata(FlowDirection.LeftToRight));
        [Category("LabelStyle")]
        [XmlIgnore]
        [Obsolete("No more supported")]
        [Browsable(false)]
        [SvgValueConverter(ConverterType = typeof(ConvertScaleFlowDirection), RequiredKey = true)]
        public FlowDirection ScaleFlowDirection
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
        #region GaugeStyle
        #region EnableBackGroundLayer
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit) && !newValue)
                    GaugeBaseModel = PredefinedBaseElementKinds.None;
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
        #region GaugeBaseModel
        public static readonly DependencyProperty GaugeBaseModelProperty = DependencyProperty.Register("GaugeBaseModel", typeof(PredefinedBaseElementKinds), typeof(CircularGauge), new UIPropertyMetadata(PredefinedBaseElementKinds.Eco, new PropertyChangedCallback(OnGaugeBaseModelChanged), new CoerceValueCallback(OnCoerceGaugeBaseModel)));

        private static object OnCoerceGaugeBaseModel(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceGaugeBaseModel((PredefinedBaseElementKinds)value);
            else
                return value;
        }

        private static void OnGaugeBaseModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnGaugeBaseModelChanged((PredefinedBaseElementKinds)e.OldValue, (PredefinedBaseElementKinds)e.NewValue);
        }

        protected virtual PredefinedBaseElementKinds OnCoerceGaugeBaseModel(PredefinedBaseElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGaugeBaseModelChanged(PredefinedBaseElementKinds oldValue, PredefinedBaseElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertGaugeBaseModel), RequiredKey = true)]
        public PredefinedBaseElementKinds GaugeBaseModel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedBaseElementKinds)GetValue(GaugeBaseModelProperty);
            }
            set
            {
                SetValue(GaugeBaseModelProperty, value);
            }
        }
        #endregion
        #region ArcScaleFill
        public static readonly DependencyProperty ArcScaleFillProperty = DependencyProperty.Register("ArcScaleFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xD6, 0xD4, 0xD4)), new PropertyChangedCallback(OnArcScaleFillChanged), new CoerceValueCallback(OnCoerceArcScaleFill)));

        private static object OnCoerceArcScaleFill(DependencyObject o, object value)
        {
            CircularGauge clock = o as CircularGauge;
            if (clock != null)
                return clock.OnCoerceArcScaleFill((Brush)value);
            else
                return value;
        }

        private static void OnArcScaleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge clock = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(130.0), new PropertyChangedCallback(OnStartAngleChanged), new CoerceValueCallback(OnCoerceStartAngle)));

        private static object OnCoerceStartAngle(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceStartAngle((Double)value);
            else
                return value;
        }

        private static void OnStartAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }


        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)410.0, new PropertyChangedCallback(OnEndAngleChanged), new CoerceValueCallback(OnCoerceEndAngle)));

        private static object OnCoerceEndAngle(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEndAngle((Double)value);
            else
                return value;
        }

        private static void OnEndAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout(); 
        }

        [Category("GaugeStyle")]
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


   
        #region MarkerVisible
        public static readonly DependencyProperty MarkerVisibleProperty = DependencyProperty.Register("MarkerVisible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerVisibleChanged), new CoerceValueCallback(OnCoerceMarkerVisible)));

        private static object OnCoerceMarkerVisible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerVisible((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty MarkerFactorHeightProperty = DependencyProperty.Register("MarkerFactorHeight", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)23, new PropertyChangedCallback(OnMarkerFactorHeightChanged), new CoerceValueCallback(OnCoerceMarkerFactorHeight)));

        private static object OnCoerceMarkerFactorHeight(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerFactorHeight((Double)value);
            else
                return value;
        }

        private static void OnMarkerFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerFactorHeightChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMarkerFactorHeight(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorHeightChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public Double MarkerFactorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MarkerFactorHeightProperty);
            }
            set
            {
                SetValue(MarkerFactorHeightProperty, value);
            }
        }
        #endregion
        #region MarkerFactorWidth
        public static readonly DependencyProperty MarkerFactorWidthProperty = DependencyProperty.Register("MarkerFactorWidth", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)23, new PropertyChangedCallback(OnMarkerFactorWidthChanged), new CoerceValueCallback(OnCoerceMarkerFactorWidth)));

        private static object OnCoerceMarkerFactorWidth(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerFactorWidth((Double)value);
            else
                return value;
        }

        private static void OnMarkerFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerFactorWidthChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMarkerFactorWidth(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorWidthChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
        public Double MarkerFactorWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MarkerFactorWidthProperty);
            }
            set
            {
                SetValue(MarkerFactorWidthProperty, value);
            }
        }
        #endregion
        #region MarkerZIndex
        public static readonly DependencyProperty MarkerZIndexProperty = DependencyProperty.Register("MarkerZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)100, new PropertyChangedCallback(OnMarkerZIndexChanged), new CoerceValueCallback(OnCoerceMarkerZIndex)));

        private static object OnCoerceMarkerZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerZIndex((int)value);
            else
                return value;
        }

        private static void OnMarkerZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMarkerZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-15.0), new PropertyChangedCallback(OnMarkerOffsetChanged), new CoerceValueCallback(OnCoerceMarkerOffset)));

        private static object OnCoerceMarkerOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerOffset((Double)value);
            else
                return value;
        }

        private static void OnMarkerOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMarkerOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
       

        [Category("GaugeStyle")]
        public Double MarkerOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(MarkerOffsetProperty);
            }
            set
            {
                SetValue(MarkerOffsetProperty, value);
            }
        }
        #endregion
        #region MarkerFill
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnMarkerFillChanged), new CoerceValueCallback(OnCoerceMarkerFill)));

        private static object OnCoerceMarkerFill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerFill((Brush)value);
            else
                return value;
        }

        private static void OnMarkerFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMarkerFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well. 
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkRed), new PropertyChangedCallback(OnMarkerStrokeChanged), new CoerceValueCallback(OnCoerceMarkerStroke)));

        private static object OnCoerceMarkerStroke(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceMarkerStroke((Brush)value);
            else
                return value;
        }

        private static void OnMarkerStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty MarkerAnimationEnableProperty = DependencyProperty.Register("MarkerAnimationEnable", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerAnimationEnableChanged), new CoerceValueCallback(OnCoerceMarkerAnimationEnable)));

        private static object OnCoerceMarkerAnimationEnable(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeStyle")]
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
        public static readonly DependencyProperty MarkerIsInteractiveProperty = DependencyProperty.Register("MarkerIsInteractive", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnMarkerIsInteractiveChanged), new CoerceValueCallback(OnCoerceMarkerIsInteractive)));

        private static object OnCoerceMarkerIsInteractive(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMarkerIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMarkerIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceMarkerIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
      
        [Category("GaugeStyle")]
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

#endregion
        #region GaugeTickmarkStyle
        #region TickmarksPresentation
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(CircularGauge), new UIPropertyMetadata(PredefinedElementKinds.Progressive, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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


        #region LineFill
        public static readonly DependencyProperty LineFillProperty = DependencyProperty.Register("LineFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnLineFillChanged), new CoerceValueCallback(OnCoerceLineFill)));

        private static object OnCoerceLineFill(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceLineFill((Brush)value);
            else
                return value;
        }

        private static void OnLineFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                control.OnLineFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceLineFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLineFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeTickmarkStyle")]
        public Brush LineFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LineFillProperty);
            }
            set
            {
                SetValue(LineFillProperty, value);
            }
        }

        #endregion
        

        #region MajorTickmarkFactorLength
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-27.0), new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        #region MajorTickmarkFill
        public static readonly DependencyProperty MajorTickmarkFillProperty = DependencyProperty.Register("MajorTickmarkFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMajorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFill)));

        private static object OnCoerceMajorTickmarkFill(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceMajorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
         [Category("GaugeTickmarkStyle")]
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
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-27.0), new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
        #region MinorTickmarkFill
        public static readonly DependencyProperty MinorTickmarkFillProperty = DependencyProperty.Register("MinorTickmarkFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMinorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFill)));

        private static object OnCoerceMinorTickmarkFill(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceMinorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        [Category("GaugeTickmarkStyle")]
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
        #region SpindleStyle

        #region SpindleCapPresentation
        public static readonly DependencyProperty SpindleCapPresentationProperty = DependencyProperty.Register("SpindleCapPresentation", typeof(PredefinedElementKinds), typeof(CircularGauge), new UIPropertyMetadata(PredefinedElementKinds.Eco, new PropertyChangedCallback(OnSpindleCapPresentationChanged), new CoerceValueCallback(OnCoerceSpindleCapPresentation)));

        private static object OnCoerceSpindleCapPresentation(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleCapPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnSpindleCapPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }


        [Category("GaugeSpindleStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpindleCapPresentation), RequiredKey = true)]
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
        #region SpindleFill
        public static readonly DependencyProperty SpindleFillProperty = DependencyProperty.Register("SpindleFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnSpindleFillChanged), new CoerceValueCallback(OnCoerceSpindleFill)));

        private static object OnCoerceSpindleFill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleFill((Brush)value);
            else
                return value;
        }

        private static void OnSpindleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnSpindleFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSpindleFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
         [Category("GaugeSpindleStyle")]
        public Brush SpindleFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SpindleFillProperty);
            }
            set
            {
                SetValue(SpindleFillProperty, value);
            }
        }

        #endregion

        #region SpindleFactorHeight
        public static readonly DependencyProperty SpindleFactorHeightProperty = DependencyProperty.Register("SpindleFactorHeight", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnSpindleFactorHeightChanged), new CoerceValueCallback(OnCoerceSpindleFactorHeight)));

        private static object OnCoerceSpindleFactorHeight(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleFactorHeight((Double)value);
            else
                return value;
        }

        private static void OnSpindleFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnSpindleFactorHeightChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceSpindleFactorHeight(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleFactorHeightChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeSpindleStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpindleFactor), RequiredKey = true)]
        public Double SpindleFactorHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(SpindleFactorHeightProperty);
            }
            set
            {
                SetValue(SpindleFactorHeightProperty, value);
            }
        }
        #endregion
        #region SpindleFactorWidth
        public static readonly DependencyProperty SpindleFactorWidthProperty = DependencyProperty.Register("SpindleFactorWidth", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnSpindleFactorWidthChanged), new CoerceValueCallback(OnCoerceSpindleFactorWidth)));

        private static object OnCoerceSpindleFactorWidth(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleFactorWidth((Double)value);
            else
                return value;
        }

        private static void OnSpindleFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnSpindleFactorWidthChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceSpindleFactorWidth(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpindleFactorWidthChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeSpindleStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpindleFactor), RequiredKey = true)]
        public Double SpindleFactorWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(SpindleFactorWidthProperty);
            }
            set
            {
                SetValue(SpindleFactorWidthProperty, value);
            }
        }
        #endregion
        #region SpindleCapZIndex
        public static readonly DependencyProperty SpindleCapZIndexProperty = DependencyProperty.Register("SpindleCapZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnSpindleCapZIndexChanged), new CoerceValueCallback(OnCoerceSpindleCapZIndex)));

        private static object OnCoerceSpindleCapZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceSpindleCapZIndex((int)value);
            else
                return value;
        }

        private static void OnSpindleCapZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeSpindleStyle")]
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
        #region NeedlePresentation
        public static readonly DependencyProperty NeedlePresentationProperty = DependencyProperty.Register("NeedlePresentation", typeof(PredefinedElementKinds), typeof(CircularGauge), new UIPropertyMetadata(PredefinedElementKinds.Default, new PropertyChangedCallback(OnNeedlePresentationChanged), new CoerceValueCallback(OnCoerceNeedlePresentation)));

        private static object OnCoerceNeedlePresentation(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedlePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnNeedlePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedlePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
        }

        protected virtual PredefinedElementKinds OnCoerceNeedlePresentation(PredefinedElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedlePresentationChanged(PredefinedElementKinds oldValue, PredefinedElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }


        [Category("GaugeNeedleStyle")]
        public PredefinedElementKinds NeedlePresentation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedElementKinds)GetValue(NeedlePresentationProperty);
            }
            set
            {
                SetValue(NeedlePresentationProperty, value);
            }
        }

        #endregion

        #region NeedleFill
        public static readonly DependencyProperty NeedleFillProperty = DependencyProperty.Register("NeedleFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnNeedleFillChanged), new CoerceValueCallback(OnCoerceNeedleFill)));

        private static object OnCoerceNeedleFill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleFill((Brush)value);
            else
                return value;
        }

        private static void OnNeedleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedleFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceNeedleFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeNeedleStyle")]
        public Brush NeedleFill
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleFillProperty);
            }
            set
            {
                SetValue(NeedleFillProperty, value);
            }
        }

        #endregion
        #region NeedleIsInteractive
        public static readonly DependencyProperty NeedleIsInteractiveProperty = DependencyProperty.Register("NeedleIsInteractive", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnNeedleIsInteractiveChanged), new CoerceValueCallback(OnCoerceNeedleIsInteractive)));

        private static object OnCoerceNeedleIsInteractive(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedleIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
        public Boolean NeedleIsInteractive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleIsInteractiveProperty);
            }
            set
            {
                SetValue(NeedleIsInteractiveProperty, value);
            }
        }

        #endregion
        #region NeedleVisible
        public static readonly DependencyProperty NeedleVisibleProperty = DependencyProperty.Register("NeedleVisible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleVisibleChanged), new CoerceValueCallback(OnCoerceNeedleVisible)));

        private static object OnCoerceNeedleVisible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleVisible((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedleVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceNeedleVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
        public Boolean NeedleVisible
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(NeedleVisibleProperty);
            }
            set
            {
                SetValue(NeedleVisibleProperty, value);
            }
        }

        #endregion
        #region NeedleAnimationEnable
        public static readonly DependencyProperty NeedleAnimationEnableProperty = DependencyProperty.Register("NeedleAnimationEnable", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleAnimationEnableChanged), new CoerceValueCallback(OnCoerceNeedleAnimationEnable)));

        private static object OnCoerceNeedleAnimationEnable(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
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
        public static readonly DependencyProperty NeedleZIndexProperty = DependencyProperty.Register("NeedleZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)500, new PropertyChangedCallback(OnNeedleZIndexChanged), new CoerceValueCallback(OnCoerceNeedleZIndex)));

        private static object OnCoerceNeedleZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleZIndex((int)value);
            else
                return value;
        }

        private static void OnNeedleZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
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
        #region NeedleStartOffset
        public static readonly DependencyProperty NeedleStartOffsetProperty = DependencyProperty.Register("NeedleStartOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnNeedleStartOffsetChanged), new CoerceValueCallback(OnCoerceNeedleStartOffset)));

        private static object OnCoerceNeedleStartOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleStartOffset((Double)value);
            else
                return value;
        }

        private static void OnNeedleStartOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedleStartOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceNeedleStartOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleStartOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
        public Double NeedleStartOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(NeedleStartOffsetProperty);
            }
            set
            {
                SetValue(NeedleStartOffsetProperty, value);
            }
        }
        #endregion
        #region NeedleEndOffset
        public static readonly DependencyProperty NeedleEndOffsetProperty = DependencyProperty.Register("NeedleEndOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)37.0, new PropertyChangedCallback(OnNeedleEndOffsetChanged), new CoerceValueCallback(OnCoerceNeedleEndOffset)));

        private static object OnCoerceNeedleEndOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceNeedleEndOffset((Double)value);
            else
                return value;
        }

        private static void OnNeedleEndOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnNeedleEndOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceNeedleEndOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNeedleEndOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeNeedleStyle")]
        public Double NeedleEndOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(NeedleEndOffsetProperty);
            }
            set
            {
                SetValue(NeedleEndOffsetProperty, value);
            }
        }
        #endregion
#endregion
        #region RangeStyle

        #region RangeBarVisible
        public static readonly DependencyProperty RangeBarVisibleProperty = DependencyProperty.Register("RangeBarVisible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarVisibleChanged), new CoerceValueCallback(OnCoerceRangeBarVisible)));

        private static object OnCoerceRangeBarVisible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarVisible((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarVisible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarVisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
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
        public static readonly DependencyProperty RangeBarZIndexProperty = DependencyProperty.Register("RangeBarZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnRangeBarZIndexChanged), new CoerceValueCallback(OnCoerceRangeBarZIndex)));

        private static object OnCoerceRangeBarZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarZIndex((int)value);
            else
                return value;
        }

        private static void OnRangeBarZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRangeBarZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
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
        public static readonly DependencyProperty RangeBarOffsetProperty = DependencyProperty.Register("RangeBarOffset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-90.0), new PropertyChangedCallback(OnRangeBarOffsetChanged), new CoerceValueCallback(OnCoerceRangeBarOffset)));

        private static object OnCoerceRangeBarOffset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarOffset((Double)value);
            else
                return value;
        }

        private static void OnRangeBarOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRangeBarOffset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarOffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
        public Double RangeBarOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Double)GetValue(RangeBarOffsetProperty);
            }
            set
            {
                SetValue(RangeBarOffsetProperty, value);
            }
        }
        #endregion
        #region RangeBarFill
        public static readonly DependencyProperty RangeBarFillProperty = DependencyProperty.Register("RangeBarFill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRangeBarFillChanged), new CoerceValueCallback(OnCoerceRangeBarFill)));

        private static object OnCoerceRangeBarFill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarFill((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRangeBarFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeRangeStyle")]
        [SvgValueConverter(typeof(ConvertBrushToSvgValue), RequiredKey = true, NeedSVGUrlBrushes = true)]
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
        public static readonly DependencyProperty RangeBarBackgroundProperty = DependencyProperty.Register("RangeBarBackground", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRangeBarBackgroundChanged), new CoerceValueCallback(OnCoerceRangeBarBackground)));

        private static object OnCoerceRangeBarBackground(DependencyObject o, object value)
        {
            CircularGauge control = o as CircularGauge;
            if (control != null)
                return control.OnCoerceRangeBarBackground((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge control = o as CircularGauge;
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
        public static readonly DependencyProperty RangeBarAnimationEnableProperty = DependencyProperty.Register("RangeBarAnimationEnable", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarAnimationEnableChanged), new CoerceValueCallback(OnCoerceRangeBarAnimationEnable)));

        private static object OnCoerceRangeBarAnimationEnable(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarAnimationEnable(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarAnimationEnableChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
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
        public static readonly DependencyProperty RangeBarIsInteractiveProperty = DependencyProperty.Register("RangeBarIsInteractive", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarIsInteractiveChanged), new CoerceValueCallback(OnCoerceRangeBarIsInteractive)));

        private static object OnCoerceRangeBarIsInteractive(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRangeBarIsInteractive(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarIsInteractiveChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
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
        public static readonly DependencyProperty RangeBarThicknessProperty = DependencyProperty.Register("RangeBarThickness", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRangeBarThicknessChanged), new CoerceValueCallback(OnCoerceRangeBarThickness)));

        private static object OnCoerceRangeBarThickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRangeBarThickness((int)value);
            else
                return value;
        }

        private static void OnRangeBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRangeBarThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRangeBarThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRangeBarThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeStyle")]
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

        #region RangeOptions


        #region Range1Visible
        public static readonly DependencyProperty Range1VisibleProperty = DependencyProperty.Register("Range1Visible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange1VisibleChanged), new CoerceValueCallback(OnCoerceRange1Visible)));

        private static object OnCoerceRange1Visible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange1VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange1Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1StartValueProperty = DependencyProperty.Register("Range1StartValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)40.0, new PropertyChangedCallback(OnRange1StartValueChanged), new CoerceValueCallback(OnCoerceRange1StartValue)));

        private static object OnCoerceRange1StartValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange1StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1EndValueProperty = DependencyProperty.Register("Range1EndValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)60.0, new PropertyChangedCallback(OnRange1EndValueChanged), new CoerceValueCallback(OnCoerceRange1EndValue)));

        private static object OnCoerceRange1EndValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange1EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1ThicknessProperty = DependencyProperty.Register("Range1Thickness", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange1ThicknessChanged), new CoerceValueCallback(OnCoerceRange1Thickness)));

        private static object OnCoerceRange1Thickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1Thickness((int)value);
            else
                return value;
        }

        private static void OnRange1ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange1Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1OffsetProperty = DependencyProperty.Register("Range1Offset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange1OffsetChanged), new CoerceValueCallback(OnCoerceRange1Offset)));

        private static object OnCoerceRange1Offset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1Offset((Double)value);
            else
                return value;
        }

        private static void OnRange1OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange1Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1FillProperty = DependencyProperty.Register("Range1Fill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Green), new PropertyChangedCallback(OnRange1FillChanged), new CoerceValueCallback(OnCoerceRange1Fill)));

        private static object OnCoerceRange1Fill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange1FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange1Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range1ZIndexProperty = DependencyProperty.Register("Range1ZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange1ZIndexChanged), new CoerceValueCallback(OnCoerceRange1ZIndex)));

        private static object OnCoerceRange1ZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange1ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange1ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange1ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange1ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange1ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2VisibleProperty = DependencyProperty.Register("Range2Visible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange2VisibleChanged), new CoerceValueCallback(OnCoerceRange2Visible)));

        private static object OnCoerceRange2Visible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange2VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange2Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2StartValueProperty = DependencyProperty.Register("Range2StartValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)60.0, new PropertyChangedCallback(OnRange2StartValueChanged), new CoerceValueCallback(OnCoerceRange2StartValue)));

        private static object OnCoerceRange2StartValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange2StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2EndValueProperty = DependencyProperty.Register("Range2EndValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)80.0, new PropertyChangedCallback(OnRange2EndValueChanged), new CoerceValueCallback(OnCoerceRange2EndValue)));

        private static object OnCoerceRange2EndValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange2EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2ThicknessProperty = DependencyProperty.Register("Range2Thickness", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange2ThicknessChanged), new CoerceValueCallback(OnCoerceRange2Thickness)));

        private static object OnCoerceRange2Thickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2Thickness((int)value);
            else
                return value;
        }

        private static void OnRange2ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange2Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2OffsetProperty = DependencyProperty.Register("Range2Offset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange2OffsetChanged), new CoerceValueCallback(OnCoerceRange2Offset)));

        private static object OnCoerceRange2Offset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2Offset((Double)value);
            else
                return value;
        }

        private static void OnRange2OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange2Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2FillProperty = DependencyProperty.Register("Range2Fill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xEB, 0x79, 0x3C)), new PropertyChangedCallback(OnRange2FillChanged), new CoerceValueCallback(OnCoerceRange2Fill)));

        private static object OnCoerceRange2Fill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange2FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange2Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range2ZIndexProperty = DependencyProperty.Register("Range2ZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange2ZIndexChanged), new CoerceValueCallback(OnCoerceRange2ZIndex)));

        private static object OnCoerceRange2ZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange2ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange2ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange2ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange2ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange2ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3VisibleProperty = DependencyProperty.Register("Range3Visible", typeof(Boolean), typeof(CircularGauge), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange3VisibleChanged), new CoerceValueCallback(OnCoerceRange3Visible)));

        private static object OnCoerceRange3Visible(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange3VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceRange3Visible(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3VisibleChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3StartValueProperty = DependencyProperty.Register("Range3StartValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)80.0, new PropertyChangedCallback(OnRange3StartValueChanged), new CoerceValueCallback(OnCoerceRange3StartValue)));

        private static object OnCoerceRange3StartValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange3StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3StartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3StartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3EndValueProperty = DependencyProperty.Register("Range3EndValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)100.0, new PropertyChangedCallback(OnRange3EndValueChanged), new CoerceValueCallback(OnCoerceRange3EndValue)));

        private static object OnCoerceRange3EndValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange3EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3EndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3EndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3ThicknessProperty = DependencyProperty.Register("Range3Thickness", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange3ThicknessChanged), new CoerceValueCallback(OnCoerceRange3Thickness)));

        private static object OnCoerceRange3Thickness(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3Thickness((int)value);
            else
                return value;
        }

        private static void OnRange3ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3ThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange3Thickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3ThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3OffsetProperty = DependencyProperty.Register("Range3Offset", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange3OffsetChanged), new CoerceValueCallback(OnCoerceRange3Offset)));

        private static object OnCoerceRange3Offset(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3Offset((Double)value);
            else
                return value;
        }

        private static void OnRange3OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceRange3Offset(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3OffsetChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3FillProperty = DependencyProperty.Register("Range3Fill", typeof(Brush), typeof(CircularGauge), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRange3FillChanged), new CoerceValueCallback(OnCoerceRange3Fill)));

        private static object OnCoerceRange3Fill(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange3FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRange3Fill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3FillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }
        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty Range3ZIndexProperty = DependencyProperty.Register("Range3ZIndex", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange3ZIndexChanged), new CoerceValueCallback(OnCoerceRange3ZIndex)));

        private static object OnCoerceRange3ZIndex(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceRange3ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange3ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnRange3ZIndexChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceRange3ZIndex(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRange3ZIndexChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateGaugeLayout();
        }

        [Category("GaugeRangeOptions")]
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
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnStartValueChanged), new CoerceValueCallback(OnCoerceStartValue)));

        private static object OnCoerceStartValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceStartValue((Double)value);
            else
                return value;
        }

        private static void OnStartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnStartValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceStartValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStartValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                MinValue = newValue;
            }
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
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register("EndValue", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnEndValueChanged), new CoerceValueCallback(OnCoerceEndValue)));

        private static object OnCoerceEndValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceEndValue((Double)value);
            else
                return value;
        }

        private static void OnEndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnEndValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceEndValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEndValueChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                MaxValue = newValue;
            }
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
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMajorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(CircularGauge), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnMinorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(CircularGauge), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                return circularGauge.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CircularGauge circularGauge = o as CircularGauge;
            if (circularGauge != null)
                circularGauge.OnValueChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceValue(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool isChangingValue;
        bool isUserMouseDownAction;
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


        #region ThumbFactor
        public static readonly DependencyProperty GaugeTypeProperty = DependencyProperty.Register("GaugeType", typeof(string), typeof(CircularGauge), new UIPropertyMetadata("Full"));
        [SvgValueConverter(ConverterType = typeof(ConvertGaugeType), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string GaugeType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(GaugeTypeProperty);
            }
            set
            {
                SetValue(GaugeTypeProperty, value);
            }
        }
        #endregion
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool IsFastGaugeControl { get { return false; } }
        [SvgValueConverter(ConverterType = typeof(ConvertScaleOptions), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string ScaleOptions { get { return null; } }
        #endregion

        #region Declarations
        List<string> matchChangedMap = new List<string>();
        IStringEditorManager stringManager;
        bool bDatacontextChanging;
        Brush defLabelForeground = new SolidColorBrush(Colors.Black);
        ScreenDocument Document;
        MonitoredItemViewModel monitoredItemViewModel;

        CancellationTokenSource cts;

        DataValue lastDataValue;
        DispatcherOperation dpUpdateWarning;
        object lockObject = new object();

        bool bLoaded = false;
        internal int CustomBaseIndex
        {
            get { return 12; }
        }

        private OPCUAEntityReference mintag;
        private OPCUAEntityReference maxtag;
        TypeHelper typeHelper = new TypeHelper();

        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        bool bInit;
        bool bDesign;
        bool bPrepareExecuted;
        bool bTerminateExecuted;
        double _StartValue;
        double _EndValue;
        string ConverterLabel;
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InvokeAutomationPeer(this);
        }

        public void CircularGaugeInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion
        
        #region Constructor
        public CircularGauge()
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
                    UpdateGaugeLayout();
                    bInit = true;
                    if (!bDesign)
                        InitControl();
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

                    UpdateRanges();
                }
            };
        }
        private void UpdateRanges()
        {
            double startValue = MinValue;
            double endValue = MaxValue;
            Action action = () =>
            {
                if (UseEUnit && monitoredItemViewModel != null)
                    if (monitoredItemViewModel.HasRange)
                    {
                        startValue = monitoredItemViewModel.Range.Low;
                        endValue = monitoredItemViewModel.Range.High;
                    }

                _StartValue = startValue;
                _EndValue = endValue;

                bDatacontextChanging = false;
                if (bInit)
                {
                    UpdateStartEndValues();
                    if (arcScale == null)
                        UpdateGaugeLayout();
                    UpdateScaleStartEndValues();
                    UpdateScaleRanges();
                }
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
                try
                {
                    UpdateCustomValue(Value);
                    if (arcScale != null)
                    {
                        arcScale.LabelOptions.FormatString = "";
                        arcScale.LabelOptions.FormatString = LabelStringFormat;
                    }
                }
                catch (Exception)
                {
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
                    dpUpdateWarning = Dispatcher.BeginInvokeAsynchronously(this, () =>
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
       
        #endregion

        #region DynObjects
        CircularGaugeControl circularGaugeObject;
        ArcScale arcBaseScale;
        ArcScale arcScale;
        ArcScaleRange ArcScaleRange1;
        ArcScaleRange ArcScaleRange2;
        ArcScaleRange ArcScaleRange3;
        ArcScaleMarker arcScaleMarker;
        internal ArcScaleLayer arcLayer;
        ArcScaleNeedle arcNeedle;
        ArcScaleRangeBar arcRangeBackground;
        ArcScaleRangeBar arcRange;
        ScaleCustomElement gridValueContainer;
        ScaleCustomElement gridUnitContainer;
        ContentControl BackContent;
        Image WarningMarker;
        #endregion

        #region Methods

        internal string LoadSVGBackContent()
        {
            return string.Format("{0}_{1}", GaugeBaseModel.ToString(), GetArcScaleBackgroundIndex());
        }

        internal string LoadSVGGaugeType()
        {
            return string.Format("{0}", GetArcScaleBackgroundIndex());
        }

        void UpdateStartEndValues()
        {
            if (bDesign)
            {
                _StartValue = UseEUnit ? 0.0 : MinValue;
                _EndValue = UseEUnit ? 100.0 : MaxValue;
            }
        }
        private void UpdateGaugeLayout()
        {
            if (bDispose || bDatacontextChanging)
                return;

            UpdateStartEndValues();

            UpdateBackBorder();
            UpdateGuageControl();
            UpdateBaseArcScale();
            UpdateArcScale();
            UpdateArcLayer();
            UpdateNeedle();
            UpdateArcScaleRange1();
            UpdateArcScaleRange2();
            UpdateArcScaleRange3();
            UpdateMarkers();
            UpdateRangeBars();
            UpdateCustomElements();
            UpdateValue();
        }

        private void UpdateCustomElements()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
            try
            {
                string meas = ConverterLabel ?? EngeneeringUnit;
                if (!ShowEngeneeringUnit || string.IsNullOrEmpty(meas))
                {
                    if (gridUnitContainer != null)
                    {
                        if (arcScale.CustomElements.Contains(gridUnitContainer))
                            arcScale.CustomElements.Remove(gridUnitContainer);

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
                            ZIndex = 300
                        };

                        Grid _grid = new Grid();
                        TextBlock _text = (TextBlock)LoadTemplate("engeneeringUnit");
                        var offsetBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringOffset"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        _grid.SetBinding(Grid.MarginProperty, offsetBinding);
                        if(_text != null)
                            _grid.Children.Add(_text);
                        gridUnitContainer.Content = _grid;

                        if (!arcScale.CustomElements.Contains(gridUnitContainer))
                            arcScale.CustomElements.Add(gridUnitContainer);
                    }
                    Grid grid = (Grid)gridUnitContainer.Content;
                    TextBlock text = grid.GetChildrenOfType<TextBlock>().FirstOrDefault();
                    if (text != null)
                        text.Text = meas;
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
                        if (arcScale.CustomElements.Contains(gridValueContainer))
                            arcScale.CustomElements.Remove(gridValueContainer);

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
                            ZIndex = 300
                        };

                        Grid _grid = new Grid();
                        TextBlock _text = (TextBlock)LoadTemplate("value");

                        var offsetBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueOffset"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        _grid.SetBinding(Grid.MarginProperty, offsetBinding);

                        if(_text != null)
                            _grid.Children.Add(_text);
                        gridValueContainer.Content = _grid;

                        if (!arcScale.CustomElements.Contains(gridValueContainer))
                            arcScale.CustomElements.Add(gridValueContainer);
                        if (bInit)
                            UpdateValue();
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
            if (circularGaugeObject == null || arcScale == null || arcScale == null)
                return;
            bool updateValue = false;
            try
            {
                /* Background Arc */
                if (RangeBarBackground == null ||
                    RangeBarBackground == transparent)
                {
                    if (arcRangeBackground != null)
                    {
                        if (arcBaseScale.RangeBars.Contains(arcRangeBackground))
                            arcBaseScale.RangeBars.Remove(arcRangeBackground);

                        arcRangeBackground.Presentation = null;
                        arcRangeBackground.Options = null;
                        arcRangeBackground.Animation = null;
                        arcRangeBackground = null;
                    }
                }
                else
                {
                    if (arcRangeBackground == null)
                    {
                        arcRangeBackground = new ArcScaleRangeBar();
                        arcRangeBackground.Options = new ArcScaleRangeBarOptions();
                        arcRangeBackground.Presentation = new DefaultArcScaleRangeBarPresentation();

                        if (!arcBaseScale.RangeBars.Contains(arcRangeBackground))
                            arcBaseScale.RangeBars.Add(arcRangeBackground);
                    }

                    (arcRangeBackground.Presentation as PredefinedArcScaleRangeBarPresentation).Fill = RangeBarBackground;
                    arcRangeBackground.Options.Thickness = RangeBarThickness;
                    arcRangeBackground.Options.ZIndex = RangeBarZIndex - 1;
                    arcRangeBackground.Options.Offset = RangeBarOffset;
                    arcRangeBackground.IsInteractive = false;
                    arcRangeBackground.Animation = null;
                    updateValue = true;
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
                    if (arcRange != null)
                    {
                        if (arcScale.RangeBars.Contains(arcRange))
                            arcScale.RangeBars.Remove(arcRange);

                        arcRange.ValueChanged -= arcNeedle_ValueChanged;

                        arcRange.Presentation = null;
                        arcRange.Options = null;
                        arcRange.Animation = null;
                        arcRange = null;
                    }
                }
                else
                {
                    if (arcRange == null)
                    {
                        arcRange = new ArcScaleRangeBar();
                        arcRange.Options = new ArcScaleRangeBarOptions();
                        arcRange.Presentation = new DefaultArcScaleRangeBarPresentation();

                        if (!arcScale.RangeBars.Contains(arcRange))
                            arcScale.RangeBars.Add(arcRange);

                        if (!bDesign)
                        {
                            arcRange.ValueChanged += arcNeedle_ValueChanged;
                        }
                    }

                    (arcRange.Presentation as PredefinedArcScaleRangeBarPresentation).Fill = RangeBarFill;
                    arcRange.Options.Thickness = RangeBarThickness;
                    arcRange.Options.ZIndex = RangeBarZIndex;
                    arcRange.Options.Offset = RangeBarOffset;
                    arcRange.IsInteractive = RangeBarIsInteractive;
                    if (!bDesign)
                    {
                        if (arcRange.Animation == null)
                            arcRange.Animation = new IndicatorAnimation() { Enable = RangeBarAnimationEnable };
                        else
                            (arcRange.Animation as IndicatorAnimation).Enable = RangeBarAnimationEnable;
                    }
                    else
                        arcRange.Animation = null;
                    updateValue = true;
                }
            }
            catch (Exception)
            {
            }

            if (updateValue && bInit)
                UpdateValue();

        }

        private void UpdateMarkers()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (MarkerVisible)
                {
                    if (arcScaleMarker == null)
                    {
                        arcScaleMarker = new ArcScaleMarker();
                        arcScaleMarker.Options = new ArcScaleMarkerOptions();
                        arcScaleMarker.Presentation = new CustomArcScaleMarkerPresentation();
                        var presentation = (arcScaleMarker.Presentation as CustomArcScaleMarkerPresentation);
                        presentation.MarkerTemplate = LoadMarkerTemplate();


                        if (!arcScale.Markers.Contains(arcScaleMarker))
                            arcScale.Markers.Add(arcScaleMarker);

                        if (!bDesign)
                        {
                            arcScaleMarker.ValueChanged += arcNeedle_ValueChanged;
                        }
                    }

                    arcScaleMarker.Options.ZIndex = MarkerZIndex;
                    arcScaleMarker.Options.Offset = MarkerOffset;
                    arcScaleMarker.IsInteractive = MarkerIsInteractive;
                    if (!bDesign)
                    {
                        if (arcScaleMarker.Animation == null)
                            arcScaleMarker.Animation = new IndicatorAnimation() { Enable = MarkerAnimationEnable };
                        else
                            (arcScaleMarker.Animation as IndicatorAnimation).Enable = MarkerAnimationEnable;
                    }
                    else
                        arcScaleMarker.Animation = null;
                }
                else
                {
                    if (arcScaleMarker != null)
                    {
                        arcScaleMarker.ValueChanged -= arcNeedle_ValueChanged;

                        arcScale.Markers.Clear();
                        arcScaleMarker.Presentation = null;
                        arcScaleMarker.Options = null;
                        arcScaleMarker.Animation = null;
                        arcScaleMarker = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateArcScaleRange1()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (Range1Visible)
                {
                    if (ArcScaleRange1 == null)
                    {
                        ArcScaleRange1 = new ArcScaleRange();
                        ArcScaleRange1.Options = new RangeOptions();
                        ArcScaleRange1.Presentation = new DefaultArcScaleRangePresentation();

                        if (!arcScale.Ranges.Contains(ArcScaleRange1))
                            arcScale.Ranges.Add(ArcScaleRange1);
                    }

                                (ArcScaleRange1.Presentation as PredefinedArcScaleRangePresentation).Fill = Range1Fill;
                    ArcScaleRange1.StartValue = new RangeValue((_EndValue - _StartValue) * Range1StartValue / 100 + _StartValue);
                    ArcScaleRange1.EndValue = new RangeValue((_EndValue - _StartValue) * Range1EndValue / 100 + _StartValue);

                    ArcScaleRange1.Options.Thickness = Range1Thickness;
                    ArcScaleRange1.Options.Offset = Range1Offset;
                    ArcScaleRange1.Options.ZIndex = Range1ZIndex;
                }
                else
                {
                    if (ArcScaleRange1 != null)
                    {
                        if (arcScale.Ranges.Contains(ArcScaleRange1))
                            arcScale.Ranges.Remove(ArcScaleRange1);
                        ArcScaleRange1.Presentation = null;
                        ArcScaleRange1.Options = null;
                        ArcScaleRange1 = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateArcScaleRange2()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            try
            {
                if (Range2Visible)
                {
                    if (ArcScaleRange2 == null)
                    {
                        ArcScaleRange2 = new ArcScaleRange();
                        ArcScaleRange2.Options = new RangeOptions();
                        ArcScaleRange2.Presentation = new DefaultArcScaleRangePresentation();

                        if (!arcScale.Ranges.Contains(ArcScaleRange2))
                            arcScale.Ranges.Add(ArcScaleRange2);
                    }

                                (ArcScaleRange2.Presentation as PredefinedArcScaleRangePresentation).Fill = Range2Fill;
                    ArcScaleRange2.StartValue = new RangeValue((_EndValue - _StartValue) * Range2StartValue / 100 + _StartValue);
                    ArcScaleRange2.EndValue = new RangeValue((_EndValue - _StartValue) * Range2EndValue / 100 + _StartValue);
                    ArcScaleRange2.Options.Thickness = Range2Thickness;
                    ArcScaleRange2.Options.Offset = Range2Offset;
                    ArcScaleRange2.Options.ZIndex = Range2ZIndex;
                }
                else
                {
                    if (ArcScaleRange2 != null)
                    {
                        if (arcScale.Ranges.Contains(ArcScaleRange2))
                            arcScale.Ranges.Remove(ArcScaleRange2);
                        ArcScaleRange2.Presentation = null;
                        ArcScaleRange2.Options = null;
                        ArcScaleRange2 = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void UpdateArcScaleRange3()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
            try
            {
                if (Range3Visible)
                {
                    if (ArcScaleRange3 == null)
                    {
                        ArcScaleRange3 = new ArcScaleRange();
                        ArcScaleRange3.Options = new RangeOptions();
                        ArcScaleRange3.Presentation = new DefaultArcScaleRangePresentation(); 

                        if (!arcScale.Ranges.Contains(ArcScaleRange3))
                            arcScale.Ranges.Add(ArcScaleRange3);
                    }

                    (ArcScaleRange3.Presentation as PredefinedArcScaleRangePresentation).Fill = Range3Fill;
                    ArcScaleRange3.StartValue = new RangeValue((_EndValue - _StartValue) * Range3StartValue / 100 + _StartValue);
                    ArcScaleRange3.EndValue = new RangeValue((_EndValue - _StartValue) * Range3EndValue / 100 + _StartValue);
                    ArcScaleRange3.Options.Thickness = Range3Thickness;
                    ArcScaleRange3.Options.Offset = Range3Offset;
                    ArcScaleRange3.Options.ZIndex = Range3ZIndex;
                }
                else
                {
                    if (ArcScaleRange3 != null)
                    {
                        if (arcScale.Ranges.Contains(ArcScaleRange3))
                            arcScale.Ranges.Remove(ArcScaleRange3);
                        ArcScaleRange3.Presentation = null;
                        ArcScaleRange3.Options = null;
                        ArcScaleRange3 = null;
                    }
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
                if(NeedleVisible)
                {
                    if (arcNeedle == null)
                    {
                        arcNeedle = new ArcScaleNeedle();
                        arcNeedle.Options = new ArcScaleNeedleOptions();

                        if (!arcScale.Needles.Contains(arcNeedle))
                            arcScale.Needles.Add(arcNeedle);

                        if (!bDesign)
                        {
                            arcNeedle.ValueChanged += arcNeedle_ValueChanged;
                        }
                    }

                    arcNeedle.Presentation = GetNeedlePresentation(NeedlePresentation);
                    (arcNeedle.Presentation as PredefinedArcScaleNeedlePresentation).Fill = NeedleFill;
                    arcNeedle.IsInteractive = NeedleIsInteractive;
                    if (!bDesign)
                    {
                        if (arcNeedle.Animation == null)
                            arcNeedle.Animation = new IndicatorAnimation() { Enable = NeedleAnimationEnable };
                        else
                            (arcNeedle.Animation as IndicatorAnimation).Enable = NeedleAnimationEnable;
                    }
                    else
                        arcNeedle.Animation = null;

                    arcNeedle.Options.ZIndex = NeedleZIndex;
                    arcNeedle.Options.StartOffset = NeedleStartOffset;
                    arcNeedle.Options.EndOffset = NeedleEndOffset;

                }
                else
                {
                    if (arcNeedle != null)
                    {
                        arcNeedle.ValueChanged -= arcNeedle_ValueChanged;

                        arcScale.Needles.Clear();
                        arcNeedle.Presentation = null;
                        arcNeedle.Options = null;
                        arcNeedle.Animation = null;
                        arcNeedle = null;
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
                if ((int)GaugeBaseModel >= CustomBaseIndex || GaugeBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
                {
                    if (arcLayer != null)
                    {
                        arcScale.Layers.Clear();
                        arcLayer = null;
                    }
                }
                else
                {
                    if (arcLayer == null)
                    {
                        arcLayer = new ArcScaleLayer();

                        if (!arcScale.Layers.Contains(arcLayer))
                            arcScale.Layers.Add(arcLayer);
                    }
                    arcLayer.Presentation = GetArcScaleBackground(GaugeBaseModel);
                    UpdateDevBackColor(ArcScaleFill);
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateBaseArcScale()
        {
            return;
            if (circularGaugeObject == null)
                return;

            if (arcBaseScale == null)
            {
                arcBaseScale = new ArcScale();
                if (!circularGaugeObject.Scales.Contains(arcBaseScale))
                    circularGaugeObject.Scales.Add(arcBaseScale);
            }

            arcBaseScale.StartAngle = StartAngle;
            arcBaseScale.EndAngle = EndAngle;
            arcBaseScale.ShowSpindleCap = DevExpress.Utils.DefaultBoolean.False;
            arcBaseScale.ShowLabels = DevExpress.Utils.DefaultBoolean.False;
            arcBaseScale.ShowMajorTickmarks = DevExpress.Utils.DefaultBoolean.False;
            arcBaseScale.ShowMinorTickmarks = DevExpress.Utils.DefaultBoolean.False;
            arcBaseScale.ShowLine = DevExpress.Utils.DefaultBoolean.False;
            arcBaseScale.StartValue = 0;
            arcBaseScale.EndValue = 100;
        }

        private void UpdateArcScale()
        {
            if (circularGaugeObject == null)
                return;

            try
            {
                if (arcScale == null)
                {
                    arcScale = new ArcScale();
                    arcScale.SpindleCapOptions = new SpindleCapOptions();
                    arcScale.MajorTickmarkOptions = new MajorTickmarkOptions();
                    arcScale.MinorTickmarkOptions = new MinorTickmarkOptions();
                    CustomScaleLabelPresentation scalelabelpresentation = new CustomScaleLabelPresentation();
                    scalelabelpresentation.LabelTemplate = (ControlTemplate)LoadTemplate("LabelTemplate");
                    arcScale.LabelPresentation = scalelabelpresentation;

                    ArcScaleLinePresentation arcscalelinePresentationpresentation = GetLineScalePresentation(PredefinedElementKinds.Future);
                    arcScale.LinePresentation = arcscalelinePresentationpresentation;

                    arcScale.LabelOptions = new ArcScaleLabelOptions();

                    if (!circularGaugeObject.Scales.Contains(arcScale))
                        circularGaugeObject.Scales.Add(arcScale);
                }

                arcScale.StartAngle = StartAngle;
                arcScale.EndAngle = EndAngle;
                arcScale.MajorIntervalCount = MajorIntervalCount;
                arcScale.MinorIntervalCount = MinorIntervalCount;
                arcScale.StartValue = _StartValue;
                arcScale.EndValue = _EndValue;

                TickmarksPresentation arcscaletickmarkspresentation = GetTickmarkPresentation(TickmarksPresentation);
                arcScale.TickmarksPresentation = arcscaletickmarkspresentation;
                var _dtpres = arcScale.TickmarksPresentation as PredefinedTickmarksPresentation;
                _dtpres.MajorTickBrush = MajorTickmarkFill;
                _dtpres.MinorTickBrush = MinorTickmarkFill;

                var _lpres = arcScale.LinePresentation as PredefinedArcScaleLinePresentation;
                _lpres.Fill = LineFill;

                arcScale.SpindleCapPresentation = GetSpindlePresentation(SpindleCapPresentation);
                (arcScale.SpindleCapPresentation as PredefinedSpindleCapPresentation).Fill = SpindleFill;

                arcScale.SpindleCapOptions.FactorHeight = SpindleFactorHeight;
                arcScale.SpindleCapOptions.FactorWidth = SpindleFactorWidth;
                arcScale.SpindleCapOptions.ZIndex = SpindleCapZIndex;

                arcScale.MajorTickmarkOptions.FactorLength = MajorTickmarkFactorLength;
                arcScale.MajorTickmarkOptions.ZIndex = MajorTickmarkZIndex;
                arcScale.MajorTickmarkOptions.FactorThickness = MajorTickmarkFactorThickness;
                arcScale.MajorTickmarkOptions.Offset = MajorTickmarkOffset;
                arcScale.MajorTickmarkOptions.ShowFirst = MajorTickmarkShowFirst;
                arcScale.MajorTickmarkOptions.ShowLast = MajorTickmarkShowLast;

                arcScale.MinorTickmarkOptions.FactorLength = MinorTickmarkFactorLength;
                arcScale.MinorTickmarkOptions.ZIndex = MinorTickmarkZIndex;
                arcScale.MinorTickmarkOptions.FactorThickness = MinorTickmarkFactorThickness;
                arcScale.MinorTickmarkOptions.Offset = MinorTickmarkOffset;
                arcScale.MinorTickmarkOptions.ShowTicksForMajor = MinorTickmarkShowTicksForMajor;

                arcScale.LabelOptions.Orientation = LabelOrientation;
                arcScale.LabelOptions.ShowFirst = ShowFirstLabel;
                arcScale.LabelOptions.ShowLast = ShowLastLabel;
                arcScale.LabelOptions.Offset = LabelOffset;
                arcScale.LabelOptions.ZIndex = LabelZIndex;
                arcScale.LabelOptions.FormatString = LabelStringFormat;
            }
            catch (Exception)
            {
            }
        }

        object LoadTemplate(string template)
        {
            try
            {
                string basename = "LabelTemplate.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(CircularGauge).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null; 
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                object content = (object)canvas.TryFindResource(template);
                return content;
            }
            catch (Exception)
            {
                return null; 
            }
        }
        bool bInnerGaugeLoaded;
        private void UpdateGuageControl()
        {
            try
            {
                if (circularGaugeObject == null)
                {
                    circularGaugeObject = new CircularGaugeControl()
                    {
                        Width = 300,
                        Height = 300,
                        EnableAnimation = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontSize = 14,
                        FontStyle = FontStyles.Normal,
                        FontWeight = FontWeights.Medium,
                        FontFamily = new FontFamily("Segoe UI"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        ClipToBounds = false
                    };
                    circularGaugeObject.Loaded += (s, e) =>
                    {
                        if (!bInnerGaugeLoaded)
                        {
                            bInnerGaugeLoaded = true;
                            (s as UIElement).IsManipulationEnabled = true;
                        }
                    };

                    circularGaugeObject.Model = new CircularEcoModel();

                    if (!container.Children.Contains(circularGaugeObject))
                        container.Children.Add(circularGaugeObject);
                }
                Grid.SetZIndex(circularGaugeObject, 1);

                circularGaugeObject.PreviewMouseDown -= circularGaugeObject_PreviewMouseDown;
                circularGaugeObject.PreviewTouchDown -= circularGaugeObject_PreviewTouchDown;

                circularGaugeObject.PreviewMouseUp -= circularGaugeObject_PreviewMouseUp;
                circularGaugeObject.PreviewTouchUp -= circularGaugeObject_PreviewTouchUp;

                if (MarkerIsInteractive || RangeBarIsInteractive || NeedleIsInteractive)
                {
                    circularGaugeObject.PreviewMouseDown += circularGaugeObject_PreviewMouseDown;
                    circularGaugeObject.PreviewTouchDown += circularGaugeObject_PreviewTouchDown;

                    circularGaugeObject.PreviewMouseUp += circularGaugeObject_PreviewMouseUp;
                    circularGaugeObject.PreviewTouchUp += circularGaugeObject_PreviewTouchUp;
                }
                circularGaugeObject.Foreground = LabelForeground;
                circularGaugeObject.FontStyle = LabelFontSettings.FontStyle;
                circularGaugeObject.FontFamily = LabelFontSettings.FontFamily;
                circularGaugeObject.FontWeight = LabelFontSettings.FontWeight;
                circularGaugeObject.FontSize = LabelFontSettings.FontSize;
            }
            catch (Exception)
            {
            }
        }
        private void circularGaugeObject_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isChangingValue = false;
            isChangingMarkerValue = false;
            isChangingRangeBarValue = false;
            isChangingNeedleValue = false;
        }

        private void circularGaugeObject_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            isChangingValue = false;
            isChangingMarkerValue = false;
            isChangingRangeBarValue = false;
            isChangingNeedleValue = false;
        }

        private void circularGaugeObject_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            CircularGaugeControl meter = sender as CircularGaugeControl;
            if (meter != null)
            {
                CircularGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetTouchPoint(meter).Position);
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) && MarkerIsInteractive ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) && RangeBarIsInteractive ||
                (hitInfo.InNeedle || hitInfo.InScale || hitInfo.InRange) && NeedleIsInteractive)
                {
                    isChangingValue = true;
                    isUserMouseDownAction = true;
                }
                if ((hitInfo.InMarker) && MarkerIsInteractive)
                    isChangingMarkerValue = true;
                if ((hitInfo.InRangeBar) && RangeBarIsInteractive)
                    isChangingRangeBarValue = true;
                if ((hitInfo.InNeedle) && NeedleIsInteractive)
                    isChangingNeedleValue = true;
            }
        }

        bool isChangingMarkerValue;
        bool isChangingRangeBarValue;
        bool isChangingNeedleValue;
        private void circularGaugeObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CircularGaugeControl meter = sender as CircularGaugeControl;
            if (meter != null)
            {
                CircularGaugeHitInfo hitInfo = meter.CalcHitInfo(e.GetPosition(meter));
                if ((hitInfo.InMarker || hitInfo.InScale || hitInfo.InRange) && MarkerIsInteractive ||
                (hitInfo.InRangeBar || hitInfo.InScale || hitInfo.InRange) && RangeBarIsInteractive ||
                (hitInfo.InNeedle || hitInfo.InScale || hitInfo.InRange) && NeedleIsInteractive)
                {
                    isChangingValue = true;
                    isUserMouseDownAction = true;
                }
            }
        }

        private void UpdateBackBorder()
        {

            if ((int)GaugeBaseModel < CustomBaseIndex || GaugeBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
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

                    if (!container.Children.Contains(BackContent))
                        container.Children.Add(BackContent);
                }

                Grid.SetZIndex(BackContent, 0);

                Viewbox content = LoadBackContent();
                BackContent.Content = content;
                if (content != null)
                {
                    (from c in (BackContent.Content as UIElement).GetVisualChildrenOfType<Shape>()
                     where (c.Tag as String) == Properties.Settings.Default.TagBackground
                     select c).ToList().ForEach(child =>
                     {
                         child.Fill = ArcScaleFill;
                     });
                }
            }
        }
        private Viewbox LoadBackContent()
        {
            try
            {
                string basename = string.Format("GaugesContainers_{0}.xaml", (int)GaugeBaseModel - CustomBaseIndex + 1);
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(CircularGauge).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                var arcAspect = GetArcScaleBackgroundIndex();
                Viewbox content = (Viewbox)canvas.TryFindResource(string.Format("{0}_{1}", GaugeBaseModel.ToString(), arcAspect));
                if(arcAspect == PredefinedAspect.Half)
                {
                    content.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
                    content.ClearValue(FrameworkElement.VerticalAlignmentProperty);
                    content.ClearValue(FrameworkElement.HeightProperty);
                    content.ClearValue(FrameworkElement.WidthProperty);
                    content.ClearValue(Viewbox.StretchProperty);
                }
                content.Width = 300;
                content.Height = 300;
                return (Viewbox)content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ControlTemplate LoadMarkerTemplate()
        {
            try
            {
                string basename = "MarkerTemplate.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(CircularGauge).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                ControlTemplate content = (ControlTemplate)canvas.TryFindResource("MarkerTemplate");
                return (ControlTemplate)content;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private void UpdateValue()
        {
            double _value = Value;
            if (bDesign)
                _value = 50;// (EndValue - StartValue) / 2;

            if (arcNeedle != null)
            {
                if (!isChangingValue && !isUserMouseDownAction)
                    arcNeedle.Value = _value;
                else if (!isChangingNeedleValue && (isChangingValue || isUserMouseDownAction))
                    arcNeedle.Value = _value;
            }

            if (arcRange != null)
            {
                if (!isChangingValue && !isUserMouseDownAction)
                    arcRange.Value = _value;
                else if (!isChangingRangeBarValue && (isChangingValue || isUserMouseDownAction))
                    arcRange.Value = _value;
            }
            
            if (arcScaleMarker != null)
            {
                if (!isChangingValue && !isUserMouseDownAction)
                    arcScaleMarker.Value = _value;
                else if (!isChangingMarkerValue && (isChangingValue || isUserMouseDownAction))
                    arcScaleMarker.Value = _value;
            }

            if (arcBaseScale != null)
            {
                arcBaseScale.StartValue = 0;
                arcBaseScale.EndValue = 100;
                if (arcRangeBackground != null)
                    arcRangeBackground.Value = 100;
            }

            UpdateCustomValue(_value);
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
                if (WarningMarker != null)
                {
                    if (container.Children.Contains(WarningMarker))
                        container.Children.Remove(WarningMarker);
                    WarningMarker = null;
                }
                if (arcScaleMarker != null)
                    arcScaleMarker.Visible = true;
                if (arcNeedle != null)
                    arcNeedle.Visible = true;
                if (arcRange != null)
                    arcRange.Visible = true;
            }
            else if (showWarning)
            {
                container.ToolTip = Properties.Resources.NullValueWarning;
                if (arcScaleMarker != null)
                    arcScaleMarker.Visible = false;
                if (arcNeedle != null)
                    arcNeedle.Visible = false;
                if (arcRange != null)
                    arcRange.Visible = false;
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
        private ArcScaleNeedlePresentation GetNeedlePresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScaleNeedle.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (ArcScaleNeedlePresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (ArcScaleNeedlePresentation)Activator.CreateInstance(((ArcScaleNeedle.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt(0) as PredefinedElementKind).Type);
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
        private ArcScaleLinePresentation GetLineScalePresentation(PredefinedElementKinds value)
        {
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScale.PredefinedLinePresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;
                return (ArcScaleLinePresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (ArcScaleLinePresentation)Activator.CreateInstance(((ArcScale.PredefinedLinePresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)PredefinedElementKinds.Future) as PredefinedElementKind).Type);
            }

        }
        private ArcScaleLayerPresentation GetArcScaleBackground(PredefinedBaseElementKinds value)
        {
            int index = (int)GetArcScaleBackgroundIndex();
            try
            {
                PredefinedElementKind gaugeModelKind = (ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value * 5 + index) as PredefinedElementKind;
                return (ArcScaleLayerPresentation)Activator.CreateInstance(gaugeModelKind.Type);
            }
            catch
            {
                return (ArcScaleLayerPresentation)Activator.CreateInstance(((ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt(index) as PredefinedElementKind).Type);
            }
        }
        private void UpdateDevBackColor(Brush newValue)
        {
            if (bDispose)
                return;
            DefaultArcScaleBackgroundLayerPresentation layer = (DefaultArcScaleBackgroundLayerPresentation)arcLayer.Presentation;
            if (layer != null)
                layer.Fill = newValue is SolidColorBrush && (newValue as SolidColorBrush).Color.A == 0 ? null : newValue;
        }

        public PredefinedAspect GetArcScaleBackgroundIndex()
        {
            return GaugeControl.GetArcScaleBackgroundIndex(StartAngle, EndAngle);
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
            if(BackContent != null)
                BackContent.Content = null;
            BackContent = null;

            if (arcScale != null)
            {
                if(arcScale.LabelPresentation != null)
                {
                    CustomScaleLabelPresentation scalelabelpresentation = (CustomScaleLabelPresentation)arcScale.LabelPresentation;
                    ControlTemplate controltemplate = scalelabelpresentation.LabelTemplate;
                    scalelabelpresentation.LabelTemplate = null;
                }
                arcScale.LabelPresentation = null;
                arcScale.Layers.Clear();
                arcScale.Needles.Clear();
                arcScale.LabelOptions = null;
                arcScale.Ranges.Clear();
                arcScale.Markers.Clear();
                arcScale.Ranges.Clear();
                arcScale.CustomElements.Clear();
            }
            if (arcBaseScale != null)
            {
                if (arcBaseScale.LabelPresentation != null)
                {
                    CustomScaleLabelPresentation scalelabelpresentation = (CustomScaleLabelPresentation)arcBaseScale.LabelPresentation;
                    ControlTemplate controltemplate = scalelabelpresentation.LabelTemplate;
                    scalelabelpresentation.LabelTemplate = null;
                }
                arcScale.LabelPresentation = null;
                arcScale.Layers.Clear();
                arcScale.Needles.Clear();
                arcScale.LabelOptions = null;
                arcScale.Ranges.Clear();
                arcScale.Markers.Clear();
                arcScale.Ranges.Clear();
                arcScale.CustomElements?.Clear();
            }

            if (arcNeedle != null)
            {
                arcNeedle.ValueChanged -= arcNeedle_ValueChanged;
                arcNeedle.Options = null;
                arcNeedle.Presentation = null;
                arcNeedle.Animation = null;
            }

            if (arcScaleMarker != null)
            {
                arcScaleMarker.ValueChanged -= arcNeedle_ValueChanged;
                arcScaleMarker.Presentation = null;
                arcScaleMarker.Options = null;
                arcScaleMarker.Animation = null;
                arcScaleMarker = null;
            }

            if (arcRange != null)
            {
                arcRange.ValueChanged -= arcNeedle_ValueChanged;
                arcRange.Presentation = null;
                arcRange.Options = null;
                arcRange.Animation = null;
                arcRange = null;
            }

            if (arcRangeBackground != null)
            {
                arcRangeBackground.Presentation = null;
                arcRangeBackground.Options = null;
                arcRangeBackground.Animation = null;
                arcRangeBackground = null;
            }

            if (gridUnitContainer != null)
                gridUnitContainer.Content = null;

            if (gridValueContainer != null)
                gridValueContainer.Content = null;

            if (ArcScaleRange1 != null)
            {
                ArcScaleRange1.Presentation = null;
                ArcScaleRange1.Options = null;
            }
            if (ArcScaleRange2 != null)
            {
                ArcScaleRange2.Presentation = null;
                ArcScaleRange2.Options = null;
            }
            if (ArcScaleRange3 != null)
            {
                ArcScaleRange3.Presentation = null;
                ArcScaleRange3.Options = null;
            }

            if (circularGaugeObject != null)
            {
                circularGaugeObject.PreviewTouchDown -= circularGaugeObject_PreviewTouchDown;
                circularGaugeObject.PreviewMouseDown -= circularGaugeObject_PreviewMouseDown;
                circularGaugeObject.PreviewMouseUp -= circularGaugeObject_PreviewMouseUp;
                circularGaugeObject.PreviewTouchUp -= circularGaugeObject_PreviewTouchUp;
                circularGaugeObject.Scales.Clear();
            }

            circularGaugeObject = null;
            arcScale = null;
            arcLayer = null;
            arcNeedle = null;

            ArcScaleRange1 = null;
            ArcScaleRange2 = null;
            ArcScaleRange3 = null;
            arcScaleMarker = null;
            arcRangeBackground = null;
            arcRange = null;
            gridValueContainer = null;
            gridUnitContainer = null;
            circularGaugeObject = null;
            WarningMarker = null;

            if (!bDesign)
            {
                typeHelper.TerminateExecution(this, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, mintag, mintagMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, maxtag, maxtagMonitoredItemViewModel);
            }

            typeHelper.Dispose();
            typeHelper = null;
            previousEffect = null;

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
                    (document, typeof(CircularGauge), EngeneeringUnitProperty).DisplayName;
                map.Add(propertyName, EngeneeringUnit);
            }
            return map;
        }
        #endregion
    }
}
