using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using System.ComponentModel;
using System.Collections;
using System.Linq;

namespace Mindscape.WpfElements.Design
{
  internal class ToolboxEntry
  {
    private readonly Type _componentType;
    private readonly string _description;
    private readonly IEnumerable _nonBrowsableProperties;
    private readonly IEnumerable<PropertyCategory> _categories;

    internal ToolboxEntry(Type componentType, string description,
      IEnumerable nonBrowsableProperties,
      IEnumerable<PropertyCategory> categories)
    {
      _componentType = componentType;
      _description = description;
      _nonBrowsableProperties = nonBrowsableProperties;
      _categories = categories;
    }

    internal void AddTo(AttributeTableBuilder builder)
    {
      builder.AddCustomAttributes(_componentType, new ToolboxBrowsableAttribute(true));
      builder.AddCustomAttributes(_componentType, new DescriptionAttribute(_description));

      var properties = _nonBrowsableProperties.Cast<object>()
                                              .Select(o => new { OwnerType = Utils.GetOwnerType(o), PropertyName = Utils.GetPropertyName(o) });
      foreach (var property in properties)
      {
        builder.AddCustomAttributes(property.OwnerType ?? _componentType, property.PropertyName, new BrowsableAttribute(false));
      }

      foreach (PropertyCategory category in _categories)
      {
        category.AddTo(builder, _componentType);
      }
    }

    internal Type ComponentType
    {
      get { return _componentType; }
    }
  }

  internal class PropertyCategory
  {
    private readonly string _category;
    private readonly IEnumerable _properties;

    internal PropertyCategory(string category, IEnumerable properties)
    {
      _category = category;
      _properties = properties;
    }

    internal void AddTo(AttributeTableBuilder builder, Type componentType)
    {
      var propertyNames = _properties.Cast<object>()
                                     .Select(o => Utils.GetPropertyName(o));

      foreach (string name in propertyNames)
      {
        builder.AddCustomAttributes(componentType, name, new CategoryAttribute(_category));
      }
    }
  }

  internal static class Utils
  {
    internal static Type GetOwnerType(object o)
    {
      DependencyProperty dp = o as DependencyProperty;
      return dp == null ? null : dp.OwnerType;
    }

    internal static string GetPropertyName(object o)
    {
      DependencyProperty dp = o as DependencyProperty;
      return dp == null ? o.ToString() : dp.Name;
    }
  }
}
