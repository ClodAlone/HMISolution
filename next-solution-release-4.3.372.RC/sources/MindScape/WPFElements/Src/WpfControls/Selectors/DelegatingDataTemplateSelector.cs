using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Selects a template, falling back to another <see cref="DataTemplateSelector"/> if
  /// no template is found by this selector.
  /// </summary>
  public abstract class DelegatingDataTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> on which to fall back if no template
    /// is found by this selector.
    /// </summary>
    public DataTemplateSelector BasedOn { get; set; }

    /// <summary>
    /// Returns a <see cref="DataTemplate"/> for the item.
    /// </summary>
    /// <param name="item">The item for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>A DataTemplate for the item.</returns>
    public sealed override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      DataTemplate template = SelectTemplateCore(item, container);

      if (template == null && BasedOn != null)
      {
        return BasedOn.SelectTemplate(item, container);
      }
      else
      {
        return template;
      }
    }

    /// <summary>
    /// When overridden in a derived class, gets a <see cref="DataTemplate"/> for the item.
    /// If this method returns null, the item is handed off to the <see cref="BasedOn"/> <see cref="DataTemplateSelector"/>.
    /// </summary>
    /// <param name="item">The item for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>A DataTemplate for the item, or null to delegate to the BasedOn DataTemplateSelector.</returns>
    protected abstract DataTemplate SelectTemplateCore(object item, DependencyObject container);
  }
}
