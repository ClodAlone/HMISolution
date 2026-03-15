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


namespace ADEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for AttachmentsPropertyEditor.xaml
    /// </summary>
    public partial class AttachmentsPropertyEditor : UserControl
    {
        public AttachmentsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            
            var settings = button.Tag as string;
            if (settings == null)
                settings = String.Empty;
            
            string filter = string.Empty;
            var fileType = new VistaOpenFileDialog();

            if (fileType.ShowDialog() == true)
            {
                String fileName = fileType.FileName;
                if (settings != null && settings.Length > 0)
                {
                    settings += string.Format(";{0}", fileName);
                }
                else
                    settings = fileName;

                button.Tag = settings;
                text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            button.Tag = String.Empty;
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
