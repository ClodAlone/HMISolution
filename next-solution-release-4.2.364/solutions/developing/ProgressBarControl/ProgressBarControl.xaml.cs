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
using System.Windows.Automation.Peers;
using System.Windows.Media.Effects;
using System.Threading;
using System.Xml;
using System.Windows.Controls.Primitives;
using UFInterfaces;
using ProgressBarControl.Automations;
using OPCUAViewModel;
using ScreenSettings;
using DynamicTagAwareHelper;

namespace ProgressBarControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ProgressBarControl : ProgressBar, IDisposable, IDynamicTagAware, IEntityReference
    {

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        #endregion

        #region DP
        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(ProgressBarControl), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            ProgressBarControl control = o as ProgressBarControl;
            if (control != null)
                return control.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBarControl control = o as ProgressBarControl;
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(ProgressBarControl), new UIPropertyMetadata(100d, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            ProgressBarControl control = o as ProgressBarControl;
            if (control != null)
                return control.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBarControl control = o as ProgressBarControl;
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

        #region EngeneeringUnit
        public static readonly DependencyProperty EngeneeringUnitProperty = DependencyProperty.Register("EngeneeringUnit", typeof(string), typeof(ProgressBarControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEngeneeringUnitChanged), new CoerceValueCallback(OnCoerceEngeneeringUnit)));

        private static object OnCoerceEngeneeringUnit(DependencyObject o, object value)
        {
            ProgressBarControl control = o as ProgressBarControl;
            if (control != null)
                return control.OnCoerceEngeneeringUnit((string)value);
            else
                return value;
        }

        private static void OnEngeneeringUnitChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBarControl control = o as ProgressBarControl;
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

        #region UseEUnit
        public static readonly DependencyProperty UseEUnitProperty = DependencyProperty.Register("UseEUnit", typeof(bool), typeof(ProgressBarControl), new UIPropertyMetadata(true));
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
        public static readonly DependencyProperty TagMinValueProperty = DependencyProperty.Register("TagMinValue", typeof(OPCUAXMLEntityReference), typeof(ProgressBarControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMinValueChanged), new CoerceValueCallback(OnCoerceTagMinValue)));

        private static object OnCoerceTagMinValue(DependencyObject o, object value)
        {
            ProgressBarControl control = o as ProgressBarControl;
            if (control != null)
                return control.OnCoerceTagMinValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBarControl control = o as ProgressBarControl;
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
            if (bLoaded && bInit && !bDesign)
                InitTag(ref mintag, TagMinValue, mintag_PropertyChanged, TagMinValueProperty.Name);
        }

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
        public static readonly DependencyProperty TagMaxValueProperty = DependencyProperty.Register("TagMaxValue", typeof(OPCUAXMLEntityReference), typeof(ProgressBarControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagMaxValueChanged), new CoerceValueCallback(OnCoerceTagMaxValue)));

        private static object OnCoerceTagMaxValue(DependencyObject o, object value)
        {
            ProgressBarControl control = o as ProgressBarControl;
            if (control != null)
                return control.OnCoerceTagMaxValue((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProgressBarControl control = o as ProgressBarControl;
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
            if (bLoaded && bInit && !bDesign)
                InitTag(ref maxtag, TagMaxValue, maxtag_PropertyChanged, TagMaxValueProperty.Name);
        }

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


        #region Measure
        public static readonly DependencyProperty MeasureProperty = DependencyProperty.Register("Measure", typeof(string), typeof(ProgressBarControl), new UIPropertyMetadata(null));
        [Browsable(false)]
        public string Measure
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(MeasureProperty);
            }
            set
            {
                SetValue(MeasureProperty, value);
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
        ScreenDocument Document;
        TypeHelper typeHelper = new TypeHelper();
        CancellationTokenSource cts;
        #endregion

        #region ctor
        public ProgressBarControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
                    
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;

                    UpdateCustomElements();
                    if (!bDesign)
                        InitControl();

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

        #region Method
        void UpdateCustomElements()
        {
            Measure = ConverterLabel ?? EngeneeringUnit;
        }
        double engStartValue;
        double engEndValue;
        bool bTagMinValueSet;
        bool bTagMaxValueSet;
        MonitoredItemViewModel monitoredItemViewModel;
        bool bInitEUnit;
        bool monitoredHasRange;
        protected void UpdateRanges()
        {
            double startValue = MinValue;
            double endValue = MaxValue;
            engStartValue = MinValue;
            engEndValue = MaxValue;
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
                bInitEUnit = true;
                if (TagMinValue == null || TagMinValue.TagReference == null || !bTagMinValueSet)
                    Minimum = startValue;
                else if (bTagMinValueSet && mintag != null && mintag.MonitoredItemViewModel != null)
                    UpdateMinMaxTagValue(mintag.MonitoredItemViewModel.DataValue, false);

                if (TagMaxValue == null || TagMaxValue.TagReference == null || !bTagMaxValueSet)
                    Maximum = endValue;
                else if (bTagMaxValueSet && maxtag != null && maxtag.MonitoredItemViewModel != null)
                    UpdateMinMaxTagValue(maxtag.MonitoredItemViewModel.DataValue, true);

                bDatacontextChanging = false;
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
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            templateApplied = true;
        }

        private void InitControl()
        {
            //WPFUtilities.MergedStylesExtension.ApplyCustomStyle<ProgressBarControl>(this);

            Minimum = MinValue;
            Maximum = MaxValue;
            InitTag(ref mintag, TagMinValue, mintag_PropertyChanged, TagMinValueProperty.Name);
            InitTag(ref maxtag, TagMaxValue, maxtag_PropertyChanged, TagMaxValueProperty.Name);
        }

        private void InitTag(ref OPCUAEntityReference preparedtag, OPCUAXMLEntityReference xmltag, PropertyChangedEventHandler propertyChangedEventHandler, string property)
        {
            if (preparedtag == null && xmltag != null && xmltag.TagReference != null)
            {
                preparedtag = xmltag.TagReference;
                if (preparedtag != null && !preparedtag.IsRelative && !matchChangedMap.Contains(property))
                    typeHelper.PrepareExecution(Properties.Resources.SessionName, Document, this, propertyChangedEventHandler, preparedtag);
            }
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

            if (e.PropertyName == "DataValue")
            {
                UpdateMinMaxTagValue(m.DataValue, true);
            }
        }

        private void mintagMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDispose)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (m == null)
                return;

            if (e.PropertyName == "DataValue")
            {
                UpdateMinMaxTagValue(m.DataValue, false);
            }
        }
        void UpdateMinMaxTagValue(Opc.Ua.DataValue dataValue, bool bMaxValue)
        {
            if (dataValue != null)
            {
                if (Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) ||
                    dataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                {
                    try
                    {
                        if (dataValue.Value != null)
                        {
                            double val;
                            System.Double.TryParse(dataValue.Value.ToString(), out val);
                            if(bMaxValue)
                            {
                                bTagMaxValueSet = true;
                                Maximum = bInitEUnit && monitoredHasRange ? Math.Min(engEndValue, val) : val;
                            }
                            else
                            {
                                bTagMinValueSet = true;
                                Minimum = bInitEUnit && monitoredHasRange ? Math.Max(engStartValue, val) : val;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
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
        protected string ConverterLabel;
        public void SetConverterLabel(string label)
        {
            ConverterLabel = label;
            if (bInit)
                UpdateCustomElements();
        }
        #endregion
    }
}
