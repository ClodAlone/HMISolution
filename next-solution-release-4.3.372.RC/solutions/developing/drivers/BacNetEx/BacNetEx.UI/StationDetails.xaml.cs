////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StationDetails.xaml.cs
//
// summary:	Implements the station details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace BACnet.UI
{
    /// <summary>   Interaction logic for StationDetails.xaml. </summary>
    public partial class StationDetails : UserControl
    {
        bool bLoaded = false;
        /// <summary>   Default constructor. </summary>
        public StationDetails()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                var st = DataContext as BACnetStationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                //insert Channel in ComboBox CmbChannel
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0,baseStation);
                CmbTimeSync.ItemsSource = Enum.GetValues(typeof(TimeSyncs));    
                CkWhoIsDisabled.IsChecked = st.WhoIsDisabled;

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(CkRegisterBBMD, CkRegisterBBMD_Changed);

                CkRegisterBBMD_Changed(CkRegisterBBMD, new EventArgs());
            };
        }

        private void CkRegisterBBMD_Changed(object sender, EventArgs e)
        {
            if (CkRegisterBBMD.IsChecked == true)
            {
                 TxBTxBBMDAddress.Visibility = System.Windows.Visibility.Visible;
                 TxBBMDAddress.Visibility = System.Windows.Visibility.Visible;
                 TxBTxBBMDLifetime.Visibility = System.Windows.Visibility.Visible;
                 TxBBMDLifetime.Visibility = System.Windows.Visibility.Visible;
            } else
            {
                TxBTxBBMDAddress.Visibility = System.Windows.Visibility.Collapsed;
                TxBBMDAddress.Visibility = System.Windows.Visibility.Collapsed;
                TxBTxBBMDLifetime.Visibility = System.Windows.Visibility.Collapsed;
                TxBBMDLifetime.Visibility = System.Windows.Visibility.Collapsed;
            }
            TxBBMDAddress.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
