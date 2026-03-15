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
using Utilities.WPF;

namespace UFSolution
{
    /// <summary>
    /// Interaction logic for ErrorPanel.xaml
    /// </summary>
    public partial class ErrorPanel : UserControl
    {
        public ErrorPanel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            wnd.DialogResult = true;
            wnd.Close();
        }
    }
}
