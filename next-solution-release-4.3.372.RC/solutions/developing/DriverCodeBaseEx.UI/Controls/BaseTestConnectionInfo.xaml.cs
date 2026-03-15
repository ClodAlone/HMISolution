using System;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseUseControlInfo.xaml
    /// </summary>
    public partial class BaseTestConnectionInfo : UserControl
    {
        private Color COLOR_RESULT_SUCCESSFULL = Colors.LightGreen;
        private Color COLOR_RESULT_FAILED = Colors.Red;

        bool bLoaded = false;
        public BaseTestConnectionInfo()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                var tci = DataContext as TestCommInfo;

                if (tci != null)
                {
                    if (tci.Connected)
                    {
                        TextStateInfo.Text = Properties.Resources.ConnectionSuccesFull;
                        TextStateInfo.Foreground = new SolidColorBrush(COLOR_RESULT_SUCCESSFULL);
                    }
                    else
                    {
                        TextStateInfo.Text = Properties.Resources.ConnectionFailed;
                        TextStateInfo.Foreground = new SolidColorBrush(COLOR_RESULT_FAILED);
                    }

                    TextBoxInfo.Text = string.Format("{0} : {1}\n{2}", Utilities.AssemblyInfo.Product, Utilities.AssemblyInfo.FileVersion, tci.Info);
                }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TextBoxInfo.Text + " ");

            // Set the value saved in the Clipboard
            Clipboard.SetDataObject(sb.ToString());
        }
    }
}
