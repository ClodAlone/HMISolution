using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Data;
using System.Globalization;

namespace Mindscape.WpfElements.WpfDataGrid
{
  // This class will hopefully help us with all our sorting, grouping and paging needs.
  // This can be internal later on. - as long as XBAPs will allow it to be internal.

  /// <summary>
  /// A collection that provides sorting and paging support for items in a <see cref="DataGrid"/>.
  /// </summary>
  public sealed class DataGridItemsSource : IEnumerable, IEnumerable<object>, INotifyPropertyChanged
  {
    private IEnumerable _source;
    //private ObservableCollection<DataGridColumn> _groupedColumns;

    private Dictionary<object, DataGridItemWrapper> _hierarchy;
    private List<object> _hierarchyItems;
    private DataGridHierarchyMode _hierarchyMode = DataGridHierarchyMode.Collapsed;
    private readonly Dictionary<object, bool> _expansionStates = new Dictionary<object,bool>();

    private List<object> _sortedItems;
    private List<object> _displayedItems;

    private SortedDictionary<string, DataGridGroup> _groups;
    private List<object> _groupedItems;

    private int _pageSize;
    private int _pageIndex;

    private readonly DataTemplate _itemTemplate;

    private readonly ObservableCollection<GroupDescription> _groupDescriptions = new ObservableCollection<GroupDescription>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridItemsSource"/> class.
    /// </summary>
    /// <param name="source">The original items source.</param>
    internal DataGridItemsSource(IEnumerable source)
      : this(source, null, DataGridHierarchyMode.Collapsed)
    {
    }

    internal DataGridItemsSource(IEnumerable source, DataTemplate itemTemplate, DataGridHierarchyMode hierarchyMode)
    {
      _itemTemplate = itemTemplate;
      _source = source;
      _sortedItems = new List<object>();
      foreach (object o in _source)
      {
        _sortedItems.Add(o);
      }
      HierarchyMode = hierarchyMode;
      BuildHierarchyModel(); // TODO: might be able to improve performance of this by merging this operation with the above loop.
      _groupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupDescriptions_CollectionChanged);
      //_groupedColumns = groupedColumns;
      //_groupedColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
      UpdateGrouping(); // TODO: can this operation also be merged with the above loop??
      INotifyCollectionChanged notifyer = source as INotifyCollectionChanged;
      if (notifyer != null)
      {
        //notifyer.CollectionChanged += new NotifyCollectionChangedEventHandler(Source_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = source as IBindingList;
        if (bindingList != null)
        {
          //bindingList.ListChanged += new ListChangedEventHandler(BindingList_ListChanged);
        }
      }
    }

    internal Dictionary<GroupDescription, DataGridColumn> GroupMap { get; set; }

    private bool _lock = false;

    internal void Begin()
    {
      _lock = true;
    }

    internal void End()
    {
      _lock = false;
      UpdateGrouping();
    }


