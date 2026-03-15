////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelDetails.xaml.cs
//
// summary:	Implements the channel details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Controls;

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
                DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI baseDyn = new DriverCodeBaseEx.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext };
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
