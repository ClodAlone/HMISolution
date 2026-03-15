using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class OpcMonitorPlugin : IPlugin
    {
        public string Name => "OpcMonitorPlugin";
        public string Header => "Data Monitor";
        public PreferredLocation Location => PreferredLocation.Bottom;

        public UIElement CreateView()
        {
            return new OpcMonitorControl();
        }
    }
}
