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
using LacbusPC;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Reflection;
using DriverCodeBase.Enumerators;
using System.Collections.ObjectModel;

namespace LacbusPC.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        bool alreadyLoaded = false;
        List<string> timeZoneList;

        public ChannelDetails()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                alreadyLoaded = true;
                DriverCodeBase.UI.Controls.BaseChannelSettings baseDyn = new DriverCodeBase.UI.Controls.BaseChannelSettings() { DataContext = DataContext };
                //baseDyn.ScheduleTime.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.ScheduleTimeText.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUse.Visibility = System.Windows.Visibility.Collapsed;
                baseDyn.PollNotUseText.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.PollError.Visibility = System.Windows.Visibility.Collapsed;
                //baseDyn.PollErrorText.Visibility = System.Windows.Visibility.Collapsed;
                MainStack.Children.Insert(0, baseDyn);
                timeZoneList = new List<string>();
                FillTimeZoneList();
                CmbTimeZone.ItemsSource = timeZoneList;
                LacbusPCChannelSettings channelSettings = (LacbusPCChannelSettings)DataContext;
                if (!String.IsNullOrWhiteSpace(channelSettings.LacbusPCTimeZone))
                {
                    CmbTimeZone.SelectedItem = channelSettings.LacbusPCTimeZone;
                }
            };
        }

        const string NoTimeConversion = "(No Time Conversion)";
        private void FillTimeZoneList()
        {
            timeZoneList.Clear();
            ReadOnlyCollection<TimeZoneInfo> timeZones;
            try
            {
                timeZones = TimeZoneInfo.GetSystemTimeZones();
            }
            catch(Exception e)
            {
                return;
            }

            timeZoneList.Add(NoTimeConversion);
            List<string> timeZoneIDs = (from tz in timeZones orderby tz.Id select tz.Id).ToList();
            LacbusPCChannelSettings channelSettings = (LacbusPCChannelSettings)DataContext;
            if (!String.IsNullOrWhiteSpace(channelSettings.LacbusPCTimeZone) &&
                !timeZoneIDs.Contains(channelSettings.LacbusPCTimeZone) &&
                !timeZoneList.Contains(channelSettings.LacbusPCTimeZone))
            {
                timeZoneList.Add(channelSettings.LacbusPCTimeZone);
            }
            timeZoneList.AddRange(timeZoneIDs);
        }
    }
}
