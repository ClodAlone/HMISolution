using System.Windows;
using WpfApp3.Contracts;

namespace WpfApp3.Plugins
{
    public class HtmlEditorPlugin : IPlugin
    {
        public string Name => "HtmlEditorPlugin";
        public string Header => "HTML/SVG Editor";
        public PreferredLocation Location => PreferredLocation.Document;

        public UIElement CreateView()
        {
            return new HtmlEditorControl();
        }
    }
}
