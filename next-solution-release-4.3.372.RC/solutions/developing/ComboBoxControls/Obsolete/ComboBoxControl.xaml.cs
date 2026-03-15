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
using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using PropertyControl.ComponentService;
using ScreenSettings;
using UFInterfaces.PropertyControl;
using Utilities;
using WPFUtilities.Extensions;

namespace ComboBoxControls
{
    /// <summary>
    /// Interaction logic for ComboBoxControl.xaml
    /// </summary>
    [Obsolete("Use the ComboBoxControls.ComboControl.xaml insted of this.")]
    public partial class ComboBoxControl : UserControl, IDisposable
    {
        #region DP
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnBorderBrushChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnBorderThicknessChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ComboBoxControl));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
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
                combobox.FontFamily = FontFamily;
                
                
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
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
                combobox.FontWeight = FontWeight;
                
                
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
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
                combobox.FontStyle = FontStyle;
                
                
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
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
                combobox.FontSize = FontSize;
            }
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
            if (control != null)
            {
                control.OnBorderBrushChanged();
            }
        }

        private void OnBorderBrushChanged()
        {
            combobox.BorderBrush = BorderBrush;
        }

        private void OnBorderThicknessChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
            if (control != null)
            {
                control.OnBorderThicknessChanged();
            }
        }

        private void OnBorderThicknessChanged()
        {
            combobox.BorderThickness = BorderThickness;
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as ComboBoxControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderBrushProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnBorderBrushChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BorderThicknessProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnBorderThicknessChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(ComboBoxControl));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }

        private void InitBrush()
        {
          OnBackgroundChanged();
          OnBorderBrushChanged();
          OnBorderThicknessChanged();
          OnForegroundChanged();
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            combobox.Foreground = Foreground;
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!IsManipulationEnabled)
                combobox.Background = Background;
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ComboBoxControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged), new CoerceValueCallback(OnCoerceItemsSource)));

        private static object OnCoerceItemsSource(DependencyObject o, object value)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                return control.OnCoerceItemsSource((IEnumerable)value);
            else
                return value;
        }

        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                control.OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        protected virtual IEnumerable OnCoerceItemsSource(IEnumerable value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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
                combobox.ItemsSource = list;
            }
            else
            {
                combobox.ItemsSource = newValue;
            }

        }
        [Category("ComboBoxControlOptions")]
        [Browsable(false)]
        public IEnumerable ItemsSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        int ItemSourceCount
        {
            get
            {
                return combobox.ItemsSource != null ? combobox.ItemsSource.Cast<string>().Count() : 0;
            }
        }
        #endregion

        #region SelectedValue
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(String), typeof(ComboBoxControl), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnSelectedValueChanged), new CoerceValueCallback(OnCoerceSelectedValue)));

        private static object OnCoerceSelectedValue(DependencyObject o, object value)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                return control.OnCoerceSelectedValue((String)value);
            else
                return value;
        }

        private static void OnSelectedValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxControl control = o as ComboBoxControl;
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
            try
            {
                if (IsInEditMode)
                    return;

                object parsetextvalue;
                int _index;
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
                        if (TryParseValue(newValue, out parsetextvalue))
                        {
                            combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
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
                        break;
                    case FormatEnum.Boolean:
                        combobox.SelectedIndex = String.Compare(newValue, "True", true) == 0 ? 1 : 0;
                        combobox.Text = (string)combobox.SelectedValue;
                        break;
                }

            }
            catch
            {

            }
        }

        [Category("ComboBoxControlOptions")]
        [Browsable(false)]
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


        
        //public bool IsReadOnly
        //{
        //    get
        //    {
        //        return this.combobox.IsReadOnly;
        //    }
        //    set
        //    {
        //        this.combobox.IsReadOnly = value;
        //        if ((bool)value)
        //            IsInEditMode = false;
        //    }
        //}


        #region IsEditable
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(ComboBoxControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsEditableChanged), new CoerceValueCallback(OnCoerceIsEditable)));

        private static object OnCoerceIsEditable(DependencyObject o, object value)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                return control.OnCoerceIsEditable((bool)value);
            else
                return value;
        }

        private static void OnIsEditableChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                control.OnIsEditableChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsEditable(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsEditableChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!(bool)newValue)
                IsInEditMode = false;
            combobox.IsReadOnly = !newValue;
            combobox.IsEditable = newValue;
        }
         [Category("ComboBoxControlOptions")]
        public bool IsEditable
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsEditableProperty);
            }
            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        #endregion
        
        

        #region FontSettings
        public static readonly DependencyProperty FontSettingsProperty = DependencyProperty.Register("FontSettings", typeof(FontSettings), typeof(ComboBoxControl), new UIPropertyMetadata(new FontSettings(FontWeights.Normal, FontStyles.Normal, new FontFamily("Segoe UI"), 20), new PropertyChangedCallback(OnFontSettingsChanged), new CoerceValueCallback(OnCoerceFontSettings)));

        private static object OnCoerceFontSettings(DependencyObject o, object value)
        {
            ComboBoxControl control = o as ComboBoxControl;
            if (control != null)
                return control.OnCoerceFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxControl control = o as ComboBoxControl;
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
        [Category("ComboBoxControlOptions")]
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
            combobox.FontSize = newValue.FontSize;
            combobox.FontFamily = newValue.FontFamily;
            combobox.FontWeight = newValue.FontWeight;
            combobox.FontStyle = newValue.FontStyle;

            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;
        }
        #endregion



        #region ComboBoxStyle
        public static readonly DependencyProperty ComboBoxStyleProperty = DependencyProperty.Register("ComboBoxStyle", typeof(ComboBoxStyleEnum), typeof(ComboBoxControl), new UIPropertyMetadata(ComboBoxStyleEnum.Default));

        //private static object OnCoerceComboBoxStyle(DependencyObject o, object value)
        //{
        //    ComboBoxControl control = o as ComboBoxControl;
        //    if (control != null)
        //        return control.OnCoerceComboBoxStyle((ComboBoxStyleEnum)value);
        //    else
        //        return value;
        //}

        //private static void OnComboBoxStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        //{
        //    ComboBoxControl control = o as ComboBoxControl;
        //    if (control != null)
        //        control.OnComboBoxStyleChanged((ComboBoxStyleEnum)e.OldValue, (ComboBoxStyleEnum)e.NewValue);
        //}

        //protected virtual ComboBoxStyleEnum OnCoerceComboBoxStyle(ComboBoxStyleEnum value)
        //{
        //    // TODO: Keep the proposed value within the desired range.
        //    return value;
        //}

        //protected virtual void OnComboBoxStyleChanged(ComboBoxStyleEnum oldValue, ComboBoxStyleEnum newValue)
        //{
        //    // TODO: Add your property changed side-effects. Descendants can override as well.
        //    combobox.Style = TryFindResource(string.Format("{0}_ComboBox", newValue.ToString())) as Style;
        //}
        [Category("ComboBoxControlOptions")]
        [Browsable(false)]
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
        #endregion

        #region Declarations
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

        #endregion

        #region Contructor
        bool bInit;
        
        IDocument document;

        public ComboBoxControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            combobox.DataContext = this;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    if (document == null)
                        document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    //if (propertyControl == null && document != null)
                    //    propertyControl = document.GetService(typeof(IPropertyControl)) as IPropertyControl;


                   
                    OverrideBaseProperties();
                    InitBrush();

                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        bDesign = true;
                        combobox.IsReadOnly = !IsEditable;
                        combobox.IsEditable = IsEditable;
                    }
                    else
                    {
                        combobox.IsReadOnly = !IsEditable;
                        combobox.IsEditable = IsEditable;
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
                        RemoveEventHandler(combobox);
                        AddEventHandler(combobox);
                    }
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

        #region Methods
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
            switch (TagType)
            {
                case FormatEnum.Numeric:
                    if (e == null || e.Key == Key.Enter)
                    {
                        try
                        {
                            _index = combobox.ItemsSource.Cast<string>().Select(Tuple.Create<string, int>).Where(t => string.Equals(t.Item1, (combobox as ComboBox).Text, StringComparison.CurrentCultureIgnoreCase)).Select(t => t.Item2).DefaultIfEmpty(-1).FirstOrDefault();
                            if(_index!=-1)
                                SelectedValue = _index.ToString(CultureInfo.InvariantCulture);

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
                            combobox.SelectedIndex = System.Convert.ToInt32(SelectedValue);
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
                                SelectedValue = _index.ToString(CultureInfo.InvariantCulture);
                                break;                                
                            }
                            _svalue = (combobox as ComboBox).Text;
                            if (_svalue.Equals("1"))
                                SelectedValue = Convert.ToString(true);
                            else if (_svalue.Equals("0"))
                                SelectedValue = Convert.ToString(false);
                            else
                            {
                                try
                                {
                                    _bvalue = System.Convert.ToBoolean(_svalue);
                                    SelectedValue = _bvalue.ToString(CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    combobox.SelectedIndex = String.Compare(SelectedValue, "True", true) == 0 ? 1 : 0;
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
                                SelectedValue = _index.ToString(CultureInfo.InvariantCulture);
                                break;
                            }

                            object parsetextvalue;
                            if (!TryParseValue((combobox as ComboBox).Text, out parsetextvalue))
                            {
                                if (int.TryParse((combobox as ComboBox).Text, out _enval) && _enval < EnumStringCount && _enval >= 0)
                                    SelectedValue = Convert.ToString(_enval);
                                else if (TryParseValue(SelectedValue, out parsetextvalue))
                                    {
                                        combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                                        combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                                    }
                            }
                            else
                            {
                                SelectedValue = Convert.ToString(parsetextvalue);
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
                                SelectedValue = _index.ToString(CultureInfo.InvariantCulture);
                                break;
                            }

                            object parsetextvalue;
                            if (!TryParseValue((combobox as ComboBox).Text, out parsetextvalue))
                            {
                                if (int.TryParse((combobox as ComboBox).Text, out _enval) && _enval < EnumStringCount && _enval >= 0)
                                    SelectedValue = Convert.ToString(_enval);
                                else if (TryParseValue(SelectedValue, out parsetextvalue))
                                {
                                    combobox.SelectedIndex = (int)parsetextvalue;
                                    combobox.Text = combobox.SelectedValue != null ? combobox.SelectedValue.ToString() : string.Empty;
                                }
                            }
                            else
                            {
                                SelectedValue = Convert.ToString(parsetextvalue);
                            }
                        }
                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                case FormatEnum.String:
                    if (e == null || e.Key == Key.Enter)
                    {
                        SelectedValue = (combobox as ComboBox).Text;

                        IsInEditMode = false;
                        updateSource = false;
                    }
                    break;
                default:
                    break;
            }
        }
        bool IsIniInitMode;
        private void InitItemsSource()
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
            object parsetextvalue;
            int _value;
            if (IsIniInitMode)
            {
                IsIniInitMode = false;
                return;
            }
            if (IsInEditMode)
                return;
            try
            {
                switch (TagType)
                {
                    case FormatEnum.Numeric:
                        SelectedValue = (sender as ComboBox).SelectedIndex.ToString();
                        break;
                    case FormatEnum.Boolean:
                        _value = (sender as ComboBox).SelectedIndex;
                        if (_value == 0)
                        {
                            SelectedValue = Convert.ToString(false);
                        }
                        else
                        {
                            SelectedValue = Convert.ToString(true);
                            combobox.SelectedIndex = 1;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        break;
                    case FormatEnum.Digital:
                        _value = (sender as ComboBox).SelectedIndex;
                        if (_value < 2)
                        {
                            SelectedValue = (sender as ComboBox).SelectedIndex.ToString();
                        }
                        else if (TryParseValue(SelectedValue, out parsetextvalue))
                        {
                            combobox.SelectedIndex = (bool)parsetextvalue ? 1 : 0;
                            combobox.Text = (string)combobox.SelectedValue;
                        }
                        break;
                    case FormatEnum.Enumerated:
                        SelectedValue = (sender as ComboBox).SelectedIndex.ToString();
                        break;
                    case FormatEnum.String:
                        SelectedValue = (sender as ComboBox).SelectedValue != null ? (sender as ComboBox).SelectedValue.ToString() : string.Empty;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
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

            if (monitoredItemViewModel != null && monitoredItemViewModel.NodeIdModel != null)
                monitoredItemViewModel.NodeIdModel.PropertyChanged -= NodeIdViewModel_PropertyChanged;

            RemoveEventHandler(combobox);
            DetachOverrideBaseProperties();
        }
        #endregion


    }
}
