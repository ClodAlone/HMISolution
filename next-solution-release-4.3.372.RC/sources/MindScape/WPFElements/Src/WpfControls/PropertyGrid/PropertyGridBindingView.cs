using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Windows.Data;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Manages view state for the <see cref="PropertyGrid"/> control.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1710")]
  public sealed class PropertyGridBindingView : ObservableCollection<PropertyGridRow>
  {
    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyGridBindingView"/> class.
    /// </summary>
    /// <param name="properties">The set of nodes of the <see cref="PropertyGrid"/>.</param>
    public PropertyGridBindingView(ObservableCollection<Node> properties)
    {
      properties.CollectionChanged += OnPropertiesCollectionChanged;

      foreach (Node property in properties)
      {
        CreateRow(property, Count);
      }
    }

    /// <summary>
    /// Gets the default view of this collection.
    /// </summary>
    public ICollectionView DefaultView
    {
      get { return CollectionViewSource.GetDefaultView(this); }
    }

    private void OnPropertiesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (Node property in e.NewItems)
        {
          CreateRow(property, Count);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (Node node in e.OldItems)
        {
          RemoveRow(node);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        Clear();

        if (e.NewItems != null)
        {
          foreach (Node node in e.NewItems)
          {
            CreateRow(node, Count);
          }
        }
      }
    }

    private void CreateRow(Node property, int index)
    {
      PropertyGridRow propertyGridRow = new PropertyGridRow(property);

      Insert(index, propertyGridRow);
    }

    private void RemoveRow(Node node)
    {
      for (int i = 0; i < Count; ++i)
      {
        if (this[i].Node == node)
        {
          RemoveAt(i);
          break;  // don't continue to iterate after modifying the collection
        }
      }
    }
  }
}