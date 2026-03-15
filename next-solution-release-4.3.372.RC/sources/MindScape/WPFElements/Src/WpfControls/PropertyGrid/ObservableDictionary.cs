using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Represents a dictionary that provides notifications when items get added, removed,
  /// or when the whole list is refreshed.
  /// </summary>
  /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
  /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
  [Serializable]
  public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, System.Collections.IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
  {
    private const string ItemPropertyName = "Item[]";
    private const string CountPropertyName = "Count";

    private readonly SortedList<TKey, TValue> _impl = new SortedList<TKey, TValue>();

    /// <summary>
    /// Occurs when an item is added, removed, or changed, or the entire list is refreshed.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (CollectionChanged != null)
      {
        CollectionChanged(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The event data.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Adds an element with the provided key and value.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    public void Add(TKey key, TValue value)
    {
      _impl.Add(key, value);
      OnPropertyChanged(CountPropertyName);
      OnPropertyChanged(ItemPropertyName);

      NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), _impl.IndexOfKey(key));
      OnCollectionChanged(e);
    }

    /// <summary>
    /// Determines whether the collection contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>true if the collection contains an element with the key; otherwise false.</returns>
    public bool ContainsKey(TKey key)
    {
      return _impl.ContainsKey(key);
    }

    /// <summary>
    /// Gets a collection containing the keys of the dictionary.
    /// </summary>
    public ICollection<TKey> Keys
    {
      get { return _impl.Keys; }
    }

    /// <summary>
    /// Removes the element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>true if the element was removed from the collection; otherwise false.  The
    /// method returns false if the item was not present in the collection.</returns>
    public bool Remove(TKey key)
    {
      int index = _impl.IndexOfKey(key);

      if (index < 0)
      {
        return false;
      }

      KeyValuePair<TKey, TValue> kvp = new KeyValuePair<TKey, TValue>(key, this[key]);

      bool removed = _impl.Remove(key);

      if (removed)
      {
        OnPropertyChanged(CountPropertyName);
        OnPropertyChanged(ItemPropertyName);

        NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, kvp, index);
        OnCollectionChanged(e);
      }

      return removed;
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">When the method returns, contains the value associated with the
    /// specified key, if the key is found; otherwise, the default value for the type of the
    /// value parameter.</param>
    /// <returns>true if the dictionary contains an element with the specified key;
    /// otherwise false.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
      return _impl.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets a collection containing the values in the dictionary.
    /// </summary>
    public ICollection<TValue> Values
    {
      get { return _impl.Values; }
    }

    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get or set.</param>
    /// <returns>The element with the specified key.</returns>
    public TValue this[TKey key]
    {
      get
      {
        return _impl[key];
      }
      set
      {
        TValue existingValue;
        bool hadExistingValue = TryGetValue(key, out existingValue);

        if (!hadExistingValue)
        {
          Add(key, value);
        }

        if (hadExistingValue && !Equals(value, existingValue))
        {
          KeyValuePair<TKey, TValue> oldKvp = new KeyValuePair<TKey, TValue>(key, existingValue);
          KeyValuePair<TKey, TValue> newKvp = new KeyValuePair<TKey, TValue>(key, value);
          _impl[key] = value;
          OnPropertyChanged(ItemPropertyName);
          NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newKvp, oldKvp, _impl.IndexOfKey(key));
          OnCollectionChanged(e);
        }
      }
    }

    /// <summary>
    /// Adds the specified value to the collection with the specified key.
    /// </summary>
    /// <param name="item">A structure representing the key and value to add
    /// to the collection.</param>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all keys and values from the dictionary.
    /// </summary>
    public void Clear()
    {
      _impl.Clear(); 
      
      OnPropertyChanged(CountPropertyName);
      OnPropertyChanged(ItemPropertyName);

      NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
      OnCollectionChanged(e);
    }

    /// <summary>
    /// Determines whether the collection contains a specific key and value.
    /// </summary>
    /// <param name="item">The key-value pair to locate in the collection.</param>
    /// <returns>true if the key-value pair is found in the collection; otherwise false.</returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return ((ICollection<KeyValuePair<TKey, TValue>>)_impl).Contains(item);
    }

    /// <summary>
    /// Copies the elements of the collection to an array.
    /// </summary>
    /// <param name="array">The array to which to copy the elements of the collection.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> where copying starts.</param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<TKey, TValue>>)_impl).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of key-value pairs in the collection.
    /// </summary>
    public int Count
    {
      get { return _impl.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the dictionary is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return ((ICollection<KeyValuePair<TKey, TValue>>)_impl).IsReadOnly; }
    }

    /// <summary>
    /// Removes a key and value from the dictionary.
    /// </summary>
    /// <param name="item">The key and value to remove.</param>
    /// <returns>true if the element was removed from the collection; otherwise false.  The
    /// method returns false if the item was not present in the collection.</returns>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
      TValue existingValue;
      bool exists = _impl.TryGetValue(item.Key, out existingValue);

      if (exists && Equals(existingValue, item.Value))
      {
        return Remove(item.Key);
      }

      return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    /// <returns>An enumerator for the dictionary.</returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return _impl.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    /// <returns>An enumerator for the dictionary.</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #region Explicit implementation of non-generic collections

    void System.Collections.IDictionary.Add(object key, object value)
    {
      Add((TKey)key, (TValue)value);
    }

    bool System.Collections.IDictionary.Contains(object key)
    {
      return ((System.Collections.IDictionary)_impl).Contains(key);
    }

    System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
    {
      return ((System.Collections.IDictionary)_impl).GetEnumerator();
    }

    /// <summary>
    /// Gets whether the dictionary is fixed size.
    /// </summary>
    public bool IsFixedSize
    {
      get { return ((System.Collections.IDictionary)_impl).IsFixedSize; }
    }

    System.Collections.ICollection System.Collections.IDictionary.Keys
    {
      get { return ((System.Collections.IDictionary)_impl).Keys; }
    }

    void System.Collections.IDictionary.Remove(object key)
    {
      Remove((TKey)key);
    }

    System.Collections.ICollection System.Collections.IDictionary.Values
    {
      get { return ((System.Collections.IDictionary)_impl).Values; }
    }

    object System.Collections.IDictionary.this[object key]
    {
      get
      {
        return this[(TKey)key];
      }
      set
      {
        this[(TKey)key] = (TValue)value;
      }
    }

    void System.Collections.ICollection.CopyTo(System.Array array, int index)
    {
      ((System.Collections.ICollection)_impl).CopyTo(array, index);
    }

    /// <summary>
    /// Gets whether the dictionary is synchronized.
    /// </summary>
    public bool IsSynchronized
    {
      get { return ((System.Collections.ICollection)_impl).IsSynchronized; }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the dictionary.
    /// </summary>
    public object SyncRoot
    {
      get { return ((System.Collections.ICollection)_impl).SyncRoot; }
    }

    #endregion
  }
}
