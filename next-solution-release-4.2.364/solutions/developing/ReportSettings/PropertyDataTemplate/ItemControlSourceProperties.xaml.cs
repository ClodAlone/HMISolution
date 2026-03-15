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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Utilities.WPF;
using DataReader;
using ReportSettings.Documents;

namespace ReportSettings.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ItemControlSourceProperties.xaml
    /// </summary>
    public partial class ItemControlSourceProperties : UserControl
    {
        public ItemControlSourceProperties()
        {
            InitializeComponent();
        }

        private void DataEditor_Click(object sender, RoutedEventArgs e)
        {
            var doc = DataContext as ReportDocument;
            var dataReaderModel = doc.ReaderItemSources;
            var dataReaderEditor = new DataReaderEditor.DataReaderEditor(dataReaderModel, doc.rootBase);
            var Dialog = new GeneralDialogContent(dataReaderEditor)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "DataReaderEditor"
            };
            if (Dialog.ShowDialog() != true)
            {
                return;
            }
            doc.ReaderItemSources = dataReaderModel;
        }

        private void ClearDataSource_Click(object sender, RoutedEventArgs e)
        {
            var doc = DataContext as ReportDocument;
            doc.ReaderItemSources = null;
        }
    }
}
