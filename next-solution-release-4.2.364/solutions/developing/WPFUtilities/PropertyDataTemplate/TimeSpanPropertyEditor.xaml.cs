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

        public TimeSpanPropertyEditor()
        {
            InitializeComponent();
        }
    }
}
