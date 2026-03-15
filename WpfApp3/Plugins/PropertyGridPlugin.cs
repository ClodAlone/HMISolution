using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class PropertyGridPlugin : IPlugin
    {
        public string Name => "PropertyGridPlugin";
        public string Header => "Properties";
        public PreferredLocation Location => PreferredLocation.Right;

        public UIElement CreateView()
        {
            return new PropertyGridControl();
        }
    }
}
