using System;
using System.Windows.Controls;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseGeneralSettings.xaml
    /// </summary>
    public partial class BaseGeneralSettings : UserControl
    {
        DriverSettings dr = null;

        /// <summary>
        /// basic driver general settings  
        /// </summary>
        public BaseGeneralSettings()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                dr = DataContext as DriverSettings;
                if (dr != null)
                {
                    if (StateCommandVariable.IsTagsSet(dr.StateCommandTag))
                    {
                        dr.StateTag = dr.StateCommandTag;
                        dr.CommandTag = dr.StateCommandTag;
                        dr.StateCommandTag = null;
                    }
                }
            };
        }
    }
}
