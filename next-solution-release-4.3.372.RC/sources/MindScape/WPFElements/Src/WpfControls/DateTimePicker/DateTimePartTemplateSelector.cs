using System;
using System.Windows;
using System.Windows.Controls;
using Mindscape.WpfElements.Properties;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Selects DataTemplates for <see cref="DateTimeDisplayElement"/> collections.
  /// </summary>
  public class DateTimePartTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the DataTemplate for numeric elements.
    /// </summary>
    public DataTemplate NumericElementTemplate { get; set; }
    
    /// <summary>
    /// Gets or sets the DataTemplate for selection elements.
    /// </summary>
    public DataTemplate SelectElementTemplate { get; set; }

    /// <summary>
    /// Gets or sets the DataTemplate for read-only elements.
    /// </summary>
    public DataTemplate ReadOnlyElementTemplate { get; set; }

    /// <summary>
    /// Returns the <see cref="DataTemplate"/> corresponding to the supplied
    /// <see cref="DateTimeDisplayElement"/>.
    /// </summary>
    /// <param name="item">A DateTimeDisplayElement.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>The template corresponding to the supplied DateTimeDisplayElement.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      DateTimeDisplayElement element = (DateTimeDisplayElement)item;

      switch (element.ElementType)
      {
        case DateTimeDisplayElementType.Numeric: return NumericElementTemplate;
        case DateTimeDisplayElementType.Select: return SelectElementTemplate;
        case DateTimeDisplayElementType.ReadOnly: return ReadOnlyElementTemplate;
        default:
          string message = StringUtils.FormatCurrentCulture(Resources.UnknownElementType, element.ElementType);
          throw new ArgumentException(message, "item");
      }
    }
  }
}
