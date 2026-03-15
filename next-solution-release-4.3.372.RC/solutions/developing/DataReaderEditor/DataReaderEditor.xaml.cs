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
using DataReader;

namespace DataReaderEditor
{
    /// <summary>
    /// Interaction logic for DataReaderEditor.xaml
    /// </summary>
    public partial class DataReaderEditor : UserControl
    {
        readonly DataReaderModelView modelView;
        public DataReaderEditor(DataReaderModel model, String projectRoot, bool showXMLTab = true)
        {
            InitializeComponent();

            modelView = new DataReaderModelView(model, projectRoot);
            DataContext = modelView;
            if (!showXMLTab || string.IsNullOrEmpty(modelView.XmlUri))
                xmlTab.Visibility = Visibility.Collapsed;
        }

        private void select_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
                return;
            if(modelView != null)
                modelView.ParseQuery();
        }
    }
}
