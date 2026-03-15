using System.Windows.Controls;
using System.Collections.Generic;

namespace FanucCNC.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        private class FC
        {
            public FanucCNCProtocol.MachineSeries Code { get; set; }
            public string Description { get; set; }

            public FC(FanucCNCProtocol.MachineSeries code, string description)
            {
                Code = code;
                Description = description;
            }
        }

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

                List<FC> lst = new List<FC>();
                lst.Add(new FC(FanucCNCProtocol.MachineSeries.Serie0iB, "0iB"));
                lst.Add(new FC(FanucCNCProtocol.MachineSeries.Serie30iB, "30iB"));
                lst.Add(new FC(FanucCNCProtocol.MachineSeries.Serie31iB, "31iB"));
                lst.Add(new FC(FanucCNCProtocol.MachineSeries.Serie35iB, "35iB"));
                cmbMachineSeries.ItemsSource = lst;
            };
        }       
    }
}
