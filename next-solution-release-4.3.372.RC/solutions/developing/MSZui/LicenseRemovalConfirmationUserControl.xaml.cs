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

namespace MSZui
{
    /// <summary>
    /// Interaction logic for LicenseRemovalConfirmationUserControl.xaml
    /// </summary>
    public partial class LicenseRemovalConfirmationUserControl : UserControl
    {
        public LicenseRemovalConfirmationUserControl(string licenseRemovalCode)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(licenseRemovalCode) == false)
            {
                codeTextBlock.Text = licenseRemovalCode;
            };
        }

        private void codeTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).Focus();
            e.Handled = true;
        }

        private void codeTextBlock_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void btnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(codeTextBlock.Text);
        }
    }
}
