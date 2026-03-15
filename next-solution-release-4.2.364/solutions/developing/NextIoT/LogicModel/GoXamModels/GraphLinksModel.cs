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
using System.Reflection;
using System.Xml.Linq;

namespace Northwoods.GoXam.Model {  // don't clutter the regular namespace with these huge parameterized types

  /// <summary>
  /// The generic implementation of a diagram model consisting of nodes and subgraphs,
  /// with any number of explicit link data representing links between any two nodes.
  /// </summary>
  /// <typeparam name="NodeType">the Type of node data</typeparam>
  /// <typeparam name="NodeKey">the Type of a value uniquely identifying a node data</typeparam>
  /// <typeparam name="PortKey">the Type of an optional value identifying a particular port on a node; should be <c>String</c></typeparam>
  /// <typeparam name="LinkType">the Type of link data</typeparam>
  /// <seealso cref="IDiagramModel"/>
  public class GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> : DiagramModel, ILinksModel, ISubGraphLinksModel {

    // model state
    private System.Collections.IEnumerable _NodesSource;

    private PropPathInfo<NodeKey> _NodeKeyPathPPI;
    private bool _NodeKeyIsNodeData;
    private bool _NodeKeyReferenceAutoInserts;
    private PropPathInfo<String> _NodeCategoryPathPPI;
    private PropPathInfo<bool> _NodeIsGroupPathPPI;
    private PropPathInfo<NodeKey> _GroupNodePathPPI;
    private PropPathInfo<System.Collections.IEnumerable> _MemberNodesPathPPI;
    private PropPathInfo<bool> _NodeIsLinkLabelPathPPI;

    private Dictionary<NodeType, NodeInfo> _NodeInfos; // nodedata --> NodeInfo
    private Dictionary<NodeKey, NodeInfo> _IndexedNodes;  // key --> NodeInfo
    private List<NodeType> _BindingListNodes;  // list of NodeType, only used if NodesSource is IBindingList
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedGroupInfos;
    private Dictionary<NodeKey, List<NodeInfo>> _DelayedMemberInfos;
    private Dictionary<System.Collections.IEnumerable, NodeInfo> _ObservedMemberKeyCollections;
    private NodeInfo _LastRemovedNodeInfo;

    private System.Collections.IEnumerable _LinksSource;

    private PropPathInfo<NodeKey> _LinkFromPathPPI;
    private PropPathInfo<PortKey> _LinkFromParameterPathPPI;
    private PropPathInfo<NodeKey> _LinkToPathPPI;
    private PropPathInfo<PortKey> _LinkToParameterPathPPI;
    private PropPathInfo<NodeKey> _LinkLabelNodePathPPI;
    private PropPathInfo<String> _LinkCategoryPathPPI;

    private Dictionary<LinkType, LinkInfo> _LinkInfos;
    private Dictionary<NodeKey, List<LinkInfo>> _DelayedLinkInfos;
    private List<LinkType> _BindingListLinks;  // list of LinkType, only used if LinksSource is IBindingList
    private LinkInfo _LastRemovedLinkInfo;

    private bool _CopyingGroupCopiesMembers = true;
    private bool _RemovingGroupRemovesMembers = true;
    private bool _CopyingLinkCopiesLabel = true;
    private bool _RemovingLinkRemovesLabel = true;

    private ValidCycle _ValidCycle = ValidCycle.All;
    private ValidUnconnectedLinks _ValidUnconnectedLinks = ValidUnconnectedLinks.None;

    private ModelDelegates _Delegates;


    private static readonly IEnumerable<NodeType> NoNodes = new NodeType[0] { };
    private static readonly IEnumerable<LinkType> NoLinks = new LinkType[0] { };


    // this sets all fields that require new values
    // this does not set any fields whose values want to be retained by CreateInitializedCopy
    private void Init() {
      this.Initializing = true;
      Reinitialize();

      var nodescoll = new ObservableCollection<NodeType>();
      nodescoll.CollectionChanged += NodesSource_CollectionChanged;
      this.NodesSourceNotifies = true;
      _NodesSource = nodescoll;

      _NodeKeyPathPPI = new PropPathInfo<NodeKey>("");
      _NodeCategoryPathPPI = new PropPathInfo<String>("");
      _NodeIsGroupPathPPI = new PropPathInfo<bool>("");
      _GroupNodePathPPI = new PropPathInfo<NodeKey>("");
      _MemberNodesPathPPI = new PropPathInfo<System.Collections.IEnumerable>("");
      _NodeIsLinkLabelPathPPI = new PropPathInfo<bool>("");

      _NodeInfos = new Dictionary<NodeType, NodeInfo>();
      _IndexedNodes = new Dictionary<NodeKey, NodeInfo>();
      _BindingListNodes = new List<NodeType>();
      _DelayedGroupInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _DelayedMemberInfos = new Dictionary<NodeKey, List<NodeInfo>>();
      _ObservedMemberKeyCollections = new Dictionary<System.Collections.IEnumerable, NodeInfo>();

      var linkscoll = new ObservableCollection<LinkType>();
      linkscoll.CollectionChanged += LinksSource_CollectionChanged;
      this.LinksSourceNotifies = true;
      _LinksSource = linkscoll;

      _LinkFromPathPPI = new PropPathInfo<NodeKey>("");
      _LinkFromParameterPathPPI = new PropPathInfo<PortKey>();
      _LinkToPathPPI = new PropPathInfo<NodeKey>("");
      _LinkToParameterPathPPI = new PropPathInfo<PortKey>();
      _LinkLabelNodePathPPI = new PropPathInfo<NodeKey>("");
      _LinkCategoryPathPPI = new PropPathInfo<String>("");

      _LinkInfos = new Dictionary<LinkType, LinkInfo>();
      _DelayedLinkInfos = new Dictionary<NodeKey, List<LinkInfo>>();
      _BindingListLinks = new List<LinkType>();

      _Delegates = new ModelDelegates();
    }


    /// <summary>
    /// The default constructor produces an empty model.
    /// </summary>
    public GraphLinksModel() {
      Init();
      if (typeof(GraphLinksModelNodeData<NodeKey>).IsAssignableFrom(typeof(NodeType))) {
        this.NodeKeyPath = "Key";
        this.NodeCategoryPath = "Category";
        this.NodeIsLinkLabelPath = "IsLinkLabel";
        this.NodeIsGroupPath = "IsSubGraph";
        this.GroupNodePath = "SubGraphKey";
        this.MemberNodesPath = "MemberKeys";
      }
      if (typeof(GraphLinksModelLinkData<NodeKey, PortKey>).IsAssignableFrom(typeof(LinkType)) ||
          typeof(UniversalLinkData).IsAssignableFrom(typeof(LinkType))) {
        this.LinkCategoryPath = "Category";
        this.LinkFromPath = "From";
        this.LinkFromParameterPath = "FromPort";
        this.LinkToPath = "To";
        this.LinkToParameterPath = "ToPort";
        this.LinkLabelNodePath = "LabelNode";
      }
      this.Initializing = false;
    }


    /// <summary>
    /// Make a copy of this model, without sharing the <see cref="NodesSource"/> or <see cref="LinksSource"/> collections.
    /// </summary>
    /// <param name="init">
    /// This is a <see cref="DataCollection"/> that provides the initial node and link data.
    /// (Such data is not copied.)
    /// If this is null, the initial <see cref="NodesSource"/> and <see cref="LinksSource"/> values are empty collections.
    /// </param>
    /// <returns>a model just like this one, but with different data</returns>
    public virtual GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> CreateInitializedCopy(DataCollection init) {
      GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> m = (GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>)MemberwiseClone();
      m.Init();
      m._Delegates = _Delegates.Clone();
      m.NodeKeyIsNodeData = this.NodeKeyIsNodeData;
      m.NodeKeyReferenceAutoInserts = this.NodeKeyReferenceAutoInserts;

      m.NodeKeyPath = this.NodeKeyPath;
      m.NodeCategoryPath = this.NodeCategoryPath;
      m.NodeIsGroupPath = this.NodeIsGroupPath;
      m.GroupNodePath = this.GroupNodePath;
      m.MemberNodesPath = this.MemberNodesPath;
      m.NodeIsLinkLabelPath = this.NodeIsLinkLabelPath;
      m.LinkFromPath = this.LinkFromPath;
      m.LinkFromParameterPath = this.LinkFromParameterPath;
      m.LinkToPath = this.LinkToPath;
      m.LinkToParameterPath = this.LinkToParameterPath;
      m.LinkLabelNodePath = this.LinkLabelNodePath;
      m.LinkCategoryPath = this.LinkCategoryPath;

      m.ValidCycle = this.ValidCycle;
      m.ValidUnconnectedLinks = this.ValidUnconnectedLinks;
#if DATASET  // DataView
      System.Data.DataView view = this.NodesSource as System.Data.DataView;
      if (view != null) {
        System.Data.DataRowView row = null;
        if (init != null) row = init.Nodes.FirstOrDefault() as System.Data.DataRowView;
        if (row != null) {
          m.NodesSource = row.DataView;
        } else {
          System.Data.DataTable table = view.Table.Clone();
          m.NodesSource = table.DefaultView;
        }
      } else
#endif
      {
        ObservableCollection<NodeType> coll = (ObservableCollection<NodeType>)m.NodesSource;
        if (init != null) {
          foreach (NodeType n in init.Nodes) coll.Add(n);
        }
      }
#if DATASET  // DataView
      view = this.LinksSource as System.Data.DataView;
      if (view != null) {
        System.Data.DataRowView row = null;
        if (init != null) row = init.Links.FirstOrDefault() as System.Data.DataRowView;
        if (row != null) {
          m.LinksSource = row.DataView;
        } else {
          System.Data.DataTable table = view.Table.Clone();
          m.LinksSource = table.DefaultView;
        }
      } else
#endif
      {
        ObservableCollection<LinkType> coll = (ObservableCollection<LinkType>)m.LinksSource;
        if (init != null) {
          foreach (LinkType l in init.Links) coll.Add(l);
        }
      }
      m.Initializing = false;
      return m;
    }
    IDiagramModel IDiagramModel.CreateInitializedCopy(IDataCollection init) { return CreateInitializedCopy((DataCollection)init); }


    // Nodes data

    /// <summary>
    /// Gets or sets the collection of node data items for the model.
    /// </summary>
    /// <value>
    /// Initially this value is an empty <c>ObservableCollection</c>.
    /// It cannot be set to a null value.
    /// For generality, this is of type <see cref="System.Collections.IEnumerable"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodesSource"/>.
    /// </para>
    /// </remarks>
    public System.Collections.IEnumerable NodesSource {
      get { return _NodesSource; }
      set {
        System.Collections.IEnumerable old = _NodesSource;
        if (value == null) ModelHelper.Error("Cannot set NodesSource to null");
        if (old != value && value != null) {
          INotifyCollectionChanged oldsource = _NodesSource as INotifyCollectionChanged;
          if (oldsource != null) {
            oldsource.CollectionChanged -= NodesSource_CollectionChanged;
#if DATASET  // IBindingList
          } else {
            IBindingList oldlist = _NodesSource as IBindingList;
            if (oldlist != null) oldlist.ListChanged -= NodesSource_ListChanged;
#endif
          }
          _NodesSource = value;
          INotifyCollectionChanged newsource = _NodesSource as INotifyCollectionChanged;
          if (newsource != null) {
            newsource.CollectionChanged += NodesSource_CollectionChanged;
            this.NodesSourceNotifies = true;
#if DATASET  // IBindingList
          } else {
            IBindingList newlist = _NodesSource as IBindingList;
            if (newlist != null) {
              newlist.ListChanged += NodesSource_ListChanged;
              this.NodesSourceNotifies = true;
            } else {
              this.NodesSourceNotifies = false;
            }
#else
          } else {
            this.NodesSourceNotifies = false;
#endif
          }
          ResetNodes(false);
          RaiseModelChanged(ModelChange.ChangedNodesSource, null, old, value);
        }
      }
    }

    private bool NodesSourceNotifies { get; set; }

    private void NodesSource_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add:
          foreach (NodeType nodedata in e.NewItems) DoNodeAdded(nodedata);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (NodeType nodedata in e.OldItems) DoNodeRemoved(nodedata);
          break;
        case NotifyCollectionChangedAction.Replace:
          foreach (NodeType nodedata in e.OldItems) DoNodeRemoved(nodedata);
          foreach (NodeType nodedata in e.NewItems) DoNodeAdded(nodedata);
          break;
        case NotifyCollectionChangedAction.Reset: {
            // don't remove and re-add an existing nodedata
            // use HashSet for faster lookup of new/desired
            var newnodes = new HashSet<NodeType>();
            foreach (NodeType nodedata in _NodesSource) newnodes.Add(nodedata);

            var oldnodes = _NodeInfos.Keys.ToArray();
            foreach (NodeType nodedata in oldnodes) {
              // only remove old nodedata if not present in new _NodesSource
              if (!newnodes.Contains(nodedata)) DoNodeRemoved(nodedata);
            }

            // add all nodedata in _NodesSource; ignore if already present in old _NodeInfos
            foreach (NodeType nodedata in _NodesSource) DoNodeAdded(nodedata);
            break;
          }
        default:
          break;
      }
    }

#if DATASET  // IBindingList
    private System.Data.DataRowView AddNewRow(System.Data.DataView view, Object[] itemarray) {
      if (view == null) return null;
      System.Data.DataRowView row = view.AddNew();
      row.Row.ItemArray = itemarray;  // also sets node key
      return row;  // you might need to call DoNodeAdded afterwards
    }

    private void NodesSource_ListChanged(Object sender, ListChangedEventArgs e) {
      IBindingList list = (IBindingList)sender;
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded:
          if (list != null) {
            NodeType data = (NodeType)list[e.NewIndex];
            //DoNodeAdded(data);  // shouldn't call DoNodeAdded with the empty row
            _BindingListNodes.Insert(e.NewIndex, data);
          }
          break;
        case ListChangedType.ItemDeleted:
          if (list != null) {
            NodeType data = _BindingListNodes[e.NewIndex];
            DoNodeRemoved(data);
            _BindingListNodes.RemoveAt(e.NewIndex);
          }
          break;
        case ListChangedType.ItemChanged:
          if (list != null) {
            NodeType data1 = _BindingListNodes[e.NewIndex];
            NodeType data2 = (NodeType)list[e.NewIndex];
            if (!data1.Equals(data2)) {
              DoNodeRemoved(data1);
              DoNodeAdded(data2);
              _BindingListNodes[e.NewIndex] = data2;
            } else {
              PropertyDescriptor desc = e.PropertyDescriptor;
              Object val = desc.GetValue(data1);
              //??? get Original DataRowVersion column value
              String pname = desc.Name;
              ModelChangedEventArgs args = new ModelChangedEventArgs(pname, data1, val, val);
              if (this.UndoManager != null) {
                args.Model = this;
                ModelChangedEventArgs prev = this.UndoManager.FindPreviousChange(args);
                if (prev != null) {
                  args.OldValue = prev.NewValue;
                } //???
              }
              HandleNodePropertyChanged(data1, args);
            }
          }
          break;
        case ListChangedType.Reset: {
            IEnumerable<NodeType> oldnodes = _NodeInfos.Keys.ToArray();
            foreach (NodeType nodedata in oldnodes) DoNodeRemoved(nodedata);
            _BindingListNodes.Clear();
            foreach (NodeType nodedata in _NodesSource) {
              DoNodeAdded(nodedata);
              _BindingListNodes.Insert(e.NewIndex, nodedata);
            }
            break;
          }
        default:
          break;
      }
    }
