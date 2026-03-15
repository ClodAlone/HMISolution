using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Describes how to build a string filter.
  /// </summary>
  public class StringFilterDescription : ViewModelBase, IFilterDescription
  {
    private string _firstValue = "";
    private string _secondValue = "";
    private bool _matchCase = false;
    private IStringFilterBuilder _builder = new StartsWithFilterBuilder();

    private IFilter _filter;

    internal StringFilterDescription() { }

    /// <summary>
    /// Gets or sets the first string value of the filter.
    /// </summary>
    public string FirstValue
    {
      get { return _firstValue; }
      set
      {
        _firstValue = value;
        UpdateFilter();
        OnPropertyChanged("FirstValue");
      }
    }

    /// <summary>
    /// Gets or sets the second string value of the filter if any.
    /// </summary>
    public string SecondValue
    {
      get { return _secondValue; }
      set
      {
        _secondValue = value;
        UpdateFilter();
        OnPropertyChanged("SecondValue");
      }
    }

    /// <summary>
    /// Gets or sets the case sensitivity of the resulting filter.
    /// </summary>
    public bool MatchCase
    {
      get { return _matchCase; }
      set
      {
        _matchCase = value;
        UpdateFilter();
        OnPropertyChanged("MatchCase");
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="IStringFilterBuilder"/>.
    /// </summary>
    public IStringFilterBuilder Builder
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
        Filter = _builder.Build(FirstValue, SecondValue, MatchCase);
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
      if (filter is StartsWithFilter)
      {
        StartsWithFilter startsWithFilter = filter as StartsWithFilter;
        Builder = new StartsWithFilterBuilder();
        FirstValue = startsWithFilter.Value;
        SecondValue = "";
        MatchCase = startsWithFilter.MatchCase;
      }
      else if (filter is EndsWithFilter)
      {
        EndsWithFilter endsWithFilter = filter as EndsWithFilter;
        Builder = new EndsWithFilterBuilder();
        FirstValue = endsWithFilter.Value;
        SecondValue = "";
        MatchCase = endsWithFilter.MatchCase;
      }
      else if (filter is ContainsFilter)
      {
        ContainsFilter containsFilter = filter as ContainsFilter;
        Builder = new ContainsFilterBuilder();
        FirstValue = containsFilter.Value;
        SecondValue = "";
        MatchCase = containsFilter.MatchCase;
      }
      else
      {
        Builder = new StartsWithFilterBuilder();
        FirstValue = "";
        SecondValue = "";
        MatchCase = false;
      }
    }
  }
}
