using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class OpcDiscoveryPlugin : IPlugin
    {
        public string Name => "OpcDiscoveryPlugin";
        public string Header => "OPC UA Discovery";
        public PreferredLocation Location => PreferredLocation.Bottom;

        public UIElement CreateView()
        {
            return new OpcDiscoveryControl();
        }
    }
}
