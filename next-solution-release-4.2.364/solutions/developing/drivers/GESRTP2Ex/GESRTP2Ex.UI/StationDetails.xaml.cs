////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	StationDetails.xaml.cs
//
// summary:	Implements the station details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using DriverCodeBaseEx;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace GESRTP2.UI
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
                var st = DataContext as GESRTP2StationSettings;
                var baseStation = new DriverCodeBaseEx.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                //insert Channel in ComboBox CmbChannel
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Insert(0,baseStation);
                CmbPlcType.ItemsSource = Enum.GetValues(typeof(GESRTP2Protocol.PlcTypes));

                DependencyPropertyDescriptor descriptor =
                    DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbPlcType, CmbPlcType_TextChanged);

                CmbPlcType_TextChanged(CmbPlcType, new EventArgs());
            };
        }

        /// <summary>
        /// CmbPlcType_TextChanged  Event linked to the PLC type combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbPlcType_TextChanged(object sender, EventArgs e)
        {
            if(CmbPlcType.SelectedValue != null)
            {
                if ((int)CmbPlcType.SelectedValue == ((int)GESRTP2Protocol.PlcTypes.PacSystem))
                {
                    TBRack.Visibility = System.Windows.Visibility.Visible;
                    TRack.Visibility = System.Windows.Visibility.Visible;
                    TBSlot.Visibility = System.Windows.Visibility.Visible;
                    TSlot.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    TBRack.Visibility = System.Windows.Visibility.Collapsed;
                    TRack.Visibility = System.Windows.Visibility.Collapsed;
                    TBSlot.Visibility = System.Windows.Visibility.Collapsed;
                    TSlot.Visibility = System.Windows.Visibility.Collapsed;
                    var st = DataContext as GESRTP2StationSettings;
                    st.Rack = 0;
                    st.Slot = 1;
                }
            }            
        }
    }
}
