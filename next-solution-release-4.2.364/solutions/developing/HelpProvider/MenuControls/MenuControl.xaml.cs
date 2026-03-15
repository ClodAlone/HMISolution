using DevExpress.Xpf.Bars;
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

namespace HelpProvider
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : BarManager
    {
        public MenuControl()
        {
            InitializeComponent();

            DataContextChanged += (o, e) =>
            {
                var ui = DataContext as HelpProviderUI;
                if (ui != null)
                    rbLocalHelp.IsChecked = ui.LocalHelp;
            };
        }
    }
}
