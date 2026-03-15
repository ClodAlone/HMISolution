using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DriverCodeBase.UI
{
    public static class UIGeneralCommands
    {
        private static GeneralCommand _AddNewStation = new GeneralCommand(
            Properties.UICommandResource.AddNewStationName,
            Properties.UICommandResource.AddNewStationText,
            Properties.UICommandResource.AddNewStationGestures,
            Properties.UICommandResource.AddNewStationGesturesDisplayText,
            Properties.UICommandResource.AddNewStationTooltip,
            Properties.UICommandResource.AddNewStationDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewStation
        {
            get { return _AddNewStation; }
        }

        private static GeneralCommand _AddNewChannel = new GeneralCommand(
            Properties.UICommandResource.AddNewChannelName,
            Properties.UICommandResource.AddNewChannelText,
            Properties.UICommandResource.AddNewChannelGestures,
            Properties.UICommandResource.AddNewChannelGesturesDisplayText,
            Properties.UICommandResource.AddNewChannelTooltip,
            Properties.UICommandResource.AddNewChannelDescription,
            typeof(UIGeneralCommands));

        public static GeneralCommand AddNewChannel
        {
            get { return _AddNewChannel; }
        }

        
    }
}
