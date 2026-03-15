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

namespace GESRTP2.UI
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
                //insert TCPChannelSettings setup
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext });
            };
        }
    }
}
