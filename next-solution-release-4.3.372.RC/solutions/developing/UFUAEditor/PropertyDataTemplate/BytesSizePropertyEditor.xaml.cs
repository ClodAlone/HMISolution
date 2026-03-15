using DevExpress.Xpf.Editors;
using Mindscape.WpfElements;
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
using UFUAEditor.Converters;
using Utilities;
using WPFUtilities;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for BytesSizePropertyEditor.xaml
    /// </summary>
    public partial class BytesSizePropertyEditor : UserControl
    {
        #region Dependency Properties

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(BytesSizePropertyEditor), new UIPropertyMetadata(double.NaN, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            BytesSizePropertyEditor updown = o as BytesSizePropertyEditor;
            if (updown != null)
                return updown.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BytesSizePropertyEditor updown = o as BytesSizePropertyEditor;
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
            //UpdateConverterParameters();
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
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(BytesSizePropertyEditor), new UIPropertyMetadata(double.NaN, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            BytesSizePropertyEditor updown = o as BytesSizePropertyEditor;
            if (updown != null)
                return updown.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BytesSizePropertyEditor updown = o as BytesSizePropertyEditor;
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
            //UpdateConverterParameters();
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

        #endregion

        #region Declarations
        bool bLoaded;
        #endregion

        #region Constructors
        public BytesSizePropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) => 
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                unitMeasureLabel.Text = WPFUtilities.Converters.BytesUnit.MBytes.ToString();
                var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                ThemeHelper.SetTheme(updown, currentStyle);
            };
        }
        #endregion
    }
}
