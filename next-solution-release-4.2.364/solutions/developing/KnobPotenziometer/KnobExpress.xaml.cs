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
using UFInterfaces;
using System.Windows.Media.Effects;
using ScreenSettings;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Utilities.WPF;
using System.IO;
using KnobPotenziometer.Enums;
using DynamicTagAwareHelper;
using System.Threading.Tasks;
using StringManager.ComponentService;
using System.Threading;
using System.Xml.Serialization;
using WPFUtilities.Extensions;
using UFInterfaces.PropertyControl;

namespace KnobPotenziometer
{
    /// <summary>
    /// Interaction logic for KnobExpressPotenziometer.xaml
    /// </summary>
    public partial class KnobExpress : RangeBaseControl.RangeBaseControl, IStringIDAware, IContainPropertyEditors
    {
        #region Dependency Properties
        #region IsEnabled

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;
            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(KnobExpress));
            dpd.AddValueChangedSafe(this, OnIsEnabledChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;
            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(KnobExpress));
            dpd.RemoveValueChangedSafe(this, OnIsEnabledChanged);
        }

        private void OnIsEnabledChanged(object sender, EventArgs e)
        {
            var control = sender as KnobExpress;
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


        #region PotenziometerStyle
        public static readonly DependencyProperty PotenziometerStyleProperty = DependencyProperty.Register("PotenziometerStyle", typeof(PotenziometerStyle), typeof(KnobExpress), new UIPropertyMetadata(PotenziometerStyle.Flat,new PropertyChangedCallback(OnPotenziometerStyleChanged), new CoerceValueCallback(OnCoercePotenziometerStyle)));

        private static object OnCoercePotenziometerStyle(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoercePotenziometerStyle((PotenziometerStyle)value);
            else
                return value;
        }

        private static void OnPotenziometerStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                control.OnPotenziometerStyleChanged((PotenziometerStyle)e.OldValue, (PotenziometerStyle)e.NewValue);
        }

        protected virtual PotenziometerStyle OnCoercePotenziometerStyle(PotenziometerStyle value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPotenziometerStyleChanged(PotenziometerStyle oldValue, PotenziometerStyle newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bLoaded && bInit)
            {
                UpdateNeedle();
                UpdateArcLayer();
            }
        }

        public PotenziometerStyle PotenziometerStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PotenziometerStyle)GetValue(PotenziometerStyleProperty);
            }
            set
            {
                SetValue(PotenziometerStyleProperty, value);
            }
        }

        #endregion


        #region MarkerOffset
        public static readonly DependencyProperty MarkerOffsetProperty = DependencyProperty.Register("MarkerOffset", typeof(double), typeof(KnobExpress), new UIPropertyMetadata(-45d, new PropertyChangedCallback(OnMarkerOffsetChanged), new CoerceValueCallback(OnCoerceMarkerOffset)));

        private static object OnCoerceMarkerOffset(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoerceMarkerOffset((double)value);
            else
                return value;
        }

        private static void OnMarkerOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                control.OnMarkerOffsetChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMarkerOffset(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerOffsetChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                UpdateNeedle();
        }

        public double MarkerOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MarkerOffsetProperty);
            }
            set
            {
                SetValue(MarkerOffsetProperty, value);
            }
        }

        #endregion
        #region MarkerDimension
        public static readonly DependencyProperty MarkerDimensionProperty = DependencyProperty.Register("MarkerDimension", typeof(double), typeof(KnobExpress), new UIPropertyMetadata(32d, new PropertyChangedCallback(OnMarkerDimensionChanged), new CoerceValueCallback(OnCoerceMarkerDimension)));

        private static object OnCoerceMarkerDimension(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoerceMarkerDimension((double)value);
            else
                return value;
        }

        private static void OnMarkerDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                control.OnMarkerDimensionChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMarkerDimension(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMarkerDimensionChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double MarkerDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MarkerDimensionProperty);
            }
            set
            {
                SetValue(MarkerDimensionProperty, value);
            }
        }

        #endregion

        #region ShowLabels
        public static readonly DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLabelsChanged), new CoerceValueCallback(OnCoerceShowLabels)));

        private static object OnCoerceShowLabels(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoerceShowLabels((bool)value);
            else
                return value;
        }

        private static void OnShowLabelsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
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
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateArcScale();
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
        public static readonly DependencyProperty ShowFirstLabelProperty = DependencyProperty.Register("ShowFirstLabel", typeof(Boolean), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowFirstLabelChanged), new CoerceValueCallback(OnCoerceShowFirstLabel)));

        private static object OnCoerceShowFirstLabel(DependencyObject o, object value)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                return KnobExpress.OnCoerceShowFirstLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowFirstLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                KnobExpress.OnShowFirstLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        public static readonly DependencyProperty ShowLastLabelProperty = DependencyProperty.Register("ShowLastLabel", typeof(Boolean), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowLastLabelChanged), new CoerceValueCallback(OnCoerceShowLastLabel)));

        private static object OnCoerceShowLastLabel(DependencyObject o, object value)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                return KnobExpress.OnCoerceShowLastLabel((Boolean)value);
            else
                return value;
        }

        private static void OnShowLastLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                KnobExpress.OnShowLastLabelChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
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
        public static readonly DependencyProperty MajorTickBrushProperty = DependencyProperty.Register("MajorTickBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnMajorTickBrushChanged), new CoerceValueCallback(OnCoerceMajorTickBrush)));

        private static object OnCoerceMajorTickBrush(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMajorTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnMajorTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty MinorTickBrushProperty = DependencyProperty.Register("MinorTickBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnMinorTickBrushChanged), new CoerceValueCallback(OnCoerceMinorTickBrush)));

        private static object OnCoerceMinorTickBrush(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMinorTickBrush((Brush)value);
            else
                return value;
        }

        private static void OnMinorTickBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)0.0, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            KnobExpress circularGaugeObject = o as KnobExpress;
            if (circularGaugeObject != null)
                return circularGaugeObject.OnCoerceValue((Double)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGaugeObject = o as KnobExpress;
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
        public static readonly DependencyProperty ValueStringFormatProperty = DependencyProperty.Register("ValueStringFormat", typeof(String), typeof(KnobExpress), new UIPropertyMetadata("0", new PropertyChangedCallback(OnValueStringFormatChanged), new CoerceValueCallback(OnCoerceValueStringFormat)));

        private static object OnCoerceValueStringFormat(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoerceValueStringFormat((String)value);
            else
                return value;
        }

        private static void OnValueStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
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
        public static readonly DependencyProperty MinorIntervalCountProperty = DependencyProperty.Register("MinorIntervalCount", typeof(int), typeof(KnobExpress), new UIPropertyMetadata((int)3, new PropertyChangedCallback(OnMinorIntervalCountChanged), new CoerceValueCallback(OnCoerceMinorIntervalCount)));

        private static object OnCoerceMinorIntervalCount(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMinorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMinorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty MajorIntervalCountProperty = DependencyProperty.Register("MajorIntervalCount", typeof(int), typeof(KnobExpress), new UIPropertyMetadata((int)4, new PropertyChangedCallback(OnMajorIntervalCountChanged), new CoerceValueCallback(OnCoerceMajorIntervalCount)));

        private static object OnCoerceMajorIntervalCount(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceMajorIntervalCount((int)value);
            else
                return value;
        }

        private static void OnMajorIntervalCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty LabelStringFormatProperty = DependencyProperty.Register("LabelStringFormat", typeof(String), typeof(KnobExpress), new UIPropertyMetadata("{0:0}", new PropertyChangedCallback(OnLabelStringFormatChanged), new CoerceValueCallback(OnCoerceLabelStringFormat)));

        private static object OnCoerceLabelStringFormat(DependencyObject o, object value)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                return KnobExpress.OnCoerceLabelStringFormat((String)value);
            else
                return value;
        }

        private static void OnLabelStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                KnobExpress.OnLabelStringFormatChanged((String)e.OldValue, (String)e.NewValue);
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
        #region LabelOrientation
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(ArcScaleLabelOrientation), typeof(KnobExpress), new UIPropertyMetadata(ArcScaleLabelOrientation.LeftToRight, new PropertyChangedCallback(OnLableOrientationChanged), new CoerceValueCallback(OnCoerceLableOrientation)));

        private static object OnCoerceLableOrientation(DependencyObject o, object value)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                return KnobExpress.OnCoerceLableOrientation((ArcScaleLabelOrientation)value);
            else
                return value;
        }

        private static void OnLableOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                KnobExpress.OnLableOrientationChanged((ArcScaleLabelOrientation)e.OldValue, (ArcScaleLabelOrientation)e.NewValue);
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
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata(-16d, new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                return KnobExpress.OnCoerceLabelOffset((Double)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress KnobExpress = o as KnobExpress;
            if (KnobExpress != null)
                KnobExpress.OnLabelOffsetChanged((Double)e.OldValue, (Double)e.NewValue);
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
        public static readonly DependencyProperty NeedleBorderBrushProperty = DependencyProperty.Register("NeedleBorderBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(new SolidColorBrush(Colors.SeaGreen), new PropertyChangedCallback(OnNeedleBorderBrushChanged), new CoerceValueCallback(OnCoerceNeedleBorderBrush)));

        private static object OnCoerceNeedleBorderBrush(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceNeedleBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnNeedleBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty NeedleBackBrushProperty = DependencyProperty.Register("NeedleBackBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(new SolidColorBrush(Colors.MediumSeaGreen), new PropertyChangedCallback(OnNeedleBackBrushChanged), new CoerceValueCallback(OnCoerceNeedleBackBrush)));

        private static object OnCoerceNeedleBackBrush(DependencyObject o, object value)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
            if (knobPotenziometer != null)
                return knobPotenziometer.OnCoerceNeedleBackBrush((Brush)value);
            else
                return value;
        }

        private static void OnNeedleBackBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobPotenziometer = o as KnobExpress;
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
        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnBackgroundBrushChanged), new CoerceValueCallback(OnCoerceBackgroundBrush)));

        private static object OnCoerceBackgroundBrush(DependencyObject o, object value)
        {
            KnobExpress knobExpressPotenziometer = o as KnobExpress;
            if (knobExpressPotenziometer != null)
                return knobExpressPotenziometer.OnCoerceBackgroundBrush((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobExpressPotenziometer = o as KnobExpress;
            if (knobExpressPotenziometer != null)
                knobExpressPotenziometer.OnBackgroundBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        public static readonly DependencyProperty BackgroundBorderBrushProperty = DependencyProperty.Register("BackgroundBorderBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(new SolidColorBrush(Colors.Silver), new PropertyChangedCallback(OnBackgroundBorderBrushChanged), new CoerceValueCallback(OnCoerceBackgroundBorderBrush)));

        private static object OnCoerceBackgroundBorderBrush(DependencyObject o, object value)
        {
            KnobExpress knobExpressPotenziometer = o as KnobExpress;
            if (knobExpressPotenziometer != null)
                return knobExpressPotenziometer.OnCoerceBackgroundBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnBackgroundBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress knobExpressPotenziometer = o as KnobExpress;
            if (knobExpressPotenziometer != null)
                knobExpressPotenziometer.OnBackgroundBorderBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
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
        public static readonly DependencyProperty RingBackBrushProperty = DependencyProperty.Register("RingBackBrush", typeof(Brush), typeof(KnobExpress), new UIPropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback(OnRingBackBrushChanged), new CoerceValueCallback(OnCoerceRingBackBrush)));

        private static object OnCoerceRingBackBrush(DependencyObject o, object value)
        {
            KnobExpress control = o as KnobExpress;
            if (control != null)
                return control.OnCoerceRingBackBrush((Brush)value);
            else
                return value;
        }

        private static void OnRingBackBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress control = o as KnobExpress;
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

        #region TickmarkOptions
        #region TickmarksPresentation
        public static readonly DependencyProperty TickmarksPresentationProperty = DependencyProperty.Register("TickmarksPresentation", typeof(PredefinedElementKinds), typeof(KnobExpress), new UIPropertyMetadata(PredefinedElementKinds.CleanWhite, new PropertyChangedCallback(OnTickmarksPresentationChanged), new CoerceValueCallback(OnCoerceTickmarksPresentation)));

        private static object OnCoerceTickmarksPresentation(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceTickmarksPresentation((PredefinedElementKinds)value);
            else
                return value;
        }

        private static void OnTickmarksPresentationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        #region MajorTickmarkFactorLength
        public static readonly DependencyProperty MajorTickmarkFactorLengthProperty = DependencyProperty.Register("MajorTickmarkFactorLength", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)0.4, new PropertyChangedCallback(OnMajorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorLength)));

        private static object OnCoerceMajorTickmarkFactorLength(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MajorTickmarkZIndexProperty = DependencyProperty.Register("MajorTickmarkZIndex", typeof(int), typeof(KnobExpress), new UIPropertyMetadata((int)20, new PropertyChangedCallback(OnMajorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMajorTickmarkZIndex)));

        private static object OnCoerceMajorTickmarkZIndex(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMajorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MajorTickmarkFactorThicknessProperty = DependencyProperty.Register("MajorTickmarkFactorThickness", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMajorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMajorTickmarkFactorThickness)));

        private static object OnCoerceMajorTickmarkFactorThickness(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MajorTickmarkOffsetProperty = DependencyProperty.Register("MajorTickmarkOffset", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)(-0.5), new PropertyChangedCallback(OnMajorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMajorTickmarkOffset)));

        private static object OnCoerceMajorTickmarkOffset(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMajorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MajorTickmarkShowFirstProperty = DependencyProperty.Register("MajorTickmarkShowFirst", typeof(Boolean), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowFirstChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowFirst)));

        private static object OnCoerceMajorTickmarkShowFirst(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowFirst((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowFirstChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MajorTickmarkShowLastProperty = DependencyProperty.Register("MajorTickmarkShowLast", typeof(Boolean), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMajorTickmarkShowLastChanged), new CoerceValueCallback(OnCoerceMajorTickmarkShowLast)));

        private static object OnCoerceMajorTickmarkShowLast(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMajorTickmarkShowLast((Boolean)value);
            else
                return value;
        }

        private static void OnMajorTickmarkShowLastChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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



        #region MinorTickmarkFactorLength
        public static readonly DependencyProperty MinorTickmarkFactorLengthProperty = DependencyProperty.Register("MinorTickmarkFactorLength", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)0.5, new PropertyChangedCallback(OnMinorTickmarkFactorLengthChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorLength)));

        private static object OnCoerceMinorTickmarkFactorLength(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorLength((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MinorTickmarkZIndexProperty = DependencyProperty.Register("MinorTickmarkZIndex", typeof(int), typeof(KnobExpress), new UIPropertyMetadata((int)10, new PropertyChangedCallback(OnMinorTickmarkZIndexChanged), new CoerceValueCallback(OnCoerceMinorTickmarkZIndex)));

        private static object OnCoerceMinorTickmarkZIndex(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkZIndex((int)value);
            else
                return value;
        }

        private static void OnMinorTickmarkZIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MinorTickmarkOffsetProperty = DependencyProperty.Register("MinorTickmarkOffset", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)(-0.5), new PropertyChangedCallback(OnMinorTickmarkOffsetChanged), new CoerceValueCallback(OnCoerceMinorTickmarkOffset)));

        private static object OnCoerceMinorTickmarkOffset(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkOffset((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MinorTickmarkFactorThicknessProperty = DependencyProperty.Register("MinorTickmarkFactorThickness", typeof(Double), typeof(KnobExpress), new UIPropertyMetadata((Double)1.0, new PropertyChangedCallback(OnMinorTickmarkFactorThicknessChanged), new CoerceValueCallback(OnCoerceMinorTickmarkFactorThickness)));

        private static object OnCoerceMinorTickmarkFactorThickness(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkFactorThickness((Double)value);
            else
                return value;
        }

        private static void OnMinorTickmarkFactorThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        public static readonly DependencyProperty MinorTickmarkShowTicksForMajorProperty = DependencyProperty.Register("MinorTickmarkShowTicksForMajor", typeof(Boolean), typeof(KnobExpress), new UIPropertyMetadata(true, new PropertyChangedCallback(OnMinorTickmarkShowTicksForMajorChanged), new CoerceValueCallback(OnCoerceMinorTickmarkShowTicksForMajor)));

        private static object OnCoerceMinorTickmarkShowTicksForMajor(DependencyObject o, object value)
        {
            KnobExpress circularGauge = o as KnobExpress;
            if (circularGauge != null)
                return circularGauge.OnCoerceMinorTickmarkShowTicksForMajor((Boolean)value);
            else
                return value;
        }

        private static void OnMinorTickmarkShowTicksForMajorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KnobExpress circularGauge = o as KnobExpress;
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
        bool bDatacontextChanging;
        bool isChangingValue;
        bool isUserMouseDownAction;
        CancellationTokenSource cts;
        #endregion

        #region DynObjects
        internal CircularGaugeControl circularGaugeObject;
        internal ArcScale arcScale;
        internal ArcScaleMarker arcScaleMarker;
        ScaleCustomElement gridValueContainer;
        internal ContentControl BackContent;
        #endregion

        #region Constructor

        public KnobExpress()
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
                    if (!bDesign)
                        InitControl();

                    bInit = true;
                }
            };
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
                    arcScale.LabelOptions.FormatString = LabelStringFormat;
                }
            });
        }

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

        protected override void UpdateCustomElements()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            string eunit = TranslationHelpers.TranslationHelper.TranslateComposedText(EngeneeringUnit, stringlist, EngeneeringUnit);
            string meas = ConverterLabel ?? eunit;
            if (gridValueContainer == null)
            {
                gridValueContainer = new ScaleCustomElement()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    ZIndex = 300
                };

                Grid _grid = new Grid();
                StackPanel _stack = new StackPanel() { Orientation = Orientation.Vertical };
                _stack.VerticalAlignment = VerticalAlignment.Center;
                TextBlock _unitLabel = (TextBlock)LoadTemplate("knobExpressEngeneeringUnitTemplate");

                TextBox _text = (TextBox)LoadTemplate("knobExpressValueTemplate");

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
                if(_text != null)
                    _stack.Children.Add(_text);
                if (_unitLabel != null)
                    _stack.Children.Add(_unitLabel);
                _grid.Children.Add(_stack);
                gridValueContainer.Content = _grid;

                if (!arcScale.CustomElements.Contains(gridValueContainer))
                    arcScale.CustomElements.Add(gridValueContainer);
            }

            UpdateEUnitControl();
        }
        internal virtual void UpdateNeedle()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;
            if (arcScaleMarker == null)
            {
                arcScaleMarker = new ArcScaleMarker();
                arcScaleMarker.Options = new ArcScaleMarkerOptions();
                arcScaleMarker.Presentation = new CustomArcScaleMarkerPresentation();

                if (!arcScale.Markers.Contains(arcScaleMarker))
                    arcScale.Markers.Add(arcScaleMarker);

                if (!bDesign)
                {
                    arcScaleMarker.ValueChanged += arcNeedle_ValueChanged;
                }
            }

            var presentation = (arcScaleMarker.Presentation as CustomArcScaleMarkerPresentation);
            presentation.MarkerTemplate = (ControlTemplate)LoadTemplate($"{PotenziometerStyle}_MarkerTemplate");

            arcScaleMarker.Options.ZIndex = 500;
            arcScaleMarker.Options.Offset = MarkerOffset;
            arcScaleMarker.IsInteractive = true;
        }
        private void arcNeedle_ValueChanged(object sender, ValueChangedEventArgs ev)
        {
            if (bDesign)
                return;

            if (isChangingValue || isUserMouseDownAction)
                Value = ev.NewValue;
            isUserMouseDownAction = false;
        }

        internal virtual void UpdateArcLayer()
        {
            if (circularGaugeObject == null || arcScale == null)
                return;

            if (BackContent == null)
            {
                BackContent = new ContentControl();

                if (!container.Children.Contains(BackContent))
                    container.Children.Add(BackContent);
            }

            Grid.SetZIndex(BackContent, 0);

            Viewbox content = LoadBackControl($"{PotenziometerStyle}_ScaleLayerTemplate");
            BackContent.Content = content;
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
                    scalelabelpresentation.LabelTemplate = (ControlTemplate)LoadTemplate("knobExpressLabelTemplate");
                    arcScale.LabelPresentation = scalelabelpresentation;

                    arcScale.LabelOptions = new ArcScaleLabelOptions();
                    arcScale.SpindleCapOptions = new SpindleCapOptions();

                    if (!circularGaugeObject.Scales.Contains(arcScale))
                        circularGaugeObject.Scales.Add(arcScale);
                }

                arcScale.ShowLine = DevExpress.Utils.DefaultBoolean.False;
                arcScale.LinePresentation = null;
                arcScale.SpindleCapOptions.ZIndex = 0;
                arcScale.SpindleCapOptions.FactorHeight = 0;
                arcScale.SpindleCapOptions.FactorWidth = 0;
                arcScale.SpindleCapPresentation = null;

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

                arcScale.ShowLabels = ShowLabels ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
            }
            catch (Exception)
            {
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
        protected override void SetEntityError(String error)
        {
        }
        protected override void UpdateScaleRanges()
        {
        }
        protected override void ShowMarker(bool v)
        {
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
        internal object LoadTemplate(string template)
        {
            try
            {
                string basename = "Templates.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(KnobExpress).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                object content = (object)canvas.TryFindResource(template);
                return (object)content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal Viewbox LoadBackControl(string template)
        {
            try
            {
                string basename = "Templates.xaml";
                Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                          String.Format("{0}.GaugeContainers.{1}", typeof(KnobExpress).Namespace, basename));
                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                if (obj == null)
                    return null;
                Canvas canvas = new Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                Viewbox content = (Viewbox)canvas.TryFindResource(template);
                return (Viewbox)content;
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
                        EnableAnimation = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontSize = 14,
                        FontStyle = FontStyles.Normal,
                        FontWeight = FontWeights.Medium,
                        FontFamily = new FontFamily("Segoe UI"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        ClipToBounds = false,
                        Background = Brushes.Transparent
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

                    Grid.SetZIndex(circularGaugeObject, 1);

                    if (!container.Children.Contains(circularGaugeObject))
                        container.Children.Add(circularGaugeObject);


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
                _value = 50; //(EndValue - StartValue) / 2;

            if (arcScaleMarker != null && !isChangingValue && !isUserMouseDownAction)
                arcScaleMarker.Value = _value;
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
        void UpdateEUnitControl()
        {
            if (gridValueContainer != null)
            {
                string eunit = TranslationHelpers.TranslationHelper.TranslateComposedText(EngeneeringUnit, stringlist, EngeneeringUnit);
                string meas = ConverterLabel ?? eunit;
                TextBlock label = gridValueContainer.GetChildrenOfType<TextBlock>().FirstOrDefault();
                if (label != null)
                    label.Text = meas;
                label.Visibility = string.IsNullOrEmpty(meas) ? Visibility.Collapsed : Visibility.Visible;
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

        public void KnobExpressInvokeAction()
        {
            //TODO: handle some operations over this object
        }
        #endregion
        #region IDisposable
        public override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            base.Dispose(true);

            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;


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

            if (arcScaleMarker != null)
            {
                arcScaleMarker.ValueChanged -= arcNeedle_ValueChanged;
                arcScaleMarker.Options = null;
                arcScaleMarker.Presentation = null;
                arcScaleMarker.Animation = null;
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
            arcScaleMarker = null;
            gridValueContainer = null;
            circularGaugeObject = null;
            
            DetachOverrideBaseProperties();
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
                    (document, typeof(KnobExpress), EngeneeringUnitProperty).DisplayName;
                map.Add(propertyName, EngeneeringUnit);
            }
            return map;
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
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                IWorkspace workspace = null;
                if (Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

                if (workspace != null)
                {
                    var dt = new DataTemplate();
                    var factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TextPropertyEditor));
                    factory.SetValue(WPFUtilities.PropertyDataTemplate.TextPropertyEditor.WorkspaceProperty, workspace);
                    dt.DataType = typeof(String);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(EngeneeringUnitProperty, dt);
                }
                return mapDataTemplates;
            }
        }

        #endregion
    }

    public enum PotenziometerStyle
    {
        Flat,
        Classic
    }
}
