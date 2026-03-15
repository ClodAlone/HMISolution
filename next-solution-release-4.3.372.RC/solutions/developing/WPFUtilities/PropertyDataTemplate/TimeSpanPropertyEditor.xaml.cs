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

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TimeSpanPropertyEditor.xaml
    /// </summary>
    public partial class TimeSpanPropertyEditor : UserControl
    {
        #region TimeSpanFormat
        public static readonly DependencyProperty TimeSpanFormatProperty = DependencyProperty.Register("TimeSpanFormat", typeof(String), typeof(TimeSpanPropertyEditor), new UIPropertyMetadata("hh:mm:ss", new PropertyChangedCallback(OnTimeSpanFormatChanged), new CoerceValueCallback(OnCoerceTimeSpanFormat)));

        private static object OnCoerceTimeSpanFormat(DependencyObject o, object value)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                return timeSpanPropertyEditor.OnCoerceTimeSpanFormat((String)value);
            else
                return value;
        }

        private static void OnTimeSpanFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                timeSpanPropertyEditor.OnTimeSpanFormatChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceTimeSpanFormat(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTimeSpanFormatChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public String TimeSpanFormat
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(TimeSpanFormatProperty);
            }
            set
            {
                SetValue(TimeSpanFormatProperty, value);
            }
        }

        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(TimeSpan?), typeof(TimeSpanPropertyEditor), new UIPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                return timeSpanPropertyEditor.OnCoerceMinValue((TimeSpan?)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                timeSpanPropertyEditor.OnMinValueChanged((TimeSpan?)e.OldValue, (TimeSpan?)e.NewValue);
        }

        protected virtual TimeSpan? OnCoerceMinValue(TimeSpan? value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TimeSpan? MinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan?)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        #endregion

        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(TimeSpan?), typeof(TimeSpanPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                return timeSpanPropertyEditor.OnCoerceMaxValue((TimeSpan?)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TimeSpanPropertyEditor timeSpanPropertyEditor = o as TimeSpanPropertyEditor;
            if (timeSpanPropertyEditor != null)
                timeSpanPropertyEditor.OnMaxValueChanged((TimeSpan?)e.OldValue, (TimeSpan?)e.NewValue);
        }

        protected virtual TimeSpan? OnCoerceMaxValue(TimeSpan? value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public TimeSpan? MaxValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan?)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        #endregion

        public TimeSpanPropertyEditor()
        {
            InitializeComponent();
        }
    }
}
