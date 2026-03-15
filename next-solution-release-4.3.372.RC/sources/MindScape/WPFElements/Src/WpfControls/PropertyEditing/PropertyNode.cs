using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// A property grid entry representing a non-indexed property on an object.
  /// </summary>
  public sealed class PropertyNode : Node
  {
    private readonly string _caption;

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyNode"/> class.
    /// </summary>
    /// <param name="source">The object whose property is being represented.</param>
    /// <param name="property">The property being represented.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    public PropertyNode(object source, PropertyInfo property, Predicate<Node> childFilter)
      : base(source, property, childFilter) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyNode"/> class.
    /// </summary>
    /// <param name="source">The object whose property is being represented.</param>
    /// <param name="caption">A human-readable display name for this property.</param>
    /// <param name="property">The property being represented.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    public PropertyNode(object source, string caption, PropertyInfo property, Predicate<Node> childFilter)
      : base(source, property, childFilter)
    {
      _caption = caption;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyNode"/> class.
    /// </summary>
    /// <param name="source">The object whose property is being represented.</param>
    /// <param name="caption">A human-readable display name for this property.</param>
    /// <param name="property">The property being represented.</param>
    /// <param name="inPlaceEditor">The editor to be used for this node.</param>
    /// <param name="childFilter">A callback for determining whether to show descendant nodes.</param>
    public PropertyNode(object source, string caption, PropertyInfo property, Predicate<Node> childFilter, NodeEditor inPlaceEditor)
      : base(source, property, childFilter, inPlaceEditor)
    {
      _caption = caption;
    }

    internal PropertyNode(object source, PropertyDescriptor property, Predicate<Node> childFilter)
      : base(source, new DescriptorPropertyInfoAdapter(property), childFilter) { }

    internal PropertyNode(object source, IPropertyInfo property, Predicate<Node> childFilter)
      : base(source, property, childFilter) { }

    /// <summary>
    /// Gets the value of the property represented by this node.
    /// </summary>
    public override object Value
    {
      get
      {
        return Property.GetValue(Source, BindingFlags.Default, null, null, null);
      }
    }

    private bool? _canWriteOverride = null;

    internal void SetCanWrite(bool? canWrite)
    {
      _canWriteOverride = canWrite;
    }

    /// <summary>
    /// Gets whether the property value can be modified.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        if (_canWriteOverride.HasValue)
        {
          return _canWriteOverride.Value;
        }
        else
        {
          return Property.CanWrite;
        }
      }
    }

    /// <summary>
    /// Gets the type of the property represented by this node.
    /// </summary>
    public override Type PropertyType
    {
      get { return Property.PropertyType; }
    }

    /// <summary>
    /// Gets the indexed property arguments.  For a <see cref="PropertyNode"/>, the property
    /// is always non-indexed and this always returns null.
    /// </summary>
    public override IList<object> IndexedPropertyArguments
    {
      get { return null; }
    }

    /// <summary>
    /// Gets a display name for the node.
    /// </summary>
    public override string HumanName
    {
      get
      {
        if (!String.IsNullOrEmpty(_caption))
        {
          return _caption;
        }
        return Property.DisplayName;
      }
    }

    public bool IsLocalizableProperty
    {
        get
        {
            if (Parent?.PropertyType == null || PropertyType == null)
                return true;

            if (!Parent.PropertyType.IsPrimitive && 
                Parent.PropertyType.IsValueType &&
                PropertyType.IsPrimitive)
                return false;

            return true;
        }
    }
    internal override Node Clone()
    {
      return new PropertyNode(Source, Property, ChildFilter);
    }
  }
}