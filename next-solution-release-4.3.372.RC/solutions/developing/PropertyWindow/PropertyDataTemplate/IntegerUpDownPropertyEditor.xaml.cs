using System;
using System.Collections.Generic;
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

namespace PropertyControl.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for IntegerUpDownPropertyEditor.xaml
    /// </summary>
    public partial class IntegerUpDownPropertyEditor : UserControl
    {
        #region Dependency Properties
        
        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(decimal), typeof(IntegerUpDownPropertyEditor), new UIPropertyMetadata((decimal)0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceMinValue((decimal)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                updown.OnMinValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceMinValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

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

        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(decimal), typeof(IntegerUpDownPropertyEditor), new UIPropertyMetadata((decimal)100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceMaxValue((decimal)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                updown.OnMaxValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceMaxValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

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

        #region SpinValue
        public static readonly DependencyProperty SpinValueProperty = DependencyProperty.Register("SpinValue", typeof(decimal), typeof(IntegerUpDownPropertyEditor), new UIPropertyMetadata((decimal)1.0, new PropertyChangedCallback(OnSpinValueChanged), new CoerceValueCallback(OnCoerceSpinValue)));

        private static object OnCoerceSpinValue(DependencyObject o, object value)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                return updown.OnCoerceSpinValue((decimal)value);
            else
                return value;
        }

        private static void OnSpinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IntegerUpDownPropertyEditor updown = o as IntegerUpDownPropertyEditor;
            if (updown != null)
                updown.OnSpinValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceSpinValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpinValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public decimal SpinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(SpinValueProperty);
            }
            set
            {
                SetValue(SpinValueProperty, value);
            }
        }
        #endregion

        #endregion

        public IntegerUpDownPropertyEditor()
        {
            InitializeComponent();
        }
    }
}
