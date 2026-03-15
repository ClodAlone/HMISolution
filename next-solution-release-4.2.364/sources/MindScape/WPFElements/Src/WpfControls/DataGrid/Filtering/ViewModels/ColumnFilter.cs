using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;
using System.Collections;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Links a <see cref="DataGridColumn"/> to an <see cref="IFilterDescription"/>.
  /// </summary>
  public class ColumnFilter : ViewModelBase
  {
    private readonly DataGridColumn _column;
    private readonly bool _useBooleanFilter;
    private IFilterDescription _description;

    private bool _lock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnFilter"/> class.
    /// </summary>
    /// <param name="column">The <see cref="DataGridColumn"/> to link with.</param>
    /// <param name="useBooleanFilter">Whether or not to use a boolean filter.</param>
    internal ColumnFilter(DataGridColumn column, bool useBooleanFilter)
    {
      _useBooleanFilter = useBooleanFilter;
      _column = column;
      _column.FilterChanged += new EventHandler(Column_FilterChanged);

      BuildDescription();
    }

    private void BuildDescription()
    {
      _lock = true;
      IFilterDescription firstFilter = null;
      IFilterDescription secondFilter = null;
      if (_column != null && _column.PropertyInfo != null)
      {
        // String values
        Type type = _column.PropertyInfo.PropertyType;
        if (type == typeof(string))
        {
          firstFilter = new StringFilterDescription();
          secondFilter = new StringFilterDescription();
        }

        // Enum values
        TypeConverter converter = _column.PropertyInfo.Converter;
        if (converter == null && type != null)
        {
          converter = TypeDescriptor.GetConverter(type);
        }
        if (ReflectionUtilities.ShouldUseStandardValues(converter, type))
        {
          EnumValuesConverter enumValuesConverter = new EnumValuesConverter();
          IList values = enumValuesConverter.Convert(_column.PropertyInfo, typeof(IList), null, CultureInfo.CurrentCulture) as IList;
          _description = new ObjectFilterDescription(_column.PropertyInfo, values);
          _description.FilterChanged += new EventHandler(Description_FilterChanged);
          if (_column.Filter != null)
          {
            _description.SetAs(_column.Filter);
          }
          _lock = false;
          return;
        }

        // Comparable values
        TypeCode code = Type.GetTypeCode(type);
        if (code == TypeCode.Int32 || code == TypeCode.Int64)
        {
          firstFilter = new ComparableFilterDescription(typeof(int));
          secondFilter = new ComparableFilterDescription(typeof(int));
        }
        else if (code == TypeCode.Decimal)
        {
          firstFilter = new ComparableFilterDescription(typeof(decimal));
          secondFilter = new ComparableFilterDescription(typeof(decimal));
        }
        else if (code == TypeCode.Double)
        {
          firstFilter = new ComparableFilterDescription(typeof(double));
          secondFilter = new ComparableFilterDescription(typeof(double));
        }
        else if (code == TypeCode.DateTime)
        {
          firstFilter = new ComparableFilterDescription(typeof(DateTime));
          secondFilter = new ComparableFilterDescription(typeof(DateTime));
        }
      }

      if (firstFilter == null)
      {
        _description = null;
      }
      else
      {
        if (_useBooleanFilter)
        {
          _description = new BooleanFilterDescription(firstFilter, secondFilter);
        }
        else
        {
          _description = firstFilter;
        }
        _description.FilterChanged += new EventHandler(Description_FilterChanged);
      }

      if (_column.Filter != null)
      {
        _description.SetAs(_column.Filter);
      }
      _lock = false;
    }

    private void Column_FilterChanged(object sender, EventArgs e)
    {
      if (!_lock)
      {
        _lock = true;
        _description.SetAs(_column.Filter);
        _lock = false;
      }
    }

    private void Description_FilterChanged(object sender, EventArgs e)
    {
      if (!_lock)
      {
        _lock = true;
        _column.Filter = _description.Filter;
        _lock = false;
      }
    }

    /// <summary>
    /// Gets the <see cref="IFilterDescription"/> used to create the filter for the <see cref="DataGridColumn"/>.
    /// </summary>
    public IFilterDescription Description
    {
      get { return _description; }
    }

    /// <summary>
    /// Gets the <see cref="DataGridColumn"/> that is linked to the filter.
    /// </summary>
    public DataGridColumn Column
    {
      get { return _column; }
    }
  }
}
