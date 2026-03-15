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
using BrPvi;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;

namespace BrPvi.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool alreadyLoaded = false;
        public ChannelDetails()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext });
                //Dictionary<int, string> r = new Dictionary<int, string>();
                //r.Add(0, Properties.Resources.BrPviVersion2);
                //r.Add(1, Properties.Resources.BrPviVersion3);
                ////r.Add(0, "Version 2x");
                ////r.Add(1, "Version 3x");
                //CmbBrPviVersion.ItemsSource = r;                
            };
        }
    }
}
