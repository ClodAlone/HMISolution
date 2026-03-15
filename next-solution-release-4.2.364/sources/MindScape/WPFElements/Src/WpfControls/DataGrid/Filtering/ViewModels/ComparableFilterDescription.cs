using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Describes how to build a filter for comparable types.
  /// </summary>
  public class ComparableFilterDescription : ViewModelBase, IFilterDescription
  {
    private readonly Type _comparableType;
    private IComparable _firstValue;
    private IComparable _secondValue;
    private IComparableFilterBuilder _builder = new NoFilterBuilder();

    private IFilter _filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComparableFilterDescription"/> class.
    /// </summary>
    /// <param name="comparableType">The comparable type.</param>
    internal ComparableFilterDescription(Type comparableType)
    {
      _comparableType = comparableType;
      SetDefaultValues();
    }

    private void SetDefaultValues()
    {
      if (Type.GetTypeCode(_comparableType) == TypeCode.DateTime)
      {
        FirstValue = DateTime.Now.Date;
        SecondValue = _firstValue;
      }
      else if (_comparableType.IsValueType)
      {
        FirstValue = Activator.CreateInstance(_comparableType) as IComparable;
        SecondValue = Activator.CreateInstance(_comparableType) as IComparable;
      }
      else
      {
        FirstValue = null;
        SecondValue = null;
      }
    }

    /// <summary>
    /// Gets or sets the first <see cref="IComparable"/> value of the filter.
    /// </summary>
    public IComparable FirstValue
    {
      get { return _firstValue == null ? null : Convert.ChangeType(_firstValue, _comparableType) as IComparable; }
      set
      {
        _firstValue = value;
        UpdateFilter();
        OnPropertyChanged("FirstValue");
      }
    }

    /// <summary>
    /// Gets or sets the second <see cref="IComparable"/> value of the filter.
    /// </summary>
    public IComparable SecondValue
    {
      get { return _secondValue == null ? null : Convert.ChangeType(_secondValue, _comparableType) as IComparable; }
      set
      {
        _secondValue = value;
        UpdateFilter();
        OnPropertyChanged("SecondValue");
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="IComparableFilterBuilder"/>.
    /// </summary>
    public IComparableFilterBuilder Builder
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
    /// Gets the type of comparable object to build a filter for.
    /// </summary>
    public Type ComparableType
    {
      get { return _comparableType; }
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
        Filter = _builder.Build(FirstValue, SecondValue);
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
      SetDefaultValues();
      if (filter is GreaterThanFilter)
      {
        GreaterThanFilter greaterThanFilter = filter as GreaterThanFilter;
        Builder = new GreaterThanFilterBuilder();
        FirstValue = greaterThanFilter.Value;
      }
      else if (filter is GreaterThanOrEqualToFilter)
      {
        GreaterThanOrEqualToFilter greaterThanOrEqualToFilter = filter as GreaterThanOrEqualToFilter;
        Builder = new GreaterThanOrEqualToFilterBuilder();
        FirstValue = greaterThanOrEqualToFilter.Value;
      }
      else if (filter is LessThanFilter)
      {
        LessThanFilter lessThanFilter = filter as LessThanFilter;
        Builder = new LessThanFilterBuilder();
        FirstValue = lessThanFilter.Value;
      }
      else if (filter is LessThanOrEqualToFilter)
      {
        LessThanOrEqualToFilter lessThanOrEqualToFilter = filter as LessThanOrEqualToFilter;
        Builder = new LessThanOrEqualToFilterBuilder();
        FirstValue = lessThanOrEqualToFilter.Value;
      }
      else if (filter is EqualsFilter)
      {
        EqualsFilter equalsFilter = filter as EqualsFilter;
        Builder = new EqualsFilterBuilder();
        FirstValue = equalsFilter.Value as IComparable;
      }
      else if (filter is NotEqualFilter)
      {
        NotEqualFilter notEqualFilter = filter as NotEqualFilter;
        Builder = new NotEqualFilterBuilder();
        FirstValue = notEqualFilter.Value as IComparable;
      }
      else
      {
        Builder = new NoFilterBuilder();
      }
    }
  }
}
