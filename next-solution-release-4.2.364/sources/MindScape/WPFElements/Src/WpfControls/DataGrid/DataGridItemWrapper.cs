using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Wraps around a data object that has been added to the data grid to surface hierarchical information.
  /// </summary>
  public class DataGridItemWrapper : Control, INotifyPropertyChanged // TODO: it would be best if this wasn't a control, it could help performance. This is a Control so we can listen to the Children property changes.
  {
    private readonly object _object;
    private IList _children;
    private readonly int _level;
    private bool _isExpanded = false;

    internal DataGridItemWrapper(object obj, int level)
    {
      _object = obj;
      _level = level;
    }

    /// <summary>
    /// Gets the wrapped object.
    /// </summary>
    public object Object { get { return _object; } }

    internal IList Children
    {
      get { return _children; }
      set { _children = value; }
    }

    private List<object> _rawChildren;
    private List<object> _sortedChildren;

    internal List<object> SortedChildren
    {
      get{return _sortedChildren;}
      set
      {
        _rawChildren = value;
        _sortedChildren = new List<object>();
      }
    }

    private SortDirection _currentSortDirection = SortDirection.None;

    internal void Sort(IComparer<object> comparer, SortDirection sortDirection)
    {
      if (_rawChildren != null)
      {
        if (sortDirection == SortDirection.None || _rawChildren.Count != _sortedChildren.Count)
        {
          _currentSortDirection = SortDirection.None;
          _sortedChildren.Clear();
          foreach (object o in _rawChildren)
          {
            _sortedChildren.Add(o);
          }
        }
        if (comparer != null)
        {
          if (_currentSortDirection == SortDirection.None && sortDirection != SortDirection.None)
          {
            _sortedChildren.Sort(comparer);
            _currentSortDirection = SortDirection.Ascending;
          }
          if (sortDirection != SortDirection.None && _currentSortDirection != sortDirection)
          {
            List<object> temp = new List<object>();
            for (int i = _sortedChildren.Count - 1; i >= 0; i--)
            {
              temp.Add(_sortedChildren[i]);
            }
            _sortedChildren = temp;
          }
        }
        _currentSortDirection = sortDirection;
      }
    }

    internal bool HasChildren { get { return _children != null && _children.Count > 0; } }

    internal int Level { get { return _level; } }

    internal bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded != value)
        {
          _isExpanded = value;
          OnIsExpandedChanged();
          PropertyChangedEventHandler handler = PropertyChanged;
          if (handler != null)
          {
            handler(this, new PropertyChangedEventArgs("IsExpanded"));
          }
        }
      }
    }

    /// <summary>
    /// Raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal event EventHandler IsExpandedChanged;

    private void OnIsExpandedChanged()
    {
      EventHandler handler = IsExpandedChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #region BoundChildren Property

    internal IList BoundChildren
    {
      get { return (IList)GetValue(BoundChildrenProperty); }
      set { SetValue(BoundChildrenProperty, value); }
    }

    internal static readonly DependencyProperty BoundChildrenProperty =
      DependencyProperty.Register("BoundChildren", typeof(IList), typeof(DataGridItemWrapper),
      new FrameworkPropertyMetadata(OnBoundChildrenChanged));

    private static void OnBoundChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridItemWrapper)d).OnBoundChildrenChanged(e);
    }

    private void OnBoundChildrenChanged(DependencyPropertyChangedEventArgs e)
    {
      INotifyCollectionChanged notifyer = e.OldValue as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged -= new NotifyCollectionChangedEventHandler(Notifyer_CollectionChanged);
      }

      notifyer = e.NewValue as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged += new NotifyCollectionChangedEventHandler(Notifyer_CollectionChanged);
      }

      EventHandler handler = ChildrenChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void Notifyer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      EventHandler handler = ChildrenChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler ChildrenChanged;

    #endregion // BoundChildren Property
  }
}
