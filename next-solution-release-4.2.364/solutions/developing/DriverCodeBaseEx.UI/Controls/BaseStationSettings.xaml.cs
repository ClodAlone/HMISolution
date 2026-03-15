using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseStationSettings.xaml
    /// </summary>
    public partial class BaseStationSettings : UserControl
    {
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
            };
        }
    }
}
