using System;
using System.Collections.Generic;
using System.Globalization;
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
using Utilities.WPF;

namespace UFShortcutEditor.Controls
{
    /// <summary>
    /// Interaction logic for TypeKey.xaml
    /// </summary>
    public partial class TypeKey : UserControl
    {
        KeyConverter keyConverter = new KeyConverter();
        ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();

        bool bValidKey;
        public TypeKey()
        {
            InitializeComponent();
            Loaded += (o, e) => { btnKey.Focus(); };
        }

        public String KeyName
        {
            get
            {
                if (!bValidKey)
                    return String.Empty;
                return btnKey.Content as String;
            }
        }

        ModifierKeys modifiers = ModifierKeys.None;
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CheckSystemKey(e))
                return;

            var keyString = (string)keyConverter.ConvertTo(null, CultureInfo.InvariantCulture, e.Key, typeof(string));
            if (modifiers != ModifierKeys.None)
            {
                if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    keyString = (string)keyConverter.ConvertTo(null, CultureInfo.InvariantCulture, e.SystemKey, typeof(string));

                var keyModifiers = (string)modifierKeysConverter.ConvertTo(null, CultureInfo.InvariantCulture, modifiers, typeof(string));
                keyString = String.Format("{0}+{1}", keyModifiers, keyString);
            }

            btnKey.Content = keyString;

            bValidKey = true;
            e.Handled = true;

            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }

        bool CheckSystemKey(KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                modifiers |= ModifierKeys.Alt;
                return true;
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                modifiers |= ModifierKeys.Shift;
                return true;
            }
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                modifiers |= ModifierKeys.Control;
                return true;
            }
            else if (e.Key == Key.LWin || e.Key == Key.RWin)
            {
                modifiers |= ModifierKeys.Windows;
                return true;
            }

            return false;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                modifiers &= ~(ModifierKeys.Alt);
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                modifiers &= ~(ModifierKeys.Shift);
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                modifiers &= ~(ModifierKeys.Control);
            else if (e.Key == Key.LWin || e.Key == Key.RWin)
                modifiers &= ~(ModifierKeys.Windows);

            e.Handled = true;
        }
    }
}
