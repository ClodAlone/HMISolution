using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Used by the <see cref="MonthCalendar" /> to manage cells for which there is no valid date.
  /// </summary>
  public class DayTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// The template for valid dates.
    /// </summary>
    public DataTemplate DayTemplate { get; set; }

    /// <summary>
    /// The template for cells beyond the range of the CLR DateTime type.
    /// </summary>
    public DataTemplate NonDayTemplate { get; set; }

    /// <summary>
    /// Selects the appropriate data template depending on whether the item is
    /// a DateTime or not.
    /// </summary>
    /// <param name="item">The item for which to select a template.</param>
    /// <param name="container">The container element.</param>
    /// <returns>A <see cref="DataTemplate"/> suitable for displaying the item.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      return item is DateTime ? DayTemplate : NonDayTemplate;
    }
  }
}
