using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// A property grid entry representing an indexed property -- specifically an item in a list or
  /// dictionary.
  /// </summary>
  public class CollectionElement : Node
  {
    internal static readonly IEnumerable<CollectionElement> ParentIsNotACollection = new CollectionElement[0];

    /// <summary>
    /// Creates <see cref="CollectionElement"/> objects for each item in a dictionary.
    /// </summary>
    /// <param name="source">The dictionary.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    /// <returns>A <see cref="CollectionElement"/> for each item in the dictionary.</returns>
    public static IEnumerable<CollectionElement> FromDictionary(IDictionary source, Predicate<Node> childFilter)
    {
      CollectionTypeOperations typeOperations = CollectionTypeOperations.ForDictionary(source);

      foreach (object key in source.Keys)
      {
        yield return new CollectionElement(source, typeOperations, key, true, childFilter);
      }
    }

    internal static IEnumerable<CollectionElement> FromExpandObject(IDictionary<string, object> source, Predicate<Node> childFilter)
    {
      CollectionTypeOperations typeOperations = CollectionTypeOperations.ForDictionary(source);

      foreach (object key in source.Keys)
      {
        yield return new CollectionElement(source, typeOperations, key, true, childFilter);
      }
    }

    /// <summary>
    /// Creates <see cref="CollectionElement"/> objects for each item in a list.
    /// </summary>
    /// <param name="source">The list.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    /// <returns>A <see cref="CollectionElement"/> for each item in the list.</returns>
    public static IEnumerable<CollectionElement> FromList(IList source, Predicate<Node> childFilter)
    {
      CollectionTypeOperations typeOperations = CollectionTypeOperations.ForList(source);

      for (int i = 0; i < source.Count; ++i)
      {
        yield return new CollectionElement(source, typeOperations, i, false, childFilter);
      }
    }

    private static IEnumerable<CollectionElement> FromGenericList(object source, Predicate<Node> childFilter)
    {
      CollectionTypeOperations typeOperations = CollectionTypeOperations.ForList(source);

      IEnumerable enumerable = source as IEnumerable;
      int index = 0;
      foreach (object o in enumerable)
      {
        yield return new CollectionElement(source, typeOperations, index, false, childFilter);
        index++;
      }
    }

    /// <summary>
    /// Creates <see cref="CollectionElement"/> objects for each item in a collection.
    /// </summary>
    /// <param name="source">The source collection - a list or dictionary.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    /// <returns>A <see cref="CollectionElement"/> for each item in the list.  If the source is not
    /// a list or dictionary, no CollectionElements are returned.</returns>
    public static IEnumerable<CollectionElement> GetCollectionElements(object source, Predicate<Node> childFilter)
    {
      Many many = source as Many;
      if (many != null && many.IsConsistent)
      {
        return GetCollectionElements(many.RawValue, childFilter);
      }

      IList list = source as IList;
      if (list != null)
      {
        return FromList(list, childFilter);
      }

      IDictionary<string, object> expandoObject = source as IDictionary<string, object>;
      if (expandoObject != null)
      {
        return FromExpandObject(expandoObject, childFilter);
      }

      if (source != null && TypeUtilities.IsGenericCollection(source.GetType()))
      {
        return FromGenericList(source, childFilter);
      }

      IDictionary dictionary = source as IDictionary;
      if (dictionary != null)
      {
        return FromDictionary(dictionary, childFilter);
      }

      return ParentIsNotACollection;
    }

    private static bool IsReadOnlyCollection(object source)
    {
      Many many = source as Many;
      if (many != null && many.IsConsistent)
      {
        return IsReadOnlyCollection(many.RawValue);
      }

      IList list = source as IList;
      if (list != null)
      {
        return list.IsReadOnly;
      }

      if (source != null && TypeUtilities.IsGenericCollection(source.GetType()))
      {
        PropertyInfo info = source.GetType().GetProperty("IsReadOnly"); // We check this for null because ExpandoObject does not have this property.
        return info == null ? true : (bool)info.GetValue(source, null);
      }

      IDictionary dictionary = source as IDictionary;
      if (dictionary != null)
      {
        return dictionary.IsReadOnly;
      }

      return true;
    }

    private readonly bool _useIndexAsFullDisplayName;
    private readonly object _index;
    private readonly CollectionTypeOperations _typeOperations;
    private readonly bool _isReadOnlyCollection;

    /// <summary>
    /// Initialises a new instance of the <see cref="CollectionElement"/> class.
    /// </summary>
    /// <param name="source">The object whose collection property is being represented.</param>
    /// <param name="typeOperations">The collection methods and properties for operating on this node.</param>
    /// <param name="index">The index or key in the collection of this element.</param>
    /// <param name="useIndexAsFullDisplayName">If true, the index alone is used as the display name;
    /// if false, the property name and index are combined to create a display name.  (The former is
    /// typically the best option for dictionary-type collections, where the keys may be meaningful
    /// in themselves; the latter is typically better for list-type collections, where the keys
    /// are plain integers.)</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    public CollectionElement(object source, CollectionTypeOperations typeOperations, object index, bool useIndexAsFullDisplayName, Predicate<Node> childFilter)
      : base(source, typeOperations.ItemProperty, childFilter)
    {
      _index = index;
      _useIndexAsFullDisplayName = useIndexAsFullDisplayName;
      _typeOperations = typeOperations;
      _isReadOnlyCollection = IsReadOnlyCollection(source);

      INotifyCollectionChanged notifier = source as INotifyCollectionChanged;
      if (notifier != null)
      {
        notifier.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
          {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
              OnPropertyChanged("Value");
            }
          };
      }
    }

    internal override Node Clone()
    {
      return new CollectionElement(Source, _typeOperations, _index, _useIndexAsFullDisplayName, ChildFilter);
    }

    /// <summary>
    /// Gets whether the collection element can be modified.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        return !_isReadOnlyCollection;
      }
    }

    /// <summary>
    /// Gets the value of the collection element.
    /// </summary>
    public override object Value
    {
      get { return Property.GetValue(Source, BindingFlags.Default, null, new object[] { _index }, null); }
    }

    /// <summary>
    /// Gets the type of the collection element.
    /// </summary>
    public override Type PropertyType
    {
      get
      {
        object value = Value;
        if (value == null)
        {
          return CollectionUtilities.GetCollectionValueType(Source);
        }
        else
        {
          return value.GetType();
        }
      }
    }

    /// <summary>
    /// Gets the index(es) of the collection element.
    /// </summary>
    public override IList<object> IndexedPropertyArguments
    {
      get { return new object[] { _index }; }
    }

    /// <summary>
    /// Gets a display name for the collection element.
    /// </summary>
    public override string HumanName
    {
      get
      {
        if (_useIndexAsFullDisplayName)
        {
          return _index.ToString();
        }
        else
        {
            var propertyName = Property.Name;
            if (propertyName == CollectionTypeOperations.collectionItem)
                propertyName = Properties.Resources.CollectionItem;
            else if (propertyName == CollectionTypeOperations.collectionRemove)
                propertyName = Properties.Resources.CollectionRemove;
            else if (propertyName == CollectionTypeOperations.collectionRemoveAt)
                propertyName = Properties.Resources.CollectionRemoveAt;
            return String.Format(CultureInfo.CurrentCulture, "{0}[{1}]", propertyName, _index);
        }
      }
    }

    /// <summary>
    /// Removes the element represented by this node from the collection of
    /// which it is a part.
    /// </summary>
    public void RemoveFromParentCollection()
    {
      try
      {
        UnhookCollectionChangedHandlers();  // TODO: would prefer this to happen when the node notices the collection has changed, so as to handle data-level changes in the collection and to centralise event hooking logic in the Node class
        _typeOperations.RemovalMethod.Invoke(Source, new List<object>(IndexedPropertyArguments).ToArray());
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException is ArgumentNullException)
        {
          // issue with trying to remove nulls; ignore
        }
        else
        {
          throw;
        }
      }
    }

    private void UnhookCollectionChangedHandlers()
    {
      if (Parent != null)
      {
        foreach (var child in Parent.Children)
        {
          INotifyCollectionChanged notifier = child.Value as INotifyCollectionChanged;
          if (notifier != null)
          {
            CollectionChangedEventManager.RemoveListener(notifier, child);
          }
        }
      }
    }

    /// <summary>
    /// Determines whether the element represented by this node can be removed
    /// from the collection of which it is a part.
    /// </summary>
    /// <returns>true if the element can be removed from its parent collection;
    /// otherwise false.</returns>
    public bool CanRemoveFromParentCollection()
    {
      return CollectionUtilities.CanRemoveFromCollection(Source);
    }
  }

  /// <summary>
  /// Represents standard operations on a collection type.
  /// </summary>
  public class CollectionTypeOperations
  {
    internal static string collectionItem = "Item";
    internal static string collectionRemove = "Remove";
    internal static string collectionRemoveAt = "RemoveAt";

    private readonly PropertyInfo _itemProperty;
    private readonly MethodInfo _removalMethod;

    /// <summary>
    /// Gets the property for accessing items using their indexes.
    /// </summary>
    public PropertyInfo ItemProperty
    {
      get { return _itemProperty; }
    }

    /// <summary>
    /// Gets the method for removing items using their indexes.
    /// </summary>
    public MethodInfo RemovalMethod
    {
      get { return _removalMethod; }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="CollectionTypeOperations"/> class.
    /// </summary>
    /// <param name="type">The collection type.</param>
    /// <param name="itemProperty">The name of the property used to access items by index.</param>
    /// <param name="removalMethod">The name of the method used to remove items by index.</param>
    public CollectionTypeOperations(Type type, string itemProperty, string removalMethod)
    {
      _itemProperty = type.GetProperty(itemProperty);
      _removalMethod = type.GetMethod(removalMethod);
    }

    /// <summary>
    /// Gets the <see cref="CollectionTypeOperations"/> for the <see cref="IDictionary"/> type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Type is immutable")]
    public static readonly CollectionTypeOperations Dictionary =
      new CollectionTypeOperations(typeof(IDictionary), collectionItem, collectionRemove);

    /// <summary>
    /// Gets the <see cref="CollectionTypeOperations"/> for the <see cref="IList"/> type.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
    public static readonly CollectionTypeOperations List =
      new CollectionTypeOperations(typeof(IList), collectionItem, collectionRemoveAt);

    internal static CollectionTypeOperations ListT(Type t)
    {
      return new CollectionTypeOperations(typeof(IList<>).MakeGenericType(t), collectionItem, collectionRemoveAt);
    }

    internal static CollectionTypeOperations DictionaryT(Type key, Type value)
    {
      return new CollectionTypeOperations(typeof(IDictionary<,>).MakeGenericType(key, value), collectionItem, collectionRemove);
    }

    internal static CollectionTypeOperations ForList(object source)
    {
      foreach (Type itf in source.GetType().GetInterfaces())
      {
        if (itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IList<>))
        {
          return CollectionTypeOperations.ListT(itf.GetGenericArguments()[0]);
        }
      }

      return CollectionTypeOperations.List;
    }

    internal static CollectionTypeOperations ForDictionary(object source)
    {
      foreach (Type itf in source.GetType().GetInterfaces())
      {
        if (itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IDictionary<,>))
        {
          return CollectionTypeOperations.DictionaryT(itf.GetGenericArguments()[0], itf.GetGenericArguments()[1]);
        }
      }

      return CollectionTypeOperations.Dictionary;
    }
  }
}
