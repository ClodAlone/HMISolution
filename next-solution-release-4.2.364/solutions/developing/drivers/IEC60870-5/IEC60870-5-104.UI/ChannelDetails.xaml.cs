////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelDetails.xaml.cs
//
// summary:	Implements the channel details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

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

namespace IEC60870_5_104.UI
{
    /// <summary>   Interaction logic for ChannelDetails.xaml. </summary>
    public partial class ChannelDetails : UserControl
    {
        bool bLoaded = false;
        /// <summary>   Default constructor. </summary>
        public ChannelDetails()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                DriverCodeBase.UI.Controls.TCPChannelSettingsUI baseDyn = new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };
                baseDyn.baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                //insert TCPChannelSettings setup
                MainStack.Children.Insert(0, baseDyn);
            };
        }
    }
}
