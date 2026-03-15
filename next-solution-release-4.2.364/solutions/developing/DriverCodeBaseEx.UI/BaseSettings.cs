using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DriverCodeBaseEx.UI
{
    public class BaseSettings
    {
        public Dictionary<string, UserControl> GetBaseDynamicSettings()
        {
            Dictionary<string, UserControl> dynSett = new Dictionary<string, UserControl>() {
                {"ElementNumber", new SettingsControls.ElementNumber() },
                {"JobConditionalVariable", new SettingsControls.JobConditionalVariable() },
                {"JobOffsetVariable", new SettingsControls.JobOffsetVariable() },
                {"LinkType", new SettingsControls.LinkType() },
                {"Method", new SettingsControls.Method() },
                {"OutputStartup", new SettingsControls.OutputStartup() },
                {"Station", new SettingsControls.Station() },
                {"SwapBytes", new SettingsControls.SwapBytes() },
                {"SwapWords", new SettingsControls.SwapWords() }
            };

            return dynSett;
        }
    }
}
