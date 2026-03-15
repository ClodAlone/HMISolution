using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using EditDisplay.Converters;
using OPCUAViewModel;
using WPFUtilities;
using WPFUtilities.Extensions;
using Utilities;
using Utilities.WPF;
using System.Windows.Automation.Peers;
using EditDisplay.Automations;
using StringManager.ComponentService;
using DocumentManager.ComponentService;
using System.Globalization;

namespace EditDisplay
{
    /// <summary>
    /// Interaction logic for DateEditDisplay.xaml
    /// </summary>
    public partial class DateEditDisplay : UserControl, IDisposable
    {
        #region DP

        #region OverrideBaseProperties
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(HorizontalContentAlignmentProperty, typeof(DateEditDisplay));
            dpd.AddValueChangedSafe(this, OnHorizontalContentAlignmentChanged);

            OnHorizontalContentAlignmentChanged();

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DateEditDisplay));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);

            OnBackgroundChanged();

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DateEditDisplay));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);

            OnForegroundChanged();

            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(DateEditDisplay));
            dpd.AddValueChangedSafe(this, OnIsEnabledChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(HorizontalContentAlignmentProperty, typeof(DateEditDisplay));
            dpd.RemoveValueChangedSafe(this, OnHorizontalContentAlignmentChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(DateEditDisplay));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(DateEditDisplay));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(DateEditDisplay));
            dpd.RemoveValueChangedSafe(this, OnIsEnabledChanged);
        }

        private void OnIsEnabledChanged(object sender, EventArgs e)
        {
            var control = sender as DateEditDisplay;
            if (control != null)
            {
                control.OnIsEnabledChanged();
            }
        }
        protected virtual void OnIsEnabledChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            grid.Opacity = IsEnabled ? 1.0 : 0.7;
        }
        private void OnHorizontalContentAlignmentChanged(object sender, EventArgs e)
        {
            var control = sender as DateEditDisplay;
            if (control != null)
            {
                control.OnHorizontalContentAlignmentChanged();
            }
        }
        protected virtual void OnHorizontalContentAlignmentChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            display.HorizontalContentAlignment = HorizontalContentAlignment;
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as DateEditDisplay;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            if (!IsManipulationEnabled)
            {
                display.Background = Background;
                /* 
                 * https://www.devexpress.com/Support/Center/Question/Details/Q574016
                 * "Our editors don't support transparent background"
                 */
                (from c in (display as UIElement).GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
                 select c).ToList().ForEach(child =>
                 {
                     child.Background = Background;
                 });
            }
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as DateEditDisplay;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            display.Foreground = Foreground;
            (from c in (display as UIElement).GetVisualChildrenOfType<TextBox>()
             select c).ToList().ForEach(child =>
             {
                 child.Foreground = Foreground;
             });
        }
        #endregion
        #region NullText
        public static readonly DependencyProperty NullTextProperty = DependencyProperty.Register("NullText", typeof(string), typeof(DateEditDisplay), new UIPropertyMetadata("MM/dd/yyyy hh:mm:ss", new PropertyChangedCallback(OnNullTextChanged), new CoerceValueCallback(OnCoerceNullText)));

        private static object OnCoerceNullText(DependencyObject o, object value)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                return control.OnCoerceNullText((string)value);
            else
                return value;
        }

        private static void OnNullTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                control.OnNullTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceNullText(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnNullTextChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string NullText
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(NullTextProperty);
            }
            set
            {
                SetValue(NullTextProperty, value);
            }
        }

        #endregion

        #region DisplayFormatString
        public static readonly DependencyProperty DisplayFormatStringProperty = DependencyProperty.Register("DisplayFormatString", typeof(string), typeof(DateEditDisplay), new UIPropertyMetadata("g", new PropertyChangedCallback(OnDisplayFormatStringChanged), new CoerceValueCallback(OnCoerceDisplayFormatString)));

        private static object OnCoerceDisplayFormatString(DependencyObject o, object value)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                return control.OnCoerceDisplayFormatString((string)value);
            else
                return value;
        }

        private static void OnDisplayFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                control.OnDisplayFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDisplayFormatString(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDisplayFormatStringChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string DisplayFormatString
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DisplayFormatStringProperty);
            }
            set
            {
                SetValue(DisplayFormatStringProperty, value);
            }
        }

        #endregion

        #region ConvertFormatString
        public static readonly DependencyProperty ConvertFormatStringProperty = DependencyProperty.Register("ConvertFormatString", typeof(string), typeof(DateEditDisplay), new UIPropertyMetadata(null, new PropertyChangedCallback(OnConvertFormatStringChanged), new CoerceValueCallback(OnCoerceConvertFormatString)));

        private static object OnCoerceConvertFormatString(DependencyObject o, object value)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                return control.OnCoerceConvertFormatString((string)value);
            else
                return value;
        }

        private static void OnConvertFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                control.OnConvertFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceConvertFormatString(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnConvertFormatStringChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string ConvertFormatString
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ConvertFormatStringProperty);
            }
            set
            {
                SetValue(ConvertFormatStringProperty, value);
            }
        }

        #endregion

        #region EditValue
        public static readonly DependencyProperty EditValueProperty = DependencyProperty.Register("EditValue", typeof(DateTime), typeof(DateEditDisplay), new UIPropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnEditValueChanged), new CoerceValueCallback(OnCoerceEditValue)));

        private static object OnCoerceEditValue(DependencyObject o, object value)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                return control.OnCoerceEditValue((DateTime)value);
            else
                return value;
        }

        private static void OnEditValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                control.OnEditValueChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual DateTime OnCoerceEditValue(DateTime value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEditValueChanged(DateTime oldValue, DateTime newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            display.EditValue = newValue;
        }

        public DateTime EditValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DateTime)GetValue(EditValueProperty);
            }
            set
            {
                SetValue(EditValueProperty, value);
            }
        }

        #endregion


        #region IsReadOnly
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(Boolean), typeof(DateEditDisplay), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged), new CoerceValueCallback(OnCoerceIsReadOnly)));

        private static object OnCoerceIsReadOnly(DependencyObject o, object value)
        {
            DateEditDisplay control = o as DateEditDisplay;
            if (control != null)
                return control.OnCoerceIsReadOnly((Boolean)value);
            else
                return value;
        }

        private static void OnIsReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DateEditDisplay control = o as DateEditDisplay;
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
                if (!bDesign && runningOnServer)
                {
                    this.IsHitTestVisible = !newValue;
                }
                display.IsHitTestVisible = !newValue;
            }
        }
        [Category("DateEditDisplayOptions")]
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


        #region ShowBorder
        public static readonly DependencyProperty ShowBorderProperty = DependencyProperty.Register("ShowBorder", typeof(bool), typeof(DateEditDisplay), new UIPropertyMetadata(true));
        public bool ShowBorder
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowBorderProperty);
            }
            set
            {
                SetValue(ShowBorderProperty, value);
            }
        }
        #endregion

        #endregion
        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        public string GetDisplayValue()
        {
            return display.DisplayText; //EditValue.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }
        public void SetDisplayValue(string newValue)
        {
            DateTime? d = ParseCalendarDisplayValue(newValue);
            if (d != null)
                AutomationPeerUpdateValue((DateTime)d);
        }

        DateTime? ParseCalendarDisplayValue(string newValue)
        {
            DateTime d;
            if (DateTime.TryParse(newValue, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out d))
                return d;
            return null;
        }

        void AutomationPeerUpdateValue(DateTime d)
        {
            BindingExpression textBinding = BindingOperations.GetBindingExpression(
                    this, DateEditDisplay.EditValueProperty);
            if (textBinding != null)
            {
                EditValue = d;
                textBinding.UpdateSource();
            }
        }

        #endregion
        private void display_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try
            {
                if (bInit && display.EditValue != null)
                {
                    if (runningOnServer && display.EditValue is string)
                        SetDisplayValue((string)display.EditValue);
                    else
                    {
                        if (runningOnServer)
                            AutomationPeerUpdateValue((DateTime)display.EditValue);
                        else
                            EditValue = (DateTime)display.EditValue;
                    }
                }
            }
            catch
            {
            }
        }


        [Browsable(false)]
        public bool runningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        internal IDocument Document;
        IStringEditorManager stringManager;
        bool bLoaded;
        bool bInit;
        bool bDesign;
        private DelayedSingleActionInvoker SizeChangedInvoker;
        public DateEditDisplay()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                        bDesign = true;
                    //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    //{
                    //    Visibility = System.Windows.Visibility.Visible;
                    //});

                    display.Width = ActualWidth;
                    display.Height = ActualHeight;
                    display.EditValue = EditValue;

                   
                    SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                    {
                        if (!bDisposed)
                        {
                            display.Width = ActualWidth;
                            display.Height = ActualHeight;
                        }
                    }, new TimeSpan(0, 0, 0, 0, 100));
                    SizeChanged += (ob, ev) =>
                    {
                        if (SizeChangedInvoker != null)
                            SizeChangedInvoker.BeginInvoke();
                    };
                    OverrideBaseProperties();
                    if (runningOnServer)
                        IsHitTestVisible = display.IsHitTestVisible = IsHitTestVisible && !IsReadOnly;
                    else
                        display.IsHitTestVisible = !IsReadOnly;

                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
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

                    bInit = true;
                }
            };

            DataContextChanged += (o, e) =>
            {
                if (bDisposed)
                    return;

                if (DataContext is MonitoredItemViewModel && !DesignerProperties.GetIsInDesignMode(this))
                {

                    if (monitoredItemViewModel != null)
                        monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                    monitoredItemViewModel = DataContext as MonitoredItemViewModel;
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                }
            };
        }

        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            if (bDesign || stringManager == null)
                return;
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                display.MaskCulture = CultureInfo.CurrentCulture;
            });
        }
        private void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataValue" || e.PropertyName == "Quality")
            {
                if (monitoredItemViewModel.DataType != null)
                {
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
                    if (!monitoredItemViewModel.DataType.Equals("String", StringComparison.InvariantCultureIgnoreCase) &&
                    !monitoredItemViewModel.DataType.Equals("UtcTime", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var converter = new DateTimeStringConverter();
                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            var bindingValue = new Binding()
                            {
                                Path = new PropertyPath("Value"),
                                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                                Mode = BindingMode.TwoWay,
                                ValidatesOnDataErrors = true,
                                Converter = converter,
                                ConverterParameter = ConvertFormatString
                            };
                            display.SetBinding(DevExpress.Xpf.Editors.TextEdit.EditValueProperty, bindingValue);
                        });
                    }
                }
            }
        }

        bool bDisposed;

        protected MonitoredItemViewModel monitoredItemViewModel;
      

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            SizeChangedInvoker = null;
            DetachOverrideBaseProperties();
            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
        }
    }
}
