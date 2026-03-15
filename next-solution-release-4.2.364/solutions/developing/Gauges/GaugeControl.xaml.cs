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
using Utilities;
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
using System.Xml;
using System.Xml.Serialization;

namespace Gauges
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class GaugeControl : RangeBaseControl.RangeBaseControl, IContainPropertyEditors, IStringIDAware
    {
        #region Dependency Properties

        #region ValueStyle

        #region ShowValue
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueChanged), new CoerceValueCallback(OnCoerceShowValue)));

        private static object OnCoerceShowValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceShowValue((bool)value);
            else
                return value;
        }

        private static void OnShowValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnShowValueChanged((bool)e.OldValue, (bool)e.NewValue);
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
                UpdateValueControl();
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
        #region ValueOffset
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(GaugeControl), new UIPropertyMetadata(new Thickness(0, 0, 0, 29), new PropertyChangedCallback(OnValueOffsetChanged), new CoerceValueCallback(OnCoerceValueOffset)));

        private static object OnCoerceValueOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceValueOffset((Thickness)value);
            else
                return value;
        }

        private static void OnValueOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnValueOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
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
                UpdateValueControl();
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(GaugeControl), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceShowEngeneeringUnit)));

        private static object OnCoerceShowEngeneeringUnit(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceShowEngeneeringUnit((bool)value);
            else
                return value;
        }

        private static void OnShowEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnShowEngeneeringUnitChanged((bool)e.OldValue, (bool)e.NewValue);
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
                UpdateEUnitControl();
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
        #region EngeneeringOffset
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(GaugeControl), new UIPropertyMetadata(new Thickness(0, 0, 0, 25), new PropertyChangedCallback(OnEngeneeringOffsetChanged), new CoerceValueCallback(OnCoerceEngeneeringOffset)));

        private static object OnCoerceEngeneeringOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceEngeneeringOffset((Thickness)value);
            else
                return value;
        }

        private static void OnEngeneeringOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnEngeneeringOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
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
                UpdateEUnitControl();
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
        #endregion
        #region LabelStyle
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(ArcScaleLabelOrientation), typeof(GaugeControl), new UIPropertyMetadata(ArcScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceLableOrientation((ArcScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnLableOrientationChanged((ArcScaleLabelOrientation)e.OldValue, (ArcScaleLabelOrientation)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-46.0), new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceLabelOffset((Double)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnLabelOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        #region LabelZIndex
        public static readonly DependencyProperty LabelZIndexProperty = DependencyProperty.Register("LabelZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnLabelZIndexChanged), new CoerceValueCallback(OnCoerceLabelZIndex)));

        private static object OnCoerceLabelZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceLabelZIndex((int)value);
            else
                return value;
        }

        private static void OnLabelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnLabelZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        #region LabelStringFormat
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(GaugeControl), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
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
            {
                UpdateArcScale();
            }
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
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScale();
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
        new public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("ScaleFlowDirection", typeof(FlowDirection), typeof(GaugeControl), new UIPropertyMetadata(FlowDirection.LeftToRight));
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
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnEnableBackGroundLayerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        public static readonly DependencyProperty GaugeBaseModelProperty = DependencyProperty.Register("GaugeBaseModel", typeof(PredefinedBaseElementKinds), typeof(GaugeControl), new UIPropertyMetadata(PredefinedBaseElementKinds.Eco, new PropertyChangedCallback(OnGaugeBaseModelChanged), new CoerceValueCallback(OnCoerceGaugeBaseModel)));

        private static object OnCoerceGaugeBaseModel(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceGaugeBaseModel((PredefinedBaseElementKinds)value);
            else
                return value;
        }

        private static void OnGaugeBaseModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnGaugeBaseModelChanged((PredefinedBaseElementKinds)e.OldValue, (PredefinedBaseElementKinds)e.NewValue);
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
        public static readonly DependencyProperty ArcScaleFillProperty = DependencyProperty.Register("ArcScaleFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xD6, 0xD4, 0xD4)), new PropertyChangedCallback(OnArcScaleFillChanged), new CoerceValueCallback(OnCoerceArcScaleFill)));

        private static object OnCoerceArcScaleFill(DependencyObject o, object value)
        {
            GaugeControl clock = o as GaugeControl;
            if (clock != null)
                return clock.OnCoerceArcScaleFill((Brush)value);
            else
                return value;
        }

        private static void OnArcScaleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl clock = o as GaugeControl;
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
            {
                UpdateBackBorder();
                UpdateArcLayer();
            }
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
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(130.0), new PropertyChangedCallback(OnStartAngleChanged), new CoerceValueCallback(OnCoerceStartAngle)));

        private static object OnCoerceStartAngle(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceStartAngle((Double)value);
            else
                return value;
        }

        private static void OnStartAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnStartAngleChanged((Double)e.OldValue, (Double)e.NewValue);
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
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)410.0, new PropertyChangedCallback(OnEndAngleChanged), new CoerceValueCallback(OnCoerceEndAngle)));

        private static object OnCoerceEndAngle(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceEndAngle((Double)value);
            else
                return value;
        }

        private static void OnEndAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnEndAngleChanged((Double)e.OldValue, (Double)e.NewValue);
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
        public static readonly DependencyProperty MarkerVisibleProperty = DependencyProperty.Register("MarkerVisible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerVisibleChanged), new CoerceValueCallback(OnCoerceMarkerVisible)));

        private static object OnCoerceMarkerVisible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerVisible((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateMarkers();
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
        public static readonly DependencyProperty MarkerFactorHeightProperty = DependencyProperty.Register("MarkerFactorHeight", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)23, new PropertyChangedCallback(OnMarkerFactorHeightChanged), new CoerceValueCallback(OnCoerceMarkerFactorHeight)));

        private static object OnCoerceMarkerFactorHeight(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerFactorHeight((Double)value);
            else
                return value;
        }

        private static void OnMarkerFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerFactorHeightChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMarkerFactorHeight(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorHeightChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("GaugeStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerFactorWidthProperty = DependencyProperty.Register("MarkerFactorWidth", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)14, new PropertyChangedCallback(OnMarkerFactorWidthChanged), new CoerceValueCallback(OnCoerceMarkerFactorWidth)));

        private static object OnCoerceMarkerFactorWidth(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerFactorWidth((Double)value);
            else
                return value;
        }

        private static void OnMarkerFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerFactorWidthChanged((Double)e.OldValue, (Double)e.NewValue);
        }

        protected virtual Double OnCoerceMarkerFactorWidth(Double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFactorWidthChanged(Double oldValue, Double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("GaugeStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerZIndexProperty = DependencyProperty.Register("MarkerZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)100, new PropertyChangedCallback(OnMarkerZIndexChanged), new CoerceValueCallback(OnCoerceMarkerZIndex)));

        private static object OnCoerceMarkerZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerZIndex((int)value);
            else
                return value;
        }

        private static void OnMarkerZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateMarkers();
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
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-15.0), new PropertyChangedCallback(OnMarkerOffsetChanged), new CoerceValueCallback(OnCoerceMarkerOffset)));

        private static object OnCoerceMarkerOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerOffset((Double)value);
            else
                return value;
        }

        private static void OnMarkerOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateMarkers();
        }


        [Category("GaugeStyle")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnMarkerFillChanged), new CoerceValueCallback(OnCoerceMarkerFill)));

        private static object OnCoerceMarkerFill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerFill((Brush)value);
            else
                return value;
        }

        private static void OnMarkerFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceMarkerFill(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerFillChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well. 
            
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
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkRed), new PropertyChangedCallback(OnMarkerStrokeChanged), new CoerceValueCallback(OnCoerceMarkerStroke)));

        private static object OnCoerceMarkerStroke(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceMarkerStroke((Brush)value);
            else
                return value;
        }

        private static void OnMarkerStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
        public static readonly DependencyProperty MarkerAnimationEnableProperty = DependencyProperty.Register("MarkerAnimationEnable", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerAnimationEnableChanged), new CoerceValueCallback(OnCoerceMarkerAnimationEnable)));

        private static object OnCoerceMarkerAnimationEnable(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateMarkers();
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
        public static readonly DependencyProperty MarkerIsInteractiveProperty = DependencyProperty.Register("MarkerIsInteractive", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnMarkerIsInteractiveChanged), new CoerceValueCallback(OnCoerceMarkerIsInteractive)));

        private static object OnCoerceMarkerIsInteractive(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMarkerIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMarkerIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGuageControl();
                UpdateMarkers();
            }
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
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(GaugeControl), new UIPropertyMetadata(PredefinedElementKinds.Progressive, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnTickmarksPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty LineFillProperty = DependencyProperty.Register("LineFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnLineFillChanged), new CoerceValueCallback(OnCoerceLineFill)));

        private static object OnCoerceLineFill(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceLineFill((Brush)value);
            else
                return value;
        }

        private static void OnLineFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)0.5d, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkFactorLengthChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkFactorThicknessChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-27.0), new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkShowFirstChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorTickmarkShowLastChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MajorTickmarkFillProperty = DependencyProperty.Register("MajorTickmarkFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMajorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFill)));

        private static object OnCoerceMajorTickmarkFill(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceMajorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata(0.5d, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorTickmarkFactorLengthChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-27.0), new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorTickmarkOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorTickmarkFactorThicknessChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorTickmarkShowTicksForMajorChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorTickmarkFillProperty = DependencyProperty.Register("MinorTickmarkFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMinorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFill)));

        private static object OnCoerceMinorTickmarkFill(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceMinorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
                UpdateArcScale();
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
        public static readonly DependencyProperty SpindleCapPresentationProperty = DependencyProperty.Register("SpindleCapPresentation", typeof(PredefinedElementKinds), typeof(GaugeControl), new UIPropertyMetadata(PredefinedElementKinds.Eco, new PropertyChangedCallback(OnSpindleCapPresentationChanged), new CoerceValueCallback(OnCoerceSpindleCapPresentation)));

        private static object OnCoerceSpindleCapPresentation(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceSpindleCapPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnSpindleCapPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnSpindleCapPresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty SpindleFillProperty = DependencyProperty.Register("SpindleFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnSpindleFillChanged), new CoerceValueCallback(OnCoerceSpindleFill)));

        private static object OnCoerceSpindleFill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceSpindleFill((Brush)value);
            else
                return value;
        }

        private static void OnSpindleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnSpindleFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty SpindleFactorHeightProperty = DependencyProperty.Register("SpindleFactorHeight", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnSpindleFactorHeightChanged), new CoerceValueCallback(OnCoerceSpindleFactorHeight)));

        private static object OnCoerceSpindleFactorHeight(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceSpindleFactorHeight((Double)value);
            else
                return value;
        }

        private static void OnSpindleFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnSpindleFactorHeightChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty SpindleFactorWidthProperty = DependencyProperty.Register("SpindleFactorWidth", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnSpindleFactorWidthChanged), new CoerceValueCallback(OnCoerceSpindleFactorWidth)));

        private static object OnCoerceSpindleFactorWidth(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceSpindleFactorWidth((Double)value);
            else
                return value;
        }

        private static void OnSpindleFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnSpindleFactorWidthChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty SpindleCapZIndexProperty = DependencyProperty.Register("SpindleCapZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnSpindleCapZIndexChanged), new CoerceValueCallback(OnCoerceSpindleCapZIndex)));

        private static object OnCoerceSpindleCapZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceSpindleCapZIndex((int)value);
            else
                return value;
        }

        private static void OnSpindleCapZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnSpindleCapZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty NeedlePresentationProperty = DependencyProperty.Register("NeedlePresentation", typeof(PredefinedElementKinds), typeof(GaugeControl), new UIPropertyMetadata(PredefinedElementKinds.Default, new PropertyChangedCallback(OnNeedlePresentationChanged), new CoerceValueCallback(OnCoerceNeedlePresentation)));

        private static object OnCoerceNeedlePresentation(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedlePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnNeedlePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedlePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleFillProperty = DependencyProperty.Register("NeedleFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnNeedleFillChanged), new CoerceValueCallback(OnCoerceNeedleFill)));

        private static object OnCoerceNeedleFill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleFill((Brush)value);
            else
                return value;
        }

        private static void OnNeedleFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleIsInteractiveProperty = DependencyProperty.Register("NeedleIsInteractive", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnNeedleIsInteractiveChanged), new CoerceValueCallback(OnCoerceNeedleIsInteractive)));

        private static object OnCoerceNeedleIsInteractive(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGuageControl();
                UpdateNeedle();
            }
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
        public static readonly DependencyProperty NeedleVisibleProperty = DependencyProperty.Register("NeedleVisible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleVisibleChanged), new CoerceValueCallback(OnCoerceNeedleVisible)));

        private static object OnCoerceNeedleVisible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleVisible((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleAnimationEnableProperty = DependencyProperty.Register("NeedleAnimationEnable", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnNeedleAnimationEnableChanged), new CoerceValueCallback(OnCoerceNeedleAnimationEnable)));

        private static object OnCoerceNeedleAnimationEnable(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnNeedleAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleZIndexProperty = DependencyProperty.Register("NeedleZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)500, new PropertyChangedCallback(OnNeedleZIndexChanged), new CoerceValueCallback(OnCoerceNeedleZIndex)));

        private static object OnCoerceNeedleZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleZIndex((int)value);
            else
                return value;
        }

        private static void OnNeedleZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleStartOffsetProperty = DependencyProperty.Register("NeedleStartOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnNeedleStartOffsetChanged), new CoerceValueCallback(OnCoerceNeedleStartOffset)));

        private static object OnCoerceNeedleStartOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleStartOffset((Double)value);
            else
                return value;
        }

        private static void OnNeedleStartOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleStartOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty NeedleEndOffsetProperty = DependencyProperty.Register("NeedleEndOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)37.0, new PropertyChangedCallback(OnNeedleEndOffsetChanged), new CoerceValueCallback(OnCoerceNeedleEndOffset)));

        private static object OnCoerceNeedleEndOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceNeedleEndOffset((Double)value);
            else
                return value;
        }

        private static void OnNeedleEndOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnNeedleEndOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateNeedle();
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
        public static readonly DependencyProperty RangeBarVisibleProperty = DependencyProperty.Register("RangeBarVisible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarVisibleChanged), new CoerceValueCallback(OnCoerceRangeBarVisible)));

        private static object OnCoerceRangeBarVisible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarVisible((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarZIndexProperty = DependencyProperty.Register("RangeBarZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnRangeBarZIndexChanged), new CoerceValueCallback(OnCoerceRangeBarZIndex)));

        private static object OnCoerceRangeBarZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarZIndex((int)value);
            else
                return value;
        }

        private static void OnRangeBarZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarOffsetProperty = DependencyProperty.Register("RangeBarOffset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-90.0), new PropertyChangedCallback(OnRangeBarOffsetChanged), new CoerceValueCallback(OnCoerceRangeBarOffset)));

        private static object OnCoerceRangeBarOffset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarOffset((Double)value);
            else
                return value;
        }

        private static void OnRangeBarOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarFillProperty = DependencyProperty.Register("RangeBarFill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRangeBarFillChanged), new CoerceValueCallback(OnCoerceRangeBarFill)));

        private static object OnCoerceRangeBarFill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarFill((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarBackgroundProperty = DependencyProperty.Register("RangeBarBackground", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRangeBarBackgroundChanged), new CoerceValueCallback(OnCoerceRangeBarBackground)));

        private static object OnCoerceRangeBarBackground(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceRangeBarBackground((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarAnimationEnableProperty = DependencyProperty.Register("RangeBarAnimationEnable", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarAnimationEnableChanged), new CoerceValueCallback(OnCoerceRangeBarAnimationEnable)));

        private static object OnCoerceRangeBarAnimationEnable(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty RangeBarIsInteractiveProperty = DependencyProperty.Register("RangeBarIsInteractive", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarIsInteractiveChanged), new CoerceValueCallback(OnCoerceRangeBarIsInteractive)));

        private static object OnCoerceRangeBarIsInteractive(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGuageControl();
                UpdateRangeBars();
            }
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
        public static readonly DependencyProperty RangeBarThicknessProperty = DependencyProperty.Register("RangeBarThickness", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRangeBarThicknessChanged), new CoerceValueCallback(OnCoerceRangeBarThickness)));

        private static object OnCoerceRangeBarThickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRangeBarThickness((int)value);
            else
                return value;
        }

        private static void OnRangeBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRangeBarThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateRangeBars();
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
        public static readonly DependencyProperty Range1VisibleProperty = DependencyProperty.Register("Range1Visible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange1VisibleChanged), new CoerceValueCallback(OnCoerceRange1Visible)));

        private static object OnCoerceRange1Visible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange1VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1StartValueProperty = DependencyProperty.Register("Range1StartValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)40.0, new PropertyChangedCallback(OnRange1StartValueChanged), new CoerceValueCallback(OnCoerceRange1StartValue)));

        private static object OnCoerceRange1StartValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange1StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1EndValueProperty = DependencyProperty.Register("Range1EndValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)60.0, new PropertyChangedCallback(OnRange1EndValueChanged), new CoerceValueCallback(OnCoerceRange1EndValue)));

        private static object OnCoerceRange1EndValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange1EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1ThicknessProperty = DependencyProperty.Register("Range1Thickness", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange1ThicknessChanged), new CoerceValueCallback(OnCoerceRange1Thickness)));

        private static object OnCoerceRange1Thickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1Thickness((int)value);
            else
                return value;
        }

        private static void OnRange1ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1OffsetProperty = DependencyProperty.Register("Range1Offset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange1OffsetChanged), new CoerceValueCallback(OnCoerceRange1Offset)));

        private static object OnCoerceRange1Offset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1Offset((Double)value);
            else
                return value;
        }

        private static void OnRange1OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1FillProperty = DependencyProperty.Register("Range1Fill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Green), new PropertyChangedCallback(OnRange1FillChanged), new CoerceValueCallback(OnCoerceRange1Fill)));

        private static object OnCoerceRange1Fill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange1FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range1ZIndexProperty = DependencyProperty.Register("Range1ZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange1ZIndexChanged), new CoerceValueCallback(OnCoerceRange1ZIndex)));

        private static object OnCoerceRange1ZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange1ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange1ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange1ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange1();
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
        public static readonly DependencyProperty Range2VisibleProperty = DependencyProperty.Register("Range2Visible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange2VisibleChanged), new CoerceValueCallback(OnCoerceRange2Visible)));

        private static object OnCoerceRange2Visible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange2VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2StartValueProperty = DependencyProperty.Register("Range2StartValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)60.0, new PropertyChangedCallback(OnRange2StartValueChanged), new CoerceValueCallback(OnCoerceRange2StartValue)));

        private static object OnCoerceRange2StartValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange2StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2EndValueProperty = DependencyProperty.Register("Range2EndValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)80.0, new PropertyChangedCallback(OnRange2EndValueChanged), new CoerceValueCallback(OnCoerceRange2EndValue)));

        private static object OnCoerceRange2EndValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange2EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2ThicknessProperty = DependencyProperty.Register("Range2Thickness", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange2ThicknessChanged), new CoerceValueCallback(OnCoerceRange2Thickness)));

        private static object OnCoerceRange2Thickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2Thickness((int)value);
            else
                return value;
        }

        private static void OnRange2ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2OffsetProperty = DependencyProperty.Register("Range2Offset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange2OffsetChanged), new CoerceValueCallback(OnCoerceRange2Offset)));

        private static object OnCoerceRange2Offset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2Offset((Double)value);
            else
                return value;
        }

        private static void OnRange2OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2FillProperty = DependencyProperty.Register("Range2Fill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xEB, 0x79, 0x3C)), new PropertyChangedCallback(OnRange2FillChanged), new CoerceValueCallback(OnCoerceRange2Fill)));

        private static object OnCoerceRange2Fill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange2FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range2ZIndexProperty = DependencyProperty.Register("Range2ZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange2ZIndexChanged), new CoerceValueCallback(OnCoerceRange2ZIndex)));

        private static object OnCoerceRange2ZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange2ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange2ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange2ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange2();
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
        public static readonly DependencyProperty Range3VisibleProperty = DependencyProperty.Register("Range3Visible", typeof(Boolean), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange3VisibleChanged), new CoerceValueCallback(OnCoerceRange3Visible)));

        private static object OnCoerceRange3Visible(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange3VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3StartValueProperty = DependencyProperty.Register("Range3StartValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)80.0, new PropertyChangedCallback(OnRange3StartValueChanged), new CoerceValueCallback(OnCoerceRange3StartValue)));

        private static object OnCoerceRange3StartValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange3StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3EndValueProperty = DependencyProperty.Register("Range3EndValue", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)100.0, new PropertyChangedCallback(OnRange3EndValueChanged), new CoerceValueCallback(OnCoerceRange3EndValue)));

        private static object OnCoerceRange3EndValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange3EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3ThicknessProperty = DependencyProperty.Register("Range3Thickness", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnRange3ThicknessChanged), new CoerceValueCallback(OnCoerceRange3Thickness)));

        private static object OnCoerceRange3Thickness(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3Thickness((int)value);
            else
                return value;
        }

        private static void OnRange3ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3OffsetProperty = DependencyProperty.Register("Range3Offset", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)(-29.0), new PropertyChangedCallback(OnRange3OffsetChanged), new CoerceValueCallback(OnCoerceRange3Offset)));

        private static object OnCoerceRange3Offset(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3Offset((Double)value);
            else
                return value;
        }

        private static void OnRange3OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3FillProperty = DependencyProperty.Register("Range3Fill", typeof(Brush), typeof(GaugeControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRange3FillChanged), new CoerceValueCallback(OnCoerceRange3Fill)));

        private static object OnCoerceRange3Fill(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange3FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateArcScaleRange3();
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
        public static readonly DependencyProperty Range3ZIndexProperty = DependencyProperty.Register("Range3ZIndex", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange3ZIndexChanged), new CoerceValueCallback(OnCoerceRange3ZIndex)));

        private static object OnCoerceRange3ZIndex(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceRange3ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange3ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnRange3ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScaleRange3();
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
        #region MajorIntervalCount
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMajorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(GaugeControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnMinorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateArcScale();
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
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(GaugeControl), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                return GaugeControl.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl GaugeControl = o as GaugeControl;
            if (GaugeControl != null)
                GaugeControl.OnValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
            {
                isChangingValue = true;
                UpdateValue();
            }
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


        #region AutoHideElements
        public static readonly DependencyProperty AutoHideElementsProperty = DependencyProperty.Register("AutoHideElements", typeof(bool), typeof(GaugeControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideElementsChanged), new CoerceValueCallback(OnCoerceAutoHideElements)));

        private static object OnCoerceAutoHideElements(DependencyObject o, object value)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                return control.OnCoerceAutoHideElements((bool)value);
            else
                return value;
        }

        private static void OnAutoHideElementsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            GaugeControl control = o as GaugeControl;
            if (control != null)
                control.OnAutoHideElementsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAutoHideElements(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAutoHideElementsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
            {
                SetMinGridDimension();
                ManageAutoUpdatingElements(true);
            }
        }

        public bool AutoHideElements
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AutoHideElementsProperty);
            }
            set
            {
                SetValue(AutoHideElementsProperty, value);
            }
        }

        #endregion

        
        #region ThumbFactor
        public static readonly DependencyProperty GaugeTypeProperty = DependencyProperty.Register("GaugeType", typeof(string), typeof(GaugeControl), new UIPropertyMetadata("Full"));
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
        private DelayedSingleActionInvoker SizeChangedInvoker;
        IStringEditorManager stringManager;

        internal int CustomBaseIndex
        {
            get { return 12; }
        }
        
        bool previousClipToBounds;
        bool errorEffectOn;
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GaugesAutomationPeer(this);
        }

        public void CircularGaugeInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion

        #region Constructor
        public GaugeControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            if (circularGaugeObject != null)
                circularGaugeObject.DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    SetMinGridDimension();

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

                    if (SizeChangedInvoker == null)
                        SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                        {
                            if (!bDispose && AutoHideElements)
                            {
                                ManageAutoUpdatingElements();
                            }
                        });

                    SizeChanged += OnSizeChanged;
                    UpdateGaugeLayout();
                    CheckInteractivity();
                    bInit = true;
                }
            };
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!bDispose && SizeChangedInvoker != null)
                SizeChangedInvoker.BeginInvoke();
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

        #region override Methods
        protected override void SetEntityError(String error)
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
        protected override void UpdateScaleRanges()
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
        protected override void UpdateScaleStartEndValues()
        {
            if (arcScale != null)
            {
                arcScale.StartValue = _StartValue;
                arcScale.EndValue = _EndValue;
            }
        }
        protected override void ShowMarker(bool showWarning)
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
                    SetArcNeedleVisibility();
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
        protected override void UpdateCustomElements()
        {
            UpdateEUnitControl();
            UpdateValueControl();
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
        double GetDelta()
        {
            Size elSize = GetLayoutSize(this);
            double dimension = Math.Min(elSize.Width, elSize.Height);
            return AutoHideElements && Properties.Settings.Default.MinControlSize > 0 && dimension <= Properties.Settings.Default.MinControlSize ? (dimension / Properties.Settings.Default.MinControlSize) : 1;
        }
        private void ManageAutoUpdatingElements(bool bForceAutoHide = false)
        {
            if (!AutoHideElements && !bForceAutoHide)
                return;
            if (circularGaugeObject == null)
                return;
            double deltaP = GetDelta();
            SetScaleLabelFormat(deltaP);
            SetCustomElementVisibility(gridUnitContainer, deltaP);
            SetCustomElementVisibility(gridValueContainer, deltaP);
            SetArcLayerVisibility(deltaP);
            SetArcNeedleVisibility(deltaP);
            SetArcRangeVisibility(deltaP);
        }
        private void SetMinGridDimension()
        {
            if (!AutoHideElements)
            {
                container.MinWidth = Properties.Settings.Default.MinControlVisibility;
                container.MinHeight = Properties.Settings.Default.MinControlVisibility;
            }
            else
            {
                container.ClearValue(Grid.MinHeightProperty);
                container.ClearValue(Grid.MinWidthProperty);
            }
        }
        void SetScaleLabelFormat(double? deltaP = null)
        {
            if (arcScale == null)
                return;

            if (!deltaP.HasValue)
                deltaP = GetDelta();
            arcScale.LabelOptions.FormatString = deltaP < Properties.Settings.Default.MinControlSizePrecVisibilityB ? "" : LabelStringFormat;
            arcScale.MajorIntervalCount = MajorIntervalCount * deltaP > 1 ? (int)(MajorIntervalCount * deltaP) : 1;
            arcScale.SpindleCapOptions.FactorHeight = (double)(SpindleFactorHeight * deltaP);
            arcScale.SpindleCapOptions.FactorWidth = (double)(SpindleFactorWidth * deltaP);
        }
        void SetCustomElementVisibility(ScaleCustomElement container, double? deltaP = null)
        {
            if (container == null)
                return;

            if (!deltaP.HasValue)
                deltaP = GetDelta();
            container.Visibility = deltaP < Properties.Settings.Default.MinControlSizePrecVisibilityA ? Visibility.Collapsed : Visibility.Visible;
        }
        void SetArcLayerVisibility(double? deltaP = null)
        {
            if (arcLayer == null)
                return;
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            arcLayer.Visible = deltaP < Properties.Settings.Default.MinControlSizePrecVisibilityD ? false : true;
        }
        void SetArcNeedleVisibility(double? deltaP = null)
        {
            if (arcNeedle == null)
                return;
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            arcNeedle.Visible = deltaP < Properties.Settings.Default.MinControlSizePrecVisibilityC ? false : true; ;
        }
        private void SetArcRangeVisibility(double? deltaP = null)
        {
            if (arcRange == null)
                return;
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            arcRange.Visible = deltaP < Properties.Settings.Default.MinControlSizePrecVisibilityE ? false : true;
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
                UpdateCustomValue(Value);
                UpdateEUnitControl();
                if (arcScale != null)
                {
                    arcScale.LabelOptions.FormatString = "";
                    SetScaleLabelFormat();
                }
            });
        }
        private double GetOffset(FontSettings newValue, FontSettings oldValue, double oldOffset)
        {
            TextBlock label = new TextBlock()
            {
                FontFamily = oldValue.FontFamily,
                FontSize = oldValue.FontSize,
                FontStyle = oldValue.FontStyle,
                FontWeight = oldValue.FontWeight,
                Text = string.Format(LabelStringFormat, $"{EndValue}"),
            };
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size oldSize = GetLayoutSize(label);

            label.FontFamily = newValue.FontFamily;
            label.FontSize = newValue.FontSize;
            label.FontStyle = newValue.FontStyle;
            label.FontWeight = newValue.FontWeight;
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size newSize = GetLayoutSize(label);
            double newOffset;

            if (oldOffset < 0)
                newOffset = oldOffset + ((oldSize.Width - newSize.Width) / 2);
            else if (oldOffset > 0)
                newOffset = oldOffset + ((newSize.Width - oldSize.Width) / 2);
            else
                newOffset = oldOffset;

            return newOffset;
        }
        protected Size GetLayoutSize(FrameworkElement fe)
        {
            double width;
            double height;
            if (!double.IsNaN(fe.Width) && !double.IsInfinity(fe.Width))
            {
                width = fe.Width;
            }
            else if ((fe.ActualWidth == 0.0) && (fe.DesiredSize.Width > 0.0))
            {
                width = fe.DesiredSize.Width;
            }
            else
            {
                width = fe.ActualWidth;
            }
            if (!double.IsNaN(fe.Height) && !double.IsInfinity(fe.Height))
            {
                height = fe.Height;
            }
            else if ((fe.ActualHeight == 0.0) && (fe.DesiredSize.Height > 0.0))
            {
                height = fe.DesiredSize.Height;
            }
            else
            {
                height = fe.ActualHeight;
            }
            return new Size(width, height);
        }
        private void UpdateGaugeLayout()
        {
            if (bDispose || bDataContextChanging)
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
            isChangingValue = true;
            UpdateValue();
        }

        private void UpdateBaseArcScale()
        {
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

        void UpdateEUnitControl()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
            string eunit = TranslationHelpers.TranslationHelper.TranslateComposedText(EngeneeringUnit, stringlist, EngeneeringUnit);
            string meas = ConverterLabel ?? eunit;
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

                    if (_text != null)
                        _grid.Children.Add(_text);
                    _grid.SetBinding(Grid.MarginProperty, offsetBinding);
                    gridUnitContainer.Content = _grid;

                    if (!arcScale.CustomElements.Contains(gridUnitContainer))
                        arcScale.CustomElements.Add(gridUnitContainer);
                }
                Grid grid = (Grid)gridUnitContainer.Content;
                TextBlock text = grid.GetChildrenOfType<TextBlock>().FirstOrDefault();
                if (text != null)
                    text.Text = meas;

                SetCustomElementVisibility(gridUnitContainer);
            }
        }

        void UpdateValueControl()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
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

                    // _text.SetBinding(TextBlock.MarginProperty, offsetBinding);

                    if (_text != null)
                        _grid.Children.Add(_text);
                    gridValueContainer.Content = _grid;

                    if (!arcScale.CustomElements.Contains(gridValueContainer))
                        arcScale.CustomElements.Add(gridValueContainer);
                    if(bInit)
                        UpdateValue();
                }

                SetCustomElementVisibility(gridValueContainer);
            }
        }
        SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        private void UpdateRangeBars()
        {
            if (circularGaugeObject == null || arcBaseScale == null || arcScale == null)
                return;
            bool updateValue = false;
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

                SetArcRangeVisibility();
                updateValue = true;
            }
            if(updateValue && bInit)
                UpdateValue();
        }

        private void UpdateMarkers()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

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

        private void UpdateArcScaleRange1()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

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
        private void UpdateArcScaleRange2()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

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
        private void UpdateArcScaleRange3()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
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

        private void UpdateNeedle()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            if (NeedleVisible)
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
                SetArcNeedleVisibility();
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
        private void arcNeedle_ValueChanged(object sender, ValueChangedEventArgs ev)
        {
            if (bDesign)
                return;

            if (!isChangingValue || bDesign)
                    Value = ev.NewValue;
        }

        private void UpdateArcLayer()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

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
                SetArcLayerVisibility();
            }
        }
        private void UpdateArcScale()
        {
            if (circularGaugeObject == null)
                return;

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
            SetScaleLabelFormat();
        }

        object LoadTemplate(string template)
        {
            try
            {
                string basename = "LabelTemplate.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(GaugeControl).Namespace, basename));
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
            if (circularGaugeObject == null)
            {
                circularGaugeObject = new CircularGaugeControl()
                {
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

            circularGaugeObject.Foreground = LabelForeground;
            circularGaugeObject.FontStyle = LabelFontSettings.FontStyle;
            circularGaugeObject.FontFamily = LabelFontSettings.FontFamily;
            circularGaugeObject.FontWeight = LabelFontSettings.FontWeight;
            circularGaugeObject.FontSize = LabelFontSettings.FontSize;
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
                Viewbox content = (Viewbox)canvas.TryFindResource(string.Format("{0}_{1}", GaugeBaseModel.ToString(), GetArcScaleBackgroundIndex()));
                return (Viewbox)content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal string LoadSVGBackContent()
        {
            return string.Format("{0}_{1}", GaugeBaseModel.ToString(), GetArcScaleBackgroundIndex());
        }

        internal string LoadSVGGaugeType()
        {
            return string.Format("{0}", GetArcScaleBackgroundIndex());
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
                if (isChangingValue || bDesign)
                    arcNeedle.Value = _value;
            }

            if (arcRange != null)
            {
                if (isChangingValue || bDesign)
                    arcRange.Value = _value;
                
            }

            if (arcScaleMarker != null)
            {
                if (isChangingValue || bDesign)
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
            isChangingValue = false;
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
                var predefineds = (ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).Skip((int)value * 5).Take(5);
                PredefinedElementKind gaugeModelKind = predefineds.FirstOrDefault() as PredefinedElementKind;
                string filter = string.Empty;
                if (index > 0)
                {
                    switch (index)
                    {
                        case 1:
                            filter = "Half";
                            break;
                        case 2:
                            filter = "Right";
                            break;
                        case 3:
                            filter = "Left";
                            break;
                        case 4:
                            filter = "Three";
                            break;
                        default:
                            break;
                    }
                    gaugeModelKind = (from p in predefineds where p.Name.Contains(filter) select p).FirstOrDefault() as PredefinedElementKind;
                }
                //PredefinedElementKind gaugeModelKind = (ArcScaleLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value * 5  GetArcScaleBackgroundIndex()) as PredefinedElementKind;                return (ArcScaleLayerPresentation)Activator.CreateInstance(gaugeModelKind.Type);
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
            PredefinedArcScaleLayerPresentation layer = (PredefinedArcScaleLayerPresentation)arcLayer.Presentation;
            if (layer != null)
                layer.Fill = newValue is SolidColorBrush && (newValue as SolidColorBrush).Color.A == 0 ? null : newValue;
        }

        public PredefinedAspect GetArcScaleBackgroundIndex()
        {
            return GetArcScaleBackgroundIndex(StartAngle, EndAngle, null);
        }

        internal static PredefinedAspect GetArcScaleBackgroundIndex(double startAngle, double endAngle, ScaleOptions scaleOptions = null)
        {
            PredefinedAspect index = PredefinedAspect.Full;

            if (Math.Abs(startAngle - endAngle) >= 360)
            {
                if(scaleOptions != null)
                {
                    scaleOptions.PredefinedAspect = index;
                    scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                    scaleOptions.VerticalAlignment = VerticalAlignment.Stretch;
                    scaleOptions.ScaleFactor = 1;
                }
            }
            else
            {
                var dimAngle = Math.Abs(endAngle - startAngle);
                if (Math.Abs(startAngle) > 360)
                    startAngle = startAngle % 360;
                if (Math.Abs(endAngle) > 360)
                    endAngle = endAngle % 360;

                //Full = 0,
                //Half = 1,
                //Rightquarter = 2,
                //Leftquarter = 3,
                //Threequarter = 4

                if (dimAngle > 0)
                {
                    if (dimAngle <= 90)
                    {
                        if ((startAngle >= 180 && startAngle <= 270 && endAngle <= 270) ||
                            (startAngle >= -180 && startAngle <= -90 && endAngle <= -90))
                        {
                            index = PredefinedAspect.Leftquarter;
                            if (scaleOptions != null)
                            {
                                scaleOptions.PredefinedAspect = index;
                                scaleOptions.HorizontalAlignment = scaleOptions.UseAlwaysFullBackLayer ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;
                                scaleOptions.VerticalAlignment = scaleOptions.UseAlwaysFullBackLayer ? VerticalAlignment.Top : VerticalAlignment.Stretch;
                                scaleOptions.ScaleFactor = scaleOptions.UseAlwaysFullBackLayer ? 0.5 : 1;
                            }
                        }
                        else if ((startAngle >= 270 && startAngle <= 360 && endAngle <= 360) ||
                                 (startAngle >= -90 && startAngle <= 0 && endAngle <= 0))
                        {
                            index = PredefinedAspect.Rightquarter;
                            if (scaleOptions != null)
                            {
                                scaleOptions.PredefinedAspect = index;
                                scaleOptions.HorizontalAlignment = scaleOptions.UseAlwaysFullBackLayer ? HorizontalAlignment.Right : HorizontalAlignment.Stretch;
                                scaleOptions.VerticalAlignment = scaleOptions.UseAlwaysFullBackLayer ? VerticalAlignment.Top : VerticalAlignment.Stretch;
                                scaleOptions.ScaleFactor = scaleOptions.UseAlwaysFullBackLayer ? 0.5 : 1;
                            }
                        }
                        else if ((startAngle >= 180 && startAngle <= 270 && endAngle > 270) ||
                                 (startAngle >= -180 && startAngle <= -90 && endAngle > -90))
                        {
                            index = PredefinedAspect.Half;
                            if (scaleOptions != null)
                            {
                                scaleOptions.PredefinedAspect = index;
                                scaleOptions.HorizontalAlignment = scaleOptions.UseAlwaysFullBackLayer ? HorizontalAlignment.Stretch : HorizontalAlignment.Stretch;
                                scaleOptions.VerticalAlignment = scaleOptions.UseAlwaysFullBackLayer ? VerticalAlignment.Top : VerticalAlignment.Center;
                                scaleOptions.ScaleFactor = 1;
                            }
                        }
                        else
                        {
                            index = PredefinedAspect.Full;
                            if (scaleOptions != null)
                            {
                                scaleOptions.PredefinedAspect = index;
                                if ((startAngle >= 0 && startAngle <= 90 && endAngle <= 90) ||
                                    (startAngle >= -360 && startAngle <= -270 && endAngle <= -270))
                                {
                                    scaleOptions.HorizontalAlignment = HorizontalAlignment.Right;
                                    scaleOptions.VerticalAlignment = VerticalAlignment.Bottom;
                                    scaleOptions.ScaleFactor = 0.5;
                                }
                                else if ((startAngle >= 90 && startAngle <= 180 && endAngle <= 180) ||
                                    (startAngle >= -270 && startAngle <= -180 && endAngle <= -180))
                                {
                                    scaleOptions.HorizontalAlignment = HorizontalAlignment.Left;
                                    scaleOptions.VerticalAlignment = VerticalAlignment.Bottom;
                                    scaleOptions.ScaleFactor = 0.5;
                                }
                                else
                                {
                                    scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    scaleOptions.VerticalAlignment = VerticalAlignment.Stretch;
                                    scaleOptions.ScaleFactor = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dimAngle <= 180)
                        {
                            if ((startAngle >= 180 && startAngle <= 270 && endAngle > 270) ||
                                 (startAngle >= -180 && startAngle <= -90 && endAngle > -90))
                            {
                                index = PredefinedAspect.Half;
                                if (scaleOptions != null)
                                {
                                    scaleOptions.PredefinedAspect = index;
                                    scaleOptions.HorizontalAlignment = scaleOptions.UseAlwaysFullBackLayer ? HorizontalAlignment.Stretch : HorizontalAlignment.Stretch;
                                    scaleOptions.VerticalAlignment = scaleOptions.UseAlwaysFullBackLayer ? VerticalAlignment.Top : VerticalAlignment.Center;
                                    scaleOptions.ScaleFactor = 1;
                                }
                            }
                            else
                            {
                                index = PredefinedAspect.Threequarter;
                                if (scaleOptions != null)
                                {
                                    scaleOptions.PredefinedAspect = index;
                                    scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    scaleOptions.VerticalAlignment = VerticalAlignment.Top;
                                    scaleOptions.ScaleFactor = 1;
                                }
                            }
                        }
                        else
                        {
                            if (dimAngle <= 270)
                            {
                                if ((startAngle >= 134 && endAngle <= 45) ||
                                    (startAngle >= -226 && endAngle <= 45))
                                {
                                    index = PredefinedAspect.Threequarter;
                                    if (scaleOptions != null)
                                    {
                                        scaleOptions.PredefinedAspect = index;
                                        scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                                        scaleOptions.VerticalAlignment = VerticalAlignment.Top;
                                        scaleOptions.ScaleFactor = 1;
                                    }
                                }
                                else
                                {
                                    index = PredefinedAspect.Full;
                                    if (scaleOptions != null)
                                    {
                                        scaleOptions.PredefinedAspect = index;
                                        scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                                        scaleOptions.VerticalAlignment = VerticalAlignment.Stretch;
                                        scaleOptions.ScaleFactor = 1;
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {
                    index = PredefinedAspect.Full;
                    if (scaleOptions != null)
                    {
                        scaleOptions.PredefinedAspect = index;
                        scaleOptions.HorizontalAlignment = HorizontalAlignment.Stretch;
                        scaleOptions.VerticalAlignment = VerticalAlignment.Stretch;
                        scaleOptions.ScaleFactor = 1;
                    }
                }
            }

            return index;

        }

        private void CheckInteractivity()
        {
            if (RunningOnServer && !MarkerIsInteractive && !RangeBarIsInteractive && !NeedleIsInteractive)
                IsHitTestVisible = false;
        }
        #endregion

        #region IDisposable
        public override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            base.Dispose(true);

            SizeChanged -= OnSizeChanged;
            SizeChangedInvoker = null;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;


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
                arcScale.Ranges.Clear();
                arcScale.Markers.Clear();
                arcScale.Ranges.Clear();
                arcScale.CustomElements.Clear();
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
                    (document, typeof(RangeBase), EngeneeringUnitProperty).DisplayName;
                map.Add(propertyName, EngeneeringUnit);
            }
            return map;
        }
        #endregion
    }

    internal class ConvertSVGHasStyles : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null)
                return null;

            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is GaugeControl)
            {
                GaugeControl gauge = sender as GaugeControl;

                var entity = gauge.GaugeBaseModel;
                
                if (entity != PredefinedBaseElementKinds.None && gauge.EnableBackGroundLayer)
                    ret.Add("ArcScaleFill", gauge.ArcScaleFill);
                ret.Add("SpindleFill", gauge.SpindleFill);
            }
            else if (sender is CircularGauge)
            {
                CircularGauge gauge = sender as CircularGauge;

                var entity = gauge.GaugeBaseModel;

                if (entity != PredefinedBaseElementKinds.None && gauge.EnableBackGroundLayer)
                    ret.Add("ArcScaleFill", gauge.ArcScaleFill);
                ret.Add("SpindleFill", gauge.SpindleFill);
            }
            else if (sender is FastGaugeControl)
            {
                FastGaugeControl gauge = sender as FastGaugeControl;
                var entity = gauge.MeterType;
                if (entity != FastMeterTypes.None)
                    ret.Add("ArcScaleFill", gauge.Background);
                ret.Add("SpindleFill", gauge.CapColor);
            }

            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            Dictionary<string, string> ret = new Dictionary<string, string>();
            if (sender is GaugeControl)
            {
                GaugeControl gauge = sender as GaugeControl;

                var entity = (PredefinedBaseElementKinds)gauge.GaugeBaseModel;
                if (entity != PredefinedBaseElementKinds.None && gauge.EnableBackGroundLayer)
                    ret.Add(gauge.GaugeBaseModel.ToString(), gauge.LoadSVGBackContent());
                ret.Add($"SpindleCap_{gauge.SpindleCapPresentation.ToString()}", $"SpindleCap_{gauge.SpindleCapPresentation.ToString()}");
            }
            else if (sender is CircularGauge)
            {
                CircularGauge gauge = sender as CircularGauge;

                var entity = (PredefinedBaseElementKinds)gauge.GaugeBaseModel;
                if (entity != PredefinedBaseElementKinds.None && gauge.EnableBackGroundLayer)
                    ret.Add(gauge.GaugeBaseModel.ToString(), gauge.LoadSVGBackContent());
                ret.Add($"SpindleCap_{gauge.SpindleCapPresentation.ToString()}", $"SpindleCap_{gauge.SpindleCapPresentation.ToString()}");
            }
            else if (sender is FastGaugeControl)
            {
                FastGaugeControl gauge = sender as FastGaugeControl;

                var entity = (FastMeterTypes)gauge.MeterType;
                //if (entity != FastMeterTypes.None)
                    ret.Add(gauge.MeterType.ToString(), gauge.GaugeBaseModel);
                ret.Add($"SpindleCap_{gauge.CapType.ToString()}", $"SpindleCap_{gauge.CapType.ToString()}");
            }

            return ret;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, XmlElement>);
            }
        }
    }
    internal class ConvertGaugeBaseModel : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            if (sender is GaugeControl)
            {
                var entity = (PredefinedBaseElementKinds)value;
                if (entity == PredefinedBaseElementKinds.None)
                    return null;
                GaugeControl gauge = sender as GaugeControl;

                if (!gauge.EnableBackGroundLayer)
                    return null;
                else
                    return gauge.LoadSVGBackContent();

            }
            else if (sender is CircularGauge)
            {
                var entity = (PredefinedBaseElementKinds)value;
                if (entity == PredefinedBaseElementKinds.None)
                    return null;
                CircularGauge gauge = sender as CircularGauge;

                if (!gauge.EnableBackGroundLayer)
                    return null;
                else
                    return gauge.LoadSVGBackContent();
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
    
    internal class ConvertSpindleFactor : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is FastGaugeControl)
            {
                FastGaugeControl gauge = sender as FastGaugeControl;
                return gauge.CapRadius.Value;
            }
            else
            {
                var entity = (Double)value;
                return entity * Properties.Settings.Default.SpindleFactor;
            }
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertSpindleCapPresentation : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null; 
            
            if (sender is FastGaugeControl)
            {
                FastGaugeControl gauge = sender as FastGaugeControl;
                var entity = (PointerCapType)gauge.CapType;
                return $"SpindleCap_{entity.ToString()}"; 
            }
            else
            {
                var entity = (PredefinedElementKinds)value;
                return $"SpindleCap_{entity.ToString()}";
            }
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertBrushToSvgValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null)
                return null;
            if ((sender is GaugeControl))
                return new Dictionary<string, Brush>() {
                            { "RangeBarFill", (sender as GaugeControl).RangeBarFill },
                            { "NeedleFill", (sender as GaugeControl).NeedleFill }
                        };
            else if ((sender is CircularGauge))
                return new Dictionary<string, Brush>() {
                            { "RangeBarFill", (sender as CircularGauge).RangeBarFill },
                            { "NeedleFill", (sender as CircularGauge).NeedleFill }
                        };
            else if ((sender is FastGaugeControl))
                return new Dictionary<string, Brush>() {
                            { "RangeBarFill", (sender as FastGaugeControl).BarColor },
                            { "NeedleFill", (sender as FastGaugeControl).NeedleColor }
                        };
            else return null;
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



    class ConvertGaugeType : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is GaugeControl)
                return (sender as GaugeControl).LoadSVGGaugeType();
            else if (sender is CircularGauge)
                return (sender as CircularGauge).LoadSVGGaugeType();
            else return null;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    class ConvertScaleFlowDirection : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is GaugeControl)
                return (sender as GaugeControl).StartAngle < (sender as GaugeControl).EndAngle ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            else if (sender is CircularGauge)
                return (sender as CircularGauge).StartAngle < (sender as CircularGauge).EndAngle ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            else return null;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertScaleOptions : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            double startAngle = 0;
            double endAngle = 0;
            ScaleOptions scaleOptions = new ScaleOptions()
            {
                PredefinedAspect = PredefinedAspect.Full,
                ScaleFactor = 1,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            if (sender is GaugeControl)
            {
                GaugeControl control = (sender as GaugeControl);
                startAngle = control.StartAngle;
                endAngle = control.EndAngle;
            }
            else if (sender is CircularGauge)
            {
                CircularGauge control = (sender as CircularGauge);
                startAngle = control.StartAngle;
                endAngle = control.EndAngle;
            }
            else if (sender is FastGaugeControl)
            {
                FastGaugeControl control = (sender as FastGaugeControl);
                startAngle = control.MeterScaleStartAngle;
                endAngle = control.EndAngle;
                scaleOptions.UseAlwaysFullBackLayer = true;
            }
            GaugeControl.GetArcScaleBackgroundIndex(startAngle, endAngle, scaleOptions);
            return new Dictionary<string, object>() {
                            { "BackLayerAspect", scaleOptions.PredefinedAspect.ToString() },
                            { "HorizontalAlignment", scaleOptions.HorizontalAlignment.ToString() },
                            { "VerticalAlignment", scaleOptions.VerticalAlignment.ToString() },
                            { "ScaleFactor", scaleOptions.ScaleFactor }
                        };
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ScaleOptions
    {
        public PredefinedAspect PredefinedAspect { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public double ScaleFactor { get; set; }
        public bool UseAlwaysFullBackLayer { get; set; }
    }
}
