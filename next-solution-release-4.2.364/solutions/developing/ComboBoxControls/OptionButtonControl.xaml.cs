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
using System.Xml.Serialization;
using ComboBoxControls.Enums;
using Converters;
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using ScreenSettings;
using StringManager.ComponentService;
using UFInterfaces.PropertyControl;
using Utilities;
using Utilities.WPF;
using WPFUtilities.Extensions;

namespace ComboBoxControls
{
    /// <summary>
    /// Interaction logic for OptionButtonControl.xaml
    /// </summary>
    public partial class OptionButtonControl : ListBox, IContainPropertyEditors, IDisposable, IStringIDAware
    {
        #region DP
        #region DP Overrides

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(OptionButtonControl));
            dpd.AddValueChangedSafe(this, OnItemsSourceChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(OptionButtonControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(OptionButtonControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(OptionButtonControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(OptionButtonControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(OptionButtonControl));
            dpd.RemoveValueChangedSafe(this, OnItemsSourceChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(OptionButtonControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(OptionButtonControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(OptionButtonControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(OptionButtonControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as OptionButtonControl;
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
            var control = sender as OptionButtonControl;
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
            var control = sender as OptionButtonControl;
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
            var control = sender as OptionButtonControl;
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
            var control = sender as OptionButtonControl;
            if (control != null)
            {
                control.OnItemsSourceChanged();
            }
        }

        protected virtual void OnItemsSourceChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (UnderlineText)
                UpdateUnderlineText();
        }
        #endregion



        #region UnderlineText
        public static readonly DependencyProperty UnderlineTextProperty = DependencyProperty.Register("UnderlineText", typeof(bool), typeof(OptionButtonControl), new UIPropertyMetadata(false));

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
        


        #region OptionList
        public static readonly DependencyProperty OptionListProperty = DependencyProperty.Register("OptionList", typeof(OptionItemList), typeof(OptionButtonControl), new UIPropertyMetadata(new OptionItemList(), new PropertyChangedCallback(OnOptionListChanged), new CoerceValueCallback(OnCoerceOptionList)));

        private static object OnCoerceOptionList(DependencyObject o, object value)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                return control.OnCoerceOptionList((OptionItemList)value);
            else
                return value;
        }

        private static void OnOptionListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                control.OnOptionListChanged((OptionItemList)e.OldValue, (OptionItemList)e.NewValue);
        }

        protected virtual OptionItemList OnCoerceOptionList(OptionItemList value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOptionListChanged(OptionItemList oldValue, OptionItemList newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!DesignerProperties.GetIsInDesignMode(this) && !bDesign)
            {
                ItemsSource = newValue;
            }
            else
            {
                if (bDInit)
                {
                    if (newValue != null && newValue.Count() > 0)
                    {
                        if (TranslationHelpers.TranslationHelper.CanTranslate(stringlist))
                            (from OptionItem x in newValue.AsParallel() where TranslationHelpers.TranslationHelper.CanBeTranslatedEvenIfComposed(x.OptionContent, stringlist) select x).ToList().ForEach(x => {
                                x.OptionContent = TranslationHelpers.TranslationHelper.TranslateComposedText(x.OptionContent, stringlist, x.OptionContent);
                            });
                         ItemsSource = newValue;
                    }
                    else
                        ItemsSource = new OptionItemList() { new OptionItem() { OptionContent = Properties.Resources.Option1, OptionValue = "0" } };

                    //SelectedIndex = 0;
                }
            }
        }
        [Category("Advanced")]
        [Browsable(false)]
        public OptionItemList OptionList
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (OptionItemList)GetValue(OptionListProperty);
            }
            set
            {
                SetValue(OptionListProperty, value);
            }
        }

        #endregion

        #region SelectedValue
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(String), typeof(OptionButtonControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSelectedValueChanged), new CoerceValueCallback(OnCoerceSelectedValue)));

        private static object OnCoerceSelectedValue(DependencyObject o, object value)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                return control.OnCoerceSelectedValue((String)value);
            else
                return value;
        }

