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
using CoDeSys;
using System.ComponentModel;

namespace CoDeSys.UI
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
                var st = DataContext as CoDeSysStationSettings;
                var baseStation = new DriverCodeBase.UI.Controls.BaseStationSettings() { DataContext = DataContext };
                baseStation.CmbChannel.ItemsSource = st.DriverSettings.ChannelSettings;
                MainStack.Children.Add(baseStation);

                //hide this parameter (driver force value = 0 on constructor) because other values (>0) create problem on detect station error status
                //baseStation.MaxRetryText.Visibility = Visibility.Collapsed;
                //baseStation.MaxRetry.Visibility = Visibility.Collapsed;

                //CmbPlcVersion.ItemsSource = Enum.GetValues(typeof(CoDeSysProtocol.PlcVersion));
                //CmbPlcVersion_TextChanged();                
                //DependencyPropertyDescriptor descriptor =
                //   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                //descriptor.AddValueChanged(CmbPlcVersion, CmbPlcVersion_TextChanged);

                //CmbProtocol.ItemsSource = Enum.GetValues(typeof(CoDeSysProtocol.Protocol));
            };
        }

        //private void CmbPlcVersion_TextChanged(object sender, EventArgs e)
        //{
        //    CmbPlcVersion_TextChanged();
        //}

        //private void CmbPlcVersion_TextChanged()
        //{
        ////    if (CmbPlcType.Text == String.Empty)
        ////        return;
        ////    if (CmbPlcType.Text == PlcTypes.Micro800_series.ToString())
        ////    {
        ////        CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
        ////        TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
        ////        CPUSlot.Visibility = System.Windows.Visibility.Collapsed;
        ////        TbCPUSlot.Visibility = System.Windows.Visibility.Collapsed;
        ////    }
        ////    else
        ////    {
        ////        CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
        ////        TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
        ////        CPUSlot.Visibility = System.Windows.Visibility.Visible;
        ////        TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
        ////    }

        //}
    }
}
