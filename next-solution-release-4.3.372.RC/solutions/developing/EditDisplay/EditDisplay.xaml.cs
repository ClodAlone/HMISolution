using System.Windows.Data;
using System.Windows.Input;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Converters;
using Utilities;
using Utilities.WPF;
using System.Windows.Shapes;
using System.Text;
using Opc.Ua;
using UFInterfaces;
using StringManager.ComponentService;
using ScreenSettings;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Windows.Automation.Peers;
using EditDisplay.Automations;
using ViewModelLib;
using System.Windows.Media.Effects;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using System.Windows.Controls.Primitives;
using DynamicTagAwareHelper;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading.Tasks;
using UFInterfaces.PropertyControl;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using System.Threading;
using AuditTrace;
using StatDef;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ExpressionManager;
using Utilities.Converters;
using CommandManagerService.CommandManager;

namespace EditDisplay
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 
    public partial class EditDisplay : UserControl, IDisposable, IEntityReference, IDynamicTagAware, IContainPropertyEditors, IDataErrorInfo, IStatisticTagAware
        , IStringIDAware, IInheritPropertiesFromControl
    {
        #region Declarations

        Grid container;
        bool bLoaded;
        bool isTemplateApplied;
        List<string> matchChangedMap = new List<string>();
        TaskScheduler syncContextScheduler;
        ScreenDocument Document;
        TypeHelper typeHelper = new TypeHelper();

        MonitoredItemViewModel monitoredItemViewModel;
        AuditTraceViewModel auditTraceViewModel;

        DataValue lastDataValue;
        DispatcherOperation dpUpdateWarning;
        object lockObject = new object();

        int EnumStringCount;
        Border BackBorder;
        //ContentControl BackContent;
        PasswordBox PasswordBoxDisplay;
        TextBox StringDisplay;
        TextBlock MeasUnitTextBox;
        Ellipse OutOfRangeMarker;
        Image WarningMarker;
        Viewbox SpinView;
        Label labelUp;
        Path pathUp;
        Label labelDown;
        Path pathDown;
        RepeatButton spinUp;
        RepeatButton spinDown;
        static DataValue nullDataValue = new DataValue();
        string nullValue = nullDataValue.ToString();
        bool isStat = false;
        bool triggerDataContextChanged = true;
        string statsError = null;

        bool isTemporaryDisabled;
        DispatcherOperation dpdupdatelayout;
        //keep for retrocompatibility**********
        [Browsable(false)]
        public bool EnableBackgroundLayer {get; set;}
        [Browsable(false)]
        public bool CrystalStyle { get; set; }
        //*************************************
        FormatEnum displayType = FormatEnum.None;
        [Browsable(false)]
        public FormatEnum DisplayType
        {
            get
            {
                return displayType;
            }
            set
            {
                if(bLoaded && displayType != value)
                {
                    displayType = value;
                    if (isTemplateApplied)
                    {
                        UpdateEditDisplayLayout();
                        if (!bDesign)
                        {
                            ManageEventsHandler();
                            OnValueChanged(Value);
                            if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null &&
                                (displayType == FormatEnum.Digital || displayType == FormatEnum.Enumerated))
                                monitoredItemViewModel.NodeIdModel.PropertyChanged += NodeIdViewModel_PropertyChanged;
                        }
                    }
                    bInit = true;
                }
            }
        }

        string ConverterLabel;

        #endregion

        #region DP
        #region DP OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(HorizontalContentAlignmentProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnHorizontalContentAlignmentChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnIsEnabledChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(EditDisplay));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(HorizontalContentAlignmentProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnHorizontalContentAlignmentChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnIsEnabledChanged);

            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(EditDisplay));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
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
                var value = ValueFontSettings.Clone();
                value.FontFamily = FontFamily;
                ValueFontSettings = value;
                var mvalue = MeasFontSettings.Clone();
                mvalue.FontFamily = FontFamily;
                MeasFontSettings = mvalue;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
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
                var value = ValueFontSettings.Clone();
                value.FontWeight = FontWeight;
                ValueFontSettings = value;
                var mvalue = MeasFontSettings.Clone();
                mvalue.FontWeight = FontWeight;
                MeasFontSettings = mvalue;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
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
                var value = ValueFontSettings.Clone();
                value.FontStyle = FontStyle;
                ValueFontSettings = value;
                var mvalue = MeasFontSettings.Clone();
                mvalue.FontStyle = FontStyle;
                MeasFontSettings = mvalue;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
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
                var value = ValueFontSettings.Clone();
                value.FontSize = (int)FontSize;
                ValueFontSettings = value;
                var mvalue = MeasFontSettings.Clone();
                mvalue.FontSize = (int)FontSize;
                MeasFontSettings = mvalue;
            }
        }

        private void OnIsEnabledChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
            if (control != null)
            {
                control.OnIsEnabledChanged();
            }
        }
        protected virtual void OnIsEnabledChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (container == null)
                return;
            container.Opacity = IsEnabled ? 1.0 : 0.7;
        }
        private void OnHorizontalContentAlignmentChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
            if (control != null)
            {
                if (bLoaded && bInit)
                    control.OnHorizontalContentAlignmentChanged();
            }
        }
        protected virtual void OnHorizontalContentAlignmentChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.

            switch (HorizontalContentAlignment)
            {
                case HorizontalAlignment.Center:
                    LabelHorizontalAlignement = TextAlignment.Center;
                    break;
                case HorizontalAlignment.Left:
                    LabelHorizontalAlignement = TextAlignment.Left;
                    break;
                case HorizontalAlignment.Right:
                    LabelHorizontalAlignement = TextAlignment.Right;
                    break;
                case HorizontalAlignment.Stretch:
                    LabelHorizontalAlignement = TextAlignment.Justify;
                    break;
                default:
                    break;
            }
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (bInit)
            {
                UpdateForeground(); 
            }
        }

        private void UpdateForeground()
        {
            if (StringDisplay != null)
            {
                StringDisplay.Foreground = this.Foreground;
                StringDisplay.CaretBrush = this.Foreground;
            }
            if (PasswordBoxDisplay != null)
            {
                PasswordBoxDisplay.Foreground = this.Foreground;
                PasswordBoxDisplay.CaretBrush = this.Foreground;
            }
            if(MeasUnitTextBox != null)
            {
                MeasUnitTextBox.Foreground = this.Foreground;
            }
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as EditDisplay;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            if (BackBorder != null && !IsManipulationEnabled)
            if (BackBorder != null && !IsManipulationEnabled)
                BackBorder.Background = Background;
        }
        #endregion

        #region DisplayStyle

        #region ShowDigitGroupingSymbol
        public static readonly DependencyProperty ShowDigitGroupingSymbolProperty = DependencyProperty.Register("ShowDigitGroupingSymbol", typeof(bool), typeof(EditDisplay), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowDigitGroupingSymbolChanged), new CoerceValueCallback(OnCoerceShowDigitGroupingSymbol)));

        private static object OnCoerceShowDigitGroupingSymbol(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceShowDigitGroupingSymbol((bool)value);
            else
                return value;
        }

        private static void OnShowDigitGroupingSymbolChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnShowDigitGroupingSymbolChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowDigitGroupingSymbol(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowDigitGroupingSymbolChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        public bool ShowDigitGroupingSymbol
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowDigitGroupingSymbolProperty);
            }
            set
            {
                SetValue(ShowDigitGroupingSymbolProperty, value);
            }
        }

        #endregion


        #region ForegroundError
        public static readonly DependencyProperty ForegroundErrorProperty = DependencyProperty.Register("ForegroundError", typeof(Brush), typeof(EditDisplay), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xFF, 0x66, 0x00)), new PropertyChangedCallback(OnForegroundErrorChanged), new CoerceValueCallback(OnCoerceForegroundError)));

        private static object OnCoerceForegroundError(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceForegroundError((Brush)value);
            else
                return value;
        }

        private static void OnForegroundErrorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnForegroundErrorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceForegroundError(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnForegroundErrorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public Brush ForegroundError
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ForegroundErrorProperty);
            }
            set
            {
                SetValue(ForegroundErrorProperty, value);
            }
        }
        #endregion

        #region DisplayForeground
        //keep for retrocompatibility
        public static readonly DependencyProperty DisplayForegroundProperty = DependencyProperty.Register("DisplayForeground", typeof(Brush), typeof(EditDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        [Category("EditDisplayStyle")]
        [Browsable(false)]
        [XmlIgnore]
        [Obsolete("Use the Foreground property insted of this.")]
        public Brush DisplayForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(DisplayForegroundProperty);
            }
            set
            {
                SetValue(DisplayForegroundProperty, value);
            }
        }

        #endregion

        #region DisplayBorderBrush
        public static readonly DependencyProperty DisplayBorderBrushProperty = DependencyProperty.Register("DisplayBorderBrush", typeof(Brush), typeof(EditDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnDisplayBorderBrushChanged), new CoerceValueCallback(OnCoerceDisplayBorderBrush)));

        private static object OnCoerceDisplayBorderBrush(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceDisplayBorderBrush((Brush)value);
            else
                return value;
        }

        private static void OnDisplayBorderBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnDisplayBorderBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceDisplayBorderBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisplayBorderBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();

        }
        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public Brush DisplayBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(DisplayBorderBrushProperty);
            }
            set
            {
                SetValue(DisplayBorderBrushProperty, value);
            }
        }

        #endregion

        #region DisplayBorderThickness
        public static readonly DependencyProperty DisplayBorderThicknessProperty = DependencyProperty.Register("DisplayBorderThickness", typeof(int), typeof(EditDisplay), new UIPropertyMetadata(0, new PropertyChangedCallback(OnDisplayBorderThicknessChanged), new CoerceValueCallback(OnCoerceDisplayBorderThickness)));

        private static object OnCoerceDisplayBorderThickness(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceDisplayBorderThickness((int)value);
            else
                return value;
        }

        private static void OnDisplayBorderThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnDisplayBorderThicknessChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceDisplayBorderThickness(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisplayBorderThicknessChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();

        }
        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public int DisplayBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(DisplayBorderThicknessProperty);
            }
            set
            {
                SetValue(DisplayBorderThicknessProperty, value);
            }
        }

        #endregion


        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(EditDisplay), new UIPropertyMetadata(new System.Windows.CornerRadius(0), new PropertyChangedCallback(OnCornerRadiusChanged), new CoerceValueCallback(OnCoerceCornerRadius)));

        private static object OnCoerceCornerRadius(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceCornerRadius((CornerRadius)value);
            else
                return value;
        }

        private static void OnCornerRadiusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnCornerRadiusChanged((CornerRadius)e.OldValue, (CornerRadius)e.NewValue);
        }

        protected virtual CornerRadius OnCoerceCornerRadius(CornerRadius value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit) && BackBorder != null)
                BackBorder.CornerRadius = CornerRadius;
        }

        public CornerRadius CornerRadius
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        #endregion


        #region SpinButtonBackColor
        public static readonly DependencyProperty SpinButtonBackColorProperty = DependencyProperty.Register("SpinButtonBackColor", typeof(Brush), typeof(EditDisplay), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSpinButtonBackColorChanged), new CoerceValueCallback(OnCoerceSpinButtonBackColor)));

        private static object OnCoerceSpinButtonBackColor(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceSpinButtonBackColor((Brush)value);
            else
                return value;
        }

        private static void OnSpinButtonBackColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnSpinButtonBackColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSpinButtonBackColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpinButtonBackColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSpinButtonBackColor();
        }
        [SvgValueConverter(typeof(ConvertSpinBackColor), RequiredKey = true)]
        public Brush SpinButtonBackColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SpinButtonBackColorProperty);
            }
            set
            {
                SetValue(SpinButtonBackColorProperty, value);
            }
        }

        #endregion


        #region SpinBackColor
        public static readonly DependencyProperty SpinBackColorProperty = DependencyProperty.Register("SpinBackColor", typeof(Brush), typeof(EditDisplay), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSpinBackColorChanged), new CoerceValueCallback(OnCoerceSpinBackColor)));

        private static object OnCoerceSpinBackColor(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceSpinBackColor((Brush)value);
            else
                return value;
        }

        private static void OnSpinBackColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnSpinBackColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceSpinBackColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpinBackColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateSpinButtonBackColor(); 
        }
        [Category("EditDisplayStyle")]
        [Browsable(true)]
        [SvgValueConverter(typeof(ConvertSpinBackColor), RequiredKey = true, NeedSVGUrlBrushes = true)]
        public Brush SpinBackColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SpinBackColorProperty);
            }
            set
            {
                SetValue(SpinBackColorProperty, value);
            }
        }

        #endregion

        #region DP MeasUnit

        public static readonly DependencyProperty MeasUnitProperty = DependencyProperty.Register("MeasUnit", typeof(String), typeof(EditDisplay), new UIPropertyMetadata(String.Empty, new PropertyChangedCallback(OnMeasUnitChanged), new CoerceValueCallback(OnCoerceMeasUnit)));

        private static object OnCoerceMeasUnit(DependencyObject o, object value)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
                return display.OnCoerceMeasUnit((String)value);
            else
                return value;
        }

        private static void OnMeasUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
                display.OnMeasUnitChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceMeasUnit(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMeasUnitChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        [Category("EditDisplayStyle")]
        public String MeasUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(MeasUnitProperty);
            }
            set
            {
                SetValue(MeasUnitProperty, value);
            }
        }


        #endregion
        #region ValueFontSettings
        public static readonly DependencyProperty ValueFontSettingsProperty = DependencyProperty.Register("ValueFontSettings", typeof(FontSettings), typeof(EditDisplay), new UIPropertyMetadata(new FontSettings(FontWeights.SemiBold, FontStyles.Normal, new FontFamily("Segoe UI"), 20), new PropertyChangedCallback(OnValueFontSettingsChanged), new CoerceValueCallback(OnCoerceValueFontSettings)));

        private static object OnCoerceValueFontSettings(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceValueFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnValueFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnValueFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceValueFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
            {
                if (StringDisplay != null)
                    UpdateFont(StringDisplay, newValue);
                if (PasswordBoxDisplay != null)
                    UpdateFont(PasswordBoxDisplay, newValue);
            }
        }

        bool bOverride;
        [Category("EditDisplayStyle")]
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

        #region MeasFontSettings
        public static readonly DependencyProperty MeasFontSettingsProperty = DependencyProperty.Register("MeasFontSettings", typeof(FontSettings), typeof(EditDisplay), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 14), new PropertyChangedCallback(OnMeasFontSettingsChanged), new CoerceValueCallback(OnCoerceMeasFontSettings)));

        private static object OnCoerceMeasFontSettings(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceMeasFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnMeasFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnMeasFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceMeasFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMeasFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateMeasUnit();
        }

        [Category("EditDisplayStyle")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings MeasFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(MeasFontSettingsProperty);
            }
            set
            {
                SetValue(MeasFontSettingsProperty, value);
            }
        }
        #endregion


        #region DP SpinPosition
        public static readonly DependencyProperty SpinPositionProperty = DependencyProperty.Register("SpinPosition", typeof(SpinRelativePosition), typeof(EditDisplay), new UIPropertyMetadata(SpinRelativePosition.Right, new PropertyChangedCallback(OnSpinPositionChanged), new CoerceValueCallback(OnCoerceSpinPosition)));

        private static object OnCoerceSpinPosition(DependencyObject o, object value)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                return display.OnCoerceSpinPosition((SpinRelativePosition)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnSpinPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                display.OnSpinPositionChanged((SpinRelativePosition)e.OldValue, (SpinRelativePosition)e.NewValue);
            }
        }

        protected virtual SpinRelativePosition OnCoerceSpinPosition(SpinRelativePosition value)
        {
            return value;
        }

        protected virtual void OnSpinPositionChanged(SpinRelativePosition oldValue, SpinRelativePosition newValue)
        {
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayStyle")]
        public SpinRelativePosition SpinPosition
        {
            get
            {
                return (SpinRelativePosition)GetValue(SpinPositionProperty);
            }
            set
            {
                SetValue(SpinPositionProperty, value);
            }
        }
        #endregion

        #region DP SpinDimension
        public static readonly DependencyProperty SpinDimensionProperty = DependencyProperty.Register("SpinDimension", typeof(SpinRelativeDimension), typeof(EditDisplay), new UIPropertyMetadata(SpinRelativeDimension.Small, new PropertyChangedCallback(OnSpinDimensionChanged), new CoerceValueCallback(OnCoerceSpinDimension)));

        private static object OnCoerceSpinDimension(DependencyObject o, object value)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                return display.OnCoerceSpinDimension((SpinRelativeDimension)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnSpinDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                display.OnSpinDimensionChanged((SpinRelativeDimension)e.OldValue, (SpinRelativeDimension)e.NewValue);
            }
        }

        protected virtual SpinRelativeDimension OnCoerceSpinDimension(SpinRelativeDimension value)
        {
            return value;
        }

        protected virtual void OnSpinDimensionChanged(SpinRelativeDimension oldValue, SpinRelativeDimension newValue)
        {
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        [Category("EditDisplayStyle")]
        public SpinRelativeDimension SpinDimension
        {
            get
            {
                return (SpinRelativeDimension)GetValue(SpinDimensionProperty);
            }
            set
            {
                SetValue(SpinDimensionProperty, value);
            }
        }
        #endregion

        #region DP SpinEnabled
        public static readonly DependencyProperty SpinEnabledProperty = DependencyProperty.Register("SpinEnabled", typeof(Boolean), typeof(EditDisplay), new UIPropertyMetadata(false, new PropertyChangedCallback(OnSpinEnabledChanged), new CoerceValueCallback(OnCoerceSpinEnabled)));

        private static object OnCoerceSpinEnabled(DependencyObject o, object value)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                return display.OnCoerceSpinEnabled((Boolean)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnSpinEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                display.OnSpinEnabledChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
            }
        }

        protected virtual Boolean OnCoerceSpinEnabled(Boolean value)
        {
            return value;
        }

        protected virtual void OnSpinEnabledChanged(Boolean oldValue, Boolean newValue)
        {
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public Boolean SpinEnabled
        {
            get
            {
                return (Boolean)GetValue(SpinEnabledProperty);
            }
            set
            {
                SetValue(SpinEnabledProperty, value);
            }
        }
        #endregion

        #region DP LabelHorizontalAlignment

        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.Register("LabelHorizontalAlignement", typeof(TextAlignment), typeof(EditDisplay), new UIPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnAlignmentChanged), new CoerceValueCallback(OnCoerceAlignment)));

        private static object OnCoerceAlignment(DependencyObject o, object value)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
                return display.OnCoerceAlignment((TextAlignment)value);
            else
                return value;
        }

        private static void OnAlignmentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
                display.OnAlignmentChanged((TextAlignment)e.OldValue, (TextAlignment)e.NewValue);
        }

        protected virtual TextAlignment OnCoerceAlignment(TextAlignment value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAlignmentChanged(TextAlignment oldValue, TextAlignment newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }


        [Category("EditDisplayStyle")]
        public TextAlignment LabelHorizontalAlignement
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextAlignment)GetValue(AlignmentProperty);
            }
            set
            {
                SetValue(AlignmentProperty, value);
            }
        }



        #endregion

        #region LabelVerticalAlignement
        public static readonly DependencyProperty LabelVerticalAlignementProperty = DependencyProperty.Register("LabelVerticalAlignement", typeof(VerticalAlignment), typeof(EditDisplay), new UIPropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnLabelVerticalAlignementChanged), new CoerceValueCallback(OnCoerceLabelVerticalAlignement)));

        private static object OnCoerceLabelVerticalAlignement(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceLabelVerticalAlignement((VerticalAlignment)value);
            else
                return value;
        }

        private static void OnLabelVerticalAlignementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnLabelVerticalAlignementChanged((VerticalAlignment)e.OldValue, (VerticalAlignment)e.NewValue);
        }

        protected virtual VerticalAlignment OnCoerceLabelVerticalAlignement(VerticalAlignment value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelVerticalAlignementChanged(VerticalAlignment oldValue, VerticalAlignment newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayStyle")]
        public VerticalAlignment LabelVerticalAlignement
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (VerticalAlignment)GetValue(LabelVerticalAlignementProperty);
            }
            set
            {
                SetValue(LabelVerticalAlignementProperty, value);
            }
        }


        #endregion

        #region MeasUnitOffset
        public static readonly DependencyProperty MeasUnitOffsetProperty = DependencyProperty.Register("MeasUnitOffset", typeof(Thickness), typeof(EditDisplay), new UIPropertyMetadata(new Thickness(0, 0, 4, 0), new PropertyChangedCallback(OnMeasUnitOffsetChanged), new CoerceValueCallback(OnCoerceMeasUnitOffset)));

        private static object OnCoerceMeasUnitOffset(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceMeasUnitOffset((Thickness)value);
            else
                return value;
        }

        private static void OnMeasUnitOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnMeasUnitOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
        }

        protected virtual Thickness OnCoerceMeasUnitOffset(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMeasUnitOffsetChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayStyle")]
        public Thickness MeasUnitOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(MeasUnitOffsetProperty);
            }
            set
            {
                SetValue(MeasUnitOffsetProperty, value);
            }
        }

        #endregion

        #region LabelOffset
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register("LabelOffset", typeof(Thickness), typeof(EditDisplay), new UIPropertyMetadata(new Thickness(2, 0, 2, 0), new PropertyChangedCallback(OnLabelOffsetChanged), new CoerceValueCallback(OnCoerceLabelOffset)));

        private static object OnCoerceLabelOffset(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceLabelOffset((Thickness)value);
            else
                return value;
        }

        private static void OnLabelOffsetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnLabelOffsetChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
        }

        protected virtual Thickness OnCoerceLabelOffset(Thickness value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLabelOffsetChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();

        }
        [Category("EditDisplayStyle")]
        public Thickness LabelOffset
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(LabelOffsetProperty);
            }
            set
            {
                SetValue(LabelOffsetProperty, value);
            }
        }
        #endregion


        #region TextWrapping
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(EditDisplay), new UIPropertyMetadata(TextWrapping.NoWrap, new PropertyChangedCallback(OnTextWrappingChanged), new CoerceValueCallback(OnCoerceTextWrapping)));

        private static object OnCoerceTextWrapping(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceTextWrapping((TextWrapping)value);
            else
                return value;
        }

        private static void OnTextWrappingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnTextWrappingChanged((TextWrapping)e.OldValue, (TextWrapping)e.NewValue);
        }

        protected virtual TextWrapping OnCoerceTextWrapping(TextWrapping value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextWrappingChanged(TextWrapping oldValue, TextWrapping newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayStyle")]
        public TextWrapping TextWrapping
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TextWrapping)GetValue(TextWrappingProperty);
            }
            set
            {
                SetValue(TextWrappingProperty, value);
            }
        }

        #endregion

        #endregion

        #region EditDisplayOptions

        #region DP SpinStep
        public static readonly DependencyProperty SpinStepProperty = DependencyProperty.Register("SpinStep", typeof(decimal), typeof(EditDisplay), new UIPropertyMetadata((decimal)1));
        [Category("EditDisplayOptions")]
        public decimal SpinStep
        {
            get
            {
                return (decimal)GetValue(SpinStepProperty);
            }
            set
            {
                SetValue(SpinStepProperty, value);
            }
        }
        #endregion

        #region DP OutOfRange

        public static readonly DependencyProperty OutOfRangeProperty = DependencyProperty.Register("OutOfRange", typeof(Boolean), typeof(EditDisplay), new UIPropertyMetadata(true));
        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public Boolean OutOfRange
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(OutOfRangeProperty);
            }
            set
            {
                SetValue(OutOfRangeProperty, value);
            }
        }


        #endregion

        #region DP UseEUnit

        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(Boolean), typeof(EditDisplay), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));
        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
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
            {
                if (!bDesign)
                {
                    UpdateMonitoredMinMaxValues(newValue);
                }
            }
        }

        private void UpdateMonitoredMinMaxValues(bool newValue)
        {
            if (newValue && HasRange && MonitoredRange != null)
            {
                if (TagMinValue == null || TagMinValue.TagReference == null)
                    MinValue = System.Convert.ToDecimal(MonitoredRange.Low, CultureInfo.InvariantCulture);

                if (TagMaxValue == null || TagMaxValue.TagReference == null)
                    MaxValue = System.Convert.ToDecimal(MonitoredRange.High, CultureInfo.InvariantCulture);
            }
        }

        [Category("EditDisplayStyle")]
        [Browsable(true)]
        public Boolean UseEUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(UseEUnitProperty);
            }
            set
            {
                SetValue(UseEUnitProperty, value);
            }
        }


        #endregion

        #region DP MinValue

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(decimal), typeof(EditDisplay), new UIPropertyMetadata((decimal)0));
        [Category("EditDisplayOptions")]
        [Browsable(true)]
        public decimal MinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }


        #endregion

        #region DP MaxValue

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(decimal), typeof(EditDisplay), new UIPropertyMetadata((decimal)100));
        [Category("EditDisplayOptions")]
        [Browsable(true)]
        public decimal MaxValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }


        #endregion

        #region TagMinValue
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(EditDisplay), new UIPropertyMetadata(null));
        [Category("EditDisplayOptions")]
        [DisplayNameExtension]
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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(EditDisplay), new UIPropertyMetadata(null));
        [Category("EditDisplayOptions")]
        [DisplayNameExtension]
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


        #region IsReadOnly
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(Boolean), typeof(EditDisplay), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged), new CoerceValueCallback(OnCoerceIsReadOnly)));

        private static object OnCoerceIsReadOnly(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceIsReadOnly((Boolean)value);
            else
                return value;
        }

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnIsReadOnlyChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceIsReadOnly(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsReadOnlyChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
            {
                if (!bDesign)
                {
                    UpdateEditDisplayLayout();
                }
            }
        }
        [Category("EditDisplayOptions")]
        [Browsable(true)]
        public Boolean IsReadOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        #endregion

        #region PasswordStyle
        public static readonly DependencyProperty PasswordStyleProperty = DependencyProperty.Register("PasswordStyle", typeof(bool), typeof(EditDisplay), new UIPropertyMetadata(false, new PropertyChangedCallback(OnPasswordStyleChanged), new CoerceValueCallback(OnCoercePasswordStyle)));

        private static object OnCoercePasswordStyle(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoercePasswordStyle((bool)value);
            else
                return value;
        }

        private static void OnPasswordStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnPasswordStyleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoercePasswordStyle(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPasswordStyleChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayOptions")]
        [Browsable(true)]
        public bool PasswordStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(PasswordStyleProperty);
            }
            set
            {
                SetValue(PasswordStyleProperty, value);
            }
        }

        #endregion


        #region DP PrecisionDigits

        public static readonly DependencyProperty PrecisionDigitsProperty = DependencyProperty.Register("PrecisionDigits", typeof(int), typeof(EditDisplay), new UIPropertyMetadata((int)0, new PropertyChangedCallback(OnPrecisionDigitsChanged), new CoerceValueCallback(OnCoercePrecisionDigits)));

        private static object OnCoercePrecisionDigits(DependencyObject o, object value)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                return display.OnCoercePrecisionDigits((int)value);
            }
            else
            {
                return value;
            }
        }

        private static void OnPrecisionDigitsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var display = o as EditDisplay;
            if (display != null)
            {
                display.OnPrecisionDigitsChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        protected virtual int OnCoercePrecisionDigits(int value)
        {
            return value;
        }

        protected virtual void OnPrecisionDigitsChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }
        [Category("EditDisplayOptions")]
        public int PrecisionDigits
        {
            get
            {
                return (int)GetValue(PrecisionDigitsProperty);
            }
            set
            {
                SetValue(PrecisionDigitsProperty, value);
            }
        }
        #endregion


        #region DisplayStringFormat
        public static readonly DependencyProperty DisplayStringFormatProperty = DependencyProperty.Register("DisplayStringFormat", typeof(string), typeof(EditDisplay), new UIPropertyMetadata("x", new PropertyChangedCallback(OnDisplayStringFormatChanged), new CoerceValueCallback(OnCoerceDisplayStringFormat)));

        private static object OnCoerceDisplayStringFormat(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceDisplayStringFormat((string)value);
            else
                return value;
        }

        private static void OnDisplayStringFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnDisplayStringFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDisplayStringFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisplayStringFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        [Category("EditDisplayOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public string DisplayStringFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DisplayStringFormatProperty);
            }
            set
            {
                SetValue(DisplayStringFormatProperty, value);
            }
        }
        string format;
        #endregion


        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(String), typeof(EditDisplay), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                return editDisplay.OnCoerceValue((String)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay editDisplay = o as EditDisplay;
            if (editDisplay != null)
                editDisplay.OnValueChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceValue(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnValueChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                OnValueChanged(newValue);
        }
        public String Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public string GetDisplayValue()
        {
            switch (DisplayType)
            {
                case FormatEnum.Integer:
                case FormatEnum.Numeric:
                case FormatEnum.String:
                case FormatEnum.Digital:
                case FormatEnum.Boolean:
                    if (StringDisplay != null)
                        return StringDisplay.Text;
                    else if (PasswordBoxDisplay != null)
                        return PasswordBoxDisplay.Password;
                    else
                        return string.Empty;
                case FormatEnum.Enumerated:
                    object parsevalue;
                    if (StringDisplay != null)
                    {
                        if (TryParseValue(StringDisplay.Text, out parsevalue))
                            return parsevalue.ToString();
                        else
                            return StringDisplay.Text;
                    }
                    else if (PasswordBoxDisplay != null)
                    {
                        if (TryParseValue(PasswordBoxDisplay.Password, out parsevalue))
                            return parsevalue.ToString();
                        else
                            return PasswordBoxDisplay.Password;
                    }
                    else
                        return string.Empty;
                default:
                    return string.Empty;
            }

        }
        internal void SetDisplayValue(string value)
        {
            ManageUpdateBinding(null, null, value);
        }
        #endregion

        #region StatDef
        public static readonly DependencyProperty StatDefProperty = DependencyProperty.Register("StatDef", typeof(StatProps), typeof(EditDisplay), new PropertyMetadata(StatProps.None, new PropertyChangedCallback(OnStatDefChanged), new CoerceValueCallback(OnCoerceStatDef)));

        private static object OnCoerceStatDef(DependencyObject o, object value)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                return control.OnCoerceStatDef((StatProps)value);
            else
                return value;
        }

        private static void OnStatDefChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay control = o as EditDisplay;
            if (control != null)
                control.OnStatDefChanged((StatProps)e.OldValue, (StatProps)e.NewValue);
        }

        protected virtual StatProps OnCoerceStatDef(StatProps value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStatDefChanged(StatProps oldValue, StatProps newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !bDispose && !DesignerProperties.GetIsInDesignMode(this))
            {
                string statsError = null;
                var statItem = GetMonitoredStat(ref statsError);
                if (statItem != null)
                    UpdateMonitoredItem(statItem);
            }
        }

        [Category("EditDisplayOptions")]
        public StatProps StatDef
        {
            get
            {
                return (StatProps)base.GetValue(StatDefProperty);
            }
            set
            {
                base.SetValue(StatDefProperty, value);
            }
        }
        #endregion

        #region TimespanFormat
        public static readonly DependencyProperty TimespanFormatProperty = DependencyProperty.Register("TimespanFormat", typeof(string), typeof(EditDisplay), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTimespanFormatChanged), new CoerceValueCallback(OnCoerceTimespanFormat)));

        private static object OnCoerceTimespanFormat(DependencyObject o, object value)
        {
            EditDisplay EditDisplay = o as EditDisplay;
            if (EditDisplay != null)
                return EditDisplay.OnCoerceTimespanFormat((string)value);
            else
                return value;
        }

        private static void OnTimespanFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay EditDisplay = o as EditDisplay;
            if (EditDisplay != null)
                EditDisplay.OnTimespanFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTimespanFormat(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTimespanFormatChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("EditDisplayOptions")]
        public string TimespanFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TimespanFormatProperty);
            }
            set
            {
                SetValue(TimespanFormatProperty, value);
            }
        }
        #endregion

        //Keep the following code to ensure compatibility with older projects
        #region Numeric/NormalizedValue
        public static readonly DependencyProperty NumericValueProperty = DependencyProperty.Register("NumericValue", typeof(Decimal), typeof(EditDisplay), new UIPropertyMetadata((Decimal)0));
        [Browsable(false)]
        [XmlIgnore]
        public Decimal NumericValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (Decimal)GetValue(NumericValueProperty); }
            set { SetValue(NumericValueProperty, value); }
        }

        public static readonly DependencyProperty NormalizedValueProperty = DependencyProperty.Register("NormalizedValue", typeof(String), typeof(EditDisplay), new UIPropertyMetadata(null));
        [Browsable(false)]
        [XmlIgnore]
        public String NormalizedValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get { return (String)GetValue(NormalizedValueProperty); }
            set { SetValue(NormalizedValueProperty, value); }
        }

        #endregion

        #region DP FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(
            "FormatString", typeof(String), typeof(EditDisplay),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnFormatStringChanged), new CoerceValueCallback(OnCoerceFormatString)));

        private static object OnCoerceFormatString(DependencyObject o, object value)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
                return display.OnCoerceFormatString((String)value);
            else
                return value;
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EditDisplay display = o as EditDisplay;
            if (display != null)
            {
                if ((String)e.NewValue== "")
                {
                    display.SetValue(FormatStringProperty, null);
                }
                display.OnFormatStringChanged((String)e.OldValue, (String)e.NewValue);
            }
        }

        protected virtual String OnCoerceFormatString(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFormatStringChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (bLoaded && bInit))
                UpdateEditDisplayLayout();
        }

        public String FormatString
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(FormatStringProperty);
            }
            set
            {
                SetValue(FormatStringProperty, value);
            }
        }


        #endregion

        #endregion

        [Browsable(false)]
        public bool RunningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }

        #endregion

        #region Events
        private void Up_Click_1(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly || isTemporaryDisabled || bDesign)
                return;
            OnIncrease();
        }

        private void Down_Click_1(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly || isTemporaryDisabled || bDesign)
                return;
            OnDecrease();
        }

        private void NodeIdViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NodeProperties")
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDispose)
                        return;

                    OnValueChanged(Value);
                });
            }
        }
        #endregion

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        #endregion

        #region Constructor
        bool bInit;
        private bool bDesign;
        public EditDisplay()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            OverrideBaseProperties();
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;
                    if (this.ReadLocalValue(HorizontalContentAlignmentProperty) != DependencyProperty.UnsetValue)
                        OnHorizontalContentAlignmentChanged();

                    InitPrecision();

                    if (Document != null)
                    {
                        if (stringManager == null)
                            stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringManager != null)
                        {
                            stringManager.CultureChanged += StringManager_CultureChanged;
                            UpdateStringList();
                        }
                    }

                    if (!bDesign)
                    {
                        InitControl();
                        if (syncContextScheduler == null)
                            syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                        if (DisplayType == FormatEnum.None)
                            UpdateDisplayType();
                    }

                    if (DisplayType == FormatEnum.None)
                        DisplayType = FormatEnum.String;

                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDispose || !triggerDataContextChanged)
                    return;

                if (syncContextScheduler == null)
                    syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this))
                {
                    if (monitoredItemViewModel != null)
                    {
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                        if (monitoredItemViewModel.NodeIdModel != null)
                            monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;
                    }

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    if (monitoredItemViewModel.ReferenceViewModel != null)
                        monitoredItemViewModel = monitoredItemViewModel.ReferenceViewModel;

                    UpdateMonitoredItem();

                    if (bDataTypeFetched && monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null && 
                        (displayType == FormatEnum.Digital || displayType == FormatEnum.Enumerated))
                        monitoredItemViewModel.NodeIdModel.PropertyChanged += NodeIdViewModel_PropertyChanged;
                }
            };
        }
        #endregion

        #region Methods
        void UpdateStringList()
        {
            bool bUntranslated = bDesign && stringManager?.GetActiveCulture(Document, false) == String.Empty;
            if (!bUntranslated)
                stringlist = stringManager?.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
            else
                stringlist = null;
        }
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDispose || !bInit)
                    return;
                UpdateStringList();
                UpdateMeasUnit();
            });
        }
        void UpdateMonitoredItem(MonitoredItemViewModel statItem = null)
        {
            statsError = null;
            isStat = false;
            if (statItem == null)
                statItem = GetMonitoredStat(ref statsError);
            if (statItem != null || (statItem == null && statsError != null))
            {
                isStat = true;
                triggerDataContextChanged = false;
                DataContext = statItem;
                triggerDataContextChanged = true;
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                if (monitoredItemViewModel.NodeIdModel != null)
                    monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;
                monitoredItemViewModel = statItem;
                if (statsError != null)
                    ManageWarning();
            }

            if (monitoredItemViewModel != null)
            {
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                if (isStat && !IsReadOnly && bInit)
                    UpdateEditDisplayLayout();
                else if (!RunningOnServer && Document != null && monitoredItemViewModel.monitoredItem != null)
                {
                    isTemporaryDisabled = true;
                    if (!IsReadOnly && bInit)
                        UpdateEditDisplayLayout();

                    var bindingExpression = GetBindingExpression(ValueProperty);
                    if (auditTraceViewModel != null)
                    {
                        auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                        auditTraceViewModel.Dispose();
                    }
                    IValueConverter converter = bindingExpression != null ? bindingExpression.ParentBinding.Converter : null;
                    var tempVariable = DataContext as MonitoredItemViewModel;
                    if (tempVariable != monitoredItemViewModel)
                    {
                        var expressionEntity = ExpressionBucket.GetInstance(Document).GetExpression(tempVariable);
                        if (expressionEntity != null)
                        {
                            if (converter == null)
                            {
                                converter = expressionEntity.Converter;
                            }
                            else
                            {
                                var converter1 = converter;
                                converter = new CombiningConverter()
                                {
                                    Converter1 = converter1,
                                    Converter2 = expressionEntity.Converter
                                };
                            }
                        }
                    }
                    auditTraceViewModel = new AuditTraceViewModel(monitoredItemViewModel, this, Document, Document.SessionString)
                    {
                        Control = this,
                        Converter = converter,
                        ConverterParameter = bindingExpression != null ? bindingExpression.ParentBinding.ConverterParameter : null,
                    };
                    auditTraceViewModel.AuditPropertiesFetched += OnAuditFetched;
                }

                if (cts == null)
                    cts = new CancellationTokenSource();

                monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
            }
        }

        void OnAuditFetched(object s, EventArgs ev)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                isTemporaryDisabled = false;
                if (!IsReadOnly && !isStat && bInit)
                    UpdateEditDisplayLayout();
            });
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            container = (this.GetVisualChild(0) as Border).Child as Grid;
            if (!isTemplateApplied /*&& RunningOnServer*/)
            {
                UpdateEditDisplayLayout();
                if (!DesignerProperties.GetIsInDesignMode(this))
                {
                    ManageEventsHandler();
                    OnValueChanged(Value);
                }
            }
            isTemplateApplied = true;
        }

        MonitoredItemViewModel GetMonitoredStat(ref string statsError)
        {
            if (monitoredItemViewModel == null)
                return null;

            MonitoredItemViewModel statItem = null;
            if (StatisticsDefinitions.statParams.ContainsKey(StatDef))
            {
                var statName = StatisticsDefinitions.statParams[StatDef];
                statItem = monitoredItemViewModel.GetChildItem(statName);
                if (statItem == null)
                {
                    var errorStr = monitoredItemViewModel.monitoredItem.Status.Error.ToString();
                    if (!string.IsNullOrEmpty(errorStr))
                        statsError = errorStr;
                }
            }
            return statItem;
        }

        bool bDataTypeFetched;
        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue")
            {
                MonitoredItemViewModel m = (MonitoredItemViewModel)sender;

                if (!bDataTypeFetched && !m.IsReadOnly && m.DataValue != null && Opc.Ua.StatusCode.IsGood(m.DataValue.StatusCode))
                {
                    bDataTypeFetched = true;
                    UpdateDataType(m);
                }

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
        bool isInError;
        private void ManageWarning()
        {
            if (container == null)
                return;
            DataValue dataValue = null;
            lock (lockObject)
            {
                dataValue = lastDataValue;
                lastDataValue = null;
            }

            if ((dataValue != null && Opc.Ua.StatusCode.IsBad(dataValue.StatusCode)) || statsError != null)
            {
                if (isInError)
                    return;
                isInError = true;
                UpdateText(string.Empty);
                container.ToolTip = statsError != null ? string.Format(Properties.Resources.MissingStatsWarning, statsError) : Properties.Resources.NullValueWarning;
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

                    Grid.SetColumn(WarningMarker, 1);
                    Grid.SetZIndex(WarningMarker, 20);
                }
            }
            else
            {
                if (!isInError)
                    return;
                isInError = false;
                OnValueChanged(Value);
                container.ToolTip = null;
                if (WarningMarker != null)
                {
                    if (container.Children.Contains(WarningMarker))
                        container.Children.Remove(WarningMarker);
                    WarningMarker = null;
                }
            }
        }

        private void UpdateDataType(MonitoredItemViewModel monitoreditem)
        {
            var token = cts != null ? cts.Token : CancellationToken.None;
            var task1 = Task.Factory.StartNew(() =>
            {
                if (token.IsCancellationRequested)
                    return;

                DataType = monitoreditem.DataType;
                MonitoredRange = monitoreditem.Range;
                MonitoredUnit = monitoreditem.EUInformation?.DisplayName?.ToString();
                HasRange = monitoreditem.HasRange;
                DataValue = monitoreditem.DataValue;
                MonitoredValue = monitoreditem.Value;
                DataValueCollection = monitoreditem.DataValueCollection;
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            if(syncContextScheduler != null)
            {
                var task2 = task1.ContinueWith(ret =>
                {
                    if (token.IsCancellationRequested)
                        return;
                    UpdateMonitoredMinMaxValues(UseEUnit);
                    UpdateDisplayType();
                }, syncContextScheduler);
            }
        }
        void UpdateDisplayType()
        {
            Object parsevalue;
            if (!TryGetDisplayType(out parsevalue))
            {
                DisplayType = FormatEnum.String;
            }
        }
        private string GetValue(ValueOptions options)
        {
            if (options?.Value == null)
                return null;

            string _sval = options.Value;
            CultureInfo culture = options.Culture;
            if (options.Value.IndexOf(':') >= 0)
            {
                _sval = options.Value.Substring(options.Value.IndexOf(':') + 1);
                culture = new CultureInfo(options.Value.Substring(0, options.Value.IndexOf(':')));
            }
            
            string ret = string.Empty;
            try
            {
                switch (options.DisplayType)
                {
                    case FormatEnum.Integer:
                        if (String.Compare(_sval, "True", true) == 0 || String.Compare(_sval as String, "False", true) == 0)
                        {
                            //displayType = FormatEnum.Boolean;
                            //ForceBoolean();
                            options.ForceBoolean = true;
                            options.IsOutOfRange = false;
                            ret = _sval;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_sval) && _sval != nullValue)
                            {
                                Decimal val = Decimal.Parse(_sval, culture);
                                options.IsOutOfRange = val > options.MaxValue || val < options.MinValue ? true : false;

                                Decimal newval = val / (Decimal)Math.Pow(10, options.PrecisionDigits);
                                ret = newval.ToString(format, options.Culture);
                            }
                            else
                            {
                                ret = _sval;
                            }
                        }
                        break;
                    case FormatEnum.Numeric:
                        if (isStat)
                        {
                            var parsed = false;
                            if (options.StatDef == StatProps.TotalTimeOn && options.TimespanFormat != null)
                            {
                                try
                                {
                                    var timespanVal = TimeSpan.Parse(_sval, System.Globalization.CultureInfo.InvariantCulture);
                                    var totalTimeHeader = String.Empty;
                                    var formatTail = options.TimespanFormat;
                                    var firstBlockMatch = Regex.Match(options.TimespanFormat, @"([a-zA-Z])\1");
                                    if (firstBlockMatch.Success)
                                    {
                                        formatTail = options.TimespanFormat.Substring(options.TimespanFormat.IndexOf(firstBlockMatch.ToString()) + firstBlockMatch.ToString().Length);
                                        char firstBlockType = firstBlockMatch.ToString().ToLower()[0];
                                        switch (firstBlockType)
                                        {
                                            case 'd':
                                                totalTimeHeader = ((int)timespanVal.TotalDays).ToString();
                                                break;
                                            case 'h':
                                                totalTimeHeader = ((int)timespanVal.TotalHours).ToString();
                                                break;
                                            case 'm':
                                                totalTimeHeader = ((int)timespanVal.TotalMinutes).ToString();
                                                break;
                                            case 's':
                                                totalTimeHeader = ((int)timespanVal.TotalSeconds).ToString();
                                                break;
                                            case 'f':
                                                totalTimeHeader = ((int)timespanVal.TotalMilliseconds).ToString();
                                                break;
                                        }
                                    }
                                    ret = String.Format("{0}{1}", totalTimeHeader, timespanVal.ToString(String.Format("{0}", formatTail)));
                                    parsed = true;
                                }
                                catch (FormatException) { }
                            }
                            if (!parsed && StatisticsDefinitions.statFormats.ContainsKey(options.StatDef))
                            {
                                ret = StatisticsDefinitions.statFormats[options.StatDef](_sval).ToString();
                                parsed = true;
                            }
                            if (parsed)
                                break;
                        }
                        if (String.Compare(_sval, "True", true) == 0 || String.Compare(_sval as String, "False", true) == 0)
                        {
                            displayType = FormatEnum.Boolean;
                            //ForceBoolean();
                            options.ForceBoolean = true;
                            options.IsOutOfRange = false;
                            ret = _sval;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_sval) && _sval != nullValue)
                            {
                                Double val = Double.Parse(_sval, culture);
                                options.IsOutOfRange = val > (double)options.MaxValue || val < (double)options.MinValue ? true : false;
                                ret = val.ToString(format, options.Culture);
                            }
                            else
                            {
                                ret = _sval;
                            }
                        }
                        break;
                    case FormatEnum.String:
                        options.IsOutOfRange = _sval.Length > options.MaxValue || _sval.Length < options.MinValue ? true : false;
                        ret = _sval;
                        break;
                    case FormatEnum.Digital:
                    case FormatEnum.Boolean:
                        options.IsOutOfRange = false;
                        object parsevalue;
                        if (TryParseTextValue(_sval, out parsevalue))
                            ret = parsevalue.ToString();
                        else
                            ret = _sval;
                        break;
                    case FormatEnum.Enumerated:
                        options.IsOutOfRange = false;
                        object parseevalue;
                        if (TryParseTextValue(_sval, out parseevalue))
                            ret = parseevalue.ToString();
                        else
                            ret = _sval;
                        break;
                }
            }
            catch(Exception ex)
            {
                ret = _sval;
            }

            options.TextValue = ret;

            return ret;
        }

        bool SetValue(String newValue)
        {
            if (auditTraceViewModel != null && auditTraceViewModel.IsAuditTraceEnabled)
            {
                if (!auditTraceViewModel.SetValue(newValue))
                {
                    OnValueChanged(Value);
                    return false;
                }
            }
            else
                Value = newValue;

            return true;
        }

        private void InitBack()
        {
            if (container == null)
                return;
            if (BackBorder == null)
                BackBorder = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    BorderThickness = new Thickness(0),
                    CornerRadius = CornerRadius,
                    Background = Background,
                };

            if (!container.Children.Contains(BackBorder))
                container.Children.Add(BackBorder);
        }

        private void InitPrecision()
        {
            if(String.IsNullOrEmpty(FormatString))
            {
                StringBuilder userformat = new StringBuilder();
                StringBuilder invariantformat = new StringBuilder();
                string uformat = string.Empty;
                string iformat = string.Empty;
                NumberFormatInfo currentFormat = CultureInfo.CurrentCulture.NumberFormat;
                NumberFormatInfo invariantFormat = CultureInfo.InvariantCulture.NumberFormat;
                if (PrecisionDigits > 0)
                {
                    userformat.Append(currentFormat.NumberDecimalSeparator);
                    invariantformat.Append(invariantFormat.NumberDecimalSeparator);
                    for (int i = 0; i < PrecisionDigits; i++)
                    {
                        userformat.Append("0");
                        invariantformat.Append("0");
                    }
                }
                if (ShowDigitGroupingSymbol)
                {
                    uformat = $"0{currentFormat.NumberGroupSeparator}000{userformat.ToString()}";
                    iformat = $"#{invariantFormat.NumberGroupSeparator}#0{invariantformat.ToString()}";
                }
                else
                {
                    uformat = $"0{userformat.ToString()}";
                    iformat = $"0{invariantformat.ToString()}";
                }

                DisplayStringFormat = $"#{uformat}";
                format = $"{iformat}";
            }
            else
            {
                DisplayStringFormat = $"{FormatString}";
                format = $"{FormatString}";
            }
        }

        private void UpdateText(string _sval)
        {
            if (StringDisplay != null)
                StringDisplay.Text = _sval;
            else if (PasswordBoxDisplay != null)
                PasswordBoxDisplay.Password = _sval;
        }

        private void UpdateEditDisplayLayout()
        {
            if (bDispose)
                return;

            InitBack();
            InitPrecision();
            UpdateBackBorder();
            //UpdateBackStyle();
            //UpdateCrystalStyle();
            UpdateStringdisplay();
            UpdatePasswordDisplay();

            InitDisplay();
            UpdateSpinView();
            UpdateMeasUnit();
            if (RunningOnServer)
            {
                var elements = (this as UIElement).GetVisualChildrenOfType<FrameworkElement>().ToList();
                elements.ForEach(x => x.IsHitTestVisible = false);
                IsHitTestVisible = IsHitTestVisible && !IsReadOnly && !isStat && !isTemporaryDisabled;
            }

            //OnValueChanged(null, null);
        }
        private void UpdateOutOfRange(bool isOutOfRange)
        {
            if (container == null)
                return;
            if (bDesign || !isOutOfRange || !OutOfRange)
            {
                if (OutOfRangeMarker != null)
                {
                    if (container.Children.Contains(OutOfRangeMarker))
                        container.Children.Remove(OutOfRangeMarker);
                    OutOfRangeMarker = null;
                }
            }
            else if (OutOfRange && isOutOfRange)
            {
                if (OutOfRangeMarker == null)
                {
                    var spwh = this.ActualHeight / 8;
                    if (spwh < 5)
                        spwh = 5;
                    OutOfRangeMarker = new Ellipse()
                    {
                        Width = spwh,
                        Height = spwh,
                        Fill = new SolidColorBrush(Colors.Orange),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    if (!container.Children.Contains(OutOfRangeMarker))
                        container.Children.Add(OutOfRangeMarker);

                    Grid.SetColumn(OutOfRangeMarker, 1);
                    Grid.SetZIndex(OutOfRangeMarker, 20);
                }
                OutOfRangeMarker.Fill = ForegroundError;
            }
        }

        private void UpdateSpinView()
        {
            if (container == null)
                return;
            if (SpinEnabled && !RunningOnServer && (DisplayType != FormatEnum.String || bDesign))
            {
                if (SpinView == null)
                {
                    InitSpin();

                    ManageSpinHandler(false);

                    if (!container.Children.Contains(SpinView))
                        container.Children.Add(SpinView);
                }

                UpdateSpinPosition();
                SpinView.IsEnabled = !IsReadOnly && !isStat && !isTemporaryDisabled;

                if (BackBorder != null)
                {
                    if (SpinPosition == SpinRelativePosition.Left)
                    {
                        Grid.SetColumn(BackBorder, 1);
                        Grid.SetColumnSpan(BackBorder, 3);
                    }
                    else
                    {
                        Grid.SetColumn(BackBorder, 0);
                        Grid.SetColumnSpan(BackBorder, 3);
                    }
                }

                UpdateSpinButtonBackColor();
            }
            else if (SpinView != null)
            {
                ManageSpinHandler(true);
                SpinView.Child = null;
                if (container.Children.Contains(SpinView))
                    container.Children.Remove(SpinView);
                ResetSpinPosition();
                SpinView = null;

                if(BackBorder!=null)
                {
                    Grid.SetColumn(BackBorder, 0);
                    Grid.SetColumnSpan(BackBorder, 4);
                }
            }
        }

        void UpdateSpinButtonBackColor()
        {
            if (dpdupdatelayout == null || dpdupdatelayout.Status == DispatcherOperationStatus.Completed
              || dpdupdatelayout.Status == DispatcherOperationStatus.Aborted)

                dpdupdatelayout = Dispatcher.BeginInvokeAsynchronouslyInBackground(this, () =>

                {
                    if (SpinEnabled && !RunningOnServer && (DisplayType != FormatEnum.String || bDesign))
                    {
                        Brush fill = this.GetUnsetPropertyValue<Brush>(SpinBackColorProperty, SpinBackColor, Foreground, false);
                        if (pathDown != null)
                            pathDown.Fill = fill;
                        if (pathUp != null)
                            pathUp.Fill = fill;

                        if (this.ReadLocalValue(SpinButtonBackColorProperty) != DependencyProperty.UnsetValue)
                        {
                            if (spinUp != null)
                                spinUp.GetVisualChildrenOfType<Control>().ToList().ForEach(c => c.Background = SpinButtonBackColor);

                            if (spinDown != null)
                                spinDown.GetVisualChildrenOfType<Control>().ToList().ForEach(c => c.Background = SpinButtonBackColor);
                        }
                    }
                });
        }

        private void ManageSpinHandler(bool bDetach)
        {
            UIElement ue = SpinView as UIElement;
            if (ue == null)
                return;
            if (bDetach)
            {
                (from c in ue.GetVisualChildrenOfType<RepeatButton>()
                 select c).ToList().ForEach(child =>
                 {
                     if (child.Name == "Up")
                         child.Click -= Up_Click_1;
                     else if (child.Name == "Down")
                         child.Click -= Down_Click_1;
                 });
            }
            else
            {
                (from c in ue.GetVisualChildrenOfType<RepeatButton>()
                 select c).ToList().ForEach(child =>
                 {
                     if (child.Name == "Up")
                         child.Click += Up_Click_1;
                     else if (child.Name == "Down")
                         child.Click += Down_Click_1;
                 });
            }
        }

        private void InitSpin()
        {
            SpinView = new Viewbox()
            {
                //Width = 20,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.Fill
            };
            DockPanel doc = new DockPanel()
            {
                LastChildFill = true
            };

            pathUp = TryFindResource("up") as Path;
            if (pathUp != null)
                pathUp.Fill = this.GetUnsetPropertyValue<Brush>(SpinBackColorProperty, SpinBackColor, Foreground, false);
            labelUp = new Label()
            {
                Content = pathUp,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            spinUp = new RepeatButton()
            {
                Name = "Up",
                Content = labelUp,
                MinWidth = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = this.GetUnsetPropertyValue<Brush>(SpinButtonBackColorProperty, Background, Background, false)
            };

            //up.Style = TryFindResource("UpArrow") as Style,
            DockPanel.SetDock(spinUp, Dock.Top);
            pathDown = TryFindResource("down") as Path;
            if (pathDown != null)
                pathDown.Fill = this.GetUnsetPropertyValue<Brush>(SpinBackColorProperty, SpinBackColor, Foreground, false);
            labelDown = new Label()
            {
                Content = pathDown,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            spinDown = new RepeatButton()
            {
                Name = "Down",
                Content = labelDown,
                MinWidth = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = this.GetUnsetPropertyValue<Brush>(SpinButtonBackColorProperty, Background, Background, false)
            };

            //down.Style = TryFindResource("DownArrow") as Style,
            DockPanel.SetDock(spinDown, Dock.Bottom);

            doc.Children.Add(spinUp);
            doc.Children.Add(spinDown);

            SpinView.Child = doc;
            Grid.SetColumn(SpinView, 3);
        }

        //private void UpdateCrystalStyle()
        //{
        //    if (CrystalStyle)
        //    {
        //        if (CrystalContent == null)
        //        {
        //            CrystalContent = new ContentControl();
        //            if (!container.Children.Contains(CrystalContent))
        //                container.Children.Add(CrystalContent);

        //            Grid.SetColumn(BackContent, 1);
        //            Grid.SetZIndex(CrystalContent, 2);
        //        }

        //        CrystalContent.Content = TryFindResource(string.Format("Crystal{0}", DisplayBaseModel.ToString()));
        //    }
        //    else if (CrystalContent != null)
        //    {
        //        CrystalContent.Content = null;
        //        if (container.Children.Contains(CrystalContent))
        //            container.Children.Remove(CrystalContent);
        //        CrystalContent = null;
        //    }
        //}

        //private void UpdateBackStyle()
        //{
        //    if(EnableBackgroundLayer)
        //    {
        //        if (BackContent == null)
        //        {
        //            BackContent = new ContentControl();
        //            if (!container.Children.Contains(BackContent))
        //                container.Children.Add(BackContent);

        //            Grid.SetColumn(BackContent, 1);
        //            Grid.SetZIndex(BackContent, 1);
        //        }

        //        FrameworkElement ue = (FrameworkElement)TryFindResource(string.Format("{0}", DisplayBaseModel.ToString()));

        //        (from c in ue.GetVisualChildrenOfType<Path>()
        //         where (c.Tag as String) == Properties.Resources.TagBackground
        //         select c).ToList().ForEach(child =>
        //         {
        //             child.Fill = Background;
        //         });

        //        BackContent.Content = ue;

        //    }
        //    else if (BackContent != null)
        //    {
        //        BackContent.Content = null;
        //        if (container.Children.Contains(BackContent))
        //            container.Children.Remove(BackContent);
        //        BackContent = null;
        //    }
        //}

        private void UpdateBackBorder()
        {
            if (BackBorder == null)
                return;
            BackBorder.Background = Background;
            BackBorder.BorderBrush = DisplayBorderBrush;
            BackBorder.BorderThickness = new Thickness(DisplayBorderThickness);
            if (SpinEnabled && !RunningOnServer && (DisplayType != FormatEnum.String || bDesign))
            {
                if (SpinPosition == SpinRelativePosition.Left)
                {
                    Grid.SetColumn(BackBorder, 1);
                    Grid.SetColumnSpan(BackBorder, 3);
                }
                else
                {
                    Grid.SetColumn(BackBorder, 0);
                    Grid.SetColumnSpan(BackBorder, 3);
                }
            }
            else
            {
                Grid.SetColumn(BackBorder, 0);
                Grid.SetColumnSpan(BackBorder, 4);
            }
        }

        IDictionary<string, string> stringlist = null;
        IStringEditorManager stringManager;
        private void UpdateMeasUnit()
        {
            if (container == null)
                return;
            string meas = ConverterLabel ?? MeasUnit;
            if (!string.IsNullOrEmpty(meas))
            {
                meas = TranslationHelpers.TranslationHelper.TranslateComposedText(meas, stringlist, meas);

                if (MeasUnitTextBox == null)
                {
                    MeasUnitTextBox = new TextBlock()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Right

                    };

                    if (!container.Children.Contains(MeasUnitTextBox))
                        container.Children.Add(MeasUnitTextBox);

                    Grid.SetColumn(MeasUnitTextBox, 2);
                    Grid.SetZIndex(MeasUnitTextBox, 3);
                }

                MeasUnitTextBox.Foreground = this.Foreground;
                MeasUnitTextBox.Text = meas;
                MeasUnitTextBox.FontFamily = MeasFontSettings.FontFamily;
                MeasUnitTextBox.FontWeight = MeasFontSettings.FontWeight;
                MeasUnitTextBox.FontStyle = MeasFontSettings.FontStyle;
                MeasUnitTextBox.FontSize = MeasFontSettings.FontSize;
                MeasUnitTextBox.VerticalAlignment = LabelVerticalAlignement; 
                InitMeasAlignement(LabelHorizontalAlignement);

                MeasUnitTextBox.Margin = MeasUnitOffset;
            }
            else if (MeasUnitTextBox != null)
            {
                if (container.Children.Contains(MeasUnitTextBox))
                    container.Children.Remove(MeasUnitTextBox);
                MeasUnitTextBox = null;
            }

        }

        private void UpdatePasswordDisplay()
        {
            if (container == null)
                return;
            if (!bDesign && PasswordStyle)
            {
                if (PasswordBoxDisplay == null)
                {
                    PasswordBoxDisplay = new PasswordBox()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        //PasswordChar = '*',
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        FlowDirection = FlowDirection.LeftToRight,
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 20,
                        FontWeight = FontWeights.SemiBold,
                        FontStyle = FontStyles.Normal
                    };

                    if (!container.Children.Contains(PasswordBoxDisplay))
                        container.Children.Add(PasswordBoxDisplay);

                    Grid.SetColumn(PasswordBoxDisplay, 1);
                    Grid.SetZIndex(PasswordBoxDisplay, 4);

                    //OnValueChanged(null, Value);
                }
                PasswordBoxDisplay.Foreground = this.Foreground;
                PasswordBoxDisplay.CaretBrush = this.Foreground;
                PasswordBoxDisplay.HorizontalContentAlignment = InitContentAlignement(LabelHorizontalAlignement);
                PasswordBoxDisplay.VerticalContentAlignment = (VerticalAlignment)LabelVerticalAlignement;
                PasswordBoxDisplay.Margin = LabelOffset;
                PasswordBoxDisplay.IsHitTestVisible = !IsReadOnly && !isStat && !isTemporaryDisabled;
                //PasswordBoxDisplay.IsEnabled = !IsReadOnly && !isStat && !isTemporaryDisabled;
                UpdateFont(PasswordBoxDisplay, ValueFontSettings);
            }
            else
            {
                if (PasswordBoxDisplay != null)
                {
                    if (!container.Children.Contains(PasswordBoxDisplay))
                        container.Children.Add(PasswordBoxDisplay);
                    PasswordBoxDisplay = null;
                }
            }
        }

        private void UpdateStringdisplay()
        {
            if (container == null)
                return;
            if (bDesign || (!bDesign && !PasswordStyle))
            {
                if (StringDisplay == null)
                {
                    StringDisplay = new TextBox()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0),
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    if (!container.Children.Contains(StringDisplay))
                        container.Children.Add(StringDisplay);

                    Grid.SetColumn(StringDisplay, 1);
                    Grid.SetZIndex(StringDisplay, 4);

                    //if (!bDesign)
                    //    OnValueChanged(null, Value);
                }

                if (bDesign)
                {
                    if (PasswordStyle)
                        StringDisplay.Text = "●●●●";
                    else
                        StringDisplay.Text = DisplayStringFormat;

                    StringDisplay.IsHitTestVisible = false;
                }
                else
                {
                    StringDisplay.IsHitTestVisible = !IsReadOnly && !isStat && !isTemporaryDisabled;
                    //StringDisplay.IsEnabled = !IsReadOnly && !isStat && !isTemporaryDisabled;
                }

                StringDisplay.Foreground = this.Foreground;
                StringDisplay.CaretBrush = this.Foreground;
                StringDisplay.TextAlignment = LabelHorizontalAlignement;
                StringDisplay.VerticalContentAlignment = LabelVerticalAlignement;
                StringDisplay.Margin = LabelOffset;
                StringDisplay.TextWrapping = TextWrapping;
                UpdateFont(StringDisplay, ValueFontSettings);
            }
            else
            {
                if (StringDisplay != null)
                {
                    if (!container.Children.Contains(StringDisplay))
                        container.Children.Add(StringDisplay);
                    StringDisplay = null;
                }
            }
        }

        private HorizontalAlignment InitContentAlignement(System.Windows.TextAlignment newValue)
        {
            HorizontalAlignment res = HorizontalAlignment.Center;

            switch (newValue)
            {
                case TextAlignment.Left:
                    res = HorizontalAlignment.Left;
                    break;
                case TextAlignment.Right:
                    res = HorizontalAlignment.Right;
                    break;
                case TextAlignment.Justify:
                    res = HorizontalAlignment.Stretch;
                    break;
                case TextAlignment.Center:
                    res = HorizontalAlignment.Center;
                    break;
            }
            return res;
        }
        private void InitMeasAlignement(System.Windows.TextAlignment newValue)
        {
            MeasUnitTextBox.TextAlignment = TextAlignment.Left;
            return;

            switch (newValue)
            {
                case TextAlignment.Left:
                    MeasUnitTextBox.TextAlignment = TextAlignment.Right;
                    break;
                case TextAlignment.Right:
                    MeasUnitTextBox.TextAlignment = TextAlignment.Left;
                    break;
                case TextAlignment.Justify:
                    MeasUnitTextBox.TextAlignment = TextAlignment.Right;
                    break;
                case TextAlignment.Center:
                    MeasUnitTextBox.TextAlignment = TextAlignment.Right;
                    break;
            }
        }
        private void UpdateSpinPosition()
        {
            if (container == null)
                return;
            if (SpinPosition == SpinRelativePosition.Left)
            {
                Grid.SetColumn(SpinView, 0);
                var leftColumn = container.ColumnDefinitions.FirstOrDefault();
                var valueColumn = container.ColumnDefinitions[1];
                switch (SpinDimension)
                {
                    case SpinRelativeDimension.Small:
                        leftColumn.Width = new GridLength(Properties.Settings.Default.SmallDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.SmallDimension, GridUnitType.Star);
                        break;
                    case SpinRelativeDimension.Medium:
                        leftColumn.Width = new GridLength(Properties.Settings.Default.MediumDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.MediumDimension, GridUnitType.Star);
                        break;
                    case SpinRelativeDimension.Large:
                        leftColumn.Width = new GridLength(Properties.Settings.Default.LargeDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.LargeDimension, GridUnitType.Star);
                        break;
                };
            }
            else
            {
                var rightColumn = container.ColumnDefinitions.LastOrDefault();
                var valueColumn = container.ColumnDefinitions[1];
                Grid.SetColumn(SpinView, 3);
                switch (SpinDimension)
                {
                    case SpinRelativeDimension.Small:
                        rightColumn.Width = new GridLength(Properties.Settings.Default.SmallDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.SmallDimension, GridUnitType.Star);
                        break;
                    case SpinRelativeDimension.Medium:
                        rightColumn.Width = new GridLength(Properties.Settings.Default.MediumDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.MediumDimension, GridUnitType.Star);
                        break;
                    case SpinRelativeDimension.Large:
                        rightColumn.Width = new GridLength(Properties.Settings.Default.LargeDimension, GridUnitType.Star);
                        valueColumn.Width = new GridLength(100d - Properties.Settings.Default.LargeDimension, GridUnitType.Star);
                        break;
                };
            }
        }
        void ResetSpinPosition()
        {
            if (container == null)
                return;
            var rightColumn = container.ColumnDefinitions.LastOrDefault();
            var leftColumn = container.ColumnDefinitions.FirstOrDefault();
            var valueColumn = container.ColumnDefinitions[1];
            rightColumn.Width = new GridLength(0d, GridUnitType.Star);
            leftColumn.Width = new GridLength(0d, GridUnitType.Star);
            valueColumn.Width = new GridLength(100d, GridUnitType.Star);
        }

        void UpdateFont(Control control, FontSettings newValue)
        {
            control.FontFamily = newValue.FontFamily;
            control.FontWeight = newValue.FontWeight;
            control.FontStyle = newValue.FontStyle;
            control.FontSize = newValue.FontSize;
            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;

        }

        private void ManageEventsHandler()
        {
            DetachEvents();
            AddEventHandler(PasswordBoxDisplay);
            AddEventHandler(StringDisplay);
        }

        private void AddEventHandler(Object control)
        {
            if (control != null)
            {
                CommandBinding pasteBinding = new CommandBinding(ApplicationCommands.Paste);
                pasteBinding.Executed += (s, e) =>
                {
                    if (Clipboard.ContainsText())
                    {
                        if (control is TextBox stringDisplay) stringDisplay.Paste();

                        if (control is PasswordBox passwordDisplay) passwordDisplay.Paste();
                    }
                    ManageUpdateBinding(control, null);
                };

                (control as FrameworkElement).TouchDown += OnTouchDown;
                (control as FrameworkElement).PreviewMouseDown += OnPreviewMouseDown;
                (control as FrameworkElement).PreviewStylusDown += OnPreviewStylusDown;
                (control as FrameworkElement).LostFocus += OnLostFocus;
                (control as FrameworkElement).PreviewKeyDown += OnPreviewKeyDown;
                (control as FrameworkElement).PreviewLostKeyboardFocus += OnLostKeyboardFocus;
                (control as FrameworkElement).CommandBindings.Add(pasteBinding);

            }
        }
        bool IsInEditMode;
        private void OnPreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            e.Handled = true;
            if (IsReadOnly || isTemporaryDisabled || isStat)
            {
                return;
            }
            IsInEditMode = true;
            TIKeyboardHelper.Show();
        }
        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            if (IsReadOnly || isTemporaryDisabled || isStat)
            {
                return;
            }

            IsInEditMode = true;
            TIKeyboardHelper.Show();
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            if (IsReadOnly || isTemporaryDisabled || isStat)
            {
                e.Handled = true;
                return;
            }

            IsInEditMode = true;
        }

        private void RemoveEventHandler(Object control)
        {
            if (control != null)
            {
                (control as FrameworkElement).TouchDown -= OnTouchDown;
                (control as FrameworkElement).PreviewMouseDown -= OnPreviewMouseDown;
                (control as FrameworkElement).PreviewStylusDown -= OnPreviewStylusDown;
                (control as FrameworkElement).LostFocus -= OnLostFocus;
                (control as FrameworkElement).PreviewKeyDown -= OnPreviewKeyDown;
                (control as FrameworkElement).PreviewLostKeyboardFocus -= OnLostKeyboardFocus;
                (control as FrameworkElement).CommandBindings.Clear();
            }
        }

        bool updateSource;
        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!updateSource || IsReadOnly || isTemporaryDisabled || isStat)
            {
                IsInEditMode = false;
                return;
            }

            ManageUpdateBinding(sender, null);
            IsInEditMode = false;
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            TIKeyboardHelper.Hide();

            if (!updateSource || IsReadOnly || isTemporaryDisabled || isStat)
            {
                IsInEditMode = false;
                OnValueChanged(Value);
                return;
            }

            ManageUpdateBinding(sender, null);
            IsInEditMode = false;
        }

        DispatcherOperation d1;
        object lockValue = new object();
        class ValueOptions
        {
            public bool IsOutOfRange { get; set; }
            public bool ForceBoolean { get; set; }
            public FormatEnum DisplayType { get; set; }
            public decimal MaxValue { get; set; }
            public decimal MinValue { get; set; }
            public int PrecisionDigits { get; set; }
            public CultureInfo Culture { get; set; }
            public string Format { get; set; }
            public string Value { get; set; }
            public string TextValue { get; set; }
            public StatProps StatDef { get; set; }
            public string TimespanFormat { get; set; }

            public ValueOptions()
            {
                IsOutOfRange = false;
                ForceBoolean = false;
                TextValue = string.Empty;
            }
            public ValueOptions(ValueOptions options)
            {
                IsOutOfRange = options.IsOutOfRange;
                ForceBoolean = options.ForceBoolean;
                DisplayType = options.DisplayType;
                MaxValue = options.MaxValue;
                MinValue = options.MinValue;
                PrecisionDigits = options.PrecisionDigits;
                Culture = options.Culture;
                Format = options.Format;
                Value = options.Value;
                TextValue = string.Empty;
                StatDef = options.StatDef;
                TimespanFormat = options.TimespanFormat;
            }
        }

        ValueOptions lastOptions;
        CancellationTokenSource cts;
        private void OnValueChanged(string value)
        {
            if (bDispose || IsInEditMode || syncContextScheduler == null)
                return;

            bool bForceDispatcherOperation = false;
            lock (lockValue)
            {
                bForceDispatcherOperation = lastOptions == null;

                lastOptions = new ValueOptions();
                lastOptions.Value = value;
                lastOptions.DisplayType = isStat ? FormatEnum.Numeric : DisplayType;
                lastOptions.MaxValue = MaxValue;
                lastOptions.MinValue = MinValue;
                lastOptions.PrecisionDigits = PrecisionDigits;
                lastOptions.Culture = CultureInfo.CurrentCulture;
                lastOptions.Format = format;
                lastOptions.StatDef = StatDef;
                lastOptions.TimespanFormat = TimespanFormat;
            }

            //var ret = GetValue(value, ref isOutOfRange, ref forceBoolean, _format, culture, precisionDigits, maxValue, minValue, _displayType, enumStrings, trueState, falseState);

            if (d1 == null || d1.Status == DispatcherOperationStatus.Completed || bForceDispatcherOperation ||
                d1.Status == DispatcherOperationStatus.Aborted)
            {
                d1 = Dispatcher.BeginInvokeAsynchronouslyInRender(this, () =>
                {
                    if (bDispose)
                        return;

                    if (cts == null)
                        cts = new CancellationTokenSource();
                    var token = cts.Token;
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        token.ThrowIfCancellationRequested();

                        ValueOptions lOptions;
                        lock (lockValue)
                        {
                            lOptions = lastOptions;
                            lastOptions = null;
                        }
                        //#if DEBUG
                        //                var text = String.Format("Getting values took") + " : {0}";
                        //                using (var stopwatcher = new StopWatcher(text))
                        //#endif
                        {
                            var ret = GetValue(lOptions);
                            return lOptions;
                        }
                    }, token);
                    var task2 = task1.ContinueWith(ret =>
                    {
                        if (ret.IsFaulted)
                            return;

                        ValueOptions options = (ret.Result as ValueOptions);
                        if (options == null)
                            return;
                        //#if DEBUG
                        //    var text = String.Format("Updating values took") + " : {0}";
                        //                using (var stopwatcher = new StopWatcher(text))
                        //#endif
                        {
                            if (string.IsNullOrEmpty(MeasUnit) && UseEUnit)
                            {
                                MeasUnit = MonitoredUnit;
                                if (bInit)
                                    UpdateMeasUnit();
                            }

                            UpdateText(options.TextValue);

                            if (options.ForceBoolean)
                                DisplayType = FormatEnum.Boolean;
                            else
                                UpdateOutOfRange(options.IsOutOfRange);
                        }
                    }, token, TaskContinuationOptions.None, syncContextScheduler);
                });
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsReadOnly || isTemporaryDisabled || isStat)
            {
                e.Handled = true;
                updateSource = false;
                IsInEditMode = false;
                return;
            }

            if (e.Key != Key.Enter || e.Key != Key.Return)
            {

                e.Handled = false;
                updateSource = true;
                return;
            }


            e.Handled = true;
            if (!updateSource)
                return;

            ManageUpdateBinding(sender, e);
            IsInEditMode = false;
        }

        bool updatingBinding;
        void ManageUpdateBinding(object sender, KeyEventArgs e, string svalue = null)
        {
            if (updatingBinding)
                return;

            try
            {
                updatingBinding = true;
                switch (DisplayType)
                {
                    case FormatEnum.Integer:
                        if (e == null || e.Key == Key.Enter)
                        {
                            decimal _value;
                            double v;
                            string _svval = Value;
                            CultureInfo culture = CultureInfo.CurrentCulture;

                            if (Value.IndexOf(':') >= 0)
                            {
                                _svval = Value.Substring(Value.IndexOf(':') + 1);
                                culture = new CultureInfo(Value.Substring(0, Value.IndexOf(':')));
                            }

                            string _sval = string.Empty;
                            if (RunningOnServer)
                                _sval = svalue;
                            else if (sender is PasswordBox)
                                _sval = (sender as PasswordBox).Password;
                            else if (sender is TextBox)
                                _sval = (sender as TextBox).Text;

                            try
                            {
                                if (CheckIsNumeric(_sval))
                                {
                                    _value = System.Convert.ToDecimal(_sval, CultureInfo.CurrentCulture) * (Decimal)Math.Pow(10, PrecisionDigits);
                                    if (_value > MaxValue || _value < MinValue)
                                    {
                                        try
                                        {
                                            Decimal val = System.Convert.ToDecimal(_svval, culture);
                                            UpdateText((val / (Decimal)Math.Pow(10, PrecisionDigits)).ToString(format, CultureInfo.CurrentCulture));
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else
                                    {
                                        if (SetValue(_value.ToString(CultureInfo.InvariantCulture)))
                                            UpdateText((_value / (Decimal)Math.Pow(10, PrecisionDigits)).ToString(format, CultureInfo.CurrentCulture));
                                    }
                                }
                                else
                                {
                                    Decimal val = System.Convert.ToDecimal(_svval, culture);
                                    UpdateText((val / (Decimal)Math.Pow(10, PrecisionDigits)).ToString(format, CultureInfo.CurrentCulture));
                                }
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    Decimal val = System.Convert.ToDecimal(_svval, culture);
                                    UpdateText((val / (Decimal)Math.Pow(10, PrecisionDigits)).ToString(format, CultureInfo.CurrentCulture));
                                }
                                catch (Exception)
                                {
                                    UpdateText(_svval);
                                }
                            }

                            IsInEditMode = false;
                            if (RunningOnServer)
                                OnValueChanged(Value);
                            else if (e != null && e.Key == Key.Enter)
                            {
                                OnValueChanged(Value);
                                if (sender is PasswordBox)
                                    (sender as PasswordBox).SelectAll();
                                else if (sender is TextBox)
                                    (sender as TextBox).SelectAll();
                            }

                            updateSource = false;
                        }
                        break;
                    case FormatEnum.Numeric:
                        if (e == null || e.Key == Key.Enter)
                        {
                            decimal _value;
                            string _svval = Value;
                            CultureInfo culture = CultureInfo.CurrentCulture;

                            if (Value.IndexOf(':') >= 0)
                            {
                                _svval = Value.Substring(Value.IndexOf(':') + 1);
                                culture = new CultureInfo(Value.Substring(0, Value.IndexOf(':')));
                            }

                            string _sval = string.Empty;
                            if (RunningOnServer)
                                _sval = svalue;
                            else if (sender is PasswordBox)
                                _sval = (sender as PasswordBox).Password;
                            else if (sender is TextBox)
                                _sval = (sender as TextBox).Text;

                            try
                            {
                                //_value = (sender as NumericTextBox).Value;
                                if (CheckIsNumeric(_sval))
                                {
                                    _value = System.Convert.ToDecimal(_sval, CultureInfo.CurrentCulture);
                                    if (_value > MaxValue || _value < MinValue)
                                    {
                                        try
                                        {
                                            Decimal val = System.Convert.ToDecimal(_svval, culture);
                                            UpdateText(val.ToString(format, CultureInfo.CurrentCulture));
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    else
                                    {
                                        if (SetValue(_value.ToString(CultureInfo.InvariantCulture)))
                                            UpdateText(_value.ToString(format, CultureInfo.CurrentCulture));
                                    }
                                }
                                else
                                {
                                    UpdateText(_svval);
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateText(_svval);
                            }

                            IsInEditMode = false;
                            if (RunningOnServer)
                                OnValueChanged(Value);
                            else if (e != null && e.Key == Key.Enter)
                            {
                                OnValueChanged(Value);
                                if (sender is PasswordBox)
                                    (sender as PasswordBox).SelectAll();
                                else if (sender is TextBox)
                                    (sender as TextBox).SelectAll();
                            }

                            updateSource = false;
                        }
                        break;
                    case FormatEnum.Digital:
                    case FormatEnum.Boolean:
                        if (e == null || e.Key == Key.Enter)
                        {
                            Boolean _bvalue;
                            object parsevalue;
                            string _sval = Value;
                            CultureInfo culture = CultureInfo.CurrentCulture;

                            if (Value.IndexOf(':') >= 0)
                            {
                                _sval = Value.Substring(Value.IndexOf(':') + 1);
                                culture = new CultureInfo(Value.Substring(0, Value.IndexOf(':')));
                            }

                            string _svalue = string.Empty;
                            if (RunningOnServer)
                                _svalue = svalue;
                            else if (sender is PasswordBox)
                                _svalue = (sender as PasswordBox).Password;
                            else if (sender is TextBox)
                                _svalue = (sender as TextBox).Text;

                            bool bResult = false;
                            if (_svalue.Equals("1"))
                            {
                                bResult = SetValue(Convert.ToString(true));
                            }
                            else if (_svalue.Equals("0"))
                            {
                                bResult = SetValue(Convert.ToString(false));
                            }
                            else if (TryParseValue(_svalue, out parsevalue))
                            {
                                bResult = SetValue(Convert.ToString(parsevalue, CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                try
                                {
                                    _bvalue = System.Convert.ToBoolean(_svalue);
                                    bResult = SetValue(Convert.ToString(_bvalue));
                                }
                                catch (Exception ex)
                                {
                                    if (TryParseTextValue(_sval, out parsevalue))
                                        UpdateText(parsevalue.ToString());
                                    else
                                        UpdateText(_sval);
                                }
                            }

                            if (bResult)
                            {
                                _sval = Value;
                                if (Value.IndexOf(':') >= 0)
                                    _sval = Value.Substring(Value.IndexOf(':') + 1);
                                if (TryParseTextValue(_sval, out parsevalue))
                                    UpdateText(parsevalue.ToString());
                                else
                                    UpdateText(_sval);
                            }


                            IsInEditMode = false;
                            if (RunningOnServer)
                                OnValueChanged(Value);
                            else if (e != null && e.Key == Key.Enter)
                            {
                                OnValueChanged(Value);
                                if (sender is PasswordBox)
                                    (sender as PasswordBox).SelectAll();
                                else if (sender is TextBox)
                                    (sender as TextBox).SelectAll();
                            }

                            updateSource = false;
                        }
                        break;
                    //case FormatEnum.Digital:
                    case FormatEnum.Enumerated:
                        if (e == null || e.Key == Key.Enter)
                        {
                            int _enval;
                            object parsevalue;
                            string _svval = Value;
                            if (Value.IndexOf(':') >= 0)
                                _svval = Value.Substring(Value.IndexOf(':') + 1);

                            string _svalue = string.Empty;
                            if (RunningOnServer)
                                _svalue = svalue;
                            else if (sender is PasswordBox)
                                _svalue = (sender as PasswordBox).Password;
                            else if (sender is TextBox)
                                _svalue = (sender as TextBox).Text;

                            bool bResult = false;
                            if (!TryParseValue(_svalue, out parsevalue))
                            {
                                if (int.TryParse(_svalue, out _enval) && _enval < EnumStringCount && _enval >= 0)
                                {
                                    bResult = SetValue(Convert.ToString(_enval));
                                }
                                else
                                {
                                    if (TryParseTextValue(_svval, out parsevalue))
                                        UpdateText(parsevalue.ToString());
                                    else
                                        UpdateText(_svval);
                                }
                            }
                            else
                            {
                                bResult = SetValue(Convert.ToString(parsevalue));
                            }

                            if (bResult)
                            {
                                _svval = Value;
                                if (Value.IndexOf(':') >= 0)
                                    _svval = Value.Substring(Value.IndexOf(':') + 1);
                                if (TryParseTextValue(_svval, out parsevalue))
                                    UpdateText(parsevalue.ToString());
                                else
                                    UpdateText(_svval);
                            }

                            IsInEditMode = false;
                            if (RunningOnServer)
                                OnValueChanged(Value);
                            else if (e != null && e.Key == Key.Enter)
                            {
                                OnValueChanged(Value);
                                if (sender is PasswordBox)
                                    (sender as PasswordBox).SelectAll();
                                else if (sender is TextBox)
                                    (sender as TextBox).SelectAll();
                            }

                            updateSource = false;
                        }
                        break;
                    case FormatEnum.String:
                        if (e == null || (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control))
                        {
                            string _svval = Value;
                            if (Value.IndexOf(':') >= 0)
                                _svval = Value.Substring(Value.IndexOf(':') + 1);


                            string _svalue = string.Empty;
                            if (RunningOnServer)
                                _svalue = svalue;
                            else if (sender is PasswordBox)
                                _svalue = (sender as PasswordBox).Password;
                            else if (sender is TextBox)
                                _svalue = (sender as TextBox).Text;

                            if (_svalue.Length > MaxValue || _svalue.Length < MinValue)
                                UpdateText(_svval);
                            else
                            {
                                bool bResult = SetValue(_svalue);
                                if (bResult && RunningOnServer)
                                    UpdateText(_svalue);
                            }

                            IsInEditMode = false;
                            if(RunningOnServer)
                                OnValueChanged(Value);
                            else if (e != null && e.Key == Key.Enter)
                            {
                                OnValueChanged(Value);
                                if (sender is PasswordBox)
                                    (sender as PasswordBox).SelectAll();
                                else if (sender is TextBox)
                                    (sender as TextBox).SelectAll();
                            }

                            updateSource = false;
                        }
                        else if (e != null && (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                        {
                            if (sender is TextBox)
                            {
                                var obj = sender as TextBox;
                                obj.AppendText("\r");
                                obj.CaretIndex = obj.Text.Length;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                OnValueChanged(null);
            }
            finally
            {
                updatingBinding = false;
            }
        }

        private bool CheckIsNumeric(string _sval)
        {
            try
            {
                Decimal parseobject;
                return Decimal.TryParse(_sval, out parseobject);
            }
            catch
            {
                return false;
            }
        }
        private void InitDisplay()
        {
            switch (DisplayType)
            {
                case FormatEnum.Integer:
                case FormatEnum.Numeric:
                case FormatEnum.String:
                    break;
                case FormatEnum.Boolean:
                case FormatEnum.Digital:
                    MinValue = 0;
                    MaxValue = 1;
                    UseEUnit = false;
                    break;
                case FormatEnum.Enumerated:
                    UseEUnit = false;
                    break;
                default:
                    break;
            }
        }

        void DetachEvents()
        {
            RemoveEventHandler(PasswordBoxDisplay);
            RemoveEventHandler(StringDisplay);
            //RemoveEventHandler(doubledisplay); 
        }

        private bool TryParseValue(String value, out Object result)
        {
            result = null;
            var nodeIdViewModel = monitoredItemViewModel?.NodeIdModel;
            if (nodeIdViewModel != null)
            {
                try
                {
                    if (nodeIdViewModel.EnumStrings != null)
                    {
                        for (int ii = 0; ii < nodeIdViewModel.EnumStrings.Length; ii++)
                        {
                            if (nodeIdViewModel.EnumStrings[ii].Text == value)
                            {
                                result = ii;
                                break;
                            }
                        }
                    }
                    else if (nodeIdViewModel.TrueState != null && nodeIdViewModel.TrueState.Text == value)
                        result = true;
                    else if (nodeIdViewModel.FalseState != null && nodeIdViewModel.FalseState.Text == value)
                        result = false;

                }
                catch
                {
                    return result != null;
                }
            }

            return result != null;
        }
        bool HasRange;
        string MonitoredUnit;
        Range MonitoredRange;
        string DataType;
        
        DataValue DataValue;
        string MonitoredValue;
        List<MonitoredItemViewModel.DataObject> DataValueCollection;
        private bool TryParseTextValue(String value, out Object result)
        {
            result = null;
            var nodeIdViewModel = monitoredItemViewModel?.NodeIdModel;
            if (nodeIdViewModel != null)
            try
            {
                if (nodeIdViewModel.EnumStrings != null)
                {
                    int index = Convert.ToInt16(value);
                    if (index < nodeIdViewModel.EnumStrings.Length)
                    {
                        result = nodeIdViewModel.EnumStrings[index].Text;
                    }
                }
                else if (value == bool.TrueString && nodeIdViewModel.TrueState != null)
                    result = nodeIdViewModel.TrueState;
                else if (value == bool.FalseString && nodeIdViewModel.FalseState != null)
                    result = nodeIdViewModel.FalseState;
            }
            catch
            {
                return result != null;
            }

            return result != null;
        }
        private bool TryGetDisplayType(out Object result)
        {
            result = null;
            var nodeIdViewModel = monitoredItemViewModel?.NodeIdModel;
            try
            {
                EnumStringCount = 0;
                if (nodeIdViewModel != null &&
                    nodeIdViewModel.EnumStrings != null &&
                    nodeIdViewModel.EnumStrings.Length > 0)
                {
                    DisplayType = FormatEnum.Enumerated;
                    EnumStringCount = nodeIdViewModel.EnumStrings.Length;
                    result = true;
                }
                else if (nodeIdViewModel != null &&
                    nodeIdViewModel.TrueState != null &&
                    nodeIdViewModel.FalseState != null)
                {
                    DisplayType = FormatEnum.Digital;
                    result = true;
                }
                else if (DataType != null)
                {
                    if (DataType == "String" || DataType == "UtcTime" || DataType == "Time")
                    {
                        DisplayType = FormatEnum.String;
                        result = true;
                    }
                    else if (DataType == "Boolean")
                    {
                        DisplayType = FormatEnum.Boolean;
                        result = true;
                    }
                    else if (DataType == "Byte" || DataType == "SByte" ||
                             DataType == "Int16" || DataType == "UInt16" ||
                             DataType == "Int32" || DataType == "UInt32" ||
                             DataType == "Int64" || DataType == "UInt64")
                    {
                        DisplayType = FormatEnum.Integer;
                        result = true;
                    }
                    else
                    {
                        DisplayType = FormatEnum.Numeric;
                        result = true;
                    }
                }
                else if (DataValueCollection != null)
                {
                    var _value = DataValueCollection[0].Value;
                    if (_value is Byte ||
                         _value is SByte ||
                         _value is Int16 ||
                         _value is UInt16 ||
                         _value is Int32 ||
                         _value is UInt32 ||
                         _value is Int64 ||
                         _value is UInt64)
                        DisplayType = FormatEnum.Integer;
                    else if (_value is Boolean)
                        DisplayType = FormatEnum.Boolean;
                    else if (_value is String)
                        DisplayType = FormatEnum.String;
                    else if (_value is float || _value is double || _value is Double)
                        DisplayType = FormatEnum.Numeric;
                    else
                        DisplayType = FormatEnum.String;

                    result = true;

                }
                else if (DataValue != null)
                {
                    var _value = DataValue.Value;
                    if (_value is Byte ||
                         _value is SByte ||
                         _value is Int16 ||
                         _value is UInt16 ||
                         _value is Int32 ||
                         _value is UInt32 ||
                         _value is Int64 ||
                         _value is UInt64)
                        DisplayType = FormatEnum.Integer;
                    else if (_value is Boolean)
                        DisplayType = FormatEnum.Boolean;
                    else if (_value is String)
                        DisplayType = FormatEnum.String;
                    else if (_value is float || _value is double || _value is Double)
                        DisplayType = FormatEnum.Numeric;
                    else
                        DisplayType = FormatEnum.String;

                    result = true;

                }
                else
                {
                    DisplayType = FormatEnum.String;
                    //result = true;
                }
            }
            catch
            {
                return result != null;
            }

            return result != null;
        }
        #endregion

        #region Command Stuff
        protected void OnIncrease()
        {
            try
            {
                object parsevalue;
                switch (DisplayType)
                {
                    case FormatEnum.Integer:
                    case FormatEnum.Numeric:
                        string _sval = Value;
                        CultureInfo culture = CultureInfo.CurrentCulture;

                        if (Value.IndexOf(':') >= 0)
                        {
                            _sval = Value.Substring(Value.IndexOf(':') + 1);
                            culture = new CultureInfo(Value.Substring(0, Value.IndexOf(':')));
                        }
                        Decimal val = System.Convert.ToDecimal(_sval, culture);
                        var newValue = val + SpinStep;
                        if (newValue <= MaxValue && newValue >= MinValue)
                            SetValue(Convert.ToString(newValue, CultureInfo.InvariantCulture));
                        break;
                    case FormatEnum.Boolean:
                        SetValue(Convert.ToString(true));
                        break;
                    case FormatEnum.Digital:
                        SetValue(Convert.ToString(true));
                        break;
                    case FormatEnum.Enumerated:
                        string _stext = string.Empty;
                        if (StringDisplay != null)
                            _stext = StringDisplay.Text;
                        else if (PasswordBoxDisplay != null)
                            _stext = PasswordBoxDisplay.Password;

                        if (TryParseValue(_stext, out parsevalue) && (int)parsevalue < EnumStringCount - 1)
                        {
                            SetValue(Convert.ToString((int)parsevalue + 1));
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {

            }
        }

        protected void OnDecrease()
        {
            try
            {
                object parsevalue;
                switch (DisplayType)
                {
                    case FormatEnum.Integer:
                    case FormatEnum.Numeric:
                        string _sval = Value;
                        CultureInfo culture = CultureInfo.CurrentCulture;

                        if (Value.IndexOf(':') >= 0)
                        {
                            _sval = Value.Substring(Value.IndexOf(':') + 1);
                            culture = new CultureInfo(Value.Substring(0, Value.IndexOf(':')));
                        }
                        Decimal val = System.Convert.ToDecimal(_sval, culture);
                        var newValue = val - SpinStep;
                        if (newValue <= MaxValue && newValue >= MinValue)
                            SetValue(Convert.ToString(newValue, CultureInfo.InvariantCulture));
                        break;
                    case FormatEnum.Boolean:
                        SetValue(Convert.ToString(false));
                        break;
                    case FormatEnum.Digital:
                        SetValue(Convert.ToString(false));
                        break;
                    case FormatEnum.Enumerated:
                        string _stext = string.Empty;
                        if (StringDisplay != null)
                            _stext = StringDisplay.Text;
                        else if (PasswordBoxDisplay != null)
                            _stext = PasswordBoxDisplay.Password;

                        if (TryParseValue(_stext, out parsevalue))
                        {
                            if (TryParseValue(_stext, out parsevalue) && (int)parsevalue > 0)
                            {
                                SetValue(Convert.ToString((int)parsevalue - 1));
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region MinMaxTags
        private void InitControl()
        {
            InitTag(ref mintag, TagMinValue, mintag_PropertyChanged,TagMinValueProperty.Name);
            InitTag(ref maxtag, TagMaxValue, maxtag_PropertyChanged, TagMaxValueProperty.Name);
        }

        private void InitTag(ref OPCUAEntityReference preparedtag, OPCUAXMLEntityReference xmltag, PropertyChangedEventHandler propertyChangedEventHandler,string property)
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
            if (container == null)
                return;
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
                                Decimal val;
                                System.Decimal.TryParse(m.DataValue.Value.ToString(), out val);
                                MaxValue = val;
                                OnValueChanged(Value);
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
                                Decimal val;
                                System.Decimal.TryParse(m.DataValue.Value.ToString(), out val);
                                MinValue = val;
                                OnValueChanged(Value);
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
       
        #region IEntityReference Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }
        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "TimespanFormat")
            {
                try
                {
                    var test = TimeSpan.MaxValue.ToString(String.Format("{0}", TimespanFormat));
                }
                catch (FormatException e)
                {
                    return Properties.Resources.TimespanFormatInlineError;
                }
            }
            return null;
        }
        #endregion

        #region IDIsposable
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted &&
                d1.Status != DispatcherOperationStatus.Completed)
            {
                d1.Abort();
                // d1 = null;
            }

            if (stringManager != null)
            {
                stringManager.CultureChanged -= StringManager_CultureChanged;
            }

            if (monitoredItemViewModel != null)
            {
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                if (monitoredItemViewModel.NodeIdModel != null)
                    monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;
            }

            if (dpUpdateWarning != null && 
                dpUpdateWarning.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateWarning.Status != DispatcherOperationStatus.Completed)
                dpUpdateWarning.Abort();

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            if (auditTraceViewModel != null)
            {
                auditTraceViewModel.AuditPropertiesFetched -= OnAuditFetched;
                auditTraceViewModel.Dispose();
                auditTraceViewModel = null;
            }

            DetachEvents();
            DetachOverrideBaseProperties();
            ManageSpinHandler(true);

            if (container != null)
                container.Children.Clear();
            BackBorder = null;
            StringDisplay = null;
            MeasUnitTextBox = null;
            PasswordBoxDisplay = null;
            OutOfRangeMarker = null;
            WarningMarker = null;
            if (SpinView!=null)
                SpinView.Child = null;
            SpinView = null;

            if (!bDesign)
            {
                typeHelper.TerminateExecution(this, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, mintag, mintagMonitoredItemViewModel);
                typeHelper.TerminateExecution(this, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, maxtag, maxtagMonitoredItemViewModel);
            }

            if (dpdupdatelayout != null && dpdupdatelayout.Status != DispatcherOperationStatus.Completed
              && dpdupdatelayout.Status != DispatcherOperationStatus.Aborted)
                dpdupdatelayout.Abort();
                
            typeHelper.Dispose();
            typeHelper = null;
        }
        #endregion

        #region IStatisticTagAware
        public int GetStatisticType()
        {
            return (int)StatDef;
        }
        #endregion

        #region IDynamicTagAware
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;

            if(TagMinValue != null && relative == TagMinValue.TagReferenceXml)
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
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void PreserveTagsFromMap(Dictionary<string, string> map)
        {
            var tobeupdated = typeHelper.PreserveTagsFromMap(GetMapDynamics(), map);
            UpdateMapDynamics(tobeupdated);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetConverterLabel(string label)
        {
            ConverterLabel = label;
            if (bInit)
                UpdateMeasUnit();
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
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                mapDataTemplates.Add(PrecisionDigitsProperty, dt);

                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                IWorkspace workspace = null;
                if (Document != null)
                    workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;

                if (workspace != null)
                {
                    dt = new DataTemplate();
                    factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
                    factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
                    dt.DataType = typeof(String);
                    dt.VisualTree = factory;
                    mapDataTemplates.Add(MeasUnitProperty, dt);
                }

                return mapDataTemplates;
            }
        }

        #endregion

        #region IStringIDAware

        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(MeasUnit))
                list.Add(MeasUnit);
            return list;
        }

        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(MeasUnit))
            {
                var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                string propertyName = WPFUtilities.PropertyNameHelper.GetLocalizedPropertyDescriptor
                    (document, typeof(EditDisplay), MeasUnitProperty).DisplayName;
                map.Add(propertyName, MeasUnit);
            }
            return map;
        }
        #endregion

        #region IInheritPropertiesFromControls
        public decimal GetMinValue()
        {
            return MinValue;
        }

        public decimal GetMaxValue()
        {
            return MaxValue;
        }

        public int GetPrecisionDigit()
        {
            return PrecisionDigits;
        }

        public bool GetUseEngineeringUnit()
        {
           return UseEUnit;
        }

        public string GetUnitConverterFromControl(object control)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

            if (Document != null)
            {
                if (!String.IsNullOrEmpty((control as Control).Name) && Document.MapScreenEntities.ContainsKey((control as Control).Name))
                    return Document.MapScreenEntities[(control as Control).Name].IdUnitConverter;
                else 
                    return null;
            }
            else
                return null;
        }

        public string GetTagMinValue()
        {
            return TagMinValue?.TagReferenceXml;
        }

        public string GetTagMaxValue()
        {
            return TagMaxValue?.TagReferenceXml;
        }
        #endregion
    }

    internal class ConvertSpinBackColor : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null || !(sender is EditDisplay))
                return null;

            EditDisplay display = sender as EditDisplay;
            var res = new Dictionary<string, Brush>() {
                        { "SpinBackColor", display.GetUnsetPropertyValue<Brush>(EditDisplay.SpinBackColorProperty, display.SpinBackColor, display.Foreground, false)},
                        //{ "SpinButtonBackColor", display.GetUnsetPropertyValue<Brush>(EditDisplay.SpinButtonBackColorProperty, display.SpinButtonBackColor, display.Background, false)}
                    };
            return res;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null || !(sender is EditDisplay))
                return null;

            EditDisplay display = sender as EditDisplay;
            if ((property as DependencyProperty) != null && (property as DependencyProperty).Name == EditDisplay.SpinBackColorProperty.Name)
                return display.GetUnsetPropertyValue<Brush>(EditDisplay.SpinBackColorProperty, display.SpinBackColor, display.Foreground, false);
            else if ((property as DependencyProperty) != null && (property as DependencyProperty).Name == EditDisplay.SpinButtonBackColorProperty.Name)
                return display.GetUnsetPropertyValue<Brush>(EditDisplay.SpinButtonBackColorProperty, display.SpinButtonBackColor, display.Background, false);
            else
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
