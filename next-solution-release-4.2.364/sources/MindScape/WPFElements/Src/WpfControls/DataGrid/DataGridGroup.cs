using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// The data model of a group in a data grid.
  /// </summary>
  public class DataGridGroup : ViewModelBase
  {
    private readonly SortedDictionary<string, DataGridGroup> _groups = new SortedDictionary<string,DataGridGroup>();
    private readonly IList _children = new List<object>();
    private string _groupName;
    private int _level;

    private bool _isExpanded;

    /// <summary>
    /// Gets the collection of items within this group.
    /// </summary>
    public IList Children
    {
      get
      {
        if (_children.Count == 0 && _groups.Count != 0)
        {
          List<object> children = new List<object>();
          foreach (DataGridGroup group in Groups.Values)
          {
            children.AddRange(group.Children as List<object>);
          }
          return children;
        }
        return _children;
      }
    }

    internal IList InternalChildren
    {
      get { return _children; }
    }

    internal SortedDictionary<string, DataGridGroup> Groups
    {
      get { return _groups; }
    }

    /// <summary>
    /// Gets the level of this <see cref="DataGridGroup"/> within the grouping tree.
    /// </summary>
    public int Level
    {
      get { return _level; }
      internal set
      {
        _level = value;
      }
    }

    /// <summary>
    /// Gets or sets whether or not this group is expanded.
    /// </summary>
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        Set<bool>(ref _isExpanded, value, "IsExpanded");
        OnIsExpandedChanged();
      }
    }

    internal event EventHandler IsExpandedChanged;

    private void OnIsExpandedChanged()
    {
      EventHandler handler = IsExpandedChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the name of this group.
    /// </summary>
    public string GroupName
    {
      get { return _groupName; }
      internal set { _groupName = value; }
    }

    internal GroupDescription GroupFrom { get; set; }
  }
}
