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
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBoxControls.Automations;
using ComboBoxControls.Enums;
using Converters;
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using ScreenSettings;
using UFInterfaces;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using DynamicTagAwareHelper;
using System.Xml.Serialization;
using WPFUtilities.Extensions;
using Utilities.WPF;

namespace ComboBoxControls
{
    /// <summary>
    /// Interaction logic for ListBoxControl.xaml
    /// </summary>
    public partial class ListBoxControl : ListBox, IEntityReference, IDisposable, IDynamicTagAware
    {
        #region DP
        #region DP Overrides

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ListBoxControl));
            dpd.AddValueChangedSafe(this, OnItemsSourceChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ListBoxControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ListBoxControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ListBoxControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ListBoxControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(ListBoxControl));
            dpd.RemoveValueChangedSafe(this, OnItemsSourceChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ListBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ListBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ListBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ListBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as ListBoxControl;
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
                var value = FontSettings.Clone();
                value.FontFamily = FontFamily;
                FontSettings = value;
                
                
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as ListBoxControl;
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
                var value = FontSettings.Clone();
                value.FontWeight = FontWeight;
                FontSettings = value;
                
                
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as ListBoxControl;
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
                var value = FontSettings.Clone();
                value.FontStyle = FontStyle;
                FontSettings = value;
                
                
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as ListBoxControl;
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
                var value = FontSettings.Clone();
                value.FontSize = (int)FontSize;
                FontSettings = value;
            }
        }

        private void OnItemsSourceChanged(object sender, EventArgs e)
        {
            var control = sender as ListBoxControl;
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

            if(bTagInit)
                OnSelectedValueChanged(string.Empty, SelectedValue);

            if (RunningOnServer) //Updating element layout for ListBoxAutomationPeer
            {
                UpdateLayout();
                peer?.ResetChildrenCache();
            }

            bIsInInitMode = false;
        }
        #endregion

        #region Custom automation peers
        //protected override AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new ListValueAutomationPeer(this);
        //}

        private AutomationPeer peer;
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            peer = base.OnCreateAutomationPeer();
            return peer;
        }

        internal string GetIndexValue()
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
        #endregion

        #region UnderlineText
        public static readonly DependencyProperty UnderlineTextProperty = DependencyProperty.Register("UnderlineText", typeof(bool), typeof(ListBoxControl), new UIPropertyMetadata(false));

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
        
        #region SelectedValue
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(String), typeof(ListBoxControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSelectedValueChanged), new CoerceValueCallback(OnCoerceSelectedValue)));

        private static object OnCoerceSelectedValue(DependencyObject o, object value)
        {
            ListBoxControl control = o as ListBoxControl;
            if (control != null)
                return control.OnCoerceSelectedValue((String)value);
            else
                return value;
        }

        private static void OnSelectedValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ListBoxControl control = o as ListBoxControl;
            if (control != null)
                control.OnSelectedValueChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceSelectedValue(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectedValueChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!bTagInit)
                return;
            try
            {
                object parsetextvalue;
                int _index;
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        if (String.Compare(newValue, "True", true) == 0 || String.Compare(newValue as String, "False", true) == 0)
                        {
                            TagType = FormatEnum.Boolean;

                            if (ItemsSource == null || ItemSourceCount < 2)
                                ItemsSource = new List<string> { Convert.ToString(false), Convert.ToString(true) };

                            SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0;
                        }
                        else
                        {
                            _index = System.Convert.ToInt32(newValue);

                            if (_index < 0 || _index > ItemSourceCount - 1)
                            {
                                //IsIniInitMode = true;
                                SelectedIndex = -1;
                            }
                            else
                            {
                                SelectedIndex = _index;
                            }
                        }
                        break;
                    case FormatEnum.String:
                        IsIniInitMode = true;
                        if (string.IsNullOrEmpty(newValue))
                        {
                            SelectedIndex = -1;
                            IsIniInitMode = false;
                            return;
                        }

                        var _selectedvalues = newValue.Split('|');
                        if (_selectedvalues.Count() == 0)
                        {
                            SelectedIndex = -1;
                            IsIniInitMode = false;
                            return;
                        }

                        if (SelectionMode == System.Windows.Controls.SelectionMode.Single)
                        {
                            SelectedItem = _selectedvalues[0];
                        }
                        else
                        {
                            SelectedItems.Clear();
                            foreach (var item in _selectedvalues)
                            {
                               SelectedItems.Add(item);
                            }
                        }
                        IsIniInitMode = false;
                        break;
                    //case FormatEnum.Digital:
                    //    if (TryParseValue(newValue, out parsetextvalue))
                    //    {
                    //        SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                    //    }
                    //    break;
                    case FormatEnum.Enumerated:
                        if (TryParseValue(newValue, out parsetextvalue))
                        {
                            SelectedIndex = (int)parsetextvalue;
                        }
                        else if (int.TryParse(newValue, out _index))
                        {
                            if(_index < ItemSourceCount)
                            SelectedIndex = _index;
                            else
                            {
                                IsIniInitMode = true;
                                SelectedIndex = -1;
                                IsIniInitMode = false;
                            }
                        }
                        else
                        {
                            IsIniInitMode = true;
                            SelectedIndex = -1;
                            IsIniInitMode = false;
                        }
                        break;
                    case FormatEnum.Digital:
                    case FormatEnum.Boolean:
                        SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0;
                        break;
                }
            }
            catch
            {
            }
        }

        [Category("ListBoxControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        public String SelectedValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(SelectedValueProperty);
            }
            set
            {
                SetValue(SelectedValueProperty, value);
            }
        }

        #endregion


        #region FontSettings
        public static readonly DependencyProperty FontSettingsProperty = DependencyProperty.Register("FontSettings", typeof(FontSettings), typeof(ListBoxControl), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 20), new PropertyChangedCallback(OnFontSettingsChanged), new CoerceValueCallback(OnCoerceFontSettings)));

        private static object OnCoerceFontSettings(DependencyObject o, object value)
        {
            ListBoxControl control = o as ListBoxControl;
            if (control != null)
                return control.OnCoerceFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ListBoxControl control = o as ListBoxControl;
            if (control != null)
                control.OnFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);
            
            //{
            
            //}
        }
        [Category("ListBoxControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings FontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(FontSettingsProperty);
            }
            set
            {
                SetValue(FontSettingsProperty, value);
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


        #region TagItemSource
        public static readonly DependencyProperty TagItemSourceProperty = DependencyProperty.Register("TagItemSource", typeof(OPCUAXMLEntityReference), typeof(ListBoxControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTagItemSourceChanged), new CoerceValueCallback(OnCoerceTagItemSource)));

        private static object OnCoerceTagItemSource(DependencyObject o, object value)
        {
            ListBoxControl control = o as ListBoxControl;
            if (control != null)
                return control.OnCoerceTagItemSource((OPCUAXMLEntityReference)value);
            else
                return value;
        }

        private static void OnTagItemSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ListBoxControl control = o as ListBoxControl;
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

        //#region SelectionMode
        //public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ListBoxControl), new UIPropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged), new CoerceValueCallback(OnCoerceSelectionMode)));

        //private static object OnCoerceSelectionMode(DependencyObject o, object value)
        //{
        //    ListBoxControl control = o as ListBoxControl;
        //    if (control != null)
        //        return control.OnCoerceSelectionMode((SelectionMode)value);
        //    else
        //        return value;
        //}

        //private static void OnSelectionModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    ListBoxControl control = o as ListBoxControl;
        //    if (control != null)
        //        control.OnSelectionModeChanged((SelectionMode)e.OldValue, (SelectionMode)e.NewValue);
        //}

        //protected virtual SelectionMode OnCoerceSelectionMode(SelectionMode value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnSelectionModeChanged(SelectionMode oldValue, SelectionMode newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    SelectionMode = newValue;
        //}

        //[Category("ListBoxControlOptions")]
        //public SelectionMode SelectionMode
        //{
        //    // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
        //    get
        //    {
        //        return (SelectionMode)GetValue(SelectionModeProperty);
        //    }
        //    set
        //    {
        //        SetValue(SelectionModeProperty, value);
        //    }
        //}
        //#endregion


        #region ListBoxStyle
        public static readonly DependencyProperty ListBoxStyleProperty = DependencyProperty.Register("ListBoxStyle", typeof(ComboBoxStyleEnum), typeof(ListBoxControl), new UIPropertyMetadata(ComboBoxStyleEnum.Default));
        [Category("ListBoxControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        [Obsolete("No longer used.")]
        public ComboBoxStyleEnum ListBoxStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ComboBoxStyleEnum)GetValue(ListBoxStyleProperty);
            }
            set
            {
                SetValue(ListBoxStyleProperty, value);
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


        #region SelectionBackgroundColor
        public static readonly DependencyProperty SelectionBackgroundColorProperty = DependencyProperty.Register("SelectionBackgroundColor", typeof(Brush), typeof(ListBoxControl), new UIPropertyMetadata(null));
        #endregion
        #region SelectionForegroundColor
        public static readonly DependencyProperty SelectionForegroundColorProperty = DependencyProperty.Register("SelectionForegroundColor", typeof(Brush), typeof(ListBoxControl), new UIPropertyMetadata(null));
        #endregion

        [SvgValueConverter(ConverterType = typeof(ConvertSelectionColor), RequiredKey = true, NeedSVGUrlBrushes = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush SelectionBackgroundColor { get { return Background; } }
        [SvgValueConverter(ConverterType = typeof(ConvertSelectionColor), RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public Brush SelectionForegroundColor { get { return Foreground; } }
        #endregion

        #region Declarations
        private OPCUAEntityReference itemsourcetag;
        Effect previousEffect;
        bool previousClipToBounds;
        bool errorEffectOn;
        bool IsInValueChanged;
        bool bLoaded;
        bool bDesign;
        bool bTagInit;
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
        bool IsIniInitMode;
        int ItemSourceCount
        {
            get
            {
                return ItemsSource != null ? ItemsSource.Cast<string>().Count() : 0;
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
        

        public ListBoxControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this) as ScreenDocument;

                    var fes = this;
                    
                    OverrideBaseProperties();

                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        bDesign = true;
                        String[] list = new String[] { Properties.Resources.Item1, Properties.Resources.Item2, Properties.Resources.Item3 };
                        ItemsSource = list;
                    }
                    else
                    {
                        InitControl();
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
                    bTagInit = true;
                    IsIniInitMode = false;
                    OnSelectedValueChanged(string.Empty, SelectedValue);
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
                typeHelper.PrepareExecution(Properties.Resources.ListSessionName, Document, this, itemsourcetag_PropertyChanged, itemsourcetag);
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
                    OnSelectedValueChanged(SelectedValue, SelectedValue);
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

        private void InitItemsSource()
        {
            if (TagItemSource == null || TagItemSource.TagReference == null)
            {
                if (ItemsSource == null || ItemsSource.Cast<string>().Count() == 0)
                {
                    IsIniInitMode = true;
                    switch (TagType)
                    {
                        case FormatEnum.Digital:
                            ItemsSource = new List<string>() { monitoredItemViewModel.NodeIdModel.FalseState.ToString(), monitoredItemViewModel.NodeIdModel.TrueState.ToString() };
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

            if (TagType != FormatEnum.String)
                SelectionMode = System.Windows.Controls.SelectionMode.Single;
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
                    else if (monitoredItemViewModel != null && monitoredItemViewModel.DataType == "Boolean")
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

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bWaitingForSelectionChanged = false;
            object parsetextvalue;

            if (!bTagInit || IsIniInitMode)
            {
                e.Handled = true;
                return;
            }

            if (IsInValueChanged)
            {
                IsInValueChanged = false;
                return;
            }

            try
            {
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        if ((sender as ListBox).SelectedIndex != -1)
                            SelectedValue = (sender as ListBox).SelectedIndex.ToString();
                        else
                        {
                            int _index = System.Convert.ToInt32(SelectedValue);

                            if (_index < 0 || _index > ItemSourceCount - 1)
                            {
                                //IsIniInitMode = true;
                                SelectedIndex = -1;
                            }
                            else
                            {
                                SelectedIndex = _index;
                            }
                        }
                        break;
                    case FormatEnum.Digital:
                    case FormatEnum.Boolean:
                        if ((sender as ListBox).SelectedIndex != -1)
                        {
                            var _value = (sender as ListBox).SelectedIndex;
                            if (_value == 0)
                            {
                                SelectedValue = Convert.ToString(false);
                            }
                            else
                            {
                                SelectedValue = Convert.ToString(true);
                                SelectedIndex = 1;
                            }
                        }
                        else
                            SelectedIndex = String.Compare(SelectedValue, "True", true) == 0 ? 1 : 0;
                        break;
                    //case FormatEnum.Digital:
                    //    if ((sender as ListBox).SelectedIndex != -1)
                    //    {
                    //        var _value = (sender as ListBox).SelectedIndex;
                    //        if (_value < 2)
                    //        {
                    //            SelectedValue = (sender as ListBox).SelectedIndex.ToString();
                    //        }
                    //    }
                    //    else if (TryParseValue(SelectedValue, out parsetextvalue))
                    //    {
                    //        (sender as ListBox).SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                    //    }
                    //    break;
                    case FormatEnum.Enumerated:
                        if ((sender as ListBox).SelectedIndex != -1)
                            SelectedValue = (sender as ListBox).SelectedIndex.ToString();
                        else if (TryParseValue(SelectedValue, out parsetextvalue))
                        {
                            (sender as ListBox).SelectedIndex = (int)parsetextvalue;
                        }
                        break;
                    case FormatEnum.String:
                        if (SelectionMode == System.Windows.Controls.SelectionMode.Single)
                            SelectedValue = (sender as ListBox).SelectedValue == null ? string.Empty : (sender as ListBox).SelectedItem.ToString();
                        else
                        {
                            StringBuilder _selectedValue = new StringBuilder(string.Empty);
                            if ((sender as ListBox).SelectedValue != null)
                            {
                                foreach (var item in (sender as ListBox).SelectedItems)
                                {
                                    _selectedValue.Append(string.Format("{0}|", item.ToString()));
                                }
                                _selectedValue.Remove(_selectedValue.Length - 1, 1);
                            }
                            SelectedValue = _selectedValue.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!bDesign)
            {
                var s = new Style(typeof(ListBoxItem));
                if (ItemContainerStyle != null)
                    foreach (var setter in ItemContainerStyle.Setters)
                        s.Setters.Add(setter);
                s.Setters.Add(new EventSetter() { Event = TouchEnterEvent, Handler = new EventHandler<TouchEventArgs>(ListBoxItem_TouchEnter) });
                ItemContainerStyle = s;
            }
        }

        private void ListBoxItem_TouchEnter(object sender, TouchEventArgs e)
        {
            if (!bDesign && IsStylusOver && SelectionMode == SelectionMode.Single) // Touch input
            {
                lastHoveredItem = sender as ListBoxItem;
                if (ItemContainerGenerator.ContainerFromItem(SelectedItem) != sender as ListBoxItem)
                    bWaitingForSelectionChanged = true;
            }
        }

        private void OnTouchLeave(object sender, TouchEventArgs e)
        {
            if (bWaitingForSelectionChanged && !bScrolled && lastHoveredItem != null && SelectionMode == SelectionMode.Single)
            {
                lastHoveredItem.Focus();
                lastHoveredItem.IsSelected = true;
            }
            bScrolled = false;
            bWaitingForSelectionChanged = false;
        }

        private void listBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!bDesign)
            {
                bScrolled = true;
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
                if(typeHelper.ChecktypeDefinition(Properties.Resources.ListSessionName, Document, this, bDesign, TagItemSource, relative, absolute, itemsourcetag_PropertyChanged, itemsourcetagMonitoredItemViewModel_PropertyChanged, ref itemsourcetag, itemsourcetagMonitoredItemViewModel))
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

    class ConvertSelectionColor : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            if (sender == null)
                return null;
            ListBoxControl listBoxControl = sender as ListBoxControl;
            //needed to apply theme border as not unset value
            listBoxControl.BorderBrush = listBoxControl.BorderBrush;
            return null;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            ListBoxControl listBoxControl = sender as ListBoxControl;
            if (document == null || !(document is IDocument) || (property as DependencyProperty) == null)
                return value;

            DependencyProperty prop = (DependencyProperty)property;
            //Brush brush = WPFUtilities.ThemeHelper.GetHilightingThemeBrush(ThemeImageHelper.GetTheme((IDocument)document));
            if (prop.Name == ListBoxControl.SelectionBackgroundColorProperty.Name)
            {
                Brush brush = WPFUtilities.ThemeHelper.GetGridHilightingThemeBrush(ThemeImageHelper.GetTheme((IDocument)document), false);
                return brush;
            }
            else if (prop.Name == ListBoxControl.SelectionForegroundColorProperty.Name)
            {
                Brush brush = WPFUtilities.ThemeHelper.GetGridHilightingThemeBrush(ThemeImageHelper.GetTheme((IDocument)document), true);
                return brush;
            }
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
