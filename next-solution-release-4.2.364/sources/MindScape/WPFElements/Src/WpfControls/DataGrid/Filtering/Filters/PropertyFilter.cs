using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Applies an <see cref="IFilter"/> to a property rather than the object itself.
  /// </summary>
  public class PropertyFilter : IFilter
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly IFilter _filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyFilter"/> class.
    /// </summary>
    /// <param name="propertyInfo">Specifies the property to apply the <see cref="IFilter"/> to.</param>
    /// <param name="filter">The <see cref="IFilter"/> to apply to the property.</param>
    public PropertyFilter(PropertyInfo propertyInfo, IFilter filter)
    {
      _propertyInfo = propertyInfo;
      _filter = filter;
    }

    /// <summary>
    /// Gets the <see cref="PropertyInfo"/> that specifies which property to apply the <see cref="IFilter"/> to.
    /// </summary>
    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    /// <summary>
    /// Gets the <see cref="IFilter"/> to apply to the property.
    /// </summary>
    public IFilter Filter
    {
      get { return _filter; }
    }

    /// <summary>
    /// Returns true if the property value is a match for the filter.
    /// </summary>
    /// <param name="o">The object to check.</param>
    /// <returns>True if the specified property on the given object is a match for the filter.</returns>
    public bool IsMatch(object o)
    {
      // If PropertyInfo or Filter is null, then return true as this is an incomplete filter.
      if (_propertyInfo != null && _filter != null)
      {
        // null input returns false because null has no properties.
        if (o != null)
        {
          // check compatable types.
          if (!_propertyInfo.DeclaringType.Equals(o.GetType()))
          {
            return false;
          }
          object value = GetValue(o);
          return _filter.IsMatch(value);
        }
        return false;
      }
      return true;
    }

    private object GetValue(object o)
    {
      object value = _propertyInfo.GetValue(o, null);
      return value;
    }
  }
}
