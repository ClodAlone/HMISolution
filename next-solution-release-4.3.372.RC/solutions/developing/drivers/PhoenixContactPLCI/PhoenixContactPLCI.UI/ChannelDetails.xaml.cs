using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace PhoenixContactPLCI.UI
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
                //CmbPlcVersion.ItemsSource = Enum.GetValues(typeof(PhoenixContactPLCIProtocol.PlcVersion));
                //CmbPlcVersion_TextChanged();
                //DependencyPropertyDescriptor descriptor =
                //   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                //descriptor.AddValueChanged(CmbPlcVersion, CmbPlcVersion_TextChanged);

                //CmbProtocol.ItemsSource = Enum.GetValues(typeof(PhoenixContactPLCIProtocol.Protocol));
            };
        }
        private void CmbPlcVersion_TextChanged(object sender, EventArgs e)
        {
            CmbPlcVersion_TextChanged();
        }

        private void CmbPlcVersion_TextChanged()
        {
            //    if (CmbPlcType.Text == String.Empty)
            //        return;
            //    if (CmbPlcType.Text == PlcTypes.Micro800_series.ToString())
            //    {
            //        CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
            //        TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Collapsed;
            //        CPUSlot.Visibility = System.Windows.Visibility.Collapsed;
            //        TbCPUSlot.Visibility = System.Windows.Visibility.Collapsed;
            //    }
            //    else
            //    {
            //        CmbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
            //        TbLogix5550NonBlockPhAdd.Visibility = System.Windows.Visibility.Visible;
            //        CPUSlot.Visibility = System.Windows.Visibility.Visible;
            //        TbCPUSlot.Visibility = System.Windows.Visibility.Visible;
            //    }

        }
    }
}
