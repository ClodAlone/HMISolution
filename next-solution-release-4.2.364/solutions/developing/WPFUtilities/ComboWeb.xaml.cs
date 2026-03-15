using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace WPFUtilities
{
    /// <summary>
    /// Interaction logic for ComboWeb.xaml
    /// </summary>
    public partial class ComboWeb : ComboBox
    {
        #region DP
        #region ItemPropertyName
        public static readonly DependencyProperty ItemPropertyNameProperty = DependencyProperty.Register("ItemPropertyName", typeof(string), typeof(ComboWeb), new UIPropertyMetadata(null, new PropertyChangedCallback(OnItemPropertyNameChanged), new CoerceValueCallback(OnCoerceItemPropertyName)));

        private static object OnCoerceItemPropertyName(DependencyObject o, object value)
        {
            ComboWeb ComboWeb = o as ComboWeb;
            if (ComboWeb != null)
                return ComboWeb.OnCoerceItemPropertyName((string)value);
            else
                return value;
        }

        private static void OnItemPropertyNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ComboWeb ComboWeb = o as ComboWeb;
            if (ComboWeb != null)
                ComboWeb.OnItemPropertyNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceItemPropertyName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnItemPropertyNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        
        public string ItemPropertyName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ItemPropertyNameProperty);
            }
            set
            {
                SetValue(ItemPropertyNameProperty, value);
            }
        }
        #endregion
        #endregion

        public ComboWeb()
        {
            InitializeComponent();
        }

        #region Custom automation peers
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ComboValueAutomationPeer(this, ItemPropertyName);
        }
        #endregion
    }
}
