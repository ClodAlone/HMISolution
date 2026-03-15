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
using System.ComponentModel;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using NaisFp;
using DriverCodeBase.Helpers;
using UFInterfaces.Editors;

namespace NaisFp.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        DriverCodeBase.UI.Controls.TCPChannelSettingsUI baseDynTcp;
        DriverCodeBase.UI.Controls.SerialChannelSettings baseDynSeial;
        public ChannelDetails()
        {


            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                baseDynTcp = new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };
                baseDynSeial = new DriverCodeBase.UI.Controls.SerialChannelSettings() { DataContext = DataContext };
                MainStack.Children.Insert(1, baseDynTcp);
                MainStack.Children.Insert(1, baseDynSeial);                
                //MainStackGrid.Children.Insert(1, baseDynTcp);
                //MainStackGrid.Children.Insert(1, baseDynSeial);

                CmbChannelType.ItemsSource = Enum.GetValues(typeof(ChannelTypes));


                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                descriptor.AddValueChanged(CmbChannelType, CmbChannelType_TextChanged);
                CmbChannelType_TextChanged();

                descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(CkFP2ETLAN, FP2ETLAN_Changed);
                FP2ETLAN_Changed();

            };
        }
        private void FP2ETLAN_Changed(object sender, EventArgs e)
        {
            FP2ETLAN_Changed();
        }

        private void FP2ETLAN_Changed()
        {
            if (CkFP2ETLAN.IsChecked == true)
            {
                TxBSupervisorID.Visibility = System.Windows.Visibility.Visible;
                SupervisorID.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TxBSupervisorID.Visibility = System.Windows.Visibility.Collapsed;
                SupervisorID.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void CmbChannelType_TextChanged(object sender, EventArgs e)
        {
            CmbChannelType_TextChanged();
        }
        private void CmbChannelType_TextChanged()
        {
            if (CmbChannelType.Text == ChannelTypes.Serial.ToString())
            {
                baseDynTcp.baseDyn.ChNameText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.ChName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.TimeoutText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.Timeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.TxBHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.HostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.TxBBackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.BackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.TxBSwitchHostTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.SwitchHostTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.TxBHostPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynTcp.HostPort.Visibility = System.Windows.Visibility.Collapsed;
                TxBFP2ETLAN.Visibility = System.Windows.Visibility.Collapsed;
                CkFP2ETLAN.Visibility = System.Windows.Visibility.Collapsed;
                TxBSupervisorID.Visibility = System.Windows.Visibility.Collapsed;
                SupervisorID.Visibility = System.Windows.Visibility.Collapsed;

                baseDynSeial.baseDyn.ChNameText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.ChName.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.TimeoutText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.Timeout.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.PollError.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBKeepOpened.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.KeepOpened.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBPort.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbPort.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBPortLinux.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbPortLinux.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBBaud.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbBaud.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBDataBits.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.DataBits.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBParity.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbParity.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBStop.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbStop.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBHandShake.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CmbHandShake.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBCommPortRtsEnable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CommPortRtsEnable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBCommPortDtrEnable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.CommPortDtrEnable.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBRxTimeout.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.RxTimeout.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxBTxTimeout.Visibility = System.Windows.Visibility.Visible;
                baseDynSeial.TxTimeout.Visibility = System.Windows.Visibility.Visible;
                TxBUSBconnection.Visibility = System.Windows.Visibility.Visible;
                CkUSBconnection.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                baseDynTcp.baseDyn.ChNameText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.ChName.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.TimeoutText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.Timeout.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.PollError.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.TxBHostName.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.HostName.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.TxBBackupHostName.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.BackupHostName.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.TxBSwitchHostTimeout.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.SwitchHostTimeout.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.TxBHostPort.Visibility = System.Windows.Visibility.Visible;
                baseDynTcp.HostPort.Visibility = System.Windows.Visibility.Visible;
                TxBFP2ETLAN.Visibility = System.Windows.Visibility.Visible;
                CkFP2ETLAN.Visibility = System.Windows.Visibility.Visible;
                if (CkFP2ETLAN.IsChecked == true)
                {
                    TxBSupervisorID.Visibility = System.Windows.Visibility.Visible;
                    SupervisorID.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    TxBSupervisorID.Visibility = System.Windows.Visibility.Collapsed;
                    SupervisorID.Visibility = System.Windows.Visibility.Collapsed;
                }

                baseDynSeial.baseDyn.ChNameText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.ChName.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.TimeoutText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.Timeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBKeepOpened.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.KeepOpened.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbPort.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBPortLinux.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbPortLinux.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBBaud.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbBaud.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBDataBits.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.DataBits.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBParity.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbParity.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBStop.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbStop.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBHandShake.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CmbHandShake.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBCommPortRtsEnable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CommPortRtsEnable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBCommPortDtrEnable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.CommPortDtrEnable.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBRxTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.RxTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxBTxTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDynSeial.TxTimeout.Visibility = System.Windows.Visibility.Collapsed;
                TxBUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
                CkUSBconnection.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
