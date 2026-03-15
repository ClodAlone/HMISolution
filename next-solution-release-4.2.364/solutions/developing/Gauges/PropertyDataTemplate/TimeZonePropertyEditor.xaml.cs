using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Gauges.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TimeSpanPropertyEditor.xaml
    /// </summary>
    public partial class TimeZonePropertyEditor : UserControl
    {
        bool bInit;
        ReadOnlyCollection<TimeZoneInfo> tz;
        public TimeZonePropertyEditor()
        {
            InitializeComponent();
            tz = TimeZoneInfo.GetSystemTimeZones();
            bInit = true;
            timezones.ItemsSource = tz;
        }

        private void timezones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bInit)
            {
                bInit = false;
                return;
            }

            timezone.Tag = new ClockTimeZone() { TimeZone = (TimeZoneInfo)timezones.SelectedValue };
        }
    }
}
