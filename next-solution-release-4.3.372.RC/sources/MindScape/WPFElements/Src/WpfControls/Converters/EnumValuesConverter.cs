using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Collections;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Converts a value from an enumerated type into a set of permitted values.
  /// </summary>
  public class EnumValuesConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets whether or not the resulting collection includes an empty value.
    /// </summary>
    public bool HasEmptyValue { get; set; }

    /// <summary>
    /// Converts a value from a binding source for use by a binding target.
    /// </summary>
    /// <param name="value">The value for which a list of permitted values is required.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>An array of values compatible with the type of <paramref name="value"/>,
    /// suitable for use as the ItemsSource of a list or combo box.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TypeConverter converter;

      Node node = value as Node;
      if (node != null)
      {
        IPropertyInfo nodePropertyInfo = node.Property;

        var conversionContext = new EnumValueConversionContext(node.Source, nodePropertyInfo);

        if (nodePropertyInfo != null)
        {
          converter = nodePropertyInfo.Converter;
          if (converter != null && converter.GetStandardValuesSupported(conversionContext))
          {
            return GetResult(converter.GetStandardValues(conversionContext));
          }
        }

        converter = TypeDescriptor.GetConverter(node.PropertyType);
        if (converter != null && converter.GetStandardValuesSupported(conversionContext))
        {
          return GetResult(converter.GetStandardValues(conversionContext));
        }
      }

      IPropertyInfo propertyInfo = value as IPropertyInfo;
      if (propertyInfo != null)
      {
        converter = propertyInfo.Converter;
        if (converter != null && converter.GetStandardValuesSupported())
        {
          return GetResult(converter.GetStandardValues());
        }
      }

      Type type = value as Type;
      if (type != null)
      {
        converter = TypeDescriptor.GetConverter(type);
        if (converter != null && converter.GetStandardValuesSupported())
        {
          return GetResult(converter.GetStandardValues());
        }
      }

      if (value != null)
      {
        converter = TypeDescriptor.GetConverter(value);
        if (converter != null && converter.GetStandardValuesSupported())
        {
          return GetResult(converter.GetStandardValues());
        }
      }

      return new object[0];
    }

    private IList GetResult(ICollection collection)
    {
      List<object> list = new List<object>();
      if (HasEmptyValue)
      {
        list.Add("");
      }
      foreach (object o in collection)
      {
        list.Add(o);
      }
      return list;
    }

    /// <summary>
    /// Converts a value from a binding target for writing to a binding source.
    /// </summary>
    /// <param name="value">The value produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>This conversion direction is not implemented by this converter.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private class EnumValueConversionContext : ITypeDescriptorContext
    {
      private readonly object _instance;
      private readonly IPropertyInfo _propertyInfo;

      public EnumValueConversionContext(object instance, IPropertyInfo propertyInfo)
      {
        _instance = instance;
        _propertyInfo = propertyInfo;
      }

      public IContainer Container
      {
        get { return null; }
      }

      public object Instance
      {
        get { return _instance; }
      }

      public void OnComponentChanged()
      {
      }

      public bool OnComponentChanging()
      {
        return false;
      }

      public PropertyDescriptor PropertyDescriptor
      {
        get { return _propertyInfo == null ? null : _propertyInfo.AsPropertyDescriptor; }
      }

      public object GetService(Type serviceType)
      {
        return null;
      }
    }

  }
}
