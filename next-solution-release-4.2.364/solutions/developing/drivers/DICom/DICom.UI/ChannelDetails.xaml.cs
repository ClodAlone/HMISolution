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
using DriverCodeBase.UI.Controls;

namespace DICom.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        public ChannelDetails()
        {
            bool bLoaded = false;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                DriverCodeBase.UI.Controls.TCPChannelSettingsUI baseDyn = new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };

                baseDyn.baseDyn.WaitTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.WaitTimeText.Visibility = System.Windows.Visibility.Collapsed;

                baseDyn.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;

                baseDyn.baseDyn.TXBStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.DKPStateCommandVariable.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.BackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.TxBBackupHostName.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.SwitchHostTimeout.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.TxBSwitchHostTimeout.Visibility = System.Windows.Visibility.Collapsed;

                baseDyn.TxBHostName.Text = Properties.Resources.LocalHostName;
                MainStack.Children.Insert(0, baseDyn);
                /*Dictionary<int, string> r = new Dictionary<int, string>();
                r.Add(0, "RTU");
                r.Add(1, "ASCII");
                CmbFrame.ItemsSource = r;*/

            };
        }
    }
}