#endif

    // Model discovery:
    // Property paths for declarative graph navigation

    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for node data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning to use the data as the key value.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindKeyForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String NodeKeyPath {
      get { return _NodeKeyPathPPI.Path; }
      set {
        String old = _NodeKeyPathPPI.Path;
        if (old != value && value != null) {
          _NodeKeyPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeKeyPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the key for node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the (hopefully) unique key for the given node data in this model</returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeKeyPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </para>
    /// <para>
    /// If <see cref="NodeKeyIsNodeData"/> is true, this just converts
    /// the <paramref name="nodedata"/> argument to the <typeparamref name="NodeKey"/> type and returns it.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindKeyForNode(NodeType nodedata) {
      if (this.Delegates.FindKeyForNode != null) {
        return this.Delegates.FindKeyForNode(nodedata);
      }
      if (this.NodeKeyIsNodeData)
        return (NodeKey)((Object)nodedata);
      else
        return _NodeKeyPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets whether all node data are also their own keys.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// For this to be set to true, the <typeparamref name="NodeType"/> type
    /// and the <typeparamref name="NodeKey"/> types must be the same.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  It is an optimization that
    /// avoids the use of an additional hash table mapping
    /// <typeparamref name="NodeKey"/> values to their <typeparamref name="NodeType"/> data,
    /// and permits other optimizations as well.
    /// This can often be set to true when "references" to nodes
    /// are actually using .NET CLR references (i.e. "pointers") instead
    /// of values such as integer, strings, GUIDs et al.
    /// </para>
    /// </remarks>
    public bool NodeKeyIsNodeData {
      get { return _NodeKeyIsNodeData; }
      set {
        if (_NodeKeyIsNodeData != value) {
          if (value) {
            if (typeof(NodeType) == typeof(NodeKey)) {
              _NodeKeyIsNodeData = true;
              ResetNodes(true);
            } else {
              ModelHelper.Error(this, "Cannot set Model.NodeKeyIsNodeData to true when the NodeType and the NodeKey are not the same Type.");
            }
          } else {
            _NodeKeyIsNodeData = false;
            ResetNodes(true);
          }
        }
      }
    }

    /// <summary>
    /// Gets or sets whether a <typeparamref name="NodeKey"/> reference,
    /// when <see cref="NodeKeyIsNodeData"/> is true,
    /// automatically inserts the node into <see cref="NodesSource"/>.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// This property only has an effect when <see cref="NodeKeyIsNodeData"/> is true.
    /// <see cref="DoNodeAdded"/> calls <see cref="InsertNode"/> if the referred-to
    /// node is not already known to be in the <see cref="NodesSource"/> collection.
    /// </remarks>
    public bool NodeKeyReferenceAutoInserts {
      get { return _NodeKeyReferenceAutoInserts; }
      set {
        bool old = _NodeKeyReferenceAutoInserts;
        if (old != value) {
          _NodeKeyReferenceAutoInserts = value;
          //?? RaiseModelChanged(ModelChange.ChangedNodeKeyReferenceAutoInserts, null, old, value);
        }
      }
    }

    /// <summary>
    /// This method is called when a duplicate key has been found.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>
    /// true to accept the data as a node after having made the key unique in the model;
    /// false to ignore this data.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// When a node data is added to the <see cref="NodesSource"/> collection,
    /// <see cref="DoNodeAdded"/> checks for a duplicate key value.
    /// If it is a duplicate key, this method is called to either modify the node
    /// data to have a unique key, or to return false to avoid adding the data
    /// to this model.  (However, the duplicate key node data will still be
    /// in the <see cref="NodesSource"/> collection.)
    /// </para>
    /// <para>
    /// This method is frequently overridden to implement the desired
    /// policy for your particular application model.
    /// </para>
    /// <para>
    /// When the user copies selected nodes into a diagram,
    /// it is likely that this method will be called.
    /// </para>
    /// </remarks>
    protected virtual bool MakeNodeKeyUnique(NodeType nodedata) {
      if (this.Delegates.MakeNodeKeyUnique != null) {
        return this.Delegates.MakeNodeKeyUnique(this, nodedata);
      }
      NodeKey key = FindKeyForNode(nodedata);
      String nodekeypath = this.NodeKeyPath;
      if (nodekeypath != null && nodekeypath.Length > 0) {
        if (ModelHelper.MakeNodeKeyUnique<NodeKey, NodeInfo>(typeof(NodeKey), key, nodekeypath, nodedata, _IndexedNodes, " ", 2)) {
          return true;
        }
      }
      ModelHelper.Error(this, "Found duplicate key '" + (key != null ? key.ToString() : "null") + "' for node; override MakeNodeKeyUnique to modify data");
      return false;
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to find the category of a node data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindCategoryForNode"/> to return an empty string.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindCategoryForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be a string.
    /// </para>
    /// </remarks>
    public String NodeCategoryPath {
      get { return _NodeCategoryPathPPI.Path; }
      set {
        String old = _NodeCategoryPathPPI.Path;
        if (old != value && value != null) {
          _NodeCategoryPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeCategoryPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the category of a node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>by default an empty string</returns>
    /// <remarks>
    /// <para>
    /// This is called on each node data that is added to the model, to decide which template to use.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeCategoryPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual String FindCategoryForNode(NodeType nodedata) {
      if (this.Delegates.FindCategoryForNode != null) {
        return this.Delegates.FindCategoryForNode(nodedata);
      }
      if (nodedata == null) return "";
      if (this.NodeCategoryPath.Length == 0) return "";
      return _NodeCategoryPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to find out whether
    /// a node data is also a "container" group.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindIsGroupForNode"/> to return false.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindIsGroupForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be a boolean.
    /// </para>
    /// </remarks>
    public String NodeIsGroupPath {
      get { return _NodeIsGroupPathPPI.Path; }
      set {
        String old = _NodeIsGroupPathPPI.Path;
        if (old != value && value != null) {
          _NodeIsGroupPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeIsGroupPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find whether a node data is a group or container of other nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is called on each node data that is added to the model, to decide whether the data represents a group node.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeIsGroupPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual bool FindIsGroupForNode(NodeType nodedata) {
      if (this.Delegates.FindIsGroupForNode != null) {
        return this.Delegates.FindIsGroupForNode(nodedata);
      }
      if (nodedata == null) return false;
      if (this.NodeIsGroupPath.Length == 0) return false;
      return _NodeIsGroupPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for "container" or group node data of a node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindGroupKeyForNode"/>.
    /// Otherwise that method is called to try to find the container node for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindGroupKeyForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindGroupKeyForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String GroupNodePath {
      get { return _GroupNodePathPPI.Path; }
      set {
        String old = _GroupNodePathPPI.Path;
        if (old != value) {
          _GroupNodePathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedGroupNodePath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find containing group node key for a given node data, if any.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>
    /// a <typeparamref name="NodeKey"/> for the containing group node,
    /// or the default value for that type if there is no container node for the node
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="GroupNodePath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="GroupNodePath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindGroupKeyForNode(NodeType nodedata) {
      if (this.Delegates.FindGroupKeyForNode != null) {
        return this.Delegates.FindGroupKeyForNode(nodedata);
      }
      return _GroupNodePathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get a list of keys for the "member" nodes of a group node data object.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, meaning not to call <see cref="FindMemberNodeKeysForNode"/>.
    /// Otherwise that method is called to try to find the list of children for each node.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindMemberNodeKeysForNode"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindMemberNodeKeysForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be of type <see cref="System.Collections.IEnumerable"/>,
    /// holding only instances of <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String MemberNodesPath {
      get { return _MemberNodesPathPPI.Path; }
      set {
        String old = _MemberNodesPathPPI.Path;
        if (old != value) {
          _MemberNodesPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedMemberNodesPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the list of keys of the children nodes for a node data object.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the list of child keys, an <see cref="System.Collections.IEnumerable"/> of <typeparamref name="NodeKey"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This is only called if <see cref="MemberNodesPath"/> is not an empty string.
    /// This method can be overridden in case the <see cref="MemberNodesPath"/>
    /// property path is not flexible enough or fast enough to get the collection of child node keys.
    /// </para>
    /// </remarks>
    protected virtual System.Collections.IEnumerable FindMemberNodeKeysForNode(NodeType nodedata) {
      if (this.Delegates.FindMemberNodeKeysForNode != null) {
        return this.Delegates.FindMemberNodeKeysForNode(nodedata);
      }
      return _MemberNodesPathPPI.EvalFor(nodedata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to find out whether
    /// a node data is also a "label" for a link data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindIsLinkLabelForNode"/> to return false.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindIsLinkLabelForNode"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a node data object must be a boolean.
    /// </para>
    /// </remarks>
    public String NodeIsLinkLabelPath {
      get { return _NodeIsLinkLabelPathPPI.Path; }
      set {
        String old = _NodeIsLinkLabelPathPPI.Path;
        if (old != value && value != null) {
          _NodeIsLinkLabelPathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedNodeIsLinkLabelPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find whether a node data is a "label" for a link data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is called on each node data that is added to the model, to decide whether the data represents a label node.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="NodeIsLinkLabelPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual bool FindIsLinkLabelForNode(NodeType nodedata) {
      if (this.Delegates.FindIsLinkLabelForNode != null) {
        return this.Delegates.FindIsLinkLabelForNode(nodedata);
      }
      if (nodedata == null) return false;
      if (this.NodeIsLinkLabelPath.Length == 0) return false;
      return _NodeIsLinkLabelPathPPI.EvalFor(nodedata);
    }


    // Keeping track of nodes

#if DEBUG  // public internals
    public Dictionary<NodeType, NodeInfo> Infos { get { return _NodeInfos; } }
    public Dictionary<NodeKey, NodeInfo> Index { get { return _IndexedNodes; } }
    public Dictionary<NodeKey, List<NodeInfo>> DelayedGroups { get { return _DelayedGroupInfos; } }
    public Dictionary<NodeKey, List<NodeInfo>> DelayedMembers { get { return _DelayedMemberInfos; } }
    public Dictionary<System.Collections.IEnumerable, NodeInfo> MemberKeyCollections { get { return _ObservedMemberKeyCollections; } }

    public void Walk() {
      ModelHelper.Trace("GraphLinksModel<" + typeof(NodeType).Name + "," + typeof(NodeKey).Name + "," + typeof(PortKey).Name + "," + typeof(LinkType).Name + ">");
      ModelHelper.Trace(" " + _NodeKeyPathPPI.Path +
        (_NodeKeyIsNodeData ? " NodeKeyIsNodeData" : "") +
        (_NodeKeyReferenceAutoInserts ? " NodeKeyReferenceAutoInserts" : "") +
        " " + _NodeCategoryPathPPI.Path +
        " " + _NodeIsGroupPathPPI.Path +
        " " + _GroupNodePathPPI.Path +
        " " + _MemberNodesPathPPI.Path +
        " " + _NodeIsLinkLabelPathPPI.Path +
        " " + _LinkFromPathPPI.Path +
        " " + _LinkFromParameterPathPPI.Path +
        " " + _LinkToPathPPI.Path +
        " " + _LinkToParameterPathPPI.Path +
        " " + _LinkLabelNodePathPPI.Path +
        " " + _LinkCategoryPathPPI.Path +
        " " + _ValidCycle.ToString() +
        " " + _ValidUnconnectedLinks.ToString());
      ModelHelper.Trace("NodeInfos: " + _NodeInfos.Count.ToString());
      foreach (NodeInfo ni in _NodeInfos.Values) Walk(ni);
      ModelHelper.Trace("Index: " + _IndexedNodes.Count.ToString());
      if (_DelayedGroupInfos.Count > 0) {
        String s = "DelayedGroups: ";
        foreach (NodeKey k in _DelayedGroupInfos.Keys) s += k.ToString() + " ";
        ModelHelper.Trace(s);
      }
      if (_DelayedMemberInfos.Count > 0) {
        String s = "DelayedMembers: ";
        foreach (NodeKey k in _DelayedMemberInfos.Keys) s += k.ToString() + " ";
        ModelHelper.Trace(s);
      }
      if (_ObservedMemberKeyCollections.Count > 0) {
        String s = "ObservedMemberKeyCollections: ";
        foreach (NodeInfo n in _ObservedMemberKeyCollections.Values) s += n.ToString() + " ";
        ModelHelper.Trace(s);
      }
      ModelHelper.Trace("LinkInfos:");
      foreach (LinkInfo li in _LinkInfos.Values) Walk(li);
      if (_DelayedLinkInfos.Count > 0) {
        String s = "DelayedLinks: ";
        foreach (NodeKey k in _DelayedGroupInfos.Keys) s += k.ToString() + " ";
        ModelHelper.Trace(s);
      }
    }

    public void Walk(NodeInfo ni) {
      ModelHelper.Trace(" NodeInfo " + (ni.IsGroup ? "IsGroup " : "") + (ni.IsLinkLabel ? "IsLinkLabel " : "") + "Category: " + ni.Category);
      ModelHelper.Trace("  Data: " + ni.Data.ToString());
      ModelHelper.Trace("  Key: " + ni.Key.ToString());
      if (ni.ConnectedLinkInfos != null && ni.ConnectedLinkInfos.Count > 0) {
        ModelHelper.Trace("  ConnectedLinkInfos:");
        foreach (LinkInfo li in ni.ConnectedLinkInfos) ModelHelper.Trace("    " + li.ToString());
      }
      if (ni.GroupNodeInfo != null) {
        ModelHelper.Trace("  GroupNodeInfo: " + ni.GroupNodeInfo.ToString());
      }
      if (ni.MemberNodeInfos != null && ni.MemberNodeInfos.Count > 0) {
        ModelHelper.Trace("  MemberNodeInfos:");
        foreach (NodeInfo n in ni.MemberNodeInfos) ModelHelper.Trace("    " + n.ToString());
      }
      if (ni.SavedMemberKeys != null && ni.SavedMemberKeys.Count > 0) {
        ModelHelper.Trace("  SavedMemberKeys:");
        foreach (NodeKey k in ni.SavedMemberKeys) ModelHelper.Trace("    " + k.ToString());
      }
      if (ni.MemberLinkInfos != null && ni.MemberLinkInfos.Count > 0) {
        ModelHelper.Trace("  MemberLinkInfos");
        foreach (LinkInfo li in ni.MemberLinkInfos) ModelHelper.Trace("    " + li.ToString());
      }
      if (ni.LabeledLinkInfo != null) {
        ModelHelper.Trace("  LabeledLinkInfo: " + ni.LabeledLinkInfo.ToString());
      }
    }

    public
#else
    private
#endif
    sealed class NodeInfo {
      public NodeType Data { get; set; }
      public NodeKey Key { get; set; }
      public List<LinkInfo> ConnectedLinkInfos { get; set; }  // links connected at this node, in either direction
      public String Category { get; set; }
      public bool IsGroup { get; set; }
      public NodeInfo GroupNodeInfo { get; set; }  // to containing group
      public List<NodeInfo> MemberNodeInfos { get; set; }  // nodes contained by this group node
      public System.Collections.IEnumerable MemberKeyCollection { get; set; }  // remember for CollectionChanged handler
      public List<NodeKey> SavedMemberKeys { get; set; }  // remember for NotifyCollectionChangedAction.Reset
      public List<LinkInfo> MemberLinkInfos { get; set; }  // links contained by this node
      public bool IsLinkLabel { get; set; }
      public LinkInfo LabeledLinkInfo { get; set; }  // link that uses this node as a label
      public override String ToString() {
        return "NI:" + ((this.Data != null) ? this.Data.ToString() : "(no Data)");
      }
    }

    private NodeInfo FindNodeInfoForNode(NodeType nodedata) {
      if (nodedata == null) return null;  // don't compare with default(NodeType), e.g. to allow finding zero
      NodeInfo ni;
      _NodeInfos.TryGetValue(nodedata, out ni);
      if (ni == null) ni = _LastRemovedNodeInfo;
      return ni;
    }

    private void ResetNodes(bool clear) {
      foreach (var kvp in _NodeInfos) {
        NodeInfo ni = kvp.Value;
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleNodePropertyChanged;
        SetMemberKeyCollectionHandler(ni, null);
      }
      _NodeInfos.Clear();
      _IndexedNodes.Clear();
      _BindingListNodes.Clear();
      _ObservedMemberKeyCollections.Clear();
      ClearUnresolvedReferences();
      bool oldInit = this.Initializing;
      try {
        this.Initializing = true;
#if DATASET  // IBindingList
        IBindingList list = this.NodesSource as IBindingList;
        if (list != null) {
          foreach (NodeType n in list) {
            _BindingListNodes.Add(n);
          }
        }
#endif
        foreach (NodeType nodedata in this.NodesSource) DoNodeAdded(nodedata);
        ResetLinks(clear);
      } finally {
        if (clear) ClearUnresolvedReferences();
        this.Initializing = oldInit;
      }
    }

    /// <summary>
    /// Forget all unresolved delayed or forward references.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The model may learn about node data in any order, so references to
    /// nodes may be unresolvable until later, perhaps never.
    /// Call this method to clear the internal table that keeps track
    /// of <typeparamref name="NodeKey"/>s that are not yet defined.
    /// </para>
    /// <para>
    /// This is called when setting a number of property path properties,
    /// because a model property path change can completely alter the references
    /// each node data might be making.
    /// </para>
    /// </remarks>
    public void ClearUnresolvedReferences() {
      _DelayedGroupInfos.Clear();
      _DelayedMemberInfos.Clear();
      _DelayedLinkInfos.Clear();
    }


    /// <summary>
    /// Cause <see cref="ResolveNodeKey"/> to be called on each
    /// known delayed or forward node reference.
    /// </summary>
    public void ResolveAllReferences() {
      HashSet<NodeKey> keys = new HashSet<NodeKey>(_DelayedGroupInfos.Keys);
      keys.UnionWith(_DelayedMemberInfos.Keys);
      keys.UnionWith(_DelayedLinkInfos.Keys);
      foreach (NodeKey key in keys) {
        ResolveNodeKey(key);
      }
    }

    /// <summary>
    /// This is called repeatedly by <see cref="ResolveAllReferences"/>,
    /// once for each known delayed or forward node reference.
    /// </summary>
    /// <param name="nodekey"></param>
    /// <remarks>
    /// Depending on the situation, you may want to create and
    /// <see cref="InsertNode"/> in order to resolve a reference.
    /// Or you may want to ignore it, and later call
    /// <see cref="ClearUnresolvedReferences"/> to make sure no
    /// future node data addition might resolve the reference.
    /// </remarks>
    protected virtual void ResolveNodeKey(NodeKey nodekey) {
      if (this.Delegates.ResolveNodeKey != null) {
        this.Delegates.ResolveNodeKey(this, nodekey);
        return;
      }
    }


    // delayed references

    // called when a node is added, but its group node reference is unresolved
    private void DelayGroup(NodeInfo memberni, NodeKey groupkey) {
      if (groupkey == null) return;
      List<NodeInfo> members;
      if (_DelayedGroupInfos.TryGetValue(groupkey, out members)) {
        if (!members.Contains(memberni)) members.Add(memberni);
      } else {
        _DelayedGroupInfos.Add(groupkey, new List<NodeInfo>() { memberni });
      }
    }

    private void DelayMember(NodeInfo groupni, NodeKey memberkey) {
      if (memberkey == null) return;
      List<NodeInfo> list;
      if (_DelayedMemberInfos.TryGetValue(memberkey, out list)) {
        if (!list.Contains(groupni)) list.Add(groupni);
      } else {
        _DelayedMemberInfos.Add(memberkey, new List<NodeInfo>() { groupni });
      }
    }

    private void RemoveDelayedNodeInfo(NodeInfo ni) {
#if DATASET  // DataRowView
      if (ni.Data is System.Data.DataRowView) return;
#endif
      if (this.GroupNodePath != "") {
        NodeKey sgkey = FindGroupKeyForNode(ni.Data);
        List<NodeInfo> members;
        if (sgkey != null && _DelayedGroupInfos.TryGetValue(sgkey, out members)) {
          members.Remove(ni);
          if (members.Count == 0) _DelayedGroupInfos.Remove(sgkey);
        }
      }
      if (this.MemberNodesPath != "") {
        System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(ni.Data);
        if (memberkeys != null) {
          foreach (NodeKey memberkey in memberkeys) {
            List<NodeInfo> list;
            if (_DelayedMemberInfos.TryGetValue(memberkey, out list)) {
              list.Remove(ni);
              if (list.Count == 0) _DelayedMemberInfos.Remove(memberkey);
            }
          }
        }
      }
    }

    private void ResolveDelayedNodeInfo(NodeInfo ni) {
      if (ni.Key == null) return;
      List<LinkInfo> linklist;
      if (_DelayedLinkInfos.TryGetValue(ni.Key, out linklist)) {
        foreach (LinkInfo li in linklist) {
          bool added = false;
          if (li.FromNodeInfo == null) {
            NodeKey fromkey = FindFromNodeKeyForLink(li.Data);
            if (EqualityComparer<NodeKey>.Default.Equals(fromkey, ni.Key)) {
              added = true;
              AddLinkInfoToNodeInfo(ni, li, true);
            }
          }
          if (li.ToNodeInfo == null) {
            NodeKey tokey = FindToNodeKeyForLink(li.Data);
            if (EqualityComparer<NodeKey>.Default.Equals(tokey, ni.Key)) {
              added = true;
              AddLinkInfoToNodeInfo(ni, li, false);
            }
          }
          if (added) ReparentLinkToLowestCommonGroup(li);
          if (li.LabelNodeInfo == null && this.LinkLabelNodePath != "") {
            NodeKey labkey = FindLabelNodeKeyForLink(li.Data);
            if (EqualityComparer<NodeKey>.Default.Equals(labkey, ni.Key)) {
              SetLabelNodeToLinkInfo(li, ni);
            }
          }
        }
        _DelayedLinkInfos.Remove(ni.Key);
      }
      List<NodeInfo> list;
      if (_DelayedGroupInfos.TryGetValue(ni.Key, out list)) {
        if (ni.IsGroup) {  // only add members if NI really is a group node
          foreach (NodeInfo mi in list) {
            if (mi.GroupNodeInfo == null) {
              AddGroupMemberInfos(ni, mi);
              if (mi.ConnectedLinkInfos != null) {
                foreach (LinkInfo mli in mi.ConnectedLinkInfos) ReparentLinkToLowestCommonGroup(mli);
              }
            }
          }
        }
        _DelayedGroupInfos.Remove(ni.Key);
      }
      if (_DelayedMemberInfos.TryGetValue(ni.Key, out list)) {
        foreach (NodeInfo groupni in list) {
          AddGroupMemberInfos(groupni, ni);
        }
        _DelayedMemberInfos.Remove(ni.Key);
      }
    }

    private void SetMemberKeyCollectionHandler(NodeInfo ni, System.Collections.IEnumerable newcoll) {
      System.Collections.IEnumerable oldcoll = ni.MemberKeyCollection;
      if (oldcoll != newcoll) {
        if (oldcoll != null) {
          INotifyCollectionChanged ncc = oldcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedMemberKeyCollections.Remove(oldcoll);
            ncc.CollectionChanged -= MemberKey_CollectionChanged;
          }
        }
        ni.MemberKeyCollection = newcoll;
        if (newcoll != null) {
          INotifyCollectionChanged ncc = newcoll as INotifyCollectionChanged;
          if (ncc != null) {
            _ObservedMemberKeyCollections.Add(newcoll, ni);
            ncc.CollectionChanged += MemberKey_CollectionChanged;
          }
        }
      }
      ni.SavedMemberKeys = null;
      if (newcoll != null) {
        ni.SavedMemberKeys = new List<NodeKey>(newcoll.Cast<NodeKey>());
      }
    }

    private void MemberKey_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      System.Collections.IEnumerable coll = (System.Collections.IEnumerable)sender;
      NodeInfo ni;
      if (_ObservedMemberKeyCollections.TryGetValue(coll, out ni)) {
        switch (e.Action) {
          case NotifyCollectionChangedAction.Add:
            foreach (NodeKey nkey in e.NewItems) DoMemberNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Remove:
            foreach (NodeKey nkey in e.OldItems) DoMemberNodeKeyRemoved(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Replace:
            foreach (NodeKey nkey in e.OldItems) DoMemberNodeKeyRemoved(ni.Data, nkey);
            foreach (NodeKey nkey in e.NewItems) DoMemberNodeKeyAdded(ni.Data, nkey);
            break;
          case NotifyCollectionChangedAction.Reset:
            DoMemberNodeKeysChanged(ni.Data);
            break;
          default:
            break;
        }
      }
    }

    // bookkeeping

    private void RemoveNodeInfos(NodeInfo ni) {  // called when a node is removed
      if (ni == null) return;
      if (ni.ConnectedLinkInfos != null) {
        foreach (LinkInfo li in ni.ConnectedLinkInfos) {
          if (li.FromNodeInfo == ni) li.FromNodeInfo = null;
          if (li.ToNodeInfo == ni) li.ToNodeInfo = null;
        }
      }
      // remove references to members and vice-versa
      if (ni.MemberLinkInfos != null) {
        foreach (LinkInfo mi in ni.MemberLinkInfos.ToArray()) {  // work on copy, to avoid modifying during traversal
          RaiseModelChanged(ModelChange.ChangedLinkGroupNodeKey, mi.Data, ni.Key, default(NodeKey));
          RemoveMemberLinkFromNodeInfo(ni, mi);
        }
      }
      // remove relationship with group, if any
      if (ni.GroupNodeInfo != null) {
        NodeInfo cni = ni.GroupNodeInfo;
        if (cni.MemberNodeInfos != null) {
          if (cni.MemberNodeInfos.Remove(ni)) {
            //??? DelayMember(cni, ni.Key);
          }
        }
      }
      // remove references to members and vice-versa
      if (ni.MemberNodeInfos != null) {
        foreach (NodeInfo mi in ni.MemberNodeInfos) {
          if (mi.GroupNodeInfo == ni) {
            mi.GroupNodeInfo = null;
            //??? DelayGroup(mi, ni.Key);
          }
        }
      }
    }

    private bool AddGroupMemberInfos(NodeInfo sgi, NodeInfo ni) {
      if (sgi == null) return false;
      if (ni == null) return false;
      if (!sgi.IsGroup) return false;
      if (!CheckMemberValid(sgi.Data, ni.Data, false)) {  // avoid circularities
        if (!IsMemberInfos(sgi, ni)) {  // ignore duplicates
          ModelHelper.Trace(this, "A group node cannot be a member of itself: " + sgi.Data.ToString() + " and " + ni.Data.ToString());
        }
        return false;
      }
      ni.GroupNodeInfo = sgi;
      if (sgi.MemberNodeInfos == null) {
        sgi.MemberNodeInfos = new List<NodeInfo>() { ni };
      } else if (!sgi.MemberNodeInfos.Contains(ni)) {
        sgi.MemberNodeInfos.Add(ni);
      }
      return true;
    }

    private void RemoveGroupMemberInfos(NodeInfo sgi, NodeInfo ni) {
      if (sgi == null) return;
      if (ni == null) return;
      if (ni.GroupNodeInfo == sgi) {
        ni.GroupNodeInfo = null;
      }
      if (sgi.MemberNodeInfos != null) {
        sgi.MemberNodeInfos.Remove(ni);
      }
    }


    /// <summary>
    /// This is the <see cref="INotifyPropertyChanged"/> event handler for node data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">preferably a <see cref="ModelChangedEventArgs"/> that describes what changed and how</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <paramref name="e"/> argument is a <see cref="ModelChangedEventArgs"/>,
    /// this first calls <see cref="DiagramModel.OnChanged"/> in order to raise a <see cref="DiagramModel.Changed"/> event
    /// to notify this model's consumers.
    /// </para>
    /// <para>
    /// If the <see cref="PropertyChangedEventArgs.PropertyName"/> is the same as the
    /// <see cref="NodeKeyPath"/>, <see cref="GroupNodePath"/>, or <see cref="MemberNodesPath"/>,
    /// this automatically calls
    /// <see cref="DoNodeKeyChanged"/>, <see cref="DoGroupNodeChanged"/>, or <see cref="DoMemberNodeKeysChanged"/>,
    /// respectively.
    /// </para>
    /// </remarks>
    protected virtual void HandleNodePropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e == null) return;
      ModelChangedEventArgs m = e as ModelChangedEventArgs;
      if (m != null) {
        m.Model = this;
        OnChanged(m);
      }

      String pname = e.PropertyName;
      if (pname == null || pname.Length == 0) return;

      NodeType nodedata = (NodeType)sender;
      if (pname == this.GroupNodePath) {
        DoGroupNodeChanged(nodedata);
      } else if (pname == this.MemberNodesPath) {
        DoMemberNodeKeysChanged(nodedata);
      } else if (pname == this.NodeKeyPath) {
        DoNodeKeyChanged(nodedata);
      } else if (pname == this.NodeCategoryPath) {
        DoNodeCategoryChanged(nodedata);
      //} else if (pname == this.NodeIsGroupPath) {
      //  DoNodeIsGroupChanged(nodedata);
      //} else if (pname == this.NodeIsLinkLabelPath) {
      //  DoNodeIsLinkLabelChanged(nodedata);
      }
    }


    /// <summary>
    /// This should be called when a node data object is added to the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been augmented.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedNode"/>.
    /// </para>
    /// </remarks>
    public void DoNodeAdded(NodeType nodedata) {
      if (nodedata == null) return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni == null) {
        // it is a new object, but is its key unique?
        NodeKey key = FindKeyForNode(nodedata);
        bool uniquekey = true;
        if (key == null || (!this.NodeKeyIsNodeData && _IndexedNodes.ContainsKey(key))) {
          uniquekey = false;
          // not unique: give opportunity to make the node's key unique
          // or to reject this nodedata as a node
          if (MakeNodeKeyUnique(nodedata)) {
            key = FindKeyForNode(nodedata);
            // key now ought to be unique
          } else {
            return;
          }
        }
        // save in model
        ni = new NodeInfo();
        ni.Data = nodedata;
        _NodeInfos.Add(nodedata, ni);
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged += HandleNodePropertyChanged;

        ni.Key = key;
        // just in case MakeNodeKeyUnique failed to make the key unique but returned true:
        // allow duplicate keys in NodesSource, but not in _IndexedNodes
        if (key != null && !this.NodeKeyIsNodeData && (uniquekey || !_IndexedNodes.ContainsKey(key))) {
          _IndexedNodes.Add(key, ni);
        }

        // see if this node is a group node
        ni.Category = FindCategoryForNode(nodedata) ?? "";
        ni.IsGroup = FindIsGroupForNode(nodedata);

        // see if this node is a member of some group node
        if (this.GroupNodePath != "") {
          NodeKey groupkey = FindGroupKeyForNode(ni.Data);
          NodeType groupdata = FindNodeByKey(groupkey);
          if (groupdata != null) {
            NodeInfo groupni = FindNodeInfoForNode(groupdata);
            if (groupni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
              InsertNode(groupdata);
              groupni = FindNodeInfoForNode(groupdata);
            }
            if (groupni != null) {
              AddGroupMemberInfos(groupni, ni);
            } else {
              DelayGroup(ni, groupkey);
            }
          } else {  // maybe group node hasn't yet been added to model
            DelayGroup(ni, groupkey);
          }
        }

        // connect up with member nodes
        if (this.MemberNodesPath != "") {
          System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
          if (memberkeys != null) {
            foreach (NodeKey memberkey in memberkeys) {
              NodeType memberdata = FindNodeByKey(memberkey);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                if (memberni == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
                  InsertNode(memberdata);
                  memberni = FindNodeInfoForNode(memberdata);
                }
                if (memberni != null) {
                  AddGroupMemberInfos(ni, memberni);
                } else {
                  DelayMember(ni, memberkey);
                }
              } else {
                DelayMember(ni, memberkey);
              }
            }
            SetMemberKeyCollectionHandler(ni, memberkeys);
          }
        }

        // see if this node is a label for a link
        ni.IsLinkLabel = FindIsLinkLabelForNode(nodedata);

        // maybe fix up unresolved references to this node
        ResolveDelayedNodeInfo(ni);
        // notify
        if (!this.Initializing) {
          Object cols = null;
#if DATASET  // DataRowView
          System.Data.DataRowView view = nodedata as System.Data.DataRowView;
          if (view != null) cols = view.Row.ItemArray.Clone();
#endif
          RaiseModelChanged(ModelChange.AddedNode, nodedata, null, cols);
        }
      } else {
        // duplicate object according to hashcode, or already present -- ignore
      }
    }
    void IDiagramModel.DoNodeAdded(Object nodedata) { DoNodeAdded((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data object is removed from the <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="NodesSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="NodesSource"/> has been diminished.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedNode"/>.
    /// </para>
    /// </remarks>
    public void DoNodeRemoved(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {  // already known by model
        // save for use by GetConnectedNodesForNode et al.
        _LastRemovedNodeInfo = ni;

        // remove from model
        SetMemberKeyCollectionHandler(ni, null);
        _NodeInfos.Remove(ni.Data);
        INotifyPropertyChanged npc = ni.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleNodePropertyChanged;

        // in case of duplicate keys, only remove from key index when
        // this nodedata's NodeInfo is the one in the index
        NodeKey key = ni.Key;
        NodeInfo kni;
        if (key != null && !this.NodeKeyIsNodeData && _IndexedNodes.TryGetValue(key, out kni) && ni == kni) {
          _IndexedNodes.Remove(key);
        }
        // forget delayed references to this nodedata
        RemoveDelayedNodeInfo(ni);
        // remove any references
        RemoveNodeInfos(ni);
        SetLabelNodeToLinkInfo(ni.LabeledLinkInfo, null);
        // notify for side-effects
        RaiseModelChanged(ModelChange.RemovedNode, nodedata, null, null);

        _LastRemovedNodeInfo = null;
      } else {
        // not present, or already gone -- OK
      }
    }
    void IDiagramModel.DoNodeRemoved(Object nodedata) { DoNodeRemoved((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data's key value may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="NodeKeyPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindKeyForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoNodeKeyChanged(NodeType nodedata) {
      if (this.NodeKeyIsNodeData) return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeKey oldkey = ni.Key;  // saved key
        NodeKey newkey = FindKeyForNode(nodedata);  // current key
        if (oldkey == null || !oldkey.Equals(newkey)) {  // if key has actually changed
          if (oldkey != null) {
            _IndexedNodes.Remove(oldkey);
          }
          ni.Key = newkey;
          if (newkey != null) {
            NodeInfo existing;
            if (_IndexedNodes.TryGetValue(newkey, out existing)) {
              ModelHelper.Trace(this, "DoNodeKeyChanged detected duplicate node data key value: " + newkey.ToString());
            } else {  // only save in index if nothing already present with that key
              _IndexedNodes.Add(newkey, ni);
            }
          }
          // Notify
          RaiseModelChanged(ModelChange.ChangedNodeKey, nodedata, oldkey, newkey);
        }
      }
    }
    void IDiagramModel.DoNodeKeyChanged(Object nodedata) { DoNodeKeyChanged((NodeType)nodedata); }


    /// <summary>
    /// This should be called when a node data's category value may have changed.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="NodeCategoryPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindCategoryForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedNodeCategory"/>.
    /// </para>
    /// </remarks>
    public void DoNodeCategoryChanged(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        String oldcategory = ni.Category;
        String newcategory = FindCategoryForNode(nodedata);
        if (oldcategory != newcategory) {
          ni.Category = newcategory;
          // Notify
          RaiseModelChanged(ModelChange.ChangedNodeCategory, nodedata, oldcategory, newcategory);
        }
      }
    }

    /// <summary>
    /// This should be called when a node data's membership in a group may have changed.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="GroupNodePath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindGroupKeyForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedGroupNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoGroupNodeChanged(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && this.GroupNodePath != "") {
        NodeInfo oldsgi = ni.GroupNodeInfo;
        NodeKey newkey = FindGroupKeyForNode(nodedata);
        NodeType newsg = FindNodeByKey(newkey);
        NodeInfo newsgi = FindNodeInfoForNode(newsg);
        if (oldsgi != newsgi) {
          RemoveGroupMemberInfos(oldsgi, ni);
          if (!AddGroupMemberInfos(newsgi, ni)) newsgi = null;  // avoid circularities
          if (oldsgi != newsgi) {
            RaiseModelChanged(ModelChange.ChangedGroupNodeKey, nodedata, (oldsgi != null ? oldsgi.Key : default(NodeKey)), (newsgi != null ? newsgi.Key : default(NodeKey)));
          }
          // update group membership for all connected links
          ReparentAllConnectedLinks(ni);
        }
      }
    }
    void ISubGraphModel.DoGroupNodeChanged(Object nodedata) { DoGroupNodeChanged((NodeType)nodedata); }

    private void ReparentAllConnectedLinks(NodeInfo ni) {
      if (ni.ConnectedLinkInfos != null) {
        foreach (LinkInfo li in ni.ConnectedLinkInfos) ReparentLinkToLowestCommonGroup(li);
      }
      if (ni.IsGroup && ni.MemberNodeInfos != null) {
        foreach (NodeInfo mi in ni.MemberNodeInfos) {
          ReparentAllConnectedLinks(mi);
        }
      }
    }

    /// <summary>
    /// This should be called when a "member" node data key has been added to the collection of "member" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="memberkey">the key for the added "member" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "member" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedMemberNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeyAdded(NodeType nodedata, NodeKey memberkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(memberkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        AddGroupMemberInfos(ni, toni);
        if (ni.SavedMemberKeys != null && !ni.SavedMemberKeys.Contains(memberkey)) ni.SavedMemberKeys.Add(memberkey);
        // Notify
        RaiseModelChanged(ModelChange.AddedMemberNodeKey, nodedata, default(NodeKey), memberkey);
      }
    }
    void IGroupsModel.DoMemberNodeKeyAdded(Object nodedata, Object memberkey) { DoMemberNodeKeyAdded((NodeType)nodedata, (NodeKey)memberkey); }

    /// <summary>
    /// This should be called when a "member" node data key has been removed from the collection of "member" node keys.
    /// </summary>
    /// <param name="nodedata">the modified node data</param>
    /// <param name="memberkey">the key for the removed "member" node data</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the list of "member" keys implements <see cref="INotifyCollectionChanged"/>
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedMemberNodeKey"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeyRemoved(NodeType nodedata, NodeKey memberkey) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        NodeType tonode = FindNodeByKey(memberkey);
        NodeInfo toni = FindNodeInfoForNode(tonode);
        RemoveGroupMemberInfos(ni, toni);
        if (ni.SavedMemberKeys != null) ni.SavedMemberKeys.Remove(memberkey);
        // Notify
        RaiseModelChanged(ModelChange.RemovedMemberNodeKey, nodedata, memberkey, default(NodeKey));
      }
    }
    void IGroupsModel.DoMemberNodeKeyRemoved(Object nodedata, Object memberkey) { DoMemberNodeKeyRemoved((NodeType)nodedata, (NodeKey)memberkey); }

    /// <summary>
    /// This should be called when a node data's list of "member" nodes has been replaced.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a node data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="MemberNodesPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindMemberNodeKeysForNode"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedMemberNodeKeys"/>.
    /// </para>
    /// </remarks>
    public void DoMemberNodeKeysChanged(NodeType nodedata) {
      if (this.MemberNodesPath == "") return;
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) {
        System.Collections.IEnumerable oldmemberkeys = ni.MemberKeyCollection;
        IEnumerable<NodeKey> oldmemberkeys2 = ni.SavedMemberKeys;
        System.Collections.IEnumerable newmemberkeys = FindMemberNodeKeysForNode(nodedata);
        IEnumerable<NodeKey> newmemberkeys2 = (newmemberkeys != null ? newmemberkeys.Cast<NodeKey>() : null);
        if (!EqualSequences<NodeKey>(oldmemberkeys2, newmemberkeys2)) {
          if (oldmemberkeys2 != null) {
            foreach (NodeKey removed in (newmemberkeys2 != null ? oldmemberkeys2.Except(newmemberkeys2) : oldmemberkeys2)) {
              NodeType memberdata = FindNodeByKey(removed);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                RemoveGroupMemberInfos(ni, memberni);
              }
            }
          }
          SetMemberKeyCollectionHandler(ni, newmemberkeys);
          if (newmemberkeys2 != null) {
            foreach (NodeKey added in (oldmemberkeys2 != null ? newmemberkeys2.Except(oldmemberkeys2) : newmemberkeys2)) {
              NodeType memberdata = FindNodeByKey(added);
              if (memberdata != null) {
                NodeInfo memberni = FindNodeInfoForNode(memberdata);
                AddGroupMemberInfos(ni, memberni);
              }
            }
          }
          RaiseModelChanged(ModelChange.ChangedMemberNodeKeys, nodedata, oldmemberkeys, newmemberkeys);
        }
      }
    }
    void IGroupsModel.DoMemberNodeKeysChanged(Object nodedata) { DoMemberNodeKeysChanged((NodeType)nodedata); }


    // Links data

    /// <summary>
    /// Gets or sets the collection of link data items for the model.
    /// </summary>
    /// <value>
    /// Initially this value is an empty <c>ObservableCollection</c>.
    /// It cannot be set to a null value.
    /// For generality, this is of type <see cref="System.Collections.IEnumerable"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedLinksSource"/>.
    /// </para>
    /// </remarks>
    public System.Collections.IEnumerable LinksSource {
      get { return _LinksSource; }
      set {
        System.Collections.IEnumerable old = _LinksSource;
        if (value == null) ModelHelper.Error("Cannot set LinksSource to null");
        if (old != value && value != null) {
          INotifyCollectionChanged oldsource = _LinksSource as INotifyCollectionChanged;
          if (oldsource != null) {
            oldsource.CollectionChanged -= LinksSource_CollectionChanged;
#if DATASET  // IBindingList
          } else {
            IBindingList oldlist = _LinksSource as IBindingList;
            if (oldlist != null) oldlist.ListChanged -= LinksSource_ListChanged;
#endif
          }
          _LinksSource = value;
          INotifyCollectionChanged newsource = _LinksSource as INotifyCollectionChanged;
          if (newsource != null) {
            newsource.CollectionChanged += LinksSource_CollectionChanged;
            this.LinksSourceNotifies = true;
#if DATASET  // IBindingList
          } else {
            IBindingList newlist = _LinksSource as IBindingList;
            if (newlist != null) {
              newlist.ListChanged += LinksSource_ListChanged;
              this.LinksSourceNotifies = true;
            } else {
              this.LinksSourceNotifies = false;
            }
#else
          } else {
            this.LinksSourceNotifies = false;
#endif
          }
          ResetLinks(false);
          if (this.NodeKeyIsNodeData) {
            // may seen some new nodes that would have been unresolved references,
            // but were added to NodesSource instead
            foreach (NodeType nodedata in this.NodesSource) {
              RaiseModelChanged(ModelChange.AddedNode, nodedata, null, null);
            }
          }
          RaiseModelChanged(ModelChange.ChangedLinksSource, null, old, value);
        }
      }
    }

    internal bool LinksSourceNotifies { get; set; }

    private void LinksSource_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add:
          foreach (LinkType linkdata in e.NewItems) DoLinkAdded(linkdata);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (LinkType linkdata in e.OldItems) DoLinkRemoved(linkdata);
          break;
        case NotifyCollectionChangedAction.Replace:
          foreach (LinkType linkdata in e.OldItems) DoLinkRemoved(linkdata);
          foreach (LinkType linkdata in e.NewItems) DoLinkAdded(linkdata);
          break;
        case NotifyCollectionChangedAction.Reset: {
            // don't remove and re-add an existing linkdata
            // use HashSet for faster lookup of new/desired
            var newlinks = new HashSet<LinkType>();
            foreach (LinkType linkdata in _LinksSource) newlinks.Add(linkdata);

            var oldlinks = _LinkInfos.Keys.ToArray();
            foreach (LinkType linkdata in oldlinks) {
              // only remove old linkdata if not present in new _LinksSource
              if (!newlinks.Contains(linkdata)) DoLinkRemoved(linkdata);
            }

            // add all linkdata in _LinksSource; ignore if already present in old _LinkInfos
            foreach (LinkType linkdata in _LinksSource) DoLinkAdded(linkdata);
            break;
          }
        default:
          break;
      }
    }

#if DATASET  // IBindingList
    private void LinksSource_ListChanged(Object sender, ListChangedEventArgs e) {
      IBindingList list = (IBindingList)sender;
      switch (e.ListChangedType) {
        case ListChangedType.ItemAdded:
          if (list != null) {
            LinkType data = (LinkType)list[e.NewIndex];
            //DoLinkAdded(data);  // shouldn't call DoLinkAdded with the empty row
            _BindingListLinks.Insert(e.NewIndex, data);
          }
          break;
        case ListChangedType.ItemDeleted:
          if (list != null) {
            LinkType data = _BindingListLinks[e.NewIndex];
            DoLinkRemoved(data);
            _BindingListLinks.RemoveAt(e.NewIndex);
          }
          break;
        case ListChangedType.ItemChanged:
          if (list != null) {
            LinkType data1 = _BindingListLinks[e.NewIndex];
            LinkType data2 = (LinkType)list[e.NewIndex];
            if (!data1.Equals(data2)) {
              DoLinkRemoved(data1);
              DoLinkAdded(data2);
              _BindingListLinks[e.NewIndex] = data2;
            } else {
              PropertyDescriptor desc = e.PropertyDescriptor;
              Object val = desc.GetValue(data1);
              //??? get Original DataRowVersion column value
              String pname = desc.Name;
              ModelChangedEventArgs args = new ModelChangedEventArgs(pname, data1, val, val);
              if (this.UndoManager != null) {
                args.Model = this;
                ModelChangedEventArgs prev = this.UndoManager.FindPreviousChange(args);
                if (prev != null) {
                  args.OldValue = prev.NewValue;
                } //???
              }
              HandleNodePropertyChanged(data1, args);
            }
          }
          break;
        case ListChangedType.Reset: {
            IEnumerable<LinkType> oldlinks = _LinkInfos.Keys.ToArray();
            foreach (LinkType linkdata in oldlinks) DoLinkRemoved(linkdata);
            _BindingListLinks.Clear();
            foreach (LinkType linkdata in _LinksSource) {
              DoLinkAdded(linkdata);
              _BindingListLinks.Insert(e.NewIndex, linkdata);
            }
            break;
          }
        default:
          break;
      }
    }
#endif

    // Model discovery:
    // Property paths for declarative graph navigation

    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for the "from" node data of a link data object.
    /// </summary>
    /// <value>
    /// This value is used by <see cref="FindFromNodeKeyForLink"/> to get a reference
    /// to the node data from which this link comes.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindFromNodeKeyForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String LinkFromPath {
      get { return _LinkFromPathPPI.Path; }
      set {
        String old = _LinkFromPathPPI.Path;
        if (old != value && value != null) {
          _LinkFromPathPPI.Path = value;
          ResetLinks(true);
          RaiseModelChanged(ModelChange.ChangedLinkFromPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the "from" node key for a given link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>
    /// a <typeparamref name="NodeKey"/> for the connected node
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkFromPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindFromNodeKeyForLink(LinkType linkdata) {
      if (this.Delegates.FindFromNodeKeyForLink != null) {
        return this.Delegates.FindFromNodeKeyForLink(linkdata);
      }
      return _LinkFromPathPPI.EvalFor(linkdata);  // value of property path on linkdata
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get optional "port" parameter information
    /// for the "from" node data of a link data object.
    /// </summary>
    /// <value>
    /// This value is used by <see cref="FindFromParameterForLink"/> to get an object
    /// describing the "from" end of this link data.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindFromParameterForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be of type <typeparamref name="PortKey"/>.
    /// </para>
    /// </remarks>
    public String LinkFromParameterPath {
      get { return _LinkFromParameterPathPPI.Path; }
      set {
        String old = _LinkFromParameterPathPPI.Path;
        if (old != value) {
          _LinkFromParameterPathPPI.Path = value;
          ResetLinks(true);
          RaiseModelChanged(ModelChange.ChangedLinkFromParameterPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find additional information about the "from" node connection for a given link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>
    /// a <typeparamref name="PortKey"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkFromParameterPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual PortKey FindFromParameterForLink(LinkType linkdata) {
      if (this.Delegates.FindFromParameterForLink != null) {
        return this.Delegates.FindFromParameterForLink(linkdata);
      }
      return _LinkFromParameterPathPPI.EvalFor(linkdata);  // optional "port" information for link's "from" node
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for the "to" node data of a link data object.
    /// </summary>
    /// <value>
    /// This value is used by <see cref="FindToNodeKeyForLink"/> to get a reference
    /// to the node data to which this link goes.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindToNodeKeyForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String LinkToPath {
      get { return _LinkToPathPPI.Path; }
      set {
        String old = _LinkToPathPPI.Path;
        if (old != value && value != null) {
          _LinkToPathPPI.Path = value;
          ResetLinks(true);
          RaiseModelChanged(ModelChange.ChangedLinkToPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the "to" node key for a given link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>
    /// a <typeparamref name="NodeKey"/> for the connected node
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkToPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindToNodeKeyForLink(LinkType linkdata) {
      if (this.Delegates.FindToNodeKeyForLink != null) {
        return this.Delegates.FindToNodeKeyForLink(linkdata);
      }
      return _LinkToPathPPI.EvalFor(linkdata);  // value of property path on linkdata
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get optional "port" parameter information
    /// for the "to" node data of a link data object.
    /// </summary>
    /// <value>
    /// This value is used by <see cref="FindToParameterForLink"/> to get an object
    /// describing the "to" end of this link data.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindToParameterForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be of type <typeparamref name="PortKey"/>.
    /// </para>
    /// </remarks>
    public String LinkToParameterPath {
      get { return _LinkToParameterPathPPI.Path; }
      set {
        String old = _LinkToParameterPathPPI.Path;
        if (old != value) {
          _LinkToParameterPathPPI.Path = value;
          ResetLinks(true);
          RaiseModelChanged(ModelChange.ChangedLinkToParameterPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find additional information about the "to" node connection for a given link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>
    /// a <typeparamref name="PortKey"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkToParameterPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual PortKey FindToParameterForLink(LinkType linkdata) {
      if (this.Delegates.FindToParameterForLink != null) {
        return this.Delegates.FindToParameterForLink(linkdata);
      }
      return _LinkToParameterPathPPI.EvalFor(linkdata);  // optional "port" information for link's "to" node
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to get the key for the "label" node data of a link data object.
    /// </summary>
    /// <value>
    /// This value is used by <see cref="FindLabelNodeKeyForLink"/> to get a reference
    /// to the node data associated with this link.
    /// The default value, an empty string, means there is no label node for any link --
    /// <see cref="FindLabelNodeKeyForLink"/> will not be called.
    /// A null value may be used to indicate that there is no property path but that
    /// <see cref="FindLabelNodeKeyForLink"/> should still be called because it has been overridden.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindLabelNodeKeyForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be of type <typeparamref name="NodeKey"/>.
    /// </para>
    /// </remarks>
    public String LinkLabelNodePath {
      get { return _LinkLabelNodePathPPI.Path; }
      set {
        String old = _LinkLabelNodePathPPI.Path;
        if (old != value) {
          _LinkLabelNodePathPPI.Path = value;
          ResetNodes(true);
          RaiseModelChanged(ModelChange.ChangedLinkLabelNodePath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the "label" node key for a given link data, if any.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>
    /// a <typeparamref name="NodeKey"/> for the label node data
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkLabelNodePath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual NodeKey FindLabelNodeKeyForLink(LinkType linkdata) {
      if (this.Delegates.FindLabelNodeKeyForLink != null) {
        return this.Delegates.FindLabelNodeKeyForLink(linkdata);
      }
      return _LinkLabelNodePathPPI.EvalFor(linkdata);
    }


    /// <summary>
    /// Gets or sets a property path that that specifies how to find the category of a link data.
    /// </summary>
    /// <value>
    /// This defaults to an empty string, which causes <see cref="FindCategoryForLink"/> to return an empty string.
    /// The value must not be null.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used for model discovery.  This is a declarative way to
    /// define the behavior of <see cref="FindCategoryForLink"/> for most cases.
    /// </para>
    /// <para>
    /// The value of this property path applied to a link data object must be a string.
    /// </para>
    /// </remarks>
    public String LinkCategoryPath {
      get { return _LinkCategoryPathPPI.Path; }
      set {
        String old = _LinkCategoryPathPPI.Path;
        if (old != value && value != null) {
          _LinkCategoryPathPPI.Path = value;
          ResetLinks(true);
          RaiseModelChanged(ModelChange.ChangedLinkCategoryPath, null, old, value);
        }
      }
    }

    /// <summary>
    /// Find the category of a link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>by default an empty string</returns>
    /// <remarks>
    /// <para>
    /// This is called on each link data that is added to the model.
    /// </para>
    /// <para>
    /// This is used for model discovery.
    /// This method can be overridden in case the <see cref="LinkCategoryPath"/>
    /// property path is not flexible enough or fast enough to determine the value.
    /// </para>
    /// </remarks>
    protected virtual String FindCategoryForLink(LinkType linkdata) {
      if (this.Delegates.FindCategoryForLink != null) {
        return this.Delegates.FindCategoryForLink(linkdata);
      }
      if (linkdata == null) return "";
      if (this.LinkCategoryPath.Length == 0) return "";
      return _LinkCategoryPathPPI.EvalFor(linkdata);
    }


    /// <summary>
    /// This predicate compares two <typeparamref name="PortKey"/> values
    /// and returns true if they are "equal".
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>
    /// Normally this uses the default <c>EqualityComparer</c> for the <typeparamref name="PortKey"/> type.
    /// But you may need to override this method if that kind of equality comparison is inappropriate
    /// for your port information.
    /// </returns>
    protected virtual bool IsEqualPortParameters(PortKey a, PortKey b) {
      if (this.Delegates.IsEqualPortParameters != null) {
        return this.Delegates.IsEqualPortParameters(a, b);
      }
      if (a == null) {
        return (b == null);
      } else {
        return EqualityComparer<PortKey>.Default.Equals(a, b);
      }
    }


    // Keeping track of links

#if DEBUG  // public internals

    public void Walk(LinkInfo li) {
      ModelHelper.Trace(" LinkInfo Category: " + li.Category);
      ModelHelper.Trace("  Data: " + li.Data.ToString());
      if (li.FromNodeInfo != null) {
        ModelHelper.Trace("  FromNodeInfo: " + li.FromNodeInfo.ToString());
      }
      if (li.FromParam != null) {
        ModelHelper.Trace("  FromParam: " + li.FromParam.ToString());
      }
      if (li.ToNodeInfo != null) {
        ModelHelper.Trace("  ToNodeInfo: " + li.ToNodeInfo.ToString());
      }
      if (li.ToParam != null) {
        ModelHelper.Trace("  ToParam: " + li.ToParam.ToString());
      }
      if (li.GroupNodeInfo != null) {
        ModelHelper.Trace("  GroupNodeInfo: " + li.GroupNodeInfo.ToString());
      }
      if (li.LabelNodeInfo != null) {
        ModelHelper.Trace("  LabelNodeInfo: " + li.LabelNodeInfo.ToString());
      }
    }

    public
#else
    private
#endif
    sealed class LinkInfo {
      public LinkType Data { get; set; }
      public NodeInfo FromNodeInfo { get; set; }
      public PortKey FromParam { get; set; }
      public NodeInfo ToNodeInfo { get; set; }
      public PortKey ToParam { get; set; }
      public NodeInfo GroupNodeInfo { get; set; }  // to containing group
      public NodeInfo LabelNodeInfo { get; set; }  // to a node that is a link label
      public String Category { get; set; }
      public override String ToString() {
        return "LI:" + ((this.Data != null) ? this.Data.ToString() : "(no Data)");
      }
    }

    private LinkInfo FindLinkInfoForLink(LinkType linkdata) {
      if (linkdata == null) return null;  // don't compare with default(LinkType), e.g. to allow finding zero
      LinkInfo li;
      _LinkInfos.TryGetValue(linkdata, out li);
      if (li == null) li = _LastRemovedLinkInfo;
      return li;
    }

    private void ResetLinks(bool clear) {
      foreach (var pair in _NodeInfos) {
        pair.Value.ConnectedLinkInfos = null;
      }
      _DelayedLinkInfos.Clear();
      foreach (var pair in _LinkInfos) {
        LinkInfo li = pair.Value;
        INotifyPropertyChanged npc = li.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleLinkPropertyChanged;
      }
      _LinkInfos.Clear();
      _BindingListLinks.Clear();  // just unresolved link references to nodes
      bool oldInit = this.Initializing;
      try {
        this.Initializing = true;
#if DATASET  // IBindingList
        IBindingList list = this.LinksSource as IBindingList;
        if (list != null) {
          foreach (LinkType l in list) {
            _BindingListLinks.Add(l);
          }
        }
#endif
        foreach (LinkType linkdata in this.LinksSource) DoLinkAdded(linkdata);
      } finally {
        if (clear) ClearUnresolvedReferences();
        this.Initializing = oldInit;
      }
    }

    // called when a link is added, but either or both of its node references were unresolved
    private void DelayLinkInfo(LinkInfo li, NodeKey nk) {
      if (nk == null) return;
      List<LinkInfo> list;
      if (_DelayedLinkInfos.TryGetValue(nk, out list)) {
        if (!list.Contains(li)) list.Add(li);
      } else {
        _DelayedLinkInfos.Add(nk, new List<LinkInfo>() { li });
      }
    }

    // called when a link is removed
    private void RemoveDelayedLinkInfo(LinkInfo li) {
#if DATASET  // DataRowView
      if (li.Data is System.Data.DataRowView) return;
#endif
      List<LinkInfo> list;
      if (li.FromNodeInfo == null) {
        NodeKey fromkey = FindFromNodeKeyForLink(li.Data);
        if (fromkey != null && _DelayedLinkInfos.TryGetValue(fromkey, out list)) {
          list.Remove(li);
          if (list.Count == 0) _DelayedLinkInfos.Remove(fromkey);
        }
      }
      if (li.ToNodeInfo == null) {
        NodeKey tokey = FindToNodeKeyForLink(li.Data);
        if (tokey != null && _DelayedLinkInfos.TryGetValue(tokey, out list)) {
          list.Remove(li);
          if (list.Count == 0) _DelayedLinkInfos.Remove(tokey);
        }
      }
      if (li.LabelNodeInfo == null && this.LinkLabelNodePath != "") {
        NodeKey labkey = FindLabelNodeKeyForLink(li.Data);
        if (labkey != null && _DelayedLinkInfos.TryGetValue(labkey, out list)) {
          list.Remove(li);
          if (list.Count == 0) _DelayedLinkInfos.Remove(labkey);
        }
      }
    }

    private bool AddLinkInfoToNodeInfo(NodeInfo ni, LinkInfo li, bool from) {
      if (ni == null) return false;
      if (li == null) return false;
      if (from) {
        NodeInfo fni = ni;
        NodeInfo tni = li.ToNodeInfo;
        if (tni != null && !CheckLinkValid(fni.Data, li.FromParam, tni.Data, li.ToParam, false, default(LinkType))) {  // avoid circularities
          ModelHelper.Trace(this, "A node cannot be a child or descendant of itself: " + fni.Data.ToString() + " and " + tni.Data.ToString());
          return false;
        }
        li.FromNodeInfo = fni;
      } else {
        NodeInfo fni = li.FromNodeInfo;
        NodeInfo tni = ni;
        if (fni != null && !CheckLinkValid(fni.Data, li.FromParam, tni.Data, li.ToParam, false, default(LinkType))) {  // avoid circularities
          ModelHelper.Trace(this, "A node cannot be a child or descendant of itself: " + fni.Data.ToString() + " and " + tni.Data.ToString());
          return false;
        }
        li.ToNodeInfo = tni;
      }
      if (ni.ConnectedLinkInfos == null) {
        ni.ConnectedLinkInfos = new List<LinkInfo>() { li };
      } else if (!ni.ConnectedLinkInfos.Contains(li)) {
        ni.ConnectedLinkInfos.Add(li);
      }
      return true;
    }

    private void RemoveLinkInfoFromNodeInfo(NodeInfo ni, LinkInfo li, bool from) {
      if (ni == null) return;
      if (li == null) return;
      bool selfloop = li.FromNodeInfo == li.ToNodeInfo;
      if (from) {
        li.FromNodeInfo = null;
      } else {
        li.ToNodeInfo = null;
      }
      if (!selfloop && ni.ConnectedLinkInfos != null) {
        ni.ConnectedLinkInfos.Remove(li);
      }
    }

    private void AddMemberLinkToNodeInfo(NodeInfo sgi, LinkInfo li) {
      if (sgi == null) return;
      if (li == null) return;
      li.GroupNodeInfo = sgi;
      if (sgi.MemberLinkInfos == null) {
        sgi.MemberLinkInfos = new List<LinkInfo>() { li };
      } else if (!sgi.MemberLinkInfos.Contains(li)) {
        sgi.MemberLinkInfos.Add(li);
      }
    }

    private void RemoveMemberLinkFromNodeInfo(NodeInfo sgi, LinkInfo li) {
      if (sgi == null) return;
      if (li == null) return;
      if (li.GroupNodeInfo == sgi) {
        li.GroupNodeInfo = null;
      }
      if (sgi.MemberLinkInfos != null) {
        sgi.MemberLinkInfos.Remove(li);
      }
    }

    private void ReparentLinkToLowestCommonGroup(LinkInfo li) {
      if (li == null) return;
      NodeInfo oldsg = li.GroupNodeInfo;
      NodeInfo newsg = FindCommonGroupInfo(li.FromNodeInfo, li.ToNodeInfo);
      if (oldsg != newsg && (newsg == null || newsg.IsGroup)) {
        if (oldsg != null) {
          RemoveMemberLinkFromNodeInfo(oldsg, li);
        }
        if (newsg != null) {
          AddMemberLinkToNodeInfo(newsg, li);
        }
        RaiseModelChanged(ModelChange.ChangedLinkGroupNodeKey, li.Data, (oldsg != null ? oldsg.Key : default(NodeKey)), (newsg != null ? newsg.Key: default(NodeKey)));
      }
    }

    // result might be null or a nodeinfo that IsGroup
    private NodeInfo FindCommonGroupInfo(NodeInfo a, NodeInfo b) {
      if (a == null) return null;
      if (a.GroupNodeInfo == b) return b;  // handle common case quickly
      if (b == null) return null;
      if (b.GroupNodeInfo == a) return a;  // handle common case quickly
      // if either node is a link label, use the link's group (if any)
      if (a.LabeledLinkInfo != null) return FindCommonGroupInfo(a.LabeledLinkInfo.GroupNodeInfo, b);
      if (b.LabeledLinkInfo != null) return FindCommonGroupInfo(a, b.LabeledLinkInfo.GroupNodeInfo);
      if (a.GroupNodeInfo == b.GroupNodeInfo) return a.GroupNodeInfo;  // should be common too
      if (b.GroupNodeInfo == null) {  // B can't be in A, so is A member of B?
        NodeInfo p = a;
        while (p != null) {
          if (p == b) return b;
          p = p.GroupNodeInfo;
        }
      } else if (a.GroupNodeInfo == null) {  // A can't be in B, so is B member of A?
        NodeInfo q = b;
        while (q != null) {
          if (q == a) return a;
          q = q.GroupNodeInfo;
        }
      } else {  // general case: A might be in B, or B might be in A
        NodeInfo p = a;
        while (p != null) {
          NodeInfo q = b;
          while (q != null) {
            if (q == p)
              return q;
            q = q.GroupNodeInfo;
          }
          p = p.GroupNodeInfo;
        }
      }
      return null;
    }

    private void SetLabelNodeToLinkInfo(LinkInfo li, NodeInfo ni) {
      if (li != null && li.LabelNodeInfo != ni) {
        NodeInfo oldmi = li.LabelNodeInfo;
        if (oldmi != null) oldmi.LabeledLinkInfo = null;
        li.LabelNodeInfo = ni;
      }
      if (ni != null && ni.LabeledLinkInfo != li) {
        LinkInfo oldli = ni.LabeledLinkInfo;
        if (oldli != null) oldli.LabelNodeInfo = null;
        ni.LabeledLinkInfo = li;
      }
    }


    /// <summary>
    /// This is the <see cref="INotifyPropertyChanged"/> event handler for link data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">preferably a <see cref="ModelChangedEventArgs"/> that describes what changed and how</param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <paramref name="e"/> argument is a <see cref="ModelChangedEventArgs"/>,
    /// this first calls <see cref="DiagramModel.OnChanged"/> in order to raise a <see cref="DiagramModel.Changed"/> event
    /// to notify this model's consumers.
    /// </para>
    /// <para>
    /// If the <see cref="PropertyChangedEventArgs.PropertyName"/> is the same as the
    /// <see cref="LinkFromPath"/>, <see cref="LinkToPath"/>,
    /// <see cref="LinkFromParameterPath"/>, <see cref="LinkToParameterPath"/>,
    /// this automatically calls <see cref="DoLinkPortsChanged"/>.
    /// If the property name is <see cref="LinkLabelNodePath"/>,
    /// this automatically calls <see cref="DoLinkLabelChanged"/>.
    /// </para>
    /// </remarks>
    protected virtual void HandleLinkPropertyChanged(Object sender, PropertyChangedEventArgs e) {
      if (e == null) return;
      ModelChangedEventArgs m = e as ModelChangedEventArgs;
      if (m != null) {
        m.Model = this;
        OnChanged(m);
      }

      String pname = e.PropertyName;
      if (pname == null || pname.Length == 0) return;

      LinkType linkdata = (LinkType)sender;
      if (pname == this.LinkFromPath) {
        DoLinkPortsChanged(linkdata);
      } else if (pname == this.LinkToPath) {
        DoLinkPortsChanged(linkdata);
      } else if (pname == this.LinkFromParameterPath) {
        DoLinkPortsChanged(linkdata);
      } else if (pname == this.LinkToParameterPath) {
        DoLinkPortsChanged(linkdata);
      } else if (pname == this.LinkLabelNodePath) {
        DoLinkLabelChanged(linkdata);
      } else if (pname == this.LinkCategoryPath) {
        DoLinkCategoryChanged(linkdata);
      }
    }


    /// <summary>
    /// This should be called when a link data object is added to the <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="LinksSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="LinksSource"/> has been augmented.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.AddedLink"/>.
    /// </para>
    /// </remarks>
    public void DoLinkAdded(LinkType linkdata) {
      if (linkdata == null) return;
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li == null) {  // not already known by model
        NodeKey fromkey = FindFromNodeKeyForLink(linkdata);  // look for node data for "from" node
        PortKey fromparam = FindFromParameterForLink(linkdata);  // optional "port" information for link's "from" node
        NodeKey tokey = FindToNodeKeyForLink(linkdata);  // look for node data for "to" node
        PortKey toparam = FindToParameterForLink(linkdata);  // optional "port" information for link's "to" node

        // remember From data and To data for this linkdata
        li = new LinkInfo();
        li.Data = linkdata;
        li.FromParam = fromparam;
        li.ToParam = toparam;
        _LinkInfos.Add(linkdata, li);
        INotifyPropertyChanged npc = li.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged += HandleLinkPropertyChanged;

        // update the corresponding NodeInfo.ConnectedLinkInfos collections
        NodeType fromdata = FindNodeByKey(fromkey);
        if (fromdata != null) {  // don't compare with default(NodeType), e.g. to allow finding zero
          NodeInfo fn = FindNodeInfoForNode(fromdata);
          if (fn == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
            InsertNode(fromdata);
            fn = FindNodeInfoForNode(fromdata);
          }
          if (fn != null) {
            AddLinkInfoToNodeInfo(fn, li, true);
          } else {
            DelayLinkInfo(li, fromkey);
          }
        } else {  // maybe node hasn't yet been added to model
          DelayLinkInfo(li, fromkey);
        }

        NodeType todata = FindNodeByKey(tokey);
        if (todata != null) {  // don't compare with default(NodeType), e.g. to allow finding zero
          NodeInfo tn = FindNodeInfoForNode(todata);
          if (tn == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
            InsertNode(todata);
            tn = FindNodeInfoForNode(todata);
          }
          if (tn != null) {
            AddLinkInfoToNodeInfo(tn, li, false);
          } else {
            DelayLinkInfo(li, tokey);
          }
        } else {  // maybe node hasn't yet been added to model
          DelayLinkInfo(li, tokey);
        }

        // don't depend on link data telling us which group should be the container for a link:
        // just add the link to the lowest common group
        ReparentLinkToLowestCommonGroup(li);

        // see if this link has a node acting as a "label"
        if (this.LinkLabelNodePath != "") {
          NodeKey labkey = FindLabelNodeKeyForLink(linkdata);
          NodeType labdata = FindNodeByKey(labkey);
          if (labdata != null) {
            NodeInfo mi = FindNodeInfoForNode(labdata);
            if (mi == null && this.NodeKeyIsNodeData && this.NodeKeyReferenceAutoInserts) {
              InsertNode(labdata);
              mi = FindNodeInfoForNode(labdata);
            }
            if (mi != null) {
              SetLabelNodeToLinkInfo(li, mi);
            } else {
              DelayLinkInfo(li, labkey);
            }
          } else {  // maybe node hasn't yet been added to model
            DelayLinkInfo(li, labkey);
          }
        }

        li.Category = FindCategoryForLink(linkdata) ?? "";

        // notify
        if (!this.Initializing) {
          Object cols = null;
#if DATASET  // DataRowView
          System.Data.DataRowView view = linkdata as System.Data.DataRowView;
          if (view != null) cols = view.Row.ItemArray.Clone();
#endif
          RaiseModelChanged(ModelChange.AddedLink, linkdata, null, cols);
        }
      } else {
        // duplicate object according to hashcode, or already present -- ignore
      }
    }
    void ILinksModel.DoLinkAdded(Object linkdata) { DoLinkAdded((LinkType)linkdata); }


    /// <summary>
    /// This should be called when a link data object is removed from the <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If the <see cref="LinksSource"/> collection implements <see cref="INotifyCollectionChanged"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the <see cref="LinksSource"/> has been diminished.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.RemovedLink"/>.
    /// </para>
    /// </remarks>
    public void DoLinkRemoved(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null) {
        // save for use by GetConnectedNodesForNode et al.
        _LastRemovedLinkInfo = li;

        // forget this linkdata
        RemoveDelayedLinkInfo(li);
        // remove from model
        _LinkInfos.Remove(linkdata);
        INotifyPropertyChanged npc = li.Data as INotifyPropertyChanged;
        if (npc != null) npc.PropertyChanged -= HandleLinkPropertyChanged;

        // remove relationship with group, if any
        if (li.GroupNodeInfo != null) {
          RaiseModelChanged(ModelChange.ChangedLinkGroupNodeKey, li.Data, li.GroupNodeInfo.Key, default(NodeKey));
          RemoveMemberLinkFromNodeInfo(li.GroupNodeInfo, li);
        }
        // remove this linkdata from the NodeInfos for the From and To data
        RemoveLinkInfoFromNodeInfo(li.FromNodeInfo, li, true);
        RemoveLinkInfoFromNodeInfo(li.ToNodeInfo, li, false);
        SetLabelNodeToLinkInfo(null, li.LabelNodeInfo);
        // notify for side-effects
        RaiseModelChanged(ModelChange.RemovedLink, linkdata, null, null);

        _LastRemovedLinkInfo = null;
      }
    }
    void ILinksModel.DoLinkRemoved(Object linkdata) { DoLinkRemoved((LinkType)linkdata); }


    /// <summary>
    /// This should be called when a link data's connected node or port, either "from" or "to", may have changed.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a link data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="LinkFromPath"/>, <see cref="LinkToPath"/>,
    /// <see cref="LinkFromParameterPath"/>, or <see cref="LinkToParameterPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindFromNodeKeyForLink"/>, <see cref="FindToNodeKeyForLink"/>,
    /// <see cref="FindFromParameterForLink"/>, or <see cref="FindToParameterForLink"/>
    /// has changed.
    /// </para>
    /// <para>
    /// This raises <see cref="DiagramModel.Changed"/> event(s) with a value of
    /// <see cref="ModelChange.ChangedLinkFromPort"/>
    /// and/or <see cref="ModelChange.ChangedLinkToPort"/>.
    /// </para>
    /// </remarks>
    public void DoLinkPortsChanged(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null) {
        NodeInfo oldfni = li.FromNodeInfo;
        NodeKey oldfromkey = (oldfni != null ? oldfni.Key : default(NodeKey));
        PortKey oldfromparam = li.FromParam;
        NodeKey newfromkey = FindFromNodeKeyForLink(linkdata);
        NodeType newfromdata = FindNodeByKey(newfromkey);
        NodeInfo newfni = FindNodeInfoForNode(newfromdata);

        NodeInfo oldtni = li.ToNodeInfo;
        NodeKey oldtokey = (oldtni != null ? oldtni.Key : default(NodeKey));
        PortKey oldtoparam = li.ToParam;
        NodeKey newtokey = FindToNodeKeyForLink(linkdata);
        NodeType newtodata = FindNodeByKey(newtokey);
        NodeInfo newtni = FindNodeInfoForNode(newtodata);
        
        if (oldfni != newfni) {
          RemoveLinkInfoFromNodeInfo(oldfni, li, true);
          AddLinkInfoToNodeInfo(newfni, li, true);
        }
        if (oldtni != newtni) {
          RemoveLinkInfoFromNodeInfo(oldtni, li, false);
          AddLinkInfoToNodeInfo(newtni, li, false);
        }

        PortKey newfromparam = FindFromParameterForLink(linkdata);  // optional "port" information for link's "from" node
        li.FromParam = newfromparam;
        PortKey newtoparam = FindToParameterForLink(linkdata);  // optional "port" information for link's "to" node
        li.ToParam = newtoparam;

        if (oldfni != newfni || !IsEqualPortParameters(oldfromparam, newfromparam)) {
          RaiseModelChanged(ModelChange.ChangedLinkFromPort, linkdata, oldfromkey, oldfromparam, newfromkey, newfromparam);
        }
        if (oldtni != newtni || !IsEqualPortParameters(oldtoparam, newtoparam)) {
          RaiseModelChanged(ModelChange.ChangedLinkToPort, linkdata, oldtokey, oldtoparam, newtokey, newtoparam);
        }

        ReparentLinkToLowestCommonGroup(li);
      }
    }
    void ILinksModel.DoLinkPortsChanged(Object linkdata) { DoLinkPortsChanged((LinkType)linkdata); }


    /// <summary>
    /// This should be called when a link data's label node may have changed.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a link data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="LinkLabelNodePath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindLabelNodeKeyForLink"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedLinkLabelKey"/>
    /// </para>
    /// </remarks>
    public void DoLinkLabelChanged(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null && this.LinkLabelNodePath != "") {
        NodeInfo oldtni = li.LabelNodeInfo;
        NodeKey oldkey = (oldtni != null ? oldtni.Key : default(NodeKey));
        NodeKey newkey = FindLabelNodeKeyForLink(linkdata);
        NodeType newdata = FindNodeByKey(newkey);
        NodeInfo newtni = FindNodeInfoForNode(newdata);

        if (oldtni != newtni) {
          SetLabelNodeToLinkInfo(li, newtni);
          RaiseModelChanged(ModelChange.ChangedLinkLabelKey, linkdata, oldkey, newkey);
        }
      }
    }
    void ILinksModel.DoLinkLabelChanged(Object linkdata) { DoLinkLabelChanged((LinkType)linkdata); }


    /// <summary>
    /// This should be called when a link data's category value may have changed.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// <para>
    /// This is used for model update, when the model data has changed and the model itself
    /// needs to be updated to reflect those changes.
    /// </para>
    /// <para>
    /// If a link data object implements <see cref="INotifyPropertyChanged"/>
    /// and if the key is a simple property on the data as reflected by the
    /// value of <see cref="LinkCategoryPath"/>,
    /// the model will automatically call this method.
    /// Otherwise, you need to do so immediately after the value of
    /// <see cref="FindCategoryForLink"/> has changed.
    /// </para>
    /// <para>
    /// This raises a <see cref="DiagramModel.Changed"/> event with a value of
    /// <see cref="ModelChange.ChangedLinkCategory"/>.
    /// </para>
    /// </remarks>
    public void DoLinkCategoryChanged(LinkType linkdata) {
      LinkInfo ni = FindLinkInfoForLink(linkdata);
      if (ni != null) {
        String oldcategory = ni.Category;
        String newcategory = FindCategoryForLink(linkdata);
        if (oldcategory != newcategory) {
          ni.Category = newcategory;
          // Notify
          RaiseModelChanged(ModelChange.ChangedLinkCategory, linkdata, oldcategory, newcategory);
        }
      }
    }


    private static bool EqualSequences<T>(IEnumerable<T> a, IEnumerable<T> b) {
      if (a == b) return true;
      if (a == null || b == null) return false;
      return a.SequenceEqual(b);
    }


    // Model navigation:
    // Remember that these methods do not operate on or return FrameworkElements, or even DependencyObjects, but data instances.

    /// <summary>
    /// Return the <typeparamref name="NodeType"/>.
    /// </summary>
    /// <returns>a <see cref="Type"/>, not a node data object, nor a string</returns>
    /// <remarks>
    /// This is useful for data transfer.
    /// </remarks>
    public Type GetNodeType() {
      return typeof(NodeType);
    }

    /// <summary>
    /// This predicate is true when the argument is an instance of <typeparamref name="NodeType"/>.
    /// </summary>
    /// <param name="nodedata">the arbitrary object to be checked for compatibility to be a node data</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> can be cast to the <typeparamref name="NodeType"/>;
    /// false otherwise
    /// </returns>
    public bool IsNodeType(Object nodedata) {
      return nodedata is NodeType;
    }

    /// <summary>
    /// This predicate is true if the argument is a node data in this model.
    /// </summary>
    /// <param name="nodedata">the <typeparamref name="NodeType"/> object to be checked to see if it is a known node data in this model</param>
    /// <returns>
    /// true if the <paramref name="nodedata"/> is in the <see cref="NodesSource"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// This uses a hash table lookup.
    /// </para>
    /// </remarks>
    public bool IsNodeData(NodeType nodedata) {
      if (nodedata == null) return false;
      return _NodeInfos.ContainsKey(nodedata);
    }
    bool IDiagramModel.IsNodeData(Object nodedata) {
      if (nodedata is NodeType)
        return IsNodeData((NodeType)nodedata);
      else
        return false;
    }

    /// <summary>
    /// Return the <typeparamref name="LinkType"/>.
    /// </summary>
    /// <returns>a <see cref="Type"/>, not a link data object, nor a string</returns>
    public bool IsLinkType(Object linkdata) {
      return linkdata is LinkType;
    }

    /// <summary>
    /// This predicate is true if the argument is a link data in this model.
    /// </summary>
    /// <param name="linkdata">the object to be checked to see if it is a known link data in this model</param>
    /// <returns>
    /// true if the <paramref name="linkdata"/> is in the <see cref="LinksSource"/>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation.
    /// </para>
    /// <para>
    /// This uses a hash table lookup.
    /// </para>
    /// </remarks>
    public bool IsLinkData(LinkType linkdata) {
      if (linkdata == null) return false;
      return _LinkInfos.ContainsKey(linkdata);
    }
    bool ILinksModel.IsLinkData(Object linkdata) {
      if (linkdata is LinkType)
        return IsLinkData((LinkType)linkdata);
      else
        return false;
    }


    /// <summary>
    /// Given a key, find the node data with that key.
    /// </summary>
    /// <param name="key">
    /// a value of null for this argument will result in the default value for <typeparamref name="NodeType"/>
    /// </param>
    /// <returns>
    /// a <typeparamref name="NodeType"/>;
    /// the value will be the default for the type if no such node data is known to be in this model
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// If <see cref="NodeKeyIsNodeData"/> is true,
    /// this just converts the <paramref name="key"/> argument
    /// to the <typeparamref name="NodeType"/> type and returns it.
    /// </para>
    /// </remarks>
    public NodeType FindNodeByKey(NodeKey key) {
      if (key == null) return default(NodeType);  // don't compare with default(NodeKey), e.g. to allow use of zero
      if (this.NodeKeyIsNodeData) {
        return (NodeType)((Object)key);
      } else {
        NodeInfo ni;
        if (_IndexedNodes.TryGetValue(key, out ni)) return ni.Data;
        return default(NodeType);
      }
    }
    Object IDiagramModel.FindNodeByKey(Object key) { return FindNodeByKey((NodeKey)key); }


    /// <summary>
    /// This predicate is true if there is a link from one node data/port to another one.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool IsLinked(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      NodeInfo fni = FindNodeInfoForNode(fromdata);
      NodeInfo tni = FindNodeInfoForNode(todata);
      return IsLinkedInfos(fni, fromparam, tni, toparam);
    }
    bool IDiagramModel.IsLinked(Object fromdata, Object fromparam, Object todata, Object toparam) { return IsLinked((NodeType)fromdata, (PortKey)fromparam, (NodeType)todata, (PortKey)toparam); }

    private bool IsLinkedInfos(NodeInfo fni, PortKey fromparam, NodeInfo tni, PortKey toparam) {
      return fni != null && tni != null &&
        tni.ConnectedLinkInfos != null &&
        tni.ConnectedLinkInfos.FirstOrDefault(li => li.FromNodeInfo == fni && IsEqualPortParameters(fromparam, li.FromParam) && IsEqualPortParameters(toparam, li.ToParam)) != null;
    }


    /// <summary>
    /// Return a sequence of node data that are directly connected to a given node, in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="portpred">
    /// a predicate to be applied to each port parameter;
    /// if non-null, this predicate must be true for the node to be included in the return sequence
    /// </param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetConnectedNodesForNode(NodeType nodedata, Predicate<PortKey> portpred) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) {
        // avoid duplicate nodes
        int num = ni.ConnectedLinkInfos.Count;
        if (num == 0) return NoNodes;
        if (num == 1) {
          LinkInfo li = ni.ConnectedLinkInfos.ElementAt(0);
          if (li.FromNodeInfo == ni) {
            if (li.ToNodeInfo != null &&
                (portpred == null || portpred(li.FromParam))) return new NodeType[1] { li.ToNodeInfo.Data };
          } else {
            if (li.FromNodeInfo != null &&
                (portpred == null || portpred(li.ToParam))) return new NodeType[1] { li.FromNodeInfo.Data };
          }
          return NoNodes;
        }
        if (num < 10) {
          List<NodeType> nodelist = new List<NodeType>();
          foreach (LinkInfo li in ni.ConnectedLinkInfos) {
            if (li.FromNodeInfo == ni) {
              if (portpred != null && !portpred(li.FromParam)) continue;
            } else {
              if (portpred != null && !portpred(li.ToParam)) continue;
            }
            NodeInfo other = (li.FromNodeInfo == ni ? li.ToNodeInfo : li.FromNodeInfo);
            NodeType n = (other != null ? other.Data : default(NodeType));
            if (n != null && !nodelist.Contains(n)) nodelist.Add(n);
          }
          return nodelist;
        }
        HashSet<NodeType> nodeset = new HashSet<NodeType>();
        foreach (LinkInfo li in ni.ConnectedLinkInfos) {
          if (li.FromNodeInfo == ni) {
            if (portpred != null && !portpred(li.FromParam)) continue;
          } else {
            if (portpred != null && !portpred(li.ToParam)) continue;
          }
          NodeInfo other = (li.FromNodeInfo == ni ? li.ToNodeInfo : li.FromNodeInfo);
          NodeType n = (other != null ? other.Data : default(NodeType));
          if (n != null && !nodeset.Contains(n)) nodeset.Add(n);
        }
        return nodeset;
      }
      return NoNodes;
    }

    /// <summary>
    /// Return a sequence of node data that are directly connected to a given node, in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetConnectedNodesForNode(NodeType nodedata) { return GetConnectedNodesForNode(nodedata, null); }
    IEnumerable<Object> IDiagramModel.GetConnectedNodesForNode(Object nodedata) { return GetConnectedNodesForNode((NodeType)nodedata, null).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links going into a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="toportpred">
    /// a predicate to be applied to each "to" port parameter;
    /// if non-null, this predicate must be true for the "from" node to be included in the return sequence
    /// </param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetFromNodesForNode(NodeType nodedata, Predicate<PortKey> toportpred) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) {
        // avoid duplicate nodes
        int num = ni.ConnectedLinkInfos.Count;
        if (num == 0) return NoNodes;
        if (num == 1) {
          LinkInfo li = ni.ConnectedLinkInfos.ElementAt(0);
          if (li.ToNodeInfo == ni && li.FromNodeInfo != null && (toportpred == null || toportpred(li.ToParam)))
            return new NodeType[1] { li.FromNodeInfo.Data };
          else
            return NoNodes;
        }
        if (num < 10) {
          List<NodeType> nodelist = new List<NodeType>();
          foreach (LinkInfo li in ni.ConnectedLinkInfos) {
            if (li.ToNodeInfo == ni && li.FromNodeInfo != null && (toportpred == null || toportpred(li.ToParam))) {
              NodeType n = li.FromNodeInfo.Data;
              if (n != null && !nodelist.Contains(n)) nodelist.Add(n);
            }
          }
          return nodelist;
        }
        HashSet<NodeType> nodeset = new HashSet<NodeType>();
        foreach (LinkInfo li in ni.ConnectedLinkInfos) {
          if (li.ToNodeInfo == ni && li.FromNodeInfo != null && (toportpred == null || toportpred(li.ToParam))) {
            NodeType n = li.FromNodeInfo.Data;
            if (n != null && !nodeset.Contains(n)) nodeset.Add(n);
          }
        }
        return nodeset;
      }
      return NoNodes;
    }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links going into a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetFromNodesForNode(NodeType nodedata) { return GetFromNodesForNode(nodedata, null); }
    IEnumerable<Object> IDiagramModel.GetFromNodesForNode(Object nodedata) { return GetFromNodesForNode((NodeType)nodedata, null).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links coming out from a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="fromportpred">
    /// a predicate to be applied to each "from" port parameter;
    /// if non-null, this predicate must be true for the "to" node to be included in the return sequence
    /// </param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetToNodesForNode(NodeType nodedata, Predicate<PortKey> fromportpred) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      // avoid duplicate nodes
      if (ni != null && ni.ConnectedLinkInfos != null) {
        int num = ni.ConnectedLinkInfos.Count;
        if (num == 0) return NoNodes;
        if (num == 1) {
          LinkInfo li = ni.ConnectedLinkInfos.ElementAt(0);
          if (li.FromNodeInfo == ni && li.ToNodeInfo != null && (fromportpred == null || fromportpred(li.FromParam)))
            return new NodeType[1] { li.ToNodeInfo.Data };
          else
            return NoNodes;
        }
        if (num < 10) {
          List<NodeType> nodelist = new List<NodeType>();
          foreach (LinkInfo li in ni.ConnectedLinkInfos) {
            if (li.FromNodeInfo == ni && li.ToNodeInfo != null && (fromportpred == null || fromportpred(li.FromParam))) {
              NodeType n = li.ToNodeInfo.Data;
              if (!nodelist.Contains(n)) nodelist.Add(n);
            }
          }
          return nodelist;
        }
        HashSet<NodeType> nodeset = new HashSet<NodeType>();
        foreach (LinkInfo li in ni.ConnectedLinkInfos) {
          if (li.FromNodeInfo == ni && li.ToNodeInfo != null && (fromportpred == null || fromportpred(li.FromParam))) {
            NodeType n = li.ToNodeInfo.Data;
            if (!nodeset.Contains(n)) nodeset.Add(n);
          }
        }
        return nodeset;
      }
      return NoNodes;
    }

    /// <summary>
    /// Return a sequence of node data that are directly connected by links coming out from a given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetToNodesForNode(NodeType nodedata) { return GetToNodesForNode(nodedata, null); }
    IEnumerable<Object> IDiagramModel.GetToNodesForNode(Object nodedata) { return GetToNodesForNode((NodeType)nodedata, null).Cast<Object>(); }


    /// <summary>
    /// Return the "from" node data at which a link data is connected.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetFromNodeForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null && li.FromNodeInfo != null) return li.FromNodeInfo.Data;
      return default(NodeType);
    }
    Object ILinksModel.GetFromNodeForLink(Object linkdata) { return GetFromNodeForLink((LinkType)linkdata); }

    /// <summary>
    /// Return additional "port" information for the "from" end of a link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public PortKey GetFromParameterForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null) return li.FromParam;
      return default(PortKey);
    }
    Object ILinksModel.GetFromParameterForLink(Object linkdata) { return GetFromParameterForLink((LinkType)linkdata); }

    /// <summary>
    /// Return the "to" node data at which a link data is connected.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetToNodeForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null && li.ToNodeInfo != null) return li.ToNodeInfo.Data;
      return default(NodeType);
    }
    Object ILinksModel.GetToNodeForLink(Object linkdata) { return GetToNodeForLink((LinkType)linkdata); }

    /// <summary>
    /// Return additional "port" information for the "to" end of a link data.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public PortKey GetToParameterForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null) return li.ToParam;
      return default(PortKey);
    }
    Object ILinksModel.GetToParameterForLink(Object linkdata) { return GetToParameterForLink((LinkType)linkdata); }


    /// <summary>
    /// Return a sequence of link data that are connnected at a given node data, in either direction.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetLinksForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) return ni.ConnectedLinkInfos.Select(li => li.Data);
      return NoLinks;
    }
    IEnumerable<Object> ILinksModel.GetLinksForNode(Object nodedata) { return GetLinksForNode((NodeType)nodedata).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of link data that are connnected at a given node data, in either direction,
    /// that satisfy a given predicate.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="predicate">a delegate that takes a <typeparamref name="LinkType"/> as an argument and returns a boolean</param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetLinksForNode(NodeType nodedata, Predicate<LinkType> predicate) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) return ni.ConnectedLinkInfos.Where(li => predicate(li.Data)).Select(li => li.Data);
      return NoLinks;
    }
    IEnumerable<Object> ILinksModel.GetLinksForNode(Object nodedata, Predicate<Object> predicate) {
      return GetLinksForNode((NodeType)nodedata, link => predicate(link)).Cast<Object>();
    }

    /// <summary>
    /// Return a sequence of link data that are connnected at a given node data, only going into the given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetFromLinksForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) return ni.ConnectedLinkInfos.Where(li => li.ToNodeInfo == ni).Select(li => li.Data);
      return NoLinks;
    }
    IEnumerable<Object> ILinksModel.GetFromLinksForNode(Object nodedata) { return GetFromLinksForNode((NodeType)nodedata).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of link data that are connnected at a given node data, only coming out of the given node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetToLinksForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.ConnectedLinkInfos != null) return ni.ConnectedLinkInfos.Where(li => li.FromNodeInfo == ni).Select(li => li.Data);
      return NoLinks;
    }
    IEnumerable<Object> ILinksModel.GetToLinksForNode(Object nodedata) { return GetToLinksForNode((NodeType)nodedata).Cast<Object>(); }

    /// <summary>
    /// Return a sequence of all link data that go from one node to another, possibly restricted to certain "ports".
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetLinksBetweenNodes(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      NodeInfo from = FindNodeInfoForNode(fromdata);
      NodeInfo to = FindNodeInfoForNode(todata);
      if (from == null || to == null) return NoLinks;
      if (to.ConnectedLinkInfos == null) return NoLinks;
      return to.ConnectedLinkInfos.Where(li => IsEqualPortParameters(toparam, li.ToParam) &&
                                               li.FromNodeInfo == from &&
                                               IsEqualPortParameters(fromparam, li.FromParam))
                                  .Select(li => li.Data);
    }
    IEnumerable<Object> ILinksModel.GetLinksBetweenNodes(Object fromdata, Object fromparam, Object todata, Object toparam) { return GetLinksBetweenNodes((NodeType)fromdata, (PortKey)fromparam, (NodeType)todata, (PortKey)toparam).Cast<Object>(); }


    /// <summary>
    /// This predicate is true if the given node data may be used as a "label" for a link.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public bool GetIsLinkLabelForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) return ni.IsLinkLabel;
      return false;
    }
    bool ILinksModel.GetIsLinkLabelForNode(Object nodedata) { return GetIsLinkLabelForNode((NodeType)nodedata); }


    /// <summary>
    /// This predicate is true if the given link data has a "label" node.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool GetHasLabelNodeForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      return (li != null && li.LabelNodeInfo != null);
    }
    bool ILinksModel.GetHasLabelNodeForLink(Object linkdata) { return GetHasLabelNodeForLink((LinkType)linkdata); }

    /// <summary>
    /// Get the "label" node data for a link data, if any.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetLabelNodeForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null && li.LabelNodeInfo != null) return li.LabelNodeInfo.Data;
      return default(NodeType);
    }
    Object ILinksModel.GetLabelNodeForLink(Object linkdata) { return GetLabelNodeForLink((LinkType)linkdata); }

    /// <summary>
    /// This method gets the category of a link.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a String, defaulting to the empty string</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public String GetCategoryForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null) return li.Category;
      return "";
    }
    String ILinksModel.GetCategoryForLink(Object linkdata) { return GetCategoryForLink((LinkType)linkdata); }

    /// <summary>
    /// This predicate is true for a node data if it is associated with a link data as its "label".
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool GetHasLabeledLinkForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      return (ni != null && ni.LabeledLinkInfo != null);
    }
    bool ILinksModel.GetHasLabeledLinkForNode(Object nodedata) { return GetHasLabeledLinkForNode((NodeType)nodedata); }

    /// <summary>
    /// Get the link data that refers to a given node data as its "label".
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a <typeparamref name="LinkType"/> link data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public LinkType GetLabeledLinkForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.LabeledLinkInfo != null) return ni.LabeledLinkInfo.Data;
      return default(LinkType);
    }
    Object ILinksModel.GetLabeledLinkForNode(Object nodedata) { return GetLabeledLinkForNode((NodeType)nodedata); }


    /// <summary>
    /// This method gets the category of a node.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a String, defaulting to the empty string</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public String GetCategoryForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) return ni.Category;
      return "";
    }
    String IDiagramModel.GetCategoryForNode(Object nodedata) { return GetCategoryForNode((NodeType)nodedata); }


    /// <summary>
    /// This predicate is true if a given node data may be a group (i.e. a container) of other nodes.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// <para>
    /// This model assumes that the value will never change.
    /// </para>
    /// </remarks>
    public bool GetIsGroupForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null) return ni.IsGroup;
      return false;
    }
    bool IGroupsModel.GetIsGroupForNode(Object nodedata) { return GetIsGroupForNode((NodeType)nodedata); }

    /// <summary>
    /// This predicate is true if the <paramref name="membernodedata"/> is a member
    /// of the <paramref name="groupnodedata"/> container group.
    /// </summary>
    /// <param name="groupnodedata"></param>
    /// <param name="membernodedata"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public bool IsMember(NodeType groupnodedata, NodeType membernodedata) {
      NodeInfo ci = FindNodeInfoForNode(groupnodedata);
      NodeInfo mi = FindNodeInfoForNode(membernodedata);
      return IsMemberInfos(ci, mi);
    }
    bool IGroupsModel.IsMember(Object groupnodedata, Object membernodedata) { return IsMember((NodeType)groupnodedata, (NodeType)membernodedata); }

    private bool IsMemberInfos(NodeInfo ci, NodeInfo mi) {
      return ci != null && mi != null && mi.GroupNodeInfo == ci;
    }


    /// <summary>
    /// Return the container group node data for a given node data, if there is one.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetGroupForNode(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.GroupNodeInfo != null) return ni.GroupNodeInfo.Data;
      return default(NodeType);
    }
    Object ISubGraphModel.GetGroupForNode(Object nodedata) { return GetGroupForNode((NodeType)nodedata); }

    /// <summary>
    /// Return a sequence of node data that are immediate members of a given node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{NodeType}"/> of member node data; an empty sequence if there are no members</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<NodeType> GetMemberNodesForGroup(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.MemberNodeInfos != null) return ni.MemberNodeInfos.Select(mi => mi.Data);
      return NoNodes;
    }
    IEnumerable<Object> IGroupsModel.GetMemberNodesForGroup(Object nodedata) { return GetMemberNodesForGroup((NodeType)nodedata).Cast<Object>(); }


    /// <summary>
    /// Return a container node data for a given link data, if the link belongs to a group.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>a <typeparamref name="NodeType"/> node data</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public NodeType GetGroupForLink(LinkType linkdata) {
      LinkInfo li = FindLinkInfoForLink(linkdata);
      if (li != null && li.GroupNodeInfo != null) return li.GroupNodeInfo.Data;
      return default(NodeType);
    }
    Object ISubGraphLinksModel.GetGroupForLink(Object linkdata) { return GetGroupForLink((LinkType)linkdata); }

    /// <summary>
    /// Return a sequence of link data that are contained by a given node data.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>an <see cref="IEnumerable{LinkType}"/> of member link data; an empty sequence if there are no member links</returns>
    /// <remarks>
    /// <para>
    /// This is used for model navigation and graph traversal.
    /// </para>
    /// </remarks>
    public IEnumerable<LinkType> GetMemberLinksForGroup(NodeType nodedata) {
      NodeInfo ni = FindNodeInfoForNode(nodedata);
      if (ni != null && ni.MemberLinkInfos != null) return ni.MemberLinkInfos.Select(mi => mi.Data);
      return NoLinks;
    }
    IEnumerable<Object> ISubGraphLinksModel.GetMemberLinksForGroup(Object nodedata) { return GetMemberLinksForGroup((NodeType)nodedata).Cast<Object>(); }


    // Additional services

    // Copying

    /// <summary>
    /// This is the first pass of copying node data, responsible for constructing
    /// a copy and copying most of its properties.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    /// <returns>the copied node data</returns>
    /// <remarks>
    /// <para>
    /// By default this handles <c>ICloneable</c> or serializable objects.
    /// But you may want to override this method to customize which properties
    /// get copied or how they are copied or to provide a faster implementation.
    /// </para>
    /// <para>
    /// This does NOT ensure that the copied node data has a unique key.
    /// You must do that before adding it to the <see cref="NodesSource"/> collection,
    /// either by overriding the data's Clone method,
    /// by declaring the data Serializable (WPF), or
    /// by overriding this method.
    /// Or you can override the <see cref="MakeNodeKeyUnique"/> method that is called when
    /// data is added to the <see cref="NodesSource"/> collection of the model.
    /// </para>
    /// </remarks>
    protected virtual NodeType CopyNode1(NodeType nodedata, CopyDictionary env) {
      if (this.Delegates.CopyNode1 != null) {
        return this.Delegates.CopyNode1(nodedata, env);
      }
#if !WINDOWS_UWP  // binary serialization
      if (nodedata == null) return default(NodeType);  // don't compare with default(NodeType), e.g. to allow copying zero
      if (typeof(ValueType).IsAssignableFrom(typeof(NodeType))) return nodedata;
      ICloneable cloneable = nodedata as ICloneable;
      if (cloneable != null) {
        return (NodeType)cloneable.Clone();
      }
#endif
#if DATASET  // DataRowView
      System.Data.DataRowView row = nodedata as System.Data.DataRowView;
      if (row != null) {
        System.Data.DataRowView newrow = row.DataView.AddNew();
        newrow.Row.ItemArray = (Object[])row.Row.ItemArray.Clone();
        return (NodeType)((Object)newrow);
      }
#endif
#if !WINDOWS_UWP  // binary serialization
      if (typeof(NodeType).GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) {
        try {
          return ModelHelper.CopyByBinarySerialization<NodeType>(nodedata);
        } catch (Exception ex) {
          ModelHelper.Trace(this, "Serialization exception in CopyNode1: " + nodedata.ToString() + "\n" + ex.ToString());
        }
      }
#endif
      //try {
      //  return ModelHelper.CopyByXmlSerialization<NodeType>(nodedata);
      //} catch (Exception ex) {
      //  ModelHelper.Error(this, "CopyNode1: override this method to copy nodes, or have node data implement ICloneable or be Serializable or be XmlSerializable: ", ex);
      //}
      ModelHelper.Error(this, "CopyNode1: override this method to copy nodes, or have node data implement ICloneable or be Serializable");
      return default(NodeType);
    }


    /// <summary>
    /// This is the second pass of copying node data, responsible for fixing up
    /// references to other objects.
    /// </summary>
    /// <param name="oldnodedata">the original node data</param>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    /// <param name="newnodedata">the copied node data</param>
    /// <param name="newgroup">for convenience, the copied "parent" node data</param>
    /// <param name="newmembers">for convenience, a list of newly copied member nodes</param>
    /// <remarks>
    /// <para>
    /// You will want to override this method if the node data should have any references
    /// to copied data.
    /// Otherwise the copied node will appear to have links to the original nodes,
    /// not to the copied nodes.
    /// </para>
    /// <para>
    /// Your overridden method may want to do something like what this method does by default:
    /// <code>
    ///   // this assumes the node data has a reference to its container group
    ///   if (this.GroupNodePath != "" &amp;&amp; newgroup != null) {
    ///     ModifyGroupNodeKey(newnodedata, FindKeyForNode(newgroup));
    ///   }
    ///   // this assumes there's a modifiable collection that InsertMemberNodeKey can work with;
    ///   // if not, you could instead construct your own list and call ModifyMemberNodeKeys.
    ///   if (this.MemberNodesPath != "" &amp;&amp; newmembers != null) {
    ///     // remove any old keys; not needed if the copied property value is empty
    ///     foreach (NodeKey k in FindMemberNodeKeysForNode(newnodedata).OfType&lt;NodeKey&gt;().ToList()) {
    ///       DeleteMemberNodeKey(newnodedata, k);
    ///     }
    ///     // add new keys (which should be different from the old keys)
    ///     foreach (NodeType newmem in newmembers) {
    ///       NodeKey newmemkey = FindKeyForNode(newmem);
    ///       InsertMemberNodeKey(newnodedata, newmemkey);
    ///     }
    ///   }
    /// </code>
    /// </para>
    /// </remarks>
    protected virtual void CopyNode2(NodeType oldnodedata, CopyDictionary env, NodeType newnodedata, NodeType newgroup, IEnumerable<NodeType> newmembers) {
      if (this.Delegates.CopyNode2 != null) {
        this.Delegates.CopyNode2(oldnodedata, env, newnodedata, newgroup, newmembers);
        return;
      }
      if (this.GroupNodePath != "" && newgroup != null) {
        ModifyGroupNodeKey(newnodedata, FindKeyForNode(newgroup));
      }
      if (this.MemberNodesPath != "" && newmembers != null) {
        // remove any old keys
        foreach (NodeKey k in FindMemberNodeKeysForNode(newnodedata).OfType<NodeKey>().ToList()) {
          DeleteMemberNodeKey(newnodedata, k);
        }
        // add new keys
        foreach (NodeType newmem in newmembers) {
          NodeKey newmemkey = FindKeyForNode(newmem);
          InsertMemberNodeKey(newnodedata, newmemkey);
        }
      }
      //ModelHelper.Error(this, "Override CopyNode2");
    }


    /// <summary>
    /// This is the first pass of copying link data, responsible for constructing
    /// a copy and copying most of its properties.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    /// <returns>the copied link data</returns>
    /// <remarks>
    /// <para>
    /// By default this handles <c>ICloneable</c> or serializable (WPF) objects.
    /// But you may want to override this method to customize which properties
    /// get copied or how they are copied or to provide a faster implementation.
    /// </para>
    /// <para>
    /// The resulting new link data may (temporarily) continue to refer to the same
    /// nodes/ports as the original did.  The second pass, <see cref="CopyLink2"/>,
    /// should fix up those references.
    /// </para>
    /// </remarks>
    protected virtual LinkType CopyLink1(LinkType linkdata, CopyDictionary env) {
      if (this.Delegates.CopyLink1 != null) {
        return this.Delegates.CopyLink1(linkdata, env);
      }
#if !WINDOWS_UWP
            if (linkdata == null) return default(LinkType);  // don't compare with default(LinkType), e.g. to allow copying zero
      if (typeof(ValueType).IsAssignableFrom(typeof(LinkType))) return linkdata;
      ICloneable cloneable = linkdata as ICloneable;
      if (cloneable != null) {
        return (LinkType)cloneable.Clone();
      }
#endif
#if DATASET  // DataRowView
      System.Data.DataRowView row = linkdata as System.Data.DataRowView;
      if (row != null) {
        System.Data.DataView view = env.DestinationModel.LinksSource as System.Data.DataView;
        if (view == null) view = row.DataView;
        System.Data.DataRowView newrow = AddNewRow(view, (Object[])row.Row.ItemArray.Clone());
        return (LinkType)((Object)newrow);
      }
#endif
#if !WINDOWS_UWP  // binary serialization
      if (typeof(LinkType).GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) {
        try {
          return ModelHelper.CopyByBinarySerialization<LinkType>(linkdata);
        } catch (Exception ex) {
          ModelHelper.Error(this, "Serialization exception in CopyLink1: " + linkdata.ToString(), ex);
        }
      }
#endif
            //try {
            //  return ModelHelper.CopyByXmlSerialization<LinkType>(linkdata);
            //} catch (Exception ex) {
            //  ModelHelper.Error(this, "CopyLink1: override this method to copy links, or have link data implement ICloneable or be Serializable or be XmlSerializable: ", ex);
            //}
            ModelHelper.Error(this, "CopyLink1: override this method to copy links, or have link data implement ICloneable or be Serializable");
      return default(LinkType);
    }


    /// <summary>
    /// This is the second pass of copying link data, responsible for fixing up
    /// references to other objects.
    /// </summary>
    /// <param name="oldlinkdata">the original link data</param>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    /// <param name="newlinkdata">the copied link data</param>
    /// <param name="newfromnodedata">for convenience, the copied "from" node</param>
    /// <param name="newtonodedata">for convenience, the copied "to" node</param>
    /// <param name="newlinklabel">for convenience, the copied "label" node</param>
    /// <remarks>
    /// <para>
    /// You will want to override this method if the link data should have any references
    /// to copied data.
    /// Otherwise the copied link will appear to connect the original nodes,
    /// not the copied nodes.
    /// </para>
    /// <para>
    /// When <see cref="ValidUnconnectedLinks"/> is <see cref="Northwoods.GoXam.Model.ValidUnconnectedLinks.None"/>
    /// (the default value), if either <paramref name="newfromnodedata"/> or <paramref name="newtonodedata"/>
    /// is null, this will remove the copied link from the model, to avoid having disconnected links.
    /// </para>
    /// </remarks>
    protected virtual void CopyLink2(LinkType oldlinkdata, CopyDictionary env, LinkType newlinkdata, NodeType newfromnodedata, NodeType newtonodedata, NodeType newlinklabel) {
      if (this.Delegates.CopyLink2 != null) {
        this.Delegates.CopyLink2(oldlinkdata, env, newlinkdata, newfromnodedata, newtonodedata, newlinklabel);
        return;
      }
      if (newfromnodedata != null) {
        ModifyLinkFromPort(newlinkdata, FindKeyForNode(newfromnodedata), FindFromParameterForLink(oldlinkdata));
      }
      if (newtonodedata != null) {
        ModifyLinkToPort(newlinkdata, FindKeyForNode(newtonodedata), FindToParameterForLink(oldlinkdata));
      }
      if (newlinklabel != null) {
        ModifyLinkLabelKey(newlinkdata, FindKeyForNode(newlinklabel));
      }
      //ModelHelper.Error(this, "Override CopyLink2");
      if (this.ValidUnconnectedLinks == ValidUnconnectedLinks.None) {
        if (newfromnodedata == null || newtonodedata == null) {
          RemoveLink(newlinkdata);
        }
      }
    }


    // Data Collection

    /// <summary>
    /// Create an empty <see cref="IDataCollection"/> for this model.
    /// </summary>
    /// <returns></returns>
    public virtual DataCollection CreateDataCollection() {
      DataCollection coll = new DataCollection();
      coll.Model = this;
      return coll;
    }
    IDataCollection IDiagramModel.CreateDataCollection() { return CreateDataCollection(); }


    /// <summary>
    /// This nested class is a serializable collection of node data and link data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <see cref="IDataCollection"/> is used in various circumstances where
    /// there is a collection of node data and link data, particularly for copying.
    /// If the node data and link data are serializable, this collection can be serialized,
    /// which is useful when copying to the clipboard or pasting from it.
    /// </para>
    /// <para>
    /// Although this nested type is not a generic class, it is parameterized
    /// by the NodeType, NodeKey, PortKey, and LinkType type parameters of the containing generic model class.
    /// You should call <see cref="CreateDataCollection"/> to allocate an instance.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class DataCollection : IDataCollection {  // nested class
      /// <summary>
      /// The default constructor produces an empty collection.
      /// </summary>
      /// <remarks>
      /// Don't use this constructor -- call <see cref="CreateDataCollection"/> instead.
      /// </remarks>
      public DataCollection() { }

      [NonSerialized]
      GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> _Model;

      /// <summary>
      /// Gets or sets the model that owns all of the nodes in this collection.
      /// </summary>
      public GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> Model {
        get { return _Model; }
        set { _Model = value; }
      }
      IDiagramModel IDataCollection.Model {
        get { return this.Model; }
        set { this.Model = (GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>)value; }
      }


      /// <summary>
      /// Gets or sets the collection of node data, each of type <typeparamref name="NodeType"/>.
      /// </summary>
      /// <value>
      /// Setting this property will enumerate all of the node data
      /// that the argument value collection contains.
      /// If the new value is null, this collection is cleared.
      /// </value>
      public IEnumerable<NodeType> Nodes { 
        get { return _Nodes; }
        set {
          if (value != null)
            _Nodes = new HashSet<NodeType>(value);
          else
            _Nodes.Clear();
        }
      }
      private HashSet<NodeType> _Nodes = new HashSet<NodeType>();
      IEnumerable<Object> IDataCollection.Nodes {
        get { return this.Nodes.Cast<Object>(); }
        set { this.Nodes = value.Cast<NodeType>(); }
      }

      /// <summary>
      /// This predicate is true if the given node data is in the collection of <see cref="Nodes"/>.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <returns></returns>
      public bool ContainsNode(NodeType nodedata) {
        return nodedata != null && _Nodes.Contains(nodedata);
      }
      bool IDataCollection.ContainsNode(Object nodedata) { return ContainsNode((NodeType)nodedata); }

      /// <summary>
      /// Add a node data to this collection.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <remarks>
      /// This is a no-op if the node data is already in the collection.
      /// </remarks>
      public void AddNode(NodeType nodedata) {
        if (nodedata != null && !_Nodes.Contains(nodedata)) _Nodes.Add(nodedata);
      }
      void IDataCollection.AddNode(Object nodedata) { AddNode((NodeType)nodedata); }

      /// <summary>
      /// Remove a node data from this collection.
      /// </summary>
      /// <param name="nodedata">the data type is the model's NodeType type parameter</param>
      /// <remarks>
      /// This is a no-op if the node data was not in the collection.
      /// </remarks>
      public void RemoveNode(NodeType nodedata) {
        if (nodedata != null) _Nodes.Remove(nodedata);
      }
      void IDataCollection.RemoveNode(Object nodedata) { RemoveNode((NodeType)nodedata); }


      /// <summary>
      /// Gets or sets the collection of link data, each of type <typeparamref name="LinkType"/>.
      /// </summary>
      /// <value>
      /// Setting this property will enumerate all of the link data
      /// that the argument value collection contains.
      /// If the new value is null, this collection is cleared.
      /// </value>
      public IEnumerable<LinkType> Links {
        get { return _Links; }
        set {
          if (value != null)
            _Links = new HashSet<LinkType>(value);
          else
            _Links.Clear();
        }
      }
      private HashSet<LinkType> _Links = new HashSet<LinkType>();
      IEnumerable<Object> IDataCollection.Links {
        get { return this.Links.Cast<Object>(); }
        set { this.Links = value.Cast<LinkType>(); }
      }

      /// <summary>
      /// This predicate is true if the given link data is in the collection of <see cref="Links"/>.
      /// </summary>
      /// <param name="linkdata">the data type is the model's LinkType type parameter</param>
      /// <returns></returns>
      public bool ContainsLink(LinkType linkdata) {
        return linkdata != null && _Links.Contains(linkdata);
      }
      bool IDataCollection.ContainsLink(Object linkdata) { return ContainsLink((LinkType)linkdata); }

      /// <summary>
      /// Add a link data to this collection.
      /// </summary>
      /// <param name="linkdata">the data type is the model's LinkType type parameter</param>
      /// <remarks>
      /// This is a no-op if the link data is already in the collection.
      /// </remarks>
      public void AddLink(LinkType linkdata) {
        if (linkdata != null && !_Links.Contains(linkdata)) _Links.Add(linkdata);
      }
      void IDataCollection.AddLink(Object linkdata) { AddLink((LinkType)linkdata); }

      /// <summary>
      /// Remove a link data from this collection.
      /// </summary>
      /// <param name="linkdata">the data type is the model's LinkType type parameter</param>
      /// <remarks>
      /// This is a no-op if the link data was not in the collection.
      /// </remarks>
      public void RemoveLink(LinkType linkdata) {
        if (linkdata != null) _Links.Remove(linkdata);
      }
      void IDataCollection.RemoveLink(Object linkdata) { RemoveLink((LinkType)linkdata); }
    }  // end of DataCollection class


    // CopyDictionary

    /// <summary>
    /// Create an <see cref="ICopyDictionary"/> initialized for this model.
    /// </summary>
    /// <returns>Normally this will be an empty dictionary.</returns>
    public virtual CopyDictionary CreateCopyDictionary() {
      CopyDictionary env = new CopyDictionary();
      env.DestinationModel = this;
      return env;
    }
    ICopyDictionary IDiagramModel.CreateCopyDictionary() { return CreateCopyDictionary(); }


    /// <summary>
    /// This nested class is used during copying to hold a mapping of original nodes
    /// to newly copied nodes and original links to newly copied links.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This <see cref="ICopyDictionary"/> is used during the two-pass copying process
    /// to keep track of which newly copied node data correspond to which original node data
    /// and which newly copied link data correspond to which original link data.
    /// </para>
    /// <para>
    /// Although this nested type is not a generic class, it is parameterized
    /// by the NodeType, NodeKey, PortKey, and LinkType type parameters of the containing generic model class.
    /// You should call <see cref="CreateCopyDictionary"/> to allocate an instance.
    /// </para>
    /// </remarks>
    public sealed class CopyDictionary : ICopyDictionary {  // nested class
      /// <summary>
      /// The default constructor builds an empty dictionary.
      /// </summary>
      /// <remarks>
      /// Don't use this constructor -- call <see cref="CreateCopyDictionary"/> instead.
      /// </remarks>
      public CopyDictionary() { }

      /// <summary>
      /// Gets or sets the source model for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.AddCollectionCopy"/> method.
      /// </remarks>
      public GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> SourceModel { get; set; }
      IDiagramModel ICopyDictionary.SourceModel {
        get { return this.SourceModel; }
        set { this.SourceModel = (GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>)value; }
      }

      /// <summary>
      /// Gets or sets the destination model for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.AddCollectionCopy"/> method.
      /// </remarks>
      public GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> DestinationModel { get; set; }
      IDiagramModel ICopyDictionary.DestinationModel {
        get { return this.DestinationModel; }
        set { this.DestinationModel = (GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>)value; }
      }

      /// <summary>
      /// Gets or sets the source collection of data to be copied for the copying operation.
      /// </summary>
      /// <remarks>
      /// This will be set by the <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.AddCollectionCopy"/> method.
      /// </remarks>
      public DataCollection SourceCollection { get; set; }
      IDataCollection ICopyDictionary.SourceCollection {
        get { return this.SourceCollection; }
        set { this.SourceCollection = (DataCollection)value; }
      }


      /// <summary>
      /// Gets the collection of copied nodes and copied links as a <see cref="DataCollection"/>.
      /// </summary>
      public DataCollection Copies {
        get {
          DataCollection c = this.DestinationModel.CreateDataCollection();
          c.Nodes = _NodeCopies.Select(kv => kv.Value).Where(n => n != null);
          c.Links = _LinkCopies.Select(kv => kv.Value).Where(l => l != null);
          return c;
        }
      }
      IDataCollection ICopyDictionary.Copies {
        get { return this.Copies; }
      }

      private Dictionary<NodeType, NodeType> _NodeCopies = new Dictionary<NodeType, NodeType>();

      /// <summary>
      /// This predicate is true if the given node data is in the source collection.
      /// </summary>
      /// <param name="oldnode"></param>
      /// <returns></returns>
      public bool ContainsSourceNode(NodeType oldnode) {
        if (oldnode == null) return false;
        return _NodeCopies.ContainsKey(oldnode);
      }
      bool ICopyDictionary.ContainsSourceNode(Object srcnodedata) { return ContainsSourceNode((NodeType)srcnodedata); }

      /// <summary>
      /// Look up the copied node for a given source node.
      /// </summary>
      /// <param name="oldnode">a source node data</param>
      /// <returns>the copied node data</returns>
      public NodeType FindCopiedNode(NodeType oldnode) {
        if (oldnode == null) return default(NodeType);  // don't compare with default(NodeType), e.g. to allow looking up zero
        NodeType newnode;
        _NodeCopies.TryGetValue(oldnode, out newnode);
        return newnode;
      }
      Object ICopyDictionary.FindCopiedNode(Object srcnodedata) { return FindCopiedNode((NodeType)srcnodedata); }

      /// <summary>
      /// Declare the mapping of a source node data to a copied node data.
      /// </summary>
      /// <param name="oldnode">a node data in the source collection</param>
      /// <param name="newnode">
      /// a copied node data,
      /// or null to indicate that there is no copied node data for a given source node data
      /// </param>
      /// <remarks>
      /// This will add a node data to the copies collection, associated with the source node data.
      /// </remarks>
      public void AddCopiedNode(NodeType oldnode, NodeType newnode) {
        if (oldnode != null) _NodeCopies[oldnode] = newnode;
      }
      void ICopyDictionary.AddCopiedNode(Object srcnodedata, Object dstnodedata) { AddCopiedNode((NodeType)srcnodedata, (NodeType)dstnodedata); }

      /// <summary>
      /// Remove any association between a source node data and any copied node data.
      /// </summary>
      /// <param name="srcnodedata">a node data in the source collection</param>
      public void RemoveSourceNode(NodeType srcnodedata) {
        if (srcnodedata != null) _NodeCopies.Remove(srcnodedata);
      }
      void ICopyDictionary.RemoveSourceNode(Object srcnodedata) { RemoveSourceNode((NodeType)srcnodedata); }


      private Dictionary<LinkType, LinkType> _LinkCopies = new Dictionary<LinkType, LinkType>();

      /// <summary>
      /// This predicate is true if the given link data is in the source collection.
      /// </summary>
      /// <param name="oldlink"></param>
      /// <returns></returns>
      public bool ContainsSourceLink(LinkType oldlink) {
        if (oldlink == null) return false;
        return _LinkCopies.ContainsKey(oldlink);
      }
      bool ICopyDictionary.ContainsSourceLink(Object srclinkdata) { return ContainsSourceLink((LinkType)srclinkdata); }

      /// <summary>
      /// Look up the copied link for a given source link.
      /// </summary>
      /// <param name="oldlink">a source link data</param>
      /// <returns>the copied link data</returns>
      public LinkType FindCopiedLink(LinkType oldlink) {
        if (oldlink == null) return default(LinkType);
        LinkType newlink;
        _LinkCopies.TryGetValue(oldlink, out newlink);
        return newlink;
      }
      Object ICopyDictionary.FindCopiedLink(Object srclinkdata) { return FindCopiedLink((LinkType)srclinkdata); }

      /// <summary>
      /// Declare the mapping of a source link data to a copied link data.
      /// </summary>
      /// <param name="oldlink">a link data in the source collection</param>
      /// <param name="newlink">
      /// a copied link data,
      /// or null to indicate that there is no copied link data for a given source link data
      /// </param>
      /// <remarks>
      /// This will add a link data to the copies collection, associated with the source link data.
      /// </remarks>
      public void AddCopiedLink(LinkType oldlink, LinkType newlink) {
        if (oldlink != null) _LinkCopies[oldlink] = newlink;
      }
      void ICopyDictionary.AddCopiedLink(Object srclinkdata, Object dstlinkdata) { AddCopiedLink((LinkType)srclinkdata, (LinkType)dstlinkdata); }

      /// <summary>
      /// Remove any association between a source link data and any copied link data.
      /// </summary>
      /// <param name="srclinkdata">a link data in the source collection</param>
      public void RemoveSourceLink(LinkType srclinkdata) {
        if (srclinkdata != null) _LinkCopies.Remove(srclinkdata);
      }
      void ICopyDictionary.RemoveSourceLink(Object srclinkdata) { RemoveSourceLink((LinkType)srclinkdata); }
    }  // end of CopyDictionary class


    /// <summary>
    /// Add a copy of a node data to this model.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns>the copied node data</returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that just calls <see cref="AddCollectionCopy"/>.
    /// </para>
    /// </remarks>
    public NodeType AddNodeCopy(NodeType nodedata) {
      if (nodedata == null) return default(NodeType);
      DataCollection coll = CreateDataCollection();
      coll.AddNode(nodedata);
      return AddCollectionCopy(coll, null).Copies.Nodes.FirstOrDefault();
    }
    Object IDiagramModel.AddNodeCopy(Object nodedata) { return AddNodeCopy((NodeType)nodedata); }

    /// <summary>
    /// Add a copy of a link data to this model.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <returns>the copied link data</returns>
    /// <remarks>
    /// <para>
    /// This is a convenience method that just calls <see cref="AddCollectionCopy"/>.
    /// </para>
    /// </remarks>
    public LinkType AddLinkCopy(LinkType linkdata) {
      if (linkdata == null) return default(LinkType);
      DataCollection coll = CreateDataCollection();
      coll.AddLink(linkdata);
      return AddCollectionCopy(coll, null).Copies.Links.FirstOrDefault();
    }
    Object ILinksModel.AddLinkCopy(Object linkdata) { return AddLinkCopy((LinkType)linkdata); }


    /// <summary>
    /// Copy existing node and link data and add to this model.
    /// </summary>
    /// <param name="coll">the collection of data to be copied</param>
    /// <param name="env">
    /// the <see cref="ICopyDictionary"/> used to keep track of copied objects;
    /// if null, the method will call <see cref="CreateCopyDictionary"/>, use it, and return it
    /// </param>
    /// <returns>the updated copy dictionary, mapping original data to copied data</returns>
    /// <remarks>
    /// <para>
    /// The primary purpose of this method is to perform a two-pass copy of a part of a diagram,
    /// and add the resulting data to this model.
    /// </para>
    /// <para>
    /// Of course you can add data without copying them by calling <see cref="AddNode"/> and <see cref="AddLink(LinkType)"/>
    /// or by just adding them directly to the <see cref="NodesSource"/> and <see cref="LinksSource"/>.
    /// </para>
    /// <para>
    /// This calls <see cref="AugmentCopyCollection"/> on the source model to allow it to extend the
    /// collection to include parts that it thinks should be in the collection.
    /// </para>
    /// <para>
    /// Then it calls <see cref="AugmentCopyDictionary"/> on this, the destination model, to allow it
    /// to prepopulate the <see cref="CopyDictionary"/> if it wants to guide the copying process to
    /// control the sharing of references in the copied parts.
    /// </para>
    /// <para>
    /// The first pass copies all of the nodes that are not already in the <paramref name="env"/>
    /// copy dictionary, by calling <see cref="CopyNode1"/>.  If the call returns a node data,
    /// it is added to this model by calling <see cref="AddNode"/> and remembered in the
    /// <paramref name="env"/> copy dictionary, mapped to the original node data.
    /// </para>
    /// <para>
    /// The first pass also copies all of the links that are not already in the <paramref name="env"/>
    /// copy dictionary, by calling <see cref="CopyLink1"/>.  If the call returns a link data,
    /// it is added to this model by calling <see cref="AddLink(LinkType)"/> and remembered in the
    /// <paramref name="env"/> copy dictionary, mapped to the original link data.
    /// </para>
    /// <para>
    /// The second pass fixes up references in all of the copied nodes by calling <see cref="CopyNode2"/>.
    /// It passes as arguments both the original node data and the copied node data, as well as the
    /// newly copied group node, if any, and a list of any member nodes.
    /// </para>
    /// <para>
    /// The second pass also fixes up references in all of the copied links by calling <see cref="CopyLink2"/>.
    /// It passes as arguments both the original link data and the copied link data, as well as the
    /// newly copied from, to, and label nodes, if any.
    /// </para>
    /// </remarks>
    public CopyDictionary AddCollectionCopy(DataCollection coll, CopyDictionary env) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot CopyCollection");
      if (coll == null) return env;
      GraphLinksModel<NodeType, NodeKey, PortKey, LinkType> srcmodel = coll.Model;
      if (env == null) env = new CopyDictionary();
      env.SourceModel = srcmodel;
      env.DestinationModel = this;
      env.SourceCollection = coll;
      // ask the source model to extend the collection to include expected parts
      srcmodel.AugmentCopyCollection(coll);
      // allow this destination model to prepopulate the CopyDictionary
      AugmentCopyDictionary(env);
      // first pass: individual copies
      foreach (NodeType n in coll.Nodes) {
        // already in environment? don't need to make a new copy
        if (env.ContainsSourceNode(n)) continue;
        // otherwise make a copy of the node, and save it in the CopyDictionary
        NodeType copy = CopyNode1(n, env);
        if (copy == null) continue;  // if no copy created, never mind
        AddNode(copy);  // this should add it to the NodesSource collection
        env.AddCopiedNode(n, copy);
      }
      foreach (LinkType l in coll.Links) {
        // already in environment? don't need to make a new copy
        if (env.ContainsSourceLink(l)) continue;
        // otherwise make a copy of the link, and save it in the CopyDictionary
        LinkType copy = CopyLink1(l, env);
        if (copy == null) continue;  // if no copy created, never mind
        AddLink(copy);  // this should add it to the LinksSource collection
        env.AddCopiedLink(l, copy);
      }
      // second pass: fix up references
      foreach (NodeType n in coll.Nodes) {
        if (!env.ContainsSourceNode(n)) continue;
        NodeType copy = env.FindCopiedNode(n);
        if (copy == null) continue;
        NodeType oldsg = srcmodel.GetGroupForNode(n);
        NodeType newsg = env.FindCopiedNode(oldsg);
        IEnumerable<NodeType> newmembers = srcmodel.GetMemberNodesForGroup(n).Select(m => env.FindCopiedNode(m)).Where(c => c != null);
        CopyNode2(n, env, copy, newsg, newmembers);
        bool needsnotification = !(copy is INotifyPropertyChanged);
#if DATASET  //?? DataRowView is not notifying, even though HandleNodePropertyChanged has been registered
        needsnotification |= (copy is System.Data.DataRowView);
#endif
        if (needsnotification && IsNodeData(copy)) {
          DoGroupNodeChanged(copy);
          DoMemberNodeKeysChanged(copy);
        }
      }
      foreach (LinkType l in coll.Links) {
        if (!env.ContainsSourceLink(l)) continue;
        LinkType copy = env.FindCopiedLink(l);
        if (copy == null) continue;
        NodeType oldfrom = srcmodel.GetFromNodeForLink(l);
        NodeType newfrom = env.FindCopiedNode(oldfrom);
        NodeType oldto = srcmodel.GetToNodeForLink(l);
        NodeType newto = env.FindCopiedNode(oldto);
        NodeType oldlab = srcmodel.GetLabelNodeForLink(l);
        NodeType newlab = env.FindCopiedNode(oldlab);
        CopyLink2(l, env, copy, newfrom, newto, newlab);
        bool needsnotification = !(copy is INotifyPropertyChanged);
#if DATASET  //?? DataRowView is not notifying, even though HandleLinkPropertyChanged has been registered
        needsnotification |= (copy is System.Data.DataRowView);
#endif
        if (needsnotification && IsLinkData(copy)) {
          DoLinkPortsChanged(copy);
          DoLinkLabelChanged(copy);
        }
      }
      return env;
    }
    ICopyDictionary IDiagramModel.AddCollectionCopy(IDataCollection coll, ICopyDictionary env) {
      return AddCollectionCopy((DataCollection)coll, (CopyDictionary)env);
    }


    /// <summary>
    /// Override this method to add more (related) data to be copied.
    /// </summary>
    /// <param name="coll">the collection of data to be copied</param>
    protected virtual void AugmentCopyCollection(DataCollection coll) {
      if (this.Delegates.AugmentCopyCollection != null) {
        this.Delegates.AugmentCopyCollection(coll);
        return;
      }
      // recursively add all of the member nodes and links for each group to the COLL collection
      if (this.CopyingGroupCopiesMembers) AddGroupParts(coll);
      // add any link labels
      if (this.CopyingLinkCopiesLabel) AddLabels(coll);
    }


    internal /*?? public */ bool CopyingGroupCopiesMembers {
      get { return _CopyingGroupCopiesMembers; }
      set {
        bool old = _CopyingGroupCopiesMembers;
        if (old != value) {
          _CopyingGroupCopiesMembers = value;
          RaiseModelChanged(ModelChange.ChangedCopyingGroupCopiesMembers, null, old, value);
        }
      }
    }


    private void AddGroupParts(DataCollection coll) {
      foreach (NodeType n in coll.Nodes.ToArray()) {  // work on copy, to allow modification of COLL
        AddGroupParts1(n, coll);
      }
    }

    private void AddGroupParts1(NodeType n, DataCollection coll) {
      coll.AddNode(n);
      if (GetIsGroupForNode(n)) {
        foreach (NodeType c in GetMemberNodesForGroup(n)) {
          AddGroupParts1(c, coll);
        }
        foreach (LinkType l in GetMemberLinksForGroup(n)) {
          coll.AddLink(l);
        }
      }
    }

    internal /*?? public */ bool CopyingLinkCopiesLabel {
      get { return _CopyingLinkCopiesLabel; }
      set {
        bool old = _CopyingLinkCopiesLabel;
        if (old != value) {
          _CopyingLinkCopiesLabel = value;
          RaiseModelChanged(ModelChange.ChangedCopyingLinkCopiesLabel, null, old, value);
        }
      }
    }

    private void AddLabels(DataCollection coll) {
      foreach (LinkType l in coll.Links) {
        if (GetHasLabelNodeForLink(l)) {
          coll.AddNode(GetLabelNodeForLink(l));
        }
      }
    }


    /// <summary>
    /// Override this method to avoid copying some data, or to change how copied references are resolved.
    /// </summary>
    /// <param name="env">the dictionary mapping original objects to copied objects</param>
    protected virtual void AugmentCopyDictionary(CopyDictionary env) {
      if (this.Delegates.AugmentCopyDictionary != null) {
        this.Delegates.AugmentCopyDictionary(env);
        return;
      }
    }


    // Insertion

    /// <summary>
    /// This method actually implements the addition of a node data to the
    /// <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="NodesSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void InsertNode(NodeType nodedata) {
      if (this.Delegates.InsertNode != null) {
        this.Delegates.InsertNode(this, nodedata);
        return;
      }
      if (nodedata == null) return;
      if (IsNodeData(nodedata)) return;  // already present
      System.Collections.IList list = this.NodesSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
