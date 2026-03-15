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

namespace Databoom.UI
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
                DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext };
                baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
