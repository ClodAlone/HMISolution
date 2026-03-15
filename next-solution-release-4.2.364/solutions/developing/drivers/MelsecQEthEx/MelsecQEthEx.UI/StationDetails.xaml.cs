using System;
using System.ComponentModel;
using System.Windows.Controls;
using MelsecQEth;

namespace MelsecQEth.UI
{
    /// <summary>
    /// Interaction logic for StationDetails.xaml
    /// </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        public StationDetails()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as MelsecQEthStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0, baseStation);

                CmbPlcType.ItemsSource = Enum.GetValues(typeof(MelsecQEthProtocol.PlcTypes));

                CmbPlcType_TextChanged(null, new EventArgs());

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbPlcType, CmbPlcType_TextChanged);
            };
        }

        private void CmbPlcType_TextChanged(object sender, EventArgs e)
        {
            if (!Enum.TryParse(CmbPlcType.Text, true, out MelsecQEthProtocol.PlcTypes plcType))
                return;

            if (MelsecQEthProtocol.PlcSupportLabelAddress(plcType))
            {
                LabelNrMaxAggregatedRequests.Visibility = System.Windows.Visibility.Visible;
                TxLabelNrMaxAggregatedRequests.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                LabelNrMaxAggregatedRequests.Visibility = System.Windows.Visibility.Collapsed;
                TxLabelNrMaxAggregatedRequests.Visibility = System.Windows.Visibility.Collapsed;
            }
        }        
    }
}
