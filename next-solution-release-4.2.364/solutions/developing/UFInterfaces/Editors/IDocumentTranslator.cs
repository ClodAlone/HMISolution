using System;
using System.Windows;

namespace UFInterfaces.Editors
{
    public interface IDocumentTranslator {
        Tuple<string, string> GetStringID(UIElement element, string propertyName);
        bool IsTranslated(UIElement control, bool tooltip = false);
    }
}
