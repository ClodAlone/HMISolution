using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;
using OPCUAViewModel;
using Sliders.Enums;
using System.Windows.Automation.Peers;
using Sliders.Automations;
using Utilities.WPF;
using Utilities;
using ViewModelLib;
using System.Windows.Media.Effects;
using UFInterfaces;
using ScreenSettings;
using System.IO;
using DynamicTagAwareHelper;
using Sliders.Converters;
using System.Threading;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using WPFUtilities.Extensions;

namespace Sliders
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class SliderControl : SurfaceSlider, IDisposable, IDynamicTagAware, IEntityReference
    {

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        #endregion


        #region DP

        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool UseThumbOptions { get { return SliderStyle == SliderStyleEnum.SimpleFlat || SliderStyle == SliderStyleEnum.SimpleFlatLight; } }

        #region DP OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            //dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SliderControl));
            //dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SliderControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(SliderControl));
            dpd.AddValueChangedSafe(this, OnBorderBrushChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(TickFrequencyProperty, typeof(SliderControl));
            dpd.AddValueChangedSafe(this, OnTickFrequencyChanged);
            //dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(SliderControl));
            //dpd.AddValueChangedSafe(this, OnFontSizeChanged);
            //dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(SliderControl));
            //dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            //dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(SliderControl));
            //dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(SliderControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(SliderControl));
            dpd.RemoveValueChangedSafe(this, OnBorderBrushChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(TickFrequencyProperty, typeof(SliderControl));
            dpd.RemoveValueChangedSafe(this, OnTickFrequencyChanged);
            //dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(SliderControl));
            //dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
            //dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(SliderControl));
            //dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
        }

        //private void OnFontSizeChanged(object sender, EventArgs e)
        //{
        //    var control = sender as SliderControl;
        //    if (control != null)
        //    {
        //        control.OnFontSizeChanged();
        //    }
        //}
        //protected virtual void OnFontSizeChanged()
        //{
        //    if (bInit)
        //    {
        //        TickFontSize = FontSize;
        //    }
        //}
        //private void OnFontFamilyChanged(object sender, EventArgs e)
        //{
        //    var control = sender as SliderControl;
        //    if (control != null)
        //    {
        //        control.OnFontFamilyChanged();
        //    }
        //}
        //protected virtual void OnFontFamilyChanged()
        //{
        //    if (bInit)
        //    {
        //        TickFontFamily = FontFamily;
        //    }
        //}
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as SliderControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (bInit)
                ThumbColor = Foreground;
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as SliderControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            if (bInit && !IsManipulationEnabled)
                BarColor = Background;
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            var control = sender as SliderControl;
            if (control != null)
            {
                control.OnBorderBrushChanged();
            }
        }

        protected virtual void OnBorderBrushChanged()
        {
            if (bInit)
                TickColor = BorderBrush;
        }

        private void OnTickFrequencyChanged(object sender, EventArgs e)
        {
            var control = sender as SliderControl;
            if (control != null)
            {
                control.OnTickFrequencyChanged();
            }
        }

        protected virtual void OnTickFrequencyChanged()
        {
            if (bInit)
                UpdateSliderLayout();
        }

        
        #endregion

        #region SliderStyle
        public static readonly DependencyProperty SliderStyleProperty = DependencyProperty.Register("SliderStyle", typeof(SliderStyleEnum), typeof(SliderControl), new UIPropertyMetadata(SliderStyleEnum.Default, new PropertyChangedCallback(OnSliderStyleChanged), new CoerceValueCallback(OnCoerceSliderStyle)));

        private static object OnCoerceSliderStyle(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceSliderStyle((SliderStyleEnum)value);
            else
                return value;
        }

        private static void OnSliderStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnSliderStyleChanged((SliderStyleEnum)e.OldValue, (SliderStyleEnum)e.NewValue);
        }

        protected virtual SliderStyleEnum OnCoerceSliderStyle(SliderStyleEnum value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSliderStyleChanged(SliderStyleEnum oldValue, SliderStyleEnum newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (oldValue != newValue && bLoaded && bInit && templateApplied)
            //    UpdateSliderStyle();
        }

        [Category("Advanced")]
        [SvgValueConverter(ConverterType = typeof(ConvertSliderControlStyle), RequiredKey = true)]
        public SliderStyleEnum SliderStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SliderStyleEnum)GetValue(SliderStyleProperty);
            }
            set
            {
                SetValue(SliderStyleProperty, value);
            }
        }

        #endregion


        #region TickSize
        public static readonly DependencyProperty TickSizeProperty = DependencyProperty.Register("TickSize", typeof(int), typeof(SliderControl), new UIPropertyMetadata(6, new PropertyChangedCallback(OnTickSizeChanged), new CoerceValueCallback(OnCoerceTickSize)));

        private static object OnCoerceTickSize(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTickSize((int)value);
            else
                return value;
        }

        private static void OnTickSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnTickSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceTickSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSliderLayout();
        }

        public int TickSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(TickSizeProperty);
            }
            set
            {
                SetValue(TickSizeProperty, value);
            }
        }

        #endregion

        #region TickMinorSize
        public static readonly DependencyProperty TickMinorSizeProperty = DependencyProperty.Register("TickMinorSize", typeof(int), typeof(SliderControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTickMinorSizeChanged), new CoerceValueCallback(OnCoerceTickMinorSize)));

        private static object OnCoerceTickMinorSize(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTickMinorSize((int)value);
            else
                return value;
        }

        private static void OnTickMinorSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnTickMinorSizeChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceTickMinorSize(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickMinorSizeChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSliderLayout();
        }

        public int TickMinorSize
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(TickMinorSizeProperty);
            }
            set
            {
                SetValue(TickMinorSizeProperty, value);
            }
        }

        #endregion


        #region TickThickness
        public static readonly DependencyProperty TickThicknessProperty = DependencyProperty.Register("TickThickness", typeof(double), typeof(SliderControl), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnTickThicknessChanged), new CoerceValueCallback(OnCoerceTickThickness)));

        private static object OnCoerceTickThickness(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTickThickness((double)value);
            else
                return value;
        }

        private static void OnTickThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnTickThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTickThickness(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickThicknessChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSliderLayout();
        }
        [Category("Advanced")]
        public double TickThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TickThicknessProperty);
            }
            set
            {
                SetValue(TickThicknessProperty, value);
            }
        }

        #endregion


        #region TickColor
        public static readonly DependencyProperty TickColorProperty = DependencyProperty.Register("TickColor", typeof(Brush), typeof(SliderControl), new UIPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnTickColorChanged), new CoerceValueCallback(OnCoerceTickColor)));

        private static object OnCoerceTickColor(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTickColor((Brush)value);
            else
                return value;
        }

        private static void OnTickColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnTickColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceTickColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Brush TickColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(TickColorProperty);
            }
            set
            {
                SetValue(TickColorProperty, value);
            }
        }

        #endregion

        #region TickMajorFrequency
        public static readonly DependencyProperty TickMajorFrequencyProperty = DependencyProperty.Register("TickMajorFrequency", typeof(double), typeof(SliderControl), new UIPropertyMetadata(20.0, new PropertyChangedCallback(OnTickMajorFrequencyChanged), new CoerceValueCallback(OnCoerceTickMajorFrequency)));

        private static object OnCoerceTickMajorFrequency(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTickMajorFrequency((double)value);
            else
                return value;
        }

        private static void OnTickMajorFrequencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnTickMajorFrequencyChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceTickMajorFrequency(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTickMajorFrequencyChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSliderLayout();
        }

        public double TickMajorFrequency
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(TickMajorFrequencyProperty);
            }
            set
            {
                SetValue(TickMajorFrequencyProperty, value);
            }
        }

        #endregion


        #region ShowLabel
        public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.Register("ShowLabel", typeof(bool), typeof(SliderControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowLabelChanged), new CoerceValueCallback(OnCoerceShowLabel)));

        private static object OnCoerceShowLabel(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceShowLabel((bool)value);
            else
                return value;
        }

        private static void OnShowLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnShowLabelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowLabel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowLabelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSliderLayout();

        }

        public bool ShowLabel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowLabelProperty);
            }
            set
            {
                SetValue(ShowLabelProperty, value);
            }
        }

        #endregion

        #region DecreaseButtonColor
        public static readonly DependencyProperty DecreaseButtonColorProperty = DependencyProperty.Register("DecreaseButtonColor", typeof(Brush), typeof(SliderControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x51, 0x51, 0x51)), new PropertyChangedCallback(OnDecreaseButtonColorChanged), new CoerceValueCallback(OnCoerceDecreaseButtonColor)));

        private static object OnCoerceDecreaseButtonColor(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceDecreaseButtonColor((Brush)value);
            else
                return value;
        }

        private static void OnDecreaseButtonColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnDecreaseButtonColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceDecreaseButtonColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDecreaseButtonColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Brush DecreaseButtonColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(DecreaseButtonColorProperty);
            }
            set
            {
                SetValue(DecreaseButtonColorProperty, value);
            }
        }

        #endregion

        #region ThumbBorderBrush
        public static readonly DependencyProperty ThumbBorderBrushProperty = DependencyProperty.Register("ThumbBorderBrush", typeof(Brush), typeof(SliderControl), new UIPropertyMetadata(Brushes.Black));
        public Brush ThumbBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ThumbBorderBrushProperty);
            }
            set
            {
                SetValue(ThumbBorderBrushProperty, value);
            }
        }

        #endregion

        #region ThumbBorderThickness
        public static readonly DependencyProperty ThumbBorderThicknessProperty = DependencyProperty.Register("ThumbBorderThickness", typeof(double), typeof(SliderControl), new UIPropertyMetadata(0d));

        public double ThumbBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ThumbBorderThicknessProperty);
            }
            set
            {
                SetValue(ThumbBorderThicknessProperty, value);
            }
        }

        #endregion


        #region ThumbColor
        public static readonly DependencyProperty ThumbColorProperty = DependencyProperty.Register("ThumbColor", typeof(Brush), typeof(SliderControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x51, 0x51, 0x51)), new PropertyChangedCallback(OnThumbColorChanged), new CoerceValueCallback(OnCoerceThumbColor)));

        private static object OnCoerceThumbColor(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceThumbColor((Brush)value);
            else
                return value;
        }

        private static void OnThumbColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnThumbColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceThumbColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnThumbColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public Brush ThumbColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ThumbColorProperty);
            }
            set
            {
                SetValue(ThumbColorProperty, value);
            }
        }

        #endregion


        #region ThumbFactor
        public static readonly DependencyProperty ThumbFactorTemplateProperty = DependencyProperty.Register("ThumbFactor", typeof(string), typeof(SliderControl), new UIPropertyMetadata("Thumb"));
        [SvgValueConverter(ConverterType = typeof(ConvertThumbFactor), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string ThumbFactor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ThumbFactorTemplateProperty);
            }
            set
            {
                SetValue(ThumbFactorTemplateProperty, value);
            }
        }
        #endregion


        #region BarColor
        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor", typeof(Brush), typeof(SliderControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x51, 0x51, 0x51)), new PropertyChangedCallback(OnBarColorChanged), new CoerceValueCallback(OnCoerceBarColor)));

        private static object OnCoerceBarColor(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceBarColor((Brush)value);
            else
                return value;
        }

        private static void OnBarColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                control.OnBarColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceBarColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBarColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public Brush BarColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BarColorProperty);
            }
            set
            {
                SetValue(BarColorProperty, value);
            }
        }

        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(SliderControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
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
                UpdateRanges();
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SliderControl), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
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
                UpdateRanges();
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
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(SliderControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));

        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
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
                UpdateSliderLayout();
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
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(SliderControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMinValueChanged), new CoerceValueCallback(OnCoerceTagMinValue)));

        private static object OnCoerceTagMinValue(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTagMinValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(SliderControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMaxValueChanged), new CoerceValueCallback(OnCoerceTagMaxValue)));

        private static object OnCoerceTagMaxValue(DependencyObject o, object value)
        {
            SliderControl control = o as SliderControl;
            if (control != null)
                return control.OnCoerceTagMaxValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SliderControl control = o as SliderControl;
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
        #endregion


        #region Declarations
        List<string> matchChangedMap = new List<string>();
        bool bLoaded;
        bool bDesign;
        bool bInit;
        bool bDatacontextChanging;
        bool templateApplied;
        private OPCUAEntityReference mintag;
        private OPCUAEntityReference maxtag;
        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        ScreenDocument Document;
        TypeHelper typeHelper = new TypeHelper();
        CancellationTokenSource cts;

        #endregion

        #region ctor
        public SliderControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

                    if (!bDesign)
                        InitControl();
                    if (bDesign)
                    {
                        Background = BarColor;
                        BorderBrush = TickColor;
                        Foreground = ThumbColor;
                    }
                    else
                        DetachOverrideBaseProperties();
                    UpdateSliderLayout();
                    //if (templateApplied)
                    //    UpdateSliderStyle();
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
        #endregion
        MonitoredItemViewModel monitoredItemViewModel;
        bool bTagMinValueSet;
        bool bTagMaxValueSet;
        double engStartValue;
        double engEndValue;
        bool monitoredHasRange;
        #region Method

        protected void UpdateRanges()
        {
            double startValue = MinValue;
            double endValue = MaxValue;
            Action action = () =>
            {
                if (monitoredItemViewModel != null && monitoredItemViewModel.HasRange)
                {
                    monitoredHasRange = true;
                    engStartValue = monitoredItemViewModel.Range.Low;
                    engEndValue = monitoredItemViewModel.Range.High;

                    if (TagMinValue == null || TagMinValue.TagReference == null)
                    {
                        if (UseEUnit)
                            startValue = engStartValue;
                        else
                            startValue = Math.Max(startValue, engStartValue);
                    }
                    if (TagMaxValue == null || TagMaxValue.TagReference == null)
                    {
                        if (UseEUnit)
                            endValue = engEndValue;
                        else
                            endValue = Math.Min(endValue, engEndValue);
                    }
                }
                else
                    monitoredHasRange = false;

                if (TagMinValue == null || TagMinValue.TagReference == null || !bTagMinValueSet)
                    Minimum = startValue;
                if (TagMaxValue == null || TagMaxValue.TagReference == null || !bTagMaxValueSet)
                    Maximum = endValue;

                bDatacontextChanging = false;
                if (bInit)
                    UpdateSliderLayout();
            };

           // if (UseEUnit)
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
            //else
            //    action();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            templateApplied = true;
            //if (bLoaded)
            //{
            //    UpdateSliderStyle();
            //}
            var tickBars = this.GetVisualChildrenOfType<CustomTickBar>().ToList();
            DimValueConverter converter = new DimValueConverter();
            tickBars.ForEach(tickbar =>
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("ActualWidth"),
                    ElementName = "Thumb",
                    Converter = converter,
                    ConverterParameter = tickbar.Name== "TopTick" ? 2 : 1
                };
                tickbar.SetBinding(MarginProperty, binding);
            });
            var trackBkgd = (from obj in this.GetVisualChildrenOfType<Rectangle>() where Name== "PART_TrackBkgd" select obj).FirstOrDefault();
            if (trackBkgd != null)
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("ActualWidth"),
                    ElementName = "Thumb",
                    Converter = converter
                };
                trackBkgd.SetBinding(MarginProperty, binding);
            }
        }

        //private void UpdateSliderStyle()
        //{
        //    var thumb = this.GetVisualChildrenOfType<Microsoft.Surface.Presentation.Controls.Primitives.SurfaceThumb>().FirstOrDefault();
        //    var style = TryFindResource($"{SliderStyle.ToString()}_ThumbStyle") as Style;
        //    if (style != null)
        //        thumb.Style = style;
        //}

        private void UpdateSliderLayout()
        {
            if (bDispose || bDatacontextChanging)
                return;

            if (bDesign || DesignerProperties.GetIsInDesignMode(this))
            {
                Minimum = /*UseEUnit ? 0.0 :*/ MinValue;
                Maximum = /*UseEUnit ? 100.0 :*/ MaxValue;
            }
            if (RunningOnServer)
            {
                var elements = (this as UIElement).GetVisualChildrenOfType<Microsoft.Surface.Presentation.Controls.Primitives.SurfaceRepeatButton>().ToList();
                elements.ForEach(x => x.IsHitTestVisible = false);
            }

            (from c in this.GetVisualChildrenOfType<CustomTickBar>()
             select c).ToList().ForEach(child =>
             {
                 child.InvalidateVisual();
             });
        }
        private void SetEntityError(String error)
        {
            return;
            //if (String.IsNullOrEmpty(error))
            //{
            //    if (errorEffectOn)
            //    {
            //        (grid as UIElement).Effect = previousEffect;
            //        (grid as UIElement).ClipToBounds = previousClipToBounds;
            //        //(this as UIElement).Opacity = 1;
            //        previousEffect = null;
            //        errorEffectOn = false;
            //    }
            //}
            //else
            //{
            //    if (!errorEffectOn)
            //    {
            //        errorEffectOn = true;
            //        previousEffect = (grid as UIElement).Effect;
            //        previousClipToBounds = (grid as UIElement).ClipToBounds;

            //        var effect = new DropShadowEffect
            //        {
            //            ShadowDepth = 0,
            //            BlurRadius = 10,
            //            Color = Colors.Red
            //        };
            //        (grid as UIElement).Effect = effect;
            //        (grid as UIElement).ClipToBounds = false;
            //    }
            //}
        }

        private void InitControl()
        {
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
                                Maximum = monitoredHasRange ? Math.Min(engEndValue, val) : val;
                                UpdateRanges();
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
                if(m.DataValue != null)
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
                                    Minimum = monitoredHasRange ? Math.Max(engStartValue, val) : val;
                                    UpdateRanges();
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

        internal double? GetThumbFactor()
        {
            switch (SliderStyle)
            {
                case SliderStyleEnum.Default:
                    return 1.7;
                case SliderStyleEnum.Plastic:
                    return 1.5;
                case SliderStyleEnum.Thin:
                    return 0.5;
                case SliderStyleEnum.Flat:
                    return 2;
                case SliderStyleEnum.SimpleFlat:
                    return 1;
                case SliderStyleEnum.SimpleFlatLight:
                    return 1;
                case SliderStyleEnum.PlasticLight:
                    return 1.5;
                case SliderStyleEnum.FlatLight:
                    return 1.7;
                default:
                    return 1.5;
            }
        }

        internal string LoadSVGThumbStyle()
        {
            return string.Format("{0}_{1}", SliderStyle.ToString(), Orientation.ToString());
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

        #region IDIsposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            DetachOverrideBaseProperties();

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

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
        }
        #endregion
        public static readonly List<string> mustAddToList = Properties.Settings.Default.SvgFrameworkProperties.Split('|').ToList();
    }
    internal class ConvertSVGHasStyles : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null)
                return null;

            if (sender is SliderControl)
            {
                return new Dictionary<string, Brush>() 
                { 
                    { "ThumbColor", (sender as SliderControl).ThumbColor }};
            }
            else
                return null;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is SliderControl)
            {
                SliderControl slider = sender as SliderControl;
                return new Dictionary<string, string>() { { slider.SliderStyle.ToString(), slider.LoadSVGThumbStyle() } };
            }
            else
                return null;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, XmlElement>);
            }
        }
    }
    internal class ConvertSliderControlStyle : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            var entity = (SliderStyleEnum)value;
            if (sender is SliderControl)
            {
                SliderControl slider = sender as SliderControl;
                return slider.LoadSVGThumbStyle();
            }
            else
                return null;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    class ConvertThumbFactor : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            if (sender is SliderControl)
            {
                SliderControl slider = sender as SliderControl;
                return new Dictionary<string, object>() {
                    { "FactorWidth", slider.Orientation == Orientation.Horizontal ? slider.GetThumbFactor() : null },
                    { "FactorHeight", slider.Orientation == Orientation.Horizontal ? null : slider.GetThumbFactor() }};
            }
            else
                return null;
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
