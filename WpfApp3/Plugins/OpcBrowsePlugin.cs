using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class OpcBrowsePlugin : IPlugin
    {
        public string Name => "OpcBrowsePlugin";
        public string Header => "OPC UA Browser";
        public PreferredLocation Location => PreferredLocation.Right;

        public UIElement CreateView()
        {
            return new OpcBrowseControl();
        }
    }
}
