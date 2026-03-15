using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UFCrypterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cmbSeed.ItemsSource = new List<string>() { "Project Protection", "Data Protection" };
            cmbSeed.SelectedIndex = 0;
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbSeed.SelectedIndex == 0)
                    txtClearBox.Text = WPFUtilities.CryptString.CryptString.DecryptString(txtCryptBox.Text);
                else
                    txtClearBox.Text = WPFUtilities.CryptString.CryptString.DecryptString(txtCryptBox.Text, DataProtection.Common.CryptCommon.key);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbSeed.SelectedIndex == 0)
                    txtCryptBox.Text = WPFUtilities.CryptString.CryptString.EncryptString(txtClearBox.Text);
                else
                    txtCryptBox.Text = WPFUtilities.CryptString.CryptString.EncryptString(txtClearBox.Text, DataProtection.Common.CryptCommon.key);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
