using System;
using System.Collections.Generic;
using System.Collections;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides helper methods for working with types.
  /// </summary>
  public static class TypeUtilities
  {
    /// <summary>
    /// Determines whether a type is a generic collection type.
    /// </summary>
    /// <param name="propertyType">The type to test.</param>
    /// <returns>true if the type is a (closed constructed) generic collection type; 
    /// otherwise false.</returns>
    public static bool IsGenericCollection(Type propertyType)
    {
      if (!typeof(IEnumerable).IsAssignableFrom(propertyType))
      {
        return false;
      }

      if (propertyType.IsGenericType)
      {
        Type[] genericTypeParameters = propertyType.GetGenericArguments();
        if (genericTypeParameters.Length == 1)
        {
          Type itemType = genericTypeParameters[0];
          Type collectionInterface = (typeof(ICollection<>)).MakeGenericType(itemType);
          if (collectionInterface.IsAssignableFrom(propertyType))
          {
            return true;
          }
        }
      }
      else if (!propertyType.IsArray)
      {
        Type[] interfaces = propertyType.FindInterfaces((t, c) => true, null);
        foreach (Type interfaceType in interfaces)
        {
          Type[] genericTypeParameters = interfaceType.GetGenericArguments();
          if (genericTypeParameters.Length == 1)
          {
            Type itemType = genericTypeParameters[0];
            Type collectionInterface = (typeof(ICollection<>)).MakeGenericType(itemType);
            if (collectionInterface.IsAssignableFrom(propertyType))
            {
              return true;
            }
          }
        }
      }

      return IsGenericCollection(propertyType.BaseType);
    }
  }
}
