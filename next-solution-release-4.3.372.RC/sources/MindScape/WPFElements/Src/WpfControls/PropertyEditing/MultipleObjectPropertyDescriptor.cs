using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  internal sealed class MultipleObjectPropertyDescriptor : PropertyDescriptor, IPropertyDescriptorWrapper
  {
    private PropertyDescriptor _property;
    private bool _isReadOnly;
    private bool _isBrowsable;

    public MultipleObjectPropertyDescriptor(PropertyDescriptor property, bool isReadOnly, bool isBrowsable)
      : base(property)
    {
      _property = property;
      _isReadOnly = isReadOnly;
      _isBrowsable = isBrowsable;
    }

    internal PropertyDescriptor UnderlyingProperty
    {
      get { return _property; }
    }

    public override bool CanResetValue(object component)
    {
      throw new NotImplementedException();
    }

    public override Type ComponentType
    {
      get { return _property.ComponentType; }
    }

    public override string DisplayName
    {
      get
      {
        return _property == null ? base.DisplayName : _property.DisplayName;
      }
    }

    public override string Description
    {
      get
      {
        return _property == null ? base.Description : _property.Description;
      }
    }

    public override string Category
    {
        get
        {
            return _property == null ? base.Category : _property.Category;
        }
    }

    public override object GetValue(object component)
    {
      MultipleObjectWrapper wrapper = (MultipleObjectWrapper)component;
      return wrapper.GetValue(this);
    }

    public override bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public override bool IsBrowsable
    {
      get { return _isBrowsable; }
    }

    public override Type PropertyType
    {
      get { return typeof(Many<>).MakeGenericType(_property.PropertyType); }
    }

    public override void ResetValue(object component)
    {
      throw new NotImplementedException();
    }

    public override void SetValue(object component, object value)
    {
      MultipleObjectWrapper wrapper = (MultipleObjectWrapper)component;
      wrapper.SetValue(this, value);
    }

    public override bool ShouldSerializeValue(object component)
    {
      throw new NotImplementedException();
    }

    #region IPropertyDescriptorWrapper Members

    PropertyDescriptor IPropertyDescriptorWrapper.PropertyDescriptor
    {
      get { return UnderlyingProperty; }
    }

    #endregion
  }
}
