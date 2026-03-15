using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Describes how to build a boolean filter.
  /// </summary>
  public class BooleanFilterDescription : ViewModelBase, IFilterDescription
  {
    private IBooleanFilterBuilder _builder = new AndFilterBuilder();
    private readonly IFilterDescription _firstFilter;
    private readonly IFilterDescription _secondFilter;

    private IFilter _filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanFilterDescription"/> class.
    /// </summary>
    /// <param name="firstFilter">The first part of the boolean filter.</param>
    /// <param name="secondFilter">The second part of the boolean filter.</param>
    internal BooleanFilterDescription(IFilterDescription firstFilter, IFilterDescription secondFilter)
    {
      _firstFilter = firstFilter;
      _secondFilter = secondFilter;

      _firstFilter.FilterChanged += new EventHandler(SubFilterChanged);
      _secondFilter.FilterChanged += new EventHandler(SubFilterChanged);
    }

    private void SubFilterChanged(object sender, EventArgs e)
    {
      UpdateFilter();
    }

    /// <summary>
    /// Gets or sets the <see cref="IBooleanFilterBuilder"/>.
    /// </summary>
    public IBooleanFilterBuilder Builder
    {
      get { return _builder; }
      set
      {
        _builder = value;
        UpdateFilter();
        OnPropertyChanged("Builder");
      }
    }

    /// <summary>
    /// Gets an <see cref="IFilterDescription"/> describing the first part of the boolean filter.
    /// </summary>
    public IFilterDescription FirstFilter
    {
      get { return _firstFilter; }
    }

    /// <summary>
    /// Gets an <see cref="IFilterDescription"/> describing the second part of the boolean filter.
    /// </summary>
    public IFilterDescription SecondFilter
    {
      get { return _secondFilter; }
    }

    /// <summary>
    /// Gets the current filter created by this filter description.
    /// </summary>
    public IFilter Filter
    {
      get { return _filter; }
      private set
      {
        _filter = value;
        OnFilterChanged();
      }
    }

    /// <summary>
    /// Raised when the <see cref="Filter"/> changes.
    /// </summary>
    public event EventHandler FilterChanged;

    private void OnFilterChanged()
    {
      EventHandler handler = FilterChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void UpdateFilter()
    {
      if (_builder != null)
      {
        Filter = _builder.Build(_firstFilter.Filter, _secondFilter.Filter);
      }
      else
      {
        Filter = null;
      }
    }

    /// <summary>
    /// Sets the filter description to best match the given <see cref="IFilter"/>.
    /// </summary>
    /// <param name="filter">The <see cref="IFilter"/> to mimic.</param>
    public void SetAs(IFilter filter)
    {
      IFilter firstFilter = filter;
      IFilter secondFilter = null;
      if (filter is OrFilter)
      {
        OrFilter orFilter = filter as OrFilter;
        if (orFilter.Filters.Count > 0)
        {
          firstFilter = orFilter.Filters[0];
        }
        if (orFilter.Filters.Count > 1)
        {
          secondFilter = orFilter.Filters[1];
        }
        Builder = new OrFilterBuilder();
      }
      if (filter is AndFilter)
      {
        AndFilter andFilter = filter as AndFilter;
        if (andFilter.Filters.Count > 0)
        {
          firstFilter = andFilter.Filters[0];
        }
        if (andFilter.Filters.Count > 1)
        {
          secondFilter = andFilter.Filters[1];
        }
        Builder = new AndFilterBuilder();
      }
      if (filter == null)
      {
        Builder = new AndFilterBuilder();
      }
      _firstFilter.SetAs(firstFilter);
      _secondFilter.SetAs(secondFilter);
    }
  }
}
