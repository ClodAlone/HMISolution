using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.PropertyEditing;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  internal class DataTablePropertyInfoAdapter : IPropertyInfo
  {
    private readonly DataColumn _column;

    public DataTablePropertyInfoAdapter(DataColumn column)
    {
      Invariant.ArgumentNotNull(column, "column");

      _column = column;
    }

    public System.Reflection.PropertyAttributes Attributes
    {
      get { return System.Reflection.PropertyAttributes.None; }
    }

    public bool CanRead
    {
      get { return true; }
    }

    public bool CanWrite
    {
      get { return !_column.ReadOnly; }
    }

    public ParameterInfo[] GetIndexParameters()
    {
      return new ParameterInfo[0];
    }

    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      DataRowView row = obj as DataRowView;
      if (row != null)
      {
        object value = row[_column.ColumnName];
        if (PropertyType.IsEnum)
        {
          string str = value.ToString();
          if (!"".Equals(str))
          {
            value = Enum.Parse(PropertyType, str);
          }
        }
        return value;
      }
      return null;
    }

    public Type PropertyType
    {
      get { return _column.DataType; }
    }

    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      DataRowView row = obj as DataRowView;
      if (row != null)
      {
        row[_column.ColumnName] = value;
      }
    }

    public Type DeclaringType
    {
      get { return typeof(DataRowView); }
    }

    public object[] GetCustomAttributes(bool inherit)
    {
      return new Attribute[0];
    }

    public object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      List<Attribute> attrsOfType = new List<Attribute>();
      return attrsOfType.ToArray();
    }

    public bool IsDefined(Type attributeType, bool inherit)
    {
      return false;
    }

    public string Name
    {
      get { return _column.ColumnName; }
    }

    public TypeConverter Converter
    {
      get { return TypeDescriptor.GetConverter(PropertyType); }
    }

    private PropertyInfo _asPropertyInfo;

    public PropertyInfo AsPropertyInfo
    {
      get
      {
        EnsurePropertyInfoInitialised();
        return _asPropertyInfo;
      }
    }

    private void EnsurePropertyInfoInitialised()
    {
      if (_asPropertyInfo == null)
      {
        _asPropertyInfo = new DelegatingPropertyInfo(this);
      }
    }

    public PropertyDescriptor AsPropertyDescriptor
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public string DisplayName
    {
      get { return _column.ColumnName; }
    }

    public string Category
    {
      get { throw new NotSupportedException(); }
    }

    public string Description
    {
      get { throw new NotSupportedException(); }
    }

    public bool IsBrowsable
    {
      get { return true; }
    }
  }
}
