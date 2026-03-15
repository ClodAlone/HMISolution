using System.Windows.Controls;

namespace WpfApp1.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        UserControl CreateControl();
    }
}