#if DATASET  // DataRowView
        System.Data.DataView view = list as System.Data.DataView;
        if (view != null) {
          System.Data.DataRowView row = nodedata as System.Data.DataRowView;
          if (row != null && row.DataView == view) {
            DoNodeAdded((NodeType)((Object)row));
          } else {
            System.Data.DataRowView newrow = AddNewRow(view, (Object[])row.Row.ItemArray.Clone());
            DoReplaceReference(row, newrow);
            DoNodeAdded((NodeType)((Object)newrow));
          }
          return;
        }
#endif
        list.Add(nodedata);
        if (!this.NodesSourceNotifies) DoNodeAdded(nodedata);
      } else {
        IList<NodeType> nlist = this.NodesSource as IList<NodeType>;
        if (nlist != null && !nlist.IsReadOnly) {
          nlist.Add(nodedata);
          if (!this.NodesSourceNotifies) DoNodeAdded(nodedata);
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertNode to support adding node data to NodesSource, which does not implement IList");
        }
      }
    }


    /// <summary>
    /// Add a node data to <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This just calls <see cref="InsertNode"/> to actually perform the addition.
    /// This is a no-op if the node data is already in the model.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddNode(NodeType nodedata) {
      if (nodedata == null) return;
      if (IsNodeData(nodedata)) return;  // already in model
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddNode");
      InsertNode(nodedata);
    }
    void IDiagramModel.AddNode(Object nodedata) { AddNode((NodeType)nodedata); }


    /// <summary>
    /// This method actually implements the addition of a link data to the
    /// <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinksSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{LinkType}"/>.
    /// </remarks>
    protected virtual void InsertLink(LinkType linkdata) {
      if (this.Delegates.InsertLink != null) {
        this.Delegates.InsertLink(this, linkdata);
        return;
      }
      if (linkdata == null) return;
      if (IsLinkData(linkdata)) return;  // already present
      System.Collections.IList list = this.LinksSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
#if DATASET  // DataRowView
        System.Data.DataView view = list as System.Data.DataView;
        if (view != null) {
          System.Data.DataRowView row = linkdata as System.Data.DataRowView;
          if (row != null && row.DataView == view) {
            DoLinkAdded((LinkType)((Object)row));
          } else {
            System.Data.DataRowView newrow = AddNewRow(view, (Object[])row.Row.ItemArray.Clone());
            DoReplaceReference(row, newrow);
            DoLinkAdded((LinkType)((Object)newrow));
          }
          return;
        }
#endif
        list.Add(linkdata);
        if (!this.LinksSourceNotifies) DoLinkAdded(linkdata);
      } else {
        IList<LinkType> llist = this.LinksSource as IList<LinkType>;
        if (llist != null && !llist.IsReadOnly) {
          llist.Add(linkdata);
          if (!this.LinksSourceNotifies) DoLinkAdded(linkdata);
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertLink(LinkType) to support adding link data to LinksSource, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// This method actually implements the creation and addition of a link data to the
    /// <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinksSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{LinkType}"/>.
    /// </remarks>
    protected virtual LinkType InsertLink(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      if (this.Delegates.InsertLink2 != null) {
        return this.Delegates.InsertLink2(this, fromdata, fromparam, todata, toparam);
      }
      if (typeof(GraphLinksModelLinkData<NodeKey, PortKey>).IsAssignableFrom(typeof(LinkType))) {
        GraphLinksModelLinkData<NodeKey, PortKey> newlink = (GraphLinksModelLinkData<NodeKey, PortKey>)Activator.CreateInstance(typeof(LinkType));
        newlink.From = FindKeyForNode(fromdata);
        newlink.FromPort = fromparam;
        newlink.To = FindKeyForNode(todata);
        newlink.ToPort = toparam;
        LinkType newlinkdata = (LinkType)((Object)newlink);
        InsertLink(newlinkdata);
        return newlinkdata;
      }
      if (typeof(UniversalLinkData).IsAssignableFrom(typeof(LinkType))) {
        UniversalLinkData newlink = (UniversalLinkData)Activator.CreateInstance(typeof(LinkType));
        newlink.From = FindKeyForNode(fromdata);
        newlink.FromPort = fromparam as String;
        newlink.To = FindKeyForNode(todata);
        newlink.ToPort = toparam as String;
        LinkType newlinkdata = (LinkType)((Object)newlink);
        InsertLink(newlinkdata);
        return newlinkdata;
      }
      ModelHelper.Error(this, "Override InsertLink(NodeType, PortKey, NodeType, PortKey) to support creating a new link");
      return default(LinkType);
    }

    /// <summary>
    /// Add a link data to <see cref="LinksSource"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// This just calls <see cref="InsertLink(LinkType)"/> to actually perform the addition.
    /// This is a no-op if the link data is already in the model.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddLink(LinkType linkdata) {
      if (IsLinkData(linkdata)) return;  // already in model
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddLink");
      InsertLink(linkdata);
    }
    void ILinksModel.AddLink(Object linkdata) { AddLink((LinkType)linkdata); }

    /// <summary>
    /// Add a link data to <see cref="LinksSource"/>.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <remarks>
    /// This just calls <see cref="InsertLink(NodeType, PortKey, NodeType, PortKey)"/> to actually perform the addition.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public LinkType AddLink(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddLink");
      return InsertLink(fromdata, fromparam, todata, toparam);
    }
    Object IDiagramModel.AddLink(Object fromdata, Object fromparam, Object todata, Object toparam) { return AddLink((NodeType)fromdata, (PortKey)fromparam, (NodeType)todata, (PortKey)toparam); }


    // Deletion

    /// <summary>
    /// This method actually implements the removal of a node data from the
    /// <see cref="NodesSource"/> collection.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="NodesSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void DeleteNode(NodeType nodedata) {
      if (this.Delegates.DeleteNode != null) {
        this.Delegates.DeleteNode(this, nodedata);
        return;
      }
      if (nodedata == null) return;
      if (!IsNodeData(nodedata)) return;  // not present
      System.Collections.IList list = this.NodesSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        Object cols = null;
