using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class GraphicEditorPlugin : IPlugin
    {
        public string Name => "GraphicEditorPlugin";
        public string Header => "Graphic Editor";
        public PreferredLocation Location => PreferredLocation.Document;

        public UIElement CreateView()
        {
            return new GraphicEditorControl();
        }
    }
}
