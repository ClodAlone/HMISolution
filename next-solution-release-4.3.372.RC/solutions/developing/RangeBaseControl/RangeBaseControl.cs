using Converters;
using DynamicTagAwareHelper;
using Opc.Ua;
using OPCUAViewModel;
using ScreenSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using UFInterfaces;
using Utilities;
using WPFUtilities.Converters;
using WPFUtilities.Extensions;

namespace RangeBaseControl
{
    public abstract class RangeBaseControl : UserControl, IDisposable, IEntityReference, IDynamicTagAware
    {
        #region Declarations
        MonitoredItemViewModel monitoredItemViewModel;
        OPCUAEntityReference mintag;
        OPCUAEntityReference maxtag;
        TypeHelper typeHelper = new TypeHelper();
        List<string> matchChangedMap = new List<string>();
        DataValue lastDataValue;
        DispatcherOperation dpUpdateWarning;
        CancellationTokenSource cts;
        object lockObject = new object();
        bool bTagMinValueSet;
        bool bTagMaxValueSet;
        #endregion
        #region Protected
        protected string lastError;
        protected Effect previousEffect;
        protected bool bLoaded;
        protected bool bInit;
        protected bool bDataContextChanging;
        protected bool bDesign;
        protected double _StartValue;
        protected double _EndValue;
        protected ScreenDocument Document;
        protected bool bDispose;
        protected string ConverterLabel;
        [Browsable(false)]
        protected bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion
        #region DPs
        #region OverrideBaseProperties

        public virtual void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RangeBaseControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RangeBaseControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RangeBaseControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RangeBaseControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RangeBaseControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        public virtual void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RangeBaseControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RangeBaseControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RangeBaseControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RangeBaseControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RangeBaseControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as RangeBaseControl;
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
            var control = sender as RangeBaseControl;
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
            var control = sender as RangeBaseControl;
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
            var control = sender as RangeBaseControl;
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

