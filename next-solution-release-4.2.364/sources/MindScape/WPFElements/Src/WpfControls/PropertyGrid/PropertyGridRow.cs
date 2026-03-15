using Mindscape.WpfElements.PropertyEditing;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Contains view state for a single row of a <see cref="PropertyGrid"/>.
  /// </summary>
  public sealed class PropertyGridRow : INotifyPropertyChanged
  {
    private readonly Node _node;

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyGridRow"/> class.
    /// </summary>
    /// <param name="node">The node displayed on this row.</param>
    public PropertyGridRow(Node node)
    {
      Invariant.ArgumentNotNull(node, "node");

      _node = node;
    }

    /// <summary>
    /// Gets the node displayed on this row.
    /// </summary>
    public Node Node
    {
      get { return _node; }
    }

    /// <summary>
    /// Gets whether this row is a leaf node (i.e. has no children).
    /// </summary>
    public bool IsLeaf
    {
      get
      {
        return (_node.Children.Count == 0);
      }
    }

    private PropertyGridBindingView _children;

    /// <summary>
    /// Gets a <see cref="PropertyGridBindingView"/> of the children of the node
    /// displayed on this row.
    /// </summary>
    public PropertyGridBindingView Children
    {
      get
      {
        if (_children == null)
        {
          _children = new PropertyGridBindingView(Node.Children);
        }
        return _children;
      }
    }

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged; // This needs to exist to resolve a memory leak.

    // This needs to exist to make use of the PropertyChanged event.
    internal void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}