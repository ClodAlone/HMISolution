using System.Windows.Controls;
using System.Windows;

using WpfApp1.Plugins;

namespace WpfApp1.SamplePlugin
{
    public class SamplePluginControl : UserControl, IPlugin
    {
        public string Name => "Sample Plugin";

        public UserControl CreateControl()
        {
            var tb = new TextBlock { Text = "Hello from Sample Plugin", Margin = new Thickness(12) };
            var border = new Border { Child = tb, Padding = new Thickness(8) };
            return new UserControl { Content = border };
        }
    }
}