        String lastForeground;
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as RangeBaseControl;
            var foreground = BindingExpressionHelper.Save(Foreground);
            if (control != null && (lastForeground == null || lastForeground != foreground))
            {
                lastForeground = foreground;
                control.OnForegroundChanged();
            }
        }

        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && bDesign && DataContext != null && ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                LabelForeground = Foreground;
                ValueForeground = Foreground;
                EngeneeringUnitForeground = Foreground;
            }
        }
        #endregion
        #region LabelForeground
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(RangeBaseControl), new UIPropertyMetadata(Brushes.Black));
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
        #region ValueForeground
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register("ValueForeground", typeof(Brush), typeof(RangeBaseControl), new UIPropertyMetadata(Brushes.Black));
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
        #region EngeneeringUnitForeground
        public static readonly DependencyProperty EngeneeringUnitForegroundProperty = DependencyProperty.Register("EngeneeringUnitForeground", typeof(Brush), typeof(RangeBaseControl), new UIPropertyMetadata(Brushes.Black));
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
        #region LabelFontSettings
        public static readonly DependencyProperty LabelFontSettingsProperty = DependencyProperty.Register("LabelFontSettings", typeof(FontSettings), typeof(RangeBaseControl), new UIPropertyMetadata(new FontSettings(FontWeights.Medium, FontStyles.Normal, new FontFamily("Segoe UI"), 10)));
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
        #region ValueFontSettings
        public static readonly DependencyProperty ValueFontSettingsProperty = DependencyProperty.Register("ValueFontSettings", typeof(FontSettings), typeof(RangeBaseControl), new UIPropertyMetadata(new FontSettings(FontWeights.Medium, FontStyles.Normal, new FontFamily("Segoe UI"), 24)));
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
        #region EngeneeringUnitFontSettings
        public static readonly DependencyProperty EngeneeringUnitFontSettingsProperty = DependencyProperty.Register("EngeneeringUnitFontSettings", typeof(FontSettings), typeof(RangeBaseControl), new UIPropertyMetadata(new FontSettings(FontWeights.Medium, FontStyles.Normal, new FontFamily("Segoe UI"), 24)));
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
        #region UseEUnit
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(RangeBaseControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseEUnitChanged), new CoerceValueCallback(OnCoerceUseEUnit)));

        private static object OnCoerceUseEUnit(DependencyObject o, object value)
        {
            RangeBaseControl control = o as RangeBaseControl;
            if (control != null)
                return control.OnCoerceUseEUnit((bool)value);
            else
                return value;
        }

        private static void OnUseEUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl control = o as RangeBaseControl;
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
            if (oldValue != newValue && (bLoaded && bInit && !bDataContextChanging))
            {
                UpdateScales();
            }
        }
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

        #region EngeneeringUnit
        public static readonly DependencyProperty EngeneeringUnitProperty = DependencyProperty.Register("EngeneeringUnit", typeof(string), typeof(RangeBaseControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceEngeneeringUnit)));

        private static object OnCoerceEngeneeringUnit(DependencyObject o, object value)
        {
            RangeBaseControl control = o as RangeBaseControl;
            if (control != null)
                return control.OnCoerceEngeneeringUnit((string)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl control = o as RangeBaseControl;
            if (control != null)
                control.OnEngeneeringUnitChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceEngeneeringUnit(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEngeneeringUnitChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateCustomElements();
        }

        public string EngeneeringUnit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(EngeneeringUnitProperty);
            }
            set
            {
                SetValue(EngeneeringUnitProperty, value);
            }
        }

        #endregion
        #region TagMinValue
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(RangeBaseControl), new UIPropertyMetadata(null));
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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(RangeBaseControl), new UIPropertyMetadata(null));
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
        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RangeBaseControl), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            RangeBaseControl control = o as RangeBaseControl;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl control = o as RangeBaseControl;
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
            if (oldValue != newValue && (bLoaded && bInit && !bDataContextChanging))
            {
                UpdateRanges();
            }
        }

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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RangeBaseControl), new UIPropertyMetadata(100d, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            RangeBaseControl control = o as RangeBaseControl;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl control = o as RangeBaseControl;
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
            if (oldValue != newValue && (bLoaded && bInit && !bDataContextChanging))
            {
                UpdateRanges();
            }
        }

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
        #region StartValue
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(Double), typeof(RangeBaseControl), new UIPropertyMetadata((Double)100.0, new PropertyChangedCallback(OnStartValueChanged), new CoerceValueCallback(OnCoerceStartValue)));

        private static object OnCoerceStartValue(DependencyObject o, object value)
        {
            RangeBaseControl RangeBaseControl = o as RangeBaseControl;
            if (RangeBaseControl != null)
                return RangeBaseControl.OnCoerceStartValue((Double)value);
            else
                return value;
        }

        private static void OnStartValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl RangeBaseControl = o as RangeBaseControl;
            if (RangeBaseControl != null)
                RangeBaseControl.OnStartValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register("EndValue", typeof(Double), typeof(RangeBaseControl), new UIPropertyMetadata((Double)100.0, new PropertyChangedCallback(OnEndValueChanged), new CoerceValueCallback(OnCoerceEndValue)));

        private static object OnCoerceEndValue(DependencyObject o, object value)
        {
            RangeBaseControl RangeBaseControl = o as RangeBaseControl;
            if (RangeBaseControl != null)
                return RangeBaseControl.OnCoerceEndValue((Double)value);
            else
                return value;
        }

        private static void OnEndValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeBaseControl RangeBaseControl = o as RangeBaseControl;
            if (RangeBaseControl != null)
                RangeBaseControl.OnEndValueChanged((Double)e.OldValue, (Double)e.NewValue);
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        #endregion
        #region ctor    
        public RangeBaseControl()
        {
            OverrideBaseProperties();
            engStartValue = MinValue;
            engEndValue = MaxValue;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

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
                    bDataContextChanging = true;
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
        #endregion
        #region Methods
        private void UpdateScales()
        {
            if (bInit)
            {
                UpdateStartEndValues();
                UpdateScaleStartEndValues();
                UpdateScaleRanges();
            }
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

        protected void InitControl()
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

        protected MonitoredItemViewModel mintagMonitoredItemViewModel;
        protected MonitoredItemViewModel maxtagMonitoredItemViewModel;
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
                                bTagMaxValueSet = true;
                                _EndValue = monitoredHasRange ? Math.Min(engEndValue, val) : val;
                                UpdateScales();
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
                                bTagMinValueSet = true;
                                _StartValue = monitoredHasRange ? Math.Max(engStartValue, val) : val;
                                UpdateScales();
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

        double engStartValue;
        double engEndValue;
        bool monitoredHasRange;
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

                    if (string.IsNullOrEmpty(EngeneeringUnit) && UseEUnit)
                        EngeneeringUnit = monitoredItemViewModel.EUInformation?.DisplayName?.ToString();
                }
                else
                    monitoredHasRange = false;

                if (TagMinValue == null || TagMinValue.TagReference == null || !bTagMinValueSet)
                    _StartValue = startValue;
                if (TagMaxValue == null || TagMaxValue.TagReference == null || !bTagMaxValueSet)
                    _EndValue = endValue;

                bDataContextChanging = false;
                UpdateScales();
            };

            //if (UseEUnit)
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

        protected void UpdateStartEndValues()
        {
            if (bDesign)
            {
                _StartValue = UseEUnit ? 0.0 : MinValue;
                _EndValue = UseEUnit ? 100.0 : MaxValue;
            }
        }
        #endregion
        #region abstract Methods
        protected abstract void SetEntityError(String error);
        protected abstract void UpdateScaleRanges();
        protected abstract void UpdateScaleStartEndValues();
        protected abstract void ShowMarker(bool v);
        protected abstract void UpdateCustomElements();
        #endregion
        #region IDisposable
        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        /// 
        public void Dispose()
        {
            if (bDispose)
                return;
            Dispose(true);
            bDispose = true;
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        ///
        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
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

                if (!bDesign)
                {
                    typeHelper.TerminateExecution(this, mintag_PropertyChanged, mintagMonitoredItemViewModel_PropertyChanged, mintag, mintagMonitoredItemViewModel);
                    typeHelper.TerminateExecution(this, maxtag_PropertyChanged, maxtagMonitoredItemViewModel_PropertyChanged, maxtag, maxtagMonitoredItemViewModel);
                }

                typeHelper.Dispose();
                typeHelper = null;
                previousEffect = null;
            }
        }
        #endregion
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
                UpdateCustomElements();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUnitConverterSystem(string converterSystem)
        {
        }
        #endregion
    }
}
