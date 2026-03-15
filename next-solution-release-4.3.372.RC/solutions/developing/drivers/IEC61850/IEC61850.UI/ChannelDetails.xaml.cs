using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using IEC61850;

namespace IEC61850.UI
{
    /// <summary>
    /// Interaction logic for ChannelDetails.xaml
    /// </summary>
    public partial class ChannelDetails : UserControl
    {
        public ChannelDetails()
        {
            bool bLoaded = false;

            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                   return;
                bLoaded = true;
                MainStack.Children.Insert(0, new DriverCodeBase.UI.Controls.TCPChannelSettingsUI() { DataContext = DataContext });

                CmbTimeZone.ItemsSource = FillTimeZoneList();
                
                IEC61850ChannelSettings channelSettings = (IEC61850ChannelSettings)DataContext;
                if (!String.IsNullOrWhiteSpace(channelSettings.TimeZone))
                    CmbTimeZone.SelectedItem = channelSettings.TimeZone;
                // Do not show the combo for the time zone
                TxtTimeZone.Visibility = System.Windows.Visibility.Collapsed;
                CmbTimeZone.Visibility = System.Windows.Visibility.Collapsed;
            };
        }

        const string NoTimeConversion = "(No Time Conversion)";
        private List<string> FillTimeZoneList()
        {
            List<string> timeZoneList = new List<string>();
            ReadOnlyCollection<TimeZoneInfo> timeZones;
            try
            {
                timeZones = TimeZoneInfo.GetSystemTimeZones();
            }
            catch (Exception e)
            {
                return timeZoneList;
            }

            timeZoneList.Add(NoTimeConversion);
            List<string> timeZoneIDs = (from tz in timeZones orderby tz.Id select tz.Id).ToList();
            IEC61850ChannelSettings channelSettings = (IEC61850ChannelSettings)DataContext;
            if (!String.IsNullOrWhiteSpace(channelSettings.TimeZone) && !timeZoneIDs.Contains(channelSettings.TimeZone) && !timeZoneList.Contains(channelSettings.TimeZone))
                timeZoneList.Add(channelSettings.TimeZone);
            timeZoneList.AddRange(timeZoneIDs);

            return timeZoneList;
        }
    }
}
