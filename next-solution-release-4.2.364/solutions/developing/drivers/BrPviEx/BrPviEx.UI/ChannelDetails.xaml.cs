using System.Windows.Controls;

namespace BrPvi.UI
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
                MainStack.Children.Insert(0, new DriverCodeBaseEx.UI.Controls.BaseChannelSettings() { DataContext = DataContext });
                //Dictionary<int, string> r = new Dictionary<int, string>();
                //r.Add(0, Properties.Resources.BrPviVersion2);
                //r.Add(1, Properties.Resources.BrPviVersion3);
                ////r.Add(0, "Version 2x");
                ////r.Add(1, "Version 3x");
                //CmbBrPviVersion.ItemsSource = r;                
            };
        }
    }
}
