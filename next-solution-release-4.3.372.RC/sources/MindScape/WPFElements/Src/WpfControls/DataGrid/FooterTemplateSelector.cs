using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Selects a template for the footer of a <see cref="DataGridColumn"/>.
  /// </summary>
  public class FooterTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the default template for displaying a column footer with no aggregate.
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
    /// Selects a <see cref="DataTemplate"/> for the footer of the given <see cref="DataGridColumn"/>.
    /// </summary>
    /// <param name="item">The <see cref="DataGridColumn"/>.</param>
    /// <param name="container">The template container.</param>
    /// <returns>A <see cref="DataTemplate"/> for the footer of the given <see cref="DataGridColumn"/>.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item is DataGridFooterCell)
      {
        DataGridFooterCell footerCell = (DataGridFooterCell)item;
        DataGridColumn column = footerCell.Column;
        DataTemplate result = column.FooterTemplate;
        if (result == null)
        {
          if (column.FooterAggregate is SumAggregate)
          {
            result = SumAggregateTemplate;
          }
          else if (column.FooterAggregate is CountAggregate)
          {
            result = CountAggregateTemplate;
          }
          else if (column.FooterAggregate is AverageAggregate)
          {
            result = AverageAggregateTemplate;
          }
          else if (column.FooterAggregate is MinimumAggregate)
          {
            result = MinimumAggregateTemplate;
          }
          else if (column.FooterAggregate is MaximumAggregate)
          {
            result = MaximumAggregateTemplate;
          }
          else if (column.FooterAggregate is ModeAggregate)
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
      return base.SelectTemplate(item, container);
    }
  }
}
