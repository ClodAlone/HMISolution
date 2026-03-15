using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfDataGrid
{
  internal class DataGridEditorSelector : DataTemplateSelector
  {
    private readonly DataTemplateSelector _parentSelector;
    private readonly IPropertyInfo _property;

    public DataGridEditorSelector(IPropertyInfo property, DataTemplateSelector parentSelector)
    {
      _parentSelector = parentSelector;
      _property = property;
    }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item == null)
      {
        return null;
      }
      PropertyNode node = new PropertyNode(item, _property, n => true);
      return _parentSelector.SelectTemplate(node, container);
    }
  }
}
