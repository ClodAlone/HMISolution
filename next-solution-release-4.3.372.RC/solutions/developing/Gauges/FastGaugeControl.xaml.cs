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
using Gauges.Enums;
using Utilities.WPF;
using System.Xml.Serialization;
using ActiproSoftware.Windows.Controls.Gauge.Primitives;
using Gauges.Automations;
using System.Windows.Automation.Peers;
using UFInterfaces;

namespace Gauges
{
    public enum FastMeterTypes
    {
        None = 0,
        Bulging = 1,
        Hollowed = 2,
        Flat = 3
    }
    public enum PointerCapType
    {
        CircleConcave = 0,
        CircleConvex = 1,
        CircleFlat = 2,
        CircleShiny = 3,
        GearConcave = 4,
        GearConvex = 5,
        GearFlat = 6,
        GearShiny = 7,
        //CustomImage = 8,
        //CustomGeometry = 9
    }
    public enum PointerNeedleType
    {
        Rectangle = 0,
        RoundedRectangle = 1,
        SwordBlunt = 2,
        SwordSharp = 3,
        Teardrop = 4,
        TriangleBlunt = 5,
        TriangleSharp = 6,
        PivotRectangle = 7,
        PivotRoundedRectangle = 8,
        PivotSwordBlunt = 9,
        PivotSwordSharp = 10,
        PivotTeardrop = 11,
        PivotTriangleBlunt = 12,
        PivotTriangleSharp = 13,
        Knob = 14,
        //CustomImage = 15,
        //CustomGeometry = 16
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
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class FastGaugeControl : RangeBaseControl.RangeBaseControl, IContainPropertyEditors, IStringIDAware
    {
        #region Declarations
        bool previousClipToBounds;
        bool errorEffectOn;
        ActiproSoftware.Windows.Controls.Gauge.CircularGauge gauge;
        CircularScale scale;
        CircularPointerBar bar;
        CircularPointerMarker marker;
        CircularPointerNeedle needle;
        CircularTickSet circularTickSet;
        TextBlock engineeringUnit;
        TextBlock valueControl;
        Viewbox backgroundControl;
        Grid mainGrid;
        IStringEditorManager stringManager;
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FastGaugesAutomationPeer(this);
        }

        public void FastGaugeInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region DPs

