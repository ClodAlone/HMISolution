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
    /// Interaction logic for MaskedTextBoxPropertyEditor.xaml
    /// </summary>
    public partial class MaskedTextBoxPropertyEditor : UserControl
    {
        public static string DefaultIpMask = @"([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}";

        #region Mask
        public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(String), typeof(MaskedTextBoxPropertyEditor), new UIPropertyMetadata("", new PropertyChangedCallback(OnMaskChanged), new CoerceValueCallback(OnCoerceMask)));

        private static object OnCoerceMask(DependencyObject o, object value)
        {
            MaskedTextBoxPropertyEditor maskedTextBoxPropertyEditor = o as MaskedTextBoxPropertyEditor;
            if (maskedTextBoxPropertyEditor != null)
                return maskedTextBoxPropertyEditor.OnCoerceMask((String)value);
            else
                return value;
        }

        private static void OnMaskChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MaskedTextBoxPropertyEditor maskedTextBoxPropertyEditor = o as MaskedTextBoxPropertyEditor;
            if (maskedTextBoxPropertyEditor != null)
                maskedTextBoxPropertyEditor.OnMaskChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceMask(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaskChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public String Mask
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(MaskProperty);
            }
            set
            {
                SetValue(MaskProperty, value);
            }
        }
        #endregion

        public MaskedTextBoxPropertyEditor()
        {
            InitializeComponent();
        }
    }
}