        private static void OnSelectedValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OptionButtonControl control = o as OptionButtonControl;
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
                OptionItem option = (from c in OptionList where (c as OptionItem).OptionValue == newValue select c).FirstOrDefault();
                if (option == null)
                    SelectedIndex = -1;
                else
                {
                    int _index = OptionList.IndexOf(option);
                    IsIniInitMode = true;
                    SelectedIndex = _index;
                    IsIniInitMode = false;
                }
            }
            catch
            {

            }
        }

        [Category("OptionButtonControlOptions")]
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


        #region OptionButtonStyle
        public static readonly DependencyProperty OptionButtonStyleProperty = DependencyProperty.Register("OptionButtonStyle", typeof(ComboBoxStyleEnum), typeof(OptionButtonControl), new UIPropertyMetadata(ComboBoxStyleEnum.Default));

        //private static object OnCoerceOptionButtonStyle(DependencyObject o, object value)
        //{
        //    OptionButtonControl control = o as OptionButtonControl;
        //    if (control != null)
        //        return control.OnCoerceOptionButtonStyle((ComboBoxStyleEnum)value);
        //    else
        //        return value;
        //}

        //private static void OnOptionButtonStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    OptionButtonControl control = o as OptionButtonControl;
        //    if (control != null)
        //        control.OnOptionButtonStyleChanged((ComboBoxStyleEnum)e.OldValue, (ComboBoxStyleEnum)e.NewValue);
        //}

        //protected virtual ComboBoxStyleEnum OnCoerceOptionButtonStyle(ComboBoxStyleEnum value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnOptionButtonStyleChanged(ComboBoxStyleEnum oldValue, ComboBoxStyleEnum newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    if (bDInit)
        //        InitOptionListStyle();
        //}
        [Category("OptionButtonControlOptions")]
        [Browsable(false)]
        [XmlIgnore]
        [Obsolete("No longer used.")]
        public ComboBoxStyleEnum OptionButtonStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ComboBoxStyleEnum)GetValue(OptionButtonStyleProperty);
            }
            set
            {
                SetValue(OptionButtonStyleProperty, value);
            }
        }

        #endregion


        #region OptionStyle
        public static readonly DependencyProperty OptionStyleProperty = DependencyProperty.Register("OptionStyle", typeof(string), typeof(OptionButtonControl), new UIPropertyMetadata("Option_ItemStyle"));

        [Browsable(false)]
        [XmlIgnore]
        public string OptionStyle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(OptionStyleProperty);
            }
            set
            {
                SetValue(OptionStyleProperty, value);
            }
        }
        #endregion


        #region OptionBorderBrush
        public static readonly DependencyProperty OptionBorderBrushProperty = DependencyProperty.Register("OptionBorderBrush", typeof(Brush), typeof(OptionButtonControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromRgb(0,201,255))));
        public Brush OptionBorderBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(OptionBorderBrushProperty);
            }
            set
            {
                SetValue(OptionBorderBrushProperty, value);
            }
        }
        #endregion


        #region OptionBorderThickness
        public static readonly DependencyProperty OptionBorderThicknessProperty = DependencyProperty.Register("OptionBorderThickness", typeof(Thickness), typeof(OptionButtonControl), new UIPropertyMetadata(new Thickness(1)));


        public Thickness OptionBorderThickness
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Thickness)GetValue(OptionBorderThicknessProperty);
            }
            set
            {
                SetValue(OptionBorderThicknessProperty, value);
            }
        }

        #endregion



        #region BulletDimension
        public static readonly DependencyProperty BulletDimensionProperty = DependencyProperty.Register("BulletDimension", typeof(double), typeof(OptionButtonControl), new UIPropertyMetadata(25.0, new PropertyChangedCallback(OnBulletDimensionChanged), new CoerceValueCallback(OnCoerceBulletDimension)));

        private static object OnCoerceBulletDimension(DependencyObject o, object value)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                return control.OnCoerceBulletDimension((double)value);
            else
                return value;
        }

        private static void OnBulletDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                control.OnBulletDimensionChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceBulletDimension(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBulletDimensionChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("OptionButtonControlOptions")]
        public double BulletDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(BulletDimensionProperty);
            }
            set
            {
                SetValue(BulletDimensionProperty, value);
            }
        }

        #endregion


        #region OptionItemDimension
        public static readonly DependencyProperty OptionItemDimensionProperty = DependencyProperty.Register("OptionItemDimension", typeof(double), typeof(OptionButtonControl), new UIPropertyMetadata(30.0, new PropertyChangedCallback(OnOptionItemDimensionChanged), new CoerceValueCallback(OnCoerceOptionItemDimension)));

        private static object OnCoerceOptionItemDimension(DependencyObject o, object value)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                return control.OnCoerceOptionItemDimension((double)value);
            else
                return value;
        }

        private static void OnOptionItemDimensionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                control.OnOptionItemDimensionChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceOptionItemDimension(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnOptionItemDimensionChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        [Category("OptionButtonControlOptions")]
        public double OptionItemDimension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(OptionItemDimensionProperty);
            }
            set
            {
                SetValue(OptionItemDimensionProperty, value);
            }
        }

        #endregion


        #region FontSettings
        public static readonly DependencyProperty FontSettingsProperty = DependencyProperty.Register("FontSettings", typeof(FontSettings), typeof(OptionButtonControl), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 20), new PropertyChangedCallback(OnFontSettingsChanged), new CoerceValueCallback(OnCoerceFontSettings)));

        private static object OnCoerceFontSettings(DependencyObject o, object value)
        {
            OptionButtonControl control = o as OptionButtonControl;
            if (control != null)
                return control.OnCoerceFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OptionButtonControl control = o as OptionButtonControl;
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
        [Category("OptionButtonControlOptions")]
        [Browsable(false)]
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

        [Browsable(false)]
        [XmlIgnore]
        public SelectionMode SelectionMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SelectionMode)GetValue(SelectionModeProperty);
            }
            set
            {
                SetValue(SelectionModeProperty, SelectionMode.Single);
            }
        }

        [Browsable(false)]
        public UserControl SmartControl
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return new Controls.SmartControl(this);
            }
        }

        public static readonly DependencyProperty SmartPropertiesProperty = DependencyProperty.Register("SmartProperties", typeof(bool), typeof(OptionButtonControl), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SmartProperties
        {
            get
            {
                return (bool)GetValue(SmartPropertiesProperty);
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

        #endregion

        #region Declarations
        bool bLoaded;
        bool bDesign;
        bool bTagInit;
        int EnumStringCount;
        MonitoredItemViewModel monitoredItemViewModel;
        IStringEditorManager stringeditorManager;
        IDictionary<String, String> stringlist;
        
        IDocument document;
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
                return ItemsSource != null ? ItemsSource.Cast<OptionItem>().Count() : 0;
            }
        }
        bool bInit;
        bool bDInit;
        public OptionItemList _OptionList;
        
        #endregion

        #region Contructor
        public OptionButtonControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;
                    
                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                    var fes = this;
                    
                    OverrideBaseProperties();

                    bDesign = DesignerProperties.GetIsInDesignMode(this) || bDesign;

                    if (document != null)
                    {
                        if (stringeditorManager == null)
                            stringeditorManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringeditorManager != null)
                        {
                            StringManager_CultureChanged(null, null);
                            stringeditorManager.CultureChanged += StringManager_CultureChanged;
                        }
                    }

                    if (bDesign)
                    {
                        if (OptionList != null && OptionList.Count() > 0)
                            ItemsSource = OptionList;// GetOptionList(OptionList);
                        else
                            ItemsSource = new OptionItemList() { new OptionItem() { OptionContent = Properties.Resources.Option1, OptionValue = "0" } };

                        //SelectedIndex = 0;
                        
                        bDInit = true;
                    }
                    else
                    {
                        InitOptions();
                        ItemsSource = _OptionList; // GetOptionList(OptionList);
                        //SelectedIndex = 0;
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

                    OptionList.Clear();
                    InitItemsSource();
                });
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            UpdateValueFont(FontSettings);
            ItemContainerStyle = TryFindResource(OptionStyle) as Style;
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
            if (OptionList.Count ==0)
            {
                try
                {
                    IsIniInitMode = true;
                    switch (TagType)
                    {
                        case FormatEnum.Boolean:
                            OptionList.Add(new OptionItem() { OptionContent = bool.FalseString, OptionValue = "False" });
                            OptionList.Add(new OptionItem() { OptionContent = bool.TrueString, OptionValue = "True" });
                            break;
                        case FormatEnum.Digital:
                            OptionList.Add(new OptionItem() { OptionContent = monitoredItemViewModel.NodeIdModel.FalseState.ToString(), OptionValue = "False" });
                            OptionList.Add(new OptionItem() { OptionContent = monitoredItemViewModel.NodeIdModel.TrueState.ToString(), OptionValue = "True" });
                            break;
                        case FormatEnum.Enumerated:
                            int i = 0;
                            (from c in monitoredItemViewModel.NodeIdModel.EnumStrings select c.ToString()).ToList().ForEach(x =>
                                {
                                    OptionList.Add(new OptionItem() { OptionContent = x, OptionValue = i.ToString() });
                                    i++;
                                });
                            break;
                    }
                }
                finally
                {
                    IsIniInitMode = false;
                }
            }

            InitOptions();
            ItemsSource = _OptionList;
            //ItemsSource = GetOptionList(OptionList);
        }

        private void InitOptions()
        {
            if (_OptionList == null)
                _OptionList = new OptionItemList();
            
            _OptionList.Clear();

            OptionList.ToList().ForEach(x =>
            {
                if (TranslationHelpers.TranslationHelper.CanBeTranslatedEvenIfComposed(x.OptionContent, stringlist))
                    _OptionList.Add(new OptionItem() { OptionContent = TranslationHelpers.TranslationHelper.TranslateComposedText(x.OptionContent, stringlist, x.OptionContent), OptionValue = x.OptionValue });
                else
                    _OptionList.Add(x);
            });
        }

        private void UpdateOptions(bool untranslated = false)
        {
            if (bDesign)
            {
                if (OptionList == null)
                    return;

                for (int i = 0; i < OptionList.Count; i++)
                {
                    if (!untranslated)
                        OptionList[i].OptionContent = TranslationHelpers.TranslationHelper.TranslateComposedText(OptionList[i].UntranslatedOptionContent, stringlist, OptionList[i].UntranslatedOptionContent);
                }
            }
            else
            {
                if (OptionList == null || _OptionList == null)
                    return;

                for (int i = 0; i < OptionList.Count && i < _OptionList.Count; i++)
                {
                    _OptionList[i].OptionContent = TranslationHelpers.TranslationHelper.TranslateComposedText(OptionList[i].OptionContent, stringlist, OptionList[i].OptionContent);
                }
            }   
        }

        private OptionItemList GetOptionList(OptionItemList OptionList)
        {
            if (!TranslationHelpers.TranslationHelper.CanTranslate(stringlist) || DesignerProperties.GetIsInDesignMode(this) || bDesign)
                return OptionList;

            OptionItemList _optionlist = new OptionItemList();
            OptionList.ToList().ForEach(x =>
                {
                    if (TranslationHelpers.TranslationHelper.CanBeTranslatedEvenIfComposed(x.OptionContent, stringlist))
                        _optionlist.Add(new OptionItem(){OptionContent = TranslationHelpers.TranslationHelper.TranslateComposedText(x.OptionContent, stringlist, x.OptionContent), OptionValue = x.OptionValue});
                    else
                        _optionlist.Add(x);
                });
             return _optionlist;
        }

        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                bool bUntranslated = bDesign && stringeditorManager.GetActiveCulture(document, false) == String.Empty;
                if (bUntranslated)
                    stringlist = null;
                else if (stringeditorManager != null && document != null)
                    stringlist = stringeditorManager.GetListStringForCulture(document, stringeditorManager.GetActiveCulture(document));

                UpdateOptions(bUntranslated);
            });
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
        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                if (bDisposed)
                    return;
                CheckRadioButtons(e.RemovedItems, false);
                CheckRadioButtons(e.AddedItems, true);
            });

            
            if (bDesign)
                return;

            if (!bTagInit || IsIniInitMode)
            {
                e.Handled = true;
                return;
            }
            try
            {
                if ((sender as ListBox).SelectedIndex != -1)
                    SelectedValue = ((sender as ListBox).SelectedItem as OptionItem).OptionValue;
                else
                {
                    OptionItem option = (from c in OptionList where (c as OptionItem).OptionValue == SelectedValue select c).FirstOrDefault();
                    if (option == null)
                        SelectedIndex = -1;
                    else
                    {
                        int _index = OptionList.IndexOf(option);
                        SelectedIndex = _index;
                    }
                }
            }
            catch
            {

            }
        }
        private void CheckRadioButtons(System.Collections.IList radioButtons, bool isChecked)
        {
            foreach (object item in radioButtons)
            {
                ListBoxItem lbi = ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                if (lbi != null)
                {
                    RadioButton radio = lbi.Template.FindName("radio", lbi) as RadioButton;
                    if (radio != null)
                        radio.IsChecked = isChecked;
                }
            }
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

                if (document == null)
                    document = ScreenSettings.ScreenDocument.GetScreenDocument(this);

                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ComboBoxControls.Controls.SmartPropertiesEditor));
                factory.SetValue(ComboBoxControls.Controls.SmartPropertiesEditor.DocumentProperty, document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SmartPropertiesProperty, dt);

                return mapDataTemplates;
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

            if (stringeditorManager != null)
                stringeditorManager.CultureChanged -= StringManager_CultureChanged;

            if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null)
                monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;

            if (_OptionList != null)
                _OptionList.Clear();
            _OptionList = null;
        }
        #endregion

        #region IStringIDAware
        public List<string> GetStringIDs()
        {
            var list = new List<string>();
            list = (from o in OptionList select o.OptionContent).ToList();
            return list;
        }
        public Dictionary<string, string> GetPropertyToStringIDMap()
        {
            var map = new Dictionary<string, string>();
            for (int i = 0; i < OptionList.Count; i++)
            {
                map.Add(string.Format(Properties.Resources.OptionContentProperty, i), OptionList[i].OptionContent);
            }
            return map;
        }
        #endregion

        private void radio_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem sel = (e.Source as RadioButton).TemplatedParent as ListBoxItem;
            int newIndex = ItemContainerGenerator.IndexFromContainer(sel);
            SelectedIndex = newIndex;
        }

        public override void OnApplyTemplate()
        {

            if (!bDesign && bLoaded)
            {
                // Set selected index to triggger SelectionChanged event.
                SelectedIndex = -1;
                OnSelectedValueChanged(string.Empty, SelectedValue);
            }
        }
    }
}
