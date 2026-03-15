using System;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  internal class PassthroughPropertyInfoAdapter : IPropertyInfo
  {
    private PropertyInfo _impl;

    public PassthroughPropertyInfoAdapter(PropertyInfo impl)
    {
      Invariant.ArgumentNotNull(impl, "impl");

      _impl = impl;
    }

    public PropertyAttributes Attributes
    {
      get { return _impl.Attributes; }
    }

    public bool CanRead
    {
      get { return _impl.CanRead; }
    }

    public bool CanWrite
    {
      get { return _impl.CanWrite; }
    }

    public ParameterInfo[] GetIndexParameters()
    {
      return _impl.GetIndexParameters();
    }

    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      return _impl.GetValue(obj, invokeAttr, binder, index, culture);
    }

    public Type PropertyType
    {
      get { return _impl.PropertyType; }
    }

    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      _impl.SetValue(obj, value, invokeAttr, binder, index, culture);
    }

    public Type DeclaringType
    {
      get { return _impl.DeclaringType; }
    }

    public object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      return _impl.GetCustomAttributes(attributeType, inherit);
    }

    public object[] GetCustomAttributes(bool inherit)
    {
      return _impl.GetCustomAttributes(inherit);
    }

    public bool IsDefined(Type attributeType, bool inherit)
    {
      return _impl.IsDefined(attributeType, inherit);
    }

    public string Name
    {
      get { return _impl.Name; }
    }

    public TypeConverter Converter
    {
      get
      {
        if (_impl.IsDefined(typeof(TypeConverterAttribute), true))
        {
          TypeConverterAttribute attr = _impl.GetCustomAttributes(typeof(TypeConverterAttribute), true)[0] as TypeConverterAttribute;
          Type type = Type.GetType(attr.ConverterTypeName, false, true);
          if (type != null)
          {
            TypeConverter converter = (TypeConverter)(Activator.CreateInstance(type));
            return converter;
          }
        }

        return TypeDescriptor.GetConverter(_impl.PropertyType);
      }
    }

    public PropertyInfo AsPropertyInfo
    {
      get { return _impl; }
    }

    private PropertyDescriptor _asDescriptor;

    public PropertyDescriptor AsPropertyDescriptor
    {
      get
      {
        EnsureDescriptorInitialised();
        return _asDescriptor;
      }
    }

    private void EnsureDescriptorInitialised()
    {
      if (_asDescriptor == null)
      {
        _asDescriptor = TypeDescriptor.GetProperties(PropertyType)[Name];
      }
    }

    public string DisplayName
    {
      get
      {
        DisplayNameAttribute attr = ReflectionUtilities.GetAttribute<DisplayNameAttribute>(_impl, true);
        if (attr != null)
        {
          return attr.DisplayName;
        }
        return StringUtils.Humanize(Name);
      }
    }

    public string Category
    {
      get
      {
        CategoryAttribute attr = ReflectionUtilities.GetAttribute<CategoryAttribute>(_impl, true);
        if (attr != null)
        {
          return attr.Category;
        }
        return null;        
      }
    }

    public string Description
    {
      get
      {
        DescriptionAttribute attr = ReflectionUtilities.GetAttribute<DescriptionAttribute>(_impl, true);
        if (attr != null)
        {
          return attr.Description;
        }
        return null;        
      }
    }

    public bool IsBrowsable
    {
      get { return ReflectionUtilities.IsBrowsable(_impl); }
    }
  }

}
