using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace Mindscape.WpfElements.PropertyEditing
{
  internal class DelegatingPropertyInfo : PropertyInfo
  {
    private IPropertyInfo _impl;

    public DelegatingPropertyInfo(IPropertyInfo impl)
    {
      Invariant.ArgumentNotNull(impl, "impl");

      _impl = impl;
    }

    public override PropertyAttributes Attributes
    {
      get { return _impl.Attributes; }
    }

    public override bool CanRead
    {
      get { return _impl.CanRead; }
    }

    public override bool CanWrite
    {
      get { return _impl.CanWrite; }
    }

    public override MethodInfo[] GetAccessors(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetGetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override ParameterInfo[] GetIndexParameters()
    {
      return _impl.GetIndexParameters();
    }

    public override MethodInfo GetSetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      return _impl.GetValue(obj, invokeAttr, binder, index, culture);
    }

    public override Type PropertyType
    {
      get { return _impl.PropertyType; }
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      _impl.SetValue(obj, value, invokeAttr, binder, index, culture);
    }

    public override Type DeclaringType
    {
      get { return _impl.DeclaringType; }
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      return _impl.GetCustomAttributes(attributeType, inherit);
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      return _impl.GetCustomAttributes(inherit);
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      return _impl.IsDefined(attributeType, inherit);
    }

    public override string Name
    {
      get { return _impl.Name; }
    }

    public override Type ReflectedType
    {
      get { return _impl.DeclaringType; }
    }
  }
}