        #region BackgroundRadius
        public static readonly DependencyProperty BackgroundRadiusProperty = DependencyProperty.Register("BackgroundRadius", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Percentage(100)));
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit BackgroundRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(BackgroundRadiusProperty);
            }
            set
            {
                SetValue(BackgroundRadiusProperty, value);
            }
        }
        #endregion


        #region BackgroundBorderThickness
        public static readonly DependencyProperty BackgroundBorderThicknessProperty = DependencyProperty.Register("BackgroundBorderThickness", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(0d));
        [Browsable(false)]
        [XmlIgnore]
        public double BackgroundBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(BackgroundBorderThicknessProperty);
            }
            set
            {
                SetValue(BackgroundBorderThicknessProperty, value);
            }
        }

        #endregion


        #region OverrideBaseProperties

        public override void OverrideBaseProperties()
        {
            base.OverrideBaseProperties();
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(FastGaugeControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(FastGaugeControl));
            dpd.AddValueChangedSafe(this, OnBorderBrushChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(FastGaugeControl));
            dpd.AddValueChangedSafe(this, OnBorderThicknessChanged);
        }

        public override void DetachOverrideBaseProperties()
        {
            base.DetachOverrideBaseProperties();
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(FastGaugeControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(FastGaugeControl));
            dpd.RemoveValueChangedSafe(this, OnBorderBrushChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(FastGaugeControl));
            dpd.RemoveValueChangedSafe(this, OnBorderThicknessChanged);
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as FastGaugeControl;
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
            var control = sender as FastGaugeControl;
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

        private void OnBorderThicknessChanged(object sender, EventArgs e)
        {
            var control = sender as FastGaugeControl;
            if (control != null)
            {
                control.OnBorderThicknessChanged();
            }
        }

        protected virtual void OnBorderThicknessChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            BackgroundBorderThickness = Math.Max(BorderThickness.Bottom, Math.Max(BorderThickness.Right, Math.Max(BorderThickness.Left, BorderThickness.Top)));
        }
        #endregion


        #region MeterScaleColor
        public static readonly DependencyProperty MeterScaleColorProperty = DependencyProperty.Register("MeterScaleColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Black));
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
        public static readonly DependencyProperty MeterScalePlacementProperty = DependencyProperty.Register("MeterScalePlacement", typeof(ScalePlacement), typeof(FastGaugeControl), new UIPropertyMetadata(ScalePlacement.Outside));
        [Category("Scale")]
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
        public static readonly DependencyProperty MeterScaleMajorIntervalProperty = DependencyProperty.Register("MeterScaleMajorInterval", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(10d));
        [Category("Scale")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "MajorIntervalCount")]
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
        public static readonly DependencyProperty MeterScaleMinorIntervalProperty = DependencyProperty.Register("MeterScaleMinorInterval", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(5d));
        [Category("Scale")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "MinorIntervalCount")]
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
        public static readonly DependencyProperty MeterScaleOffsetProperty = DependencyProperty.Register("MeterScaleOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(-7)));
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
        public static readonly DependencyProperty MeterScaleMinorOffsetProperty = DependencyProperty.Register("MeterScaleMinorOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(-5)));
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


        #region MeterScaleRadius
        public static readonly DependencyProperty MeterScaleRadiusProperty = DependencyProperty.Register("MeterScaleRadius", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Percentage(95)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit MeterScaleRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(MeterScaleRadiusProperty);
            }
            set
            {
                SetValue(MeterScaleRadiusProperty, value);
            }
        }

        #endregion


        #region StartAngle
        public static readonly DependencyProperty MeterScaleStartAngleProperty = DependencyProperty.Register("MeterScaleStartAngle", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(135d));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertMeterScaleStartAngle), RequiredKey = true, PropertyName = "StartAngle")]
        public double MeterScaleStartAngle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MeterScaleStartAngleProperty);
            }
            set
            {
                SetValue(MeterScaleStartAngleProperty, value);
            }
        }
        #endregion 
        #region SweepAngle
        public static readonly DependencyProperty MeterScaleSweepAngleProperty = DependencyProperty.Register("MeterScaleSweepAngle", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(270d));
        [Category("Scale")]
        public double MeterScaleSweepAngle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MeterScaleSweepAngleProperty);
            }
            set
            {
                SetValue(MeterScaleSweepAngleProperty, value);
            }
        }

        #endregion

        #region LabelOffset
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(-32)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty CanDragProperty = DependencyProperty.Register("CanDrag", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(false));
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
        public static readonly DependencyProperty MajorTickMarkHeightProperty = DependencyProperty.Register("MajorTickMarkHeight", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(5)));
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
        public static readonly DependencyProperty MajorTickMarkWidthProperty = DependencyProperty.Register("MajorTickMarkWidth", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(1)));
        [Category("Scale")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true, PropertyName = "MajorTickmarkFactorThickness")]
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
        public static readonly DependencyProperty MinorTickMarkHeightProperty = DependencyProperty.Register("MinorTickMarkHeight", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(3)));
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
        public static readonly DependencyProperty MinorTickMarkWidthProperty = DependencyProperty.Register("MinorTickMarkWidth", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(1)));
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
        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register("BarColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Red));
        [Category("Bar")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "RangeBarFill")]
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
        public static readonly DependencyProperty BarBackColorProperty = DependencyProperty.Register("BarBackColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Transparent));
        [Category("Bar")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "RangeBarBackground")]
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
        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register("BarWidth", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(9)));
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
        public static readonly DependencyProperty HasMarkerProperty = DependencyProperty.Register("HasMarker", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(false));
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
        public static readonly DependencyProperty MarkerColorProperty = DependencyProperty.Register("MarkerColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Red));
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
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Black));
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
        public static readonly DependencyProperty MarkerStrokeThicknessProperty = DependencyProperty.Register("MarkerBorderThickness", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(1d));
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
        public static readonly DependencyProperty MarkerWidthProperty = DependencyProperty.Register("MarkerWidth", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(8)));
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
        public static readonly DependencyProperty MarkerHeightProperty = DependencyProperty.Register("MarkerHeight", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(8)));
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
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(-8)));
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
        public static readonly DependencyProperty MarkerPlacementProperty = DependencyProperty.Register("MarkerPlacement", typeof(ScalePlacement), typeof(FastGaugeControl), new UIPropertyMetadata(ScalePlacement.Outside));
        [Category("Marker")]
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
        public static readonly DependencyProperty MarkerTypeProperty = DependencyProperty.Register("MarkerType", typeof(PointerMarkerType), typeof(FastGaugeControl), new UIPropertyMetadata(PointerMarkerType.TriangleSharp));
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


        #region HasNeedle
        public static readonly DependencyProperty HasNeedleProperty = DependencyProperty.Register("HasNeedle", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(true));
        [Category("Needle")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "NeedleVisible")]
        public bool HasNeedle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(HasNeedleProperty);
            }
            set
            {
                SetValue(HasNeedleProperty, value);
            }
        }
        #endregion
        #region NeedleColor
        public static readonly DependencyProperty NeedleColorProperty = DependencyProperty.Register("NeedleColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Red));
        [Category("Needle")]
        [SvgValueConverter(typeof(ConvertBrushToSvgValue), RequiredKey = true, NeedSVGUrlBrushes = true, PropertyName = "NeedleFill")]
        public Brush NeedleColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(NeedleColorProperty);
            }
            set
            {
                SetValue(NeedleColorProperty, value);
            }
        }

        #endregion
        #region NeedleWidth
        public static readonly DependencyProperty NeedleWidthProperty = DependencyProperty.Register("NeedleWidth", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Percentage(8)));
        [Category("Needle")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit NeedleWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(NeedleWidthProperty);
            }
            set
            {
                SetValue(NeedleWidthProperty, value);
            }
        }
        #endregion
        #region NeedleHeight
        public static readonly DependencyProperty NeedleHeightProperty = DependencyProperty.Register("NeedleHeight", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Percentage(90)));
        [Category("Needle")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit NeedleHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(NeedleHeightProperty);
            }
            set
            {
                SetValue(NeedleHeightProperty, value);
            }
        }
        #endregion
        #region NeedleOffset
        public static readonly DependencyProperty NeedleOffsetProperty = DependencyProperty.Register("NeedleOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(0)));
        [Category("Needle")]
        [Browsable(false)]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit NeedleOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(NeedleOffsetProperty);
            }
            set
            {
                SetValue(NeedleOffsetProperty, value);
            }
        }
        #endregion

        #region NeedleType
        public static readonly DependencyProperty NeedleTypeProperty = DependencyProperty.Register("NeedleType", typeof(PointerNeedleType), typeof(FastGaugeControl), new UIPropertyMetadata(PointerNeedleType.TriangleSharp));
        [Category("Needle")]
        public PointerNeedleType NeedleType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PointerNeedleType)GetValue(NeedleTypeProperty);
            }
            set
            {
                SetValue(NeedleTypeProperty, value);
            }
        }
        #endregion

        #region HasCap
        public static readonly DependencyProperty HasCapProperty = DependencyProperty.Register("HasCap", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(true));
        [Category("Cap")]
        public bool HasCap
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(HasCapProperty);
            }
            set
            {
                SetValue(HasCapProperty, value);
            }
        }
        #endregion
        #region CapColor
        public static readonly DependencyProperty CapColorProperty = DependencyProperty.Register("CapColor", typeof(Brush), typeof(FastGaugeControl), new UIPropertyMetadata(Brushes.Black));
        [Category("Cap")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "SpindleFill")]
        public Brush CapColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(CapColorProperty);
            }
            set
            {
                SetValue(CapColorProperty, value);
            }
        }
        #endregion
        #region CapRadius
        public static readonly DependencyProperty CapRadiusProperty = DependencyProperty.Register("CapRadius", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(9)));
        [Category("Cap")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpindleFactor), RequiredKey = true)]
        public Unit CapRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(CapRadiusProperty);
            }
            set
            {
                SetValue(CapRadiusProperty, value);
            }
        }
        #endregion

        #region CapOffset
        public static readonly DependencyProperty CapOffsetProperty = DependencyProperty.Register("CapOffset", typeof(Unit), typeof(FastGaugeControl), new UIPropertyMetadata(Unit.Pixel(0)));
        [Category("Cap")]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
        public Unit CapOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Unit)GetValue(CapOffsetProperty);
            }
            set
            {
                SetValue(CapOffsetProperty, value);
            }
        }
        #endregion
        #region CapType
        public static readonly DependencyProperty CapTypeProperty = DependencyProperty.Register("CapType", typeof(PointerCapType), typeof(FastGaugeControl), new UIPropertyMetadata(PointerCapType.CircleFlat));
        [Category("Cap")]
        [SvgValueConverter(ConverterType = typeof(ConvertSpindleCapPresentation), RequiredKey = true)]
        public PointerCapType CapType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PointerCapType)GetValue(CapTypeProperty);
            }
            set
            {
                SetValue(CapTypeProperty, value);
            }
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(FastGaugeControl), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            FastGaugeControl control = o as FastGaugeControl;
            if (control != null)
                return control.OnCoerceValue((double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastGaugeControl control = o as FastGaugeControl;
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
            if (oldValue != newValue && bInit && bLoaded)
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
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(false));
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
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(FastGaugeControl), new UIPropertyMetadata(new Thickness(100, 170, 0, 0)));
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
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(FastGaugeControl), new UIPropertyMetadata(false));
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
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(FastGaugeControl), new UIPropertyMetadata(new Thickness(0)));
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(string), typeof(FastGaugeControl), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            FastGaugeControl control = o as FastGaugeControl;
            if (control != null)
                return control.OnCoerceValueStringFormat((string)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastGaugeControl control = o as FastGaugeControl;
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
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(string), typeof(FastGaugeControl), new UIPropertyMetadata("{0:0.#}"));
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
        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(FastGaugeControl), new UIPropertyMetadata(FlowDirection.LeftToRight));
        [Category("Style")]
        [SvgValueConverter(RequiredKey = true, PropertyName = "ScaleFlowDirection")]
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

        #region MeterType
        public static readonly DependencyProperty MeterTypeProperty = DependencyProperty.Register("MeterType", typeof(FastMeterTypes), typeof(FastGaugeControl), new UIPropertyMetadata(FastMeterTypes.None, new PropertyChangedCallback(OnMeterTypeChanged), new CoerceValueCallback(OnCoerceMeterType)));

        private static object OnCoerceMeterType(DependencyObject o, object value)
        {
            FastGaugeControl control = o as FastGaugeControl;
            if (control != null)
                return control.OnCoerceMeterType((FastMeterTypes)value);
            else
                return value;
        }

        private static void OnMeterTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FastGaugeControl control = o as FastGaugeControl;
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
        public string SpindleCapPresentation { get { return $"SpindleCap_{CapType.ToString()}"; } }

        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string GaugeBaseModel 
        { 
            get 
            {
                if (MeterScaleStartAngle >= 180 && MeterScaleStartAngle <= 360 && EndAngle <= 360)
                    return $"{MeterType.ToString()}_Half"; 
                else
                    return $"{MeterType.ToString()}_Full";
            } 
        }

        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string GaugeType
        {
            get
            {
                if (MeterScaleStartAngle >= 180 && MeterScaleStartAngle <= 360 && EndAngle <= 360)
                    return $"Half";
                else
                    return $"Full";
            }
        }


        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush ArcScaleFill { get { return Background; } }

        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(RequiredKey = true)]
        public bool ShowFirstLabel { get{ return true;} }

        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public double EndAngle 
        { 
            get 
            {
                var angle = MeterScaleStartAngle;
                if (angle < 0)
                    angle += 360;
                return angle + MeterScaleSweepAngle; 
            } 
        }

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
        public double SpindleFactorHeight { get { return CapRadius.Value; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public double SpindleFactorWidth { get { return CapRadius.Value; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool RangeBarVisible { get { return true; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public double RangeBarOffset { get { return -(BarWidth.Value/2) - 11d; } }
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
        public Double MajorTickmarkFactorLength { get { return MajorTickMarkHeight.Value/10; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MajorTickmarkOffset { get { return MeterScaleOffset.Value + MajorTickMarkHeight.Value -11; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MinorTickmarkFactorLength { get { return MinorTickMarkHeight.Value / 6; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Double MinorTickmarkOffset { get { return MeterScaleOffset.Value + MinorTickMarkHeight.Value - 11; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool IsFastGaugeControl { get { return true; } }
        [SvgValueConverter(ConverterType = typeof(ConvertScaleOptions), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public string ScaleOptions { get { return null; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool MarkerIsInteractive { get { return CanDrag; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool NeedleIsInteractive { get { return CanDrag; } }
        [SvgValueConverter(RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool RangeBarIsInteractive { get { return CanDrag; } }
        #endregion


        #region ctor
        static FastGaugeControl()
        {
            ActiproUtilities.ActiproUtilities.RegisterLicense();
        }

        public FastGaugeControl()
        {
            InitializeComponent();
            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                if (bInit)
                {
                    UpdateStartEndValues();
                    UpdateScaleStartEndValues();
                    UpdateCustomElements();
                }

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
            };
        }
        #endregion

        #region override Methods
        [EditorBrowsable(EditorBrowsableState.Never)]
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
            gauge = mainGrid?.Children[0] as ActiproSoftware.Windows.Controls.Gauge.CircularGauge;
            engineeringUnit = mainGrid?.Children[1] as TextBlock;
            valueControl = mainGrid?.Children[2] as TextBlock;
            if (mainGrid != null && gauge != null)
            {
                scale = gauge.Scales.FirstOrDefault();
                if (scale != null)
                {
                    circularTickSet = scale.TickSets.FirstOrDefault() as CircularTickSet;
                    needle = circularTickSet.Pointers[1] as CircularPointerNeedle;
                    bar = circularTickSet.Pointers[2] as CircularPointerBar;
                    marker = circularTickSet.Pointers[3] as CircularPointerMarker;
                    UpdateBackground();
                    if (bLoaded)
                    {
                        UpdateStartEndValues();
                        UpdateScaleStartEndValues();
                        UpdateCustomElements();
                    }
                    bInit = true;
                    if (!string.IsNullOrEmpty(lastError))
                        SetEntityError(lastError);
                }
            }
        }
        protected override void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (!bInit)
                {
                    lastError = error;
                    return;
                }
                else
                    lastError = null;

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
            if (circularTickSet != null)
            {
                if (!double.IsNaN(_StartValue))
                    circularTickSet.Minimum = _StartValue;
                if (!double.IsNaN(_EndValue))
                    circularTickSet.Maximum = _EndValue;
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

                backgroundControl = TryFindResource($"{MeterType}") as Viewbox;
                if (backgroundControl != null)
                    mainGrid.Children.Insert(0, backgroundControl);
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
                if (bInit && bLoaded)
                {
                    UpdateEUnitControl();
                    UpdateValue();
                }
            });
        }

        private void CircularPointer_ValueChanged(object sender, ActiproSoftware.Windows.DoublePropertyChangedRoutedEventArgs e)
        {
            CircularPointerBase circularPointer = sender as CircularPointerBase;
            if (circularPointer != null && circularPointer.IsDragging)
            {
                Value = circularPointer.Value;
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
            if (needle != null && !needle.IsDragging)
                needle.Value = _value; 
            if (valueControl != null)
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

        internal string LoadSVGGaugeType()
        {
            return "0";
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
            mainGrid = null;
            bar = null;
            marker = null;
            circularTickSet = null;
            scale = null;
            gauge = null;
            engineeringUnit = null;
            valueControl = null;
            backgroundControl = null;
            needle = null;
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
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
                factory.SetValue(NumericUpDownPropertyEditor.ValueConverterProperty, new ActiproUtilities.Converters.SpinEditUnitConverter());
                dt.DataType = typeof(Unit);
                dt.VisualTree = factory;
                mapDataTemplates.Add(MajorTickMarkHeightProperty, dt);
                mapDataTemplates.Add(MajorTickMarkWidthProperty, dt);
                mapDataTemplates.Add(MinorTickMarkHeightProperty, dt);
                mapDataTemplates.Add(MinorTickMarkWidthProperty, dt);
                mapDataTemplates.Add(BarWidthProperty, dt);
                mapDataTemplates.Add(MarkerWidthProperty, dt);
                mapDataTemplates.Add(MarkerHeightProperty, dt);
                mapDataTemplates.Add(CapRadiusProperty, dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0d);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
                factory.SetValue(NumericUpDownPropertyEditor.ValueConverterProperty, new ActiproUtilities.Converters.SpinEditUnitConverter(UnitType.Percentage));
                dt.DataType = typeof(Unit);
                dt.VisualTree = factory;
                mapDataTemplates.Add(NeedleWidthProperty, dt);
                mapDataTemplates.Add(NeedleHeightProperty, dt);
                mapDataTemplates.Add(MeterScaleRadiusProperty, dt);
                mapDataTemplates.Add(BackgroundRadiusProperty, dt);


                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, double.MinValue);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 1d);
                factory.SetValue(NumericUpDownPropertyEditor.ValueConverterProperty, new ActiproUtilities.Converters.SpinEditUnitConverter());
                dt.DataType = typeof(Unit);
                dt.VisualTree = factory;
                mapDataTemplates.Add(NeedleOffsetProperty, dt);
                mapDataTemplates.Add(MeterScaleOffsetProperty, dt);
                mapDataTemplates.Add(MeterScaleMinorOffsetProperty, dt);
                mapDataTemplates.Add(MarkerOffsetProperty, dt);
                mapDataTemplates.Add(LabelOffsetProperty, dt);
                mapDataTemplates.Add(CapOffsetProperty, dt);

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

    internal class ConvertUnit : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;

            if(sender is GaugeControl)
            {
                var prop = property as DependencyProperty;
                if (prop.Name.Equals(GaugeControl.MarkerFactorWidthProperty.Name))
                    return Math.Round((double)value / 1.16, 2);
                else if (prop.Name.Equals(GaugeControl.MarkerFactorHeightProperty.Name))
                    return Math.Round((double)value / 2.55, 2);
                else
                    return value;
            }
            else if (value is Unit)
            {
                var prop = property as DependencyProperty;
                if (prop.Name.Equals(FastGaugeControl.MarkerOffsetProperty.Name))
                {
                    return -((sender as FastGaugeControl).MarkerOffset.Value) - 8; //the fixed margin between the marker and gauge's borders
                }
                else
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

    internal class ConvertMeterScaleStartAngle : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            FastGaugeControl fastGauge = sender as FastGaugeControl;
            if (fastGauge == null)
                return null;
            var angle = fastGauge.MeterScaleStartAngle;
            if (angle < 0)
                angle += 360;
            return angle;
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
