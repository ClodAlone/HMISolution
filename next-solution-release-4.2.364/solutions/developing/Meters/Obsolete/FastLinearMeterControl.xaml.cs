using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RangeBaseControl;
using Utilities;
using ActiproSoftware.Windows.Controls.Gauge;
using ActiproSoftware.Windows;
using System.Globalization;
using StringManager.ComponentService;
using UFInterfaces.PropertyControl;
using ScreenSettings;
using System.ComponentModel;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using Utilities.WPF;
using System.Xml.Serialization;
using ActiproSoftware.Windows.Controls.Gauge.Primitives;
using Meters.Automations;
using System.Windows.Automation.Peers;
using UFInterfaces;

namespace Meters
{
    public enum FastMeterTypes
    {
        None = 0,
        Bulging = 1,
        Hollowed = 2,
        RoundedBulging = 3,
        RoundedHollowed = 4,
        Flat = 5
    }
    public enum PointerMarkerType
    {
        Circle = 0,
        Diamond = 1,
        Ellipse = 2,
        Rectangle = 3,
        RoundedRectangle = 4,
        SwordBlunt = 5,
        SwordSharp = 6,
        TriangleBlunt = 7,
        TriangleSharp = 8,
        //CustomImage = 9,
        //CustomGeometry = 10,
        DoubleCircleBar = 11,
        DoubleRectangleBar = 12,
        DoubleTriangleBar = 13
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [Obsolete("Use the Meters.FastLinearMeter.xaml insted of this.")]
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true, TypeName="LinearMeterControl")]
    public partial class FastLinearMeterControl : RangeBaseControl.RangeBaseControl, IContainPropertyEditors, IStringIDAware
    {
        #region Declarations
        bool previousClipToBounds;
        bool errorEffectOn;
        LinearGauge gauge;
        LinearScale scale;
        LinearPointerBar bar;
        LinearPointerMarker marker;
        LinearTickSet linearTickSet;
        TextBlock engineeringUnit;
        TextBlock valueControl;
        Viewbox backgroundControl;
        Grid mainGrid;
        IStringEditorManager stringManager;
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FastLinearMeterAutomationPeer(this);
        }

        public void FastLinearMeterInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region DPs

        #region OverrideBaseProperties

        public override void OverrideBaseProperties()
        {
            base.OverrideBaseProperties();
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(FastLinearMeterControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(FastLinearMeterControl));
            dpd.AddValueChangedSafe(this, OnBorderBrushChanged);
        }

        public override void DetachOverrideBaseProperties()
        {
            base.DetachOverrideBaseProperties();
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(FastLinearMeterControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(FastLinearMeterControl));
            dpd.RemoveValueChangedSafe(this, OnBorderBrushChanged);
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as FastLinearMeterControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }

        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesign)
            {
                UpdateBackgroundColor();
            }
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            var control = sender as FastLinearMeterControl;
            if (control != null)
            {
                control.OnBorderBrushChanged();
            }
        }

        protected virtual void OnBorderBrushChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesign)
            {
                UpdateBorderColor();
            }
        }
        #endregion


        #region MeterScaleColor
        public static readonly DependencyProperty MeterScaleColorProperty = DependencyProperty.Register("MeterScaleColor", typeof(Brush), typeof(FastLinearMeterControl), new UIPropertyMetadata(Brushes.Black));
        [Category("Scale")]
        public Brush MeterScaleColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MeterScaleColorProperty);
            }
            set
            {
                SetValue(MeterScaleColorProperty, value);
            }
        }
        #endregion
        #region MeterScalePlacement
        public static readonly DependencyProperty MeterScalePlacementProperty = DependencyProperty.Register("MeterScalePlacement", typeof(ScalePlacement), typeof(FastLinearMeterControl), new UIPropertyMetadata(ScalePlacement.Outside));
        [Category("Scale")]
        [Browsable(false)]
        [XmlIgnore]
        public ScalePlacement MeterScalePlacement
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ScalePlacement)GetValue(MeterScalePlacementProperty);
            }
            set
            {
                SetValue(MeterScalePlacementProperty, value);
            }
        }
        #endregion
        #region MeterScaleMajorInterval
        public static readonly DependencyProperty MeterScaleMajorIntervalProperty = DependencyProperty.Register("MeterScaleMajorInterval", typeof(double), typeof(FastLinearMeterControl), new UIPropertyMetadata(10d));
        [Category("Scale")]
        public double MeterScaleMajorInterval
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MeterScaleMajorIntervalProperty);
            }
            set
            {
                SetValue(MeterScaleMajorIntervalProperty, value);
            }
        }

        #endregion
        #region MeterScaleMinorInterval
        public static readonly DependencyProperty MeterScaleMinorIntervalProperty = DependencyProperty.Register("MeterScaleMinorInterval", typeof(double), typeof(FastLinearMeterControl), new UIPropertyMetadata(5d));
        [Category("Scale")]
        public double MeterScaleMinorInterval
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MeterScaleMinorIntervalProperty);
            }
            set
            {
                SetValue(MeterScaleMinorIntervalProperty, value);
            }
        }

        #endregion
        #region MeterScaleOffset
        public static readonly DependencyProperty MeterScaleOffsetProperty = DependencyProperty.Register("MeterScaleOffset", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(5)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MeterScaleOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MeterScaleOffsetProperty);
            }
            set
            {
                SetValue(MeterScaleOffsetProperty, value);
            }
        }
        #endregion
        #region MeterScaleMinorOffset
        public static readonly DependencyProperty MeterScaleMinorOffsetProperty = DependencyProperty.Register("MeterScaleMinorOffset", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(5)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MeterScaleMinorOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MeterScaleMinorOffsetProperty);
            }
            set
            {
                SetValue(MeterScaleMinorOffsetProperty, value);
            }
        }
        #endregion
        #region LabelOffset
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(20)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertLabelUnit), RequiredKey = true)]
        public Unit LabelOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(LabelOffsetProperty);
            }
            set
            {
                SetValue(LabelOffsetProperty, value);
            }
        }
        #endregion
        #region CanDrag
        public static readonly DependencyProperty CanDragProperty = DependencyProperty.Register("CanDrag", typeof(bool), typeof(FastLinearMeterControl), new UIPropertyMetadata(false));
        public bool CanDrag
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(CanDragProperty);
            }
            set
            {
                SetValue(CanDragProperty, value);
            }
        }
        #endregion


        #region MajorTickMarkHeight
        public static readonly DependencyProperty MajorTickMarkHeightProperty = DependencyProperty.Register("MajorTickMarkHeight", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(10)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MajorTickMarkHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MajorTickMarkHeightProperty);
            }
            set
            {
                SetValue(MajorTickMarkHeightProperty, value);
            }
        }
        #endregion
        #region MajorTickMarkWidth
        public static readonly DependencyProperty MajorTickMarkWidthProperty = DependencyProperty.Register("MajorTickMarkWidth", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(1)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName="MajorTickmarkFactorThickness")]
        public Unit MajorTickMarkWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MajorTickMarkWidthProperty);
            }
            set
            {
                SetValue(MajorTickMarkWidthProperty, value);
            }
        }
        #endregion

        #region MinorTickMarkHeight
        public static readonly DependencyProperty MinorTickMarkHeightProperty = DependencyProperty.Register("MinorTickMarkHeight", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(6)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MinorTickMarkHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MinorTickMarkHeightProperty);
            }
            set
            {
                SetValue(MinorTickMarkHeightProperty, value);
            }
        }
        #endregion
        #region MinorTickMarkWidth
        public static readonly DependencyProperty MinorTickMarkWidthProperty = DependencyProperty.Register("MinorTickMarkWidth", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(1)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName = "MinorTickmarkFactorThickness")]
        public Unit MinorTickMarkWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MinorTickMarkWidthProperty);
            }
            set
            {
                SetValue(MinorTickMarkWidthProperty, value);
            }
        }
        #endregion

     
        #region BarColor
        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor", typeof(Brush), typeof(FastLinearMeterControl), new UIPropertyMetadata(Brushes.Red));
        [Category("Bar")]
        [SvgValueConverter(typeof(ConvertLevelFill), RequiredKey = true, NeedSVGUrlBrushes = true, PropertyName = "LevelFill")]
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
        #region BarBackColor
        public static readonly DependencyProperty BarBackColorProperty = DependencyProperty.Register("BarBackColor", typeof(Brush), typeof(FastLinearMeterControl), new UIPropertyMetadata(Brushes.Black));
        [Category("Bar")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "LevelBackgroundFill")]
        public Brush BarBackColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(BarBackColorProperty);
            }
            set
            {
                SetValue(BarBackColorProperty, value);
            }
        }
        #endregion
        #region BarWidth
        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register("BarWidth", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(20)));
        [Category("Bar")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName = "RangeBarThickness")]
        public Unit BarWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(BarWidthProperty);
            }
            set
            {
                SetValue(BarWidthProperty, value);
            }
        }
        #endregion
        


        #region HasMarker
        public static readonly DependencyProperty HasMarkerProperty = DependencyProperty.Register("HasMarker", typeof(bool), typeof(FastLinearMeterControl), new UIPropertyMetadata(true));
        [Category("Marker")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "MarkerVisible")]
        public bool HasMarker
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(HasMarkerProperty);
            }
            set
            {
                SetValue(HasMarkerProperty, value);
            }
        }

        #endregion
        #region MarkerColor
        public static readonly DependencyProperty MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(FastLinearMeterControl), new UIPropertyMetadata(Brushes.Red));
        [Category("Marker")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "MarkerFill")]
        public Brush MarkerColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MarkerColorProperty);
            }
            set
            {
                SetValue(MarkerColorProperty, value);
            }
        }
        #endregion
        #region MarkerStroke
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(FastLinearMeterControl), new UIPropertyMetadata(Brushes.Black));
        [Category("Marker")]
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

        #region MarkerStrokeThickness
        public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register("MarkerBorderThickness", typeof(double), typeof(FastLinearMeterControl), new UIPropertyMetadata(1d));
        [Category("Marker")]
        public double MarkerBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MarkerStrokeThicknessProperty);
            }
            set
            {
                SetValue(MarkerStrokeThicknessProperty, value);
            }
        }
        #endregion

        #region MarkerWidth
        public static readonly DependencyProperty MarkerWidthProperty = DependencyProperty.Register("MarkerWidth", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(18)));
        [Category("Marker")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName = "MarkerFactorWidth")]
        public Unit MarkerWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MarkerWidthProperty);
            }
            set
            {
                SetValue(MarkerWidthProperty, value);
            }
        }
        #endregion
        #region MarkerHeight
        public static readonly DependencyProperty MarkerHeightProperty = DependencyProperty.Register("MarkerHeight", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(18)));
        [Category("Marker")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName = "MarkerFactorHeight")]
        public Unit MarkerHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MarkerHeightProperty);
            }
            set
            {
                SetValue(MarkerHeightProperty, value);
            }
        }
        #endregion
        #region MarkerOffset
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(Unit), typeof(FastLinearMeterControl), new UIPropertyMetadata(Unit.Pixel(0)));
        [Category("Marker")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MarkerOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MarkerOffsetProperty);
            }
            set
            {
                SetValue(MarkerOffsetProperty, value);
            }
        }
        #endregion
        #region MarkerPlacement
        public static readonly DependencyProperty MarkerPlacementProperty = DependencyProperty.Register("MarkerPlacement", typeof(ScalePlacement), typeof(FastLinearMeterControl), new UIPropertyMetadata(ScalePlacement.Inside));
        [Category("Marker")]
        [Browsable(false)]
        [XmlIgnore]
        public ScalePlacement MarkerPlacement
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ScalePlacement)GetValue(MarkerPlacementProperty);
            }
            set
            {
                SetValue(MarkerPlacementProperty, value);
            }
        }
        #endregion
        #region MarkerType
        public static readonly DependencyProperty MarkerTypeProperty = DependencyProperty.Register("MarkerType", typeof(PointerMarkerType), typeof(FastLinearMeterControl), new UIPropertyMetadata(PointerMarkerType.TriangleSharp));
        [Category("Marker")]
        public PointerMarkerType MarkerType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PointerMarkerType)GetValue(MarkerTypeProperty);
            }
            set
            {
                SetValue(MarkerTypeProperty, value);
            }
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(FastLinearMeterControl), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                return control.OnCoerceValue((double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                control.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bInit)
                UpdateValue();
        }

        public double Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        #endregion


        #region ShowEngeneeringUnit
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(FastLinearMeterControl), new UIPropertyMetadata(false));
        [Category("EngeneeringUnit")]
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
        #region EngeneeringOffset
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(FastLinearMeterControl), new UIPropertyMetadata(new Thickness(10,80,0,0)));
        [Category("EngeneeringUnit")]
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

        #region ShowValue
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(FastLinearMeterControl), new UIPropertyMetadata(false));
        [Category("Value")]
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
        #region ValueOffset
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(FastLinearMeterControl), new UIPropertyMetadata(new Thickness(10,4,0,0)));
        [Category("Value")]
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(string), typeof(FastLinearMeterControl), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                return control.OnCoerceValueStringFormat((string)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                control.OnValueStringFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceValueStringFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueStringFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bLoaded)
                UpdateValue();
        }
        [Category("Value")]
        public string ValueStringFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ValueStringFormatProperty);
            }
            set
            {
                SetValue(ValueStringFormatProperty, value);
            }
        }

        #endregion
        #region LabelStringFormat
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(string), typeof(FastLinearMeterControl), new UIPropertyMetadata("{0:0}"));
        [Category("Value")]
        public string LabelStringFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(LabelStringFormatProperty);
            }
            set
            {
                SetValue(LabelStringFormatProperty, value);
            }
        }

        #endregion


        #region FlowDirection
        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(FastLinearMeterControl), new UIPropertyMetadata(FlowDirection.LeftToRight));
        [Category("Style")]
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

        #region Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FastLinearMeterControl), new UIPropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged), new CoerceValueCallback(OnCoerceOrientation)));

        private static object OnCoerceOrientation(DependencyObject o, object value)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                return control.OnCoerceOrientation((Orientation)value);
            else
                return value;
        }

        private static void OnOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                control.OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
        }

        protected virtual Orientation OnCoerceOrientation(Orientation value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bLoaded)
                UpdateBackground();
        }
        [Category("Style")]
        public Orientation Orientation
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #endregion
        #region MeterType
        public static readonly DependencyProperty MeterTypeProperty = DependencyProperty.Register("MeterType", typeof(FastMeterTypes), typeof(FastLinearMeterControl), new UIPropertyMetadata(FastMeterTypes.None, new PropertyChangedCallback(OnMeterTypeChanged), new CoerceValueCallback(OnCoerceMeterType)));

        private static object OnCoerceMeterType(DependencyObject o, object value)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                return control.OnCoerceMeterType((FastMeterTypes)value);
            else
                return value;
        }

        private static void OnMeterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastLinearMeterControl control = o as FastLinearMeterControl;
            if (control != null)
                control.OnMeterTypeChanged((FastMeterTypes)e.OldValue, (FastMeterTypes)e.NewValue);
        }

        protected virtual FastMeterTypes OnCoerceMeterType(FastMeterTypes value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMeterTypeChanged(FastMeterTypes oldValue, FastMeterTypes newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bLoaded)
                UpdateBackground();
        }
        [Category("Style")]
        public FastMeterTypes MeterType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FastMeterTypes)GetValue(MeterTypeProperty);
            }
            set
            {
                SetValue(MeterTypeProperty, value);
            }
        }

        #endregion


        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool RangeBarVisible { get { return true; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool LevelVisible { get { return false; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string LevelBackgroundTemplate { get { return $"_FastLinearMeterControl_{LoadSVGBackLevelContent()}"; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string LinearBaseModel { get { return LoadSVGBackContent(); } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Dictionary<string, object> LevelOptions { get { return GetLevelOptions().ToDictionary(); } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int MarkerFactorHeight { get { return (int)MarkerHeight.Value / 10; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int MarkerFactorWidth { get { return (int)MarkerWidth.Value / 10; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool MarkerIsInteractive { get { return CanDrag; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool LevelIsInteractive { get { return CanDrag; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool RangeBarIsInteractive { get { return CanDrag; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int LayoutMode { get { return Orientation == Orientation.Vertical ? 2 : 0; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MajorTickmarkFactorLength { get { return MajorTickMarkHeight.Value / 10; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MajorTickmarkOffset 
        { 
            get 
            {
                var offset = - MeterScaleOffset.Value;
                int delta = 3;
                return offset <= delta && offset >= - delta ? 0 : 1;
            } 
        }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MinorTickmarkFactorLength { get { return MinorTickMarkHeight.Value / 6; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MinorTickmarkOffset { get { return -MeterScaleMinorOffset.Value; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush MinorTickmarkFill { get { return MeterScaleColor; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush MajorTickmarkFill { get { return MeterScaleColor; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool Range1Visible { get { return false; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool Range2Visible { get { return false; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool Range3Visible { get { return false; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int RangeBarOffset { get { return 0; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool IsFastLinearMeterControl { get { return true; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public int SvgFOEmptyMargin { get { return Orientation == Orientation.Vertical ? (int)Height * 25 / 100 : (int)Width * 25 / 100; } }
        #endregion


        #region ctor
        static FastLinearMeterControl()
        {
            ActiproUtilities.ActiproUtilities.RegisterLicense();
        }

        public FastLinearMeterControl()
        {
            InitializeComponent();
            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                UpdateStartEndValues();
                UpdateScaleStartEndValues();

                UpdateCustomElements();
            };
        }
        #endregion

        #region override Methods
#if !WINDOWS_UWP
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                bDesign = true;

            mainGrid = this.GetVisualChild(0) as Grid;
            gauge = mainGrid?.Children[0] as ActiproSoftware.Windows.Controls.Gauge.LinearGauge;
            engineeringUnit = mainGrid?.Children[1] as TextBlock;
            valueControl = mainGrid?.Children[2] as TextBlock;
            if (mainGrid != null && gauge != null)
            {
                scale = gauge.Scales.FirstOrDefault();
                if (scale != null)
                {
                    linearTickSet = scale.TickSets.FirstOrDefault() as LinearTickSet;
                    bar = linearTickSet.Pointers[0] as LinearPointerBar;
                    marker = linearTickSet.Pointers[1] as LinearPointerMarker;
                    //if (MeterType != FastMeterTypes.None)
                        UpdateBackground();

                    if (Document == null)
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
                    UpdateStartEndValues();
                    UpdateScaleStartEndValues();
                    UpdateCustomElements(); 

                    bInit = true;
                }
            }
        }
        protected override void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        (mainGrid as UIElement).Effect = previousEffect;
                        (mainGrid as UIElement).ClipToBounds = previousClipToBounds;
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
                        previousEffect = (mainGrid as UIElement).Effect;
                        previousClipToBounds = (mainGrid as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (mainGrid as UIElement).Effect = effect;
                        (mainGrid as UIElement).ClipToBounds = false;
                    }
                }
            });
        }
        protected override void UpdateScaleRanges()
        {

        }
        protected override void UpdateScaleStartEndValues()
        {
            if (linearTickSet != null)
            {
                if (!double.IsNaN(_StartValue))
                    linearTickSet.Minimum = _StartValue;
                if (!double.IsNaN(_EndValue))
                    linearTickSet.Maximum = _EndValue;
            }
        }
        protected override void ShowMarker(bool v)
        {

        }
        protected override void UpdateCustomElements()
        {
            UpdateEUnitControl();
            UpdateValue();
        }
        #endregion

        #region Methods

        private LevelOptions GetLevelOptions()
        {
            int offset = 0;
            int thickness = (int)BarWidth.Value / 6;
            return new LevelOptions()
            {
                Offset = offset,
                Thickness = thickness
            };
        }
        internal string LoadSVGBackLevelContent(bool retKey = false)
        {
            string key = "Bar";
            if (retKey)
                return key;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return string.Format("{0}_H", key);
                    break;
                case Orientation.Vertical:
                    return string.Format("{0}_V", key);
                    break;
                default:
                    return string.Format("{0}_H", key);
                    break;
            }
        }
        internal string LoadSVGBackContent()
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    return string.Format("{0}_H", MeterType.ToString());
                    break;
                case Orientation.Vertical:
                    return string.Format("{0}_V", MeterType.ToString());
                    break;
                default:
                    return string.Format("{0}_H", MeterType.ToString());
                    break;
            }
        }
        void UpdateBorderColor()
        {
            if (MeterType != FastMeterTypes.None && backgroundControl != null)
            {
                if (this.ReadLocalValue(BorderBrushProperty) != DependencyProperty.UnsetValue)
                {
                    (from c in (backgroundControl as UIElement).GetVisualChildrenOfType<Shape>()
                     where (c.Tag as String) == Properties.Settings.Default.TagFill
                     select c).ToList().ForEach(child =>
                     {
                         child.Fill = BorderBrush;
                     });
                }
            }
        }
        void UpdateBackgroundColor()
        {
            if (MeterType != FastMeterTypes.None && backgroundControl != null)
            {
                if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
                {
                    (from c in (backgroundControl as UIElement).GetVisualChildrenOfType<Shape>()
                     where (c.Tag as String) == Properties.Settings.Default.TagBackground
                     select c).ToList().ForEach(child =>
                     {
                         child.Fill = Background;
                     });
                }
            }
        }
        void UpdateBackground()
        {
            //if (MeterType != FastMeterTypes.None)
            {
                if(backgroundControl != null)
                    mainGrid.Children.Remove(backgroundControl);

                backgroundControl = TryFindResource($"{MeterType}_{Orientation}") as Viewbox;
                if (backgroundControl != null)
                    mainGrid.Children.Insert(0, backgroundControl);
                MeterScalePlacement = Orientation == Orientation.Horizontal ? ScalePlacement.Outside : ScalePlacement.Inside;
                MarkerPlacement = Orientation == Orientation.Horizontal ? ScalePlacement.Inside : ScalePlacement.Outside;
                UpdateBackgroundColor();
                UpdateBorderColor();
            }
            //else
            //{
            //    if (backgroundControl != null)
            //    {
            //        mainGrid.Children.Remove(backgroundControl);
            //        backgroundControl = null;
            //    }
            //}
        }
        IDictionary<string, string> stringlist = new Dictionary<string, string>();
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            if (stringManager == null)
                return;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                bool bUntranslated = bDesign && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                else
                    stringlist?.Clear();
                UpdateEUnitControl();
                UpdateValue(); 
            });
        }

        private void LinearPointer_ValueChanged(object sender, ActiproSoftware.Windows.DoublePropertyChangedRoutedEventArgs e)
        {
            LinearPointerBase linearPointer = sender as LinearPointerBase;
            if (linearPointer != null && linearPointer.IsDragging)
            {
                Value = linearPointer.Value;
            }
        }

        private void UpdateValue()
        {
            double _value = Value;
            if (bDesign)
                _value = 50;// (EndValue - StartValue) / 2;

            if (double.IsNaN(_value))
                return;
            if (bar != null && !bar.IsDragging)
                bar.Value = _value;
            if (marker != null && !marker.IsDragging)
                marker.Value = _value;
            if(valueControl != null)
                valueControl.Text = ConvertValue(_value);
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
        void UpdateEUnitControl()
        {
            string eunit = TranslationHelpers.TranslationHelper.TranslateComposedText(EngeneeringUnit, stringlist, EngeneeringUnit);
            string meas = ConverterLabel ?? eunit;
            if (engineeringUnit != null)
            {
                engineeringUnit.Text = meas;
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            base.Dispose(true);

            DetachOverrideBaseProperties();

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            mainGrid?.Children.Clear();
            bar = null;
            marker = null;
            linearTickSet = null;
            scale = null;
            gauge = null;
            engineeringUnit = null;
            valueControl = null;
            backgroundControl = null;
            mainGrid = null;
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
                factory.SetValue(NumericUpDownPropertyEditor.ValueConverterProperty, new ActiproUtilities.Converters.SpinEditUnitConverter());
                dt.DataType = typeof(Unit);
                dt.VisualTree = factory;
                mapDataTemplates.Add(MeterScaleOffsetProperty, dt);
                mapDataTemplates.Add(MeterScaleMinorOffsetProperty, dt);
                mapDataTemplates.Add(MajorTickMarkHeightProperty, dt);
                mapDataTemplates.Add(MajorTickMarkWidthProperty, dt);
                mapDataTemplates.Add(MinorTickMarkHeightProperty, dt);
                mapDataTemplates.Add(MinorTickMarkWidthProperty, dt);
                mapDataTemplates.Add(BarWidthProperty, dt);
                mapDataTemplates.Add(MarkerWidthProperty, dt);
                mapDataTemplates.Add(MarkerHeightProperty, dt);
                mapDataTemplates.Add(MarkerOffsetProperty, dt);
                mapDataTemplates.Add(LabelOffsetProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0d);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
                dt.DataType = typeof(double);
                dt.VisualTree = factory;
                mapDataTemplates.Add(MeterScaleMajorIntervalProperty, dt);
                mapDataTemplates.Add(MeterScaleMinorIntervalProperty, dt);

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                IWorkspace workspace = null;
                if (Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

                if (workspace != null)
                {
                    dt = new DataTemplate();
                    factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TextPropertyEditor));
                    factory.SetValue(WPFUtilities.PropertyDataTemplate.TextPropertyEditor.WorkspaceProperty, workspace);
                    dt.DataType = typeof(String);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(EngeneeringUnitProperty, dt);
                }
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
                    (document, typeof(RangeBaseControl.RangeBaseControl), EngeneeringUnitProperty).DisplayName;
                map.Add(propertyName, EngeneeringUnit);
            }
            return map;
        }
        #endregion
    }
    
    internal class ConvertLabelUnit : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (value is Unit)
            {
                return -((Unit)value).Value - 10;
            }

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

    internal class ConvertUnit : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is LinearMeterControl)
            {
                var prop = property as DependencyProperty;
                if (prop.Name.Equals(LinearMeterControl.MarkerFactorWidthProperty.Name))
                    return (int)value * 9;
                else if (prop.Name.Equals(LinearMeterControl.MarkerFactorHeightProperty.Name))
                    return (int)value * 9;
                else if (prop.Name.Equals(LinearMeterControl.MarkerOffsetProperty.Name))
                {
                    var meter = (sender as LinearMeterControl);
                    var delta = (meter.MarkerFactorHeight * 9) / 2;
                    var barThickness = meter.RangeBarVisible ? meter.RangeBarThickness : meter.LevelThickness * 6.25;
                    return (int)value - barThickness - delta;
                }
                else
                    return value;
            }
            else if (value is Unit)
            {
                return ((Unit)value).Value;
            }

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
