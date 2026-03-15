using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DevExpress.Xpo.DB;
using Utilities;
using System.Text;
using System.IO;
using System.Windows.Media;
using DevExpress.Xpf.Gauges;
using Gauges.Enums;
using WPFUtilities.PropertyDataTemplate;
using WPFUtilities.Extensions;
using UFInterfaces.PropertyControl;
using System.Globalization;
using Gauges.PropertyDataTemplate;
using System.Xml.Serialization;
using Utilities.WPF;

namespace Gauges
{
    /// <summary>
    /// Interaction logic for DigitalClock.xaml
    /// </summary>
    public partial class DigitalClock : UserControl, IDisposable, IContainPropertyEditors
    {
        #region DP
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DigitalClock));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DigitalClock));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as DigitalClock;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            FillActive = Foreground;
        }
        #endregion


        #region SevenSegmentType
        public static readonly DependencyProperty SevenSegmentTypeProperty = DependencyProperty.Register("SevenSegmentType", typeof(bool), typeof(DigitalClock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnSevenSegmentTypeChanged), new CoerceValueCallback(OnCoerceSevenSegmentType)));

        private static object OnCoerceSevenSegmentType(DependencyObject o, object value)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                return control.OnCoerceSevenSegmentType((bool)value);
            else
                return value;
        }

        private static void OnSevenSegmentTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                control.OnSevenSegmentTypeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceSevenSegmentType(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSevenSegmentTypeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit)
                UpdateFill();
        }

        public bool SevenSegmentType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(SevenSegmentTypeProperty);
            }
            set
            {
                SetValue(SevenSegmentTypeProperty, value);
            }
        }

        #endregion


        #region TimeText
        public static readonly DependencyProperty TimeTextProperty = DependencyProperty.Register("TimeText", typeof(string), typeof(DigitalClock), new UIPropertyMetadata(string.Format("{0:HH:mm:ss}", DateTime.Now), new PropertyChangedCallback(OnTimeTextChanged), new CoerceValueCallback(OnCoerceTimeText)));

        private static object OnCoerceTimeText(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceTimeText((string)value);
            else
                return value;
        }

        private static void OnTimeTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnTimeTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTimeText(string value) 
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTimeTextChanged(string oldValue, string newValue)
        {
            time7Segment.Text = newValue;
        }
        [Category("ClockStyle")]
        [Browsable(false)]
        [XmlIgnore]
        public string TimeText
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TimeTextProperty);
            }
            set
            {
                SetValue(TimeTextProperty, value);
            }
        }
        

        #endregion


        #region UseSystemTimeZone
        public static readonly DependencyProperty UseSystemTimeProperty = DependencyProperty.Register("UseSystemTimeZone", typeof(bool), typeof(DigitalClock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseSystemTimeChanged), new CoerceValueCallback(OnCoerceUseSystemTime)));

        private static object OnCoerceUseSystemTime(DependencyObject o, object value)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                return control.OnCoerceUseSystemTime((bool)value);
            else
                return value;
        }

        private static void OnUseSystemTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                control.OnUseSystemTimeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseSystemTime(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseSystemTimeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bDesign)
            {
                DisplayAfterTimerElapsed(null, null);
            }
        }
        [Category("ClockStyle")]
        public bool UseSystemTimeZone
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseSystemTimeProperty);
            }
            set
            {
                SetValue(UseSystemTimeProperty, value);
            }
        }

        #endregion



        #region ClockTimeZone
        public static readonly DependencyProperty ClockTimeZoneProperty = DependencyProperty.Register("ClockTimeZone", typeof(ClockTimeZone), typeof(DigitalClock), new UIPropertyMetadata(null, new PropertyChangedCallback(OnClockTimeZoneChanged), new CoerceValueCallback(OnCoerceClockTimeZone)));

        private static object OnCoerceClockTimeZone(DependencyObject o, object value)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                return control.OnCoerceClockTimeZone((ClockTimeZone)value);
            else
                return value;
        }

        private static void OnClockTimeZoneChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock control = o as DigitalClock;
            if (control != null)
                control.OnClockTimeZoneChanged((ClockTimeZone)e.OldValue, (ClockTimeZone)e.NewValue);
        }

        protected virtual ClockTimeZone OnCoerceClockTimeZone(ClockTimeZone value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnClockTimeZoneChanged(ClockTimeZone oldValue, ClockTimeZone newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != null && bDesign && bInit)
            {
                DisplayAfterTimerElapsed(null, null);
            }
        }

        [Category("ClockStyle")]
        public ClockTimeZone ClockTimeZone
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ClockTimeZone)GetValue(ClockTimeZoneProperty);
            }
            set
            {
                SetValue(ClockTimeZoneProperty, value);
            }
        }

        #endregion
        #region ShowSeconds
        public static readonly DependencyProperty ShowSecondsProperty = DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(DigitalClock), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSecondsChanged), new CoerceValueCallback(OnCoerceShowSeconds)));

        private static object OnCoerceShowSeconds(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceShowSeconds((bool)value);
            else
                return value;
        }

        private static void OnShowSecondsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnShowSecondsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSeconds(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSecondsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                if (newValue)
                {
                    time7Segment.SymbolCount = 8;
                    time7Segment.Text = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                }
                else
                {
                    time7Segment.SymbolCount = 5;
                    time7Segment.Text = string.Format("{0:00}:{1:00}", DateTime.Now.Hour, DateTime.Now.Minute);
                }
            }
        }
        [Category("ClockStyle")]
        public bool ShowSeconds
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSecondsProperty);
            }
            set
            {
                SetValue(ShowSecondsProperty, value);
            }
        }
        
        #endregion
        #region EnableBackGroundLayer
        public static readonly DependencyProperty EnableBackGroundLayerProperty = DependencyProperty.Register("EnableBackGroundLayer", typeof(Boolean), typeof(DigitalClock), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableBackGroundLayerChanged), new CoerceValueCallback(OnCoerceEnableBackGroundLayer)));

        private static object OnCoerceEnableBackGroundLayer(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceEnableBackGroundLayer((Boolean)value);
            else
                return value;
        }

        private static void OnEnableBackGroundLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnEnableBackGroundLayerChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceEnableBackGroundLayer(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableBackGroundLayerChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (newValue != oldValue && bLoaded && bInit)
                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    UpdateBackLayer();
                });
        }

        [Category("ClockStyle")]
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
        #region BaseModel
        public static readonly DependencyProperty BaseModelProperty = DependencyProperty.Register("BaseModel", typeof(PredefinedDigitElementKinds), typeof(DigitalClock), new UIPropertyMetadata(PredefinedDigitElementKinds.Eco, new PropertyChangedCallback(OnBaseModelChanged), new CoerceValueCallback(OnCoerceBaseModel)));

        private static object OnCoerceBaseModel(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceBaseModel((PredefinedDigitElementKinds)value);
            else
                return value;
        }

        private static void OnBaseModelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnBaseModelChanged((PredefinedDigitElementKinds)e.OldValue, (PredefinedDigitElementKinds)e.NewValue);
        }

        protected virtual PredefinedDigitElementKinds OnCoerceBaseModel(PredefinedDigitElementKinds value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBaseModelChanged(PredefinedDigitElementKinds oldValue, PredefinedDigitElementKinds newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && oldValue != newValue)
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    UpdateFill();
                });
            }
        }
        [Category("ClockStyle")]
        public PredefinedDigitElementKinds BaseModel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (PredefinedDigitElementKinds)GetValue(BaseModelProperty);
            }
            set
            {
                SetValue(BaseModelProperty, value);
            }
        }
        
        #endregion


        #region ClockBackground
        public static readonly DependencyProperty ClockBackgroundProperty = DependencyProperty.Register("ClockBackground", typeof(Brush), typeof(DigitalClock), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0xD6, 0xD4, 0xD4)), new PropertyChangedCallback(OnClockBackgroundChanged), new CoerceValueCallback(OnCoerceClockBackground)));

        private static object OnCoerceClockBackground(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceClockBackground((Brush)value);
            else
                return value;
        }

        private static void OnClockBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnClockBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceClockBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnClockBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                if (bLoaded && oldValue != newValue)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        UpdateFill();
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
        [Category("ClockStyle")]
        public Brush ClockBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ClockBackgroundProperty);
            }
            set
            {
                SetValue(ClockBackgroundProperty, value);
            }
        }
        #endregion
        
        #region  Foreground

        public static readonly DependencyProperty FillActiveProperty = DependencyProperty.Register("FillActive", typeof(Brush), typeof(DigitalClock), new UIPropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnFillActiveChanged), new CoerceValueCallback(OnCoerceFillActive)));

        private static object OnCoerceFillActive(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceFillActive((Brush)value);
            else
                return value;
        }

        private static void OnFillActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnFillActiveChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFillActive(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFillActiveChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                if(!oldValue.ToString().Equals(newValue.ToString()))
                    UpdateFill();
            }
            catch (Exception ex)
            {

            }
        }
        [Category("ClockStyle")]
        public Brush FillActive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FillActiveProperty);
            }
            set
            {
                SetValue(FillActiveProperty, value);
            }
        }


        public static readonly DependencyProperty FillInactiveProperty = DependencyProperty.Register("FillInactive", typeof(Brush), typeof(DigitalClock), new UIPropertyMetadata(new SolidColorBrush(Colors.Transparent), new PropertyChangedCallback(OnFillInactiveChanged), new CoerceValueCallback(OnCoerceFillInactive)));

        private static object OnCoerceFillInactive(DependencyObject o, object value)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                return digitalClock.OnCoerceFillInactive((Brush)value);
            else
                return value;
        }

        private static void OnFillInactiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DigitalClock digitalClock = o as DigitalClock;
            if (digitalClock != null)
                digitalClock.OnFillInactiveChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFillInactive(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFillInactiveChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            try
            {
                if (!oldValue.ToString().Equals(newValue.ToString()))
                    UpdateFill();
            }
            catch (Exception ex)
            {

            }
        }
        [Category("ClockStyle")]
        public Brush FillInactive
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FillInactiveProperty);
            }
            set
            {
                SetValue(FillInactiveProperty, value);
            }
        }
        

        #endregion

        private void UpdateFill()
        {
            try
            {
                if (bLoaded)
                {

                    UpdateBackLayer();
                    
                    if(!SevenSegmentType && labelLayer.Presentation is DefaultSevenSegmentsPresentation)
                    {
                        var presentation = TryFindResource("customPresentation") as CustomSevenSegmentsPresentation;
                        labelLayer.Presentation = presentation;
                    }
                    else if(SevenSegmentType && labelLayer.Presentation is CustomSevenSegmentsPresentation)
                    {
                        var presentation = TryFindResource("defaultPresentation") as DefaultSevenSegmentsPresentation;
                        labelLayer.Presentation = presentation;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void UpdateBackLayer()
        {
            PredefinedDigitElementKinds model;
            if (!EnableBackGroundLayer && NotRunningOnServer)
            {
                model = PredefinedDigitElementKinds.CleanWhite;
                PredefinedElementKind gaugeModelKind = (DigitalGaugeControl.PredefinedModels as IEnumerable<PredefinedElementKind>).ElementAt((int)model) as PredefinedElementKind;
                time7Segment.Model = (DigitalGaugeModel)Activator.CreateInstance(gaugeModelKind.Type);
                time7Segment.Model.Visibility = System.Windows.Visibility.Hidden;
                backgroundLayer.Presentation = null;
                backgroundLayer.Visible = false;
            }
            else
            {
                model = BaseModel;
                PredefinedElementKind gaugeModelKind = (DigitalGaugeControl.PredefinedModels as IEnumerable<PredefinedElementKind>).ElementAt((int)model) as PredefinedElementKind;
                time7Segment.Model = (DigitalGaugeModel)Activator.CreateInstance(gaugeModelKind.Type);
                time7Segment.Model.Visibility = System.Windows.Visibility.Visible;
                PredefinedElementKind digitModelKind = (DigitalGaugeLayer.PredefinedPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)model) as PredefinedElementKind;
                if (digitModelKind != null)
                {
                    PredefinedDigitalGaugeLayerPresentation blayer = (PredefinedDigitalGaugeLayerPresentation)Activator.CreateInstance(digitModelKind.Type);
                    backgroundLayer.Presentation = blayer;
                    if (blayer != null)
                        blayer.Fill = ClockBackground;
                    backgroundLayer.Visible = true;
                }
            }

        }

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

        [Browsable(false)]
        int ClientTimezoneOffset
        {
            get
            {
                return (int)ScreenSettings.ScreenDocument.GetClientTimezoneOffset(this);
            }
        }
        #endregion
        bool bLoaded = false;
        private DispatcherTimer _updateClock;
        private DispatcherTimer updateLayer;
        //private TimeSpan tst = new TimeSpan(0,0,0);
          #region Methods
        private void DisplayAfterTimerElapsed(object sender, EventArgs e)
        {

            DateTime date;
            if (UseSystemTimeZone)
            {
                if (RunningOnServer)
                    date = DateTime.UtcNow.AddMinutes(ClientTimezoneOffset);
                else
                    date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            }
            else
            {
                if (ClockTimeZone == null)
                    ClockTimeZone = new Gauges.ClockTimeZone() {TimeZone = TimeZoneInfo.Local}; 
                date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ClockTimeZone.TimeZone);
            }

            var hour = date.Hour % 24;
            var minute = date.Minute % 60;
            var second = date.Second;

            if (ShowSeconds)
            {
                TimeText = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
            }
            else
            {
                TimeText = string.Format("{0:00}:{1:00}", hour, minute);
            }
        }
        #endregion
        bool bInit;
        bool bDesign;
        public DigitalClock()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            // we used datacontex because releative source sometimes fails with invalid expression binding.
            // see: https://support.progea.com/Products/default.asp?11080
            time7Segment.DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    

                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        bDesign = true;

                        UpdateFill();
                        time7Segment.IsEnabled = false;

                        if (ClockTimeZone == null)
                            ClockTimeZone = new Gauges.ClockTimeZone() { TimeZone = TimeZoneInfo.Local };

                        DisplayAfterTimerElapsed(null, null);
                    }
                    else
                    {
                        UpdateFill();

                        if (ClockTimeZone == null)
                            ClockTimeZone = new Gauges.ClockTimeZone() { TimeZone = TimeZoneInfo.Local };

                        DisplayAfterTimerElapsed(null, null);
                        time7Segment.IsEnabled = NotRunningOnServer;

                        if (_updateClock == null)
                        {
                            _updateClock = new DispatcherTimer(DispatcherPriority.Render);
                            _updateClock.Interval = TimeSpan.FromMilliseconds(958);
                            _updateClock.Tick += DisplayAfterTimerElapsed;
                        }

                        if (NotRunningOnServer)
                        {
                            labelLayer.Animation = new BlinkingAnimation()
                            {
                                Enable = true,
                                RefreshTime = new TimeSpan(0, 0, 0, 0, 500),
                                SymbolsStates = new StatesMask(new bool[8] { false, false, true, false, false, true, false, false })
                            };
                        }

                        _updateClock.Start();
                    }
                    
                    OverrideBaseProperties();
                    bInit = true;
                    time7Segment.Text = TimeText;
                }
            };
        }
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            
            //blinkAnimation.Enable = false;
            if (_updateClock != null)
            {
                _updateClock.Stop();
                _updateClock.Tick -= DisplayAfterTimerElapsed;
                _updateClock = null;
            }

            DetachOverrideBaseProperties();
        }

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

                // Defines Data Template for 'RecipeNameProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(TimeZonePropertyEditor));
                dt.DataType = typeof(ClockTimeZone);
                dt.VisualTree = factory;
                mapDataTemplates.Add(ClockTimeZoneProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion
    }
}
