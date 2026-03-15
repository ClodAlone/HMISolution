using System.Reflection;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides helper methods for working with reflection.
  /// </summary>
  public static class ReflectionUtilities
  {
    /// <summary>
    /// Determines whether a member is browsable (i.e. does not have a <see cref="BrowsableAttribute"/>
    /// with the Browsable property set to false).
    /// </summary>
    /// <param name="member">The member to check.</param>
    /// <returns>true if the member is browsable (does not have the BrowsableAttribute, or has
    /// BrowsableAttribute.Yes); false if the member is not browsable.</returns>
    public static bool IsBrowsable(ICustomAttributeProvider member)
    {
      object[] browsableAttribute = member.GetCustomAttributes(typeof(BrowsableAttribute), false);
      return (browsableAttribute == null || 
        browsableAttribute.Length == 0 || 
        BrowsableAttribute.Yes.Equals(browsableAttribute[0]));
    }

    internal static T GetAttribute<T>(ICustomAttributeProvider member, bool inherit)
      where T : Attribute
    {
      object[] attrs = member.GetCustomAttributes(typeof(T), inherit);
      if (attrs.Length > 0)
      {
        return (T)attrs[0];
      }
      return null;
    }

    internal static bool ShouldUseStandardValues(TypeConverter converter, Type dataType)
    {
      // The default type converter for interfaces is a ReferenceConverter which returns
      // true from GetStandardValuesSupported.  This confuses us because we interpret
      // that to mean we should present a drop-down list, and confuses us further because
      // we further interpret that to mean the node is in-place editable and therefore shouldn't
      // be shown as expandable.  So we exclude this case.
      //
      // Note that the test is for type equality to ReferenceConverter: it's entirely legitimate
      // for a component creator to create their own type converter derived from ReferenceConverter,
      // and if they do that, we should respect it.

      if (converter == null)
      {
        return false;
      }

      bool converterSupportsStandardValues = converter.GetStandardValuesSupported();
      bool isFishingForInterfaceInstances = ((converter.GetType() == typeof(ReferenceConverter)) && dataType.IsInterface);

      return converterSupportsStandardValues && !isFishingForInterfaceInstances;
    }

    internal static PropertyDescriptorCollection GetProperties(object value)
    {
      if (value == null)
      {
        return PropertyDescriptorCollection.Empty;
      }

      if (value is MultipleObjectWrapper)
      {
        return TypeDescriptor.GetProperties(value);
      }

      TypeConverter typeConverter = TypeDescriptor.GetConverter(value);
      if (typeConverter != null && typeConverter.GetPropertiesSupported())
      {
        return typeConverter.GetProperties(value);
      }

      return TypeDescriptor.GetProperties(value);
    }

    internal static PropertyDescriptorCollection GetProperties(object value, Attribute[] attributes)
    {
      if (value == null)
      {
        return PropertyDescriptorCollection.Empty;
      }

      if (value is MultipleObjectWrapper)
      {
        return TypeDescriptor.GetProperties(value, attributes);
      }

      TypeConverter typeConverter = TypeDescriptor.GetConverter(value);
      if (typeConverter != null && typeConverter.GetPropertiesSupported())
      {
        return typeConverter.GetProperties(null, value, attributes);
      }

      return TypeDescriptor.GetProperties(value, attributes);
    }

    internal static PropertyDescriptor Find(PropertyDescriptorCollection pdc, PropertyDescriptor pd)
    {
      int index = pdc.IndexOf(pd);
      if (index >= 0)
      {
        return pdc[index];  // This may actually be a different PropertyDescriptor from pd (e.g. different attributes), so we *DON'T* just return pd.  This is the "equivalent" to pd within pdc, and will have correct attrs etc.
      }
      return pdc[pd.Name];
    }
  }
}
