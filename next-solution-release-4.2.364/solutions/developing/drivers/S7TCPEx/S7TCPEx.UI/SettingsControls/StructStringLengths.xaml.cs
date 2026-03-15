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

namespace S7TCP.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for StructStringLengths.xaml
    /// </summary>
    public partial class StructStringLengths : UserControl
    {
        public StructStringLengths()
        {
            InitializeComponent();
        }

        private void ResetStructStringLength_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as S7TCPDynTagSettings;
            nDC.StructStringFieldLengths = String.Empty;
        }
    }
}