#if DATASET  // DataRowView
        System.Data.DataRowView row = nodedata as System.Data.DataRowView;
        if (row != null) cols = row.Row.ItemArray.Clone();
#endif
        RaiseModelChanged(ModelChange.RemovingNode, nodedata, cols, null);
        list.Remove(nodedata);
        if (!this.NodesSourceNotifies) DoNodeRemoved(nodedata);
      } else {
        IList<NodeType> nlist = this.NodesSource as IList<NodeType>;
        if (nlist != null && !nlist.IsReadOnly) {
          RaiseModelChanged(ModelChange.RemovingNode, nodedata, null, null);
          nlist.Remove(nodedata);
          if (!this.NodesSourceNotifies) DoNodeRemoved(nodedata);
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteNode to support removing node data from NodesSource, which does not implement IList");
        }
      }
    }


    /// <summary>
    /// Remove a node data from <see cref="NodesSource"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <remarks>
    /// This also removes any links connected to this node.
    /// This just calls <see cref="DeleteNode"/> to actually perform the removal.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveNode(NodeType nodedata) {
      if (nodedata == null) return;
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveNode");
      if (this.RemovingGroupRemovesMembers) RemoveGroupMembers(nodedata);
      RemoveConnectedLinks(nodedata);
      DeleteNode(nodedata);
    }
    void IDiagramModel.RemoveNode(Object nodedata) { RemoveNode((NodeType)nodedata); }


    private void RemoveConnectedLinks(NodeType nodedata) {
      foreach (LinkType link in GetLinksForNode(nodedata).ToArray()) {  // work on copy, to allow modification of collection
        RemoveLink(link);
      }
    }


    internal /*?? public */ bool RemovingGroupRemovesMembers {
      get { return _RemovingGroupRemovesMembers; }
      set {
        bool old = _RemovingGroupRemovesMembers;
        if (old != value) {
          _RemovingGroupRemovesMembers = value;
          RaiseModelChanged(ModelChange.ChangedRemovingGroupRemovesMembers, null, old, value);
        }
      }
    }


    private void RemoveGroupMembers(NodeType nodedata) {
      if (GetIsGroupForNode(nodedata)) {
        foreach (LinkType link in GetMemberLinksForGroup(nodedata).ToArray()) {  // work on copy, to allow modification of collection
          RemoveLink(link);
        }
        foreach (NodeType node in GetMemberNodesForGroup(nodedata).ToArray()) {  // work on copy, to allow modification of collection
          RemoveNode(node);
        }
      }
    }


    /// <summary>
    /// This method actually implements the removal of a link data from the
    /// <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinksSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{LinkType}"/>.
    /// </remarks>
    protected virtual void DeleteLink(LinkType linkdata) {
      if (this.Delegates.DeleteLink != null) {
        this.Delegates.DeleteLink(this, linkdata);
        return;
      }
      if (linkdata == null) return;
      if (!IsLinkData(linkdata)) return;  // not present
      System.Collections.IList list = this.LinksSource as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        Object cols = null;
#if DATASET  // DataRowView
        System.Data.DataRowView row = linkdata as System.Data.DataRowView;
        if (row != null) cols = row.Row.ItemArray.Clone();
#endif
        RaiseModelChanged(ModelChange.RemovingLink, linkdata, cols, null);
        list.Remove(linkdata);
        if (!this.LinksSourceNotifies) DoLinkRemoved(linkdata);
      } else {
        IList<LinkType> llist = this.LinksSource as IList<LinkType>;
        if (llist != null && !llist.IsReadOnly) {
          RaiseModelChanged(ModelChange.RemovingLink, linkdata, null, null);
          llist.Remove(linkdata);
          if (!this.LinksSourceNotifies) DoLinkRemoved(linkdata);
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteLink to support removing link data from LinksSource, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// This method actually implements the removal of a link data from the
    /// <see cref="LinksSource"/> collection.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <remarks>
    /// By default this calls <see cref="DeleteLink(LinkType)"/> on each link found by
    /// <see cref="GetLinksBetweenNodes"/>.
    /// This method can be overridden in case the <see cref="LinksSource"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{LinkType}"/>.
    /// </remarks>
    protected virtual void DeleteLink(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      if (this.Delegates.DeleteLink2 != null) {
        this.Delegates.DeleteLink2(this, fromdata, fromparam, todata, toparam);
        return;
      }
      if (fromdata == null || todata == null) return;
      NodeInfo from = FindNodeInfoForNode(fromdata);
      NodeInfo to = FindNodeInfoForNode(todata);
      if (from == null || to == null) return;
      // work on copy of list of links to avoid concurrent modification problems
      foreach (LinkType linkdata in GetLinksBetweenNodes(fromdata, fromparam, todata, toparam).ToArray()) {
        DeleteLink(linkdata);
      }
    }

    /// <summary>
    /// Remove a link data from <see cref="LinksSource"/>.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <remarks>
    /// This also removes any links connected to this link.
    /// This just calls <see cref="DeleteLink(LinkType)"/> to actually perform the removal.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveLink(LinkType linkdata) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveLink");
      if (this.RemovingLinkRemovesLabel) RemoveLabel(linkdata);
      DeleteLink(linkdata);
    }
    void ILinksModel.RemoveLink(Object linkdata) { RemoveLink((LinkType)linkdata); }

    /// <summary>
    /// Remove all link data from <see cref="LinksSource"/> that connect the two nodes/ports.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <remarks>
    /// This also removes any links connected to this link.
    /// This just calls <see cref="DeleteLink(NodeType, PortKey, NodeType, PortKey)"/> to actually perform the removal.
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveLink(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveLink");
      DeleteLink(fromdata, fromparam, todata, toparam);
    }
    void IDiagramModel.RemoveLink(Object fromdata, Object fromparam, Object todata, Object toparam) { RemoveLink((NodeType)fromdata, (PortKey)fromparam, (NodeType)todata, (PortKey)toparam); }



    internal /*?? public */ bool RemovingLinkRemovesLabel {
      get { return _RemovingLinkRemovesLabel; }
      set {
        bool old = _RemovingLinkRemovesLabel;
        if (old != value) {
          _RemovingLinkRemovesLabel = value;
          RaiseModelChanged(ModelChange.ChangedRemovingLinkRemovesLabel, null, old, value);
        }
      }
    }

    private void RemoveLabel(LinkType linkdata) {
      if (GetHasLabelNodeForLink(linkdata)) {
        NodeType label = GetLabelNodeForLink(linkdata);
        RemoveNode(label);
      }
    }


    // Modification

    /// <summary>
    /// This method actually implements the modification of a link data
    /// to change the reference to the "from" node data and the link's "from" port information.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="nodekey">the new node data's <typeparamref name="NodeKey"/></param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinkFromPath"/>
    /// or <see cref="LinkFromParameterPath"/>
    /// property paths are not flexible enough or fast enough to get the node and port for a link.
    /// </remarks>
    protected virtual void ModifyLinkFromPort(LinkType linkdata, NodeKey nodekey, PortKey portparam) {
      if (this.Delegates.SetLinkFromPort != null) {
        this.Delegates.SetLinkFromPort(this, linkdata, nodekey, portparam);
        return;
      }
      String path = this.LinkFromPath;
      if (path != null && path.Length > 0) {
        _LinkFromPathPPI.SetFor(linkdata, nodekey);
      } else {
        ModelHelper.Error(this, "Override ModifyLinkFromPort to support relinking existing link data");
      }
      String ppath = this.LinkFromParameterPath;
      if (ppath != null && ppath.Length > 0) {
        _LinkFromParameterPathPPI.SetFor(linkdata, portparam);
      } else {
        //ModelHelper.Error(this, "Override ModifyLinkFromPort to support relinking existing link data");
      }
    }

    /// <summary>
    /// Change a link data so that it refers to a different "from" node data and associated port information.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="nodekey">the new node data's <typeparamref name="NodeKey"/></param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// This calls <see cref="ModifyLinkFromPort"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetLinkFromPort(LinkType linkdata, NodeKey nodekey, PortKey portparam) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetLinkFromPort");
      ModifyLinkFromPort(linkdata, nodekey, portparam);
    }
    void ILinksModel.SetLinkFromPort(Object linkdata, Object nodedata, Object portparam) { SetLinkFromPort((LinkType)linkdata, FindKeyForNode((NodeType)nodedata), (PortKey)portparam); }


    /// <summary>
    /// This method actually implements the modification of a link data
    /// to change the reference to the "to" node data and the link's "to" port information.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="nodekey">the new node data's <typeparamref name="NodeKey"/></param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinkToPath"/>
    /// or <see cref="LinkToParameterPath"/>
    /// property paths are not flexible enough or fast enough to get the node and port for a link.
    /// </remarks>
    protected virtual void ModifyLinkToPort(LinkType linkdata, NodeKey nodekey, PortKey portparam) {
      if (this.Delegates.SetLinkToPort != null) {
        this.Delegates.SetLinkToPort(this, linkdata, nodekey, portparam);
        return;
      }
      String path = this.LinkToPath;
      if (path != null && path.Length > 0) {
        _LinkToPathPPI.SetFor(linkdata, nodekey);
      } else {
        ModelHelper.Error(this, "Override ModifyLinkToPort to support relinking existing link data");
      }
      String ppath = this.LinkToParameterPath;
      if (ppath != null && ppath.Length > 0) {
        _LinkToParameterPathPPI.SetFor(linkdata, portparam);
      } else {
        //ModelHelper.Error(this, "Override ModifyLinkToPort to support relinking existing link data");
      }
    }

    /// <summary>
    /// Change a link data so that it refers to a different "to" node data and associated port information.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="nodekey">the new node data's <typeparamref name="NodeKey"/></param>
    /// <param name="portparam"></param>
    /// <remarks>
    /// This calls <see cref="ModifyLinkToPort"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetLinkToPort(LinkType linkdata, NodeKey nodekey, PortKey portparam) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetLinkToPort");
      ModifyLinkToPort(linkdata, nodekey, portparam);
    }
    void ILinksModel.SetLinkToPort(Object linkdata, Object nodedata, Object portparam) { SetLinkToPort((LinkType)linkdata, FindKeyForNode((NodeType)nodedata), (PortKey)portparam); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that its reference to a containing group node data (if any) is the given <paramref name="groupkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="groupkey">the key value of the "group" node data</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="GroupNodePath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyGroupNodeKey(NodeType nodedata, NodeKey groupkey) {
      if (this.Delegates.SetGroupNodeKey != null) {
        this.Delegates.SetGroupNodeKey(this, nodedata, groupkey);
        return;
      }
      String path = this.GroupNodePath;
      if (path != null && path.Length > 0) {
        _GroupNodePathPPI.SetFor(nodedata, groupkey);
      } else {
        ModelHelper.Error(this, "Override ModifyGroupNodeKey to support changing group membership of existing node data");
      }
    }

    /// <summary>
    /// Change a node data so that it refers to a different container group node data, by node key.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="groupkey">the key value of the new "group" node data</param>
    /// <remarks>
    /// This calls <see cref="ModifyGroupNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetGroupNodeKey(NodeType nodedata, NodeKey groupkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetGroupNodeKey");
      ModifyGroupNodeKey(nodedata, groupkey);
    }
    void ISubGraphModel.SetGroupNode(Object nodedata, Object groupnodedata) { SetGroupNodeKey((NodeType)nodedata, FindKeyForNode((NodeType)groupnodedata)); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of member node data includes a given <paramref name="memberkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="memberkey">the key value of the new "member" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindMemberNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void InsertMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (this.Delegates.InsertMemberNodeKey != null) {
        this.Delegates.InsertMemberNodeKey(this, nodedata, memberkey);
        return;
      }
      if (memberkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
      if (memberkeys == null) {
        ModelHelper.Error(this, "Override InsertMemberNodeKey to support adding node key to the MemberNodeKeys of node data, which is now null");
      }
      System.Collections.IList list = memberkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (!list.Contains(memberkey)) {
          list.Add(memberkey);
          if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyAdded(nodedata, memberkey);
        }
      } else {
        IList<NodeKey> nlist = memberkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (!nlist.Contains(memberkey)) {
            nlist.Add(memberkey);
            if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyAdded(nodedata, memberkey);
          }
        } else {
          //?? what about other types that offer the Add method?
          ModelHelper.Error(this, "Override InsertMemberNodeKey to support adding node key to the MemberNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Add a "member" node data's key value to a node data's list of "members".
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="memberkey">the key value of the new "member" node data</param>
    /// <remarks>
    /// This calls <see cref="InsertMemberNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void AddMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot AddMemberNodeKey");
      InsertMemberNodeKey(nodedata, memberkey);
    }
    void IGroupsModel.AddMemberNodeKey(Object nodedata, Object memberkey) { AddMemberNodeKey((NodeType)nodedata, (NodeKey)memberkey); }


    /// <summary>
    /// This method actually implements the modification of a node data
    /// so that it's collection of children node data does not include a given <paramref name="memberkey"/>.
    /// </summary>
    /// <param name="nodedata">the node data to be modified; if null, this method does nothing</param>
    /// <param name="memberkey">the key value of the "member" node data; if null, this method does nothing</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="FindMemberNodeKeysForNode"/>
    /// sequence is not an <see cref="System.Collections.IList"/> or an <see cref="IList{NodeType}"/>.
    /// </remarks>
    protected virtual void DeleteMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (this.Delegates.DeleteMemberNodeKey != null) {
        this.Delegates.DeleteMemberNodeKey(this, nodedata, memberkey);
        return;
      }
      if (memberkey == null) return;
      if (nodedata == null) return;
      System.Collections.IEnumerable memberkeys = FindMemberNodeKeysForNode(nodedata);
      System.Collections.IList list = memberkeys as System.Collections.IList;
      if (list != null && !list.IsReadOnly && !list.IsFixedSize) {
        if (list.Contains(memberkey)) {
          list.Remove(memberkey);
          if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyRemoved(nodedata, memberkey);
        }
      } else {
        IList<NodeKey> nlist = memberkeys as IList<NodeKey>;
        if (nlist != null && !nlist.IsReadOnly) {
          if (nlist.Contains(memberkey)) {
            nlist.Remove(memberkey);
            if (!(memberkeys is INotifyCollectionChanged)) DoMemberNodeKeyRemoved(nodedata, memberkey);
          }
        } else {
          //?? what about other types that offer the Remove method?
          ModelHelper.Error(this, "Override DeleteMemberNodeKey to support removing node key from the MemberNodeKeys of node data, which does not implement IList");
        }
      }
    }

    /// <summary>
    /// Remove a child node data's key value from a group node data's list of "member" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="memberkey">the key value of the "member" node data</param>
    /// <remarks>
    /// This calls <see cref="DeleteMemberNodeKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void RemoveMemberNodeKey(NodeType nodedata, NodeKey memberkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot RemoveMemberNodeKey");
      DeleteMemberNodeKey(nodedata, memberkey);
    }
    void IGroupsModel.RemoveMemberNodeKey(Object nodedata, Object memberkey) { RemoveMemberNodeKey((NodeType)nodedata, (NodeKey)memberkey); }


    /// <summary>
    /// This method actually implements the replacement of a group node data's
    /// collection of member node keys.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="membernodekeys">a sequence of "member" node data key values</param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="MemberNodesPath"/>
    /// property path is not flexible enough or fast enough to get the key for a node.
    /// </remarks>
    protected virtual void ModifyMemberNodeKeys(NodeType nodedata, System.Collections.IEnumerable membernodekeys) {
      if (this.Delegates.SetMemberNodeKeys != null) {
        this.Delegates.SetMemberNodeKeys(this, nodedata, membernodekeys);
        return;
      }
      String path = this.MemberNodesPath;
      if (path != null && path.Length > 0) {
        _MemberNodesPathPPI.SetFor(nodedata, membernodekeys);
      } else {
        ModelHelper.Error(this, "Override ModifyMemberNodeKeys to support reconnecting existing node data");
      }
    }

    /// <summary>
    /// Replace a node data's list of "member" key values.
    /// </summary>
    /// <param name="nodedata">the node data to be modified</param>
    /// <param name="membernodekeys">a sequence of "member" node data key values</param>
    /// <remarks>
    /// This calls <see cref="ModifyMemberNodeKeys"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetMemberNodeKeys(NodeType nodedata, System.Collections.IEnumerable membernodekeys) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetMemberNodeKeys");
      ModifyMemberNodeKeys(nodedata, membernodekeys);
    }
    void IGroupsModel.SetMemberNodeKeys(Object nodedata, System.Collections.IEnumerable memberkeys) { SetMemberNodeKeys((NodeType)nodedata, memberkeys); }


    private void SetLinkGroupKey(LinkType linkdata, NodeKey groupkey) {
      // not needed to be protected/public, since once set it's supposed to be unchangeable
    }

    /// <summary>
    /// This method actually implements the modification of a link data
    /// to change the reference to the "label" node data.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="linklabelkey">the new label data's <typeparamref name="NodeKey"/></param>
    /// <remarks>
    /// This method can be overridden in case the <see cref="LinkLabelNodePath"/>
    /// property path is not flexible enough or fast enough to get the node and port for a link.
    /// </remarks>
    protected virtual void ModifyLinkLabelKey(LinkType linkdata, NodeKey linklabelkey) {
      if (this.Delegates.SetLinkLabelKey != null) {
        this.Delegates.SetLinkLabelKey(this, linkdata, linklabelkey);
        return;
      }
      String path = this.LinkLabelNodePath;
      if (path != null && path.Length > 0) {
        _LinkLabelNodePathPPI.SetFor(linkdata, linklabelkey);
      } else {
        ModelHelper.Error(this, "Override ModifyLinkLabelKey to support changing a link data's reference to a link label node");
      }
    }

    /// <summary>
    /// Change a link data so that it refers to a different "label" node data.
    /// </summary>
    /// <param name="linkdata">the link data to be modified</param>
    /// <param name="linklabelkey">the new label data's <typeparamref name="NodeKey"/></param>
    /// <remarks>
    /// This calls <see cref="ModifyLinkLabelKey"/>
    /// This is an error if <see cref="DiagramModel.Modifiable"/> is false.
    /// </remarks>
    public void SetLinkLabelKey(LinkType linkdata, NodeKey linklabelkey) {
      if (!this.Modifiable) ModelHelper.Error(this, "Model is not modifiable -- cannot SetLinkLabelKey");
      ModifyLinkLabelKey(linkdata, linklabelkey);
    }
    void ILinksModel.SetLinkLabel(Object linkdata, Object labelnodedata) { SetLinkLabelKey((LinkType)linkdata, FindKeyForNode((NodeType)labelnodedata)); }


    // ValidCycle

    /// <summary>
    /// Specify what kinds of graphs this model allows.
    /// </summary>
    /// <remarks>
    /// Changing this property will not remove or add any links,
    /// but will only affect future attempts to add or modify links
    /// by affecting the results of calling <see cref="CheckLinkValid"/>.
    /// </remarks>
    public ValidCycle ValidCycle {
      get { return _ValidCycle; }
      set {
        ValidCycle old = _ValidCycle;
        if (old != value) {
          _ValidCycle = value;
          RaiseModelChanged(ModelChange.ChangedValidCycle, null, old, value);
        }
      }
    }

    /// <summary>
    /// Gets or sets whether each link data must have both ends refer to node data.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Model.ValidUnconnectedLinks.None"/>.
    /// </value>
    /// <remarks>
    /// This affects the standard behavior of <see cref="CheckLinkValid"/> and <see cref="CopyLink2"/>.
    /// </remarks>
    public ValidUnconnectedLinks ValidUnconnectedLinks {
      get { return _ValidUnconnectedLinks; }
      set {
        ValidUnconnectedLinks old = _ValidUnconnectedLinks;
        if (old != value) {
          _ValidUnconnectedLinks = value;
          RaiseModelChanged(ModelChange.ChangedValidUnconnectedLinks, null, old, value);
        }
      }
    }


    /// <summary>
    /// This predicate is true if adding a link between two nodes/ports would result in a valid graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckLinkValid"/> to do the actual graph structure check.
    /// </remarks>
    /// <seealso cref="ValidCycle"/>
    /// <seealso cref="ValidUnconnectedLinks"/>
    public bool IsLinkValid(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam) {
      return CheckLinkValid(fromdata, fromparam, todata, toparam, false, default(LinkType));
    }
    bool IDiagramModel.IsLinkValid(Object fromdata, Object fromparam, Object todata, Object toparam) {
      return CheckLinkValid((NodeType)fromdata, (PortKey)fromparam, (NodeType)todata, (PortKey)toparam, false, default(LinkType));
    }

    /// <summary>
    /// This predicate is true if replacing a link between two nodes/ports would result in a valid graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <param name="oldlinkdata">the original link data that might be modified</param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckLinkValid"/> to do the actual graph structure check.
    /// </remarks>
    /// <seealso cref="ValidCycle"/>
    /// <seealso cref="ValidUnconnectedLinks"/>
    public bool IsRelinkValid(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam, LinkType oldlinkdata) {
      return CheckLinkValid(fromdata, fromparam, todata, toparam, true, oldlinkdata);
    }
    bool ILinksModel.IsRelinkValid(Object newfromdata, Object newfromparam, Object newtodata, Object newtoparam, Object oldlinkdata) {
      return CheckLinkValid((NodeType)newfromdata, (PortKey)newfromparam, (NodeType)newtodata, (PortKey)newtoparam, true, (LinkType)oldlinkdata);
    }

    /// <summary>
    /// This predicate is true if adding a link between two nodes/ports would result in a validly structured graph.
    /// </summary>
    /// <param name="fromdata">a node key identifying the node data from which the link comes</param>
    /// <param name="fromparam">an optional value identifying which port on the "from" node the link is connected to</param>
    /// <param name="todata">a node key identify the node data to which the link goes</param>
    /// <param name="toparam">an optional value identifying which port on the "to" node the link is connected to</param>
    /// <param name="ignoreexistinglink">true if relinking; false if adding a new link</param>
    /// <param name="oldlinkdata">the original link data that might be modified, if <paramref name="ignoreexistinglink"/> is true</param>
    /// <returns>
    /// The behavior of this predicate depends on the value of <see cref="ValidCycle"/> and <see cref="ValidUnconnectedLinks"/>.
    /// </returns>
    protected virtual bool CheckLinkValid(NodeType fromdata, PortKey fromparam, NodeType todata, PortKey toparam, bool ignoreexistinglink, LinkType oldlinkdata) {
      if (this.Delegates.CheckLinkValid != null) {
        return this.Delegates.CheckLinkValid(fromdata, fromparam, todata, toparam, ignoreexistinglink, oldlinkdata);
      }
      if (this.ValidUnconnectedLinks == ValidUnconnectedLinks.None) {
        if (!IsNodeData(fromdata) || !IsNodeData(todata)) return false;
      } else {
        if (fromdata == null || todata == null) return true;
      }
      ValidCycle vc = this.ValidCycle;
      if (vc == ValidCycle.All) return true;
      NodeInfo from = FindNodeInfoForNode(fromdata);
      NodeInfo to = FindNodeInfoForNode(todata);
      LinkInfo ignore = (ignoreexistinglink ? FindLinkInfoForLink(oldlinkdata) : null);
      switch (vc) {
        case ValidCycle.NotDirected: return !MakesDirectedCycle(from, to, ignore);
        case ValidCycle.NotDirectedFast: return !MakesDirectedCycleFast(from, to, ignore);
        case ValidCycle.NotUndirected: return !MakesUndirectedCycle(from, to, ignore);
        case ValidCycle.DestinationTree: return (GetFromNodeInfos(to, ignore).FirstOrDefault() == null) && !MakesDirectedCycleFast(from, to, ignore);
        case ValidCycle.SourceTree: return (GetToNodeInfos(from, ignore).FirstOrDefault() == null) && !MakesDirectedCycleFast(from, to, ignore);
        default: return true;
      }
    }


    private static IEnumerable<NodeInfo> GetConnectedNodeInfos(NodeInfo a, LinkInfo ignore) {
      if (a.ConnectedLinkInfos == null) return Enumerable.Empty<NodeInfo>();
      return a.ConnectedLinkInfos.Where(li => li != ignore).Select(li => (li.ToNodeInfo == a ? li.FromNodeInfo : li.ToNodeInfo)).Where(ni => ni != null);
    }

    private static IEnumerable<NodeInfo> GetFromNodeInfos(NodeInfo a, LinkInfo ignore) {
      if (a.ConnectedLinkInfos == null) return Enumerable.Empty<NodeInfo>();
      return a.ConnectedLinkInfos.Where(li => li != ignore && li.ToNodeInfo == a && li.FromNodeInfo != null).Select(li => li.FromNodeInfo);
    }

    private static IEnumerable<NodeInfo> GetToNodeInfos(NodeInfo a, LinkInfo ignore) {
      if (a.ConnectedLinkInfos == null) return Enumerable.Empty<NodeInfo>();
      return a.ConnectedLinkInfos.Where(li => li != ignore && li.FromNodeInfo == a && li.ToNodeInfo != null).Select(li => li.ToNodeInfo);
    }


    private static bool MakesDirectedCycleFast(NodeInfo a, NodeInfo b, LinkInfo ignore) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      foreach (NodeInfo n in GetFromNodeInfos(a, ignore)) {
        if (n == a) continue;
        if (MakesDirectedCycleFast(n, b, ignore))
          return true;
      }
      return false;
    }


    private static bool MakesDirectedCycle(NodeInfo a, NodeInfo b, LinkInfo ignore) {
      if (a == b) return true;
      HashSet<NodeInfo> seen = new HashSet<NodeInfo>();
      seen.Add(b);
      return MakesDirectedCycle1(seen, a, b, ignore);
    }

    private static bool MakesDirectedCycle1(HashSet<NodeInfo> seen, NodeInfo a, NodeInfo b, LinkInfo ignore) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (seen.Contains(a)) return false;
      seen.Add(a);
      foreach (NodeInfo n in GetFromNodeInfos(a, ignore)) {
        if (n == a) continue;
        if (MakesDirectedCycle1(seen, n, b, ignore))
          return true;
      }
      return false;
    }


    private static bool MakesUndirectedCycle(NodeInfo a, NodeInfo b, LinkInfo ignore) {
      if (a == b) return true;
      HashSet<NodeInfo> seen = new HashSet<NodeInfo>();
      seen.Add(b);
      return MakesUndirectedCycle1(seen, a, b, ignore);
    }

    private static bool MakesUndirectedCycle1(HashSet<NodeInfo> seen, NodeInfo a, NodeInfo b, LinkInfo ignore) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (seen.Contains(a)) return false;
      seen.Add(a);
      foreach (NodeInfo n in GetConnectedNodeInfos(a, ignore)) {
        if (n == a) continue;
        if (MakesUndirectedCycle1(seen, n, b, ignore))
          return true;
      }
      return false;
    }


    // ValidMember

    /// <summary>
    /// This predicate is true if adding <paramref name="membernodedata"/> to a group node
    /// (<paramref name="groupnodedata"/>) would result in an invalid graph.
    /// </summary>
    /// <param name="groupnodedata">the node data that is a group</param>
    /// <param name="membernodedata">a node data</param>
    /// <param name="ignoreexistingmembership">
    /// whether to ignore the <paramref name="membernodedata"/>'s existing group membership,
    /// because that member is being transferred out of its existing group
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// This just calls <see cref="CheckMemberValid"/> to do the actual graph structure check.
    /// </remarks>
    /// <seealso cref="ValidCycle"/>
    public bool IsMemberValid(NodeType groupnodedata, NodeType membernodedata, bool ignoreexistingmembership) {
      return CheckMemberValid(groupnodedata, membernodedata, ignoreexistingmembership);
    }
    bool IGroupsModel.IsMemberValid(Object groupnodedata, Object membernodedata) { return IsMemberValid((NodeType)groupnodedata, (NodeType)membernodedata, true); }

    /// <summary>
    /// This predicate is true if adding a node to a group node would result in an invalid graph.
    /// </summary>
    /// <param name="groupnodedata">the node data that is a group</param>
    /// <param name="membernodedata">a node data to be considered for adding to the group</param>
    /// <param name="ignoreexistingmembership">
    /// whether to ignore the <paramref name="membernodedata"/>'s existing group membership,
    /// because that member is being transferred out of its existing group
    /// </param>
    /// <returns>
    /// This returns false for membership that would break the tree structure of the model's graph of groups.
    /// </returns>
    protected virtual bool CheckMemberValid(NodeType groupnodedata, NodeType membernodedata, bool ignoreexistingmembership) {
      if (this.Delegates.CheckMemberValid != null) {
        return this.Delegates.CheckMemberValid(groupnodedata, membernodedata, ignoreexistingmembership);
      }
      if (groupnodedata == null) return true;
      if (!IsNodeData(groupnodedata) || !IsNodeData(membernodedata)) return false;
      NodeInfo sg = FindNodeInfoForNode(groupnodedata);
      NodeInfo n = FindNodeInfoForNode(membernodedata);
      if (!ignoreexistingmembership && n.GroupNodeInfo != null) return false;
      return !MakesDirectedMember(sg, n);
    }


    private static bool MakesDirectedMember(NodeInfo a, NodeInfo b) {
      if (a == b) return true;
      if (a == null) return false;
      if (b == null) return false;
      if (MakesDirectedMember(a.GroupNodeInfo, b)) return true;
      return false;
    }


    // Undo/redo

    /// <summary>
    /// This is called during undo or redo to effect state changes to this model.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    protected override void ChangeModelValue(ModelChangedEventArgs e, bool undo) {
      if (this.Delegates.ChangeModelValue != null) {
        this.Delegates.ChangeModelValue(this, e, undo);
      } else {
        base.ChangeModelValue(e, undo);
      }
    }

    /// <summary>
    /// This is called during undo or redo to effect state changes to model data.
    /// </summary>
    /// <param name="e">an edit describing the change to be performed</param>
    /// <param name="undo">true if undoing; false if redoing</param>
    protected override void ChangeDataValue(ModelChangedEventArgs e, bool undo) {
      if (e == null) return;
      if (e.Data is NodeType && this.Delegates.ChangeDataValue != null) {
        this.Delegates.ChangeDataValue((NodeType)e.Data, e, undo);
      } else if (e.Data is LinkType && this.Delegates.ChangeLinkDataValue != null) {
        this.Delegates.ChangeLinkDataValue((LinkType)e.Data, e, undo);
      } else {
        base.ChangeDataValue(e, undo);
      }
    }

    internal override void ChangeModelState(ModelChangedEventArgs e, bool undo) {
      switch (e.Change) {
        // contents
        case ModelChange.AddedNode:
          if (undo) {
            DeleteNode((NodeType)e.Data);
          } else {
#if DATASET  // DataRowView
            System.Data.DataRowView row = e.Data as System.Data.DataRowView;
            Object[] cols = e.NewValue as Object[];
            if (row != null && cols != null) {
              System.Data.DataRowView newrow = AddNewRow(this.NodesSource as System.Data.DataView, cols);
              DoReplaceReference(row, newrow);
              DoNodeAdded((NodeType)((Object)newrow));
            } else
#endif
            InsertNode((NodeType)e.Data);
          }
          break;
        case ModelChange.RemovingNode:
#if DATASET  // DataRowView
          if (undo) {
            System.Data.DataRowView row = e.Data as System.Data.DataRowView;
            Object[] cols = e.OldValue as Object[];
            if (row != null && cols != null) {
              System.Data.DataRowView newrow = AddNewRow(this.NodesSource as System.Data.DataView, cols);
              DoReplaceReference(row, newrow);
              DoNodeAdded((NodeType)((Object)newrow));
            }
          }
#endif
          break;
        case ModelChange.RemovedNode:
          if (undo) {
#if DATASET  // DataRowView
            System.Data.DataRowView row = e.Data as System.Data.DataRowView;
            if (row == null)
#endif
            InsertNode((NodeType)e.Data);
          } else {
            DeleteNode((NodeType)e.Data);
          }
          break;

        case ModelChange.ChangedNodeKey: break;

        case ModelChange.AddedLink:
          if (undo)
            DeleteLink((LinkType)e.Data);
          else
            InsertLink((LinkType)e.Data);
          break;
        case ModelChange.RemovingLink: break;
        case ModelChange.RemovedLink:
          if (undo)
            InsertLink((LinkType)e.Data);
          else
            DeleteLink((LinkType)e.Data);
          break;

        case ModelChange.ChangedLinkFromPort: {
            LinkType linkdata = (LinkType)e.Data;
            if (undo)
              ModifyLinkFromPort(linkdata, (NodeKey)e.OldValue, (PortKey)e.OldParam);
            else
              ModifyLinkFromPort(linkdata, (NodeKey)e.NewValue, (PortKey)e.NewParam);
            break;
          }
        case ModelChange.ChangedLinkToPort: {
            LinkType linkdata = (LinkType)e.Data;
            if (undo)
              ModifyLinkToPort(linkdata, (NodeKey)e.OldValue, (PortKey)e.OldParam);
            else
              ModifyLinkToPort(linkdata, (NodeKey)e.NewValue, (PortKey)e.NewParam);
            break;
          }

        case ModelChange.ChangedLinkLabelKey: {
            LinkType linkdata = (LinkType)e.Data;
            ModifyLinkLabelKey(linkdata, (NodeKey)e.GetValue(undo));
            break;
          }

        case ModelChange.ChangedGroupNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyGroupNodeKey(nodedata, (NodeKey)e.GetValue(undo));
            break;
          }

        case ModelChange.ChangedMemberNodeKeys: {
            NodeType nodedata = (NodeType)e.Data;
            ModifyMemberNodeKeys(nodedata, (System.Collections.IEnumerable)e.GetValue(undo));
            break;
          }
        case ModelChange.AddedMemberNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              DeleteMemberNodeKey(nodedata, (NodeKey)e.NewValue);
            else
              InsertMemberNodeKey(nodedata, (NodeKey)e.NewValue);
            break;
          }
        case ModelChange.RemovedMemberNodeKey: {
            NodeType nodedata = (NodeType)e.Data;
            if (undo)
              InsertMemberNodeKey(nodedata, (NodeKey)e.OldValue);
            else
              DeleteMemberNodeKey(nodedata, (NodeKey)e.OldValue);
            break;
          }

        case ModelChange.ChangedLinkGroupNodeKey: {
            //LinkType linkdata = (LinkType)e.Data;
            //SetLinkGroupKey(linkdata, (NodeKey)e.GetValue(undo));
            break;
          }

        case ModelChange.ChangedNodesSource:
          this.NodesSource = (System.Collections.IEnumerable)e.GetValue(undo);
          break;

        case ModelChange.ChangedNodeCategory: break;  // handled by HandleNodePropertyChanged
        case ModelChange.ChangedLinkCategory: break;  // handled by HandleLinkPropertyChanged
        
        case ModelChange.ChangedNodeKeyPath:
          this.NodeKeyPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedNodeCategoryPath:
          this.NodeCategoryPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedNodeIsGroupPath:
          this.NodeIsGroupPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedGroupNodePath:
          this.GroupNodePath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedMemberNodesPath:
          this.MemberNodesPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedNodeIsLinkLabelPath:
          this.NodeIsLinkLabelPath = (String)e.GetValue(undo);
          break;

        case ModelChange.ChangedLinksSource:
          this.LinksSource = (System.Collections.IEnumerable)e.GetValue(undo);
          break;
        
        case ModelChange.ChangedLinkFromPath:
          this.LinkFromPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedLinkToPath:
          this.LinkToPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedLinkLabelNodePath:
          this.LinkLabelNodePath = (String)e.GetValue(undo);
          break;

        case ModelChange.ChangedLinkFromParameterPath:
          this.LinkFromParameterPath = (String)e.GetValue(undo);
          break;
        case ModelChange.ChangedLinkToParameterPath:
          this.LinkToParameterPath = (String)e.GetValue(undo);
          break;

        // model state properties
        case ModelChange.ChangedCopyingGroupCopiesMembers:
          this.CopyingGroupCopiesMembers = (bool)e.GetValue(undo);
          break;
        case ModelChange.ChangedRemovingGroupRemovesMembers:
          this.RemovingGroupRemovesMembers = (bool)e.GetValue(undo);
          break;

        case ModelChange.ChangedCopyingLinkCopiesLabel:
          this.CopyingLinkCopiesLabel = (bool)e.GetValue(undo);
          break;
        case ModelChange.ChangedRemovingLinkRemovesLabel:
          this.RemovingLinkRemovesLabel = (bool)e.GetValue(undo);
          break;

        case ModelChange.ChangedValidCycle:
          this.ValidCycle = (ValidCycle)e.GetValue(undo);
          break;
        case ModelChange.ChangedValidUnconnectedLinks:
          this.ValidUnconnectedLinks = (ValidUnconnectedLinks)e.GetValue(undo);
          break;

        default:
          base.ChangeModelState(e, undo);
          break;
      }
    }

