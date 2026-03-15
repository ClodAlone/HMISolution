using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriverCodeBaseEx.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for Station.xaml
    /// </summary>
    public partial class Station : UserControl
    {
        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;

        public Station()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                DynTagSettings dts = DataContext as DynTagSettings;
                if (dts != null)
                {
                    // force to select 1st station when no station was previously associated to tag (generally when create a new tag)
                    if (string.IsNullOrEmpty(dts.StationName) && CmbStation.Items.Count == 1)
                    {
                        dts.StationName = ((StationSettings)CmbStation.Items[0]).Name;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dts.StationName))
                        {
                            // check if station assigned to tag still exist --> if not, empty it
                            if (CmbStation.Items.Cast<StationSettings>().ToList().Count(a => a.Name == dts.StationName) == 0)
                                dts.StationName = String.Empty;
                        }
                    }
                }
            };
        }
    }
}
