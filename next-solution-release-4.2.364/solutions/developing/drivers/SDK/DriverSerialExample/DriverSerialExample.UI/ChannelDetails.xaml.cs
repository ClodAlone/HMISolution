////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelDetails.xaml.cs
//
// summary:	Implements the channel details.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Windows.Controls;

namespace DriverSerialExample.UI
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
                   //insert SerialChannelSettings setup
                   MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.SerialChannelSettings() { DataContext = DataContext});

                   //insert option in ComboBox CmbFrame
                   Dictionary<int, string> r = new Dictionary<int, string>();
                   r.Add(0, "RTU");
                   r.Add(1, "ASCII");
                   CmbFrame.ItemsSource = r;
               };
        }
    }
}
