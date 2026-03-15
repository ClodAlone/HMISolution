using System;
using System.Runtime.Serialization;
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
using WPFUtilities;
using System.Windows.Threading;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using System.Threading.Tasks;
using StringManager.ComponentService;
using RangeBaseControl;
using System.Xml.Serialization;

namespace Meters
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [SvgValueConverter(ConverterType = typeof(ConvertSVGHasStyles), HasStyles = true, HasBrushes = true)]
    public partial class LinearMeterControl : RangeBaseControl.RangeBaseControl, IContainPropertyEditors, IStringIDAware
    {
        #region Dependency Properties
        #region LevelBackgroundMargin
        public static readonly DependencyProperty LevelBackgroundMarginProperty = DependencyProperty.Register("LevelBackgroundMargin", typeof(Thickness), typeof(LinearMeterControl), new UIPropertyMetadata(new Thickness(0,-10,0,-10)));
        [Browsable(false)]
        [XmlIgnore]
        public Thickness LevelBackgroundMargin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(LevelBackgroundMarginProperty);
            }
            set
            {
                SetValue(LevelBackgroundMarginProperty, value);
            }
        }
        #endregion


        #region LevelOptions
        public static readonly DependencyProperty LevelOptionsProperty = DependencyProperty.Register("LevelOptions", typeof(LevelOptions), typeof(LinearMeterControl), new UIPropertyMetadata(null));
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
                thickness = RangeBarThickness/6;
            }

            //if (LinearBaseModel == PredefinedBaseElementKinds.Termometer || LinearBaseModel == PredefinedBaseElementKinds.Termometer1)
            //    thickness = thickness - 3;

            return new LevelOptions()
                    {
                        Offset = offset,
                        Thickness = thickness
            };
        }
        #endregion


        #region LevelBackgroundTemplate
        public static readonly DependencyProperty LevelBackgroundTemplateProperty = DependencyProperty.Register("LevelBackgroundTemplate", typeof(string), typeof(LinearMeterControl), new UIPropertyMetadata("Bar"));
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

        #region ValueStyle

        #region ShowValue
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.Register("ShowValue", typeof(bool), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowValueChanged), new CoerceValueCallback(OnCoerceShowValue)));

        private static object OnCoerceShowValue(DependencyObject o, object value)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowValue((bool)value);
            else
                return value;
        }

        private static void OnShowValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
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
                UpdateValueControl();
        }


        [Category("Meter Value Style")]
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
        public static readonly DependencyProperty ValueOffsetProperty = DependencyProperty.Register("ValueOffset", typeof(Thickness), typeof(LinearMeterControl), new UIPropertyMetadata(new Thickness(90, 0, 0, 0), new PropertyChangedCallback(OnValueOffsetChanged), new CoerceValueCallback(OnCoerceValueOffset)));

        private static object OnCoerceValueOffset(DependencyObject o, object value)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
            if (circularGauge != null)
                return circularGauge.OnCoerceValueOffset((Thickness)value);
            else
                return value;
        }

        private static void OnValueOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
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
                UpdateValueControl();
        }
        [Category("Meter Value Style")]
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(LinearMeterControl), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
                UpdateValueControl();
        }
        [Category("Meter Value Style")]
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
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnEnableBackGroundLayerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        public static readonly DependencyProperty LinearBaseModelProperty = DependencyProperty.Register("LinearBaseModel", typeof(PredefinedBaseElementKinds), typeof(LinearMeterControl), new UIPropertyMetadata(PredefinedBaseElementKinds.Progressive, new PropertyChangedCallback(OnLinearBaseModelChanged), new CoerceValueCallback(OnCoerceLinearBaseModel)));

        private static object OnCoerceLinearBaseModel(DependencyObject o, object value)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
            if (circularGauge != null)
                return circularGauge.OnCoerceLinearBaseModel((PredefinedBaseElementKinds)value);
            else
                return value;
        }

        private static void OnLinearBaseModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
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
        [Category("Meter Style")]
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
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnBackgroundChanged), new CoerceValueCallback(OnCoerceBackground)));

        private static object OnCoerceBackground(DependencyObject o, object value)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
            if (linearMeter != null)
                return linearMeter.OnCoerceBackground((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
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
        [Category("Meter Style")]
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
        public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(LinearScaleLayoutMode), typeof(LinearMeterControl), new UIPropertyMetadata(LinearScaleLayoutMode.BottomToTop, new PropertyChangedCallback(OnLayoutModeChanged), new CoerceValueCallback(OnCoerceLayoutMode)));

        private static object OnCoerceLayoutMode(DependencyObject o, object value)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
            if (linearMeter != null)
                return linearMeter.OnCoerceLayoutMode((LinearScaleLayoutMode)value);
            else
                return value;
        }

        private static void OnLayoutModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
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
            if (oldValue != newValue && (bLoaded && bInit) && linearScale != null)
                linearScale.LayoutMode = LayoutMode;
        }
         [Category("Meter Style")]
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
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(LinearScaleLabelOrientation), typeof(LinearMeterControl), new UIPropertyMetadata(LinearScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLableOrientation((LinearScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLableOrientationChanged((LinearScaleLabelOrientation)e.OldValue, (LinearScaleLabelOrientation)e.NewValue);
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

        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-30, new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLabelOffset((int)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLabelOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Scale Style")]
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
        #region LabelZIndex
        public static readonly DependencyProperty LabelZIndexProperty = DependencyProperty.Register("LabelZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnLabelZIndexChanged), new CoerceValueCallback(OnCoerceLabelZIndex)));

        private static object OnCoerceLabelZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLabelZIndex((int)value);
            else
                return value;
        }

        private static void OnLabelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLabelZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(LinearMeterControl), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
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
        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty ShowEngeneeringUnitProperty = DependencyProperty.Register("ShowEngeneeringUnit", typeof(bool), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceShowEngeneeringUnit)));

        private static object OnCoerceShowEngeneeringUnit(DependencyObject o, object value)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
            if (circularGauge != null)
                return circularGauge.OnCoerceShowEngeneeringUnit((bool)value);
            else
                return value;
        }

        private static void OnShowEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
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
                UpdateEUnitControl();
        }

        [Category("Engeneering Unit Label Style")]
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
        public static readonly DependencyProperty EngeneeringOffsetProperty = DependencyProperty.Register("EngeneeringOffset", typeof(Thickness), typeof(LinearMeterControl), new UIPropertyMetadata(new Thickness(85, 110, 0, 0), new PropertyChangedCallback(OnEngeneeringOffsetChanged), new CoerceValueCallback(OnCoerceEngeneeringOffset)));

        private static object OnCoerceEngeneeringOffset(DependencyObject o, object value)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
            if (circularGauge != null)
                return circularGauge.OnCoerceEngeneeringOffset((Thickness)value);
            else
                return value;
        }

        private static void OnEngeneeringOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl circularGauge = o as LinearMeterControl;
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
                UpdateEUnitControl();
        }

        [Category("Engeneering Unit Label Style")]
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
        #region FlowDirection
        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(LinearMeterControl), new UIPropertyMetadata(FlowDirection.LeftToRight, new PropertyChangedCallback(OnFlowDirectionChanged), new CoerceValueCallback(OnCoerceFlowDirection)));

        private static object OnCoerceFlowDirection(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceFlowDirection((FlowDirection)value);
            else
                return value;
        }

        private static void OnFlowDirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnFlowDirectionChanged((FlowDirection)e.OldValue, (FlowDirection)e.NewValue);
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
        public static readonly DependencyProperty LinePresentationProperty = DependencyProperty.Register("LinePresentation", typeof(PredefinedElementKinds), typeof(LinearMeterControl), new UIPropertyMetadata(PredefinedElementKinds.CleanWhite, new PropertyChangedCallback(OnLinePresentationChanged), new CoerceValueCallback(OnCoerceLinePresentation)));

        private static object OnCoerceLinePresentation(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLinePresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnLinePresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLinePresentationChanged((PredefinedElementKinds)e.OldValue, (PredefinedElementKinds)e.NewValue);
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
        public static readonly DependencyProperty ScaleLineTickmarkZIndexProperty = DependencyProperty.Register("ScaleLineTickmarkZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)0, new PropertyChangedCallback(OnScaleLineTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkZIndex)));

        private static object OnCoerceScaleLineTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceScaleLineTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnScaleLineTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty ScaleLineTickmarkOffsetProperty = DependencyProperty.Register("ScaleLineTickmarkOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-27, new PropertyChangedCallback(OnScaleLineTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkOffset)));

        private static object OnCoerceScaleLineTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceScaleLineTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnScaleLineTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty ScaleLineTickmarkFactorThicknessProperty = DependencyProperty.Register("ScaleLineTickmarkFactorThickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnScaleLineTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceScaleLineTickmarkFactorThickness)));

        private static object OnCoerceScaleLineTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceScaleLineTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnScaleLineTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnScaleLineTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        #region Meter Tickmark Style


        #region TickmarkFill
        public static readonly DependencyProperty TickmarkFillProperty = DependencyProperty.Register("TickmarkFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnTickmarkFillChanged), new CoerceValueCallback(OnCoerceTickmarkFill)));

        private static object OnCoerceTickmarkFill(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(LinearMeterControl), new UIPropertyMetadata(PredefinedElementKinds.Eco, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
            if (linearMeter != null)
                return linearMeter.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkFactorLength((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkFactorLengthChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-5, new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
        [SvgValueConverter(typeof(ConvertMajorTickmarkOffset), RequiredKey = true)]
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
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkShowFirstChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorTickmarkShowLastChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MajorTickmarkFillProperty = DependencyProperty.Register("MajorTickmarkFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMajorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFill)));

        private static object OnCoerceMajorTickmarkFill(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceMajorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorTickmarkFactorLength((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorTickmarkFactorLengthChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorTickmarkZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-5, new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorTickmarkOffset((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorTickmarkOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)1, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorTickmarkFactorThickness((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorTickmarkFactorThicknessChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorTickmarkShowTicksForMajorChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Tickmark Style")]
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
        public static readonly DependencyProperty MinorTickmarkFillProperty = DependencyProperty.Register("MinorTickmarkFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnMinorTickmarkFillChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFill)));

        private static object OnCoerceMinorTickmarkFill(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceMinorTickmarkFill((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
        [Category("Meter Tickmark Style")]
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
        #region Meter Marker Style

        #region MarkerVisible
        public static readonly DependencyProperty MarkerVisibleProperty = DependencyProperty.Register("MarkerVisible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerVisibleChanged), new CoerceValueCallback(OnCoerceMarkerVisible)));

        private static object OnCoerceMarkerVisible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerVisible((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerFactorHeightProperty = DependencyProperty.Register("MarkerFactorHeight", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnMarkerFactorHeightChanged), new CoerceValueCallback(OnCoerceMarkerFactorHeight)));

        private static object OnCoerceMarkerFactorHeight(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerFactorHeight((int)value);
            else
                return value;
        }

        private static void OnMarkerFactorHeightChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerFactorHeightChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Marker Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerFactorWidthProperty = DependencyProperty.Register("MarkerFactorWidth", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnMarkerFactorWidthChanged), new CoerceValueCallback(OnCoerceMarkerFactorWidth)));

        private static object OnCoerceMarkerFactorWidth(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerFactorWidth((int)value);
            else
                return value;
        }

        private static void OnMarkerFactorWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerFactorWidthChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Marker Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerZIndexProperty = DependencyProperty.Register("MarkerZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)100, new PropertyChangedCallback(OnMarkerZIndexChanged), new CoerceValueCallback(OnCoerceMarkerZIndex)));

        private static object OnCoerceMarkerZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerZIndex((int)value);
            else
                return value;
        }

        private static void OnMarkerZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)30, new PropertyChangedCallback(OnMarkerOffsetChanged), new CoerceValueCallback(OnCoerceMarkerOffset)));

        private static object OnCoerceMarkerOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerOffset((int)value);
            else
                return value;
        }

        private static void OnMarkerOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Marker Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertUnit), RequiredKey = true)]
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
        public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register("MarkerFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnMarkerFillChanged), new CoerceValueCallback(OnCoerceMarkerFill)));

        private static object OnCoerceMarkerFill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerFill((Brush)value);
            else
                return value;
        }

        private static void OnMarkerFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register("MarkerStroke", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.DarkRed), new PropertyChangedCallback(OnMarkerStrokeChanged), new CoerceValueCallback(OnCoerceMarkerStroke)));

        private static object OnCoerceMarkerStroke(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceMarkerStroke((Brush)value);
            else
                return value;
        }

        private static void OnMarkerStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerAnimationEnableProperty = DependencyProperty.Register("MarkerAnimationEnable", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMarkerAnimationEnableChanged), new CoerceValueCallback(OnCoerceMarkerAnimationEnable)));

        private static object OnCoerceMarkerAnimationEnable(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerIsInteractiveProperty = DependencyProperty.Register("MarkerIsInteractive", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnMarkerIsInteractiveChanged), new CoerceValueCallback(OnCoerceMarkerIsInteractive)));

        private static object OnCoerceMarkerIsInteractive(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMarkerIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnMarkerIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMarkerIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGaugeLayout();
            }
        }

        [Category("Meter Marker Style")]
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
        public static readonly DependencyProperty MarkerOrientationProperty = DependencyProperty.Register("MarkerOrientation", typeof(LinearScaleMarkerOrientation), typeof(LinearMeterControl), new UIPropertyMetadata(LinearScaleMarkerOrientation.Normal, new PropertyChangedCallback(OnMarkerOrientationChanged), new CoerceValueCallback(OnCoerceMarkerOrientation)));

        private static object OnCoerceMarkerOrientation(DependencyObject o, object value)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
            if (linearMeter != null)
                return linearMeter.OnCoerceMarkerOrientation((LinearScaleMarkerOrientation)value);
            else
                return value;
        }

        private static void OnMarkerOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl linearMeter = o as LinearMeterControl;
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

        [Category("Meter Marker Style")]
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
        #region Meter Value Range Bar Style

        #region RangeBarVisible
        public static readonly DependencyProperty RangeBarVisibleProperty = DependencyProperty.Register("RangeBarVisible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarVisibleChanged), new CoerceValueCallback(OnCoerceRangeBarVisible)));

        private static object OnCoerceRangeBarVisible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarVisible((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarZIndexProperty = DependencyProperty.Register("RangeBarZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)51, new PropertyChangedCallback(OnRangeBarZIndexChanged), new CoerceValueCallback(OnCoerceRangeBarZIndex)));

        private static object OnCoerceRangeBarZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarZIndex((int)value);
            else
                return value;
        }

        private static void OnRangeBarZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarOffsetProperty = DependencyProperty.Register("RangeBarOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRangeBarOffsetChanged), new CoerceValueCallback(OnCoerceRangeBarOffset)));

        private static object OnCoerceRangeBarOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarOffset((int)value);
            else
                return value;
        }

        private static void OnRangeBarOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarFillProperty = DependencyProperty.Register("RangeBarFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRangeBarFillChanged), new CoerceValueCallback(OnCoerceRangeBarFill)));

        private static object OnCoerceRangeBarFill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarFill((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarBackgroundProperty = DependencyProperty.Register("RangeBarBackground", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRangeBarBackgroundChanged), new CoerceValueCallback(OnCoerceRangeBarBackground)));

        private static object OnCoerceRangeBarBackground(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceRangeBarBackground((Brush)value);
            else
                return value;
        }

        private static void OnRangeBarBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
        public static readonly DependencyProperty RangeBarAnimationEnableProperty = DependencyProperty.Register("RangeBarAnimationEnable", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnRangeBarAnimationEnableChanged), new CoerceValueCallback(OnCoerceRangeBarAnimationEnable)));

        private static object OnCoerceRangeBarAnimationEnable(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarIsInteractiveProperty = DependencyProperty.Register("RangeBarIsInteractive", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRangeBarIsInteractiveChanged), new CoerceValueCallback(OnCoerceRangeBarIsInteractive)));

        private static object OnCoerceRangeBarIsInteractive(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnRangeBarIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGaugeLayout();
            }
        }

        [Category("Meter Value Range Bar Style")]
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
        public static readonly DependencyProperty RangeBarThicknessProperty = DependencyProperty.Register("RangeBarThickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)25, new PropertyChangedCallback(OnRangeBarThicknessChanged), new CoerceValueCallback(OnCoerceRangeBarThickness)));

        private static object OnCoerceRangeBarThickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRangeBarThickness((int)value);
            else
                return value;
        }

        private static void OnRangeBarThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRangeBarThicknessChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Range Bar Style")]
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
        #region Meter Value Bar Style

        #region LevelVisible
        public static readonly DependencyProperty LevelVisibleProperty = DependencyProperty.Register("LevelVisible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLevelVisibleChanged), new CoerceValueCallback(OnCoerceLevelVisible)));

        private static object OnCoerceLevelVisible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelVisible((Boolean)value);
            else
                return value;
        }

        private static void OnLevelVisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelVisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelZIndexProperty = DependencyProperty.Register("LevelZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)50, new PropertyChangedCallback(OnLevelZIndexChanged), new CoerceValueCallback(OnCoerceLevelZIndex)));

        private static object OnCoerceLevelZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelZIndex((int)value);
            else
                return value;
        }

        private static void OnLevelZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelZIndexChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelOffsetProperty = DependencyProperty.Register("LevelOffset", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnLevelOffsetChanged), new CoerceValueCallback(OnCoerceLevelOffset)));

        private static object OnCoerceLevelOffset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelOffset((int)value);
            else
                return value;
        }

        private static void OnLevelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelOffsetChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelFillProperty = DependencyProperty.Register("LevelFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnLevelFillChanged), new CoerceValueCallback(OnCoerceLevelFill)));

        private static object OnCoerceLevelFill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelFill((Brush)value);
            else
                return value;
        }

        private static void OnLevelFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        [Category("Meter Value Bar Style")]
        [SvgValueConverter(typeof(ConvertLevelFill), RequiredKey = true, NeedSVGUrlBrushes = true)]
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
        public static readonly DependencyProperty LevelBackgroundFillProperty = DependencyProperty.Register("LevelBackgroundFill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnLevelBackgroundFillChanged), new CoerceValueCallback(OnCoerceLevelBackgroundFill)));

        private static object OnCoerceLevelBackgroundFill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelBackgroundFill((Brush)value);
            else
                return value;
        }

        private static void OnLevelBackgroundFillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelBackgroundFillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelAnimationEnableProperty = DependencyProperty.Register("LevelAnimationEnable", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnLevelAnimationEnableChanged), new CoerceValueCallback(OnCoerceLevelAnimationEnable)));

        private static object OnCoerceLevelAnimationEnable(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelAnimationEnable((Boolean)value);
            else
                return value;
        }

        private static void OnLevelAnimationEnableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelAnimationEnableChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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

        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelIsInteractiveProperty = DependencyProperty.Register("LevelIsInteractive", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLevelIsInteractiveChanged), new CoerceValueCallback(OnCoerceLevelIsInteractive)));

        private static object OnCoerceLevelIsInteractive(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelIsInteractive((Boolean)value);
            else
                return value;
        }

        private static void OnLevelIsInteractiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelIsInteractiveChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
            {
                CheckInteractivity();
                UpdateGaugeLayout();
            }
        }

        [Category("Meter Value Bar Style")]
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
        public static readonly DependencyProperty LevelThicknessProperty = DependencyProperty.Register("LevelThickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)2, new PropertyChangedCallback(OnLevelThicknessChanged), new CoerceValueCallback(OnCoerceLevelThickness)));

        private static object OnCoerceLevelThickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceLevelThickness((int)value);
            else
                return value;
        }

        private static void OnLevelThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnLevelThicknessChanged((int)e.OldValue, (int)e.NewValue);
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

        [Category("Meter Value Bar Style")]
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

        #region Meter Scale Range Style
        #region Range1Visible
        public static readonly DependencyProperty Range1VisibleProperty = DependencyProperty.Register("Range1Visible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange1VisibleChanged), new CoerceValueCallback(OnCoerceRange1Visible)));

        private static object OnCoerceRange1Visible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange1VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1StartValueProperty = DependencyProperty.Register("Range1StartValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)0, new PropertyChangedCallback(OnRange1StartValueChanged), new CoerceValueCallback(OnCoerceRange1StartValue)));

        private static object OnCoerceRange1StartValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange1StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1EndValueProperty = DependencyProperty.Register("Range1EndValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)50, new PropertyChangedCallback(OnRange1EndValueChanged), new CoerceValueCallback(OnCoerceRange1EndValue)));

        private static object OnCoerceRange1EndValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange1EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1ThicknessProperty = DependencyProperty.Register("Range1Thickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange1ThicknessChanged), new CoerceValueCallback(OnCoerceRange1Thickness)));

        private static object OnCoerceRange1Thickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1Thickness((int)value);
            else
                return value;
        }

        private static void OnRange1ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1OffsetProperty = DependencyProperty.Register("Range1Offset", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange1OffsetChanged), new CoerceValueCallback(OnCoerceRange1Offset)));

        private static object OnCoerceRange1Offset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1Offset((Double)value);
            else
                return value;
        }

        private static void OnRange1OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1FillProperty = DependencyProperty.Register("Range1Fill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(115,170,19)), new PropertyChangedCallback(OnRange1FillChanged), new CoerceValueCallback(OnCoerceRange1Fill)));

        private static object OnCoerceRange1Fill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange1FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateLinearScaleRange1();
        }
        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range1ZIndexProperty = DependencyProperty.Register("Range1ZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange1ZIndexChanged), new CoerceValueCallback(OnCoerceRange1ZIndex)));

        private static object OnCoerceRange1ZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange1ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange1ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange1ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange1();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2VisibleProperty = DependencyProperty.Register("Range2Visible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange2VisibleChanged), new CoerceValueCallback(OnCoerceRange2Visible)));

        private static object OnCoerceRange2Visible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange2VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2StartValueProperty = DependencyProperty.Register("Range2StartValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)50, new PropertyChangedCallback(OnRange2StartValueChanged), new CoerceValueCallback(OnCoerceRange2StartValue)));

        private static object OnCoerceRange2StartValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange2StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2EndValueProperty = DependencyProperty.Register("Range2EndValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)80, new PropertyChangedCallback(OnRange2EndValueChanged), new CoerceValueCallback(OnCoerceRange2EndValue)));

        private static object OnCoerceRange2EndValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange2EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2ThicknessProperty = DependencyProperty.Register("Range2Thickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange2ThicknessChanged), new CoerceValueCallback(OnCoerceRange2Thickness)));

        private static object OnCoerceRange2Thickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2Thickness((int)value);
            else
                return value;
        }

        private static void OnRange2ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2OffsetProperty = DependencyProperty.Register("Range2Offset", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange2OffsetChanged), new CoerceValueCallback(OnCoerceRange2Offset)));

        private static object OnCoerceRange2Offset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2Offset((Double)value);
            else
                return value;
        }

        private static void OnRange2OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2FillProperty = DependencyProperty.Register("Range2Fill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(OnRange2FillChanged), new CoerceValueCallback(OnCoerceRange2Fill)));

        private static object OnCoerceRange2Fill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange2FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateLinearScaleRange2();
        }
        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range2ZIndexProperty = DependencyProperty.Register("Range2ZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange2ZIndexChanged), new CoerceValueCallback(OnCoerceRange2ZIndex)));

        private static object OnCoerceRange2ZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange2ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange2ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange2ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange2();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3VisibleProperty = DependencyProperty.Register("Range3Visible", typeof(Boolean), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnRange3VisibleChanged), new CoerceValueCallback(OnCoerceRange3Visible)));

        private static object OnCoerceRange3Visible(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3Visible((Boolean)value);
            else
                return value;
        }

        private static void OnRange3VisibleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3VisibleChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3StartValueProperty = DependencyProperty.Register("Range3StartValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)80, new PropertyChangedCallback(OnRange3StartValueChanged), new CoerceValueCallback(OnCoerceRange3StartValue)));

        private static object OnCoerceRange3StartValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3StartValue((Double)value);
            else
                return value;
        }

        private static void OnRange3StartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3StartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3EndValueProperty = DependencyProperty.Register("Range3EndValue", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)100, new PropertyChangedCallback(OnRange3EndValueChanged), new CoerceValueCallback(OnCoerceRange3EndValue)));

        private static object OnCoerceRange3EndValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3EndValue((Double)value);
            else
                return value;
        }

        private static void OnRange3EndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3EndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3ThicknessProperty = DependencyProperty.Register("Range3Thickness", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnRange3ThicknessChanged), new CoerceValueCallback(OnCoerceRange3Thickness)));

        private static object OnCoerceRange3Thickness(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3Thickness((int)value);
            else
                return value;
        }

        private static void OnRange3ThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3ThicknessChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3OffsetProperty = DependencyProperty.Register("Range3Offset", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)(-10), new PropertyChangedCallback(OnRange3OffsetChanged), new CoerceValueCallback(OnCoerceRange3Offset)));

        private static object OnCoerceRange3Offset(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3Offset((Double)value);
            else
                return value;
        }

        private static void OnRange3OffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3OffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3FillProperty = DependencyProperty.Register("Range3Fill", typeof(Brush), typeof(LinearMeterControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnRange3FillChanged), new CoerceValueCallback(OnCoerceRange3Fill)));

        private static object OnCoerceRange3Fill(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3Fill((Brush)value);
            else
                return value;
        }

        private static void OnRange3FillChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3FillChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
                UpdateLinearScaleRange3();
        }
        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty Range3ZIndexProperty = DependencyProperty.Register("Range3ZIndex", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)-10, new PropertyChangedCallback(OnRange3ZIndexChanged), new CoerceValueCallback(OnCoerceRange3ZIndex)));

        private static object OnCoerceRange3ZIndex(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceRange3ZIndex((int)value);
            else
                return value;
        }

        private static void OnRange3ZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnRange3ZIndexChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScaleRange3();
        }

        [Category("Meter Scale Range Style")]
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
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMajorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScale();
        }

        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(LinearMeterControl), new UIPropertyMetadata((int)5, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnMinorIntervalCountChanged((int)e.OldValue, (int)e.NewValue);
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
                UpdateLinearScale();
        }

        [Category("Meter Scale Style")]
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
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(LinearMeterControl), new UIPropertyMetadata((Double)0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                return LinearMeterControl.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl LinearMeterControl = o as LinearMeterControl;
            if (LinearMeterControl != null)
                LinearMeterControl.OnValueChanged((Double)e.OldValue, (Double)e.NewValue);
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

        #region AutoHideElements
        public static readonly DependencyProperty AutoHideElementsProperty = DependencyProperty.Register("AutoHideElements", typeof(bool), typeof(LinearMeterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoHideElementsChanged), new CoerceValueCallback(OnCoerceAutoHideElements)));

        private static object OnCoerceAutoHideElements(DependencyObject o, object value)
        {
            LinearMeterControl control = o as LinearMeterControl;
            if (control != null)
                return control.OnCoerceAutoHideElements((bool)value);
            else
                return value;
        }

        private static void OnAutoHideElementsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            LinearMeterControl control = o as LinearMeterControl;
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
                ManageAutoUpdatingElements(newValue);
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
        private DelayedSingleActionInvoker SizeChangedInvoker;
        IStringEditorManager stringManager;
        LinearScaleMarker selectedMarker;
        LinearScaleLevelBar selectedLevel;
        LinearScaleRangeBar selectedRangeBar;
        bool previousClipToBounds;
        bool errorEffectOn;
        public IEnumerable<PredefinedElementKind> PredefinedLinearMeterModelKinds { get { return LinearGaugeControl.PredefinedModels; } }
        internal int CustomBaseIndex
        {
            get { return 12; }
        }
        #endregion

        #region Constructors
        Size? deltaP;
        public LinearMeterControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            if (linearGauge != null)
                linearGauge.DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    deltaP = GetDelta();

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
                                deltaP = GetDelta();
                                ManageAutoUpdatingElements(AutoHideElements);
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
            if (LinearScaleRange1 != null)
            {
                LinearScaleRange1.StartValue = new RangeValue((_EndValue - _StartValue) * Range1StartValue / 100 + _StartValue);
                LinearScaleRange1.EndValue = new RangeValue((_EndValue - _StartValue) * Range1EndValue / 100 + _StartValue);
            }
            if (LinearScaleRange2 != null)
            {
                LinearScaleRange2.StartValue = new RangeValue((_EndValue - _StartValue) * Range2StartValue / 100 + _StartValue);
                LinearScaleRange2.EndValue = new RangeValue((_EndValue - _StartValue) * Range2EndValue / 100 + _StartValue);
            }
            if (LinearScaleRange3 != null)
            {
                LinearScaleRange3.StartValue = new RangeValue((_EndValue - _StartValue) * Range3StartValue / 100 + _StartValue);
                LinearScaleRange3.EndValue = new RangeValue((_EndValue - _StartValue) * Range3EndValue / 100 + _StartValue);
            }
        }
        protected override void UpdateScaleStartEndValues()
        {
            if (linearScale != null)
            {
                linearScale.StartValue = _StartValue;
                linearScale.EndValue = _EndValue;
            }
        }
        protected override void ShowMarker(bool showWarning)
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
        protected override void UpdateCustomElements()
        {
            UpdateEUnitControl();
            UpdateValueControl();
        }
        #endregion

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new LinearMeterAutomationPeer(this);
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
        private void ManageAutoUpdatingElements(bool bForceAutoHide = false)
        {
            if (!AutoHideElements && !bForceAutoHide)
                return; 
            if (linearGauge == null)
                return;
            UpdateGaugeLayout();
        }

        Size GetDelta()
        {
            if (!AutoHideElements)
                return new Size()
                {
                    Width = 1,
                    Height = 1
                };

            var orientation = GetOrientation();
            var w = GetDeltaWidth();
            var h = GetDeltaHeight();
            return new Size()
            {
                Width = orientation == horizontal ? w : h,
                Height = orientation == horizontal ? h : w
            };
        }
        double GetDeltaWidth()
        {
            if (!AutoHideElements)
                return 1;
            Size elSize = GetLayoutSize(this);
            double dimension = elSize.Width;
            return dimension <= Properties.Settings.Default.MinControlWidthSize && Properties.Settings.Default.MinControlWidthSize > 0 ? (dimension / Properties.Settings.Default.MinControlWidthSize) : 1;
        }
        double GetDeltaHeight()
        {
            if (!AutoHideElements)
                return 1;
            Size elSize = GetLayoutSize(this);
            double dimension = elSize.Height;
            return AutoHideElements && dimension <= Properties.Settings.Default.MinControlHeightSize && Properties.Settings.Default.MinControlHeightSize > 0 ? (dimension / Properties.Settings.Default.MinControlHeightSize) : 1;
        }
        void SetScaleLabelFormat()
        {
            if (linearScale == null)
                return;

            if (!deltaP.HasValue)
                deltaP = GetDelta();
            linearScale.LabelOptions.FormatString = deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityA || deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityB ? "" : LabelStringFormat;
            linearScale.MajorIntervalCount = MajorIntervalCount * deltaP.Value.Width > 1 ? (int)(MajorIntervalCount * deltaP.Value.Width) : 1;
            bool bVisibility = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityC || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityB;
            linearScale.MajorTickmarkOptions.FactorLength = bVisibility ? 0 : MajorTickmarkFactorLength;
            linearScale.MinorTickmarkOptions.FactorLength = bVisibility ? 0 : MinorTickmarkFactorLength;
        }
        void SetCustomElementVisibility(TextBlock container)
        {
            if (container == null)
                return;

            if (!deltaP.HasValue)
                deltaP = GetDelta();
            container.Visibility = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityA || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityB ? Visibility.Collapsed : Visibility.Visible;
        }
        bool GetLinearLayerVisibility()
        {
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            return deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityD || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityC ? false : true;
        }
        private void SetLinearRangeVisibility(LinearScaleRangeBar linearRange)
        {
            if (linearRange == null)
                return;
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            linearRange.Visible = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityE || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityE ? false : true;
        }
        private void SetLinearLevelBarVisibility()
        {
            if (linearLevelBar == null)
                return;
            if (!deltaP.HasValue)
                deltaP = GetDelta();
            linearLevelBar.Visible = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityE || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityE ? false : true;
        }
        private void SetLinearScaleRangeVisibility(LinearScaleRange linearScaleRange)
        {
            if (linearScaleRange == null)
                return;

            if (!deltaP.HasValue)
                deltaP = GetDelta();
            linearScaleRange.Visible = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityC || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityB ? false : true;
        }
        private void SetMarkerVisibility()
        {
            linearScaleMarker.Visible = deltaP.Value.Width < Properties.Settings.Default.MinControlWSizePrecVisibilityD || deltaP.Value.Height < Properties.Settings.Default.MinControlHSizePrecVisibilityD ? false : true;
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
                if (linearScale != null)
                {
                    linearScale.LabelOptions.FormatString = "";
                    SetScaleLabelFormat();
                }
            });
        }
        private int GetOffset(FontSettings newValue, FontSettings oldValue, int oldOffset)
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
            int newOffset;

            if (oldOffset < 0)
                newOffset = oldOffset + (int)((oldSize.Width - newSize.Width) / 2);
            else if (oldOffset > 0)
                newOffset = oldOffset + (int)((newSize.Width - oldSize.Width) / 2);
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
        void UpdateEUnitControl()
        {
            if (linearGauge == null || linearScale == null)
                return;
            try
            {
                string eunit = TranslationHelpers.TranslationHelper.TranslateComposedText(EngeneeringUnit, stringlist, EngeneeringUnit);
                string meas = ConverterLabel ?? eunit;
                if (!ShowEngeneeringUnit || string.IsNullOrEmpty(meas))
                {
                    if (unitContainer != null)
                    {
                        if (container.Children.Contains(unitContainer))
                            container.Children.Remove(unitContainer);

                        unitContainer = null;
                    }
                }
                else
                {
                    if (unitContainer == null)
                    {
                        unitContainer = new TextBlock()
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        };

                        var fontSizeBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringUnitFontSettings.FontSize"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontWeightBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringUnitFontSettings.FontWeight"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontFamilyBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringUnitFontSettings.FontFamily"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontStyleBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringUnitFontSettings.FontStyle"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var foregroundBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringUnitForeground"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var offsetBinding = new Binding()
                        {
                            Path = new PropertyPath("EngeneeringOffset"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        unitContainer.SetBinding(TextBlock.FontSizeProperty, fontSizeBinding);
                        unitContainer.SetBinding(TextBlock.FontWeightProperty, fontWeightBinding);
                        unitContainer.SetBinding(TextBlock.FontStyleProperty, fontStyleBinding);
                        unitContainer.SetBinding(TextBlock.FontFamilyProperty, fontFamilyBinding);
                        unitContainer.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                        unitContainer.SetBinding(TextBlock.MarginProperty, offsetBinding);

                        if (!container.Children.Contains(unitContainer))
                            container.Children.Add(unitContainer);
                        Grid.SetZIndex(unitContainer, 100);
                    }

                    if (unitContainer != null)
                    {
                        unitContainer.Text = meas;
                    }
                    SetCustomElementVisibility(unitContainer);
                }
            }
            catch (Exception)
            {
            }
        }
        void UpdateValueControl()
        {
            if (linearGauge == null || linearScale == null)
                return;
            try
            {
                if (!ShowValue)
                {
                    if (valueContainer != null)
                    {
                        //if (linearScale.CustomElements.Contains(valueContainer))
                        //    linearScale.CustomElements.Remove(valueContainer);
                        if (container.Children.Contains(valueContainer))
                            container.Children.Remove(valueContainer);

                        valueContainer = null;
                    }
                }
                else
                {
                    if (valueContainer == null)
                    {
                        valueContainer = new TextBlock()
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        };

                        var fontSizeBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueFontSettings.FontSize"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontWeightBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueFontSettings.FontWeight"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontFamilyBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueFontSettings.FontFamily"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var fontStyleBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueFontSettings.FontStyle"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var foregroundBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueForeground"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        var offsetBinding = new Binding()
                        {
                            Path = new PropertyPath("ValueOffset"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1),
                        };
                        valueContainer.SetBinding(TextBlock.FontSizeProperty, fontSizeBinding);
                        valueContainer.SetBinding(TextBlock.FontWeightProperty, fontWeightBinding);
                        valueContainer.SetBinding(TextBlock.FontStyleProperty, fontStyleBinding);
                        valueContainer.SetBinding(TextBlock.FontFamilyProperty, fontFamilyBinding);
                        valueContainer.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);
                        valueContainer.SetBinding(TextBlock.MarginProperty, offsetBinding);

                        if (!container.Children.Contains(valueContainer))
                            container.Children.Insert(container.Children.Count, valueContainer);
                        Grid.SetZIndex(valueContainer, 100);
                    }

                    SetCustomElementVisibility(valueContainer);
                }

                if (bInit)
                    UpdateCustomValue(Value);
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
                    linearRangeBackground.Options.ZIndex = RangeBarZIndex /*- 1*/;
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
                    SetLinearRangeVisibility(linearRange);

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
                    SetMarkerVisibility();
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
                    SetLinearScaleRangeVisibility(LinearScaleRange1);
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
                    SetLinearScaleRangeVisibility(LinearScaleRange2);
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
                    SetLinearScaleRangeVisibility(LinearScaleRange3);
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
        PredefinedBaseElementKinds oldLinearBaseModel;
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

                        SetLevelPresentation();
                        oldLinearBaseModel = LinearBaseModel;
                    }

                    if (oldLinearBaseModel != LinearBaseModel)
                    {
                        SetLevelPresentation();
                        oldLinearBaseModel = LinearBaseModel;
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
                    SetLinearLevelBarVisibility();
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
        void SetLevelPresentation()
        {
            if (linearLevelBar == null)
                return;

            CustomLinearScaleLevelBarPresentation scalelabelpresentation = new CustomLinearScaleLevelBarPresentation();
            if ((int)LinearBaseModel >= (int)PredefinedBaseElementKinds.Pile && (int)LinearBaseModel != (int)PredefinedBaseElementKinds.None)
            {
                scalelabelpresentation.LevelBarForegroundTemplate = (ControlTemplate)LoadTemplate($"{LinearBaseModel}StyleTemplates", $"{LinearBaseModel}_ForegroundTemplate");
                scalelabelpresentation.LevelBarBackgroundTemplate = (ControlTemplate)LoadTemplate($"{LinearBaseModel}StyleTemplates", $"{LinearBaseModel}_Template");
            }
            else
            {
                scalelabelpresentation.LevelBarForegroundTemplate = (ControlTemplate)LoadTemplate("Templates", "levelForegroundTemplate");
                scalelabelpresentation.LevelBarBackgroundTemplate = (ControlTemplate)LoadTemplate("Templates", "levelBackgroundTemplate");
            }

            linearLevelBar.Presentation = scalelabelpresentation;
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
                if (!GetLinearLayerVisibility() ||(int)LinearBaseModel >= CustomBaseIndex || LinearBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
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
                    linearScale.Name = "linearScale";
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


                linearScale.ShowLine = DevExpress.Utils.DefaultBoolean.False;
                linearScale.MajorTickmarkOptions.ZIndex = MajorTickmarkZIndex;
                linearScale.MajorTickmarkOptions.FactorThickness = MajorTickmarkFactorThickness;
                linearScale.MajorTickmarkOptions.Offset = MajorTickmarkOffset;
                linearScale.MajorTickmarkOptions.ShowFirst = MajorTickmarkShowFirst;
                linearScale.MajorTickmarkOptions.ShowLast = MajorTickmarkShowLast;

                linearScale.MinorTickmarkOptions.ZIndex = MinorTickmarkZIndex;
                linearScale.MinorTickmarkOptions.FactorThickness = MinorTickmarkFactorThickness;
                linearScale.MinorTickmarkOptions.Offset = MinorTickmarkOffset;
                linearScale.MinorTickmarkOptions.ShowTicksForMajor = MinorTickmarkShowTicksForMajor;

                linearScale.LabelOptions.Orientation = LabelOrientation;
                linearScale.LabelOptions.ShowFirst = ShowFirstLabel;
                linearScale.LabelOptions.ShowLast = ShowLastLabel;
                linearScale.LabelOptions.Offset = LabelOffset;
                linearScale.LabelOptions.ZIndex = LabelZIndex;
                SetScaleLabelFormat();
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
                        ClipToBounds = true,
                        Name = "linearGauge"
                    };

                    linearGauge.Model = new LinearCleanWhiteModel();
                    var innerPadding = (linearGauge.Model as GaugeModelBase).InnerPadding;
                    if(LinearBaseModel == PredefinedBaseElementKinds.None || !EnableBackGroundLayer)
                    {
                        if (GetOrientation() == horizontal)
                            (linearGauge.Model as GaugeModelBase).InnerPadding = new Thickness(innerPadding.Left, Properties.Settings.Default.MeterInnerPadding, innerPadding.Right, Properties.Settings.Default.MeterInnerPadding);
                        else
                            (linearGauge.Model as GaugeModelBase).InnerPadding = new Thickness(Properties.Settings.Default.MeterInnerPadding, innerPadding.Top, Properties.Settings.Default.MeterInnerPadding, innerPadding.Bottom);
                    }

                    if (!container.Children.Contains(linearGauge))
                        container.Children.Insert(0,linearGauge);
                }
                Grid.SetZIndex(linearGauge, 1);

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
            //container.MinHeight = gridMinDimension;
            //container.MinWidth = gridMinDimension;

            if ((int)LinearBaseModel < CustomBaseIndex || ((int)LinearBaseModel >= (int)PredefinedBaseElementKinds.Pile) || !EnableBackGroundLayer)
            {
                if (BackContent != null)
                {
                    if (container.Children.Contains(BackContent))
                        container.Children.Remove(BackContent);

                    BackContent.Content = null;
                    BackContent = null;
                }

                //******************************************************
                //Temporary solution for Devexpress problem
                //if ((int)LinearBaseModel < CustomBaseIndex)
                //{
                //    container.ClearValue(FrameworkElement.MinHeightProperty);
                //    container.ClearValue(FrameworkElement.MaxHeightProperty);
                //}
                //else
                //{
                //    container.MinHeight = gridMinDimension;
                //    container.MinWidth = gridMinDimension;
                //}
                //******************************************************
            }
            else
            {
                if((int)LinearBaseModel < (int)PredefinedBaseElementKinds.Pile)
                {
                    //container.MinHeight = gridMinDimension;
                    //container.MinWidth = gridMinDimension;

                    if (BackContent == null)
                    {
                        BackContent = new ContentControl();
                        BackContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                        BackContent.VerticalContentAlignment = VerticalAlignment.Stretch;


                        if (!container.Children.Contains(BackContent))
                            container.Children.Add(BackContent);
                    }
                    Grid.SetZIndex(BackContent, 0);

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
                else
                {
                    if (BackContent != null)
                    {
                        if (container.Children.Contains(BackContent))
                            container.Children.Remove(BackContent);

                        BackContent.Content = null;
                        BackContent = null;
                    }
                }
            }
        }
        private FrameworkElement LoadBackContent()
        {
            try
            {
                string basename = string.Format("meter{0}.xaml", (int)LinearBaseModel - CustomBaseIndex + 1);
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.MeterContainers.{1}", typeof(LinearMeterControl).Namespace, basename));
                if (stream == null)
                    return null;
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                FrameworkElement content = (FrameworkElement)canvas.TryFindResource(string.Format("{0}_{1}", LinearBaseModel.ToString(), GetOrientation()));

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
                                                          String.Format("{0}.MeterContainers.{1}", typeof(LinearMeterControl).Namespace, basename));
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
                _value = 50;// (EndValue - StartValue) / 2;

            if (linearLevelBar != null)
                linearLevelBar.Value = _value;
            if (linearRange != null)
                linearRange.Value = _value;
            if (linearScaleMarker != null)
                linearScaleMarker.Value = _value;
            UpdateCustomValue(_value);

            if(linearBaseScale != null)
            {
                linearBaseScale.StartValue = 0;
                linearBaseScale.EndValue = 100;
                if (linearRangeBackground != null)
                    linearRangeBackground.Value = 100;
            }
        }
        void UpdateCustomValue(double value)
        {
            if (valueContainer != null)
                valueContainer.Text = ConvertValue(value);
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
        private void CheckInteractivity()
        {
            if (RunningOnServer && !MarkerIsInteractive && !RangeBarIsInteractive && !LevelIsInteractive)
                IsHitTestVisible = false;
        }
        #endregion

        #region DynObjects
        LinearGaugeControl linearGauge;
        LinearScale linearScale;
        LinearScale linearBaseScale;
        LinearScaleRange LinearScaleRange1;
        LinearScaleRange LinearScaleRange2;
        LinearScaleRange LinearScaleRange3;
        LinearScaleMarker linearScaleMarker;
        LinearScaleLayer linearLayer;
        LinearScaleLevelBar linearLevelBar;
        LinearScaleRangeBar linearRangeBackground;
        LinearScaleRangeBar linearRange;
        TextBlock valueContainer;
        TextBlock unitContainer;
        ContentControl BackContent;
        Image WarningMarker;
        #endregion

        #region IDisposable
        public override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            base.Dispose(true);


            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            SizeChanged -= OnSizeChanged;
            SizeChangedInvoker = null;

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

            if (unitContainer != null)
                unitContainer = null;

            if (valueContainer != null)
                valueContainer = null;

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
            valueContainer = null;
            unitContainer = null;
            linearGauge = null;
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

            if (sender is LinearMeterControl)
            {
                LinearMeterControl meter = sender as LinearMeterControl;
                Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
                var entity = (PredefinedBaseElementKinds)meter.LinearBaseModel;
                ret.Add("Background", meter.Background);
                ret.Add("LevelBackgroundFill", meter.RangeBarVisible ? meter.RangeBarBackground : meter.LevelBackgroundFill);
                return ret;
            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;
                Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
                var entity = (PredefinedBaseElementKinds)meter.LinearBaseModel;
                ret.Add("Background", meter.Background);
                ret.Add("LevelBackgroundFill", meter.RangeBarVisible ? meter.RangeBarBackground : meter.LevelBackgroundFill);
                return ret;
            }
            else if (sender is FastLinearMeterControl)
            {
                FastLinearMeterControl meter = sender as FastLinearMeterControl;
                Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
                ret.Add("Background", meter.Background);
                ret.Add("LevelBackgroundFill", meter.BarBackColor);
                return ret;
            }
            else if (sender is FastLinearMeter)
            {
                FastLinearMeter meter = sender as FastLinearMeter;
                Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
                ret.Add("Background", meter.Background);
                ret.Add("LevelBackgroundFill", meter.BarBackColor);
                return ret;
            }
            else
                return null;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            if (sender is LinearMeterControl)
            {
                LinearMeterControl meter = sender as LinearMeterControl;
                var ret = new Dictionary<string, string>();
                var entity = (PredefinedBaseElementKinds)meter.LinearBaseModel;
                if (entity != PredefinedBaseElementKinds.None && meter.EnableBackGroundLayer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer1 &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Linear)
                    ret.Add(meter.LinearBaseModel.ToString(), meter.LoadSVGBackContent());
                ret.Add(meter.GetTemplateName(), meter.LoadSVGBackLevelContent());
                return ret;
            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;

                var ret = new Dictionary<string, string>();
                var entity = (PredefinedBaseElementKinds)meter.LinearBaseModel;
                if (entity != PredefinedBaseElementKinds.None && meter.EnableBackGroundLayer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer1 &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Linear)
                    ret.Add(meter.LinearBaseModel.ToString(), meter.LoadSVGBackContent());
                ret.Add(meter.GetTemplateName(), meter.LoadSVGBackLevelContent());
                return ret;
            }
            else if (sender is FastLinearMeterControl)
            {
                FastLinearMeterControl meter = sender as FastLinearMeterControl;
                var ret = new Dictionary<string, string>();
                if (meter.MeterType != FastMeterTypes.None)
                     ret.Add(meter.MeterType.ToString(), meter.LoadSVGBackContent());
                ret.Add(meter.LoadSVGBackLevelContent(true), meter.LoadSVGBackLevelContent());
                return ret;
            }
            else if (sender is FastLinearMeter)
            {
                FastLinearMeter meter = sender as FastLinearMeter;
                var ret = new Dictionary<string, string>();
                if (meter.MeterType != FastMeterTypes.None)
                    ret.Add(meter.MeterType.ToString(), meter.LoadSVGBackContent());
                ret.Add(meter.LoadSVGBackLevelContent(true), meter.LoadSVGBackLevelContent());
                return ret;
            }
            else
                return null;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, System.Xml.XmlElement>);
            }
        }
    }
    internal class ConvertMeterBaseModel : CustomValueConverter
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
                var entity = (PredefinedBaseElementKinds)value;
                if (entity == PredefinedBaseElementKinds.Termometer || entity == PredefinedBaseElementKinds.Termometer1)
                    return "UseTermometer";
                if (entity == PredefinedBaseElementKinds.None || (int)entity >= (int)PredefinedBaseElementKinds.Pile)
                    return null;
                LinearMeterControl meter = sender as LinearMeterControl;

                if (!meter.EnableBackGroundLayer)
                    return null;
                else
                    return meter.LoadSVGBackContent();

            }
            else if (sender is LinearMeter)
            {
                var entity = (PredefinedBaseElementKinds)value;
                if (entity == PredefinedBaseElementKinds.Termometer || entity == PredefinedBaseElementKinds.Termometer1)
                    return "UseTermometer";
                if (entity == PredefinedBaseElementKinds.None || (int)entity >= (int)PredefinedBaseElementKinds.Pile)
                    return null;
                LinearMeter meter = sender as LinearMeter;

                if (!meter.EnableBackGroundLayer)
                    return null;
                else
                    return meter.LoadSVGBackContent();
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
    internal class ConvertLevelOptions : CustomValueConverter
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
                LinearMeterControl meter = sender as LinearMeterControl;
                return meter.GetLevelOptions().ToDictionary();

            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;
                return meter.GetLevelOptions().ToDictionary();
            }
            else
                return value;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }
    internal class ConvertLevelBackgroundTemplate : CustomValueConverter
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
                LinearMeterControl meter = sender as LinearMeterControl;

                if ((int)meter.LinearBaseModel >= (int)PredefinedBaseElementKinds.Pile && 
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.None &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer1 &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Linear)
                    return meter.LoadSVGBackContent();
                else
                    return $"_LinearMeterControl_{meter.LoadSVGBackLevelContent()}";

            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;

                if ((int)meter.LinearBaseModel >= (int)PredefinedBaseElementKinds.Pile && 
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.None &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Termometer1 &&
                    (int)meter.LinearBaseModel != (int)PredefinedBaseElementKinds.Linear)
                    return meter.LoadSVGBackContent();
                else
                    return $"_LinearMeterControl_{meter.LoadSVGBackLevelContent()}";
            }
            else if (sender is FastLinearMeterControl)
            {
                FastLinearMeterControl meter = sender as FastLinearMeterControl;
                return $"_FastLinearMeterControl_{meter.LoadSVGBackLevelContent()}";
            }
            else if (sender is FastLinearMeter)
            {
                FastLinearMeter meter = sender as FastLinearMeter;
                return $"_FastLinearMeter_{meter.LoadSVGBackLevelContent()}";
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

    internal class ConvertMajorTickmarkOffset : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
           return null;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender is LinearMeterControl)
            {
                LinearMeterControl meter = sender as LinearMeterControl;
                int barOffset = meter.RangeBarVisible ? meter.RangeBarOffset : meter.LevelOffset;
                int delta = 3;
                return meter.MajorTickmarkOffset <= barOffset + delta && meter.MajorTickmarkOffset >= barOffset - delta ? 0 : 1;
            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;
                int barOffset = meter.RangeBarVisible ? meter.RangeBarOffset : meter.LevelOffset;
                int delta = 3;
                return meter.MajorTickmarkOffset <= barOffset + delta && meter.MajorTickmarkOffset >= barOffset - delta ? 0 : 1;
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
    internal class ConvertLevelFill : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null)
                return null;

            if (sender is LinearMeterControl)
            {
                LinearMeterControl meter = sender as LinearMeterControl;
                var levelFillBrush = meter.RangeBarVisible ? meter.RangeBarFill : meter.LevelFill;
                if (levelFillBrush?.Opacity == 0 || (levelFillBrush as SolidColorBrush)?.Color.A == 0)
                    levelFillBrush = meter.RangeBarBackground;
                return new Dictionary<string, Brush>() {
                        { "LevelFill", levelFillBrush },
                        { "MarkerFill", meter.MarkerFill },
                        { "MarkerStroke", meter.MarkerStroke }
                    };
            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;
                var levelFillBrush = meter.RangeBarVisible ? meter.RangeBarFill : meter.LevelFill;
                if (levelFillBrush?.Opacity == 0 || (levelFillBrush as SolidColorBrush)?.Color.A == 0)
                    levelFillBrush = meter.RangeBarBackground;
                return new Dictionary<string, Brush>() {
                        { "LevelFill", levelFillBrush },
                        { "MarkerFill", meter.MarkerFill },
                        { "MarkerStroke", meter.MarkerFill }
                    };
            }
            else if (sender is FastLinearMeterControl)
            {
                FastLinearMeterControl meter = sender as FastLinearMeterControl;
                return new Dictionary<string, Brush>() {
                        { "LevelFill", meter.BarColor },
                        { "MarkerFill", meter.MarkerColor },
                        { "MarkerStroke", meter.MarkerStroke }
                    };
            }
            else if (sender is FastLinearMeter)
            {
                FastLinearMeter meter = sender as FastLinearMeter;
                return new Dictionary<string, Brush>() {
                        { "LevelFill", meter.BarColor },
                        { "MarkerFill", meter.MarkerColor },
                        { "MarkerStroke", meter.MarkerStroke }
                    };
            }
            else
                return null;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            if (sender is LinearMeterControl)
            {
                LinearMeterControl meter = sender as LinearMeterControl;
                Brush fill = meter.RangeBarVisible ? meter.RangeBarFill : meter.LevelFill;
                if (fill?.Opacity == 0 || (fill as SolidColorBrush)?.Color.A == 0)
                    fill = meter.RangeBarBackground;
                return fill;
            }
            else if (sender is LinearMeter)
            {
                LinearMeter meter = sender as LinearMeter;
                Brush fill = meter.RangeBarVisible ? meter.RangeBarFill : meter.LevelFill;
                if (fill?.Opacity == 0 || (fill as SolidColorBrush)?.Color.A == 0)
                    fill = meter.RangeBarBackground;
                return fill;
            }
            else if (sender is FastLinearMeterControl)
            {
                FastLinearMeterControl meter = sender as FastLinearMeterControl;
                return meter.BarColor;
            }
            else if (sender is FastLinearMeter)
            {
                FastLinearMeter meter = sender as FastLinearMeter;
                return meter.BarColor;
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
    
    [DataContract(Name = "LevelOptions", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class LevelOptions
    {
        public int Offset { get; set; }
        public int Thickness { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Offset", Offset);
            res.Add("Thickness", Thickness);
            return res;
        }
    }
}
