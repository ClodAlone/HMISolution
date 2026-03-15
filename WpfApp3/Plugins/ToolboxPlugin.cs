using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class ToolboxPlugin : IPlugin
    {
        public string Name => "ToolboxPlugin";
        public string Header => "Toolbox";
        public PreferredLocation Location => PreferredLocation.Right;

        public UIElement CreateView()
        {
            return new ToolboxControl();
        }
    }
}
