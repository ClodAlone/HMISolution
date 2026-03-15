/*
 *  Copyright © Northwoods Software Corporation, 1998-2016. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using System.Windows;  // for Point
using System.Reflection;
using Windows.Foundation;

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// A simple representation of node data for <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  /// that supports property change notification, copying, and undo
  /// via the <c>INotifyPropertyChanged</c>, <c>ICloneable</c>, and <see cref="IChangeDataValue"/> interfaces.
  /// </summary>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data in the model</typeparam>
  /// <remarks>
  /// <para>
  /// This provides a standard implementation of
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  /// data that represents nodes with support for subgraphs,
  /// including properties for specifying the containing subgraph node key
  /// and/or the collection of member node keys.
  /// You can use this class if you do not already have your own application class
  /// holding information about nodes and if you want to inherit from an existing
  /// class so that you can just add your own properties.  Here's a simple example:
  /// </para>
  /// <para>
  /// <code>
  ///  [Serializable]
  ///  public class MyData : GraphLinksModelNodeData&lt;String&gt; {
  ///    public MyData() { }
  ///
  ///    public String Color {
  ///      get { return _Color; }
  ///      set { if (_Color != value) { String old = _Color; _Color = value; RaisePropertyChanged("Color", old, value); } }
  ///    }
  ///    private String _Color = "White";
  ///
  ///    public String Name {
  ///      get { return _Name; }
  ///      set { if (_Name != value) { String old = _Name; _Name = value; RaisePropertyChanged("Name", old, value); } }
  ///    }
  ///    private String _Name;
  ///
  ///    public String Address {
  ///      get { return _Address; }
  ///      set { if (_Address != value) { String old = _Address; _Address = value; RaisePropertyChanged("Address", old, value); } }
  ///    }
  ///    private String _Address;
  ///  }
  /// </code>
  /// Then you can bind to these data properties in the <c>DataTemplate</c> for your nodes:
  /// <code>
  /// &lt;DataTemplate x:Key="NodeTemplate"&gt;
  ///   &lt;Border BorderBrush="Black" BorderThickness="1" CornerRadius="5" Padding="5"
  ///           Background="{Binding Path=Data.Color, Converter={StaticResource theColorConverter}}"
  ///           go:Node.Location="{Binding Path=Data.Location, Mode=TwoWay}"&gt;
  ///     &lt;StackPanel&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" HorizontalAlignment="Left" /&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Address}" HorizontalAlignment="Left" /&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/Border&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// <para>
  /// Note that property setters need to raise the model's Changed event,
  /// so that the model knows about changes in the data and can then update the diagram.
  /// You should call <see cref="RaisePropertyChanged"/> only when the value has actually changed,
  /// and you should pass both the previous and the new values, in order to support undo/redo.
  /// </para>
  /// <para>
  /// For both Silverlight and WPF you should override the <see cref="Clone"/> method
  /// if the fields contain data that should not be shared between copies.
  /// For WPF the properties that you define should also be serializable,
  /// in order for the data to be copiable, especially to and from the clipboard.
  /// </para>
  /// <para>
  /// If you add properties to this node data class, and if you are using the
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Save"/> and
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Load(XContainer, XName, XName)"/> methods,
  /// you should override the <see cref="MakeXElement"/> and <see cref="LoadFromXElement"/>
  /// methods to add new attributes and/or elements as needed,
  /// </para>
  /// <para>
  /// Normally, each <see cref="Key"/> should have a unique value within the model.
  /// You can maintain that yourself, by setting the <see cref="Key"/> to unique values
  /// before adding the node data to the model's collection of nodes.
  /// Or you can ensure this by overriding the
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.MakeNodeKeyUnique"/>
  /// method.  The override (or the setting of the same-named delegate in
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Delegates"/>)
  /// is required if nodes might be copied within the model.
  /// </para>
  /// <para>
  /// If you want each node to keep a "reference" to the containing ("parent") group,
  /// you can use the <see cref="SubGraphKey"/> property.
  /// If you want subgraph data to keep a list of "references" to the contained
  /// ("children") nodes, you can use the <see cref="MemberKeys"/> property.
  /// You can use both properties at the same time.
  /// </para>
  /// <para>
  /// This class is not useful with <see cref="GraphModel{NodeType, NodeKey}"/> or <see cref="TreeModel{NodeType, NodeKey}"/>.
  /// </para>
  /// </remarks>
  [Serializable]
  public class GraphLinksModelNodeData<NodeKey> : INotifyPropertyChanged, IChangeDataValue {
    /// <summary>
    /// The default constructor produces an empty object.
    /// </summary>
    /// <remarks>
    /// If the <i>NodeKey</i> is <c>Guid</c>, this automatically assigns the <see cref="Key"/> to be a new <c>Guid</c>.
    /// </remarks>
    public GraphLinksModelNodeData() {
      if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        _Key = (NodeKey)((object)Guid.NewGuid());
      }
    }
    /// <summary>
    /// This constructor also initializes the <see cref="Key"/> property.
    /// </summary>
    /// <param name="key"></param>
    public GraphLinksModelNodeData(NodeKey key) { _Key = key; }

    /// <summary>
    /// Create a copy of this data; this implements the <c>ICloneable</c> interface.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// When you add your own state in a subclass, and when you expect to be able to copy the data,
    /// you should override this method in your derived class when it has some fields that are object references.
    /// Your override method should first call <c>base.Clone()</c> to get the newly copied object.
    /// The result should be the object you return,
    /// after performing any other deeper copying of referenced objects that you deem necessary,
    /// and after removing references that should not be shared (such as to cached data structures).
    /// </para>
    /// <para>
    /// The standard implementation of this method is to do a shallow copy, by <c>Object.MemberwiseClone()</c>,
    /// and reinitialize the <see cref="SubGraphKey"/> and <see cref="MemberKeys"/> properties.
    /// You do not need to override this method if you have only added some fields/properties
    /// that are values or are references to intentionally shared objects.
    /// </para>
    /// </remarks>
    public virtual object Clone() {
      GraphLinksModelNodeData<NodeKey> d = (GraphLinksModelNodeData<NodeKey>)MemberwiseClone();
      d.PropertyChanged = null;
      d._SubGraphKey = default(NodeKey);
      d._MemberKeys = new ObservableCollection<NodeKey>();
      return d;
    }

    /// <summary>
    /// This event implements the <see cref="INotifyPropertyChanged"/> interface,
    /// so that both the model and the dependency object system can be informed
    /// of changes to property values.
    /// </summary>
    [field: NonSerializedAttribute()]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnPropertyChanged(ModelChangedEventArgs e) {
      if (this.PropertyChanged != null) this.PropertyChanged(this, e);
    }

    /// <summary>
    /// Call this method from property setters to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="pname">the property name</param>
    /// <param name="oldval">the value before the property was set</param>
    /// <param name="newval">the new value</param>
    /// <remarks>
    /// Only call this method when the property value actually changes.
    /// The <paramref name="oldval"/> and <paramref name="newval"/> values are needed
    /// to support undo/redo.
    /// </remarks>
    protected void RaisePropertyChanged(String pname, Object oldval, Object newval) {
      OnPropertyChanged(new ModelChangedEventArgs(pname, this, oldval, newval));
    }

    /// <summary>
    /// For debugging, use the <see cref="Key"/> property as this object's default text rendering.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      return (this.Key != null) ? this.Key.ToString() : "(nokey)";
    }

    /// <summary>
    /// This method implements the <see cref="IChangeDataValue"/> interface,
    /// used to perform state changes for undo and redo.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// Unless you override this method to explicitly handle each property that you define,
    /// this implementation uses reflection to set the property.
    /// </remarks>
    public virtual void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.PropertyName == "Location") {
        this.Location = (Point)e.GetValue(undo);
      } else if (e.PropertyName == "Text") {
        this.Text = (String)e.GetValue(undo);
      } else if (e.PropertyName == "Key") {
        this.Key = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "IsSubGraph") {
        this.IsSubGraph = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "IsSubGraphExpanded") {
        this.IsSubGraphExpanded = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "WasSubGraphExpanded") {
        this.WasSubGraphExpanded = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "SubGraphKey") {
        this.SubGraphKey = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "MemberKeys") {
        this.MemberKeys = (IList<NodeKey>)e.GetValue(undo);
      } else if (e.PropertyName == "IsLinkLabel") {
        this.IsLinkLabel = (bool)e.GetValue(undo);
      } else if (e.PropertyName == "Category") {
        this.Category = (String)e.GetValue(undo);
      } else if (e.Change == ModelChange.Property) {
        if (!ModelHelper.SetProperty(e.PropertyName, e.Data, e.GetValue(undo))) {
          ModelHelper.Error("ERROR: Unrecognized property name: " + e.PropertyName != null ? e.PropertyName : "(noname)" + " in GraphLinksModelNodeData.ChangeDataValue");
        }
      }
    }


    /// <summary>
    /// Constructs a Linq for XML <c>XElement</c> holding the data of this node.
    /// </summary>
    /// <param name="n">the name of the new <c>XElement</c></param>
    /// <returns>an initialized <c>XElement</c></returns>
    /// <remarks>
    /// <para>
    /// This constructs a new <c>XElement</c> and adds an <c>XAttribute</c> for each simple property
    /// that has a value different from its default value.
    /// For each property that is a collection, it adds an <c>XElement</c> with nested item elements.
    /// This does not add an element if the collection is empty.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertNodeKeyToString"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual XElement MakeXElement(XName n) {
    ///   XElement e = new XElement(n);
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("Key", this.Key, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute("Category", this.Category, ""));
    ///   e.Add(XHelper.Attribute("IsLinkLabel", this.IsLinkLabel, false));
    ///   e.Add(XHelper.Attribute("IsSubGraph", this.IsSubGraph, false));
    ///   e.Add(XHelper.Attribute("IsSubGraphExpanded", this.IsSubGraphExpanded, true));
    ///   e.Add(XHelper.Attribute("WasSubGraphExpanded", this.WasSubGraphExpanded, false));
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("SubGraphKey", this.SubGraphKey, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Elements&lt;NodeKey&gt;("MemberKeys", "Key", this.MemberKeys, ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute("Location", this.Location, new Point(Double.NaN, Double.NaN)));
    ///   e.Add(XHelper.Attribute("Text", this.Text, ""));
    ///   return e;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this node data class, and if you are using the
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Save"/> and
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Load(XContainer, XName, XName)"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="LoadFromXElement"/>.
    /// </para>
    /// </remarks>
#if !WINDOWS_UWP
    public virtual XElement MakeXElement(XName n) {
      XElement e = new XElement(n);
      e.Add(XHelper.Attribute<NodeKey>("Key", this.Key, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Attribute("Category", this.Category, ""));
      e.Add(XHelper.Attribute("IsLinkLabel", this.IsLinkLabel, false));
      e.Add(XHelper.Attribute("IsSubGraph", this.IsSubGraph, false));
      e.Add(XHelper.Attribute("IsSubGraphExpanded", this.IsSubGraphExpanded, true));
      e.Add(XHelper.Attribute("WasSubGraphExpanded", this.WasSubGraphExpanded, false));
      e.Add(XHelper.Attribute<NodeKey>("SubGraphKey", this.SubGraphKey, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Elements<NodeKey>("MemberKeys", "Key", this.MemberKeys, ConvertNodeKeyToString));
      e.Add(XHelper.Attribute("Location", this.Location, new Point(Double.NaN, Double.NaN)));
      e.Add(XHelper.Attribute("Text", this.Text, ""));
      return e;
    }
#endif
    /// <summary>
    /// Initialize this node data with data held in a Linq for XML <c>XElement</c>.
    /// </summary>
    /// <param name="e">the <c>XElement</c></param>
    /// <remarks>
    /// <para>
    /// This sets this node data's properties by reading the data from attributes and nested elements
    /// of the given <c>XElement</c>.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertStringToNodeKey"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual void LoadFromXElement(XElement e) {
    ///   this.Key = XHelper.Read&lt;NodeKey&gt;("Key", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.Category = XHelper.Read("Category", e, "");
    ///   this.IsLinkLabel = XHelper.Read("IsLinkLabel", e, false);
    ///   this.IsSubGraph = XHelper.Read("IsSubGraph", e, false);
    ///   this.IsSubGraphExpanded = XHelper.Read("IsSubGraphExpanded", e, true);
    ///   this.WasSubGraphExpanded = XHelper.Read("WasSubGraphExpanded", e, false);
    ///   this.SubGraphKey = XHelper.Read&lt;NodeKey&gt;("SubGraphKey", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.MemberKeys = (IList&lt;NodeKey&gt;)XHelper.ReadElements&lt;NodeKey&gt;(e.Element("MemberKeys"), "Key", new ObservableCollection&lt;NodeKey&gt;(), ConvertStringToNodeKey);
    ///   this.Location = XHelper.Read("Location", e, new Point(Double.NaN, Double.NaN));
    ///   this.Text = XHelper.Read("Text", e, "");
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this node data class, and if you are using the
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Save"/> and
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Load(XContainer, XName, XName)"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="MakeXElement"/>.
    /// </para>
    /// </remarks>
    public virtual void LoadFromXElement(XElement e) {
      if (e == null) return;
      this.Key = XHelper.Read<NodeKey>("Key", e, default(NodeKey), ConvertStringToNodeKey);
      this.Category = XHelper.Read("Category", e, "");
      this.IsLinkLabel = XHelper.Read("IsLinkLabel", e, false);
      this.IsSubGraph = XHelper.Read("IsSubGraph", e, false);
      this.IsSubGraphExpanded = XHelper.Read("IsSubGraphExpanded", e, true);
      this.WasSubGraphExpanded = XHelper.Read("WasSubGraphExpanded", e, false);
      this.SubGraphKey = XHelper.Read<NodeKey>("SubGraphKey", e, default(NodeKey), ConvertStringToNodeKey);
      this.MemberKeys = (IList<NodeKey>)XHelper.ReadElements<NodeKey>(e.Element("MemberKeys"), "Key", new ObservableCollection<NodeKey>(), ConvertStringToNodeKey);
#if !WINDOWS_UWP
            this.Location = XHelper.Read("Location", e, new Point(Double.NaN, Double.NaN));
#endif
      this.Text = XHelper.Read("Text", e, "");
    }


    /// <summary>
    /// Convert a <typeparamref name="NodeKey"/> key value to a string.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>a String from which <see cref="ConvertStringToNodeKey"/> can recover the original key value</returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, Double,
    /// DateTime, TimeSpan, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual String ConvertNodeKeyToString(NodeKey key) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return XHelper.ToString((String)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return XHelper.ToString((int)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(double))) {
        return XHelper.ToString((double)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return XHelper.ToString((Guid)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(DateTime))) {
        return XHelper.ToString((DateTime)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(TimeSpan))) {
        return XHelper.ToString((TimeSpan)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Decimal))) {
        return XHelper.ToString((Decimal)(Object)key);
      } else {
        ModelHelper.Error("Cannot convert NodeKey type to String: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelNodeData.ConvertNodeKeyToString");
        return key.ToString();
      }
    }

    /// <summary>
    /// Convert a string to a <typeparamref name="NodeKey"/> key value.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>a <typeparamref name="NodeKey"/></returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, Double,
    /// DateTime, TimeSpan, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual NodeKey ConvertStringToNodeKey(String s) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return (NodeKey)(Object)XHelper.ToString(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return (NodeKey)(Object)XHelper.ToInt32(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(double))) {
        return (NodeKey)(Object)XHelper.ToDouble(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return (NodeKey)(Object)XHelper.ToGuid(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(DateTime))) {
        return (NodeKey)(Object)XHelper.ToDateTime(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(TimeSpan))) {
        return (NodeKey)(Object)XHelper.ToTimeSpan(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Decimal))) {
        return (NodeKey)(Object)XHelper.ToDecimal(s);
      } else {
        ModelHelper.Error("Cannot convert String to NodeKey type: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelNodeData.ConvertStringToNodeKey");
        return default(NodeKey);
      }
    }


    /// <summary>
    /// Gets or sets the key property for this node data.
    /// </summary>
    /// <value>
    /// The type is the parameterized type <typeparamref name="NodeKey"/>,
    /// which must be compatible with and should the same as the NodeKey type parameter
    /// used for the model, <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>.
    /// </value>
    public NodeKey Key {
      get { return _Key; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_Key, value)) { NodeKey old = _Key; _Key = value; RaisePropertyChanged("Key", old, value); } }
    }
    private NodeKey _Key;

    /// <summary>
    /// Gets or sets a <c>String</c> that names the category to which the node data belongs.
    /// </summary>
    /// <value>
    /// <para>
    /// The default value is an empty string.
    /// </para>
    /// </value>
    public String Category {
      get { return _Category; }
      set { if (_Category != value) { String old = _Category; _Category = value; RaisePropertyChanged("Category", old, value); } }
    }
    private String _Category = "";

    /// <summary>
    /// Gets or sets whether this node data represents a "label" on a link instead of a simple node.
    /// </summary>
    /// <value>
    /// By default this is false.
    /// </value>
    public bool IsLinkLabel {
      get { return _IsLinkLabel; }
      set { if (_IsLinkLabel != value) { bool old = _IsLinkLabel; _IsLinkLabel = value; RaisePropertyChanged("IsLinkLabel", old, value); } }
    }
    private bool _IsLinkLabel;

    /// <summary>
    /// Gets or sets whether this node data represents a group or "subgraph"
    /// instead of a normal "atomic" node.
    /// </summary>
    /// <value>
    /// By default this is false.
    /// </value>
    public virtual bool IsSubGraph {
      get { return _IsSubGraph; }
      set { if (_IsSubGraph != value) { bool old = _IsSubGraph; _IsSubGraph = value; RaisePropertyChanged("IsSubGraph", old, value); } }
    }
    private bool _IsSubGraph;

    /// <summary>
    /// Gets or sets a reference to the containing subgraph node, if any.
    /// </summary>
    public NodeKey SubGraphKey {
      get { return _SubGraphKey; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_SubGraphKey, value)) { NodeKey old = _SubGraphKey; _SubGraphKey = value; RaisePropertyChanged("SubGraphKey", old, value); } }
    }
    private NodeKey _SubGraphKey;

    /// <summary>
    /// Gets or sets a list of references to member nodes.
    /// </summary>
    /// <value>
    /// By default this is an empty <see cref="ObservableCollection{NodeKey}"/>.
    /// </value>
    public IList<NodeKey> MemberKeys {
      get {
        if (_MemberKeys == null) _MemberKeys = new ObservableCollection<NodeKey>();
        return _MemberKeys;
      }
      set {
        if (_MemberKeys != value && value != null) {
          IList<NodeKey> old = _MemberKeys;
          _MemberKeys = value;
          RaisePropertyChanged("MemberKeys", old, value);
        }
      }
    }
    private IList<NodeKey> _MemberKeys;

    /// <summary>
    /// Gets or sets whether this node is in the "expanded" state.
    /// </summary>
    /// <value>
    /// By default this is true.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public bool IsSubGraphExpanded {
      get { return _IsSubGraphExpanded; }
      set { if (_IsSubGraphExpanded != value) { bool old = _IsSubGraphExpanded; _IsSubGraphExpanded = value; RaisePropertyChanged("IsSubGraphExpanded", old, value); } }
    }
    private bool _IsSubGraphExpanded = true;

    /// <summary>
    /// Gets or sets whether this node had been "expanded" when its containing group was "collapsed".
    /// </summary>
    /// <value>
    /// By default this is false.  This is meaningful only when the container subgraph is not expanded.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public bool WasSubGraphExpanded {
      get { return _WasSubGraphExpanded; }
      set { if (_WasSubGraphExpanded != value) { bool old = _WasSubGraphExpanded; _WasSubGraphExpanded = value; RaisePropertyChanged("WasSubGraphExpanded", old, value); } }
    }
    private bool _WasSubGraphExpanded = false;

    /// <summary>
    /// Gets or sets a <c>Point</c> that is the location of the node in model coordinates.
    /// </summary>
    /// <value>
    /// The default value is (NaN, NaN).
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public Point Location {
      get { return _Location; }
      set {
        if (_Location != value) {
          // NaN != NaN, but don't treat them as different!
          if (Double.IsNaN(_Location.X) && Double.IsNaN(value.X) && Double.IsNaN(_Location.Y) && Double.IsNaN(value.Y)) return;
          Point old = _Location;
          _Location = value;
          RaisePropertyChanged("Location", old, value);
        }
      }
    }
    private Point _Location = new Point(Double.NaN, Double.NaN);

    /// <summary>
    /// Gets or sets a <c>String</c> that is associated with the node.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public String Text {
      get { return _Text; }
      set { if (_Text != value) { String old = _Text; _Text = value; RaisePropertyChanged("Text", old, value); } }
    }
    private String _Text = "";
  }


  /// <summary>
  /// A simple representation of link data
  /// that supports property change notification, copying, and undo
  /// via the <c>INotifyPropertyChanged</c>, <c>ICloneable</c>, and <see cref="IChangeDataValue"/> interfaces.
  /// </summary>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data in the model</typeparam>
  /// <typeparam name="PortKey">the Type of an optional value that helps distinguish different "ports" on a node</typeparam>
  /// <remarks>
  /// <para>
  /// This provides a standard implementation of
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
  /// data that represents links,
  /// including properties that are "references" to the two nodes at each end
  /// of the link and optional "port" parameter information at both ends.
  /// You can use this class if you do not already have your own application class
  /// holding information about links and if you want to inherit from an existing
  /// class so that you can just add your own properties.  Here's a simple example:
  /// </para>
  /// <para>
  /// <code>
  ///  [Serializable]
  ///  public class MyLinkData : GraphLinksModelLinkData&lt;String, String&gt; {
  ///    public double Cost {
  ///      get { return _Cost; }
  ///      set { if (_Cost != value) { double old = _Cost; _Cost = value; RaisePropertyChanged("Cost", old, value); } }
  ///    }
  ///    private double _Cost;
  ///  }
  /// </code>
  /// This associates a number with each link so that you can bind values to this property in a <c>DataTemplate</c>.
  /// For example, look at the <c>TextBlock</c>'s binding of <c>Text</c> in this template:
  /// <code>
  /// &lt;DataTemplate x:Key="LinkTemplate"&gt;
  ///   &lt;go:LinkPanel go:Part.SelectionElementName="Path" go:Part.SelectionAdorned="True"&gt;
  ///     &lt;go:LinkShape x:Name="Path" go:LinkPanel.IsLinkShape="True" Stroke="Black" StrokeThickness="1" /&gt;
  ///     &lt;Path Fill="Black" go:LinkPanel.ToArrow="Standard" /&gt;
  ///     &lt;TextBlock Text="{Binding Path=Data.Cost}" /&gt;
  ///   &lt;/go:LinkPanel&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// (In Silverlight, replace the <c>go:LinkShape</c> with <c>Path</c>.)
  /// </para>
  /// <para>
  /// Note that property setters need to raise the model's Changed event,
  /// so that the model knows about changes in the data and can then update the diagram.
  /// You should call <see cref="RaisePropertyChanged"/> only when the value has actually changed,
  /// and you should pass both the previous and the new values, in order to support undo/redo.
  /// </para>
  /// <para>
  /// For WPF the properties that you define should also be serializable,
  /// in order for the data to be copiable, especially to and from the clipboard.
  /// For both Silverlight and WPF you should override the <see cref="Clone"/> method.
  /// </para>
  /// <para>
  /// In the sample above,
  /// the <typeparamref name="NodeKey"/> is declared to be a string,
  /// which is used as the way to uniquely refer to node data.
  /// The <typeparamref name="PortKey"/> is declared to be a string,
  /// which is useful if you want to distinguish between multiple "ports" on a single node.
  /// However, if your nodes only have a single port, you can ignore the port
  /// parameter information, and the <typeparamref name="PortKey"/> type does not really matter,
  /// although <c>string</c> is recommended for compatibility with the implementation of <see cref="Node"/>
  /// methods that deal with ports.
  /// </para>
  /// <para>
  /// This class is not useful with <see cref="GraphModel{NodeType, NodeKey}"/> or <see cref="TreeModel{NodeType, NodeKey}"/>.
  /// </para>
  /// </remarks>
  [Serializable]
  public class GraphLinksModelLinkData<NodeKey, PortKey> : INotifyPropertyChanged, IChangeDataValue {
    /// <summary>
    /// The default constructor produces an empty object with no references to nodes.
    /// </summary>
    public GraphLinksModelLinkData() { }
    /// <summary>
    /// This constructor initializes the <see cref="From"/> and <see cref="To"/> properties.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public GraphLinksModelLinkData(NodeKey from, NodeKey to) { _FromKey = from; _ToKey = to; }
    /// <summary>
    /// This constructor initializes the <see cref="From"/> and <see cref="To"/> properties.
    /// </summary>
    /// <param name="from">a node key identifying the node data from which the link comes</param>
    /// <param name="fromport">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="to">a node key identify the node data to which the link goes</param>
    /// <param name="toport">an optional value identifying which port on the "to" node the link is connected to</param>
    public GraphLinksModelLinkData(NodeKey from, PortKey fromport, NodeKey to, PortKey toport) { _FromKey = from; _ToKey = to; _FromPort = fromport; _ToPort = toport; }

    /// <summary>
    /// Create a copy of this data; this implements the <c>ICloneable</c> interface.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// When you add your own state in a subclass, and when you expect to be able to copy the data,
    /// you must override this method and make sure the additional state is copied to the
    /// newly cloned object returned by calling the base method.
    /// This reinitializes the <see cref="From"/> and <see cref="To"/> properties.
    /// </remarks>
    public virtual object Clone() {
      GraphLinksModelLinkData<NodeKey, PortKey> d = (GraphLinksModelLinkData<NodeKey, PortKey>)MemberwiseClone();
      d.PropertyChanged = null;
      d._FromKey = default(NodeKey);
      d._ToKey = default(NodeKey);
      d._LabelNode = default(NodeKey);
      // can't share the collection of Points
      if (_Points != null) d._Points = new List<Point>(_Points);
      return d;
    }

    /// <summary>
    /// This event implements the <see cref="INotifyPropertyChanged"/> interface,
    /// so that both the model and the dependency object system can be informed
    /// of changes to property values.
    /// </summary>
    [field: NonSerializedAttribute()]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    public virtual void OnPropertyChanged(ModelChangedEventArgs e) {
      if (this.PropertyChanged != null) this.PropertyChanged(this, e);
    }

    /// <summary>
    /// Call this method from property setters to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="pname">the property name</param>
    /// <param name="oldval">the value before the property was set</param>
    /// <param name="newval">the new value</param>
    /// <remarks>
    /// Only call this method when the property value actually changes.
    /// The <paramref name="oldval"/> and <paramref name="newval"/> values are needed
    /// to support undo/redo.
    /// </remarks>
    protected void RaisePropertyChanged(String pname, Object oldval, Object newval) {
      OnPropertyChanged(new ModelChangedEventArgs(pname, this, oldval, newval));
    }

    /// <summary>
    /// For debugging, use the <see cref="From"/> and <see cref="To"/> properties
    /// as this object's default text rendering.
    /// </summary>
    /// <returns>a String like "Node1 --> Node2"</returns>
    public override string ToString() {
      String s = "";
      if (this.From != null) s += this.From;
      if (this.FromPort != null) s += "(" + this.FromPort.ToString() + ")";
      if (this.To != null) s += " --> " + this.To;
      if (this.ToPort != null) s += "(" + this.ToPort.ToString() + ")";
      if (this.LabelNode != null) s += " lab: " + this.LabelNode.ToString();
      return s;
    }

    /// <summary>
    /// This method implements the <see cref="IChangeDataValue"/> interface,
    /// used to perform state changes for undo and redo.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    /// <remarks>
    /// Unless you override this method to explicitly handle each property that you define,
    /// this implementation uses reflection to set the property.
    /// </remarks>
    public virtual void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.PropertyName == "From") {
        this.From = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "To") {
        this.To = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "FromPort") {
        this.FromPort = (PortKey)e.GetValue(undo);
      } else if (e.PropertyName == "ToPort") {
        this.ToPort = (PortKey)e.GetValue(undo);
      } else if (e.PropertyName == "LabelNode") {
        this.LabelNode = (NodeKey)e.GetValue(undo);
      } else if (e.PropertyName == "Text") {
        this.Text = (String)e.GetValue(undo);
      } else if (e.PropertyName == "Points") {
        this.Points = (IEnumerable<Point>)e.GetValue(undo);
      } else if (e.PropertyName == "Category") {
        this.Category = (String)e.GetValue(undo);
      } else if (e.Change == ModelChange.Property) {
        if (!ModelHelper.SetProperty(e.PropertyName, e.Data, e.GetValue(undo))) {
          ModelHelper.Error("ERROR: Unrecognized property name: " + e.PropertyName != null ? e.PropertyName : "(noname)" + " in GraphLinksModelLinkData.ChangeDataValue");
        }
      }
    }


    /// <summary>
    /// Constructs a Linq for XML <c>XElement</c> holding the data of this link.
    /// </summary>
    /// <param name="n">the name of the new <c>XElement</c></param>
    /// <returns>an initialized <c>XElement</c></returns>
    /// <remarks>
    /// <para>
    /// This constructs a new <c>XElement</c> and adds an <c>XAttribute</c> for each simple property
    /// that has a value different from its default value.
    /// For each property that is a collection, it adds an <c>XElement</c> with nested item elements.
    /// This does not add an element if the collection is empty.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertNodeKeyToString"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual XElement MakeXElement(XName n) {
    ///   XElement e = new XElement(n);
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("From", this.From, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("To", this.To, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute&lt;PortKey&gt;("FromPort", this.FromPort, default(PortKey), ConvertPortKeyToString));
    ///   e.Add(XHelper.Attribute&lt;PortKey&gt;("ToPort", this.ToPort, default(PortKey), ConvertPortKeyToString));
    ///   e.Add(XHelper.Attribute&lt;NodeKey&gt;("LabelNode", this.LabelNode, default(NodeKey), ConvertNodeKeyToString));
    ///   e.Add(XHelper.Attribute("Category", this.Category, ""));
    ///   e.Add(XHelper.Attribute("Text", this.Text, ""));
    ///   e.Add(XHelper.Attribute("Points", this.Points, null));
    ///   return e;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this link data class, and if you are using the
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Save"/> and
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Load(XContainer, XName, XName)"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="LoadFromXElement"/>.
    /// </para>
    /// </remarks>
#if !WINDOWS_UWP
    public virtual XElement MakeXElement(XName n) {
      XElement e = new XElement(n);
      e.Add(XHelper.Attribute<NodeKey>("From", this.From, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Attribute<NodeKey>("To", this.To, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Attribute<PortKey>("FromPort", this.FromPort, default(PortKey), ConvertPortKeyToString));
      e.Add(XHelper.Attribute<PortKey>("ToPort", this.ToPort, default(PortKey), ConvertPortKeyToString));
      e.Add(XHelper.Attribute<NodeKey>("LabelNode", this.LabelNode, default(NodeKey), ConvertNodeKeyToString));
      e.Add(XHelper.Attribute("Category", this.Category, ""));
      e.Add(XHelper.Attribute("Text", this.Text, ""));
      e.Add(XHelper.Attribute("Points", this.Points, null));
      return e;
    }
#endif
    /// <summary>
    /// Initialize this link data with data held in a Linq for XML <c>XElement</c>.
    /// </summary>
    /// <param name="e">the <c>XElement</c></param>
    /// <remarks>
    /// <para>
    /// This sets this link data's properties by reading the data from attributes and nested elements
    /// of the given <c>XElement</c>.
    /// </para>
    /// <para>
    /// Because the <typeparamref name="NodeKey"/> type might be a type for which we have an implementation
    /// to convert to and from strings for XML, this calls the <see cref="ConvertStringToNodeKey"/> method,
    /// which you can override.
    /// </para>
    /// <para>
    /// This is implemented as:
    /// <code>
    /// public virtual void LoadFromXElement(XElement e) {
    ///   this.From = XHelper.Read&lt;NodeKey&gt;("From", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.To = XHelper.Read&lt;NodeKey&gt;("To", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.FromPort = XHelper.Read&lt;PortKey&gt;("FromPort", e, default(PortKey), ConvertStringToPortKey);
    ///   this.ToPort = XHelper.Read&lt;PortKey&gt;("ToPort", e, default(PortKey), ConvertStringToPortKey);
    ///   this.LabelNode = XHelper.Read&lt;NodeKey&gt;("LabelNode", e, default(NodeKey), ConvertStringToNodeKey);
    ///   this.Category = XHelper.Read("Category", e, "");
    ///   this.Text = XHelper.Read("Text", e, "");
    ///   this.Points = XHelper.Read("Points", e, (IEnumerable&lt;Point&gt;)null);
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// If you add properties to this link data class, and if you are using the
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Save"/> and
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.Load(XContainer, XName, XName)"/> methods,
    /// you should override this method to add new attributes and/or elements as needed,
    /// and you should override <see cref="MakeXElement"/>.
    /// </para>
    /// </remarks>
    public virtual void LoadFromXElement(XElement e) {
      this.From = XHelper.Read<NodeKey>("From", e, default(NodeKey), ConvertStringToNodeKey);
      this.To = XHelper.Read<NodeKey>("To", e, default(NodeKey), ConvertStringToNodeKey);
      this.FromPort = XHelper.Read<PortKey>("FromPort", e, default(PortKey), ConvertStringToPortKey);
      this.ToPort = XHelper.Read<PortKey>("ToPort", e, default(PortKey), ConvertStringToPortKey);
      this.LabelNode = XHelper.Read<NodeKey>("LabelNode", e, default(NodeKey), ConvertStringToNodeKey);
      this.Category = XHelper.Read("Category", e, "");
      this.Text = XHelper.Read("Text", e, "");
#if !WINDOWS_UWP
            this.Points = XHelper.Read("Points", e, (IEnumerable<Point>)null);
#endif
    }

    /// <summary>
    /// Convert a <typeparamref name="NodeKey"/> key value to a string.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>a String from which <see cref="ConvertStringToNodeKey"/> can recover the original key value</returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual String ConvertNodeKeyToString(NodeKey key) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return XHelper.ToString((String)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return XHelper.ToString((int)(Object)key);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return XHelper.ToString((Guid)(Object)key);
      } else {
        ModelHelper.Error("Cannot convert NodeKey type to String: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelLinkData.ConvertNodeKeyToString");
        return key.ToString();
      }
    }

    /// <summary>
    /// Convert a <typeparamref name="PortKey"/> key value to a string.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>a String from which <see cref="ConvertStringToPortKey"/> can recover the original key value</returns>
    /// <remarks>
    /// Currently this handles PortKey types that are String, Int32, or Guid.
    /// Override this method to handle additional types.
    /// However, the PortKey is almost always a String, so the need to override this method is rare.
    /// </remarks>
    protected virtual String ConvertPortKeyToString(PortKey key) {
      if (typeof(PortKey).IsAssignableFrom(typeof(String))) {
        return XHelper.ToString((String)(Object)key);
      } else if (typeof(PortKey).IsAssignableFrom(typeof(int))) {
        return XHelper.ToString((int)(Object)key);
      } else if (typeof(PortKey).IsAssignableFrom(typeof(Guid))) {
        return XHelper.ToString((Guid)(Object)key);
      } else {
        ModelHelper.Error("Cannot convert PortKey type to String: " + typeof(PortKey).ToString() + " -- override GraphLinksModelLinkData.ConvertPortKeyToString");
        return key.ToString();
      }
    }

    /// <summary>
    /// Convert a string to a <typeparamref name="NodeKey"/> key value.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>a <typeparamref name="NodeKey"/></returns>
    /// <remarks>
    /// Currently this handles NodeKey types that are String, Int32, or Guid.
    /// Override this method to handle additional types.
    /// </remarks>
    protected virtual NodeKey ConvertStringToNodeKey(String s) {
      if (typeof(NodeKey).IsAssignableFrom(typeof(String))) {
        return (NodeKey)(Object)XHelper.ToString(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(int))) {
        return (NodeKey)(Object)XHelper.ToInt32(s);
      } else if (typeof(NodeKey).IsAssignableFrom(typeof(Guid))) {
        return (NodeKey)(Object)XHelper.ToGuid(s);
      } else {
        ModelHelper.Error("Cannot convert String to NodeKey type: " + typeof(NodeKey).ToString() + " -- override GraphLinksModelData.ConvertStringToNodeKey");
        return default(NodeKey);
      }
    }

    /// <summary>
    /// Convert a string to a <typeparamref name="PortKey"/> key value.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>a <typeparamref name="PortKey"/></returns>
    /// <remarks>
    /// Currently this handles PortKey types that are String, Int32, or Guid.
    /// Override this method to handle additional types.
    /// However, the PortKey is almost always a String, so the need to override this method is rare.
    /// </remarks>
    protected virtual PortKey ConvertStringToPortKey(String s) {
      if (typeof(PortKey).IsAssignableFrom(typeof(String))) {
        return (PortKey)(Object)XHelper.ToString(s);
      } else if (typeof(PortKey).IsAssignableFrom(typeof(int))) {
        return (PortKey)(Object)XHelper.ToInt32(s);
      } else if (typeof(PortKey).IsAssignableFrom(typeof(Guid))) {
        return (PortKey)(Object)XHelper.ToGuid(s);
      } else {
        ModelHelper.Error("Cannot convert String to PortKey type: " + typeof(PortKey).ToString() + " -- override GraphLinksModelData.ConvertStringToPortKey");
        return default(PortKey);
      }
    }


    /// <summary>
    /// Gets or sets a <c>String</c> that names the category to which the link data belongs.
    /// </summary>
    /// <value>
    /// <para>
    /// The default value is an empty string.
    /// </para>
    /// </value>
    public String Category {
      get { return _Category; }
      set { if (_Category != value) { String old = _Category; _Category = value; RaisePropertyChanged("Category", old, value); } }
    }
    private String _Category = "";

    /// <summary>
    /// Gets or sets the key value of the node from which this link comes.
    /// </summary>
    public NodeKey From {
      get { return _FromKey; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_FromKey, value)) { NodeKey old = _FromKey; _FromKey = value; RaisePropertyChanged("From", old, value); } }
    }
    private NodeKey _FromKey;

    /// <summary>
    /// Gets or sets the key value of the node to which this link goes.
    /// </summary>
    public NodeKey To {
      get { return _ToKey; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_ToKey, value)) { NodeKey old = _ToKey; _ToKey = value; RaisePropertyChanged("To", old, value); } }
    }
    private NodeKey _ToKey;

    /// <summary>
    /// Gets or sets the optional parameter information for the <see cref="From"/> node's port.
    /// </summary>
    public PortKey FromPort {
      get { return _FromPort; }
      set { if (!EqualityComparer<PortKey>.Default.Equals(_FromPort, value)) { PortKey old = _FromPort; _FromPort = value; RaisePropertyChanged("FromPort", old, value); } }
    }
    private PortKey _FromPort;

    /// <summary>
    /// Gets or sets the optional parameter information for the <see cref="To"/> node's port.
    /// </summary>
    public PortKey ToPort {
      get { return _ToPort; }
      set { if (!EqualityComparer<PortKey>.Default.Equals(_ToPort, value)) { PortKey old = _ToPort; _ToPort = value; RaisePropertyChanged("ToPort", old, value); } }
    }
    private PortKey _ToPort;

    /// <summary>
    /// Gets or sets the key value of the node which is on this link acting as if it were a label.
    /// </summary>
    public NodeKey LabelNode {
      get { return _LabelNode; }
      set { if (!EqualityComparer<NodeKey>.Default.Equals(_LabelNode, value)) { NodeKey old = _LabelNode; _LabelNode = value; RaisePropertyChanged("LabelNode", old, value); } }
    }
    private NodeKey _LabelNode;

    /// <summary>
    /// Gets or sets a string that can be used for a text label on a link.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public String Text {
      get { return _Text; }
      set { if (_Text != value) { String old = _Text; _Text = value; RaisePropertyChanged("Text", old, value); } }
    }
    private String _Text = "";

    /// <summary>
    /// Gets or sets a collection of <c>Point</c>s used to define the path of the link.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// Although this data property is defined for your convenience, the model does not know about this property.
    /// </remarks>
    public IEnumerable<Point> Points {
      get { return _Points; }
      set { if (_Points != value) { IEnumerable<Point> old = _Points; _Points = value; RaisePropertyChanged("Points", old, value); } }
    }
    private IEnumerable<Point> _Points;
  }


  /// <summary>
  /// This simple class is handy with many <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>s
  /// or with <see cref="UniversalGraphLinksModel"/>, so that for many cases you do not need to define
  /// your own link data class inheriting from <see cref="GraphLinksModelLinkData{Object, String}"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The node data type can be any <c>Object</c>; the (optional) port parameter information is assumed
  /// to be of type <c>String</c>.
  /// There are constructors for most of the common uses.
  /// </para>
  /// <para>
  /// For reasons of both compile-time type checking and run-time efficiency,
  /// we recommend defining your own data class derived from <see cref="GraphLinksModelLinkData{NodeKey, PortKey}"/>.
  /// Doing so also permits the addition of application-specific properties.
  /// </para>
  /// <para>
  /// This class is not useful with <see cref="GraphModel{NodeType, NodeKey}"/> or <see cref="TreeModel{NodeType, NodeKey}"/>.
  /// </para>
  /// </remarks>
  [Serializable]
  public sealed class UniversalLinkData : GraphLinksModelLinkData<Object, String> {
    /// <summary>
    /// The default constructor produces link-representing data that needs to be initialized.
    /// </summary>
    public UniversalLinkData() { }

    /// <summary>
    /// This constructor produces a link-representing data that connects two nodes, identified by their node data keys.
    /// </summary>
    /// <param name="fromnodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.From"/></param>
    /// <param name="tonodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.To"/></param>
    public UniversalLinkData(Object fromnodekey, Object tonodekey) {
      this.From = fromnodekey;
      this.To = tonodekey;
    }

    /// <summary>
    /// This constructor produces a link-representing data that connects two nodes,
    /// identified by their node data keys, and the text for a label annotation.
    /// </summary>
    /// <param name="fromnodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.From"/></param>
    /// <param name="tonodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.To"/></param>
    /// <param name="labeltext">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.Text"/></param>
    public UniversalLinkData(Object fromnodekey, Object tonodekey, String labeltext) {
      this.From = fromnodekey;
      this.To = tonodekey;
      this.Text = labeltext;
    }

    /// <summary>
    /// This constructor produces a link-representing data that connects two nodes and also provides port-identifying information at each node.
    /// </summary>
    /// <param name="fromnodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.From"/></param>
    /// <param name="fromport">the initial string value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.FromPort"/></param>
    /// <param name="tonodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.To"/></param>
    /// <param name="toport">the initial string value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.ToPort"/></param>
    public UniversalLinkData(Object fromnodekey, String fromport, Object tonodekey, String toport) {
      this.From = fromnodekey;
      this.FromPort = fromport;
      this.To = tonodekey;
      this.ToPort = toport;
    }

    /// <summary>
    /// This constructor produces a link-representing data that connects two nodes and
    /// also provides port-identifying information at each node and the text for a label annotation.
    /// </summary>
    /// <param name="fromnodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.From"/></param>
    /// <param name="fromport">the initial string value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.FromPort"/></param>
    /// <param name="tonodekey">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.To"/></param>
    /// <param name="toport">the initial string value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.ToPort"/></param>
    /// <param name="labeltext">the initial value for <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.Text"/></param>
    public UniversalLinkData(Object fromnodekey, String fromport, Object tonodekey, String toport, String labeltext) {
      this.From = fromnodekey;
      this.FromPort = fromport;
      this.To = tonodekey;
      this.ToPort = toport;
      this.Text = labeltext;
    }
  }
}
