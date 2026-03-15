using System;
using System.Windows.Controls;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseChannelSettings.xaml
    /// </summary>
    public partial class BaseChannelSettings : UserControl
    {
        ChannelSettings cs = null;
        /// <summary>
        /// basic channel settings 
        /// </summary>
        public BaseChannelSettings()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                cs = DataContext as ChannelSettings;
                if (cs != null)
                {
                    if (StateCommandVariable.IsTagsSet(cs.StateCommandTag))
                    {
                        cs.StateTag = cs.StateCommandTag;
                        cs.CommandTag = cs.StateCommandTag;
                        cs.StateCommandTag = null;
                    }
                }
            };
        }
    }
}
