using System;
using System.Windows.Controls;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseStationSettings.xaml
    /// </summary>
    public partial class BaseStationSettings : UserControl
    {
        StationSettings st = null;
        /// <summary>
        /// basic station settings 
        /// </summary>
        public BaseStationSettings()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (CmbChannel.SelectedIndex == -1 && CmbChannel.Items.Count > 0)
                {
                    CmbChannel.SelectedIndex = CmbChannel.Items.Count - 1;
                }
                st = DataContext as StationSettings;
                if (st != null)
                {
                    if (StateCommandVariable.IsTagsSet(st.StateCommandTag))
                    {
                        st.StateTag = st.StateCommandTag;
                        st.CommandTag = st.StateCommandTag;
                        st.StateCommandTag = null;
                    }
                }
            };
        }
    }
}
