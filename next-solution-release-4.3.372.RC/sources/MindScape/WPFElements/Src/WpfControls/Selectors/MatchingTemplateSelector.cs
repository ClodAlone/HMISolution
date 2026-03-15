using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Selects a data template based on a list of match criteria.
  /// </summary>
  [ContentProperty("Templates")]
  public class MatchingTemplateSelector : DelegatingDataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the default data template.
    /// </summary>
    public DataTemplate DefaultTemplate { get; set; }

    private readonly Collection<IDataTemplateMatcher> _templates = new Collection<IDataTemplateMatcher>();

    /// <summary>
    /// Gets the list of templates and matching criteria.
    /// </summary>
    public Collection<IDataTemplateMatcher> Templates
    {
      get { return _templates; }
    }

    /// <summary>
    /// Returns the <see cref="DataTemplate"/> associated with the first criterion to match the item.
    /// </summary>
    /// <param name="item">The item for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>The template from the first matching entry in the <see cref="Templates"/> list if any; otherwise
    /// the <see cref="DefaultTemplate"/>.</returns>
    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
      foreach (IDataTemplateMatcher matcher in _templates)
      {
        if (matcher.Matches(item))
        {
          return matcher.Template;
        }
      }

      return DefaultTemplate;
    }
  }
}
