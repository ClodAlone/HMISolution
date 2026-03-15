using System.Collections.Generic;
using System;
using System.Collections;
using System.Windows.Data;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Provides helper methods for working with collections.
  /// </summary>
  public static class CollectionUtilities
  {
    /// <summary>
    /// Appends a set of items to a collection.
    /// </summary>
    /// <typeparam name="TCollection">The type of item in the collection being appended to.</typeparam>
    /// <typeparam name="TAppend">The type of item being appended.</typeparam>
    /// <param name="collection">The collection being appended to.</param>
    /// <param name="itemsToAdd">The items to be appended.</param>
    /// <remarks>This method is equivalent to the AddRange method available on various concrete
    /// collection types.</remarks>
    public static void Append<TCollection, TAppend>(ICollection<TCollection> collection, IEnumerable<TAppend> itemsToAdd)
      where TAppend : TCollection
    {
      foreach (TAppend obj in itemsToAdd)
      {
        collection.Add(obj);
      }
    }

    private static readonly Type TypeOfIEnumerable = typeof(IEnumerable);

    internal static bool IsEnumerable(Type type)
    {
      return TypeOfIEnumerable.IsAssignableFrom(type);
    }
    
    internal static Type GetCollectionValueType(object collectionInstance)
    {
      if (collectionInstance == null)
      {
        return null;
      }

      CollectionView view = collectionInstance as CollectionView;
      if (view != null && view.SourceCollection != null)
      {
        return GetValueType(view.SourceCollection.GetType());
      }

      return GetValueType(collectionInstance.GetType());
    }

    internal static Type GetValueType(Type collectionType)
    {
      if (!IsEnumerable(collectionType))
      {
        return null;
      }

      if (collectionType.IsGenericType)
      {
        Type[] typeParameters = collectionType.GetGenericArguments();
        if (typeParameters.Length == 1)
        {
          Type candidateType = typeof(ICollection<>).MakeGenericType(typeParameters);
          if (candidateType.IsAssignableFrom(collectionType))
          {
            return typeParameters[0];
          }
        }
      }
      else
      {
        Type[] interfaces = collectionType.FindInterfaces((t, c) => true, null);
        foreach (Type interfaceType in interfaces)
        {
          Type[] genericTypeParameters = interfaceType.GetGenericArguments();
          if (genericTypeParameters.Length == 1)
          {
            Type itemType = genericTypeParameters[0];
            Type collectionInterface = (typeof(ICollection<>)).MakeGenericType(itemType);
            if (collectionInterface.IsAssignableFrom(collectionType))
            {
              return genericTypeParameters[0];
            }
          }
        }
      }

      return GetValueType(collectionType.BaseType);
    }

    /// <summary>
    /// Adds an entry to a collection.  The new entry is a new instance of the collection
    /// value type (constructed using the default constructor), or null for reference types
    /// with no default constructor.
    /// </summary>
    /// <param name="collectionValue">The collection.</param>
    public static void AddDefaultValueEntry(object collectionValue)
    {
      Type t = GetCollectionValueType(collectionValue);
      object obj = null;
      if (t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null)
      {
        obj = Activator.CreateInstance(t);
      }
      if (collectionValue is IList)
      {
        ((IList)collectionValue).Add(obj);
      }
      else if (TypeUtilities.IsGenericCollection(collectionValue.GetType()))
      {
        collectionValue.GetType().GetMethod("Add").Invoke(collectionValue, new object[] { obj });
      }
    }

    /// <summary>
    /// Determines whether a collection can be added to using the <see cref="AddDefaultValueEntry"/>
    /// method.
    /// </summary>
    /// <param name="collectionValue">The collection.</param>
    /// <returns>true if it is possible to add values to this collection using
    /// <see cref="AddDefaultValueEntry"/>; otherwise false.</returns>
    public static bool CanAddToCollection(object collectionValue)
    {
      if (GetCollectionValueType(collectionValue) == null)
      {
        return false;
      }

      IList list = collectionValue as IList;
      if (list != null)
      {
        return !list.IsReadOnly && !list.IsFixedSize && !list.GetType().IsArray;
      }

      if (TypeUtilities.IsGenericCollection(collectionValue.GetType()))
      {
        return false.Equals(collectionValue.GetType().GetProperty("IsReadOnly").GetValue(collectionValue, null)) && !collectionValue.GetType().IsArray;
      }

      return false;
    }

    /// <summary>
    /// Determines whether elements can be removed from a collection.
    /// </summary>
    /// <param name="collectionValue">The collection.</param>
    /// <returns>true if it is possible to remove values from this collection;
    /// otherwise false.</returns>
    public static bool CanRemoveFromCollection(object collectionValue)
    {
      IList list = collectionValue as IList;
      if (list != null)
      {
        return !list.IsReadOnly && !list.IsFixedSize && !list.GetType().IsArray;
      }

      if (TypeUtilities.IsGenericCollection(collectionValue.GetType()))
      {
        PropertyInfo info = collectionValue.GetType().GetProperty("IsReadOnly"); // We check this for null because ExpandoObject does not have this property.
        return info == null ? true : false.Equals(info.GetValue(collectionValue, null)) && !collectionValue.GetType().IsArray;
      }

      IDictionary dictionary = collectionValue as IDictionary;
      if (dictionary != null)
      {
        return !dictionary.IsReadOnly;
      }

      return false;
    }

    internal static T GetFirstOrNull<T>(IList items) where T : class
    {
      if (items == null)
      {
        return null;
      }

      foreach (object item in items)
      {
        T row = item as T;
        if (row != null)
        {
          return row;
        }
      }

      return null;
    }

  }
}
