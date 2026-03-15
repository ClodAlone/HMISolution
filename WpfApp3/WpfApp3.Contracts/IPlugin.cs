using System.Windows;

namespace WpfApp3.Contracts
{
    public interface IPlugin
    {
        string Name { get; }
        string Header { get; }
        UIElement CreateView();
        PreferredLocation Location { get; }
    }

    public enum PreferredLocation
    {
        Right,
        Bottom,
        Document
    }
}
