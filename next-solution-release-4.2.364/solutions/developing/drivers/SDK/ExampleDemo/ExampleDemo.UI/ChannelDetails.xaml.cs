using System.Windows.Controls;

namespace ExampleDemo.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
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
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext });
            };
        }
    }
}
