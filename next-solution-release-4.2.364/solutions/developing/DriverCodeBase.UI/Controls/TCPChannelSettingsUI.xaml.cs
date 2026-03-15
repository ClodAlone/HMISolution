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

namespace DriverCodeBase.UI.Controls
{
    /// <summary>
    /// Interaction logic for TCPChannelSettingsUI.xaml
    /// </summary>
    public partial class TCPChannelSettingsUI : UserControl
    {
        bool bLoaded = false;
        public BaseChannelSettings baseDyn;
        public TCPChannelSettingsUI()
        {
            InitializeComponent();
            baseDyn = new BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                baseDyn.DataContext = DataContext;
                TCPStack.Children.Insert(0, baseDyn);                
            };
        }
    }
}
