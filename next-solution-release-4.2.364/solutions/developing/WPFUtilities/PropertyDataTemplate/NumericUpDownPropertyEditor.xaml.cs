using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using UFInterfaces;
using Utilities.WPF;
using WPFUtilities.Converters;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for NumericUpDownPropertyEditor.xaml
    /// </summary>
    public partial class NumericUpDownPropertyEditor : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(null));

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }
        #endregion

        #region ValidationNames
        public static readonly DependencyProperty ValidationNamesProperty = DependencyProperty.Register("ValidationNames", typeof(string[]), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(null));

        public string[] ValidationNames
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string[])GetValue(ValidationNamesProperty);
            }
            set
            {
                SetValue(ValidationNamesProperty, value);
            }
        }
        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(-100.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                updown.OnMinValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                updown.OnMaxValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMaxValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
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

        #region SpinValue
        public static readonly DependencyProperty SpinValueProperty = DependencyProperty.Register("SpinValue", typeof(double), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnSpinValueChanged), new CoerceValueCallback(OnCoerceSpinValue)));

        private static object OnCoerceSpinValue(DependencyObject o, object value)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceSpinValue((double)value);
            else
                return value;
        }

        private static void OnSpinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDownPropertyEditor updown = o as NumericUpDownPropertyEditor;
            if (updown != null)
                updown.OnSpinValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceSpinValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpinValueChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double SpinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(SpinValueProperty);
            }
            set
            {
                SetValue(SpinValueProperty, value);
            }
        }
        #endregion

        #region DisplayFormatString
        public static readonly DependencyProperty DisplayFormatStringProperty = DependencyProperty.Register("DisplayFormatString", typeof(string), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata("0.##", new PropertyChangedCallback(OnDisplayFormatStringChanged), new CoerceValueCallback(OnCoerceDisplayFormatString)));

        private static object OnCoerceDisplayFormatString(DependencyObject o, object value)
        {
            NumericUpDownPropertyEditor control = o as NumericUpDownPropertyEditor;
            if (control != null)
                return control.OnCoerceDisplayFormatString((string)value);
            else
                return value;
        }

        private static void OnDisplayFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDownPropertyEditor control = o as NumericUpDownPropertyEditor;
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

        #region BytesMode
        public static readonly DependencyProperty BytesModeProperty = DependencyProperty.Register("BytesMode", typeof(bool), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(false, new PropertyChangedCallback(OnBytesModeChanged), new CoerceValueCallback(OnCoerceBytesMode)));

        private static object OnCoerceBytesMode(DependencyObject o, object value)
        {
            NumericUpDownPropertyEditor NumericUpDownPropertyEditor = o as NumericUpDownPropertyEditor;
            if (NumericUpDownPropertyEditor != null)
                return NumericUpDownPropertyEditor.OnCoerceBytesMode((bool)value);
            else
                return value;
        }

        private static void OnBytesModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDownPropertyEditor NumericUpDownPropertyEditor = o as NumericUpDownPropertyEditor;
            if (NumericUpDownPropertyEditor != null)
                NumericUpDownPropertyEditor.OnBytesModeChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceBytesMode(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnBytesModeChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //updown.EditValue = new Binding() {
            //    Path = new PropertyPath("Value"),
            //    Mode = BindingMode.TwoWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //    ValidatesOnDataErrors = true,
            //    Converter = newValue ? FindResource("BytesSizeConverter") as IValueConverter : FindResource("SpinEditDoubleConverter") as IValueConverter 
            //};
        }

        public bool BytesMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(BytesModeProperty);
            }
            set
            {
                SetValue(BytesModeProperty, value);
            }
        }
        #endregion

        #region ValueConverter
        public static readonly DependencyProperty ValueConverterProperty = DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(null));

        public IValueConverter ValueConverter
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IValueConverter)GetValue(ValueConverterProperty);
            }
            set
            {
                SetValue(ValueConverterProperty, value);
            }
        }
        #endregion

        #region MaskUseAsDisplayFormat
        public static readonly DependencyProperty MaskUseAsDisplayFormatProperty = DependencyProperty.Register("MaskUseAsDisplayFormat", typeof(bool), typeof(NumericUpDownPropertyEditor), new UIPropertyMetadata(true));
        public bool MaskUseAsDisplayFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(MaskUseAsDisplayFormatProperty);
            }
            set
            {
                SetValue(MaskUseAsDisplayFormatProperty, value);
            }
        }

        #endregion

        #endregion

        #region Declarations
        DependencyObject depObject;
        DependencyPropertyDescriptor dpd;
        object initialValue;
        bool bLoaded;
        #endregion

        #region Constructors
        public NumericUpDownPropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    if (BytesMode)
                    {
                        updown.Mask = "#########.0";
                        updown.MaskUseAsDisplayFormat = true;
                        updown.EditValueType = typeof(Decimal);
                    }
                    else if(MaskUseAsDisplayFormat)
                        updown.MaskUseAsDisplayFormat = true;

                    if (ValueConverter != null)
                    {
                        BindingOperations.ClearBinding(updown, DevExpress.Xpf.Editors.BaseEdit.EditValueProperty);
                        updown.SetBinding(DevExpress.Xpf.Editors.BaseEdit.EditValueProperty, new Binding()
                        {
                            Path = new PropertyPath("Value"),
                            Mode = BindingMode.TwoWay,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            ValidatesOnDataErrors = true,
                            Converter = ValueConverter
                        });
                    }

                    if (ValidationNames != null)
                    {
                        var propertyNames = new List<string>(ValidationNames);
                        var notifyObject = GetContextObject() as INotifyPropertyChanged;
                        if (notifyObject != null)
                        {
                            bool bUpdatingSource = false;
                            notifyObject.PropertyChanged += (s1, e1) =>
                            {
                                if (!bUpdatingSource && !string.IsNullOrEmpty(e1.PropertyName) && propertyNames.Contains(e1.PropertyName))
                                {
                                    try
                                    {
                                        bUpdatingSource = true;
                                        var multiExpr = BindingOperations.GetMultiBindingExpression(updown, DevExpress.Xpf.Editors.SpinEdit.EditValueProperty);
                                        if (multiExpr != null)
                                            multiExpr.UpdateSource();
                                        else
                                        {
                                            var expr = updown.GetBindingExpression(DevExpress.Xpf.Editors.BaseEdit.EditValueProperty);
                                            if (expr != null)
                                                expr.UpdateSource();
                                        }
                                    }
                                    finally
                                    {
                                        bUpdatingSource = false;
                                    }
                                }
                            };
                        }
                    }
                }
            };

            DataContextChanged += (o, e) => 
            {
                if (e.NewValue != null && e.NewValue is Mindscape.WpfElements.PropertyEditing.ObjectWrapper)
                {
                    var wrapper = e.NewValue as Mindscape.WpfElements.PropertyEditing.ObjectWrapper;
                    if (wrapper.UnderlyingObject is ICustomTypeDescriptor)
                    {
                        var typeDesc = wrapper.UnderlyingObject as ICustomTypeDescriptor;
                        depObject = typeDesc.GetPropertyOwner(wrapper.Property.Property.AsPropertyDescriptor) as DependencyObject;
                    }
                    else if (wrapper.UnderlyingObject is DependencyObject)
                        depObject = wrapper.UnderlyingObject as DependencyObject;

                    if (depObject != null)
                    {
                        dpd = DependencyPropertyDescriptor.FromProperty(wrapper.Property.Property.AsPropertyDescriptor);
                    }

                    var targetType = wrapper.DataType;
                    if (wrapper.DataType.IsGenericType && wrapper.DataType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        targetType = Nullable.GetUnderlyingType(targetType);
                    updown.IsFloatValue = targetType == typeof(Single) || targetType == typeof(Decimal) || targetType == typeof(Double);

                    initialValue = wrapper.Property.Value;
                    OnPropertyChanged("CanClearValue");
                }
                else
                {
                    depObject = null;
                    dpd = null;
                    initialValue = null;

                    OnPropertyChanged("CanClearValue");
                }
            };
        }
        #endregion

        #region Properties

        public bool CanClearValue
        {
            get
            {
                return depObject != null && dpd != null || initialValue != null;
            }
        }

        #endregion

        #region Methods
        object GetContextObject()
        {
            if (Workspace == null)
                return null;

            var contextObject = Workspace.ContextObject;
            if (contextObject == null && Workspace.ContextObjects != null && Workspace.ContextObjects.Count > 0)
                contextObject = Workspace.ContextObjects[0];
            if (contextObject is IEntityReference)
                contextObject = (contextObject as IEntityReference).ContainedObject;
            return contextObject;
        }
        #endregion

        #region Events Handler
        void unsetValueBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (depObject != null && dpd != null)
            {
                var value = depObject.ReadLocalValue(dpd.DependencyProperty);
                if (initialValue != null && !object.Equals(value, initialValue))
                    dpd.SetValue(depObject, initialValue);
                else if (dpd.DependencyProperty.DefaultMetadata.DefaultValue != null)
                {
                    dpd.SetValue(depObject, DependencyProperty.UnsetValue);
                    initialValue = null;
                }

                var expr = BindingOperations.GetMultiBindingExpression(updown, DevExpress.Xpf.Editors.SpinEdit.EditValueProperty);
                if (expr != null)
                    expr.UpdateTarget();
            }
            else if (initialValue != null)
            {
                PropertyInfo prop = DataContext.GetType().GetProperty("Value");
                if (prop != null)
                { 
                    prop.SetValue(DataContext, initialValue, null);

                    var expr = BindingOperations.GetMultiBindingExpression(updown, DevExpress.Xpf.Editors.SpinEdit.EditValueProperty);
                    if (expr != null)
                        expr.UpdateTarget();
                }
            }
        }

        private void updown_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)e.OriginalSource;
            if (updown.MinValue.HasValue || updown.MaxValue.HasValue)
            {
                decimal dValue;
                if (decimal.TryParse(textBox.Text, out dValue))
                {
                    if (updown.MinValue.HasValue && dValue < updown.MinValue.Value)
                        updown.EditValue = updown.MinValue.Value;
                    else if (updown.MaxValue.HasValue && dValue > updown.MaxValue.Value)
                        updown.EditValue = updown.MaxValue.Value;
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }

        #endregion
    }
}