    private void GroupDescriptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (!_lock)
      {
        UpdateGrouping();
      }
    }

    internal void Destroy()
    {
      INotifyCollectionChanged notifyer = _source as INotifyCollectionChanged;
      if (notifyer != null)
      {
        //notifyer.CollectionChanged -= new NotifyCollectionChangedEventHandler(Source_CollectionChanged);
      }
      else
      {
        IBindingList bindingList = _source as IBindingList;
        if (bindingList != null)
        {
          //bindingList.ListChanged -= new ListChangedEventHandler(BindingList_ListChanged);
        }
      }
      //_groupedColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(GroupedColumns_CollectionChanged);
      _groupDescriptions.CollectionChanged -= new NotifyCollectionChangedEventHandler(GroupDescriptions_CollectionChanged);

      if (_hierarchy != null)
      {
        foreach (DataGridItemWrapper wrapper in _hierarchy.Values)
        {
          /*notifyer = wrapper.Children as INotifyCollectionChanged;
          if (notifyer != null)
          {
            notifyer.CollectionChanged -= Source_CollectionChanged;
          }*/
          wrapper.IsExpandedChanged -= new EventHandler(Wrapper_IsExpandedChanged);
          wrapper.ChildrenChanged -= new EventHandler(Wrapper_ChildrenChanged);
        }
      }

      _source = null;
      _sortedItems = null;
      //_groupedColumns = null;
      _displayedItems = null;
      _hierarchy = null;
      _hierarchyItems = null;
      _expansionStates.Clear();
    }

    /*private void BindingList_ListChanged(object sender, ListChangedEventArgs e)
    {
      // TODO: can we use some fancy flags thing to simplify this condition?
      if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemMoved || e.ListChangedType == ListChangedType.Reset)
      {
        BuildHierarchyModel();
        Sort(Sorter, SortDirection.None);
      }
    }*/

    /*private void GroupedColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateGrouping();
    }*/

    internal void UpdateGrouping()
    {
      _groups = new SortedDictionary<string, DataGridGroup>();
      if (_groupDescriptions.Count > 0)
      {
        foreach (object o in _sortedItems)
        {
          SortedDictionary<string, DataGridGroup> currentGroups = _groups;
          DataGridGroup innerMostGroup = null;
          int level = 0;
          foreach (GroupDescription groupDescription in _groupDescriptions)
          {
            string groupName = GetGroupName(o, groupDescription);
            if (groupName != null)
            {
              innerMostGroup = GetGroup(currentGroups, groupName, groupDescription);
              innerMostGroup.Level = level;
              currentGroups = innerMostGroup.Groups;
              level++;
            }
          }
          if (innerMostGroup != null)
          {
            innerMostGroup.InternalChildren.Add(o);
          }
        }
        // Now that the group tree has been created, we iterate the tree to create the linear collection.
        // TODO: I'm wondering if this linear creation of the collection can be skipped. Maybe parts of the linear collection
        //       could be created only when they are requested.
        BuildGroupedItemsList();
      }
      UpdateHierarchy();
      //UpdatePaging();
    }

    private void BuildGroupedItemsList()
    {
      _groupedItems = new List<object>();
      foreach (DataGridGroup group in _groups.Values)
      {
        AddGroupToGroupedItems(group);
      }
    }

    private void AddGroupToGroupedItems(DataGridGroup group)
    {
      _groupedItems.Add(group);
      if (group.IsExpanded)
      {
        if (group.InternalChildren.Count != 0)
        {
          foreach (object o in group.InternalChildren)
          {
            _groupedItems.Add(o);
          }
        }
        else
        {
          foreach (DataGridGroup g in group.Groups.Values)
          {
            AddGroupToGroupedItems(g);
          }
        }
      }
    }

    private DataGridGroup GetGroup(SortedDictionary<string, DataGridGroup> groups, string name, GroupDescription groupDescription)
    {
      DataGridGroup group;
      groups.TryGetValue(name, out group);
      if (group == null)
      {
        group = new DataGridGroup() { GroupName = name, IsExpanded = true, GroupFrom = groupDescription };
        group.IsExpandedChanged += new EventHandler(Group_IsExpandedChanged);
        groups[name] = group;
      }
      return group;
    }

    // TODO: remove event handler from deleted groups!
    private void Group_IsExpandedChanged(object sender, EventArgs e)
    {
      // TODO: is there a way to do this faster? Such as only remove or add the necessary items from the grouped list collection?
      BuildGroupedItemsList();
      UpdateHierarchy();
      //UpdatePaging();
    }

    private string GetGroupName(object o, GroupDescription groupDescription)
    {
      if (groupDescription != null)
      {
        //obj = column.PropertyInfo.AsPropertyInfo.GetValue(o, null);
        // TODO: The performance of groupDescription.GroupNameFromItem is terrible, so we have this workaround.
        //       This bypasses the StringComparison and Converter properties which makes it seem pointless to use GroupDescription.
        //       Need to decide what the grouping API should be. But it's internal for now so can sort it out later.
        PropertyGroupDescription description = groupDescription as PropertyGroupDescription;
        if (description != null && o != null)
        {
          if (o is DataRowView)
          {
            DataRowView rowView = o as DataRowView;
            object groupName = rowView[description.PropertyName];

            if (rowView.DataView != null && rowView.DataView.Table != null && rowView.DataView.Table.Columns.Contains(description.PropertyName))
            {
              DataColumn column = rowView.DataView.Table.Columns[description.PropertyName];
              if (column != null && groupName != null && column.DataType.IsEnum)
              {
                string str = groupName.ToString();
                if (!"".Equals(str))
                {
                  groupName = Enum.Parse(column.DataType, str);
                }
              }
            }

            return groupName == null ? "NULL" : groupName.ToString();
          }
          PropertyInfo info = o.GetType().GetProperty(description.PropertyName);
          if (info != null)
          {
            object groupName = info.GetValue(o, null);
            return groupName == null ? "NULL" : groupName.ToString();
          }
          else
          {
            if (GroupMap != null)
            {
              DataGridColumn column;
              GroupMap.TryGetValue(groupDescription, out column);
              if (column != null)
              {
                object groupName = column.PropertyInfo.AsPropertyInfo.GetValue(o, null);
                return groupName == null ? "NULL" : groupName.ToString();
              }
            }
            return null;
          }
        }
        //object groupName = groupDescription.GroupNameFromItem(o, 0, CultureInfo.CurrentCulture);
      }
      return "NULL";
    }

    private int _hierarchyLock;

    private void BuildHierarchyModel()
    {
      HierarchicalDataTemplate template = _itemTemplate as HierarchicalDataTemplate;
      if (template != null)
      {
        Binding binding = template.ItemsSource as Binding;
        if (binding != null)
        {
          DestroyHierarchyModel(); // TODO: rather than destroying the heirarchy model, only reconstruct the parts that changed.
          _hierarchy = new Dictionary<object, DataGridItemWrapper>();
          if (_source != null)
          {
            foreach (object o in _source)
            {
              List<object> rawChildren;
              DataGridItemWrapper wrapper = BuildItemWrapper(o, 0);
              IList children = GetChildren(wrapper, o, binding, 1, out rawChildren);
              wrapper.ChildrenChanged += new EventHandler(Wrapper_ChildrenChanged);
              if (children != null && children.Count > 0)
              {
                wrapper.Children = children;
                wrapper.SortedChildren = rawChildren;

                if (HierarchyMode == DataGridHierarchyMode.Expanded)
                {
                  _hierarchyLock++;
                  wrapper.IsExpanded = true;
                  _hierarchyLock--;
                }
                _hierarchyLock++;
                bool expandedState;
                bool found = _expansionStates.TryGetValue(wrapper.Object, out expandedState);
                if (found)
                {
                  wrapper.IsExpanded = expandedState;
                }
                _hierarchyLock--;
              }
            }
          }
        }
      }
    }

    // TODO: performance of this could be a little slow with massive hierarchies.
    private void DestroyHierarchyModel()
    {
      if (_hierarchy != null)
      {
        foreach (DataGridItemWrapper wrapper in _hierarchy.Values)
        {
          /*INotifyCollectionChanged notifyer = wrapper.Children as INotifyCollectionChanged;
          if (notifyer != null)
          {
            notifyer.CollectionChanged -= Source_CollectionChanged;
          }*/
          wrapper.IsExpandedChanged -= new EventHandler(Wrapper_IsExpandedChanged);
          wrapper.ChildrenChanged -= new EventHandler(Wrapper_ChildrenChanged);
        }
      }
    }

    private DataGridItemWrapper BuildItemWrapper(object o, int level)
    {
      DataGridItemWrapper wrapper = new DataGridItemWrapper(o, level);
      wrapper.IsExpandedChanged += new EventHandler(Wrapper_IsExpandedChanged);
      _hierarchy[o] = wrapper;
      return wrapper;
    }

    private void Wrapper_ChildrenChanged(object sender, EventArgs e)
    {
      //BuildHierarchyModel();
      //UpdateHierarchy();
      HierarchicalDataTemplate template = _itemTemplate as HierarchicalDataTemplate;
      if (template != null)
      {
        Binding binding = template.ItemsSource as Binding;
        if (binding != null)
        {
          DataGridItemWrapper wrapper = sender as DataGridItemWrapper;
          if (wrapper != null)
          {
            DestroyHierarchyNode(wrapper);

            List<object> rawChildren;
            wrapper.ChildrenChanged -= new EventHandler(Wrapper_ChildrenChanged);
            IList children = GetChildren(wrapper, wrapper.Object, binding, wrapper.Level + 1, out rawChildren);
            wrapper.ChildrenChanged += new EventHandler(Wrapper_ChildrenChanged);
            wrapper.Children = children;
            wrapper.SortedChildren = rawChildren;

            if (wrapper.IsExpanded)
            {
              foreach (object o in children)
              {
                DataGridItemWrapper childWrapper = o as DataGridItemWrapper;
                if (childWrapper != null)
                {
                  List<object> subRawChildren;
                  IList subChildren = GetChildren(childWrapper, childWrapper.Object, binding, childWrapper.Level + 1, out subRawChildren);
                  childWrapper.Children = subChildren;
                  childWrapper.SortedChildren = subRawChildren;

                  if (HierarchyMode == DataGridHierarchyMode.Expanded)
                  {
                    _hierarchyLock++;
                    childWrapper.IsExpanded = true;
                    _hierarchyLock--;
                  }
                  _hierarchyLock++;
                  bool expandedState;
                  bool found = _expansionStates.TryGetValue(childWrapper.Object, out expandedState);
                  if (found)
                  {
                    childWrapper.IsExpanded = expandedState;
                  }
                  _hierarchyLock--;
                }
              }
            }

            UpdateHierarchy();
          }
        }
      }
    }

    private void DestroyHierarchyNode(DataGridItemWrapper wrapper)
    {
      if (wrapper.Children != null)
      {
        foreach (object o in wrapper.Children)
        {
          DataGridItemWrapper childWrapper = o as DataGridItemWrapper;
          if (childWrapper != null)
          {
            childWrapper.ChildrenChanged -= new EventHandler(Wrapper_ChildrenChanged);
            childWrapper.IsExpandedChanged -= new EventHandler(Wrapper_IsExpandedChanged);
            if (childWrapper.DataContext != null)
            {
              _hierarchy.Remove(childWrapper.DataContext);
            }
            DestroyHierarchyNode(childWrapper);
          }
        }
      }
    }

    private void Wrapper_IsExpandedChanged(object sender, EventArgs e)
    {
      DataGridItemWrapper wrapper = sender as DataGridItemWrapper;

      if (wrapper.IsExpanded)
      {
        HierarchicalDataTemplate template = _itemTemplate as HierarchicalDataTemplate;
        if (template != null)
        {
          Binding binding = template.ItemsSource as Binding;
          if (binding != null && wrapper.Children != null)
          {
            // Build the next level in the hierarchy:
            foreach (object child in wrapper.Children)
            {
              DataGridItemWrapper wrappedChild = child as DataGridItemWrapper;
              if (wrappedChild != null && wrappedChild.Children == null)
              {
                List<object> rawChildren;
                IList children = GetChildren(wrappedChild, wrappedChild.Object, binding, wrappedChild.Level + 1, out rawChildren);
                wrappedChild.ChildrenChanged += new EventHandler(Wrapper_ChildrenChanged);
                wrappedChild.Children = children;
                wrappedChild.SortedChildren = rawChildren;

                if (HierarchyMode == DataGridHierarchyMode.Expanded)
                {
                  _hierarchyLock++;
                  wrappedChild.IsExpanded = true;
                  _hierarchyLock--;
                }
                _hierarchyLock++;
                bool expandedState;
                bool found = _expansionStates.TryGetValue(wrappedChild.Object, out expandedState);
                if (found)
                {
                  wrappedChild.IsExpanded = expandedState;
                }
                _hierarchyLock--;
              }
            }
          }
        }
      }

      _expansionStates[wrapper.Object] = wrapper.IsExpanded;

      if (_hierarchyLock <= 0)
      {
        UpdateHierarchy();
      }

      OnRowIsExpandedChanged(wrapper.Object, wrapper.IsExpanded);
    }

    internal event EventHandler<RowIsExpandedChangedEventArgs> RowIsExpandedChanged;

    private void OnRowIsExpandedChanged(object item, bool isExpanded)
    {
      EventHandler<RowIsExpandedChangedEventArgs> handler = RowIsExpandedChanged;
      if (handler != null)
      {
        handler(this, new RowIsExpandedChangedEventArgs(item, isExpanded));
      }
    }

    internal void Expand(object item)
    {
      DataGridItemWrapper wrapper;
      _hierarchy.TryGetValue(item, out wrapper);
      if (wrapper != null)
      {
        wrapper.IsExpanded = true;
      }
    }

    internal void Collapse(object item)
    {
      DataGridItemWrapper wrapper;
      _hierarchy.TryGetValue(item, out wrapper);
      if (wrapper != null)
      {
        wrapper.IsExpanded = false;
      }
    }

    private void UpdateHierarchy()
    {
      _hierarchyItems = new List<object>();
      if (_hierarchy != null && _hierarchy.Count > 0)
      {
        IList activeList = _groupDescriptions.Count == 0 ? _sortedItems : _groupedItems;
        for (int i = 0; i < activeList.Count; i++)
        {
          object o = activeList[i];
          DataGridItemWrapper wrapper;
          _hierarchy.TryGetValue(o, out wrapper);
          if (wrapper != null)
          {
            _hierarchyItems.Add(wrapper);
            if (wrapper.IsExpanded)
            {
              AddChildrenToHierarchyList(wrapper);
            }
          }
          else
          {
            _hierarchyItems.Add(o);
          }
        }
      }
      UpdatePaging();
    }

    private void AddChildrenToHierarchyList(DataGridItemWrapper wrapper)
    {
      if (wrapper != null & _hierarchy != null && _hierarchyItems != null)
      {
        wrapper.Sort(_currentComparer, _currentSortDirection);
        if (wrapper.SortedChildren != null || wrapper.Children != null)
        {
          foreach (object child in wrapper.SortedChildren ?? wrapper.Children)
          {
            _hierarchyItems.Add(child);
            DataGridItemWrapper childWrapper = child as DataGridItemWrapper;
            if (childWrapper == null)
            {
              _hierarchy.TryGetValue(child, out childWrapper);
            }
            if (childWrapper != null)
            {
              if (childWrapper.IsExpanded)
              {
                AddChildrenToHierarchyList(childWrapper);
              }
            }
          }
        }
      }
    }

    internal bool HasHierarchy
    {
      get
      {
        return _hierarchy != null && _hierarchy.Count > 0;
      }
    }

    internal DataGridItemWrapper GetHierarchyWrapper(object item)
    {
      DataGridItemWrapper wrapper = null;
      if (_hierarchy != null)
      {
        _hierarchy.TryGetValue(item, out wrapper);
      }
      return wrapper;
    }

    // index is the index of the parent within the given collection.
    /*private void AddChildren(IList collection, int index)
    {

    }*/

    private IList GetChildren(DataGridItemWrapper wrapper, object o, Binding binding, int level, out List<object> rawChildren)
    {
      //Dummy.Instance.DataContext = o;
      //BindingOperations.SetBinding(Dummy.Instance, Dummy.ListProperty, binding);
      //IList children = Dummy.Instance.List;
      wrapper.DataContext = o;
      BindingOperations.SetBinding(wrapper, DataGridItemWrapper.BoundChildrenProperty, binding);
      IList children = wrapper.BoundChildren;
      /*INotifyCollectionChanged notifyer = children as INotifyCollectionChanged;
      if (notifyer != null)
      {
        notifyer.CollectionChanged += Source_CollectionChanged;
      }*/
      rawChildren = new List<object>();
      if (children != null && children.Count > 0)
      {
        IList wrappedChildren = new List<object>();
        foreach (object child in children)
        {
          DataGridItemWrapper wrappedChild = BuildItemWrapper(child, level);
          wrappedChildren.Add(wrappedChild);
          rawChildren.Add(child);
        }
        children = wrappedChildren;
      }
      return children;
    }

    /*private class Dummy : Control
    {
      internal static readonly Dummy Instance = new Dummy();

      private Dummy() { }

      #region List Property

      public IList List
      {
        get { return (IList)GetValue(ListProperty); }
        set { SetValue(ListProperty, value); }
      }

      public static readonly DependencyProperty ListProperty =
        DependencyProperty.Register("List", typeof(IList), typeof(Dummy),
        new FrameworkPropertyMetadata(OnListChanged));

      private static void OnListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
        ((Dummy)d).OnListChanged();
      }

      private void OnListChanged()
      {
      }

      #endregion // List Property
    }*/

    internal void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (object o in e.OldItems)
        {
          _expansionStates.Remove(o);
        }
      }
      //if (e.Action != NotifyCollectionChangedAction.Reset) // TODO: What was this for?
      {
        BuildHierarchyModel();
        Sort(Sorter, SortDirection.None);
      }
    }

    internal void ClearSorting()
    {
      Sort((IComparer<object>)null, SortDirection.None);
    }

    /// <summary>
    /// Sorts the items by the given property in the given sort direction.
    /// </summary>
    /// <param name="sortProperty">The property to sort the items by.</param>
    /// <param name="sortDirection">The direction to sort the items.</param>
    public void Sort(PropertyInfo sortProperty, SortDirection sortDirection)
    {
      Sort(new PropertyBasedComparer(sortProperty), sortDirection);
    }

    private IComparer<object> _currentComparer;
    private SortDirection _currentSortDirection = SortDirection.None;

    internal IComparer<object> SortComparer
    {
      get { return _currentComparer; }
      set
      {
        _currentComparer = value;
      }
    }

    internal SortDirection SortDirection
    {
      get { return _currentSortDirection; }
      set
      {
        _currentSortDirection = value;
      }
    }

    /// <summary>
    /// Sorts the items using the given comparer in the given sort direction.
    /// </summary>
    /// <param name="comparer">The comparer to sort the items by.</param>
    /// <param name="sortDirection">The direction to sort the items.</param>
    public void Sort(IComparer<object> comparer, SortDirection sortDirection)
    {
      if (_source == null)
      {
        return;
      }
      _currentComparer = comparer;
      _currentSortDirection = sortDirection;
      if (sortDirection == SortDirection.None)
      {
        Filter();
      }
      else if (CustomSort != null)
      {
        Filter();
        CustomSort(_sortedItems, SortColumn);
      }
      else if (IsReverseOfCurrentSort(comparer, sortDirection))
      {
        _sortedItems.Reverse();
      }
      else
      {
        Sorter = comparer;
        Filter();
        _sortedItems.Sort(Sorter);

        // This could be done slightly more efficiently by performing the sort in reverse,
        // rather than sorting and then reversing.  But that's probably a micro-optimisation.
        if (sortDirection == SortDirection.Descending)
        {
          _sortedItems.Reverse();
        }
      }

      _lastSortDirection = sortDirection;

      UpdateGrouping();
      //UpdatePaging();
    }

    private void Filter()
    {
      _sortedItems = new List<object>();
      if (_filterExpression != null)
      {
        foreach (object o in _source)
        {
          if (_filterExpression.IsMatch(o))
          {
            _sortedItems.Add(o);
          }
        }
      }
      else
      {
        foreach (object o in _source)
        {
          _sortedItems.Add(o);
        }
      }
    }

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> for enumerating this collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for enumerating this collection.</returns>
    public IEnumerator<object> GetEnumerator()
    {
      return GetActiveCollection().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    // TODO: would be good to improve the performance of this:
    /// <summary>
    /// Gets the index of the given item.
    /// </summary>
    /// <param name="item">The item to search for.</param>
    /// <returns>The index of the given item.</returns>
    public int IndexOf(object item)
    {
      IList<object> collection = GetTotalActiveCollection();
      int index = 0;
      foreach (object o in collection)
      {
        DataGridItemWrapper wrapper = o as DataGridItemWrapper;
        if (wrapper != null)
        {
          if (wrapper.Object == item)
          {
            return index;
          }
        }
        if (o == item)
        {
          return index;
        }
        index++;
      }
      return -1;
      //return _sortedItems.IndexOf(item);
    }

    // Could implement the index operator instead:
    internal object GetItemAt(int index)
    {
      IList<object> collection = GetTotalActiveCollection();
      object o = collection[index];
      if (o is DataGridGroup)
      {
        return null;
      }
      DataGridItemWrapper wrapper = o as DataGridItemWrapper;
      if (wrapper != null)
      {
        return wrapper.Object;
      }
      return o;
      //return _sortedItems[index];
    }

    internal object GetItemAt_SortedItemsOnly(int index)
    {
      return _sortedItems[index];
    }

    internal int Count
    {
      get { return _sortedItems.Count; }
    }

    internal int CountOnTotalActiveCollection
    {
      get
      {
        IList<object> collection = GetTotalActiveCollection();
        return collection.Count;
      }
    }

    internal int IndexOfOnCurrentPage(object item)
    {
      object originalItem = item;
      if (_hierarchy != null && _hierarchy.Count > 0)
      {
        DataGridItemWrapper wrapper;
        _hierarchy.TryGetValue(item, out wrapper);
        item = wrapper ?? item;
      }
      if (item is DataRow)
      {
        return IndexOfDataRowOnCurrentPage(item as DataRow);
      }
      List<object> collection = GetActiveCollection();
      if (collection != null)
      {
        if (collection == _hierarchyItems)
        {
          int index = collection.IndexOf(item);
          return index == -1 ? collection.IndexOf(originalItem) : index;
        }
        return collection.IndexOf(item);
      }
      return -1;
    }

    private int IndexOfDataRowOnCurrentPage(DataRow row)
    {
      int index = 0;
      foreach (object o in GetActiveCollection())
      {
        DataRowView rowView = o as DataRowView;
        if (rowView != null && rowView.Row == row)
        {
          return index;
        }
        index++;
      }
      return -1;
    }

    internal object GetItemOnCurrentPageAt(int index)
    {
      // TODO: index range check:
      List<Object> activeCollection = GetActiveCollection();
      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("Index can not be negative");
      }
      if (index >= activeCollection.Count)
      {
        throw new ArgumentOutOfRangeException("Index is " + index + ", Collection size is " + activeCollection.Count);
      }
      object item = GetActiveCollection()[index];
      DataGridItemWrapper wrapper = item as DataGridItemWrapper;
      if (wrapper != null)
      {
        item = wrapper.Object;
      }
      return item;
    }

    internal object GetItemOnCurrentPageAt_NullGroupHeaders(int index)
    {
      // TODO: index range check:
      List<Object> activeCollection = GetActiveCollection();
      if (index < 0)
      {
        throw new ArgumentOutOfRangeException("Index can not be negative");
      }
      if (index >= activeCollection.Count)
      {
        throw new ArgumentOutOfRangeException("Index is " + index + ", Collection size is " + activeCollection.Count);
      }
      object item = GetActiveCollection()[index];
      if (item is DataGridGroup)
      {
        return null;
      }
      DataGridItemWrapper wrapper = item as DataGridItemWrapper;
      if (wrapper != null)
      {
        item = wrapper.Object;
      }
      return item;
    }

    /// <summary>
    /// Gets the number of items on the current page after filtering has been applied. This count will include group headers.
    /// </summary>
    public int CountOnCurrentPage
    {
      get
      {
        List<object> activeCollection = GetActiveCollection();
        return activeCollection == null ? 0 : activeCollection.Count;
      }
    }

    private IList<object> GetTotalActiveCollection()
    {
      if (_hierarchyItems == null || _hierarchyItems.Count == 0)
      {
        if (_groups.Count == 0)
        {
          return _sortedItems;
        }
        return _groupedItems;
      }
      return _hierarchyItems;
    }

    // Returns the appropriate collection based on which data grid features are being used.
    private List<object> GetActiveCollection()
    {
      if (PageSize == 0)
      {
        if (_hierarchyItems == null || _hierarchyItems.Count == 0)
        {
          if (_groups.Count == 0)
          {
            return _sortedItems;
          }
          return _groupedItems;
        }
        return _hierarchyItems;
      }
      else
      {
        return _displayedItems;
      }
    }

    internal List<object> GetSortedCollection()
    {
      return _sortedItems;
    }

    internal ObservableCollection<GroupDescription> GroupDescriptions
    {
      get { return _groupDescriptions; }
    }

    private IFilter _filterExpression;

    internal IFilter FilterExpression
    {
      get { return _filterExpression; }
      set
      {
        _filterExpression = value;
        Sort(_currentComparer, _currentSortDirection);
      }
    }

    private IComparer<object> Sorter { get; set; }
    private SortDirection _lastSortDirection = SortDirection.None;

    internal Action<List<object>, DataGridColumn> CustomSort { get; set; }
    internal DataGridColumn SortColumn { get; set; }

    private bool IsReverseOfCurrentSort(IComparer<object> comparer, SortDirection sortDirection)
    {
      bool sameCriteria = EquivalentComparers(comparer, Sorter);
      bool reverseDirection = (sortDirection == SortDirection.Ascending && _lastSortDirection == SortDirection.Descending)
        || (sortDirection == SortDirection.Descending && _lastSortDirection == SortDirection.Ascending);

      return sameCriteria && reverseDirection;
    }

    private static bool EquivalentComparers(IComparer<object> comparer1, IComparer<object> comparer2)
    {
      PropertyBasedComparer pbc1 = comparer1 as PropertyBasedComparer;
      PropertyBasedComparer pbc2 = comparer2 as PropertyBasedComparer;

      if (pbc1 != null && pbc2 != null)
      {
        return pbc1.IsEquivalentTo(pbc2);
      }

      return comparer1 == comparer2;
    }

    private class PropertyBasedComparer : IComparer<object>
    {
      private readonly Dictionary<object, object> _valueMap = new Dictionary<object, object>();
      private readonly PropertyInfo _sortProperty;

      public PropertyBasedComparer(PropertyInfo sortProperty)
      {
        _sortProperty = sortProperty;
      }

      private object GetSortKey(object o)
      {
        object value;
        _valueMap.TryGetValue(o, out value);
        if (value == null)
        {
          value = _sortProperty.GetValue(o, null);
          _valueMap[o] = value;
        }
        return value;
      }

      public int Compare(object o1, object o2)
      {
        object attribute1 = GetSortKey(o1);
        object attribute2 = GetSortKey(o2);
        if (attribute1 is IComparable || attribute2 is IComparable)
        {
          return Comparer.Default.Compare(attribute1, attribute2);
        }
        return Comparer.Default.Compare(attribute1 == null ? "" : attribute1.ToString(), attribute2 == null ? "" : attribute2.ToString());
      }

      internal bool IsEquivalentTo(PropertyBasedComparer other)
      {
        return other._sortProperty == this._sortProperty;
      }
    }

    private DataGridHierarchyMode HierarchyMode
    {
      get { return _hierarchyMode; }
      set
      {
        _hierarchyMode = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of items on each page.
    /// </summary>
    public int PageSize
    {
      get { return _pageSize; }
      set
      {
        //value = Math.Min(_sortedItems.Count, value);
        if (_pageSize != value)
        {
          _pageSize = value;
          PageIndex = Math.Max(0, Math.Min(PageCount - 1, PageIndex));
          UpdatePaging();
        }
      }
    }

    /// <summary>
    /// Gets or sets the index of the currently displayed page of data.
    /// </summary>
    public int PageIndex
    {
      get { return _pageIndex; }
      set
      {
        value = Math.Max(0, Math.Min(PageCount - 1, value));
        if (_pageIndex != value)
        {
          _pageIndex = value;
          UpdatePaging();
          OnPageIndexChanged();
        }
      }
    }

    internal event EventHandler PageIndexChanged;

    private void OnPageIndexChanged()
    {
      EventHandler handler = PageIndexChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void UpdatePaging()
    {
      if (PageSize != 0)
      {
        _displayedItems = new List<object>(PageSize);
        int startIndex = PageIndex * PageSize;
        IList items = _groupDescriptions.Count == 0 ? _sortedItems : _groupedItems;
        if (_hierarchyItems != null && _hierarchyItems.Count > 0)
        {
          items = _hierarchyItems;
        }
        for (int i = startIndex; i < Math.Min(items.Count, startIndex + PageSize); i++)
        {
          _displayedItems.Add(items[i]);
        }
      }
      OnPropertyChanged("PageCount"); // TODO: really we should only do this if PageCount value is actually going to change.
      AddUserRow();
      OnCollectionUpdated();
    }

    private object _newRowObject = null;
    private bool _allowUserToAddRows = false;

    internal object NewRowObject { get { return _newRowObject; } }

    internal bool AllowUserToAddRows
    {
      get { return _allowUserToAddRows; }
      set
      {
        _allowUserToAddRows = value;
        if (_allowUserToAddRows && _newRowObject == null)
        {
          AddUserRow();
          OnCollectionUpdated();
        }
        if (!_allowUserToAddRows && _newRowObject != null)
        {
          IList activeList = GetActiveCollection();
          activeList.Remove(_newRowObject);
          _newRowObject = null;
          OnCollectionUpdated();
        }
      }
    }

    private void AddUserRow()
    {
      if (AllowUserToAddRows)
      {
        IList activeList = GetActiveCollection();
        if (_source != null)
        {
          if (ObjectBuilder != null)
          {
            _newRowObject = ObjectBuilder.Build();
            activeList.Add(_newRowObject);
          }
          else
          {
            Type type = CollectionUtilities.GetCollectionValueType(_source);
            if (type != null)
            {
              ConstructorInfo defaultConstructor = type.GetConstructor(Type.EmptyTypes);
              if (defaultConstructor != null)
              {
                _newRowObject = defaultConstructor.Invoke(null);
                activeList.Add(_newRowObject);
              }
            }
          }
        }
      }
    }

    internal IObjectBuilder ObjectBuilder { get; set; }

    /// <summary>
    /// Gets the total number of data pages.
    /// </summary>
    public int PageCount
    {
      get
      {
        IList items = _groupDescriptions.Count == 0 ? _sortedItems : _groupedItems;
        if (_hierarchyItems != null && _hierarchyItems.Count > 0)
        {
          items = _hierarchyItems;
        }
        if (items == null)
        {
          return 0;
        }
        return PageSize == 0 ? 1 : (int)Math.Ceiling(items.Count / (double)PageSize);
      }
    }

    /// <summary>
    /// Raised when the collection is updated.
    /// </summary>
    internal event EventHandler CollectionUpdated;

    private void OnCollectionUpdated()
    {
      EventHandler handler = CollectionUpdated;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
