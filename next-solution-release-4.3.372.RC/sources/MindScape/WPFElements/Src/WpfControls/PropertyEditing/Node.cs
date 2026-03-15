using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;
using System.Windows;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Node.#System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs)")]

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Represents an entry in a <see cref="PropertyGrid"/>.
  /// </summary>
  public abstract class Node : IWeakEventListener, INotifyPropertyChanged
  {
    private static readonly Predicate<Node> _allChildren = delegate(Node node) { return true; };
    private static readonly Predicate<Node> _noChildren = delegate(Node node) { return false; };

    private object _source;
    private IPropertyInfo _property;
    private readonly string _name;
    private readonly Predicate<Node> _childFilter = _allChildren;
    private readonly NodeEditor _inPlaceEditor;
    private ObservableCollection<Node> _children;
    private Node _parent;

    internal ObjectWrapper SourceReference { get; set; }

    private Node(object source, IPropertyInfo property)
    {
      Invariant.ArgumentNotNull(source, "source");
      Invariant.ArgumentNotNull(property, "property");

      _source = source;
      _property = property;
      _previousType = _property.PropertyType;
      _name = property.Name;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="source">The object whose property is being represented.</param>
    /// <param name="property">The property being represented.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    protected Node(object source, PropertyInfo property, Predicate<Node> childFilter)
      : this(source, new PassthroughPropertyInfoAdapter(property))
    {
      if (childFilter != null)
      {
        _childFilter = childFilter;
      }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="source">The object whose property is being represented.</param>
    /// <param name="property">The property being represented.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    /// <param name="inPlaceEditor">The editor to be used for this node.</param>
    protected Node(object source, PropertyInfo property, Predicate<Node> childFilter, NodeEditor inPlaceEditor)
      : this(source, new PassthroughPropertyInfoAdapter(property))
    {
      Invariant.ArgumentNotNull(inPlaceEditor, "inPlaceEditor");

      _inPlaceEditor = inPlaceEditor;
      _childFilter = (inPlaceEditor.AllowExpand ? childFilter : _noChildren);
    }

    internal Node(object source, IPropertyInfo property, Predicate<Node> childFilter)
      : this(source, property)
    {
      if (childFilter != null)
      {
        _childFilter = childFilter;
      }
    }

    // This is a helper for scenarios where WPF is hanging onto
    // Node objects via bindings, but the Node contains expensive
    // data.  It would be better if we could persuade WPF to release
    // the Node, and then let the GC do its thing, but this is more
    // expedient when the leaks are difficult to track down.
    internal void ClearContents()
    {
      _source = null;
      _previousType = null;
      _property = null;
      if (_children != null)
      {
        foreach (Node child in _children)
        {
          child.ClearContents();
        }
        _children.Clear();
      }
      _children = null;
      _parent = null;
      SourceReference = null;
    }

    /// <summary>
    /// Gets a callback for determining whether to show descendant nodes.
    /// </summary>
    protected internal Predicate<Node> ChildFilter
    {
      get { return _childFilter; }
    }

    /// <summary>
    /// Gets the object whose property is represented by this node.
    /// </summary>
    public object Source
    {
      get { return _source; }
    }

    /// <summary>
    /// Gets the name of the property represented by this node.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// Gets the metadata for the property represented by this node.
    /// </summary>
    public IPropertyInfo Property
    {
      get { return _property; }
    }

    /// <summary>
    /// Gets the metadata for the property represented by this node.
    /// </summary>
    /// <remarks>It is recommended that you use <see cref="Property"/> rather than PropertyInfo,
    /// as PropertyInfo is not supported in partial trust situations.</remarks>
    public PropertyInfo PropertyInfo
    {
      get
      {
        try
        {
          NamedPermissionSet fullTrust = new NamedPermissionSet("FullTrust");
          fullTrust.Demand();
          return _property.AsPropertyInfo;
        }
        catch (SecurityException ex)
        {
          string message = "Node.PropertyInfo is not supported in partial trust.  Use Node.Property instead.";
          throw new InvalidOperationException(message, ex);
        }
      }
    }

    /// <summary>
    /// Gets the type which declares the property represented by this node.
    /// </summary>
    public Type DeclaringType
    {
      get { return _property.DeclaringType; }
    }

    /// <summary>
    /// Gets whether the node has an editor associated directly with it.
    /// </summary>
    /// <remarks>Most nodes use the editing services provided by the property grid, and are
    /// allocated an editor based on type and/or property name.  However, if an application
    /// explicitly adds a node to the grid, it can specify an editor for that node.</remarks>
    public bool HasOwnInPlaceEditor
    {
      get { return _inPlaceEditor != null; }
    }

    /// <summary>
    /// Gets the editor associated directly with this node, if there is one.
    /// </summary>
    /// <remarks>Most nodes use the editing services provided by the property grid, and are
    /// allocated an editor based on type and/or property name.  However, if an application
    /// explicitly adds a node to the grid, it can specify an editor for that node.</remarks>
    public Editor InPlaceEditor
    {
      get { return _inPlaceEditor; }
    }

    /// <summary>
    /// Gets the parent node of this node, if any.
    /// </summary>
    public Node Parent
    {
      get { return _parent; }
      internal set { _parent = value; }
    }

    /// <summary>
    /// Gets the displayable children of this node.
    /// </summary>
    public ObservableCollection<Node> Children
    {
      get
      {
        if (_children == null)
        {
          UpdateChildren();
        }

        return _children;
      }
    }

    internal void UpdateChildren()
    {
      if (_children == null)
      {
        _children = new ObservableCollection<Node>();
      }
      else
      {
        _children.Clear();
      }

      INotifyCollectionChanged notifier = Value as INotifyCollectionChanged;
      if (notifier != null)
      {
        CollectionChangedEventManager.AddListener(notifier, this);
      }

      INotifyPropertyChanged npc = _source as INotifyPropertyChanged;
      if (npc != null && !_isHookingPropertyChanged)
      {
        PropertyChangedEventManager.AddListener(npc, this, Name);
        _isHookingPropertyChanged = true;
      }

      ReloadChildren();
    }

    private bool _isHookingPropertyChanged;

    private Type _previousType;

    private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Debug.Assert(String.IsNullOrEmpty(e.PropertyName) || Name.Equals(e.PropertyName, StringComparison.OrdinalIgnoreCase));

      INotifyCollectionChanged notifier = Value as INotifyCollectionChanged;
      if (notifier != null)
      {
        CollectionChangedEventManager.AddListener(notifier, this);
      }

      object currentValue = Value;
      if (currentValue == null || currentValue.GetType() != _previousType)
      {
        _previousType = (currentValue == null ? _property.PropertyType : currentValue.GetType());

        ClearChildren();
        ReloadChildren();
      }
      else if (currentValue is ICustomTypeDescriptor)
      {
        ClearChildren();
        ReloadChildren();
      }
    }

    internal void RefreshChildren()
    {
      ClearChildren();
      ReloadChildren();
    }

    private bool _alwaysExpand;

    internal bool AlwaysExpand
    {
      get { return _alwaysExpand; }
      set { _alwaysExpand = value; }
    }

    internal abstract Node Clone();

    private static readonly Attribute[] BrowsableOnly = new Attribute[] { BrowsableAttribute.Yes };

    private void ReloadChildren()
    {
      if (ChildFilter(this) || AlwaysExpand)
      {
        object value = Value;
        if (value != null)
        {
          IEnumerable<CollectionElement> collectionElements = CollectionElement.GetCollectionElements(value, ChildFilter);
          Many many = value as Many;

          if (collectionElements == CollectionElement.ParentIsNotACollection || (value is ICustomTypeDescriptor && many == null))
          {
            object valueForPropertyDescriptors = value;
            if (many != null && many.IsConsistent)
            {
              valueForPropertyDescriptors = many.RawValue;
            }

            foreach (PropertyDescriptor propertyDescriptor in ReflectionUtilities.GetProperties(valueForPropertyDescriptors, BrowsableOnly))
            {
              IPropertyInfo propertyInfo = new DescriptorPropertyInfoAdapter(propertyDescriptor);
              if (propertyInfo.CanRead && propertyInfo.IsBrowsable && (CanDisplayReadOnlyProperties || propertyInfo.CanWrite))
              {
                Node property = new PropertyNode(valueForPropertyDescriptors, propertyInfo, ChildFilter);
                property.CanDisplayReadOnlyProperties = CanDisplayReadOnlyProperties;
                property.Parent = this;
                ObjectWrapper sourceReference = ObjectWrapperFactory.CreateWrapper(Clone(), true);
                property.SourceReference = sourceReference;
                _children.Add(property);
              }
            }
          }
          else
          {
            CollectionUtilities.Append(_children, collectionElements);
            foreach (Node child in _children)
            {
              child.CanDisplayReadOnlyProperties = CanDisplayReadOnlyProperties;
              child.Parent = this;
            }
          }
        }
      }
    }

    private void ClearChildren()
    {
      while (_children.Count > 0)
      {
        _children.RemoveAt(0);
      }
    }

    internal Node FindChild(string propertyName)
    {
      // TODO: indexed properties
      foreach (Node child in Children)
      {
        if (child.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
        {
          return child;
        }
      }
      return null;
    }

    internal void RefreshSourceReference()
    {
      SourceReference = ObjectWrapperFactory.CreateWrapper(Clone(), true);
    }

    private bool _canDisplayReadOnlyProperties = true;

    internal bool CanDisplayReadOnlyProperties
    {
      get { return _canDisplayReadOnlyProperties; }
      set
      {
        if (_canDisplayReadOnlyProperties != value)
        {
          _canDisplayReadOnlyProperties = value;
        }
      }
    }

    /// <summary>
    /// When overridden in a derived class, gets the index(es) of the instance of the property
    /// represented by this node.
    /// </summary>
    public abstract IList<object> IndexedPropertyArguments { get; }

    /// <summary>
    /// When overridden in a derived class, gets the type of the property represented by this node.
    /// </summary>
    public abstract Type PropertyType { get; }

    /// <summary>
    /// When overridden in a derived class, gets whether the node value can be modified.
    /// </summary>
    public abstract bool CanWrite { get; }

    /// <summary>
    /// When overridden in a derived class, gets a display name for the node.
    /// </summary>
    public abstract string HumanName { get; }

    /// <summary>
    /// When overridden in a derived class, gets the category priority of the property represented by
    /// this node.
    /// </summary>
    public virtual int CategoryPriority { get; set; }
        
    /// <summary>
    /// When overridden in a derived class, gets the priority of the property represented by
    /// this node.
    /// </summary>
    public virtual int Priority { get; set; }

    /// <summary>
    /// When overridden in a derived class, gets the category priority of the property represented by
    /// this node.
    /// </summary>
    public virtual int AdvPropertyPriority { get; set; }

    /// <summary>
    /// When overridden in a derived class, gets the value of the property represented by
    /// this node.
    /// </summary>
    public abstract object Value { get; }

    #region IWeakEventListener Members

    bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      PropertyChangedEventArgs pcea = e as PropertyChangedEventArgs;
      if (pcea != null && (String.IsNullOrEmpty(pcea.PropertyName) || Name.Equals(pcea.PropertyName, StringComparison.OrdinalIgnoreCase)))
      {
        OnSourcePropertyChanged(sender, pcea);
        return true;
      }
      NotifyCollectionChangedEventArgs ccea = e as NotifyCollectionChangedEventArgs;
      if (ccea != null)
      {
        if (ccea.Action != NotifyCollectionChangedAction.Replace)
        {
          if (ccea.Action == NotifyCollectionChangedAction.Remove)
          {
            foreach (object o in (System.Collections.IEnumerable)sender)
            {
              INotifyCollectionChanged n = o as INotifyCollectionChanged;
              if (n != null)
              {
                CollectionChangedEventManager.RemoveListener(n, this);
              }
            }
            if (ccea.OldItems != null)
            {
              foreach (object o in ccea.OldItems)
              {
                INotifyCollectionChanged n = o as INotifyCollectionChanged;
                if (n != null)
                {
                  CollectionChangedEventManager.RemoveListener(n, this);
                }
              }
            }
          }
          ClearChildren();
          ReloadChildren();
        }
        return true;
      }
      return false;
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

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