#if DATASET
    private void DoReplaceReference(Object oldref, Object newref) {
      if (this.UndoManager != null) this.UndoManager.ReplaceReference(oldref, newref);
      RaiseModelChanged(ModelChange.ReplacedReference, null, oldref, newref);
    }
#endif

// Save and Load to/from XElement

#if !WINDOWS_UWP  // binary serialization
    public XElement Save<NodeDataType, LinkDataType>(XName rootname, XName nodename, XName linkname)
        where NodeDataType : GraphLinksModelNodeData<NodeKey>
        where LinkDataType : GraphLinksModelLinkData<NodeKey, PortKey> {
      XElement root = new XElement(rootname);
      root.Add(this.NodesSource.OfType<NodeDataType>().Select(d => d.MakeXElement(nodename)));
      root.Add(this.LinksSource.OfType<LinkDataType>().Select(l => l.MakeXElement(linkname)));
      return root;
    }
#endif

    /// <summary>
    /// Given a Linq for XML <c>XContainer</c> holding node and link data, replace this model's
    /// <see cref="NodesSource"/> and <see cref="LinksSource"/> collections with collections
    /// of new node data and new link data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a <see cref="GraphLinksModelNodeData{NodeKey}"/> with a public zero-argument constructor</typeparam>
    /// <typeparam name="LinkDataType">this must be a <see cref="GraphLinksModelLinkData{NodeKey, PortKey}"/> with a public zero-argument constructor</typeparam>
    /// <param name="root">the <c>XContainer</c> holding all of the data</param>
    /// <param name="nodename">the name of each <c>XElement</c> holding node data</param>
    /// <param name="linkname">the name of each <c>XElement</c> holding link data</param>
    /// <remarks>
    /// <para>
    /// All of the changes to this model are performed within a transaction.
    /// </para>
    /// <para>
    /// This does not set the <see cref="DiagramModel.IsModified"/> property to false.
    /// You may wish to do so, depending on your application requirements.
    /// You might also wish to clear the <see cref="UndoManager"/>.
    /// </para>
    /// </remarks>
    public void Load<NodeDataType, LinkDataType>(XContainer root, XName nodename, XName linkname)
        where NodeDataType : GraphLinksModelNodeData<NodeKey>, new()
        where LinkDataType : GraphLinksModelLinkData<NodeKey, PortKey>, new() {
      if (root == null) return;
      StartTransaction("Load");
      var nodedata = new ObservableCollection<NodeDataType>();
      foreach (XElement xe in root.Elements(nodename)) {
        NodeDataType d = new NodeDataType();
        d.LoadFromXElement(xe);
        nodedata.Add(d);
      }
      this.NodesSource = nodedata;
      var linkdata = new ObservableCollection<LinkDataType>();
      foreach (XElement xe in root.Elements(linkname)) {
        LinkDataType l = new LinkDataType();
        l.LoadFromXElement(xe);
        linkdata.Add(l);
      }
      this.LinksSource = linkdata;
      CommitTransaction("Load");
    }

    /// <summary>
    /// Given a Linq for XML <c>XContainer</c> holding node and link data, replace this model's
    /// <see cref="NodesSource"/> and <see cref="LinksSource"/> collections with collections
    /// of new node data and new link data.
    /// </summary>
    /// <typeparam name="NodeDataType">this must be a class inheriting from <see cref="GraphLinksModelNodeData{NodeKey}"/></typeparam>
    /// <typeparam name="LinkDataType">this must be a class inheriting from <see cref="GraphLinksModelLinkData{NodeKey, PortKey}"/></typeparam>
    /// <param name="root">the <c>XContainer</c> holding all of the data</param>
    /// <param name="nodedataallocator">
    /// a function that takes an <c>XElement</c> and returns either a newly constructed object of type <typeparamref name="NodeDataType"/>
    /// or null if that <c>XElement</c> is to be ignored
    /// </param>
    /// <param name="linkdataallocator">
    /// a function that takes an <c>XElement</c> and returns either a newly constructed object of type <typeparamref name="LinkDataType"/>
    /// or null if that <c>XElement</c> is to be ignored
    /// </param>
    /// <remarks>
    /// <para>
    /// This will iterate over all of the child elements of the <paramref name="root"/> container,
    /// calling <paramref name="nodedataallocator"/> on each one.
    /// If that function returns non-null, it calls <see cref="GraphLinksModelNodeData{NodeKey}.LoadFromXElement"/> on the new data
    /// and then adds it to the <see cref="NodesSource"/> collection.
    /// It will then iterate again over all of the child elements of the root container,
    /// calling <paramref name="linkdataallocator"/> on each one.
    /// If that function returns non-null, it calls <see cref="GraphLinksModelLinkData{NodeKey, PortKey}.LoadFromXElement"/> on the new data
    /// and then adds it to the <see cref="LinksSource"/> collection.
    /// </para>
    /// <para>
    /// All of the changes to this model are performed within a transaction.
    /// </para>
    /// <para>
    /// This does not set the <see cref="DiagramModel.IsModified"/> property to false.
    /// You may wish to do so, depending on your application requirements.
    /// You might also wish to clear the <see cref="UndoManager"/>.
    /// </para>
    /// </remarks>
    public void Load<NodeDataType, LinkDataType>(XContainer root,
                                                 Func<XElement, NodeDataType> nodedataallocator,
                                                 Func<XElement, LinkDataType> linkdataallocator)
        where NodeDataType : GraphLinksModelNodeData<NodeKey>
        where LinkDataType : GraphLinksModelLinkData<NodeKey, PortKey> {
      if (root == null) return;
      StartTransaction("Load");
      var nodedata = new ObservableCollection<NodeDataType>();
      foreach (XElement xe in root.Elements()) {
        NodeDataType d = nodedataallocator(xe);
        if (d == null) continue;
        d.LoadFromXElement(xe);
        nodedata.Add(d);
      }
      this.NodesSource = nodedata;
      var linkdata = new ObservableCollection<LinkDataType>();
      foreach (XElement xe in root.Elements()) {
        LinkDataType l = linkdataallocator(xe);
        if (l == null) continue;
        l.LoadFromXElement(xe);
        linkdata.Add(l);
      }
      this.LinksSource = linkdata;
      CommitTransaction("Load");
    }


    internal /*?? public */ ModelDelegates Delegates {
      get { return _Delegates; }
    }

    internal /*?? public */ sealed class ModelDelegates {  // nested class
      internal ModelDelegates() { }

      internal ModelDelegates Clone() {
        return (ModelDelegates)MemberwiseClone();
      }

      public Func<NodeType, NodeKey> FindKeyForNode { get; set; }

      public Predicate<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType> MakeNodeKeyUnique { get; set; }

      public Func<NodeType, String> FindCategoryForNode { get; set; }

      public Predicate<NodeType> FindIsGroupForNode { get; set; }

      public Func<NodeType, NodeKey> FindGroupKeyForNode { get; set; }

      public Func<NodeType, System.Collections.IEnumerable> FindMemberNodeKeysForNode { get; set; }

      public Predicate<NodeType> FindIsLinkLabelForNode { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeKey> ResolveNodeKey { get; set; }

      public Func<LinkType, NodeKey> FindFromNodeKeyForLink { get; set; }

      public Func<LinkType, PortKey> FindFromParameterForLink { get; set; }

      public Func<LinkType, NodeKey> FindToNodeKeyForLink { get; set; }

      public Func<LinkType, PortKey> FindToParameterForLink { get; set; }

      public Func<LinkType, NodeKey> FindLabelNodeKeyForLink { get; set; }

      public Func<LinkType, String> FindCategoryForLink { get; set; }

      public Predicate<PortKey, PortKey> IsEqualPortParameters { get; set; }

      public Func<NodeType, CopyDictionary, NodeType> CopyNode1 { get; set; }

      public Action<NodeType, CopyDictionary, NodeType, NodeType, IEnumerable<NodeType>> CopyNode2 { get; set; }

      public Func<LinkType, CopyDictionary, LinkType> CopyLink1 { get; set; }

      public Action<LinkType, CopyDictionary, LinkType, NodeType, NodeType, NodeType> CopyLink2 { get; set; }

      public Action<DataCollection> AugmentCopyCollection { get; set; }

      public Action<CopyDictionary> AugmentCopyDictionary { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType> InsertNode { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  LinkType> InsertLink { get; set; }

      public Func<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, PortKey, NodeType, PortKey, LinkType> InsertLink2 { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType> DeleteNode { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  LinkType> DeleteLink { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, PortKey, NodeType, PortKey> DeleteLink2 { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  LinkType, NodeKey, PortKey> SetLinkFromPort { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  LinkType, NodeKey, PortKey> SetLinkToPort { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, NodeKey> SetGroupNodeKey { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, NodeKey> InsertMemberNodeKey { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, NodeKey> DeleteMemberNodeKey { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  NodeType, System.Collections.IEnumerable> SetMemberNodeKeys { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  LinkType, NodeKey> SetLinkLabelKey { get; set; }

      public Predicate<NodeType, PortKey, NodeType, PortKey, bool, LinkType> CheckLinkValid { get; set; }

      public Predicate<NodeType, NodeType, bool> CheckMemberValid { get; set; }

      public Action<GraphLinksModel<NodeType, NodeKey, PortKey, LinkType>,  ModelChangedEventArgs, bool> ChangeModelValue { get; set; }

      public Action<NodeType, ModelChangedEventArgs, bool> ChangeDataValue { get; set; }

      public Action<LinkType, ModelChangedEventArgs, bool> ChangeLinkDataValue { get; set; }
    }

  }


  /// <summary>
  /// This enumeration controls whether link data must be connected at both "ends" to nodes.
  /// </summary>
  public enum ValidUnconnectedLinks {
    /// <summary>
    /// All link data must have both the "from" and the "to" ends connected to nodes.
    /// </summary>
    None,
    /// <summary>
    /// Link data permits having no reference to nodes for either or both ends of the link.
    /// </summary>
    Allowed
  }


  /// <summary>
  /// This is a universal model, handling all kinds of datatypes representing nodes, node keys, and links.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This assumes that each node or link can be a member of at most one subgraph node.
  /// Since it uses Object as the type for node data, this model class supports multiple instances
  /// of different (unrelated) types.
  /// </para>
  /// <para>
  /// For reasons of both compile-time type checking and run-time efficiency,
  /// we recommend defining your own model class derived from <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>.
  /// </para>
  /// <para>
  /// This defines nested classes: DataCollection and CopyDictionary.
  /// </para>
  /// </remarks>
  public sealed class UniversalGraphLinksModel : GraphLinksModel<Object, Object, Object, Object> {
    /// <summary>
    /// Create a modifiable <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}"/>
    /// with empty <c>ObservableCollection</c>s for the
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.NodesSource"/> and
    /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.LinksSource"/>.
    /// </summary>
    public UniversalGraphLinksModel() {
      this.Initializing = true;
      this.Modifiable = true;
      this.Initializing = false;
    }
  }
}
