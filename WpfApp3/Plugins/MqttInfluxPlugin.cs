using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class MqttInfluxPlugin : IPlugin
    {
        public string Name => "MqttInfluxPlugin";
        public string Header => "MQTT to InfluxDB Bridge";
        public PreferredLocation Location => PreferredLocation.Right;

        public UIElement CreateView()
        {
            return new MqttInfluxControl();
        }
    }
}
