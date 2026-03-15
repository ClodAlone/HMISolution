using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;

namespace Mindscape.WpfElements.PropertyEditing
{
  internal class DescriptorPropertyInfoAdapter : IPropertyInfo
  {
    private PropertyDescriptor _impl;

    public DescriptorPropertyInfoAdapter(PropertyDescriptor property)
    {
      Invariant.ArgumentNotNull(property, "property");

      _impl = property;
    }

    public PropertyAttributes Attributes
    {
      get { return PropertyAttributes.None; }
    }

    public bool CanRead
    {
      get { return true; }
    }

    public bool CanWrite
    {
      get { return !_impl.IsReadOnly; }
    }

    public ParameterInfo[] GetIndexParameters()
    {
      return new ParameterInfo[0];
    }

    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      if (index != null && index.Length > 0)
      {
        throw new TargetParameterCountException("This property does not support indexed access.");
      }

      if (obj is ICustomTypeDescriptor) // TODO: write tests for this
      {
        obj = (obj as ICustomTypeDescriptor).GetPropertyOwner(_impl);
      }

      return _impl.GetValue(obj);
    }

    public Type PropertyType
    {
      get { return _impl.PropertyType; }
    }

    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      if (index != null && index.Length > 0)
      {
        throw new TargetParameterCountException("This property does not support indexed access.");
      }

      if (obj is ICustomTypeDescriptor) // TODO: write tests for this
      {
        obj = (obj as ICustomTypeDescriptor).GetPropertyOwner(_impl);
      }

      _impl.SetValue(obj, value);
    }

    public Type DeclaringType
    {
      get { return _impl.ComponentType; }
    }

    public object[] GetCustomAttributes(bool inherit)
    {
      AttributeCollection attrs = _impl.Attributes;
      Attribute[] attributeArray = new Attribute[attrs.Count];
      attrs.CopyTo(attributeArray, 0);
      return attributeArray;
    }

    public object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      AttributeCollection attrs = _impl.Attributes;
      List<Attribute> attrsOfType = new List<Attribute>();
      foreach (Attribute attr in attrs)
      {
        if (attributeType.IsInstanceOfType(attr))
        {
          attrsOfType.Add(attr);
        }
      }
      return attrsOfType.ToArray();
    }

    public bool IsDefined(Type attributeType, bool inherit)
    {
      AttributeCollection attrs = _impl.Attributes;
      foreach (Attribute attr in attrs)
      {
        if (attributeType.IsInstanceOfType(attr))
        {
          return true;
        }
      }
      return false;
    }

    public string Name
    {
      get { return _impl.Name; }
    }

    public TypeConverter Converter
    {
      get { return _impl.Converter; }
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
      get { return _impl; }
    }

    public string DisplayName
    {
      get { return _impl.DisplayName; }
    }

    public string Category
    {
      get { return _impl.Category; }
    }

    public string Description
    {
      get { return _impl.Description; }
    }

    public bool IsBrowsable
    {
      get { return _impl.IsBrowsable; }
    }
  }

}
