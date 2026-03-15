using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Mindscape.WpfElements.PropertyEditing;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Base class for aggregates.
  /// </summary>
  public abstract class AggregateBase : ViewModelBase, IAggregate
  {
    private object _result;
    private IPropertyInfo _propertyInfo;

    /// <summary>
    /// Calculates the aggregate for the given collection of items.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The aggregate value.</returns>
    public object Calculate(IEnumerable items)
    {
      object result = CalculateCore(items);
      _result = result;
      OnPropertyChanged("Result");
      return result;
    }

    /// <summary>
    /// Calculates the aggregate for the given collection of items.
    /// </summary>
    /// <param name="items">The collection of items.</param>
    /// <returns>The aggregate value.</returns>
    protected abstract object CalculateCore(IEnumerable items);

    /// <summary>
    /// Gets the result of the aggregate.
    /// </summary>
    public object Result
    {
      get { return _result; }
    }

    internal IPropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
      set
      {
        _propertyInfo = value;
      }
    }

    /// <summary>
    /// Returns a value from the given item based on the property info.
    /// </summary>
    /// <param name="item">A data object.</param>
    /// <returns>A value extracted from the given data object.</returns>
    protected object GetValue(object item)
    {
      if (_propertyInfo != null)
      {
        return _propertyInfo.GetValue(item, System.Reflection.BindingFlags.Default, null, null, CultureInfo.CurrentCulture);
      }
      return item;
    }
  }
}
