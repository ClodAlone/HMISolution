using System.Windows;
using System.Windows.Controls;

namespace BrPvi.UI
{
    internal class BrPviUtils
    {
        public static void MessageBox_Show(UserControl control, string messageBoxText)
        {
            control.Focus();
            MessageBox.Show(messageBoxText);
        }

        public static void MessageBox_Show(UserControl control, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            control.Focus();
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        public static void MessageBox_Show(UserControl control, string messageBoxText, string caption)
        { 
            control.Focus();
            MessageBox.Show(messageBoxText, caption);
        }
    }
}
