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
using Ookii.Dialogs.Wpf;

namespace CommonControls.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for SelectPathPropertyEditor.xaml
    /// </summary>
    public partial class SelectPathPropertyEditor : UserControl
    {
        public SelectPathPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = button.Tag as String;
            if (dialog.ShowDialog() == true)
            {
                button.Tag = dialog.SelectedPath;
                text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = null;
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
