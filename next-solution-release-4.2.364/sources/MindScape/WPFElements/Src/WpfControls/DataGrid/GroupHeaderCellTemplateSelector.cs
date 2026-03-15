using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Selects a template for <see cref="GroupHeaderCell"/> instances.
  /// </summary>
  public class GroupHeaderCellTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the default template for displaying a group header cell with no aggregate.
    /// </summary>
    public DataTemplate NoAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="SumAggregate"/> results.
    /// </summary>
    public DataTemplate SumAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="CountAggregate"/> results.
    /// </summary>
    public DataTemplate CountAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="AverageAggregate"/> results.
    /// </summary>
    public DataTemplate AverageAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="MinimumAggregate"/> results.
    /// </summary>
    public DataTemplate MinimumAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="MaximumAggregate"/> results.
    /// </summary>
    public DataTemplate MaximumAggregateTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default template for displaying <see cref="ModeAggregate"/> results.
    /// </summary>
    public DataTemplate ModeAggregateTemplate { get; set; }

    /// <summary>
    /// Selects a template for the given <see cref="GroupHeaderCell"/>.
    /// </summary>
    /// <param name="item">The <see cref="GroupHeaderCell"/>.</param>
    /// <param name="container">The template container.</param>
    /// <returns>The selected <see cref="DataTemplate"/>.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item is GroupHeaderCell)
      {
        GroupHeaderCell cell = item as GroupHeaderCell;
        if (cell.Column != null)
        {
          DataTemplate result = cell.Column.GroupHeaderCellTemplate;
          if (result == null)
          {
            if (cell.Aggregate is SumAggregate)
            {
              result = SumAggregateTemplate;
            }
            else if (cell.Aggregate is CountAggregate)
            {
              result = CountAggregateTemplate;
            }
            else if (cell.Aggregate is AverageAggregate)
            {
              result = AverageAggregateTemplate;
            }
            else if (cell.Aggregate is MinimumAggregate)
            {
              result = MinimumAggregateTemplate;
            }
            else if (cell.Aggregate is MaximumAggregate)
            {
              result = MaximumAggregateTemplate;
            }
            else if (cell.Aggregate is ModeAggregate)
            {
              result = ModeAggregateTemplate;
            }
            else
            {
              result = NoAggregateTemplate;
            }
          }
          return result;
        }
      }
      return base.SelectTemplate(item, container);
    }
  }
}
