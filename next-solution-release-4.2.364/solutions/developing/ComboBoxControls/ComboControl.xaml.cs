using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBoxControls.Enums;
using Converters;
using Opc.Ua;
using OPCUAViewModel;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.WPF;
using System.Windows.Automation.Peers;
using ComboBoxControls.Automations;
using PropertyControl.ComponentService;
using DocumentManager.ComponentService;
using ScreenSettings;
using UFInterfaces;
using System.Windows.Media.Effects;
using ViewModelLib;
using DynamicTagAwareHelper;
using System.Xml.Serialization;
using WPFUtilities.Extensions;

namespace ComboBoxControls
{
    /// <summary>
    /// Interaction logic for ComboControl.xaml
    /// </summary>
    public partial class ComboControl : ComboBox, IDisposable, IDynamicTagAware, IEntityReference
    {
        #region DP
        #region DP Overrides

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ComboBox));
            dpd.AddValueChangedSafe(this, OnItemsSourceChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ComboBox));
            dpd.RemoveValueChangedSafe(this, OnItemsSourceChanged);
        }

        private void OnItemsSourceChanged(object sender, EventArgs e)
        {
            var control = sender as ComboControl;
            if (control != null)
            {
                control.OnItemsSourceChanged();
            }
        }

        bool bIsInInitMode;
        protected virtual void OnItemsSourceChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bIsInInitMode)
                return;
            bIsInInitMode = true;

            var newValue = ItemsSource;
            var combobox = this;
            if (newValue.Cast<object>().Count() > 0 && newValue.Cast<object>().FirstOrDefault() is ExpandoObject)
            {
                var list = new List<String>();
                foreach (var item in newValue.Cast<object>().ToList())
                {
                    var dic = item as IDictionary<string, object>;
                    if (dic == null)
                        continue;
                    String value = String.Empty;
                    list.Add(dic[dic.Keys.FirstOrDefault()].ToString());
                }
                ItemsSource = list;
            }
            else
            {
                ItemsSource = newValue;
            }

            if (UnderlineText)
                UpdateUnderlineText();

            if (bTagInit)
                OnValueChanged(null, Value);
            bIsInInitMode = false;
        }
        #endregion

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(String), typeof(ComboControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(OnCoerceValue)));

        private static object OnCoerceValue(DependencyObject o, object value)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                return control.OnCoerceValue((String)value);
            else
                return value;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                control.OnValueChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceValue(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool IsInValueChanged;
        bool IsInSelectionChanged;
        protected virtual void OnValueChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bTagInit)
                return; 
            try
            {
                if (IsInSelectionChanged)
                {
                    IsInSelectionChanged = false;
                    return;
                }
                
                if (IsInEditMode)
                    return;

                object parsetextvalue;
                var combobox = this;

                int _index;
                IsInValueChanged = true;
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        if (String.Compare(newValue, "True", true) == 0 || String.Compare(newValue as String, "False", true) == 0)
                        {
                            TagType = FormatEnum.Boolean;

                            if (combobox.ItemsSource == null || ItemSourceCount < 2)
                                ItemsSource = new List<string> { Convert.ToString(false), Convert.ToString(true) };

                            combobox.SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        else
                        {
                            _index = System.Convert.ToInt32(newValue);

                            if (_index < 0 || _index > ItemSourceCount - 1)
                            {
                                //IsIniInitMode = true;
                                combobox.SelectedIndex = -1;
                                combobox.Text = string.Empty;
                            }
                            else
                            {
                                combobox.SelectedIndex = _index;
                                combobox.Text = (string)combobox.SelectedValue;
                            }
                        }
                        break;
                    case FormatEnum.String:
                        combobox.SelectedValue = newValue;
                        combobox.Text = newValue;
                        break;
                    case FormatEnum.Digital:
                        //if (TryParseValue(newValue, out parsetextvalue))
                        {
                            //combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                            combobox.SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0; 
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        break;
                    case FormatEnum.Enumerated:
                        if (TryParseValue(newValue, out parsetextvalue))
                        {
                            combobox.SelectedIndex = (int)parsetextvalue;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        else if (int.TryParse(newValue, out _index))
                        {
                            if (_index < ItemSourceCount)
                            {
                                combobox.SelectedIndex = _index;
                                combobox.Text = (string)combobox.SelectedValue;
                            }
                            else
                            {
                                IsIniInitMode = true;
                                combobox.SelectedIndex = -1;
                                combobox.Text = newValue;
                            }
                        }
                        else
                        {
                            IsIniInitMode = true;
                            combobox.SelectedIndex = -1;
                            combobox.Text = newValue;
                        }
                        IsIniInitMode = false;
                        break;
                    case FormatEnum.Boolean:
                        combobox.SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0;
                        combobox.Text = (string)combobox.SelectedValue;
                        break;
                }
                IsInValueChanged = false;
            }
            catch
            {

            }
        }
        [Category("ComboBoxControlOptions")]
        public String Value
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(ValueProperty);
            }
            set
            {
                if (RunningOnServer)
                    SetIndexValue(value);
                else
                    SetValue(ValueProperty, value);
            }
        }

        public string GetIndexValue()
        {
            switch (TagType)
            {
                case FormatEnum.Numeric:
                    return this.SelectedIndex.ToString();
                case FormatEnum.String:
                    return this.SelectedValue.ToString();
                case FormatEnum.Digital:
                    return this.SelectedIndex.ToString();
                case FormatEnum.Enumerated:
                    return this.SelectedValue.ToString();
                case FormatEnum.Boolean:
                    return this.SelectedIndex == 0 ? "True" : "False";
                default:
                    return this.SelectedValue.ToString();
            }

        }
        private void SetIndexValue(string value)
        {
            try
            {
                if (IsInSelectionChanged)
                {
                    IsInSelectionChanged = false;
                    return;
                }

                if (IsInEditMode)
                    return;

                object parsetextvalue;
                var combobox = this;

                int _index;

                IsInSelectionChanged = true;
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, value, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                        if (_index != -1)
                        {
                            combobox.SelectedIndex = _index;
                            combobox.Text = (string)combobox.SelectedValue;
                            SetValue(ValueProperty, _index.ToString());
                        }
                        else
                        {
                            //IsIniInitMode = true;
                            combobox.SelectedIndex = -1;
                            combobox.Text = string.Empty;
                            SetValue(ValueProperty, value);
                        }
                        break;
                    case FormatEnum.String:
                        combobox.SelectedValue = value;
                        combobox.Text = value;
                        SetValue(ValueProperty, value);
                        break;
                    case FormatEnum.Digital:
                        //if (TryParseValue(value, out parsetextvalue))
                        {
                            combobox.SelectedIndex = String.Compare(value, "True", true) == 0 ? 1 : 0;
                            //combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                            combobox.Text = (string)combobox.SelectedValue;
                            SetValue(ValueProperty, value);
                        }
                        break;
                    case FormatEnum.Enumerated:
                        if (TryParseValue(value, out parsetextvalue))
                        {
                            combobox.SelectedIndex = (int)parsetextvalue;
                            combobox.Text = (string)combobox.SelectedValue;
                            SetValue(ValueProperty, value);
                        }
                        else if (int.TryParse(value, out _index))
                        {
                            if (_index < ItemSourceCount)
                            {
                                combobox.SelectedIndex = _index;
                                combobox.Text = (string)combobox.SelectedValue;
                                SetValue(ValueProperty, value);
                            }
                            else
                            {
                                IsIniInitMode = true;
                                combobox.SelectedIndex = -1;
                                combobox.Text = value;
                                SetValue(ValueProperty, value);
                            }
                        }
                        else
                        {
                            IsIniInitMode = true;
                            combobox.SelectedIndex = -1;
                            combobox.Text = string.Empty;
                            SetValue(ValueProperty, value);
                        }
                        IsIniInitMode = false;
                        break;
                    case FormatEnum.Boolean:
                        combobox.SelectedIndex = String.Compare(value, "True", true) == 0 ? 1 : 0;
                        combobox.Text = (string)combobox.SelectedValue;
                        SetValue(ValueProperty, value);
                        break;
                }
                IsInSelectionChanged = false;

            }
            catch
            {
                IsInSelectionChanged = false;
            }
        }

        #endregion


        #region UnderlineText
        public static readonly DependencyProperty UnderlineTextProperty = DependencyProperty.Register("UnderlineText", typeof(bool), typeof(ComboControl), new UIPropertyMetadata(false));

        [Browsable(false)]
        [XmlIgnore]
        public bool UnderlineText
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UnderlineTextProperty);
            }
            set
            {
                SetValue(UnderlineTextProperty, value);
            }
        }

        #endregion
        
        
        #region ControlFontSettings
        public static readonly DependencyProperty ControlFontSettingsProperty = DependencyProperty.Register("ControlFontSettings", typeof(FontSettings), typeof(ComboControl), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 20), new PropertyChangedCallback(OnControlFontSettingsChanged), new CoerceValueCallback(OnCoerceControlFontSettings)));

        private static object OnCoerceControlFontSettings(DependencyObject o, object value)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                return control.OnCoerceControlFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnControlFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                control.OnControlFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceControlFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }
        bool bChanged;
        protected virtual void OnControlFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //UpdateValueFont(newValue);
            //{

            //}
        }
        [Category("ComboBoxControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings ControlFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(ControlFontSettingsProperty);
            }
            set
            {
                SetValue(ControlFontSettingsProperty, value);
            }
        }
        bool bOverride;
        void UpdateValueFont(FontSettings newValue)
        {
            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;
        }
        #endregion

        #region ComboBoxStyle
        public static readonly DependencyProperty ComboBoxStyleProperty = DependencyProperty.Register("ComboBoxStyle", typeof(ComboBoxStyleEnum), typeof(ComboControl), new UIPropertyMetadata(ComboBoxStyleEnum.Default));
        [Category("ComboBoxControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        [Obsolete("No longer used.")]
        public ComboBoxStyleEnum ComboBoxStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ComboBoxStyleEnum)GetValue(ComboBoxStyleProperty);
            }
            set
            {
                SetValue(ComboBoxStyleProperty, value);
            }
        }

        #endregion
        #region ToggleButtonDimension
        public static readonly DependencyProperty ToggleButtonDimensionProperty = DependencyProperty.Register("ToggleButtonDimension", typeof(Dimension), typeof(ComboControl), new UIPropertyMetadata(Dimension.Small, new PropertyChangedCallback(OnToggleButtonDimensionChanged), new CoerceValueCallback(OnCoerceToggleButtonDimension)));

        private static object OnCoerceToggleButtonDimension(DependencyObject o, object value)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                return control.OnCoerceToggleButtonDimension((Dimension)value);
            else
                return value;
        }

        private static void OnToggleButtonDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                control.OnToggleButtonDimensionChanged((Dimension)e.OldValue, (Dimension)e.NewValue);
        }

        protected virtual Dimension OnCoerceToggleButtonDimension(Dimension value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnToggleButtonDimensionChanged(Dimension oldValue, Dimension newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateArrowDimension(newValue);
        }
        void UpdateArrowDimension(Dimension value)
        {
            switch (value)
            {
                case Dimension.Small:
                    ButtonWidth = (double)SystemParameters.VerticalScrollBarWidth * 1.5;
                    break;
                case Dimension.Medium:
                    ButtonWidth = (double)SystemParameters.VerticalScrollBarWidth * 2;
                    break;
                case Dimension.Large:
                    ButtonWidth = (double)SystemParameters.VerticalScrollBarWidth * 2.5;
                    break;
                default:
                    break;
            }
        }
        [Category("ComboBoxControlOptions")]
        public Dimension ToggleButtonDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Dimension)GetValue(ToggleButtonDimensionProperty);
            }
            set
            {
                SetValue(ToggleButtonDimensionProperty, value);
            }
        }

        #endregion


        #region ToggleButtonBackground
        public static readonly DependencyProperty ToggleButtonBackgroundProperty = DependencyProperty.Register("ToggleButtonBackground", typeof(Brush), typeof(ComboControl), new UIPropertyMetadata(Brushes.Transparent));

        public Brush ToggleButtonBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ToggleButtonBackgroundProperty);
            }
            set
            {
                SetValue(ToggleButtonBackgroundProperty, value);
            }
        }

        #endregion


        #region ButtonWidth
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register("ButtonWidth", typeof(double), typeof(ComboControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnButtonWidthChanged), new CoerceValueCallback(OnCoerceButtonWidth)));

        private static object OnCoerceButtonWidth(DependencyObject o, object value)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                return control.OnCoerceButtonWidth((double)value);
            else
                return value;
        }

        private static void OnButtonWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                control.OnButtonWidthChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceButtonWidth(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnButtonWidthChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Browsable(false)]
        [XmlIgnore]
        public double ButtonWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ButtonWidthProperty);
            }
            set
            {
                SetValue(ButtonWidthProperty, value);
            }
        }

        #endregion

        #region TagItemSource
        public static readonly DependencyProperty TagItemSourceProperty = DependencyProperty.Register("TagItemSource", typeof(OPCUAXMLEntityReference), typeof(ComboControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagItemSourceChanged), new CoerceValueCallback(OnCoerceTagItemSource)));

        private static object OnCoerceTagItemSource(DependencyObject o, object value)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                return control.OnCoerceTagItemSource((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagItemSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboControl control = o as ComboControl;
            if (control != null)
                control.OnTagItemSourceChanged((OPCUAXMLEntityReference)e.OldValue, (OPCUAXMLEntityReference)e.NewValue);
        }

        protected virtual OPCUAXMLEntityReference OnCoerceTagItemSource(OPCUAXMLEntityReference value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTagItemSourceChanged(OPCUAXMLEntityReference oldValue, OPCUAXMLEntityReference newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("Advanced")]
        public OPCUAXMLEntityReference TagItemSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (OPCUAXMLEntityReference)GetValue(TagItemSourceProperty);
            }
            set
            {
                SetValue(TagItemSourceProperty, value);
            }
        }

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

        #region Declarations
        private OPCUAEntityReference itemsourcetag;
        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        bool bTagInit;
        bool bLoaded;
        bool bDesign;
        int EnumStringCount;
        MonitoredItemViewModel monitoredItemViewModel;
        FormatEnum _tagType = FormatEnum.String; 
        FormatEnum TagType 
        { 
            get
            {
                return _tagType;
            }
            set
            {
                _tagType = value;
            }
        }
        TypeHelper typeHelper = new TypeHelper();
        ScreenDocument Document;

        bool bWaitingForSelectionChanged;
        bool bScrolled;
        ListBoxItem lastHoveredItem;
        #endregion
        #region Contructor
        bool bInit;

        public ComboControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Text = string.Empty;

            //combobox.DataContext = this;
            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;
                   
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                    var fes = this;

                    
                    OverrideBaseProperties();
                    UpdateArrowDimension(ToggleButtonDimension);
                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                        Text = Properties.Resources.Option1;
                    }
                    else
                    {
                        InitControl();
                    }

                    if (RunningOnServer)
                    {
                        var elements = (this as UIElement).GetVisualChildrenOfType<FrameworkElement>().ToList();
                        elements.ForEach(x => x.IsHitTestVisible = false);
                        IsHitTestVisible = IsHitTestVisible && IsEnabled;
                    }

                    bInit = true;
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDisposed)
                    return;

                if (DataContext is MonitoredItemViewModel && !bDesign && !DesignerProperties.GetIsInDesignMode(this))
                {
                    if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null)
                        monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    Object parsevalue;
                    if (!TryGetType(out parsevalue))
                        TagType = FormatEnum.String;
                    else if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null &&
                            (TagType == FormatEnum.Digital || TagType == FormatEnum.Enumerated))
                        monitoredItemViewModel.NodeIdModel.PropertyChanged += NodeIdViewModel_PropertyChanged;
                    InitItemsSource();
                    if (IsEditable)
                    {
                        RemoveEventHandler(this);
                        AddEventHandler(this);
                    }
                    OnValueChanged(null, Value);
                    bTagInit = true;
                }
            };
        }

        private void NodeIdViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NodeProperties")
            {
                var nodeIdViewModel = (NodeIdViewModel)sender;
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;

                    try
                    {
                        IsIniInitMode = true;
                        switch (TagType)
                        {
                            case FormatEnum.Digital:
                                ItemsSource = new List<string>() { nodeIdViewModel.FalseState.ToString(), nodeIdViewModel.TrueState.ToString() };
                                break;
                            case FormatEnum.Enumerated:
                                ItemsSource = (from c in nodeIdViewModel.EnumStrings select c.ToString()).ToList();
                                break;
                        }
                    }
                    finally
                    {
                        IsIniInitMode = false;
                    }
                });
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

        #region TagItemSource Prepare
        private void InitControl()
        {
            TouchLeave += OnTouchLeave;

            if (itemsourcetag == null && TagItemSource != null && TagItemSource.TagReference != null)
                itemsourcetag = TagItemSource.TagReference;

            if (itemsourcetag != null && !itemsourcetag.IsRelative && !matchChangedMap.Contains(TagItemSourceProperty.Name))
                typeHelper.PrepareExecution(Properties.Resources.ComboSessionName, Document, this, itemsourcetag_PropertyChanged, itemsourcetag);
        }
    
        MonitoredItemViewModel itemsourcetagMonitoredItemViewModel;
        private void itemsourcetag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (itemsourcetagMonitoredItemViewModel != null)
                    itemsourcetagMonitoredItemViewModel.PropertyChanged -= itemsourcetagMonitoredItemViewModel_PropertyChanged;
                itemsourcetagMonitoredItemViewModel = n.MonitoredItemViewModel;

                if (itemsourcetagMonitoredItemViewModel != null)
                {
                    itemsourcetagMonitoredItemViewModel.PropertyChanged += itemsourcetagMonitoredItemViewModel_PropertyChanged;

                    itemsourcetagMonitoredItemViewModel_PropertyChanged(itemsourcetagMonitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                }
            }
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!bDesign)
            {
                var s = new Style(typeof(ComboBoxItem));
                if (ItemContainerStyle != null)
                    foreach (var setter in ItemContainerStyle.Setters)
                        s.Setters.Add(setter);
                s.Setters.Add(new EventSetter() { Event = TouchEnterEvent, Handler = new EventHandler<TouchEventArgs>(ComboBoxItem_TouchEnter) });
                ItemContainerStyle = s;
            }
        }

        private void ComboBoxItem_TouchEnter(object sender, TouchEventArgs e)
        {
            if (IsStylusOver) // Touch input
            {
                lastHoveredItem = sender as ComboBoxItem;
                if (ItemContainerGenerator.ContainerFromItem(SelectedItem) != sender as ComboBoxItem)
                    bWaitingForSelectionChanged = true;
            }
        }

        private void OnTouchLeave(object sender, TouchEventArgs e)
        {
            if (bWaitingForSelectionChanged && !bScrolled && lastHoveredItem != null)
            {
                lastHoveredItem.Focus();
                lastHoveredItem.IsSelected = true;
                Dispatcher.BeginInvokeAsynchronously(() =>
                {
                    IsDropDownOpen = false;
                });
            }
            bScrolled = false;
            bWaitingForSelectionChanged = false;
        }

        private void comboBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!bDesign)
                bScrolled = true;
        }

        private void itemsourcetagMonitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
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
                                UpdateItemsSource(m.DataValue.Value.ToString());
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
        private void UpdateItemsSource(string p)
        {
            string[] val;
            int index;
            try
            {
                val = p.Split('|');
                IsInValueChanged = true;
                index = SelectedIndex;
                ItemsSource = val;
                if (bTagInit)
                    OnValueChanged(Value, Value);
                else
                    SelectedIndex = index < val.Count() ? index : -1;
            }
            finally
            {
                IsInValueChanged = false;
            }
        }
        private void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                UIElement ue = (from c in this.GetVisualChildrenOfType<Border>()
                                where (c.Name as String) == "EffectBorder"
                                select c).FirstOrDefault() as UIElement;
                if (ue == null)
                    return;

                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        ue.Effect = previousEffect;
                        ue.ClipToBounds = previousClipToBounds;
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
                        previousEffect = ue.Effect;
                        previousClipToBounds = ue.ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        ue.Effect = effect;
                        ue.ClipToBounds = false;
                    }
                }
            });
        }
     
        #endregion

        #region Methods
        void UpdateUnderlineText()
        {
            Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
            {
                if (bDisposed)
                    return;

                TextBoxProperties.ApplyUnderlineTextToChilds(this);
            });
        }

        bool IsInEditMode;
        private void AddEventHandler(Object control)
        {
            //(control as ComboBox).TouchDown += OnTouchDown;
            (control as ComboBox).LostFocus += OnLostFocus;
            (control as ComboBox).PreviewKeyDown += OnPreviewKeyDown;
            (control as ComboBox).PreviewLostKeyboardFocus += OnLostKeyboardFocus;
            (control as ComboBox).MouseDoubleClick += OnMouseDoubleClick;
            //(control as ComboBox).PreviewMouseDown -= OnPreviewMouseDown;
        }

        private void RemoveEventHandler(Object control)
        {
            //(control as ComboBox).TouchDown -= OnTouchDown;
            (control as ComboBox).LostFocus -= OnLostFocus;
            (control as ComboBox).PreviewKeyDown -= OnPreviewKeyDown;
            (control as ComboBox).PreviewLostKeyboardFocus -= OnLostKeyboardFocus;
            (control as ComboBox).MouseDoubleClick -= OnMouseDoubleClick;
            //(control as ComboBox).PreviewMouseDown -= OnPreviewMouseDown;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsEditable)
                return;
            IsInEditMode = true;
            updateSource = true;
            e.Handled = false;
        }

        private void OnPreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (!IsEditable)
                return;
            IsInEditMode = true;
            updateSource = true;
            e.Handled = false;
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsEditable)
                return;
            IsInEditMode = true;
            updateSource = true;
            e.Handled = false;
        }

        bool updateSource;
        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            e.Handled = false;
            if (IsInEditMode)
            {
                if(!updateSource)
                {
                    IsInEditMode = false;
                    return;
                }
                ManageUpdateBinding(sender, null);
            }
        }
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            OSKeyboardHelper.Hide();

            if (IsInEditMode)
            {
                if (!updateSource)
                {
                    IsInEditMode = false;
                    return;
                }
                ManageUpdateBinding(sender, null);
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;

            if (!IsEditable)
            {
                updateSource = false;
                IsInEditMode = false;
                return;
            }

            if (e.Key != Key.Enter)
            {
                updateSource = true;
                IsInEditMode = true;
                return;
            }

            if (!updateSource)
                return;

            ManageUpdateBinding(sender, e);
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            OSKeyboardHelper.Show();
        }

        void ManageUpdateBinding(object sender, KeyEventArgs e)
        {
            int _index;
            var combobox = this;
            switch (TagType)
            {
                case FormatEnum.Numeric:
                    if (e == null || e.Key == Key.Enter)
                    {
                        try
                        {
                            _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, (combobox as ComboBox).Text, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                            if (_index != -1)
                                Value = _index.ToString(CultureInfo.InvariantCulture);

                            else
                            {
                                _index = System.Convert.ToInt32(SelectedValue);
                                if (_index < 0 || _index > ItemSourceCount - 1)
                                {
                                    //IsIniInitMode = true;
                                    combobox.SelectedIndex = -1;
                                    combobox.Text = string.Empty;
                                }
                                else
                                {
                                    combobox.SelectedIndex = _index;
                                    combobox.Text = (string)combobox.SelectedValue;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            int index;
                            if(int.TryParse(Value, out index))
                                combobox.SelectedIndex = index;
                            combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                        }

                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                case FormatEnum.Boolean:
                    if (e == null || e.Key == Key.Enter)
                    {
                        Boolean _bvalue;
                        string _svalue;
                        {
                            _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, (combobox as ComboBox).Text, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                            if (_index != -1 && _index < 2)
                            {
                                Value = _index.ToString(CultureInfo.InvariantCulture);
                                IsInEditMode = false;
                                updateSource = false;
                                break;
                            }
                            _svalue = (combobox as ComboBox).Text;
                            if (_svalue.Equals("1"))
                                Value = Convert.ToString(true);
                            else if (_svalue.Equals("0"))
                                Value = Convert.ToString(false);
                            else
                            {
                                try
                                {
                                    _bvalue = System.Convert.ToBoolean(_svalue);
                                    Value = _bvalue.ToString(CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    combobox.SelectedIndex = String.Compare(Value, "True", true) == 0 ? 1 : 0;
                                    combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                                }
                            }
                        }
                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                case FormatEnum.Digital:
                    if (e == null || e.Key == Key.Enter)
                    {
                        int _enval;
                        {
                            _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, (combobox as ComboBox).Text, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                            if (_index != -1 && _index < 2)
                            {
                                Value = _index.ToString(CultureInfo.InvariantCulture);
                                IsInEditMode = false;
                                updateSource = false;
                                break;
                            }

                            object parsetextvalue;
                            if (!TryParseValue((combobox as ComboBox).Text, out parsetextvalue))
                            {
                                if (int.TryParse((combobox as ComboBox).Text, out _enval) && _enval < EnumStringCount && _enval >= 0)
                                    Value = Convert.ToString(_enval);
                                else if (TryParseValue(Value, out parsetextvalue))
                                {
                                    combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                                    combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                                }
                            }
                            else
                            {
                                Value = Convert.ToString(parsetextvalue);
                            }
                        }
                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                case FormatEnum.Enumerated:
                    if (e == null || e.Key == Key.Enter)
                    {
                        int _enval;
                        {
                            _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, (combobox as ComboBox).Text, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                            if (_index != -1)
                            {
                                Value = _index.ToString(CultureInfo.InvariantCulture);
                                IsInEditMode = false;
                                updateSource = false;
                                break;
                            }

                            object parsetextvalue;
                            if (!TryParseValue((combobox as ComboBox).Text, out parsetextvalue))
                            {
                                if (int.TryParse((combobox as ComboBox).Text, out _enval) && _enval < EnumStringCount && _enval >= 0)
                                    Value = Convert.ToString(_enval);
                                else if (TryParseValue(Value, out parsetextvalue))
                                {
                                    combobox.SelectedIndex = (int)parsetextvalue;
                                    combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                                }
                            }
                            else
                            {
                                Value = Convert.ToString(parsetextvalue);
                            }
                        }
                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                case FormatEnum.String:
                    if (e == null || e.Key == Key.Enter)
                    {
                        Value = (combobox as ComboBox).Text;

                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                default:
                    break;
            }
        }
        bool IsIniInitMode;
        int ItemSourceCount
        {
            get
            {
                return ItemsSource != null ? ItemsSource.Cast<string>().Count() : 0;
            }
        }
        private void InitItemsSource()
        {
            if(TagItemSource == null || TagItemSource.TagReference == null)
            {
                if (ItemsSource == null || ItemSourceCount == 0)
                {
                    IsIniInitMode = true;
                    switch (TagType)
                    {
                        case FormatEnum.Digital:
                            ItemsSource = new List<string>(){ monitoredItemViewModel.NodeIdModel.FalseState.ToString(), monitoredItemViewModel.NodeIdModel.TrueState.ToString() };
                            break;
                        case FormatEnum.Enumerated:
                            ItemsSource = (from c in monitoredItemViewModel.NodeIdModel.EnumStrings select c.ToString()).ToList();
                            break;
                        case FormatEnum.Boolean:
                            ItemsSource = new List<string>() { Convert.ToString(false), Convert.ToString(true) };
                            break;
                    }
                    IsIniInitMode = false;
                }
            }
            else
            {
                if (itemsourcetag != null && itemsourcetag.IsValid)
                {
                    if (itemsourcetag.MonitoredItemViewModel != null && itemsourcetag.MonitoredItemViewModel.DataValue != null)
                    {
                        if (Opc.Ua.StatusCode.IsGood(itemsourcetag.MonitoredItemViewModel.DataValue.StatusCode) ||
                            itemsourcetag.MonitoredItemViewModel.DataValue.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue)
                        {
                            try
                            {
                                if (itemsourcetag.MonitoredItemViewModel.DataValue.Value != null)
                                {
                                    UpdateItemsSource(itemsourcetag.MonitoredItemViewModel.DataValue.Value.ToString()); 
                                }
                            }
                            catch (Exception ex)
                            {
                                SetEntityError(ex.Message);
                            }

                            SetEntityError(null);
                        }
                        else
                            SetEntityError(itemsourcetag.MonitoredItemViewModel.DataValue.StatusCode.ToString());
                    }
                }

            }
        }
        private bool TryGetType(out Object result)
        {
            result = null;

            var nodeIdViewModel = monitoredItemViewModel?.NodeIdModel;
            try
            {
                EnumStringCount = 0;
                if (nodeIdViewModel != null && nodeIdViewModel.EnumStrings != null && nodeIdViewModel.EnumStrings.Length > 0)
                {
                    TagType = FormatEnum.Enumerated;
                    EnumStringCount = nodeIdViewModel.EnumStrings.Length;
                    result = true;
                }
                else if (nodeIdViewModel != null && nodeIdViewModel.TrueState != null && nodeIdViewModel.FalseState != null)
                {
                    TagType = FormatEnum.Digital;
                    result = true;
                }
                else if (monitoredItemViewModel != null && monitoredItemViewModel.DataType != null)
                {
                    if (monitoredItemViewModel.DataType == "String")
                    {
                        TagType = FormatEnum.String;
                        result = true;
                    }
                    else if (monitoredItemViewModel.DataType == "Boolean")
                    {
                        TagType = FormatEnum.Boolean;
                        result = true;
                    }
                    else
                    {
                        TagType = FormatEnum.Numeric;
                        result = true;
                    }
                }
                else if (monitoredItemViewModel != null && monitoredItemViewModel.DataValueCollection != null)
                {
                    var _value = monitoredItemViewModel.DataValueCollection[0].Value;

                    if (_value is String)
                        TagType = FormatEnum.String;
                    else if (_value is Boolean)
                        TagType = FormatEnum.Boolean;
                    else
                        TagType = FormatEnum.Numeric;

                    result = true;
                }
                else if (monitoredItemViewModel != null && monitoredItemViewModel.DataValue != null)
                {
                    var _value = monitoredItemViewModel.DataValue.Value;

                    if (_value is String)
                        TagType = FormatEnum.String;
                    else if (_value is Boolean)
                        TagType = FormatEnum.Boolean;
                    else
                        TagType = FormatEnum.Numeric;

                    result = true;
                }
                else
                {
                    TagType = FormatEnum.Numeric;
                    result = true;
                }
            }
            catch
            {
                return result != null;
            }

            return result != null;
        }
        private bool TryParseValue(String value, out Object result)
        {
            result = null;

            var nodeIdViewModel = monitoredItemViewModel?.NodeIdModel;
            try
            {
                if (nodeIdViewModel != null && nodeIdViewModel.EnumStrings != null)
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
                else if (nodeIdViewModel != null && nodeIdViewModel.TrueState != null && nodeIdViewModel.TrueState.Text == value)
                    result = true;
                else if (nodeIdViewModel != null && nodeIdViewModel.FalseState != null && nodeIdViewModel.FalseState.Text == value)
                    result = false;

            }
            catch
            {
                return result != null;
            }

            return result != null;
        }
        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bWaitingForSelectionChanged = false;
            object parsetextvalue;
            var combobox = this;
            int _value;

            if (!bTagInit)
            {
                e.Handled = true;
                return;
            }

            if(IsInValueChanged)
            {
                IsInValueChanged = false;
                return;
            }

            if (IsIniInitMode)
            {
                IsIniInitMode = false;
                return;
            }

            if (IsInEditMode)
                return;

            try
            {
                IsInSelectionChanged = true;
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        Value = (sender as ComboBox).SelectedIndex.ToString();
                        break;
                    case FormatEnum.Boolean:
                        _value = (sender as ComboBox).SelectedIndex;
                        if (_value == 0)
                        {
                            Value = Convert.ToString(false);
                        }
                        else
                        {
                            Value = Convert.ToString(true);
                            combobox.SelectedIndex = 1;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        break;
                    case FormatEnum.Digital:
                        _value = (sender as ComboBox).SelectedIndex;
                        if (_value < 2)
                        {
                            Value = (sender as ComboBox).SelectedIndex.ToString();
                        }
                        else if (TryParseValue(Value, out parsetextvalue))
                        {
                            combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        break;
                    case FormatEnum.Enumerated:
                        Value = (sender as ComboBox).SelectedIndex.ToString();
                        break;
                    case FormatEnum.String:
                        Value = (sender as ComboBox).SelectedValue != null ? (sender as ComboBox).SelectedValue.ToString() : string.Empty;
                        break;
                    default:
                        break;
                }
                IsInSelectionChanged = false;
            }
            catch
            {
                IsInSelectionChanged = false;
            }
        }
        #endregion

        #region IDIsposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            RemoveEventHandler(this);
            DetachOverrideBaseProperties();

            if (!bDesign)
                typeHelper.TerminateExecution(this, itemsourcetag_PropertyChanged, itemsourcetagMonitoredItemViewModel_PropertyChanged, itemsourcetag, itemsourcetagMonitoredItemViewModel);

            TouchLeave -= OnTouchLeave;

            typeHelper.Dispose();
            typeHelper = null;

            if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null)
                monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;

            previousEffect = null;
        }
        #endregion

        #region IDynamicTagAware
        public Dictionary<string, string> GetMapDynamics()
        {
            var ret = new Dictionary<string, string>();
            if (TagItemSource != null && TagItemSource.TagReference != null /*&& TagItemSource.TagReference.IsValid*/)
                ret.Add(CreateUniqueName(TagItemSourceProperty.Name, ret.Keys.ToList()), TagItemSource.TagReferenceXml);
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
        List<string> matchChangedMap = new List<string>();
        public bool MatchTypeDefinition(String relative, String absolute)
        {
            if (Document == null)
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;
            if (string.IsNullOrEmpty(relative) || string.IsNullOrEmpty(absolute))
                return false;

            if (TagItemSource != null && relative == TagItemSource.TagReferenceXml)
            {
                if (typeHelper.ChecktypeDefinition(Properties.Resources.ComboSessionName, Document, this, bDesign, TagItemSource, relative, absolute, itemsourcetag_PropertyChanged, itemsourcetagMonitoredItemViewModel_PropertyChanged, ref itemsourcetag, itemsourcetagMonitoredItemViewModel))
                    matchChangedMap.Add(TagItemSourceProperty.Name);
            }
            
            return (TagItemSource == null || (TagItemSource != null && matchChangedMap.Contains(TagItemSourceProperty.Name)));
        }
        public void UpdateMapDynamics(Dictionary<string, string> map)
        {
            if (TagItemSource != null && TagItemSource.TagReference != null/* && TagItemSource.TagReference.IsValid*/)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                if (map.ContainsKey(TagItemSourceProperty.Name))
                    newValue.TagReferenceXml = map[TagItemSourceProperty.Name];
                else
                    newValue.TagReference = typeHelper.UpdateTag(TagItemSource.TagReferenceXml, map).FromXml<OPCUAEntityReference>();
                TagItemSource = newValue;
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
    }
}
