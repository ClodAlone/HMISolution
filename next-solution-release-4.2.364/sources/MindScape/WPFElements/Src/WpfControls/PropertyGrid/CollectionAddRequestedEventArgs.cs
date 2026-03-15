using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Provides information about the <see cref="PropertyGrid.CollectionAddRequested" /> event of a
  /// <see cref="PropertyGrid" />.
  /// </summary>
  public class CollectionAddRequestedEventArgs : RoutedEventArgs
  {
    internal CollectionAddRequestedEventArgs(Type collectionElementType)
      : base(PropertyGrid.CollectionAddRequestedEvent)
    {
      _collectionElementType = collectionElementType;
    }

    private readonly Type _collectionElementType;
    private object _value;
    private bool _valueSet;

    /// <summary>
    /// Gets the element type of the collection.
    /// </summary>
    public Type CollectionElementType
    {
      get { return _collectionElementType; }
    }

    /// <summary>
    /// Gets or sets the value to be added to the collection.
    /// </summary>
    public object Value
    {
      get { return _value; }
      set
      {
        _value = value;
        _valueSet = true;
      }
    }

    internal bool ValueSet
    {
      get { return _valueSet; }
    }
  }
}
