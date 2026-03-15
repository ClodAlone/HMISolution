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

namespace ReportSettings.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for DataReaderPropertyEditor.xaml
    /// </summary>
    public partial class DataReaderPropertyEditor : UserControl
    {
        public DataReaderPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            DataReaderReference value = (DataReaderReference)(button.Tag);
            if (value != null && value.Edit())
                button.Tag = value;
        }
    }
}
