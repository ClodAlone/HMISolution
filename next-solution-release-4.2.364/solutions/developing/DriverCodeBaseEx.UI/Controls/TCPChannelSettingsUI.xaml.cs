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

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for TCPChannelSettingsUI.xaml
    /// </summary>
    public partial class TCPChannelSettingsUI : UserControl
    {
        bool bLoaded = false;
        public DriverCodeBaseEx.UI.Controls.BaseChannelSettings baseDyn;

        public TCPChannelSettingsUI()
        {
            tcpChannelSettingsUI(true);
        }

        public TCPChannelSettingsUI(bool loadBaseControl)
        {
            tcpChannelSettingsUI(loadBaseControl);
        }

        private void tcpChannelSettingsUI(bool loadBaseControl)
        {
            InitializeComponent();
            if (loadBaseControl)
                baseDyn = new DriverCodeBaseEx.UI.Controls.BaseChannelSettings();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                if (loadBaseControl)
                {
                    baseDyn.DataContext = DataContext;
                    TCPStack.Children.Insert(0, baseDyn);
                }
            };
        }
    }
}
